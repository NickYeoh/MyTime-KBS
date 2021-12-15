using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class DepartmentAttendanceHistoryReportViewModel
    {

        public IEnumerable<SelectListItem> SelectListMonthYear { get; set; }

        public IEnumerable<SelectListItem> SelectListDepartment { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }

    }
}