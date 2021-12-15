using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class SystemViewModel
    {
        public SystemModel SystemModel { get; set; }

        public List<RoleModel> RoleList { get; set; }
        public IEnumerable<SelectListItem> SelectListRole { get; set; }

        public List<ShiftModel> ShiftList { get; set; }
        public IEnumerable<SelectListItem> SelectListShift { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }

        public List<AccessRoleModel> AccessRoleList { get; set; }
        public IEnumerable<SelectListItem> SelectListAccessRole { get; set; }

    }
}