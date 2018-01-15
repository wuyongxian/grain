<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="REGoods.aspx.cs" Inherits="Web.Admin.Returned.REGoods" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   

      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {

              var now = new Date(); //获取系统日期，即Sat Jul 29 08:24:48 UTC+0800 2006 
              var yy = now.getFullYear(); //截取年，即2006 
              var mo = now.getMonth() + 1; //截取月，即07 
              var dd = now.getDate(); //截取日，即29 
              //取时间 
              var hh = now.getHours(); //截取小时，即8 
              var mm = now.getMinutes(); //截取分钟，即34 
              var ss = now.getSeconds(); //获取秒 
              $('#dt_GuaShi').val(yy + '-' + mo + '-' + dd + ' ' + hh + ':' + mm + ':' + ss);

          });


          /*--ENd loadFuntion--*/


          function PrintCunDan() {

          }

          function PrintCunZhe() {

          }

          function PrintXiaoPiao() {

          }

          function ShowDepositorInfo() {
              $('#depositorInfo').fadeIn('normal');
          }

          //选择兑换商品
          function FunSelect(obj, exid) {

              var IDList = $('input[name=IDList]').val();
              var arrayObj = new Array();
              arrayObj = IDList.split(',');

              IDList = '';
              if (obj.checked) {//当前节点被选中
                  arrayObj.push(exid); //添加被选中节点
                  for (var i = 0; i < arrayObj.length; i++) {
                      IDList += arrayObj[i] + ',';
                  }
              } else { //当前节点没被选中
                  for (var i = 0; i < arrayObj.length; i++) {
                      if (arrayObj[i] != exid) {
                          IDList += arrayObj[i];
                      }
                  }
              }
              if (IDList.indexOf(',') == IDList.length - 1) {

                  IDList = IDList.substr(IDList.length - 1);
              }

              $('input[name=IDList]').val(IDList);
          }


          function FunSave() {
              var IDList = $('input[name=IDList]').val();

              if ($.trim(IDList) == '') {
                 showMsg(' 请选择要退还的商品 ！');
                  return;
              } else {

                  var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
                  showConfirm(msg, function (obj) {
                      if (obj == 'yes') {
                          
                          $.ajax({
                              url: '/Ashx/exchangeprop.ashx?type=Return_GoodExchange&IDList=' + IDList+ '&AccountNumber=' + $('#D_AccountNumber').html(),
                              type: 'post',
                              data: '',
                              dataType: 'text',
                              success: function (r) {
                                  showMsg(' 退还数据保存成功 ！');
                                  $('#btnSave').attr('disabled', 'disabled');
                                  $('#btnSave').css('background', '#aaa');
                              }, error: function (r) {
                                  showMsg(' 退还操作失败 ！');
                              }
                          });
                      } else {
                          //console.log('你点击了取消！');
                      }
                
                  });
              }
          }
      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead" >
        <b>退还储户兑换商品</b><span id="spanHelp" style="cursor: pointer">帮助</span>
    </div>
    <div id="divHelp"  class="pageHelp">
<span>提示1：首先找到储户信息与兑换信息，选择对应的商品进行退还。当存在多条可退换商品时，储户可以一次退换多种商品。</span><br />
<span>提示2：营业员可以退还当日兑换的商品，管理员可以按原数退还储户所有兑换的商品</span><br />
</div>
<div id="storageQuery">

