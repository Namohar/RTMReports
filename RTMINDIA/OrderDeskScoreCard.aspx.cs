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
using System.Drawing;
using DAL;

public partial class OrderDeskScoreCard : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());

    SqlDataAdapter da;
    DataTable dtResult = new DataTable();
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
        if (!IsPostBack)
        {
            BindOrderStatus();
            BindEmp();
        }
    }

    private void BindOrderStatus()
    {
        dt = new DataTable();
        SQlQuery = "select STL_ID, TL_Task+'-'+STL_SubTask as STL_SubTask from RTM_SubTask_List left join rtm_task_list on STL_Task_Id = TL_ID where TL_TeamId = 1 and STL_Status != ''";
        dt = objDB.DBExecDataTable(SQlQuery);
     
        if (dt.Rows.Count > 0)
        {
            ddlOrderStatus.DataSource = dt;
            ddlOrderStatus.DataTextField = "STL_SubTask";
            ddlOrderStatus.DataValueField = "STL_ID";
            ddlOrderStatus.DataBind();
            ddlOrderStatus.Items.Insert(0, "All");
            ddlOrderStatus.SelectedIndex = 0;
        }
    }

    private void BindEmp()
    {
        dt = new DataTable();
        SQlQuery = "select * from RTM_User_List where UL_Team_Id = 1 and UL_User_Status =1 order by UL_User_Name";
        dt = objDB.DBExecDataTable(SQlQuery);
       
        if (dt.Rows.Count > 0)
        {
            ddlEmp.DataSource = dt;
            ddlEmp.DataTextField = "UL_User_Name";
            ddlEmp.DataValueField = "UL_ID";
            ddlEmp.DataBind();
            ddlEmp.Items.Insert(0, "All");
            ddlEmp.SelectedIndex = 0;
        }
    }

    private void BuildTable()
    {
        DataColumn dc;

        try
        {
            dc = new DataColumn("Employee");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Client Name");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Order Status");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Order#");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("StartTime");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("EndTime");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Duration");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Status");
            dtResult.Columns.Add(dc);
        }
        catch (Exception)
        {

        }
    }

    private DataTable getDetails()
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", txtFrom.Text),
               new SqlParameter("@to", txtTo.Text),
               new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
               new SqlParameter("@orderstatus", ddlOrderStatus.SelectedValue),
           };

        dt = new DataTable();
        if (ddlOrderStatus.SelectedIndex == 0 && ddlEmp.SelectedIndex == 0)
        {

            SQlQuery = "select * from RTM_IPVDetails left join RTM_SubTask_List on SubTask_Id = STL_ID left join RTM_Records on RTM_IPVDetails.R_Id = RTM_Records.R_ID left join RTM_Client_List on R_Client = CL_ID where Team_Id =1 and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) Between @from and @to";
        }
        else if (ddlOrderStatus.SelectedIndex == 0)
        {

            SQlQuery = "select * from RTM_IPVDetails left join RTM_SubTask_List on SubTask_Id = STL_ID left join RTM_Records on RTM_IPVDetails.R_Id = RTM_Records.R_ID left join RTM_Client_List on R_Client = CL_ID where UserName =@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) Between @from and @to";
        }
        else if (ddlEmp.SelectedIndex == 0)
        {

            SQlQuery = "select * from RTM_IPVDetails left join RTM_SubTask_List on SubTask_Id = STL_ID left join RTM_Records on RTM_IPVDetails.R_Id = RTM_Records.R_ID left join RTM_Client_List on R_Client = CL_ID where SubTask_Id=@orderstatus  and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) Between @from and @to";
        }
        else
        {

            SQlQuery = "select * from RTM_IPVDetails left join RTM_SubTask_List on SubTask_Id = STL_ID left join RTM_Records on RTM_IPVDetails.R_Id = RTM_Records.R_ID left join RTM_Client_List on R_Client = CL_ID where SubTask_Id=@orderstatus and UserName = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) Between @from and @to";
        }

        //da.Fill(dt);
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
        return dt;
    }

    private void PrintOrders()
    {
        DataRow dr;

        BuildTable();

        dt = getDetails();

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr1 in dt.Rows)
            {
                dr = dtResult.NewRow();

                dr["Employee"] = dr1["UserName"];
                dr["Client Name"] = dr1["CL_ClientName"];
                dr["Order Status"] = dr1["STL_SubTask"];
                dr["Order#"] = dr1["SKU_Id"];
                dr["StartTime"] = dr1["StartTime"];
                dr["EndTime"] = dr1["EndTime"];
                dr["Duration"] = dr1["Duration"];
                dr["Status"] = dr1["Comments"];
                dtResult.Rows.Add(dr);
            }

            //export

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename= Orders - " + ddlEmp.SelectedItem.Text + "-" + txtFrom.Text + "-" + txtTo.Text + "-TSheet.csv");
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
                        sb.Append(dtResult.Rows[i][k].ToString().Replace(",", ";") + ',');
                    }
                    //append new line
                    sb.Append("\r\n");
                }
            }
           
            //Response.ContentEncoding = System.Text.Encoding.ASCII;
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtFrom.Text.Length == 0)
            {
                lblError.Text = "Please select from date";
            }
            else if (txtTo.Text.Length == 0)
            {
                lblError.Text = "Please select to date";
            }
            else
            {
                PrintOrders();
            }

        }
        catch (Exception)
        {


        }

    }
}