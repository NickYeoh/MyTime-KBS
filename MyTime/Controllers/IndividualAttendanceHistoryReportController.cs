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
//using CrystalDecisions.Shared;

namespace MyTime.Controllers
{
    public class IndividualAttendanceHistoryReportController : EnvironmentController
    {

        SystemDBService systemDBService = new SystemDBService();
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        ApproverDBService approverDBService = new ApproverDBService();
        AttendanceDBService attendanceDBService = new AttendanceDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();


        // GET: IndividualAttendanceHistoryReport
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            ApproverViewModel approverViewModel = new ApproverViewModel();

            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            IndividualAttendanceHistoryReportViewModel individualAttendanceHistoryReportViewModel = new IndividualAttendanceHistoryReportViewModel();

            SystemModel systemModel = new SystemModel();
            DateTime dataStartDate;


            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                TempData["User"] = userModel;

                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                individualAttendanceHistoryReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                systemModel = systemDBService.GetData();
                dataStartDate = systemModel.DataStartDate;

                individualAttendanceHistoryReportViewModel.SelectListMonthYear = PrepareSelectMonthYearList(dataStartDate);
                individualAttendanceHistoryReportViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

            }

            return View(individualAttendanceHistoryReportViewModel);

        }
              
        [HttpPost]
        public ActionResult GetDepartmentUser(string selectedMonthYear, string selectedDepartment)
        {
            List<UserModel> userList = new List<UserModel>();
            IndividualAttendanceHistoryReportViewModel individualAttendanceHistoryReportViewModel = new IndividualAttendanceHistoryReportViewModel();

            //// Get Month Start and End Date
            DateTime startOn;
            //DateTime endOn;

            DateTime.TryParse(selectedMonthYear, out startOn);
            //int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            //endOn = startOn.AddDays((days - 1));

            // include those resigned after attendance date
            userList = userDBService.ListUser().Where(u => u.DepartmentID == selectedDepartment && ((u.IsResigned == true && u.ResignedOn > startOn.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();
            individualAttendanceHistoryReportViewModel.SelectListUser = PrepareSelectUserList(userList);
            return Json(individualAttendanceHistoryReportViewModel.SelectListUser, JsonRequestBehavior.AllowGet);

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


        public ActionResult GenerateAttendanceList(string startDate, string NRIC)
        {        
            string approverName;

            UserModel userModel = new UserModel();
            List<AttendanceModel> attendanceList = new List<AttendanceModel>();

            approverName = GetFirstUserApproverName(NRIC);

            userModel = userDBService.GetDataByID(NRIC);           
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

        public string GetFirstUserApproverName(string NRIC)
        {

            List<ApproverUserModel> userApproverList = new List<ApproverUserModel>();
            ApproverUserModel userApproverModel = new ApproverUserModel();
            userApproverList = approverDBService.GetUserApprover(NRIC).OrderBy(a => a.UserName).ToList();
            
            string approverName= "";

            userApproverModel = userApproverList.Select(ua => ua).FirstOrDefault();

            if (userApproverModel != null)
            {
                approverName = userApproverModel.UserName;

            }

            return approverName;
        }

    }
}