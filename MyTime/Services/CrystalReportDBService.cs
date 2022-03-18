using System.Collections.Generic;
using MyTime.Models;
using System.Linq;

namespace MyTime.Services
{
    public class CrystalReportDBService
    {

        public List<CRAttendanceMonthlyModel> PrepareAttendanceMonthlyReport(List<AttendanceModel> attendanceList, AttendanceSummaryModel attendanceSummaryModel)
        {
            List<CRAttendanceMonthlyModel> crAttendanceMonthlyList = new List<CRAttendanceMonthlyModel>();
            CRAttendanceMonthlyModel crAttendanceMonthlyModel;

            int rowCount = attendanceList.Count;
            int rowNo = 0;

            foreach (var row in attendanceList)
            {
                rowNo += 1;

                crAttendanceMonthlyModel = new CRAttendanceMonthlyModel();

                crAttendanceMonthlyModel.NRIC = row.NRIC;
                crAttendanceMonthlyModel.UserName = row.UserName;
                crAttendanceMonthlyModel.DepartmentName = row.DepartmentName;
                crAttendanceMonthlyModel.CardColorCode = "";
                crAttendanceMonthlyModel.AttendanceDate = row.AttendanceDate;
                crAttendanceMonthlyModel.AttendanceDay = row.AttendanceDay;
                crAttendanceMonthlyModel.AttendanceStatusID = row.AttendanceStatusID;
                crAttendanceMonthlyModel.AttendanceStatus = row.AttendanceStatus;
                crAttendanceMonthlyModel.AttendanceCardStatus = row.AttendanceCardStatus;

                crAttendanceMonthlyModel.IsEarlyIn = row.IsEarlyIn;
                crAttendanceMonthlyModel.FirstIn = row.FirstIn;
                crAttendanceMonthlyModel.Lateness = row.Lateness;
                crAttendanceMonthlyModel.LastOut = row.LastOut;
                crAttendanceMonthlyModel.WorkTime = row.WorkTime;

                // Overtime
                crAttendanceMonthlyModel.Overtime = row.Overtime;
                crAttendanceMonthlyModel.OvertimeStart = row.OvertimeStart;
                crAttendanceMonthlyModel.OvertimeEnd = row.OvertimeEnd;

                // Overtime Extra
                crAttendanceMonthlyModel.OvertimeExtra = row.OvertimeExtra;
                crAttendanceMonthlyModel.OvertimeExtraStart = row.OvertimeExtraStart;
                crAttendanceMonthlyModel.OvertimeExtraEnd = row.OvertimeExtraEnd;
                
                crAttendanceMonthlyModel.ReasonID = row.ReasonID;
                crAttendanceMonthlyModel.ReasonName = row.ReasonName;
                crAttendanceMonthlyModel.Remark = row.Remark;
             
                crAttendanceMonthlyModel.SubmissionDueDate = row.SubmissionDueDate;
                crAttendanceMonthlyModel.IsSubmissionDue = row.IsSubmissionDue;
                crAttendanceMonthlyModel.IsSubmitted = row.IsSubmitted;
                crAttendanceMonthlyModel.IsApproved = row.IsApproved;
                crAttendanceMonthlyModel.IsRejected = row.IsRejected;
                crAttendanceMonthlyModel.IsRequestedToAmend = row.IsRequestedToAmend;
                crAttendanceMonthlyModel.ApproverName = row.ApproverName;

                crAttendanceMonthlyModel.TotalLateIn = attendanceSummaryModel.TotalLateIn;
                crAttendanceMonthlyModel.TotalEarlyOut = attendanceSummaryModel.TotalEarlyOut;
                crAttendanceMonthlyModel.TotalLateInEarlyOut = attendanceSummaryModel.TotalLateInEarlyOut;
                crAttendanceMonthlyModel.TotalIncomplete = attendanceSummaryModel.TotalIncomplete;
                crAttendanceMonthlyModel.TotalAbsent = attendanceSummaryModel.TotalAbsent;
                crAttendanceMonthlyModel.TotalAttend = attendanceSummaryModel.TotalAttend;
                crAttendanceMonthlyModel.TotalOnLeave = attendanceSummaryModel.TotalOnLeave;

                // Total Overtime
                crAttendanceMonthlyModel.TotalOvertime = attendanceSummaryModel.TotalOvertime;

                crAttendanceMonthlyModel.ReportType = "Monthly";
                crAttendanceMonthlyModel.ReportDate = row.AttendanceDate.ToString("MMM, yyyy");

                if (rowNo < rowCount)
                {
                    crAttendanceMonthlyModel.SetPageBreak = false;
                }
                else
                {
                    crAttendanceMonthlyModel.SetPageBreak = true;
                }

                crAttendanceMonthlyList.Add(crAttendanceMonthlyModel);



            }

            return crAttendanceMonthlyList;

        }

