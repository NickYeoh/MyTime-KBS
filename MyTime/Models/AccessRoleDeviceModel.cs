using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class AccessRoleDeviceModel
    {
        public int AccessRoleID { get; set; }

        [Display(Name = "DeviceID", ResourceType = typeof(Resource))]
        public int DeviceID { get; set; }

        [Display(Name = "DeviceName", ResourceType = typeof(Resource))]
        public string DeviceName { get; set; }

        [Display(Name = "IsOvertimeExtraDevice", ResourceType = typeof(Resource))]
        public bool IsOvertimeExtraDevice { get; set; }

    }
}