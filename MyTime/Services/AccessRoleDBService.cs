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
    public class AccessRoleDBService
    {

        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);

        private readonly static string connStrJC = ConfigurationManager.ConnectionStrings["DeviceDB"].ConnectionString;
        private readonly SqlConnection connJC = new SqlConnection(connStrJC);

        LogActivityDBService logActivityDBService = new LogActivityDBService();
        UserDBService userDBService = new UserDBService();
      
        public List<AccessRoleModel> ListAccessRole()
        {
            AccessRoleModel accessRoleModel;
            List<AccessRoleModel> dataList = new List<AccessRoleModel>();

            try
            {

                string sql = $@"SELECT * FROM AccessRole ORDER BY AccessRoleID ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        accessRoleModel = new AccessRoleModel();

                        accessRoleModel.AccessRoleID = Convert.ToInt32(dr["AccessRoleID"]);
                        accessRoleModel.AccessRoleName = dr["AccessRoleName"].ToString();
                        accessRoleModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

                        dataList.Add(accessRoleModel);

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

                string sql = $@"SELECT * FROM AccessRole WHERE AccessRoleName='{Name}'";

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

        public AccessRoleModel GetDataByID(int ID)
        {
            AccessRoleModel accessRoleModel;

            try
            {
                accessRoleModel = new AccessRoleModel();

                string sql = $@"SELECT * FROM AccessRole WHERE AccessRoleID={ID}";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    accessRoleModel.AccessRoleID = Convert.ToInt32(dr["AccessRoleID"]);
                    accessRoleModel.AccessRoleName = dr["AccessRoleName"].ToString();
                    accessRoleModel.IsActivated = Convert.ToBoolean(dr["IsActivated"]);

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

            return accessRoleModel;

        }


        public List<AccessRoleDeviceModel> ListJohnsonControlAccessRoleDevice(int accessRoleID, bool isIncludedAll, bool IsOvertimeExtraDevice)
        {          
            AccessRoleDeviceModel accessRoleDeviceModel;
            List<AccessRoleDeviceModel> accessRoleDeviceList = new List<AccessRoleDeviceModel>();
            string sql="";

            SqlCommand cmd;
            SqlDataReader dr;

            SqlCommand cmdJC;
            SqlDataReader drJC;

            try
            {
                //SELECT A.AccessRoleID, A.DeviceID, T.tp_term_name AS DeviceName
                //FROM AccessRoleDevice A INNER JOIN
                //Pegasys.dbo.terminal T ON T.tp_term_id = A.DeviceID




                //sql = $@"SELECT A.AccessRoleID, A.DeviceID, T.tp_term_name AS DeviceName";
                //sql += " " + $@"FROM AccessRoleDevice A INNER JOIN";
                //sql += " " + $@"Pegasys.dbo.terminal T ON T.tp_term_id = A.DeviceID";
                //sql += " " + $@"WHERE A.AccessRoleID='{accessRoleID}'";
                //sql += " " + $@"ORDER BY A.AccessRoleID ASC";

                sql = $@"SELECT AccessRoleID, DeviceID, IsOvertimeExtraDevice";
                sql += " " + $@"FROM AccessRoleDevice";            
                sql += " " + $@"WHERE AccessRoleID='{accessRoleID}'";

                if (isIncludedAll == false)
                {
                    sql += " " + $@"AND IsOvertimeExtraDevice='{IsOvertimeExtraDevice}'";
                }
              
                sql += " " + $@"ORDER BY AccessRoleID ASC";

                conn.Open();

               cmd = new SqlCommand(sql, conn);

               dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        accessRoleDeviceModel = new AccessRoleDeviceModel();

                        accessRoleDeviceModel.AccessRoleID = Convert.ToInt32(dr["AccessRoleID"]);
                        accessRoleDeviceModel.DeviceID = Convert.ToInt32(dr["DeviceID"]);
                        accessRoleDeviceModel.IsOvertimeExtraDevice = Convert.ToBoolean(dr["IsOvertimeExtraDevice"]);

                        sql = $@"SELECT tp_term_name AS DeviceName FROM Pegasys.dbo.terminal WHERE tp_term_id='{accessRoleDeviceModel.DeviceID}'";

                        connJC.Open();

                        cmdJC= new SqlCommand(sql, connJC);
                        drJC = cmdJC.ExecuteReader();

                        if (drJC.HasRows)
                        {

                            while (drJC.Read())
                            {
                                accessRoleDeviceModel.DeviceName = drJC["DeviceName"].ToString();
                            }
                         
                        }

                        connJC.Close();

                        accessRoleDeviceList.Add(accessRoleDeviceModel);

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

            return accessRoleDeviceList;
        }


        public List<AccessRoleDeviceModel> ListSupremaAccessRoleDevice(int accessRoleID, bool isIncludedAll, bool IsOvertimeExtraDevice)
        {
            AccessRoleDeviceModel accessRoleDeviceModel;
            List<AccessRoleDeviceModel> accessRoleDeviceList = new List<AccessRoleDeviceModel>();

            try
            {
                //SELECT A.AccessRoleID, A.DeviceID, B.NM AS DeviceName
                //FROM AccessRoleDevice A INNER JOIN
                //BioStarAC.dbo.T_DEV B ON B.DEVID = A.DeviceID

                string sql = $@"SELECT A.AccessRoleID, A.DeviceID, A.IsOvertimeExtraDevice, B.NM AS DeviceName";
                sql += " " + $@"FROM AccessRoleDevice A INNER JOIN";
                sql += " " + $@"BioStarAC.dbo.T_DEV B ON B.DEVID = A.DeviceID";
                sql += " " + $@"WHERE A.AccessRoleID='{accessRoleID}'";
                sql += " " + $@"AND A.IsOvertimeExtraDevice='{IsOvertimeExtraDevice}'";
                sql += " " + $@"ORDER BY A.AccessRoleID ASC";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        accessRoleDeviceModel = new AccessRoleDeviceModel();

                        accessRoleDeviceModel.AccessRoleID = Convert.ToInt32(dr["AccessRoleID"]);
                        accessRoleDeviceModel.DeviceID = Convert.ToInt32(dr["DeviceID"]);
                        accessRoleDeviceModel.DeviceName = dr["DeviceName"].ToString();

                        if (isIncludedAll == false)
                        {
                            accessRoleDeviceModel.IsOvertimeExtraDevice = Convert.ToBoolean(dr["IsOvertimeExtraDevice"]);
                        }

                        accessRoleDeviceList.Add(accessRoleDeviceModel);

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

            return accessRoleDeviceList;
        }


        public bool AddAccessRoleDevice(int accessRoleID, String selectedDeviceID, bool IsOvertimeExtraDevice)
        {
            bool status = false;

            string[] deviceID = selectedDeviceID.Split('#');
            string sql;
            SqlCommand cmd;
                       
            try
            {

                if (!deviceID.Length.Equals(0))
                {
                    conn.Open();

                    //sql = $@"DELETE AccessRoleDevice";
                    //sql += " " + $@"WHERE AccessRoleID='{accessRoleID}'";
                    //cmd = new SqlCommand(sql, conn);
                    //cmd.ExecuteNonQuery();

                    for (int i = 0; i < deviceID.Length; i++)
                    {

                        sql = $@"INSERT INTO AccessRoleDevice";
                        sql += " " + $@"(AccessRoleID, DeviceID, IsOvertimeExtraDevice)";
                        sql += " " + $@"VALUES";
                        sql += " " + $@"('{accessRoleID}', '{deviceID[i]}','{IsOvertimeExtraDevice}')";

                        cmd = new SqlCommand(sql, conn);

                        cmd.ExecuteNonQuery();

                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Access Role Device", $@"Create; {accessRoleID}, {deviceID[i]}, {IsOvertimeExtraDevice}", DateTime.Now);

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

        //public List<DeviceModel> AssignAccessRoleDevice(List<DeviceModel> deviceList,  string selectedDeviceID, string selectedDeviceName)
        //{
        //    string[] deviceID = selectedDeviceID.Split('#');
        //    string[] deviceName = selectedDeviceName.Split('#');

        //    DeviceModel deviceModel;

        //    if (! deviceID.Length.Equals(0))
        //    {
        //        for (int i=0; i < deviceID.Length; i++)
        //        {
        //            deviceModel = new DeviceModel();

        //            deviceModel.DeviceID = Convert.ToInt32( deviceID[i]);
        //            deviceModel.DeviceName = deviceName[i];

        //            deviceList.Add(deviceModel);
        //        }
        //    }

            
        //    return deviceList;
        //}

        public bool Create(AccessRoleModel accessRoleModel)
        {
            bool status = false;
            
            try
            {

                string sql = $@"INSERT INTO AccessRole";
                sql += " " + $@"(AccessRoleName, IsActivated)";
                sql += " " + $@"VALUES";
                sql += " " + $@"('{accessRoleModel.AccessRoleName}', '{accessRoleModel.IsActivated}')";
                sql += " " + $@"SELECT SCOPE_IDENTITY()";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (! cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;
                }


                logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Access Role", $@"Create; {accessRoleModel.AccessRoleName}", DateTime.Now);

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

        public bool UpdateAccessRoleDevice(int accessRoleID, List<DeviceModel> deviceList)
        {
            bool status = false;
            string sql = "";

            bool IsOvertimeExtraDevice = false;

            try
            {
                conn.Open();

                sql = $@"DELETE AccessRoleDevice";
                sql += " " + $@"WHERE AccessRoleID='{accessRoleID}'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                foreach (var device in deviceList)
                {
                    sql = $@"INSERT INTO AccessRoleDevice";
                    sql += " " + $@"(AccessRoleID, DeviceID, IsOvertimeExtraDevice)";
                    sql += " " + $@"VALUES";
                    sql += " " + $@"('{accessRoleID}', '{device.DeviceID}', '{IsOvertimeExtraDevice}')";

                    cmd = new SqlCommand(sql, conn);

                    cmd.ExecuteNonQuery();

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Access Role Device", $@"Create; {accessRoleID}, {device.DeviceID}, {IsOvertimeExtraDevice}", DateTime.Now);

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

        public bool DeleteAccessRoleDevice(int accessRoleID, String deviceID)
        {

            bool status = false;

            try
            {
                string sql = $@"DELETE AccessRoleDevice WHERE AccessRoleID='{accessRoleID}' AND";
                sql += " " + $@"DeviceID='{deviceID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Access Role Device", $@"Delete; {accessRoleID}, {deviceID}", DateTime.Now);

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


        public bool Update(AccessRoleModel accessRoleModel)
        {

            bool status = false;

            try
            {
                string sql = $@"UPDATE AccessRole SET AccessRoleName='{accessRoleModel.AccessRoleName}', IsActivated='{accessRoleModel.IsActivated}' WHERE AccessRoleID='{accessRoleModel.AccessRoleID}'";

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (!cmd.ExecuteNonQuery().Equals(0))
                {
                    status = true;

                    logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Access Role", $@"Update; {accessRoleModel.AccessRoleName}, {accessRoleModel.IsActivated}, '{accessRoleModel.AccessRoleID}'", DateTime.Now);

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

        public bool Delete(AccessRoleModel accessRoleModel)
        {

            bool status = false;
            string sql;
            SqlCommand cmd;

            int accessRoleID = accessRoleModel.AccessRoleID;
            List<UserModel> userList = new List<UserModel>();


            try
            {
                userList = userDBService.ListUser();

                if (userList.Where(u => u.AccessRoleID == accessRoleID).ToList().Count.Equals(0))
                {


                    conn.Open();

                    // Delete AccessRoleDevice
                    sql = $@"DELETE AccessRoleDevice WHERE AccessRoleID='{accessRoleModel.AccessRoleID}'";
                    cmd = new SqlCommand(sql, conn);

                    cmd.ExecuteNonQuery();

                    // Delete AccessRole
                    sql = $@"DELETE AccessRole WHERE AccessRoleID='{accessRoleModel.AccessRoleID}'";

                    cmd = new SqlCommand(sql, conn);

                    if (!cmd.ExecuteNonQuery().Equals(0))
                    {
                        status = true;

                        logActivityDBService.LogActivity(HttpContext.Current.User.Identity.Name, "Access Role", $@"Delete; {accessRoleModel.AccessRoleID}, {accessRoleModel.AccessRoleName}", DateTime.Now);

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