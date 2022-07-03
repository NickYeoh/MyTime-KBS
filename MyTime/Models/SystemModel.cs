using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class SystemModel
    {
        [Display(Name = "OrganisationName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "OrganisationNameRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string OrganisationName { get; set; }

        [Display(Name = "OrganisationShortName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "OrganisationNameRequired")]
        [MaxLength(10, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string OrganisationShortName { get; set; }

        [Display(Name = "OrganisationLogo", ResourceType = typeof(Resource))]
        public string OrganisationLogo { get; set; }

        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessageResourceName = "InvalidImageFile", ErrorMessageResourceType = typeof(Resource))]
        public HttpPostedFileBase PostedOrganisationLogo { get; set; }

        [Display(Name = "DefaultRoleName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DefaultRoleNameRequired")]
        public int DefaultRoleID { get; set; }
        public string DefaultRoleName { get; set; }

        [Display(Name = "DefaultShiftName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DefaultShiftNameRequired")]
        public string DefaultShiftID { get; set; }
        public string DefaultShiftName { get; set; }

        [Display(Name = "ReasonSubmissionPeriod", ResourceType = typeof(Resource))]
        public int ReasonSubmissionPeriod { get; set; }

        [Display(Name = "DefaultAccessRoleName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DefaultAccessRoleNameRequired")]
        public int DefaultAccessRoleID { get; set; }
        public string DefaultAccessRoleName { get; set; }

        // Send notification to approver after the user submitted reason
        public bool IsEmailNotificationEnabled { get; set; }

        // Send reminder to user on 1st, 5th and 10th if reason not submitted
        public bool IsEmailReminderEnabled { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DataStartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime AttendanceCardStartDate { get; set; }

    }
}