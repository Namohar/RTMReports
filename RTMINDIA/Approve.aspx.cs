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
using System.Net.Mail;

public partial class Approve : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    DataTable dtResult = new DataTable();
    clsTSheetReports objTSheet = new clsTSheetReports();
    DataSet dtUser = new DataSet();
    clsApprove objApprove = new clsApprove();
    DateTime temp;
    bool result;
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
            LoadEmp();
            if (Session["select"] != null)
            {
                if (Session["select"].ToString() == "user")
                {
                    rbUser.Checked = true;
                    rbTeam.Checked = false;
                    LoadEmp();
                }
                else
                {
                    rbUser.Checked = false;
                    rbTeam.Checked = true;
                    LoadTeams();
                }
                
            }
            if (Session["apTeam"] != null)
            {
                if (Session["select"].ToString() == "team")
                {
                    ddlEmp.ClearSelection();
                    ddlEmp.Items.FindByValue(Session["apTeam"].ToString()).Selected = true;
                }
                Session.Remove("apTeam");
            }

            if (Session["apUser"] != null)
            {
                if (Session["select"].ToString() == "user")
                {
                    ddlEmp.ClearSelection();
                    ddlEmp.Items.FindByText(Session["apUser"].ToString()).Selected = true;
                }
                Session.Remove("apUser");
            }

            if (Session["apDate"] != null)
            {
                txtFrom.Text = Session["apDate"].ToString();
                Session.Remove("apDate");
                Display();

            }
            if (Session["select"] != null)
            {
                Session.Remove("select");
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
        ddlEmp.Items.Insert(0, "--Select Employee--");
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
        ddlEmp.Items.Insert(0, "--Select Team--");
        ddlEmp.SelectedIndex = 0;          
        
    }

    protected void rbUser_CheckedChanged(object sender, EventArgs e)
    {
        if (rbUser.Checked)
        {
            LoadEmp();
        }
    }

    protected void rbTeam_CheckedChanged(object sender, EventArgs e)
    {
        if (rbTeam.Checked)
        {
            LoadTeams();
        }
    }

    private void BuildTable()
    {
        dtResult = new DataTable();
        dtResult.Columns.Add("Username");
        dtResult.Columns.Add("TotalHours");
        dtResult.Columns.Add("SubmittedOn");
        dtResult.Columns.Add("ApprovedOn");
        dtResult.Columns.Add("Approve");
        dtResult.Columns.Add("Submit");
    }

    private void Display()
    {
        DataRow dr;
        
        decimal recDuration=0;
        decimal logDuration=0;
        decimal leaveDuration=0;
        
        if (rbUser.Checked)
        {
            BuildTable();
            //dr = dtResult.NewRow();
            dt = new DataTable();
            dt = objApprove.GetTotalHoursforApproval("user", ddlEmp.SelectedItem.Text, txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
            if (dt.Rows.Count > 0)
            {
                dr = dtResult.NewRow();
                dr["Username"] = dt.Rows[0]["R_User_Name"];
                dr["TotalHours"] = dt.Rows[0]["totalDuration"];
                dr["SubmittedOn"] = dt.Rows[0]["R_SubmittedOn"];
                dr["ApprovedOn"] = dt.Rows[0]["R_ApprovedOn"];
                dr["Approve"] = dt.Rows[0]["R_Approve"];
                dr["Submit"] = dt.Rows[0]["R_Submit"];

                dtResult.Rows.Add(dr);

            }
            //dt = objApprove.GetTotalWorkHoursForApproval("user", ddlEmp.SelectedItem.Text, txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
            //if (dt.Rows.Count > 0)
            //{
            //    if (!string.IsNullOrEmpty(dt.Rows[0]["hours"].ToString()))
            //    {
            //        recDuration = Convert.ToDecimal(dt.Rows[0]["hours"]) + (Convert.ToDecimal(dt.Rows[0]["minutes"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
            //    }

            //    dt = objApprove.GetApprovalStatus("user", ddlEmp.SelectedItem.Text, txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
            //    if (dt.Rows.Count > 0)
            //    {
            //        dr["SubmittedOn"] = dt.Rows[0]["R_SubmittedOn"];
            //        dr["ApprovedOn"] = dt.Rows[0]["R_ApprovedOn"];
            //        dr["Approve"] = dt.Rows[0]["R_Approve"];
            //        dr["Submit"] = dt.Rows[0]["R_Submit"];                    
            //    }
            //}

            //dt = objApprove.GetTotalLogHoursForApproval("user", ddlEmp.SelectedItem.Text, txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
            //if (dt.Rows.Count > 0)
            //{
            //    if (!string.IsNullOrEmpty(dt.Rows[0]["hour"].ToString()))
            //    {
            //        logDuration = Convert.ToDecimal(dt.Rows[0]["hour"]) + (Convert.ToDecimal(dt.Rows[0]["minute"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
            //    }
            //    dr["SubmittedOn"] = dt.Rows[0]["LA_SubmittedOn"];
            //    dr["ApprovedOn"] = dt.Rows[0]["LA_ApprovedOn"];
            //    dr["Approve"] = dt.Rows[0]["LA_Approve"];
            //    dr["Submit"] = dt.Rows[0]["LA_Submit"];
            //}           

            //dr["Username"] = ddlEmp.SelectedItem.Text;
            //dr["TotalHours"] = decimal.Round(recDuration, 2, MidpointRounding.AwayFromZero) + decimal.Round(logDuration, 2, MidpointRounding.AwayFromZero) + decimal.Round(leaveDuration, 2, MidpointRounding.AwayFromZero);

            //dtResult.Rows.Add(dr);
            //recDuration = 0;
            //logDuration = 0;
            //leaveDuration = 0;
        }
        else
        {
            BuildTable();

            //dt = new DataTable();
            //dt = objApprove.GetTotalHoursforApproval("team", ddlEmp.SelectedValue, txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow drRow in dt.Rows)
            //    {
            //        dr = dtResult.NewRow();

            //        dr["Username"] = drRow["R_User_Name"];
            //        dr["TotalHours"] = drRow["totalDuration"];
            //        dr["SubmittedOn"] = drRow["R_SubmittedOn"];
            //        dr["ApprovedOn"] = drRow["R_ApprovedOn"];
            //        dr["Approve"] = drRow["R_Approve"];
            //        dr["Submit"] = drRow["R_Submit"];

            //        dtResult.Rows.Add(dr);

            //    }
            //}
            dtUser = objReal.GetUsers(ddlEmp.SelectedValue);
            if (dtUser.Tables["absent"].Rows.Count > 0)
            {
                foreach (DataRow drUser in dtUser.Tables["absent"].Rows)
                {
                    dt = new DataTable();
                    dt = objApprove.GetTotalHoursforApproval("user", drUser["UL_User_Name"].ToString(), txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow drRow in dt.Rows)
                        {
                            dr = dtResult.NewRow();

                            dr["Username"] = drRow["R_User_Name"];
                            dr["TotalHours"] = drRow["totalDuration"];
                            dr["SubmittedOn"] = drRow["R_SubmittedOn"];
                            dr["ApprovedOn"] = drRow["R_ApprovedOn"];
                            dr["Approve"] = drRow["R_Approve"];
                            dr["Submit"] = drRow["R_Submit"];

                            dtResult.Rows.Add(dr);
                        }
                    }
                    
                }
            }
            //dtUser = objReal.GetUsers(ddlEmp.SelectedValue);
            //if (dtUser.Tables["absent"].Rows.Count > 0)
            //{
            //    foreach (DataRow drUser in dtUser.Tables["absent"].Rows)
            //    {
            //        dr = dtResult.NewRow();

            //        dt = objApprove.GetTotalWorkHoursForApproval("user", drUser["UL_User_Name"].ToString(), txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
            //        if (dt.Rows.Count > 0)
            //        {
            //            if (!string.IsNullOrEmpty(dt.Rows[0]["hours"].ToString()))
            //            {
            //                recDuration = Convert.ToDecimal(dt.Rows[0]["hours"]) + (Convert.ToDecimal(dt.Rows[0]["minutes"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
            //            }

            //            dt = objApprove.GetApprovalStatus("user", drUser["UL_User_Name"].ToString(), txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
            //            if (dt.Rows.Count > 0)
            //            {                            
            //                dr["SubmittedOn"] = dt.Rows[0]["R_SubmittedOn"];
            //                dr["ApprovedOn"] = dt.Rows[0]["R_ApprovedOn"];
            //                dr["Approve"] = dt.Rows[0]["R_Approve"];
            //                dr["Submit"] = dt.Rows[0]["R_Submit"];
            //            }
            //            else
            //            {
            //                dr["SubmittedOn"] = "";
            //                dr["ApprovedOn"] = "";
            //                dr["Approve"] = "";
            //                dr["Submit"] = "";
            //            }                        
            //        }

            //        dt = objApprove.GetTotalLogHoursForApproval("user", drUser["UL_User_Name"].ToString(), txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString());
            //        if (dt.Rows.Count > 0)
            //        {
            //            if (!string.IsNullOrEmpty(dt.Rows[0]["hour"].ToString()))
            //            {
            //                logDuration = Convert.ToDecimal(dt.Rows[0]["hour"]) + (Convert.ToDecimal(dt.Rows[0]["minute"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
            //            }
            //            dr["SubmittedOn"] = dt.Rows[0]["LA_SubmittedOn"];
            //            dr["ApprovedOn"] = dt.Rows[0]["LA_ApprovedOn"];
            //            dr["Approve"] = dt.Rows[0]["LA_Approve"];
            //            dr["Submit"] = dt.Rows[0]["LA_Submit"];
            //        }

                    
            //        dr["Username"] = drUser["UL_User_Name"].ToString();
            //        dr["TotalHours"] = decimal.Round(recDuration, 2, MidpointRounding.AwayFromZero) + decimal.Round(logDuration, 2, MidpointRounding.AwayFromZero) + decimal.Round(leaveDuration, 2, MidpointRounding.AwayFromZero);

            //        dtResult.Rows.Add(dr);

            //        recDuration = 0;
            //        logDuration = 0;
            //        leaveDuration = 0;
            //    }
            //}
        }

        if (dtResult.Rows.Count > 0)
        {
            gvList.DataSource = dtResult;
            gvList.DataBind();
            dateDisplay.Visible = true;
            lblFromDate.Text = txtFrom.Text;
            lblToDate.Text = Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString();
        }
                
    }
    protected void btnDisplay_Click(object sender, EventArgs e)
    {
        if (ddlEmp.SelectedIndex == 0)
        {
            if (rbUser.Checked)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select emp", "alert('Please select Employee')", true);
                return;
            }
            else
            {                
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select team", "alert('Please select Team')", true);
                return;                
            }
        }
        if (DateTime.TryParse(txtFrom.Text, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "from date", "alert('Please select valid from date')", true);
            return;
        }

        //if (DateTime.TryParse(txtTo.Text, out temp))
        //{ }
        //else
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "to date", "alert('Please select valid to date')", true);
        //    return;
        //}

        //if (Convert.ToDateTime(txtFrom.Text) > Convert.ToDateTime(txtTo.Text))
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "to date", "alert('To date should be greated than from date')", true);
        //    return;
        //}

        Display();
        
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        Label lblEmployee = (Label)grdrow.FindControl("lblEmployee");
        Label lblHours = (Label)grdrow.FindControl("lblHours");

        if (lblHours.Text == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Zero", "alert('Sorry! Cannot submit or approve. No time entries found for this period. If user was on PTO/Vacation please update that before submitting or approving the week!')", true);
            return;
        }

        result = objRec.ApproveRecordsData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());
        result = objRec.ApproveLogsData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());

        result = objRec.UpdateSubmittedOn(lblEmployee.Text, lblFromDate.Text, lblToDate.Text);
        //result = objRec.ApproveLeavesData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());

        Display();
       

    }
    protected void btnReject_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((Button)sender).NamingContainer;
        Label lblEmployee = (Label)grdrow.FindControl("lblEmployee");

        result = objRec.RejectRecordsData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());
       
        result = objRec.RejectLogsData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());

        result = objRec.RejectLeavesData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());

        dt = objApprove.GetTeam(lblEmployee.Text);
        if (dt.Rows.Count > 0)
        {
            MailMessage message1 = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            message1.From = new MailAddress("RTM-Mailer@tangoe.com");
            message1.To.Add(new MailAddress(dt.Rows[0]["UL_EmailId"].ToString()));

            message1.Subject = "RTM: Timesheet records rejected";
            message1.Body = "Hi, " + Environment.NewLine + " Your timesheet for Week " + lblFromDate.Text + " to " + Convert.ToDateTime(lblFromDate.Text).AddDays(6).ToShortDateString() + " is rejected. " + Environment.NewLine + "Thanks, " + Environment.NewLine + " RTM Support";
            message1.IsBodyHtml = true;

            smtp.Port = 25;
            smtp.Host = "10.0.5.104";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = false;

            smtp.Send(message1);
        }
        Display();
            
        
    }
    protected void lnkView_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        Label lblEmployee = (Label)grdrow.FindControl("lblEmployee");
        if (rbTeam.Checked)
        {
            Session["select"] = "team";
            Session["apTeam"] = ddlEmp.SelectedValue;
            Session["apUser"] = lblEmployee.Text;
            Session["apDate"] = lblFromDate.Text;
            Response.Redirect("~/RecordsWeeklyView.aspx");
        }
        else
        {
            dt = objApprove.GetTeam(ddlEmp.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                Session["select"] = "user";
                Session["apTeam"] = dt.Rows[0]["UL_Team_Id"].ToString();
                Session["apUser"] = lblEmployee.Text;
                Session["apDate"] = lblFromDate.Text;
                Response.Redirect("~/RecordsWeeklyView.aspx");
            }
        }
    }
    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblApproveStatus = (Label)e.Row.FindControl("lblApproveStatus");
            Label lblSubmitStatus = (Label)e.Row.FindControl("lblSubmitStatus");
            Button btnApprove = (Button)e.Row.FindControl("btnApprove");
            Button btnReject = (Button)e.Row.FindControl("btnReject");
            CheckBox chkRow = (CheckBox)e.Row.FindControl("chkRow");

            if (lblSubmitStatus.Text == "1")
            {
                btnReject.Attributes.Remove("Disabled");
                btnReject.Text = "Reject";
            }

            

            if (lblApproveStatus.Text == "1")
            {
                btnApprove.Attributes.Add("Disabled", "");
                btnReject.Attributes.Remove("Disabled");
                btnReject.Text = "Unapprove";
                chkRow.Enabled = false;
            }
            else if (lblSubmitStatus.Text == "0" && lblApproveStatus.Text == "0")
            {
                    btnReject.Attributes.Add("Disabled", "");
                    btnReject.Text = "Reject";
                    chkRow.Enabled = false;
                    btnApprove.Attributes.Add("Disabled", "");
            }
            else 
            {
                btnApprove.Attributes.Remove("Disabled");
                chkRow.Visible = true;
                if (lblSubmitStatus.Text != "1")
                {
                    btnReject.Attributes.Add("Disabled", "");
                    btnReject.Text = "Reject";
                }
            }

        }
    }
    protected void btnApproveAll_Click(object sender, EventArgs e)
    {
        int flag = 0;
        foreach (GridViewRow row in gvList.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                if (chkRow.Checked)
                {
                    flag = 1;
                }
            }
        }

        if (flag == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select rows", "alert('Please select atleast one user to approve.')", true);
            return;
        }

        foreach (GridViewRow row in gvList.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                Label lblEmployee = (row.Cells[0].FindControl("lblEmployee") as Label); 
                if (chkRow.Checked)
                {
                    result = objRec.ApproveRecordsData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());
                   
                    result = objRec.ApproveLogsData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());

                    result = objRec.ApproveLeavesData(lblEmployee.Text, lblFromDate.Text, lblToDate.Text, Session["UID"].ToString(), DateTime.Now.ToString());
                                          
                   
                }
            }
        }

        Display();

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select approve", "alert('Selected users data approved.')", true);

    }
}