<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="ManageClients, App_Web_diasz2zu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script type = "text/javascript">
    function Confirm() {
        var confirm_value = document.createElement("INPUT");
        confirm_value.type = "hidden";
        confirm_value.name = "confirm_value";
        if (confirm("Are you sure you want to Activate/Deactivate this client?")) {
            confirm_value.value = "Yes";
        } else {
            confirm_value.value = "No";
        }
        document.forms[0].appendChild(confirm_value);
    }
 </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Manage Clients</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Manage Clients</li>
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
                   <strong>Team:</strong>
                    <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" 
                        AutoPostBack="True" onselectedindexchanged="ddlTeam_SelectedIndexChanged">
                    </asp:DropDownList>
                   <%-- <br />
                    <strong>Enter Client Name (Exactly as T-Sheet Job Code):</strong>--%>
                    <asp:TextBox ID="txtClientName" runat="server" class="form-control" Visible="false"></asp:TextBox>
                    <%--<br />
                    <strong>Enter Project Code  (Exactly as T-Sheet Job Code):</strong>--%>
                    <asp:TextBox ID="txtJobCode" runat="server" class="form-control" Visible="false"></asp:TextBox>
                </div>
              </div>

              <div class="col-lg-6">
               <div class="form-group">
               <br />
                  <%--<strong>Core Platform:</strong>--%>
                   <asp:DropDownList ID="ddlCore" runat="server" class="form-control" Visible="false">
                   </asp:DropDownList>
                  <%-- <br />
                   <strong>Enter Client Code (3 Digit):</strong>--%>
                   <asp:TextBox ID="txtCode" runat="server" class="form-control" MaxLength="3" Visible="false"></asp:TextBox>
                  <%-- <br />
                   <br />
                   --%>
                   <asp:Button ID="btnAdd" runat="server" Font-Bold="True" Text="Add Record" 
                     class="btn btn-primary" onclick="btnAdd_Click" Visible="false" />
               </div>
             </div>
             <br />
             <div class="table-responsive">
                 <asp:GridView ID="gvClients" runat="server" AutoGenerateColumns="False" 
                     class="table table-striped table-bordered table-hover" 
                     onrowdatabound="gvClients_RowDataBound">
                     <Columns>
                         <%--<asp:BoundField DataField="T_TeamName" HeaderText="Team" />--%>
                         <asp:BoundField DataField="CL_ClientName" HeaderText="Client" />
                         <asp:BoundField DataField="CL_TSheetClient" HeaderText="Job Code" />
                         <asp:BoundField DataField="CL_Product" HeaderText="Core Platform" />
                         <asp:BoundField DataField="CL_Code" HeaderText="Client Code" />
                         <asp:BoundField DataField="status" HeaderText="Status" />
                         <asp:TemplateField HeaderText="Action">
                             <ItemTemplate>
                                 <asp:LinkButton ID="lnkAction" runat="server" onclick="lnkAction_Click" OnClientClick = "Confirm()">Activate/Deactivate</asp:LinkButton>
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
</asp:Content>

