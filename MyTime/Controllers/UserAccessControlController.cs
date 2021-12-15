using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.Services;
using MyTime.Models;
using MyTime.ViewModels;

namespace MyTime.Controllers
{
    public class UserAccessControlController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        RoleDBService roleDBService = new RoleDBService();
      
        // GET: AccessControl
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            UserAccessControlViewModel userAccessControlViewModel = new UserAccessControlViewModel();


            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {

                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                userAccessControlViewModel.RoleList = roleDBService.ListRole().OrderBy(r => r.RoleName).ToList();
                userAccessControlViewModel.SelectListRole = PrepareSelectListRole(userAccessControlViewModel.RoleList);

                if (userAccessControlViewModel.RoleList.Count >= 0)
                {
                    userAccessControlViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userAccessControlViewModel.RoleList[0].RoleID);

                }

            }

            return View(userAccessControlViewModel);
        }

        [HttpPost]
        public ActionResult Index(UserAccessControlViewModel userAccessControlViewModel)
        {

            UserAccessControlModel userAccessControlModel = new UserAccessControlModel();
            userAccessControlModel = userAccessControlViewModel.UserAccessControlModel;

            if (userAccessControlDBService.Update(userAccessControlModel).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SelectListItem> PrepareSelectListRole(List<RoleModel> roleList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in roleList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.RoleID.ToString(),
                    Text = row.RoleName.ToString()
                });
            }
            return selectList;
        }


        public ActionResult ListFunctionAccessStatus(int roleID)
        {
            UserAccessControlModel userAccessControlModel = new UserAccessControlModel();

            userAccessControlModel = userAccessControlDBService.IsAccessAllowed(roleID);

            return Json(userAccessControlModel, JsonRequestBehavior.AllowGet);
        }

    }
}