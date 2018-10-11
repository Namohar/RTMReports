using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
using System.Text.RegularExpressions;
using BAL;
using System.IO;
using System.Security.Cryptography;
using DAL;

public partial class AddRecord : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlCommand cmd;
    SqlDataAdapter da;
    DataSet ds = new DataSet();
    DataTable dt = new DataTable();
    clsTeam objTeam = new clsTeam();
    clsRealTimeReports objReal = new clsRealTimeReports();

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
            lblid.Text = objTeam.Decrypt(HttpUtility.UrlDecode(Request.QueryString["TID"]));
            //LoadEmp();
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

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        System.Threading.Thread.Sleep(50);
        try
        {
            Regex regex = new Regex("^(?:(?:([01]?\\d|2[0-3]):)?([0-5]?\\d):)?([0-5]?\\d)$");
            Regex newRegex = new Regex("^([2][0-3]|[0-1]?[0-9])([.:][0-9]?[0-9])?([:][0-9]?[0-9])?$");
            Regex newRegex2 = new Regex("^([2][0-3]|[0-1]?[0-9])([.:][0-5]?[0-9])?([:][0-5]?[0-9])?$");
            if (!string.IsNullOrEmpty(txtDuration.Text))
            {
                if (txtDuration.Text.Contains('.'))
                {
                    int dotCount = txtDuration.Text.Count(x => x == '.');
                    if (dotCount == 1)
                    {
                        decimal val = Math.Round(Convert.ToDecimal(txtDuration.Text), 2, MidpointRounding.AwayFromZero);
                        txtDuration.Text = val.ToString();
                    }
                }
            }
            if (ddlNewClient.SelectedIndex == 0)
            {
                lblError.Text = "Please Select Client";
                lblError.ForeColor = Color.Red;

            }
            else if (ddlNewTask.SelectedIndex == 0)
            {
                lblError.Text = "Please Select Task";
                lblError.ForeColor = Color.Red;

            }
            else if (ddlNewSubTask.SelectedIndex == 0)
            {
                lblError.Text = "Please Select Sub Task";
                lblError.ForeColor = Color.Red;

            }           
            else
            {
                string addDuration = "00:00:00";
                if (txtDuration.Text.Contains(':'))
                {
                    if (newRegex2.IsMatch(txtDuration.Text) == false)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                        return;
                    }
                }
                else if (newRegex.IsMatch(txtDuration.Text) == false)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "duration", "alert('Enter valid duration')", true);
                    return;
                }
                if (txtDuration.Text.Contains(':') == false)
                {

                    decimal time = decimal.Parse(txtDuration.Text.Trim());
                    var hours = time.ToString().Split('.')[0];
                    var minutes = ((time * 60) % 60).ToString().Split('.')[0];
                    var seconds = ((time * 3600) % 60).ToString().Split('.')[0];

                    addDuration = hours + ":" + minutes + ":" + seconds;

                }
                else
                {
                    int charCount = txtDuration.Text.Count(x => x == ':');
                    if (charCount == 1)
                    {
                        addDuration = txtDuration.Text + ":00";
                    }
                    else
                    {
                        addDuration = txtDuration.Text;
                    }
                }
                using (cmd = new SqlCommand())
                {
                    string comm = txtComments.Text + "-Manual addition";
                    //Namohar code changes on 10-Aug-2016.
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@id", lblid.Text),
                        new SqlParameter("@empid", lblEmpID.Text),
                        new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
                        new SqlParameter("@newClient", ddlNewClient.SelectedValue),                        
                        new SqlParameter("@newtask", ddlNewTask.SelectedValue),
                        new SqlParameter("@subTask", ddlNewSubTask.SelectedValue),
                        new SqlParameter("@comments", txtComments.Text ),
                        new SqlParameter("@duration", addDuration),
                        new SqlParameter("@date", Convert.ToDateTime(txtDate.Text)),
                        new SqlParameter("@date1", Convert.ToDateTime(txtDate.Text)),
                        new SqlParameter("@system", "P")
                    };


                    SQlQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn, R_Status, R_System) values (@id, @empid, @emp, @newClient, @newtask, @subTask, @comments,@duration, @date,@date1, 'Completed', @system)";
                    result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);

                }
                ddlNewClient.SelectedIndex = 0;
                ddlNewTask.SelectedIndex = 0;
                ddlNewSubTask.SelectedIndex = 0;
                txtDuration.Text = string.Empty;
                txtComments.Text = string.Empty;
                addDuration = "0";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Saved Successfully')", true);
                DisplayRecords();
            }
        }
        catch (System.Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }

    }

    protected void ddlNewTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlNewTask.SelectedIndex != 0)
        {
            LoadSubTasks();
        }
    }

    private void LoadEmp()
    {
        dt = new DataTable();

        dt = objTeam.LoadEmp(Session["access"].ToString(), lblid.Text, Session["user"].ToString());
               
        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();
        ddlEmp.Items.Insert(0, "--Select--");
        ddlEmp.SelectedIndex = 0;

    }

    protected void ddlEmp_SelectedIndexChanged(object sender, System.EventArgs e)
    {
       

        if (ddlEmp.SelectedIndex != 0)
        {
            //Namohar code changes on 10-Aug-2016.
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@emp",  ddlEmp.SelectedItem.Text),
            };
            dt = new DataTable();
            
            SQlQuery = "select R_Employee_Id from RTM_Records where R_User_Name =@emp";
            dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);


            if (dt.Rows.Count > 0)
            {
                lblEmpID.Text = dt.Rows[0][0].ToString();
            }
            //Namohar code changes on 10-Aug-2016.
            SqlParameter[] parameters1 = new SqlParameter[]
            {
                new SqlParameter("@id", lblid.Text),
            };
            dt = new DataTable();
            
            SQlQuery = "select * from RTM_Client_List where CL_TeamId=@id and CL_Status=1 order by CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters1);

            if (dt.Rows.Count > 0)
            {
                ddlNewClient.DataSource = dt;
                ddlNewClient.DataTextField = "CL_ClientName";
                ddlNewClient.DataValueField = "CL_ID";
                ddlNewClient.DataBind();
                ddlNewClient.Items.Insert(0, "--Select--");
                ddlNewClient.SelectedIndex = 0;
            }

            //Namohar code changes on 10-Aug-2016.
            SqlParameter[] parameters2 = new SqlParameter[]
            {
                new SqlParameter("@id", lblid.Text),
            };
            dt = new DataTable();
           
            SQlQuery = "select * from RTM_Task_List where TL_TeamId = @id and TL_Status =1 order By TL_Task";
            dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters2);

            if (dt.Rows.Count > 0)
            {
                ddlNewTask.DataSource = dt;
                ddlNewTask.DataTextField = "TL_Task";
                ddlNewTask.DataValueField = "TL_ID";
                ddlNewTask.DataBind();
                ddlNewTask.Items.Insert(0, "--Select--");
                ddlNewTask.SelectedIndex = 0;
            }

        }
    }

    private void LoadSubTasks()
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@newTask", ddlNewTask.SelectedValue),
        };
        dt = new DataTable();
        
        SQlQuery = "select * from RTM_SubTask_List where STL_Task_Id= @newTask";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        if (dt.Rows.Count > 0)
        {
            ddlNewSubTask.DataSource = dt;
            ddlNewSubTask.DataTextField = "STL_SubTask";
            ddlNewSubTask.DataValueField = "STL_ID";
            ddlNewSubTask.DataBind();
            ddlNewSubTask.Items.Insert(0, "--Select--");
            ddlNewSubTask.SelectedIndex = 0;
        }
    }

    private void DisplayRecords()
    {

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@date",  Convert.ToDateTime(txtDate.Text).ToShortDateString()),
            new SqlParameter("@emp",  ddlEmp.SelectedItem.Text),
        };
        dt = new DataTable();
       
        SQlQuery = "select R_ID, R_User_Name, CL_ClientName, TL_Task, STL_SubTask, R_Duration, R_Start_Date_Time, R_Comments from RTM_Records, RTM_Client_List, RTM_Task_List, RTM_SubTask_List where R_Client = CL_ID and R_Task = TL_ID and R_SubTask = STL_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) =@date and R_User_Name=@emp order By R_ID, R_Start_Date_Time";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        gvRecords.DataSource = dt;
        gvRecords.DataBind();
    }

    protected void txtDate_TextChanged(object sender, System.EventArgs e)
    {
        if (txtDate.Text.Length > 0)
        {
            DisplayRecords();
        }
    }

    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex > 0)
        {
            lblid.Text = ddlTeam.SelectedValue;
            LoadEmp();
        }
        else
        {
            lblid.Text = "0";
        }
    }
}