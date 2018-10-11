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

public partial class MSPHoursReport : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    DataTable dtResult = new DataTable();
    clsTSheetReports objTSheet = new clsTSheetReports();
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
            //DataTable ds = (DataTable)Session["dtResult"];
            //if (ds.Rows.Count > 0)
            //{

            //}
            if (Session["Location"].ToString() == "ADM")
            {
                rbUser.Visible = false;
                rbTeam.Visible = false;
                lblEmp.Visible = false;
                ddlEmp.Visible = false;
                lblLoc.Visible = true;
                ddlLocation.Visible = true;
                LoadLocation();
            }
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
        dc = new DataColumn("ProjectPLDepartment");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("ClientName");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("cf_CustomerCode");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("ProjectCodeName");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("CompleteProjectServiceCodeDescription");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("EmployeePLDepartment"); 
        dtResult.Columns.Add(dc);

        dc = new DataColumn("EmployeeName"); 
        dtResult.Columns.Add(dc);

        dc = new DataColumn("EmployeeNumber"); 
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Hours"); 
        dtResult.Columns.Add(dc);

        dc = new DataColumn("TimeDate"); 
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Period"); 
        dtResult.Columns.Add(dc);

        dc = new DataColumn("PeriodDateRange"); 
        dtResult.Columns.Add(dc);

    }

    private void ExportReport()
    {
        try
        {
            if (Session["Location"].ToString() != "ADM")
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
            if (Session["Location"].ToString() == "ADM")
            {
                dt = objTSheet.GetHourlyTimeCardRecords("adm", ddlLocation.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
            }
            else if (rbUser.Checked)
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
                    try
                    {
                        dr = dtResult.NewRow();

                        dr["ProjectPLDepartment"] = drRow["CL_Product"].ToString().Trim();
                        if (drRow["Client"].ToString() == "Jury Duty" || drRow["Client"].ToString() == "Inclement Weather" || drRow["Client"].ToString() == "Personal/Sick Time" || drRow["Client"].ToString() == "Holiday" || drRow["Client"].ToString() == "Public Holiday" || drRow["Client"].ToString() == "Vacation" || drRow["Client"].ToString() == "Bereavement")
                        {
                            dr["ClientName"] = "Administration";
                            dr["ProjectCodeName"] = "Administration";
                        }
                        else
                        {
                            if (drRow["STL_ServiceCode"].ToString().Contains("|") && drRow["Client"].ToString() == "Internal")
                            {
                                if (drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Contains("Administration") || drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Contains("Training") || drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Contains("Learning") || drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Contains("Meetings") || drRow["STL_SubTask"].ToString().Contains("Employee Engagement"))
                                {
                                    dr["ClientName"] = "Administration";
                                    dr["ProjectCodeName"] = "Administration";
                                }
                                else
                                {
                                    dr["ClientName"] = drRow["Client"].ToString().Trim();
                                    dr["ProjectCodeName"] = drRow["Client"].ToString().Trim();
                                }
                            }                            
                            else
                            {
                                dr["ClientName"] = drRow["Client"].ToString().Trim();
                                dr["ProjectCodeName"] = drRow["Client"].ToString().Trim();
                            }
                        }
                        dr["cf_CustomerCode"] = drRow["CL_Code"].ToString().Trim();
                        
                        if (drRow["Client"].ToString() == "Jury Duty")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Jury Duty";
                        }
                        if (drRow["Client"].ToString() == "Inclement Weather")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Inclement Weather";
                        }
                        else if (drRow["Client"].ToString() == "Personal/Sick Time")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Personal/Sick Time";
                        }
                        else if (drRow["Client"].ToString() == "Holiday")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Holiday";
                        }
                        else if (drRow["Client"].ToString() == "Public Holiday")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Holiday";
                        }
                        else if (drRow["Client"].ToString() == "Vacation")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Vacation";
                        }
                        else if (drRow["Client"].ToString() == "Bereavement")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Bereavement";
                        }
                        else if (drRow["Client"].ToString() == "Maternity Leave")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Maternity Leave";
                        }
                        else if (drRow["Client"].ToString() == "Paternity Leave")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Paternity Leave";
                        }
                        else if (drRow["Client"].ToString() == "Medical Leave")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Medical Leave";
                        }
                        else if (drRow["Client"].ToString() == "Comp off")
                        {
                            dr["CompleteProjectServiceCodeDescription"] = "Comp off";
                        }
                        else if (drRow["STL_ServiceCode"].ToString().Contains("|"))
                        {
                            dr["CompleteProjectServiceCodeDescription"] = drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Trim();
                        }
                        else
                        {
                            dr["CompleteProjectServiceCodeDescription"] = drRow["STL_ServiceCode"].ToString().Trim();
                        }
                        dr["EmployeePLDepartment"] = drRow["T_TeamName"].ToString().Trim();
                        dr["EmployeeName"] = drRow["User"].ToString().Trim();
                        dr["EmployeeNumber"] = "";
                        decimal duration = Convert.ToDecimal(drRow["hours"]) + (Convert.ToDecimal(drRow["minutes"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600);
                        dr["Hours"] = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);
                        dr["TimeDate"] = drRow["Date"];
                        int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drRow["Date"]));
                        dr["Period"] = weekNum;
                        int offset = Convert.ToDateTime(drRow["Date"]).DayOfWeek - DayOfWeek.Sunday;
                        DateTime lastSunday = Convert.ToDateTime(drRow["Date"]).AddDays(-offset);
                        DateTime nextSaturday = lastSunday.AddDays(6);
                        dr["PeriodDateRange"] = lastSunday.ToString("MMM d, yyyy") + " through " + nextSaturday.ToString("MMM d, yyyy");

                        dtResult.Rows.Add(dr);
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                }
            }
            
            if (Session["Location"].ToString() == "ADM")
            {
                dt = objTSheet.GetHourlyTimeCardLogs("adm", ddlLocation.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
            }
            else if (rbUser.Checked)
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
                    if (drRow["LA_Reason"].ToString().Trim() == "Meeting" || drRow["LA_Reason"].ToString().Trim() == "Meetings")
                    {
                        dr["ClientName"] = "Administration";
                        dr["ProjectCodeName"] = "Administration";
                    }
                    else
                    {
                        dr["ClientName"] = "Internal";
                        dr["ProjectCodeName"] = "Internal";
                    }
                    dr["ProjectPLDepartment"] = "";
                    //dr["ClientName"] = "";
                    dr["cf_CustomerCode"] = "";
                    //dr["ProjectCodeName"] = drRow["LA_Reason"].ToString().Trim();
                    dr["CompleteProjectServiceCodeDescription"] = drRow["LA_Reason"].ToString().Trim();
                    dr["EmployeePLDepartment"] = drRow["T_TeamName"].ToString().Trim();
                    dr["EmployeeName"] = drRow["LA_User_Name"].ToString().Trim();
                    dr["EmployeeNumber"] = "";
                    decimal duration = Convert.ToDecimal(drRow["hour"]) + (Convert.ToDecimal(drRow["minute"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600);
                    dr["Hours"] = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);
                    dr["TimeDate"] = drRow["date"];
                    int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drRow["date"]));
                    dr["Period"] = weekNum;
                    int offset = Convert.ToDateTime(drRow["date"]).DayOfWeek - DayOfWeek.Sunday;
                    DateTime lastSunday = Convert.ToDateTime(drRow["date"]).AddDays(-offset);
                    DateTime nextSaturday = lastSunday.AddDays(6);
                    dr["PeriodDateRange"] = lastSunday.ToString("MMM d, yyyy") + " through " + nextSaturday.ToString("MMM d, yyyy");
                    
                    dtResult.Rows.Add(dr);
                }
            }
           

            if (dtResult.Rows.Count > 0)
            {
                DataView dv = dtResult.DefaultView;
                dv.Sort = "EmployeeName asc, TimeDate asc";
                dtResult = dv.ToTable();
                //Session["dtResult"] = dtResult;
                //Response.Write(Request.RawUrl.ToString());
                ExportToCSV(dtResult);
                //ExportToExcel(dtResult);
            }

        }
        catch (Exception)
        {

        }
    }

    private void ExportToExcel(DataTable dt)
    {
        string attachment;
        if (Session["Location"].ToString() == "ADM")
        {
            attachment = "attachment; filename=MSP_Hours_Report_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
        }
        else
        {
            attachment = "attachment; filename=MSP_Hours_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
        }
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
        Response.Flush();
        Response.End();
    }

    private void ExportToCSV(DataTable dtResult)
    {
        Response.Clear();
        Response.Buffer = true;
        if (Session["Location"].ToString() == "ADM")
        {
            Response.AddHeader("content-disposition", "attachment;filename=MSP_Hours_Report_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        }
        else
        {
            Response.AddHeader("content-disposition", "attachment;filename=MSP_Hours_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
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