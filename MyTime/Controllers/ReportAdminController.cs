using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;
using System.IO;

namespace MyTime.Controllers
{
    public class ReportAdminController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        DepartmentDBService departmentDBService = new DepartmentDBService();

        // GET: System
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            ReportAdminViewModel ReportAdminViewModel = new ReportAdminViewModel();


            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                ReportAdminViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            }

            return View(ReportAdminViewModel);
        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<ReportAdminModel> dataList = new List<ReportAdminModel>();

            dataList = reportAdminDBService.ListReportAdmin();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _ListReportAdminDepartment(String id)
        {
            UserModel userModel = new UserModel();
            ReportAdminViewModel ReportAdminViewModel = new ReportAdminViewModel();

            ReportAdminViewModel.ReportAdminModel = reportAdminDBService.GetDataByID(id);

            userModel = userDBService.GetDataByID(User.Identity.Name);
            ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

            ReportAdminViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            return PartialView(ReportAdminViewModel);
        }

        public ActionResult _ListNewReportAdminDepartment(String id)
        {
            UserModel userModel = new UserModel();
            ReportAdminViewModel ReportAdminViewModel = new ReportAdminViewModel();

            ReportAdminViewModel.ReportAdminModel = reportAdminDBService.GetDataByID(id);

            userModel = userDBService.GetDataByID(User.Identity.Name);
            ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

            ReportAdminViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            return PartialView(ReportAdminViewModel);
        }
             
        [HttpPost]
        public ActionResult GetNewReportAdminDepartmentData(String id)
        {
            ReportAdminViewModel ReportAdminViewModel = new ReportAdminViewModel();
            List<ReportAdminDepartmentModel> dataList = new List<ReportAdminDepartmentModel>();

            dataList = reportAdminDBService.ListNewReportAdminDepartment(id);

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetReportAdminDepartmentData(String id)
        {
            ReportAdminViewModel ReportAdminViewModel = new ReportAdminViewModel();
            List<ReportAdminDepartmentModel> dataList = new List<ReportAdminDepartmentModel>();

            dataList = reportAdminDBService.ListReportAdminDepartment(id);

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddReportAdminDepartment(String reportAdminNRIC, String selectedDepartmentID)
        {
            if (reportAdminDBService.AddReportAdminDepartment(reportAdminNRIC, selectedDepartmentID).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult DeleteReportAdminDepartment(String reportAdminNRIC, String departmentID)
        {
            if (reportAdminDBService.DeleteReportAdminDepartment(reportAdminNRIC, departmentID).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }


    }
}