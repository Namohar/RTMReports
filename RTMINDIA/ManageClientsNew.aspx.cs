using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Data.OleDb;
using System.Data;
using BAL;
using System.Data.SqlClient;
using System.Web.Services;
using System.Drawing;
using System.Text;

public partial class ManageClientsNew : System.Web.UI.Page
{
    clsAccount objAccount = new clsAccount();
    clsRealTimeReports objReal = new clsRealTimeReports();
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
            try
            {
                //BindClientGrid();
                LoadTeams();
                //LoadCorePlatforms();
                if (Session["access"].ToString() == "1")
                {
                   dvUpload.Visible = false;
                }
            }
            catch (Exception)
            {
                Response.Redirect("Default.aspx");
            }            
        }
        
    }    

    [WebMethod]
    public static List<string> GetAutoCompleteData(string clientname)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString()))
        {
            using (SqlCommand cmd = new SqlCommand("select MCD_Id,MCD_ClientName_JCOne from RTM_MasterClient_Db where MCD_ClientName_JCOne LIKE '%'+@SearchText+'%' and MCD_Status=1", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", clientname);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}!{1}", dr["MCD_ClientName_JCOne"], dr["MCD_Id"]));
                }
                return result;
            }
        }
    }

    
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            int flag = 0;
            if (FileUpload1.HasFile)
            {
                DataTable dtExcelData = new DataTable();
                string excelPath = Server.MapPath("~/Files/") + Path.GetFileName(FileUpload1.PostedFile.FileName);
                FileUpload1.SaveAs(excelPath);

                string conString = string.Empty;
                string extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                if (extension == ".xls" || extension == ".xlsx")
                {

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select valid File", "alert('Please select valid excel file.')", true);
                    return;
                }
                switch (extension)
                {
                    case ".xls": //Excel 97-03
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 or higher
                        conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                        break;
                }
                conString = string.Format(conString, excelPath);

                using (OleDbConnection excel_con = new OleDbConnection(conString))
                {
                    excel_con.Open();
                    string sheet1 = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();
                    
                    using (OleDbDataAdapter oda = new OleDbDataAdapter("SELECT * FROM [" + sheet1 + "]", excel_con))
                    {
                        oda.Fill(dtExcelData);
                    }
                    excel_con.Close();

                    File.Delete(excelPath);
                }

                if (dtExcelData.Rows.Count > 0)
                {
                    foreach (DataRow drRow in dtExcelData.Rows)
                    {
                        dt = objAccount.checkMasterClents(drRow["JobcodeLevel_1"].ToString(), drRow["JobcodeLevel_0"].ToString());
                        if (dt.Rows.Count > 0)
                        {                            
                            continue;
                        }
                        else
                        {
                            bool result = objAccount.AddMasterClients(drRow["JobcodeLevel_1"].ToString(), drRow["JobcodeLevel_0"].ToString(), drRow["ProjectPLDepartment"].ToString(), drRow["CustomerCode"].ToString(), drRow["ClientName"].ToString(), Session["UID"].ToString());
                        }
                    }
                    //lblError.Text = "";
                    //lblError.ForeColor=
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Added", "alert('Clients Uploaded Successfully')", true);
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select File", "alert('Please select file to upload.')", true);
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('Please select valid file to upload')", true);
        }    
    }

    private void BindClientGrid()
    {
        dt = objAccount.GetMasterClientsNew(txtClientSearch.Text);
        if (dt.Rows.Count > 0)
        {
            gvClients.DataSource = dt;
            gvClients.DataBind();
        }
    }

    private void LoadTeams()
    {
        dt = new DataTable();
        dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());
        if (dt.Rows.Count > 0)
        {
            ddlTeam.DataSource = dt;
            ddlTeam.DataTextField = "T_TeamName";
            ddlTeam.DataValueField = "T_ID";
            ddlTeam.DataBind();

            ddlTeam.Items.Insert(0, "--Select Team--");
            ddlTeam.SelectedIndex = 0;
        }
    }

    private void LoadCorePlatforms()
    {
        dt = new DataTable();
        dt = objAccount.LoadCorePlatform();
        if (dt.Rows.Count > 0)
        {
            ddlPlatform.DataSource = dt;
            ddlPlatform.DataTextField = "CL_Product";
            ddlPlatform.DataValueField = "CL_Product";
            ddlPlatform.DataBind();

            ddlPlatform.Items.Insert(0, "--Select Platform--");
            ddlPlatform.SelectedIndex = 0;
        }
    }

    protected void lnkSelect_Click(object sender, EventArgs e)
    {
        GridViewRow grdrow = (GridViewRow)((LinkButton)sender).NamingContainer;
        Label _selectedClient = (Label)grdrow.FindControl("lblClientName");
        Label _JC0 = (Label)grdrow.FindControl("lblJC0");
        Label _ClientCode = (Label)grdrow.FindControl("lblClientCode");

        lblSelectedClient.Text = _selectedClient.Text;
        lbljl0.Text = _JC0.Text;
        lblCode.Text = _ClientCode.Text;
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(lblSelectedClient.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select client", "alert('Please select client from list')", true);
            return;
        }
        string platform;
        if (ddlPlatform.SelectedIndex == 0)
        {
            if (string.IsNullOrEmpty(txtPlatform.Text.Trim()))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "select platform", "alert('Please select platform')", true);
                return;
            }
            else
            {
                platform = txtPlatform.Text.Trim();
            }
        }
        else
        {
            platform = ddlPlatform.SelectedItem.Text;
        }

        string jobcode = lbljl0.Text.Trim() + " => " + lblSelectedClient.Text.Trim();
        string clientname = lblSelectedClient.Text.Trim();

        dt = new DataTable();
        dt = objAccount.CheckJobCode(jobcode, ddlTeam.SelectedValue);
        if (dt.Rows.Count > 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Client exist", "alert('Client already exist')", true);
            return;
        }
        bool result = objAccount.AddNewClient(ddlTeam.SelectedValue, clientname, jobcode, ddlPlatform.SelectedValue, lblCode.Text, Session["UID"].ToString());
        if (result == true)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Client added successfully please sync RTM to reflect the new client')", true);
            ddlPlatform.SelectedIndex = 0;
            //ddlTeam.SelectedIndex = 0;
            lblSelectedClient.Text = string.Empty;
            lblCode.Text = string.Empty;
            lbljl0.Text = string.Empty;
           
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "failed", "alert('Failed.. Please try again')", true);
        }
    }
    protected void btnSample_Click(object sender, EventArgs e)
    {
        //Response.Redirect(Server.MapPath("FileTemplates/Template.xlsx"));

        string FileName = "Template.xlsx"; // Its a file name displayed on downloaded file on client side.

        System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
        response.ClearContent();
        response.Clear();
        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ";");
        response.TransmitFile(Server.MapPath("~/FileTemplates/Template.xlsx"));
        response.Flush();
        response.End();
    }
    protected void btnClientSearch_Click(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(txtClientSearch.Text) || string.IsNullOrWhiteSpace(txtClientSearch.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CLient", "alert('Please enter client name to search.')", true);
            return;
        }
        //BindClientGrid();

        AddNewRow();
        
        txtClientSearch.Text = string.Empty;
    }

    protected void BindGrid()
    {
        dt = new DataTable();
        dt.Columns.Add("jobCode", typeof(string));
        dt.Columns.Add("clientName", typeof(string));
        dt.Columns.Add("clientCode", typeof(string));
        dt.Columns.Add("team", typeof(string));
        dt.Columns.Add("platform", typeof(string));
        DataRow dr = dt.NewRow();
        DataTable dtClient = new DataTable();
        dtClient = objAccount.GetMasterClientsNew(txtClientSearch.Text);

        if (dtClient.Rows.Count > 0)
        {
            dr["jobCode"] = dtClient.Rows[0]["MCD_JCZero"];
            dr["clientName"] = dtClient.Rows[0]["MCD_ClientName_JCOne"];
            dr["clientCode"] = dtClient.Rows[0]["MCD_ClientCode"];
            dr["team"] = string.Empty;
            dr["platform"] = string.Empty;
            dt.Rows.Add(dr);

            ViewState["clients"] = dt;
            grClients.DataSource = dt;
            grClients.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select valid Client name from autosuggested list.')", true);
            return;
        }
    }

    private void AddNewRow()
    {
        int rowIndex = 0;
        if (ViewState["clients"] != null)
        {
            //SetOldData();
            DataTable dtCl = (DataTable)ViewState["clients"];
            DataRow drCurrentRow = null;
            if (dtCl.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCl.Rows.Count; i++)
                {
                   // DropDownList ddlGVTeam = (DropDownList)grClients.Rows[rowIndex].Cells[3].FindControl("ddlGVTeam");
                    DropDownList ddlGVPlatform = (DropDownList)grClients.Rows[rowIndex].Cells[4].FindControl("ddlGVPlatform");

                    drCurrentRow = dtCl.NewRow();
                    DataTable dtClient = new DataTable();
                    dtClient = objAccount.GetMasterClientsNew(txtClientSearch.Text);
                    if (dtClient.Rows.Count <= 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "client", "alert('Please select valid Client name from autosuggested list.')", true);
                        return;
                    }
                    drCurrentRow["jobCode"] = dtClient.Rows[0]["MCD_JCZero"];
                    drCurrentRow["clientName"] = dtClient.Rows[0]["MCD_ClientName_JCOne"];
                    drCurrentRow["clientCode"] = dtClient.Rows[0]["MCD_ClientCode"];

                    foreach (GridViewRow row in grClients.Rows)
                    {
                        Label lblJobCode = (Label)row.FindControl("lblJC0");
                        Label lblClientName = (Label)row.FindControl("lblClientName");

                        if (lblJobCode.Text == dtClient.Rows[0]["MCD_JCZero"].ToString() && lblClientName.Text == dtClient.Rows[0]["MCD_ClientName_JCOne"].ToString())
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "CLientExist", "alert('Client already added to the grid.')", true);                            
                            return;
                        }
                    }
                    //if (ddlGVTeam.Items.Count > 0)
                    //{
                    //    dtCl.Rows[i - 1]["team"] = ddlGVTeam.SelectedItem.Text;
                    //}
                    //else
                    //{
                    //    dtCl.Rows[i - 1]["team"] = string.Empty;
                    //}

                    if (ddlGVPlatform.Items.Count > 0)
                    {
                        dtCl.Rows[i - 1]["platform"] = ddlGVPlatform.SelectedItem.Text;
                    }
                    else
                    {
                        dtCl.Rows[i - 1]["platform"] = string.Empty;
                    }

                    rowIndex++;
                }
                dtCl.Rows.Add(drCurrentRow);
                ViewState["clients"] = dtCl;
                grClients.DataSource = dtCl;
                grClients.DataBind();
                
            }
            else
            {
                BindGrid();
            }
        }
        else
        {
            BindGrid();
        }
    }

    private void AddNewRow_Updated()
    {
        DataTable dtCl = new DataTable();

    }

    private void SetOldData()
    {
        int rowIndex = 0;
        if (ViewState["clients"] != null)
        {
            DataTable dtCl = (DataTable)ViewState["clients"];
            if (dtCl.Rows.Count > 0)
            {
                for (int i = 0; i < dtCl.Rows.Count; i++)
                {
                    Label lblJobCode = (Label)grClients.Rows[rowIndex].Cells[0].FindControl("lblJC0");
                    Label lblClientName = (Label)grClients.Rows[rowIndex].Cells[1].FindControl("lblClientName");
                    Label lblClientCode = (Label)grClients.Rows[rowIndex].Cells[2].FindControl("lblClientCode");
                    //DropDownList ddlGVTeam = (DropDownList)grClients.Rows[rowIndex].Cells[3].FindControl("ddlGVTeam");
                    DropDownList ddlGVPlatform = (DropDownList)grClients.Rows[rowIndex].Cells[4].FindControl("ddlGVPlatform");

                    lblJobCode.Text = dtCl.Rows[i]["jobCode"].ToString();
                    lblClientName.Text = dtCl.Rows[i]["clientName"].ToString();
                    lblClientCode.Text = dtCl.Rows[i]["clientCode"].ToString();
                    //string team = 
                    string platform = dtCl.Rows[i]["platform"].ToString();
                    //if (!string.IsNullOrEmpty(team))
                    //{
                    //    if (ddlGVTeam.Items.FindByValue(team) != null)
                    //    {
                    //        ddlGVTeam.ClearSelection();
                    //        ddlGVTeam.Items.FindByText(team).Selected = true;
                    //    }
                    //}

                    if (!string.IsNullOrEmpty(platform))
                    {
                        if (ddlGVPlatform.Items.FindByValue(platform) != null)
                        {
                            ddlGVPlatform.ClearSelection();
                            ddlGVPlatform.Items.FindByText(platform).Selected = true;
                        }
                    }

                    rowIndex++;
                }
            }
        }
    }
    protected void grClients_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView drv = e.Row.DataItem as DataRowView;
            //DropDownList ddlGVTeam = (e.Row.FindControl("ddlGVTeam") as DropDownList);
            DropDownList ddlGVPlatform = (e.Row.FindControl("ddlGVPlatform") as DropDownList);

            //dt = new DataTable();
            //dt = objReal.LoadTeams(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["UID"].ToString());
            //if (dt.Rows.Count > 0)
            //{
            //    ddlGVTeam.DataSource = dt;
            //    ddlGVTeam.DataTextField = "T_TeamName";
            //    ddlGVTeam.DataValueField = "T_ID";
            //    ddlGVTeam.DataBind();
            //}

           string platform = drv["platform"].ToString();

           dt = new DataTable();
           dt = objAccount.LoadCorePlatform();
           if (dt.Rows.Count > 0)
           {
               ddlGVPlatform.DataSource = dt;
               ddlGVPlatform.DataTextField = "CL_Product";
               ddlGVPlatform.DataValueField = "CL_Product";
               ddlGVPlatform.DataBind();

               ddlGVPlatform.Items.Insert(0, "--Select Platform--");
               ddlGVPlatform.SelectedIndex = 0;

               if (!string.IsNullOrWhiteSpace(platform))
               {
                   if (ddlGVPlatform.Items.FindByValue(platform) != null)
                   {
                       ddlGVPlatform.ClearSelection();
                       ddlGVPlatform.Items.FindByText(platform).Selected = true;
                   }
               }               
           }          
        }
    }

    protected void grClients_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["clients"] != null)
        {
            DataTable dtCL = (DataTable)ViewState["clients"];
            DataRow drCurrentRow = null;
            int rowIndex = Convert.ToInt32(e.RowIndex);
            if (dtCL.Rows.Count > 0)
            {
                dtCL.Rows.Remove(dtCL.Rows[rowIndex]);
                drCurrentRow = dtCL.NewRow();
                ViewState["tasks"] = dtCL;
                grClients.DataSource = dtCL;
                grClients.DataBind();               

                SetOldData();
            }
        }
    }
    protected void btnSaveClients_Click(object sender, EventArgs e)
    {
        try
        {
            if (grClients.Rows.Count <= 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddClients", "alert('Please select atleast one client to add.')", true);
                txtClientSearch.Focus();
                return;
            }

            if (ddlTeam.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "selectTeam", "alert('Please select team.')", true);
                ddlTeam.Focus();
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (GridViewRow row in grClients.Rows)
            {

                DropDownList ddlGVPlatform = (DropDownList)row.FindControl("ddlGVPlatform");
                Label lblJobCode = (Label)row.FindControl("lblJC0");
                Label lblClientName = (Label)row.FindControl("lblClientName");

                string jobcode = lblJobCode.Text.Trim() + " => " + lblClientName.Text.Trim();

                if (ddlGVPlatform.SelectedIndex == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "selectPlatform", "alert('Please select platform for all the clients.')", true);
                    return;
                }
                dt = new DataTable();
                dt = objAccount.CheckJobCode(jobcode, ddlTeam.SelectedValue);

                if (dt.Rows.Count > 0)
                {
                    sb.Append(lblClientName.Text + ", ");
                }
            }

            if (sb.Length > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "selectPlatform", "alert('Client/s " + sb.ToString().TrimEnd(',').Trim() + " are already available in RTM.Please remove from grid and save.')", true);
                return;
            }

            foreach (GridViewRow row in grClients.Rows)
            {
                DropDownList ddlGVPlatform = (DropDownList)row.FindControl("ddlGVPlatform");
                Label lblJobCode = (Label)row.FindControl("lblJC0");
                Label lblClientName = (Label)row.FindControl("lblClientName");
                Label lblClientCode = (Label)row.FindControl("lblClientCode");

                string jobcode = lblJobCode.Text.Trim() + " => " + lblClientName.Text.Trim();

                bool result = objAccount.AddNewClient(ddlTeam.SelectedValue, lblClientName.Text.Trim(), jobcode, ddlGVPlatform.SelectedValue, lblClientCode.Text, Session["UID"].ToString());
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Success", "alert('Client added successfully please sync RTM to reflect the new client')", true);
            ddlTeam.SelectedIndex = 0;
            grClients.DataSource = null;
            grClients.DataBind();
            ViewState["clients"] = null;
        }
        catch (Exception)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Filure", "alert('Exception Occured. Please contact RTM Team.')", true);          
        }
        
    }
}