using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class DepartmentUserCountModel
    {
      
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int UserCount { get; set; }
    }
}