<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QByCun.aspx.cs" Inherits="Web.Admin.Query.QByCun" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>储户信息查询</b>
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>村名:</span>
                    </td>
                    <td>
                        <input type="text" id="QCunName" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" /></td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                            runat="server" OnClick="ImageButton1_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 120px; height: 20px; text-align: center;">网点
                        </th>
                        <th style="width: 100px; text-align: center;">储户账号
                        </th>
                        <th style="width: 100px; text-align: center;">储户姓名
                        </th>
                        <th style="width: 100px; text-align: center;">村名
                        </th>
                        <th style="width: 100px; text-align: center;">存入日期
                        </th>
                        <th style="width: 80px; text-align: center;">存储类型
                        </th>
                        <th style="width: 80px; text-align: center;">存期
                        </th>
                        <th style="width: 80px; text-align: center;">存储产品
                        </th>

                        <th style="width: 100px; text-align: center;">当前结存
                        </th>

                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;">
                        <%#Eval("WBID") %>
                    </td>
                    <td>
                        <a href="/User/Query/DepositorInfo.aspx?Account=<%#Eval("AccountNumber") %>"><%#Eval("AccountNumber") %></a>
                    </td>
                    <td>
                        <%#Eval("strName")%>
                    </td>
                    <td>
                        <%#Eval("CunID")%>
                    </td>
                    <td>
                        <%#Eval("StorageDate")%>
                    </td>
                    <td>
                        <%#Eval("TypeID")%>
                    </td>
                    <td>
                        <%#(Eval("TimeID"))%>
                    </td>
                    <td>
                        <%#Eval("VarietyID")%>
                    </td>
                    <td>
                        <%#Eval("StorageNumber")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>

                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>

        <webdiyer:AspNetPager ID="AspNetPager1" runat="server"
            FirstPageText="首页" LastPageText="尾页" PrevPageText="上一页" NextPageText="下一页"
            NumericButtonTextFormatString="[{0}]" PageSize="15" OnPageChanging="AspNetPager1_PageChanging">
        </webdiyer:AspNetPager>

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