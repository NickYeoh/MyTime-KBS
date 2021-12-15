using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class ReasonDBService
    {

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        public List<ReasonModel> ListReason()
        {
            ReasonModel reasonModel;
            List<ReasonModel> dataList = new List<ReasonModel>();

            try
            {

                string sql = $@"SELECT * FROM Reason ORDER BY ReasonID ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        reasonModel = new ReasonModel();

                        reasonModel.ReasonID = dr["ReasonID"].ToString();
                        reasonModel.ReasonName = dr["ReasonName"].ToString();
                        reasonModel.IsForLateIn = Convert.ToBoolean(dr["IsForLateIn"]);
                        reasonModel.IsForEarlyOut = Convert.ToBoolean(dr["IsForEarlyOut"]);
                        reasonModel.IsForIncomplete = Convert.ToBoolean(dr["IsForIncomplete"]);
                        reasonModel.IsForAbsent = Convert.ToBoolean(dr["IsForAbsent"]);
                        reasonModel.IsForOnLeave = Convert.ToBoolean(dr["IsForOnLeave"]);
                        reasonModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

                        dataList.Add(reasonModel);

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


                string sql = $@"SELECT * FROM Reason WHERE ReasonID='{ID}'";

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

                string sql = $@"SELECT * FROM Reason WHERE ReasonName='{Name}'";

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

        public ReasonModel GetDataByID(string ID)
        {
            ReasonModel reasonModel;

            try
            {
                reasonModel = new ReasonModel();

                string sql = $@"SELECT * FROM Reason WHERE ReasonID='{ID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    reasonModel.ReasonID = dr["ReasonID"].ToString();
                    reasonModel.ReasonName = dr["ReasonName"].ToString();
                    reasonModel.IsForLateIn = Convert.ToBoolean(dr["IsForLateIn"]);
                    reasonModel.IsForEarlyOut = Convert.ToBoolean(dr["IsForEarlyOut"]);
                    reasonModel.IsForIncomplete = Convert.ToBoolean(dr["IsForIncomplete"]);
                    reasonModel.IsForAbsent = Convert.ToBoolean(dr["IsForAbsent"]);
                    reasonModel.IsForOnLeave = Convert.ToBoolean(dr["IsForOnLeave"]);
                    reasonModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

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

            return reasonModel;

        }

        public bool Create(ReasonModel reasonModel)
        {

            bool status = false;

            try
            {

                string sql = $@"INSERT INTO Reason (ReasonID, ReasonName, IsForLateIn, IsForEarlyOut, IsForIncomplete, IsForAbsent, IsForOnLeave, IsActivated)";
                sql += " " + $@"VALUES ('{reasonModel.ReasonID}', '{reasonModel.ReasonName}',";
                sql += " " + $@"'{reasonModel.IsForLateIn}',  '{reasonModel.IsForEarlyOut}',";
                sql += " " + $@"'{reasonModel.IsForIncomplete}','{reasonModel.IsForAbsent}',  '{reasonModel.IsForOnLeave}',";
                sql += " " + $@"'{reasonModel.IsActivated}')";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{reasonModel.ReasonID}, {reasonModel.ReasonName},";
                    logData += " " + $@"{reasonModel.IsForLateIn},  {reasonModel.IsForEarlyOut},";
                    logData += " " + $@"{reasonModel.IsForIncomplete},{reasonModel.IsForAbsent},  {reasonModel.IsForOnLeave},";
                    logData += " " + $@"{reasonModel.IsActivated}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Reason", $@"Create; {logData}", DateTime.Now);

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

        public bool Update(ReasonModel reasonModel)
        {

            bool status = false;

            try
            {
                string sql = $@"UPDATE Reason SET ReasonName='{reasonModel.ReasonName}', 
                            IsForLateIn='{reasonModel.IsForLateIn}', 
                            IsForEarlyOut='{reasonModel.IsForEarlyOut}', 
                            IsForAbsent='{reasonModel.IsForAbsent}', 
                            IsForOnLeave='{reasonModel.IsForOnLeave}', 
                            IsActivated='{reasonModel.IsActivated}' 
                            WHERE ReasonID='{reasonModel.ReasonID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{reasonModel.ReasonName},";
                    logData += " " + $@"{reasonModel.IsForLateIn},  {reasonModel.IsForEarlyOut},";
                    logData += " " + $@"{reasonModel.IsForIncomplete},{reasonModel.IsForAbsent},  {reasonModel.IsForOnLeave},";
                    logData += " " + $@"{reasonModel.IsActivated}, {reasonModel.ReasonID}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Reason", $@"Update; {logData}", DateTime.Now);

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

        public bool Delete(ReasonModel reasonModel)
        {

            bool status = false;

            try
            {

                string sql = $@"DELETE Reason WHERE ReasonID='{reasonModel.ReasonID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{reasonModel.ReasonID}, {reasonModel.ReasonName}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Reason", $@"Delete; {logData}", DateTime.Now);

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