<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Supplier.aspx.cs" Inherits="Web.Admin.Good.Supplier" %>

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

        });
        /*--------End  窗体启动设置和基本设置--------*/


        /*--------自定义div窗口的开启和关闭--------*/
        //显示新增数据窗口
        function ShowFrm_Add(wbid) {
            $('#divfrm').fadeIn("fast");
            $('#trAdd').fadeIn("fast");
            $('#trUpdate').fadeOut("fast");

            $('input[name=strName]').val('');
            $('input[name=LinkMan]').val('');
            $('input[name=LinManDuty]').val('');
            $('input[name=Address]').val('');
            $('input[name=PostCode]').val('341222');
            $('input[name=PhoneNO]').val('');
            $('input[name=Mobile]').val('');
            $('input[name=strRemark]').val('');
        }
        //显示更新数据窗口
        function ShowFrm_Update(wbid) {
            $('#divfrm').fadeIn("fast");
            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");

            /*----数据提交----*/
            $.ajax({
                url: '/Ashx/good.ashx?type=GetByID_Supplier&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    $('input[name=strName]').val(r[0].strName);
                    $('input[name=LinkMan]').val(r[0].LinkMan);
                    $('input[name=LinManDuty]').val(r[0].LinManDuty);
                    $('input[name=Address]').val(r[0].Address);
                    $('input[name=PostCode]').val(r[0].PostCode);
                    $('input[name=PhoneNO]').val(r[0].PhoneNO);
                    $('input[name=Mobile]').val(r[0].Mobile);
                    $('input[name=strRemark]').val(r[0].strRemark);

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
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                    $.ajax({
                        url: '/Ashx/good.ashx?type=Add_Supplier',
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
                            window.location = "error.html";
                        }
                    });
                } else {
                    //console.log('你点击了取消！');
                }

            });
        }
        //更新数据方法
        function FunUpdate() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var wbid = $('#WBID').val();
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                    $.ajax({
                        url: '/Ashx/good.ashx?type=Update_Supplier&ID=' + wbid,
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
            SingleDataDelete('/Ashx/good.ashx?type=DeleteByID_Supplier&ID=' + wbid);
         
        }
        //提交检测
        function SubmitCheck() {
            if (!CheckInput('strName', '公司名称', '-1')) {
                return false;
            }
            if (!CheckInput('LinkMan', '联系人', '-1')) {
                return false;
            }

            if (!CheckInput('Address', '联系地址', '-1')) {
                return false;
            }
            if (!CheckInput('PostCode', '邮政编码', '-1')) {
                return false;
            }
            if (!CheckInput('Mobile', '移动电话', '-1')) {
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
        <b>供应商信息管理</b>
    </div>
   
    <table>
        <tr>
            <td>
         <span>供应商信息列表</span>
            </td>
          
            <td>
                <a href="#" onclick="ShowFrm(0)">添加供应商</a>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="Repeater1" runat="server">
        <HeaderTemplate>
            <table class="tabData">
                <tr class="tr_head">
                    <th style="width: 200px; text-align: center;">
                        供应商名称
                    </th>
                    <th style="width: 100px; text-align: center;">
                        联系人
                    </th>
                     <th style="width: 100px; text-align: center;">
                        电话
                    </th>
                    <th style="width: 200px; text-align: center;">
                        联系人地址
                    </th>
                    <th style="width: 150px; text-align: center;">
                        查看/修改
                    </th>
                     <th style="width: 100px; text-align: center;">
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
                    <%#Eval("LinkMan")%>
                </td>
                <td>
                    <%#Eval("Mobile")%>
                </td>
                 <td>
                    <%#Eval("Address")%>
                </td>
               
      <td> <%# GetUpdateItem(Eval("ID")) %> </td>
      <td> <%# GetDeleteItem(Eval("ID")) %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <!--底部模板-->
            </table>
            <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>
    
    <div id="divfrm" class="pageEidt" style="display: none;">
        <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口"  style="float:right; cursor:pointer;" onclick="CloseFrm()" />
        <div style="clear: both;">
            <table class="tabEdit">
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>公司名称:</span>
                    </td>
                    <td>
                        <input type="text" name="strName" style="width: 200px;" /><span style="color:Red; font-weight:bolder;">*</span>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>联系人:</span>
                    </td>
                    <td>
                        <input type="text" name="LinkMan" style="width: 200px; " /><span style="color:Red; font-weight:bolder;">*</span>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>联系人职务:</span>
                    </td>
                    <td>
                        <input name="LinkManDuty" type="text" style="width: 200px;" />
                    </td>
                </tr>
                  <tr>
                    <td align="right">
                        <span>联系地址:</span>
                    </td>
                    <td>
                        <input name="Address" type="text" style="width: 200px;" /><span style="color:Red; font-weight:bolder;">*</span>
                    </td>
                </tr>
                  <tr>
                    <td align="right">
                        <span>邮政编码:</span>
                    </td>
                    <td>
                        <input name="PostCode" type="text" style="width: 200px;" /><span style="color:Red; font-weight:bolder;">*</span>
                    </td>
                </tr>
                  <tr>
                    <td align="right">
                        <span>单位电话:</span>
                    </td>
                    <td>
                        <input name="PhoneNO" type="text" style="width: 200px;" />
                    </td>
                </tr>
                  <tr>
                    <td align="right">
                        <span>移动电话:</span>
                    </td>
                    <td>
                        <input name="Mobile" type="text" style="width: 200px;" /><span style="color:Red; font-weight:bolder;">*</span>
                    </td>
                </tr>
                   <tr>
                    <td align="right">
                        <span>备注:</span>
                    </td>
                    <td>
                       <textarea name="strRemark"  style="width:200px; height:50px;"></textarea>
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
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
