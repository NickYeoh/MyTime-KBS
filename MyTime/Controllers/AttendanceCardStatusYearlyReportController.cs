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
    public class AttendanceCardStatusYearlyReportController : EnvironmentController
    {
        SystemDBService systemDBService = new SystemDBService();
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();
        AttendanceCardDBService attendanceCardDBService = new AttendanceCardDBService();

        // GET: AttendanceCardSummaryReport
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            SystemModel systemModel = new SystemModel();
            DateTime attendanceCardStartDate;

            systemModel = systemDBService.GetData();
            attendanceCardStartDate = systemModel.AttendanceCardStartDate;


            AttendanceCardStatusYearlyReportViewModel attendanceCardStatusYearlyReportViewModel = new AttendanceCardStatusYearlyReportViewModel();
            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                attendanceCardStatusYearlyReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                attendanceCardStatusYearlyReportViewModel.SelectListYear = PrepareSelectYearList(attendanceCardStartDate);
                attendanceCardStatusYearlyReportViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

            }

            return View(attendanceCardStatusYearlyReportViewModel);
           
        }


        private IEnumerable<SelectListItem> PrepareSelectYearList(DateTime attendanceCardStartDate)
        {
            var selectYearList = new List<SelectListItem>();
            int currentYear = DateTime.Now.Year;

            if (!currentYear.Equals(attendanceCardStartDate.Year))
            {
                for (int year = attendanceCardStartDate.Year; year <= currentYear; year++)
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
                    Value = attendanceCardStartDate.ToString("yyyy-MM-dd"),
                    Text = attendanceCardStartDate.Year.ToString()
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

        [HttpPost]
        public ActionResult GenerateAttendanceCardList(string selectedYear, string selectedDepartmentID, string selectedAttendanceCardStatus)
        {

            List<AttendanceCardStatusYearlyReportModel> attendanceCardSummaryReportList = new List<AttendanceCardStatusYearlyReportModel>();

            DateTime attendanceYear = Convert.ToDateTime(selectedYear);

            attendanceCardSummaryReportList = attendanceCardDBService.GetYearlyAttendanceCardByDepartment(attendanceYear, selectedDepartmentID);

            TempData["AttendanceCardStatusYearlyReportList"] = attendanceCardSummaryReportList;

            return Json(attendanceCardSummaryReportList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintAttendanceCardStatusYearlyReport()
        {
            if (Session["OrganisationName"] != null)
            {


                List<AttendanceCardStatusYearlyReportModel> AttendanceCardStatusYearlyReportList = new List<AttendanceCardStatusYearlyReportModel>();
                List<CRAttendanceCardStatusYearlyReportModel> crAttendanceCardStatusYearlyReportList = new List<CRAttendanceCardStatusYearlyReportModel>();

                //string reportType;

                //reportType = "Monthly";
                AttendanceCardStatusYearlyReportList = TempData["AttendanceCardStatusYearlyReportList"] as List<AttendanceCardStatusYearlyReportModel>;

                //reportType = TempData["ReportType"] as string;

                TempData.Keep("AttendanceCardStatusYearlyReportList");
                //TempData.Keep("ReportType");

                crAttendanceCardStatusYearlyReportList = crystalReportDBService.PrepareAttendanceCardSummaryReport(AttendanceCardStatusYearlyReportList.OrderBy(a => a.UserName).ToList());

                ReportDocument report = new ReportDocument();
                report.Load(Path.Combine(Server.MapPath("~/Reports"), "AttendanceCardStatusYearlyCR.rpt"));
                report.SetDataSource(crAttendanceCardStatusYearlyReportList);

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

                return File(stream, "application/pdf", "Laporan Status Kad Kedatangan Tahunan.pdf");

            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }

        }

    }
}