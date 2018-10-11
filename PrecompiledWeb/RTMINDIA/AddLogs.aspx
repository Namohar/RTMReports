<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="AddLogs, App_Web_as44pg0l" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<link rel="stylesheet" type="text/css" href="./jquery.datetimepicker.css"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form runat="server">
  <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Add Logs</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Add Logs</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
             <div class="col-lg-6">
                <div class="form-group">
                   <asp:Label ID="lblError" runat="server" Font-Bold="True"></asp:Label>
                   <asp:Label ID="lblTeam" runat="server" Font-Bold="True" Text="Select Team:"></asp:Label>
                 <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" 
                        AutoPostBack="True" onselectedindexchanged="ddlTeam_SelectedIndexChanged" >
                 </asp:DropDownList>
                 <br />
                   <asp:Label ID="lblEmployee" runat="server" Font-Bold="True" Text="Employee:"></asp:Label>
                   <asp:DropDownList ID="ddlEmp" runat="server" class="form-control" >
                 </asp:DropDownList>
                 <asp:Label ID="lblid" runat="server" Visible="False"></asp:Label>
                 <asp:Label ID="lblEmpID" runat="server" Visible="False"></asp:Label>
                 <br />
                 <asp:Label ID="lblDate" runat="server" Font-Bold="True" Text="Date:"></asp:Label>
                   <asp:TextBox ID="txtDate" runat="server" ClientIDMode="Static"  
                     class="some_class form-control" AutoPostBack="True" ontextchanged="txtDate_TextChanged"></asp:TextBox>
                 <br />
                 <asp:Label ID="lblSubCat" runat="server" Font-Bold="True" Text="Sub Category:" 
                     Visible="False"></asp:Label>
                  <asp:DropDownList ID="ddlSubCategory" runat="server" Visible="false" 
                    class="form-control">
                 </asp:DropDownList>                 
                    <br />
                    <strong>Comments:</strong>
                    <asp:TextBox ID="txtComments" runat="server" MaxLength="200" 
                     TextMode="MultiLine" class="form-control"></asp:TextBox>
                </div>
             </div>

             <div class="col-lg-6">
                <div class="form-group">
                           
                   <strong>Idle Reason:</strong>
                  <asp:DropDownList ID="ddlReason" runat="server" AutoPostBack="True" 
                     onselectedindexchanged="ddlReason_SelectedIndexChanged" class="form-control">
                     <asp:ListItem>--Select Reason--</asp:ListItem>
                     <asp:ListItem Value="Meeting">Meeting</asp:ListItem>
                     <asp:ListItem>Conference-Call</asp:ListItem>
                     <asp:ListItem>Break</asp:ListItem>
                     <asp:ListItem>Non-Task</asp:ListItem>
                     <asp:ListItem>Idle Time</asp:ListItem>
                 </asp:DropDownList>
                   <br />
                 <asp:Label ID="lblCategory" runat="server" Font-Bold="True" Text="Category:" 
                     Visible="False"></asp:Label>
                  <asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="True" 
                     onselectedindexchanged="ddlCategory_SelectedIndexChanged" Visible="false" 
                     class="form-control">
                 </asp:DropDownList>
                 <br />
                 <asp:Label ID="lblClient" runat="server" Font-Bold="True" Text="Client:" 
                     Visible="False"></asp:Label>
                  <asp:DropDownList ID="ddlClient" runat="server" Visible="false" class="form-control">
                 </asp:DropDownList>
                 <br />
                 <strong>Duration:</strong>
                  <asp:TextBox ID="txtDuration" runat="server" placeholder="HH:MM:SS" 
                     class="form-control"></asp:TextBox>
                 <br />
                 <br />
                
                 
                 <asp:Button ID="btnAdd" runat="server" Font-Bold="True" Text="Add Log" 
                     class="btn btn-primary" onclick="btnAdd_Click" />
                </div>
             </div>

             <br />

             <div class="table-responsive">
                <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="False" DataKeyNames="LA_ID" 
                     Width="100%" class="table table-striped table-bordered table-hover">
                     <Columns>
                         <asp:TemplateField HeaderText="User Name">
                             <ItemTemplate>
                                 <asp:Label ID="lblUser" runat="server" Text='<%# Eval("LA_User_Name") %>'></asp:Label>
                             </ItemTemplate>
                             
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Start Time">
                             <ItemTemplate>
                                 <asp:Label ID="lblStarTime" runat="server" 
                                     Text='<%# Eval("LA_Start_Date_Time") %>'></asp:Label>
                             </ItemTemplate>
                            
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Idle Reason">
                             <ItemTemplate>
                                 <asp:Label ID="lblReason" runat="server" Text='<%# Eval("LA_Reason") %>'></asp:Label>
                             </ItemTemplate>
                            
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Duration">
                             <ItemTemplate>
                                 <asp:Label ID="lblDuration" runat="server" Text='<%# Eval("LA_Duration") %>'></asp:Label>
                             </ItemTemplate>
                           
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Comments">
                             <ItemTemplate>
                                 <asp:Label ID="lblComments" runat="server" Text='<%# Eval("LA_Comments") %>'></asp:Label>
                             </ItemTemplate>
                          
                         </asp:TemplateField>
                     </Columns>
                 </asp:GridView>
             </div>
             </div>
            </div>
        </div>
    </div>
</form>

<script type="text/javascript" src="build/jquery.datetimepicker.full.js"></script>

<script type="text/javascript">
    $('.some_class').datetimepicker({ maxDate: '0', minDate: '-1970/01/10' });
  </script>
</asp:Content>

