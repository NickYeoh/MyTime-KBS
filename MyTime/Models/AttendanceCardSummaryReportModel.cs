using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class AttendanceCardSummaryReportModel
    {

        [Display(Name = "AttendanceYear", ResourceType = typeof(Resource))]
        public string AttendanceYear { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

    }
}