<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageUserType.aspx.cs" Inherits="Web.BasicData.StoragePara.StorageUserType" %>

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
        });

        function ShowFrm(wbid) {
            //$('#divfrm').fadeIn("normal");
            showBodyCenter($('#divfrm'));
            $('#WBID').val(wbid);
            if (wbid == "0") {
                $('#trAdd').fadeIn("fast");
                $('#trUpdate').fadeOut("fast");
                $('input[name=strName]').val('');
            }
            else { //编辑网点
                $('#trAdd').fadeOut("fast");
                $('#trUpdate').fadeIn("fast");

                /*--------数据提交--------*/
                $.ajax({
                    url: 'storage.ashx?type=GetStorageUserByID&ID=' + wbid,
                    type: 'post',
                    data: '',
                    dataType: 'json',
                    success: function (r) {
                        $('input[name=strName]').val(r[0].strName);
                        }, error: function (r) {
                       showMsg('加载信息失败 ！');
                    }
                });
                /*--------End 数据提交--------*/
            }
        };
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
                    
                    var strurl = 'storage.ashx?type=UpdateStorageUser&ID=' + wbid;
                    if (wbid == "0") {
                        strurl = 'storage.ashx?type=AddStorageUser&ID=' + wbid;
                    }
                    /*--------数据提交--------*/
                    $.ajax({
                        url: strurl,
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
                                showMsg('已存在相同的储户类型名称，请修改后添加 ！');
                            }
                        }, error: function (r) {
                            showMsg('操作失败 ！');
                        }
                    });
                } else {
                    //console.log('你点击了取消！');
                }

            });
        }

        //提交检测
        function SubmitCheck() {
            if ($('input[name=strName]').val() == "") {
               showMsg('请输入储户类型名称 ！');
                $('input[name=strName]').focus();
                return false;
            }
            return true;
        }


        function FunDelete(wbid) {
          
            SingleDataDelete('storage.ashx?type=DeleteStorageUserByID&ID=' + wbid);
          
        }
     
    </script>
</head>
<body>
     <form id="form1" runat="server">
<div class="pageHead">
<b>储户类型设置</b><%--<span id="spanHelp" style="cursor:pointer" >帮助</span>--%>
</div>
<div id="divHelp" style="border:1px solid #333; border-radius:5px; display:none; ">
<span>提示1：</span><br />
<span>提示2：</span><br />
<span>提示3：</span><br />
</div>

<div class="QueryHead">
<table>
            <tr>
            <td><b>已有储户类型列表</b></td>
            
            <td><%=GetAddItem() %></td>
            </tr>
            
        </table>
</div>
<asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table  class="tabData" style="width:900px">
          <tr class="tr_head" >
                <th style="width:300px; text-align:center;">
                    储户类型名称</th>
                
                    <th style="width:200px; text-align:center;">
                    查看/修改</th>
                   <th style="width:100px; text-align:center;">
                    删除</th>
                
            </tr>
        
    </HeaderTemplate>
    <ItemTemplate>
    <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
        <td><%#Eval("strName")%></td>
        
        <td> <%# GetUpdateItem(Eval("ID")) %> </td>
      <td> <%# GetDeleteItem(Eval("ID")) %></td>
   
    </tr>
    </ItemTemplate>
    
    <FooterTemplate><!--底部模板-->
        </table>        <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>
   <div class="divWarning">
   <b>特别提示:</b><span>这是系统关键数据，最好不要添加、修改、删除!</span>
   </div>
    <div  id="divfrm" class="pageEidt" style="display:none; ">
    <img  class="imgclose" src="../../images/winClose.png" alt="关闭窗口"  style="float:right; cursor:pointer;" onclick="CloseFrm()" />
    <div style="clear:both;">
        <table >
            
            <tr>
            <td style="width:120px;"><span>储户类型名称:</span></td>
            <td><input type="text" name="strName" style="width:150px; height:25px" /></td>
            </tr>
             
            <tr id="trAdd">
            <td></td>
            <td ><input type="button" id="btnAdd" value="添加" onclick="WBUpdate()" /> </td>
            </tr>
               <tr id="trUpdate">
            <td></td>
            <td ><input type="button" id="btnUpdate" value="修改" onclick="WBUpdate()" />
         
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

