<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="CMPDetails, App_Web_as44pg0l" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form runat="server">
  <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>CMP Details</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>CMP Details</li>
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
            
            <asp:Label ID="lblFrom" runat="server" Font-Bold="True" Text="From Date:"></asp:Label>
           
        </td>
        <td>
          
            <asp:TextBox ID="txtFrom" runat="server" Width="200px" ClientIDMode="Static"></asp:TextBox>
            
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
          
            <asp:Label ID="lblTo" runat="server" Font-Bold="True" Text="To Date:"></asp:Label>
          
        </td>
        <td>
          
            <asp:TextBox ID="txtTo" runat="server" Width="200px" ClientIDMode="Static"></asp:TextBox>
          
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
            
            <asp:Button ID="btnGet" runat="server" Text="Get Employees" 
                Class="button_example" onclick="btnGet_Click"/>
            
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
          
            <asp:Label ID="lblEmp" runat="server" Font-Bold="True" Text="Employee:"></asp:Label>
          
            </td>
        <td>
          
            <asp:DropDownList ID="ddlEmp" runat="server" Width="200px">
            </asp:DropDownList>
          
            </td>
    </tr>
    <tr>
        <td style="text-align: right">
           &nbsp;  </td>
        <td>
          &nbsp;   </td>
    </tr>
    <tr>
        <td style="text-align: right">
            </td>
        <td>
            <asp:Button ID="btnExport" runat="server" Text="Export" 
                 Class="styled-button-2" onclick="btnExport_Click" />
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
        $("#txtFrom").datepicker({ maxDate: new Date() });
    });

    $(function () {
        $("#txtTo").datepicker({ maxDate: new Date() });
    });
  </script>
</asp:Content>

