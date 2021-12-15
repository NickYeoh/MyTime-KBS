using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class ShiftScheduleModel
    {
        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentID { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

        [Display(Name = "UnitID", ResourceType = typeof(Resource))]
        public int? UnitID { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(Resource))]
        public string UnitName { get; set; }

        [Display(Name = "ShiftID", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ShiftNameRequired")]
        public string ShiftID { get; set; }

        [Display(Name = "ShiftName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ShiftNameRequired")]
        public string ShiftName { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "EffectiveOn", ResourceType = typeof(Resource))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EffectiveOn { get; set; }



    }
}