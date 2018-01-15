<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuerySupplyStock.aspx.cs" Inherits="Web.Admin.Good.QuerySupplyStock" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    
   
    <style type="text/css">
        
    </style>
    <script type="text/javascript">
        /*--------窗体启动设置和基本设置--------*/

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


        function ShowUpdateQuantity(WBID, GoodSupplyID, Quantity) {
            $('#WBID').val(WBID);
            $('#GoodSupplyID').val(GoodSupplyID);
            showBodyCenter($('#divfrm'));
            $('input[name=Quantity]').val(Quantity);
          
        }


        function FunUpdateQuantity() {
            var Quantity = $('input[name=Quantity]').val();
            if (!CheckNumInt(Quantity, '库存数量', '0', '-1')) {
                return false;
            }
            var WBID = $('#WBID').val();
            var GoodSupplyID = $('#GoodSupplyID').val();
            /*--------数据提交--------*/
            $.ajax({
                url: '/Ashx/good.ashx?type=UpdateGoodSupplyQuantity&WBID=' + WBID + '&GoodSupplyID=' + GoodSupplyID + '&Quantity=' + Quantity,
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                    if (r == "OK") {
                       showMsg('修改库存数量成功！');

                    } else {
                       showMsg('修改库存数量失败 ！');
                    }
                }, error: function (r) {
                   showMsg('修改库存数量失败 ！');
                }
            });
            /*--------End 数据提交--------*/
        }


        function frmClose() {
            $('#divfrm').hide();
        }



    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>社员商品库存查询</b><span id="spanID"></span>
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>网点名称:</span>
                    </td>
                    <td>
                        <input type="text" id="txtWBID" onkeyup="InitWBID();" style="font-size: 16px; width: 100px; height:26px; font-weight: bolder;" runat="server" />
                        <select id="QWBID" runat="server" style="width: 100px;height:30px;"></select></td>
                    <td><span>商品名称:</span>
                    </td>
                    <td>
                        <input type="text" id="txtstrName" style="width: 120px;" runat="server" />
                    </td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="~/images/search_red.png" OnClick="ImageButton1_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 120px; text-align: center;">商品名称
                        </th>
                        <th style="width: 100px; text-align: center;">规格型号
                        </th>

                        <th style="width: 120px; text-align: center;">商品单价
                        </th>
                        <th style="width: 120px; text-align: center;">网点购买价
                        </th>
                        <th style="width: 120px; text-align: center;">返利金额
                        </th>
                        <th style="width: 120px; text-align: center;">社员优惠价
                        </th>
                        <th style="width: 80px; text-align: center;">商品折率
                        </th>
                        <th style="width: 80px; text-align: center;">预定
                        </th>
                        <th style="width: 80px; text-align: center;">库存信息
                        </th>
                        <th style="width: 120px; text-align: center;">库存金额(元)
                        </th>
                        <th style="width: 120px; text-align: center;">返利金额(元)
                        </th>
                        <th style="width: 100px; text-align: center;">操作
                        </th>

                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td>
                        <%#Eval("strName")%>
                    </td>
                    <td>
                        <%#Eval("SpecID")%>
                    </td>

                    <td>
                        <%#Eval("Price")%>
                    </td>
                    <td>
                        <%#Eval("Price_WB")%>
                    </td>
                    <td>
                        <%#Eval("Price_WBBack")%>
                    </td>
                    <td>
                        <%#Eval("Price_Commune")%>
                    </td>
                    <td>
                        <%#Eval("numDiscount")%>
                    </td>
                    <td>
                        <%#Eval("ISPreDefine")%>
                    </td>

                    <td>
                        <%#GetGoodStorage( Eval("ID"))%>
                    </td>
                    <td>
                        <%#KunCunJinE(Eval("ID"), Eval("numPrice"))%>
                    </td>
                    <td>
                        <%#FanLiJinE(Eval("ID"), Eval("numPrice_WBBack"))%>
                    </td>
                    <td>
                        <%#GetUpdateItem(Eval("ID"))%>
                   
                    </td>


                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr>
                    <td colspan="9" style="text-align: right" align="right">统计</td>
                    <td><%=Math.Round(KuCunJinET,2) %></td>
                    <td><%=Math.Round(FanLiJineET,2) %></td>
                    <td></td>
                </tr>
                <!--底部模板-->
                </table>
            <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server"
            FirstPageText="首页" LastPageText="尾页" PrevPageText="上一页" NextPageText="下一页"
            NumericButtonTextFormatString="[{0}]" PageSize="15"
            OnPageChanging="AspNetPager1_PageChanging">
        </webdiyer:AspNetPager>

        <div id="divfrm" class="pageEidt" style="display: none;">
            <div style="color: Blue; font-weight: bolder;">
                [修改网点商品库存]
                 <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="frmClose()" />
            </div>
            <div style="clear: both;">
                <table class="tabEdit">
                    <tr>
                        <td align="right" style="width: 100px;">
                            <span></span>
                        </td>
                        <td>
                            <span style="color: Red; font-weight: bolder;">请输入该商品的实际库存数量</span>
                        </td>
                    </tr>

                    <tr>
                        <td align="right">
                            <span>库存数量:</span>
                        </td>
                        <td>
                            <input type="text" name="Quantity" style="width: 200px;" />
                        </td>
                    </tr>

                    <tr>
                        <td></td>
                        <td>

                            <input type="button" id="btnUpdate" value="修改" onclick="FunUpdateQuantity()" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

    </form>

    <div style="display: none;">
        <%--商品存储信息编号--%>
        <input type="hidden" id="GoodSupplyID" value="" />
        <%--定义编号--%>
        <input type="hidden" id="WBID" value="" />
        <%--定义背景色的隐藏域--%>
        <input type="hidden" id="colorName" value="" />
    </div>

</body>
</html>