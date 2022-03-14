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
    public class AttendanceAnalysisController : EnvironmentController
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

            AttendanceAnalysisViewModel attendanceAnalysisViewModel = new AttendanceAnalysisViewModel();
            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                attendanceAnalysisViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                //dailyAttendanceRecordViewModel.AttendanceDate = DateTime.Now;
                attendanceAnalysisViewModel.SelectListYear = PrepareSelectYearList(dataStartDate);
                attendanceAnalysisViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);
                attendanceAnalysisViewModel.SelectListOperand = PrepareSelectOperand();
                attendanceAnalysisViewModel.SelectListNumberOfDay = PrepareSelectNumberOfDay();
            }

            return View(attendanceAnalysisViewModel);
        }


        public ActionResult PrintAttendanceAnalysis()
        {
            List<AttendanceAnalysisModel> attendanceAnalysisList = new List<AttendanceAnalysisModel>();
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            List<CRAttendanceAnalysisModel> crAttendanceAnalysisList = new List<CRAttendanceAnalysisModel>();

            string reportType;

            attendanceAnalysisList = TempData["AttendanceAnalysisList"] as List<AttendanceAnalysisModel>;
            attendanceSummaryModel = TempData["AttendanceSummary"] as AttendanceSummaryModel;

            reportType = TempData["ReportType"] as string;

            TempData.Keep("AttendanceAnalysisList");
            TempData.Keep("AttendanceSummary");
            TempData.Keep("ReportType");

            crAttendanceAnalysisList = crystalReportDBService.PrepareAttendanceAnalysis(reportType, attendanceAnalysisList.OrderBy(a => a.UserName).ToList(), attendanceSummaryModel);

            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports"), "AttendanceAnalysisCR.rpt"));
            report.SetDataSource(crAttendanceAnalysisList);

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
                return File(stream, "application/pdf", "Laporan Analisis Kedatangan Bulanan.pdf");
            }
            else
            {
                return File(stream, "application/pdf", "Laporan Analisis Kedatangan Tahunan.pdf");
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

        private IEnumerable<SelectListItem> PrepareSelectOperand()
        {
            var selectOperandList = new List<SelectListItem>();

            selectOperandList.Add(new SelectListItem
            {
                Value = "<",
                Text = "<"
            });

            selectOperandList.Add(new SelectListItem
            {
                Value = "<=",
                Text = "<="
            });

            selectOperandList.Add(new SelectListItem
            {
                Value = "=",
                Text = "="
            });

            selectOperandList.Add(new SelectListItem
            {
                Value = ">=",
                Text = ">="
            });

            selectOperandList.Add(new SelectListItem
            {
                Value = ">",
                Text = ">"
            });

            return selectOperandList;
        }

        private IEnumerable<SelectListItem> PrepareSelectNumberOfDay()
        {
            var selectOperandList = new List<SelectListItem>();

            for (int i=0; i < 365; i++)
            {
                selectOperandList.Add(new SelectListItem
                {
                    Value = (i+1).ToString(),
                    Text = (i+1).ToString()
                });
            }

            return selectOperandList;
        }


        public ActionResult GenerateAttendanceList(string reportType, string selectedDate, string selectedDepartmentID, string selectedNRIC, 
            string includedLateIn, string includedEarlyOut, string includedLateInEarlyOut, string includedIncomplete, string includedAbsent, 
            string includedAttend, string includedOnLeave, string selectedOperand, string selectedNumberOfDay )
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

            List<AttendanceModel> tempList;
            List<AttendanceModel> attendanceList = new List<AttendanceModel>();

            AttendanceAnalysisModel attendanceAnalysisModel;
            List<AttendanceAnalysisModel> attendanceAnalysisList = new List<AttendanceAnalysisModel>();

            string approverName;
            string NRIC;

            userList = TempData["UserList"] as List<UserModel>;
            TempData.Keep("UserList");

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

           
            int totalLateIn = 0;
            int totalEarlyOut = 0;
            int totalLateInEarlyOut = 0;
            int totalIncomplete = 0;
            int totalAbsent = 0;
            int totalAttend = 0;
            int totalOnLeave = 0;

            int totalAll = 0;

            bool isIncludedLateIn = Convert.ToBoolean(includedLateIn);
            bool isIncludedEarlyOut = Convert.ToBoolean(includedEarlyOut);
            bool isIncludedLateInEarlyOut = Convert.ToBoolean(includedLateInEarlyOut);
            bool isIncludedIncomplete = Convert.ToBoolean(includedIncomplete);
            bool isIncludedAbsent = Convert.ToBoolean(includedAbsent);
            bool isIncludedAttend = Convert.ToBoolean(includedAttend);
            bool isIncludedOnLeave = Convert.ToBoolean(includedOnLeave);
            int numberOfDay = Convert.ToInt32(selectedNumberOfDay);

            bool isMatched;


            foreach (var row in selectedUser)
            {
                NRIC = row.NRIC;

                totalLateIn = attendanceList.Where(a => a.AttendanceStatusID == "LIN" && a.IsApproved == false && a.NRIC == NRIC).Count();
                totalEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "EOT" && a.IsApproved == false && a.NRIC == NRIC).Count();
                totalLateInEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "L/E" && a.IsApproved == false && a.NRIC == NRIC).Count();
                totalIncomplete = attendanceList.Where(a => a.AttendanceStatusID == "ICP" && a.IsApproved == false && a.NRIC == NRIC).Count();
                totalAbsent = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == false && a.NRIC == NRIC).Count();
                totalAttend = attendanceList.Where(a => (a.AttendanceStatusID == "NOR" || (a.IsApproved == true && a.IsForOnLeave == false)) && a.NRIC == NRIC).Count();
                totalOnLeave = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == true && a.IsForOnLeave == true && a.NRIC == NRIC).Count();

                totalAll = 0;
                isMatched = false;

                if (isIncludedLateIn.Equals(true))
                {
                    totalAll += totalLateIn;
                }

                if (isIncludedEarlyOut.Equals(true))
                {
                    totalAll += totalEarlyOut;
                }

                if (isIncludedLateInEarlyOut.Equals(true))
                {
                    totalAll += totalLateInEarlyOut;
                }

                if (isIncludedIncomplete.Equals(true))
                {
                    totalAll += totalIncomplete;
                }

                if (isIncludedAbsent.Equals(true))
                {
                    totalAll += totalAbsent;
                }

                if (isIncludedAttend.Equals(true))
                {
                    totalAll += totalAttend;
                }

                if (isIncludedOnLeave.Equals(true))
                {
                    totalAll += totalOnLeave;
                }


                switch (selectedOperand)
                {
                    case "<":

                        if (totalAll < numberOfDay)
                        {
                            isMatched = true;
                        }
                        break;

                    case "<=":

                        if (totalAll <= numberOfDay)
                        {
                            isMatched = true;
                        }
                        break;

                    case "=":

                        if (totalAll == numberOfDay)
                        {
                            isMatched = true;
                        }
                        break;


                    case ">=":

                        if (totalAll >= numberOfDay)
                        {
                            isMatched = true;
                        }
                        break;


                    case ">":

                        if (totalAll > numberOfDay)
                        {
                            isMatched = true;
                        }
                        break;
                }


                if (isMatched.Equals(true))
                {

                    attendanceAnalysisModel = new AttendanceAnalysisModel();

                    attendanceAnalysisModel.ReportDate = reportDate;
                    attendanceAnalysisModel.DepartmentName = row.DepartmentName;
                    attendanceAnalysisModel.UserName = row.UserName;
                    attendanceAnalysisModel.NRIC = row.NRIC;
                    attendanceAnalysisModel.TotalLateIn = totalLateIn;
                    attendanceAnalysisModel.TotalEarlyOut = totalEarlyOut;
                    attendanceAnalysisModel.TotalLateInEarlyOut = totalLateInEarlyOut;
                    attendanceAnalysisModel.TotalIncomplete = totalIncomplete;
                    attendanceAnalysisModel.TotalAbsent = totalAbsent;
                    attendanceAnalysisModel.TotalAttend = totalAttend;
                    attendanceAnalysisModel.TotalOnLeave = totalOnLeave;

                    attendanceAnalysisList.Add(attendanceAnalysisModel);

                }
               
            }

            TempData["AttendanceAnalysisList"] = attendanceAnalysisList;

            return Json(attendanceAnalysisList.OrderBy(a => a.UserName).ToList(), JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetAttendanceSummary()
        {
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            List<AttendanceAnalysisModel> attendanceAnalysisList = new List<AttendanceAnalysisModel> ();
            attendanceAnalysisList = TempData["AttendanceAnalysisList"] as List<AttendanceAnalysisModel>;

            TempData.Keep("AttendanceAnalysisList");

            int totalLateIn = 0;
            int totalEarlyOut = 0;
            int totalLateInEarlyOut = 0;
            int totalIncomplete = 0;
            int totalAbsent = 0;
            int totalAttend = 0;
            int totalOnLeave = 0;

            totalLateIn = attendanceAnalysisList.Select(a=>a.TotalLateIn).Sum();
            totalEarlyOut = attendanceAnalysisList.Select(a => a.TotalEarlyOut).Sum();
            totalLateInEarlyOut = attendanceAnalysisList.Select(a => a.TotalLateInEarlyOut).Sum();
            totalIncomplete = attendanceAnalysisList.Select(a => a.TotalIncomplete).Sum();
            totalAbsent = attendanceAnalysisList.Select(a => a.TotalAbsent).Sum();
            totalAttend = attendanceAnalysisList.Select(a => a.TotalAttend).Sum();
            totalOnLeave = attendanceAnalysisList.Select(a => a.TotalOnLeave).Sum();

            attendanceSummaryModel.TotalLateIn = totalLateIn;
            attendanceSummaryModel.TotalEarlyOut = totalEarlyOut;
            attendanceSummaryModel.TotalLateInEarlyOut = totalLateInEarlyOut;
            attendanceSummaryModel.TotalIncomplete = totalIncomplete;
            attendanceSummaryModel.TotalAbsent = totalAbsent;
            attendanceSummaryModel.TotalAttend = totalAttend;
            attendanceSummaryModel.TotalOnLeave = totalOnLeave;
                     
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