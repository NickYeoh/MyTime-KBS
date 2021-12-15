using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class DeviceModel
    {
        public int DeviceID { get; set; }

        [Display(Name = "DeviceName", ResourceType = typeof(Resource))]
        public string DeviceName { get; set; }
    }
}