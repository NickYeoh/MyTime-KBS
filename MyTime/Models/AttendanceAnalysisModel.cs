using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class AttendanceAnalysisModel
    {

        public DateTime ReportDate { get; set; }

        public string DepartmentName { get; set; }

        public string UserName { get; set; }

        public string NRIC { get; set; }

        public int TotalLateIn { get; set; }

        public int TotalEarlyOut { get; set; }

        public int TotalLateInEarlyOut { get; set; }

        public int TotalIncomplete { get; set; }

        public int TotalAbsent { get; set; }

        public int TotalAttend { get; set; }

        public int TotalOnLeave { get; set; }

    }
}