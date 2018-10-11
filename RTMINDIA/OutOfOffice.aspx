<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="OutOfOffice.aspx.cs" Inherits="OutOfOffice" %>

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

      <script type = "text/javascript">
          function Check_Click(objRef) {
              //Get the Row based on checkbox
              var row = objRef.parentNode.parentNode;
              if (objRef.checked) {
                  //If checked change color to Aqua
                  row.style.backgroundColor = "#C2D69B";
              }
              else {
                  //If not checked change back to original color
                  if (row.rowIndex % 2 == 0) {
                      //Alternating Row Color
                      row.style.backgroundColor = "#C2D69B";
                  }
                  else {
                      row.style.backgroundColor = "white";
                  }
              }

              //Get the reference of GridView
              var GridView = row.parentNode;

              //Get all input elements in Gridview
              var inputList = GridView.getElementsByTagName("input");

              for (var i = 0; i < inputList.length; i++) {
                  //The First element is the Header Checkbox
                  var headerCheckBox = inputList[0];

                  //Based on all or none checkboxes
                  //are checked check/uncheck Header Checkbox
                  var checked = true;
                  if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) {
                      if (!inputList[i].checked) {
                          checked = false;
                          break;
                      }
                  }
              }
              headerCheckBox.checked = checked;

          }

//          $(function () {
//              $("[id*=CheckBox1]").bind("click", function () {

//                  //Find and reference the GridView.
//                  var grid = $(this).closest("table");

//                  //Find and reference the Header CheckBox.
//                  //var chkHeader = $("[id*=chkHeader]", grid);

//                  //If the CheckBox is Checked then enable the TextBoxes in thr Row.
//                  if (!$(this).is(":checked")) {
//                      var td = $("td", $(this).closest("tr"));
//                      td.css({ "background-color": "#FFF" });
//                      $("input[type=text]", td).attr("disabled", "disabled");
//                  } else {
//                      var td = $("td", $(this).closest("tr"));
//                      td.css({ "background-color": "#D8EBF2" });
//                      $("input[type=text]", td).removeAttr("disabled");
//                  }

//                  //Enable Header Row CheckBox if all the Row CheckBoxes are checked and vice versa.
//                  if ($("[id*=CheckBox1]", grid).length == $("[id*=CheckBox1]:checked", grid).length) {
//                      chkHeader.attr("checked", "checked");
//                  } else {
//                      chkHeader.removeAttr("checked");
//                  }
//              });
//          });
</script>
<script type = "text/javascript">
    function checkAll(objRef) {
        var GridView = objRef.parentNode.parentNode.parentNode;
        var inputList = GridView.getElementsByTagName("input");
        for (var i = 0; i < inputList.length; i++) {
            //Get the Cell To find out ColumnIndex
            var row = inputList[i].parentNode.parentNode;
            if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                if (objRef.checked) {
                    //If the header checkbox is checked
                    //check all checkboxes
                    //and highlight all rows
                    row.style.backgroundColor = "aqua";
                    inputList[i].checked = true;
                }
                else {
                    //If the header checkbox is checked
                    //uncheck all checkboxes
                    //and change rowcolor back to original 
                    if (row.rowIndex % 2 == 0) {
                        //Alternating Row Color
                        row.style.backgroundColor = "#C2D69B";
                    }
                    else {
                        row.style.backgroundColor = "white";
                    }
                    inputList[i].checked = false;
                }
            }
        }
    }
