<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InfoDetail.aspx.cs" Inherits="Web.Admin.Info.InfoDetail" %>

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
    .infoTitle{font-family:@Microsoft YaHei; text-align:left;  padding:5px 20px; width:150px; font-size:18px; font-weight:bolder; color:Green; background-color:#eee; border-radius:5px;}
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
        var ID = getQueryString("ID");
        if (ID != "") {

            $.ajax({
                url: '/Admin/Info/info.ashx?type=UpdateGetByID_Info&ID=' + ID,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('#WBID').val(r[0].ID); //保存信息编号

                    $('#strTitle').html(r[0].strTitle);
                    $('#UserID').html(r[0].UserID);
                    $('#dt_Add').html(r[0].dt_Add);
                    $('#BrowseTime').html(r[0].BrowseTime);
                    $('#strContent').html(r[0].strContent);
                }, error: function (r) {

                }
            });
        }

        //获取是否有信息列表
       
        $.ajax({
            url: '/Admin/Info/info.ashx?type=HasReply&ID=' + ID,
            type: 'post',
            data: '',
            dataType: 'text',
            success: function (r) {
                if (r == "0") {
                    $('#divReply').hide();
                } else {
                    $('#divReply').show();
                }
            }, error: function (r) {

            }
        });


    });

    function FunUpdate() {
        window.location = "InfoAdd.aspx?ID="+$('#WBID').val();
    }

    function FunDelete() {

       showMsg('您确认要删除这条信息吗？', {
            'type': 'question',
            'buttons': ['Yes', 'No'],
            'onClose': function (caption) {
                if (caption == 'Yes') {
                    $.ajax({
                        url: '/Admin/Info/info.ashx?type=DeleteByID_Info&ID=' + $('#WBID').val(),
                        type: 'post',
                        data: '',
                        dataType: 'text',
                        success: function (r) {
                            window.location = "InfoList.aspx";//删除成功后转向列表界面
                        }, error: function (r) {
                           showMsg('删除数据失败 ！');
                        }
                    });

                }
                else if (caption == "No") {
                }
            }
        });
        }


        function FunReply() {

        if (!CheckSubmit()) {
            return false;
        }
        $.ajax({
            url: '/Admin/Info/info.ashx?type=Add_Reply&ID=' + $("#WBID").val(),
            type: 'post',
            data: $('#form1').serialize(),
            dataType: 'text',
            success: function (r) {
                location.reload();
                // window.location = 'InfoAdd.aspx?AddInfo=1';
            }, error: function (r) {
               showMsg('回复信息失败 ！');
            }
        });
    }

    function CheckSubmit() {

        if ($('textarea[name=strContent]').val() == "") {
           showMsg('请输入信息内容 ！');
            return false;
        }
        if ($('textarea[name=strContent]').val().length > 1000) {
           showMsg('信息内容最多为1000个字,您输入的字符数目已超出限制 ！');
            return false;
        }
        return true;
    }

    
</script>
</head>
<body>
   <form id="form1" runat="server">

   <div>
       <table>
           <tr>
               <td style="text-align: center;">
                   <h2 id="strTitle">
                   </h2>
               </td>
           </tr>
           <tr>
               <td style="text-align: center; color: #666;">
                   作者:<span id="UserID"></span> 浏览次数:<span id="BrowseTime"></span> 添加时间:<span id="dt_Add"></span>&nbsp;&nbsp;
                  <%=GetEditContent()%>
               </td>
             <%--  <td id="tdEdit"> <a onclick="FunUpdate()" href="#">[修改]</a>&nbsp; <a onclick="FunDelete()" href="#">[删除]</a></td>--%>
           </tr>
           <tr>
               <td style="text-align: center;">
                   <div class="infoTitle">
                       正文内容</div>
               </td>
           </tr>
           <tr>
               <td style="text-align: left;">
                   <div id="strContent">
                   </div>
               </td>
           </tr>
       </table>
   </div>
   <div id="divReply" class="infoTitle" style="margin-bottom:20px;">
       回复信息列表</div>
 <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
         <table class="tabData" style="width:100%">
            <tr style='background-color:#efefef;'>
                <td style="text-align:left; height:25px;" >
                &nbsp;&nbsp;<span style="color:Blue"><%#Eval("RUserID")%></span>&nbsp;
                <span><%#Eval("RTime")%></span>
                </td>
               
            </tr>
            <tr style="background-color:#fcfcfc" >
            <td  style="text-align:left;">
            <div style=" padding:5px 0px 20px 30px;">
            <%#Eval("RContent")%></div>
            </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <!--底部模板-->
            </table>
            <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>

    
    <div>
        <table >
        
             <tr>
                <td style="text-align:center;">
                <div class="infoTitle" >我要回复</div>
                </td>
            </tr>
            <tr>
                <td >
                <span >回复内容(最多1000字)</span>             
                </td>
            </tr>
             <tr>
                <td >
                      <textarea id="txtContent" name="strContent" style="width:700px;height:200px;visibility:hidden;"></textarea>
                </td>
            </tr>
             <tr>
                <td  style="text-align:center">
              <input type="button" value="发表回复" onclick="FunReply()" />          
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

