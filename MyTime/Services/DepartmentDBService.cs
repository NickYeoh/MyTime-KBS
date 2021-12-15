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
    public class DepartmentDBService
    {

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        UserDBService userDBService = new UserDBService();

        public List<DepartmentModel> ListDepartment()
        {
            DepartmentModel departmentModel;
            List<DepartmentModel> dataList = new List<DepartmentModel>();

            try
            {

                string sql = $@"SELECT * FROM Department ORDER BY DepartmentID ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        departmentModel = new DepartmentModel();

                        departmentModel.DepartmentID = dr["DepartmentID"].ToString();
                        departmentModel.DepartmentName = dr["DepartmentName"].ToString();
                        departmentModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

                        dataList.Add(departmentModel);

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


                string sql = $@"SELECT * FROM Department WHERE DepartmentID='{ID}'";

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

                string sql = $@"SELECT * FROM Department WHERE DepartmentName='{Name}'";

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

        public DepartmentModel GetDataByID(string ID)
        {
            DepartmentModel departmentModel;

            try
            {
                departmentModel = new DepartmentModel();

                string sql = $@"SELECT * FROM Department WHERE DepartmentID='{ID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    departmentModel.DepartmentID = dr["DepartmentID"].ToString();
                    departmentModel.DepartmentName = dr["DepartmentName"].ToString();
                    departmentModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

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

            return departmentModel;

        }

        public bool Create(DepartmentModel departmentModel)
        {

            bool status = false;

            try
            {

                string sql = $@"INSERT INTO Department (DepartmentID, DepartmentName, IsActivated) VALUES ('{departmentModel.DepartmentID}', '{departmentModel.DepartmentName}', '{departmentModel.IsActivated}')";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Department", $@"Create; {departmentModel.DepartmentID}, {departmentModel.DepartmentName}, {departmentModel.IsActivated}", DateTime.Now);

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

        public bool Update(DepartmentModel departmentModel)
        {

            bool status = false;

            try
            {
                string sql = $@"UPDATE Department SET DepartmentName='{departmentModel.DepartmentName}', IsActivated='{departmentModel.IsActivated}' WHERE DepartmentID='{departmentModel.DepartmentID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Department", $@"Update; {departmentModel.DepartmentName}, {departmentModel.IsActivated}, {departmentModel.DepartmentID}", DateTime.Now);

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

        public bool Delete(DepartmentModel departmentModel)
        {

            bool status = false;

            string departmentID = departmentModel.DepartmentID;
            List<UserModel> userList = new List<UserModel>();

            try
            {

                userList = userDBService.ListUser();

                if (userList.Where(u=>u.DepartmentID == departmentID).ToList().Count.Equals(0))
                {

                    string sql = $@"DELETE Department WHERE DepartmentID='{departmentModel.DepartmentID}'";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;

                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Department", $@"Delete; {departmentModel.DepartmentID}, {departmentModel.DepartmentName}", DateTime.Now);

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