<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="OrderDeskHourlyTaskReport.aspx.cs" Inherits="OrderDeskHourlyTaskReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<script type="text/javascript" src="Scripts/jquery.timepicker.js"></script>
<link rel="stylesheet" type="text/css" href="Styles/jquery.timepicker.css" />
<style type="text/css">
   #timepicker
        {
            width: 150px;
        }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form runat="server">
  <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Hourly Task Report</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Hourly Task Report</li>
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
            
                            <asp:Label ID="lblEmp" runat="server" Font-Bold="True" Text="Employee"></asp:Label>
          
                        </td>
                        <td>
          
                            <asp:DropDownList ID="ddlEmp" runat="server" Width="200px">
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
            
                            <asp:Label ID="lblFrom" runat="server" Font-Bold="True" Text="Date"></asp:Label>
           
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
          
                            <asp:Label ID="lblFromTime" runat="server" Text="From Time" Font-Bold="True" ></asp:Label>
          
                        </td>
                        <td>
          
                            <asp:TextBox ID="txtFromTime" runat="server" Width="200px" ClientIDMode="Static"></asp:TextBox>
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
           
                            <asp:Label ID="lblToTime" runat="server" Font-Bold="True" Text="To Time" 
                                Visible="False"></asp:Label>
                        </td>
                        <td>
            
                            <asp:TextBox ID="txtToTime" runat="server" Width="200px" ClientIDMode="Static" 
                                Visible="False"></asp:TextBox>
                        </td>
                    </tr>
                    <%--<tr>
                        <td style="text-align: right">
           
                            &nbsp;</td>
                        <td>
            
                            &nbsp;</td>
                    </tr>--%>
                    <tr>
                        <td style="text-align: right">
                            <asp:Label ID="lblHours" runat="server" Font-Bold="True" Text="Total Hours"></asp:Label>
                        </td>
                        <td>
          
                            <asp:DropDownList ID="ddlHours" runat="server" Width="200px">
                                <asp:ListItem>--Select--</asp:ListItem>
                                <asp:ListItem>1</asp:ListItem>
                                <asp:ListItem>2</asp:ListItem>
                                <asp:ListItem>3</asp:ListItem>
                                <asp:ListItem>4</asp:ListItem>
                                <asp:ListItem>5</asp:ListItem>
                                <asp:ListItem>6</asp:ListItem>
                                <asp:ListItem>7</asp:ListItem>
                                <asp:ListItem>8</asp:ListItem>
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
                            </td>
                        <td>
                            <asp:Button ID="btnExport" runat="server" Text="Export" 
                                 Class="styled-button-2" onclick="btnExport_Click"  />
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

    $(function () {
        $('#txtFromTime').timepicker({
            'step': '60',
            'minTime': '6:00pm'
            //            'maxTime': new Date()
        });
    });

    $(function () {
        $('#txtToTime').timepicker({
            'step': '60',
            'minTime': '6:00pm'
            //            'maxTime': new Date()
        });
    });
  </script>
</asp:Content>

