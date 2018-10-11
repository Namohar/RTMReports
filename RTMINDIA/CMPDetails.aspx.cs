using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using DAL;

public partial class CMPDetails : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlDataAdapter da;
    DataTable dt = new DataTable();
    SqlDBHelper objDB = new SqlDBHelper();
    string SQlQuery;
    bool result;

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

    protected void btnGet_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtFrom.Text.Length <= 0)
            {
                lblError.Text = "Please select From date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
            if (txtTo.Text.Length <= 0)
            {
                lblError.Text = "Please select To date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
            GetEmployees();

        }
        catch (Exception)
        {

        }
    }

    private void GetEmployees()
    {

        dt = new DataTable();
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", txtFrom.Text),
               new SqlParameter("@to",  txtTo.Text),
           };

        
        SQlQuery = "select distinct UserName from RTM_IPVDetails where (SubTask_Id ='100' or SubTask_Id='103') and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
        if (dt.Rows.Count > 0)
        {
            ddlEmp.DataSource = dt;
            ddlEmp.DataTextField = "UserName";
            ddlEmp.DataValueField = "UserName";
            ddlEmp.DataBind();
            ddlEmp.Items.Insert(0, "All");
            ddlEmp.SelectedIndex = 0;
        }
    }

    private void printCMPDetails()
    {
        dt = new DataTable();
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", txtFrom.Text),
               new SqlParameter("@to",  txtTo.Text),
               new SqlParameter("@emp",  ddlEmp.SelectedItem.Text ),
           };

        if (ddlEmp.SelectedIndex == 0)
        {
            
            SQlQuery = "select UserName, SKU_Id as SID#, Duration, " +
                                    " StartTime from RTM_IPVDetails where (SubTask_Id ='100' or SubTask_Id='103') and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to";
        }
        else
        {
            
            SQlQuery = "select UserName, SKU_Id as SID#, Duration, " +
                                    " StartTime from RTM_IPVDetails where (SubTask_Id ='100' or SubTask_Id='103') and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to and UserName=@emp";
        }
        
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
        if (dt.Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=CMPDetails-" + txtFrom.Text + " to " + txtTo.Text + ".csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            StringBuilder sb = new StringBuilder();
            checked
            {
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    //add separator
                    sb.Append(dt.Columns[k].ColumnName + ',');
                }
            }
            
            sb.Append("\r\n");
            checked
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        //add separator
                        sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                    }
                    //append new line
                    sb.Append("\r\n");
                }
            }
            
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }
        else
        {
            lblError.Text = "No data found";
            lblError.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            printCMPDetails();
        }
        catch (Exception)
        {


        }
    }
}