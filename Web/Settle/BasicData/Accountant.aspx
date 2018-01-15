<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Accountant.aspx.cs" Inherits="Web.Settle.BasicData.Accountant" %>

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
            $('#trAdd').fadeIn("fast");
            $('#trUpdate').fadeOut("fast");
            $('input[name=strName]').val('');
        }
        //显示更新数据窗口
        function ShowFrm_Update(wbid) {
            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");

            /*----数据提交----*/
            $.ajax({
                url: '/Ashx/settlebasic.ashx?type=GetByID_Accountant&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    $('input[name=strName]').val(r[0].strName);

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
            SingleDataAdd('/Ashx/settlebasic.ashx?type=Add_Accountant', $('#form1').serialize());
           
        }
        //更新数据方法
        function FunUpdate() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var wbid = $('#WBID').val();
            SingleDataUpdate('/Ashx/settlebasic.ashx?type=Update_Accountant&ID=' + wbid, $('#form1').serialize());
         
        }

        //删除数据方法
        function FunDelete(wbid) {
            SingleDataDelete('/Ashx/settlebasic.ashx?type=DeleteByID_Accountant&ID=' + wbid);
          
        }
        //提交检测
        function SubmitCheck() {
            if ($('input[name=strName]').val() == "") {
               showMsg('请输入总部会计名称 ！');
                $('input[name=strName]').focus();
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
       <b>总部会计管理</b><%--<span id="spanHelp" style="cursor: pointer">帮助</span>--%>
    </div>
    
    <table>
        <tr>
            <td>
                <b>已有总部会计列表</b>
            </td>
            <td>
              <%=GetAddItem() %>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="Repeater1" runat="server">
        <HeaderTemplate>
            <table class="tabData">
                <tr class="tr_head">
                    <th style="width: 200px; text-align: center;">
                       总部会计
                    </th>
                    
                    <th style="width: 120px; text-align: center;">
                        查看/修改
                    </th><th style="width: 120px; text-align: center;">
                       删除
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                
                <td>
                    <%#Eval("strName")%>
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
    <div class="divWarning">
        <b>特别提示:</b><span>这是系统关键数据，最好不要添加、修改、删除!</span>
    </div>
    <div id="divfrm" class="pageEidt" style="display: none;">
       <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口"  style="float:right; cursor:pointer;" onclick="CloseFrm()" />
        <div style="clear: both;">
            <table class="tabEdit">
               
                <tr>
                    <td>
                        <span>总部会计:</span>
                    </td>
                    <td>
                        <input name="strName" type="text" style="width: 200px;" />
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
