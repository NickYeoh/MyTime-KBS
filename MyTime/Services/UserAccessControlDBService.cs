using System;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;

namespace MyTime.Services
{
    public class UserAccessControlDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();


        public UserAccessControlModel IsAccessAllowed(int roleID)
        {
            //bool isAccessAllowed = false;

            string sql = $@"SELECT * FROM Access WHERE RoleID='{roleID}'";

            UserAccessControlModel userAccessControlModel = new UserAccessControlModel();

            try
            {
                userAccessControlModel.RoleID = roleID;
                userAccessControlModel.IsAllowedDashboard = false;
                userAccessControlModel.IsAllowedAttendance = false;
                userAccessControlModel.IsAllowedAnnouncement = false;
                userAccessControlModel.IsAllowedSystemSetting = false;
                userAccessControlModel.IsAllowedDevice = false;
                userAccessControlModel.IsAllowedOrganisation = false;
                userAccessControlModel.IsAllowedUser = false;
                userAccessControlModel.IsAllowedShiftSchedule = false;
                userAccessControlModel.IsAllowedApproveReason = false;
                userAccessControlModel.IsAllowedPrintReport = false;
                userAccessControlModel.IsAllowedContactUs = false;

                // 2022-07-11 : Default               
                userAccessControlModel.IsAllowedChangePassword = true;

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        switch (dr["FunctionID"].ToString().ToUpper())
                        {
                            case "DB":
                                // Dashboard
                                userAccessControlModel.IsAllowedDashboard = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "AT":
                                // Attendance
                                userAccessControlModel.IsAllowedAttendance = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "AN":
                                // Announcement
                                userAccessControlModel.IsAllowedAnnouncement = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "SS":
                                // System Setting
                                userAccessControlModel.IsAllowedSystemSetting = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "DE":
                                // Device
                                userAccessControlModel.IsAllowedDevice = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "OR":
                                // Organisation
                                userAccessControlModel.IsAllowedOrganisation = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "UR":
                                // User
                                userAccessControlModel.IsAllowedUser = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "ST":
                                // Shift Schedule
                                userAccessControlModel.IsAllowedShiftSchedule= Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "AR":
                                // Approve Reason
                                userAccessControlModel.IsAllowedApproveReason = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "PR":
                                // Print Report
                                userAccessControlModel.IsAllowedPrintReport = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "CU":
                                // Contact Us
                                userAccessControlModel.IsAllowedContactUs= Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            default:
                                break;
                        };

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

            return userAccessControlModel;
        }

        public UserAccessControlModel IsAccessAllowed(UserModel userModel)
        {
            // Overloading technique
            //bool isAccessAllowed = false;
            int roleID;

            roleID = userModel.RoleID;

            string sql = $@"SELECT * FROM Access WHERE RoleID='{roleID}'";

            UserAccessControlModel userAccessControlModel = new UserAccessControlModel();

            try
            {
                userAccessControlModel.RoleID = roleID;
                userAccessControlModel.IsAllowedDashboard = false;
                userAccessControlModel.IsAllowedAttendance = false;
                userAccessControlModel.IsAllowedAnnouncement = false;
                userAccessControlModel.IsAllowedSystemSetting = false;
                userAccessControlModel.IsAllowedDevice = false;
                userAccessControlModel.IsAllowedOrganisation = false;
                userAccessControlModel.IsAllowedUser = false;
                userAccessControlModel.IsAllowedShiftSchedule = false;
                userAccessControlModel.IsAllowedApproveReason = false;
                userAccessControlModel.IsAllowedPrintReport = false;
                userAccessControlModel.IsAllowedContactUs = false;

              


                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        switch (dr["FunctionID"].ToString().ToUpper())
                        {
                            case "DB":
                                // Dashboard
                                userAccessControlModel.IsAllowedDashboard = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "AT":
                                // Attendance
                                userAccessControlModel.IsAllowedAttendance = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "AN":
                                // Announcement
                                userAccessControlModel.IsAllowedAnnouncement = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "SS":
                                // System Setting
                                userAccessControlModel.IsAllowedSystemSetting = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "DE":
                                // Device
                                userAccessControlModel.IsAllowedDevice = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "OR":
                                // Organisation
                                userAccessControlModel.IsAllowedOrganisation = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "UR":
                                // User
                                userAccessControlModel.IsAllowedUser = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "ST":
                                // Shift Schedule
                                userAccessControlModel.IsAllowedShiftSchedule = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "AR":
                                // Approve Reason
                                userAccessControlModel.IsAllowedApproveReason = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "PR":
                                // Print Report
                                userAccessControlModel.IsAllowedPrintReport = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            case "CU":
                                // Contact Us
                                userAccessControlModel.IsAllowedContactUs = Convert.ToBoolean(dr["IsAccessAllowed"]);
                                break;
                            default:
                                break;
                        };

                    }
                }

                // 2022-07-11 : Block functions
                if (userModel.DepartmentID != null || userModel.DepartmentID == "")
                {                  
                    userAccessControlModel.IsAllowedChangePassword = true;
                }
                else
                {
                    userAccessControlModel.IsAllowedAttendance = false;
                    userAccessControlModel.IsAllowedChangePassword = false;
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

            return userAccessControlModel;
        }

        public bool Update(UserAccessControlModel userAccessControlModel)
        {
            bool isFound = false;
            bool status = false;
            string functionID = "";
            bool isAccessAllowed = false;

            try
            {
                conn.Open();
                SqlCommand cmd;

                for (int i = 0; i < 11; i++)
                {
                    isFound = false;
                    isAccessAllowed = false;

                    switch (i)
                    {
                        case 0:
                            // Dashboard
                            functionID = "DB";
                            isAccessAllowed = userAccessControlModel.IsAllowedDashboard;
                            break;

                        case 1:
                            //Announcement
                            functionID = "AT";
                            isAccessAllowed = userAccessControlModel.IsAllowedAttendance;
                            break;

                        case 2:
                            //Announcement
                            functionID = "AN";
                            isAccessAllowed = userAccessControlModel.IsAllowedAnnouncement;
                            break;

                        case 3:
                            //System Setting
                            functionID = "SS";
                            isAccessAllowed = userAccessControlModel.IsAllowedSystemSetting;
                            break;

                        case 4:
                            //System Setting
                            functionID = "DE";
                            isAccessAllowed = userAccessControlModel.IsAllowedDevice;
                            break;

                        case 5:
                            //Organisation
                            functionID = "OR";
                            isAccessAllowed = userAccessControlModel.IsAllowedOrganisation;
                            break;

                        case 6:
                            //User
                            functionID = "UR";
                            isAccessAllowed = userAccessControlModel.IsAllowedUser;
                            break;

                        case 7:
                            //Shift Schedule
                            functionID = "ST";
                            isAccessAllowed = userAccessControlModel.IsAllowedShiftSchedule;
                            break;
                        case 8:
                            // Approve Reason
                            functionID = "AR";
                            isAccessAllowed = userAccessControlModel.IsAllowedApproveReason;
                            break;
                        case 9:
                            // Print Report
                            functionID = "PR";
                            isAccessAllowed = userAccessControlModel.IsAllowedPrintReport;
                            break;
                        case 10:
                            // Print Report
                            functionID = "CU";
                            isAccessAllowed = userAccessControlModel.IsAllowedContactUs;
                            break;
                    }

                    string sql = $@"SELECT COUNT(*) FROM Access";
                    sql += " " + $@"WHERE RoleID='{userAccessControlModel.RoleID}' AND FunctionID='{functionID}'";
                    
                    cmd = new SqlCommand(sql, conn);
                    
                    if (! ((Int32)cmd.ExecuteScalar()).Equals(0))
                    {
                        isFound = true;
                    }

                
                    if (! isFound.Equals(true))
                    {
                        sql = $@"INSERT INTO Access";
                        sql += " " + $@"(RoleID, FunctionID, IsAccessAllowed) VALUES";                       
                        sql += " " + $@"('{userAccessControlModel.RoleID}', '{functionID}', '{isAccessAllowed}')";

                    }
                    else
                    {
                        sql = $@"UPDATE Access";
                        sql += " " + $@"SET IsAccessAllowed='{isAccessAllowed}'";
                        sql += " " + $@"WHERE RoleID='{userAccessControlModel.RoleID}' AND FunctionID='{functionID}'";
                    }

                
                    cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;

                        if (! isFound.Equals(true))
                        {
                            logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "User Access Control", $@"Create; {userAccessControlModel.RoleID}, {functionID}, {isAccessAllowed}", DateTime.Now);

                        }
                        else
                        {
                            logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "User Access Control", $@"Update; {userAccessControlModel.RoleID}, {functionID}, {isAccessAllowed}", DateTime.Now);

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

            return status;

        }
    }
}