<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AddRecordsNew.aspx.cs" Inherits="AddRecordsNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <style>
   .tb10 {
	background-image:url(images/form_bg.jpg);
	background-repeat:repeat-x;
	border:1px solid #d1c7ac;
	width: 180px;
	color:#333333;
	padding:3px;
	margin-right:4px;
	margin-bottom:8px;
	font-family:tahoma, arial, sans-serif;
}
</style>

  <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.0.min.js" type="text/javascript"></script>
<script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
<link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css"
    rel="Stylesheet" type="text/css" />
<script type="text/javascript">
    $(function () {
        $("[id$=txtSeachClient]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/AddRecordsNew.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('!')[0],
                                val: item.split('!')[1],
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
                $("[id$=hfCustomerId]").val(i.item.val);
            },
            minLength: 1
        });
    });  
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form id="Form1" runat="server" autocomplete="off">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Add Records</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Add Records</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
               
                <div class="col-lg-4">
                 <div class="form-group"> 
                     <strong>Select Team:</strong>
                     <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" Width="200px" 
                         AutoPostBack="True" onselectedindexchanged="ddlTeam_SelectedIndexChanged">

                     </asp:DropDownList>
                     <br />
                     <strong>Employee:</strong>
                     <asp:DropDownList ID="ddlEmployee" runat="server" class="form-control" Width="200px" 
                         AutoPostBack="True" onselectedindexchanged="ddlEmployee_SelectedIndexChanged">
                     </asp:DropDownList>
                     <br />
                     <strong>Date:</strong>
                     <asp:TextBox ID="txtDate" runat="server" class="form-control" 
                         ClientIDMode="Static" ontextchanged="txtDate_TextChanged" 
                         AutoPostBack="True" Width="200px"></asp:TextBox>
                 </div>
                </div>
                <asp:Panel ID="pnlAdd" runat="server" ScrollBars="None">
                <div class="col-lg-8">
                  <div id="select" class="col-lg-12" runat="server" visible="false">
                      <asp:RadioButton ID="rbMultClient" runat="server" Checked="True" 
                          GroupName="select" AutoPostBack="True" 
                          oncheckedchanged="rbMultClient_CheckedChanged" 
                          Text="Multiple Clients, Similar Task" />
                      &nbsp;&nbsp;&nbsp;
                      <asp:RadioButton ID="rbMultTask" runat="server" GroupName="select" 
                          AutoPostBack="True" oncheckedchanged="rbMultTask_CheckedChanged" 
                          Text="Multiple Clients, Multiple Tasks" />
                  </div>
                  <br />
                 <div class="form-group" id="dvAdd" runat="server" visible="false"> 
                     <strong>Search Client:</strong>
                      <asp:HiddenField ID="hfCustomerId" runat="server" />
                     <asp:TextBox ID="txtSeachClient" runat="server" class="tb10"></asp:TextBox>
                     &nbsp;&nbsp;&nbsp;
                     <asp:Button ID="btnSearch" runat="server" Text="Select It" class="btn btn-primary" 
                         onclick="btnSearch_Click" />
                         <br />
                         <br />
                     <div class="table-responsive" id="dvClientsGrid" runat="server" visible="false">
                         <asp:GridView ID="gvClients" runat="server" AutoGenerateColumns="False" 
                             ShowHeaderWhenEmpty="true" 
                             class="table table-striped table-bordered table-hover" 
                             onrowdeleting="gvClients_RowDeleting">
                         <Columns>
                            
                            <asp:TemplateField HeaderText="Client Name">
                                     <ItemTemplate>
                                         <asp:Label ID="lblClientID" runat="server" Text='<%# Eval("clientId") %>' Visible="false"></asp:Label>
                                         <asp:Label ID="lblClientName" runat="server" Text='<%# Eval("clientName") %>'></asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                            <asp:TemplateField HeaderText="Duration">
                                     <ItemTemplate>
                                         <asp:TextBox ID="txtDuration" runat="server" Width="90px" ClientIDMode="Static"></asp:TextBox>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Comments">
                                     <ItemTemplate>
                                         <asp:TextBox ID="txtComments" runat="server" MaxLength="199"></asp:TextBox>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowDeleteButton="True" />
                        </Columns>
                         </asp:GridView>
                         
                         <asp:Label ID="lblPleaseNote" Visible="false" Font-Bold="true" runat="server" Text="Please note: Whenever you select Client as “Internal”, the Search Client option will be made Inactive. 
This is done to avoid inadvertent user error in selecting Task and Sub-Task, as Client “Internal” can accept only Tasks where service code is “General/Internal”. 
 As soon as you add/update your “Internal” client data, Search Client option will be active again to take new inputs.
"></asp:Label>
                         <asp:Label ID="lblSorry" runat="server" Visible="false" Font-Bold="true" Text="Sorry, you won’t be able to add “Internal” to this list if there is already another customer/client present. 
