using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace MyTime.Services
{
    public class ShiftDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();
        ShiftScheduleDBService shiftScheduleDBService = new ShiftScheduleDBService();

        public List<ShiftModel> ListShift()
        {
            ShiftModel shiftModel;
            List<ShiftModel> dataList = new List<ShiftModel>();

            try
            {

                string sql = $@"SELECT * FROM Shift ORDER BY ShiftID ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        shiftModel = new ShiftModel();

                        shiftModel.ShiftID = dr["ShiftID"].ToString();
                        shiftModel.ShiftName = dr["ShiftName"].ToString();

                        shiftModel.IsWorkDay1 = Convert.ToBoolean(dr["IsWorkDay1"]);
                        shiftModel.TimeIn1 = Convert.ToString(dr["TimeIn1"]);
                        shiftModel.TimeOut1 = Convert.ToString(dr["TimeOut1"]);
                        shiftModel.FlexiTimeInterval1 = Convert.ToInt32(dr["FlexiTimeInterval1"]);

                        shiftModel.IsWorkDay2 = Convert.ToBoolean(dr["IsWorkDay2"]);
                        shiftModel.TimeIn2 = Convert.ToString(dr["TimeIn2"]);
                        shiftModel.TimeOut2 = Convert.ToString(dr["TimeOut2"]);
                        shiftModel.FlexiTimeInterval2 = Convert.ToInt32(dr["FlexiTimeInterval2"]);

                        shiftModel.IsWorkDay3 = Convert.ToBoolean(dr["IsWorkDay3"]);
                        shiftModel.TimeIn3 = Convert.ToString(dr["TimeIn3"]);
                        shiftModel.TimeOut3 = Convert.ToString(dr["TimeOut3"]);
                        shiftModel.FlexiTimeInterval3 = Convert.ToInt32(dr["FlexiTimeInterval3"]);

                        shiftModel.IsWorkDay4 = Convert.ToBoolean(dr["IsWorkDay4"]);
                        shiftModel.TimeIn4 = Convert.ToString(dr["TimeIn4"]);
                        shiftModel.TimeOut4 = Convert.ToString(dr["TimeOut4"]);
                        shiftModel.FlexiTimeInterval4 = Convert.ToInt32(dr["FlexiTimeInterval4"]);

                        shiftModel.IsWorkDay5 = Convert.ToBoolean(dr["IsWorkDay5"]);
                        shiftModel.TimeIn5 = Convert.ToString(dr["TimeIn5"]);
                        shiftModel.TimeOut5 = Convert.ToString(dr["TimeOut5"]);
                        shiftModel.FlexiTimeInterval5 = Convert.ToInt32(dr["FlexiTimeInterval5"]);

                        shiftModel.IsWorkDay6 = Convert.ToBoolean(dr["IsWorkDay6"]);
                        shiftModel.TimeIn6 = Convert.ToString(dr["TimeIn6"]);
                        shiftModel.TimeOut6 = Convert.ToString(dr["TimeOut6"]);
                        shiftModel.FlexiTimeInterval6 = Convert.ToInt32(dr["FlexiTimeInterval6"]);

                        shiftModel.IsWorkDay7 = Convert.ToBoolean(dr["IsWorkDay7"]);
                        shiftModel.TimeIn7 = Convert.ToString(dr["TimeIn7"]);
                        shiftModel.TimeOut7 = Convert.ToString(dr["TimeOut7"]);
                        shiftModel.FlexiTimeInterval7 = Convert.ToInt32(dr["FlexiTimeInterval7"]);

                        shiftModel.IsOverNightShift = Convert.ToBoolean(dr["IsOverNightShift"]);

                        shiftModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

                        dataList.Add(shiftModel);

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

        public Boolean CheckDuplicateID(string ID)
        {
            Boolean isDuplicated = false;

            try
            {


                string sql = $@"SELECT * FROM Shift WHERE ShiftID='{ID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    isDuplicated = true;

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

            return isDuplicated;

        }

        public Boolean CheckDuplicateName(string Name)
        {
            Boolean isDuplicated = false;

            try
            {

                string sql = $@"SELECT * FROM Shift WHERE ShiftName='{Name}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    isDuplicated = true;

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

            return isDuplicated;

        }

        public ShiftModel GetDataByID(string ID)
        {
            ShiftModel shiftModel;

            try
            {
                shiftModel = new ShiftModel();

                string sql = $@"SELECT * FROM Shift WHERE ShiftID='{ID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    shiftModel.ShiftID = dr["ShiftID"].ToString();
                    shiftModel.ShiftName = dr["ShiftName"].ToString();


                    shiftModel.IsWorkDay1 = Convert.ToBoolean(dr["IsWorkDay1"]);
                    shiftModel.TimeIn1 = Convert.ToDateTime(dr["TimeIn1"]).ToString("HH:mm");
                    shiftModel.TimeOut1 = Convert.ToDateTime(dr["TimeOut1"]).ToString("HH:mm");
                    shiftModel.FlexiTimeInterval1 = Convert.ToInt32(dr["FlexiTimeInterval1"]);

                    shiftModel.IsWorkDay2 = Convert.ToBoolean(dr["IsWorkDay2"]);
                    shiftModel.TimeIn2 = Convert.ToDateTime(dr["TimeIn2"]).ToString("HH:mm");
                    shiftModel.TimeOut2 = Convert.ToDateTime(dr["TimeOut2"]).ToString("HH:mm");
                    shiftModel.FlexiTimeInterval2 = Convert.ToInt32(dr["FlexiTimeInterval2"]);

                    shiftModel.IsWorkDay3 = Convert.ToBoolean(dr["IsWorkDay3"]);
                    shiftModel.TimeIn3 = Convert.ToDateTime(dr["TimeIn3"]).ToString("HH:mm");
                    shiftModel.TimeOut3 = Convert.ToDateTime(dr["TimeOut3"]).ToString("HH:mm");
                    shiftModel.FlexiTimeInterval3 = Convert.ToInt32(dr["FlexiTimeInterval3"]);

                    shiftModel.IsWorkDay4 = Convert.ToBoolean(dr["IsWorkDay4"]);
                    shiftModel.TimeIn4 = Convert.ToDateTime(dr["TimeIn4"]).ToString("HH:mm");
                    shiftModel.TimeOut4 = Convert.ToDateTime(dr["TimeOut4"]).ToString("HH:mm");
                    shiftModel.FlexiTimeInterval4 = Convert.ToInt32(dr["FlexiTimeInterval4"]);

                    shiftModel.IsWorkDay5 = Convert.ToBoolean(dr["IsWorkDay5"]);
                    shiftModel.TimeIn5 = Convert.ToDateTime(dr["TimeIn5"]).ToString("HH:mm");
                    shiftModel.TimeOut5 = Convert.ToDateTime(dr["TimeOut5"]).ToString("HH:mm");
                    shiftModel.FlexiTimeInterval5 = Convert.ToInt32(dr["FlexiTimeInterval5"]);

                    shiftModel.IsWorkDay6 = Convert.ToBoolean(dr["IsWorkDay6"]);
                    shiftModel.TimeIn6 = Convert.ToDateTime(dr["TimeIn6"]).ToString("HH:mm");
                    shiftModel.TimeOut6 = Convert.ToDateTime(dr["TimeOut6"]).ToString("HH:mm");
                    shiftModel.FlexiTimeInterval6 = Convert.ToInt32(dr["FlexiTimeInterval6"]);

                    shiftModel.IsWorkDay7 = Convert.ToBoolean(dr["IsWorkDay7"]);
                    shiftModel.TimeIn7 = Convert.ToDateTime(dr["TimeIn7"]).ToString("HH:mm");
                    shiftModel.TimeOut7 = Convert.ToDateTime(dr["TimeOut7"]).ToString("HH:mm");
                    shiftModel.FlexiTimeInterval7 = Convert.ToInt32(dr["FlexiTimeInterval7"]);

                    shiftModel.IsOverNightShift = Convert.ToBoolean(dr["IsOverNightShift"]);

                    shiftModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

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

            return shiftModel;

        }

        public bool Create(ShiftModel shiftModel)
        {

            bool status = false;

            {
                shiftModel.IsOverNightShift = false;

                try
                {

                    string sql = $@"INSERT INTO Shift (";
                    sql += " " + $@"ShiftID, ShiftName,";
                    sql += " " + $@"IsWorkDay1, TimeIn1, TimeOut1, FlexiTimeInterval1,";
                    sql += " " + $@"IsWorkDay2, TimeIn2, TimeOut2, FlexiTimeInterval2,";
                    sql += " " + $@"IsWorkDay3, TimeIn3, TimeOut3, FlexiTimeInterval3,";
                    sql += " " + $@"IsWorkDay4, TimeIn4, TimeOut4, FlexiTimeInterval4,";
                    sql += " " + $@"IsWorkDay5, TimeIn5, TimeOut5, FlexiTimeInterval5,";
                    sql += " " + $@"IsWorkDay6, TimeIn6, TimeOut6, FlexiTimeInterval6,";
                    sql += " " + $@"IsWorkDay7, TimeIn7, TimeOut7, FlexiTimeInterval7,";
                    sql += " " + $@"IsOverNightShift, IsActivated";
                    sql += " " + $@") VALUES (";
                    sql += " " + $@"'{shiftModel.ShiftID}', '{shiftModel.ShiftName}', ";
                    sql += " " + $@"'{shiftModel.IsWorkDay1}', '{shiftModel.TimeIn1}', '{shiftModel.TimeOut1}', '{shiftModel.FlexiTimeInterval1}',";
                    sql += " " + $@"'{shiftModel.IsWorkDay2}', '{shiftModel.TimeIn2}', '{shiftModel.TimeOut2}', '{shiftModel.FlexiTimeInterval2}',";
                    sql += " " + $@"'{shiftModel.IsWorkDay3}', '{shiftModel.TimeIn3}', '{shiftModel.TimeOut3}', '{shiftModel.FlexiTimeInterval3}',";
                    sql += " " + $@"'{shiftModel.IsWorkDay4}', '{shiftModel.TimeIn4}', '{shiftModel.TimeOut4}', '{shiftModel.FlexiTimeInterval4}',";
                    sql += " " + $@"'{shiftModel.IsWorkDay5}', '{shiftModel.TimeIn5}', '{shiftModel.TimeOut5}', '{shiftModel.FlexiTimeInterval5}',";
                    sql += " " + $@"'{shiftModel.IsWorkDay6}', '{shiftModel.TimeIn6}', '{shiftModel.TimeOut6}', '{shiftModel.FlexiTimeInterval6}',";
                    sql += " " + $@"'{shiftModel.IsWorkDay7}', '{shiftModel.TimeIn7}', '{shiftModel.TimeOut7}', '{shiftModel.FlexiTimeInterval7}',";
                    sql += " " + $@"'{shiftModel.IsOverNightShift}', '{shiftModel.IsActivated}')";


                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;

                        string logData = $@"{shiftModel.ShiftID}, {shiftModel.ShiftName},";
                        logData += " " + $@"{shiftModel.IsWorkDay1}, {shiftModel.TimeIn1}, {shiftModel.TimeOut1}, {shiftModel.FlexiTimeInterval1},";
                        logData += " " + $@"{shiftModel.IsWorkDay2}, {shiftModel.TimeIn2}, {shiftModel.TimeOut2}, {shiftModel.FlexiTimeInterval2},";
                        logData += " " + $@"{shiftModel.IsWorkDay3}, {shiftModel.TimeIn3}, {shiftModel.TimeOut3}, {shiftModel.FlexiTimeInterval3},";
                        logData += " " + $@"{shiftModel.IsWorkDay4}, {shiftModel.TimeIn4}, {shiftModel.TimeOut4}, {shiftModel.FlexiTimeInterval4},";
                        logData += " " + $@"{shiftModel.IsWorkDay5}, {shiftModel.TimeIn5}, {shiftModel.TimeOut5}, {shiftModel.FlexiTimeInterval5},";
                        logData += " " + $@"{shiftModel.IsWorkDay6}, {shiftModel.TimeIn6}, {shiftModel.TimeOut6}, {shiftModel.FlexiTimeInterval6},";
                        logData += " " + $@"{shiftModel.IsWorkDay7}, {shiftModel.TimeIn7}, {shiftModel.TimeOut7}, {shiftModel.FlexiTimeInterval7},";
                        logData += " " + $@"{shiftModel.IsOverNightShift}, {shiftModel.IsActivated}";

                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Shift", $@"Create; {logData}", DateTime.Now);


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
        }

        public bool Update(ShiftModel shiftModel)
        {

            bool status = false;

            try
            {
                string sql = $@"UPDATE Shift SET"; 
                sql += " " + $@"ShiftName ='{shiftModel.ShiftName}',";
                sql += " " + $@"IsWorkDay1='{shiftModel.IsWorkDay1}',";
                sql += " " + $@"TimeIn1 ='{shiftModel.TimeIn1}',";
                sql += " " + $@"TimeOut1='{shiftModel.TimeOut1}',";
                sql += " " + $@"FlexiTimeInterval1 ='{shiftModel.FlexiTimeInterval1}',";
                sql += " " + $@"IsWorkDay2='{shiftModel.IsWorkDay2}',";
                sql += " " + $@"TimeIn2 ='{shiftModel.TimeIn2}',";
                sql += " " + $@"TimeOut2='{shiftModel.TimeOut2}',";
                sql += " " + $@"FlexiTimeInterval2 ='{shiftModel.FlexiTimeInterval2}',";
                sql += " " + $@"IsWorkDay3='{shiftModel.IsWorkDay3}',";
                sql += " " + $@"TimeIn3 ='{shiftModel.TimeIn3}',";
                sql += " " + $@"TimeOut3='{shiftModel.TimeOut3}',";
                sql += " " + $@"FlexiTimeInterval3 ='{shiftModel.FlexiTimeInterval3}',";
                sql += " " + $@"IsWorkDay4='{shiftModel.IsWorkDay4}',";
                sql += " " + $@"TimeIn4 ='{shiftModel.TimeIn4}',";
                sql += " " + $@"TimeOut4='{shiftModel.TimeOut4}',";
                sql += " " + $@"FlexiTimeInterval4 ='{shiftModel.FlexiTimeInterval4}',";
                sql += " " + $@"IsWorkDay5='{shiftModel.IsWorkDay5}',";
                sql += " " + $@"TimeIn5 ='{shiftModel.TimeIn5}',";
                sql += " " + $@"TimeOut5='{shiftModel.TimeOut5}',";
                sql += " " + $@"FlexiTimeInterval5 ='{shiftModel.FlexiTimeInterval5}',";
                sql += " " + $@"IsWorkDay6='{shiftModel.IsWorkDay6}',";
                sql += " " + $@"TimeIn6 ='{shiftModel.TimeIn6}',";
                sql += " " + $@"TimeOut6='{shiftModel.TimeOut6}',";
                sql += " " + $@"FlexiTimeInterval6 ='{shiftModel.FlexiTimeInterval6}',";
                sql += " " + $@"IsWorkDay7='{shiftModel.IsWorkDay7}',";
                sql += " " + $@"TimeIn7 ='{shiftModel.TimeIn7}',";
                sql += " " + $@"TimeOut7='{shiftModel.TimeOut7}',";
                sql += " " + $@"FlexiTimeInterval7 ='{shiftModel.FlexiTimeInterval7}',";
                sql += " " + $@"IsOverNightShift='{shiftModel.IsOverNightShift}',";
                sql += " " + $@"IsActivated ='{shiftModel.IsActivated}'";
                sql += " " + $@"WHERE ShiftID='{shiftModel.ShiftID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{shiftModel.ShiftName},";
                    logData += " " + $@"{shiftModel.IsWorkDay1}, {shiftModel.TimeIn1}, {shiftModel.TimeOut1}, {shiftModel.FlexiTimeInterval1},";
                    logData += " " + $@"{shiftModel.IsWorkDay2}, {shiftModel.TimeIn2}, {shiftModel.TimeOut2}, {shiftModel.FlexiTimeInterval2},";
                    logData += " " + $@"{shiftModel.IsWorkDay3}, {shiftModel.TimeIn3}, {shiftModel.TimeOut3}, {shiftModel.FlexiTimeInterval3},";
                    logData += " " + $@"{shiftModel.IsWorkDay4}, {shiftModel.TimeIn4}, {shiftModel.TimeOut4}, {shiftModel.FlexiTimeInterval4},";
                    logData += " " + $@"{shiftModel.IsWorkDay5}, {shiftModel.TimeIn5}, {shiftModel.TimeOut5}, {shiftModel.FlexiTimeInterval5},";
                    logData += " " + $@"{shiftModel.IsWorkDay6}, {shiftModel.TimeIn6}, {shiftModel.TimeOut6}, {shiftModel.FlexiTimeInterval6},";
                    logData += " " + $@"{shiftModel.IsWorkDay7}, {shiftModel.TimeIn7}, {shiftModel.TimeOut7}, {shiftModel.FlexiTimeInterval7},";
                    logData += " " + $@"{shiftModel.IsOverNightShift}, {shiftModel.IsActivated}, {shiftModel.ShiftID}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Shift", $@"Update; {logData}", DateTime.Now);

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

        public bool Delete(ShiftModel shiftModel)
        {

            bool status = false;

            string shiftID = shiftModel.ShiftID;
            List<ShiftScheduleModel> shiftScheduleList = new List<ShiftScheduleModel>();

            try
            {

                shiftScheduleList = shiftScheduleDBService.ListShiftSchedule();

                if (shiftScheduleList.Where(ss => ss.ShiftID == shiftID).ToList().Count.Equals(0))
                {

                    string sql = $@"DELETE Shift WHERE ShiftID='{shiftModel.ShiftID}'";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;


                        string logData = $@"{shiftModel.ShiftID}, {shiftModel.ShiftName}";

                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Shift", $@"Delete; {logData}", DateTime.Now);

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
    }
}