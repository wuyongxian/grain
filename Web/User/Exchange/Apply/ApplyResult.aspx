<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyResult.aspx.cs" Inherits="Web.User.Exchange.Apply.ApplyResult" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
   
    

      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {
              var AccountNumber = getQueryString("AccountNumber");
              if (AccountNumber != '') {
                  $('#QAccountNumber').val(AccountNumber);
                  //将当前储户的存转销申请设为已读
                  var url = '/ashx/exchangeprop.ashx?type=UpdateAdviceState_Response';
                  var para = 'AccountNumber=' + AccountNumber;
                  $.ajax({
                      url: url,
                      type: 'post',
                      data: para,
                      dataType: 'json',
                      success: function (r) {
                          if (r == "OK") {
                              console.log('----------将当前储户的存转销申请设为已读成功!');
                          } else {
                              console.log('----------将当前储户的存转销申请设为已读失败!');
                          }
                      }, error: function (r) {
                          console.log('----------将当前储户的存转销申请设为已读失败!');
                      }
                  });
              }
          });




      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>存转销审核结果</b>
    </div>
    
<div id="storageQuery">

</div>
    <div style="margin: 20px 0px;">
        <div id="Query">
            <span>储户账号:</span>
            <input type="text" id="QAccountNumber" style="font-size:16px; font-weight:bolder;" runat="server" />
          
            <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png" 
                runat="server" onclick="ImageButton1_Click" />
        </div>
        
        <div id="StorageList" style="margin:20px 0px">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                    <tr class="tr_head">
                        <th style="width: 100px; height:20px; text-align: center;">
                            储户账号
                        </th>
                        <th style="width: 100px; text-align: center;">
                            储户姓名
                        </th>
                        <th style="width: 80px; text-align: center;">
                            申请业务
                        </th>
                        <th style="width: 100px; text-align: center;">
                            申请商品
                        </th>
                        <th style="width: 80px; text-align: center;">
                            申请数量
                        </th>
                        <th style="width: 80px; text-align: center;">
                            申请日期
                        </th>
                       
                        <th style="width: 100px; text-align: center;">
                            申请状态
                        </th>
                        <th style="width: 160px; text-align: center;">
                            操作
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height:25px;">
                        <%#Eval("Dep_AccountNumber")%>
                    </td>
                    <td>
                        <%#Eval("Dep_Name")%>
                    </td>
                    <td>
                        <%#Eval("ApplyType")%>
                    </td>
                    <td>
                        <%#Eval("VarietyName")%>
                    </td>
                    <td>
                        <%#Eval("VarietyCount")%>
                    </td>
                    <td>
                        <%#(Eval("ApplyDate"))%>
                    </td>
                    <td>
                        <%#Eval("strApplyState")%>
                    </td>
                    <td>
                    <%#GetLink(Eval("ID"), Eval("Dep_SID"), Eval("ApplyState"))%>
                   <%-- <a  style="display:none" href="/User/Exchange/StoreToSell.aspx?Dep_SID=<%#Eval("ID") %>&ISApply=1">存转销</a>--%>
                       
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
