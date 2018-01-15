<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetStorageFee.aspx.cs" Inherits="Web.BasicData.StoragePara.SetStorageFee" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    
   
    <style type="text/css">

</style>
    <script type="text/javascript">
        /*--------窗体启动设置和基本设置--------*/
        //表格鼠标悬停换色
        function change_colorOver(e) {
            var oldColor = e.style.backgroundColor;
            document.getElementById("colorName").value = oldColor;
            e.style.backgroundColor = "#b9bace";
        }
        function change_colorOut(e) {
            e.style.backgroundColor = document.getElementById("colorName").value;
        }

        $(function () {
            $('#spanHelp').toggle(function () {
                $('#divHelp').fadeIn();
            }, function () {
                $('#divHelp').fadeOut();
            });
            InitSelect('storage.ashx?type=GetStorageUser', 'TypeID','加载储户类型失败！');
            InitSelect('storage.ashx?type=GetStorageVariety', 'VarietyID','加载存粮类型失败！');
            InitSelect('storage.ashx?type=GetStorageTime', 'TimeID','加载存期类型失败！');
        });
        //初始化select标签
        function InitSelect(strUrl, strName,strAlert) {
            $.ajax({
                url: strUrl,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=' + strName + ']').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                }, error: function (r) {
                    showMsg(strAlert);
                }
            });
        }
        /*--------End  窗体启动设置和基本设置--------*/


        /*--------自定义div窗口的开启和关闭--------*/
        function ShowFrm(wbid) {
            $('#divfrm').fadeIn("normal");
            $('#WBID').val(wbid);
            if (wbid == "0") {//新增网点
                ShowFrm_Add(wbid);
            }
            else { //编辑网点
                ShowFrm_Update(wbid);
            }
        };

        function CloseFrm() {
            $('#divfrm').fadeOut("normal");
        }

        //显示新增数据窗口
        function ShowFrm_Add(wbid) {
            $('#trAdd').fadeIn("fast");
            $('#trUpdate').fadeOut("fast");

            $("select[name=VarietyID]").get(0).selectedIndex = 0; //设置索引为1的项选中
            $("select[name=TypeID]").get(0).selectedIndex = 0;
            $("select[name=TimeID]").get(0).selectedIndex = 0;

            $('input[name=numStorageFee]').val('0.000');
            $('input[name=numUpper]').val('0');
            $('input[name=numLower]').val('0');
        }
        //显示更新数据窗口
        function ShowFrm_Update(wbid) {
            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");

            /*----数据提交----*/
            $.ajax({
                url: 'storage.ashx?type=GetStorageFeeByID&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $("select[name=TypeID]  option[value='" + r[0].TypeID + "'] ").attr("selected", 'selected');
                    $("select[name=VarietyID]  option[value='" + r[0].VarietyID + "'] ").attr("selected", 'selected');
               $("select[name=TimeID]  option[value='" + r[0].TimeID + "'] ").attr("selected", 'selected');

               $('input[name=numStorageFee]').val(r[0].numStorageFee);
               $('input[name=numUpper]').val(r[0].numUpper);
               $('input[name=numLower]').val(r[0].numLower);
                   
                }, error: function (r) {
                   showMsg('加载信息失败 ！');
                }
            });
            /*---End 数据提交----*/
        }
        /*--------End 自定义div窗口的开启和关闭--------*/


        /*--------数据增删改操作--------*/
        //新增数据方法
        function FunAdd() {
            SingleDataAdd('storage.ashx?type=AddStorageFee', $('#form1').serialize());
            
        }
        //更新数据方法
        function FunUpdate() {
          
            var wbid = $('#WBID').val();
            
            SingleDataUpdate('storage.ashx?type=UpdateStorageFee&ID=' + wbid,$('#form1').serialize());
          
        }

        //删除数据方法
        function FunDelete(wbid) {
            SingleDataDelete('storage.ashx?type=DeleteStorageFeeByID&ID=' + wbid);
          
        }
        //提交检测
        function SubmitCheck() {
            if ($('input[name=strName]').val() == "") {
               showMsg('请输入存储期限名称 ！');
                $('input[name=strName]').focus();
                return false;
            }
            if (isNaN($('input[name=numStorageDate]').val())) {
               showMsg('约定实存天数应填写数字 ！');
                $('input[name=numStorageDate]').focus();
                return false;
            }
            if (isNaN($('input[name=numExChangeProp]').val())) {
               showMsg('约定兑换比例应填写数字 ！');
                $('input[name=numExChangeProp]').focus();
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
<b>设置分档保管费</b><span id="spanHelp" style="cursor:pointer" >帮助</span>
</div>
<div id="divHelp" style="border:1px solid #333; border-radius:5px; display:none; ">
<span>提示1：</span><br />
<span>提示2：</span><br />
<span>提示3：</span><br />
</div>

<div class="QueryHead">
<table>
            <tr>
            <td><b>已有分档保管费列表</b></td>
            
            <td><a href="#" onclick="ShowFrm(0)">添加分档保管费</td>
            </tr>
            
        </table>
</div>
<asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table  class="tabData" >
          <tr class="tr_head" >
                <th style="width:100px; text-align:center;">
                    储户类型</th>
                    <th style="width:100px; text-align:center;">
                   存储期限</th>
                    <th style="width:100px; text-align:center;">
                   存储产品</th>
                    <th style="width:100px; text-align:center;">
                   保管费率</th>
                    <th style="width:100px; text-align:center;">
                    适用上限</th>
                    <th style="width:100px; text-align:center;">
                    适用下限</th>
                   
                
                    <th style="width:120px; text-align:center;">
                    查看/修改</th>
                   <th style="width:100px; text-align:center;">
                    删除</th>
                
            </tr>
        
    </HeaderTemplate>
    <ItemTemplate>
    <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
        <td><%#Eval("TypeID")%></td>
         <td><%#Eval("TimeID")%></td>
          <td><%#Eval("VarietyID")%></td>
          <td><%#Eval("numStorageFee")%></td>
          <td><%#Eval("numUpper")%></td>
          <td><%#Eval("numLower")%></td>
        <td><input type="button" value="查看/修改" style="width:80px; height:25px;" onclick="ShowFrm(<%#Eval("ID") %>)" /></td>
        <td><input type="button" value="删除" style="width:80px; height:25px;" onclick="FunDelete(<%#Eval("ID") %>)" /></td>
   
    </tr>
    </ItemTemplate>
    
    <FooterTemplate><!--底部模板-->
        </table>        <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>
   <div class="divWarning">
   <b>特别提示:</b><span>这是系统关键数据，最好不要添加、修改、删除!</span>
   </div>
    <div  id="divfrm" class="pageEidt" style="display:none;">
    <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口"  style="float:right; cursor:pointer;" onclick="CloseFrm()" />   
    <div style="clear:both;">
        <table >
            
            <tr>
            <td style="width:100px;"><span>储户类型:</span></td>
            <td><select name="TypeID" style="width:150px; height:25px" ></select></td>
            </tr>
              <tr>
            <td ><span>存储期限:</span></td>
            <td><select name="TimeID" style="width:150px; height:25px" ></select></td>
            </tr>
                        <tr>
            <td ><span>存储产品:</span></td>
            <td><select name="VarietyID" style="width:150px; height:25px" ></select></td>
             </tr>
            <tr>
            <td ><span>保管费率:</span></td>
            <td><input  name="numStorageFee" type="text" style="width:100px;" /><span>元/公斤/月</span></td>
           
            </tr>
            <tr>
            <td ><span>适用上限:</span></td>
            <td><input name="numUpper" type="text" style="width:100px;"  /></td>
            </tr>
            <tr>
            <td ><span>适用下限:</span></td>
            <td><input name="numLower" type="text" style="width:100px;"  /><span>元/公斤/月</span></td>
            </tr>
        
             
            <tr id="trAdd">
            <td></td>
            <td ><input type="button" id="btnAdd" value="添加" onclick="FunAdd()" /> </td>
            </tr>
               <tr id="trUpdate">
            <td></td>
            <td ><input type="button" id="btnUpdate" value="修改" onclick="FunUpdate()" />
         
             </td>
            </tr>
        </table>
        </div>
    </div>

    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>


