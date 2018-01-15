<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOutEdit.aspx.cs" Inherits="Web.Settle.CheckOutEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <link href="../Styles/Common.css" rel="stylesheet" type="text/css" />
    
   
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../Lodop6.198/LodopFuncs.js" type="text/javascript"></script>

      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          $(function () {
              var ID = getQueryString('ID');
              $('input[name=CheckOutID]').val(ID);
              GetCheckOutInfo(ID);
          });

          function GetCheckOutInfo(ID) {
              $.ajax({
                  url: '/Ashx/settle.ashx?type=GetCheckOutInfo&ID=' + ID,
                  type: 'post',
                  data: '',
                  dataType: 'json',
                  success: function (r) {
                      $('#SA_Account').val(r[0].SA_AN);
                      $('input[name=Weight_Mao]').val(r[0].Weight_Mao);
                      $('input[name=Weight_Pi]').val(r[0].Weight_Pi);
                      $('input[name=ChuLiangRongLiang]').val(r[0].ChuLiangRongLiang);
                      $('input[name=RongLiangKouZhong]').val(r[0].RongLiangKouZhong);
                      $('input[name=ZaZhiHanLiang]').val(r[0].ZaZhiHanLiang);
                      $('input[name=ZazhiKouZhong]').val(r[0].ZazhiKouZhong);
                      $('input[name=ShuiFenHanLiang]').val(r[0].ShuiFenHanLiang);
                      $('input[name=ShuiFenKouZhong]').val(r[0].ShuiFenKouZhong);
                      $('input[name=QiTaKouZhong]').val(r[0].QiTaKouZhong);
                      $('input[name=Weight_Reality]').val(r[0].Weight_Reality);

                  }, error: function (r) {
                     showMsg('加载原始出库数据出现错误 ！');
                  }
              });
          }

       

          //出库操作
          function FunUpdate() {
             
              if (!frmCheck()) {
                  return false;
              }
              var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
              showConfirm(msg, function (obj) {
                  if (obj == 'yes') {
                      
                      $.ajax({
                          url: '/Ashx/settle.ashx?type=UpdateSA_CheckOut',
                          type: 'post',
                          data: $('#form1').serialize(),
                          dataType: 'text',
                          success: function (r) {
                              if (r == "0") {
                                  showMsg('修改数据失败 ！');
                              } else {
                                  showMsg('修改数据成功 ！');

                              }
                              $('#btnCheckOut').attr('disabled', 'disabled');

                          }, error: function (r) {
                              showMsg('修改数据失败 ！');
                          }
                      });
                  } else {
                      //console.log('你点击了取消！');
                  }

              });
          }





          function frmCheck() {


              if (!CheckNumDecimal($('input[name=Weight_Mao]').val(), '毛重', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=Weight_Pi]').val(), '皮重', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=ShuiFenHanLiang]').val(), '水分含量', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=ShuiFenKouZhong]').val(), '水分扣重', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=ChuLiangRongLiang]').val(), '储粮容量', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=RongLiangKouZhong]').val(), '容重扣重', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=ZaZhiHanLiang]').val(), '杂质含量', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=ZazhiKouZhong]').val(), '杂质扣重', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=QiTaKouZhong]').val(), '其他扣重', 2)) {
                  return false;
              }
              if (!CheckNumDecimal($('input[name=Weight_Reality]').val(), '出库实重', 2)) {
                  return false;
              }

           
              return true;
          }

      

          function FunJiSuan() {
              if (!frmCheck()) {
                  return;
              }
              var Weight_Mao = $('input[name=Weight_Mao]').val();
              var Weight_Pi = $('input[name=Weight_Pi]').val();

              var Weight_Total = parseFloat(Weight_Mao) - parseFloat(Weight_Pi);
              $('input[name=Weight_Total]').val(Weight_Total);

              var ShuiFenHanLiang = $('input[name=ShuiFenHanLiang]').val();
              var ShuiFenKouZhong = $('input[name=ShuiFenKouZhong]').val();
              ShuiFenKouZhong = accMul(parseFloat(Weight_Total), parseFloat(ShuiFenHanLiang));
              ShuiFenKouZhong = accMul(parseFloat(ShuiFenKouZhong), 0.01);


              var ChuLiangRongLiang = $('input[name=ChuLiangRongLiang]').val();
              var RongLiangKouZhong = $('input[name=RongLiangKouZhong]').val();
              RongLiangKouZhong = accMul(parseFloat(Weight_Total), parseFloat(ChuLiangRongLiang));
              RongLiangKouZhong = accMul(parseFloat(RongLiangKouZhong), 0.01);


              var ZaZhiHanLiang = $('input[name=ZaZhiHanLiang]').val();
              var ZazhiKouZhong = $('input[name=ZazhiKouZhong]').val();
              ZazhiKouZhong = accMul(parseFloat(Weight_Total), parseFloat(ZaZhiHanLiang));
              ZazhiKouZhong = accMul(parseFloat(ZazhiKouZhong), 0.01);


              var QiTaKouZhong = $('input[name=QiTaKouZhong]').val();

              var Weight_Reality = parseFloat(Weight_Total) - parseFloat(ShuiFenKouZhong) - parseFloat(RongLiangKouZhong) - parseFloat(ZaZhiHanLiang) - parseFloat(QiTaKouZhong);
              Weight_Reality = changeTwoDecimal_f(Weight_Reality);
              $('input[name=Weight_Reality]').val(Weight_Reality);

              ShuiFenKouZhong = changeTwoDecimal_f(ShuiFenKouZhong);
              $('input[name=ShuiFenKouZhong]').val(ShuiFenKouZhong);

              RongLiangKouZhong = changeTwoDecimal_f(RongLiangKouZhong);
              $('input[name=RongLiangKouZhong]').val(RongLiangKouZhong);

              ZazhiKouZhong = changeTwoDecimal_f(ZazhiKouZhong);
              $('input[name=ZazhiKouZhong]').val(ZazhiKouZhong);

              $('#btnCheckOut').removeAttr('disabled');

          }



      </script>
