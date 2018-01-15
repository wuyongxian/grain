<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WBControl.aspx.cs" Inherits="Web.Admin.WebSiteSetting.WBControl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    
   
    <style type="text/css">

</style>
<script type="text/javascript">
    function change_colorOver(e) {
        var oldColor = e.style.backgroundColor;
        document.getElementById("colorName").value = oldColor;
        e.style.backgroundColor = "#b9bace";
    }
    function change_colorOut(e) {
        e.style.backgroundColor = document.getElementById("colorName").value;
    }
    function HelpOpen() {
        $('#divHelp').fadeIn();
    }

    function HelpClose() {
        $('#divHelp').fadeOut();
    }

    $(function () {
        //显示网点类型
        $.ajax({
            url: 'ws.ashx?type=getWBType',
            type: 'post',
            data: '',
            dataType: 'json',
            success: function (r) {
                for (var i = 0; i < r.length; i++) {
                    $('select[name=WBType_ID]').append("<option value='" + r[i].ID + "'>" + r[i].strType + "</option>");
                }
            }, error: function (r) {
               showMsg('加载网点类型失败 ！');
            }
        });

    });

    function ShowFrm(wbid) {
        showBodyCenter($('#divfrm'));
        $('#WBID').val(wbid);
        if (wbid == "0") {//新建网点
            $('#trAdd').fadeIn("fast");
            $('#trUpdate').fadeOut("fast");
            $('input[name=sNumber]').val('000');
            $('input[name=strType]').val('');
            $('input[name=strAddress]').val('');
            $('input[name=numAgent]').val('0');
            $('input[name=numSettle]').val('0');
            $('input[name=numDay]').val('180');
            $('input[name=draw_exchange]').removeAttr('checked');
            $('input[name=draw_sell]').removeAttr('checked');
            $('input[name=draw_shopping]').removeAttr('checked');
        }
        else { //编辑网点
            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");

            /*--------数据提交--------*/
            $.ajax({
                url: 'ws.ashx?type=GetWBByID&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('input[name=sNumber]').val(r[0].SerialNumber);
                    $('input[name=strType]').val(r[0].strName);
                    $('input[name=strAddress]').val(r[0].strAddress);
                    $('input[name=numAgent]').val(r[0].numAgent);
                    $('input[name=numSettle]').val(r[0].numSettle);
                    $('input[name=numDay]').val(r[0].numDay);
                    $("select[name=WBType_ID]  option[value='" + r[0].WBType_ID + "'] ").attr("selected", "selected")
                    if (r[0].draw_exchange == true || r[0].draw_exchange == '1') {
                        $('input[name=draw_exchange]').attr('checked', 'checked');
                    } else {
                        $('input[name=draw_exchange]').removeAttr('checked');
                    }
                    if (r[0].draw_sell == true || r[0].draw_sell == '1') {
                        $('input[name=draw_sell]').attr('checked', 'checked');
                    } else {
                        $('input[name=draw_sell]').removeAttr('checked');
                    }
                    if (r[0].draw_shopping == true || r[0].draw_shopping == '1') {
                        $('input[name=draw_shopping]').attr('checked', 'checked');
                    } else {
                        $('input[name=draw_shopping]').removeAttr('checked');
                    }

                }, error: function (r) {
                   showMsg('加载网点数据失败 ！');
                }
            });
            /*--------End 数据提交--------*/
        }

    }
    function CloseFrm() {
        $('#divfrm').fadeOut("normal");
    }

    //添加、修改网点 （ID=0是添加网点）
    function WBUpdate() {
        if (!SubmitCheck()) {//检测输入内容
            return false;
        }
     

        var wbid = $('#WBID').val();
        var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
        showConfirm(msg, function (obj) {
            if (obj == 'yes') {
                
                $.ajax({
                    url: 'ws.ashx?type=UpdateWB&ID=' + wbid + "&sNumber=" + $('input[name=sNumber]').val(),
                    type: 'post',
                    data: $('#form1').serialize(),
                    dataType: 'text',
                    success: function (r) {
                        if (r == "OK") {
                            if (wbid == "0") {
                                showMsg('添加数据成功 ！');
                            } else {
                                showMsg('更新数据成功 ！');
                            }
                            CloseFrm();
                            location.reload();
                        } else if (r == "1") {
                            showMsg('已存在相同的网点名称，请修改后添加 ！');
                        }
                    }, error: function (r) {
                        showMsg('加载网点类型失败 ！');
                    }
                });
            } else {
                //console.log('你点击了取消！');
            }

        });
    }

    //提交检测
    function SubmitCheck() {
        var wbid = $('#WBID').val();



        if ($('input[name=strType]').val() == "") {
           showMsg('请输入网点名称 ！');
            $('input[name=strType]').focus();
            return false;
        }
        if ($('input[name=strAddress]').val() == "") {
           showMsg('请输入网点地址 ！');
            $('input[name=strAddress]').focus();
            return false;
        }

        if (isNaN($('input[name=numSettle]').val())) {
           showMsg('结算扣率请输入数字 ！');
            $('input[name=numSettle]').focus();
            return false;
        }
        if (isNaN($('input[name=numAgent]').val())) {
           showMsg('代理费率请输入数字 ！');
            $('input[name=numAgent]').focus();
            return false;
        }
        return true;

    }


    function WBDelete(ID) {
        var msg = '您确认要删除此网点吗？';
        showConfirm(msg, function (obj) {
            if (obj == 'yes') {
                
                $.ajax({
                    url: 'ws.ashx?type=DeleteWBByID&ID=' + ID,
                    type: 'post',
                    data: '',
                    dataType: 'json',
                    success: function (r) {
                        showMsg(r.msg);
                    }, error: function (r) {
                        showMsg(r.msg);
                    }
                });
            } else {
                //console.log('你点击了取消！');
            }

        });
    }

   
     
