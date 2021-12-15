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
    public class UserController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        DepartmentDBService departmentDBService = new DepartmentDBService();
        UnitDBService unitDBService = new UnitDBService();
        RoleDBService roleDBService = new RoleDBService();
        AccessRoleDBService accessRoleDBService = new AccessRoleDBService();
        ShiftDBService shiftDBService = new ShiftDBService();

        // GET: User
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            UserViewModel userViewModel = new UserViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                userViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);
            }

            return View(userViewModel);
        }

        public ActionResult GetData(string ID)
        {
            UserViewModel userViewModel = new UserViewModel();
            List<DepartmentModel> departmentList = new List<DepartmentModel>();
            List<UnitModel> unitList = new List<UnitModel>();
            List<RoleModel> roleList = new List<RoleModel>();
            List<AccessRoleModel> accessRoleList = new List<AccessRoleModel>();

            departmentList = departmentDBService.ListDepartment();
            unitList = unitDBService.ListUnit();
            roleList = roleDBService.ListRole();
            accessRoleList = accessRoleDBService.ListAccessRole();

            userViewModel.UserModel = userDBService.GetDataByID(ID);

            // LINQ - Filter IsActivated = True
            departmentList = departmentList.Where(d => d.IsActivated.Equals(true) || d.DepartmentID.Equals(userViewModel.UserModel.DepartmentID)).OrderBy(d => d.DepartmentName).Select(d => d).ToList();
            unitList = unitList.Where(u => u.IsActivated.Equals(true) || u.UnitID.Equals(userViewModel.UserModel.UnitID)).OrderBy(u => u.UnitName).Select(u => u).ToList();

            userViewModel.DepartmentList = departmentList;
            userViewModel.UnitList = unitList;

            userViewModel.SelectListDepartment = PrepareSelectListDepartment(departmentList);
            userViewModel.SelectListUnit = PrepareSelectListUnit(unitList);
            userViewModel.SelectListRole = PrepareSelectListRole(roleList);
            userViewModel.SelectListAccessRole = PrepareSelectListAccessRole(accessRoleList);

            return View(userViewModel);
        }

        [HttpPost]
        public ActionResult GetData()
        {
            List<UserModel> dataList = new List<UserModel>();

            dataList = userDBService.ListUser();

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Update(string ID)
        {
            UserViewModel userViewModel = new UserViewModel();

            // User Model
            UserModel userModel = new UserModel();
            userModel = userDBService.GetDataByID(ID);
            userViewModel.UserModel = userModel;

            // Department and list Models
            List<DepartmentModel> departmentList = new List<DepartmentModel>();
            List<UnitModel> unitList = new List<UnitModel>();
            List<RoleModel> roleList = new List<RoleModel>();
            List<AccessRoleModel> accessRoleList = new List<AccessRoleModel>();

            departmentList = departmentDBService.ListDepartment();
            unitList = unitDBService.ListUnit(userModel.DepartmentID);
            roleList = roleDBService.ListRole();
            accessRoleList = accessRoleDBService.ListAccessRole();

            // LINQ - Filter IsActivated = True
            departmentList = departmentList.Where(d => d.IsActivated.Equals(true) || d.DepartmentID.Equals(userModel.DepartmentID)).OrderBy(d => d.DepartmentName).Select(d => d).ToList();
            unitList = unitList.Where(u => u.IsActivated.Equals(true) || u.UnitID.Equals(userModel.UnitID)).OrderBy(u => u.UnitName).Select(u => u).ToList();
            roleList = roleList.Where(r => r.IsActivated.Equals(true) || r.RoleID.Equals(userModel.RoleID)).OrderBy(r => r.RoleName).Select(r => r).ToList();
            accessRoleList = accessRoleList.Where(r => r.IsActivated.Equals(true) || r.AccessRoleID.Equals(userModel.AccessRoleID)).OrderBy(r => r.AccessRoleName).Select(r => r).ToList();

            userViewModel.SelectListDepartment = PrepareSelectListDepartment(departmentList);
            userViewModel.SelectListUnit = PrepareSelectListUnit(unitList);
            userViewModel.SelectListRole = PrepareSelectListRole(roleList);
            userViewModel.SelectListAccessRole = PrepareSelectListAccessRole(accessRoleList);

            return PartialView(userViewModel);
        }

        [HttpPost]

        //public ActionResult _Update([Bind(Exclude ="USRID, Password")] UserViewModel userViewModel)
        //public ActionResult _Update([Bind(Include = "NRIC, UserName, Gender, ContactNo, Email, RoleID, DepartmentID, UnitID, Designation, Grade, IsResigned, ResignedOn")] UserViewModel userViewModel)
        public ActionResult _Update(UserViewModel userViewModel)
        {
            UserModel userModel = new UserModel();
            userModel = userViewModel.UserModel;

            // Department and list Models
            List<DepartmentModel> departmentList = new List<DepartmentModel>();
            List<UnitModel> unitList = new List<UnitModel>();
            List<RoleModel> roleList = new List<RoleModel>();
            List<AccessRoleModel> accessRoleList = new List<AccessRoleModel>();

            departmentList = departmentDBService.ListDepartment();
            unitList = unitDBService.ListUnit(userModel.DepartmentID);
            roleList = roleDBService.ListRole();
            accessRoleList = accessRoleDBService.ListAccessRole();

            // LINQ - Filter IsActivated = True
            departmentList = departmentList.Where(d => d.IsActivated.Equals(true) || d.DepartmentID.Equals(userModel.DepartmentID)).OrderBy(d => d.DepartmentName).Select(d => d).ToList();
            unitList = unitList.Where(u => u.IsActivated.Equals(true) || u.UnitID.Equals(userModel.UnitID)).OrderBy(u => u.UnitName).Select(u => u).ToList();
            roleList = roleList.Where(r => r.IsActivated.Equals(true) || r.RoleID.Equals(userModel.RoleID)).OrderBy(r => r.RoleName).Select(r => r).ToList();
            accessRoleList = accessRoleList.Where(r => r.IsActivated.Equals(true) || r.AccessRoleID.Equals(userModel.AccessRoleID)).OrderBy(r => r.AccessRoleName).Select(r => r).ToList();

            userViewModel.SelectListDepartment = PrepareSelectListDepartment(departmentList);
            userViewModel.SelectListUnit = PrepareSelectListUnit(unitList);
            userViewModel.SelectListRole = PrepareSelectListRole(roleList);
            userViewModel.SelectListAccessRole = PrepareSelectListAccessRole(accessRoleList);

            if (ModelState.IsValid.Equals(true))
            {
                if (userDBService.Update(userModel).Equals(false))
                {

                    return PartialView(userViewModel);
                }
                else
                {
                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return PartialView(userViewModel);
            }

        }

        public ActionResult _Delete(string ID)
        {

            UserModel userModel = new UserModel();
            userModel = userDBService.GetDataByID(ID);

            return PartialView(userModel);
        }

        [HttpPost]
        public ActionResult _Delete(UserModel userModel)
        {
            if (!userModel.NRIC.Equals(null))
            {

                if (userDBService.Delete(userModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(userModel);

        }

        public ActionResult _DeletePassword(string ID)
        {

            UserModel userModel = new UserModel();
            userModel = userDBService.GetDataByID(ID);

            return PartialView(userModel);
        }

        [HttpPost]
        public ActionResult _DeletePassword(UserModel userModel)
        {

            if (!userModel.NRIC.Equals(null))
            {

                if (userDBService.DeletePassword(userModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            }

            return PartialView(userModel);

        }

        public ActionResult ChangePassword()
        {
            UserViewModel userViewModel = new UserViewModel();
            UserModel userModel = new UserModel();

            ChangePasswordModel changePasswordModel = new ChangePasswordModel();
            userModel = userDBService.GetDataByID(User.Identity.Name);
            userViewModel.UserModel = userModel;
            userViewModel.ChangePasswordModel = changePasswordModel;
            userViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            ViewBag.UserDetail = string.Format("{0} ( {1} )", userViewModel.UserModel.UserName, userViewModel.UserModel.RoleName);

            return View(userViewModel);
        }

        [HttpPost]
        public ActionResult ChangePassword(UserViewModel userViewModel)
        {

            if (userDBService.ChangePassword(userViewModel).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PersonalProfile()
        {
            UserViewModel userViewModel = new UserViewModel();

            List<DepartmentModel> departmentList = new List<DepartmentModel>();
            List<UnitModel> unitList = new List<UnitModel>();
            List<AccessRoleModel> accessRoleList = new List<AccessRoleModel>();

            UserModel userModel = new UserModel();

            if (userDBService.CheckIsUserExist(User.Identity.Name).Equals(true))
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);

                // Get department list
                departmentList = departmentDBService.ListDepartment();
                unitList = unitDBService.ListUnit(userModel.DepartmentID);

                // LINQ - Filter IsActivated = True
                departmentList = departmentList.Where(d => d.IsActivated.Equals(true) || d.DepartmentID.Equals(userModel.DepartmentID)).OrderBy(d => d.DepartmentName).Select(d => d).ToList();
                unitList = unitList.Where(u => u.IsActivated.Equals(true) || u.UnitID.Equals(userModel.UnitID)).OrderBy(u => u.UnitName).Select(u => u).ToList();

            }
            else
            {

                string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

                switch (device)
                {
                    case "JohnsonControl":

                        userModel = userDBService.GetJohnsonControlUserDataByID(User.Identity.Name);
                        break;

                    case "Suprema":

                        userModel = userDBService.GetSupremaUserDataByID(User.Identity.Name);
                        break;

                    default:

                        userModel = userDBService.GetJohnsonControlUserDataByID(User.Identity.Name);
                        break;
                }

                // Get department list
                departmentList = departmentDBService.ListDepartment();
                departmentList = departmentList.Where(d => d.IsActivated.Equals(true)).OrderBy(d => d.DepartmentName).Select(d => d).ToList();

                // Unit by View Ajax
            }


            // Get Access Role List
            accessRoleList = accessRoleDBService.ListAccessRole();
            accessRoleList = accessRoleList.Where(r => r.IsActivated.Equals(true) || r.AccessRoleID.Equals(userModel.AccessRoleID)).OrderBy(r => r.AccessRoleName).Select(r => r).ToList();

            userViewModel.UserModel = userModel;
            userViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            userViewModel.DepartmentList = departmentList;
            userViewModel.SelectListDepartment = PrepareSelectListDepartment(departmentList);

            userViewModel.UnitList = unitList;
            userViewModel.SelectListUnit = PrepareSelectListUnit(unitList);

            userViewModel.AccessRoleList = accessRoleList;
            userViewModel.SelectListAccessRole = PrepareSelectListAccessRole(accessRoleList);

            ViewBag.UserDetail = string.Format("{0} ( {1} )", userViewModel.UserModel.UserName, userViewModel.UserModel.RoleName);

            return View(userViewModel);
        }

        [HttpPost]
        public ActionResult PersonalProfile(UserViewModel userViewModel)
        {
            List<DepartmentModel> DepartmentList = new List<DepartmentModel>();
            DepartmentList = departmentDBService.ListDepartment();
            userViewModel.SelectListDepartment = PrepareSelectListDepartment(DepartmentList);

            UserModel userModel = new UserModel();
            userModel = userViewModel.UserModel;

            string NRIC = userViewModel.UserModel.NRIC;

            if (userDBService.CheckIsUserExist(userViewModel.UserModel.NRIC).Equals(false))
            {
                if (userDBService.Create(userModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (userDBService.Update(userModel).Equals(false))
                {
                    return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult _ListDeviceUser()
        {
            UserViewModel userViewModel = new UserViewModel();

            // List new device user only           
            List<UserModel> deviceUserList = new List<UserModel>();

            string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

            switch (device)
            {
                case "JohnsonControl":

                    deviceUserList = userDBService.GetJohnsonControlUserList();
                    break;

                case "Suprema":

                    deviceUserList = userDBService.GetSupremaUserList();
                    break;

                default:

                    deviceUserList = userDBService.GetJohnsonControlUserList();
                    break;
            }

            TempData["deviceUserList"] = deviceUserList;
            userViewModel.SelectListDeviceUser = PrepareSelectListDeviceUser(deviceUserList.OrderBy(u => u.NRIC).ToList());

            return View(userViewModel);
        }


        [HttpPost]
        public ActionResult _ListDeviceUser(UserViewModel userViewModel)
        {

            //int selectedDeviceUser = userViewModel.SelectedDeviceUser;

            List<UserModel> deviceUserList = new List<UserModel>();

            string NRIC;
            UserModel deviceUserModel = new UserModel();

            deviceUserList = TempData["deviceUserList"] as List<UserModel>;
            userViewModel.SelectListDeviceUser = PrepareSelectListDeviceUser(deviceUserList.OrderBy(u => u.NRIC).ToList());

            TempData.Keep("deviceUserList");

            if (userViewModel.UserModel.NRIC != null)
            {

                NRIC = userViewModel.UserModel.NRIC.Trim();

                string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

                switch (device)
                {
                    case "JohnsonControl":

                        deviceUserModel = userDBService.GetJohnsonControlUserDataByID(NRIC);
                        break;

                    case "Suprema":

                        deviceUserModel = userDBService.GetSupremaUserDataByID(NRIC);
                        break;

                    default:

                        deviceUserModel = userDBService.GetJohnsonControlUserDataByID(NRIC);
                        break;
                }

                if (deviceUserModel.NRIC != null)
                {
                    return Json(deviceUserModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("NRIC", MyTime.Resource.NRICRequired);
                }

            }
            else
            {
                ModelState.AddModelError("NRIC", MyTime.Resource.NRICRequired);
            }

            return View(userViewModel);
        }



        public ActionResult _SearchDeviceUser()
        {
            UserModel userModel = new UserModel();

            return View(userModel);
        }

        [HttpPost]
        public ActionResult _SearchDeviceUser(UserModel userModel)
        {
            string NRIC = userModel.NRIC;
            UserModel deviceUserModel = new UserModel();

            if (NRIC != null)
            {

                if (userDBService.CheckIsUserExist(NRIC).Equals(true))
                {
                    ModelState.AddModelError("NRIC", MyTime.Resource.UserExistIn + " " + MyTime.Resource.ApplicationName);
                }
                else
                {

                    string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

                    switch (device)
                    {
                        case "JohnsonControl":

                            deviceUserModel = userDBService.GetJohnsonControlUserDataByID(NRIC);
                            break;

                        case "Suprema":

                            deviceUserModel = userDBService.GetSupremaUserDataByID(NRIC);
                            break;

                        default:

                            deviceUserModel = userDBService.GetJohnsonControlUserDataByID(NRIC);
                            break;
                    }

                    if (deviceUserModel.NRIC != null)
                    {
                        return Json(deviceUserModel, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ModelState.AddModelError("NRIC", MyTime.Resource.UserNotExistIn + " " + MyTime.Resource.Device);
                    }

                }

            }
            else
            {
                ModelState.AddModelError("NRIC", MyTime.Resource.NRICRequired);
            }

            return View(userModel);
        }


        public ActionResult _ImportDeviceUser(string NRIC, string userName, string usrID)
        {
            UserViewModel userViewModel = new UserViewModel();

            // User Model
            UserModel userModel = new UserModel();

            userModel.NRIC = NRIC;
            userModel.UserName = userName;
            userModel.USRID = usrID;
            userModel.Gender = "Lelaki";

            userViewModel.UserModel = userModel;

            // Department and list Models
            List<DepartmentModel> departmentList = new List<DepartmentModel>();
            List<UnitModel> unitList = new List<UnitModel>();
            List<RoleModel> roleList = new List<RoleModel>();
            List<AccessRoleModel> accessRoleList = new List<AccessRoleModel>();
            List<ShiftModel> shiftList = new List<ShiftModel>();

            departmentList = departmentDBService.ListDepartment();
            unitList = unitDBService.ListUnit(userModel.DepartmentID);
            roleList = roleDBService.ListRole();
            accessRoleList = accessRoleDBService.ListAccessRole();
            shiftList = shiftDBService.ListShift();

            // LINQ - Filter IsActivated = True
            departmentList = departmentList.Where(d => d.IsActivated.Equals(true) || d.DepartmentID.Equals(userModel.DepartmentID)).OrderBy(d => d.DepartmentName).Select(d => d).ToList();
            unitList = unitList.Where(u => u.IsActivated.Equals(true) || u.UnitID.Equals(userModel.UnitID)).OrderBy(u => u.UnitName).Select(u => u).ToList();
            roleList = roleList.Where(r => r.IsActivated.Equals(true) || r.RoleID.Equals(userModel.RoleID)).OrderBy(r => r.RoleName).Select(r => r).ToList();
            accessRoleList = accessRoleList.Where(r => r.IsActivated.Equals(true) || r.AccessRoleID.Equals(userModel.AccessRoleID)).OrderBy(r => r.AccessRoleName).Select(r => r).ToList();
            shiftList = shiftList.Where(s => s.IsActivated.Equals(true) || s.ShiftID.Equals(userModel.ShiftID)).OrderBy(s => s.ShiftName).Select(s => s).ToList();

            userViewModel.SelectListDepartment = PrepareSelectListDepartment(departmentList);
            userViewModel.SelectListUnit = PrepareSelectListUnit(unitList);
            userViewModel.SelectListRole = PrepareSelectListRole(roleList);
            userViewModel.SelectListAccessRole = PrepareSelectListAccessRole(accessRoleList);
            userViewModel.SelectListShift = PrepareSelectListShift(shiftList);

            return PartialView(userViewModel);
        }

        [HttpPost]
        public ActionResult
            _ImportDeviceUser(UserViewModel userViewModel)
        {
            UserModel userModel = new UserModel();
            userModel = userViewModel.UserModel;

            // Department and list Models
            List<DepartmentModel> departmentList = new List<DepartmentModel>();
            List<UnitModel> unitList = new List<UnitModel>();
            List<RoleModel> roleList = new List<RoleModel>();
            List<AccessRoleModel> accessRoleList = new List<AccessRoleModel>();
            List<ShiftModel> shiftList = new List<ShiftModel>();

            departmentList = departmentDBService.ListDepartment();
            unitList = unitDBService.ListUnit(userModel.DepartmentID);
            roleList = roleDBService.ListRole();
            accessRoleList = accessRoleDBService.ListAccessRole();
            shiftList = shiftDBService.ListShift();

            // LINQ - Filter IsActivated = True
            departmentList = departmentList.Where(d => d.IsActivated.Equals(true) || d.DepartmentID.Equals(userModel.DepartmentID)).OrderBy(d => d.DepartmentName).Select(d => d).ToList();
            unitList = unitList.Where(u => u.IsActivated.Equals(true) || u.UnitID.Equals(userModel.UnitID)).OrderBy(u => u.UnitName).Select(u => u).ToList();
            roleList = roleList.Where(r => r.IsActivated.Equals(true) || r.RoleID.Equals(userModel.RoleID)).OrderBy(r => r.RoleName).Select(r => r).ToList();
            accessRoleList = accessRoleList.Where(r => r.IsActivated.Equals(true) || r.AccessRoleID.Equals(userModel.AccessRoleID)).OrderBy(r => r.AccessRoleName).Select(r => r).ToList();
            shiftList = shiftList.Where(s => s.IsActivated.Equals(true) || s.ShiftID.Equals(userModel.ShiftID)).OrderBy(s => s.ShiftName).Select(s => s).ToList();

            userViewModel.SelectListDepartment = PrepareSelectListDepartment(departmentList);
            userViewModel.SelectListUnit = PrepareSelectListUnit(unitList);
            userViewModel.SelectListRole = PrepareSelectListRole(roleList);
            userViewModel.SelectListAccessRole = PrepareSelectListAccessRole(accessRoleList);
            userViewModel.SelectListShift = PrepareSelectListShift(shiftList);

            if (ModelState.IsValid.Equals(true))
            {
                if (userDBService.Create(userModel).Equals(false))
                {

                    return PartialView(userViewModel);
                }
                else
                {
                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return PartialView(userViewModel);
            }

        }


        private IEnumerable<SelectListItem> PrepareSelectListDepartment(List<DepartmentModel> DepartmentList)
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

        private IEnumerable<SelectListItem> PrepareSelectListUnit(List<UnitModel> UnitList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in UnitList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.UnitID.ToString(),
                    Text = row.UnitName.ToString()
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

        private IEnumerable<SelectListItem> PrepareSelectListDeviceUser(List<UserModel> deviceUserList)
        {
            var selectList = new List<SelectListItem>();

            String text = "";

            foreach (var row in deviceUserList)
            {

                text =  string.Format("{0}{1}{2}", row.NRIC.ToString(), new string(' ', 5), row.UserName.ToString());

                selectList.Add(new SelectListItem
                {
                    Value = row.NRIC.ToString(),
                    Text = text
                });
            }
            return selectList;
        }

        public ActionResult FilterUnit(string departmentID)
        {
            List<UnitModel> unitList = new List<UnitModel>();

            unitList = unitDBService.ListUnit(departmentID);

            // LINQ - Filter IsActivated = True
            unitList = unitList.Where(u => u.IsActivated.Equals(true)).OrderBy(u => u.UnitName).Select(u => u).ToList();

            return Json(unitList, JsonRequestBehavior.AllowGet);
        }

    }

}