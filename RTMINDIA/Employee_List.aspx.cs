using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;

public partial class Employee_List : System.Web.UI.Page
{
    clsAccount objAccount = new clsAccount();
    DataTable dt = new DataTable();
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
            BindGrid();
        }
    }

    private void BindGrid()
    {
        dt = objAccount.EmployeeList();
        if (dt.Rows.Count > 0)
        {
            GridEmployeeList.DataSource = dt;
            GridEmployeeList.DataBind();

            //gvEmployeeList.DataSource = dt;
            //gvEmployeeList.DataBind();
        }
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        dt = objAccount.EmployeeList();
        if (dt.Rows.Count > 0)
        {
            ExportToExcel(dt);
        }
    }

    private void ExportToExcel(DataTable dt)
    {

        string attachment = "attachment; filename=Employee List.xls";
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
}