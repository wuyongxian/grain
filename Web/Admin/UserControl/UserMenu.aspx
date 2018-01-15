<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserMenu.aspx.cs" Inherits="Web.Admin.UserControl.UserMenu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>管理员菜单管理</title>
    
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../Styles/Common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        table.border {
            border-collapse: collapse;
            border: 1px solid #9CF;
            text-align: center;
        }

            table.border thead td, table.set_border th {
                font-weight: bold;
                background-color: White;
            }

            table.border tr:nth-child(even) {
                background-color: #EAF2D3;
            }

            table.border td, table.border th {
                border: 1px solid #C3C3C3;
                text-align: center;
            }

        #divADDMenu {
            width: 400px;
            height: 300px;
            position: absolute;
            left: 200px;
            top: 50px;
            background-color: #ccc;
            border-radius: 10px;
            z-index: 1000;
        }

        .tabAddMenu {
            margin: 30px 50px 10px 50px;
            font-size: 18px;
        }

            .tabAddMenu tr td {
                height: 50px;
            }

                .tabAddMenu tr td input {
                    border-radius: 2px;
                    padding-left: 10px;
                    font-size: 18px;
                    width: 300px;
                }
    </style>
    <script type="text/javascript">
        $(function () {
            var UserID = getQueryString("UserID");
            //var UserID = 14;
            if (UserID != "") {
                $('#UserID').val(UserID);
            }
             var numtype = getQueryString("numtype");
           // var numtype = 2;
            if (numtype != "") {
                $('#numtype').val(numtype);
            }
            var url = 'user.ashx?type=getMenuAdminAll&PID=0&UserID=' + $('#UserID').val();
            if (numtype == 2) {
                url = 'user.ashx?type=getMenuUserAll&PID=0&UserID=' + $('#UserID').val();
            }

            $.ajax({
                url: url,
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                   
                    $('#divMenu').append(r);
                  
                },
                error: function (r) { showMsg(" 加载信息失败……"); }

            });

        });



        /*
        PID:父节点id
        imgID：父节点左侧图片的id
        tabID：父节点table的id
        numLevel:这是第几级菜单
        */
        function OpenChildMenu(PID, imgID, tabID, numLevel) {

            if ($('#' + imgID).attr('alt') == "Open") {
                $('#' + imgID).attr('alt', 'Close');
                $('#' + imgID).attr('src', '../../images/menuClose.png');
                $('#div_' + PID).slideDown("normal");

            } else {
                $('#' + imgID).attr('alt', 'Open');
                $('#' + imgID).attr('src', '../../images/menuOpen.png');
                $('#div_' + PID).slideUp("normal");
            }
        }

        function frmSubmit() {
            var url = 'user.ashx?type=updateMenuAdmin&UserID=' + $('#UserID').val();
            if ($('#numtype').val() == 2) {
                url = 'user.ashx?type=updateMenuUser&UserID=' + $('#UserID').val();
            }
         
            $.ajax({
                url: url,
                type: 'post',
                data: $('#form1').serialize(),
                dataType: 'text',
                success: function (r) { showMsg("提交成功!"); }, error: function (r) { showMsg("提交失败!"); }
            });
        };


        function FunQuanXuan() {
            $(":checkbox").each(function (i) {
                $(this).attr('checked', 'checked');
            });
        }

        function FunQuanBuXuan() {
            $(":checkbox").each(function (i) {
                $(this).removeAttr('checked');
            });
        }

        function FunFanXuan() {
            $(":checkbox").each(function (i) {
                if ($(this).attr("checked")) {
                    $(this).removeAttr('checked');
                } else {
                    $(this).attr('checked', 'checked');
                }
            });
           
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    
   <div class="usermenu">
     <div id="divMenu" >
    
    </div>
   <div style="margin:20px 20px 50px 100px">
   <a href="#" onclick="FunQuanXuan()">全选</a>&nbsp;
    <a href="#" onclick="FunQuanBuXuan()">全不选</a>&nbsp;
     <a href="#" onclick="FunFanXuan()">反选</a>&nbsp;&nbsp;
  <input style=" width:100px; height:30px; font-size:16px; font-weight:bold; color:Green" type="button" value="提交" onclick="frmSubmit()" />
   </div>
       </div>
 <div style="display:none">
 
  <input type="text" id="numtype" value="" />
 <input type="text" id="UserID" value="" />
 </div>
    </form>
</body>
</html>