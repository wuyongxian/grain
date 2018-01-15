<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryDepositor.aspx.cs" Inherits="Web.User.Query.QueryDepositor" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

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


          });




      </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>储户信息查询</b>
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
            <span>村名:</span>
            <input type="text" id="QCunName" style="font-size:16px; width:100px; font-weight:bolder;" runat="server" />&nbsp;
            <span>账号:</span>
            <input type="text" id="QAccountNumber" style="font-size:16px;width:100px; font-weight:bolder;" runat="server" />&nbsp;
           <span>姓名:</span>
            <input type="text" id="QName" style="font-size:16px;width:100px; font-weight:bolder;" runat="server" />&nbsp;
             <span>身份证号:</span>
            <input type="text" id="QIDCard" style="font-size:16px;width:150px; font-weight:bolder;" runat="server" />


            <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png" 
                runat="server" onclick="ImageButton1_Click" />
        </div>
        
        <div id="StorageList" style="margin:20px 0px">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                    <tr class="tr_head">
                        <th style="width: 120px; height:20px; text-align: center;">
                            储户账号
                        </th>
                        <th style="width: 100px; text-align: center;">
                            储户姓名
                        </th>
                        <th style="width: 100px; text-align: center;">
                            村名
                        </th>
                        <th style="width: 100px; text-align: center;">
                            存入日期
                        </th>
                        <th style="width: 80px; text-align: center;">
                            存储类型
                        </th>
                         <th style="width: 80px; text-align: center;">
                            存期
                        </th>
                        <th style="width: 80px; text-align: center;">
                            存储产品
                        </th>
                       
                        <th style="width: 100px; text-align: center;">
                            当前结存
                        </th>
                       
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                    <td style="height:25px;">
                       <a href="DepositorInfo.aspx?Account=<%#Eval("AccountNumber") %>"><%#Eval("AccountNumber") %></a>
                    </td>
                    <td>
                        <%#Eval("strName")%>
                    </td>
                    <td>
                        <%#Eval("CunID")%>
                    </td>
                    <td>
                        <%#Eval("StorageDate")%>
                    </td>
                    <td>
                        <%#Eval("TypeID")%>
                    </td>
                    <td>
                        <%#(Eval("TimeID"))%>
                    </td>
                    <td>
                        <%#Eval("VarietyID")%>
                    </td>
                   <td>
                        <%#Eval("StorageNumber")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
           
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server"
             FirstPageText="首页" LastPageText="尾页"  PrevPageText="上一页" NextPageText="下一页" 
        NumericButtonTextFormatString="[{0}]" PageSize="15" 
                onpagechanging="AspNetPager1_PageChanging" >
            </webdiyer:AspNetPager>
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

