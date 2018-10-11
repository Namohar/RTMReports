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

public partial class HourlyTimeCardReport : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    DataTable dtResult = new DataTable();
    clsTSheetReports objTSheet = new clsTSheetReports();
    clsExport objExport = new clsExport();
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
            LoadEmp();
        }
    }

    protected void rbUser_CheckedChanged(object sender, EventArgs e)
    {
        if (rbUser.Checked)
        {
            LoadEmp();
        }
    }

    protected void rbTeam_CheckedChanged(object sender, EventArgs e)
    {
        if (rbTeam.Checked)
        {
            LoadTeams();
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        ExportReport();
    }

    private void LoadEmp()
    {
        dt = objReal.LoadEmp(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["user"].ToString(), Session["UID"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();
        lblEmp.Text = "Select Employee:";
        ddlEmp.Items.Insert(0, "--Select Employee--");
        ddlEmp.SelectedIndex = 0;
    }

    private void LoadTeams()
    {
        dt = new DataTable();

        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "T_ID";
        ddlEmp.DataTextField = "T_TeamName";
        ddlEmp.DataBind();
        lblEmp.Text = "Select Team:";
        if (Session["access"].ToString() == "1")
        {
            ddlEmp.Items.Insert(0, "All");
            ddlEmp.SelectedIndex = 0;
        }
        else if (Session["access"].ToString() == "2")
        {
            ddlEmp.Items.Insert(0, "All");
            ddlEmp.SelectedIndex = 0;
        }
        else
        {
            ddlEmp.Items.Insert(0, "--Select Team--");
            ddlEmp.SelectedIndex = 0;
        }
        
    }

    private void BuildDetailsTable()
    {
        DataColumn dc;
        dtResult = new DataTable();
        dc = new DataColumn("EmployeeName");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("EmployeeNumber");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("payroll_id");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("EmployeePLDepartment");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("salaried");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("EmployeeType");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("TimeDate");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Period");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("PeriodDateRange");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("ProjectCodeName");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("ProjectServiceCode");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Hours");
        dtResult.Columns.Add(dc);

    }

    private void ExportReport()
    {
        try
        {
            if (ddlEmp.SelectedIndex == 0)
            {
                if (rbUser.Checked)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select emp", "alert('Please select Employee')", true);
                    return;
                }
                else
                {
                    if (Session["access"].ToString() != "1")
                    {
                        if (Session["access"].ToString() != "2")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select team", "alert('Please select Team')", true);
                            return;
                        }
                    }
                }
            }
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
            DataRow dr;
            BuildDetailsTable();
            if (rbUser.Checked)
            {
                dt = objTSheet.GetHourlyTimeCardRecords("user", ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
            }
            else
            {
                if (Session["access"].ToString() == "1")
                {
                    if (ddlEmp.SelectedIndex == 0)
                    {
                        dt = objTSheet.GetHourlyTimeCardRecords("All", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                    }
                    else
                    {
                        dt = objTSheet.GetHourlyTimeCardRecords("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                    }
                }
                else
                {
                    dt = objTSheet.GetHourlyTimeCardRecords("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                }
                
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    dr["EmployeeName"] = drRow["User"];
                    dr["EmployeeNumber"] = "";
                    dr["payroll_id"] = drRow["R_Employee_Id"];
                    dr["EmployeePLDepartment"] = drRow["T_TeamName"];
                    dr["salaried"] = "";
                    dr["EmployeeType"] = "";
                    dr["TimeDate"] = drRow["Date"];
                    int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drRow["Date"]));
                    dr["Period"] = weekNum;
                    int offset = Convert.ToDateTime(drRow["Date"]).DayOfWeek - DayOfWeek.Sunday;
                    DateTime lastSunday = Convert.ToDateTime(drRow["Date"]).AddDays(-offset);
                    DateTime nextSaturday = lastSunday.AddDays(6);
                    dr["PeriodDateRange"] = lastSunday.ToString("MMM d, yyyy") + " through " + nextSaturday.ToString("MMM d, yyyy");
                    dr["ProjectCodeName"] = drRow["Client"].ToString().Trim();
                    if (drRow["STL_ServiceCode"].ToString().Contains("|"))
                    {
                        dr["ProjectServiceCode"] = drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Trim();
                    }
                    else
                    {
                        dr["ProjectServiceCode"] = drRow["STL_ServiceCode"].ToString().Trim();
                    }
                    decimal duration = Convert.ToDecimal(drRow["hours"]) + (Convert.ToDecimal(drRow["minutes"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600);
                    dr["Hours"] = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);

                    dtResult.Rows.Add(dr);
                }
            }

            //if (rbUser.Checked)
            //{
            //    dt = objTSheet.GetHourlyTimeCardRecordsGreaterThan24("user", ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString());
            //}
            //else
            //{
            //    if (Session["access"].ToString() == "1")
            //    {
            //        if (ddlEmp.SelectedIndex == 0)
            //        {
            //            dt = objTSheet.GetHourlyTimeCardRecordsGreaterThan24("All", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString());
            //        }
            //        else
            //        {
            //            dt = objTSheet.GetHourlyTimeCardRecordsGreaterThan24("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString());
            //        }
            //    }
            //    else
            //    {
            //        dt = objTSheet.GetHourlyTimeCardRecordsGreaterThan24("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString());
            //    }

            //}

            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow drRow in dt.Rows)
            //    {
            //        dr = dtResult.NewRow();

            //        dr["EmployeeName"] = drRow["User"];
            //        dr["EmployeeNumber"] = "";
            //        dr["payroll_id"] = drRow["R_Employee_Id"];
            //        dr["EmployeePLDepartment"] = drRow["T_TeamName"];
            //        dr["salaried"] = "";
            //        dr["EmployeeType"] = "";
            //        dr["TimeDate"] = drRow["Date"];
            //        int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drRow["Date"]));
            //        dr["Period"] = weekNum;
            //        int offset = Convert.ToDateTime(drRow["Date"]).DayOfWeek - DayOfWeek.Sunday;
            //        DateTime lastSunday = Convert.ToDateTime(drRow["Date"]).AddDays(-offset);
            //        DateTime nextSaturday = lastSunday.AddDays(6);
            //        dr["PeriodDateRange"] = lastSunday.ToString("MMM d, yyyy") + " through " + nextSaturday.ToString("MMM d, yyyy");
            //        dr["ProjectCodeName"] = drRow["Client"];
            //        if (drRow["STL_ServiceCode"].ToString().Contains("|"))
            //        {
            //            dr["ProjectServiceCode"] = drRow["STL_ServiceCode"].ToString().Split('|')[0];
            //        }
            //        else
            //        {
            //            dr["ProjectServiceCode"] = drRow["STL_ServiceCode"];
            //        }
            //        decimal duration = Convert.ToDecimal(drRow["hours"]) + (Convert.ToDecimal(drRow["minutes"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600);
            //        dr["Hours"] = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);

            //        dtResult.Rows.Add(dr);
            //    }
            //}

            if (rbUser.Checked)
            {
                dt = objTSheet.GetHourlyTimeCardLogs("user", ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
            }
            else
            {
                if (Session["access"].ToString() == "1")
                {
                    if (ddlEmp.SelectedIndex == 0)
                    {
                        dt = objTSheet.GetHourlyTimeCardLogs("All", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                    }
                    else
                    {
                        dt = objTSheet.GetHourlyTimeCardLogs("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                    }
                }
                else
                {
                    dt = objTSheet.GetHourlyTimeCardLogs("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                }
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    dr["EmployeeName"] = drRow["LA_User_Name"];
                    dr["EmployeeNumber"] = "";
                    dr["payroll_id"] = drRow["UL_Employee_Id"];
                    dr["EmployeePLDepartment"] = drRow["T_TeamName"];
                    dr["salaried"] = "";
                    dr["EmployeeType"] = "";
                    dr["TimeDate"] = drRow["date"];
                    int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drRow["Date"]));
                    dr["Period"] = weekNum;
                    int offset = Convert.ToDateTime(drRow["date"]).DayOfWeek - DayOfWeek.Sunday;
                    DateTime lastSunday = Convert.ToDateTime(drRow["date"]).AddDays(-offset);
                    DateTime nextSaturday = lastSunday.AddDays(6);
                    dr["PeriodDateRange"] = lastSunday.ToString("MMM d, yyyy") + " through " + nextSaturday.ToString("MMM d, yyyy");
                    dr["ProjectCodeName"] = drRow["LA_Reason"].ToString().Trim();
                    dr["ProjectServiceCode"] = "";
                    decimal duration = Convert.ToDecimal(drRow["hour"]) + (Convert.ToDecimal(drRow["minute"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600);
                    dr["Hours"] = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);

                    dtResult.Rows.Add(dr);
                }
            }

            if (rbUser.Checked)
            {
                dt = objTSheet.GetHourlyTimeCardLeaves("user", ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString());
            }
            else
            {
                if (Session["access"].ToString() == "1")
                {
                    if (ddlEmp.SelectedIndex == 0)
                    {
                        dt = objTSheet.GetHourlyTimeCardLeaves("All", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString());
                    }
                    else
                    {
                        dt = objTSheet.GetHourlyTimeCardLeaves("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString());
                    }
                }
                else
                {
                    dt = objTSheet.GetHourlyTimeCardLeaves("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString());
                }
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    dr["EmployeeName"] = drRow["LD_UserName"];
                    dr["EmployeeNumber"] = "";
                    dr["payroll_id"] = drRow["UL_Employee_Id"];
                    dr["EmployeePLDepartment"] = drRow["T_TeamName"];
                    dr["salaried"] = "";
                    dr["EmployeeType"] = "";
                    dr["TimeDate"] = drRow["Date"];
                    int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drRow["Date"]));
                    dr["Period"] = weekNum;
                    int offset = Convert.ToDateTime(drRow["Date"]).DayOfWeek - DayOfWeek.Sunday;
                    DateTime lastSunday = Convert.ToDateTime(drRow["Date"]).AddDays(-offset);
                    DateTime nextSaturday = lastSunday.AddDays(6);
                    dr["PeriodDateRange"] = lastSunday.ToString("MMM d, yyyy") + " through " + nextSaturday.ToString("MMM d, yyyy");
                    dr["ProjectCodeName"] = drRow["L_JobCode"];
                    dr["ProjectServiceCode"] = drRow["L_ServiceCode"];
                    if (!string.IsNullOrEmpty(drRow["LD_Duration"].ToString()))
                    {
                        dr["Hours"] = drRow["LD_Duration"];
                    }
                    else
                    {
                        dr["Hours"] = "8";
                    }
                    

                    dtResult.Rows.Add(dr);
                }
            }

            if (dtResult.Rows.Count > 0)
            {
                DataView dv = dtResult.DefaultView;
                dv.Sort = "EmployeeName asc, TimeDate asc";
                dtResult = dv.ToTable();

                //ExportToCSV(dtResult);
                ExportToExcel(dtResult);
            }

        }
        catch (Exception)
        {
            
        }
    }
    private void ExportToCSV(DataTable dtResult)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=Hourly_TimeCard_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
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

    private void ExportToExcel(DataTable dt)
    {
        string attachment = "attachment; filename=Hourly_TimeCard_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
        Response.ClearContent();
        Response.AddHeader("content-disposition", attachment);
        Response.ContentType = "application/vnd.ms-excel";
        string tab = "";
        foreach (DataColumn dc in dt.Columns)
        {
            Response.Write(tab + dc.ColumnName);
            tab = "\t";
        }
        Response.Write("\n");
        int i;
        foreach (DataRow dr in dt.Rows)
        {
            tab = "";
            for (i = 0; i < dt.Columns.Count; i++)
            {
                Response.Write(tab + dr[i].ToString());
                tab = "\t";
            }
            Response.Write("\n");
        }
        Response.End();
    }
}