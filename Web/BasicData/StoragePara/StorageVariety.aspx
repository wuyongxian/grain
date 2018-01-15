<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageVariety.aspx.cs" Inherits="Web.BasicData.StoragePara.StorageVariety" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
     <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    
   
    <style type="text/css">

</style>
    <script type="text/javascript">
        function change_colorOver(e) {
            var oldColor = e.style.backgroundColor;
            document.getElementById("colorName").value = oldColor;
            e.style.backgroundColor = "#b9bace";
        }
        function change_colorOut(e) {
            e.style.backgroundColor = document.getElementById("colorName").value;
        }

        $(function () {

            $('#spanHelp').toggle(function () {
                $('#divHelp').fadeIn();
            }, function () {
                $('#divHelp').fadeOut();
             });

            //显示网点类型,操作级别
            $.ajax({
                url: 'storage.ashx?type=GetMeasuringUnit',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=MeasuringUnitID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                }, error: function (r) {
                   showMsg('加载网点失败 ！');
                }
            });
        });


        function ShowFrm(wbid) {
            //$('#divfrm').fadeIn("normal");
            showBodyCenter($('#divfrm'));
            $('#WBID').val(wbid);
            if (wbid == "0") {
                $('#trAdd').fadeIn("fast");
                $('#trUpdate').fadeOut("fast");
                $('input[name=strName]').val('');
                $('input[name=AgencyFee]').val('0.00');
            }
            else { //编辑网点
                $('#trAdd').fadeOut("fast");
                $('#trUpdate').fadeIn("fast");

                /*--------数据提交--------*/
                $.ajax({
                    url: 'storage.ashx?type=GetStorageVarietyByID&ID=' + wbid,
                    type: 'post',
                    data: '',
                    dataType: 'json',
                    success: function (r) {
                        $('input[name=strName]').val(r[0].strName);
                          $('input[name=AgencyFee]').val(r[0].AgencyFee);
                           $("select[name=MeasuringUnitID]  option[value='" + r[0].MeasuringUnitID + "'] ").attr("selected", 'selected');
                       
                    }, error: function (r) {
                       showMsg('加载信息失败 ！');
                    }
                });
                /*--------End 数据提交--------*/
            }
        };
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
                    
                    var strurl = 'storage.ashx?type=UpdateStorageVariety&ID=' + wbid;
                    if (wbid == "0") {
                        strurl = 'storage.ashx?type=AddStorageVariety&ID=' + wbid;
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
            if ($('input[name=strName]').val() == "") {
               showMsg('请输入存储产品名称 ！');
                $('input[name=strName]').focus();
                return false;
            }
             if (isNaN($('input[name=AgencyFee]').val())) {
               showMsg('代理费数据类型为长度不超过3的小数类型 ！');
                $('input[name=AgencyFee]').focus();
                return false;
            } else {
            if($('input[name=AgencyFee]').val(),length>4){
             showMsg('代理费数据类型为长度不超过3的小数类型 ！');
                $('input[name=AgencyFee]').focus();
                return false;
            }
            
            }
            return true;
        }


        function FunDelete(wbid) {
            SingleDataDelete('storage.ashx?type=DeleteStorageVarietyByID&ID=' + wbid);
        
        }
     
    </script>
</head>
<body>
     <form id="form1" runat="server">
<div class="pageHead">
<b>设置存储产品</b><span id="spanHelp" style="cursor:pointer" >帮助</span>
</div>
<div id="divHelp"  class="pageHelp">
<span>提示1：粮食银行系统中的存储产品主要是粮食，但系统不限只存粮食，也可以存其它产品。</span><br />
<span>提示2：这里设置的是储户存贷产品，不是储户要兑换的商品，请注意。</span><br />

</div>

<div class="QueryHead">
<table>
            <tr>
            <td><b>已有存贷产品列表</b></td>
            
            <td><%=GetAddItem() %></td>
            </tr>
            
        </table>
</div>
<asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table  class="tabData" style="width:900px">
          <tr class="tr_head" >
                <th style="width:200px; text-align:center;">
                    存贷产品名称</th>
                    <th style="width:100px; text-align:center;">
                    计量单位</th>
                    <th style="width:100px; text-align:center;">
                    网点代理费</th>
                
                    <th style="width:120px; text-align:center;">
                    查看/修改</th>
                   <th style="width:100px; text-align:center;">
                    删除</th>
                
            </tr>
        
    </HeaderTemplate>
    <ItemTemplate>
    <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
        <td><%#Eval("strName")%></td>
         <td><%#Eval("MeasuringUnitID")%></td>
          <td><%#Eval("AgencyFee")%></td>
        
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
    <div  id="divfrm" class="pageEidt" style="display:none;">
    <img class="imgclose" src="../../images/winClose.png" alt="关闭窗口"  style="float:right; cursor:pointer;" onclick="CloseFrm()" />
    <div style="clear:both;">
        <table class="tabEdit">
            
            <tr>
            <td align="right" style="width:100px;"><span>存储产品:</span></td>
            <td><input type="text" name="strName" style="width:200px; height:25px" /></td>
            </tr>
            <tr>
             <td align="right"><span>代理费:</span></td>
            <td><input type="text" name="AgencyFee" style="width:200px; height:25px" /></td>
            </tr>
            <tr>
             <td align="right"><span>计量单位:</span></td>
            <td><select name="MeasuringUnitID" style="width:200px; height:25px" ></select></td>
            </tr>
             
            <tr id="trAdd">
            <td></td>
            <td ><input type="button" id="btnAdd" value="添加" onclick="WBUpdate()" /> </td>
            </tr>
               <tr id="trUpdate">
            <td></td>
            <td ><input type="button" id="btnUpdate" value="修改" onclick="WBUpdate()" />
         
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

