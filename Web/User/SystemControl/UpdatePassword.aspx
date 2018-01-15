<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdatePassword.aspx.cs" Inherits="Web.User.SystemControl.UpdatePassword" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    
   
    <style type="text/css">
    #divPwd
    {
       position:absolute;
       top:200px; left:50px; width:160px; height:100px;
       padding:20px;
       border:2px solid #666;
       border-radius:5px;
       background-color:#eee;
        }
</style>
    <script type="text/javascript">


        $(function () {
            $.ajax({
                url: '/Ashx/wbinfo.ashx?type=GetUserBySessionID',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    $('#WBID').val(r[0].ID);
                    $('input[name=strRealName]').val(r[0].strRealName);
                    $('input[name=strLoginName]').val(r[0].strLoginName);
                    //                         $('input[name=strPassword]').val(r[0].strPassword);
                    //                         $('input[name=strPassword2]').val(r[0].strPassword);
                    $('input[name=strPhone]').val(r[0].strPhone);
                    $('input[name=strAddress]').val(r[0].strAddress);
                    $('input[name=strPassword_old]').val(r[0].strPassword);

                }, error: function (r) {
                   showMsg('加载营业员信息失败 ！');
                }
            });
        });

        //密码验证
        function FunCheck() {

            $.ajax({
                url: '/Ashx/wbinfo.ashx?type=GetUserPWD&PWD=' + $('#txtPwd').val(),
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                    if ($('input[name=strPassword_old]').val() == r) {
                        $('#divPwd').hide("fast");
                        $('#divfrm').fadeIn("normal");
                    } else {
                       showMsg('您输入的密码错误 ！');
                    }
                }, error: function (r) {

                }
            });
           
        }


        //添加、修改网点 （ID=0是添加网点）
        function WBUpdate() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var wbid = $('#WBID').val();
            var strurl = '/Ashx/wbinfo.ashx?type=UpdateUserPart&ID=' + wbid;
            SingleDataUpdate(strurl, $('#form1').serialize());
           
        }

        //提交检测
        function SubmitCheck() {

            if (!CheckInput('strRealName', '真实姓名', -1)) {
                return false;
            }
            if (!CheckInput('strLoginName', '登陆名', -1)) {
                return false;
            }
            if (!CheckInput('strPhone', '手机号', -1)) {
                return false;
            }
            if (!CheckInput('strAddress', '联系地址', -1)) {
                return false;
            }

            if ($('input[name=strPassword]').val() != $('input[name=strPassword2]').val()) {
               showMsg('两次输入的密码不一致，请检查 ！');
                $('input[name=strPassword2]').focus();
                return false;
            }

            return true;

        }


    </script>
</head>
<body>
     <form id="form1" runat="server">
<div class="pageHead">
<b>个人信息管理</b><span id="spanHelp"  style="cursor:pointer" onclick="HelpOpen();">帮助</span>
</div>
<div id="divHelp"  class="pageHelp">

<span>提示1：密码要有一定难度（英文+数字+标点符号），不能是自己的姓名、出生日期、电话号码。请牢记你的密码，如果忘记无法找回。
</span><br />

</div>

   <div id="divPwd" >
   <span>验证密码：</span>
   <input type="password" id="txtPwd" style="width:120px; margin-bottom:10px;" /><br />
   <input type="button"  value="验证" onclick="FunCheck();" style="width:50px;" />
   </div>
 
    <div id="divfrm" class="pageEidtInner" style="display:none;">
        <table >
            
        
             <tr>
            <td align="right" style="width:100px;"><span>真实姓名:</span></td>
            <td><input type="text" style="width:150px;" name="strRealName" />
            <span style="color:Red; font-weight:bolder;">*</span><span style="font-size:12px;">必填</span>
            </td>
            </tr>
             <tr>
             <td align="right"><span>登录名:</span></td>
            <td><input type="text" style="width:150px;"  name="strLoginName" />  <span style="color:Red; font-weight:bolder;">*</span><span style="font-size:12px;">必填</span></td>
            </tr>
             <tr>
            <td align="right"><span>登录密码:</span></td>
            <td><input type="password" style="width:150px;"  name="strPassword" />
              <span style="color:Red; font-weight:bolder;">*</span><span style="font-size:12px;">英文与数字，至少四位</span>
            </td>
            </tr>
             <tr>
            <td align="right"><span>确认密码:</span></td>
            <td><input  type="password" style="width:150px;"  name="strPassword2" />
             <span style="color:Red; font-weight:bolder;">*</span><span style="font-size:12px;">再次输入密码</span>
            </td>
            </tr>
            <tr>
           <td align="right"><span>手机:</span></td>
            <td><input type="text" style="width:150px;"  name="strPhone" />
             <span style="color:Red; font-weight:bolder;">*</span><span style="font-size:12px;">必填</span>
            </td>
            </tr>
            <tr>
            <td align="right"><span>住址:</span></td>
            <td><input type="text" style=" width:240px;"  name="strAddress" />
            <span style="color:Red; font-weight:bolder;">*</span><span style="font-size:12px;">必填</span>
            </td>
            </tr>

               <tr id="trUpdate">
            <td></td>
            <td ><input type="button" id="btnUpdate" value="修改" onclick="WBUpdate()" />
     
             </td>
            </tr>
        </table>
        </div>


    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
       <%--定义编号--%>
     <input type="hidden" name="strPassword_old" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>