<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageTasks.aspx.cs" Inherits="ManageTasks" %>

<%@ Register Assembly="obout_Grid_NET" Namespace="Obout.Grid" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
<div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Add Task</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Add Task</li>
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
                                 <asp:HiddenField ID="hfTaskId" runat="server" Value="0" ClientIDMode="Static" />
                                 <strong>Location:</strong>
                                 <asp:DropDownList ID="ddlLocation" runat="server" class="form-control"  Width="60%"
                                 AutoPostBack="True" onselectedindexchanged="ddlLocation_SelectedIndexChanged">
                             </asp:DropDownList>
                             </td>
                             <td style="width:34%">
                                 <strong>Team:</strong>
                                 <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" 
                                     AutoPostBack="True" onselectedindexchanged="ddlTeam_SelectedIndexChanged">
                                 </asp:DropDownList>
                             </td>
                             <td style="width:33%">
                                 <strong>Task Name:</strong>
                                 <asp:TextBox ID="txtTaskName" runat="server" class="form-control" ClientIDMode="Static" Width="70%"></asp:TextBox>
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
                                 &nbsp;
                             </td>
                             <td style="text-align:right">
                                  
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
                         <cc1:Grid ID="grTasks" runat="server" AllowAddingRecords = "false" CallbackMode="true" Serialize="true" 
                      AllowRecordSelection="true" AllowMultiRecordSelection="false" KeepSelectedRecords="true" AllowFiltering ="true"
                        FolderStyle="Styles/grand_gray" AutoGenerateColumns="False" OnRebind="RebindGrid">
                          <ClientSideEvents OnClientSelect="onRecordSelect" OnClientCallbackError="onCallbackError" />

                          <FilteringSettings InitialState="Visible" FilterPosition="Top" />                          
                            <Columns>
                               <cc1:Column DataField="TaskId" ReadOnly="true" Visible="false" HeaderText="ID"></cc1:Column>
                               <cc1:Column DataField="TeamName" ReadOnly="true" Visible="true" Width="250px" HeaderText="Team Name"></cc1:Column>
                               <cc1:Column DataField="TaskName" ReadOnly="true" Visible="true" Width="250px" HeaderText="Task Name"></cc1:Column>
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
            for (var i = 0; i < grTasks.SelectedRecords.length; i++) {
                var record = grTasks.SelectedRecords[i];

                $('#txtTaskName').val(record.TaskName);
                $('#hfTaskId').val(record.TaskId);
                if (record.IsActive == "yes") {
                    $('#chkActiveStatus').prop('checked', true);
                }
                else {
                    $('#chkActiveStatus').prop('checked', false);
                }

                $('#btnADD').val("Update");
            }
        }

        function onCallbackError(errorMessage, commandType, recordIndex, data) {
            alert(errorMessage);
        }
    </script>
</form>
</asp:Content>

