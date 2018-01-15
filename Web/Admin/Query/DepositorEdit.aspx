<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepositorEdit.aspx.cs" Inherits="Web.Admin.Query.DepositorEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
    <script src="../../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>
    <style type="text/css">
     #divfrm select {
            font-size: 18px;
            font-weight: bold;
            height: 30px;
            width: 100px;
            color:#111;
        }

       
         input[type=text], input[type=password] {
            width: 200px;
            height: 30px;
            font-size: 18px;
            font-weight: bold;
            color: #111;
        }

        #txt_AccountNumber {
            font-size: 22px;
            font-weight: bolder;
            background-color: #ccc;
            color: green;
        }

        input[type=button]{
            width: 100px;
            height: 40px;
            border-radius:20px;
            font-size: 18px;
            font-weight: bold;
            background:#0081bc;
            color:#fff;
        }

        #btnPrint {
            background: #aaa;
            color: #fff;
        }
        

        .tabDep td {
         padding:5px 5px;
        }
        .tabDep span {
            font-size: 16px;
            font-weight: bold;
            color: #444;
        }
        .tabDep .spanwarning {
            display:none;
            font-size: 12px;
            color: #ff6347;
        }
    </style>
       <script type="text/javascript">
           /*--------窗体启动设置和基本设置--------*/
           /*--loadFuntion--*/
           var ISCodekeyboard;//是否启用密码键盘
           var VerifyDepInfo;//是否验证储户信息
           $(function () {
               ISCodekeyboard = JSON.parse(localStorage.getItem("WBAuthority")).ISCodekeyboard;
               VerifyDepInfo = JSON.parse(localStorage.getItem("WBAuthority")).VerifyDepInfo;
               var AccountNumber = getQueryString("AccountNumber");
               if (AccountNumber != '') {
                   $('#QAccountNumber').val(AccountNumber);
               }
               GetAddressXian();
               //编辑列表选项
               $('select[name=XianID]').change(function () {
                   var XianID = $('select[name=XianID] option:selected').val();
                   $('select[name=XiangID]').empty();
                   $('select[name=CunID]').empty();
                   GetAddressXiang(XianID);
               });

               $('select[name=XiangID]').change(function () {
                   var XiangID = $('select[name=XiangID] option:selected').val();
                   $('select[name=CunID]').empty();
                   GetAddressCun(XiangID);
               });

               $('select[name=CunID]').change(function () {
                   var strXian = $('select[name=XianID] option:selected').text();
                   var strXiang = $('select[name=XiangID] option:selected').text();
                   var strCun = $('select[name=CunID] option:selected').text();

                   $('input[name=strAddress]').val(strXian + ' ' + strXiang + ' ' + strCun);
               });

           });

           //检测是否是密码键盘输入
           document.onkeydown = function (event) {
               var e = event || window.event || arguments.callee.caller.arguments[0];
               if (ISCodekeyboard) {
                   if (e.keyCode > 47 && e.keyCode < 58) {
                       if (document.activeElement.name != 'strPassword' && document.activeElement.name != 'strPassword2') {
                           $('input[name=strPassword]').val('');
                           $('input[name=strPassword2]').val('');
                           return false;
                       }
                   }
               }
               if (e.keyCode == 13) {//确认按键
               }
           };

           /*--ENd loadFuntion--*/
           function GetAddressXian() {
               $.ajax({
                   url: '/User/Query/depositor.ashx?type=Get_Address_Xian',
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       var XianID = r[0].ID;
                       GetAddressXiang(XianID); //设置县名
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=XianID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });
           }

           function GetAddressXiang(XianID) {
               $.ajax({
                   url: '/User/Query/depositor.ashx?type=Get_Address_Xiang&XianID=' + XianID,
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       var XiangID = r[0].ID;
                       GetAddressCun(XiangID); //设置村名
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=XiangID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });

           }

           function GetAddressCun(XiangID) {
               $.ajax({
                   url: '/User/Query/depositor.ashx?type=Get_Address_Cun&XiangID=' + XiangID,
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=CunID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }
                       var strXian = $('select[name=XianID] option:selected').text();
                       var strXiang = $('select[name=XiangID] option:selected').text();
                       var strCun = $('select[name=CunID] option:selected').text();

                       $('input[name=strAddress]').val(strXian + ' ' + strXiang + ' ' + strCun);
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });
           }

           /*--------数据增删改操作--------*/
           //新增数据方法
           function FunUpdate() {
               if (!SubmitCheck()) {//检测输入内容
                   return false;
               }
               var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
               showConfirm(msg, function (obj) {
                   if (obj == 'yes') {
                       
                       $.ajax({
                           url: '/User/Query/depositor.ashx?type=Update_Depositor',
                           type: 'post',
                           data: $('#form1').serialize(),
                           dataType: 'text',
                           success: function (r) {
                               $('#btnAdd').attr('disabled', 'disabled');
                               $('#btnAdd').css('background', '#aaa');
                               $('#btnPrint').removeAttr('disabled');
                               $('#btnPrint').css('background', '#0081bc');
                               showMsg('更新储户信息成功 ！');

                           }, error: function (r) {
                               showMsg('更新储户信息失败 ！');
                           }
                       });
                   } else {
                       //console.log('你点击了取消！');
                   }

               });

           }

           //提交检测
           function SubmitCheck() {

            
               var strPassword = $.trim($('input[name=strPassword]').val());
               var strPassword2 = $.trim($('input[name=strPassword2]').val());

               if (strPassword != strPassword2) {
                  showMsg('两次输入的密码不一致，请重新输入！');
                   $('input[name=strPassword]').focus();
                   return false;
               }
               if ($('input[name=strName]').val() == '') {
                  showMsg('请输入姓名！');
                   $('input[name=strName]').focus();
                   return false;
               }
               if ($('input[name=IDCard]').val() == '') {
                  showMsg('请输入身份证号！');
                   $('input[name=strName]').focus();
                   return false;
               }
               if ($('input[name=PhoneNO]').val() == '') {
                  showMsg('请输入手机号！');
                   $('input[name=strName]').focus();
                   return false;
               }
               if (VerifyDepInfo) {
                   if (!checkIdcard($('input[name=IDCard]').val())) {
                       showMsg('身份证号格式不正确，请重新输入 ！');
                       $('input[name=IDCard]').focus();
                       return false;
                   }
                   if (!CheckInput('PhoneNO', '手机号', '-1')) {
                       return false;
                   };
                   var flag1 = CheckMobile($('input[name=PhoneNO]').val());
                   var flag2 = CheckPhone($('input[name=PhoneNO]').val());
                   if (!flag1 && !flag2) {
                       showMsg('手机号格式不正确，请重新输入 ！');
                       $('input[name=PhoneNO]').focus();
                       return false;
                   }
               }

               if ($('select[name=XianID] option:selected').val() == "") {
                  showMsg('县名不能为空 ！');
                   return false;
               }
               if ($('select[name=XiangID] option:selected').text() == "") {
                  showMsg('乡名不能为空 ！');
                   return false;
               }
               if ($('select[name=CunID] option:selected').text() == "") {
                  showMsg('村名不能为空 ！');
                   return false;
               }
               return true;
           }

           /*--------End 数据增删改操作--------*/
           function FunQuery() {

               if ($('#QAccountNumber').val() == "") {
                  showMsg('请输入储户账号 ！');
                   return false;
               }

               $.ajax({
                   url: '/User/Query/depositor.ashx?type=GetDepositor&AccountNumber=' + $('#QAccountNumber').val(),
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       $('#divfrm').fadeIn('normal');
                       $('input[name=WBID]').val(r[0].WBID);
                       $('input[name=AccountNumber]').val(r[0].AccountNumber);
                       $('input[name=strName]').val(r[0].strName);
                       $('input[name=IDCard]').val(r[0].IDCard);
                       $('input[name=PhoneNO]').val(r[0].PhoneNO);
                       //if (r[0].ISSendMessage) {
                       //    $('input[name=PhoneNO]').attr('checked', true);
                       //}

                       $('select[name=XianID]').val(r[0].XianID);
                       $('select[name=XiangID]').val(r[0].XiangID);
                       //先根据乡名加载村名，然后再显示村信息
                       $.ajax({
                           url: '/User/Query/depositor.ashx?type=Get_Address_Cun&XiangID=' + r[0].XiangID,
                           type: 'post',
                           data: '',
                           dataType: 'json',
                           success: function (r2) {
                               $('select[name=CunID]').empty();
                               for (var i = 0; i < r2.length; i++) {
                                   $('select[name=CunID]').append("<option value='" + r2[i].ID + "'>" + r2[i].strName + "</option>");
                               }

                               $('select[name=CunID]').val(r[0].CunID);

                               var strXian = $('select[name=XianID] option:selected').text();
                               var strXiang = $('select[name=XiangID] option:selected').text();
                               var strCun = $('select[name=CunID] option:selected').text();

                               $('input[name=strAddress]').val(strXian + ' ' + strXiang + ' ' + strCun);
                               

                           }, error: function (r) {
                               if (r.responseText != "Error") {
                                  showMsg('加载信息失败 ！');
                               }
                           }
                       });
                      

                   }, error: function (r) {
                       $('#divfrm').fadeOut('normal');
                      showMsg(' 系统中没有此账号信息 ！');
                   }
               });
           }

           var p_left = 0; var p_ltop = 0; var p_lwidth = 0; var p_lheight = 0;
           $(function () {
               $.ajax({
                   url: '/Ashx/wbinfo.ashx?type=GetPrintSetting_Dep',
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       p_lwidth = r[0].Width;
                       p_lheight = r[0].Height;
                       p_lleft = r[0].DriftRateX;
                       p_ltop = r[0].DriftRateY;
                   }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                   }
               });
           });

           function CreateOneFormPage() {
               LODOP = getLodop();
               LODOP.PRINT_INIT("存折打印");
               LODOP.SET_PRINT_STYLE("FontSize", 18);
               LODOP.SET_PRINT_STYLE("Bold", 1);
               LODOP.ADD_PRINT_TEXT(0, 0, 0, 0, "打印页面部分内容");
               LODOP.ADD_PRINT_HTM(p_ltop, p_lleft, p_lwidth, p_lheight, document.getElementById("divPrint").innerHTML);

           };

           function FunPrint() {
               //查询社员信息
               $.ajax({
                   url: '/User/Storage/storage.ashx?type=GetDepositorPrint&AccountNumber=' + $('input[name=AccountNumber]').val(),
                   type: 'post',
                   data: '',
                   dataType: 'text',
                   success: function (r) {
                       $('#divPrint').html('');
                       $('#divPrint').append(r);
                       CreateOneFormPage();
                       LODOP.PREVIEW(); //打印存折
                   }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                   }
               });

           }

       </script>
