<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyToSell.aspx.cs" Inherits="Web.User.Exchange.Apply.ApplyToSell" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
   
    

      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {

              var Dep_SID = getQueryString("Dep_SID");
           
              if (Dep_SID != "") {
                  $('#divSell').fadeIn("normal");
                  $('input[name=txtDep_SID]').val(Dep_SID);

                  //查找当前选择的存储信息
                  InitStorageInfo(Dep_SID);
                  //加载当前的储户信息
                  InitDepositor(Dep_SID);
              }


              $('#btnJiSuan').click(function () {
                  FunJiSuan();
              });
              $('#btnFanSuan').click(function () {
                  FunFanSuan();
              });
            
          });



        

          function frmSubmit() {

              if (!CheckNumDecimal($("input[name=VarietyCount]").val(), '申请数量', 2)) {
                  return false;
              }
              if (parseFloat($("input[name=VarietyCount]").val()) <= 0) {
                 showMsg('您申请的数量不能为0或负数 ！');
                  return false;
              }
              if (parseFloat($("input[name=VarietyCount]").val()) > parseFloat($('#StorageNumber').html())) {
                 showMsg('您申请的数量不能大于当前的结存数量 ！');
                  return false;
              }

              //验证身份证号是否正确
              var AN = $('#D_AccountNumber').html(); //储户账号
          
              $.ajax({
                  url: '/Ashx/depositor.ashx?type=GetDepositorByAccountNumber&AN=' + AN,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {

                      if (r[0].IDCard == $('input[name=IDCard]').val()) {
                          /*存转销申请*/
                          var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
                          showConfirm(msg, function (obj) {
                              if (obj == 'yes') {
                                  
                                  $.ajax({
                                      url: '/Ashx/exchangeprop.ashx?type=Add_SellApply',
                                      type: 'post',
                                      data: $('#form1').serialize(),
                                      dataType: 'text',
                                      success: function (r) {
                                          if (r == "OK") {
                                              showMsg('提交成功，等待管理员审核 ！');
                                              $('#btnAdd').attr('disabled', 'disabled');
                                              $('#btnAdd').css('background', '#aaa');
                                          } else {
                                              showMsg('提交失败 ！');
                                          }
                                      }, error: function (r) {
                                          showMsg('提交失败 ！');
                                      }
                                  });
                              } else {
                                  //console.log('你点击了取消！');
                              }
                          });
                          /* End 存转销申请*/

                      } else {
                         showMsg(' 您输入的身份证号不正确 ！');
                      }
                  }, error: function (r) {
                     showMsg(' 查找用户信息时发生未知错误 ！');
                  }
              });

             
          }




          function InitStorageInfo(Dep_SID) {
              $.ajax({
                  url: '/User/Exchange/exchange.ashx?type=GetStorageInfoByID&ID=' + Dep_SID,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                   
                      $('#StorageList').hide();
                      $('#Storage').show();
                      $('#depositorInfo').show();
                      $('#VarietyID').html(r[0].VarietyID);
                      $('#StorageNumber').html(r[0].StorageNumber);
                      $('input[name=txtJieCun]').val(r[0].StorageNumber); //记录结存数量
                      $('#StorageDate').html(r[0].StorageDate);
                      $('#Price_ShiChang').html(r[0].Price_ShiChang);
                      $('input[name=txtPrice_ShiChang]').val(r[0].Price_ShiChang);
                      $('#TimeID').html(r[0].TimeID);
                      $('#numDate').html(r[0].numDate);

                      $('input[name=txtVarietyID]').val(r[0].txtVarietyID);
                      $('input[name=txtTimeID]').val(r[0].txtTimeID);

                      $('#spanUnit').html(r[0].UnitID);
                  }, error: function (r) {
                     showMsg('加载储户存储信息失败 ！');
                  }
              });
          }

         

          function InitDepositor(Dep_SID) {
              $.ajax({
                  url: '/User/Exchange/exchange.ashx?type=GetDepInfoByStorageID&ID=' + Dep_SID,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      $('#StorageList').hide();
                      $('#Storage').show();
                      $('#depositorInfo').show();
                      $('#QAccountNumber').val(r[0].AccountNumber);

                      $('#D_AccountNumber').html(r[0].AccountNumber);
                      $('#D_strName').html(r[0].strName);
                      $('#D_PhoneNo').html(r[0].PhoneNo);
                      $('#D_strAddress').html(r[0].strAddress);
                      $('#D_numState').html(r[0].numState);
                      $('#D_IDCard').html(r[0].IDCard);

                  }, error: function (r) {
                     showMsg('加载储户存储信息失败 ！');
                  }
              });
          }


         function FunJiSuan() {
              var VarietyCount = $.trim($('input[name=VarietyCount]').val

());
              if (VarietyCount == '' || VarietyCount == '0') {
                 showMsg('请输入兑换数量 ！');
                  return false;
              }
              $.ajax({
                  url: '/Ashx/exchangeprop.ashx?type=SellJiSuan&txtDep_SID=' 

+ $('input[name=txtDep_SID]').val() + '&VarietyCount=' + VarietyCount,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                    
                      $('input[name=VarietyCount]').val(r.Count);
                      $('input[name=VarietyMoney]').val(r.Money);
                      $('#divMsg').html('');
                      $('#divMsg').append(r.Msg);
                  }, error: function (r) {
                     showMsg('计算操作失败 ！');
                  }
              });
          }

          function FunFanSuan() {
              var VarietyMoney = $.trim($('input[name=VarietyMoney]').val

());
              if (VarietyMoney == '' || VarietyMoney == '0') {
                 showMsg('请输入存转销金额 ！');
                  return false;
              }
              $.ajax({
                  url: '/Ashx/exchangeprop.ashx?type=SellFanSuan&txtDep_SID=' + $('input[name=txtDep_SID]').val() + '&VarietyMoney=' + VarietyMoney,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      $('input[name=VarietyCount]').val(r.Count);
                      $('input[name=VarietyMoney]').val(r.Money);
                      $('#divMsg').html('');
                      $('#divMsg').append(r.Msg);
                  }, error: function (r) {
                     showMsg('反算操作失败 ！');
                  }
              });
          }

          /*--ENd loadFuntion--*/




      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
        <b>储户申请存转销</b><span id="spanHelp" style="cursor: pointer">帮助</span>
    </div>
    <div id="divHelp"  class="pageHelp">
