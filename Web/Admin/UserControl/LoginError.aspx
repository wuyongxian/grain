<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginError.aspx.cs" Inherits="Web.Admin.UserControl.LoginError" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    
   
    <style type="text/css">

</style>
    <script type="text/javascript">
        function ClearError(ID) {
            $.ajax({
                url: '/Admin/UserControl/user.ashx?type=ClearError&ID='+ID,
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                   showMsg('OK,已为该营业员解除登录限制 ！');
                }, error: function (r) {
                   showMsg('Error,解除营业员登录限制失败 ！');
                }
            });
        }
    </script>
</head>
<body>
     <form id="form1" runat="server">
<div class="pageHead">
<b>营业员异常登陆管理</b>
</div>


<div class="QueryHead">
<table>
            <tr>
            <td><span>网点名称:</span></td>
            <td><span><input type="text" id="WBName" style="width:80px;" runat="server" />  </span></td>
            <td><span>营业员登陆名:</span></td>
            <td><span><input type="text" id="UserLoginName" style="width:80px;" runat="server" />  </span></td>
            <td style="width:60px">
                <asp:ImageButton ID="ImageButton1" runat="server" 
                    ImageUrl="~/images/search_red.png" onclick="ImageButton1_Click"  />
                </td>
           
            </tr>
            
        </table>
</div>
<asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table  class="tabData" style="width:900px">
          <tr class="tr_head" >
                <th style="width:100px; text-align:center;">
                    网点</th>
                <th style="width:100px; text-align:center;">
                    登陆名</th>
                <th style="width:100px; text-align:center;">
                    真实姓名</th>
                     <th style="width:100px; text-align:center;">
                    最近登陆时间</th>
                <th style="width:100px; text-align:center;">
                    错误次数</th>
                    <th style="width:100px; text-align:center;">
                    状态</th>
                   
                    <th style="width:100px; text-align:center;">
                    操作</th>
                   
                
            </tr>
        
    </HeaderTemplate>
    <ItemTemplate>
    <tr  onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">
        <td><%#Eval("WBName")%></td>
        <td><%#Eval("LoginName")%></td>
        <td><%#Eval("RealName")%></td>
        <td><%#Eval("dt_LoginIn")%></td>
        <td><%#Eval("ErrorTime")%></td>
        <td><%#GetState(Eval("ErrorTime"), Eval("dt_LoginIn"))%></td>
        <td><input type="button" value="解除" style="width:80px; height:25px;" onclick="ClearError(<%#Eval("ID") %>)" /></td>
   
    </tr>
    </ItemTemplate>
    
    <FooterTemplate><!--底部模板-->
        </table>        <!--表格结束部分-->
        </FooterTemplate>
    </asp:Repeater>
     <webdiyer:AspNetPager ID="AspNetPager1" runat="server"
             FirstPageText="首页" LastPageText="尾页"  PrevPageText="上一页" NextPageText="下一页" 
        NumericButtonTextFormatString="[{0}]" PageSize="15" onpagechanging="AspNetPager1_PageChanging" 
               >
            </webdiyer:AspNetPager>
    <div  id="divfrm" class="pageEidt" style="display:none; width:410px; height:420px;">
    <div style="float:right; margin:10px 20px"> <img src="../../images/winClose.png" alt="关闭窗口"  style="float:right; cursor:pointer;" onclick="CloseFrm()" /></div>    
   
  
    </div>

    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>

