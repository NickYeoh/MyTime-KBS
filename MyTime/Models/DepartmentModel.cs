using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class DepartmentModel
    {
        [Display(Name = "DepartmentID", ResourceType = typeof(Resource))]   
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DepartmentIDRequired")]
        [MaxLength(20, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]       
        public string DepartmentID { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DepartmentNameRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

        //[Display(Name = "LateIn", ResourceType = typeof(Resource))]
        //public bool IsForLateIn { get; set; }

        //[Display(Name = "EarlyOut", ResourceType = typeof(Resource))]
        //public bool IsForEarlyOut { get; set; }

        [Display(Name = "IsActivated", ResourceType = typeof(Resource))]
        public bool IsActivated { get; set; }

    }

    //[Remote("CheckDuplicate", "Department", ErrorMessage = "Account already exists!")]
}