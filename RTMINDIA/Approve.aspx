<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Approve.aspx.cs" Inherits="Approve" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
 <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        var jQuery_1_8_3 = $.noConflict(true);
    </script>
    <%--<link rel="stylesheet" href="http://code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />--%>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
     <script src="//code.jquery.com/jquery-1.8.2.js"></script>
     <script src="//code.jquery.com/ui/1.8.24/jquery-ui.js"></script>

     <script type="text/javascript">
         $("[id*=chkHeader]").live("click", function () {
             var chkHeader = $(this);
             var grid = $(this).closest("table");
             $(("input[type=checkbox]"), grid).each(function () {
                 if (chkHeader.is(":checked")) {
                     if (!this.disabled) {
                         $(this).attr("checked", "checked");
                         $("td", $(this).closest("tr")).addClass("selected");
                     }
                 } else {
                     $(this).removeAttr("checked");
                     $("td", $(this).closest("tr")).removeClass("selected");
                 }
             });
         });
         $("[id*=chkRow]").live("click", function () {
             var grid = $(this).closest("table");
             var chkHeader = $("[id*=chkHeader]", grid);
             if (!$(this).is(":checked")) {
                 $("td", $(this).closest("tr")).removeClass("selected");
                 chkHeader.removeAttr("checked");
             } else {
                 $("td", $(this).closest("tr")).addClass("selected");
                 if ($("[id*=chkRow]", grid).length == $("[id*=chkRow]:checked", grid).length) {
                     chkHeader.attr("checked", "checked");
                 }
             }
         });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-edit"></i>Approvals Report</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-pencil-square-o"></i>Approvals Report</li>
			</ol>
	 </div>
    </div>

    <div class="row">
      <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
             <div class="panel-heading" style="text-align:center">
                 <asp:RadioButton ID="rbUser" runat="server" AutoPostBack="True" Checked="True" 
                            Font-Bold="True" GroupName="user" oncheckedchanged="rbUser_CheckedChanged" 
                            Text="User" />
                  <asp:RadioButton ID="rbTeam" runat="server" AutoPostBack="True" 
                            Font-Bold="True" GroupName="user" oncheckedchanged="rbTeam_CheckedChanged" 
                            Text="Team" />
              </div>
             <div class="panel-body">
                 <table style="width: 100%;">
                      <tr>
                          <td style="text-align: right" width="40%">
                              <asp:Label ID="lblEmp" runat="server" Text="Select Employee:"></asp:Label>  </td>
                          <td>
                              <asp:DropDownList ID="ddlEmp" runat="server" class="form-control" style="width:250px">
                              </asp:DropDownList>
                          </td>
                      </tr>
                      <tr>
                          <td style="text-align: right" width="40%">
                              &nbsp;</td>
                          <td>
                              &nbsp;</td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                              <asp:Label ID="lblFrom" runat="server" Text="Select Date:"></asp:Label> </td>
                          <td>
                              <asp:TextBox ID="txtFrom" runat="server" ClientIDMode="Static" class="form-control txtSunDate" style="width:250px"></asp:TextBox>
                          </td>
                      </tr>                     
                      <tr>
                          <td style="text-align: right">
                              <asp:Label ID="lblTo" runat="server" Text="To Date:" Visible="false"></asp:Label> </td>
                          <td>
                              <asp:TextBox ID="txtTo" runat="server" ClientIDMode="Static" class="form-control txtSatDate" Visible="false" style="width:250px"></asp:TextBox>
                          </td>
                      </tr>
                      <tr>
                          <td style="text-align: right" width="40%">
                              &nbsp;</td>
                          <td>
                              &nbsp;</td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                              &nbsp;</td>
                          <td>
                              <asp:Button ID="btnDisplay" runat="server"  
                                  Text="Display" class="btn btn-primary" 
                                  OnClientClick="this.disabled = true; this.value = 'Please wait..';" 
                                  UseSubmitBehavior="false" onclick="btnDisplay_Click" />
                          </td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                              &nbsp;</td>
                          <td>
                              &nbsp;</td>
                      </tr>
                  </table> 
                  <br />
                  <div id="dateDisplay" runat="server" visible="false" >
                      <asp:Button ID="btnApproveAll" runat="server" Text="Approve Selected Users" 
                          class="btn btn-success" onclick="btnApproveAll_Click" />
                      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                      <asp:Label ID="lblRepDate" runat="server" Text="Report Dates:" Font-Bold="true" Font-Size="20px"></asp:Label>
                      <asp:Label ID="lblFromDate" runat="server" Text="" Font-Bold="true" Font-Size="20px"></asp:Label>
                      <asp:Label ID="Label1" runat="server" Text="-" Font-Bold="true" Font-Size="20px"></asp:Label>
                      <asp:Label ID="lblToDate" runat="server" Text="" Font-Bold="true" Font-Size="20px"></asp:Label>
                  </div>
                  <br />
                  <div class="table-responsive">
                      <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False" 
                          class="table table-striped table-bordered table-hover" onrowdatabound="gvList_RowDataBound" 
                          >
                          <Columns>
                              <asp:TemplateField>
                                   <HeaderTemplate>
                                        &nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkHeader" runat="server"  />
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:CheckBox ID="chkRow" runat="server" />
                                  </ItemTemplate>
                                   <HeaderStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                              <asp:TemplateField HeaderText="Action">
                                  <ItemTemplate>
                                      <asp:Button ID="btnApprove" runat="server" Text="Approve" Width="100px" 
                                          class="btn btn-success" onclick="btnApprove_Click" OnClientClick="this.disabled = true; this.value = 'Please wait..';" 
                                  UseSubmitBehavior="false" /><br /><br />
                                      <asp:Button ID="btnReject" runat="server" Text="Reject" Width="100px" 
                                          class="btn btn-warning" onclick="btnReject_Click" OnClientClick="this.disabled = true; this.value = 'Please wait..';" 
                                  UseSubmitBehavior="false" />
                                  </ItemTemplate>
                                  <HeaderStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                              <asp:TemplateField HeaderText="Employee">
                                  <ItemTemplate>
                                      <asp:Label ID="lblEmployee" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
                                  </ItemTemplate>
                                  <HeaderStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                              <asp:TemplateField HeaderText="Total Hours">
                                  <ItemTemplate>
                                      <asp:Label ID="lblHours" runat="server" Font-Size="30px" Text='<%# Eval("TotalHours") %>'></asp:Label>
                                      <br />
                                      <asp:LinkButton ID="lnkView" runat="server" Font-Size="10px" 
                                          onclick="lnkView_Click">View Details</asp:LinkButton>
                                  </ItemTemplate>
                                  <HeaderStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                              <asp:TemplateField HeaderText="Employee Submitted On">
                                  <ItemTemplate>
                                      <asp:Label ID="lblSubmittedOn" runat="server" Text='<%# Eval("SubmittedOn") %>'></asp:Label>
                                  </ItemTemplate>
                                  <HeaderStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                              <asp:TemplateField HeaderText="Manager Approved On">
                                  <ItemTemplate>
                                      <asp:Label ID="lblApprovedOn" runat="server" Text='<%# Eval("ApprovedOn") %>'></asp:Label>
                                  </ItemTemplate>
                                  <HeaderStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                              <asp:TemplateField HeaderText="Approve" Visible="false">
                                  <ItemTemplate>
                                      <asp:Label ID="lblApproveStatus" runat="server" Text='<%# Eval("Approve") %>'></asp:Label>
                                      <asp:Label ID="lblSubmitStatus" runat="server" Text='<%# Eval("Submit") %>'></asp:Label>
                                  </ItemTemplate>
                                  <HeaderStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                          </Columns>
                          <HeaderStyle HorizontalAlign="Center" />
                          <RowStyle HorizontalAlign="Center" />
                      </asp:GridView>
                  </div>
             </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(function () {
            $(".txtSunDate").datepicker({ maxDate: new Date(), beforeShowDay:
             function (dt) {
                 return [dt.getDay() == 0, ""];
             }
            });
        });

        $(function () {
            $(".txtSatDate").datepicker({ maxDate: new Date(), beforeShowDay:
             function (dt) {
                 return [dt.getDay() == 6, ""];
             }
            });
        });
  </script>
</form>
</asp:Content>

