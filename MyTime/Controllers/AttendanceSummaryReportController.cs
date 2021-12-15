using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace MyTime.Controllers
{
    public class AttendanceSummaryReportController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        ApproverDBService approverDBService = new ApproverDBService();
        SystemDBService systemDBService = new SystemDBService();

        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        AttendanceDBService attendanceDBService = new AttendanceDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();

        // GET: AttendanceSummaryReport
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            SystemModel systemModel = new SystemModel();
            DateTime dataStartDate;

            systemModel = systemDBService.GetData();
            dataStartDate = systemModel.DataStartDate;

            AttendanceSummaryReportViewModel attendanceSummaryReportViewModel = new AttendanceSummaryReportViewModel();
            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                attendanceSummaryReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                //dailyAttendanceRecordViewModel.AttendanceDate = DateTime.Now;
                attendanceSummaryReportViewModel.SelectListYear = PrepareSelectYearList(dataStartDate);
                attendanceSummaryReportViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

                TempData["ReportAdminDepartmentList"] = reportAdminDepartmentList;

            }

            return View(attendanceSummaryReportViewModel);
        }


        private IEnumerable<SelectListItem> PrepareSelectYearList(DateTime dataStartDate)
        {
            var selectYearList = new List<SelectListItem>();
            int currentYear = DateTime.Now.Year;

            if (!currentYear.Equals(dataStartDate.Year))
            {
                for (int year = dataStartDate.Year; year <= currentYear; year++)
                {
                    selectYearList.Add(new SelectListItem
                    {
                        Value = string.Format("{0}-{1}-{2}", year.ToString(), "01", "01"),
                        Text = year.ToString()
                    });
                }
            }
            else
            {
                selectYearList.Add(new SelectListItem
                {
                    Value = dataStartDate.ToString("yyyy-MM-dd"),
                    Text = dataStartDate.Year.ToString()
                });
            }

            return selectYearList;
        }

        private IEnumerable<SelectListItem> PrepareSelectDepartmentList(List<ReportAdminDepartmentModel> reportAdminDepartmentList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in reportAdminDepartmentList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.DepartmentID.ToString(),
                    Text = row.DepartmentName.ToString()
                });
            }
            return selectList;
        }


        public ActionResult GetMonth(string selectedYear)
        {
            AttendanceReportViewModel attendanceReportViewModel = new AttendanceReportViewModel();

            SystemModel systemModel = new SystemModel();
            DateTime dataStartDate;

            systemModel = systemDBService.GetData();
            dataStartDate = systemModel.DataStartDate;

            int currentYear = DateTime.Now.Year;

            DateTime yearStartOn;
            DateTime.TryParse(selectedYear, out yearStartOn);

            int totalMonth;
            string tempDate;
            DateTime monthStartOn;

            var selectMonthList = new List<SelectListItem>();

            if (!yearStartOn.Year.Equals(currentYear))
            {
                totalMonth = 12;

                for (int i = 0; i < totalMonth; i++)
                {

                    tempDate = string.Format("{0}-{1}-{2}", yearStartOn.ToString("yyyy"), (i + 1).ToString("00"), "01");
                    DateTime.TryParse(tempDate, out monthStartOn);

                    if (monthStartOn.Year == dataStartDate.Year)
                    {

                        if (monthStartOn.Month >= dataStartDate.Month)
                        {
                            selectMonthList.Add(new SelectListItem
                            {
                                Value = monthStartOn.ToString("yyyy-MM-dd"),
                                Text = monthStartOn.ToString("MMM")
                            });


                        }

                    }
                    else
                    {
                        selectMonthList.Add(new SelectListItem
                        {
                            Value = monthStartOn.ToString("yyyy-MM-dd"),
                            Text = monthStartOn.ToString("MMM")
                        });
                    }


                }

            }
            else
            {
                //  current Year
                totalMonth = (DateTime.Now.Month - yearStartOn.Month) + 1;

                for (int i = 0; i < totalMonth; i++)
                {

                    if (i == 0)
                    {
                        monthStartOn = yearStartOn.AddMonths(i);
                    }
                    else
                    {

                        tempDate = string.Format("{0}-{1}-{2}", yearStartOn.ToString("yyyy"), yearStartOn.AddMonths(i).ToString("MM"), "01");
                        DateTime.TryParse(tempDate, out monthStartOn);
                    }

                    selectMonthList.Add(new SelectListItem
                    {
                        Value = monthStartOn.ToString("yyyy-MM-dd"),
                        Text = monthStartOn.ToString("MMM")
                    });

                }
            }

            attendanceReportViewModel.SelectListMonth = selectMonthList;
            return Json(attendanceReportViewModel.SelectListMonth, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GenerateAttendanceList(string reportType, string selectedDate, string selectedDepartmentID)
        {
            List<UserModel> userList = new List<UserModel>();

            // Get Month Start and End Date
            DateTime startOn;
            DateTime endOn;

            DateTime reportDate;

            int days;

            List<UserModel> selectedUser = new List<UserModel>();
            List<ApproverUserModel> userApproverList = new List<ApproverUserModel>();
            ApproverUserModel userApproverModel = new ApproverUserModel();

            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            List<AttendanceModel> tempList = new List<AttendanceModel>();
            List<AttendanceModel> attendanceList = new List<AttendanceModel>();

            AttendanceSummaryReportModel attendanceSummaryReport = new AttendanceSummaryReportModel();
            List<AttendanceSummaryReportModel> attendanceSummaryReportList;

            string approverName;

            string departmentID;
            string departmentName;
                        
            //userList = TempData["UserList"] as List<UserModel>;
            //TempData.Keep("UserList");

            TempData["ReportType"] = reportType;
            DateTime.TryParse(selectedDate, out reportDate);

            switch (reportType)
            {
                //case "Weekly":

                //    string[] tempDate = selectedDate.Split(';');

                //    DateTime.TryParse(tempDate[0], out startOn);
                //    DateTime.TryParse(tempDate[1], out endOn);                    
                //    break;

                case "Monthly":

                    DateTime.TryParse(selectedDate, out startOn);
                  
                    days = DateTime.DaysInMonth(startOn.Year, startOn.Month);

                    if (startOn.ToString("yyyyMM") != DateTime.Now.ToString("yyyyMM"))
                    {
                        endOn = startOn.AddDays((days - 1));
                    }
                    else
                    {
                        endOn = DateTime.Now.Date;
                    }
                                        // include those resigned after attendance date
                    userList = userDBService.ListUser().Where(u => ((u.IsResigned == true && u.ResignedOn > startOn.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();

                    if (!selectedDepartmentID.Equals(""))
                    {
                        // By Single Department
                        selectedUser = userList.Where(u => u.DepartmentID == selectedDepartmentID).ToList();
                    }
                    else
                    {
                        // By All Department
                        selectedUser = userList.ToList();                  
                    }

                    // Get Attendance
                    attendanceList = new List<AttendanceModel>();

                    foreach (var row in selectedUser)
                    {
                        approverName = "";

                        userApproverList = new List<ApproverUserModel>();

                        userApproverList = approverDBService.GetUserApprover(row.NRIC);

                        if (userApproverList.Count > 0)
                        {
                            userApproverModel = userApproverList.Select(ua => ua).FirstOrDefault();
                            approverName = userApproverModel.UserName;
                        }

                        tempList = new List<AttendanceModel>();

                        tempList = attendanceDBService.GetMonthlyAttendance(row.NRIC, row.USRID, row.UserName, row.DepartmentID, row.DepartmentName, startOn, endOn, row.AccessRoleID, approverName).Where(t => t.AttendanceStatus != "").ToList();

                        attendanceList.AddRange(tempList);
                    }

                    break;

                case "Yearly":

                    DateTime.TryParse(selectedDate, out startOn);
                 
                    // Get Attendance
                    attendanceList = new List<AttendanceModel>();

                    // 12 months or until current month
                    for (int i = 0; i < 12; i++)
                    {

                        if (startOn.ToString("yyyyMM") != DateTime.Now.ToString("yyyyMM"))
                        {
                            days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
                            endOn = startOn.AddDays((days - 1));
                        }
                        else
                        {
                            endOn = DateTime.Now.Date;

                        }

                        // include those resigned after attendance date
                        userList = userDBService.ListUser().Where(u => ((u.IsResigned == true && u.ResignedOn > startOn.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();

                        if (!selectedDepartmentID.Equals(""))
                        {
                            // By Single Department
                            selectedUser = userList.Where(u => u.DepartmentID == selectedDepartmentID).ToList();
                        }
                        else
                        {
                            // By All Department
                            selectedUser = userList.ToList();                         
                        }

                        foreach (var row in selectedUser)
                        {
                            approverName = "";

                            userApproverList = new List<ApproverUserModel>();

                            userApproverList = approverDBService.GetUserApprover(row.NRIC);

                            if (userApproverList.Count > 0)
                            {
                                userApproverModel = userApproverList.Select(ua => ua).FirstOrDefault();
                                approverName = userApproverModel.UserName;
                            }

                            tempList = new List<AttendanceModel>();

                            tempList = attendanceDBService.GetMonthlyAttendance(row.NRIC, row.USRID, row.UserName, row.DepartmentID, row.DepartmentName, startOn, endOn, row.AccessRoleID, approverName).Where(t => t.AttendanceStatus != "").ToList(); ;

                            attendanceList.AddRange(tempList);
                        }


                        if (startOn.ToString("yyyyMM") != DateTime.Now.ToString("yyyyMM"))
                        {
                            startOn = startOn.AddMonths(1);
                        }
                        else
                        {
                            break;
                        }

                    }

                    break;
            }

            reportAdminDepartmentList = TempData["ReportAdminDepartmentList"] as List<ReportAdminDepartmentModel>;
            TempData.Keep("ReportAdminDepartmentList");

            attendanceSummaryReportList = new List<AttendanceSummaryReportModel>();

            int userCount = 0;
            int totalLateIn = 0;
            int totalEarlyOut = 0;
            int totalLateInEarlyOut = 0;
            int totalIncomplete = 0;
            int totalAbsent = 0;
            int totalAttend = 0;
            int totalOnLeave = 0;

            if (!selectedDepartmentID.Equals(""))
            {
                // By Single Department
                            

                departmentID = selectedDepartmentID;

                departmentName = reportAdminDepartmentList.Where(rad => rad.DepartmentID == departmentID).Select(rad => rad.DepartmentName).FirstOrDefault();

                selectedUser = userList.Where(u => u.DepartmentID == departmentID).ToList();
                userCount = selectedUser.Count();

                totalLateIn = attendanceList.Where(a => a.AttendanceStatusID == "LIN" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                totalEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "EOT" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                totalLateInEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "L/E" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                totalIncomplete = attendanceList.Where(a => a.AttendanceStatusID == "ICP" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                totalAbsent = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                totalAttend = attendanceList.Where(a => a.AttendanceStatusID == "NOR" || (a.IsApproved == true && a.IsForOnLeave == false) && a.DepartmentID == departmentID).Count();
                totalOnLeave = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == true && a.IsForOnLeave == true && a.DepartmentID == departmentID).Count();

                attendanceSummaryReport = new AttendanceSummaryReportModel();

                attendanceSummaryReport.ReportDate= reportDate;
                attendanceSummaryReport.DepartmentName = departmentName;
                attendanceSummaryReport.UserCount = userCount;
                attendanceSummaryReport.TotalLateIn = totalLateIn;
                attendanceSummaryReport.TotalEarlyOut = totalEarlyOut;
                attendanceSummaryReport.TotalLateInEarlyOut = totalLateInEarlyOut;
                attendanceSummaryReport.TotalIncomplete = totalIncomplete;
                attendanceSummaryReport.TotalAbsent = totalAbsent;
                attendanceSummaryReport.TotalAttend = totalAttend;
                attendanceSummaryReport.TotalOnLeave = totalOnLeave;

                attendanceSummaryReportList.Add(attendanceSummaryReport);

            }
            else
            {
                // By All Department

                foreach (var row in reportAdminDepartmentList)
                {
                    departmentID = row.DepartmentID;
                    departmentName = row.DepartmentName;

                    selectedUser = userList.Where(u => u.DepartmentID == departmentID).ToList();
                    userCount = selectedUser.Count();

                    totalLateIn = attendanceList.Where(a => a.AttendanceStatusID == "LIN" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                    totalEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "EOT" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                    totalLateInEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "L/E" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                    totalIncomplete = attendanceList.Where(a => a.AttendanceStatusID == "ICP" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                    totalAbsent = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == false && a.DepartmentID == departmentID).Count();
                    totalAttend = attendanceList.Where(a => a.AttendanceStatusID == "NOR" || (a.IsApproved == true && a.IsForOnLeave == false) && a.DepartmentID == departmentID).Count();
                    totalOnLeave = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == true && a.IsForOnLeave == true && a.DepartmentID == departmentID).Count();

                    attendanceSummaryReport = new AttendanceSummaryReportModel();

                    attendanceSummaryReport.ReportDate = reportDate;
                    attendanceSummaryReport.DepartmentName = departmentName;
                    attendanceSummaryReport.UserCount = userCount;
                    attendanceSummaryReport.TotalLateIn = totalLateIn;
                    attendanceSummaryReport.TotalEarlyOut = totalEarlyOut;
                    attendanceSummaryReport.TotalLateInEarlyOut = totalLateInEarlyOut;
                    attendanceSummaryReport.TotalIncomplete = totalIncomplete;
                    attendanceSummaryReport.TotalAbsent = totalAbsent;
                    attendanceSummaryReport.TotalAttend = totalAttend;
                    attendanceSummaryReport.TotalOnLeave = totalOnLeave;

                    attendanceSummaryReportList.Add(attendanceSummaryReport);

                }
            }

            TempData["SelectedDepartmentID"] = selectedDepartmentID;
            TempData["AttendanceSummaryReportList"] = attendanceSummaryReportList;

            return Json(attendanceSummaryReportList.OrderBy(a => a.DepartmentName).ToList(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetAttendanceSummary()
        {
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            List<AttendanceSummaryReportModel> attendanceSummaryReportList = new List<AttendanceSummaryReportModel>();
                       
            attendanceSummaryReportList = TempData["AttendanceSummaryReportList"] as List<AttendanceSummaryReportModel>;

            int totalLateIn = 0;
            int totalEarlyOut = 0;
            int totalLateInEarlyOut = 0;
            int totalIncomplete = 0;
            int totalAbsent = 0;
            int totalAttend = 0;
            int totalOnLeave = 0;

            totalLateIn = attendanceSummaryReportList.Select(a=> a.TotalLateIn).Sum();
            totalEarlyOut = attendanceSummaryReportList.Select(a => a.TotalEarlyOut).Sum();
            totalLateInEarlyOut = attendanceSummaryReportList.Select(a => a.TotalLateInEarlyOut).Sum();
            totalIncomplete = attendanceSummaryReportList.Select(a => a.TotalIncomplete).Sum();
            totalAbsent = attendanceSummaryReportList.Select(a => a.TotalAbsent).Sum();
            totalAttend = attendanceSummaryReportList.Select(a => a.TotalAttend).Sum();
            totalOnLeave = attendanceSummaryReportList.Select(a => a.TotalOnLeave).Sum();

            attendanceSummaryModel.TotalLateIn = totalLateIn;
            attendanceSummaryModel.TotalEarlyOut = totalEarlyOut;
            attendanceSummaryModel.TotalLateInEarlyOut = totalLateInEarlyOut;
            attendanceSummaryModel.TotalIncomplete = totalIncomplete;
            attendanceSummaryModel.TotalAbsent = totalAbsent;
            attendanceSummaryModel.TotalAttend = totalAttend;
            attendanceSummaryModel.TotalOnLeave = totalOnLeave;

            TempData.Keep("AttendanceSummaryReportList");
            TempData["AttendanceSummary"] = attendanceSummaryModel;

            return Json(attendanceSummaryModel, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintAttendanceSummaryReport()
        {
            string selectedDepartmentID;
            string reportType;
            List<AttendanceSummaryReportModel> attendanceSummaryReportList = new List<AttendanceSummaryReportModel>();
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            List<CRAttendanceSummaryReportModel> crAttendanceSummaryReportList = new List<CRAttendanceSummaryReportModel>();

            selectedDepartmentID = TempData["SelectedDepartmentID"] as string;
            reportType = TempData["ReportType"] as string;

            attendanceSummaryReportList = TempData["AttendanceSummaryReportList"] as List<AttendanceSummaryReportModel>;
            attendanceSummaryModel = TempData["AttendanceSummary"] as AttendanceSummaryModel;

            TempData.Keep("SelectedDepartmentID");

            TempData.Keep("ReportType");
            TempData.Keep("AttendanceSummaryReportList");
            TempData.Keep("AttendanceSummary");

            crAttendanceSummaryReportList = crystalReportDBService.PrepareAttendanceSummaryReport(selectedDepartmentID, reportType, attendanceSummaryReportList, attendanceSummaryModel);

            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports"), "AttendanceSummaryCR.rpt"));
            report.SetDataSource(crAttendanceSummaryReportList);

            //report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
            //report.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            //report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            string organisationName = Session["OrganisationName"].ToString();
            string organisationLogo = Session["OrganisationLogo"].ToString();
            string organisationLogoPath = Path.Combine(Server.MapPath("~/Images"), organisationLogo);

            report.SetParameterValue("Language", System.Globalization.CultureInfo.CurrentCulture.Name.ToString());
            report.SetParameterValue("OrganisationName", organisationName);
            report.SetParameterValue("OrganisationLogo", organisationLogoPath);

            Response.Buffer = false;
            Response.ClearContent();

            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Rumusan Laporan Kedatangan.pdf");

        }

    }
}