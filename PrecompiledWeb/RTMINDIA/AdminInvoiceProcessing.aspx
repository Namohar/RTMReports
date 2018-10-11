<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="AdminInvoiceProcessing, App_Web_as44pg0l" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>
<script type="text/javascript" src="Scripts/quicksearch.js"></script>
   <script type="text/javascript">
       var startingIndex = 0, gridViewID = '<%= gvInvoices.ClientID %>';
       function selectCheckbox(e, selectedIndex) {
           if (e.shiftKey) {
             
               if (startingIndex < selectedIndex) {
                 
                   $(':checkbox', '#' + gridViewID).slice(startingIndex, selectedIndex).prop("checked", true);
               }
               else
                   $(':checkbox', '#' + gridViewID).slice(selectedIndex, startingIndex).prop("checked", true);
           }
           startingIndex = selectedIndex;
       }

       $(function () {
           $('.search_textbox').each(function (i) {
               $(this).quicksearch("[id*=gvInvoices] tr:not(:has(th))", {
                   'testQuery': function (query, txt, row) {
                       return $(row).children(":eq(" + i + ")").text().toLowerCase().indexOf(query[0].toLowerCase()) != -1;
                   }
               });
           });
       });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
  <div class="row">
	<div class="col-lg-12">
	 <h3 class="page-header"><i class="fa fa-file-o"></i>EMSDB Admin</h3>
	 <ol class="breadcrumb">
		<li><i class="fa fa-home"></i><a href="DashBoard.aspx">Home</a></li>
		<li><i class="fa fa-file-o"></i>EMSDB Admin</li>
	 </ol>
	</div>
  </div>
  <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                        <div class="panel panel-default">
                            <div class="panel-body">
                                <div>
    
                                    <asp:Label ID="lblClient" runat="server" Text="Select Client:" Font-Bold="True" 
                                        Font-Size="Medium"></asp:Label>
                                    &nbsp;
                                    <asp:DropDownList ID="ddlClient" runat="server" AutoPostBack="True" 
                                         Width="200px" onselectedindexchanged="ddlClient_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="lblTotal" runat="server" Text="Total Invoices:"></asp:Label>
                                    <asp:Label ID="txtTotal" runat="server" Text="0" Font-Bold="True"></asp:Label>
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="lblAssigned" runat="server" Text="Invoices Assigned:"></asp:Label>
                                    <asp:Label ID="txtAssigned" runat="server" Text="0" Font-Bold="True"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
            </div>
   <div class="row">
     <div class="col-md-12 col-sm-12 col-xs-12">
       <div class="panel panel-default">
           <div class="panel-body">
             <div class="table-responsive">
                 <div style="text-align: right; width: 100%">
                     <asp:Label ID="lblUser" runat="server" Text="Select User:"></asp:Label>
                     <asp:DropDownList ID="ddlUser" runat="server">
                     </asp:DropDownList>
                     <asp:Button ID="btnSubmit" runat="server" Text="Assign" class="btn btn-primary" 
                         onclick="btnSubmit_Click" />
                 </div>
                 <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Height="500px">
                 
                 <asp:GridView ID="gvInvoices" runat="server" AutoGenerateColumns="False" 
                     class="table table-striped table-bordered table-hover" DataKeyNames="FI_ID">
                     <Columns>
                         <asp:TemplateField HeaderText="ID" Visible="False">
                             <ItemTemplate>
                                 <asp:Label ID="lblFIID" runat="server" Text='<%# Eval("FI_ID") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Select">
                                 <HeaderTemplate>
                                    <asp:Label ID="Label3" runat="server" Text="Select"></asp:Label><br />
                                    <asp:TextBox ID="txtSearch" runat="server" class="search_textbox" ReadOnly="True" BorderStyle="None" BorderColor="White" Width="10px" BackColor="#f9f9f9"></asp:TextBox>
                                </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:CheckBox ID="chkSelect" runat="server" onclick='<%# string.Format("javascript:selectCheckbox(event,{0});", Container.DataItemIndex) %>' AutoPostBack="false" />
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="PDF">
                             <HeaderTemplate>
                                    <asp:Label ID="Label1" runat="server" Text="PDF"></asp:Label><br />
                                    <asp:TextBox ID="txtSearch1" runat="server" placeholder="PDF" class="search_textbox"></asp:TextBox>
                                </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:LinkButton ID="lnkViewFile" runat="server" 
                                     Text='<%# Eval("FI_FileName") %>' onclick="lnkViewFile_Click" CommandArgument='<%# Eval("FI_ID") %>' Target="_blank"></asp:LinkButton>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Batch">
                             <HeaderTemplate>
                                    <asp:Label ID="Label2" runat="server" Text="Batch"></asp:Label><br />
                                    <asp:TextBox ID="txtSearch2" runat="server" placeholder="Batch" class="search_textbox"></asp:TextBox>
                                </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblBatch" runat="server" Text='<%# Eval("FI_CreatedOn") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Source">
                             <HeaderTemplate>
                                    <asp:Label ID="Label4" runat="server" Text="Source"></asp:Label><br />
                                    <asp:TextBox ID="txtSearch3" runat="server" placeholder="Source" class="search_textbox"></asp:TextBox>
                                </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblSource" runat="server" Text='<%# Eval("FI_Source") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Status">
                             <ItemTemplate>
                                 <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("IND_Status") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="Invoice#" Visible="False">
                             <ItemTemplate>
                                 <asp:Label ID="lblInvoiceNo" runat="server" Text='<%# Eval("IND_InvoiceNo") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="IP Assigned By" Visible="False">
                             <ItemTemplate>
                                 <asp:Label ID="lblIPAssigned" runat="server" 
                                     Text='<%# Eval("IND_IP_Assigned_By") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="IP Assigned To" Visible="False">
                             <ItemTemplate>
                                 <asp:Label ID="lblIPProcessed" runat="server" 
                                     Text='<%# Eval("IND_IP_Processed_By") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="QC Assigned By" Visible="False">
                             <ItemTemplate>
                                 <asp:Label ID="lblQCAssigned" runat="server" 
                                     Text='<%# Eval("IND_QC_Assigned_By") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                         <asp:TemplateField HeaderText="QC Assigned To" Visible="False">
                             <ItemTemplate>
                                 <asp:Label ID="lblQCProcessed" runat="server" 
                                     Text='<%# Eval("IND_QC_Processed_By") %>'></asp:Label>
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
</form>
</asp:Content>

