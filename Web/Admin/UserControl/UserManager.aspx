<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManager.aspx.cs" Inherits="Web.Admin.UserControl.UserManager" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    
   
    <style type="text/css">
        .usermenu {
            display: none;
            position: fixed;
            top: 20px;
            left: 10px;
            width: 540px;
            height: 500px;
            background: #efefef;
            border: 2px solid #9ac0cd;
            padding: 5px 10px;
            border-radius: 10px;
            overflow:auto;
            z-index: 100;
        }
         .usermenu #divMenu{
            clear:both;
        }

        #divMenu .tabP {
            width: 500px;
            margin: 5px;
            font-size: 14px;
            background:#e0eeee;
        }
         #divMenu .tabP td {
           height:25px;
        }
         #divMenu .tabC {
            width: 500px;
            margin: 5px 5px 5px 30px;
            font-size: 12px;
        }
          #divMenu .tabC td {
           height:20px;
        }


        .usermenu .usermenu-action {
            text-align: center;
            margin: 5px;
        }
         .usermenu .usermenu-action  a{
           font-size:14px;
           font-weight:bold;
           margin-right:20px;
        }
           .usermenu .usermenu-action  input{
           font-size:14px;
           font-weight:bold;
           margin-right:20px;
           width:80px;
           height:25px;
        }



        

         table.border {
            border-collapse: collapse;
            border: 1px solid #9CF;
            text-align: center;
        }

            table.border thead td, table.set_border th {
                font-weight: bold;
                background-color: White;
            }

            table.border tr:nth-child(even) {
                background-color: #EAF2D3;
            }

            table.border td, table.border th {
                border: 1px solid #C3C3C3;
                text-align: center;
            }

        #divADDMenu {
            width: 400px;
            height: 300px;
            position: absolute;
            left: 200px;
            top: 50px;
            background-color: #ccc;
            border-radius: 10px;
            z-index: 1000;
        }

        .tabAddMenu {
            margin: 30px 50px 10px 50px;
            font-size: 18px;
        }

            .tabAddMenu tr td {
                height: 50px;
            }

                .tabAddMenu tr td input {
                    border-radius: 2px;
                    padding-left: 10px;
                    font-size: 18px;
                    width: 300px;
                }