</head>
<body>

    <div id="divPrint" style="display: none;">
    </div>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>储户信息修改</b>
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>账号:</span>
                    </td>
                    <td>
                        <input type="text" id="QAccountNumber" style="font-size: 16px; width: 120px; font-weight: bolder;" /></td>
                    <td style="width: 60px">
                        <a href="#" onclick="FunQuery()">
                            <img src="../../images/search_red.png" /></a>

                    </td>
                </tr>
            </table>
        </div>

        <div id="divfrm" class="pageEidtInner" style="border-radius: 20px; display: none;">

            <div style="padding-left: 10px;">
                <table class="tabDep">
                    <tr>
                        <td align="right" style="width: 100px;">
                            <span>网点:</span>
                        </td>
                        <td>
                            <input type="text" name="WBID" readonly="readonly" style="width: 200px; font-weight: bolder; background-color: #ccc;" />

                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <span>储户账号:</span>
                        </td>
                        <td>
                            <input type="text" name="AccountNumber" readonly="readonly" style="width: 200px; font-weight: bolder; background-color: #ccc;" />

                        </td>
                    </tr>
                    <tr id="trUpdate">
                        <td align="right"><span>密码:</span>
                        </td>
                        <td>
                            <input type="password" name="strPassword" style="width: 200px;" />
                        </td>
                    </tr>
                    <tr id="tr1">
                        <td align="right"><span>重复密码:</span>
                        </td>
                        <td>
                            <input type="password" name="strPassword2" style="width: 200px;" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <span>储户姓名:</span>
                        </td>
                        <td>
                            <input name="strName" type="text" style="width: 200px;" />
                        </td>
                    </tr>
                    <tr id="trAdd">
                        <td align="right"><span>身份证号:</span>
                        </td>
                        <td>
                            <input name="IDCard" type="text" style="width: 200px;" />
                        </td>
                    </tr>
                    <tr id="tr3">
                        <td align="right"><span>手机号:</span>
                        </td>
                        <td>
                            <input name="PhoneNO" type="text" style="width: 200px;" />
                        <%--    <input type="checkbox" name="ISSendMessage" />接收群发短信--%>
                        </td>
                    </tr>
                    <tr id="tr2">
                        <td align="right"><span>储户住址:</span>
                        </td>
                        <td>
                            <select name="XianID" style="width: 100px"></select>
                            <select name="XiangID" style="width: 100px"></select>
                            <select name="CunID" style="width: 100px"></select>
                        <%--    <a href="../../BasicData/Data/Address_Cun.aspx">添加村名</a>--%>
                        </td>
                    </tr>

                    <tr id="tr4">
                        <td></td>
                        <td>
                            <input type="button" id="btnAdd" value="修改储户" onclick="FunUpdate()" />&nbsp;&nbsp;
                           <input type="button" id="btnPrint" disabled="disabled" value="打印存折" onclick="FunPrint()" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div style="display: none">
            <input type="text" name="strAddress" value="" />
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>

</body>
</html>
