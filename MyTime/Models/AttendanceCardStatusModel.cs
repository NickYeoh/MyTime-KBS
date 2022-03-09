using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class AttendanceCardStatusModel
    {

        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

        [Display(Name = "AttendanceMonth", ResourceType = typeof(Resource))]
        public string AttendanceMonth { get; set; }

        [Display(Name = "AttendanceCardStatus", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus { get; set; }

    }
}