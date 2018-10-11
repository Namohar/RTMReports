<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Employee_List.aspx.cs" Inherits="Employee_List" %>

<%@ Register Assembly="obout_Grid_NET" Namespace="Obout.Grid" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script type="text/javascript" src="Scripts/quicksearch.js"></script>
<script type="text/javascript">
    $(function () {
        $('.search_textbox').each(function (i) {
            $(this).quicksearch("[id*=gvEmployeeList] tr:not(:has(th))", {
                'testQuery': function (query, txt, row) {
                    return $(row).children(":eq(" + i + ")").text().toLowerCase().indexOf(query[0].toLowerCase()) != -1;
                }
            });
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form id="Form1" runat="server">
    <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Employee List</h3>
	    <ol class="breadcrumb">
			<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
			<li><i class="fa fa-pencil-square-o"></i>Employee List</li>
		</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
                 <asp:Button ID="btnExport" runat="server" Text="Export Employees" 
                     class="btn btn-link" onclick="btnExport_Click" Font-Bold="true" />
                 <div class="table-responsive">
                     <cc1:Grid ID="GridEmployeeList" AllowAddingRecords="false"  runat="server" class="table table-striped table-bordered table-hover"
                      AllowFiltering ="true" FolderStyle="Styles/grand_gray">
                      <FilteringSettings InitialState="Visible" FilterPosition="Top" />
                     </cc1:Grid>
                     <asp:Panel ID="Panel1" runat="server" Height="350px" ScrollBars="Both" Visible="false">                       
                         <br />
                         <br />
                         <asp:GridView ID="gvEmployeeList" runat="server" AutoGenerateColumns="False" 
                             Width="100%" class="table table-striped table-bordered table-hover">
                             <Columns>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label ID="lblHUser" runat="server" Text="User Name"></asp:Label><br />
                                         <asp:TextBox ID="txtSearch1" runat="server" placeholder="User Name" class="search_textbox"></asp:TextBox>
                                     </HeaderTemplate>
                                     <ItemTemplate>
                                         <asp:Label ID="lblUserName" runat="server" Text='<%# Eval("User Name") %>'></asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label ID="lblHEmpId" runat="server" Text="Employee Id"></asp:Label><br />
                                         <asp:TextBox ID="txtSearch2" runat="server" placeholder="Employee Id" class="search_textbox"></asp:TextBox>
                                     </HeaderTemplate>
                                     <ItemTemplate>
                                         <asp:Label ID="lblEmpId" runat="server" Text='<%# Eval("Employee Id") %>'></asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label ID="lblHEmail" runat="server" Text="Email Id"></asp:Label><br />
                                         <asp:TextBox ID="txtSearch3" runat="server" placeholder="Email Id" class="search_textbox"></asp:TextBox>
                                     </HeaderTemplate>
                                     <ItemTemplate>
                                         <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email Id") %>'></asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label ID="lblHTeam" runat="server" Text="Team"></asp:Label><br />
                                         <asp:TextBox ID="txtSearch4" runat="server" placeholder="Team" class="search_textbox"></asp:TextBox>
                                     </HeaderTemplate>
                                     <ItemTemplate>
                                         <asp:Label ID="lblTeam" runat="server" Text='<%# Eval("Team") %>'></asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField>
                                     <HeaderTemplate>
                                         <asp:Label ID="lblHMgr" runat="server" Text="Reporting Manager"></asp:Label><br />
                                         <asp:TextBox ID="txtSearch5" runat="server" placeholder="Reporting Manager" class="search_textbox"></asp:TextBox>
                                     </HeaderTemplate>
                                     <ItemTemplate>
                                         <asp:Label ID="lblMgrEmail" runat="server" Text='<%# Eval("Reporting Manager") %>'></asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                             </Columns>
                         </asp:GridView>

                     </asp:Panel>
                 </div>                               
             </div>
            </div>
        </div>
    </div>
</form>
</asp:Content>

