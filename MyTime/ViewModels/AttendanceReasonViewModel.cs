using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class AttendanceReasonViewModel
    {
        public AttendanceReasonModel AttendanceReasonModel { get; set; }
        public IEnumerable<SelectListItem> SelectReason { get; set; }

    }
}