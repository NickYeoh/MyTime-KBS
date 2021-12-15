using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyTime.ViewModels
{
    public class AttendanceStatisticsViewModel
    {
        public IEnumerable<SelectListItem> SelectListYear { get; set; }
        public IEnumerable<SelectListItem> SelectListMonth { get; set; }
        public IEnumerable<SelectListItem> SelectListWeek { get; set; }

        public IEnumerable<SelectListItem> SelectListDepartment { get; set; }
        public IEnumerable<SelectListItem> SelectListUser { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }

    }
}