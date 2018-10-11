using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using BAL;
using System.Text.RegularExpressions;
using System.Configuration;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.Services;



public partial class ManageUsers : System.Web.UI.Page
{
    clsRealTimeReports objReal = new clsRealTimeReports();
    clsAccount objAccount = new clsAccount();
    DataTable dt = new DataTable();
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

            btnViewTeam.Visible = false;
            //LoadTeams();
        }
    }



    [WebMethod(EnableSession = true)]
    public static string[] GetTeams(string prefix)
    {
        string team = HttpContext.Current.Session["team"].ToString();
        List<string> Teams = new List<string>();
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = ConfigurationManager.AppSettings["conString"].ToString();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "SELECT * From RTM_Team_List where T_TeamName LIKE '%'+@SearchText+'%' and T_Active = 1 order by T_TeamName";
                cmd.Parameters.AddWithValue("@SearchText", prefix);
                cmd.Connection = conn;
                conn.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        Teams.Add(string.Format("{0}/{1}", sdr["T_TeamName"], sdr["T_ID"]));
                    }
                }
                conn.Close();
            }
        }
        return Teams.ToArray();
    }


    protected bool CheckDate(String date)
    {
        try
        {
            DateTime dt = DateTime.Parse(date);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (Convert.ToInt32(hfTeamId.Value) == 0)
        {
            lblError.Text = "Select Team";
            lblError.ForeColor = Color.Red;
            return;
        }
        if (string.IsNullOrWhiteSpace(txtEmpId.Text))
        {
            lblError.Text = "Enter valid Employee Id";
            lblError.ForeColor = Color.Red;
            return;
        }

        if (txtEmpId.Text.Length > 15)
        {
            lblError.Text = "Enter valid Employee Id, Length exceeds 15 digits";
            lblError.ForeColor = Color.Red;
            return;
        }

        if (string.IsNullOrWhiteSpace(txtname.Text)) 
        {
            lblError.Text = "Enter Name";
            lblError.ForeColor = Color.Red;
            return;
        }
        if (string.IsNullOrWhiteSpace(txtSystemName.Text))
        {
            lblError.Text = "Enter valid System User Name";
            lblError.ForeColor = Color.Red;
            return;
        }
        bool validEmail = ValidateEmail(txtEmail.Text);
        if (validEmail == false)
        {
            lblError.Text = "Enter valid email";
            lblError.ForeColor = Color.Red;
            return;
        }
        else
        {
            lblError.Text = string.Empty;
        }

        if (ValidateEmail(txtMgrEmailId.Text) == false)
        {
            lblError.Text = "Enter valid email";
            lblError.ForeColor = Color.Red;
            return;
        }
        else
        {
            lblError.Text = string.Empty;
        }

        if (string.IsNullOrWhiteSpace(txtDOJ.Text))
        {
            lblError.Text = "Enter valid Date Of Joining";
            lblError.ForeColor = Color.Red;
            return;
        }

        if (string.IsNullOrWhiteSpace(txtMgrId.Text))
        {
            lblError.Text = "Enter valid Manager Employee Id";
            lblError.ForeColor = Color.Red;
            return;
        }


        if (txtMgrId.Text.Length > 15)
        {
            lblError.Text = "Enter valid Manager Employee Id, Length exceeds 6 digits";
            lblError.ForeColor = Color.Red;
            return;
        }

        if (ddlGender.SelectedIndex == 0)
        {
            lblError.Text = "Please select Gender";
            lblError.ForeColor = Color.Red;
            return;
        }

        if (ddlHourly.SelectedIndex == 0)
        {
            lblError.Text = "Please select Hourly type";
            lblError.ForeColor = Color.Red;
            return;
        }


        if (CheckDate(txtDOJ.Text) == false)
        {
            lblError.Text = "Sorry, Date Of Joining is invalid";
            lblError.ForeColor = Color.Red;
            return;
        }

        if (ddlType.SelectedIndex == 0)
        {
            lblError.Text = "Please select Employee type";
            lblError.ForeColor = Color.Red;
            return;
        }

        bool result = true;

        string Hourly = "0";
        if (ddlHourly.SelectedValue == "No")
        {
         Hourly = "0";
        }

        else if(ddlHourly.SelectedValue == "Yes")
        {
         Hourly = "1";
        }

        if (btnAdd.Text == "Add Record")
        {
            bool checkEmpId = objAccount.CheckEmpId(txtEmpId.Text.Trim());
            if (checkEmpId == true)
            {
                lblError.Text = "Employee Id already exist";
                lblError.ForeColor = Color.Red;
                return;
            }
            bool checkUser = objAccount.CheckUserName(txtname.Text.Trim());
            if (checkUser == true)
            {
                lblError.Text = "Employee name already exist";
                lblError.ForeColor = Color.Red;
                return;
            }
            result = objAccount.AddNewUser(hfTeamId.Value, txtEmpId.Text.Trim(), txtname.Text.Trim(), txtSystemName.Text.Trim(), ddlGender.SelectedItem.Text, txtEmail.Text.Trim(), Session["UID"].ToString(), txtMgrId.Text.Trim(), txtMgrEmailId.Text.Trim(), Hourly, txtDOJ.Text.Trim(), ddlType.SelectedValue, txtEmpNo.Text.Trim(), txtPayrollId.Text.Trim());
            Response.Write("<script>alert('User added Successfully')</script>");
        
        }
        else
        {
            btnAdd.Text = "Add Record";
            txtname.ReadOnly = false;
            txtEmpId.ReadOnly = false;
            result = objAccount.UpdateUser(txtEmpId.Text.Trim(), txtSystemName.Text.Trim(), txtMgrId.Text.Trim(), txtMgrEmailId.Text.Trim(), hfTeamId.Value, Session["UID"].ToString(), ddlGender.SelectedValue, Hourly, txtDOJ.Text, ddlType.SelectedValue, txtEmpNo.Text.Trim(), txtPayrollId.Text.Trim());
            Response.Write("<script>alert('User updated Successfully')</script>");   
        }


        if (result == true)
        {
            result = objAccount.AddAccessLevel(txtEmpId.Text, Session["UID"].ToString());
            if (result == true)
            {

                lblError.ForeColor = Color.Blue;
                txtEmail.Text = string.Empty;
                txtEmpId.Text = string.Empty;
                txtname.Text = string.Empty;
                txtSystemName.Text = string.Empty;
            }
            else
            {
                Response.Write("<script>alert('User added But failed to provide access please contact RTM Support')</script>");
                lblError.ForeColor = Color.Red;
            }
        }
        else
        {
            lblError.Text = "User not added please try again or contact RTM Support";
            lblError.ForeColor = Color.Red;
        }

        resetFields();
        BindUsersGrid();
    }

    protected void txtTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (Convert.ToInt16(hfTeamId.Value) == 0)
        //{
        //    gvUsers.DataSource = null;
        //    gvUsers.DataBind();
        //}
        //else
        //{
        //    BindUsersGrid();
        //}
    }

    //private void LoadTeams()
    //{
    //    dt = new DataTable();
    //    dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());
    //    if (dt.Rows.Count > 0)
    //    {
    //        ddlTeam.DataSource = dt;
    //        ddlTeam.DataTextField = "T_TeamName";
    //        ddlTeam.DataValueField = "T_ID";
    //        ddlTeam.DataBind();
    //        ddlTeam.Items.Insert(0, "--Select Team--");
    //        ddlTeam.SelectedIndex = 0;
    //    }
    //}

    private bool ValidateEmail(string emailId)
    {
        
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(emailId);
        if (match.Success)
            return true;
        else
            return false;
    }

    private void BindUsersGrid()
    {
        dt = objAccount.GetUsers(hfTeamId.Value);
        if (dt.Rows.Count > 0)
        {
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }
        else
        {
            gvUsers.DataSource = null;
            gvUsers.DataBind();
        }
    }

    protected void txtTeam_TextChanged(object sender, System.EventArgs e)
    {     
        //if (Convert.ToInt16(hfTeamId.Value) == 0)
        //{
        //    gvUsers.DataSource = null;
        //    gvUsers.DataBind();
        //}
        //else
        //{
        //    BindUsersGrid();
        //}
        btnViewTeam.Visible = true;
}

    public int userId = 0;


    protected void btnReset_Click(object sender, System.EventArgs e)
    {
        resetFields();
        gvUsers.Visible = false;
    }

    private void resetFields()
    {
        txtTeam.Text = "";
        txtEmpId.Text = "";
        txtDOJ.Text = "";
        txtEmail.Text = "";
        txtname.Text = "";
        txtMgrEmailId.Text = "";
        txtMgrId.Text = "";
        txtSystemName.Text = "";
        txtEmpNo.Text = "";
        txtPayrollId.Text = "";
        ddlGender.SelectedIndex = 0;
        ddlHourly.SelectedIndex = 0;
        ddlType.SelectedIndex = 0;
        btnAdd.Text = "Add Record";
        btnViewTeam.Visible = false;
     
    }


    protected void gvUsers_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName == "EditRow")
        {
            btnAdd.Text = "Update";
            txtname.ReadOnly = true;
            txtEmpId.ReadOnly = true;
           txtEmpId.Text = gvUsers.Rows[index].Cells[0].Text; //consider you use bound field and the column you want to show its value is the first column.
           txtname.Text = gvUsers.Rows[index].Cells[1].Text;
           txtSystemName.Text = gvUsers.Rows[index].Cells[2].Text;
           ddlGender.Text = gvUsers.Rows[index].Cells[3].Text;
           txtEmail.Text = gvUsers.Rows[index].Cells[4].Text;
           if (gvUsers.Rows[index].Cells[5].Text == "&nbsp;")
           {
               txtMgrId.Text = "";
           }
           else
           {
               txtMgrId.Text = gvUsers.Rows[index].Cells[5].Text;
           }

           if (gvUsers.Rows[index].Cells[6].Text == "&nbsp;")
           {
               txtMgrEmailId.Text = "";
           }
           else
           {
               txtMgrEmailId.Text = gvUsers.Rows[index].Cells[6].Text;
           }
       
           ddlHourly.Text = gvUsers.Rows[index].Cells[7].Text;

           if (gvUsers.Rows[index].Cells[8].Text == "&nbsp;")
           {
               txtDOJ.Text = "";
           }
           else
           {
               txtDOJ.Text = gvUsers.Rows[index].Cells[8].Text;
           }

           ddlType.Text = gvUsers.Rows[index].Cells[9].Text;

           if (gvUsers.Rows[index].Cells[10].Text == "&nbsp;")
           {
               txtEmpNo.Text = "";
           }
           else
           {
               txtEmpNo.Text = gvUsers.Rows[index].Cells[10].Text;
           }


           if (gvUsers.Rows[index].Cells[11].Text == "&nbsp;")
           {
               txtPayrollId.Text = "";
           }
           else
           {
               txtPayrollId.Text = gvUsers.Rows[index].Cells[11].Text; 
           }

        }

   

    }
    protected void btnViewTeam_Click(object sender, System.EventArgs e)
    {
        gvUsers.Visible = true;
        if (Convert.ToInt16(hfTeamId.Value) == 0)
        {
            gvUsers.DataSource = null;
            gvUsers.DataBind();
        }
        else
        {
            BindUsersGrid();
        }
    }
}