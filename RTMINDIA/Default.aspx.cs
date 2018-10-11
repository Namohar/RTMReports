using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Security.Principal;
using BAL;
using System.Web.Security;

public partial class _Default : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    DataTable dt = new DataTable();
    clsUserLogin objLogin = new clsUserLogin();
    clsAuditTrail objAuditTrail = new clsAuditTrail();


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            System.Security.Principal.IPrincipal UserNew;
            UserNew = System.Web.HttpContext.Current.User;
           // System.Security.Principal.WindowsPrincipal user;
            string _userNew = UserNew.Identity.Name;
            //WindowsIdentity winId = WindowsIdentity.GetCurrent();
            //WindowsPrincipal winPrincipal = new WindowsPrincipal(winId);
            //user = winPrincipal.Identity.Name;
           // user = new WindowsPrincipal(this.Request.LogonUserIdentity);
            lblUserName.Text = _userNew; // user.Identity.Name;
            //lblUserName.Text = @"CORP\piyali.bhattacharjee";
            if (!IsPostBack)
            {
                
                //string userHost = Request.ServerVariables["REMOTE_ADDR"];
                //TimeZone localZone = TimeZone.CurrentTimeZone;
                //string timezone = localZone.StandardName;
                //DateTime currentDate = DateTime.Now;
                //TimeZoneInfo time = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                
            }
        }
    }
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        System.Security.Principal.WindowsPrincipal user;
        user = new WindowsPrincipal(this.Request.LogonUserIdentity);
        dt = new DataTable();
        dt = objLogin.getUser(user.Identity.Name);
        //dt = objLogin.getUser(@"CORP\piyali.bhattacharjee");
        if (dt.Rows.Count > 0)
        {
            string team = dt.Rows[0]["UL_Team_Id"].ToString();
            string accessLevel = dt.Rows[0]["AL_AccessLevel"].ToString();
            Session["UID"] = dt.Rows[0]["UL_ID"].ToString();
            Session["team"] = team;
            Session["backupTeam"] = team;
            Session["access"] = accessLevel;
            Session["EMSTeam"] = dt.Rows[0]["UL_EMS_Team"].ToString();
            Session["user"] = lblUserName.Text;
            Session["username"] = dt.Rows[0]["UL_User_Name"].ToString();
            Session["Location"] = dt.Rows[0]["T_Location"].ToString();
            Session["preference"] = dt.Rows[0]["T_Preference"].ToString();
            Session["empId"] = dt.Rows[0]["UL_Employee_Id"].ToString();
            Session["hourly"] = dt.Rows[0]["UL_Hourly"].ToString();
            Session["doj"] = dt.Rows[0]["UL_DOJ"].ToString();
            //if preference = 1 then login to logout else 12 - 12
            if (Session["hourly"].ToString() == "1")
            {                
                //FormsAuthentication.RedirectFromLoginPage(lblUserName.Text, true);
                Response.Redirect("ClockInClockOut");
            }
            else if (accessLevel == "1")
            {
                getLocations();
                mpePopUp.Show();
            }
            else
            {
                
                //Response.Redirect("~/DashBoard.aspx");
                Response.Redirect("WeeklyView");
            }

            try
            {
                objAuditTrail.AddLogs("Login", "", Session["username"].ToString(), DateTime.Now);
            }
            catch(Exception ex)
            {

            }
        }
        else
        {
            lblError.Text = "You are not authorized to access reports";
            lblError.ForeColor = System.Drawing.Color.Red;
        }
    }

    private void getLocations()
    {
        dt = new DataTable();
        dt = objLogin.getLocations();

        if (dt.Rows.Count > 0)
        {
            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "T_Location";
            ddlLocation.DataValueField = "T_Location";
            ddlLocation.DataBind();
        }
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        Session["Location"] = ddlLocation.SelectedItem.Text;
        //FormsAuthentication.RedirectFromLoginPage(lblUserName.Text, true);
        Response.Redirect("admindashboard");
        //Response.Redirect("~/RecordsWeeklyView.aspx");
    }

    protected void btnNo_Click(object sender, EventArgs e)
    {

    }  
}