        // For Daily, Monthly and Yearly
        public List<CRAttendanceDailyModel> PrepareAttendanceReport(string reportType, List<AttendanceModel> attendanceList, AttendanceSummaryModel attendanceSummaryModel)
        {
            List<CRAttendanceDailyModel> crAttendanceDailyList = new List<CRAttendanceDailyModel>();
            CRAttendanceDailyModel crAttendanceDailyModel;
                    
            int rowCount = attendanceList.Count;
            int rowNo = 0;

            foreach (var row in attendanceList)
            {
                rowNo += 1;

                crAttendanceDailyModel = new CRAttendanceDailyModel();

                crAttendanceDailyModel.NRIC = row.NRIC;
                crAttendanceDailyModel.UserName = row.UserName;
                crAttendanceDailyModel.DepartmentName = row.DepartmentName;
                crAttendanceDailyModel.CardColorCode = "";
                crAttendanceDailyModel.AttendanceDate = row.AttendanceDate;
                crAttendanceDailyModel.AttendanceDay = row.AttendanceDay;
                crAttendanceDailyModel.AttendanceStatusID = row.AttendanceStatusID;
                crAttendanceDailyModel.AttendanceStatus = row.AttendanceStatus;

                crAttendanceDailyModel.IsEarlyIn = row.IsEarlyIn;
                crAttendanceDailyModel.FirstIn = row.FirstIn;
                crAttendanceDailyModel.Lateness = row.Lateness;
                crAttendanceDailyModel.LastOut = row.LastOut;
                crAttendanceDailyModel.WorkTime = row.WorkTime;

                //crAttendanceDailyModel.Overtime = row.Overtime;


                // Overtime
                crAttendanceDailyModel.Overtime = row.Overtime;
                crAttendanceDailyModel.OvertimeStart = row.OvertimeStart;
                crAttendanceDailyModel.OvertimeEnd = row.OvertimeEnd;

                // Overtime Extra
                crAttendanceDailyModel.OvertimeExtra = row.OvertimeExtra;
                crAttendanceDailyModel.OvertimeExtraStart = row.OvertimeExtraStart;
                crAttendanceDailyModel.OvertimeExtraEnd = row.OvertimeExtraEnd;


                crAttendanceDailyModel.ReasonID = row.ReasonID;
                crAttendanceDailyModel.ReasonName = row.ReasonName;
                crAttendanceDailyModel.Remark = row.Remark;

                crAttendanceDailyModel.SubmissionDueDate = row.SubmissionDueDate;
                crAttendanceDailyModel.IsSubmissionDue = row.IsSubmissionDue;
                crAttendanceDailyModel.IsSubmitted = row.IsSubmitted;
                crAttendanceDailyModel.IsApproved = row.IsApproved;
                crAttendanceDailyModel.IsRejected = row.IsRejected;
                crAttendanceDailyModel.IsRequestedToAmend = row.IsRequestedToAmend;

                crAttendanceDailyModel.TotalLateIn = attendanceSummaryModel.TotalLateIn;
                crAttendanceDailyModel.TotalEarlyOut = attendanceSummaryModel.TotalEarlyOut;
                crAttendanceDailyModel.TotalLateInEarlyOut = attendanceSummaryModel.TotalLateInEarlyOut;
                crAttendanceDailyModel.TotalIncomplete = attendanceSummaryModel.TotalIncomplete;
                crAttendanceDailyModel.TotalAbsent = attendanceSummaryModel.TotalAbsent;
                crAttendanceDailyModel.TotalAttend = attendanceSummaryModel.TotalAttend;
                crAttendanceDailyModel.TotalOnLeave = attendanceSummaryModel.TotalOnLeave;
                crAttendanceDailyModel.TotalOvertime = attendanceSummaryModel.TotalOvertime;

                crAttendanceDailyModel.ReportType = reportType ;

                switch(reportType)
                {
                    case "Daily":
                        crAttendanceDailyModel.ReportDate = row.AttendanceDate.ToString("dd MMM, yyyy");
                        break;
                    case "Monthly":
                        crAttendanceDailyModel.ReportDate = row.AttendanceDate.ToString("MMMM, yyyy");
                        break;
                    case "Yearly":
                        crAttendanceDailyModel.ReportDate = row.AttendanceDate.ToString("yyyy");
                        break;
                }

                if (rowNo < rowCount)
                {
                    crAttendanceDailyModel.SetPageBreak = false;
                }
                else
                {
                    crAttendanceDailyModel.SetPageBreak = true;
                }

                crAttendanceDailyList.Add(crAttendanceDailyModel);

            }

            return crAttendanceDailyList;


        }

