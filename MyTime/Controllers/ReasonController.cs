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
    public class ReasonController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        ReasonDBService reasonDBService = new ReasonDBService();
      

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: Reason
        public ActionResult Index()
        {

            UserModel userModel = new UserModel();
            ReasonViewModel reasonViewModel = new ReasonViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                reasonViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            }

            return View(reasonViewModel);

        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<ReasonModel> dataList = new List<ReasonModel>();

            dataList = reasonDBService.ListReason();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Create()
        {
            ReasonModel reasonModel = new ReasonModel();

            reasonModel.ReasonID = "";
            reasonModel.ReasonName = "";
            reasonModel.IsActivated = true;

            return PartialView(reasonModel);
        }

        [HttpPost]
       
        public ActionResult _Create(ReasonModel reasonModel)
        {

            if (ModelState.IsValid)
            {
                if (reasonDBService.CheckDuplicateID(reasonModel.ReasonID).Equals(true))
                {
                    ModelState.AddModelError("ReasonID", MyTime.Resource.ReasonIDDuplicated);
                    return PartialView(reasonModel);

                }
                else if (reasonDBService.CheckDuplicateName(reasonModel.ReasonName).Equals(true))
                {
                    ModelState.AddModelError("ReasonName", MyTime.Resource.ReasonNameDuplicated);
                    return PartialView(reasonModel);
                }

                if (reasonDBService.Create(reasonModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(reasonModel);

        }

        public ActionResult _Update(string ID)
        {
            ReasonModel reasonModel = new ReasonModel();

            reasonModel = reasonDBService.GetDataByID(ID);

            return PartialView(reasonModel);
        }

        [HttpPost]       
        public ActionResult _Update(ReasonModel reasonModel)
        {

            if (ModelState.IsValid)
            {

                if (reasonDBService.Update(reasonModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(reasonModel);

        }

        public ActionResult _Delete(string ID)
        {
            ReasonModel reasonModel = new ReasonModel();

            reasonModel = reasonDBService.GetDataByID(ID);

            return PartialView(reasonModel);
        }

        [HttpPost]       
        public ActionResult _Delete(ReasonModel reasonModel)
        {
            if (ModelState.IsValid)
            {
                if (reasonDBService.Delete(reasonModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(reasonModel);

        }

    }
}