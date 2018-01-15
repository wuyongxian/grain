<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommuneClosing.aspx.cs" Inherits="Web.User.OtherOperate.CommuneClosing" %>

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

           
              var total =<%=numTotol %>;
             
              if (total >0) {
                 showMsg('当前社员预存款不为0，不能销户！');
                  $('#btnAdd').attr('disabled','disabled');
                  return false;
              }
              if (!CheckNumDecimal($('input[name=numMoney]').val(), '退换金额', 2)) {
                  return false;
              }
              //验证身份证号是否正确
              var AN = $('#D_AccountNumber').html(); //储户账号
              var msg = '您确定要对此社员执行销户操作吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      $.ajax({
                          url: '/Ashx/commune.ashx?type=CloseCommune&AN=' + AN,
                          type: 'post',
                          data: '',
                          dataType: 'text',
                          success: function (r) {
                              showMsg(' 操作成功 ！');
                          }, error: function (r) {
                              showMsg(' 操作失败 ！');
                          }
                      });
                  } else {
                      //console.log('你点击了取消！');
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
        <b>储户销户</b><%--<span id="spanHelp" style="cursor: pointer">帮助</span>--%>
    </div>
    <div id="divHelp" style="border:1px solid #333; border-radius:5px; display:none; ">
<span>提示1：结存数量为0.00的储户才可以销户。如果开户时收取了折（卡）押金，销户时可以退还卡押金。开户时没有收取押金的储户可以不销户。
</span><br />

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
                 <td style="width:120px;">
                       
                    </td>
                    <td style="width: 200px;">
                        <span style=" font-weight:bolder">退换押金金额</span>
                    </td>
                   
                </tr>
                <tr>
                    <td align="right">
                        <span>卡折押金:</span>
                    </td>
                    <td>
                        <input type="text" name="numMoney" value="0" style="width: 100px; font-weight:bolder;" /><span>元</span>
                    </td>
                </tr>
              
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="button" id="btnAdd" value="保存数据" onclick="FunSave()" />&nbsp;
                    <%--    <input type="button" id="btnCunDan" value="打印凭证" disabled="disabled" onclick="PrintCunDan()" />--%>
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
