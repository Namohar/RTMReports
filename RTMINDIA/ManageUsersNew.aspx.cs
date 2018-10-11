using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using BAL;
using System.IO;
using System.Text;
using System.Net.Mail;

public partial class ManageUsersNew : System.Web.UI.Page
{
    clsAccount objAccount = new clsAccount();
    DataTable dt = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
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
            if (Session["access"].ToString() == "1")
            {
                lblOr.Visible = true;
                lblLastName.Visible = true;
                txtLastName.Visible = true;
                dvHRIS.Visible = true;
            }
            BindTeam();
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtEmpId.Text))
            {
                dt = objAccount.SearchHRIS(txtEmpId.Text, txtLastName.Text, "empId");
            }
            else if (!string.IsNullOrEmpty(txtLastName.Text))
            {
                dt = objAccount.SearchHRIS(txtEmpId.Text, txtLastName.Text, "lastName");
            }
            else
            {
                lblResult.Text = "Please enter search criteria";
                lblResult.ForeColor = Color.Red;
                return;
            }
            if (dt.Rows.Count > 0)
            {
                gvUsers.DataSource = dt;
                gvUsers.DataBind();
                dvsave.Visible = true;
                lblResult.Text = "Please see the employee data in the grid below!";
                lblResult.ForeColor = Color.Blue;
            }
            else
            {
                gvUsers.DataSource = null;
                gvUsers.DataBind();
                dvsave.Visible = false;
                lblResult.Text = "Sorry, employee data not found in HRIS records. Please contact People Operations (HR).";
                lblResult.ForeColor = Color.Blue;
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = ex.Message;
            lblResult.ForeColor = Color.Red;           
        }
        
    }
    protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox chkSelect = (CheckBox)e.Row.FindControl("chkRow");
            Label lblCheckemp = (Label)e.Row.FindControl("lblGvCheckempId");
            Label lblVersion = (Label)e.Row.FindControl("lblGvVersion");
            Label lblStatus = (Label)e.Row.FindControl("lblGvStatus");
            TextBox txtSys = (TextBox)e.Row.FindControl("txtSystemName");


            if (!string.IsNullOrEmpty(lblCheckemp.Text))
            {
                if (!string.IsNullOrEmpty(lblVersion.Text))
                {
                    lblStatus.Text = "Installed";
                    chkSelect.Enabled = false;
                }
                else
                {
                    lblStatus.Text = "Not Installed";
                }
            }
            else
            {
                lblStatus.Text = "Not Installed";
            }

            if (!string.IsNullOrEmpty(txtSys.Text))
            {
                txtSys.Enabled = false;
            }
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int flag = 0;
            if (ddlTeam.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select team", "alert('Select Team')", true);
                return;
            }
            foreach (GridViewRow row in gvUsers.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                    if (chkRow.Checked)
                    {
                        string SysName = (row.Cells[6].FindControl("txtSystemName") as TextBox).Text;

                        if (SysName.ToLower().StartsWith("corp\\") == false)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Sys Name", "alert('Enter valid System/Network UserName')", true);
                            return;
                        }

                        string empId = (row.Cells[1].FindControl("lblGvEmpId") as Label).Text;
                        string firstname = (row.Cells[2].FindControl("lblGvFirstName") as Label).Text;
                        string lastname = (row.Cells[3].FindControl("lblGvLastName") as Label).Text;
                        string username = firstname + " " + lastname;
                        string managerId = (row.Cells[5].FindControl("lblGvManagerId") as Label).Text;
                        string managerEmail = (row.Cells[5].FindControl("lblManagerEmail") as Label).Text;
                        string email = (row.Cells[4].FindControl("lblGvEmailId") as Label).Text;
                        string checkEmp = (row.Cells[8].FindControl("lblGvCheckempId") as Label).Text;

                        if (string.IsNullOrEmpty(checkEmp))
                        {
                            bool checkUser = objAccount.CheckUserName(username);
                            if (checkUser == true)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select team", "alert('Another user with same name exist in RTM')", true);
                                return;
                            }

                            bool result = objAccount.AddNewUser(ddlTeam.SelectedValue, empId, username, SysName, string.Empty, email, Session["UID"].ToString(), managerId, managerEmail,"","","","","");
                            if (result == true)
                            {
                                result = objAccount.AddAccessLevel(empId, Session["UID"].ToString());
                                if (result == true)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine("Hi Team,");
                                    sb.AppendLine("Details of User added into RTM database is as follows");
                                    sb.AppendLine("Employee Id:" + empId);
                                    sb.AppendLine("Employee Name:" + username);
                                    sb.AppendLine("Employee email id:"+email );
                                    sb.AppendLine("System Name:"+ SysName);
                                    sb.AppendLine("Team Name:"+ ddlTeam.SelectedItem.Text);
                                    sb.AppendLine("Reporting Manager Email Id:"+ managerEmail);
                                    sb.AppendLine("");
                                    sb.AppendLine("Please update the gender information as well.");
                                    sb.AppendLine("");
                                    sb.AppendLine("Thanks,");
                                    sb.AppendLine("RTM Support");

                                    MailMessage message1 = new MailMessage();
                                    SmtpClient smtp = new SmtpClient();

                                    message1.From = new MailAddress("RTM-Mailer@tangoe.com");
                                    message1.To.Add(new MailAddress("RTM-Support@tangoe.com"));
                                    message1.To.Add(new MailAddress("Rachel.Millette@tangoe.com"));
                                    message1.Subject = "New User added to RTM database.";
                                    message1.Body = sb.ToString();
                                    message1.IsBodyHtml = true;

                                    smtp.Port = 25;
                                    smtp.Host = "10.0.5.104";
                                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    smtp.EnableSsl = false;

                                    smtp.Send(message1);

                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('User added Successfully')", true);
                                    return;
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "access", "alert('User added But failed to provid access please contact RTM Support')", true);
                                    return;
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "failed", "alert('User not added please try again or contact RTM Support')", true);
                                return;
                            }
                        }
                        else
                        {
                            bool result = objAccount.UpdateUser(empId, SysName, managerId, managerEmail, ddlTeam.SelectedValue, Session["UID"].ToString(), "", "", "", "","","");

                            if (result == true)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("Hi Team,");
                                sb.AppendLine("Details of User updated into RTM database is as follows");
                                sb.AppendLine("Employee Id:" + empId);
                                sb.AppendLine("Employee Name:" + username);
                                sb.AppendLine("Employee email id:" + email);
                                sb.AppendLine("System Name:" + SysName);
                                sb.AppendLine("Team Name:" + ddlTeam.SelectedItem.Text);
                                sb.AppendLine("Reporting Manager Email Id:" + managerEmail);
                                sb.AppendLine("");
                                sb.AppendLine("Please update the gender information as well.");
                                sb.AppendLine("");
                                sb.AppendLine("Thanks,");
                                sb.AppendLine("RTM Support");

                                MailMessage message1 = new MailMessage();
                                SmtpClient smtp = new SmtpClient();

                                message1.From = new MailAddress("RTM-Mailer@tangoe.com");
                                message1.To.Add(new MailAddress("RTM-Support@tangoe.com"));
                                message1.To.Add(new MailAddress("Rachel.Millette@tangoe.com"));
                                message1.Subject = "User details updated to RTM database.";
                                message1.Body = sb.ToString();
                                message1.IsBodyHtml = true;

                                smtp.Port = 25;
                                smtp.Host = "10.0.5.104";
                                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                smtp.EnableSsl = false;

                                smtp.Send(message1);

                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('User added Successfully')", true);
                                return;
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "failed", "alert('User not added please try again or contact RTM Support')", true);
                                return;
                            }
                        }
                    }
                }
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('Please select user')", true);

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('"+ ex.Message +"')", true);
        }
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
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            bool result1;
            var sourceLoc = @"\\files\Shares\HRIS Data\Production";
            DataTable data = new DataTable();
            DataTable dtEmp = new DataTable();
            var recent = new DirectoryInfo(sourceLoc).GetDirectories()
                       .OrderByDescending(d => d.LastWriteTimeUtc).First();

            var recentfolder = recent.FullName;

            var directory = new DirectoryInfo(recentfolder);
            //var myFile = directory.GetFiles()
            // .OrderByDescending(f => f.LastWriteTime)
            // .First();

            var myFile = directory.GetFiles();

            foreach (var file in myFile)
            {
                if (file.FullName.Contains("1TRQ_IMPORT_EMP_MGR"))
                {
                    var fileName = file.FullName;

                    var filename1 = fileName;
                    var reader = ReadAsLines(filename1);



                    //this assume the first record is filled with the column names
                    var headers = reader.First().Split('|');
                    foreach (var header in headers)
                        data.Columns.Add(header);

                    var records = reader.Skip(1);
                    foreach (var record in records)
                        data.Rows.Add(record.Split('|'));

                    if (data.Rows.Count > 0)
                    {
                       result1= objAccount.DeactivateAllEmployees();
                       dtEmp = objAccount.CheckExistingEmployee();
                        foreach (DataRow drRow in data.Rows)
                        {

                            // DataRow[] dr = dtEmp.Select("MUL_EmployeeId = '" + drRow["employee_id"].ToString() + "'");
                            var result = dtEmp.AsEnumerable().Where(drCheck => drCheck.Field<string>("MUL_EmployeeId") == drRow["employee_id"].ToString()).ToList();
                            if (result.Count > 0)
                            {
                                result1 = objAccount.UpdateDepartment(drRow["employee_id"].ToString(), drRow["department_id"].ToString());
                            }
                            else
                            {
                                if (drRow["department_id"].ToString() == "Unknown")
                                {
                                    continue;
                                }
                                bool resultAdd = objAccount.AddMasterUser(drRow["employee_id"].ToString(), drRow["first_name"].ToString(), drRow["last_name"].ToString(), drRow["email_address"].ToString(), drRow["department_id"].ToString());
                                //SqlParameter[] parameters = new SqlParameter[]
                                //{
                                //    new SqlParameter("@empId", drRow["employee_id"].ToString()),
                                //    new SqlParameter("@first", drRow["first_name"].ToString()),
                                //    new SqlParameter("@last", drRow["last_name"].ToString()),
                                //    new SqlParameter("@emailId", drRow["email_address"].ToString()),
                                //    new SqlParameter("@managerId", drRow["department_id"].ToString()),
                                //    new SqlParameter("@createdOn", DateTime.Now),
                                //    new SqlParameter("@status", 1)
                                //};
                                //string sQuery = "insert into RTM_Master_UserList (MUL_EmployeeId,MUL_FirstName,MUL_LastName,MUL_EmailId,MUL_ManagerID,MUL_CreatedOn,MUL_ActiveStatus) " +
                                //                 "values(@empId, @first,@last,@emailId,@managerId,@createdOn,@status)";
                                //using (cmd = new SqlCommand())
                                //{
                                //    cmd.Parameters.AddRange(parameters);
                                //    cmd.CommandText = sQuery;
                                //    cmd.CommandType = CommandType.Text;
                                //    cmd.Connection = globalCon;
                                //    globalCon.Open();
                                //    cmd.ExecuteNonQuery();
                                //    globalCon.Close();
                                //}
                            }
                        }
                    }

                    result1 = objAccount.UpdateManagerEmail();

                    return;
                }
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = ex.Message;
            lblResult.ForeColor = Color.Red;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "UploadError", "alert('" + ex.Message + "')", true);      
        }
       
    }

    static IEnumerable<string> ReadAsLines(string filename)
    {
        using (var reader = new StreamReader(filename))
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
    }
}