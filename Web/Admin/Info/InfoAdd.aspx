<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InfoAdd.aspx.cs" Inherits="Web.Admin.Info.InfoAdd" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
      <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../kindeditor/kindeditor-min.js" charset="utf-8"></script>
    
   
    <style type="text/css">

</style>
<script type="text/javascript">
    var editor;
    KindEditor.ready(function (K) {
        editor = K.create('textarea[name="strContent"]', {
            resizeType: 1,
            allowPreviewEmoticons: false,
            allowImageUpload: false,
            afterBlur: function () {
                this.sync();
            },
            items: [
						'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', 'bold', 'italic', 'underline',
						'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
						'insertunorderedlist', '|', 'emoticons']
        });
    });



    $(function () {
        InitSelect('/Admin/Info/info.ashx?type=Get_InfoType', 'InfoTypeID', '加载信息分类失败！'); //初始化信息类型
        //接受新建信息后跳转的信息
        var strUrl = getQueryString("AddInfo");
      
        if (strUrl != "") {
           showMsg('保存信息成功，您可以继续添加 ！');
        }
        //接受从详细信息界面跳转的修改信息
        var ID = getQueryString("ID");
       
        if (ID != "") {
            $('#WBID').val(ID);
            //加载初始信息
            $.ajax({
                url: '/Admin/Info/info.ashx?type=GetByID_Info&ID=' + ID,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('input[name=strTitle]').val(r[0].strTitle);
                    $('select[name=InfoTypeID]').val(r[0].InfoTypeID);

                    editor.html(r[0].strContent);
                    if (r[0].ISStick) {
                        $('input[name=ISStick]').attr("checked", "checked");
                    }
                    if (r[0].ISKeepSecret) {
                        $('input[name=ISKeepSecret]').attr("checked", "checked");
                    }
                }, error: function (r) {
                   showMsg('加载信息失败 ！');
                }
            });

        }



    });

    function SaveInfo() {
       
        if (!CheckSubmit()) {
            return false;
        }
        var ID = $('#WBID').val();
        if (ID == "") {
            $.ajax({
                url: '/Admin/Info/info.ashx?type=Add_Info',
                type: 'post',
                data: $('#form1').serialize(),
                dataType: 'text',
                success: function (r) {
                    // location.reload(); //重新加载此页面
                    window.location = 'InfoAdd.aspx?AddInfo=1';
                }, error: function (r) {
                   showMsg('新增信息失败 ！');
                }
            });
        } else {
            $.ajax({
                url: '/Admin/Info/info.ashx?type=Update_Info&ID='+ID,
                type: 'post',
                data: $('#form1').serialize(),
                dataType: 'text',
                success: function (r) {
                   showMsg('更新信息成功 ！');
                }, error: function (r) {
                   showMsg('更新信息失败 ！');
                }
            });
        }
    }

    function CheckSubmit() {
     
        if (!CheckInput('strTitle', "信息标题", -1)) {
            return false;
        }
        if ($('input[name=strTitle]').val().length > 100) {
           showMsg('信息标题最多输入100个字符 ！');
            return false;
        };
        if ($('select[name=InfoTypeID] option:selected').val() == "") {
           showMsg('请选择添加信息类型 ！');
            return false;
        }
        if ($('textarea[name=strContent]').val() == "") {
           showMsg('请输入信息内容 ！');
            return false;
        }
        if ($('textarea[name=strContent]').val().length > 4000) {
           showMsg('信息内容最多为4000个字,您输入的字符数目已超出限制 ！');
            return false;
        }
        return true;
    }

    
</script>
</head>
<body>
   <form id="form1" runat="server">
<div class="pageHead" style="width:300px;">
<b>设置公告信息</b>
</div>

   
    <div  id="divfrm" class="pageEidtInner" >
 
    <div>
        <table >
        <tr>
                <td ><span>信息标题</span>
                <input type="text" name="strTitle" style="width:400px;" />
                </td>
            </tr>
             <tr>
                <td ><span>信息分类</span>
              <select name="InfoTypeID" style="width:150px;"></select>
                </td>
            </tr>
            <tr>
                <td ><span>信息详细内容（最多4000字）</span></td>
            </tr>
             <tr>
                <td >
                <textarea id="txtContent" name="strContent" style="width:700px;height:200px;visibility:hidden;"></textarea>
                </td>
            </tr>
            <tr>
                <td >
               <span>是否置顶</span>
                <input type="checkbox" name="ISStick"  /><span>把信息放到最上面显示</span>
                </td>
            </tr>
            <tr>
                <td ><span>是否保密</span>
                <input type="checkbox" name="ISKeepSecret"  /><span>该信息只有自己可见</span>
                </td>
            </tr>
          
               <tr>
            <td ><input type="button" id="btnUpdate" value="保存信息" onclick="SaveInfo()" />
           
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
