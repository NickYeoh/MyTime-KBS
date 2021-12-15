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
    public class AccessRoleController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        AccessRoleDBService accessRoleDBService = new AccessRoleDBService();
        DeviceDBService deviceDBService = new DeviceDBService();

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        // GET: AccessRole
        public ActionResult Index()
        {

            UserModel userModel = new UserModel();
            AccessRoleViewModel accessRoleViewModel = new AccessRoleViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                accessRoleViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            }

            return View(accessRoleViewModel);

        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<AccessRoleModel> dataList = new List<AccessRoleModel>();

            dataList = accessRoleDBService.ListAccessRole();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _Create()
        {            
            AccessRoleModel accessRoleModel = new AccessRoleModel();
            List<DeviceModel> deviceList = new List<DeviceModel>();
       
            accessRoleModel.AccessRoleName = "";
            accessRoleModel.IsActivated = true;

         
            return PartialView(accessRoleModel);
        }

        [HttpPost]
        public ActionResult _Create(AccessRoleModel accessRoleModel)
        {
            List<DeviceModel> deviceList = new List<DeviceModel>();

          
            if (ModelState.IsValid)
            {
                if (accessRoleDBService.CheckDuplicateName(accessRoleModel.AccessRoleName).Equals(true))
                {
                    ModelState.AddModelError("AccessRoleName", MyTime.Resource.AccessRoleNameDuplicated);
                    
                    return PartialView(accessRoleModel);           

                }

                if (accessRoleDBService.Create(accessRoleModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(accessRoleModel);

        }

        public ActionResult _Update(int ID)
        {
            AccessRoleModel accessRoleModel = new AccessRoleModel();

            accessRoleModel = accessRoleDBService.GetDataByID(ID);

            return PartialView(accessRoleModel);
        }

        [HttpPost]
        public ActionResult _Update(AccessRoleModel accessRoleModel)
        {

            if (ModelState.IsValid)
            {

                if (accessRoleDBService.Update(accessRoleModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(accessRoleModel);

        }

        public ActionResult _Delete(int ID)
        {
            AccessRoleModel accessRoleModel = new AccessRoleModel();

            accessRoleModel = accessRoleDBService.GetDataByID(ID);

            return PartialView(accessRoleModel);
        }

        [HttpPost]
        public ActionResult _Delete(AccessRoleModel accessRoleModel)
        {
            if (ModelState.IsValid)
            {
                if (accessRoleDBService.Delete(accessRoleModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(accessRoleModel);

        }

        public ActionResult _ListNewAccessRoleDevice(string id)
        {
            UserModel userModel = new UserModel();
            AccessRoleViewModel accessRoleViewModel = new AccessRoleViewModel();

            accessRoleViewModel.AccessRoleModel = accessRoleDBService.GetDataByID(Convert.ToInt32(id));

            userModel = userDBService.GetDataByID(User.Identity.Name);
            ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

            accessRoleViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            return PartialView(accessRoleViewModel);
        }


        [HttpPost]
        public ActionResult ListNewDevice(string id, bool IsOvertimeExtraDevice)
        {
            AccessRoleViewModel accessRoleViewModel = new AccessRoleViewModel();
            List<DeviceModel> deviceList = new List<DeviceModel>();

            List<AccessRoleDeviceModel> selectedDeviceList = new List<AccessRoleDeviceModel>();

            //var result = list1.Where(p => list2.All(x => x.Id != p.Id));
            string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

            //if (IsOvertimeExtraDevice == false)
            //{
            //    ViewBag.Title = MyTime.Resource.AccessRole + " :: " + MyTime.Resource.SelectDevice;
            //}
            //else
            //{

            //    ViewBag.Title = MyTime.Resource.AccessRole + " :: " + MyTime.Resource.OvertimeDevice;
            //}
        

            bool isIncludedAll = true;
            //bool IsOvertimeExtraDevice = false;

            switch (device)
            {
                case "JohnsonControl":

                    selectedDeviceList = accessRoleDBService.ListJohnsonControlAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    deviceList = deviceDBService.ListJohnsonControlDevice().Where(d => selectedDeviceList.All(sd => sd.DeviceID != d.DeviceID)).ToList();
                    break;

                case "Suprema":

                    selectedDeviceList = accessRoleDBService.ListSupremaAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    deviceList = deviceDBService.ListSupremaDevice().Where(d => selectedDeviceList.All(sd => sd.DeviceID != d.DeviceID)).ToList();
                    break;

                default:

                    selectedDeviceList = accessRoleDBService.ListJohnsonControlAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    deviceList = deviceDBService.ListJohnsonControlDevice().Where(d => selectedDeviceList.All(sd => sd.DeviceID != d.DeviceID)).ToList();
                    break;


            }

            return Json(deviceList, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult AssignAccessRoleDevice(string accessRoleID, string selectedDeviceID, bool IsOvertimeExtraDevice)
        {
            if (accessRoleDBService.AddAccessRoleDevice(Convert.ToInt32(accessRoleID), selectedDeviceID, IsOvertimeExtraDevice).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult DeleteAccessRoleDevice(string accessRoleID, string deviceID)
        {
            if (accessRoleDBService.DeleteAccessRoleDevice(Convert.ToInt32(accessRoleID), deviceID).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetAccessRoleDeviceData(string id)
        {
            AccessRoleDeviceModel accessRoleDeviceModel = new AccessRoleDeviceModel();
            List<AccessRoleDeviceModel> accessRoleDeviceList = new List<AccessRoleDeviceModel>();

            string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

            bool isIncludedAll = false;
            bool IsOvertimeExtraDevice = false;


            switch (device)
            {
                case "JohnsonControl":

                    accessRoleDeviceList = accessRoleDBService.ListJohnsonControlAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    break;

                case "Suprema":

                    accessRoleDeviceList = accessRoleDBService.ListSupremaAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    break;

                default:

                    accessRoleDeviceList = accessRoleDBService.ListJohnsonControlAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    break;

            }
           

            return Json(accessRoleDeviceList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAccessRoleOvertimeDeviceData(string id)
        {
            AccessRoleDeviceModel accessRoleDeviceModel = new AccessRoleDeviceModel();
            List<AccessRoleDeviceModel> accessRoleDeviceList = new List<AccessRoleDeviceModel>();

            string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

            bool isIncludedAll = false;
            bool IsOvertimeExtraDevice = true;


            switch (device)
            {
                case "JohnsonControl":

                    accessRoleDeviceList = accessRoleDBService.ListJohnsonControlAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    break;

                case "Suprema":

                    accessRoleDeviceList = accessRoleDBService.ListSupremaAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    break;

                default:

                    accessRoleDeviceList = accessRoleDBService.ListJohnsonControlAccessRoleDevice(Convert.ToInt32(id), isIncludedAll, IsOvertimeExtraDevice);
                    break;

            }


            return Json(accessRoleDeviceList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _ListAccessRoleDevice(string id)
        {         
            AccessRoleModel accessRoleModel = new AccessRoleModel();
          
            accessRoleModel = accessRoleDBService.GetDataByID(Convert.ToInt32( id));
            
            return PartialView(accessRoleModel);
        }

        [HttpPost]
        public ActionResult _ListAccessRoleDevice(AccessRoleModel accessRoleModel)
        {
            List<DeviceModel> deviceList = new List<DeviceModel>();

            if (! accessRoleDBService.UpdateAccessRoleDevice(accessRoleModel.AccessRoleID, deviceList).Equals(true))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

    }
}