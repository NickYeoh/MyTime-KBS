using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTime.ViewModels;
using MyTime.Services;
using MyTime.Models;
using System.IO;
using MyTime.Interfaces;

namespace MyTime.Controllers
{
    public class ReasonApprovalController : EnvironmentController
    {
        UserDBService userDBService = new UserDBService();
        UserAccessControlDBService userAccessControlDBService = new UserAccessControlDBService();

        SystemDBService systemDBService = new SystemDBService();
        ReasonApprovalDBService reasonApprovalDBService = new ReasonApprovalDBService();

        // GET: ApproveReason
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();          
            ReasonApprovalViewModel reasonApprovalViewModel = new ReasonApprovalViewModel();

            SystemModel systemModel = new SystemModel();
            DateTime dataStartDate;

            List<DepartmentModel> departmentList = new List<DepartmentModel>();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                userModel = userDBService.GetDataByID(User.Identity.Name);
                TempData["User"] = userModel;

                ViewBag.UserDetail = string.Format("{0} ( {1} )", userModel.UserName, userModel.RoleName);

                reasonApprovalViewModel.UserAccessControlModel = userAccessControlDBService.IsAccessAllowed(userModel.RoleID);

                systemModel = systemDBService.GetData();
                dataStartDate = systemModel.DataStartDate;

                reasonApprovalViewModel.SelectListMonthYear = PrepareSelectMonthYearList(dataStartDate);

                departmentList = reasonApprovalDBService.GetApproverUserDepartmentList(userModel.NRIC);
                reasonApprovalViewModel.SelectListDepartment = PrepareSelectDepartmentList(departmentList);
            }

