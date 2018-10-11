using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using DAL;

public partial class LateInEarlyOff : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    DataTable dtResult = new DataTable();
    SqlDataAdapter da;
    DataTable dt = new DataTable();
    DataSet ds = new DataSet();
    SqlCommand cmd;
    SqlDataReader reader;

    SqlDBHelper objDB = new SqlDBHelper();
    string SQlQuery;
    bool result;
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
    }

    protected void btnPrint_Click(object sender, EventArgs e)
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
            if (DateTime.TryParse(txtFrom.Text, out temp))
            { }
            else
            {
                lblError.Text = "Please Select valid from Date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (DateTime.TryParse(txtTo.Text, out temp))
            { }
            else
            {
                lblError.Text = "Please Select Valid To Date";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
            DisplayReport();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = System.Drawing.Color.Red;
        }

    }

    private void BuildTable()
    {
        dtResult = new DataTable();
        DataColumn dc;

        dc = new DataColumn("Team");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("User");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("First Activity");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Delayed Login");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Last Activity");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Early Logoff");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Total Office Hours");
        dtResult.Columns.Add(dc);

        dc = new DataColumn("Login Delay Reason");
        dtResult.Columns.Add(dc);
    }

    private void DisplayReport()
    {
        //Namohar code changes on 12-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", txtFrom.Text),
               new SqlParameter("@to", txtTo.Text),
           };

        BuildTable();
        DataRow dr1;
        
        ds = new DataSet();

        SQlQuery = "select LA_User_Name,LA_Log_Action,LA_Start_Date_Time, T_TeamName, UL_SCH_Login, UL_SCH_Logout from RTM_Log_Actions with (nolock) left join RTM_User_List with (nolock) on LA_User_Name = UL_User_Name left join RTM_Team_List with (nolock) on LA_TeamId = T_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @from and @to and LA_Log_Action ='First Activity' and T_Location = 'IND' Order By T_TeamName, UL_User_Name";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "tbo");
        if (ds.Tables["tbo"].Rows.Count > 0)
        {
            foreach (DataRow dr in ds.Tables["tbo"].Rows)
            {
                try
                {

                    if (dr["LA_User_Name"].ToString() == "Mohammed Sulaiman")
                    {
                        string us = dr["LA_User_Name"].ToString();
                    }
                string login = dr["LA_Start_Date_Time"].ToString();
                string loginTime = Convert.ToDateTime(login).ToString("HH:mm:ss");
                string schLogin = Convert.ToDateTime(dr["UL_SCH_Login"]).ToString("HH:mm:ss");
                string schLogoff = Convert.ToDateTime(dr["UL_SCH_Logout"]).ToString("HH:mm:ss");
                dt = new DataTable();
                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@start",dr["LA_Start_Date_Time"].ToString()),
                    new SqlParameter("@user", dr["LA_User_Name"].ToString())
                };
                SQlQuery = "select Top 1 LA_Start_Date_Time from RTM_Log_Actions where LA_Start_Date_Time > @start and LA_User_Name= @user and LA_Log_Action ='Last Activity' Order By LA_Start_Date_Time";
                dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, param);
                if (dt.Rows.Count > 0)
                {
                    string logout = dt.Rows[0]["LA_Start_Date_Time"].ToString();
                    string logoutTime = Convert.ToDateTime(logout).ToString("HH:mm:ss");
                    TimeSpan diff = Convert.ToDateTime(logout) - Convert.ToDateTime(login);
                    if (diff.TotalHours < 9)
                    {
                        dr1 = dtResult.NewRow();
                        dr1["Team"] = dr["T_TeamName"];
                        dr1["User"] = dr["LA_User_Name"];
                        dr1["First Activity"] = login;
                        if (TimeSpan.Parse(loginTime) > TimeSpan.Parse(schLogin))
                        {
                            dr1["Delayed Login"] = TimeSpan.Parse(loginTime).Subtract(TimeSpan.Parse(schLogin));
                            //Namohar code changes on 12-Aug-2016.
                            SqlParameter[] parameters1 = new SqlParameter[]
                                   {
                                       new SqlParameter("@login", Convert.ToDateTime(login).ToShortDateString()),
                                        new SqlParameter("@user", dr["LA_User_Name"].ToString())
                                   };

                            SQlQuery = "select D_Reason from RTM_DelayedLogInOff where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, D_Date))) =@login and D_UserName=@user";
                            dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters1);
                            if (dt.Rows.Count > 0)
                            {
                                dr1["Login Delay Reason"] = dt.Rows[0]["D_Reason"].ToString();
                            }
                            //reader = objDB.ParameterizedSelectReader(SQlQuery, CommandType.Text, parameters1);
                            //while (reader.Read())
                            //{
                            //    dr1["Login Delay Reason"] = reader["D_Reason"].ToString();
                            //}
                            //reader.Close();
                            //con.Close();
                        }
                        //Math.Round(totalWorkHours, 2, MidpointRounding.AwayFromZero); 
                        dr1["Last Activity"] = logout;
                        if (TimeSpan.Parse(schLogoff) > TimeSpan.Parse(logoutTime))
                        {
                            dr1["Early Logoff"] = TimeSpan.Parse(schLogoff).Subtract(TimeSpan.Parse(logoutTime));
                        }

                        dr1["Total Office Hours"] = diff;


                        dtResult.Rows.Add(dr1);
                    }
                }

                }
                catch (Exception)
                {

                    //throw;
                }
            }

            if (dtResult.Rows.Count > 0)
            {
                gvEarlyLate.DataSource = dtResult;
                gvEarlyLate.DataBind();
            }
            else
            {
                gvEarlyLate.DataSource = null;
                gvEarlyLate.DataBind();
            }
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (DateTime.TryParse(txtFrom.Text, out temp))
        { }
        else
        {
            lblError.Text = "Please Select valid from Date";
            lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }

        if (DateTime.TryParse(txtTo.Text, out temp))
        { }
        else
        {
            lblError.Text = "Please Select Valid To Date";
            lblError.ForeColor = System.Drawing.Color.Red;
            return;
        }
        DisplayReport();

        string attachment = "attachment; filename=RTM Late Login Report From " + Convert.ToDateTime(txtFrom.Text).ToString("MM-dd-yyyy") + " To " + Convert.ToDateTime(txtTo.Text).ToString("MM-dd-yyyy") + ".xls";
        Response.ClearContent();
        Response.AddHeader("content-disposition", attachment);
        Response.ContentType = "application/vnd.ms-excel";
        string tab = "";
        foreach (DataColumn dc in dtResult.Columns)
        {
            Response.Write(tab + dc.ColumnName);
            tab = "\t";
        }
        Response.Write("\n");
        int i;
        foreach (DataRow dr in dtResult.Rows)
        {
            tab = "";
            checked
            {
                for (i = 0; i < dtResult.Columns.Count; i++)
                {
                    Response.Write(tab + dr[i].ToString());
                    tab = "\t";
                }
            }
          
            Response.Write("\n");
        }
        Response.End();
    }
}