<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WBSupplyList.aspx.cs" Inherits="Web.Admin.Good.WBSupplyList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
   
    
    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {

              InitWBID();
              $('#QWBID').change(function () {
                  if ($('#QWBID option:selected').val() != "") {
                      $('#txtWBID').val($('#QWBID option:selected').text());
                  }
              });
          });
          function InitWBID() {
              var WBName = $.trim($('#txtWBID').val());
              $('#QWBID').empty();
              $.ajax({
                  url: '/Ashx/wbinfo.ashx?type=GetWBByName&strName=' + WBName,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
               
                      if (WBName == '') {
                          $('#QWBID').append("<option value=''>--请选择--</option>");
                      }
                      if (r.responseText == "Error") { return false; }
                      for (var i = 0; i < r.length; i++) {
                          $('#QWBID').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                      }

                  }, error: function (r) {

                  }
              });
          }


          function ShowUpdateQuanlity(ID, GoodSupplyID, WBID, Quanlity) {

              $('#divfrm').fadeIn('normal');
              $('#btnSave').removeAttr('disabled');
              $('#txt_ID').val(ID);
              $('#txt_GoodID').val(GoodSupplyID);
              $('#txt_WBID').val(WBID);
              $('#txt_QuanlityOriginal').val(Quanlity);
              $('input[name=Quanlity]').val(Quanlity);
          }

          //修改进货数量
          function FunUpdateQuanlity() {
              var ID = $('#txt_ID').val();
              var WBID = $('#txt_WBID').val();
              var GoodSupplyID = $('#txt_GoodID').val();
              var Quanlity = $('input[name=Quanlity]').val();
              var QuanlityOriginal = $('#txt_QuanlityOriginal').val();
              $.ajax({
                  url: '/Ashx/good.ashx?type=Update_GoodSupplyStockWB&ID=' + ID + '&WBID=' + WBID + '&GoodSupplyID=' + GoodSupplyID + '&Quanlity=' + Quanlity + '&QuanlityOriginal=' + QuanlityOriginal,
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      $('#btnSave').attr('disabled', 'disabled');
                     showMsg('修改进货数量成功 ！');

                  }, error: function (r) {
                     showMsg('修改进货数量失败 ！');
                  }
              });
          }


          function frmClose() {
              $('#divfrm').fadeOut();
          }



      </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>社员商品进货列表</b>
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>网点名称:</span>
                    </td>
                    <td>
                        <input type="text" id="txtWBID" onkeyup="InitWBID();" style="font-size: 16px; width: 100px; height: 26px; font-weight: bolder;" runat="server" />
                        <select id="QWBID" runat="server" style="width: 100px; height: 30px;"></select></td>
                    <td><span>日期:</span>
                    </td>
                    <td>
                        <input type="text" id="Qdtstart" readonly="readonly" onclick="WdatePicker();" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                        <span>-</span>
                        <input type="text" id="Qdtend" readonly="readonly" onclick="WdatePicker()" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                    </td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="~/images/search_red.png" OnClick="ImageButton1_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnOutPut" runat="server" class="outputExcel" Text="导出Excel"
                            OnClick="btnOutPut_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 150px; height: 20px; text-align: center;">网点
                        </th>
                        <th style="width: 100px; text-align: center;">商品
                        </th>
                        <th style="width: 100px; text-align: center;">商品价格
                        </th>
                        <th style="width: 100px; text-align: center;">网点进价
                        </th>
                        <th style="width: 100px; text-align: center;">返利金额
                        </th>
                        <th style="width: 100px; text-align: center;">购买数量
                        </th>
                        <th style="width: 100px; text-align: center;">购买金额
                        </th>
                        <th style="width: 100px; text-align: center;">进货日期
                        </th>
                        <th style="width: 100px; text-align: center;">操作
                        </th>

                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;">

                        <%#Eval("WBName")%>
                    </td>
                    <td>
                        <%#Eval("GoodSupplyName")%>
                    </td>
                    <td>￥<%#Eval("Price")%>
                    </td>
                    <td>￥<%#Eval("Price_WB")%>
                    </td>
                    <td>￥<%#(Eval("Price_WBBack"))%>
                    </td>
                    <td>
                        <%#Eval("Quantity")%>
                    </td>
                    <td>￥<%#Eval("Price_Money")%>
                    </td>
                    <td>
                        <%#Eval("dt_Trade")%>
                    </td>
                    <td>
                        <%#GetUpdateItem(Eval("ID"),Eval("GoodSupplyID"),Eval("WBID"),Eval("Quantity") )%>
                     
                    </td>

                </tr>
            </ItemTemplate>
            <FooterTemplate>

                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>

        <div id="divfrm" class="pageEidt" style="display: none;">
             <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="frmClose()" />
            <div style="clear: both;">
                <table class="tabEdit">
                    <tr>
                        <td align="right" style="width: 100px;">
                            <span>进货数量:</span>
                        </td>
                        <td>
                            <input type="text" name="Quanlity" style="width: 200px;" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" style="width: 100px;"></td>
                        <td>
                            <input type="button" id="btnSave" value="确认" onclick="FunUpdateQuanlity();" style="width: 80px;" />
                        </td>
                    </tr>

                </table>
            </div>
        </div>

        <div style="display: none;">
            <input type="text" id="txt_ID" />
            <input type="text" id="txt_GoodID" />
            <input type="text" id="txt_WBID" />
            <input type="text" id="txt_QuanlityOriginal" />
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>

</body>
</html>
