using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class AttendanceCardStatusMontlyReportModel
    {
        [Display(Name = "AttendanceMonth", ResourceType = typeof(Resource))]
        public string AttendanceMonth { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

        [Display(Name = "LateInCount", ResourceType = typeof(Resource))]
        public int LateInCount{ get; set; }

        [Display(Name = "EarlyOutCount", ResourceType = typeof(Resource))]
        public int EarlyOutCount { get; set; }

        [Display(Name = "LateInEarlyOutCount", ResourceType = typeof(Resource))]
        public int LateInEarlyOutCount { get; set; }

        [Display(Name = "IncompleteCount", ResourceType = typeof(Resource))]
        public int IncompleteCount { get; set; }

        [Display(Name = "AbsentCount", ResourceType = typeof(Resource))]
        public int AbsentCount { get; set; }

        [Display(Name = "AttendCount", ResourceType = typeof(Resource))]
        public int AttendCount { get; set; }

        [Display(Name = "OnLeaveCount", ResourceType = typeof(Resource))]
        public int OnLeaveCount { get; set; }

        [Display(Name = "TotalAttendanceIssue", ResourceType = typeof(Resource))]
        public int TotalAttendanceIssue { get; set; }

        [Display(Name = "AttendanceCardStatus", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus { get; set; }

    }
}