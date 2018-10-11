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

public partial class NonComplianceReport : System.Web.UI.Page
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
            LoadTeams();
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        ExportReport();
    }

    private void LoadTeams()
    {
        dt = new DataTable();

        dt = objReal.LoadAllTeams(Session["access"].ToString(), Session["Location"].ToString());

        ddlTeam.DataSource = dt;
        ddlTeam.DataValueField = "T_ID";
        ddlTeam.DataTextField = "T_TeamName";
        ddlTeam.DataBind();
        
        ddlTeam.Items.Insert(0, "All");
        ddlTeam.SelectedIndex = 0;
       
    }

    private void BuildTable()
    {
        dtResult = new DataTable();
        dtResult.Columns.Add("Employee Id");
        dtResult.Columns.Add("Employee Name");
        dtResult.Columns.Add("Team Name");
        dtResult.Columns.Add("Reporting Manager email Id");
        dtResult.Columns.Add("Last Submitted Date");
        dtResult.Columns.Add("Director");
    }

    private void ExportReport()
    {
        DataRow dr;
        DataTable dtDirector = new DataTable();
        BuildTable();

        dt = objTSheet.NonComplianceReport(ddlTeam.SelectedValue, Session["Location"].ToString());
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow drRow in dt.Rows)
            {
                dr = dtResult.NewRow();

                dr["Employee Id"] = drRow["Employee Id"];
                dr["Employee Name"] = drRow["Employee Name"];
                dr["Team Name"] = drRow["Team Name"];
                dr["Reporting Manager email Id"] = drRow["Reporting Manager email Id"];
                dr["Last Submitted Date"] = drRow["Last Submitted Date"];
                dr["Director"] = drRow["UL_Director"];
                //dtDirector = objTSheet.getDirectorFromMasterTable(drRow["Employee Id"].ToString());
                ////dtDirector = objTSheet.getDirector(drRow["Employee Name"].ToString());
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

                //    if (!string.IsNullOrEmpty(drRow["UL_RepMgrId"].ToString()))
                //    {
                //        dtDirector = objTSheet.getDirectorFromMasterTable(drRow["UL_RepMgrId"].ToString());
                //        if (dtDirector.Rows.Count > 0)
                //        {
                //          string expression = "MUL_Job_Title Like '%Dir%'";
                //          string sortOrder = "Lvl ASC";
                //           DataRow[] foundRows = dtDirector.Select(expression, sortOrder);
                //            if (foundRows.Length > 0)
                //            {
                //                dr["Director"] = foundRows[0]["MUL_EmailId"];
                //            }
                //            else
                //            {
                //                expression = "MUL_Job_Title Like '%VP%'";
                //                foundRows = dtDirector.Select(expression, sortOrder);
                //                if (foundRows.Length > 0)
                //                {
                //                    dr["Director"] = foundRows[0]["MUL_EmailId"]; // dtDirector.Rows[0]["UL_EmailId"].ToString();
                //                }
                //                else
                //                {
                //                    dr["Director"] = "";
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        dr["Director"] = "";
                //    }
                //}

                dtResult.Rows.Add(dr);
            }
        }

        if (dtResult.Rows.Count > 0)
        {
            ExportToCSV(dtResult);
        }
    }

    private void ExportToCSV(DataTable dt)
    {
        string csv = string.Empty;

        foreach (DataColumn column in dt.Columns)
        {
            //Add the Header row for CSV file.
            csv += column.ColumnName + ',';
        }

        //Add new line.
        csv += "\r\n";

        foreach (DataRow row in dt.Rows)
        {
            foreach (DataColumn column in dt.Columns)
            {
                //Add the Data rows.
                csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';
            }

            //Add new line.
            csv += "\r\n";
        }

        //Download the CSV file.
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=Non-Compliance_Report.csv");
        Response.Charset = "";
        Response.ContentType = "application/text";
        Response.Output.Write(csv);
        Response.Flush();
        Response.End();
    }
}