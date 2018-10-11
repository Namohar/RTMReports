<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="ManageUsersNew, App_Web_diasz2zu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        var jQuery_1_8_3 = $.noConflict(true);
    </script>
    <%--<link rel="stylesheet" href="http://code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />--%>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
      <script src="//code.jquery.com/jquery-1.8.2.js"></script>
      <script src="//code.jquery.com/ui/1.8.24/jquery-ui.js"></script>
     <style type="text/css">
        .loading
    {
        
        display: none;
        position: fixed;
        background-color: White;
       
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
                 var top = '60px';
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
    <form runat="server">
     <div class="row">
     <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-dashboard"></i>Manage Users</h3>
		<ol class="breadcrumb">
		    <li><i class="fa fa-home"></i><a href="DashBoard.aspx">Home</a></li>
	    	<li><i class="fa fa-edit"></i>Manage Users</li>
		</ol>
	 </div>
	</div>

    <div  class="row" >
       <div class="col-md-12 col-sm-12 col-xs-12">
          <div class="panel panel-default">
             <%--<div class="panel-heading">
                 Upload Clients
             </div>--%>
             <div class="panel-body">
                 <div>
                    <strong>
                        <asp:Label ID="lblEmpid" runat="server" Text="Employee ID:"></asp:Label></strong>
                     <asp:TextBox ID="txtEmpId" runat="server"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <strong><asp:Label ID="lblOr" runat="server" Text="OR" Visible="false"></asp:Label></strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <strong>
                         <asp:Label ID="lblLastName" runat="server" Text="Last Name:" Visible="false"></asp:Label></strong>
                     <asp:TextBox ID="txtLastName" runat="server" Visible="false"></asp:TextBox> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <asp:Button ID="btnSearch" runat="server" Text="Search HRIS" 
                         class="btn btn-primary" onclick="btnSearch_Click"/>
                 </div>
                 <br />
                 <div style="text-align:center">
                     <strong><asp:Label ID="lblResult" runat="server" Text=""></asp:Label></strong>
                 </div>
                 <div class="table-responsive">
                     <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" 
                         class="table table-striped table-bordered table-hover" 
                         onrowdatabound="gvUsers_RowDataBound">
                         <Columns>
                             <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkRow" runat="server" onclick="CheckOne(this)" />
                                </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="Employee ID">
                                 <ItemTemplate>
                                     <asp:Label ID="lblGvEmpId" runat="server" Text='<%# Eval("MUL_EmployeeId") %>'></asp:Label>
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="First Name">
                                 <ItemTemplate>
                                      <asp:Label ID="lblGvFirstName" runat="server" Text='<%# Eval("MUL_FirstName") %>'></asp:Label>     
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="Last Name">
                                 <ItemTemplate>
                                      <asp:Label ID="lblGvLastName" runat="server" Text='<%# Eval("MUL_LastName") %>'></asp:Label>
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="Email Id">
                                 <ItemTemplate>
                                       <asp:Label ID="lblGvEmailId" runat="server" Text='<%# Eval("MUL_EmailId") %>'></asp:Label>        
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="Reporting Manager">
                                 <ItemTemplate>
                                       <asp:Label ID="lblGvManager" runat="server" Text='<%# Eval("Manager") %>'></asp:Label>
                                     <asp:Label ID="lblGvManagerId" runat="server" Text='<%# Eval("MUL_ManagerID") %>' Visible="false"></asp:Label> 
                                     <asp:Label ID="lblManagerEmail" runat="server" Text='<%# Eval("MUL_ManagerEmail_id") %>' Visible="false"></asp:Label>    
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="System/Network UserName">
                                 <ItemTemplate>
                                     <asp:TextBox ID="txtSystemName" runat="server" placeholder="Ex: CORP\UserName" Text='<%# Eval("UL_System_User_Name") %>'></asp:TextBox>     
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="RTM Status">
                                 <ItemTemplate>
                                     <asp:Label ID="lblGvStatus" runat="server" Text='<%# Eval("MUL_ActiveStatus") %>'></asp:Label>    
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="empId" Visible="false">
                                 <ItemTemplate>
                                     <asp:Label ID="lblGvCheckempId" runat="server" Text='<%# Eval("UL_Employee_Id") %>'></asp:Label>        
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="version" Visible="false">
                                 <ItemTemplate>
                                       <asp:Label ID="lblGvVersion" runat="server" Text='<%# Eval("UL_Version") %>'></asp:Label>        
                                 </ItemTemplate>
                             </asp:TemplateField>

                         </Columns>
                     </asp:GridView>
                 </div>
                 <br />
                 <div id="dvsave" runat="server" visible="false">
                     <asp:Label ID="lblTeam" runat="server" Text="Assign Team:"></asp:Label>
                     <asp:DropDownList ID="ddlTeam" runat="server">
                     </asp:DropDownList>
                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <asp:Button ID="btnSave" runat="server" Text="Initiate RTM Installation" 
                         onclick="btnSave_Click" class="btn btn-primary" />
                 </div>

                 
             </div>
           </div>
        </div>

        <div class="col-md-12 col-sm-12 col-xs-12" id="dvHRIS" runat="server" visible="false">
          <div class="panel panel-default">
             <div class="panel-heading">
                 <strong>Upload HRIS File</strong> 
             </div>
             <div class="panel-body">
                 <asp:Label ID="lblError" runat="server" Text=""></asp:Label><br />
                 <asp:TextBox ID="txtPath" runat="server" Text="\\files\Shares\HRIS Data\Production" ReadOnly="true" Width="300px" ></asp:TextBox>
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                 <asp:Button ID="btnUpload" runat="server" Text="Upload" 
                     onclick="btnUpload_Click" class="btn btn-primary" />
             </div>
         </div>
        </div>
    </div>

     <div class="loading" align="center"> 
        <img src="images/pleasewait.gif" height="40px" width="40px" alt="" /><br />
        Please wait...
     </div>
</form>
<script src="http://code.jquery.com/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="http://code.jquery.com/ui/1.11.2/jquery-ui.js" type="text/javascript"></script>
    
<script type="text/javascript">
   
//    $(function () {
//        $("[id*=chkRow]").bind("click", function () {

//            //Find and reference the GridView.
//            var grid = $(this).closest("table");

//            //Find and reference the Header CheckBox.
//            //var chkHeader = $("[id*=chkHeader]", grid);

//            //If the CheckBox is Checked then enable the TextBoxes in thr Row.
//            if (!$(this).is(":checked")) {
//                var td = $("td", $(this).closest("tr"));
//                td.css({ "background-color": "#FFF" });
//                $("input[type=text]", td).attr("disabled", "disabled");
//            } else {
//                var td = $("td", $(this).closest("tr"));
//                td.css({ "background-color": "#D8EBF2" });
//                $("input[type=text]", td).removeAttr("disabled");
//            }

//            //Enable Header Row CheckBox if all the Row CheckBoxes are checked and vice versa.
//            if ($("[id*=chkRow]", grid).length == $("[id*=chkRow]:checked", grid).length) {
//                chkHeader.attr("checked", "checked");
//            } else {
//                chkHeader.removeAttr("checked");
//            }
//        });
//    });

    function CheckOne(obj) {
        var grid = obj.parentNode.parentNode.parentNode;
        var inputs = grid.getElementsByTagName("input");
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].type == "checkbox") {
                if (obj.checked && inputs[i] != obj && inputs[i].checked) {
                    inputs[i].checked = false;
//                    var td = $("td", $(obj).closest("tr"));
//                    td.css({ "background-color": "#FFF" });
//                    $("input[type=text]", td).attr("disabled", "disabled");
                }
            }
        }
    }  
  </script>
</asp:Content>

