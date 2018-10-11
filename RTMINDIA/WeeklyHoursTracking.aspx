<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WeeklyHoursTracking.aspx.cs" Inherits="WeeklyHoursTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>RTM Tracking Report for Specific Hours</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>RTM Tracking Report for Specific Hours</li>
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
                              <asp:Label ID="lblTeam" runat="server" Text="Select Team:"></asp:Label>  </td>
                          <td>
                              <asp:DropDownList ID="ddlTeam" runat="server" class="form-control" style="width:250px">
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
                              <asp:Label ID="lblFrom" runat="server" Text="Week Start Date:"></asp:Label> </td>
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
                              <asp:TextBox ID="txtTo" runat="server" ClientIDMode="Static" class="form-control txtSatDate" Visible="false" style="width:250px"></asp:TextBox>
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
                              <asp:Button ID="btnExportLessHours" runat="server" onclick="btnExportLessHours_Click" 
                                  Text="Export Hours < 38.75" class="btn btn-primary"  />
                              <asp:Button ID="btnExportMoreHours" runat="server" onclick="btnExportMoreHours_Click" 
                                  Text="Export Hours > 45" class="btn btn-primary"  />
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

    <script type="text/javascript">
        //        $(function () {
        //            $("#txtFrom").datepicker({ maxDate: new Date() });
        //        });

        //        $(function () {
        //            //            $("#datepickerTo").datepicker({ maxDate: "+1M +10D" });
        //            $("#txtTo").datepicker({ maxDate: new Date() });
        //        });

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