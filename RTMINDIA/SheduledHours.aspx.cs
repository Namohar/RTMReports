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

public partial class SheduledHours : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    SqlDataAdapter da;
    DataSet ds = new DataSet();
    clsRealTimeReports objReal = new clsRealTimeReports();

    SqlDBHelper objDB = new SqlDBHelper();
    string SQlQuery;

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
            BindTeamList();
        }
    }

    private void BindGrid()
    {
        //Namohar code changes on 10-Aug-2016.
        SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@team", ddlTeam.SelectedValue),
           };

        if (ds.Tables.Contains("emp"))
        {
            ds.Tables.Remove(ds.Tables["emp"]);
        }
        SQlQuery = "SELECT UL_Employee_Id, UL_User_Name, CONVERT(VARCHAR(8),UL_SCH_Login,108) AS UL_SCH_Login, CONVERT(VARCHAR(8),UL_SCH_Logout,108) AS UL_SCH_Logout FROM RTM_User_List WHERE UL_Team_Id = @team and UL_User_Status =1 order by UL_User_Name";
        
        ds = objDB.DSExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters, "emp");
        gvEmployee.DataSource = ds.Tables["emp"];
        gvEmployee.DataBind();
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

    protected void gvEmployee_RowEditing(object sender, GridViewEditEventArgs e)
    {
        lblError.Text = "";
        gvEmployee.EditIndex = e.NewEditIndex;
        BindGrid();
    }

    protected void gvEmployee_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            bool result;
            string empid = ((Label)gvEmployee.Rows[e.RowIndex].FindControl("lblEmpId")).Text;

            DateTime login = Convert.ToDateTime(((TextBox)gvEmployee.Rows[e.RowIndex].FindControl("txtSCHLogin")).Text);

            DateTime logout = Convert.ToDateTime(((TextBox)gvEmployee.Rows[e.RowIndex].FindControl("txtSCHLogout")).Text);

            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@login", login),
               new SqlParameter("@logout", logout),
               new SqlParameter("@empid", empid)
           };

            SQlQuery = "UPDATE RTM_User_List SET UL_SCH_Login=@login, UL_SCH_Logout=@logout where UL_Employee_Id =@empid";
            result = objDB.ExecuteNonQuery(SQlQuery, CommandType.Text, parameters);
           
            lblError.Text = "Updated Succesfully";
            lblError.ForeColor = System.Drawing.Color.Blue;
            //con.Close();
            gvEmployee.EditIndex = -1;
            BindGrid();
        }
        catch (Exception)
        {
            lblError.Text = "Something went wrong..Please try again";
            lblError.ForeColor = System.Drawing.Color.Red;
        }

    }

    protected void gvEmployee_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvEmployee.EditIndex = -1;
        BindGrid();
    }

    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlTeam.SelectedIndex != 0)
        {
            BindGrid();
        }
    }
}