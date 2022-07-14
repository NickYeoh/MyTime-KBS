using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class AttendanceCardStatusYearlyReportModel
    {

        [Display(Name = "AttendanceYear", ResourceType = typeof(Resource))]
        public string AttendanceYear { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

        [Display(Name = "Month.Jan", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus01 { get; set; }
        public int TotalAttendanceIssue01 { get; set; }

        [Display(Name = "Month.Feb", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus02 { get; set; }
        public int TotalAttendanceIssue02 { get; set; }

        [Display(Name = "Month.Mar", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus03 { get; set; }
        public int TotalAttendanceIssue03 { get; set; }

        [Display(Name = "Month.Apr", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus04 { get; set; }
        public int TotalAttendanceIssue04 { get; set; }

        [Display(Name = "Month.May", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus05 { get; set; }
        public int TotalAttendanceIssue05 { get; set; }

        [Display(Name = "Month.Jun", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus06 { get; set; }
        public int TotalAttendanceIssue06 { get; set; }

        [Display(Name = "Month.Jul", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus07 { get; set; }
        public int TotalAttendanceIssue07 { get; set; }

        [Display(Name = "Month.Aug", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus08 { get; set; }
        public int TotalAttendanceIssue08 { get; set; }

        [Display(Name = "Month.Sep", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus09 { get; set; }
        public int TotalAttendanceIssue09 { get; set; }

        [Display(Name = "Month.Oct", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus10 { get; set; }
        public int TotalAttendanceIssue10 { get; set; }

        [Display(Name = "Month.Nov", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus11 { get; set; }
        public int TotalAttendanceIssue11 { get; set; }

        [Display(Name = "Month.Dec", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus12 { get; set; }
        public int TotalAttendanceIssue12 { get; set; }


    }
}