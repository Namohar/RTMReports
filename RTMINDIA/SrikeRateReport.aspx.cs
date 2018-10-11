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
using BAL;

public partial class SrikeRateReport : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlDataAdapter da;
    DataTable dt = new DataTable();

    SqlDBHelper objDB = new SqlDBHelper();
    string SQlQuery;
    bool result;
    clsRealTimeReports objReal = new clsRealTimeReports();
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
            BindTeam();
        }
    }

    private void BindTeam()
    {
        dt = new DataTable();

        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());
        ddlTeam.DataSource = dt;
        ddlTeam.DataValueField = "T_ID";
        ddlTeam.DataTextField = "T_TeamName";
        ddlTeam.DataBind();
        ddlTeam.Items.Insert(0, "--Select Team--");
        ddlTeam.SelectedIndex = 0;
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
            //GetEmployees();

            GetTasks();

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

        //INvoice
        SQlQuery = "select distinct UserName from RTM_IPVDetails where (SubTask_Id ='129' or SubTask_Id='141' or SubTask_Id='1740') and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) BETWEEN @from and @to";
        //QC
        //SQlQuery = "select distinct UserName from RTM_IPVDetails where SubTask_Id ='213' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to";
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

    private void GetTasks()
    {
        dt = new DataTable();

        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", txtFrom.Text),
               new SqlParameter("@to",  txtTo.Text),
               new SqlParameter("@team", ddlTeam.SelectedValue)
           };

        //INvoice
        SQlQuery = "select SubTask_Id, TL_Task +' - '+ STL_SubTask as [Task] from RTM_IPVDetails left join RTM_SubTask_List on SubTask_Id = STL_ID left join RTM_Task_List on STL_Task_Id = TL_ID where TL_TeamId = @team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) between @from and @to group by SubTask_Id, TL_Task, STL_SubTask order by TL_Task";
        //QC
        //SQlQuery = "select distinct UserName from RTM_IPVDetails where SubTask_Id ='213' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        if (dt.Rows.Count > 0)
        {
            ddlEmp.DataSource = dt;
            ddlEmp.DataTextField = "Task";
            ddlEmp.DataValueField = "SubTask_Id";
            ddlEmp.DataBind();
            ddlEmp.Items.Insert(0, "All");
            ddlEmp.SelectedIndex = 0;
        }
    }

    private void printIPVDetails()
    {
        dt = new DataTable();
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from",txtFrom.Text),
               new SqlParameter("@to", txtTo.Text),
               //new SqlParameter("@emp",  ddlEmp.SelectedItem.Text),
               new SqlParameter("@subTask",  ddlEmp.SelectedValue),
               new SqlParameter("@team", ddlTeam.SelectedValue)
              
           };
        if (ddlTeam.SelectedValue == "24")
        {
            if (ddlEmp.SelectedIndex == 0)
            {
                SQlQuery = "select ipv.UserName as [User Name], CONVERT(VARCHAR(10),ipv.StartTime,101) as [Date], ipv.SKU_ID as [Account #], CL_ClientName as [CLient], TL_Task as [Task], STL_SubTask as [Subtask], ipv.[Type], " +
                            "ipv.[Status], ipv.LineItem, ipv.Duration " +
                            "from RTM_IPVDetails as ipv " +
                            "left join RTM_Records as rec on ipv.R_ID = rec.R_ID " +
                            "left join RTM_Client_List cl on rec.R_Client = cl.CL_ID " +
                            "left join RTM_Task_List tl on rec.R_Task = tl.TL_ID " +
                            "left join RTM_SubTask_List stl on rec.R_SubTask = stl.STL_ID " +
                            "where Team_Id = @team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) Between @from and @to " +                            
                            "order by ipv.StartTime";
            }
            else
            {
                SQlQuery = "select ipv.UserName as [User Name], CONVERT(VARCHAR(10),ipv.StartTime,101) as [Date], ipv.SKU_ID as [Account #], CL_ClientName as [Client], TL_Task as [Task], STL_SubTask as [Subtask], ipv.[Type], " +
                           "ipv.[Status], ipv.LineItem, ipv.Duration " +
                           "from RTM_IPVDetails as ipv " +
                           "left join RTM_Records as rec on ipv.R_ID = rec.R_ID " +
                           "left join RTM_Client_List cl on rec.R_Client = cl.CL_ID " +
                           "left join RTM_Task_List tl on rec.R_Task = tl.TL_ID " +
                           "left join RTM_SubTask_List stl on rec.R_SubTask = stl.STL_ID " +
                           "where Team_Id = @team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) Between @from and @to " +
                           "and SubTask_Id = @subTask " +
                           "order by ipv.StartTime";
            }
        }
        else if (ddlEmp.SelectedIndex == 0)
        {
            //Invoice
            //SQlQuery = "select UserName as [User Name], SKU_ID as [SKU#], CL_ClientName as Client," +
            //                        "CONVERT(varchar(10), sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600) +':'+ " +
            //                        "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60) +':'+ " +
            //                        "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60)))%60) as [Total Duration] " +
            //                        " from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and (SubTask_Id='129' or SubTask_Id='141' or SubTask_Id ='1740') and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EndTime))) Between @from and @to  GROUP BY UserName, SKU_ID, CL_ClientName";

            SQlQuery = "select UserName as [User Name], SKU_ID as [ID#], CL_ClientName as Client," +
                                    "CONVERT(varchar(10), sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600) +':'+ " +
                                    "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60) +':'+ " +
                                    "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60)))%60) as [Total Duration] " +
                                    " from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) Between @from and @to and Team_Id = @team  GROUP BY UserName, SKU_ID, CL_ClientName";
            //QC

            //SQlQuery = "select UserName as [User Name], SKU_ID as [SKU#], CL_ClientName as Client," +
            //                        "CONVERT(varchar(10), sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600) +':'+ " +
            //                        "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60) +':'+ " +
            //                        "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60)))%60) as [Total Duration] " +
            //                        " from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and SubTask_Id='213' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EndTime))) Between @from and @to  GROUP BY UserName, SKU_ID, CL_ClientName";
        }
        else
        {
            //Invoice  
            //SQlQuery = "select UserName as [User Name], SKU_ID as [SKU#], CL_ClientName as Client," +
            //                       "CONVERT(varchar(10), sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600) +':'+ " +
            //                       "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60) +':'+ " +
            //                       "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60)))%60) as [Total Duration] " +
            //                       " from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and (SubTask_Id='129' or SubTask_Id='141' or SubTask_Id='1740') and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EndTime))) Between @from and @to and UserName=@emp  GROUP BY UserName, SKU_ID, CL_ClientName";

            SQlQuery = "select UserName as [User Name], SKU_ID as [ID#], CL_ClientName as Client," +
                                   "CONVERT(varchar(10), sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600) +':'+ " +
                                   "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60) +':'+ " +
                                   "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60)))%60) as [Total Duration] " +
                                   " from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and SubTask_Id=@subTask and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) Between @from and @to  GROUP BY UserName, SKU_ID, CL_ClientName";

            //QC

            //SQlQuery = "select UserName as [User Name], SKU_ID as [SKU#], CL_ClientName as Client," +
            //                       "CONVERT(varchar(10), sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600) +':'+ " +
            //                       "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60) +':'+ " +
            //                       "CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(Duration,'-', '')))/60)%60)))%60) as [Total Duration] " +
            //                       " from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and SubTask_Id='213' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EndTime))) Between @from and @to and UserName=@emp  GROUP BY UserName, SKU_ID, CL_ClientName";

        }
        // da.Fill(dt);
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
        if (dt.Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=StrikeRateDetails-(" + ddlEmp.SelectedItem.Text + ")" + txtFrom.Text + " to " + txtTo.Text + ".csv");
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

    private void printStatusDetails()
    {
        dt = new DataTable();

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from",txtFrom.Text),
               new SqlParameter("@to", txtTo.Text),
               //new SqlParameter("@emp",  ddlEmp.SelectedItem.Text),
               new SqlParameter("@subTask",  ddlEmp.SelectedValue),
               new SqlParameter("@team", ddlTeam.SelectedValue)
           };

        if (ddlEmp.SelectedIndex == 0)
        {
            //Invoice
            //SQlQuery = "select UserName, SKU_Id as SKU#, Duration, " +
            //                        "(CASE LoadedUnloaded " +
            //                          " WHEN 'Y' THEN 'Loaded'" +
            //                          " WHEN 'N' THEN 'Not Loaded'" +
            //                        "END) as LoadedStatus,CL_ClientName as Client, " +
            //                        "Comments, StartTime from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and (SubTask_Id ='129' or SubTask_Id='141' or SubTask_Id='1740') and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to order by SKU_Id";

            SQlQuery = "select UserName, SKU_Id as ID#, Duration, " +
                                    "(CASE LoadedUnloaded " +
                                      " WHEN 'Y' THEN 'Loaded'" +
                                      " WHEN 'N' THEN 'Not Loaded'" +
                                      "ELSE ''" +
                                    "END) as LoadedStatus,CL_ClientName as Client, " +
                                    "Comments, StartTime from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) BETWEEN @from and @to and Team_Id = @team order by SKU_Id";

            //QC

            //SQlQuery = "select UserName, SKU_Id as SKU#, Duration, " +
            //                        "(CASE LoadedUnloaded " +
            //                          " WHEN 'Y' THEN 'Loaded'" +
            //                          " WHEN 'N' THEN 'Not Loaded'" +
            //                        "END) as LoadedStatus,CL_ClientName as Client, " +
            //                        "Comments, StartTime from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and SubTask_Id ='213' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to order by SKU_Id";

        }
        else
        {
            //Invoice
            //SQlQuery = "select UserName, SKU_Id as SKU#, Duration, " +
            //                        "(CASE LoadedUnloaded " +
            //                          " WHEN 'Y' THEN 'Loaded'" +
            //                          " WHEN 'N' THEN 'Not Loaded'" +
            //                        "END) as LoadedStatus,CL_ClientName as Client, " +
            //                        "Comments, StartTime from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and (SubTask_Id ='129' or SubTask_Id='141' or SubTask_Id='1740') and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to and UserName=@emp order by SKU_Id";

            SQlQuery = "select UserName, SKU_Id as ID#, Duration, " +
                                    "(CASE LoadedUnloaded " +
                                      " WHEN 'Y' THEN 'Loaded'" +
                                      " WHEN 'N' THEN 'Not Loaded'" +
                                    "END) as LoadedStatus,CL_ClientName as Client, " +
                                    "Comments, StartTime from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and SubTask_Id = @subTask and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, TimeDate))) BETWEEN @from and @to order by SKU_Id";

            //QC

            //SQlQuery = "select UserName, SKU_Id as SKU#, Duration, " +
            //                        "(CASE LoadedUnloaded " +
            //                          " WHEN 'Y' THEN 'Loaded'" +
            //                          " WHEN 'N' THEN 'Not Loaded'" +
            //                        "END) as LoadedStatus,CL_ClientName as Client, " +
            //                        "Comments, StartTime from RTM_IPVDetails, RTM_Records, RTM_Client_List where RTM_IPVDetails.R_Id = RTM_Records.R_ID and R_Client = CL_ID and SubTask_Id ='213' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) BETWEEN @from and @to and UserName=@emp order by SKU_Id";


        }
        // da.Fill(dt);
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
        if (dt.Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=StrikerateDetails-(" + ddlEmp.SelectedItem.Text + ")" + txtFrom.Text + " to " + txtTo.Text + ".csv");
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
            printIPVDetails();
        }
        catch (Exception)
        {

        }
    }
}