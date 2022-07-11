
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MyTime.Services;

namespace MyTime.Models
{    
    public class UserAccessControlModel
    {
        [Display(Name = "RoleName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RoleNameRequired")]
        public int RoleID { get; set; }

        [Display(Name = "Dashboard", ResourceType = typeof(Resource))]
        public bool IsAllowedDashboard { get; set; }

        [Display(Name = "Attendance", ResourceType = typeof(Resource))]
        public bool IsAllowedAttendance { get; set; }

        [Display(Name = "Announcement", ResourceType = typeof(Resource))]
        public bool IsAllowedAnnouncement { get; set; }

        [Display(Name = "SystemSetting", ResourceType = typeof(Resource))]
        public bool IsAllowedSystemSetting { get; set; }

        [Display(Name = "Organisation", ResourceType = typeof(Resource))]
        public bool IsAllowedOrganisation { get; set; }

        [Display(Name = "User", ResourceType = typeof(Resource))]
        public bool IsAllowedUser { get; set; }

        [Display(Name = "ShiftSchedule", ResourceType = typeof(Resource))]
        public bool IsAllowedShiftSchedule { get; set; }

        [Display(Name = "Device", ResourceType = typeof(Resource))]
        public bool IsAllowedDevice { get; set; }

        [Display(Name = "ApproveReason", ResourceType = typeof(Resource))]
        public bool IsAllowedApproveReason { get; set; }

        [Display(Name = "PrintReport", ResourceType = typeof(Resource))]
        public bool IsAllowedPrintReport { get; set; }

        [Display(Name = "ContactUs", ResourceType = typeof(Resource))]
        public bool IsAllowedContactUs { get; set; }

        //2022-07-11
        public bool IsAllowedChangePassword { get; set; }
    }

}