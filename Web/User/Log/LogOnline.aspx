<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogOnline.aspx.cs" Inherits="Web.User.Log.LogOnline" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    
   
    <style type="text/css">
        
    </style>
  
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>用户操作记录</b>
    </div>
  
    <table>
        <tr>
            <td>
         <span>近100次登录当前网点的记录</span>
            </td>
          
          
        </tr>
    </table>
    <asp:Repeater ID="Repeater1" runat="server">
        <HeaderTemplate>
            <table class="tabData">
                <tr class="tr_head">
                    <th style="width: 60px; text-align: center;">
                        序号
                    </th>
                    <th style="width: 100px; text-align: center;">
                        营业员
                    </th>
                     <th style="width: 100px; text-align: center;">
                        登录IP
                    </th>
                     <th style="width: 200px; text-align: center;">
                        登录时间
                    </th>
                     <th style="width: 200px; text-align: center;">
                        退出时间
                    </th>
                     <th style="width: 150px; text-align: center;">
                        在线时长
                    </th>
                  
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr 
                onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
                <td>
                    <%#Container.ItemIndex%>
                </td>
                 <td>
                    <%#Eval("UserID")%>
                </td>
                <td>
                    <%#Eval("IpAddress")%>
                </td>
                <td>
                    <%#Eval("dt_LoginIn")%>
                </td>
                 <td>
                    <%#Eval("dt_LoginOut")%>
                </td>
                 <td>
                    <%#Eval("TimeLength")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <!--底部模板-->
            </table>
            <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>
    
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
