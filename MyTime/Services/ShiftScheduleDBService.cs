using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class ShiftScheduleDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        public List<ShiftScheduleModel> ListShiftSchedule()
        {
            ShiftScheduleModel shiftScheduleModel;
            List<ShiftScheduleModel> dataList = new List<ShiftScheduleModel>();

            try
            {

                string sql = $@"SELECT U.NRIC, U.UserName, D.DepartmentID, D.DepartmentName,";
                sql += " " + $@"U.UnitID, UT.UnitName, SS.ShiftID, S.ShiftName, SS.EffectiveOn FROM ShiftSchedule SS";
                sql += " " + $@"LEFT JOIN [User] U ON U.NRIC = SS.NRIC";
                sql += " " + $@"LEFT JOIN Shift S ON S.ShiftID = SS.ShiftID";
                sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
                sql += " " + $@"LEFT JOIN Unit UT ON UT.UnitID = U.UnitID";
                sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
                sql += " " + $@"WHERE U.NRIC IS NOT NULL";
                sql += " " + $@"ORDER BY U.UserName, D.DepartmentName ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        shiftScheduleModel = new ShiftScheduleModel();

                        shiftScheduleModel.NRIC= dr["NRIC"].ToString();
                        shiftScheduleModel.UserName = dr["UserName"].ToString();
                        shiftScheduleModel.DepartmentID = dr["DepartmentID"].ToString();
                        shiftScheduleModel.DepartmentName = dr["DepartmentName"].ToString();

                        if (!dr["UnitID"].Equals(DBNull.Value))
                        {
                            shiftScheduleModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            shiftScheduleModel.UnitName = dr["UnitName"].ToString();
                        }

                        shiftScheduleModel.ShiftID = dr["ShiftID"].ToString();
                        shiftScheduleModel.ShiftName = dr["ShiftName"].ToString();

                        shiftScheduleModel.EffectiveOn = Convert.ToDateTime(dr["EffectiveOn"]);

                        dataList.Add(shiftScheduleModel);

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

   
        public bool Create(String selectedNRIC, DateTime effectiveOn, String shiftID )
        {

            string[] NRIC = selectedNRIC.Split('#');
            bool status = false;

            string sql;
            SqlCommand cmd;

            try
            {

                if (!NRIC.Length.Equals(0))
                {
                    conn.Open();

                    for (int i = 0; i < NRIC.Length; i++)
                    {

                        sql = $@"INSERT INTO ShiftSchedule";
                        sql += " " + $@"(NRIC, ShiftID, EffectiveOn) VALUES";
                        sql += " " + $@"('{NRIC[i]}', '{shiftID}','{effectiveOn.ToString("yyyyMMdd")}')";

                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        string logData = $@"{NRIC[i]}, {shiftID}, {effectiveOn.ToString("yyyyMMdd")}";

                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Shift Schedule", $@"Create; {logData}", DateTime.Now);

                    }

                    status = true;

                 

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

        public bool Delete(String NRIC)
        {

            bool status = false;

            try
            {

                string sql = $@"DELETE FROM ShiftSchedule";
                sql += " " + $@"WHERE NRIC='{NRIC}'";
         
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{NRIC}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Shift Schedule", $@"Delete; {logData}", DateTime.Now);

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

        public List<UserShiftModel> GetUserShift(String NRIC)
        {
            UserShiftModel userShiftModel;
            List<UserShiftModel> userShiftList = new List<UserShiftModel>();

            try
            {

                string sql = $@"SELECT* FROM ShiftSchedule SS";
                sql += " " + $@" INNER JOIN Shift S ON S.ShiftID = SS.ShiftID";
                sql += " " + $@"WHERE SS.NRIC='{NRIC}' ORDER BY EffectiveOn";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userShiftModel = new UserShiftModel();

                        userShiftModel.NRIC = dr["NRIC"].ToString();
                        userShiftModel.ShiftID = dr["ShiftID"].ToString();
                        userShiftModel.ShiftName = dr["ShiftName"].ToString();
                        userShiftModel.EffectiveOn = Convert.ToDateTime(dr["EffectiveOn"]);

                        userShiftModel.IsWorkDay1 = Convert.ToBoolean(dr["IsWorkDay1"]);
                        userShiftModel.TimeIn1 = dr["TimeIn1"].ToString();
                        userShiftModel.TimeOut1 = dr["TimeOut1"].ToString();
                        userShiftModel.FlexiTimeInterval1 = Convert.ToInt32(dr["FlexiTimeInterval1"]);

                        userShiftModel.IsWorkDay2 = Convert.ToBoolean(dr["IsWorkDay2"]);
                        userShiftModel.TimeIn2 = dr["TimeIn2"].ToString();
                        userShiftModel.TimeOut2 = dr["TimeOut2"].ToString();
                        userShiftModel.FlexiTimeInterval2 = Convert.ToInt32(dr["FlexiTimeInterval2"]);

                        userShiftModel.IsWorkDay3 = Convert.ToBoolean(dr["IsWorkDay3"]);
                        userShiftModel.TimeIn3 = dr["TimeIn3"].ToString();
                        userShiftModel.TimeOut3 = dr["TimeOut3"].ToString();
                        userShiftModel.FlexiTimeInterval3 = Convert.ToInt32(dr["FlexiTimeInterval3"]);

                        userShiftModel.IsWorkDay4 = Convert.ToBoolean(dr["IsWorkDay4"]);
                        userShiftModel.TimeIn4 = dr["TimeIn4"].ToString();
                        userShiftModel.TimeOut4 = dr["TimeOut4"].ToString();
                        userShiftModel.FlexiTimeInterval4 = Convert.ToInt32(dr["FlexiTimeInterval4"]);

                        userShiftModel.IsWorkDay5 = Convert.ToBoolean(dr["IsWorkDay5"]);
                        userShiftModel.TimeIn5 = dr["TimeIn5"].ToString();
                        userShiftModel.TimeOut5 = dr["TimeOut5"].ToString();
                        userShiftModel.FlexiTimeInterval5 = Convert.ToInt32(dr["FlexiTimeInterval5"]);

                        userShiftModel.IsWorkDay6 = Convert.ToBoolean(dr["IsWorkDay6"]);
                        userShiftModel.TimeIn6 = dr["TimeIn6"].ToString();
                        userShiftModel.TimeOut6 = dr["TimeOut6"].ToString();
                        userShiftModel.FlexiTimeInterval6 = Convert.ToInt32(dr["FlexiTimeInterval6"]);

                        userShiftModel.IsWorkDay7 = Convert.ToBoolean(dr["IsWorkDay7"]);
                        userShiftModel.TimeIn7 = dr["TimeIn7"].ToString();
                        userShiftModel.TimeOut7 = dr["TimeOut7"].ToString();
                        userShiftModel.FlexiTimeInterval7 = Convert.ToInt32(dr["FlexiTimeInterval7"]);

                        userShiftList.Add(userShiftModel);
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

            return userShiftList;

        }



    }
}