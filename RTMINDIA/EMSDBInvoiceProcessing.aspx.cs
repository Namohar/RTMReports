using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using DAL;

public partial class EMSDBInvoiceProcessing : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    clsEMSDB objEMS = new clsEMSDB();

    SqlDBHelper objDB = new SqlDBHelper();
    string SQlQuery;
    bool result;
    private const int _firstEditCellIndex = 2;
    
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
           
            GetAssignedInvoices();
        }

        if (this.gvIPInvoices.SelectedIndex > -1)
        {
            // Call UpdateRow on every postback
            this.gvIPInvoices.UpdateRow(this.gvIPInvoices.SelectedIndex, false);
        } 
    }

    protected void btnHidden_Click(object sender, EventArgs e)
    {
        if (lblgvRowId.Text.Length <= 0)
        {
            return;
        }
        System.Threading.Thread.Sleep(5000);
        
        CheckandUpdateLineItem(lblgvRowId.Text);

        lblgvRowId.Text = string.Empty;
        lblgvInvoiceNo.Text = string.Empty;
    }

    private void GetAssignedInvoices()
    {
        dt = new DataTable();
        dt = objEMS.GetAssignedIPInvoices(Session["access"].ToString(), Session["username"].ToString());
        if (dt.Rows.Count > 0)
        {
            gvIPInvoices.DataSource = dt;
            gvIPInvoices.DataBind();
        }
        else
        {
            gvIPInvoices.DataSource = null;
            gvIPInvoices.DataBind();
        }
    }
    
    protected void gvIPInvoices_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            GridView _gridView = (GridView)sender;
            bool result;
            switch (e.CommandName)
            {
                case ("SingleClick"):

                    // Get the row index
                    int _rowIndex = int.Parse(e.CommandArgument.ToString());
                    int rowid = Convert.ToInt32(_gridView.DataKeys[_rowIndex].Value.ToString());
                    CheckBox chkSelect = (CheckBox)_gridView.Rows[_rowIndex].Cells[1].FindControl("chkSelect");
                    CheckBox chkIssue = (CheckBox)_gridView.Rows[_rowIndex].Cells[1].FindControl("chkIssue");
                    CheckBox chkCompleted = (CheckBox)_gridView.Rows[_rowIndex].Cells[1].FindControl("chkCompleted");
                    Label lblInvoiceNo = (Label)_gridView.Rows[_rowIndex].Cells[6].FindControl("lblInvoiceNo");
                    Label lblStatus = (Label)_gridView.Rows[_rowIndex].Cells[5].FindControl("lblStatus");
                    int _columnIndex = int.Parse(Request.Form["__EVENTARGUMENT"]);
                    _gridView.SelectedIndex = _rowIndex;
                    if (_columnIndex == 3)
                    {

                        if (chkSelect.Checked == true)
                        {
                            string url = "ViewFile.aspx?id=" + rowid;
                            string s = "window.open('" + url + "', 'popup_window', 'width=1000,height =600, top=100,left=100,resizable=yes');";
                            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Invoice", "alert('Please select invoice to view file')", true);
                            return;
                        }
                        return;
                    }
                    if (lblStatus.Text == "QC_Idle" || lblStatus.Text == "QC_Asg" || lblStatus.Text == "QC_Inp" || lblStatus.Text == "QC_Issue" || lblStatus.Text == "QC_Comp")
                    {

                        chkCompleted.Checked = true;
                        chkSelect.Checked = true;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "QCInvoices", "alert('Invoice Sent for QC. You cannot modify')", true);
                        GetAssignedInvoices();
                        return;
                    }

                    if (_columnIndex == 2)
                    {
                        if (lblStatus.Text == "IP_Asg" || lblStatus.Text == "Rework")
                        {
                            result = objEMS.UpdateStatus("IP_Inp", "", Convert.ToString(rowid));
                            if (result == true)
                            {
                                GetAssignedInvoices();
                                result = false;
                                result = objEMS.RecordLog(rowid.ToString(), "IP", "Status", "IP_Asg", "IP_Inp", Session["username"].ToString());
                                if (result == true)
                                {
                                    result = false;
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Invoice Error", "alert('Error occured.. Please try again')", true);
                            }
                        }

                        return;
                    }

                    if (_columnIndex == 11)
                    {
                        if (Session["access"].ToString() == "4")
                        {
                            return;
                        }
                    }

                    if (_columnIndex == 4 || _columnIndex == 5 || _columnIndex == 10 || _columnIndex == 12 || _columnIndex == 13 || _columnIndex == 14)
                    {
                        return;
                    }
                    if (_columnIndex == 7)
                    {
                        if (chkSelect.Checked == true)
                        {
                            lblPopTeam.Text = "IP";
                            lblPopFileId.Text = rowid.ToString();
                            dt = new DataTable();
                            dt = objEMS.GetComments(rowid);
                            if (dt.Rows.Count > 0)
                            {
                                gvComments.DataSource = dt;
                                gvComments.DataBind();
                            }
                            else
                            {
                                gvComments.DataSource = null;
                                gvComments.DataBind();
                            }
                            mpePopUp.Show();
                        }

                        return;
                    }
                    if (_columnIndex == 8)
                    {

                        if (chkSelect.Checked == true)
                        {

                            result = objEMS.UpdateStatus("IP_Issue", "", Convert.ToString(rowid));
                            if (result == true)
                            {
                                GetAssignedInvoices();
                                result = false;
                                result = objEMS.RecordLog(rowid.ToString(), "IP", "Status", lblStatus.Text, "IP_Issue", Session["username"].ToString());
                                if (result == true)
                                {
                                    result = false;
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Invoice Error2", "alert('Error occured.. Please try again')", true);
                                chkIssue.Checked = false;
                            }
                        }
                        else
                        {
                            chkIssue.Checked = false;
                        }
                        return;
                    }
                    if (_columnIndex == 9)
                    {
                        if (chkSelect.Checked == true)
                        {
                            //if (chkCompleted.Checked == true)
                            //{
                            //    chkCompleted.Checked = true;
                            //    return;
                            //}

                            if (lblInvoiceNo.Text.Length <= 0)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Enter Invoice No", "alert('Please enter invoice#')", true);
                                chkCompleted.Checked = false;
                                return;
                            }
                            lblgvRowId.Text = rowid.ToString();
                            lblgvInvoiceNo.Text = lblInvoiceNo.Text;
                            string script = "$(document).ready(function () { $('[id*=btnHidden]').click(); });";
                            ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
                            //System.Threading.Thread.Sleep(5000);
                            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "load", "ShowProgress();", true);

                            //dt = new DataTable();
                            //dt = objEMS.getLineItem(rowid);
                            //if (dt.Rows.Count > 0)
                            //{
                            //    SqlConnection con = new SqlConnection("Data Source=" + dt.Rows[0]["C_DataSource"].ToString() + ";Initial Catalog=" + dt.Rows[0]["C_DataBase"].ToString() + ";User ID=" + dt.Rows[0]["C_UserId"].ToString() + ";Password=" + dt.Rows[0]["C_Password"].ToString() + ";");

                            //    SqlDataAdapter da = new SqlDataAdapter("SELECT INVOICE_NUM, ATTACHMENT_NAME FROM IM_INVOICE IM WITH (NOLOCK) LEFT JOIN IM_INVOICE_ATTACHMENT IMA WITH (NOLOCK) ON IM.INVOICE_RID = IMA.INVOICE_FK where Invoice_Num = '11897981'", con);

                            //    dt = new DataTable();

                            //    da.Fill(dt);

                            //    if (dt.Rows.Count > 0)
                            //    {
                            //        da = new SqlDataAdapter("SELECT IM.INVOICE_NUM, COUNT(INVOICE_LINE_ITEM_RID) AS LINE_ITEM_COUNT FROM IM_INVOICE IM WITH (NOLOCK) " +
                            //                                             "JOIN IM_INVOICE_INVENTORY INV WITH (NOLOCK) ON IM.INVOICE_RID = INV.INVOICE_FK " +
                            //                                            "JOIN IM_INVOICE_LINE_ITEM LIN WITH (NOLOCK) ON LIN.INVOICE_INV_FK = INV.INVOICE_INVENTORY_RID " +
                            //                                             "WHERE Invoice_Num = '1189798' " +
                            //                                       "GROUP BY IM.INVOICE_NUM", con);
                            //        DataTable dt3 = new DataTable();
                            //        da.Fill(dt3);

                            //        if (dt3.Rows.Count > 0)
                            //        {
                            //            result = objEMS.UpdateStatus("QC_Idle", dt3.Rows[0]["LINE_ITEM_COUNT"].ToString(), Convert.ToString(rowid));
                            //            if (result == true)
                            //            {
                            //                GetAssignedInvoices();
                            //                result = false;
                            //            }
                            //            else
                            //            {
                            //                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Invoice Error3", "alert('Error occured.. Please try again')", true);
                            //                chkCompleted.Checked = false;
                            //            }
                            //        }
                            //        else
                            //        {

                            //        }
                            //    }
                            //    else
                            //    {

                            //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "LineItem Error", "alert('OOPS! You missed to attach the invoice to EMS')", true);
                            //        chkCompleted.Checked = false;
                            //    }

                            //}
                            //else
                            //{

                            //}


                        }
                        else
                        {
                            chkCompleted.Checked = false;
                        }
                        return;
                    }
                    if (chkSelect.Checked == true)
                    {
                        GetAssignedInvoices();
                        Control _displayControl = _gridView.Rows[_rowIndex].Cells[_columnIndex].Controls[1];
                        _displayControl.Visible = false;
                        Control _editControl = _gridView.Rows[_rowIndex].Cells[_columnIndex].Controls[3];
                        _editControl.Visible = true;
                        _gridView.Rows[_rowIndex].Cells[_columnIndex].Attributes.Clear();
                        ScriptManager.RegisterStartupScript(this, GetType(), "SetFocus",
                                    "document.getElementById('" + _editControl.ClientID + "').focus();", true);
                        if (_editControl is DropDownList && _displayControl is Label)
                        {
                            try
                            {
                                ((DropDownList)_editControl).ClearSelection();
                                ((DropDownList)_editControl).Items.FindByText(((Label)_displayControl).Text).Selected = true;
                            }
                            catch
                            {
                                ((DropDownList)_editControl).SelectedValue = ((Label)_displayControl).Text;
                            }

                        }
                        if (_editControl is TextBox)
                        {
                            ((TextBox)_editControl).Attributes.Add("onfocus", "this.select()");
                        }
                    }

                    break;
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('"+ ex.Message +"')", true);
            GetAssignedInvoices();
        }
        
    }
    protected void gvIPInvoices_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            
            LinkButton _singleClickButton = (LinkButton)e.Row.Cells[0].Controls[0];
            
            string _jsSingle = ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");

            
            if (Page.Validators.Count > 0)
                _jsSingle = _jsSingle.Insert(11, "if(Page_ClientValidate())");
            checked
            {
                for (int columnIndex = _firstEditCellIndex; columnIndex < e.Row.Cells.Count; columnIndex++)
                {
                    string js = _jsSingle.Insert(_jsSingle.Length - 2, columnIndex.ToString());
                    e.Row.Cells[columnIndex].Attributes["onclick"] = js;
                    e.Row.Cells[columnIndex].Attributes["style"] += "cursor:pointer;cursor:hand;";
                }
            }
           

            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            CheckBox chkSelect = (CheckBox)e.Row.FindControl("chkSelect");
            CheckBox chkIssue = (CheckBox)e.Row.FindControl("chkIssue");
            CheckBox chkComp = (CheckBox)e.Row.FindControl("chkCompleted");
            DropDownList ddlGvUser = (DropDownList)e.Row.FindControl("ddlGvUser");

            dt = new DataTable();
            dt = objEMS.getUser("IP");
            if (dt.Rows.Count > 0)
            {
                ddlGvUser.DataSource = dt;
                ddlGvUser.DataTextField = "UL_User_Name";
                ddlGvUser.DataValueField = "UL_ID";
                ddlGvUser.DataBind();
                ddlGvUser.Items.Insert(0, "--Select--");
                ddlGvUser.SelectedIndex = 0;
            }
            if (lblStatus.Text == "IP_Asg" )
            {
                chkSelect.Checked = false;
            }
            else if (lblStatus.Text == "Rework")
            {
                chkSelect.Checked = false;
            }
            else
            {
                chkSelect.Checked = true;
            }
            if (lblStatus.Text == "IP_Issue")
            {
                chkSelect.Checked = true;
                chkIssue.Checked = true;
            }
            if (lblStatus.Text == "QC_Idle" || lblStatus.Text == "QC_Asg" || lblStatus.Text == "QC_Inp" || lblStatus.Text == "QC_Issue" || lblStatus.Text == "QC_Comp")
            {
                chkSelect.Checked = true;
                chkComp.Checked = true;
            }
        }
    }
    protected void gvIPInvoices_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView _gridView = (GridView)sender;
        string key = "";
        string value = "";
        //string selectedText = "";

        string[] _columnKeys = new string[] { "select", "File", "Source", "Status", " Invoice#", "Notes", "Issue", "Completed", "IP Asg By", "IP Com By", "ErrorNo", "QC Asg By", "QC Com By" };
        Label lblStatus = (_gridView.Rows[e.RowIndex].FindControl("lblStatus") as Label);
        if (lblStatus.Text == "QC_Idle" || lblStatus.Text == "QC_Asg" || lblStatus.Text == "QC_Inp" || lblStatus.Text == "QC_Issue" || lblStatus.Text == "QC_Comp")
        {
            return;
        }
        if (e.RowIndex > -1)
        {
            checked
            {
                for (int i = _firstEditCellIndex; i < _gridView.Columns.Count; i++)
                {
                    if (i == 1)
                    {
                        continue;
                    }
                    if (i == 2)
                    {
                        continue;
                    }
                    if (i == 3)
                    {
                        continue;
                    }
                    if (i == 11)
                    {
                        if (Session["access"].ToString() == "4")
                        {
                            continue;
                        }
                    }
                    if (i == 4 || i == 5 || i == 10 || i == 12 || i == 13 || i == 14)
                    {
                        continue;
                    }
                    if (i == 7)
                    {
                        continue;
                    }
                    if (i == 8)
                    {
                        continue;
                    }
                    if (i == 9)
                    {
                        continue;
                    }


                    int rowid = Convert.ToInt32(_gridView.DataKeys[e.RowIndex].Value.ToString());
                    Control _displayControl = _gridView.Rows[e.RowIndex].Cells[i].Controls[1];
                    Control _editControl = _gridView.Rows[e.RowIndex].Cells[i].Controls[3];
                    key = _columnKeys[i - _firstEditCellIndex];
                    if (_editControl.Visible)
                    {
                        if (_editControl is TextBox)
                        {
                            value = ((TextBox)_editControl).Text;
                        }
                        else if (_editControl is DropDownList)
                        {
                            // value = ((DropDownList)_editControl).SelectedValue;
                            value = ((DropDownList)_editControl).SelectedItem.Text;
                        }
                        if (((Label)_displayControl).Text != value)
                        {
                            bool result;
                            if (i == 6)
                            {
                                result = objEMS.UpdateInvoiceNo(value, rowid.ToString());
                                if (result == true)
                                {
                                    result = objEMS.RecordLog(rowid.ToString(), "IP", "InvoiceNo", ((Label)_displayControl).Text, value, Session["username"].ToString());
                                    if (result == true)
                                    {
                                        result = false;
                                    }
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Invoice No", "alert('Error occured.. Please try again')", true);
                                }
                            }
                            if (i == 11)
                            {
                                if (value == "--Select--")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select User", "alert('Please select user')", true);
                                }
                                else
                                {
                                    result = objEMS.UpdateIPUser(rowid.ToString(), "IP_Asg", Session["username"].ToString(), value);
                                    if (result == true)
                                    {
                                        result = objEMS.RecordLog(rowid.ToString(), "IP", "Change User", ((Label)_displayControl).Text, value, Session["username"].ToString());
                                        if (result == true)
                                        {
                                            result = false;
                                        }
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Update User", "alert('Error occured while updating user.. Please try again')", true);
                                    }
                                }

                            }
                        }
                        e.NewValues.Add(key, value);
                        break;
                    }
                }
            }
           
            _gridView.SelectedIndex = -1;
            GetAssignedInvoices();
        }
    }

    protected override void Render(HtmlTextWriter writer)
    {
        // The client events for GridView1 were created in GridView1_RowDataBound
        foreach (GridViewRow r in gvIPInvoices.Rows)
        {
            if (r.RowType == DataControlRowType.DataRow)
            {
                checked
                {
                    for (int columnIndex = _firstEditCellIndex; columnIndex < r.Cells.Count; columnIndex++)
                    {
                        Page.ClientScript.RegisterForEventValidation(r.UniqueID + "$ctl00", columnIndex.ToString());
                    }
                }
                
            }
        }

        base.Render(writer);
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        bool result;
        if (txtComments.Text.Trim().Length <= 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EmptyComments", "alert('Please enter comments')", true);
            mpePopUp.Show();
            return;
        }
        result = objEMS.UpdateComments(txtComments.Text.Replace("'", " "), lblPopTeam.Text, lblPopFileId.Text, Session["username"].ToString());
        if (result == true)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Comments", "alert('Comments updated')", true);
            txtComments.Text = string.Empty;
            lblPopFileId.Text = string.Empty;
            lblPopTeam.Text = string.Empty;
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Comments Error", "alert('Error occured.. Please try again')", true);
            dt = new DataTable();
            dt = objEMS.GetComments(Convert.ToInt32(lblPopFileId.Text));
            if (dt.Rows.Count > 0)
            {
                gvComments.DataSource = dt;
                gvComments.DataBind();
            }
            else
            {
                gvComments.DataSource = null;
                gvComments.DataBind();
            }
            mpePopUp.Show();
        }
    }

    protected void btnNo_Click(object sender, EventArgs e)
    {

    }

    private void CheckandUpdateLineItem(string rowid)
    {
        try
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@invoiceNo",lblgvInvoiceNo.Text ),           
            };
            bool result;
            dt = new DataTable();
            dt = objEMS.getLineItem(Convert.ToInt32(rowid));
            if (dt.Rows.Count > 0)
            {
                SqlConnection con = new SqlConnection("Data Source=" + dt.Rows[0]["C_DataSource"].ToString() + ";Initial Catalog=" + dt.Rows[0]["C_DataBase"].ToString() + ";User ID=" + dt.Rows[0]["C_UserId"].ToString() + ";Password=" + dt.Rows[0]["C_Password"].ToString() + ";");
                SqlCommand cmd;
                SqlDataAdapter da;
                //SqlDataAdapter da = new SqlDataAdapter("SELECT INVOICE_NUM, ATTACHMENT_NAME FROM IM_INVOICE IM WITH (NOLOCK) LEFT JOIN IM_INVOICE_ATTACHMENT IMA WITH (NOLOCK) ON IM.INVOICE_RID = IMA.INVOICE_FK where Invoice_Num = '" + lblgvInvoiceNo.Text + "'", con);
                SQlQuery = "SELECT INVOICE_NUM, ATTACHMENT_NAME FROM IM_INVOICE IM WITH (NOLOCK) LEFT JOIN IM_INVOICE_ATTACHMENT IMA WITH (NOLOCK) ON IM.INVOICE_RID = IMA.INVOICE_FK where Invoice_Num =@invoiceNo";
                cmd = new SqlCommand();
                cmd.CommandText = SQlQuery;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.Parameters.AddRange(parameters);
                con.Open();
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                //dt = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    //da = new SqlDataAdapter("SELECT IM.INVOICE_NUM, COUNT(INVOICE_LINE_ITEM_RID) AS LINE_ITEM_COUNT FROM IM_INVOICE IM WITH (NOLOCK) " +
                    //                                     "JOIN IM_INVOICE_INVENTORY INV WITH (NOLOCK) ON IM.INVOICE_RID = INV.INVOICE_FK " +
                    //                                    "JOIN IM_INVOICE_LINE_ITEM LIN WITH (NOLOCK) ON LIN.INVOICE_INV_FK = INV.INVOICE_INVENTORY_RID " +
                    //                                     "WHERE Invoice_Num = '" + lblgvInvoiceNo.Text + "' " +
                    //                               "GROUP BY IM.INVOICE_NUM", con);
                    SqlParameter[] parameters1 = new SqlParameter[]
                    {
                        new SqlParameter("@invoiceNo",lblgvInvoiceNo.Text ),           
                    };
                    SQlQuery = "SELECT IM.INVOICE_NUM, COUNT(INVOICE_LINE_ITEM_RID) AS LINE_ITEM_COUNT FROM IM_INVOICE IM WITH (NOLOCK) " +
                                                         "JOIN IM_INVOICE_INVENTORY INV WITH (NOLOCK) ON IM.INVOICE_RID = INV.INVOICE_FK " +
                                                        "JOIN IM_INVOICE_LINE_ITEM LIN WITH (NOLOCK) ON LIN.INVOICE_INV_FK = INV.INVOICE_INVENTORY_RID " +
                                                         "WHERE Invoice_Num = @invoiceNo " +
                                                   "GROUP BY IM.INVOICE_NUM";
                    cmd = new SqlCommand();
                    cmd.CommandText = SQlQuery;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.AddRange(parameters1);
                    con.Open();
                    da = new SqlDataAdapter(cmd);

                    DataTable dt3 = new DataTable();
                    //dt3 = objDB.ExecuteParamerizedSelectCommand(SQlQuery, CommandType.Text, parameters);
                    da.Fill(dt3);
                    con.Close();

                    if (dt3.Rows.Count > 0)
                    {
                        result = objEMS.UpdateStatus("QC_Idle", dt3.Rows[0]["LINE_ITEM_COUNT"].ToString(), Convert.ToString(rowid));
                        if (result == true)
                        {
                            GetAssignedInvoices();
                            result = false;
                            result = objEMS.RecordLog(rowid.ToString(), "IP", "Status", "IP_Inp", "QC_Idle", Session["username"].ToString());
                            if (result == true)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Select Invoice Error3", "alert('Error occured.. Please try again')", true);
                            //chkCompleted.Checked = false;
                        }
                    }
                    else
                    {

                    }
                }
                else
                {

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "LineItem Error", "alert('OOPS! You missed to attach the invoice to EMS')", true);
                    //chkCompleted.Checked = false;
                }

            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "InvNo Error", "alert('Error Occured while checking invoice.. Please try again')", true);
            }

            GetAssignedInvoices();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Exception", "alert('"+ ex.Message +"')", true);
            GetAssignedInvoices();
        }
        
    }
}