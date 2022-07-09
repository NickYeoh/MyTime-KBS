using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class AttendanceCardSummaryReportViewModel
    {    
        public UserAccessControlModel UserAccessControlModel { get; set; }

        public IEnumerable<SelectListItem> SelectListYear { get; set; }

        public IEnumerable<SelectListItem> SelectListDepartment { get; set; }
    }
}