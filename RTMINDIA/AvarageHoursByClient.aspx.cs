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

public partial class AvarageHoursByClient : System.Web.UI.Page
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

        GenerateReport();
    }

    public int MonthDifference(DateTime lValue, DateTime rValue)
    {
        return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
    }

    private void BuildTable()
    {
        dtResult = new DataTable();
        dtResult.Columns.Add("Location");
        dtResult.Columns.Add("Client Code");
        dtResult.Columns.Add("Project Name");
        dtResult.Columns.Add("Service Code/Task");
        dtResult.Columns.Add("Total Hours");
        dtResult.Columns.Add("Average");
    }

    private void GenerateReport()
    {
        DataRow dr;

        BuildTable();

        int noOfMonths = MonthDifference(Convert.ToDateTime(txtTo.Text), Convert.ToDateTime(txtFrom.Text));

        dt = objADM.GetAvgHoursByClient(txtFrom.Text, txtTo.Text, ddlLocation.SelectedItem.Text);

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow drRow in dt.Rows)
            {
                dr = dtResult.NewRow();

                dr["Location"] = drRow["UL_Emp_Location"].ToString().Trim();
                dr["Client Code"] = drRow["CL_Code"].ToString().Trim();
                dr["Project Name"] = drRow["CL_ClientName"].ToString().Trim();
                dr["Service Code/Task"] = drRow["STL_ServiceCode"].ToString().Trim();
                if (!string.IsNullOrEmpty(drRow["Total Hours"].ToString()))
                {
                    decimal total = Math.Round(Convert.ToDecimal(drRow["Total Hours"].ToString()), 2, MidpointRounding.AwayFromZero);
                    if (total > 0)
                    {
                        dr["Total Hours"] = total;
                        dr["Average"] = total / (noOfMonths + 1);
                        dtResult.Rows.Add(dr);
                    }
                }                
            }
        }

        if (dtResult.Rows.Count > 0)
        {
            //ExportToExcel(dtResult);
            ExportToCSV(dtResult);
        }
    }

    private void ExportToExcel(DataTable dt)
    {
        string attachment="";
        attachment = "attachment; filename=Average_Hours_By_Client_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
        //if (Session["Location"].ToString() == "ADM")
        //{
        //    attachment = "attachment; filename=MSP_Hours_Report_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
        //}
        //else
        //{
        //    attachment = "attachment; filename=MSP_Hours_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".xls";
        //}
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
        Response.AddHeader("content-disposition", "attachment;filename=Average_Hours_By_Client_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        //if (Session["Location"].ToString() == "ADM")
        //{
        //    Response.AddHeader("content-disposition", "attachment;filename=Average_Hours_By_Client_" + ddlLocation.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        //}
        //else
        //{
        //    Response.AddHeader("content-disposition", "attachment;filename=Expanded_RTM_Hours_Report_" + ddlEmp.SelectedItem.Text + "_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        //}
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