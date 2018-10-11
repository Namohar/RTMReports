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

public partial class IncompleteRecords : System.Web.UI.Page
{
    clsRealTimeReports objReal = new clsRealTimeReports();
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();
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

            if (Session["access"].ToString() == "4")
            {
                rbUser.Visible = false;
                rbTeam.Visible = false;
            }
        }
    }

    protected void rbUser_CheckedChanged(object sender, EventArgs e)
    {
        LoadEmp();
    }
    protected void rbTeam_CheckedChanged(object sender, EventArgs e)
    {
        LoadTeams();
    }
    protected void ddlEmp_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlEmp.SelectedIndex > 0)
        {
            if (rbUser.Checked == true)
            {
                dt = objReal.GetPreference("User", "", ddlEmp.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    Session["preference"] = dt.Rows[0]["T_Preference"].ToString();
                }
            }
            else if (rbTeam.Checked == true)
            {
                dt = objReal.GetPreference("Team", ddlEmp.SelectedValue, "");
                if (dt.Rows.Count > 0)
                {
                    Session["preference"] = dt.Rows[0]["T_Preference"].ToString();
                }
            }
        }
        
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (DateTime.TryParse(datepicker.Value, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "valid from date", "alert('Please Select valid from Date')", true);
            //lblError.Text = "Please Select valid from Date";
            //lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }
        if (DateTime.TryParse(datepickerTo.Value, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "valid to date", "alert('Please Select valid to Date')", true);
            //lblError.Text = "Please Select Valid To Date";
            //lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }

        if (ddlEmp.SelectedIndex > 0)
        {
            BindIncorrectGrid();
        }
        else
        {
            if (rbUser.Checked)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select User", "alert('Please Select user')", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select team", "alert('Please Select team')", true);
            }
        }
    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        ddlEmp.SelectedIndex = 0;
        datepicker.Value = string.Empty;
        datepickerTo.Value = string.Empty;
    }

    private void LoadEmp()
    {
        dt = objReal.LoadEmp(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["user"].ToString(), Session["UID"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();
        lblEmp.Text = "Select Employee:";
        ddlEmp.Items.Insert(0, "--Select--");
        ddlEmp.SelectedIndex = 0;
    }

    private void LoadTeams()
    {
        dt = new DataTable();

        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "T_ID";
        ddlEmp.DataTextField = "T_TeamName";
        ddlEmp.DataBind();

        lblEmp.Text = "Select Team:";

        ddlEmp.Items.Insert(0, "--Select--");
        ddlEmp.SelectedIndex = 0;
    }

    private void BindIncorrectGrid()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(ddlEmp.SelectedValue)),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
            
        };
        dt = new DataTable();
        if (rbUser.Checked == true)
        {
            sQuery = "select R_ID, R_User_Name, R_Start_Date_Time, CL_ClientName, TL_Task, STL_SubTask,CL_TSheetClient, STL_ServiceCode, REPLACE(R_Duration,'-', '') as R_Duration, R_Comments from RTM_Records, RTM_Client_List, RTM_Task_List, RTM_SubTask_List where R_Client = CL_ID and R_Task=TL_ID  and R_SubTask = STL_ID And R_User_Name=@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from AND @to and CL_TSheetClient='Internal' and STL_ServiceCode NOT LIKE '% General/Internal'  and R_Duration != 'HH:MM:SS' order by R_ID";
        }
        else if (rbTeam.Checked == true)
        {
            sQuery = "select R_ID, R_User_Name, R_Start_Date_Time, CL_ClientName, TL_Task, STL_SubTask,CL_TSheetClient, STL_ServiceCode, REPLACE(R_Duration,'-', '') as R_Duration, R_Comments from RTM_Records, RTM_Client_List, RTM_Task_List, RTM_SubTask_List where R_Client = CL_ID and R_Task=TL_ID  and R_SubTask = STL_ID And R_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from AND @to and CL_TSheetClient='Internal' and STL_ServiceCode NOT LIKE '% General/Internal'  and R_Duration != 'HH:MM:SS' order by R_ID";
        }
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        //da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            gvIncorrectData.DataSource = dt;
            gvIncorrectData.DataBind();

            gvIncorrectData.Visible = true;

        }
        else
        {
            gvIncorrectData.DataSource = null;
            gvIncorrectData.DataBind();
            gvIncorrectData.Visible = false;
        }

    }

    protected void gvIncorrectData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (gvIncorrectData.EditIndex == e.Row.RowIndex)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Saplin.Controls.DropDownCheckBoxes ddlMultiClient = (Saplin.Controls.DropDownCheckBoxes)e.Row.FindControl("ddlMultiple");
                //Saplin.Controls.DropDownCheckBoxes ddlMultiClient =
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@emp",ddlEmp.SelectedItem.Text),
                    new SqlParameter("@team", ddlEmp.SelectedValue)
                };
                ds = new DataSet();
                if (rbUser.Checked == true)
                {
                    sQuery = "select * from RTM_Client_List, RTM_User_List where CL_TeamId= UL_Team_Id and UL_User_Name=@emp and CL_Status=1 and CL_ClientName <> 'Other' order by CL_ClientName";

                }
                else if (rbTeam.Checked == true)
                {
                    sQuery = "select * from RTM_Client_List where CL_TeamId= @team and CL_Status=1 and CL_ClientName <> 'Other' order by CL_ClientName";
                }

                ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "multiclient");
                if (ds.Tables["multiclient"].Rows.Count > 0)
                {
                    ddlMultiClient.DataSource = ds.Tables["multiclient"];
                    ddlMultiClient.DataTextField = "CL_ClientName";
                    ddlMultiClient.DataValueField = "CL_ID";
                    ddlMultiClient.DataBind();
                }
            }
        }
    }

    protected void gvIncorrectData_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvIncorrectData.EditIndex = -1;
        BindIncorrectGrid();
    }

    protected void gvIncorrectData_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvIncorrectData.EditIndex = e.NewEditIndex;
        BindIncorrectGrid();
    }

    protected void gvIncorrectData_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int checkedCount = 0;
        try
        {
            int id = Convert.ToInt32(gvIncorrectData.DataKeys[e.RowIndex].Values["R_ID"].ToString());
            Saplin.Controls.DropDownCheckBoxes ddlMultiSelect = (Saplin.Controls.DropDownCheckBoxes)gvIncorrectData.Rows[e.RowIndex].FindControl("ddlMultiple");

            foreach (System.Web.UI.WebControls.ListItem item in ddlMultiSelect.Items)
            {
                if (item.Selected)
                {
                    checkedCount = checkedCount + 1;
                }
            }

            if (checkedCount == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Client", "alert('Please select the clients')", true);
                //lblError.Text = "Please select the clients";
                //lblError.ForeColor = System.Drawing.Color.Red;
                ddlMultiSelect.Focus();
                return;
            }

            if (checkedCount == 1)
            {
                foreach (System.Web.UI.WebControls.ListItem item in ddlMultiSelect.Items)
                {
                    if (item.Selected)
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@client",item.Value),
                            new SqlParameter("@id", id)
                        };
                        sQuery = "Update RTM_Records SET R_Client= @client where R_ID=@id";
                        bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                    }
                }
            }
            else
            {
                ds = getRecord(id);

                if (ds.Tables["Irecord"].Rows.Count > 0)
                {
                    int TID = Convert.ToInt32(ds.Tables["Irecord"].Rows[0]["R_TeamId"]);
                    string EID = ds.Tables["Irecord"].Rows[0]["R_Employee_Id"].ToString();
                    string Uname = ds.Tables["Irecord"].Rows[0]["R_User_Name"].ToString();
                    int taskId = Convert.ToInt32(ds.Tables["Irecord"].Rows[0]["R_Task"]);
                    int subTaskId = Convert.ToInt32(ds.Tables["Irecord"].Rows[0]["R_SubTask"]);
                    string comments = ds.Tables["Irecord"].Rows[0]["R_Comments"].ToString();
                    string dur = ds.Tables["Irecord"].Rows[0]["R_Duration"].ToString();
                    string startTime = ds.Tables["Irecord"].Rows[0]["R_Start_Date_Time"].ToString();
                    string createdOn = ds.Tables["Irecord"].Rows[0]["R_CreatedOn"].ToString();

                    if (dur != "")
                    {
                        double timeSeconds = TimeSpan.Parse(dur).TotalSeconds;
                        timeSeconds = timeSeconds / checkedCount;
                        // string splitTime = ConvertToTime(timeSeconds);
                        var hms = TimeSpan.FromSeconds(timeSeconds);
                        var h = hms.Hours.ToString("00");
                        var m = hms.Minutes.ToString("00");
                        var s = hms.Seconds.ToString("00");
                        string splitTime = h + ":" + m + ":" + s;

                        foreach (System.Web.UI.WebControls.ListItem item in ddlMultiSelect.Items)
                        {
                            if (item.Selected)
                            {
                                SqlParameter[] parameters = new SqlParameter[]
                                {
                                    new SqlParameter("@teamId",TID),
                                    new SqlParameter("@empId",EID),
                                    new SqlParameter("@userName",Uname),
                                    new SqlParameter("@client", item.Value),
                                    new SqlParameter("@task",taskId),
                                    new SqlParameter("@subtask",subTaskId),
                                    new SqlParameter("@comments",comments),
                                    new SqlParameter("@duration",splitTime),
                                    new SqlParameter("@startTime",startTime),
                                    new SqlParameter("@createdOn",createdOn),
                                    new SqlParameter("@status", "Completed")
                                };
                                sQuery = "insert into RTM_Records(R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn, R_Status) values (@teamId, @empId, @userName, @client, @task, @subtask, @comments, @duration, @startTime, @createdOn,@status )";
                                bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                            }
                        }

                        SqlParameter[] parameters1 = new SqlParameter[]
                        {
                            new SqlParameter("@id",id)
                        };
                        sQuery = "Delete from RTM_Records where R_ID =@id";
                        bool result1 = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters1);

                    }


                }
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Incorrect Record", "alert('Record Corrected Successfully')", true);
            gvIncorrectData.EditIndex = -1;
            BindIncorrectGrid();
        }
        catch (Exception)
        {


        }

    }

    private DataSet getRecord(int rId)
    {
        SqlParameter[] parameters1 = new SqlParameter[]
         {
             new SqlParameter("@id",rId)
         };
        if (ds.Tables.Contains("Irecord"))
        {
            ds.Tables.Remove(ds.Tables["Irecord"]);
        }
        sQuery = "select * from RTM_Records where R_ID =@id";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters1, "Irecord");
        return ds;
    }
}