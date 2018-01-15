<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommuneQuery.aspx.cs" Inherits="Web.User.Commune.CommuneQuery" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
   
    
    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {
//              GetAddressXian();

//              //编辑列表选项
//              $('select[name=XianID]').change(function () {
//                  var XianID = $('select[name=XianID] option:selected').val();
//                  $('#QXianID').val(XianID);
//                  alert($('#QXianID').val());
//                  $('select[name=XiangID]').empty();
//                  $('select[name=CunID]').empty();
//                  GetAddressXiang(XianID);
//              });

//              $('select[name=XiangID]').change(function () {
//                  var XiangID = $('select[name=XiangID] option:selected').val();
//                  $('select[name=CunID]').empty();
//                  GetAddressCun(XiangID);
//              });
//              $('select[name=CunID]').change(function () {
//                  var CunID = $('select[name=CunID] option:selected').val();
//                  $('select[name=ZuID]').empty();
//                  GetAddressZu(CunID);
//              });



          });

//          function GetAddressXian() {
//              $.ajax({
//                  url: '/Ashx/commune.ashx?type=Get_Address_Xian',
//                  type: 'post',
//                  data: '',
//                  dataType: 'json',
//                  success: function (r) {
//                      var XianID = r[0].ID;
//                      GetAddressXiang(XianID); //设置县名
//                      $('select[name=XianID]').append("<option value='0'>--请选择--</option>");
//                      for (var i = 0; i < r.length; i++) {
//                          $('select[name=XianID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
//                      }
//                  }, error: function (r) {
//                      if (r.responseText != "Error") {
//                         showMsg('加载信息失败 ！');
//                      }
//                  }
//              });
//          }

//          function GetAddressXiang(XianID) {
//              $.ajax({
//                  url: '/Ashx/commune.ashx?type=Get_Address_Xiang&XianID=' + XianID,
//                  type: 'post',
//                  data: '',
//                  dataType: 'json',
//                  success: function (r) {
//                      var XiangID = r[0].ID;
//                      GetAddressCun(XiangID); //设置村名
//                      $('select[name=XiangID]').append("<option value='0'>--请选择--</option>");
//                      for (var i = 0; i < r.length; i++) {
//                          $('select[name=XiangID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
//                      }
//                  }, error: function (r) {
//                      if (r.responseText != "Error") {
//                         showMsg('加载信息失败 ！');
//                      }
//                  }
//              });

//          }

//          function GetAddressCun(XiangID) {
//              $.ajax({
//                  url: '/Ashx/commune.ashx?type=Get_Address_Cun&XiangID=' + XiangID,
//                  type: 'post',
//                  data: '',
//                  dataType: 'json',
//                  success: function (r) {
//                      var CunID = r[0].ID;
//                      GetAddressZu(CunID); //设置县名
//                      $('select[name=CunID]').append("<option value='0'>--请选择--</option>");
//                      for (var i = 0; i < r.length; i++) {
//                          $('select[name=CunID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
//                      }

//                  }, error: function (r) {
//                      if (r.responseText != "Error") {
//                         showMsg('加载信息失败 ！');
//                      }
//                  }
//              });
//          }

//          function GetAddressZu(CunID) {
//              $.ajax({
//                  url: '/Ashx/commune.ashx?type=Get_Address_Zu&CunID=' + CunID,
//                  type: 'post',
//                  data: '',
//                  dataType: 'json',
//                  success: function (r) {
//                      $('select[name=ZuID]').append("<option value='0'>--请选择--</option>");
//                      for (var i = 0; i < r.length; i++) {
//                          $('select[name=ZuID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
//                      }
//                    
//                  }, error: function (r) {
//                      if (r.responseText != "Error") {
//                         showMsg('加载信息失败 ！');
//                      }
//                  }
//              });
//          }


          function QClick() {
              $('#QAccountNumber').val('');
              $('#QWBID').val('');
              $('#QstrName').val('');
              $('#QIDCard').val('');
              $('#QFieldCopies').val('');
              $('#QPhoneNO').val('');
              $('#Qdt_Commune1').val('');
              $('#Qdt_Commune2').val('');
              $('#XianID').get(0).selectedIndex = 0;
              $('#XiangID').get(0).selectedIndex = 0;
              $('#CunID').get(0).selectedIndex = 0;
              $('#ZuID').get(0).selectedIndex = 0;
          }

          function ShowApplication(src) {
              $('#divCommunePic').hide("normal");
              if (src != '') {
                  if ($('#divApplication').is(':hidden')) {
                      $('#imgApplication').attr('src', src);
                      $('#divApplication').show("normal");
                  } else {
                      if ($('#imgApplication').attr('src') == src) {
                          $('#divApplication').hide("normal");
                      } else {
                          $('#imgApplication').attr('src', src);
                      }
                  }
              }
          }

          function ShowCommunePic(src) {
              $('#divApplication').hide("normal");
              if (src != '') {
                  if ($('#divCommunePic').is(':hidden')) {
                      $('#imgCommunePic').attr('src', src);
                      $('#divCommunePic').show("normal");
                  } else {
                      if ($('#imgCommunePic').attr('src') == src) {
                          $('#divCommunePic').hide("normal");
                      } else {
                          $('#imgCommunePic').attr('src', src);
                      }
                  }
              }
             
          }



          function FunClose() {
              $('#divApplication').hide("normal");
              $('#divCommunePic').hide("normal");
          }
      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>社员信息查询</b>
        <span id="spanHelp" style="cursor: pointer">帮助</span>
    </div>
    <div id="divHelp"  class="pageHelp">
