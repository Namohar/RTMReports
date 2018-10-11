<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="ManageClientsNew, App_Web_as44pg0l" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link href="http://code.jquery.com/ui/1.11.4/themes/ui-lightness/jquery-ui.css" rel="stylesheet" type="text/css"/>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        var jQuery_1_8_3 = $.noConflict(true);
    </script>
    <script type="text/javascript" src="http://code.jquery.com/jquery-1.10.2.js"></script>
<script type="text/javascript" src="http://code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
<script type="text/javascript" src="Scripts/quicksearch.js"></script>
 <style type="text/css">
    .loading
    {
        
        display: none;
        position: fixed;
        background-color: White;
       
    }
    
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
         jQuery_1_8_3('form').live("submit", function () {
             ShowProgress();
         });
</script>
<script type="text/javascript">
    function pageLoad(sender, args) {
        if (args.get_isPartialLoad()) {
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
        $('.search_textbox').each(function (i) {
            $(this).quicksearch("[id*=gvClients] tr:not(:has(th))", {
                'testQuery': function (query, txt, row) {
                    return $(row).children(":eq(" + i + ")").text().toLowerCase().indexOf(query[0].toLowerCase()) != -1;
                }
            });
        });
    });
</script>

<script type="text/javascript">
    $(function () {
        SearchText();
    });
    function SearchText() {
        $(".autosuggest").autocomplete({
            source: function (request, response) {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "ManageClientsNew.aspx/GetAutoCompleteData",
                    data: "{'clientname':'" + $('#txtClientSearch').val() + "'}",
                    dataType: "json",
                    success: function (data) {
                        if (data.d.length > 0) {
                            response($.map(data.d, function (item) {
                                return {
                                    label: item.split('!')[0],
                                    val: item.split('!')[1]
                                }
                            }));
                        }
                        else {
                            response([{ label: 'No Records Found', val: -1}]);
                        }
                    },
                    error: function (result) {
                        alert("Error");
                    }
                });
            },
            select: function (event, ui) {
                if (ui.item.val == -1) {
                    return false;
                }
//                $('#txtClientSearch').text(ui.item.val);
            }
        });
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form id="Form1" runat="server">
   <div class="row">
     <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-dashboard"></i>Manage Clients</h3>
		<ol class="breadcrumb">
		    <li><i class="fa fa-home"></i><a href="DashBoard.aspx">Home</a></li>
	    	<li><i class="fa fa-edit"></i>Manage Clients</li>
		</ol>
	 </div>
	</div>
