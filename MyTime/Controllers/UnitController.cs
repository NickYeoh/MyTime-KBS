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
    public class UnitController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        DepartmentDBService departmentDBService = new DepartmentDBService();
        UnitDBService unitDBService = new UnitDBService();
         

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: Unit
        public ActionResult Index()
        {

            UserModel userModel = new UserModel();          
            UnitViewModel unitViewModel = new UnitViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                unitViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            }

            return View(unitViewModel);

        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<UnitModel> dataList = new List<UnitModel>();

            dataList = unitDBService.ListUnit();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SelectListItem> PrepareSelectList(List<DepartmentModel> DepartmentList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in DepartmentList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.DepartmentID.ToString(),
                    Text = row.DepartmentName.ToString()
                });
            }
            return selectList;
        }

        public ActionResult _Create()
        {
            UnitViewModel unitViewModel = new UnitViewModel();

            List<DepartmentModel> DepartmentList = new List<DepartmentModel>();
            DepartmentList = departmentDBService.ListDepartment();

            unitViewModel.IsActivated = true;
            unitViewModel.SelectListDepartment = PrepareSelectList(DepartmentList);

            return PartialView(unitViewModel);
        }

        [HttpPost]
       
        public ActionResult _Create(UnitViewModel unitViewModel)
        {
            List<DepartmentModel> DepartmentList = new List<DepartmentModel>();

            if (ModelState.IsValid)
            {
                if (unitDBService.CheckDuplicateName(unitViewModel.DepartmentID, unitViewModel.UnitName).Equals(true))
                {
                    ModelState.AddModelError("UnitName", MyTime.Resource.UnitNameDuplicated);
                                
                    DepartmentList = departmentDBService.ListDepartment();
                    unitViewModel.SelectListDepartment = PrepareSelectList(DepartmentList);

                    return PartialView(unitViewModel);
                }

                if (unitDBService.Create(unitViewModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            DepartmentList = departmentDBService.ListDepartment();
            unitViewModel.SelectListDepartment = PrepareSelectList(DepartmentList);

            return PartialView(unitViewModel);

        }

        public ActionResult _Update(string ID)
        {
            UnitModel unitModel = new UnitModel();

            unitModel = unitDBService.GetDataByID(ID);

            return PartialView(unitModel);
        }

        [HttpPost]       
        public ActionResult _Update(UnitModel unitModel)
        {
            if (ModelState.IsValid)
            {

                if (unitDBService.Update(unitModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(unitModel);

        }

        public ActionResult _Delete(string ID)
        {
            UnitModel unitModel = new UnitModel();

            unitModel = unitDBService.GetDataByID(ID);

            return PartialView(unitModel);
        }

        [HttpPost]       
        public ActionResult _Delete(UnitModel unitModel)
        {
            if (ModelState.IsValid)
            {

                if (unitDBService.Delete(unitModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(unitModel);

        }
    }
}