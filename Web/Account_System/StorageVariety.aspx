<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageVariety.aspx.cs" Inherits="Web.Account_System.StorageVariety" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
       
        <asp:Button ID="btnSql" runat="server" Text="查询库存记录" onclick="btnSql_Click" />  
          <asp:Button ID="btnInitStorage" runat="server" Text="初始化SA_VarietyStorageLog" OnClick="btnInitStorage_Click" />   
        <div>
        <%=strContent %>
        </div>
    </div>
    </form>
</body>
</html>