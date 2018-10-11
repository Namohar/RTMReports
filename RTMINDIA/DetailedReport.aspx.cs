using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.Text;

public partial class DetailedReport : System.Web.UI.Page
{
    DataTable dtResult = new DataTable();
    DataTable dt = new DataTable();
    clsUtilization objUtil = new clsUtilization();
    clsRealTimeReports objReal = new clsRealTimeReports();
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

        ddlTeam.Items.Insert(0, "--Select Team--");
        ddlTeam.SelectedIndex = 0;

        //lblEmp.Text = "Select Team:";
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlTeam.SelectedIndex == 0)
            {
                lblError.Text = "Please select  team";
                return;
            }

            if (DateTime.TryParse(datepicker.Value, out temp))
            { }
            else
            {
                lblError.Text = "Please Select valid from Date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (DateTime.TryParse(datepickerTo.Value, out temp))
            { }
            else
            {
                lblError.Text = "Please Select Valid To Date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
            DataRow dr;
            BuildDetailsTable();

            dt = objUtil.DetailedReportNew("team", ddlTeam.SelectedValue, datepicker.Value, datepickerTo.Value);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    //dr["Team"] = drRow["T_TeamName"];
                    dr["Date"] = drRow["Date"];
                    dr["User"] = drRow["User"];
                    dr["Client"] = drRow["Client"];
                    dr["Client Code"] = drRow["CL_Code"];
                    dr["Task"] = drRow["Task"];
                    dr["Subtask"] = drRow["Sub Task"];
                    dr["Lock Reason"] = "";
                    double taskduration = Convert.ToDouble(drRow["Duration"].ToString());
                    dr["Duration"] = Math.Round(taskduration, 2);
                    dr["Comments"] = drRow["R_Comments"];

                    dtResult.Rows.Add(dr);
                }
            }

            dt = objUtil.DetailedMeetingReportNew("team", ddlTeam.SelectedValue, datepicker.Value, datepickerTo.Value);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    //dr["Team"] = drRow["T_TeamName"];
                    dr["Date"] = drRow["date"];
                    dr["User"] = drRow["LA_User_Name"];
                    dr["Client"] = "";
                    dr["Client Code"] = "";
                    dr["Task"] = "";
                    dr["Subtask"] = "";
                    dr["Lock Reason"] = drRow["LA_Reason"];
                    double meetingDuration = Convert.ToDouble(drRow["Duration"].ToString());
                    dr["Duration"] = Math.Round(meetingDuration, 2);
                    dr["Comments"] = drRow["LA_Comments"];

                    dtResult.Rows.Add(dr);
                }
            }

            dt = objUtil.DetailedLogReportNew("team", ddlTeam.SelectedValue, datepicker.Value, datepickerTo.Value);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    //dr["Team"] = drRow["T_TeamName"];
                    dr["Date"] = drRow["date"];
                    dr["User"] = drRow["LA_User_Name"];
                    dr["Client"] = "";
                    dr["Client Code"] = "";
                    dr["Task"] = "";
                    dr["Subtask"] = "";
                    dr["Lock Reason"] = drRow["LA_Reason"];
                    double logduration = Convert.ToDouble(drRow["Duration"].ToString());
                    dr["Duration"] = Math.Round(logduration, 2);
                    dr["Comments"] = drRow["LA_Comments"];
                    dtResult.Rows.Add(dr);
                }
            }

            if (dtResult.Rows.Count > 0)
            {
                ExportToCSV(dtResult);
            }
        }
        catch (Exception)
        {


        }
    }

    private void BuildDetailsTable()
    {
        DataColumn dc;
        dtResult = new DataTable();

        //dc = new DataColumn("Team");
        //dtResult.Columns.Add(dc);

        dc = new DataColumn("Date");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("User");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Client");
        dtResult.Columns.Add(dc);

        dtResult.Columns.Add("Client Code");

        dc = new DataColumn("Task");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Subtask");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Lock Reason");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Duration");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Comments");
        dtResult.Columns.Add(dc);

    }

    private void ExportToCSV(DataTable dtResult)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=" + ddlTeam.SelectedItem.Text + "-" + datepicker.Value + "-" + datepickerTo.Value + "Detailed Report.csv");
        Response.Charset = "";

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