        // For Monthly and Yearly
        public List<CRAttendanceAnalysisModel> PrepareAttendanceAnalysis(string reportType, List<AttendanceAnalysisModel> attendanceAnalysisList, AttendanceSummaryModel attendanceSummaryModel)
        {
            List<CRAttendanceAnalysisModel> crAttendanceAnalysisList = new List<CRAttendanceAnalysisModel>();
            CRAttendanceAnalysisModel crAttendanceAnalysisModel;

            int rowCount = attendanceAnalysisList.Count;
            int rowNo = 0;

            foreach (var row in attendanceAnalysisList)
            {
                rowNo += 1;

                crAttendanceAnalysisModel = new CRAttendanceAnalysisModel();

                crAttendanceAnalysisModel.DepartmentName = row.DepartmentName;
                crAttendanceAnalysisModel.UserName = row.UserName;
                crAttendanceAnalysisModel.NRIC = row.NRIC;

                crAttendanceAnalysisModel.TotalLateIn = row.TotalLateIn;
                crAttendanceAnalysisModel.TotalEarlyOut = row.TotalEarlyOut;
                crAttendanceAnalysisModel.TotalLateInEarlyOut = row.TotalLateInEarlyOut;
                crAttendanceAnalysisModel.TotalIncomplete = row.TotalIncomplete;
                crAttendanceAnalysisModel.TotalAbsent = row.TotalAbsent;
                crAttendanceAnalysisModel.TotalAttend = row.TotalAttend;
                crAttendanceAnalysisModel.TotalOnLeave = row.TotalOnLeave;

                crAttendanceAnalysisModel.ReportType = reportType;

                switch (reportType)
                {
                    case "Monthly":
                        crAttendanceAnalysisModel.ReportDate = row.ReportDate.ToString("MMMM, yyyy");
                        break;
                    case "Yearly":
                        crAttendanceAnalysisModel.ReportDate = row.ReportDate.ToString("yyyy");
                        break;
                }

                if (rowNo < rowCount)
                {
                    crAttendanceAnalysisModel.SetPageBreak = false;
                }
                else
                {
                    crAttendanceAnalysisModel.SetPageBreak = true;
                }

                crAttendanceAnalysisList.Add(crAttendanceAnalysisModel);

            }

            return crAttendanceAnalysisList;


        }

