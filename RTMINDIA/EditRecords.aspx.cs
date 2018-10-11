using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
using System.Text.RegularExpressions;
using BAL;
using System.IO;
using System.Security.Cryptography;
using DAL;

public partial class EditRecords : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlCommand cmd;
    SqlDataAdapter da;
    DataSet ds = new DataSet();
    DataTable dt = new DataTable();
    clsTeam objTeam = new clsTeam();
    clsRealTimeReports objReal = new clsRealTimeReports();
    clsRecords objRec = new clsRecords();
    SqlDBHelper objDB = new SqlDBHelper();
    string SQlQuery;
    bool result;
    public string datePickerMinDate = "-3w";
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

        btnYes.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(btnYes, null) + ";");
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
    private void LoadEmp()
    {
        dt = new DataTable();

        dt = objTeam.LoadEmp(Session["access"].ToString(), lblid.Text, Session["user"].ToString());

        
        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();

    }

    private void DisplayRecords()
    {
        dt = new DataTable();
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@date",  txtDate.Text),
               new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
           };


        SQlQuery = "select R_ID, R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Duration, R_Start_Date_Time, R_Comments from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date and R_User_Name=@emp and R_Status !='Running' order By R_ID, R_Start_Date_Time";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        gvRecords.DataSource = dt;
        gvRecords.DataBind();

        int offset = Convert.ToDateTime(txtDate.Text).DayOfWeek - DayOfWeek.Sunday;
        DateTime lastSunday = Convert.ToDateTime(txtDate.Text).AddDays(-offset);
        DateTime nextSaturday = lastSunday.AddDays(6);
        dt = objRec.CheckSubmitStatus(ddlEmp.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
        if (dt.Rows.Count > 0)
        {
            string submit = dt.Rows[0]["R_Submit"].ToString();
            if (submit == "1")
            {
                pnlAdd.Enabled = false;
            }
            else
            {
                dt = objRec.CheckLeaveSubmitStatus(ddlEmp.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
                if (dt.Rows.Count > 0)
                {
                    string submit1 = dt.Rows[0]["LD_Submit"].ToString();
                    if (submit1 == "1")
                    {
                        pnlAdd.Enabled = false;
                    }
                    else
                    {
                        pnlAdd.Enabled = true;
                    }
                }
                else
                {
                    pnlAdd.Enabled = true;
                }
            }
        }
        else
        {
            dt = objRec.CheckLeaveSubmitStatus(ddlEmp.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
            if (dt.Rows.Count > 0)
            {
                string submit1 = dt.Rows[0]["LD_Submit"].ToString();
                if (submit1 == "1")
                {
                    pnlAdd.Enabled = false;
                }
                else
                {
                    pnlAdd.Enabled = true;
                }
            }
            else
            {
                pnlAdd.Enabled = true;
            }
        }
    }

    protected void btnDisplay_Click(object sender, EventArgs e)
    {
        DateTime temp;
        if (ddlTeam.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Employee", "alert('Please select team')", true);
            return;
        }

        if (DateTime.TryParse(txtDate.Text, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "from date", "alert('Please select valid date')", true);
            return;
        }
       
        DisplayRecords();
    }

    protected void gvRecords_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecords.EditIndex = -1;
        DisplayRecords();
    }

    protected void gvRecords_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecords.EditIndex == e.Row.RowIndex)
        {
            dt = new DataTable();
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlGvClient");
            Label lblClient = (Label)e.Row.FindControl("lblEditClient");

            //Namohar code changes on 10-Aug-2016.
            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id",lblid.Text ),
           };

            SQlQuery = "select CL_ID, CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName != 'Inclement Weather' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

            if (dt.Rows.Count > 0)
            {
                ddlGvClient.DataSource = dt;
                ddlGvClient.DataTextField = "CL_ClientName";
                ddlGvClient.DataValueField = "CL_ID";
                ddlGvClient.DataBind();
                if (ddlGvClient.Items.FindByText(lblClient.Text) != null)
                {
                    ddlGvClient.Items.FindByText(lblClient.Text).Selected = true;
                }
            }

            dt = new DataTable();

            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlGvTask");
            Label lblTask = (Label)e.Row.FindControl("lblEditTask");

            DropDownList ddlGvSubTask = (DropDownList)e.Row.FindControl("ddlGvSubTask");
            Label lblEditSubtask = (Label)e.Row.FindControl("lblEditSubtask");

            if (ddlGvClient.SelectedItem.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));

                if (dt.Rows.Count > 0)
                {
                    ddlGvTask.DataSource = dt;
                    ddlGvTask.DataTextField = "TL_Task";
                    ddlGvTask.DataValueField = "TL_ID";
                    ddlGvTask.DataBind();

                    if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                    {
                        ddlGvTask.ClearSelection();
                        ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                    }
                }

                if (ddlGvTask.Items.Count > 0)
                {
                    dt = new DataTable();

                    dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));

                    if (dt.Rows.Count > 0)
                    {
                        ddlGvSubTask.DataSource = dt;
                        ddlGvSubTask.DataTextField = "STL_SubTask";
                        ddlGvSubTask.DataValueField = "STL_ID";
                        ddlGvSubTask.DataBind();

                        if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                        {
                            ddlGvSubTask.ClearSelection();
                            ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                        }
                    }
                }
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));

                if (dt.Rows.Count > 0)
                {
                    ddlGvTask.DataSource = dt;
                    ddlGvTask.DataTextField = "TL_Task";
                    ddlGvTask.DataValueField = "TL_ID";
                    ddlGvTask.DataBind();

                    if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                    {
                        ddlGvTask.ClearSelection();
                        ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                    }
                }

                if (ddlGvTask.Items.Count > 0)
                {
                    dt = new DataTable();

                    dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));

                    if (dt.Rows.Count > 0)
                    {
                        ddlGvSubTask.DataSource = dt;
                        ddlGvSubTask.DataTextField = "STL_SubTask";
                        ddlGvSubTask.DataValueField = "STL_ID";
                        ddlGvSubTask.DataBind();

                        if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                        {
                            ddlGvSubTask.ClearSelection();
                            ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                        }
                    }
                }
            }           
            
        }
    }

    protected void gvRecords_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvRecords.EditIndex = e.NewEditIndex;
        DisplayRecords();
    }

    protected void gvRecords_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecords.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecords.Rows[e.RowIndex].FindControl("ddlGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecords.Rows[e.RowIndex].FindControl("ddlGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecords.Rows[e.RowIndex].FindControl("ddlGvSubTask");
            TextBox txtComments = (TextBox)gvRecords.Rows[e.RowIndex].FindControl("txtComments");

            //Namohar code changes on 10-Aug-2016.
            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid)               
           };

            SQlQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Comments=@comments where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);

            gvRecords.EditIndex = -1;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            DisplayRecords();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
            gvRecords.EditIndex = -1;
            DisplayRecords();
        }
    }

    protected void ddlGvClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlClient = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlClient.NamingContainer;
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlGvClient");
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlGvSubTask");

        if (ddlGvClient.SelectedItem.Text == "Internal")
        {
            dt = new DataTable();

            dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
            }

            if (ddlGvTask.Items.Count > 0)
            {
                dt = new DataTable();

                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));

                if (dt.Rows.Count > 0)
                {
                    ddlGvSubTask.DataSource = dt;
                    ddlGvSubTask.DataTextField = "STL_SubTask";
                    ddlGvSubTask.DataValueField = "STL_ID";
                    ddlGvSubTask.DataBind();
                }
            }
            
        }
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
            }
            if (ddlGvTask.Items.Count > 0)
            {
                dt = new DataTable();

                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));

                if (dt.Rows.Count > 0)
                {
                    ddlGvSubTask.DataSource = dt;
                    ddlGvSubTask.DataTextField = "STL_SubTask";
                    ddlGvSubTask.DataValueField = "STL_ID";
                    ddlGvSubTask.DataBind();
                }
            }
        }       
        
    }

    protected void ddlGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlGvSubTask");
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlGvClient");

        dt = new DataTable();
        if (ddlGvClient.SelectedItem.Text == "Internal")
        {
            dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
        }
        else
        {
            dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
        }

        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();
        }
    }

    protected void lnkInsert_Click(object sender, EventArgs e)
    {
        lblProcess.Text = "Split";
        LinkButton lnk = (LinkButton)sender;
        GridViewRow currentRow = (GridViewRow)lnk.NamingContainer;
        int rowid = Convert.ToInt32(gvRecords.DataKeys[currentRow.RowIndex].Value.ToString());
        dt = new DataTable();

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {  
               new SqlParameter("@rowid",rowid),
           };


        SQlQuery = "select * from RTM_Records WITH (NOLOCK) where R_ID=@rowid";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        if (dt.Rows.Count > 0)
        {
            lblTeamId.Text = dt.Rows[0]["R_TeamId"].ToString();
            lblEmpID.Text = dt.Rows[0]["R_Employee_Id"].ToString();
            lblUserName.Text = dt.Rows[0]["R_User_Name"].ToString();
            lblStartTime.Text = dt.Rows[0]["R_Start_Date_Time"].ToString();
            lblEndTime.Text = dt.Rows[0]["R_CreatedOn"].ToString();
            decimal d = Math.Round(Convert.ToDecimal(dt.Rows[0]["R_Duration"].ToString().Split(':')[0]) + (Convert.ToDecimal(dt.Rows[0]["R_Duration"].ToString().Split(':')[1]) / 60) + (Convert.ToDecimal(dt.Rows[0]["R_Duration"].ToString().Split(':')[2]) / 3600), 2, MidpointRounding.AwayFromZero);
            lblMainDuration.Text = "Duration should be less than " + dt.Rows[0]["R_Duration"].ToString() +" / "+ d;
            lblOldDuration.Text = dt.Rows[0]["R_Duration"].ToString();
            lblOldRId.Text = rowid.ToString();
            LoadClients(Convert.ToInt32(lblTeamId.Text));
            //LoadTasks(Convert.ToInt32(lblTeamId.Text));
            mpePopUp.Show();
        }

    }

    protected void btnNo_Click(object sender, EventArgs e)
    {
        txtDuration.Text = string.Empty;
        txtComments.Text = string.Empty;
        lblPopError.Text = string.Empty;
        lblOldDuration.Text = string.Empty;
        mpePopUp.Hide();
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        try
        {

            Regex regex = new Regex("^(?:(?:([01]?\\d|2[0-3]):)?([0-5]?\\d):)?([0-5]?\\d)$");
            Regex newRegex = new Regex("^([2][0-3]|[0-1]?[0-9])([.:][0-9]?[0-9])?([:][0-9]?[0-9])?$");
            Regex newRegex2 = new Regex("^([2][0-3]|[0-1]?[0-9])([.:][0-5]?[0-9])?([:][0-5]?[0-9])?$");
            string addDuration = "00:00:00";
            if (!string.IsNullOrEmpty(txtDuration.Text))
            {
                if (txtDuration.Text.Contains('.'))
                {
                    int dotCount = txtDuration.Text.Count(x => x == '.');
                    if (dotCount == 1)
                    {
                        decimal val = Math.Round(Convert.ToDecimal(txtDuration.Text), 2, MidpointRounding.AwayFromZero);
                        txtDuration.Text = val.ToString();
                    }
                }
            }
            if (ddlNewClient.SelectedIndex == 0)
            {
                lblPopError.Text = "Please Select Client";
                lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else if (ddlNewTask.SelectedIndex == 0)
            {
                lblPopError.Text = "Please Select Task";
                lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else if (ddlNewSubTask.SelectedIndex == 0)
            {
                lblPopError.Text = "Please Select Sub Task";
                lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }   
            else
            {
                if (txtDuration.Text.Contains(':'))
                {
                    if (newRegex2.IsMatch(txtDuration.Text) == false)
                    {
                        lblPopError.Text = "Please enter valid duration";
                        lblPopError.ForeColor = Color.Red;
                        mpePopUp.Show();
                        return;
                    }
                }
                else if (newRegex.IsMatch(txtDuration.Text) == false)
                {
                    lblPopError.Text = "Please enter valid duration";
                    lblPopError.ForeColor = Color.Red;
                    mpePopUp.Show();
                    return;
                }        
                if (txtDuration.Text.Contains(':') == false)
                {

                    decimal time = decimal.Parse(txtDuration.Text.Trim());
                    var hours = time.ToString().Split('.')[0];
                    var minutes = ((time * 60) % 60).ToString().Split('.')[0];
                    var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

                    addDuration = hours + ":" + minutes + ":" + seconds;

                }
                else
                {
                    int charCount = txtDuration.Text.Count(x => x == ':');
                    if (charCount == 1)
                    {
                        addDuration = txtDuration.Text + ":00";
                    }
                    else
                    {
                        addDuration = txtDuration.Text;
                    }
                }

                if (TimeSpan.Parse(addDuration) >= TimeSpan.Parse(lblOldDuration.Text))
                {
                    lblPopError.Text = "Duration is incorrect";
                    lblPopError.ForeColor = Color.Red;
                    mpePopUp.Show();
                    return;
                }

                if (lblProcess.Text == "Split")
                {
                    TimeSpan dur = TimeSpan.Parse(lblOldDuration.Text).Subtract(TimeSpan.Parse(addDuration));

                    string comm = txtComments.Text + "-Split";
                   
                    //Namohar code changes on 10-Aug-2016.
                    SqlParameter[] parameters = new SqlParameter[]
                        {  
                            new SqlParameter("@teamId",lblTeamId.Text),
                            new SqlParameter("@empId",lblEmpID.Text),
                            new SqlParameter("@username",lblUserName.Text),
                            new SqlParameter("@newClient",ddlNewClient.SelectedValue),
                            new SqlParameter("@newTask",ddlNewTask.SelectedValue),
                            new SqlParameter("@subTask", ddlNewSubTask.SelectedValue),
                            new SqlParameter("@comm",comm),
                            new SqlParameter("@duration",addDuration),
                            new SqlParameter("@startTime",lblStartTime.Text),
                            new SqlParameter("@endTime",lblEndTime.Text),
                            new SqlParameter("@status", "Completed"),
                            new SqlParameter("@system", "P")
                       };
                    SQlQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn,R_Status, R_System) values (@teamId,@empId, @username, @newClient, @newTask, @subTask, @comm,@duration, @startTime, @endTime, @status, @system)";
                    result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);

                   
                    SqlParameter[] parameters1 = new SqlParameter[]
                           { 
                              new SqlParameter("@dur",dur.ToString(@"hh\:mm\:ss")),
                              new SqlParameter("@oldRid",lblOldRId.Text),
                           };
                    SQlQuery = "Update RTM_Records SET R_Duration=@dur where R_ID=@oldRid ";
                    result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters1);
                    
                    txtDuration.Text = string.Empty;
                    txtComments.Text = string.Empty;
                    lblPopError.Text = string.Empty;
                    lblOldDuration.Text = string.Empty;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Saved Successfully')", true);
                    DisplayRecords();
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }
    }

    protected void ddlNewTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlNewTask.SelectedIndex != 0)
        {
            LoadSubTasks();
            mpePopUp.Show();
        }
    }

    private void LoadClients(int TID)
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        { 
            new SqlParameter("@TID",TID),
        };

        dt = new DataTable();

        SQlQuery = "select CL_ID, CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @TID and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName!='Public Holiday' and CL_ClientName !='Inclement Weather' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        if (dt.Rows.Count > 0)
        {
            ddlNewClient.DataSource = dt;
            ddlNewClient.DataTextField = "CL_ClientName";
            ddlNewClient.DataValueField = "CL_ID";
            ddlNewClient.DataBind();
            ddlNewClient.Items.Insert(0, "--Select--");
            ddlNewClient.SelectedIndex = 0;
        }
    }

    private void LoadTasks(int TID)
    {
        dt = new DataTable();
        if (ddlNewClient.SelectedIndex > 0)
        {
            if (ddlNewClient.SelectedItem.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(TID);
            }
            else
            {
                dt = objRec.GetClientTasks(TID);
            }

            if (dt.Rows.Count > 0)
            {
                ddlNewTask.DataSource = dt;
                ddlNewTask.DataTextField = "TL_Task";
                ddlNewTask.DataValueField = "TL_ID";
                ddlNewTask.DataBind();
                ddlNewTask.Items.Insert(0, "--Select--");
                ddlNewTask.SelectedIndex = 0;
            }

            ddlNewSubTask.Items.Clear();
        }
        else
        {
            ddlNewTask.Items.Clear();
            ddlNewSubTask.Items.Clear();
        }
    }

    private void LoadSubTasks()
    {
        dt = new DataTable();
        if (ddlNewClient.SelectedIndex > 0)
        {
            if (ddlNewTask.SelectedIndex > 0)
            {
                if (ddlNewClient.SelectedItem.Text == "Internal")
                {
                    dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlNewTask.SelectedValue));
                }
                else
                {
                    dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlNewTask.SelectedValue));
                }

                if (dt.Rows.Count > 0)
                {
                    ddlNewSubTask.DataSource = dt;
                    ddlNewSubTask.DataTextField = "STL_SubTask";
                    ddlNewSubTask.DataValueField = "STL_ID";
                    ddlNewSubTask.DataBind();
                    ddlNewSubTask.Items.Insert(0, "--Select--");
                    ddlNewSubTask.SelectedIndex = 0;
                }
            }
            else
            {
                ddlNewSubTask.Items.Clear();
            }
        }
        else
        {
            ddlNewSubTask.Items.Clear();
            ddlNewTask.Items.Clear();
        }       
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

    protected void ddlNewClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            
        mpePopUp.Show();
    }
}