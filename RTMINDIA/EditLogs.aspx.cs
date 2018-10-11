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

public partial class EditLogs : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlCommand cmd;
    SqlDataAdapter da;
    DataSet ds = new DataSet();
    DataTable dt = new DataTable();
    clsTeam objTeam = new clsTeam();
    clsRealTimeReports objReal = new clsRealTimeReports();
    SqlDBHelper objDB = new SqlDBHelper();
    clsRecords objRec = new clsRecords();
    string SQlQuery;
    bool result;

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

    private void DisplayLogs()
    {
        try
        {
            //Namohar code changes on 10-Aug-2016.
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
               new SqlParameter("@date",  txtDate.Text ),
           };

            dt = new DataTable();

            SQlQuery = "select * from RTM_Log_Actions where LA_User_Name =@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) =@date and La_log_action != 'First Activity' and La_log_action !='Last Activity' and LA_Reason != 'Peer Support' and (LA_Status ='Unlocked' or LA_Status ='Task Resumed') and LA_Duration !='' order By LA_Start_Date_Time, LA_ID";
            dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

            gvLogs.DataSource = dt;
            gvLogs.DataBind();

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
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }
    }

    protected void btnDisplay_Click(object sender, EventArgs e)
    {
        DisplayLogs();
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        if (lblProcess.Text == "Split")
        {
            Regex regex = new Regex("^(?:(?:([01]?\\d|2[0-3]):)?([0-5]?\\d):)?([0-5]?\\d)$");

            if (ddlReason.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select Reason", "alert('Please select Reason')", true);
               
                mpePopUp.Show();
            }
            else if (ddlCategory.Visible == true && ddlCategory.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select category", "alert('Please select Category')", true);
                
                mpePopUp.Show();
            }
            else if (ddlSubCategory.Visible == true && ddlSubCategory.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select sub category", "alert('Please select Sub Category')", true);
              
                mpePopUp.Show();
            }
            else if (ddlClient.Visible == true && ddlClient.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select client", "alert('Please select Client')", true);
               
                mpePopUp.Show();
            }
            else if (ddlCategory.Visible == true && ddlClient.Visible == true && ddlCategory.SelectedItem.Text == "Client Specific" && ddlClient.SelectedItem.Text == "Internal")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client select", "alert('Please select Client')", true);

                mpePopUp.Show();
            }
            else if (txtComments.Text.Length == 0 || string.IsNullOrWhiteSpace(txtComments.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "enter comments", "alert('Please Enter Comments')", true);

                mpePopUp.Show();
            }
            else if (txtDuration.Text.Length < 8)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "correct time format", "alert('Please enter time in HH:MM:SS format')", true);

                mpePopUp.Show();
            }
            else if (regex.IsMatch(txtDuration.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "valid time", "alert('Please enter valid time in HH:MM:SS format')", true);

                mpePopUp.Show();
            }
            else if (TimeSpan.Parse(txtDuration.Text) >= TimeSpan.Parse(lblOldDuration.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "incorrect duration", "alert('Duration is incorrect')", true);

                mpePopUp.Show();
            }
            else
            {
                TimeSpan dur = TimeSpan.Parse(lblOldDuration.Text).Subtract(TimeSpan.Parse(txtDuration.Text));

                string comments;
                if (ddlCategory.Visible == true)
                {
                    comments = ddlCategory.SelectedItem.Text + "-" + ddlCategory.SelectedItem.Text + "-" + txtComments.Text;
                }
                else if (ddlClient.Visible == true)
                {
                    comments = ddlClient.SelectedItem.Text + "-" + txtComments.Text;
                }
                else
                {
                    comments = txtComments.Text;
                }

                //Namohar code changes on 10-Aug-2016.
                string comm = comments + "-Split";

                if (ddlCategory.SelectedItem.Text == "Client Specific" || ddlReason.SelectedItem.Text == "Conference-Call" || ddlReason.SelectedItem.Text == "Peer Support")
                {
                    int taskId = 0;
                    int subTaskId = 0;
                    if (ddlReason.SelectedItem.Text == "Meeting" && ddlCategory.SelectedItem.Text == "Client Specific")
                    {
                        dt = new DataTable();
                        dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Client Meeting");
                        if (dt.Rows.Count > 0)
                        {
                            taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                            subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                            bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LS");


                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                        }
                    }
                    else if (ddlReason.SelectedItem.Text == "Conference-Call")
                    {
                        if (ddlClient.SelectedItem.Text.Trim() == "Internal")
                        {
                            dt = new DataTable();
                            dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Conf - Call - General");
                            if (dt.Rows.Count > 0)
                            {
                                taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                                subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                                bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LS");


                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                            }
                        }
                        else
                        {
                            dt = new DataTable();
                            dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Conf - Call - Client");
                            if (dt.Rows.Count > 0)
                            {
                                taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                                subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                                bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LS");


                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                            }
                        }
                    }
                    else if (ddlReason.SelectedItem.Text == "Peer Support")
                    {
                        if (ddlClient.SelectedItem.Text.Trim() == "Internal")
                        {
                            dt = new DataTable();
                            dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Peer Support - General");
                            if (dt.Rows.Count > 0)
                            {
                                taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                                subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                                bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LS");


                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                            }
                        }
                        else
                        {
                            dt = new DataTable();
                            dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Peer Support - Client");
                            if (dt.Rows.Count > 0)
                            {
                                taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                                subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                                bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LS");


                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                            }
                        }
                    }
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                   {
                       new SqlParameter("@teamId", lblTeamId.Text),
                       new SqlParameter("@userName",  lblUserName.Text),
                       new SqlParameter("@startTime", lblStartTime.Text),
                       new SqlParameter("@duration", txtDuration.Text),
                       new SqlParameter("@reason", ddlReason.SelectedItem.Text),
                       new SqlParameter("@endTime",lblEndTime.Text ),
                       new SqlParameter("@comm",comm),
                   };
                    SQlQuery = "Insert into RTM_Log_Actions (LA_TeamId, LA_User_Name, LA_Log_Action, LA_Start_Date_Time, LA_Duration, LA_Reason, LA_CreatedOn, LA_Status, LA_Comments) values(@teamId,@userName, 'Locked',@startTime, @duration, @reason, @endTime, 'Unlocked', @comm)";
                    result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);
                }

                SqlParameter[] parameters1 = new SqlParameter[]
                   {
                       new SqlParameter("@dur", dur.ToString(@"hh\:mm\:ss")),
                       new SqlParameter("@oldRid",lblOldRId.Text),
                   };
                SQlQuery = "Update RTM_Log_Actions SET LA_Duration=@dur where LA_ID=@oldRid";
                result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters1);


                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Saved Successfully')", true);

                txtDuration.Text = string.Empty;
                txtComments.Text = string.Empty;
                lblPopError.Text = string.Empty;
                lblCategory.Visible = false;
                ddlCategory.Visible = false;
                lblSubCat.Visible = false;
                ddlSubCategory.Visible = false;
                lblClient.Visible = false;
                ddlClient.Visible = false;
                txtDuration.Enabled = true;
                lblOldRId.Text = string.Empty;
                ddlReason.SelectedIndex = 0;
                lblMainDuration.Text = string.Empty;
                lblOldDuration.Text = string.Empty;
                DisplayLogs();
            }
        }
        else
        {
            if (ddlReason.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select Reason1", "alert('Please select Reason')", true);
                
                mpePopUp.Show();
            }
            else if (ddlCategory.Visible == true && ddlCategory.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select category1", "alert('Please select Category')", true);
                
                mpePopUp.Show();
            }
            else if (ddlSubCategory.Visible == true && ddlSubCategory.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select sub category1", "alert('Please select Sub Category')", true);
                
                mpePopUp.Show();
            }
            else if (ddlClient.Visible == true && ddlClient.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select client1", "alert('Please select Client')", true);
                
                mpePopUp.Show();
            }
            else if (ddlCategory.Visible == true && ddlClient.Visible == true && ddlCategory.SelectedItem.Text == "Client Specific" && ddlClient.SelectedItem.Text == "Internal")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client select1", "alert('Please select Client')", true);

                mpePopUp.Show();
            }
            else if (txtComments.Text.Length == 0 || string.IsNullOrWhiteSpace(txtComments.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "enter comments1", "alert('Please Enter Comments')", true);
                
                mpePopUp.Show();
            }
            else
            {
                string comments;
                if (ddlSubCategory.Visible == true)
                {
                    comments = ddlCategory.SelectedItem.Text + "-" + ddlCategory.SelectedItem.Text + "-" + txtComments.Text;
                }
                else if (ddlClient.Visible == true)
                {
                    comments = ddlClient.SelectedItem.Text + "-" + txtComments.Text;
                }
                else
                {
                    comments = txtComments.Text;
                }

                if (ddlCategory.SelectedItem.Text == "Client Specific" || ddlReason.SelectedItem.Text == "Conference-Call" || ddlReason.SelectedItem.Text == "Peer Support")
                {
                    int taskId = 0;
                    int subTaskId = 0;
                    if (ddlReason.SelectedItem.Text == "Meeting" && ddlCategory.SelectedItem.Text == "Client Specific")
                    {
                        dt = new DataTable();
                        dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Client Meeting");
                        if (dt.Rows.Count > 0)
                        {
                            taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                            subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                            bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LS");
                            if (result == true)
                            {
                                result = objRec.DeleteLog(lblOldRId.Text);

                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Updated Successfully')", true);
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "update failed", "alert('Failed to update. Please try again later or contact RTM-Support')", true);
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                        }
                    }
                    else if (ddlReason.SelectedItem.Text == "Conference-Call")
                    {
                        if (ddlClient.SelectedItem.Text.Trim() == "Internal")
                        {
                            dt = new DataTable();
                            dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Conf - Call - General");
                            if (dt.Rows.Count > 0)
                            {
                                taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                                subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                                bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LE");

                                if (result == true)
                                {
                                    result = objRec.DeleteLog(lblOldRId.Text);

                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Updated Successfully')", true);
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "update failed", "alert('Failed to update. Please try again later or contact RTM-Support')", true);
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                            }
                        }
                        else
                        {
                            dt = new DataTable();
                            dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Conf - Call - Client");
                            if (dt.Rows.Count > 0)
                            {
                                taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                                subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                                bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LE");

                                if (result == true)
                                {
                                    result = objRec.DeleteLog(lblOldRId.Text);

                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Updated Successfully')", true);
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "update failed", "alert('Failed to update. Please try again later or contact RTM-Support')", true);
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                            }
                        }
                    }
                    else if (ddlReason.SelectedItem.Text == "Peer Support")
                    {
                        if (ddlClient.SelectedItem.Text.Trim() == "Internal")
                        {
                            dt = new DataTable();
                            dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Peer Support - General");
                            if (dt.Rows.Count > 0)
                            {
                                taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                                subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                                bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LE");

                                if (result == true)
                                {
                                    result = objRec.DeleteLog(lblOldRId.Text);

                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Updated Successfully')", true);
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "update failed", "alert('Failed to update. Please try again later or contact RTM-Support')", true);
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                            }
                        }
                        else
                        {
                            dt = new DataTable();
                            dt = objRec.GetTaskAndSubtask(ddlTeam.SelectedValue, "Peer Support - Client");
                            if (dt.Rows.Count > 0)
                            {
                                taskId = Convert.ToInt32(dt.Rows[0]["TL_ID"].ToString());
                                subTaskId = Convert.ToInt32(dt.Rows[0]["STL_ID"].ToString());

                                bool result = objRec.AddRecord(ddlTeam.SelectedValue, ddlEmp.SelectedValue, ddlEmp.SelectedItem.Text.ToString(), ddlClient.SelectedValue.ToString(), Convert.ToString(taskId), Convert.ToString(subTaskId), txtDuration.Text, txtComments.Text, txtDate.Text, DateTime.Now.ToShortDateString(), "Completed", "LE");

                                if (result == true)
                                {
                                    result = objRec.DeleteLog(lblOldRId.Text);

                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Updated Successfully')", true);
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "update failed", "alert('Failed to update. Please try again later or contact RTM-Support')", true);
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to update. Please contact RTM-Support')", true);
                            }
                        }
                    }
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                   {
                       new SqlParameter("@reason", ddlReason.SelectedItem.Text),
                       new SqlParameter("@comments ", comments),
                       new SqlParameter("@oldRid ", lblOldRId.Text),
                   };
                    SQlQuery = "update RTM_Log_Actions set LA_Reason=@reason, LA_Comments =@comments  where LA_ID =@oldRid";
                    result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Updated Successfully')", true);
                } 

                txtDuration.Text = string.Empty;
                txtComments.Text = string.Empty;
                lblPopError.Text = string.Empty;
                lblCategory.Visible = false;
                ddlCategory.Visible = false;
                lblSubCat.Visible = false;
                ddlSubCategory.Visible = false;
                lblClient.Visible = false;
                ddlClient.Visible = false;
                txtDuration.Enabled = true;
                lblOldRId.Text = string.Empty;
                ddlReason.SelectedIndex = 0;
                lblMainDuration.Text = string.Empty;
                lblOldDuration.Text = string.Empty;
                DisplayLogs();

            }
        }


    }

    protected void btnNo_Click(object sender, EventArgs e)
    {
        txtDuration.Text = string.Empty;
        txtComments.Text = string.Empty;
        lblPopError.Text = string.Empty;
        lblCategory.Visible = false;
        ddlCategory.Visible = false;
        lblSubCat.Visible = false;
        ddlSubCategory.Visible = false;
        lblClient.Visible = false;
        ddlClient.Visible = false;
        txtDuration.Enabled = true;
        lblOldRId.Text = string.Empty;
        ddlReason.SelectedIndex = 0;
        lblMainDuration.Text = string.Empty;
        lblOldDuration.Text = string.Empty;
        DisplayLogs();

    }

    protected void lnkInsert_Click(object sender, EventArgs e)
    {
        LinkButton lnk = (LinkButton)sender;
        GridViewRow currentRow = (GridViewRow)lnk.NamingContainer;
        int rowid = Convert.ToInt32(gvLogs.DataKeys[currentRow.RowIndex].Value.ToString());
        dt = new DataTable();

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@rowid ", rowid),
        };
        

        SQlQuery = "select * from RTM_Log_Actions where LA_ID=@rowid";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        if (dt.Rows.Count > 0)
        {
            lblProcess.Text = "Split";
            lblTeamId.Text = dt.Rows[0]["LA_TeamId"].ToString();
            lblUserName.Text = dt.Rows[0]["LA_User_Name"].ToString();
            lblStartTime.Text = dt.Rows[0]["LA_Start_Date_Time"].ToString();
            lblEndTime.Text = dt.Rows[0]["LA_CreatedOn"].ToString();
            lblMainDuration.Text = "Duration should be less than " + dt.Rows[0]["LA_Duration"].ToString();
            lblOldDuration.Text = dt.Rows[0]["LA_Duration"].ToString();
            lblOldRId.Text = rowid.ToString();
            btnYes.Text = "Insert";
            mpePopUp.Show();
        }
    }

    protected void ddlReason_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlReason.SelectedIndex == 1)
        {
            lblCategory.Visible = true;
            ddlCategory.Visible = true;
            //LoadCategory();
        }
        else if (ddlReason.SelectedIndex == 2)
        {
            lblClient.Visible = true;
            ddlClient.Visible = true;
            lblCategory.Visible = false;
            ddlCategory.Visible = false;
            lblSubCat.Visible = false;
            ddlSubCategory.Visible = false;
            LoadIdleClient();
        }
        else if (ddlReason.SelectedIndex == 3)
        {
            lblClient.Visible = true;
            ddlClient.Visible = true;
            lblCategory.Visible = false;
            ddlCategory.Visible = false;
            lblSubCat.Visible = false;
            ddlSubCategory.Visible = false;
            LoadIdleClient();
        }
        else
        {
            lblCategory.Visible = false;
            ddlCategory.Visible = false;
            lblClient.Visible = false;
            ddlClient.Visible = false;
            lblSubCat.Visible = false;
            ddlSubCategory.Visible = false;
        }
        mpePopUp.Show();
    }

    private void LoadCategory()
    {
        if (ds.Tables.Contains("category"))
        {
            ds.Tables.Remove(ds.Tables["category"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@id", lblid.Text)
        };

        string sQuery = "select M_Category from RTM_MeetingsCategory Where M_TeamId =@id group by M_Category";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "category");
        

        ddlCategory.DataSource = ds.Tables["category"];
        ddlCategory.DataTextField = "M_Category";
        ddlCategory.DataValueField = "M_Category";
        ddlCategory.DataBind();
        ddlCategory.Items.Insert(0, "--Select Category--");
        ddlCategory.SelectedIndex = 0;
    }

    private void LoadSubCategory()
    {
        if (ds.Tables.Contains("subcategory"))
        {
            ds.Tables.Remove(ds.Tables["subcategory"]);
        }

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@category ", ddlCategory.SelectedItem.Text),
        };

       
        SQlQuery = "select M_SubCategory from RTM_MeetingsCategory where M_Category =@category";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "subcategory");

        ddlSubCategory.DataSource = ds.Tables["subcategory"];
        ddlSubCategory.DataTextField = "M_SubCategory";
        ddlSubCategory.DataValueField = "M_SubCategory";
        ddlSubCategory.DataBind();
        ddlSubCategory.Items.Insert(0, "--Select SubCategory--");
        ddlSubCategory.SelectedIndex = 0;
    }

    private void LoadIdleClient()
    {
        if (ds.Tables.Contains("client"))
        {
            ds.Tables.Remove(ds.Tables["client"]);
        }

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team ",Convert.ToInt32(Session["team"])),
        };

        SQlQuery = "select * from RTM_Client_List WITH (NOLOCK) where CL_TeamId=@team and CL_ClientName !='Personal/Sick Time' and CL_ClientName != 'Inclement Weather' and CL_ClientName!='Public Holiday' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' order by CL_ClientName";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "client");

        ddlClient.DataSource = ds.Tables["client"];
        ddlClient.DataTextField = "CL_ClientName";
        ddlClient.DataValueField = "CL_ID";
        ddlClient.DataBind();
        ddlClient.Items.Insert(0, "-Select Client-");
        ddlClient.SelectedIndex = 0;
    }

    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCategory.SelectedIndex != 0)
        {
            if (ddlCategory.SelectedItem.Text == "Client Specific")
            {
                lblClient.Visible = true;
                ddlClient.Visible = true;
                LoadIdleClient();
            }
            else
            {
                lblClient.Visible = false;
                ddlClient.Visible = false;
            }
            //lblSubCat.Visible = true;
            //ddlSubCategory.Visible = true;

            //LoadSubCategory();
        }
        else
        {
            lblSubCat.Visible = false;
            ddlSubCategory.Visible = false;
        }
        mpePopUp.Show();
    }

    protected void lnkEdit_Click(object sender, EventArgs e)
    {
        LinkButton lnk = (LinkButton)sender;
        GridViewRow currentRow = (GridViewRow)lnk.NamingContainer;
        int rowid = Convert.ToInt32(gvLogs.DataKeys[currentRow.RowIndex].Value.ToString());
        lblOldRId.Text = rowid.ToString();
        Label duration = (Label)currentRow.FindControl("lblDuration");

        lblProcess.Text = "Edit";
        txtDuration.Text = duration.Text;
        txtDuration.Enabled = false;
        btnYes.Text = "Update";
        mpePopUp.Show();

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
}