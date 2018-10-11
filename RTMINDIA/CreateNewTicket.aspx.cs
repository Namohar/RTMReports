using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Security.Principal;
using System.Data;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;

public partial class CreateNewTicket : System.Web.UI.Page
{
    clsTicketing objTicket = new clsTicketing();
    DataTable dt = new DataTable();
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
            System.Security.Principal.WindowsPrincipal user;
            user = new WindowsPrincipal(this.Request.LogonUserIdentity);
            dt = objTicket.fetchEmpId(user.Identity.Name);

            if (dt.Rows.Count > 0)
            {
                lblEmpId.Text = dt.Rows[0]["UL_Employee_Id"].ToString();
                lblUid.Text = dt.Rows[0]["UL_ID"].ToString();
                lblTeamId.Text = dt.Rows[0]["UL_Team_Id"].ToString();
                lblEmail.Text = dt.Rows[0]["UL_EmailId"].ToString();
                lblUserName.Text = dt.Rows[0]["UL_User_Name"].ToString();
                lblRptMgrEmail.Text = dt.Rows[0]["UL_RepMgrEmail"].ToString();
                DeleteImages();

                dt = new DataTable();
                dt = objTicket.fetchAccess(lblEmpId.Text);
                if (dt.Rows.Count > 0)
                {
                    int access = Convert.ToInt32(dt.Rows[0]["AL_AccessLevel"]);
                }
            }
            GetTicketTypes();
        }
    }

    private void GetTicketTypes()
    {
        dt = new DataTable();

        dt = objTicket.GetTicketType();
        ddlTicketType.DataSource = dt;
        ddlTicketType.DataTextField = "TP_Name";
        ddlTicketType.DataValueField = "TP_ID";
        ddlTicketType.DataBind();
        ddlTicketType.Items.Insert(0, "--Select--");
        ddlTicketType.SelectedIndex = 0;
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        int ticketId = 0;
        if (FileUpload1.HasFile)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "upload", "alert('Please click on upload button after selecting file to attach')", true);
            lblMessage.Text = "Please click on upload button after selecting file to attach";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            FileUpload1.Focus();
            return;
        }
        if (ddlTicketType.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Ticket type", "alert('Please select ticket type')", true);
            //lblMessage.Text = "Please select ticket type";
            //lblMessage.ForeColor = System.Drawing.Color.Red;
            ddlTicketType.Focus();
            return;
        }
        if (string.IsNullOrEmpty(txtTicketSummary.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ticket summary", "alert('Please enter ticket summary')", true);
            //lblMessage.Text = "Please enter ticket summary";
            //lblMessage.ForeColor = System.Drawing.Color.Red;
            txtTicketSummary.Focus();
            return;
        }
        if (string.IsNullOrEmpty(txtDetails.Value))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "details", "alert('Please enter Details')", true);
            //lblMessage.Text = "Please enter Details";
            //lblMessage.ForeColor = System.Drawing.Color.Red;
            txtDetails.Focus();
            return;
        }
        ticketId = objTicket.AddTicket(0, lblEmpId.Text, ddlTicketType.SelectedItem.Text, txtTicketSummary.Text.Replace("'", ""), txtDetails.Value.Replace("'", ""), DateTime.Now, "Not Assigned Yet", "No ETA set", "Priority Not Set", "Active", Convert.ToInt32(lblUid.Text));

        if (ticketId != 0)
        {
            bool result = objTicket.UpdateImages(ticketId, lblEmpId.Text);
            lblMessage.Text = "Ticket raised successfully you can view your ticket status in 'View your past tickets' page";
            lblMessage.ForeColor = System.Drawing.Color.Blue;
            // SendEmail(ticketId);
            string screen = "";
            if (GridView1.Rows.Count > 0)
            {
                screen = "Yes";
            }
            else
            {
                screen = "No";
            }
            string body = PopulateBody(ticketId.ToString(), lblUserName.Text, ddlTicketType.SelectedItem.Text, txtTicketSummary.Text, txtDetails.Value, screen);
            SendHtmlFormattedEmail(ticketId.ToString(), body);
            ClearAll();
        }
        else
        {
            lblMessage.Text = "Something went wrong please trhy again";
        }

    }

    private void SendEmail(int TicketId)
    {
        try
        {
            dt = new DataTable();
            //dt = objTicket.GetLeadEmails(Convert.ToInt32(lblTeamId.Text));

            MailMessage message1 = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            //message1.From = new MailAddress("BLR-RTM-Server@tangoe.com");
            message1.From = new MailAddress("RTM-Mailer@tangoe.com");
            message1.To.Add(new MailAddress("RTM-Support@tangoe.com"));

            if  (Session["Location"].ToString() != "IND" | Session["Location"].ToString() != "CHN")
            {
                message1.To.Add(new MailAddress("Rachel.Millette@tangoe.com"));
            }


            if (string.IsNullOrEmpty(lblEmail.Text) == false)
            {
                message1.CC.Add(new MailAddress(lblEmail.Text));
            }
            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        if (string.IsNullOrEmpty(dr["UL_EmailId"].ToString()) == false)
            //        {
            //            message1.CC.Add(new MailAddress(dr["UL_EmailId"].ToString()));
            //        }
            //    }
            //}
            if (Session["access"].ToString() == "4")
            {

                if (string.IsNullOrEmpty(lblRptMgrEmail.Text.ToString().Trim()) == false)
                {
                    message1.CC.Add(new MailAddress(lblRptMgrEmail.Text.ToString().Trim()));
                }

            }


            message1.Subject = "RTM Tickets: New Ticket # " + TicketId + " is created by user " + lblUserName.Text;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("A new ticket # " + TicketId + " is created by user " + lblUserName.Text + ".");
            sb.AppendLine("Ticket Type:" + ddlTicketType.SelectedItem.Text);
            sb.AppendLine("Ticket Summary:" + txtTicketSummary.Text);
            sb.AppendLine("Request Details:" + txtDetails.Value);
            if (GridView1.Rows.Count > 0)
            {
                sb.AppendLine("Screen shot attached: YES");
            }
            else
            {
                sb.AppendLine("Screen shot attached: NO");
            }

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

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            if (FileUpload1.HasFile)
            {
                UploadImages();
                BindGrid();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select File", "alert('Please select files to upload')", true);
            }
        }
        catch (Exception)
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
                            cmd.Parameters.AddWithValue("@I_TicketId", 0);
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

    private void BindGrid()
    {
        string constr = ConfigurationManager.AppSettings["conString"].ToString();
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "SELECT I_ID, I_Name FROM RTM_Ticket_Attachments where I_TicketId=0 and I_EmpId='" + lblEmpId.Text + "'";
                cmd.Connection = con;
                con.Open();
                GridView1.DataSource = cmd.ExecuteReader();
                GridView1.DataBind();
                con.Close();
            }
        }
    }

    private void ClearAll()
    {
        ddlTicketType.SelectedIndex = 0;
        txtTicketSummary.Text = string.Empty;
        txtDetails.Value = string.Empty;
        GridView1.DataSource = null;
        GridView1.DataBind();
    }

    private void DeleteImages()
    {
        bool result = objTicket.DeleteImages(lblEmpId.Text);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Contact.aspx");
    }

    public string PopulateBody(string ticketid, string userName, string type, string summary, string details, string screenshot)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(Server.MapPath("~/TicketMail.htm")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{UserName}", userName);
        body = body.Replace("{ticketId}", ticketid);
        body = body.Replace("{type}", type);
        body = body.Replace("{summary}", summary);
        body = body.Replace("{details}", details);
        body = body.Replace("{screenshot}", screenshot);
        return body;
    }


    private void SendHtmlFormattedEmail(string TicketId, string body)
    {
        dt = new DataTable();
        //dt = objTicket.GetLeadEmails(Convert.ToInt32(lblTeamId.Text));
        using (MailMessage mailMessage = new MailMessage())
        {
            SmtpClient smtp = new SmtpClient();
            //mailMessage.From = new MailAddress("BLR-RTM-Server@tangoe.com");
            mailMessage.From = new MailAddress("RTM-Mailer@tangoe.com");
            mailMessage.To.Add(new MailAddress("RTM-Support@tangoe.com"));
            //mailMessage.To.Add(new MailAddress("Lokesha.B@tangoe.com"));

            if (Session["Location"].ToString() != "IND" & Session["Location"].ToString() != "CHN")
            {
                mailMessage.To.Add(new MailAddress("Rachel.Millette@tangoe.com"));
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

            mailMessage.Subject = "RTM Tickets: New Ticket # " + TicketId + " is created by user " + lblUserName.Text;

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


    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        LinkButton lnk = (LinkButton)sender;
        GridViewRow currentRow = (GridViewRow)lnk.NamingContainer;
        int rowid = Convert.ToInt32(GridView1.DataKeys[currentRow.RowIndex].Value.ToString());
        string name = currentRow.Cells[0].Text;

        bool result = objTicket.DeleteAttachment("0", name, lblEmpId.Text);
        BindGrid();
    }
}