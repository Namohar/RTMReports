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
using iTextSharp.text;

public partial class WeeklyHoursReportGreaterThan40 : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    DataTable dtResult = new DataTable();
    clsTSheetReports objTSheet = new clsTSheetReports();
    DateTime temp;
    List lstDate = new List();
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

    protected void btnExport_Click(object sender, EventArgs e)
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

        ExportReport();
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

    private void BuildTable()
    {
        dtResult = new DataTable();
        dtResult.Columns.Add("Employee Id");
        dtResult.Columns.Add("Employee Name");
        dtResult.Columns.Add("Employee Title");
        dtResult.Columns.Add("Team Name");
        dtResult.Columns.Add("Reporting Manager email id");
        dtResult.Columns.Add("Director");
        dtResult.Columns.Add("Total Hours");
        dtResult.Columns.Add("Week Name");
    }

    private void ExportReport()
    {
        DataRow dr;
        BuildTable();
        int weeks = Convert.ToInt32((Convert.ToDateTime(txtTo.Text) - Convert.ToDateTime(txtFrom.Text)).TotalDays / 7);
        string from = Convert.ToDateTime(txtFrom.Text).ToString("MM/dd/yyyy");
        for (int i = 0; i <= weeks - 1; i++)
        {

            string to = Convert.ToDateTime(from).AddDays(6).ToString("MM/dd/yyyy");

            if (Convert.ToDateTime(to) > Convert.ToDateTime(txtTo.Text))
            {
                break;
            }
            lstDate.Add(from + "-" + to);
            from = Convert.ToDateTime(to).AddDays(1).ToString("MM/dd/yyyy");

        }

        foreach (var item in lstDate.Chunks)
        {
            dt = objTSheet.GetWeeklyHoursGreaterThan40(item.ToString().Split('-')[0], item.ToString().Split('-')[1], ddlEmp.SelectedValue, Session["Location"].ToString());
            DataTable dtDirector = new DataTable();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    dr["Employee Id"] = drRow["UL_Employee_ID"];
                    dr["Employee Name"] = drRow["user1"];
                    dr["Employee Title"] = drRow["UL_Job_Title"];
                    dr["Team Name"] = drRow["Team1"];
                    dr["Reporting Manager email id"] = drRow["UL_RepMgrEmail"];
                    dr["Director"] = drRow["UL_Director"];
                    //dtDirector = objTSheet.getDirectorFromMasterTable(drRow["UL_Employee_ID"].ToString());
                    ////dtDirector = objTSheet.getDirector(drRow["user1"].ToString());
                    //if (dtDirector.Rows.Count > 0)
                    //{
                    //    string expression = "MUL_Job_Title Like '%Dir%'";
                    //    string sortOrder = "Lvl ASC";
                    //    DataRow[] foundRows;
                    //    foundRows = dtDirector.Select(expression, sortOrder);
                    //    if (foundRows.Length > 0)
                    //    {
                    //        dr["Director"] = foundRows[0]["MUL_EmailId"];
                    //    }
                    //    else
                    //    {
                    //        expression = "MUL_Job_Title Like '%VP%'";
                    //        foundRows = dtDirector.Select(expression, sortOrder);
                    //        if (foundRows.Length > 0)
                    //        {
                    //            dr["Director"] = foundRows[0]["MUL_EmailId"]; // dtDirector.Rows[0]["UL_EmailId"].ToString();
                    //        }
                    //        else
                    //        {
                    //            dr["Director"] = "";
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    dr["Director"] = "";
                    //}
                    dr["Total Hours"] = drRow["Total"];
                    dr["Week Name"] = Convert.ToDateTime(item.ToString().Split('-')[0]).ToString("MMM d, yyyy") + " through " + Convert.ToDateTime(item.ToString().Split('-')[1]).ToString("MMM d, yyyy");

                    dtResult.Rows.Add(dr);
                }
            }
        }

        if (dtResult.Rows.Count > 0)
        {
            ExportToCSV(dtResult);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NoData", "alert('No Data Found')", true);
        }
    }

    //private void ExportToCSV(DataTable dt)
    //{
    //    string csv = string.Empty;

    //    foreach (DataColumn column in dt.Columns)
    //    {
    //        //Add the Header row for CSV file.
    //        csv += column.ColumnName + ',';
    //    }

    //    //Add new line.
    //    csv += "\r\n";

    //    foreach (DataRow row in dt.Rows)
    //    {
    //        foreach (DataColumn column in dt.Columns)
    //        {
    //            //Add the Data rows.
    //            csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';
    //        }

    //        //Add new line.
    //        csv += "\r\n";
    //    }

    //    //Download the CSV file.
    //    Response.Clear();
    //    Response.Buffer = true;
    //    Response.AddHeader("content-disposition", "attachment;filename=Weekly_Hours_Report_GreaterThan_40Hrs.csv");
    //    Response.Charset = "";
    //    Response.ContentType = "application/text";
    //    Response.Output.Write(csv);
    //    Response.Flush();
    //    Response.End();
    //}

    private void ExportToCSV(DataTable dtResult)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=Weekly_Hours_Report_GreaterThan_40Hrs.csv");
        //if (Session["Location"].ToString() == "ADM")
        //{
        //    Response.AddHeader("content-disposition", "attachment;filename=Average_Hours_By_Client_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        //}
        //else
        //{
        //    Response.AddHeader("content-disposition", "attachment;filename=Expanded_RTM_Hours_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        //}
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