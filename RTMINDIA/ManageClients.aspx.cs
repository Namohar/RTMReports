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

public partial class ManageClients : System.Web.UI.Page
{
    clsRealTimeReports objReal = new clsRealTimeReports();
    clsAccount objAccount = new clsAccount();
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
            LoadTeams();
            LoadCorePlatforms();
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        SaveClient();
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

    private void LoadCorePlatforms()
    {
        dt = new DataTable();
        dt = objAccount.LoadCorePlatform();
        if (dt.Rows.Count > 0)
        {
            ddlCore.DataSource = dt;
            ddlCore.DataTextField = "CL_Product";
            ddlCore.DataValueField = "CL_Product";
            ddlCore.DataBind();
            ddlCore.Items.Insert(0, "--Select--");
            ddlCore.SelectedIndex = 0;
        }
    }

    private void SaveClient()
    {
        try
        {
            if (ddlTeam.SelectedIndex == 0)
            {
                lblError.Text = "Select Team";
                lblError.ForeColor = Color.Red;
                return;
            }
            if (ddlCore.SelectedIndex == 0)
            {
                lblError.Text = "Select Core Platform";
                lblError.ForeColor = Color.Red;
                return;
            }
            if (string.IsNullOrWhiteSpace(txtClientName.Text))
            {
                lblError.Text = "Enter client name";
                lblError.ForeColor = Color.Red;
                return;
            }
            if (string.IsNullOrWhiteSpace(txtJobCode.Text))
            {
                lblError.Text = "Enter job code";
                lblError.ForeColor = Color.Red;
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                lblError.Text = "Enter Code";
                lblError.ForeColor = Color.Red;
                return;
            }
            string clientname;
            int index = txtClientName.Text.IndexOf("›");
            if (index > 0)
            {
                clientname = txtClientName.Text.Substring(index + 1).TrimStart().Trim();
            }
            else
            {
                clientname = txtClientName.Text.TrimStart().Trim();
            }

            string jobcode = txtJobCode.Text.Replace("›", "=>").TrimStart().Trim();

            dt = new DataTable();
            dt = objAccount.CheckJobCode(jobcode, ddlTeam.SelectedValue);
            if (dt.Rows.Count > 0)
            {
                lblError.Text = "Job Code already exist";
                lblError.ForeColor = Color.Red;
                return;
            }
            else
            {
                bool result = objAccount.AddNewClient(ddlTeam.SelectedValue, clientname, jobcode, ddlCore.SelectedValue, txtCode.Text, Session["UID"].ToString());
                if (result == true)
                {
                    lblError.Text = "Client added Successfully";
                    lblError.ForeColor = Color.Blue;
                    ddlCore.SelectedIndex = 0;
                    //ddlTeam.SelectedIndex = 0;
                    txtClientName.Text = string.Empty;
                    txtCode.Text = string.Empty;
                    txtJobCode.Text = string.Empty;
                    BindGrid();
                }
                else
                {
                    lblError.Text = "Something went wrong.. Please try again";
                    lblError.ForeColor = Color.Red;
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }
       
    }

    private void BindGrid()
    {
        dt = new DataTable();
        dt = objAccount.GetClientData(ddlTeam.SelectedValue);
        if (dt.Rows.Count > 0)
        {
            gvClients.DataSource = dt;
            gvClients.DataBind();
            btnExport.Visible = true;
        }
        else
        {
            gvClients.DataSource = null;
            gvClients.DataBind();
            btnExport.Visible = false;
        }
    }

    protected void lnkAction_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        string confirmValue = Request.Form["confirm_value"];
        if (confirmValue == "Yes")
        {
            LinkButton lbtAction = (LinkButton)grdrow.FindControl("lnkAction");
            string status = grdrow.Cells[4].Text;
            string message;
            if (status == "Active")
            {
                status = "0";
                message = "Client deactivated";
            }
            else 
            {
                status ="1";
                message = "Client activated";
            }
            string jobCode = HttpUtility.HtmlDecode(grdrow.Cells[1].Text);
            bool result = objAccount.UpdateClientStatus(ddlTeam.SelectedValue, jobCode, status);

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('"+ message +"')", true);
            BindGrid();

        }
        else
        {
           // this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('You clicked NO!')", true);
        }
    }
    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex == 0)
        {
            gvClients.DataSource = null;
            gvClients.DataBind();
            btnExport.Visible = false;
        }
        else
        {
            BindGrid();
        }
    }
    protected void gvClients_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lbtAction = (LinkButton)e.Row.FindControl("lnkAction");
            //Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            string status = e.Row.Cells[4].Text;

            if (status == "Active")
            {
                lbtAction.Text = "Deactivate";
            }
            else if (status == "Deactive")
            {
                lbtAction.Text = "Activate";
            }
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        dt = new DataTable();
        dt = objAccount.GetClientData(ddlTeam.SelectedValue);
        if (dt.Rows.Count > 0)
        {
            ExportToCSV(dt, "RTM_ClientList - (" + ddlTeam.SelectedItem.Text + ").csv");
        }

        //Response.Clear();
        //Response.Buffer = true;
        //Response.ClearContent();
        //Response.ClearHeaders();
        //Response.Charset = "";
        //string FileName = string.Empty;
        //FileName = "RTM_ClientList - ("+ ddlTeam.SelectedItem.Text +").xls";
        ////if (Session["Location"].ToString() == "ADM")
        ////{
        ////    FileName = "Employee_Hours_by_Week-" + ddlLocation.SelectedItem.Text + "-From-" + txtFrom.Text + "-to-" + txtTo.Text + ".xls";
        ////}
        ////else
        ////{
        ////    FileName = "Summary_Client_Hours_by_Employees-" + ddlEmp.SelectedItem.Text + "-From-" + txtFrom.Text + "-to-" + txtTo.Text + ".xls";
        ////}

        //StringWriter strwritter = new StringWriter();
        //HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //Response.ContentType = "application/vnd.ms-excel";
        //Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
        //gvClients.GridLines = GridLines.Both;
        //gvClients.HeaderStyle.Font.Bold = true;
        //gvClients.RenderControl(htmltextwrtter);
        //Response.Write(strwritter.ToString());
        //Response.End();
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
           server control at run time. */
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