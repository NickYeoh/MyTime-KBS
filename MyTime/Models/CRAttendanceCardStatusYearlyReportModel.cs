using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class CRAttendanceCardStatusYearlyReportModel
    {

        public string AttendanceYear { get; set; }
        public string NRIC { get; set; }
        public string UserName { get; set; }
        public string DepartmentName { get; set; }
        public string AttendanceCardStatus01 { get; set; }
        public int TotalAttendanceIssue01 { get; set; }
        public string AttendanceCardStatus02 { get; set; }
        public int TotalAttendanceIssue02 { get; set; }
        public string AttendanceCardStatus03 { get; set; }
        public int TotalAttendanceIssue03 { get; set; }
        public string AttendanceCardStatus04 { get; set; }
        public int TotalAttendanceIssue04 { get; set; }
        public string AttendanceCardStatus05 { get; set; }
        public int TotalAttendanceIssue05 { get; set; }
        public string AttendanceCardStatus06 { get; set; }
        public int TotalAttendanceIssue06 { get; set; }
        public string AttendanceCardStatus07 { get; set; }
        public int TotalAttendanceIssue07 { get; set; }
        public string AttendanceCardStatus08 { get; set; }
        public int TotalAttendanceIssue08 { get; set; }
        public string AttendanceCardStatus09 { get; set; }
        public int TotalAttendanceIssue09 { get; set; }
        public string AttendanceCardStatus10 { get; set; }
        public int TotalAttendanceIssue10 { get; set; }
        public string AttendanceCardStatus11 { get; set; }
        public int TotalAttendanceIssue11 { get; set; }        
        public string AttendanceCardStatus12 { get; set; }
        public int TotalAttendanceIssue12 { get; set; }

        public string ReportDate { get; set; }
        public string ReportType { get; set; }
        public string ReportDateExtra { get; set; }

        public bool SetPageBreak { get; set; }

    }
}