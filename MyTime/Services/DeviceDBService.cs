using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Linq;

namespace MyTime.Services
{
    public class DeviceDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["DeviceDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        private readonly static string connStrMT = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection connMT = new SqlConnection(connStrMT);

        //AccessRoleDBService accessRoleDBService = new AccessRoleDBService();

        public List<DeviceModel> ListJohnsonControlDevice()
        {

            List<DeviceModel> deviceList = new List<DeviceModel>();

            SqlCommand cmd;
            SqlDataReader dr;

            DeviceModel deviceModel;

            string sql = $@"SELECT * FROM terminal";
            sql += " " + $@"WHERE tp_enb_dis = '1'";

            conn.Open();

            cmd = new SqlCommand(sql, conn);
            dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {

                    deviceModel = new DeviceModel();

                    deviceModel.DeviceID = Convert.ToInt32(dr["tp_term_id"]);
                    deviceModel.DeviceName = dr["tp_term_name"].ToString();

                    deviceList.Add(deviceModel);

                }
            }

            conn.Close();

            return deviceList;

        }

        public List<DeviceModel> ListSupremaDevice()
        {

            List<DeviceModel> deviceList = new List<DeviceModel>();

            SqlCommand cmd;
            SqlDataReader dr;

            DeviceModel deviceModel;

            string sql = $@"SELECT * FROM T_Dev";
            sql += " " + $@"WHERE Del = '0'";

            conn.Open();

            cmd = new SqlCommand(sql, conn);
            dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {

                    deviceModel = new DeviceModel();

                    deviceModel.DeviceID = Convert.ToInt32(dr["DevID"]);
                    deviceModel.DeviceName = dr["NM"].ToString();

                    deviceList.Add(deviceModel);

                }
            }

            conn.Close();

