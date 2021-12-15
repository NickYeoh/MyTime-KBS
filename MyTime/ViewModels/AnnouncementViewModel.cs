using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;


namespace MyTime.ViewModels
{
    public class AnnouncementViewModel
    {
        public AnnouncementModel AnnouncementModel { get; set; }
        public List<AnnouncementModel> AnnouncementList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }

    }
}