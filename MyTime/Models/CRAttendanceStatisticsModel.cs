using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class CRAttendanceStatisticsModel
    {
        public string DepartmentName { get; set; }
        public string UserName { get; set; }
        public string NRIC { get; set; }

        public string AttendanceStatus { get; set; }
        public int TotalCount { get; set; }      

        public string ReportDate { get; set; }
        public string ReportType { get; set; }
        public string ReportDateExtra { get; set; }

        public bool SetPageBreak { get; set; }

    }
}