using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Net;
using System.Text;
using DAL;

public partial class ClentUsageTeam : System.Web.UI.Page
{
    DataTable dtResult = new DataTable();
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlDataAdapter da;
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();
    string TeamName;
    string type;
    DateTime lastLogin;
    DateTime lastLogout;
    string[] x;
    double[] y;
    string sQuery;
    SqlDBHelper objDB = new SqlDBHelper();
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
        type = Request.QueryString["type"].ToString();
        if (!IsPostBack)
        {

            ds = GetTeamName(Convert.ToInt32(Session["team"]));

            if (ds.Tables["Team"].Rows.Count > 0)
            {
                TeamName = ds.Tables["Team"].Rows[0]["T_TeamName"].ToString();
            }
            try
            {
                LastLogin1();
                LastLogout1();
            }
            catch (Exception)
            {


            }

            BindPieChart1();
        }
    }

    private DataSet GetTeamName(int id)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@id", id)
        };
        if (ds.Tables.Contains("Team"))
        {
            ds.Tables.Remove(ds.Tables["Team"]);
        }
        sQuery = "select T_TeamName from RTM_Team_List where T_ID= @id";
        
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Team");
        return ds;
    }


    private DataTable LoadClient()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(Session["team"])),
            new SqlParameter("@from", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
            new SqlParameter("@to", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        sQuery = "SELECT C.CL_ClientName as Client, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Client_List C where  B.R_Client = C.CL_ID and R_TeamId=@team and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from  AND @to GROUP BY C.CL_ClientName ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
       
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        return dt;
    }
    private DataSet LoadClientPercent()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(Session["team"])),
            new SqlParameter("@from", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
            new SqlParameter("@to", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("clientPer"))
        {
            ds.Tables.Remove(ds.Tables["clientPer"]);
        }
        sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records where R_TeamId=@team and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from AND @to";
        
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "clientPer");
        return ds;
    }

    private DataSet LoadTaskPercent()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(Session["team"])),
            new SqlParameter("@from", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
            new SqlParameter("@to", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
        };
        if (ds.Tables.Contains("clientPer"))
        {
            ds.Tables.Remove(ds.Tables["clientPer"]);
        }
        if (Session["access"].ToString() == "3" || Session["access"].ToString() == "2" || Session["access"].ToString() == "1")
        {
            sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B where R_TeamId=@team  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to";
        }
        else
        {
            
            sQuery = "SELECT sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records where R_TeamId=@team  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from  AND @to";
           
        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "clientPer");
       
        return ds;
    }

    private void BuildTable()
    {
        dtResult = new DataTable();
        DataColumn dc;

        dc = new DataColumn("Client");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Total Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Percent");
        dtResult.Columns.Add(dc);


    }

    private void BuildTaskTable()
    {
        dtResult = new DataTable();
        DataColumn dc;

        dc = new DataColumn("Task");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Total Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Percent");
        dtResult.Columns.Add(dc);


    }

    private void LoadClientUsage()
    {
        DataRow dr;
        double totalHours = 0;

        ds = LoadClientPercent();
        if (ds.Tables["clientPer"].Rows.Count > 0)
        {
            totalHours = Math.Round(Convert.ToDouble(ds.Tables["clientPer"].Rows[0]["hour"]) + (Convert.ToDouble(ds.Tables["clientPer"].Rows[0]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["clientPer"].Rows[0]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
        }
        dt = new DataTable();
        BuildTable();
        dt = LoadClient();
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr1 in dt.Rows)
            {
                dr = dtResult.NewRow();

                dr["Client"] = dr1["Client"];
                double Dur = Math.Round(Convert.ToDouble(dr1["hour"]) + (Convert.ToDouble(dr1["minute"]) / 60) + (Convert.ToDouble(dr1["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                dr["Total Hours"] = Dur;
                dr["Percent"] = Math.Round((Dur / totalHours) * 100, 2, MidpointRounding.AwayFromZero);
                dtResult.Rows.Add(dr);
            }
        }
    }

    private void LoadTaskUsage()
    {
        DataRow dr;
        double totalHours = 0;

        ds = LoadTaskPercent();
        if (ds.Tables["clientPer"].Rows.Count > 0)
        {
            totalHours = Math.Round(Convert.ToDouble(ds.Tables["clientPer"].Rows[0]["hour"]) + (Convert.ToDouble(ds.Tables["clientPer"].Rows[0]["minute"]) / 60) + (Convert.ToDouble(ds.Tables["clientPer"].Rows[0]["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
        }
        dt = new DataTable();
        BuildTaskTable();
        dt = LoadUserTasks();
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr1 in dt.Rows)
            {
                dr = dtResult.NewRow();

                dr["Task"] = dr1["Task"];
                double Dur = Math.Round(Convert.ToDouble(dr1["hour"]) + (Convert.ToDouble(dr1["minute"]) / 60) + (Convert.ToDouble(dr1["seconds"]) / 3600), 2, MidpointRounding.AwayFromZero);
                dr["Total Hours"] = Dur;
                dr["Percent"] = Math.Round((Dur / totalHours) * 100, 2, MidpointRounding.AwayFromZero);
                dtResult.Rows.Add(dr);
            }
        }
    }

    private DataTable LoadUserTasks()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(Session["team"])),
            new SqlParameter("@from", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
            new SqlParameter("@to", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString()),
            new SqlParameter("@lastLogin", lastLogin),
            new SqlParameter("@lastLogout", lastLogout),
            new SqlParameter("@user", Session["username"].ToString()),
            new SqlParameter("@date", DateTime.Now.AddDays(-1).ToShortDateString())
        };
        if (Session["access"].ToString() == "3" || Session["access"].ToString() == "2" || Session["access"].ToString() == "1")
        {
            sQuery = "SELECT C.TL_Task as Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Task_List C where  B.R_Task = C.TL_ID and R_TeamId=@team  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to GROUP BY C.TL_Task";
        }
        else
        {
            if (Session["preference"].ToString() == "2")
            {
                sQuery = "SELECT C.TL_Task as Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Task_List C where  B.R_Task = C.TL_ID and R_User_Name=@user  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date GROUP BY C.TL_Task ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
            }
            else
            {
                sQuery = "SELECT C.TL_Task as Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Task_List C where  B.R_Task = C.TL_ID and R_User_Name=@user  and R_Duration != 'HH:MM:SS' and R_Start_Date_Time BETWEEN @lastLogin and @lastLogout GROUP BY C.TL_Task ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
            }
            
        }
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        
        return dt;
    }

    private void BindPieChart1()
    {
        if (type == "TeamClientsUsage")
        {
            LoadClientUsage();
            
            btnExport.ToolTip = "Export all Clients";
        }
        else if (type == "Tasks")
        {
            LoadTaskUsage();
            
        }

        if (dtResult.Rows.Count > 20)
        {
            x = new string[19];
            y = new double[19];
        }
        else
        {
            x = new string[dtResult.Rows.Count];
            y = new double[dtResult.Rows.Count];
        }

        checked
        {
            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                //TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]); 
                if (i >= 19)
                {
                    break;
                }
                x[i] = dtResult.Rows[i][0].ToString();
                //y[i] = Convert.ToDouble(dt.Rows[i][1] + "." + dt.Rows[i][2]);
                //y[i] = Math.Round(Convert.ToDouble(dt.Rows[i][1]) + Convert.ToDouble(dt.Rows[i][2]) / 60 + Convert.ToDouble(dt.Rows[i][3]) / 3600, 2, MidpointRounding.AwayFromZero);
                y[i] = Convert.ToDouble(dtResult.Rows[i][1]);
            }
        }
       

        //string[] x = new string[dt.Rows.Count];
        //int[] y = new int[dt.Rows.Count];

        //for (int i = 0; i < dt.Rows.Count; i++)
        //{
        //    TimeSpan span1 = TimeSpan.Parse(dt.Rows[i][1] + ":" + dt.Rows[i][2] + ":" + dt.Rows[i][3]);
        //    if (span1.TotalMinutes != 0)
        //    {
        //        x[i] = dt.Rows[i][0].ToString();
        //        y[i] = Convert.ToInt32(span1.TotalMinutes);
        //    }
        //}
        //Chart1.Series[0].Points.DataBindXY(x, y);
        ////Chart1.ChartAreas[0].AxisY.Minimum = Chart1.Series[0].Points.FindMinByValue("Y1", 0).YValues[0] - 5;
        ////Chart1.ChartAreas[0].AxisY.Maximum = Chart1.Series[0].Points.FindMaxByValue("Y1", 0).YValues[0] + 5;
        ////Chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 90;
        ////Chart1.ChartAreas["ChartArea1"].AxisX.Maximum = 100;
        //Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
        //Chart1.ChartAreas[0].AxisX.LabelStyle.Angle = 45;


        Chart1.Series[0].Points.DataBindXY(x, y);

        this.Chart1.Series[0].BorderWidth = 1;
        this.Chart1.Series[0].Label = "#VALY";
        this.Chart1.Series[0]["BarLabelStyle"] = "Outside";
        this.Chart1.Series[0].LegendText = "#VALX (#VALY - #PERCENT)"; // "#VALX (#PERCENT)";
        Chart1.ChartAreas[0].AxisY.LabelStyle.Angle = 45;
        //Chart1.Series[0].ChartType = SeriesChartType.Column;
        //Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;
        //Chart1.Legends[0].Enabled = true;
        //Chart1.Series[0].Points[0].CustomProperties = "Exploded=true";

        if (type == "TeamClientsUsage")
        {
            Chart1.ChartAreas["ChartArea1"].AxisX.Title = "Clients (Showing Top 20 Clients)";
            Chart1.ChartAreas["ChartArea1"].AxisY.Title = "In Hours";
            Chart1.Titles.Add(CreateTitle("RTM Client Usage - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
        }
        else if (type == "Tasks")
        {
            Chart1.ChartAreas["ChartArea1"].AxisX.Title = "Tasks";
            Chart1.ChartAreas["ChartArea1"].AxisY.Title = "In Hours";
            if (Session["access"].ToString() == "4")
            {
                Chart1.Titles.Add(CreateTitle("RTM Prod-Task Summary - (" + lastLogin + ")"));

            }
            else
            {
                Chart1.Titles.Add(CreateTitle("RTM Prod-Task Summary - " + TeamName + " (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString() + ") - (" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + ")"));
            }

        }

        Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

        Chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
        Chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;


    }

    public Title CreateTitle(string name)
    {
        Title title = new Title();
        title.Text = name;
        title.ShadowColor = System.Drawing.Color.FromArgb(32, 0, 0, 0);
        title.Font = new System.Drawing.Font("Trebuchet MS", 14F, FontStyle.Bold);
        title.ShadowOffset = 3;
        title.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
        return title;
    }

    private void PrintChart()
    {
        ds = GetTeamName(Convert.ToInt32(Session["team"]));

        if (ds.Tables["Team"].Rows.Count > 0)
        {
            TeamName = ds.Tables["Team"].Rows[0]["T_TeamName"].ToString();
        }

        BindPieChart1();

        Document pdfDoc = new Document(PageSize.A3, 10f, 10f, 10f, 0f);
        PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        pdfDoc.Open();

        using (MemoryStream stream = new MemoryStream())
        {




            //iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imagePath);

            //gif.ScalePercent(24f);
            //gif.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;

            Chart1.SaveImage(stream, ChartImageFormat.Png);
            iTextSharp.text.Image chartImage = iTextSharp.text.Image.GetInstance(stream.GetBuffer());
            chartImage.ScalePercent(75f);
            //pdfDoc.Add(gif);
            pdfDoc.Add(chartImage);
        }

        pdfDoc.Close();
        Response.ContentType = "application/pdf";
        if (type == "TeamClientsUsage")
        {
            Response.AddHeader("content-disposition", "attachment;" +
                                      "filename=RTM Client Usage - " + TeamName + ".pdf");
        }
        else if (type == "Tasks")
        {
            Response.AddHeader("content-disposition", "attachment;" +
                                     "filename=RTM Task Usage - " + TeamName + ".pdf");
        }

        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Write(pdfDoc);
        Response.End();

        //Response.ContentType = "application/pdf";


        //Response.AddHeader("content-disposition", "inline;filename=TestPage.pdf");


        ////Response.AddHeader("content-disposition", "attachment;filename=TestPage.pdf");       

        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //StringWriter sw = new StringWriter();
        //HtmlTextWriter hw = new HtmlTextWriter(sw);
        //this.Page.RenderControl(hw);
        //StringReader sr = new StringReader(sw.ToString());
        //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
        //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //pdfDoc.Open();
        ////htmlparser.Parse(sr);
        //pdfDoc.Close();
        //Response.Write(pdfDoc);
        //Response.End();

    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        PrintChart();
    }

    private void LastLogin1()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {           
            new SqlParameter("@date", DateTime.Now.AddDays(-1)),
            new SqlParameter("@user", Session["username"].ToString())
        };
        try
        {
            if (Session["preference"].ToString() == "2")
            {
                sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) <= @date ORDER BY LA_ID DESC";
            }
            else
            {
                sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) <= @date and LA_Log_Action = 'First Activity' ORDER BY LA_ID DESC";
                
            }
           
            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "login");
            if (ds.Tables["login"].Rows.Count > 0)
            {
                lastLogin = Convert.ToDateTime(ds.Tables["login"].Rows[0]["LA_Start_Date_Time"]);
            }
            else
            {
                lastLogin = DateTime.Now.AddDays(-1);
            }
        }
        catch (Exception)
        {

        }
    }

    private void LastLogout1()
    {
        try
        {
            SqlParameter[] parameters = new SqlParameter[]
            {           
                new SqlParameter("@date", lastLogin.ToShortDateString()),
                new SqlParameter("@user", Session["username"].ToString()),
                new SqlParameter("@lastLogin", lastLogin)
            };
            if (Session["preference"].ToString() == "2")
            {
                sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) =@date ORDER BY LA_ID Desc";
               
            }
            else
            {
                sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @user and LA_Start_Date_Time >= @lastLogin and LA_Log_Action = 'Last Activity' ORDER BY LA_ID";
              
            }
            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "logout");

            if (ds.Tables["logout"].Rows.Count > 0)
            {
                lastLogout = Convert.ToDateTime(ds.Tables["logout"].Rows[0]["LA_Start_Date_Time"]);
            }
            else
            {
                lastLogout = lastLogin;
            }
        }
        catch (Exception)
        {

        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        ds = GetTeamName(Convert.ToInt32(Session["team"]));

        if (ds.Tables["Team"].Rows.Count > 0)
        {
            TeamName = ds.Tables["Team"].Rows[0]["T_TeamName"].ToString();
        }
        StringBuilder sb = new StringBuilder();
        if (type == "TeamClientsUsage")
        {
            LoadClientUsage();
            Response.AddHeader("content-disposition", "attachment;filename=Client Usage-" + TeamName + ".csv");

        }
        else if (type == "Tasks")
        {
            
            LoadTaskUsage();
            Response.AddHeader("content-disposition", "attachment;filename=Task Usage-" + TeamName + ".csv");

        }
        Response.Clear();
        Response.Buffer = true;

        Response.Charset = "";
        Response.ContentType = "application/text";
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
        

        Response.Output.Write(sb.ToString());
        Response.Flush();
        Response.End();
    }
}