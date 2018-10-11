<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AddTeam.aspx.cs" Inherits="AddTeam" %>

<%@ Register Assembly="obout_Grid_NET" Namespace="Obout.Grid" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
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
                    url: "AddTeam.aspx/GetAutoCompleteUsers",
                    data: "{'username':'" + $('#txtManager').val() + "'}",
                    dataType: "json",
                    success: function (data) {
                        $("[id$=hfMid]").val("0");
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
                $("[id$=hfMid]").val(ui.item.val);
            }
        });

        $(".autosuggestTeams").autocomplete({
            source: function (request, response) {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "AddTeam.aspx/GetAutoCompleteTeams",
                    data: "{'teamname':'" + $('#txtTeamName').val() + "'}",
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
//                            response([{ label: 'No Records Found', val: -1}]);
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
            }
        });
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form id="Form1" runat="server">
  <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Add Team</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Add Team</li>
			</ol>
	 </div>
    </div>

     <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
            <div class="panel-body">
             <div class="col-lg-12">              
                 <table style="width: 100%;">
                     <tr>
                         <td style="text-align:right; width:20%">
                             <asp:HiddenField ID="lblTid" runat="server" Value="0" ClientIDMode="Static" />
                             <asp:HiddenField ID="hfMid" runat="server" Value="0" ClientIDMode="Static" />
                             <%--<asp:Label ID="lblTid" runat="server" Text="0" Visible="false" ClientIDMode="Static"></asp:Label>--%>
                             <strong>Team Name : </strong>
                         </td>
                         <td style="width:20%">
                             <asp:TextBox ID="txtTeamName" runat="server" class="form-control autosuggestTeams" ClientIDMode="Static" Width="80%"></asp:TextBox> 
                         </td>
                          <td style="text-align:right">
                             <strong>Location : </strong>
                         </td>
                          <td>
                             <asp:DropDownList ID="ddlLocation" runat="server" class="form-control"  Width="40%"
                                 AutoPostBack="True" onselectedindexchanged="ddlLocation_SelectedIndexChanged">
                             </asp:DropDownList>
                         </td>                  
                     </tr>
                     <tr>
                         <td>
                             &nbsp;
                         </td>
                         <td>
                             &nbsp;
                         </td>
                          <td>
                             &nbsp;
                         </td>
                          <td>
                             &nbsp;
                         </td>
                     </tr>
                     <tr>
                         <td style="text-align:right; width:20%">
                             <strong>Team POC/Manager : </strong>
                         </td>
                         <td>
                             <asp:TextBox ID="txtManager" runat="server" class="form-control autosuggest" ClientIDMode="Static" Width="80%"></asp:TextBox>
                         </td> 
                          <td style="text-align:right">
                             <strong>Is Active : </strong>
                         </td>
                          <td>
                             &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkActiveStatus" runat="server" Checked="true" ClientIDMode="Static" /> 
                         </td>                  
                     </tr>
                     <tr>
                         <td>
                             &nbsp;
                         </td>
                         <td>
                             &nbsp;
                         </td>
                          <td>
                             &nbsp;
                         </td>
                          <td>
                             &nbsp;
                         </td>
                     </tr>
                     <tr>
                         <td style="text-align:right">
                             
                         </td>
                         <td>
                             
                                                                  
                         </td>
                          <td style="text-align:left">
                             
                         </td>
                          <td >
                              <asp:Button ID="btnSave" runat="server" Text="Save" class="btn btn-primary" 
                                 onclick="btnSave_Click" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false" ClientIDMode="Static" /> 
                              &nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" Text="Clear" 
                                 class="btn btn-primary" onclick="btnCancel_Click" />
                         </td>
                     </tr>
                     <tr>
                         <td>
                             <asp:LinkButton ID="lnkExport" runat="server" Font-Bold="true" 
                                  onclick="lnkExport_Click" Visible="false">Export to CSV</asp:LinkButton>
                         </td>
                         <td>
                             &nbsp;
                         </td>
                          <td>
                             &nbsp;
                         </td>
                          <td>
                             &nbsp;
                         </td>
                     </tr>
                 </table>
             </div>

             <div class="col-lg-12">
                <div class="table-responsive">
                    <cc1:Grid ID="grTeams" runat="server" AllowAddingRecords = "false" CallbackMode="true" Serialize="true" 
                      AllowRecordSelection="true" AllowMultiRecordSelection="false" KeepSelectedRecords="true" AllowFiltering ="true"
                        FolderStyle="Styles/grand_gray" AutoGenerateColumns="False" OnRebind="RebindGrid" OnUpdateCommand="UpdateRecord">
                        <ClientSideEvents OnClientSelect="onRecordSelect" />
                        <FilteringSettings InitialState="Visible" FilterPosition="Top" />
                        <Columns>
                            <cc1:Column DataField="T_ID" ReadOnly="true" Visible="false" HeaderText="ID"></cc1:Column>
                            <cc1:Column DataField="T_TeamName" ReadOnly="true" HeaderText="Team Name"></cc1:Column>
                            <cc1:Column DataField="T_Location" ReadOnly="true" HeaderText="Location"></cc1:Column>
                            <cc1:Column DataField="T_Manager" ReadOnly="true" HeaderText="Manager"></cc1:Column>
                            <cc1:Column DataField="T_Active" HeaderText="Is Active">
                              <TemplateSettings TemplateID="isActive" EditTemplateID="isActiveEdit" />
                            </cc1:Column>
                            <%--<cc1:Column AllowEdit="false" AllowDelete="false" HeaderText="" Width="125" runat="server" />--%>
                        </Columns>
                        <Templates>
                            <cc1:GridTemplate runat="server" ID="isActive" UseQuotes="true">
					            <Template>
						            <%# (Container.Value == "yes" ? "yes" : "no")%>
					            </Template>
				            </cc1:GridTemplate>
				           <%-- <cc1:GridTemplate runat="server" ID="isActiveEdit" ControlID="chkActive" ControlPropertyName="checked" UseQuotes="false">
					            <Template>
						            <input type="checkbox" id="chkActive"/>
					            </Template>
				            </cc1:GridTemplate>--%>
                       </Templates>
                    </cc1:Grid>
                </div>
             </div>
            </div>
           </div>           
        </div>
    </div>
    <script type="text/javascript">
        function onRecordSelect(arrSelectedRecords) {
            for (var i = 0; i < grTeams.SelectedRecords.length; i++) {
                var record = grTeams.SelectedRecords[i];
                
                $('#txtTeamName').val(record.T_TeamName);
                $('#txtManager').val(record.T_Manager);
                $('#lblTid').val(record.T_ID);
                if (record.T_Active == "yes") {
                    $('#chkActiveStatus').prop('checked', true);
                }
                else {
                    $('#chkActiveStatus').prop('checked', false);
                }

                $('#btnSave').val("Update");
            }
        }
    </script>
</form>
</asp:Content>

