<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WBType.aspx.cs" Inherits="Web.Admin.WebSiteSetting.WBType" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
     <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />


    <script type="text/javascript">
        //function change_colorOver(e) {
        //    var oldColor = e.style.backgroundColor;
        //    document.getElementById("colorName").value = oldColor;
        //    e.style.backgroundColor = "#b9bace";
        //}
        //function change_colorOut(e) {
        //    e.style.backgroundColor = document.getElementById("colorName").value;
        //}

        function ShowFrm(wbid) {
            showBodyCenter($('#divEidt'));
            $("#WBID").val(wbid);
            $.ajax({
                url: 'ws.ashx?type=getWBTypeByID&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('input[name=strType]').val(r[0].strType);
                    $('input[name=strDescript]').val(r[0].strDescript);

                }, error: function (r) {
                    showMsg('加载数据失败 ！');
                }
            });

            $('#trAdd').hide();
            $('#trUpdate').show();
        }

        function ShowFrm_Add(wbid) {
            showBodyCenter($('#divEidt'));
            $('input[name=strType]').val('');
            $('input[name=strDescript]').val('');
            $('#trAdd').show();
            $('#trUpdate').hide();
        }

        function CloseFrm() {
            $('#divEidt').fadeOut();
        }
       

        function FunAdd() {
            
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {

                    $.ajax({
                        url: 'ws.ashx?type=AddWBType',
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
            var wbid = $("#WBID").val();
            $.ajax({
                url: 'ws.ashx?type=UpdateWBType&ID=' + wbid,
                type: 'post',
                data: $('#form1').serialize(),
                dataType: 'text',
                success: function (r) {
                    if (r == "OK") {
                        showMsg('修改数据成功 ！');
                        CloseFrm();
                        location.reload();
                    } else {
                        showMsg('修改数据失败 ！');
                    }
                }, error: function (r) {
                    showMsg('修改数据失败 ！');
                }
            });
        }

        //删除数据方法
        function FunDelete(wbid) {
            SingleDataDelete('ws.ashx?type=DeleteWBTypeByID&ID=' + wbid);

        }
    </script>
</head>
<body>
    <div class="pageHead">
<b>网点类型设置</b>
</div>
       <table>
        <tr>
            <td>
         <span>网点类型管理</span>
            </td>
          
            <td>
              <%=GetAddItem() %>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table  class="tabData" style="width:600px">
          <tr class="tr_head">
                <%--<th style="width:50px; text-align:center;">
                    编号</th>--%>
                <th style="width:100px; text-align:center;">
                    网点类型</th>
                <th style="width:350px; text-align:center;">
                    网点类型说明</th>
                <th style="width:100px; text-align:center;">
                    查看/修改</th>
              <th style="width:100px; text-align:center;">
                    删除</th>
                
            </tr>
        
    </HeaderTemplate>
    <ItemTemplate>
    <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
      <%--  <td><%#Eval("SerialNumber")%></td>--%>
        <td><%#Eval("strType")%></td>
        <td><%#Eval("strDescript")%></td>
        <td><%#GetUpdateItem(Eval("ID")) %></td>
         <td><%#GetDeleteItem(Eval("ID")) %></td>
    </tr>
    </ItemTemplate>
    
    <FooterTemplate><!--底部模板-->
        </table>        <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>
    <form id="form1" runat="server">
        <div id="divEidt" class="pageEidt" style="display: none;">
            <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="CloseFrm()" />

            <table>
                <tr>
                    <td><span>网点类型:</span></td>
                    <td><span>
                        <input type="text" name="strType" />
                    </span></td>
                </tr>
                <tr>
                    <td><span>说明:</span></td>
                    <td><span>
                        <input type="text" name="strDescript" />
                    </span></td>
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
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