<%-- <asp:ScriptManager ID="ScriptManager1" runat="server" CombineScripts="false">
 </asp:ScriptManager>--%>
    

    <div  class="row" >
       <div id="dvUpload" class="col-md-12 col-sm-12 col-xs-12"  runat="server" visible="false">
          <div class="panel panel-default">
             <div class="panel-heading">
                 Upload Clients
                 <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
             </div>
             <div class="panel-body">
                 <table style="width: 100%;">
                     <tr>
                         <td style="text-align: right" width="40%">
                 <asp:FileUpload ID="FileUpload1" runat="server" />
                         </td>
                         <td>
                <asp:Button ID="btnUpload" runat="server" Text="Upload"
                         OnClick="btnUpload_Click" class="btn btn-primary" />
                         &nbsp;<asp:Button ID="btnSample" runat="server" Text="Download Template" 
                                 class="btn btn-primary" onclick="btnSample_Click" />
                         </td>
                     </tr>
                     </table>
                <asp:Label ID="lblMessage" runat="server" Text="" />
             </div>
           </div>
       </div>
        <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">
         <ContentTemplate>--%>
             <div class="col-md-12 col-sm-12 col-xs-12">
              <div class="panel panel-default">
                 <div class="panel-heading">
                     Add Clients
                 </div>
                 <div class="panel-body">
                    <div  class="row" >
                      <div class="col-md-8 col-sm-12 col-xs-12">
                        <div class="panel panel-default">
                             <div class="panel-body">
                                 <div>
                                <%-- <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" CombineScripts="false">
    </asp:ToolkitScriptManager>--%>
                                     <asp:Label ID="lblClientSearch" runat="server" Text="Search Client:" 
                                         Font-Bold="True"></asp:Label>
                                   <asp:TextBox ID="txtClientSearch" runat="server" Width="200px" class="autosuggest tb10" ClientIDMode="Static" ></asp:TextBox>
                                    <%-- <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" ServiceMethod="SearchClients" MinimumPrefixLength="2"
                                                CompletionInterval="100" EnableCaching="false" CompletionSetCount="10"
                                                TargetControlID="txtClientSearch" FirstRowSelected = "false">
                                     </asp:AutoCompleteExtender>--%>
                                     &nbsp;&nbsp;&nbsp;
                                     <asp:Button ID="btnClientSearch" runat="server" Text="Search" 
                                         onclick="btnClientSearch_Click" class="btn btn-primary" />
                                 </div>
                                 <asp:Panel ID="Panel1" runat="server" Height="350px" ScrollBars="Both">
                                 <asp:GridView ID="gvClients" runat="server" AutoGenerateColumns="False" 
                                     class="table table-striped table-bordered table-hover" >
                                     <Columns>
                                          <asp:TemplateField HeaderText="Client Name">
                                                <HeaderTemplate>
                                                    <asp:Label ID="Label3" runat="server" Text="Client Name"></asp:Label><br />
                                                    <asp:TextBox ID="txtSearch" runat="server" placeholder="Client Name" class="search_textbox"></asp:TextBox>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblClientName" runat="server" Text='<%# Eval("MCD_ClientName_JCOne") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Job Code 0">
                                                <HeaderTemplate>
                                                    <asp:Label ID="Label4" runat="server" Text="Job Code 0"></asp:Label><br />
                                                    <asp:TextBox ID="txtSearch1" runat="server" placeholder="Job Code 0" class="search_textbox"></asp:TextBox>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblJC0" runat="server" Text='<%# Eval("MCD_JCZero") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Client Code">
                                                <HeaderTemplate>
                                                    <asp:Label ID="Label5" runat="server" Text="Client Code"></asp:Label><br />
                                                    <asp:TextBox ID="txtSearch2" runat="server" placeholder="Client Code" class="search_textbox"></asp:TextBox>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblClientCode" runat="server" Text='<%# Eval("MCD_ClientCode") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                          <asp:TemplateField HeaderText="Select">
                                              <ItemTemplate>
                                                  <asp:LinkButton ID="lnkSelect" runat="server" onclick="lnkSelect_Click">Select</asp:LinkButton>
                                              </ItemTemplate>
                                          </asp:TemplateField>
                                     </Columns>
                                 </asp:GridView>
                                 </asp:Panel>
                             </div>
                        </div>
                      </div>

                      <div class="col-md-4 col-sm-12 col-xs-12">
                         <div class="panel panel-default">
                             <div class="panel-body">
                                 <div class="form-group">
                                    <strong>Please select the team:</strong>
                                    <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" Width="170px">
                                    </asp:DropDownList>
                                    <br />
                                    <strong>Client:</strong>
                                    <asp:Label ID="lblSelectedClient" runat="server" Text=""></asp:Label>
                                     <asp:Label ID="lbljl0" runat="server" Text="" Visible= "false"></asp:Label>
                                     <asp:Label ID="lblCode" runat="server" Text="" Visible="false"></asp:Label>
                                    <br />
                                    <strong>Please select the platform:</strong>
                                     <asp:DropDownList ID="ddlPlatform" runat="server" class="form-control">
                                     </asp:DropDownList>
                                     <br />
                                     <strong>Please specify if any other platform:</strong>
                                     <asp:TextBox ID="txtPlatform" runat="server" class="form-control" 
                                         MaxLength="15"></asp:TextBox>
                                     <br />
                                     <asp:Button ID="btnAdd" runat="server" Text="ADD" class="btn btn-primary" 
                                         onclick="btnAdd_Click" />
                                 </div>
                             </div>
                         </div>
                       </div>
                    </div>

                 </div>
               </div>
           </div>
        <%-- </ContentTemplate>
        </asp:UpdatePanel>--%>
    </div>
    <div class="loading" align="center"> 
        <img src="images/pleasewait.gif" height="40px" width="40px" alt="" /><br />
        Please wait...
     </div>
 </form>
</asp:Content>