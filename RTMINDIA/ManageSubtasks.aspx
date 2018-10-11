<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageSubtasks.aspx.cs" Inherits="ManageSubtasks" %>
<%@ Register Assembly="obout_Grid_NET" Namespace="Obout.Grid" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<style>
  .ui-autocomplete {
    max-height: 250px;
    overflow-y: auto;
    overflow-x: hidden;
    position:static;
    max-width: 250px;
}
</style>
<script type="text/javascript">
    $(function () {
        SearchText();
    });
    function SearchText() {
        $(".autosuggestServiceCodes").autocomplete({
            source: function (request, response) {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "ManageSubtasks.aspx/GetAutoCompleteServiceCodes",
                    data: "{'servicecode':'" + $('#txtServieCode').val() + "'}",
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
		<h3 class="page-header"><i class="fa fa-edit"></i>Add Subtask</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Add Subtask</li>
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
                             <td style="width:33%">
                                 <asp:HiddenField ID="hfSubtaskId" runat="server" Value="0" ClientIDMode="Static" />
                                 <strong>Location:</strong>
                                 <asp:DropDownList ID="ddlLocation" runat="server" class="form-control"  Width="60%"
                                 AutoPostBack="True" onselectedindexchanged="ddlLocation_SelectedIndexChanged">
                             </asp:DropDownList>
                             </td>
                             <td style="width:34%">
                                 <strong>Team:</strong>
                                 <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" 
                                     AutoPostBack="True" onselectedindexchanged="ddlTeam_SelectedIndexChanged" 
                                     Width="60%">
                                 </asp:DropDownList>
                             </td>
                             <td style="width:33%">
                                 <strong>Task:</strong>
                                 <asp:DropDownList ID="ddlTask" runat="server" class="form-control" 
                                     AutoPostBack="True" Width="60%" onselectedindexchanged="ddlTask_SelectedIndexChanged">
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
                         </tr>
                         <tr>
                             <td>
                                 <strong>Subtask Name:</strong>
                                 <asp:TextBox ID="txtSubTaskName" runat="server" class="form-control" 
                                     ClientIDMode="Static" Width="60%"></asp:TextBox>
                             </td>
                             <td>
                                  <strong>Service code:</strong>
                                 <asp:TextBox ID="txtServieCode" runat="server" class="form-control autosuggestServiceCodes ui-autocomplete" 
                                      ClientIDMode="Static" Width="80%"></asp:TextBox>
                             </td>
                             <td>
                                 <strong>Is Active: </strong>
                                 <asp:CheckBox ID="chkActiveStatus" runat="server" Checked="true" ClientIDMode="Static" />
                                 &nbsp;&nbsp;&nbsp;&nbsp;
                                 <asp:Button ID="btnADD" runat="server" Text="ADD" class="btn btn-primary" 
                                     onclick="btnADD_Click" ClientIDMode="Static" OnClientClick="this.disabled = true; this.value = 'Please wait.';" UseSubmitBehavior="false" />
                                      &nbsp;&nbsp;&nbsp;&nbsp;
                                 <asp:Button ID="btnClear" runat="server" Text="Clear" class="btn btn-primary" 
                                     onclick="btnClear_Click" />
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
                         </tr>
                         <tr>
                             <td>
                                 &nbsp;&nbsp;<asp:LinkButton ID="lnkExport" runat="server" Font-Bold="true" 
                                  onclick="lnkExport_Click" Visible="false">Export to CSV</asp:LinkButton>
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
                     <div class="table-responsive" style="text-align:center">
                         <cc1:Grid ID="grSubTasks" runat="server" AllowAddingRecords = "false" CallbackMode="true" Serialize="true" 
                      AllowRecordSelection="true" AllowMultiRecordSelection="false" KeepSelectedRecords="true" AllowFiltering ="true"
                        FolderStyle="Styles/grand_gray" AutoGenerateColumns="False" OnRebind="RebindGrid">
                          <ClientSideEvents OnClientSelect="onRecordSelect" />
                          <FilteringSettings InitialState="Visible" FilterPosition="Top" />                          
                            <Columns>
                               <cc1:Column DataField="SubtaskId" ReadOnly="true" Visible="false" HeaderText="ID"></cc1:Column>
                               <cc1:Column DataField="Task" ReadOnly="true" Visible="true" Width="250px" HeaderText="Task Name"></cc1:Column>
                               <cc1:Column DataField="Subtask" ReadOnly="true" Visible="true" Width="250px" HeaderText="Subtask Name"></cc1:Column>
                               <cc1:Column DataField="ServiceCode" ReadOnly="true" Visible="true" Width="250px" HeaderText="Service Code"></cc1:Column>
                               <cc1:Column DataField="IsActive" ReadOnly="true" Visible="true" HeaderText="Is Active"></cc1:Column>
                            </Columns>
                        </cc1:Grid>
                     </div>
                 </div>
              </div>
           </div>
      </div>
    </div>

    <script type="text/javascript">
        function onRecordSelect(arrSelectedRecords) {
            for (var i = 0; i < grSubTasks.SelectedRecords.length; i++) {
                var record = grSubTasks.SelectedRecords[i];

                $('#txtSubTaskName').val(record.Subtask);
                $('#txtServieCode').val(record.ServiceCode);
                $('#hfSubtaskId').val(record.SubtaskId);
                if (record.IsActive == "yes") {
                    $('#chkActiveStatus').prop('checked', true);
                }
                else {
                    $('#chkActiveStatus').prop('checked', false);
                }

                $('#btnADD').val("Update");
            }
        }
    </script>
</form>
</asp:Content>

