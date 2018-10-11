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

public partial class OrderDeskHourlyTaskReport : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());

    SqlDataAdapter da;
    DataTable dtResult = new DataTable();
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();

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
            BindEmp();
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
            dc = new DataColumn("Client");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Task");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("SubTask");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("CountOfOrders", typeof(Int32));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Date");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("StartTime");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("EndTime");
            dtResult.Columns.Add(dc);
        }
        catch (Exception)
        {

        }
    }

    private DataSet getRecords()
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from",  DateTime.Parse(txtFromTime.Text).ToString("HH:mm:ss")),
               new SqlParameter("@to",DateTime.Parse(txtToTime.Text).ToString("HH:mm:ss")),
                 new SqlParameter("@fromtxt",txtFrom.Text),
               new SqlParameter("@emp",ddlEmp.SelectedItem.Text),
            
           };

        if (ds.Tables.Contains("records"))
        {
            ds.Tables.Remove(ds.Tables["records"]);
        }
        if (ddlEmp.SelectedIndex == 0)
        {
            
            SQlQuery = "select R_ID, R_User_Name, CL_ClientName,  TL_Task, STL_SubTask from RTM_Records left join rtm_client_list on R_Client = CL_ID left join rtm_task_list on R_Task = TL_ID  left join rtm_subtask_list on R_SubTask = STL_ID where R_TeamId = 1 and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) =@fromtxt " +
                               "and convert(char(8), R_Start_Date_Time, 108) BETWEEN @from AND @to  Group by  CL_ClientName,  TL_Task,  STL_SubTask, R_ID, R_User_Name";
        }
        else
        {
           
            SQlQuery = "select R_ID, R_User_Name, CL_ClientName,  TL_Task, STL_SubTask from RTM_Records left join rtm_client_list on R_Client = CL_ID left join rtm_task_list on R_Task = TL_ID  left join rtm_subtask_list on R_SubTask = STL_ID where R_TeamId = 1 and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) =@fromtxt  and R_User_Name=@emp " +
                               "and convert(char(8), R_Start_Date_Time, 108) BETWEEN  @from AND @to  Group by  CL_ClientName,  TL_Task,  STL_SubTask, R_ID, R_User_Name";
        }

        //da.Fill(ds, "records");
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "records");
        return ds;
    }

    private DataSet getCountOfOrders(int rId)
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from",  DateTime.Parse(txtFromTime.Text).ToString("HH:mm:ss")),
               new SqlParameter("@to",DateTime.Parse(txtToTime.Text).ToString("HH:mm:ss")),
               new SqlParameter("@fromtxt",txtFrom.Text),
               new SqlParameter("@id",rId),
           };
        if (ds.Tables.Contains("count"))
        {
            ds.Tables.Remove(ds.Tables["count"]);
        }
       
        SQlQuery = "select Count(SKU_Id) as SKU_IdCount from RTM_IPVDetails where R_Id =@id and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, StartTime))) =@fromtxt and convert(char(8), StartTime, 108) BETWEEN @from AND @to";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "count");
        return ds;
    }

    private DataSet getRecords1()
    {
        
        DateTime fromDate = Convert.ToDateTime(txtFrom.Text + " " + txtFromTime.Text);
        DateTime toDate = fromDate.AddHours(Convert.ToDouble(ddlHours.SelectedItem.Text));
        if (ds.Tables.Contains("records"))
        {
            ds.Tables.Remove(ds.Tables["records"]);
        }
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from",fromDate),
               new SqlParameter("@to",toDate),
               new SqlParameter("@emp",ddlEmp.SelectedItem.Text),
       
           };
        if (ddlEmp.SelectedIndex == 0)
        {
            
          
            SQlQuery = "select R_ID, R_User_Name, CL_ClientName,  TL_Task, STL_SubTask from RTM_Records left join rtm_client_list on R_Client = CL_ID left join rtm_task_list on R_Task = TL_ID  left join rtm_subtask_list on R_SubTask = STL_ID where R_TeamId = 1 and  " +
                               "R_Start_Date_Time BETWEEN @from AND @to  Group by  CL_ClientName,  TL_Task,  STL_SubTask, R_ID, R_User_Name";
        }
        else
        {
            
            SQlQuery = "select R_ID, R_User_Name, CL_ClientName,  TL_Task, STL_SubTask from RTM_Records left join rtm_client_list on R_Client = CL_ID left join rtm_task_list on R_Task = TL_ID  left join rtm_subtask_list on R_SubTask = STL_ID where R_TeamId = 1 and R_User_Name=@emp " +
                               "and R_Start_Date_Time BETWEEN @from AND @to  Group by  CL_ClientName,  TL_Task,  STL_SubTask, R_ID, R_User_Name";
        }

        //da.Fill(ds, "records");
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "records");
        return ds;
    }

    private DataSet getCountOfOrders1(int rId)
    {
        DateTime fromDate = Convert.ToDateTime(txtFrom.Text + " " + txtFromTime.Text);
        DateTime toDate = fromDate.AddHours(Convert.ToDouble(ddlHours.SelectedItem.Text));
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from",fromDate),
               new SqlParameter("@to",toDate),
               new SqlParameter("@id",rId),
       
           };
        if (ds.Tables.Contains("count"))
        {
            ds.Tables.Remove(ds.Tables["count"]);
        }
        
        SQlQuery = "select Count(SKU_Id) as SKU_IdCount from RTM_IPVDetails where R_Id =@id and StartTime BETWEEN @from AND @to";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "count");
        return ds;
    }

    private DataTable getHourlyTasks()
    {
        SQlQuery = "SELECT username, Client, Task, SubTask, SUM(CountOfOrders) as CountOfOrders from RTM_HourlyTaskTemp GROUP BY Client, Task, SubTask, username";
        dt = objDB.DBExecDataTable(SQlQuery);
        return dt;
    }

    private void PrintReport()
    {
        //DataRow dr;

        // BuildTable();

        ds = getRecords1();

        if (ds.Tables["records"].Rows.Count > 0)
        {
            foreach (DataRow dr1 in ds.Tables["records"].Rows)
            {
                int count = 0;
                ds = getCountOfOrders1(Convert.ToInt32(dr1["R_ID"]));
                if (ds.Tables["count"].Rows.Count > 0)
                {
                    if (ds.Tables["count"].Rows[0]["SKU_IdCount"].ToString().Length > 0)
                    {
                        count = Convert.ToInt32(ds.Tables["count"].Rows[0]["SKU_IdCount"]);
                    }
                }

                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@client",dr1["CL_ClientName"].ToString()),
                    new SqlParameter("@task",dr1["TL_Task"].ToString()),
                    new SqlParameter("@subtask",dr1["STL_SubTask"].ToString()),
                    new SqlParameter("@count",count),
                    new SqlParameter("@user",dr1["R_User_Name"].ToString())
                };

                SQlQuery = "INSERT INTO RTM_HourlyTaskTemp (Client, Task, SubTask, CountOfOrders, username) values (@client, @task, @subtask, @count, @user)";
                bool result1 = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, param);
             
                //dr = dtResult.NewRow();

                //dr["Client"] = dr1["CL_ClientName"];
                //dr["Task"] = dr1["TL_Task"];
                //dr["SubTask"] = dr1["STL_SubTask"];
                //ds = getCountOfOrders(Convert.ToInt32(dr1["R_ID"]));
                //if (ds.Tables["count"].Rows.Count > 0)
                //{
                //    dr["CountOfOrders"] = ds.Tables["count"].Rows[0]["SKU_IdCount"].ToString();
                //}
                //else
                //{
                //    dr["CountOfOrders"] = "0";
                //}
                //dr["Date"] = txtFrom.Text ;
                //dr["StartTime"] = txtFromTime.Text ;
                //dr["EndTime"] = txtToTime.Text ;

                //dtResult.Rows.Add(dr);
            }
            dt = new DataTable();
            dt = getHourlyTasks();
            SQlQuery = "TRUNCATE TABLE RTM_HourlyTaskTemp";
            result = objDB.DBExecuteNoNQuery(SQlQuery);

           
            if (dt.Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=HourlyTaskReport-(" + Convert.ToDateTime(txtFrom.Text + " " + txtFromTime.Text) + " to " + Convert.ToDateTime(txtFrom.Text + " " + txtFromTime.Text).AddHours(Convert.ToDouble(ddlHours.SelectedItem.Text)) + ") .csv");
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


            //var query = (from r in dtResult.AsEnumerable()
            //             select new
            //             {
            //                 Client = r.Field<string>("Client"),
            //                 Task = r.Field<string>("Client"),
            //                 SubTask = r.Field<string>("SubTask"),
            //                 CountOfOrders = r.Field<int>("CountOfOrders")
            //             }).Distinct();

            //string strScript;
            //Session["rptSrc"] = dtResult;

            //strScript = "<script language=javascript>window.open('OrderDeskHourlyTaskReportDisplay.aspx')</script>";
            //ClientScript.RegisterClientScriptBlock(this.GetType(), "strScript", strScript);
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtFrom.Text.Length <= 0)
            {
                lblError.Text = "Select Date";
                lblError.ForeColor = Color.Red;
                return;
            }
            if (txtFromTime.Text.Length <= 0)
            {
                lblError.Text = "Select FromTime";
                lblError.ForeColor = Color.Red;
                return;
            }
            //if (txtToTime.Text.Length <= 0)
            //{
            //    lblError.Text = "Select ToTime";
            //    lblError.ForeColor = Color.Red;
            //    return;
            //}
            PrintReport();
        }
        catch (Exception)
        {


        }
    }
}