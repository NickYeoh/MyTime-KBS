using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;
using System.Web.Mvc;

namespace MyTime.ViewModels
{
    public class ApproverViewModel
    {
        public ApproverModel ApproverModel { get; set; }
        public List<ApproverModel> ApproverList { get; set; }

        public ApproverUserModel ApproverUserModel { get; set; }
        public List<ApproverUserModel> ApproverUserList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}