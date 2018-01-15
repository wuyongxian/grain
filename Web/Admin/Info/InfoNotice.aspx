<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeBehind="InfoNotice.aspx.cs" Inherits="Web.Admin.Info.InfoNotice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
      <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js"></script>
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
        $('textarea[name=strContent]').html('asdf');
        $.ajax({
            url: '/Admin/Info/info.ashx?type=Get_InfoNotice',
            type: 'post',
            data: '',
            dataType: 'text',
            success: function (r) {
                editor.html(r);
            }, error: function (r) {
                showMsg("加载信息失败!");
            }
        });
    });

    function SaveInfo() {
       
        $.ajax({
            url: '/Admin/Info/info.ashx?type=Update_InfoNotice',
            type: 'post',
            data: $('#form1').serialize(),
            dataType: 'text',
            success: function (r) {
                showMsg("保存成功!");
            }, error: function (r) {
                showMsg("保存失败!");
            }
        });
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
                <td ><span>编辑公告信息（内容最多500字）</span></td>
            </tr>
             <tr>
                <td >
                <textarea id="txtContent" name="strContent" style="width:700px;height:200px;visibility:hidden;"></textarea>
                </td>
            </tr>
          
               <tr>
            <td ><input type="button" id="btnUpdate" value="保存公告设置" onclick="SaveInfo()" />
           
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