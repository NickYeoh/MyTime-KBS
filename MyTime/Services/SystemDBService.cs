using System;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class SystemDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        public SystemModel GetData()
        {
            SystemModel systemSettingModel;

            try
            {
                systemSettingModel = new SystemModel();

                string sql = $@"SELECT * FROM System S";
                sql += " " + "LEFT JOIN Role R ON R.RoleID = S.DefaultRoleID";
                sql += " " + "LEFT JOIN Shift ST ON ST.ShiftID= S.DefaultShiftID";
                sql += " " + "LEFT JOIN AccessRole A ON A.AccessRoleID = S.DefaultAccessRoleID";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    if (!dr["OrganisationName"].Equals(DBNull.Value))
                    {
                        systemSettingModel.OrganisationName = dr["OrganisationName"].ToString();

                    }

                    if (!dr["OrganisationShortName"].Equals(DBNull.Value))
                    {
                        systemSettingModel.OrganisationShortName= dr["OrganisationShortName"].ToString();

                    }

                    if (!dr["OrganisationLogo"].Equals(DBNull.Value))
                    {
                        systemSettingModel.OrganisationLogo = dr["OrganisationLogo"].ToString();
                    }
                 
                    if (!dr["DefaultRoleID"].Equals(DBNull.Value))
                    {
                        systemSettingModel.DefaultRoleID = Convert.ToInt32(dr["DefaultRoleID"]);
                        systemSettingModel.DefaultRoleName = dr["RoleName"].ToString();
                    }

                    if (!dr["DefaultShiftID"].Equals(DBNull.Value))
                    {
                        systemSettingModel.DefaultShiftID = dr["DefaultShiftID"].ToString();
                        systemSettingModel.DefaultShiftName = dr["ShiftName"].ToString();

                    }

                    if (!dr["ReasonSubmissionPeriod"].Equals(DBNull.Value))
                    {
                        systemSettingModel.ReasonSubmissionPeriod = Convert.ToInt32(dr["ReasonSubmissionPeriod"]);
                    }
                    else
                    {
                        systemSettingModel.ReasonSubmissionPeriod = 0;
                    }


                    if (!dr["DefaultAccessRoleID"].Equals(DBNull.Value))
                    {
                        systemSettingModel.DefaultAccessRoleID = Convert.ToInt32(dr["DefaultAccessRoleID"]);
                        systemSettingModel.DefaultAccessRoleName = dr["AccessRoleName"].ToString();
                    }

                    if (!dr["DataStartDate"].Equals(DBNull.Value))
                    {
                        systemSettingModel.DataStartDate = Convert.ToDateTime(dr["DataStartDate"]);
                    }
                    else
                    {
                        systemSettingModel.DataStartDate = DateTime.Now;
                    }


                    if (!dr["IsEmailNotificationEnabled"].Equals(DBNull.Value))
                    {
                        systemSettingModel.IsEmailNotificationEnabled = Convert.ToBoolean(dr["IsEmailNotificationEnabled"]);
                    }
                    else
                    {
                        systemSettingModel.IsEmailNotificationEnabled = false;
                    }


                    if (!dr["IsEmailReminderEnabled"].Equals(DBNull.Value))
                    {
                        systemSettingModel.IsEmailReminderEnabled = Convert.ToBoolean(dr["IsEmailReminderEnabled"]);
                    }
                    else
                    {
                        systemSettingModel.IsEmailReminderEnabled = false;
                    }

                    if (!dr["AttendanceCardStartDate"].Equals(DBNull.Value))
                    {
                        systemSettingModel.AttendanceCardStartDate= Convert.ToDateTime(dr["AttendanceCardStartDate"]);
                    }
                    else
                    {
                        systemSettingModel.AttendanceCardStartDate = DateTime.Now;
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

            return systemSettingModel;

        }


        public bool Update(SystemModel systemModel)
        {

            bool status = false;

            try
            {
                string sql = $@"UPDATE System SET";
                sql += " " + $@"OrganisationName='{systemModel.OrganisationName}',";
                sql += " " + $@"OrganisationShortName='{systemModel.OrganisationShortName}',";
                if (!systemModel.OrganisationLogo.Equals(""))
                {
                    sql += " " + $@"OrganisationLogo='{systemModel.OrganisationLogo}',";
                }
               
                sql += " " + $@"DefaultRoleID='{systemModel.DefaultRoleID}',";
                sql += " " + $@"DefaultShiftID='{systemModel.DefaultShiftID}',";
                sql += " " + $@"ReasonSubmissionPeriod='{systemModel.ReasonSubmissionPeriod}',";
                sql += " " + $@"DefaultAccessRoleID='{systemModel.DefaultAccessRoleID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{systemModel.OrganisationName}, {systemModel.OrganisationShortName}, {systemModel.DefaultRoleID}, {systemModel.DefaultShiftID}, {systemModel.ReasonSubmissionPeriod}, {systemModel.DefaultAccessRoleID}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "System", $@"Update; {logData}", DateTime.Now);

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