using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class DepartmentAttendanceDailyModel
    {
        public DateTime AttendanceDate { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int UserCount { get; set; }
        public int InCount { get; set; }
        public int OutCount { get; set; }
        public int AttendCount { get; set; }
        public string AttendPercentage { get; set; }

    }
}