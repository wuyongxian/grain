<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AToSell.aspx.cs" Inherits="Web.Admin.Approval.AToSell" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   

      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {
              var url = '/ashx/exchangeprop.ashx?type=UpdateAdviceState_Request';
              $.ajax({
                  url: url,
                  type: 'post',
                  data: '',
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

          });

          function FunApply(ID) {
              $('#WBID').val(ID);
              showBodyCenter($('#divfrm'));
          }

          function FunSubmit() {
              var ID = $('#WBID').val();
              var Apply = '';
              var chkObjs = document.getElementsByName("Apply");
              for (var i = 0; i < chkObjs.length; i++) {
                  if (chkObjs[i].checked) {
                      Apply = chkObjs[i].value;
                  }
              }

              $.ajax({
                  url: '/Ashx/exchangeprop.ashx?type=ApprovalApply&Apply=' + Apply + "&ID=" + ID,
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                     showMsg('审批成功 ！');
                      location.reload();
                  }, error: function (r) {
                     showMsg('审批失败 ！');
                  }
              });
          }



      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>储户存转销审核</b>
    </div>
    
<div id="storageQuery">

</div>
    <div style="margin: 20px 0px;">
      
        
        <div id="StorageList" style="margin:20px 0px">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                    <tr class="tr_head">
                    <th style="width: 100px; height:20px; text-align: center;">
                            网点名称
                        </th>
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
                            申请价格
                        </th>
                        <th style="width: 80px; text-align: center;">
                            申请金额
                        </th>
                        <th style="width: 80px; text-align: center;">
                            计量单位
                        </th>
                        <th style="width: 80px; text-align: center;">
                            申请日期
                        </th>
                       
                        <th style="width: 100px; text-align: center;">
                            申请状态
                        </th>
                        <th style="width: 70px; text-align: center;">
                            操作
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height:30px;">
                        <%#Eval("WBID")%>
                    </td>
                    <td >
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
                        <%#Eval("ApplyPrice")%>
                    </td>
                     <td>
                        <%#Eval("ApplyMoney")%>
                    </td>
                    <td>
                        <%#Eval("UnitName")%>
                    </td>
                    <td>
                        <%#(Eval("ApplyDate"))%>
                    </td>
                    <td>
                        <%#Eval("strApplyState")%>
                    </td>
                    <td>
                    <input type="button" value="审批" style=" width:60px;" onclick="FunApply(<%#Eval("ID") %>)" />
                  
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
     
       <div id="divfrm" class="pageEidt" style="display: none;">
        <div style="float: right;">
           <img  class="imgclose" src="../../images/winClose.png" alt="关闭窗口"  style="float:right; cursor:pointer;" onclick="CloseFrm()" /></div>
        <div style="clear: both;">
            <table class="tabEdit">
                <tr>
                    <td align="right" style="width: 100px;">
                        <span>审批人:</span>
                    </td>
                    <td>
                      <span><%=Approval %></span>
                    </td>
                </tr>
                <tr>
                   <td align="right">
                        <span>审批时间:</span>
                    </td>
                    <td>
                        <span><%=dt_Apply %></span>
                    </td>
                </tr>
                <tr>
                  <td align="right">
                        <span>审批:</span>
                    </td>
                    <td>
                  <%--     <input type="radio" name="Apply" checked="checked" value="1" />通过
                       <input type="radio" name="Apply" value="0" />拒绝--%>
                         <input type="radio" id="InterestType1"  name="Apply" value="1" class="custom-radio" style="margin-left:20px;"  /><label for="InterestType1"></label><span>通过</span>
                         <input type="radio" id="InterestType2"  name="Apply" value="0" class="custom-radio" style="margin-left:20px;"  /><label for="InterestType2"></label><span>拒绝</span>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="button" id="btnAdd" value="提交" onclick="FunSubmit()" />
                    </td>
                </tr>
                
            </table>
        </div>
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