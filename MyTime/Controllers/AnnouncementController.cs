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
    public class AnnouncementController : EnvironmentController
    {
   
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        AnnouncementDBService announcementDBService = new AnnouncementDBService();
    
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: Announcement
        public ActionResult Index()
        {


            UserModel userModel = new UserModel();
            AnnouncementViewModel announcementViewModel = new AnnouncementViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                announcementViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            }

            return View(announcementViewModel);

        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<AnnouncementModel> dataList = new List<AnnouncementModel>();

            dataList = announcementDBService.ListAnnouncement();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Create()
        {
            AnnouncementModel announcementModel = new AnnouncementModel();

            announcementModel.AnnouncementMessage = "";
         
            return PartialView(announcementModel);
        }

        [HttpPost]
       
        public ActionResult _Create(AnnouncementModel announcementModel)
        {
            if (ModelState.IsValid)
            {
               
                if (announcementDBService.Create(announcementModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(announcementModel);

        }

        public ActionResult _Update(int ID)
        {
            AnnouncementModel announcementModel = new AnnouncementModel();

            announcementModel = announcementDBService.GetDataByID(ID);

            return PartialView(announcementModel);
        }

        [HttpPost]
       
        public ActionResult _Update(AnnouncementModel announcementModel)
        {

            if (ModelState.IsValid)
            {

                if (announcementDBService.Update(announcementModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(announcementModel);

        }

        public ActionResult _Delete(int ID)
        {
            AnnouncementModel announcementModel = new AnnouncementModel();

            announcementModel = announcementDBService.GetDataByID(ID);

            return PartialView(announcementModel);
        }

        [HttpPost]
       
        public ActionResult _Delete(AnnouncementModel announcementModel)
        {
            if (ModelState.IsValid)
            {
                if (announcementDBService.Delete(announcementModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(announcementModel);

        }

    }
}