using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using Obout.Grid;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Collections;

public partial class ManageSubtasks : System.Web.UI.Page
{
    clsUserLogin objLogin = new clsUserLogin();
    clsTeam objTeam = new clsTeam();
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
            GetLocations();
        }
    }

    /// <summary>
    /// service to get the autocomplete service codes
    /// </summary>
    /// <param name="servicecode">service code to search</param>
    /// <returns>Matching service codes</returns>
    [WebMethod]
    public static List<string> GetAutoCompleteServiceCodes(string servicecode)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString()))
        {
            using (SqlCommand cmd = new SqlCommand("select Distinct STL_ServiceCode from RTM_SubTask_List WITH (NOLOCK) where STL_ServiceCode LIKE '%'+@SearchText+'%' ORDER BY STL_ServiceCode", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", servicecode);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}!{1}", dr["STL_ServiceCode"], dr["STL_ServiceCode"]));
                }
                return result;
            }
        }
    }

    /// <summary>
    /// Fetches the corresponding teams when location changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
    {
        grSubTasks.DataSource = null;
        grSubTasks.DataBind();
        if (ddlLocation.SelectedIndex > 0)
        {
            GetTeams(ddlLocation.SelectedValue);
        }
        else
        {
            ddlTeam.Items.Clear();
        }
    }
    /// <summary>
    /// Gets Locations
    /// </summary>
    private void GetLocations()
    {
        dt = new DataTable();
        dt = objLogin.getLocationsforMSPReport();
        if (dt.Rows.Count > 0)
        {
            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "T_Location";
            ddlLocation.DataValueField = "T_Location";
            ddlLocation.DataBind();

            ddlLocation.Items.Insert(0, "--Select Location--");
            ddlLocation.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Gets Teams
    /// </summary>
    /// <param name="location">location to fetch the teams belongs to</param>
    private void GetTeams(string location)
    {
        dt = new DataTable();
        dt = objTeam.GetTeams(location);

        if (dt.Rows.Count > 0)
        {
            ddlTeam.DataSource = dt;
            ddlTeam.DataValueField = "T_ID";
            ddlTeam.DataTextField = "T_TeamName";
            ddlTeam.DataBind();

            ddlTeam.Items.Insert(0, "--Select team--");
            ddlTeam.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Gets task in a selected team
    /// </summary>
    /// <param name="teamId">team id to fetch tasks</param>
    private void GetTasks(string teamId)
    {
        dt = new DataTable();

        dt = objTeam.GetTasks(teamId);
        if (dt.Rows.Count > 0)
        {
            ddlTask.DataSource = dt;
            ddlTask.DataTextField = "TaskName";
            ddlTask.DataValueField = "TaskId";
            ddlTask.DataBind();

            ddlTask.Items.Insert(0, "--Select task--");
            ddlTask.SelectedIndex = 0;
        }        
    }

    /// <summary>
    /// Gets Subtasks for a selected task
    /// </summary>
    /// <param name="taskId">task id to fetch the subtasks</param>
    private void GetSubtasks(string taskId)
    {
        dt = new DataTable();
        dt = objTeam.GetSubTasks(taskId);
        if (dt.Rows.Count > 0)
        {
            grSubTasks.DataSource = dt;
            grSubTasks.DataBind();
        }
        else
        {
            grSubTasks.DataSource = null;
            grSubTasks.DataBind();
        }
    }

    /// <summary>
    /// Gets tasks belongs to selected team when team changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        grSubTasks.DataSource = null;
        grSubTasks.DataBind();
        if (ddlTeam.SelectedIndex > 0)
        {
            GetTasks(ddlTeam.SelectedValue);            
        }
        else
        {
            ddlTask.Items.Clear();
        }
    }

    /// <summary>
    /// Gets subtasks belongs to selected task when task changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTask.SelectedIndex > 0)
        {
            GetSubtasks(ddlTask.SelectedValue);
            lnkExport.Visible = true;
        }
        else
        {
            grSubTasks.DataSource = null;
            grSubTasks.DataBind();
            lnkExport.Visible = false;
        }
    }

    /// <summary>
    /// button to add or Modify the subtask details
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnADD_Click(object sender, EventArgs e)
    {
        AddSubtask();
    }
    /// <summary>
    /// button to resets the input fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtSubTaskName.Text = string.Empty;
        txtServieCode.Text = string.Empty;
        btnADD.Text = "ADD";
        hfSubtaskId.Value = "0";
    }

    /// <summary>
    /// Rebing the grid when any changes made.
    /// </summary>    
    protected void RebindGrid(object sender, EventArgs e)
    {
        if (ddlTask.SelectedIndex > 0)
        {
            GetSubtasks(ddlTask.SelectedValue);
        }
        else
        {
            grSubTasks.DataSource = null;
            grSubTasks.DataBind();
        }
    }

    /// <summary>
    /// button to export data to csv
    /// </summary>    
    protected void lnkExport_Click(object sender, EventArgs e)
    {
        dt = new DataTable();
        if (ddlTask.SelectedIndex > 0)
        {
            dt = objTeam.GetSubTasks(ddlTask.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                dt.Columns.RemoveAt(0);
                ExportToCSV(dt, "SubTaskList-" + ddlTeam.SelectedItem.Text + "-" + ddlTask.SelectedItem.Text + ".csv");
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No data", "alert('No records found.')", true);
            }
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
            ddlTask.Focus();
            return;
        }
    }

    /// <summary>
    /// Add or Modify the subtask
    /// </summary>
    private void AddSubtask()
    {
        string status;
        if (ddlLocation.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "location", "alert('Please select location')", true);
            ddlLocation.Focus();
            return;
        }

        if (ddlTeam.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "team", "alert('Please select team')", true);
            ddlTeam.Focus();
            return;
        }

        if (ddlTask.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please select task')", true);
            ddlTask.Focus();
            return;
        }

        if (string.IsNullOrEmpty(txtSubTaskName.Text) || string.IsNullOrWhiteSpace(txtSubTaskName.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subtask", "alert('Please enter subtask name')", true);
            txtSubTaskName.Focus();
            return;
        }

        if (string.IsNullOrEmpty(txtServieCode.Text) || string.IsNullOrWhiteSpace(txtServieCode.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "servicecode", "alert('Please enter servicecode')", true);
            txtServieCode.Focus();
            return;
        }

        dt = new DataTable();
        dt = objTeam.CheckSubTask(ddlTask.SelectedValue, txtSubTaskName.Text, hfSubtaskId.Value);
        if (dt.Rows.Count > 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "subtask name", "alert('Subtask name already exist')", true);
            txtSubTaskName.Focus();
            return;
        }

        if (chkActiveStatus.Checked)
        {
            status = "1";
        }
        else
        {
            status = "0";
        }

        bool result = objTeam.AddSubtask(hfSubtaskId.Value, ddlTask.SelectedValue, txtSubTaskName.Text, txtServieCode.Text, status, Session["UID"].ToString());

        if (result == true)
        {
            if (hfSubtaskId.Value == "0")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Subtask added successfully')", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Subtask updated successfully')", true);
            }
            txtSubTaskName.Text = string.Empty;
            txtServieCode.Text = string.Empty;
            hfSubtaskId.Value = "0";            
            btnADD.Text = "ADD";
            GetSubtasks(ddlTask.SelectedValue);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to add subtask. Please try again or contact RTM team.')", true);
        }
    }

    private void ExportToCSV(DataTable dtResult, string reportName)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
        Response.Charset = "";
        //Response.ContentType = "text/csv";
        Response.ContentType = "application/text";
        StringBuilder sb = new StringBuilder();
        checked
        {
            for (int k = 0; k < dtResult.Columns.Count; k++)
            {
                //add separator
                sb.Append(dtResult.Columns[k].ColumnName + ',');
            }
        }

        sb.Append("\r\n");
        checked
        {
            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                for (int k = 0; k < dtResult.Columns.Count; k++)
                {
                    //add separator
                    sb.Append(dtResult.Rows[i][k].ToString().Replace(",", ";").Replace("\"", string.Empty).Trim() + ',');
                }
                //append new line
                sb.Append("\r\n");
            }
        }

        //Response.ContentEncoding = System.Text.Encoding.ASCII;
        Response.Output.Write(sb.ToString());
        Response.Flush();
        Response.SuppressContent = true;
        HttpContext.Current.ApplicationInstance.CompleteRequest();
    }
}