using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class AnnouncementDBService
    {

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        public List<AnnouncementModel> ListAnnouncement()
        {
            AnnouncementModel AnnouncementModel;
            List<AnnouncementModel> dataList = new List<AnnouncementModel>();

            try
            {

                string sql = $@"SELECT * FROM Announcement ORDER BY AnnouncementID ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AnnouncementModel = new AnnouncementModel();

                        AnnouncementModel.AnnouncementID = Convert.ToInt32(dr["AnnouncementID"]);
                        AnnouncementModel.AnnouncementMessage= dr["AnnouncementMessage"].ToString();
                        AnnouncementModel.AnnouncedOn = Convert.ToDateTime(dr["AnnouncedOn"]);
                        dataList.Add(AnnouncementModel);

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
    

        public AnnouncementModel GetDataByID(int ID)
        {
            AnnouncementModel AnnouncementModel;

            try
            {
                AnnouncementModel = new AnnouncementModel();

                string sql = $@"SELECT * FROM Announcement WHERE AnnouncementID={ID}";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    AnnouncementModel.AnnouncementID = Convert.ToInt32(dr["AnnouncementID"]);
                    AnnouncementModel.AnnouncementMessage= dr["AnnouncementMessage"].ToString();
                    AnnouncementModel.AnnouncedOn = Convert.ToDateTime(dr["AnnouncedOn"]);
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

            return AnnouncementModel;

        }

        public bool Create(AnnouncementModel AnnouncementModel)
        {
            bool status = false;

            try
            {

                string sql = $@"INSERT INTO Announcement (AnnouncementMessage, AnnouncedOn) VALUES ('{AnnouncementModel.AnnouncementMessage}' , '{DateTime.Now.ToString("yyyyMMdd")}')";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                   logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Announcement", $@"Create; {AnnouncementModel.AnnouncementMessage}, {DateTime.Now.ToString("yyyyMMdd")}", DateTime.Now);

                //    string log = "";
                //    foreach (SqlParameter para in cmd.Parameters)
                //    {
                //       if (log.Length > 0)
                //        {
                //            log += string.Format("{0}", ",");
                //        }

                //        log += string.Format("{0}", para.Value.ToString());
                //    }
                   
                //    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Announcement", $@"Create; {log}", DateTime.Now);
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

        public bool Update(AnnouncementModel AnnouncementModel)
        {

            bool status = false;

            try
            {
                string sql = $@"UPDATE Announcement SET AnnouncementMessage='{AnnouncementModel.AnnouncementMessage}' WHERE AnnouncementID='{AnnouncementModel.AnnouncementID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Announcement", $@"Update; {AnnouncementModel.AnnouncementMessage}, {AnnouncementModel.AnnouncementID}", DateTime.Now);
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

        public bool Delete(AnnouncementModel AnnouncementModel)
        {

            bool status = false;

            try
            {

                string sql = $@"DELETE Announcement WHERE AnnouncementID='{AnnouncementModel.AnnouncementID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Announcement", $@"Delete; {AnnouncementModel.AnnouncementID}, {AnnouncementModel.AnnouncementMessage}", DateTime.Now);
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