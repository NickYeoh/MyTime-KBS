using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;
using System.Collections.Specialized;
using System.Globalization;
using System.Data.SqlClient;

namespace MyTimeAutomation
{

    public partial class frmMain : Form
    {

        private DateTime pdteNextClosingDateTime;
        private Boolean blnIsClosingDone;

        private string connStr;
        private string connDeviceStr;

        public frmMain()
        {
            InitializeComponent();


            init();
        }

        private void init()
        {
            setWindowsPosition();

            lblNextScheduleClosingDate.Text = "-";
            blnIsClosingDone = false;

            getConnSetting();

        }

        private void setWindowsPosition()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);

        }


        private void getConnSetting()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (ConfigurationManager.ConnectionStrings["MyTimeDB"] == null)
            {

                config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("MyTimeDB", String.Format("Persist Security Info=false; Integrated Security=true; Data Source={0};Initial Catalog={1};", "localhost\\SQL2019", "MyTimeDB"), "System.Data.SqlClient"));
                config.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("connectionStrings");

            }

            connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;


            if (ConfigurationManager.ConnectionStrings["DeviceDB"] == null)
            {

                config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("DeviceDB", String.Format("Persist Security Info=false; Integrated Security=true; Data Source={0};Initial Catalog={1};", "localhost\\SQL2016", "Pegasys"), "System.Data.SqlClient"));
                config.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("connectionStrings");

            }

            connDeviceStr = ConfigurationManager.ConnectionStrings["DeviceDB"].ConnectionString;

        }

        private string getGeneralSetting(string param)
        {
            string strValue = "";

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (settings[param] == null)
            {

                settings.Remove(param);

                switch (param)
                {
                    case "ScheduledClosingDay":
                        {
                            strValue = "01";

                            settings.Add(param, strValue);
                            break;
                        }
                    case "ScheduledClosingTime":
                        {
                            strValue = "03:00:00";

                            settings.Add(param, strValue);
                            break;
                        }
                }

            }
            else
            {
                strValue = settings[param].Value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

            return strValue;
        }


        private void optEnable_CheckedChanged(object sender, EventArgs e)
        {

            string strScheduledClosingDay;
            string strScheduledClosingTime;

            lblMonthEndClosingProcessStatus.Text = "Running";
            lblMonthEndClosingProcessStatus.ForeColor = Color.Green;

            strScheduledClosingDay = getGeneralSetting("ScheduledClosingDay");
            strScheduledClosingTime = getGeneralSetting("ScheduledClosingTime");

            setNextClosingDateTime(strScheduledClosingDay, strScheduledClosingTime);

            lblNextScheduleClosingDate.Text = Convert.ToString(pdteNextClosingDateTime);

            tmrAutoCloseService.Enabled = true;

        }


        private void setNextClosingDateTime(string strScheduledClosingDay, string strScheduledClosingTime)
        {

            DateTime dteNextClosingDateTime;
            string strNextClosingDateTime;

            try
            {
                DateTime dteCurrentMonth = DateTime.Now;
                DateTime dteLastMonth;
                DateTime dteNextMonth;

                dteLastMonth = dteCurrentMonth.AddMonths(-1);
                dteNextMonth = dteCurrentMonth.AddMonths(1);

                if (createMonthAttendanceTable(dteLastMonth) == true)
                {
                    // New table created
                    // Get and insert trans                                  

                }
                else
                {
                    // Table already existe

                 
                    // Previous data table found in database
                    strNextClosingDateTime = string.Format("{0}-{1}-{2} {3}", dteNextMonth.Year.ToString("D4"), dteNextMonth.Month.ToString("D2"), Convert.ToInt32(strScheduledClosingDay).ToString("D2"), strScheduledClosingTime);

                    //dteNextClosingDateTime = DateTime.ParseExact(strNextClosingDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    //pdteNextClosingDateTime = new DateTime();
                    //pdteNextClosingDateTime = dteNextClosingDateTime;

                }               


            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
              
            }

        }

        private bool createMonthAttendanceTable(DateTime dteMonth)
        {
           
            bool isTableCreated = false;
            string tableName;
            string sql;

            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            tableName = string.Format("AttendanceTrans_{0}", dteMonth.ToString("yyyyMM"));
            sql = "Select count(*) from  information_schema.tables Where table_name='" + tableName + "'";

            SqlCommand cmd = new SqlCommand(sql, conn);
          
            if ((int)cmd.ExecuteScalar() == 0)
            {

                sql = "CREATE TABLE " + tableName;
                sql += " " + "(NRIC NVARCHAR(20) NOT NULL, AttendanceDate DATETIME NOT NULL, AttendanceDay NVARCHAR(10) NOT NULL,";
                sql += " " + "AttendanceStatusID NVARCHAR(3), AttendanceStatus NVARCHAR(25),";
                sql += " " + "FirstIn NVARCHAR(20), Lateness NVARCHAR(20), LastOut NVARCHAR(20),";
                sql += " " + "WorkTime NVARCHAR(20), OvertimeStart NVARCHAR(20), OvertimeEnd NVARCHAR(20), Overtime NVARCHAR(20),";
                sql += " " + "OvertimeExtraStart NVARCHAR(20), OvertimeExtraEnd NVARCHAR(20), OvertimeExtra NVARCHAR(20), TotalOvertime NVARCHAR(20)";
                sql += " " + "PRIMARY KEY (NRIC, AttendanceDate))";

                cmd = new SqlCommand(sql, conn);

                cmd.ExecuteNonQuery();


                isTableCreated = true;
            }
                     
          
            conn.Close();
            conn.Dispose();


            return isTableCreated;
        }



        private void listUser(DateTime teScheduleClosingDateTime)
        {

            //DateTime.TryParse(selectedDate, out startOn);

            //int days = DateTime.DaysInMonth(startOn.Year, startOn.Month);
            //endOn = startOn.AddDays((days - 1));

            //userList = userDBService.ListUser().Where(u => u.DepartmentID == selectedDepartmentID && ((u.IsResigned == true && u.ResignedOn >= startOn.Date) || u.IsResigned == false) && u.IsAttendanceExcluded == false).OrderBy(u => u.UserName).ToList();
            //break;

            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            SqlCommand cmd = new SqlCommand("Select * from tablename", conn);

            conn.Close();
            conn.Dispose();
        }


        private void optDisable_CheckedChanged(object sender, EventArgs e)
        {
            lblMonthEndClosingProcessStatus.Text = "-";
            lblMonthEndClosingProcessStatus.ForeColor = Color.Red;

            tmrAutoCloseService.Enabled = false;
        }

        private void tmrAutoCloseService_Tick(object sender, EventArgs e)
        {

            DateTime dteCurrentDateTime = DateTime.Now;


            //if (DateTime.Now.ToString("yyyyMMdd hh:mm") >= pdteNextClosingDateTime.ToString("yyyyMMdd hh:mm"))
            if (dteCurrentDateTime >= pdteNextClosingDateTime)
            {
                if (blnIsClosingDone == false)
                {

                    // Perform Closing Here



                    // Set the Flag to true
                    blnIsClosingDone = true;
                    Application.DoEvents();


                    MessageBox.Show("OK");

                }

                Application.DoEvents();

            }

            Application.DoEvents();

        }


    }
}
