<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="NonComplianceReport, App_Web_ecwshzyq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
 <form id="Form1" runat="server">
    <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Non-Compliance Report</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>Non-Compliance Report</li>
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
 </form>
</asp:Content>

