using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;

public partial class ProfileView : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsProfile objProfile = new clsProfile();
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
            GetUserDetails();
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        UpdateDetails();
    }

    private void GetUserDetails()
    {
        dt = objProfile.getUserDetails(Session["user"].ToString());
        if (dt.Rows.Count > 0)
        {
            lblName.Text = dt.Rows[0]["UL_User_Name"].ToString();
            lblTeam.Text = dt.Rows[0]["T_TeamName"].ToString();
            lblEmpId.Text = dt.Rows[0]["UL_Employee_Id"].ToString();
            lblUserName.Text = dt.Rows[0]["UL_System_User_Name"].ToString();
            lblEmail.Text = dt.Rows[0]["UL_EmailId"].ToString();

            txtName.Text = dt.Rows[0]["UL_User_Name"].ToString();
            txtTeam.Text = dt.Rows[0]["T_TeamName"].ToString();
            txtEmployeeId.Text = dt.Rows[0]["UL_Employee_Id"].ToString();
            txtUserName.Text = dt.Rows[0]["UL_System_User_Name"].ToString();
            txtEmail.Text = dt.Rows[0]["UL_EmailId"].ToString();
            if (dt.Rows[0]["UL_Gender"].ToString() == "M")
            {
                lblGender.Text = "Male";
                rbMale.Checked = true;
            }
            else if (dt.Rows[0]["UL_Gender"].ToString() == "F")
            {
                lblGender.Text = "Female";
                rbFemale.Checked = true;
            }
            else
            {
                lblGender.Text = "";
                rbFemale.Checked = false;
                rbMale.Checked = false;
            }
        }
    }

    private void UpdateDetails()
    {
        bool result;
        string gender="";
        if (txtEmail.Text.ToLower().Contains("@tangoe.com") == false)
        {
            lblMessage.Text = "Enter valid tangoe email id";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            return;
        }
        if (rbMale.Checked == true) { gender = "M"; }
        else if (rbFemale.Checked)
        { gender = "F"; }
        else
        {
            gender = "";
        }

        result = objProfile.UpdateProfile(gender, Session["user"].ToString(), txtEmail.Text);

        if (result == true)
        {
            lblMessage.Text = "Updated Successfully";
            lblMessage.ForeColor = System.Drawing.Color.Blue;
            GetUserDetails();
        }
        else
        {
            lblMessage.Text = "Failed.. Please try again";
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
    }
}