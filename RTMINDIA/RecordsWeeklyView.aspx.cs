using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Configuration;
using HtmlPivot;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using NReco.PivotData;
using NReco.PivotData.Output;
using MySql.Data.MySqlClient;
using BAL.Cache;

public partial class RecordsWeeklyView : System.Web.UI.Page
{
    clsTeam objTeam = new clsTeam();
    DAL.SqlDBHelper objDB = new DAL.SqlDBHelper();
    clsRealTimeReports objReal = new clsRealTimeReports();
    clsRecords objRec = new clsRecords();
    clsLeavePortal objLp = new clsLeavePortal();
    List<string> lstDate = new List<string>();
    DataTable dt = new DataTable();
    DataTable dtResult = new DataTable();
    DataTable dtEmpty = new DataTable();
    DataTable dtFooter = new DataTable();
    DataTable dtLeaves = new DataTable();
    DataTable dtRTMLeaves = new DataTable();
    string sQuery;
    bool result;
    Regex regex = new Regex("^(?:(?:([01]?\\d|2[0-3]):)([0-5]?\\d):)([0-5]?\\d)$");
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
        
        if (!Page.IsPostBack)
        {
            TasksCache.RemoveTasksCachedItem();
            SubTasksCache.RemoveSubTasksCachedItem();
            ClientsCache.RemoveClientsCachedItem();
            Session["team"] = Session["backupTeam"];
            string loc = Session["Location"].ToString();
            if (loc == "ADM")
            {
                Response.Redirect("~/ADMDashboard.aspx");
            }
            if (Session["access"].ToString() == "4")
            {
                btnApprove.Visible = false;
                btnReject.Visible = false;
                btnSubmit.Visible = true;
                btnBack.Visible = false;
            }
            else
            {
                btnSubmit.Visible = true;
                btnApprove.Visible = false;
                btnReject.Visible = false;
                btnReject.Attributes.Add("Disabled", "");
                btnBack.Visible = false;
            }

            
            LoadTeams();
            if (ddlTeam.Items.FindByValue(Session["team"].ToString()) != null)
            {
                if (Session["apTeam"] != null)
                {
                    if (ddlTeam.Items.FindByValue(Session["apTeam"].ToString()) != null)
                    {
                        ddlTeam.ClearSelection();
                        ddlTeam.Items.FindByValue(Session["apTeam"].ToString()).Selected = true;
                    }
                }
                else
                {
                    ddlTeam.ClearSelection();
                    ddlTeam.Items.FindByValue(Session["team"].ToString()).Selected = true;
                }

                if (ddlTeam.SelectedIndex > 0)
                {
                    LoadEmp();
                                        
                        if (Session["apUser"] != null)
                        {
                            if (ddlEmployee.Items.FindByText(Session["apUser"].ToString()) != null)
                            {
                                ddlEmployee.ClearSelection();
                                ddlEmployee.Items.FindByText(Session["apUser"].ToString()).Selected = true;
                            }
                        }
                        else
                        {
                            if (ddlEmployee.Items.FindByText(Session["username"].ToString()) != null)
                            {
                                ddlEmployee.ClearSelection();
                                ddlEmployee.Items.FindByText(Session["username"].ToString()).Selected = true;
                            }
                        }
                        if (ddlEmployee.SelectedIndex > 0)
                        {
                            int offset = DateTime.Now.DayOfWeek - DayOfWeek.Sunday;
                            DateTime lastSunday = DateTime.Now.AddDays(-offset);
                            DateTime nextSaturday = lastSunday.AddDays(6);
                            if (Session["apDate"] != null)
                            {
                                string selDate = Session["apDate"].ToString();
                                offset = Convert.ToDateTime(Session["apDate"].ToString()).DayOfWeek - DayOfWeek.Sunday;
                                lastSunday = Convert.ToDateTime(Session["apDate"].ToString()).AddDays(-offset);
                                nextSaturday = lastSunday.AddDays(6);
                            }
                            txtFrom.Text = lastSunday.ToShortDateString();
                            txtTo.Text = nextSaturday.ToShortDateString();
                            dvDate.Visible = true;
                            dvWeek.Visible = true;
                            dvWeekView.Visible = true;                           

                            BindAllData();
                        }
                    
                }
            }
            Session["weekTeam"] = ddlTeam.SelectedValue;

            dt = new DataTable();
            

            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                lnkCopyLastWeek.Visible = false;
                btnCopySun.Visible = false;
                btnCopyMon.Visible = false;
                btnCopyTue.Visible = false;
                btnCopyWed.Visible = false;
                btnCopyThu.Visible = false;
                btnCopyFri.Visible = false;
                btnCopySat.Visible = false;
            }
            
        }
    }

    private void DisableControls()
    {
        pnlSunday.Enabled = false;
        pnlMonday.Enabled = false;
        pnlTuesday.Enabled = false;
        pnlWednessday.Enabled = false;
        pnlThursday.Enabled = false;
        pnlFriday.Enabled = false;
        pnlSaturday.Enabled = false;
        //pnlHeading.Enabled = false;
        //lnkCopyLastWeek.Attributes.Add("Disabled", "");
    }

    private void EnableControls()
    {
        pnlSunday.Enabled = true;
        pnlMonday.Enabled = true;
        pnlTuesday.Enabled = true;
        pnlWednessday.Enabled = true;
        pnlThursday.Enabled = true;
        pnlFriday.Enabled = true;
        pnlSaturday.Enabled = true;
        //pnlHeading.Enabled = true;
        //lnkCopyLastWeek.Attributes.Remove("Disabled");
    }

    [WebMethod(EnableSession = true)]
    public static List<string> GetAutoCompleteData(string clientname)
    {
        string team = HttpContext.Current.Session["weekTeam"].ToString();        
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString()))
        {
            using (SqlCommand cmd = new SqlCommand("select CL_ID,CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_ClientName LIKE '%'+@SearchText+'%' and CL_Status=1 and CL_TeamId='" + team + "'", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", clientname);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}/{1}", dr["CL_ClientName"], dr["CL_ID"]));
                }
                return result;
            }
        }
    }

    [WebMethod(EnableSession = true)]
    public static string[] GetCustomers(string prefix)
    {
        string team = HttpContext.Current.Session["weekTeam"].ToString();
        List<string> customers = new List<string>();
       
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager.AppSettings["conString"].ToString();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "select CL_ID,CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_ClientName LIKE '%'+@SearchText+'%' and CL_Status=1 and CL_TeamId='" + team + "'";
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

    private void bindemptytable()
    {
        dtEmpty = new DataTable();
        DataColumn dc;
        dc = new DataColumn("R_ID");
        dtEmpty.Columns.Add(dc);

        dc = new DataColumn("R_User_Name");
        dtEmpty.Columns.Add(dc);

        dc = new DataColumn("CL_ClientName");
        dtEmpty.Columns.Add(dc);

        dc = new DataColumn("TL_Task");
        dtEmpty.Columns.Add(dc);

        dc = new DataColumn("STL_SubTask");
        dtEmpty.Columns.Add(dc);

        dc = new DataColumn("R_Duration");
        dtEmpty.Columns.Add(dc);

        dc = new DataColumn("R_Comments");
        dtEmpty.Columns.Add(dc);

        dtEmpty.Rows.Add("0","", "", "", "", "", "");
    }

    private DataTable GetTasks(int teamId)
    {
        dt = new DataTable();
        dt = TasksCache.GetTasks(teamId);
        return dt;
    }

    private DataTable GetSubTasks(int teamId, int taskId)
    {
        dt = new DataTable();
        dt = SubTasksCache.GetSubTasks(teamId);
        dt = dt.Select("STL_Task_Id =" + taskId).CopyToDataTable();
        return dt;
    }

    private DataTable GetClientsWithoutPTOs(int teamId)
    {
        dt = new DataTable();
        dt = ClientsCache.GetClients(teamId);
        dt = dt.Select("CL_ClientName <> 'Personal/Sick Time' and CL_ClientName <> 'Public Holiday' and CL_ClientName <> 'Vacation' and CL_ClientName <> 'Bereavement' and CL_ClientName <> 'Jury Duty' and CL_ClientName <> 'Inclement Weather'").CopyToDataTable();
        return dt;
    }

    private DataTable GetAllClients(int teamId)
    {
        dt = new DataTable();
        dt = ClientsCache.GetClients(teamId);
        return dt;
    }

    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex > 0)
        {
            TasksCache.RemoveTasksCachedItem();
            SubTasksCache.RemoveSubTasksCachedItem();
            ClientsCache.RemoveClientsCachedItem();
            LoadEmp();
            Session["weekTeam"] = ddlTeam.SelectedValue;
            dvDate.Visible = false;
            dvWeek.Visible = false;
            dvWeekView.Visible = false;
        }
        else
        {
            TasksCache.RemoveTasksCachedItem();
            SubTasksCache.RemoveSubTasksCachedItem();
            ClientsCache.RemoveClientsCachedItem();
            Session["weekTeam"] = ddlTeam.SelectedIndex;
            dvDate.Visible = false;
            dvWeek.Visible = false;
            dvWeekView.Visible = false;
        }
    }

    private void FetchDate(string from, string to)
    {
        int noOfDays = Convert.ToInt32((Convert.ToDateTime(to) - Convert.ToDateTime(from)).TotalDays);
        checked
        {
            for (int i = 0; i <= noOfDays; i++)
            {
                lstDate.Add(Convert.ToDateTime(from).AddDays(i).ToShortDateString());
            }
        }
    }

    public DataTable DisplayAllRecords(string date, string emp, string toDate)
    {
        dt = new DataTable();
        SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@date",  date),
               new SqlParameter("@emp", emp),
               new SqlParameter("@toDate", toDate)
           };


        sQuery = "select R_ID, R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Duration, R_Start_Date_Time, R_Comments, cast(R_TimeDate as date) as R_TimeDate from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @date and @toDate and R_User_Name=@emp and R_Status !='Running' order By R_ID, R_Start_Date_Time";
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

        return dt;
    }

    public DataTable DisplayRecords(string date, string emp)
    {
        dt = new DataTable();
        SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@date",  date),
               new SqlParameter("@emp", emp),
           };


        sQuery = "select R_ID, R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Duration, R_Start_Date_Time, R_Comments from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) = @date and R_User_Name=@emp and R_Status !='Running' order By R_ID, R_Start_Date_Time";
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

        return dt;
    }

    public DataTable DisplayRecordsNew(string from, string to, string emp)
    {
        DataTable dtRecords = new DataTable();
        SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@from",  from),
               new SqlParameter("@to", to),
               new SqlParameter("@emp", emp)
           };


        sQuery = "select R_ID, R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Duration, CONVERT(CHAR(10), R_TimeDate, 101) as R_Start_Date_Time, R_Comments from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWeen @from and @to and R_User_Name=@emp and R_Status !='Running' order By R_ID, R_Start_Date_Time";
        dtRecords = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

        return dtRecords;
    }

    protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlEmployee.SelectedIndex > 0)
            {                
                dvDate.Visible = true;
                dvWeek.Visible = true;
                dvWeekView.Visible = true;
                if (string.IsNullOrEmpty(txtFrom.Text))
                {
                    int offset = DateTime.Now.DayOfWeek - DayOfWeek.Sunday;
                    DateTime lastSunday = DateTime.Now.AddDays(-offset);
                    DateTime nextSaturday = lastSunday.AddDays(6);
                    txtFrom.Text = lastSunday.ToShortDateString();
                    txtTo.Text = nextSaturday.ToShortDateString();
                    //txtFrom.Text = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 7).ToShortDateString();
                    //txtTo.Text = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 1).ToShortDateString();
                }               

                BindAllData();            
            }
        }
        catch (Exception)
        {            
            throw;
        }
        
    }
    protected void btnView_Click(object sender, EventArgs e)
    {
        if (Convert.ToDateTime(txtFrom.Text).DayOfWeek != DayOfWeek.Sunday)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select valid From", "alert('Please Select the date from Sunday to Saturday of same week.')", true);
            return;
        }

        if (Convert.ToDateTime(txtTo.Text) != Convert.ToDateTime(txtFrom.Text).AddDays(6))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select valid To", "alert('Please Select the date from Sunday to Saturday of same week.')", true);
            return;
        }

        BindAllData();       

    }

    //Sunday

    protected void gvRecordsSunday_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsSunday.EditIndex == e.Row.RowIndex)
        {
            dt = new DataTable();
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlSGvClient");
            Label lblClient = (Label)e.Row.FindControl("lblSEditClient");


            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id", ddlTeam.SelectedValue),
           };

            sQuery = "select * from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Inclement Weather' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!= 'Medical Leave' and CL_ClientName!= 'Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            //dt = GetClientsWithoutPTOs(Convert.ToInt32(ddlTeam.SelectedValue));

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
            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlSGvTask");
            
            Label lblTask = (Label)e.Row.FindControl("lblSEditTask");

            if (lblClient.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }

            //dt = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));          

            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
                if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                {
                    ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                }
               
            }
            dt = new DataTable();

            DropDownList ddlGvSubTask = (DropDownList)e.Row.FindControl("ddlSGvSubTask");
            Label lblEditSubtask = (Label)e.Row.FindControl("lblSEditSubtask");


            SqlParameter[] parameters2 = new SqlParameter[]
           {           
               new SqlParameter("@task",ddlGvTask.SelectedValue),
           };

            sQuery = "select STL_ID, STL_SubTask from RTM_SubTask_List WITH (NOLOCK)  where STL_Task_Id =@task and STL_ViewStatus=1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

            //dt = GetSubTasks(Convert.ToInt32(ddlTeam.SelectedValue), Convert.ToInt32(ddlGvTask.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvSubTask.DataSource = dt;
                ddlGvSubTask.DataTextField = "STL_SubTask";
                ddlGvSubTask.DataValueField = "STL_ID";
                ddlGvSubTask.DataBind();
                if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                {
                    ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsSunday.EditIndex != e.Row.RowIndex)
        {
            Label ddlGvClient = (Label)e.Row.FindControl("lblSClient");
            if (ddlGvClient.Text == "Personal/Sick Time" || ddlGvClient.Text == "Inclement Weather" || ddlGvClient.Text == "Public Holiday" || ddlGvClient.Text == "Vacation" || ddlGvClient.Text == "Bereavement" || ddlGvClient.Text == "Jury Duty" || ddlGvClient.Text == "Maternity Leave" || ddlGvClient.Text == "Paternity Leave" || ddlGvClient.Text == "Medical Leave" || ddlGvClient.Text == "Comp off")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }

            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            string myDataKey = rowView["R_ID"].ToString();
            if (myDataKey == "0")
                e.Row.Visible = false;
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSADDUser = (Label)e.Row.FindControl("lblSADDUser");
            lblSADDUser.Text = ddlEmployee.SelectedItem.Text;

            //DropDownList ddlSGVAddTask = (DropDownList)e.Row.FindControl("ddlSAddTask");
         
            //dtFooter = new DataTable();
            //dtFooter = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            
            //if (dtFooter.Rows.Count > 0)
            //{
            //    ddlSGVAddTask.DataSource = dtFooter;
            //    ddlSGVAddTask.DataTextField = "TL_Task";
            //    ddlSGVAddTask.DataValueField = "TL_ID";
            //    ddlSGVAddTask.DataBind();
            //    ddlSGVAddTask.Items.Insert(0, "--Select Task--");
            //    ddlSGVAddTask.SelectedIndex = 0;

            //    ddlSGVAddTask.Enabled = false;

            //}
        }
    }
    protected void gvRecordsSunday_RowEditing(object sender, GridViewEditEventArgs e)
    {
        
        gvRecordsSunday.EditIndex = e.NewEditIndex;
        dt = DisplayRecords(lblSunday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsSunday.DataSource = dt;
            gvRecordsSunday.DataBind();
        }
        
    }
    protected void gvRecordsSunday_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecordsSunday.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecordsSunday.Rows[e.RowIndex].FindControl("ddlSGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecordsSunday.Rows[e.RowIndex].FindControl("ddlSGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecordsSunday.Rows[e.RowIndex].FindControl("ddlSGvSubTask");
            TextBox txtComments = (TextBox)gvRecordsSunday.Rows[e.RowIndex].FindControl("txtSComments");
            TextBox txtDuration = (TextBox)gvRecordsSunday.Rows[e.RowIndex].FindControl("txtSDuration");
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
            //if (regex.IsMatch(txtDuration.Text) == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration4", "alert('Please enter valid duration in HH:MM:SS format')", true);
            //    return;
            //}

            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid),
               new SqlParameter("@duration",addDuration),
               new SqlParameter("@system", "P")
           };

            sQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Duration=@duration, R_Comments=@comments, R_System = @system where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            gvRecordsSunday.EditIndex = -1;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            dt = DisplayRecords(lblSunday.Text, ddlEmployee.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                gvRecordsSunday.DataSource = dt;
                gvRecordsSunday.DataBind();
                ViewState["dirSun"] = dt;
                ViewState["sortdrS"] = "Asc";
            }

            BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        }
        catch (Exception ex)
        {
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
            //gvRecords.EditIndex = -1;
            //DisplayRecords();
        }
    }
    protected void gvRecordsSunday_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecordsSunday.EditIndex = -1;
        dt = DisplayRecords(lblSunday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsSunday.DataSource = dt;
            gvRecordsSunday.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlSGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlSGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlSGvSubTask");
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlSGvClient");

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

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsSunday_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int rowid = Convert.ToInt32(gvRecordsSunday.DataKeys[e.RowIndex].Value.ToString());
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblSunday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsSunday.DataSource = dt;
            gvRecordsSunday.DataBind();
            ViewState["dirSun"] = dt;
            ViewState["sortdrS"] = "Asc";
        }
        else
        {
            gvRecordsSunday.DataSource = null;
            gvRecordsSunday.DataBind();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void lnkSunDelete_Click(object sender, EventArgs e)
    {
        
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        int rowid = Convert.ToInt32(gvRecordsSunday.DataKeys[grdrow.RowIndex].Value.ToString());
        Label lblclient = (Label)grdrow.FindControl("lblSClient");
        if (lblclient.Text == "Personal/Sick Time" || lblclient.Text == "Inclement Weather" || lblclient.Text == "Public Holiday" || lblclient.Text == "Vacation" || lblclient.Text == "Bereavement" || lblclient.Text == "Jury Duty" || lblclient.Text == "Maternity Leave" || lblclient.Text == "Paternity Leave" || lblclient.Text == "Medical Leave" || lblclient.Text == "Comp off")
        {

        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access to delete", "alert('you are not authorized to delete work time.')", true);
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                return;
            }
        }

        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblSunday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsSunday.DataSource = dt;
            gvRecordsSunday.DataBind();
            ViewState["dirSun"] = dt;
            ViewState["sortdrS"] = "Asc";
        }
        else
        {
           
            bindemptytable();
            gvRecordsSunday.DataSource = dtEmpty;
            gvRecordsSunday.DataBind();

            gvRecordsSunday.ShowHeaderWhenEmpty = true;
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void lnkMonDelete_Click(object sender, EventArgs e)
    {
       
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        int rowid = Convert.ToInt32(gvRecordsMonday.DataKeys[grdrow.RowIndex].Value.ToString());
        Label lblclient = (Label)grdrow.FindControl("lblMClient");
        if (lblclient.Text == "Personal/Sick Time" || lblclient.Text == "Inclement Weather" || lblclient.Text == "Public Holiday" || lblclient.Text == "Vacation" || lblclient.Text == "Bereavement" || lblclient.Text == "Jury Duty" || lblclient.Text == "Maternity Leave" || lblclient.Text == "Paternity Leave" || lblclient.Text == "Medical Leave" || lblclient.Text == "Comp off")
        {

        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access to delete", "alert('you are not authorized to delete work time.')", true);
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                return;
            }
        }

        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblMonday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsMonday.DataSource = dt;
            gvRecordsMonday.DataBind();
            ViewState["dirMon"] = dt;
            ViewState["sortdr"] = "Asc";
        }
        else
        {
            bindemptytable();
            gvRecordsMonday.DataSource = dtEmpty;
            gvRecordsMonday.DataBind();

            gvRecordsMonday.ShowHeaderWhenEmpty = true;
        }
       // ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + gvRecordsMonday.ClientID + "', 250, 1000 , 27 ,true); </script>", false);

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    clsAuditTrail objAuditTrail = new clsAuditTrail();
    protected void lnkTueDelete_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        int rowid = Convert.ToInt32(gvRecordsTue.DataKeys[grdrow.RowIndex].Value.ToString());
        Label lblclient = (Label)grdrow.FindControl("lblTClient");
        if (lblclient.Text == "Personal/Sick Time" || lblclient.Text == "Inclement Weather" || lblclient.Text == "Public Holiday" || lblclient.Text == "Vacation" || lblclient.Text == "Bereavement" || lblclient.Text == "Jury Duty" || lblclient.Text == "Maternity Leave" || lblclient.Text == "Paternity Leave" || lblclient.Text == "Medical Leave" || lblclient.Text == "Comp off")
        {

        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access to delete", "alert('you are not authorized to delete work time.')", true);
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                return;
            }
        }

        try
        {
            objAuditTrail.AddLogs("Deleted", "RTM_Records", Session["username"].ToString(), DateTime.Now);
        }

        catch (Exception ex)
        {
        }

        
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblTuesday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsTue.DataSource = dt;
            gvRecordsTue.DataBind();
            ViewState["dirTue"] = dt;
            ViewState["sortdrT"] = "Asc";
        }
        else
        {
            bindemptytable();
            gvRecordsTue.DataSource = dtEmpty;
            gvRecordsTue.DataBind();

            gvRecordsTue.ShowHeaderWhenEmpty = true;
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void lnkWedDelete_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        int rowid = Convert.ToInt32(gvRecordsWed.DataKeys[grdrow.RowIndex].Value.ToString());
        Label lblclient = (Label)grdrow.FindControl("lblWClient");
        if (lblclient.Text == "Personal/Sick Time" || lblclient.Text == "Inclement Weather" || lblclient.Text == "Public Holiday" || lblclient.Text == "Vacation" || lblclient.Text == "Bereavement" || lblclient.Text == "Jury Duty" || lblclient.Text == "Maternity Leave" || lblclient.Text == "Paternity Leave" || lblclient.Text == "Medical Leave" || lblclient.Text == "Comp off")
        {

        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access to delete", "alert('you are not authorized to delete work time.')", true);
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                return;
            }
        }
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblWed.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsWed.DataSource = dt;
            gvRecordsWed.DataBind();
            ViewState["dirWed"] = dt;
            ViewState["sortdrW"] = "Asc";
        }
        else
        {
            bindemptytable();
            gvRecordsWed.DataSource = dtEmpty;
            gvRecordsWed.DataBind();

            gvRecordsWed.ShowHeaderWhenEmpty = true;
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void lnkThuDelete_Click(object sender, EventArgs e)
    {
        //if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access to delete", "alert('you are not authorized to delete work time.')", true);
        //    return;
        //}

        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        int rowid = Convert.ToInt32(gvRecordsThu.DataKeys[grdrow.RowIndex].Value.ToString());
        Label lblclient = (Label)grdrow.FindControl("lblTHClient");
        if (lblclient.Text == "Personal/Sick Time" || lblclient.Text == "Inclement Weather" || lblclient.Text == "Public Holiday" || lblclient.Text == "Vacation" || lblclient.Text == "Bereavement" || lblclient.Text == "Jury Duty" || lblclient.Text == "Maternity Leave" || lblclient.Text == "Paternity Leave" || lblclient.Text == "Medical Leave" || lblclient.Text == "Comp off")
        {

        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access to delete", "alert('you are not authorized to delete work time.')", true);
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                return;
            }
        }
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblThu.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsThu.DataSource = dt;
            gvRecordsThu.DataBind();
            ViewState["dirThu"] = dt;
            ViewState["sortdrTH"] = "Asc";
        }
        else
        {
            bindemptytable();
            gvRecordsThu.DataSource = dtEmpty;
            gvRecordsThu.DataBind();

            gvRecordsThu.ShowHeaderWhenEmpty = true;
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void lnkFriDelete_Click(object sender, EventArgs e)
    {
        
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        int rowid = Convert.ToInt32(gvRecordsFri.DataKeys[grdrow.RowIndex].Value.ToString());
        Label lblclient = (Label)grdrow.FindControl("lblFClient");
        if (lblclient.Text == "Personal/Sick Time" || lblclient.Text == "Inclement Weather" || lblclient.Text == "Public Holiday" || lblclient.Text == "Vacation" || lblclient.Text == "Bereavement" || lblclient.Text == "Jury Duty" || lblclient.Text == "Maternity Leave" || lblclient.Text == "Paternity Leave" || lblclient.Text == "Medical Leave" || lblclient.Text == "Comp off")
        {

        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access to delete", "alert('you are not authorized to delete work time.')", true);
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                return;
            }
        }
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblFriday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsFri.DataSource = dt;
            gvRecordsFri.DataBind();
            ViewState["dirFri"] = dt;
            ViewState["sortdrF"] = "Asc";
        }
        else
        {
            bindemptytable();
            gvRecordsFri.DataSource = dtEmpty;
            gvRecordsFri.DataBind();

            gvRecordsFri.ShowHeaderWhenEmpty = true;
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void lnkSatDelete_Click(object sender, EventArgs e)
    {
        

        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        int rowid = Convert.ToInt32(gvRecordsSat.DataKeys[grdrow.RowIndex].Value.ToString());
        Label lblclient = (Label)grdrow.FindControl("lblSAClient");
        if (lblclient.Text == "Personal/Sick Time" || lblclient.Text == "Inclement Weather" || lblclient.Text == "Public Holiday" || lblclient.Text == "Vacation" || lblclient.Text == "Bereavement" || lblclient.Text == "Jury Duty" || lblclient.Text == "Maternity Leave" || lblclient.Text == "Paternity Leave" || lblclient.Text == "Medical Leave" || lblclient.Text == "Comp off")
        {

        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access to delete", "alert('you are not authorized to delete work time.')", true);
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                return;
            }
        }
       
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblSat.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsSat.DataSource = dt;
            gvRecordsSat.DataBind();
            ViewState["dirSat"] = dt;
            ViewState["sortdrSA"] = "Asc";
        }
        else
        {
            bindemptytable();
            gvRecordsSat.DataSource = dtEmpty;
            gvRecordsSat.DataBind();

            gvRecordsSat.ShowHeaderWhenEmpty = true;
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlSAddTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlSAddTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlSADDSubTask");
        TextBox txtCL = (TextBox)Grow.FindControl("txtSearchGvClient");

        dt = new DataTable();
        if (txtCL.Text == "Internal")
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }
        else
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }

        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();

            ddlGvSubTask.Items.Insert(0, "--Select Subtask--");
            ddlGvSubTask.SelectedIndex = 0;
        }

        if (ddlGvTask.SelectedIndex == 0)
        {
            ddlGvSubTask.Items.Clear();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlMAddTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlMAddTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlMADDSubTask");
        TextBox txtCL = (TextBox)Grow.FindControl("txtSearchGvClientM");

        dt = new DataTable();
        if (txtCL.Text == "Internal")
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }
        else
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }

        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();

            ddlGvSubTask.Items.Insert(0, "--Select Subtask--");
            ddlGvSubTask.SelectedIndex = 0;
        }

        if (ddlGvTask.SelectedIndex == 0)
        {
            ddlGvSubTask.Items.Clear();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlTAddTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlTAddTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlTADDSubTask");
        TextBox txtCL = (TextBox)Grow.FindControl("txtSearchGvClientT");

        dt = new DataTable();
        if (txtCL.Text == "Internal")
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }
        else
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }

        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();

            ddlGvSubTask.Items.Insert(0, "--Select Subtask--");
            ddlGvSubTask.SelectedIndex = 0;
        }

        if (ddlGvTask.SelectedIndex == 0)
        {
            ddlGvSubTask.Items.Clear();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlWAddTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlWAddTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlWADDSubTask");
        TextBox txtCL = (TextBox)Grow.FindControl("txtSearchGvClientW");

        dt = new DataTable();
        if (txtCL.Text == "Internal")
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }
        else
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }

        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();

            ddlGvSubTask.Items.Insert(0, "--Select Subtask--");
            ddlGvSubTask.SelectedIndex = 0;
        }

        if (ddlGvTask.SelectedIndex == 0)
        {
            ddlGvSubTask.Items.Clear();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlTHAddTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlTHAddTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlTHADDSubTask");
        TextBox txtCL = (TextBox)Grow.FindControl("txtSearchGvClientTH");

        dt = new DataTable();
        if (txtCL.Text == "Internal")
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }
        else
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }

        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();

            ddlGvSubTask.Items.Insert(0, "--Select Subtask--");
            ddlGvSubTask.SelectedIndex = 0;
        }

        if (ddlGvTask.SelectedIndex == 0)
        {
            ddlGvSubTask.Items.Clear();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlFAddTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlFAddTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlFADDSubTask");
        TextBox txtCL = (TextBox)Grow.FindControl("txtSearchGvClientF");

        dt = new DataTable();
        if (txtCL.Text == "Internal")
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }
        else
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }

        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();

            ddlGvSubTask.Items.Insert(0, "--Select Subtask--");
            ddlGvSubTask.SelectedIndex = 0;
        }

        if (ddlGvTask.SelectedIndex == 0)
        {
            ddlGvSubTask.Items.Clear();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlSAAddTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlSAAddTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlSAADDSubTask");
        TextBox txtCL = (TextBox)Grow.FindControl("txtSearchGvClientSA");

        dt = new DataTable();
        if (txtCL.Text == "Internal")
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetGeneralSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }
        else
        {
            if (ddlGvTask.SelectedIndex > 0)
            {
                dt = objRec.GetClientSubTasks(Convert.ToInt32(ddlGvTask.SelectedValue));
            }
        }

        if (dt.Rows.Count > 0)
        {
            ddlGvSubTask.DataSource = dt;
            ddlGvSubTask.DataTextField = "STL_SubTask";
            ddlGvSubTask.DataValueField = "STL_ID";
            ddlGvSubTask.DataBind();

            ddlGvSubTask.Items.Insert(0, "--Select Subtask--");
            ddlGvSubTask.SelectedIndex = 0;
        }

        if (ddlGvTask.SelectedIndex == 0)
        {
            ddlGvSubTask.Items.Clear();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    //Monday

    protected void gvRecordsMonday_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsMonday.EditIndex == e.Row.RowIndex)
        {
            dt = new DataTable();
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlMGvClient");
            Label lblClient = (Label)e.Row.FindControl("lblMEditClient");


            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id", ddlTeam.SelectedValue),
           };

            sQuery = "select CL_ID, CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Inclement Weather' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            //dt = GetClientsWithoutPTOs(Convert.ToInt32(ddlTeam.SelectedValue));

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
            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlMGvTask");
            Label lblTask = (Label)e.Row.FindControl("lblMEditTask");

            if (lblClient.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }

            //dt = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
           

            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
                if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                {
                    ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                }
            }
            dt = new DataTable();

            DropDownList ddlGvSubTask = (DropDownList)e.Row.FindControl("ddlMGvSubTask");
            Label lblEditSubtask = (Label)e.Row.FindControl("lblMEditSubtask");


            SqlParameter[] parameters2 = new SqlParameter[]
           {           
               new SqlParameter("@task",ddlGvTask.SelectedValue),
           };

            sQuery = "select STL_ID, STL_SubTask from RTM_SubTask_List WITH (NOLOCK)  where STL_Task_Id =@task and STL_ViewStatus=1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

            //dt = GetSubTasks(Convert.ToInt32(ddlTeam.SelectedValue), Convert.ToInt32(ddlGvTask.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvSubTask.DataSource = dt;
                ddlGvSubTask.DataTextField = "STL_SubTask";
                ddlGvSubTask.DataValueField = "STL_ID";
                ddlGvSubTask.DataBind();
                if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                {
                    ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsMonday.EditIndex != e.Row.RowIndex)
        {
            Label ddlGvClient = (Label)e.Row.FindControl("lblMClient");
            if (ddlGvClient.Text == "Personal/Sick Time" || ddlGvClient.Text == "Inclement Weather" || ddlGvClient.Text == "Public Holiday" || ddlGvClient.Text == "Vacation" || ddlGvClient.Text == "Bereavement" || ddlGvClient.Text == "Jury Duty" || ddlGvClient.Text == "Maternity Leave" || ddlGvClient.Text == "Paternity Leave" || ddlGvClient.Text == "Medical Leave" || ddlGvClient.Text == "Comp off")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }

            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            string myDataKey = rowView["R_ID"].ToString();
            
            if (myDataKey == "0")
            {
                e.Row.Visible = false;
            }

            
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSADDUser = (Label)e.Row.FindControl("lblMADDUser");
            lblSADDUser.Text = ddlEmployee.SelectedItem.Text;
            //DropDownList ddlSGVAddTask = (DropDownList)e.Row.FindControl("ddlMAddTask");
           
            //dtFooter = new DataTable();
            //dtFooter = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            
            //if (dtFooter.Rows.Count > 0)
            //{
            //    ddlSGVAddTask.DataSource = dtFooter;
            //    ddlSGVAddTask.DataTextField = "TL_Task";
            //    ddlSGVAddTask.DataValueField = "TL_ID";
            //    ddlSGVAddTask.DataBind();
            //    ddlSGVAddTask.Items.Insert(0, "--Select Task--");
            //    ddlSGVAddTask.SelectedIndex = 0;

            //    ddlSGVAddTask.Enabled = false;

            //}
        }
    }
    protected void gvRecordsMonday_RowEditing(object sender, GridViewEditEventArgs e)
    {
       
        gvRecordsMonday.EditIndex = e.NewEditIndex;
        
        dt = DisplayRecords(lblMonday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsMonday.DataSource = dt;
            gvRecordsMonday.DataBind();
        }
      // ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + gvRecordsMonday.ClientID + "', 250, 1000 , 27 ,true); </script>", false);
    }
    protected void gvRecordsMonday_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecordsMonday.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecordsMonday.Rows[e.RowIndex].FindControl("ddlMGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecordsMonday.Rows[e.RowIndex].FindControl("ddlMGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecordsMonday.Rows[e.RowIndex].FindControl("ddlMGvSubTask");
            TextBox txtComments = (TextBox)gvRecordsMonday.Rows[e.RowIndex].FindControl("txtMComments");
            TextBox txtDuration = (TextBox)gvRecordsMonday.Rows[e.RowIndex].FindControl("txtMDuration");

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

            //if (regex.IsMatch(txtDuration.Text) == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration2", "alert('Please enter valid duration in HH:MM:SS format')", true);
            //    return;
                
            //}

            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid),
               new SqlParameter("@duration", addDuration),
               new SqlParameter("@system", "P")
           };

            sQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Duration=@duration, R_Comments=@comments, R_System = @system where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            gvRecordsMonday.EditIndex = -1;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            dt = DisplayRecords(lblMonday.Text, ddlEmployee.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                gvRecordsMonday.DataSource = dt;
                gvRecordsMonday.DataBind();
                ViewState["dirMon"] = dt;
                ViewState["sortdr"] = "Asc";
            }
          // ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + gvRecordsMonday.ClientID + "', 250, 1000 , 27 ,true); </script>", false);

            BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        }
        catch (Exception ex)
        {
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
            //gvRecords.EditIndex = -1;
            //DisplayRecords();
        }
    }
    protected void gvRecordsMonday_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecordsMonday.EditIndex = -1;
        dt = DisplayRecords(lblMonday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsMonday.DataSource = dt;
            gvRecordsMonday.DataBind();
        }
     // ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + gvRecordsMonday.ClientID + "', 250, 1000 , 27 ,true); </script>", false);
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlMGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlMGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlMGvSubTask");
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlMGvClient");

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

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsMonday_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int rowid = Convert.ToInt32(gvRecordsMonday.DataKeys[e.RowIndex].Value.ToString());
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblMonday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsMonday.DataSource = dt;
            gvRecordsMonday.DataBind();
            ViewState["dirMon"] = dt;
            ViewState["sortdr"] = "Asc";
        }
        else
        {
            gvRecordsMonday.DataSource = null;
            gvRecordsMonday.DataBind();
        }
      // ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + gvRecordsMonday.ClientID + "', 250, 1000 , 27 ,true); </script>", false);

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    //Tuesday

    protected void gvRecordsTue_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsTue.EditIndex == e.Row.RowIndex)
        {
            dt = new DataTable();
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlTGvClient");
            Label lblClient = (Label)e.Row.FindControl("lblTEditClient");


            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id", ddlTeam.SelectedValue),
           };

            sQuery = "select CL_ID, CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_CLientName!='Inclement Weather' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            //dt = GetClientsWithoutPTOs(Convert.ToInt32(ddlTeam.SelectedValue));

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
            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlTGvTask");
            Label lblTask = (Label)e.Row.FindControl("lblTEditTask");

            if (lblClient.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
           
           // dt = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            

            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
                if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                {
                    ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                }
            }
            dt = new DataTable();

            DropDownList ddlGvSubTask = (DropDownList)e.Row.FindControl("ddlTGvSubTask");
            Label lblEditSubtask = (Label)e.Row.FindControl("lblTEditSubtask");


            SqlParameter[] parameters2 = new SqlParameter[]
           {           
               new SqlParameter("@task",ddlGvTask.SelectedValue),
           };

            sQuery = "select STL_ID, STL_SubTask from RTM_SubTask_List WITH (NOLOCK) where STL_Task_Id =@task and STL_ViewStatus=1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

           // dt = GetSubTasks(Convert.ToInt32(ddlTeam.SelectedValue), Convert.ToInt32(ddlGvTask.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvSubTask.DataSource = dt;
                ddlGvSubTask.DataTextField = "STL_SubTask";
                ddlGvSubTask.DataValueField = "STL_ID";
                ddlGvSubTask.DataBind();
                if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                {
                    ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsTue.EditIndex != e.Row.RowIndex)
        {
            Label ddlGvClient = (Label)e.Row.FindControl("lblTClient");
            if (ddlGvClient.Text == "Personal/Sick Time" || ddlGvClient.Text == "Inclement Weather" || ddlGvClient.Text == "Public Holiday" || ddlGvClient.Text == "Vacation" || ddlGvClient.Text == "Bereavement" || ddlGvClient.Text == "Jury Duty" || ddlGvClient.Text == "Maternity Leave" || ddlGvClient.Text == "Paternity Leave" || ddlGvClient.Text == "Medical Leave" || ddlGvClient.Text == "Comp off")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }

            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            string myDataKey = rowView["R_ID"].ToString();
            if (myDataKey == "0")
                e.Row.Visible = false;
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSADDUser = (Label)e.Row.FindControl("lblTADDUser");
            lblSADDUser.Text = ddlEmployee.SelectedItem.Text;

            //DropDownList ddlSGVAddTask = (DropDownList)e.Row.FindControl("ddlTAddTask");
           
            //dtFooter = new DataTable();
            //dtFooter = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            
            //if (dtFooter.Rows.Count > 0)
            //{
            //    ddlSGVAddTask.DataSource = dtFooter;
            //    ddlSGVAddTask.DataTextField = "TL_Task";
            //    ddlSGVAddTask.DataValueField = "TL_ID";
            //    ddlSGVAddTask.DataBind();
            //    ddlSGVAddTask.Items.Insert(0, "--Select Task--");
            //    ddlSGVAddTask.SelectedIndex = 0;

            //    ddlSGVAddTask.Enabled = false;

            //}
        }
    }
    protected void gvRecordsTue_RowEditing(object sender, GridViewEditEventArgs e)
    {
        
        gvRecordsTue.EditIndex = e.NewEditIndex;
        dt = DisplayRecords(lblTuesday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsTue.DataSource = dt;
            gvRecordsTue.DataBind();
        }
    }
    protected void gvRecordsTue_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecordsTue.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecordsTue.Rows[e.RowIndex].FindControl("ddlTGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecordsTue.Rows[e.RowIndex].FindControl("ddlTGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecordsTue.Rows[e.RowIndex].FindControl("ddlTGvSubTask");
            TextBox txtComments = (TextBox)gvRecordsTue.Rows[e.RowIndex].FindControl("txtTComments");
            TextBox txtDuration = (TextBox)gvRecordsTue.Rows[e.RowIndex].FindControl("txtTDuration");

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

            //if (regex.IsMatch(txtDuration.Text) == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration6", "alert('Please enter valid duration in HH:MM:SS format')", true);
            //    return;
            //}

            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid),
               new SqlParameter("@duration", addDuration),
               new SqlParameter("@system", "P")
           };

            sQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Duration=@duration, R_Comments=@comments, R_System=@system where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            gvRecordsTue.EditIndex = -1;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            dt = DisplayRecords(lblTuesday.Text, ddlEmployee.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                gvRecordsTue.DataSource = dt;
                gvRecordsTue.DataBind();
                ViewState["dirTue"] = dt;
                ViewState["sortdrT"] = "Asc";
            }

            BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        }
        catch (Exception ex)
        {
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
            //gvRecords.EditIndex = -1;
            //DisplayRecords();
        }
    }
    protected void gvRecordsTue_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecordsTue.EditIndex = -1;
        dt = DisplayRecords(lblTuesday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsTue.DataSource = dt;
            gvRecordsTue.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsTue_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int rowid = Convert.ToInt32(gvRecordsTue.DataKeys[e.RowIndex].Value.ToString());
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblTuesday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsTue.DataSource = dt;
            gvRecordsTue.DataBind();
            ViewState["dirTue"] = dt;
            ViewState["sortdrT"] = "Asc";
        }
        else
        {
            gvRecordsTue.DataSource = null;
            gvRecordsTue.DataBind();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlTGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlTGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlTGvSubTask");
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlTGvClient");

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

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    //Wednesday

    protected void gvRecordsWed_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsWed.EditIndex == e.Row.RowIndex)
        {
            dt = new DataTable();
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlWGvClient");
            Label lblClient = (Label)e.Row.FindControl("lblWEditClient");


            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id", ddlTeam.SelectedValue),
           };

            sQuery = "select CL_ID, CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName !='Inclement Weather' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            //dt = GetClientsWithoutPTOs(Convert.ToInt32(ddlTeam.SelectedValue));

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
            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlWGvTask");
            Label lblTask = (Label)e.Row.FindControl("lblWEditTask");

            if (lblClient.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
          
            //dt = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            
            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
                if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                {
                    ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                }
            }
            dt = new DataTable();

            DropDownList ddlGvSubTask = (DropDownList)e.Row.FindControl("ddlWGvSubTask");
            Label lblEditSubtask = (Label)e.Row.FindControl("lblWEditSubtask");


            SqlParameter[] parameters2 = new SqlParameter[]
           {           
               new SqlParameter("@task",ddlGvTask.SelectedValue),
           };

            sQuery = "select STL_ID, STL_SubTask from RTM_SubTask_List WITH (NOLOCK) where STL_Task_Id =@task and STL_ViewStatus=1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

            //dt = GetSubTasks(Convert.ToInt32(ddlTeam.SelectedValue), Convert.ToInt32(ddlGvTask.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvSubTask.DataSource = dt;
                ddlGvSubTask.DataTextField = "STL_SubTask";
                ddlGvSubTask.DataValueField = "STL_ID";
                ddlGvSubTask.DataBind();
                if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                {
                    ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsWed.EditIndex != e.Row.RowIndex)
        {
            Label ddlGvClient = (Label)e.Row.FindControl("lblWClient");
            if (ddlGvClient.Text == "Personal/Sick Time" || ddlGvClient.Text == "Inclement Weather" || ddlGvClient.Text == "Public Holiday" || ddlGvClient.Text == "Vacation" || ddlGvClient.Text == "Bereavement" || ddlGvClient.Text == "Jury Duty" || ddlGvClient.Text == "Maternity Leave" || ddlGvClient.Text == "Paternity Leave" || ddlGvClient.Text == "Medical Leave" || ddlGvClient.Text == "Comp off")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }

            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            string myDataKey = rowView["R_ID"].ToString();
            if (myDataKey == "0")
                e.Row.Visible = false;
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSADDUser = (Label)e.Row.FindControl("lblWADDUser");
            lblSADDUser.Text = ddlEmployee.SelectedItem.Text;
            //DropDownList ddlSGVAddTask = (DropDownList)e.Row.FindControl("ddlWAddTask");
           
            //dtFooter = new DataTable();
            //dtFooter = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            
            //if (dtFooter.Rows.Count > 0)
            //{
            //    ddlSGVAddTask.DataSource = dtFooter;
            //    ddlSGVAddTask.DataTextField = "TL_Task";
            //    ddlSGVAddTask.DataValueField = "TL_ID";
            //    ddlSGVAddTask.DataBind();
            //    ddlSGVAddTask.Items.Insert(0, "--Select Task--");
            //    ddlSGVAddTask.SelectedIndex = 0;

            //    ddlSGVAddTask.Enabled = false;

            //}
        }
    }
    protected void gvRecordsWed_RowEditing(object sender, GridViewEditEventArgs e)
    {
       
        gvRecordsWed.EditIndex = e.NewEditIndex;
        dt = DisplayRecords(lblWed.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsWed.DataSource = dt;
            gvRecordsWed.DataBind();
        }
    }
    protected void gvRecordsWed_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecordsWed.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecordsWed.Rows[e.RowIndex].FindControl("ddlWGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecordsWed.Rows[e.RowIndex].FindControl("ddlWGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecordsWed.Rows[e.RowIndex].FindControl("ddlWGvSubTask");
            TextBox txtComments = (TextBox)gvRecordsWed.Rows[e.RowIndex].FindControl("txtWComments");
            TextBox txtDuration = (TextBox)gvRecordsWed.Rows[e.RowIndex].FindControl("txtWDuration");

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

            //if (regex.IsMatch(txtDuration.Text) == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration7", "alert('Please enter valid duration in HH:MM:SS format')", true);
            //    return;
            //}

            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid),
               new SqlParameter("@duration", addDuration)
           };

            sQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Duration=@duration, R_Comments=@comments, R_System= 'P' where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            gvRecordsWed.EditIndex = -1;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            dt = DisplayRecords(lblWed.Text, ddlEmployee.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                gvRecordsWed.DataSource = dt;
                gvRecordsWed.DataBind();
                ViewState["dirWed"] = dt;
                ViewState["sortdrW"] = "Asc";
            }

            BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        }
        catch (Exception ex)
        {
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
            //gvRecords.EditIndex = -1;
            //DisplayRecords();
        }
    }
    protected void gvRecordsWed_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecordsWed.EditIndex = -1;
        dt = DisplayRecords(lblWed.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsWed.DataSource = dt;
            gvRecordsWed.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsWed_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int rowid = Convert.ToInt32(gvRecordsWed.DataKeys[e.RowIndex].Value.ToString());
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblWed.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsWed.DataSource = dt;
            gvRecordsWed.DataBind();
            ViewState["dirWed"] = dt;
            ViewState["sortdrW"] = "Asc";
        }
        else
        {
            gvRecordsWed.DataSource = null;
            gvRecordsWed.DataBind();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlWGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlWGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlWGvSubTask");
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlWGvClient");

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

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    //Thursday

    protected void gvRecordsThu_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsThu.EditIndex == e.Row.RowIndex)
        {
            dt = new DataTable();
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlTHGvClient");
            Label lblClient = (Label)e.Row.FindControl("lblTHEditClient");


            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id", ddlTeam.SelectedValue),
           };

            sQuery = "select CL_ID, CL_ClientName from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName != 'Inclement Weather' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            //dt = GetClientsWithoutPTOs(Convert.ToInt32(ddlTeam.SelectedValue));

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
            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlTHGvTask");
            Label lblTask = (Label)e.Row.FindControl("lblTHEditTask");

            if (lblClient.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }

            //dt = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));            

            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
                if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                {
                    ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                }
            }
            dt = new DataTable();

            DropDownList ddlGvSubTask = (DropDownList)e.Row.FindControl("ddlTHGvSubTask");
            Label lblEditSubtask = (Label)e.Row.FindControl("lblTHEditSubtask");


            SqlParameter[] parameters2 = new SqlParameter[]
           {           
               new SqlParameter("@task",ddlGvTask.SelectedValue),
           };

            sQuery = "select STL_ID, STL_SubTask from RTM_SubTask_List WITH (NOLOCK) where STL_Task_Id =@task and STL_ViewStatus=1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

            //dt = GetSubTasks(Convert.ToInt32(ddlTeam.SelectedValue), Convert.ToInt32(ddlGvTask.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvSubTask.DataSource = dt;
                ddlGvSubTask.DataTextField = "STL_SubTask";
                ddlGvSubTask.DataValueField = "STL_ID";
                ddlGvSubTask.DataBind();
                if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                {
                    ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsThu.EditIndex != e.Row.RowIndex)
        {
            Label ddlGvClient = (Label)e.Row.FindControl("lblTHClient");
            if (ddlGvClient.Text == "Personal/Sick Time" || ddlGvClient.Text == "Inclement Weather" || ddlGvClient.Text == "Public Holiday" || ddlGvClient.Text == "Vacation" || ddlGvClient.Text == "Bereavement" || ddlGvClient.Text == "Jury Duty" || ddlGvClient.Text == "Maternity Leave" || ddlGvClient.Text == "Paternity Leave" || ddlGvClient.Text == "Medical Leave" || ddlGvClient.Text == "Comp off")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }

            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            string myDataKey = rowView["R_ID"].ToString();
            if (myDataKey == "0")
                e.Row.Visible = false;
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSADDUser = (Label)e.Row.FindControl("lblTHADDUser");
            lblSADDUser.Text = ddlEmployee.SelectedItem.Text;
            //DropDownList ddlSGVAddTask = (DropDownList)e.Row.FindControl("ddlTHAddTask");
           
            //dtFooter = new DataTable();
            //dtFooter = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            
            //if (dtFooter.Rows.Count > 0)
            //{
            //    ddlSGVAddTask.DataSource = dtFooter;
            //    ddlSGVAddTask.DataTextField = "TL_Task";
            //    ddlSGVAddTask.DataValueField = "TL_ID";
            //    ddlSGVAddTask.DataBind();
            //    ddlSGVAddTask.Items.Insert(0, "--Select Task--");
            //    ddlSGVAddTask.SelectedIndex = 0;

            //    ddlSGVAddTask.Enabled = false;

            //}
        }
    }
    protected void gvRecordsThu_RowEditing(object sender, GridViewEditEventArgs e)
    {
        
        gvRecordsThu.EditIndex = e.NewEditIndex;
        dt = DisplayRecords(lblThu.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsThu.DataSource = dt;
            gvRecordsThu.DataBind();
        }
    }
    protected void gvRecordsThu_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecordsThu.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecordsThu.Rows[e.RowIndex].FindControl("ddlTHGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecordsThu.Rows[e.RowIndex].FindControl("ddlTHGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecordsThu.Rows[e.RowIndex].FindControl("ddlTHGvSubTask");
            TextBox txtComments = (TextBox)gvRecordsThu.Rows[e.RowIndex].FindControl("txtTHComments");
            TextBox txtDuration = (TextBox)gvRecordsThu.Rows[e.RowIndex].FindControl("txtTHDuration");

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

            //if (regex.IsMatch(txtDuration.Text) == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration5", "alert('Please enter valid duration in HH:MM:SS format')", true);
            //    return;
            //}

            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid),
               new SqlParameter("@duration", addDuration)
           };

            sQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Duration = @duration, R_Comments=@comments, R_System= 'P' where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            gvRecordsThu.EditIndex = -1;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            dt = DisplayRecords(lblThu.Text, ddlEmployee.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                gvRecordsThu.DataSource = dt;
                gvRecordsThu.DataBind();
                ViewState["dirThu"] = dt;
                ViewState["sortdrTH"] = "Asc";
            }

            BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        }
        catch (Exception ex)
        {
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
            //gvRecords.EditIndex = -1;
            //DisplayRecords();
        }
    }
    protected void gvRecordsThu_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecordsThu.EditIndex = -1;
        dt = DisplayRecords(lblThu.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsThu.DataSource = dt;
            gvRecordsThu.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsThu_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int rowid = Convert.ToInt32(gvRecordsThu.DataKeys[e.RowIndex].Value.ToString());
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblThu.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsThu.DataSource = dt;
            gvRecordsThu.DataBind();
            ViewState["dirThu"] = dt;
            ViewState["sortdrTH"] = "Asc";
        }
        else
        {
            gvRecordsThu.DataSource = null;
            gvRecordsThu.DataBind();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlTHGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlTHGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlTHGvSubTask");
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlTHGvClient");

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

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    //Friday

    protected void gvRecordsFri_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsFri.EditIndex == e.Row.RowIndex)
        {
            dt = new DataTable();
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlFGvClient");
            Label lblClient = (Label)e.Row.FindControl("lblFEditClient");


            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id", ddlTeam.SelectedValue),
           };

            sQuery = "select * from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName !='Inclement Weather' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            //dt = GetClientsWithoutPTOs(Convert.ToInt32(ddlTeam.SelectedValue));

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
            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlFGvTask");
            Label lblTask = (Label)e.Row.FindControl("lblFEditTask");

            if (lblClient.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }

            //dt = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            
            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
                if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                {
                    ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                }
            }
            dt = new DataTable();

            DropDownList ddlGvSubTask = (DropDownList)e.Row.FindControl("ddlFGvSubTask");
            Label lblEditSubtask = (Label)e.Row.FindControl("lblFEditSubtask");


            SqlParameter[] parameters2 = new SqlParameter[]
           {           
               new SqlParameter("@task",ddlGvTask.SelectedValue),
           };

            sQuery = "select STL_ID, STL_SubTask from RTM_SubTask_List WITH (NOLOCK) where STL_Task_Id =@task and STL_ViewStatus=1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

            //dt = GetSubTasks(Convert.ToInt32(ddlTeam.SelectedValue), Convert.ToInt32(ddlGvTask.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvSubTask.DataSource = dt;
                ddlGvSubTask.DataTextField = "STL_SubTask";
                ddlGvSubTask.DataValueField = "STL_ID";
                ddlGvSubTask.DataBind();
                if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                {
                    ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsFri.EditIndex != e.Row.RowIndex)
        {
            Label ddlGvClient = (Label)e.Row.FindControl("lblFClient");
            if (ddlGvClient.Text == "Personal/Sick Time" || ddlGvClient.Text == "Inclement Weather" || ddlGvClient.Text == "Public Holiday" || ddlGvClient.Text == "Vacation" || ddlGvClient.Text == "Bereavement" || ddlGvClient.Text == "Jury Duty" || ddlGvClient.Text == "Maternity Leave" || ddlGvClient.Text == "Paternity Leave" || ddlGvClient.Text == "Medical Leave" || ddlGvClient.Text == "Comp off")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }
        }


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            string myDataKey = rowView["R_ID"].ToString();
            
            if (myDataKey == "0")
            {
                e.Row.Visible = false;
            }
           
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSADDUser = (Label)e.Row.FindControl("lblFADDUser");
            lblSADDUser.Text = ddlEmployee.SelectedItem.Text;
            //DropDownList ddlSGVAddTask = (DropDownList)e.Row.FindControl("ddlFAddTask");
          
            //dtFooter = new DataTable();
            //dtFooter = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));            

            //if (dtFooter.Rows.Count > 0)
            //{
            //    ddlSGVAddTask.DataSource = dtFooter;
            //    ddlSGVAddTask.DataTextField = "TL_Task";
            //    ddlSGVAddTask.DataValueField = "TL_ID";
            //    ddlSGVAddTask.DataBind();
            //    ddlSGVAddTask.Items.Insert(0, "--Select Task--");
            //    ddlSGVAddTask.SelectedIndex = 0;

            //    ddlSGVAddTask.Enabled = false;

            //}
        }
    }
    protected void gvRecordsFri_RowEditing(object sender, GridViewEditEventArgs e)
    {
        
        gvRecordsFri.EditIndex = e.NewEditIndex;
        dt = DisplayRecords(lblFriday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsFri.DataSource = dt;
            gvRecordsFri.DataBind();
        }
    }
    protected void gvRecordsFri_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecordsFri.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecordsFri.Rows[e.RowIndex].FindControl("ddlFGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecordsFri.Rows[e.RowIndex].FindControl("ddlFGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecordsFri.Rows[e.RowIndex].FindControl("ddlFGvSubTask");
            TextBox txtComments = (TextBox)gvRecordsFri.Rows[e.RowIndex].FindControl("txtFComments");
            TextBox txtDuration = (TextBox)gvRecordsFri.Rows[e.RowIndex].FindControl("txtFDuration");

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

            //if (regex.IsMatch(txtDuration.Text) == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration1", "alert('Please enter valid duration in HH:MM:SS format')", true);
            //    return;
               
            //}

            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid),
               new SqlParameter("@duration", addDuration)
           };

            sQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Duration = @duration, R_Comments=@comments, R_System= 'P' where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            gvRecordsFri.EditIndex = -1;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            dt = DisplayRecords(lblFriday.Text, ddlEmployee.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                gvRecordsFri.DataSource = dt;
                gvRecordsFri.DataBind();
                ViewState["dirFri"] = dt;
                ViewState["sortdrF"] = "Asc";
            }

            BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        }
        catch (Exception ex)
        {
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
            //gvRecords.EditIndex = -1;
            //DisplayRecords();
        }
    }
    protected void gvRecordsFri_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecordsFri.EditIndex = -1;
        dt = DisplayRecords(lblFriday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsFri.DataSource = dt;
            gvRecordsFri.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsFri_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int rowid = Convert.ToInt32(gvRecordsFri.DataKeys[e.RowIndex].Value.ToString());

        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblFriday.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsFri.DataSource = dt;
            gvRecordsFri.DataBind();
            ViewState["dirFri"] = dt;
            ViewState["sortdrF"] = "Asc";
        }
        else
        {
            gvRecordsFri.DataSource = null;
            gvRecordsFri.DataBind();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlFGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlFGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlFGvSubTask");
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlFGvClient");

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

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    //Saturday

    protected void gvRecordsSat_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsSat.EditIndex == e.Row.RowIndex)
        {
            dt = new DataTable();
            DropDownList ddlGvClient = (DropDownList)e.Row.FindControl("ddlSAGvClient");
            Label lblClient = (Label)e.Row.FindControl("lblSAEditClient");


            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@id", ddlTeam.SelectedValue),
           };

            sQuery = "select * from RTM_Client_List WITH (NOLOCK) where CL_TeamId= @id and CL_Status = 1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName !='Inclement Weather' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            //dt = GetClientsWithoutPTOs(Convert.ToInt32(ddlTeam.SelectedValue));

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
            DropDownList ddlGvTask = (DropDownList)e.Row.FindControl("ddlSAGvTask");
            Label lblTask = (Label)e.Row.FindControl("lblSAEditTask");

            if (lblClient.Text == "Internal")
            {
                dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
            else
            {
                dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            }
           
            //dt = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));
            

            if (dt.Rows.Count > 0)
            {
                ddlGvTask.DataSource = dt;
                ddlGvTask.DataTextField = "TL_Task";
                ddlGvTask.DataValueField = "TL_ID";
                ddlGvTask.DataBind();
                if (ddlGvTask.Items.FindByText(lblTask.Text) != null)
                {
                    ddlGvTask.Items.FindByText(lblTask.Text).Selected = true;
                }
            }
            dt = new DataTable();

            DropDownList ddlGvSubTask = (DropDownList)e.Row.FindControl("ddlSAGvSubTask");
            Label lblEditSubtask = (Label)e.Row.FindControl("lblSAEditSubtask");


            SqlParameter[] parameters2 = new SqlParameter[]
           {           
               new SqlParameter("@task",ddlGvTask.SelectedValue),
           };

            sQuery = "select STL_ID, STL_SubTask from RTM_SubTask_List WITH (NOLOCK) where STL_Task_Id =@task and STL_ViewStatus=1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

            //dt = GetSubTasks(Convert.ToInt32(ddlTeam.SelectedValue), Convert.ToInt32(ddlGvTask.SelectedValue));

            if (dt.Rows.Count > 0)
            {
                ddlGvSubTask.DataSource = dt;
                ddlGvSubTask.DataTextField = "STL_SubTask";
                ddlGvSubTask.DataValueField = "STL_ID";
                ddlGvSubTask.DataBind();
                if (ddlGvSubTask.Items.FindByText(lblEditSubtask.Text) != null)
                {
                    ddlGvSubTask.Items.FindByText(lblEditSubtask.Text).Selected = true;
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow && gvRecordsSat.EditIndex != e.Row.RowIndex)
        {
            Label ddlGvClient = (Label)e.Row.FindControl("lblSAClient");
            if (ddlGvClient.Text == "Personal/Sick Time" || ddlGvClient.Text == "Inclement Weather" || ddlGvClient.Text == "Public Holiday" || ddlGvClient.Text == "Vacation" || ddlGvClient.Text == "Bereavement" || ddlGvClient.Text == "Jury Duty" || ddlGvClient.Text == "Maternity Leave" || ddlGvClient.Text == "Paternity Leave" || ddlGvClient.Text == "Medical Leave" || ddlGvClient.Text == "Comp off")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }

            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                DataControlFieldCell editable = (DataControlFieldCell)e.Row.Controls[6];
                editable.Enabled = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            string myDataKey = rowView["R_ID"].ToString();
            if (myDataKey == "0")
                e.Row.Visible = false;
        }



        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSADDUser = (Label)e.Row.FindControl("lblSAADDUser");
            lblSADDUser.Text = ddlEmployee.SelectedItem.Text;
           // DropDownList ddlSGVAddTask = (DropDownList)e.Row.FindControl("ddlSAAddTask");
           
           //dtFooter = new DataTable();
           //dtFooter = objRec.GetTasks(Convert.ToInt32(ddlTeam.SelectedValue));           

           // if (dtFooter.Rows.Count > 0)
           // {
           //     ddlSGVAddTask.DataSource = dtFooter;
           //     ddlSGVAddTask.DataTextField = "TL_Task";
           //     ddlSGVAddTask.DataValueField = "TL_ID";
           //     ddlSGVAddTask.DataBind();
           //     ddlSGVAddTask.Items.Insert(0, "--Select Task--");
           //     ddlSGVAddTask.SelectedIndex = 0;

           //     ddlSGVAddTask.Enabled = false;

           // }
        }
    }
    protected void gvRecordsSat_RowEditing(object sender, GridViewEditEventArgs e)
    {
       
        gvRecordsSat.EditIndex = e.NewEditIndex;
        dt = DisplayRecords(lblSat.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsSat.DataSource = dt;
            gvRecordsSat.DataBind();
        }
    }
    protected void gvRecordsSat_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int rowid = Convert.ToInt32(gvRecordsSat.DataKeys[e.RowIndex].Value.ToString());
            DropDownList ddlGvClient = (DropDownList)gvRecordsSat.Rows[e.RowIndex].FindControl("ddlSAGvClient");
            DropDownList ddlGvTask = (DropDownList)gvRecordsSat.Rows[e.RowIndex].FindControl("ddlSAGvTask");
            DropDownList ddlGvSubTask = (DropDownList)gvRecordsSat.Rows[e.RowIndex].FindControl("ddlSAGvSubTask");
            TextBox txtComments = (TextBox)gvRecordsSat.Rows[e.RowIndex].FindControl("txtSAComments");
            TextBox txtDuration = (TextBox)gvRecordsSat.Rows[e.RowIndex].FindControl("txtSADuration");

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

            //if (regex.IsMatch(txtDuration.Text) == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration3", "alert('Please enter valid duration in HH:MM:SS format')", true);
            //    return;
            //}

            SqlParameter[] parameters = new SqlParameter[]
           {           
               new SqlParameter("@client",ddlGvClient.SelectedValue),
               new SqlParameter("@task",ddlGvTask.SelectedValue),
               new SqlParameter("@subTask",ddlGvSubTask.SelectedValue ),
               new SqlParameter("@comments",txtComments.Text),
               new SqlParameter("@rowid",rowid),
               new SqlParameter("@duration", addDuration)
           };

            sQuery = "UPDATE RTM_Records SET R_Client =@client, R_Task=@task, R_SubTask =@subTask, R_Duration=@duration, R_Comments=@comments, R_System= 'P' where R_ID=@rowid";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            gvRecordsSat.EditIndex = -1;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update Record", "alert('Updated Successfully')", true);
            dt = DisplayRecords(lblSat.Text, ddlEmployee.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                gvRecordsSat.DataSource = dt;
                gvRecordsSat.DataBind();
                ViewState["dirSat"] = dt;
                ViewState["sortdrSA"] = "Asc";
            }

            BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        }
        catch (Exception ex)
        {
            //lblError.Text = ex.Message;
            //lblError.ForeColor = Color.Red;
            //gvRecords.EditIndex = -1;
            //DisplayRecords();
        }
    }
    protected void gvRecordsSat_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvRecordsSat.EditIndex = -1;
        dt = DisplayRecords(lblSat.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsSat.DataSource = dt;
            gvRecordsSat.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsSat_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int rowid = Convert.ToInt32(gvRecordsSat.DataKeys[e.RowIndex].Value.ToString());
        result = objRec.DeleteRecord(rowid.ToString());

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Deleted", "alert('Deleted Successfully')", true);
        dt = DisplayRecords(lblSat.Text, ddlEmployee.SelectedItem.Text);
        if (dt.Rows.Count > 0)
        {
            gvRecordsSat.DataSource = dt;
            gvRecordsSat.DataBind();
            ViewState["dirSat"] = dt;
            ViewState["sortdrSA"] = "Asc";
        }
        else
        {
            gvRecordsSat.DataSource = null;
            gvRecordsSat.DataBind();
        }

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlSAGvTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlSubTask = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlSubTask.NamingContainer;
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlSAGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlSAGvSubTask");
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlSAGvClient");

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

        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }
    protected void btnAddSun_Click(object sender, EventArgs e)
    {
        lblTeamId.Text = ddlTeam.SelectedValue;
        lblEmpID.Text = ddlEmployee.SelectedValue;
        lblUserName.Text = ddlEmployee.SelectedItem.Text;
        lblStartTime.Text = lblSunday.Text;
        lblEndTime.Text = lblSunday.Text;

        lblDay.Text = "Sun";
        
        LoadClients(Convert.ToInt32(lblTeamId.Text));
        LoadTasks(Convert.ToInt32(lblTeamId.Text));
        mpePopUp.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnAddMon_Click(object sender, EventArgs e)
    {
        lblTeamId.Text = ddlTeam.SelectedValue;
        lblEmpID.Text = ddlEmployee.SelectedValue;
        lblUserName.Text = ddlEmployee.SelectedItem.Text;
        lblStartTime.Text = lblMonday.Text;
        lblEndTime.Text = lblMonday.Text;

        lblDay.Text = "Mon";

        LoadClients(Convert.ToInt32(lblTeamId.Text));
        LoadTasks(Convert.ToInt32(lblTeamId.Text));
        mpePopUp.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnAddTue_Click(object sender, EventArgs e)
    {
        lblTeamId.Text = ddlTeam.SelectedValue;
        lblEmpID.Text = ddlEmployee.SelectedValue;
        lblUserName.Text = ddlEmployee.SelectedItem.Text;
        lblStartTime.Text = lblTuesday.Text;
        lblEndTime.Text = lblTuesday.Text;

        lblDay.Text = "Tue";

        LoadClients(Convert.ToInt32(lblTeamId.Text));
        LoadTasks(Convert.ToInt32(lblTeamId.Text));
        mpePopUp.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnAddWed_Click(object sender, EventArgs e)
    {
        lblTeamId.Text = ddlTeam.SelectedValue;
        lblEmpID.Text = ddlEmployee.SelectedValue;
        lblUserName.Text = ddlEmployee.SelectedItem.Text;
        lblStartTime.Text = lblWed.Text;
        lblEndTime.Text = lblWed.Text;

        lblDay.Text = "Wed";

        LoadClients(Convert.ToInt32(lblTeamId.Text));
        LoadTasks(Convert.ToInt32(lblTeamId.Text));
        mpePopUp.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnAddThu_Click(object sender, EventArgs e)
    {
        lblTeamId.Text = ddlTeam.SelectedValue;
        lblEmpID.Text = ddlEmployee.SelectedValue;
        lblUserName.Text = ddlEmployee.SelectedItem.Text;
        lblStartTime.Text = lblThu.Text;
        lblEndTime.Text = lblThu.Text;

        lblDay.Text = "Thu";

        LoadClients(Convert.ToInt32(lblTeamId.Text));
        LoadTasks(Convert.ToInt32(lblTeamId.Text));
        mpePopUp.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnAddFri_Click(object sender, EventArgs e)
    {
        lblTeamId.Text = ddlTeam.SelectedValue;
        lblEmpID.Text = ddlEmployee.SelectedValue;
        lblUserName.Text = ddlEmployee.SelectedItem.Text;
        lblStartTime.Text = lblFriday.Text;
        lblEndTime.Text = lblFriday.Text;

        lblDay.Text = "Fri";

        LoadClients(Convert.ToInt32(lblTeamId.Text));
        LoadTasks(Convert.ToInt32(lblTeamId.Text));
        mpePopUp.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnAddSat_Click(object sender, EventArgs e)
    {
        lblTeamId.Text = ddlTeam.SelectedValue;
        lblEmpID.Text = ddlEmployee.SelectedValue;
        lblUserName.Text = ddlEmployee.SelectedItem.Text;
        lblStartTime.Text = lblSat.Text;
        lblEndTime.Text = lblSat.Text;

        lblDay.Text = "Sat";

        LoadClients(Convert.ToInt32(lblTeamId.Text));
        LoadTasks(Convert.ToInt32(lblTeamId.Text));
        mpePopUp.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlNewTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlNewTask.SelectedIndex != 0)
        {
            LoadSubTasks();
            mpePopUp.Show();
        }
    }

    private void LoadSubTasks()
    {
        SqlParameter[] parameters = new SqlParameter[]
        { 
            new SqlParameter("@newtask", ddlNewTask.SelectedValue ),
        };

        dt = new DataTable();

        sQuery = "select * from RTM_SubTask_List WITH (NOLOCK) where STL_Task_Id=@newtask and STL_ViewStatus=1";
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

        //dt = GetSubTasks(Convert.ToInt32(ddlTeam.SelectedValue), Convert.ToInt32(ddlNewTask.SelectedValue));

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

    private void LoadTasks(int TID)
    {
        //SqlParameter[] parameters = new SqlParameter[]
        //{ 
        //    new SqlParameter("@TID",TID),
        //};

        dt = new DataTable();

        //sQuery = "select * from RTM_Task_List WITH (NOLOCK) where TL_TeamId =@TID and TL_Status =1 order By TL_Task";
        //dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        dt = objRec.GetTasks(TID);
        //dt = GetTasks(TID);

        if (dt.Rows.Count > 0)
        {
            ddlNewTask.DataSource = dt;
            ddlNewTask.DataTextField = "TL_Task";
            ddlNewTask.DataValueField = "TL_ID";
            ddlNewTask.DataBind();
            ddlNewTask.Items.Insert(0, "--Select--");
            ddlNewTask.SelectedIndex = 0;
        }
    }

    private void LoadClients(int TID)
    {
        dt = new DataTable();       

        dt = objRec.GetClients(TID);

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

    protected void btnNo_Click(object sender, EventArgs e)
    {
        txtDuration.Text = string.Empty;
        txtComments.Text = string.Empty;
        lblPopError.Text = string.Empty;
        lblDay.Text = string.Empty;
        mpePopUp.Hide();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        try
        {
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
            else if (txtDuration.Text.Length < 8)
            {
                lblPopError.Text = "Please enter time in HH:MM:SS format";
                lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else if (regex.IsMatch(txtDuration.Text) == false)
            {
                lblPopError.Text = "Please enter valid time in HH:MM:SS format";
                lblPopError.ForeColor = Color.Red;
                mpePopUp.Show();
            }
            else
            {
                SqlParameter[] parameters = new SqlParameter[]
                        {  
                            new SqlParameter("@teamId",lblTeamId.Text),
                            new SqlParameter("@empId",lblEmpID.Text),
                            new SqlParameter("@username",lblUserName.Text),
                            new SqlParameter("@newClient",ddlNewClient.SelectedValue),
                            new SqlParameter("@newTask",ddlNewTask.SelectedValue),
                            new SqlParameter("@subTask", ddlNewSubTask.SelectedValue),
                            new SqlParameter("@comm",txtComments.Text),
                            new SqlParameter("@duration",txtDuration.Text),
                            new SqlParameter("@startTime",lblStartTime.Text),
                            new SqlParameter("@endTime",lblEndTime.Text),
                            new SqlParameter("@status", "Completed"),
                            new SqlParameter("@system", "P")
                       };
                sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn,R_Status, R_System) values (@teamId,@empId, @username, @newClient, @newTask, @subTask, @comm,@duration, @startTime, @endTime, @status, @system)";
                result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                ddlNewClient.SelectedIndex = 0;
                ddlNewTask.SelectedIndex = 0;
                ddlNewSubTask.SelectedIndex = 0;
                txtDuration.Text = string.Empty;
                txtComments.Text = string.Empty;
                lblPopError.Text = string.Empty;
                
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Saved Successfully')", true);
                if (lblDay.Text == "Sun")
                {
                    dt = DisplayRecords(lblStartTime.Text, lblUserName.Text);
                    if (dt.Rows.Count > 0)
                    {
                        gvRecordsSunday.DataSource = dt;
                        gvRecordsSunday.DataBind();
                        ViewState["dirSun"] = dt;
                        ViewState["sortdrS"] = "Asc";
                    }
                    lblDay.Text = string.Empty;
                }
                if (lblDay.Text == "Mon")
                {
                    dt = DisplayRecords(lblStartTime.Text, lblUserName.Text);
                    if (dt.Rows.Count > 0)
                    {
                        gvRecordsMonday.DataSource = dt;
                        gvRecordsMonday.DataBind();
                        ViewState["dirMon"] = dt;
                        ViewState["sortdr"] = "Asc";
                    }
                    lblDay.Text = string.Empty;
                }
                if (lblDay.Text == "Tue")
                {
                    dt = DisplayRecords(lblStartTime.Text, lblUserName.Text);
                    if (dt.Rows.Count > 0)
                    {
                        gvRecordsTue.DataSource = dt;
                        gvRecordsTue.DataBind();
                        ViewState["dirTue"] = dt;
                        ViewState["sortdrT"] = "Asc";
                    }
                    lblDay.Text = string.Empty;
                }
                if (lblDay.Text == "Wed")
                {
                    dt = DisplayRecords(lblStartTime.Text, lblUserName.Text);
                    if (dt.Rows.Count > 0)
                    {
                        gvRecordsWed.DataSource = dt;
                        gvRecordsWed.DataBind();
                        ViewState["dirWed"] = dt;
                        ViewState["sortdrW"] = "Asc";
                    }
                    lblDay.Text = string.Empty;
                }
                if (lblDay.Text == "Thu")
                {
                    dt = DisplayRecords(lblStartTime.Text, lblUserName.Text);
                    if (dt.Rows.Count > 0)
                    {
                        gvRecordsThu.DataSource = dt;
                        gvRecordsThu.DataBind();
                        ViewState["dirThu"] = dt;
                        ViewState["sortdrTH"] = "Asc";
                    }
                    lblDay.Text = string.Empty;
                }
                if (lblDay.Text == "Fri")
                {
                    dt = DisplayRecords(lblStartTime.Text, lblUserName.Text);
                    if (dt.Rows.Count > 0)
                    {
                        gvRecordsFri.DataSource = dt;
                        gvRecordsFri.DataBind();
                        ViewState["dirFri"] = dt;
                        ViewState["sortdrF"] = "Asc";
                    }
                    lblDay.Text = string.Empty;
                }
                if (lblDay.Text == "Sat")
                {
                    dt = DisplayRecords(lblStartTime.Text, lblUserName.Text);
                    if (dt.Rows.Count > 0)
                    {
                        gvRecordsSat.DataSource = dt;
                        gvRecordsSat.DataBind();
                        ViewState["dirSat"] = dt;
                        ViewState["sortdrSA"] = "Asc";
                    }
                    lblDay.Text = string.Empty;
                }
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
            }
        }
        catch (Exception ex)
        {
            
        }
    }
    protected void btnCopySun_Click(object sender, EventArgs e)
    {
        lblCopyFrom.Text = lblSunday.Text;
        lblDay.Text = "Sun";
        mpeDate.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnCopyMon_Click(object sender, EventArgs e)
    {
        lblCopyFrom.Text = lblMonday.Text;
        lblDay.Text = "Mon";
        mpeDate.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnCopyTue_Click(object sender, EventArgs e)
    {
        lblCopyFrom.Text = lblTuesday.Text;
        lblDay.Text = "Tue";
        mpeDate.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnCopyWed_Click(object sender, EventArgs e)
    {
        lblCopyFrom.Text = lblWed.Text;
        lblDay.Text = "Wed";
        mpeDate.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnCopyThu_Click(object sender, EventArgs e)
    {
        lblCopyFrom.Text = lblThu.Text;
        lblDay.Text = "Thu";
        mpeDate.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnCopyFri_Click(object sender, EventArgs e)
    {
        lblCopyFrom.Text = lblFriday.Text;
        lblDay.Text = "Fri";
        mpeDate.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnCopySat_Click(object sender, EventArgs e)
    {
        lblCopyFrom.Text = lblSat.Text;
        lblDay.Text = "Sat";
        mpeDate.Show();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnDateYes_Click(object sender, EventArgs e)
    {
        dt = objRec.GetData(txtSelDate.Text, ddlEmployee.SelectedValue);
        if (dt.Rows.Count > 0)
        {
            lblstatus.Text = "day";
            lblOn.Text = "on";
            lblCopyto.Text = txtSelDate.Text;
            mpeConfirm.Show();
        }
        else
        {
            dt = objRec.GetData(lblCopyFrom.Text, ddlEmployee.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    SqlParameter[] parameters = new SqlParameter[]
                        {  
                            new SqlParameter("@teamId",drRow["R_TeamId"].ToString()),
                            new SqlParameter("@empId",drRow["R_Employee_Id"].ToString()),
                            new SqlParameter("@username",drRow["R_User_Name"].ToString()),
                            new SqlParameter("@newClient",drRow["R_Client"].ToString()),
                            new SqlParameter("@newTask",drRow["R_Task"].ToString()),
                            new SqlParameter("@subTask", drRow["R_SubTask"].ToString()),
                            new SqlParameter("@comm",drRow["R_Comments"].ToString()),
                            new SqlParameter("@duration",drRow["R_Duration"].ToString()),
                            new SqlParameter("@startTime",txtSelDate.Text),
                            new SqlParameter("@endTime",txtSelDate.Text),
                            new SqlParameter("@status", "Completed"),
                            new SqlParameter("@system", "P")
                       };
                    sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn,R_Status, R_System, R_TimeDate) values (@teamId,@empId, @username, @newClient, @newTask, @subTask, @comm,@duration, @startTime, @endTime, @status, @system, @startTime)";
                    result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Copy", "alert('Data Copied Successfully')", true);
                dt = DisplayRecords(txtSelDate.Text, ddlEmployee.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToDateTime(txtSelDate.Text).DayOfWeek == DayOfWeek.Sunday)
                    {                       
                        gvRecordsSunday.DataSource = dt;
                        gvRecordsSunday.DataBind();
                        ViewState["dirSun"] = dt;
                        ViewState["sortdrS"] = "Asc";
                        lblDay.Text = string.Empty;
                    }

                    if (Convert.ToDateTime(txtSelDate.Text).DayOfWeek == DayOfWeek.Monday)
                    {                        
                        gvRecordsMonday.DataSource = dt;
                        gvRecordsMonday.DataBind();
                        ViewState["dirMon"] = dt;
                        ViewState["sortdr"] = "Asc";
                        lblDay.Text = string.Empty;
                    }
                    if (Convert.ToDateTime(txtSelDate.Text).DayOfWeek == DayOfWeek.Tuesday)
                    {                        
                        gvRecordsTue.DataSource = dt;
                        gvRecordsTue.DataBind();
                        ViewState["dirTue"] = dt;
                        ViewState["sortdrT"] = "Asc";
                        lblDay.Text = string.Empty;
                    }
                    if (Convert.ToDateTime(txtSelDate.Text).DayOfWeek == DayOfWeek.Wednesday)
                    {                        
                        gvRecordsWed.DataSource = dt;
                        gvRecordsWed.DataBind();
                        ViewState["dirWed"] = dt;
                        ViewState["sortdrW"] = "Asc";
                        lblDay.Text = string.Empty;
                    }
                    if (Convert.ToDateTime(txtSelDate.Text).DayOfWeek == DayOfWeek.Thursday)
                    {                        
                        gvRecordsThu.DataSource = dt;
                        gvRecordsThu.DataBind();
                        ViewState["dirThu"] = dt;
                        ViewState["sortdrTH"] = "Asc";
                        lblDay.Text = string.Empty;
                    }
                    if (Convert.ToDateTime(txtSelDate.Text).DayOfWeek == DayOfWeek.Friday)
                    {                        
                        gvRecordsFri.DataSource = dt;
                        gvRecordsFri.DataBind();
                        ViewState["dirFri"] = dt;
                        ViewState["sortdrF"] = "Asc";
                        lblDay.Text = string.Empty;
                    }
                    if (Convert.ToDateTime(txtSelDate.Text).DayOfWeek == DayOfWeek.Saturday)
                    {                        
                        gvRecordsSat.DataSource = dt;
                        gvRecordsSat.DataBind();
                        ViewState["dirSat"] = dt;
                        ViewState["sortdrSA"] = "Asc";
                        lblDay.Text = string.Empty;
                    }
                }
                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                
            }
            lblCopyFrom.Text = string.Empty;
        }
    }

    protected void btmDateNo_Click(object sender, EventArgs e)
    {
        lblstatus.Text = "";
        lblOn.Text = "";
        lblCopyFrom.Text = string.Empty;
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void btnConfirmYes_Click(object sender, EventArgs e)
    {
        if (lblstatus.Text == "day")
        {
            result = objRec.DeleteData(lblCopyto.Text, ddlEmployee.SelectedValue);
            if (result == true)
            {
                dt = objRec.GetData(lblCopyFrom.Text, ddlEmployee.SelectedValue);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow drRow in dt.Rows)
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {  
                            new SqlParameter("@teamId",drRow["R_TeamId"].ToString()),
                            new SqlParameter("@empId",drRow["R_Employee_Id"].ToString()),
                            new SqlParameter("@username",drRow["R_User_Name"].ToString()),
                            new SqlParameter("@newClient",drRow["R_Client"].ToString()),
                            new SqlParameter("@newTask",drRow["R_Task"].ToString()),
                            new SqlParameter("@subTask", drRow["R_SubTask"].ToString()),
                            new SqlParameter("@comm",drRow["R_Comments"].ToString()),
                            new SqlParameter("@duration",drRow["R_Duration"].ToString()),
                            new SqlParameter("@startTime",lblCopyto.Text),
                            new SqlParameter("@endTime",lblCopyto.Text),
                            new SqlParameter("@status", "Completed"),
                            new SqlParameter("@system","P")
                       };
                        sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn,R_Status, R_System) values (@teamId,@empId, @username, @newClient, @newTask, @subTask, @comm,@duration, @startTime, @endTime, @status, @system)";
                        result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
                    }

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Copy Rec", "alert('Data Copied Successfully')", true);
                    dt = DisplayRecords(lblCopyto.Text, ddlEmployee.SelectedItem.Text);
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToDateTime(lblCopyto.Text).DayOfWeek == DayOfWeek.Sunday)
                        {
                            gvRecordsSunday.DataSource = dt;
                            gvRecordsSunday.DataBind();
                            ViewState["dirSun"] = dt;
                            ViewState["sortdrS"] = "Asc";
                            lblDay.Text = string.Empty;
                        }
                        if (Convert.ToDateTime(lblCopyto.Text).DayOfWeek == DayOfWeek.Monday)
                        {
                            gvRecordsMonday.DataSource = dt;
                            gvRecordsMonday.DataBind();
                            ViewState["dirMon"] = dt;
                            ViewState["sortdr"] = "Asc";
                            lblDay.Text = string.Empty;
                        }
                        if (Convert.ToDateTime(lblCopyto.Text).DayOfWeek == DayOfWeek.Tuesday)
                        {
                            gvRecordsTue.DataSource = dt;
                            gvRecordsTue.DataBind();
                            ViewState["dirTue"] = dt;
                            ViewState["sortdrT"] = "Asc";
                            lblDay.Text = string.Empty;
                        }
                        if (Convert.ToDateTime(lblCopyto.Text).DayOfWeek == DayOfWeek.Wednesday)
                        {
                            gvRecordsWed.DataSource = dt;
                            gvRecordsWed.DataBind();
                            ViewState["dirWed"] = dt;
                            ViewState["sortdrW"] = "Asc";
                            lblDay.Text = string.Empty;
                        }
                        if (Convert.ToDateTime(lblCopyto.Text).DayOfWeek == DayOfWeek.Thursday)
                        {
                            gvRecordsThu.DataSource = dt;
                            gvRecordsThu.DataBind();
                            ViewState["dirThu"] = dt;
                            ViewState["sortdrTH"] = "Asc";
                            lblDay.Text = string.Empty;
                        }
                        if (Convert.ToDateTime(lblCopyto.Text).DayOfWeek == DayOfWeek.Friday)
                        {
                            gvRecordsFri.DataSource = dt;
                            gvRecordsFri.DataBind();
                            ViewState["dirFri"] = dt;
                            ViewState["sortdrF"] = "Asc";
                            lblDay.Text = string.Empty;
                        }
                        if (Convert.ToDateTime(lblCopyto.Text).DayOfWeek == DayOfWeek.Saturday)
                        {
                            gvRecordsSat.DataSource = dt;
                            gvRecordsSat.DataBind();
                            ViewState["dirSat"] = dt;
                            ViewState["sortdrSA"] = "Asc";
                            lblDay.Text = string.Empty;
                        }
                    }

                    BindWeekViewPivote(txtFrom.Text, txtTo.Text);
                }
                lblCopyto.Text = string.Empty;
            }
        }
        else
        {
            mpeDuration.Show();            
        }
    }

    protected void btnConfirmNo_Click(object sender, EventArgs e)
    {
        lblOn.Text = "";
        lblstatus.Text = "";
        lblCopyto.Text = string.Empty;
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }
    protected void lnkCopyLastWeek_Click(object sender, EventArgs e)
    {

        try
        {
            dt = objRec.CheckSubmitStatus(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text);
            if (dt.Rows.Count > 0)
            {
                string submit = dt.Rows[0]["R_Submit"].ToString();
                if (submit == "1")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Cannot copy", "alert('Data already submitted for the week.')", true);
                    BindAllData();
                    return;
                }
            }
            dt = objRec.GetLastWeekData(txtFrom.Text, txtTo.Text, ddlEmployee.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                lblOn.Text = "Between";
                lblstatus.Text = "week";
                lblCopyto.Text = txtFrom.Text + " and " + txtTo.Text;
                mpeConfirm.Show();
            }
            else
            {
                mpeDuration.Show();
            }
        }
        catch (Exception)
        {
            
        }
        
        
       
    }

    //private void GetLogins(string from, string to)
    //{

    //}

    private void BindWeekViewPivote(string from, string to)
    {
        try
        {
            dtResult = new DataTable();
            DataRow dr;
            dtResult.Columns.Add("Client", typeof(string));
            dtResult.Columns.Add("day", typeof(string));
            dtResult.Columns.Add("daynum", typeof(string));
            dtResult.Columns.Add("Duration", typeof(double));

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", ddlTeam.SelectedValue),
            new SqlParameter("@emp", ddlEmployee.SelectedItem.Text),
            new SqlParameter("@from", from),
            new SqlParameter("@to", to)
        };

        sQuery = "select CL_ClientName as [Client], CONVERT(VARCHAR(12), R_TimeDate, 107) as [day], DATEPART(dw,R_TimeDate) as daynum, " +
                    "sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds " +
                     "from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID where R_User_Name= @emp and " +
                    "CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' " +
                    "GROUP BY CL_ClientName, CONVERT(VARCHAR(12), R_TimeDate, 107) , DATEPART(dw,R_TimeDate) order by DATEPART(dw,R_TimeDate)";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    dr["Client"] = drRow["Client"];
                    dr["day"] = Convert.ToDateTime(drRow["day"]).ToString("MM/dd, ddd");
                    dr["daynum"] = drRow["daynum"];
                    decimal duration = Math.Round(Convert.ToDecimal(drRow["hour"]) + (Convert.ToDecimal(drRow["minute"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                    dr["Duration"] = duration;

                    dtResult.Rows.Add(dr);
                }
            }

            dt = new DataTable();

            SqlParameter[] parameters1 = new SqlParameter[]
        {
            new SqlParameter("@team", ddlTeam.SelectedValue),
            new SqlParameter("@emp", ddlEmployee.SelectedItem.Text),
            new SqlParameter("@from", from),
            new SqlParameter("@to", to)
        };

            sQuery = "select LA_Reason as [Client], CONVERT(VARCHAR(12), LA_TimeDate, 107) as [day], DATEPART(dw,LA_TimeDate) as daynum, " +
                    "sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds " +
                     "from RTM_Log_Actions WITH (NOLOCK) where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) BETWEEN @from and @to and LA_Duration != 'HH:MM:SS' " +
                      "and (LA_Reason = 'Meeting' or LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call') " +
                     "GROUP BY LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 107) , DATEPART(dw,LA_TimeDate) order by DATEPART(dw,LA_TimeDate)";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters1);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    dr["Client"] = drRow["Client"];
                    dr["day"] = Convert.ToDateTime(drRow["day"]).ToString("MM/dd, ddd");
                    dr["daynum"] = drRow["daynum"];
                    decimal duration1 = Math.Round(Convert.ToDecimal(drRow["hour"]) + (Convert.ToDecimal(drRow["minute"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                    dr["Duration"] = duration1;

                    dtResult.Rows.Add(dr);
                }
            }

        //    dt = new DataTable();
        // SqlParameter[] parameters2 = new SqlParameter[]
        //{
        //    new SqlParameter("@team", ddlTeam.SelectedValue),
        //    new SqlParameter("@emp", ddlEmployee.SelectedItem.Text),
        //    new SqlParameter("@from", from),
        //    new SqlParameter("@to", to)
        //};
        // sQuery = "select L_Type as [Client], CONVERT(VARCHAR(12), LD_Date, 107) as [day], DATEPART(dw,LD_Date) as daynum, LD_Duration as [hour], '0' as [minute], '0' as [seconds] from RTM_LeaveDetails WITH (NOLOCK) left join RTM_Leave WITH (NOLOCK) on LD_LID = L_ID where LD_UserName= @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to GROUP BY L_Type, CONVERT(VARCHAR(12), LD_Date, 107) , DATEPART(dw,LD_Date), LD_Duration order by DATEPART(dw,LD_Date)";
        //    dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

        //    if (dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow drRow in dt.Rows)
        //        {
        //            dr = dtResult.NewRow();

        //            dr["Client"] = drRow["Client"];
        //            dr["day"] = Convert.ToDateTime(drRow["day"]).ToString("MM/dd, ddd");
        //            dr["daynum"] = drRow["daynum"];
        //            //decimal duration1 = Math.Round(Convert.ToDecimal(drRow["hour"]) + (Convert.ToDecimal(drRow["minute"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
        //            if (!string.IsNullOrEmpty(drRow["hour"].ToString()))
        //            {
        //                dr["Duration"] = drRow["hour"].ToString();
        //            }
        //            else
        //            {
        //                dr["Duration"] = "8";
        //            }
                    

        //            dtResult.Rows.Add(dr);
        //        }
        //    }

            DataView dv = dtResult.DefaultView;
            dv.Sort = "daynum asc";
            dtResult = dv.ToTable();
            //hfDayCount.Value = dtResult.Rows.Count.ToString();
            var item = from row in dtResult.AsEnumerable()
                       group row by row.Field<string>("daynum") into days                       
                       select new
                       {                           
                           CountOfDays = days.Count()
                       };
            hfDayCount.Value = "0";
            hfLeaveCount.Value = "0";
            hfDayCount.Value = item.Count().ToString();
            string expression = "(Client Like '%Personal/Sick Time%' or Client Like '%Vacation%' or Client Like '%Maternity Leave%' or Client Like '%Paternity Leave%') and Duration >= 4";
            var rows = dtResult.Select(expression);
            if (rows.Any())
            {
                hfLeaveCount.Value = rows.Count().ToString();
            }

            if (dtResult.Rows.Count > 0)
            {
                lblHeading.Text = "Week (" + txtFrom.Text + " - " + txtTo.Text + ")";
                lblNoData.Text = "";
                div2.Visible = true;
                lblHeading.Visible = true;

                //CustomePivote pvt = new CustomePivote(dtResult);
                //DataTable pivotTable = new DataTable();

                //pivotTable = pvt.PivotData("Client", "Duration", AggregateFunction.Sum, true, "day");

               // var ordersPvtData = new PivotData(new[] { "Client", "day" },
               //new SumAggregatorFactory("Duration"));
               //     ordersPvtData.ProcessData(new DataTableReader(dtResult));

               //     var pvtTbl = new PivotTable(
               //                 new[] { "Client" }, //rows
               //                 new[] { "day" }, // columns
               //                 ordersPvtData,

               //                 new CustomSortKeyComparer(new[] { NaturalSortKeyComparer.Instance }),
               //                new CustomSortKeyComparer(new[] { NaturalSortKeyComparer.Instance })
               //             );

               //     var pvtDataTableWr = new PivotTableDataTableWriter("PivotTable");
               //     DataTable res = pvtDataTableWr.Write(pvtTbl);

               //     res.Columns[0].ColumnName = "Customer";
               //     res.Columns[res.Columns.Count - 1].ColumnName = "Totals";

               //     gvWeeklyView.DataSource = res;
               //     gvWeeklyView.DataBind();
               //     gvWeeklyView.Rows[gvWeeklyView.Rows.Count - 1].ForeColor = Color.Gray;                    
               //     gvWeeklyView.Rows[gvWeeklyView.Rows.Count - 1].Enabled = false;
               //     gvWeeklyView.Rows[gvWeeklyView.Rows.Count - 1].Font.Bold = true;
                    //ViewState["dirWk"] = res;
                    //ViewState["sortdrWk"] = "Asc";
                Pivot advPivot = new Pivot(dtResult);
                advPivot.CssTopHeading = "Heading";
                advPivot.CssLeftColumn = "LeftColumn";
                advPivot.CssItems = "Items";
                advPivot.CssTotals = "Totals";

                HtmlTable advancedPivot = advPivot.PivotTable("day", "Client", new string[] { "Duration" });
                //string myTable = advancedPivot.InnerHtml.ToString();
                advancedPivot.Align = "center";
                advancedPivot.Border = 1;
                advancedPivot.Rows.RemoveAt(1);
                //advancedPivot.Rows[0].Cells[0].Attributes.Add("class", "Heading");
                advancedPivot.Rows[0].Cells[0].InnerText = "Job";
                advancedPivot.Attributes.Add("class", "Heading");
                advancedPivot.Attributes.Add("id", "timesheet");
                //advancedPivot.Width = "80%";
                div2.Controls.Add(advancedPivot);
                //ViewState["weekView"] = advancedPivot; 
            }
            else
            {
                lblHeading.Text = "Week (" + txtFrom.Text + " - " + txtTo.Text + ")";
                lblNoData.Text = "No data available for the selected week!";
                lblHeading.Visible = true;
                div2.Visible = true;
                gvWeeklyView.DataSource = null;
                gvWeeklyView.DataBind();
               // ViewState["weekView"] = "";
            }

           //UpdatePanel9.Update();
            
            
        }
        catch (Exception)
        {
            
            
        }
        
    }
    protected void txtFrom_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtFrom.Text))
        {
            txtTo.Text = string.Empty;
        }
        else if (string.IsNullOrWhiteSpace(txtFrom.Text))
        {
            txtTo.Text = string.Empty;
        }
        else
        {
            txtTo.Text = Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString();
        }        

        BindAllData();        
    }

    protected void btnWithDuration_Click(object sender, EventArgs e)
    {
        try
        {         
                dt = objRec.CopyLastWeekData(Convert.ToDateTime(txtFrom.Text).AddDays(-7).ToShortDateString(), Convert.ToDateTime(txtFrom.Text).AddDays(-1).ToShortDateString(), ddlEmployee.SelectedValue);
                if (dt.Rows.Count > 0)
                {
                    result = objRec.DeleteWeekData(txtFrom.Text, txtTo.Text, ddlEmployee.SelectedValue);
                    foreach (DataRow drRow in dt.Rows)
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {  
                            new SqlParameter("@teamId",drRow["R_TeamId"].ToString()),
                            new SqlParameter("@empId",drRow["R_Employee_Id"].ToString()),
                            new SqlParameter("@username",drRow["R_User_Name"].ToString()),
                            new SqlParameter("@newClient",drRow["R_Client"].ToString()),
                            new SqlParameter("@newTask",drRow["R_Task"].ToString()),
                            new SqlParameter("@subTask", drRow["R_SubTask"].ToString()),
                            new SqlParameter("@comm",drRow["R_Comments"].ToString()),
                            new SqlParameter("@duration",drRow["R_Duration"].ToString()),
                            new SqlParameter("@startTime", drRow["R_Start_Date_Time"].ToString()),
                            new SqlParameter("@endTime",drRow["R_CreatedOn"].ToString()),
                            new SqlParameter("@status", "Completed"),
                            new SqlParameter("@system", "P")
                       };
                        sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn,R_Status, R_System, R_TimeDate) values (@teamId,@empId, @username, @newClient, @newTask, @subTask, @comm,@duration, @startTime, @endTime, @status, @system, @startTime)";
                        result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
                    }

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Copy week Rec", "alert('Data Copied Successfully')", true);

                    BindAllData();
                    
                }
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void btnWithoutDuration_Click(object sender, EventArgs e)
    {
        try
        {
            
                dt = objRec.CopyLastWeekData(Convert.ToDateTime(txtFrom.Text).AddDays(-7).ToShortDateString(), Convert.ToDateTime(txtFrom.Text).AddDays(-1).ToShortDateString(), ddlEmployee.SelectedValue);
                if (dt.Rows.Count > 0)
                {
                    result = objRec.DeleteWeekData(txtFrom.Text, txtTo.Text, ddlEmployee.SelectedValue);
                    foreach (DataRow drRow in dt.Rows)
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {  
                            new SqlParameter("@teamId",drRow["R_TeamId"].ToString()),
                            new SqlParameter("@empId",drRow["R_Employee_Id"].ToString()),
                            new SqlParameter("@username",drRow["R_User_Name"].ToString()),
                            new SqlParameter("@newClient",drRow["R_Client"].ToString()),
                            new SqlParameter("@newTask",drRow["R_Task"].ToString()),
                            new SqlParameter("@subTask", drRow["R_SubTask"].ToString()),
                            new SqlParameter("@comm",drRow["R_Comments"].ToString()),
                            new SqlParameter("@duration","00:00:00"),
                            new SqlParameter("@startTime", drRow["R_Start_Date_Time"].ToString()),
                            new SqlParameter("@endTime",drRow["R_CreatedOn"].ToString()),
                            new SqlParameter("@status", "Completed"),
                            new SqlParameter("@system", "P")
                       };
                        sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn,R_Status, R_System, R_TimeDate) values (@teamId,@empId, @username, @newClient, @newTask, @subTask, @comm,@duration, @startTime, @endTime, @status, @system, @startTime)";
                        result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
                    }

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Copy week Rec", "alert('Data Copied Successfully')", true);

                    BindAllData();
                    
                }          
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void btnCancelCopy_Click(object sender, EventArgs e)
    {
        BindAllData();
    }

    protected void gvRecordsSunday_DataBound(object sender, EventArgs e)
    {
        //DropDownList ddlSGVAddTask = gvRecordsSunday.FooterRow.FindControl("ddlSAddTask") as DropDownList;
        //SqlParameter[] parameters3 = new SqlParameter[]
        //   {           
        //       new SqlParameter("@id",ddlTeam.SelectedValue ),
        //   };

        //sQuery = "select * from RTM_Task_List where TL_TeamId = @id and TL_Status =1 order By TL_Task";
        //dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters3);
        //if (dt.Rows.Count > 0)
        //{
        //    ddlSGVAddTask.DataSource = dt;
        //    ddlSGVAddTask.DataTextField = "TL_Task";
        //    ddlSGVAddTask.DataValueField = "TL_ID";
        //    ddlSGVAddTask.DataBind();

        //}
    }

    protected void btnSGVAdd_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        string addDuration="00:00:00";
        string clientId = hfCustomerId.Value;
        TextBox txtClientSelect = (TextBox)grdrow.FindControl("txtSearchGvClient");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlSAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlSADDSubTask");
        TextBox txtAddComments = (TextBox)grdrow.FindControl("txtSGVAddComments");
        Label txtAddUser = (Label)grdrow.FindControl("lblSADDUser");
        string empId = ddlEmployee.SelectedValue;
        TextBox txtAddGVDuration = (TextBox)grdrow.FindControl("txtSADDDuration");
        string selTask = "0";
        string selSubTask = "0";
        if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select client')", true);
            return;
        }

        if (string.IsNullOrEmpty(txtClientSelect.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client1", "alert('Please select client')", true);
            return;
        }

        if (txtClientSelect.Text == "Personal/Sick Time" || txtClientSelect.Text == "Inclement Weather" || txtClientSelect.Text == "Public Holiday" || txtClientSelect.Text == "Vacation" || txtClientSelect.Text == "Bereavement" || txtClientSelect.Text == "Jury Duty" || txtClientSelect.Text == "Maternity Leave" || txtClientSelect.Text == "Paternity Leave" || txtClientSelect.Text == "Medical Leave" || txtClientSelect.Text == "Comp off")
        {
            selTask = "0";
            selSubTask = "0";
        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access", "alert('you are not authorized to add work time.')", true);
                return;
            }
            if (ddlAddTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
                return;
            }
            else
            {
                selTask = ddlAddTask.SelectedValue;
            }

            if (ddlAddSubTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subTask", "alert('Please select subtask')", true);
                return;
            }
            else
            {
                selSubTask = ddlAddSubTask.SelectedValue;
            }
        }
        if (!string.IsNullOrEmpty(txtAddGVDuration.Text))
        {
            if (txtAddGVDuration.Text.Contains('.'))
            {
                int dotCount = txtAddGVDuration.Text.Count(x => x == '.');
                if (dotCount == 1)
                {
                    decimal val = Math.Round(Convert.ToDecimal(txtAddGVDuration.Text), 2, MidpointRounding.AwayFromZero);
                    txtAddGVDuration.Text = val.ToString();
                }
            }
        }
        if (txtAddGVDuration.Text.Contains(':'))
        {
            if (newRegex2.IsMatch(txtAddGVDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                return;
            }
        }
        else if (newRegex.IsMatch(txtAddGVDuration.Text) == false)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
            return;
        }
        if (txtAddGVDuration.Text.Contains(':') ==false)
        {
            
            decimal time = decimal.Parse(txtAddGVDuration.Text.Trim());
            var hours = time.ToString().Split('.')[0];
            var minutes = ((time * 60) % 60).ToString().Split('.')[0];
            var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

           addDuration = hours + ":" + minutes + ":" + seconds;

        }
        else
        {
            int charCount = txtAddGVDuration.Text.Count(x => x == ':');
            if (charCount == 1)
            {
                addDuration = txtAddGVDuration.Text + ":00";
            }
            else
            {
                addDuration = txtAddGVDuration.Text;
            }
        }
        if (hfCustomerId.Value != "0")
        {
            result = objRec.AddRecord(ddlTeam.SelectedValue, empId, txtAddUser.Text, clientId, selTask, selSubTask , addDuration, txtAddComments.Text, lblSunday.Text, DateTime.Now.ToString(), "Completed", "P");
            if (result == true)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "add Rec", "alert('Record added successfully)", true);
                hfCustomerId.Value = "0";

                dt = DisplayRecords(lblSunday.Text, ddlEmployee.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    gvRecordsSunday.DataSource = dt;
                    gvRecordsSunday.DataBind();
                    ViewState["dirSun"] = dt;
                    ViewState["sortdrS"] = "Asc";
                }
                else
                {
                    gvRecordsSunday.DataSource = null;
                    gvRecordsSunday.DataBind();
                }

                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
            }
        }
    }

    protected void btnMGVAdd_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        string addDuration = "00:00:00";
        string clientId = hfCustomerId.Value;
        TextBox txtClientSelect = (TextBox)grdrow.FindControl("txtSearchGvClientM");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlMAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlMADDSubTask");
        TextBox txtAddComments = (TextBox)grdrow.FindControl("txtMGVAddComments");
        Label txtAddUser = (Label)grdrow.FindControl("lblMADDUser");
        string empId = ddlEmployee.SelectedValue;
        TextBox txtAddGVDuration = (TextBox)grdrow.FindControl("txtMADDDuration");
        string selTask = "0";
        string selSubTask = "0";
        if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select client')", true);
            return;
        }

        if (string.IsNullOrEmpty(txtClientSelect.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client1", "alert('Please select client')", true);
            return;
        }

        if (txtClientSelect.Text == "Personal/Sick Time" || txtClientSelect.Text == "Inclement Weather" || txtClientSelect.Text == "Public Holiday" || txtClientSelect.Text == "Vacation" || txtClientSelect.Text == "Bereavement" || txtClientSelect.Text == "Jury Duty" || txtClientSelect.Text == "Maternity Leave" || txtClientSelect.Text == "Paternity Leave" || txtClientSelect.Text == "Medical Leave" || txtClientSelect.Text == "Comp off")
        {
            selTask = "0";
            selSubTask = "0";
        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access", "alert('you are not authorized to add work time.')", true);
                return;
            }
            if (ddlAddTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
                return;
            }
            else
            {
                selTask = ddlAddTask.SelectedValue;
            }

            if (ddlAddSubTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subTask", "alert('Please select subtask')", true);
                return;
            }
            else
            {
                selSubTask = ddlAddSubTask.SelectedValue;
            }
        }

        if (!string.IsNullOrEmpty(txtAddGVDuration.Text))
        {
            if (txtAddGVDuration.Text.Contains('.'))
            {
                int dotCount = txtAddGVDuration.Text.Count(x => x == '.');
                if (dotCount == 1)
                {
                    decimal val = Math.Round(Convert.ToDecimal(txtAddGVDuration.Text), 2, MidpointRounding.AwayFromZero);
                    txtAddGVDuration.Text = val.ToString();
                }
            }
        }
        if (txtAddGVDuration.Text.Contains(':'))
        {
            if (newRegex2.IsMatch(txtAddGVDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                return;
            }
        }
        else if (newRegex.IsMatch(txtAddGVDuration.Text) == false)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
            return;
        }
        if (txtAddGVDuration.Text.Contains(':') ==false)
        {

            decimal time = decimal.Parse(txtAddGVDuration.Text.Trim());
            var hours = time.ToString().Split('.')[0];
            var minutes = ((time * 60) % 60).ToString().Split('.')[0];
            var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

            addDuration = hours + ":" + minutes + ":" + seconds;

        }
        else
        {
            int charCount = txtAddGVDuration.Text.Count(x => x == ':');
            if (charCount == 1)
            {
                addDuration = txtAddGVDuration.Text + ":00";
            }
            else
            {
                addDuration = txtAddGVDuration.Text;
            }
        }

        if (hfCustomerId.Value != "0")
        {
            result = objRec.AddRecord(ddlTeam.SelectedValue, empId, txtAddUser.Text, clientId, selTask, selSubTask, addDuration, txtAddComments.Text, lblMonday.Text, DateTime.Now.ToString(), "Completed", "P");
            if (result == true)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "add Rec", "alert('Record added successfully)", true);
                hfCustomerId.Value = "0";

                dt = DisplayRecords(lblMonday.Text, ddlEmployee.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    gvRecordsMonday.DataSource = dt;
                    gvRecordsMonday.DataBind();
                    ViewState["dirMon"] = dt;
                    ViewState["sortdr"] = "Asc";
                }
                else
                {
                    gvRecordsMonday.DataSource = null;
                    gvRecordsMonday.DataBind();
                }
                //ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + gvRecordsMonday.ClientID + "', 250, 1000 , 27 ,true); </script>", false);

                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
            }
        }
    }

    protected void btnTGVAdd_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        string addDuration = "00:00:00";
        string clientId = hfCustomerId.Value;
        TextBox txtClientSelect = (TextBox)grdrow.FindControl("txtSearchGvClientT");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlTAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlTADDSubTask");
        TextBox txtAddComments = (TextBox)grdrow.FindControl("txtTGVAddComments");
        Label txtAddUser = (Label)grdrow.FindControl("lblTADDUser");
        string empId = ddlEmployee.SelectedValue;
        TextBox txtAddGVDuration = (TextBox)grdrow.FindControl("txtTADDDuration");
        
        if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select client')", true);
            return;
        }

        if (string.IsNullOrEmpty(txtClientSelect.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client1", "alert('Please select client')", true);
            return;
        }
        string selTask = "0";
        string selSubTask = "0";
        if (txtClientSelect.Text == "Personal/Sick Time" || txtClientSelect.Text == "Inclement Weather" || txtClientSelect.Text == "Public Holiday" || txtClientSelect.Text == "Vacation" || txtClientSelect.Text == "Bereavement" || txtClientSelect.Text == "Jury Duty" || txtClientSelect.Text == "Maternity Leave" || txtClientSelect.Text == "Paternity Leave" || txtClientSelect.Text == "Medical Leave" || txtClientSelect.Text == "Comp off")
        {
            selTask = "0";
            selSubTask = "0";
        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access", "alert('you are not authorized to add work time.')", true);
                return;
            }
            if (ddlAddTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
                return;
            }
            else
            {
                selTask = ddlAddTask.SelectedValue;
            }

            if (ddlAddSubTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subTask", "alert('Please select subtask')", true);
                return;
            }
            else
            {
                selSubTask = ddlAddSubTask.SelectedValue;
            }
        }

        if (!string.IsNullOrEmpty(txtAddGVDuration.Text))
        {
            if (txtAddGVDuration.Text.Contains('.'))
            {
                int dotCount = txtAddGVDuration.Text.Count(x => x == '.');
                if (dotCount == 1)
                {
                    decimal val = Math.Round(Convert.ToDecimal(txtAddGVDuration.Text), 2, MidpointRounding.AwayFromZero);
                    txtAddGVDuration.Text = val.ToString();
                }
            }
        }
        if (txtAddGVDuration.Text.Contains(':'))
        {
            if (newRegex2.IsMatch(txtAddGVDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                return;
            }
        }
        else if (newRegex.IsMatch(txtAddGVDuration.Text) == false)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
            return;
        }
        if (txtAddGVDuration.Text.Contains(':') == false)
        {

            decimal time = decimal.Parse(txtAddGVDuration.Text.Trim());
            var hours = time.ToString().Split('.')[0];
            var minutes = ((time * 60) % 60).ToString().Split('.')[0];
            var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

            addDuration = hours + ":" + minutes + ":" + seconds;

        }
        else
        {
            int charCount = txtAddGVDuration.Text.Count(x => x == ':');
            if (charCount == 1)
            {
                addDuration = txtAddGVDuration.Text + ":00";
            }
            else
            {
                addDuration = txtAddGVDuration.Text;
            }
        }

        if (hfCustomerId.Value != "0")
        {
            result = objRec.AddRecord(ddlTeam.SelectedValue, empId, txtAddUser.Text, clientId, selTask, selSubTask, addDuration, txtAddComments.Text, lblTuesday.Text, DateTime.Now.ToString(), "Completed", "P");
            if (result == true)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "add Rec", "alert('Record added successfully')", true);
                hfCustomerId.Value = "0";

                dt = DisplayRecords(lblTuesday.Text, ddlEmployee.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    gvRecordsTue.DataSource = dt;
                    gvRecordsTue.DataBind();
                    ViewState["dirTue"] = dt;
                    ViewState["sortdrT"] = "Asc";
                }
                else
                {
                    gvRecordsTue.DataSource = null;
                    gvRecordsTue.DataBind();
                }

                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
            }
        }
    }

    protected void btnWGVAdd_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        string addDuration = "00:00:00";
        string clientId = hfCustomerId.Value;
        TextBox txtClientSelect = (TextBox)grdrow.FindControl("txtSearchGvClientW");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlWAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlWADDSubTask");
        TextBox txtAddComments = (TextBox)grdrow.FindControl("txtWGVAddComments");
        Label txtAddUser = (Label)grdrow.FindControl("lblWADDUser");
        string empId = ddlEmployee.SelectedValue;
        TextBox txtAddGVDuration = (TextBox)grdrow.FindControl("txtWADDDuration");
        if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select client')", true);
            return;
        }

        if (string.IsNullOrEmpty(txtClientSelect.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client1", "alert('Please select client')", true);
            return;
        }

        string selTask = "0";
        string selSubTask = "0";
        if (txtClientSelect.Text == "Personal/Sick Time" || txtClientSelect.Text == "Inclement Weather" || txtClientSelect.Text == "Public Holiday" || txtClientSelect.Text == "Vacation" || txtClientSelect.Text == "Bereavement" || txtClientSelect.Text == "Jury Duty" || txtClientSelect.Text == "Maternity Leave" || txtClientSelect.Text == "Paternity Leave" || txtClientSelect.Text == "Medical Leave" || txtClientSelect.Text == "Comp off")
        {
            selTask = "0";
            selSubTask = "0";
        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access", "alert('you are not authorized to add work time.')", true);
                return;
            }
            if (ddlAddTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
                return;
            }
            else
            {
                selTask = ddlAddTask.SelectedValue;
            }

            if (ddlAddSubTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subTask", "alert('Please select subtask')", true);
                return;
            }
            else
            {
                selSubTask = ddlAddSubTask.SelectedValue;
            }
        }

        if (!string.IsNullOrEmpty(txtAddGVDuration.Text))
        {
            if (txtAddGVDuration.Text.Contains('.'))
            {
                int dotCount = txtAddGVDuration.Text.Count(x => x == '.');
                if (dotCount == 1)
                {
                    decimal val = Math.Round(Convert.ToDecimal(txtAddGVDuration.Text), 2, MidpointRounding.AwayFromZero);
                    txtAddGVDuration.Text = val.ToString();
                }
            }
        }
        if (txtAddGVDuration.Text.Contains(':'))
        {
            if (newRegex2.IsMatch(txtAddGVDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                return;
            }
        }
        else if (newRegex.IsMatch(txtAddGVDuration.Text) == false)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
            return;
        }
        if (txtAddGVDuration.Text.Contains(':') == false)
        {

            decimal time = decimal.Parse(txtAddGVDuration.Text.Trim());
            var hours = time.ToString().Split('.')[0];
            var minutes = ((time * 60) % 60).ToString().Split('.')[0];
            var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

            addDuration = hours + ":" + minutes + ":" + seconds;

        }
        else
        {
            int charCount = txtAddGVDuration.Text.Count(x => x == ':');
            if (charCount == 1)
            {
                addDuration = txtAddGVDuration.Text + ":00";
            }
            else
            {
                addDuration = txtAddGVDuration.Text;
            }
        }

        if (hfCustomerId.Value != "0")
        {
            result = objRec.AddRecord(ddlTeam.SelectedValue, empId, txtAddUser.Text, clientId, selTask, selSubTask, addDuration, txtAddComments.Text, lblWed.Text, DateTime.Now.ToString(), "Completed", "P");
            if (result == true)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "add Rec", "alert('Record added successfully)", true);
                hfCustomerId.Value = "0";

                dt = DisplayRecords(lblWed.Text, ddlEmployee.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    gvRecordsWed.DataSource = dt;
                    gvRecordsWed.DataBind();
                    ViewState["dirWed"] = dt;
                    ViewState["sortdrW"] = "Asc";
                }
                else
                {
                    gvRecordsWed.DataSource = null;
                    gvRecordsWed.DataBind();
                }

                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
            }
        }
    }

    protected void btnTHGVAdd_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        string addDuration = "00:00:00";
        string clientId = hfCustomerId.Value;
        TextBox txtClientSelect = (TextBox)grdrow.FindControl("txtSearchGvClientTH");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlTHAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlTHADDSubTask");
        TextBox txtAddComments = (TextBox)grdrow.FindControl("txtTHGVAddComments");
        Label txtAddUser = (Label)grdrow.FindControl("lblTHADDUser");
        string empId = ddlEmployee.SelectedValue;
        TextBox txtAddGVDuration = (TextBox)grdrow.FindControl("txtTHADDDuration");
        if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select client')", true);
            return;
        }

        if (string.IsNullOrEmpty(txtClientSelect.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client1", "alert('Please select client')", true);
            return;
        }

        string selTask = "0";
        string selSubTask = "0";
        if (txtClientSelect.Text == "Personal/Sick Time" || txtClientSelect.Text == "Inclement Weather" || txtClientSelect.Text == "Public Holiday" || txtClientSelect.Text == "Vacation" || txtClientSelect.Text == "Bereavement" || txtClientSelect.Text == "Jury Duty" || txtClientSelect.Text == "Maternity Leave" || txtClientSelect.Text == "Paternity Leave" || txtClientSelect.Text == "Medical Leave" || txtClientSelect.Text == "Comp off")
        {
            selTask = "0";
            selSubTask = "0";
        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access", "alert('you are not authorized to add work time.')", true);
                return;
            }
            if (ddlAddTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
                return;
            }
            else
            {
                selTask = ddlAddTask.SelectedValue;
            }

            if (ddlAddSubTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subTask", "alert('Please select subtask')", true);
                return;
            }
            else
            {
                selSubTask = ddlAddSubTask.SelectedValue;
            }
        }

        if (!string.IsNullOrEmpty(txtAddGVDuration.Text))
        {
            if (txtAddGVDuration.Text.Contains('.'))
            {
                int dotCount = txtAddGVDuration.Text.Count(x => x == '.');
                if (dotCount == 1)
                {
                    decimal val = Math.Round(Convert.ToDecimal(txtAddGVDuration.Text), 2, MidpointRounding.AwayFromZero);
                    txtAddGVDuration.Text = val.ToString();
                }
            }
        }
        if (txtAddGVDuration.Text.Contains(':'))
        {
            if (newRegex2.IsMatch(txtAddGVDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                return;
            }
        }
        else if (newRegex.IsMatch(txtAddGVDuration.Text) == false)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
            return;
        }
        if (txtAddGVDuration.Text.Contains(':')==false)
        {

            decimal time = decimal.Parse(txtAddGVDuration.Text.Trim());
            var hours = time.ToString().Split('.')[0];
            var minutes = ((time * 60) % 60).ToString().Split('.')[0];
            var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

            addDuration = hours + ":" + minutes + ":" + seconds;

        }
        else
        {
            int charCount = txtAddGVDuration.Text.Count(x => x == ':');
            if (charCount == 1)
            {
                addDuration = txtAddGVDuration.Text + ":00";
            }
            else
            {
                addDuration = txtAddGVDuration.Text;
            }
        }

        if (hfCustomerId.Value != "0")
        {
            result = objRec.AddRecord(ddlTeam.SelectedValue, empId, txtAddUser.Text, clientId, selTask, selSubTask, addDuration, txtAddComments.Text, lblThu.Text, DateTime.Now.ToString(), "Completed", "P");
            if (result == true)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "add Rec", "alert('Record added successfully)", true);
                hfCustomerId.Value = "0";

                dt = DisplayRecords(lblThu.Text, ddlEmployee.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    gvRecordsThu.DataSource = dt;
                    gvRecordsThu.DataBind();
                    ViewState["dirThu"] = dt;
                    ViewState["sortdrTH"] = "Asc";
                }
                else
                {
                    gvRecordsThu.DataSource = null;
                    gvRecordsThu.DataBind();
                }

                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
            }
        }
    }

    protected void btnFGVAdd_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        string addDuration = "00:00:00";
        string clientId = hfCustomerId.Value;
        TextBox txtClientSelect = (TextBox)grdrow.FindControl("txtSearchGvClientF");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlFAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlFADDSubTask");
        TextBox txtAddComments = (TextBox)grdrow.FindControl("txtFGVAddComments");
        Label txtAddUser = (Label)grdrow.FindControl("lblFADDUser");
        string empId = ddlEmployee.SelectedValue;
        TextBox txtAddGVDuration = (TextBox)grdrow.FindControl("txtFADDDuration");
        if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select client')", true);
            return;
        }

        if (string.IsNullOrEmpty(txtClientSelect.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client1", "alert('Please select client')", true);
            return;
        }

        string selTask = "0";
        string selSubTask = "0";
        if (txtClientSelect.Text == "Personal/Sick Time" || txtClientSelect.Text == "Inclement Weather" || txtClientSelect.Text == "Public Holiday" || txtClientSelect.Text == "Vacation" || txtClientSelect.Text == "Bereavement" || txtClientSelect.Text == "Jury Duty" || txtClientSelect.Text == "Maternity Leave" || txtClientSelect.Text == "Paternity Leave" || txtClientSelect.Text == "Medical Leave" || txtClientSelect.Text == "Comp off")
        {
            selTask = "0";
            selSubTask = "0";
        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {               
                 ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access", "alert('you are not authorized to add work time.')", true);
                 return;
            }
            if (ddlAddTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
                return;
            }
            else
            {
                selTask = ddlAddTask.SelectedValue;
            }

            if (ddlAddSubTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subTask", "alert('Please select subtask')", true);
                return;
            }
            else
            {
                selSubTask = ddlAddSubTask.SelectedValue;
            }
        }

        if (!string.IsNullOrEmpty(txtAddGVDuration.Text))
        {
            if (txtAddGVDuration.Text.Contains('.'))
            {
                int dotCount = txtAddGVDuration.Text.Count(x => x == '.');
                if (dotCount == 1)
                {
                    decimal val = Math.Round(Convert.ToDecimal(txtAddGVDuration.Text), 2, MidpointRounding.AwayFromZero);
                    txtAddGVDuration.Text = val.ToString();
                }
            }
        }
        if (txtAddGVDuration.Text.Contains(':'))
        {
            if (newRegex2.IsMatch(txtAddGVDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                return;
            }
        }
        else if (newRegex.IsMatch(txtAddGVDuration.Text) == false)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
            return;
        }
        if (txtAddGVDuration.Text.Contains(':') ==false)
        {

            decimal time = decimal.Parse(txtAddGVDuration.Text.Trim());
            var hours = time.ToString().Split('.')[0];
            var minutes = ((time * 60) % 60).ToString().Split('.')[0];
            var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

            addDuration = hours + ":" + minutes + ":" + seconds;

        }
        else
        {
            int charCount = txtAddGVDuration.Text.Count(x => x == ':');
            if (charCount == 1)
            {
                addDuration = txtAddGVDuration.Text + ":00";
            }
            else
            {
                addDuration = txtAddGVDuration.Text;
            }
        }

        if (hfCustomerId.Value != "0")
        {
            result = objRec.AddRecord(ddlTeam.SelectedValue, empId, txtAddUser.Text, clientId, selTask, selSubTask, addDuration, txtAddComments.Text, lblFriday.Text, DateTime.Now.ToString(), "Completed", "P");
            if (result == true)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "add Rec", "alert('Record added successfully)", true);
                hfCustomerId.Value = "0";

                dt = DisplayRecords(lblFriday.Text, ddlEmployee.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    gvRecordsFri.DataSource = dt;
                    gvRecordsFri.DataBind();
                    ViewState["dirFri"] = dt;
                    ViewState["sortdrF"] = "Asc";
                }
                else
                {
                    gvRecordsFri.DataSource = null;
                    gvRecordsFri.DataBind();
                }

                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
            }
        }
    }

    protected void btnSAGVAdd_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        
        string clientId = hfCustomerId.Value;
        TextBox txtClientSelect = (TextBox)grdrow.FindControl("txtSearchGvClientSA");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlSAAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlSAADDSubTask");
        TextBox txtAddComments = (TextBox)grdrow.FindControl("txtSAGVAddComments");
        Label txtAddUser = (Label)grdrow.FindControl("lblSAADDUser");
        string empId = ddlEmployee.SelectedValue;
        TextBox txtAddGVDuration = (TextBox)grdrow.FindControl("txtSAADDDuration");
        string addDuration = "00:00:00";
        if (hfCustomerId.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select client')", true);
            return;
        }

        if (string.IsNullOrEmpty(txtClientSelect.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client1", "alert('Please select client')", true);
            return;
        }

        string selTask = "0";
        string selSubTask = "0";
        if (txtClientSelect.Text == "Personal/Sick Time" || txtClientSelect.Text == "Inclement Weather" || txtClientSelect.Text == "Public Holiday" || txtClientSelect.Text == "Vacation" || txtClientSelect.Text == "Bereavement" || txtClientSelect.Text == "Jury Duty" || txtClientSelect.Text == "Maternity Leave" || txtClientSelect.Text == "Paternity Leave" || txtClientSelect.Text == "Medical Leave" || txtClientSelect.Text == "Comp off")
        {
            selTask = "0";
            selSubTask = "0";
        }
        else
        {
            if (Session["Location"].ToString() == "IND" && Session["access"].ToString() == "4")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No access", "alert('you are not authorized to add work time.')", true);
                return;
            }

            if (ddlAddTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
                return;
            }
            else
            {
                selTask = ddlAddTask.SelectedValue;
            }

            if (ddlAddSubTask.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subTask", "alert('Please select subtask')", true);
                return;
            }
            else
            {
                selSubTask = ddlAddSubTask.SelectedValue;
            }
        }

        if (!string.IsNullOrEmpty(txtAddGVDuration.Text))
        {
            if (txtAddGVDuration.Text.Contains('.'))
            {
                int dotCount = txtAddGVDuration.Text.Count(x => x == '.');
                if (dotCount == 1)
                {
                    decimal val = Math.Round(Convert.ToDecimal(txtAddGVDuration.Text), 2, MidpointRounding.AwayFromZero);
                    txtAddGVDuration.Text = val.ToString();
                }
            }
        }
        if (txtAddGVDuration.Text.Contains(':'))
        {
            if (newRegex2.IsMatch(txtAddGVDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                return;
            }
        }
        else if (newRegex.IsMatch(txtAddGVDuration.Text) == false)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
            return;
        }
        if (txtAddGVDuration.Text.Contains(':')==false)
        {

            decimal time = decimal.Parse(txtAddGVDuration.Text.Trim());
            var hours = time.ToString().Split('.')[0];
            var minutes = ((time * 60) % 60).ToString().Split('.')[0];
            var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

            addDuration = hours + ":" + minutes + ":" + seconds;

        }
        else
        {
            int charCount = txtAddGVDuration.Text.Count(x => x == ':');
            if (charCount == 1)
            {
                addDuration = txtAddGVDuration.Text + ":00";
            }
            else
            {
                addDuration = txtAddGVDuration.Text;
            }
        }

        if (hfCustomerId.Value != "0")
        {
            result = objRec.AddRecord(ddlTeam.SelectedValue, empId, txtAddUser.Text, clientId, selTask, selSubTask, addDuration, txtAddComments.Text, lblSat.Text, DateTime.Now.ToString(), "Completed", "P");
            if (result == true)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "add Rec", "alert('Record added successfully)", true);
                hfCustomerId.Value = "0";

                dt = DisplayRecords(lblSat.Text, ddlEmployee.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    gvRecordsSat.DataSource = dt;
                    gvRecordsSat.DataBind();
                    ViewState["dirSat"] = dt;
                    ViewState["sortdrSA"] = "Asc";
                }
                else
                {
                    gvRecordsSat.DataSource = null;
                    gvRecordsSat.DataBind();
                }

                BindWeekViewPivote(txtFrom.Text, txtTo.Text);
            }
        }
    }
    protected void txtSearchGvClient_TextChanged(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((TextBox)sender).NamingContainer;        
        TextBox txtCL = (TextBox)grdrow.FindControl("txtSearchGvClient");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlSAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlSADDSubTask");
        TextBox txtDuration = (TextBox)grdrow.FindControl("txtSADDDuration");
        
        dt = new DataTable();
        if (txtCL.Text == "Personal/Sick Time" || txtCL.Text == "Inclement Weather" || txtCL.Text == "Public Holiday" || txtCL.Text == "Vacation" || txtCL.Text == "Bereavement" || txtCL.Text == "Jury Duty" || txtCL.Text == "Maternity Leave" || txtCL.Text == "Paternity Leave" || txtCL.Text == "Medical Leave" || txtCL.Text == "Comp off")
        {
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
            txtDuration.Focus();
        }        
        else if (string.IsNullOrEmpty(txtCL.Text))
        {
            hfCustomerId.Value = "0";
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();            
        }
        else if (txtCL.Text == "Internal")
        {
            dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));            
        }        
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }

        if (dt.Rows.Count > 0)
        {
            ddlAddTask.DataSource = dt;
            ddlAddTask.DataTextField = "TL_Task";
            ddlAddTask.DataValueField = "TL_ID";
            ddlAddTask.DataBind();

            ddlAddTask.Items.Insert(0, "--Select task--");
            ddlAddTask.SelectedIndex = 0;
        }
        ddlAddSubTask.Items.Clear();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);

    }

    protected void txtSearchGvClientM_TextChanged(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((TextBox)sender).NamingContainer;
        //TextBox TxtMarks = (TextBox)sender; 
        //TextBox txtCl = (TextBox)gvRecordsSunday.Rows
        TextBox txtCL = (TextBox)grdrow.FindControl("txtSearchGvClientM");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlMAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlMADDSubTask");
        TextBox txtDuration = (TextBox)grdrow.FindControl("txtMADDDuration");

        dt = new DataTable();
        if (txtCL.Text == "Personal/Sick Time" || txtCL.Text == "Inclement Weather" || txtCL.Text == "Public Holiday" || txtCL.Text == "Vacation" || txtCL.Text == "Bereavement" || txtCL.Text == "Jury Duty" || txtCL.Text == "Maternity Leave" || txtCL.Text == "Paternity Leave" || txtCL.Text == "Medical Leave" || txtCL.Text == "Comp off")
        {
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
            txtDuration.Focus();
        }
        else if (string.IsNullOrEmpty(txtCL.Text))
        {
            hfCustomerId.Value = "0";
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
        }
        else if (txtCL.Text == "Internal")
        {
            dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }

        if (dt.Rows.Count > 0)
        {
            ddlAddTask.DataSource = dt;
            ddlAddTask.DataTextField = "TL_Task";
            ddlAddTask.DataValueField = "TL_ID";
            ddlAddTask.DataBind();

            ddlAddTask.Items.Insert(0, "--Select task--");
            ddlAddTask.SelectedIndex = 0;
        }
        ddlAddSubTask.Items.Clear();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void txtSearchGvClientT_TextChanged(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((TextBox)sender).NamingContainer;
        //TextBox TxtMarks = (TextBox)sender; 
        //TextBox txtCl = (TextBox)gvRecordsSunday.Rows
        TextBox txtCL = (TextBox)grdrow.FindControl("txtSearchGvClientT");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlTAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlTADDSubTask");
        TextBox txtDuration = (TextBox)grdrow.FindControl("txtTADDDuration");

        dt = new DataTable();
        if (txtCL.Text == "Personal/Sick Time" || txtCL.Text == "Inclement Weather" || txtCL.Text == "Public Holiday" || txtCL.Text == "Vacation" || txtCL.Text == "Bereavement" || txtCL.Text == "Jury Duty" || txtCL.Text == "Maternity Leave" || txtCL.Text == "Paternity Leave" || txtCL.Text == "Medical Leave" || txtCL.Text == "Comp off")
        {
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
            txtDuration.Focus();
        }
        else if (string.IsNullOrEmpty(txtCL.Text))
        {
            hfCustomerId.Value = "0";
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
        }
        else if (txtCL.Text == "Internal")
        {
            dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }

        if (dt.Rows.Count > 0)
        {
            ddlAddTask.DataSource = dt;
            ddlAddTask.DataTextField = "TL_Task";
            ddlAddTask.DataValueField = "TL_ID";
            ddlAddTask.DataBind();

            ddlAddTask.Items.Insert(0, "--Select task--");
            ddlAddTask.SelectedIndex = 0;
        }
        ddlAddSubTask.Items.Clear();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void txtSearchGvClientW_TextChanged(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((TextBox)sender).NamingContainer;
        //TextBox TxtMarks = (TextBox)sender; 
        //TextBox txtCl = (TextBox)gvRecordsSunday.Rows
        TextBox txtCL = (TextBox)grdrow.FindControl("txtSearchGvClientW");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlWAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlWADDSubTask");
        TextBox txtDuration = (TextBox)grdrow.FindControl("txtWADDDuration");

        dt = new DataTable();
        if (txtCL.Text == "Personal/Sick Time" || txtCL.Text == "Inclement Weather" || txtCL.Text == "Public Holiday" || txtCL.Text == "Vacation" || txtCL.Text == "Bereavement" || txtCL.Text == "Jury Duty" || txtCL.Text == "Maternity Leave" || txtCL.Text == "Paternity Leave" || txtCL.Text == "Medical Leave" || txtCL.Text == "Comp off")
        {
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
            txtDuration.Focus();
        }
        else if (string.IsNullOrEmpty(txtCL.Text))
        {
            hfCustomerId.Value = "0";
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
        }
        else if (txtCL.Text == "Internal")
        {
            dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }

        if (dt.Rows.Count > 0)
        {
            ddlAddTask.DataSource = dt;
            ddlAddTask.DataTextField = "TL_Task";
            ddlAddTask.DataValueField = "TL_ID";
            ddlAddTask.DataBind();

            ddlAddTask.Items.Insert(0, "--Select task--");
            ddlAddTask.SelectedIndex = 0;
        }
        ddlAddSubTask.Items.Clear();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void txtSearchGvClientTH_TextChanged(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((TextBox)sender).NamingContainer;
        //TextBox TxtMarks = (TextBox)sender; 
        //TextBox txtCl = (TextBox)gvRecordsSunday.Rows
        TextBox txtCL = (TextBox)grdrow.FindControl("txtSearchGvClientTH");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlTHAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlTHADDSubTask");
        TextBox txtDuration = (TextBox)grdrow.FindControl("txtTHADDDuration");

        dt = new DataTable();
        if (txtCL.Text == "Personal/Sick Time" || txtCL.Text == "Inclement Weather" || txtCL.Text == "Public Holiday" || txtCL.Text == "Vacation" || txtCL.Text == "Bereavement" || txtCL.Text == "Jury Duty" || txtCL.Text == "Maternity Leave" || txtCL.Text == "Paternity Leave" || txtCL.Text == "Medical Leave" || txtCL.Text == "Comp off")
        {
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
            txtDuration.Focus();
        }
        else if (string.IsNullOrEmpty(txtCL.Text))
        {
            hfCustomerId.Value = "0";
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
        }
        else if (txtCL.Text == "Internal")
        {
            dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }

        if (dt.Rows.Count > 0)
        {
            ddlAddTask.DataSource = dt;
            ddlAddTask.DataTextField = "TL_Task";
            ddlAddTask.DataValueField = "TL_ID";
            ddlAddTask.DataBind();

            ddlAddTask.Items.Insert(0, "--Select task--");
            ddlAddTask.SelectedIndex = 0;
        }
        ddlAddSubTask.Items.Clear();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void txtSearchGvClientF_TextChanged(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((TextBox)sender).NamingContainer;
        //TextBox TxtMarks = (TextBox)sender; 
        //TextBox txtCl = (TextBox)gvRecordsSunday.Rows
        TextBox txtCL = (TextBox)grdrow.FindControl("txtSearchGvClientF");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlFAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlFADDSubTask");
        TextBox txtDuration = (TextBox)grdrow.FindControl("txtFADDDuration");

        dt = new DataTable();
        if (txtCL.Text == "Personal/Sick Time" || txtCL.Text == "Inclement Weather" || txtCL.Text == "Public Holiday" || txtCL.Text == "Vacation" || txtCL.Text == "Bereavement" || txtCL.Text == "Jury Duty" || txtCL.Text == "Maternity Leave" || txtCL.Text == "Paternity Leave" || txtCL.Text == "Medical Leave" || txtCL.Text == "Comp off")
        {
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
            txtDuration.Focus();
        }
        else if (string.IsNullOrEmpty(txtCL.Text))
        {
            hfCustomerId.Value = "0";
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
        }
        else if (txtCL.Text == "Internal")
        {
            dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }

        if (dt.Rows.Count > 0)
        {
            ddlAddTask.DataSource = dt;
            ddlAddTask.DataTextField = "TL_Task";
            ddlAddTask.DataValueField = "TL_ID";
            ddlAddTask.DataBind();

            ddlAddTask.Items.Insert(0, "--Select task--");
            ddlAddTask.SelectedIndex = 0;
        }
        ddlAddSubTask.Items.Clear();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void txtSearchGvClientSA_TextChanged(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((TextBox)sender).NamingContainer;
        //TextBox TxtMarks = (TextBox)sender; 
        //TextBox txtCl = (TextBox)gvRecordsSunday.Rows
        TextBox txtCL = (TextBox)grdrow.FindControl("txtSearchGvClientSA");
        DropDownList ddlAddTask = (DropDownList)grdrow.FindControl("ddlSAAddTask");
        DropDownList ddlAddSubTask = (DropDownList)grdrow.FindControl("ddlSAADDSubTask");
        TextBox txtDuration = (TextBox)grdrow.FindControl("txtSAADDDuration");

        dt = new DataTable();
        if (txtCL.Text == "Personal/Sick Time" || txtCL.Text == "Inclement Weather" || txtCL.Text == "Public Holiday" || txtCL.Text == "Vacation" || txtCL.Text == "Bereavement" || txtCL.Text == "Jury Duty" || txtCL.Text == "Maternity Leave" || txtCL.Text == "Paternity Leave" || txtCL.Text == "Medical Leave" || txtCL.Text == "Comp off")
        {
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
            txtDuration.Focus();
        }
        else if (string.IsNullOrEmpty(txtCL.Text))
        {
            hfCustomerId.Value = "0";
            ddlAddTask.Items.Clear();
            ddlAddSubTask.Items.Clear();
        }
        else if (txtCL.Text == "Internal")
        {
            dt = objRec.GetGeneralTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }
        else
        {
            dt = objRec.GetClientTasks(Convert.ToInt32(ddlTeam.SelectedValue));
        }

        if (dt.Rows.Count > 0)
        {
            ddlAddTask.DataSource = dt;
            ddlAddTask.DataTextField = "TL_Task";
            ddlAddTask.DataValueField = "TL_ID";
            ddlAddTask.DataBind();

            ddlAddTask.Items.Insert(0, "--Select task--");
            ddlAddTask.SelectedIndex = 0;
        }
        ddlAddSubTask.Items.Clear();
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void ddlSGvClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlClient = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlClient.NamingContainer;
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlSGvClient");
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlSGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlSGvSubTask");

        dt = new DataTable();
        if (ddlGvClient.SelectedItem.Text == "Internal")
        {            
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

    protected void ddlMGvClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlClient = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlClient.NamingContainer;
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlMGvClient");
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlMGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlMGvSubTask");

        dt = new DataTable();
        if (ddlGvClient.SelectedItem.Text == "Internal")
        {
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

    protected void ddlTGvClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlClient = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlClient.NamingContainer;
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlTGvClient");
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlTGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlTGvSubTask");

        dt = new DataTable();
        if (ddlGvClient.SelectedItem.Text == "Internal")
        {
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

    protected void ddlWGvClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlClient = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlClient.NamingContainer;
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlWGvClient");
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlWGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlWGvSubTask");

        dt = new DataTable();
        if (ddlGvClient.SelectedItem.Text == "Internal")
        {
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

    protected void ddlTHGvClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlClient = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlClient.NamingContainer;
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlTHGvClient");
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlTHGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlTHGvSubTask");

        dt = new DataTable();
        if (ddlGvClient.SelectedItem.Text == "Internal")
        {
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

    protected void ddlFGvClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlClient = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlClient.NamingContainer;
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlFGvClient");
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlFGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlFGvSubTask");

        dt = new DataTable();
        if (ddlGvClient.SelectedItem.Text == "Internal")
        {
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

    protected void ddlSAGvClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlClient = (DropDownList)sender;
        GridViewRow Grow = (GridViewRow)ddlClient.NamingContainer;
        DropDownList ddlGvClient = (DropDownList)Grow.FindControl("ddlSAGvClient");
        DropDownList ddlGvTask = (DropDownList)Grow.FindControl("ddlSAGvTask");
        DropDownList ddlGvSubTask = (DropDownList)Grow.FindControl("ddlSAGvSubTask");

        dt = new DataTable();
        if (ddlGvClient.SelectedItem.Text == "Internal")
        {
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
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (lblHeading.Text == "No data available for the selected week!")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No data", "alert('Sorry! Cannot submit or approve. No time entries found for this period. If user was on PTO/Vacation please update that before submitting or approving the week!')", true);
                return;
            }

            string doj = Session["doj"].ToString();
            if (Session["username"].ToString() != ddlEmployee.SelectedItem.Text)
            {
                doj = "";
            }

            dt = objRec.CheckLastWeekSubmitStatus(ddlEmployee.SelectedItem.Text, Convert.ToDateTime(txtFrom.Text).AddDays(-7).ToShortDateString(), Convert.ToDateTime(txtTo.Text).AddDays(-7).ToShortDateString());

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["R_Submit"].ToString() == "1")
                { }
                else
                {
                    if (string.IsNullOrEmpty(doj) || Convert.ToDateTime(doj) >= Convert.ToDateTime(txtFrom.Text).AddDays(-7))
                    {  }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Last week", "alert('Please submit the last week data.')", true);
                        return;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(doj) || Convert.ToDateTime(doj) >= Convert.ToDateTime(txtFrom.Text).AddDays(-7))
                {   }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Last week Data", "alert('Please submit the last week data.')", true);
                    return;
                }
            }

            string empLocation = Session["Location"].ToString();
            string tm = Session["team"].ToString();

            //if(tm == "4" || tm =="6" || tm == "9" || tm == "29")
            if (empLocation == "IND")      
            {
                if (Convert.ToInt32(hfDayCount.Value) < 5)
                {
                    if (!string.IsNullOrEmpty(doj) && Convert.ToDateTime(doj) <= Convert.ToDateTime(txtFrom.Text).AddDays(-7))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Less data", "alert('Please enter time for all working days.')", true);
                        return;
                    }
                }

                if (Convert.ToInt32(hfLeaveCount.Value) > 0)
                {
                    string dtList = "";
                    int flagLeave = 0;
                    GetLeavesFromLeavePortal();
                    GetLeavesFromRTM();
                    if (dtLeaves.Rows.Count > 0)
                    {
                        if (dtRTMLeaves.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRTMLeaves.Rows)
                            {
                                DateTime leaveDate = Convert.ToDateTime(dr["R_TimeDate"]);
                                int RTMLeavetype = Convert.ToInt32(dr["CL_ClientName"]);
                                flagLeave = 0;
                                foreach (DataRow drPortal in dtLeaves.Rows)
                                {
                                    DateTime fr = Convert.ToDateTime(drPortal["from_date"]);
                                    DateTime tod = Convert.ToDateTime(drPortal["to_date"]);
                                    int leaveType = Convert.ToInt32(drPortal["leave_type"]);
                                    if (leaveDate >= fr && leaveDate <= tod && RTMLeavetype == leaveType)
                                    {
                                        flagLeave = 0;
                                        break;
                                    }
                                    else
                                    {
                                        flagLeave = 1;
                                    }
                                }

                                if (flagLeave == 1)
                                {
                                    dtList = dtList + ", " + leaveDate.ToString("MM/dd/yyyy");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(dtList))
                        {
                            dtList = dtList.Trim().TrimStart(',');
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Apply Leave", "alert('Please apply leaves in leave portal for the dates " + dtList + " to continue with timesheet submission')", true);
                            return;
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in dtRTMLeaves.Rows)
                        {
                            dtList = dtList + ", " + Convert.ToDateTime(dr["R_TimeDate"]).ToString("MM/dd/yyyy");
                        }
                        dtList = dtList.Trim().TrimStart(',');
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Apply Leave", "alert('Please apply leaves in leave portal for the dates " + dtList + " to continue with timesheet submission')", true);
                        return;
                    }
                }
            }
            
            mpeSubmitConfirm.Show();
        }
        catch (Exception)
        {

        }
        finally
        {
            BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        }
    }


    protected void btnReject_Click(object sender, EventArgs e)
    {
        result = objRec.RejectRecordsData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());
        if (result == true)
        {
            result = objRec.RejectLogsData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());

            result = objRec.RejectLeavesData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());

            BindAllData();
            btnSubmit.Text = "Submit";
            //btnSubmit.Enabled = false;
            btnApprove.Text = "Approve";
            btnSubmit.Attributes.Remove("Disabled");
            btnApprove.Attributes.Remove("Disabled");
            btnReject.Attributes.Add("Disabled","");

            EnableControls();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "submition failed", "alert('Rejected Successfully')", true);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "submition failed", "alert('Failed.. Please try again..')", true);
        }
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        result = objRec.ApproveRecordsData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());
        if (result == true)
        {
            result = objRec.ApproveLogsData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());

            result = objRec.ApproveLeavesData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());

            BindAllData();
            btnSubmit.Text = "Already Submitted";
            //btnSubmit.Enabled = false;
            btnApprove.Text = "Approved";
            btnSubmit.Attributes.Add("Disabled", "");
            btnApprove.Attributes.Add("Disabled", "");
            btnReject.Attributes.Remove("Disabled");
            DisableControls();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "submition failed", "alert('Approved Successfully')", true);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "submition failed", "alert('Failed.. Please try again..')", true);
        }
    }

    private void BindAllData()
    {
        //dt = DisplayAllRecords(txtFrom.Text, ddlEmployee.SelectedItem.Text, txtTo.Text);
        FetchDate(txtFrom.Text, txtTo.Text);
        DataTable dtRecords = new DataTable();
        dtRecords = DisplayRecordsNew(txtFrom.Text, txtTo.Text, ddlEmployee.SelectedItem.Text);
        foreach (var dtDate in lstDate)
        {
            //if (dt.Rows.Count > 0)
            //{
            //    var filteredData = dt.Select("R_TimeDate='" + dtDate + "'").CopyToDataTable();
            //}

            dt = new DataTable();

            //dt = DisplayRecords(dtDate, ddlEmployee.SelectedItem.Text);
            if (Convert.ToDateTime(dtDate).DayOfWeek == DayOfWeek.Sunday)
            {
                string expression = "R_Start_Date_Time='" + Convert.ToDateTime(dtDate).ToString("MM/dd/yyyy") + "'";

                var rows = dtRecords.Select(expression);

                if (rows.Any())
                {                    
                    dt = rows.CopyToDataTable();
                }
                lblSunday.Text = dtDate;
                if (dt.Rows.Count > 0)
                {

                    gvRecordsSunday.DataSource = dt;
                    gvRecordsSunday.DataBind();
                    ViewState["dirSun"] = dt;
                    ViewState["sortdrS"] = "Asc";
                   // upnlSun.Update();
                }
                else
                {

                    //lblSunday.Text = dtDate;
                    bindemptytable();
                    gvRecordsSunday.DataSource = dtEmpty;
                    gvRecordsSunday.DataBind();

                    gvRecordsSunday.ShowHeaderWhenEmpty = true;
                   // upnlSun.Update();
                }
            }

            if (Convert.ToDateTime(dtDate).DayOfWeek == DayOfWeek.Monday)
            {
                string expression = "R_Start_Date_Time='" + Convert.ToDateTime(dtDate).ToString("MM/dd/yyyy") + "'";

                var rows = dtRecords.Select(expression);

                if (rows.Any())
                {                    
                    dt = rows.CopyToDataTable();
                }
                lblMonday.Text = dtDate;
                if (dt.Rows.Count > 0)
                {

                    gvRecordsMonday.DataSource = dt;
                    gvRecordsMonday.DataBind();
                    ViewState["dirMon"] = dt;
                    ViewState["sortdr"] = "Asc";
                  //  upnlMon.Update();
                }
                else
                {

                    //lblMonday.Text = dtDate;
                    bindemptytable();
                    gvRecordsMonday.DataSource = dtEmpty;
                    gvRecordsMonday.DataBind();

                    gvRecordsMonday.ShowHeaderWhenEmpty = true;
                   // upnlMon.Update();
                }
               //ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + gvRecordsMonday.ClientID + "', 250, 1000 , 27 ,true); </script>", false);
            }

            if (Convert.ToDateTime(dtDate).DayOfWeek == DayOfWeek.Tuesday)
            {
                string expression = "R_Start_Date_Time='" + Convert.ToDateTime(dtDate).ToString("MM/dd/yyyy") + "'";

                var rows = dtRecords.Select(expression);

                if (rows.Any())
                {                    
                    dt = rows.CopyToDataTable();
                }
                lblTuesday.Text = dtDate;
                if (dt.Rows.Count > 0)
                {
                    gvRecordsTue.DataSource = dt;
                    gvRecordsTue.DataBind();
                    ViewState["dirTue"] = dt;
                    ViewState["sortdrT"] = "Asc";
                }
                else
                {

                    //lblTuesday.Text = dtDate;
                    bindemptytable();
                    gvRecordsTue.DataSource = dtEmpty;
                    gvRecordsTue.DataBind();

                    gvRecordsTue.ShowHeaderWhenEmpty = true;
                }
               // upnlTue.Update();
            }

            if (Convert.ToDateTime(dtDate).DayOfWeek == DayOfWeek.Wednesday)
            {
                string expression = "R_Start_Date_Time='" + Convert.ToDateTime(dtDate).ToString("MM/dd/yyyy") + "'";

                var rows = dtRecords.Select(expression);

                if (rows.Any())
                {                    
                    dt = rows.CopyToDataTable();
                }
                lblWed.Text = dtDate;
                if (dt.Rows.Count > 0)
                {
                    gvRecordsWed.DataSource = dt;
                    gvRecordsWed.DataBind();
                    ViewState["dirWed"] = dt;
                    ViewState["sortdrW"] = "Asc";
                }
                else
                {

                    bindemptytable();
                    gvRecordsWed.DataSource = dtEmpty;
                    gvRecordsWed.DataBind();

                    gvRecordsWed.ShowHeaderWhenEmpty = true;
                }
               // upnlWed.Update();
            }

            if (Convert.ToDateTime(dtDate).DayOfWeek == DayOfWeek.Thursday)
            {
                string expression = "R_Start_Date_Time='" + Convert.ToDateTime(dtDate).ToString("MM/dd/yyyy") + "'";

                var rows = dtRecords.Select(expression);

                if (rows.Any())
                {                    
                    dt = rows.CopyToDataTable();
                }
                lblThu.Text = dtDate;
                if (dt.Rows.Count > 0)
                {
                    gvRecordsThu.DataSource = dt;
                    gvRecordsThu.DataBind();
                    ViewState["dirThu"] = dt;
                    ViewState["sortdrTH"] = "Asc";
                }
                else
                {

                    bindemptytable();
                    gvRecordsThu.DataSource = dtEmpty;
                    gvRecordsThu.DataBind();

                    gvRecordsThu.ShowHeaderWhenEmpty = true;
                }
               // upnlThu.Update();
            }

            if (Convert.ToDateTime(dtDate).DayOfWeek == DayOfWeek.Friday)
            {
                string expression = "R_Start_Date_Time='" + Convert.ToDateTime(dtDate).ToString("MM/dd/yyyy") + "'";

                var rows = dtRecords.Select(expression);

                if (rows.Any())
                {                    
                    dt = rows.CopyToDataTable();
                }
                lblFriday.Text = dtDate;
                if (dt.Rows.Count > 0)
                {
                    gvRecordsFri.DataSource = dt;
                    gvRecordsFri.DataBind();
                    ViewState["dirFri"] = dt;
                    ViewState["sortdrF"] = "Asc";
                }
                else
                {

                    bindemptytable();
                    gvRecordsFri.DataSource = dtEmpty;
                    gvRecordsFri.DataBind();

                    gvRecordsFri.ShowHeaderWhenEmpty = true;
                }
               // upnlFri.Update();
            }

            if (Convert.ToDateTime(dtDate).DayOfWeek == DayOfWeek.Saturday)
            {
                string expression = "R_Start_Date_Time='" + Convert.ToDateTime(dtDate).ToString("MM/dd/yyyy") + "'";

                var rows = dtRecords.Select(expression);

                if (rows.Any())
                {                    
                    dt = rows.CopyToDataTable();
                }
                lblSat.Text = dtDate;
                if (dt.Rows.Count > 0)
                {
                    gvRecordsSat.DataSource = dt;
                    gvRecordsSat.DataBind();
                    ViewState["dirSat"] = dt;
                    ViewState["sortdrSA"] = "Asc";
                }
                else
                {

                    bindemptytable();
                    gvRecordsSat.DataSource = dtEmpty;
                    gvRecordsSat.DataBind();

                    gvRecordsSat.ShowHeaderWhenEmpty = true;
                }

                //upnlSat.Update();
            }

        }

            BindWeekViewPivote(txtFrom.Text, txtTo.Text);

        upnlSun.Update();
        upnlMon.Update();
        upnlTue.Update();
        upnlWed.Update();
        upnlThu.Update();
        upnlFri.Update();
        upnlSat.Update();

        if (ddlEmployee.SelectedIndex > 0)
        {
            dt = objRec.CheckSubmitStatus(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text);
            if (dt.Rows.Count > 0)
            {
                string submit = dt.Rows[0]["R_Submit"].ToString();
                if (submit == "1")
                {
                    btnSubmit.Text = "Already Submitted";
                    // btnSubmit.Enabled = false;
                    btnSubmit.Attributes.Add("Disabled", "");
                    DisableControls();
                }
                else
                {
                    btnSubmit.Text = "Submit";
                    btnSubmit.Attributes.Remove("Disabled");
                    EnableControls();
                }
                //else
                //{
                //    dt = objRec.CheckLeaveSubmitStatus(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text);
                //    if (dt.Rows.Count > 0)
                //    {
                //        string submit1 = dt.Rows[0]["LD_Submit"].ToString();

                //        if (submit1 == "1")
                //        {
                //            btnSubmit.Text = "Already Submitted";
                //            // btnSubmit.Enabled = false;
                //            btnSubmit.Attributes.Add("Disabled", "");
                //            DisableControls();
                //        }
                //        else
                //        {
                //            btnSubmit.Text = "Submit";
                //            btnSubmit.Attributes.Remove("Disabled");
                //            //btnSubmit.Enabled = true;
                //            EnableControls();
                //        }
                //    }
                //    else
                //    {
                //        btnSubmit.Text = "Submit";
                //        btnSubmit.Attributes.Remove("Disabled");
                //        //btnSubmit.Enabled = true;
                //        EnableControls();
                //    }
                //}

            }
            else
            {
                btnSubmit.Text = "Submit";
                btnSubmit.Attributes.Remove("Disabled");
                EnableControls();
            }
            //else
            //{
            //    dt = objRec.CheckLeaveSubmitStatus(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text);
            //    if (dt.Rows.Count > 0)
            //    {
            //        string submit1 = dt.Rows[0]["LD_Submit"].ToString();

            //        if (submit1 == "1")
            //        {
            //            btnSubmit.Text = "Already Submitted";
            //            // btnSubmit.Enabled = false;
            //            btnSubmit.Attributes.Add("Disabled", "");
            //            DisableControls();
            //        }
            //        else
            //        {
            //            btnSubmit.Text = "Submit";
            //            btnSubmit.Attributes.Remove("Disabled");
            //            //btnSubmit.Enabled = true;
            //            EnableControls();
            //        }
            //    }
            //    else
            //    {
            //        btnSubmit.Text = "Submit";
            //        btnApprove.Text = "Approve";
            //        btnSubmit.Attributes.Remove("Disabled");
            //        btnApprove.Attributes.Remove("Disabled");
            //        btnReject.Attributes.Add("Disabled", "");
            //        //btnSubmit.Enabled = true;
            //        EnableControls();
            //    }
            //}
        }

        upnlSelection.Update();
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Approve.aspx");
    }

    protected void btnSubmitYes_Click(object sender, EventArgs e)
    {
        result = objRec.SubmitRecordsData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());

        result = objRec.SubmitLogsData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());

        //result = objRec.SubmitLeavesData(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["UID"].ToString(), DateTime.Now.ToString());
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        //BindAllData();
        btnSubmit.Text = "Already Submitted";
        btnSubmit.Attributes.Add("Disabled", "");
        DisableControls();
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "submition Success", "alert('Data Submitted Successfully')", true);
    }

    protected void btnSubmitNo_Click(object sender, EventArgs e)
    {
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
        //BindAllData();
    }

    private void SaveData()
    {
        BindAllData();
    }
    protected void btnPrev_Click(object sender, ImageClickEventArgs e)
    {
        txtFrom.Text = Convert.ToDateTime(txtFrom.Text).AddDays(-7).ToShortDateString();
        txtTo.Text = Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString();

        BindAllData();
    }

    protected void btnNext_Click(object sender, ImageClickEventArgs e)
    {
        txtFrom.Text = Convert.ToDateTime(txtFrom.Text).AddDays(7).ToShortDateString();
        txtTo.Text = Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString();

        BindAllData();
    }

    protected void gvRecordsSunday_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtrslt = (DataTable)ViewState["dirSun"];
        if (dtrslt.Rows.Count > 0)
        {
            if (Convert.ToString(ViewState["sortdrS"]) == "Asc")
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                ViewState["sortdrS"] = "Desc";
            }
            else
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                ViewState["sortdrS"] = "Asc";
            }
            gvRecordsSunday.DataSource = dtrslt;
            gvRecordsSunday.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsMonday_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtrslt = (DataTable)ViewState["dirMon"];
        if (dtrslt.Rows.Count > 0)
        {
            if (Convert.ToString(ViewState["sortdr"]) == "Asc")
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                ViewState["sortdr"] = "Desc";
            }
            else
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                ViewState["sortdr"] = "Asc";
            }
            gvRecordsMonday.DataSource = dtrslt;
            gvRecordsMonday.DataBind();

        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    

    protected void gvRecordsTue_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtrslt = (DataTable)ViewState["dirTue"];
        if (dtrslt.Rows.Count > 0)
        {
            if (Convert.ToString(ViewState["sortdrT"]) == "Asc")
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                ViewState["sortdrT"] = "Desc";
            }
            else
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                ViewState["sortdrT"] = "Asc";
            }
            gvRecordsTue.DataSource = dtrslt;
            gvRecordsTue.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsWed_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtrslt = (DataTable)ViewState["dirWed"];
        if (dtrslt.Rows.Count > 0)
        {
            if (Convert.ToString(ViewState["sortdrW"]) == "Asc")
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                ViewState["sortdrW"] = "Desc";
            }
            else
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                ViewState["sortdrW"] = "Asc";
            }
            gvRecordsWed.DataSource = dtrslt;
            gvRecordsWed.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsThu_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtrslt = (DataTable)ViewState["dirThu"];
        if (dtrslt.Rows.Count > 0)
        {
            if (Convert.ToString(ViewState["sortdrTH"]) == "Asc")
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                ViewState["sortdrTH"] = "Desc";
            }
            else
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                ViewState["sortdrTH"] = "Asc";
            }
            gvRecordsThu.DataSource = dtrslt;
            gvRecordsThu.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvWeeklyView_Sorting(object sender, GridViewSortEventArgs e)
    {
        string filter="";
        DataTable dtrslt = (DataTable)ViewState["dirWk"];
        if (dtrslt.Rows.Count > 0)
        {
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdrWk"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    filter = "Customer LIKE '*Totals*'";
                    filter = Convert.ToString(filter);
                    dtrslt.DefaultView.RowFilter = filter;
                    ViewState["sortdrWk"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    filter = "Customer LIKE '*Totals*'";
                    filter = Convert.ToString(filter);
                    dtrslt.DefaultView.RowFilter = filter;
                    ViewState["sortdrWk"] = "Asc";
                }
                gvWeeklyView.DataSource = dtrslt;
                gvWeeklyView.DataBind();
            }
        }
    }

    protected void gvRecordsFri_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtrslt = (DataTable)ViewState["dirFri"];
        if (dtrslt.Rows.Count > 0)
        {
            if (Convert.ToString(ViewState["sortdrF"]) == "Asc")
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                ViewState["sortdrF"] = "Desc";
            }
            else
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                ViewState["sortdrF"] = "Asc";
            }
            gvRecordsFri.DataSource = dtrslt;
            gvRecordsFri.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    protected void gvRecordsSat_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtrslt = (DataTable)ViewState["dirSat"];
        if (dtrslt.Rows.Count > 0)
        {
            if (Convert.ToString(ViewState["sortdrSA"]) == "Asc")
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                ViewState["sortdrSA"] = "Desc";
            }
            else
            {
                dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                ViewState["sortdrSA"] = "Asc";
            }
            gvRecordsSat.DataSource = dtrslt;
            gvRecordsSat.DataBind();
        }
        BindWeekViewPivote(txtFrom.Text, txtTo.Text);
    }

    private void GetLeavesFromLeavePortal()
    {        
        try
        {
            dtLeaves = objLp.GetLeaves(ddlEmployee.SelectedValue, txtFrom.Text, txtTo.Text);
        }
        catch (Exception ex)
        {            
            
        }
    }

    private void GetLeavesFromRTM()
    {
        try
        {
            dtRTMLeaves = objRec.GetLeaves(ddlEmployee.SelectedItem.Text, txtFrom.Text, txtTo.Text);
        }
        catch (Exception)
        {

        }
    }
}