using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class CRAttendanceMonthlyModel
    {
        // For Crystal Report : Attendance Monthly    
                
        public string NRIC { get; set; }
        public string UserName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string AttendanceDay { get; set; }
        public string CardColorCode { get; set; }       
        public string AttendanceStatusID { get; set; }
        public string AttendanceStatus { get; set; }

        public bool IsEarlyIn { get; set; }
        public string FirstIn { get; set; }
        public string Lateness { get; set; }
        public string LastOut { get; set; }
        public string WorkTime { get; set; }

        // Overtime
        public string Overtime { get; set; }
        public string OvertimeStart { get; set; }
        public string OvertimeEnd { get; set; }

        // Overtime Extra From O/T Device
        public string OvertimeExtra { get; set; }
        public string OvertimeExtraStart { get; set; }
        public string OvertimeExtraEnd { get; set; }

        public string TotalOvertime { get; set; }

        public DateTime SubmissionDueDate { get; set; }
        public bool IsSubmissionDue { get; set; }

        public string ReasonID { get; set; }
        public string ReasonName { get; set; }
        public string Remark { get; set; }
        public string Result { get; set; }
      
        public bool IsForOnLeave { get; set; }

        public bool IsSubmitted { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool IsRequestedToAmend { get; set; }

        public int TotalLateIn { get; set; }
        public int TotalEarlyOut { get; set; }
        public int TotalLateInEarlyOut { get; set; }
        public int TotalIncomplete { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalAttend { get; set; }
        public int TotalOnLeave { get; set; }

        public string ReportDate { get; set; }
        public string ReportType { get; set; }
        public string ReportDateExtra { get; set; }

        // to store Approver details    
        public string ApproverName { get; set; }

        public bool SetPageBreak { get; set; }

    }
}