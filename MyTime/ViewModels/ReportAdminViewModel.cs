using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class ReportAdminViewModel
    {
        public ReportAdminModel ReportAdminModel { get; set; }
        public List<ReportAdminModel> ReportAdminList { get; set; }

        public ReportAdminDepartmentModel ReportAdminDepartmentModel { get; set; }
        public List<ReportAdminDepartmentModel> ReportAdminDepartmentList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }

    }
}