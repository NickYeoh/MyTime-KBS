using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class HolidayDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        public List<HolidayModel> ListHoliday()
        {
            HolidayModel holidayModel;
            List<HolidayModel> dataList = new List<HolidayModel>();

            try
            {

                string sql = $@"SELECT * FROM Holiday ORDER BY StartOn ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        holidayModel = new HolidayModel();

                        holidayModel.HolidayID = Convert.ToInt32(dr["HolidayID"]);
                        holidayModel.HolidayName = dr["HolidayName"].ToString();
                        holidayModel.StartOn = Convert.ToDateTime(dr["StartOn"]);
                        holidayModel.EndOn = Convert.ToDateTime(dr["EndOn"]);

                        dataList.Add(holidayModel);

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

        public HolidayModel GetDataByID(int ID)
        {
            HolidayModel holidayModel;

            try
            {
                holidayModel = new HolidayModel();

                string sql = $@"SELECT * FROM Holiday WHERE HolidayID={ID}";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    holidayModel.HolidayID = Convert.ToInt32(dr["HolidayID"]);
                    holidayModel.HolidayName = dr["HolidayName"].ToString();
                    holidayModel.StartOn = Convert.ToDateTime(dr["StartOn"]);
                    holidayModel.EndOn = Convert.ToDateTime(dr["EndOn"]);

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

            return holidayModel;

        }

        public bool Create(HolidayModel holidayModel)
        {

            bool status = false;

            try
            {

                string sql = $@"INSERT INTO Holiday (HolidayName, StartOn, EndOn) VALUES ('{holidayModel.HolidayName}', '{holidayModel.StartOn.ToString("yyyyMMdd")}', '{holidayModel.EndOn.ToString("yyyyMMdd")}')";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Holiday", $@"Create; {holidayModel.HolidayName}, {holidayModel.StartOn.ToString("yyyyMMdd")}, {holidayModel.StartOn.ToString("yyyyMMdd")}, {holidayModel.EndOn.ToString("yyyyMMdd")}", DateTime.Now);

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

        public bool Update(HolidayModel holidayModel)
        {

            bool status = false;

            try
            {

                string sql = $@"UPDATE Holiday SET HolidayName='{holidayModel.HolidayName}', StartOn='{holidayModel.StartOn.ToString("yyyyMMdd")}', EndOn='{holidayModel.EndOn.ToString("yyyyMMdd")}' WHERE HolidayID={holidayModel.HolidayID}";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Holiday", $@"Update; {holidayModel.HolidayName}, {holidayModel.StartOn.ToString("yyyyMMdd")}, {holidayModel.StartOn.ToString("yyyyMMdd")}, {holidayModel.EndOn.ToString("yyyyMMdd")}, {holidayModel.HolidayID}", DateTime.Now);

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

        public bool Delete(HolidayModel holidayModel)
        {

            bool status = false;

            try
            {

                string sql = $@"DELETE Holiday WHERE HolidayID={holidayModel.HolidayID}";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Holiday", $@"Delete; {holidayModel.HolidayID}, {holidayModel.HolidayName}, {holidayModel.StartOn.ToString("yyyyMMdd")}, {holidayModel.EndOn.ToString("yyyyMMdd")}", DateTime.Now);

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

        public List<HolidayModel> GetYearHoliday(string year)
        {  
          
            HolidayModel holidayModel;
            List<HolidayModel> dataList = new List<HolidayModel>();

         
            try
            {

                string sql = $@"SELECT * FROM Holiday";
                sql += " " + $@"WHERE YEAR(StartOn)='{year}' OR YEAR(EndOn)='{year}'";
                sql += " " + $@"ORDER BY StartOn ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        holidayModel = new HolidayModel();

                        holidayModel.HolidayID = Convert.ToInt32(dr["HolidayID"]);
                        holidayModel.HolidayName = dr["HolidayName"].ToString();
                        holidayModel.StartOn = Convert.ToDateTime(dr["StartOn"]);
                        holidayModel.EndOn = Convert.ToDateTime(dr["EndOn"]);

                        dataList.Add(holidayModel);

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


    }
}