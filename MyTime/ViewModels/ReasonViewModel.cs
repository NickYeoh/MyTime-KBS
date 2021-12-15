using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;

namespace MyTime.ViewModels
{
    public class ReasonViewModel
    {
        public ReasonModel ReasonModel { get; set; }
        public List<ReasonModel> ReasonList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}