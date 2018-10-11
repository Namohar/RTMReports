using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.Drawing;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using DAL;

public partial class ViewTicketDetails : System.Web.UI.Page
{
    clsTicketing objTicket = new clsTicketing();
    clsTeam objTeam = new clsTeam();
    DataTable dt = new DataTable();
    SqlDBHelper objSqlDBHelper = new SqlDBHelper();

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
            int TID = Convert.ToInt32(HttpUtility.UrlDecode(objTeam.Decrypt(Request.QueryString["id"])));
            lblAccess.Text = Session["access"].ToString(); // HttpUtility.UrlDecode(objTeam.Decrypt(Request.QueryString["acc"]));
            lblUser.Text = Session["username"].ToString();// HttpUtility.UrlDecode(objTeam.Decrypt(Request.QueryString["user"]));
            lblTeam.Text = Session["team"].ToString(); // HttpUtility.UrlDecode(objTeam.Decrypt(Request.QueryString["team"]));
            GetTicketStatus();
            GetTicketTypes();
            FillAssignedToDDLNew();
            GetTicketDetails(TID);
            GetNotes();
            GetInternalNotes();

        }
    }

    private void GetTicketTypes()
    {
        dt = new DataTable();

        dt = objTicket.GetTicketType();
        ddlType.DataSource = dt;
        ddlType.DataTextField = "TP_Name";
        ddlType.DataValueField = "TP_ID";
        ddlType.DataBind();
        ddlType.Items.Insert(0, "--Select--");
        ddlType.SelectedIndex = 0;
    }

    private void GetTicketStatus()
    {
        dt = new DataTable();

        dt = objTicket.GetTicketStatus();
        ddlStatus.DataSource = dt;
        ddlStatus.DataTextField = "TS_Name";
        ddlStatus.DataValueField = "TS_ID";
        ddlStatus.DataBind();
    }


    private void GetTicketDetails(int tID)
    {
        dt = objTicket.getTicketDetails(tID);
        if (dt.Rows.Count > 0)
        {
            txtTicketId.Text = dt.Rows[0]["T_ID"].ToString();
            //ddlStatus.SelectedItem.Text = dt.Rows[0]["T_Status"].ToString();
            ddlStatus.ClearSelection();
            ddlStatus.Items.FindByText(dt.Rows[0]["T_Status"].ToString()).Selected = true;
            txtCreatedBY.Text = dt.Rows[0]["UL_User_Name"].ToString();
            lblEmail.Text = dt.Rows[0]["UL_EmailId"].ToString();
            lblTeam.Text = dt.Rows[0]["UL_Team_Id"].ToString();
            lblEmpId.Text = dt.Rows[0]["UL_Employee_Id"].ToString();
            ddlType.ClearSelection();
            ddlType.Items.FindByText(dt.Rows[0]["T_Ticket_Type"].ToString()).Selected = true;
            txtType.Text = dt.Rows[0]["T_Ticket_Type"].ToString();
            txtSummary.Text = dt.Rows[0]["T_Summary"].ToString();
            txtDetails.Value = dt.Rows[0]["T_Request_Details"].ToString();
            txtCreatedOn.Text = dt.Rows[0]["T_CreatedOn"].ToString();
            txtClosedOn.Text = dt.Rows[0]["T_ModifiedOn"].ToString();
            lblRptMgrEmail.Text = dt.Rows[0]["UL_RepMgrEmail"].ToString();


            if (dt.Rows[0]["T_Status"].ToString() == "Closed")
            {
                lblClosedOn.Visible = true;
                txtClosedOn.Visible = true;
            }
            else
            {
                lblClosedOn.Visible = false;
                txtClosedOn.Visible = false;
            }
            if (dt.Rows[0]["T_Assigned_To"].ToString() == "Not Assigned Yet")
            {
                lblAssigned.Text = dt.Rows[0]["T_Assigned_To"].ToString();
                lblAssigned.Visible = true;

            }
            else
            {
                
                if (ddlAssignedTo.Items.FindByText(dt.Rows[0]["T_Assigned_To"].ToString()) != null)
                {
                    ddlAssignedTo.ClearSelection();
                    ddlAssignedTo.Items.FindByText(dt.Rows[0]["T_Assigned_To"].ToString()).Selected = true;
                }
                //ddlAssignedTo.SelectedItem.Text = dt.Rows[0]["T_Assigned_To"].ToString();
                ddlAssignedTo.Visible = true;
            }

            txtETA.Text = dt.Rows[0]["T_ETA_Date_Time"].ToString();


            if (lblAccess.Text == "1")
            {
                lblHeader.Text = "Admin Console";
                ddlAssignedTo.Visible = true;

                GetAttachments(Convert.ToInt32(txtTicketId.Text));
                lblAttachments.Visible = true;
                gvAttachments.Visible = true;
                ddlType.Visible = true;
                txtType.Visible = false;
                lblInternal.Visible = true;
                txtInternal.Visible = true;
                txtInternal.Disabled = true;
                lblAddInternal.Visible = true;
                txtMessage.Visible = true;
                btnSend.Visible = true;
            }
            else if (lblAccess.Text != "1" && lblTeam.Text == "4")
            {
                lblHeader.Text = "Ticket Details";
                //ddlStatus.Enabled = false;
                GetAttachments(Convert.ToInt32(txtTicketId.Text));
                ddlAssignedTo.Enabled = false;
                txtETA.Enabled = false;
                ddlType.Visible = true;
                txtType.Visible = false;
            }
            else
            {
                lblHeader.Text = "Ticket Details";
                ddlStatus.Enabled = true;
                GetAttachments(Convert.ToInt32(txtTicketId.Text));
                ddlAssignedTo.Enabled = false;
                txtETA.Enabled = false;
                btnUpdate.Text = "Add";
                ddlType.Visible = false;
                txtType.Visible = true;
            }

        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            //btnUpdate.Enabled = false;
            UpdateTicket();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;

        }
        finally
        {
            // btnUpdate.Enabled = true;
        }

    }

    private void FillAssignedToDDL()
    {
        dt = new DataTable();

        dt = objTicket.getRTMTeam();
        if (dt.Rows.Count > 0)
        {
            ddlAssignedTo.DataSource = dt;
            ddlAssignedTo.DataValueField = "UL_ID";
            ddlAssignedTo.DataTextField = "UL_User_Name";
            ddlAssignedTo.DataBind();
            ddlAssignedTo.Items.Insert(0, "Please Select");
            ddlAssignedTo.SelectedIndex = 0;

        }
    }

    private void FillAssignedToDDLNew()
    {
        dt = new DataTable();

        dt = objTicket.getRTMSupportTeam();
        if (dt.Rows.Count > 0)
        {
            ddlAssignedTo.DataSource = dt;
            ddlAssignedTo.DataValueField = "ID";
            ddlAssignedTo.DataTextField = "Name";
            ddlAssignedTo.DataBind();
            ddlAssignedTo.Items.Insert(0, "Please Select");
            ddlAssignedTo.SelectedIndex = 0;

        }
    }

    private void GetAttachments(int Tid)
    {
        dt = new DataTable();
        dt = objTicket.getAttachments(Tid);

        if (dt.Rows.Count > 0)
        {
            gvAttachments.DataSource = dt;
            gvAttachments.DataBind();
        }
    }

    private void UpdateTicket()
    {
        if (FileUpload1.HasFile)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Upload", "alert('Please click on upload button after selecting file to attach')", true);
            //lblError.Text = "Please click on upload button after selecting file to attach";
            //lblError.ForeColor = System.Drawing.Color.Red;
            FileUpload1.Focus();
            return;
        }
        if (lblAccess.Text == "1")
        {
            if (ddlAssignedTo.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select user", "alert('Please select user to assign ticket')", true);
                //lblError.Text = "Please select user to assign ticket";
                //lblError.ForeColor = Color.Red;
                ddlAssignedTo.Focus();
                return;
            }
            bool result = objTicket.UpdateTicket(ddlStatus.SelectedItem.Text, ddlAssignedTo.SelectedItem.Text, txtETA.Text, Convert.ToInt32(txtTicketId.Text), ddlType.SelectedItem.Text);
            if (string.IsNullOrEmpty(txtAdd.Value) == false)
            {
                bool result2 = objTicket.UpdateNotes(txtAdd.Value.Replace("'", ""), Convert.ToInt32(txtTicketId.Text), lblUser.Text);
            }
            GetNotes();
            GetInternalNotes();
            txtAdd.Value = string.Empty;
            if (result == true)
            {
                string assign = "";
                if (ddlAssignedTo.SelectedIndex == 0)
                {
                    assign = lblAssigned.Text;
                }
                else
                {
                    assign = ddlAssignedTo.SelectedItem.Text;
                }
                string body = PopulateBody(txtTicketId.Text, lblUser.Text, ddlStatus.SelectedItem.Text, txtCreatedBY.Text, txtType.Text, txtSummary.Text, txtDetails.Value, assign, txtETA.Text, txtNotes.Value);
                SendHtmlFormattedEmail(txtTicketId.Text, body);
                //SendEmail(Convert.ToInt32(txtTicketId.Text));
                lblError.Text = "Updated successfully";
                lblError.ForeColor = Color.Blue;
            }
            else
            {
                lblError.Text = "Somthing went wrong.. Please try again";
                lblError.ForeColor = Color.Red;
            }
        }
        else if (lblTeam.Text == "4")
        {
            bool result = objTicket.UpdateTicket(ddlStatus.SelectedItem.Text, ddlAssignedTo.SelectedItem.Text, txtETA.Text, Convert.ToInt32(txtTicketId.Text), txtType.Text);
            if (string.IsNullOrEmpty(txtAdd.Value) == false)
            {
                bool result2 = objTicket.UpdateNotes(txtAdd.Value, Convert.ToInt32(txtTicketId.Text), lblUser.Text);
            }
            GetNotes();
            txtAdd.Value = string.Empty;
            if (result == true)
            {
                string assign = "";
                if (ddlAssignedTo.SelectedIndex == 0)
                {
                    assign = lblAssigned.Text;
                }
                else
                {
                    assign = ddlAssignedTo.SelectedItem.Text;
                }


                string body = PopulateBody(txtTicketId.Text, lblUser.Text, ddlStatus.SelectedItem.Text, txtCreatedBY.Text, txtType.Text, txtSummary.Text, txtDetails.Value, assign, txtETA.Text, txtNotes.Value);
                SendHtmlFormattedEmail(txtTicketId.Text, body);
                //SendEmail(Convert.ToInt32(txtTicketId.Text));
                lblError.Text = "Updated successfully";
                lblError.ForeColor = Color.Blue;
            }
            else
            {
                lblError.Text = "Somthing went wrong.. Please try again";
                lblError.ForeColor = Color.Red;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(txtAdd.Value))
            {
                lblError.Text = "Enter notes";
                txtAdd.Focus();
                lblError.ForeColor = Color.Red;
                return;
            }
            bool result1 = objTicket.UpdateStatus(ddlStatus.SelectedItem.Text, Convert.ToInt32(txtTicketId.Text));
            bool result2 = objTicket.UpdateNotes(txtAdd.Value, Convert.ToInt32(txtTicketId.Text), lblUser.Text);
            GetNotes();
            txtAdd.Value = string.Empty;
            if (result2 == true)
            {
                string assign = "";
                if (ddlAssignedTo.SelectedIndex == 0)
                {
                    assign = lblAssigned.Text;
                }
                else
                {
                    assign = ddlAssignedTo.SelectedItem.Text;
                }
                string body = PopulateBody(txtTicketId.Text, lblUser.Text, ddlStatus.SelectedItem.Text, txtCreatedBY.Text, txtType.Text, txtSummary.Text, txtDetails.Value, assign, txtETA.Text, txtNotes.Value);
                SendHtmlFormattedEmail(txtTicketId.Text, body);
                //SendEmail(Convert.ToInt32(txtTicketId.Text));
                lblError.Text = "Updated successfully";
                lblError.ForeColor = Color.Blue;
            }
            else
            {
                lblError.Text = "Somthing went wrong.. Please try again";
                lblError.ForeColor = Color.Red;
            }
        }
    }

    private void GetNotes()
    {
        dt = new DataTable();
        txtNotes.Value = string.Empty;
        dt = objTicket.getNotes(Convert.ToInt32(txtTicketId.Text));
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                txtNotes.Value += dr["notes"] + "\n\n";
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("ViewTickets.aspx");
    }

    protected void DownloadFile(object sender, EventArgs e)
    {
        dt = new DataTable();
        LinkButton btn = (LinkButton)(sender);
        string id = btn.CommandArgument;
        //Response.Write(string.Format("<script>window.open('{0}','_blank');</script>", "ViewAttachment.aspx?id="+ id));
        //dt = objTicket.DownloadFile(Convert.ToInt32(id));
        //if (dt.Rows.Count > 0)
        //{
        //    download(dt);
        //}

        string url = "ViewAttachment.aspx?id=" + id;
        string s = "window.open('" + url + "', 'popup_window', 'width=800,height=500,left=100,top=100,resizable=yes');";
        ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);

    }



    private void download(DataTable dt)
    {
        Byte[] bytes = (Byte[])dt.Rows[0]["I_Data"];
        Response.Buffer = true;
        Response.Charset = "";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = dt.Rows[0]["I_ContentType"].ToString();
        Response.AddHeader("content-disposition", "inline;filename="
        + dt.Rows[0]["I_Name"].ToString());
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }

    private void SendEmail(int TicketId)
    {
        try
        {
            dt = new DataTable();
            dt = objTicket.GetLeadEmails(Convert.ToInt32(lblTeam.Text));

            MailMessage message1 = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            //message1.From = new MailAddress("BLR-RTM-Server@tangoe.com");
            message1.From = new MailAddress("RTM-Mailer@tangoe.com");
            message1.To.Add(new MailAddress("RTM-Support@tangoe.com"));
            if (string.IsNullOrEmpty(lblEmail.Text) == false)
            {
                message1.CC.Add(new MailAddress(lblEmail.Text));
            }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrEmpty(dr["UL_EmailId"].ToString()) == false)
                    {
                        message1.CC.Add(new MailAddress(dr["UL_EmailId"].ToString()));
                    }
                }
            }
            message1.Subject = "RTM Tickets: Ticket # " + TicketId + " is modified by user " + lblUser.Text;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Ticket # " + TicketId + " is modified by user " + lblUser.Text + ".");
            sb.AppendLine("Ticket Status: " + ddlStatus.SelectedItem.Text);
            sb.AppendLine("Created by: " + txtCreatedBY.Text);
            sb.AppendLine("Ticket Type: " + txtType.Text);
            sb.AppendLine("Ticket Summary: " + txtSummary.Text);
            sb.AppendLine("Ticket Details: " + txtDetails.Value);
            if (ddlAssignedTo.SelectedIndex == 0)
            {
                sb.AppendLine("");
                sb.AppendLine("Assigned To: " + lblAssigned.Text);
            }
            else
            {
                sb.AppendLine("");
                sb.AppendLine("Assigned To: " + ddlAssignedTo.SelectedItem.Text);
            }
            sb.AppendLine("ETA: " + txtETA.Text);
            sb.AppendLine("Notes Updated in ticket: " + txtNotes.Value);
            sb.AppendLine("");
            sb.AppendLine("This is a system generated mail. Please do not reply to this mail.");
            message1.Body = sb.ToString();
            message1.IsBodyHtml = false;
            smtp.Port = 25;
            smtp.Host = "10.0.5.104";
            //smtp.Host = "outlook-south.tangoe.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = false;

            smtp.Send(message1);
        }
        catch (Exception)
        {

        }

    }

    public string PopulateBody(string ticketid, string userName, string status, string created, string type, string summary, string details, string assigned, string eta, string notes)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(Server.MapPath("~/TicketModifiedMail.htm")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{UserName}", userName);
        body = body.Replace("{ticketId}", ticketid);
        body = body.Replace("{status}", status);
        body = body.Replace("{created}", created);
        body = body.Replace("{type}", type);
        body = body.Replace("{summary}", summary);
        body = body.Replace("{details}", details);
        body = body.Replace("{assigned}", assigned);
        body = body.Replace("{eta}", eta);
        body = body.Replace("{notes}", notes);
        return body;
    }

    private void SendHtmlFormattedEmail(string TicketId, string body)
    {
        dt = new DataTable();
        //dt = objTicket.GetLeadEmails(Convert.ToInt32(lblTeam.Text));
        using (MailMessage mailMessage = new MailMessage())
        {

            SmtpClient smtp = new SmtpClient();
           // mailMessage.From = new MailAddress("BLR-RTM-Server@tangoe.com");
            mailMessage.From = new MailAddress("RTM-Mailer@tangoe.com");
            mailMessage.To.Add(new MailAddress("RTM-Support@tangoe.com"));
            //mailMessage.To.Add(new MailAddress("Lokesha.B@tangoe.com"));

            DataTable dtLocation = new DataTable();

            dtLocation = objSqlDBHelper.ExecuteSelectCommand("SELECT top 1 dbo.RTM_Team_List.T_Location "
                     + " FROM            dbo.RTM_User_List INNER JOIN "
                     + "    dbo.RTM_Team_List ON dbo.RTM_User_List.UL_Team_Id = dbo.RTM_Team_List.T_ID "
                     + " WHERE        (dbo.RTM_User_List.UL_Employee_Id = '" + lblEmpId.Text + "')", CommandType.Text);

            if (dtLocation.Rows.Count > 0)
            {

                if (dtLocation.Rows[0][0].ToString() != "IND" & dtLocation.Rows[0][0].ToString() != "CHN")
                {
                    mailMessage.To.Add(new MailAddress("Rachel.Millette@tangoe.com"));
                }

            }


     

            if (string.IsNullOrEmpty(lblEmail.Text) == false)
            {
                mailMessage.CC.Add(new MailAddress(lblEmail.Text));
            }


            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        if (string.IsNullOrEmpty(dr["UL_EmailId"].ToString()) == false)
            //        {
            //            mailMessage.CC.Add(new MailAddress(dr["UL_EmailId"].ToString()));
            //        }
            //    }
            //}

            if (Session["access"].ToString() == "4")
            {
                if (string.IsNullOrEmpty(lblRptMgrEmail.Text.ToString().Trim()) == false)
                {
                    mailMessage.CC.Add(new MailAddress(lblRptMgrEmail.Text.ToString().Trim()));
                }
            }

            if (ddlStatus.SelectedItem.Text == "Closed")
            {
                mailMessage.Subject = "RTM Tickets: Ticket # " + TicketId + " is Closed by user " + lblUser.Text;
            }
            else if (ddlStatus.SelectedItem.Text == "Re-Open")
            {
                mailMessage.Subject = "RTM Tickets: Ticket # " + TicketId + " is Re-Opened by user " + lblUser.Text;
            }
            else
            {
                mailMessage.Subject = "RTM Tickets: Ticket # " + TicketId + " is Modified by user " + lblUser.Text;
            }

            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            smtp.Port = 25;
            smtp.Host = "10.0.5.104";
            //smtp.Host = "outlook-south.tangoe.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = false;
            smtp.Send(mailMessage);
        }
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            if (FileUpload1.HasFile)
            {
                UploadImages();
                GetAttachments(Convert.ToInt32(txtTicketId.Text));
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select File", "alert('Please select files to upload')", true);
            }
        }
        catch (Exception)
        {


        }
        finally
        {

        }
    }

    private void UploadImages()
    {
        dynamic fileUploadControl = FileUpload1;
        foreach (HttpPostedFile postedFile in fileUploadControl.PostedFiles)
        {
            string filename = Path.GetFileName(postedFile.FileName);
            string contentType = postedFile.ContentType;
            using (Stream fs = postedFile.InputStream)
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    string constr = ConfigurationManager.AppSettings["conString"].ToString();
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string query = "insert into RTM_Ticket_Attachments values (@I_TicketId, @I_EmpId, @I_Name, @I_ContentType, @I_Data)";
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@I_TicketId", txtTicketId.Text);
                            cmd.Parameters.AddWithValue("@I_EmpId", lblEmpId.Text);
                            cmd.Parameters.AddWithValue("@I_Name", filename);
                            cmd.Parameters.AddWithValue("@I_ContentType", contentType);
                            cmd.Parameters.AddWithValue("@I_Data", bytes);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
        }
        fileUploadControl = string.Empty;
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtMessage.Value.Length == 0)
            {
                lblError.Text = "Please enter message";
                lblError.ForeColor = Color.Red;
                return;
            }
            string constr = ConfigurationManager.AppSettings["conString"].ToString();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "insert into RTM_Ticket_Internal_Notes values (@INT_TID, @INT_Message, @INT_CreatedOn, @INT_CreatedBy)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@INT_TID", txtTicketId.Text);
                    cmd.Parameters.AddWithValue("@INT_Message", txtMessage.Value);
                    cmd.Parameters.AddWithValue("@INT_CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@INT_CreatedBy", lblUser.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    GetInternalNotes();
                    txtMessage.Value = string.Empty;
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        SmtpClient smtp = new SmtpClient();
                        //mailMessage.From = new MailAddress("BLR-RTM-Server@tangoe.com");
                        mailMessage.From = new MailAddress("RTM-Mailer@tangoe.com");
                        mailMessage.To.Add(new MailAddress("RTM-Support@tangoe.com"));

                        mailMessage.Subject = "Internal Notes for Ticket# " + txtTicketId.Text;
                        mailMessage.Body = txtInternal.Value;
                        mailMessage.IsBodyHtml = false;

                        smtp.Port = 25;
                        smtp.Host = "10.0.5.104";
                        //smtp.Host = "outlook-south.tangoe.com";
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.EnableSsl = false;
                        smtp.Send(mailMessage);

                        lblError.Text = "Sent Successfully";
                        lblError.ForeColor = Color.Blue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }

    }

    private void GetInternalNotes()
    {
        dt = new DataTable();
        txtInternal.Value = string.Empty;
        dt = objTicket.getInternalNotes(Convert.ToInt32(txtTicketId.Text));
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                txtInternal.Value += dr["notes"] + "\n\n";
            }
        }
    }
}