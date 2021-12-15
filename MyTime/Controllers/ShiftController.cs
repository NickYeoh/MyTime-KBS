using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;
using System.Globalization;

namespace MyTime.Controllers
{
    public class ShiftController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        ShiftDBService shiftDBService = new ShiftDBService();
      
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: Shift
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            ShiftViewModel shiftViewModel = new ShiftViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                shiftViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);
            }

            return View(shiftViewModel);

        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<ShiftModel> dataList = new List<ShiftModel>();

            dataList = shiftDBService.ListShift();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Create()
        {
            ShiftModel shiftModel = new ShiftModel();

            shiftModel.ShiftID = "";
            shiftModel.ShiftName = "";

            shiftModel.IsWorkDay1 = true;
            shiftModel.TimeIn1 = "08:30";
            shiftModel.TimeOut1 = "16:30";
            shiftModel.FlexiTimeInterval1 = 0 ;

            shiftModel.IsWorkDay2 = true;
            shiftModel.TimeIn2 ="08:30";
            shiftModel.TimeOut2 = "16:30";
            shiftModel.FlexiTimeInterval2 = 0;

            shiftModel.IsWorkDay3 = true;
            shiftModel.TimeIn3 = "08:30";
            shiftModel.TimeOut3 = "16:30";
            shiftModel.FlexiTimeInterval3 = 0;

            shiftModel.IsWorkDay4 = true;
            shiftModel.TimeIn4 = "08:30";
            shiftModel.TimeOut4 = "16:30";
            shiftModel.FlexiTimeInterval4 = 0;

            shiftModel.IsWorkDay5 = true;
            shiftModel.TimeIn5 = "08:30";
            shiftModel.TimeOut5 = "16:30";
            shiftModel.FlexiTimeInterval5 = 0;

            shiftModel.IsWorkDay6 = false;
            shiftModel.TimeIn6 = "08:30";
            shiftModel.TimeOut6 = "16:30";
            shiftModel.FlexiTimeInterval6 = 0;

            shiftModel.IsWorkDay7 = false;
            shiftModel.TimeIn7 = "08:30";
            shiftModel.TimeOut7 = "16:30";
            shiftModel.FlexiTimeInterval7 = 0;

            shiftModel.IsActivated = true;

            return PartialView(shiftModel);
        }

        [HttpPost]
        public ActionResult _Create(ShiftModel shiftModel)
        {

            if (ModelState.IsValid)
            {
                if (shiftDBService.CheckDuplicateID(shiftModel.ShiftID).Equals(true))
                {
                    ModelState.AddModelError("ShiftID", MyTime.Resource.ShiftIDDuplicated);
                    return PartialView(shiftModel);

                }
                else if (shiftDBService.CheckDuplicateName(shiftModel.ShiftName).Equals(true))
                {
                    ModelState.AddModelError("ShiftName", MyTime.Resource.ShiftNameDuplicated);
                    return PartialView(shiftModel);
                }

                if (shiftDBService.Create(shiftModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(shiftModel);

        }

        public ActionResult _Update(string ID)
        {
            ShiftModel shiftModel = new ShiftModel();

            shiftModel = shiftDBService.GetDataByID(ID);

            return PartialView(shiftModel);
        }

        [HttpPost]
        public ActionResult _Update(ShiftModel shiftModel)
        {

            if (ModelState.IsValid)
            {

                if (shiftDBService.Update(shiftModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(shiftModel);

        }

        public ActionResult _Delete(string ID)
        {
            ShiftModel shiftModel = new ShiftModel();

            shiftModel = shiftDBService.GetDataByID(ID);

            return PartialView(shiftModel);
        }

        [HttpPost]
        public ActionResult _Delete(ShiftModel shiftModel)
        {
            if (ModelState.IsValid)
            {
                if (shiftDBService.Delete(shiftModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(shiftModel);

        }
    }
}