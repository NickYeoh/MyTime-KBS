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
    public class AttendanceCardReportController : EnvironmentController
    {
        SystemDBService systemDBService = new SystemDBService();
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();
        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();
        CrystalReportDBService crystalReportDBService = new CrystalReportDBService();
        AttendanceCardDBService attendanceCardDBService = new AttendanceCardDBService();


        // GET: AttendanceCardReport
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            SystemModel systemModel = new SystemModel();
            DateTime attendanceCardStartDate;

            systemModel = systemDBService.GetData();
            attendanceCardStartDate = systemModel.AttendanceCardStartDate;


            AttendanceCardReportViewModel attendanceCardReportViewModel = new AttendanceCardReportViewModel();
            List<ReportAdminDepartmentModel> reportAdminDepartmentList = new List<ReportAdminDepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                attendanceCardReportViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                reportAdminDepartmentList = reportAdminDBService.ListReportAdminDepartment(userModel.NRIC).OrderBy(rad => rad.DepartmentName).ToList();

                attendanceCardReportViewModel.SelectListMonthYear = PrepareSelectMonthYearList(attendanceCardStartDate);
                attendanceCardReportViewModel.SelectListDepartment = PrepareSelectDepartmentList(reportAdminDepartmentList);

            }

            return View(attendanceCardReportViewModel);
        }

        [HttpPost]
        public ActionResult GenerateAttendanceCardList(string selectedMonthYear, string selectedDepartmentID, string selectedAttendanceCardStatus)
        {

            List<AttendanceCardModel> attendanceCardList = new List<AttendanceCardModel>();

            DateTime attendanceMonth = Convert.ToDateTime(selectedMonthYear);

            attendanceCardList = attendanceCardDBService.GetMonthlyAttendanceCardByAttendanceCardStatusAndDepartment(attendanceMonth.ToString("yyyyMM"), selectedDepartmentID, selectedAttendanceCardStatus);
            
            TempData["AttendanceCardList"] = attendanceCardList;

            return Json(attendanceCardList, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SelectListItem> PrepareSelectMonthYearList(DateTime attendanceCardStartDate)
        {
            ISharedFunction sharedFunction;
            sharedFunction = new SharedFunction();

            var selectMonthYearList = new List<SelectListItem>();

            int totalMonthDiff = sharedFunction.CalculateMonthDiff(attendanceCardStartDate, DateTime.Now);


            DateTime monthStart;
            string monthYear;

            for (int month = totalMonthDiff; month >= 0; month--)
            {

                monthStart = new DateTime(attendanceCardStartDate.AddMonths(month).Year, attendanceCardStartDate.AddMonths(month).Month, 1);
                monthYear = attendanceCardStartDate.AddMonths(month).ToString("MMM, yyyy");

                selectMonthYearList.Add(new SelectListItem
                {
                    Value = monthStart.ToString("yyyy-MM-dd"),
                    Text = monthYear
                });
            }


            return selectMonthYearList;
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