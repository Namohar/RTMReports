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

public partial class WeeklyandMonthlyHours : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    DataTable dtResult = new DataTable();
    DataTable dtWeek = new DataTable();
    clsADM objADM = new clsADM();
    clsTSheetReports objTSheet = new clsTSheetReports();
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
            LoadLocation();
        }
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

    protected void btnExport_Click(object sender, EventArgs e)
    {
        //FisrtAndLastDay();
        ExportWeekData();
    }

    protected void btnExportMonth_Click(object sender, EventArgs e)
    {
        ExportMonthData();
    }

    private void BuildWeekTable()
    {
        DataColumn dc;
        dtResult = new DataTable();
        dc = new DataColumn("Employee Name");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Employee Title");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Employee Department");
        dtResult.Columns.Add(dc);
        dtResult.Columns.Add("Employee Location");
        dc = new DataColumn("Customer");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Customer Code");
        dtResult.Columns.Add(dc);
        dtResult.Columns.Add("Task");
        dtResult.Columns.Add("Subtask");
        dtResult.Columns.Add("Service code");
        dc = new DataColumn("Hours per Week");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Week #");
        dtResult.Columns.Add(dc);
        dtResult.Columns.Add("WeekDateRange");
        dc = new DataColumn("Employee Manager");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Director");
        dtResult.Columns.Add(dc);
    }
    private void ExportWeekData()
    {
        var now = Convert.ToDateTime(txtFrom.Text);
        var first = new DateTime(now.Year, now.Month, 1);
        var last = first.AddMonths(1).AddDays(-1);
        DataRow dr;
        BuildWeekTable();
        dtWeek = new DataTable();
        dtWeek = objADM.GetWeeks(first.ToShortDateString(), last.ToShortDateString());
        //var monthYears = GetMonths(24).Select(dt => dt.ToString("MMMM yyyy")).ToList();
        if (dtWeek.Rows.Count > 0)
        {
            foreach (DataRow drDate in dtWeek.Rows)
            {
                dt = objADM.EmployeeHoursByWeek(ddlLocation.SelectedItem.Text, Convert.ToDateTime(drDate["From"]).AddDays(1).ToShortDateString(), Convert.ToDateTime(drDate["To"]).ToShortDateString());
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow drRow in dt.Rows)
                    {
                        dr = dtResult.NewRow();

                        dr["Employee Name"] = drRow["R_User_Name"];
                        dr["Employee Title"] = drRow["UL_Job_Title"];
                        dr["Employee Department"] = drRow["T_TeamName"];
                        dr["Employee Location"] = drRow["UL_Emp_Location"];
                        dr["Customer"] = drRow["CL_ClientName"];
                        dr["Customer Code"] = drRow["CL_Code"];
                        dr["Task"] = drRow["TL_Task"];
                        dr["Subtask"] = drRow["STL_Subtask"];
                        dr["Service code"] = drRow["STL_ServiceCode"];
                        dr["Hours per Week"] = Math.Round(Convert.ToDecimal(drRow["Total Hours"]), 2, MidpointRounding.AwayFromZero);
                        int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drDate["From"]));
                        dr["Week #"] = weekNum;
                        dr["WeekDateRange"] = Convert.ToDateTime(drDate["From"]).AddDays(1).ToString("MMM d, yyyy") + " through " + Convert.ToDateTime(drDate["To"]).ToString("MMM d, yyyy");
                        dr["Employee Manager"] = drRow["UL_RepMgrEmail"];
                        dr["Director"] = drRow["UL_Director"];
                        if (dr["Hours per Week"].ToString() != "0")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
        }

        //ExportGridToExcel(dtResult, "Week", first.ToString("MM-dd-yyyy"), last.ToString("MM-dd-yyyy"));
        ExportToCSV(dtResult, "Week", first.ToString("MM-dd-yyyy"), last.ToString("MM-dd-yyyy"));
    }

    private void BuildMonthTable()
    {
        DataColumn dc;
        dtResult = new DataTable();
        dc = new DataColumn("Employee Name");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Employee Title");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Employee Department");
        dtResult.Columns.Add(dc);
        dtResult.Columns.Add("Employee Location");
        dc = new DataColumn("Customer");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Customer Code");
        dtResult.Columns.Add(dc);
        dtResult.Columns.Add("Task");
        dtResult.Columns.Add("Subtask");
        dtResult.Columns.Add("Service code");
        dc = new DataColumn("Hours per Month");
        dtResult.Columns.Add(dc);       
        dtResult.Columns.Add("WeekDateRange");
        dc = new DataColumn("Employee Manager");
        dtResult.Columns.Add(dc);
        dc = new DataColumn("Director");
        dtResult.Columns.Add(dc);
    }

    private void ExportMonthData()
    {
        var now = Convert.ToDateTime(txtFrom.Text);
        var first = new DateTime(now.Year, now.Month, 1);
        var last = first.AddMonths(1).AddDays(-1);
        DataRow dr;
        BuildMonthTable();
        dt = objADM.EmployeeHoursByMonth(ddlLocation.SelectedItem.Text, first.ToShortDateString(), last.ToShortDateString());

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow drRow in dt.Rows)
            {
                dr = dtResult.NewRow();

                dr["Employee Name"] = drRow["R_User_Name"];
                dr["Employee Title"] = drRow["UL_Job_Title"];
                dr["Employee Department"] = drRow["T_TeamName"];
                dr["Employee Location"] = drRow["UL_Emp_Location"];
                dr["Customer"] = drRow["CL_ClientName"];
                dr["Customer Code"] = drRow["CL_Code"];
                dr["Task"] = drRow["TL_Task"];
                dr["Subtask"] = drRow["STL_Subtask"];
                dr["Service code"] = drRow["STL_ServiceCode"];
                dr["Hours per Month"] = Math.Round(Convert.ToDecimal(drRow["Total Hours"]), 2, MidpointRounding.AwayFromZero);
                dr["WeekDateRange"] = first.ToString("MMM d, yyyy") + " through " + last.ToString("MMM d, yyyy"); //Convert.ToDateTime(drDate["From"]).ToString("MMM d, yyyy") + " through " + Convert.ToDateTime(drDate["To"]).ToString("MMM d, yyyy");
                dr["Employee Manager"] = drRow["UL_RepMgrEmail"];
                dr["Director"] = drRow["UL_Director"];
                if (dr["Hours per Month"].ToString() != "0")
                {
                    dtResult.Rows.Add(dr);
                }
            }
        }

        if (dtResult.Rows.Count > 0)
        {
           // ExportGridToExcel(dtResult, "Month", first.ToString("MM-dd-yyyy"), last.ToString("MM-dd-yyyy"));
            ExportToCSV(dtResult, "Month", first.ToString("MM-dd-yyyy"), last.ToString("MM-dd-yyyy"));
        }

    }

    //public static IEnumerable<DateTime> GetMonths(int count)
    //{
    //    // Note: this uses the system local time zone. Are you sure that's what
    //    // you want?
    //    var today = DateTime.Today;
    //    // Always return the 1st of the month, so we don't need to worry about
    //    // what "March 30th - 1 month" means
    //    var startOfMonth = new DateTime(today.Year, today.Month, 1);
    //    for (int i = 0; i < count; i++)
    //    {
    //        yield return startOfMonth;
    //        startOfMonth = startOfMonth.AddMonths(-1);
    //    }
    //}

    private void ExportGridToExcel(DataTable dt, string type, string from, string to)
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
        FileName = "Employee_Hours_by_"+ type +"-" + ddlLocation.SelectedItem.Text + "-From-" + from + "-to-" + to + ".xls";
        //if (Session["Location"].ToString() == "ADM")
        //{
        //    FileName = "Employee_Hours_by_Week-" + ddlLocation.SelectedItem.Text + "-From-" + txtFrom.Text + "-to-" + txtTo.Text + ".xls";
        //}
        //else
        //{
        //    FileName = "Summary_Client_Hours_by_Employees-" + ddlEmp.SelectedItem.Text + "-From-" + txtFrom.Text + "-to-" + txtTo.Text + ".xls";
        //}

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

    private void ExportToCSV(DataTable dtResult, string type, string from, string to)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=Employee_Hours_by_" + type + "-" + ddlLocation.SelectedItem.Text + "-From-" + from + "-to-" + to + ".csv");
        //if (Session["Location"].ToString() == "ADM")
        //{
        //    Response.AddHeader("content-disposition", "attachment;filename=Average_Hours_By_Client_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        //}
        //else
        //{
        //    Response.AddHeader("content-disposition", "attachment;filename=Expanded_RTM_Hours_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        //}
        Response.Charset = "";
        //Response.ContentType = "application/text";
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
        Response.End();
    }   
}