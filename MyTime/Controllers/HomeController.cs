using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyTime.Services;
using MyTime.ViewModels;
using MyTime.Models;
using System.Management;

namespace MyTime.Controllers
{
    public class HomeController : EnvironmentController
    {

        // version : 1.0.5 : Enable the email notification function based on IsEmailNotificationEnabled column value in Setting Table
        // to solve the email server offline caused user unable to submit reason

        // version : 1.0.6 (20220309) :
        // 1. Rectified the UserAccessControl jquery. The checkbox value for User is not same as data.
        // 2. Recfified the datetime conversion issue for Submit Reason / Approve Reason / Reject Reason / Request Amend Reason / Log Avtivity functions 

        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        AttendanceDBService attendanceDBService = new AttendanceDBService();

        SystemDBService systemDBService = new SystemDBService();
        ReasonApprovalDBService reasonApprovalDBService = new ReasonApprovalDBService();

        AnnouncementDBService announcementDBService = new AnnouncementDBService();

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            HomeViewModel homeViewModel = new HomeViewModel();

            UserAccessControlModel userAccessControlModel = new UserAccessControlModel();

            List<AttendanceModel> attendanceList = new List<AttendanceModel>();
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();
            List<DepartmentModel> departmentList = new List<DepartmentModel>();

            string departmentID;

            List<ReasonApprovalModel> reasonApprovalList;

            ReasonApprovalSummaryModel reasonApprovalSummaryModel;
            List<ReasonApprovalSummaryModel> reasonApprovalSummaryList = new List<ReasonApprovalSummaryModel>();

            List<AnnouncementModel> announcementList = new List<AnnouncementModel>();

            CheckProductCode();
            //GetHDDSerialNo();
            //GetMotherBoardID();

            if (!User.Identity.IsAuthenticated || CheckProductCode() == false)
            {

                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                // Get Cuurent Month Start and End Date         
                DateTime currentDate;
                string tempDate;

                DateTime startOn;
                DateTime endOn;

                int totalLateIn = 0;
                int totalEarlyOut = 0;
                int totalLateInEarlyOut = 0;
                int totalIncomplete = 0;
                int totalAbsent = 0;
                int totalAttend = 0;
                int totalOnLeave = 0;

                startOn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
                endOn = startOn.AddDays((days - 1));

                attendanceList = attendanceDBService.GetMonthlyAttendance(userModel.NRIC, userModel.USRID, userModel.UserName, userModel.DepartmentID, userModel.DepartmentName, startOn, endOn, userModel.AccessRoleID, "");

                // Attendance Summary
                totalLateIn = attendanceList.Where(a => a.AttendanceStatusID == "LIN" && a.IsApproved == false).Count();
                totalEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "EOT" && a.IsApproved == false).Count();
                totalLateInEarlyOut = attendanceList.Where(a => a.AttendanceStatusID == "L/E" && a.IsApproved == false).Count();
                totalIncomplete = attendanceList.Where(a => a.AttendanceStatusID == "ICP" && a.IsApproved == false).Count();
                totalAbsent = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == false).Count();
                totalAttend = attendanceList.Where(a => a.AttendanceStatusID == "NOR" || (a.IsApproved == true && a.IsForOnLeave == false)).Count();
                totalOnLeave = attendanceList.Where(a => a.AttendanceStatusID == "ABS" && a.IsApproved == true && a.IsForOnLeave == true).Count();

                attendanceSummaryModel.TotalLateIn = totalLateIn;
                attendanceSummaryModel.TotalEarlyOut = totalEarlyOut;
                attendanceSummaryModel.TotalLateInEarlyOut = totalLateInEarlyOut;
                attendanceSummaryModel.TotalIncomplete = totalIncomplete;
                attendanceSummaryModel.TotalAbsent = totalAbsent;
                attendanceSummaryModel.TotalAttend = totalAttend;
                attendanceSummaryModel.TotalOnLeave = totalOnLeave;

                homeViewModel.AttendanceSummary = attendanceSummaryModel;

                userAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);


                if (userAccessControlModel.IsAllowedApproveReason.Equals(true))
                {


                    // ReasonApprovalSummary

                    currentDate = DateTime.Now;

                    departmentList = reasonApprovalDBService.GetApproverUserDepartmentList(User.Identity.Name);

                    if (departmentList.Count > 0)
                    {

                        for (int monthNo = 0; monthNo < 2; monthNo++)
                        {

                            int totalPending = 0;
                            int totalApproved = 0;
                            int totalRejected = 0;
                            int totalRequiredToAmend = 0;

                            SystemModel systemModel = new SystemModel();
                            systemModel = systemDBService.GetData();
                            int reasonSubmissionPeriod = systemModel.ReasonSubmissionPeriod;

                            DateTime approvalDueDate = new DateTime();

                            switch (monthNo)
                            {
                                case 0:
                                    // Get Last Month Reason Approval Summary
                                    tempDate = string.Format("{0}-{1}-{2}", currentDate.AddMonths(-1).ToString("yyyy"), currentDate.AddMonths(-1).ToString("MM"), "01");

                                    DateTime.TryParse(tempDate, out startOn);
                                    days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
                                    endOn = startOn.AddDays((days - 1));

                                    break;

                                case 1:
                                    // Get Current Month Reason Approval Summary
                                    tempDate = string.Format("{0}-{1}-{2}", currentDate.ToString("yyyy"), currentDate.ToString("MM"), "01");

                                    DateTime.TryParse(tempDate, out startOn);
                                    days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
                                    endOn = startOn.AddDays((days - 1));

                                    break;
                            }

                            for (int i = 0; i < departmentList.Count; i++)
                            {
                                departmentID = departmentList[i].DepartmentID.ToString();

                                reasonApprovalList = new List<ReasonApprovalModel>();
                                reasonApprovalList = reasonApprovalDBService.GetReasonApprovalList(startOn, endOn, User.Identity.Name, departmentID);

                                totalPending += reasonApprovalList.Where(r => r.IsApproved == false && r.IsRejected == false && r.IsRequestedToAmend == false).Count();
                                totalApproved += reasonApprovalList.Where(r => r.IsApproved == true).Count();
                                totalRejected += reasonApprovalList.Where(r => r.IsRejected == true).Count();
                                totalRequiredToAmend += reasonApprovalList.Where(r => r.IsRequestedToAmend == true).Count();
                            }

                            reasonApprovalSummaryModel = new ReasonApprovalSummaryModel();

                            reasonApprovalSummaryModel.MonthYear = startOn.ToString("MMMM, yyyy");
                            reasonApprovalSummaryModel.TotalPending = totalPending;
                            reasonApprovalSummaryModel.TotalApproved = totalApproved;
                            reasonApprovalSummaryModel.TotalRejected = totalRejected;
                            reasonApprovalSummaryModel.TotalRequestedToAmend = totalRequiredToAmend;

                            // Approval Due Date = Submission Due Date           
                            approvalDueDate = new DateTime();
                            approvalDueDate = endOn.AddDays(reasonSubmissionPeriod);

                            reasonApprovalSummaryModel.ApprovalDueDate = approvalDueDate;
                            reasonApprovalSummaryList.Add(reasonApprovalSummaryModel);
                        }

                    }

                }

                homeViewModel.ReasonApprovalSummaryList = reasonApprovalSummaryList;

                // Announcement
                announcementList = announcementDBService.ListAnnouncement();
                homeViewModel.AnnouncementList = announcementList;

                // User Access Control
                homeViewModel.UserAccessControlModel = userAccessControlModel;

            }

            return View(homeViewModel);
        }

        public ActionResult ContactUs()
        {

            UserViewModel userViewModel = new UserViewModel();
            UserModel userModel = new UserModel();
           
            userModel = userDBService.GetDataByID(User.Identity.Name);
            userViewModel.UserModel = userModel;
            userViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

            ViewBag.UserDetail = string.Format("{0} ( {1} )", userViewModel.UserModel.UserName, userViewModel.UserModel.RoleName);

            return View(userViewModel);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Auth");
        }

        public bool CheckProductCode()
        {
            bool isValid = false;
            UserDBService userDBService = new UserDBService();

            string productCode = System.Web.Configuration.WebConfigurationManager.AppSettings["ProductCode"];


            // By C Drive HDD Serial Number
            string drive = "C";
            string hddID = "";
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive + ":\"");
            disk.Get();
            hddID = disk["VolumeSerialNumber"].ToString();

            string hashHddID = "";

            hashHddID = userDBService.HashData(hddID);

            if (hashHddID.Trim().Equals(productCode.Trim()))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }

        //public static string GetMotherBoardID()
        //{
        //    string mbInfo = String.Empty;
        //    ManagementScope scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
        //    scope.Connect();
        //    ManagementObject wmiClass = new ManagementObject(scope, new ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""), new ObjectGetOptions());

        //    foreach (PropertyData propData in wmiClass.Properties)
        //    {
        //        if (propData.Name == "SerialNumber")

        //            mbInfo = String.Format("{0,-25}{1}", propData.Name, Convert.ToString(propData.Value));
        //    }

        //    return mbInfo;
        //}



    }
}