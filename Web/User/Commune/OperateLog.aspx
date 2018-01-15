<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OperateLog.aspx.cs" Inherits="Web.User.Commune.OperateLog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          /*-------打印存折--------*/
          var p_left = 0; var p_ltop = 0; var p_lwidth = 0; var p_lheight = 0;
          $(function () {
              $.ajax({
                  url: '/Ashx/wbinfo.ashx?type=GetPrintSetting',
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      p_lwidth = r[0].Width;
                      p_lheight = r[0].Height;
                      p_lleft = r[0].DriftRateX;
                      p_ltop = r[0].DriftRateY;
                  }, error: function (r) {
                     showMsg('加载打印坐标时出现错误 ！');
                  }
              });
          });
          function CreateOneFormPage() {
              LODOP = getLodop();
              LODOP.PRINT_INIT("存折打印");
              LODOP.SET_PRINT_STYLE("FontSize", 18);
              LODOP.SET_PRINT_STYLE("Bold", 1);
              LODOP.ADD_PRINT_TEXT(0, 0, 0, 0, "打印页面部分内容");
              LODOP.ADD_PRINT_HTM(p_ltop, p_lleft, p_lwidth, p_lheight, document.getElementById("divPrint").innerHTML);

          };

          function FunPrint(BusinessNO) {
              var msg = '您确认要补打此存折吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      $.ajax({
                          url: '/Ashx/commune.ashx?type=GetC_SupplyPrint&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html(),
                          type: 'post',
                          data: '',
                          dataType: 'text',
                          success: function (r) {
                              $('#divPrint').html('');
                              $('#divPrint').append(r);
                              CreateOneFormPage();
                              LODOP.PREVIEW(); //打印存折
                          }, error: function (r) {
                              showMsg('加载打印坐标时出现错误 ！');
                          }
                      });
                  } else {
                      //console.log('你点击了取消！');
                  }

              });

          }
          /*-------End 打印存折--------*/
      </script>
</head>
<body>
    <div id="divPrint" style="display: none">
    </div>

    <form id="form1" runat="server">
        <div class="pageHead">
            <b>社员供销历史查询</b>
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>社员账号:</span></td>
                    <td>
                        <input type="text" id="QAccountNumber" style="font-size: 16px; width: 120px; font-weight: bolder;" runat="server" /></td>
                    <td><span>日期:</span></td>
                    <td>
                        <input type="text" id="Qdtstart" readonly="readonly" onclick="WdatePicker();" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                        <span>-</span>
                        <input type="text" id="Qdtend" readonly="readonly" onclick="WdatePicker()" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" /></td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="~/images/search_red.png" OnClick="ImageButton1_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <div id="depositorInfo" runat="server" style="display: none;">
            <table class="tabData" style="margin: 20px 0px;">
                <tr>
                    <td colspan="6" style="border-bottom: 1px solid #aaa; height: 25px; text-align: center">
                        <span style="font-size: 16px; font-weight: bolder; color: Green">社员基本信息</span>
                    </td>
                </tr>
                <tr>
                    <th align="center" style="width: 100px; height: 30px;">社员账号
                    </th>
                    <th align="center" style="width: 100px;">社员姓名
                    </th>
                    <th align="center" style="width: 100px;">移动电话
                    </th>
                    <th align="center" style="width: 100px;">田亩册
                    </th>
                    <th align="center" style="width: 150px;">身份证号
                    </th>
                    <th align="center" style="width: 200px;">储户地址
                    </th>

                </tr>
                <tr>

                    <td style="height: 30px;">
                        <span style="font-weight: bolder; color: Blue;" id="D_AccountNumber" runat="server"></span>
                    </td>

                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_strName" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_PhoneNo" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_FieldCopies" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_IDCard" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_strAddress" runat="server"></span>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 100px; height: 20px; text-align: center;">网点
                        </th>
                        <th style="width: 80px; text-align: center;">业务类型
                        </th>
                        <th style="width: 80px; text-align: center;">交易商品
                        </th>
                        <th style="width: 80px; text-align: center;">单价
                        </th>
                        <th style="width: 60px; text-align: center;">单位
                        </th>
                        <th style="width: 80px; text-align: center;">交易数量
                        </th>
                        <th style="width: 100px; text-align: center;">交易金额
                        </th>
                        <th style="width: 80px; text-align: center;">折扣
                        </th>
                        <th style="width: 100px; text-align: center;">实付金额
                        </th>
                        <th style="width: 80px; text-align: center;">交易时间
                        </th>
                        <th style="width: 80px; text-align: center;">办理人
                        </th>
                        <th style="width: 80px; text-align: center;">打印
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;">

                        <%#Eval("WBID")%>
                    </td>
                    <td>
                        <%#Eval("BusinessName")%>

                    </td>
                    <td>
                        <%#Eval("VarietyID")%>
                    </td>
                    <td>
                        <%#Eval("Price")%>
                    </td>
                    <td>
                        <%#Eval("UnitID")%>
                    </td>
                    <td>
                        <%#(Eval("CountTrade"))%>
                    </td>
                    <td>￥ <%#Eval("Money_Trade")%>
                    </td>
                    <td>
                        <%#Eval("numDisCount")%>%
                    </td>
                    <td>￥<%#Eval("Money_Reality")%>
                    </td>
                    <td>
                        <%#Eval("dt_Trade")%>
                    </td>
                    <td>
                        <%#Eval("UserID")%>
                    </td>

                    <td><a href="#" onclick="FunPrint('<%#Eval("BusinessNO") %>')">补打存折</a></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>

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
