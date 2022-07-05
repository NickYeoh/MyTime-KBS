using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MyTime.Models;
using System.Web.Mvc;
using System.ComponentModel;

namespace MyTime.ViewModels
{
    public class UserViewModel
    {
        public UserModel UserModel { get; set; }
        public List<UserModel> UserList { get; set; }

        // For drop-downlist
        public List<DepartmentModel> DepartmentList { get; set; }
        public IEnumerable<SelectListItem> SelectListDepartment { get; set; }

        public List<UnitModel> UnitList { get; set; }
        public IEnumerable<SelectListItem> SelectListUnit { get; set; }

        public List<RoleModel> RoleList { get; set; }
        public IEnumerable<SelectListItem> SelectListRole { get; set; }

        public List<AccessRoleModel> AccessRoleList { get; set; }
        public IEnumerable<SelectListItem> SelectListAccessRole { get; set; }

        public List<ShiftModel> ShiftList { get; set; }
        public IEnumerable<SelectListItem> SelectListShift { get; set; }

        // For Import Device User
        public IEnumerable<SelectListItem> SelectListDeviceUser { get; set; }
     
        public ChangePasswordModel ChangePasswordModel { get; set; }
        public UserAccessControlModel UserAccessControlModel { get; set; }

        // For Attendance Card Status
        public IEnumerable<SelectListItem> SelectListMonthYear { get; set; }
      
        public List<AttendanceCardReportModel> AttendanceCardList { get; set; }
    }
}