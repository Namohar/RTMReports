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

public partial class ExpandedRTMHours : System.Web.UI.Page
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

    private void BuildDetailsTable()
    {
        
        dtResult = new DataTable();
        dtResult.Columns.Add("Team Name");
        dtResult.Columns.Add("Client Code");
        dtResult.Columns.Add("Project Name");
        dtResult.Columns.Add("RTM Task");
        dtResult.Columns.Add("RTM Sub-Task");
        dtResult.Columns.Add("Service Code");
        dtResult.Columns.Add("Employee Id");
        dtResult.Columns.Add("Employee Name");
        dtResult.Columns.Add("Task Hours");
        dtResult.Columns.Add("Record Date");
        dtResult.Columns.Add("Week No.");
        dtResult.Columns.Add("Week Details");
        dtResult.Columns.Add("Hourly");
        dtResult.Columns.Add("Employee Start Date");
        dtResult.Columns.Add("Employee Job Title");
        dtResult.Columns.Add("Manager Emp Id");
        dtResult.Columns.Add("Manager Email Id");
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
                dt = objTSheet.GetExpandedHourRecords("adm", ddlLocation.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
            }
            else if (rbUser.Checked)
            {
                dt = objTSheet.GetExpandedHourRecords("user", ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
            }
            else
            {
                if (Session["access"].ToString() == "1")
                {
                    if (ddlEmp.SelectedIndex == 0)
                    {
                        dt = objTSheet.GetExpandedHourRecords("All", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                    }
                    else
                    {
                        dt = objTSheet.GetExpandedHourRecords("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                    }
                }
                else
                {
                    dt = objTSheet.GetExpandedHourRecords("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                }
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    try
                    {
                        dr = dtResult.NewRow();

                        dr["Team Name"] = drRow["T_TeamName"].ToString().Trim();
                       
                        dr["Client Code"] = drRow["CL_Code"].ToString().Trim();
                        if (drRow["Client"].ToString() == "Jury Duty" || drRow["Client"].ToString() == "Inclement Weather" || drRow["Client"].ToString() == "Personal/Sick Time" || drRow["Client"].ToString() == "Public Holiday" || drRow["Client"].ToString() == "Holiday" || drRow["Client"].ToString() == "Vacation" || drRow["Client"].ToString() == "Bereavement")
                        {
                            dr["Project Name"] = "Administration";
                        }
                        else
                        {
                            if (drRow["STL_ServiceCode"].ToString().Contains("|") && drRow["Client"].ToString() == "Internal")
                            {
                                if (drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Contains("Administration") || drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Contains("Training") || drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Contains("Learning") || drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Contains("Meetings") || drRow["STL_SubTask"].ToString().Contains("Employee Engagement"))
                                {
                                    dr["Project Name"] = "Administration";
                                }
                                else
                                {
                                    dr["Project Name"] = drRow["Client"].ToString().Trim();
                                }
                            }                          
                            else
                            {
                                dr["Project Name"] = drRow["Client"].ToString().Trim();
                            }
                        }
                        string newTask = drRow["TL_Task_New"].ToString().Trim();
                        if (!string.IsNullOrEmpty(newTask))
                        {
                            dr["RTM Task"] = newTask;
                        }
                        else
                        {
                            dr["RTM Task"] = drRow["TL_Task"].ToString().Trim();
                        }

                        //Service now mapping
                        if (dr["Project Name"].ToString() == "Administration")
                        {
                            dr["RTM Task"] = "Administration";
                        }

                        dr["RTM Sub-Task"] = drRow["STL_SubTask"].ToString().Trim();
                        if (drRow["Client"].ToString() == "Jury Duty")
                        {
                            dr["Service Code"] = "Jury Duty";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Inclement Weather")
                        {
                            dr["Service Code"] = "Inclement Weather";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Personal/Sick Time")
                        {
                            dr["Service Code"] = "Personal/Sick Time";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Holiday")
                        {
                            dr["Service Code"] = "Holiday";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Public Holiday")
                        {
                            dr["Service Code"] = "Holiday";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Vacation")
                        {
                            dr["Service Code"] = "Vacation";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Bereavement")
                        {
                            dr["Service Code"] = "Bereavement";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Maternity Leave")
                        {
                            dr["Service Code"] = "Maternity Leave";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Paternity Leave")
                        {
                            dr["Service Code"] = "Paternity Leave";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Medical Leave")
                        {
                            dr["Service Code"] = "Medical Leave";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["Client"].ToString() == "Comp off")
                        {
                            dr["Service Code"] = "Comp off";
                            dr["RTM Task"] = "Paid Time Off (PTO) | PTO";
                            dr["RTM Sub-Task"] = "Paid Time Off (PTO) | PTO";
                        }
                        else if (drRow["STL_ServiceCode"].ToString().Contains("|"))
                        {
                            dr["Service Code"] = drRow["STL_ServiceCode"].ToString().Split('|')[0].ToString().Trim();
                        }
                        else
                        {
                            dr["Service Code"] = drRow["STL_ServiceCode"].ToString().Trim();
                        }
                        dr["Employee Id"] = drRow["UL_Employee_Id"].ToString().Trim();
                        dr["Employee Name"] = drRow["User"].ToString().Trim();
                        if (string.IsNullOrEmpty(drRow["hours"].ToString()) || string.IsNullOrEmpty(drRow["minutes"].ToString()) || string.IsNullOrEmpty(drRow["seconds"].ToString()))
                        {
                            continue;
                        }
                        decimal duration = Convert.ToDecimal(drRow["hours"]) + (Convert.ToDecimal(drRow["minutes"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600);
                        dr["Task Hours"] = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);
                        dr["Record Date"] = drRow["Date"];
                        int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drRow["Date"]));
                        dr["Week No."] = weekNum;
                        int offset = Convert.ToDateTime(drRow["Date"]).DayOfWeek - DayOfWeek.Sunday;
                        DateTime lastSunday = Convert.ToDateTime(drRow["Date"]).AddDays(-offset);
                        DateTime nextSaturday = lastSunday.AddDays(6);
                        dr["Week Details"] = lastSunday.ToString("MMM d, yyyy") + " through " + nextSaturday.ToString("MMM d, yyyy");
                        dr["Hourly"] = drRow["UL_Hourly"].ToString().Trim();
                        dr["Employee Start Date"] = drRow["UL_DOJ"].ToString().Trim();
                        dr["Employee Job Title"] = drRow["UL_Job_Title"].ToString().Trim();
                        dr["Manager Emp Id"] = drRow["UL_RepMgrId"].ToString().Trim();
                        dr["Manager Email Id"] = drRow["UL_RepMgrEmail"].ToString().Trim();
                        
                        //Service now
                        if (drRow["Client"].ToString() == "000001007 - Training" || drRow["Client"].ToString() == "000001001 - Administration" || drRow["Client"].ToString() == "000001011 - Vodafone Project Work" || drRow["Client"].ToString() == "000101023 - Meetings" || drRow["Client"].ToString() == "000001006 - Learning" || drRow["Client"].ToString() == "000001010 - Travel" || drRow["Client"].ToString() == "000101024 - Rivermine to Tangoe Hosting Move" )
                        {
                            dr["Project Name"] = "Administration";
                            dr["RTM Task"] = "Administration";
                        }
                        else if (drRow["Client"].ToString() == "000001002 - Client Support" || drRow["Client"].ToString() == "000001012 - Product Mgmt Support" || drRow["Client"].ToString() == "000001005 - Sales Support" || drRow["Client"].ToString() == "000101017 - Non-Task")
                        {
                            dr["Project Name"] = "Internal";
                            dr["RTM Task"] = "General";
                        }

                        dtResult.Rows.Add(dr);
                    }
                    catch (Exception)
                    {

                    }

                }
            }

            
            if (Session["Location"].ToString() == "ADM")
            {
                dt = objTSheet.GetExpandedHourLogs("adm", ddlLocation.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
            }
            else if (rbUser.Checked)
            {
                dt = objTSheet.GetExpandedHourLogs("user", ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
            }
            else
            {
                if (Session["access"].ToString() == "1")
                {
                    if (ddlEmp.SelectedIndex == 0)
                    {
                        dt = objTSheet.GetExpandedHourLogs("All", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                    }
                    else
                    {
                        dt = objTSheet.GetExpandedHourLogs("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                    }
                }
                else
                {
                    dt = objTSheet.GetExpandedHourLogs("team", ddlEmp.SelectedValue, txtFrom.Text, txtTo.Text, Session["Location"].ToString(), Session["access"].ToString(), Session["UID"].ToString());
                }
            }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();

                    dr["Team Name"] = drRow["T_TeamName"].ToString().Trim();
                    dr["Client Code"] = "";                    
                    if (drRow["LA_Reason"].ToString().Trim() == "Meeting" || drRow["LA_Reason"].ToString().Trim() == "Meetings")
                    {
                        dr["Project Name"] = "Administration";                        

                    }
                    else if (drRow["LA_Reason"].ToString().Trim() == "Non-Task")
                    {
                        dr["Project Name"] = "Internal";                        
                    }
                    else
                    {
                        dr["Project Name"] = "Internal";
                    }
                    //Service now mapping
                    if (drRow["LA_Reason"].ToString().Trim() == "Conf-Call" || drRow["LA_Reason"].ToString().Trim() == "Conference-Call" || drRow["LA_Reason"].ToString().Trim() == "Non-Task")
                    {
                        dr["RTM Task"] = "General";
                        dr["RTM Sub-Task"] = drRow["LA_Reason"].ToString().Trim();
                    }
                    else
                    {
                        dr["RTM Task"] = "Administration";
                        dr["RTM Sub-Task"] = drRow["LA_Reason"].ToString().Trim();
                    }
                    //dr["RTM Sub-Task"] = "";
                    dr["Service Code"] = drRow["LA_Reason"].ToString().Trim();

                    dr["Employee Id"] = drRow["UL_Employee_Id"].ToString().Trim();
                    dr["Employee Name"] = drRow["LA_User_Name"].ToString().Trim();
                    if (string.IsNullOrEmpty(drRow["hour"].ToString()) || string.IsNullOrEmpty(drRow["minute"].ToString()) || string.IsNullOrEmpty(drRow["seconds"].ToString()))
                    {
                        continue;
                    }
                    decimal duration = Convert.ToDecimal(drRow["hour"]) + (Convert.ToDecimal(drRow["minute"]) / 60) + (Convert.ToDecimal(drRow["seconds"]) / 3600);
                    dr["Task Hours"] = decimal.Round(duration, 2, MidpointRounding.AwayFromZero);
                    dr["Record Date"] = drRow["Date"];
                    int weekNum = objTSheet.GetWeekNumber(Convert.ToDateTime(drRow["Date"]));
                    dr["Week No."] = weekNum;
                    int offset = Convert.ToDateTime(drRow["Date"]).DayOfWeek - DayOfWeek.Sunday;
                    DateTime lastSunday = Convert.ToDateTime(drRow["Date"]).AddDays(-offset);
                    DateTime nextSaturday = lastSunday.AddDays(6);
                    dr["Week Details"] = lastSunday.ToString("MMM d, yyyy") + " through " + nextSaturday.ToString("MMM d, yyyy");
                    dr["Hourly"] = drRow["UL_Hourly"].ToString();
                    dr["Employee Start Date"] = drRow["UL_DOJ"].ToString();
                    dr["Employee Job Title"] = drRow["UL_Job_Title"].ToString();
                    dr["Manager Emp Id"] = drRow["UL_RepMgrId"].ToString();
                    dr["Manager Email Id"] = drRow["UL_RepMgrEmail"].ToString();

                    
                  
                    dtResult.Rows.Add(dr);
                }
            }

           

            if (dtResult.Rows.Count > 0)
            {
                DataView dv = dtResult.DefaultView;
                dv.Sort = "[Employee Name] asc, [Record Date] asc";
                dtResult = dv.ToTable();
                                
               //ExportToExcel(dtResult);

               ExportToCSV(dtResult);
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
            attachment = "attachment; filename=Expanded_RTM_Hours_Report_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
        }
        else
        {
            attachment = "attachment; filename=Expanded_RTM_Hours_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
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
            Response.AddHeader("content-disposition", "attachment;filename=Expanded_RTM_Hours_Report_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        }
        else
        {
            Response.AddHeader("content-disposition", "attachment;filename=Expanded_RTM_Hours_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
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