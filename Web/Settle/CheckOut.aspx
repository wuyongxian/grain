<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOut.aspx.cs" Inherits="Web.Settle.CheckOut" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <link href="../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   
    <script src="../Scripts/My97DatePicker/calendar.js" type="text/javascript"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {

              InitWBID();
              $('#QWBID').change(function () {
                  if ($('#QWBID option:selected').val() != "") {
                      $('#txtWBID').val($('#QWBID option:selected').text());
                  }
              });
          });
          function InitWBID() {
              var WBName = $.trim($('#txtWBID').val());
              $('#QWBID').empty();
              $.ajax({
                  url: '/Ashx/wbinfo.ashx?type=GetWBByName&strName=' + WBName,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (WBName == '') {
                          $('#QWBID').append("<option value=''>--请选择--</option>");
                      }
                      if (r.responseText == "Error") { return false; }
                      for (var i = 0; i < r.length; i++) {
                          $('#QWBID').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                      }

                  }, error: function (r) {

                  }
              });
          }

     


      </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>原粮出库操作</b>
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>网点名称:</span>
                    </td>
                    <td>
                        <input type="text" id="txtWBID" onkeyup="InitWBID();" style="font-size: 16px; width: 100px;height:26px; font-weight: bolder;" runat="server" />
                        <select id="QWBID" runat="server" style="width: 100px;height:30px;"></select></td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                            runat="server" OnClick="ImageButton1_Click" />
                    </td>
                    <td><a href="CheckOutHistory.aspx">出库记录查询</a></td>
                    <td><asp:Button ID="btnOutPut" runat="server" class="outputExcel" Text="Excel"
                        style="width:50px;margin-left:20px;"    OnClick="btnOutPut_Click" /></td>
                </tr>
            </table>
        </div>

        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 200px; height: 20px; text-align: center;">网点名称
                        </th>
                        <th style="width: 100px; text-align: center;">品种
                        </th>
                         <th style="width: 50px; text-align: center;">等级
                        </th>
                        <%-- <th style="width: 100px; text-align: center;">
                           等级
                        </th>--%>
                        <th style="width: 150px; text-align: center;">库存
                        </th>

                        <th style="width: 120px; text-align: center;">出库
                        </th>

                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td align="center" style="height: 25px;">

                        <%#Eval("WBName")%>
                     
                    </td>
                    <td align="center">
                        <%#Eval("VarietyName")%>
                    </td>
                     <td align="center">
                        <%#Eval("VarietyLevelName")%>
                    </td>
                    <%-- <td align="center">
                   <%#GetVarietyLevel(Eval("VarietyName"))%>
                   </td>--%>
                    <td align="center">
                        <%#Eval("numStorage")%>
                    </td>

                    <td align="center">
                        <%-- <a href="/Settle/CheckOutDetail.aspx?ID=<%#Eval("ID") %>&WBID=<%#Eval("WBID") %>&VarietyID=<%#Eval("VarietyID") %>">出库</a>--%>
                        <%#OutPut(Eval("ID"),Eval("WBID"),Eval("VarietyID"),Eval("VarietyLevelID")) %>
                    </td>

                </tr>
            </ItemTemplate>
            <FooterTemplate>

                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>

        <div style="display: none;">
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>


</body>
</html>