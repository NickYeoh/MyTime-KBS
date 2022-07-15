using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;


namespace MyTime.Services
{
    public class DataMaintenanceDBService
    {

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();
        
        public Boolean CloseLastMonthAttendanceData(UserModel userModel)
        {
            DateTime currentMonth;
            DateTime lastMonth;
            Boolean isClosed = false;

            string year, month;
                     

            try
            {

                currentMonth = DateTime.Now;
                lastMonth = currentMonth.AddMonths(-1);

                year = lastMonth.ToString("yyyy");
                month = lastMonth.ToString("MM");

                conn.Open();

                SqlCommand cmd = new SqlCommand("spBEP1m", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1200;
                cmd.Parameters.AddWithValue("@Year", SqlDbType.NVarChar).Value = year;
                cmd.Parameters.AddWithValue("@Month", SqlDbType.NVarChar).Value = month;
                cmd.Parameters.AddWithValue("@Passcode", SqlDbType.NVarChar).Value = "DEV118";

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    isClosed = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Data Maintenance", $@"Manual Close Attendance Data; {userModel.NRIC}, Exec spBEP1m, {lastMonth.ToString("yyyyMM")} ", DateTime.Now);

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

            return isClosed;

        }

        public Boolean GenerateLastMonthAttendanceCardStatus(UserModel userModel)
        {
            DateTime currentMonth;
            DateTime lastMonth;
            Boolean isClosed = false;

            string year, month;


            try
            {

                currentMonth = DateTime.Now;
                lastMonth = currentMonth.AddMonths(-1);

                year = lastMonth.ToString("yyyy");
                month = lastMonth.ToString("MM");

                conn.Open();

                SqlCommand cmd = new SqlCommand("spBEP2m", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1200;
                cmd.Parameters.AddWithValue("@Year", SqlDbType.NVarChar).Value = year;
                cmd.Parameters.AddWithValue("@Month", SqlDbType.NVarChar).Value = month;
                cmd.Parameters.AddWithValue("@Passcode", SqlDbType.NVarChar).Value = "DEV118";

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    isClosed = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Data Maintenance", $@"Manual Generate Attendance Card Status; {userModel.NRIC}, Exec spBEP1m, {lastMonth.ToString("yyyyMM")} ", DateTime.Now);

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

            return isClosed;

        }

    }
}