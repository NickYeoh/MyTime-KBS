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
//using CrystalDecisions.Shared;

namespace MyTime.Controllers
{
    public class DepartmentAttendanceDailySummaryController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        AttendanceDBService attendanceDBService = new AttendanceDBService();
        DepartmentAttendanceDailySummaryViewModel departmentAttendanceDailySummaryViewModel = new DepartmentAttendanceDailySummaryViewModel();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();

        // GET: DepartmentAttendanceDailySummary
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                departmentAttendanceDailySummaryViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);
                departmentAttendanceDailySummaryViewModel.AttendanceDate = DateTime.Now;

            }

            return View(departmentAttendanceDailySummaryViewModel);
        }

        [HttpGet]
        public ActionResult GenerateAttendanceList(string selectedAttendanceDate)
        {
            //UserModel userModel = new UserModel();
            List<UserModel> userList = new List<UserModel>();
            List<AttendanceModel> attendanceList = new List<AttendanceModel>();

            List<DepartmentAttendanceDailyModel> departmentAttendanceDailyList = new List<DepartmentAttendanceDailyModel>();

            string tempDate;
            DateTime attendanceDate;

            // To get 1 month attendance
            DateTime startOn;
            DateTime endOn;

            tempDate = selectedAttendanceDate.Substring(6, 4) + "-" + selectedAttendanceDate.Substring(3, 2) + "-" + selectedAttendanceDate.Substring(0, 2);
            DateTime.TryParse(tempDate, out attendanceDate);

            tempDate = selectedAttendanceDate.Substring(6, 4) + "-" + selectedAttendanceDate.Substring(3, 2) + "-" + "01";
            DateTime.TryParse(tempDate, out startOn);

            int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            endOn = startOn.AddDays((days - 1));

            departmentAttendanceDailyList = attendanceDBService.GetDepartmentDailyAttendanceSummary(attendanceDate);

            TempData["DepartmentAttendanceDailyList"] = departmentAttendanceDailyList;

            return Json(departmentAttendanceDailyList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAttendanceSummary()
        {
            List<DepartmentAttendanceDailyModel> departmentAttendanceDailyList = new List<DepartmentAttendanceDailyModel>();
            int totalUserCount;
            int totalInCount;
            int totalOutCount;
            int totalAttendCount;
            string totalAttendPercentage;


            departmentAttendanceDailyList = TempData["DepartmentAttendanceDailyList"] as List<DepartmentAttendanceDailyModel>;

            DepartmentAttendanceDailySummaryModel departmentAttendanceDailySummaryModel = new DepartmentAttendanceDailySummaryModel();

            TempData.Keep("DepartmentAttendanceDailyList");

            totalUserCount = departmentAttendanceDailyList.Select(s => s.UserCount).Sum();
            totalInCount = departmentAttendanceDailyList.Select(s => s.InCount).Sum();
            totalOutCount = departmentAttendanceDailyList.Select(s => s.OutCount).Sum();
            totalAttendCount = departmentAttendanceDailyList.Select(s => s.AttendCount).Sum();
            totalAttendPercentage = (Convert.ToDecimal(totalAttendCount) / Convert.ToDecimal(totalUserCount)).ToString("0.0%");

            departmentAttendanceDailySummaryModel.TotalUserCount = totalUserCount;
            departmentAttendanceDailySummaryModel.TotalInCount = totalInCount;
            departmentAttendanceDailySummaryModel.TotalOutCount = totalOutCount;
            departmentAttendanceDailySummaryModel.TotalAttendCount = totalAttendCount;
            departmentAttendanceDailySummaryModel.TotalAttendPercentage = totalAttendPercentage;


            return Json(departmentAttendanceDailySummaryModel, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintDepartmentAttendanceDailySummary()
        {
            if (Session["OrganisationName"] != null)
            {



                List<DepartmentAttendanceDailyModel> departmentAttendanceDailyList = new List<DepartmentAttendanceDailyModel>();
                departmentAttendanceDailyList = TempData["DepartmentAttendanceDailyList"] as List<DepartmentAttendanceDailyModel>;

                List<CRDepartmentAttendanceDailyModel> crDepartmentAttendanceDailyList = new List<CRDepartmentAttendanceDailyModel>();

                TempData.Keep("DepartmentAttendanceDailyList");

                crDepartmentAttendanceDailyList = crystalReportDBService.PrepareDepartmentAttendanceDailySummary(departmentAttendanceDailyList.OrderBy(s => s.DepartmentName).ToList());

                ReportDocument report = new ReportDocument();
                report.Load(Path.Combine(Server.MapPath("~/Reports"), "DepartmentAttendanceDailySummaryCR.rpt"));
                report.SetDataSource(crDepartmentAttendanceDailyList);

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


                return File(stream, "application/pdf", "Rumusan Kedatangan Harian Bahagian.pdf");
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }

        }

    }
}