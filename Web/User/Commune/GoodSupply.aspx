<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodSupply.aspx.cs" Inherits="Web.User.Commune.GoodSupply" %>

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
          var limitMount = 0; //营业员操作限额
          $(function () {
              InitGoodSupply();

              GetMoney_PreDefine(); //获取预付款信息
            
              //初始化存储产品类型信息
              $('select[name=GoodSupplyID]').change(function () {
                  ShowGoodSupplyInfo(); //显示一条商品的信息
              });

              //显示隐藏历史营销信息
              $('#imghistory').toggle(function () {
                  $('#SupplyList').show("normal");
              }, function () {
                  $('#SupplyList').hide("normal");
              });

              //清除重来
              $('#btnChongLai').click(function () {
                  window.location = '/User/Commune/GoodSupply.aspx'; //重新刷新该页面
              });

              GetUserLimit(); //获取营业员操作限额
          });

          function GetUserLimit() {
              //查看当前营业员的限额是否足够
              $.ajax({
                  url: '/Ashx/wbinfo.ashx?type=GetUserLimit',
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      limitMount = parseInt(r);
                  }, error: function (r) {
                     showMsg('获取当前营业员的操作限额时发生错误 ！');
                  }
              });
          }

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

          //加载商品信息
          function InitGoodSupply() {
              $.ajax({
                  url: "/Ashx/commune.ashx?type=GetGoodSupply",
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      for (var i = 0; i < r.length; i++) {
                          $('select[name=GoodSupplyID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                      }
                      ShowGoodSupplyInfo();//显示一条商品的信息
                  }, error: function (r) {
                     showMsg('加载信息失败 ！');
                  }
              });
          }

          //显示一条商品的详细信息
          function ShowGoodSupplyInfo() {
              $.ajax({
                  url: '/Ashx/commune.ashx?type=GetGoodSupplyInfo&ID=' + $('select[name=GoodSupplyID] option:selected').val(),
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {


                      GetGoodSupply(); //获取库存信息

                      $('#spanPrice').html(r[0].Price); //商品单价

                      $('input[name=S_GoodSupplyID]').val(r[0].strName);
                      $('input[name=S_Price]').val(r[0].Price);
                      $('input[name=S_Price_Commune]').val(r[0].Price_Commune);
                      $('input[name=S_Price_WBBack]').val(r[0].Price_WBBack);
                      $('input[name=S_numDisCount]').val(r[0].numDiscount);

                      $('input[name=S_SpecName]').val(r[0].SpecID);
                      $('input[name=S_UnitName]').val(r[0].UnitID);

                  }, error: function (r) {
                     showMsg('加载商品信息时错误错误 ！');
                  }
              });

          }

          //获取当前商品的库存
          function GetGoodSupply() {

              $.ajax({
                  url: '/Ashx/good.ashx?type=GetGoodSupplyStorage&GoodSupplyID=' + $('select[name=GoodSupplyID] option:selected').val(),
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      $('#spanStorage').html(r);

                  }, error: function (r) {
                      $('#spanStorage').html('0');
                  }
              });
          }

          //获取预付款数据
          function GetMoney_PreDefine() {
              $.ajax({
                  url: '/Ashx/commune.ashx?type=GetMoney_PreDefine&C_AccountNumber=' + $('#D_AccountNumber').html(),
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      if (r != "Error" && r != "") {
                         
                          $('input[name=S_Money_PreDefine]').val(r);

                      } else {
                          $('input[name=S_Money_PreDefine]').val('0');
                      }

                  }, error: function (r) {
                      $('input[name=S_Money_PreDefine]').val('0');

                  }
              });
          }


          //删除一条兑换记录
          function FunDelList(numIndex, PreDefine) {
             
              $.ajax({
                  url: '/Ashx/commune.ashx?type=DeleteSupplyList&numIndex=' + numIndex,
                  type: 'post',
                  data: $('#form1').serialize(),
                  dataType: 'text',
                  success: function (r) {
                      if (r == "") {
                          $('#btnJieZhang').attr('disabled', 'disabled');
                      }
                      $('#ExchangeInfo').html('');
                      $('#ExchangeInfo').html(r);
                      if (parseFloat(PreDefine) > 0) {
                          var Money_PreDefine = $('input[name=S_Money_PreDefine]').val();
                         
                          $('input[name=S_Money_PreDefine]').val(parseFloat(Money_PreDefine) + parseFloat(PreDefine));
                     }

                  }, error: function (r) {
                     showMsg('取消失败 ！');
                      return false;
                  }
              });
          }


          //添加结算业务
          function AddBusiness() {
             

              //计算商品的实际价格
              var Price = $('input[name=S_Price]').val(); //商品价格
             //商品数量
              var GoodSupplyCount = $('input[name=GoodSupplyCount]').val();//商品数量
              var Money_Total = accMul(Price, GoodSupplyCount); //商品总价值
              if (parseFloat(Money_Total) > parseFloat(limitMount)) {
                 showMsg('该笔交易超出营业员的单笔限额，无法添加 ！');
                  return false;
              }

              var GoodSupplyStorage = $('#spanStorage').html();//库存数量
              if (parseFloat(GoodSupplyCount) > parseFloat(GoodSupplyStorage)) {
                 showMsg('当前产品的库存不足，无法完成交易 ！');
                  return false;
              }

              $('#ExchangeInfo').fadeIn("normal"); //显示交易信息
              $('#ExchangeSubmit').fadeIn("normal");
            
              //优惠价
              var Price_Commune = $('input[name=S_Price_Commune]').val();
              var Price_YouHui = parseFloat(Price) - parseFloat(Price_Commune);
              //按照社员价的总金额
            
              var Money_Commune = accMul(Price_Commune, GoodSupplyCount);
              //优惠金额
              var Money_YouHui = accMul(Price_YouHui, GoodSupplyCount);

              var Money_PreDefine = $('input[name=S_Money_PreDefine]').val();
              if (parseFloat(Money_Commune) <= parseFloat(Money_PreDefine)) { //需要付款的金额小于预付款
                  Money_PreDefine = parseFloat(Money_PreDefine) - parseFloat(Money_Commune);
              }
              else {
                  Money_PreDefine = 0;
              }
              $('input[name=S_Money_PreDefine_S]').val(Money_PreDefine); //剩余的预付款金额

              $.ajax({
                  url: '/Ashx/commune.ashx?type=UpdateSupplyList',
                  type: 'post',
                  data: $('#form1').serialize(),
                  dataType: 'text',
                  success: function (r) {
                      $('#btnJieZhang').removeAttr('disabled');
                      $('#ExchangeInfo').html('');
                      $('#ExchangeInfo').html(r);
                      $('input[name=S_Money_PreDefine]').val($('input[name=S_Money_PreDefine_S]').val());
                  }, error: function (r) {
                     showMsg('添加商品失败 ！');
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
                              if (r != "Error") {
                                  $('input[name=SupplyID]').val(r);
                              }
                              showMsg("结账操作成功，请打印单据");
                              $('#ImageButton1').attr('disabled', 'disabled');
                              $('#btnChongLai').attr('disabled', 'disabled');
                              $('#btnChongLai').css('background', '#aaa');
                              $('#btnJieZhang').attr('disabled', 'disabled');
                              $('#btnJieZhang').css('background', '#aaa');
                              $('#btnPrint').removeAttr('disabled');
                              $('#btnPrintPage').removeAttr('disabled');

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
              else {
                  return true;
              }
             
          }


          function CloseCommune() {
              $('#divCommuneList').hide();
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
        <b>社员供销管理</b><%--<span id="spanHelp" style="cursor: pointer">帮助</span>--%>
    </div>
    <div id="divHelp" class="pageHelp" style="border:1px solid #333; border-radius:5px; display:none; ">
<span>提示1：请认真填写储户的存储产品信息，储户类型、存期信息；储户类型确定储户存储的是否是定期产品，不同的存期类型也会有不同的利息计算方式！</span><br />
<span>提示2：如果保存后，发现有错误，可以使用 修改错误存粮 修改数据。</span><br />

</div>
<div id="storageQuery">

</div>
    <div style="margin: 20px 0px;">
        <div id="Query">
        <table>
        <tr>
            <td style="width:40px; text-align:right">账号:</td>
        <td style="width:115px; text-align:left"><input type="text" id="QAccountNumber" style="font-size:16px;width:120px; font-weight:bolder;" runat="server" /></td>
        <td style="width:75px; text-align:right">身份证号:</td>
         <td style="width:155px; text-align:left"><input type="text" id="QIDCard" style="font-size:14px; width:160px; font-weight:bolder;" runat="server" /></td>
        <td style="width:40px; text-align:right">姓名:</td>
         <td style="width:9px; text-align:left"><input type="text" id="QstrName" style="font-size:14px; font-weight:bolder; width:100px;" runat="server" /></td>
             <td style="width:60px; text-align:right">手机号:</td>
       <td style="width:90px; text-align:left"><input type="text" id="QPhoneNO" style="font-size:14px; font-weight:bolder; width:100px;" runat="server" /></td>
        <td>  <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png" 
                runat="server" onclick="ImageButton1_Click" /></td>
        </tr>
        </table>
            <span></span>
            
          &nbsp; <%--<span>密码:</span>
            <input type="password" id="QPassword" style="font-size:12px;  width:100px;" runat="server" />--%>
          
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
                            优惠价
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
                        <%#Eval("Price_Commune")%>%
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
             <tr style="margin-bottom:20px;">
                    <td align="right" style="width: 100px;">
                     <span>供销产品:</span> 
                    </td>
                    <td>
                       <select name="GoodSupplyID" style="width:100px"></select>
                       <span>单价</span><span id="spanPrice"></span>  <span>元</span>    
                     &nbsp;   <span>购买数量:</span>  
                        <input type="text" style="width:40px;" name="GoodSupplyCount" value="1"   />
                        &nbsp;   <span>库存:</span> <span id="spanStorage">0</span>           
                    </td>
                <td style="padding:5px 5px;"> <input type="button" id="btnAddEx" onclick="AddBusiness();" value="添加" class="btnOperate" style="width:50px;" /></td>
                </tr>
                </table>
    </div>

    <div id="ExchangeInfo" style="display:none;">
  
        </div>
        <div id="ExchangeSubmit" style="display:none;">
        <input type="button" id="btnChongLai" value="清除重来" style="width:80px;" />&nbsp;
             <input type="button" id="btnJieZhang" value="结账" onclick=" InitBusinessNO();" style="width:80px; font-size:18px; font-weight:bolder; color:Blue; border:1px solid #888;" />&nbsp;
             <input type="button" id="btnPrint" value="打印存折"  onclick=" PrintCunZhe();" disabled="disabled" style="width:80px;" />&nbsp;
                 <input type="button" id="btnPrintFanYe" value="翻页打印"  onclick=" PrintCunZheFanYe();"  style="width:80px; display:none;" />&nbsp;
                 <input type="button" id="btnPrintPage" value="打印小票"  onclick=" PrintPage();" disabled="disabled" style="width:80px;" />&nbsp;
        </div>
   
   
   <div id="divCommuneList" runat="server" style="display:none; position:absolute; top:50px; left:200px; width:600px; height:300px; background-color:#c7edcc; margin:20px; border-radius:10px;">
    <div style="float:right; margin:10px 20px"> <img src="../../images/winClose.png" alt="关闭窗口"  style="float:right; cursor:pointer;" onclick="CloseCommune()" /></div>    
        <asp:Repeater ID="rptCommuneList" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                <tr><td  colspan="5" style="height:25px; text-align:center;"><span style="color:Blue; font-weight:bolder">社员信息</span> </td></tr>
                    <tr class="tr_head">
                        
                        <th style="width: 120px; text-align: center;">
                            账号
                        </th>
                        <th style="width: 100px; text-align: center;">
                            姓名
                        </th>
                        <th style="width: 100px; text-align: center;">
                            身份证号
                        </th>
                        <th style="width: 100px; text-align: center;">
                            电话号码
                        </th>
                        <th style="width: 100px; text-align: center;">
                            选择
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    
                    <td>
                        <%#Eval("AccountNumber")%>
                    </td>
                    <td>
                        <%#Eval("strName")%>
                    </td>
                    <td>
                        <%#Eval("IDCard")%>
                    </td>
                    <td>
                        <%#Eval("PhoneNO")%>
                    </td>
                  <td>
                     <a href="GoodSupply.aspx?AccountNumber=<%#Eval("AccountNumber") %>">选择</a>
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
       
  
    <div  style="display:none;">

    <%--业务编号--%>
      <input type="text"  name="BusinessNO" value="" />
    <%--储户账号--%>
    <input type="text" id="txtC_AccountNumber" name="txtC_AccountNumber" value=""  runat="server"/>
    <input type="text" id="txtC_Name" name="C_Name" value=""  runat="server"/>
    <input type="text"  name="S_SpecName" value="" />
    <input type="text" name="S_UnitName" value="" />
   <%-- 结算信息--%>
     <%-- 商品名称--%>
   <input name="S_GoodSupplyID" value="" />
    <%-- 商品单价--%>
   <input name="S_Price" value="" />
    <%-- 商品社员加--%>
   <input name="S_Price_Commune" value="" />
       <%-- 商品网点返利价--%>
   <input name="S_Price_WBBack" value="" />
 
     <%-- 商品优惠兑换比例--%>
   <input name="S_numDisCount" value="" />

    <%-- 使用预付款金额--%>
   <input id="txtS_Money_PreDefine" name="S_Money_PreDefine"  value="0" />
   <%-- 剩余预付款金额--%>
    <input id="Text1" name="S_Money_PreDefine_S"  value="0" />

    <input name="SupplyID" value="" />
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
