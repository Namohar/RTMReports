<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ContentManagementReport.aspx.cs" Inherits="ContentManagementReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
   <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.1/jquery.min.js"
type = "text/javascript"></script>
<script type="text/javascript">
    var jQuery_1_4_1 = $.noConflict(true);
    </script>
   <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    
    <%--<link rel="stylesheet" href="http://code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />--%>
    <%--<link rel="stylesheet" href="//code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
     <script src="//code.jquery.com/jquery-1.8.2.js"></script>
     <script src="//code.jquery.com/ui/1.8.24/jquery-ui.js"></script>--%>
          


<script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"
type = "text/javascript"></script>
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css"
rel = "Stylesheet" type="text/css" />
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=txtSearch.ClientID %>").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "ContentManagementReport.aspx/GetCustomers",
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data.d, function (item) {
                            return {
                                label: item.split('-')[0],
                                val: item.split('-')[1]
                            }
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            },
            select: function (e, i) {
                var text = this.value.split(/,\s*/);
                text.pop();
                text.push(i.item.value);
                text.push("");
                this.value = text.join(",");
                var value = $("[id$=hfCustomerId]").val().split(/,\s*/);
                value.pop();
                value.push(i.item.val);
                value.push("");
                $("[id$=hfCustomerId]")[0].value = value.join(",");
                return false;
            },
            minLength: 1
        });

    });
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
    <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Content Management Report</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="#">Home</a></li>
				<li><i class="fa fa-table"></i>Content Management Report</li>
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
                              <asp:Label ID="lblMgr" runat="server" Text="Select Managers:"></asp:Label>  </td>
                          <td>
                              <asp:TextBox ID="txtSearch" runat="server" class="form-control"></asp:TextBox>
                              <asp:HiddenField ID="hfCustomerId" runat="server" />
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
                              <asp:Label ID="lblFrom" runat="server" Text="From Month:"></asp:Label> </td>
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
                              <asp:Label ID="lblTo" runat="server" Text="To Month:"></asp:Label> </td>
                          <td>
                              <asp:TextBox ID="txtTo" runat="server" ClientIDMode="Static" class="form-control txtSatDate" style="width:250px"></asp:TextBox>
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
                                  Text="Export Report" class="btn btn-primary"  />  
                                  &nbsp;&nbsp;&nbsp;&nbsp;
                                  <asp:Button ID="btnClear" runat="server" Text="Clear Managers" 
                                  onclick="btnClear_Click" class="btn btn-sm" />                            
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

