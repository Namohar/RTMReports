using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using Obout.Grid;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Collections;

public partial class AddTeam : System.Web.UI.Page
{
    clsUserLogin objLogin = new clsUserLogin();
    clsTeam objTeam = new clsTeam();
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
            GetLocations();
        }
    }

    private void GetLocations()
    {
        dt = new DataTable();
        dt = objLogin.getLocationsforMSPReport();
        ddlLocation.DataSource = dt;
        ddlLocation.DataTextField = "T_Location";
        ddlLocation.DataValueField = "T_Location";
        ddlLocation.DataBind();

        ddlLocation.Items.Insert(0, "--Select Location--");
        ddlLocation.SelectedIndex = 0;
    }

    private void GetTeams(string location)
    {
        dt = new DataTable();
        dt = objTeam.GetTeams(location);

        grTeams.DataSource = dt;
        grTeams.DataBind();
    }
    protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlLocation.SelectedIndex > 0)
        {
            GetTeams(ddlLocation.SelectedValue);
            lnkExport.Visible = true;
        }
        else
        {
            grTeams.DataSource = null;
            grTeams.DataBind();
            lnkExport.Visible = false;
        }
    }

    protected void RebindGrid(object sender, EventArgs e)
    {
        if (ddlLocation.SelectedIndex > 0)
        {
            GetTeams(ddlLocation.SelectedValue);
        }
        else
        {
            grTeams.DataSource = null;
            grTeams.DataBind();
        }
    }

    protected void UpdateRecord(object sender, GridRecordEventArgs e)
    {
        string status = e.Record["T_Active"].ToString() == "yes" ? "1" : "0";
        string id = e.Record["T_ID"].ToString();

        bool result = objTeam.UpdateTeamStatus(status, Convert.ToInt32(id));
    }

    [WebMethod]
    public static List<string> GetAutoCompleteUsers(string username)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString()))
        {
            using (SqlCommand cmd = new SqlCommand("select UL_ID,UL_User_Name from RTM_User_List WITH (NOLOCK) left join RTM_Access_Level WITH (NOLOCK) on UL_Employee_Id = AL_EmployeeId where UL_User_Name LIKE '%'+@SearchText+'%' and UL_User_Status=1 and AL_AccessLevel != 4 Order by UL_User_Name", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", username);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}!{1}", dr["UL_User_Name"], dr["UL_ID"]));
                }
                return result;
            }
        }
    }

    [WebMethod]
    public static List<string> GetAutoCompleteTeams(string teamname)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString()))
        {
            using (SqlCommand cmd = new SqlCommand("select T_ID, T_TeamName from RTM_Team_List WITH (NOLOCK) where T_TeamName LIKE '%'+@SearchText+'%' and T_Active=1", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", teamname);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}!{1}", dr["T_TeamName"], dr["T_ID"]));
                }
                return result;
            }
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        SaveTeam();
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        txtManager.Text = string.Empty;
        txtTeamName.Text = string.Empty;
        //ddlLocation.SelectedIndex = 0;
        btnSave.Text = "Save";
        lblTid.Value = "0";

        //grTeams.DataSource = null;
        //grTeams.DataBind();
    }
    

    private void SaveTeam()
    {
        if (string.IsNullOrEmpty(txtTeamName.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "team", "alert('Please enter team name')", true);
            return;
        }
        else if (string.IsNullOrWhiteSpace(txtTeamName.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "teamname", "alert('Please enter team name')", true);
            return;
        }

        if (ddlLocation.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select location", "alert('Please select location')", true);
            return;
        }

        if (string.IsNullOrEmpty(txtManager.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "manager", "alert('Please enter manager name')", true);
            return;
        }
        else if(string.IsNullOrWhiteSpace(txtManager.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "managername", "alert('Please enter manager name')", true);
            return;
        }
        else if (hfMid.Value == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "managername", "alert('Please select valid manager name from autosuggested list.')", true);
            return;
        }

        dt = objTeam.CheckTeam(txtTeamName.Text, ddlLocation.SelectedValue, lblTid.Value);

        if (dt.Rows.Count > 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "check team", "alert('Team name already exist')", true);
            return;
        }

        dt = new DataTable();

        dt = objTeam.CheckManager(txtManager.Text);

        if (dt.Rows.Count <= 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "check manager", "alert('Manager not exist in RTM.')", true);
            return;
        }

        bool result = false;
        string status = "0";
        if (chkActiveStatus.Checked)
        {
            status = "1";
        }
        else
        {
            status = "0";
        }
        if (lblTid.Value == "0")
        {
            result = objTeam.InsertTeam(txtTeamName.Text, ddlLocation.SelectedValue, txtManager.Text, status, Session["UID"].ToString());
        }
        else
        {
            result = objTeam.UpdateTeam(lblTid.Value, txtTeamName.Text, ddlLocation.SelectedValue, txtManager.Text, status, Session["UID"].ToString());
        }
        

        if (result == true)
        {
            if (lblTid.Value == "0")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Team created successfully')", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Team updated successfully')", true);
            }
            
            GetTeams(ddlLocation.SelectedValue);
            txtManager.Text = string.Empty;
            txtTeamName.Text = string.Empty;
            lblTid.Value = "0";
            btnSave.Text = "Save";
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Failure", "alert('Failed to create team. Please try again or contact RTM team')", true);
        }
    }
    protected void lnkExport_Click(object sender, EventArgs e)
    {
        dt = new DataTable();
        dt = objTeam.GetTeams(ddlLocation.SelectedValue);
        if (dt.Rows.Count > 0)
        {
            dt.Columns.RemoveAt(0);
            //grTeams.PageSize = -1;
            //grTeams.DataBind();
            //ExportGridToCSV();
            ExportToCSV(dt, "TeamList - "+ ddlLocation.SelectedValue +".csv");
        }
    }

    private void ExportToCSV(DataTable dtResult, string reportName)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
        string colName = "";
        Response.Charset = "";
        //Response.ContentType = "text/csv";
        Response.ContentType = "application/text";
        StringBuilder sb = new StringBuilder();
        checked
        {
            for (int k = 0; k < dtResult.Columns.Count; k++)
            {

                switch (dtResult.Columns[k].ColumnName)
                {
                    case "T_ID":
                        colName = "Team Id";
                        break;

                    case "T_TeamName":
                        colName = "Team Name";
                        break;

                    case "T_Location":
                        colName = "Location";
                        break;

                    case "T_Manager":
                        colName = "Manager Name";
                        break;

                    case "T_Active":
                        colName = "Is Active";
                        break;

                    default:
                        colName = "";
                        break;
                }
                //add separator
                sb.Append(colName + ',');
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

    private void ExportGridToCSV()
    {
        Response.Clear();

        int j = 0;

        //Add headers of the csv table
        foreach (Column col in grTeams.Columns)
        {
            if (j > 0)
            {
                Response.Write(",");
            }

            Response.Write(col.HeaderText);

            j++;
        }

        //How add the data from the Grid to csv table
        for (int i = 0; i < grTeams.Rows.Count; i++)
        {
            Hashtable dataItem = grTeams.Rows[i].ToHashtable();
            j = 0;

            Response.Write("\n");

            foreach (Column col in grTeams.Columns)
            {
                if (j > 0)
                {
                    Response.Write(",");
                }

                Response.Write(dataItem[col.DataField].ToString());

                j++;
            }
        }

        // Send the data and the appropriate headers to the browser        
        Response.AddHeader("content-disposition", "attachment;filename=TeamList.csv");
        Response.ContentType = "text/csv";
        Response.End();
    }
}