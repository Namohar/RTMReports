<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"  CodeFile="RecordsWeeklyView.aspx.cs" Inherits="RecordsWeeklyView" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">

      
<link href="PivotStyle.css" rel="Stylesheet" type="text/css" />
 <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        var jQuery_1_8_3 = $.noConflict(true);
    </script>
    
    
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

  <script language="javascript" type="text/javascript">
      function MakeStaticHeader(gridId, height, width, headerHeight, isFooter) {
          var tbl = document.getElementById(gridId);
          if (tbl) {
              var DivHR = document.getElementById('DivHeaderRow');
              var DivMC = document.getElementById('DivMainContent');
              var DivFR = document.getElementById('DivFooterRow');

              //*** Set divheaderRow Properties ****
              DivHR.style.height = headerHeight + 'px';
              DivHR.style.width = (parseInt(width) - 16) + 'px';
              DivHR.style.position = 'relative';
              DivHR.style.top = '0px';
              DivHR.style.zIndex = '10';
              DivHR.style.verticalAlign = 'top';

              //*** Set divMainContent Properties ****
              DivMC.style.width = width + 'px';
              DivMC.style.height = height + 'px';
              DivMC.style.position = 'relative';
              DivMC.style.top = -headerHeight + 'px';
              DivMC.style.zIndex = '1';

              //*** Set divFooterRow Properties ****
              DivFR.style.width = (parseInt(width) - 16) + 'px';
              DivFR.style.position = 'relative';
              DivFR.style.top = -headerHeight + 'px';
              DivFR.style.verticalAlign = 'top';
              DivFR.style.paddingtop = '2px';

              if (isFooter) {
                  var tblfr = tbl.cloneNode(true);
                  tblfr.removeChild(tblfr.getElementsByTagName('tbody')[0]);
                  var tblBody = document.createElement('tbody');
                  tblfr.style.width = '100%';
                  tblfr.cellSpacing = "0";
                  tblfr.border = "0px";
                  tblfr.rules = "none";
                  //*****In the case of Footer Row *******
                  tblBody.appendChild(tbl.rows[tbl.rows.length - 1]);
                  tblfr.appendChild(tblBody);
                  DivFR.appendChild(tblfr);
              }
              //****Copy Header in divHeaderRow****
              DivHR.appendChild(tbl.cloneNode(true));
          }
      }

</script>

   <script type="text/javascript">
       $(document).ready(function () {
           var days = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday'];
           var dayName = days[new Date().getDay()];
           var selectedTab = $("#<%=hfTab.ClientID%>");
           var tabId = selectedTab.val() != "" ? selectedTab.val() : dayName;
           $('#dvTab a[href="#' + tabId + '"]').tab('show');
           $("#dvTab a").click(function () {
               selectedTab.val($(this).attr("href").substring(1));
           });
        });
    </script> 

