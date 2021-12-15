using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;

namespace MyTime.ViewModels
{
    public class HolidayViewModel
    {
        public HolidayModel HolidayModel { get; set; }
        public List<HolidayModel> HolidayList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}