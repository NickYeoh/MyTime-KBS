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
    public class HolidayController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        HolidayDBService holidayDBService = new HolidayDBService();
      

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: Holiday
        public ActionResult Index()
        {

            UserModel userModel = new UserModel();

            HolidayViewModel holidayViewModel = new HolidayViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                holidayViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);
            }

            return View(holidayViewModel);
          
        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<HolidayModel> dataList = new List<HolidayModel>();

            dataList = holidayDBService.ListHoliday();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Create()
        {
            HolidayModel holidayModel = new HolidayModel();

            holidayModel.HolidayName = "";
            holidayModel.StartOn = DateTime.Now;
            holidayModel.EndOn = DateTime.Now;

            return PartialView(holidayModel);
        }
             
        [HttpPost]
       
        public ActionResult _Create(HolidayModel holidayModel)
        {

            if (ModelState.IsValid)
            {
            
                if (holidayDBService.Create(holidayModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
                
            }

            return PartialView( holidayModel);
           
        }

        public ActionResult _Update(int ID)
        {
            HolidayModel holidayModel = new HolidayModel();

            holidayModel = holidayDBService.GetDataByID(ID);

            return PartialView(holidayModel);
        }

        [HttpPost]
       
        public ActionResult _Update(HolidayModel holidayModel)
        {

            if (ModelState.IsValid)
            {

                if (holidayDBService.Update(holidayModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(holidayModel);

        }

        public ActionResult _Delete(int ID)
        {
            HolidayModel holidayModel = new HolidayModel();

            holidayModel = holidayDBService.GetDataByID(ID);

            return PartialView(holidayModel);
        }

        [HttpPost]
       
        public ActionResult _Delete(HolidayModel holidayModel)
        {
            if (ModelState.IsValid)
            {

                if (holidayDBService.Delete(holidayModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(holidayModel);

        }




    }
}