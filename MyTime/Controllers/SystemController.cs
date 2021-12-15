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
    public class SystemController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        SystemDBService systemDBService = new SystemDBService();
        RoleDBService roleDBService = new RoleDBService();
        ShiftDBService shiftDBService = new ShiftDBService();
        AccessRoleDBService accessRoleDBService = new AccessRoleDBService();

        // GET: System
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            SystemViewModel systemViewModel = new SystemViewModel();

            List<RoleModel> roleList = new List<RoleModel>();
            List<ShiftModel> shiftList = new List<ShiftModel>();
            List<AccessRoleModel> accessRoleList = new List<AccessRoleModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                roleList = roleDBService.ListRole().OrderBy(r => r.RoleName).ToList();
                shiftList = shiftDBService.ListShift().OrderBy(s => s.ShiftName).ToList();
                accessRoleList = accessRoleDBService.ListAccessRole().OrderBy(a => a.AccessRoleName).ToList();

                systemViewModel.SystemModel = systemDBService.GetData();
                systemViewModel.SelectListRole = PrepareSelectListRole(roleList);
                systemViewModel.SelectListShift = PrepareSelectListShift(shiftList);
                systemViewModel.SelectListAccessRole = PrepareSelectListAccessRole(accessRoleList);
                systemViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);
            }

            return View(systemViewModel);
        }

        [HttpPost]
        public ActionResult Index(SystemViewModel systemViewModel)
        {
            UserModel userModel = new UserModel();
            List<RoleModel> roleList = new List<RoleModel>();
            List<ShiftModel> shiftList = new List<ShiftModel>();
            List<AccessRoleModel> accessRoleList = new List<AccessRoleModel>();

            userModel = userDBService.GetDataByID(User.Identity.Name);
            ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

            roleList = roleDBService.ListRole().OrderBy(r => r.RoleName).ToList();
            shiftList = shiftDBService.ListShift().OrderBy(s => s.ShiftName).ToList();
            accessRoleList = accessRoleDBService.ListAccessRole().OrderBy(a => a.AccessRoleName).ToList();

            systemViewModel.SelectListRole = PrepareSelectListRole(roleList);
            systemViewModel.SelectListShift = PrepareSelectListShift(shiftList);
            systemViewModel.SelectListAccessRole = PrepareSelectListAccessRole(accessRoleList);
            systemViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            //if (ModelState.IsValid)
            //{
            //HttpPostedFileBase organisationLogoFile = Request.Files["OrganisationLogoFile"];
            SystemModel systemModel = new SystemModel();
            systemModel = systemViewModel.SystemModel;

            string path = Server.MapPath("~/Images/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string organisationLogoFilename;
            string destination;

            if (systemModel.PostedOrganisationLogo != null)
            {
                organisationLogoFilename = systemModel.PostedOrganisationLogo.FileName;

                if (!organisationLogoFilename.Equals(""))
                {
                    //destination = Path.Combine(HttpContext.Server.MapPath("/Images/" + organisationLogoFilename));
                    //destination = Path.Combine(HttpContext.Server.MapPath("/Images/" + organisationLogoFilename));
                    destination = path + organisationLogoFilename;
                    systemModel.PostedOrganisationLogo.SaveAs(destination);

                    systemModel.OrganisationLogo = organisationLogoFilename;
                }
                else
                {
                    systemModel.OrganisationLogo = "";
                }

            }
            else
            {
                systemModel.OrganisationLogo = "";
            }

            if (!systemDBService.Update(systemModel).Equals(true))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

            }


            // Update the session
            Session["OrganisationName"] = systemModel.OrganisationName;
            Session["OrganisationShortName"] = systemModel.OrganisationShortName;
            Session["OrganisationLogo"] = systemModel.OrganisationLogo;

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{

            //    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            //}
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


        private IEnumerable<SelectListItem> PrepareSelectListShift(List<ShiftModel> shiftList)
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


        private IEnumerable<SelectListItem> PrepareSelectListAccessRole(List<AccessRoleModel> accessRoleList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in accessRoleList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.AccessRoleID.ToString(),
                    Text = row.AccessRoleName.ToString()
                });
            }
            return selectList;
        }

    }
}