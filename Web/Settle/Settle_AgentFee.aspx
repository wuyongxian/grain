<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settle_AgentFee.aspx.cs" Inherits="Web.Settle.Settle_AgentFee" %>

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
              $('#excel_export').ExportExcel('dataInfo', '网点代理费结算报表');
              InitWBID();
              $('#QWBID').change(function () {
                  if ($('#QWBID option:selected').val() != "") {
                      $('#txtWBID').val($('#QWBID option:selected').text());
                    
                      var WBName = $('#QWBID option:selected').text();
                      var para = 'WBName=' + WBName;
                      $.ajax({
                          url: '/Ashx/settle.ashx?type=GetAgentFeeDate',
                          type: 'post',
                          data: para,
                          dataType: 'json',
                          success: function (r) {
                              if (r.state == 'error') {
                                  return false;
                              }
                              $('#Qdtstart').val(getDate( r.date_begin));
                              $('#Qdtend').val(getDate(r.date_end));
                              var strhtml = '当前选择网点：' + WBName;
                              strhtml += ' 最低存储期限：' + r.numDay + '天，'
                              var zhiqu = '';
                              if (r.draw_exchange == '1') { zhiqu += '、兑换'; }
                              if (r.draw_sell == '1') { zhiqu += '、存转销'; }
                              if (r.draw_shopping == '1') { zhiqu += '、产品换购'; }
                              if (zhiqu == '') {
                                  strhtml += '提前支取选项：无。'
                              } else {
                                  zhiqu = zhiqu.substring(1);
                                  strhtml += '提前支取选项：' + zhiqu + '。'
                              }
                              $('#div-warning').html(strhtml);
                          }, error: function (r) {

                          }
                      });
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


          //获取单笔代理费的信息
          function GetAgentFeeSingle(obj) {
              //查询当前记录是否已经结算


              $.ajax({
                  url: '/Ashx/settle.ashx?type=ISHaveAgentFee&Dep_SID=' + obj,
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      if (r != '0') {
                         showMsg('该笔记录已经结算 ！');
                          return false;
                      }
                      else {
                          ShowAgentFeeSingle(obj);
                      }
                  }, error: function (r) {

                  }
              });

            
          }

          function ShowAgentFeeSingle(obj) {
              $('input[name=Dep_SID]').val(obj);
              $('input[name=ISPay]').val('0');
              $('#btnPay').removeAttr('disabled');
              $('#btnPrint').attr('disabled', 'disabled');
              $.ajax({
                  url: '/Ashx/settle.ashx?type=GetAgentFeeSingle&ID=' + obj,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      showBodyCenter($('#divfrm'));
                 
                      $('#S_AccountNumber').html(r[0].AccountNumber);
                      $('#S_strName').html(r[0].strName);
                      $('#S_StorageDate').html(r[0].StorageDate);
                      $('#S_StorageDay').html(r[0].StorageDay);
                      $('#S_ShiCun').html(r[0].ShiCun);
                      $('#S_numAgent').html(r[0].numAgent);
                      $('#S_MoenyFee').html(r[0].MoenyFee);
                      $('#S_MoenyFee2').html(r[0].MoenyFee);
                      $('#S_WBName').html(r[0].WBName);
                      //加载网点营业员
                      GetWBUser(r[0].WBName);


                  }, error: function (r) {
                      if (r.reponseText == "Error") {
                         showMsg('获取代理费信息失败 ！');
                      }
                  }
              });
          }


          function GetWBUser(WBName) {
              $.ajax({
                  url: '/Ashx/settle.ashx?type=GetUserByWBName&WBName=' + WBName,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      //加载网点营业员
                      if (r.length < 1) {
                         showMsg('获取代网点营业员信息失败 ！');
                          return false;
                      }
                      $('select[name=WBUser]').empty();
                      for (var i = 0; i < r.length; i++) {
                          $('select[name=WBUser]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                      }

                  }, error: function (r) {
                      if (r.reponseText == "Error") {
                         showMsg('获取代网点营业员信息失败 ！');
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
                      var Dep_SID = $('input[name=Dep_SID]').val();
                      var WBUser = $('select[name=WBUser] option:selected').text();

                      $.ajax({
                          url: '/Ashx/settle.ashx?type=AddSA_AgentFee&Dep_SID=' + Dep_SID + '&WBUser=' + WBUser,
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
              var msg = '您确认要将当前页面中的代理费全部结算吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      var WBID = $('#QWBID').val();
                      var dtStart = $('#Qdtstart').val();
                      var dtEnd = $('#Qdtend').val();

                      $.ajax({
                          url: '/Ashx/settle.ashx?type=AddSA_AgentFeeAll&WBID=' + WBID + '&dtStart=' + dtStart + '&dtEnd=' + dtEnd,
                          type: 'post',
                          data: '',
                          dataType: 'json',
                          success: function (r) {
                              showMsg(r.msg);
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
//              if ($('input[name=ISPay]').val() == '1') {//已经发生了结算业务
//                  var WBName = $('#txtWBID').val();
//                  var dtStart = $('#Qdtstart').val();
//                  var dtEnd = $('#Qdtend').val();
//                  window.location.href = '~/Settle/Settle_AgentFell.aspx?WBName=' + WBName + '&dtStart=' + dtStart + '&dtEnd=' + dtEnd; //刷新页面
//              } else {
//              $('#divfrm').fadeOut('normal');
              //              }
              $('#divfrm').fadeOut('normal');
          }



          //打印付款单据

          function PrintPageSettle() {
              var Dep_SID = $('input[name=Dep_SID]').val();
              PrintPage(Dep_SID);
          }

          function PrintPage(obj) {

              $.ajax({
                  url: '/Ashx/settle.ashx?type=PrintAgentFee&Dep_SID=' + obj,
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
    <div id="divPrintPaper" style="width: 640px; font-size: 12px; display: none;"></div>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>网点存粮代理费报表</b>
            <span id="spanHelp" style="cursor: pointer">帮助</span>
        </div>
        <div id="divHelp" class="pageHelp">
            <span>提示1：各种查询可以独使用，也可以联合使用，但必须保证至少有一项查询条件。</span><br />
            <span>提示2：每项查询均为模糊查询条件，为保证查找信息的正确性，请输入完整的查询信息。</span><br />

        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>网点名称:</span></td>
                    <td>
                        <input type="text" id="txtWBID" onkeyup="InitWBID();" style="font-size: 16px; width: 100px;height:26px; font-weight: bolder;" runat="server" />
                        <select id="QWBID" runat="server" style="width: 100px;height:30px;"></select>
                    </td>
                    <td><span>日期:</span></td>
                    <td>
                        <input type="text" id="Qdtstart" readonly="readonly" onclick="WdatePicker();" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                        <span>-</span>
                        <input type="text" id="Qdtend" readonly="readonly" onclick="WdatePicker()" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                    </td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton" runat="server"
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

        <div id="div-warning" style="margin: 10px 0px; margin: 5px 10px; background: #efefef; color: green;"></div>
        <div id="StorageList" style="margin: 10px 0px">
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
                            <th style="width: 100px; text-align: center;">业务类型
                            </th>
                            <th style="width: 100px; text-align: center;">产品类型
                            </th>
                            <th style="width: 100px; text-align: center;">存入日期
                            </th>
                            <th style="width: 60px; text-align: center;">实存天数
                            </th>
                            <th style="width: 100px; text-align: center;">存入重量
                            </th>
                            <th style="width: 100px; text-align: center;">提前支取
                            </th>
                            <th style="width: 100px; text-align: center;">有效重量
                            </th>
                            <th style="width: 80px; text-align: center;">费率
                            </th>
                            <th style="width: 100px; text-align: center;">代理费
                            </th>
                            <th style="width: 160px; text-align: center;">操作
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
                            <%#Eval("strName")%>
                        </td>
                        <td>
                            <%#Eval("BusinessName")%>
                        </td>
                        <td>
                            <%#Eval("VarietyName")%>
                        </td>
                        <td>
                            <%#Eval("StorageDate")%>
                        </td>
                        <td>
                            <%#Eval("StorageDay")%>
                        </td>
                        <td>
                            <%#Eval("CunRu")%>
                        </td>
                        <td>
                            <%#Eval("ZhiQu")%>
                        </td>
                        <td>
                            <%#Eval("ShiCun")%>
                        </td>
                        <td>
                            <%#Eval("numAgent")%>
                        </td>
                        <td>
                            <%#Eval("MoenyFee")%>
                        </td>
                        <td>
                            <%-- <a style="color:Red; font-weight:bold;" href="Settle_AgentFeeDetail.aspx?ID=1">asdf</a>--%>
                            <%#GetOperateInfo(Eval("ID"),Eval("AgentFeeID"))%>
                        </td>

                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr>
                        <td style="height: 25px;"></td>
                        <td align="right" colspan="6">

                            <span>代理费合计:</span>
                            <span style="padding-left: 10px;">已结算:</span>
                            <span style="color: green"><%=JieSuan%></span>
                        </td>
                        <td>
                            <%=numCunRu%>
                        </td>
                        <td>
                            <%=numZhiQu%>
                        </td>
                        <td>
                            <%=numShiCun%>
                        </td>
                        <td></td>
                        <td>
                            <%=numMoenyFee%>
                        </td>
                        <td></td>
                    </tr>
                    <!--底部模板-->
                    </table>
                <!--表格结束部分-->
                </FooterTemplate>
            </asp:Repeater>

        </div>

        <div id="divfrm" class="pageEidt" style="display: none;">
            <div style="float: right;">
                <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="CloseFrm()" /></div>
            <br />
            <div style="clear: both">
                <table class="tabEdit" style="margin: 10px;">

                    <tr style="height:25px; background:#e0eeee">
                        <td align="center" style="width: 80px;">账号</td>
                        <td align="center" style="width: 80px;">储户姓名</td>
                        <td align="center" style="width: 80px;">存储日期</td>
                        <td align="center" style="width: 80px;">实存天数</td>
                        <td align="center" style="width: 80px;">有效重量</td>
                        <td align="center" style="width: 80px;">费率</td>
                        <td align="center" style="width: 80px;">代理费</td>
                    </tr>
                    <tr>
                        <td align="center"><span id="S_AccountNumber"></span></td>
                        <td align="center"><span id="S_strName"></span></td>
                        <td align="center"><span id="S_StorageDate"></span></td>
                        <td align="center"><span id="S_StorageDay"></span>天</td>
                        <td align="center"><span id="S_ShiCun"></span></td>
                        <td align="center"><span id="S_numAgent"></span></td>
                        <td align="center"><span id="S_MoenyFee">元</span></td>
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
                        <td align="right"><span>网点营业员:</span></td>
                        <td>
                            <select name="WBUser" style="width: 100px;"></select></td>
                    </tr>
                    <tr>
                        <td align="right"><span>应付金额:</span></td>
                        <td><span id="S_MoenyFee2" style="font-weight: bolder;"></span>元</td>
                    </tr>


                    <tr>
                        <td></td>
                        <td>
                            <input type="button" id="btnPay" value="支付现金" onclick="FunPay()" />
                            <input type="button" id="btnPrint" value="打印凭证" disabled="disabled" onclick="PrintPageSettle()" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div style="display: none;">
            <%--被选择的存入信息条目--%>
            <input type="text" name="Dep_SID" value="" />
            <%--是否已经发生了结算业务  1:发生了 0：未发生--%>
            <input type="text" name="ISPay" value="" />
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>


</body>
</html>

