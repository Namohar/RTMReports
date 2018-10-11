<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DetailedReport.aspx.cs" Inherits="DetailedReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Task & Subtask Details</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Task & Subtask Details</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
                   <table style="width: 100%;">
                      <tr>
                          <td colspan="2">
                              <asp:Label ID="lblError" runat="server" Font-Bold="True"></asp:Label>
                          </td>
                      </tr>
                      <tr>
                          <td style="text-align: right" width="40%">
                              <asp:Label ID="lblTeam" runat="server" Text="Team:" Font-Bold="True"></asp:Label>
                          </td>
                          <td width="60%">
                              <asp:DropDownList ID="ddlTeam" runat="server" 
                                     class="form-control" >
                                </asp:DropDownList>
                          </td>
                      </tr>
                    
                      <tr>
                          <td style="text-align: right" width="40%">
                              &nbsp;</td>
                          <td>
                              &nbsp;</td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                              <asp:Label ID="lblDate" runat="server" Font-Bold="True" Text="Date:"></asp:Label>
                          </td>
                          <td>
                              <input id="datepicker" runat="server" clientidmode="Static" type="text" class="form-control" /></td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                              &nbsp;</td>
                          <td>
                              &nbsp;</td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                              <asp:Label ID="lblTo" runat="server" Font-Bold="True" Text="To:" 
                                  ></asp:Label>
                          </td>
                          <td>
                              <input id="datepickerTo" runat="server" clientidmode="Static" type="text" 
                                   class="form-control" /></td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                          </td>
                          <td>
                              &nbsp;</td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                              &nbsp;</td>
                          <td>
                             
                              &nbsp;<asp:Button ID="btnExport" runat="server" class="btn btn-primary" 
                                  Text="Export" onclick="btnExport_Click"  />
                          </td>
                      </tr>
                  </table>
             </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(function () {
            $("#datepicker").datepicker({ maxDate: new Date() });
        });

        $(function () {
            //            $("#datepickerTo").datepicker({ maxDate: "+1M +10D" });
            $("#datepickerTo").datepicker({ maxDate: new Date() });
        });
  </script>
</form>
</asp:Content>