            return deviceList;

        }

        public List<DeviceTransactionModel> GetJohnsonControlDeviceTrans(string usrID, DateTime startOn, DateTime endOn, int accessRoleID, bool isFromOvertimeDevice)
        {

            List<DeviceTransactionModel> deviceTrans = new List<DeviceTransactionModel>();
            List<AccessRoleDeviceModel> accessRoleDeviceList = new List<AccessRoleDeviceModel>();

            DeviceTransactionModel deviceTransactionModel;

            string sql;
            SqlCommand cmd;
            SqlDataReader dr;

            DeviceModel deviceModel;
            List<DeviceModel> deviceList = new List<DeviceModel>();
            
            try
            {
                // From normal device
                sql = "SELECT DeviceID FROM AccessRoleDevice WHERE AccessRoleID='" + accessRoleID.ToString() + "'";
                sql += " " + $@"AND IsOvertimeExtraDevice='{isFromOvertimeDevice}'";

                connMT.Open();

                cmd = new SqlCommand(sql, connMT);
                dr = cmd.ExecuteReader();
             
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        deviceModel = new DeviceModel();
                      
                        deviceModel.DeviceID = Convert.ToInt32(dr["DeviceID"]);

                        deviceList.Add(deviceModel);

                    }

                }
                connMT.Close();


                //sql = $@"SELECT t.tp_term_id ,[x_term_name],";
                //sql += " " + $@"ch.c_id AS USRID ,x.x_timestamp AS TRDateTime";
                //sql += " " + $@"FROM[Pegasys].[dbo].[xaction] x";
                //sql += " " + $@"LEFT JOIN[badge] b ON b.[b_number_str] = x.[x_badge_number]";
                //sql += " " + $@"LEFT JOIN[cardholder] ch ON ch.c_id = b.[b_cardholder_id]";
                //sql += " " + $@" LEFT JOIN[terminal] t ON t.tp_term_name = x.x_term_name";
                //sql += " " + $@"WHERE x_hist_type = '68' AND";

                // 2021-11-19
                //Truncate the seconds and miniseconds for x.x_timestamp to get accurate in, out and work hour

                sql = $@"SELECT t.tp_term_id ,[x_term_name],";
                sql += " " + $@"LTRIM(RTRIM(REPLACE(x_cardholder_nick_name, '-',''))) AS USRID , CAST(CONVERT(varchar, x.x_timestamp, 100) as datetime) AS TRDateTime";
                sql += " " + $@"FROM[Pegasys].[dbo].[xaction] x";
                sql += " " + $@"LEFT JOIN[terminal] t ON t.tp_term_name = x.x_term_name";
                sql += " " + $@"WHERE x_hist_type = '68' AND";

                if (startOn.Date.Equals(endOn.Date))
                {
                    // Daily
                    sql += " " + $@"(convert(varchar, x_timestamp, 112) = '{startOn.ToString("yyyyMMdd")}')";
                }
                else
                {
                    // Monthly
                    sql += " " + $@"(x_timestamp BETWEEN '{startOn.ToString("yyyyMMdd")}' AND '{endOn.ToString("yyyyMMdd")}')";
                }

                // Don't remove
                //sql += " " + $@"AND c_id = '{usrID}'";

                // 2021-11-19
                // Use NRIC to retrieve transaction
                sql += " " + $@"AND LTRIM(RTRIM(REPLACE(x_cardholder_nick_name, '-',''))) ='{usrID}'";
               
                sql += " " + $@"AND  tp_term_id IN (";

                foreach (var row  in deviceList)
                {
                  sql += "'" + row.DeviceID.ToString() + "',";
                }

                sql += "'')";

                sql += " " + $@"ORDER BY x_timestamp";

                conn.Open();

                cmd = new SqlCommand(sql, conn);
                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        deviceTransactionModel = new DeviceTransactionModel();

                        deviceTransactionModel.TRDateTime = Convert.ToDateTime(dr["TRDateTime"]);
                        deviceTransactionModel.USRID = dr["USRID"].ToString();

                        deviceTrans.Add(deviceTransactionModel);

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

            return deviceTrans;

        }

    
        public List<DeviceTransactionModel> GetSupremaDeviceTrans(string usrID, DateTime startOn, DateTime endOn, int accessRoleID)
        {
            List<DeviceTransactionModel> deviceTrans = new List<DeviceTransactionModel>();
            DeviceTransactionModel deviceTransactionModel;

            bool isPreviousMonthDeviceTableFound = false;
            bool isCurrentMonthDeviceTableFound = false;

            string previousMonthTableName = "";
            string currentMonthTableName = "";
            string sql = "";

            SqlCommand cmd;
            SqlDataReader dr;

            try
            {

                for (int i = 0; i <= 1; i++)
                {
                    switch (i)
                    {
                        case 0:
                            // Previous Month
                            previousMonthTableName = "T_LG" + startOn.AddMonths(-1).ToString("yyyyMM");

                            sql = $@"SELECT * FROM INFORMATION_SCHEMA.TABLES";
                            sql += " " + $@"WHERE TABLE_SCHEMA ='dbo'";
                            sql += " " + $@"AND TABLE_NAME='{ previousMonthTableName}'";

                            break;
                        case 1:
                            // Current Month
                            currentMonthTableName = "T_LG" + startOn.ToString("yyyyMM");

                            sql = $@"SELECT * FROM INFORMATION_SCHEMA.TABLES";
                            sql += " " + $@"WHERE TABLE_SCHEMA ='dbo'";
                            sql += " " + $@"AND TABLE_NAME='{currentMonthTableName}'";

                            break;
                    }


                    conn.Open();

                    cmd = new SqlCommand(sql, conn);
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {

                        switch (i)
                        {
                            case 0:
                                // Previous Month
                                isPreviousMonthDeviceTableFound = true;
                                break;
                            case 1:
                                // Current Month
                                isCurrentMonthDeviceTableFound = true;
                                break;
                        }

                    }

                    conn.Close();
                };
                
                //2021-09-25 : Pls replace the SRVDT with the following SQL script

                //CONVERT(TIME, DATEADD(s, [DEVDT] , '19700101 08:00:00:000')
                //CONVERT(DATE, DATEADD(s, [DEVDT], '19700101 08:00:00:000')

                if (isPreviousMonthDeviceTableFound == true && isCurrentMonthDeviceTableFound == true)
                {
                    //sql = "(SELECT CONVERT(DATE, [SRVDT]) AS TRDate";
                    //sql += " " + $@", CONVERT(TIME,[SRVDT]) AS TRTIME";
                    sql = "(SELECT [SRVDT] AS TRDateTime";
                    sql += " " + $@",[USRID], EVTLGUID";
                    sql += " " + $@"FROM {previousMonthTableName}";
                    sql += " " + $@"WHERE USRID='{usrID}'";

                    if (startOn.Date.Equals(endOn.Date))
                    {
                        sql += " " + $@"(convert(varchar, [SRVDT] , 112) = '{startOn.ToString("yyyyMMdd")}')";
                    }
                    else
                    {
                        sql += " " + $@"AND (CONVERT(DATE, [SRVDT]) BETWEEN '{startOn.ToString("yyyyMMdd")}'";
                        sql += " " + $@"AND '{endOn.ToString("yyyyMMdd")}')";
                    }

                    sql += " " + $@"AND (EVT='4865' OR EVT='4102' OR EVT='4097'))";
                    sql += " " + $@"UNION";

                    //sql += " " + $@"(SELECT CONVERT(DATE, [SRVDT]) AS TRDate";
                    //sql += " " + $@", CONVERT(TIME,[SRVDT]) AS TRTIME";
                    sql += " " + $@"(SELECT [SRVDT] AS TRDateTime";
                    sql += " " + $@",[USRID], EVTLGUID";
                    sql += " " + $@"FROM {currentMonthTableName}";
                    sql += " " + $@"WHERE USRID='{usrID}'";

                    if (startOn.Date.Equals(endOn.Date))
                    {
                        sql += " " + $@"(convert(varchar, [SRVDT] , 112) = '{startOn.ToString("yyyyMMdd")}')";
                    }
                    else
                    {
                        sql += " " + $@"AND (CONVERT(DATE, [SRVDT]) BETWEEN '{startOn.ToString("yyyyMMdd")}'";
                        sql += " " + $@"AND '{endOn.ToString("yyyyMMdd")}')";

                    }

                    sql += " " + $@"AND (EVT='4865' OR EVT='4102' OR EVT='4097'))";
                    sql += " " + $@"ORDER BY TRDateTime";
                    //sql += " " + $@"ORDER BY TRDate, TRTIME";

                }
                else if (isCurrentMonthDeviceTableFound == true)
                {
                    //sql += " " + $@"SELECT CONVERT(DATE, [SRVDT]) AS TRDate";
                    //sql += " " + $@", CONVERT(TIME,[SRVDT]) AS TRTIME";
                    sql = "(SELECT [SRVDT] AS TRDateTime";
                    sql += " " + $@",[USRID], EVTLGUID";
                    sql += " " + $@"FROM {currentMonthTableName}";
                    sql += " " + $@"WHERE USRID='{usrID}'";
                    sql += " " + $@"AND (CONVERT(DATE, [SRVDT]) BETWEEN '{startOn.ToString("yyyyMMdd")}'";
                    sql += " " + $@"AND '{endOn.ToString("yyyyMMdd")}')";
                    sql += " " + $@"AND (EVT='4865' OR EVT='4102' OR EVT='4097')";
                    sql += " " + $@"ORDER BY TRDateTime";
                    //sql += " " + $@"ORDER BY TRDate, TRTIME";

                }

                conn.Open();

                cmd = new SqlCommand(sql, conn);
                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        deviceTransactionModel = new DeviceTransactionModel();

                        deviceTransactionModel.TRDateTime = Convert.ToDateTime(dr["TRDateTime"]);
                        deviceTransactionModel.USRID = dr["USRID"].ToString();

                        deviceTrans.Add(deviceTransactionModel);

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

            return deviceTrans;

        }


    }
}