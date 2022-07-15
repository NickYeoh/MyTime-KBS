using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
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
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);
           
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        DataMaintenanceDBService dataMaintenanceDBService = new DataMaintenanceDBService();
               
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
            UserModel userModel = new UserModel();
            userModel = userDBService.GetDataByID(User.Identity.Name);

            dataMaintenanceDBService.CloseLastMonthAttendanceData(userModel);

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
          
        }

        [HttpPost]
        public ActionResult ManualGenerateLastMonthAttendanceCardStatus()
        {

            UserModel userModel = new UserModel();
            userModel = userDBService.GetDataByID(User.Identity.Name);

            dataMaintenanceDBService.GenerateLastMonthAttendanceCardStatus(userModel);

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

    }
}