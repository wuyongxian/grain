<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditError.aspx.cs" Inherits="Web.User.Storage.EditError" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   
    <script src="../../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>

    <script type="text/javascript">
        /*--------窗体启动设置和基本设置--------*/
        /*--loadFuntion--*/
        var ISCodekeyboard;//是否启用密码键盘
        $(function () {
            ISCodekeyboard = JSON.parse(localStorage.getItem("WBAuthority")).ISCodekeyboard;
            $('#QPassword').focus(function () {
                $('#QPassword').val('');
            })
        });

        //检测是否是密码键盘输入
        document.onkeydown = function (event) {
            var e = event || window.event || arguments.callee.caller.arguments[0];
            if (ISCodekeyboard) {
                if (e.keyCode > 47 && e.keyCode < 58) {
                    if (document.activeElement.id != 'QPassword') {
                        $('#QPassword').val('');
                        return false;
                    }
                }
            }
            if (e.keyCode == 13) {//确认按键
            }
        };


        /*--ENd loadFuntion--*/



        //显示更新数据窗口
        function ShowFrm_Update(wbid) {
            $('#WBID').val(wbid);
          
            /*----数据提交----*/
            $.ajax({
                url: '/User/Storage/storage.ashx?type=GetByID_Dep_Storage&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $('#divfrm').fadeIn('normal'); //显示编辑窗口
                    $("input[name=StorageFee]").val(r[0].StorageFee);
                    $("input[name=StorageDate]").val(r[0].StorageDate);
                    $("input[name=WeighNO]").val(r[0].WeighNo);
                    $("input[name=StorageNumber]").val(r[0].StorageNumber);

                }, error: function (r) {
                   showMsg('加载信息失败 ！');
                }
            });
            /*---End 数据提交----*/
        }



        function InitBusinessNO() {
            $.ajax({
                url: "/User/Storage/storage.ashx?type=GetNewBusinessNO&AccountNumber=" + $('#D_AccountNumber').html(),
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                    $('input[name=BusinessNO]').val(r);

                    FunUpdate();
                }, error: function (r) {
                   showMsg('加载信息失败 ！');
                }
            });
        }


        //更新数据方法
        function FunUpdate() {
            if (!SubmitCheck()) {//检测输入内容
                return false;
            }
            var wbid = $('#WBID').val();
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                    $.ajax({
                        url: '/User/Storage/storage.ashx?type=Update_Dep_Storage&ID=' + wbid,
                        type: 'post',
                        data: $('#form1').serialize(),
                        dataType: 'text',
                        success: function (r) {
                            if (r == "OK") {
                                $('#btnAdd').attr('disabled', 'disabled');
                                $('#btnCunDan').removeAttr('disabled');
                                $('#btnCunZhe').removeAttr('disabled');
                                $('#btnXiaoPiao').removeAttr('disabled');
                                showMsg('更新数据成功 ！');
                                $('#btnCunZhe').removeAttr('disabled');
                            } else {
                                showMsg('更新数据失败 ！');
                            }
                        }, error: function (r) {
                            showMsg('更新数据失败 ！');
                        }
                    });
                } else {
                    //console.log('你点击了取消！');
                }              
            });
        }


        //提交检测
        function SubmitCheck() {

            var AccountNumber = $('#txtAccountNumber').val();
            if (AccountNumber == 'name=' || AccountNumber == undefined) {
               showMsg('请先选择储户账号！');
                $('#QAccountNumber').focus();
                return false;
            }
            if ($('select[name=VarietyID] option:selected').val() == "") {
               showMsg('存储产品类型不能为空 ！');
                return false;
            }
            if ($('select[name=UserTypeID] option:selected').val() == "") {
               showMsg('储户类型不能为空 ！');
                return false;
            }
            if ($('select[name=TimeID] option:selected').val() == "") {
               showMsg('存期类型不能为空 ！');
                return false;
            }
            if (!CheckNumInt($('input[name=StorageNumber]').val(), '存储数量', 1, -1)) {
                return false;
            }

            return true;
        }

        function EditNumber() {
            if ($('#btnEdit').val() == '增加') {
                $('#btnEdit').val("减少");
                $('#btnEdit').css({ color: 'Red' });
                $('input[name=StorageNumber]').css({ color: 'Red' });

                $('input[name=editType]').val("减少");
            } else {
                $('#btnEdit').val("增加");
                $('#btnEdit').css({ color: 'Green' });
                $('input[name=StorageNumber]').css({ color: 'Green' });
                $('input[name=editType]').val("增加");
            }
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



        function PrintCunZhe() {

            $.ajax({
                url: '/Ashx/storage.ashx?type=PrintDep_OperateLog&BusinessNO=' + $('input[name=BusinessNO]').val() + '&AccountNumber=' + $('#D_AccountNumber').html(),
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
 <div id="divPrint" style="display:none">

    </div>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>修改当日错误存粮数据</b><span id="spanHelp" style="cursor: pointer">帮助</span>
        </div>
        <div id="divHelp" class="pageHelp" style="border: 1px solid #333; border-radius: 5px; display: none;">
            <span>提示1：如果您的存储数额发生错误，可以重新更正存储数额。</span><br />
            <span>提示2：营业员只可以修改当日的错误数据，管理员可以修改其他日期的错误数据。</span><br />

        </div>

        <div class="QueryHead">
            <table>
                <tr>
                    <td>
                          <span>储户账号:</span>
                    </td>
                    <td>
                         <input type="text" id="QAccountNumber" style="font-size: 16px;" runat="server" />
                    </td>
                    <td>
                           <span>密码:</span>
                    </td>
                    <td>
                         <input type="password" id="QPassword" style="font-size: 12px; width: 100px;" runat="server" />
                    </td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton2" runat="server"
                            ImageUrl="~/images/search_red.png" OnClick="ImageButton1_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="depositorInfo" runat="server" style="display: none;">
            <table class="tabData">
                <tr>
                    <td colspan="6" style="border-bottom: 1px solid #aaa; height: 25px; text-align: center">
                        <span style="font-size: 16px; font-weight: bolder; color: Green">储户基本信息</span>
                    </td>
                </tr>
                <tr>
                    <th align="center" style="width: 100px; height: 30px;">储户账号
                    </th>
                    <th align="center" style="width: 100px;">储户姓名
                    </th>
                    <th align="center" style="width: 100px;">移动电话
                    </th>
                    <th align="center" style="width: 100px;">当前状态
                    </th>
                    <th align="center" style="width: 150px;">身份证号
                    </th>
                    <th align="center" style="width: 200px;">储户地址
                    </th>

                </tr>
                <tr>

                    <td style="height: 30px;">
                        <span style="font-weight: bolder; color: Blue;" id="D_AccountNumber" runat="server"></span>
                    </td>

                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_strName" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_PhoneNo" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_numState" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_IDCard" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight: bolder; color: Blue;" id="D_strAddress" runat="server"></span>
                    </td>
                </tr>
            </table>
        </div>
        <h4>最近存粮信息</h4>
        <span id="StorageInfo" runat="server"></span>
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 160px; text-align: center;">存入时间
                        </th>
                        <th style="width: 60px; text-align: center;">存储产品
                        </th>
                        <th style="width: 160px; text-align: center;">磅单编号
                        </th>
                        <th style="width: 60px; text-align: center;">存期
                        </th>
                        <th style="width: 60px; text-align: center;">结存数量
                        </th>
                        <th style="width: 100px; text-align: center;">修改
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td>
                        <%#Eval("StorageDate")%>
                    </td>
                    <td>
                        <%#Eval("VarietyID")%>
                    </td>
                    <td>
                        <%#Eval("WeighNo")%>
                    </td>
                    <td>
                        <%#Eval("TimeID")%>
                    </td>
                    <td>
                        <%#Eval("StorageNumber")%>
                    </td>
                    <td>
                        <input type="button" value="修改" style="width: 60px;height:25px;" onclick="ShowFrm_Update(<%#Eval("ID") %>)" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>

        <div id="divfrm" class="pageEidtInner" style="display: none;">
          
           <table class="tabEdit">
                    <tr>
                        <td colspan="2" >
                            <span style="font-size: 16px; font-weight: bolder; color: Blue; margin-left: 50px;">存粮信息</span>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <span>保管费率:</span>
                        </td>
                        <td>
                            <input type="text" name="StorageFee" readonly="readonly" style="background-color: #ddd;" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <span>存入日期:</span>
                        </td>
                        <td>
                            <input type="text" name="StorageDate" readonly="readonly" style="background-color: #ddd;" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <span>磅单编号:</span>
                        </td>
                        <td>
                            <input name="WeighNO" type="text" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <span>存储数量:</span>
                        </td>
                        <td>
                            <input type="button" id="btnEdit" onclick="EditNumber();" style="color: Green; font-size: 16px; font-weight: bold; width: 50px;" value="增加" />
                            <input name="StorageNumber" type="text" style="width: 100px; font-size: 16px; font-weight: bold; color: Green;" value="0" />
                            <span id=""></span>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <input type="button" id="btnAdd" value="保存数据" onclick="InitBusinessNO()" />
                            <input type="button" id="btnCunZhe" value="打印存折" disabled="disabled" onclick="PrintCunZhe()" />
                        </td>
                    </tr>
                </table>
         
        </div>

        <div style="display: none;">

            <%--修改类型--%>
            <input type="text" name="editType" value="增加" />
            <%--新的业务编号--%>
            <input type="text" name="BusinessNO" value="" />
            <%--储户账号--%>
            <input type="text" id="txtAccountNumber" name="AccountNumber" value="" runat="server" />
            <%--存储于利率账号--%>
            <input type="text" name="StorageRateID" value="" />
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>

</body>
</html>
