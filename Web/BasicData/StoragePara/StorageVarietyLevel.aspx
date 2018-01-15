<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageVarietyLevel.aspx.cs" Inherits="Web.BasicData.StoragePara.StorageVarietyLevel" %>

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
       /*--------窗体启动设置和基本设置--------*/
        //表格鼠标悬停换色
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
            InitSelect('storage.ashx?type=GetStorageVariety', 'VarietyID', '加载存粮类型失败！');
            InitSelect('storage.ashx?type=GetStorageLevel_B', 'VarietyLevelID', '加载存粮等级失败！');
        });
        //初始化select标签
        function InitSelect(strUrl,strName,strAlert){
            $.ajax({
                url: strUrl,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    for (var i = 0; i < r.length; i++) {
                        $('select[name=' + strName + ']').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                    }
                }, error: function (r) {
                   showMsg(strAlert);
                }
            });
        }
        /*--------End  窗体启动设置和基本设置--------*/


        /*--------自定义div窗口的开启和关闭--------*/
        function ShowFrm(wbid) {
            // $('#divfrm').fadeIn("normal");
            showBodyCenter($('#divfrm'));
            $('#WBID').val(wbid);
            if (wbid == "0") {//新增网点
                ShowFrm_Add(wbid);
            }
            else { //编辑网点
                ShowFrm_Update(wbid);
            }
        };

        function CloseFrm() {
            $('#divfrm').fadeOut("normal");
        }

        //显示新增数据窗口
        function ShowFrm_Add(wbid) {
            $('#trAdd').fadeIn("fast");
            $('#trUpdate').fadeOut("fast");

            $("select[name=VarietyID]").get(0).selectedIndex=0;//设置索引为1的项选中
            $("select[name=VarietyLevelID]").get(0).selectedIndex=0;
            $('input[name=YingDu]').val('0.00');

            $('input[name=ShuiFen]').val('0.00');
            $('input[name=ShuiFen_CK]').val('0.00');
            $('input[name=ShuiFen_DZ]').val('0.00');

            $('input[name=Rongzhong]').val('0.00');
            $('input[name=Rongzhong_DK]').val('0.00');
            $('input[name=Rongzhong_CZ]').val('0.00');

            $('input[name=ZaZhi]').val('0.00');
            $('input[name=ZaZhi_CK]').val('0.00');
            $('input[name=ZaZhi_DZ]').val('0.00');

            $('input[name=ChuCao]').val('0.00');
            $('input[name=ChuCao_CZ]').val('0.00');
            $('input[name=ChuCao_DK]').val('0.00');

            $('input[name=MeiBian]').val('0.00');
            $('input[name=MeiBian_CK]').val('0.00');
            $('input[name=MeiBian_DZ]').val('0.00');
        }
        //显示更新数据窗口
        function ShowFrm_Update(wbid) {
            $('#trAdd').fadeOut("fast");
            $('#trUpdate').fadeIn("fast");

            /*----数据提交----*/
            $.ajax({
                url: 'storage.ashx?type=GetStorageLevelByID&ID=' + wbid,
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {
                    $("select[name=VarietyID]  option[value='" + r[0].VarietyID + "'] ").attr("selected", 'selected');
                    $("select[name=VarietyLevelID]  option[value='" + r[0].VarietyID + "'] ").attr("selected", 'selected');
                    $('input[name=YingDu]').val(r[0].YingDu);

                    $('input[name=ShuiFen]').val(r[0].ShuiFen);
                    $('input[name=ShuiFen_CK]').val(r[0].ShuiFen_CK);
                    $('input[name=ShuiFen_DZ]').val(r[0].ShuiFen_DZ);

                    $('input[name=Rongzhong]').val(r[0].Rongzhong);
                    $('input[name=Rongzhong_DK]').val(r[0].Rongzhong_DK);
                    $('input[name=Rongzhong_CZ]').val(r[0].Rongzhong_CZ);

                    $('input[name=ZaZhi]').val(r[0].ZaZhi);
                    $('input[name=ZaZhi_CK]').val(r[0].ZaZhi_CK);
                    $('input[name=ZaZhi_DZ]').val(r[0].ZaZhi_DZ);

                    $('input[name=ChuCao]').val(r[0].ChuCao);
                    $('input[name=ChuCao_CZ]').val(r[0].ChuCao_CZ);
                    $('input[name=ChuCao_DK]').val(r[0].ChuCao_DK);

                    $('input[name=MeiBian]').val(r[0].MeiBian);
                    $('input[name=MeiBian_CK]').val(r[0].MeiBian_CK);
                    $('input[name=MeiBian_DZ]').val(r[0].MeiBian_DZ);
                   
                }, error: function (r) {
                   showMsg('加载信息失败 ！');
                }
            });
            /*---End 数据提交----*/
        }
         /*--------End 自定义div窗口的开启和关闭--------*/


        /*--------数据增删改操作--------*/
        //提交检测
        function SubmitCheck() {
            if ($('input[name=strName]').val() == "") {
               showMsg('请输入存储期限名称 ！');
                $('input[name=strName]').focus();
                return false;
            }
            if (isNaN($('input[name=numStorageDate]').val())) {
               showMsg('约定实存天数应填写数字 ！');
                $('input[name=numStorageDate]').focus();
                return false;
            }
            if (isNaN($('input[name=numExChangeProp]').val())) {
               showMsg('约定兑换比例应填写数字 ！');
                $('input[name=numExChangeProp]').focus();
                return false;
            }
            return true;
        }

        //新增数据方法
        function FunAdd() {
            SingleDataAdd('storage.ashx?type=AddStorageLevel', $('#form1').serialize());
          
        }
        //更新数据方法
        function FunUpdate() {

            var wbid = $('#WBID').val();
            SingleDataUpdate('storage.ashx?type=UpdateStorageLevel&ID=' + wbid, $('#form1').serialize());
          
        }

        //删除数据方法
        function FunDelete(ID, VarietyID, VarietyLevelID) {
            SingleDataDelete('storage.ashx?type=DeleteStorageLevelByID&ID=' + ID + '&VarietyID=' + VarietyID + '&VarietyLevelID=' + VarietyLevelID);
           
        }
       
         /*--------End 数据增删改操作--------*/
     
    </script>
