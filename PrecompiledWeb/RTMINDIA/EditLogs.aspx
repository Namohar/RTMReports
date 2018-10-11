<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="EditLogs, App_Web_as44pg0l" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  <style type="text/css">
        .style1
        {
            height: 217px;
        }
        .modalBackground
    {
        background-color: Black;
        filter: alpha(opacity=60);
        opacity: 0.6;
    }
    .modalPopup
    {
        background-color: #FFFFFF;
        width: 600px;
        border: 3px solid #0DA9D0;
        border-radius: 12px;
        padding:0
      
    }
    .modalPopup .header
    {
        background-color: #2FBDF1;
        height: 30px;
        color: White;
        line-height: 30px;
        text-align: center;
        font-weight: bold;
        border-top-left-radius: 6px;
        border-top-right-radius: 6px;
    }
    .modalPopup .body
    {
        min-height: 50px;
        line-height: 30px;
        text-align: center;
        font-weight: bold;
    }
    .modalPopup .footer
    {
        padding: 6px;
    }
    .modalPopup .yes, .modalPopup .no
    {
        height: 23px;
        color: White;
        line-height: 23px;
        text-align: center;
        font-weight: bold;
        cursor: pointer;
        border-radius: 4px;
    }
    .modalPopup .yes
    {
        background-color: #2FBDF1;
        border: 1px solid #0DA9D0;
    }
    .modalPopup .no
    {
        background-color: #9F9F9F;
        border: 1px solid #5C5C5C;
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Edit Logs</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Edit Logs</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
               <div class="panel-body">
                   <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
                    </asp:ToolkitScriptManager>
                  <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                  <ContentTemplate>
  
  
                
                     <table style="width: 100%;">
                         <tr>
                             <td colspan="2">
                                 &nbsp;
                                 &nbsp;
                                 &nbsp;
                                 <asp:Label ID="lblError" runat="server" Font-Bold="True"></asp:Label>
                             </td>
                         </tr>
                         <tr>
                             <td style="text-align: right" width="40%">
                                 <asp:Label ID="lblTeam" runat="server" Font-Bold="True" Text="Select Team:"></asp:Label>
                             </td>
                             <td>
                                 <asp:DropDownList ID="ddlTeam" runat="server" AutoPostBack="True" 
                                     class="form-control" onselectedindexchanged="ddlTeam_SelectedIndexChanged">
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
                             <td style="text-align: right" width="40%">
                                 <asp:Label ID="lblEmployee" runat="server" Font-Bold="True" Text="Employee:"></asp:Label>
                             </td>
                             <td>
                                 <asp:DropDownList ID="ddlEmp" runat="server" class="form-control">
                                 </asp:DropDownList>
                                 <asp:Label ID="lblid" runat="server" Visible="False"></asp:Label>
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
                                 <asp:TextBox ID="txtDate" runat="server" ClientIDMode="Static" class="form-control"></asp:TextBox>
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
                                 <asp:Button ID="btnDisplay" runat="server" onclick="btnDisplay_Click" 
                                     Text="Display" class="btn btn-primary" />
                             </td>
                         </tr>
                         <tr>
                             <td style="text-align: right">
                                 &nbsp;</td>
                             <td>
                                 &nbsp;</td>
                         </tr>
                         <tr>
                             <td style="text-align: right" class="style1" colspan="2">
                                <%-- <asp:Panel ID="Panel1" runat="server" Height="302px">--%>
                                <asp:Panel ID="pnlAdd" runat="server" ScrollBars="None">
                                 <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="False" DataKeyNames="LA_ID" 
                                     Width="100%" class="table table-striped table-bordered table-hover">
                                     <Columns>
                                         <asp:TemplateField HeaderText="User Name">
                                             <ItemTemplate>
                                                 <asp:Label ID="lblUser" runat="server" Text='<%# Eval("LA_User_Name") %>'></asp:Label>
                                             </ItemTemplate>
                                             <HeaderStyle HorizontalAlign="Center" />
                                             <ItemStyle HorizontalAlign="Center" />
                                         </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Start Time">
                                             <ItemTemplate>
                                                 <asp:Label ID="lblStarTime" runat="server" 
                                                     Text='<%# Eval("LA_Start_Date_Time") %>'></asp:Label>
                                             </ItemTemplate>
                                             <HeaderStyle HorizontalAlign="Center" />
                                             <ItemStyle HorizontalAlign="Center" />
                                         </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Idle Reason">
                                             <ItemTemplate>
                                                 <asp:Label ID="lblReason" runat="server" Text='<%# Eval("LA_Reason") %>'></asp:Label>
                                             </ItemTemplate>
                                             <HeaderStyle HorizontalAlign="Center" />
                                             <ItemStyle HorizontalAlign="Center" />
                                         </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Duration">
                                             <ItemTemplate>
                                                 <asp:Label ID="lblDuration" runat="server" Text='<%# Eval("LA_Duration") %>'></asp:Label>
                                             </ItemTemplate>
                                             <HeaderStyle HorizontalAlign="Center" />
                                             <ItemStyle HorizontalAlign="Center" />
                                         </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Comments">
                                             <ItemTemplate>
                                                 <asp:Label ID="lblComments" runat="server" Text='<%# Eval("LA_Comments") %>'></asp:Label>
                                             </ItemTemplate>
                                             <HeaderStyle HorizontalAlign="Center" />
                                             <ItemStyle HorizontalAlign="Center" />
                                         </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Edit">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkEdit" runat="server" onclick="lnkEdit_Click" >Edit</asp:LinkButton>
                                                </ItemTemplate>
                                         </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Split">
                                             <ItemTemplate>
                                                 <asp:LinkButton ID="lnkInsert" runat="server" onclick="lnkInsert_Click">Split</asp:LinkButton>
                                             </ItemTemplate>
                                             <HeaderStyle HorizontalAlign="Center" />
                                             <ItemStyle HorizontalAlign="Center" />
                                         </asp:TemplateField>
                                     </Columns>
                                 </asp:GridView>
                                 <%--</asp:Panel>--%>
                                 </asp:Panel>
                             </td>
                         </tr>
                     </table>
                       
                        <asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>
                    <asp:ModalPopupExtender ID="mpePopUp" runat="server" TargetControlID="lblHidden" PopupControlID="divPopUp">
                    </asp:ModalPopupExtender>
                    <div id="divPopUp" class="modalPopup" style="display: none">
                    <div class="header">
                       Split the record
                    </div>
                    <div class="body">
                        <asp:Label ID="lblMessage" runat="server" Font-Bold="True" Font-Size="Small"></asp:Label>
                        <table style="width: 100%;">
                            <tr>
                                <td style="text-align: left" colspan="3">
                                    <asp:Label ID="lblPopError" runat="server" Text=""></asp:Label>
                                </td>
               
                            </tr>
                            <tr>
                                <td style="text-align: right" width="40%">
                                     Idle Reason:
                                </td>
                                <td style="text-align: left" colspan="2">
                                    <asp:DropDownList ID="ddlReason" runat="server" Width="200px" 
                                        AutoPostBack="True" onselectedindexchanged="ddlReason_SelectedIndexChanged">
                                       <asp:ListItem>--Select Reason--</asp:ListItem>
                                        <asp:ListItem Value="Meeting">Meeting</asp:ListItem>
                                        <asp:ListItem>Conference-Call</asp:ListItem>
                                        <asp:ListItem>Break</asp:ListItem>
                                        <asp:ListItem>Non-Task</asp:ListItem>
                                        <asp:ListItem>Idle Time</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
               
                            </tr>
                            <tr>
                                <td style="text-align: right">
                                    <asp:Label ID="lblCategory" runat="server" Text="Category:" Visible="false"></asp:Label>
                                </td>
                                <td style="text-align: left" colspan="2">
                                    <asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="True" 
                                         Width="200px" Visible="false" 
                                        onselectedindexchanged="ddlCategory_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                
                            </tr>
                            <tr>
                                <td style="text-align: right">
                                    <asp:Label ID="lblSubCat" runat="server" Text="Sub Category:" Visible="false"></asp:Label>
                                </td>
                                <td style="text-align: left" colspan="2">
                                    <asp:DropDownList ID="ddlSubCategory" runat="server" Width="200px" Visible="false">
                                    </asp:DropDownList>
                                </td>
                
                            </tr>

                            <tr>
                                <td style="text-align: right">
                                    <asp:Label ID="lblClient" runat="server" Text="Client" Visible="false"></asp:Label>
                                </td>
                                <td style="text-align: left" colspan="2">
                                   <asp:DropDownList ID="ddlClient" runat="server" Width="200px" Visible="false">
                                    </asp:DropDownList>
                                </td>
                            </tr>

                             <tr>
                                <td style="text-align: right">
                                   Duration:
                                </td>
                                <td style="text-align: left" colspan="2">
                
                                    <asp:TextBox ID="txtDuration" runat="server" placeholder="HH:MM:SS" ></asp:TextBox>
                    
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right">
                   
                                </td>
                                <td style="text-align: left" colspan="2">
                                    <asp:Label ID="lblMainDuration" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                             <tr>
                                <td style="text-align: right">
                                   Comments:
                                </td>
                                <td style="text-align: left" colspan="2">
                                    <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" MaxLength="200" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: left" colspan="3">
                                    <asp:Label ID="lblTeamId" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblUserName" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblStartTime" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblEndTime" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblOldRId" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblProcess" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblOldDuration" runat="server" Text="" Visible="false"></asp:Label>
                                </td>
                
                            </tr>
                        </table>       
                    </div>
                    <div class="footer" align="right">
                        <asp:Button ID="btnYes" runat="server" Text="Insert" CssClass="yes" 
                            onclick="btnYes_Click" />
                        <asp:Button ID="btnNo" runat="server" Text="Cancel" CssClass="no" 
                            onclick="btnNo_Click" />
                    </div>
                </div>
                </ContentTemplate>
                  </asp:UpdatePanel>

               </div>
           </div>
      </div>
    </div>
</form>

<script type="text/javascript">
    function pageLoad(sender, args) {
        if (args.get_isPartialLoad()) {
            $("#txtDate").datepicker({ maxDate: new Date(), minDate: '-10' });
        }
    }
    $(function () {
        $("#txtDate").datepicker({ maxDate: new Date(), minDate: '-10' });
    });
    //, minDate: '-10'
  </script>
</asp:Content>

