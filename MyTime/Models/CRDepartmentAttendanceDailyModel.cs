using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class CRDepartmentAttendanceDailyModel
    {   
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int UserCount { get; set; }
        public int InCount { get; set; }
        public int OutCount { get; set; }
        public int AttendCount { get; set; }
    
        public string ReportDate { get; set; }
        public string ReportType { get; set; }
        public string ReportDateExtra { get; set; }

        public bool SetPageBreak { get; set; }


    }
}