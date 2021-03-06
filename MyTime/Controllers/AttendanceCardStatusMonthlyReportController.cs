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
    public class AttendanceCardStatusMonthlyReportController : EnvironmentController
    {
        SystemDBService systemDBService = new SystemDBService();
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();
        AttendanceCardDBService attendanceCardDBService = new AttendanceCardDBService();


        // GET: AttendanceCardReport
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            SystemModel systemModel = new SystemModel();
            DateTime attendanceCardStartDate;

            systemModel = systemDBService.GetData();
            attendanceCardStartDate = systemModel.AttendanceCardStartDate;


            AttendanceCardStatusMonthlyReportViewModel attendanceCardStatusMonthlyReportViewModel = new AttendanceCardStatusMonthlyReportViewModel();
            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                attendanceCardStatusMonthlyReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                attendanceCardStatusMonthlyReportViewModel.SelectListMonthYear = PrepareSelectMonthYearList(attendanceCardStartDate);
                attendanceCardStatusMonthlyReportViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

            }

            return View(attendanceCardStatusMonthlyReportViewModel);
        }

        [HttpPost]
        public ActionResult GenerateAttendanceCardList(string selectedMonthYear, string selectedDepartmentID, string selectedAttendanceCardStatus)
        {

            List<AttendanceCardStatusMontlyReportModel> attendanceCardReportList = new List<AttendanceCardStatusMontlyReportModel>();

            DateTime attendanceMonth = Convert.ToDateTime(selectedMonthYear);

            attendanceCardReportList = attendanceCardDBService.GetMonthlyAttendanceCardByAttendanceCardStatusAndDepartment(attendanceMonth, selectedDepartmentID, selectedAttendanceCardStatus);
            
            TempData["AttendanceCardStatusMonthlyReportList"] = attendanceCardReportList;

            return Json(attendanceCardReportList, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SelectListItem> PrepareSelectMonthYearList(DateTime attendanceCardStartDate)
        {
            ISharedFunction sharedFunction;
            sharedFunction = new SharedFunction();

            var selectMonthYearList = new List<SelectListItem>();

            int totalMonthDiff = sharedFunction.CalculateMonthDiff(attendanceCardStartDate, DateTime.Now);


            DateTime monthStart;
            string monthYear;

            for (int month = totalMonthDiff; month >= 0; month--)
            {

                monthStart = new DateTime(attendanceCardStartDate.AddMonths(month).Year, attendanceCardStartDate.AddMonths(month).Month, 1);
                monthYear = attendanceCardStartDate.AddMonths(month).ToString("MMM, yyyy");

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


        public ActionResult PrintAttendanceCardStatusMonthlyReport()
        {
            if (Session["OrganisationName"] != null)
            {


                List<AttendanceCardStatusMontlyReportModel> AttendanceCardStatusMonthlyReportList = new List<AttendanceCardStatusMontlyReportModel>();
                List<CRAttendanceCardStatusMonthlyReportModel> crAttendanceCardStatusMonthlyReportList = new List<CRAttendanceCardStatusMonthlyReportModel>();

                //string reportType;

                //reportType = "Monthly";
                AttendanceCardStatusMonthlyReportList = TempData["AttendanceCardStatusMonthlyReportList"] as List<AttendanceCardStatusMontlyReportModel>;
               
                //reportType = TempData["ReportType"] as string;

                TempData.Keep("AttendanceCardStatusMonthlyReportList");               
                //TempData.Keep("ReportType");

                crAttendanceCardStatusMonthlyReportList = crystalReportDBService.PrepareAttendanceCardReport(AttendanceCardStatusMonthlyReportList.OrderBy(a => a.UserName).ToList());

                ReportDocument report = new ReportDocument();
                report.Load(Path.Combine(Server.MapPath("~/Reports"), "AttendanceCardStatusMonthlyCR.rpt"));
                report.SetDataSource(crAttendanceCardStatusMonthlyReportList);

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

                return File(stream, "application/pdf", "Laporan Status Kad Kedatangan Bulanan.pdf");             

            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }

        }

    }
}