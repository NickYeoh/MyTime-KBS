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
    public class ApproverController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        ApproverDBService approverDBService = new ApproverDBService();
        DepartmentDBService departmentDBService = new DepartmentDBService();

        // GET: System
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            ApproverViewModel approverViewModel = new ApproverViewModel();


            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                approverViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            }

            return View(approverViewModel);
        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<ApproverModel> dataList = new List<ApproverModel>();

            dataList = approverDBService.ListApprover();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _ListApproverUser(String id)
        {
            UserModel userModel = new UserModel();
            ApproverViewModel approverViewModel = new ApproverViewModel();

            approverViewModel.ApproverModel = approverDBService.GetDataByID(id);

            userModel = userDBService.GetDataByID(User.Identity.Name);
            ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

            approverViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            return PartialView(approverViewModel);
        }

        public ActionResult _ListNewApproverUser(String id)
        {
            UserModel userModel = new UserModel();
            ApproverViewModel approverViewModel = new ApproverViewModel();
            
            approverViewModel.ApproverModel = approverDBService.GetDataByID(id);

            userModel = userDBService.GetDataByID(User.Identity.Name);
            ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

            approverViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            return PartialView(approverViewModel);
        }

        
        [HttpPost]
        public ActionResult GetNewApproverUserData(String id)
        {
            ApproverViewModel approverViewModel = new ApproverViewModel();
            List<ApproverUserModel> dataList = new List<ApproverUserModel>();

            dataList = approverDBService.ListNewApproverUser(id);

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetApproverUserData(String id)
        {
            ApproverViewModel approverViewModel = new ApproverViewModel();
            List<ApproverUserModel> dataList = new List<ApproverUserModel>();

            dataList = approverDBService.ListApproverUser(id);

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddApproverUser(String approverNRIC, String selectedUserNRIC)
        {
            if (approverDBService.AddApproverUser(approverNRIC, selectedUserNRIC).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult DeleteApproverUser(String approverNRIC, String userNRIC)
        {
            if (approverDBService.DeleteApproverUser(approverNRIC, userNRIC).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

    }
}