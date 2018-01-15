<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Custom.aspx.cs" Inherits="Web.Admin.Good.Custom" %>

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

            var code=getQueryString("code");
            if(code.indexOf('NY')!=-1){
                code=="NY";
            }else if(code.indexOf('HF')!=-1){
                code=="HF";
            }if(code.indexOf('ZHZ')!=-1){
                code=="ZHZ";
            }

            InitSelect('/Ashx/good.ashx?type=Get_WBGoodCategoryByCode&code='+code, 'CategoryID','加载商品分类信息失败！');   //商品分类
            InitSelect('/Ashx/good.ashx?type=Get_WBSupplier', 'WBSupplierID','加载供应商信息失败！'); //供应商
            InitSelect('/Ashx/good.ashx?type=Get_WBWareHouse', 'WBWareHouseID','加载网点仓库信息失败！'); //仓库名称
            InitSelect('/Ashx/good.ashx?type=Get_BD_PackingSpec', 'PackingSpecID','加载产品规格信息失败！'); //规格

            //计价单位
            $.ajax({
                url: '/Ashx/good.ashx?type=Get_BD_MeasuringUnit',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                 
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=MeasuringUnit]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                    $('#spanUnit1').html(r[0].strName);
                    $('#spanUnit2').html(r[0].strName);
                    $('#spanUnit3').html(r[0].strName);
                    $('#spanUnit4').html(r[0].strName);
                    $('#spanUnit5').html(r[0].strName);
                    $('#spanUnit6').html(r[0].strName);
                }, error: function (r) {
                    showMsg('加载信息失败！');
                }
            });

            $('select[name=MeasuringUnit]').change(function(){
                $('.spanUnit').html($('select[name=MeasuringUnit] option:selected').text());
            })
        });
      



        function goodfrmClose() {
            $('#divStorage').fadeOut('normal');
            $('#divfrm').fadeOut('normal');
        };


        /*--------End  窗体启动设置和基本设置--------*/


        /*--------自定义div窗口的开启和关闭--------*/
        //显示新增数据窗口
        function ShowFrm_Add(wbid) {
            showBodyCenter($('#divfrm'));
            $('#divStorage').fadeOut("fast");
            $('#trAdd').fadeIn("fast");
            $('#trUpdate').fadeOut("fast");

            //            $('input[name=strName]').removeAttr('disabled');
            $('input[name=strName]').val('');
            $('input[name=BarCode]').val('');
            
            $('select[name=CategoryID]').get(0).selectIndex = 0;
            $('select[name=PackingSpecID]').get(0).selectIndex = 0;
            $('select[name=MeasuringUnit]').get(0).selectIndex = 0;
            $('input[name=Price_Stock]').val('0');
            $('input[name=Price_XiaoShou]').val('0');
            $('input[name=Price_DuiHuan]').val('0');
            $('input[name=Price_PiFa]').val('0');
            $('input[name=Price_VIP]').val('0');
            $('input[name=Price_TeJia]').val('0');
            $('input[name=numExchangeLimit]').val('0');
            $('input[name=PiFaCount_Start]').val('1000');

        }
        //显示更新数据窗口
        function ShowFrm_Update(wbid) {
            showBodyCenter($('#divfrm'));
            $('#divStorage').fadeOut("fast");
            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");
            $('#WBID').val(wbid);
            /*----数据提交----*/
            $.ajax({
                url: '/Ashx/good.ashx?type=GetByID_Good&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                  
                    //                    $('input[name=strName]').attr('disabled', 'disabled');
                    $('input[name=strName]').val(r[0].strName);
                    $('input[name=BarCode]').val(r[0].BarCode);

                    $('select[name=CategoryID]').val(r[0].CategoryID);
                    $('select[name=PackingSpecID]').val(r[0].PackingSpecID);
                    $('select[name=MeasuringUnit]').val(r[0].MeasuringUnit);
                    $('.spanUnit').html($('select[name=MeasuringUnit] option:selected').text());
                    $('input[name=Price_Stock]').val(r[0].Price_Stock);
                    $('input[name=Price_XiaoShou]').val(r[0].Price_XiaoShou);
                    $('input[name=Price_DuiHuan]').val(r[0].Price_DuiHuan);
                    $('input[name=Price_PiFa]').val(r[0].Price_PiFa);
                    $('input[name=Price_VIP]').val(r[0].Price_VIP);
                    $('input[name=Price_TeJia]').val(r[0].Price_TeJia);
                    $('input[name=numExchangeLimit]').val(r[0].numExchangeLimit);
                    $('input[name=PiFaCount_Start]').val(r[0].PiFaCount_Start);
                }, error: function (r) {
                    showMsg('加载信息失败！');
                }
            });
            /*---End 数据提交----*/
        }

        /*
        wbid:Good表ID
        strname：当前商品名称
        */
        function ShowStorage(wbid,strname) {
            $('#divfrm').fadeOut("fast");
            showBodyCenter($('#divStorage'));
            $('#WBID').val(wbid);
            $('input[name=strName2]').val(strname);
            $('input[name=strName2]').attr('disabled', 'disabled');
                       
            /*----数据提交----*/
            $.ajax({
                url: '/Ashx/good.ashx?type=GetGoodStorageByGoodID&GoodID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('#tdStorage').show();
                    $('#GoodStorageID').val(r[0].ID); //加载存储信息编号

                    $('select[name=WBSupplierID]').val(r[0].WBSupplierID);
                    $('select[name=WBWareHouseID]').val(r[0].WBWareHouseID);
                    $('input[name=maxStore]').val(r[0].maxStore);
                    $('#numStore').html(r[0].numStore);
                }, error: function (r) {
                    if (r.responseText == "Error") {
                        $('#tdStorage').hide(); //隐藏进货入口
                        $('select[name=WBSupplierID]').get(0).selectIndex = 0;
                        $('select[name=WBWareHouseID]').get(0).selectIndex = 0;
                        $('input[name=maxStore]').val('0');
                        $('#numStore').html('0');
                    }
                }
            });
            /*---End 数据提交----*/
        }

        //保存仓储信息
        function FunSaveStorage() {
            var strUrl;
            if ($('#GoodStorageID').val() == "") {
                strUrl = '/Ashx/good.ashx?type=Add_GoodStorage&GoodID=' + $('#WBID').val();
            }
            else{
                strUrl = '/Ashx/good.ashx?type=Update_GoodStorage&ID=' + $('#GoodStorageID').val();
            }
            $.ajax({
                url: strUrl,
                type: 'post',
                data: $('#form1').serialize(),
                dataType: 'text',
                success: function (r) {
                    if (r == "OK") {
                        showMsg('操作成功 ！');
                        CloseFrm();
                        location.reload();
                    }
                }, error: function (r) {
                    showMsg('操作失败 ！');
                }
            });
        }

        //打开进货界面
        function OpenJinHuo() {
            //window.location = "/Admin/Good/GoodJinHuo.aspx?GoodID=" + $('#WBID').val();
            window.location = "/Admin/Good/goodjinhuo.html?GoodID=" + $('#WBID').val();
        }

        /*--------End 自定义div窗口的开启和关闭--------*/


        /*--------数据增删改操作--------*/
        //新增数据方法
        function FunAdd() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    //alert('你点击了确定！');
                    $.ajax({
                        url: '/Ashx/good.ashx?type=Add_Good',
                        type: 'post',
                        data: $('#form1').serialize(),
                        dataType: 'text',
                        success: function (r) {
                            if (r == "OK") {
                                showMsg('添加数据成功 ！');
                                CloseFrm();
                                location.reload();
                            } else if (r == "1") {
                                showMsg('已存在相同的类型名称，请修改后添加 ！');
                            }
                        }, error: function (r) {
                            showMsg('添加数据失败 ！');
                        }
                    });
                } else {
                    //console.log('你点击了取消！');
                }
                
            });
        }


        function FunUpdate() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var wbid = $('#WBID').val();
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    //alert('你点击了确定！');
                    $.ajax({
                        url: '/Ashx/good.ashx?type=Update_Good&ID=' + wbid,
                        type: 'post',
                        data: $('#form1').serialize(),
                        dataType: 'text',
                        success: function (r) {
                            if (r == "OK") {
                                showMsg('更新数据成功 ！');
                                CloseFrm();
                                location.reload();
                            } else {
                                showMsg('更新数据失败 ！');
                            }
                        }, error: function (r) {
                            showMsg('更新数据失败 ！');
                        }
                    });
                } else {
                    //console.log('你点击了取消！');
                }
                
            });
        }
      

        //删除数据方法
        function FunDelete(wbid) {
            SingleDataDelete('/Ashx/good.ashx?type=DeleteByID_Good&ID=' + wbid);
           
        }
        //提交检测
        function SubmitCheck() {
            if (!CheckInput('strName', '商品名称', '-1')) {
                return false;
            }

            if ($('input[name=CategoryID] option:selected').val() == '') {
                showMsg('请选择商品分类 ！');
                return false;
            }
            //            if ($('input[name=WBSupplierID] option:selected').val() == '') {
            //               showMsg('请选择供应商名 ！');
            //                return false;
            //            }
            //            if ($('input[name=WBWareHouseID] option:selected').val() == '') {
            //               showMsg('请选择仓库 ！');
            //                return false;
            //            }
            if ($('input[name=PackingSpecID] option:selected').val() == '') {
                showMsg('请选择包装规格 ！');
                return false;
            }
            if ($('input[name=MeasuringUnit] option:selected').val() == '') {
                showMsg('请选择计量单位 ！');
                return false;
            }

            if (!CheckNumDecimal($('input[name=Price_Stock]').val(),'商品进价', 2)) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=Price_XiaoShou]').val(), '销售价格', 2)) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=Price_DuiHuan]').val(), '兑换价格', 2)) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=Price_VIP]').val(), 'VIP价格', 2)) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=Price_PiFa]').val(), '批发价格', 2)) {
                return false;
            }
          
            if (!CheckNumInt($('input[name=PiFaCount_Start]').val(), '起批数量', 1, -1)) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=Price_TeJia]').val(), '商品特价', 2)) {
                return false;
            }
            return true;
        }
       
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
        <b>网点商品管理</b><span id="spanID"></span>
    </div>
   
   <div class="QueryHead">
