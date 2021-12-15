using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class DepartmentAttendanceDailySummaryModel
    {
        
        public int TotalUserCount { get; set; }
        public int TotalInCount { get; set; }
        public int TotalOutCount { get; set; }
        public int TotalAttendCount { get; set; }
        public string TotalAttendPercentage { get; set; }

    }
}