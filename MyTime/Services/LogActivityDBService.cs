using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using MyTime.ViewModels;
using System.Configuration;
using System.Data;

namespace MyTime.Services
{
    public class LogActivityDBService
    {

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        public bool LogActivity (string NRIC, string functionality, string activity, DateTime logDateTime)
        {
            Boolean isDone = false;
            Boolean isTableFound = false;

            try
            {
                string tableName = "ActivityLog_" + DateTime.Now.ToString("yyyyMM");

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

                if (! isTableFound == true )
                {
                    // Create New Activity Log Table

                    sql = "CREATE TABLE " + tableName;
                    sql += " " + "(LogID int IDENTITY(1,1), IP NVARCHAR(100), NRIC NVARCHAR(20),";
                    sql += " " + "Functionality NVARCHAR(50), Activity NVARCHAR(300), LogDateTime DATETIME";
                    sql += " " + "PRIMARY KEY (LogID))";

                    conn.Open();

                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();

                }

                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                sql = "INSERT INTO" + " " + tableName;
                sql += " " + $@"(IP, NRIC, Functionality, Activity, LogDateTime) VALUES";
                sql += " " + $@"('{ip}', '{NRIC}', '{functionality}', '{activity}', '{logDateTime.ToString("yyyyMMdd HH:mm:ss")}')";

                conn.Open();

                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

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


            return isDone;

        }

    }
}