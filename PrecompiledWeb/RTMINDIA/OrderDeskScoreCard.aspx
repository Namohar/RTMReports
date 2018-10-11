<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="OrderDeskScoreCard, App_Web_ecwshzyq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form runat="server">
<div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Order Details</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Order Details</li>
			</ol>
	 </div>
    </div>

     <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
               <table style="width: 100%;">
    <tr>
        <td style="text-align: right" width="50%">
            
            &nbsp;</td>
        <td>
          
            <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="text-align: right">
            
            <asp:Label ID="lblOrderStatus" runat="server" Font-Bold="True" 
                Text="Order Status"></asp:Label>
            
        </td>
        <td>
          
            <asp:DropDownList ID="ddlOrderStatus" runat="server" Width="200px" class="form-control">
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
          
            <asp:Label ID="lblEmp" runat="server" Font-Bold="True" Text="Employee"></asp:Label>
          
        </td>
        <td>
          
            <asp:DropDownList ID="ddlEmp" runat="server" Width="200px" class="form-control">
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
           
            <asp:Label ID="lblFrom" runat="server" Font-Bold="True" Text="From Date"></asp:Label>
           
        </td>
        <td>
            
            <asp:TextBox ID="txtFrom" runat="server" Width="200px" ClientIDMode="Static" class="form-control"></asp:TextBox>
            
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
            <asp:Label ID="lblTo" runat="server" Font-Bold="True" Text="To Date"></asp:Label>
            </td>
        <td>
            <asp:TextBox ID="txtTo" runat="server" Width="200px" ClientIDMode="Static" class="form-control"></asp:TextBox>
            </td>
    </tr>
    <tr>
        <td style="text-align: right">
           &nbsp; </td>
        <td>
            </td>
    </tr>
    <tr>
        <td style="text-align: right">
            </td>
        <td>
            <asp:Button ID="btnExport" runat="server" Text="Export" 
                onclick="btnExport_Click" Class="btn btn-primary" />
            </td>
    </tr>
    <tr>
        <td style="text-align: right">
            </td>
        <td>
           </td>
    </tr>
</table>
             </div>
            </div>
        </div>
    </div>
</form>
<script type="text/javascript">
    $(function () {
        $("#txtFrom").datepicker();
    });

    $(function () {
        $("#txtTo").datepicker();
    });
  </script>
</asp:Content>

