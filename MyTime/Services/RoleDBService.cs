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
    public class RoleDBService
    {

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();
        UserDBService userDBService = new UserDBService();

        public List<RoleModel> ListRole()
        {
            RoleModel roleModel;
            List<RoleModel> dataList = new List<RoleModel>();

            try
            {

                string sql = $@"SELECT * FROM Role ORDER BY RoleID ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        roleModel = new RoleModel();

                        roleModel.RoleID = Convert.ToInt32(dr["RoleID"]);
                        roleModel.RoleName = dr["RoleName"].ToString();
                        roleModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

                        dataList.Add(roleModel);

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

     
        public Boolean CheckDuplicateName(string Name)
        {
            Boolean isDuplicated = false;

            try
            {

                string sql = $@"SELECT * FROM Role WHERE RoleName='{Name}'";

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

        public RoleModel GetDataByID(int ID)
        {
            RoleModel roleModel;

            try
            {
                roleModel = new RoleModel();

                string sql = $@"SELECT * FROM Role WHERE RoleID={ID}";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    roleModel.RoleID = Convert.ToInt32(dr["RoleID"]);
                    roleModel.RoleName = dr["RoleName"].ToString();
                    roleModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

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

            return roleModel;

        }

        public bool Create(RoleModel roleModel)
        {
            bool status = false;

            try
            {

                string sql = $@"INSERT INTO Role (RoleName, IsActivated) VALUES ('{roleModel.RoleName}', '{roleModel.IsActivated}')";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Role", $@"Create; {roleModel.RoleName}, {roleModel.IsActivated}", DateTime.Now);
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

        public bool Update(RoleModel roleModel)
        {

            bool status = false;

            try
            {
                string sql = $@"UPDATE Role SET RoleName='{roleModel.RoleName}', IsActivated='{roleModel.IsActivated}' WHERE RoleID='{roleModel.RoleID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;


                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Role", $@"Update; {roleModel.RoleName}, {roleModel.IsActivated}, {roleModel.RoleID}", DateTime.Now);


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

        public bool Delete(RoleModel roleModel)
        {

            bool status = false;


            int roleID = roleModel.RoleID;
            List<UserModel> userList = new List<UserModel>();

            try
            {
                userList = userDBService.ListUser();

                if (userList.Where(u => u.RoleID == roleID).ToList().Count.Equals(0))
                {

                    string sql = $@"DELETE Role WHERE RoleID='{roleModel.RoleID}'";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;


                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Role", $@"Delete;  {roleModel.RoleID}, {roleModel.RoleName}", DateTime.Now);

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