            return View(reasonApprovalViewModel);
        }

        private IEnumerable<SelectListItem> PrepareSelectMonthYearList(DateTime dataStartDate)
        {
            ISharedFunction sharedFunction;
            sharedFunction = new SharedFunction();

            var selectMonthYearList = new List<SelectListItem>();
            int totalMonthDiff = sharedFunction.CalculateMonthDiff(dataStartDate, DateTime.Now);

            DateTime monthStart;
            string monthYear;

            for (int month = totalMonthDiff; month >= 0; month--)
            {

                monthStart = new DateTime(dataStartDate.AddMonths(month).Year, dataStartDate.AddMonths(month).Month, 1);
                monthYear = dataStartDate.AddMonths(month).ToString("MMM, yyyy");

                selectMonthYearList.Add(new SelectListItem
                {
                    Value = monthStart.ToString("yyyy-MM-dd"),
                    Text = monthYear
                });
            }


            return selectMonthYearList;
        }

        public ActionResult GetPendingReasonList(string startDate, string  departmentID)
        {
            List<ReasonApprovalModel> reasonApprovalList = new List<ReasonApprovalModel>();
            List<ReasonApprovalModel> dataList = new List<ReasonApprovalModel>();

            // Get Month Start and End Date
            DateTime startOn;
            DateTime endOn;

            DateTime.TryParse(startDate, out startOn);
            int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            endOn = startOn.AddDays((days - 1));

            string approvalNRIC = User.Identity.Name;
            reasonApprovalList = reasonApprovalDBService.GetReasonApprovalList( startOn, endOn, approvalNRIC, departmentID);
            
            TempData["ReasonApprovalList"] = reasonApprovalList;

            dataList = reasonApprovalList.Where(ar => ar.IsRejected == false && ar.IsApproved == false && ar.IsRequestedToAmend == false).ToList();
                   

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovedReasonList()
        {
            List<ReasonApprovalModel> reasonApprovalList = new List<ReasonApprovalModel>();
            List<ReasonApprovalModel> dataList = new List<ReasonApprovalModel>();

            reasonApprovalList = TempData["ReasonApprovalList"] as List<ReasonApprovalModel>;
            TempData.Keep("ReasonApprovalList");

            if (reasonApprovalList != null)
            {
                dataList = reasonApprovalList.Where(dl => dl.IsApproved == true).ToList();
          
            }          

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRejectedReasonList()
        {
            List<ReasonApprovalModel> reasonApprovalList = new List<ReasonApprovalModel>();
            List<ReasonApprovalModel> dataList = new List<ReasonApprovalModel>();

            reasonApprovalList = TempData["ReasonApprovalList"] as List<ReasonApprovalModel>;
            TempData.Keep("ReasonApprovalList");

            if (reasonApprovalList != null)
            {
                dataList = reasonApprovalList.Where(dl => dl.IsRejected == true).ToList();

            }
          

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRequestedToAmendReasonList()
        {
            List<ReasonApprovalModel> reasonApprovalList = new List<ReasonApprovalModel>();
            List<ReasonApprovalModel> dataList = new List<ReasonApprovalModel>();

            reasonApprovalList = TempData["ReasonApprovalList"] as List<ReasonApprovalModel>;
            TempData.Keep("ReasonApprovalList");

            if (reasonApprovalList != null)
            {
                dataList = reasonApprovalList.Where(dl => dl.IsRequestedToAmend == true).ToList();

            }
                       

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _ApproveReason(string rowData)
        {
            ReasonApprovalViewModel reasonApprovalViewModel = new ReasonApprovalViewModel();
            ReasonApprovalModel reasonApprovalModel = new ReasonApprovalModel();

            string[] data = rowData.Split('#');

            for (int i = 0; i < 17; i++)
            {
                switch (i)
                {
                    case 0:
                        reasonApprovalModel.AttendanceDate = Convert.ToDateTime(data[i]);
                        break;
                    case 1:
                        reasonApprovalModel.AttendanceDay = data[i];
                        break;
                    case 2:
                        reasonApprovalModel.UserName = data[i];
                        break;
                    case 3:
                        reasonApprovalModel.NRIC = data[i];
                        break;    
                    case 4:
                        reasonApprovalModel.AttendanceStatusID = data[i];
                        break;
                    case 5:
                        reasonApprovalModel.AttendanceStatus = data[i];
                        break;
                    case 6:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.FirstIn = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.FirstIn = "";
                        }

                        break;
                    case 7:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Lateness = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.Lateness = "";
                        }

                        break;
                    case 8:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.LastOut = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.LastOut = "";
                        }

                        break;
                    case 9:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.WorkTime = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.WorkTime = "";
                        }

                        break;
                    //case 10:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.OvertimeStart = data[i].ToString();
                    //    }

                    //    break;
                    //case 11:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.OvertimeEnd = data[i].ToString();
                    //    }

                    //    break;
                    case 10:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Overtime = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.Overtime = "";
                        }

                        break;
                    case 11:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtraStart = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.OvertimeExtraStart = "";
                        }

                        break;
                    case 12:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtraEnd = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.OvertimeExtraEnd = "";
                        }

                        break;
                    case 13:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtra = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.OvertimeExtra = "";
                        }

                        break;

                    //case 16:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.TotalOvertime = data[i].ToString();
                    //    }

                    //    break;

                    case 14:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.ReasonID = data[i].ToString();
                        }

                        break;

                    case 15:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.ReasonName = data[i].ToString();
                        }

                        break;

                    case 16:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Remark = data[i].ToString();
                        }
                        else
                        {
                            reasonApprovalModel.Remark = "";
                        }

                        break;
                }

            }

            reasonApprovalViewModel.ReasonApprovalModel = reasonApprovalModel;

            return PartialView(reasonApprovalViewModel);
        }

        [HttpPost]
        public ActionResult _ApproveReason(ReasonApprovalViewModel reasonApprovalViewModel)
        {
            ReasonApprovalModel reasonApprovalModel = new ReasonApprovalModel();

            UserModel userModel = new UserModel();

            userModel = TempData["User"] as UserModel;

            TempData.Keep("User");

            string approvelNRIC = userModel.NRIC;
            reasonApprovalModel = reasonApprovalViewModel.ReasonApprovalModel;

            if (reasonApprovalDBService.ApproveReason(approvelNRIC, reasonApprovalModel).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }
          
        }


        public ActionResult _RejectReason(string rowData)
        {
            ReasonApprovalViewModel reasonApprovalViewModel = new ReasonApprovalViewModel();
            ReasonApprovalModel reasonApprovalModel = new ReasonApprovalModel();

            string[] data = rowData.Split('#');


            for (int i = 0; i < 17; i++)
            {
                switch (i)
                {
                    case 0:
                        reasonApprovalModel.AttendanceDate = Convert.ToDateTime(data[i]);
                        break;
                    case 1:
                        reasonApprovalModel.AttendanceDay = data[i];
                        break;
                    case 2:
                        reasonApprovalModel.UserName = data[i];
                        break;
                    case 3:
                        reasonApprovalModel.NRIC = data[i];
                        break;
                    case 4:
                        reasonApprovalModel.AttendanceStatusID = data[i];
                        break;
                    case 5:
                        reasonApprovalModel.AttendanceStatus = data[i];
                        break;
                    case 6:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.FirstIn = data[i].ToString();
                        }

                        break;
                    case 7:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Lateness = data[i].ToString();
                        }

                        break;
                    case 8:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.LastOut = data[i].ToString();
                        }

                        break;
                    case 9:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.WorkTime = data[i].ToString();
                        }

                        break;
                    //case 10:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.OvertimeStart = data[i].ToString();
                    //    }

                    //    break;
                    //case 11:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.OvertimeEnd = data[i].ToString();
                    //    }

                    //    break;
                    case 10:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Overtime = data[i].ToString();
                        }

                        break;
                    case 11:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtraStart = data[i].ToString();
                        }

                        break;
                    case 12:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtraEnd = data[i].ToString();
                        }

                        break;
                    case 13:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtra = data[i].ToString();
                        }

                        break;

                    //case 16:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.TotalOvertime = data[i].ToString();
                    //    }

                    //    break;

                    case 14:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.ReasonID = data[i].ToString();
                        }

                        break;

                    case 15:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.ReasonName = data[i].ToString();
                        }

                        break;

                    case 16:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Remark = data[i].ToString();
                        }

                        break;
                }

            }

            reasonApprovalViewModel.ReasonApprovalModel = reasonApprovalModel;

            return PartialView(reasonApprovalViewModel);
        }

        [HttpPost]
        public ActionResult _RejectReason(ReasonApprovalViewModel reasonApprovalViewModel)
        {
            ReasonApprovalModel reasonApprovalModel = new ReasonApprovalModel();

            UserModel userModel = new UserModel();

            userModel = TempData["User"] as UserModel;

            TempData.Keep("User");

            string approvelNRIC = userModel.NRIC;
            reasonApprovalModel = reasonApprovalViewModel.ReasonApprovalModel;

            if (reasonApprovalDBService.RejectReason(approvelNRIC, reasonApprovalModel).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult _RequestToAmendReason(string rowData)
        {
            ReasonApprovalViewModel reasonApprovalViewModel = new ReasonApprovalViewModel();
            ReasonApprovalModel reasonApprovalModel = new ReasonApprovalModel();

            string[] data = rowData.Split('#');


            for (int i = 0; i < 17; i++)
            {
                switch (i)
                {
                    case 0:
                        reasonApprovalModel.AttendanceDate = Convert.ToDateTime(data[i]);
                        break;
                    case 1:
                        reasonApprovalModel.AttendanceDay = data[i];
                        break;
                    case 2:
                        reasonApprovalModel.UserName = data[i];
                        break;
                    case 3:
                        reasonApprovalModel.NRIC = data[i];
                        break;
                    case 4:
                        reasonApprovalModel.AttendanceStatusID = data[i];
                        break;
                    case 5:
                        reasonApprovalModel.AttendanceStatus = data[i];
                        break;
                    case 6:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.FirstIn = data[i].ToString();
                        }

                        break;
                    case 7:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Lateness = data[i].ToString();
                        }

                        break;
                    case 8:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.LastOut = data[i].ToString();
                        }

                        break;
                    case 9:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.WorkTime = data[i].ToString();
                        }

                        break;
                    //case 10:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.OvertimeStart = data[i].ToString();
                    //    }

                    //    break;
                    //case 11:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.OvertimeEnd = data[i].ToString();
                    //    }

                    //    break;
                    case 10:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Overtime = data[i].ToString();
                        }

                        break;
                    case 11:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtraStart = data[i].ToString();
                        }

                        break;
                    case 12:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtraEnd = data[i].ToString();
                        }

                        break;
                    case 13:
                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.OvertimeExtra = data[i].ToString();
                        }

                        break;

                    //case 16:
                    //    if (data[i].ToString() != "null")
                    //    {
                    //        reasonApprovalModel.TotalOvertime = data[i].ToString();
                    //    }

                    //    break;

                    case 14:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.ReasonID = data[i].ToString();
                        }

                        break;

                    case 15:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.ReasonName = data[i].ToString();
                        }

                        break;

                    case 16:

                        if (data[i].ToString() != "null")
                        {
                            reasonApprovalModel.Remark = data[i].ToString();
                        }

                        break;
                }

            }

            reasonApprovalViewModel.ReasonApprovalModel = reasonApprovalModel;

            return PartialView(reasonApprovalViewModel);
        }

        [HttpPost]
        public ActionResult _RequestToAmendReason(ReasonApprovalViewModel reasonApprovalViewModel)
        {
            ReasonApprovalModel reasonApprovalModel = new ReasonApprovalModel();

            UserModel userModel = new UserModel();

            userModel = TempData["User"] as UserModel;

            TempData.Keep("User");

            string approvelNRIC = userModel.NRIC;
            reasonApprovalModel = reasonApprovalViewModel.ReasonApprovalModel;

            if (reasonApprovalDBService.RequestToAmendReason(approvelNRIC, reasonApprovalModel).Equals(false))
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }

        }


        private IEnumerable<SelectListItem> PrepareSelectDepartmentList(List<DepartmentModel> departmentList)
        {
            var selectDepartmentList = new List<SelectListItem>();


            foreach (var row in departmentList)
            {
                selectDepartmentList.Add(new SelectListItem
                {
                    Value = row.DepartmentID.ToString(),
                    Text = row.DepartmentName.ToString()
                });

            }
           
            return selectDepartmentList;
        }


    }
}