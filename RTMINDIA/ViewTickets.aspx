<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ViewTickets.aspx.cs" Inherits="ViewTickets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script type="text/javascript" src="Scripts/quicksearch.js"></script>
<script type="text/javascript">
    $(function () {
        $('.search_textbox').each(function (i) {
            $(this).quicksearch("[id*=gvTickets] tr:not(:has(th))", {
                'testQuery': function (query, txt, row) {
                    return $(row).children(":eq(" + i + ")").text().toLowerCase().indexOf(query[0].toLowerCase()) != -1;
                }
            });
        });
    });
</script>
<style type="text/css">
       .ddl{
     width: 50%;
  height: 34px;
  padding: 6px 12px;
  font-size: 14px;
  line-height: 1.42857143;
  color: #555;
  background-color: #fff;
  background-image: none;
  border: 1px solid #ccc;
  border-radius: 4px;
  -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
  box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
  -webkit-transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
  transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
    }
</style>
<script type="text/javascript">
    var specialKeys = new Array();
    specialKeys.push(8); //Backspace
    function IsNumeric(e) {
        var keyCode = e.which ? e.which : e.keyCode
        var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
        //document.getElementById("error").style.display = ret ? "none" : "inline";
        return ret;
    }
    </script>
    <%--<script type="text/javascript">
        $(function () {
            $('[id*=gvTickets] tr').each(function () {
                var toolTip = $(this).attr("title");
                $(this).find("td").each(function () {
                    $(this).simpletip({
                        content: toolTip
                    });
                });
                $(this).removeAttr("title");
            });
        });
</script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form runat="server">
    <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>View Tickets</h3>
	    <ol class="breadcrumb">
			<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
			<li><i class="fa fa-pencil-square-o"></i>View Tickets</li>
		</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-body">
                 <table style="width: 100%;">
                    <tr>
                        <td>
                          <div id="totalCount" runat="server"> <strong>
                          Total # of tickets closed: <asp:Label ID="lblTotalTicket" runat="server" Text=""></asp:Label>
                          </strong> 
                          
                          </div>                  
                         </td>
                         <td><div id="count" runat="server"><strong> 
                         Ticket closed last week:<asp:Label ID="lblLastWeekTickets" runat="server" Text=""></asp:Label>                           
                         </strong></div></td>
                         <td><div id="filter" runat="server"><strong>Filter:</strong><asp:DropDownList ID="ddlFilter" runat="server" 
                                 class="ddl" AutoPostBack="True" 
                                 onselectedindexchanged="ddlFilter_SelectedIndexChanged">                                
                                <asp:ListItem Value="Open">Open tickets</asp:ListItem>
                                <asp:ListItem Value="Closed">Closed tickets</asp:ListItem>
                             </asp:DropDownList></div> </td>
                    </tr>
                    <tr>
                        <td style="text-align: left; vertical-align: top;">
                            
                        </td>
                        <td>
                        &nbsp;
                            </td>
                        <td>
                            <asp:Label ID="lblEmpId" runat="server" Visible="False"></asp:Label>
                            <asp:Label ID="lblAccess" runat="server" Visible="False"></asp:Label>
                            <asp:Label ID="lblUser" runat="server" Visible="False"></asp:Label>
                            <asp:Label ID="lblTeam" runat="server" Visible="False"></asp:Label>
                            <asp:Label ID="lblError" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        </td>
                    </tr>                   
       
                </table>
                <div class="table-responsive">
                     
                     <asp:Panel ID="Panel1" runat="server" Height="350px" ScrollBars="Both">                             
                                <asp:GridView ID="gvTickets" runat="server" AutoGenerateColumns="False" Width="100%" 
                                    onrowcommand="gvTickets_RowCommand" 
                                    onrowdatabound="gvTickets_RowDataBound" OnDataBound="OnDataBound" class="table table-striped table-bordered table-hover">
                                    <Columns>
                                        <asp:TemplateField HeaderText="View">
                                             <HeaderTemplate>
                                                <asp:Label ID="Label33" runat="server" Text="View"></asp:Label><br />
                                                <asp:TextBox ID="txtSearch33" runat="server" Width="1px" ReadOnly="true" Border="White" BorderColor="White" class="search_textbox"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Edit" 
                                            ImageUrl="~/images/View.png" CommandArgument='<%# Eval("T_ID") %>' />
                                            </ItemTemplate>                            
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ticket #">
                                            <HeaderTemplate>
                                                <asp:Label ID="Label3" runat="server" Text="Ticket #"></asp:Label><br />
                                                <asp:TextBox ID="txtSearch" runat="server" placeholder="Ticket #" class="search_textbox"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblTicketId" runat="server" Text='<%# Eval("T_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ticket Type">
                                            <HeaderTemplate>
                                                <asp:Label ID="Label1" runat="server" Text="Ticket Type"></asp:Label><br />
                                                <asp:TextBox ID="txtSearch1" runat="server" placeholder="Ticket Type" class="search_textbox"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblTicketType" runat="server" Text='<%# Eval("T_Ticket_Type") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Created By">
                                            <HeaderTemplate>
                                                <asp:Label ID="Label2" runat="server" Text="Created By"></asp:Label><br />
                                                <asp:TextBox ID="txtSearch2" runat="server" placeholder="Created By" class="search_textbox"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblCreatedBy" runat="server" Text='<%# Eval("UL_User_Name") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Created Date">
                                            <HeaderTemplate>
                                                <asp:Label ID="Label4" runat="server" Text="Created Date"></asp:Label><br />
                                                <asp:TextBox ID="txtSearch3" runat="server" placeholder="Created Date" class="search_textbox"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblCreatedDate" runat="server" Text='<%# Eval("T_Issued_Date_Time") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Assigned To">
                                            <HeaderTemplate>
                                                <asp:Label ID="Label5" runat="server" Text="Assigned To"></asp:Label><br />
                                                <asp:TextBox ID="txtSearch4" runat="server" placeholder="Assigned To" class="search_textbox"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblAssignedTo" runat="server" Text='<%# Eval("T_Assigned_To") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ETA">
                                            <HeaderTemplate>
                                                <asp:Label ID="Label6" runat="server" Text="ETA"></asp:Label><br />
                                                <asp:TextBox ID="txtSearch5" runat="server" placeholder="ETA" class="search_textbox"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblETA" runat="server" Text='<%# Eval("T_ETA_Date_Time") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status">
                                            <HeaderTemplate>
                                                <asp:Label ID="Label7" runat="server" Text="Status"></asp:Label><br />
                                                <asp:TextBox ID="txtSearch6" runat="server" placeholder="Status" class="search_textbox"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("T_Status") %>'></asp:Label>
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