<span>提示1：各种查询可以独使用，也可以联合使用，但必须保证至少有一项查询条件。</span><br />
<span>提示2：每项查询均为模糊查询条件，为保证查找信息的正确性，请输入完整的查询信息。</span><br />

</div>
    
<div id="storageQuery">

</div>
    <div style="margin: 20px 0px;">
        <div id="Query">
        <table class="tabEdit" style="width:880px; background-color:#efefef">
        <tr>
        <td align="right" style="width:80px;"> <span>社员账号:</span></td>
        <td align="left" style="width:110px;">  <input type="text" id="QAccountNumber" style="font-size:14px; width:100px; font-weight:bolder;" runat="server" /></td>
        <td align="right" style="width:80px;">  <span>姓名:</span></td>
        <td align="left" style="width:100px;">    <input type="text" id="QstrName" style="font-size:14px;width:90px; font-weight:bolder;" runat="server" /></td>
        
        <td align="right" style="width:60px;">  <span>电话:</span></td>
        <td align="left" style="width:100px;">      <input type="text" id="QPhoneNO" style="font-size:14px;width:90px; font-weight:bolder;" runat="server" /></td>
        <td align="right" style="width:80px;"> <span>身份证号:</span></td>
        <td align="left" style="width:160px;">     <input type="text" id="QIDCard" style="font-size:14px;width:150px; font-weight:bolder;" runat="server" /></td>
        <td rowspan="3" style="width:50px;">
        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png" 
                runat="server" onclick="ImageButton1_Click" />
        </td>
        <td rowspan="1"> <input type="button" value="清空" style="width:50px;" onclick="QClick()" /></td>
        </tr>
        <tr>
        <td align="right" style="width:80px;">  <span>网点:</span></td>
        <td align="left" style="width:110px;">   <input type="text" id="QWBID" disabled="disabled" style="font-size:14px; width:100px; font-weight:bolder;" runat="server" /></td>
        <td align="right" style="width:80px;">  <span>入社日期:</span></td>
        <td align="left" style="width:100px;">    <input type="text" id="Qdt_Commune1" readonly="readonly" onclick="WdatePicker()" style="font-size:14px;width:90px; font-weight:bolder;" runat="server" /></td>
        
        <td align="right" style="width:60px;">   <span>至:</span></td>
        <td align="left" style="width:100px;">      <input type="text" id="Qdt_Commune2" readonly="readonly" onclick="WdatePicker()" style="font-size:14px;width:90px; font-weight:bolder;" runat="server" /></td>
        <td align="right" style="width:80px;">   <span>田亩册:</span></td>
        <td align="left" style="width:160px;">      <input type="text" id="QFieldCopies" style="font-size:14px;width:150px; font-weight:bolder;" runat="server" /></td>
        <td rowspan="2"> 
             <asp:Button ID="btnOutPut" runat="server"  class="outputExcel" Text="导出Excel" 
                 onclick="btnOutPut_Click" />
         </td>
        </tr>
        <tr>
        <td align="right" style="width:80px;">  <span>地址:  县</span></td>
        <td align="left" style="width:110px;">    <asp:DropDownList ID="XianID" style="width:90px;" runat="server" 
                AutoPostBack="True" onselectedindexchanged="XianID_SelectedIndexChanged">
            </asp:DropDownList></td>
        <td align="right" style="width:80px;">   <span>乡</span></td>
        <td align="left" style="width:100px;">     <asp:DropDownList ID="XiangID"  style="width:90px;" runat="server" 
                AutoPostBack="True" onselectedindexchanged="XiangID_SelectedIndexChanged"></asp:DropDownList></td>
        
        <td align="right" style="width:60px;">    <span>村</span></td>
        <td align="left" style="width:100px;">       <asp:DropDownList ID="CunID"  style="width:90px;" runat="server" 
                AutoPostBack="True" onselectedindexchanged="CunID_SelectedIndexChanged">
            </asp:DropDownList></td>
        <td align="right" style="width:80px;">    <span>组</span></td>
        <td align="left" style="width:160px;">    <asp:DropDownList ID="ZuID"  style="width:90px;" runat="server" 
                AutoPostBack="True">
            </asp:DropDownList></td>
      
        </tr>
        </table>
        </div>
        
        <div id="StorageList" style="margin:20px 0px">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                    <tr class="tr_head">
                        <th style="width: 80px; height:20px; text-align: center;">
                            网点
                        </th>
                        <th style="width: 90px; text-align: center;">
                            社员账号
                        </th>
                        <th style="width: 70px; text-align: center;">
                            姓名
                        </th>
                         <th style="width: 150px; text-align: center;">
                            地址
                        </th>
                        <th style="width: 150px; text-align: center;">
                            身份证号
                        </th>
                        <th style="width: 90px; text-align: center;">
                            手机号
                        </th>
                         <th style="width: 100px; text-align: center;">
                            田亩册
                        </th>
                        <th style="width: 60px; text-align: center;">
                            土地亩数
                        </th>
                       
                        <th style="width: 90px; text-align: center;">
                            入社时间
                        </th>
                        <th style="width: 40px; text-align: center;">
                            状态
                        </th>
                       <th style="width: 60px; text-align: center;">
                            申请表
                        </th>
                       
                       <th style="width: 40px; text-align: center;">
                            头像
                        </th>
                        <th style="width: 40px; text-align: center;">
                            修改
                        </th>
                       
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    
                    <td>
                        <%#Eval("WBID")%>
                    </td>
                    <td>
                      <a href="/User/Commune/OperateLog.aspx?AccountNumber=<%#Eval("AccountNumber")%>" ><%#Eval("AccountNumber")%></a> 
                    </td>
                    <td>
                        <%#Eval("strName")%>
                    </td>
                    <td>
                        <%#Eval("strAddress")%>
                    </td>
                    <td>
                        <%#(Eval("IDCard"))%>
                    </td>
                    <td>
                        <%#Eval("PhoneNO")%>
                    </td>
                   <td>
                        <%#Eval("FieldCopies")%>
                    </td>
                     <td>
                        <%#Eval("FieldCount")%>
                    </td>
                     <td>
                        <%#Eval("dt_Commune")%>
                    </td>
                     <td>
                        <%#Eval("numState")%>
                    </td>
                     <td>
                    <a href="#" onclick="ShowApplication('<%#Eval("ApplicationForm") %>');"><img style="width:21px; height:30px;" src="<%#Eval("ApplicationForm")%>" </a>
                     
                    </td>
                    <td>
                     <a href="#" onclick="ShowCommunePic('<%#Eval("CommunePic") %>');"><img style="width:21px; height:30px;" src="<%#Eval("CommunePic")%>" </a>
                     
                   
                    </td>
                    <td>
                     <a href="/Admin/Commune/CommuneEdit.aspx?AccountNumber=<%#Eval("AccountNumber")%>" >修改</a> 
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
             <tr style='background-color: #c1cdc1; font-weight:bolder;'>
                      <td>
                      统计
                    </td>
                    <td colspan="2" style="height:20px;">
                    <span>社员数：</span><%=IDCount %>
                 
                    </td>
                  
                    <td>
                      
                    </td>
                    <td>
                       
                    </td>
                    <td>
                       
                    </td>
                   <td colspan="2">
                    <span>田亩数:</span>
                     <%=FieldCount %>
                    </td>
                     <td>
                    
                    </td>
                     <td>
                     
                    </td>
                     <td>
                  
                     
                    </td>
                    <td>
                  
                   
                    </td>
                    <td>
                   
                    </td>
                </tr>
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server"
             FirstPageText="首页" LastPageText="尾页"  PrevPageText="上一页" NextPageText="下一页" 
        NumericButtonTextFormatString="[{0}]" PageSize="15" onpagechanging="AspNetPager1_PageChanging" 
               >
            </webdiyer:AspNetPager>
        </div>
       
    </div>
    <div id="divApplication" style="width:420px; height:600px; padding:10px; background-color:#ae8787; position:absolute; left:100px; top:20px; display:none">
    <img id="imgApplication" style="width:420px; height:600px;"  onclick="FunClose();" />
    </div>
    <div id="divCommunePic" style="width:210px; height:300px;  padding:5px; background-color:#ae8787; position:absolute; left:200px; top:50px; display:none">
    <img id="imgCommunePic" style="width:210px; height:300px;"  onclick="FunClose();" />
    </div>
    <div  style="display:none;">
      <%--选择兑换的存储产品信息--%>
     <input type="text" name="txtDep_SID" value="" />

    </div>
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>

