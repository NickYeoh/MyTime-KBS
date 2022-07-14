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
    public class DataMaintenanceController : EnvironmentController
    {

        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        SystemDBService systemDBService = new SystemDBService();
        RoleDBService roleDBService = new RoleDBService();
        ShiftDBService shiftDBService = new ShiftDBService();
        AccessRoleDBService accessRoleDBService = new AccessRoleDBService();


        // GET: DataMaintenance
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
          
            DataMaintenanceViewModel dataMaintenanceViewModel = new DataMaintenanceViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                dataMaintenanceViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);
            }

            return View(dataMaintenanceViewModel);
        }

        [HttpPost]
        public ActionResult ManualCloseLastMonthAttendanceData()
        {
          
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
          
        }

        [HttpPost]
        public ActionResult ManualGenerateLastMonthAttendanceCardStatus()
        {
            
            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

    }
}