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
using DAL;

public partial class Implimentation : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());

    SqlDataAdapter da;
    DataTable dt = new DataTable();
    DataTable dt2 = new DataTable();
    DataSet ds = new DataSet();
    DataTable dtResult = new DataTable();
    clsTeam objTeam = new clsTeam();
    string sQuery;
    SqlDBHelper objDB = new SqlDBHelper();
    DateTime temp;
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
            LoadEmp();
        }
    }

    private void LoadEmp()
    {
        
        dt = new DataTable();
        dt = objTeam.LoadEmpForEstimate(Session["access"].ToString(), "13", Session["user"].ToString());
       
        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();

        ddlEmp.Items.Insert(0, "--Select--");
        ddlEmp.SelectedIndex = 0;
    }

    private void getRecords()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",datepicker.Text),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        dt = new DataTable();
        sQuery = "select R_ID, CL_ClientName, TL_Task, STL_SubTask, R_Comments from rtm_records left join rtm_client_list on R_Client = CL_ID left join rtm_task_list on R_Task = TL_ID " +
                      "left join rtm_subtask_list on R_SubTask = STL_ID where STL_SubTask = 'NEW Client' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) =@date and R_User_Name=@emp";
        
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        if (dt.Rows.Count > 0)
        {
            gvRecords.DataSource = dt;
            gvRecords.DataBind();
        }
        else
        {
            gvRecords.DataSource = null;
            gvRecords.DataBind();
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (DateTime.TryParse(datepicker.Text, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "From Date Error", "alert('Please select valid From date')", true);
            return;
        }
        if (ddlEmp.SelectedIndex == 0)
        {

        }
        else if (datepicker.Text.Length <= 0)
        {

        }
        else
        {
            getRecords();
        }
    }

    protected void gvRecords_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvRecords.EditIndex = e.NewEditIndex;
        getRecords();
    }

    protected void gvRecords_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecords.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecords.Rows[e.RowIndex].FindControl("ddlGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecords.Rows[e.RowIndex].FindControl("ddlGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecords.Rows[e.RowIndex].FindControl("ddlGvSubTask");
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@client",ddlGvClient.SelectedValue),
                new SqlParameter("@task",ddlGvTask.SelectedValue),
                new SqlParameter("@subtask",ddlGvSubTask.SelectedValue),
                new SqlParameter("@id",rowid)
            };
            
            sQuery = "update rtm_records set R_Client=@client, R_Task=@task , R_SubTask=@subtask where R_ID = @id";
            bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);

            gvRecords.EditIndex = -1;
            getRecords();

        }
        catch (Exception)
        {


        }
    }

    protected void gvRecords_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecords.EditIndex = -1;
        getRecords();
    }

    protected void ddlGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlGvSubTask");
        int taskid = Convert.ToInt32(ddlGvTask.SelectedValue);
        string taskname = ddlGvTask.SelectedItem.Text;
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@taskid", taskid)
        };
        dt = new DataTable();
        sQuery = "select * from RTM_SubTask_List where STL_Task_Id= @taskid";
       
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();
            ddlGvSubTask.Items.Insert(0, "-Select SubTask-");
            ddlGvSubTask.SelectedIndex = 0;
        }

    }

    protected void gvRecords_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecords.EditIndex == e.Row.RowIndex)
        {
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlGvClient");
            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlGvTask");

            dt = new DataTable();
            da = new SqlDataAdapter("select * from RTM_Client_List where CL_TeamId= 13", con);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddlGvClient.DataSource = dt;
                ddlGvClient.DataTextField = "CL_ClientName";
                ddlGvClient.DataValueField = "CL_ID";
                ddlGvClient.DataBind();
                ddlGvClient.Items.Insert(0, "-Select Client-");
                ddlGvClient.SelectedIndex = 0;
            }

            dt = new DataTable();

            da = new SqlDataAdapter("select * from RTM_Task_List where TL_TeamId = 13", con);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
                ddlGvTask.Items.Insert(0, "-Select Task-");
                ddlGvTask.SelectedIndex = 0;
            }

        }
    }
}