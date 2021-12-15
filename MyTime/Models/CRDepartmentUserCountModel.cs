using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTime.Models
{
    public class CRDepartmentUserCountModel
    {
        public string DepartmentName { get; set; }
        public int UserCount { get; set; }

        public bool SetPageBreak { get; set; }
    }
}