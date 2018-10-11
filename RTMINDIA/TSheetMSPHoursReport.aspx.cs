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

public partial class TSheetMSPHoursReport : System.Web.UI.Page
{
      clsTSheetReports objclsTSheetReports = new clsTSheetReports();
         DataTable dtTeam = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Context.Session != null)
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
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (txtFrom.Text == "" | txtTo.Text == "")
        {
            Response.Write("<script>alert('Please select from date and to date');</script>");
            return;
        }

        if (CheckDate(txtFrom.Text) == false)
        {
            Response.Write("<script>alert('Invalid from date');</script>");
            return;
        }

        if (CheckDate(txtTo.Text) == false)
        {
            Response.Write("<script>alert('Invalid to date');</script>");
            return;
        }


        if (Convert.ToDateTime(txtTo.Text) < Convert.ToDateTime(txtFrom.Text))
        {
            Response.Write("<script>alert('To date cannot be greater than from date');</script>");
            return;
        }


        //if (ddlTeams.SelectedIndex == 0)
        //{
        //    Response.Write("<script>alert('Please select Team');</script>");
        //    return;
        //}


        DataTable dtResult = new DataTable();

        dtResult = objclsTSheetReports.getResultTsheetMSPReport(ddlTeams.Text, txtFrom.Text, txtTo.Text);

        if (dtResult.Rows.Count > 0)
        {
           // ExportToCSV(dtResult, "TSheetMSPHoursReport");

            WriteDelimitedData(dtResult, "TSheetMSPHoursReport " + ddlTeams.Text + " - " + txtFrom.Text + " Through " + txtTo.Text + ".csv", ",");
        }
        else
        {
            Response.Write("<script>alert('No data to export');</script>");
        }

        //btnExport.Text = "Export";
    }


    protected bool CheckDate(String date)
    {
        try
        {
            DateTime dt = DateTime.Parse(date);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void LoadTeams()
    {
        dtTeam = objclsTSheetReports.getResultTeamTsheetMSPReport();
        ddlTeams.DataSource = dtTeam;
        //ddlTeams.DataValueField = "T_ID";
        ddlTeams.DataTextField = "EmployeePLDepartment";
        ddlTeams.DataBind();

        ddlTeams.Items.Insert(0, "All");
        ddlTeams.SelectedIndex = 0;
    }


    private void ExportToCSV(DataTable dtResult, string fileName)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=" + fileName + "-" + txtFrom.Text + "-" + txtTo.Text + "-Report.csv");
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
                    sb.Append(dtResult.Rows[i][k].ToString().Replace("\"", string.Empty).Trim() + ',');
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

    private void WriteDelimitedData(DataTable dt, string fileName, string delimiter)
    {
        //prepare the output stream
        Response.Clear();
        Response.ContentType = "text/csv";
        Response.AppendHeader("Content-Disposition",
            string.Format("attachment; filename={0}", fileName));

        string value = "";
        StringBuilder builder = new StringBuilder();

        //write the csv column headers
        for (int i = 0; i < dt.Columns.Count; i++)
        {

            value = dt.Columns[i].ColumnName;
            // Implement special handling for values that contain comma or quote
            // Enclose in quotes and double up any double quotes
            if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
            else
            {
                builder.Append(value);

            }

            Response.Write(value);
            Response.Write((i < dt.Columns.Count - 1) ? delimiter : Environment.NewLine);
            builder.Clear();
        }

        //write the data
        foreach (DataRow row in dt.Rows)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                value = row[i].ToString();
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes

                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                {
                    builder.Append(value);

                }

                Response.Write(builder.ToString());
                Response.Write((i < dt.Columns.Count - 1) ? delimiter : Environment.NewLine);
                builder.Clear();
            }
        }

        Response.End();
    }



}