</script>
<script type = "text/javascript">
    function MouseEvents(objRef, evt) {
        var checkbox = objRef.getElementsByTagName("input")[0];
        if (evt.type == "mouseover") {
            objRef.style.backgroundColor = "orange";
        }
        else {
            if (checkbox.checked) {
                objRef.style.backgroundColor = "aqua";
            }
            else if (evt.type == "mouseout") {
                if (objRef.rowIndex % 2 == 0) {
                    //Alternating Row Color
                    objRef.style.backgroundColor = "#C2D69B";
                }
                else {
                    objRef.style.backgroundColor = "white";
                }
            }
        }
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <form id="Form1" runat="server">
   <div class="row">
	 <div class="col-lg-12">
		<h3 class="page-header"><i class="fa fa-table"></i>Out of Office</h3>
	    	<ol class="breadcrumb">
				<li><i class="fa fa-home"></i><a href="DashBoard_Admin.aspx">Home</a></li>
				<li><i class="fa fa-table"></i>Out of Office</li>
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
               <br />
                <div id="dvAbsent" class="row" runat="server" visible="false">
                   <div class="col-md-6 col-sm-12 col-xs-12">
                      <div class="panel panel-default">
                         <div class="panel-body">
                             <asp:GridView ID="GridView1" runat="server" 
                                  AutoGenerateColumns="False" 
                                 class="table table-striped table-bordered table-hover"
                                  HorizontalAlign="Center" Visible="False">
                                  <Columns>
                                      <asp:TemplateField>
                                            <HeaderTemplate>
                                              <asp:CheckBox ID="checkAll" runat="server" Enabled="false" onclick = "checkAll(this);" />
                                            </HeaderTemplate> 
                                           <ItemTemplate>
                                             <asp:CheckBox ID="CheckBox1" runat="server" onclick = "Check_Click(this)" />
                                           </ItemTemplate> 
                                      </asp:TemplateField> 
                                      <asp:TemplateField HeaderText="User Name">
                                          <ItemTemplate>
                                              <asp:Label ID="lblName" runat="server" Text='<%# Eval("User") %>'></asp:Label>
                                          </ItemTemplate>
                                          <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" />
                                      </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Date">
                                          <ItemTemplate>
                                              <asp:Label ID="lblDate" runat="server" Text='<%# Eval("MissedDate") %>'></asp:Label>
                                          </ItemTemplate>
                                          <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" />
                                      </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Reason" Visible="false">
                                          <ItemTemplate>
                                              <asp:DropDownList ID="ddlReason" runat="server" Width="150px">
                                              </asp:DropDownList>
                                          </ItemTemplate>
                                         <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" />
                                      </asp:TemplateField>
                                      <asp:TemplateField Visible="false">
                                          <ItemTemplate>
                                              <asp:LinkButton ID="lbUpdate" runat="server" Font-Bold="True" Font-Size="Small" 
                                                  >Update</asp:LinkButton>
                                          </ItemTemplate>
                                      </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Total Work Hours">
                                          <ItemTemplate>
                                              <asp:Label ID="lblWork" runat="server" Text='<%# Eval("totalHours") %>' ></asp:Label>
                                          </ItemTemplate>
                                      </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Absent Hours">
                                          <ItemTemplate>
                                              <asp:TextBox ID="txtAbsent" runat="server" class="decimalPt roundUp" Text='<%# Eval("absentHours") %>' Width="40px" ClientIDMode="Static"></asp:TextBox>
                                          </ItemTemplate>
                                      </asp:TemplateField>
                                  </Columns>
                                  <HeaderStyle />
                              </asp:GridView>
                         </div>
                      </div>
                   </div>

                   <div class="col-md-4 col-sm-12 col-xs-12">
                        <div class="panel panel-default">
                             <div class="panel-body">
                                <div class="form-group">
                                   <asp:Label ID="lblReason" runat="server" Text="Select Reason:"></asp:Label>
                                   <asp:DropDownList ID="ddlReasonNew" runat="server" class="form-control">
                                   </asp:DropDownList>
                                   <br />
                                    <asp:Button ID="btnAbsent" runat="server" Text="Submit" class="btn btn-primary"
                                        onclick="btnAbsent_Click" />
                                </div>
                             </div>
                        </div>
                    </div>
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

    function isNumberKey(evt, element) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode == 46 || charcode == 8))
            return false;
        else {
            var len = $(element).val().length;
            var index = $(element).val().indexOf('.');
            if (index > 0 && charCode == 46) {
                return false;
            }
            if (index > 0) {
                var CharAfterdot = (len + 1) - index;
                if (CharAfterdot > 3) {
                    return false;
                }
            }

        }
        return true;
    }

    $('.decimal').keyup(function () {
        var val = $(this).val();
        if (isNaN(val)) {
            val = val.replace(/[^0-9\.]/g, '');
            if (val.split('.').length > 2)
                val = val.replace(/\.+$/, "");
        }
        $(this).val(val);
    });

    $('#decimalPt, .decimalPt').keypress(function (evt) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode == 8 || charCode == 37) {
            return true;
        } else if (charCode == 46 && $(this).val().indexOf('.') != -1) {
            return false;
        } else if (charCode > 31 && charCode != 46 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    });

    $('.roundUp').blur(function () {
        var value = parseFloat($(this).val());
        if (value > 8) {
            value = 8;
        }
        if (!isNaN(value)) {
            $(this).val(parseFloat(value).toFixed(1));
        }
    });
  </script>
</asp:Content>

