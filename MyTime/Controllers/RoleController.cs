using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;

namespace MyTime.Controllers
{
    public class RoleController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        RoleDBService roleDBService = new RoleDBService();
     
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: Role
        public ActionResult Index()
        {


            UserModel userModel = new UserModel();
            RoleViewModel roleViewModel = new RoleViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                roleViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            }

            return View(roleViewModel);

        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<RoleModel> dataList = new List<RoleModel>();

            dataList = roleDBService.ListRole();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Create()
        {
            RoleModel roleModel = new RoleModel();

            roleModel.RoleName = "";
            roleModel.IsActivated = true;

            return PartialView(roleModel);
        }

        [HttpPost]
       
        public ActionResult _Create(RoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                if (roleDBService.CheckDuplicateName(roleModel.RoleName).Equals(true))
                {
                    ModelState.AddModelError("RoleName", MyTime.Resource.RoleNameDuplicated);
                    return PartialView(roleModel);
                }

                if (roleDBService.Create(roleModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(roleModel);

        }

        public ActionResult _Update(int ID)
        {
            RoleModel roleModel = new RoleModel();

            roleModel = roleDBService.GetDataByID(ID);

            return PartialView(roleModel);
        }

        [HttpPost]
       
        public ActionResult _Update(RoleModel roleModel)
        {

            if (ModelState.IsValid)
            {

                if (roleDBService.Update(roleModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(roleModel);

        }

        public ActionResult _Delete(int ID)
        {
            RoleModel roleModel = new RoleModel();

            roleModel = roleDBService.GetDataByID(ID);

            return PartialView(roleModel);
        }

        [HttpPost]
       
        public ActionResult _Delete(RoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                if (roleDBService.Delete(roleModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(roleModel);

        }

    }
}