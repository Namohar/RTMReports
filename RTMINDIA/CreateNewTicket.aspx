<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CreateNewTicket.aspx.cs" Inherits="CreateNewTicket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form runat="server">
 <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Create New Ticket</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Create New Ticket</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
                <table style="width: 100%;">
        
        <tr>
            <td style="text-align: center" colspan="3">
                <asp:Label ID="lblMessage" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                <asp:Label ID="lblTicketType" runat="server" Text="Ticket Type:" 
                    Font-Bold="True" Font-Size="Small"></asp:Label>
            </td>
            <td>
                
                <asp:DropDownList ID="ddlTicketType" runat="server" class="form-control">
                    <asp:ListItem>--Select--</asp:ListItem>
                    <asp:ListItem>Additional Feature</asp:ListItem>
                    <asp:ListItem>Change Request</asp:ListItem>
                    <asp:ListItem>New Report</asp:ListItem>
                    <asp:ListItem>Production Issue</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:Label ID="lblEmpId" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblUid" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblTeamId" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblEmail" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblUserName" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblRptMgrEmail" runat="server" Text="lblRptMgrEmail" 
                    Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                &nbsp;</td>
            <td>
                
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: right">
                <asp:Label ID="lblTicketSummary" runat="server" Text="Ticket Summary:" 
                    Font-Bold="True" Font-Size="Small"></asp:Label>
            </td>
            <td>
                
                <asp:TextBox ID="txtTicketSummary" runat="server" class="form-control" placeholder="Max 36 Characters" MaxLength="36"></asp:TextBox>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: right">
                &nbsp;</td>
            <td>
                
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: right; vertical-align: top;">
                <asp:Label ID="lblRequestDetails" runat="server" Text="Request Details:" 
                    Font-Bold="True" Font-Size="Small"></asp:Label>
            </td>
            <td>
    
                <textarea id="txtDetails" cols="20" rows="5" placeholder="Max 512 Characters" maxlength="512" runat="server" class="form-control" ></textarea> </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: right; vertical-align: top;">
                &nbsp;</td>
            <td>
    
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: right">
                <asp:Label ID="lblScreenShot" runat="server" Text="Attach file/Screen Shot:" 
                    Font-Bold="True" Font-Size="Small"></asp:Label>
            </td>
            <td>
                
                <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="true" />
</td>
            <td rowspan="6" style="vertical-align: bottom; text-align: right">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: right">
                &nbsp;</td>
            <td>
                <br />
               <asp:Button ID="btnUpload" runat="server" Text="Upload" onclick="btnUpload_Click"  />
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
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="I_ID">
                <Columns>
                    <asp:BoundField DataField="I_Name" HeaderText="File Name" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkDelete" runat="server" onclick="lnkDelete_Click">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                </asp:GridView></td>
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
                <asp:Button ID="btnCreate" class="btn btn-primary" runat="server" Text="Create Ticket" 
                    onclick="btnCreate_Click" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancel" class="btn btn-primary" runat="server" Text="Back" 
                    onclick="btnCancel_Click" Visible="false" />
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: right">
                &nbsp;</td>
            <td>
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
</asp:Content>

