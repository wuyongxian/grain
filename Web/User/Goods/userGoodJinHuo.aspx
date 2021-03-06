﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userGoodJinHuo.aspx.cs" Inherits="Web.User.Goods.userGoodJinHuo" %>

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
            //首次加载商品列表
            var strName = $('input[name=GoodQ]').val();
            $.ajax({
                url: '/Ashx/good.ashx?type=Get_Good&strName=' + strName,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('select[name=GoodID]').empty();
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=GoodID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                    var GoodID = getQueryString("GoodID"); //是否有商品名ID
                    if (GoodID != "") {
                        $('select[name=GoodID]').val(GoodID);
                    }
                    LoadGoodInfo($('select[name=GoodID] option:selected').val());

                }, error: function (r) {
                   showMsg('不存在的商品类型 ！');
                }
            });


            $('select[name=GoodID]').change(function () {
                LoadGoodInfo($('select[name=GoodID] option:selected').val());
            });

            //  $('input[name=Price_Stock]').val('0');
            $('input[name=Quantity]').val('0');
            $('#spanSum').html('0');
            var date = new Date();
            var strdate = date.getFullYear() + '-' + parseInt(date.getMonth() + 1) + '-' + date.getDay();
            $('input[name=dt_JinHuo]').val(strdate);
        });

        function LoadGoodInfo(GoodID) {
            $('input[name=GoodIDSelect]').val(GoodID);
            $.ajax({
                url: '/Ashx/good.ashx?type=GetGoodByWBID&GoodID=' + GoodID,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('#btnStock').removeAttr('disabled');
                    $('#spanGood').html(r[0].strName);
                    $('input[name=Price_Stock]').val(r[0].Price_Stock);
                    $('#WBSupplierID').html(r[0].WBSupplierID);
                    $('#PackingSpecID').html(r[0].PackingSpecID);
                    $('#MeasuringUnit').html(r[0].MeasuringUnit);
                    $('#WBWareHouseID').html(r[0].WBWareHouseID);
                    $('#spanUnit').html(r[0].MeasuringUnit);
                    $('#spanUnit2').html(r[0].MeasuringUnit);
                }, error: function (r) {
                    if (r.responseText == "Error") {
                        $('#btnStock').attr("disabled", "disabled");
                       showMsg('请添加产品的仓储信息后再进货 ！');

                    }
                    else {
                       showMsg('加载信息失败 ！');
                    }
                }
            });
        }

        function calcSum() {
            //计算总价格

            if (!CheckNumDecimal($('input[name=Price_Stock]').val(), '进货价格', 2)) {
                return false;
            }
            if (!CheckNumInt($('input[name=Quantity]').val(), '进货数量', '-1', '-1')) {
                return false;
            }
            var price = parseFloat($('input[name=Price_Stock]').val());
            var Quantity = parseFloat($('input[name=Quantity]').val());
            var moneysum = price * Quantity;
                
            $('#spanSum').html(moneysum.toFixed(2));

        }

        /*--------End  窗体启动设置和基本设置--------*/
        //当前当前系统中有没有此商品
        function GoodQuery() {
            var strName = $('input[name=GoodQ]').val();
            $.ajax({
                url: '/Ashx/good.ashx?type=Get_Good&strName=' + strName,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('select[name=GoodID]').empty();
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=GoodID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                    LoadGoodInfo($('select[name=GoodID] option:selected').val());
                }, error: function (r) {
                   showMsg('不存在的商品信息 ！');
                }
            });
        }

        function FunStock() {

            if (!CheckNumDecimal($('input[name=Price_Stock]').val(), '进货价格', 2)) {
                return false;
            }
            if (!CheckNumInt($('input[name=Quantity]').val(), '进货数量', '-1', '-1')) {
                return false;
            }

            $.ajax({
                url: '/Ashx/good.ashx?type=GetHQStorage_Dep&GoodID=' + $('select[name=GoodID] option:selected').val(),
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                    var Quantity = $('input[name=Quantity]').val();
                    if (parseFloat(Quantity) > parseFloat(r)) {
                       showMsg('总部库存不足，无法出货 ！');
                    }
                    else {

                        /*----网店进货----*/
                        $.ajax({
                            url: '/Ashx/good.ashx?type=Add_GoodStockWB',
                            type: 'post',
                            data: $('#form1').serialize(),
                            dataType: 'text',
                            success: function (r) {
                               showMsg('添加进货信息成功 ！');
                                $('#btnStock').attr("disabled", "disabled");
                            }, error: function (r) {
                               showMsg('添加进货信息失败 ！');
                            }
                        });
                        /*----End 网店进货----*/

                    }
                }, error: function (r) {
                   showMsg('检索总部库存失败 ！');
                }
            });

         
        }




    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
        <b>仓库进货</b><span id="spanHelp" style="cursor: pointer">帮助</span>
    </div>
     <div id="divHelp"  class="pageHelp">
        <span>提示1：商品列表最多只列出20种商品，如果商品不在列表中，可以按商品名查询。找到商品后，填写相关进货数据</span><br />

    </div>
   
    <div id="divfrm" class="pageEidtInner" >
      <div style="clear: both;">
            <table>
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>商品搜索:</span>
                    </td>
                    <td>
                        <input type="text" name="GoodQ" style="width: 100px;" />
                        <input  type="button"  id="btnQ" value="查询" onclick="GoodQuery()" style="width:60px;"  />
                        <select name="GoodID" style="width:100px"></select>
                    </td>
                </tr>
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>已选择商品:</span>
                    </td>
                    <td>
                     <span style=" font-weight:bolder; color:Blue" id="spanGood"></span>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>供应商名:</span>
                    </td>
                    <td>
                    <span style="font-weight:bolder;" id="WBSupplierID"></span>
                      
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>规格型号:</span>
                    </td>
                    <td>
                     <span style="font-weight:bolder;" id="PackingSpecID"></span> 
                         <span>计量单位:</span>
                          <span style="font-weight:bolder;" id="MeasuringUnit"></span>
                         
                    </td>
                </tr>
               <tr>
                    <td align="right">
                        <span>进货日期:</span>
                    </td>
                    <td>
                       <input type="text" name="dt_JinHuo" disabled="disabled" style="width:100px;" />
                         <span>入库仓号:</span>
                          <span style="font-weight:bolder;" id="WBWareHouseID"></span>
                        
                    </td>
                </tr>
             
                <tr>
                    <td align="right">
                        <span>商品进价:</span>
                    </td>
                    <td>
                        <input name="Price_Stock" type="text" onkeyup="calcSum()" style="width: 100px;" />
                        <span>元/</span>
                      <span id="spanUnit"></span>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>进货数量:</span>
                    </td>
                    <td>
                        <input name="Quantity" type="text" onkeyup="calcSum()" style="width: 100px;" />
                     
                      <span id="spanUnit2"></span>
                    </td>
                </tr>
                    <tr>
                    <td align="right">
                        <span>交易额:</span>
                    </td>
                    <td>
                      <span id="spanSum" style="font-weight:bolder; color:Green"></span><span>元</span>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>现金交易:</span>
                    </td>
                    <td>
                    <input  type="checkbox" name="ISCash" /><span style="color:Red">请谨慎选择</span>
                    </td>
                </tr>
               
                   
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="button" id="btnStock" value="确认进货" onclick="FunStock()" />
                    </td>
                </tr>
               
            </table>
        </div>
    </div>
    <div style="display:none">
    <input type="text"  name="GoodIDSelect" />
    </div>
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>

