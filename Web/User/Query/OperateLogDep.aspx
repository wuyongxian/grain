<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OperateLogDep.aspx.cs" Inherits="Web.User.Query.OperateLogDep" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" /> 
    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          /*-------打印存折--------*/
          var p_left = 0; var p_ltop = 0; var p_lwidth = 0; var p_lheight = 0;
          $(function () {
             
              $.ajax({
                  url: '/Ashx/wbinfo.ashx?type=GetPrintSetting_Dep',
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


          //加载新的业务编号
          function InitBusinessNO() {
              $.ajax({
                  url: "/User/Storage/storage.ashx?type=GetNewBusinessNO&AccountNumber=" + $('#D_AccountNumber').html(),
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      $('input[name=BusinessNO]').val(r);

                      frmSubmit();
                  }, error: function (r) {
                     showMsg('加载信息失败 ！');
                  }
              });
          }



          function FunPrint(BusinessNO,BusinessName) {
              if (BusinessName == "商品销售" || BusinessName == "退还商品销售" || BusinessName == "积分兑换商品") {
                  showMsg("该操作不需要打印存折！");
                  return;
              }

              var msg = '您确认要补打此存折吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      $.ajax({
                          url: '/Ashx/storage.ashx?type=PrintDep_OperateLog&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html(),
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

          function FunPrintPage(BusinessNO, BusinessName) {
              if (BusinessName == "存入") {
                  PrintPageStorage(BusinessNO,'存粮');
              }

              if (BusinessName == "结息") {
                  PrintPageStorage(BusinessNO, '计息续存');
              }

              if (BusinessName == "兑换") {
                  PrintPageExchange(BusinessNO);
              }
              else if (BusinessName == "存转销") {
                  PrintPageSell(BusinessNO);
              } else if (BusinessName == "退还兑换") {
             
                  PrintPageExchangeReturn(BusinessNO);
              } else if (BusinessName == "退还存转销") {
             
                  PrintPageSellReturn(BusinessNO);
              }
              else if (BusinessName == "产品换购") {

                  PrintPageShopping(BusinessNO);
              }
              else if (BusinessName == "退还产品换购") {

                  PrintPageShoppingReturn(BusinessNO);
              }
              else if (BusinessName == "商品销售") {

                  PrintPageGoodSell(BusinessNO);
              }
              else if (BusinessName == "退还商品销售") {

                  PrintPageGoodSellReturn(BusinessNO);
              }
              else if (BusinessName == "积分兑换商品") {

                  PrintPageExchangeIntegral(BusinessNO);
              }

              else if (BusinessName == "批量兑换") {

                  PrintPageExchangeGroup(BusinessNO);
              }
          }

          //打印存粮凭证
          function PrintPageStorage(BusinessNO,OperateType) {
              var url = '/Ashx/storage.ashx?type=PrintDep_StorageInfo';
              var para = 'AccountNumber=' + $('#D_AccountNumber').html() + '&BusinessNO=' + BusinessNO + '&OperateType=' + OperateType;
              $.ajax({
                  url: url,
                  type: 'post',
                  data: para,
                  dataType: 'text',
                  success: function (r) {
                      if (r == '') {
                          showMsg('打印内容不可以为空!');
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r);
                      CreatePage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                  }
              });

          }

          function PrintPageExchange(BusinessNO) {
              $.ajax({
                  url: '/Ashx/storage.ashx?type=PrintGoodExchange&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html(),
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (r.state == 'false' || r.state == false) {
                          showMsg(r.msg);
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r.msg);
                      CreatePage();
                     // LODOP.PREVIEW(); //打印存折
                      var printime = '';
                      if (LODOP.CVERSION) CLODOP.On_Return = function (TaskID, Value) {
                          printime = Value;
                      };
                      printime = LODOP.PREVIEW();
                      if (parseInt(printime) > 0) {
                          updatePrintTime(printime, BusinessNO);
                      }
                  
                     
                  }, error: function (r) {
                     showMsg('加载打印坐标时出现错误 ！');
                  }
              });

          }

          //更新打印次数
          function updatePrintTime(printime, BusinessNO) {
              $.ajax({
                  url: '/Ashx/storage.ashx?type=updatePrintTime&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html(),
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      if (r == 'Y') {
                          console.log('----更新打印次数成功----');
                      } else {
                          console.log('----更新打印次数失败----');
                      }
                  }, error: function (r) {
                      console.log('----更新打印次数失败----');
                  }
              });
          }

          function PrintPageExchangeReturn(BusinessNO) {
              $.ajax({
                  url: '/Ashx/storage.ashx?type=PrintGoodExchange&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html() + '&model=return',
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (r.state == 'false' || r.state == false) {
                          showMsg(r.msg);
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r.msg);
                      CreatePage();
                      // LODOP.PREVIEW(); //打印存折
                      var printime = '';
                      if (LODOP.CVERSION) CLODOP.On_Return = function (TaskID, Value) {
                          printime = Value;
                      };
                      printime = LODOP.PREVIEW();
                      if (parseInt(printime) > 0) {
                          updatePrintTime(printime, BusinessNO);
                      }
                  }, error: function (r) {
                     showMsg('加载打印坐标时出现错误 ！');
                  }
              });

          }

          function PrintPageSell(BusinessNO) {
              $.ajax({
                  url: '/Ashx/storage.ashx?type=PrintStorageSell&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html(),
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (r.state == 'false' || r.state == false) {
                          showMsg(r.msg);
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r.msg);
                      CreatePage();
                      // LODOP.PREVIEW(); //打印存折
                      var printime = '';
                      if (LODOP.CVERSION) CLODOP.On_Return = function (TaskID, Value) {
                          printime = Value;
                      };
                      printime = LODOP.PREVIEW();
                      if (parseInt(printime) > 0) {
                          updatePrintTime(printime, BusinessNO);
                      }
                  }, error: function (r) {
                     showMsg('加载打印坐标时出现错误 ！');
                  }
              });

          }


          function PrintPageSellReturn(BusinessNO) {
              $.ajax({
                  url: '/Ashx/storage.ashx?type=PrintStorageSell&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html() + '&model=return',
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (r.state == 'false' || r.state == false) {
                          showMsg(r.msg);
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r.msg);
                      CreatePage();
                      // LODOP.PREVIEW(); //打印存折
                      var printime = '';
                      if (LODOP.CVERSION) CLODOP.On_Return = function (TaskID, Value) {
                          printime = Value;
                      };
                      printime = LODOP.PREVIEW();
                      if (parseInt(printime) > 0) {
                          updatePrintTime(printime, BusinessNO);
                      }
                  }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                  }
              });

          }

          function PrintPageShopping(BusinessNO) {
              $.ajax({
                  url: '/Ashx/storage.ashx?type=PrintStorageShopping&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html(),
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (r.state == 'false' || r.state == false) {
                          showMsg(r.msg);
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r.msg);
                      CreatePage();
                      // LODOP.PREVIEW(); //打印存折
                      var printime = '';
                      if (LODOP.CVERSION) CLODOP.On_Return = function (TaskID, Value) {
                          printime = Value;
                      };
                      printime = LODOP.PREVIEW();
                      if (parseInt(printime) > 0) {
                          updatePrintTime(printime, BusinessNO);
                      }
                  }, error: function (r) {
                     showMsg('加载打印坐标时出现错误 ！');
                  }
              });

          }


          function PrintPageShoppingReturn(BusinessNO) {
              $.ajax({
                  url: '/Ashx/storage.ashx?type=PrintStorageShopping&BusinessNO=' + BusinessNO + '&AccountNumber=' + $('#D_AccountNumber').html()+'&model=return',
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (r.state == 'false' || r.state == false) {
                          showMsg(r.msg);
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r.msg);
                      CreatePage();
                      // LODOP.PREVIEW(); //打印存折
                      var printime = '';
                      if (LODOP.CVERSION) CLODOP.On_Return = function (TaskID, Value) {
                          printime = Value;
                      };
                      printime = LODOP.PREVIEW();
                      if (parseInt(printime) > 0) {
                          updatePrintTime(printime, BusinessNO);
                      }
                  }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                  }
              });

          }


          function PrintPageGoodSell(BusinessNO) {
              var url = '/User/Exchange/exchange.ashx?type=PrintGoodSellList';
              var para = 'AccountNumber=' + $('#D_AccountNumber').html() + '&BNOList=' + BusinessNO + '&model=';
              $.ajax({
                  url: url,
                  type: 'post',
                  data: para,
                  dataType: 'text',
                  success: function (r) {
                      if (r == '') {
                          showMsg('打印内容不可以为空!');
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r);
                      CreatePage();
                      LODOP.PREVIEW(); //打印存折

                     
                  }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                  }
              });
             

          }

          function PrintPageGoodSellReturn(BusinessNO) {
              var url = '/User/Exchange/exchange.ashx?type=PrintGoodSellList';
              var para = 'AccountNumber=' + $('#D_AccountNumber').html() + '&BNOList=' + BusinessNO + '&model=return';
              $.ajax({
                  url: url,
                  type: 'post',
                  data: para,
                  dataType: 'text',
                  success: function (r) {
                      if (r == '') {
                          showMsg('打印内容不可以为空!');
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r);
                      CreatePage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                  }
              });
             
          }


          function PrintPageExchangeIntegral(BusinessNO) {
              var url = '/User/Exchange/exchange.ashx?type=PrintGoodExchangeIntegral';
              // var para = 'AccountNumber=' + $('#QAccountNumber').val() + '&BusinessNO=' + BusinessNO + '&model=';
              var para = 'AccountNumber=' + $('#D_AccountNumber').html() + '&BusinessNO=' + BusinessNO
              $.ajax({
                  url: url,
                  type: 'post',
                  data: para,
                  dataType: 'text',
                  success: function (r) {
                      if (r == '') {
                          showMsg('打印内容不可以为空!');
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r);
                      CreatePage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                  }
              });
          }


          function PrintPageExchangeGroup(BusinessNO) {
              var url = '/ashx/storage.ashx?type=PrintGoodExchangeGroup';
              // var para = 'AccountNumber=' + $('#QAccountNumber').val() + '&BusinessNO=' + BusinessNO + '&model=';
              var para = 'AccountNumber=' + $('#D_AccountNumber').html() + '&BusinessNO=' + BusinessNO + '&model=';
              $.ajax({
                  url: url,
                  type: 'post',
                  data: para,
                  dataType: 'json',
                  success: function (r) {
                      if (r == '') {
                          showMsg('打印内容不可以为空!');
                          return false;
                      }
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r.msg);
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
    <div id="divPrint" style="display: none">
    </div>
    <div id="divPrintPaper" style="display: none">
    </div>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>储户业务记录查询</b>
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>储户账号:</span></td>
                    <td>
                        <input type="text" id="QAccountNumber" style="font-size: 16px; width: 120px;height:26px; font-weight: bolder;" runat="server" />
                    </td>
                    <td><span>日期:</span></td>
                    <td>
                        <input type="text" id="Qdtstart" onclick="WdatePicker();" style="font-size: 16px; width: 100px;height:26px; font-weight: bolder;" runat="server" />
                        <span>-</span>
                        <input type="text" id="Qdtend" onclick="WdatePicker()" style="font-size: 16px; width: 100px;height:26px; font-weight: bolder;" runat="server" />
                    </td>

                    <td>
                        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                            runat="server" OnClick="ImageButton1_Click" /></td>

                </tr>

            </table>
        </div>

        <div id="depositorInfo" runat="server" style="display: none;">
            <table class="tabData" style="margin: 10px 0px;">
                <tr>
                    <td colspan="6" style="border-bottom: 1px solid #aaa; height: 25px; text-align: center">
                        <span style="font-size: 16px; font-weight: bolder; color: Green">储户基本信息</span>
                    </td>
                </tr>
                <tr>
                    <th align="center" style="width: 100px; height: 30px;">储户账号
                    </th>
                    <th align="center" style="width: 100px;">储户姓名
                    </th>
                    <th align="center" style="width: 100px;">移动电话
                    </th>
                    <th align="center" style="width: 100px;">当前状态
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
                        <span style="font-weight: bolder; color: Blue;" id="D_numState" runat="server"></span>
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
        <div id="StorageList" style="margin: 10px 0px">
            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <table class="tabData">
                        <tr class="tr_head">
                            <th style="width: 100px; height: 20px; text-align: center;">网点
                            </th>
                            <th style="width: 100px; text-align: center;">业务类型
                            </th>
                            <th style="width: 120px; text-align: center;">交易商品
                            </th>
                            <th style="width: 60px; text-align: center;">单价
                            </th>
                            <th style="width: 60px; text-align: center;">单位
                            </th>
                            <th style="width: 60px; text-align: center;">数量
                            </th>
                            <th style="width: 80px; text-align: center;">折合原粮
                            </th>
                            <th style="width: 100px; text-align: center;">交易金额
                            </th>
                            <th style="width: 80px; text-align: center;">结存数量
                            </th>

                            <th style="width: 80px; text-align: center;">交易时间
                            </th>
                            <th style="width: 80px; text-align: center;">办理人
                            </th>
                            <th style="width: 80px; text-align: center;">打印存折
                            </th>
                            <th style="width: 80px; text-align: center;">打印凭据
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
                            <%#Eval("VarietyName")%>
                        </td>
                        <td>
                            <%#Eval("Price")%>
                        </td>
                        <td>
                            <%#Eval("UnitName")%>
                        </td>
                        <td>
                            <%#(Eval("GoodCount"))%>
                        </td>
                        <td>
                            <%#(Eval("Count_Trade"))%>
                        </td>

                        <td>￥ <%#Eval("Money_Trade")%>
                        </td>

                        <td>
                            <%#Eval("Count_Balance")%>
                        </td>
                        <td>
                            <%#Eval("dt_Trade")%>
                        </td>
                        <td>
                            <%#Eval("UserID")%>
                        </td>

                        <td><a href="#" onclick="FunPrint('<%#Eval("BusinessNO") %>','<%#Eval("BusinessName") %>')">补打存折</a></td>
                        <td><a href="#" onclick="FunPrintPage('<%#Eval("BusinessNO") %>','<%#Eval("BusinessName") %>')">补打凭据</a></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>

                    <!--底部模板-->
                    </table>
                <!--表格结束部分-->
                </FooterTemplate>
            </asp:Repeater>

        </div>

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
