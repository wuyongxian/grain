<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOutHistory.aspx.cs" Inherits="Web.Settle.CheckOutHistory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <link href="../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   
    <script src="../Scripts/My97DatePicker/calendar.js" type="text/javascript"></script>
    <script src="../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>
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

          

          function GetCunZheInfo(ID) {

              $.ajax({
                  url: '/Ashx/settle.ashx?type=GetCheckOut_LogByCheckOut_ID&ID=' + ID,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      var AccountNumber = r[0].AccountNumber;
                      var BusinessNO = r[0].BusinessNO;

                      PrintCunZhe(AccountNumber, BusinessNO);
                  }, error: function (r) {
                     showMsg('未能找到打印存折信息 ！');
                  }
              });
          }


          function PrintCunZhe(AccountNumber, BusinessNO) {

              $.ajax({
                  url: '/Ashx/settle.ashx?type=PrintSA_CheckOutLog&AccountNumber=' + AccountNumber + '&BusinessNO=' + BusinessNO,
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {

                      $('#divPrint').html('');
                      $('#divPrint').append(r);
                      CreateOneFormPage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                     showMsg('打印存折时出现错误 ！');
                  }
              });
          }




          function PrintPage(ID) {
             
              $.ajax({
                  url: '/Ashx/settle.ashx?type=PrintCheckOut&ID=' + ID,
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {

                      $('#divPrintPaper').html('');
                      $('#divPrintPaper').append(r);
                      CreatePage();
                      LODOP.PREVIEW(); //打印存折
                  }, error: function (r) {
                     showMsg('加载打印坐标时出现错误 ！');
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

              LODOP.ADD_PRINT_HTM(20, 60, 800, 400, document.getElementById("divPrintPaper").innerHTML);

          };



      </script>
</head>
<body>
    <div id="divPrint" style="display: none">
    </div>
    <div id="divPrintPaper" style="display: none">
    </div>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>原粮出库历史记录</b>
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>网点名称:</span>
                    </td>
                    <td>
                        <input type="text" id="txtWBID" onkeyup="InitWBID();" style="font-size: 16px; width: 100px;height:26px; font-weight: bolder;" runat="server" />
                        <select id="QWBID" runat="server" style="width: 100px;height:30px;"></select></td>

                    <td style="width: 60px">

                        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                            runat="server" OnClick="ImageButton1_Click" />
                    </td>
                     <td><asp:Button ID="btnOutPut" runat="server" class="outputExcel" Text="Excel"
                        style="width:50px;margin-left:20px;"    OnClick="btnOutPut_Click" /></td>
                </tr>
            </table>
        </div>
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 100px; height: 20px; text-align: center;">网点名称
                        </th>
                        <th style="width: 100px; text-align: center;">网点账号
                        </th>
                        <th style="width: 100px; text-align: center;">产品名称
                        </th>
                        <th style="width: 80px; text-align: center;">毛重
                        </th>
                        <th style="width: 80px; text-align: center;">皮重
                        </th>
                        <th style="width: 80px; text-align: center;">净重
                        </th>
                        <th style="width: 80px; text-align: center;">出库实重
                        </th>
                        <th style="width: 100px; text-align: center;">出库时间
                        </th>
                        <th style="width: 80px; text-align: center;">补打存折
                        </th>
                        <th style="width: 80px; text-align: center;">补打小票
                        </th>
                        <th style="width: 60px; text-align: center;">修改
                        </th>

                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td align="center" style="height: 25px;">

                        <%#Eval("WBName")%>
                     
                    </td>
                    <td align="center">
                        <%#Eval("SA_AN")%>
                    </td>
                    <td align="center">
                        <%#Eval("Variety_Name")%> 
               
                    </td>
                    <td align="center">
                        <%#Eval("Weight_Mao")%>
                    </td>
                    <td align="center">
                        <%#Eval("Weight_Pi")%>
                    </td>
                    <td align="center">
                        <%#Eval("Weight_Jing")%>
                    </td>
                    <td align="center">
                        <%#Eval("Weight_Reality")%>
                    </td>
                    <td align="center">
                        <%#Eval("dt_Trade")%>
                    </td>
                    <td align="center">
                        <a href="#" onclick="GetCunZheInfo(<%#Eval("ID") %>)">补打存折</a>
                    </td>
                    <td align="center">
                        <a href="#" onclick="PrintPage(<%#Eval("ID") %>)">补打小票</a>
                    </td>
                    <td align="center">
                        <a href="CheckOutEdit.aspx?ID=<%#Eval("ID") %>">修改</a>
                    </td>

                </tr>
            </ItemTemplate>
            <FooterTemplate>

                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>

         <asp:Repeater ID="Repeater2" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width:600px;height: 20px; text-align: center;" colspan="7">总计
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td align="center" style="width:100px;height: 25px;">

                        <%#Eval("WBName")%>
                     
                    </td>
                    <td align="center" style="width:100px;height: 25px;">
                       
                    </td>
                    <td align="center" style="width:100px;height: 25px;">
                        <%#Eval("Variety_Name")%> 
               
                    </td>
                    <td align="center" style="width:80px;height: 25px;">
                        <%#Eval("Weight_Mao")%>
                    </td>
                    <td align="center" style="width:80px;height: 25px;">
                        <%#Eval("Weight_Pi")%>
                    </td>
                    <td align="center" style="width:80px;height: 25px;">
                        <%#Eval("Weight_Jing")%>
                    </td>
                    <td align="center" style="width:80px;height: 25px;">
                        <%#Eval("Weight_Reality")%>
                    </td>
                 
                </tr>
            </ItemTemplate>
            <FooterTemplate>

                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
        <div style="display: none;">
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>

</body>
</html>
