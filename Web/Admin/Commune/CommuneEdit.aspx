<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommuneEdit.aspx.cs" Inherits="Web.Admin.Commune.CommuneEdit" %>

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
    #divfrm input{ font-size:18px;}
     #divfrm select{ font-size:16px;}
    </style>
       <script type="text/javascript">
           /*--------窗体启动设置和基本设置--------*/
           /*--loadFuntion--*/
           $(function () {


               var AccountNumber = getQueryString('AccountNumber');
               if (AccountNumber != '') {
                   $('#QAccountNumber').val(AccountNumber);
                   $('#Query').hide();
                    FunQuery();
               }
              
               //   GetAddressXian();
              // InitAddressXian();


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
                   var CunID = $('select[name=CunID] option:selected').val();
                   $('select[name=ZuID]').empty();
                   GetAddressZu(CunID);
               });
               $('select[name=ZuID]').change(function () {
                   var strXian = $('select[name=XianID] option:selected').text();
                   var strXiang = $('select[name=XiangID] option:selected').text();
                   var strCun = $('select[name=CunID] option:selected').text();
                   var strZu = $('select[name=ZuID] option:selected').text();
                   $('input[name=strAddress]').val(strXian + ' ' + strXiang + ' ' + strCun + ' ' + strZu);
               });

           });
           /*--ENd loadFuntion--*/
           /*----用户选择地址 和第一次加载地址----*/
           function GetAddressXian() {
               $.ajax({
                   url: '/Ashx/commune.ashx?type=Get_Address_Xian',
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
                   url: '/Ashx/commune.ashx?type=Get_Address_Xiang&XianID=' + XianID,
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
                   url: '/Ashx/commune.ashx?type=Get_Address_Cun&XiangID=' + XiangID,
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       var CunID = r[0].ID;
                       GetAddressZu(CunID); //设置县名
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=CunID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }

                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });
           }

           function GetAddressZu(CunID) {
               $.ajax({
                   url: '/Ashx/commune.ashx?type=Get_Address_Zu&CunID=' + CunID,
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=ZuID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }

                       var strXian = $('select[name=XianID] option:selected').text();
                       var strXiang = $('select[name=XiangID] option:selected').text();
                       var strCun = $('select[name=CunID] option:selected').text();
                       var strZu = $('select[name=ZuID] option:selected').text();
                       $('input[name=strAddress]').val(strXian + ' ' + strXiang + ' ' + strCun + ' ' + strZu);
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });
           }

           function InitAddressXian(XianID) {
               $.ajax({
                   url: '/Ashx/commune.ashx?type=Get_Address_Xian',
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       $('select[name=XianID]').empty();

                       for (var i = 0; i < r.length; i++) {
                           $('select[name=XianID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }
                       $('select[name=XianID]').val(XianID);
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });
           }

           function InitAddressXiang(XianID,XiangID) {
               $.ajax({
                   url: '/Ashx/commune.ashx?type=Get_Address_Xiang&XianID=' + XianID,
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {

                       $('select[name=XiangID]').empty();
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=XiangID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }
                        $('select[name=XiangID]').val(XiangID);
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });

           }

           function InitAddressCun(XiangID,CunID) {
               $.ajax({
                   url: '/Ashx/commune.ashx?type=Get_Address_Cun&XiangID='+XiangID,
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {

                       $('select[name=CunID]').empty();
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=CunID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }
                        $('select[name=CunID]').val(CunID);
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });
           }

           function InitAddressZu(CunID,ZuID) {
               $.ajax({
                   url: '/Ashx/commune.ashx?type=Get_Address_Zu&CunID='+CunID ,
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       $('select[name=ZuID]').empty();
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=ZuID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }
                       $('select[name=ZuID]').val(ZuID);
                      
                      
                       var strXian = $('select[name=XianID] option:selected').text();
                       var strXiang = $('select[name=XiangID] option:selected').text();
                       var strCun = $('select[name=CunID] option:selected').text();
                       var strZu = $('select[name=ZuID] option:selected').text();
                       $('input[name=strAddress]').val(strXian + ' ' + strXiang + ' ' + strCun + ' ' + strZu);
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载信息失败 ！');
                       }
                   }
               });
           }
           /*--  End 用户选择地址 和第一次加载地址--*/


           function FunQuery() {

               if ($('#QAccountNumber').val() == "") {
                  showMsg('请输入社员账号 ！');
                   return false;
               }

               $.ajax({
                   url: '/Ashx/commune.ashx?type=GetCommuneByAccountNumber&txtAccountNumber=' + $('#QAccountNumber').val(),
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                       $('#divfrm').fadeIn('normal');
                       $('#txtWBID').val(r[0].WBID);
                       $('#txtAccountNumber').val(r[0].AccountNumber);
                       $('#txtstrName').val(r[0].strName);
                       $('#txtdt_Commune').val(r[0].dt_Commune);
                       $('#txtIDCard').val(r[0].IDCard);
                       $('#txtPhoneNO').val(r[0].PhoneNO);
                       $('#txtFieldCopies').val(r[0].FieldCopies);
                       $('#txtFiledCount').val(r[0].FieldCount);

                       $('#imgApplication').attr('src', r[0].ApplicationForm.substring(1));
                       $('#imgCommune').attr('src', r[0].CommunePic.substring(1));

                      
                       InitAddressXian(r[0].XianID);
                       InitAddressXiang(r[0].XianID, r[0].XiangID);
                       InitAddressCun(r[0].XiangID, r[0].CunID);
                       InitAddressZu(r[0].CunID, r[0].ZuID);


                   }, error: function (r) {
                       $('#divfrm').fadeOut('normal');
                      showMsg(' 系统中没有此账号信息 ！');
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
                           url: '/Ashx/commune.ashx?type=Update_Commune',
                           type: 'post',
                           data: $('#form1').serialize(),
                           dataType: 'text',
                           success: function (r) {
                               showMsg('修改社员信息成功 ！');
                               $('#btnAdd').attr('disabled', 'disabled');
                               $('#btnAdd').css('background', '#aaa');
                               $('#btnPrint').removeAttr("disabled");

                           }, error: function (r) {
                               showMsg('修改社员信息失败 ！');
                           }
                       });
                   } else {
                       //console.log('你点击了取消！');
                   }

               });
              
           }

           //提交检测
           function SubmitCheck() {

               if ($.trim($('#txtstrName').val()) == '') {
                  showMsg('社员姓名不能为空 ！');
                   $('#txtstrName').focus();
                   return false;
               }
               if ($.trim($('#txtPhoneNO').val()) == '') {
                  showMsg('手机号不能为空 ！');
                   $('#txtPhoneNO').focus();
                   return false;
               }
               if ($.trim($('#txtIDCard').val()) == '') {
                  showMsg('身份证号不能为空 ！');
                   $('#txtIDCard').focus();
                   return false;
               }

               if ($.trim($('#txtFieldCopies').val()) == '') {
                  showMsg('田亩册不能为空 ！');
                   $('#txtFieldCopies').focus();
                   return false;
               }

               if (!CheckNumDecimal($('#txtFiledCount').val(), '土地亩数', 2)) {
                   return false;
               }
//               if ($.trim($('#txtApplicationFileName').val()) == '') {
//                  showMsg('请上传申请表 ！');
//                   $('#txtApplicationFileName').focus();
//                   return false;
//               }




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
               if ($('select[name=ZuID] option:selected').text() == "") {
                  showMsg('组名不能为空 ！');
                   return false;
               }
               return true;
           }


           /*--------End 数据增删改操作--------*/

     



           /*-------打印存折--------*/
           var p_left = 0; var p_ltop = 0; var p_lwidth = 0; var p_lheight = 0;
           $(function () {
               $.ajax({
                   url: '/Ashx/wbinfo.ashx?type=GetPrintSetting',
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
                   url: '/Ashx/commune.ashx?type=GetCommunePrint&AccountNumber=' + $('#txtAccountNumber').val(),
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
           /*-------End 打印存折--------*/
       </script>
</head>
<body>
    <div id="divPrint" style="display: none">
    </div>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="pageHead">
            <b>社员信息修改</b><a href="#" onclick="window.history.back();">返回</a>
        </div>
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>账号:</span>
                    </td>
                    <td>
                        <input type="text" id="QAccountNumber" style="font-size: 16px; width: 120px;height:26px; font-weight: bolder;" /></td>
                    <td style="width: 60px">
                        <a href="#" onclick="FunQuery()">
                            <img src="../../images/search_red.png" /></a>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divfrm" class="pageEidtInner" style="border-radius: 10px; display: none">

            <div style="padding-left: 10px;">
                <table class="tabEdit">
                    <tr>
                        <td align="right" style="width: 120px;">
                            <span>网点:</span>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtWBID" name="WBID" readonly="readonly" style="width: 200px; font-weight: bolder; background-color: #ccc;" />

                        </td>
                    </tr>
                    <tr>
                        <td align="right" style="width: 120px;">
                            <span>社员账号:</span>
                        </td>
                        <td>
                            <input type="text" id="txtAccountNumber" runat="server" name="AccountNumber" readonly="readonly" style="width: 200px; font-weight: bolder; background-color: #ccc;" />

                        </td>
                    </tr>
                    <tr>
                        <td align="right" style="width: 120px;">
                            <span>入社时间:</span>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtdt_Commune" name="dt_Commune" readonly="readonly" style="width: 200px; font-weight: bolder; background-color: #ccc;" />

                        </td>
                    </tr>

                    <tr>
                        <td align="right">
                            <span>社员姓名:</span>
                        </td>
                        <td>
                            <input name="strName" id="txtstrName" runat="server" type="text" style="width: 200px;" /><span style="color: Red;">*</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>手机号:</span>
                        </td>
                        <td>
                            <input name="PhoneNO" id="txtPhoneNO" runat="server" type="text" style="width: 200px;" /><span style="color: Red;">*</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>身份证号:</span>
                        </td>
                        <td>
                            <input name="IDCard" id="txtIDCard" runat="server" type="text" style="width: 200px;" /><span style="color: Red;">*</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>田亩册:</span>
                        </td>
                        <td>
                            <input name="FieldCopies" id="txtFieldCopies" runat="server" type="text" style="width: 200px;" /><span style="color: Red;">*</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>入社土地亩数:</span>
                        </td>
                        <td>
                            <input name="FieldCount" id="txtFiledCount" runat="server" type="text" value="0" style="width: 200px;" /><span style="color: Red;">*</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>入社申请表:</span>
                        </td>
                        <td>
                            <%-- <input type="file" name="ApplicationForm" style="width:200px" />--%>
                            <asp:FileUpload ID="FileApplication" runat="server" Style="width: 200px;" />

                            <asp:Button ID="btnUploadApplication" runat="server" Text="上传"
                                OnClick="btnUploadApplication_Click" />
                            <span style="font-size: 12px;">已选择图片:</span>
                            <img runat="server" id="imgApplication" style="width: 28px; height: 40px;" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right"><span>社员头像:</span>
                        </td>
                        <td>
                            <%-- <input type="file" name="ApplicationForm" style="width:200px" />--%>
                            <asp:FileUpload ID="FileCommunePic" runat="server" Style="width: 200px;" />

                            <asp:Button ID="btnUploadCommunePic" runat="server" Text="上传"
                                OnClick="btnUploadCommunePic_Click" />
                            <span style="font-size: 12px;">已选择图片:</span>
                            <img runat="server" id="imgCommune" style="width: 28px; height: 40px;" />
                        </td>
                    </tr>

                    <tr>
                        <td align="right"><span>储户住址:</span>
                        </td>
                        <td>
                            <select name="XianID" style="width: 100px"></select>
                            <select name="XiangID" style="width: 100px"></select>
                            <select name="CunID" style="width: 100px"></select>
                            <select name="ZuID" style="width: 100px"></select>
                           <%-- <a href="../../BasicData/Data/Address_Zu.aspx">添加组名</a>--%>
                        </td>
                    </tr>



                    <tr>
                        <td></td>
                        <td>
                            <input type="button" id="btnAdd" value="修改社员信息" style="width: 120px" onclick="FunUpdate()" />&nbsp;&nbsp;
                          <input type="button" id="btnPrint" value="打印存折" disabled="disabled" onclick="FunPrint()" />

                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div style="display: none">
            <input type="text" name="strAddress" value="" />
            <%--会员申请表存放地址--%>
            <input type="text" name="Applicationfilename" id="txtApplicationFileName" value="" runat="server" />
            <%--会员头像存放地址--%>
            <input type="text" name="Communefilename" id="txtCommuneFileName" value="" runat="server" />
            <input type="text" id="XianID" />
            <input type="text" id="XiangID" />
            <input type="text" id="CunID" />
            <input type="text" id="ZuID" />
        </div>
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
