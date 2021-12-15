using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class ReportAdminDepartmentModel
    {
        [Display(Name = "ReportAdminNRIC", ResourceType = typeof(Resource))]
        public string ReportAdminNRIC { get; set; }

        [Display(Name = "DepartmenID", ResourceType = typeof(Resource))]
        public string DepartmentID { get; set; }

        [Display(Name = "DepartmentName", ResourceType = typeof(Resource))]
        public string DepartmentName { get; set; }

    }
}