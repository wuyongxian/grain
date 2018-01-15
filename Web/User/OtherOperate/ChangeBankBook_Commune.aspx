<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeBankBook_Commune.aspx.cs" Inherits="Web.User.OtherOperate.ChangeBankBook_Commune" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   
       <script src="../../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {
              //显示隐藏历史营销信息
              $('#imghistory').toggle(function () {
                  $('#SupplyList').show("normal");
              }, function () {
                  $('#SupplyList').hide("normal");
              });


              GetNewAccountNumber();//获取新的存折账号

              //添加时间
              var now = new Date(); //获取系统日期，即Sat Jul 29 08:24:48 UTC+0800 2006
              var yy = now.getFullYear(); //截取年，即2006
              var mo = now.getMonth() + 1; //截取月，即07 （系统中的月份为0~11，所有使用的时候要+1）
              var dd = now.getDate(); //截取日，即29
              //取时间
              var hh = now.getHours(); //截取小时，即8
              var mm = now.getMinutes(); //截取分钟，即34
              var ss = now.getSeconds(); //获取秒 
              $('input[name=dt_change]').val(yy + '-' + mo + '-' + dd + ' ' + hh + ':' + mm + ':' + ss);

          });


                     function GetNewAccountNumber() {
                         $.ajax({
                             url: '/Ashx/commune.ashx?type=GetNewAccountNumber',
                             type: 'post',
                             data: '',
                             dataType: 'text',
                             success: function (r) {
                                 $('input[name=AccountNumber]').val(r);
                             }, error: function (r) {
                                showMsg('加载信息失败 ！');

                             }
                         });
                     }


          /*--------数据增删改操作--------*/
      
          function frmSubmit() {
              if (!SubmitCheck()) {//检测输入内容
                  return false;
              }

              var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      $.ajax({
                          url: '/Ashx/commune.ashx?type=ChangeCard_Commune',
                          type: 'post',
                          data: $('#form1').serialize(),
                          dataType: 'text',
                          success: function (r) {
                              if (r != "Error") {


                                  alert("操作成功，请打印新存折");

                                  $('#btnSave').attr('disabled', 'disabled');
                                  $('#btnSave').css('background', '#aaa');
                                  $('#btnPrint').removeAttr('disabled');
                              }


                          }, error: function (r) {
                              showMsg('操作失败 ！');
                          }
                      });
                  } else {
                      //console.log('你点击了取消！');
                  }
              });
          }

          //提交检测
          function SubmitCheck() {
              if (!CheckNumInt($('input[name=Toll]').val(), '存折收费', -1, -1)) {
                  return false;
              }
              return true;
          }


          /*--------End 数据增删改操作--------*/

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


          function FunPrint() {
              //查询社员信息
              $.ajax({
                  url: '/Ashx/commune.ashx?type=GetCommunePrint&AccountNumber=' + $('input[name=AccountNumber]').val(),
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

          }
      </script>
</head>
<body>
    <div id="divPrint" style="display:none">

    </div>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b style="color:Red">社员</b> <b>换存折</b><%--<span id="spanHelp" style="cursor: pointer">帮助</span>--%>
    </div>
    <div id="divHelp" class="pageHelp" style="border:1px solid #333; border-radius:5px; display:none; ">
<span>提示1：请认真填写储户的存储产品信息，储户类型、存期信息；储户类型确定储户存储的是否是定期产品，不同的存期类型也会有不同的利息计算方式！</span><br />
<span>提示2：如果保存后，发现有错误，可以使用 修改错误存粮 修改数据。</span><br />

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
        <div id="Supply" runat="server" style="display:none">
        <div id="Supplyhistory" style="background-color:#ddd; width:300px; border-radius:5px; padding:5px 5px 5px 20px"><span style="color:Blue;">历史交易记录</span> 
        <img id="imghistory" style="width:20px; height:20px;" src="../../images/search2_blue.png" />        </div>
        <div id="SupplyList" style="display:none">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                    <tr class="tr_head">
                        <th style="width: 120px; height:20px; text-align: center;">
                            供销产品
                        </th>
                        <th style="width: 80px; text-align: center;">
                            单位名称
                        </th>
                        <th style="width: 100px; text-align: center;">
                            购买数量
                        </th>
                        <th style="width: 100px; text-align: center;">
                            商品单价
                        </th>
                        <th style="width: 100px; text-align: center;">
                            商品总价金额
                        </th>
                        <th style="width: 100px; text-align: center;">
                            折率
                        </th>
                          <th style="width: 100px; text-align: center;">
                            优惠金额
                        </th>
                          <th style="width: 100px; text-align: center;">
                            预付款金额
                        </th>
                        <th style="width: 100px; text-align: center;">
                            实付金额
                        </th>
                        <th style="width: 100px; text-align: center;">
                            交易时间
                        </th>
                        
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height:25px;">
                        <%#Eval("GoodSupplyName")%>
                    </td>
                    <td>
                        <%#Eval("UnitName")%>
                    </td>
                    <td>
                        <%#Eval("GoodSupplyCount")%>
                    </td>
                    <td>
                        <%#Eval("GoodSupplyPrice")%>
                    </td>
                    <td>
                        <%#Eval("Money_Total")%>
                    </td>
                     <td>
                        <%#Eval("numDisCount")%>%
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
                        <%#Eval("dt_Trade")%>
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
    <div id="divfrm" runat="server" style="background-color:#eee;border-radius:10px; display:none;" >

            <table>
             <tr >
                    <td align="right" style="width: 100px;">
                     <span>新账号:</span> 
                    </td>
                    <td>
                        <input type="text" readonly="readonly" style="width:150px; background-color:#ddd; font-size:16px; font-weight:bolder; color:Red;"  name="AccountNumber"   />            
                    </td>      
                </tr>
                <tr >
                    <td align="right" style="width: 100px;">
                     <span>换折收费:</span> 
                    </td>
                    <td>
                        <input type="text" style="width:150px; font-size:15px; font-weight:bolder;" name="Toll"  value="0"  />            
                    </td>      
                </tr>
                <tr >
                    <td align="right" style="width: 100px;">
                     <span>更换日期:</span> 
                    </td>
                    <td>
                        <input type="text" readonly="readonly"  style="width:150px; background-color:#ddd" name="dt_change"   />            
                    </td>      
                </tr>

                <tr>
                <td></td>
                <td>
                <input type="button" id="btnSave" onclick="frmSubmit();" value="保存数据" />&nbsp;
                 <input type="button" id="btnPrint" onclick="FunPrint();" disabled="disabled" value="打印存折" />&nbsp;
                </td>
                </tr>
                </table>
    </div>

   
       
  
    <div  style="display:none;">

   <input type="text" id="txtC_AccountNumber" name="C_AccountNumber" runat="server" />
    </div>
    </form>
   
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>

