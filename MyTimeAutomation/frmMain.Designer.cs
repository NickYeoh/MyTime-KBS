
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
            this.lblLastActivity = new System.Windows.Forms.Label();
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
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.lblLastActivity);
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.lblCaption01);
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.lblMonthEndClosingProcessStatus);
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.optDisable);
            this.gbxAutoCloseLastMonthAttendance.Controls.Add(this.optEnable);
            this.gbxAutoCloseLastMonthAttendance.ForeColor = System.Drawing.SystemColors.Window;
            this.gbxAutoCloseLastMonthAttendance.Location = new System.Drawing.Point(23, 17);
            this.gbxAutoCloseLastMonthAttendance.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.gbxAutoCloseLastMonthAttendance.Name = "gbxAutoCloseLastMonthAttendance";
            this.gbxAutoCloseLastMonthAttendance.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.gbxAutoCloseLastMonthAttendance.Size = new System.Drawing.Size(393, 123);
            this.gbxAutoCloseLastMonthAttendance.TabIndex = 0;
            this.gbxAutoCloseLastMonthAttendance.TabStop = false;
            this.gbxAutoCloseLastMonthAttendance.Text = "Compile Last Month Attendance Data :";
            // 
            // lblLastActivity
            // 
            this.lblLastActivity.Location = new System.Drawing.Point(21, 80);
            this.lblLastActivity.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblLastActivity.Name = "lblLastActivity";
            this.lblLastActivity.Size = new System.Drawing.Size(332, 23);
            this.lblLastActivity.TabIndex = 4;
            this.lblLastActivity.Text = "-";
            this.lblLastActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCaption01
            // 
            this.lblCaption01.AutoSize = true;
            this.lblCaption01.Location = new System.Drawing.Point(21, 60);
            this.lblCaption01.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblCaption01.Name = "lblCaption01";
            this.lblCaption01.Size = new System.Drawing.Size(74, 15);
            this.lblCaption01.TabIndex = 3;
            this.lblCaption01.Text = "Last Activity :";
            // 
            // lblMonthEndClosingProcessStatus
            // 
            this.lblMonthEndClosingProcessStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonthEndClosingProcessStatus.Location = new System.Drawing.Point(234, 38);
            this.lblMonthEndClosingProcessStatus.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblMonthEndClosingProcessStatus.Name = "lblMonthEndClosingProcessStatus";
            this.lblMonthEndClosingProcessStatus.Size = new System.Drawing.Size(142, 58);
            this.lblMonthEndClosingProcessStatus.TabIndex = 2;
            this.lblMonthEndClosingProcessStatus.Text = "-";
            this.lblMonthEndClosingProcessStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // optDisable
            // 
            this.optDisable.AutoSize = true;
            this.optDisable.Location = new System.Drawing.Point(117, 30);
            this.optDisable.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.optDisable.Name = "optDisable";
            this.optDisable.Size = new System.Drawing.Size(67, 19);
            this.optDisable.TabIndex = 1;
            this.optDisable.TabStop = true;
            this.optDisable.Text = "Disable";
            this.optDisable.UseVisualStyleBackColor = true;
            this.optDisable.CheckedChanged += new System.EventHandler(this.optDisable_CheckedChanged);
            // 
            // optEnable
            // 
            this.optEnable.AutoSize = true;
            this.optEnable.Location = new System.Drawing.Point(21, 30);
            this.optEnable.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.optEnable.Name = "optEnable";
            this.optEnable.Size = new System.Drawing.Size(64, 19);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(439, 162);
            this.Controls.Add(this.gbxAutoCloseLastMonthAttendance);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MyTime Backend Service";
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
        private System.Windows.Forms.Label lblLastActivity;
    }
}

