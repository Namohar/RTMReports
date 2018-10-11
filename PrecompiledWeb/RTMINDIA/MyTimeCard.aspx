<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="MyTimeCard, App_Web_as44pg0l" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <style>
   .tb10 {
	background-image:url(images/form_bg.jpg);
	background-repeat:repeat-x;
	border:1px solid #d1c7ac;
	width: 220px;
	color:#333333;
	padding:3px;
	margin-right:4px;
	margin-bottom:8px;
	font-family:tahoma, arial, sans-serif;
}
</style>

 <style type="text/css">
        .modalBackground
    {
        background-color: Black;
        filter: alpha(opacity=60);
        opacity: 0.6;
    }
    .styled-button-2 {
	-webkit-box-shadow:rgba(0,0,0,0.2) 0 1px 0 0;
	-moz-box-shadow:rgba(0,0,0,0.2) 0 1px 0 0;
	box-shadow:rgba(0,0,0,0.2) 0 1px 0 0;
	border-bottom-color:#333;
	border:1px solid #61c4ea;
	background-color:#7cceee;
	border-radius:5px;
	-moz-border-radius:5px;
	-webkit-border-radius:5px;
	color:#333;
	font-family:'Verdana',Arial,sans-serif;
	font-size:14px;
	text-shadow:#b2e2f5 0 1px 0;
	padding:5px
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

    <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.0.min.js" type="text/javascript"></script>
<script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
<link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css"
    rel="Stylesheet" type="text/css" />
<script type="text/javascript">
    $(function () {
        $("[id$=txtSeachClient]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/MyTimeCard.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
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
                $("[id$=hfCustomerId]").val(i.item.val);
            },
            minLength: 1
        });
    });  

    function scrollTo(x, y)
    {

    }

    
</script>

   

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form id="Form1" runat="server" autocomplete="off">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Clock-In / Clock-Out</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>Clock-In/Clock-Out</li>
			</ol>
	 </div>
  </div>
  <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" CombineScripts="false">
    </asp:ToolkitScriptManager>
     <asp:Timer ID="tmrElapsed" runat="server" Enabled="False" 
        ontick="tmrElapsed_Tick" Interval="1">
                         </asp:Timer>
    
          <div class="row">
     <div class="col-md-12 col-sm-12 col-xs-12">
        <div class="panel panel-default">
           <div class="panel-heading">

           </div>
           <div class="panel-body">
              <div class="col-lg-12">
                 <div class="form-group"> 
                     <asp:Label ID="lblrecId" runat="server" Text="0"  Visible="false"></asp:Label>
                     <strong>Client:</strong>
                     <asp:HiddenField ID="hfCustomerId" runat="server" Value="0" />
                     <asp:TextBox ID="txtSeachClient" runat="server" class="tb10" style="width:260px"></asp:TextBox>
                     <strong>&nbsp; Task:</strong>
                     <asp:DropDownList ID="ddlTask" runat="server" class="tb10" AutoPostBack="True" 
                         onselectedindexchanged="ddlTask_SelectedIndexChanged">
                     </asp:DropDownList>
                     <strong>&nbsp; Sub-Task:</strong>
                     <asp:DropDownList ID="ddlSubTask" runat="server" class="tb10" 
                         AutoPostBack="True" onselectedindexchanged="ddlSubTask_SelectedIndexChanged" >
                         
                     </asp:DropDownList>
                     <br />
                     <br />
                     <strong style="vertical-align:top">Comments:</strong>
                     <textarea id="txtTaskComments" cols="40" rows="2" class="tb10" runat="server" style="margin: 0px 4px 8px 0px; width: 363px; height: 60px;" placeholder="Max 200 characters"></textarea>
                     <asp:Button ID="btnClockin" runat="server" Text="Clock In" 
                         class="btn btn-primary" onclick="btnClockin_Click" style="vertical-align:top" />
                     <asp:Button ID="btnClockout" runat="server" Text="Clock Out" Visible="false" 
                         class="btn btn-primary" onclick="btnClockout_Click" style="vertical-align:top" />
                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                         <asp:Label ID="lblStart" runat="server" Text="Start Time:" Font-Bold="True" Visible="false" style="vertical-align:top"></asp:Label>
                         <asp:Label ID="lblStartTime" runat="server" Text="" Font-Bold="True" Visible="false" style="vertical-align:top"></asp:Label>
                         <asp:Label ID="lblTimeZone" runat="server" Text="(UTC)" Font-Bold="True" Visible="false" style="vertical-align:top"></asp:Label>
                     
                     <br />
                     <br />
                     <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                         <Triggers>
                          <asp:AsyncPostBackTrigger controlid="tmrElapsed" eventname="Tick" />
                      </Triggers>
                    <ContentTemplate>
                     <div id="runningDetails" runat="server" visible="true" style="text-align:center" >
                      
                      <asp:Button ID="btnElapsed" runat="server" Text="Elapsed Time" Visible="false" 
                             class="btn btn-primary" onclick="btnElapsed_Click" />
                             <strong style="font-size:18px">Current:</strong>&nbsp;&nbsp;&nbsp;&nbsp;
                      <asp:Label ID="lblElapsed" runat="server" Text="0:0.00" Font-Bold="True" style="font-size:18px"></asp:Label>
                      &nbsp;&nbsp;&nbsp;&nbsp;
                       <strong style="font-size:18px">Day:</strong>&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblDay" runat="server" Text="00:00:00" style="font-size:18px" Font-Bold="True"></asp:Label>
                     &nbsp;&nbsp;&nbsp;&nbsp;
                     <strong style="font-size:18px">Week:</strong>&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblWeek" runat="server" Text="00:00:00" style="font-size:18px" Font-Bold="True"></asp:Label>
                     </div>
                     </ContentTemplate>
                  </asp:UpdatePanel>

                 </div>
              </div>
           </div>
        </div>
    </div>
