﻿<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="TaskDetails, App_Web_ecwshzyq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form runat="server">
  <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Task Details</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Task Details</li>
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
                 <asp:Label ID="lblTeam" runat="server" Font-Bold="True" Text="Select Team:"></asp:Label>
             </td>
             <td>
                 <asp:DropDownList ID="ddlTeam" runat="server" AutoPostBack="True" 
                     class="form-control" onselectedindexchanged="ddlTeam_SelectedIndexChanged">
                 </asp:DropDownList>
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
                 <asp:Label ID="lblFrom" runat="server" Text="From Date:" Font-Bold="True"></asp:Label>
             </td>
             <td>
                 <asp:TextBox ID="txtFrom" runat="server" ClientIDMode="Static" class="form-control"></asp:TextBox>
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
                 <asp:Label ID="lblTo" runat="server" Text="To Date:" Font-Bold="True"></asp:Label>
             </td>
             <td>
                 <asp:TextBox ID="txtTo" runat="server" ClientIDMode="Static" class="form-control"></asp:TextBox>
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
             &nbsp;
                 </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td colspan="2" style="text-align: right">
                 &nbsp;</td>
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

