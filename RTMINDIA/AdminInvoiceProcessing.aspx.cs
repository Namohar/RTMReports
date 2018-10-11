using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;

public partial class AdminInvoiceProcessing : System.Web.UI.Page
{
    
    clsEMSDB objEMS = new clsEMSDB();
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
            FetchClients();
            BindUser();
            TotalInvoicesRecieved();
            TotalInvoicesAssigned();
        }
    }

    private void FetchClients()
    {
        dt = objEMS.getClients(Session["EMSTeam"].ToString());
        if (dt.Rows.Count > 0)
        {
            ddlClient.DataSource = dt;
            ddlClient.DataTextField = "C_Name";
            ddlClient.DataValueField = "C_Code";
            ddlClient.DataBind();
            ddlClient.Items.Insert(0, "--Select Client--");
            ddlClient.SelectedIndex = 0;
        }
    }
    protected void ddlClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblError.Text = String.Empty;
        if (ddlClient.SelectedIndex == 0)
        {
            gvInvoices.DataSource = null;
            gvInvoices.DataBind();

            TotalInvoicesRecieved();
            TotalInvoicesAssigned();
        }
        else
        {
            BindGrid();
            TotalInvoicesRecieved();
            TotalInvoicesAssigned();
        }
    }

    private void TotalInvoicesRecieved()
    {
        dt = new DataTable();
        if (ddlClient.SelectedIndex == 0)
        {
            dt = objEMS.GetTotalInvoicesRecieved("all");
        }
        else
        {
            dt = objEMS.GetTotalInvoicesRecieved(ddlClient.SelectedValue);
        }
        if (dt.Rows.Count > 0)
        {
            txtTotal.Text = dt.Rows[0]["total"].ToString();
        }
    }

    private void TotalInvoicesAssigned()
    {
        dt = new DataTable();
        if (ddlClient.SelectedIndex == 0)
        {
            dt = objEMS.GetTotalInvoicesAssigned("all");
        }
        else
        {
            dt = objEMS.GetTotalInvoicesAssigned(ddlClient.SelectedValue);
        }
        if (dt.Rows.Count > 0)
        {
            txtAssigned.Text = dt.Rows[0]["total"].ToString();
        }
    }

    private void BindGrid()
    {
        dt = new DataTable();

        dt = objEMS.getInvoices(ddlClient.SelectedValue, Session["EMSTeam"].ToString());
        
        if (dt.Rows.Count > 0)
        {
            gvInvoices.DataSource = dt;
            gvInvoices.DataBind();
        }
        else
        {
            gvInvoices.DataSource = null;
            gvInvoices.DataBind();
        }
    }

    private void BindUser()
    {
        dt = new DataTable();
        dt = objEMS.getUser(Session["EMSTeam"].ToString());
        if (dt.Rows.Count > 0)
        {
            ddlUser.DataSource = dt;
            ddlUser.DataTextField = "UL_User_Name";
            ddlUser.DataValueField = "UL_ID";
            ddlUser.DataBind();
            ddlUser.Items.Insert(0, "--Select User--");
            ddlUser.SelectedIndex = 0;
        }
    }
    protected void lnkViewFile_Click(object sender, EventArgs e)
    {
        LinkButton lnk = (LinkButton)(sender);
        string id = lnk.CommandArgument;
        string url = "ViewFile.aspx?id="+id;
        string s = "window.open('"+ url +"', 'popup_window', 'width=1000,height =600, top=100,left=100,resizable=yes');";
        ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
       
    }

    private void download(DataTable dt)
    {
        Byte[] bytes = (Byte[])dt.Rows[0]["FI_Data"];
        Response.Buffer = true;
        Response.Charset = "";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = dt.Rows[0]["FI_ContentType"].ToString();
        Response.AddHeader("content-disposition", "attachment;filename="
        + dt.Rows[0]["FI_FileName"].ToString());
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        Assign();
    }

    private void Assign()
    {
        try
        {
            if (ddlUser.SelectedIndex == 0)
            {
                lblError.Text = "Please select user to Assign";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }
            int flag = 0;
            foreach (GridViewRow row in gvInvoices.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkRow = (row.Cells[1].FindControl("chkSelect") as CheckBox);
                    if (chkRow.Checked)
                    {
                        string _fileId = (row.Cells[0].FindControl("lblFIID") as Label).Text;

                        bool result;
                        if (Session["EMSTeam"].ToString() == "IP")
                        {
                            result = objEMS.IPAssign(_fileId, "IP_Asg", Session["username"].ToString(), ddlUser.SelectedItem.Text, DateTime.Now.ToString());
                        }
                        else if (Session["EMSTeam"].ToString() == "QC")
                        {
                            result = objEMS.QCAssign(_fileId, "QC_Asg", Session["username"].ToString(), ddlUser.SelectedItem.Text, DateTime.Now.ToString());
                        }

                        flag = 1;
                        

                    }
                }

            }
            if (flag == 1)
            {
                lblError.Text = "Selected invoices successfully assigned to " + ddlUser.SelectedItem.Text;
                lblError.ForeColor = System.Drawing.Color.Blue;
            }
            else
            {
                lblError.Text = "Please select the invoices";
                lblError.ForeColor = System.Drawing.Color.Red;
            }
            

            ddlUser.SelectedIndex = 0;
            BindGrid();
            TotalInvoicesRecieved();
            TotalInvoicesAssigned();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.ForeColor = System.Drawing.Color.Red;
        }
    }
}