using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyTime.Models
{
    public class AttendanceSummaryModel
    {
        public int TotalLateIn { get; set; }

        public int TotalEarlyOut { get; set; }

        public int TotalLateInEarlyOut { get; set; }

        public int TotalIncomplete { get; set; }

        public int TotalAbsent { get; set; }

        public int TotalAttend { get; set; }

        public int TotalOnLeave { get; set; }

        public string TotalOvertime { get; set; }

    }
}