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
using System.Security.Principal;
using BAL;
using System.Drawing;
using System.Security.Cryptography;

public partial class ViewTickets : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString());
    clsTicketing objTicket = new clsTicketing();
    clsRealTimeReports objReal = new clsRealTimeReports();
    clsTeam objTeam = new clsTeam();
    SqlDataAdapter da;
    DataTable dt = new DataTable();
    int team;
    string username;
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
            lblEmpId.Text = Session["empId"].ToString();
            lblTeam.Text = Session["team"].ToString();
            lblUser.Text = Session["username"].ToString();
            lblAccess.Text = Session["access"].ToString();

            if (lblAccess.Text != "1")
            {
                totalCount.Visible = false;
                count.Visible = false;
                filter.Visible = false;
            }
            

            if (lblAccess.Text == "1")
            {
                GetClosedTicketsCount();
            }
            
            getTickets();
        }
    }

    private void GetTicketStatus()
    {
        dt = new DataTable();

        dt = objTicket.GetTicketStatus();
    }


    private void LoadEmp(int teamid, string user1)
    {
        dt = new DataTable();
        dt = objReal.LoadEmp(Session["access"].ToString(), Session["Location"].ToString(), Session["username"].ToString(), Session["team"].ToString(), Session["user"].ToString(), Session["UID"].ToString());
        da.Fill(dt);        
    }

    private void getTickets()
    {
        dt = new DataTable();

        dt = objTicket.SearchTickets(Convert.ToInt32(lblTeam.Text), Convert.ToInt32(lblAccess.Text), lblEmpId.Text, lblUser.Text, Session["UID"].ToString());

        if (dt.Rows.Count > 0)
        {
            gvTickets.DataSource = dt;
            gvTickets.DataBind();
        }
        else
        {
            gvTickets.DataSource = null;
            gvTickets.DataBind();
        }
    }

    private void GetClosedTickets()
    {
        dt = objTicket.SearchClosedTickets();
        if (dt.Rows.Count > 0)
        {
            gvTickets.DataSource = dt;
            gvTickets.DataBind();
        }
        else
        {
            gvTickets.DataSource = null;
            gvTickets.DataBind();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        getTickets();
    }

    private void GetClosedTicketsCount()
    {
        dt = new DataTable();

        dt = objTicket.ClosedTicketsCount();

        if (dt.Rows.Count > 0)
        {
            lblTotalTicket.Text = dt.Rows[0]["total count"].ToString();
            lblLastWeekTickets.Text = dt.Rows[0]["last week"].ToString();
        }
        else
        {
            lblTotalTicket.Text = "0";
            lblLastWeekTickets.Text = "0";
        }
    }

    private void Search()
    {
        dt = new DataTable();

        //dt = objTicket.SearchData(ddlUser.SelectedValue, ddlStatus.SelectedItem.Text, Convert.ToInt32(lblAccess.Text), Convert.ToInt32(lblTeam.Text), lblUser.Text, lblEmpId.Text);
        //if (dt.Rows.Count > 0)
        //{
        //    gvTickets.DataSource = dt;
        //    gvTickets.DataBind();
        //}
        //else
        //{
        //    gvTickets.DataSource = null;
        //    gvTickets.DataBind();
        //}
    }

    protected void gvTickets_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Edit")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                //GridViewRow row = gvTickets.Rows[index];
                //Response.Redirect("ViewTicketDetails.aspx?id=" + HttpUtility.UrlEncode(objTeam.Encrypt(index.ToString())) + "&acc=" + HttpUtility.UrlEncode(objTeam.Encrypt(lblAccess.Text)) + "&user=" + HttpUtility.UrlEncode(objTeam.Encrypt(lblUser.Text)) + "&team=" + HttpUtility.UrlEncode(objTeam.Encrypt(lblTeam.Text)));
                Response.Redirect("ViewTicketDetails.aspx?id=" + HttpUtility.UrlEncode(objTeam.Encrypt(index.ToString())));
            }
        }
        catch (Exception)
        {


        }
    }

    

    private void GetAllTickets()
    {

    }

    protected void gvTickets_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.ToolTip = (e.Row.DataItem as DataRowView)["T_Summary"].ToString();
            Label lblcreatedDt = (Label)e.Row.FindControl("lblCreatedDate");
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            Label lbleta = (Label)e.Row.FindControl("lblETA");
            Label lblassigned = (Label)e.Row.FindControl("lblAssignedTo");
            string createdDt = lblcreatedDt.Text; //e.Row.Cells[3].Text; 
            string status = lblStatus.Text; // e.Row.Cells[6].Text;
            string eta = lbleta.Text; // e.Row.Cells[5].Text;
            string assigned = lblassigned.Text; // e.Row.Cells[4].Text;
            int diff = Convert.ToInt32(DateTime.Now.Subtract(Convert.ToDateTime(createdDt)).TotalDays);
            string tomorrow = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
            //foreach (TableCell cell in e.Row.Cells)
            //{
            //if (diff >= 2)
            //{
            //    if (status == "Active" || status == "In Progress")
            //    {
            //        cell.BackColor = System.Drawing.Color.Yellow;
            //    }
            //}

            if ((eta == "No ETA set" || eta == "") && assigned == "Not Assigned Yet")
            {
                e.Row.Cells[5].BackColor = Color.Green;
                e.Row.Cells[5].ForeColor = Color.White;
                //cell.BackColor = Color.Green;
            }
            else if (string.IsNullOrEmpty(eta))
            {
                e.Row.Cells[5].BackColor = Color.Green;
                e.Row.Cells[5].ForeColor = Color.White;
            }
            else if (string.IsNullOrWhiteSpace(eta))
            {
                e.Row.Cells[5].BackColor = Color.Green;
                e.Row.Cells[5].ForeColor = Color.White;
            }
            else if (eta == "No ETA set")
            {
                if (status != "Closed")
                {
                    e.Row.Cells[5].BackColor = Color.Green;
                    e.Row.Cells[5].ForeColor = Color.White;
                    //cell.BackColor = Color.Green;
                }

            }
            else if (tomorrow == eta && status != "Closed")
            {
                e.Row.Cells[5].BackColor = Color.Yellow;
                e.Row.Cells[5].ForeColor = Color.Black;
                //cell.BackColor = Color.Yellow;
            }
            else if (DateTime.Now.ToString("MM/dd/yyyy") == eta && status != "Closed")
            {
                e.Row.Cells[5].BackColor = Color.Yellow;
                e.Row.Cells[5].ForeColor = Color.Black;
            }
            else if (DateTime.Now > Convert.ToDateTime(eta) && status != "Closed")
            {
                e.Row.Cells[5].BackColor = Color.Red;
                e.Row.Cells[5].ForeColor = Color.White;
                //cell.BackColor= Color.Red ;
            }
            //}

        }
    }

    protected void OnDataBound(object sender, EventArgs e)
    {
        //GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
        //for (int i = 0; i < gvTickets.Columns.Count; i++)
        //{
        //    TableHeaderCell cell = new TableHeaderCell();
        //    TextBox txtSearch = new TextBox();
        //    txtSearch.Attributes["placeholder"] = gvTickets.Columns[i].HeaderText;
        //    txtSearch.CssClass = "search_textbox";
        //    cell.Controls.Add(txtSearch);
        //    row.Controls.Add(cell);
        //}
        //gvTickets.HeaderRow.Parent.Controls.AddAt(0, row);
        ////gvTickets.HeaderRow.Controls.AddAt(1, row);
    }
    protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlFilter.SelectedValue == "Open")
        {
            getTickets();
        }
        else
        {
            GetClosedTickets();
        }
    }
}