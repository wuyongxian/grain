<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WBSimulate.aspx.cs" Inherits="Web.Admin.WebSiteSetting.WBSimulate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
      <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
     <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   
    <style type="text/css">

</style>
<script type="text/javascript">
 
    function HelpOpen() {
        $('#divHelp').fadeIn();
    };

    function HelpClose() {
        $('#divHelp').fadeOut();
    };

    $(function () {
        $.ajax({
            url: 'ws.ashx?type=getWB',
            type: 'post',
            data: '',
            dataType: 'json',
            success: function (r) {
                for (var i = 0; i < r.length; i++) {
                    $('select[name=WBID]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                }
                for (var i = 0; i < r.length; i++) {
                    if (r[i].ISHQ == 1) {
                        $('#spanHQWB').html(r[i].strName);
                    }
                    $('select[name=WBIDHQ]').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
                }
            }, error: function (r) {
               showMsg('加载信息失败 ！');
            }
        });
    });

    //设定模拟网点
    function SetSimulate() {
        var msg = '您确认要将此网点设为模拟网点吗？';
        showConfirm(msg, function (obj) {
            if (obj == 'yes') {
                
                var WBID = $('select[name=WBID] option:selected').val();

                $.ajax({
                    url: 'ws.ashx?type=SetSimulate&ID=' + WBID + "&ISSimulate=1",
                    type: 'post',
                    data: '',
                    dataType: 'text',
                    success: function (r) {
                        if (r == "HQ") {
                            showMsg('您选择的网点是总部，不可以设为模拟网点 ！');
                        } else {
                            showMsg('设置模拟网点成功 ！');
                        }
                    }, error: function (r) {
                        showMsg('设置模拟网点失败 ！');
                    }
                });
            } else {
                //console.log('你点击了取消！');
            }

        });
  }

  //设定总部网点
  function SetHQ() {
     
          var wbid = $('select[name=WBIDHQ] option:selected').val();
          var msg = '您确认此网点设置为总部吗？';
          showConfirm(msg, function (obj) {
              if (obj == 'yes') {
                  
                  $.ajax({
                      url: 'ws.ashx?type=SetHQ&ID=' + wbid,
                      type: 'post',
                      data: '',
                      dataType: 'text',
                      success: function (r) {
                          if (r == "OK") {
                              $('#spanHQWB').html($('select[name=WBIDHQ] option:selected').text());
                              showMsg('已将此网点设置为总部！');

                          } else if (r == "S") {
                              showMsg('当前的网点是模拟网点，不可以设为总部 ！');
                          } else {
                              showMsg('设置总部网点失败 ！');
                          }
                      }, error: function (r) {
                          showMsg('设置总部网点失败 ！');
                      }
                  });
              } else {
                  //console.log('你点击了取消！');
              }

          });
  }


  function CancelSimulate() {
      var msg = '您确认要将此网点设为普通网点吗？';
      showConfirm(msg, function (obj) {
          if (obj == 'yes') {
              
              var WBID = $('select[name=WBID] option:selected').val();

              $.ajax({
                  url: 'ws.ashx?type=SetSimulate&ID=' + WBID + "&ISSimulate=0",
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      showMsg('设置网点成功 ！');
                  }, error: function (r) {
                      showMsg('设置网点失败 ！');
                  }
              });
          } else {
              //console.log('你点击了取消！');
          }

      });
  }
</script>
</head>
<body>
   <form id="form1" runat="server">
<div class="pageHead">
<b>网点设置</b><%--<span style="cursor:pointer" onclick="HelpOpen();">帮助</span>--%>
</div>
<div id="divHelp" style="border:1px solid #333; border-radius:5px; display:none; ">
<div style="float:right; margin:10px 20px;"><input type="button"  value="关闭" onclick="HelpClose();" /></div>
<span>提示1：</span><br />
<span>提示2：</span><br />
<span>提示3：</span><br />
</div>

  <div  id="div1" class="pageEidtInner" style="width:600px;" >
 
    <div>
        <table class="tabEdit">
        <tr>
            <td style="width:100px"></td>
            <td>
            <span style="font-weight:bolder;">总部网点设置</span>
            </td>
            </tr>
             <tr>
            <td style="width:100px"><span>当前总部网点:</span></td>
            <td>
            <span id="spanHQWB" style="color:Green"></span>
            </td>
            </tr>
            <tr>
            <td style="width:100px"><span>选择网点:</span></td>
            <td><select name="WBIDHQ"></select>
            <span style="color:Red">请慎重选择</span>
            </td>
            </tr>
           
               <tr>
            <td>
                
               </td>
            <td ><input type="button" id="Button2" value="设定总部网点" onclick="SetHQ()" />
         
             </td>
            </tr>
        </table>
        </div>
    </div>
   <div style="margin-bottom:20px;">
                <div><span style="color:Red; font-weight:bolder;">说明</span></div>
                <div><span>1:非首次设置系统，请勿更改总部网点。</span></div>
                  <div><span>2:单位管理员和网点管理员必须是总部网点的账号。</span></div>
                <div><span>3:如果总部网点更改，则总部网点下的所有单位管理员和网点管理员将被同步迁移到新的网点。</span></div>
                    </div>
    <div  id="divfrm" class="pageEidtInner" style="width:600px;" >
 
    <div>
           <table class="tabEdit">
        <tr>
            <td style="width:100px"></td>
            <td>
            <span style="font-weight:bolder;">模拟网点设置</span>
            </td>
            </tr>
            <tr>
            <td style="width:100px"><span>选择网点:</span></td>
            <td><select name="WBID"></select>
            <span style="color:Red">请慎重选择</span>
            </td>
            </tr>
           
               <tr>
            <td></td>
            <td ><input type="button" id="btnUpdate" value="设定模拟网点" onclick="SetSimulate()" />&nbsp;
           <input type="button" id="Button1" value="取消设定" onclick="CancelSimulate()" />
             </td>
            </tr>
        </table>
        </div>
         
    </div>
       <div style="margin-bottom:20px;">
                <div><span style="color:Red; font-weight:bolder;">说明</span></div>
                <div><span>1:模拟网点用于设置模拟操作，非必要时请勿随意设置。</span></div>
                  <div><span>2:模拟网点下产生的数据不参与报表统计。</span></div>
                <div><span>3:请勿将正在使用的网点设置成模拟网点。</span></div>
                    </div>
    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>
