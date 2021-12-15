using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using MyTime.ViewModels;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;    

namespace MyTime.Services
{
    public class UnitDBService
    {

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        UserDBService userDBService = new UserDBService();


        public List<UnitModel> ListUnit()
        {
            UnitModel unitModel;
            List<UnitModel> dataList = new List<UnitModel>();

            try
            {

                string sql = $@"SELECT * FROM Unit U 
                             INNER JOIN Department D ON D.DepartmentID = U.DepartmentID
                             ORDER BY UnitID ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        unitModel = new UnitModel();

                        unitModel.DepartmentID = dr["DepartmentID"].ToString();
                        unitModel.DepartmentName = dr["DepartmentName"].ToString();
                        unitModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        unitModel.UnitName = dr["UnitName"].ToString();
                        unitModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

                        dataList.Add(unitModel);

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


        public List<UnitModel> ListUnit(string departmentID)
        {
            UnitModel unitModel;
            List<UnitModel> dataList = new List<UnitModel>();

            try
            {
                string sql = $@"SELECT U.DepartmentID, D.DepartmentName, U.UnitID, U.UnitName, U.IsActivated FROM Unit U 
                             INNER JOIN Department D ON D.DepartmentID = U.DepartmentID
                             WHERE U.DepartmentID='{departmentID}'  
                             ORDER BY UnitName ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        unitModel = new UnitModel();

                        unitModel.DepartmentID = dr["DepartmentID"].ToString();
                        unitModel.DepartmentName = dr["DepartmentName"].ToString();
                        unitModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        unitModel.UnitName = dr["UnitName"].ToString();
                        unitModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

                        dataList.Add(unitModel);

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

        public Boolean CheckDuplicateName(string ID,  string Name)
        {
            Boolean isDuplicated = false;

            try
            {

                string sql = $@"SELECT * FROM Unit WHERE DepartmentID='{ID}' AND UnitName='{Name}'";

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


        public UnitModel GetDataByID(string ID)
        {
            UnitModel unitModel;

            try
            {
                unitModel = new UnitModel();

                string sql = $@"SELECT * FROM Unit U 
                             INNER JOIN Department D ON D.DepartmentID = U.DepartmentID
                             WHERE UnitID='{ID}'";
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    unitModel.DepartmentID = dr["DepartmentID"].ToString();
                    unitModel.DepartmentName = dr["DepartmentName"].ToString();
                    unitModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                    unitModel.UnitName = dr["UnitName"].ToString();
                    unitModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

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

            return unitModel;

        }

        public bool Create(UnitViewModel unitViewModel)
        {

            bool status = false;

            try
            {

                string sql = $@"INSERT INTO Unit (DepartmentID, UnitName, IsActivated) VALUES ('{unitViewModel.DepartmentID}', '{unitViewModel.UnitName}', '{unitViewModel.IsActivated}')";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Unit", $@"Create; {unitViewModel.DepartmentID}, {unitViewModel.UnitName}, {unitViewModel.IsActivated}", DateTime.Now);

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

        public bool Update(UnitModel unitModel)
        {

            bool status = false;

            try
            {

                string sql = $@"UPDATE Unit SET UnitName='{unitModel.UnitName}', IsActivated='{unitModel.IsActivated}' WHERE UnitID='{unitModel.UnitID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Unit", $@"Update; {unitModel.UnitName}, {unitModel.IsActivated}, {unitModel.UnitID}", DateTime.Now);

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

        public bool Delete(UnitModel unitModel)
        {

            bool status = false;

            int unitID = unitModel.UnitID;
            List<UserModel> userList = new List<UserModel>();

            try
            {
                userList = userDBService.ListUser();

                if (userList.Where(u => u.UnitID== unitID).ToList().Count.Equals(0))
                {

                    string sql = $@"DELETE Unit WHERE UnitID='{unitModel.UnitID}'";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;


                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Unit", $@"Delete; {unitModel.DepartmentID}, {unitModel.UnitID}, {unitModel.DepartmentName}", DateTime.Now);

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