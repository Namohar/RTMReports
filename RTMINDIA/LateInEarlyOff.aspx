<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="LateInEarlyOff.aspx.cs" Inherits="LateInEarlyOff" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form runat="server">
    <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Late Login and Early Logoff Details</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Late Login and Early Logoff Details</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
                  <table style="width: 100%;">
         <tr>
             <td width="40%" colspan="2">
                 <asp:Label ID="lblError" runat="server"></asp:Label>
                 <asp:Label ID="lblid" runat="server" Text="0" Visible="false"></asp:Label>
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 <asp:Label ID="lblFrom" runat="server" Text="From Date" Font-Bold="True"></asp:Label>
             </td>
             <td>
                 <asp:TextBox ID="txtFrom" runat="server" ClientIDMode="Static"></asp:TextBox>
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td style="text-align: right">
                 <asp:Label ID="lblTo" runat="server" Text="To Date" Font-Bold="True"></asp:Label>
             </td>
             <td>
                 <asp:TextBox ID="txtTo" runat="server" ClientIDMode="Static"></asp:TextBox>
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 <asp:Button ID="btnPrint" runat="server" onclick="btnPrint_Click" 
                     Text="Display" class="btn btn-primary" />
             &nbsp;&nbsp;
                 <asp:Button ID="btnExport" runat="server" onclick="btnExport_Click" 
                     Text="Export" class="btn btn-primary" />
                 </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td colspan="2">
                 <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Height="400px">
                 
                 <asp:GridView ID="gvEarlyLate" runat="server" CellPadding="4" 
                     ForeColor="#333333" Width="100%">
                     <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                     <EditRowStyle BackColor="#999999" />
                     <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                     <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                     <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                     <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                     <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                     <SortedAscendingCellStyle BackColor="#E9E7E2" />
                     <SortedAscendingHeaderStyle BackColor="#506C8C" />
                     <SortedDescendingCellStyle BackColor="#FFFDF8" />
                     <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                 </asp:GridView>
                 </asp:Panel>
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
     </table>
             </div>
            </div>
        </div>
    </div>
</form>
<script type="text/javascript">
    $(function () {
        $("#txtFrom").datepicker({ maxDate: new Date() });
    });

    $(function () {
        $("#txtTo").datepicker({ maxDate: new Date() });
    });

  </script>
</asp:Content>

