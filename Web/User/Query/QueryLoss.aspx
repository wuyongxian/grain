<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryLoss.aspx.cs" Inherits="Web.User.Query.QueryLoss" %>

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
        <b>网点挂失储户列表</b>
    </div>
    
<div id="storageQuery">

</div>
    <div style="margin: 20px 0px;">
       
        
        <div id="StorageList" style="margin:20px 0px">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData" >
                    <tr class="tr_head">
                        <th style="width: 150px; height:20px; text-align: center;">
                            储户账号
                        </th>
                        <th style="width: 100px; text-align: center;">
                            储户姓名
                        </th>
                        <th style="width: 80px; text-align: center;">
                            乡名
                        </th>
                        <th style="width: 180px; text-align: center;">
                            村名
                        </th>
                        <th style="width: 100px; text-align: center;">
                            手机号
                        </th>
                        <th style="width: 150px; text-align: center;">
                            住址
                        </th>
                       
                        <th style="width: 80px; text-align: center;">
                            状态
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
                        <%#Eval("XiangID")%>
                    </td>
                    <td>
                        <%#Eval("CunID")%>
                    </td>
                    <td>
                        <%#Eval("PhoneNO")%>
                    </td>
                    <td>
                        <%#(Eval("strAddress"))%>
                    </td>
                    <td>
                        <%#Eval("numState")%>
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
