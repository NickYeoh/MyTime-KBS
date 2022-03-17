using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class AttendanceCardModel
    {

        [Display(Name = "NRIC", ResourceType = typeof(Resource))]
        public string NRIC { get; set; }

    
        [Display(Name = "YearMonth", ResourceType = typeof(Resource))]
        public string YearMonth { get; set; }

        [Display(Name = "AttendanceCardStatus", ResourceType = typeof(Resource))]
        public string AttendanceCardStatus { get; set; }

    }
}