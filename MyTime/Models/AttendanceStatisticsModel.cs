using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class AttendanceStatisticsModel
    {
        public int No { get; set; }

        public DateTime ReportDate { get; set; }

        public string DepartmentName { get; set; }

        public string UserName { get; set; }

        public string NRIC { get; set; }

        public string AttendanceStatus { get; set; }

        public int TotalCount { get; set; }


    }
}