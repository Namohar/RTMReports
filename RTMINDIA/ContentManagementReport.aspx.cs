using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.Text;
using iTextSharp.text;
using NReco.PivotData;
using NReco.PivotData.Output;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.html.simpleparser;
using OfficeOpenXml;
using System.Net;
using System.Web.Services;
using System.Web.Script.Services;
using System.Data.SqlClient;
using System.Configuration;

public partial class ContentManagementReport : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsADM objAdm = new clsADM();
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
           
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string[] GetCustomers(string prefix)
    {
        List<string> customers = new List<string>();
        using (SqlConnection conn = new SqlConnection())
        {
            List<string> terms = prefix.Split(',').ToList();
            terms = terms.Select(s => s.Trim()).ToList();

            //Extract the term to be searched from the list
            string searchTerm = terms.LastOrDefault().ToString().Trim();

            //Return if Search Term is empty
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new string[0];
            }

            //Populate the terms that need to be filtered out
            List<string> excludeTerms = new List<string>();
            if (terms.Count > 1)
            {
                terms.RemoveAt(terms.Count - 1);
                excludeTerms = terms;
            }

            conn.ConnectionString = ConfigurationManager.AppSettings["conString"].ToString();
            using (SqlCommand cmd = new SqlCommand())
            {
                //string query = "select ContactName, CustomerId from Customers where " +
                //"ContactName like @SearchText + '%'";

                string query = "select Distinct U2.UL_User_Name, U2.UL_Employee_Id from RTM_User_List U1 left outer join RTM_User_List U2 on U1.UL_RepMgrId = U2.UL_Employee_Id "+
                                "Where U2.UL_User_Name is not null and U2.UL_User_Name like '%' + @SearchText + '%' ";

                //Filter out the existing searched items
                if (excludeTerms.Count > 0)
                {
                    query += string.Format(" and U2.UL_User_Name not in ({0})", string.Join(",", excludeTerms.Select(s => "'" + s + "'").ToArray()));
                }
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@SearchText", searchTerm);
                cmd.Connection = conn;
                conn.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        customers.Add(string.Format("{0}-{1}", sdr["UL_User_Name"], sdr["UL_Employee_Id"]));
                    }
                }
                conn.Close();
            }
            return customers.ToArray();
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "managers", "alert('Please select Managers')", true);
                return;
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

            ExportReport();
        }
        catch (Exception)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Exception", "alert('Something went wrong. Please try again or contact RTM-Support')", true);
        }
        
    }

   

    private void ExportReport()
    {
        DateTime start = Convert.ToDateTime(txtFrom.Text);
        var startDate = new DateTime(start.Year, start.Month, 1);        
        DateTime end = Convert.ToDateTime(txtTo.Text);
        var endDate = end.AddMonths(1).AddDays(-1);
        //string customerIds = Request.Form[hfCustomerId.UniqueID];
        string customerIds = txtSearch.Text;
        customerIds = customerIds.Trim();
        customerIds = customerIds.Remove(customerIds.Length - 1);
        //dt = objAdm.ContentMgmntReport("400541,400479", startDate.ToShortDateString(), endDate.ToShortDateString());

        dt = objAdm.ContentMgmntReport(customerIds, startDate.ToShortDateString(), endDate.ToShortDateString());

        hfCustomerId.Value = string.Empty;
        txtSearch.Text = string.Empty;

        if (dt.Rows.Count > 0)
        {
            var pkg = new ExcelPackage();
            pkg.Compression = CompressionLevel.Default;
            var wsPvt = pkg.Workbook.Worksheets.Add("Pivot Table");
            var wsData = pkg.Workbook.Worksheets.Add("Source Data");
            var PvtData = new PivotData(new[] { "Service Code", "User Name", "Month", "Reporting Manager" },
            new SumAggregatorFactory("Total Hours"));

            PvtData.ProcessData(new DataTableReader(dt));

            var pvtTbl = new PivotTable(
                    new[] { "Service Code", "User Name" }, //rows
                    new[] { "Month" }, // columns
                    PvtData,
                    new CustomSortKeyComparer(new[] { NaturalSortKeyComparer.Instance }),
                    new CustomSortKeyComparer(new[] { NaturalSortKeyComparer.Instance })
                );

            //var pvtDataTableWr = new PivotTableDataTableWriter("PivotTable");            
            var excelPvtTblWr = new ExcelPivotTableWriter(wsPvt, wsData);
            excelPvtTblWr.Write(pvtTbl);

            string path = @"~/Files/result.xlsx";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var excelFs = new FileStream(Server.MapPath(path), FileMode.Create, FileAccess.Write))
            {
                pkg.SaveAs(excelFs);
            }

            WebClient req = new WebClient();
            HttpResponse response = HttpContext.Current.Response;
            string filePath = path;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=result.xlsx");
            byte[] data = req.DownloadData(Server.MapPath(filePath));
            response.BinaryWrite(data);
            response.End();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "No Data", "alert('No data found')", true);
        }
    }


    private void ExportToCSV(DataTable dtResult)
    {
        Response.Clear();
        Response.Buffer = true;
        if (Session["Location"].ToString() == "ADM")
        {
            Response.AddHeader("content-disposition", "attachment;filename=ReportExport_"+ txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
        }
        else
        {
            Response.AddHeader("content-disposition", "attachment;filename=ReportExport_" + txtFrom.Text + "_thru_" + txtTo.Text + ".csv");
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

    protected void btnClear_Click(object sender, EventArgs e)
    {
        hfCustomerId.Value = string.Empty;
        txtSearch.Text = string.Empty;
    }
}