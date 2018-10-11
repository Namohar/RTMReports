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

public partial class AddLogs : System.Web.UI.Page
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
            //lblid.Text = objTeam.Decrypt(HttpUtility.UrlDecode(Request.QueryString["TID"]));
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

    private void LoadEmp()
    {
        dt = new DataTable();
        

        dt = objTeam.LoadEmp(Session["access"].ToString(), lblid.Text, Session["user"].ToString());

        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();

    }

    protected void ddlReason_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlReason.SelectedIndex == 1)
        {
            lblCategory.Visible = true;
            ddlCategory.Visible = true;
            LoadCategory();
        }
        else if (ddlReason.SelectedIndex == 2)
        {
            lblClient.Visible = true;
            ddlClient.Visible = true;
            lblCategory.Visible = false;
            ddlCategory.Visible = false;
            lblSubCat.Visible = false;
            ddlSubCategory.Visible = false;
            LoadIdleClient();
        }
        else
        {
            lblCategory.Visible = false;
            ddlCategory.Visible = false;
            lblClient.Visible = false;
            ddlClient.Visible = false;
            lblSubCat.Visible = false;
            ddlSubCategory.Visible = false;
        }
    }

    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCategory.SelectedIndex != 0)
        {
            //lblSubCat.Visible = true;
            //ddlSubCategory.Visible = true;

            //LoadSubCategory();
        }
        else
        {
            lblSubCat.Visible = false;
            ddlSubCategory.Visible = false;
        }
    }

    private void LoadCategory()
    {
        if (ds.Tables.Contains("category"))
        {
            ds.Tables.Remove(ds.Tables["category"]);
        }
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@id", lblid.Text)
        };

        string sQuery = "select M_Category from RTM_MeetingsCategory Where M_TeamId =@id group by M_Category";
        ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "category");
        

        ddlCategory.DataSource = ds.Tables["category"];
        ddlCategory.DataTextField = "M_Category";
        ddlCategory.DataValueField = "M_Category";
        ddlCategory.DataBind();
        ddlCategory.Items.Insert(0, "--Select Category--");
        ddlCategory.SelectedIndex = 0;
    }

    private void LoadSubCategory()
    {
        if (ds.Tables.Contains("subcategory"))
        {
            ds.Tables.Remove(ds.Tables["subcategory"]);
        }

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@category",  ddlCategory.SelectedItem.Text),
           };

        SQlQuery = "select M_SubCategory from RTM_MeetingsCategory where M_Category =@category";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "subcategory");

        ddlSubCategory.DataSource = ds.Tables["subcategory"];
        ddlSubCategory.DataTextField = "M_SubCategory";
        ddlSubCategory.DataValueField = "M_SubCategory";
        ddlSubCategory.DataBind();
        ddlSubCategory.Items.Insert(0, "--Select SubCategory--");
        ddlSubCategory.SelectedIndex = 0;
    }

    private void LoadIdleClient()
    {
        if (ds.Tables.Contains("client"))
        {
            ds.Tables.Remove(ds.Tables["client"]);
        }

        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@team", Convert.ToInt32(Session["team"])),
           };


        SQlQuery = "select * from RTM_Client_List where CL_TeamId= @team and CL_Status=1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName!='Public Holiday' and CL_ClientName !='Inclement Weather' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order by CL_ClientName";
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "client");

        ddlClient.DataSource = ds.Tables["client"];
        ddlClient.DataTextField = "CL_ClientName";
        ddlClient.DataValueField = "CL_ID";
        ddlClient.DataBind();
        ddlClient.Items.Insert(0, "-Select Client-");
        ddlClient.SelectedIndex = 0;
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            Regex regex = new Regex("^(?:(?:([01]?\\d|2[0-3]):)?([0-5]?\\d):)?([0-5]?\\d)$");

            if (ddlReason.SelectedIndex == 0)
            {
                lblError.Text = "Please select Reason";
                lblError.ForeColor = Color.Red;

            }
            else if (ddlCategory.Visible == true && ddlCategory.SelectedIndex == 0)
            {
                lblError.Text = "Please select Category";
                lblError.ForeColor = Color.Red;

            }
            else if (ddlSubCategory.Visible == true && ddlSubCategory.SelectedIndex == 0)
            {
                lblError.Text = "Please select Sub Category";
                lblError.ForeColor = Color.Red;

            }
            else if (ddlClient.Visible == true && ddlClient.SelectedIndex == 0)
            {
                lblError.Text = "Please select Client";
                lblError.ForeColor = Color.Red;

            }
            else if (txtComments.Text.Length == 0 || string.IsNullOrWhiteSpace(txtComments.Text))
            {
                lblError.Text = "Please Enter Comments";
                lblError.ForeColor = Color.Red;

            }
            else if (txtDuration.Text.Length < 8)
            {
                lblError.Text = "Please enter time in HH:MM:SS format";
                lblError.ForeColor = Color.Red;

            }
            else if (regex.IsMatch(txtDuration.Text) == false)
            {
                lblError.Text = "Please enter valid time in HH:MM:SS format";
                lblError.ForeColor = Color.Red;

            }
            else
            {
                string comments;
                if (ddlSubCategory.Visible == true)
                {
                    comments = ddlCategory.SelectedItem.Text + "-" + ddlCategory.SelectedItem.Text + "-" + txtComments.Text;
                }
                else if (ddlClient.Visible == true)
                {
                    comments = ddlClient.SelectedItem.Text + "-" + txtComments.Text;
                }
                else
                {
                    comments = txtComments.Text;
                }
                //Namohar code changes on 10-Aug-2016.
                SqlParameter[] parameters = new SqlParameter[]
               {
                   new SqlParameter("@id",  lblid.Text),
                   new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
                   new SqlParameter("@date",  txtDate.Text),
                   new SqlParameter("@duration",  txtDuration.Text),
                   new SqlParameter("@reason",  ddlReason.SelectedItem.Text),
                   new SqlParameter("@date1", txtDate.Text),
                   new SqlParameter("@comments",  comments),
               };

                SQlQuery = "Insert into RTM_Log_Actions (LA_TeamId, LA_User_Name, LA_Log_Action, LA_Start_Date_Time, LA_Duration, LA_Reason, LA_CreatedOn, LA_Status, LA_Comments) values(@id, @emp, 'Locked', @date, @duration, @reason, @date1, 'Unlocked',@comments)";
                result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);
                
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Inserted Record", "alert('Saved Successfully')", true);
                DisplayLogs();

                txtDuration.Text = string.Empty;
                txtComments.Text = string.Empty;
                lblCategory.Visible = false;
                ddlCategory.Visible = false;
                lblSubCat.Visible = false;
                ddlSubCategory.Visible = false;
                lblClient.Visible = false;
                ddlClient.Visible = false;
                ddlReason.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {

            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }

    }

    private void DisplayLogs()
    {
        try
        {
            //Namohar code changes on 10-Aug-2016.
            SqlParameter[] parameters = new SqlParameter[]
               {
                   new SqlParameter("@emp",  ddlEmp.SelectedItem.Text),
                   new SqlParameter("@date", Convert.ToDateTime(txtDate.Text).ToShortDateString()),
               };
            dt = new DataTable();

            SQlQuery = "select * from RTM_Log_Actions where LA_User_Name =@emp and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date and La_log_action != 'First Activity' and La_log_action !='Last Activity' and LA_Reason != 'Peer Support' and LA_Status ='Unlocked' and LA_Duration !='' order By LA_Start_Date_Time, LA_ID";
            dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

            gvLogs.DataSource = dt;
            gvLogs.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = Color.Red;
        }
    }

    protected void txtDate_TextChanged(object sender, EventArgs e)
    {
        if (txtDate.Text.Length > 0)
        {
            DisplayLogs();
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