<span>提示1：有两种情况需要储户提出申请，管理审批通过才能办理业务。（1）定期储户如果不到期支取，需要预先提出申请，管理员审批通过方能结算；（2）如果储户支取额度大于营业员的支取额度，也需要申请，管理员审批通过才能支取。</span><br />
<span>提示2：凭身份证可以申请结算。输入储户的身份证号时，最后一位可以是大写字母X。身份证号正确才能申请存转销。</span><br />

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
           <div id="depositorInfo"  runat="server"  style="display:none;">
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
        <div id="StorageList" runat="server" >
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
                        <th style="width: 160px; text-align: center;">
                            结算类型
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
                    <td>
                       <a href="/User/Exchange/Apply/ApplyToSell.aspx?Dep_SID=<%#Eval("ID") %>">申请存转销售</a>
                       
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
            <tr>
            <td colspan="2">
            <span style="font-weight:bolder">折合现金合计:</span>
            </td>
            <td colspan="6" style="text-align:center">
            <span id="spanTotal" runat="server" style="color:Red; font-size:16px">￥<%=Web.common.numTotol %></span>
          
            </td>
            </tr>
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
        </div>
        <div id="Storage" style="display:none;">
          <table class="tabData" style="margin:20px 0px;">
          <tr>
          <td colspan="6" style="font-weight:bolder; height:25px; color:Green; font-size:16px; border-bottom:1px solid #888;">存储信息</td>
          </tr>
                    <tr class="tr_head">
                        <th style="width: 200px; height:30px; text-align: center;">
                            存储产品
                        </th>
                        <th style="width: 110px; text-align: center;">
                            结存数量
                        </th>
                        <th style="width: 110px; text-align: center;">
                            存入时间
                        </th>
                        <th style="width: 110px; text-align: center;">
                            存入价
                        </th>
                        <th style="width: 110px; text-align: center;">
                            存期
                        </th>
                        <th style="width: 110px; text-align: center;">
                            天数
                        </th>
                    </tr>
                    <tr>
                     <td style="height:30px;"><span id="VarietyID" style="font-weight:bolder; color:Blue; padding:5px 0px;"></span></td>
                     <td><span id="StorageNumber" style="font-weight:bolder; color:Blue; padding:5px 0px;"></span></td>
                      <td><span id="StorageDate" style="font-weight:bolder; color:Blue; padding:5px 0px;"></span></td>
                       <td><span id="Price_ShiChang" style="font-weight:bolder; color:Blue; padding:5px 0px;"></span></td>
                        <td><span id="TimeID" style="font-weight:bolder; color:Blue; padding:5px 0px;"></span></td>
                         <td><span id="numDate" style="font-weight:bolder; color:Blue; padding:5px 0px;"></span></td>
                    </tr>
                    </table>
           
        </div>
        
             
        <div id="divSell" style="display:none;">
            <table>
               
                <tr>
                    <td align="right">
                        <span>申请数量:</span>
                    </td>
                    <td>
                      <input type="text" name="VarietyCount" value="0"  

style="width: 100px;" /><span style="color:Red">*</span>
                        <span id="spanUnit"></span>
                        <input type="button" id="btnJiSuan" value="计算" 

style="width:60px;" />&nbsp;
                         <input type="button" id="btnFanSuan" value="反算" 

style="width:60px;" />
                    </td>
                </tr>
                  <tr>
                  <td align="right">
                        <span>折合现金:</span>
                    </td>
                    <td>
                        <input type="text" name="VarietyMoney" value="0"  style="width: 100px;" /><span >元</span>
                      
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>身份证号:</span>
                    </td>
                    <td>
                        <input type="text" name="IDCard" value="0"  style="width: 150px;" /><span style="color:Red">*</span>
                      
                    </td>
                </tr>
                
                <tr >
                    <td>
                    </td>
                    <td>
                        <input type="button" id="btnAdd" value="提交申请" onclick="frmSubmit()" />&nbsp;
                      
                    </td>
                </tr>
              
            </table>
        </div>


       
    </div>
    
    <div  style="display:none;">
      <%--选择兑换的存储产品信息--%>
     <input type="text" name="txtDep_SID" value="" />

    </div>
    </form>
    
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>