</div>
    <div style="margin: 20px 0px;">
        <div id="Query">
            <span>储户账号:</span>
            <input type="text" id="QAccountNumber" style="font-size:16px; font-weight:bolder;" runat="server" />
          
            <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png" 
                runat="server" onclick="ImageButton1_Click" />
        </div>
        <div id="depositorInfo" runat="server" style="display:none;">
            <table class="tabData"  style="margin:20px 0px;">
                <tr >
                    <td colspan="6" style="border-bottom:1px solid #aaa; height:25px; text-align:center">
                        <span style="font-size: 16px; font-weight: bolder; color:Green">储户基本信息</span>
                    </td>
                </tr>
                <tr>
                    <th align="center" style="width:100px; height:30px;">
                        储户账号
                    </th>
                    <th align="center" style="width:100px;">
                        储户姓名
                    </th>
                     <th align="center" style="width:100px;">
                        移动电话
                    </th>
                      <th align="center" style="width:100px;">
                        当前状态
                    </th>
                      <th align="center" style="width:150px;">
                        身份证号
                    </th>
                     <th align="center" style="width:200px;">
                        储户地址
                    </th>
                   
                </tr>
                   <tr>
                  
                    <td style="height:30px;">
                        <span style="font-weight:bolder; color:Blue;" id="D_AccountNumber"   runat="server"></span>
                    </td>
                    
                    <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_strName" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_PhoneNo" runat="server"></span>
                    </td>
                     <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_numState" runat="server"></span>
                    </td>
                      <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_IDCard" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_strAddress" runat="server"></span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="StorageList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                    <tr class="tr_head">
                        <th style="width: 100px; height:20px; text-align: center;">
                            存贷产品
                        </th>
                        <th style="width: 80px; text-align: center;">
                            结存数量
                        </th>
                        <th style="width: 100px; text-align: center;">
                            存入时间
                        </th>
                        <th style="width: 80px; text-align: center;">
                            存入价
                        </th>
                        <th style="width: 80px; text-align: center;">
                            存期
                        </th>
                        <th style="width: 80px; text-align: center;">
                            天数
                        </th>
                        <th style="width: 80px; text-align: center;">
                            活期利率
                        </th>
                        <th style="width: 100px; text-align: center;">
                            利息
                        </th>
                      
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height:25px;">
                        <%#Eval("VarietyID")%>
                    </td>
                    <td>
                        <%#Eval("StorageNumber")%>
                    </td>
                    <td>
                        <%#Eval("StorageDate")%>
                    </td>
                    <td>
                        <%#Eval("Price_ShiChang")%>
                    </td>
                    <td>
                        <%#Eval("TimeID")%>
                    </td>
                    <td>
                        <%#Web.common.GetDay(Eval("StorageDate"))%>
                    </td>
                    <td>
                        <%#Eval("CurrentRate")%>
                    </td>
                    <td>
                        <%#Web.common.GetLiXi(Eval("ID"))%>
                    </td>
                    
                </tr>
            </ItemTemplate>
            <FooterTemplate>
            <tr>
            <td colspan="2">
            <span style="font-weight:bolder">折合现金合计:</span>
            </td>
            <td colspan="6" style="text-align:center">
            <span id="spanTotal" runat="server" style="color:Red; font-size:16px">￥<%=Web.common.numTotol%></span>
          
            </td>
            </tr>
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
        </div>

        <div id="exchangeList" style="margin:20px 0px;">
        <span style="font-size:16px; font-weight:bolder;">储户兑换信息</span>
        
            <asp:Repeater ID="Repeater2" runat="server">
                <HeaderTemplate>
                    <table class="tabData">
                        <tr class="tr_head">
                            <th style="width: 100px; height: 20px; text-align: center;">
                                业务名称
                            </th>
                            <th style="width: 80px; text-align: center;">
                                品名
                            </th>
                            <th style="width: 100px; text-align: center;">
                                单价
                            </th>
                            <th style="width: 80px; text-align: center;">
                                数量
                            </th>
                            <th style="width: 80px; text-align: center;">
                                计价单位
                            </th>
                            <th style="width: 80px; text-align: center;">
                                折合原粮
                            </th>
                            <th style="width: 80px; text-align: center;">
                                收费
                            </th>
                            <th style="width: 100px; text-align: center;">
                                退还
                            </th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr 
                        onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                        <td style="height: 25px;">
                            <%#Eval("BusinessName")%>
                        </td>
                        <td>
                            <%#Eval("GoodName")%>
                        </td>
                        <td>
                            <%#Eval("GoodPrice")%>
                        </td>
                        <td>
                            <%#Eval("GoodCount")%>
                        </td>
                        <td>
                            <%#Eval("UnitName")%>
                        </td>
                        <td>
                            <%#Eval("VarietyCount")%>
                        </td>
                        <td>
                            <%#Eval("Money_DuiHuan")%>
                        </td>
                        <td>
                            <input type="checkbox" name="chkExchange" onclick="FunSelect(this,<%#Eval("ID") %>)" value="<%#Eval("ID") %>" />选择
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                   
                    <!--底部模板-->
                    </table>
                    <!--表格结束部分-->
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div id="divNO"  runat="server" style="color:Red; font-weight:bold;display:none;">该储户没有兑换信息</div>
        <div id="divYes" style="display:none" runat="server">
        <input type="button" id="btnSave" value="保存数据" onclick="FunSave()" />&nbsp;
        <input type="button" disabled="disabled" value="打印凭证" />
        <input type="button" disabled="disabled" value="打印存折" />
        </div>
    </div>
 
    <div  style="display:none;">
    <%--兑换业务编号集合--%>
   <input type="text" name="IDList"  />
    </div>
    </form>
    
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
