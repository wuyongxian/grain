<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InfoList.aspx.cs" Inherits="Web.Admin.Info.InfoList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    
   
    <style type="text/css">
        
    </style>
    <script type="text/javascript">

        $(function () {

        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pageHead">
       <b>信息交流</b>
    </div>

    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
         <table class="tabData">
            <tr style='background-color:#efefef; height:30px;'
               >
                <td style="text-align:left">
                &nbsp;   &nbsp;<%#ISStick(Eval("ISStick"))%>
                <%#GetLinkUrl(Eval("ID"),Eval("strTitle") )%>
               &nbsp;
                 浏览:<%#Eval("BrowseTime")%>&nbsp;
                 发布时间:<%#Eval("dt_Add")%></td>
               
            </tr>
            <tr style="background-color:#fcfcfc" >
            <td style="width:1000px; text-align:left;"> 
            <div style="margin:10px 0px 20px 50px">
            导读：<%#Web.Fun.CutString( Eval("strContent"),100) %><%#GetLinkUrl(Eval("ID")) %>
            </div>
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
       FirstPageText="首页" LastPageText="尾页"  PrevPageText="上一页" NextPageText="下一页" NumericButtonTextFormatString="[{0}]"
        onpagechanging="AspNetPager1_PageChanging">
    </webdiyer:AspNetPager>
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>


