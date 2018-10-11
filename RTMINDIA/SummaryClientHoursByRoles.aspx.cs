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
using System.IO;

public partial class SummaryClientHoursByRoles : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    DataTable dtResult = new DataTable();
    clsTSheetReports objTSheet = new clsTSheetReports();
    clsADM objADM = new clsADM();
    DateTime temp;
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

            if (Session["Location"].ToString() == "ADM")
            {

                lblEmp.Visible = false;
                ddlEmp.Visible = false;
                lblLoc.Visible = true;
                ddlLocation.Visible = true;
                LoadLocation();
            }
            LoadTeams();
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        ExportReport();
    }

    private void LoadLocation()
    {
        clsUserLogin objLogin = new clsUserLogin();
        dt = objLogin.getLocationsforMSPReport();
        if (dt.Rows.Count > 0)
        {
            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "T_Location";
            ddlLocation.DataValueField = "T_Location";
            ddlLocation.DataBind();
            ddlLocation.Items.Insert(0, "All");
            ddlLocation.SelectedIndex = 0;
        }
    }

    private void LoadTeams()
    {
        dt = new DataTable();

        dt = objReal.LoadAllTeams(Session["access"].ToString(), Session["Location"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "T_ID";
        ddlEmp.DataTextField = "T_TeamName";
        ddlEmp.DataBind();
        lblEmp.Text = "Select Team:";
        ddlEmp.Items.Insert(0, "All");
        ddlEmp.SelectedIndex = 0;

    }

    private void ExportReport()
    {


        if (DateTime.TryParse(txtFrom.Text, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "from date", "alert('Please select valid from date')", true);
            return;
        }

        if (DateTime.TryParse(txtTo.Text, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "to date", "alert('Please select valid to date')", true);
            return;
        }

        if (Convert.ToDateTime(txtFrom.Text) > Convert.ToDateTime(txtTo.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "to date", "alert('To date should be greated than from date')", true);
            return;
        }

        if (Session["Location"].ToString() == "ADM")
        {
            dt = objADM.SummaryClientHoursbyRoles(ddlLocation.SelectedItem.Text, "ADM", ddlLocation.SelectedItem.Text, txtFrom.Text, txtTo.Text);
        }
        else
        {
            dt = objADM.SummaryClientHoursbyRoles(ddlEmp.SelectedValue, Session["Location"].ToString(), Session["Location"].ToString(), txtFrom.Text, txtTo.Text);
        }

        if (dt.Rows.Count > 0)
        {
            ExportToCSV(dt);
            //ExportGridToExcel(dt);
        }
    }

    private void ExportGridToExcel(DataTable dt)
    {
        GridView GridView1 = new GridView();
        GridView1.DataSource = dt;
        GridView1.DataBind();
        GridView1.AllowPaging = false;
        Response.Clear();
        Response.Buffer = true;
        Response.ClearContent();
        Response.ClearHeaders();
        Response.Charset = "";
        string FileName = string.Empty;
        if (Session["Location"].ToString() == "ADM")
        {
            FileName = "Summary_Client_Hours_by_Role-" + ddlLocation.SelectedItem.Text + "-From-" + txtFrom.Text + "-to-" + txtTo.Text + ".xls";
        }
        else
        {
            FileName = "Summary_Client_Hours_by_Role-" + ddlEmp.SelectedItem.Text + "-From-" + txtFrom.Text + "-to-" + txtTo.Text + ".xls";
        }

        StringWriter strwritter = new StringWriter();
        HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
        GridView1.GridLines = GridLines.Both;
        GridView1.HeaderStyle.Font.Bold = true;
        GridView1.RenderControl(htmltextwrtter);
        Response.Write(strwritter.ToString());
        Response.End();

    }

    private void ExportToCSV(DataTable dtResult)
    {
        Response.Clear();
        Response.Buffer = true;
        if (Session["Location"].ToString() == "ADM")
        {
            Response.AddHeader("content-disposition", "attachment;filename=Summary_Client_Hours_by_Role-" + ddlLocation.SelectedItem.Text + "-From-" + txtFrom.Text + "-to-" + txtTo.Text + ".csv");
        }
        else
        {
            Response.AddHeader("content-disposition", "attachment;filename=Summary_Client_Hours_by_Role-" + ddlEmp.SelectedItem.Text + "-From-" + txtFrom.Text + "-to-" + txtTo.Text + ".csv");
        }
        Response.Charset = "";
        Response.ContentType = "application/text";
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