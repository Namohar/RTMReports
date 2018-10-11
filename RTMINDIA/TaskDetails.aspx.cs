using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using BAL;
using System.IO;
using System.Security.Cryptography;
using DAL;

public partial class TaskDetails : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlDataAdapter da;
    DataSet ds = new DataSet();
    DataTable dt = new DataTable();
    clsTeam objTeam = new clsTeam();
    clsUtilization objUtil = new clsUtilization();
    clsRealTimeReports objReal = new clsRealTimeReports();

    SqlDBHelper objDB = new SqlDBHelper();
    string SQlQuery;
    bool result;

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
            //lblid.Text = objTeam.Decrypt(HttpUtility.UrlDecode(Request.QueryString["TID"]));
            BindTeam();
        }
    }

    private void BindTeam()
    {
        dt = new DataTable();

        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());
        ddlTeam.DataSource = dt;
        ddlTeam.DataValueField = "T_ID";
        ddlTeam.DataTextField = "T_TeamName";
        ddlTeam.DataBind();
        ddlTeam.Items.Insert(0, "--Select Team--");
        ddlTeam.SelectedIndex = 0;
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        PrintReport();

        if (dt.Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Task Details from " + txtFrom.Text + " to " + txtTo.Text + ".csv");
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

        //string strScript;
        //Session["rptSrc"] = dt;

        //strScript = "<script language=javascript>window.open('TaskDetailsReport.aspx')</script>";
        //ClientScript.RegisterClientScriptBlock(this.GetType(), "strScript", strScript);
    }

    private void BuildTable()
    {
        dt = new DataTable();
        DataColumn dc;

        dc = new DataColumn("Client");
        dt.Columns.Add(dc);

        dc = new DataColumn("Task");
        dt.Columns.Add(dc);

        dc = new DataColumn("Sub Task");
        dt.Columns.Add(dc);

        dc = new DataColumn("Time Spent");
        dt.Columns.Add(dc);

        dc = new DataColumn("FTE Effort", typeof(decimal));
        dt.Columns.Add(dc);
    }

    private DataSet FetchData()
    {

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from",  txtFrom.Text ),
               new SqlParameter("@to",  txtTo.Text),
               new SqlParameter("@id", lblid.Text ),
           };

       
        SQlQuery = "select CL_ClientName, TL_Task, STL_SubTask , ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour,ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute,ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                             "from RTM_Records, RTM_Client_List, rtm_task_list, rtm_subtask_list " +
                             "where R_Client = CL_ID and R_Task = TL_ID and R_SubTask = STL_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) between @from and @to and R_Duration != 'HH:MM:SS' " +
                             "and R_TeamId =@id GROUP BY CL_ClientName, TL_Task, STL_SubTask order by CL_ClientName, TL_Task, STL_SubTask";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "data");
        return ds;
    }

    private void PrintReport()
    {
        try
        {
            DataRow dr;
            BuildTable();
            int _workingDays = objUtil.GetNumberOfWorkingDays(Convert.ToDateTime(txtFrom.Text), Convert.ToDateTime(txtTo.Text));
            ds = FetchData();
            if (ds.Tables["data"].Rows.Count > 0)
            {
                foreach (DataRow dr1 in ds.Tables[0].Rows)
                {
                    dr = dt.NewRow();

                    dr["Client"] = dr1["CL_ClientName"];
                    dr["Task"] = dr1["TL_Task"];
                    dr["Sub Task"] = dr1["STL_SubTask"];
                    decimal timeSpent = Math.Round(Convert.ToDecimal(dr1["hour"]) + (Convert.ToDecimal(dr1["minute"]) / 60) + (Convert.ToDecimal(dr1["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                    dr["Time Spent"] = dr1["hour"] + ":" + dr1["minute"] + ":" + dr1["seconds"];//Math.Round(Convert.ToDecimal(dr1["hour"]) + (Convert.ToDecimal(dr1["minute"]) / 60) + (Convert.ToDecimal(dr1["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);

                    dr["FTE Effort"] = Math.Round(timeSpent / (_workingDays * 8), 2, MidpointRounding.AwayFromZero);

                    dt.Rows.Add(dr);
                }
            }
        }
        catch (Exception ex)
        {

            lblError.Text = ex.Message;
            lblError.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex > 0)
        {
            lblid.Text = ddlTeam.SelectedValue;
            //LoadEmp();
        }
        else
        {
            lblid.Text = "0";
        }
    }

    //private static int GetNumberOfWorkingDays(DateTime start, DateTime stop)
    //{
    //    TimeSpan interval = stop - start;

    //    int totalWeek = interval.Days / 7;
    //    int totalWorkingDays = 5 * totalWeek;

    //    int remainingDays = interval.Days % 7;


    //    for (int i = 0; i <= remainingDays; i++)
    //    {
    //        DayOfWeek test = (DayOfWeek)(((int)start.DayOfWeek + i) % 7);
    //        if (test >= DayOfWeek.Monday && test <= DayOfWeek.Friday)
    //            totalWorkingDays++;
    //    }

    //    return totalWorkingDays;
    //}
}