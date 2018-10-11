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
using System.Drawing;
using BAL;
using System.IO;
using System.Security.Cryptography;
using DAL;

public partial class EstimateCompareReport : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());

    SqlDataAdapter da;
    DataTable dt = new DataTable();
    DataTable dtResult = new DataTable();
    DataSet ds = new DataSet();
    clsTeam objTeam = new clsTeam();
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
            //LoadEmp();
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

    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex > 0)
        {
            lblid.Text = ddlTeam.SelectedValue;
            LoadEmp();
        }
        else
        {
            lblid.Text = "0";
        }
    }

    protected void btnDisplay_Click(object sender, EventArgs e)
    {
        try
        {
            DisplayData();
        }
        catch (Exception)
        {

        }
    }

    private void LoadEmp()
    {
        dt = new DataTable();

        dt = objTeam.LoadEmp(Session["access"].ToString(), lblid.Text, Session["user"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();
        ddlEmp.Items.Insert(0, "All");
        ddlEmp.SelectedIndex = 0;
    }

    private void BuidTable()
    {
        DataColumn dc;

        dc = new DataColumn("Employee Name");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Date");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Client");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Task");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Subtask");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("User Estimate");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("RTM Time");
        dtResult.Columns.Add(dc);
    }

    private DataSet GetRTMRecords()
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",txtDate.Text),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
            new SqlParameter("@id",lblid.Text),
        };

        if (ds.Tables.Contains("Records"))
        {
            ds.Tables.Remove(ds.Tables["Records"]);
        }

        if (ddlEmp.SelectedIndex == 0)
        {

            SQlQuery = "select R_User_Name, R_Client, CL_ClientName, R_Task, TL_Task, R_SubTask, STL_SubTask, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds " +
                        " from RTM_Records left join rtm_client_list on R_Client = CL_ID left join rtm_task_list on R_Task = TL_ID " +
                        " left join rtm_subtask_list on R_SubTask = STL_ID " +
                         " where R_TeamId =@id and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date " +
                         " Group by R_User_Name, R_Client, CL_ClientName, R_Task, TL_Task, R_SubTask, STL_SubTask";

        }
        else
        {
            SQlQuery = "select R_User_Name, R_Client, CL_ClientName, R_Task, TL_Task, R_SubTask, STL_SubTask, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds " +
                         " from RTM_Records left join rtm_client_list on R_Client = CL_ID left join rtm_task_list on R_Task = TL_ID " +
                    " left join rtm_subtask_list on R_SubTask = STL_ID " +
                    " where R_TeamId =@id and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) =@date and R_User_Name=@emp " +
                    " Group by R_User_Name, R_Client, CL_ClientName, R_Task, TL_Task, R_SubTask, STL_SubTask";
        }

        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "Records");
        return ds;
    }

    private DataSet GetEstimateRecords(string user, int client, int task, int subtask)
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",txtDate.Text),
            new SqlParameter("@client", client),
            new SqlParameter("@task",task),
            new SqlParameter("@subtask",subtask),
            new SqlParameter("@user",user),
        };

        if (ds.Tables.Contains("estRec"))
        {
            ds.Tables.Remove(ds.Tables["estRec"]);
        }

        SQlQuery = "select  sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/60)%60)))%60 as seconds " +
                    " from RTM_Estimation left join rtm_client_list on EST_ClientId = CL_ID left join rtm_task_list on EST_TaskId = TL_ID" +
                    " left join rtm_subtask_list on EST_SubTaskId = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EST_Date))) =@date and  EST_ClientId =@client and EST_TaskId =@task and EST_SubTaskId =@subtask and EST_UserName =@user ";

        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "estRec");
        return ds;
    }

    private DataSet GetEstNotInRec(string user)
    {

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",txtDate.Text),
            new SqlParameter("@id",  lblid.Text),
            new SqlParameter("@user",user),
        };
        if (ds.Tables.Contains("onlyest"))
        {
            ds.Tables.Remove(ds.Tables["onlyest"]);
        }

        if (ddlEmp.SelectedIndex == 0)
        {
            SQlQuery = "elect EST_UserName, EST_ClientId,CL_ClientName, EST_TaskId,TL_Task, EST_SubTaskId, STL_SubTask, EST_Duration from RTM_Estimation left join rtm_client_list on EST_ClientId = CL_ID left join rtm_task_list on EST_TaskId = TL_ID left join rtm_subtask_list on EST_SubTaskId = STL_ID where NOT EXISTS (select * from RTM_Records where EST_ClientId = R_Client and EST_TaskId= R_Task and EST_SubTaskId=R_SubTask  and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) =@date ) and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EST_Date))) =@date and EST_TeamId =@id";

        }
        else
        {
            SQlQuery = "select EST_UserName, EST_ClientId,CL_ClientName, EST_TaskId,TL_Task, EST_SubTaskId, STL_SubTask, EST_Duration from RTM_Estimation left join rtm_client_list on EST_ClientId = CL_ID left join rtm_task_list on EST_TaskId = TL_ID left join rtm_subtask_list on EST_SubTaskId = STL_ID where NOT EXISTS (select * from RTM_Records where EST_ClientId = R_Client and EST_TaskId= R_Task and EST_SubTaskId=R_SubTask and R_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) =@date ) and EST_UserName =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EST_Date))) =@date and EST_TeamId =@id";
        }
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "onlyest");
        return ds;
    }

    private DataSet GetLogs(string user)
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",txtDate.Text),
            new SqlParameter("@id",  lblid.Text),
            new SqlParameter("@emp",ddlEmp.SelectedItem.Text),
        };
        if (ds.Tables.Contains("logs"))
        {
            ds.Tables.Remove(ds.Tables["logs"]);
        }
        //dt = new DataTable();
        if (ddlEmp.SelectedIndex == 0)
        {
        
            SQlQuery = "select LA_User_Name, LA_Reason, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds " +
                       "from RTM_Log_Actions where LA_TeamId = @id and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) =@date and (LA_Reason = 'Meetings' or LA_Reason='Meeting'  or LA_Reason = 'Conference-Call')  group by LA_User_Name, LA_Reason";

        
        }
        else
        {
        
            SQlQuery = "select LA_User_Name, LA_Reason, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds " +
                       "from RTM_Log_Actions where LA_TeamId =@id and  CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) =@date and LA_User_Name=@emp and (LA_Reason = 'Meetings' or LA_Reason='Meeting'  or LA_Reason = 'Conference-Call' )  group by LA_User_Name, LA_Reason";
        }

        //da.Fill(ds, "logs");
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "logs");
        return ds;
    }

    private void DisplayData()
    {
        DataRow dr;

        BuidTable();

        ds = GetRTMRecords();
        if (ds.Tables["Records"].Rows.Count > 0)
        {
            foreach (DataRow dr1 in ds.Tables["Records"].Rows)
            {
                dr = dtResult.NewRow();

                dr["Employee Name"] = dr1["R_User_Name"];
                dr["Date"] = txtDate.Text;
                dr["Client"] = dr1["CL_ClientName"];
                dr["Task"] = dr1["TL_Task"];
                dr["Subtask"] = dr1["STL_SubTask"];

                ds = GetEstimateRecords(dr1["R_User_Name"].ToString(), Convert.ToInt32(dr1["R_Client"]), Convert.ToInt32(dr1["R_Task"]), Convert.ToInt32(dr1["R_SubTask"]));

                if (ds.Tables["estRec"].Rows.Count > 0 && ds.Tables["estRec"].Rows[0]["hour"].ToString().Length > 0)
                {
                    dr["User Estimate"] = ds.Tables["estRec"].Rows[0]["hour"].ToString() + ":" + ds.Tables["estRec"].Rows[0]["minute"].ToString() + ":" + ds.Tables["estRec"].Rows[0]["seconds"].ToString();
                }

                dr["RTM Time"] = dr1["hour"] + ":" + dr1["minute"] + ":" + dr1["seconds"];

                dtResult.Rows.Add(dr);
            }
        }

        ds = GetEstNotInRec(ddlEmp.SelectedItem.Text);

        if (ds.Tables["onlyest"].Rows.Count > 0)
        {
            foreach (DataRow dr2 in ds.Tables["onlyest"].Rows)
            {
                dr = dtResult.NewRow();

                dr["Employee Name"] = dr2["EST_UserName"];
                dr["Date"] = txtDate.Text;
                dr["Client"] = dr2["CL_ClientName"];
                dr["Task"] = dr2["TL_Task"];
                dr["Subtask"] = dr2["STL_SubTask"];
                dr["User Estimate"] = dr2["EST_Duration"];
                dr["RTM Time"] = "";

                dtResult.Rows.Add(dr);
            }
        }

        ds = GetLogs(ddlEmp.SelectedItem.Text);
        if (ds.Tables["logs"].Rows.Count > 0)
        {
            foreach (DataRow dr2 in ds.Tables["logs"].Rows)
            {
                dr = dtResult.NewRow();

                dr["Employee Name"] = dr2["LA_User_Name"];
                dr["Date"] = txtDate.Text;
                dr["Client"] = "";
                dr["Task"] = dr2["LA_Reason"];
                dr["Subtask"] = "";
                dr["User Estimate"] = "";
                dr["RTM Time"] = dr2["hour"] + ":" + dr2["minute"] + ":" + dr2["seconds"];

                dtResult.Rows.Add(dr);
            }
        }

        DataView dv = dtResult.DefaultView;
        dv.Sort = "[Employee Name] asc";
        dtResult = dv.ToTable();

        gvEstimate.DataSource = dtResult;
        gvEstimate.DataBind();
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        DisplayData();
        string attachment = "attachment; filename=Estimate comparison report - " + Convert.ToDateTime(txtDate.Text).ToString("MM-dd-yyyy") + ".xls";
        Response.ClearContent();
        Response.AddHeader("content-disposition", attachment);
        Response.ContentType = "application/vnd.ms-excel";
        string tab = "";
        foreach (DataColumn dc in dtResult.Columns)
        {
            Response.Write(tab + dc.ColumnName);
            tab = "\t";
        }
        Response.Write("\n");
        int i;
        foreach (DataRow dr in dtResult.Rows)
        {
            tab = "";
            for (i = 0; i < dtResult.Columns.Count; i++)
            {
                Response.Write(tab + dr[i].ToString());
                tab = "\t";
            }
            Response.Write("\n");
        }
        Response.End();
    }
}