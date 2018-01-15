<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Address_Zu.aspx.cs" Inherits="Web.BasicData.Data.Address_Zu" %>
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

            //编辑列表选项
            InitXianID(); //加载县列表

            $('select[name=XianID]').change(function () {
                var XianID = $('select[name=XianID] option:selected').val();
                InitXiangID(XianID);//根据选择县名加载乡名
            });

            $('select[name=XiangID]').change(function () {
                var XiangID = $('select[name=XiangID] option:selected').val();
                InitCunID(XiangID); //根据选择乡名加载组名
            });

        });


        function InitXianID() {
            $('select[name=XianID]').empty();
            /*--在县名加载完成后，根据县名加载乡名--*/
            $.ajax({
                url: 'addr.ashx?type=Get_Address_Xian',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    var XianID = r[0].ID;
                    InitXiangID(XianID); //根据乡名加载村名
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=XianID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                }, error: function (r) {
                   showMsg('加载县信息失败 ！');
                }
            });
        }



        function InitXiangID(XianID,XiangID,CunID) {
            $('select[name=XiangID]').empty();
            /*--在县名加载完成后，根据县名加载乡名--*/
            $.ajax({
                url: 'addr.ashx?type=Get_Address_Xiang&XianID=' + XianID,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    if (!isNull(XiangID)) {
                        if (!isNull(CunID)) {
                            InitCunID(XiangID,CunID);//根据乡名加载村名
                        } else {
                            InitCunID(XiangID);//根据乡名加载村名
                        }
                        
                    } else {
                        InitCunID(r[0].ID);//根据乡名加载村名
                    }
                    for (var i = 0; i < r.length; i++) {
                        if (!isNull(XiangID)) {
                            if (XiangID == r[i].ID) {
                                $('select[name=XiangID]').append("<option value='" + r[i].ID + "' selected='selected'>" + r[i].strName + "</option>");
                            } else {
                                $('select[name=XiangID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                            }
                        } else {
                            $('select[name=XiangID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                        }
                     
                    }
                }, error: function (r) {
                   showMsg('加载乡信息失败 ！');
                }
            });
        }

        function InitCunID(XiangID,CunID) {
            $('select[name=CunID]').empty();
            /*--在县名加载完成后，根据县名加载乡名--*/
            $.ajax({
                url: 'addr.ashx?type=Get_Address_Cun&XiangID=' + XiangID,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    for (var i = 0; i < r.length; i++) {
                        if (!isNull(CunID)) {
                            if (CunID == r[i].ID) {
                                $('select[name=CunID]').append("<option value='" + r[i].ID + "' selected='selected'>" + r[i].strName + "</option>");
                            } else {
                                $('select[name=CunID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                            }
                        } else {
                            $('select[name=CunID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                        }
                    }
                }, error: function (r) {
                   showMsg('加载村信息失败 ！');
                }
            });
        }


        /*--------End  窗体启动设置和基本设置--------*/


        /*--------自定义div窗口的开启和关闭--------*/
        //显示新增数据窗口
        function ShowFrm_Add(wbid) {
            $('#trAdd').fadeIn("fast");
            $('#trUpdate').fadeOut("fast");
            $("select[name=XianID]").removeAttr('disabled');
            $("select[name=XiangID]").removeAttr('disabled');
            $("select[name=CunID]").removeAttr('disabled');
            $("select[name=XianID]").get(0).selectedIndex = 0; //设置索引为1的项选中
            $("select[name=XiangID]").get(0).selectedIndex = 0;
            $("select[name=CunID]").get(0).selectedIndex = 0;

            $('input[name=strName]').val('');
        }
        //显示更新数据窗口
        function ShowFrm_Update(wbid) {
            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");

            /*----数据提交----*/
            $.ajax({
                url: 'addr.ashx?type=GetByID_Address_Zu&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $("select[name=XianID]").attr('disabled', 'disabled');
                    $("select[name=XiangID]").attr('disabled', 'disabled');
                    $("select[name=CunID]").attr('disabled', 'disabled');
                    $("select[name=XianID]  option[value='" + r[0].XianID + "'] ").attr("selected", 'selected');
                    //$("select[name=XiangID]  option[value='" + r[0].XiangID + "'] ").attr("selected", 'selected');
                    //$("select[name=CunID]  option[value='" + r[0].CunID + "'] ").attr("selected", 'selected');
                    InitXiangID(r[0].XianID, r[0].XiangID, r[0].CunID);
                    //InitCunID(r[0].XiangID, r[0].CunID);
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
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                    var CunID = $('select[name=CunID] option:selected').val();
                    $.ajax({
                        url: 'addr.ashx?type=Add_Address_Zu&CunID=' + CunID,
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
        //更新数据方法
        function FunUpdate() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var wbid = $('#WBID').val();
            var CunID = $('select[name=CunID]').val();
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                    $.ajax({
                        url: 'addr.ashx?type=Update_Address_Zu&ID=' + wbid + '&CunID=' + CunID,
                        type: 'post',
                        data: $('#form1').serialize(),
                        dataType: 'text',
                        success: function (r) {
                            if (r == "OK") {
                                showMsg('更新数据成功 ！');
                                CloseFrm();
                                location.reload();
                            } else if (r == "1") {
                                showMsg('已存在相同的类型名称，请修改后添加 ！');
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
            SingleDataDelete('addr.ashx?type=DeleteByID_Address_Zu&ID=' + wbid);
           
        }
        //提交检测
        function SubmitCheck() {
            if ($('select[name=XianID] option:selected').val() == "") {
               showMsg('县名不能为空 ！');
                return false;
            }
            if ($('select[name=XiangID] option:selected').text() == "") {
               showMsg('乡名不能为空 ！');
                return false;
            }
            if ($('input[name=strName]').val() == "") {
               showMsg('村名不能为空 ！');
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
            <b>组名管理</b>
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>县名:</span>
                    </td>
                    <td>
                        <input type="text" id="txtXian" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" /></td>
                    <td><span>乡名:</span>
                    </td>
                    <td>
                        <input type="text" id="txtXiang" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" /></td>

                    <td><span>村名:</span></td>
                    <td>
                        <input type="text" id="txtCun" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                    </td>
                    <td style="width: 60px">

                        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                            runat="server" OnClick="ImageButton1_Click" />
                    </td>
                    <td><%=GetAddItem() %></td>
                </tr>
            </table>
        </div>

        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 200px; text-align: center;">县
                        </th>
                        <th style="width: 200px; text-align: center;">乡
                        </th>
                        <th style="width: 200px; text-align: center;">村
                        </th>
                        <th style="width: 200px; text-align: center;">组
                        </th>
                        <th style="width: 200px; text-align: center;">查看/修改
                        </th>
                        <th style="width: 100px; text-align: center;">删除
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td>
                        <%#Eval("XianID")%>
                    </td>
                    <td>
                        <%#Eval("XiangID")%>
                    </td>
                    <td>
                        <%#Eval("CunID")%>
                    </td>
                    <td>
                        <%#Eval("strName")%>
                    </td>
                    <td><%# GetUpdateItem(Eval("ID")) %> </td>
                    <td><%# GetDeleteItem(Eval("ID")) %></td>
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

        <div class="divWarning">
            <b>特别提示:</b><span>这是系统关键数据，最好不要添加、修改、删除!</span>
        </div>
        <div id="divfrm" class="pageEidt" style="display: none;">
            <div style="float: right;">
                <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="CloseFrm()" />
            </div>
            <div style="clear: both;">
                <table class="tabEdit">
                    <tr>
                        <td style="width: 60px;">
                            <span>县名:</span>
                        </td>
                        <td>
                            <select name="XianID" style="width: 200px; height: 25px">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 60px;">
                            <span>乡名:</span>
                        </td>
                        <td>
                            <select name="XiangID" style="width: 200px; height: 25px">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 60px;">
                            <span>村名:</span>
                        </td>
                        <td>
                            <select name="CunID" style="width: 200px; height: 25px">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>组名:</span>
                        </td>
                        <td>
                            <input name="strName" type="text" style="width: 200px;" />
                        </td>
                    </tr>
                    <tr id="trAdd">
                        <td></td>
                        <td>
                            <input type="button" id="btnAdd" value="添加" onclick="FunAdd()" />
                        </td>
                    </tr>
                    <tr id="trUpdate">
                        <td></td>
                        <td>
                            <input type="button" id="btnUpdate" value="修改" onclick="FunUpdate()" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
    <div style="display: none;">
        <%--定义编号--%>
        <input type="hidden" id="WBID" value="" />
        <%--定义背景色的隐藏域--%>
        <input type="hidden" id="colorName" value="" />
    </div>

</body>
</html>