</div>
    


<div class="row">
     <div class="col-md-12 col-sm-12 col-xs-12">
        <div class="panel panel-default">
          <%-- <div class="panel-heading">
               <strong>Past selection</strong>
           </div>--%>
           <div class="panel-body">
              <div class="col-lg-6">
              <div><strong>Pick a new client</strong></div>
              <br />
               <div class="table-responsive">
                <asp:Panel ID="Panel3" runat="server" Width="100%" Height="400px" 
                         ScrollBars="Both">
                 <asp:GridView ID="gvClients" runat="server" AutoGenerateColumns="False" 
                        class="table table-striped table-bordered table-hover" 
                        onrowdatabound="gvClients_RowDataBound">
                         <Columns>                             
                             <asp:TemplateField HeaderText="Client Name">
                                 <HeaderTemplate>
                                    <asp:Label ID="lblHeading" runat="server" Text="Client Name"></asp:Label><br />
                                    <asp:TextBox ID="txtSearch" runat="server" Width="300px" AutoCompleteType="Disabled" class="search_textbox tb10"></asp:TextBox>
                                 </HeaderTemplate>
                                 <ItemTemplate>
                                     <asp:Label ID="lblCLName" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:TemplateField HeaderText="ClientId" Visible="False">
                                 <ItemTemplate>
                                     <asp:Label ID="lblCLID1" runat="server" Text='<%# Eval("CL_ID") %>'></asp:Label>
                                 </ItemTemplate>
                             </asp:TemplateField>                             
                             <asp:TemplateField>
                                 <ItemTemplate>
                                     <asp:LinkButton ID="lnkClientSelect" runat="server" onclick="lnkClientSelect_Click">Clock-In</asp:LinkButton>
                                 </ItemTemplate>
                             </asp:TemplateField>
                         </Columns>
                     </asp:GridView>
                     </asp:Panel>
                 </div>
              </div>
              <div class="col-lg-6">
              <div><strong>Your past choices</strong></div>
              <br />
                 <div class="table-responsive">
                   <asp:Panel ID="Panel1" runat="server" Width="100%" Height="400px" 
                         ScrollBars="Both">
                     <asp:GridView ID="gvPastTasks" runat="server" AutoGenerateColumns="False" 
                           class="table table-striped table-bordered table-hover" 
                           onrowdatabound="gvPastTasks_RowDataBound">
                         <Columns>
                             <asp:BoundField DataField="CL_ClientName" HeaderText="Client" 
                                 HtmlEncode="False" />
                             <asp:TemplateField HeaderText="ClientId" Visible="False">
                                 <ItemTemplate>
                                     <asp:Label ID="lblCLID" runat="server" Text='<%# Eval("CL_ID") %>'></asp:Label>
                                 </ItemTemplate>
                             </asp:TemplateField>
                             <asp:BoundField DataField="TL_Task" HeaderText="Task" />
                             <asp:BoundField DataField="STL_SubTask" HeaderText="Subtask" />
                             <asp:TemplateField>
                                 <ItemTemplate>
                                     <asp:LinkButton ID="lnkSelect" runat="server" onclick="lnkSelect_Click">Clock-In</asp:LinkButton>
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
</div>

<asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
 
  <ContentTemplate>
 
<div class="row">
     <div class="col-md-12 col-sm-12 col-xs-12">
        <div class="panel panel-default">
           <div class="panel-heading">
               <strong>Completed Tasks - <asp:TextBox ID="txtDate" runat="server" 
                   ClientIDMode="Static" class="txtDate tb10"></asp:TextBox></strong>
                   <asp:Button ID="Submit" runat="server" Text="Submit" onclick="Submit_Click" class="btn btn-primary" />
           </div>
           <div class="panel-body">
              <div class="col-lg-12">
                 <div class="table-responsive">
       <asp:Panel ID="Panel2" runat="server" Width="100%" Height="400px" ScrollBars="Both">
         <asp:GridView ID="gvRecords" runat="server" AutoGenerateColumns="False" 
            onrowcancelingedit="gvRecords_RowCancelingEdit" 
            onrowdatabound="gvRecords_RowDataBound" 
            onrowediting="gvRecords_RowEditing" 
            onrowupdating="gvRecords_RowUpdating" Width="100%" 
            DataKeyNames="R_ID" class="table table-striped table-bordered table-hover">
            <Columns>
                <asp:TemplateField HeaderText="Employee">
                    <ItemTemplate>
                        <asp:Label ID="lblUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                    </ItemTemplate>
                   
                    <ItemStyle HorizontalAlign="Left" />
                   
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Client">
                    <ItemTemplate>
                        <asp:Label ID="lblClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Label ID="lblEditClient" runat="server" Text='<%# Eval("CL_ClientName") %>' Visible ="false"></asp:Label>
                        <asp:DropDownList ID="ddlGvClient" runat="server" Width="200px" AutoPostBack="True" onselectedindexchanged="ddlGvClient_SelectedIndexChanged">
                        </asp:DropDownList>
                    </EditItemTemplate>
                   
                    <ItemStyle HorizontalAlign="Left" />
                   
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Task">
                    <ItemTemplate>
                    <asp:Label ID="lblTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>  
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Label ID="lblEditTask" runat="server" Text='<%# Eval("TL_Task") %>' Visible="false"></asp:Label>
                        <asp:DropDownList ID="ddlGvTask" runat="server" Width="200px" 
                            AutoPostBack="True" onselectedindexchanged="ddlGvTask_SelectedIndexChanged">
                        </asp:DropDownList>
                    </EditItemTemplate>
                   
                    <ItemStyle HorizontalAlign="Left" />
                   
                </asp:TemplateField>
                <asp:TemplateField HeaderText="SubTask">
                    <ItemTemplate>
                        <asp:Label ID="lblSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:Label ID="lblEditSubtask" runat="server" Text='<%# Eval("STL_SubTask") %>' Visible="false"></asp:Label>
                        <asp:DropDownList ID="ddlGvSubTask" runat="server" Width="200px" 
                            AutoPostBack="True">
                        </asp:DropDownList>
                    </EditItemTemplate>
                   <ItemStyle HorizontalAlign="Left" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Duration">
                    <ItemTemplate>
                        <asp:Label ID="lblDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Left" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comments">
                    <ItemTemplate>
                        <asp:Label ID="lblComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                    </ItemTemplate>
                   
                    <EditItemTemplate>
                        <asp:TextBox ID="txtComments" runat="server" Text='<%# Eval("R_Comments") %>' 
                            Width="150px"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemStyle HorizontalAlign="Left" />
                   
                </asp:TemplateField>
               
                <asp:CommandField HeaderText="Edit" ShowEditButton="True" >
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Left" />
                </asp:CommandField>
                <asp:TemplateField HeaderText="Insert">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkInsert" runat="server" onclick="lnkInsert_Click">Split</asp:LinkButton>
                    </ItemTemplate>
                  
                    <ItemStyle HorizontalAlign="Left" />
                  
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        </asp:Panel>
     </div>
              </div>
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
                     Client:
                </td>
                <td style="text-align: left" colspan="2">
                    <asp:DropDownList ID="ddlNewClient" runat="server" Width="200px" AutoPostBack="True" 
                        onselectedindexchanged="ddlNewClient_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
               
            </tr>
            <tr>
                <td style="text-align: right">
                    Task:
                </td>
                <td style="text-align: left" colspan="2">
                    <asp:DropDownList ID="ddlNewTask" runat="server" AutoPostBack="True" 
                        onselectedindexchanged="ddlNewTask_SelectedIndexChanged" Width="200px">
                    </asp:DropDownList>
                </td>
                
            </tr>
            <tr>
                <td style="text-align: right">
                   Sub Task:
                </td>
                <td style="text-align: left" colspan="2">
                    <asp:DropDownList ID="ddlNewSubTask" runat="server" Width="200px">
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
               <td style="text-align: right"></td>
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
                    <asp:Label ID="lblEmpID" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblUserName" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="Label1" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblEndTime" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblOldRId" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblOldDuration" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblProcess" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblStartTime1" runat="server" Text="" Visible="false"></asp:Label>
                </td>                
            </tr>
        </table>       
        </div>
        <div class="footer" align="right">
            <asp:Button ID="btnYes" runat="server" Text="Insert" CssClass="yes" 
                onclick="btnYes_Click" OnClientClick="this.disabled = true; this.value = 'Please wait...';" 
                         UseSubmitBehavior="false" />
            <asp:Button ID="btnNo" runat="server" Text="Cancel" CssClass="no" 
                onclick="btnNo_Click" />
        </div>
    </div>
            </div>
        </div>
    </div>
