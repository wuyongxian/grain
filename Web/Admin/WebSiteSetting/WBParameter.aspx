<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WBParameter.aspx.cs" Inherits="Web.Admin.WebSiteSetting.WBParameter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
      <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
      <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .spantitle {
        font-weight:bolder;
        color:#0067a6;
        }
        .spancontent {
        margin-left:30px;
        }
</style>
<script type="text/javascript">

    function HelpOpen() {
        $('#divHelp').fadeIn();
    };

    function HelpClose() {
        $('#divHelp').fadeOut();
    };

    $(function () {
        $.ajax({
            url: 'ws.ashx?type=getWBAuthority',
            type: 'post',
            data: '',
            dataType: 'json',
            success: function (r) {
                $('#WBID').val(r[0].ID);

                if (r[0].Enable_Distance) {
                    $("input[name=Enable_Distance]").attr("checked", "checked");
                }
                if (r[0].ISIntegral) {
                    $("input[name=ISIntegral]").attr("checked", "checked");
                }
                if (r[0].VerifyDepInfo) {
                    $("input[name=VerifyDepInfo]").attr("checked", "checked");
                }

                if (r[0].ErrorLogin_Admin) {
                    $("input[name=ErrorLogin_Admin]").attr("checked", "checked");
                }
                if (r[0].ErrorLogin_User) {
                    $("input[name=ErrorLogin_User]").attr("checked", "checked");
                }

                if (r[0].Price_PrintOnCunZhe) {
                    $("input[name=Price_PrintOnCunZhe]").attr("checked", "checked");
                }
                if (r[0].ISPrintIDCard) {
                    $("input[name=ISPrintIDCard]").attr("checked", "checked");
                }
                if (r[0].ISPrintPhoneNo) {
                    $("input[name=ISPrintPhoneNo]").attr("checked", "checked");
                }


                if (r[0].VerifyType == "1") {
                    $("input[name=VerifyType]").attr("checked", "checked");
                }
               
                if (r[0].InterestType == 1) {
                    $('#InterestType1').attr("checked","checked");
                } else {
                    $('#InterestType2').attr("checked", "checked");
                }

                if (r[0].ISCurrentCal) {
                    $("input[name=ISCurrentCal]").attr("checked", "checked");
                }
                if (r[0].ISCodekeyboard) {
                    $("input[name=ISCodekeyboard]").attr("checked", "checked");
                }
                if (r[0].ISExchangeLimit) {
                    $("input[name=ISExchangeLimit]").attr("checked", "checked");
                }
                $("textarea[name=strPricePolicy]").html(r[0].strPricePolicy);
                $("input[name=numMinDay]").val(r[0].numMinDay);
                $("input[name=exchangeGroupProp]").val(r[0].exchangeGroupProp);
                $("input[name=exchangeGroupPeriod]").val(r[0].exchangeGroupPeriod);
                $("input[name=Integral]").val(r[0].Integral);
                $("input[name=Integral_StorageDep]").val(r[0].Integral_StorageDep);
                $("input[name=Integral_StorageRecommend]").val(r[0].Integral_StorageRecommend);
               

            }, error: function (r) {
               showMsg('加载信息失败 ！');
            }
        });
    });

    function SetWBAuthority() {
        var numMinDay = $("input[name=numMinDay]").val();
        if (!CheckNumInt(numMinDay, '存储利息计算最低期限请输入数字!', 0, 1000)) {
            return false;
        }

        var exchangeGroupProp = $("input[name=exchangeGroupProp]").val();
        if (!CheckNumDecimal(exchangeGroupProp, '分时批量兑换比例输入两位小数!', 2)) {
            return false;
        }

        var exchangeGroupPeriod = $("input[name=exchangeGroupPeriod]").val();
        if (!CheckNumInt(exchangeGroupPeriod, '分时批量兑换周期请输入数字!', 0, 60)) {
            return false;
        }

        var Integral = $("input[name=Integral]").val();
        if (!CheckNumDecimal(Integral, '开户推荐人单次积分请输入两位小数!', 2)) {
            return false;
        }

        var Integral_StorageDep = $("input[name=Integral_StorageDep]").val();
        if (!CheckNumDecimal(Integral_StorageDep, '存粮储户单次积分请输入两位小数!', 2)) {
            return false;
        }

        var Integral_StorageRecommend = $("input[name=Integral_StorageRecommend]").val();
        if (!CheckNumDecimal(Integral_StorageRecommend, '存粮推荐人单次积分请输入两位小数!', 2)) {
            return false;
        }

        var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
        showConfirm(msg, function (obj) {
            if (obj == 'yes') {
                
                var ID = $('#WBID').val();

                $.ajax({
                    url: 'ws.ashx?type=UpateWBAuthority&ID=' + ID,
                    type: 'post',
                    data: $('#form1').serialize(),
                    dataType: 'text',
                    success: function (r) {
                        showMsg('保存设置成功 ！');
                    }, error: function (r) {
                        showMsg('保存设置失败 ！');
                    }
                });
            } else {
                //console.log('你点击了取消！');
            }

        });
    }

    
