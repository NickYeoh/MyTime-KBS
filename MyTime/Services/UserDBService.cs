using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using MyTime.ViewModels;
using System.Data;
using System.Linq;
using System.Web;
using System.IO;

namespace MyTime.Services
{
    public class UserDBService
    {
        //public enum Access
        //{
        //    [Description("Dashboard")]
        //    Dashboard = 0,

        //    [Description("Holiday")]
        //    Holiday = 1,

        //    [Description("System Setting")]
        //    SystemSetting = 2
        //}

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        private readonly static string connDeviceStr = ConfigurationManager.ConnectionStrings["DeviceDB"].ConnectionString;
        private readonly SqlConnection connDevice = new SqlConnection(connDeviceStr);

        LogActivityDBService logActivityDBService = new LogActivityDBService();

        ShiftScheduleDBService shiftScheduleDBService = new ShiftScheduleDBService();
        ApproverDBService approverDBService = new ApproverDBService();
        ReportAdminDBService reportAdminDBService = new ReportAdminDBService();


        public List<UserModel> ListUser()
        {
            UserModel userModel;
            List<UserModel> dataList = new List<UserModel>();

            try
            {
                //string sql = $@"SELECT * FROM [User] U LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID LEFT JOIN Unit UT ON U.UnitID = UT.UnitID LEFT JOIN Role R ON R.RoleID= U.RoleID ORDER BY NRIC ASC";

                string sql = $@"SELECT U.NRIC, U.USRID, U.UserName, U.Gender,";
                sql += " " + $@"U.RoleID, R.RoleName,";

                sql += " " + $@"(SELECT TOP 1 (ShiftSchedule.ShiftID) FROM ShiftSchedule";
                sql += " " + $@"LEFT JOIN Shift ON Shift.ShiftID = ShiftSchedule.ShiftID";
                sql += " " + $@"WHERE NRIC = U.NRIC AND EffectiveOn <= GETDATE()";
                sql += " " + $@"ORDER BY EffectiveOn DESC) AS ShiftID,";

                sql += " " + $@"(SELECT TOP 1 (Shift.ShiftName) FROM ShiftSchedule";
                sql += " " + $@"LEFT JOIN Shift ON Shift.ShiftID = ShiftSchedule.ShiftID";
                sql += " " + $@"WHERE NRIC = U.NRIC AND EffectiveOn <= GETDATE()";
                sql += " " + $@"ORDER BY EffectiveOn DESC) AS ShiftName,";

                sql += " " + $@"U.ContactNo, U.Email, U.DepartmentID, D.DepartmentName,";
                sql += " " + $@"U.UnitID, UT.UnitName, U.Designation,";
                sql += " " + $@"U.Grade, U.IsResigned, U.ResignedOn,";
                sql += " " + $@"U.AccessRoleID, A.AccessRoleName, U.IsAttendanceExcluded,";

                // 2022-03-11 : Attendance Card Status
                //sql += " " + $@"AC.EffectiveOn, IIF(AC.AttendanceCardStatus IS NULL, 'YL',AC.AttendanceCardStatus) AS AttendanceCardStatus";

                // 2022-06-25 : Attendance Card Status
                sql += " " + $@"IIF(ACT.AttendanceCardStatus IS NULL, 'YL',ACT.AttendanceCardStatus) AS AttendanceCardStatus";

                sql += " " + $@"FROM [User] U";
                sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
                sql += " " + $@"LEFT JOIN Unit UT ON U.UnitID = UT.UnitID";
                sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
                sql += " " + $@"LEFT JOIN AccessRole A ON A.AccessRoleID = U.AccessRoleID";
               
                //sql += " " + $@"LEFT JOIN (SELECT TOP 1  * FROM [AttendanceCard]";
                //sql += " " + $@"ORDER BY YearMonth DESC) AC ON AC.NRIC = U.NRIC";

                //sql += " " + $@"OUTER APPLY  (";
                //sql += " " + $@"SELECT TOP 1 * FROM [AttendanceCard]";
                //sql += " " + $@"WHERE [AttendanceCard].NRIC = u.NRIC";
                //sql += " " + $@"ORDER BY EffectiveOn DESC";
                //sql += " " + $@") AS AC";

                sql += " " + $@"OUTER APPLY (";
                sql += " " + $@"SELECT TOP 1 AttendanceCardStatus FROM [AttendanceCard]";
                sql += " " + $@"WHERE [AttendanceCard].NRIC = U.NRIC";
                sql += " " + $@"AND CONVERT(INT,  [AttendanceCard].AttendanceMonth) < CONVERT(INT, FORMAT(GETDATE(), 'yyyyMM'))";
                sql += " " + $@"ORDER BY AttendanceMonth DESC) AS ACT";

                sql += " " + $@"ORDER BY NRIC ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userModel = new UserModel();

                        userModel.NRIC = dr["NRIC"].ToString();

                        if (!dr["USRID"].Equals(DBNull.Value))
                        {
                            userModel.USRID = dr["USRID"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            userModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["Gender"].Equals(DBNull.Value))
                        {
                            userModel.Gender = dr["Gender"].ToString();
                        }

                        if (!dr["ContactNo"].Equals(DBNull.Value))
                        {
                            userModel.ContactNo = dr["ContactNo"].ToString();
                        }

                        if (!dr["Email"].Equals(DBNull.Value))
                        {
                            userModel.Email = dr["Email"].ToString();
                        }

                        if (!dr["DepartmentID"].Equals(DBNull.Value))
                        {
                            userModel.DepartmentID = dr["DepartmentID"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            userModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UnitID"].Equals(DBNull.Value))
                        {
                            userModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            userModel.UnitName = dr["UnitName"].ToString();
                        }

                        if (!dr["Designation"].Equals(DBNull.Value))
                        {
                            userModel.Designation = dr["Designation"].ToString();
                        }

                        if (!dr["Grade"].Equals(DBNull.Value))
                        {
                            userModel.Grade = dr["Grade"].ToString();
                        }

                        if (!dr["RoleID"].Equals(DBNull.Value))
                        {
                            userModel.RoleID = Convert.ToInt32(dr["RoleID"]);
                        }

                        if (!dr["RoleName"].Equals(DBNull.Value))
                        {
                            userModel.RoleName = dr["RoleName"].ToString();
                        }

                        if (!dr["ShiftID"].Equals(DBNull.Value))
                        {
                            userModel.ShiftID = dr["ShiftID"].ToString();
                        }

                        if (!dr["ShiftName"].Equals(DBNull.Value))
                        {
                            userModel.ShiftName = dr["ShiftName"].ToString();
                        }

                        if (!dr["IsResigned"].Equals(DBNull.Value))
                        {
                            userModel.IsResigned = Convert.ToBoolean(dr["IsResigned"]);
                        }

                        if (!dr["ResignedOn"].Equals(DBNull.Value))
                        {
                            userModel.ResignedOn = Convert.ToDateTime(dr["ResignedOn"]);
                        }

                        if (!dr["ShiftName"].Equals(DBNull.Value))
                        {
                            userModel.ShiftName = dr["ShiftName"].ToString();
                        }

                        if (!dr["AccessRoleID"].Equals(DBNull.Value))
                        {
                            userModel.AccessRoleID = Convert.ToInt32(dr["AccessRoleID"]);
                        }

                        if (!dr["AccessRoleName"].Equals(DBNull.Value))
                        {
                            userModel.AccessRoleName = dr["AccessRoleName"].ToString();
                        }


                        if (!dr["IsAttendanceExcluded"].Equals(DBNull.Value))
                        {
                            userModel.IsAttendanceExcluded = Convert.ToBoolean(dr["IsAttendanceExcluded"]);
                        }


                        if (!dr["AttendanceCardStatus"].Equals(DBNull.Value))
                        {
                            userModel.AttendanceCardStatus = dr["AttendanceCardStatus"].ToString();
                        }

                        dataList.Add(userModel);

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


        public bool Create(UserModel userModel)
        {

            SystemDBService systemDBService = new SystemDBService();

            SystemModel systemSettingModel = new SystemModel();
            systemSettingModel = systemDBService.GetData();

            bool status = false;

            //Default Value

            string password = HashPassword(userModel.NRIC);

            if (userModel.UnitID == 0)
            {
                userModel.UnitID = null;
            }

            //userModel.RoleID = systemSettingModel.DefaultRoleID;
            //userModel.RoleName = systemSettingModel.DefaultRoleName;

            //userModel.ShiftID = systemSettingModel.DefaultShiftID;
            //userModel.ShiftName = systemSettingModel.DefaultShiftName;

            userModel.IsResigned = false;
            userModel.ResignedOn = null;
            userModel.IsAttendanceExcluded = false;

            string effectiveOn = "20200101";

            try
            {
                string sql = $@"INSERT INTO [User]";
                sql += " " + " (NRIC, UserName, Gender, ContactNo,";
                sql += " " + "Email, RoleID, DepartmentID, UnitID,";
                sql += " " + "Grade, IsResigned, ResignedOn, Password, USRID, AccessRoleID, IsAttendanceExcluded)";
                sql += " " + "VALUES";
                sql += " " + $@"('{userModel.NRIC}', '{userModel.UserName}', '{userModel.Gender}', '{userModel.ContactNo}',";
                sql += " " + $@"'{userModel.Email}', '{userModel.RoleID}', '{userModel.DepartmentID}', '{userModel.UnitID}',";
                sql += " " + $@"'{userModel.Grade}', '{userModel.IsResigned}','{userModel.ResignedOn}', '{password}',";
                sql += " " + $@"'{userModel.USRID}', '{userModel.AccessRoleID}', '{userModel.IsAttendanceExcluded}')";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {

                    string logData = $@"{userModel.NRIC}, {userModel.UserName}, {userModel.Gender}, {userModel.ContactNo},";
                    logData += " " + $@"{userModel.Email}, {userModel.RoleID}, {userModel.DepartmentID}, {userModel.UnitID},";
                    logData += " " + $@"{userModel.Grade}, {userModel.IsResigned},{userModel.ResignedOn}, {password}, {userModel.USRID}, {userModel.AccessRoleID}, {userModel.IsAttendanceExcluded}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "User", $@"Create; {logData}", DateTime.Now);

                    sql = $@"INSERT INTO ShiftSchedule";
                    sql += " " + " (NRIC, ShiftID, EffectiveOn)";
                    sql += " " + "VALUES";
                    sql += " " + $@"('{userModel.NRIC}', '{userModel.ShiftID}', '{effectiveOn}' )";

                    cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;

                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Shift Schedule", $@"Create; {userModel.NRIC}, {userModel.ShiftID}, {effectiveOn}", DateTime.Now);

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

        public bool Update(UserModel userModel)
        {
            bool status = false;

            if (userModel.UnitID == 0)
            {
                userModel.UnitID = null;
            }

            try
            {
                string sql = $@"UPDATE [User] ";
                sql += " " + $@"SET UserName='{userModel.UserName}', Gender='{userModel.Gender}', ContactNo='{userModel.ContactNo}',";
                sql += " " + $@"Email='{userModel.Email}', RoleID='{userModel.RoleID}', DepartmentID='{userModel.DepartmentID}', UnitID='{userModel.UnitID}',";
                sql += " " + $@"Grade='{userModel.Grade}', IsResigned='{userModel.IsResigned}',";
                if (userModel.IsResigned == false)
                {
                    sql += " " + $@"ResignedOn='{DBNull.Value}',";
                }
                else
                {
                    sql += " " + $@"ResignedOn='{userModel.ResignedOn?.ToString("yyyyMMdd")}',";
                }

                sql += " " + $@"USRID='{userModel.USRID}', AccessRoleID='{userModel.AccessRoleID}',";
                sql += " " + $@"IsAttendanceExcluded='{userModel.IsAttendanceExcluded}'";
                sql += " " + $@"WHERE NRIC='{userModel.NRIC}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{userModel.UserName}, {userModel.Gender}, {userModel.ContactNo},";
                    logData += " " + $@"{userModel.Email}, {userModel.RoleID}, {userModel.DepartmentID}, {userModel.UnitID},";
                    logData += " " + $@"{userModel.Grade}, {userModel.IsResigned},{userModel.ResignedOn}, {userModel.USRID}, {userModel.AccessRoleID}, {userModel.IsAttendanceExcluded}, {userModel.NRIC}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "User", $@"Delete; {logData}", DateTime.Now);
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

        public bool UpdateAttendanceCard(string NRIC, string effectiveOn, string attendanceCardStatus)
        {
            bool status = false;

            //// convert to Date
            //DateTime startOn;
            //DateTime.TryParse(monthYear, out startOn);
            //string yearMonth = startOn.ToString("yyyyMM");


            // Delete
            try
            {
                string sql = $@"DELETE FROM AttendanceCard";
                sql += " " + $@"WHERE NRIC='{NRIC}' AND EffectiveOn='{effectiveOn}'";


                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.ExecuteNonQuery();

                conn.Close();

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

            // Insert
            try
            {
                string sql = $@"INSERT INTO AttendanceCard";
                sql += " " + $@"(NRIC, EffectiveOn, AttendanceCardStatus)";
                sql += " " + "VALUES";
                sql += " " + $@"('{NRIC}', '{effectiveOn}' , '{attendanceCardStatus}')";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    string logData = $@"{NRIC},  {effectiveOn}, {attendanceCardStatus}";

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "User", $@"Update Attendance Card; {logData}", DateTime.Now);
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

        public bool DeleteAttendanceCard(string NRIC, string effectiveOn)
        {
            bool status = false;

            // Delete
            try
            {
                string sql = $@"DELETE FROM AttendanceCard";
                sql += " " + $@"WHERE NRIC='{NRIC}' AND effectiveOn = '{effectiveOn}'";


                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.ExecuteNonQuery();

                conn.Close();

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


        public bool Delete(UserModel userModel)
        {

            bool status = false;

            string NRIC = userModel.NRIC;

            ApproverUserModel approverUserModel = new ApproverUserModel();
            ReportAdminModel reportAdminModel = new ReportAdminModel();

            List<ApproverModel> approverList = new List<ApproverModel>();
            List<ReportAdminModel> reportAdminList = new List<ReportAdminModel>();

            try
            {
                approverList = approverDBService.ListApprover();
                reportAdminList = reportAdminDBService.ListReportAdmin();

                if (approverList.Where(a => a.ApproverNRIC == NRIC).ToList().Count.Equals(0))
                {
                    if (reportAdminList.Where(ra => ra.ReportAdminNRIC == NRIC).ToList().Count.Equals(0))
                    {

                        string sql = $@"DELETE [User] WHERE NRIC='{userModel.NRIC}'";

                        conn.Open();

                        SqlCommand cmd = new SqlCommand(sql, conn);

                        if (!cmd.ExecuteNonQuery().Equals(0))
                        {
                            string logData = $@"{userModel.NRIC}, {userModel.UserName}";

                            logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "User", $@"Delete; {logData}", DateTime.Now);

                            status = shiftScheduleDBService.Delete(userModel.NRIC);

                            //status = true;
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

        public bool DeletePassword(UserModel userModel)
        {
            bool status = false;

            try
            {

                string password = HashPassword(userModel.NRIC);

                string sql = $@"UPDATE [User] ";
                sql += " " + $@"SET Password='{password}'";
                sql += " " + $@"WHERE NRIC='{userModel.NRIC}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "User", $@"Delete Password; {userModel.NRIC}", DateTime.Now);
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

        public bool ChangePassword(UserViewModel userViewModel)
        {
            bool status = false;

            try
            {
                string password = HashPassword(userViewModel.ChangePasswordModel.PasswordNew);

                string sql = $@"UPDATE [User] ";
                sql += " " + $@"SET Password='{password}'";
                sql += " " + $@"WHERE NRIC='{userViewModel.UserModel.NRIC}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "User", $@"Change Password; {password}, {userViewModel.UserModel.NRIC}", DateTime.Now);
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

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "admin118";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (System.IO.MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        public Boolean AuthUser(AuthModel authModel)
        {
            Boolean isValid = false;

            string sql = $@"SELECT NRIC, Password FROM [User] WHERE NRIC='{authModel.NRIC.Trim()}'";
            string password = "";
            string nric = "";

            try
            {

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        nric = dr["NRIC"].ToString();

                        if (!dr["Password"].Equals(DBNull.Value) && !dr["Password"].Equals(""))
                        {
                            password = dr["Password"].ToString();

                            if (HashPassword(authModel.Password.Trim()).Equals(password))
                            {
                                FormsAuthentication.SetAuthCookie(nric, false);
                                isValid = true;

                            }
                        }
                        else
                        {
                            if (HashPassword(authModel.Password).Equals(HashPassword("abc123")) || HashPassword(authModel.Password).Equals(HashPassword(authModel.NRIC)))
                            {
                                FormsAuthentication.SetAuthCookie(nric, false);
                                isValid = true;
                            }
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

            return isValid;

        }

        public Boolean CheckIsUserExist(string ID)
        {
            Boolean isExisted = false;

            string sql = $@"SELECT * FROM [User] WHERE NRIC='{ID}'";

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    isExisted = true;
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

            return isExisted;

        }


        public UserModel GetJohnsonControlUserDataByID(string ID)
        {

            UserModel userModel = new UserModel();

            //cardholder + badge
            // ch.c_nick_name as NRIC
            string sql = $@"SELECT ch.c_id AS USRID";
            sql += " " + $@", ch.c_lname";
            sql += " " + $@", ch.c_mname";
            sql += " " + $@", ch.c_fname";
            sql += " " + $@", ch.c_nick_name";
            sql += " " + $@", b.b_number_str";
            sql += " " + $@"FROM cardholder ch";
            sql += " " + $@"LEFT JOIN  badge b";
            sql += " " + $@"ON b.b_cardholder_id = ch.c_id";
            sql += " " + $@"WHERE b.b_disabled = '0'";
            //sql += " " + $@"AND ch.c_addr = '{ID}'";
            sql += " " + $@"AND REPLACE(ch.c_nick_name, '-','') = '{ID}'";

            try
            {
                connDevice.Open();

                SqlCommand cmd = new SqlCommand(sql, connDevice);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        userModel.NRIC = ID;

                        if (!dr["USRID"].Equals(DBNull.Value))
                        {
                            userModel.USRID = dr["USRID"].ToString();
                        }

                        if (!dr["c_fname"].Equals(DBNull.Value))
                        {
                            if (!dr["c_lname"].Equals(DBNull.Value))
                            {
                                if (!dr["c_mname"].Equals(DBNull.Value))
                                {
                                    userModel.UserName = string.Format("{0} {1} {2}", dr["c_fname"].ToString().Trim(), dr["c_mname"].ToString().Trim(), dr["c_lname"].ToString().Trim());
                                }
                                else
                                {
                                    userModel.
                                        UserName = string.Format("{0} {1}", dr["c_fname"].ToString().Trim(), dr["c_lname"].ToString().Trim());

                                }
                            }
                            else
                            {
                                userModel.UserName = dr["c_fname"].ToString().Trim();
                            }

                        }

                        // Default to device user
                        userModel.RoleID = 0;
                        userModel.RoleName = "-";

                        userModel.ShiftID = "";
                        userModel.ShiftName = "-";

                        userModel.Gender = "Lelaki";

                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                if (connDevice.State == ConnectionState.Open)
                {
                    connDevice.Close();
                }

            }

            return userModel;

        }

        public List<UserModel> GetJohnsonControlUserList()
        {

            List<UserModel> userList = new List<UserModel>();
            userList = ListUser();

            string nric = "";

            List<UserModel> deviceUserList = new List<UserModel>();
            UserModel userModel;

            //cardholder + badge
            // ch.c_nick_name as NRIC
            string sql = $@"SELECT ch.c_id AS USRID";
            sql += " " + $@", ch.c_lname";
            sql += " " + $@", ch.c_mname";
            sql += " " + $@", ch.c_fname";
            sql += " " + $@", LTRIM(RTRIM(REPLACE(ch.c_nick_name, '-',''))) AS NRIC";
            sql += " " + $@", b.b_number_str";
            sql += " " + $@"FROM cardholder ch";
            sql += " " + $@"LEFT JOIN  badge b";
            sql += " " + $@"ON b.b_cardholder_id = ch.c_id";
            sql += " " + $@"WHERE b.b_disabled = '0'";
            sql += " " + $@"AND LEN(ch.c_nick_name ) >= 6";

            // NRIC len = min 6 Digit

            try
            {
                connDevice.Open();

                SqlCommand cmd = new SqlCommand(sql, connDevice);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {

                        if (!dr["NRIC"].Equals(DBNull.Value))
                        {
                            nric = dr["NRIC"].ToString().Trim();
                        }


                        if ((userList.Where(u => u.NRIC == nric).Count()) == 0)
                        {
                            // not found in MyTime


                            userModel = new UserModel();

                            userModel.NRIC = nric;

                            if (!dr["USRID"].Equals(DBNull.Value))
                            {
                                userModel.USRID = dr["USRID"].ToString();
                            }

                            if (!dr["c_fname"].Equals(DBNull.Value))
                            {
                                if (!dr["c_lname"].Equals(DBNull.Value))
                                {
                                    if (!dr["c_mname"].Equals(DBNull.Value))
                                    {
                                        userModel.UserName = string.Format("{0} {1} {2}", dr["c_fname"].ToString().Trim(), dr["c_mname"].ToString().Trim(), dr["c_lname"].ToString().Trim());
                                    }
                                    else
                                    {
                                        userModel.UserName = string.Format("{0} {1}", dr["c_fname"].ToString().Trim(), dr["c_lname"].ToString().Trim());

                                    }
                                }
                                else
                                {
                                    userModel.UserName = dr["c_fname"].ToString().Trim();
                                }

                            }

                            // Default to device user
                            userModel.RoleID = 0;
                            userModel.RoleName = "-";

                            userModel.ShiftID = "";
                            userModel.ShiftName = "-";

                            userModel.Gender = "Lelaki";

                            deviceUserList.Add(userModel);

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
                if (connDevice.State == ConnectionState.Open)
                {
                    connDevice.Close();
                }

            }

            return deviceUserList;

        }


        public UserModel GetSupremaUserDataByID(string ID)
        {

            UserModel userModel = new UserModel();

            string sql = $@"SELECT * FROM T_USR WHERE PH='{ID}' AND DEL='N'";

            try
            {
                connDevice.Open();

                SqlCommand cmd = new SqlCommand(sql, connDevice);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        userModel.NRIC = ID;


                        if (!dr["USRID"].Equals(DBNull.Value))
                        {
                            userModel.USRID = dr["USRID"].ToString();
                        }

                        if (!dr["NM"].Equals(DBNull.Value))
                        {
                            userModel.UserName = dr["NM"].ToString();
                        }


                        // Default to device user
                        userModel.RoleID = 0;
                        userModel.RoleName = "-";

                        userModel.ShiftID = "";
                        userModel.ShiftName = "-";

                        userModel.Gender = "Lelaki";

                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                if (connDevice.State == ConnectionState.Open)
                {
                    connDevice.Close();
                }

            }

            return userModel;

        }

        public List<UserModel> GetSupremaUserList()
        {
            List<UserModel> userList = new List<UserModel>();
            userList = ListUser();

            string nric = "";

            List<UserModel> deviceUserList = new List<UserModel>();
            UserModel userModel;

            string sql = $@"SELECT * FROM T_USR WHERE DEL='N' AND ISUSRGR='N' AND PH <> '' ";

            try
            {
                connDevice.Open();

                SqlCommand cmd = new SqlCommand(sql, connDevice);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        if (!dr["PH"].Equals(DBNull.Value))
                        {
                            nric = dr["PH"].ToString().Trim();
                        }

                        if (userList.Where(u => u.NRIC == nric).Count().Equals(0))
                        {
                            userModel = new UserModel();

                            userModel.NRIC = nric;

                            if (!dr["USRID"].Equals(DBNull.Value))
                            {
                                userModel.USRID = dr["USRID"].ToString();
                            }

                            if (!dr["NM"].Equals(DBNull.Value))
                            {
                                userModel.UserName = dr["NM"].ToString();
                            }


                            // Default to device user
                            userModel.RoleID = 0;
                            userModel.RoleName = "-";

                            userModel.ShiftID = "";
                            userModel.ShiftName = "-";

                            userModel.Gender = "Lelaki";

                            deviceUserList.Add(userModel);

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
                if (connDevice.State == ConnectionState.Open)
                {
                    connDevice.Close();
                }

            }

            return deviceUserList;

        }

        public string GetAttendanceCardStatusByIDAndMonth(string ID, DateTime startOn)
        {
            string attendanceCardStatus = "";

            //string sql = " " + $@"SELECT TOP 1 IIF(AttendanceCardStatus IS NULL, 'YL', AttendanceCardStatus) AS AttendanceCardStatus FROM [AttendanceCard]";
            //sql += " " + $@"WHERE [AttendanceCard].NRIC = {ID}";
            //sql += " " + $@"AND FORMAT(EffectiveOn, 'yyyyMM') <= '{startOn.ToString("yyyyMM")}'";
            //sql += " " + $@"ORDER BY EffectiveOn DESC";

            string sql =$@"SELECT TOP 1 AttendanceCardStatus FROM [AttendanceCard]";
            sql += " " + $@"WHERE [AttendanceCard].NRIC = {ID}";
            sql += " " + $@"AND CONVERT(INT,AttendanceMonth) < CONVERT(INT, {startOn.ToString("yyyyMM")})";
            sql += " " + $@"ORDER BY AttendanceMonth DESC";


            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);


                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        if (!dr["AttendanceCardStatus"].Equals(DBNull.Value))
                        {
                            attendanceCardStatus = dr["AttendanceCardStatus"].ToString();
                        }

                    }
                }
                else
                {
                    // Default to Yellow
                    attendanceCardStatus = "YL";
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


            return attendanceCardStatus;
        }


        public UserModel GetDataByID(string ID)
        {
            UserModel userModel = new UserModel();

            string sql = $@"SELECT U.NRIC, U.USRID, U.UserName, U.Gender,";
            sql += " " + $@"U.RoleID, R.RoleName,";

            sql += " " + $@"(SELECT TOP 1 (ShiftSchedule.ShiftID) FROM ShiftSchedule";
            sql += " " + $@"LEFT JOIN Shift ON Shift.ShiftID = ShiftSchedule.ShiftID";
            sql += " " + $@"WHERE NRIC = '{ID}' AND EffectiveOn <= GETDATE()";
            sql += " " + $@"ORDER BY EffectiveOn DESC) AS ShiftID,";

            sql += " " + $@"(SELECT TOP 1 (Shift.ShiftName) FROM ShiftSchedule";
            sql += " " + $@"LEFT JOIN Shift ON Shift.ShiftID = ShiftSchedule.ShiftID";
            sql += " " + $@"WHERE NRIC = '{ID}' AND EffectiveOn <= GETDATE()";
            sql += " " + $@"ORDER BY EffectiveOn DESC) AS ShiftName,";

            sql += " " + $@"U.ContactNo, U.Email, U.DepartmentID, D.DepartmentName,";
            sql += " " + $@"U.UnitID, UT.UnitName, U.Designation,";
            sql += " " + $@"U.Grade, U.IsResigned, U.ResignedOn,";
            sql += " " + $@"U.AccessRoleID, A.AccessRoleName, U.IsAttendanceExcluded,";

            // 2022-06-23 : Attendance Card Status
            //sql += " " + $@"ACT.EffectiveOn, IIF(ACT.AttendanceCardStatus IS NULL, 'YL',ACT.AttendanceCardStatus) AS AttendanceCardStatus";
            sql += " " + $@"IIF(ACT.AttendanceCardStatus IS NULL, 'YL',ACT.AttendanceCardStatus) AS AttendanceCardStatus";

            sql += " " + $@"FROM [User] U";

            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID = U.DepartmentID";
            sql += " " + $@"LEFT JOIN Unit UT ON U.UnitID = UT.UnitID";
            sql += " " + $@"LEFT JOIN Role R ON R.RoleID = U.RoleID";
            sql += " " + $@"LEFT JOIN AccessRole A ON A.AccessRoleID = U.AccessRoleID";

            //sql += " " + $@"OUTER APPLY  (";
            //sql += " " + $@"SELECT TOP 1 * FROM [AttendanceCard]";
            //sql += " " + $@"WHERE [AttendanceCard].NRIC = U.NRIC";
            //sql += " " + $@"ORDER BY EffectiveOn DESC";
            //sql += " " + $@") AS AC";

            sql += " " + $@"OUTER APPLY (";
            sql += " " + $@"SELECT TOP 1 AttendanceCardStatus FROM [AttendanceCard]";
            sql += " " + $@"WHERE [AttendanceCard].NRIC = U.NRIC";
            sql += " " + $@"AND CONVERT(INT,  [AttendanceCard].AttendanceMonth) < CONVERT(INT, FORMAT(GETDATE(), 'yyyyMM'))";
            sql += " " + $@"ORDER BY AttendanceMonth DESC) AS ACT";

            sql += " " + $@"WHERE U.NRIC = '{ID}'";

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {


                    while (dr.Read())
                    {

                        userModel.NRIC = dr["NRIC"].ToString();

                        if (!dr["USRID"].Equals(DBNull.Value))
                        {
                            userModel.USRID = dr["USRID"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            userModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["Gender"].Equals(DBNull.Value))
                        {
                            userModel.Gender = dr["Gender"].ToString();
                        }
                        else
                        {
                            userModel.Gender = "Lelaki";
                        }

                        if (!dr["ContactNo"].Equals(DBNull.Value))
                        {
                            userModel.ContactNo = dr["ContactNo"].ToString();
                        }

                        if (!dr["Email"].Equals(DBNull.Value))
                        {
                            userModel.Email = dr["Email"].ToString();
                        }

                        if (!dr["RoleID"].Equals(DBNull.Value))
                        {
                            userModel.RoleID = Convert.ToInt32(dr["RoleID"]);
                        }

                        if (!dr["RoleName"].Equals(DBNull.Value))
                        {
                            userModel.RoleName = dr["RoleName"].ToString();
                        }

                        if (!dr["ShiftID"].Equals(DBNull.Value))
                        {
                            userModel.ShiftID = dr["ShiftID"].ToString();
                        }

                        if (!dr["ShiftName"].Equals(DBNull.Value))
                        {
                            userModel.ShiftName = dr["ShiftName"].ToString();
                        }

                        if (!dr["DepartmentID"].Equals(DBNull.Value))
                        {
                            userModel.DepartmentID = dr["DepartmentID"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            userModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UnitID"].Equals(DBNull.Value))
                        {
                            userModel.UnitID = Convert.ToInt32(dr["UnitID"]);
                        }

                        if (!dr["UnitName"].Equals(DBNull.Value))
                        {
                            userModel.UnitName = dr["UnitName"].ToString();
                        }

                        if (!dr["Designation"].Equals(DBNull.Value))
                        {
                            userModel.Designation = dr["Designation"].ToString();
                        }

                        if (!dr["Grade"].Equals(DBNull.Value))
                        {
                            userModel.Grade = dr["Grade"].ToString();
                        }

                        if (!dr["IsResigned"].Equals(DBNull.Value))
                        {
                            userModel.IsResigned = Convert.ToBoolean(dr["IsResigned"]);
                        }

                        if (userModel.IsResigned.Equals(false))
                        {
                            userModel.ResignedOn = DateTime.Now;
                        }
                        else
                        {
                            if (!dr["ResignedOn"].Equals(DBNull.Value))
                            {
                                userModel.ResignedOn = Convert.ToDateTime(dr["ResignedOn"]);
                            }
                        }

                        if (!dr["AccessRoleID"].Equals(DBNull.Value))
                        {
                            userModel.AccessRoleID = Convert.ToInt32(dr["AccessRoleID"]);
                        }

                        if (!dr["AccessRoleName"].Equals(DBNull.Value))
                        {
                            userModel.AccessRoleName = dr["AccessRoleName"].ToString();
                        }

                        if (!dr["IsAttendanceExcluded"].Equals(DBNull.Value))
                        {
                            userModel.IsAttendanceExcluded = Convert.ToBoolean(dr["IsAttendanceExcluded"]);
                        }

                        if (!dr["AttendanceCardStatus"].Equals(DBNull.Value))
                        {
                            userModel.AttendanceCardStatus = dr["AttendanceCardStatus"].ToString();
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

            return userModel;
        }


        public List<AttendanceCardModel> GetAttendanceCardByID(string ID)
        {
            List<AttendanceCardModel> attendanceCardList = new List<AttendanceCardModel>();
            AttendanceCardModel attendanceCardModel = new AttendanceCardModel();

            string sql = $@"SELECT AttendanceMonth, DepartmentName, ";
            sql += " " + $@"AC.NRIC, U.UserName,";
            sql += " " + $@"LateInCount, EarlyOutCount,";
            sql += " " + $@"LateInEarlyOutCount, IncompleteCount,";
            sql += " " + $@"AbsentCount, AttendCount,";
            sql += " " + $@"OnLeaveCount, TotalAttendanceIssue,";
            sql += " " + $@"AttendanceCardStatus AC";
            sql += " " + $@"LEFT JOIN [User] U ON U.NRIC=AC.NRIC";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID=U.DepartmentID";
            sql += " " + $@"FROM AttendanceCard";
            sql += " " + $@"WHERE NRIC='{ID}'";
            sql += " " + $@"ORDER BY NRIC";


            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        attendanceCardModel = new AttendanceCardModel();

                        if (!dr["AttendanceMonth"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendanceMonth = dr["AttendanceMonth"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["NRIC"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.NRIC = dr["NRIC"].ToString();
                        }

                        if (!dr["LateInCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.LateInCount = Convert.ToInt32(dr["LateInCount"]);
                        }

                        if (!dr["EarlyOutCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.EarlyOutCount = Convert.ToInt32(dr["EarlyOutCount"]);
                        }

                        if (!dr["LateInEarlyOutCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.LateInEarlyOutCount = Convert.ToInt32(dr["LateInEarlyOutCount"]);
                        }

                        if (!dr["IncompleteCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.IncompleteCount = Convert.ToInt32(dr["IncompleteCount"]);
                        }

                        if (!dr["AbsentCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AbsentCount = Convert.ToInt32(dr["AbsentCount"]);
                        }

                        if (!dr["AttendCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendCount = Convert.ToInt32(dr["AttendCount"]);
                        }

                        if (!dr["OnLeaveCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.OnLeaveCount = Convert.ToInt32(dr["OnLeaveCount"]);
                        }

                        if (!dr["TotalAttendanceIssue"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.TotalAttendanceIssue= Convert.ToInt32(dr["TotalAttendanceIssue"]);
                        }

                        if (!dr["AttendanceCardStatus"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendanceCardStatus = dr["AttendanceCardStatus"].ToString();
                        }

                        attendanceCardList.Add(attendanceCardModel);

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

            return attendanceCardList;
        }

        public List<AttendanceCardModel> GetAttendanceCardByAttendanceCardStatusAndMonth( string attendanceMonth, string attendanceCardStatus)
        {
            List<AttendanceCardModel> attendanceCardList = new List<AttendanceCardModel>();
            AttendanceCardModel attendanceCardModel = new AttendanceCardModel();

            string sql = $@"SELECT AttendanceMonth, DepartmentName, ";
            sql += " " + $@"AC.NRIC, U.UserName,";
            sql += " " + $@"LateInCount, EarlyOutCount,";
            sql += " " + $@"LateInEarlyOutCount, IncompleteCount,";
            sql += " " + $@"AbsentCount, AttendCount,";
            sql += " " + $@"OnLeaveCount, TotalAttendanceIssue,";
            sql += " " + $@"AttendanceCardStatus AC";
            sql += " " + $@"LEFT JOIN [User] U ON U.NRIC=AC.NRIC";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID=U.DepartmentID";
            sql += " " + $@"FROM AttendanceCard";
            sql += " " + $@"WHERE AttendanceMonth='{attendanceMonth}'";
            sql += " " + $@"AND AttendanceCardStatus='{attendanceCardStatus}'";
            sql += " " + $@"ORDER BY AttendanceMonth, DepartmentName, UserName";


            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        attendanceCardModel = new AttendanceCardModel();
                                                                       

                        if (!dr["AttendanceMonth"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendanceMonth = dr["AttendanceMonth"].ToString();
                        }

                        if (!dr["DepartmentName"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.DepartmentName = dr["DepartmentName"].ToString();
                        }

                        if (!dr["UserName"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.UserName = dr["UserName"].ToString();
                        }

                        if (!dr["NRIC"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.NRIC = dr["NRIC"].ToString();
                        }

                        if (!dr["LateInCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.LateInCount = Convert.ToInt32(dr["LateInCount"]);
                        }

                        if (!dr["EarlyOutCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.EarlyOutCount = Convert.ToInt32(dr["EarlyOutCount"]);
                        }

                        if (!dr["LateInEarlyOutCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.LateInEarlyOutCount = Convert.ToInt32(dr["LateInEarlyOutCount"]);
                        }

                        if (!dr["IncompleteCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.IncompleteCount = Convert.ToInt32(dr["IncompleteCount"]);
                        }

                        if (!dr["AbsentCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AbsentCount = Convert.ToInt32(dr["AbsentCount"]);
                        }

                        if (!dr["AttendCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendCount = Convert.ToInt32(dr["AttendCount"]);
                        }

                        if (!dr["OnLeaveCount"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.OnLeaveCount = Convert.ToInt32(dr["OnLeaveCount"]);
                        }

                        if (!dr["TotalAttendanceIssue"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.TotalAttendanceIssue = Convert.ToInt32(dr["TotalAttendanceIssue"]);
                        }

                        if (!dr["AttendanceCardStatus"].Equals(DBNull.Value))
                        {
                            attendanceCardModel.AttendanceCardStatus = dr["AttendanceCardStatus"].ToString();
                        }

                        attendanceCardList.Add(attendanceCardModel);

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

            return attendanceCardList;
        }


        public string HashPassword(string Password)
        {
            string saltKey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
            string saltAndPassword = string.Concat(Password, saltKey);

            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();

            byte[] passwordData = Encoding.Default.GetBytes(saltAndPassword);
            byte[] hashData = sha256Hasher.ComputeHash(passwordData);
            string hashResult = Convert.ToBase64String(hashData);

            return hashResult;
        }

        public string HashData(string Input)
        {
            string saltKey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
            string saltAndInput = string.Concat(Input, saltKey);

            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();

            byte[] inputData = Encoding.Default.GetBytes(saltAndInput);
            byte[] hashData = sha256Hasher.ComputeHash(inputData);
            string hashResult = Convert.ToBase64String(hashData);

            return hashResult;
        }


    }
}