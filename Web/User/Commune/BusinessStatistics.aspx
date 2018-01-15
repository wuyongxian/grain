<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BusinessStatistics.aspx.cs" Inherits="Web.User.Commune.BusinessStatistics" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />  
    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>社员信息统计</b>
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>社员账号:</span></td>
                    <td>
                        <input type="text" id="QAccountNumber" style="font-size: 16px; width: 120px; font-weight: bolder;" runat="server" /></td>
                    <td><span>日期:</span></td>
                    <td>
                        <input type="text" id="Qdtstart" readonly="readonly" onclick="WdatePicker();" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                        <span>-</span>
                        <input type="text" id="Qdtend" readonly="readonly" onclick="WdatePicker()" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" /></td>
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
                        <th style="width: 100px; text-align: center;">商品种类
                        </th>
                        <th style="width: 100px; text-align: center;">商品单价
                        </th>
                        <th style="width: 100px; text-align: center;">折率
                        </th>
                        <th style="width: 100px; text-align: center;">计量单位
                        </th>
                        <th style="width: 100px; text-align: center;">购买数量
                        </th>
                        <th style="width: 100px; text-align: center;">商品总价
                        </th>
                        <th style="width: 100px; text-align: center;">优惠金额
                        </th>
                        <th style="width: 100px; text-align: center;">预付款金额
                        </th>
                        <th style="width: 100px; text-align: center;">实付金额
                        </th>
                        <th style="width: 100px; text-align: center;">返利金额
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;">

                        <%#Eval("WBID")%>
                    </td>
                    <td>
                        <%#Eval("GoodSupplyID")%>
                    </td>
                    <td>
                        <%#Eval("Price")%>
                    </td>
                    <td>
                        <%#Eval("numDisCount")%>%
                    </td>
                    <td>
                        <%#Eval("UnitName")%>
                    </td>
                    <td>
                        <%#Eval("GoodSupplyCount")%>
                    </td>
                    <td>
                        <%#Eval("Money_Total")%>
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
                            <%#Eval("Money_Back")%>
                        </td>

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
