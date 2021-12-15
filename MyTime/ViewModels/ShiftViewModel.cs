using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyTime.Models;

namespace MyTime.ViewModels
{
    public class ShiftViewModel
    {
        public ShiftModel ShiftModel { get; set; }
        public List<ShiftModel> ShiftList { get; set; }

        public UserAccessControlModel UserAccessControlModel { get; set; }
    }
}