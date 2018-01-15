<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="strSql2.aspx.cs" Inherits="Web.Account_System.strSql2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

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
        <asp:TextBox ID="txtSql" TextMode="MultiLine"  Width="600px" Height="150px" runat="server"></asp:TextBox>
        <asp:Button ID="btnSql" runat="server" Text="SQL" onclick="btnSql_Click" />   
        <div>
        <%=strContent %>
        </div>
    </div>
    </form>
</body>
</html>
