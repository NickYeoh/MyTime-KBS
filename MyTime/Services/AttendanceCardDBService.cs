using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyTime.Models;
using System.Configuration;
using System.Data;
using System.Web;
using System.Linq;


namespace MyTime.Services
{

    public class AttendanceCardDBService
    {
        private readonly static string connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(connStr);


        public List<AttendanceCardReportModel> GetAttendanceCardByID(string ID)
        {
            List<AttendanceCardReportModel> attendanceCardList = new List<AttendanceCardReportModel>();
            AttendanceCardReportModel attendanceCardModel = new AttendanceCardReportModel();

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
                        attendanceCardModel = new AttendanceCardReportModel();

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

        public string GetMonthlyAttendanceCardStatusByID(string ID, DateTime startOn)
        {
            string attendanceCardStatus = "";

            //string sql = " " + $@"SELECT TOP 1 IIF(AttendanceCardStatus IS NULL, 'YL', AttendanceCardStatus) AS AttendanceCardStatus FROM [AttendanceCard]";
            //sql += " " + $@"WHERE [AttendanceCard].NRIC = {ID}";
            //sql += " " + $@"AND FORMAT(EffectiveOn, 'yyyyMM') <= '{startOn.ToString("yyyyMM")}'";
            //sql += " " + $@"ORDER BY EffectiveOn DESC";

            string sql = $@"SELECT TOP 1 AttendanceCardStatus FROM [AttendanceCard]";
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


        public List<AttendanceCardReportModel> GetMonthlyAttendanceCardByAttendanceCardStatusAndDepartment(DateTime attendanceMonth, string departmentID, string attendanceCardStatus)
        {
            List<AttendanceCardReportModel> attendanceCardList = new List<AttendanceCardReportModel>();
            AttendanceCardReportModel attendanceCardModel = new AttendanceCardReportModel();


            string sql = $@"SELECT AttendanceMonth, DepartmentName, ";
            sql += " " + $@"AC.NRIC, U.UserName,";
            sql += " " + $@"LateInCount, EarlyOutCount,";
            sql += " " + $@"LateInEarlyOutCount, IncompleteCount,";
            sql += " " + $@"AbsentCount, AttendCount,";
            sql += " " + $@"OnLeaveCount, TotalAttendanceIssue,";
            sql += " " + $@"AttendanceCardStatus";
            sql += " " + $@"FROM AttendanceCard AC";
            sql += " " + $@"LEFT JOIN [User] U ON U.NRIC=AC.NRIC";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID=U.DepartmentID";           
            sql += " " + $@"WHERE AttendanceMonth='{attendanceMonth.ToString("yyyyMM")}'";
            sql += " " + $@"AND AttendanceCardStatus='{attendanceCardStatus}'";
            sql += " " + $@"AND U.DepartmentID='{departmentID}'";
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
                        attendanceCardModel = new AttendanceCardReportModel();


                        if (!dr["AttendanceMonth"].Equals(DBNull.Value))
                        {
                            //attendanceCardModel.AttendanceMonth = dr["AttendanceMonth"].ToString();
                            attendanceCardModel.AttendanceMonth = attendanceMonth.ToString("MMM, yyyy");
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

        public List<AttendanceCardSummaryReportModel> GetYearlyAttendanceCardByDepartment(DateTime attendanceYear, string departmentID)
        {
            
            List<AttendanceCardReportModel> tempList = new List<AttendanceCardReportModel>();
            AttendanceCardReportModel attendanceCardModel = new AttendanceCardReportModel();

            List<AttendanceCardSummaryReportModel> attendanceCardList = new List<AttendanceCardSummaryReportModel>();
            AttendanceCardSummaryReportModel attendanceCardSummaryReportModel = new AttendanceCardSummaryReportModel();

            string sql = $@"SELECT AttendanceMonth, DepartmentName, ";
            sql += " " + $@"AC.NRIC, U.UserName,";
            sql += " " + $@"LateInCount, EarlyOutCount,";
            sql += " " + $@"LateInEarlyOutCount, IncompleteCount,";
            sql += " " + $@"AbsentCount, AttendCount,";
            sql += " " + $@"OnLeaveCount, TotalAttendanceIssue,";
            sql += " " + $@"AttendanceCardStatus";
            sql += " " + $@"FROM AttendanceCard AC";
            sql += " " + $@"LEFT JOIN [User] U ON U.NRIC=AC.NRIC";
            sql += " " + $@"LEFT JOIN Department D ON D.DepartmentID=U.DepartmentID";
            sql += " " + $@"WHERE SUBSTRING(AttendanceMonth, 1,4) ='{attendanceYear.ToString("yyyy")}'";        
            sql += " " + $@"AND U.DepartmentID='{departmentID}'";
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
                        attendanceCardModel = new AttendanceCardReportModel();


                        if (!dr["AttendanceMonth"].Equals(DBNull.Value))
                        {
                            //attendanceCardModel.AttendanceMonth = dr["AttendanceMonth"].ToString();
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

                        tempList.Add(attendanceCardModel);

                    }


                    // Get 12 month breakdown
                    var userNameList = tempList.OrderBy(a=>a.UserName).GroupBy(a => a.NRIC).ToList();

                    string NRIC;
                    //int rowCount;
                    //rowCount = userNameList.Count();

                    string monthYear;


                    foreach (var row in userNameList)
                    {  

                        NRIC = row.Key;
                      
                        attendanceCardSummaryReportModel = new AttendanceCardSummaryReportModel();
                        attendanceCardSummaryReportModel.AttendanceYear = attendanceYear.ToString("yyyy");
                        attendanceCardSummaryReportModel.NRIC = NRIC;

                        for (int i=1; i<=12; i++)
                        {
                           
                            monthYear = attendanceYear.ToString("yyyy") + i.ToString("D2");

                            attendanceCardModel = tempList.Select(t => t).Where(t => t.NRIC == NRIC && t.AttendanceMonth == monthYear).FirstOrDefault();

                            if (attendanceCardModel != null)
                            {
                                attendanceCardSummaryReportModel.DepartmentName = attendanceCardModel.DepartmentName;
                                attendanceCardSummaryReportModel.UserName = attendanceCardModel.UserName;
                            }
                            //else
                            //{
                            //    attendanceCardSummaryReportModel.DepartmentName = "";
                            //    attendanceCardSummaryReportModel.UserName = "";
                            //}
                                                     

                            switch(i)
                            {
                                case 1:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus01 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue01 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus01 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue01 = 0;
                                    }
                                    break;
                                case 2:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus02 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue02 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus02 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue02 = 0;
                                    }
                                    break;
                                case 3:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus03 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue03 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus03 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue03 = 0;
                                    }
                                    break;
                                case 4:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus04 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue04 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus04 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue04 = 0;
                                    }
                                    break;
                                case 5:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus05 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue05 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus06 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue06 = 0;
                                    }
                                    break;
                                case 6:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus06 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue06 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus06 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue06 = 0;
                                    }
                                    break;
                                case 7:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus07 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue07 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus07 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue07 = 0;
                                    }
                                    break;
                                case 8:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus08 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue08 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus08 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue08 = 0;
                                    }
                                    break;
                                case 9:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus09 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue09 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus09 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue09 = 0;
                                    }
                                    break;
                                case 10:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus10 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue10 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus10 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue10 = 0;
                                    }
                                    break;
                                case 11:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus11 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue11 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus11 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue11 = 0;
                                    }
                                    break;
                                case 12:
                                    if (attendanceCardModel != null)
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus12 = attendanceCardModel.AttendanceCardStatus;
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue12 = attendanceCardModel.TotalAttendanceIssue;
                                    }
                                    else
                                    {
                                        attendanceCardSummaryReportModel.AttendanceCardStatus12 = "";
                                        attendanceCardSummaryReportModel.TotalAttendanceIssue12 = 0;
                                    }
                                    break;
                            }
                              

                        }

                        attendanceCardList.Add(attendanceCardSummaryReportModel);







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


    }
}