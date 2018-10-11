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

public partial class Payroll_Hours_Summary : System.Web.UI.Page
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
        dc = new DataColumn("Payroll ID");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Employee Number");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Name");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Group");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Salaried");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Exempt");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Reg Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("O/T Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("D/T Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Bereavement Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Holiday Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Jury Duty Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Personal/Sick Time Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Vacation Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Approval State");
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
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select team", "alert('Please select Team')", true);
                        return;
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
            decimal totalWorkHours = 0;
            decimal totalLogHours = 0;
            decimal regHours = 0;
            if (rbUser.Checked)
            {

                dr = dtResult.NewRow();
                dt = objTSheet.GetUsers("user", Session["Location"].ToString(), ddlEmp.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    dr["Payroll ID"] = dt.Rows[0]["UL_Employee_Id"];
                    dr["Group"] = dt.Rows[0]["T_TeamName"];
                }

                dr["Employee Number"] = "";
                dr["Name"] = ddlEmp.SelectedItem.Text;

                dr["Salaried"] = "";
                dr["Exempt"] = "0";
                dt = objTSheet.GetTotalWorkHours(ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text);
                if (dt.Rows.Count > 0)
                {
                    totalWorkHours = Convert.ToDecimal(dt.Rows[0]["hours"]) + (Convert.ToDecimal(dt.Rows[0]["minutes"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
                }

                dt = objTSheet.GetTotalLogHours(ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text);
                if (dt.Rows.Count > 0)
                {
                    totalLogHours = Convert.ToDecimal(dt.Rows[0]["hour"]) + (Convert.ToDecimal(dt.Rows[0]["minute"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
                }

                regHours = totalWorkHours + totalLogHours;
                dr["Reg Hours"] = decimal.Round(regHours, 2, MidpointRounding.AwayFromZero);
                dr["O/T Hours"] = "0";
                dr["D/T Hours"] = "0";
                dt = objTSheet.GetTotalLeaveHours(ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow drRow in dt.Rows)
                    {
                        string leaveType = drRow["L_JobCode"].ToString();
                        if (leaveType == "Personal/Sick Time")
                        {
                            dr["Personal/Sick Time Hours"] = drRow["LD_Duration"];
                        }
                        else
                        {
                            dr["Personal/Sick Time Hours"] = "0";
                        }

                        if (leaveType == "Holiday")
                        {
                            dr["Holiday Hours"] = drRow["LD_Duration"];
                        }
                        else
                        {
                            dr["Holiday Hours"] = "0";
                        }
                        if (leaveType == "Vacation")
                        {
                            dr["Vacation Hours"] = drRow["LD_Duration"];
                        }
                        else
                        {
                            dr["Vacation Hours"] = "0";
                        }
                        if (leaveType == "Bereavement")
                        {
                            dr["Bereavement Hours"] = drRow["LD_Duration"];
                        }
                        else
                        {
                            dr["Bereavement Hours"] = "0";
                        }
                        if (leaveType == "Jury Duty")
                        {
                            dr["Jury Duty Hours"] = drRow["LD_Duration"];
                        }
                        else
                        {
                            dr["Jury Duty Hours"] = "0";
                        }
                    }
                }

                dt = objTSheet.GetApprovalStatus(ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["R_Approve"].ToString() == "1")
                    {
                        dr["Approval State"] = "approved";
                    }
                    else
                    {
                        dr["Approval State"] = "submitted";
                    }
                }
                else
                {
                    dt = objTSheet.GetLeaveApprovalStatus(ddlEmp.SelectedItem.Text, txtFrom.Text, txtTo.Text);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["LD_Approve"].ToString() == "1")
                        {
                            dr["Approval State"] = "approved";
                        }
                        else
                        {
                            dr["Approval State"] = "submitted";
                        }
                    }
                }

                dtResult.Rows.Add(dr);
                totalWorkHours = 0;
                totalLogHours = 0;
                regHours = 0;
            }
            else
            {
                DataTable dtUsers = new DataTable();
                if (Session["access"].ToString() == "1")
                {
                    if (ddlEmp.SelectedIndex == 0)
                    {
                        dtUsers = objTSheet.GetUsers("All", Session["Location"].ToString(), ddlEmp.SelectedValue);
                    }
                    else
                    {
                        dtUsers = objTSheet.GetUsers("team", Session["Location"].ToString(), ddlEmp.SelectedValue);
                    }
                }
                else
                {
                    dtUsers = objTSheet.GetUsers("team", Session["Location"].ToString(), ddlEmp.SelectedValue);
                }

                if (dtUsers.Rows.Count > 0)
                {
                    foreach (DataRow drUser in dtUsers.Rows)
                    {
                        dr = dtResult.NewRow();

                        dr["Payroll ID"] = drUser["UL_Employee_Id"];
                        dr["Group"] = drUser["T_TeamName"];
                        dr["Employee Number"] = "";
                        dr["Name"] = drUser["UL_User_Name"];

                        dr["Salaried"] = "";
                        dr["Exempt"] = "0";

                        dt = objTSheet.GetTotalWorkHours(drUser["UL_User_Name"].ToString(), txtFrom.Text, txtTo.Text);
                        if (dt.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dt.Rows[0]["hours"].ToString()))
                            {
                                totalWorkHours = Convert.ToDecimal(dt.Rows[0]["hours"]) + (Convert.ToDecimal(dt.Rows[0]["minutes"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
                            }
                        }

                        dt = objTSheet.GetTotalLogHours(drUser["UL_User_Name"].ToString(), txtFrom.Text, txtTo.Text);
                        if (dt.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dt.Rows[0]["hour"].ToString()))
                            {
                                totalLogHours = Convert.ToDecimal(dt.Rows[0]["hour"]) + (Convert.ToDecimal(dt.Rows[0]["minute"]) / 60) + (Convert.ToDecimal(dt.Rows[0]["seconds"]) / 3600);
                            }
                        }

                        regHours = totalWorkHours + totalLogHours;
                        dr["Reg Hours"] = decimal.Round(regHours, 2, MidpointRounding.AwayFromZero);
                        dr["O/T Hours"] = "0";
                        dr["D/T Hours"] = "0";
                        dt = objTSheet.GetTotalLeaveHours(drUser["UL_User_Name"].ToString(), txtFrom.Text, txtTo.Text);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow drRow in dt.Rows)
                            {
                                string leaveType = drRow["L_JobCode"].ToString();
                                if (leaveType == "Personal/Sick Time")
                                {
                                    dr["Personal/Sick Time Hours"] = drRow["LD_Duration"];
                                }
                                else
                                {
                                    dr["Personal/Sick Time Hours"] = "0";
                                }

                                if (leaveType == "Holiday")
                                {
                                    dr["Holiday Hours"] = drRow["LD_Duration"];
                                }
                                else
                                {
                                    dr["Holiday Hours"] = "0";
                                }
                                if (leaveType == "Vacation")
                                {
                                    dr["Vacation Hours"] = drRow["LD_Duration"];
                                }
                                else
                                {
                                    dr["Vacation Hours"] = "0";
                                }
                                if (leaveType == "Bereavement")
                                {
                                    dr["Bereavement Hours"] = drRow["LD_Duration"];
                                }
                                else
                                {
                                    dr["Bereavement Hours"] = "0";
                                }
                                if (leaveType == "Jury Duty")
                                {
                                    dr["Jury Duty Hours"] = drRow["LD_Duration"];
                                }
                                else
                                {
                                    dr["Jury Duty Hours"] = "0";
                                }
                            }
                        }
                        else
                        {
                            dr["Personal/Sick Time Hours"] = "0";
                            dr["Holiday Hours"] = "0";
                            dr["Vacation Hours"] = "0";
                            dr["Bereavement Hours"] = "0";
                            dr["Jury Duty Hours"] = "0";
                        }

                        dt = objTSheet.GetApprovalStatus(drUser["UL_User_Name"].ToString(), txtFrom.Text, txtTo.Text);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["R_Approve"].ToString() == "1")
                            {
                                dr["Approval State"] = "approved";
                            }
                            else
                            {
                                dr["Approval State"] = "submitted";
                            }
                        }
                        else
                        {
                            dt = objTSheet.GetLeaveApprovalStatus(drUser["UL_User_Name"].ToString(), txtFrom.Text, txtTo.Text);
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0]["LD_Approve"].ToString() == "1")
                                {
                                    dr["Approval State"] = "approved";
                                }
                                else
                                {
                                    dr["Approval State"] = "submitted";
                                }
                            }
                        }

                        dtResult.Rows.Add(dr);
                        totalWorkHours = 0;
                        totalLogHours = 0;
                        regHours = 0;
                    }
                }
                
            }

            if (dtResult.Rows.Count > 0)
            {
                ExportToExcel(dtResult);
            }
        }
        catch (Exception)
        {
            
        }
    }

    private void ExportToExcel(DataTable dt)
    {
        string attachment = "attachment; filename=Payroll_Hours_Summary_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
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