<style type="text/css">
      .tb10 {
	background-image:url(images/form_bg.jpg);
	background-repeat:repeat-x;
	border:1px solid #d1c7ac;
	width: 230px;
	color:#333333;
	padding:3px;
	margin-right:4px;
	margin-bottom:8px;
	font-family:tahoma, arial, sans-serif;
}
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
    .styled-button-2 {
	-webkit-box-shadow:rgba(0,0,0,0.2) 0 1px 0 0;
	-moz-box-shadow:rgba(0,0,0,0.2) 0 1px 0 0;
	
	border-bottom-color:#333;
	border:1px solid #61c4ea;
	background-color:#7cceee;
	
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form id="Form1" runat="server">
    
    <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Records Weekly View</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Records Weekly View</li>
			</ol>
	 </div>     
    </div>
    
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" CombineScripts="false">
    
    </asp:ToolkitScriptManager>
   <asp:UpdateProgress id="updateProgress" runat="server">
    <ProgressTemplate>
        <div style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.7;">
            <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/pleasewait.gif" AlternateText="Please wait ..." ToolTip="Please wait ..." style="padding: 10px;position:fixed;top:45%;left:50%;" />
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
     <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
                <div class="col-lg-12">
                    <div class="form-group">
                    <asp:UpdatePanel ID="upnlSelection" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False" >
                    <Triggers>
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlEmployee" EventName="SelectedIndexChanged" />--%>
                        <asp:PostBackTrigger ControlID="ddlEmployee" /> 
                        <asp:PostBackTrigger ControlID="ddlTeam" /> 
                    </Triggers>
                    <ContentTemplate>
                     <strong>Select Team:</strong>
                     <asp:DropDownList ID="ddlTeam" runat="server" 
                         AutoPostBack="True" onselectedindexchanged="ddlTeam_SelectedIndexChanged" class="tb10">
                     </asp:DropDownList>
                     <strong>Employee:</strong>
                     <asp:DropDownList ID="ddlEmployee" runat="server" AutoPostBack="True" 
                           onselectedindexchanged="ddlEmployee_SelectedIndexChanged" class="tb10">
                     </asp:DropDownList>
                     <br />
                     <br />
                     <div id="dvDate" runat="server" visible="false">
                         <strong>From Date:</strong>
                           <asp:TextBox ID="txtFrom" runat="server" ClientIDMode="Static" 
                             class="txtSunDate tb10" AutoPostBack="True" 
                             ontextchanged="txtFrom_TextChanged" ></asp:TextBox>
                         <%--<strong>To Date:</strong>--%>
                           <asp:TextBox ID="txtTo" runat="server" ClientIDMode="Static" class="txtDate tb10" Visible="false" ></asp:TextBox>
                           <asp:Button ID="btnView" runat="server" Text="View" class="btn btn-primary" 
                               onclick="btnView_Click" Visible="false" />
                               <asp:HiddenField ID="hfCustomerId" runat="server" Value="0" />                               
                    </div>
                    </ContentTemplate>
                    </asp:UpdatePanel>
                   </div>
                </div>
                <asp:UpdatePanel ID="UpdatePanel9" runat="server" >
                 <ContentTemplate>
                      <div class="row" id="dvWeekView" runat="server">
                           <div class="col-md-12 col-sm-12 col-xs-12">
                               <div class="panel panel-default">
                                  <div class="panel-body">
                                    <div class="col-lg-10">
                                     <div id="div2" runat="server" visible= "false" align="center">
                                        <asp:ImageButton ID="btnPrev" runat="server" ToolTip="Previous week" ImageUrl="~/images/Previous Week.png" 
                                            Width="30px" onclick="btnPrev_Click" /> 
                                        <asp:Label ID="lblHeading" runat="server" 
                                          Text="" Font-Bold="True" style="vertical-align:top" 
                                          Font-Size="Medium" ForeColor="#003399" Visible ="False"></asp:Label>
                                        <asp:ImageButton ID="btnNext" runat="server" ToolTip="Next week" ImageUrl="~/images/Next week.png" Width="30px" onclick="btnNext_Click" /><br />
                                        <asp:Label ID="lblNoData" runat="server" Text=""></asp:Label>
                                        <br />
                                        <asp:GridView ID="gvWeeklyView" runat="server" AllowSorting="False" onsorting="gvWeeklyView_Sorting" class="table table-striped table-bordered table-hover" Width="100%">
                                        </asp:GridView>
                                        
                                     </div>                                        
                                    </div>

                                     <div class="col-lg-2">
                                         <asp:HiddenField ID="hfDayCount" runat="server" Value="0" />
                                        <asp:HiddenField ID="hfLeaveCount" runat="server" Value="0" />
                                         <br />
                                         <br />
                                         <asp:Button ID="btnApprove" runat="server" Text="Approve" Width="100px" 
                                             class="btn btn-success" onclick="btnApprove_Click" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false" />
                                         
                                         <asp:Button ID="btnReject" runat="server" Text="Reject" class="btn btn-warning" Width="100px" 
                                             onclick="btnReject_Click" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false" />
                                          
                                          <asp:Button ID="btnBack" runat="server" Text="Approval Page" class="btn btn-primary" Width="120px" 
                                             onclick="btnBack_Click" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false" />
                                          
                                         <asp:Button ID="btnSubmit" runat="server" Text="Submit Week" 
                                             class="btn btn-primary" onclick="btnSubmit_Click" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false" />
                                     </div>
                                  </div>
                              </div>
                           </div>
                        </div>
                 </ContentTemplate>
                 </asp:UpdatePanel>
                 <div class="col-md-12 col-sm-12" id="dvWeek" runat="server" visible="false">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                           <asp:Panel ID="pnlHeading" runat="server">
                                Weekly View &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                <asp:LinkButton ID="lnkCopyLastWeek" runat="server" 
                                    onclick="lnkCopyLastWeek_Click">Copy last week data</asp:LinkButton>
                            </asp:Panel>
                        </div>
                        <div id="dvTab" class="panel-body">
                            <ul class="nav nav-tabs">
                                <li class="active"><a href="#sunday" data-toggle="tab">Sunday</a>
                                </li>
                                <li class=""><a href="#monday" data-toggle="tab">Monday</a>
                                </li>
                                <li class=""><a href="#tuesday" data-toggle="tab">Tuesday</a>
                                </li>
                                <li class=""><a href="#wednesday" data-toggle="tab">Wednesday</a>
                                </li>
                                <li class=""><a href="#thursday" data-toggle="tab">Thursday</a>
                                </li>
                                <li class=""><a href="#friday" data-toggle="tab">Friday</a>
                                </li>
                                <li class=""><a href="#saturday" data-toggle="tab">Saturday</a>
                                </li>
                            </ul>

                            <div class="tab-content">
                                <div class="tab-pane fade active in" id="sunday">
                                    <asp:UpdatePanel ID="upnlSun" runat="server" UpdateMode="Conditional">
                                       <ContentTemplate>
                                          <asp:Panel ID="pnlSunday" runat="server" ScrollBars="Horizontal">
                                           <h4><asp:Label ID="lblSunday" runat="server" Text=""></asp:Label></h4>
                                            
                                           <asp:Button ID="btnAddSun" runat="server" Text="Add" 
                                               onclick="btnAddSun_Click" class="btn btn-primary" Visible="false" />
                                           <asp:Button ID="btnCopySun" runat="server" Text="Copy To" class="btn btn-primary" 
                                               onclick="btnCopySun_Click" />
                                           <br />
                                           <br />
                                           
                                           <asp:GridView ID="gvRecordsSunday" runat="server" AutoGenerateColumns="False" AllowSorting="false" onsorting="gvRecordsSunday_Sorting" HeaderStyle-ForeColor="#428bca"
                                                Width="100%" 
                                                DataKeyNames="R_ID" 
                                               class="table table-striped table-bordered table-hover" 
                                               onrowcancelingedit="gvRecordsSunday_RowCancelingEdit" 
                                               onrowdatabound="gvRecordsSunday_RowDataBound" 
                                               onrowediting="gvRecordsSunday_RowEditing" 
                                               onrowupdating="gvRecordsSunday_RowUpdating" 
                                               onrowdeleting="gvRecordsSunday_RowDeleting" ShowFooter="true" 
                                               EmptyDataText="No records has been added." 
                                               ondatabound="gvRecordsSunday_DataBound" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Employee">
                                                        <FooterTemplate>
                                                            <asp:Label ID="lblSADDUser" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                                                        </ItemTemplate>                   
                                                        <ItemStyle HorizontalAlign="Left" />               
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Client" SortExpression="CL_ClientName">
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtSearchGvClient" runat="server" ClientIDMode="Static" 
                                                                AutoPostBack="True" ontextchanged="txtSearchGvClient_TextChanged"></asp:TextBox>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:Label ID="lblSEditClient" runat="server" Text='<%# Eval("CL_ClientName") %>' Visible ="false"></asp:Label>
                                                            <asp:DropDownList ID="ddlSGvClient" runat="server" Width="120px" onselectedindexchanged="ddlSGvClient_SelectedIndexChanged" AutoPostBack="True">
                                                            </asp:DropDownList>
                                                        </EditItemTemplate>                   
                                                        <ItemStyle HorizontalAlign="Left" />                   
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Task" SortExpression="TL_Task">
                                                        <FooterTemplate>
                                                            <asp:DropDownList ID="ddlSAddTask" runat="server" onselectedindexchanged="ddlSAddTask_SelectedIndexChanged" AutoPostBack="True" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                        <asp:Label ID="lblSTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>  
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:Label ID="lblSEditTask" runat="server" Text='<%# Eval("TL_Task") %>' Visible="false"></asp:Label>
                                                            <asp:DropDownList ID="ddlSGvTask" runat="server" Width="120px" onselectedindexchanged="ddlSGvTask_SelectedIndexChanged"
                                                                AutoPostBack="True" >
                                                            </asp:DropDownList>
                                                        </EditItemTemplate>                   
                                                        <ItemStyle HorizontalAlign="Left" />                   
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SubTask" SortExpression="STL_SubTask">
                                                        <FooterTemplate>
                                                            <asp:DropDownList ID="ddlSADDSubTask" runat="server" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:Label ID="lblSEditSubtask" runat="server" Text='<%# Eval("STL_SubTask") %>' Visible="false"></asp:Label>
                                                            <asp:DropDownList ID="ddlSGvSubTask" runat="server" Width="120px" 
                                                                AutoPostBack="True">
                                                            </asp:DropDownList>
                                                        </EditItemTemplate>
                                                       <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Duration">
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtSADDDuration" runat="server" Width="60px"></asp:TextBox>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                                                        </ItemTemplate>
                                                         <EditItemTemplate>
                                                            <asp:TextBox ID="txtSDuration" runat="server" Text='<%# Eval("R_Duration") %>' 
                                                                Width="80px"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Comments">
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtSGVAddComments" runat="server" Width="100px" TextMode="MultiLine"></asp:TextBox>                                                            
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                                                        </ItemTemplate>
                   
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtSComments" runat="server" Text='<%# Eval("R_Comments") %>' 
                                                                Width="150px"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" />                   
                                                    </asp:TemplateField>
               
                                                    <asp:CommandField HeaderText="Edit" ShowEditButton="True" >
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    </asp:CommandField>
                                                    <asp:CommandField HeaderText="Delete" ShowDeleteButton="true" Visible="false"  />
                                                    <asp:TemplateField Visible="true">
                                                        <FooterTemplate>                                    
                                                            <asp:Button ID="btnSGVAdd" runat="server" Text="Add" 
                                                                onclick="btnSGVAdd_Click" class="btn btn-primary" OnClientClick="this.disabled = true; this.value = 'Adding...';" UseSubmitBehavior="false" />
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkSunDelete" runat="server" onclick="lnkSunDelete_Click">Delete</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>  
                                            </asp:Panel>                                         
                                       </ContentTemplate>
                                    </asp:UpdatePanel>                                   
                                </div>
                                <div class="tab-pane fade" id="monday">
                                    <asp:UpdatePanel ID="upnlMon" runat="server" UpdateMode="Conditional">
                                     <ContentTemplate>
                                        <asp:Panel ID="pnlMonday" runat="server" ScrollBars="Horizontal" >
                                         <h4><asp:Label ID="lblMonday" runat="server" Text=""></asp:Label></h4>
                                         
                                           <asp:Button ID="btnAddMon" onclick="btnAddMon_Click" class="btn btn-primary" runat="server" Text="Add" Visible="false" />
                                           <asp:Button ID="btnCopyMon" runat="server" Text="Copy To" class="btn btn-primary" onclick="btnCopyMon_Click" />
                                           <br />
                                           <br />
                                          <%-- <div id="DivRoot" align="left">
                                            <div style="overflow: hidden;" id="DivHeaderRow">
                                            </div>
                                            <div style="overflow:auto;" onscroll="OnScrollDiv(this)" id="DivMainContent" >--%>
                                         <asp:GridView ID="gvRecordsMonday" runat="server" AutoGenerateColumns="False" AllowSorting="false" onsorting="gvRecordsMonday_Sorting"
                                            Width="100%" onrowcancelingedit="gvRecordsMonday_RowCancelingEdit"  HeaderStyle-ForeColor="#428bca"
                                               onrowdatabound="gvRecordsMonday_RowDataBound" 
                                               onrowediting="gvRecordsMonday_RowEditing" onrowupdating="gvRecordsMonday_RowUpdating" onrowdeleting="gvRecordsMonday_RowDeleting"
                                            DataKeyNames="R_ID" class="table table-striped table-bordered table-hover" ShowFooter="true">
                                            <%--<HeaderStyle CssClass="GVFixedHeader" />
                                            <FooterStyle CssClass="GVFixedFooter" />--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Employee" SortExpression="R_User_Name">
                                                    <FooterTemplate>
                                                            <asp:Label ID="lblMADDUser" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Client" SortExpression="CL_ClientName">
                                                    <FooterTemplate>
                                                         <asp:TextBox ID="txtSearchGvClientM" runat="server" ClientIDMode="Static" AutoPostBack="True" ontextchanged="txtSearchGvClientM_TextChanged"></asp:TextBox>
                                                    </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblMEditClient" runat="server" Text='<%# Eval("CL_ClientName") %>' Visible ="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlMGvClient" runat="server" Width="120px" onselectedindexchanged="ddlMGvClient_SelectedIndexChanged" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Task" SortExpression="TL_Task">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlMAddTask" runat="server" onselectedindexchanged="ddlMAddTask_SelectedIndexChanged" AutoPostBack="True" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblMTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>  
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblMEditTask" runat="server" Text='<%# Eval("TL_Task") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlMGvTask" runat="server" Width="120px" onselectedindexchanged="ddlMGvTask_SelectedIndexChanged"
                                                            AutoPostBack="True" >
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SubTask" SortExpression="STL_SubTask">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlMADDSubTask" runat="server" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblMEditSubtask" runat="server" Text='<%# Eval("STL_SubTask") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlMGvSubTask" runat="server" Width="120px" 
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                                                   <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Duration">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtMADDDuration" runat="server" Width="60px"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                            <asp:TextBox ID="txtMDuration" runat="server" Text='<%# Eval("R_Duration") %>' 
                                                                Width="80px"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Comments">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtMGVAddComments" runat="server" Width="100px" TextMode="MultiLine"></asp:TextBox>                                                            
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtMComments" runat="server" Text='<%# Eval("R_Comments") %>' 
                                                            Width="150px"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
               
                                                <asp:CommandField HeaderText="Edit" ShowEditButton="True" >
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="70px" />
                                                </asp:CommandField>
                                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" Visible="false" />
                                                <asp:TemplateField Visible="true">
                                                        <FooterTemplate>                                    
                                                            <asp:Button ID="btnMGVAdd" runat="server" Text="Add" onclick="btnMGVAdd_Click" class="btn btn-primary" OnClientClick="this.disabled = true; this.value = 'Adding...';" UseSubmitBehavior="false" />
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkMonDelete" runat="server" onclick="lnkMonDelete_Click">Delete</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <%--</div>
                                        <div id="DivFooterRow" style="overflow:hidden">
                                        </div>
                                        </div>--%>
                                        </asp:Panel>
                                     </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <div class="tab-pane fade" id="tuesday">
                                    <asp:UpdatePanel ID="upnlTue" runat="server" UpdateMode="Conditional">
                                      <ContentTemplate>
                                         <asp:Panel ID="pnlTuesday" runat="server" ScrollBars="Horizontal">
                                          <h4><asp:Label ID="lblTuesday" runat="server" Text=""></asp:Label></h4>
                                          
                                           <asp:Button ID="btnAddTue" onclick="btnAddTue_Click" class="btn btn-primary" runat="server" Text="Add" Visible="false" />
                                           <asp:Button ID="btnCopyTue" runat="server" Text="Copy To" class="btn btn-primary" onclick="btnCopyTue_Click" />
                                           <br />
                                           <br />
                                          <asp:GridView ID="gvRecordsTue" runat="server" AutoGenerateColumns="False" AllowSorting="false" onsorting="gvRecordsTue_Sorting" HeaderStyle-ForeColor="#428bca"
                                            Width="100%" onrowcancelingedit="gvRecordsTue_RowCancelingEdit" 
                                               onrowdatabound="gvRecordsTue_RowDataBound" 
                                               onrowediting="gvRecordsTue_RowEditing" onrowupdating="gvRecordsTue_RowUpdating" onrowdeleting="gvRecordsTue_RowDeleting"
                                            DataKeyNames="R_ID" class="table table-striped table-bordered table-hover" ShowFooter="true">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Employee">
                                                    <FooterTemplate>
                                                            <asp:Label ID="lblTADDUser" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Client" SortExpression="CL_ClientName">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtSearchGvClientT" runat="server" ClientIDMode="Static"  AutoPostBack="True" ontextchanged="txtSearchGvClientT_TextChanged"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblTEditClient" runat="server" Text='<%# Eval("CL_ClientName") %>' Visible ="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlTGvClient" runat="server" Width="120px" onselectedindexchanged="ddlTGvClient_SelectedIndexChanged" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Task" SortExpression="TL_Task">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlTAddTask" runat="server" onselectedindexchanged="ddlTAddTask_SelectedIndexChanged" AutoPostBack="True" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblTTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label> 
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblTEditTask" runat="server" Text='<%# Eval("TL_Task") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlTGvTask" runat="server" Width="120px" onselectedindexchanged="ddlTGvTask_SelectedIndexChanged"
                                                            AutoPostBack="True" >
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SubTask" SortExpression="STL_SubTask">
                                                   <FooterTemplate>
                                                            <asp:DropDownList ID="ddlTADDSubTask" runat="server" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblTEditSubtask" runat="server" Text='<%# Eval("STL_SubTask") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlTGvSubTask" runat="server" Width="120px" 
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                                                   <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Duration">
                                                   <FooterTemplate>
                                                            <asp:TextBox ID="txtTADDDuration" runat="server" Width="60px"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                            <asp:TextBox ID="txtTDuration" runat="server" Text='<%# Eval("R_Duration") %>' 
                                                                Width="60px"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Comments">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtTGVAddComments" runat="server" Width="100px" TextMode="MultiLine"></asp:TextBox>                                                            
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtTComments" runat="server" Text='<%# Eval("R_Comments") %>' 
                                                            Width="150px"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
               
                                                <asp:CommandField HeaderText="Edit" ShowEditButton="True" >
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" />
                                                </asp:CommandField>
                                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" Visible="false" />
                                                <asp:TemplateField Visible="true">
                                                        <FooterTemplate>                                    
                                                            <asp:Button ID="btnTGVAdd" runat="server" Text="Add" onclick="btnTGVAdd_Click" class="btn btn-primary" OnClientClick="this.disabled = true; this.value = 'Adding...';" UseSubmitBehavior="false" />
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkTueDelete" runat="server" onclick="lnkTueDelete_Click">Delete</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        </asp:Panel>
                                      </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <div class="tab-pane fade" id="wednesday">
                                    <asp:UpdatePanel ID="upnlWed" runat="server" UpdateMode="Conditional">
                                      <ContentTemplate>
                                        <asp:Panel ID="pnlWednessday" runat="server" ScrollBars="Horizontal">
                                         <h4><asp:Label ID="lblWed" runat="server" Text=""></asp:Label></h4>
                                         
                                           <asp:Button ID="btnAddWed" onclick="btnAddWed_Click" class="btn btn-primary" runat="server" Text="Add" Visible="false" />
                                           <asp:Button ID="btnCopyWed" runat="server" Text="Copy To" class="btn btn-primary" onclick="btnCopyWed_Click" />
                                           <br />
                                           <br />
                                         <asp:GridView ID="gvRecordsWed" runat="server" AutoGenerateColumns="False" AllowSorting="false" onsorting="gvRecordsWed_Sorting" HeaderStyle-ForeColor="#428bca" 
                                             Width="100%" onrowcancelingedit="gvRecordsWed_RowCancelingEdit" 
                                               onrowdatabound="gvRecordsWed_RowDataBound" ShowFooter="true"
                                               onrowediting="gvRecordsWed_RowEditing" onrowupdating="gvRecordsWed_RowUpdating" onrowdeleting="gvRecordsWed_RowDeleting"
                                            DataKeyNames="R_ID" class="table table-striped table-bordered table-hover">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Employee">
                                                    <FooterTemplate>
                                                            <asp:Label ID="lblWADDUser" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Client" SortExpression="CL_ClientName">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtSearchGvClientW" runat="server" ClientIDMode="Static" AutoPostBack="True" ontextchanged="txtSearchGvClientW_TextChanged"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblWEditClient" runat="server" Text='<%# Eval("CL_ClientName") %>' Visible ="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlWGvClient" runat="server" Width="120px" onselectedindexchanged="ddlWGvClient_SelectedIndexChanged" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Task" SortExpression="TL_Task">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlWAddTask" runat="server" onselectedindexchanged="ddlWAddTask_SelectedIndexChanged" AutoPostBack="True" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblWTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>  
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblWEditTask" runat="server" Text='<%# Eval("TL_Task") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlWGvTask" runat="server" Width="120px" onselectedindexchanged="ddlWGvTask_SelectedIndexChanged"
                                                            AutoPostBack="True" >
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SubTask" SortExpression="STL_SubTask">
                                                   <FooterTemplate>
                                                            <asp:DropDownList ID="ddlWADDSubTask" runat="server" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblWEditSubtask" runat="server" Text='<%# Eval("STL_SubTask") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlWGvSubTask" runat="server" Width="120px" 
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                                                   <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Duration">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtWADDDuration" runat="server" Width="60px"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                            <asp:TextBox ID="txtWDuration" runat="server" Text='<%# Eval("R_Duration") %>' 
                                                                Width="80px"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Comments">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtWGVAddComments" runat="server" Width="100px" TextMode="MultiLine"></asp:TextBox>                                                            
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtWComments" runat="server" Text='<%# Eval("R_Comments") %>' 
                                                            Width="150px"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
               
                                                <asp:CommandField HeaderText="Edit" ShowEditButton="True" >
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" />
                                                </asp:CommandField>
                                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" Visible="false" />
                                                <asp:TemplateField Visible="true">
                                                        <FooterTemplate>                                    
                                                            <asp:Button ID="btnWGVAdd" runat="server" Text="Add" onclick="btnWGVAdd_Click" class="btn btn-primary" OnClientClick="this.disabled = true; this.value = 'Adding...';" UseSubmitBehavior="false" />
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkWedDelete" runat="server" onclick="lnkWedDelete_Click">Delete</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        </asp:Panel>
                                      </ContentTemplate>
                                    </asp:UpdatePanel>                                   
                                </div>
                                 <div class="tab-pane fade" id="thursday">
                                  <asp:UpdatePanel ID="upnlThu" runat="server" UpdateMode="Conditional">
                                      <ContentTemplate>
                                       <asp:Panel ID="pnlThursday" runat="server" ScrollBars="Horizontal">
                                        <h4><asp:Label ID="lblThu" runat="server" Text=""></asp:Label></h4>
                                        
                                           <asp:Button ID="btnAddThu" onclick="btnAddThu_Click" class="btn btn-primary" Visible="false" runat="server" Text="Add" />
                                           <asp:Button ID="btnCopyThu" runat="server" Text="Copy To" class="btn btn-primary" onclick="btnCopyThu_Click" />
                                           <br />
                                           <br />
                                        <asp:GridView ID="gvRecordsThu" runat="server" AutoGenerateColumns="False" AllowSorting="false" onsorting="gvRecordsThu_Sorting" HeaderStyle-ForeColor="#428bca" 
                                             Width="100%" onrowcancelingedit="gvRecordsThu_RowCancelingEdit" 
                                               onrowdatabound="gvRecordsThu_RowDataBound" ShowFooter="true"
                                               onrowediting="gvRecordsThu_RowEditing" onrowupdating="gvRecordsThu_RowUpdating" onrowdeleting="gvRecordsThu_RowDeleting"
                                            DataKeyNames="R_ID" class="table table-striped table-bordered table-hover">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Employee">
                                                    <FooterTemplate>
                                                            <asp:Label ID="lblTHADDUser" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTHUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Client" SortExpression="CL_ClientName">
                                                   <FooterTemplate>
                                                            <asp:TextBox ID="txtSearchGvClientTH" runat="server" ClientIDMode="Static" AutoPostBack="True" ontextchanged="txtSearchGvClientTH_TextChanged"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTHClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblTHEditClient" runat="server" Text='<%# Eval("CL_ClientName") %>' Visible ="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlTHGvClient" runat="server" Width="120px" onselectedindexchanged="ddlTHGvClient_SelectedIndexChanged" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Task" SortExpression="TL_Task">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlTHAddTask" runat="server" onselectedindexchanged="ddlTHAddTask_SelectedIndexChanged" AutoPostBack="True" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblTHTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>  
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblTHEditTask" runat="server" Text='<%# Eval("TL_Task") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlTHGvTask" runat="server" Width="120px" onselectedindexchanged="ddlTHGvTask_SelectedIndexChanged"
                                                            AutoPostBack="True" >
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SubTask" SortExpression="STL_SubTask">
                                                   <FooterTemplate>
                                                            <asp:DropDownList ID="ddlTHADDSubTask" runat="server" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTHSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblTHEditSubtask" runat="server" Text='<%# Eval("STL_SubTask") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlTHGvSubTask" runat="server" Width="120px" 
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                                                   <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Duration">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtTHADDDuration" runat="server" Width="60px"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTHDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                            <asp:TextBox ID="txtTHDuration" runat="server" Text='<%# Eval("R_Duration") %>' 
                                                                Width="80px"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Comments">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtTHGVAddComments" runat="server" Width="100px" TextMode="MultiLine"></asp:TextBox>                                                            
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTHComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtTHComments" runat="server" Text='<%# Eval("R_Comments") %>' 
                                                            Width="150px"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
               
                                                <asp:CommandField HeaderText="Edit" ShowEditButton="True" >
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" />
                                                </asp:CommandField>
                                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" Visible="false" />
                                                <asp:TemplateField Visible="true">
                                                        <FooterTemplate>                                    
                                                            <asp:Button ID="btnTHGVAdd" runat="server" Text="Add" onclick="btnTHGVAdd_Click" class="btn btn-primary" OnClientClick="this.disabled = true; this.value = 'Adding...';" UseSubmitBehavior="false" />
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkThuDelete" runat="server" onclick="lnkThuDelete_Click">Delete</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        </asp:Panel>
                                      </ContentTemplate>
                                    </asp:UpdatePanel>                                   
                                </div>
                                 <div class="tab-pane fade" id="friday">
                                  <asp:UpdatePanel ID="upnlFri" runat="server" UpdateMode="Conditional">
                                      <ContentTemplate>
                                      <asp:Panel ID="pnlFriday" runat="server" ScrollBars="Horizontal">
                                        <h4><asp:Label ID="lblFriday" runat="server" Text=""></asp:Label></h4>
                                        
                                           <asp:Button ID="btnAddFri" onclick="btnAddFri_Click" Visible="false" class="btn btn-primary" runat="server" Text="Add" />
                                           <asp:Button ID="btnCopyFri" runat="server" Text="Copy To" class="btn btn-primary" onclick="btnCopyFri_Click" />
                                           <br />
                                           <br />
                                        <asp:GridView ID="gvRecordsFri" runat="server" AutoGenerateColumns="False" AllowSorting="false" onsorting="gvRecordsFri_Sorting" HeaderStyle-ForeColor="#428bca" 
                                            Width="100%" onrowcancelingedit="gvRecordsFri_RowCancelingEdit" 
                                               onrowdatabound="gvRecordsFri_RowDataBound" ShowFooter="true"
                                               onrowediting="gvRecordsFri_RowEditing" onrowupdating="gvRecordsFri_RowUpdating" onrowdeleting="gvRecordsFri_RowDeleting"
                                            DataKeyNames="R_ID" class="table table-striped table-bordered table-hover">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Employee">
                                                    <FooterTemplate>
                                                            <asp:Label ID="lblFADDUser" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Client" SortExpression="CL_ClientName">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtSearchGvClientF" runat="server" ClientIDMode="Static" AutoPostBack="True" ontextchanged="txtSearchGvClientF_TextChanged"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblFEditClient" runat="server" Text='<%# Eval("CL_ClientName") %>' Visible ="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlFGvClient" runat="server" Width="120px" onselectedindexchanged="ddlFGvClient_SelectedIndexChanged" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Task" SortExpression="TL_Task">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlFAddTask" runat="server" onselectedindexchanged="ddlFAddTask_SelectedIndexChanged" AutoPostBack="True" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblFTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>  
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblFEditTask" runat="server" Text='<%# Eval("TL_Task") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlFGvTask" runat="server" Width="120px" onselectedindexchanged="ddlFGvTask_SelectedIndexChanged"
                                                            AutoPostBack="True" >
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SubTask" SortExpression="STL_SubTask">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlFADDSubTask" runat="server" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblFEditSubtask" runat="server" Text='<%# Eval("STL_SubTask") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlFGvSubTask" runat="server" Width="120px" 
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                                                   <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Duration">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtFADDDuration" runat="server" Width="60px"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                            <asp:TextBox ID="txtFDuration" runat="server" Text='<%# Eval("R_Duration") %>' 
                                                                Width="80px"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Comments">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtFGVAddComments" runat="server" Width="100px" TextMode="MultiLine"></asp:TextBox>                                                            
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtFComments" runat="server" Text='<%# Eval("R_Comments") %>' 
                                                            Width="150px"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
               
                                                <asp:CommandField HeaderText="Edit" ShowEditButton="True" >
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" />
                                                </asp:CommandField>
                                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" Visible="false" />
                                                <asp:TemplateField Visible="true">
                                                        <FooterTemplate>                                    
                                                            <asp:Button ID="btnFGVAdd" runat="server" Text="Add" onclick="btnFGVAdd_Click" class="btn btn-primary" OnClientClick="this.disabled = true; this.value = 'Adding...';" UseSubmitBehavior="false" />
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkFriDelete" runat="server" onclick="lnkFriDelete_Click">Delete</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        </asp:Panel>
                                      </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                 <div class="tab-pane fade" id="saturday">
                                  <asp:UpdatePanel ID="upnlSat" runat="server" UpdateMode="Conditional">
                                      <ContentTemplate>
                                       <asp:Panel ID="pnlSaturday" runat="server" ScrollBars="Horizontal">
                                        <h4><asp:Label ID="lblSat" runat="server" Text=""></asp:Label></h4>
                                        
                                           <asp:Button ID="btnAddSat" onclick="btnAddSat_Click" Visible="false" class="btn btn-primary" runat="server" Text="Add" />
                                           <asp:Button ID="btnCopySat" runat="server" Text="Copy To" class="btn btn-primary" onclick="btnCopySat_Click" />
                                           <br />
                                           <br />
                                        <asp:GridView ID="gvRecordsSat" runat="server" AutoGenerateColumns="False" AllowSorting="false" onsorting="gvRecordsSat_Sorting" HeaderStyle-ForeColor="#428bca" 
                                             Width="100%" onrowcancelingedit="gvRecordsSat_RowCancelingEdit" 
                                               onrowdatabound="gvRecordsSat_RowDataBound" ShowFooter="true"
                                               onrowediting="gvRecordsSat_RowEditing" onrowupdating="gvRecordsSat_RowUpdating" onrowdeleting="gvRecordsSat_RowDeleting"
                                            DataKeyNames="R_ID" class="table table-striped table-bordered table-hover">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Employee">
                                                     <FooterTemplate>
                                                            <asp:Label ID="lblSAADDUser" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSAUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Client" SortExpression="CL_ClientName">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtSearchGvClientSA" runat="server" ClientIDMode="Static" AutoPostBack="True" ontextchanged="txtSearchGvClientSA_TextChanged"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSAClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblSAEditClient" runat="server" Text='<%# Eval("CL_ClientName") %>' Visible ="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlSAGvClient" runat="server" Width="120px" onselectedindexchanged="ddlSAGvClient_SelectedIndexChanged" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Task" SortExpression="TL_Task">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlSAAddTask" runat="server" onselectedindexchanged="ddlSAAddTask_SelectedIndexChanged" AutoPostBack="True" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblSATask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>  
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblSAEditTask" runat="server" Text='<%# Eval("TL_Task") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlSAGvTask" runat="server" Width="120px" onselectedindexchanged="ddlSAGvTask_SelectedIndexChanged"
                                                            AutoPostBack="True" >
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                   
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SubTask" SortExpression="STL_SubTask">
                                                    <FooterTemplate>
                                                            <asp:DropDownList ID="ddlSAADDSubTask" runat="server" Width="100px">
                                                            </asp:DropDownList>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSASubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblSAEditSubtask" runat="server" Text='<%# Eval("STL_SubTask") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlSAGvSubTask" runat="server" Width="120px" 
                                                            AutoPostBack="True">
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                                                   <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Duration">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtSAADDDuration" runat="server" Width="60px"></asp:TextBox>
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSADuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                            <asp:TextBox ID="txtSADuration" runat="server" Text='<%# Eval("R_Duration") %>' 
                                                                Width="80px"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Comments">
                                                    <FooterTemplate>
                                                            <asp:TextBox ID="txtSAGVAddComments" runat="server" Width="100px" TextMode="MultiLine"></asp:TextBox>                                                            
                                                        </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSAComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                                                    </ItemTemplate>
                   
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtSAComments" runat="server" Text='<%# Eval("R_Comments") %>' 
                                                            Width="150px"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                   
                                                </asp:TemplateField>
               
                                                <asp:CommandField HeaderText="Edit" ShowEditButton="True" >
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" />
                                                </asp:CommandField>
                                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" Visible="false" />
                                                <asp:TemplateField Visible="true">
                                                        <FooterTemplate>                                    
                                                            <asp:Button ID="btnSAGVAdd" runat="server" Text="Add" onclick="btnSAGVAdd_Click" class="btn btn-primary" OnClientClick="this.disabled = true; this.value = 'Adding...';" UseSubmitBehavior="false" />
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkSatDelete" runat="server" onclick="lnkSatDelete_Click">Delete</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        </asp:Panel>
                                      </ContentTemplate>
                                    </asp:UpdatePanel>                                                                        
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
             </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hfTab" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:Label ID="lblHidden" runat="server" Text=""></asp:Label>

    <asp:ModalPopupExtender ID="mpePopUp" runat="server" TargetControlID="lblHidden" PopupControlID="divPopUp">
    </asp:ModalPopupExtender>
    <div id="divPopUp" class="modalPopup" style="display: none">
    <div class="header">
       Add new record
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
                    <asp:DropDownList ID="ddlNewClient" runat="server" Width="200px">
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
                 <%--<asp:Label ID="lblMainDuration" runat="server" Text=""></asp:Label>--%>
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
                    <asp:Label ID="lblStartTime" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblEndTime" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblOldRId" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblDay" runat="server" Text="" Visible="false"></asp:Label>
                    <asp:Label ID="lblProcess" runat="server" Text="" Visible="false"></asp:Label>
                </td>
                
            </tr>
        </table>     
        </div>
        <div class="footer" align="right">
            <asp:Button ID="btnYes" runat="server" Text="Add" CssClass="yes" 
                onclick="btnYes_Click" />
            <asp:Button ID="btnNo" runat="server" Text="Cancel" CssClass="no" 
                onclick="btnNo_Click" />
        </div>
    </div>

    <asp:Label ID="lblHidden1" runat="server" Text=""></asp:Label>
    <asp:ModalPopupExtender ID="mpeDate" runat="server" TargetControlID="lblHidden1" PopupControlID="divDatePopup">
    </asp:ModalPopupExtender>
    <div id="divDatePopup" class="modalPopup" style="display: none">
        <div class="header">
           Select Date
        </div>
        <div class="body">
            <asp:Label ID="lblCopyFrom" runat="server" Text="" Visible="false"></asp:Label>
           <strong>Select Date:</strong>
            <asp:TextBox ID="txtSelDate" runat="server" class="tb10 txtDate" ClientIDMode="Static"></asp:TextBox>
        </div>
        <div class="footer" align="right">
            <asp:Button ID="btnDateYes" runat="server" Text="Submit" CssClass="yes" 
                onclick="btnDateYes_Click" />
            <asp:Button ID="btmDateNo" runat="server" Text="Cancel" CssClass="no" 
                onclick="btmDateNo_Click" />
        </div>
    </div>

    <asp:Label ID="lblHidden2" runat="server" Text=""></asp:Label>
    <asp:ModalPopupExtender ID="mpeConfirm" runat="server" TargetControlID="lblHidden2" PopupControlID="divConfirmPopUp">
    </asp:ModalPopupExtender>
    <div id="divConfirmPopUp" class="modalPopup" style="display: none">
        <div class="header">
           Confirm
        </div>
        <div class="body">
            <asp:Label ID="lblstatus" runat="server" Text="" Visible="false"></asp:Label>
            <strong>Data already present 
                <asp:Label ID="lblOn" runat="server" Text=""></asp:Label> 
                <asp:Label ID="lblCopyto" runat="server" Text=""></asp:Label>. Do you want to replace?</strong>           
        </div>
        <div class="footer" align="right">
            <asp:Button ID="btnConfirmYes" runat="server" Text="Yes" CssClass="yes" 
                onclick="btnConfirmYes_Click" />
            <asp:Button ID="btnConfirmNo" runat="server" Text="No" CssClass="no" 
                onclick="btnConfirmNo_Click" />
        </div>
    </div>

     <asp:Label ID="lblHidden3" runat="server" Text=""></asp:Label>
    <asp:ModalPopupExtender ID="mpeDuration" runat="server" TargetControlID="lblHidden3" PopupControlID="divDurationPopUp">
    </asp:ModalPopupExtender>
    <div id="divDurationPopUp" class="modalPopup" style="display: none">
        <div class="header">
           Confirm
        </div>
        <div class="body">
             <br />
             <strong>Copy complete data (with duration)? Or Copy only Clients and Task?</strong>
             
        </div>
        <div class="footer" align="right">
             <asp:Button ID="btnWithDuration" runat="server" Text="Complete Data" CssClass="btn btn-primary"
                onclick="btnWithDuration_Click" />   
                &nbsp&nbsp&nbsp&nbsp
                <asp:Button ID="btnWithoutDuration" runat="server" Text="Only Clients/Tasks" CssClass="btn btn-primary" 
                onclick="btnWithoutDuration_Click" /> 
                
                 &nbsp&nbsp&nbsp&nbsp
                <asp:Button ID="btnCancelCopy" runat="server" Text="Cancel" CssClass="btn btn-primary" 
                onclick="btnCancelCopy_Click" />  
        </div>
    </div>

    <asp:Label ID="lblHiddenConfirm" runat="server" Text=""></asp:Label>
    <asp:ModalPopupExtender ID="mpeSubmitConfirm" runat="server" TargetControlID="lblHiddenConfirm" PopupControlID="dvSubmitConfirm">
    </asp:ModalPopupExtender>
    <div id="dvSubmitConfirm" class="modalPopup" style="display: none;width:800px">
        <div class="header">
          <strong>CAUTION!</strong>
        </div>
        <div class="body">
            
             <strong>All your customer data/time sheet for current week and past weeks (if not submitted) will now be submitted to your manager for review and approval. And you won’t be able to edit any of these entries in future.</strong>
             <br /><br /><strong>If you want to go back and review your current week data (or previous week data) - please click on [Cancel] else, please click on [Submit Time].</strong>
        </div>
        <div class="footer" align="right">
                 <asp:Button ID="btnSubmitNo" runat="server" Text="Cancel" CssClass="btn btn-primary" 
                onclick="btnSubmitNo_Click" /> 
                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
    
                <asp:Button ID="btnSubmitYes" runat="server" Text="Submit Time" CssClass="btn btn-primary"
                onclick="btnSubmitYes_Click" /> 
        </div>
    </div>

    </ContentTemplate>
    </asp:UpdatePanel>
    
</form>
<script type="text/javascript">
    function pageLoad(sender, args) {
        if (args.get_isPartialLoad()) {
            $(".txtDate").datepicker({ minDate: '-30' });
            $(".txtSunDate").datepicker({ beforeShowDay:
             function (dt) {
                 return [dt.getDay() == 0 , ""];
             }
         });

        }
    }
    $(function () {
        $(".txtDate").datepicker({  minDate: '-30' });
    });

    $(function () {
        $(".txtSunDate").datepicker({ beforeShowDay: 
             function(dt)
              {
                return [dt.getDay() == 0 , ""];
              }
        });
    });
  </script>

  <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.0.min.js" type="text/javascript"></script>
<script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
<link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css"
    rel="Stylesheet" type="text/css" />
   <%-- <script src="js/jquery.tablesorter.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#gvRecordsMonday').tablesorter();
        });
    </script>--%>
