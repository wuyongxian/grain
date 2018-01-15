<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemIndex.aspx.cs" Inherits="Web.Account.SystemIndex" %>
<%@ Register Src="~/Account_System/SystemIndex.ascx" TagName="SysIndex" TagPrefix="sIndex"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
   
    <sIndex:SysIndex ID="sysindex1" runat="server" />
    </div>
    </form>
</body>
</html>
