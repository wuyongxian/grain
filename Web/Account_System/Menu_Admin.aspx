<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menu_Admin.aspx.cs" Inherits="Web.Account_System.Menu_Admin" %>
<%@ Register Src="~/Account_System/SystemIndex.ascx" TagName="SysIndex" TagPrefix="sIndex"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>管理员菜单管理</title>
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
     <script src="../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <link href="../Styles/Common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">

        select {
        width:50px;}
        table.border
        {
            border-collapse: collapse;
            border: 1px solid #9CF;
            text-align: center;
        }
        table.border thead td, table.set_border th
        {
            font-weight: bold;
            background-color: White;
        }
        table.border tr:nth-child(even)
        {
            background-color: #EAF2D3;
        }
        table.border td, table.border th
        {
            border: 1px solid #C3C3C3;
            text-align: center;
        }
        
        #divADDMenu
        {
            width: 400px;
            height: 300px;
            position: absolute;
            left: 200px;
            top: 50px;
            background-color: #ccc;
            border-radius: 10px;
            z-index: 1000;
        }
        .tabAddMenu
        {
            margin: 30px 50px 10px 50px;
            font-size: 18px;
        }
        .tabAddMenu tr td
        {
            height: 50px;
        }
        .tabAddMenu tr td input
        {
            border-radius: 2px;
            padding-left: 10px;
            font-size: 18px;
            width: 300px;
        }
        #divClearDB
        {
           margin-left:300px;
           background:#aaa;
           padding:10px;
           border-radius:10px;
           width:200px;
           height:50px;
          display:none;
            }


          #divCompanyInfo {
                position: fixed;
                left:20px;
                top:20px;
                width:300px;
                height:200px;
                display: none;
                border: 2px solid #666;
                border-radius: 10px;
                background: #9ac0cd;
                padding: 20px 20px;
            }

    </style>
    <script type="text/javascript">
        $(function () {

            $.ajax({
                url: 'Menu_Admin.ashx?type=GetMenu&PID=0',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    for (var i = 0; i < r.length; i++) {
                        var m_ID = r[i].ID;
                        var m_PID = r[i].PID;
                        var m_strValue = r[i].strValue;
                        var m_strUrl = r[i].strUrl;
                        var m_numSort = r[i].numSort;
                        var m_ISEnable = r[i].ISEnable;
                        var m_ISSysW = r[i].ISSysW;

                        var innerValue = "<table class='border' id='tabMenu" + m_ID + "' style='width:1200px; margin:10px;'><tr>";
                        if (r[i].HasChild != "0") {
                            innerValue += "<td style='width:50px;'><img id='img_" + m_ID + "' alt='Open' src='../images/open.jpg' onclick='GetChildMenu(" + m_ID + ",\"img_" + m_ID + "\",\"tabMenu" + m_ID + "\",0)' /></td>";
                        } else {
                            innerValue += "<td style='width:50px;'><td>"
                        }
                        innerValue += "<td >名称：<input style='width:150px;' type='text' name='txtValue_" + m_ID + "' value='" + m_strValue + "'/></td>";
                        innerValue += "<td >链接：<input style='width:300px;' type='text' name='txtUrl_" + m_ID + "' value='" + m_strUrl + "'/></td>";
                        innerValue += "<td >排序：<input style='width:40px;' type='text' name='txtSort_" + m_ID + "' value='" + m_numSort + "'/></td>";
                        if (m_ISEnable) {
                            innerValue += "<td ><select name='selectEnable_" + m_ID + "'><option selected='selected'>启用</option><option >禁用</option></select></td>";
                        } else {
                            innerValue += "<td ><select name='selectEnable_" + m_ID + "'><option selected='selected'>禁用</option><option >启用</option></select></td>";
                        }
                        if (m_ISSysW) {
                            innerValue += "<td >开放权限给网点管理员<select name='selectAuthority_" + m_ID + "'><option selected='selected'>是</option><option >否</option></select></td>";
                        } else {
                            innerValue += "<td >开放权限给网点管理员<select name='selectAuthority_" + m_ID + "'><option selected='selected'>否</option><option >是</option></select></td>";
                        }
                        innerValue += "<td ><input style='width:100px;' type='button' name='btnAdd_" + m_ID + "' value='添加子菜单'  onclick='ShowMenuAdd(" + m_ID + ")'/></td>";

                        innerValue += "<td ><input style='width:100px;' type='button' name='btndel_" + m_ID + "' value='删除菜单'onclick='DeleteMenu(" + m_ID + ")' /></td>";

                        innerValue += "</tr></table>";
                        $('#divMenu').append(innerValue);

                    }
                   
                },
                error: function (r) { showMsg(" 加载信息失败……"); }

            });

        });



        /*
        PID:父节点id
        imgID：父节点左侧图片的id
        tabID：父节点table的id
        numLevel:这是第几级菜单
        */
        function GetChildMenu(PID,imgID,tabID,numLevel) {
            
            if ($('#' + imgID).attr('alt') == "Open") {
                $('#' + imgID).attr('alt', 'Close');
               
                if ($('#div_' + PID).length <= 0) {//由div的长度判断该div是否存在
                    /*--------获取menuData--------*/
                    $.ajax({
                        url: 'Menu_Admin.ashx?type=GetMenu&PID=' + PID,
                        type: 'post',
                        data: '',
                        dataType: 'json',
                        success: function (r) {
                            var innerValue = "<div id='div_" + PID + "'>";
                            for (var i = 0; i < r.length; i++) {

                                var m_ID = r[i].ID;
                                var m_PID = r[i].PID;
                                var m_strValue = r[i].strValue;
                                var m_strUrl = r[i].strUrl;
                                var m_numSort = r[i].numSort;
                                var m_ISEnable = r[i].ISEnable;
                                var m_ISSysW = r[i].ISSysW;
                                if (numLevel == 0) {
                                    innerValue += "<table class='border'  id='tabMenu" + m_ID + "' style='width:1000px; margin:10px 10px 10px 60px;'><tr>";
                                }
                                else {
                                    innerValue += "<table class='border'  id='tabMenu" + m_ID + "' style='width:1000px; margin:10px 10px 10px 100px;'><tr>";
                                }
                                //  innerValue += "<td style='width:50px;'><td>"
                                if (r[i].HasChild != "0") {
                                    innerValue += "<td style='width:50px;'><img id='img_" + m_ID + "' alt='Open' src='../images/open.jpg' onclick='GetChildMenu(" + m_ID + ",\"img_" + m_ID + "\",\"tabMenu" + m_ID + "\",1)' /></td>";
                                } else {
                                    innerValue += "<td style='width:50px;'><td>"
                                }
                                innerValue += "<td ><input style='width:150px;' type='text' name='txtValue_" + m_ID + "' value='" + m_strValue + "'/></td>";
                                innerValue += "<td ><input style='width:300px;' type='text' name='txtUrl_" + m_ID + "' value='" + m_strUrl + "'/></td>";
                                innerValue += "<td ><input style='width:40px;' type='text' name='txtSort_" + m_ID + "' value='" + m_numSort + "'/></td>";
                                if (m_ISEnable) {
                                    innerValue += "<td ><select name='selectEnable_" + m_ID + "'><option selected='selected'>启用</option><option >禁用</option></select></td>";
                                } else {
                                    innerValue += "<td ><select name='selectEnable_" + m_ID + "'><option selected='selected'>禁用</option><option >启用</option></select></td>";
                                }
                                if (m_ISSysW) {
                                    innerValue += "<td >开放权限给网点管理员<select name='selectAuthority_" + m_ID + "'><option selected='selected'>是</option><option >否</option></select></td>";
                                } else {
                                    innerValue += "<td >开放权限给网点管理员<select name='selectAuthority_" + m_ID + "'><option selected='selected'>否</option><option >是</option></select></td>";
                                }
                                innerValue += "<td ><input style='width:100px;' type='button' name='btnAdd_" + m_ID + "' value='添加子菜单'  onclick='ShowMenuAdd(" + m_ID + ")'/></td>";

                                innerValue += "<td ><input style='width:100px;' type='button' name='btndel_" + m_ID + "' value='删除菜单'onclick='DeleteMenu(" + m_ID + ")' /></td>";

                                innerValue += "</tr></table>";
                            }
                            innerValue += "</div>";
                            $('#' + tabID).after(innerValue);
                        },
                        error: function (r) { showMsg('加载数据错误……'); }
                    });
                    /*--------End 获取menuData--------*/
                } else {
                $('#div_' + PID).slideDown("normal");
                }
             
            } else {
            $('#' + imgID).attr('alt', 'Open');
            $('#div_' + PID).slideUp("normal");
            }
    }

    function frmSubmit() {
        $.ajax({
            url: 'Menu_Admin.ashx?type=frmsubmit',
            type: 'post',
            data: $('#form1').serialize(),
            dataType: 'text',
            success: function (r) { showMsg("提交成功!"); }, error: function (r) { showMsg("提交失败!"); }
        });
    };
    var M_PID;//当前操作的菜单ID
    //打开menu添加的div
    function ShowMenuAdd(PID) {
         M_PID=PID;
         $('#divADDMenu').fadeIn("normal");
         $('#txtValue').val('');
         $('#txtUrl').val('');
         $('#txtSort').val('1');
    }

    // 关闭menu添加的div 
    function frmClose() {
        $('#divADDMenu').fadeOut("normal");
    }

    //添加menu方法 
    function FunAddMenu() {
        if ($('#txtValue').val() == "") {
            showMsg("请输入菜单名称！");
            return;
        }

        var re = /^[0-9]+.?[0-9]*$/;   //判断字符串是否为数字     //判断正整数 /^[1-9]+[0-9]*]*$/  
        if (!re.test($('#txtSort').val())) {
            showMsg("排序文本框中请输入整数！");
            return;
        }
        $.ajax({
            url: 'Menu_Admin.ashx?type=AddMenu&PID=' + M_PID,
            type: 'post',
            data: "['strValue':'" + $('#txtValue').val() + "','strUrl':'" + $('#txtUrl').val() + "','numSort':'" + $('#txtSort').val() + "']",
            dataType: 'text',
            success: function (r) {
                showMsg("添加菜单成功！");
            }, error: function (r) {
                showMsg("添加菜单失败！");
            }
        });
    }

    function DeleteMenu(mID) {
        var msg = '您确认要删除此节点吗？';
        showConfirm(msg, function (obj) {
            if (obj == 'yes') {
                
                $.ajax({
                    url: 'Menu_Admin.ashx?type=DeleteMenu&ID=' + mID,
                    type: 'post',
                    data: '',
                    dataType: 'text',
                    success: function (r) {
                        showMsg("删除菜单成功！");
                    }, error: function (r) {
                        showMsg("删除菜单失败！");
                    }
                });
            } else {
                //console.log('你点击了取消！');
            }

        });
    }

    function ShowClearDB() {
        var msg = '您确定要清空数据库中的所有数据吗?';
        showConfirm(msg, function (obj) {
            if (obj == 'yes') {
                
                $('#divClearDB').show();
            } else {
                //console.log('你点击了取消！');
            }

        });
    }
    function CloseClearDB(){
        $('#divClearDB').hide();
    }
    function ClearDB() {
        $.ajax({
            url: 'Menu_Admin.ashx?type=ClearDB&strPassword=' + $('#txtPassword').val(),
            type: 'post',
            data: '',
            dataType: 'text',
            success: function (r) {
              
                if (r == "ErrorPassword"||r=="") {
                    showMsg("密码输入错误！");
                }
                else if (r == "OK") {
                    showMsg("清空数据库成功！");
                    $('#divClearDB').hide();
                }
                else {
                    showMsg("清空数据库失败！");
                }
               
            }, error: function (r) {
                showMsg("清空数据库失败！");
                
            }
        });
    }


    function ShowCompanyInfo() {
        var width = 320;
        var height = 150;
        var top = (document.body.scrollHeight - height) / 2;
        var left = (document.body.scrollWidth - width) / 2;
        $('#divCompanyInfo').css('width', width).css('height', height).css('top', top).css('left', left);
        $('#divCompanyInfo').show();
        var url = 'Menu_Admin.ashx?type=GetCompanyInfo';
        //显示内容
        $.ajax({
            url: url,
            type: 'post',
            data: '',
            dataType: 'text',
            success: function (r) {
                var msg = JSON.parse(r);
                if (r != "Error") {
                    $('#c_strName').val(msg[0].strName);
                    $('#c_pushmsgApiurl').val(msg[0].pushmsgApiurl);
                    $('#c_webSiteCode').val(msg[0].webSiteCode);
                } else { showMsg("获取信息失败！"); }

            }, error: function (r) {
                showMsg("获取信息失败！");
            }
        });

        $('#noticeclose').click(function () {
            $('#divCompanyInfo').hide();
        });
    }
    

    function saveCompanyInfo() {
        var strName = $('#c_strName').val();
        var pushmsgApiurl = $('#c_pushmsgApiurl').val();
        var webSiteCode = $('#c_webSiteCode').val();
        var url = 'Menu_Admin.ashx?type=SaveCompanyInfo&strName=' + strName + '&pushmsgApiurl=' + pushmsgApiurl + '&webSiteCode=' + webSiteCode;
        $.ajax({
            url: url,
            type: 'post',
            data: '',
            dataType: 'text',
            success: function (r) {
                if (r == "OK") {
                    showMsg("保存信息成功 ！");
                } else { showMsg("保存信息失败！"); }

            }, error: function (r) {
                showMsg("保存信息失败！");
            }
        });
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <sIndex:SysIndex ID="sysindex1" runat="server" />
    </div>
   
     <div id="divMenu">
    
    </div>
   <div >
    <input style="margin-left:100px;" type="button" value="添加节点" onclick="ShowMenuAdd(0)" />
    <input style="margin-left:50px;" type="button" value="提交" onclick="frmSubmit()" />&nbsp;&nbsp;
      <input style="margin-left:50px;" type="button" value="清空数据库" onclick="ShowClearDB()" />
       <input style="margin-left:50px;" type="button" value="推送设置" onclick="ShowCompanyInfo()" />
   </div>
    <div id="divClearDB">
        <span>请输入密码:</span>
        <input type="text" id="txtPassword" style="width: 120px; font-size: large" /><br />
        <input type="button" onclick="ClearDB()" value="确认" />
        &nbsp; &nbsp;
        <input type="button" onclick="CloseClearDB()" value="关闭" />
    </div>
       <div id="divCompanyInfo">
           
        <table>
            <tr><td align="right" style="width:120px;"><span>粮食银行名称：</span></td>
                <td style="width:150px;"><input type="text" id="c_strName" /></td>
            </tr>
            <tr><td align="right" style="width:120px;"><span>粮食银行网址：</span></td>
                <td style="width:150px;"><input type="text" id="c_pushmsgApiurl" /></td>
            </tr>
            <tr><td align="right" style="width:120px;"><span>粮食银行代码：</span></td>
                <td style="width:150px;"><input type="text" id="c_webSiteCode" /></td>
            </tr>
            <tr><td align="right" style="width:120px;"></td>
                <td style="width:150px;"><input type="button" style="width:80px;" id="btnCompanyInfo" value="保存" onclick="saveCompanyInfo()" />
                     &nbsp;<input type="button" style="width:80px;" id="noticeclose" value="取消"  />
                </td>
               
              
            </tr>
        </table>

       </div>
   <div id="divADDMenu" style="display:none">
   <div style="float:right; margin:5px 10px">
   <input type="button" value="关闭" onclick="frmClose();" />
   </div>
       <table class="tabAddMenu">
        <tr><td><span>名称:</span><input type="text" id="txtValue" /></td></tr>
           <tr><td><span>网址:</span><input type="text" id="txtUrl" /></td></tr>
              <tr><td><span>排序:</span><input type="text" id="txtSort" value="1" /></td></tr>
       </table>
       <input style="margin-left:50px; font-size:16px; width:100px; height:30px" type="button" value="添加" onclick="FunAddMenu()" />
   </div>

    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
  
    </form>
</body>
</html>
