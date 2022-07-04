using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;


namespace MyTime.Services
{

    public class AttendanceCardDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);


        public List<AttendanceCardModel> GetAttendanceCardByID(string ID)
        {
            List<AttendanceCardModel> attendanceCardList = new List<AttendanceCardModel>();
            AttendanceCardModel attendanceCardModel = new AttendanceCardModel();

            string sql = $@"SELECT AttendanceMonth, DepartmentName, ";
            sql += " " + $@"AC.NRIC, U.UserName,";
            sql += " " + $@"LateInCount, EarlyOutCount,";
            sql += " " + $@"LateInEarlyOutCount, IncompleteCount,";
            sql += " " + $@"AbsentCount, AttendCount,";
            sql += " " + $@"OnLeaveCount, TotalAttendanceIssue,";
            sql += " " + $@"AttendanceCardStatus AC";
            sql += " " + $@"LEFT JOIN [User] U ON U.NRIC=AC.NRIC";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID=U.DepartmentID";
            sql += " " + $@"FROM AttendanceCard";
            sql += " " + $@"WHERE NRIC='{ID}'";
            sql += " " + $@"ORDER BY NRIC";


            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        attendanceCardModel = new AttendanceCardModel();

                        if (!dr["AttendanceMonth"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendanceMonth = dr["AttendanceMonth"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["NRIC"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.NRIC = dr["NRIC"].ToString();
                        }

                        if (!dr["LateInCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.LateInCount = Convert.ToInt32(dr["LateInCount"]);
                        }

                        if (!dr["EarlyOutCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.EarlyOutCount = Convert.ToInt32(dr["EarlyOutCount"]);
                        }

                        if (!dr["LateInEarlyOutCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.LateInEarlyOutCount = Convert.ToInt32(dr["LateInEarlyOutCount"]);
                        }

                        if (!dr["IncompleteCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.IncompleteCount = Convert.ToInt32(dr["IncompleteCount"]);
                        }

                        if (!dr["AbsentCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AbsentCount = Convert.ToInt32(dr["AbsentCount"]);
                        }

                        if (!dr["AttendCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendCount = Convert.ToInt32(dr["AttendCount"]);
                        }

                        if (!dr["OnLeaveCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.OnLeaveCount = Convert.ToInt32(dr["OnLeaveCount"]);
                        }

                        if (!dr["TotalAttendanceIssue"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.TotalAttendanceIssue = Convert.ToInt32(dr["TotalAttendanceIssue"]);
                        }

                        if (!dr["AttendanceCardStatus"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendanceCardStatus = dr["AttendanceCardStatus"].ToString();
                        }

                        attendanceCardList.Add(attendanceCardModel);

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

            return attendanceCardList;
        }

        public string GetMonthlyAttendanceCardStatusByID(string ID, DateTime startOn)
        {
            string attendanceCardStatus = "";

            //string sql = " " + $@"SELECT TOP 1 IIF(AttendanceCardStatus IS NULL, 'YL', AttendanceCardStatus) AS AttendanceCardStatus FROM [AttendanceCard]";
            //sql += " " + $@"WHERE [AttendanceCard].NRIC = {ID}";
            //sql += " " + $@"AND FORMAT(EffectiveOn, 'yyyyMM') <= '{startOn.ToString("yyyyMM")}'";
            //sql += " " + $@"ORDER BY EffectiveOn DESC";

            string sql = $@"SELECT TOP 1 AttendanceCardStatus FROM [AttendanceCard]";
            sql += " " + $@"WHERE [AttendanceCard].NRIC = {ID}";
            sql += " " + $@"AND CONVERT(INT,AttendanceMonth) < CONVERT(INT, {startOn.ToString("yyyyMM")})";
            sql += " " + $@"ORDER BY AttendanceMonth DESC";


            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);


                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        if (!dr["AttendanceCardStatus"].Equals(DBNull.Value))
                        {
                            attendanceCardStatus = dr["AttendanceCardStatus"].ToString();
                        }

                    }
                }
                else
                {
                    // Default to Yellow
                    attendanceCardStatus = "YL";
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


            return attendanceCardStatus;
        }


        public List<AttendanceCardModel> GetMonthlyAttendanceCardByAttendanceCardStatusAndDepartment(string attendanceMonth, string departmentID, string attendanceCardStatus)
        {
            List<AttendanceCardModel> attendanceCardList = new List<AttendanceCardModel>();
            AttendanceCardModel attendanceCardModel = new AttendanceCardModel();


            string sql = $@"SELECT AttendanceMonth, DepartmentName, ";
            sql += " " + $@"AC.NRIC, U.UserName,";
            sql += " " + $@"LateInCount, EarlyOutCount,";
            sql += " " + $@"LateInEarlyOutCount, IncompleteCount,";
            sql += " " + $@"AbsentCount, AttendCount,";
            sql += " " + $@"OnLeaveCount, TotalAttendanceIssue,";
            sql += " " + $@"AttendanceCardStatus";
            sql += " " + $@"FROM AttendanceCard AC";
            sql += " " + $@"LEFT JOIN [User] U ON U.NRIC=AC.NRIC";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID=U.DepartmentID";           
            sql += " " + $@"WHERE AttendanceMonth='{attendanceMonth}'";
            sql += " " + $@"AND AttendanceCardStatus='{attendanceCardStatus}'";
            sql += " " + $@"AND U.DepartmentID='{departmentID}'";
            sql += " " + $@"ORDER BY AttendanceMonth, DepartmentName, UserName";


            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        attendanceCardModel = new AttendanceCardModel();


                        if (!dr["AttendanceMonth"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendanceMonth = dr["AttendanceMonth"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["NRIC"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.NRIC = dr["NRIC"].ToString();
                        }

                        if (!dr["LateInCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.LateInCount = Convert.ToInt32(dr["LateInCount"]);
                        }

                        if (!dr["EarlyOutCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.EarlyOutCount = Convert.ToInt32(dr["EarlyOutCount"]);
                        }

                        if (!dr["LateInEarlyOutCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.LateInEarlyOutCount = Convert.ToInt32(dr["LateInEarlyOutCount"]);
                        }

                        if (!dr["IncompleteCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.IncompleteCount = Convert.ToInt32(dr["IncompleteCount"]);
                        }

                        if (!dr["AbsentCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AbsentCount = Convert.ToInt32(dr["AbsentCount"]);
                        }

                        if (!dr["AttendCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendCount = Convert.ToInt32(dr["AttendCount"]);
                        }

                        if (!dr["OnLeaveCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.OnLeaveCount = Convert.ToInt32(dr["OnLeaveCount"]);
                        }

                        if (!dr["TotalAttendanceIssue"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.TotalAttendanceIssue = Convert.ToInt32(dr["TotalAttendanceIssue"]);
                        }

                        if (!dr["AttendanceCardStatus"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendanceCardStatus = dr["AttendanceCardStatus"].ToString();
                        }

                        attendanceCardList.Add(attendanceCardModel);

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

            return attendanceCardList;
        }


    }
}