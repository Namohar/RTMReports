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

public partial class TangoeApprovals : System.Web.UI.Page
{
    clsTSheetReports objclsTSheetReports = new clsTSheetReports();
    DataTable dtTeam = new DataTable();

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

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (txtFrom.Text == "")
        {
            Response.Write("<script>alert('Please select to date');</script>");
            return;
        }

        if (CheckDate(txtFrom.Text) == false)
        {
            Response.Write("<script>alert('Invalid to date');</script>");
            return;
        }

        btnExport.Text = "Please wait..";

        dtTeam = objclsTSheetReports.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());

        //string strQuery = "(";
        string strQuery = "dbo.RTM_Team_List.T_ID in (";

        foreach (DataRow dr in dtTeam.Rows)
        {
            strQuery = strQuery + "'" + dr["T_ID"].ToString() + "',";
        }

        strQuery = strQuery.Substring(0, strQuery.Length - 1);

        strQuery = strQuery + ")";

        DataTable dtResult = new DataTable();

        dtResult = objclsTSheetReports.getResultTangoeApprovals(strQuery, txtFrom.Text);
        if (dtResult.Rows.Count > 0)
        {
            ExportToCSV(dtResult, "TangoeApprovals");
        }
        else
        {
            Response.Write("<script>alert('No data to export');</script>");
        }


        btnExport.Text = "Export";
  
    }

    private void ExportToCSV(DataTable dtResult, string fileName)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=" + fileName + "-" + txtFrom.Text + "-Report.csv");
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
}