This is done to avoid incorrect Client and Task selection as “Internal” only accepts Tasks where service code is “General/Internal”. 
Please finish updating the Client(s) already in the list – then you can add “Internal” as a fresh selection.
"></asp:Label>
                         <br />
                         <br />
                      <strong>Task:</strong>
                     <asp:DropDownList ID="ddlTask" runat="server" class="tb10" AutoPostBack="True" 
                         onselectedindexchanged="ddlTask_SelectedIndexChanged">
                     </asp:DropDownList>

                     <strong>Sub Task:</strong>
                     <asp:DropDownList ID="ddlSubTask" runat="server" class="tb10" 
                         onselectedindexchanged="ddlSubTask_SelectedIndexChanged">
                     </asp:DropDownList>
                     
                     </div>

                     <div class="table-responsive" id="dvTasksGrid" runat="server" visible="false">
                         <asp:Panel ID="Panel1" runat="server" ScrollBars="Both">
                         
                        <asp:GridView ID="gvTasks" runat="server" AutoGenerateColumns="False" 
                             ShowHeaderWhenEmpty="true" 
                             class="table table-striped table-bordered table-hover" 
                             onrowdeleting="gvTasks_RowDeleting" onrowdatabound="gvTasks_RowDataBound">
                         <Columns>
                           
                            <asp:TemplateField HeaderText="Client Name">
                                     <ItemTemplate>
                                         <asp:Label ID="lblTaskClientID" runat="server" Text='<%# Eval("clientId") %>' Visible="false"></asp:Label>
                                         <asp:Label ID="lblTaskClientName" runat="server" Text='<%# Eval("clientName") %>'></asp:Label>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                            <asp:TemplateField HeaderText="Task">
                                     <ItemTemplate>
                                         <asp:DropDownList ID="ddlGVTask" runat="server" class="tb10" AutoPostBack="True" onselectedindexchanged="ddlGVTask_SelectedIndexChanged" Width="80px" >
                                         </asp:DropDownList>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sub Task">
                                     <ItemTemplate>
                                         <asp:DropDownList ID="ddlGVSubTask" runat="server" class="tb10" Width="80px" >
                                         </asp:DropDownList>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                            <asp:TemplateField HeaderText="Duration">
                                     <ItemTemplate>
                                         <asp:TextBox ID="txtTaskDuration" runat="server" Width="40px" ClientIDMode="Static" ></asp:TextBox>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Comments">
                                     <ItemTemplate>
                                         <asp:TextBox ID="txtTaskComments" runat="server" TextMode="MultiLine" MaxLength="199" ></asp:TextBox>
                                     </ItemTemplate>
                                 </asp:TemplateField>
                                 <asp:CommandField ShowDeleteButton="True" />
                        </Columns>
                         </asp:GridView>
                         </asp:Panel>
                     </div>
                     <br />
                    <asp:Button ID="btnSave" runat="server" Text="Add" class="btn btn-primary" 
                         onclick="btnSave_Click" OnClientClick="this.disabled = true; this.value = 'Saving...';" 
                         UseSubmitBehavior="false" />
                 </div>
                </div>
                </asp:Panel>
             </div>
           </div>
      </div>
      <br />
      <div class="col-md-12 col-sm-12 col-xs-12">
         <div class="table-responsive">
         <asp:GridView ID="gvRecords" runat="server" AutoGenerateColumns="False" 
             Width="100%" 
            DataKeyNames="R_ID" class="table table-striped table-bordered table-hover">
            <Columns>
                <asp:TemplateField HeaderText="Employee">
                    <ItemTemplate>
                        <asp:Label ID="lblUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                    </ItemTemplate>
                    
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Client">
                    <ItemTemplate>
                        <asp:Label ID="lblClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                    </ItemTemplate>
                    
                    
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Task">
                    <ItemTemplate>
                    <asp:Label ID="lblTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>  
                    </ItemTemplate>
                   
                   
                </asp:TemplateField>
                <asp:TemplateField HeaderText="SubTask">
                    <ItemTemplate>
                        <asp:Label ID="lblSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Duration">
                    <ItemTemplate>
                        <asp:Label ID="lblDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                    </ItemTemplate>
                   
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comments">                   
                    <ItemTemplate>
                        <asp:Label ID="lblComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                    </ItemTemplate>                   
                </asp:TemplateField>               
            </Columns>
        </asp:GridView>
     </div>
      </div>
    </div>
</form>
<script type="text/javascript" src="timepicker.js"></script>
<link rel="stylesheet" href="http://code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
<script type="text/javascript">
    $(function () {
        $("#txtDate").datepicker({ maxDate: new Date() });
    });
  </script>

  <script type="text/javascript">

      var _gaq = _gaq || [];
      _gaq.push(['_setAccount', 'UA-36251023-1']);
      _gaq.push(['_setDomainName', 'jqueryscript.net']);
      _gaq.push(['_trackPageview']);

      (function () {
          var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
          ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
          var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
      })();

</script>
</asp:Content>

