using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class ShiftScheduleViewModel
    {
        public ShiftModel ShiftModel { get; set; }
     
        public ShiftScheduleModel ShiftScheduleModel { get; set; }
        public List<ShiftScheduleModel> ShiftScheduleList { get; set; }

        // For drop-downlist
        public List<ShiftModel> ShiftList { get; set; }
        public IEnumerable<SelectListItem> SelectListShift { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}