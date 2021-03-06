using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;
using MyTime.Interfaces;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace MyTime.Controllers
{
    public class AttendanceReportController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        ApproverDBService approverDBService = new ApproverDBService();
        SystemDBService systemDBService = new SystemDBService();

        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        AttendanceDBService attendanceDBService = new AttendanceDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();

        // GET: AttendanceReport
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            SystemModel systemModel = new SystemModel();
            DateTime dataStartDate;

            systemModel = systemDBService.GetData();
            dataStartDate = systemModel.DataStartDate;

            AttendanceReportViewModel attendanceReportViewModel = new AttendanceReportViewModel();
            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                attendanceReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                //dailyAttendanceRecordViewModel.AttendanceDate = DateTime.Now;
                attendanceReportViewModel.SelectListYear = PrepareSelectYearList(dataStartDate);
                attendanceReportViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

            }

            return View(attendanceReportViewModel);
        }


        public ActionResult PrintAttendanceReport()
        {

            if (Session["OrganisationName"] != null)
            {

                List<AttendanceModel> attendanceList = new List<AttendanceModel>();
                AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

                List<CRAttendanceDailyModel> crAttendanceDailyList = new List<CRAttendanceDailyModel>();
                ApproverUserModel userApproverModel = new ApproverUserModel();

                string reportType;

                attendanceList = TempData["AttendanceList"] as List<AttendanceModel>;
                attendanceSummaryModel = TempData["AttendanceSummary"] as AttendanceSummaryModel;

                reportType = TempData["ReportType"] as string;

                TempData.Keep("AttendanceList");
                TempData.Keep("AttendanceSummary");
                TempData.Keep("ReportType");

                crAttendanceDailyList = crystalReportDBService.PrepareAttendanceReport(reportType, attendanceList.OrderBy(a => a.AttendanceDate).ThenBy(a => a.UserName).ToList(), attendanceSummaryModel);

                ReportDocument report = new ReportDocument();
                report.Load(Path.Combine(Server.MapPath("~/Reports"), "AttendanceRecordCR.rpt"));
                report.SetDataSource(crAttendanceDailyList);

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


                report.Close();
                report.Dispose();


                if (reportType.Equals("Monthly"))
                {
                    return File(stream, "application/pdf", "Laporan Kedatangan Bulanan.pdf");
                }
                else
                {
                    return File(stream, "application/pdf", "Laporan Kedatangan Tahunan.pdf");
                }
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
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


        public ActionResult GetWeek(string selectedMonth)
        {

            AttendanceReportViewModel attendanceReportViewModel = new AttendanceReportViewModel();

            DateTime monthStartOn;
            DateTime attendanceDate;

            var selectWeekList = new List<SelectListItem>();

            DateTime weekStartOn;
            DateTime weekEndOn;

            DateTime.TryParse(selectedMonth, out monthStartOn);

            int days;
            int dayOfWeek;

            if (!monthStartOn.ToString("yyyyMM").Equals(DateTime.Now.ToString("yyyyMM")))
            {
                days = DateTime.DaysInMonth(monthStartOn.Year, monthStartOn.Month);
            }
            else
            {
                days = DateTime.Now.Subtract(monthStartOn).Days;
                days += 1;

            }

            weekStartOn = monthStartOn.Date;

            for (int i = 0; i < days; i++)
            {
                attendanceDate = monthStartOn.AddDays(i);

                dayOfWeek = (int)attendanceDate.DayOfWeek;
                // 0 = sunday, 1 = monday
                switch (dayOfWeek)
                {
                    case 6:
                        {
                            weekEndOn = attendanceDate.Date;

                            selectWeekList.Add(new SelectListItem
                            {
                                Value = string.Format("{0};{1}", weekStartOn.ToString("yyyy-MM-dd"), weekEndOn.ToString("yyyy-MM-dd")),
                                Text = string.Format("{0} ~ {1}", weekStartOn.ToString("dd"), weekEndOn.ToString("dd"))
                            }); ;

                            break;
                        }
                    case 0:
                        {
                            weekStartOn = attendanceDate.Date;
                            break;
                        }
                }

                if (i == (days - 1))
                {

                    if (dayOfWeek != 6)
                    {

                        weekEndOn = attendanceDate.Date;

                        selectWeekList.Add(new SelectListItem
                        {
                            Value = string.Format("{0};{1}", weekStartOn.ToString("yyyy-MM-dd"), weekEndOn.ToString("yyyy-MM-dd")),
                            Text = string.Format("{0} ~ {1}", weekStartOn.ToString("dd"), weekEndOn.ToString("dd"))
                        }); ;

                    }

                }
            }

            attendanceReportViewModel.SelectListWeek = selectWeekList;

            return Json(attendanceReportViewModel.SelectListWeek, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult GetDepartmentUser(string reportType, string selectedDate, string selectedDepartmentID)
        {
            List<UserModel> userList = new List<UserModel>();
            AttendanceReportViewModel attendanceReportViewModel = new AttendanceReportViewModel();

            //// Get Month Start and End Date
            DateTime startOn;
            DateTime endOn;

            switch (reportType)
            {
                case "Weekly":

                    string[] tempDate = selectedDate.Split(';');

                    DateTime.TryParse(tempDate[0], out startOn);
                    DateTime.TryParse(tempDate[1], out endOn);

                    userList = userDBService.ListUser().Where(u => u.DepartmentID == selectedDepartmentID && ((u.IsResigned == true && u.ResignedOn > startOn.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();
                    break;

                case "Monthly":

                    DateTime.TryParse(selectedDate, out startOn);

                    int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
                    endOn = startOn.AddDays((days - 1));

                    userList = userDBService.ListUser().Where(u => u.DepartmentID == selectedDepartmentID && ((u.IsResigned == true && u.ResignedOn >= startOn.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();
                    break;

                case "Yearly":

                    DateTime.TryParse(selectedDate, out startOn);
                    endOn = startOn.AddYears(1).AddDays(-1);

                    userList = userDBService.ListUser().Where(u => u.DepartmentID == selectedDepartmentID && ((u.IsResigned == true && u.ResignedOn >= startOn.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();
                    break;
            }

            TempData["UserList"] = userList;

            attendanceReportViewModel.SelectListUser = PrepareSelectUserList(userList);
            return Json(attendanceReportViewModel.SelectListUser, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateAttendanceList(string reportType, string selectedDate, string selectedDepartmentID, string selectedNRIC)
        {
            List<UserModel> userList = new List<UserModel>();

            // Get Month Start and End Date
            DateTime startOn;
            DateTime endOn;

            int days;

            List<UserModel> selectedUser = new List<UserModel>();
            List<ApproverUserModel> userApproverList = new List<ApproverUserModel>();
            ApproverUserModel userApproverModel = new ApproverUserModel();

            List<AttendanceModel> tempList;
            List<AttendanceModel> attendanceList = new List<AttendanceModel>();

            string approverName;

            userList = TempData["UserList"] as List<UserModel>;
            TempData.Keep("UserList");

            TempData["ReportType"] = reportType;

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

                    if (!selectedNRIC.Equals(""))
                    {
                        // By User
                        selectedUser = userList.Where(u => u.NRIC == selectedNRIC).ToList();
                    }
                    else
                    {
                        // By Department
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

                        if (!selectedNRIC.Equals(""))
                        {
                            // By User
                            selectedUser = userList.Where(u => u.NRIC == selectedNRIC).ToList();
                        }
                        else
                        {
                            // By Department
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

            TempData["AttendanceList"] = attendanceList;

            return Json(attendanceList.OrderBy(a => a.AttendanceDate).ThenBy(a => a.UserName).ToList(), JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetAttendanceSummary()
        {
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            List<AttendanceModel> attendanceList = new List<AttendanceModel>();
            attendanceList = TempData["AttendanceList"] as List<AttendanceModel>;

            TempData.Keep("AttendanceList");

            int totalLateIn = 0;
            int totalEarlyOut = 0;
            int totalLateInEarlyOut = 0;
            int totalIncomplete = 0;
            int totalAbsent = 0;
            int totalAttend = 0;
            int totalOnLeave = 0;

            totalLateIn = attendanceList.Where(a => a.AttendanceStatusID == "LIN" && a.IsApproved == false).Count();
            totalEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "EOT" && a.IsApproved == false).Count();
            totalLateInEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "L/E" && a.IsApproved == false).Count();
            totalIncomplete = attendanceList.Where(a => a.AttendanceStatusID == "ICP" && a.IsApproved == false).Count();
            totalAbsent = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == false).Count();
            totalAttend = attendanceList.Where(a => a.AttendanceStatusID == "NOR" || (a.IsApproved == true && a.IsForOnLeave == false)).Count();
            totalOnLeave = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == true && a.IsForOnLeave == true).Count();

            attendanceSummaryModel.TotalLateIn = totalLateIn;
            attendanceSummaryModel.TotalEarlyOut = totalEarlyOut;
            attendanceSummaryModel.TotalLateInEarlyOut = totalLateInEarlyOut;
            attendanceSummaryModel.TotalIncomplete = totalIncomplete;
            attendanceSummaryModel.TotalAbsent = totalAbsent;
            attendanceSummaryModel.TotalAttend = totalAttend;
            attendanceSummaryModel.TotalOnLeave = totalOnLeave;

            TempData["AttendanceList"] = attendanceList;
            TempData["AttendanceSummary"] = attendanceSummaryModel;

            return Json(attendanceSummaryModel, JsonRequestBehavior.AllowGet);
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


        private IEnumerable<SelectListItem> PrepareSelectUserList(List<UserModel> userList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in userList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.NRIC.ToString(),
                    Text = row.UserName.ToString()
                });
            }
            return selectList;
        }

    }
}