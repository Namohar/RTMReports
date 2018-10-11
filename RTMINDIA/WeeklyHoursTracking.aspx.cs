using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.Text;

public partial class WeeklyHoursTracking : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    DateTime temp;
    clsADM objADM = new clsADM();

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
        }
    }

    private void LoadTeams()
    {

        dt = new DataTable();
        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());

        ddlTeam.DataSource = dt;
        ddlTeam.DataValueField = "T_ID";
        ddlTeam.DataTextField = "T_TeamName";
        ddlTeam.DataBind();

        if (Session["access"].ToString() == "1")
        {
            ddlTeam.Items.Insert(0, "All");
            ddlTeam.SelectedIndex = 0;
        }
        else
        {
            ddlTeam.Items.Insert(0, "--Select--");
            ddlTeam.SelectedIndex = 0;
        }
    }

    protected void btnExportLessHours_Click(object sender, EventArgs e)
    {
        if (Session["access"].ToString() != "1")
        {
            if (ddlTeam.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select team", "alert('Please select team')", true);
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
        string team = "0";
        if (ddlTeam.SelectedIndex == 0)
        {
            team = "0";
        }
        else
        {
            team = ddlTeam.SelectedValue;
        }
        dt = new DataTable();
        dt = objADM.GetWeeklyHoursLessThan38(txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString(), "IND", team);

        if (dt.Rows.Count > 0)
        {
            ExportToCSV(dt, "WeeklyHoursLessThan38_75.csv");
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No Data", "alert('No data available for the week selected')", true);
        }
    }

    protected void btnExportMoreHours_Click(object sender, EventArgs e)
    {
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

        string team = "0";
        if (ddlTeam.SelectedIndex == 0)
        {
            team = "0";
        }
        else
        {
            team = ddlTeam.SelectedValue;
        }
        dt = new DataTable();
        dt = objADM.GetWeeklyHoursGreaterThan45(txtFrom.Text, Convert.ToDateTime(txtFrom.Text).AddDays(6).ToShortDateString(), "IND", team);

        if (dt.Rows.Count > 0)
        {
            ExportToCSV(dt, "WeeklyHoursGreaterThan45.csv");
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No Data", "alert('No data available for the week selected')", true);
        }
    }


    private void ExportToCSV(DataTable dtResult, string reportName)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename="+reportName);
        
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