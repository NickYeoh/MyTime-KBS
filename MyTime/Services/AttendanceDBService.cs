using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Linq;
using System.Data;
using System.Web;
using System.IO;

using System.Net.Mail;

namespace MyTime.Services
{
    public class AttendanceDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        ShiftScheduleDBService shiftScheduleDBService = new ShiftScheduleDBService();
        HolidayDBService holidayDBService = new HolidayDBService();
        SystemDBService systemDBService = new SystemDBService();
        DeviceDBService deviceDBService = new DeviceDBService();
        DepartmentDBService departmentDBService = new DepartmentDBService();
        UserDBService userDBService = new UserDBService();
        AttendanceCardDBService attendanceCardDBService = new AttendanceCardDBService();

        public AttendanceSummaryModel GetLatestAttendanceSummary(string NRIC, string usrID)
        {
            AttendanceSummaryModel attendanceSummaryModel = new AttendanceSummaryModel();

            try
            {

                attendanceSummaryModel.TotalLateIn = 0;
                attendanceSummaryModel.TotalEarlyOut = 0;
                attendanceSummaryModel.TotalLateInEarlyOut = 0;
                attendanceSummaryModel.TotalAbsent = 0;
                attendanceSummaryModel.TotalOnLeave = 0;
                attendanceSummaryModel.TotalOvertime = string.Format("0{0} 0{1}", MyTime.Resource.Hour, MyTime.Resource.Minute);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return attendanceSummaryModel;

        }

        public List<AttendanceModel> GetMonthlyAttendance(string NRIC, string usrID, string userName, string departmentID, string departmentName, DateTime startOn, DateTime endOn, int accessRoleID, string approverName)
        {


            List<AttendanceModel> attendanceList = new List<AttendanceModel>();

            // For Normal Transaction
            List<DeviceTransactionModel> deviceTransactionList = new List<DeviceTransactionModel>();

            // For O/T Device Transaction
            List<DeviceTransactionModel> deviceOvertimeExtraTransactionList = new List<DeviceTransactionModel>();

            List<UserShiftModel> userShiftList = new List<UserShiftModel>();
            List<HolidayModel> holidayList = new List<HolidayModel>();

            List<AttendanceReasonModel> attendanceReasonList = new List<AttendanceReasonModel>();
            AttendanceReasonModel attendanceReasonModel = new AttendanceReasonModel();

            UserModel userModel = new UserModel();

            bool isResigned = false;

            int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            DateTime attendanceDate;
            DateTime currentDate;


            bool isSubmitted = false;
            bool isApproved = false;
            bool isRejected = false;
            bool isRequestedToAmend = false;
            string reasonID;
            string reasonName;
            bool isForOnLeave = false;
            string remark;
            string proof;

            string approverComment;

            SystemModel systemModel = new SystemModel();
            UserShiftModel userShiftModel = new UserShiftModel();
            HolidayModel holidayModel = new HolidayModel();

            AttendanceModel attendanceModel;
            DeviceTransactionModel deviceTransactionModel;
            DeviceTransactionModel deviceOvertimeExtraTransactionModel;

            int transCount;
            int overtimeExtraTransCount;

            DateTime? firstIn;
            DateTime? lastOut;
            DateTime shiftStart;
            DateTime shiftEnd;
            TimeSpan shiftWorkHour;

            DateTime? overtimeStart;
            DateTime? overtimeEnd;

            DateTime? overtimeExtraStart;
            DateTime? overtimeExtraEnd;

            int flexiTimeInterval;
            DateTime shiftFlexiStart;

            TimeSpan lateness;
            TimeSpan workTime;
            TimeSpan overTime;
            TimeSpan overTimeExtra;
            TimeSpan totalOverTime;

            bool isEarlyIn;
            bool isLateIn;
            bool isEarlyOut;

            string attendanceStatusID = "";

            // 2022-03-18
            string attendanceCardStatus = "";


            // 2022-06-24

            bool isAttendanceTransTableFound = false;

            // Check and Get Submitted Attendance Reason
            bool isAttendanceReasonTableFound = false;

            string attendanceTransTableName = "";
            string attendanceReasonTableName = "";

            string sql;
            SqlCommand cmd;
            SqlDataReader dr;


            try
            {


                // Check User Status
                userModel = userDBService.GetDataByID(NRIC);
                isResigned = Convert.ToBoolean(userModel.IsResigned);


                // Get Attendance Card Status - Warna Kad Kedatangan
                attendanceCardStatus = attendanceCardDBService.GetMonthlyAttendanceCardStatusByID(NRIC, startOn);

                // Current Date
                currentDate = DateTime.Now.Date;

                // Check Submission Due Date
                systemModel = systemDBService.GetData();
                int reasonSubmissionPeriod = systemModel.ReasonSubmissionPeriod;
                DateTime submissionDueDate = endOn.AddDays(reasonSubmissionPeriod);
                bool isSubmissionDue;

                if (currentDate < submissionDueDate.Date)
                {
                    isSubmissionDue = false;
                }
                else
                {
                    isSubmissionDue = true;
                }


                // Check AttendanceTrans_xxxx is exist 
                attendanceTransTableName = "AttendanceTrans_" + startOn.ToString("yyyyMM");
                isAttendanceTransTableFound = CheckServerTableExist(attendanceTransTableName);

                // Check AttendanceReason_xxxx is exist 
                attendanceReasonTableName = "AttendanceReason_" + startOn.ToString("yyyyMM");
                isAttendanceReasonTableFound = CheckServerTableExist(attendanceReasonTableName);

                if ( isAttendanceReasonTableFound == true)
                {
                    attendanceReasonList = GetSubmittedReasonList( attendanceReasonTableName, NRIC);
                }


                // 2022-06-21 - To decide retrieve from pre-generated monthly table or from live
                if (isAttendanceTransTableFound == true)
                {


                    // For Total OverTime
                    totalOverTime = TimeSpan.Zero;

                    sql = $@"SELECT *";
                    sql += " " + $@"FROM {attendanceTransTableName} ";
                    sql += " " + $@"WHERE NRIC='{NRIC}'";
                    sql += " " + $@"ORDER BY AttendanceDate ASC";

                    conn.Open();

                    cmd = new SqlCommand(sql, conn);
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {

                            firstIn = null;
                            lastOut = null;

                            workTime = TimeSpan.Zero;
                            overTime = TimeSpan.Zero;

                            overTimeExtra = TimeSpan.Zero;
                            overtimeExtraStart = null;
                            overtimeExtraEnd = null;

                            attendanceModel = new AttendanceModel();

                            attendanceModel.NRIC = NRIC;
                            attendanceModel.UserName = userName;
                            attendanceModel.DepartmentID = departmentID;
                            attendanceModel.DepartmentName = departmentName;
                                                       
                            attendanceModel.SubmissionDueDate = submissionDueDate;
                            attendanceModel.IsSubmissionDue = isSubmissionDue;

                            // to store approver name (1 only)
                            attendanceModel.ApproverName = approverName;

                            attendanceModel.AttendanceDate= Convert.ToDateTime(dr["AttendanceDate"]);
                            attendanceModel.AttendanceDay = dr["AttendanceDay"].ToString();
                        
                            attendanceModel.ShiftID= dr["ShiftID"].ToString();
                            attendanceModel.AttendanceStatusID = dr["AttendanceStatusID"].ToString();

                            switch (dr["AttendanceStatusID"].ToString())
                            {
                                case "NOR":
                                    attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_NOR;
                                break;
                                case "ABS":
                                    attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_ABS;
                                break;
                                case "ICP":
                                    attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_ICP;
                                break;
                                case "L/E":
                                    attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_L_N;
                                break;
                                case "LIN":
                                    attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_LIN;
                                break;
                                case "EOT":
                                    attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_EOT;
                                break;
                                case "NWK":
                                    attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_NWK;
                                break;
                                case "HLY":
                                    attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_HLY;
                                break;
                                default:
                                    attendanceModel.AttendanceStatus = "";
                                break;                          

                            }

                            if (!dr["FirstIn"].Equals(DBNull.Value))
                            {
                                firstIn = Convert.ToDateTime(dr["FirstIn"]);

                                attendanceModel.FirstIn = firstIn?.ToString("HH:mm").ToLower();
                            }

                            if (!dr["DeviceIn"].Equals(DBNull.Value))
                            {
                                attendanceModel.DeviceIn = dr["DeviceIn"].ToString();
                            }

                            if (!dr["IsEarlyIn"].Equals(DBNull.Value))
                            {
                                attendanceModel.IsEarlyIn = Convert.ToBoolean(dr["IsEarlyIn"]);
                            }

                            if (!dr["Lateness"].Equals(DBNull.Value))
                            {
                                lateness = (TimeSpan)(dr["Lateness"]);

                                attendanceModel.Lateness = string.Format("{0} {1} {2} {3}", lateness.Hours, MyTime.Resource.Hour, lateness.Minutes, MyTime.Resource.Minute);
                            }

                            if (!dr["LastOut"].Equals(DBNull.Value))
                            {
                                lastOut = Convert.ToDateTime(dr["LastOut"]);

                                attendanceModel.LastOut = lastOut?.ToString("HH:mm").ToLower();
                            }

                            if (!dr["DeviceOut"].Equals(DBNull.Value))
                            {
                                attendanceModel.DeviceOut = dr["DeviceOut"].ToString();
                            }

                            if (!dr["WorkTime"].Equals(DBNull.Value))
                            {
                                workTime = (TimeSpan)(dr["WorkTime"]);
                            
                                attendanceModel.WorkTime = string.Format("{0} {1} {2} {3}", workTime.Hours, MyTime.Resource.Hour, workTime.Minutes, MyTime.Resource.Minute);
                            }

                            if (!dr["OverTime"].Equals(DBNull.Value))
                            {
                                overTime = (TimeSpan)(dr["OverTime"]);

                                attendanceModel.Overtime= string.Format("{0} {1} {2} {3}", overTime.Hours, MyTime.Resource.Hour, overTime.Minutes, MyTime.Resource.Minute);
                            }

                            if (!dr["OverTimeExtraStart"].Equals(DBNull.Value))
                            {
                                overtimeExtraStart = Convert.ToDateTime(dr["OverTimeExtraStart"]);

                                attendanceModel.OvertimeExtraStart= overtimeExtraStart?.ToString("HH:mm").ToLower();
                            }

                            if (!dr["OverTimeExtraEnd"].Equals(DBNull.Value))
                            {
                                overtimeExtraEnd = Convert.ToDateTime(dr["OverTimeExtraEnd"]);

                                attendanceModel.OvertimeExtraEnd = overtimeExtraEnd?.ToString("HH:mm").ToLower();
                            }

                            if (!dr["OverTimeExtra"].Equals(DBNull.Value))
                            {
                                overTimeExtra = (TimeSpan)(dr["OverTimeExtra"]);

                                attendanceModel.OvertimeExtra = string.Format("{0} {1} {2} {3}", overTimeExtra.Hours, MyTime.Resource.Hour, overTimeExtra.Minutes, MyTime.Resource.Minute);
                            }

                            totalOverTime = totalOverTime.Add(overTime);
                            totalOverTime = totalOverTime.Add(overTimeExtra);

                            //if (totalOverTime > TimeSpan.Zero)
                            //{
                            attendanceModel.TotalOvertime = string.Format("{0} {1} {2} {3}", totalOverTime.Hours, MyTime.Resource.Hour, totalOverTime.Minutes, MyTime.Resource.Minute);
                            //}
                                                   

                            attendanceModel.AttendanceCardStatus = attendanceCardStatus;

                            if ((dr["AttendanceStatusID"].ToString() != "NOR") && (dr["AttendanceStatusID"].ToString() != "NWK") && (dr["AttendanceStatusID"].ToString() != "HLY"))
                            {
                                isSubmitted = false;
                                isApproved = false;
                                isRejected = false;
                                isRequestedToAmend = false;
                                reasonID = "";
                                reasonName = "";
                                isForOnLeave = false;
                                remark = "";
                                proof = "";

                                approverComment = "";

                                // Check submitted reason detail
                                if (attendanceReasonList.Count != 0)
                                {

                                    attendanceReasonModel = attendanceReasonList.Where(ar => ar.AttendanceDate.Date == Convert.ToDateTime(dr["AttendanceDate"])).FirstOrDefault();

                                    if (attendanceReasonModel != null)
                                    {
                                        isSubmitted = true;
                                        isApproved = attendanceReasonModel.IsApproved;
                                        isRejected = attendanceReasonModel.IsRejected;
                                        isRequestedToAmend = attendanceReasonModel.IsRequestedToAmend;
                                        reasonID = attendanceReasonModel.ReasonID;
                                        reasonName = attendanceReasonModel.ReasonName;
                                        isForOnLeave = attendanceReasonModel.IsForOnLeave;
                                        remark = attendanceReasonModel.Remark;
                                        proof = attendanceReasonModel.Proof;
                                        approverComment = attendanceReasonModel.ApproverComment;

                                    }

                                }

                                if (isSubmitted == true)
                                {
                                    if (isApproved != true)
                                    {
                                        if (isSubmissionDue == true)
                                        {
                                            // Overdue, set all pending to rejected
                                            isRejected = true;
                                            isRequestedToAmend = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (isSubmissionDue == true)
                                    {
                                        // Overdue, set all pending to rejected
                                        isRejected = true;
                                        isRequestedToAmend = false;
                                    }
                                }

                                attendanceModel.IsSubmitted = isSubmitted;
                                attendanceModel.IsApproved = isApproved;
                                attendanceModel.IsRejected = isRejected;
                                attendanceModel.IsRequestedToAmend = isRequestedToAmend;
                                attendanceModel.ReasonID = reasonID;
                                attendanceModel.ReasonName = reasonName;
                                attendanceModel.IsForOnLeave = isForOnLeave;
                                attendanceModel.Remark = remark;
                                attendanceModel.Proof = proof;
                                attendanceModel.ApproverComment = approverComment;
                            }


                            attendanceList.Add(attendanceModel);
                        }

                    }
                    conn.Close();
                  

                }
                else
                {

                    //// Check User Status
                    //userModel = userDBService.GetDataByID(NRIC);

                    //// Get Attendance Card Status - Warna Kad Kedatangan
                    //attendanceCardStatus = userDBService.GetMonthlyAttendanceCardStatusByID(NRIC, startOn);

                    //isResigned = Convert.ToBoolean(userModel.IsResigned);                 

                    //// Check and Get Submitted Attendance Reason
                    ////attendanceReasonTableName = "AttendanceReason_" + startOn.ToString("yyyyMM");
                    ////isAttendanceReasonTableFound = CheckServerTableExist(attendanceReasonTableName);

                    //if (isAttendanceReasonTableFound == true)
                    //{
                    //    attendanceReasonList = GetSubmittedReasonList(attendanceReasonTableName, NRIC);
                    //}


                    // Get Device Transaction

                    string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

                    switch (device)
                    {
                        case "JohnsonControl":

                            // 2021-11-30 to overcome the last day of the month data not retrieved
                            endOn = endOn.AddDays(1);
                            //endOn = endOn.AddSeconds(-1);

                            // 2021-11-19 - changed to by NRIC to retrieve data

                            // Normal Transaction
                            deviceTransactionList = deviceDBService.GetJohnsonControlDeviceTrans(NRIC, startOn, endOn, accessRoleID, false);

                            // Overtime Extra Transaction
                            deviceOvertimeExtraTransactionList = deviceDBService.GetJohnsonControlDeviceTrans(NRIC, startOn, endOn, accessRoleID, true);
                            break;

                        case "Suprema":

                            // Normal Transaction
                            deviceTransactionList = deviceDBService.GetSupremaDeviceTrans(usrID, startOn, endOn, accessRoleID, false);

                            // Overtime Extra Transaction
                            deviceOvertimeExtraTransactionList = deviceDBService.GetSupremaDeviceTrans(usrID, startOn, endOn, accessRoleID, true);
                            break;
                    }



                    // Get user shift schedule
                    userShiftList = shiftScheduleDBService.GetUserShift(NRIC);
                    holidayList = holidayDBService.GetYearHoliday(startOn.ToString("yyyy"));
                                   

                    // For Total OverTime
                    totalOverTime = TimeSpan.Zero;

                    // Get total days in the month           
                    for (int day = 0; day < days; day++)
                    {

                        firstIn = null;
                        lastOut = null;

                        workTime = TimeSpan.Zero;
                        overTime = TimeSpan.Zero;

                        overTimeExtra = TimeSpan.Zero;
                        overtimeExtraStart = null;
                        overtimeExtraEnd = null;

                        shiftStart = new DateTime();
                        shiftEnd = new DateTime();

                        flexiTimeInterval = 0;
                        shiftFlexiStart = new DateTime();

                        isEarlyIn = false;
                        isLateIn = false;
                        isEarlyOut = false;

                        deviceTransactionModel = new DeviceTransactionModel();
                        attendanceModel = new AttendanceModel();

                        attendanceModel.NRIC = NRIC;
                        attendanceModel.UserName = userName;
                        attendanceModel.DepartmentName = departmentName;
                        attendanceModel.DepartmentID = departmentID;

                        attendanceDate = startOn.AddDays(day);
                        attendanceModel.AttendanceDate = attendanceDate;
                        attendanceModel.AttendanceDay = attendanceDate.ToString("ddd", new System.Globalization.CultureInfo("ms-MY"));

                        attendanceModel.AttendanceCardStatus = attendanceCardStatus;

                        //switch (attendanceCardStatus)
                        //{
                        //    case "YL":

                        //        attendanceModel.AttendanceCardStatus = MyTime.Resource.Colour_Yellow;
                        //        break;

                        //    case "GN":

                        //        attendanceModel.AttendanceCardStatus = MyTime.Resource.Colour_Green;
                        //        break;

                        //    case "RD":

                        //        attendanceModel.AttendanceCardStatus = MyTime.Resource.Colour_Red;
                        //        break;
                        //}

                        attendanceStatusID = "";

                        if (attendanceDate.Date <= currentDate.Date && (isResigned == false || (isResigned == true && Convert.ToDateTime(userModel.ResignedOn) > attendanceDate.Date)))
                        //if (attendanceDate.Date <= currentDate.Date )
                        {

                            // Device Transaction
                            transCount = 0;
                            transCount = deviceTransactionList.Where(d => d.TRDateTime.Date == attendanceDate.Date).Count();

                            if (transCount > 0)
                            {
                                deviceTransactionModel = new DeviceTransactionModel();
                                deviceTransactionModel = deviceTransactionList.Where(d => d.TRDateTime.Date == attendanceDate.Date).OrderBy(d => d.TRDateTime).First();

                                firstIn = deviceTransactionModel.TRDateTime;

                                if (transCount > 1)
                                {
                                    deviceTransactionModel = new DeviceTransactionModel();
                                    deviceTransactionModel = deviceTransactionList.Where(d => d.TRDateTime.Date == attendanceDate.Date).OrderBy(d => d.TRDateTime).Last();

                                    lastOut = deviceTransactionModel.TRDateTime;

                                }
                            }

                            // Device Overtime Extra Transaction
                            overtimeExtraTransCount = 0;

                            if (lastOut != null)
                            {
                                overtimeExtraTransCount = deviceOvertimeExtraTransactionList.Where(d => d.TRDateTime.Date == attendanceDate.Date && d.TRDateTime > lastOut).Count();
                            }
                            else
                            {
                                overtimeExtraTransCount = deviceOvertimeExtraTransactionList.Where(d => d.TRDateTime.Date == attendanceDate.Date).Count();
                            }

                            if (overtimeExtraTransCount > 0)
                            {
                                deviceOvertimeExtraTransactionModel = new DeviceTransactionModel();


                                if (lastOut != null)
                                {
                                    deviceOvertimeExtraTransactionModel = deviceOvertimeExtraTransactionList.Where(d => d.TRDateTime.Date == attendanceDate.Date && d.TRDateTime > lastOut).OrderBy(d => d.TRDateTime).First();
                                }
                                else
                                {
                                    deviceOvertimeExtraTransactionModel = deviceOvertimeExtraTransactionList.Where(d => d.TRDateTime.Date == attendanceDate.Date).OrderBy(d => d.TRDateTime).First();

                                }
                                overtimeExtraStart = deviceOvertimeExtraTransactionModel.TRDateTime;

                                if (overtimeExtraTransCount > 1)
                                {
                                    deviceOvertimeExtraTransactionModel = new DeviceTransactionModel();
                                    deviceOvertimeExtraTransactionModel = deviceOvertimeExtraTransactionList.Where(d => d.TRDateTime.Date == attendanceDate.Date).OrderBy(d => d.TRDateTime).Last();
                                    overtimeExtraEnd = deviceOvertimeExtraTransactionModel.TRDateTime;
                                }
                            }

                            // Get Shift
                            userShiftModel = userShiftList.Where(s => s.EffectiveOn <= attendanceDate).Last();
                            attendanceModel.ShiftID = userShiftModel.ShiftID;

                            //string attendanceStatusID = "";

                            switch (Convert.ToInt32(attendanceDate.DayOfWeek))
                            {
                                case 0:
                                    // Sun

                                    shiftStart = Convert.ToDateTime(userShiftModel.TimeIn7);
                                    shiftEnd = Convert.ToDateTime(userShiftModel.TimeOut7);
                                    flexiTimeInterval = Convert.ToInt32(userShiftModel.FlexiTimeInterval7);

                                    if (userShiftModel.IsWorkDay7 == true)
                                    {
                                        attendanceStatusID = "NOR";
                                    }
                                    else
                                    {
                                        attendanceStatusID = "NWK";
                                    }
                                    break;

                                case 1:
                                    // Mon

                                    shiftStart = Convert.ToDateTime(userShiftModel.TimeIn1);
                                    shiftEnd = Convert.ToDateTime(userShiftModel.TimeOut1);
                                    flexiTimeInterval = Convert.ToInt32(userShiftModel.FlexiTimeInterval1);

                                    if (userShiftModel.IsWorkDay1 == true)
                                    {
                                        attendanceStatusID = "NOR";
                                    }
                                    else
                                    {
                                        attendanceStatusID = "NWK";
                                    }
                                    break;

                                case 2:
                                    // Tue

                                    shiftStart = Convert.ToDateTime(userShiftModel.TimeIn2);
                                    shiftEnd = Convert.ToDateTime(userShiftModel.TimeOut2);
                                    flexiTimeInterval = Convert.ToInt32(userShiftModel.FlexiTimeInterval2);

                                    if (userShiftModel.IsWorkDay2 == true)
                                    {
                                        attendanceStatusID = "NOR";
                                    }
                                    else
                                    {
                                        attendanceStatusID = "NWK";
                                    }
                                    break;

                                case 3:
                                    // Wed

                                    shiftStart = Convert.ToDateTime(userShiftModel.TimeIn3);
                                    shiftEnd = Convert.ToDateTime(userShiftModel.TimeOut3);
                                    flexiTimeInterval = Convert.ToInt32(userShiftModel.FlexiTimeInterval3);

                                    if (userShiftModel.IsWorkDay3 == true)
                                    {
                                        attendanceStatusID = "NOR";
                                    }
                                    else
                                    {
                                        attendanceStatusID = "NWK";
                                    }
                                    break;

                                case 4:
                                    //Thu

                                    shiftStart = Convert.ToDateTime(userShiftModel.TimeIn4);
                                    shiftEnd = Convert.ToDateTime(userShiftModel.TimeOut4);
                                    flexiTimeInterval = Convert.ToInt32(userShiftModel.FlexiTimeInterval4);

                                    if (userShiftModel.IsWorkDay4 == true)
                                    {
                                        attendanceStatusID = "NOR";
                                    }
                                    else
                                    {
                                        attendanceStatusID = "NWK";
                                    }
                                    break;

                                case 5:
                                    // Fri

                                    shiftStart = Convert.ToDateTime(userShiftModel.TimeIn5);
                                    shiftEnd = Convert.ToDateTime(userShiftModel.TimeOut5);
                                    flexiTimeInterval = Convert.ToInt32(userShiftModel.FlexiTimeInterval5);

                                    if (userShiftModel.IsWorkDay5 == true)
                                    {
                                        attendanceStatusID = "NOR";
                                    }
                                    else
                                    {
                                        attendanceStatusID = "NWK";
                                    }
                                    break;

                                case 6:
                                    // Sat

                                    shiftStart = Convert.ToDateTime(userShiftModel.TimeIn6);
                                    shiftEnd = Convert.ToDateTime(userShiftModel.TimeOut6);
                                    flexiTimeInterval = Convert.ToInt32(userShiftModel.FlexiTimeInterval6);

                                    if (userShiftModel.IsWorkDay6 == true)
                                    {
                                        attendanceStatusID = "NOR";
                                    }
                                    else
                                    {
                                        attendanceStatusID = "NWK";
                                    }
                                    break;

                                default:
                                    break;
                            }

                            // Calculate shift flexi start and work hour

                            shiftFlexiStart = shiftStart.AddMinutes(flexiTimeInterval);
                            shiftWorkHour = shiftEnd.Subtract(shiftStart);

                            // Get Holiday             
                            holidayModel = holidayList.Where(h => h.StartOn <= attendanceDate).Where(h => h.EndOn >= attendanceDate).FirstOrDefault();

                            if (holidayModel != null)
                            {
                                attendanceStatusID = "HLY";
                            }


                            if (attendanceStatusID.Equals("NOR"))
                            {
                                // Check ABS / LIN / EOT / L/E / ICP

                                // Check Status
                                if (!firstIn.Equals(null))
                                {
                                    attendanceModel.FirstIn = firstIn?.ToString("HH:mm").ToLower();

                                    if (firstIn?.TimeOfDay < shiftStart.TimeOfDay)
                                    {
                                        // Come Early
                                        isEarlyIn = true;
                                    }

                                    if (!lastOut.Equals(null))
                                    {
                                        attendanceModel.LastOut = lastOut?.ToString("HH:mm").ToLower();

                                        //if (firstIn?.TimeOfDay < shiftStart.TimeOfDay)
                                        if (isEarlyIn == true)
                                        {
                                            //workTime = lastOut.Value.Subtract(shiftStart);

                                            if (lastOut?.TimeOfDay > shiftStart.TimeOfDay)
                                            {
                                                workTime = (TimeSpan)(lastOut?.TimeOfDay.Subtract(shiftStart.TimeOfDay));
                                            }
                                            else
                                            {
                                                workTime = TimeSpan.Zero;
                                            }



                                        }
                                        else
                                        {
                                            workTime = lastOut.Value.Subtract(firstIn.Value);


                                            // Check Late In
                                            //if (firstIn.Value.TimeOfDay > shiftFlexiStart.TimeOfDay.Add(new TimeSpan(0,1,0)))
                                            if (firstIn.Value.TimeOfDay > shiftFlexiStart.TimeOfDay)
                                            {
                                                lateness = firstIn.Value.Subtract(shiftStart);
                                                attendanceModel.Lateness = string.Format("{0} {1} {2} {3}", lateness.Hours, MyTime.Resource.Hour, lateness.Minutes, MyTime.Resource.Minute);

                                                isLateIn = true;
                                            }
                                        }

                                        attendanceModel.WorkTime = string.Format("{0} {1} {2} {3}", workTime.Hours, MyTime.Resource.Hour, workTime.Minutes, MyTime.Resource.Minute);

                                        // Check Early Out
                                        if (shiftWorkHour.Subtract(workTime) > TimeSpan.Zero)
                                        {
                                            isEarlyOut = true;

                                        }

                                        if (isLateIn == true && isEarlyOut == true)
                                        {
                                            attendanceStatusID = "L/E";
                                        }
                                        else
                                        {
                                            if (isLateIn == true)
                                            {
                                                attendanceStatusID = "LIN";
                                            }
                                            else
                                            {
                                                if (isEarlyOut == true)
                                                {
                                                    attendanceStatusID = "EOT";
                                                }
                                                else
                                                {
                                                    overTime = workTime.Subtract(shiftWorkHour);

                                                    if (overTime > TimeSpan.Zero)
                                                    {

                                                        // 2022-05-03 - For KBS, please ignore this
                                                        //overTime = overTime.Subtract(new TimeSpan(0, 1, 0));

                                                        overtimeStart = new DateTime();
                                                        overtimeEnd = new DateTime();

                                                        overtimeStart = shiftEnd.AddMinutes(1);
                                                        overtimeEnd = lastOut;

                                                        attendanceModel.Overtime = string.Format("{0} {1} {2} {3}", overTime.Hours, MyTime.Resource.Hour, overTime.Minutes, MyTime.Resource.Minute);
                                                        attendanceModel.OvertimeStart = overtimeStart?.ToString("HH:mm").ToLower();
                                                        attendanceModel.OvertimeEnd = overtimeEnd?.ToString("HH:mm").ToLower();

                                                        //attendanceModel.Overtime = string.Format("{0} {1} {2} {3}", overTime.Hours, MyTime.Resource.Hour, overTime.Minutes, MyTime.Resource.Minute);
                                                    }

                                                }

                                            }
                                        }

                                    }
                                    else
                                    {
                                        // Incomplete
                                        attendanceStatusID = "ICP";
                                    }

                                }
                                else
                                {
                                    // Absent
                                    attendanceStatusID = "ABS";
                                }

                                isSubmitted = false;
                                isApproved = false;
                                isRejected = false;
                                isRequestedToAmend = false;
                                reasonID = "";
                                reasonName = "";
                                isForOnLeave = false;
                                remark = "";
                                proof = "";

                                approverComment = "";

                                // Check submitted reason detail
                                if (attendanceReasonList.Count != 0)
                                {

                                    attendanceReasonModel = attendanceReasonList.Where(ar => ar.AttendanceDate.Date == attendanceDate.Date).FirstOrDefault();

                                    if (attendanceReasonModel != null)
                                    {
                                        isSubmitted = true;
                                        isApproved = attendanceReasonModel.IsApproved;
                                        isRejected = attendanceReasonModel.IsRejected;
                                        isRequestedToAmend = attendanceReasonModel.IsRequestedToAmend;
                                        reasonID = attendanceReasonModel.ReasonID;
                                        reasonName = attendanceReasonModel.ReasonName;
                                        isForOnLeave = attendanceReasonModel.IsForOnLeave;
                                        remark = attendanceReasonModel.Remark;
                                        proof = attendanceReasonModel.Proof;
                                        approverComment = attendanceReasonModel.ApproverComment;

                                    }

                                }

                                if (isSubmitted == true)
                                {
                                    if (isApproved != true)
                                    {
                                        if (isSubmissionDue == true)
                                        {
                                            // Overdue, set all pending to rejected
                                            isRejected = true;
                                            isRequestedToAmend = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (isSubmissionDue == true)
                                    {
                                        // Overdue, set all pending to rejected
                                        isRejected = true;
                                        isRequestedToAmend = false;
                                    }
                                }

                                attendanceModel.IsSubmitted = isSubmitted;
                                attendanceModel.IsApproved = isApproved;
                                attendanceModel.IsRejected = isRejected;
                                attendanceModel.IsRequestedToAmend = isRequestedToAmend;
                                attendanceModel.ReasonID = reasonID;
                                attendanceModel.ReasonName = reasonName;
                                attendanceModel.IsForOnLeave = isForOnLeave;
                                attendanceModel.Remark = remark;
                                attendanceModel.Proof = proof;
                                attendanceModel.ApproverComment = approverComment;
                            }
                            else
                            {
                                // NWK & HLY

                                // Check Status
                                if (!firstIn.Equals(null))
                                {
                                    attendanceModel.FirstIn = firstIn?.ToString("HH:mm").ToLower();

                                    if (firstIn?.TimeOfDay < shiftStart.TimeOfDay)
                                    {
                                        // Come Early
                                        isEarlyIn = true;
                                    }

                                    if (!lastOut.Equals(null))
                                    {
                                        attendanceModel.LastOut = lastOut?.ToString("HH:mm").ToLower();

                                        //if (firstIn?.TimeOfDay < shiftStart.TimeOfDay)
                                        if (isEarlyIn == true)
                                        {
                                            //workTime = lastOut.Value.Subtract(shiftStart);
                                            //workTime = (TimeSpan)(lastOut?.TimeOfDay.Subtract(shiftStart.TimeOfDay));

                                            if (lastOut?.TimeOfDay > shiftStart.TimeOfDay)
                                            {
                                                workTime = (TimeSpan)(lastOut?.TimeOfDay.Subtract(shiftStart.TimeOfDay));
                                            }
                                            else
                                            {
                                                workTime = TimeSpan.Zero;
                                            }
                                        }
                                        else
                                        {
                                            workTime = lastOut.Value.Subtract(firstIn.Value);

                                            // Check Late In
                                            if (firstIn.Value.TimeOfDay > shiftFlexiStart.TimeOfDay)
                                            {
                                                lateness = firstIn.Value.Subtract(shiftStart);
                                                attendanceModel.Lateness = string.Format("{0} {1} {2} {3}", lateness.Hours, MyTime.Resource.Hour, lateness.Minutes, MyTime.Resource.Minute);

                                                isLateIn = true;
                                            }

                                        }

                                        attendanceModel.WorkTime = string.Format("{0} {1} {2} {3}", workTime.Hours, MyTime.Resource.Hour, workTime.Minutes, MyTime.Resource.Minute);

                                        //overTime = workTime.Subtract(shiftWorkHour);

                                        overTime = workTime;

                                        if (overTime > TimeSpan.Zero)
                                        {

                                            overtimeStart = new DateTime();
                                            overtimeEnd = new DateTime();

                                            overtimeStart = shiftEnd.AddMinutes(1);
                                            overtimeEnd = lastOut;

                                            attendanceModel.Overtime = string.Format("{0} {1} {2} {3}", overTime.Hours, MyTime.Resource.Hour, overTime.Minutes, MyTime.Resource.Minute);
                                            attendanceModel.OvertimeStart = overtimeStart?.ToString("HH:mm").ToLower();
                                            attendanceModel.OvertimeEnd = overtimeEnd?.ToString("HH:mm").ToLower();

                                        }

                                    }

                                    isSubmitted = false;
                                    isApproved = false;
                                    isRejected = false;
                                    isRequestedToAmend = false;
                                    reasonID = "";
                                    reasonName = "";
                                    isForOnLeave = false;
                                    remark = "";
                                    proof = "";

                                    approverComment = "";

                                    attendanceModel.IsSubmitted = isSubmitted;
                                    attendanceModel.IsApproved = isApproved;
                                    attendanceModel.IsRejected = isRejected;
                                    attendanceModel.IsRequestedToAmend = isRequestedToAmend;
                                    attendanceModel.ReasonID = reasonID;
                                    attendanceModel.ReasonName = reasonName;
                                    attendanceModel.IsForOnLeave = isForOnLeave;
                                    attendanceModel.Remark = remark;
                                    attendanceModel.Proof = proof;

                                    attendanceModel.ApproverComment = approverComment;

                                }
                            }
                        }


                        attendanceModel.AttendanceStatusID = attendanceStatusID;

                        switch (attendanceStatusID)
                        {
                            case "NOR":
                                attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_NOR;
                                break;
                            case "ABS":
                                attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_ABS;
                                break;
                            case "ICP":
                                attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_ICP;
                                break;
                            case "L/E":
                                attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_L_N;
                                break;
                            case "LIN":
                                attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_LIN;
                                break;
                            case "EOT":
                                attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_EOT;
                                break;
                            case "NWK":
                                attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_NWK;
                                break;
                            case "HLY":
                                attendanceModel.AttendanceStatus = MyTime.Resource.AttendanceStatus_HLY;
                                break;
                            default:
                                attendanceModel.AttendanceStatus = "";
                                break;
                        }

                        attendanceModel.IsEarlyIn = isEarlyIn;
                        attendanceModel.SubmissionDueDate = submissionDueDate;
                        attendanceModel.IsSubmissionDue = isSubmissionDue;

                        // to store approver name (1 only)
                        attendanceModel.ApproverName = approverName;

                        // For Testing - manual input value
                        //overtimeExtraStart = DateTime.Now;
                        //overtimeExtraEnd = overtimeExtraStart.Value.AddMinutes(20);

                      

                        // to store overtime extra
                        if (!overtimeExtraStart.Equals(null))
                        {

                            attendanceModel.OvertimeExtraStart = overtimeExtraStart?.ToString("HH:mm").ToLower();

                            if (!overtimeExtraEnd.Equals(null))
                            {
                                attendanceModel.OvertimeExtraEnd = overtimeExtraEnd?.ToString("HH:mm").ToLower();

                                overTimeExtra = (TimeSpan)(overtimeExtraEnd?.TimeOfDay.Subtract((TimeSpan)(overtimeExtraStart?.TimeOfDay)));

                                attendanceModel.OvertimeExtra = string.Format("{0} {1} {2} {3}", overTimeExtra.Hours, MyTime.Resource.Hour, overTimeExtra.Minutes, MyTime.Resource.Minute);

                            }
                        }

                        //totalOverTime = overTime.Add(overTimeExtra);

                        totalOverTime = totalOverTime.Add(overTime);
                        totalOverTime = totalOverTime.Add(overTimeExtra);

                        //if (totalOverTime > TimeSpan.Zero)
                        //{
                        attendanceModel.TotalOvertime = string.Format("{0} {1} {2} {3}", totalOverTime.Hours, MyTime.Resource.Hour, totalOverTime.Minutes, MyTime.Resource.Minute);
                        //}

                        attendanceList.Add(attendanceModel);
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }



            return attendanceList;
        }


        public bool SubmitReason(AttendanceReasonModel attendanceReasonModel, string userName, string userEmail, List<ApproverUserModel> approverUserList, string isEmailNotificationEnabled)
        {
            bool status = false;
            bool isTableFound = false;
            string sql;

            SqlCommand cmd;

            string postedProof;
            string proof;

            string approverNRIC;

            UserModel userModel;
            string approverEmail;

            try
            {

                // Create Attendance Reason Table
                string tableName = "AttendanceReason_" + attendanceReasonModel.AttendanceDate.ToString("yyyyMM");

                isTableFound = CheckServerTableExist(tableName);

                if (!isTableFound == true)
                {
                    // Create New Attendance Reason

                    //sql = "CREATE TABLE " + tableName;
                    //sql += " " + "(NRIC NVARCHAR(20) NOT NULL, AttendanceDate DATETIME NOT NULL, AttendanceDay NVARCHAR(10) NOT NULL,";
                    //sql += " " + "AttendanceStatusID NVARCHAR(3), AttendanceStatus NVARCHAR(25),";
                    //sql += " " + "FirstIn NVARCHAR(20), Lateness NVARCHAR(20), LastOut NVARCHAR(20),";
                    //sql += " " + "WorkTime NVARCHAR(20), OvertimeStart NVARCHAR(20), OvertimeEnd NVARCHAR(20), Overtime NVARCHAR(20),";
                    //sql += " " + "OvertimeExtraStart NVARCHAR(20), OvertimeExtraEnd NVARCHAR(20), OvertimeExtra NVARCHAR(20), TotalOvertime NVARCHAR(20),";
                    //sql += " " + "ReasonID NVARCHAR(20) NOT NULL, Remark NVARCHAR(100), SubmittedOn DATETIME NOT NULL,";
                    //sql += " " + "IsApproved BIT, IsRejected BIT, IsRequestedToAmend BIT,";
                    //sql += " " + "Proof NVARCHAR(100), ApproverComment NVARCHAR(100),";
                    //sql += " " + "ProcessedBy NVARCHAR(20), ProcessedOn DATETIME";
                    //sql += " " + "PRIMARY KEY (NRIC, AttendanceDate))";


                    // 2022-06-29 : Create New Attendance Reason

                    sql = "CREATE TABLE " + tableName;
                    sql += " " + "(NRIC NVARCHAR(20) NOT NULL, AttendanceDate DATETIME NOT NULL, ";
                    sql += " " + "ShiftID NVARCHAR(20), AttendanceStatusID NVARCHAR(3),";
                    sql += " " + "FirstIn DATETIME, Lateness TIME(0), LastOut DATETIME,";
                    sql += " " + "WorkTime TIME(0), Overtime TIME(0),";
                    sql += " " + "OvertimeExtraStart DATETIME, OvertimeExtraEnd DATETIME, OvertimeExtra TIME(0),";
                    sql += " " + "ReasonID NVARCHAR(20) NOT NULL, Remark NVARCHAR(100), SubmittedOn DATETIME NOT NULL,";
                    sql += " " + "IsApproved BIT, IsRejected BIT, IsRequestedToAmend BIT,";
                    sql += " " + "Proof NVARCHAR(100), ApproverComment NVARCHAR(100),";
                    sql += " " + "ProcessedBy NVARCHAR(20), ProcessedOn DATETIME";
                    sql += " " + "PRIMARY KEY (NRIC, AttendanceDate))";


                    conn.Open();

                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();

                }

                // Delete previous submission is any
                sql = $@"DELETE " + tableName;
                sql += " " + $@"WHERE NRIC='{attendanceReasonModel.NRIC}'";
                sql += " " + $@"AND CONVERT(nvarchar, AttendanceDate,112) ='{attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd")}'";
                conn.Open();

                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                // Upload the proof to server before insert record
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Proof/");
                string destination = "";
                proof = "";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (attendanceReasonModel.PostedProof != null)
                {
                    postedProof = attendanceReasonModel.PostedProof.FileName;
                    proof = string.Format("P{0}_{1}{2}", attendanceReasonModel.NRIC, attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd"), Path.GetExtension(postedProof));
                    destination = path + proof;
                    attendanceReasonModel.PostedProof.SaveAs(destination);
                }

                //string totalOvertime = attendanceReasonModel.TotalOvertime;
                //totalOvertime += " #";

                //// Insert 
                //sql = $@"INSERT INTO {tableName} (NRIC, AttendanceDate, AttendanceDay,";
                //sql += " " + $@"AttendanceStatusID, AttendanceStatus,";
                //sql += " " + $@"FirstIn, Lateness, LastOut, WorkTime,";
                //sql += " " + $@"OvertimeStart, OvertimeEnd, Overtime,";
                //sql += " " + $@"OvertimeExtraStart, OvertimeExtraEnd, OvertimeExtra, TotalOvertime,";
                //sql += " " + $@"ReasonID, Remark, Proof, SubmittedOn,";
                //sql += " " + $@"IsApproved, IsRejected, IsRequestedToAmend) VALUES";
                ////sql += " " + $@"ProcessedBy, ProcessedOn) VALUES";
                //sql += " " + $@"('{attendanceReasonModel.NRIC}', '{attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd HH:mm:ss")}', '{attendanceReasonModel.AttendanceDay}',";
                //sql += " " + $@"'{attendanceReasonModel.AttendanceStatusID}', '{attendanceReasonModel.AttendanceStatus}',";
                //sql += " " + $@"'{attendanceReasonModel.FirstIn}', '{attendanceReasonModel.Lateness}', '{attendanceReasonModel.LastOut}', '{attendanceReasonModel.WorkTime}',";
                //sql += " " + $@"'{attendanceReasonModel.OvertimeStart}','{attendanceReasonModel.OvertimeEnd}' ,  '{attendanceReasonModel.Overtime}',";
                //sql += " " + $@"'{attendanceReasonModel.OvertimeExtraStart}','{attendanceReasonModel.OvertimeExtraEnd}' , '{attendanceReasonModel.OvertimeExtra}', '{attendanceReasonModel.TotalOvertime}',";
                //sql += " " + $@"'{attendanceReasonModel.ReasonID}', '{attendanceReasonModel.Remark}', '{proof}', '{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}',";
                //sql += " " + $@"'False', 'False', 'False')";
                ////sql += " " + $@"'{SqlString.Null}', '{DBNull.Value}')";
                ///

                             
                //DateTime? firstIn, lastOut, overTimeExtraStart, overTimeExtraEnd;
                //TimeSpan? lateness, workTime, overTime, overTimeExtra;

                //if (attendanceReasonModel.FirstIn != null && attendanceReasonModel.FirstIn != "")
                //{
                //    firstIn = attendanceReasonModel.AttendanceDate.Add(TimeSpan.Parse( attendanceReasonModel.FirstIn));
                //}
                //else
                //{
                //    firstIn = null;
                //}

                //if (attendanceReasonModel.Lateness !=  null && attendanceReasonModel.Lateness != "")
                //{
                //    lateness = TimeSpan.Parse(attendanceReasonModel.Lateness.Replace("j", ":").Replace("m", "").Replace("h", ":"));
                //}
                //else
                //{
                //    lateness = null;
                //}

                //if (attendanceReasonModel.LastOut !=  null && attendanceReasonModel.LastOut != "")
                //{
                //    lastOut = attendanceReasonModel.AttendanceDate.Add(TimeSpan.Parse( attendanceReasonModel.LastOut));
                //}
                //else
                //{
                //    lastOut = null;
                //}


                //if (attendanceReasonModel.WorkTime !=  null && attendanceReasonModel.WorkTime != "")
                //{

                //    workTime = TimeSpan.Parse(attendanceReasonModel.WorkTime.Replace("j",":").Replace("m","").Replace("h", ":").Replace(" ", ""));
                //}
                //else
                //{
                //    workTime = null;
                //}


                //if (attendanceReasonModel.Overtime != null && attendanceReasonModel.Overtime != "")
                //{
                
                //    overTime = TimeSpan.Parse(attendanceReasonModel.Overtime.Replace("j", ":").Replace("m", "").Replace("h", ":").Replace(" ", ""));
                //}
                //else
                //{
                //    overTime = null;
                //}

                //if (attendanceReasonModel.OvertimeExtraStart != null && attendanceReasonModel.OvertimeExtraStart != "")
                //{
                //    overTimeExtraStart = Convert.ToDateTime(attendanceReasonModel.AttendanceDate).Add(TimeSpan.Parse(attendanceReasonModel.OvertimeExtraStart));
                //}
                //else
                //{
                //    overTimeExtraStart = null;
                //}

                //if (attendanceReasonModel.OvertimeExtraEnd != null && attendanceReasonModel.OvertimeExtraEnd != "")
                //{
                //    overTimeExtraEnd = Convert.ToDateTime(attendanceReasonModel.AttendanceDate).Add(TimeSpan.Parse(attendanceReasonModel.OvertimeExtraEnd));
                //}
                //else
                //{
                //    overTimeExtraEnd = null;
                //}



                //if (attendanceReasonModel.OvertimeExtra != null && attendanceReasonModel.OvertimeExtra != "")
                //{
                //    overTimeExtra = TimeSpan.Parse(attendanceReasonModel.OvertimeExtra.Replace("j", ":").Replace("m", "").Replace("h", ":").Replace(" ", ""));
                //}
                //else
                //{
                //    overTimeExtra = null;
                //}

                // 2022-06-29 : Insert

                sql = $@"INSERT INTO {tableName} (NRIC, AttendanceDate, ";
                sql += " " + $@"ShiftID,";
                sql += " " + $@"AttendanceStatusID,";
                sql += " " + $@"FirstIn,";
                sql += " " + $@"Lateness,";
                sql += " " + $@"LastOut,";
                sql += " " + $@"WorkTime,";
                sql += " " + $@"Overtime,";
                sql += " " + $@"OvertimeExtraStart,";
                sql += " " + $@"OvertimeExtraEnd,";
                sql += " " + $@"OvertimeExtra,";
                sql += " " + $@"ReasonID, Remark, Proof, SubmittedOn,";
                sql += " " + $@"IsApproved, IsRejected, IsRequestedToAmend) VALUES";
                //sql += " " + $@"ProcessedBy, ProcessedOn) VALUES";
                sql += " " + $@"('{attendanceReasonModel.NRIC}', '{attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd HH:mm:ss")}',";
                sql += " " + $@"'{attendanceReasonModel.ShiftID}',";
                sql += " " + $@"'{attendanceReasonModel.AttendanceStatusID}',";

                if (attendanceReasonModel.FirstIn != null && attendanceReasonModel.FirstIn != "")
                {
                    sql += " " + $@"'{attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd")} {attendanceReasonModel.FirstIn}',";
                }
                else
                {
                
                    sql += " " + $@"null,";
                }


                if (attendanceReasonModel.Lateness != null && attendanceReasonModel.Lateness != "")
                {
                    sql += " " + $@"{attendanceReasonModel.Lateness.Replace("j", ":").Replace("m", "").Replace("h", ":")},";
                }
                else
                {

                    sql += " " + $@"null,";
                }

                if (attendanceReasonModel.LastOut != null && attendanceReasonModel.LastOut != "")
                {
                    sql += " " + $@"'{attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd")} {attendanceReasonModel.LastOut}',";
                }
                else
                {

                    sql += " " + $@"null,";
                }


                if (attendanceReasonModel.WorkTime != null && attendanceReasonModel.WorkTime!= "")
                {
                    sql += " " + $@"'{attendanceReasonModel.WorkTime.Replace("j", ":").Replace("m", "").Replace("h", ":").Replace(" ","")}',";
                }
                else
                {

                    sql += " " + $@"null,";
                }

                if (attendanceReasonModel.Overtime != null && attendanceReasonModel.Overtime!= "")
                {
                    sql += " " + $@"'{attendanceReasonModel.Overtime.Replace("j", ":").Replace("m", "").Replace("h", ":").Replace(" ", "")}',";
                }
                else
                {

                    sql += " " + $@"null,";
                }


                if (attendanceReasonModel.OvertimeExtraStart!= null && attendanceReasonModel.OvertimeExtraStart!= "")
                {
                    sql += " " + $@"'{attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd")} {attendanceReasonModel.OvertimeExtraStart}',";
                }
                else
                {

                    sql += " " + $@"null,";
                }


                if (attendanceReasonModel.OvertimeExtraEnd != null && attendanceReasonModel.OvertimeExtraEnd != "")
                {
                    sql += " " + $@"'{attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd")} {attendanceReasonModel.OvertimeExtraEnd}',";
                }
                else
                {

                    sql += " " + $@"null,";
                }

                if (attendanceReasonModel.OvertimeExtra != null && attendanceReasonModel.OvertimeExtra!= "")
                {
                    sql += " " + $@"'{attendanceReasonModel.OvertimeExtra.Replace("j", ":").Replace("m", "").Replace("h", ":").Replace(" ", "")}',";
                }
                else
                {

                    sql += " " + $@"null,";
                }

                //sql += " " + $@"IIF('{firstIn}' <> '', '{firstIn}.ToString('yyyyMMdd HH:mm:ss')', null),";
                //sql += " " + $@"IIF('{lateness}' <> '', '{lateness.ToString()}', null),";
                //sql += " " + $@"IIF('{lastOut}' <> '', '{lastOut}.ToString('yyyyMMdd HH:mm:ss')', null),";
                //sql += " " + $@"IIF('{workTime}' <> '', '{workTime.ToString()}', null),";
                //sql += " " + $@"IIF('{overTime}' <> '', '{overTime.ToString()}', null),";
                //sql += " " + $@"IIF('{overTimeExtraStart}' <> '','{overTimeExtraStart}.ToString('yyyyMMdd HH:mm:ss')', null),";
                //sql += " " + $@"IIF('{overTimeExtraEnd}' <> '', '{overTimeExtraEnd}.ToString('yyyyMMdd HH:mm:ss')', null),";
                //sql += " " + $@"IIF('{overTimeExtra}' <> '', '{overTimeExtra.ToString()}', null),";

                //sql += " " + $@"'{lateness.ToString()}',";
                //sql += " " + $@"'{lastOut.ToString()}',";
                //sql += " " + $@"'{workTime.ToString()}',";
                //sql += " " + $@"'{overTime.ToString()}',";
                //sql += " " + $@"'{overTimeExtraStart.ToString()}',";
                //sql += " " + $@"'{overTimeExtraEnd.ToString()}',";
                //sql += " " + $@"'{overTimeExtra.ToString()}',";     

                sql += " " + $@"'{attendanceReasonModel.ReasonID}', '{attendanceReasonModel.Remark}', '{proof}', '{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}',";
                sql += " " + $@"'False', 'False', 'False')";
                //sql += " " + $@"'{SqlString.Null}', '{DBNull.Value}')";
                


                conn.Open();

                cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{attendanceReasonModel.NRIC}, {attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd HH:mm:ss")}, {attendanceReasonModel.AttendanceDay},";
                    logData += " " + $@"{attendanceReasonModel.ShiftID}, {attendanceReasonModel.AttendanceStatusID}, {attendanceReasonModel.AttendanceStatus},";
                    logData += " " + $@"{attendanceReasonModel.FirstIn}, {attendanceReasonModel.Lateness}, {attendanceReasonModel.LastOut}, {attendanceReasonModel.WorkTime},";
                    logData += " " + $@"{attendanceReasonModel.Overtime},";
                    //logData += " " + $@"{attendanceReasonModel.OvertimeStart}, {attendanceReasonModel.OvertimeEnd}, {attendanceReasonModel.Overtime},";
                    //logData += " " + $@"{attendanceReasonModel.OvertimeExtraStart}, {attendanceReasonModel.OvertimeExtraEnd}, {attendanceReasonModel.OvertimeExtra}, {attendanceReasonModel.TotalOvertime}, ";
                    logData += " " + $@"{attendanceReasonModel.OvertimeExtraStart}, {attendanceReasonModel.OvertimeExtraEnd}, {attendanceReasonModel.OvertimeExtra}, ";
                    logData += " " + $@"{attendanceReasonModel.ReasonID}, {attendanceReasonModel.ReasonName}, {attendanceReasonModel.Proof}, {attendanceReasonModel.Remark}, {DateTime.Now.ToString("yyyyMMdd HH:mm:ss")},";
                    logData += " " + $@"False, False, False)";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Attendance", $@"Submit; {logData}", DateTime.Now);

                    // Send Notification Email to every Approver
                    foreach (var row in approverUserList)
                    {
                        approverNRIC = "";
                        approverEmail = "";

                        approverNRIC = row.ApproverNRIC;

                        userModel = new UserModel();
                        userModel = userDBService.GetDataByID(approverNRIC);

                        string sender = ConfigurationManager.AppSettings["Sender"].ToString();
                        string smtpHost = ConfigurationManager.AppSettings["SmtpHost"].ToString();
                        string smtpPort = ConfigurationManager.AppSettings["SmtpPort"].ToString();
                        string loginName = ConfigurationManager.AppSettings["Username"].ToString();
                        string password = ConfigurationManager.AppSettings["Password"].ToString();
                        string enableSSL = ConfigurationManager.AppSettings["EnableSSL"].ToString();

                        //string isEmailNotificationEnabled = Session["IsEmailNotificationEnabled"].ToString();


                        if (userModel.Email != null)
                        {
                            approverEmail = userModel.Email;
                        }

                        if (approverEmail != "" && isEmailNotificationEnabled != "False")
                        {

                            MailMessage mail = new MailMessage();
                            mail.To.Add(approverEmail);
                            //mail.From = new MailAddress("yeoh.nick@gmail.com");
                            mail.From = new MailAddress(sender);
                            mail.Subject = "Pengesahan Dalam Sistem Kedatangan";

                            string body = "Tuan / Puan,";
                            body += "<br><br>";
                            body += $@"Pegawai {userName} telah menghantar alasan dalam sistem kedatangan <br>";
                            //body += $@"bagi kedatangan tarikh {attendanceReasonModel.AttendanceDate.ToString("yyyyMMdd HH:mm:ss")} <br>";
                            body += $@"dan memerlukan tuan/puan membuatkan pengesahan.<br><br>";
                            body += $@"Sekian Terima Kasih.";

                            mail.Body = body;
                            mail.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = smtpHost;
                            smtp.Port = Convert.ToInt32(smtpPort);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new System.Net.NetworkCredential(loginName, password); // Enter seders User name and password  
                            smtp.EnableSsl = Convert.ToBoolean(enableSSL);
                            smtp.Send(mail);

                        }

                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return status;

        }

        public List<AttendanceReasonModel> GetSubmittedReasonList( string attendanceReasonTableName, string NRIC)
        {

            List<AttendanceReasonModel> attendanceReasonList = new List<AttendanceReasonModel>();
            AttendanceReasonModel attendanceReasonModel;

            try
            {

                string sql = $@"SELECT AR.NRIC, AR.AttendanceDate,";                
                //sql += " " + AT.AttendanceDay + ",";
                sql += " " + $@"AR.AttendanceStatusID,";
                sql += " " + $@"CASE AR.AttendanceStatusID";
                sql += " " + $@"WHEN 'NOR' THEN '{MyTime.Resource.AttendanceStatus_NOR}'";
                sql += " " + $@"WHEN 'ABS' THEN '{MyTime.Resource.AttendanceStatus_ABS}'";
                sql += " " + $@"WHEN 'ICP' THEN '{MyTime.Resource.AttendanceStatus_ICP}'";
                sql += " " + $@"WHEN 'L/E' THEN '{MyTime.Resource.AttendanceStatus_L_N}'";
                sql += " " + $@"WHEN 'LIN' THEN '{MyTime.Resource.AttendanceStatus_LIN}'";
                sql += " " + $@"WHEN 'EOT' THEN '{MyTime.Resource.AttendanceStatus_EOT}'";
                sql += " " + $@"WHEN 'NWK' THEN '{MyTime.Resource.AttendanceStatus_NWK}'";
                sql += " " + $@"WHEN 'L/E' THEN '{MyTime.Resource.AttendanceStatus_HLY}'";
                sql += " " + $@"ELSE ''";
                sql += " " + $@"END AS AttendanceStatus,";
           
                sql += " " + $@"AR.FirstIn, AR.Lateness, AR.LastOut,";
                sql += " " + $@"AR.WorkTime, AR.OverTime,";
                sql += " " + $@"AR.ReasonID, R.ReasonName, R.IsForOnLeave,";
                sql += " " + $@"AR.Remark, AR.Proof,";
                sql += " " + $@"AR.IsApproved, AR.IsRejected, AR.IsRequestedToAmend,";
                sql += " " + $@"AR.ApproverComment, AR.ProcessedOn, AR.ProcessedBy";

                sql += " " + $@"FROM {attendanceReasonTableName} AR";
                sql += " " + $@"LEFT JOIN Reason R ON R.ReasonID = AR.ReasonID";                
                sql += " " + $@"WHERE AR.NRIC='{NRIC}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        attendanceReasonModel = new AttendanceReasonModel();

                        attendanceReasonModel.NRIC = dr["NRIC"].ToString();
                        attendanceReasonModel.AttendanceDate = Convert.ToDateTime(dr["AttendanceDate"]);
                        //attendanceReasonModel.AttendanceDay = dr["AttendanceDay"].ToString();

                        attendanceReasonModel.AttendanceDay = Convert.ToDateTime(dr["AttendanceDate"]).ToString("ddd", new System.Globalization.CultureInfo("ms-MY"));

                        attendanceReasonModel.AttendanceStatusID = dr["AttendanceStatusID"].ToString();
                        attendanceReasonModel.AttendanceStatus = dr["AttendanceStatus"].ToString();

                        if (!dr["FirstIn"].Equals(DBNull.Value))
                        {
                            attendanceReasonModel.FirstIn = dr["FirstIn"].ToString();
                        }

                        if (!dr["Lateness"].Equals(DBNull.Value))
                        {
                            attendanceReasonModel.Lateness = dr["Lateness"].ToString();
                        }

                        if (!dr["LastOut"].Equals(DBNull.Value))
                        {
                            attendanceReasonModel.LastOut = dr["LastOut"].ToString();
                        }

                        if (!dr["WorkTime"].Equals(DBNull.Value))
                        {
                            attendanceReasonModel.WorkTime = dr["WorkTime"].ToString();
                        }

                        if (!dr["Overtime"].Equals(DBNull.Value))
                        {
                            attendanceReasonModel.Overtime = dr["Overtime"].ToString();
                        }

                        attendanceReasonModel.ReasonID = dr["ReasonID"].ToString();
                        attendanceReasonModel.ReasonName = dr["ReasonName"].ToString();

                        attendanceReasonModel.IsForOnLeave = Convert.ToBoolean(dr["IsForOnLeave"]);

                        attendanceReasonModel.Remark = dr["Remark"].ToString();
                        attendanceReasonModel.Proof = dr["Proof"].ToString();

                        attendanceReasonModel.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                        attendanceReasonModel.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                        attendanceReasonModel.IsRequestedToAmend = Convert.ToBoolean(dr["IsRequestedToAmend"]);

                        attendanceReasonModel.ApproverComment = dr["ApproverComment"].ToString();

                        if (!dr["ProcessedOn"].Equals(DBNull.Value))
                        {
                            attendanceReasonModel.ProcessedOn = Convert.ToDateTime(dr["ProcessedOn"]);
                        }

                        if (!dr["ProcessedBy"].Equals(DBNull.Value))
                        {
                            attendanceReasonModel.ProcessedBy = dr["ProcessedBy"].ToString();
                        }

                        attendanceReasonList.Add(attendanceReasonModel);

                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return attendanceReasonList;
        }


        public List<DepartmentAttendanceDailyModel> GetDepartmentDailyAttendanceSummary(DateTime attendanceDate)
        {
            List<DepartmentAttendanceDailyModel> CRDepartmentAttendanceDailySummaryList = new List<DepartmentAttendanceDailyModel>();
            DepartmentAttendanceDailyModel cRDepartmentAttendanceDailySummaryModel;


            List<DepartmentModel> departmentList = new List<DepartmentModel>();

            string departmentID;
            string departmentName;

            List<UserModel> userList = new List<UserModel>();
            UserModel userModel = new UserModel();

            int userCount;
            string NRIC;
            string usrID;
            int accessRoleID;

            List<DeviceTransactionModel> deviceTransactionList;

            int inCount;
            int outCount;
            int attendCount;
            //decimal attendPercentage;

            departmentList = departmentDBService.ListDepartment();

            if (departmentList.Count > 0)
            {
                // Get Device Transaction
                string device = System.Web.Configuration.WebConfigurationManager.AppSettings["Device"];

                for (int i = 0; i < departmentList.Count; i++)
                {
                    departmentID = departmentList[i].DepartmentID;
                    departmentName = departmentList[i].DepartmentName;

                    userList = userDBService.ListUser().Where(u => u.DepartmentID == departmentID && ((u.IsResigned == true && u.ResignedOn > attendanceDate.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();

                    userCount = userList.Count();
                    inCount = 0;

                    outCount = 0;
                    attendCount = 0;

                    //attendPercentage = 0;

                    if (userCount > 0)
                    {
                        for (int j = 0; j < userCount; j++)
                        {

                            usrID = userList[j].USRID;
                            NRIC = userList[j].NRIC;
                            accessRoleID = userList[j].AccessRoleID;

                            deviceTransactionList = new List<DeviceTransactionModel>();

                            switch (device)
                            {
                                case "JohnsonControl":
                                    //deviceTransactionList = deviceDBService.GetJohnsonControlDeviceTrans(usrID, attendanceDate, attendanceDate, accessRoleID, false);
                                    deviceTransactionList = deviceDBService.GetJohnsonControlDeviceTrans(NRIC, attendanceDate, attendanceDate, accessRoleID, false);
                                    break;

                                case "Suprema":
                                    deviceTransactionList = deviceDBService.GetSupremaDeviceTrans(usrID, attendanceDate, attendanceDate, accessRoleID, false);
                                    break;
                            }

                            if (deviceTransactionList.Count > 0)
                            {
                                inCount += 1;

                                if (deviceTransactionList.Count > 1)
                                {
                                    outCount += 1;
                                }

                            }
                        }
                    }


                    cRDepartmentAttendanceDailySummaryModel = new DepartmentAttendanceDailyModel();

                    cRDepartmentAttendanceDailySummaryModel.AttendanceDate = attendanceDate;
                    cRDepartmentAttendanceDailySummaryModel.DepartmentID = departmentID;
                    cRDepartmentAttendanceDailySummaryModel.DepartmentName = departmentName;
                    cRDepartmentAttendanceDailySummaryModel.UserCount = userCount;
                    cRDepartmentAttendanceDailySummaryModel.InCount = inCount;
                    cRDepartmentAttendanceDailySummaryModel.OutCount = outCount;

                    switch (device)
                    {
                        case "JohnsonControl":
                            // Door Access
                            attendCount = inCount;
                            break;

                        case "Suprema":

                            // Fingerprint
                            attendCount = inCount - outCount;
                            break;
                    }

                    cRDepartmentAttendanceDailySummaryModel.AttendCount = attendCount;

                    //attendPercentage = (Convert.ToDecimal(attendCount) / Convert.ToDecimal(userCount)) * 100;

                    if (userCount > 0)
                    {
                        cRDepartmentAttendanceDailySummaryModel.AttendPercentage = (Convert.ToDecimal(attendCount) / Convert.ToDecimal(userCount)).ToString("0.0%");

                    }
                    else
                    {
                        cRDepartmentAttendanceDailySummaryModel.AttendPercentage = (0).ToString("0.0%");
                    }

                    CRDepartmentAttendanceDailySummaryList.Add(cRDepartmentAttendanceDailySummaryModel);

                }
            }

            return CRDepartmentAttendanceDailySummaryList;

        }


        public bool CheckServerTableExist(string tableName)
        {
            bool isTableFound = false;

            // 2022-03-27 : To check is the Table existed in SQL Server Database

            try
            {

                string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES";
                sql += " " + "WHERE TABLE_SCHEMA = 'dbo'";
                sql += " " + "AND TABLE_NAME = '" + tableName + "'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    isTableFound = true;

                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return isTableFound;

        }


    }
}