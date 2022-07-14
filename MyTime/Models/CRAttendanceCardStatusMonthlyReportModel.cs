using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class CRAttendanceCardStatusMonthlyReportModel
    {
        public string AttendanceMonth { get; set; }
        public string NRIC { get; set; }
        public string UserName { get; set; }
        public string DepartmentName { get; set; }
        public string AttendanceCardStatus { get; set; }
        public int LateInCount { get; set; }      
        public int EarlyOutCount { get; set; }
        public int LateInEarlyOutCount { get; set; }       
        public int IncompleteCount { get; set; }    
        public int AbsentCount { get; set; }        
        public int AttendCount { get; set; }    
        public int OnLeaveCount { get; set; }
        public int TotalAttendanceIssue { get; set; }

        public string ReportDate { get; set; }
        public string ReportType { get; set; }
        public string ReportDateExtra { get; set; }

        public bool SetPageBreak { get; set; }
    }
}