using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class ReportAdminDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        public List<ReportAdminModel> ListReportAdmin()
        {
            ReportAdminModel reportAdminModel = new ReportAdminModel();
            List<ReportAdminModel> dataList = new List<ReportAdminModel>();

            try
            {
                // Get User with role can approve reason - PR 
                string sql = $@"SELECT U.NRIC, U.UserName, U.DepartmentID, D.DepartmentName,";
                sql += " " + $@"U.UnitID, UT.UnitName, U.RoleID, R.RoleName";
                sql += " " + $@"FROM [User] U LEFT JOIN Access A ON A.RoleID = U.RoleID";
                sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
                sql += " " + $@"LEFT JOIN Unit UT ON UT.UnitID = U.UnitID";
                sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
                sql += " " + $@"WHERE A.FunctionID = 'PR' AND A.IsAccessAllowed = 'true'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        reportAdminModel = new ReportAdminModel();

                        reportAdminModel.ReportAdminNRIC = dr["NRIC"].ToString();
                        reportAdminModel.ReportAdminName = dr["UserName"].ToString();
                        reportAdminModel.DepartmentID = dr["DepartmentID"].ToString();
                        reportAdminModel.DepartmentName = dr["DepartmentName"].ToString();

                        if (!dr["UnitID"].Equals(DBNull.Value))
                        {
                            reportAdminModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            reportAdminModel.UnitName = dr["UnitName"].ToString();
                        }

                        reportAdminModel.RoleID = Convert.ToInt32(dr["RoleID"]);
                        reportAdminModel.RoleName = dr["RoleName"].ToString();

                        dataList.Add(reportAdminModel);

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

        public ReportAdminModel GetDataByID(string ID)
        {
            ReportAdminModel reportAdminModel = new ReportAdminModel();

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
                        reportAdminModel.ReportAdminNRIC = dr["NRIC"].ToString();


                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            reportAdminModel.ReportAdminName = dr["UserName"].ToString();
                        }

                        if (!dr["RoleID"].Equals(DBNull.Value))
                        {
                            reportAdminModel.RoleID = Convert.ToInt32(dr["RoleID"]);
                        }

                        if (!dr["RoleName"].Equals(DBNull.Value))
                        {
                            reportAdminModel.RoleName = dr["RoleName"].ToString();
                        }

                        if (!dr["DepartmentID"].Equals(DBNull.Value))
                        {
                            reportAdminModel.DepartmentID = dr["DepartmentID"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            reportAdminModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UnitID"].Equals(DBNull.Value))
                        {
                            reportAdminModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            reportAdminModel.UnitName = dr["UnitName"].ToString();
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

            return reportAdminModel;
        }

        public List<ReportAdminDepartmentModel> ListReportAdminDepartment(String reportAdminNRIC)
        {

            List<ReportAdminDepartmentModel> dataList = new List<ReportAdminDepartmentModel>();
            ReportAdminDepartmentModel reportAdminDepartment = new ReportAdminDepartmentModel();

            string sql = $@"SELECT RA.DepartmentID, D.DepartmentName";           
            sql += " " + $@"FROM ReportAdminDepartment RA";         
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = RA.DepartmentID";        
            sql += " " + $@"WHERE RA.ReportAdminNRIC = '{reportAdminNRIC}'";

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        reportAdminDepartment = new ReportAdminDepartmentModel();

                        reportAdminDepartment.ReportAdminNRIC = reportAdminNRIC;

                        if (!dr["DepartmentID"].Equals(DBNull.Value))
                        {
                            reportAdminDepartment.DepartmentID = dr["DepartmentID"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            reportAdminDepartment.DepartmentName = dr["DepartmentName"].ToString();
                        }


                        dataList.Add(reportAdminDepartment);
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

        public List<ReportAdminDepartmentModel> ListNewReportAdminDepartment(String reportAdminNRIC)
        {

            List<ReportAdminDepartmentModel> dataList = new List<ReportAdminDepartmentModel>();
            ReportAdminDepartmentModel reportAdminDepartment = new ReportAdminDepartmentModel();

            string sql = $@"SELECT DepartmentID, DepartmentName";           
            sql += " " + $@"FROM Department";          
            sql += " " + $@"WHERE DepartmentID NOT IN";
            sql += " " + $@"(SELECT DepartmentID FROM ReportAdminDepartment WHERE ReportAdminNRIC='{reportAdminNRIC}')";

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        reportAdminDepartment = new ReportAdminDepartmentModel();

                        reportAdminDepartment.ReportAdminNRIC = reportAdminNRIC;


                        if (!dr["DepartmentID"].Equals(DBNull.Value))
                        {
                            reportAdminDepartment.DepartmentID= dr["DepartmentID"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            reportAdminDepartment.DepartmentName = dr["DepartmentName"].ToString();
                        }                                         

                        dataList.Add(reportAdminDepartment);
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

        public bool AddReportAdminDepartment(String reportAdminNRIC, String selectedDepartmentID)
        {
            bool status = false;

            string[] departmentID = selectedDepartmentID.Split('#');
            string sql;
            SqlCommand cmd;


            try
            {

                if (!departmentID.Length.Equals(0))
                {
                    conn.Open();

                    for (int i = 0; i < departmentID.Length; i++)
                    {

                        sql = $@"INSERT INTO ReportAdminDepartment (ReportAdminNRIC, DepartmentID)";
                        
                        sql += " " + $@"VALUES ('{reportAdminNRIC}', '{departmentID[i]}')";

                        cmd = new SqlCommand(sql, conn);

                        cmd.ExecuteNonQuery();


                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Report Admin", $@"Create; {reportAdminNRIC}, {departmentID[i]}", DateTime.Now);


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



        public bool DeleteReportAdminDepartment(String reportAdminNRIC, String departmentID)
        {

            bool status = false;

            try
            {
                string sql = $@"DELETE ReportAdminDepartment WHERE ReportAdminNRIC='{reportAdminNRIC}' AND";
                sql += " " + $@"DepartmentID='{departmentID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Report Admin", $@"Delete; {reportAdminNRIC}, {departmentID}", DateTime.Now);

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