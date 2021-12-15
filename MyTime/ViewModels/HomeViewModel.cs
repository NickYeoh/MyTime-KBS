using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyTime.Models;
using MyTime.Services;

namespace MyTime.ViewModels
{

    public class HomeViewModel
    {
        public UserModel User { get; set; }
        public AttendanceSummaryModel AttendanceSummary { get; set; }
        public List<ReasonApprovalSummaryModel> ReasonApprovalSummaryList { get; set; }
        public List<AnnouncementModel> AnnouncementList { get; set; }
        public UserAccessControlModel UserAccessControlModel { get; set; }
       
    }
}