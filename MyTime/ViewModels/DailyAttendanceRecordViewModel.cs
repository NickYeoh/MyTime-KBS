using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyTime.ViewModels
{
    public class DailyAttendanceRecordViewModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime AttendanceDate { get; set; }

        public IEnumerable<SelectListItem> SelectListDepartment { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }

    }
}