using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class AttendanceViewModel
    {   

        public IEnumerable<SelectListItem> SelectListMonthYear { get; set; }

        // To store the user approver detail
        public List<ApproverUserModel> UserApproverList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}