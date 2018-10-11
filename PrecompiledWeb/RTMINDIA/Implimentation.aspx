<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="Implimentation, App_Web_diasz2zu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>New Client</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>New Client</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
                <div style="text-align: center; width: 100%;">
 
    <asp:Label ID="lblEmp" runat="server" Font-Bold="True" Text="Select User"></asp:Label>
&nbsp;
    <asp:DropDownList ID="ddlEmp" runat="server" Width="200px">
    </asp:DropDownList>
<br />
<br />
    <asp:Label ID="lblDate" runat="server" Font-Bold="True" Text="Select Date"></asp:Label>
&nbsp;
    <asp:TextBox id="datepicker" ClientIDMode="Static" runat="server" Width="200px"></asp:TextBox>

 
    <br />
    <br />
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
        Class ="styled-button-2" onclick="btnSubmit_Click" />

 
</div>
<br />
<br />
<div style="width: 100%">

    <asp:GridView ID="gvRecords" runat="server" AutoGenerateColumns="False" 
        Width="100%" DataKeyNames="R_ID" 
        onrowcancelingedit="gvRecords_RowCancelingEdit" 
        onrowdatabound="gvRecords_RowDataBound" onrowediting="gvRecords_RowEditing" 
        onrowupdating="gvRecords_RowUpdating" class="table table-striped table-bordered table-hover">
        <Columns>
            <asp:TemplateField HeaderText="Client">
                <ItemTemplate>
                    <asp:Label ID="lblClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlGvClient" runat="server" Width="200px">
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Task">
                <ItemTemplate>
                    <asp:Label ID="lblTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlGvTask" runat="server" 
                        onselectedindexchanged="ddlGvTask_SelectedIndexChanged" AutoPostBack="True" Width="200px">
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="SubTask">
                <ItemTemplate>
                    <asp:Label ID="lblSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlGvSubTask" runat="server" Width="200px">
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Comments">
                <ItemTemplate>
                    <asp:Label ID="lblComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" />
        </Columns>
    </asp:GridView>

</div>
             </div>
           </div>
     </div>
    </div>
</form>
<script type="text/javascript">
    $(function () {
        $("#datepicker").datepicker({ maxDate: new Date() });
    });
  </script>
</asp:Content>

