﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AvarageHoursByClient.aspx.cs" Inherits="AvarageHoursByClient" %>

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
             //             ShowProgress();
         });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
    <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Avarage Hours By Client</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>Avarage Hours By Client</li>
			</ol>
	 </div>
    </div>

    <div class="row">
       <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">              
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
                          <td style="text-align: right" width="40%">
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
                              <asp:Label ID="lblTo" runat="server" Text="To Date:"></asp:Label> </td>
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
                                  Text="Export" class="btn btn-primary"  />
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
    <div id="loadin" runat="server" class="loading" align="center"> 
        <img src="images/pleasewait.gif" height="40px" width="40px" alt="" /><br />
        Please wait...
     </div>
    <script type="text/javascript">
        //        $(function () {
        //            $("#txtFrom").datepicker({ maxDate: new Date() });
        //        });

        //        $(function () {
        //            //            $("#datepickerTo").datepicker({ maxDate: "+1M +10D" });
        //            $("#txtTo").datepicker({ maxDate: new Date() });
        //        });

        function FirstDayOfMonth(Year, Month) {
            return (new Date((new Date(Year, Month + 1 , 1)))).getDate();
        }
        function LastDayOfMonth(Year, Month) {
            return (new Date((new Date(Year, Month + 1, 1)) - 1)).getDate();
        }

        $('.txtSunDate').datepicker({
            beforeShowDay: function (date) {
                //getDate() returns the day (0-31)
                if (date.getDate() == FirstDayOfMonth(date.getFullYear(), date.getMonth())) {
                    return [true, ''];
                }
                return [false, ''];
            }
        });


//        $(function () {
//            $(".txtSunDate").datepicker({ maxDate: new Date(), beforeShowDay:
//             function (dt) {
//                 return [dt.getMonth() == 0, ""];
//             }
//            });
//     });

     $('.txtSatDate').datepicker({
         beforeShowDay: function (date) {
             //getDate() returns the day (0-31)
             if (date.getDate() == LastDayOfMonth(date.getFullYear(), date.getMonth())) {
                 return [true, ''];
             }
             return [false, ''];
         }
     });

//        $(function () {
//            $(".txtSatDate").datepicker({ maxDate: new Date(), beforeShowDay:
//             function (dt) {
//                 return [dt.getDay() == 6, ""];
//             }
//            });
//        });
  </script>
 </form>
</asp:Content>

