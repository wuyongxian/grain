<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommuneLoss.aspx.cs" Inherits="Web.User.OtherOperate.CommuneLoss" %>

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

          /*--------数据增删改操作--------*/
          //新增数据方法
          function FunSave() {
              if ($('#D_AccountNumber').html() == "") {
                 showMsg('请选择储户账号！');
                  return false;
              }
              var IDCard = $.trim($('input[name=IDCard]').val());
              if (IDCard == "") {
                 showMsg('请输入身份证号！');
                  return false;
              }
              //验证身份证号是否正确
              var AN = $('#D_AccountNumber').html(); //储户账号

              $.ajax({
                  url: '/Ashx/commune.ashx?type=GetCommuneByAccountNumber&txtAccountNumber=' + AN,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {

                      if (r[0].IDCard == IDCard) {
                          //更新用户状态
                          $.ajax({
                              url: '/Ashx/commune.ashx?type=Update_CommuneState&AN=' + AN,
                              type: 'post',
                              data: $('#form1').serialize(),
                              dataType: 'text',
                              success: function (r) {
                                 showMsg(' 操作成功 ！');
                              }, error: function (r) {
                                 showMsg(' 操作失败 ！');
                              }
                          });

                      } else {
                         showMsg(' 您输入的身份证号不正确 ！');
                      }
                  }, error: function (r) {
                     showMsg(' 查找用户信息时发生未知错误 ！');
                  }
              });



          }




          /*--------End 数据增删改操作--------*/
          function PrintCunDan() {

          }

          function PrintCunZhe() {

          }

          function PrintXiaoPiao() {

          }

          function ShowDepositorInfo() {
              $('#depositorInfo').fadeIn('normal');
          }
      

      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
        <b>社员挂失/解除挂失</b><%--<span id="spanHelp" style="cursor: pointer">帮助</span>--%>
    </div>
    <div id="divHelp" class="pageHelp">
        <span>提示1：输入储户账号才能挂失。如果存折已丢，不知道自己的账号，可以按姓名查找，找到账号后，记录账号，再挂失。 </span>
        <br />
        <span>提示2：如果挂失时把储户的身份证号输错了，解除挂失时输入正确的身份证号不能执行解除挂失，请管理员解决，管理员可以进行强力解除挂失。</span><br />
    </div>
<div id="storageQuery">

</div>
    <div style="margin: 20px 0px;">
        <div id="Query">
            <span>社员账号:</span>
            <input type="text" id="QAccountNumber" style="font-size:16px; font-weight:bolder;" runat="server" />
          
            <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png" 
                runat="server" onclick="ImageButton1_Click" />
        </div>
         <div id="depositorInfo"  runat="server" style="display:none;">
            <table class="tabData"  style="margin:20px 0px;">
                <tr >
                    <td colspan="6" style="border-bottom:1px solid #aaa; height:25px; text-align:center">
                        <span style="font-size: 16px; font-weight: bolder; color:Green">社员基本信息</span>
                    </td>
                </tr>
                <tr>
                    <th align="center" style="width:100px; height:30px;">
                        社员账号
                    </th>
                    <th align="center" style="width:100px;">
                        社员姓名
                    </th>
                     <th align="center" style="width:100px;">
                        移动电话
                    </th>
                      <th align="center" style="width:100px;">
                        田亩册
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
                        <span style="font-weight:bolder; color:Blue;" id="D_FieldCopies" runat="server"></span>
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
  
   <div id="divPreDefine" runat="server" style="display:none; margin:10px 0px;">
        <asp:Repeater ID="RPreDefine" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                <tr><td  colspan="5" style="height:25px; text-align:center;"><span style="color:Blue; font-weight:bolder">预存款信息</span> </td></tr>
                    <tr class="tr_head">
                        <th style="width: 200px; height:20px; text-align: center;">
                            供销产品
                        </th>
                        <th style="width: 120px; text-align: center;">
                            预存金额
                        </th>
                        <th style="width: 100px; text-align: center;">
                            预存时间
                        </th>
                        <th style="width: 100px; text-align: center;">
                            天数
                        </th>
                        <th style="width: 100px; text-align: center;">
                            经办人
                        </th>
                        
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height:25px;">
                        <%#Eval("GoodSupplyID")%>
                    </td>
                    <td>
                        <%#Eval("Money_PreDefine")%>
                    </td>
                    <td>
                        <%#Eval("dt_Trade")%>
                    </td>
                    <td>
                        <%#Eval("numday")%>
                    </td>
                    <td>
                        <%#Eval("UserID")%>
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

    </div>

    </div>
    <div id="divfrm" class="pageEidtInner" style="border-radius:10px;" >
       
        
        <div>
            <table>
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>选择操作:</span>
                    </td>
                    <td>
                        <input name="GuaShi" checked="checked" type="radio" value="0" />挂失&nbsp; &nbsp;
                        <input name="GuaShi" type="radio" value="1" />
                        解除挂失
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>身份证号:</span>
                    </td>
                    <td>
                        <input type="text" name="IDCard" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>操作日期:</span>
                    </td>
                    <td>
                        <input type="text" id="dt_GuaShi" disabled="disabled" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="button" id="btnAdd" value="保存数据" onclick="FunSave()" />&nbsp;
                       <%-- <input type="button" id="btnCunDan" value="打印凭证" disabled="disabled" onclick="PrintCunDan()" />--%>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="divMsg" style="color: Red; font-size: 16px;">
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div  style="display:none;">
   

    </div>
    </form>
    
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
