<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepositorInfo.aspx.cs" Inherits="Web.User.Query.DepositorInfo" %>

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

          });
          /*--ENd loadFuntion--*/

      </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <a href="#" onclick="javascript :history.back(-1);"><img src="../../images/icon-back.png" /></a>
            <b>储户基本信息查询</b><%--<span id="spanHelp" style="cursor: pointer">帮助</span>--%>
        </div>
        <div id="divHelp" style="border: 1px solid #333; border-radius: 5px; display: none;">
            <span>提示1：</span><br />
            <span>提示2：</span><br />
            <span>提示3：</span><br />
        </div>
         <div class="QueryHead">
            <table>
                <tr>
                    <td> <span>储户账号:</span></td>
                    <td><span>
                         <input type="text" id="QAccountNumber"  runat="server" />
                    </span></td>
                    <td style="width: 60px">
                         <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                runat="server" OnClick="ImageButton1_Click" />
                    </td>
                </tr>

            </table>
        </div>

        <div id="depositorInfo" runat="server" style="display: none;margin: 10px 0px;">
            <table class="tabData" style=" max-width: 750px;">
                <thead>

                    <tr>
                        <td colspan="6">基本信息
                        </td>
                    </tr>
                </thead>
                <tr class="tr_head">
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
                    <table class="tabData" style="width: 750px;">
                        <thead>
                            <tr>
                                <td colspan="8" align="center">存储信息</td>
                            </tr>
                        </thead>

                        <tr class="tr_head">
                            <th style="width: 120px; height: 20px; text-align: center;">存贷产品
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
                            <th style="width: 120px; text-align: center;">利息
                            </th>

                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
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
                            <%#Eval("strLixi")%>
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

        <div id="exchangeList" runat="server" style="margin: 10px 0px;">

            <asp:Repeater ID="R_exchange" runat="server">
                <HeaderTemplate>
                    <table class="tabData"  style="max-width:750px;">
                        <thead>
                            <tr>
                                <td colspan="7" align="center">兑换信息</td>
                            </tr>
                        </thead>

                        <tr class="tr_head">
                            <th style="width: 100px; height: 20px; text-align: center;">业务名称
                            </th>
                            <th style="width: 150px; text-align: center;">品名
                            </th>
                            <th style="width: 100px; text-align: center;">单价
                            </th>
                            <th style="width: 100px; text-align: center;">数量
                            </th>
                            <th style="width: 100px; text-align: center;">计价单位
                            </th>
                            <th style="width: 100px; text-align: center;">折合原粮
                            </th>
                            <th style="width: 100px; text-align: center;">收费
                            </th>

                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                        <td style="height: 25px;">
                            <%#Eval("ISReturn")%>
                        </td>
                        <td>
                            <%#Eval("GoodName")%>
                        </td>
                        <td>
                            <%#Eval("GoodPrice")%>
                        </td>
                        <td>
                            <%#Eval("GoodCount")%>
                        </td>
                        <td>
                            <%#Eval("UnitName")%>
                        </td>
                        <td>
                            <%#Eval("VarietyCount")%>
                        </td>
                        <td>
                            <%#Eval("Money_DuiHuan")%>
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

        <div id="SellList" runat="server" style="margin: 10px 0px;">
            <asp:Repeater ID="R_Sell" runat="server">
                <HeaderTemplate>
                    <table class="tabData" style="max-width:750px;">
                        <thead>
                            <tr>
                                <td colspan="7" align="center">存转销信息</td>
                            </tr>
                        </thead>

                        <tr class="tr_head">
                            <th style="width: 100px; height: 20px; text-align: center;">结算类型
                            </th>
                            <th style="width: 100px; text-align: center;">结算日期
                            </th>
                            <th style="width: 150px; text-align: center;">存储产品
                            </th>
                            <th style="width: 100px; text-align: center;">实存天数
                            </th>
                            <th style="width: 100px; text-align: center;">结算重量
                            </th>
                            <th style="width: 100px; text-align: center;">利息
                            </th>
                            <th style="width: 100px; text-align: center;">结算金额
                            </th>

                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                        <td style="height: 25px;">
                            <%#Eval("ISReturn")%>
                        </td>
                        <td>
                            <%#Eval("dt_Sell")%>
                        </td>
                        <td>
                            <%#Eval("VarietyName")%>
                        </td>
                        <td>
                            <%#Eval("StorageDate")%>
                        </td>
                        <td>
                            <%#Eval("VarietyCount")%>
                        </td>
                        <td>
                            <%#Eval("VarietyInterest")%>
                        </td>
                        <td>
                            <%#Eval("VarietyMoney")%>
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

          <div id="ShoppingList" runat="server" style="margin: 10px 0px;">
            <asp:Repeater ID="R_Shopping" runat="server">
                <HeaderTemplate>
                    <table class="tabData" style="max-width:750px;">
                        <thead>
                            <tr>
                                <td colspan="7" align="center">换购信息</td>
                            </tr>
                        </thead>

                        <tr class="tr_head">
                            <th style="width: 100px; height: 20px; text-align: center;">结算类型
                            </th>
                            <th style="width: 100px; text-align: center;">结算日期
                            </th>
                            <th style="width: 150px; text-align: center;">存储产品
                            </th>
                            <th style="width: 100px; text-align: center;">实存天数
                            </th>
                            <th style="width: 100px; text-align: center;">结算重量
                            </th>
                            <th style="width: 100px; text-align: center;">利息
                            </th>
                            <th style="width: 100px; text-align: center;">结算金额
                            </th>

                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                        <td style="height: 25px;">
                            <%#Eval("ISReturn")%>
                        </td>
                        <td>
                            <%#Eval("dt_Sell")%>
                        </td>
                        <td>
                            <%#Eval("VarietyName")%>
                        </td>
                        <td>
                            <%#Eval("StorageDate")%>
                        </td>
                        <td>
                            <%#Eval("VarietyCount")%>
                        </td>
                        <td>
                            <%#Eval("VarietyInterest")%>
                        </td>
                        <td>
                            <%#Eval("VarietyMoney")%>
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

        <div style="display: none;">
            <%--选择兑换的存储产品信息--%>
            <input type="text" name="txtDep_SID" value="" />
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>


</body>
</html>
