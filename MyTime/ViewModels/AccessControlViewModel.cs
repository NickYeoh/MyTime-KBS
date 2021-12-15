using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;

namespace MyTime.ViewModels
{
    public class AccessControlViewModel
    {
        public RoleModel RoleModel { get; set; }
        public List<RoleModel> RoleList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}