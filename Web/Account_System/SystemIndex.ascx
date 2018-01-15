<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SystemIndex.ascx.cs" Inherits="Web.Account.SystemIndex1" %>
<script src="../Scripts/jquery.js" type="text/javascript"></script>
 <style>
        *
        {
            margin: 0;
            padding: 0;
            font-weight: normal;
            font-family: inherit;
        }
        body
        {
            font-size: 14px;
            font-weight: bolder;
            font-family: Arial, Hel,Microsoft YaHei;
        }
        .nav
        {
            width:400px;
            margin: 0 auto;
            position: relative;
             background:#eee;
              border:1px solid #aaa;
              border-radius:5px;
              box-shadow:0px 0px 10px #888;
        }
        .nav ul
        {
            list-style: none;
        }
        .nav ul li
        {
            float: left;
            padding: 0 20px;
            text-align: center;
            height: 40px;
            line-height: 40px;
        }
        .nav ul li a
        {
            text-decoration: none;
            color: #111;
        }
        .nav ul li a:hover
        {
            color: #ee7700;
        }
        .cls
        {
            clear: both;
        }
        .curBg
        {
            background: #cc6600;
            height: 4px;
            position: absolute;
            bottom: 0;
            width: 68px;
            left: 0px;
            border-radius: 4px;
        }
    </style>


<div align="center">
  
<div class="nav">
    <ul>
        <li><a href="../Account_System/Menu_Admin.aspx">管理员网站管理</a></li>
        <li><a href="../Account_System/Menu_User.aspx">营业员网站管理</a></li><%--
        <li><a href="../Account_System/Authority_Admin.aspx">管理员权限管理</a></li>
        <li><a href="../Account_System/Authority_User.aspx">营业员权限管理</a></li>--%>
         <li><a href="../sysindex.html?Type=loginOut">退出</a></li>
    </ul> 
    <div class="curBg"></div>
    <div class="cls"></div>
</div>  

</div>