        // For Monthly and Yearly
        public List<CRAttendanceStatisticsModel> PrepareAttendanceStatistics(string reportType, List<AttendanceStatisticsModel> attendanceStatisticsList)
        {
            List<CRAttendanceStatisticsModel> crAttendanceStatisticsList = new List<CRAttendanceStatisticsModel>();
            CRAttendanceStatisticsModel crAttendanceStatisticsModel;

            int rowCount = attendanceStatisticsList.Count;
            int rowNo = 0;

            foreach (var row in attendanceStatisticsList)
            {
                rowNo += 1;

                crAttendanceStatisticsModel = new CRAttendanceStatisticsModel();

                crAttendanceStatisticsModel.DepartmentName = row.DepartmentName;
                crAttendanceStatisticsModel.UserName = row.UserName;
                crAttendanceStatisticsModel.NRIC = row.NRIC;

                crAttendanceStatisticsModel.AttendanceStatus = row.AttendanceStatus;
                crAttendanceStatisticsModel.TotalCount = row.TotalCount;

                crAttendanceStatisticsModel.ReportType = reportType;

                switch (reportType)
                {
                    case "Monthly":
                        crAttendanceStatisticsModel.ReportDate = row.ReportDate.ToString("MMMM, yyyy");
                        break;
                    case "Yearly":
                        crAttendanceStatisticsModel.ReportDate = row.ReportDate.ToString("yyyy");
                        break;
                }

                if (rowNo < rowCount)
                {
                    crAttendanceStatisticsModel.SetPageBreak = false;
                }
                else
                {
                    crAttendanceStatisticsModel.SetPageBreak = true;
                }

                crAttendanceStatisticsList.Add(crAttendanceStatisticsModel);

            }

            return crAttendanceStatisticsList;
        }

        // For Daily, Monthly and Yearly
        public List<CRAttendanceSummaryReportModel> PrepareAttendanceSummaryReport(string selectedDepartmentID, string reportType, List<AttendanceSummaryReportModel> attendanceSummaryReportList, AttendanceSummaryModel attendanceSummaryModel)
        {
            List<CRAttendanceSummaryReportModel> crAttendanceSummaryReportList = new List<CRAttendanceSummaryReportModel>();
            CRAttendanceSummaryReportModel crAttendanceSummaryReportModel;

            int departmentCount;
            bool isForAllDepartment;

            if ( !selectedDepartmentID.Equals(""))
            {
                isForAllDepartment = false;
            }
            else
            {
                isForAllDepartment = true;
            }

            departmentCount = attendanceSummaryReportList.Select(a => a.DepartmentName).Distinct().Count();

            if (departmentCount > 1)
            {
                isForAllDepartment = false;
            }
            else
            {
                isForAllDepartment = true;
            }

            int rowCount = attendanceSummaryReportList.Count;
            int rowNo = 0;

            foreach (var row in attendanceSummaryReportList)
            {
                rowNo += 1;

                crAttendanceSummaryReportModel = new CRAttendanceSummaryReportModel();

                crAttendanceSummaryReportModel.DepartmentName = row.DepartmentName;
                crAttendanceSummaryReportModel.UserCount = row.UserCount;
                crAttendanceSummaryReportModel.TotalLateIn = row.TotalLateIn;
                crAttendanceSummaryReportModel.TotalEarlyOut = row.TotalEarlyOut;
                crAttendanceSummaryReportModel.TotalLateInEarlyOut = row.TotalLateInEarlyOut;
                crAttendanceSummaryReportModel.TotalIncomplete = row.TotalIncomplete;
                crAttendanceSummaryReportModel.TotalAbsent = row.TotalAbsent;
                crAttendanceSummaryReportModel.TotalAttend = row.TotalAttend;
                crAttendanceSummaryReportModel.TotalOnLeave = row.TotalOnLeave;
                crAttendanceSummaryReportModel.IsForAllDepartment = isForAllDepartment;
                crAttendanceSummaryReportModel.ReportType = reportType;

                switch (reportType)
                {  
                    case "Monthly":
                        crAttendanceSummaryReportModel.ReportDate = row.ReportDate.ToString("MMMM, yyyy");
                        break;
                    case "Yearly":
                        crAttendanceSummaryReportModel.ReportDate = row.ReportDate.ToString("yyyy");
                        break;
                }

                if (rowNo < rowCount)
                {
                    crAttendanceSummaryReportModel.SetPageBreak = false;
                }
                else
                {
                    crAttendanceSummaryReportModel.SetPageBreak = true;
                }

                crAttendanceSummaryReportList.Add(crAttendanceSummaryReportModel);

            }

            return crAttendanceSummaryReportList;


        }


