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

        private DateTime pdteScheduleClosingDateTime;
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

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);

            lblNextScheduleClosingDate.Text = "-";

            blnIsClosingDone = false;

            getConnSetting();

        }


        private void getConnSetting()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (ConfigurationManager.ConnectionStrings["MyTimeDB"] == null)
            {

                config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("MyTimeDB", String.Format("Persist Security Info=false; Integrated Security=true; Data Source={0};Initial Catalog={1};", "DESKTOP-FPM7EFQ\\SQL2019", "MyTimeDB"), "System.Data.SqlClient"));
                config.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("connectionStrings");

            }

            connStr = ConfigurationManager.ConnectionStrings["MyTimeDB"].ConnectionString;


            if (ConfigurationManager.ConnectionStrings["DeviceDB"] == null)
            {

                config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("DeviceDB", String.Format("Persist Security Info=false; Integrated Security=true; Data Source={0};Initial Catalog={1};", "DESKTOP-FPM7EFQ\\SQL2016", "Pegasys"), "System.Data.SqlClient"));
                config.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("connectionStrings");

            }

            connDeviceStr = ConfigurationManager.ConnectionStrings["DeviceDB"].ConnectionString;

        }


        private string getScheduleSetting(string param)
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
            DateTime dteScheduleClosingDateTime;
            string strScheduledClosingDay;
            string strScheduledClosingTime;
            string strScheduledClosingDateTime;

            lblMonthEndClosingProcessStatus.Text = "Running";
            lblMonthEndClosingProcessStatus.ForeColor = Color.Green;

            //mydate = "2016/31/05 13:33";
            //date = mydate.ToDateTime("yyyy/d/M HH:mm"); // {31.05.2016 13:33:00}

            strScheduledClosingDay = getScheduleSetting("ScheduledClosingDay");
            strScheduledClosingTime = getScheduleSetting("ScheduledClosingTime");

            strScheduledClosingDateTime = string.Format("{0}-{1}-{2} {3}", DateTime.Now.Year.ToString("D4"), DateTime.Now.Month.ToString("D2"), Convert.ToInt32(strScheduledClosingDay).ToString("D2"), strScheduledClosingTime);

            dteScheduleClosingDateTime = DateTime.ParseExact(strScheduledClosingDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            pdteScheduleClosingDateTime = new DateTime();
            pdteScheduleClosingDateTime = dteScheduleClosingDateTime;

            lblNextScheduleClosingDate.Text = Convert.ToString(pdteScheduleClosingDateTime);

            tmrAutoCloseService.Enabled = true;

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
            if (blnIsClosingDone == false)
            {

                if (DateTime.Now.ToString("yyyyMMdd hh:mm") == pdteScheduleClosingDateTime.ToString("yyyyMMdd hh:mm"))
                {

                    blnIsClosingDone = true;

                    if (blnIsClosingDone == true)
                    {

                        MessageBox.Show("OK");
                        Application.DoEvents();

                        blnIsClosingDone = false;
                        Application.DoEvents();
                    }


                }

            }

        }
    }
}
