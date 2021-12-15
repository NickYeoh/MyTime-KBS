using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyTime.Models;
using MyTime.Services;

namespace MyTime.Controllers
{
    public class AuthController : EnvironmentController
    {
        readonly UserDBService userDBService = new UserDBService();
        readonly LogActivityDBService logActivityDBService = new LogActivityDBService();

        SystemDBService systemDBService = new SystemDBService();

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: Auth
        [HttpGet]
        public ActionResult Index()
        {
            SystemModel systemModel = new SystemModel();

            systemModel = systemDBService.GetData();

            Session["OrganisationName"] = systemModel.OrganisationName;
            Session["OrganisationShortName"] = systemModel.OrganisationShortName;
            Session["OrganisationLogo"] = systemModel.OrganisationLogo;

            Session["IsEmailNotificationEnabled"] = systemModel.IsEmailNotificationEnabled;
            Session["IsEmailReminderEnabled"] = systemModel.IsEmailReminderEnabled;

            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Auth");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "NRIC, Password")] AuthModel authModel)
        {
            if (userDBService.CheckIsUserExist(authModel.NRIC).Equals(true))
            {
                if (userDBService.AuthUser(authModel) != true)
                {
                    ModelState.AddModelError("Password", MyTime.Resource.InvalidPassword);

                    logActivityDBService.LogActivity(authModel.NRIC, "Auth", $@"Password not valid; {authModel.Password}", DateTime.Now);

                    return View(authModel);

                }
                else
                {
                    logActivityDBService.LogActivity(authModel.NRIC, "Auth", $@"Auth passed", DateTime.Now);

                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
                }
              
            }
            else
            {
                ModelState.AddModelError("NRIC", MyTime.Resource.InvalidNRIC);
                
                logActivityDBService.LogActivity(authModel.NRIC, "Auth", $@"NRIC not valid; {authModel.NRIC}", DateTime.Now);

                return View(authModel);

            }


            //else 
            //{
            //    //Get user data from device

            //    UserModel deviceUserModel = new UserModel();

            //    string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

            //    if ( ! device.Equals("Suprema"))
            //    {
            //        deviceUserModel = userDBService.GetJohnsonControlUserDataByID(authModel.NRIC);
            //    }
            //    else
            //    {
            //        deviceUserModel = userDBService.GetSupremaUserDataByID(authModel.NRIC);
            //    }

            //    if (! string.IsNullOrWhiteSpace( deviceUserModel.NRIC))
            //    {
            //        if (! string.IsNullOrWhiteSpace(authModel.Password) && !authModel.Password.Equals("abc123"))
            //        {
            //            ModelState.AddModelError("Password", MyTime.Resource.InvalidPassword);

            //            logActivityDBService.LogActivity(authModel.NRIC, "Auth", $@"Password not valid; {authModel.Password}", DateTime.Now);

            //            return View(authModel);
            //        }
            //        else
            //        {
            //            // Device User ( New User )
            //            logActivityDBService.LogActivity(authModel.NRIC, "Auth", $@"Auth passed", DateTime.Now);

            //            FormsAuthentication.SetAuthCookie(authModel.NRIC, false);
            //            return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            //        }

            //    }
            //    else
            //    {
            //        // NRIC not registered in device 

            //        logActivityDBService.LogActivity(authModel.NRIC, "Auth", $@"NRIC not valid; {authModel.NRIC}", DateTime.Now);

            //        ModelState.AddModelError("NRIC", MyTime.Resource.InvalidNRIC);
            //        return View(authModel);
            //    }
            //}

        }

        public ActionResult SwitchLanguage(string lang)
        {
            new SwitchLanguage().SetLanguage(lang);

            return RedirectToAction("Index", "Auth");
   
        }
    }
}