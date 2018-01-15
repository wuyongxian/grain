<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeBankBook.aspx.cs" Inherits="Web.User.OtherOperate.ChangeBankBook" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />  
    <script src="../../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>
    <script src="../../Scripts/LodopPrint.js"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          var BNOList = '';
          var BNOListSurPlue = '';
          var AccountNumber_New = '';//新的储户账号
          $(function () {

              GetNewAccountNumber();

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
                  url: '/User/Query/depositor.ashx?type=GetNewAccountNumber',
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


          function frmSubmit() {
              if (!SubmitCheck()) {//检测输入内容
                  return false;
              }
              var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      $.ajax({
                          url: '/Ashx/commune.ashx?type=ChangeCard_Dep',
                          type: 'post',
                          data: $('#form1').serialize(),
                          dataType: 'json',
                          success: function (r) {
                              if (r.state == "success") {
                                  showMsg(r.msg);
                                  $('#btnSave').attr('disabled', 'disabled');
                                  $('#btnSave').css('background', '#aaa');
                                  $('#btnPrint').removeAttr('disabled');
                                  $('#btnPrint_Storage').removeAttr('disabled');
                                  BNOList = r.BNOList;
                                  AccountNumber_New = r.AccountNumber_New
                              } else {
                                  showMsg(r.msg);
                              }


                          }, error: function (r) {
                              showMsg("操作失败!");
                          }
                      });
                  } else {
                      //console.log('你点击了取消！');
                  }
              });
          }

          function SubmitCheck() {
              if (!CheckNumInt($('input[name=Toll]').val(), '存折收费', -1, -1)) {
                  return false;
              }
              return true;
          }

          function FunPrint() {
              //查询社员信息
              $.ajax({
                  url: '/User/Storage/storage.ashx?type=GetDepositorPrint&AccountNumber=' + $('input[name=AccountNumber]').val(),
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


          function FunPrint_Storage() {
              var url = '/User/Exchange/exchange.ashx?type=PrintDep_OperateLogList';
              var para = 'AccountNumber=' + AccountNumber_New + '&BNOList=' + BNOList;
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
                      
                      if (r.SurPlus != '') {
                          $('#btnPrint_StorageFanYe').fadeIn('normal');//显示可翻页打印
                          BNOListSurPlue = r.SurPlus;
                      }

                      $('#divPrint').html('');
                      $('#divPrint').append(r.Msg);
                      CreateOneFormPage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                  }
              });
          }

          function FunPrint_StorageFanYe() {
              var url = '/User/Exchange/exchange.ashx?type=PrintDep_OperateLogList';
              var para = 'AccountNumber=' + AccountNumber_New + '&BNOList=' + BNOListSurPlue;
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
                      $('#divPrint').html('');
                      $('#divPrint').append(r.Msg);
                      CreateOneFormPage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                  }
              });
          }
      </script>
     <style type="text/css">
        #depositorInfo table,#StorageList table{
        width:750px;
        }
         #divfrm{
        width:730px;
        }
    </style>
</head>
<body>
    <div id="divPrint" style="display: none">
    </div>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b style="color: Red">储户</b> <b>换存折</b><%--<span id="spanHelp" style="cursor: pointer">帮助</span>--%>
        </div>
        <div id="divHelp" class="pageHelp">
            <span>提示1：有两种情况需要储户提出申请，管理审批通过才能办理业务。（1）定期储户如果不到期支取，需要预先提出申请，管理员审批通过方能结算；（2）如果储户支取额度大于营业员的支取额度，也需要申请，管理员审批通过才能支取。</span><br />
            <span>提示2：凭身份证可以申请结算。输入储户的身份证号时，最后一位可以是大写字母X。身份证号正确才能申请存转销。</span><br />

        </div>
        <div id="storageQuery">
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>储户账号:</span></td>
                    <td>
                        <input type="text" id="QAccountNumber" style="font-size: 16px; font-weight: bolder;" runat="server" /></td>
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
        <div id="StorageList" runat="server">
            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <table class="tabData">
                        <tr class="tr_head">
                            <th style="width: 100px; height: 20px; text-align: center;">存贷产品
                            </th>
                            <th style="width: 80px; text-align: center;">结存数量
                            </th>
                            <th style="width: 100px; text-align: center;">存入时间
                            </th>
                            <th style="width: 80px; text-align: center;">存入价
                            </th>
                            <th style="width: 80px; text-align: center;">存期
                            </th>
                            <th style="width: 80px; text-align: center;">天数
                            </th>
                            <th style="width: 80px; text-align: center;">活期利率
                            </th>
                            <th style="width: 100px; text-align: center;">利息
                            </th>

                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr 
                        onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                        <td style="height: 25px;">
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
                            <%#GetDay(Eval("StorageDate"))%>
                        </td>
                        <td>
                            <%#Eval("CurrentRate")%>
                        </td>
                        <td>
                            <%#Eval("strlixi")%>
                        </td>

                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr>
                        <td colspan="2">
                            <span style="font-weight: bolder">折合现金合计:</span>
                        </td>
                        <td colspan="6" style="text-align: center">
                            <span id="spanTotal" runat="server" style="color: Red; font-size: 16px">￥<%=numTotol %></span>

                        </td>
                    </tr>
                    <!--底部模板-->
                    </table>
                <!--表格结束部分-->
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div id="divfrm" class="pageEidtInner" runat="server" style="display: none; border-radius: 10px;">

            <table>
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>新账号:</span>
                    </td>
                    <td>
                        <input type="text" readonly="readonly" style="width: 120px; background-color: #ddd; color: Red; font-size: 16px; font-weight: bolder;" name="AccountNumber" />
                    </td>
                </tr>
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>换折收费:</span>
                    </td>
                    <td>
                        <input type="text" style="width: 120px; font-size: 16px; font-weight: bolder;" name="Toll" value="0" /><span>元</span>
                    </td>
                </tr>
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>更换日期:</span>
                    </td>
                    <td>
                        <input type="text" readonly="readonly" style="width: 120px; background-color: #ddd" name="dt_change" />
                    </td>
                </tr>

                <tr>
                    <td></td>
                    <td>
                        <input type="button" id="btnSave" onclick="frmSubmit();" value="保存数据" />&nbsp;
                 <input type="button" id="btnPrint" onclick="FunPrint();" disabled="disabled" value="打印存折" />&nbsp;
                  <input type="button" id="btnPrint_Storage" onclick="FunPrint_Storage();" disabled="disabled" value="打印存储记录" />&nbsp;
                  <input type="button" id="btnPrint_StorageFanYe" style="display:none;" value="翻页打印" onclick="FunPrint_StorageFanYe();" />
                    </td>
                </tr>
            </table>
        </div>


        <div style="display: none;">
            <%--选择兑换的存储产品信息--%>
            <input type="text" name="txtDep_SID" value="" />

            <input type="text" id="txtC_AccountNumber" name="C_AccountNumber" runat="server" />
        </div>
    </form>

    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>


