using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;
using MyTime.Interfaces;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace MyTime.Controllers
{
    public class AttendanceCardSummaryReportController : EnvironmentController
    {
        SystemDBService systemDBService = new SystemDBService();
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();
        AttendanceCardDBService attendanceCardDBService = new AttendanceCardDBService();

        // GET: AttendanceCardSummaryReport
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            SystemModel systemModel = new SystemModel();
            DateTime attendanceCardStartDate;

            systemModel = systemDBService.GetData();
            attendanceCardStartDate = systemModel.AttendanceCardStartDate;


            AttendanceCardSummaryReportViewModel attendanceCardSummaryReportViewModel = new AttendanceCardSummaryReportViewModel();
            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                attendanceCardSummaryReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                attendanceCardSummaryReportViewModel.SelectListYear = PrepareSelectYearList(attendanceCardStartDate);
                attendanceCardSummaryReportViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

            }

            return View(attendanceCardSummaryReportViewModel);
           
        }


        private IEnumerable<SelectListItem> PrepareSelectYearList(DateTime attendanceCardStartDate)
        {
            var selectYearList = new List<SelectListItem>();
            int currentYear = DateTime.Now.Year;

            if (!currentYear.Equals(attendanceCardStartDate.Year))
            {
                for (int year = attendanceCardStartDate.Year; year <= currentYear; year++)
                {
                    selectYearList.Add(new SelectListItem
                    {
                        Value = string.Format("{0}-{1}-{2}", year.ToString(), "01", "01"),
                        Text = year.ToString()
                    });
                }
            }
            else
            {
                selectYearList.Add(new SelectListItem
                {
                    Value = attendanceCardStartDate.ToString("yyyy-MM-dd"),
                    Text = attendanceCardStartDate.Year.ToString()
                });
            }

            return selectYearList;
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

        [HttpPost]
        public ActionResult GenerateAttendanceCardList(string selectedYear, string selectedDepartmentID, string selectedAttendanceCardStatus)
        {

            List<AttendanceCardSummaryReportModel> attendanceCardReportList = new List<AttendanceCardSummaryReportModel>();

            DateTime attendanceYear = Convert.ToDateTime(selectedYear);

            attendanceCardReportList = attendanceCardDBService.GetYearlyAttendanceCardByDepartment(attendanceYear, selectedDepartmentID);

            TempData["AttendanceCardReportList"] = attendanceCardReportList;

            return Json(attendanceCardReportList, JsonRequestBehavior.AllowGet);
        }

    }
}