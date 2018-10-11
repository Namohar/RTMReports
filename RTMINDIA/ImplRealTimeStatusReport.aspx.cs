using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Text;
using DAL;
public partial class ImplRealTimeStatusReport : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlDataAdapter da;

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

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        PrintData();
    }

    private void BuildTable()
    {
        DataColumn dc;
        dt = new DataTable();

        dc = new DataColumn("Request ID");
        dt.Columns.Add(dc);

        dc = new DataColumn("Product");
        dt.Columns.Add(dc);

        dc = new DataColumn("Client");
        dt.Columns.Add(dc);

        dc = new DataColumn("Status");
        dt.Columns.Add(dc);

        dc = new DataColumn("Time Tracking Code");
        dt.Columns.Add(dc);

        dc = new DataColumn("Type");
        dt.Columns.Add(dc);
    }

    private DataSet GetData()
    {
        if (ds.Tables.Contains("data"))
        {
            ds.Tables.Remove(ds.Tables["data"]);
        }
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", txtFrom.Text),
               new SqlParameter("@to", txtTo.Text),
           };

       

        SQlQuery = "select RequestId = PARSENAME(REPLACE(R_Comments, ';', '.'),3), " +
      "CASE PARSENAME(REPLACE(R_Comments, ';', '.'), 2) " +
         "WHEN 'INP' THEN 'In Progress'  WHEN 'COM' THEN 'Completed'  WHEN 'CMP' THEN 'Completed' ELSE PARSENAME(REPLACE(R_Comments, ';', '.'), 2) " +
      "END as [Status], " +
       "CL_ClientName, TL_Task, STL_SubTask,  R_Duration from  " +
       "RTM_Records, RTM_Client_List, rtm_task_list, rtm_subtask_list  " +
       "where R_Client = CL_ID and R_Task = TL_ID and R_SubTask = STL_ID and R_TeamId ='13' " +
       "and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and R_Duration != '' " +
       "and STL_SubTask <> 'FA Support' and STL_SubTask <> 'Past Due Balance' and STL_SubTask <> 'Report & Queries' and STL_SubTask <> 'Service Order Validation' and STL_SubTask <>'Service Orders' and STL_SubTask <> 'Threshold Activity' and STL_SubTask <> 'Adhoc' and STL_SubTask <> 'Idle Time' and STL_SubTask <>'NEW Client' and STL_SubTask <> 'IPA & Service code' AND STL_SubTask <> 'TRG/DOC & Service Code' and STL_ServiceCode <> '000001006 - Learning | General/Internal'";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "data");
        return ds;
    }

    private void PrintData()
    {
        try
        {
            DataRow dr;
            BuildTable();

            ds = GetData();

            if (ds.Tables["data"].Rows.Count > 0)
            {
                foreach (DataRow dr1 in ds.Tables["data"].Rows)
                {
                    dr = dt.NewRow();

                    dr["Request ID"] = dr1["RequestId"];
                    dr["Product"] = dr1["TL_Task"];
                    dr["Client"] = dr1["CL_ClientName"];
                    dr["Status"] = dr1["Status"];
                    dr["Time Tracking Code"] = dr1["R_Duration"];
                    dr["Type"] = dr1["STL_SubTask"];

                    dt.Rows.Add(dr);
                }

                gvStatus.DataSource = dt;
                gvStatus.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            PrintData();

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Real Time Status Report " + txtFrom.Text + " to " + txtTo.Text + ".csv");
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
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }
    }
}