using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class CRAttendanceSummaryReportModel
    {

        public string DepartmentName { get; set; }
        public int UserCount { get; set; }
        public int TotalLateIn { get; set; }
        public int TotalEarlyOut { get; set; }
        public int TotalLateInEarlyOut { get; set; }
        public int TotalIncomplete { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalAttend { get; set; }
        public int TotalOnLeave { get; set; }

        public bool IsForAllDepartment { get; set; }

        public string ReportDate { get; set; }
        public string ReportType { get; set; }
        public string ReportDateExtra { get; set; }

        public bool SetPageBreak { get; set; }

    }
}