using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace MyTime.Models
{
    public class RoleModel
    {
        public int RoleID { get; set; }

        [Display(Name = "RoleName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RoleNameRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string RoleName { get; set; }

        [Display(Name = "IsActivated", ResourceType = typeof(Resource))]
        public bool IsActivated { get; set; }

    }
}