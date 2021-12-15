using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class AccessRoleViewModel
    {

        public AccessRoleModel AccessRoleModel { get; set; }
        public string[] DeviceID { get; set; }
        
        public List<AccessRoleModel> AccessRoleList { get; set; }

        public List<DeviceModel> DeviceList { get; set; }
     
        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}