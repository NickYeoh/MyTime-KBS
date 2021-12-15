using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class ApproverModel
    {
        [Display(Name = "ApproverNRIC", ResourceType = typeof(Resource))]
        public string ApproverNRIC { get; set; }

        [Display(Name = "ApproverName", ResourceType = typeof(Resource))]
        public string ApproverName { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentID { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

        [Display(Name = "UnitID", ResourceType = typeof(Resource))]
        public int? UnitID { get; set; }

        [Display(Name = "UnitName", ResourceType = typeof(Resource))]
        public string UnitName { get; set; }

        [Display(Name = "RoleID", ResourceType = typeof(Resource))]
        public int RoleID { get; set; }

        [Display(Name = "RoleName", ResourceType = typeof(Resource))]
        public string RoleName { get; set; }

    }
}