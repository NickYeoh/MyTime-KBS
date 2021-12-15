using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;

namespace MyTime.ViewModels
{
    public class DepartmentViewModel
    {
        public DepartmentModel DepartmentModel { get; set; }
        public List<DepartmentModel> DepartmentList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}