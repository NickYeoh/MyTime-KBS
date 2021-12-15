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
    public class ShiftScheduleController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
       
        ShiftDBService shiftDBService = new ShiftDBService();
        ShiftScheduleDBService shiftScheduleDBService = new ShiftScheduleDBService();
       
        // GET: System
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            ShiftScheduleViewModel shiftScheduleViewModel = new ShiftScheduleViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                shiftScheduleViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            }

            return View(shiftScheduleViewModel);
        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<ShiftScheduleModel> dataList = new List<ShiftScheduleModel>();

            dataList = shiftScheduleDBService.ListShiftSchedule();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _ListUser()
        {
            UserModel userModel = new UserModel();
            ShiftScheduleViewModel shiftScheduleViewModel = new ShiftScheduleViewModel();

            List <ShiftModel> shiftList = new List<ShiftModel>();
            ShiftScheduleModel shiftSchedule = new ShiftScheduleModel();
            
            userModel = userDBService.GetDataByID(User.Identity.Name);
            ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

            shiftScheduleViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            shiftList = shiftDBService.ListShift().Where(s => s.IsActivated.Equals(true)).OrderBy(s => s.ShiftName).Select(s => s).ToList();
            shiftScheduleViewModel.ShiftList = shiftList;
            shiftScheduleViewModel.SelectListShift = PrepareSelectList(shiftList);

            shiftSchedule.EffectiveOn = DateTime.Now;
            shiftScheduleViewModel.ShiftScheduleModel = shiftSchedule;

            return PartialView(shiftScheduleViewModel);
        }
        
        
        public ActionResult ListUser()
        {
            List<UserModel> dataList = new List<UserModel>();

            dataList = userDBService.ListUser();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }
             

        [HttpPost]
        public ActionResult Create(String selectedNRIC, DateTime effectiveOn, String shiftID)
        {  

            if (shiftScheduleDBService.Create(selectedNRIC, effectiveOn, shiftID).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

        private IEnumerable<SelectListItem> PrepareSelectList(List<ShiftModel> shiftList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in shiftList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.ShiftID.ToString(),
                    Text = row.ShiftName.ToString()
                });
            }
            return selectList;
        }


    }
}