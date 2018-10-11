<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="RUChart, App_Web_ecwshzyq" validaterequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form id="Form1" runat="server">
   <div class="row">
	<div class="col-lg-12">
	 <h3 class="page-header"><i class="fa fa-bar-chart-o"></i>RU</h3>
	 <ol class="breadcrumb">
		<li><i class="fa fa-home"></i><a href="DashBoard.aspx">Home</a></li>
		<li><i class="fa fa-bar-chart-o"></i>RU</li>
	 </ol>
	</div>
  </div>

  <div class="row">
     <div class="col-md-12 col-sm-12 col-xs-12">
         <div class="panel panel-default">
             <div class="panel-body">
                 <asp:Chart ID="Chart1" runat="server" Width="900px" Height="450px" 
                        oncustomize="Chart1_Customize">
                    <Series>
                        <asp:Series Name="Series1">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                            <AxisY TitleFont="Microsoft Sans Serif, 10pt, style=Bold">
                            </AxisY>
                            <AxisX TitleFont="Microsoft Sans Serif, 10pt, style=Bold">
                            </AxisX>
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>  
                <br />
                <br />
                <div>
                  <asp:Button ID="btnExport" class="btn btn-primary" runat="server" Text="Export" onclick="btnExport_Click" />
                  <asp:Button ID="btnPrint" class="btn btn-primary" runat="server" Text="PDF" 
                        onclick="btnPrint_Click" />
                </div>           
             </div>
         </div>
     </div>
   </div>
</form>
</asp:Content>

