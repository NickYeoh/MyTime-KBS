using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class ApproverUserModel
    {
        [Display(Name = "ApproverNRIC", ResourceType = typeof(Resource))]
        public string ApproverNRIC { get; set; }

        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(Resource))]
        public string UserName { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(Resource))]
        public string UnitName { get; set; }
  
    }
}