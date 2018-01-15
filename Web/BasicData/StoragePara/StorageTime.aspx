<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageTime.aspx.cs" Inherits="Web.BasicData.StoragePara.StorageTime" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    
   
    <style type="text/css">
        #divfrm {
            /*width: 480px;
            height: 470px;*/
            left: 100px;
            display:none;
            z-index:100; 

        }
            #divfrm .tabEdit td {
                padding: 5px 5px;
            }

        .pay_list_c1 {
            width: 24px;
            height: 18px;
            float: left;
            padding-top: 3px;
            cursor: pointer;
            text-align: center;
            margin-right: 10px;
            background-image: url(images/inputradio.gif);
            background-repeat: no-repeat;
            background-position: -24px 0;
        }
        .radioclass {
            opacity: 0;
            cursor: pointer;
            -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=0)";
            filter: alpha(opacity=0);
        }
        .on {
            background-position: 0 0;
        }
</style>
    <script type="text/javascript">
     
        $(function () {

            //显示网点类型,操作级别
            $.ajax({
                url: '/BasicData/StoragePara/storage.ashx?type=GetStorageType',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=TypeID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                }, error: function (r) {
                   showMsg('加载存储产品名称失败 ！');
                }
            });

            $('input[name=ISRegular]').click(function () {
                var ISRegular_value = $('input[name=ISRegular]:checked').val();
                ShowOption(ISRegular_value);
            });

            $('input[name=InterestType]').click(function () {
                if ($(this).attr('id') == 'InterestType-3') {//按照到期价格计息
                    $('#tr-numExChangeProp').show();
                } else {
                    $('#tr-numExChangeProp').hide();
                }
            })

            $('input[name=ISRegular]').click(function () {
                $('#tr-numExChangeProp').hide();
            })
        });



        function ShowFrm(wbid) {

            // $('#divfrm').fadeIn("normal");
            showBodyCenter($('#divfrm'));

            $('#WBID').val(wbid);
            if (wbid == "0") {
                console.log('-------------add');
                ////$('input[name=ISRegular]').removeAttr('disabled');
                //$('input[name=ISRegular]').removeAttr('readonly');
                $('#trAdd').fadeIn("fast");
                $('#trUpdate').fadeOut("fast");
                $('select[name=TypeID]').removeAttr('disabled');
                $('select[name=TypeID]').get(0).selectIndex = 0;
                $('input[name=strName]').val('');
                //$('input[name=ISRegular]').removeAttr('checked');
                $('#ISRegular-1').attr('checked', 'checked');
                ShowOption('1');
                $('input[name=CalculateInterest]').removeAttr('checked');
                $('input[name=numStorageDate]').val('0');
                $('input[name=numExChangeProp]').val('0.000');
                $('input[name=limitExChangeProp]').val('100');
                $('input[name=PricePolicy]').val('90');
                $('input[name=InterestType]:eq(0)').attr('checked', 'checked');
                $('#tr-numExChangeProp').hide();
            }
            else { //编辑网点
                console.log('-------------update');
                $('#trAdd').fadeOut("fast");
                $('#trUpdate').fadeIn("fast");
                ////$('input[name=ISRegular]').attr('disabled', 'disabled');
                //$('input[name=ISRegular]+label').attr('readonly', 'readonly');
                /*--------数据提交--------*/
                $.ajax({
                    url: 'storage.ashx?type=GetStorageTimeByID&ID=' + wbid,
                    type: 'post',
                    data: '',
                    dataType: 'json',
                    success: function (r) {
                        $('input[name=txtTypeID]').val(r[0].TypeID);
                        $("select[name=TypeID]  option[value='" + r[0].TypeID + "'] ").attr("selected", 'selected');
                        $('select[name=TypeID]').attr('disabled', 'disabled');
                        $('input[name=strName]').val(r[0].strName);
                     
                        $('input[name=numStorageDate]').val(r[0].numStorageDate);
                        $('input[name=numExChangeProp]').val(r[0].numExChangeProp);
                        $('input[name=limitExChangeProp]').val(r[0].limitExChangeProp);
                        $('input[name=PricePolicy]').val(r[0].PricePolicy);

                        var InterestType = parseInt(parseInt(r[0].InterestType) - 1);
                      
                        // $('input[name=InterestType]:eq(0)').attr('checked', 'checked');

                        if (r[0].CalculateInterest == true) {
                            $('input[name=CalculateInterest]').attr('checked', 'checked');
                        } else {
                            $('input[name=CalculateInterest]').removeAttr('checked');
                        }
                        if (r[0].ISRegular == true) {//定期
                            $('#ISRegular-2').attr('checked', 'checked');
                            ShowOption('2');
                        } else {
                            $('#ISRegular-1').attr('checked', 'checked');
                            ShowOption('1');
                        }
                        if (InterestType == '2') {
                            $('#tr-numExChangeProp').show();
                        } else {
                            $('#tr-numExChangeProp').hide();
                        }
                        $('input[name=InterestType]:eq(' + InterestType + ')').attr('checked', 'checked');

                    }, error: function (r) {
                        showMsg('加载信息失败 ！');
                    }
                });
                /*--------End 数据提交--------*/
            }
        };

        //选择显示内容 type=1:活期 type=2：定期
        function ShowOption(type) {
            if (type == '1') {
                //$('#tr-numStorageDate').hide();
                $('#div-InterestType-1').show();
                $('#div-InterestType-2').hide();
                $('#div-InterestType-3').hide();
                $('#div-InterestType-4').hide();
                //$('#tr-numExChangeProp').hide();
                $('#InterestType-1').attr('checked', 'checked');

            } else {
                //$('#tr-numStorageDate').show();
                $('#div-InterestType-1').hide();
                $('#div-InterestType-2').show();
                $('#div-InterestType-3').show();
                $('#div-InterestType-4').show();
                //$('#tr-numExChangeProp').show();
                $('#InterestType-2').attr('checked', 'checked');
            }
        }

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
                    
                    var strurl = 'storage.ashx?type=UpdateStorageTime&ID=' + wbid;
                    if (wbid == "0") {
                        strurl = 'storage.ashx?type=AddStorageTime&ID=' + wbid;
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
            if (!CheckSelect('TypeID', '存储类型')) {
                return false;
            }
            if (!CheckInput('strName', '存期名称', '-1')) {
                return false;
            }
            if (!CheckNumInt($('input[name=numStorageDate]').val(), '约定实存天数', '-1')) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=numExChangeProp]').val(), '定期优惠兑换比例',3)) {
                return false;
            }
            if (!CheckNumDecimal($('input[name=limitExChangeProp]').val(), '每月最多兑换比例', 3)) {
                return false;
            }
            if (!CheckNumInt($('input[name=PricePolicy]').val(), '价格政策期限', -1,-1)) {
                return false;
            }
            return true;
        }


        function FunDelete(wbid) {
            SingleDataDelete('storage.ashx?type=DeleteStorageTimeByID&ID=' + wbid);
           
        }


    </script>