<script type="text/javascript">
   
      
   //On Page Load.
    $(function () {
        SetAutoComplete();
        header();
        SortTable();
        
    });
    
    //On UpdatePanel Refresh.
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    if (prm != null) {
        prm.add_endRequest(function (sender, e) {
            if (sender._postBackSettings.panelsToUpdate != null) {
                SetAutoComplete();
                header();
                SortTable();
                
            }
        });
    };

    function header(){
       $('tr:first-child').children('td').replaceWith(function(i, html) {
         return '<th>' + $(this).text() + '</th>';
        });

         var $table = $('#movie');

    // This gets the first <tr> within the table, and remembers it here.
    var $headRow = $('tr', $table).first();
    $headRow.remove();

    var $footRow = $('tr', $table).last();
    $footRow.remove();

    if (!$table.has('tbody')) {
        var $otherRows = $('tr', $table);
        $otherRows.remove();

        var $tbody = $('<tbody>');
        $table.append($tbody);
        $tbody.append($otherRows);
    }

    var $thead = $('<thead>');
    $table.prepend($thead);
    $thead.append($headRow);
    var $foot = $('<tfoot>');
    $table.prepend($foot);
    $foot.append($footRow);
    }

    function SortTable() {
          
     // $('tr:first td').wrapInner('<div />').find('div').unwrap().wrap('<th/>');
     
         // Selectors for future use

        var myTable = "#movie";
        var myTableBody = myTable + " tbody";
        var myTableRows = myTableBody + " tr";
        var myTableColumn = myTable + " th";
 
        // Starting table state
        function initTable() {
 
            // Increment the table width for sort icon support
            $(myTableColumn).each(function () {
                var width = $(this).width();
                $(this).width(width + 40);
            });
 
            // Set the first column as sorted ascending
            $(myTableColumn).eq(0).addClass("sorted-asc");
 
            //Sort the table using the current sorting order
            sortTable($(myTable), 0, "asc");
 
        }
 
        // Table starting state
        initTable();
 
        // Table sorting function
        function sortTable(table, column, order) {
            var asc = order === 'asc';
            var tbody = table.find('tbody');
 
            // Sort the table using a custom sorting function by switching 
            // the rows order, then append them to the table body
            tbody.find('tr').sort(function (a, b) {
                if (asc) {
                    return $('td:eq(' + column + ')', a).text()
                        .localeCompare($('td:eq(' + column + ')', b).text());
                } else {
                    return $('td:eq(' + column + ')', b).text()
                        .localeCompare($('td:eq(' + column + ')', a).text());
                }
            }).appendTo(tbody);
 
        }
 
        // Heading click
        $(myTableColumn).click(function () {
 
            // Remove the sort classes for all the column, but not the first
            $(myTableColumn).not($(this)).removeClass("sorted-asc sorted-desc");
 
            // Set or change the sort direction
            if ($(this).hasClass("sorted-asc") || $(this).hasClass("sorted-desc")) {
                $(this).toggleClass("sorted-asc sorted-desc");
            } else {
                $(this).addClass("sorted-asc");
            }
 
            //Sort the table using the current sorting order
            sortTable($(myTable),
                        $(this).index(),
                        $(this).hasClass("sorted-asc") ? "asc" : "desc");
 
        });
    }
         
    function SetAutoComplete() {
        $("[id$=txtSearchGvClient]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/RecordsWeeklyView.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('!')[0],
                                val: item.split('!')[1]
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
                $('#txtSearchGvClient').blur();
            },
            minLength: 1
           
        });

        $("[id$=txtSearchGvClientM]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/RecordsWeeklyView.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('!')[0],
                                val: item.split('!')[1]
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
                $('#txtSearchGvClientM').blur();
            },
            minLength: 1
        });

         $("[id$=txtSearchGvClientT]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/RecordsWeeklyView.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('!')[0],
                                val: item.split('!')[1]
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
                $('#txtSearchGvClientT').blur();
            },
            minLength: 1
        });

        $("[id$=txtSearchGvClientW]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/RecordsWeeklyView.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('!')[0],
                                val: item.split('!')[1]
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
                $('#txtSearchGvClientW').blur();
            },
            minLength: 1
        });

         $("[id$=txtSearchGvClientTH]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/RecordsWeeklyView.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('!')[0],
                                val: item.split('!')[1]
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
                $('#txtSearchGvClientTH').blur();
            },
            minLength: 1
        });

        $("[id$=txtSearchGvClientF]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/RecordsWeeklyView.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('!')[0],
                                val: item.split('!')[1]
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
                $('#txtSearchGvClientF').blur();
            },
            minLength: 1
        });

        $("[id$=txtSearchGvClientSA]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveUrl("~/RecordsWeeklyView.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                         $("[id$=hfCustomerId]").val("0");
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('!')[0],
                                val: item.split('!')[1]
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
                $('#txtSearchGvClientSA').blur();
            },
            minLength: 1
        });
    }  
</script>
</asp:Content>

