﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using DAL;
using BAL;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

public partial class DashBoard_Admin : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlDataAdapter da;
    DataTable dt = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    clsUtilization objUtil = new clsUtilization();
    DataSet ds = new DataSet();
    string TeamName;
    SqlDBHelper objDB = new SqlDBHelper();
    string sQuery;
    DataTable dtResult = new DataTable();
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
            string acc = Session["access"].ToString();
            string loc = Session["Location"].ToString();
            if (loc == "ADM")
            {
                Response.Redirect("~/ADMDashboard.aspx");
            }
            if (acc != "1")
            {
                if (acc != "2")
                {
                    Response.Redirect("~/DashBoard.aspx");
                }
            }
            BindTeamList();
        }
    }

    private void BindTeamList()
    {
        //SqlParameter[] parameters = new SqlParameter[]
        //{
        //    new SqlParameter("@location", Session["Location"].ToString())
        //};

        //sQuery = "SELECT * FROM RTM_Team_List where T_Location=@location and T_Active = 1 order by T_TeamName";
        
        //ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Team");

        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());

        ddlTeam.DataSource = dt;// ds.Tables["Team"];
        ddlTeam.DataTextField = "T_TeamName";
        ddlTeam.DataValueField = "T_ID";
        ddlTeam.DataBind();
        ddlTeam.Items.Insert(0, "--Select Team--");
        ddlTeam.SelectedIndex = 0;
    }

    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlTeam.SelectedIndex != 0)
            {
                TeamName = ddlTeam.SelectedItem.Text;
                Session["team"] = ddlTeam.SelectedValue;
                BindClientPieChart();
                BindRUPieChart();
                BindTaskPieChart();
                BindSummaryChart();
                BindWeekChart();
                dvCharts.Visible = true;
            }
            else
            {
                lblClientHeader.Visible = false;
                lblRUHeader.Visible = false;
                lblTaskHeader.Visible = false;
                lblSummaryHeading.Visible = false;
                lblWeekHeading.Visible = false;
                dvCharts.Visible = false;
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = System.Drawing.Color.Red;
        }
    }

    private DataSet LoadClient()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("client"))
        {
            ds.Tables.Remove(ds.Tables["client"]);
        }
       
        sQuery = "SELECT TOP 5 C.CL_ClientName, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Client_List C where  B.R_Client = C.CL_ID and R_TeamId=@team and R_Duration != 'HH:MM:SS' and R_Start_Date_Time BETWEEN @fromDt AND @toDt GROUP BY C.CL_ClientName ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "client");
        return ds;
    }

    private void BindClientPieChart()
    {
        ds = LoadClient();

        string[] x = new string[ds.Tables["client"].Rows.Count];
        double[] y = new double[ds.Tables["client"].Rows.Count];
        checked
        {
            for (int i = 0; i < ds.Tables["client"].Rows.Count; i++)
            {
                //TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]); 
                x[i] = ds.Tables["client"].Rows[i]["CL_ClientName"].ToString();
                //y[i] = Convert.ToDouble(ds.Tables["client"].Rows[i]["hour"] + "." + ds.Tables["client"].Rows[i]["minute"]);
                y[i] = Math.Round(Convert.ToDouble(ds.Tables["client"].Rows[i]["hour"]) + (Convert.ToDouble(ds.Tables["client"].Rows[i]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["client"].Rows[i]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
            }
        }
        

        Chart1.Series[0].Points.DataBindXY(x, y);

        this.Chart1.Series[0].BorderWidth = 1;
        this.Chart1.Series[0].Label = "#VALY"; // "#PERCENT{P2}";
        this.Chart1.Series[0].LegendText = "#VALX (#VALY)";
        lblClientHeader.Text = "Client Usage - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")";
        lblClientHeader.Visible = true;
        //Chart1.Titles.Add(CreateTitle("Client Usage - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
    }

    //Resource Utilization Pie Chart

    private DataSet LoadRUDetails()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue)
        };
        if (ds.Tables.Contains("ru"))
        {
            ds.Tables.Remove(ds.Tables["ru"]);
        }
        sQuery = "select TOP 5 RU_UserName, RU_Percent from RTM_ResourceUtil where RU_TeamId =@team order by RU_Percent desc";
        
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "ru");
        return ds;
    }

    private void BindRUPieChart()
    {
        dt = new DataTable();
        dt = new DataTable();
        dt = objUtil.GetRUUsers(ddlTeam.SelectedValue, DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString(), DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString(), ddlTeam.SelectedItem.Text);

        string[] x = new string[dt.Rows.Count];
        double[] y = new double[dt.Rows.Count];

        checked
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 5)
                {
                    break;
                }
                x[i] = dt.Rows[i]["Employee Name"].ToString();
                y[i] = Convert.ToDouble(dt.Rows[i]["RU%"]);
            }
        }

        //ds = LoadRUDetails();

        //string[] x = new string[ds.Tables["ru"].Rows.Count];
        //double[] y = new double[ds.Tables["ru"].Rows.Count];
        //checked
        //{
        //    for (int i = 0; i < ds.Tables["ru"].Rows.Count; i++)
        //    {
        //        x[i] = ds.Tables["ru"].Rows[i]["RU_UserName"].ToString();
        //        y[i] = Convert.ToDouble(ds.Tables["ru"].Rows[i]["RU_Percent"]);
        //    }
        //}
       

        Chart2.Series[0].Points.DataBindXY(x, y);

        this.Chart2.Series[0].BorderWidth = 1;
        this.Chart2.Series[0].Label = "#VALY %";
        this.Chart2.Series[0].LegendText = "#VALX (#VALY)";
        lblRUHeader.Text = "Resource Utilization - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")";
        lblRUHeader.Visible = true;
        //Chart2.Titles.Add(CreateTitle("Resource Utilization - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
    }

    //Task Usage 

    private DataSet LoadTask()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };

        if (ds.Tables.Contains("task"))
        {
            ds.Tables.Remove(ds.Tables["task"]);
        }
        sQuery = "SELECT TOP 5 C.TL_Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Task_List C where  B.R_Task = C.TL_ID and R_TeamId=@team  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @fromDt and @toDt GROUP BY C.TL_Task  ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
        
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "task");
        return ds;
    }

    private void BindTaskPieChart()
    {
        ds = LoadTask();

        string[] x = new string[ds.Tables["task"].Rows.Count];
        double[] y = new double[ds.Tables["task"].Rows.Count];
        checked
        {
            for (int i = 0; i < ds.Tables["task"].Rows.Count; i++)
            {
                //TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]); 
                x[i] = ds.Tables["task"].Rows[i]["TL_Task"].ToString();
                //y[i] = Convert.ToDouble(ds.Tables["task"].Rows[i]["hour"] + "." + ds.Tables["task"].Rows[i]["minute"]);
                y[i] = Math.Round(Convert.ToDouble(ds.Tables["task"].Rows[i]["hour"]) + (Convert.ToDouble(ds.Tables["task"].Rows[i]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["task"].Rows[i]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
            }
        }
        

        Chart3.Series[0].Points.DataBindXY(x, y);

        this.Chart3.Series[0].BorderWidth = 1;
        this.Chart3.Series[0].Label = "#VALY"; // "#PERCENT{P2}";
        this.Chart3.Series[0].LegendText = "#VALX (#VALY)";
        lblTaskHeader.Text = "Task Usage - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")";
        lblTaskHeader.Visible = true;
        //Chart3.Titles.Add(CreateTitle("Task Usage - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
    }

    //Team Total Summary

    private void buildSummaryTable()
    {
        DataColumn dc;
        try
        {

            dc = new DataColumn("Team");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Log");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Hours");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Percent");
            dtResult.Columns.Add(dc);

        }
        catch (Exception)
        {


        }
    }

    private DataSet TotalTask()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("totalTask"))
        {
            ds.Tables.Remove(ds.Tables["totalTask"]);
        }

        sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records , RTM_SubTask_List where R_SubTask = STL_ID and R_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @fromDt and @toDt and R_Duration != 'HH:MM:SS'";
        
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "totalTask");
        return ds;
    }

    private DataSet TotalBreak()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("Break"))
        {
            ds.Tables.Remove(ds.Tables["Break"]);
        }
        sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Break'";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Break");
        return ds;
    }

    private DataSet TotalMeeting()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("Meeting"))
        {
            ds.Tables.Remove(ds.Tables["Meeting"]);
        }
        sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Meetings' or LA_Reason = 'Meeting')";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Meeting");
        return ds;
    }

    private DataSet TotalConfCall()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("Call"))
        {
            ds.Tables.Remove(ds.Tables["Call"]);
        }
        sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Conference-Call'";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Call");
        return ds;
    }

    private DataSet TotalNonTask1()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("nonTask1"))
        {
            ds.Tables.Remove(ds.Tables["nonTask1"]);
        }
        sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Others' or LA_Reason = 'Non-Task')";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "nonTask1");
        return ds;
    }

    private DataSet TotalNonTask2()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("nonTask2"))
        {
            ds.Tables.Remove(ds.Tables["nonTask2"]);
        }
        sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records, RTM_SubTask_List where R_SubTask = STL_ID and R_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @fromDt and @toDt and R_Duration != 'HH:MM:SS' and STL_SubTask = 'Non Task'";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "nonTask2");
        return ds;
    }

    private DataSet TotalPeerSupport()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };

        if (ds.Tables.Contains("peer"))
        {
            ds.Tables.Remove(ds.Tables["peer"]);
        }
        sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Peer Support'";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "peer");
        return ds;
    }

    private void BindSummaryChart()
    {


        DataRow dr;
        buildSummaryTable();
        double totalTask = 0;
        double totalBreak = 0;
        double totalMeet = 0;
        double totalCall = 0;
        double totalPeerSup = 0;
        double totalNonTask1 = 0;
        //double totalNonTask2 = 0;
        double totalNonTask = 0;

        ds = TotalTask();

        if (ds.Tables["totalTask"].Rows.Count > 0 && ds.Tables["totalTask"].Rows[0]["hour"].ToString().Length > 0)
        {
            //totalTask = Convert.ToDouble(ds.Tables["totalTask"].Rows[0]["hour"] + "." + ds.Tables["totalTask"].Rows[0]["minute"]);
            totalTask = Math.Round(Convert.ToDouble(ds.Tables["totalTask"].Rows[0]["hour"]) + (Convert.ToDouble(ds.Tables["totalTask"].Rows[0]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["totalTask"].Rows[0]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
            dr = dtResult.NewRow();
            // dr["Team"] = ddlTeam.SelectedItem.Text;
            dr["Log"] = "Task";
            dr["Hours"] = totalTask;
            dr["Percent"] = "";

            dtResult.Rows.Add(dr);
        }

        ds = TotalMeeting();

        if (ds.Tables["Meeting"].Rows.Count > 0 && ds.Tables["Meeting"].Rows[0]["hour"].ToString().Length > 0)
        {
            //totalMeet = Convert.ToDouble(ds.Tables["Meeting"].Rows[0]["hour"] + "." + ds.Tables["Meeting"].Rows[0]["minute"]);
            totalMeet = Math.Round(Convert.ToDouble(ds.Tables["Meeting"].Rows[0]["hour"]) + (Convert.ToDouble(ds.Tables["Meeting"].Rows[0]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["Meeting"].Rows[0]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
            dr = dtResult.NewRow();
            // dr["Team"] = ddlTeam.SelectedItem.Text;
            dr["Log"] = "Meeting";
            dr["Hours"] = totalMeet;
            dr["Percent"] = "";

            dtResult.Rows.Add(dr);
        }

        ds = TotalConfCall();

        if (ds.Tables["Call"].Rows.Count > 0 && ds.Tables["Call"].Rows[0]["hour"].ToString().Length > 0)
        {
            //totalCall = Convert.ToDouble(ds.Tables["Call"].Rows[0]["hour"] + "." + ds.Tables["Call"].Rows[0]["minute"]);
            totalCall = Math.Round(Convert.ToDouble(ds.Tables["Call"].Rows[0]["hour"]) + (Convert.ToDouble(ds.Tables["Call"].Rows[0]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["Call"].Rows[0]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
            dr = dtResult.NewRow();
            //dr["Team"] = ddlTeam.SelectedItem.Text;
            dr["Log"] = "Conference-Call";
            dr["Hours"] = totalCall;
            dr["Percent"] = "";

            dtResult.Rows.Add(dr);
        }

        //ds = TotalPeerSupport();

        //if (ds.Tables["peer"].Rows.Count > 0 && ds.Tables["peer"].Rows[0]["hour"].ToString().Length > 0)
        //{
        //    //totalPeerSup = Convert.ToDouble(ds.Tables["peer"].Rows[0]["hour"] + "." + ds.Tables["peer"].Rows[0]["minute"]);
        //    totalPeerSup = Math.Round(Convert.ToDouble(ds.Tables["peer"].Rows[0]["hour"]) + Convert.ToDouble(ds.Tables["peer"].Rows[0]["minute"]) / 60, 2, MidpointRounding.AwayFromZero);
        //    dr = dtResult.NewRow();
        //    //dr["Team"] = ddlTeam.SelectedItem.Text;
        //    dr["Log"] = "Peer Support";
        //    dr["Hours"] = totalPeerSup;
        //    dr["Percent"] = "";

        //    dtResult.Rows.Add(dr);
        //}

        ds = TotalBreak();

        if (ds.Tables["Break"].Rows.Count > 0 && ds.Tables["Break"].Rows[0]["hour"].ToString().Length > 0)
        {
            //totalBreak = Convert.ToDouble(ds.Tables["Break"].Rows[0]["hour"] + "." + ds.Tables["Break"].Rows[0]["minute"]);
            totalBreak = Math.Round(Convert.ToDouble(ds.Tables["Break"].Rows[0]["hour"]) + (Convert.ToDouble(ds.Tables["Break"].Rows[0]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["Break"].Rows[0]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
            dr = dtResult.NewRow();
            //dr["Team"] = ddlTeam.SelectedItem.Text;
            dr["Log"] = "Break";
            dr["Hours"] = totalBreak;
            dr["Percent"] = "";

            dtResult.Rows.Add(dr);
        }

        ds = TotalNonTask1();

        if (ds.Tables["nonTask1"].Rows.Count > 0 && ds.Tables["nonTask1"].Rows[0]["hour"].ToString().Length > 0)
        {
            //totalNonTask1 = Convert.ToDouble(ds.Tables["nonTask1"].Rows[0]["hour"] + "." + ds.Tables["nonTask1"].Rows[0]["minute"]);
            totalNonTask1 = Math.Round(Convert.ToDouble(ds.Tables["nonTask1"].Rows[0]["hour"]) + (Convert.ToDouble(ds.Tables["nonTask1"].Rows[0]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["nonTask1"].Rows[0]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
        }

        //ds = TotalNonTask2();

        //if (ds.Tables["nonTask2"].Rows.Count > 0 && ds.Tables["nonTask2"].Rows[0]["hour"].ToString().Length > 0)
        //{
        //    totalNonTask2 = Convert.ToDouble(ds.Tables["nonTask2"].Rows[0]["hour"] + "." + ds.Tables["nonTask2"].Rows[0]["minute"]);
        //}

        totalNonTask = totalNonTask1;
        if (totalNonTask != 0)
        {
            dr = dtResult.NewRow();
            //dr["Employee"] = Session["username"].ToString();
            dr["Log"] = "Non Task";
            dr["Hours"] = totalNonTask;
            dr["Percent"] = "";

            dtResult.Rows.Add(dr);
        }

        // string[] x = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        string[] x = new string[dtResult.Rows.Count];
        double[] y = new double[dtResult.Rows.Count];
        checked
        {
            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                x[i] = dtResult.Rows[i]["Log"].ToString();
                y[i] = Convert.ToDouble(dtResult.Rows[i]["Hours"]);
            }
        }
        

        Chart4.Series[0].Points.DataBindXY(x, y);
        //this.Chart1.Series[0]["PieLabelStyle"] = "Outside";
        this.Chart4.Series[0].BorderWidth = 1;
        this.Chart4.Series[0].Label = "#VALY";

        Chart4.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
        Chart4.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
        lblSummaryHeading.Text = "Action Summary - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")";
        lblSummaryHeading.Visible = true;
        //Chart4.Titles.Add(CreateTitle("Action Summary - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
    }

    //Daywise chart

    private DataSet TotalLineTask()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("LineTask"))
        {
            ds.Tables.Remove(ds.Tables["LineTask"]);
        }
        sQuery = "SELECT datename(dw,CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time)))) as day, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records , RTM_SubTask_List where R_SubTask = STL_ID and R_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @fromDt and @toDt and R_Duration != 'HH:MM:SS'  GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time)))";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "LineTask");
        return ds;
    }

    private DataSet TotalLineBreak()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("LineBreak"))
        {
            ds.Tables.Remove(ds.Tables["LineBreak"]);
        }
        sQuery = "SELECT datename(dw,CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))) as day,sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Break' GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "LineBreak");
        return ds;
    }

    private DataSet TotalLineMeeting()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("LineMeeting"))
        {
            ds.Tables.Remove(ds.Tables["LineMeeting"]);
        }
        sQuery = "SELECT datename(dw,CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))) as day,sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Meetings' or LA_Reason = 'Meeting') GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "LineMeeting");
        return ds;
    }

    private DataSet TotalLineConfCall()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("LineCall"))
        {
            ds.Tables.Remove(ds.Tables["LineCall"]);
        }
        sQuery = "SELECT datename(dw,CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))) as day,sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Conference-Call' GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "LineCall");
        return ds;
    }

    private DataSet TotalLineNonTask1()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("LinenonTask1"))
        {
            ds.Tables.Remove(ds.Tables["LinenonTask1"]);
        }
        sQuery = "SELECT datename(dw,CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))) as day, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Others' or LA_Reason = 'Non-Task') GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "LinenonTask1");
        return ds;
    }

    private DataSet LinePeerSupport()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@team", ddlTeam.SelectedValue),
           new SqlParameter("@fromDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
           new SqlParameter("@toDt",DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("Linepeer"))
        {
            ds.Tables.Remove(ds.Tables["Linepeer"]);
        }
        sQuery = "SELECT datename(dw,CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))) as day, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @fromDt and @toDt and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Peer Support' GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Linepeer");
        return ds;
    }

    //private DataSet TotalLineNonTask2(string user, DateTime start, DateTime end)
    //{
    //    if (Session["access"].ToString() == "3" || Session["access"].ToString() == "2")
    //    {
    //        da = new SqlDataAdapter("SELECT datename(dw,CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time)))) as day, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records, RTM_SubTask_List where R_SubTask = STL_ID and R_TeamId='" + Convert.ToInt32(Session["team"]) + "' and R_Start_Date_Time BETWEEN '" + start + "' and '" + end + "' and R_Duration != 'HH:MM:SS' and STL_SubTask = 'Non Task' GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time)))", con);
    //    }
    //    else
    //    {
    //        da = new SqlDataAdapter("SELECT datename(dw,CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time)))) as day, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records, RTM_SubTask_List where R_SubTask = STL_ID and R_User_Name='" + user + "' and R_Start_Date_Time BETWEEN '" + start + "' and '" + end + "' and R_Duration != 'HH:MM:SS' and STL_SubTask = 'Non Task' GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time)))", con);
    //    }

    //    da.Fill(ds, "LinenonTask2");
    //    return ds;
    //}

    private void BindWeekChart()
    {
        //Chart5.Titles.Add(CreateTitle("Action Summary - Weekly (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
        lblWeekHeading.Text = "Action Summary - Weekly (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")";
        lblWeekHeading.Visible = true;
        // Chart5.ChartAreas["ChartArea1"].AxisY.Interval = 1;
        Chart5.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
        Chart5.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

        ds = TotalLineTask();

        if (ds.Tables["LineTask"].Rows.Count > 0 && ds.Tables["LineTask"].Rows[0]["hour"].ToString().Length > 0)
        {
            string[] x = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            //string[] x = new string[ds.Tables["LineTask"].Rows.Count];
            double[] y = new double[7];
            checked
            {
                for (int i = 0; i < ds.Tables["LineTask"].Rows.Count; i++)
                {
                    int index1 = Array.IndexOf(x, ds.Tables["LineTask"].Rows[i][0].ToString());
                    if (index1 >= 0)
                    {
                        x[index1] = ds.Tables["LineTask"].Rows[i]["day"].ToString();
                        //y[index1] = Convert.ToDouble(ds.Tables["LineTask"].Rows[i]["Hour"] + "." + ds.Tables["LineTask"].Rows[i]["minute"]);
                        y[index1] = Math.Round(Convert.ToDouble(ds.Tables["LineTask"].Rows[i]["Hour"]) + (Convert.ToDouble(ds.Tables["LineTask"].Rows[i]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["LineTask"].Rows[i]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                    }

                }
            }
            

            Chart5.Series[0].Points.DataBindXY(x, y);
            this.Chart5.Series[0].BorderWidth = 1;
            this.Chart5.Series[0].Label = "#VALY";

        }

        ds = TotalLineMeeting();

        if (ds.Tables["LineMeeting"].Rows.Count > 0 && ds.Tables["LineMeeting"].Rows[0]["hour"].ToString().Length > 0)
        {
            // totalMeet = Convert.ToDouble(ds.Tables["Meeting"].Rows[0]["hour"] + "." + ds.Tables["Meeting"].Rows[0]["minute"]);
            string[] x = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            //string[] x = new string[ds.Tables["LineMeeting"].Rows.Count];
            double[] y = new double[7];
            checked
            {
                for (int i = 0; i < ds.Tables["LineMeeting"].Rows.Count; i++)
                {
                    int index1 = Array.IndexOf(x, ds.Tables["LineMeeting"].Rows[i][0].ToString());
                    if (index1 >= 0)
                    {
                        x[index1] = ds.Tables["LineMeeting"].Rows[i]["day"].ToString();
                        //y[index1] = Convert.ToDouble(ds.Tables["LineMeeting"].Rows[i]["Hour"] + "." + ds.Tables["LineMeeting"].Rows[i]["minute"]);
                        y[index1] = Math.Round(Convert.ToDouble(ds.Tables["LineMeeting"].Rows[i]["Hour"]) + (Convert.ToDouble(ds.Tables["LineMeeting"].Rows[i]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["LineMeeting"].Rows[i]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                    }
                    //TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]); 

                }
            }
            
            // totalTask = Convert.ToDouble(ds.Tables["Task"].Rows[0]["hour"] + "." + ds.Tables["Task"].Rows[0]["minute"]);
            Chart5.Series[1].Points.DataBindXY(x, y);
            this.Chart5.Series[1].BorderWidth = 1;
            this.Chart5.Series[1].Label = "#VALY";
            //Chart5.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            //Chart5.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

        }

        ds = TotalLineConfCall();

        if (ds.Tables["LineCall"].Rows.Count > 0 && ds.Tables["LineCall"].Rows[0]["hour"].ToString().Length > 0)
        {
            //totalCall = Convert.ToDouble(ds.Tables["Call"].Rows[0]["hour"] + "." + ds.Tables["Call"].Rows[0]["minute"]);

            //string[] x = new string[ds.Tables["LineCall"].Rows.Count];
            //double[] y = new double[ds.Tables["LineCall"].Rows.Count];
            string[] x = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            double[] y = new double[7];
            checked
            {
                for (int i = 0; i < ds.Tables["LineCall"].Rows.Count; i++)
                {
                    int index1 = Array.IndexOf(x, ds.Tables["LineCall"].Rows[i][0].ToString());
                    if (index1 >= 0)
                    {
                        x[index1] = ds.Tables["LineCall"].Rows[i]["day"].ToString();
                        //y[index1] = Convert.ToDouble(ds.Tables["LineCall"].Rows[i]["Hour"] + "." + ds.Tables["LineCall"].Rows[i]["minute"]);
                        y[index1] = Math.Round(Convert.ToDouble(ds.Tables["LineCall"].Rows[i]["Hour"]) + (Convert.ToDouble(ds.Tables["LineCall"].Rows[i]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["LineCall"].Rows[i]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                    }
                    //TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]); 

                }
            }
            
            // totalTask = Convert.ToDouble(ds.Tables["Task"].Rows[0]["hour"] + "." + ds.Tables["Task"].Rows[0]["minute"]);
            Chart5.Series[2].Points.DataBindXY(x, y);
            this.Chart5.Series[2].BorderWidth = 1;
            this.Chart5.Series[2].Label = "#VALY";
            //Chart5.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            //Chart5.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

        }

        ds = TotalLineBreak();

        if (ds.Tables["LineBreak"].Rows.Count > 0 && ds.Tables["LineBreak"].Rows[0]["hour"].ToString().Length > 0)
        {
            // totalBreak = Convert.ToDouble(ds.Tables["Break"].Rows[0]["hour"] + "." + ds.Tables["Break"].Rows[0]["minute"]);
            //string[] x = new string[ds.Tables["LineBreak"].Rows.Count];
            //double[] y = new double[ds.Tables["LineBreak"].Rows.Count];

            string[] x = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            double[] y = new double[7];
            checked
            {
                for (int i = 0; i < ds.Tables["LineBreak"].Rows.Count; i++)
                {
                    int index1 = Array.IndexOf(x, ds.Tables["LineBreak"].Rows[i][0].ToString());
                    if (index1 >= 0)
                    {
                        x[index1] = ds.Tables["LineBreak"].Rows[i]["day"].ToString();
                        // y[index1] = Convert.ToDouble(ds.Tables["LineBreak"].Rows[i]["Hour"] + "." + ds.Tables["LineBreak"].Rows[i]["minute"]);
                        y[index1] = Math.Round(Convert.ToDouble(ds.Tables["LineBreak"].Rows[i]["Hour"]) + (Convert.ToDouble(ds.Tables["LineBreak"].Rows[i]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["LineBreak"].Rows[i]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                    }
                    //TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]); 

                }
            }
           
            // totalTask = Convert.ToDouble(ds.Tables["Task"].Rows[0]["hour"] + "." + ds.Tables["Task"].Rows[0]["minute"]);
            Chart5.Series[3].Points.DataBindXY(x, y);
            this.Chart5.Series[3].BorderWidth = 1;
            this.Chart5.Series[3].Label = "#VALY";
            //Chart5.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            //Chart5.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

        }

        ds = TotalLineNonTask1();

        if (ds.Tables["LinenonTask1"].Rows.Count > 0 && ds.Tables["LinenonTask1"].Rows[0]["hour"].ToString().Length > 0)
        {
            //totalNonTask2 = Convert.ToDouble(ds.Tables["nonTask2"].Rows[0]["hour"] + "." + ds.Tables["nonTask2"].Rows[0]["minute"]);
            //string[] x = new string[ds.Tables["LinenonTask1"].Rows.Count];
            //double[] y = new double[ds.Tables["LinenonTask1"].Rows.Count];

            string[] x = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            double[] y = new double[7];
            checked
            {
                for (int i = 0; i < ds.Tables["LinenonTask1"].Rows.Count; i++)
                {

                    int index1 = Array.IndexOf(x, ds.Tables["LinenonTask1"].Rows[i][0].ToString());
                    if (index1 >= 0)
                    {
                        x[index1] = ds.Tables["LinenonTask1"].Rows[i]["day"].ToString();
                        //y[index1] = Convert.ToDouble(ds.Tables["LinenonTask1"].Rows[i]["Hour"] + "." + ds.Tables["LinenonTask1"].Rows[i]["minute"]);
                        y[index1] = Math.Round(Convert.ToDouble(ds.Tables["LinenonTask1"].Rows[i]["Hour"]) + (Convert.ToDouble(ds.Tables["LinenonTask1"].Rows[i]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["LinenonTask1"].Rows[i]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                    }
                    //TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]); 

                }
            }
           
            // totalTask = Convert.ToDouble(ds.Tables["Task"].Rows[0]["hour"] + "." + ds.Tables["Task"].Rows[0]["minute"]);
            Chart5.Series[4].Points.DataBindXY(x, y);
            this.Chart5.Series[4].BorderWidth = 1;
            this.Chart5.Series[4].Label = "#VALY";
            //Chart5.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            //Chart5.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
        }

        //ds = LinePeerSupport();

        //if (ds.Tables["Linepeer"].Rows.Count > 0 && ds.Tables["Linepeer"].Rows[0]["hour"].ToString().Length > 0)
        //{
        //    //totalNonTask2 = Convert.ToDouble(ds.Tables["nonTask2"].Rows[0]["hour"] + "." + ds.Tables["nonTask2"].Rows[0]["minute"]);
        //    //string[] x = new string[ds.Tables["Linepeer"].Rows.Count];
        //    //double[] y = new double[ds.Tables["Linepeer"].Rows.Count];

        //    string[] x = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        //    double[] y = new double[7];

        //    for (int i = 0; i < ds.Tables["Linepeer"].Rows.Count; i++)
        //    {

        //        int index1 = Array.IndexOf(x, ds.Tables["Linepeer"].Rows[i][0].ToString());
        //        if (index1 >= 0)
        //        {
        //            x[index1] = ds.Tables["Linepeer"].Rows[i]["day"].ToString();
        //            //y[index1] = Convert.ToDouble(ds.Tables["Linepeer"].Rows[i]["Hour"] + "." + ds.Tables["Linepeer"].Rows[i]["minute"]);
        //            y[index1] = Math.Round(Convert.ToDouble(ds.Tables["Linepeer"].Rows[i]["Hour"]) + Convert.ToDouble(ds.Tables["Linepeer"].Rows[i]["minute"]) / 60, 2, MidpointRounding.AwayFromZero);
        //        }
        //        //TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]); 

        //    }
        //    // totalTask = Convert.ToDouble(ds.Tables["Task"].Rows[0]["hour"] + "." + ds.Tables["Task"].Rows[0]["minute"]);
        //    Chart5.Series[5].Points.DataBindXY(x, y);
        //    this.Chart5.Series[5].BorderWidth = 1;
        //    this.Chart5.Series[5].Label = "#VALY";
        //    //Chart5.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
        //    //Chart5.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
        //}

    }


    protected void Chart1_Click(object sender, ImageMapEventArgs e)
    {
       Response.Redirect("~/ClentUsageTeam.aspx?type=TeamClientsUsage");
    }

    protected void Chart3_Click(object sender, ImageMapEventArgs e)
    {
        Response.Redirect("~/ClentUsageTeam.aspx?type=Tasks");
    }

    protected void Chart2_Click(object sender, ImageMapEventArgs e)
    {
        Response.Redirect("~/RUChart.aspx");
    }

    //protected void btnExport_Click(object sender, EventArgs e)
    //{
    //    //StringBuilder StrHtmlGenerate = new StringBuilder();
    //    //StringBuilder StrExport = new StringBuilder();

    //    //Response.AppendHeader("content-disposition", "attachment;filename=ExportedHtml.xls");

    //    //Response.Charset = "";

    //    //Response.Cache.SetCacheability(HttpCacheability.NoCache);

    //    //Response.ContentType = "application/vnd.ms-excel";

    //    //this.EnableViewState = false;

    //    ////StringWriter sw = new StringWriter();
    //    ////HtmlTextWriter h = new HtmlTextWriter(sw);
    //    ////ExportDiv.RenderControl(h);
    //    ////string str = sw.GetStringBuilder().ToString();

    //    //var sb = new StringBuilder();
    //    //ExportDiv.RenderControl(new HtmlTextWriter(new StringWriter(sb)));

    //    //string s = sb.ToString();

    //    //Response.Write(s);

    //    //Response.End();
    //    ////////////////////////////////////
    //    Response.Clear();

    //    Response.AddHeader("content-disposition", "attachment;filename=FileName.xls");

    //    Response.Charset = "";

    //    Response.Cache.SetCacheability(HttpCacheability.NoCache);

    //    Response.ContentType = "application/vnd.xls";

    //    System.IO.StringWriter stringWrite = new System.IO.StringWriter();

    //    System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

    //    dvCharts.RenderControl(htmlWrite);

    //    Response.Write(stringWrite.ToString());

    //    Response.End();

    //    //Response.ContentType = "application/pdf";
    //    //Response.AddHeader("content-disposition", "attachment;filename=TestPage.pdf");
    //    //Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //    //StringWriter sw = new StringWriter();
    //    //HtmlTextWriter hw = new HtmlTextWriter(sw);
    //    //this.Page.RenderControl(hw);
    //    //StringReader sr = new StringReader(sw.ToString());
    //    //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
    //    //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
    //    //PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
    //    //pdfDoc.Open();
    //    //htmlparser.Parse(sr);
    //    //pdfDoc.Close();
    //    //Response.Write(pdfDoc);
    //    //Response.End();
    //}

    //public override void VerifyRenderingInServerForm(Control control)
    //{

    //}
}