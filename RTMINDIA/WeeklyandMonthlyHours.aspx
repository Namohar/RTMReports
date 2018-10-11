<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WeeklyandMonthlyHours.aspx.cs" Inherits="WeeklyandMonthlyHours" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        var jQuery_1_8_3 = $.noConflict(true);
    </script>
    <%--<link rel="stylesheet" href="http://code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />--%>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
     <script src="//code.jquery.com/jquery-1.8.2.js"></script>
     <script src="//code.jquery.com/ui/1.8.24/jquery-ui.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
    <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Employee Hours By Week</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>Employee Hours By Week</li>
			</ol>
	 </div>
    </div>

    <div class="row">
       <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
              <div class="panel-heading" style="text-align:center">
                
              </div>

              <div class="panel-body">
                  <table style="width: 100%;">
                      <tr>
                          <td style="text-align: right" width="40%">
                              <asp:Label ID="lblLoc" runat="server" Text="Select Location:"></asp:Label>  </td>
                          <td>
                              <asp:DropDownList ID="ddlLocation" runat="server" class="form-control" style="width:250px">
                              </asp:DropDownList>
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
                              <asp:Label ID="lblFrom" runat="server" Text="From Date:"></asp:Label> </td>
                          <td>
                              <asp:TextBox ID="txtFrom" runat="server" ClientIDMode="Static" class="form-control txtSunDate" style="width:250px"></asp:TextBox>
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
                              <asp:Label ID="lblTo" runat="server" Text="To Date:" Visible="false"></asp:Label> </td>
                          <td>
                              <asp:TextBox ID="txtTo" runat="server" ClientIDMode="Static" class="form-control txtSatDate" style="width:250px" Visible="false"></asp:TextBox>
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
                              <asp:Button ID="btnExport" runat="server" onclick="btnExport_Click" 
                                  Text="Export Week Data" class="btn btn-primary"  />&nbsp;&nbsp;
                              <asp:Button ID="btnExportMonth" runat="server" onclick="btnExportMonth_Click" 
                                  Text="Export Month Data" class="btn btn-primary"  />
                          </td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                              &nbsp;</td>
                          <td>
                              &nbsp;</td>
                      </tr>
                  </table> 
              </div>
            </div>
        </div>
    </div>
    <style type="text/css">
    .ui-datepicker-calendar {
        display: none;
    }
</style>
    <script type="text/javascript">
        //        $(function () {
        //            $("#txtFrom").datepicker({ maxDate: new Date() });
        //        });

        //        $(function () {
        //            //            $("#datepickerTo").datepicker({ maxDate: "+1M +10D" });
        //            $("#txtTo").datepicker({ maxDate: new Date() });
        //        });

//        $(function () {
//            $(".txtSunDate").datepicker({ maxDate: new Date() });
//        });

//        $(function () {
//            $(".txtSatDate").datepicker({ maxDate: new Date() });
//        });

        $(function () {
            $('.txtSunDate').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'MM yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                }
            });
        });

        $(function () {
            $('.txtSatDate').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'MM yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                }
            });
        });
  </script>
 </form>
</asp:Content>

