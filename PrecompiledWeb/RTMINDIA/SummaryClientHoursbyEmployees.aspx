<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="SummaryClientHoursbyEmployees, App_Web_as44pg0l" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<form id="Form1" runat="server">
  <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Summary Client Hours by Employees</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>Summary Client Hours by Employees</li>
			</ol>
	 </div>
    </div>

    <div class="row">
       <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
              <div class="panel-heading" style="text-align:center">
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
                              <asp:Label ID="lblLoc" runat="server" Text="Select Locatioon:" Visible="false"></asp:Label>  </td>
                          <td>
                              <asp:DropDownList ID="ddlLocation" runat="server" class="form-control" style="width:250px" Visible="false">
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
    </div>

    <script type="text/javascript">

        $(function () {
//            $(".txtSunDate").datepicker({ maxDate: new Date(), beforeShowDay:
//             function (dt) {
//                 return [dt.getDay() == 0, ""];
//             }
//            });

         $(".txtSunDate").datepicker({ maxDate: new Date() });
        });

        $(function () {
//            $(".txtSatDate").datepicker({ maxDate: new Date(), beforeShowDay:
//             function (dt) {
//                 return [dt.getDay() == 6, ""];
//             }
//         });
         $(".txtSatDate").datepicker({ maxDate: new Date() });
        });
  </script>
</form>
</asp:Content>

