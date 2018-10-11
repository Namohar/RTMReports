<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="ManageUsers, App_Web_as44pg0l" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script type="text/javascript">
    $(function () {
        $("[id$=txtTeam]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/ManageUsers.aspx/GetTeams") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfTeamId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('/')[0],
                                val: item.split('/')[1],
                            }
                        }));
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            },
            select: function (e, i) {
                $("[id$=hfTeamId]").val(i.item.val);
            },
            minLength: 1
        });
    });  
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Manage Users</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Manage Users</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
         <div class="panel panel-default">
            <div class="panel-body">
              <div class="col-lg-6">
                <div class="form-group">
                 <asp:Label ID="lblError" runat="server"></asp:Label>
                    <br />
                   <strong style="color: #990000">Team:</strong>
  <%--                  <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" 
                        AutoPostBack="True" onselectedindexchanged="ddlTeam_SelectedIndexChanged">
                    </asp:DropDownList>--%>
                    <asp:HiddenField ID="hfTeamId" runat="server" Value="0" />
                       <br />
                       <asp:TextBox ID="txtTeam" runat="server" class="form-control"  
                        ontextchanged="txtTeam_TextChanged"  AutoPostBack="True"> </asp:TextBox>&nbsp&nbsp

                          <asp:Button ID="btnViewTeam" runat="server" Font-Bold="True" Text="View Details" 
                     class="btn btn-primary" onclick="btnViewTeam_Click" />
                    <br />
                         <br />
                    <strong style="color: #990000">First Name (space) Last Name</strong>
                    <asp:TextBox ID="txtname" runat="server" class="form-control" MaxLength="50"></asp:TextBox>
                    <br />
                     <strong style="color: #990000">Gender:</strong>
                    <asp:DropDownList ID="ddlGender" runat="server" class="form-control">
                      <asp:ListItem>-Select Gender-</asp:ListItem>
                        <asp:ListItem>M</asp:ListItem>
                        <asp:ListItem>F</asp:ListItem>
                        <asp:ListItem>U</asp:ListItem>
                    </asp:DropDownList>
                    <br />                  
                       <strong style="color: #990000">Employee ID:</strong>
                    <asp:TextBox ID="txtEmpId" runat="server" class="form-control" MaxLength="6"></asp:TextBox>
                    <br />
                         <strong style="color: #990000">Email ID:</strong>
                    <asp:TextBox ID="txtEmail" runat="server" class="form-control" MaxLength="50"></asp:TextBox>
                    <br />   
                    <strong style="color: #990000">System Name(Ex. CORP\John.Smith):</strong>
                    <asp:TextBox ID="txtSystemName" runat="server" class="form-control" 
                        MaxLength="50"></asp:TextBox>
                    <br /> 
                   
                      <strong style="color: #990000">Date of Joining</strong>
<%--                    <asp:TextBox ID="txtDOJ" runat="server" class="form-control"></asp:TextBox>--%>
                                                  <asp:TextBox ID="txtDOJ" runat="server" ClientIDMode="Static" class="form-control"></asp:TextBox>
                     <br /> 
                    <asp:Button ID="btnAdd" runat="server" Font-Bold="True" Text="Add Record" 
                     class="btn btn-primary" onclick="btnAdd_Click" />&nbsp&nbsp
                    <asp:Button ID="btnReset" runat="server" Font-Bold="True" Text="Reset" 
                     class="btn btn-primary" onclick="btnReset_Click" />
                </div>
              </div>

               <div class="col-lg-6">
                <div class="form-group">
                   <br />
                     <strong style="color: #990000">Reporting manager Employee Id</strong>
                    <asp:TextBox ID="txtMgrId" runat="server" class="form-control" MaxLength="6"></asp:TextBox>
                    <br />
        

                      <strong style="color: #990000">Reporting manager Email Id</strong>
                    <asp:TextBox ID="txtMgrEmailId" runat="server" class="form-control" 
                        MaxLength="50"></asp:TextBox>
                  
                    <br />
                 <strong style="color: #990000">Hourly?:</strong>
                    <asp:DropDownList ID="ddlHourly" runat="server" class="form-control">
                        <asp:ListItem>-Select Hourly type-</asp:ListItem>
                        <asp:ListItem>Yes</asp:ListItem>
                        <asp:ListItem>No</asp:ListItem>
                    </asp:DropDownList>
                    <br />
                       <strong style="color: #990000">Employee Type:</strong>
                    <asp:DropDownList ID="ddlType" runat="server" class="form-control">
                        <asp:ListItem>-Employee Type-</asp:ListItem>
                        <asp:ListItem>Contractor</asp:ListItem>
                        <asp:ListItem>Tangoe</asp:ListItem>
                        <asp:ListItem>Tangoe Temp</asp:ListItem>
                    </asp:DropDownList>      
                    <br />
                       <strong>Payroll Id</strong>
                    <asp:TextBox ID="txtPayrollId" runat="server" class="form-control" 
                        MaxLength="50"></asp:TextBox>

                     <br />
                    <strong>Employee Number</strong>
                    <asp:TextBox ID="txtEmpNo" runat="server" class="form-control" MaxLength="50"></asp:TextBox>
                     <br /> 
          
                </div>
              </div>
              <br />
        <br />
<%--                  <strong style="color: #0000FF; font-size: x-large">Team Details</strong>--%>
                <br />
        <br />
                      <br />
              <div class="table-responsive">
                  <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" 
                      class="table table-striped table-bordered table-hover" 
                      DataKeyNames="UL_ID" onrowcommand="gvUsers_RowCommand">

                      <Columns>
                          <asp:BoundField DataField="UL_Employee_Id" HeaderText="Employee Id" />
                          <asp:BoundField DataField="UL_User_Name" HeaderText="User Name" />
                          <asp:BoundField DataField="UL_System_User_Name" HeaderText="System Name" />
                          <asp:BoundField DataField="UL_Gender" HeaderText="Gender" />
                          <asp:BoundField DataField="UL_EmailId" HeaderText="Email" />
                          <asp:BoundField DataField="UL_RepMgrId" HeaderText="Report manager emp Id" />
                          <asp:BoundField DataField="UL_RepMgrEmail" HeaderText="Report manager email Id" />
            <%--              <asp:BoundField DataField="UL_Hourly" HeaderText="Hourly" />--%>
                            <asp:TemplateField HeaderText="Hourly">
      
     <ItemTemplate>
     <asp:Label ID="Label1"   runat="server" Text='<%# (Eval("UL_Hourly").ToString()== "") || (Eval("UL_Hourly").ToString()== "0") ? "No" : "Yes" %>'></asp:Label>
     </ItemTemplate>
     </asp:TemplateField>
                          <asp:BoundField DataField="UL_DOJ" HeaderText="DOJ" DataFormatString="{0:MM/dd/yyyy}" />
                          <asp:BoundField DataField="UL_EmployeeType" HeaderText="Employee Type" />
                             <asp:BoundField DataField="UL_Employee_Number" HeaderText="Employee Number" />
                             <asp:BoundField DataField="UL_Payroll_Id" HeaderText="Payroll Id" />
                             
                             <asp:TemplateField HeaderText="Edit">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' CommandName = "EditRow">Edit</asp:LinkButton>
                                                </ItemTemplate>
                             </asp:TemplateField>
                      </Columns>
                  </asp:GridView>
              </div>
            </div>
        </div>
    </div>
   </div>




 <script type="text/javascript">
     $(function () {
         $("#txtDOJ").datepicker({ maxDate: new Date() });
     });
  </script>

  </form>
</asp:Content>





