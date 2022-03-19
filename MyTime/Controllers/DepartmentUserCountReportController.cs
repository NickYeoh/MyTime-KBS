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
    public class DepartmentUserCountReportController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        DepartmentDBService departmentDBService = new DepartmentDBService();

        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();

        // GET: DepartmentUserCount
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();

            List<DepartmentUserCountModel> departmentUserCountList = new List<DepartmentUserCountModel>();

            DepartmentUserCountReportViewModel departmentUserCountReportViewModel = new DepartmentUserCountReportViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                departmentUserCountReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);
                departmentUserCountReportViewModel.OrganisationName = Session["OrganisationName"].ToString();

            }

            return View(departmentUserCountReportViewModel);
        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<DepartmentModel> departmentList = new List<DepartmentModel>();
            DepartmentModel departmentModel = new DepartmentModel();

            List<UserModel> userList = new List<UserModel>();
            string departmentID;
            string departmentName;
            int userCount;

            List<DepartmentUserCountModel> departmentUserCountList = new List<DepartmentUserCountModel>();
            DepartmentUserCountModel departmentUserCountModel;

            departmentList = departmentDBService.ListDepartment();
            userList = userDBService.ListUser();

            if (departmentList.Count > 0)
            {
                for (int i = 0; i < departmentList.Count; i++)
                {
                    departmentID = "";
                    userCount = 0;

                    departmentUserCountModel = new DepartmentUserCountModel();

                    departmentModel = departmentList[i];

                    departmentID = departmentModel.DepartmentID;
                    departmentName = departmentModel.DepartmentName;
                    userCount = userList.Where(u => u.DepartmentID == departmentID && ((u.IsResigned == true && u.ResignedOn > DateTime.Now) || u.IsResigned == false) && u.IsAttendanceExcluded == false).Count();

                    departmentUserCountModel.DepartmentID = departmentID;
                    departmentUserCountModel.DepartmentName = departmentName;
                    departmentUserCountModel.UserCount = userCount;

                    departmentUserCountList.Add(departmentUserCountModel);
                }

            }

            TempData["DepartmentUserCountList"] = departmentUserCountList;

            return Json(departmentUserCountList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserCountSummary()
        {
            List<DepartmentUserCountModel> departmentUserCountList = new List<DepartmentUserCountModel>();
            int totalUserCount = 0;

            departmentUserCountList = TempData["DepartmentUserCountList"] as List<DepartmentUserCountModel>;

            if (!departmentUserCountList.Equals(null))
            {
                totalUserCount = departmentUserCountList.Select(s => s.UserCount).Sum();

            }

            TempData.Keep("DepartmentUserCountList");

            return Json(totalUserCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintDepartmentUserCountReport()
        {
            if (Session["OrganisationName"] != null)
            {


                List<DepartmentUserCountModel> departmentUserCountList = new List<DepartmentUserCountModel>();
                departmentUserCountList = TempData["DepartmentUserCountList"] as List<DepartmentUserCountModel>;
                TempData.Keep("DepartmentUserCountList");

                List<CRDepartmentUserCountModel> crDepartmentUserCountList = new List<CRDepartmentUserCountModel>();

                crDepartmentUserCountList = crystalReportDBService.PrepareDepartmentUserCountReport(departmentUserCountList);

                ReportDocument report = new ReportDocument();
                report.Load(Path.Combine(Server.MapPath("~/Reports"), "DepartmentUserCountCR.rpt"));
                report.SetDataSource(crDepartmentUserCountList);

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


                return File(stream, "application/pdf", "Laporan Bilangan Pegawai Bahagian.pdf");
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }

        }
    }
}