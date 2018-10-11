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

public partial class OutOfOffice : System.Web.UI.Page
{
    clsRealTimeReports objReal = new clsRealTimeReports();
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();
    string sQuery;
    SqlDBHelper objDB = new SqlDBHelper();
    DateTime temp;
    List<string> lstDate = new List<string>();
    List<string> lstUser = new List<string>();
    DataTable dtResult = new DataTable();
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());  
    SqlDataAdapter da;

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
            LoadReason();
            if (Session["access"].ToString() == "4")
            {
                rbUser.Visible = false;
                rbTeam.Visible = false;
            }
        }
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

    protected void rbUser_CheckedChanged(object sender, EventArgs e)
    {
        LoadEmp();
        dvAbsent.Visible = false;
    }
    protected void rbTeam_CheckedChanged(object sender, EventArgs e)
    {
        LoadTeams();
        dvAbsent.Visible = false;
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
        //CheckAbsents();
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

        if (Convert.ToDateTime(datepicker.Value) > Convert.ToDateTime(datepickerTo.Value))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "date", "alert('To date should be greater than From date')", true);
            return;
        }

        if (ddlEmp.SelectedIndex > 0)
        {
            if (Session["preference"].ToString() == "1")
            {
                CheckAbsentsNew();
            }
            else
            {
                CheckAbsents();
            }
           
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

    protected void btnAbsent_Click(object sender, EventArgs e)
    {
        int check = 0;
       
        if (ddlReasonNew.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Reason", "alert('Please Select Reason')", true);
            return;
        }
        foreach (GridViewRow row in GridView1.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkRow1 = (row.Cells[0].FindControl("CheckBox1") as CheckBox);
                if (chkRow1.Checked)
                {
                    if (string.IsNullOrEmpty((row.Cells[6].FindControl("txtAbsent") as TextBox).Text))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration", "alert('Please enter duration.')", true);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace((row.Cells[6].FindControl("txtAbsent") as TextBox).Text))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Duration1", "alert('Please enter duration.')", true);
                        return;
                    }
                }
            }
        }
        foreach (GridViewRow row in GridView1.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {

                CheckBox chkRow = (row.Cells[0].FindControl("CheckBox1") as CheckBox);
                if (chkRow.Checked)
                {
                    string name = (row.Cells[1].FindControl("lblName") as Label).Text;
                    string date = Convert.ToDateTime((row.Cells[2].FindControl("lblDate") as Label).Text).ToString("MM/dd/yyyy");
                    string reason = ddlReasonNew.SelectedValue;
                    string duration = (row.Cells[6].FindControl("txtAbsent") as TextBox).Text;



                    SqlParameter[] parameters = new SqlParameter[]
                      {
                         new SqlParameter("@user",name),
                         new SqlParameter("@reason",reason),
                         new SqlParameter("@date", date),
                         new SqlParameter("@createdBy", Session["UID"].ToString()),
                         new SqlParameter("@createdOn", DateTime.Now),
                         new SqlParameter("@duration", duration)
                      };
                    sQuery = "INSERT INTO RTM_LeaveDetails (LD_UserName, LD_LID, LD_Date, LD_CreatedBy, LD_CreatedOn, LD_Duration) VALUES (@user, @reason, @date, @createdBy, @createdOn, @duration)";
                    bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                    check = 1;
                }
            }
        }

        if (check == 1)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Save", "alert('Details Saved Successfully')", true);
            if (Session["preference"].ToString() == "1")
            {
                CheckAbsentsNew();
            }
            else
            {
                CheckAbsents();
            }
            //CheckAbsentsNew();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select", "alert('Please select at least one user to update leave record.')", true);
            //CheckAbsentsNew();
        }
    }

    private void BuildMissingLoginsTable()
    {
        dt = new DataTable();
        DataColumn dc;

        dc = new DataColumn("User");
        dt.Columns.Add(dc);

        dc = new DataColumn("MissedDate");
        dt.Columns.Add(dc);

        dc = new DataColumn("totalHours");
        dt.Columns.Add(dc);

        dc = new DataColumn("absentHours");
        dt.Columns.Add(dc);
    }

    private void CheckAbsentsNew()
    {
        DataRow dr;

        BuildMissingLoginsTable();

        //Check count of logins

        int noOfDays = Convert.ToInt32((Convert.ToDateTime(datepickerTo.Value) - Convert.ToDateTime(datepicker.Value)).TotalDays);
        checked
        {
            for (int i = 0; i <= noOfDays; i++)
            {
                DayOfWeek day = Convert.ToDateTime(datepicker.Value).AddDays(i).DayOfWeek;
                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                {
                    continue;
                }
                else
                {
                    lstDate.Add(Convert.ToDateTime(datepicker.Value).AddDays(i).ToShortDateString());
                    //WorkingDays = WorkingDays + 1;
                }
            }
        }

        foreach (var dtDate in lstDate)
        {
            if (rbUser.Checked == true)
            {
                ds = objReal.CheckAbsentUsers(ddlEmp.SelectedItem.Text, dtDate, "user", Session["preference"].ToString());
                if (ds.Tables["absent"].Rows.Count > 0)
                {
                    continue;
                }
                else
                {
                    dtResult = objReal.CheckUpdatedAbsents(ddlEmp.SelectedItem.Text, dtDate);
                    if (dtResult.Rows.Count > 0)
                    {
                        continue;
                    }
                    else
                    {
                        dr = dt.NewRow();
                        dr["User"] = ddlEmp.SelectedItem.Text;
                        dr["MissedDate"] = Convert.ToDateTime(dtDate).ToString("dddd, MMMM d, yyyy");
                        dr["totalHours"] = "0";
                        dr["absentHours"] = "";

                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                ds = objReal.CheckAbsentUsers(ddlEmp.SelectedValue, dtDate, "team", Session["preference"].ToString());
                if (ds.Tables["absent"].Rows.Count > 0)
                {
                    foreach (DataRow drRow in ds.Tables["absent"].Rows)
                    {
                        dtResult = objReal.CheckUpdatedAbsents(drRow["UL_User_Name"].ToString(), dtDate);
                        if (dtResult.Rows.Count > 0)
                        {
                            continue;
                        }
                        else
                        {
                            dr = dt.NewRow();
                            dr["User"] = drRow["UL_User_Name"];
                            dr["MissedDate"] = Convert.ToDateTime(dtDate).ToString("dddd, MMMM d, yyyy");
                            dr["totalHours"] = "0";
                            dr["absentHours"] = "";

                            dt.Rows.Add(dr);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        if (dt.Rows.Count > 0)
        {
            GridView1.DataSource = dt;
            GridView1.DataBind();
            GridView1.Visible = true;
            dvAbsent.Visible = true;
            //return;
        }
        else
        {
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No Data", "alert('No data found.')", true);
            GridView1.DataSource = null;
            GridView1.DataBind();
            dvAbsent.Visible = false;            
        }

    }

    private void LoadReason()
    {
        dt = new DataTable();
        da = new SqlDataAdapter("select * from dbo.RTM_Leave where L_Status =1", con);
        da.Fill(dt);

        ddlReasonNew.DataSource = dt;
        ddlReasonNew.DataTextField = "L_Type";
        ddlReasonNew.DataValueField = "L_ID";
        ddlReasonNew.DataBind();

        ddlReasonNew.Items.Insert(0, "--Select--");
        ddlReasonNew.SelectedIndex = 0;
    }

    private void CheckAbsents()
    {
        DataRow dr;
        decimal totalTaskDuration = 0;
        decimal totalLogDuration = 0;
        decimal totalDuration = 0;
        BuildMissingLoginsTable();

        //Check count of logins

        int noOfDays = Convert.ToInt32((Convert.ToDateTime(datepickerTo.Value) - Convert.ToDateTime(datepicker.Value)).TotalDays);
        checked
        {
            for (int i = 0; i <= noOfDays; i++)
            {
                DayOfWeek day = Convert.ToDateTime(datepicker.Value).AddDays(i).DayOfWeek;
                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                {
                    continue;
                }
                else
                {
                    lstDate.Add(Convert.ToDateTime(datepicker.Value).AddDays(i).ToShortDateString());
                    //WorkingDays = WorkingDays + 1;
                }
            }
        }

        foreach (var dtDate in lstDate)
        {
            if (rbUser.Checked == true)
            {
                dtResult = objReal.CheckUpdatedAbsents(ddlEmp.SelectedItem.Text, dtDate);
                if (dtResult.Rows.Count > 0)
                {
                    continue;
                }
                ds = objReal.TotalTask(ddlEmp.SelectedItem.Text, dtDate, "user");
                if (ds.Tables["task"].Rows.Count > 0)
                {
                    totalTaskDuration = Convert.ToDecimal(ds.Tables["task"].Rows[0]["hour"]) + (Convert.ToDecimal(ds.Tables["task"].Rows[0]["minute"]) / 60) + (Convert.ToDecimal(ds.Tables["task"].Rows[0]["seconds"]) / 3600);
                }
                ds = objReal.TotalLog(ddlEmp.SelectedItem.Text, dtDate, "user");
                if (ds.Tables["log"].Rows.Count > 0)
                {
                    totalLogDuration = Convert.ToDecimal(ds.Tables["log"].Rows[0]["hour"]) + (Convert.ToDecimal(ds.Tables["log"].Rows[0]["minute"]) / 60) + (Convert.ToDecimal(ds.Tables["log"].Rows[0]["seconds"]) / 3600);
                }
                totalDuration = decimal.Round(totalTaskDuration + totalLogDuration, 2, MidpointRounding.AwayFromZero);
                if (totalDuration < 8)
                {
                    dr = dt.NewRow();
                    dr["User"] = ddlEmp.SelectedItem.Text;
                    dr["MissedDate"] = Convert.ToDateTime(dtDate).ToString("dddd, MMMM d, yyyy");
                    dr["totalHours"] = totalDuration;
                    //dtResult = objReal.CheckUpdatedAbsents(ddlEmp.SelectedItem.Text, dtDate);
                    //if (dtResult.Rows.Count > 0)
                    //{
                    //    string leaveDuration = dtResult.Rows[0]["LD_Duration"].ToString();
                    //    if (string.IsNullOrEmpty(leaveDuration))
                    //    {
                    //        dr["absentHours"] = "8";
                    //    }
                    //    else
                    //    {
                    //        dr["absentHours"] = leaveDuration;
                    //    }
                    //}

                    dr["absentHours"] = "";
                    dt.Rows.Add(dr);

                }
                totalTaskDuration = 0;
                totalLogDuration = 0;
                totalDuration = 0;
            }
            else
            {
                //ds = objReal.CheckAbsentUsers(ddlEmp.SelectedValue, dtDate, "team", Session["preference"].ToString());
                ds = objReal.GetUsers(ddlEmp.SelectedValue);
                if (ds.Tables["absent"].Rows.Count > 0)
                {
                    foreach (DataRow drRow in ds.Tables["absent"].Rows)
                    {
                        dtResult = objReal.CheckUpdatedAbsents(drRow["UL_User_Name"].ToString(), dtDate);
                        if (dtResult.Rows.Count > 0)
                        {
                            continue;
                        }
                        ds = objReal.TotalTask(drRow["UL_User_Name"].ToString(), dtDate, "user");
                        if (ds.Tables["task"].Rows.Count > 0)
                        {
                            totalTaskDuration = Convert.ToDecimal(ds.Tables["task"].Rows[0]["hour"]) + (Convert.ToDecimal(ds.Tables["task"].Rows[0]["minute"]) / 60) + (Convert.ToDecimal(ds.Tables["task"].Rows[0]["seconds"]) / 3600);
                        }
                        ds = objReal.TotalLog(drRow["UL_User_Name"].ToString(), dtDate, "user");
                        if (ds.Tables["log"].Rows.Count > 0)
                        {
                            totalLogDuration = Convert.ToDecimal(ds.Tables["log"].Rows[0]["hour"]) + (Convert.ToDecimal(ds.Tables["log"].Rows[0]["minute"]) / 60) + (Convert.ToDecimal(ds.Tables["log"].Rows[0]["seconds"]) / 3600);
                        }
                        totalDuration = decimal.Round(totalTaskDuration + totalLogDuration, 2, MidpointRounding.AwayFromZero);
                        if (totalDuration < 8)
                        {
                            dr = dt.NewRow();
                            dr["User"] = drRow["UL_User_Name"].ToString();
                            dr["MissedDate"] = Convert.ToDateTime(dtDate).ToString("dddd, MMMM d, yyyy");
                            dr["totalHours"] = totalDuration;
                            //dtResult = objReal.CheckUpdatedAbsents(drRow["UL_User_Name"].ToString(), dtDate);
                            //if (dtResult.Rows.Count > 0)
                            //{
                            //    string leaveDuration = dtResult.Rows[0]["LD_Duration"].ToString();
                            //    if (string.IsNullOrEmpty(leaveDuration))
                            //    {
                            //        dr["absentHours"] = "8";
                            //    }
                            //    else
                            //    {
                            //        dr["absentHours"] = leaveDuration;
                            //    }
                            //}

                            dr["absentHours"] = "";
                            dt.Rows.Add(dr);
                        }
                        totalTaskDuration = 0;
                        totalLogDuration = 0;
                        totalDuration = 0;
                    }
                }
            }
        }

        if (dt.Rows.Count > 0)
        {
            DataView dv = dt.DefaultView;
            dv.Sort = "User desc";
            dt = dv.ToTable();
            GridView1.DataSource = dt;
            GridView1.DataBind();
            GridView1.Visible = true;
            dvAbsent.Visible = true;
            //return;
        }
        else
        {
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No Data", "alert('No data found.')", true);
            GridView1.DataSource = null;
            GridView1.DataBind();
            dvAbsent.Visible = false;
        }

    }

    
}