</script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>网点设置</b><span id="spanHelp" style="cursor: pointer">帮助</span>
        </div>
        <div id="divHelp" class="pageHelp">
            <span>提示1：总部用于管理其它网点，每个单位可以设置一个总部；如果想把某个网点设为总部，在设置网点中单击“设为总部”；每个单位可以设置一个模拟网点用于练习，单击指定模拟网点即可设置。</span>
            <br />
            <span>提示2：单击“查看/修改”时可以修改所选网点的详细信息。</span><br />
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>按网点名称查询:</span></td>
                    <td><span>
                        <input type="text" id="txtType" runat="server" />
                    </span></td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="~/images/seach_brown.png" OnClick="ImageButton1_Click" />
                    </td>
                    <td><%=GetAddItem() %></td>
                </tr>

            </table>
        </div>
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" style="width: 900px">
                    <tr class="tr_head">
                        <th style="width: 100px; text-align: center;">网点编号</th>
                        <th style="width: 100px; text-align: center;">网点名称</th>
                        <th style="width: 200px; text-align: center;">网点地址</th>
                        <th style="width: 100px; text-align: center;">网点类型</th>
                        <th style="width: 100px; text-align: center;">代理费率</th>

                        <th style="width: 120px; text-align: center;">查看/修改</th>
                        <th style="width: 120px; text-align: center;">删除</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td><%#Eval("SerialNumber")%></td>
                    <td><%#Eval("strName")%><%#SetISHQ(Eval("ISHQ"),Eval("ISSimulate")) %></td>
                    <td><%#Eval("strAddress")%></td>
                    <td><%#Eval("WBType_ID")%></td>
                    <td><%#Eval("numAgent")%></td>

                    <td><%#GetUpdateItem(Eval("ID")) %></td>
                    <td><%#GetDelItem(Eval("ID"),Eval("ISHQ")) %></td>
                </tr>
            </ItemTemplate>

            <FooterTemplate>
                <!--底部模板-->
                </table>       
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>

        <div id="divfrm" class="pageEidt" style="display: none;">
            <div style="float: left; ">
                <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="CloseFrm()" />
             
                <div style="clear: both">
                    <table class="tabEdit">
                        <tr>
                            <td class="td_right" style="width: 160px;"><span>网点序号:</span></td>
                            <td>
                                <input type="text" name="sNumber" disabled="disabled" style="font-size: 18px; font-weight: Bold;" /></td>
                        </tr>
                        <tr>
                            <td class="td_right"><span>网点类型:</span></td>
                            <td>
                                <select name="WBType_ID" style="width: 100px; height: 25px"></select></td>
                        </tr>
                        <tr>
                            <td class="td_right"><span>网点名称:</span></td>
                            <td>
                                <input type="text" name="strType" /></td>
                        </tr>
                        <tr>
                            <td class="td_right"><span>网点地址:</span></td>
                            <td>
                                <input type="text" name="strAddress" /></td>
                        </tr>
                        <tr style="display:none;">
                            <td class="td_right"><span>结算扣率:</span></td>
                            <td>
                                <input type="text" name="numSettle" /></td>
                        </tr>
                        <tr>
                            <td class="td_right"><span>加盟店结算代理费率:</span></td>
                            <td><span>
                                <input type="text" name="numAgent" style="width:50px;" />
                            </span></td>
                        </tr>
                        <tr>
                            <td class="td_right"><span>代理费结算最低期限:</span></td>
                            <td><span>
                                <input type="text" name="numDay" style="width:50px;" /><span>天</span>
                            </span></td>
                        </tr>
                        <tr>
                            <td class="td_right"><span>代理费结算支取选项:</span></td>
                            <td>
                               <%-- <input type="checkbox" name="draw_exchange" /><span style="margin-right: 5px;">兑换</span>
                                <input type="checkbox" name="draw_sell" /><span style="margin-right: 5px;">存转销</span>
                                <input type="checkbox" name="draw_shopping" /><span style="margin-right: 5px;">产品换购</span>--%>
                                 <input type="checkbox" id="ISDefault-1"  name="draw_exchange" value="1" class="custom-checkbox"  /><label for="ISDefault-1"></label><span style="margin:0px 0px 0px 5px">兑换</span>
                                <input type="checkbox" id="ISDefault-2"  name="draw_sell" value="1" class="custom-checkbox"  /><label for="ISDefault-2"></label><span style="margin:0px 0px 0px 5px">存转销</span>
                                <input type="checkbox" id="ISDefault-3"  name="draw_shopping" value="1" class="custom-checkbox"  /><label for="ISDefault-3"></label><span style="margin:0px 0px 0px 5px">产品换购</span>
                            </td>
                        </tr>

                        <tr id="trAdd">
                            <td></td>
                            <td>
                                <input type="button" id="btnAdd" value="添加网点" onclick="WBUpdate()" />
                            </td>
                        </tr>
                        <tr id="trUpdate">
                            <td></td>
                            <td>
                                <input type="button" id="btnUpdate" value="修改网点" onclick="WBUpdate()" />
                                <input type="button" id="btnDel" value="删除网点" style="display: none;" onclick="WBDelete()" />

                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