<table>
            <tr>
            <td><span>按商品名称查询:</span></td>
            <td><span><input type="text" id="txtstrName" runat="server" /> </span></td>
            <td style="width:60px">
                <asp:ImageButton ID="ImageButton1" runat="server" 
                    ImageUrl="~/images/search_red.png" onclick="ImageButton1_Click" />
                </td>
            <td>
           
<%=GetAddItem() %>
            </td>
            </tr>
            
        </table>
</div>
    <asp:Repeater ID="Repeater1" runat="server">
        <HeaderTemplate>
            <table class="tabData">
                <tr class="tr_head">
                    <th style="width: 200px; text-align: center;">
                        商品名称
                    </th>
                    <th style="width: 100px; text-align: center;">
                        规格型号
                    </th>
                     <th style="width: 100px; text-align: center;">
                        进货价格
                    </th>
                    <th style="width: 100px; text-align: center;">
                        兑换价格
                    </th>
                   <%-- <th style="width: 100px; text-align: center;">
                        销售价格
                    </th>
                    <th style="width: 100px; text-align: center;">
                        批发价格
                    </th>--%>
                     <th style="width: 100px; text-align: center;">
                        库存
                    </th>
                    <th style="width: 100px; text-align: center;">
                        兑换限额
                    </th>
                     <th style="width: 120px; text-align: center;">
                        存储信息
                    </th>
                    <th style="width: 120px; text-align: center;">
                        查看/修改
                    </th>
                    <th style="width: 80px; text-align: center;">
                        删除
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr 
                onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                <td>
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
              <%--  <td>
                    <%#Eval("Price_XiaoShou")%>
                </td>
                <td>
                    <%#Eval("Price_PiFa")%>
                </td>--%>
                 <td>
                    <%#GetGoodStorage( Eval("ID"))%>
                </td>
                <td>
                    <%#Eval("numExchangeLimit")%>
                </td>
                <td>
                    <input type="button" style="width:80px; height:25px;" value="存储信息" onclick="ShowStorage(<%#Eval("ID") %>,'<%#Eval("strName") %>    ')" />
                </td>
                <td>
                <%#GetUpdateColumn(Eval("ID"))%>
                 
      <td> <%# GetDeleteColumn(Eval("ID"))%></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <!--底部模板-->
            </table>
            <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>
    <webdiyer:AspNetPager ID="AspNetPager1" runat="server"
      FirstPageText="首页" LastPageText="尾页"  PrevPageText="上一页" NextPageText="下一页" 
        NumericButtonTextFormatString="[{0}]" PageSize="15" 
        onpagechanging="AspNetPager1_PageChanging">
    </webdiyer:AspNetPager>    


    <div id="divfrm" class="pageEidt" style="display: none;">
     <div style="color:Blue; font-weight:bolder;">
          [商品信息添加/修改]
          <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="goodfrmClose()" />
            </div>
        <div style="clear: both;">
            <table class="tabEdit">
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>商品名称:</span>
                    </td>
                    <td>
                        <input type="text" name="strName" style="width: 200px;" /><span style="color: Red;
                            font-weight: bolder;">*</span>
                    </td>
                </tr>
               
                <tr>
                    <td align="right">
                        <span>商品条码:</span>
                    </td>
                    <td>
                        <input type="text" name="BarCode" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>商品分类:</span>
                    </td>
                    <td>
                        <select name="CategoryID" style="width: 200px;">
                        </select>
                    </td>
                </tr>
            
                <tr>
                    <td align="right">
                        <span>包装规格:</span>
                    </td>
                    <td>
                        <select name="PackingSpecID" style="width: 200px;">
                        </select>
                    </td>
                </tr>
                   <tr>
                    <td align="right">
                        <span>计价单位:</span>
                    </td>
                    <td>
                        <select name="MeasuringUnit" style="width: 200px;">
                        </select>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>商品进价:</span>
                    </td>
                    <td>
                        <input name="Price_Stock" type="text"  style="width: 100px;" />
                        <span>元/</span><span class="spanUnit"></span>
                    </td>
                </tr>
                <tr style="display:none;">
                    <td align="right">
                        <span>销售价格:</span>
                    </td>
                    <td>
                        <input type="text" name="Price_XiaoShou" style="width: 100px; " />
                         <span>元/</span><span class="spanUnit"></span>
                    </td>
                </tr>
                 <tr>
                    <td align="right">
                        <span>兑换价格:</span>
                    </td>
                    <td>
                        <input type="text" name="Price_DuiHuan" style="width: 100px; " />
                         <span>元/</span><span class="spanUnit"></span>
                    </td>
                </tr>
                    <tr style="display:none;">
                    <td align="right">
                        <span>VIP价格:</span>
                    </td>
                    <td>
                        <input type="text" name="Price_VIP" style="width: 100px; " />
                         <span>元/</span><span class="spanUnit"></span>
                    </td>
                </tr>
                   <tr style="display:none;">
                    <td align="right">
                        <span>批发价格:</span>
                    </td>
                    <td>
                        <input type="text" name="Price_PiFa" style="width: 100px; " />
                         <span>元/</span><span class="spanUnit"></span>
                    </td>
                </tr>
                    <tr style="display:none;">
                    <td align="right">
                       <span>起批数量:</span>
                    </td>
                    <td>
                          <input type="text" name="PiFaCount_Start" style="width: 100px; " />
                    </td>
                </tr>
               
                   <tr style="display:none;">
                    <td align="right">
                        <span>商品特价:</span>
                    </td>
                    <td>
                        <input type="text" name="Price_TeJia" style="width: 100px; " />
                         <span>元/</span><span class="spanUnit"></span>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>兑换限额:</span>
                    </td>
                    <td>
                        <input type="text" name="numExchangeLimit" style="width: 100px; " />
                        <span  class="spanUnit"></span>
                    </td>
                </tr>
                <tr id="trAdd">
                    <td>
                    </td>
                    <td>
                        <input type="button" id="btnAdd" value="添加" onclick="FunAdd()" />
                    </td>
                </tr>
                <tr id="trUpdate">
                    <td>
                    </td>
                    <td>
                        <input type="button" id="btnUpdate" value="修改" onclick="FunUpdate()" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

     <div id="divStorage" class="pageEidt" style="display: none;">
    <div style="color:Blue; font-weight:bolder;">
          [商品储存信息修改]
         <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="goodfrmClose()" />
            </div>
        <div style="clear: both;">
            <table class="tabEdit">
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>商品名称:</span>
                    </td>
                    <td>
                        <input type="text" name="strName2" style="width: 200px;" /><span style="color: Red;
                            font-weight: bolder;">*</span>
                    </td>
                </tr>

                <tr>
                    <td align="right">
                        <span>供应商:</span>
                    </td>
                    <td>
                        <select name="WBSupplierID" style="width: 200px;">
                        </select>
                    </td>
                </tr>
            
                <tr>
                    <td align="right">
                        <span>入仓库号:</span>
                    </td>
                    <td>
                        <select name="WBWareHouseID" style="width: 200px;">
                        </select>
                    </td>
              
                <tr>
                    <td align="right">
                        <span>最大库存:</span>
                    </td>
                    <td>
                        <input name="maxStore" type="text"  style="width: 100px;" />
                       
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>当前库存:</span>
                    </td>
                    <td id="tdStorage">
                      
                      <span style="color:Blue" id="numStore"></span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                      <a href="#" onclick="OpenJinHuo()">[进货]</a>
                    </td>
                </tr>
               
                <tr id="tr2">
                    <td>
                    </td>
                    <td>
                        <input type="button" id="Button2" value="保存" onclick="FunSaveStorage()" />
                    </td>
                </tr>
            </table>
        </div>
    </div>


    </form>
     <%--商品存储信息编号--%>
    <input type="hidden" id="GoodStorageID" value="" />
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
