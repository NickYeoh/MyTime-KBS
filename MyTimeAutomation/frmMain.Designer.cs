
namespace MyTimeAutomation
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gbxAutoCloseLastMonthAttendance = new System.Windows.Forms.GroupBox();
            this.lblNextScheduleClosingDate = new System.Windows.Forms.Label();
            this.lblCaption01 = new System.Windows.Forms.Label();
            this.lblMonthEndClosingProcessStatus = new System.Windows.Forms.Label();
            this.optDisable = new System.Windows.Forms.RadioButton();
            this.optEnable = new System.Windows.Forms.RadioButton();
            this.tmrAutoCloseService = new System.Windows.Forms.Timer(this.components);
            this.gbxAutoCloseLastMonthAttendance.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxAutoCloseLastMonthAttendance
            // 
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.lblNextScheduleClosingDate);
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.lblCaption01);
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.lblMonthEndClosingProcessStatus);
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.optDisable);
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.optEnable);
            this.gbxAutoCloseLastMonthAttendance.Location = new System.Drawing.Point(12, 12);
            this.gbxAutoCloseLastMonthAttendance.Name = "gbxAutoCloseLastMonthAttendance";
            this.gbxAutoCloseLastMonthAttendance.Size = new System.Drawing.Size(304, 98);
            this.gbxAutoCloseLastMonthAttendance.TabIndex = 0;
            this.gbxAutoCloseLastMonthAttendance.TabStop = false;
            this.gbxAutoCloseLastMonthAttendance.Text = "Month End Closing Process :";
            // 
            // lblNextScheduleClosingDate
            // 
            this.lblNextScheduleClosingDate.Location = new System.Drawing.Point(12, 74);
            this.lblNextScheduleClosingDate.Name = "lblNextScheduleClosingDate";
            this.lblNextScheduleClosingDate.Size = new System.Drawing.Size(152, 13);
            this.lblNextScheduleClosingDate.TabIndex = 4;
            this.lblNextScheduleClosingDate.Text = "-";
            this.lblNextScheduleClosingDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCaption01
            // 
            this.lblCaption01.AutoSize = true;
            this.lblCaption01.Location = new System.Drawing.Point(12, 54);
            this.lblCaption01.Name = "lblCaption01";
            this.lblCaption01.Size = new System.Drawing.Size(152, 13);
            this.lblCaption01.TabIndex = 3;
            this.lblCaption01.Text = "Next Scheduled Closing Date :";
            // 
            // lblMonthEndClosingProcessStatus
            // 
            this.lblMonthEndClosingProcessStatus.AutoSize = true;
            this.lblMonthEndClosingProcessStatus.Location = new System.Drawing.Point(199, 26);
            this.lblMonthEndClosingProcessStatus.Name = "lblMonthEndClosingProcessStatus";
            this.lblMonthEndClosingProcessStatus.Size = new System.Drawing.Size(10, 13);
            this.lblMonthEndClosingProcessStatus.TabIndex = 2;
            this.lblMonthEndClosingProcessStatus.Text = "-";
            this.lblMonthEndClosingProcessStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // optDisable
            // 
            this.optDisable.AutoSize = true;
            this.optDisable.Location = new System.Drawing.Point(87, 24);
            this.optDisable.Name = "optDisable";
            this.optDisable.Size = new System.Drawing.Size(60, 17);
            this.optDisable.TabIndex = 1;
            this.optDisable.TabStop = true;
            this.optDisable.Text = "Disable";
            this.optDisable.UseVisualStyleBackColor = true;
            this.optDisable.CheckedChanged += new System.EventHandler(this.optDisable_CheckedChanged);
            // 
            // optEnable
            // 
            this.optEnable.AutoSize = true;
            this.optEnable.Location = new System.Drawing.Point(12, 24);
            this.optEnable.Name = "optEnable";
            this.optEnable.Size = new System.Drawing.Size(58, 17);
            this.optEnable.TabIndex = 0;
            this.optEnable.TabStop = true;
            this.optEnable.Text = "Enable";
            this.optEnable.UseVisualStyleBackColor = true;
            this.optEnable.CheckedChanged += new System.EventHandler(this.optEnable_CheckedChanged);
            // 
            // tmrAutoCloseService
            // 
            this.tmrAutoCloseService.Interval = 1000;
            this.tmrAutoCloseService.Tick += new System.EventHandler(this.tmrAutoCloseService_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 180);
            this.Controls.Add(this.gbxAutoCloseLastMonthAttendance);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Automation";
            this.gbxAutoCloseLastMonthAttendance.ResumeLayout(false);
            this.gbxAutoCloseLastMonthAttendance.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxAutoCloseLastMonthAttendance;
        private System.Windows.Forms.RadioButton optDisable;
        private System.Windows.Forms.RadioButton optEnable;
        private System.Windows.Forms.Label lblMonthEndClosingProcessStatus;
        private System.Windows.Forms.Timer tmrAutoCloseService;
        private System.Windows.Forms.Label lblCaption01;
        private System.Windows.Forms.Label lblNextScheduleClosingDate;
    }
}

