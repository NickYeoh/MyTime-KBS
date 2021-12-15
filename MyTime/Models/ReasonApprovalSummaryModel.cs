using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyTime.Models
{
    public class ReasonApprovalSummaryModel
    {        
        public string MonthYear { get; set; }

        public int TotalPending { get; set; }

        public int TotalApproved { get; set; }

        public int TotalRejected { get; set; }

        public int TotalRequestedToAmend { get; set; }

        public DateTime ApprovalDueDate { get; set; }

        //public bool IsApprovalDue { get; set; }


    }
}