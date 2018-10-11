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

public partial class EstimateReport : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());

    SqlDataAdapter da;
    DataTable dt = new DataTable();
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
            lblTotal.Text = "00:00:00";
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
            lblError.Text = "";
            if (txtDate.Text.Length <= 0)
            {
                lblError.Text = "Please select Date";
                return;
            }
            DisplayRecords();
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
        if (Session["access"].ToString() != "4")
        {
            ddlEmp.Items.Insert(0, "All");
            ddlEmp.SelectedIndex = 0;
        }

    }

    private void DisplayRecords()
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",txtDate.Text),
            new SqlParameter("@emp",ddlEmp.SelectedItem.Text),
            new SqlParameter("@id",lblid.Text),
        };
        dt = new DataTable();

        if (ddlEmp.SelectedItem.Text == "All")
        {
      
            SQlQuery = "select EST_UserName, CL_ClientName, TL_Task, STL_SubTask, EST_Duration, CONVERT(VARCHAR(10),EST_Date,101) as EST_Date, EST_Comments from RTM_Estimation left join rtm_client_list on EST_ClientId = CL_ID left join rtm_task_list on EST_TaskId = TL_ID " +
                       "left join rtm_subtask_list on EST_SubTaskId = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EST_Date))) =@date and EST_TeamId=@id Order By EST_UserName";
        }
        else
        {
      
            SQlQuery = "select EST_UserName, CL_ClientName, TL_Task, STL_SubTask, EST_Duration, CONVERT(VARCHAR(10),EST_Date,101) as EST_Date, EST_Comments from RTM_Estimation left join rtm_client_list on EST_ClientId = CL_ID left join rtm_task_list on EST_TaskId = TL_ID " +
                       "left join rtm_subtask_list on EST_SubTaskId = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EST_Date))) =@date and EST_UserName =@emp and EST_TeamId=@id Order By EST_UserName";
        }

        // da.Fill(dt);
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        if (dt.Rows.Count > 0)
        {
            gvEstimate.DataSource = dt;
            gvEstimate.DataBind();
            GetTotal();
        }
        else
        {
            gvEstimate.DataSource = null;
            gvEstimate.DataBind();
            lblTotal.Text = "00:00:00";
        }
    }

    private void GetTotal()
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",txtDate.Text),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
            new SqlParameter("@id",lblid.Text),
        };
        dt = new DataTable();
        if (ddlEmp.SelectedItem.Text == "All")
        {
            SQlQuery = "select Convert(varchar(4), sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/3600) +':'+ Convert(varchar(4),(sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/60)%60) +':'+ Convert(varchar(4),(sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/60)%60)))%60) as Duration from RTM_Estimation where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EST_Date))) =@date and EST_TeamId=@id";
        }
        else
        {
            SQlQuery = "select Convert(varchar(4), sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/3600) +':'+ Convert(varchar(4),(sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/60)%60) +':'+ Convert(varchar(4),(sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(EST_Duration,'-', '')))/60)%60)))%60) as Duration from RTM_Estimation where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EST_Date))) =@date and EST_UserName =@emp and EST_TeamId=@id";
        }
        //da.Fill(dt);
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
        if (dt.Rows.Count > 0)
        {
            lblTotal.Text = dt.Rows[0]["Duration"].ToString();
        }
    }
}