</script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead" style="width: 300px;">
            <b>网点其他参数设置</b><%--<span style="cursor:pointer" onclick="HelpOpen();">帮助</span>--%>
        </div>

        <div id="divfrm" class="pageEidtInner" style="width:750px;">


            <table class="tabEdit">
                  <tr>
                    <td style="text-align: right;"><span class="spantitle">管理员登陆验证:</span></td>
                    <td>
                        <%--<input type="checkbox" name="ErrorLogin_Admin" />--%>
                        <input type="checkbox" id="ISDefault-ErrorLogin_Admin"  name="ErrorLogin_Admin" value="1" class="custom-checkbox"  /><label for="ISDefault-ErrorLogin_Admin"></label>
                    </td>
                      <td style="text-align: right;"><span class="spantitle">营业员登陆验证:</span></td>
                    <td>
                       <%-- <input type="checkbox" name="ErrorLogin_User" />--%>
                          <input type="checkbox" id="ISDefault-ErrorLogin_User"  name="ErrorLogin_User" value="1" class="custom-checkbox"  /><label for="ISDefault-ErrorLogin_User"></label>
                    </td>
                </tr>


                <tr>
                    <td style="text-align: right;"><span class="spantitle">储户查询密码验证:</span></td>
                    <td>
                       <%-- <input type="checkbox" name="VerifyType" />--%>
                         <input type="checkbox" id="ISDefault-VerifyType"  name="VerifyType" value="1" class="custom-checkbox"  /><label for="ISDefault-VerifyType"></label>
                    </td>
                     <td style="text-align: right;"><span class="spantitle">启用密码键盘:</span></td>
                    <td>
                        <%--<input type="checkbox" name="ISCodekeyboard" />--%>
                         <input type="checkbox" id="ISDefault-ISCodekeyboard"  name="ISCodekeyboard" value="1" class="custom-checkbox"  /><label for="ISDefault-ISCodekeyboard"></label>
                    </td>
                </tr>
                 <tr>
                      <td style="text-align: right;"><span class="spantitle">允许异地存取:</span></td>
                    <td >
                       <%-- <input type="checkbox" name="Enable_Distance" />--%>
                        <input type="checkbox" id="ISDefault-Enable_Distance"  name="Enable_Distance" value="1" class="custom-checkbox"  /><label for="ISDefault-Enable_Distance"></label>
                    </td>
                    <td style="text-align: right;"><span class="spantitle">储户注册信息验证:</span></td>
                    <td >
                       <%-- <input type="checkbox" name="Enable_Distance" />--%>
                        <input type="checkbox" id="ISDefault-VerifyDepInfo"  name="VerifyDepInfo" value="1" class="custom-checkbox"  /><label for="ISDefault-VerifyDepInfo"></label>
                    </td>
                      
                </tr>
                   <tr>
                      <td style="text-align: right;"><span class="spantitle">计算储户积分:</span></td>
                    <td >
                       <%-- <input type="checkbox" name="Enable_Distance" />--%>
                        <input type="checkbox" id="ISDefault-ISIntegral"  name="ISIntegral" value="1" class="custom-checkbox"  /><label for="ISDefault-ISIntegral"></label>
                    </td>
                    <td style="text-align: right;"><span class="spantitle">开户推荐人单次积分:</span></td>
                    <td >
                      <input type="text" style="width: 50px;" name="Integral" />
                    </td>
                      
                </tr>
                  <tr>
                      <td style="text-align: right;"><span class="spantitle">储户首次存粮:</span></td>
                    <td colspan="3">
                        <span>存粮每价值1000元，推荐人可获取积分</span>  
                        <input type="text" style="width: 40px;" name="Integral_StorageRecommend" />
                        <span>，储户可获取积分</span>
                          <input type="text" style="width: 40px;" name="Integral_StorageDep" />
                       
                    </td>
                  
                      
                </tr>

                 <tr>
                    <td style="text-align: right;"><span class="spantitle">活期利息结算方式:</span>
                    </td>
                    <td>
                        <%--<input type="radio" name="InterestType" id="InterestType1" value="1" checked="checked" />按天结算利息--%>
