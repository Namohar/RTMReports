<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="AddRecord, App_Web_diasz2zu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<link rel="stylesheet" type="text/css" href="./jquery.datetimepicker.css"/>
    <style type="text/css">
        #timepicker
        {
            width: 99px;
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
    
     .loading
    {
        font-family: Arial;
        font-size: 10pt;
        border: 5px solid #67CFF5;
        width: 400px;
        height: 100px;
        display: none;
        position: fixed;
        background-color: White;
        z-index: 999;
    }
    </style>

    <script type="text/javascript">
        function ShowProgress() {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("loadingmodal");
                $('body').append(modal);
                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                loading.css({ top: top, left: left });
            }, 100);
        }
        $('form').live("submit", function () {
            ShowProgress();
        });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
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

             <div class="col-lg-6">
                <div class="form-group">
                <asp:Label ID="lblError" runat="server" Font-Bold="True"></asp:Label>
                <asp:Label ID="lblTeam" runat="server" Font-Bold="True" Text="Select Team:"></asp:Label>
                 <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" 
                        AutoPostBack="True" onselectedindexchanged="ddlTeam_SelectedIndexChanged" >
                 </asp:DropDownList>
                 <br />
                  <asp:Label ID="lblEmployee" runat="server" Font-Bold="True" Text="Employee:"></asp:Label>
                  <asp:DropDownList ID="ddlEmp" runat="server"  AutoPostBack="True" 
                     onselectedindexchanged="ddlEmp_SelectedIndexChanged" class="form-control">
                 </asp:DropDownList>
                 <asp:Label ID="lblid" runat="server" Visible="False"></asp:Label>
                 <asp:Label ID="lblEmpID" runat="server" Visible="False"></asp:Label>

                 <br />
                 <asp:Label ID="lblDate" runat="server" Font-Bold="True" Text="Date:"></asp:Label>
                 <asp:TextBox ID="txtDate" runat="server" ClientIDMode="Static" 
                     class="some_class form-control" AutoPostBack="True" ontextchanged="txtDate_TextChanged"></asp:TextBox>

                 <br />
                
                 <strong>Comments:</strong>
                 <asp:TextBox ID="txtComments" runat="server" MaxLength="200" 
                     TextMode="MultiLine"  class="form-control"></asp:TextBox>

                    
                </div>
             </div>

             <div class="col-lg-6">
               <div class="form-group">
                 
                 <strong>Client:</strong>
                 <asp:DropDownList ID="ddlNewClient" runat="server"  class="form-control" >
                 </asp:DropDownList>
                 <br />
                 <strong>Task:</strong>
                  <asp:DropDownList ID="ddlNewTask" runat="server" AutoPostBack="True" 
                     onselectedindexchanged="ddlNewTask_SelectedIndexChanged"  class="form-control">
                 </asp:DropDownList>
                 <br />
                  <strong>SubTask:</strong>
                  <asp:DropDownList ID="ddlNewSubTask" runat="server"  class="form-control">
                 </asp:DropDownList>

                 <br />
                 <strong>Duration:</strong>
                 <asp:TextBox ID="txtDuration" runat="server" 
                      class="form-control"></asp:TextBox>
                  <br />
                 
                <asp:Button ID="btnAdd" runat="server" Font-Bold="True" Text="Add Record" 
                     class="btn btn-primary" onclick="btnAdd_Click" />
               </div>
             </div>
              
     <br />

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
     </div>
    </div>

     <div class="loading" align="center"> <b>Please wait... </b>
        <br />
        <br />
        <img src="images/loader.gif" alt="" />
     </div>
</form>

<script type="text/javascript" src="build/jquery.datetimepicker.full.js"></script>

<script type="text/javascript">
    $('.some_class').datetimepicker({ maxDate: '0', minDate: '-1970/01/10' });
  </script>
</asp:Content>

