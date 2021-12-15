using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace MyTime.Models
{
    public class AnnouncementModel
    {
        public int AnnouncementID { get; set; }

        [Display(Name = "AnnouncementMessage", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "AnnouncementMessageRequired")]
        [MaxLength(300, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string AnnouncementMessage { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "AnnouncedOn", ResourceType = typeof(Resource))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime AnnouncedOn { get; set; }

    }
}