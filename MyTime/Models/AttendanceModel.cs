 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class AttendanceModel
    {
        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        // to store Department details    
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "AttendanceDate", ResourceType = typeof(Resource))]
        public DateTime AttendanceDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "AttendanceDay", ResourceType = typeof(Resource))]
        public string AttendanceDay { get; set; }

        [Display(Name = "ShiftID", ResourceType = typeof(Resource))]
        public string ShiftID { get; set; }

        [Display(Name = "AttendanceStatusID", ResourceType = typeof(Resource))]
        public string AttendanceStatusID { get; set; }

        [Display(Name = "AttendanceStatus", ResourceType = typeof(Resource))]
        public string AttendanceStatus { get; set; }

        public bool IsEarlyIn { get; set; }
          
        public int DeviceIn { get; set; }

        [Display(Name = "FirstIn", ResourceType = typeof(Resource))]
        public string FirstIn { get; set; }

        [Display(Name = "Lateness", ResourceType = typeof(Resource))]
        public string Lateness { get; set; }

        public int DeviceOut { get; set; }

        [Display(Name = "LastOut", ResourceType = typeof(Resource))]
        public string LastOut { get; set; }

        [Display(Name = "WorkTime", ResourceType = typeof(Resource))]
        public string WorkTime { get; set; }

        [Display(Name = "Overtime", ResourceType = typeof(Resource))]
        public string Overtime { get; set; }
        public string OvertimeStart { get; set; }
        public string OvertimeEnd { get; set; }

        // From O/T Device
        [Display(Name = "OvertimeExtra", ResourceType = typeof(Resource))]
        public string OvertimeExtra { get; set; }
        public string OvertimeExtraStart { get; set; }
        public string OvertimeExtraEnd { get; set; }

        [Display(Name = "TotalOvertime", ResourceType = typeof(Resource))]
        public string TotalOvertime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "SubmissionDueDate", ResourceType = typeof(Resource))]
        public DateTime SubmissionDueDate { get; set; }

        public bool IsSubmissionDue { get; set; }

        public string ReasonID { get; set; }

        [Display(Name = "Reason", ResourceType = typeof(Resource))]
        public string ReasonName { get; set; }

        public bool IsForOnLeave { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Resource))]
        public string Remark { get; set; }

        public string Proof { get; set; }
              
        public bool IsSubmitted { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool IsRequestedToAmend { get; set; }

        [Display(Name = "ApproverComment", ResourceType = typeof(Resource))]
        public string ApproverComment { get; set; }

        // to store Approver details    
        public string ApproverName { get; set; }

    }
}