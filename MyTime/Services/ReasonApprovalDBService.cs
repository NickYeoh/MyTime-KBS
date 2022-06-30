using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class ReasonApprovalDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();
        SystemDBService systemDBService = new SystemDBService();

        public List<DepartmentModel> GetApproverUserDepartmentList(string approverNRIC)
        {
            List<DepartmentModel> dataList = new List<DepartmentModel>();
            DepartmentModel departmentModel;

            try
            {
                string sql = $@"SELECT DISTINCT D.DepartmentID, D.DepartmentName,D.IsActivated FROM ApproverUser AS ap";
                sql += " " + $@"LEFT JOIN [User] U ON U.NRIC = ap.UserNRIC";
                sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
                sql += " " + $@"WHERE IsActivated='True'";
                sql += " " + $@"GROUP BY D.DepartmentID, D.DepartmentName, D.IsActivated";
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        departmentModel = new DepartmentModel();

                        departmentModel.DepartmentID = dr["DepartmentID"].ToString();
                        departmentModel.DepartmentName = dr["DepartmentName"].ToString();
                        departmentModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

                        dataList.Add(departmentModel);

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

            return dataList;
        }


        public List<ReasonApprovalModel> GetReasonApprovalList(DateTime startOn, DateTime endOn, string approverNRIC, string departmentID)
        {

            List<ReasonApprovalModel> dataList = new List<ReasonApprovalModel>();
            ReasonApprovalModel reasonApprovalModel;

            SystemModel systemModel = new SystemModel();

            string attendanceReasonTableName = string.Format("AttendanceReason_{0}", startOn.ToString("yyyyMM"));

            bool isattendanceReasonFound = FindAttendanceReasonTable(attendanceReasonTableName);

            if (isattendanceReasonFound == true)
            {
                try
                {
                    // Approval Due Date = Submission Due Date
                    systemModel = systemDBService.GetData();
                    int reasonSubmissionPeriod = systemModel.ReasonSubmissionPeriod;
                    DateTime submissionDueDate = endOn.AddDays(reasonSubmissionPeriod);
                    bool isSubmissionDue;

                    TimeSpan workTime;
                    TimeSpan overTime;
                    TimeSpan overTimeExtra;

                    if (DateTime.Now.Date < submissionDueDate.Date)
                    {
                        isSubmissionDue = false;
                    }
                    else
                    {
                        isSubmissionDue = true;
                    }
                    //string.Format("{0} {1} {2} {3}", lateness.Hours, MyTime.Resource.Hour, lateness.Minutes, MyTime.Resource.Minute);
                    string sql = $@"SELECT u.UserName, ap.UserNRIC, ar.AttendanceDate,";
                    sql += " " + $@"ar.AttendanceStatusID,";
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

                    sql += " " + $@"ar.FirstIn, ar.Lateness, ar.LastOut,";
                    sql += " " + $@"ar.WorkTime, ar.OverTime,";
                    sql += " " + $@"ar.OvertimeExtraStart, ar.OvertimeExtraEnd, ar.OvertimeExtra,";
                    sql += " " + $@"ar.ReasonID, r.ReasonName, ar.Remark, ar.Proof,";
                    sql += " " + $@"ar.IsApproved, ar.IsRejected, ar.IsRequestedToAmend, ar.ApproverComment, ar.SubmittedOn";
                    sql += " " + $@"FROM ApproverUser ap";
                    sql += " " + $@"INNER JOIN {attendanceReasonTableName} ar";
                    sql += " " + $@"ON ar.NRIC = ap.UserNRIC";
                    sql += " " + $@"LEFT JOIN [User] u on u.NRIC = ap.UserNRIC";
                    sql += " " + $@"LEFT JOIN Reason r on r.ReasonID = ar.ReasonID";
                    sql += " " + $@"WHERE ap.ApproverNRIC='{approverNRIC}'";
                    sql += " " + $@"AND u.DepartmentID ='{departmentID}'";
                    sql += " " + $@"ORDER BY AttendanceDate, u.UserName";

                    //string sql = $@"SELECT u.UserName, ap.UserNRIC, ar.AttendanceDate, ar.AttendanceDay,";
                    //sql += " " + $@"ar.AttendanceStatusID, ar.AttendanceStatus, ar.FirstIn, ar.Lateness, ar.LastOut,";
                    //sql += " " + $@"ar.WorkTime, ar.OvertimeStart, ar.OvertimeEnd, ar.Overtime,";
                    //sql += " " + $@"ar.OvertimeExtraStart, ar.OvertimeExtraEnd, ar.OvertimeExtra, ar.TotalOvertime,";
                    //sql += " " + $@"ar.ReasonID, r.ReasonName, ar.Remark, ar.Proof,";
                    //sql += " " + $@"ar.IsApproved, ar.IsRejected, ar.IsRequestedToAmend, ar.ApproverComment, ar.SubmittedOn";
                    //sql += " " + $@"FROM ApproverUser ap";
                    //sql += " " + $@"INNER JOIN {attendanceReasonTableName} ar";
                    //sql += " " + $@"ON ar.NRIC = ap.UserNRIC";
                    //sql += " " + $@"LEFT JOIN [User] u on u.NRIC = ap.UserNRIC";
                    //sql += " " + $@"LEFT JOIN Reason r on r.ReasonID = ar.ReasonID";
                    //sql += " " + $@"WHERE ap.ApproverNRIC='{approverNRIC}'";
                    //sql += " " + $@"AND u.DepartmentID ='{departmentID}'";
                    //sql += " " + $@"ORDER BY AttendanceDate, u.UserName";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {

                            reasonApprovalModel = new ReasonApprovalModel();

                            reasonApprovalModel.UserName = dr["UserName"].ToString();
                            reasonApprovalModel.NRIC = dr["UserNRIC"].ToString();
                            reasonApprovalModel.AttendanceDate = Convert.ToDateTime(dr["AttendanceDate"]);
                        
                            reasonApprovalModel.AttendanceDay = Convert.ToDateTime(dr["AttendanceDate"]).ToString("ddd", new System.Globalization.CultureInfo("ms-MY"));
                            reasonApprovalModel.AttendanceStatusID = dr["AttendanceStatusID"].ToString();
                            reasonApprovalModel.AttendanceStatus = dr["AttendanceStatus"].ToString();
                           
                            if (dr["FirstIn"] != DBNull.Value)
                            {
                                reasonApprovalModel.FirstIn = Convert.ToDateTime(dr["FirstIn"]).ToString("HH:mm");
                            }
                            else
                            {
                                reasonApprovalModel.FirstIn = "";
                            }
                            
                            reasonApprovalModel.Lateness = dr["Lateness"].ToString();

                            if (dr["LastOut"] != DBNull.Value)
                            {
                                reasonApprovalModel.LastOut = Convert.ToDateTime(dr["LastOut"]).ToString("HH:mm");
                            }
                            else
                            {
                                reasonApprovalModel.LastOut = "";
                            }

                           if (dr["WorkTime"] != DBNull.Value)
                            {

                                workTime = (TimeSpan)(dr["WorkTime"]);
                                reasonApprovalModel.WorkTime = string.Format("{0} {1} {2} {3}", workTime.Hours, MyTime.Resource.Hour, workTime.Minutes, MyTime.Resource.Minute);
                               
                            }
                           else
                            {
                                reasonApprovalModel.WorkTime = "";
                            }


                            if (dr["OverTime"] != DBNull.Value)
                            {
                                overTime = (TimeSpan)(dr["OverTime"]);
                                reasonApprovalModel.Overtime = string.Format("{0} {1} {2} {3}", overTime.Hours, MyTime.Resource.Hour, overTime.Minutes, MyTime.Resource.Minute);
                            }
                            else
                            {
                                reasonApprovalModel.Overtime = "";
                            }

                            //reasonApprovalModel.OvertimeStart = dr["OvertimeStart"].ToString();
                            //reasonApprovalModel.OvertimeEnd = dr["OvertimeEnd"].ToString();


                            if (dr["OvertimeExtraStart"] != DBNull.Value)
                            {
                                reasonApprovalModel.OvertimeExtraStart = Convert.ToDateTime(dr["OvertimeExtraStart"]).ToString("HH:mm");
                            }
                            else
                            {
                                reasonApprovalModel.OvertimeExtraStart= "";
                            }


                            if (dr["OvertimeExtraEnd"] != DBNull.Value)
                            {
                                reasonApprovalModel.OvertimeExtraEnd = Convert.ToDateTime(dr["OvertimeExtraEnd"]).ToString("HH:mm");
                            }
                            else
                            {
                                reasonApprovalModel.OvertimeExtraEnd = "";
                            }

                            if (dr["OverTimeExtra"] != DBNull.Value)
                            {
                                overTimeExtra = (TimeSpan)(dr["OverTimeExtra"]);
                                reasonApprovalModel.OvertimeExtra = string.Format("{0} {1} {2} {3}", overTimeExtra.Hours, MyTime.Resource.Hour, overTimeExtra.Minutes, MyTime.Resource.Minute);
                            }
                            else
                            {
                                reasonApprovalModel.OvertimeExtra = "";
                            }


                            //reasonApprovalModel.TotalOvertime = dr["TotalOvertime"].ToString();

                            reasonApprovalModel.ReasonID = dr["ReasonID"].ToString();                            
                            reasonApprovalModel.ReasonName = dr["ReasonName"].ToString();

                            reasonApprovalModel.Remark = dr["Remark"].ToString();
                            reasonApprovalModel.Proof = dr["Proof"].ToString();
                            reasonApprovalModel.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                            reasonApprovalModel.IsRejected = Convert.ToBoolean(dr["IsRejected"]);
                            reasonApprovalModel.IsRequestedToAmend = Convert.ToBoolean(dr["IsRequestedToAmend"]);
                            reasonApprovalModel.ApproverComment= dr["ApproverComment"].ToString();
                            reasonApprovalModel.ReceivedOn = Convert.ToDateTime(dr["SubmittedOn"]);
                            reasonApprovalModel.ApprovalDueDate = submissionDueDate;
                            reasonApprovalModel.IsApprovalDue = isSubmissionDue;

                            dataList.Add(reasonApprovalModel);

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
            }

            return dataList;
        }

        public bool FindAttendanceReasonTable(string tableName)
        {

            Boolean isTableFound = false;

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

                conn.Close();


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

        public bool ApproveReason(string approverNRIC, ReasonApprovalModel reasonApprovalModel)
        {

            bool status = false;

            string attendanceReasonTableName = string.Format("AttendanceReason_{0}", reasonApprovalModel.AttendanceDate.ToString("yyyyMM"));

            bool isattendanceReasonFound = FindAttendanceReasonTable(attendanceReasonTableName);

            if (isattendanceReasonFound == true)
            {

                try
                {
                    string sql = $@"UPDATE {attendanceReasonTableName} SET IsApproved='True',";
                    sql += " " + $@"ProcessedBy='{approverNRIC}', ProcessedOn='{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}'";
                    sql += " " + $@"WHERE CONVERT(VARCHAR, AttendanceDate, 112)='{reasonApprovalModel.AttendanceDate.ToString("yyyyMMdd")}'";
                    sql += " " + $@"AND NRIC='{reasonApprovalModel.NRIC}'";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;

                        string logData = $@"{reasonApprovalModel.AttendanceDate.ToString("yyyyMMdd")}, {reasonApprovalModel.NRIC},";
                        logData += " " + $@"{reasonApprovalModel.ReasonID}, {reasonApprovalModel.ReasonName}, {reasonApprovalModel.Remark}";


                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Reason Approval", $@"Approve; {logData}", DateTime.Now);

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

            }

            return status;

        }

        public bool RejectReason(string approverNRIC, ReasonApprovalModel reasonApprovalModel)
        {

            bool status = false;

            string attendanceReasonTableName = string.Format("AttendanceReason_{0}", reasonApprovalModel.AttendanceDate.ToString("yyyyMM"));

            bool isattendanceReasonFound = FindAttendanceReasonTable(attendanceReasonTableName);

            if (isattendanceReasonFound == true)
            {

                try
                {

                    string sql = $@"UPDATE {attendanceReasonTableName} SET IsRejected='True',";
                    sql += " " + $@"ApproverComment='{reasonApprovalModel.ApproverComment}',";
                    sql += " " + $@"ProcessedBy='{approverNRIC}', ProcessedOn='{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}'";
                    sql += " " + $@"WHERE CONVERT(VARCHAR, AttendanceDate, 112)='{reasonApprovalModel.AttendanceDate.ToString("yyyyMMdd")}'";
                    sql += " " + $@"AND NRIC='{reasonApprovalModel.NRIC}'";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;

                        string logData = $@"{reasonApprovalModel.AttendanceDate.ToString("yyyyMMdd")}, {reasonApprovalModel.NRIC},";
                        logData += " " + $@"{reasonApprovalModel.ReasonID}, {reasonApprovalModel.ReasonName}, {reasonApprovalModel.Remark}";


                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Reason Approval", $@"Reject; {logData}", DateTime.Now);

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

            }

            return status;

        }

        public bool RequestToAmendReason(string approverNRIC, ReasonApprovalModel reasonApprovalModel)
        {

            bool status = false;

            string attendanceReasonTableName = string.Format("AttendanceReason_{0}", reasonApprovalModel.AttendanceDate.ToString("yyyyMM"));

            bool isattendanceReasonFound = FindAttendanceReasonTable(attendanceReasonTableName);

            if (isattendanceReasonFound == true)
            {

                try
                {

                    string sql = $@"UPDATE {attendanceReasonTableName} SET IsRequestedToAmend='True',";
                    sql += " " + $@"ApproverComment='{reasonApprovalModel.ApproverComment}',";
                    sql += " " + $@"ProcessedBy='{approverNRIC}', ProcessedOn='{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}'";
                    sql += " " + $@"WHERE CONVERT(VARCHAR, AttendanceDate, 112)='{reasonApprovalModel.AttendanceDate.ToString("yyyyMMdd")}'";
                    sql += " " + $@"AND NRIC='{reasonApprovalModel.NRIC}'";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;

                        string logData = $@"{reasonApprovalModel.AttendanceDate.ToString("yyyyMMdd")}, {reasonApprovalModel.NRIC},";
                        logData += " " + $@"{reasonApprovalModel.ReasonID}, {reasonApprovalModel.ReasonName}, {reasonApprovalModel.Remark}";


                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Reason Approval", $@"Request To Amend; {logData}", DateTime.Now);


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

            }

            return status;

        }


    }
}