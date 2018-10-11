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

public partial class RTMComments : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());

    SqlDataAdapter da;
    DataSet ds = new DataSet();
    SqlCommand cmd;
    SqlDataReader dr;
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
            if (Session["access"].ToString() == "3")
            {
                divReminder.Visible = false;
                BindTeamList();
            }
            else if (Session["access"].ToString() == "2")
            {
                //divReminder.Visible = false;
                BindTeamList();
            }
            else if (Session["access"].ToString() == "1")
            {
                BindTeamList();
            }
            else if (Session["access"].ToString() == "4")
            {
                divComments.Visible = false;

            }

            LoadEmp();
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            
            int checkedValue = 0;
            string check = "";
            //Namohar code changes on 10-Aug-2016.
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@checkedValue", checkedValue),
               new SqlParameter("@team", ddlTeam.SelectedValue),
           };
            if (ddlTeam.SelectedIndex == 0)
            {
                lblError.Text = "Please select Team";
                lblError.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                if (rbNo.Checked == true)
                {
                    checkedValue = 0;
                    check = "No";
                }
                else if (rbYes.Checked == true)
                {
                    checkedValue = 1;
                    check = "Yes";
                }
                
                SQlQuery = "UPDATE RTM_Team_List SET T_Status =@checkedValue  WHERE T_ID =@team";
                result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RTM Comments Updated", "alert('RTM Comments for " + ddlTeam.SelectedItem.Text + " is set to " + check + "')", true);
                //lblError.Text = "Updated Successfully";
                //lblError.ForeColor = System.Drawing.Color.Blue;
            }

        }
        catch (Exception)
        {
            lblError.Text = "Failed.. Please try again";
            lblError.ForeColor = System.Drawing.Color.Red;
        }
    }

    private void BindTeamList()
    {
        //if (ds.Tables.Contains("Team"))
        //{
        //    ds.Tables.Remove(ds.Tables["Team"]);
        //}

        DataTable dt = new DataTable();
        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());
        

        ddlTeam.DataSource = dt; // ds.Tables["Team"];
        ddlTeam.DataTextField = "T_TeamName";
        ddlTeam.DataValueField = "T_ID";
        ddlTeam.DataBind();
        ddlTeam.Items.Insert(0, "--Select Team--");
        ddlTeam.SelectedIndex = 0;
    }

    private void CheckLastInput()
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@team", ddlTeam.SelectedValue),
           };

        DataTable dt = new DataTable();
      
        SQlQuery = "SELECT T_Status FROM RTM_Team_List WHERE T_ID =@team";
        dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);

        if (dt.Rows.Count > 0)
        {
            if (dt.Rows[0]["T_Status"].ToString() == "0")
            {
                rbNo.Checked = true;
                rbYes.Checked = false;
            }
            else
            {
                rbNo.Checked = false;
                rbYes.Checked = true;
            }
        }
        else
        {
            rbNo.Checked = true;
            rbYes.Checked = false;
        }
    }

    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex != 0)
        {
            CheckLastInput();
        }
    }

    private void LoadEmp()
    {
        DataTable dt = new DataTable();
        dt = objReal.LoadEmp(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["user"].ToString(), Session["UID"].ToString());
       
        ddlEmp.DataSource = dt;
        ddlEmp.DataValueField = "UL_ID";
        ddlEmp.DataTextField = "UL_User_Name";
        ddlEmp.DataBind();
        ddlEmp.Items.Insert(0, "--Select Employee--");
        ddlEmp.SelectedIndex = 0;

        ddlCustEmp.DataSource = dt;
        ddlCustEmp.DataValueField = "UL_ID";
        ddlCustEmp.DataTextField = "UL_User_Name";
        ddlCustEmp.DataBind();
        ddlCustEmp.Items.Insert(0, "--Select Employee--");
        ddlCustEmp.SelectedIndex = 0;

    }

    protected void ddlEmp_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Namohar code changes on 10-Aug-2016.
        

        if (ddlEmp.SelectedIndex != 0)
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
           };

            SQlQuery = "SELECT UL_Status FROM RTM_User_List where UL_User_Name=@emp ORDER BY UL_User_Name";
            dr = objDB.ParameterizedSelectReader(SQlQuery, CommandType.Text, parameters);

            if (dr.HasRows)
            {
                if (dr.Read())
                {
                    if (dr["UL_Status"].ToString() == "1")
                    {
                        rbRemNo.Checked = false;
                        rbRemYes.Checked = true;
                    }
                    else
                    {
                        rbRemNo.Checked = true;
                        rbRemYes.Checked = false;
                    }
                }
               
            }
            //con.Close();
        }
    }

    protected void btnReminder_Click(object sender, EventArgs e)
    {
        try
        {
           

            int remCheck = 0;
            string check = "";
            //Namohar code changes on 10-Aug-2016.
          
            if (ddlEmp.SelectedIndex == 0)
            {
                lblError.Text = "Please select Employee";
                lblError.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                if (rbRemYes.Checked == true)
                {
                    remCheck = 1;
                    check = "Yes";
                }
                else if (rbRemNo.Checked == true)
                {
                    remCheck = 0;
                    check = "No";
                }
               SqlParameter[] parameters = new SqlParameter[]
               {
                   new SqlParameter("@remCheck", remCheck),
                   new SqlParameter("@emp", ddlEmp.SelectedItem.Text),
               };
               
                SQlQuery = "UPDATE RTM_User_List SET UL_Status =@remCheck WHERE UL_User_Name =@emp";
                result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RTM Task Duration Reminder Updated", "alert('RTM Task Duration Reminder for user " + ddlEmp.SelectedItem.Text + " is set to " + check + "')", true);
                //lblError.Text = "Updated Successfully";
                //lblError.ForeColor = System.Drawing.Color.Blue;
            }
        }
        catch (Exception)
        {
            lblError.Text = "Failed.. Please try again";
            lblError.ForeColor = System.Drawing.Color.Red;
        }

    }

    protected void btnCustomize_Click(object sender, EventArgs e)
    {
        try
        {

            //Namohar code changes on 10-Aug-2016.
           

            if (ddlCustEmp.SelectedIndex == 0)
            {
                lblError.Text = "Please Select Employee";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
            if (ddlDuration.SelectedIndex == 0)
            {
                lblError.Text = "Please Select Duration";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@duration", ddlDuration.SelectedItem.Text),
               new SqlParameter("@custEmp", ddlCustEmp.SelectedItem.Text),
           };

            SQlQuery = "UPDATE RTM_User_List SET UL_Reminder_Duration =@duration WHERE UL_User_Name =@custEmp";
            result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "RTM Task Duration Reminder Updated", "alert('RTM Task Duration Reminder for user " + ddlCustEmp.SelectedItem.Text + " is set to " + ddlDuration.SelectedItem.Text + " Mins')", true);

        }
        catch (Exception)
        {

        }
    }
}