using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Text;
using DAL;
using BAL;

public partial class RUChart : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlDataAdapter da;
    DataTable dtResult = new DataTable();
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();
    DateTime lastLogin;
    DateTime lastLogout;
    string TeamName;
    string[] z;
    SqlDBHelper objDB = new SqlDBHelper();
    clsUtilization objUtil = new clsUtilization();
    string SQLQuery;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Context.Session != null)
        {
            if (Session.IsNewSession)
            {
                HttpCookie newSessionIdCookie = Request.Cookies["ASP.NET_SessionId"];
                if (newSessionIdCookie != null)
                {
                    string newSessionIdCookieValue = newSessionIdCookie.Value;
                    if (newSessionIdCookieValue != string.Empty)
                    {
                        if (!Page.IsCallback)
                        {
                            Response.Redirect("Default.aspx");
                        }
                        else
                        {
                            Response.RedirectLocation = Page.ResolveUrl("~/Default.aspx");
                        }
                    }
                }
            }
        }
        if (!IsPostBack)
        {

            if (Session["access"].ToString() == "3" || Session["access"].ToString() == "2" || Session["access"].ToString() == "1")
            {
                ds = GetTeamName(Convert.ToInt32(Session["team"]));

                if (ds.Tables["Team"].Rows.Count > 0)
                {
                    TeamName = ds.Tables["Team"].Rows[0]["T_TeamName"].ToString();
                }
                // BindURChart();
                BindRUPieChart();

            }
            else
            {
                LastLogin1();
                LastLogout1();
                LoadUserCleints();
            }
        }
    }

    private void BuildRUTable()
    {
        DataColumn dc;
        try
        {
            dc = new DataColumn("Employee Name");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Team Name");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total Hours", typeof(double));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Absent Hours");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("RU%");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("NonTask");
            dtResult.Columns.Add(dc);

        }
        catch (Exception)
        {


        }
    }

    private DataSet GetEmployees(int teamId)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId",teamId),
           };

        
        SQLQuery = "SELECT UL_User_Name FROM RTM_User_List WHERE UL_Team_Id =@teamId";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "users");
        return ds;
    }

    private DataSet GetLoginTime(string user)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",user),
               new SqlParameter("@date6",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
               new SqlParameter("@DateAd",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() ),
           };
       
        SQLQuery = "SELECT LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @date6 and @DateAd and LA_Log_Action = 'First Activity'";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "Login");
        return ds;
    }

    private DataSet GetLogoutTime(string user, DateTime date1)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",user),
               new SqlParameter("@date1",date1),
           };
        
        SQLQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name =@user and LA_Start_Date_Time >=@date1 and LA_Log_Action = 'Last Activity'";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "Logout");
        return ds;
    }

    private DataSet GetTaskHours(string user, DateTime start, DateTime end)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",user),
               new SqlParameter("@start",start),
              new SqlParameter("@end",end),
           };
        
        SQLQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records , RTM_SubTask_List where R_SubTask = STL_ID and R_User_Name=@user and R_Start_Date_Time BETWEEN @start and @end and R_Duration != 'HH:MM:SS' and STL_SubTask != 'Non Task'";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "Task");
        return ds;
    }

    private DataSet GetLogHours(string user, DateTime start, DateTime end)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",user),
               new SqlParameter("@start",start),
              new SqlParameter("@end",end),
           };
        SQLQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_User_Name=@user and LA_Start_Date_Time BETWEEN @start  and @end and LA_Duration != 'HH:MM:SS' and LA_Reason != 'Break' and LA_Reason != 'Others'";
        //da.Fill(ds, "Log");
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "Log");
        return ds;
    }

    private DataSet GetLeaves(string user)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",user),
               new SqlParameter("@dateAd",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString()),
              new SqlParameter("@date6",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           };
        
        SQLQuery = "SELECT CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) From RTM_Log_Actions where LA_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @date6 and @dateAd and LA_Log_Action = 'First Activity' GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "Leaves");
        return ds;
    }

    private DataSet GetNonTaskRecords(string user, DateTime start, DateTime end)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",user),
               new SqlParameter("@start",start),
               new SqlParameter("@end",end),
           };
       
        SQLQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records, RTM_SubTask_List where R_SubTask = STL_ID and R_User_Name=@user and R_Start_Date_Time BETWEEN @start and @end and R_Duration != 'HH:MM:SS' and STL_SubTask = 'Non Task'";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "NonTaskRecords");
        return ds;
    }

    private DataSet GetNonTaskLogs(string user, DateTime start, DateTime end)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",user),
               new SqlParameter("@start",start),
               new SqlParameter("@end",end),
           };
        
        SQLQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_User_Name=@user and LA_Start_Date_Time BETWEEN @start and @end and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Others'";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "NonTaskLog");
        return ds;
    }

    private void BindURChart()
    {
        DataRow dr;

        double totalTaskHours = 0;
        double totalLogHours = 0;
        double totalWorkHours;
        double RUPercent = 0;
        int leaveHours = 0;
        double nonTaskRecord = 0;
        double nonTaskLog = 0;
        double totalNonTaskHours = 0;
        DateTime logoutTime;
        BuildRUTable();

        //ds = GetTeamName(Convert.ToInt32(Session["team"]));

        //if (ds.Tables["Team"].Rows.Count > 0)
        //{
        //    TeamName = ds.Tables["Team"].Rows[0]["T_TeamName"].ToString();
        //}



        ds = GetEmployees(Convert.ToInt32(Session["team"]));

        if (ds.Tables["users"].Rows.Count > 0)
        {
            foreach (DataRow dr1 in ds.Tables["users"].Rows)
            {
                if (ds.Tables.Contains("Login"))
                {
                    ds.Tables.Remove(ds.Tables["Login"]);
                }
                if (ds.Tables.Contains("Logout"))
                {
                    ds.Tables.Remove(ds.Tables["Logout"]);
                }
                if (ds.Tables.Contains("Task"))
                {
                    ds.Tables.Remove(ds.Tables["Task"]);
                }
                if (ds.Tables.Contains("Log"))
                {
                    ds.Tables.Remove(ds.Tables["Log"]);
                }
                if (ds.Tables.Contains("Leaves"))
                {
                    ds.Tables.Remove(ds.Tables["Leaves"]);
                }
                if (ds.Tables.Contains("NonTaskRecords"))
                {
                    ds.Tables.Remove(ds.Tables["NonTaskRecords"]);
                }
                if (ds.Tables.Contains("NonTaskLog"))
                {
                    ds.Tables.Remove(ds.Tables["NonTaskLog"]);
                }

                totalTaskHours = 0;
                totalLogHours = 0;
                totalWorkHours = 0;
                RUPercent = 0;
                leaveHours = 0;
                nonTaskRecord = 0;
                nonTaskLog = 0;
                totalNonTaskHours = 0;
                string username = dr1["UL_User_Name"].ToString();


                ds = GetLoginTime(username);

                if (ds.Tables["Login"].Rows.Count > 0)
                {
                    DateTime loginTime = Convert.ToDateTime(ds.Tables["Login"].Rows[0]["LA_Start_Date_Time"]);

                    DataRow lastRow = ds.Tables["Login"].Rows[ds.Tables["Login"].Rows.Count - 1];


                    ds = GetLogoutTime(username, Convert.ToDateTime(lastRow["LA_Start_Date_Time"]));
                    if (ds.Tables["Logout"].Rows.Count > 0)
                    {
                        logoutTime = Convert.ToDateTime(ds.Tables["Logout"].Rows[0]["LA_Start_Date_Time"]);

                        ds = GetTaskHours(username, loginTime, logoutTime);

                        if (ds.Tables["Task"].Rows.Count > 0 && ds.Tables["Task"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            totalTaskHours = Convert.ToDouble(ds.Tables["Task"].Rows[0]["hour"] + "." + ds.Tables["Task"].Rows[0]["minute"]);
                        }


                        ds = GetLogHours(username, loginTime, logoutTime);

                        if (ds.Tables["Log"].Rows.Count > 0 && ds.Tables["Log"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            totalLogHours = Convert.ToDouble(ds.Tables["Log"].Rows[0]["hour"] + "." + ds.Tables["Log"].Rows[0]["minute"]);
                        }


                        totalWorkHours = totalTaskHours + totalLogHours;

                        ds = GetLeaves(username);
                        if (ds.Tables["Leaves"].Rows.Count <= 5)
                        {
                            leaveHours = (5 - Convert.ToInt32(ds.Tables["Leaves"].Rows.Count)) * 8;
                        }


                        if (totalWorkHours != 0)
                        {
                            RUPercent = (totalWorkHours / (40 - leaveHours)) * 100;

                            RUPercent = Math.Round(RUPercent, 2, MidpointRounding.AwayFromZero);
                        }

                        ds = GetNonTaskRecords(username, loginTime, logoutTime);

                        if (ds.Tables["NonTaskRecords"].Rows.Count > 0 && ds.Tables["NonTaskRecords"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            nonTaskRecord = Convert.ToDouble(ds.Tables["NonTaskRecords"].Rows[0]["hour"] + "." + ds.Tables["NonTaskRecords"].Rows[0]["minute"]);
                        }

                        ds = GetNonTaskLogs(username, loginTime, logoutTime);

                        if (ds.Tables["NonTaskLog"].Rows.Count > 0 && ds.Tables["NonTaskLog"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            nonTaskLog = Convert.ToDouble(ds.Tables["NonTaskLog"].Rows[0]["hour"] + "." + ds.Tables["NonTaskLog"].Rows[0]["minute"]);
                        }

                        totalNonTaskHours = nonTaskRecord + nonTaskLog;

                        dr = dtResult.NewRow();

                        dr["Employee Name"] = username;
                        dr["Team Name"] = "";
                        dr["Total Hours"] = totalWorkHours;
                        dr["Absent Hours"] = leaveHours;
                        dr["RU%"] = RUPercent;
                        dr["NonTask"] = totalNonTaskHours;

                        dtResult.Rows.Add(dr);
                    }
                }
            }
            DataView view = dtResult.DefaultView;
            view.Sort = "Total Hours DESC";
            dtResult = view.ToTable();

            string[] x = new string[dtResult.Rows.Count];
            double[] y = new double[dtResult.Rows.Count];
            checked
            {
                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    x[i] = dtResult.Rows[i]["Employee Name"].ToString();
                    y[i] = Convert.ToDouble(dtResult.Rows[i]["RU%"]);
                }
            }
            

            Chart1.Series[0].Points.DataBindXY(x, y);

            this.Chart1.Series[0].BorderWidth = 1;
            this.Chart1.Series[0].Label = "#VALY";
            this.Chart1.Series[0]["BarLabelStyle"] = "Outside";
            Chart1.ChartAreas["ChartArea1"].AxisX.Title = "Employees";
            Chart1.ChartAreas["ChartArea1"].AxisY.Title = "In Percentage";
            //this.Chart1.Series[0].LegendText = "#VALX (#VALY - #PERCENT)"; // "#VALX (#PERCENT)";
            Chart1.ChartAreas[0].AxisY.LabelStyle.Angle = 45;
            Chart1.Titles.Add(CreateTitle("RTM Resource Utilization - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
            Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

            Chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            Chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
        }
    }


    private void LoadUserCleints()
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",Session["username"].ToString()),
               new SqlParameter("@lastLogin",lastLogin),
               new SqlParameter("@lastLogout",lastLogout),
               new SqlParameter("@date", DateTime.Now.AddDays(-1).ToShortDateString()) 
           };

        if (Session["preference"].ToString() == "2")
        {
            SQLQuery = "SELECT C.CL_ClientName, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Client_List C where  B.R_Client = C.CL_ID and R_User_Name=@user  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date GROUP BY C.CL_ClientName ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
        }
        else
        {
            SQLQuery = "SELECT C.CL_ClientName, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Client_List C where  B.R_Client = C.CL_ID and R_User_Name=@user  and R_Duration != 'HH:MM:SS' and R_Start_Date_Time BETWEEN @lastLogin and @lastLogout GROUP BY C.CL_ClientName ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
        }
        dtResult = objDB.ExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters);

        string[] x = new string[dtResult.Rows.Count];
        double[] y = new double[dtResult.Rows.Count];
        checked
        {
            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                x[i] = dtResult.Rows[i][0].ToString();
                y[i] = Convert.ToDouble(dtResult.Rows[i][1] + "." + dtResult.Rows[i][2]);
            }
        }
       

        Chart1.Series[0].Points.DataBindXY(x, y);

        this.Chart1.Series[0].BorderWidth = 1;
        this.Chart1.Series[0].Label = "#VALY";
        this.Chart1.Series[0]["BarLabelStyle"] = "Outside";
        //this.Chart1.Series[0].LegendText = "#VALX (#VALY - #PERCENT)"; // "#VALX (#PERCENT)";
        Chart1.ChartAreas[0].AxisY.LabelStyle.Angle = 45;
        Chart1.Titles.Add(CreateTitle("RTM Client Usage Data (" + lastLogin.ToShortDateString() + ")"));
        Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
        Chart1.ChartAreas["ChartArea1"].AxisX.Title = "Employees";
        Chart1.ChartAreas["ChartArea1"].AxisY.Title = "In Hours";
        Chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
        Chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
    }

    private void LastLogin1()
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",Session["username"].ToString()),
               new SqlParameter("@date",DateTime.Now.AddDays(-1)),
           };
        

        SQLQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) <=@date and LA_Log_Action = 'First Activity' ORDER BY LA_ID DESC";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "login");

        if (ds.Tables["login"].Rows.Count > 0)
        {
            lastLogin = Convert.ToDateTime(ds.Tables["login"].Rows[0]["LA_Start_Date_Time"]);
        }
        else
        {
            lastLogin = DateTime.Now.AddDays(-1);
        }
    }

    private void LastLogout1()
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user",Session["username"].ToString()),
               new SqlParameter("@lastLogin",lastLogin),
           };
        

        SQLQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @use and LA_Start_Date_Time >=@lastLogin and LA_Log_Action = 'Last Activity' ORDER BY LA_ID";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "logout");

        if (ds.Tables["logout"].Rows.Count > 0)
        {
            lastLogout = Convert.ToDateTime(ds.Tables["logout"].Rows[0]["LA_Start_Date_Time"]);
        }
        else
        {
            lastLogout = lastLogin;
        }
    }

    public Title CreateTitle(string name)
    {
        Title title = new Title();
        title.Text = name;
        title.ShadowColor = System.Drawing.Color.FromArgb(32, 0, 0, 0);
        title.Font = new System.Drawing.Font("Trebuchet MS", 14F, FontStyle.Bold);
        title.ShadowOffset = 3;
        title.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
        return title;
    }

    private DataSet LoadRUDetails()
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@team",Convert.ToInt32(Session["team"])),
           };
        if (ds.Tables.Contains("ru"))
        {
            ds.Tables.Remove(ds.Tables["ru"]);
        }
       
        SQLQuery = "select RU_UserName as [User Name], RU_WorkHours as [Total Hours], RU_Leaves as [Leave Hours], RU_Percent as [RU%] from RTM_ResourceUtil where RU_TeamId =@team order by CAST(RU_WorkHours AS DECIMAL(10, 2)) desc";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "ru");
        return ds;
    }

    private void BindRUPieChart()
    {
        dt = new DataTable();
        dt = objUtil.GetRUUsers(Session["team"].ToString(), DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString(), DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString(), TeamName);
        string[] x = new string[dt.Rows.Count];
        double[] y = new double[dt.Rows.Count];
        z = new string[dt.Rows.Count];

        checked
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                x[i] = dt.Rows[i]["Employee Name"].ToString();
                y[i] = Convert.ToDouble(dt.Rows[i]["RU%"]);
                z[i] = dt.Rows[i]["Absent Hours"].ToString();
            }
        }

        //ds = LoadRUDetails();

        //string[] x = new string[ds.Tables["ru"].Rows.Count];
        //double[] y = new double[ds.Tables["ru"].Rows.Count];
        //z = new string[ds.Tables["ru"].Rows.Count];
        //checked
        //{
        //    for (int i = 0; i < ds.Tables["ru"].Rows.Count; i++)
        //    {
        //        x[i] = ds.Tables["ru"].Rows[i]["User Name"].ToString();
        //        y[i] = Convert.ToDouble(ds.Tables["ru"].Rows[i]["Total Hours"]);
        //        z[i] = ds.Tables["ru"].Rows[i]["Leave Hours"].ToString();
        //    }
        //}
        

        Chart1.Series[0].Points.DataBindXY(x, y);
        this.Chart1.Series[0].BorderWidth = 1;
        this.Chart1.Series[0].Label = "#VALY";
        this.Chart1.Series[0]["BarLabelStyle"] = "Outside";
        Chart1.ChartAreas["ChartArea1"].AxisX.Title = "Employees";
        Chart1.ChartAreas["ChartArea1"].AxisY.Title = "In Hours";
        //this.Chart1.Series[0].LegendText = "#VALX (#VALY - #PERCENT)"; // "#VALX (#PERCENT)";
        Chart1.ChartAreas[0].AxisY.LabelStyle.Angle = 45;
        Chart1.Titles.Add(CreateTitle("RTM Resource Utilization - " + TeamName + " - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
        Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

        Chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
        Chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

        for (int i = 0; i <= Chart1.Series[0].Points.Count - 1; i++)
        {
            switch(i)
            {
                case 1:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.DarkGreen;
                    break;

                case 2:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.Navy;
                    break;

                case 3:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.DarkOliveGreen;
                    break;

                case 4:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.Gold;
                    break;

                case 5:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.Chocolate;
                    break;

                case 6:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.Green;
                    break;

                case 7:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.OrangeRed;
                    break;

                case 8:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.Gray;
                    break;

                case 9:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.MediumVioletRed;
                    break;

                case 10:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.SkyBlue;
                    break;

                case 11:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.DarkGoldenrod;
                    break;

                case 12:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.DarkGreen;
                    break;

                case 13:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.DarkOrange;
                    break;

                case 14:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.Firebrick;
                    break;
                default:
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.Yellow;
                    break;
            }
                
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        ds = GetTeamName(Convert.ToInt32(Session["team"]));

        if (ds.Tables["Team"].Rows.Count > 0)
        {
            TeamName = ds.Tables["Team"].Rows[0]["T_TeamName"].ToString();
        }

        BindRUPieChart();

        Document pdfDoc = new Document(PageSize.A3, 10f, 10f, 10f, 0f);
        PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        pdfDoc.Open();

        using (MemoryStream stream = new MemoryStream())
        {
            Chart1.SaveImage(stream, ChartImageFormat.Png);
            iTextSharp.text.Image chartImage = iTextSharp.text.Image.GetInstance(stream.GetBuffer());
            chartImage.ScalePercent(75f);
            pdfDoc.Add(chartImage);
        }

        pdfDoc.Close();
        Response.ContentType = "application/pdf";
        if (Session["access"].ToString() == "3" || Session["access"].ToString() == "2" || Session["access"].ToString() == "1")
        {
            Response.AddHeader("content-disposition", "attachment;" +
                                      "filename=Resource Utilisation - " + TeamName + ".pdf");
        }
        else
        {
            Response.AddHeader("content-disposition", "attachment;" +
                                      "filename=Client Usage - " + TeamName + ".pdf");
        }

        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Write(pdfDoc);
        Response.End();
    }

    private DataSet GetTeamName(int id)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@id",id),
           };
        
        SQLQuery = "select T_TeamName from RTM_Team_List where T_ID=@id";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQLQuery, CommandType.Text, parameters, "Team");
        return ds;
    }

    protected void Chart1_Customize(object sender, EventArgs e)
    {
        //foreach (var labeln in Chart1.ChartAreas[0].AxisY.CustomLabels)
        //{
        //    labeln.Text = "2";
        //}

        //foreach (var value in z)
        //{

        //    Chart1.Series[0].Label = "#VALY" +value.ToString();
        //}

    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        ds = GetTeamName(Convert.ToInt32(Session["team"]));

        if (ds.Tables["Team"].Rows.Count > 0)
        {
            TeamName = ds.Tables["Team"].Rows[0]["T_TeamName"].ToString();
        }

        BindRUPieChart();

        Response.Clear();
        Response.Buffer = true;

        Response.AddHeader("content-disposition", "attachment;filename=Resource Utilization-" + TeamName + ".csv");

        Response.Clear();
        Response.Buffer = true;

        Response.Charset = "";
        Response.ContentType = "application/text";
        StringBuilder sb = new StringBuilder();
        checked
        {
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                //add separator
                sb.Append(dt.Columns[k].ColumnName + ',');
            }
        }
        
        sb.Append("\r\n");
        checked
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    //add separator
                    sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                }
                //append new line
                sb.Append("\r\n");
            }
        }
       

        Response.Output.Write(sb.ToString());
        Response.Flush();
        Response.End();
    }
}