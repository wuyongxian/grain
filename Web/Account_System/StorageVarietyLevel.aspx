<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageVarietyLevel.aspx.cs" Inherits="Web.Account_System.StorageVarietyLevel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            uid:<asp:TextBox ID="txtuid" runat="server"></asp:TextBox>&nbsp;
     pwd:<asp:TextBox ID="txtpwd" runat="server"></asp:TextBox>&nbsp;
        <asp:Button ID="btnVerify" runat="server" Text="Verify"
            OnClick="btnVerify_Click" />
        </div>
        <div id="divSql" style="display: none" runat="server">


            <div style="margin: 20px;">
                <span>初始化Dep_StorageInfo表字段VarietyLevelID</span>
                <asp:Button ID="btnInitStorage" Enabled="false" runat="server" Text="执行" OnClick="btnInitStorage_Click" />
            </div>

            <div style="margin: 20px;">
                <span>初始化SA_VarietyStorage\SA_VarietyStorageLog\SA_CheckOut表字段VarietyLevelID</span>
                <asp:Button ID="btnStorageLevel"  runat="server" Text="执行" OnClick="btnStorageLevel_Click"  />
            </div>
        </div>
    </form>
</body>
</html>
