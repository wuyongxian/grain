<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddAccount.aspx.cs" Inherits="Web.Settle.AddAccount" %>

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
               GetNewAccountNumber();
               GetWB();
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
           /*--ENd loadFuntion--*/
           function GetWB() {
               $.ajax({
                   url: '/Ashx/settlebasic.ashx?type=Get_WBList',
                   type: 'post',
                   data: '',
                   dataType: 'json',
                   success: function (r) {
                     
                       for (var i = 0; i < r.length; i++) {
                           $('select[name=WBID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                       }
                   }, error: function (r) {
                       if (r.responseText != "Error") {
                          showMsg('加载网点信息失败 ！');
                       }
                   }
               });
           }



           function GetAddressXian() {
               $.ajax({
                   url: '/Ashx/settlebasic.ashx?type=Get_Address_Xian',
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
                   url: '/Ashx/settlebasic.ashx?type=Get_Address_Xiang&XianID=' + XianID,
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
                   url: '/Ashx/settlebasic.ashx?type=Get_Address_Cun&XiangID=' + XiangID,
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


           function GetNewAccountNumber() {
               $.ajax({
                   url: '/Ashx/settlebasic.ashx?type=GetNewAccountNumber',
                   type: 'post',
                   data: '',
                   dataType: 'text',
                   success: function (r) {
                       $('input[name=AccountNumber]').val(r);
                   }, error: function (r) {
                      showMsg('加载信息失败 ！');

                   }
               });
           }



           /*--------数据增删改操作--------*/
           //新增数据方法
           function FunAdd() {
               if (!SubmitCheck()) {//检测输入内容
                   return false;
               }
               var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
               showConfirm(msg, function (obj) {
                   if (obj == 'yes') {
                       //alert('你点击了确定！');
                       $.ajax({
                           url: '/Ashx/settlebasic.ashx?type=Add_SA_Account',
                           type: 'post',
                           data: $('#form1').serialize(),
                           dataType: 'text',
                           success: function (r) {
                               if (r == "OK") {
                                   showMsg("新增网点账户成功!");

                                   $('#btnAdd').attr('disabled', 'disabled');
                                   $('#btnAdd').css('background', '#aaa');
                                   $('#btnPrint').removeAttr('disabled');

                               } else if (r == "1") {
                                   showMsg("系统中已存在相同的网点账户信息，请修改后再添加!");

                               }
                           }, error: function (r) {
                               showMsg('添加网点账户失败 ！');
                           }
                       });

                   } else {
                       //alert('你点击了取消！');
                   }
               });


           }

           //提交检测
           function SubmitCheck() {

            
               if ($('input[name=strName]').val() == '') {
                  showMsg('请输入姓名！');
                   $('input[name=strName]').focus();
                   return false;
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
                   url: '/Ashx/settlebasic.ashx?type=PrintSAAccount&AccountNumber=' + $('input[name=AccountNumber]').val(),
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

           function ISEixtAccount() {
               // 查询当前网点是否已经开户
               var AccountNumber = $('#txtAccount').val();
               $.ajax({
                   url: '/Ashx/settlebasic.ashx?type=ISExitSA_Account&AccountNumber=' + AccountNumber,
                   type: 'post',
                   data: '',
                   dataType: 'text',
                   success: function (r) {
                       if (r == "-1") {
                          showMsg('这是一个不存在的网点账号 ！');
                       }
                       else if (r == "0") {
                          showMsg('此网点账号已被禁用 ！');
                       } else {
                           PrintAccount(AccountNumber);
                       }
                   }, error: function (r) {
                      showMsg('加载打印坐标时出现错误 ！');
                   }
               });
           }

           function PrintAccount(AccountNumber) {

               $.ajax({
                   url: '/Ashx/settlebasic.ashx?type=PrintSAAccount&AccountNumber=' + AccountNumber,
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
    <div id="divPrint" style="display:none"> </div>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>原粮出库开户</b>&nbsp;<span id="spanHelp" style="cursor: pointer">补打存折</span>
    </div>
     <div id="divHelp" class="pageHelp" >
<span>网点账号:</span><input type="text" id="txtAccount" />
<input type="button" onclick="ISEixtAccount();" value="补打" /> <br />

</div>
    <div id="divfrm" class="pageEidtInner"  >
   
            <table class="tabEdit">
                <tr>
                    <td align="right" style="width: 100px;">
                     <span>网点账号:</span> 
                    </td>
                    <td>
                        <input type="text"  name="AccountNumber"  readonly="readonly" style="width: 200px; font-weight:bolder; background-color:#ccc;" />
                      
                    </td>
                </tr>
              
                <tr>
                    <td align="right">
                        <span>网点:</span>
                    </td>
                    <td>
                     <select name="WBID" style="width:200px;"></select>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <span>代理人姓名:</span>
                    </td>
                    <td>
                        <input name="strName" type="text" style="width: 200px;" />
                    </td>
                </tr>
                <tr id="trAdd">
                    <td align="right"> <span>身份证号:</span>
                    </td>
                    <td>
                       <input name="IDCard" type="text" style="width: 200px;" />
                    </td>
                </tr>
                <tr id="tr3">
                   <td align="right"> <span>手机号:</span>
                    </td>
                    <td>
                          <input name="PhoneNO" type="text" style="width: 200px;" />
                         
                    </td>
                </tr>
                 <tr id="tr2">
                   <td align="right"><span>储户住址:</span>
                    </td>
                    <td>
                      <select name="XianID" style="width:100px"></select>
                        <select name="XiangID" style="width:100px"></select>
                          <select name="CunID" style="width:100px"></select>
                          
                    </td>
                </tr>

                   <tr id="tr4">
                    <td>
                    </td>
                    <td>
                        <input type="button" id="btnAdd" value="添加账户" onclick="FunAdd()" />&nbsp;&nbsp;
                           <input type="button" id="btnPrint" disabled="disabled" value="打印存折" onclick="FunPrint()" />
                    </td>
                </tr>
            </table>
        </div>


    <div style="display:none">
    <input type="text" name="strAddress" value="" />
        <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
    </div>
    </form>
    
</body>
</html>

