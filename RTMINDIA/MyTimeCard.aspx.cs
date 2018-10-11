using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using BAL;
using DAL;
using System.Text.RegularExpressions;

public partial class MyTimeCard : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    DataTable dtResult = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    SqlDBHelper objDB = new SqlDBHelper();
    clsTeam objTeam = new clsTeam();
    clsClock objClock = new clsClock();
    string sQuery;
    bool result;
    public string datePickerMinDate = "-3w";
    DateTime temp;
    clsRecords objRec = new clsRecords();

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
            txtSeachClient.Enabled = false;
            //LoadTask(Convert.ToString(Session["team"]));
            getTodaysTotal();
            getWeeksTotal();
            GetClockIn();

            LoadClients();

            PastChoice();

            txtDate.Text = DateTime.Now.ToShortDateString();

            DisplayRecords();
        }
    }

    private void getTodaysTotal()
    {
        dt = objClock.getTodaysHours(Session["username"].ToString(), DateTime.Now.ToShortDateString());
        if (dt.Rows.Count > 0)
        {
            if (!string.IsNullOrEmpty(dt.Rows[0]["hours"].ToString()))
            {
               // decimal totalhours = Convert.ToDecimal(dt.Rows[0]["hours"]) + (Convert.ToDecimal(dt.Rows[0]["minutes"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
                lblDay.Text = dt.Rows[0]["hours"] + ":" + dt.Rows[0]["minutes"] + ":" + dt.Rows[0]["seconds"];// decimal.Round(totalhours, 2, MidpointRounding.AwayFromZero).ToString();
            }

        }
    }

    private void getWeeksTotal()
    {
        int offset = DateTime.Now.DayOfWeek - DayOfWeek.Sunday;
        DateTime lastSunday = DateTime.Now.AddDays(-offset);
        dt = objClock.getWeeksHours(Session["username"].ToString(),lastSunday.ToShortDateString(), DateTime.Now.ToShortDateString());
        if (dt.Rows.Count > 0)
        {
            if (!string.IsNullOrEmpty(dt.Rows[0]["hours"].ToString()))
            {
               
                //decimal totalhours = Convert.ToDecimal(dt.Rows[0]["hours"]) + (Convert.ToDecimal(dt.Rows[0]["minutes"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
                lblWeek.Text = dt.Rows[0]["hours"] + ":" + dt.Rows[0]["minutes"] + ":" + dt.Rows[0]["seconds"];//decimal.Round(totalhours, 2, MidpointRounding.AwayFromZero).ToString();
            }

        }
    }

    [WebMethod(EnableSession = true)]
    public static string[] GetCustomers(string prefix)
    {
        string team = HttpContext.Current.Session["team"].ToString();
        List<string> customers = new List<string>();
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager.AppSettings["conString"].ToString();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "select CL_ID,CL_ClientName from RTM_Client_List where CL_ClientName LIKE '%'+@SearchText+'%' and CL_Status=1 and CL_TeamId='" + team + "'";
                cmd.Parameters.AddWithValue("@SearchText", prefix);
                cmd.Connection = conn;
                conn.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        customers.Add(string.Format("{0}/{1}", sdr["CL_ClientName"], sdr["CL_ID"]));
                    }
                }
                conn.Close();
            }
        }
        return customers.ToArray();
    }

    private void LoadClients()
    {
        DataSet ds = new DataSet();
        ds = objTeam.LoadClient(Session["team"].ToString(), "clients");
        if (ds.Tables["clients"].Rows.Count > 0)
        {
            gvClients.DataSource = ds.Tables["clients"];
            gvClients.DataBind();
        }
    }

    private void LoadTask(string teamId)
    {
        dt = new DataTable();
        if (txtSeachClient.Text == "Internal")
        {
            dt = objRec.GetGeneralTasks(Convert.ToInt32(teamId));
        }
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(teamId));
        }
        
        if (dt.Rows.Count > 0)
        {

            ddlTask.DataSource = dt;
            ddlTask.DataTextField = "TL_Task";
            ddlTask.DataValueField = "TL_ID";
            ddlTask.DataBind();
            //ddlTask.Items.Insert(0, "--Select--");
            //ddlTask.SelectedIndex = 0;
        }
    }

    private void LoadSubTasks()
    {
        dt = new DataTable();
        if (txtSeachClient.Text == "Internal")
        {
            dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlTask.SelectedValue));
        }
        else
        {
            dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlTask.SelectedValue));
        }       

        if (dt.Rows.Count > 0)
        {
            ddlSubTask.DataSource = dt;
            ddlSubTask.DataTextField = "STL_SubTask";
            ddlSubTask.DataValueField = "STL_ID";
            ddlSubTask.DataBind();
            //ddlSubTask.Items.Insert(0, "--Select--");
            //ddlSubTask.SelectedIndex = 0;
        }
    }

    protected void ddlTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTask.SelectedIndex > 0)
        {
            LoadSubTasks();
        }

    }
    protected void btnClockin_Click(object sender, EventArgs e)
    {
        try
        {
            ClockIn();

            PastChoice();

            DisplayRecords();
            UpdatePanel1.Update();
        }
        catch (Exception ex)
        {
            
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ClockinEx", "alert('"+ ex.Message +"')", true);
        }
    }
    protected void btnClockout_Click(object sender, EventArgs e)
    {
        ClockOut();
        LoadClients();
        PastChoice();
        UpdatePanel1.Update();
    }
    protected void btnElapsed_Click(object sender, EventArgs e)
    {
        DateTime endTime = DateTime.Now;
        string duration = endTime.Subtract(Convert.ToDateTime(lblStartTime.Text)).ToString(@"hh\:mm\:ss");
        lblElapsed.Text = duration;
    }

    private void ClockIn()
    {
        if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select valid client name')", true);
            return;
        }

        dt = objClock.PastTaskSelection(Session["username"].ToString());
        if (dt.Rows.Count > 0)
        {
            string task = dt.Rows[0]["R_Task"].ToString();
            string subTask = dt.Rows[0]["R_SubTask"].ToString();
            LoadTask(Convert.ToString(Session["team"]));
            if (!string.IsNullOrEmpty(task))
            {
                ddlTask.ClearSelection();
                if (ddlTask.Items.FindByValue(task) != null)
                {
                    ddlTask.Items.FindByValue(task).Selected = true;                   
                }

                LoadSubTasks();
                if (ddlSubTask.Items.FindByValue(subTask) != null)
                {
                    ddlSubTask.ClearSelection();
                    ddlSubTask.Items.FindByValue(subTask).Selected = true;
                }
            }
        }

        //string custId = hfCustomerId.Value;
        //string _taskId = ddlTask.SelectedValue;
        //string _subtaskId = ddlSubTask.SelectedValue;
        
        DateTime starttime = DateTime.Now;
        int rId = objClock.AddClockIn(0, Session["team"].ToString(), Session["empId"].ToString(), Session["username"].ToString(), Convert.ToInt32(hfCustomerId.Value), Convert.ToInt32(ddlTask.SelectedValue), Convert.ToInt32(ddlSubTask.SelectedValue), txtTaskComments.Value.Replace("'", ""), "00:00:00", starttime, "Running");
        if (rId > 0)
        {
            lblrecId.Text = rId.ToString();
            lblStartTime.Text = starttime.ToString();
           // runningDetails.Visible = true;
            lblStart.Visible = true;
            lblStartTime.Visible = true;
            lblTimeZone.Visible = true;
            txtSeachClient.Enabled = false;
            //ddlTask.Enabled = false;
            //ddlSubTask.Enabled = false;
            btnClockin.Visible = false;
            btnClockout.Visible = true;
            tmrElapsed.Enabled = true;
        }
    }

    private void GetClockIn()
    {
        try
        {
            dt = new DataTable();
            dt = objClock.getClickInTask(Session["username"].ToString());
            if (dt.Rows.Count > 0)
            {
                string client = dt.Rows[0]["CL_ClientName"].ToString();
                hfCustomerId.Value = dt.Rows[0]["R_Client"].ToString();
                string task = dt.Rows[0]["R_Task"].ToString();
                string subtask = dt.Rows[0]["R_SubTask"].ToString();
                lblrecId.Text = dt.Rows[0]["R_ID"].ToString();
                lblStartTime.Text = dt.Rows[0]["R_Start_Date_Time"].ToString();
                txtSeachClient.Text = client;

                LoadTask(Convert.ToString(Session["team"]));

                
                if (ddlTask.Items.FindByValue(task) != null)
                {
                    ddlTask.ClearSelection();
                    ddlTask.Items.FindByValue(task).Selected = true;
                   
                    //runningDetails.Visible = true;

                    lblStart.Visible = true;
                    lblStartTime.Visible = true;
                    lblTimeZone.Visible = true;
                    txtSeachClient.Enabled = false;
                    //ddlTask.Enabled = false;
                    //ddlSubTask.Enabled = false;
                    btnClockin.Visible = false;
                    btnClockout.Visible = true;
                    tmrElapsed.Enabled = true;
                }
                LoadSubTasks();

                if (ddlSubTask.Items.FindByValue(subtask) != null)
                {
                    ddlSubTask.ClearSelection();
                    ddlSubTask.Items.FindByValue(subtask).Selected = true;
                }

                
            }
            else
            {
                ddlTask.Items.Clear();
                ddlSubTask.Items.Clear();
                //dt = objClock.PastTaskSelection(Session["username"].ToString());
                //if (dt.Rows.Count > 0)
                //{
                //    string task = dt.Rows[0]["R_Task"].ToString();
                //    string subTask = dt.Rows[0]["R_SubTask"].ToString();
                //    LoadTask(Convert.ToString(Session["team"]));
                //    if (!string.IsNullOrEmpty(task))
                //    {
                //        ddlTask.ClearSelection();
                //        if (ddlTask.Items.FindByValue(task) != null)
                //        {
                //            ddlTask.Items.FindByValue(task).Selected = true;

                //            if (ddlTask.SelectedIndex > 0)
                //            {
                //                LoadSubTasks();
                //                if (ddlSubTask.Items.FindByValue(subTask) != null)
                //                {
                //                    ddlSubTask.ClearSelection();
                //                    ddlSubTask.Items.FindByValue(subTask).Selected = true;
                //                }
                //            }
                //        }
                //    }

                //}
            }
        }
        catch (Exception)
        {            
            
        }
        
    }

    private void ClockOut()
    {
        try
        { 
            DateTime endTime = DateTime.Now;
            string duration = endTime.Subtract(Convert.ToDateTime(lblStartTime.Text)).ToString(@"hh\:mm\:ss");

            bool result = objClock.AddClockOut(lblrecId.Text, endTime, duration, txtTaskComments.Value.Replace("'",""));

            if (result == true)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "clockOut", "alert('Please Select valid from Date')", true);
                lblrecId.Text = "0";
                lblStartTime.Text = "";
                //runningDetails.Visible = false;
                //txtSeachClient.Enabled = true;
                lblStart.Visible = false;
                lblStartTime.Visible = false;
                lblTimeZone.Visible = false;

                ddlTask.Enabled = true;
                ddlSubTask.Enabled = true;
                btnClockin.Visible = true;
                btnClockout.Visible = false;
                txtSeachClient.Text = string.Empty;
                hfCustomerId.Value = "0";
                //ddlSubTask.SelectedIndex = 0;
                //ddlTask.SelectedIndex = 0;
                tmrElapsed.Enabled = false;
                lblElapsed.Text = "0:0.00";
                
                txtTaskComments.Value = string.Empty;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "clockOutFailure", "alert('Failed to end task please try again')", true);
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Exception", "alert('Failed to end task please try again')", true);
        }
        getTodaysTotal();
        getWeeksTotal();
    }

    private void PastChoice()
    {
        dt = new DataTable();
        dt = objClock.PastChoices(Session["username"].ToString(), DateTime.Now.AddDays(-15).ToShortDateString(), Session["team"].ToString());
        if (dt.Rows.Count > 0)
        {
            gvPastTasks.DataSource = dt;
            gvPastTasks.DataBind();
        }
    }
    protected void lnkSelect_Click(object sender, EventArgs e)
    {
        try
        {
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            LinkButton lnkSelect = (LinkButton)grdrow.FindControl("lnkSelect");
            //if (lblrecId.Text != "0")
            //{
            //    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "TaskRunning", "alert('Please end the running task and try again')", true);
            //}
            //else
            //{

            if (lnkSelect.Text == "Clock-Out")
            {
                ClockOut();
                
                LoadClients();
                PastChoice();
                return;
            }

            if (lblrecId.Text != "0")
            {
                ClockOut();

                string clientName = grdrow.Cells[0].Text;
                string taskName = grdrow.Cells[2].Text;
                string subTaskName = grdrow.Cells[3].Text;
                Label _selectedClient = (Label)grdrow.FindControl("lblCLID");
                txtSeachClient.Text = clientName;
                hfCustomerId.Value = _selectedClient.Text;

                LoadTask(Convert.ToString(Session["team"]));
                if (ddlTask.Items.FindByValue(taskName) != null)
                {
                    ddlTask.ClearSelection();
                    ddlTask.Items.FindByText(taskName).Selected = true;
                }              
               
                LoadSubTasks();
                if (ddlSubTask.Items.FindByValue(subTaskName) != null)
                {
                    ddlSubTask.ClearSelection();
                    ddlSubTask.Items.FindByText(subTaskName).Selected = true;
                }
            }
            else
            {
                string clientName = grdrow.Cells[0].Text;
                string taskName = grdrow.Cells[2].Text;
                string subTaskName = grdrow.Cells[3].Text;
                Label _selectedClient = (Label)grdrow.FindControl("lblCLID");
                txtSeachClient.Text = clientName;
                hfCustomerId.Value = _selectedClient.Text;

                LoadTask(Convert.ToString(Session["team"]));

                if (ddlTask.Items.FindByValue(taskName) != null)
                {
                    ddlTask.ClearSelection();
                    ddlTask.Items.FindByText(taskName).Selected = true;
                } 

                LoadSubTasks();
                if (ddlSubTask.Items.FindByValue(subTaskName) != null)
                {
                    ddlSubTask.ClearSelection();
                    ddlSubTask.Items.FindByText(subTaskName).Selected = true;
                }
            }
            
           
                

                ClockIn();
                LoadClients();
                PastChoice();
           // }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "pastselectExcep", "alert('" + ex.Message + "')", true);
        }        
    }

    protected void lnkClientSelect_Click(object sender, EventArgs e)
    {
        try
        {
            GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
            LinkButton lnkClientSelect = (LinkButton)grdrow.FindControl("lnkClientSelect");
            Label _selectedClientId = (Label)grdrow.FindControl("lblCLID1");
            Label _selectedClientName = (Label)grdrow.FindControl("lblCLName");

            if (lnkClientSelect.Text == "Clock-Out")
            {
                ClockOut();
                
                LoadClients();
                PastChoice();
                return;
            }

            if (lblrecId.Text == "0")
            {
                txtSeachClient.Text = _selectedClientName.Text;
                hfCustomerId.Value = _selectedClientId.Text;

                ClockIn();

                LoadClients();
                PastChoice();
            }
            else
            {
                ClockOut();
                
                txtSeachClient.Text = _selectedClientName.Text;
                hfCustomerId.Value = _selectedClientId.Text;

                ClockIn();

                LoadClients();
                PastChoice();
            }
        }
        catch (Exception)
        {
            
            throw;
        }
    }

    private void DisplayRecords()
    {
        dt = new DataTable();
        
        SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@date",  txtDate.Text),
               new SqlParameter("@emp", Session["username"].ToString()),
           };


        sQuery = "select R_ID, R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Duration, R_Start_Date_Time, R_Comments from RTM_Records, RTM_Client_List, RTM_Task_List, RTM_SubTask_List where R_Client = CL_ID and R_Task = TL_ID and R_SubTask = STL_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date and R_User_Name=@emp and R_Status !='Running' order By R_ID, R_Start_Date_Time";
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

        gvRecords.DataSource = dt;
        gvRecords.DataBind();
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

            
            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id",Session["team"].ToString() ),
           };

            sQuery = "select CL_ID,CL_ClientName from RTM_Client_List where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

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
                dt = objRec.GetGeneralTasks(Convert.ToInt32(Session["team"]));

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
                dt = objRec.GetClientTasks(Convert.ToInt32(Session["team"]));

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

            
            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid),
               new SqlParameter("@system","P"),
               new SqlParameter("@approve", "0")
           };

            sQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Comments=@comments, R_System= @system, R_Approve=@approve where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            gvRecords.EditIndex = -1;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            DisplayRecords();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "update", "alert('"+ ex.Message  +"')", true);
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
            gvRecords.EditIndex = -1;
            DisplayRecords();
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

        
        SqlParameter[] parameters = new SqlParameter[]
           {  
               new SqlParameter("@rowid",rowid),
           };


        sQuery = "select * from RTM_Records where R_ID=@rowid";
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

        if (dt.Rows.Count > 0)
        {
            lblTeamId.Text = dt.Rows[0]["R_TeamId"].ToString();
            lblEmpID.Text = dt.Rows[0]["R_Employee_Id"].ToString();
            lblUserName.Text = dt.Rows[0]["R_User_Name"].ToString();
            lblStartTime1.Text = dt.Rows[0]["R_Start_Date_Time"].ToString();
            lblEndTime.Text = dt.Rows[0]["R_CreatedOn"].ToString();
            lblMainDuration.Text = "Duration should be less than " + dt.Rows[0]["R_Duration"].ToString();
            lblOldDuration.Text = dt.Rows[0]["R_Duration"].ToString();
            lblOldRId.Text = rowid.ToString();
            LoadClients(Convert.ToInt32(lblTeamId.Text));
            LoadTasks(Convert.ToInt32(lblTeamId.Text));
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

            if (ddlNewClient.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Please Select Client", "alert('Please Select Client')", true);
                //lblPopError.Text = "Please Select Client";
                //lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else if (ddlNewTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Please Select Task", "alert('Please Select Task')", true);
                //lblPopError.Text = "Please Select Task";
                //lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else if (ddlNewSubTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Please Select Sub Task", "alert('Please Select Sub Task')", true);
                //lblPopError.Text = "Please Select Sub Task";
                //lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else if (txtDuration.Text.Length < 8)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration format", "alert('Please enter time in HH:MM:SS format')", true);
                //lblPopError.Text = "Please enter time in HH:MM:SS format";
                //lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else if (regex.IsMatch(txtDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "invalid duration", "alert('Please enter valid time in HH:MM:SS format')", true);
                //lblPopError.Text = "Please enter valid time in HH:MM:SS format";
                //lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else if (TimeSpan.Parse(txtDuration.Text) >= TimeSpan.Parse(lblOldDuration.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "incorrect duration", "alert('Duration is incorrect')", true);
                //lblPopError.Text = "Duration is incorrect";
                //lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else
            {
                if (lblProcess.Text == "Split")
                {
                    TimeSpan dur = TimeSpan.Parse(lblOldDuration.Text).Subtract(TimeSpan.Parse(txtDuration.Text));

                    string comm = txtComments.Text + "-Split";

                    
                    SqlParameter[] parameters = new SqlParameter[]
                        {  
                            new SqlParameter("@teamId",lblTeamId.Text),
                            new SqlParameter("@empId",lblEmpID.Text),
                            new SqlParameter("@username",lblUserName.Text),
                            new SqlParameter("@newClient",ddlNewClient.SelectedValue),
                            new SqlParameter("@newTask",ddlNewTask.SelectedValue),
                            new SqlParameter("@subTask", ddlNewSubTask.SelectedValue),
                            new SqlParameter("@comm",comm),
                            new SqlParameter("@duration",txtDuration.Text),
                            new SqlParameter("@startTime",lblStartTime1.Text),
                            new SqlParameter("@endTime",lblEndTime.Text),
                            new SqlParameter("@status", "Completed"),
                            new SqlParameter("@system", "P"),
                            new SqlParameter("@approve", "0")
                       };
                    sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn,R_Status,R_System, R_Approve) values (@teamId,@empId, @username, @newClient, @newTask, @subTask, @comm,@duration, @startTime, @endTime, @status, @system,@approve)";
                    result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);


                    SqlParameter[] parameters1 = new SqlParameter[]
                           { 
                              new SqlParameter("@dur",dur.ToString(@"hh\:mm\:ss")),
                              new SqlParameter("@oldRid",lblOldRId.Text),
                           };
                    sQuery = "Update RTM_Records SET R_Duration=@dur where R_ID=@oldRid ";
                    result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters1);

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
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "exc", "alert('"+ ex.Message +"')", true);
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
        }
    }

    protected void ddlNewTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlNewTask.SelectedIndex != 0)
        {
            LoadSubTasks2();
            mpePopUp.Show();
        }
    }

    private void LoadClients(int TID)
    {
        
        SqlParameter[] parameters = new SqlParameter[]
        { 
            new SqlParameter("@TID",TID),
        };

        dt = new DataTable();

        sQuery = "select CL_ID, CL_ClientName from RTM_Client_List where CL_TeamId= @TID and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

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
        
        //SqlParameter[] parameters = new SqlParameter[]
        //{ 
        //    new SqlParameter("@TID",TID),
        //};

        //dt = new DataTable();

        //sQuery = "select * from RTM_Task_List where TL_TeamId =@TID and TL_Status =1 order By TL_Task";
        //dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

        //if (dt.Rows.Count > 0)
        //{
        //    ddlNewTask.DataSource = dt;
        //    ddlNewTask.DataTextField = "TL_Task";
        //    ddlNewTask.DataValueField = "TL_ID";
        //    ddlNewTask.DataBind();
        //    ddlNewTask.Items.Insert(0, "--Select--");
        //    ddlNewTask.SelectedIndex = 0;
        //}
    }

    private void LoadSubTasks2()
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
        
        //SqlParameter[] parameters = new SqlParameter[]
        //{ 
        //    new SqlParameter("@newtask", ddlNewTask.SelectedValue ),
        //};

        //dt = new DataTable();

        //sQuery = "select * from RTM_SubTask_List where STL_Task_Id=@newtask";
        //dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

        //if (dt.Rows.Count > 0)
        //{
        //    ddlNewSubTask.DataSource = dt;
        //    ddlNewSubTask.DataTextField = "STL_SubTask";
        //    ddlNewSubTask.DataValueField = "STL_ID";
        //    ddlNewSubTask.DataBind();
        //    ddlNewSubTask.Items.Insert(0, "--Select--");
        //    ddlNewSubTask.SelectedIndex = 0;
        //}
    }


   
    protected void Submit_Click(object sender, EventArgs e)
    {
        if (DateTime.TryParse(txtDate.Text, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "valid Date", "alert('Please Select valid from Date')", true);
            //lblError.Text = "Please Select valid from Date";
            //lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }
        DisplayRecords();
    }
    protected void ddlSubTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlSubTask.SelectedIndex > 0)
        {
            if (lblrecId.Text != "0")
            {
                result = objClock.UpdateTaskSubTask(lblrecId.Text, ddlTask.SelectedValue, ddlSubTask.SelectedValue);
            }
        }
    }
    protected void gvClients_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i <= gvClients.Rows.Count - 1; i++)
        {
            LinkButton lnkClientSelect = (LinkButton)gvClients.Rows[i].FindControl("lnkClientSelect");
            Label _clientId = (Label)gvClients.Rows[i].FindControl("lblCLID1");
            if (hfCustomerId.Value == _clientId.Text)
            {
                lnkClientSelect.Text = "Clock-Out";
            }
            else
            {
                if (hfCustomerId.Value != "0")
                {
                    lnkClientSelect.Text = "Switch";
                }
                else
                {
                    lnkClientSelect.Text = "Clock-In";
                }
                
            }
        }
    }
    protected void gvPastTasks_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        for (int i = 0; i <= gvPastTasks.Rows.Count - 1; i++)
        {
            LinkButton lnkClientSelect = (LinkButton)gvPastTasks.Rows[i].FindControl("lnkSelect");
            Label _clientId = (Label)gvPastTasks.Rows[i].FindControl("lblCLID");

            string taskName = gvPastTasks.Rows[i].Cells[2].Text;
            string subTaskName = gvPastTasks.Rows[i].Cells[3].Text;
            if (hfCustomerId.Value == _clientId.Text && taskName == ddlTask.SelectedItem.Text && subTaskName == ddlSubTask.SelectedItem.Text)
            {
                lnkClientSelect.Text = "Clock-Out";
            }
            else
            {
                if (hfCustomerId.Value != "0")
                {
                    lnkClientSelect.Text = "Switch";
                }
                else
                {
                    lnkClientSelect.Text = "Clock-In";
                }

            }
        }
    }
    protected void tmrElapsed_Tick(object sender, EventArgs e)
    {
        //Dim span As TimeSpan = DateTime.Now.Subtract(startTime)
        TimeSpan span = DateTime.Now.Subtract(Convert.ToDateTime(lblStartTime.Text));
        lblElapsed.Text = span.Hours.ToString() + ":" + span.Minutes.ToString() + "." + span.Seconds;
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

            dt = objRec.GetGeneralTasks(Convert.ToInt32(Session["team"]));

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
            dt = objRec.GetClientTasks(Convert.ToInt32(Session["team"]));

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

    protected void ddlNewClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadTasks(Convert.ToInt32(lblTeamId.Text));
        mpePopUp.Show();
    }
}