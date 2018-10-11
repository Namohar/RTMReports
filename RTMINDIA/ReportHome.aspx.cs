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

public partial class ReportHome : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    int absentflag = 0;
    SqlDataAdapter da;
    DataTable dt = new DataTable();
    DataTable dt2 = new DataTable();
    DataSet ds = new DataSet();
    DataTable dtResult = new DataTable();
    clsRealTimeReports objReal = new clsRealTimeReports();
    SqlDBHelper objDB = new SqlDBHelper();
    clsUtilization objUtil = new clsUtilization();
    List<string> lstDate = new List<string>();
    List<string> lstUser = new List<string>();
    string sQuery;
    bool flag = false;
    bool loginFlag = false;

    TimeSpan span1;
    TimeSpan span2;
    TimeSpan span3;
    TimeSpan span4;
    TimeSpan span5;
    TimeSpan span6;
    TimeSpan span7;
    TimeSpan time4;
    TimeSpan span8;
    TimeSpan span9;
    TimeSpan span10;
    TimeSpan span11;
    TimeSpan span12;
    TimeSpan span13;
    TimeSpan span14;
    TimeSpan span15;
    DateTime temp;
    string start1 = null;
    string end1 = null;
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
            rbTsheet.Visible = false;
            LoadEmp();
            rbDisplay.Visible = false;
            rbExport.Visible = false;
            if (rbRealTime.Checked == true)
            {
                //if (Session["access"].ToString() != "4")
                //{
                //    ddlEmp.Items.Insert(0, "All");
                //    ddlEmp.SelectedIndex = 0;
                //}

                datepicker.Visible = true;
                lblDate.Visible = true;
                rbUser.Visible = false;
                rbTeam.Visible = false;
            }

            if (Session["access"].ToString() == "4")
            {
                //ddlEmp.Items.RemoveAt(0);
                //rbRealTime.Visible = false;
                //rbClient.Visible = false;
                //rbTask.Visible = false;
                rbCompleteLog.Visible = false;
                //rbRealTime.Checked = false;
                //rbSummary.Checked = true;
                //rbTsheet.Checked = true;
                datepicker.Visible = true;
                lblDate.Visible = true;

                rbUser.Visible = false;
                rbTeam.Visible = false;
                rbEarlyLogoff.Visible = false;

            }
        }
    }

    protected void rbCompleteLog_CheckedChanged(object sender, EventArgs e)
    {
        LoadEmp();
        datepickerTo.Visible = false;
        lblTo.Visible = false;
        if (rbCompleteLog.Checked == true)
        {
            datepicker.Visible = false;
            lblDate.Visible = false;
        }
        else
        {
            datepicker.Visible = true;
            lblDate.Visible = true;
        }
        lblEmp.Visible = true;
        ddlEmp.Visible = true;
        btnPrint.Text = "Display";
        rbDisplay.Visible = false;
        rbExport.Visible = false;
        dvAbsent.Visible = false;
    }

    protected void rbTask_CheckedChanged(object sender, EventArgs e)
    {
        LoadEmp();
        datepicker.Visible = true;
        lblDate.Visible = true;
        datepickerTo.Visible = false;
        lblTo.Visible = false;
        datepicker.Visible = true;
        lblDate.Visible = true;
        lblEmp.Visible = true;
        ddlEmp.Visible = true;
        btnPrint.Text = "Display";
        rbDisplay.Visible = false;
        rbExport.Visible = false;
        dvAbsent.Visible = false;
    }

    protected void rbClient_CheckedChanged(object sender, EventArgs e)
    {
        LoadEmp();
        datepicker.Visible = true;
        lblDate.Visible = true;
        datepickerTo.Visible = false;
        lblTo.Visible = false;
        datepicker.Visible = true;
        lblDate.Visible = true;
        lblEmp.Visible = true;
        ddlEmp.Visible = true;
        btnPrint.Text = "Display";
        rbDisplay.Visible = false;
        rbExport.Visible = false;
        dvAbsent.Visible = false;
    }

    protected void rbSummary_CheckedChanged(object sender, EventArgs e)
    {
        //lblNotice.Visible = false;
        gvEarlyLogoff.Visible = false;
        gvIncorrectData.Visible = false;
        gvTSheet.Visible = false;
        GridView1.Visible = false;
        LoadEmp();
        datepickerTo.Visible = false;
        lblTo.Visible = false;
        if (Session["access"].ToString() != "4")
        {
            //ddlEmp.Items.Insert(0, "All");
            //ddlEmp.SelectedIndex = 0;

            datepickerTo.Visible = true;
            lblTo.Visible = true;
            rbUser.Visible = true;
            rbTeam.Visible = true;
        }

        if (rbUser.Checked == true)
        {
            datepickerTo.Visible = false;
            lblTo.Visible = false;
        }
        else if (rbTeam.Checked == true)
        {
            datepickerTo.Visible = true;
            lblTo.Visible = true;
        }

        datepicker.Visible = true;
        lblDate.Visible = true;
        dvAbsent.Visible = false;
        lblEmp.Visible = true;
        ddlEmp.Visible = true;
        if (rbUser.Checked == true)
        {
            LoadEmp();
        }
        else if (rbTeam.Checked == true)
        {
            LoadTeams();
        }
        //gvTSheet.Visible = false;
        btnPrint.Text = "Display";
        rbDisplay.Visible = false;
        rbExport.Visible = false;
    }

    protected void rbRealTime_CheckedChanged(object sender, EventArgs e)
    {
        //lblNotice.Visible = false;
        gvEarlyLogoff.Visible = false;
        gvIncorrectData.Visible = false;
        gvTSheet.Visible = false;
        GridView1.Visible = false;
        LoadEmp();
        if (Session["access"].ToString() != "4")
        {
            //ddlEmp.Items.Insert(0, "All");
            //ddlEmp.SelectedIndex = 0;
            //datepicker.Visible = false;
            //lblDate.Visible = false;
        }


        datepickerTo.Visible = false;
        lblTo.Visible = false;
        lblEmp.Visible = true;
        ddlEmp.Visible = true;
        rbUser.Visible = false;
        rbTeam.Visible = false;
        btnPrint.Text = "Display";
        rbDisplay.Visible = false;
        rbExport.Visible = false;
        dvAbsent.Visible = false;
    }

    protected void ddlEmp_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rbUser.Checked == true)
        {
            dt = objReal.GetPreference("User", "", ddlEmp.SelectedItem.Text);
            if (dt.Rows.Count > 0)
            {
                Session["preference"] = dt.Rows[0]["T_Preference"].ToString();
            }
        }
        else if (rbTeam.Checked == true)
        {
            dt = objReal.GetPreference("Team", ddlEmp.SelectedValue, "");
            if (dt.Rows.Count > 0)
            {
                Session["preference"] = dt.Rows[0]["T_Preference"].ToString();
            }
        }
        if (rbSummary.Checked == true)
        {
            if (rbUser.Checked == true)
            {
                dt = objReal.GetPreference("User", "", ddlEmp.SelectedItem.Text);
                if (dt.Rows.Count > 0)
                {
                    Session["preference"] = dt.Rows[0]["T_Preference"].ToString();
                }
                datepickerTo.Visible = false;
                lblTo.Visible = false;
            }
            else if (rbTeam.Checked == true)
            {
                dt = objReal.GetPreference("Team", ddlEmp.SelectedValue, "");
                if (dt.Rows.Count > 0)
                {
                    Session["preference"] = dt.Rows[0]["T_Preference"].ToString();
                }
                datepickerTo.Visible = true;
                lblTo.Visible = true;
            }
            //if (ddlEmp.SelectedIndex > 0)
            //{
            //    datepickerTo.Visible = false;
            //    lblTo.Visible = false;

            //}
            //else
            //{
            //    datepickerTo.Visible = true;
            //    lblTo.Visible = true;
            //}
        }
        if (rbRealTime.Checked == true)
        {

            if (ddlEmp.SelectedIndex > 0)
            {
                datepicker.Visible = true;
                lblDate.Visible = true;
            }
            else
            {
                datepicker.Visible = false;
                lblDate.Visible = false;
            }
        }
    }

    protected void rbTsheet_CheckedChanged(object sender, EventArgs e)
    {
        //lblNotice.Visible = false;
        gvIncorrectData.Visible = false;
        gvTSheet.Visible = false;
        gvEarlyLogoff.Visible = false;
        GridView1.Visible = false;
        LoadEmp();
        //ddlEmp.Items.Insert(0, "All");
        //ddlEmp.SelectedIndex = 0;
        datepicker.Visible = true;
        lblDate.Visible = true;
        datepickerTo.Visible = true;
        lblTo.Visible = true;
        rbDisplay.Visible = true;
        rbExport.Visible = true;
        if (Session["access"].ToString() == "4")
        {
            lblEmp.Visible = false;
            ddlEmp.Visible = false;
            rbUser.Visible = false;
            rbTeam.Visible = false;
        }
        else
        {
            lblEmp.Visible = true;
            ddlEmp.Visible = true;
            rbUser.Visible = true;
            rbTeam.Visible = true;
        }

        if (rbUser.Checked == true)
        {
            LoadEmp();
            btnPrint.Text = "Submit";
            rbDisplay.Visible = true;
            rbExport.Visible = true;
        }
        else if (rbTeam.Checked == true)
        {
            LoadTeams();
            btnPrint.Text = "Export";
            rbDisplay.Visible = false;
            rbExport.Visible = false;
        }
    }

    protected void rbUser_CheckedChanged(object sender, EventArgs e)
    {
        if (rbUser.Checked == true)
        {
            GridView1.Visible = false;
            LoadEmp();
            if (rbTsheet.Checked == true)
            {
                btnPrint.Text = "Submit";
            }
            if (rbSummary.Checked == true)
            {
                datepickerTo.Visible = false;
                lblTo.Visible = false;
            }
            dvAbsent.Visible = false;
        }
    }

    protected void rbTeam_CheckedChanged(object sender, EventArgs e)
    {
        if (rbTeam.Checked == true)
        {
            GridView1.Visible = false;
            LoadTeams();
            datepickerTo.Visible = true;
            lblTo.Visible = true;
            if (rbTsheet.Checked == true)
            {
                btnPrint.Text = "Export";
            }
            dvAbsent.Visible = false;
        }
    }

    protected void rbIncorrect_CheckedChanged(object sender, EventArgs e)
    {
        //lblNotice.Visible = false;
        gvIncorrectData.Visible = false;
        gvTSheet.Visible = false;
        gvEarlyLogoff.Visible = false;
        LoadEmp();
        //ddlEmp.Items.Insert(0, "All");
        //ddlEmp.SelectedIndex = 0;
        datepicker.Visible = true;
        lblDate.Visible = true;
        datepickerTo.Visible = true;
        lblTo.Visible = true;
        if (Session["access"].ToString() == "4")
        {
            lblEmp.Visible = false;
            ddlEmp.Visible = false;
            rbUser.Visible = false;
            rbTeam.Visible = false;
        }
        else
        {
            lblEmp.Visible = true;
            ddlEmp.Visible = true;
            rbUser.Visible = true;
            rbTeam.Visible = true;
        }

        if (rbUser.Checked == true)
        {
            LoadEmp();
            btnPrint.Text = "Display";
        }
        else if (rbTeam.Checked == true)
        {
            LoadTeams();
            btnPrint.Text = "Display";
        }
        GridView1.Visible = false;
        rbDisplay.Visible = false;
        rbExport.Visible = false;
    }

    protected void rbEarlyLogoff_CheckedChanged(object sender, EventArgs e)
    {
        LoadTeams();
        //lblEmp.Visible = false;
        //ddlEmp.Visible = false;
        rbUser.Visible = false;
        rbTeam.Visible = false;
        datepicker.Visible = true;
        lblDate.Visible = true;
        datepickerTo.Visible = true;
        lblTo.Visible = true;
        GridView1.Visible = false;
        rbDisplay.Visible = false;
        rbExport.Visible = false;
    }

    protected void rbLeave_CheckedChanged(object sender, EventArgs e)
    {
        LoadTeams();
        //lblEmp.Visible = false;
        //ddlEmp.Visible = false;
        rbUser.Visible = false;
        rbTeam.Visible = false;
        datepicker.Visible = true;
        lblDate.Visible = true;
        datepickerTo.Visible = true;
        lblTo.Visible = true;
        rbDisplay.Visible = false;
        rbExport.Visible = false;
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {

        System.Threading.Thread.Sleep(50);
        if (datepicker.Visible == true)
        {
            if (DateTime.TryParse(datepicker.Value, out temp))
            { }
            else
            {
                lblError.Text = "Please Select valid from Date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
        }

        if (datepickerTo.Visible == true)
        {
            if (DateTime.TryParse(datepickerTo.Value, out temp))
            { }
            else
            {
                lblError.Text = "Please Select Valid To Date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
        }
        lblNotice.Visible = false;
        gvIncorrectData.Visible = false;
        gvTSheet.Visible = false;
        if (datepicker.Visible == true)
        {
            if (datepicker.Value.Length > 0)
            {
                lblError.Text = "";
            }
            else
            {
                lblError.Text = "Please Select Date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
        }
        try
        {
            if (rbRealTime.Checked == true)
            {
                printRealTimeReport();

                string strScript;
                Session["rptSrc"] = dtResult;

                strScript = "<script language=javascript>window.open('RealTimeReport.aspx')</script>";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "strScript", strScript);
            }
            else if (rbSummary.Checked == true)
            {
                if (lblEmp.Text == "Select Team" && datepickerTo.Visible == true)
                {
                    if (datepickerTo.Value.Length == 0)
                    {
                        lblError.Text = "Please select To Date";
                        lblError.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (Convert.ToDateTime(datepicker.Value) > Convert.ToDateTime(datepickerTo.Value))
                    {
                        lblError.Text = "From Date should be less than To Date";
                        lblError.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        printAllSummeryReport();

                        if (Session["access"].ToString() == "1")
                        {
                            //ExportToCSV(dtResult);
                            ExportToExcel(dtResult);
                        }
                        else
                        {
                            string strScript;
                            Session["rptSrc"] = dtResult;

                            strScript = "<script language=javascript>window.open('AllSummeryReport.aspx')</script>";
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "strScript", strScript);
                        }
                    }
                }
                else
                {
                    //printSummeryReport();
                    if (Session["preference"].ToString() == "2")
                    {
                        SummaryreportFRO();
                    }
                    else
                    {
                        PrintSummaryReportNew();
                    }
                    string strScript;
                    Session["rptSrc"] = dtResult;

                    strScript = "<script language=javascript>window.open('SummeryReport.aspx')</script>";
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "strScript", strScript);
                }

            }
            else if (rbTsheet.Checked == true)
            {
                PrintTSheetReport();
                //PrintTSheetReport2();
            }
            else if (rbIncorrect.Checked == true)
            {
                BindIncorrectGrid();
            }

            else if (rbEarlyLogoff.Checked == true)
            {
                BindEarlyLogoffGrid();
            }
        }
        catch (Exception)
        {
            //lblError.Text = ex.Message;
        }
    }
    protected void btnReset_Click(object sender, EventArgs e)
    {

    }

    private void LoadEmp()
    {
        //dt.Clear();
        //dt.Clone();

        dt = objReal.LoadEmp(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["user"].ToString(), Session["UID"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();
        lblEmp.Text = "Employee";
        //ddlEmp.Items.Insert(0, "All"); 
        //ddlEmp.SelectedIndex = 0;

        if (rbTsheet.Checked)
        {
            rbDisplay.Visible = true;
            rbExport.Visible = true;
        }
    }

    // Real Time Report start1

    private DataSet LoadUsers()
    {
        ds = objReal.LoadUsers(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), "Users", Session["UID"].ToString());

        return ds;
    }

    private void LoadTeams()
    {
        dt = new DataTable();

        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "T_ID";
        ddlEmp.DataTextField = "T_TeamName";
        ddlEmp.DataBind();

        lblEmp.Text = "Select Team";

        if (rbTsheet.Checked)
        {
            rbDisplay.Visible = false;
            rbExport.Visible = false;
        }

        
    }

    private DataTable LoadUserLastTaskRecord(string user)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user)
        };
        sQuery = "select TOP 1 R_ID, R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Start_Date_Time, R_Duration, R_Status, R_Comments from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where R_User_Name= @user and R_Duration != 'HH:MM:SS' ORDER BY R_ID DESC";

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        return dt;
    }
    private DataTable LoadTaskReportData()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date", datepicker.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        //if (ddlEmp.SelectedIndex == 0 && Session["access"].ToString() != "4")
        //{
        //    sQuery = "select R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Start_Date_Time, R_Duration, R_Status, R_Comments from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where R_Start_Date_Time >= @date and R_Duration != 'HH:MM:SS'";

        //}
        //else
        //{
        //    sQuery = "select R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Start_Date_Time, R_Duration, R_Status, R_Comments from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where R_User_Name = @emp And R_Start_Date_Time >= @date and R_Duration != 'HH:MM:SS'";

        //}

        sQuery = "select R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Start_Date_Time, R_Duration, R_Status, R_Comments,R_System from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where R_User_Name = @emp And R_TimeDate >= @date and R_Duration != 'HH:MM:SS'";

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        // da.Fill(dt);
        return dt;
    }

    private DataSet LoadUserLastLogRecord(string user)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user)
        };
        sQuery = "select TOP 1 LA_ID, LA_User_Name, LA_Log_Action, LA_Start_Date_Time, LA_Duration, LA_Status, LA_Comments, LA_Reason from RTM_Log_Actions where LA_User_Name= @user and LA_Duration != 'HH:MM:SS' ORDER BY LA_ID DESC";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "UserLastLog");
        return ds;
    }

    private DataSet LoadLogReportData()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date", datepicker.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        //if (ddlEmp.SelectedIndex == 0 && Session["access"].ToString() != "4")
        //{
        //    sQuery = "select LA_User_Name, LA_Log_Action, LA_Start_Date_Time, LA_Duration, LA_Status, LA_Comments, LA_Reason from RTM_Log_Actions where LA_Start_Date_Time >= @date and LA_Duration != 'HH:MM:SS'";

        //}
        //else
        //{
        //    sQuery = "select LA_User_Name, LA_Log_Action, LA_Start_Date_Time, LA_Duration, LA_Status, LA_Comments, LA_Reason from RTM_Log_Actions where LA_User_Name = @emp and LA_Start_Date_Time >= @date and LA_Duration != 'HH:MM:SS'";

        //}

        sQuery = "select LA_User_Name, LA_Log_Action, LA_Start_Date_Time, LA_Duration, LA_Status, LA_Comments, LA_Reason from RTM_Log_Actions where LA_User_Name = @emp and LA_TimeDate >= @date and LA_Duration != 'HH:MM:SS' and LA_Reason !='Break' and LA_Reason !='Unassigned Time' and LA_Reason !='Idle Time' ";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "UserLastLog");
        // da.Fill(ds, "UserLastLog");
        return ds;
    }


    private void buildRealTimeTable()
    {
        DataColumn dc;
        try
        {
            dc = new DataColumn("EmpName");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Client");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Task");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("SubTask");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Log");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("StartTime",typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Duration");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Status");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Comments");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("R_System");
            dtResult.Columns.Add(dc);

        }
        catch (Exception)
        {


        }
    }

    private void printRealTimeReport()
    {
        DataRow dr;

        buildRealTimeTable();
        //if (Session["access"].ToString() != "4")
        //{
        //    ds = LoadUsers();
        //}

        //if (ddlEmp.SelectedIndex == 0 && Session["access"].ToString() != "4")
        //{

        //    if (ds.Tables["Users"].Rows.Count > 0)
        //    {
        //        foreach (DataRow dr5 in ds.Tables["Users"].Rows)
        //        {
        //            dt = LoadUserLastTaskRecord(dr5["UL_User_Name"].ToString());
        //        }
        //    }
        //}
        //else
        //{
        //    dt = LoadTaskReportData();
        //}

        dt = LoadTaskReportData();

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr1 in dt.Rows)
            {
                dr = dtResult.NewRow();

                dr["EmpName"] = dr1["R_User_Name"];
                dr["Client"] = dr1["CL_ClientName"];
                dr["Task"] = dr1["TL_Task"];
                dr["SubTask"] = dr1["STL_SubTask"];

                //if (dr1["R_System"].ToString() == "P")
                //{
                //    dr["StartTime"] = String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(dr1["R_Start_Date_Time"]));
                //}
                //else
                //{
                    dr["StartTime"] = dr1["R_Start_Date_Time"];
                //}
   
                dr["Log"] = "";

                dr["Duration"] = dr1["R_Duration"].ToString().Replace("-", "");


                dr["Status"] = dr1["R_Status"];
                dr["Comments"] = dr1["R_Comments"];
                dr["R_System"] = dr1["R_System"];
                dtResult.Rows.Add(dr);

            }
        }
        dt.Clear();
        dt.Clone();
        //if (ddlEmp.SelectedIndex == 0 && Session["access"].ToString() != "4")
        //{
        //    //ds = LoadUsers();
        //    if (ds.Tables["Users"].Rows.Count > 0)
        //    {
        //        foreach (DataRow dr5 in ds.Tables["Users"].Rows)
        //        {
        //            ds = LoadUserLastLogRecord(dr5["UL_User_Name"].ToString());
        //        }
        //    }
        //}
        //else
        //{
            
        //}

        ds = LoadLogReportData();

        if (ds.Tables["UserLastLog"].Rows.Count > 0)
        {

            foreach (DataRow dr1 in ds.Tables["UserLastLog"].Rows)
            {
                if (dr1["LA_Log_Action"].ToString() == "Login")
                {
                    continue;
                }

                //if (dr1["LA_Log_Action"].ToString() == "Shutdown")
                //{
                //    continue;
                //}

                dr = dtResult.NewRow();

                dr["EmpName"] = dr1["LA_User_Name"];
                dr["Client"] = "";
                dr["Task"] = "";
                dr["SubTask"] = "";
                dr["StartTime"] = dr1["LA_Start_Date_Time"];
                dr["Log"] = dr1["LA_Log_Action"];

                dr["Duration"] = dr1["LA_Duration"].ToString().Replace("-", "");


                dr["Status"] = dr1["LA_Status"];
                dr["Comments"] = dr1["LA_Reason"] + "--" + dr1["LA_Comments"];

                dtResult.Rows.Add(dr);
                //continue;
            }
        }

        dtResult.DefaultView.Sort = "StartTime";
        dtResult = dtResult.DefaultView.ToTable();

    }

    //Real Time Report end1

    //Summery Report start1

    private void buildSummeryTable()
    {
        DataColumn dc;
        try
        {
            dc = new DataColumn("EmpName");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Log");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("DateTime", typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Reason");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Duration");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("ReportHeader");
            dtResult.Columns.Add(dc);
        }
        catch (Exception)
        {

        }
    }

    private DataTable LoadSummeryReport()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date", datepicker.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        sQuery = "SELECT * from RTM_Log_Actions where LA_User_Name = @emp and LA_Start_Date_Time >= @date and LA_Duration != 'HH:MM:SS'";

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        return dt;
    }

    private DataTable LoadTaskTotalSummery()
    {
        DateTime datetime1;

        if (start1 == null)
        {
            start1 = datepicker.Value;
        }
        if (end1 == null)
        {
            end1 = datepicker.Value;
            datetime1 = DateTime.Now; //Convert.ToDateTime(end1).AddDays(1);
        }
        else
        {
            datetime1 = Convert.ToDateTime(end1);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@start1", start1),
            new SqlParameter("@datetime1", datetime1),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        sQuery = "select sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records where R_User_Name=@emp and R_Start_Date_Time between @start1 and @datetime1 and R_Duration != 'HH:MM:SS'";

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        return dt;
    }

    private DataSet userLogin()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date", datepicker.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        if (Session["preference"].ToString() == "1")
        {
            sQuery = "select TOP 1 LA_Start_Date_Time from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date and LA_Log_Action ='First Activity' order by LA_Start_Date_Time";

        }
        else
        {
            sQuery = "select TOP 1 LA_Start_Date_Time from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date order By LA_Start_Date_Time";

        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "userLogin");
        //da.Fill(ds, "userLogin");
        return ds;
    }

    private DataSet userLogout(DateTime login)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@login", login),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
            new SqlParameter("@date", login.ToShortDateString())
        };
        if (Session["preference"].ToString() == "1")
        {
            sQuery = "select TOP 1 LA_Start_Date_Time from RTM_Log_Actions where LA_User_Name = @emp and LA_Start_Date_Time >= @login and LA_Log_Action ='Last Activity' ORDER BY LA_Start_Date_Time";

        }
        else
        {
            sQuery = "select TOP 1 LA_Start_Date_Time from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date ORDER BY LA_Start_Date_Time DESC";

        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "userLogout");
        // da.Fill(ds, "userLogout");
        return ds;
    }
    private void printSummeryReport()
    {
        DataRow dr;
        string TaskDuration = "00:00:00";
        string MeetDuration = "00:00:00";
        string BreakDuration = "00:00:00";
        string CallDuration = "00:00:00";
        string peerDuration = "00:00:00";
        string otherDuration = "00:00:00";
        string idleDuration = "00:00:00";
        buildSummeryTable();

        ds = userLogin();

        if (ds.Tables["userLogin"].Rows.Count > 0)
        {
            foreach (DataRow dr2 in ds.Tables["userLogin"].Rows)
            {
                DateTime loginTime = Convert.ToDateTime(dr2["LA_Start_Date_Time"]);
                DateTime logoutTime;// = Convert.ToDateTime(dr2["LA_Start_Date_Time"]);
                ds = userLogout(Convert.ToDateTime(dr2["LA_Start_Date_Time"]));
                if (ds.Tables["userLogout"].Rows.Count > 0)
                {
                    logoutTime = Convert.ToDateTime(ds.Tables["userLogout"].Rows[0]["LA_Start_Date_Time"]);
                    span1 = logoutTime.Subtract(loginTime);
                    string diff = span1.ToString(@"hh\:mm\:ss");
                    dr = dtResult.NewRow();

                    dr["EmpName"] = ddlEmp.SelectedItem.Text;
                    dr["Log"] = "First Activity";
                    dr["DateTime"] = loginTime;
                    dr["Reason"] = "";
                    dr["Duration"] = "00:00:00";

                    dtResult.Rows.Add(dr);

                    ds = LoadTask(ddlEmp.SelectedItem.Text, loginTime, logoutTime);

                    if (ds.Tables["Task"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        dr["EmpName"] = ddlEmp.SelectedItem.Text;
                        dr["Log"] = "Task";
                        // dr["DateTime"] = "";
                        dr["Reason"] = "";
                        dr["Duration"] = TimeSpan.Parse(ds.Tables["Task"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Task"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Task"].Rows[0]["seconds"].ToString());
                        TaskDuration = dr["Duration"].ToString().Replace("-", "");
                        if (dr["Duration"].ToString() != "00:00:00")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }

                    ds = LoadBreak(ddlEmp.SelectedItem.Text, loginTime, logoutTime);

                    if (ds.Tables["Break"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        dr["EmpName"] = ddlEmp.SelectedItem.Text;
                        dr["Log"] = "Locked/Paused";
                        // dr["DateTime"] = "";
                        dr["Reason"] = "Break";
                        dr["Duration"] = TimeSpan.Parse(ds.Tables["Break"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Break"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Break"].Rows[0]["seconds"].ToString());
                        BreakDuration = dr["Duration"].ToString().Replace("-", "");
                        if (dr["Duration"].ToString() != "00:00:00")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }

                    ds = LoadMeeting(ddlEmp.SelectedItem.Text, loginTime, logoutTime);

                    if (ds.Tables["Meeting"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        dr["EmpName"] = ddlEmp.SelectedItem.Text;
                        dr["Log"] = "Locked/Paused";
                        // dr["DateTime"] = "";
                        dr["Reason"] = "Meeting";
                        dr["Duration"] = TimeSpan.Parse(ds.Tables["Meeting"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Meeting"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Meeting"].Rows[0]["seconds"].ToString());
                        MeetDuration = dr["Duration"].ToString().Replace("-", "");
                        if (dr["Duration"].ToString() != "00:00:00")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }

                    ds = LoadCall(ddlEmp.SelectedItem.Text, loginTime, logoutTime);

                    if (ds.Tables["Call"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        dr["EmpName"] = ddlEmp.SelectedItem.Text;
                        dr["Log"] = "Locked/Paused";
                        // dr["DateTime"] = "";
                        dr["Reason"] = "Conference-Call";
                        dr["Duration"] = TimeSpan.Parse(ds.Tables["Call"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Call"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Call"].Rows[0]["seconds"].ToString());
                        CallDuration = dr["Duration"].ToString().Replace("-", "");
                        if (dr["Duration"].ToString() != "00:00:00")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }

                    ds = LoadOthers(ddlEmp.SelectedItem.Text, loginTime, logoutTime);

                    if (ds.Tables["Others"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        dr["EmpName"] = ddlEmp.SelectedItem.Text;
                        dr["Log"] = "Locked/Paused";
                        // dr["DateTime"] = "";
                        dr["Reason"] = "Non Task";
                        dr["Duration"] = TimeSpan.Parse(ds.Tables["Others"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Others"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Others"].Rows[0]["seconds"].ToString());
                        otherDuration = dr["Duration"].ToString().Replace("-", "");
                        if (dr["Duration"].ToString() != "00:00:00")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }

                    ds = LoadPeerSupport(ddlEmp.SelectedItem.Text, loginTime, logoutTime);

                    if (ds.Tables["peerSupport"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        dr["EmpName"] = ddlEmp.SelectedItem.Text;
                        dr["Log"] = "Locked/Paused";
                        // dr["DateTime"] = "";
                        dr["Reason"] = "Peer Support";
                        dr["Duration"] = TimeSpan.Parse(ds.Tables["peerSupport"].Rows[0]["hour"].ToString() + ":" + ds.Tables["peerSupport"].Rows[0]["minute"].ToString() + ":" + ds.Tables["peerSupport"].Rows[0]["seconds"].ToString());
                        peerDuration = dr["Duration"].ToString().Replace("-", "");
                        if (dr["Duration"].ToString() != "00:00:00")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }

                    ds = LoadIdleTimeUser(ddlEmp.SelectedItem.Text, loginTime, logoutTime);
                    if (ds.Tables["IdleTimeUser"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        dr["EmpName"] = ddlEmp.SelectedItem.Text;
                        dr["Log"] = "Locked/Paused";
                        // dr["DateTime"] = "";
                        dr["Reason"] = "Idle Time (User Selected)";
                        dr["Duration"] = TimeSpan.Parse(ds.Tables["IdleTimeUser"].Rows[0]["hour"].ToString() + ":" + ds.Tables["IdleTimeUser"].Rows[0]["minute"].ToString() + ":" + ds.Tables["IdleTimeUser"].Rows[0]["seconds"].ToString());
                        idleDuration = dr["Duration"].ToString().Replace("-", "");
                        if (dr["Duration"].ToString() != "00:00:00")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }

                    //ds = LoadIdleTime(ddlEmp.SelectedItem.Text, loginTime, logoutTime);

                    //if (ds.Tables["IdleTime"].Rows.Count > 0)
                    //{
                    //dr = dtResult.NewRow();

                    //dr["EmpName"] = ddlEmp.SelectedItem.Text;
                    //dr["Log"] = "";
                    //// dr["DateTime"] = "";
                    //dr["Reason"] = "Idle Time (RTM Idle)";
                    //span2 = TimeSpan.Parse(TaskDuration).Add(TimeSpan.Parse(BreakDuration)).Add(TimeSpan.Parse(MeetDuration)).Add(TimeSpan.Parse(CallDuration)).Add(TimeSpan.Parse(otherDuration)).Add(TimeSpan.Parse(peerDuration)).Add(TimeSpan.Parse(idleDuration));
                    //span3 = TimeSpan.Parse(diff).Subtract(span2);

                    //dr["Duration"] = span3; //TimeSpan.Parse(ds.Tables["IdleTime"].Rows[0]["hour"].ToString() + ":" + ds.Tables["IdleTime"].Rows[0]["minute"].ToString() + ":" + ds.Tables["IdleTime"].Rows[0]["seconds"].ToString());

                    //if (dr["Duration"].ToString() != "00:00:00")
                    //{
                    //    if (dr["Duration"].ToString().Contains('-'))
                    //    {

                    //    }
                    //    else
                    //    {
                    //        dtResult.Rows.Add(dr);
                    //    }

                    //}

                    //}

                    dr = dtResult.NewRow();

                    dr["EmpName"] = ddlEmp.SelectedItem.Text;
                    dr["Log"] = "Last Activity";
                    dr["DateTime"] = logoutTime;
                    dr["Reason"] = "";
                    dr["Duration"] = "00:00:00";

                    dtResult.Rows.Add(dr);

                    break;
                }
                else
                {
                    //lblError.Text = "No Logout found";
                }
            }
        }
    }

    private DataSet LoadSummaryLogs(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("logs"))
        {
            ds.Tables.Remove(ds.Tables["logs"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end),
            new SqlParameter("@date", start.ToShortDateString())
            
        };
        if (Session["preference"].ToString() == "1")
        {
            sQuery = "SELECT LA_Reason, ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour,ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute,ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds from RTM_Log_Actions where LA_Start_Date_Time BETWEEN @start AND @end and LA_User_Name = @user and LA_Duration != 'HH:MM:SS' and (LA_Reason != 'Peer Support' and LA_Reason != '' and LA_Reason != 'Break' and LA_Reason != 'Unassigned Time' and LA_Reason != 'Idle Time') and (LA_Log_Action != 'First Activity' and LA_Log_Action != 'Last Activity') GROUP BY LA_Reason";
        }
        else
        {
            sQuery = "SELECT LA_Reason, ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour,ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute,ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date and LA_User_Name = @user and LA_Duration != 'HH:MM:SS' and (LA_Reason != 'Peer Support' and LA_Reason != '' and LA_Reason != 'Break' and LA_Reason != 'Unassigned Time' and LA_Reason != 'Idle Time' ) and (LA_Log_Action != 'First Activity' and LA_Log_Action != 'Last Activity') GROUP BY LA_Reason";
        }

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "logs");
        return ds;
    }

    private void PrintSummaryReportNew()
    {
        DataRow dr;
        string TaskDuration = "00:00:00";
        string LogDuration = "00:00:00";
        buildSummeryTable();

        ds = userLogin();

        if (ds.Tables["userLogin"].Rows.Count > 0)
        {
            foreach (DataRow dr2 in ds.Tables["userLogin"].Rows)
            {
                DateTime loginTime = Convert.ToDateTime(dr2["LA_Start_Date_Time"]);
                DateTime logoutTime;// = Convert.ToDateTime(dr2["LA_Start_Date_Time"]);
                ds = userLogout(Convert.ToDateTime(dr2["LA_Start_Date_Time"]));
                if (ds.Tables["userLogout"].Rows.Count > 0)
                {
                    logoutTime = Convert.ToDateTime(ds.Tables["userLogout"].Rows[0]["LA_Start_Date_Time"]);
                    span1 = logoutTime.Subtract(loginTime);
                    string diff = span1.ToString(@"hh\:mm\:ss");
                    dr = dtResult.NewRow();

                    dr["EmpName"] = ddlEmp.SelectedItem.Text;
                    dr["Log"] = "First Activity";
                    dr["DateTime"] = loginTime;
                    dr["Reason"] = "";
                    dr["Duration"] = "00:00:00";

                    dtResult.Rows.Add(dr);

                    ds = LoadTask(ddlEmp.SelectedItem.Text, loginTime, logoutTime);

                    if (ds.Tables["Task"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        dr["EmpName"] = ddlEmp.SelectedItem.Text;
                        dr["Log"] = "Task";
                        // dr["DateTime"] = "";
                        dr["Reason"] = "";
                        dr["Duration"] = TimeSpan.Parse(ds.Tables["Task"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Task"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Task"].Rows[0]["seconds"].ToString());
                        TaskDuration = dr["Duration"].ToString().Replace("-", "");
                        if (dr["Duration"].ToString() != "00:00:00")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }

                    ds = LoadSummaryLogs(ddlEmp.SelectedItem.Text, loginTime, logoutTime);
                    if (ds.Tables["logs"].Rows.Count > 0)
                    {
                        foreach (DataRow dr3 in ds.Tables["logs"].Rows)
                        {
                            dr = dtResult.NewRow();

                            dr["EmpName"] = ddlEmp.SelectedItem.Text;
                            dr["Log"] = "Locked/Paused";
                            // dr["DateTime"] = "";
                            dr["Reason"] = dr3["LA_Reason"].ToString();
                            dr["Duration"] = TimeSpan.Parse(dr3["hour"].ToString() + ":" + dr3["minute"].ToString() + ":" + dr3["seconds"].ToString());
                            var logDur = TimeSpan.Parse(LogDuration).Add(TimeSpan.Parse(dr["Duration"].ToString()));
                            LogDuration = logDur.ToString();
                            if (dr["Duration"].ToString() != "00:00:00")
                            {
                                dtResult.Rows.Add(dr);
                            }
                        }
                    }

                    //dr = dtResult.NewRow();

                    //dr["EmpName"] = ddlEmp.SelectedItem.Text;
                    //dr["Log"] = "";
                    //// dr["DateTime"] = "";
                    //dr["Reason"] = "Idle Time (RTM Idle)";
                    //span2 = TimeSpan.Parse(TaskDuration).Add(TimeSpan.Parse(LogDuration));
                    //span3 = TimeSpan.Parse(diff).Subtract(span2);

                    //dr["Duration"] = span3; //TimeSpan.Parse(ds.Tables["IdleTime"].Rows[0]["hour"].ToString() + ":" + ds.Tables["IdleTime"].Rows[0]["minute"].ToString() + ":" + ds.Tables["IdleTime"].Rows[0]["seconds"].ToString());

                    //if (dr["Duration"].ToString() != "00:00:00")
                    //{
                    //    if (dr["Duration"].ToString().Contains('-'))
                    //    {

                    //    }
                    //    else
                    //    {
                    //        dtResult.Rows.Add(dr);
                    //    }

                    //}

                    //}

                    dr = dtResult.NewRow();

                    dr["EmpName"] = ddlEmp.SelectedItem.Text;
                    dr["Log"] = "Last Activity";
                    dr["DateTime"] = logoutTime;
                    dr["Reason"] = "";
                    dr["Duration"] = "00:00:00";

                    dtResult.Rows.Add(dr);

                    break;
                }
            }
        }
    }

    private void SummaryreportFRO()
    {
        DataRow dr;
        string TaskDuration = "00:00:00";
        string LogDuration = "00:00:00";
        buildSummeryTable();

        dr = dtResult.NewRow();

        ds = LoadTask(ddlEmp.SelectedItem.Text, Convert.ToDateTime(datepicker.Value), Convert.ToDateTime(datepicker.Value));
        if (ds.Tables["Task"].Rows.Count > 0)
        {
            dr = dtResult.NewRow();

            dr["EmpName"] = ddlEmp.SelectedItem.Text;
            dr["Log"] = "Task";
            // dr["DateTime"] = "";
            dr["Reason"] = "";
            dr["Duration"] = TimeSpan.Parse(ds.Tables["Task"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Task"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Task"].Rows[0]["seconds"].ToString());
            TaskDuration = dr["Duration"].ToString().Replace("-", "");
            if (dr["Duration"].ToString() != "00:00:00")
            {
                dtResult.Rows.Add(dr);
            }
        }

        ds = LoadSummaryLogs(ddlEmp.SelectedItem.Text, Convert.ToDateTime(datepicker.Value), Convert.ToDateTime(datepicker.Value));
        if (ds.Tables["logs"].Rows.Count > 0)
        {
            foreach (DataRow dr3 in ds.Tables["logs"].Rows)
            {
                dr = dtResult.NewRow();

                dr["EmpName"] = ddlEmp.SelectedItem.Text;
                dr["Log"] = "Locked/Paused";
                // dr["DateTime"] = "";
                dr["Reason"] = dr3["LA_Reason"].ToString();
                dr["Duration"] = TimeSpan.Parse(dr3["hour"].ToString() + ":" + dr3["minute"].ToString() + ":" + dr3["seconds"].ToString());
                var logDur = TimeSpan.Parse(LogDuration).Add(TimeSpan.Parse(dr["Duration"].ToString()));
                LogDuration = logDur.ToString();
                if (dr["Duration"].ToString() != "00:00:00")
                {
                    dtResult.Rows.Add(dr);
                }
            }
        }
    }
    //Summery Report end1

    //By Task Report start1

    private void buildByTaskTable()
    {
        DataColumn dc;
        try
        {

            dc = new DataColumn("EmpName");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Client");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Task");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Date", typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Duration");
            dtResult.Columns.Add(dc);

        }
        catch (Exception)
        {


        }
    }

    private DataTable LoadByTaskReport()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date", datepicker.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        sQuery = "SELECT A.TL_Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Task_List A, RTM_Records B where A.TL_ID  = B.R_Task and R_User_Name=@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date and R_Duration != 'HH:MM:SS' GROUP BY A.TL_Task";

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        return dt;
    }

    private void PrintByTaskReport()
    {
        DataRow dr;

        buildByTaskTable();

        dt = LoadByTaskReport();

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr1 in dt.Rows)
            {
                dr = dtResult.NewRow();

                dr["EmpName"] = ddlEmp.SelectedItem.Text;
                dr["Client"] = "";
                dr["Task"] = dr1["TL_Task"];
                dr["Date"] = Convert.ToDateTime(datepicker.Value);
                dr["Duration"] = TimeSpan.Parse(dr1["hour"] + ":" + dr1["minute"] + ":" + dr1["seconds"]);

                dtResult.Rows.Add(dr);
            }
        }
    }

    //By Task Report end1

    //By Client start1



    //private DataTable LoadByClientReport()
    //{
    //    da = new SqlDataAdapter("SELECT C.CL_ClientName, A.TL_Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Task_List A, RTM_Records B, RTM_Client_List C where A.TL_ID  = B.R_Task and B.R_Client = C.CL_ID and R_User_Name='" + ddlEmp.SelectedItem.Text + "' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = '" + Convert.ToDateTime(datepicker.Value).ToShortDateString() + "' and R_Duration != 'HH:MM:SS' GROUP BY A.TL_Task, C.CL_ClientName", con);
    //    da.Fill(dt);
    //    return dt;
    //}


    //private void PrintByClientReport()
    //{
    //    DataRow dr;

    //    buildByTaskTable();

    //    dt = LoadByClientReport();

    //    if (dt.Rows.Count > 0)
    //    {
    //        foreach (DataRow dr1 in dt.Rows)
    //        {
    //            dr = dtResult.NewRow();

    //            dr["EmpName"] = ddlEmp.SelectedItem.Text;
    //            dr["Client"] = dr1["CL_ClientName"];
    //            dr["Task"] = dr1["TL_Task"];
    //            dr["Date"] = Convert.ToDateTime(datepicker.Value);
    //            dr["Duration"] = TimeSpan.Parse(dr1["hour"] + ":" + dr1["minute"] + ":" + dr1["seconds"]);

    //            dtResult.Rows.Add(dr);
    //        }
    //    }
    //}


    //Complete log Report

    private void buildCompleteLogTable()
    {
        DataColumn dc;
        try
        {

            dc = new DataColumn("EmpName");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Log");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("StartDate", typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Reason");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Date", typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("StartTime", typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Duration");
            dtResult.Columns.Add(dc);
        }
        catch (Exception)
        {


        }
    }

    private DataTable LoadCompleteLogReportData()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        sQuery = "SELECT * from RTM_Log_Actions where LA_User_Name = @emp and LA_Duration != 'HH:MM:SS'";

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        return dt;
    }

    private void printCompleteLogReport()
    {
        DataRow dr;

        buildCompleteLogTable();

        dt = LoadCompleteLogReportData();

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr1 in dt.Rows)
            {
                if (dr1["LA_Log_Action"].ToString() == "Login")
                {
                    continue;
                }

                if (dr1["LA_Log_Action"].ToString() == "Shutdown")
                {
                    continue;
                }


                dr = dtResult.NewRow();


                dr["EmpName"] = dr1["LA_User_Name"];
                dr["Log"] = dr1["LA_Log_Action"];
                string date1 = Convert.ToDateTime(dr1["LA_Start_Date_Time"]).ToShortDateString();
                dr["StartDate"] = Convert.ToDateTime(dr1["LA_Start_Date_Time"]).ToShortDateString();
                dr["Reason"] = dr1["LA_Reason"];
                dr["Date"] = dr1["LA_CreatedOn"];
                string time1 = Convert.ToDateTime(dr1["LA_Start_Date_Time"]).ToShortTimeString();
                dr["StartTime"] = Convert.ToDateTime(dr1["LA_Start_Date_Time"]).ToLongTimeString();
                dr["Duration"] = dr1["LA_Duration"];

                dtResult.Rows.Add(dr);

            }
        }
    }

    //Summery report for all employess

    private void buildTable()
    {
        DataColumn dc;
        try
        {

            dc = new DataColumn("EmpName");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Team");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Date", typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total_Office_Hours");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total_Task_Hours");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total_Break_Hours");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total_Meeting_Hours");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total_Conference_Call");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total_Others");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total_IdleTime");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Total_IdleTimeUser");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("PeerSupport");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Actual_Login", typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("Actual_Logout", typeof(DateTime));
            dtResult.Columns.Add(dc);

            dc = new DataColumn("ReportHeader");
            dtResult.Columns.Add(dc);
        }
        catch (Exception)
        {


        }
    }

    private DataTable LoadReportData()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@user", Session["username"].ToString()),
            new SqlParameter("@team",Convert.ToInt32(Session["team"]))
        };
        if (Session["access"].ToString() == "1")
        {
            sQuery = "SELECT * FROM RTM_Log_Actions,RTM_Team_List  WHERE LA_TeamId = T_ID and " +
                                "CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) BETWEEN @from AND @to" +
                                "AND LA_Log_Action = 'Last Activity'";

        }
        else if (Session["access"].ToString() == "2")
        {
            sQuery = "SELECT * FROM RTM_Log_Actions,RTM_Team_List  WHERE LA_TeamId = T_ID and " +
                               "CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) BETWEEN @from AND @to" +
                               "AND LA_Log_Action = 'Last Activity' and LA_TeamId IN (Select T_ID from RTM_Team_List where T_Manager = @user)";

        }
        else if (Session["access"].ToString() == "3")
        {
            sQuery = "SELECT * FROM RTM_Log_Actions,RTM_Team_List  WHERE LA_TeamId = T_ID and " +
                               "CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) BETWEEN @from AND @to" +
                               "AND LA_Log_Action = 'Last Activity' and LA_TeamId = @team";

        }

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        //da.Fill(dt);
        return dt;
    }

    private DataTable LoadLoginData()
    {

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@team", ddlEmp.SelectedValue)
           
        };
        sQuery = "SELECT * FROM RTM_Log_Actions,RTM_Team_List  WHERE LA_TeamId = T_ID and " +
                               "CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @from AND @to" +
                               " AND LA_Log_Action = 'First Activity' and LA_TeamId = @team order by LA_User_Name, LA_Start_Date_Time";

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        // da.Fill(dt);
        return dt;
    }

    private DataSet LoadLogoutData(string user, DateTime login)
    {
        if (ds.Tables.Contains("logout"))
        {
            ds.Tables.Remove(ds.Tables["logout"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),           
        };

        sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @user and LA_Start_Date_Time >= @login and LA_Log_Action = 'Last Activity' ORDER BY LA_Start_Date_Time";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "logout");
        return ds;
    }

    private DataSet LoadTask(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("Task"))
        {
            ds.Tables.Remove(ds.Tables["Task"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end),
            new SqlParameter("@start2", start.ToShortDateString())
        };
        if (Session["preference"].ToString() == "1")
        {
            sQuery = "SELECT " +
                "ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                "from RTM_Records " +
                "where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) = @start2 and R_User_Name = @user and R_Duration != 'HH:MM:SS'";
        }
        else
        {
            sQuery = "SELECT " +
                "ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                "from RTM_Records " +
                "where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) = @start2 and R_User_Name = @user and R_Duration != 'HH:MM:SS'";
        }

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Task");
        return ds;
    }

    private DataSet LoadTaskWithoutPeer(string user, DateTime start, DateTime end)
    {

        if (ds.Tables.Contains("Task"))
        {
            ds.Tables.Remove(ds.Tables["Task"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };

        sQuery = "SELECT " +
                "ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                "from RTM_Records, RTM_SubTask_List " +
                "where R_SubTask = STL_ID and STL_SubTask NOT Like 'Peer Support%' and R_Start_Date_Time BETWEEN @start AND @end and R_User_Name = @user and R_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Task");
        // da.Fill(ds, "Task");
        return ds;
    }

    private DataSet LoadPeerTask(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("PeerSupport"))
        {
            ds.Tables.Remove(ds.Tables["PeerSupport"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
                "ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                "from RTM_Records, RTM_SubTask_List " +
                "where R_SubTask = STL_ID and STL_SubTask Like 'Peer Support%' and R_Start_Date_Time BETWEEN @start AND @end and R_User_Name = @user and R_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "PeerSupport");
        return ds;
    }

    private DataSet LoadBreak(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("Break"))
        {
            ds.Tables.Remove(ds.Tables["Break"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
            "ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour," +
            "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute," +
            "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
            "from RTM_Log_Actions " +
            "where LA_Start_Date_Time BETWEEN @start AND @end and LA_Reason ='Break' and LA_User_Name = @user and LA_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Break");
        return ds;
    }

    private DataSet LoadMeeting(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("Meeting"))
        {
            ds.Tables.Remove(ds.Tables["Meeting"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
           "ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
           "from RTM_Log_Actions " +
           "where LA_Start_Date_Time BETWEEN @start AND @end and (LA_Reason ='Meetings' or LA_Reason ='Meeting') and LA_User_Name = @user and LA_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Meeting");
        return ds;
    }



    private DataSet LoadOthers(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("Others"))
        {
            ds.Tables.Remove(ds.Tables["Others"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
           "ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
           "from RTM_Log_Actions " +
           "where LA_Start_Date_Time BETWEEN @start AND @end and (LA_Reason ='Others' or LA_Reason ='Non-Task') and LA_User_Name = @user and LA_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Others");
        return ds;
    }

    private DataSet LoadIdleTimeUser(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("IdleTimeUser"))
        {
            ds.Tables.Remove(ds.Tables["IdleTimeUser"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
           "ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
           "from RTM_Log_Actions " +
           "where LA_Start_Date_Time BETWEEN @start AND @end and LA_Reason ='Idle Time' and LA_User_Name = @user and LA_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "IdleTimeUser");
        return ds;
    }

    private DataSet LoadPeerSupport(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("peerSupport"))
        {
            ds.Tables.Remove(ds.Tables["peerSupport"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
           "ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
           "from RTM_Log_Actions " +
           "where LA_Start_Date_Time BETWEEN @start AND @end and LA_Reason ='Peer Support' and LA_User_Name = @user and LA_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "peerSupport");
        return ds;
    }

    private DataSet LoadPeerComments(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("peerComments"))
        {
            ds.Tables.Remove(ds.Tables["peerComments"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT LA_Comments from RTM_Log_Actions " +
           "where LA_Start_Date_Time BETWEEN @start AND @end and LA_Reason ='Peer Support' and LA_User_Name = @user and LA_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "peerComments");
        return ds;
    }

    private DataSet LoadIdleTime(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("IdleTime"))
        {
            ds.Tables.Remove(ds.Tables["IdleTime"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
                "ISNULL(sum(datediff(second,'00:00:00',convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)/3600)+':'+convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)%3600/60)+':'+convert(varchar(5),(DateDiff(s, IT_StartTime, IT_EndTime)%60))))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)/3600)+':'+convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)%3600/60)+':'+convert(varchar(5),(DateDiff(s, IT_StartTime, IT_EndTime)%60))))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)/3600)+':'+convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)%3600/60)+':'+convert(varchar(5),(DateDiff(s, IT_StartTime, IT_EndTime)%60))))-(((sum(datediff(second,'00:00:00',convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)/3600)+':'+convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)%3600/60)+':'+convert(varchar(5),(DateDiff(s, IT_StartTime, IT_EndTime)%60))))/3600)*3600)-60*((sum(datediff(second,'00:00:00',convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)/3600)+':'+convert(varchar(5),DateDiff(s, IT_StartTime, IT_EndTime)%3600/60)+':'+convert(varchar(5),(DateDiff(s, IT_StartTime, IT_EndTime)%60))))/60)%60)))%60,00) as seconds " +
                "from RTM_IdleTime " +
                "where IT_StartTime BETWEEN @start AND @end and " +
                "IT_UserName = @user and IT_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "IdleTime");
        return ds;
    }

    private DataSet LoadCall(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("Call"))
        {
            ds.Tables.Remove(ds.Tables["Call"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
           "ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
           "from RTM_Log_Actions " +
           "where LA_Start_Date_Time BETWEEN @start AND @end and LA_Reason ='Conference-Call' and LA_User_Name = @user and LA_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Call");
        return ds;
    }

    private DataSet LoadLogTotal(string user, DateTime start, DateTime end)
    {
        if (ds.Tables.Contains("TotalLog"))
        {
            ds.Tables.Remove(ds.Tables["TotalLog"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@start", start),
            new SqlParameter("@end", end)
        };
        sQuery = "SELECT " +
           "ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60,00) as minute," +
           "ISNULL((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
           "from RTM_Log_Actions " +
           "where LA_Start_Date_Time BETWEEN @start AND @end and LA_User_Name = @user and LA_Duration != 'HH:MM:SS'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TotalLog");
        return ds;
    }


    private void printAllSummeryReport()
    {
        try
        {
            DataRow dr;

            buildTable();

            dt = LoadLoginData();

            //dt = LoadReportData();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr1 in dt.Rows)
                {
                    string username = dr1["LA_User_Name"].ToString();
                    DateTime loginTime = Convert.ToDateTime(dr1["LA_Start_Date_Time"]);

                    ds = LoadLogoutData(username, loginTime);
                    if (ds.Tables["logout"].Rows.Count > 0)
                    {
                        DateTime logoutTime = Convert.ToDateTime(ds.Tables["logout"].Rows[0]["LA_Start_Date_Time"]);

                        dr = dtResult.NewRow();

                        dr["EmpName"] = dr1["LA_User_Name"];
                        dr["Team"] = dr1["T_TeamId"];
                        dr["Actual_Login"] = loginTime;
                        dr["Actual_Logout"] = logoutTime;
                        dr["Date"] = loginTime;
                        dr["ReportHeader"] = ddlEmp.SelectedItem.Text + " Summary Report";

                        ds = LoadTaskWithoutPeer(username, loginTime, logoutTime);

                        if (ds.Tables["Task"].Rows.Count > 0 && ds.Tables["Task"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            if (Convert.ToInt32(ds.Tables["Task"].Rows[0]["hour"]) >= 24)
                            {
                                dr["Total_Task_Hours"] = TimeSpan.Parse(23 + ":" + ds.Tables["Task"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Task"].Rows[0]["seconds"].ToString());
                            }
                            else
                            {
                                dr["Total_Task_Hours"] = TimeSpan.Parse(ds.Tables["Task"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Task"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Task"].Rows[0]["seconds"].ToString());
                            }
                        }

                        ds = LoadBreak(username, loginTime, logoutTime);

                        if (ds.Tables["Break"].Rows.Count > 0 && ds.Tables["Break"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            dr["Total_Break_Hours"] = TimeSpan.Parse(ds.Tables["Break"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Break"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Break"].Rows[0]["seconds"].ToString());
                        }

                        ds = LoadMeeting(username, loginTime, logoutTime);

                        if (ds.Tables["Meeting"].Rows.Count > 0 && ds.Tables["Meeting"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            dr["Total_Meeting_Hours"] = TimeSpan.Parse(ds.Tables["Meeting"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Meeting"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Meeting"].Rows[0]["seconds"].ToString());
                        }

                        ds = LoadCall(username, loginTime, logoutTime);

                        if (ds.Tables["Call"].Rows.Count > 0 && ds.Tables["Call"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            dr["Total_Conference_Call"] = TimeSpan.Parse(ds.Tables["Call"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Call"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Call"].Rows[0]["seconds"].ToString());
                        }

                        ds = LoadOthers(username, loginTime, logoutTime);

                        if (ds.Tables["Others"].Rows.Count > 0 && ds.Tables["Others"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            dr["Total_Others"] = TimeSpan.Parse(ds.Tables["Others"].Rows[0]["hour"].ToString() + ":" + ds.Tables["Others"].Rows[0]["minute"].ToString() + ":" + ds.Tables["Others"].Rows[0]["seconds"].ToString());
                        }

                        ds = LoadPeerTask(username, loginTime, logoutTime);

                        if (ds.Tables["peerSupport"].Rows.Count > 0 && ds.Tables["peerSupport"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            dr["PeerSupport"] = TimeSpan.Parse(ds.Tables["peerSupport"].Rows[0]["hour"].ToString() + ":" + ds.Tables["peerSupport"].Rows[0]["minute"].ToString() + ":" + ds.Tables["peerSupport"].Rows[0]["seconds"].ToString());
                        }

                        ds = LoadIdleTimeUser(username, loginTime, logoutTime);

                        if (ds.Tables["IdleTimeUser"].Rows.Count > 0 && ds.Tables["IdleTimeUser"].Rows[0]["hour"].ToString().Length > 0)
                        {
                            dr["Total_IdleTimeUser"] = TimeSpan.Parse(ds.Tables["IdleTimeUser"].Rows[0]["hour"].ToString() + ":" + ds.Tables["IdleTimeUser"].Rows[0]["minute"].ToString() + ":" + ds.Tables["IdleTimeUser"].Rows[0]["seconds"].ToString());
                        }

                        //ds = LoadIdleTime(username, loginTime, logoutTime);

                        //if (ds.Tables["IdleTime"].Rows.Count > 0 && ds.Tables["IdleTime"].Rows[0]["hour"].ToString().Length >0)
                        //{
                        //    dr["Total_IdleTime"] = TimeSpan.Parse(ds.Tables["IdleTime"].Rows[0]["hour"].ToString() + ":" + ds.Tables["IdleTime"].Rows[0]["minute"].ToString() + ":" + ds.Tables["IdleTime"].Rows[0]["seconds"].ToString());
                        //}

                        span4 = TimeSpan.Parse(dr["Total_Task_Hours"].ToString()).Add(TimeSpan.Parse(dr["Total_Break_Hours"].ToString())).Add(TimeSpan.Parse(dr["Total_Meeting_Hours"].ToString())).Add(TimeSpan.Parse(dr["Total_Conference_Call"].ToString())).Add(TimeSpan.Parse(dr["Total_Others"].ToString())).Add(TimeSpan.Parse(dr["PeerSupport"].ToString())).Add(TimeSpan.Parse(dr["Total_IdleTimeUser"].ToString()));
                        //ds = LoadLogTotal(dr1["LA_User_Name"].ToString(), Convert.ToDateTime(dr1["LA_LoginTime"]), Convert.ToDateTime(dr1["LA_Start_Date_Time"]));

                        //if (ds.Tables["TotalLog"].Rows.Count > 0)
                        //{
                        //    string total = ds.Tables["TotalLog"].Rows[0]["hour"].ToString() + ":" + ds.Tables["TotalLog"].Rows[0]["minute"].ToString() + ":" + ds.Tables["TotalLog"].Rows[0]["seconds"].ToString();
                        //    TimeSpan span1 = TimeSpan.Parse(total).Add(TimeSpan.Parse(dr["Total_Task_Hours"].ToString()));
                        //    dr["Total_Office_Hours"] = span1;
                        //}
                        var datetimediff = (logoutTime - loginTime).ToString("hhmmss");
                        dr["Total_Office_Hours"] = datetimediff.Substring(0, 2) + ":" + datetimediff.Substring(2, 2) + ":" + datetimediff.Substring(4, 2);
                        TimeSpan totalIdleTime4 = TimeSpan.Parse(dr["Total_Office_Hours"].ToString()).Subtract(span4);
                        if (totalIdleTime4 <= TimeSpan.Parse("00:00:00"))
                        {
                            dr["Total_IdleTime"] = "00:00:00";
                        }
                        else
                        {
                            dr["Total_IdleTime"] = totalIdleTime4;
                        }

                        dtResult.Rows.Add(dr);
                    }


                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = System.Drawing.Color.Red;
        }
    }


    //Time Sheet Report Start

    private void BuildTimeSheetTable()
    {

        try
        {
            DataColumn dc;
            dtResult = new DataTable();

            dc = new DataColumn("username");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("date");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("jobcode");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("hours");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("notes");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("service code");
            dtResult.Columns.Add(dc);

            dc = new DataColumn("service code value");
            dtResult.Columns.Add(dc);
        }
        catch (Exception)
        {

        }
    }

    private DataSet getLogoutTime()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user",  Session["username"].ToString()),
            new SqlParameter("@team", Convert.ToInt32(Session["team"])),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        if (ddlEmp.Visible == true && datepickerTo.Visible == false)
        {
            if (ddlEmp.SelectedIndex == 0)
            {
                if (Session["access"].ToString() == "1")
                {
                    sQuery = "select * from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) = @from and LA_Log_Action ='Last Activity'";
                }
                else if (Session["access"].ToString() == "2")
                {
                    sQuery = "select * from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) = @from and LA_Log_Action ='Last Activity' and LA_TeamId IN (Select T_ID from RTM_Team_List where T_Manager = @user)";
                }
                else if (Session["access"].ToString() == "3")
                {
                    sQuery = "select * from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) = @from and LA_Log_Action ='Last Activity' and and LA_TeamId=@team";
                }

            }
            else
            {
                sQuery = "select * from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) = @from and LA_Log_Action ='Last Activity'";
            }

        }
        else if (ddlEmp.Visible == true)
        {
            if (ddlEmp.SelectedIndex == 0)
            {
                if (Session["access"].ToString() == "1")
                {
                    sQuery = "select * from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) BETWEEN @from and @to and LA_Log_Action ='Last Activity'";
                }
                else if (Session["access"].ToString() == "2")
                {
                    sQuery = "select * from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) BETWEEN @from and @to and LA_Log_Action ='Last Activity' and LA_TeamId IN (Select T_ID from RTM_Team_List where T_Manager = @user)";
                }
                else if (Session["access"].ToString() == "3")
                {
                    sQuery = "select * from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) BETWEEN @from and @to and LA_Log_Action ='Last Activity' and LA_TeamId=@team";
                }

            }
            else
            {
                sQuery = "select * from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) BETWEEN @from and @to and LA_Log_Action ='Last Activity'";
            }

        }
        else
        {
            sQuery = "select * from RTM_Log_Actions where LA_User_Name = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_LoginTime))) BETWEEN @from and @to and LA_Log_Action ='Last Activity'";
        }

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Log");
        return ds;
    }

    private DataSet getLogoutTime2()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user",  Session["username"].ToString()),
            new SqlParameter("@team", Convert.ToInt32(ddlEmp.SelectedValue)),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
        };
        if (ddlEmp.Visible == true && datepickerTo.Visible == false)
        {

            sQuery = "select * from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @from and LA_Log_Action ='Last Activity'";

        }
        else if (ddlEmp.Visible == true)
        {

            if (lblEmp.Text == "Select Team")
            {
                sQuery = "select * from RTM_Log_Actions where LA_TeamId = @team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @from and @to and LA_Log_Action ='Last Activity'";
            }
            else
            {
                sQuery = "select * from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @from and @to and LA_Log_Action ='Last Activity'";
            }


        }
        else
        {
            sQuery = "select * from RTM_Log_Actions where LA_User_Name = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @from and @to and LA_Log_Action ='Last Activity'";
        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Log");
        return ds;
    }


    private DataSet GetLoginTime(string user, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user",  user),
            new SqlParameter("@logout", logout)
        };
        sQuery = "select Top 1 LA_Start_Date_Time from RTM_Log_Actions where LA_User_Name = @user and LA_Start_Date_Time <= @logout and LA_Log_Action='First Activity' Order By LA_ID DESC";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Login");
        return ds;
    }

    private DataSet GetLoginTime3()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user",  Session["username"].ToString()),
            new SqlParameter("@emp",  ddlEmp.SelectedItem.Text),
            new SqlParameter("@date", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@team", Convert.ToInt32(ddlEmp.SelectedValue))
        };
        if (ddlEmp.Visible == true && datepickerTo.Visible == false)
        {
            if (Session["preference"].ToString() == "1")
            {
                sQuery = "select * from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date and LA_Log_Action ='First Activity' order by LA_Start_Date_Time";
            }
            else
            {
                sQuery = "select R_User_Name as LA_User_Name,CONVERT(VARCHAR(10), R_Start_Date_Time, 101) as LA_Start_Date_Time from RTM_Records where R_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date group by R_User_Name, CONVERT(VARCHAR(10), R_Start_Date_Time, 101) order by LA_Start_Date_Time";
            }


        }
        else if (ddlEmp.Visible == true)
        {
            if (lblEmp.Text == "Select Team")
            {
                if (Session["preference"].ToString() == "1")
                {
                    sQuery = "select * from RTM_Log_Actions where LA_TeamId = @team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @date and @to and LA_Log_Action ='First Activity' order by LA_Start_Date_Time";
                }
                else
                {
                    sQuery = "select R_User_Name as LA_User_Name, CONVERT(VARCHAR(10), R_Start_Date_Time, 101) as LA_Start_Date_Time from RTM_Records where R_TeamId = @team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @date and @to Group by R_User_Name, CONVERT(VARCHAR(10), R_Start_Date_Time, 101) order by LA_Start_Date_Time";
                }

            }
            else
            {
                if (Session["preference"].ToString() == "1")
                {
                    sQuery = "select * from RTM_Log_Actions where LA_User_Name = @emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @date and @to and LA_Log_Action ='First Activity' order by LA_Start_Date_Time";
                }
                else
                {
                    sQuery = "select R_User_Name as LA_User_Name, CONVERT(VARCHAR(10), R_Start_Date_Time, 101) as LA_Start_Date_Time from RTM_Records where R_User_Name =@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @date and @to group by R_User_Name, CONVERT(VARCHAR(10), R_Start_Date_Time, 101) order by LA_Start_Date_Time";
                }

            }
        }
        else
        {
            if (Session["preference"].ToString() == "1")
            {
                sQuery = "select * from RTM_Log_Actions where LA_User_Name = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @date and @to and LA_Log_Action ='First Activity' order by LA_Start_Date_Time";
            }
            else
            {
                sQuery = "select R_User_Name as LA_User_Name, CONVERT(VARCHAR(10), R_Start_Date_Time, 101) as LA_Start_Date_Time from RTM_Records where R_User_Name = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @date and @to and group by R_User_Name, CONVERT(VARCHAR(10), R_Start_Date_Time, 101) order by LA_Start_Date_Time";
            }

        }

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Log");
        //da.Fill(ds, "Log");
        return ds;
    }

    private DataSet getLogoutTime3(string user, DateTime login)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user",  user),
            new SqlParameter("@login", login)
        };
        if (Session["preference"].ToString() == "1")
        {
            sQuery = "select Top 1 LA_Start_Date_Time from RTM_Log_Actions where LA_User_Name = @user and LA_Start_Date_Time >= @login and LA_Log_Action='Last Activity' ORDER BY LA_Start_Date_Time";
        }
        else
        {
            sQuery = "select Top 1 R_Start_Date_Time as LA_Start_Date_Time from RTM_Records where R_User_Name = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @login ORDER BY R_Start_Date_Time Desc";
        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "Login");
        //da.Fill(ds, "Login");
        return ds;
    }


    private DataSet getTSheetTasks(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout),
            new SqlParameter("@from", login.ToShortDateString()),
            new SqlParameter("@to", logout.ToShortDateString())
        };
        if (ddlEmp.Visible == true)
        {
            if (Session["preference"].ToString() == "1")
            {
                sQuery = "select R_User_Name, CL_TSheetClient, STL_ServiceCode," +
                   "ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour," +
                   "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute," +
                   "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                   "from RTM_Records, RTM_Client_List, RTM_SubTask_List " +
                   "where R_Client = CL_ID  and R_SubTask = STL_ID and R_User_Name = @user And R_Start_Date_Time BETWEEN @login AND @logout  and R_Duration != 'HH:MM:SS' and STL_SubTask != 'Idle Time' " +
                   "GROUP BY R_User_Name, CL_TSheetClient, STL_ServiceCode";
            }
            else
            {
                sQuery = "select R_User_Name, CL_TSheetClient, STL_ServiceCode," +
                   "ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour," +
                   "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute," +
                   "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                   "from RTM_Records, RTM_Client_List, RTM_SubTask_List " +
                   "where R_Client = CL_ID  and R_SubTask = STL_ID and R_User_Name = @user And CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @from and R_Duration != 'HH:MM:SS' and STL_SubTask != 'Idle Time' " +
                   "GROUP BY R_User_Name, CL_TSheetClient, STL_ServiceCode";
            }


        }
        else
        {
            if (Session["preference"].ToString() == "1")
            {
                sQuery = "select R_User_Name, CL_TSheetClient, STL_ServiceCode," +
                        "ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour," +
                        "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute," +
                        "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                        "from RTM_Records, RTM_Client_List, RTM_SubTask_List " +
                        "where R_Client = CL_ID  and R_SubTask = STL_ID and R_User_Name = @user And R_Start_Date_Time BETWEEN @login AND @logout  and R_Duration != 'HH:MM:SS' " +
                        "GROUP BY R_User_Name, CL_TSheetClient, STL_ServiceCode";
            }
            else
            {
                sQuery = "select R_User_Name, CL_TSheetClient, STL_ServiceCode," +
                       "ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour," +
                       "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute," +
                       "ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds " +
                       "from RTM_Records, RTM_Client_List, RTM_SubTask_List " +
                       "where R_Client = CL_ID  and R_SubTask = STL_ID and R_User_Name = @user And CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @from and R_Duration != 'HH:MM:SS' " +
                       "GROUP BY R_User_Name, CL_TSheetClient, STL_ServiceCode";
            }


        }
        //da.Fill(ds, "TSheetTask");
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSheetTask");
        return ds;
    }

    private DataSet GetTSheetTaksComments(string user, DateTime login, DateTime logout, string TSclient, string TSCode)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout),
            new SqlParameter("@TSclient", TSclient),
            new SqlParameter("@TSCode", TSCode),
            new SqlParameter("@from",  login.ToShortDateString())
        };
        if (Session["preference"].ToString() == "1")
        {
            sQuery = "select R_Comments " +
                        "from RTM_Records, RTM_Client_List, RTM_SubTask_List " +
                        "where R_Client = CL_ID  and R_SubTask = STL_ID and R_User_Name = @user And R_Start_Date_Time BETWEEN @login AND @logout  and R_Duration != 'HH:MM:SS' and CL_TSheetClient = @TSclient and STL_ServiceCode=@TSCode" +
                        "";
        }
        else
        {
            sQuery = "select R_Comments " +
                        "from RTM_Records, RTM_Client_List, RTM_SubTask_List " +
                        "where R_Client = CL_ID  and R_SubTask = STL_ID and R_User_Name = @user And CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @from and R_Duration != 'HH:MM:SS' and CL_TSheetClient = @TSclient and STL_ServiceCode=@TSCode" +
                        "";
        }

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TaskComments");
        return ds;
    }

    private DataSet GetTSMeetings(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout)
        };
        if (ddlEmp.Visible == true)
        {
            sQuery = "select LA_Comments, CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) AS date1," +
                "ISNULL(sum(datediff(second,'00:00:00',LA_Duration))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))-(((sum(datediff(second,'00:00:00',LA_Duration))/3600)*3600)-60*((sum(datediff(second,'00:00:00',LA_Duration))/60)%60)))%60,00) as seconds " +
                 "from RTM_Log_Actions where LA_User_Name= @user and " +
                 "LA_Start_Date_Time BETWEEN @login AND @logout and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Meetings' or LA_Reason='Meeting') " +
                 "GROUP BY LA_Comments, CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) ";

        }
        else
        {
            sQuery = "select LA_Comments, CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) AS date1, " +
                "ISNULL(sum(datediff(second,'00:00:00',LA_Duration))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))-(((sum(datediff(second,'00:00:00',LA_Duration))/3600)*3600)-60*((sum(datediff(second,'00:00:00',LA_Duration))/60)%60)))%60,00) as seconds " +
                 "from RTM_Log_Actions where LA_User_Name= @user and " +
                 "LA_Start_Date_Time BETWEEN @login AND @logout  and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Meetings' or LA_Reason='Meeting') " +
                 "GROUP BY LA_Comments, CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time)))";

        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSMeetings");
        //da.Fill(ds, "TSMeetings");
        return ds;
    }

    private DataSet GetTSheetMeetings1(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout),
            new SqlParameter("@date", login.ToShortDateString()),
            
        };
        ds = objReal.GetMeetingsForTSheet(user, login, logout, "TSMeetings");

        return ds;
    }

    private DataSet GetTSheetMeetComments1(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout)
        };
        sQuery = "select LA_Comments " +
                 "from RTM_Log_Actions where LA_User_Name= @user and " +
                 "LA_Start_Date_Time BETWEEN @login AND @logout  and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Meetings' or LA_Reason='Meeting') " +
                 "GROUP BY LA_Comments";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSLearningComments");
        return ds;
    }

    private DataSet GetTSheetMeetings2(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout)
        };
        sQuery = "select " +
               "ISNULL(sum(datediff(second,'00:00:00',LA_Duration))/3600,00) as hour," +
               "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))/60)%60,00) as minute," +
               "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))-(((sum(datediff(second,'00:00:00',LA_Duration))/3600)*3600)-60*((sum(datediff(second,'00:00:00',LA_Duration))/60)%60)))%60,00) as seconds " +
                "from RTM_Log_Actions where LA_User_Name= @user and " +
                "LA_Start_Date_Time BETWEEN @login AND @logout  and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Meetings' or LA_Reason='Meeting') and LA_Comments LIKE '%Trainings%' " +
                " ";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSMeetings2");
        return ds;
    }

    private DataSet GetTSheetMeetComments2(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout)
        };
        sQuery = "select LA_Comments " +
                 "from RTM_Log_Actions where LA_User_Name= @user and " +
                 "LA_Start_Date_Time BETWEEN @login AND @logout  and REPLACE(LA_Duration,'-', '') != 'HH:MM:SS' and (LA_Reason = 'Meetings' or LA_Reason='Meeting') and LA_Comments LIKE '%Trainings%' " +
                 "GROUP BY LA_Comments";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSLearningComments2");
        return ds;
    }

    private DataSet GetTSConfCalls(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout)
        };
        if (ddlEmp.Visible == true)
        {
            sQuery = "select  " +
                "ISNULL(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))-(((sum(datediff(second,'00:00:00',LA_Duration))/3600)*3600)-60*((sum(datediff(second,'00:00:00',LA_Duration))/60)%60)))%60,00) as seconds " +
                 "from RTM_Log_Actions where LA_User_Name= @user and " +
                 "LA_Start_Date_Time BETWEEN @login AND @logout  and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Conference-Call' or LA_Reason='Conf-Call') " +
                 " ";

        }
        else
        {
            sQuery = "select " +
                "ISNULL(sum(datediff(second,'00:00:00',LA_Duration))/3600,00) as hour," +
                "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))/60)%60,00) as minute," +
                "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))-(((sum(datediff(second,'00:00:00',LA_Duration))/3600)*3600)-60*((sum(datediff(second,'00:00:00',LA_Duration))/60)%60)))%60,00) as seconds " +
                 "from RTM_Log_Actions where LA_User_Name= @user and " +
                 "LA_Start_Date_Time BETWEEN @login AND @logout  and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Conference-Call' or LA_Reason='Conf-Call')  " +
                 " ";

        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSConf");
        // da.Fill(ds, "TSConf");
        return ds;
    }

    private DataSet GetConfCallComments(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout)
        };
        sQuery = "select LA_Comments " +
                 "from RTM_Log_Actions where LA_User_Name= @user and " +
                 "LA_Start_Date_Time BETWEEN @login AND @logout  and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Conference-Call' GROUP BY LA_Comments " +
                 " ";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSConfComments");
        return ds;
    }

    private DataSet GetTSOthers(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout)
        };
        if (ddlEmp.Visible == true)
        {
            sQuery = "select CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) AS date1, " +
               "ISNULL(sum(datediff(second,'00:00:00',LA_Duration))/3600,00) as hour," +
               "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))/60)%60,00) as minute," +
               "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))-(((sum(datediff(second,'00:00:00',LA_Duration))/3600)*3600)-60*((sum(datediff(second,'00:00:00',LA_Duration))/60)%60)))%60,00) as seconds " +
                "from RTM_Log_Actions where LA_User_Name= @user and " +
                "LA_Start_Date_Time BETWEEN @login AND @logout  and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Others' or LA_Reason = 'Non-Task') GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) " +
                "";

        }
        else
        {
            sQuery = "select CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) AS date1, " +
               "ISNULL(sum(datediff(second,'00:00:00',LA_Duration))/3600,00) as hour," +
               "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))/60)%60,00) as minute," +
               "ISNULL((sum(datediff(second,'00:00:00',LA_Duration))-(((sum(datediff(second,'00:00:00',LA_Duration))/3600)*3600)-60*((sum(datediff(second,'00:00:00',LA_Duration))/60)%60)))%60,00) as seconds " +
                "from RTM_Log_Actions where LA_User_Name= @user and " +
                "LA_Start_Date_Time BETWEEN @login AND @logout and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Others' or LA_Reason = 'Non-Task') GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) " +
                "";

        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSOthers");
        //da.Fill(ds, "TSOthers");
        return ds;
    }

    private DataSet GetOthersComments(string user, DateTime login, DateTime logout)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@login", login),
            new SqlParameter("@logout", logout)
        };
        sQuery = "select LA_Comments from RTM_Log_Actions where LA_User_Name= @user and LA_Start_Date_Time BETWEEN @login AND @logout and (LA_Reason = 'Others' or LA_Reason = 'Non-Task')";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "TSOthersComments");
        return ds;
    }

    private DataSet GetMeetingsServiceCode(string task)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@task", task)
            
        };
        sQuery = "select * from RTM_MeetingsCategory where M_SubCategory = @task";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "servicecode");
        return ds;
    }

    private DataSet CheckIncorectRecords()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(ddlEmp.SelectedValue)),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value)
        };
        if (ds.Tables.Contains("incorrect"))
        {
            ds.Tables.Remove(ds.Tables["incorrect"]);
        }
        sQuery = "select R_User_Name, R_Start_Date_Time,R_Comments, CL_ClientName, TL_Task, STL_SubTask,CL_TSheetClient, STL_ServiceCode,ISNULL(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600,00) as hour,ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60,00) as minute,ISNULL((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60,00) as seconds from RTM_Records, RTM_Client_List, RTM_Task_List, RTM_SubTask_List where R_Client = CL_ID and R_Task=TL_ID  and R_SubTask = STL_ID And CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from AND @to and R_TeamId=@team and CL_TSheetClient='Internal' and STL_ServiceCode NOT LIKE '% General/Internal' and STL_SubTask != 'Idle Time'  and R_Duration != 'HH:MM:SS' GROUP BY R_User_Name, CL_TSheetClient, STL_ServiceCode, CL_ClientName, TL_Task, STL_SubTask, R_Start_Date_Time,R_Comments";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "incorrect");
        return ds;
    }

    private DataSet CheckLogins(int totalWorkingDays)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(ddlEmp.SelectedValue)),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
            new SqlParameter("@totalWorkingDays", totalWorkingDays)
        };
        if (ds.Tables.Contains("totalLogins"))
        {
            ds.Tables.Remove(ds.Tables["totalLogins"]);
        }
        if (rbUser.Checked)
        {
            sQuery = "select LA_User_Name, COUNT(CONVERT(VARCHAR(10), LA_Start_Date_Time, 101)) + Count(CONVERT(VARCHAR(10), LD_Date, 101)) as [LoginCount] from RTM_Log_Actions left join dbo.RTM_LeaveDetails on LA_User_Name = LD_UserName where LA_User_Name =@emp and (CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to or CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) between @from and @to) and la_log_action ='First Activity' and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday') group by LA_User_Name having (COUNT(CONVERT(VARCHAR(10), LA_Start_Date_Time, 101)) + Count(CONVERT(VARCHAR(10), LD_Date, 101))) < @totalWorkingDays order by LA_User_Name";
        }
        else
        {
            sQuery = "select LA_User_Name, COUNT(CONVERT(VARCHAR(10), LA_Start_Date_Time, 101)) + Count(CONVERT(VARCHAR(10), LD_Date, 101)) as [LoginCount] from RTM_Log_Actions left join dbo.RTM_LeaveDetails on LA_User_Name = LD_UserName where (CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to or CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) between @from and @to) and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday') and LA_TeamId =@team and la_log_action ='First Activity' group by LA_User_Name having (COUNT(CONVERT(VARCHAR(10), LA_Start_Date_Time, 101)) + Count(CONVERT(VARCHAR(10), LD_Date, 101))) <@totalWorkingDays order by LA_User_Name";
        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "totalLogins");
        //da.Fill(ds, "totalLogins");
        return ds;
    }

    private DataSet CheckLoginsNew(int totalWorkingDays)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(ddlEmp.SelectedValue)),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
            new SqlParameter("@totalWorkingDays", totalWorkingDays)
        };
        if (ds.Tables.Contains("totalLogins"))
        {
            ds.Tables.Remove(ds.Tables["totalLogins"]);
        }
        if (rbUser.Checked)
        {
            if (Session["preference"].ToString() == "1")
            {
                sQuery = "select LA_User_Name, CONVERT(VARCHAR(10), LA_Start_Date_Time, 101) as [date] from RTM_Log_Actions where LA_User_Name =@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and la_log_action ='First Activity' and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday') group by LA_User_Name, CONVERT(VARCHAR(10), LA_Start_Date_Time, 101) order by LA_User_Name";
            }
            else
            {
                sQuery = "select LA_User_Name, CONVERT(VARCHAR(10), LA_Start_Date_Time, 101) as [date] from RTM_Log_Actions where LA_User_Name =@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday') group by LA_User_Name, CONVERT(VARCHAR(10), LA_Start_Date_Time, 101) order by LA_User_Name";
            }

        }
        else
        {
            if (Session["preference"].ToString() == "1")
            {
                sQuery = "select LA_User_Name, CONVERT(VARCHAR(10), LA_Start_Date_Time, 101) as [date] from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday') and LA_TeamId =@team and la_log_action ='First Activity' group by LA_User_Name, CONVERT(VARCHAR(10), LA_Start_Date_Time, 101)  order by LA_User_Name";
            }
            else
            {
                sQuery = "select LA_User_Name, CONVERT(VARCHAR(10), LA_Start_Date_Time, 101) as [date] from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday') and LA_TeamId =@team group by LA_User_Name, CONVERT(VARCHAR(10), LA_Start_Date_Time, 101)  order by LA_User_Name";
            }

        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "totalLogins");
        //da.Fill(ds, "totalLogins");
        return ds;
    }

    private DataSet CheckLeaveCount(string user)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value)
        };
        if (ds.Tables.Contains("leaveCount"))
        {
            ds.Tables.Remove(ds.Tables["leaveCount"]);
        }
        sQuery = "select LD_UserName, Count(CONVERT(VARCHAR(10), LD_Date, 101)) as [LeaveCount] from dbo.RTM_LeaveDetails where LD_UserName = @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) between @from and @to Group by LD_UserName";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "leaveCount");
        //da.Fill(ds, "leaveCount");
        return ds;
    }
    private DataSet FetchLoginDates(string user)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user", user),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value)
        };
        if (ds.Tables.Contains("logindates"))
        {
            ds.Tables.Remove(ds.Tables["logindates"]);
        }
        if (Session["preference"].ToString() == "1")
        {
            sQuery = "select CONVERT(VARCHAR(10), LA_Start_Date_Time, 101)  as logins from RTM_Log_Actions where LA_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and la_log_action ='First Activity' and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday') " +
                               "UNION " +
                               "Select CONVERT(VARCHAR(10), LD_Date, 101) as logins from RTM_LeaveDetails where LD_UserName=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) between @from and @to order by logins";
        }
        else
        {
            sQuery = "select CONVERT(VARCHAR(10), LA_Start_Date_Time, 101)  as logins from RTM_Log_Actions where LA_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday') " +
                               "UNION " +
                               "Select CONVERT(VARCHAR(10), LD_Date, 101) as logins from RTM_LeaveDetails where LD_UserName=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) between @from and @to order by logins";
        }


        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "logindates");
        return ds;
    }

    private DataSet FetchMissingUser()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
            new SqlParameter("@team", ddlEmp.SelectedValue),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value)
        };
        if (ds.Tables.Contains("missingUsers"))
        {
            ds.Tables.Remove(ds.Tables["missingUsers"]);
        }
        if (rbUser.Checked == true)
        {
            sQuery = "select UL_User_Name from RTM_User_List where NOT EXISTS(select LA_User_Name from RTM_Log_Actions where UL_User_Name = LA_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @from and @to and la_log_action ='First Activity') and UL_User_Name = @emp and UL_User_Status =1";
        }
        else
        {
            sQuery = "select UL_User_Name from RTM_User_List where NOT EXISTS(select LA_User_Name from RTM_Log_Actions where UL_User_Name = LA_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @from and @to and la_log_action ='First Activity') and UL_Team_Id = @team and UL_User_Status =1";
        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "missingUsers");
        //da.Fill(ds, "missingUsers");
        return ds;
    }

    private void BuildMissingLoginsTable()
    {
        dt = new DataTable();
        DataColumn dc;

        dc = new DataColumn("User");
        dt.Columns.Add(dc);

        dc = new DataColumn("MissedDate");
        dt.Columns.Add(dc);
    }

    private DataSet GetLeaveDetails(string user)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
            new SqlParameter("@team", ddlEmp.SelectedValue),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value)
        };
        if (ds.Tables.Contains("leaveDetails"))
        {
            ds.Tables.Remove(ds.Tables["leaveDetails"]);
        }
        if (rbUser.Checked == true)
        {
            sQuery = "select * from dbo.RTM_LeaveDetails,dbo.RTM_Leave  where L_ID = LD_LID and LD_UserName=@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) between @from and @to";
        }
        else
        {
            sQuery = "select * from dbo.RTM_LeaveDetails,dbo.RTM_Leave,RTM_User_List  where L_ID = LD_LID and LD_UserName= UL_User_Name and UL_Team_Id= @team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) between @from and @to";
        }
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "leaveDetails");
        //da.Fill(ds, "leaveDetails");
        return ds;
    }

    private void CheckAbsents()
    {
        DataRow dr;

        BuildMissingLoginsTable();

        //Check count of logins
        int WorkingDays = 0;
        int noOfDays = Convert.ToInt32((Convert.ToDateTime(datepickerTo.Value) - Convert.ToDateTime(datepicker.Value)).TotalDays);
        checked
        {
            for (int i = 0; i <= noOfDays; i++)
            {
                DayOfWeek day = Convert.ToDateTime(datepicker.Value).AddDays(i).DayOfWeek;
                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                {
                    continue;
                }
                else
                {
                    lstDate.Add(Convert.ToDateTime(datepicker.Value).AddDays(i).ToShortDateString());
                    //WorkingDays = WorkingDays + 1;
                }
            }
        }


        WorkingDays = objUtil.GetNumberOfWorkingDays(Convert.ToDateTime(datepicker.Value), Convert.ToDateTime(datepickerTo.Value));

        ds = CheckLoginsNew(WorkingDays);
        if (ds.Tables["totalLogins"].Rows.Count > 0)
        {
            var query = ds.Tables["totalLogins"].AsEnumerable()
            .GroupBy(r => r.Field<string>("LA_User_Name"))
            .Select(grp => new
            {
                LA_User_Name = grp.Key,
                date = grp.Count()
            })
            .OrderBy(o => o.LA_User_Name)
            .ToList();

            if (query.Count > 0)
            {
                foreach (var item in query)
                {
                    string user = item.LA_User_Name;
                    int loginCountFromLogAction = item.date;
                    if (loginCountFromLogAction < WorkingDays)
                    {
                        ds = CheckLeaveCount(user);
                        if (ds.Tables["leaveCount"].Rows.Count > 0)
                        {
                            int leaveCount = Convert.ToInt32(ds.Tables["leaveCount"].Rows[0]["LeaveCount"]);
                            int total = loginCountFromLogAction + leaveCount;
                            if (total < WorkingDays)
                            {
                                lstUser.Add(user);
                            }
                        }
                        else
                        {
                            lstUser.Add(user);
                        }
                    }
                }

                //if (rbUser.Checked == true)
                //{
                //    dtResult = objReal.GetUsersForCheckingLeaves(ddlEmp.SelectedItem.Text, "user", datepicker.Value, datepickerTo.Value);

                //    if (dtResult.Rows.Count > 0)
                //    {
                //        foreach (DataRow drRow in dtResult.Rows)
                //        {
                //            foreach (var dtDate in lstDate)
                //            {
                //                dr = dt.NewRow();
                //                dr["User"] = drRow["UL_User_Name"].ToString();
                //                dr["MissedDate"] = dtDate;

                //                dt.Rows.Add(dr);
                //            }
                //        }

                //    }
                //}
                //else if (rbTeam.Checked == true)
                //{
                //    dtResult = objReal.GetUsersForCheckingLeaves(ddlEmp.SelectedValue, "team", datepicker.Value, datepickerTo.Value);
                //    if (dtResult.Rows.Count > 0)
                //    {
                //        foreach (DataRow drRow in dtResult.Rows)
                //        {
                //            foreach (var dtDate in lstDate)
                //            {
                //                dr = dt.NewRow();
                //                dr["User"] = drRow["UL_User_Name"].ToString();
                //                dr["MissedDate"] = dtDate;

                //                dt.Rows.Add(dr);
                //            }
                //        }

                //    }
                //}

                if (lstUser.Count > 0)
                {
                    checked
                    {
                        for (int j = 0; j <= lstUser.Count - 1; j++)
                        {
                            ds = FetchLoginDates(lstUser[j]);

                            List<string> lstActual = ds.Tables["logindates"].AsEnumerable().Select(r => r.Field<string>("logins")).ToList();
                            checked
                            {
                                for (int i = 0; i <= lstDate.Count - 1; i++)
                                {
                                    if (lstActual.Contains(Convert.ToDateTime(lstDate[i]).ToString("MM/dd/yyyy")))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        dr = dt.NewRow();
                                        dr["User"] = lstUser[j];
                                        dr["MissedDate"] = lstDate[i];

                                        dt.Rows.Add(dr);
                                    }
                                }
                            }

                        }
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    absentflag = 1;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Incorrect Records", "alert('Possible leave records found. Please update in the table below and then generate the T-Sheet')", true);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                    GridView1.Visible = true;
                    return;
                }
                else
                {
                    absentflag = 0;
                }
            }

        }
    }

    private void CheckAbsentsNew()
    {
        DataRow dr;

        BuildMissingLoginsTable();

        //Check count of logins
        
        int noOfDays = Convert.ToInt32((Convert.ToDateTime(datepickerTo.Value) - Convert.ToDateTime(datepicker.Value)).TotalDays);
        checked
        {
            for (int i = 0; i <= noOfDays; i++)
            {
                DayOfWeek day = Convert.ToDateTime(datepicker.Value).AddDays(i).DayOfWeek;
                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                {
                    continue;
                }
                else
                {
                    lstDate.Add(Convert.ToDateTime(datepicker.Value).AddDays(i).ToShortDateString());
                    //WorkingDays = WorkingDays + 1;
                }
            }
        }

        foreach (var dtDate in lstDate)
        {
            if (rbUser.Checked == true)
            {
                ds = objReal.CheckAbsentUsers(ddlEmp.SelectedItem.Text, dtDate, "user", Session["preference"].ToString());
                if (ds.Tables["absent"].Rows.Count > 0)
                {
                    continue;
                }
                else
                {
                    dtResult = objReal.CheckUpdatedAbsents(ddlEmp.SelectedItem.Text, dtDate);
                    if (dtResult.Rows.Count > 0)
                    {
                        continue;
                    }
                    else
                    {
                        dr = dt.NewRow();
                        dr["User"] = ddlEmp.SelectedItem.Text;
                        dr["MissedDate"] = Convert.ToDateTime(dtDate).ToString("dddd, MMMM d, yyyy");

                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                ds = objReal.CheckAbsentUsers(ddlEmp.SelectedValue, dtDate, "team", Session["preference"].ToString());
                if (ds.Tables["absent"].Rows.Count > 0)
                {
                    foreach (DataRow drRow in ds.Tables["absent"].Rows)
                    {
                        dtResult = objReal.CheckUpdatedAbsents(drRow["UL_User_Name"].ToString(), dtDate);
                        if (dtResult.Rows.Count > 0)
                        {
                            continue;
                        }
                        else
                        {
                            dr = dt.NewRow();
                            dr["User"] = drRow["UL_User_Name"];
                            dr["MissedDate"] = Convert.ToDateTime(dtDate).ToString("dddd, MMMM d, yyyy");

                            dt.Rows.Add(dr);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        if (dt.Rows.Count > 0)
        {
            absentflag = 1;
            
            GridView1.DataSource = dt;
            GridView1.DataBind();
            GridView1.Visible = true;
            //return;
        }
        else
        {
            GridView1.DataSource = null;
            GridView1.DataBind();
            dvAbsent.Visible = false;
            absentflag = 0;
        }

    }

    private void PrintTSheetReportFRO()
    {
        gvTSheet.Visible = false;
        GridView1.Visible = false;

        CheckAbsentsNew();
        if (absentflag == 1)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Incorrect Records", "alert('Possible leave records found. Please update in the table below and then generate the T-Sheet')", true);
            LoadReason();
            dvAbsent.Visible = true;
            return;
        }
        else
        {
            dvAbsent.Visible = false;
        }

        BuildTimeSheetTable();


    }

    private void PrintTSheetReport()
    {
        gvTSheet.Visible = false;
        GridView1.Visible = false;
        // var sunday = DateTime.Now.Previous(DayOfWeek.Sunday);
        //var thisWeekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
        DataRow dr;


        CheckAbsentsNew();
        if (absentflag == 1)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Incorrect Records", "alert('Possible leave records found. Please update in the table below and then generate the T-Sheet')", true);
            LoadReason();
            dvAbsent.Visible = true;
            return;
        }
        else
        {
            dvAbsent.Visible = false;
        }
        if (rbTeam.Checked == true)
        {
            ds = CheckIncorectRecords();

            if (ds.Tables["incorrect"].Rows.Count > 0)
            {
                BuildTimeSheetTable();
                foreach (DataRow dr2 in ds.Tables["incorrect"].Rows)
                {
                    dr = dtResult.NewRow();
                    dr["username"] = dr2["R_User_Name"].ToString();
                    dr["date"] = Convert.ToDateTime(dr2["R_Start_Date_Time"]).ToShortDateString();
                    // dr["date"] = "";
                    dr["jobcode"] = dr2["CL_TSheetClient"];
                    dr["service code"] = "service code";
                    dr["service code value"] = dr2["STL_ServiceCode"];
                    dr["notes"] = dr2["R_Comments"].ToString();
                    decimal num1 = Convert.ToInt32(dr2["hour"]);
                    decimal num2 = Convert.ToInt32(dr2["minute"]);

                    decimal num = num1 + num2 / 60;

                    dr["hours"] = Convert.ToDecimal(dr2["hour"]) + Convert.ToDecimal(dr2["minute"]) / 60;

                    dr["hours"] = decimal.Round(Convert.ToDecimal(dr["hours"]), 4, MidpointRounding.AwayFromZero);
                    dtResult.Rows.Add(dr);
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Incorrect Records", "alert('Incompatible JOB CODE and SERVICE CODE found. T-Sheet upload file cannot be generated. Please click OK to see the records.')", true);

                gvTSheet.DataSource = dtResult;
                gvTSheet.DataBind();
                gvTSheet.Visible = true;
                lblNotice.Visible = true;
                return;
            }
        }
        //ds = getLogoutTime2();
        ds = GetLoginTime3();

        BuildTimeSheetTable();

        if (ds.Tables["Log"].Rows.Count > 0)
        {
            foreach (DataRow dr1 in ds.Tables["Log"].Rows)
            {
                
                string user = dr1["LA_User_Name"].ToString();
                DateTime loginTime = Convert.ToDateTime(dr1["LA_Start_Date_Time"]); //= Convert.ToDateTime(dr1["LA_LoginTime"]); //Convert.ToDateTime(ds.Tables["Log"].Rows[0]["LA_LoginTime"]);

                DateTime logoutTime; //= Convert.ToDateTime(dr1["LA_Start_Date_Time"]); // Convert.ToDateTime(ds.Tables["Log"].Rows[0]["LA_Start_Date_Time"]);

                //ds = GetLoginTime(user, logoutTime);
                ds = getLogoutTime3(user, loginTime);
                if (ds.Tables["Login"].Rows.Count > 0)
                {
                    //loginTime = Convert.ToDateTime(ds.Tables["Login"].Rows[0]["LA_Start_Date_Time"]);
                    logoutTime = Convert.ToDateTime(ds.Tables["Login"].Rows[0]["LA_Start_Date_Time"]);

                    if (loginTime < Convert.ToDateTime(datepicker.Value))
                    {
                        continue;
                    }

                    //dr["username"] = ds.Tables[""].Rows[0][""].ToString();
                    ds = getTSheetTasks(user, loginTime, logoutTime);
                    if (ds.Tables["TSheetTask"].Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in ds.Tables["TSheetTask"].Rows)
                        {
                            dr = dtResult.NewRow();
                            if (ddlEmp.Visible == true)
                            {
                                dr["username"] = user;
                            }
                            else
                            {
                                dr["username"] = Session["user"].ToString().TrimStart('C', 'O', 'R', 'P', '\\');
                            }

                            dr["date"] = loginTime.ToShortDateString();

                            if (dr2["STL_ServiceCode"].ToString().Contains("|") == false)
                            {
                                string clientname = dr2["CL_TSheetClient"].ToString();
                                if (clientname == "Fixed IPV Projects => Harman Intl Fixed IPV MSP")
                                {
                                    dr["jobcode"] = "Fixed IPV Projects => Harman Int'l Fixed IPV MSP";
                                }
                                else if (clientname == "Wireless Projects => Harman Intl Wireless MSP")
                                {
                                    dr["jobcode"] = "Wireless Projects => Harman Int'l Wireless MSP";
                                }
                                else
                                {
                                    dr["jobcode"] = dr2["CL_TSheetClient"];
                                }
                                dr["service code"] = "service code";
                                dr["service code value"] = dr2["STL_ServiceCode"].ToString().Trim();

                            }
                            else
                            {
                                string[] service = Convert.ToString(dr2["STL_ServiceCode"]).Split('|');
                                dr["jobcode"] = "Internal => " + service[0].Trim();
                                dr["service code"] = "service code";
                                dr["service code value"] = service[1].Trim();

                            }

                            //if (dr["jobcode"].ToString() == "Internal => 000101017 - Non-Task")
                            //{
                            //    dr["service code value"] = "General/Internal";
                            //}
                            if (Convert.ToInt32(dr2["hour"]) == 0 && Convert.ToInt32(dr2["minute"]) == 0 && Convert.ToInt32(dr2["seconds"]) == 0)
                            {
                                continue;
                            }

                            if (ds.Tables.Contains("TaskComments"))
                            {
                                ds.Tables.Remove(ds.Tables["TaskComments"]);
                            }

                            ds = GetTSheetTaksComments(user, loginTime, logoutTime, dr2["CL_TSheetClient"].ToString(), dr2["STL_ServiceCode"].ToString());
                            if (ds.Tables["TaskComments"].Rows.Count > 0)
                            {
                                foreach (DataRow dr7 in ds.Tables["TaskComments"].Rows)
                                {
                                    if (dr7["R_Comments"].ToString().Length > 0)
                                    {
                                        if (dr["notes"].ToString().Length == 0)
                                        {
                                            dr["notes"] = dr7["R_Comments"].ToString().Trim();
                                            dr["notes"] = dr["notes"].ToString().Replace("\r", "").Replace("\n", "");
                                        }
                                        else
                                        {
                                            dr["notes"] = dr["notes"] + "|" + dr7["R_Comments"].ToString().Trim();
                                            dr["notes"] = dr["notes"].ToString().Replace("\r", "").Replace("\n", "");
                                        }

                                    }

                                }
                                dr["notes"] = RTM.StringExt.Truncate(dr["notes"].ToString(), 512);
                            }

                            //dr["hours"] = dr2["hour"] + ":" + dr2["minute"] + ":" + dr2["seconds"];

                            decimal num1 = Convert.ToInt32(dr2["hour"]);
                            decimal num2 = Convert.ToInt32(dr2["minute"]);
                            decimal num3 = Convert.ToInt32(dr2["seconds"]);

                            decimal num = num1 + (num2 / 60) + (num3 / 3600);

                            dr["hours"] = Convert.ToDecimal(dr2["hour"]) + (Convert.ToDecimal(dr2["minute"]) / 60) + (Convert.ToDecimal(dr2["seconds"]) / 3600);

                            dr["hours"] = decimal.Round(Convert.ToDecimal(dr["hours"]), 4, MidpointRounding.AwayFromZero);

                            dtResult.Rows.Add(dr);

                        }
                    }

                    //ds = GetTSMeetings(user, loginTime, logoutTime);
                    ds = GetTSheetMeetings1(user, loginTime, logoutTime);
                    if (ds.Tables["TSMeetings"].Rows.Count > 0)
                    {
                        foreach (DataRow dr3 in ds.Tables["TSMeetings"].Rows)
                        {

                            dr = dtResult.NewRow();
                            dr["username"] = user;
                            dr["date"] = loginTime.ToShortDateString();
                            if (rbUser.Checked)
                            {
                                dt = objReal.GetMeetingServiceCode(dr3["group"].ToString(), "0", user);
                            }
                            else
                            {
                                dt = objReal.GetMeetingServiceCode(dr3["group"].ToString(), ddlEmp.SelectedValue, user);
                            }
                            if (dt.Rows.Count > 0)
                            {
                                dr["jobcode"] = dt.Rows[0]["M_ServiceCode"].ToString();
                            }
                            else
                            {
                                dr["jobcode"] = "Internal => 000001006 - Learning";
                            }

                            dr["service code"] = "service code";
                            dr["service code value"] = "General/Internal";
                            //if (Convert.ToInt32(dr3["hour"]) == 0 && Convert.ToInt32(dr3["minute"]) == 0 && Convert.ToInt32(dr3["seconds"]) == 0)
                            //{
                            //    continue;
                            //}
                            //dr["hours"] = dr3["hour"] + ":" + dr3["minute"] + ":" + dr3["seconds"];
                            dr["hours"] = Convert.ToDecimal(dr3["hour"]) + (Convert.ToDecimal(dr3["minute"]) / 60) + (Convert.ToDecimal(dr3["seconds"]) / 3600);
                            dr["hours"] = decimal.Round(Convert.ToDecimal(dr["hours"]), 4, MidpointRounding.AwayFromZero);
                            ds = GetTSheetMeetComments1(user, loginTime, logoutTime);
                            if (ds.Tables["TSLearningComments"].Rows.Count > 0)
                            {
                                foreach (DataRow dr4 in ds.Tables["TSLearningComments"].Rows)
                                {
                                    if (dr4["LA_Comments"].ToString().Length > 0)
                                    {
                                        if (dr["notes"].ToString().Length == 0)
                                        {
                                            dr["notes"] = dr4["LA_Comments"].ToString().Trim();
                                            dr["notes"] = dr["notes"].ToString().Replace("\r", "").Replace("\n", "");
                                        }
                                        else
                                        {
                                            dr["notes"] = dr["notes"] + "|" + dr4["LA_Comments"].ToString().Trim();
                                            dr["notes"] = dr["notes"].ToString().Replace("\r", "").Replace("\n", "");
                                        }

                                    }

                                }
                                dr["notes"] = RTM.StringExt.Truncate(dr["notes"].ToString(), 512);

                            }
                            
                            if (dr["hours"].ToString() != "0")
                            {
                                dtResult.Rows.Add(dr);
                            }
                        }
                    }
                    //else
                    //{
                    ds = GetTSConfCalls(user, loginTime, logoutTime);
                    if (ds.Tables["TSConf"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();
                        dr["username"] = user;
                        dr["date"] = loginTime.ToShortDateString();
                        dr["jobcode"] = "Internal => 000001006 - Learning";
                        dr["service code"] = "service code";
                        dr["service code value"] = "General/Internal";

                        // dr["hours"] = ds.Tables["TSConf"].Rows[0]["hour"].ToString() + ":" + ds.Tables["TSConf"].Rows[0]["minute"].ToString() + ":" + ds.Tables["TSConf"].Rows[0]["seconds"].ToString();
                        dr["hours"] = Convert.ToDecimal(ds.Tables["TSConf"].Rows[0]["hour"]) + Convert.ToDecimal(ds.Tables["TSConf"].Rows[0]["minute"]) / 60;
                        dr["hours"] = decimal.Round(Convert.ToDecimal(dr["hours"]), 2, MidpointRounding.AwayFromZero);
                        ds = GetConfCallComments(user, loginTime, logoutTime);
                        if (ds.Tables["TSConfComments"].Rows.Count > 0)
                        {
                            foreach (DataRow dr4 in ds.Tables["TSConfComments"].Rows)
                            {
                                if (dr4["LA_Comments"].ToString().Length > 0)
                                {
                                    if (dr["notes"].ToString().Length == 0)
                                    {
                                        dr["notes"] = dr4["LA_Comments"].ToString().Trim();
                                        dr["notes"] = dr["notes"].ToString().Replace("\r", "").Replace("\n", "");
                                    }
                                    else
                                    {
                                        dr["notes"] = dr["notes"] + "|" + dr4["LA_Comments"].ToString().Trim();
                                        dr["notes"] = dr["notes"].ToString().Replace("\r", "").Replace("\n", "");
                                    }

                                }

                            }
                        }
                        if (dr["hours"].ToString() != "0")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }
                   
                    ds = GetTSOthers(user, loginTime, logoutTime);
                    if (ds.Tables["TSOthers"].Rows.Count > 0)
                    {
                        dr = dtResult.NewRow();

                        if (ddlEmp.Visible == true)
                        {
                            dr["username"] = user;
                        }
                        else
                        {
                            dr["username"] = Session["user"].ToString().TrimStart('C', 'O', 'R', 'P', '\\');
                        }
                        dr["date"] = loginTime.ToShortDateString();
                        dr["jobcode"] = "Internal => 000101017 - Non-Task";
                        dr["service code"] = "service code";
                        dr["service code value"] = "General/Internal";
                        //dr["notes"] = ds.Tables["TSOthers"].Rows[0]["LA_Comments"].ToString().Trim();

                        //dr["hours"] = ds.Tables["TSOthers"].Rows[0]["hour"].ToString() + ":" + ds.Tables["TSOthers"].Rows[0]["minute"].ToString() + ":" + ds.Tables["TSOthers"].Rows[0]["seconds"].ToString();
                        dr["hours"] = Convert.ToDecimal(ds.Tables["TSOthers"].Rows[0]["hour"]) + (Convert.ToDecimal(ds.Tables["TSOthers"].Rows[0]["minute"]) / 60) + (Convert.ToDecimal(ds.Tables["TSOthers"].Rows[0]["seconds"]) / 3600);
                        dr["hours"] = decimal.Round(Convert.ToDecimal(dr["hours"]), 4, MidpointRounding.AwayFromZero);

                        ds = GetOthersComments(user, loginTime, logoutTime);

                        if (ds.Tables["TSOthersComments"].Rows.Count > 0)
                        {
                            foreach (DataRow dr4 in ds.Tables["TSOthersComments"].Rows)
                            {
                                if (dr4["LA_Comments"].ToString().Length > 0)
                                {
                                    if (dr["notes"].ToString().Length == 0)
                                    {
                                        dr["notes"] = dr4["LA_Comments"].ToString().Trim();
                                        dr["notes"] = dr["notes"].ToString().Replace("\r", "").Replace("\n", "");
                                    }
                                    else
                                    {
                                        dr["notes"] = dr["notes"] + "|" + dr4["LA_Comments"].ToString().Trim();
                                        dr["notes"] = dr["notes"].ToString().Replace("\r", "").Replace("\n", "");
                                    }

                                }

                            }
                            dr["notes"] = RTM.StringExt.Truncate(dr["notes"].ToString(), 512);
                        }

                        if (dr["hours"].ToString() != "0")
                        {
                            dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
        }

        ds = GetLeaveDetails(ddlEmp.SelectedItem.Text);

        if (ds.Tables["leaveDetails"].Rows.Count > 0)
        {
            foreach (DataRow dr4 in ds.Tables["leaveDetails"].Rows)
            {
                dr = dtResult.NewRow();
                dr["username"] = dr4["LD_UserName"];
                dr["date"] = Convert.ToDateTime(dr4["LD_Date"]).ToShortDateString();
                dr["jobcode"] = dr4["L_JobCode"];
                if (dr4["L_ServiceCode"].ToString().Length > 0)
                {
                    dr["service code"] = "service code";
                    dr["service code value"] = dr4["L_ServiceCode"];
                }
                dr["hours"] = "8";
                dr["notes"] = "";
                dtResult.Rows.Add(dr);
            }
        }

        if (Session["access"].ToString() == "4")
        {

            //DataTable dt3 = new DataTable();
            //dt3 = GetInversedDataTable(dtResult, "date", "jobcode", "hours", "", true);
            if (rbExport.Checked)
            {
                ExportToCSV(dtResult);
            }
            else
            {
                gvTSheet.DataSource = dtResult;
                gvTSheet.DataBind();
                gvTSheet.Visible = true;
            }
        }
        else if (rbUser.Checked == true)
        {
            //DataTable dt3 = new DataTable();
            //dt3 = GetInversedDataTable(dtResult, "date", "jobcode", "hours", "", true);
            if (rbExport.Checked)
            {
                ExportToCSV(dtResult);
            }
            else
            {
                gvTSheet.DataSource = dtResult;
                gvTSheet.DataBind();
                gvTSheet.Visible = true;
            }
           
        }
        else
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + ddlEmp.SelectedItem.Text + "-" + datepicker.Value + "-" + datepickerTo.Value + "-TSheet.csv");
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
            //try
            //{
            //    Response.End();
            //}
            //catch (Exception)
            //{

            //}
            //finally
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "TSheet", "alert('TSheet upload file exported successfully')", true);
            //}


        }



    }

    private void ExportToCSV(DataTable dtResult)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=" + ddlEmp.SelectedItem.Text + "-" + datepicker.Value + "-" + datepickerTo.Value + "-TSheet.csv");
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

    private void PrintTSheetReport2()
    {

        DataRow dr;

        //ds = getLogoutTime();

        BuildTimeSheetTable();

        ds = getTSheetTasks(ddlEmp.SelectedItem.Text, Convert.ToDateTime(datepicker.Value), Convert.ToDateTime(datepickerTo.Value));
        if (ds.Tables["TSheetTask"].Rows.Count > 0)
        {
            foreach (DataRow dr2 in ds.Tables["TSheetTask"].Rows)
            {
                dr = dtResult.NewRow();
                if (ddlEmp.Visible == true)
                {
                    dr["username"] = ddlEmp.SelectedItem.Text;
                }
                else
                {
                    dr["username"] = Session["user"].ToString().TrimStart('C', 'O', 'R', 'P', '\\');
                }


                dr["date"] = Convert.ToDateTime(dr2["date1"]).ToShortDateString(); // Convert.ToDateTime(datepicker.Value).ToShortDateString();

                if (dr2["STL_ServiceCode"].ToString().Contains("|") == false)
                {
                    dr["jobcode"] = dr2["CL_TSheetClient"];
                    dr["service code"] = "service code";
                    dr["service code value"] = dr2["STL_ServiceCode"];

                }
                else
                {
                    string[] service = Convert.ToString(dr2["STL_ServiceCode"]).Split('|');
                    dr["jobcode"] = "Internal =>" + service[0];
                    dr["service code"] = "service code";
                    dr["service code value"] = service[1];

                }

                dr["hours"] = dr2["hour"] + ":" + dr2["minute"] + ":" + dr2["seconds"];

                dtResult.Rows.Add(dr);

            }
        }

        ds = GetTSMeetings(ddlEmp.SelectedItem.Text, Convert.ToDateTime(datepicker.Value), Convert.ToDateTime(datepickerTo.Value));
        if (ds.Tables["TSMeetings"].Rows.Count > 0)
        {
            foreach (DataRow dr3 in ds.Tables["TSMeetings"].Rows)
            {
                if (dr3["LA_Comments"].ToString().Length >= 0)
                {
                    dr = dtResult.NewRow();
                    string[] comment = Convert.ToString(dr3["LA_Comments"]).Split('-');
                    //ds.Tables["servicecode"].Clear();
                    ds = GetMeetingsServiceCode(comment[1]);

                    if (ds.Tables["servicecode"].Rows.Count > 0)
                    {
                        string[] service = ds.Tables["servicecode"].Rows[0]["M_ServiceCode"].ToString().Split('|');

                        if (ddlEmp.Visible == true)
                        {
                            dr["username"] = ddlEmp.SelectedItem.Text;
                        }
                        else
                        {
                            dr["username"] = Session["user"].ToString().TrimStart('C', 'O', 'R', 'P', '\\');
                        }

                        dr["date"] = Convert.ToDateTime(dr3["date1"]).ToShortDateString(); //Convert.ToDateTime(datepicker.Value).ToShortDateString();
                        dr["jobcode"] = "Internal => " + service[0].Trim();
                        dr["service code"] = "service code";
                        dr["service code value"] = service[1].Trim();

                        dr["hours"] = dr3["hour"] + ":" + dr3["minute"] + ":" + dr3["seconds"];
                        dtResult.Rows.Add(dr);
                    }

                }
            }
        }

        ds = GetTSConfCalls(ddlEmp.SelectedItem.Text, Convert.ToDateTime(datepicker.Value), Convert.ToDateTime(datepickerTo.Value));
        if (ds.Tables["TSConf"].Rows.Count > 0)
        {
            dr = dtResult.NewRow();

            if (ddlEmp.Visible == true)
            {
                dr["username"] = ddlEmp.SelectedItem.Text;
            }
            else
            {
                dr["username"] = Session["user"].ToString().TrimStart('C', 'O', 'R', 'P', '\\');
            }
            dr["date"] = Convert.ToDateTime(ds.Tables["TSConf"].Rows[0]["date1"]).ToShortDateString(); // Convert.ToDateTime(datepicker.Value).ToShortDateString();
            dr["jobcode"] = "Internal => 000001006 - Learning";
            dr["service code"] = "service code";
            dr["service code value"] = "General/Internal";

            dr["hours"] = ds.Tables["TSConf"].Rows[0]["hour"].ToString() + ":" + ds.Tables["TSConf"].Rows[0]["minute"].ToString() + ":" + ds.Tables["TSConf"].Rows[0]["seconds"].ToString();
            dtResult.Rows.Add(dr);
        }

        ds = GetTSOthers(ddlEmp.SelectedItem.Text, Convert.ToDateTime(datepicker.Value), Convert.ToDateTime(datepickerTo.Value));
        if (ds.Tables["TSOthers"].Rows.Count > 0)
        {
            dr = dtResult.NewRow();

            if (ddlEmp.Visible == true)
            {
                dr["username"] = ddlEmp.SelectedItem.Text;
            }
            else
            {
                dr["username"] = Session["user"].ToString().TrimStart('C', 'O', 'R', 'P', '\\');
            }
            dr["date"] = Convert.ToDateTime(ds.Tables["TSOthers"].Rows[0]["date1"]).ToShortDateString(); // Convert.ToDateTime(datepicker.Value).ToShortDateString();
            dr["jobcode"] = "Internal => 000101017 - Non-Task";
            dr["service code"] = "service code";
            dr["service code value"] = "General/Internal";

            dr["hours"] = ds.Tables["TSOthers"].Rows[0]["hour"].ToString() + ":" + ds.Tables["TSOthers"].Rows[0]["minute"].ToString() + ":" + ds.Tables["TSOthers"].Rows[0]["seconds"].ToString();
            dtResult.Rows.Add(dr);
        }

        dtResult.DefaultView.Sort = "date";
        dtResult = dtResult.DefaultView.ToTable();

        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=TSheet-" + DateTime.Now + ".csv");
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


        Response.Output.Write(sb.ToString());
        Response.Flush();
        Response.End();
    }

    public DataTable GetInversedDataTable(DataTable table, string columnX, string columnY, string columnZ, string nullValue, bool sumValues)
    {

        DataTable returnTable = new DataTable();

        if (string.IsNullOrEmpty(columnX))
        {
            columnX = table.Columns[0].ColumnName;
        }

        //Add a Column at the beginning of the table
        returnTable.Columns.Add(columnY);



        List<string> columnXValues = new List<string>();

        foreach (DataRow dr in table.Rows)
        {
            string columnXTemp = dr[columnX].ToString();
            if (!columnXValues.Contains(columnXTemp))
            {
                columnXValues.Add(columnXTemp);
                returnTable.Columns.Add(columnXTemp);
            }
        }

        //Verify if Y and Z Axis columns re provided
        if (!string.IsNullOrEmpty(columnY) && !string.IsNullOrEmpty(columnZ))
        {
            //Read DISTINCT Values for Y Axis Column
            List<string> columnYValues = new List<string>();

            foreach (DataRow dr in table.Rows)
            {
                if (!columnYValues.Contains(dr[columnY].ToString()))
                {
                    columnYValues.Add(dr[columnY].ToString());
                }
            }

            //Loop all Column Y Distinct Value
            foreach (string columnYValue in columnYValues)
            {
                //Creates a new Row
                DataRow drReturn = returnTable.NewRow();
                drReturn[0] = columnYValue;
                //foreach column Y value, The rows are selected distincted
                DataRow[] rows = table.Select((columnY + "='") + columnYValue + "'");

                //Read each row to fill the DataTable
                foreach (DataRow dr in rows)
                {
                    string rowColumnTitle = dr[columnX].ToString();

                    //Read each column to fill the DataTable
                    foreach (DataColumn dc in returnTable.Columns)
                    {
                        if (dc.ColumnName == rowColumnTitle)
                        {
                            //If Sum of Values is True it try to perform a Sum
                            //If sum is not possible due to value types, the value displayed is the last one read
                            if (sumValues)
                            {
                                try
                                {
                                    drReturn[rowColumnTitle] = TimeSpan.Parse(drReturn[rowColumnTitle].ToString()).Add(TimeSpan.Parse(dr[columnZ].ToString()));
                                }
                                catch
                                {
                                    drReturn[rowColumnTitle] = dr[columnZ];
                                }
                            }
                            else
                            {
                                drReturn[rowColumnTitle] = dr[columnZ];
                            }
                        }
                    }
                }
                returnTable.Rows.Add(drReturn);
            }
        }
        else
        {
            throw new Exception("The columns to perform inversion are not provided");
        }

        //if a nullValue is provided, fill the datable with it
        if (!string.IsNullOrEmpty(nullValue))
        {
            foreach (DataRow dr in returnTable.Rows)
            {
                foreach (DataColumn dc in returnTable.Columns)
                {
                    if (string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                    {
                        dr[dc.ColumnName] = nullValue;
                    }
                }
            }
        }
        return returnTable;
    }

    protected void gvIncorrectData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (gvIncorrectData.EditIndex == e.Row.RowIndex)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Saplin.Controls.DropDownCheckBoxes ddlMultiClient = (Saplin.Controls.DropDownCheckBoxes)e.Row.FindControl("ddlMultiple");
                //Saplin.Controls.DropDownCheckBoxes ddlMultiClient =
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@emp",ddlEmp.SelectedItem.Text),
                    new SqlParameter("@team", ddlEmp.SelectedValue)
                };
                ds = new DataSet();
                if (rbUser.Checked == true)
                {
                    sQuery = "select * from RTM_Client_List, RTM_User_List where CL_TeamId= UL_Team_Id and UL_User_Name=@emp and CL_Status=1 and CL_ClientName <> 'Other' order by CL_ClientName";

                }
                else if (rbTeam.Checked == true)
                {
                    sQuery = "select * from RTM_Client_List where CL_TeamId= @team and CL_Status=1 and CL_ClientName <> 'Other' order by CL_ClientName";
                }

                ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "multiclient");
                if (ds.Tables["multiclient"].Rows.Count > 0)
                {
                    ddlMultiClient.DataSource = ds.Tables["multiclient"];
                    ddlMultiClient.DataTextField = "CL_ClientName";
                    ddlMultiClient.DataValueField = "CL_ID";
                    ddlMultiClient.DataBind();
                }
            }
        }
    }

    protected void gvIncorrectData_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvIncorrectData.EditIndex = -1;
        BindIncorrectGrid();
    }

    protected void gvIncorrectData_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvIncorrectData.EditIndex = e.NewEditIndex;
        BindIncorrectGrid();
    }

    protected void gvIncorrectData_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int checkedCount = 0;
        try
        {
            int id = Convert.ToInt32(gvIncorrectData.DataKeys[e.RowIndex].Values["R_ID"].ToString());
            Saplin.Controls.DropDownCheckBoxes ddlMultiSelect = (Saplin.Controls.DropDownCheckBoxes)gvIncorrectData.Rows[e.RowIndex].FindControl("ddlMultiple");

            foreach (System.Web.UI.WebControls.ListItem item in ddlMultiSelect.Items)
            {
                if (item.Selected)
                {
                    checkedCount = checkedCount + 1;
                }
            }

            if (checkedCount == 0)
            {
                lblError.Text = "Please select the clients";
                lblError.ForeColor = System.Drawing.Color.Red;
                ddlMultiSelect.Focus();
                return;
            }

            if (checkedCount == 1)
            {
                foreach (System.Web.UI.WebControls.ListItem item in ddlMultiSelect.Items)
                {
                    if (item.Selected)
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@client",item.Value),
                            new SqlParameter("@id", id)
                        };
                        sQuery = "Update RTM_Records SET R_Client= @client where R_ID=@id";
                        bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                    }
                }
            }
            else
            {
                ds = getRecord(id);

                if (ds.Tables["Irecord"].Rows.Count > 0)
                {
                    int TID = Convert.ToInt32(ds.Tables["Irecord"].Rows[0]["R_TeamId"]);
                    string EID = ds.Tables["Irecord"].Rows[0]["R_Employee_Id"].ToString();
                    string Uname = ds.Tables["Irecord"].Rows[0]["R_User_Name"].ToString();
                    int taskId = Convert.ToInt32(ds.Tables["Irecord"].Rows[0]["R_Task"]);
                    int subTaskId = Convert.ToInt32(ds.Tables["Irecord"].Rows[0]["R_SubTask"]);
                    string comments = ds.Tables["Irecord"].Rows[0]["R_Comments"].ToString();
                    string dur = ds.Tables["Irecord"].Rows[0]["R_Duration"].ToString();
                    string startTime = ds.Tables["Irecord"].Rows[0]["R_Start_Date_Time"].ToString();
                    string createdOn = ds.Tables["Irecord"].Rows[0]["R_CreatedOn"].ToString();

                    if (dur != "")
                    {
                        double timeSeconds = TimeSpan.Parse(dur).TotalSeconds;
                        timeSeconds = timeSeconds / checkedCount;
                        // string splitTime = ConvertToTime(timeSeconds);
                        var hms = TimeSpan.FromSeconds(timeSeconds);
                        var h = hms.Hours.ToString("00");
                        var m = hms.Minutes.ToString("00");
                        var s = hms.Seconds.ToString("00");
                        string splitTime = h + ":" + m + ":" + s;

                        foreach (System.Web.UI.WebControls.ListItem item in ddlMultiSelect.Items)
                        {
                            if (item.Selected)
                            {
                                SqlParameter[] parameters = new SqlParameter[]
                                {
                                    new SqlParameter("@teamId",TID),
                                    new SqlParameter("@empId",EID),
                                    new SqlParameter("@userName",Uname),
                                    new SqlParameter("@client", item.Value),
                                    new SqlParameter("@task",taskId),
                                    new SqlParameter("@subtask",subTaskId),
                                    new SqlParameter("@comments",comments),
                                    new SqlParameter("@duration",splitTime),
                                    new SqlParameter("@startTime",startTime),
                                    new SqlParameter("@createdOn",createdOn),
                                    new SqlParameter("@status", "Completed")
                                };
                                sQuery = "insert into RTM_Records(R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn, R_Status) values (@teamId, @empId, @userName, @client, @task, @subtask, @comments, @duration, @startTime, @createdOn,@status )";
                                bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                            }
                        }

                        SqlParameter[] parameters1 = new SqlParameter[]
                        {
                            new SqlParameter("@id",id)
                        };
                        sQuery = "Delete from RTM_Records where R_ID =@id";
                        bool result1 = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters1);

                    }


                }
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Incorrect Record", "alert('Record Corrected Successfully')", true);
            gvIncorrectData.EditIndex = -1;
            BindIncorrectGrid();
        }
        catch (Exception)
        {


        }

    }

    public string ConvertToTime(double timeSeconds)
    {
        int mySeconds = System.Convert.ToInt32(timeSeconds);

        int myHours = mySeconds / 3600; //3600 Seconds in 1 hour
        mySeconds %= 3600;

        int myMinutes = mySeconds / 60; //60 Seconds in a minute
        mySeconds %= 60;


        string mySec = mySeconds.ToString(),
        myMin = myMinutes.ToString(),
        myHou = myHours.ToString();

        if (myHours < 10) { myHou = myHou.Insert(0, "0"); }
        if (myMinutes < 10) { myMin = myMin.Insert(0, "0"); }
        if (mySeconds < 10) { mySec = mySec.Insert(0, "0"); }


        return myHou + ":" + myMin + ":" + mySec;
    }

    private DataSet getRecord(int rId)
    {
        SqlParameter[] parameters1 = new SqlParameter[]
         {
             new SqlParameter("@id",rId)
         };
        if (ds.Tables.Contains("Irecord"))
        {
            ds.Tables.Remove(ds.Tables["Irecord"]);
        }
        sQuery = "select * from RTM_Records where R_ID =@id";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters1, "Irecord");
        return ds;
    }

    private void BindIncorrectGrid()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@team", Convert.ToInt32(ddlEmp.SelectedValue)),
            new SqlParameter("@from", datepicker.Value),
            new SqlParameter("@to", datepickerTo.Value),
            new SqlParameter("@emp", ddlEmp.SelectedItem.Text)
            
        };
        dt = new DataTable();
        if (rbUser.Checked == true)
        {
            sQuery = "select R_ID, R_User_Name, R_Start_Date_Time, CL_ClientName, TL_Task, STL_SubTask,CL_TSheetClient, STL_ServiceCode, REPLACE(R_Duration,'-', '') as R_Duration, R_Comments from RTM_Records, RTM_Client_List, RTM_Task_List, RTM_SubTask_List where R_Client = CL_ID and R_Task=TL_ID  and R_SubTask = STL_ID And R_User_Name=@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from AND @to and CL_TSheetClient='Internal' and STL_ServiceCode NOT LIKE '% General/Internal'  and R_Duration != 'HH:MM:SS' order by R_ID";
        }
        else if (rbTeam.Checked == true)
        {
            sQuery = "select R_ID, R_User_Name, R_Start_Date_Time, CL_ClientName, TL_Task, STL_SubTask,CL_TSheetClient, STL_ServiceCode, REPLACE(R_Duration,'-', '') as R_Duration, R_Comments from RTM_Records, RTM_Client_List, RTM_Task_List, RTM_SubTask_List where R_Client = CL_ID and R_Task=TL_ID  and R_SubTask = STL_ID And R_TeamId=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from AND @to and CL_TSheetClient='Internal' and STL_ServiceCode NOT LIKE '% General/Internal'  and R_Duration != 'HH:MM:SS' order by R_ID";
        }
        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        //da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            gvIncorrectData.DataSource = dt;
            gvIncorrectData.DataBind();

            gvIncorrectData.Visible = true;

        }
        else
        {
            gvIncorrectData.DataSource = null;
            gvIncorrectData.DataBind();
            gvIncorrectData.Visible = false;
        }

    }

    private void BuildEarlyLogOffTable()
    {
        dt = new DataTable();
        DataColumn dc;

        dc = new DataColumn("EL_ID");
        dt.Columns.Add(dc);

        dc = new DataColumn("EL_User_Name");
        dt.Columns.Add(dc);

        dc = new DataColumn("EL_Date");
        dt.Columns.Add(dc);

        dc = new DataColumn("EL_Scheduled");
        dt.Columns.Add(dc);

        dc = new DataColumn("EL_Actual");
        dt.Columns.Add(dc);

        dc = new DataColumn("EL_Reason");
        dt.Columns.Add(dc);

        dc = new DataColumn("EL_Comments");
        dt.Columns.Add(dc);

        dc = new DataColumn("EL_Total_Office_Hours");
        dt.Columns.Add(dc);

        dc = new DataColumn("Cab Delay");
        dt.Columns.Add(dc);
    }

    private DataSet GetDelayLoginDuration(string user, string date)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@user",user),
            new SqlParameter("@date",date)
        };
        if (ds.Tables.Contains("cab"))
        {
            ds.Tables.Remove(ds.Tables["cab"]);
        }
        sQuery = "select D_Duration from dbo.RTM_DelayedLogInOff where D_UserName= @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, D_Date))) = @date and D_Reason='Company Cab'";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "cab");
        return ds;
    }

    private void BindEarlyLogoffGrid()
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@from",datepicker.Value),
            new SqlParameter("@to",datepickerTo.Value),
            new SqlParameter("@team", ddlEmp.SelectedValue)
        };
        BuildEarlyLogOffTable();
        DataRow dr;
        if (ds.Tables.Contains("early"))
        {
            ds.Tables.Remove(ds.Tables["early"]);
        }
        //dt = new DataTable();
        sQuery = "select EL_ID, EL_User_Name , CONVERT(VARCHAR(10), EL_Date, 101) as EL_Date, LTRIM(RIGHT(CONVERT(VARCHAR(20), EL_Scheduled, 100), 7)) as [EL_Scheduled], EL_Actual, EL_Reason, EL_Comments, EL_Total_Office_Hours from RTM_EarlyLogOffDetails, RTM_User_List, RTM_Team_List where EL_User_Name= UL_User_Name and UL_Team_Id = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EL_Actual))) BETWEEN @from and @to and T_ID=@team";

        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "early");

        if (ds.Tables["early"].Rows.Count > 0)
        {
            foreach (DataRow drEarly in ds.Tables["early"].Rows)
            {
                dr = dt.NewRow();

                dr["EL_ID"] = drEarly["EL_ID"];
                dr["EL_User_Name"] = drEarly["EL_User_Name"];
                dr["EL_Date"] = drEarly["EL_Date"];
                dr["EL_Scheduled"] = drEarly["EL_Scheduled"];
                dr["EL_Actual"] = drEarly["EL_Actual"];
                dr["EL_Reason"] = drEarly["EL_Reason"];
                dr["EL_Comments"] = drEarly["EL_Comments"];
                dr["EL_Total_Office_Hours"] = drEarly["EL_Total_Office_Hours"];
                ds = GetDelayLoginDuration(drEarly["EL_User_Name"].ToString(), drEarly["EL_Date"].ToString());
                if (ds.Tables["cab"].Rows.Count > 0)
                {
                    dr["Cab Delay"] = ds.Tables["cab"].Rows[0]["D_Duration"].ToString();
                }

                dt.Rows.Add(dr);
            }
            gvEarlyLogoff.DataSource = dt;
            gvEarlyLogoff.DataBind();
            gvEarlyLogoff.Visible = true;
        }
        else
        {
            gvEarlyLogoff.Visible = false;
        }
    }

    protected void gvEarlyLogoff_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvEarlyLogoff.EditIndex = -1;
        BindEarlyLogoffGrid();
    }

    protected void gvEarlyLogoff_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvEarlyLogoff.EditIndex = e.NewEditIndex;
        BindEarlyLogoffGrid();
    }

    protected void gvEarlyLogoff_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            int id = Convert.ToInt32(gvEarlyLogoff.DataKeys[e.RowIndex].Values["EL_ID"].ToString());
            DropDownList ddlReason = (DropDownList)gvEarlyLogoff.Rows[e.RowIndex].FindControl("ddlReason");
            TextBox txtComments = (TextBox)gvEarlyLogoff.Rows[e.RowIndex].FindControl("txtComments");
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@reason",ddlReason.SelectedItem.Text),
                new SqlParameter("@comments",txtComments.Text),
                new SqlParameter("@id", id)
            };
            sQuery = "Update RTM_EarlyLogOffDetails SET  EL_Reason=@reason , EL_Comments=@comments where EL_ID=@id";
            bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);


            gvEarlyLogoff.EditIndex = -1;
            BindEarlyLogoffGrid();
        }
        catch (Exception)
        {


        }
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
            e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
        }
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    DropDownList ddlReason = (DropDownList)e.Row.FindControl("ddlReason");
        //    dt = new DataTable();

        //    da = new SqlDataAdapter("select * from dbo.RTM_Leave where L_Status =1", con);
        //    da.Fill(dt);

        //    ddlReason.DataSource = dt;
        //    ddlReason.DataTextField = "L_Type";
        //    ddlReason.DataValueField = "L_ID";
        //    ddlReason.DataBind();

        //    ddlReason.Items.Insert(0, "--Select--");
        //    ddlReason.SelectedIndex = 0;
        //}
    }

    //protected void lbUpdate_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        LinkButton lnkbtn = sender as LinkButton;
    //        GridViewRow gvrow = lnkbtn.NamingContainer as GridViewRow;
    //        DropDownList ddlReason = (DropDownList)gvrow.FindControl("ddlReason");
    //        Label lblName = (Label)gvrow.FindControl("lblName");
    //        Label lblDate = (Label)gvrow.FindControl("lblDate");
    //        string name = lblName.Text;
    //        string date = lblDate.Text;
    //        string reason = ddlReason.SelectedValue;
    //        if (ddlReason.SelectedIndex == 0)
    //        {
    //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Reason", "alert('Please Select Reason')", true);
    //            return;
    //        }
    //        else
    //        {
    //            dt = CheckLeaveRecord(lblName.Text, lblDate.Text);
    //            if (dt.Rows.Count > 0)
    //            {
    //                SqlParameter[] parameters = new SqlParameter[]
    //                {
    //                    new SqlParameter("@reason",reason),
    //                    new SqlParameter("@modifiedBy",Session["UID"].ToString()),
    //                    new SqlParameter("@modifiedOn", DateTime.Now),
    //                    new SqlParameter("@id", dt.Rows[0]["LD_ID"])
    //                };
    //                sQuery = "UPDATE RTM_LeaveDetails SET LD_LID = @reason, LD_ModifiedBy=@modifiedBy, LD_ModifiedOn=@modifiedOn where LD_ID = @id";
    //                bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

    //                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Save", "alert('Detailes Updated Successfully')", true);
    //            }
    //            else
    //            {
    //                SqlParameter[] parameters = new SqlParameter[]
    //                {
    //                    new SqlParameter("@user",lblName.Text),
    //                    new SqlParameter("@reason",reason),
    //                    new SqlParameter("@date", lblDate.Text),
    //                    new SqlParameter("@createdBy", Session["UID"].ToString()),
    //                    new SqlParameter("@createdOn", DateTime.Now)
    //                };
    //                sQuery = "INSERT INTO RTM_LeaveDetails (LD_UserName, LD_LID, LD_Date, LD_CreatedBy, LD_CreatedOn) VALUES (@user, @reason, @date, @createdBy, @createdOn)";
    //                bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

    //                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Save", "alert('Details Saved Successfully')", true);
    //            }
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Save", "alert('Error occured while inserting. Please try again later')", true);
    //    }

    //}

    private void LoadReason()
    {
        dt = new DataTable();
        da = new SqlDataAdapter("select * from dbo.RTM_Leave where L_Status =1", con);
        da.Fill(dt);

        ddlReasonNew.DataSource = dt;
        ddlReasonNew.DataTextField = "L_Type";
        ddlReasonNew.DataValueField = "L_ID";
        ddlReasonNew.DataBind();

        ddlReasonNew.Items.Insert(0, "--Select--");
        ddlReasonNew.SelectedIndex = 0;
    }

    private DataTable CheckLeaveRecord(string name, string date)
    {
        SqlParameter[] parameters = new SqlParameter[]
        {
           new SqlParameter("@name",name),
           new SqlParameter("@date",date)                       
        };
        dt = new DataTable();
        sQuery = "select * from dbo.RTM_LeaveDetails where LD_UserName=@name and LD_Date= @date";

        dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
        return dt;
    }
    protected void btnAbsent_Click(object sender, EventArgs e)
    {
        int check = 0;
        if (ddlReasonNew.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Reason", "alert('Please Select Reason')", true);
            return;
        }
        foreach (GridViewRow row in GridView1.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkRow = (row.Cells[0].FindControl("CheckBox1") as CheckBox);
                if (chkRow.Checked)
                {
                    string name = (row.Cells[1].FindControl("lblName") as Label).Text;
                    string date = Convert.ToDateTime((row.Cells[2].FindControl("lblDate") as Label).Text).ToString("MM/dd/yyyy");
                    string reason = ddlReasonNew.SelectedValue;



                    SqlParameter[] parameters = new SqlParameter[]
                      {
                         new SqlParameter("@user",name),
                         new SqlParameter("@reason",reason),
                         new SqlParameter("@date", date),
                         new SqlParameter("@createdBy", Session["UID"].ToString()),
                         new SqlParameter("@createdOn", DateTime.Now)
                      };
                    sQuery = "INSERT INTO RTM_LeaveDetails (LD_UserName, LD_LID, LD_Date, LD_CreatedBy, LD_CreatedOn) VALUES (@user, @reason, @date, @createdBy, @createdOn)";
                    bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

                    check = 1;
                }
            }
        }

        if (check == 1)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Save", "alert('Details Saved Successfully')", true);
            CheckAbsentsNew();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select", "alert('Please select at least one user to update leave record.')", true);
            //CheckAbsentsNew();
        }
    }

    private void ExportToExcel(DataTable dt)
    {

        string attachment = "attachment; filename=SummaryReport_"+ ddlEmp.SelectedItem.Text +".xls";
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