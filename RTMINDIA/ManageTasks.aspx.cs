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

public partial class ManageTasks : System.Web.UI.Page
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

    protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
    {
        grTasks.DataSource = null;
        grTasks.DataBind();
        if (ddlLocation.SelectedIndex > 0)
        {
            GetTeams(ddlLocation.SelectedValue);
            lnkExport.Visible = false;
        }
        else
        {
            ddlTeam.Items.Clear();
            lnkExport.Visible = false;
        }
    }

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

    private void GetTasks(string teamId)
    {
        dt = new DataTable();

        dt = objTeam.GetTasks(teamId);
        if (dt.Rows.Count > 0)
        {
            grTasks.DataSource = dt;
            grTasks.DataBind();
        }
        else
        {
            grTasks.DataSource = null;
            grTasks.DataBind();
        }
    }
    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex > 0)
        {
            GetTasks(ddlTeam.SelectedValue);
            lnkExport.Visible = true;
        }
        else
        {
            grTasks.DataSource = null;
            grTasks.DataBind();

            lnkExport.Visible = false;
        }
    }

    protected void RebindGrid(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex > 0)
        {
            GetTasks(ddlTeam.SelectedValue);
        }
        else
        {
            grTasks.DataSource = null;
            grTasks.DataBind();
        }
    }
    protected void btnADD_Click(object sender, EventArgs e)
    {
        ADDTask();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtTaskName.Text = string.Empty;
        hfTaskId.Value = "0";
        btnADD.Text = "ADD";

        //grTasks.DataSource = null;
        //grTasks.DataBind();
    }

    private void ADDTask()
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

        if (string.IsNullOrEmpty(txtTaskName.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task", "alert('Please enter task name')", true);
            txtTaskName.Focus();
            return;
        }
        else if (string.IsNullOrWhiteSpace(txtTaskName.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "taskname", "alert('Please enter task name')", true);
            txtTaskName.Focus();
            return;
        }

        dt = new DataTable();
        dt = objTeam.CheckTask(ddlTeam.SelectedValue, txtTaskName.Text, hfTaskId.Value);

        if (dt.Rows.Count > 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "task name", "alert('Task name already exist')", true);
            txtTaskName.Focus();
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
        bool result = objTeam.InsertTask(ddlTeam.SelectedValue, txtTaskName.Text, status, hfTaskId.Value, Session["UID"].ToString());

        if (result == true)
        {
            if (hfTaskId.Value == "0")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Task added successfully')", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Task updated successfully')", true);
            }
            txtTaskName.Text = string.Empty;
            hfTaskId.Value = "0";
            btnADD.Text = "ADD";
            GetTasks(ddlTeam.SelectedValue);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failed", "alert('Failed to add task. Please try again or contact RTM team.')", true);
        }
    }

    //Export grid to CVS
    protected void lnkExport_Click(object sender, EventArgs e)
    {
        //grTasks.PageSize = -1;
        //grTasks.DataBind();
        //ExportGridToCSV();

        dt = new DataTable();
        dt = objTeam.GetTasks(ddlTeam.SelectedValue);

        if (dt.Rows.Count > 0)
        {
            dt.Columns.RemoveAt(0);
            ExportToCSV(dt, "TaskList-" + ddlTeam.SelectedItem.Text + ".csv");
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No data", "alert('No records found.')", true);
        }
    }

    private void ExportGridToCSV()
    {
        Response.Clear();

        int j = 0;

        //Add headers of the csv table
        foreach (Column col in grTasks.Columns)
        {
            if (j > 0)
            {
                Response.Write(",");
            }

            Response.Write(col.HeaderText);

            j++;
        }

        //How add the data from the Grid to csv table
        for (int i = 0; i < grTasks.Rows.Count; i++)
        {
            Hashtable dataItem = grTasks.Rows[i].ToHashtable();
            j = 0;

            Response.Write("\n");

            foreach (Column col in grTasks.Columns)
            {
                if (j > 0)
                {
                    Response.Write(",");
                }

                Response.Write(dataItem[col.DataField].ToString());

                j++;
            }
        }

        // Send the data and the appropriate headers to the browser        
        Response.AddHeader("content-disposition", "attachment;filename=TaskList.csv");
        Response.ContentType = "text/csv";
        Response.End();
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