</div>
 </ContentTemplate>
   </asp:UpdatePanel>
     <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        var jQuery_1_8_3 = $.noConflict(true);
    </script>
<script type="text/javascript" src="Scripts/quicksearch.js"></script>
<script type="text/javascript">
    function pageLoad(sender, args) {
        if (args.get_isPartialLoad()) {
            var min = '<%=datePickerMinDate %>';
            $(".txtDate").datepicker({ maxDate: new Date(), minDate: min });

            $('.search_textbox').each(function (i) {
                $(this).quicksearch("[id*=gvClients] tr:not(:has(th))", {
                    'testQuery': function (query, txt, row) {
                        return $(row).children(":eq(" + i + ")").text().toLowerCase().indexOf(query[0].toLowerCase()) != -1;
                    }
                });
            });
        }
    }
    $(function () {
        var min = '<%=datePickerMinDate %>';
        $(".txtDate").datepicker({ maxDate: new Date(), minDate: min });
    });

    $(function () {
        $('.search_textbox').each(function (i) {
            $(this).quicksearch("[id*=gvClients] tr:not(:has(th))", {
                'testQuery': function (query, txt, row) {
                    return $(row).children(":eq(" + i + ")").text().toLowerCase().indexOf(query[0].toLowerCase()) != -1;
                }
            });
        });
    });
  </script>
</form>

</asp:Content>