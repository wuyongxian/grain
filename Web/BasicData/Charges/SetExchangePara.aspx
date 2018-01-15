<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetExchangePara.aspx.cs" Inherits="Web.BasicData.Charges.SetExchangePara" %>

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
            //初始化存储产品类型信息
            InitVarietyID();
            InitTypeID();
            InitGoodID();
            $('select[name=TypeID]').change(function () { InitTimeID(); });

            $('input[name=ChuFenLv]').val('0.8');
            $('input[name=FuPi]').val('0');
            $('input[name=JiaGongFei]').val('0');
            $('#spanHelp').show();
        });



        //获取储户类型
        function InitTypeID() {
          
         
            $.ajax({
                url: '/User/Storage/storage.ashx?type=GetTypeFromVarietyRate',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('select[name=TypeID]').empty();
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=TypeID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                    InitTimeID(); //获取存期信息
                }, error: function (r) {
                    if (r.responseText != 'Error') {
                       showMsg('不存在此储户类型,您可以请管理员添加相关的基础数据信息 ！');
                    }
                }
            });
        }

        //获取存期信息
        function InitTimeID() {
            var VarietyID = $('select[name=VarietyID] option:selected').val();
            var TypeID = $('select[name=TypeID] option:selected').val();
            $('select[name=TimeID]').empty();
            $.ajax({
                url: '/BasicData/StoragePara/storage.ashx?type=GetStorageTimeByTypeID&TypeID=' + TypeID,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=TimeID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }

                }, error: function (r) {
                    if (r.responseText != 'Error') {
                       showMsg('加载储户存期失败 ！');
                    }
                }
            });
        }
        /*--ENd loadFuntion--*/
        //获取储存产品类型
        function InitVarietyID() {
            $('select[name=VarietyID]').empty();
            $.ajax({
                url: '/User/Storage/storage.ashx?type=GetVarietyFromStorageRate',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    for (var i = 0; i < r.length; i++) {
                        $('select[name=VarietyID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                  
                    InitTypeID(); //加载储户类型
                }, error: function (r) {
                    if (r.responseText != 'Error') {
                       showMsg('加载储户产品类型失败 ！');
                    }
                    if (r.responseText != 'Price') {
                       showMsg('无法获取当前的存储产品和价格利率 ！');
                    }
                }
            });
        }


        function InitGoodID() {
            $('select[name=GoodID]').empty();
            var strName = $.trim($('input[name=QGood]').val());
            var strUrl ='/Ashx/good.ashx?type=Get_Good&strName='+strName;
            
            $.ajax({
                url: strUrl,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    for (var i = 0; i < r.length; i++) {
                        $('select[name=GoodID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }

                    InitTypeID(); //加载储户类型
                }, error: function (r) {
                    if (r.responseText != 'Error') {
                       showMsg('加载商品类型失败 ！');
                    }
                    if (r.responseText != 'Price') {
                       showMsg('无法获取当前的存储产品和价格利率 ！');
                    }
                }
            });
        }

        function FunUpdate() {
            if (!SubmitCheck()) {
                return false;
            }
            $.ajax({
                url: '/BasicData/StoragePara/storage.ashx?type=UpdateExchangeProp',
                type: 'post',
                data: $('#form1').serialize(),
                dataType: 'text',
                success: function (r) {
                   showMsg('设置成功 ！');
                    location.reload();
                }, error: function (r) {
                   showMsg('设置失败 ！');
                }
            });

        }


        //删除数据方法
        function FunDelete(wbid) {
            SingleDataDelete('/BasicData/StoragePara/storage.ashx?type=DeleteExchangeProp&ID=' + wbid);
           
        }
        //提交检测
        function SubmitCheck() {
            if ($('select[name=TypeID] option:selected').val() == "") {
               showMsg('存入类型不能为空 ！');
                return false;
            }
            if ($('select[name=TimeID] option:selected').text() == "") {
               showMsg('存期类型不能为空 ！');
                return false;
            }
            if ($('input[name=VarietyID]').val() == "") {
               showMsg('存储产品不能为空 ！');
                return false;
            }
            if ($('input[name=GoodID]').val() == "") {
               showMsg('兑换商品不能为空 ！');
                return false;
            }

            if (!CheckNumDecimal($('input[name=ChuFenLv]').val(), '出粉/米率', 5)) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=FuPi]').val(), '麸皮/米糠', 4)) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=JiaGongFei]').val(), '加工费', 4)) {
                return false;
            }
            return true;
        }
        /*--------End 数据增删改操作--------*/
     
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>按比例兑换参数设置</b><span id="spanHelp" style="cursor: pointer">帮助</span>
    </div>
    <div id="divHelp"  class="pageHelp">
        <span>提示1：按比例兑换的折换率只设置兑换业务（兑换业务——按比例兑换，如100公斤小麦兑换70公斤面粉，兑换比例不随价格波动）的相关费用。</span><br />
        <span>提示2：如果想把一种商品由按比例兑换转换为完全按价格折换，只要把对应的费用率删除即可；反之，如果想把一种商品由完全按价格折换转换成按比例兑换，只要添加对应的折换参数即可。</span><br />
        <span>提示3：如果要设置的费用率在费用率中不存在，程序会自动添加；如果要设置的费用率在费用率列表中已经存在，程序会自动修改费用率。</span><br />
    </div>
     <table class="tabEdit">
            
                <tr class="tr_head">
                    <th style="width: 150px; text-align: center;">
                        存入类型
                    </th>
                    <th style="width: 150px; text-align: center;">
                        存期类型
                    </th>
                     <th style="width: 150px; text-align: center;">
                        存储产品
                    </th>
                     <th style="width: 150px; text-align: center;">
                        兑换商品
                    </th>
                     <th style="width: 80px; text-align: center;">
                        出粉/米率
                    </th>
                     <th style="width: 80px; text-align: center;">
                        麸皮/米糠
                    </th>
                     <th style="width: 80px; text-align: center;">
                       加工费
                    </th>
                   
                </tr>
         <tr>
             <td>
                 <select name="TypeID" style="width: 120px">
                 </select>
             </td>
             <td>
                 <select name="TimeID" style="width: 120px">
                 </select>
             </td>
             <td>
                 <select name="VarietyID" style="width: 120px">
                 </select>
             </td>
             <td>
                 <select name="GoodID" style="width: 120px">
                 </select>
             </td>
             <td>
                 <input type="text" name="ChuFenLv" style="width: 60px;" />
             </td>
             <td>
                 <input type="text" name="FuPi" style="width: 60px;" />
             </td>
             <td>
                 <input type="text" name="JiaGongFei" style="width: 60px;" />
             </td>
         </tr>
         <tr>
             <td style="text-align: left" colspan="7">
                 <span>搜索兑换商品:</span>
                 <input type="text" name="QGood" style="width: 100px" />
                 <input type="button" id="btnQGood" value="查询" style="width: 60px;"  onclick="InitGoodID()" />&nbsp;&nbsp;
                 <input type="checkbox" name="synGood" />同步设置同种产品&nbsp;&nbsp;&nbsp;&nbsp;
                 <input type="button" value="添加/修改" style="width: 100px;" onclick="FunUpdate()" />
             </td>
         </tr>
                </table>
    <asp:Repeater ID="Repeater1" runat="server">
        <HeaderTemplate>
            <table class="tabData">
            <tr>
            <th></th>
            </tr>
                <tr class="tr_head">
                    <th style="width: 150px; text-align: center;">
                        存入类型
                    </th>
                    <th style="width: 150px; text-align: center;">
                        存期类型
                    </th>
                     <th style="width: 150px; text-align: center;">
                        存储产品
                    </th>
                     <th style="width: 150px; text-align: center;">
                        兑换商品
                    </th>
                     <th style="width: 80px; text-align: center;">
                        出粉/米率
                    </th>
                     <th style="width: 80px; text-align: center;">
                        麸皮/米糠
                    </th>
                     <th style="width: 80px; text-align: center;">
                       加工费
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
                    <%#Eval("TypeID")%>
                </td>
                 <td>
                    <%#Eval("TimeID")%>
                </td>
                <td>
                    <%#Eval("VarietyID")%>
                </td>
                <td>
                    <%#Eval("GoodID")%>
                </td>
                <td>
                    <%#Eval("ChuFenLv")%>
                </td>
                <td>
                    <%#Eval("FuPi")%>
                </td>
                <td>
                    <%#Eval("JiaGongFei")%>
                </td>
               
                 <td><input type="button" style="width:50px;height:25px;" value="删除" onclick="FunDelete(<%#Eval("ID") %>)" /></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <!--底部模板-->
            </table>
            <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>
  
    
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
