using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;
using System.IO;
using MyTime.Interfaces;
using CrystalDecisions.CrystalReports.Engine;
using System.Text.RegularExpressions;
//using CrystalDecisions.Shared;


namespace MyTime.Controllers
{
    public class AttendanceController : EnvironmentController
    {

        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        SystemDBService systemDBService = new SystemDBService();
        AttendanceDBService attendanceDBService = new AttendanceDBService();
        ReasonDBService reasonDBService = new ReasonDBService();
        ApproverDBService approverDBService = new ApproverDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();

        // GET: Attendance
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            ApproverViewModel approverViewModel = new ApproverViewModel();

            AttendanceViewModel attendanceViewModel = new AttendanceViewModel();
            SystemModel systemModel = new SystemModel();
            DateTime dataStartDate;

            List<ApproverUserModel> userApproverList = new List<ApproverUserModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                TempData["User"] = userModel;

                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                attendanceViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                systemModel = systemDBService.GetData();
                dataStartDate = systemModel.DataStartDate;

                userApproverList = approverDBService.GetUserApprover(userModel.NRIC).OrderBy(a => a.UserName).ToList();

                attendanceViewModel.SelectListMonthYear = PrepareSelectMonthYearList(dataStartDate);
                attendanceViewModel.UserApproverList = userApproverList;

                TempData["UserApproverList"] = userApproverList;

            }

