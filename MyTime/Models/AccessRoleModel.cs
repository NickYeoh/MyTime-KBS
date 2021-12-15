using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace MyTime.Models
{
    public class AccessRoleModel
    {
        public int AccessRoleID { get; set; }

        [Display(Name = "AccessRoleName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "AccessRoleNameRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string AccessRoleName { get; set; }

        [Display(Name = "IsActivated", ResourceType = typeof(Resource))]
        public bool IsActivated { get; set; }

    }
}