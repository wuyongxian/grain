<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BusinessStatistics.aspx.cs" Inherits="Web.Admin.Commune.BusinessStatistics" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
   
    
    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
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
              var WBName =$.trim( $('#txtWBID').val());
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


                      //查找当前的网点位
                      $.ajax({
                          url: '/Ashx/wbinfo.ashx?type=GetWBLogin&strName=' + WBName,
                          type: 'post',
                          data: '',
                          dataType: 'json',
                          success: function (t) {

                              if (r.responseText == "Error") { return false; }
                              else {
                                  $('#QWBID').empty();
                                  $('#QWBID').append("<option value='" + t[0].ID + "'>" + t[0].strName + "</option>");
                                  $('#txtWBID').val(t[0].strName);
                                  $('#QWBID').attr('disabled', 'disabled');
                                  $('#txtWBID').attr('disabled', 'disabled');
                              }

                          }, error: function (r) {

                          }
                      });
                      //end


                  }, error: function (r) {

                  }
              });
          }




      </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>业务统计</b>
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>网点名称:</span>
                    </td>
                    <td>
                        <input type="text" id="txtWBID" onkeyup="InitWBID();" style="font-size: 16px; width: 100px; height: 26px; font-weight: bolder;" runat="server" />
                        <select id="QWBID" runat="server" style="width: 100px; height: 30px;"></select></td>
                    <td><span>日期:</span>
                    </td>
                    <td>
                        <input type="text" id="Qdtstart" readonly="readonly" onclick="WdatePicker();" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                        <span>-</span>
                        <input type="text" id="Qdtend" readonly="readonly" onclick="WdatePicker()" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                    </td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="~/images/search_red.png" OnClick="ImageButton1_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnOutPut" runat="server" class="outputExcel" Text="导出Excel"
                            OnClick="btnOutPut_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 150px; height: 20px; text-align: center;">网点
                        </th>
                        <th style="width: 100px; text-align: center;">商品种类
                        </th>
                        <th style="width: 100px; text-align: center;">商品单价
                        </th>

                        <th style="width: 100px; text-align: center;">计量单位
                        </th>
                        <th style="width: 100px; text-align: center;">购买数量
                        </th>
                        <th style="width: 100px; text-align: center;">商品总价
                        </th>
                        <th style="width: 100px; text-align: center;">优惠金额
                        </th>
                        <th style="width: 100px; text-align: center;">预付款金额
                        </th>
                        <th style="width: 100px; text-align: center;">实付金额
                        </th>
                        <th style="width: 100px; text-align: center;">返利金额
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;">

                        <%#Eval("WBID")%>
                    </td>
                    <td>
                        <%#Eval("GoodSupplyID")%>
                    </td>
                    <td>
                        <%#Eval("Price")%>
                    </td>

                    <td>
                        <%#Eval("UnitName")%>
                    </td>
                    <td>
                        <%#Eval("GoodSupplyCount")%>
                    </td>
                    <td>
                        <%#Eval("Money_Total")%>
                    </td>
                    <td>
                        <%#Eval("Money_YouHui")%>
                    </td>
                    <td>
                        <%#Eval("Money_PreDefine")%>
                    </td>
                    <td>
                        <%#Eval("Money_Reality")%>
                    </td>
                    <td>
                        <%#Eval("Money_Back")%>
                    </td>


                </tr>
            </ItemTemplate>
            <FooterTemplate>

                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
        <asp:Repeater ID="Repeater2" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr>
                        <td></td>
                        <td colspan="5" style="text-align: center">总计</td>
                    </tr>
                    <tr class="tr_head">
                        <th style="width: 150px; text-align: center;"></th>
                        <th style="width: 100px; text-align: center;">商品种类
                        </th>
                        <th style="width: 100px; text-align: center;">商品单价
                        </th>

                        <th style="width: 100px; text-align: center;">计量单位
                        </th>
                        <th style="width: 100px; text-align: center;">购买数量
                        </th>
                        <th style="width: 100px; text-align: center;">商品总价
                        </th>
                        <th style="width: 100px; text-align: center;">优惠金额
                        </th>
                        <th style="width: 100px; text-align: center;">预付款金额
                        </th>
                        <th style="width: 100px; text-align: center;">实付金额
                        </th>
                        <th style="width: 100px; text-align: center;">返利金额
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;"></td>
                    <td>
                        <%#Eval("GoodSupplyID")%>
                    </td>

                    <td></td>
                    <td></td>
                    <td>
                        <%#Eval("GoodSupplyCount")%>
                    </td>
                    <td>
                        <%#Eval("Money_Total")%>
                        <td>
                            <%#Eval("Money_YouHui")%>
                        </td>
                        <td>
                            <%#Eval("Money_PreDefine")%>
                        </td>
                        <td>
                            <%#Eval("Money_Reality")%>
                        </td>
                        <td>
                            <%#Eval("Money_Back")%>
                        </td>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr>
                    <td colspan="5"></td>
                    <td>金额总计:</td>
                    <td><%=Money_Youhui %></td>
                    <td><%=Money_Yufukuan%></td>
                    <td><%=Money_Shifu%></td>
                    <td><%=Moeny_Fanli%></td>
                </tr>
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>

        <div style="display: none;">
            <%--选择兑换的存储产品信息--%>
            <input type="text" name="txtDep_SID" value="" />
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>


</body>
</html>