            return View(attendanceViewModel);
        }


        public ActionResult GenerateAttendanceList(string startDate)
        {
            List<ApproverUserModel> userApproverList = new List<ApproverUserModel>();
            string approverName;

            UserModel userModel = new UserModel();
            List<AttendanceModel> attendanceList = new List<AttendanceModel>();
            ApproverUserModel userApproverModel = new ApproverUserModel();

            userApproverList = TempData["UserApproverList"] as List<ApproverUserModel>;
            TempData.Keep("UserApproverList");

            // Get First Approver Name
            if (userApproverList.Count > 0)
            {
                userApproverModel = userApproverList.Select(ua => ua).FirstOrDefault();
                approverName = userApproverModel.UserName;
            }
            else
            {
                approverName = "";
            }

            userModel = userDBService.GetDataByID(User.Identity.Name);
            string NRIC = userModel.NRIC;
            string usrID = userModel.USRID;
            string userName = userModel.UserName;
            string departmentID = userModel.DepartmentID;
            string departmentName = userModel.DepartmentName;
            int accessRoleID = userModel.AccessRoleID;

            // Get Month Start and End Date
            DateTime startOn;
            DateTime endOn;

            DateTime.TryParse(startDate, out startOn);
            int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            endOn = startOn.AddDays((days - 1));

            attendanceList = attendanceDBService.GetMonthlyAttendance(NRIC, usrID, userName, departmentID, departmentName, startOn, endOn, accessRoleID, approverName);

            TempData["AttendanceList"] = attendanceList;

            return Json(attendanceList, JsonRequestBehavior.AllowGet);
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
            string totalOvertime = string.Format("0{0} 0{1}", MyTime.Resource.Hour, MyTime.Resource.Minute);

            totalLateIn = attendanceList.Where(a => a.AttendanceStatusID == "LIN" && a.IsApproved == false).Count();
            totalEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "EOT" && a.IsApproved == false).Count();
            totalLateInEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "L/E" && a.IsApproved == false).Count();
            totalIncomplete = attendanceList.Where(a => a.AttendanceStatusID == "ICP" && a.IsApproved == false).Count();
            totalAbsent = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == false).Count();
            totalAttend = attendanceList.Where(a => a.AttendanceStatusID == "NOR" || (a.IsApproved == true && a.IsForOnLeave == false)).Count();
            totalOnLeave = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == true && a.IsForOnLeave == true).Count();
            totalOvertime = attendanceList.Select(a => a.TotalOvertime).Last();

            attendanceSummaryModel.TotalLateIn = totalLateIn;
            attendanceSummaryModel.TotalEarlyOut = totalEarlyOut;
            attendanceSummaryModel.TotalLateInEarlyOut = totalLateInEarlyOut;
            attendanceSummaryModel.TotalIncomplete = totalIncomplete;
            attendanceSummaryModel.TotalAbsent = totalAbsent;
            attendanceSummaryModel.TotalAttend = totalAttend;
            attendanceSummaryModel.TotalOnLeave = totalOnLeave;
            attendanceSummaryModel.TotalOvertime = totalOvertime;

            TempData["AttendanceList"] = attendanceList;
            TempData["AttendanceSummary"] = attendanceSummaryModel;

            return Json(attendanceSummaryModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintAttendanceMonthlyReport()
        {

            List<AttendanceModel> attendanceList = new List<AttendanceModel>();
            UserModel userModel = new UserModel();
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            List<CRAttendanceMonthlyModel> crAttendanceMonthlyList = new List<CRAttendanceMonthlyModel>();

            attendanceList = TempData["AttendanceList"] as List<AttendanceModel>;
            attendanceSummaryModel = TempData["AttendanceSummary"] as AttendanceSummaryModel;

            TempData.Keep("UserApproverList");
            TempData.Keep("AttendanceList");
            TempData.Keep("AttendanceSummary");

            crAttendanceMonthlyList = crystalReportDBService.PrepareAttendanceMonthlyReport(attendanceList, attendanceSummaryModel);

            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports"), "AttendanceHistoryMonthlyCR.rpt"));
            report.SetDataSource(crAttendanceMonthlyList);

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


            return File(stream, "application/pdf", "Laporan Perakam Waktu.pdf");

        }


        public ActionResult _SubmitReason(string rowData)
        {

            AttendanceReasonViewModel attendanceReasonViewModel = new AttendanceReasonViewModel();
            AttendanceReasonModel attendanceReasonModel = new AttendanceReasonModel();
            List<ReasonModel> reasonList = new List<ReasonModel>();

            string[] data = rowData.Split('#');

            for (int i = 0; i < 21; i++)
            {
                switch (i)
                {
                    case 0:
                        attendanceReasonModel.NRIC = data[i];
                        break;
                    case 1:
                        attendanceReasonModel.AttendanceDate = Convert.ToDateTime(data[i]);
                        break;
                    case 2:
                        attendanceReasonModel.AttendanceDay = data[i];
                        break;
                    case 3:
                        attendanceReasonModel.ShiftID = data[i];
                        break;
                    case 4:
                        attendanceReasonModel.AttendanceStatusID = data[i];
                        break;
                    case 5:
                        attendanceReasonModel.AttendanceStatus = data[i];
                        break;
                    case 6:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.FirstIn = data[i].ToString();
                        }

                        break;
                    case 7:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.Lateness = data[i].ToString();
                        }

                        break;
                    case 8:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.LastOut = data[i].ToString();
                        }

                        break;
                    case 9:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.WorkTime = data[i].ToString();
                        }

                        break;
                    case 10:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.OvertimeStart = data[i].ToString();
                        }

                        break;
                    case 11:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.OvertimeEnd = data[i].ToString();
                        }

                        break;
                    case 12:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.Overtime = data[i].ToString();
                        }

                        break;

                    case 13:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.OvertimeExtraStart = data[i].ToString();
                        }

                        break;
                    case 14:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.OvertimeExtraEnd = data[i].ToString();
                        }

                        break;
                    case 15:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.OvertimeExtra = data[i].ToString();
                        }

                        break;
                    case 16:
                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.TotalOvertime = data[i].ToString();
                        }

                        break;
                    case 17:

                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.ReasonID = data[i].ToString();
                        }

                        break;

                    case 18:

                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.ReasonName = data[i].ToString();
                        }

                        break;

                    case 19:

                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.Remark = data[i].ToString();
                        }

                        break;

                    case 20:

                        if (data[i].ToString() != "null")
                        {
                            attendanceReasonModel.Proof = data[i].ToString();
                        }

                        break;
                }

            }

            attendanceReasonViewModel.AttendanceReasonModel = attendanceReasonModel;

            switch (attendanceReasonModel.AttendanceStatusID)
            {

                case "ABS":
                    reasonList = reasonDBService.ListReason().Where(r => (r.IsForAbsent == true || r.IsForOnLeave == true) && r.IsActivated == true).ToList();
                    break;
                case "LIN":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForLateIn == true && r.IsActivated == true).ToList();
                    break;
                case "EOT":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForEarlyOut == true && r.IsActivated == true).ToList();
                    break;
                case "L/E":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForLateIn == true && r.IsForEarlyOut == true && r.IsActivated == true).ToList();
                    break;
                case "ICP":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForIncomplete == true && r.IsActivated == true).ToList();
                    break;
            }


            attendanceReasonViewModel.SelectReason = PrepareSelectReasonList(reasonList);

            return PartialView(attendanceReasonViewModel);
        }


        [HttpPost]
        public ActionResult _SubmitReason(AttendanceReasonViewModel attendanceReasonViewModel)
        {
            AttendanceReasonModel attendanceReasonModel = new AttendanceReasonModel();

            List<ApproverUserModel> approverUserList = new List<ApproverUserModel>();
            UserModel userModel = new UserModel();

            string userName = "";
            String userEmail = "";

            //attendanceReasonViewModel.AttendanceReasonModel.PostedProof.si

            string isEmailNotificationEnabled = Session["IsEmailNotificationEnabled"].ToString();


            var reg = new Regex(@"([a-zA-Z0-9\s_\\.\-:\(\)])+(.pdf|.png|.jpg|.gif)$");

            //string fn = attendanceReasonViewModel.AttendanceReasonModel.PostedProof.FileName;

            //fn += " ";

            if (attendanceReasonViewModel.AttendanceReasonModel.PostedProof != null && !reg.IsMatch(attendanceReasonViewModel.AttendanceReasonModel.PostedProof.FileName))//model.File is public HttpPostedFileBase File { get; set; }
            {
                attendanceReasonViewModel.AttendanceReasonModel.PostedProof = null;
                ModelState.AddModelError("AttendanceReasonModel.PostedProof", MyTime.Resource.InvalidFile);
            }
            else
            {
                if (ModelState.IsValid)
                {

                    approverUserList = TempData["UserApproverList"] as List<ApproverUserModel>;
                    TempData.Keep("UserApproverList");

                    userModel = TempData["User"] as UserModel;
                    TempData.Keep("User");

                    attendanceReasonModel = attendanceReasonViewModel.AttendanceReasonModel;

                    if (attendanceReasonModel.ReasonID != null)
                    {
                        if (userModel.UserName != null)
                        {
                            userName = userModel.UserName;
                        }

                        if (userModel.Email != null)
                        {
                            userEmail = userModel.Email;
                        }

                        if (attendanceDBService.SubmitReason(attendanceReasonModel, userName, userEmail, approverUserList, isEmailNotificationEnabled).Equals(false))
                        {
                            return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                        }

                        return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        ModelState.AddModelError("AttendanceReasonModel.ReasonID", string.Format("{0} {1}", MyTime.Resource.PleaseSelect, MyTime.Resource.Reason));
                    }

                }
            }

            List<ReasonModel> reasonList = new List<ReasonModel>();
            reasonList = reasonDBService.ListReason();

            switch (attendanceReasonModel.AttendanceStatusID)
            {

                case "ABS":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForAbsent == true || r.IsForOnLeave == true).ToList();
                    break;
                case "LIN":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForLateIn == true).ToList();
                    break;
                case "EOT":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForEarlyOut == true).ToList();
                    break;
                case "L/E":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForLateIn == true || r.IsForEarlyOut == true).ToList();
                    break;
                case "ICP":
                    reasonList = reasonDBService.ListReason().Where(r => r.IsForIncomplete == true && r.IsActivated == true).ToList();
                    break;
            }

            attendanceReasonViewModel.SelectReason = PrepareSelectReasonList(reasonList);

            return PartialView(attendanceReasonViewModel);

        }

        private IEnumerable<SelectListItem> PrepareSelectMonthYearList(DateTime dataStartDate)
        {
            ISharedFunction sharedFunction;
            sharedFunction = new SharedFunction();

            var selectMonthYearList = new List<SelectListItem>();

            int totalMonthDiff = sharedFunction.CalculateMonthDiff(dataStartDate, DateTime.Now);


            DateTime monthStart;
            string monthYear;

            for (int month = totalMonthDiff; month >= 0; month--)
            {

                monthStart = new DateTime(dataStartDate.AddMonths(month).Year, dataStartDate.AddMonths(month).Month, 1);
                monthYear = dataStartDate.AddMonths(month).ToString("MMM, yyyy");

                selectMonthYearList.Add(new SelectListItem
                {
                    Value = monthStart.ToString("yyyy-MM-dd"),
                    Text = monthYear
                });
            }


            return selectMonthYearList;
        }

        private IEnumerable<SelectListItem> PrepareSelectReasonList(List<ReasonModel> ReasonList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in ReasonList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.ReasonID.ToString(),
                    Text = row.ReasonName.ToString()
                });
            }
            return selectList;
        }

    }
}