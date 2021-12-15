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
    public class DepartmentController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        DepartmentDBService departmentDBService = new DepartmentDBService();
      

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: Department
        public ActionResult Index()
        {

            UserModel userModel = new UserModel();
            DepartmentViewModel departmentViewModel = new DepartmentViewModel();
            
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                departmentViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);
            }

            return View(departmentViewModel);
            
        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<DepartmentModel> dataList = new List<DepartmentModel>();

            dataList = departmentDBService.ListDepartment();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Create()
        {
            DepartmentModel departmentModel = new DepartmentModel();

            departmentModel.DepartmentID = "";
            departmentModel.DepartmentName = "";
            departmentModel.IsActivated = true;

            return PartialView(departmentModel);
        }

        [HttpPost]       
        public ActionResult _Create(DepartmentModel departmentModel)
        {

            if (ModelState.IsValid)
            {
                if (departmentDBService.CheckDuplicateID(departmentModel.DepartmentID).Equals(true))
                {
                    ModelState.AddModelError("DepartmentID", MyTime.Resource.DepartmentIDDuplicated);
                    return PartialView(departmentModel);

                } else if (departmentDBService.CheckDuplicateName(departmentModel.DepartmentName).Equals(true))
                {
                    ModelState.AddModelError("DepartmentName", MyTime.Resource.DepartmentNameDuplicated);
                    return PartialView(departmentModel);
                }

                if (departmentDBService.Create(departmentModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);                    
                }   
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(departmentModel);

        }

        public ActionResult _Update(string ID)
        {
            DepartmentModel departmentModel = new DepartmentModel();

            departmentModel = departmentDBService.GetDataByID(ID);

            return PartialView(departmentModel);
        }

        [HttpPost]       
        public ActionResult _Update(DepartmentModel departmentModel)
        {

            if (ModelState.IsValid)
            {

                if (departmentDBService.Update(departmentModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(departmentModel);

        }

        public ActionResult _Delete(string ID)
        {
            DepartmentModel departmentModel = new DepartmentModel();

            departmentModel = departmentDBService.GetDataByID(ID);

            return PartialView(departmentModel);
        }

        [HttpPost]       
        public ActionResult _Delete(DepartmentModel departmentModel)
        {
            if (ModelState.IsValid)
            {
                if (departmentDBService.Delete(departmentModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(departmentModel);

        }

      
    }
}