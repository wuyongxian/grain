<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settle_Exchange.aspx.cs" Inherits="Web.Settle.Settle_Exchange" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <link href="../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>
     <script src="../../Scripts/excelhelper.js" type="text/javascript"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {

              InitWBID();
              $('#QWBID').change(function () {
                  if ($('#QWBID option:selected').val() != "") {
                      $('#WBID').val($('#QWBID option:selected').val());
                      $('#txtWBID').val($('#QWBID option:selected').text());
                  }
              });
              GetAccountant();//获取总部会计信息
              $('#excel_export').ExportExcel('dataInfo', '网点兑换结算报表');
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


          function GetAccountant() {
              $.ajax({
                  url: '/Ashx/settlebasic.ashx?type=GetAccountant',
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      //加载网点营业员
                      if (r.length < 1) {
                         showMsg('获取总部会计信息失败 ！');
                          return false;
                      }
                      $('select[name=Accountant]').empty();
                      for (var i = 0; i < r.length; i++) {
                          $('select[name=Accountant]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                      }

                  }, error: function (r) {
                      if (r.reponseText == "Error") {
                         showMsg('获取总部会计信息失败 ！');
                      }
                  }
              });
          }



          //获取单笔兑换的信息
          function GetSettleSingle(obj) {
              //查询当前记录是否已经结算
            

              $.ajax({
                  url: '/Ashx/settle.ashx?type=ISHaveExchange&ID=' + obj,
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      if (r != '0') {
                         showMsg('该笔记录已经结算 ！');
                          return false;
                      }
                      else {
                          ShowSettleSingle(obj);
                      }
                  }, error: function (r) {

                  }
              });


          }

          //获取单笔的计算记录
          function ShowSettleSingle(obj) {
              $('input[name=ID]').val(obj);
              $('input[name=ISPay]').val('0');
              $('#btnPay').removeAttr('disabled');
              $('#btnPrint').attr('disabled', 'disabled');
              $.ajax({
                  url: '/Ashx/settle.ashx?type=GetExchangeSingle&ID=' + obj,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      showBodyCenter($('#divfrm'));
                   
                      $('#S_strName').html(r[0].DepName);
                      $('#S_StorageDate').html(r[0].dt_Exchange);
                      $('#S_GoodName').html(r[0].GoodName);
                      $('#S_UnitName').html(r[0].UnitName);
                      $('#S_GoodCount').html(r[0].GoodCount);
                      $('#S_GoodPrice').html(r[0].GoodPrice);
                      $('#S_VarietyCount').html(r[0].VarietyCount);
                      var ISReturn = r[0].ISReturn;
                    
                      if (parseInt(ISReturn)== 0) {
                          $('#S_Money_DuiHuan').html(r[0].Money_DuiHuan);
                          $('#S_Money_DuiHuan2').html(r[0].Money_DuiHuan);
                      }
                      else {
                          $('#S_Money_DuiHuan').html('-' + r[0].Money_DuiHuan);
                          $('#S_Money_DuiHuan2').html('-' + r[0].Money_DuiHuan);
                      }
                      $('#S_WBName').html(r[0].WBName);


                  }, error: function (r) {
                      if (r.reponseText == "Error") {
                         showMsg('获取兑换信息失败 ！');
                      }
                  }
              });
          }



          //支付现金
          function FunPay() {
              var msg = '您确认要结算这笔业务吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      $('input[name=ISPay]').val('1');
                      var ID = $('input[name=ID]').val();

                      var Accountant = $('select[name=Accountant] option:selected').text();

                      $.ajax({
                          url: '/Ashx/settle.ashx?type=AddSA_Exchange&ID=' + ID + '&Accountant=' + Accountant,
                          type: 'post',
                          data: $('#form1').serialize(),
                          dataType: 'text',
                          success: function (r) {
                              showMsg('结算成功 ！');
                              $('#btnPay').attr('disabled', 'disabled');
                              $('#btnPrint').removeAttr('disabled');
                          }, error: function (r) {
                              if (r.reponseText == "Error") {
                                  showMsg('结算失败 ！');
                              }
                          }
                      });
                  } else {
                      //console.log('你点击了取消！');
                  }

              });
          }

          //全部结算
          function FunPayAll() {
              var msg = '您确认要将当前页面中的兑换全部结算吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      //var WBName = $('#txtWBID').val();
                      var WBID = $('#QWBID').val();
                      var dtStart = $('#Qdtstart').val();
                      var dtEnd = $('#Qdtend').val();

                      $.ajax({
                          url: '/Ashx/settle.ashx?type=AddSA_ExchangeAll&WBID=' + WBID + '&dtStart=' + dtStart + '&dtEnd=' + dtEnd,
                          type: 'post',
                          data: $('#form1').serialize(),
                          dataType: 'text',
                          success: function (r) {
                              if (r == "Error") {
                                  showMsg('结算失败 ！');
                              }
                              else {
                                  showMsg('全部结算成功 ！');

                              }
                          }, error: function (r) {
                              showMsg('结算失败 ！');
                          }
                      });
                  } else {
                      //console.log('你点击了取消！');
                  }

              });
          }

          //关闭窗口
          function CloseFrm() {
             
              $('#divfrm').fadeOut('normal');
          }



          //打印付款单据

          function PrintPageSettle() {
              var ID = $('input[name=ID]').val();
              PrintPage(ID);
          }

          function PrintPage(obj) {

              $.ajax({
                  url: '/Ashx/settle.ashx?type=PrintExchange&ID=' + obj,
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                   
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r);
                      CreatePage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                     showMsg('加载打印坐标时出现错误 ！');
                  }
              });

          }

          //小票打印
          function CreatePage() {
              LODOP = getLodop();
              LODOP.PRINT_INIT("小票打印");
              LODOP.SET_PRINT_STYLE("FontSize", 12);
              LODOP.SET_PRINT_STYLE("Bold", 1);
              LODOP.ADD_PRINT_TEXT(0, 0, 0, 0, "打印页面部分内容");

              LODOP.ADD_PRINT_HTM(20, 60, 800, 400, document.getElementById("divPrintPaper").innerHTML);

          };

      </script>