<%--                &nbsp; &nbsp; &nbsp;<input type="radio" name="InterestType" id="InterestType2" value="2" />按月结算利息--%>
                          <input type="radio" id="InterestType1"  name="InterestType" value="1" class="custom-radio"  /><label for="InterestType1"></label><span>按天计息</span>
                        <input type="radio" id="InterestType2"  name="InterestType" value="2" class="custom-radio" style="margin-left:20px;"  /><label for="InterestType2"></label><span>按月计息</span>
                    </td>
                     <td style="text-align: right;"><span class="spantitle">存储利息计算最低期限:</span></td>
                    <td>
                        <input type="text" style="width: 50px;" name="numMinDay" /><span>天</span></td>
                </tr>

                     <tr>
                    <td style="text-align: right;"><span class="spantitle">存转销时定期不到期按照活期计息:</span></td>
                    <td>
                        <%--<input type="checkbox" name="ISCurrentCal" />--%>
                          <input type="checkbox" id="ISDefault-ISCurrentCal"  name="ISCurrentCal" value="1" class="custom-checkbox"  /><label for="ISDefault-ISCurrentCal"></label>
                    </td>
                     <td style="text-align: right;"><span class="spantitle">启用商品品类兑换额度限制:</span></td>
                    <td>
                        <%--<input type="checkbox" name="ISExchangeLimit" />--%>
                         <input type="checkbox" id="ISDefault-ISExchangeLimit"  name="ISExchangeLimit" value="1" class="custom-checkbox"  /><label for="ISDefault-ISExchangeLimit"></label>
                    </td>
                </tr>

                  <tr>
                    <td style="text-align: right;"><span class="spantitle">分时批量兑换价格比例:</span></td>
                    <td>
                        <input type="text" style="width: 50px;" name="exchangeGroupProp" /><span>%</span>
                    </td>
                     <td style="text-align: right;"><span class="spantitle">分时批量兑换周期:</span></td>
                    <td>
                        <input type="text" style="width: 50px;" name="exchangeGroupPeriod" /><span>月</span>
                    </td>
                </tr>
              
                 <tr>
                    <td style="text-align: right;"><span class="spantitle">在存折上打印身份证号:</span></td>
                    <td>
                       <%-- <input type="checkbox" name="ISPrintIDCard" />--%>
                         <input type="checkbox" id="ISDefault-ISPrintIDCard"  name="ISPrintIDCard" value="1" class="custom-checkbox"  /><label for="ISDefault-ISPrintIDCard"></label>
                    </td>
                     <td style="text-align: right;"><span class="spantitle">在存折上打印手机号:</span></td>
                    <td>
                       <%-- <input type="checkbox" name="ISPrintPhoneNo" />--%>
                          <input type="checkbox" id="ISDefault-ISPrintPhoneNo"  name="ISPrintPhoneNo" value="1" class="custom-checkbox"  /><label for="ISDefault-ISPrintPhoneNo"></label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;"><span class="spantitle">在存折上打印价格政策:</span></td>
                    <td>
                       <%-- <input type="checkbox" name="Price_PrintOnCunZhe" />--%>
                        <input type="checkbox" id="ISDefault-Price_PrintOnCunZhe"  name="Price_PrintOnCunZhe" value="1" class="custom-checkbox"  /><label for="ISDefault-Price_PrintOnCunZhe"></label>
                    </td>
                </tr>
               
                <tr>
                    <td style="text-align: right;"><span class="spantitle">价格政策展示:</span></td>
                    <td colspan="3">
                        <textarea name="strPricePolicy" style="width: 400px; height: 50px;"></textarea></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <input type="button" id="btnUpdate" value="保存设置" onclick="SetWBAuthority()" />

                    </td>
                </tr>
            </table>
        </div>

        <div style="width: 100%;">
            <div><span style="color: Red; font-weight: bolder;">说明：</span></div>
            <div>
                <span class="spantitle">管理员登陆验证:</span>
                <br />
                <span class="spancontent">启用后管理员账号有三次输错密码的机会，否则该账号将被禁用24小时。</span>
            </div>
            <div>
                <span class="spantitle">营业员登陆验证:</span>
                <br />
                <span class="spancontent">启用后营业员账号有三次输错密码的机会，否则该账号将被禁用24小时。</span>
            </div>
             <div>
                <span class="spantitle">储户查询密码验证:</span>
                <br />
                <span class="spancontent">启用后所有的储户交易必须输入储户账号和密码才可以完成。</span>
            </div>
             <div>
                <span class="spantitle">启用密码键盘:</span>
                <br />
                <span class="spancontent">是否连接外接密码输入设备。</span>
            </div>
             <div>
                <span class="spantitle">允许异地存取:</span>
                <br />
                <span class="spancontent">允许储户在非注册网点交易（包含存粮、修改存粮、退还存粮、结息续存、兑换\退还兑换、存转销\退还存转销、产品换购\退还产品换购）。</span>
            </div>
              <div>
                <span class="spantitle">储户注册信息验证:</span>
                <br />
                <span class="spancontent">启用后在新建和修改储户时，储户的手机号和身份证号必须为真实有效的信息。</span>
            </div>
             <div>
                <span class="spantitle">计算储户积分:</span>
                <br />
                <span class="spancontent">启用后，在添加储户的时候可以选择此储户的推荐人。</span>
            </div>
             <div>
                <span class="spantitle">开户推荐人单次积分:</span>
                <br />
                <span class="spancontent">新建储户后，给与推荐人的积分奖励（最多两位小数）。</span>
            </div>
             <div>
                <span class="spantitle">活期利息结算方式:</span>
                <br />
                <span class="spancontent">按月计息：计算当前月份之前的存储利息（在本月产生的存储利率不计算）</span><br />
                   <span class="spancontent">按天计息：计算当天之前的存储利息</span>
            </div>
             <div>
                <span class="spantitle">存储利息计算最低期限:</span>
                <br />
                <span class="spancontent">未达到最低存储期限的，在存转销的时候不计算利息（但是在兑换、产品换购、结息续存操作中计算利息）。</span>
            </div>
             <div>
                <span class="spantitle">存转销时定期不到期按照活期计息:</span>
                <br />
                <span class="spancontent">已达到最低存储期限，并且未到期的定期存储（利率计算方式为：按约定到期价计息的定期存储）申请存转销时，可以按照活期利率计息。</span>
            </div> 
            <div>
                <span class="spantitle">启用商品品类兑换额度:</span>
                <br />
                <span class="spancontent">与“商品类型管理”中的商品兑换额度参数结合，限制每个储户每个月兑换单种商品时不可以达到设定上限。</span>
                </div>
              <div>
                <span class="spantitle">分时批量兑换价格比例:</span>
                <br />
                <span class="spancontent">如分时批量兑换价格比例为80%，则批量兑换的商品价格为普通兑换价格的80%。</span>
                </div>
              <div>
                <span class="spantitle">分时批量兑换周期:</span>
                <br />
                <span class="spancontent">设定分时批量兑换一共分为多少个月完成。</span>
                </div>
                 <div>
                <span class="spantitle">在存折上打印身份证号:</span>
                <br />
                <span class="spancontent">启用时在存折首页打印储户的身份证号，不启用在存折首页以“******”代替储户的身份证号</span>
            </div>
                 <div>
                <span class="spantitle">在存折上打印手机号:</span>
                <br />
                <span class="spancontent">启用时在存折首页打印储户的手机号，不启用在存折首页以“******”代替储户的手机号</span>
            </div>
                 <div>
                <span class="spantitle">在存折上打印价格政策:</span>
                <br />
                <span class="spancontent">启用时在存折首页最后一行打印价格政策，不启用在存折首页不打印任何信息</span>
            </div>
                 <div>
                <span class="spantitle">价格政策展示:</span>
                <br />
                <span class="spancontent">当启用“在存折上打印价格政策”时，在存折首页最后一行打印价格政策展示信息</span>
            </div>
            </div>

        </div>



    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>