<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryGoodStock.aspx.cs" Inherits="Web.Admin.Good.QueryGoodStock" %>
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


        function ShowUpdateQuantity(GoodStorageID, numStore) {
            //console.log('GoodStorageID:' + GoodStorageID);
            $('input[name=Quantity]').val(numStore);
            $('input[name=GoodStorageID]').val(GoodStorageID);
            showBodyCenter($('#divfrm'));
            //$('#divfrm').fadeIn();
        }

        function FunUpdateQuantity() {
            var Quantity = $('input[name=Quantity]').val();
            if (!CheckNumInt(Quantity, '库存数量', '0', '-1')) {
                return false;
            }
            var GoodStorageID = $('input[name=GoodStorageID]').val();
            var para = 'GoodStorageID=' + GoodStorageID + '&Quantity=' + Quantity;
            /*--------数据提交--------*/
            $.ajax({
                url: '/Ashx/good.ashx?type=UpdateGoodQuantity',
                type: 'post',
                data: para,
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
            <b>网点库存查询</b>
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>网点名称:</span>
                    </td>
                    <td>
                        <input type="text" id="txtWBID" onkeyup="InitWBID();" style="font-size: 16px; width: 100px;height:26px; font-weight: bolder;" runat="server" />
                        <select id="QWBID" runat="server" style="width: 100px;height:30px;"></select></td>
                    <td><span>商品名称:</span></td>
                    <td>
                        <input type="text" id="txtstrName" style="width:120px;" runat="server" />
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
                        <th style="width: 200px; text-align: center;">商品名称
                        </th>
                        <th style="width: 100px; text-align: center;">规格型号
                        </th>
                        <th style="width: 100px; text-align: center;">进货价格
                        </th>
                        <th style="width: 100px; text-align: center;">兑换价格
                        </th>
                        <th style="width: 100px; text-align: center;">销售价格
                        </th>
                        <th style="width: 100px; text-align: center;">批发价格
                        </th>
                        <th style="width: 100px; text-align: center;">仓库
                        </th>
                        <th style="width: 100px; text-align: center;">库存
                        </th>
                        <th style="width: 100px; text-align: center;">操作
                        </th>

                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height: 25px;">
                        <%#Eval("strName")%>
                    </td>
                    <td>
                        <%#Eval("PackingSpecID")%>
                    </td>
                    <td>
                        <%#Eval("Price_Stock")%>
                    </td>
                    <td>
                        <%#Eval("Price_DuiHuan")%>
                    </td>
                    <td>
                        <%#Eval("Price_XiaoShou")%>
                    </td>
                    <td>
                        <%#Eval("Price_PiFa")%>
                    </td>
                    <td>
                        <%#( Eval("WBWareHouseName"))%>
                    </td>
                    <td>
                        <%#( Eval("numStore"))%>
                    </td>
                    <td>
                        <%--  <%#GetUpdateItem(Eval("GoodStorageID"))%>--%>
                        <a href="#" style="width:80px;height:25px;" onclick="ShowUpdateQuantity( <%#( Eval("GoodStorageID"))%>,<%#( Eval("numStore"))%>)">修改库存</a>
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
            NumericButtonTextFormatString="[{0}]" PageSize="20"
            OnPageChanging="AspNetPager1_PageChanging">
        </webdiyer:AspNetPager>

        <div id="divfrm" class="pageEidt" style="display: none;">
            <div style="color: Blue; font-weight: bolder;">
                [修改网点商品库存]
                  <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="frmClose()" />
            </div>
            <div style="clear: both;">
                <table>
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

    <div style="display: none">
        <%--存储编号--%>
        <input type="text" name="GoodStorageID" value="" />

        <%--定义背景色的隐藏域--%>
        <input type="hidden" id="colorName" value="" />
    </div>

</body>
</html>
