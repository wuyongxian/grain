<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodSupplyReturn.aspx.cs" Inherits="Web.User.Commune.GoodSupplyReturn" %>

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
 
          });

          //加载新的业务编号
          function InitBusinessNO() {
              $.ajax({
                  url: "/Ashx/commune.ashx?type=GetNewBusinessNO&txtC_AccountNumber=" + $('#D_AccountNumber').html(),
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


          /*--------数据增删改操作--------*/
          //新增数据方法
          function frmSubmit() {
              if (!SubmitCheck()) {//检测输入内容
                  return false;
              }
              var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      $.ajax({
                          url: '/Ashx/commune.ashx?type=AddC_Supply',
                          type: 'post',
                          data: $('#form1').serialize(),
                          dataType: 'text',
                          success: function (r) {
                          

                          }, error: function (r) {
                              showMsg('结账操作失败 ！');
                          }
                      });
                  } else {
                      //console.log('你点击了取消！');
                  }
                
              });

          }

          //提交检测
          function SubmitCheck() {

              var AccountNumber = $('#D_AccountNumber').html()
              if (AccountNumber == '' || AccountNumber == undefined) {
                 showMsg('请先选择社员账号！');
                  $('#QAccountNumber').focus();
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
                     showMsg('加载网点打印坐标时出现错误 ！');
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




          function PrintCunZhe() {

              $.ajax({
                  url: '/Ashx/commune.ashx?type=PrintC_OperateLogList&AccountNumber=' + $('#D_AccountNumber').html(),
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (r.SurPlus != '') {
                          $('#btnPrintFanYe').fadeIn('normal'); //显示可翻页打印
                      }
                      $('#divPrint').html('');
                      $('#divPrint').append(r.Msg);
                      CreateOneFormPage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                     showMsg('打印存折时出现错误 ！');
                  }
              });
          }


          function PrintCunZheFanYe() {

              $.ajax({
                  url: '/Ashx/commune.ashx?type=PrintC_OperateLogList&AccountNumber=' + $('#D_AccountNumber').html() + '&Surplus=1', //添加翻页打印标识
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {

                      $('#divPrint').html('');
                      $('#divPrint').append(r.Msg);
                      CreateOneFormPage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                     showMsg('加载打印坐标时出现错误 ！');
                  }
              });
          }



          function PrintPage() {
              $.ajax({
                  url: '/Ashx/commune.ashx?type=PrintSupplyList&AccountNumber=' + $('#D_AccountNumber').html(),
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r);
                      CreatePage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                     showMsg('打印凭据坐标时出现错误 ！');
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

              LODOP.ADD_PRINT_HTM(20, 60, 800, 1000, document.getElementById("divPrintPaper").innerHTML);

          };

      </script>
</head>
<body>
    <div id="divPrint" style="display:none">

    </div>
     <div id="divPrintPaper" style="display:none">

    </div>
    <form id="form1" runat="server">
    <div class="pageHead">
        <b>供销商品退还</b><%--<span id="spanHelp" style="cursor: pointer">帮助</span>--%>
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
           &nbsp; <span>密码:</span>
            <input type="password" id="QPassword" style="font-size:12px;  width:100px;" runat="server" />
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
                        社员地址
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
      
  <div id="divSupplyList" style="margin:20px 0px;">
        <span style="font-size:16px; font-weight:bolder;">储户今日购买信息</span>
        
            <asp:Repeater ID="Repeater2" runat="server">
                <HeaderTemplate>
                    <table class="tabData">
                        <tr class="tr_head">
                            <th style="width: 100px; height: 20px; text-align: center;">
                                商品名
                            </th>
                            <th style="width: 80px; text-align: center;">
                                单位
                            </th>
                            <th style="width: 100px; text-align: center;">
                                数量
                            </th>
                            <th style="width: 80px; text-align: center;">
                                单价
                            </th>
                            <th style="width: 80px; text-align: center;">
                                优惠金额
                            </th>
                            <th style="width: 80px; text-align: center;">
                                退还金额
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
                            <%#Eval("Money_YouHui")%>
                        </td>
                        <td>
                            <%#Eval("Money_Return")%>
                        </td>
                      
                        <td>
                            <input type="checkbox" name="chkSelect" onclick="FunSelect(this,<%#Eval("ID") %>)" value="<%#Eval("ID") %>" />选择
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
        <div id="divNO"  runat="server" style="color:Red; font-weight:bold;display:none;">该储户今日没有兑换信息</div>

    </div>
    

    <div id="ExchangeInfo" style="display:none;">
  
        </div>
        <div id="ExchangeSubmit" style="display:none;">
       
             <input type="button" id="btnJieZhang" value="退还商品" onclick=" InitBusinessNO();" style="width:80px; font-size:18px; font-weight:bolder; color:Blue; border:1px solid #888;" />&nbsp;
             <input type="button" id="btnPrint" value="打印存折"  onclick=" PrintCunZhe();" disabled="disabled" style="width:80px;" />&nbsp;
                 <input type="button" id="btnPrintFanYe" value="翻页打印"  onclick=" PrintCunZheFanYe();"  style="width:80px; display:none;" />&nbsp;
        </div>
              
       
  
    <div  style="display:none;">

    <%--业务编号--%>
      <input type="text"  name="BusinessNO" value="" />
   
        <%--剩余未打印的编号--%>
     <input name="BusinessNOSurPlus" value="" />
    </div>
    </form>
   
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>

