<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>
<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        RegisterRoutes(RouteTable.Routes);
        // Code that runs on application startup
        Application["TotalOnlineUsers"] = 0; 
    }

    static void RegisterRoutes(RouteCollection routes)
    {
        routes.MapPageRoute("Login", "Login", "~/Default.aspx");
        routes.MapPageRoute("Weekly View", "WeeklyView", "~/RecordsWeeklyView.aspx");
        routes.MapPageRoute("Clock In Clock Out", "ClockInClockOut", "~/MyTimeCard.aspx");
        routes.MapPageRoute("Admin Dashboard", "admindashboard", "~/DashBoard_Admin.aspx");
        routes.MapPageRoute("Time Card", "timecard", "~/ReportHome.aspx");
        routes.MapPageRoute("Utilization", "utilization", "~/UtilizationReports.aspx");
        routes.MapPageRoute("Expanded RTM Hours", "ExpandedRTMHours", "~/ExpandedRTMHours.aspx");
        routes.MapPageRoute("Task and Subtask Details", "taskSubtaskDetails", "~/DetailedReport.aspx");
        routes.MapPageRoute("Late Login", "latelogin", "~/LateInEarlyOff.aspx");
        routes.MapPageRoute("Internal Metrics", "internalmetrics", "~/InternalMetrics.aspx");
        routes.MapPageRoute("Non-Compliance", "NonCompliance", "~/NonComplianceReport.aspx");
        routes.MapPageRoute("Weekly Hours Greater than 40", "weeklyhoursgreaterthan40", "~/WeeklyHoursReportGreaterThan40.aspx");
        routes.MapPageRoute("Summary Client hours by employee", "ClientHoursByEmployee", "~/SummaryClientHoursbyEmployees.aspx");
        routes.MapPageRoute("Summary Client hours by Role", "ClientHoursByRole", "~/SummaryClientHoursbyRoles.aspx");
        
        //Standard T-Sheet Reports
        routes.MapPageRoute("Hourly Time Card", "Hourlytimecard", "~/HourlyTimeCardReport.aspx");
        routes.MapPageRoute("Hours by Jobcode", "HoursbyJobcode", "~/HoursbyFullJobCode.aspx");
        routes.MapPageRoute("Hours by Team", "HoursbyTeam", "~/HoursbyTeam.aspx");
        routes.MapPageRoute("Hours by Date", "HoursbyDate", "~/HoursbyDate.aspx");
        routes.MapPageRoute("Hours by Employee", "HoursbyEmployee", "~/HoursByEmployee.aspx");
        routes.MapPageRoute("Tangoe Approvals", "Tangoeapprovals", "~/TangoeApprovals.aspx");
        routes.MapPageRoute("MSP Hours", "MSPhours", "~/MSPHoursReport.aspx");
        routes.MapPageRoute("Payroll Hours", "PayrollHours", "~/Payroll_Hours_Summary.aspx");
        routes.MapPageRoute("MSP Hours Report (Before 04-01-2017)", "TsheetMSPHours", "~/TSheetMSPHoursReport.aspx");
        
        //ADM Page
        routes.MapPageRoute("Employee Hours by Week", "Employeehoursbyweek", "~/EmployeeHoursByWeek.aspx");
        routes.MapPageRoute("AverageHoursByClient", "AverageHoursByClient", "~/AvarageHoursByClient.aspx");
        routes.MapPageRoute("Content Management", "Contentmanagement", "~/ContentManagementReport.aspx");
        
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started
        Application.Lock();
        Application["TotalOnlineUsers"] = (int)Application["TotalOnlineUsers"] + 1;
        Application.UnLock(); 
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
        Application.Lock();
        Application["TotalOnlineUsers"] = (int)Application["TotalOnlineUsers"] - 1;
        Application.UnLock();  
    }
       
</script>
