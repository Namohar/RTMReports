<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ImplRealTimeStatusReport.aspx.cs" Inherits="ImplRealTimeStatusReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form runat="server">
   <table style="width: 100%;">
         <tr>
             <td width="40%" colspan="2">
                 <asp:Label ID="lblError" runat="server"></asp:Label>
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 <asp:Label ID="lblFrom" runat="server" Text="From Date" Font-Bold="True"></asp:Label>
             </td>
             <td>
                 <asp:TextBox ID="txtFrom" runat="server" ClientIDMode="Static"></asp:TextBox>
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td style="text-align: right">
                 <asp:Label ID="lblTo" runat="server" Text="To Date" Font-Bold="True"></asp:Label>
             </td>
             <td>
                 <asp:TextBox ID="txtTo" runat="server" ClientIDMode="Static"></asp:TextBox>
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 <asp:Button ID="btnPrint" runat="server" onclick="btnPrint_Click" 
                     Text="Display" class="btn btn-primary" />
             &nbsp;
                 <asp:Button ID="btnExport" runat="server" Text="Export" class="btn btn-primary" 
                     onclick="btnExport_Click" />
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
         <tr>
             <td colspan="2" style="text-align: right">
                 <asp:Panel ID="Panel1" runat="server" ScrollBars="Horizontal" Height="280px">
                 <asp:GridView ID="gvStatus" runat="server" Width="100%" AutoGenerateColumns="False" class="table table-striped table-bordered table-hover">
                     <Columns>
                         <asp:BoundField DataField="Request ID" HeaderText="Request ID">
                         <HeaderStyle HorizontalAlign="Center" />
                         <ItemStyle HorizontalAlign="Center" />
                         </asp:BoundField>
                         <asp:BoundField DataField="Product" HeaderText="Product">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                         </asp:BoundField>
                         <asp:BoundField DataField="Client" HeaderText="Client" >
                         <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                         </asp:BoundField>
                         <asp:BoundField DataField="Status" HeaderText="Status" >
                         <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                         </asp:BoundField>
                         <asp:BoundField DataField="Time Tracking Code" 
                             HeaderText="Time Tracking Code" >
                             <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                         </asp:BoundField>
                         <asp:BoundField DataField="Type" HeaderText="Type" >
                         <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                         </asp:BoundField>
                     </Columns>
                 </asp:GridView>
                 </asp:Panel>
             </td>
         </tr>
         <tr>
             <td style="text-align: right">
                 &nbsp;</td>
             <td>
                 &nbsp;</td>
         </tr>
     </table>
</form>
<script type="text/javascript">
    $(function () {
        $("#txtFrom").datepicker({ maxDate: new Date() });
    });

    $(function () {
        $("#txtTo").datepicker({ maxDate: new Date() });
    });

  </script>
</asp:Content>