</head>
<body>
     <form id="form1" runat="server">
<div class="pageHead">
<b>存储期限设置</b><span id="spanHelp" style="cursor:pointer" >帮助</span>
</div>
<div id="divHelp"  class="pageHelp">
<span>提示1：请认真设置产品的存期和付息类型。</span><br />
<span>提示2：请根据你们的实际情况设置粮食银行系统中的存储期限名称、实际存储天数、定期储粮储户享受到期价兑换商品的优惠比例（正数）。实际存储天数应为30的正整数倍，现粮的存期为0天。</span><br />
<span>提示3：兑换优惠比例是指定期储粮（不论到期与否）在指定的额度内用到期价兑换（实际上是给储户很大的优惠），优惠额度用比例体现，超过部分用存入价兑换。</span><br />
<span>提示4：设置完成后，最好不要删除，否则会造成已经存在的数据找不到对应的存期。</span><br />
</div>

<div class="QueryHead">
<table>
            <tr>
            <td><b>已有存储期限列表</b></td>
            
            <td><%=GetAddItem() %></td>
           
            </tr>
            
        </table>
    
</div>
<asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table  class="tabData" style="width:900px">
          <tr class="tr_head" >
                <th style="width:200px; text-align:center;">
                    存储类型</th>
                    <th style="width:100px; text-align:center;">
                    存储期限</th>
                    <th style="width:100px; text-align:center;">
                    约定存储天数</th>
                    <th style="width:150px; text-align:center;">
                    计息方式</th>
                    <th style="width:100px; text-align:center;">
                    价格政策</th>
                
                    <th style="width:120px; text-align:center;">
                    查看/修改</th>
                   <th style="width:100px; text-align:center;">
                    删除</th>
                
            </tr>
        
    </HeaderTemplate>
    <ItemTemplate>
    <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
        <td><%#Eval("TypeID")%></td>
         <td><%#Eval("strName")%></td>
          <td><%#Eval("numStorageDate")%></td>
          <td><%#Eval("InterestType")%></td>
          <td><%#Eval("PricePolicy")%>天</td>
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

    <div id="divfrm" class="pageEidt">
            <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口" style="float: right; cursor: pointer;" onclick="CloseFrm()" />
             <div style="clear: both;">
                 <table class="tabEdit">
                      <tr>
                         <td align="right"><span>存期类型:</span></td>
                         <td>
                            <input type="radio" id="ISRegular-1" name="ISRegular" value="1" class="custom-radio" checked="checked" /><label for="ISRegular-1"></label><span style="margin:0px 20px 0px 5px">活期</span>
                              <input type="radio" id="ISRegular-2" name="ISRegular" value="2" class="custom-radio"  /><label for="ISRegular-2"></label><span style="margin:0px 0px 0px 5px">定期</span>
                             </td>
                     </tr>
                     <tr>
                         <td align="right" style="width: 120px;"><span>储户类型:</span></td>
                         <td>
                             <select name="TypeID" style="width: 150px;"></select></td>
                     </tr>
                     <tr>
                         <td align="right"><span>存储期限名称:</span></td>
                         <td>
                             <input type="text" name="strName" style="width: 150px;" /></td>
                     </tr>
                    
                     <tr id="tr-numStorageDate">
                         <td align="right"><span>约定存储天数:</span></td>
                         <td>
                             <input type="text" name="numStorageDate" value="0" style="width: 40px;" /><span style="font-size: 12px;">(定期产品请输入存储天数，非定期产品不做要求)</span></td>
                     </tr>
                     <tr>
                         <td align="right"><span>利息计算方式:</span></td>
                         <td>
                             <div id="div-InterestType-1" style="padding:2px 0px;">
                               <input type="radio" id="InterestType-1" name="InterestType" value="1" class="custom-radio"  /><label for="InterestType-1"></label><span style="margin:5px 0px 0px 5px">每月按照活期利率计息</span><span style="font-size:12px;color:#666;">(付息)</span>
                                 </div>
                             <div id="div-InterestType-2" style="padding:2px 0px;">
                              <input type="radio" id="InterestType-2" name="InterestType" value="2" class="custom-radio"  /><label for="InterestType-2"></label><span style="margin:5px 0px 0px 5px">按照到时市场价计息</span><span style="font-size:12px;color:#666;">(分红)</span></div>
                              <div id="div-InterestType-3" style="padding:2px 0px;">
                              <input type="radio" id="InterestType-3" name="InterestType" value="3" class="custom-radio"  /><label for="InterestType-3"></label><span style="margin:5px 0px 0px 5px">按照约定到期价格计息</span><span style="font-size:12px;color:#666;">(定期)</span></div>
                              <div id="div-InterestType-4" style="padding:2px 0px;">
                              <input type="radio" id="InterestType-4" name="InterestType" value="4" class="custom-radio"  /><label for="InterestType-4"></label><span style="margin:5px 0px 0px 5px">按照约定合同价计息</span><span style="font-size:12px;color:#666;">(入股)</span></div>
                            
                         </td>
                     </tr>
                     <tr id="tr_CalculateInterest" style="display:none;">
                         <td align="right"><span>兑换商品计息:</span></td>
                         <td>
                             <input type="checkbox" id="CalculateInterest-1"  name="CalculateInterest" value="1" class="custom-checkbox"  /><label for="CalculateInterest-1"></label><span style="margin:0px 0px 0px 5px">兑换商品时计利息</span></td>
                     </tr>
                     <tr>
                         <td align="right"><span>价格政策:</span></td>
                         <td>存储不满<input type="text" name="PricePolicy" value="0" style="width: 40px;" />天，按约定收取保管费</td>
                     </tr>
                     <tr>
                         <td align="right"><span>每月最多兑换比例:</span></td>
                         <td>
                             <input type="text" name="limitExChangeProp" value="100" style="width: 60px;" />%</td>
                     </tr>
                     <tr id="tr-numExChangeProp">
                         <td align="right"><span>定期优惠兑换比例:</span></td>
                         <td>
                             <input type="text" name="numExChangeProp" value="5" style="width: 60px;" />%</td>
                     </tr>

                     <tr id="trAdd">
                         <td></td>
                         <td>
                             <input type="button" id="btnAdd" value="添加" onclick="WBUpdate()" />
                         </td>
                     </tr>
                     <tr id="trUpdate">
                         <td></td>
                         <td>
                             <input type="button" id="btnUpdate" value="修改" onclick="WBUpdate()" />

                         </td>
                     </tr>
                 </table>
             </div>
         </div>
    <div style="display:none">
     <input type="text" name="txtTypeID" value="" />
    </div>
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>


