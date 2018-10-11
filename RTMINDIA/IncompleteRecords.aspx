<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="IncompleteRecords.aspx.cs" Inherits="IncompleteRecords" %>
<%@ Register assembly="DropDownCheckBoxes" namespace="Saplin.Controls" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        var jQuery_1_8_3 = $.noConflict(true);
    </script>
    <%--<link rel="stylesheet" href="http://code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />--%>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
      <script src="//code.jquery.com/jquery-1.8.2.js"></script>
      <script src="//code.jquery.com/ui/1.8.24/jquery-ui.js"></script>

      <style type="text/css">
        .loading
        {        
            display: none;
            position: fixed;
            background-color: White;       
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
          $('form').live("submit", function () {
              ShowProgress();
          });
       </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Missing Client</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>Missing Client</li>
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
                       <td style="text-align: right">
                           <asp:Label ID="lblEmp" runat="server" Text="Employee:" Font-Bold="True"></asp:Label></td>
                       <td width="60%">
                          <asp:DropDownList ID="ddlEmp" runat="server" 
                                    onselectedindexchanged="ddlEmp_SelectedIndexChanged" AutoPostBack="True" class="form-control" >
                                </asp:DropDownList>
                       </td>
                   </tr>
                   <tr>
                       <td style="text-align: right">
                           &nbsp;
                       </td>
                       <td>
                          &nbsp;
                       </td>
                   </tr>
                   <tr>
                       <td style="text-align: right">
                           <asp:Label ID="lblDate" runat="server" Font-Bold="True" Text="Date:"></asp:Label>    
                       </td>
                       <td>
                           <input id="datepicker" runat="server" clientidmode="Static" type="text" class="form-control" />
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
                            <asp:Label ID="lblTo" runat="server" Font-Bold="True" Text="To:" 
                                  ></asp:Label></td>
                       <td>
                          <input id="datepickerTo" runat="server" clientidmode="Static" type="text" 
                                  class="form-control" />
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
                           <asp:Button ID="btnPrint" runat="server" class="btn btn-primary" 
                                  onclick="btnPrint_Click" Text="Display" />
                              &nbsp;<asp:Button ID="btnReset" runat="server" class="btn btn-primary" 
                                  onclick="btnReset_Click" Text="Reset" />
                      </td>
                   </tr>
               </table>

               <div class="table-responsive">
                  <asp:GridView ID="gvIncorrectData" runat="server" AutoGenerateColumns="False" Visible="False" 
                    Width="100%" onrowcancelingedit="gvIncorrectData_RowCancelingEdit" 
                    onrowdatabound="gvIncorrectData_RowDataBound" 
                    onrowediting="gvIncorrectData_RowEditing" 
                    onrowupdating="gvIncorrectData_RowUpdating" DataKeyNames="R_ID" class="table table-striped table-bordered table-hover">
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="False">
                            <ItemTemplate>
                                <asp:Label ID="lblid" runat="server" Text='<%# Eval("R_ID") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User Name">
                            <ItemTemplate>
                                <asp:Label ID="lblUser" runat="server" Text='<%# Eval("R_User_Name") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Date">
                            <ItemTemplate>
                                <asp:Label ID="lblDate" runat="server" Text='<%# Eval("R_Start_Date_Time") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Client">
                            <ItemTemplate>
                                <asp:Label ID="lblClient" runat="server" Text='<%# Eval("CL_ClientName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <cc1:dropdowncheckboxes ID="ddlMultiple" runat="server" 
                                    style="top: 0px; left: 0px; height: 16px; width: 202px" >
                                    <Style DropDownBoxBoxHeight="130" DropDownBoxBoxWidth="250" 
                                        SelectBoxWidth="200" />
                                </cc1:dropdowncheckboxes>
                            </EditItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Task">
                            <ItemTemplate>
                                <asp:Label ID="lblTask" runat="server" Text='<%# Eval("TL_Task") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="SubTask">
                            <ItemTemplate>
                                <asp:Label ID="lblSubTask" runat="server" Text='<%# Eval("STL_SubTask") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Duration">
                            <ItemTemplate>
                                <asp:Label ID="lblDuration" runat="server" Text='<%# Eval("R_Duration") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Comments">
                            <ItemTemplate>
                                <asp:Label ID="lblComments" runat="server" Text='<%# Eval("R_Comments") %>'></asp:Label>
                            </ItemTemplate>
                           <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" />
                    </Columns>
                </asp:GridView>
               </div>
           </div>
        </div>
     </div>
  </div>
  <div class="loading" align="center"> 
     <img src="images/pleasewait.gif" height="40px" width="40px" alt="" /><br />
     Please wait...
  </div>
</form>
<script type="text/javascript">
    $(function () {
        $("#datepicker").datepicker({ maxDate: new Date() });
    });

    $(function () {
        //            $("#datepickerTo").datepicker({ maxDate: "+1M +10D" });
        $("#datepickerTo").datepicker({ maxDate: new Date() });
    });
  </script>
  </asp:Content>