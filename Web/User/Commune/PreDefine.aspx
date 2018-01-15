<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreDefine.aspx.cs" Inherits="Web.User.Commune.PreDefine" %>

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
                          url: '/Ashx/commune.ashx?type=AddGoodSupply',
                          type: 'post',
                          data: $('#form1').serialize(),
                          dataType: 'text',
                          success: function (r) {
                              showMsg('添加数据成功，请打印单据 ！');
                              $('#btnAdd').attr('disabled', 'disabled');
                              $('#btnAdd').css('background', '#aaa');
                              $('#btnPrint').removeAttr("disabled");
                          }, error: function (r) {
                              showMsg('添加数据失败 ！');
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

              if ($('input[name=Money_PreDefine]').val() == '') {
                 showMsg('实付金额不能为空！');
                  return false;
              }
              return true;
          }


          //计算抵用金额
          function FunCalc() {

              var numReality = $('input[name=Money_Reality]').val();
              if (!CheckNumDecimal(numReality, '实付金额', 2)) {
                  return false;
              }

              var numRate = $('input[name=Money_Rate]').val();
              if (!CheckNumDecimal(numRate, '抵用率', 2)) {
                  return false;
              }

              var numPreDefine = accDiv(numReality, numRate);
              numPreDefine = accMul(100, numPreDefine);
              numPreDefine = changeTwoDecimal_f(numPreDefine);
             
              $('input[name=Money_PreDefine]').val(numPreDefine);
          }


          /*--------End 数据增删改操作--------*/

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
          /*-------打印存折--------*/
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
                  url: '/Ashx/commune.ashx?type=GetC_SupplyPrint&BusinessNO=' + $('input[name=BusinessNO]').val() + '&AccountNumber=' + $('#D_AccountNumber').html(),
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
          /*-------End 打印存折--------*/

      </script>
</head>
<body>
    <div id="divPrint" style="display: none">
    </div>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>供销预付款</b>
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>社员账号:</span>
                    </td>
                    <td>
                        <input type="text" id="QAccountNumber" style="font-size: 16px; font-weight: bolder;" runat="server" /></td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                            runat="server" OnClick="ImageButton1_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <div id="depositorInfo" runat="server" style="display: none;">
            <table class="tabData" style="margin: 20px 0px;">
                <tr>
                    <td colspan="6" style="border-bottom: 1px solid #aaa; height: 25px; text-align: center">
                        <span style="font-size: 16px; font-weight: bolder; color: Green">社员基本信息</span>
                    </td>
                </tr>
                <tr>
                    <th align="center" style="width: 100px; height: 30px;">社员账号
                    </th>
                    <th align="center" style="width: 100px;">社员姓名
                    </th>
                    <th align="center" style="width: 100px;">移动电话
                    </th>
                    <th align="center" style="width: 100px;">田亩册
                    </th>
                    <th align="center" style="width: 150px;">身份证号
                    </th>
                    <th align="center" style="width: 200px;">社员地址
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
                        <span style="font-weight: bolder; color: Blue;" id="D_FieldCopies" runat="server"></span>
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

        <div id="StorageList" runat="server" style="display: none">
            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <table class="tabData">
                        <tr class="tr_head">

                            <th style="width: 120px; text-align: center;">预存金额
                            </th>
                            <th style="width: 100px; text-align: center;">预存时间
                            </th>
                            <th style="width: 100px; text-align: center;">天数
                            </th>
                            <th style="width: 100px; text-align: center;">经办人
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

        <div id="divfrm" runat="server" style="background-color: #ddd; border-radius: 10px; display: none;">


            <div>
                <table>
                    <tr style="margin-bottom: 20px;">

                        <td>
                            <span>实付金额:</span>
                            <input type="text" style="width: 60px;" name="Money_Reality" />
                            &nbsp;   <span>抵用率:</span>
                            <input type="text" style="width: 60px;" name="Money_Rate" value="100" />%
                        <input type="button" value="计算" id="btnCalc" style="width: 60px;" onclick="FunCalc()" />
                            &nbsp;   <span>抵用金额:</span>
                            <input type="text" readonly="readonly" style="width: 60px;" name="Money_PreDefine" />
                            <span>元</span>
                        </td>

                        <td align="right"></td>
                        <td></td>

                    </tr>
                    <tr>
                        <td>
                            <input type="button" id="btnAdd" value="保存数据" onclick="InitBusinessNO()" />&nbsp;&nbsp;
                         <input type="button" id="btnPrint" value="打印存折" disabled="disabled" onclick="FunPrint()" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="divMsg" style="color: Red; font-size: 16px;"></div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div style="display: none;">
            <%--业务编号--%>
            <input type="text" name="BusinessNO" value="" />
            <%--储户账号--%>
            <input type="text" id="txtC_AccountNumber" name="C_AccountNumber" value="" runat="server" />

            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>

</body>
</html>
