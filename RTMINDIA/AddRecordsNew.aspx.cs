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

public partial class AddRecordsNew : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    DataTable dtResult = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    clsRecords objRec = new clsRecords();
    SqlDBHelper objDB = new SqlDBHelper();
    clsTeam objTeam = new clsTeam();
    Regex regex = new Regex("^(?:(?:([01]?\\d|2[0-3]):)?([0-5]?\\d):)?([0-5]?\\d)$");
    Regex newRegex = new Regex("^([2][0-3]|[0-1]?[0-9])([.:][0-9]?[0-9])?([:][0-9]?[0-9])?$");
    Regex newRegex2 = new Regex("^([2][0-3]|[0-1]?[0-9])([.:][0-5]?[0-9])?([:][0-5]?[0-9])?$");
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
           // Session["selTeam"] = ddlTeam.SelectedValue;
            //BindGridview();
            LoadTeams();

            if (ddlTeam.Items.FindByValue(Session["team"].ToString()) != null)
            {
                ddlTeam.ClearSelection();
                ddlTeam.Items.FindByValue(Session["team"].ToString()).Selected = true;
                Session["selTeam"] = ddlTeam.SelectedValue;
                if (ddlTeam.SelectedIndex > 0)
                {
                    //LoadTask(Session["selTeam"].ToString());
                    LoadEmp();

                    if (ddlEmployee.Items.FindByText(Session["username"].ToString()) != null)
                    {
                        ddlEmployee.ClearSelection();
                        ddlEmployee.Items.FindByText(Session["username"].ToString()).Selected = true;

                        if (ddlEmployee.SelectedIndex > 0)
                        {
                            txtDate.Text = DateTime.Now.ToShortDateString();

                            dvAdd.Visible = true;
                            select.Visible = true;
                            if (rbMultClient.Checked)
                            {
                                dvClientsGrid.Visible = true;
                            }
                            else
                            {
                                dvTasksGrid.Visible = true;
                            }


                            DisplayRecords();
                        }
                    }
                }
            }

            if (ddlEmployee.SelectedIndex > 0)
            {
                int offset = Convert.ToDateTime(txtDate.Text).DayOfWeek - DayOfWeek.Sunday;
                DateTime lastSunday = Convert.ToDateTime(txtDate.Text).AddDays(-offset);
                DateTime nextSaturday = lastSunday.AddDays(6);
                dt = objRec.CheckSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
                if (dt.Rows.Count > 0)
                {
                    string submit = dt.Rows[0]["R_Submit"].ToString();
                    if (submit == "1")
                    {
                        pnlAdd.Enabled = false;
                    }
                    else
                    {
                        dt = objRec.CheckLeaveSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
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
                    dt = objRec.CheckLeaveSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
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
            
        }
    }

    

    [WebMethod(EnableSession = true)]
    public static string[] GetCustomers(string prefix)
    {
        string team = HttpContext.Current.Session["selTeam"].ToString();
        List<string> customers = new List<string>();
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager.AppSettings["conString"].ToString();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "select CL_ID,CL_ClientName from RTM_Client_List where CL_ClientName LIKE '%'+@SearchText+'%' and CL_Status=1 and CL_TeamId='" + team + "' and CL_ClientName !='Personal/Sick Time' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName !='Inclement Weather' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off'";
                cmd.Parameters.AddWithValue("@SearchText", prefix);
                cmd.Connection = conn;
                conn.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        customers.Add(string.Format("{0}!{1}", sdr["CL_ClientName"], sdr["CL_ID"]));
                    }
                }
                conn.Close();
            }
        }
        return customers.ToArray();
    }

    private void LoadTeams()
    {
        dt = new DataTable();
        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());
        if (dt.Rows.Count > 0)
        {
            ddlTeam.DataSource = dt;
            ddlTeam.DataTextField = "T_TeamName";
            ddlTeam.DataValueField = "T_ID";
            ddlTeam.DataBind();

            ddlTeam.Items.Insert(0, "--Select Team--");

            ddlTeam.SelectedIndex = 0;
        }
    }

    private void LoadEmp()
    {
        dt = new DataTable();

        dt = objTeam.LoadEmp(Session["access"].ToString(), ddlTeam.SelectedValue, Session["user"].ToString());

        ddlEmployee.DataSource = dt;
        ddlEmployee.DataValueField = "UL_Employee_Id";
        ddlEmployee.DataTextField = "UL_User_Name";
        ddlEmployee.DataBind();
        ddlEmployee.Items.Insert(0, "--Select--");
        ddlEmployee.SelectedIndex = 0;

    }

    protected void gvClients_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["clients"] != null)
        {
            DataTable dt = (DataTable)ViewState["clients"];
            DataRow drCurrentRow = null;
            int rowIndex = Convert.ToInt32(e.RowIndex);
            if (dt.Rows.Count > 0)
            {
                //for (int i = 1; i <= dt.Rows.Count; i++)
                //{

                //    TextBox txtDuration = (TextBox)gvClients.Rows[rowIndex].Cells[2].FindControl("txtDuration");
                //    TextBox txtComments = (TextBox)gvClients.Rows[rowIndex].Cells[2].FindControl("txtComments");


                //    drCurrentRow = dt.NewRow();

                //    drCurrentRow["clientId"] = hfCustomerId.Value;
                //    drCurrentRow["ClientName"] = txtSeachClient.Text;
                //    dt.Rows[i - 1]["duration"] = txtDuration.Text;
                //    dt.Rows[i - 1]["comments"] = txtComments.Text;


                //    rowIndex++;
                //}
                dt.Rows.Remove(dt.Rows[rowIndex]);
                drCurrentRow = dt.NewRow();
                ViewState["clients"] = dt;
                gvClients.DataSource = dt;
                gvClients.DataBind();
                //for (int i = 0; i < gvClients.Rows.Count - 1; i++)
                //{
                //    gvClients.Rows[i].Cells[0].Text = Convert.ToString(i + 1);
                //}
                SetOldData();
                if (dt.Rows.Count == 0)
                {
                    ddlTask.Items.Clear();
                    ddlSubTask.Items.Clear();
                }
                txtSeachClient.Enabled = true;
                btnSearch.Enabled = true;
                ddlTask.Enabled = true;
                lblPleaseNote.Visible = false;
            }
        }
    }

    protected void BindGridview()
    {
        DataTable dt = new DataTable();
        
        dt.Columns.Add("clientId", typeof(string));
        dt.Columns.Add("clientName", typeof(string));
        dt.Columns.Add("duration", typeof(string));
        dt.Columns.Add("comments", typeof(string));
        DataRow dr = dt.NewRow();

        dr["clientId"] = hfCustomerId.Value;
        dr["clientName"] = txtSeachClient.Text;
        dr["duration"] = string.Empty;
        dr["comments"] = string.Empty;
        dt.Rows.Add(dr);
        ViewState["clients"] = dt;
        gvClients.DataSource = dt;
        gvClients.DataBind();
    }

    
   
    private void AddNewRow()
    {
        int rowIndex = 0;

        if (ViewState["clients"] != null)
        {
            DataTable dt = (DataTable)ViewState["clients"];
            DataRow drCurrentRow = null;
            if (dt.Rows.Count > 0)
            {
                for (int i = 1; i <= dt.Rows.Count; i++)
                {

                    TextBox txtDuration = (TextBox)gvClients.Rows[rowIndex].Cells[2].FindControl("txtDuration");
                    TextBox txtComments = (TextBox)gvClients.Rows[rowIndex].Cells[2].FindControl("txtComments");


                    drCurrentRow = dt.NewRow();

                    drCurrentRow["clientId"] = hfCustomerId.Value;
                    drCurrentRow["ClientName"] = txtSeachClient.Text;
                    dt.Rows[i - 1]["duration"] = txtDuration.Text;
                    dt.Rows[i - 1]["comments"] = txtComments.Text;


                    rowIndex++;
                }
                dt.Rows.Add(drCurrentRow);
                ViewState["clients"] = dt;
                gvClients.DataSource = dt;
                gvClients.DataBind();
            }
            else
            {
                BindGridview();
            }
           
        }
        else
        {
            BindGridview();
            //Response.Write("ViewState Value is Null");
        }
        SetOldData();
    }

    private void SetOldData()
    {
        int rowIndex = 0;
        if (ViewState["clients"] != null)
        {
            DataTable dt = (DataTable)ViewState["clients"];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Label lblClientId = (Label)gvClients.Rows[rowIndex].Cells[0].FindControl("lblClientID");
                    Label lblClientName = (Label)gvClients.Rows[rowIndex].Cells[0].FindControl("lblClientName");
                    TextBox txtDuration = (TextBox)gvClients.Rows[rowIndex].Cells[1].FindControl("txtDuration");
                    TextBox txtComments = (TextBox)gvClients.Rows[rowIndex].Cells[2].FindControl("txtComments");
                    lblClientId.Text = dt.Rows[i]["clientId"].ToString();
                    lblClientName.Text = dt.Rows[i]["clientName"].ToString();
                    txtDuration.Text = dt.Rows[i]["duration"].ToString();
                    txtComments.Text = dt.Rows[i]["comments"].ToString();
                    
                    rowIndex++;
                }
            }
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        lblPleaseNote.Visible = false;
        lblSorry.Visible = false;
        if (string.IsNullOrEmpty(txtSeachClient.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Client", "alert('Please enter client name.')", true);
            return;
        }
        else if (string.IsNullOrWhiteSpace(txtSeachClient.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Client1", "alert('Please enter client name.')", true);
            return;
        }
        else if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select valid Client", "alert('Please enter valid client name.')", true);
            return;
        }
        if (rbMultClient.Checked)
        {
            if (gvClients.Rows.Count > 0)
            {
                if (txtSeachClient.Text == "Internal")
                {
                    lblSorry.Visible = true;
                    txtSeachClient.Text = string.Empty;
                    hfCustomerId.Value = "0";
                    return;
                }
            }
            if (txtSeachClient.Text == "Internal")
            {
                txtSeachClient.Enabled = false;
                btnSearch.Enabled = false;

                dt = new DataTable();
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
                if (dt.Rows.Count > 0)
                {

                    ddlTask.DataSource = dt;
                    ddlTask.DataTextField = "TL_Task";
                    ddlTask.DataValueField = "TL_ID";
                    ddlTask.DataBind();
                    ddlTask.Items.Insert(0, "--Select--");
                    ddlTask.SelectedIndex = 0;
                }

                lblPleaseNote.Visible = true;
            }
            else
            {
                if (gvClients.Rows.Count <= 0)
                {
                    dt = new DataTable();
                    dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
                    if (dt.Rows.Count > 0)
                    {

                        ddlTask.DataSource = dt;
                        ddlTask.DataTextField = "TL_Task";
                        ddlTask.DataValueField = "TL_ID";
                        ddlTask.DataBind();
                        ddlTask.Items.Insert(0, "--Select--");
                        ddlTask.SelectedIndex = 0;
                    }
                    lblPleaseNote.Visible = false;
                }

            }
            AddNewRow();
        }
        else
        {
            AddNewTaskRow();
        }
        
        txtSeachClient.Text = string.Empty;
        hfCustomerId.Value = "0";
    }
    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex > 0)
        {
            Session["selTeam"] = ddlTeam.SelectedValue;
            LoadEmp();
            //LoadTask(Session["selTeam"].ToString());
            ddlTask.Items.Clear();
            ddlSubTask.Items.Clear();
            gvRecords.DataSource = null;
            gvRecords.DataBind();
            dvAdd.Visible = false;
            select.Visible = false;
        }
        else
        {
            Session["selTeam"] = Session["team"];
            txtDate.Text = string.Empty;
            dvAdd.Visible = false;
        }
    }
    protected void txtDate_TextChanged(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(txtDate.Text))
        {
            if (ddlTeam.SelectedIndex > 0)
            {
                dvAdd.Visible = true;
                select.Visible = true;
                if (rbMultClient.Checked)
                {
                    dvClientsGrid.Visible = true;
                }
                else
                {
                    dvTasksGrid.Visible = true;
                }

               
                DisplayRecords();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Team", "alert('Please select team.')", true);
                txtDate.Text = string.Empty;
            }

            if (ddlEmployee.SelectedIndex > 0)
            {
                int offset = Convert.ToDateTime(txtDate.Text).DayOfWeek - DayOfWeek.Sunday;
                DateTime lastSunday = Convert.ToDateTime(txtDate.Text).AddDays(-offset);
                DateTime nextSaturday = lastSunday.AddDays(6);
                dt = objRec.CheckSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
                if (dt.Rows.Count > 0)
                {
                    string submit = dt.Rows[0]["R_Submit"].ToString();
                    if (submit == "1")
                    {
                        pnlAdd.Enabled = false;
                    }
                    else
                    {
                        dt = objRec.CheckLeaveSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
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
                    dt = objRec.CheckLeaveSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
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
        }
        else 
        {
            dvAdd.Visible= false;
        }
    }

    private void LoadTask(string taskId)
    {        
        SqlParameter[] parameters2 = new SqlParameter[]
            {
                new SqlParameter("@id", taskId),
            };
        dt = new DataTable();

       string SQlQuery = "select * from RTM_Task_List where TL_TeamId = @id and TL_Status =1 order By TL_Task";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters2);

        if (dt.Rows.Count > 0)
        {

            ddlTask.DataSource = dt;
            ddlTask.DataTextField = "TL_Task";
            ddlTask.DataValueField = "TL_ID";
            ddlTask.DataBind();
            ddlTask.Items.Insert(0, "--Select--");
            ddlTask.SelectedIndex = 0;
        }
    }

    private void LoadSubTasks()
    {
        string _client = "";
        dt = new DataTable();
        if (gvClients.Rows.Count > 0)
        {
            Label lblClientName = (Label)gvClients.Rows[0].Cells[0].FindControl("lblClientName");
            _client = lblClientName.Text;
        }
        if (_client == "Internal")
        {
            dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlTask.SelectedValue));
        }
        else if (_client == "")
        {
            ddlTask.Items.Clear();
            ddlSubTask.Items.Clear();
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
            ddlSubTask.Items.Insert(0, "--Select--");
            ddlSubTask.SelectedIndex = 0;
        }
    }
    protected void ddlSubTask_SelectedIndexChanged(object sender, EventArgs e)
    {
       
    }
    protected void ddlTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTask.SelectedIndex > 0)
        {
            LoadSubTasks();
        }
        else
        {
            ddlSubTask.AppendDataBoundItems = false;
            ddlSubTask.Items.Clear();
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {

        if (ddlEmployee.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select employee", "alert('Please select employee name')", true);
            return;
        }
        string sQuery = string.Empty;
        bool result;
        try
        {
            if (rbMultClient.Checked)
            {
                if (gvClients.Rows.Count <= 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select clients", "alert('Please add clients')", true);
                    return;
                }

                if (ddlTask.SelectedIndex <= 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Task", "alert('Please select task')", true);
                    return;
                }
                if (ddlSubTask.SelectedIndex <= 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Subtask", "alert('Please select subtask')", true);
                    return;
                }
                foreach (GridViewRow row in gvClients.Rows)
                {
                    Label lblClientId = (Label)row.FindControl("lblClientID");
                    Label lblClientName = (Label)row.FindControl("lblClientName");
                    TextBox txtDuration = (TextBox)row.FindControl("txtDuration");
                    TextBox txtComments = (TextBox)row.FindControl("txtComments");

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
                    if (txtDuration.Text.Contains(':'))
                    {
                        if (newRegex2.IsMatch(txtDuration.Text) == false)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                            return;
                        }
                    }
                   else if (newRegex.IsMatch(txtDuration.Text) == false)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                        return;
                    }
                   

                    //if (regex.IsMatch(txtDuration.Text + ":00") == false)
                    //{
                    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration", "alert('Please enter valid duration in HH:MM format for client - " + lblClientName.Text + "')", true);
                    //    return;
                    //}
                    if (!string.IsNullOrEmpty(txtDuration.Text))
                    {
                        continue;
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration", "alert('Please enter duration for client - " + lblClientName.Text + "')", true);
                        return;
                    }
                }

                foreach (GridViewRow row in gvClients.Rows)
                {
                    Label lblClientId = (Label)row.FindControl("lblClientID");
                    Label lblClientName = (Label)row.FindControl("lblClientName");
                    TextBox txtDuration = (TextBox)row.FindControl("txtDuration");
                    TextBox txtComments = (TextBox)row.FindControl("txtComments");

                    string addDuration = "00:00:00";
                   
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
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        string comm = txtComments.Text ;
                       
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@teamid", ddlTeam.SelectedValue),
                            new SqlParameter("@empid", ddlEmployee.SelectedValue),
                            new SqlParameter("@emp", ddlEmployee.SelectedItem.Text),
                            new SqlParameter("@newClient", lblClientId.Text),                        
                            new SqlParameter("@newtask", ddlTask.SelectedValue),
                            new SqlParameter("@subTask", ddlSubTask.SelectedValue),
                            new SqlParameter("@comments", comm ),
                            new SqlParameter("@duration", addDuration),
                            new SqlParameter("@date", Convert.ToDateTime(txtDate.Text)),
                            new SqlParameter("@date1", Convert.ToDateTime(txtDate.Text)),
                            new SqlParameter("@system", "P"),
                            new SqlParameter("@timedate",Convert.ToDateTime(txtDate.Text))
                        };


                        sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn, R_Status, R_System, R_TimeDate) values (@teamid, @empid, @emp, @newClient, @newtask, @subTask, @comments,@duration, @date,@date1, 'Completed',@system, @timedate)";
                        result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                    }

                }
                
            }
            else
            {
                if (gvTasks.Rows.Count <= 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select clients2", "alert('Please add clients')", true);
                    return;
                }

                foreach (GridViewRow row in gvTasks.Rows)
                {
                    Label lblClientId = (Label)row.FindControl("lblTaskClientID");
                    Label lblClientName = (Label)row.FindControl("lblTaskClientName");
                    TextBox txtDuration = (TextBox)row.FindControl("txtTaskDuration");
                    TextBox txtComments = (TextBox)row.FindControl("txtTaskComments");
                    DropDownList ddlGVTask = (DropDownList)row.FindControl("ddlGVTask");
                    DropDownList ddlGVSubTask = (DropDownList)row.FindControl("ddlGVSubTask");
                    if (ddlGVTask.SelectedIndex <= 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Task1", "alert('Please select task for all the clients')", true);
                        return;
                    }
                    if (ddlGVSubTask.SelectedIndex <= 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select SubTask1", "alert('Please select sub task for all the clients')", true);
                        return;
                    }

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
                    if (txtDuration.Text.Contains(':'))
                    {
                        if (newRegex2.IsMatch(txtDuration.Text) == false)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                            return;
                        }
                    }
                    else if (newRegex.IsMatch(txtDuration.Text) == false)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                        return;
                    }
                    //if (regex.IsMatch(txtDuration.Text + ":00") == false)
                    //{
                    //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration", "alert('Please enter valid duration in HH:MM format')", true);
                    //    return;
                    //}
                    if (!string.IsNullOrEmpty(txtDuration.Text))
                    {
                        continue;
                    }
                    else if (!string.IsNullOrWhiteSpace(txtDuration.Text))
                    {
                        continue;
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration", "alert('Please enter duration for client - " + lblClientName.Text + "')", true);
                        return;
                    }
                }

                foreach (GridViewRow row in gvTasks.Rows)
                {
                    Label lblClientId = (Label)row.FindControl("lblTaskClientID");
                    Label lblClientName = (Label)row.FindControl("lblTaskClientName");
                    TextBox txtDuration = (TextBox)row.FindControl("txtTaskDuration");
                    TextBox txtComments = (TextBox)row.FindControl("txtTaskComments");
                    DropDownList ddlGVTask = (DropDownList)row.FindControl("ddlGVTask");
                    DropDownList ddlGVSubTask = (DropDownList)row.FindControl("ddlGVSubTask");
                    string addDuration = "00:00:00";

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
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        string comm = txtComments.Text ;

                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@teamid", ddlTeam.SelectedValue),
                            new SqlParameter("@empid", ddlEmployee.SelectedValue),
                            new SqlParameter("@emp", ddlEmployee.SelectedItem.Text),
                            new SqlParameter("@newClient", lblClientId.Text),                        
                            new SqlParameter("@newtask", ddlGVTask.SelectedValue),
                            new SqlParameter("@subTask", ddlGVSubTask.SelectedValue),
                            new SqlParameter("@comments", comm ),
                            new SqlParameter("@duration", addDuration),
                            new SqlParameter("@date", Convert.ToDateTime(txtDate.Text)),
                            new SqlParameter("@date1", Convert.ToDateTime(txtDate.Text)),
                            new SqlParameter("@system", "P"),
                            new SqlParameter("@timedate", Convert.ToDateTime(txtDate.Text))
                        };

                        sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn, R_Status, R_System, R_TimeDate) values (@teamid, @empid, @emp, @newClient, @newtask, @subTask, @comments,@duration, @date,@date1, 'Completed', @system, @timedate)";
                        result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                    }

                }
            }
            

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Record Added", "alert('Records added Successfully')", true);
            ViewState["clients"] = null;
            ViewState["tasks"] = null;
            gvClients.DataSource = null;
            gvClients.DataBind();
            gvTasks.DataSource = dt;
            gvTasks.DataBind();
            txtSeachClient.Enabled = true;
            btnSearch.Enabled = true;
            ddlTask.Enabled = true;
            lblPleaseNote.Visible = false;
            lblSorry.Visible = false;
            ddlTask.SelectedIndex = 0;
            ddlSubTask.AppendDataBoundItems = false;
            ddlSubTask.Items.Clear();
            //ddlSubTask.SelectedIndex = 0;
            DisplayRecords();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void DisplayRecords()
    {

        
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",  Convert.ToDateTime(txtDate.Text).ToShortDateString()),
            new SqlParameter("@emp",  ddlEmployee.SelectedItem.Text),
        };
        dt = new DataTable();

        string SQlQuery = "select R_ID, R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Duration, R_Start_Date_Time, R_Comments from RTM_Records, RTM_Client_List, RTM_Task_List, RTM_SubTask_List where R_Client = CL_ID and R_Task = TL_ID and R_SubTask = STL_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) =@date and R_User_Name=@emp order By R_ID, R_Start_Date_Time";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
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
    protected void rbMultTask_CheckedChanged(object sender, EventArgs e)
    {
        txtSeachClient.Enabled = true;
        btnSearch.Enabled = true;
        txtSeachClient.Text = string.Empty;
        hfCustomerId.Value = "0";
        ddlTask.Enabled = true;

        lblSorry.Visible = false;
        lblPleaseNote.Visible = false;
        dvTasksGrid.Visible = true;
        dvClientsGrid.Visible = false;
    }
    protected void rbMultClient_CheckedChanged(object sender, EventArgs e)
    {
        txtSeachClient.Text = string.Empty;
        hfCustomerId.Value = "0";
        if (gvClients.Rows.Count > 0)
        {
            Label lblclname = (Label)gvClients.Rows[0].FindControl("lblClientName");
            if (lblclname.Text == "Internal")
            {
                txtSeachClient.Enabled = false;
                btnSearch.Enabled = false;
                dt = new DataTable();
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
                if (dt.Rows.Count > 0)
                {

                    ddlTask.DataSource = dt;
                    ddlTask.DataTextField = "TL_Task";
                    ddlTask.DataValueField = "TL_ID";
                    ddlTask.DataBind();
                    ddlTask.Items.Insert(0, "--Select--");
                    ddlTask.SelectedIndex = 0;
                }               

                lblPleaseNote.Visible = true;
            }
            else
            {
                dt = new DataTable();
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
                if (dt.Rows.Count > 0)
                {

                    ddlTask.DataSource = dt;
                    ddlTask.DataTextField = "TL_Task";
                    ddlTask.DataValueField = "TL_ID";
                    ddlTask.DataBind();
                    ddlTask.Items.Insert(0, "--Select--");
                    ddlTask.SelectedIndex = 0;
                }
                lblPleaseNote.Visible = false;
            }
        }
        lblSorry.Visible = false;
        //lblPleaseNote.Visible = false;
        dvClientsGrid.Visible = true;
        dvTasksGrid.Visible = false;
    }

    protected void gvTasks_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["tasks"] != null)
        {
            DataTable dt = (DataTable)ViewState["tasks"];
            DataRow drCurrentRow = null;
            int rowIndex = Convert.ToInt32(e.RowIndex);
            if (dt.Rows.Count > 0)
            {
                dt.Rows.Remove(dt.Rows[rowIndex]);
                drCurrentRow = dt.NewRow();
                ViewState["tasks"] = dt;
                gvTasks.DataSource = dt;
                gvTasks.DataBind();
                //for (int i = 0; i < gvTasks.Rows.Count - 1; i++)
                //{
                //    gvTasks.Rows[i].Cells[0].Text = Convert.ToString(i + 1);
                //}
                
                SetOldTaskData();
            }
        }
    }

    protected void ddlGVTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).Parent.Parent;
        DropDownList ddl = (DropDownList)sender;
        DropDownList ddlGVSubTask = gvr.FindControl("ddlGVSubTask") as DropDownList;
        Label lblClient = gvr.FindControl("lblTaskClientName") as Label;
        if (ddl.SelectedIndex > 0)
        {
            dt = new DataTable();
            if (lblClient.Text == "Internal")
            {
                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddl.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddl.SelectedValue));
            }
            
            if (dt.Rows.Count > 0)
            {
                ddlGVSubTask.DataSource = dt;
                ddlGVSubTask.DataTextField = "STL_SubTask";
                ddlGVSubTask.DataValueField = "STL_ID";
                ddlGVSubTask.DataBind();
                ddlGVSubTask.Items.Insert(0, "--Select--");
                ddlGVSubTask.SelectedIndex = 0;
            }
        }
        else
        {
            ddlGVSubTask.Items.Clear();
        }
    }


    protected void gvTasks_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList ddlGVTask = (e.Row.FindControl("ddlGVTask") as DropDownList);
            DropDownList ddlGVSubTask = (e.Row.FindControl("ddlGVSubTask") as DropDownList);
            Label lblTaskClientName = (e.Row.FindControl("lblTaskClientName") as Label);
            
            dt = new DataTable();
                        
            if (lblTaskClientName.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }

            if (dt.Rows.Count > 0)
            {
                ddlGVTask.DataSource = dt;
                ddlGVTask.DataTextField = "TL_Task";
                ddlGVTask.DataValueField = "TL_ID";
                ddlGVTask.DataBind();
                ddlGVTask.Items.Insert(0, "--Select--");
                ddlGVTask.SelectedIndex = 0;
            }
        }
    }

    protected void BindTaskGridview()
    {
        DataTable dt = new DataTable();

        dt.Columns.Add("clientId", typeof(string));
        dt.Columns.Add("clientName", typeof(string));
        dt.Columns.Add("task", typeof(string));
        dt.Columns.Add("subTask", typeof(string));
        dt.Columns.Add("duration", typeof(string));
        dt.Columns.Add("comments", typeof(string));
        DataRow dr = dt.NewRow();

        dr["clientId"] = hfCustomerId.Value;
        dr["clientName"] = txtSeachClient.Text;
        dr["task"] = string.Empty;
        dr["subTask"] = string.Empty;
        dr["duration"] = string.Empty;
        dr["comments"] = string.Empty;
        dt.Rows.Add(dr);
        ViewState["tasks"] = dt;
        gvTasks.DataSource = dt;
        gvTasks.DataBind();
    }

    private void AddNewTaskRow()
    {
        int rowIndex = 0;

        if (ViewState["tasks"] != null)
        {
            DataTable dt = (DataTable)ViewState["tasks"];
            DataRow drCurrentRow = null;
            if (dt.Rows.Count > 0)
            {
                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    DropDownList ddlGVTask = (DropDownList)gvTasks.Rows[rowIndex].Cells[2].FindControl("ddlGVTask");
                    DropDownList ddlGVSubTask = (DropDownList)gvTasks.Rows[rowIndex].Cells[2].FindControl("ddlGVSubTask");
                    TextBox txtDuration = (TextBox)gvTasks.Rows[rowIndex].Cells[2].FindControl("txtTaskDuration");
                    TextBox txtComments = (TextBox)gvTasks.Rows[rowIndex].Cells[2].FindControl("txtTaskComments");


                    drCurrentRow = dt.NewRow();

                    drCurrentRow["clientId"] = hfCustomerId.Value;
                    drCurrentRow["ClientName"] = txtSeachClient.Text;
                    if (ddlGVTask.Items.Count > 0)
                    {
                        dt.Rows[i - 1]["task"] = ddlGVTask.SelectedItem.Text;
                    }
                    else
                    {
                        dt.Rows[i - 1]["task"] = string.Empty;
                    }
                    if (ddlGVSubTask.Items.Count > 0)
                    {
                        dt.Rows[i - 1]["subTask"] = ddlGVSubTask.SelectedItem.Text;
                    }
                    else
                    {
                        dt.Rows[i - 1]["subTask"] = string.Empty;
                    }

                    dt.Rows[i - 1]["duration"] = txtDuration.Text;
                    dt.Rows[i - 1]["comments"] = txtComments.Text;


                    rowIndex++;
                }
                dt.Rows.Add(drCurrentRow);
                ViewState["tasks"] = dt;
                gvTasks.DataSource = dt;
                gvTasks.DataBind();
            }
            else
            {
                BindTaskGridview();
            }

        }
        else
        {
            BindTaskGridview();
            //Response.Write("ViewState Value is Null");
        }
        SetOldTaskData();
    }

    private void SetOldTaskData()
    {
        int rowIndex = 0;
        if (ViewState["tasks"] != null)
        {
            DataTable dt = (DataTable)ViewState["tasks"];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Label lblClientId = (Label)gvTasks.Rows[rowIndex].Cells[1].FindControl("lblTaskClientID");
                    Label lblClientName = (Label)gvTasks.Rows[rowIndex].Cells[1].FindControl("lblTaskClientName");
                    TextBox txtDuration = (TextBox)gvTasks.Rows[rowIndex].Cells[4].FindControl("txtTaskDuration");
                    TextBox txtComments = (TextBox)gvTasks.Rows[rowIndex].Cells[5].FindControl("txtTaskComments");
                    DropDownList ddlGVTask = (DropDownList)gvTasks.Rows[rowIndex].Cells[2].FindControl("ddlGVTask");
                    DropDownList ddlGVSubTask = (DropDownList)gvTasks.Rows[rowIndex].Cells[3].FindControl("ddlGVSubTask");
                    lblClientId.Text = dt.Rows[i]["clientId"].ToString();
                    lblClientName.Text = dt.Rows[i]["clientName"].ToString();
                    txtDuration.Text = dt.Rows[i]["duration"].ToString();
                    txtComments.Text = dt.Rows[i]["comments"].ToString();
                    string subTask = dt.Rows[i]["subTask"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[i]["task"].ToString()))
                    {
                        if (dt.Rows[i]["task"].ToString() != "--Select--")
                        {

                            ddlGVTask.ClearSelection();
                            ddlGVTask.Items.FindByText(dt.Rows[i]["task"].ToString()).Selected = true;
                        }

                        if (ddlGVTask.SelectedIndex > 0)
                        {
                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                new SqlParameter("@newTask", ddlGVTask.SelectedValue),
                            };
                            dtResult = new DataTable();

                            string SQlQuery = "select * from RTM_SubTask_List where STL_Task_Id= @newTask";
                            dtResult = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

                            if (dt.Rows.Count > 0)
                            {
                                ddlGVSubTask.DataSource = dtResult;
                                ddlGVSubTask.DataTextField = "STL_SubTask";
                                ddlGVSubTask.DataValueField = "STL_ID";
                                ddlGVSubTask.DataBind();
                                ddlGVSubTask.Items.Insert(0, "--Select--");
                                ddlGVSubTask.SelectedIndex = 0;
                                if (!string.IsNullOrEmpty(subTask))
                                {
                                    if (subTask != "--Select--")
                                    {
                                        ddlGVSubTask.ClearSelection();
                                        ddlGVSubTask.Items.FindByText(subTask).Selected = true;
                                    }
                                }                               
                            }
                        }
                    }

                    
                    
                    rowIndex++;
                }
            }
        }
    }
    protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlEmployee.SelectedIndex > 0)
        {
            if (!string.IsNullOrEmpty(txtDate.Text))
            {
                if (ddlTeam.SelectedIndex > 0)
                {
                    dvAdd.Visible = true;
                    select.Visible = true;
                    if (rbMultClient.Checked)
                    {
                        dvClientsGrid.Visible = true;
                    }
                    else
                    {
                        dvTasksGrid.Visible = true;
                    }


                    DisplayRecords();
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Team", "alert('Please select team.')", true);
                    txtDate.Text = string.Empty;
                }

                int offset = Convert.ToDateTime(txtDate.Text).DayOfWeek - DayOfWeek.Sunday;
                DateTime lastSunday = Convert.ToDateTime(txtDate.Text).AddDays(-offset);
                DateTime nextSaturday = lastSunday.AddDays(6);
                dt = objRec.CheckSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
                if (dt.Rows.Count > 0)
                {
                    string submit = dt.Rows[0]["R_Submit"].ToString();
                    if (submit == "1")
                    {
                        pnlAdd.Enabled = false;
                    }
                    else
                    {
                        dt = objRec.CheckLeaveSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
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
                    dt = objRec.CheckLeaveSubmitStatus(ddlEmployee.SelectedItem.Text, lastSunday.ToShortDateString(), nextSaturday.ToShortDateString());
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
                dvAdd.Visible = false;
            }
        }
    }
}