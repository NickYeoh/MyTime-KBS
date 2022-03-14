﻿using System;
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
    public class DailyAttendanceRecordController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
    
        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        AttendanceDBService attendanceDBService = new AttendanceDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();

        // GET: Report
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        
            DailyAttendanceRecordViewModel dailyAttendanceRecordViewModel = new DailyAttendanceRecordViewModel();
            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                dailyAttendanceRecordViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                dailyAttendanceRecordViewModel.AttendanceDate = DateTime.Now;
                dailyAttendanceRecordViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

            }

            return View(dailyAttendanceRecordViewModel);
        }

        [HttpGet]
        public ActionResult GenerateAttendanceList(string selectedAttendanceDate, string selectedDepartmentID)
        {
            //UserModel userModel = new UserModel();
            List<UserModel> userList = new List<UserModel>();
            List<AttendanceModel> tempList = new List<AttendanceModel>();
            List<AttendanceModel> attendanceList = new List<AttendanceModel>();
                      
            string tempDate;
            DateTime attendanceDate;

            // To get 1 month attendance
            DateTime startOn;
            DateTime endOn;

            tempDate = selectedAttendanceDate.Substring(6, 4) + "-" + selectedAttendanceDate.Substring(3, 2) + "-" + selectedAttendanceDate.Substring(0, 2);
            DateTime.TryParse(tempDate, out attendanceDate);

            tempDate = selectedAttendanceDate.Substring(6, 4) + "-" + selectedAttendanceDate.Substring(3, 2) + "-" + "01";
            DateTime.TryParse(tempDate, out startOn);

            int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            endOn = startOn.AddDays((days - 1));

            // include those resigned after attendance date
            userList = userDBService.ListUser().Where(u => u.DepartmentID == selectedDepartmentID && ((u.IsResigned == true && u.ResignedOn > attendanceDate) || u.IsResigned== false) && u.IsAttendanceExcluded== false).OrderBy(u=>u.UserName).ToList();

            if (!userList.Equals(0))
            {               
                foreach ( UserModel userModel in userList)
                {
                    // Get 1 Month Then Filter
                    tempList = attendanceDBService.GetMonthlyAttendance(userModel.NRIC, userModel.USRID, userModel.UserName, userModel.DepartmentID, userModel.DepartmentName, startOn, endOn, userModel.AccessRoleID, "");

                    // Filter and combine the data into Attendance List
                    attendanceList.AddRange(tempList.Where(t=>t.AttendanceDate == attendanceDate.Date).ToList());
                }

            }

            TempData["AttendanceList"] = attendanceList;

            return Json(attendanceList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAttendanceSummary()
        {
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            List<AttendanceModel> attendanceList = new List<AttendanceModel>();
            attendanceList = TempData["AttendanceList"] as List<AttendanceModel>;

            TempData.Keep("AttendanceList");

            int totalLateIn = 0;
            int totalEarlyOut = 0;
            int totalLateInEarlyOut = 0;
            int totalIncomplete = 0;
            int totalAbsent = 0;
            int totalAttend = 0;
            int totalOnLeave = 0;
            string totalOvertime = string.Format("0{0} 0{1}", MyTime.Resource.Hour, MyTime.Resource.Minute);

            totalLateIn = attendanceList.Where(a => a.AttendanceStatusID == "LIN" && a.IsApproved == false).Count();
            totalEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "EOT" && a.IsApproved == false).Count();
            totalLateInEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "L/E" && a.IsApproved == false).Count();
            totalIncomplete = attendanceList.Where(a => a.AttendanceStatusID == "ICP" && a.IsApproved == false).Count();
            totalAbsent = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == false).Count();
            totalAttend = attendanceList.Where(a => a.AttendanceStatusID == "NOR" || (a.IsApproved == true && a.IsForOnLeave == false)).Count();
            totalOnLeave = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == true && a.IsForOnLeave == true).Count();
            totalOvertime = attendanceList.Select(a => a.TotalOvertime).Last();

            attendanceSummaryModel.TotalLateIn = totalLateIn;
            attendanceSummaryModel.TotalEarlyOut = totalEarlyOut;
            attendanceSummaryModel.TotalLateInEarlyOut = totalLateInEarlyOut;
            attendanceSummaryModel.TotalIncomplete = totalIncomplete;
            attendanceSummaryModel.TotalAbsent = totalAbsent;
            attendanceSummaryModel.TotalAttend = totalAttend;
            attendanceSummaryModel.TotalOnLeave = totalOnLeave;
            attendanceSummaryModel.TotalOvertime = totalOvertime;

            TempData["AttendanceList"] = attendanceList;
            TempData["AttendanceSummary"] = attendanceSummaryModel;

            return Json(attendanceSummaryModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintDailyAttendanceRecordReport()
        {

            List<AttendanceModel> attendanceList = new List<AttendanceModel>();
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            List<CRAttendanceDailyModel> crAttendanceDailyList = new List<CRAttendanceDailyModel>();
            ApproverUserModel userApproverModel = new ApproverUserModel();

          
            attendanceList = TempData["AttendanceList"] as List<AttendanceModel>;
            attendanceSummaryModel = TempData["AttendanceSummary"] as AttendanceSummaryModel;
                    
            TempData.Keep("AttendanceList");
            TempData.Keep("AttendanceSummary");

            crAttendanceDailyList = crystalReportDBService.PrepareAttendanceReport("Daily", attendanceList, attendanceSummaryModel);

            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports"), "AttendanceRecordCR.rpt"));
            report.SetDataSource(crAttendanceDailyList);

            //report.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
            //report.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            //report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            string organisationName = Session["OrganisationName"].ToString();
            string organisationLogo = Session["OrganisationLogo"].ToString();
            string organisationLogoPath = Path.Combine(Server.MapPath("~/Images"), organisationLogo);

            report.SetParameterValue("Language", System.Globalization.CultureInfo.CurrentCulture.Name.ToString());
            report.SetParameterValue("OrganisationName", organisationName);
            report.SetParameterValue("OrganisationLogo", organisationLogoPath);
          
            Response.Buffer = false;
            Response.ClearContent();

            Response.ClearHeaders();

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Laporan Kedatangan Harian.pdf");

        }


        private IEnumerable<SelectListItem> PrepareSelectDepartmentList(List<ReportAdminDepartmentModel> reportAdminDepartmentList)
        {
            var selectList = new List<SelectListItem>();

            foreach (var row in reportAdminDepartmentList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = row.DepartmentID.ToString(),
                    Text = row.DepartmentName.ToString()
                });
            }
            return selectList;
        }

    }
}