</head>
<body>
    <div id="divPrintPaper" style="width: 640px; font-size: 12px; display: none;">
    </div>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>网点兑换报表</b>
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
                        <input type="button" value="全部结算" onclick="FunPayAll();" style="width: 100px;height:25px; font-weight: bolder; font-size: 16px; color: Blue;" />
                    </td>
                    <td>
                         <a id="excel_export" href="#">Excel</a>
                    </td>
                </tr>
            </table>
        </div>

        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" id="dataInfo">
                    <tr class="tr_head">
                        <th style="width: 100px; height: 20px; text-align: center;">网点
                        </th>
                        <th style="width: 100px; text-align: center;">账号
                        </th>
                        <th style="width: 100px; text-align: center;">储户姓名
                        </th>
                        <th style="width: 100px; text-align: center;">日期
                        </th>
                        <th style="width: 100px; text-align: center;">业务类型
                        </th>

                        <th style="width: 100px; text-align: center;">商品名称
                        </th>
                        <th style="width: 80px; text-align: center;">单位
                        </th>
                        <th style="width: 80px; text-align: center;">重量
                        </th>
                        <th style="width: 80px; text-align: center;">单价
                        </th>
                        <th style="width: 80px; text-align: center;">折合原粮
                        </th>
                        <th style="width: 80px; text-align: center;">利息
                        </th>
                        <th style="width: 100px; text-align: center;">折合现金
                        </th>
                        <th style="width: 180px; text-align: center;">操作
                        </th>


                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;">

                        <%#Eval("WBName")%>
                    </td>
                    <td>
                        <%#Eval("AccountNumber")%>
                    </td>
                    <td>
                        <%#Eval("DepName")%>
                    </td>
                    <td>
                        <%#Eval("dt_Exchange")%>
                    </td>
                    <td>
                        <%#Eval("BusinessName")%>
                    </td>
                    <td>
                        <%#Eval("GoodName")%>
                    </td>
                    <td>
                        <%#Eval("UnitName")%>
                    </td>
                    <td>
                        <%#Eval("GoodCount")%>
                    </td>
                    <td>
                        <%#Eval("GoodPrice")%>
                    </td>
                    <td>
                        <%#Eval("VarietyCount")%>
                    </td>
                    <td>
                        <%#Eval("VarietyInterest")%>
                    </td>
                    <td>
                        <%#GetMoneyExchange(Eval("ISReturn"),Eval("Money_DuiHuan"))%>
                    </td>
                    <td>
                        <%-- <a style="color:Red; font-weight:bold;" href="Settle_AgentFeeDetail.aspx?ID=1">asdf</a>--%>
                        <%#GetOperateInfo(Eval("ID"))%>
                    </td>

                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr>
                    <td style="height: 25px;">


                        <td align="center" colspan="8">
                            <span style="color: Red; font-weight: bold;">兑换商品合计:</span>
                            <span style="padding-left: 10px;">已结算:</span>
                            <span style="color: green;">￥<%=JieSuan%></span>
                        </td>
                        <td>
                            <%=VarietyCount%>
                        </td>
                        <td>
                            <%=VarietyInterest%>
                        </td>
                        <td>
                            <%=Money_DuiHuan%>
                        </td>

                        <td></td>
                </tr>
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>

        <div id="divfrm" class="pageEidt" style="display: none; ">
             <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="CloseFrm()" />
            <div style="clear: both;">
                <table class="tabEdit" style="margin: 10px 0px 10px 20px">

                      <tr style="height:25px; background:#e0eeee">
                        <td align="center" style="width: 80px;">储户姓名</td>
                        <td align="center" style="width: 80px;">存储日期</td>
                        <td align="center" style="width: 80px;">商品名称</td>
                        <td align="center" style="width: 80px;">单位</td>
                        <td align="center" style="width: 80px;">重量</td>
                        <td align="center" style="width: 80px;">单价</td>
                        <td align="center" style="width: 80px;">折合原粮</td>
                        <td align="center" style="width: 80px;">折合现金</td>
                    </tr>
                    <tr>
                        <td align="center">
                            <span id="S_strName"></span>
                        </td>
                        <td align="center">
                            <span id="S_StorageDate"></span>
                        </td>
                        <td align="center">
                            <span id="S_GoodName"></span>
                        </td>
                        <td align="center">
                            <span id="S_UnitName"></span>
                        </td>
                        <td align="center">
                            <span id="S_GoodCount"></span>
                        </td>
                        <td align="center">
                            <span id="S_GoodPrice"></span>
                        </td>
                        <td align="center">
                            <span id="S_VarietyCount"></span>
                        </td>
                        <td align="center">
                            <span id="S_Money_DuiHuan">元</span>
                        </td>
                    </tr>
                </table>
                <table style="margin: 10px 0px 0px 100px">
                    <tr>
                        <td></td>
                        <td><span style="color: Blue; font-size: 16px; font-weight: bolder;">与网点结算单笔业务费用</span></td>

                    </tr>
                    <tr>
                        <td align="right" style="width: 120px;"><span>网点:</span></td>
                        <td><span id="S_WBName"></span></td>
                    </tr>
                    <tr>
                        <td align="right"><span>总部会计:</span></td>
                        <td>
                            <select name="Accountant" style="width: 100px;"></select></td>
                    </tr>
                    <tr>
                        <td align="right"><span>应付金额:</span></td>
                        <td><span id="S_Money_DuiHuan2" style="font-weight: bolder;"></span>元</td>
                    </tr>


                    <tr>
                        <td></td>
                        <td>
                            <input type="button" id="btnPay" value="支付现金" onclick="FunPay()" />&nbsp;
            <input type="button" id="btnPrint" value="打印凭证" disabled="disabled" onclick="PrintPageSettle()" />

                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div style="display: none;">
            <%--被选择的存入信息条目--%>
            <input type="text" name="ID" value="" />
            <%--是否已经发生了结算业务  1:发生了 0：未发生--%>
            <input type="text" name="ISPay" value="" />
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" runat="server" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>
</body>
</html>