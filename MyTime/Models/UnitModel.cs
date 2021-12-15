using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class UnitModel
    {    

        [Display(Name = "DepartmentID", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DepartmentIDRequired")]
        public string DepartmentID { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DepartmentNameRequired")]
        public string DepartmentName { get; set; }

        public int UnitID { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "UnitNameRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string UnitName { get; set; }

        [Display(Name = "IsActivated", ResourceType = typeof(Resource))]
        public bool IsActivated { get; set; }

    }
}