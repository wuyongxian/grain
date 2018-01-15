<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommuneStatistics.aspx.cs" Inherits="Web.Admin.Commune.CommuneStatistics" %>

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

              InitWBID();
              $('#QWBID').change(function () {
                  if ($('#QWBID option:selected').val() != "") {
                      $('#txtWBID').val($('#QWBID option:selected').text());
                  }
              });
          });
          function InitWBID() {
              var WBName = $('#txtWBID').val();
              $('#QWBID').empty();
              $.ajax({
                  url: '/Ashx/wbinfo.ashx?type=GetWBByName&strName=' + WBName,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      if (WBName == '') {
                          $('#QWBID').append("<option value=''>--请选择--</option>");
                      }
                      if (r.responseText == "Error") { return false; }
                      for (var i = 0; i < r.length; i++) {
                          $('#QWBID').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                      }

                  }, error: function (r) {

                  }
              });
          }

      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>社员信息统计</b>
    </div>
   
        <div class="QueryHead">
            <table>
                <tr>
                    <td><span>网点名称:</span>
                    </td>
                    <td>
                        <input type="text" id="txtWBID" onkeyup="InitWBID();" style="font-size: 16px; width: 100px;height:26px; font-weight: bolder;" runat="server" />
                        <select id="QWBID" runat="server" style="width: 100px;height:30px;"></select></td>
                    <td><span>日期:</span>
                    </td>
                    <td>
                        <input type="text" id="Qdtstart" readonly="readonly" onclick="WdatePicker();" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                        <span>-</span>
                        <input type="text" id="Qdtend" readonly="readonly" onclick="WdatePicker()" style="font-size: 16px; width: 100px; font-weight: bolder;" runat="server" />
                    </td>
                    <td style="width: 60px">
                        <asp:ImageButton ID="ImageButton1" runat="server"
                            ImageUrl="~/images/search_red.png" OnClick="ImageButton1_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="StorageList" style="margin:20px 0px">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                    <tr class="tr_head">
                        <th style="width: 150px; height:20px; text-align: center;">
                            网点
                        </th>
                         <th style="width: 100px; text-align: center;">
                            社员数
                        </th>
                        <th style="width: 100px; text-align: center;">
                            田亩数
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                   <td style="height:25px;">
                     
                        <%#Eval("WBID")%>
                    </td>
                    <td>
                        <%#Eval("CommuneCount")%>
                    </td>
                    <td>
                        <%#Eval("FieldCount")%>
                    </td>
                   
                   
                </tr>
            </ItemTemplate>
            <FooterTemplate>
           
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
             <asp:Repeater ID="Repeater2" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                <tr><td></td><td colspan="5" style="text-align:center">总计</td></tr>
                    <tr class="tr_head">
                        <th style="width: 150px;  text-align: center;">
                           
                        </th>
                         <th style="width: 100px; height:20px; text-align: center;">
                            社员数
                        </th>
                        <th style="width: 100px; text-align: center;">
                            田亩数
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                   <td style="height:25px;">
                     
                       
                    </td>
                    <td>
                        <%#Eval("CommuneCount")%>
                    </td>
                    <td>
                        <%#Eval("FieldCount")%>
                    </td>
                  
                </tr>
            </ItemTemplate>
            <FooterTemplate>
           
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
         
        </div>

    
    <div  style="display:none;">
      <%--选择兑换的存储产品信息--%>
     <input type="text" name="txtDep_SID" value="" />
            
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
    </div>
    </form>

</body>
</html>