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
using MyTime.Interfaces;
//using CrystalDecisions.Shared;

namespace MyTime.Controllers
{
    public class DepartmentAttendanceHistoryReportController : EnvironmentController
    {
        SystemDBService systemDBService = new SystemDBService();
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();

        ApproverDBService approverDBService = new ApproverDBService();
        AttendanceDBService attendanceDBService = new AttendanceDBService();

        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();

        // GET: DepartmentlAttendanceHistoryReport
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            ApproverViewModel approverViewModel = new ApproverViewModel();

            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            DepartmentAttendanceHistoryReportViewModel departmentAttendanceHistoryReportViewModel = new DepartmentAttendanceHistoryReportViewModel();

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

                departmentAttendanceHistoryReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                systemModel = systemDBService.GetData();
                dataStartDate = systemModel.DataStartDate;

                departmentAttendanceHistoryReportViewModel.SelectListMonthYear = PrepareSelectMonthYearList(dataStartDate);
                departmentAttendanceHistoryReportViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

            }

            return View(departmentAttendanceHistoryReportViewModel);
        }

        [HttpPost]
        public ActionResult GetDepartmentUser(string selectedMonthYear, string selectedDepartment)
        {
            List<UserModel> userList = new List<UserModel>();
            DepartmentAttendanceHistoryReportViewModel departmentAttendanceHistoryReportViewModel = new DepartmentAttendanceHistoryReportViewModel();

            //// Get Month Start and End Date
            DateTime startOn;
            //DateTime endOn;

            DateTime.TryParse(selectedMonthYear, out startOn);
            //int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            //endOn = startOn.AddDays((days - 1));

            // include those resigned after attendance date
            userList = userDBService.ListUser().Where(u => u.DepartmentID == selectedDepartment && ((u.IsResigned == true && u.ResignedOn > startOn.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();

            return Json(userList, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GenerateAttendanceList(string selectedAttendanceDate, string selectedNRIC)
        {
            string[] NRIC = selectedNRIC.Split('#');

            string approverName;
            UserModel userModel;

            string usrID;
            string userName;
            string departmentID;
            string departmentName;
            int accessRoleID;

            List<AttendanceModel> attendanceList;
            AttendanceSummaryModel attendanceSummaryModel;

            List<CRAttendanceMonthlyModel> crTempList;
            List<CRAttendanceMonthlyModel> crAttendanceMonthlyList = new List<CRAttendanceMonthlyModel>();

            // Get Month Start and End Date
            DateTime startOn;
            DateTime endOn;

            DateTime.TryParse(selectedAttendanceDate, out startOn);
            int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            endOn = startOn.AddDays((days - 1));

            for (int i = 0; i < NRIC.Length; i++)
            {
                approverName = GetFirstUserApproverName(NRIC[i]);

                userModel = new UserModel();
                userModel = userDBService.GetDataByID(NRIC[i]);

                usrID = userModel.USRID;
                userName = userModel.UserName;
                departmentID = userModel.DepartmentID;
                departmentName = userModel.DepartmentName;
                accessRoleID = userModel.AccessRoleID;

                // Attendance
                attendanceList = new List<AttendanceModel>();
                attendanceList = attendanceDBService.GetMonthlyAttendance(NRIC[i], usrID, userName, departmentID, departmentName, startOn, endOn, accessRoleID, approverName);

                // Summary
                attendanceSummaryModel = new AttendanceSummaryModel();

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

                // Crystal Report
                crTempList = new List<CRAttendanceMonthlyModel>();
                crTempList = crystalReportDBService.PrepareAttendanceMonthlyReport(attendanceList, attendanceSummaryModel);

                crAttendanceMonthlyList.AddRange(crTempList);

            }

            TempData["crAttendanceMonthlyList"] = crAttendanceMonthlyList;

            return Json(true, JsonRequestBehavior.AllowGet);

        }


        public ActionResult PrintAttendanceMonthlyReport()
        {
            if (Session["OrganisationName"] != null)
            {



                //List<AttendanceModel> attendanceList = new List<AttendanceModel>();
                //UserModel userModel = new UserModel();
                //AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

                List<CRAttendanceMonthlyModel> crAttendanceMonthlyList = new List<CRAttendanceMonthlyModel>();

                //attendanceList = TempData["AttendanceList"] as List<AttendanceModel>;
                //attendanceSummaryModel = TempData["AttendanceSummary"] as AttendanceSummaryModel;

                //TempData.Keep("UserApproverList");
                //TempData.Keep("AttendanceList");
                //TempData.Keep("AttendanceSummary");

                //crAttendanceMonthlyList = crystalReportDBService.PrepareAttendanceMonthlyReport(attendanceList, attendanceSummaryModel);

                crAttendanceMonthlyList = TempData["crAttendanceMonthlyList"] as List<CRAttendanceMonthlyModel>;
                TempData.Keep("crAttendanceMonthlyList");

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
            else
            {
                return RedirectToAction("Index", "Auth");
            }

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

        public string GetFirstUserApproverName(string NRIC)
        {

            List<ApproverUserModel> userApproverList = new List<ApproverUserModel>();
            ApproverUserModel userApproverModel = new ApproverUserModel();
            userApproverList = approverDBService.GetUserApprover(NRIC).OrderBy(a => a.UserName).ToList();

            string approverName = "";

            userApproverModel = userApproverList.Select(ua => ua).FirstOrDefault();

            if (userApproverModel != null)
            {
                approverName = userApproverModel.UserName;
            }


            return approverName;
        }


    }
}