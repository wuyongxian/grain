<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepositorQuery.aspx.cs" Inherits="Web.Admin.Query.DepositorQuery" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
    <script src="../../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../../../Scripts/WebInner.js" type="text/javascript"></script>
    <script src="../../../Scripts/Common.js" type="text/javascript"></script>
    <link href="../../../Styles/Common.css" rel="stylesheet" type="text/css" />
    
    <style type="text/css">
        .tabinfo tr td {
        height:20px;
        }
         .tabinfo tr td input,select{
        height:20px;
        }

        .tabinfo #btnOutPut {
        width:50px;
        }

         .tabinfo #ImageButton1 {
        width:30px;
        height:30px;
        }

        
    </style>

    <script src="../../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
      <script type="text/javascript">
          /*--------窗体启动设置和基本设置--------*/
          /*--loadFuntion--*/
          var ISHQ = true;
          $(function () {
              GetWBName();
          });


          function GetWBName() {

              $.ajax({
                  url: '/Ashx/wbinfo.ashx?type=GetWBNameForCommuneQuery',
                  type: 'post',
                  data: '',
                  dataType: 'text',
                  success: function (r) {
                      if (r == '') {
                          $('#QWBID').removeAttr("readonly");
                      }
                      else if (r == 'Error') {
                         showMsg('加载网点信息失败 ！');
                      }
                      else {
                          $('#QWBID').attr('readonly', 'readonly');
                          $('#QWBID').val(r);
                          ISHQ = false;
                      }
                  }, error: function (r) {
                     showMsg('加载网点信息失败 ！');
                  }
              });

          }

          function QClick() {
              $('#QAccountNumber').val('');
              if (ISHQ) {
                  $('#QWBID').val('');
              }
              $('#QstrName').val('');
              $('#QIDCard').val('');
              $('#QFieldCopies').val('');
              $('#QPhoneNO').val('');
              $('#Qdt_Commune1').val('');
              $('#Qdt_Commune2').val('');
              $('#XianID').get(0).selectedIndex = 0;
              $('#XiangID').get(0).selectedIndex = 0;
              $('#CunID').get(0).selectedIndex = 0;
              $('#ZuID').get(0).selectedIndex = 0;
          }

          function ShowApplication(src) {
              $('#divCommunePic').hide("normal");
              if (src != '') {
                  if ($('#divApplication').is(':hidden')) {
                      $('#imgApplication').attr('src', src);
                      $('#divApplication').show("normal");
                  } else {
                      if ($('#imgApplication').attr('src') == src) {
                          $('#divApplication').hide("normal");
                      } else {
                          $('#imgApplication').attr('src', src);
                      }
                  }
              }
          }

          function ShowCommunePic(src) {
              $('#divApplication').hide("normal");
              if (src != '') {
                  if ($('#divCommunePic').is(':hidden')) {
                      $('#imgCommunePic').attr('src', src);
                      $('#divCommunePic').show("normal");
                  } else {
                      if ($('#imgCommunePic').attr('src') == src) {
                          $('#divCommunePic').hide("normal");
                      } else {
                          $('#imgCommunePic').attr('src', src);
                      }
                  }
              }

          }



          function FunClose() {
              $('#divApplication').hide("normal");
              $('#divCommunePic').hide("normal");
          }
      </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageHead">
            <b>储户信息查询</b>
            <span id="spanHelp" style="cursor: pointer">帮助</span>
        </div>
        <div id="divHelp" class="pageHelp">
            <span>提示1：各种查询可以独使用，也可以联合使用，但必须保证至少有一项查询条件。</span><br />
            <span>提示2：每项查询均为模糊查询条件，为保证查找信息的正确性，请输入完整的查询信息。</span><br />

        </div>

        <div id="Query" style="margin:5px 0px;">
            <table class="tabinfo" style="width: 950px; background-color: #efefef">
                <tr>
                    <td align="right" style="width: 90px;"><span>储户账号:</span></td>
                    <td align="left" style="width: 110px;">
                        <input type="text" id="QAccountNumber" style="font-size: 14px; width: 100px; font-weight: bolder;" runat="server" /></td>
                    <td align="right" style="width: 90px;"><span>姓名:</span></td>
                    <td align="left" style="width: 100px;">
                        <input type="text" id="QstrName" style="font-size: 14px; width: 90px; font-weight: bolder;" runat="server" /></td>

                    <td align="right" style="width: 60px;"><span>电话:</span></td>
                    <td align="left" style="width: 100px;">
                        <input type="text" id="QPhoneNO" style="font-size: 14px; width: 90px; font-weight: bolder;" runat="server" /></td>
                    <td align="right" style="width: 90px;"><span>身份证号:</span></td>
                    <td align="left" style="width: 120px;">
                        <input type="text" id="QIDCard" style="font-size: 14px; width: 120px; font-weight: bolder;" runat="server" /></td>
                    <td rowspan="3" style="width: 50px;">
                        <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/search_red.png"
                            runat="server" OnClick="ImageButton1_Click" />
                    </td>
                    <td>
                        <input type="button" value="清空" style="width: 50px;" onclick="QClick()" /></td>
                </tr>
                <tr>
                    <td align="right" style="width: 80px;"><span>网点:</span></td>
                    <td align="left" style="width: 110px;">
                        <input type="text" id="QWBID" style="font-size: 14px; width: 100px; font-weight: bolder;" runat="server" /></td>
                    <td align="right" style="width: 80px;"><span>入社日期:</span></td>
                    <td align="left" style="width: 100px;">
                        <input type="text" id="Qdt_Commune1" readonly="readonly" onclick="WdatePicker()" style="font-size: 14px; width: 90px; font-weight: bolder;" runat="server" /></td>

                    <td align="right" style="width: 60px;"><span>至:</span></td>
                    <td align="left" style="width: 100px;">
                        <input type="text" id="Qdt_Commune2" readonly="readonly" onclick="WdatePicker()" style="font-size: 14px; width: 90px; font-weight: bolder;" runat="server" /></td>
                    <td align="right" style="width: 80px;"><span>状态:</span></td>
                    <td align="left" style="width: 120px;">
                        <select id="QState" runat="server" style="width: 120px;">
                            <option value="1">启用</option>
                            <option value="0">禁用</option>
                        </select>
                    </td>
                    <td rowspan="2">
                        <asp:Button ID="btnOutPut"  runat="server" class="outputExcel" Text="Excel"
                            OnClick="btnOutPut_Click" />
                    </td>
                </tr>
                <tr>
                    <td align="right" style="width: 80px;"><span>地址:  县</span></td>
                    <td align="left" style="width: 110px;">
                        <asp:DropDownList ID="XianID" Style="width: 90px;" runat="server"
                            AutoPostBack="True" OnSelectedIndexChanged="XianID_SelectedIndexChanged">
                        </asp:DropDownList></td>
                    <td align="right" style="width: 80px;"><span>乡</span></td>
                    <td align="left" style="width: 100px;">
                        <asp:DropDownList ID="XiangID" Style="width: 90px;" runat="server"
                            AutoPostBack="True" OnSelectedIndexChanged="XiangID_SelectedIndexChanged">
                        </asp:DropDownList></td>

                    <td align="right" style="width: 60px;"><span>村</span></td>
                    <td align="left" style="width: 100px;">
                        <asp:DropDownList ID="CunID" Style="width: 90px;" runat="server"
                            AutoPostBack="True" OnSelectedIndexChanged="CunID_SelectedIndexChanged">
                        </asp:DropDownList></td>
                    <td></td>
                    <td></td>
                </tr>
            </table>
        </div>


        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="tabData">
                    <tr class="tr_head">
                        <th style="width: 100px; height: 25px; text-align: center;">网点
                        </th>
                        <th style="width: 100px; text-align: center;">储户账号
                        </th>
                        <th style="width: 80px; text-align: center;">姓名
                        </th>
                        <th style="width: 150px; text-align: center;">地址
                        </th>
                        <th style="width: 150px; text-align: center;">身份证号
                        </th>
                        <th style="width: 100px; text-align: center;">手机号
                        </th>

                        <th style="width: 100px; text-align: center;">开户时间
                        </th>
                         <th style="width: 80px; text-align: center;">结存数
                        </th>
                        <th style="width: 80px; text-align: center;">状态
                        </th>
                        <th style="width: 60px; text-align: center;">修改
                        </th>

                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr 
                    onmouseover="change_colorOver(this)" onmouseout="change_colorOut(this)">

                    <td style="height: 25px; text-align: center;">
                        <%#Eval("WBID")%>
                    </td>
                    <td>
                        <a href="/User/Query/DepositorInfo.aspx?Account=<%#Eval("AccountNumber") %>"><%#Eval("AccountNumber") %></a>
                        <%-- <a href="/User/Commune/OperateLog.aspx?AccountNumber=<%#Eval("AccountNumber")%>" ><%#Eval("AccountNumber")%></a> --%>
        
                    </td>
                    <td>
                        <%#Eval("strName")%>
                    </td>
                    <td>
                        <%#Eval("strAddress")%>
                    </td>
                    <td>
                        <%#(Eval("IDCard"))%>
                    </td>
                    <td>
                        <%#Eval("PhoneNO")%>
                    </td>
                    <td>
                        <%#Eval("dt_Add")%>
                    </td>
                    <td>
                        <%#showStorageNumber( Eval("StorageNumber"))%>
                    </td>
                    <td>
                        <%#Eval("numState")%>
                    </td>

                    <td>
                        <a href="/Admin/Query/DepositorEdit.aspx?AccountNumber=<%#Eval("AccountNumber")%>">修改</a>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr style='background-color: #c1cdc1; font-weight: bolder;'>
                    <td>统计
                    </td>
                    <td colspan="2" style="height: 20px;">
                        <span>储户数：</span><%=IDCount %>
                 
                    </td>

                    <td></td>
                    <td></td>
                    <td></td>

                    <td></td>
                    <td></td>
                    <td></td>

                </tr>
                <!--底部模板-->
                </table>
                <!--表格结束部分-->
            </FooterTemplate>
        </asp:Repeater>
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server"
            FirstPageText="首页" LastPageText="尾页" PrevPageText="上一页" NextPageText="下一页"
            NumericButtonTextFormatString="[{0}]" PageSize="20" OnPageChanging="AspNetPager1_PageChanging">
        </webdiyer:AspNetPager>

        <div id="divApplication" style="width: 420px; height: 600px; padding: 10px; background-color: #ae8787; position: absolute; left: 100px; top: 20px; display: none">
            <img id="imgApplication" style="width: 420px; height: 600px;" onclick="FunClose();" />
        </div>
        <div id="divCommunePic" style="width: 210px; height: 300px; padding: 5px; background-color: #ae8787; position: absolute; left: 200px; top: 50px; display: none">
            <img id="imgCommunePic" style="width: 210px; height: 300px;" onclick="FunClose();" />
        </div>
        <div style="display: none;">
            <%--选择兑换的存储产品信息--%>
            <input type="text" name="txtDep_SID" value="" />
            <%--定义编号--%>
            <input type="hidden" id="WBID" value="" />
            <%--定义背景色的隐藏域--%>
            <input type="hidden" id="colorName" value="" />
        </div>
    </form>

</body>
</html>