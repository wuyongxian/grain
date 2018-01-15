<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userStorageAdd.aspx.cs" Inherits="Web.User.Goods.userStorageAdd" %>

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
            $('input[name=Address]').val('');
            $('input[name=Capacity]').val('');
            $('input[name=ISDefault]').removeAttr('checked');
        }
        //显示更新数据窗口
        function ShowFrm_Update(wbid) {
            $('#divfrm').fadeIn("fast");
            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");

            /*----数据提交----*/
            $.ajax({
                url: '/BasicData/HQStorage/storage.ashx?type=GetByID_storage&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    $('input[name=strName]').val(r[0].strName);
                    $('input[name=Address]').val(r[0].Address);
                    $('input[name=Capacity]').val(r[0].Capacity);
                    if (r[0].ISDefault) {
                        $('input[name=ISDefault]').attr('checked', 'checked');
                    } else {
                        $('input[name=ISDefault]').removeAttr('checked');
                    }
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
            SingleDataAdd('/BasicData/HQStorage/storage.ashx?type=Add_storage', $('#form1').serialize());
            
        }
        //更新数据方法
        function FunUpdate() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var wbid = $('#WBID').val();
            SingleDataUpdate('/BasicData/HQStorage/storage.ashx?type=Update_storage&ID=' + wbid, $('#form1').serialize());
          
        }

        //删除数据方法
        function FunDelete(wbid) {
            SingleDataDelete('/BasicData/HQStorage/storage.ashx?type=DeleteByID_storage&ID=' + wbid);
           
        }
        //提交检测
        function SubmitCheck() {
            if (!CheckInput('strName', '仓库名', '-1')) {
                return false;
            }
            if (!CheckInput('strName', '仓库地点', '-1')) {
                return false;
            }

            if (!CheckNumInt($('input[name=Capacity]').val(), '仓库容量', '0', '-1')) {
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
      <b>仓库管理</b>
    </div>
   
    <table>
        <tr>
            <td>
         <span>已有仓库列表</span>
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
                    <th style="width: 150px; text-align: center;">
                        所在网点
                    </th>
                    <th style="width: 100px; text-align: center;">
                        仓库名称
                    </th>
                     <th style="width: 150px; text-align: center;">
                        仓库地点
                    </th>
                    <th style="width: 150px; text-align: center;">
                        仓库容量
                    </th>
                      <th style="width: 100px; text-align: center;">
                        默认仓库
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
                    <%#Eval("WBID")%>
                </td>
                 <td>
                    <%#Eval("strName")%>
                </td>
                <td>
                    <%#Eval("Address")%>
                </td>
                 <td>
                    <%#Eval("Capacity")%>
                </td>
                <td>
                    <%#Eval("ISDefault")%>
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
                    <td align="right" style="width: 100px;">
                        <span>仓库名:</span>
                    </td>
                    <td>
                        <input type="text" name="strName" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>仓库地点:</span>
                    </td>
                    <td>
                        <input type="text" name="Address" style="width: 200px; " />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>仓库容量:</span>
                    </td>
                    <td>
                        <input name="Capacity" type="text" style="width: 200px;" />
                    </td>
                </tr>
                   <tr>
                    <td align="right">
                        <span>默认仓库:</span>
                    </td>
                    <td>
                         <input type="checkbox" id="ISDefault-1"  name="ISDefault" value="1" class="custom-checkbox"  /><label for="ISDefault-1"></label><span style="margin:0px 0px 0px 5px">设为默认仓库</span></td>
                   
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