</style>
    <script type="text/javascript">
   
        $(function () {
            //显示网点类型,操作级别
            $.ajax({
                url: '/Admin/UserControl/user.ashx?type=GetWB',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=WB_ID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");

                    }
                    GetUserGroupByWBID(r[0].ID);
                }, error: function (r) {
                   showMsg('加载网点类别失败 ！');
                }
            });

            $('select[name=WB_ID]').change(function () {
                GetUserGroupByWBID($('select[name=WB_ID] option:selected').val());
            });


        });

        //根据网点加载用户组级别（只有总部人员可以成为网店管理员和单位管理员）
        function GetUserGroupByWBID(WBID) {
            $('select[name=UserGroup_ID]').empty();
            $.ajax({
                url: '/Admin/UserControl/user.ashx?type=GetUserGroup&WBID=' + WBID,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=UserGroup_ID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                }, error: function (r) {
                   showMsg('加载操作级别失败 ！');
                }
            });
        }

        function ShowFrm(userID) {
            $('select[name=UserGroup_ID]').removeAttr('disabled');
            showBodyCenter($('#divfrm'));
            var wbid = $('select[name=WB_ID] option:selected').val();
            $('select[name=WB_ID]').removeAttr('disabled');
            GetUserGroupByWBID(wbid); //根据网点类型判定要加载的营业员类型
            $('#trAdd').fadeIn("fast");
            $('#trUpdate').fadeOut("fast");
            $('input[name=strRealName]').val('');
            $('input[name=strLoginName]').val('');
            $('input[name=strPassword]').val('');
            $('input[name=strPhone]').val('');
            $('input[name=strAddress]').val('');
            $('input[name=numLimitAmount]').val('');
            $('input[name=numLimitAmount_sell]').val('');
            $('input[name=numLimitAmount_shopping]').val('');
            $('input[name=numPrint]').val('');
            $('select[name=ISEnable]').val('1');
        }

        function ShowFrmEdit(id, wbid, UserGroup_ID) {
            showBodyCenter($('#divfrm'));
            $('#ID').val(id);
            $('select[name=WB_ID]').attr('disabled', 'disabled');
            $('#UserGroup_ID').val(UserGroup_ID);
            //if (UserGroup_ID == "单位管理员") {
            //    $('select[name=UserGroup_ID]').attr('disabled', 'disabled');
            //} else {
            //    $('select[name=UserGroup_ID]').removeAttr('disabled');
            //}
            $('select[name=UserGroup_ID]').attr('disabled', 'disabled');
          
            GetUserGroupByWBID(wbid); //根据网点类型判定要加载的营业员类型

            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");
            
            /*--------数据提交--------*/
            $.ajax({
                url: '/Admin/UserControl/user.ashx?type=GetUserByID&ID=' + id,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    $("select[name=WB_ID]  option[value='" + r[0].WB_ID + "'] ").attr("selected", 'selected');

                    $('input[name=strRealName]').val(r[0].strRealName);
                    $('input[name=strLoginName]').val(r[0].strLoginName);
                    //                         $('input[name=strPassword]').val(r[0].strPassword);
                    //                         $('input[name=strPassword2]').val(r[0].strPassword);
                    $('input[name=strPhone]').val(r[0].strPhone);
                    $('input[name=strAddress]').val(r[0].strAddress);
                    $('input[name=numLimitAmount]').val(r[0].numLimitAmount);
                    $('input[name=numLimitAmount_sell]').val(r[0].numLimitAmount_sell);
                    $('input[name=numLimitAmount_shopping]').val(r[0].numLimitAmount_shopping);
                    $('input[name=numPrint]').val(r[0].numPrint);

                    // $("select[name=UserGroup_ID]option[value='" + r[0].UserGroup_ID + "'] ").attr("selected", 'selected');
                    $("select[name=UserGroup_ID]").val(r[0].UserGroup_ID);
                    if (r[0].ISEnable) {
                        $("select[name=ISEnable]  option[value='1'] ").attr("selected", 'selected');
                    } else {
                        $("select[name=ISEnable]  option[value='0'] ").attr("selected", 'selected');
                    }
                }
            });
        }

        function CloseFrm() {
            $('#divfrm').fadeOut("normal");
        }

        //添加、修改网点 （ID=0是添加网点）
        function WBAdd() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                    var strurl = '/Admin/UserControl/user.ashx?type=AddUser&ID=0'
                    if ($('input[name=strPassword]').val() == "") {
                        showMsg('请输入密码 ！');
                        $('input[name=strPassword]').focus();
                        return false;
                    }
                    if ($('input[name=strPassword2]').val() == "") {
                        showMsg('请输入确认密码 ！');
                        $('input[name=strPassword2]').focus();
                        return false;
                    }
                    if ($('input[name=strPassword]').val() != $('input[name=strPassword2]').val()) {
                        showMsg('两次输入的密码不一致，请检查 ！');
                        $('input[name=strPassword2]').focus();
                        return false;
                    }
                    /*--------数据提交--------*/
                    $.ajax({
                        url: strurl,
                        type: 'post',
                        data: $('#form1').serialize(),
                        dataType: 'text',
                        success: function (r) {
                            if (r == "OK") {
                                showMsg('添加网点营业员成功 ！');
                                location.reload();
                            } else if (r == "1") {
                                showMsg('已存在相同的用户登陆名，请修改后添加 ！');
                            }
                        }, error: function (r) {
                            showMsg('添加营业员失败 ！');
                        }
                    });
                } else {
                    //console.log('你点击了取消！');
                }

            });
        }

        //添加、修改网点 （ID=0是添加网点）
        function WBUpdate() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
          
            var ID = $('#ID').val();
            var UserGroup_ID = $('select[name=UserGroup_ID]').val();
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                   var strurl = '/Admin/UserControl/user.ashx?type=UpdateUser&ID=' + ID + '&UserGroup_ID='+UserGroup_ID;
                    //var strurl = '/Admin/UserControl/user.ashx?type=UpdateUser&ID=' + ID ;
                    /*--------数据提交--------*/
                    $.ajax({
                        url: strurl,
                        type: 'post',
                        data: $('#form1').serialize(),
                        dataType: 'text',
                        success: function (r) {
                            if (r == "OK") {
                                showMsg('更新营业员信息成功 ！');
                                $('#divfrm').fadeOut();
                            } else if (r == "1") {
                                showMsg('已存在相同的用户登陆名 ！');
                            }
                        }, error: function (r) {
                            showMsg('更新营业员信息失败 ！');
                        }
                    });
                } else {
                    //console.log('你点击了取消！');
                }

            });
        }

        //提交检测
        function SubmitCheck() {
            if ($('input[name=strRealName]').val() == "") {
               showMsg('请输入真实姓名 ！');
                $('input[name=strRealName]').focus();
                return false;
            }
            if ($('input[name=strLoginName]').val() == "") {
               showMsg('请输入登陆名 ！');
                $('input[name=strLoginName]').focus();
                return false;
            }
         
            if (!CheckInput('strPhone', '手机号', -1)) {
                return false;
            }
            if (!CheckInput('strAddress', '联系地址', -1)) {
                return false;
            }
            
            if (isNaN($('input[name=numLimitAmount]').val()) || $.trim( $('input[name=numLimitAmount]').val())=='') {
               showMsg('兑换额度请输入数字 ！');
                $('input[name=numLimitAmount]').focus();
                return false;
            }
            if (isNaN($('input[name=numLimitAmount_sell]').val()) || $.trim($('input[name=numLimitAmount_sell]').val()) == '') {
                showMsg('存转销额度请输入数字 ！');
                $('input[name=numLimitAmount_sell]').focus();
                return false;
            }
            if (isNaN($('input[name=numLimitAmount_shopping]').val()) || $.trim($('input[name=numLimitAmount_shopping]').val()) == '') {
                showMsg('产品换购额度请输入数字 ！');
                $('input[name=numLimitAmount_shopping]').focus();
                return false;
            }
            if (isNaN($('input[name=numPrint]').val()) || $.trim($('input[name=numPrint]').val()) == '') {
                showMsg('小票打印次数请输入数字 ！');
                $('input[name=numPrint]').focus();
                return false;
            }
            return true;

        }
      
        //打开菜单编辑窗口
        function FunMenu(ID, UserGroup_ID) {

            var numtype = 1;
            if (UserGroup_ID == "单位管理员") {
                return false;
            }
            else if (UserGroup_ID == "营业员") {
             numtype = 2;
            }
           
            $('#UserID').val(ID);
            $('#numtype').val(numtype);

            var url = 'user.ashx?type=getMenuAdminAll&PID=0&UserID=' + $('#UserID').val();
            if (numtype == 2) {
                url = 'user.ashx?type=getMenuUserAll&PID=0&UserID=' + $('#UserID').val();
            }

            $.ajax({
                url: url,
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                    //$('#divMenu').append(r);
                    $('#divMenu').html('').html(r);
                    $('.usermenu').fadeOut();
                    showBodyCenter($('.usermenu'));
                },
                error: function (r) {
                    $('.usermenu').fadeOut();
                    showMsg(" 加载信息失败……");
                }

            });
        }

        function CloseMenuFrm() {
            $('.usermenu').fadeOut();
        }

        function FunDel(ID, UserGroup_ID) {
            var msg = '您确认要删除此营业员/管理员吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {

                    $.ajax({
                        url: '/Admin/UserControl/user.ashx?type=DeleteUserByID&ID=' + ID + '&UserGroup_ID=' + UserGroup_ID,
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



        /*
        PID:父节点id
        imgID：父节点左侧图片的id
        tabID：父节点table的id
        numLevel:这是第几级菜单
        */
        function OpenChildMenu(PID, imgID, tabID, numLevel) {

            if ($('#' + imgID).attr('alt') == "Open") {
                $('#' + imgID).attr('alt', 'Close');
                $('#' + imgID).attr('src', '../../images/menuClose.png');
                $('#div_' + PID).slideDown("normal");

            } else {
                $('#' + imgID).attr('alt', 'Open');
                $('#' + imgID).attr('src', '../../images/menuOpen.png');
                $('#div_' + PID).slideUp("normal");
            }
        }

        function frmSubmit() {
            var url = 'user.ashx?type=updateMenuAdmin&UserID=' + $('#UserID').val();
            if ($('#numtype').val() == 2) {
                url = 'user.ashx?type=updateMenuUser&UserID=' + $('#UserID').val();
            }

            $.ajax({
                url: url,
                type: 'post',
                data: $('#form1').serialize(),
                dataType: 'text',
                success: function (r) {
                    showMsg("提交成功!");
                    $('.usermenu').fadeOut();
                }, error: function (r) { showMsg("提交失败!"); }
            });
        };


        function FunQuanXuan() {
            $(":checkbox").each(function (i) {
                $(this).attr('checked', 'checked');
            });
        }

        function FunQuanBuXuan() {
            $(":checkbox").each(function (i) {
                $(this).removeAttr('checked');
            });
        }

        function FunFanXuan() {
            $(":checkbox").each(function (i) {
                if ($(this).attr("checked")) {
                    $(this).removeAttr('checked');
                } else {
                    $(this).attr('checked', 'checked');
                }
            });

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <div class="pageHead">
            <b>营业员管理</b>
        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>按营业员名称查询:</span></td>
                    <td><span>
                        <input type="text" id="txtType" runat="server" />
                    </span></td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="~/images/search_red.png" OnClick="ImageButton1_Click" />
                    </td>
                    <td><%=GetAddItem() %></td>
                </tr>

            </table>
        </div>

        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" style="width: 900px">
                    <tr class="tr_head">
                        <th style="width: 100px; text-align: center;">网点名称</th>
                        <th style="width: 100px; text-align: center;">营业员编号</th>
                        <th style="width: 100px; text-align: center;">真实姓名</th>
                        <th style="width: 100px; text-align: center;">登录名</th>
                        <th style="width: 100px; text-align: center;">级别</th>
                        <th style="width: 100px; text-align: center;">状态</th>
                        <th style="width: 120px; text-align: center;">查看/修改</th>
                        <th style="width: 120px; text-align: center;">删除</th>
                        <th style="width: 120px; text-align: center;">权限管理</th>

                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td><%#Eval("WBName")%></td>
                    <td><%#Eval("SerialNumber")%></td>
                    <td><%#Eval("strRealName")%></td>
                    <td><%#Eval("strLoginName")%></td>
                    <td><%#Eval("UserGroup_ID")%></td>
                    <td><%#Eval("ISEnable")%></td>
                    <td>
                        <%#GetUpdateItem(Eval("ID"),Eval("WBID"),Eval("UserGroup_ID")) %>
                    </td>
                    <td>
                        <%#GetDelItem(Eval("ID"),Eval("WBID"),Eval("UserGroup_ID")) %>
                    </td>
                    <td>
                        <%#GetAnthorityItem(Eval("ID"),Eval("UserGroup_ID")) %>
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
            NumericButtonTextFormatString="[{0}]" PageSize="15" OnPageChanging="AspNetPager1_PageChanging">
        </webdiyer:AspNetPager>


        <div id="divfrm" class="pageEidt" style="display: none;">
            <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="CloseFrm()" />

            <div style="clear: both; padding: 5px 10px;">
                <table class="tabEdit">

                    <tr>
                        <td align="right" style="width: 120px;"><span>所属网点:</span></td>
                        <td>
                            <select name="WB_ID" style="width: 160px; height: 25px"></select></td>
                    </tr>
                    <tr>
                        <td align="right"><span>操作级别:</span></td>
                        <td>
                            <select name="UserGroup_ID" style="width: 160px; height: 25px"></select></td>
                    </tr>
                    <tr>
                        <td align="right"><span>真实姓名:</span></td>
                        <td>
                            <input type="text" style="width: 150px;" name="strRealName" />
                            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">必填</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>登录名:</span></td>
                        <td>
                            <input type="text" style="width: 150px;" name="strLoginName" />
                            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">必填</span></td>
                    </tr>
                    <tr>
                        <td align="right"><span>登录密码:</span></td>
                        <td>
                            <input type="password" style="width: 150px;" name="strPassword" />
                            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">英文与数字，至少四位</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>确认密码:</span></td>
                        <td>
                            <input type="password" style="width: 150px;" name="strPassword2" />
                            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">再次输入密码</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>手机:</span></td>
                        <td>
                            <input type="text" style="width: 150px;" name="strPhone" />
                            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">必填</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>住址:</span></td>
                        <td>
                            <input type="text" style="width: 240px;" name="strAddress" />
                            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">必填</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>兑换限额:</span></td>
                        <td>
                            <input type="text" style="width: 100px;" name="numLimitAmount" />元
            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">必填</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>存转销限额:</span></td>
                        <td>
                            <input type="text" style="width: 100px;" name="numLimitAmount_sell" />元
            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">必填</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>产品换购限额:</span></td>
                        <td>
                            <input type="text" style="width: 100px;" name="numLimitAmount_shopping" />元
            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">必填</span>
                        </td>
                    </tr>
                     <tr>
                        <td align="right"><span>小票打印次数:</span></td>
                        <td>
                            <input type="text" style="width: 100px;" name="numPrint" />次
            <span style="color: Red; font-weight: bolder;">*</span><span style="font-size: 12px;">必填</span>
                        </td>
                    </tr>

                    <tr>
                        <td align="right"><span>使用状态:</span></td>
                        <td>
                            <select name="ISEnable" style="width: 110px; height: 25px">
                                <option value="1" selected="selected">启用</option>
                                <option value="0">禁用</option>
                            </select></td>
                    </tr>
                    <tr id="trAdd">
                        <td></td>
                        <td>
                            <input type="button" id="btnAdd" value="添加" onclick="WBAdd()" />
                        </td>
                    </tr>
                    <tr id="trUpdate">
                        <td></td>
                        <td>
                            <input type="button" id="btnUpdate" value="修改" style="width: 80px;" onclick="WBUpdate()" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="usermenu">
           <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="CloseMenuFrm()" />
            <div id="divMenu">
            </div>
            <div class="usermenu-action">
             <a href="#" onclick="FunQuanXuan()">全选</a>
            <a href="#" onclick="FunQuanBuXuan()">全不选</a>
            <a href="#" onclick="FunFanXuan()">反选</a>
            <input  type="button" value="提交" onclick="frmSubmit()" />
                 <input  type="button" value="取消" onclick="CloseMenuFrm()" />
            </div>
        </div>
    </form>
    <div style="display:none;">
         <%--定义编号--%>
    <input type="hidden" id="ID" value="" />
    <input type="hidden" id="UserGroup_ID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />

     <input type="text" id="numtype" value="" />
 <input type="text" id="UserID" value="" />
    </div>
   
</body>
</html>
