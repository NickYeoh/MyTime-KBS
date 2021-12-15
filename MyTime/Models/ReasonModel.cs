using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class ReasonModel
    {

        [Display(Name = "ReasonID", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ReasonIDRequired")]
        [MaxLength(20, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string ReasonID { get; set; }

        [Display(Name = "ReasonName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ReasonNameRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string ReasonName { get; set; }

        [Display(Name = "LateIn", ResourceType = typeof(Resource))]
        public bool IsForLateIn { get; set; }

        [Display(Name = "EarlyOut", ResourceType = typeof(Resource))]
        public bool IsForEarlyOut { get; set; }

        [Display(Name = "Incomplete", ResourceType = typeof(Resource))]
        public bool IsForIncomplete { get; set; }

        [Display(Name = "Absent", ResourceType = typeof(Resource))]
        public bool IsForAbsent { get; set; }

        [Display(Name = "OnLeave", ResourceType = typeof(Resource))]
        public bool IsForOnLeave { get; set; }

        [Display(Name = "IsActivated", ResourceType = typeof(Resource))]
        public bool IsActivated { get; set; }
    }
}