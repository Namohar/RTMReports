<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ViewTicketDetails.aspx.cs" Inherits="ViewTicketDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form runat="server">
<div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>View Ticket Details</h3>
	    <ol class="breadcrumb">
			<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
			<li><i class="fa fa-pencil-square-o"></i>View Ticket Details</li>
		</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
           <div class="panel-heading">
             <asp:Label ID="lblHeader" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
           </div>
             <div class="panel-body">
                <div class="col-lg-6">
                 <div class="form-group"> 
                    <asp:Label ID="lblError" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                   <asp:Label ID="lblTicketid" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Ticket#: "></asp:Label>
                   <asp:Label ID="txtTicketId" runat="server" Font-Bold="True"></asp:Label>
                            <asp:Label ID="lblAccess" runat="server" Visible="False"></asp:Label>
                            <asp:Label ID="lblUser" runat="server" Visible="False"></asp:Label>
                            <asp:Label ID="lblTeam" runat="server" Visible="False"></asp:Label>
                            <asp:Label ID="lblEmail" runat="server" Visible="False"></asp:Label>
                            <asp:Label ID="lblEmpId" runat="server" Visible="False"></asp:Label>

                               <asp:Label ID="lblRptMgrEmail" runat="server" Visible="False"></asp:Label>


                   <br />
                   <asp:Label ID="lblStatus" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Ticket Status: "></asp:Label>
                   <asp:DropDownList ID="ddlStatus" runat="server" Width="300px" class="form-control">
                   </asp:DropDownList>
                   <br />
                   <asp:Label ID="lblCreatedBy" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Created By: "></asp:Label>
                   <asp:Label ID="txtCreatedBY" runat="server" Font-Bold="True"></asp:Label>
                   <br />
                   <asp:Label ID="lblCreatedOn" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Created On: "></asp:Label>
                   <asp:Label ID="txtCreatedOn" runat="server" Font-Bold="True"></asp:Label>
                   <br />
                   <asp:Label ID="lblClosedOn" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Closed On: " Visible="False"></asp:Label>
                   <asp:Label ID="txtClosedOn" runat="server" Font-Bold="True" Visible="False"></asp:Label>
                   <br />
                   <asp:Label ID="lblType" runat="server" Font-Bold="True" Font-Size="Small" 
                    Text="Ticket Type:"></asp:Label>
                     <asp:DropDownList ID="ddlType" runat="server" Visible="False" Width="300px" class="form-control">
                     </asp:DropDownList>
                     <asp:Label ID="txtType" runat="server" Font-Bold="True" Visible="False"></asp:Label>
                    <br />
                    <asp:Label ID="lblSummary" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Summary: "></asp:Label>
                    <asp:Label ID="txtSummary" runat="server" Font-Bold="True"></asp:Label>
                    <br />
                    <asp:Label ID="lblDetails" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Details: "></asp:Label>
                    <textarea id="txtDetails" cols="20" rows="5" style="width: 300px" 
                                 maxlength="512" runat="server" name="S1" readonly="readonly" class="form-control"></textarea>
                    <br />
                    <asp:Label ID="lblAssignedto" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Assigned To: "></asp:Label>
                    <asp:DropDownList ID="ddlAssignedTo" runat="server" Width="300px" 
                                Visible="False" class="form-control">
                            </asp:DropDownList>
                            <asp:Label ID="lblAssigned" runat="server" Font-Bold="True" Visible="False"></asp:Label>
                    <br />
                    <asp:Label ID="lblETA" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="ETA: "></asp:Label>
                    <asp:TextBox ID="txtETA" ClientIDMode="Static" runat="server" Width="300px" class="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="lblScreenShot" runat="server" Text="Attach file/Screen Shot: " 
                                Font-Bold="True" Font-Size="Small"></asp:Label>
                    <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="true" />
                    <br />
                    <asp:Button ID="btnUpload" runat="server" Text="Upload" onclick="btnUpload_Click" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false"  />
                    <br />
                    <asp:Label ID="lblAttachments" runat="server" Font-Bold="True" 
                                Font-Size="Medium" Text="Attachments: "></asp:Label>
                    <asp:GridView ID="gvAttachments" runat="server" AutoGenerateColumns="False" 
                                DataKeyNames="I_ID">
                                <Columns>
                                    <asp:BoundField DataField="I_Name" HeaderText="File Name" />
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDownload" runat="server" Text="View" OnClick="DownloadFile"
                                        CommandArgument='<%# Eval("I_ID") %>'></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                 </div>
                 </div>

                 <div class="col-lg-6">
                 <div class="form-group"> 
                    <asp:Label ID="lblNotes" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Notes:"></asp:Label>
                    <textarea id="txtNotes" style="border-style: none; width: 100%; height: 191px; font-family: 'Century Gothic'; font-size: 12px;" 
                                  runat="server" name="S2" readonly="readonly" class="form-control"></textarea>
                    <br />
                    <asp:Label ID="lblNewNotes" runat="server" Font-Bold="True" Font-Size="Small" 
                                Text="Add New Notes:"></asp:Label>
                    <textarea id="txtAdd" rows="5" style="width: 100%" 
                                  runat="server" name="S3" cols="20" placeholder="Max 256 Characters" maxlength="256" class="form-control"></textarea>

                    <br />

                    <asp:Button ID="btnUpdate" class="btn btn-primary" runat="server" onclick="btnUpdate_Click" 
                                Text="Update" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancel" class="btn btn-primary" runat="server" Text="Back" 
                                onclick="btnCancel_Click" />
                    <br />
                    <br />
                    <asp:Label ID="lblInternal" runat="server" Font-Bold="True" 
                                Text="Internal Notes:" Visible="False"></asp:Label>

                    <textarea id="txtInternal" name="S4" visible="False" runat="server" style="width: 100%" cols="60" 
                                rows="6" class="form-control"></textarea>
                    <br />
                    <asp:Label ID="lblAddInternal" runat="server" Font-Bold="True" Text="Message" 
                                Visible="False"></asp:Label>
                    <textarea id="txtMessage" name="S5" visible="False" runat="server" style="width: 100%" cols="60" 
                                rows="4" maxlength="350" class="form-control"></textarea>
                    <br />
                    <asp:Button ID="btnSend" runat="server" class="btn btn-primary" Text="Send" 
                                onclick="btnSend_Click" Visible="False" OnClientClick="this.disabled = true; this.value = 'Please wait...';" UseSubmitBehavior="false" />
                 </div>
                 </div>
               
             </div>
            </div>
        </div>
    </div>
</form>
<script type="text/javascript">
    $(function () {
        //            $("#datepickerTo").datepicker({ maxDate: "+1M +10D" });
        $("#txtETA").datepicker({ minDate: new Date() });
    });
  </script>
</asp:Content>

