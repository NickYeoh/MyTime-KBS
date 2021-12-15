using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class UnitViewModel
    {
        public int UnitID { get; set; }

        // Purposedly set display name to department name
        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DepartmentNameRequiredForList")]
        public string DepartmentID { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName =  "UnitNameRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenExceeded", ErrorMessageResourceType = typeof(Resource))]
        public string UnitName { get; set; }

        [Display(Name = "IsActivated", ResourceType = typeof(Resource))]
        public bool IsActivated { get; set; }

        public List<UnitModel> UnitList { get; set; }

        // For drop-downlist
        public List<DepartmentModel> DepartmentList { get; set; }
        public IEnumerable<SelectListItem> SelectListDepartment { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}