        public List<CRDepartmentAttendanceDailyModel> PrepareDepartmentAttendanceDailySummary(List<DepartmentAttendanceDailyModel> departmentAttendanceDailySummaryList)
        {
            List<CRDepartmentAttendanceDailyModel> crDepartmentAttendanceDailyList = new List<CRDepartmentAttendanceDailyModel>();
            CRDepartmentAttendanceDailyModel crDepartmentAttendanceDailyModel;

            //int totalUserCount;
            //int totalInCount;
            //int totalOutCount;
            //int totalAttendCount;
            //string totalAttendPercentage;

            int rowCount = departmentAttendanceDailySummaryList.Count;
            int rowNo = 0;

            //totalUserCount = departmentAttendanceDailySummaryList.Select(s => s.UserCount).Sum();
            //totalInCount = departmentAttendanceDailySummaryList.Select(s => s.InCount).Sum();
            //totalOutCount = departmentAttendanceDailySummaryList.Select(s => s.OutCount).Sum();
            //totalAttendCount = departmentAttendanceDailySummaryList.Select(s => s.AttendCount).Sum();

            foreach (var row in departmentAttendanceDailySummaryList)
            {
                rowNo += 1;

                crDepartmentAttendanceDailyModel = new CRDepartmentAttendanceDailyModel();

                crDepartmentAttendanceDailyModel.DepartmentID = row.DepartmentID;
                crDepartmentAttendanceDailyModel.DepartmentName = row.DepartmentName;
                crDepartmentAttendanceDailyModel.UserCount = row.UserCount;
                crDepartmentAttendanceDailyModel.InCount = row.InCount;
                crDepartmentAttendanceDailyModel.OutCount = row.OutCount;
                crDepartmentAttendanceDailyModel.AttendCount = row.AttendCount;

                crDepartmentAttendanceDailyModel.ReportType = "Daily";
                crDepartmentAttendanceDailyModel.ReportDate = row.AttendanceDate.ToString("dd MMM, yyyy");

                if (rowNo < rowCount)
                {
                    crDepartmentAttendanceDailyModel.SetPageBreak = false;
                }
                else
                {
                   crDepartmentAttendanceDailyModel.SetPageBreak = true;
                }


                crDepartmentAttendanceDailyList.Add(crDepartmentAttendanceDailyModel);

            }

            return crDepartmentAttendanceDailyList;
        }


        public List<CRDepartmentUserCountModel> PrepareDepartmentUserCountReport(List<DepartmentUserCountModel> departmentUserCountList)
        {

            List<CRDepartmentUserCountModel> cRDepartmentUserCountList = new List<CRDepartmentUserCountModel>();
            CRDepartmentUserCountModel cRDepartmentUserCountModel;

            int rowCount = departmentUserCountList.Count;
            int rowNo = 0;

            foreach (var row in departmentUserCountList)
            {
                rowNo += 1;

                cRDepartmentUserCountModel = new CRDepartmentUserCountModel();

                cRDepartmentUserCountModel.DepartmentName = row.DepartmentName;
                cRDepartmentUserCountModel.UserCount = row.UserCount;

                if (rowNo < rowCount)
                {
                    cRDepartmentUserCountModel.SetPageBreak = false;
                }
                else
                {
                    cRDepartmentUserCountModel.SetPageBreak = true;
                }


                cRDepartmentUserCountList.Add(cRDepartmentUserCountModel);

            }
            
            return cRDepartmentUserCountList;

        }
    }
}