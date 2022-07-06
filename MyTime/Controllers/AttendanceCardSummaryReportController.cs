using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace MyTime.Controllers
{
    public class AttendanceCardSummaryReportController : EnvironmentController
    {
        // GET: AttendanceCardSummaryReport
        public ActionResult Index()
        {
            return View();
        }
    }
}