</head>
<body>
<div id="divPrint" style="display:none">

    </div>
     <div id="divPrintPaper" style="display:none">

    </div>

    <form id="form1" runat="server">
    <div class="pageHead">
       <b>原粮出库数量修改</b>
    </div>
    
<div id="storageQuery">

</div>
    <div style="margin: 20px 0px;">
      
      <form id="form1">
        <div id="divGuoBang">
        <p style="color:Blue; font-weight:bold;font-size:16px;">过磅数据</p>
        <table><tr><td align="right" style="width:100px"><span style="height:25px;">网点账号:</span></td>
        <td style="width:130px;"><input type="text" id="SA_Account" readonly="readonly"  style="width:120px; font-size:16px; font-weight:bold; background-color:#ddd;" /></td>
       
        </tr></table>
     
         <table style="height:25px;">
        <tr>
        <td align="right" style="width:50px;"><span>毛重:</span></td>
        <td style="width:120px;"> <input type="text" name="Weight_Mao" value="0" style="width:80px;" /><span>公斤</span></td>
        <td align="right" style="width:50px;"><span>皮重:</span></td>
        <td style="width:120px;"> <input type="text" name="Weight_Pi" value="0" style="width:80px;" /><span>公斤</span></td>
       </tr>
        </table>

        </div>


       <div id="divZhiJian">
        <p style="color:Blue; font-weight:bold; font-size:16px;">填写质检数据</p>
       <table class="tabEdit" style="margin:10px 0px ">

        <tr>
       <td align="right" style="height:25px; width:100px;"><span>水分含量</span></td>
       <td style="width:200px"><input type="text" name="ShuiFenHanLiang" value="0" style="width:100px;" />%</td>
       <td align="right" style="width:100px"><span>水分扣重</span></td>
       <td style="width:200px;"><input type="text" name="ShuiFenKouZhong" value="0" readonly="readonly" style="width:100px; background-color:#eee" /><span>公斤</span></td>
       </tr>

        <tr>
       <td align="right" style="height:25px; width:100px;"><span>储粮容量</span></td>
       <td style="width:200px"><input type="text" name="ChuLiangRongLiang" value="0" style="width:100px;" />%</td>
       <td align="right" style="width:100px"><span>容重扣重</span></td>
       <td style="width:200px;"><input type="text" name="RongLiangKouZhong" value="0" readonly="readonly" style="width:100px; background-color:#eee" /><span>公斤</span></td>
       </tr>

        <tr>
       <td align="right" style="height:25px; width:100px;"><span>杂质含量</span></td>
       <td style="width:200px"><input type="text" name="ZaZhiHanLiang" value="0" style="width:100px;" />%</td>
       <td align="right" style="width:100px"><span>杂质扣重</span></td>
       <td style="width:200px;"><input type="text" name="ZazhiKouZhong" value="0" readonly="readonly" style="width:100px; background-color:#eee" /><span>公斤</span></td>
       </tr>

        <tr>
       <td align="right" style="height:25px; width:100px;"><span>其他扣重</span></td>
       <td style="width:200px"><input type="text" name="QiTaKouZhong" value="0" style="width:100px;" /><span>公斤</span></td>
    
       </tr>

        <tr>
       <td align="right" style="height:25px; width:100px;"><span>计算实重</span></td>
       <td style="width:200px"><input type="button" id="btnJiSuan" onclick="FunJiSuan();" value="计算实重"  style="width:80px;" /></td>
       <td align="right" style="width:100px"><span>出库实重</span></td>
       <td style="width:200px;"><input type="text" name="Weight_Reality" value="0" readonly="readonly" style="width:100px; background-color:#eee" /><span>公斤</span></td>
       </tr>

       

       </table>
      
        </div>
        </form>

        <div style=" padding-left:100px;">
        <input type="button" value="修改数据" id="btnCheckOut" disabled="disabled"  onclick="FunUpdate();" style="width:80px; font-size:16px; font-weight:bold" />&nbsp;&nbsp;
        </div>
    
    <div  style="display:none;">
    <input type="text" name="CheckOutID" />

    <input type="text" name="Weight_Total" />
    </div>
    </form>
   

</body>
</html>