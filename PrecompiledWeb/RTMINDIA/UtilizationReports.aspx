<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="UtilizationReports, App_Web_ecwshzyq" validaterequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link href="PivotStyle.css" rel="Stylesheet" type="text/css" />
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
  <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Utilization Reports</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>Utilization Reports</li>
			</ol>
	</div>

     <div class="row">
       <div class="col-md-12 col-sm-12 col-xs-12">
           <div class="panel panel-default">
              <div class="panel-heading" style="text-align:center">
                <asp:RadioButton ID="rbClient" runat="server" Font-Bold="True" 
                     GroupName="RTM" Text="By Client"  
                    Checked="True" AutoPostBack="True" 
                    oncheckedchanged="rbClient_CheckedChanged" />
                <asp:RadioButton ID="rbTask" runat="server" Font-Bold="True" 
                    GroupName="RTM" Text="By Task" AutoPostBack="True" 
                    oncheckedchanged="rbTask_CheckedChanged" />

                <asp:RadioButton ID="rbSubTask" runat="server" Font-Bold="True"  
                    GroupName="RTM" Text="By SubTask" AutoPostBack="True" oncheckedchanged="rbSubTask_CheckedChanged" 
                     />
               
                <asp:RadioButton ID="rbServiceCode" runat="server" AutoPostBack="True" 
                    Font-Bold="True"  GroupName="RTM" 
                    oncheckedchanged="rbServiceCode_CheckedChanged" Text="Service Code%" />
               
                <asp:RadioButton ID="rbTaskPer" runat="server" AutoPostBack="True" 
                    Font-Bold="True"  GroupName="RTM" 
                    oncheckedchanged="rbTaskPer_CheckedChanged" Text="Task %" />
               
                <asp:RadioButton ID="rbJobCode" runat="server" AutoPostBack="True" 
                    Font-Bold="True"  GroupName="RTM" 
                    oncheckedchanged="rbJobCode_CheckedChanged" Text="Job Code %" />
               
                <asp:RadioButton ID="rbProductCode" runat="server" AutoPostBack="True" 
                    Font-Bold="True"  GroupName="RTM" 
                    oncheckedchanged="rbProductCode_CheckedChanged" Text="Product Code" />
                <asp:RadioButton ID="rbRU" runat="server" AutoPostBack="True" Font-Bold="True" 
                     GroupName="RTM" oncheckedchanged="rbRU_CheckedChanged" 
                    Text="RU%(User)" />
                <asp:RadioButton ID="rbRUTeam" runat="server" AutoPostBack="True" Font-Bold="True" 
                     GroupName="RTM" oncheckedchanged="rbRUTeam_CheckedChanged" 
                    Text="RU%(Team)" />
               
                <asp:RadioButton ID="RadioButton1" runat="server" AutoPostBack="True" 
                    Font-Bold="True"  GroupName="RTM" 
                    oncheckedchanged="RadioButton1_CheckedChanged" Text="Peer Support" />
              </div>

              <div class="panel-body">
                 <div style="text-align:center; width: 100%;"> 
                    <asp:RadioButton ID="rbUser" runat="server" AutoPostBack="True" Checked="True" 
                        Font-Bold="True" GroupName="user" oncheckedchanged="rbUser_CheckedChanged" 
                        Text="User" />
                    <asp:RadioButton ID="rbTeam" runat="server" AutoPostBack="True" 
                        Font-Bold="True" GroupName="user" oncheckedchanged="rbTeam_CheckedChanged" 
                        Text="Team" />
                    </div>  
                    <br />

                    <table style="width: 100%;">
                      <tr>
                          <td colspan="2">
                              <asp:Label ID="lblError" runat="server" Font-Bold="True"></asp:Label>
                          </td>
                      </tr>
                      <tr>
                          <td style="text-align: right" width="40%">
                              <asp:Label ID="lblEmp" runat="server" Text="Employee:" Font-Bold="True"></asp:Label>
                          </td>
                          <td width="60%">
                              <asp:DropDownList ID="ddlEmp" runat="server" 
                                    onselectedindexchanged="ddlEmp_SelectedIndexChanged" AutoPostBack="True" class="form-control" >
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
                          <td style="text-align: right" width="40%">
                              <asp:Label ID="lblTask" runat="server" Font-Bold="True" Text="Task:" 
                                 Visible="False"></asp:Label>
                         </td>
                          <td>
                             <asp:DropDownList ID="ddlTask" runat="server" Visible="False" class="form-control">
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
                              <asp:Label ID="lblDate" runat="server" Font-Bold="True" Text="Date:"></asp:Label>
                          </td>
                          <td>
                              <input id="datepicker" runat="server" clientidmode="Static" type="text" class="form-control" /></td>
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
                                  ></asp:Label>
                          </td>
                          <td>
                              <input id="datepickerTo" runat="server" clientidmode="Static" type="text" 
                                   class="form-control" /></td>
                      </tr>
                      <tr>
                          <td style="text-align: right">
                          </td>
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
                              &nbsp;<asp:Button ID="btnAllSubTasks" runat="server" Text="Export Details" 
                    class="btn btn-primary" onclick="btnAllSubTasks_Click" Visible="False" />
                          </td>
                      </tr>
                  </table>

                  <div style="text-align: center; overflow:auto; width:100%">
                    <asp:Chart ID="Chart1" runat="server" Height="600px" Width="1200px"
                        BackSecondaryColor="White" Visible="False" BorderlineColor="DarkBlue" 
                        BorderlineDashStyle="Dash">
                        <Series>
                            <asp:Series Name="Series1" CustomProperties="PointWidth=0.8">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1" ShadowColor="White">
                                <AxisY TitleFont="Microsoft Sans Serif, 10pt, style=Bold">
                                </AxisY>
                                <AxisX TitleFont="Microsoft Sans Serif, 10pt, style=Bold">
                                </AxisX>
                                <%--<Area3DStyle Enable3D="True" LightStyle="Realistic" />--%>
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                    <label title="Test"></label>
                  </div>
                  <div>
                        <asp:Button ID="btnExport" runat="server" Font-Bold="True" Text="Export Data" 
                        class="btn btn-primary" onclick="btnExport_Click" Visible="False" />
    &nbsp;
                    <asp:Button ID="btnPDF" runat="server" Font-Bold="True" Text="PDF" 
                        class="btn btn-primary" onclick="btnPDF_Click" Visible="False" />
                  </div>
                  <br />
                  <div class="table-responsive">
                     <asp:GridView ID="gvPeerSupport" runat="server" AutoGenerateColumns="False" Visible="False" 
                        Width="100%" class="table table-striped table-bordered table-hover">
                        <Columns>
                            <asp:BoundField DataField="User Name" HeaderText="User Name">
                            <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Date" HeaderText="Date">
                            <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Duration" HeaderText="Duration">
                            <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Clients" HeaderText="Clients">
                            <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Comments" HeaderText="Comments">
                            <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Supported Members" HeaderText="Supported Members">
                            <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                
                      <div id="div2" runat="server" visible= "false" align="center">
                              <asp:Label ID="lblHeading" runat="server" 
                          Text="" Font-Bold="True" 
                          Font-Size="Medium" ForeColor="#003399" Visible ="False"></asp:Label>
                      </div>
                      <br />
                      <asp:Button ID="btnDetail" class="btn btn-primary" runat="server" 
                    Text="Detail View" onclick="btnDetail_Click" Visible="False" />
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