</head>
<body>
     <form id="form1" runat="server">
<div class="pageHead">
<b>产品等级及其详细指标</b><span id="spanHelp" style="cursor:pointer" >帮助</span>
</div>
<div id="divHelp" class="pageHelp">
<span>提示1：粮食指标国家标准是以三级为标准，程序中也以三级为准，请至少保证每个品种的三级有正确的参数。单击“查看/修改”可以设置对应储粮等级的详细参数，单击“添加新的存粮等级”可以添加新的等级。</span><br />
<span>提示2：可以添加、修改，设置完成后最好不要删除。删除会造成以往已经存入的对应等级找不到。</span><br />

</div>

<div class="QueryHead">
<table>
            <tr>
            <td><b>已有存储产品等级</b></td>
            
            <td><%=GetAddItem() %></td>
            </tr>
            
        </table>
</div>
<asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table  class="tabData" style="width:900px">
          <tr class="tr_head" >
                <th style="width:200px; text-align:center;">
                    存储产品</th>
                    <th style="width:200px; text-align:center;">
                    等级</th>
                
                    <th style="width:120px; text-align:center;">
                    查看/修改</th>
                   <th style="width:100px; text-align:center;">
                    删除</th>
                
            </tr>
        
    </HeaderTemplate>
    <ItemTemplate>
    <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
        <td><%#Eval("VarietyName")%></td>
         <td><%#Eval("VarietyLevelName")%></td>
      
   <td> <%# GetUpdateItem(Eval("ID")) %> </td>
      <td> <%# GetDeleteItem(Eval("ID"), Eval("VarietyID"), Eval("VarietyLevelID"))%></td>
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
                <td style="width: 80px;">
                    <span>存储产品:</span>
                    </td>
                    <td style="width:120px">
                    <select name="VarietyID" style="width: 80px; height: 25px">
                    </select>
                </td>
                <td >
                    <span>品种等级:</span></td>
                    <td>
                    <select name="VarietyLevelID" style="width: 80px; height: 25px">
                    </select>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <span>小麦硬度:</span>
                    <input type="text" name="YingDu" style="width: 80px; height: 25px" />
                </td>
                
            </tr>
             <tr>
                <td style="width: 80px;">
                    <span>水分基点:</span>
                    </td>
                    <td style="width:80px">
                   <input type="text" name="ShuiFen" style="width: 80px; height: 25px" />
                    
                </td>
                <td >
                    <span>超点扣重:</span></td>
                    <td>
                    <input type="text" name="ShuiFen_CK" style="width: 80px; height: 25px" />
                    %
                </td>
                <td >
                    <span>低点增重:</span></td>
                    <td>
                     <input type="text" name="ShuiFen_DZ" style="width: 80px; height: 25px" />
                    %
                </td>
            </tr>
            <tr>
                <td style="width: 80px;">
                    <span>容重基点:</span>
                    </td>
                    <td style="width:80px">
                   <input type="text" name="Rongzhong" style="width: 80px; height: 25px" />
                    
                </td>
                <td >
                    <span>低点扣重:</span></td>
                    <td>
                    <input type="text" name="Rongzhong_DK" style="width: 80px; height: 25px" />
                    %
                </td>
                <td >
                    <span>超点增重:</span></td>
                    <td>
                     <input type="text" name="Rongzhong_CZ" style="width: 80px; height: 25px" />
                    %
                </td>
            </tr>
            <tr>
                <td style="width: 80px;">
                    <span>杂质基点:</span>
                    </td>
                    <td style="width:80px">
                     <input type="text" name="ZaZhi" style="width: 80px; height: 25px" />
                    </select>
                </td>
                <td >
                    <span>超点扣重:</span></td>
                    <td>
                     <input type="text" name="ZaZhi_CK" style="width: 80px; height: 25px" />
                    %
                </td>
                <td >
                    <span>低点增重:</span></td>
                    <td>
                    <input type="text" name="ZaZhi_DZ" style="width: 80px; height: 25px" />
                    %
                </td>
            </tr>
            <tr>
                <td style="width: 80px;">
                    <span>出糙基点:</span>
                    </td>
                    <td style="width:80px">
                     <input type="text" name="ChuCao" style="width: 80px; height: 25px" />
                    
                </td>
                <td >
                    <span>超点增重:</span></td>
                    <td>
                    <input type="text" name="ChuCao_CZ" style="width: 80px; height: 25px" />
                    %
                </td>
                <td >
                    <span>低点扣重:</span></td>
                    <td>
                      <input type="text" name="ChuCao_DK" style="width: 80px; height: 25px" />
                    %
                </td>
            </tr>
            <tr>
                <td style="width: 80px;">
                    <span>霉变基点:</span>
                    </td>
                    <td style="width:80px">
                    <input type="text" name="MeiBian" style="width: 80px; height: 25px" />
                </td>
                <td >
                    <span>超点扣重:</span></td>
                    <td>
                    <input type="text" name="MeiBian_CK" style="width: 80px; height: 25px" />
                   %
                </td>
                <td >
                    <span>低点增重:</span></td>
                    <td>
                   <input type="text" name="MeiBian_DZ" style="width: 80px; height: 25px" />
                    %
                </td>
            </tr>
            <tr id="trAdd">
                <td>
                </td>
                <td>
                    <input type="button" id="btnAdd" value="添加" onclick="FunAdd()" />
                </td>
            </tr>
            <tr id="trUpdate">
                <td>
                </td>
                <td>
                    <input type="button" id="btnUpdate" value="修改" onclick="FunUpdate()" />
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