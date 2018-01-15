<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintSettingModel.aspx.cs" Inherits="Web.User.SystemControl.PrintSettingModel" %>

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


        $(function () {
            InitPrintSetting();
        });


        function InitPrintSetting() {
            $.ajax({
                url: '/Ashx/wbinfo.ashx?type=GetPrintSettingModel',
                type: 'post',
                data: '',
                dataType: 'json',
                success: function (r) {

                    $('input[name=Width]').val(r[0].Width);
                    $('input[name=Height]').val(r[0].Height);
                    $('input[name=DriftRateX]').val(r[0].DriftRateX);
                    $('input[name=DriftRateY]').val(r[0].DriftRateY);
                    $('input[name=FontSize]').val(r[0].FontSize);

                    $('input[name=HomeR1C1X]').val(r[0].HomeR1C1X);
                    $('input[name=HomeR1C1Y]').val(r[0].HomeR1C1Y);
                    $('input[name=HomeR1C2X]').val(r[0].HomeR1C2X);
                    $('input[name=HomeR1C2Y]').val(r[0].HomeR1C2Y);
                    $('input[name=HomeR2C1X]').val(r[0].HomeR2C1X);
                    $('input[name=HomeR2C1Y]').val(r[0].HomeR2C1Y);
                    $('input[name=HomeR2C2X]').val(r[0].HomeR2C2X);
                    $('input[name=HomeR2C2Y]').val(r[0].HomeR2C2Y);
                    $('input[name=HomeR3C1X]').val(r[0].HomeR3C1X);
                    $('input[name=HomeR3C1Y]').val(r[0].HomeR3C1Y);
                    $('input[name=HomeR3C2X]').val(r[0].HomeR3C2X);
                    $('input[name=HomeR3C2Y]').val(r[0].HomeR3C2Y);
                    $('input[name=HomeR4C1X]').val(r[0].HomeR4C1X);
                    $('input[name=HomeR4C1Y]').val(r[0].HomeR4C1Y);
                    $('input[name=HomeR4C2X]').val(r[0].HomeR4C2X);
                    $('input[name=HomeR4C2Y]').val(r[0].HomeR4C2Y);
                    $('input[name=HomeR5C1X]').val(r[0].HomeR5C1X);
                    $('input[name=HomeR5C1Y]').val(r[0].HomeR5C1Y);
                    $('input[name=HomeR5C2X]').val(r[0].HomeR5C2X);
                    $('input[name=HomeR5C2Y]').val(r[0].HomeR5C2Y);

                    $('input[name=RecordR1Y]').val(r[0].RecordR1Y);
                    $('input[name=RecordR2Y]').val(r[0].RecordR2Y);
                    $('input[name=RecordR3Y]').val(r[0].RecordR3Y);
                    $('input[name=RecordR4Y]').val(r[0].RecordR4Y);
                    $('input[name=RecordR5Y]').val(r[0].RecordR5Y);
                    $('input[name=RecordR6Y]').val(r[0].RecordR6Y);
                    $('input[name=RecordR7Y]').val(r[0].RecordR7Y);
                    $('input[name=RecordR8Y]').val(r[0].RecordR8Y);
                    $('input[name=RecordR9Y]').val(r[0].RecordR9Y);
                    $('input[name=RecordR10Y]').val(r[0].RecordR10Y);
                    $('input[name=RecordR11Y]').val(r[0].RecordR11Y);
                    $('input[name=RecordR12Y]').val(r[0].RecordR12Y);
                    $('input[name=RecordR13Y]').val(r[0].RecordR13Y);
                    $('input[name=RecordR14Y]').val(r[0].RecordR14Y);
                    $('input[name=RecordR15Y]').val(r[0].RecordR15Y);
                    $('input[name=RecordR16Y]').val(r[0].RecordR16Y);
                    $('input[name=RecordR17Y]').val(r[0].RecordR17Y);
                    $('input[name=RecordR18Y]').val(r[0].RecordR18Y);
                    $('input[name=RecordR19Y]').val(r[0].RecordR19Y);
                    $('input[name=RecordR20Y]').val(r[0].RecordR20Y);

                    $('input[name=RecordC1X]').val(r[0].RecordC1X);
                    $('input[name=RecordC2X]').val(r[0].RecordC2X);
                    $('input[name=RecordC3X]').val(r[0].RecordC3X);
                    $('input[name=RecordC4X]').val(r[0].RecordC4X);
                    $('input[name=RecordC5X]').val(r[0].RecordC5X);
                    $('input[name=RecordC6X]').val(r[0].RecordC6X);
                    $('input[name=RecordC7X]').val(r[0].RecordC7X);
                    $('input[name=RecordC8X]').val(r[0].RecordC8X);
                    $('input[name=RecordC9X]').val(r[0].RecordC9X);


                }, error: function (r) {
                   showMsg('加载打印坐标时出现错误 ！');
                }
            });
        }

        //添加、修改网点 （ID=0是添加网点）
        function WBUpdate() {
            if (!submitCheck()) {
                return false;
            }
            var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
            showConfirm(msg, function (obj) {
                if (obj == 'yes') {
                    
                    $.ajax({
                        url: '/Ashx/wbinfo.ashx?type=UpdatePrintSettingModel',
                        type: 'post',
                        data: $('#form1').serialize(),
                        dataType: 'text',
                        success: function (r) {
                            showMsg('更新数据成功 ！');

                        }, error: function (r) {
                            showMsg('操作失败 ！');
                        }
                    });
                } else {
                    //console.log('你点击了取消！');
                }
            });
        }

        //提交检测

        function CheckInt(obj) {
            if (isNaN(obj.value)) {
                obj.value = '0';
            }
        }

        function submitCheck() {
            //y坐标判断
            var ay = new Array();
            for (var i = 1; i < 21; i++) {
                ay.push($('input[name=RecordR' + i + 'Y]').val());
            }
            for (var i = 1; i < ay.length; i++) {
                if (parseInt(ay[i]) < parseInt(ay[i - 1])) {
                    showMsg('存取记录第[' + parseInt(i + 1) + ']行的Y坐标不能小于第[' + i + ']行的Y坐标,请重新设置!');

                    return false;
                }
            }

            //x坐标判断
            var ax = new Array();
            for (var i = 1; i < 10; i++) {
                ax.push($('input[name=RecordC' + i + 'X]').val());
            }
            for (var i = 1; i < ax.length; i++) {
                if (parseInt(ax[i]) < parseInt(ax[i - 1])) {
                    showMsg('存取记录第[' + parseInt(i + 1) + ']列的X坐标不能小于第[' + i + ']列的X坐标,请重新设置!');
                    return false;
                }
            }
            return true;
        }

    </script>
