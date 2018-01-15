<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodJinHuoListHQ.aspx.cs" Inherits="Web.Admin.Good.GoodJinHuoListHQ" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
     
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>总部进货记录</b>
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>日期:</span>
                    </td>
                    <td>
                       <input type="text" id="Qdtstart" readonly="readonly" onclick="WdatePicker();" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                        <span>-</span>
                      <input type="text" id="Qdtend" readonly="readonly" onclick="WdatePicker()" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                    </td>
                    <td><span>商品名称:</span></td>
                    <td>
                        <input type="text" id="txtstrName" style="width:120px;" runat="server" />
                    </td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                            runat="server" OnClick="ImageButton1_Click" />
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
                        <th style="min-width: 100px; height: 20px; text-align: center;">网点
                        </th>
                        <th style="min-width: 100px; text-align: center;">商品
                        </th>

                        <th style="min-width: 60px; text-align: center;">商品规格
                        </th>
                        <th style="min-width: 60px; text-align: center;">计量单位
                        </th>
                        <th style="min-width: 80px; text-align: center;">商品价格
                        </th>
                        <th style="min-width: 60px; text-align: center;">购买数量
                        </th>
                        <th style="min-width: 100px; text-align: center;">供应商
                        </th>
                        <th style="min-width: 100px; text-align: center;">入库仓库
                        </th>
                        <th style="min-width: 80px; text-align: center;">购买日期
                        </th>


                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;">

                        <%#Eval("WBName")%>
                    </td>
                    <td>
                        <%#Eval("GoodName")%>
                    </td>
                    <td>
                        <%#Eval("SpecID")%>
                    </td>
                    <td>
                        <%#Eval("UnitID")%>
                    </td>
                    <td>￥<%#(Eval("Price_Stock"))%>
                    </td>
                    <td>
                        <%#Eval("Quantity")%>
                    </td>
                    <td>
                        <%#Eval("WBSupplierName")%>
                    </td>
                    <td>
                        <%#Eval("WBWareHouseName")%>
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