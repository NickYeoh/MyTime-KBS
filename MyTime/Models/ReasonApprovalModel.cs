using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class ReasonApprovalModel
    {
        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "AttendanceDate", ResourceType = typeof(Resource))]
        public DateTime AttendanceDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "AttendanceDay", ResourceType = typeof(Resource))]
        public string AttendanceDay { get; set; }

        [Display(Name = "AttendanceStatusID", ResourceType = typeof(Resource))]
        public string AttendanceStatusID { get; set; }

        [Display(Name = "AttendanceStatus", ResourceType = typeof(Resource))]
        public string AttendanceStatus { get; set; }

        [Display(Name = "FirstIn", ResourceType = typeof(Resource))]
        public string FirstIn { get; set; }

        [Display(Name = "Lateness", ResourceType = typeof(Resource))]
        public string Lateness { get; set; }

        [Display(Name = "LastOut", ResourceType = typeof(Resource))]
        public string LastOut { get; set; }

        [Display(Name = "WorkTime", ResourceType = typeof(Resource))]
        public string WorkTime { get; set; }

        //[Display(Name = "Overtime", ResourceType = typeof(Resource))]
        //public string Overtime { get; set; }

        //[Display(Name = "Overtime", ResourceType = typeof(Resource))]
        //public string Overtime { get; set; }
        //public string OvertimeStart { get; set; }
        //public string OvertimeEnd { get; set; }

        //// From O/T Device
        //[Display(Name = "OvertimeExtra", ResourceType = typeof(Resource))]
        //public string OvertimeExtra { get; set; }
        //public string OvertimeExtraStart { get; set; }
        //public string OvertimeExtraEnd { get; set; }

        //[Display(Name = "TotalOvertime", ResourceType = typeof(Resource))]
        //public string TotalOvertime { get; set; }

        [Display(Name = "OvertimeStart", ResourceType = typeof(Resource))]
        public string OvertimeStart { get; set; }

        [Display(Name = "OvertimeEnd", ResourceType = typeof(Resource))]
        public string OvertimeEnd { get; set; }

        [Display(Name = "Overtime", ResourceType = typeof(Resource))]
        public string Overtime { get; set; }

        // From O/T Device
        [Display(Name = "OvertimeExtraStart", ResourceType = typeof(Resource))]
        public string OvertimeExtraStart { get; set; }

        [Display(Name = "OvertimeExtraEnd", ResourceType = typeof(Resource))]
        public string OvertimeExtraEnd { get; set; }

        [Display(Name = "OvertimeExtra", ResourceType = typeof(Resource))]
        public string OvertimeExtra { get; set; }

        [Display(Name = "TotalOvertime", ResourceType = typeof(Resource))]
        public string TotalOvertime { get; set; }


        [Display(Name = "Reason", ResourceType = typeof(Resource))]
        public string ReasonID { get; set; }

        [Display(Name = "Reason", ResourceType = typeof(Resource))]
        public string ReasonName { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Resource))]
        public string Remark { get; set; }

        public string Proof { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ReceivedOn { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public bool IsRequestedToAmend { get; set; }

        [Display(Name = "ApproverComment", ResourceType = typeof(Resource))]
        public string ApproverComment { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ApprovalDueDate { get; set; }

        public bool IsApprovalDue { get; set; }

    }
}