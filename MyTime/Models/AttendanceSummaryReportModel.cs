using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class AttendanceSummaryReportModel
    {

        public DateTime ReportDate { get; set; }

        public string DepartmentName { get; set; }

        public int UserCount { get; set; }

        public int TotalLateIn { get; set; }

        public int TotalEarlyOut { get; set; }

        public int TotalLateInEarlyOut { get; set; }

        public int TotalIncomplete { get; set; }

        public int TotalAbsent { get; set; }

        public int TotalAttend { get; set; }

        public int TotalOnLeave { get; set; }

    }
}