<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogBusiness.aspx.cs" Inherits="Web.User.Log.LogBusiness" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   
    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>

      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {

          });
          /*--ENd loadFuntion--*/

      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>网点业务日志</b>
      <%--  <span id="spanHelp" style="cursor: pointer">帮助</span>--%>
    </div>
    <div id="divHelp"  class="pageHelp">
<span>提示1：</span><br />
<span>提示2：</span><br />
<span>提示3：</span><br />
</div>

    <div style="margin: 20px 0px;">
    <span>按日期查询</span>
      <input id="txtDate" runat="server" type="text" style="width:120px;" readonly="readonly" onClick="WdatePicker()"/>
       
         
            <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png" 
                runat="server" onclick="ImageButton1_Click" />
        </div>
        
        <div id="StorageLog" runat="server" style="width:500px;  height:25px; display:none; margin:5px 30px;  background-color:#cdcdcd; border-radius:5px; font-weight:bolder; color:Red">当前日期没有存储信息</div>
        <div id="exchangeLog" runat="server" style="width:500px; height:25px;display:none; margin:5px 30px;  background-color:#cdcdcd; border-radius:5px; font-weight:bolder; color:Red">当前日期没有兑换信息</div>
        <div id="SellLog" runat="server" style="width:500px; height:25px;display:none; margin:5px 30px;  background-color:#cdcdcd; border-radius:5px; font-weight:bolder; color:Red">当前日期没有存转销信息</div>
       <div id="StorageList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                <tr><td colspan="8" align="center" style=" height:20px; color:Green; font-weight:bolder; background-color:#cdc9a5">储户存储信息列表</td></tr>
                    <tr class="tr_head">
                    <th style="width: 120px; height:20px; text-align: center;">
                            储户账号
                        </th>
                        <th style="width: 120px;">
                            存贷产品
                        </th>
                        <th style="width: 80px; text-align: center;">
                            结存数量
                        </th>
                        <th style="width: 100px; text-align: center;">
                            存入时间
                        </th>
                        <th style="width: 80px; text-align: center;">
                            存入价
                        </th>
                        <th style="width: 80px; text-align: center;">
                            存期
                        </th>
                      
                        <th style="width: 80px; text-align: center;">
                         营业员
                        </th>
                     
                        
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height:25px;">
                        <%#Eval("AccountNumber")%>
                    </td>
                    <td>
                        <%#Eval("VarietyID")%>
                    </td>
                    <td>
                        <%#Eval("StorageNumber")%>
                    </td>
                    <td>
                        <%#Eval("StorageDate")%>
                    </td>
                    <td>
                        <%#Eval("Price_ShiChang")%>
                    </td>
                    <td>
                        <%#(Eval("TimeID"))%>
                    </td>
                    <td>
                        <%#Eval("UserID")%>
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
      
         <div id="exchangeList" runat="server" style="margin:20px 0px;">
      
            <asp:Repeater ID="R_exchange" runat="server">
                <HeaderTemplate>
                    <table class="tabData">
                     <tr><td colspan="9" align="center" style=" height:20px; color:Green; font-weight:bolder; background-color:#cdc9a5">储户兑换信息列表</td></tr>
                        <tr class="tr_head">
                        <th style="width: 120px; height: 20px; text-align: center;">
                                储户账号
                            </th>
                            <th style="width: 100px;">
                                业务名称
                            </th>
                            <th style="width: 80px; text-align: center;">
                                品名
                            </th>
                            <th style="width: 100px; text-align: center;">
                                单价
                            </th>
                            <th style="width: 80px; text-align: center;">
                                数量
                            </th>
                            <th style="width: 80px; text-align: center;">
                                计价单位
                            </th>
                            <th style="width: 80px; text-align: center;">
                                折合原粮
                            </th>
                            <th style="width: 80px; text-align: center;">
                                收费
                            </th>
                          <th style="width: 80px; text-align: center;">
                                营业员
                            </th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr 
                        onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                        <td style="height: 25px;">
                            <%#Eval("AccountNumber")%>
                        </td>
                        <td >
                            <%#Eval("BusinessName")%>
                        </td>
                        <td>
                            <%#Eval("GoodName")%>
                        </td>
                        <td>
                            <%#Eval("GoodPrice")%>
                        </td>
                        <td>
                            <%#Eval("GoodCount")%>
                        </td>
                        <td>
                            <%#Eval("UnitName")%>
                        </td>
                        <td>
                            <%#Eval("VarietyCount")%>
                        </td>
                        <td>
                            <%#Eval("Money_DuiHuan")%>
                        </td>
                          <td>
                            <%#Eval("UserID")%>
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

      <div id="SellList" runat="server" style="margin:20px 0px;">
        
        
            <asp:Repeater ID="R_Sell" runat="server">
                <HeaderTemplate>
                    <table class="tabData">
                     <tr><td colspan="9" align="center" style=" height:20px; color:Green; font-weight:bolder; background-color:#cdc9a5">储户存转销信息列表</td></tr>
                        <tr class="tr_head">

                            <th style="width: 120px; height: 20px; text-align: center;">
                               储户账号
                            </th>
                             <th style="width: 100px; text-align: center;">
                                结算类型
                            </th>
                            <th style="width: 80px; text-align: center;">
                                结算日期
                            </th>
                            <th style="width: 100px; text-align: center;">
                                存储产品
                            </th>
                            <th style="width: 80px; text-align: center;">
                                实存天数
                            </th>
                            <th style="width: 80px; text-align: center;">
                                结算重量
                            </th>
                            <th style="width: 80px; text-align: center;">
                                利息
                            </th>
                            <th style="width: 80px; text-align: center;">
                                结算金额
                            </th>
                            <th style="width: 80px; text-align: center;">
                                营业员
                            </th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr 
                        onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                        <td style="height: 25px;">
                            <%#Eval("AccountNumber")%>
                        </td>
                         <td>
                            <%#Eval("BusinessName")%>
                        </td>
                        <td>
                            <%#Eval("dt_Sell")%>
                        </td>
                        <td>
                            <%#Eval("VarietyName")%>
                        </td>
                        <td>
                            <%#Eval("StorageDate")%>
                        </td>
                        <td>
                            <%#Eval("VarietyCount")%>
                        </td>
                        <td>
                            <%#Eval("VarietyInterest")%>
                        </td>
                        <td>
                            <%#Eval("VarietyMoney")%>
                        </td>
                        <td>
                            <%#Eval("UserID")%>
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

    </div>
    </form>
    
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
