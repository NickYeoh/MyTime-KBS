using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class UserAccessControlViewModel
    {
        public RoleModel RoleModel { get; set; }
        public List<RoleModel> RoleList { get; set; }
        public IEnumerable<SelectListItem> SelectListRole { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
        public List<UserAccessControlModel> UserAccessControlList { get; set; }
    }
}