</head>
<body>
     <form id="form1" runat="server">
<div class="pageHead">
<b style="color:Red">社员</b><b>存折打印坐标模板设置</b>
</div>
<div id="divHelp"  class="pageHelp">

<span>提示1：密码要有一定难度（英文+数字+标点符号），不能是自己的姓名、出生日期、电话号码。请牢记你的密码，如果忘记无法找回。
</span><br />

</div>


 
    <div id="divfrm" >
        <table class="tabEdit" style="margin:5px 0px;">
            <tr>
                <td align="right" style="width: 100px;">
                    <span>存折宽度</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="Width"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 100px;">
                    <span>存折高度</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="Height"
                        onkeyup="CheckInt(this);" />
                </td>
            </tr>
             <tr>
                <td align="right" style="width: 100px;">
                    <span>X轴偏移</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="DriftRateX"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 100px;">
                    <span>Y轴偏移</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="DriftRateY"
                        onkeyup="CheckInt(this);" />
                </td>
            </tr>
            </table>

        <table class="tabEdit"  style="margin:5px 0px;">
            <tr>
                <td align="right" style="width: 180px;">
                    <span>首页第1行第1列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR1C1X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR1C1Y"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 120px;">
                    <span>第1行第2列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR1C2X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR1C2Y"
                        onkeyup="CheckInt(this);" />
                </td> 
            </tr>

             <tr>
                <td align="right" style="width: 180px;">
                    <span>首页第2行第1列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR2C1X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR2C1Y"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 120px;">
                    <span>第2行第2列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR2C2X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR2C2Y"
                        onkeyup="CheckInt(this);" />
                </td> 
            </tr>

             <tr>
                <td align="right" style="width: 180px;">
                    <span>首页第3行第1列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR3C1X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR3C1Y"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 120px;">
                    <span>第3行第2列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR3C2X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR3C2Y"
                        onkeyup="CheckInt(this);" />
                </td> 
            </tr>

             <tr>
                <td align="right" style="width: 180px;">
                    <span>首页第4行第1列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR4C1X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR4C1Y"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 120px;">
                    <span>第4行第2列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR4C2X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR4C2Y"
                        onkeyup="CheckInt(this);" />
                </td> 
            </tr>

             <tr>
                <td align="right" style="width: 180px;">
                    <span>首页第5行第1列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR5C1X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR5C1Y"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 120px;">
                    <span>第5行第2列X坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR5C2X"
                        onkeyup="CheckInt(this);" />
                </td>
                <td align="right" style="width: 50px;">
                    <span>Y坐标</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="HomeR5C2Y"
                        onkeyup="CheckInt(this);" />
                </td> 
            </tr>
        </table>


          <table class="tabEdit"  style="margin:5px 0px;">
        
            <tr>
                <td align="right" style="width: 180px;">
                    <span>存取记录打印字体大小</span>
                </td>
                <td style="width: 50px;">
                    <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="FontSize"
                        onkeyup="CheckInt(this);" />
                </td>
             
            </tr>
              <tr>
                  <td align="right" style="width: 180px;">
                      <span>存取记录第1行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR1Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第2行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR2Y"
                          onkeyup="CheckInt(this);" />
                  </td>
              
                  <td align="right" style="width: 180px;">
                      <span>存取记录第3行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR3Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第4行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR4Y"
                          onkeyup="CheckInt(this);" />
                  </td>
              </tr>

                <tr>
                  <td align="right" style="width: 180px;">
                      <span>存取记录第5行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR5Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第6行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR6Y"
                          onkeyup="CheckInt(this);" />
                  </td>
             
                  <td align="right" style="width: 180px;">
                      <span>存取记录第7行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR7Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第8行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR8Y"
                          onkeyup="CheckInt(this);" />
                  </td>
              </tr>

                <tr>
                  <td align="right" style="width: 180px;">
                      <span>存取记录第9行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR9Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第10行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR10Y"
                          onkeyup="CheckInt(this);" />
                  </td>
              
                  <td align="right" style="width: 180px;">
                      <span>存取记录第11行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR11Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第12行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR12Y"
                          onkeyup="CheckInt(this);" />
                  </td>
              </tr>

                <tr>
                  <td align="right" style="width: 180px;">
                      <span>存取记录第13行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR13Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第14行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR14Y"
                          onkeyup="CheckInt(this);" />
                  </td>
             
                  <td align="right" style="width: 180px;">
                      <span>存取记录第15行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR15Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第16行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR16Y"
                          onkeyup="CheckInt(this);" />
                  </td>
              </tr>

                <tr>
                  <td align="right" style="width: 180px;">
                      <span>存取记录第17行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR17Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第18行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR18Y"
                          onkeyup="CheckInt(this);" />
                  </td>
              
                  <td align="right" style="width: 180px;">
                      <span>存取记录第19行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR19Y"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                      <span>存取记录第20行Y坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordR20Y"
                          onkeyup="CheckInt(this);" />
                  </td>
              </tr>

              <tr><td></td></tr>
                 <tr >
                  <td align="right" style="width: 180px;">
                      <span>存取记录第1列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC1X"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                     <span>存取记录第2列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC2X"
                          onkeyup="CheckInt(this);" />
                  </td>
              
                  <td align="right" style="width: 180px;">
                    <span>存取记录第3列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC3X"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                     <span>存取记录第4列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC4X"
                          onkeyup="CheckInt(this);" />
                  </td>
              </tr>

               <tr >
                  <td align="right" style="width: 180px;">
                      <span>存取记录第5列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC5X"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                     <span>存取记录第6列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC6X"
                          onkeyup="CheckInt(this);" />
                  </td>
              
                  <td align="right" style="width: 180px;">
                    <span>存取记录第7列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC7X"
                          onkeyup="CheckInt(this);" />
                  </td>
                  <td align="right" style="width: 170px;">
                     <span>存取记录第8列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC8X"
                          onkeyup="CheckInt(this);" />
                  </td>
              </tr>

               <tr >
                  <td align="right" style="width: 180px;">
                      <span>存取记录第9列X坐标</span>
                  </td>
                  <td style="width: 50px;">
                      <input type="text" style="width: 40px; font-size: 16px; font-weight: bold;" name="RecordC9X"
                          onkeyup="CheckInt(this);" />
                  </td>
                 
              </tr>
            </table>
            <input type="button" id="btnUpdate" value="修改" style="margin-left:200px;" onclick="WBUpdate()" />
        </div>


    </form>
    <%--定义编号--%>
    <input type="hidden" id="WBID" value="" />
    <%--定义背景色的隐藏域--%>
    <input type="hidden" id="colorName" value="" />
</body>
</html>