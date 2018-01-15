<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RStorage.aspx.cs" Inherits="Web.Admin.Returned.RStorage" %>

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

        /*--ENd loadFuntion--*/

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




       


        function InitBusinessNO(obj) {
            $.ajax({
                url: "/User/Storage/storage.ashx?type=GetNewBusinessNO&AccountNumber=" + $('#D_AccountNumber').html(),
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                    $('input[name=BusinessNO]').val(r);

                    FunUpdate(obj);
                }, error: function (r) {
                   showMsg('加载信息失败 ！');
                }
            });
        }


        //更新数据方法
        function FunUpdate(id) {
          
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                    var BusinessNO = $('input[name=BusinessNO]').val();
                    $.ajax({
                        url: '/User/Storage/storage.ashx?type=Delete_Dep_Storage&ID=' + id + '&BusinessNO=' + BusinessNO,
                        type: 'post',
                        data: '',
                        dataType: 'text',
                        success: function (r) {
                            if (r == "0") {
                                showMsg('这是一条不存在的存储记录，不可以退还 ！');
                                $('#divfrm').fadeOut('normal');
                            }
                            else if (r == "OK") {
                          
                                showMsg('退还储户存粮成功 ！');
                                $('#divfrm').fadeIn('normal');
                            } else {
                                showMsg('退还储户存粮失败 ！');
                                $('#divfrm').fadeOut('normal');
                            }
                        }, error: function (r) {
                            showMsg('退还储户存粮失败 ！');
                            $('#divfrm').fadeOut('normal');
                        }
                    });

                } else {
                    //console.log('你点击了取消！');
                }
                
            });
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
       <b>退还储户存粮</b>
    </div>
       
        <div class="QueryHead">
            <table>
                <tr>
                    <td> <span>储户账号:</span></td>
                    <td> <input type="text" id="QAccountNumber" runat="server" /></td>
                    <td> <span>密码:</span></td>
                    <td> <input type="password" id="QPassword" runat="server" /></td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="~/images/search_red.png" OnClick="ImageButton1_Click" />
                    </td>
                </tr>
            </table>
        </div>

     <div id="depositorInfo"  runat="server" style="display:none;">
            <table class="tabData"  style="margin:10px 0px;">
                <tr >
                    <td colspan="6" style="border-bottom:1px solid #aaa; height:25px; text-align:center">
                        <span style="font-size: 16px; font-weight: bolder; color:Green">储户基本信息</span>
                    </td>
                </tr>
                <tr>
                    <th align="center" style="width:100px; height:30px;">
                        储户账号
                    </th>
                    <th align="center" style="width:100px;">
                        储户姓名
                    </th>
                     <th align="center" style="width:100px;">
                        移动电话
                    </th>
                      <th align="center" style="width:100px;">
                        当前状态
                    </th>
                      <th align="center" style="width:150px;">
                        身份证号
                    </th>
                     <th align="center" style="width:200px;">
                        储户地址
                    </th>
                   
                </tr>
                   <tr>
                  
                    <td style="height:30px;">
                        <span style="font-weight:bolder; color:Blue;" id="D_AccountNumber"   runat="server"></span>
                    </td>
                    
                    <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_strName" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_PhoneNo" runat="server"></span>
                    </td>
                     <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_numState" runat="server"></span>
                    </td>
                      <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_IDCard" runat="server"></span>
                    </td>
                    <td>
                        <span style="font-weight:bolder; color:Blue;" id="D_strAddress" runat="server"></span>
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
                        <th style="width: 160px; text-align: center;">
                            存入时间
                        </th>
                        <th style="width: 60px; text-align: center;">
                            存储产品
                        </th>
                        <th style="width: 160px; text-align: center;">
                            磅单编号
                        </th>
                        <th style="width: 60px; text-align: center;">
                            存期
                        </th>
                        <th style="width: 60px; text-align: center;">
                            结存数量
                        </th>
                        <th style="width: 100px; text-align: center;">
                            修改
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
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
                        <input type="button" value="退还" style="width: 60px;height:25px;" onclick="InitBusinessNO(<%#Eval("ID") %>)" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
  
    
     <div id="divfrm" class="pageEidtInner" style="border-radius: 20px; width:200px;  padding:5px; display: none; text-align:center;">
        <input type="button" id="btnCunZhe" value="打印存折"  onclick="PrintCunZhe()" />
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
