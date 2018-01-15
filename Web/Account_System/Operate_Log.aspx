<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Operate_Log.aspx.cs" Inherits="Web.Account_System.Operate_Log" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>更改Dep_OperateLog记录</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    uid:<asp:TextBox ID="txtuid" runat="server"></asp:TextBox>&nbsp;
     pwd:<asp:TextBox ID="txtpwd" runat="server"></asp:TextBox>&nbsp;
        <asp:Button ID="btnVerify" runat="server" Text="Verify" 
            onclick="btnVerify_Click" />
    </div>
    <div id="divSql" style="display:none" runat="server">
       
      <%--  <asp:Button ID="btnSql" runat="server" Text="查询库存记录"   onclick="btnSql_Click" />  --%>
         <div style="margin:20px;">
            <span>初始化VarietyID字段</span>
            <asp:Button ID="btnInitStorage" runat="server" Text="VarietyID初始化" OnClick="btnInitStorage_Click" />   
        </div>

         <div style="margin:20px;">
            <span>初始化Dep_SID字段</span>
            <asp:Button ID="btnInitDep_SID" runat="server" Text="Dep_SID初始化" OnClick="btnInitDep_SID_Click" />   
        </div>
          
        <div>
        <%=strContent %>
        </div>
    </div>
    </form>
</body>
</html>