using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class ApproverDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        public List<ApproverModel> ListApprover()
        {
            ApproverModel approverModel = new ApproverModel();                        
            List<ApproverModel> dataList = new List<ApproverModel>();

            try
            {
                // Get User with role can approve reason - AR 
                string sql = $@"SELECT U.NRIC, U.UserName, U.DepartmentID, D.DepartmentName,";
                sql += " " + $@"U.UnitID, UT.UnitName, U.RoleID, R.RoleName";
                sql += " " + $@"FROM [User] U LEFT JOIN Access A ON A.RoleID = U.RoleID";
                sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
                sql += " " + $@"LEFT JOIN Unit UT ON UT.UnitID = U.UnitID";
                sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
                sql += " " + $@"WHERE A.FunctionID = 'AR' AND A.IsAccessAllowed = 'true'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        approverModel = new ApproverModel();

                        approverModel.ApproverNRIC = dr["NRIC"].ToString();
                        approverModel.ApproverName = dr["UserName"].ToString();
                        approverModel.DepartmentID = dr["DepartmentID"].ToString();
                        approverModel.DepartmentName = dr["DepartmentName"].ToString();

                        if (!dr["UnitID"].Equals(DBNull.Value))
                        {
                            approverModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            approverModel.UnitName = dr["UnitName"].ToString();
                        }

                        approverModel.RoleID = Convert.ToInt32(dr["RoleID"]);
                        approverModel.RoleName = dr["RoleName"].ToString();

                        dataList.Add(approverModel);

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

        public ApproverModel GetDataByID(string ID)
        {
            ApproverModel approverModel = new ApproverModel();

            string sql = $@"SELECT U.NRIC, U.USRID, U.UserName,";
            sql += " " + $@"U.DepartmentID, D.DepartmentName,";
            sql += " " + $@"U.UnitID, UT.UnitName,";
            sql += " " + $@"U.RoleID, R.RoleName";
            sql += " " + $@"FROM [User] U";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
            sql += " " + $@"LEFT JOIN Unit UT ON U.UnitID = UT.UnitID";
            sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
            sql += " " + $@"WHERE NRIC = '{ID}'";

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {


                    while (dr.Read())
                    {
                       approverModel.ApproverNRIC= dr["NRIC"].ToString();


                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            approverModel.ApproverName = dr["UserName"].ToString();
                        }

                        if (!dr["RoleID"].Equals(DBNull.Value))
                        {
                            approverModel.RoleID = Convert.ToInt32(dr["RoleID"]);
                        }

                        if (!dr["RoleName"].Equals(DBNull.Value))
                        {
                            approverModel.RoleName = dr["RoleName"].ToString();
                        }

                        if (!dr["DepartmentID"].Equals(DBNull.Value))
                        {
                            approverModel.DepartmentID = dr["DepartmentID"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            approverModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UnitID"].Equals(DBNull.Value))
                        {
                            approverModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            approverModel.UnitName = dr["UnitName"].ToString();
                        }
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

            return approverModel;
        }

        public List<ApproverUserModel> ListApproverUser(String approverNRIC)
        {

            List<ApproverUserModel> dataList = new List<ApproverUserModel>();
            ApproverUserModel approverUserModel = new ApproverUserModel();

            string sql = $@"SELECT U.NRIC, U.UserName,";
            sql += " " + $@"U.DepartmentID, D.DepartmentName,";
            sql += " " + $@"U.UnitID, UT.UnitName,";
            sql += " " + $@"U.RoleID, R.RoleName";
            sql += " " + $@"FROM ApproverUser A";
            sql += " " + $@"INNER JOIN [User] U ON U.NRIC = A.UserNRIC";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
            sql += " " + $@"LEFT JOIN Unit UT ON U.UnitID = UT.UnitID";
            sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
            sql += " " + $@"WHERE A.ApproverNRIC = '{approverNRIC}'";

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        approverUserModel = new ApproverUserModel();

                        approverUserModel.ApproverNRIC = approverNRIC;

                        if (!dr["NRIC"].Equals(DBNull.Value))
                        {
                            approverUserModel.NRIC = dr["NRIC"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            approverUserModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            approverUserModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            approverUserModel.UnitName = dr["UnitName"].ToString();
                        }

                        dataList.Add(approverUserModel);
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

        public List<ApproverUserModel> GetUserApprover(String userNRIC)
        {

            List<ApproverUserModel> dataList = new List<ApproverUserModel>();
            ApproverUserModel approverUserModel = new ApproverUserModel();

            string sql = $@"SELECT U.NRIC, U.UserName,";
            sql += " " + $@"U.DepartmentID, D.DepartmentName,";
            sql += " " + $@"U.UnitID, UT.UnitName,";
            sql += " " + $@"U.RoleID, R.RoleName";
            sql += " " + $@"FROM ApproverUser A";
            sql += " " + $@"INNER JOIN [User] U ON U.NRIC = A.ApproverNRIC";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
            sql += " " + $@"LEFT JOIN Unit UT ON U.UnitID = UT.UnitID";
            sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
            sql += " " + $@"WHERE A.UserNRIC = '{userNRIC}'";

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        approverUserModel = new ApproverUserModel();

                        approverUserModel.NRIC = userNRIC;

                        if (!dr["NRIC"].Equals(DBNull.Value))
                        {
                            approverUserModel.ApproverNRIC = dr["NRIC"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            approverUserModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            approverUserModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            approverUserModel.UnitName = dr["UnitName"].ToString();
                        }

                        dataList.Add(approverUserModel);
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

        public List<ApproverUserModel> ListNewApproverUser(String approverNRIC)
        {

            List<ApproverUserModel> dataList = new List<ApproverUserModel>();
            ApproverUserModel approverUserModel = new ApproverUserModel();

            string sql = $@"SELECT U.NRIC, U.UserName,";
            sql += " " + $@"U.DepartmentID, D.DepartmentName,";
            sql += " " + $@"U.UnitID, UT.UnitName,";
            sql += " " + $@"U.RoleID, R.RoleName";
            sql += " " + $@"FROM [User] U";            
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
            sql += " " + $@"LEFT JOIN Unit UT ON U.UnitID = UT.UnitID";
            sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
            sql += " " + $@"WHERE U.NRIC NOT IN";
            sql += " " + $@"(SELECT UserNRIC FROM ApproverUser WHERE ApproverNRIC='{approverNRIC}')"; 

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        approverUserModel = new ApproverUserModel();

                        approverUserModel.ApproverNRIC = approverNRIC;

                        if (!dr["NRIC"].Equals(DBNull.Value))
                        {
                            approverUserModel.NRIC = dr["NRIC"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            approverUserModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            approverUserModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            approverUserModel.UnitName = dr["UnitName"].ToString();
                        }

                        dataList.Add(approverUserModel);
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

        public bool AddApproverUser(String approverNRIC, String selectedUserNRIC)
        {
            bool status = false;

            string[] userNRIC = selectedUserNRIC.Split('#');
            string sql;
            SqlCommand cmd;


            try
            {

                if (!userNRIC.Length.Equals(0))
                {
                    conn.Open();

                    for (int i = 0; i < userNRIC.Length; i++)
                    {

                        sql = $@"INSERT INTO ApproverUser (ApproverNRIC, UserNRIC)"; 
                        sql += " " + $@"VALUES ('{approverNRIC}', '{userNRIC[i]}')";

                        cmd = new SqlCommand(sql, conn);

                        cmd.ExecuteNonQuery();

                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Approver User", $@"Create; {approverNRIC}, {userNRIC[i]}", DateTime.Now);


                    }
                }

                status = true;

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



        public bool DeleteApproverUser(String approverNRIC, String userNRIC)
        {

            bool status = false;

            try
            {
                string sql = $@"DELETE FROM ApproverUser WHERE ApproverNRIC='{approverNRIC}' AND";
                sql += " " + $@"UserNRIC='{userNRIC}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Approver User", $@"Delete; {approverNRIC}, {userNRIC}", DateTime.Now);

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