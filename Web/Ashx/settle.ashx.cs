using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Web.SessionState;


namespace Web.Ashx
{
    /// <summary>
    /// settle 的摘要说明
    /// </summary>
    public class settle : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            common.IsLogin();
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {

                    case "GetAgentFeeDate": GetAgentFeeDate(context); break;
                    case "GetCheckOut_LogByCheckOut_ID": GetCheckOut_LogByCheckOut_ID(context); break;//查询要打印的小票信息
                    case "PrintSA_CheckOutLog": PrintSA_CheckOutLog(context); break;
                    case "GetNewBusinessNO": GetNewBusinessNO(context); break;//加载新的业务编号 
                    case "PrintSAAccount": PrintSAAccount(context); break;//网点账户的存折打印
                    case "PrintExchange": PrintExchange(context); break;//网点兑换结算凭据
                    case "PrintAgentFee": PrintAgentFee(context); break;//网点代理费结算凭据
                    case "PrintSell": PrintSell(context); break;//网点存转销结算凭据
                    case "PrintShopping": PrintShopping(context); break;//网点产品换购结算凭据


                    case "PrintCheckOut": PrintCheckOut(context); break;

                    case "GetCheckOutInfo": GetCheckOutInfo(context); break;
                    case "AddSA_CheckOut": AddSA_CheckOut(context); break;
                    case "UpdateSA_CheckOut": UpdateSA_CheckOut(context); break;

                    case "GetStorageVarietyInfo": GetStorageVarietyInfo(context); break;//获取商品存量信息

                        

                    case "GetUserByWBName": GetUserByWBName(context); break;

                    case "GetAgentFeeSingle": GetAgentFeeSingle(context); break;//获取单笔代理费信息
                    case "ISHaveAgentFee": ISHaveAgentFee(context); break;//添加业务之前判断是否已经存在此笔业务
                    case "AddSA_AgentFee": AddSA_AgentFee(context); break;//添加单笔业务结算
                    case "AddSA_AgentFeeAll": AddSA_AgentFeeAll(context); break;//将页面中的代理费全部结算


                    case "GetExchangeSingle": GetExchangeSingle(context); break;
                    case "ISHaveExchange": ISHaveExchange(context); break;//添加业务之前判断是否已经存在此笔业务
                    case "AddSA_Exchange": AddSA_Exchange(context); break;//添加单笔业务结算
                    case "AddSA_ExchangeAll": AddSA_ExchangeAll(context); break;//将页面中的代理费全部结算


                    case "GetSellSingle": GetSellSingle(context); break;
                    case "ISHaveSell": ISHaveSell(context); break;//添加业务之前判断是否已经存在此笔业务
                    case "AddSA_Sell": AddSA_Sell(context); break;//添加单笔业务结算
                    case "AddSA_SellAll": AddSA_SellAll(context); break;//将页面中的代理费全部结算

                    case "GetShoppingSingle": GetShoppingSingle(context); break;
                    case "ISHaveShopping": ISHaveShopping(context); break;//添加业务之前判断是否已经存在此笔业务
                    case "AddSA_Shopping": AddSA_Shopping(context); break;//添加单笔业务结算
                    case "AddSA_ShoppingAll": AddSA_ShoppingAll(context); break;//将页面中的代理费全部结算
                }
            }

        }

        void GetAgentFeeDate(HttpContext context)
        {
            string WBName = context.Request.Form["WBName"].ToString();
            DataTable dtWB = SQLHelper.ExecuteDataTable(" SELECT  top 1  numDay,draw_exchange,draw_sell,draw_shopping FROM dbo.WB WHERE strName ='" + WBName + "'");
            if (dtWB == null || dtWB.Rows.Count == 0)
            {
                var res = new { state = "error" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else {
               
                int numDay = Convert.ToInt32(dtWB.Rows[0]["numDay"]);
                int draw_exchange = Convert.ToInt32(dtWB.Rows[0]["draw_exchange"]);
                int draw_sell = Convert.ToInt32(dtWB.Rows[0]["draw_sell"]);
                int draw_shopping = Convert.ToInt32(dtWB.Rows[0]["draw_shopping"]);
                string date_begin = DateTime.Now.AddDays(-numDay - 30).ToString();
                string date_end = DateTime.Now.AddDays(-numDay).ToString();
                var res = new { state = "success",numDay=numDay,date_begin=date_begin,date_end=date_end,draw_exchange=draw_exchange,draw_sell=draw_sell,draw_shopping=draw_shopping };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void GetNewBusinessNO(HttpContext context)
        {

            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            string BusinessNO = "001";
            //查询当前社员的业务编号
            StringBuilder strSqlNO = new StringBuilder();
            strSqlNO.Append("  SELECT TOP 1 BusinessNO ");
            strSqlNO.Append("  FROM dbo. SA_CheckOutLog  ");
            strSqlNO.Append("  WHERE AccountNumber='" + AccountNumber + "'");
            strSqlNO.Append("  ORDER BY  ID DESC");
            object obj = SQLHelper.ExecuteScalar(strSqlNO.ToString());
            if (obj != null && obj.ToString() != "")
            {
                int BNO = Convert.ToInt32(obj) + 1;
                BusinessNO = Fun.ConvertIntToString(BNO, 4);
            }

            context.Response.Write(BusinessNO);
        }


        /// <summary>
        /// 获取储户的打印信息
        /// </summary>
        /// <param name="context"></param>
        void PrintSAAccount(HttpContext context)
        {
            //获取需要打印的信息
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            StringBuilder strSqlCommume = new StringBuilder();
            strSqlCommume.Append("SELECT A.AccountNumber,A.strName,A.strAddress,A.IDCard,B.strName AS WBID,'密码' AS OperateWay, PhoneNO, CONVERT(NVARCHAR(100),A.dt_Add,23) AS dt_Add");
            strSqlCommume.Append(" FROM dbo.SA_Account A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlCommume.Append(" WHERE A.AccountNumber='" + AccountNumber + "'");


            DataTable dtCommune = SQLHelper.ExecuteDataTable(strSqlCommume.ToString());
            if (dtCommune == null || dtCommune.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }

            string strName = dtCommune.Rows[0]["strName"].ToString();
            string strAddress = dtCommune.Rows[0]["strAddress"].ToString();
            string IDCard = dtCommune.Rows[0]["IDCard"].ToString();
            string WBID = dtCommune.Rows[0]["WBID"].ToString();
            string PhoneNO = dtCommune.Rows[0]["PhoneNO"].ToString();
            string dt_Commune = dtCommune.Rows[0]["dt_Add"].ToString();
            string OperateWay = dtCommune.Rows[0]["OperateWay"].ToString();


            string strWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += "  where 1=1 and WBID=" + strWBID;

            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null && dt.Rows.Count != 0)
            {

                string HomeR1C1X = dt.Rows[0]["HomeR1C1X"].ToString();
                string HomeR1C1Y = dt.Rows[0]["HomeR1C1Y"].ToString();
                string HomeR1C2X = dt.Rows[0]["HomeR1C2X"].ToString();
                string HomeR1C2Y = dt.Rows[0]["HomeR1C2Y"].ToString();

                string HomeR2C1X = dt.Rows[0]["HomeR2C1X"].ToString();
                string HomeR2C1Y = dt.Rows[0]["HomeR2C1Y"].ToString();
                string HomeR2C2X = dt.Rows[0]["HomeR2C2X"].ToString();
                string HomeR2C2Y = dt.Rows[0]["HomeR2C2Y"].ToString();

                string HomeR3C1X = dt.Rows[0]["HomeR3C1X"].ToString();
                string HomeR3C1Y = dt.Rows[0]["HomeR3C1Y"].ToString();
                string HomeR3C2X = dt.Rows[0]["HomeR3C2X"].ToString();
                string HomeR3C2Y = dt.Rows[0]["HomeR3C2Y"].ToString();

                string HomeR4C1X = dt.Rows[0]["HomeR4C1X"].ToString();
                string HomeR4C1Y = dt.Rows[0]["HomeR4C1Y"].ToString();
                string HomeR4C2X = dt.Rows[0]["HomeR4C2X"].ToString();
                string HomeR4C2Y = dt.Rows[0]["HomeR4C2Y"].ToString();

                string HomeR5C1X = dt.Rows[0]["HomeR5C1X"].ToString();
                string HomeR5C1Y = dt.Rows[0]["HomeR5C1Y"].ToString();
                string HomeR5C2X = dt.Rows[0]["HomeR5C2X"].ToString();
                string HomeR5C2Y = dt.Rows[0]["HomeR5C2Y"].ToString();

                StringBuilder strReturn = new StringBuilder();
                strReturn.Append("  <table style='height:" + HomeR1C1Y + "px'><tr><td></td> </tr></table>");
                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR2C1Y) - Convert.ToInt32(HomeR1C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR1C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:20px; font-weight:bolder; width:" + (Convert.ToInt32(HomeR1C2X) - Convert.ToInt32(HomeR1C1X)).ToString() + "px;'>" + AccountNumber + "</td>");
                strReturn.Append("   <td style='font-size:20px; font-weight:bolder;'>" + strName + "</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR3C1Y) - Convert.ToInt32(HomeR2C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR2C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR2C2X) - Convert.ToInt32(HomeR2C1X)).ToString() + "px;'>" + strAddress + "</td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;'>" + IDCard + "</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR4C1Y) - Convert.ToInt32(HomeR3C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR3C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR3C2X) - Convert.ToInt32(HomeR3C1X)).ToString() + "px;'>" + WBID + "</td>");
                strReturn.Append("  <td style='font-size:14px; font-weight:bold;'>" + OperateWay + "</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR5C1Y) - Convert.ToInt32(HomeR4C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR4C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR4C2X) - Convert.ToInt32(HomeR4C1X)).ToString() + "px;'>" + PhoneNO + "</td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;' >" + dt_Commune + "</td>");

                strReturn.Append("   </tr></table>");


                context.Response.Write(strReturn.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 打印对话凭据
        /// </summary>
        /// <param name="context"></param>
        void PrintExchange(HttpContext context)
        {
            //获取需要打印的信息
            string GoodExchangeID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SA_AN,GoodExchangeID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,CONVERT(NVARCHAR(100),StorageDate,23) AS StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,CONVERT(NVARCHAR(100),dt_Trade,23) AS dt_Trade ");
            strSql.Append(" FROM [SA_Exchange] ");
            strSql.Append(" where GoodExchangeID=@GoodExchangeID ");
            SqlParameter[] parameters = {
					new SqlParameter("@GoodExchangeID", SqlDbType.Int,4)};
            parameters[0].Value = GoodExchangeID;

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSql.ToString(),parameters);
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();
            string serialNumber = dtLog.Rows[0]["serialNumber"].ToString();
            string WBName = dtLog.Rows[0]["WBName"].ToString();
            string Dep_AN = dtLog.Rows[0]["Dep_AN"].ToString();
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();
            string StorageDay = dtLog.Rows[0]["StorageDay"].ToString();
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string GoodName = dtLog.Rows[0]["GoodName"].ToString();
            string UnitName = dtLog.Rows[0]["UnitName"].ToString();
            string numWeight = dtLog.Rows[0]["numWeight"].ToString();
            string numPrice = dtLog.Rows[0]["numPrice"].ToString();
            string Money_Total = dtLog.Rows[0]["Money_Total"].ToString();
            string Money_Reality = dtLog.Rows[0]["Money_Reality"].ToString();
            string Money_Surplus = dtLog.Rows[0]["Money_Surplus"].ToString();
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();
            string StorageDate = dtLog.Rows[0]["StorageDate"].ToString();//兑换时间
            string Accountant_Name = dtLog.Rows[0]["Accountant_Name"].ToString();
           
          

            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户兑换凭证</span></td> </tr>");
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + serialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");


            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 200px;'>  <span >网点名称：" + WBName + "</span> </td>");
            strReturn.Append("    <td style='width: 240px;'>  <span >储户姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 200px;'>  <span >兑换日期：" + StorageDate + "</span> </td>");
            strReturn.Append("   </tr> </table>");


            //表格内容
            strReturn.Append("    <table class='tabPrint' style='padding: 5px 0px; font-size: 14px;'>");
            //添加表格样式
            strReturn.Append("    <style>");
            strReturn.Append("    table.tabPrint{ border-collapse: collapse; border: 1px solid #666;  font-size: 14px;}");
            strReturn.Append("     table.tabPrint thead td, table.set_border th{ font-weight: bold; background-color: White;}");
            strReturn.Append("    table.tabPrint tr:nth-child(even){ background-color: #666;}");
            strReturn.Append("     table.tabPrint td, table.border th{  border: 1px solid #666;}");
            strReturn.Append("   </style>");


            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 100px;'> <span>业务名称</span></td>");
            strReturn.Append("    <td style='width: 100px;'> <span>储户账号</span></td>");
            strReturn.Append("    <td style='width: 100px;'> <span>商品名称</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>重量</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>单价</span></td>");
            strReturn.Append("    <td style='width: 90px;'> <span>应付款</span></td>");
            strReturn.Append("    <td style='width: 90px;'> <span>实付款</span></td>");
            strReturn.Append("  </tr>");
          
            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>"+BusinessName+"</span></td>");
            strReturn.Append("    <td> <span>" + Dep_AN + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodName + "</span></td>");
            strReturn.Append("    <td> <span>" + numWeight + "</span></td>");
            strReturn.Append("    <td> <span>" + numPrice + "</span></td>");
            strReturn.Append("    <td> <span>￥" +Money_Total  + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_Reality + "</span></td>");
            strReturn.Append("  </tr>");
            string strMoney_Reality = Fun.ChangeToRMB(Money_Reality);
            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td> <span>大写金额</span></td>");
            strReturn.Append("    <td colspan='4'> <span>" + strMoney_Reality + "</span></td>");
            strReturn.Append("    <td colspan='2'><span>余款合计:￥</span> <span>" + Money_Surplus + "</span></td>");

            strReturn.Append("  </tr>");

            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
             strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width:160px;'> <span>付款日期："+dt_Trade+"</span></td>" );
            strReturn.Append("   <td style='width:160px;'> <span>计量单位：元、" + UnitName + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>总部会计：" + Accountant_Name+ "</span></td>");
            strReturn.Append("   <td align='right' style='width:80px;'> <span>分行签名：</span></td><td> <div style='width:80px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("  </tr>");

       
            strReturn.Append("  </table>");


            context.Response.Write(strReturn.ToString());
        }


        /// <summary>
        /// 打印存转销凭据
        /// </summary>
        /// <param name="context"></param>
        void PrintAgentFee(HttpContext context)
        {
            //获取需要打印的信息
            string Dep_SID = context.Request.QueryString["Dep_SID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT ID,SA_AN,Dep_StorageInfoID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,CONVERT(NVARCHAR(100),StorageDate,23) AS  StorageDate,CONVERT(NVARCHAR(100),dt_Trade,23) AS  dt_Trade,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight_Storage,numWeight_ZhiQu,numWeight_Effect,AgentFee,Money_Total,Money_Reality,Money_Surplus,Accountant_Name ");
            strSql.Append(" FROM SA_AgentFee ");
            strSql.Append(" where Dep_StorageInfoID=@Dep_StorageInfoID ");
            SqlParameter[] parameters = {
					new SqlParameter("@Dep_StorageInfoID", SqlDbType.Int,4)};
            parameters[0].Value = Dep_SID;

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();
            string serialNumber = dtLog.Rows[0]["serialNumber"].ToString();
            string WBName = dtLog.Rows[0]["WBName"].ToString();
            string Dep_AN = dtLog.Rows[0]["Dep_AN"].ToString();
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();
            string StorageDay = dtLog.Rows[0]["StorageDay"].ToString();
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string GoodName = dtLog.Rows[0]["GoodName"].ToString();
            string UnitName = dtLog.Rows[0]["UnitName"].ToString();
            string numWeight_Storage = dtLog.Rows[0]["numWeight_Storage"].ToString();//存入重量
            string numWeight_ZhiQu = dtLog.Rows[0]["numWeight_ZhiQu"].ToString();//支取质量
            string numWeight_Effect = dtLog.Rows[0]["numWeight_Effect"].ToString();//有效重量
            string AgentFee = dtLog.Rows[0]["AgentFee"].ToString();//费率
            string Money_Total = dtLog.Rows[0]["Money_Total"].ToString();//应付款
            string Money_Reality = dtLog.Rows[0]["Money_Total"].ToString();//应付款
            string Money_Surplus = dtLog.Rows[0]["Money_Surplus"].ToString();//应付款
            string StorageDate = dtLog.Rows[0]["StorageDate"].ToString();//存入时间
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();//存入时间
            string Accountant_Name = dtLog.Rows[0]["Accountant_Name"].ToString();//办理人
            
            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  代理费结算凭证</span></td> </tr>");
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + serialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");


            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 180px;'>  <span >网点名称：" + WBName + "</span> </td>");
            strReturn.Append("    <td style='width: 280px;'>  <span >储户：" + Dep_Name +"("+Dep_AN+") </span> </td>");
            strReturn.Append("    <td style='width: 180px;'>  <span >实存天数：" + StorageDay + "天</span> </td>");
            strReturn.Append("   </tr> </table>");


            //表格内容
            strReturn.Append("    <table class='tabPrint' style='padding: 5px 0px; font-size: 14px;'>");
            //添加表格样式
            strReturn.Append("    <style>");
            strReturn.Append("    table.tabPrint{ border-collapse: collapse; border: 1px solid #666;  font-size: 14px;}");
            strReturn.Append("     table.tabPrint thead td, table.set_border th{ font-weight: bold; background-color: White;}");
            strReturn.Append("    table.tabPrint tr:nth-child(even){ background-color: #666;}");
            strReturn.Append("     table.tabPrint td, table.border th{  border: 1px solid #666;}");
            strReturn.Append("   </style>");


            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 80px;'> <span>业务名称</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>产品类型</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>存入重量</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>支取重量</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>有效重量</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>费率</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>应付款</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>实付款</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>" + BusinessName + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodName + "</span></td>");
            strReturn.Append("    <td> <span>" + numWeight_Storage + "</span></td>");
            strReturn.Append("    <td> <span>" + numWeight_ZhiQu + "</span></td>");
            strReturn.Append("    <td> <span>" + numWeight_Effect + "</span></td>");
            strReturn.Append("    <td> <span>" + AgentFee + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_Total + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_Reality + "</span></td>");
            strReturn.Append("  </tr>");
            string strMoney_Reality = Fun.ChangeToRMB(Money_Total);
            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td> <span>大写金额</span></td>");
            strReturn.Append("    <td colspan='5'> <span>" + strMoney_Reality + "</span></td>");
            strReturn.Append("    <td colspan='2'><span>余款合计:￥</span> <span>" + Money_Surplus + "</span></td>");

            strReturn.Append("  </tr>");


            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width:160px;'> <span>付款日期：" + dt_Trade + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>计量单位：元、" + UnitName + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>总部会计：" + Accountant_Name + "</span></td>");
            strReturn.Append("   <td align='right' style='width:80px;'> <span>分行签名：</span></td><td> <div style='width:80px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("  </tr>");


            strReturn.Append("  </table>");


            context.Response.Write(strReturn.ToString());
        }

        /// <summary>
        /// 打印存转销凭据
        /// </summary>
        /// <param name="context"></param>
        void PrintSell(HttpContext context)
        {
            //获取需要打印的信息
            string StorageSellID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SA_AN,StorageSellID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,CONVERT(NVARCHAR(100),StorageDate,23) AS  StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Storage,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,CONVERT(NVARCHAR(100),dt_Trade,23) AS  dt_Trade ");
            strSql.Append(" FROM [SA_Sell] ");
            strSql.Append(" where StorageSellID=@StorageSellID ");
            SqlParameter[] parameters = {
					new SqlParameter("@StorageSellID", SqlDbType.Int,4)};
            parameters[0].Value = StorageSellID;

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();
            string serialNumber = dtLog.Rows[0]["serialNumber"].ToString();
            string WBName = dtLog.Rows[0]["WBName"].ToString();
            string Dep_AN = dtLog.Rows[0]["Dep_AN"].ToString();
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();
            string StorageDay = dtLog.Rows[0]["StorageDay"].ToString();
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string GoodName = dtLog.Rows[0]["GoodName"].ToString();
            string UnitName = dtLog.Rows[0]["UnitName"].ToString();
            string numWeight = dtLog.Rows[0]["numWeight"].ToString();
            string numPrice = dtLog.Rows[0]["numPrice"].ToString();
            string Money_Total = dtLog.Rows[0]["Money_Total"].ToString();
            string Money_Surplus = dtLog.Rows[0]["Money_Surplus"].ToString();
            string Money_Reality = dtLog.Rows[0]["Money_Reality"].ToString();
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();
            string StorageDate = dtLog.Rows[0]["StorageDate"].ToString();
            string Accountant_Name = dtLog.Rows[0]["Accountant_Name"].ToString();

            

            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  存转销结算凭证</span></td> </tr>");
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + serialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");


            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 200px;'>  <span >网点名称：" + WBName + "</span> </td>");
            strReturn.Append("    <td style='width: 240px;'>  <span >储户姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 200px;'>  <span >存转销日期：" + StorageDate + "</span> </td>");
            strReturn.Append("   </tr> </table>");


            //表格内容
            strReturn.Append("    <table class='tabPrint' style='padding: 5px 0px; font-size: 14px;'>");
            //添加表格样式
            strReturn.Append("    <style>");
            strReturn.Append("    table.tabPrint{ border-collapse: collapse; border: 1px solid #666;  font-size: 14px;}");
            strReturn.Append("     table.tabPrint thead td, table.set_border th{ font-weight: bold; background-color: White;}");
            strReturn.Append("    table.tabPrint tr:nth-child(even){ background-color: #666;}");
            strReturn.Append("     table.tabPrint td, table.border th{  border: 1px solid #666;}");
            strReturn.Append("   </style>");


            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 100px;'> <span>业务名称</span></td>");
            strReturn.Append("    <td style='width: 100px;'> <span>储户账号</span></td>");
            strReturn.Append("    <td style='width: 100px;'> <span>产品类型</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>重量</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>单价</span></td>");
            strReturn.Append("    <td style='width: 90px;'> <span>应付款</span></td>");
            strReturn.Append("    <td style='width: 90px;'> <span>实付款</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>" + BusinessName + "</span></td>");
            strReturn.Append("    <td> <span>" + Dep_AN + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodName + "</span></td>");
            strReturn.Append("    <td> <span>" + numWeight + "</span></td>");
            strReturn.Append("    <td> <span>" + numPrice + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_Total + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_Reality + "</span></td>");
            strReturn.Append("  </tr>");
            string strMoney_Reality = Fun.ChangeToRMB(Money_Reality);
            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td> <span>大写金额</span></td>");
            strReturn.Append("    <td colspan='4'> <span>" + strMoney_Reality + "</span></td>");
            strReturn.Append("    <td colspan='2'><span>余款合计:￥</span> <span>" + Money_Surplus + "</span></td>");
           
            strReturn.Append("  </tr>");


            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width:160px;'> <span>付款日期：" + dt_Trade + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>计量单位：元、" + UnitName + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>总部会计：" + Accountant_Name + "</span></td>");
            strReturn.Append("   <td align='right' style='width:80px;'> <span>分行签名：</span></td><td> <div style='width:80px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("  </tr>");


            strReturn.Append("  </table>");


            context.Response.Write(strReturn.ToString());
        }

        /// <summary>
        /// 打印产品换购结算凭据凭据
        /// </summary>
        /// <param name="context"></param>
        void PrintShopping(HttpContext context)
        {
            //获取需要打印的信息
            string StorageShoppingID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SA_AN,StorageShoppingID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,CONVERT(NVARCHAR(100),StorageDate,23) AS  StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Storage,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,CONVERT(NVARCHAR(100),dt_Trade,23) AS  dt_Trade ");
            strSql.Append(" FROM [SA_Shopping] ");
            strSql.Append(" where StorageShoppingID=@StorageShoppingID ");
            SqlParameter[] parameters = {
					new SqlParameter("@StorageShoppingID", SqlDbType.Int,4)};
            parameters[0].Value = StorageShoppingID;

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();
            string serialNumber = dtLog.Rows[0]["serialNumber"].ToString();
            string WBName = dtLog.Rows[0]["WBName"].ToString();
            string Dep_AN = dtLog.Rows[0]["Dep_AN"].ToString();
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();
            string StorageDay = dtLog.Rows[0]["StorageDay"].ToString();
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string GoodName = dtLog.Rows[0]["GoodName"].ToString();
            string UnitName = dtLog.Rows[0]["UnitName"].ToString();
            string numWeight = dtLog.Rows[0]["numWeight"].ToString();
            string numPrice = dtLog.Rows[0]["numPrice"].ToString();
            string Money_Total = dtLog.Rows[0]["Money_Total"].ToString();
            string Money_Surplus = dtLog.Rows[0]["Money_Surplus"].ToString();
            string Money_Reality = dtLog.Rows[0]["Money_Reality"].ToString();
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();
            string StorageDate = dtLog.Rows[0]["StorageDate"].ToString();
            string Accountant_Name = dtLog.Rows[0]["Accountant_Name"].ToString();



            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  产品换购结算凭证</span></td> </tr>");
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + serialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");


            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 200px;'>  <span >网点名称：" + WBName + "</span> </td>");
            strReturn.Append("    <td style='width: 240px;'>  <span >储户姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 200px;'>  <span >换购日期：" + StorageDate + "</span> </td>");
            strReturn.Append("   </tr> </table>");


            //表格内容
            strReturn.Append("    <table class='tabPrint' style='padding: 5px 0px; font-size: 14px;'>");
            //添加表格样式
            strReturn.Append("    <style>");
            strReturn.Append("    table.tabPrint{ border-collapse: collapse; border: 1px solid #666;  font-size: 14px;}");
            strReturn.Append("     table.tabPrint thead td, table.set_border th{ font-weight: bold; background-color: White;}");
            strReturn.Append("    table.tabPrint tr:nth-child(even){ background-color: #666;}");
            strReturn.Append("     table.tabPrint td, table.border th{  border: 1px solid #666;}");
            strReturn.Append("   </style>");


            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 100px;'> <span>业务名称</span></td>");
            strReturn.Append("    <td style='width: 100px;'> <span>储户账号</span></td>");
            strReturn.Append("    <td style='width: 100px;'> <span>产品类型</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>重量</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>单价</span></td>");
            strReturn.Append("    <td style='width: 90px;'> <span>应付款</span></td>");
            strReturn.Append("    <td style='width: 90px;'> <span>实付款</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>" + BusinessName + "</span></td>");
            strReturn.Append("    <td> <span>" + Dep_AN + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodName + "</span></td>");
            strReturn.Append("    <td> <span>" + numWeight + "</span></td>");
            strReturn.Append("    <td> <span>" + numPrice + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_Total + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_Reality + "</span></td>");
            strReturn.Append("  </tr>");
            string strMoney_Reality = Fun.ChangeToRMB(Money_Reality);
            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td> <span>大写金额</span></td>");
            strReturn.Append("    <td colspan='4'> <span>" + strMoney_Reality + "</span></td>");
            strReturn.Append("    <td colspan='2'><span>余款合计:￥</span> <span>" + Money_Surplus + "</span></td>");

            strReturn.Append("  </tr>");


            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width:160px;'> <span>付款日期：" + dt_Trade + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>计量单位：元、" + UnitName + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>总部会计：" + Accountant_Name + "</span></td>");
            strReturn.Append("   <td align='right' style='width:80px;'> <span>分行签名：</span></td><td> <div style='width:80px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("  </tr>");


            strReturn.Append("  </table>");


            context.Response.Write(strReturn.ToString());
        }

        /// <summary>
        /// 打印网点出库记录
        /// </summary>
        /// <param name="context"></param>
        void PrintCheckOut(HttpContext context)
        {
            //获取需要打印的信息
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SA_AN,strGUID,serialNumber,CheckOutNO,WBID,SA_Account_Name,SA_Account_WH,LicensePN,Weight_Mao,Weight_Pi,Weight_Jing,StockType_Name,strYear,Variety_Name,VarietyLevel_Name,ChuLiangRongLiang,RongLiangKouZhong,ZaZhiHanLiang,ZazhiKouZhong,ShuiFenHanLiang,ShuiFenKouZhong,QiTaKouZhong,strLevel,Weight_Total,Weight_Reality,Weigh_Name,Quanlity_Name,CONVERT(NVARCHAR(100),dt_Trade,23) AS dt_Trade ");
            strSql.Append(" FROM [SA_CheckOut] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = ID;

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();
            string serialNumber = dtLog.Rows[0]["serialNumber"].ToString();
            string CheckOutNO = dtLog.Rows[0]["CheckOutNO"].ToString();
           // string WBID = dtLog.Rows[0]["WBID"].ToString();
            string SA_Account_Name = dtLog.Rows[0]["SA_Account_Name"].ToString();
            string SA_Account_WH = dtLog.Rows[0]["SA_Account_WH"].ToString();
            string LicensePN = dtLog.Rows[0]["LicensePN"].ToString();
            string Weight_Mao = dtLog.Rows[0]["Weight_Mao"].ToString();
            string Weight_Pi = dtLog.Rows[0]["Weight_Pi"].ToString();
            string Weight_Jing = dtLog.Rows[0]["Weight_Jing"].ToString();////////
            string StockType_Name = dtLog.Rows[0]["StockType_Name"].ToString();
            string strYear = dtLog.Rows[0]["strYear"].ToString();
            string Variety_ID = dtLog.Rows[0]["Variety_Name"].ToString();
            string Variety_Name = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.StorageVariety WHERE ID="+Variety_ID).ToString();
            string ChuLiangRongLiang = dtLog.Rows[0]["ChuLiangRongLiang"].ToString();
            string RongLiangKouZhong = dtLog.Rows[0]["RongLiangKouZhong"].ToString();
            string ZaZhiHanLiang = dtLog.Rows[0]["ZaZhiHanLiang"].ToString();
            string ZazhiKouZhong = dtLog.Rows[0]["ZazhiKouZhong"].ToString();
            string ShuiFenHanLiang = dtLog.Rows[0]["ShuiFenHanLiang"].ToString();
            string ShuiFenKouZhong = dtLog.Rows[0]["ShuiFenKouZhong"].ToString();
            string QiTaKouZhong = dtLog.Rows[0]["QiTaKouZhong"].ToString();
            string strLevel = dtLog.Rows[0]["strLevel"].ToString();
            string Weight_Total = dtLog.Rows[0]["Weight_Total"].ToString();////////
            string Weight_Reality = dtLog.Rows[0]["Weight_Reality"].ToString();
            string Weigh_Name = dtLog.Rows[0]["Weigh_Name"].ToString();
            string Quanlity_Name = dtLog.Rows[0]["Quanlity_Name"].ToString();
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();


            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  出库质检过磅单</span></td> </tr>");
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + serialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");


  

            //表格内容
            strReturn.Append("    <table class='tabPrint' style='padding: 5px 0px; font-size: 14px;'>");
            //添加表格样式
            strReturn.Append("    <style>");
            strReturn.Append("    table.tabPrint{ border-collapse: collapse; border: 1px solid #666;  font-size: 14px;}");
            strReturn.Append("     table.tabPrint thead td, table.set_border th{ font-weight: bold; background-color: White;}");
            strReturn.Append("    table.tabPrint tr:nth-child(even){ background-color: #666;}");
            strReturn.Append("     table.tabPrint td, table.border th{  border: 1px solid #666;}");
            strReturn.Append("   </style>");


            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>储户姓名</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + SA_Account_Name + "</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>出仓库号</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>"+SA_Account_WH+"</span></td>");
            strReturn.Append("  </tr>");
            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>车号</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>毛重</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>皮重</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>净重</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>"+LicensePN+"</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + Weight_Mao + "</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>"+Weight_Pi+"</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + Weight_Jing + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>出库类型</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + StockType_Name + "</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>品种</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + Variety_Name + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>储量容量</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + ChuLiangRongLiang + "%</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>容重扣重</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + RongLiangKouZhong + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>杂质含量</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + ZaZhiHanLiang + "%</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>杂质扣重</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + ZazhiKouZhong + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>水分含量</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + ShuiFenHanLiang + "%</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>水分扣重</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + ShuiFenKouZhong + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>其他扣重</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + QiTaKouZhong + "</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>等级</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + strLevel+ "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td style='width: 120px;'> <span>总计:净重含杂</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + Weight_Total + "</span></td>");
            strReturn.Append("    <td style='width: 120px;'> <span>出库实重</span></td>");
            strReturn.Append("    <td style='width: 200px;'> <span>" + Weight_Reality + "</span></td>");
            strReturn.Append("  </tr>");

            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width:160px;'> <span>过磅员：" + Weigh_Name + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>质检员:" + Quanlity_Name + "</span></td>");
            strReturn.Append("   <td style='width:160px;'> <span>过磅日期:" + dt_Trade + "</span></td>");
            strReturn.Append("   <td></td>");
            strReturn.Append("  </tr>");


            strReturn.Append("  </table>");


            context.Response.Write(strReturn.ToString());
        }


        /// <summary>
        /// 获取出库信息
        /// </summary>
        /// <param name="context"></param>
        void GetCheckOutInfo(HttpContext context)
        {
            //获取需要打印的信息
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SA_AN,strGUID,serialNumber,CheckOutNO,WBID,SA_Account_Name,SA_Account_WH,LicensePN,Weight_Mao,Weight_Pi,Weight_Jing,StockType_Name,strYear,Variety_Name,VarietyLevel_Name,ChuLiangRongLiang,RongLiangKouZhong,ZaZhiHanLiang,ZazhiKouZhong,ShuiFenHanLiang,ShuiFenKouZhong,QiTaKouZhong,strLevel,Weight_Total,Weight_Reality,Weigh_Name,Quanlity_Name,CONVERT(NVARCHAR(100),dt_Trade,23) AS dt_Trade ");
            strSql.Append(" FROM [SA_CheckOut] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = ID;

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("Error");
            }
            else
            {
                context.Response.Write(JsonHelper.ToJson(dtLog));
            }
        }


        /// <summary>
        /// 添加网点出库记录
        /// </summary>
        /// <param name="context"></param>
        void AddSA_CheckOut(HttpContext context)
        {
              string SA_AN = context.Request.Form["AccountNumber"].ToString();
             
            string serialNumber = "";
            string serialNumber_A= DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + SA_AN;
            string serialNumber_B="000001";
            StringBuilder strSN = new StringBuilder();
            strSN.Append(" SELECT TOP 1 serialNumber");
            strSN.Append(" FROM dbo.SA_CheckOut");
            strSN.Append(" WHERE SA_AN='"+SA_AN+"'");
            strSN.Append(" ORDER BY ID DESC");
            object objSN = SQLHelper.ExecuteScalar(strSN.ToString());
            if (objSN != null && objSN.ToString() != "")
            {
                serialNumber_B = objSN.ToString().Substring(objSN.ToString().Length - 6);
                serialNumber_B = Fun.ConvertIntToString(Convert.ToInt32(serialNumber_B)+1, 6);
            }
            serialNumber = serialNumber_A + serialNumber_B;

            string strGUID = Fun.getGUID();
            string CheckOutNO = context.Request.Form["CheckOutNO"].ToString();
            string WBID = context.Request.Form["WB_ID"].ToString();
            string SA_Account_Name = context.Request.Form["SA_Account_Name"].ToString();
            string SA_Account_WH = context.Request.Form["SA_Account_WH"].ToString();
            string LicensePN = context.Request.Form["LicensePN"].ToString();
            string Weight_Mao = context.Request.Form["Weight_Mao"].ToString();
            string Weight_Pi = context.Request.Form["Weight_Pi"].ToString();
            string Weight_Jing = context.Request.Form["Weight_Total"].ToString();////
            string StockType_Name = context.Request.Form["strStockType"].ToString();
            string strYear = context.Request.Form["strYear"].ToString();
           string Variety_ID = context.Request.Form["Variety_ID"].ToString();//使用产品编号 
           string VarietyLevel_ID = context.Request.Form["VarietyLevel_ID"].ToString();//使用产品编号 
           string Variety_Name = context.Request.Form["Variety_Name"].ToString();//使用产品编号 
            string ChuLiangRongLiang = context.Request.Form["ChuLiangRongLiang"].ToString();
            string RongLiangKouZhong = context.Request.Form["RongLiangKouZhong"].ToString();
            string ZaZhiHanLiang = context.Request.Form["ZaZhiHanLiang"].ToString();
            string ZazhiKouZhong = context.Request.Form["ZazhiKouZhong"].ToString();
            string ShuiFenHanLiang = context.Request.Form["ShuiFenHanLiang"].ToString();
            string ShuiFenKouZhong = context.Request.Form["ShuiFenKouZhong"].ToString();
            string QiTaKouZhong = context.Request.Form["QiTaKouZhong"].ToString();
            string strLevel = context.Request.Form["strLevel"].ToString();
            string Weight_Total = context.Request.Form["Weight_Total"].ToString();////
            string Weight_Reality = context.Request.Form["Weight_Reality"].ToString();
            string Weigh_Name = context.Request.Form["strWeigh"].ToString();
            string Quanlity_Name = context.Request.Form["strQuality"].ToString();
            string dt_Trade = DateTime.Now.ToString();

            //添加商品出库记录
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SA_CheckOut] (");
            strSql.Append("SA_AN,strGUID,serialNumber,CheckOutNO,WBID,SA_Account_Name,SA_Account_WH,LicensePN,Weight_Mao,Weight_Pi,Weight_Jing,StockType_Name,strYear,Variety_Name,VarietyLevel_Name,ChuLiangRongLiang,RongLiangKouZhong,ZaZhiHanLiang,ZazhiKouZhong,ShuiFenHanLiang,ShuiFenKouZhong,QiTaKouZhong,strLevel,Weight_Total,Weight_Reality,Weigh_Name,Quanlity_Name,dt_Trade)");
            strSql.Append(" values (");
            strSql.Append("@SA_AN,@strGUID,@serialNumber,@CheckOutNO,@WBID,@SA_Account_Name,@SA_Account_WH,@LicensePN,@Weight_Mao,@Weight_Pi,@Weight_Jing,@StockType_Name,@strYear,@Variety_Name,@VarietyLevel_Name,@ChuLiangRongLiang,@RongLiangKouZhong,@ZaZhiHanLiang,@ZazhiKouZhong,@ShuiFenHanLiang,@ShuiFenKouZhong,@QiTaKouZhong,@strLevel,@Weight_Total,@Weight_Reality,@Weigh_Name,@Quanlity_Name,@dt_Trade)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@CheckOutNO", SqlDbType.NVarChar,50),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@SA_Account_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@SA_Account_WH", SqlDbType.NVarChar,50),
					new SqlParameter("@LicensePN", SqlDbType.NVarChar,50),
					new SqlParameter("@Weight_Mao", SqlDbType.Decimal,9),
					new SqlParameter("@Weight_Pi", SqlDbType.Decimal,9),
					new SqlParameter("@Weight_Jing", SqlDbType.Decimal,9),
					new SqlParameter("@StockType_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@strYear", SqlDbType.NVarChar,50),
					new SqlParameter("@Variety_Name", SqlDbType.NVarChar,50),
                    new SqlParameter("@VarietyLevel_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@ChuLiangRongLiang", SqlDbType.Decimal,9),
					new SqlParameter("@RongLiangKouZhong", SqlDbType.Decimal,9),
					new SqlParameter("@ZaZhiHanLiang", SqlDbType.Decimal,9),
					new SqlParameter("@ZazhiKouZhong", SqlDbType.Decimal,9),
					new SqlParameter("@ShuiFenHanLiang", SqlDbType.Decimal,9),
					new SqlParameter("@ShuiFenKouZhong", SqlDbType.Decimal,9),
					new SqlParameter("@QiTaKouZhong", SqlDbType.Decimal,9),
					new SqlParameter("@strLevel", SqlDbType.NVarChar,50),
					new SqlParameter("@Weight_Total", SqlDbType.Decimal,9),
					new SqlParameter("@Weight_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Weigh_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@Quanlity_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parameters[0].Value = SA_AN;
            parameters[1].Value = strGUID;
            parameters[2].Value = serialNumber;
            parameters[3].Value = CheckOutNO;
            parameters[4].Value = WBID;
            parameters[5].Value = SA_Account_Name;
            parameters[6].Value = SA_Account_WH;
            parameters[7].Value = LicensePN;
            parameters[8].Value = Weight_Mao;
            parameters[9].Value = Weight_Pi;
            parameters[10].Value = Weight_Jing;
            parameters[11].Value = StockType_Name;
            parameters[12].Value = strYear;
            parameters[13].Value = Variety_ID;
            parameters[14].Value = VarietyLevel_ID;
            parameters[15].Value = ChuLiangRongLiang;
            parameters[16].Value = RongLiangKouZhong;
            parameters[17].Value = ZaZhiHanLiang;
            parameters[18].Value = ZazhiKouZhong;
            parameters[19].Value = ShuiFenHanLiang;
            parameters[20].Value = ShuiFenKouZhong;
            parameters[21].Value = QiTaKouZhong;
            parameters[22].Value = strLevel;
            parameters[23].Value = Weight_Total;
            parameters[24].Value = Weight_Reality;
            parameters[25].Value = Weigh_Name;
            parameters[26].Value = Quanlity_Name;
            parameters[27].Value = dt_Trade;

            //添加商品出库日志

            string WBName = SQLHelper.ExecuteScalar("  SELECT strName  FROM dbo.WB WHERE ID="+WBID).ToString();
            string UserID = context.Session["ID"].ToString();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();
            string BusinessName = "出库";

            string UnitID = "公斤";//暂定
            string Price = "0.00";//暂定 
            string Money_Trade = "0.00";//暂定
            string strYear2=DateTime.Now.Year.ToString();
            string strMonth=Fun.ConvertIntToString(DateTime.Now.Month,2);
            string strDay=Fun.ConvertIntToString(DateTime.Now.Day,2);
            string str_Date = strYear2 + "-" + strMonth + "-" + strDay;


            string Count_Trade = Weight_Reality;
            string WB_ID = context.Request.Form["WB_ID"].ToString();//出库的网点编号
            double Count_Balance = 0;
            object objBalance = SQLHelper.ExecuteScalar(" SELECT numStorage FROM dbo.SA_VarietyStorage WHERE VarietyID=" + Variety_ID + " and VarietyLevelID=" + VarietyLevel_ID+" AND WBID=" + WBID);
            if (objBalance != null && objBalance.ToString() != "")
            {
                Count_Balance = Convert.ToDouble(objBalance) - Convert.ToDouble(Count_Trade);
            }
            Count_Balance = Math.Round(Count_Balance, 4);//此次出库后网点应该剩余的库存量

           

            string SA_VarietyStorage_ID = context.Request.Form["SA_VarietyStorage_ID"].ToString();//产品出库编号



            int storageType = 4;
            double numStorageIn = 0;
            double numStorageOut = 0;
            double numStorage = 0;
            double numStorageChange = 0;
            double numStorageLoss = 0;
            //查询上一次的存粮操作记录
            DataRow rowVSLog_last = common.GetLastVSLog(WBID, Variety_ID,VarietyLevel_ID);
            if (rowVSLog_last != null) {
                numStorageIn = Convert.ToDouble(rowVSLog_last["numStorageIn"]);
                numStorageOut = Convert.ToDouble(rowVSLog_last["numStorageOut"]);
                numStorage= Convert.ToDouble(rowVSLog_last["numStorage"]);
                //double numStorageChange_last = Convert.ToDouble(rowVSLog_last["numStorageChange"]);
                //double numStorageLoss_last = Convert.ToDouble(rowVSLog_last["numStorageLoss"]);
            }
            numStorageChange = -Convert.ToDouble(Count_Trade);//存粮数据变化量，出库取负数
            numStorageOut = numStorageOut + numStorageChange;
            numStorage = numStorage + numStorageChange;

            //根据ID修改当前网点的库存数量
            string strSqlUpdate = " UPDATE dbo.SA_VarietyStorage SET numStorage=" + numStorage + " WHERE ID=" + SA_VarietyStorage_ID;
            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    object objID = SQLHelper.ExecuteScalar(tran, CommandType.Text, strSql.ToString(), parameters);//出库记录

                    StringBuilder strSqlLog = new StringBuilder();
                    strSqlLog.Append("insert into [SA_CheckOutLog] (");
                    strSqlLog.Append("CheckOut_ID,WBID,UserID,AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,Count_Trade,Money_Trade,Count_Balance,dt_Trade)");
                    strSqlLog.Append(" values (");
                    strSqlLog.Append("@CheckOut_ID,@WBID,@UserID,@AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade)");
                    strSqlLog.Append(";select @@IDENTITY");
                    SqlParameter[] parametersLog = {
					new SqlParameter("@CheckOut_ID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.NVarChar,50),
					new SqlParameter("@UserID", SqlDbType.NVarChar,50),
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyID", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitID", SqlDbType.NVarChar,50),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
					new SqlParameter("@Count_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Count_Balance", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Trade", SqlDbType.NVarChar,50)};
                    parametersLog[0].Value = objID;
                    parametersLog[1].Value = WBID;
                    parametersLog[2].Value = UserID;
                    parametersLog[3].Value = AccountNumber;
                    parametersLog[4].Value = BusinessNO;
                    parametersLog[5].Value = BusinessName;
                    parametersLog[6].Value = Variety_ID;
                    parametersLog[7].Value = UnitID;
                    parametersLog[8].Value = Price;
                    parametersLog[9].Value = Count_Trade;
                    parametersLog[10].Value = Money_Trade;
                    parametersLog[11].Value = Count_Balance;
                    parametersLog[12].Value = dt_Trade;
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text,strSqlLog.ToString(),parametersLog);//出库日志记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlUpdate.ToString());


                    //添加存粮操作日志
                    StringBuilder sqlVS = new StringBuilder();
                    sqlVS.Append(" ");
                    sqlVS.Append(" INSERT INTO dbo.SA_VarietyStorageLog");
                    sqlVS.Append("   ( storageType , OperateLogID ,  CheckOutID ,   AccountNumber ,  VarietyID ,VarietyLevelID ,WBID , numStorage, numStorageChange, numStorageIn ,numStorageOut ,numStorageLoss , WareHouseID ,ISHQ , ISSimulate,dtLog)");
                    sqlVS.Append(" VALUES  ( " + storageType + " ,0 ,   " + objID.ToString() + " ,  N'" + SA_AN + "' ,  " + Variety_ID + " ,  " + VarietyLevel_ID + " ,  " + WBID + " , ");
                    sqlVS.Append("    " + numStorage + " ," + numStorageChange + " , " + numStorageIn + " , " + numStorageOut + " , " + numStorageLoss + " ,");
                    sqlVS.Append("    0 , 0 ,   0 ,'"+DateTime.Now.ToString()+"'  )");
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlVS.ToString());
                    tran.Commit();

                    context.Response.Write(objID);
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("0");
                }
            }

         
        }


        /// <summary>
        /// 更新网点出库记录
        /// </summary>
        /// <param name="context"></param>
        void UpdateSA_CheckOut(HttpContext context)
        {
            string CheckOutID = context.Request.Form["CheckOutID"].ToString();//存储产品编号
            //string strSqlCheckOut = " SELECT Variety_Name,Weight_Reality,WBID  FROM dbo.SA_CheckOut WHERE ID="+CheckOutID;
            string strSqlCheckOut = " SELECT *  FROM dbo.SA_CheckOut WHERE ID=" + CheckOutID;
            DataTable dtCheckOut = SQLHelper.ExecuteDataTable(strSqlCheckOut);
            string O_VarietyName = dtCheckOut.Rows[0]["Variety_Name"].ToString();
            string O_VarietyLevelName = dtCheckOut.Rows[0]["VarietyLevel_Name"].ToString();
            string O_Weight_Reality = dtCheckOut.Rows[0]["Weight_Reality"].ToString();//原来的实际扣重
            string O_WBID= dtCheckOut.Rows[0]["WBID"].ToString();
           
            string Weight_Mao = context.Request.Form["Weight_Mao"].ToString();
            string Weight_Pi = context.Request.Form["Weight_Pi"].ToString();
            string Weight_Jing = context.Request.Form["Weight_Total"].ToString();////
           
            string ChuLiangRongLiang = context.Request.Form["ChuLiangRongLiang"].ToString();
            string RongLiangKouZhong = context.Request.Form["RongLiangKouZhong"].ToString();
            string ZaZhiHanLiang = context.Request.Form["ZaZhiHanLiang"].ToString();
            string ZazhiKouZhong = context.Request.Form["ZazhiKouZhong"].ToString();
            string ShuiFenHanLiang = context.Request.Form["ShuiFenHanLiang"].ToString();
            string ShuiFenKouZhong = context.Request.Form["ShuiFenKouZhong"].ToString();
            string QiTaKouZhong = context.Request.Form["QiTaKouZhong"].ToString();
        
            string Weight_Total = context.Request.Form["Weight_Total"].ToString();
            string Weight_Reality = context.Request.Form["Weight_Reality"].ToString();//修改后的实际扣重
           

            //添加商品出库记录
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  UPDATE dbo.SA_CheckOut ");
            strSql.Append("  SET Weight_Mao="+Weight_Mao+",");
            strSql.Append("  Weight_Pi="+Weight_Pi+",");
            strSql.Append("  Weight_Jing="+Weight_Jing+",");
            strSql.Append("  Weight_Total="+Weight_Total+",");
            strSql.Append("  Weight_Reality="+Weight_Reality+", ");
            strSql.Append("  ChuLiangRongLiang="+ChuLiangRongLiang+",");
            strSql.Append("  RongLiangKouZhong="+RongLiangKouZhong+",");
            strSql.Append("  ZaZhiHanLiang="+ZaZhiHanLiang+",");
            strSql.Append("  ZazhiKouZhong="+ZazhiKouZhong+",");
            strSql.Append("  ShuiFenHanLiang="+ShuiFenHanLiang+",");
            strSql.Append("  ShuiFenKouZhong="+ShuiFenKouZhong+",");
            strSql.Append("  QiTaKouZhong="+QiTaKouZhong);
            strSql.Append("  WHERE ID=" + CheckOutID);

            //添加商品出库日志
            double WeightGap = Convert.ToDouble(Weight_Reality) - Convert.ToDouble(O_Weight_Reality);
            WeightGap = Math.Round(WeightGap, 4);
          

            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append(" UPDATE dbo.SA_CheckOutLog ");
            strSqlLog.Append(" SET Count_Trade=" + Weight_Reality + ", ");
            if (WeightGap > 0)//修改后的出库数量大于原始的出库数量
            {
                strSqlLog.Append(" Count_Balance=Count_Balance- " + Math.Abs( WeightGap));
            }
            else {
                strSqlLog.Append(" Count_Balance=Count_Balance+ " + Math.Abs(WeightGap));
            }
            strSqlLog.Append(" WHERE CheckOut_ID="+CheckOutID);


            int storageType = 5;
            double numStorageIn = 0;
            double numStorageOut = 0;
            double numStorage = 0;
            double numStorageChange = 0;
            double numStorageLoss = 0;
            //查询上一次的存粮操作记录
            DataRow rowVSLog_last = common.GetLastVSLog(O_WBID, O_VarietyName,O_VarietyLevelName);
            if (rowVSLog_last != null)
            {
                numStorageIn = Convert.ToDouble(rowVSLog_last["numStorageIn"]);
                numStorageOut = Convert.ToDouble(rowVSLog_last["numStorageOut"]);
                numStorage = Convert.ToDouble(rowVSLog_last["numStorage"]);
                //double numStorageChange_last = Convert.ToDouble(rowVSLog_last["numStorageChange"]);
                //double numStorageLoss_last = Convert.ToDouble(rowVSLog_last["numStorageLoss"]);
            }
            numStorageChange = -Convert.ToDouble(WeightGap);
            numStorageOut = numStorageOut + numStorageChange;
            numStorage = numStorage + numStorageChange;

            //根据ID修改当前网点的库存数量
            StringBuilder strSqlUpdate = new StringBuilder();
            strSqlUpdate.Append(" UPDATE dbo.SA_VarietyStorage SET numStorage=" + numStorage);
            strSqlUpdate.Append(" where 1=1 and VarietyID=" + O_VarietyName + " and VarietyLevelID=" +O_VarietyLevelName+ " and WBID=" + O_WBID);
            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    object objID = SQLHelper.ExecuteScalar(tran, CommandType.Text, strSql.ToString());//出库记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlLog.ToString());//出库日志记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlUpdate.ToString());


                    //添加存粮操作日志
                    StringBuilder sqlVS = new StringBuilder();
                    sqlVS.Append(" ");
                    sqlVS.Append(" INSERT INTO dbo.SA_VarietyStorageLog");
                    sqlVS.Append("   ( storageType , OperateLogID ,  CheckOutID ,   AccountNumber ,  VarietyID ,VarietyLevelID ,WBID , numStorage, numStorageChange, numStorageIn ,numStorageOut ,numStorageLoss , WareHouseID ,ISHQ , ISSimulate,dtLog)");
                    sqlVS.Append(" VALUES  ( " + storageType + " ,0 ,   " + CheckOutID + " ,  N'" + dtCheckOut.Rows[0]["SA_AN"].ToString() + "' ,  " + O_VarietyName + " ,  " + O_VarietyLevelName + " ,  " + O_WBID + " , ");
                    sqlVS.Append("    " + numStorage + " ," + numStorageChange + " , " + numStorageIn + " , " + numStorageOut + " , " + numStorageLoss + " ,");
                    sqlVS.Append("    0 , 0 ,   0,'"+DateTime.Now.ToString()+"'   )");
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlVS.ToString());

                    tran.Commit();

                    context.Response.Write(objID);
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("0");
                }
            }


        }

        /// <summary>
        /// 根据网点名称获取网点营业员的名称
        /// </summary> 
        /// <param name="context"></param>
        void GetCheckOut_LogByCheckOut_ID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
         StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,CheckOut_ID,WBID,UserID,AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,Count_Trade,Money_Trade,Count_Balance,dt_Trade ");
			strSql.Append(" FROM [SA_CheckOutLog] ");
            strSql.Append(" where CheckOut_ID=@ID ");
			SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
			parameters[0].Value = ID;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(),parameters);

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        /// <summary>
        /// 获取消费信息与预付款消费记录 
        /// </summary>
        /// <param name="context"></param>
        void PrintSA_CheckOutLog(HttpContext context)
        {

          
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select ID,WBID,UserID,AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,Count_Trade,Money_Trade,Count_Balance,dt_Trade ");

            strSqlLog.Append(" FROM SA_CheckOutLog");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string WBID = dtLog.Rows[0]["WBID"].ToString();
            int numValue = 1;
            if (int.TryParse(WBID, out numValue))
            {
                object objwbid = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.WB  WHERE ID=" + WBID);
                if (objwbid != null && objwbid.ToString() != "")
                {
                    WBID = objwbid.ToString();
                }
            }
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string VarietyID = dtLog.Rows[0]["VarietyID"].ToString();
           
            if (int.TryParse(VarietyID, out numValue))
            {
                object objVarietyID = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.StorageVariety  WHERE ID=" + VarietyID);
                if (objVarietyID != null && objVarietyID.ToString() != "")
                {
                    VarietyID = objVarietyID.ToString();
                }
            }
            string UnitID = dtLog.Rows[0]["UnitID"].ToString();
            string Price = dtLog.Rows[0]["Price"].ToString();
            string PriceUnit = Price + "元/" + UnitID;
          
            // string Price_C = dtLog.Rows[0]["Price_C"].ToString();
            string CountTrade = dtLog.Rows[0]["Count_Trade"].ToString();
            if (CountTrade == "0.00")
            {
                CountTrade = "";
            }
            string Money_Trade = dtLog.Rows[0]["Money_Trade"].ToString();

            string Count_Balance = dtLog.Rows[0]["Count_Balance"].ToString();

            DateTime dt_Trade = Convert.ToDateTime(dtLog.Rows[0]["dt_Trade"]);
           
            string strTrade = dt_Trade.Year.ToString() + "/" + dt_Trade.Month.ToString() + "/" + dt_Trade.Day.ToString();
            string numWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += " where 1=1 and WBID=" + numWBID;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null && dt.Rows.Count != 0)
            {
                int numBusinessNo = Convert.ToInt32(BusinessNO);
                int numIndex = 0;//确定当前打印的行的索引
                List<int> listUp = new List<int>();
                List<int> listDown = new List<int>();
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (i % 2 == 0)
                        {
                            listDown.Add(i * 10 + j + 1);
                        }
                        else
                        {
                            listUp.Add(i * 10 + j + 1);
                        }
                    }
                }
                if (listUp.Contains(numBusinessNo))
                {
                    if (numBusinessNo % 10 == 0)
                    {
                        numIndex = 10;
                    }
                    else
                    {
                        numIndex = Convert.ToInt32(BusinessNO.Substring(BusinessNO.Length - 1));
                    }
                }
                else
                {
                    if (numBusinessNo % 10 == 0)
                    {
                        numIndex = 20;
                    }
                    else
                    {
                        numIndex = 10 + Convert.ToInt32(BusinessNO.Substring(BusinessNO.Length - 1));
                    }
                }

                string strName = "RecordR" + numIndex.ToString() + "Y";
                int RecordRY = Convert.ToInt32(dt.Rows[0][strName]);//当前行的Y坐标位置


                int FontSize = Convert.ToInt32(dt.Rows[0]["FontSize"]);
                int RecordC1X = Convert.ToInt32(dt.Rows[0]["RecordC1X"]);
                int RecordC2X = Convert.ToInt32(dt.Rows[0]["RecordC2X"]);
                int RecordC3X = Convert.ToInt32(dt.Rows[0]["RecordC3X"]);
                int RecordC4X = Convert.ToInt32(dt.Rows[0]["RecordC4X"]);
                int RecordC5X = Convert.ToInt32(dt.Rows[0]["RecordC5X"]);
                int RecordC6X = Convert.ToInt32(dt.Rows[0]["RecordC6X"]);
                int RecordC7X = Convert.ToInt32(dt.Rows[0]["RecordC7X"]);
                int RecordC8X = Convert.ToInt32(dt.Rows[0]["RecordC8X"]);
                int RecordC9X = Convert.ToInt32(dt.Rows[0]["RecordC9X"]);

                StringBuilder strReturn = new StringBuilder();
                strReturn.Append("  <table style='width:100%; height:" + RecordRY + "px'><tr><td></td> </tr></table>");
                strReturn.Append("   <table style='margin-left:" + RecordC1X + "px; font-size:" + FontSize + "px;'><tr>");
                strReturn.Append("   <td style='width:" + (RecordC2X - RecordC1X).ToString() + "px;'>" + strTrade + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>" + BusinessName + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>" + VarietyID + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>" + PriceUnit + "</td>");
                strReturn.Append("   <td style='width:" + (RecordC6X - RecordC5X).ToString() + "px;'>" + CountTrade + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>0.00</td>");
                strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>" + Count_Balance + "</td>");
                strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>" + WBID + "</td>");
               // strReturn.Append("   <td >" + WBID + " </td>");
                strReturn.Append("   </tr></table>");

                context.Response.Write(strReturn.ToString());

            }
        }


        /// <summary>
        /// 根据网点名称获取网点营业员的名称
        /// </summary>
        /// <param name="context"></param>
        void GetStorageVarietyInfo(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT VarietyID,VarietyLevelID,WBID,numStorage,WareHouseID FROM dbo.SA_VarietyStorage WHERE ID=" + ID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        /// <summary>
        /// 根据网点名称获取网点营业员的名称
        /// </summary>
        /// <param name="context"></param>
        void GetUserByWBName(HttpContext context)
        {
            string WBName = context.Request.QueryString["WBName"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT  A.ID,A.strRealName as strName");
            strSql.Append("  FROM dbo.Users A INNER JOIN dbo.WB B ON A.WB_ID=B.ID");
            strSql.Append("  WHERE B.strName='"+WBName+"'");
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        /// <summary>
        /// 获取单笔代理费的信息
        /// </summary>
        /// <param name="context"></param>
        void GetAgentFeeSingle(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT A.ID,A.VarietyID ,V.strName AS VarietyName, C.strName AS WBName,B.AccountNumber,B.strName,'存粮代理' AS BusinessName,CONVERT(NVARCHAR(100),A.StorageDate,23 ) AS  StorageDate, DATEDIFF(DAY,A.StorageDate,GETDATE()) AS StorageDay,");
            strSql.Append("  A.StorageNumberRaw AS CunRu,0 AS ZhiQu,A.StorageNumberRaw AS ShiCun,C.numAgent,A.StorageNumberRaw*C.numAgent AS MoenyFee");
            strSql.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB C ON B.WBID=C.ID");
            strSql.Append("    LEFT OUTER JOIN dbo.StorageVariety V ON A.VarietyID=V.ID");
            strSql.Append("  where A.ID=" + ID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            string WBName = dt.Rows[0]["WBName"].ToString();
            DataTable dtWB = SQLHelper.ExecuteDataTable(" SELECT  top 1  numDay,draw_exchange,draw_sell,draw_shopping FROM dbo.WB WHERE strName ='" + WBName + "'");
            if (dtWB == null || dtWB.Rows.Count == 0)
            {
                Fun.Alert("查询该网点信息失败！");
                return;
            }
            int numDay = Convert.ToInt32(dtWB.Rows[0]["numDay"]);
            int draw_exchange = Convert.ToInt32(dtWB.Rows[0]["draw_exchange"]);
            int draw_sell = Convert.ToInt32(dtWB.Rows[0]["draw_sell"]);
            int draw_shopping = Convert.ToInt32(dtWB.Rows[0]["draw_shopping"]);
            string buslist = "";
            if (draw_exchange == 1)
            {
                buslist += "," + "'2','6'";
            }
            if (draw_sell == 1)
            {
                buslist += "," + "'3','7'";
            }
            if (draw_shopping == 1)
            {
                buslist += "," + "'9','10'";
            }


            if (buslist != "")
            {
                buslist = buslist.Substring(1);

                double zhiqu = 0;
                double shicun = 0;
                double cunru = Convert.ToDouble(dt.Rows[0]["CunRu"]);
                double numAgent = Convert.ToDouble(dt.Rows[0]["numAgent"]);
                double MoenyFee = Convert.ToDouble(dt.Rows[0]["MoenyFee"]);
                string AccountNumber = dt.Rows[0]["AccountNumber"].ToString();
                string VarietyID = dt.Rows[0]["VarietyID"].ToString();
                string StorageDate = dt.Rows[0]["StorageDate"].ToString();
                string sql = string.Format("   SELECT SUM(Count_Trade) FROM dbo.Dep_OperateLog  WHERE BusinessName IN ({0}) AND Dep_AccountNumber='{1}' AND VarietyID='{2}'  AND  DATEDIFF(day,'{3}',dt_Trade)<={4}", buslist, AccountNumber, VarietyID, StorageDate, numDay);
                object obj_zhiqu = SQLHelper.ExecuteScalar(sql);
                if (obj_zhiqu != null && obj_zhiqu.ToString() != "")
                {
                    zhiqu = Convert.ToDouble(obj_zhiqu);
                }
                shicun = cunru - zhiqu;
                MoenyFee = shicun * numAgent;
                dt.Rows[0]["ZhiQu"] = Math.Round(zhiqu, 2);
                dt.Rows[0]["ShiCun"] = Math.Round(shicun, 2);
                dt.Rows[0]["MoenyFee"] = Math.Round(MoenyFee, 2);
            }



            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        ///// <summary>
        ///// 获取单笔代理费的信息
        ///// </summary>
        ///// <param name="context"></param>
        //void GetAgentFeeSingle(HttpContext context)
        //{
        //    string ID = context.Request.QueryString["ID"].ToString();
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("  SELECT A.ID, C.strName AS WBName,B.AccountNumber,B.strName,'存粮代理' AS BusinessName,CONVERT(NVARCHAR(100),A.StorageDate,23 ) AS  StorageDate, DATEDIFF(DAY,A.StorageDate,GETDATE()) AS StorageDay,");
        //    strSql.Append("  A.StorageNumberRaw AS CunRu,0 AS ZhiQu,A.StorageNumberRaw AS ShiCun,C.numAgent,A.StorageNumberRaw*C.numAgent AS MoenyFee");
        //    strSql.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
        //    strSql.Append("  INNER JOIN dbo.WB C ON B.WBID=C.ID");
        //    strSql.Append("  where A.ID="+ID);
        //    DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

        //    if (dt != null && dt.Rows.Count != 0)
        //    {
        //        context.Response.Write(JsonHelper.ToJson(dt));
        //    }
        //    else
        //    {
        //        context.Response.Write("Error");
        //    }
        //}

        /// <summary>
        /// 当前记录是否已经发生代理费结算
        /// </summary>
        /// <param name="context"></param>
        void ISHaveAgentFee(HttpContext context)
        {
            string Dep_SID = context.Request.QueryString["Dep_SID"].ToString();
            StringBuilder strSql = new StringBuilder();

            strSql.Append(" SELECT COUNT(ID)  FROM dbo.SA_AgentFee WHERE Dep_StorageInfoID=" + Dep_SID);

            context.Response.Write(SQLHelper.ExecuteScalar(strSql.ToString()).ToString());
        }
         
        /// <summary>
        /// 添加单笔的结算业务
        /// </summary>
        /// <param name="context"></param>
        void AddSA_AgentFee(HttpContext context)
        {
            string Dep_SID = context.Request.QueryString["Dep_SID"].ToString();
            string WBUser = context.Request.QueryString["WBUser"].ToString();
            StringBuilder strSqlInfo = new StringBuilder();
            strSqlInfo.Append("   SELECT F.ID AS AgentFeeID, A.ID,A.VarietyID ,V.strName AS VarietyName, C.strName AS WBName,B.AccountNumber,B.strName,'存粮代理' AS BusinessName,CONVERT(NVARCHAR(100),A.StorageDate,23 ) AS  StorageDate,");
            strSqlInfo.Append("   DATEDIFF(DAY,A.StorageDate,GETDATE()) AS StorageDay,  A.StorageNumberRaw AS CunRu, 0 as zhiqu,A.StorageNumberRaw AS shicun, C.numAgent,A.StorageNumberRaw*C.numAgent AS MoenyFee ");
            strSqlInfo.Append("    FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber  ");
            strSqlInfo.Append("    LEFT OUTER JOIN dbo.StorageVariety V ON A.VarietyID=V.ID");
            strSqlInfo.Append("    LEFT OUTER JOIN dbo.SA_AgentFee F ON A.ID=F.Dep_StorageInfoID");
            strSqlInfo.Append("   INNER JOIN dbo.WB C ON B.WBID=C.ID ");
            strSqlInfo.Append("  where A.ID=" + Dep_SID);
            DataTable dtInfo = SQLHelper.ExecuteDataTable(strSqlInfo.ToString());
            if (dtInfo == null || dtInfo.Rows.Count == 0) {
                context.Response.Write("Error");
                return;
            }

            string WBName = dtInfo.Rows[0]["WBName"].ToString();
            DataTable dtWB = SQLHelper.ExecuteDataTable(" SELECT  top 1  numDay,draw_exchange,draw_sell,draw_shopping FROM dbo.WB WHERE strName ='" + WBName + "'");
            if (dtWB == null || dtWB.Rows.Count == 0)
            {
                Fun.Alert("查询该网点信息失败！");
                return;
            }
            int numDay = Convert.ToInt32(dtWB.Rows[0]["numDay"]);
            int draw_exchange = Convert.ToInt32(dtWB.Rows[0]["draw_exchange"]);
            int draw_sell = Convert.ToInt32(dtWB.Rows[0]["draw_sell"]);
            int draw_shopping = Convert.ToInt32(dtWB.Rows[0]["draw_shopping"]);
            string buslist = "";
            if (draw_exchange == 1)
            {
                buslist += "," + "'2','6'";
            }
            if (draw_sell == 1)
            {
                buslist += "," + "'3','7'";
            }
            if (draw_shopping == 1)
            {
                buslist += "," + "'9','10'";
            }
            if (buslist != "")
            {
                buslist = buslist.Substring(1);
                double zhiqu = 0;
                double shicun = 0;
                double cunru = Convert.ToDouble(dtInfo.Rows[0]["CunRu"]);
                double numAgent = Convert.ToDouble(dtInfo.Rows[0]["numAgent"]);
                double MoenyFee = Convert.ToDouble(dtInfo.Rows[0]["MoenyFee"]);
                string AccountNumber = dtInfo.Rows[0]["AccountNumber"].ToString();
                string VarietyID = dtInfo.Rows[0]["VarietyID"].ToString();
                string StorageDate_para = dtInfo.Rows[0]["StorageDate"].ToString();
                string sql = string.Format("   SELECT SUM(Count_Trade) FROM dbo.Dep_OperateLog  WHERE BusinessName IN ({0}) AND Dep_AccountNumber='{1}' AND VarietyID='{2}'  AND  DATEDIFF(day,'{3}',dt_Trade)<={4}", buslist, AccountNumber, VarietyID, StorageDate_para, numDay);
                object obj_zhiqu = SQLHelper.ExecuteScalar(sql);
                if (obj_zhiqu != null && obj_zhiqu.ToString() != "")
                {
                    zhiqu = Convert.ToDouble(obj_zhiqu);
                }
                shicun = cunru - zhiqu;
                MoenyFee = shicun * numAgent;
                dtInfo.Rows[0]["zhiqu"] = Math.Round(zhiqu, 2);
                dtInfo.Rows[0]["shicun"] = Math.Round(shicun, 2);
                dtInfo.Rows[0]["MoenyFee"] = Math.Round(MoenyFee, 2);
            }

            string SA_AN = "";//暂时不用
            string Dep_StorageInfoID = Dep_SID;
            string strGUID = Fun.getGUID();//获取新的GUID
            //临时暂用的编号
            string serialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0000001";
          
            string Dep_AN = dtInfo.Rows[0]["AccountNumber"].ToString();
            string Dep_Name = dtInfo.Rows[0]["strName"].ToString();
            string StorageDate = dtInfo.Rows[0]["StorageDate"].ToString();
            string StorageDay = dtInfo.Rows[0]["StorageDay"].ToString();
            string BusinessName ="代理费结算";
            string numWeight_Storage = dtInfo.Rows[0]["CunRu"].ToString();
            string numWeight_ZhiQu = dtInfo.Rows[0]["zhiqu"].ToString();
            string numWeight_Effect = dtInfo.Rows[0]["shicun"].ToString();
            string AgentFee = dtInfo.Rows[0]["numAgent"].ToString();
            string Money_Total = dtInfo.Rows[0]["MoenyFee"].ToString();
            string Money_Reality = Money_Total;
            string Money_Surplus = "0";
            string dt_Trade = DateTime.Now.ToString();
            string Accountant_Name = WBUser;

            StringBuilder strSqlGoodInfo = new StringBuilder();
            strSqlGoodInfo.Append("  SELECT B.strName AS GoodName,C.strName AS UnitName");
            strSqlGoodInfo.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSqlGoodInfo.Append("  INNER JOIN dbo.BD_MeasuringUnit C ON B.MeasuringUnitID=C.ID");
            strSqlGoodInfo.Append("  where A.ID=" + Dep_SID);
            DataTable dtGoodInfo = SQLHelper.ExecuteDataTable(strSqlGoodInfo.ToString());
            string GoodName = dtGoodInfo.Rows[0]["GoodName"].ToString();
            string UnitName = dtGoodInfo.Rows[0]["UnitName"].ToString();
            string SpecName = "";


            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SA_AgentFee] (");
            strSql.Append("SA_AN,Dep_StorageInfoID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight_Storage,numWeight_ZhiQu,numWeight_Effect,AgentFee,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,dt_Trade)");
            strSql.Append(" values (");
            strSql.Append("@SA_AN,@Dep_StorageInfoID,@strGUID,@serialNumber,@WBName,@Dep_AN,@Dep_Name,@StorageDate,@StorageDay,@BusinessName,@GoodName,@UnitName,@SpecName,@numWeight_Storage,@numWeight_ZhiQu,@numWeight_Effect,@AgentFee,@Money_Total,@Money_Reality,@Money_Surplus,@Accountant_Name,@dt_Trade)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_StorageInfoID", SqlDbType.Int,4),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WBName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDay", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
					new SqlParameter("@numWeight_Storage", SqlDbType.Decimal,9),
					new SqlParameter("@numWeight_ZhiQu", SqlDbType.Decimal,9),
					new SqlParameter("@numWeight_Effect", SqlDbType.Decimal,9),
					new SqlParameter("@AgentFee", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Surplus", SqlDbType.Decimal,9),
					new SqlParameter("@Accountant_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parameters[0].Value = SA_AN;
            parameters[1].Value = Dep_StorageInfoID;
            parameters[2].Value = strGUID;
            parameters[3].Value = serialNumber;
            parameters[4].Value = WBName;
            parameters[5].Value = Dep_AN;
            parameters[6].Value = Dep_Name;
            parameters[7].Value = StorageDate;
            parameters[8].Value = StorageDay;
            parameters[9].Value = BusinessName;
            parameters[10].Value = GoodName;
            parameters[11].Value = UnitName;
            parameters[12].Value = SpecName;
            parameters[13].Value = numWeight_Storage;
            parameters[14].Value = numWeight_ZhiQu;
            parameters[15].Value = numWeight_Effect;
            parameters[16].Value = AgentFee;
            parameters[17].Value = Money_Total;
            parameters[18].Value = Money_Reality;
            parameters[19].Value = Money_Surplus;
            parameters[20].Value = Accountant_Name;
            parameters[21].Value = dt_Trade;




            if (SQLHelper.ExecuteNonQuery(strSql.ToString(),parameters)!= 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 添加单笔的结算业务
        /// </summary>
        /// <param name="context"></param>
        void AddSA_AgentFeeAll(HttpContext context)
        {
            string WBID = context.Request.QueryString["WBID"].ToString();
            string WBName = common.GetWBInfoByID(Convert.ToInt32(WBID))["strName"].ToString();
            string dtStart = context.Request.QueryString["dtStart"].ToString();
            string dtEnd = context.Request.QueryString["dtEnd"].ToString();


            DataTable dtWB = SQLHelper.ExecuteDataTable(" SELECT  top 1  numDay,draw_exchange,draw_sell,draw_shopping FROM dbo.WB WHERE ID =" + WBID);
            if (dtWB == null || dtWB.Rows.Count == 0)
            {
                Fun.Alert("查询该网点信息失败！");
                return;
            }
            int numDay = Convert.ToInt32(dtWB.Rows[0]["numDay"]);
            int draw_exchange = Convert.ToInt32(dtWB.Rows[0]["draw_exchange"]);
            int draw_sell = Convert.ToInt32(dtWB.Rows[0]["draw_sell"]);
            int draw_shopping = Convert.ToInt32(dtWB.Rows[0]["draw_shopping"]);
            string buslist = "";
            if (draw_exchange == 1)
            {
                buslist += "," + "'2','6'";
            }
            if (draw_sell == 1)
            {
                buslist += "," + "'3','7'";
            }
            if (draw_shopping == 1)
            {
                buslist += "," + "'9','10'";
            }
            if (buslist != "")
            {
                buslist = buslist.Substring(1);
            }
            //获取存粮信息
           
            StringBuilder strSqlPara = new StringBuilder();//查询参数
            strSqlPara.Append("   SELECT F.ID AS AgentFeeID, A.ID,A.VarietyID ,V.strName AS VarietyName, C.strName AS WBName,B.AccountNumber,B.strName,'存粮代理' AS BusinessName,CONVERT(NVARCHAR(100),A.StorageDate,23 ) AS  StorageDate,");
            strSqlPara.Append("   DATEDIFF(DAY,A.StorageDate,GETDATE()) AS StorageDay,  A.StorageNumberRaw AS CunRu, 0 as zhiqu,A.StorageNumberRaw AS shicun, C.numAgent,A.StorageNumberRaw*C.numAgent AS MoenyFee ");
            strSqlPara.Append("    FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber  ");
            strSqlPara.Append("    LEFT OUTER JOIN dbo.StorageVariety V ON A.VarietyID=V.ID");
            strSqlPara.Append("    LEFT OUTER JOIN dbo.SA_AgentFee F ON A.ID=F.Dep_StorageInfoID");
            strSqlPara.Append("   INNER JOIN dbo.WB C ON B.WBID=C.ID ");

            strSqlPara.Append("  WHERE DATEDIFF(minute,A.StorageDate,GETDATE())>" + numDay);
            if (WBID != "")
            {
                strSqlPara.Append("   AND A.WBID = '" + WBID + "'");
            }
            if (dtStart != "")
            {
                strSqlPara.Append("   AND A.StorageDate> '" + dtStart + "'");
            }
            if (dtEnd != "")
            {
                dtEnd = Convert.ToDateTime(dtEnd).AddDays(1).ToString();
                strSqlPara.Append("   AND A.StorageDate < '" + dtEnd + "'");
            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSqlPara.ToString());
                  
            if (dt == null || dt.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "不存在需要结算的项目" };
                context.Response.Write(JsonHelper.ToJson(res)); 
                return;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {      
                string SA_AN = "";//暂时不用
                string AgentFeeID = dt.Rows[i]["AgentFeeID"].ToString();
                if (AgentFeeID != "") { continue; }//已经存在的结算
                string Dep_StorageInfoID = dt.Rows[i]["ID"].ToString();
                string strGUID = Fun.getGUID();//获取新的GUID
                //临时暂用的编号
                string serialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0000001";

                if (buslist != "")
                {
                    double zhiqu = 0;
                    double shicun = 0;
                    double cunru = Convert.ToDouble(dt.Rows[i]["CunRu"]);
                    double numAgent = Convert.ToDouble(dt.Rows[i]["numAgent"]);
                    double MoenyFee = Convert.ToDouble(dt.Rows[i]["MoenyFee"]);
                    string AccountNumber = dt.Rows[i]["AccountNumber"].ToString();
                    string VarietyID = dt.Rows[i]["VarietyID"].ToString();
                    string StorageDate_para = dt.Rows[i]["StorageDate"].ToString();
                    string sql = string.Format("   SELECT SUM(Count_Trade) FROM dbo.Dep_OperateLog  WHERE BusinessName IN ({0}) AND Dep_AccountNumber='{1}' AND VarietyID='{2}'  AND  DATEDIFF(day,'{3}',dt_Trade)<={4}", buslist, AccountNumber, VarietyID, StorageDate_para, numDay);
                    object obj_zhiqu = SQLHelper.ExecuteScalar(sql);
                    if (obj_zhiqu != null && obj_zhiqu.ToString() != "")
                    {
                        zhiqu = Convert.ToDouble(obj_zhiqu);
                    }
                    shicun = cunru - zhiqu;
                    MoenyFee = shicun * numAgent;
                    dt.Rows[i]["zhiqu"] = Math.Round(zhiqu, 2);
                    dt.Rows[i]["shicun"] = Math.Round(shicun, 2);
                    dt.Rows[i]["MoenyFee"] = Math.Round(MoenyFee, 2);
                }

                string Dep_AN = dt.Rows[i]["AccountNumber"].ToString();
                string Dep_Name = dt.Rows[i]["strName"].ToString();
                string StorageDate = dt.Rows[i]["StorageDate"].ToString();
                string StorageDay = dt.Rows[i]["StorageDay"].ToString();
                string BusinessName = "代理费结算";
                string numWeight_Storage = dt.Rows[i]["CunRu"].ToString();
                string numWeight_ZhiQu = dt.Rows[i]["zhiqu"].ToString();
                string numWeight_Effect = dt.Rows[i]["shicun"].ToString();
                string AgentFee = dt.Rows[i]["numAgent"].ToString();
                string Money_Total = dt.Rows[i]["MoenyFee"].ToString();
                string Money_Reality = Money_Total;
                string Money_Surplus = Money_Total;
                string dt_Trade = DateTime.Now.ToString();
                string Accountant_Name = "";//暂不适用

                StringBuilder strSqlGoodInfo = new StringBuilder();
                strSqlGoodInfo.Append("  SELECT B.strName AS GoodName,C.strName AS UnitName");
                strSqlGoodInfo.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
                strSqlGoodInfo.Append("  INNER JOIN dbo.BD_MeasuringUnit C ON B.MeasuringUnitID=C.ID");
                strSqlGoodInfo.Append("  where A.ID=" + Dep_StorageInfoID);
                DataTable dtGoodInfo = SQLHelper.ExecuteDataTable(strSqlGoodInfo.ToString());
                string GoodName = dtGoodInfo.Rows[0]["GoodName"].ToString();
                string UnitName = dtGoodInfo.Rows[0]["UnitName"].ToString();
                string SpecName = "";

                StringBuilder strSql = new StringBuilder();
                   
                strSql.Append("insert into [SA_AgentFee] (");
                strSql.Append("SA_AN,Dep_StorageInfoID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight_Storage,numWeight_ZhiQu,numWeight_Effect,AgentFee,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,dt_Trade)");
                strSql.Append(" values (");
                strSql.Append("@SA_AN,@Dep_StorageInfoID,@strGUID,@serialNumber,@WBName,@Dep_AN,@Dep_Name,@StorageDate,@StorageDay,@BusinessName,@GoodName,@UnitName,@SpecName,@numWeight_Storage,@numWeight_ZhiQu,@numWeight_Effect,@AgentFee,@Money_Total,@Money_Reality,@Money_Surplus,@Accountant_Name,@dt_Trade)");
                strSql.Append(";select @@IDENTITY");
                SqlParameter[] parameters = {
				new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
				new SqlParameter("@Dep_StorageInfoID", SqlDbType.Int,4),
				new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
				new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
				new SqlParameter("@WBName", SqlDbType.NVarChar,50),
				new SqlParameter("@Dep_AN", SqlDbType.NVarChar,50),
				new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
				new SqlParameter("@StorageDate", SqlDbType.DateTime),
				new SqlParameter("@StorageDay", SqlDbType.Int,4),
				new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
				new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
				new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
				new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
				new SqlParameter("@numWeight_Storage", SqlDbType.Decimal,9),
				new SqlParameter("@numWeight_ZhiQu", SqlDbType.Decimal,9),
				new SqlParameter("@numWeight_Effect", SqlDbType.Decimal,9),
				new SqlParameter("@AgentFee", SqlDbType.Decimal,9),
				new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
				new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
				new SqlParameter("@Money_Surplus", SqlDbType.Decimal,9),
				new SqlParameter("@Accountant_Name", SqlDbType.NVarChar,50),
				new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
                parameters[0].Value = SA_AN;
                parameters[1].Value = Dep_StorageInfoID;
                parameters[2].Value = strGUID;
                parameters[3].Value = serialNumber;
                parameters[4].Value = WBName;
                parameters[5].Value = Dep_AN;
                parameters[6].Value = Dep_Name;
                parameters[7].Value = StorageDate;
                parameters[8].Value = StorageDay;
                parameters[9].Value = BusinessName;
                parameters[10].Value = GoodName;
                parameters[11].Value = UnitName;
                parameters[12].Value = SpecName;
                parameters[13].Value = numWeight_Storage;
                parameters[14].Value = numWeight_ZhiQu;
                parameters[15].Value = numWeight_Effect;
                parameters[16].Value = AgentFee;
                parameters[17].Value = Money_Total;
                parameters[18].Value = Money_Reality;
                parameters[19].Value = Money_Surplus;
                parameters[20].Value = Accountant_Name;
                parameters[21].Value = dt_Trade;


                SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters);

            }
              
            var res2 = new { state = "success", msg = "全部结算成功！" };
            context.Response.Write(JsonHelper.ToJson(res2));

            ////添加事务处理
            //using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            //{
            //    try
            //    {

            //        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString());//出库记录

            //        tran.Commit();

            //        var res = new { state = "success", msg = "全部结算成功！" };
            //        context.Response.Write(JsonHelper.ToJson(res));
            //    }
            //    catch
            //    {
            //        var res = new { state = "error", msg = "数据处理失败！" };
            //        context.Response.Write(JsonHelper.ToJson(res));
            //    }
            //}
          
        }



        /// <summary>
        /// 获取单笔代理费的信息
        /// </summary>
        /// <param name="context"></param>
        void GetExchangeSingle(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT A.ID, B.strName AS WBName,C.AccountNumber,C.strName AS DepName,CONVERT(NVARCHAR(100),A.dt_Exchange,23)  AS dt_Exchange,");
            strSql.Append(" '兑换' AS BusinessName,A.GoodName,A.SpecName,A.UnitName,A.GoodCount,A.GoodPrice,A.VarietyCount,A.VarietyInterest,A.Money_DuiHuan,A.ISReturn");
            strSql.Append("  FROM dbo.GoodExchange A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber =C.AccountNumber");
            strSql.Append("  where A.ID=" + ID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 当前记录是否已经发生代理费结算
        /// </summary>
        /// <param name="context"></param>
        void ISHaveExchange(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();

            strSql.Append(" SELECT COUNT(ID)  FROM dbo.SA_Exchange WHERE GoodExchangeID=" + ID);

            context.Response.Write(SQLHelper.ExecuteScalar(strSql.ToString()).ToString());
        }

        /// <summary>
        /// 添加单笔的结算业务
        /// </summary>
        /// <param name="context"></param>
        void AddSA_Exchange(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string Accountant_Name = context.Request.QueryString["Accountant"].ToString();
            StringBuilder strSqlInfo = new StringBuilder();
            strSqlInfo.Append("  SELECT A.ID, B.strName AS WBName,C.AccountNumber,C.strName AS DepName,CONVERT(NVARCHAR(100),A.dt_Exchange,23)  AS dt_Exchange,");
            strSqlInfo.Append(" '兑换' AS BusinessName,A.GoodName,A.SpecName,A.UnitName,A.GoodCount,A.GoodPrice,A.VarietyCount,A.VarietyInterest,A.Money_DuiHuan,A.ISReturn");
            strSqlInfo.Append("  FROM dbo.GoodExchange A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlInfo.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber =C.AccountNumber");
            strSqlInfo.Append("  where A.ID=" + ID);
            DataTable dtInfo = SQLHelper.ExecuteDataTable(strSqlInfo.ToString());

            string SA_AN = "";//暂时不用
            string GoodExchangeID = ID;
            string strGUID = Fun.getGUID();//获取新的GUID
            //临时暂用的编号
            string serialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0000001";
            string ISReturn = dtInfo.Rows[0]["ISReturn"].ToString();
            string WBName = dtInfo.Rows[0]["WBName"].ToString();
            string Dep_AN = dtInfo.Rows[0]["AccountNumber"].ToString();
            string Dep_Name = dtInfo.Rows[0]["DepName"].ToString();
            string StorageDate = dtInfo.Rows[0]["dt_Exchange"].ToString();
            string StorageDay = DateTime.Now.Subtract(Convert.ToDateTime(StorageDate)).TotalDays.ToString();
            StorageDay = Convert.ToInt32(Math.Round( Convert.ToDouble( StorageDay),0)).ToString();
            string BusinessName = "兑换结算";
            if (ISReturn != "0") {
                BusinessName = "退还兑换结算";
            }
            string GoodName = dtInfo.Rows[0]["GoodName"].ToString();
            string UnitName = dtInfo.Rows[0]["UnitName"].ToString();
            string SpecName = dtInfo.Rows[0]["SpecName"].ToString();
            string numWeight = dtInfo.Rows[0]["GoodCount"].ToString();
            string numPrice = dtInfo.Rows[0]["GoodPrice"].ToString();
            double Money_Interest =Convert.ToDouble( dtInfo.Rows[0]["VarietyInterest"]);
            double Money_Total = Convert.ToDouble( dtInfo.Rows[0]["Money_DuiHuan"]);
            double Money_Reality =Convert.ToDouble(  dtInfo.Rows[0]["Money_DuiHuan"]);
            if (ISReturn != "0")
            {
                Money_Total = -Money_Total;
                Money_Reality = -Money_Reality;
            }
            double Money_Surplus =0;
            string dt_Trade = DateTime.Now.ToString();



            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SA_Exchange] (");
            strSql.Append("SA_AN,GoodExchangeID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,dt_Trade)");
            strSql.Append(" values (");
            strSql.Append("@SA_AN,@GoodExchangeID,@strGUID,@serialNumber,@WBName,@Dep_AN,@Dep_Name,@StorageDate,@StorageDay,@BusinessName,@GoodName,@UnitName,@SpecName,@numWeight,@numPrice,@Money_Interest,@Money_Total,@Money_Reality,@Money_Surplus,@Accountant_Name,@dt_Trade)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodExchangeID", SqlDbType.Int,4),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WBName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDay", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
					new SqlParameter("@numWeight", SqlDbType.Decimal,9),
					new SqlParameter("@numPrice", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Interest", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Surplus", SqlDbType.Decimal,9),
					new SqlParameter("@Accountant_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parameters[0].Value = SA_AN;
            parameters[1].Value = GoodExchangeID;
            parameters[2].Value = strGUID;
            parameters[3].Value = serialNumber;
            parameters[4].Value = WBName;
            parameters[5].Value = Dep_AN;
            parameters[6].Value = Dep_Name;
            parameters[7].Value = StorageDate;
            parameters[8].Value = StorageDay;
            parameters[9].Value = BusinessName;
            parameters[10].Value = GoodName;
            parameters[11].Value = UnitName;
            parameters[12].Value = SpecName;
            parameters[13].Value = numWeight;
            parameters[14].Value = numPrice;
            parameters[15].Value = Money_Interest;
            parameters[16].Value = Money_Total;
            parameters[17].Value = Money_Reality;
            parameters[18].Value = Money_Surplus;
            parameters[19].Value = Accountant_Name;
            parameters[20].Value = dt_Trade;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 添加单笔的结算业务
        /// </summary>
        /// <param name="context"></param>
        void AddSA_ExchangeAll(HttpContext context)
        {
            string WBID = context.Request.QueryString["WBID"].ToString();
            string WBName = common.GetWBInfoByID(Convert.ToInt32(WBID))["strName"].ToString();
            string dtStart = context.Request.QueryString["dtStart"].ToString();
            string dtEnd = context.Request.QueryString["dtEnd"].ToString();



            StringBuilder strSqlSettle = new StringBuilder();
            StringBuilder strSqlInner = new StringBuilder();

            strSqlSettle.Append("  SELECT A.ID, B.strName AS WBName,C.AccountNumber,C.strName AS DepName,CONVERT(NVARCHAR(100),A.dt_Exchange,23)  AS dt_Exchange,");
            strSqlSettle.Append(" '兑换' AS BusinessName,A.GoodName,A.SpecName,A.UnitName,A.GoodCount,A.GoodPrice,A.VarietyCount,A.VarietyInterest,A.Money_DuiHuan,A.ISReturn");

            strSqlInner.Append(" SELECT * FROM dbo.GoodExchange WHERE (");
            strSqlInner.Append(string.Format("    ISReturn IN (SELECT ID FROM dbo.GoodExchange WHERE WBID={0})", WBID));
            strSqlInner.Append(string.Format(" OR dbo.GoodExchange.WBID={0})", WBID));
            DateTime dateEnd = Convert.ToDateTime(dtEnd);
            strSqlInner.Append(string.Format(" AND dt_Exchange> '{0}' ", dtStart));
            strSqlInner.Append(string.Format(" AND dt_Exchange < '{0}' ", dateEnd.AddDays(1).ToString()));


            strSqlSettle.Append("   FROM (" + strSqlInner.ToString() + ") A");
            strSqlSettle.Append("   INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlSettle.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber =C.AccountNumber");        

            DataTable dtInfo = SQLHelper.ExecuteDataTable(strSqlSettle.ToString());
            if (dtInfo == null || dtInfo.Rows.Count == 0)
            {
                context.Response.Write("Error");
                return;
            }
            try
            {
                
                for (int i = 0; i < dtInfo.Rows.Count; i++)
                {

                    string SA_AN = "";//暂时不用
                    string GoodExchangeID = dtInfo.Rows[i]["ID"].ToString();

                    object objExit = SQLHelper.ExecuteScalar(" SELECT COUNT(ID) FROM dbo.SA_Exchange WHERE GoodExchangeID="+GoodExchangeID);
                    if (Convert.ToInt32(objExit) > 0)
                    {
                        continue;//如果已经存在了结算记录，则不在添加
                    }

                    string strGUID = Fun.getGUID();//获取新的GUID
                    //临时暂用的编号
                    string serialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0000001";

                    string ISReturn = dtInfo.Rows[i]["ISReturn"].ToString();
                    string Dep_AN = dtInfo.Rows[i]["AccountNumber"].ToString();
                    string Dep_Name = dtInfo.Rows[i]["DepName"].ToString();
                    string StorageDate = dtInfo.Rows[i]["dt_Exchange"].ToString();
                    string StorageDay = DateTime.Now.Subtract(Convert.ToDateTime(StorageDate)).TotalDays.ToString();
                    StorageDay = Convert.ToInt32(Math.Round(Convert.ToDouble(StorageDay), 0)).ToString();
                    string BusinessName = "兑换结算";
                    if (ISReturn != "0") {
                        BusinessName = "退还兑换结算";
                    }
                    string GoodName = dtInfo.Rows[i]["GoodName"].ToString();
                    string UnitName = dtInfo.Rows[i]["UnitName"].ToString();
                    string SpecName = dtInfo.Rows[i]["SpecName"].ToString();
                    string numWeight = dtInfo.Rows[i]["GoodCount"].ToString();
                    string numPrice = dtInfo.Rows[i]["GoodPrice"].ToString();
                    double Money_Interest = Convert.ToDouble(dtInfo.Rows[i]["VarietyInterest"]);
                    double Money_Total = Convert.ToDouble(dtInfo.Rows[i]["Money_DuiHuan"]);
                    double Money_Reality = Convert.ToDouble(dtInfo.Rows[i]["Money_DuiHuan"]);
                    if (ISReturn != "0")
                    {
                        Money_Total = -Money_Total;
                        Money_Reality = -Money_Reality;
                    }
                    double Money_Surplus = 0;
                    string dt_Trade = DateTime.Now.ToString();
                    string Accountant_Name = "";//暂时不用



                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into [SA_Exchange] (");
                    strSql.Append("SA_AN,GoodExchangeID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,dt_Trade)");
                    strSql.Append(" values (");
                    strSql.Append("@SA_AN,@GoodExchangeID,@strGUID,@serialNumber,@WBName,@Dep_AN,@Dep_Name,@StorageDate,@StorageDay,@BusinessName,@GoodName,@UnitName,@SpecName,@numWeight,@numPrice,@Money_Interest,@Money_Total,@Money_Reality,@Money_Surplus,@Accountant_Name,@dt_Trade)");
                    strSql.Append(";select @@IDENTITY");
                    SqlParameter[] parameters = {
					new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodExchangeID", SqlDbType.Int,4),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WBName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDay", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
					new SqlParameter("@numWeight", SqlDbType.Decimal,9),
					new SqlParameter("@numPrice", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Interest", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Surplus", SqlDbType.Decimal,9),
					new SqlParameter("@Accountant_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
                    parameters[0].Value = SA_AN;
                    parameters[1].Value = GoodExchangeID;
                    parameters[2].Value = strGUID;
                    parameters[3].Value = serialNumber;
                    parameters[4].Value = WBName;
                    parameters[5].Value = Dep_AN;
                    parameters[6].Value = Dep_Name;
                    parameters[7].Value = StorageDate;
                    parameters[8].Value = StorageDay;
                    parameters[9].Value = BusinessName;
                    parameters[10].Value = GoodName;
                    parameters[11].Value = UnitName;
                    parameters[12].Value = SpecName;
                    parameters[13].Value = numWeight;
                    parameters[14].Value = numPrice;
                    parameters[15].Value = Money_Interest;
                    parameters[16].Value = Money_Total;
                    parameters[17].Value = Money_Reality;
                    parameters[18].Value = Money_Surplus;
                    parameters[19].Value = Accountant_Name;
                    parameters[20].Value = dt_Trade;

                    SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters);

                }

                context.Response.Write("OK");
            }
            catch
            {
                context.Response.Write("Error");
            }


        }


        /// <summary>
        /// 获取单笔代理费的信息
        /// </summary>
        /// <param name="context"></param>
        void GetSellSingle(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();//查询参数
            strSql.Append("  SELECT A.ID,B.strName AS WBName,C.AccountNumber,C.strName AS DepName,'存转销结算' AS BusinessName,CONVERT(NVARCHAR(100),A.dt_Sell,23) AS dt_Sell,DATEDIFF(DAY,A.dt_Sell,GETDATE()) AS dt_SellDay,");
            strSql.Append(" A.VarietyName,A.UnitName,A.VarietyCount,A.Price_JieSuan, A.StorageMoney,A.VarietyInterest,A.VarietyMoney,A.Money_Earn,A.ISReturn");
            strSql.Append("  FROM dbo.StorageSell A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber=C.AccountNumber");
            strSql.Append("  where A.ID=" + ID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 当前记录是否已经发生代理费结算
        /// </summary>
        /// <param name="context"></param>
        void ISHaveSell(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();

            strSql.Append(" SELECT COUNT(ID) FROM dbo.SA_Sell WHERE  StorageSellID=" + ID);

            context.Response.Write(SQLHelper.ExecuteScalar(strSql.ToString()).ToString());
        }

        /// <summary>
        /// 添加单笔的结算业务
        /// </summary>
        /// <param name="context"></param>
        void AddSA_Sell(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string Accountant_Name = context.Request.QueryString["Accountant"].ToString();
            StringBuilder strSqlInfo = new StringBuilder();//查询参数
            strSqlInfo.Append("  SELECT A.ID,B.strName AS WBName,C.AccountNumber,C.strName AS DepName,'存转销结算' AS BusinessName,CONVERT(NVARCHAR(100),A.dt_Sell,23) AS dt_Sell,DATEDIFF(DAY,A.dt_Sell,GETDATE()) AS dt_SellDay,");
            strSqlInfo.Append(" A.VarietyName,A.UnitName,A.VarietyCount,A.Price_JieSuan, A.StorageMoney,A.VarietyInterest,A.VarietyMoney,A.Money_Earn,A.ISReturn");
            strSqlInfo.Append("  FROM dbo.StorageSell A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlInfo.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber=C.AccountNumber");
            strSqlInfo.Append("  where A.ID=" + ID);
            DataTable dtInfo = SQLHelper.ExecuteDataTable(strSqlInfo.ToString());

            string SA_AN = "";//暂时不用
            string StorageSellID = ID;
            string strGUID = Fun.getGUID();//获取新的GUID
            //临时暂用的编号
            string serialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0000001";
            string ISReturn = dtInfo.Rows[0]["ISReturn"].ToString();
            string WBName = dtInfo.Rows[0]["WBName"].ToString();
            string Dep_AN = dtInfo.Rows[0]["AccountNumber"].ToString();
            string Dep_Name = dtInfo.Rows[0]["DepName"].ToString();
            string StorageDate = dtInfo.Rows[0]["dt_Sell"].ToString();
            string StorageDay = dtInfo.Rows[0]["dt_SellDay"].ToString();
           
            string BusinessName = "存转销结算";
            if (ISReturn != "0") {
                BusinessName = "退还存转销结算";
            }
            string GoodName = dtInfo.Rows[0]["VarietyName"].ToString();
            string UnitName = dtInfo.Rows[0]["UnitName"].ToString();
            string SpecName = "";
            string numWeight = dtInfo.Rows[0]["VarietyCount"].ToString();
            string numPrice = dtInfo.Rows[0]["Price_JieSuan"].ToString();
            if (numPrice.Trim() == "")
            {
                numPrice = "0";
            }
            double Money_Interest =Convert.ToDouble( dtInfo.Rows[0]["VarietyInterest"]);
            double Money_Storage =Convert.ToDouble(  dtInfo.Rows[0]["StorageMoney"]);
            double Money_Total =Convert.ToDouble(  dtInfo.Rows[0]["Money_Earn"]);
            double Money_Reality =Convert.ToDouble(  dtInfo.Rows[0]["Money_Earn"]);
            if (ISReturn != "0")
            {
                Money_Storage = -Money_Storage;
                Money_Total = -Money_Total;
                Money_Reality = -Money_Reality;
            }
            double Money_Surplus = 0;
            string dt_Trade = DateTime.Now.ToString();




            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SA_Sell] (");
            strSql.Append("SA_AN,StorageSellID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Storage,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,dt_Trade)");
            strSql.Append(" values (");
            strSql.Append("@SA_AN,@StorageSellID,@strGUID,@serialNumber,@WBName,@Dep_AN,@Dep_Name,@StorageDate,@StorageDay,@BusinessName,@GoodName,@UnitName,@SpecName,@numWeight,@numPrice,@Money_Interest,@Money_Storage,@Money_Total,@Money_Reality,@Money_Surplus,@Accountant_Name,@dt_Trade)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageSellID", SqlDbType.Int,4),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WBName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDay", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
					new SqlParameter("@numWeight", SqlDbType.Decimal,9),
					new SqlParameter("@numPrice", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Interest", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Storage", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Surplus", SqlDbType.Decimal,9),
					new SqlParameter("@Accountant_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parameters[0].Value = SA_AN;
            parameters[1].Value = StorageSellID;
            parameters[2].Value = strGUID;
            parameters[3].Value = serialNumber;
            parameters[4].Value = WBName;
            parameters[5].Value = Dep_AN;
            parameters[6].Value = Dep_Name;
            parameters[7].Value = StorageDate;
            parameters[8].Value = StorageDay;
            parameters[9].Value = BusinessName;
            parameters[10].Value = GoodName;
            parameters[11].Value = UnitName;
            parameters[12].Value = SpecName;
            parameters[13].Value = numWeight;
            parameters[14].Value = numPrice;
            parameters[15].Value = Money_Interest;
            parameters[16].Value = Money_Storage;
            parameters[17].Value = Money_Total;
            parameters[18].Value = Money_Reality;
            parameters[19].Value = Money_Surplus;
            parameters[20].Value = Accountant_Name;
            parameters[21].Value = dt_Trade;



            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 添加单笔的结算业务
        /// </summary>
        /// <param name="context"></param>
        void AddSA_SellAll(HttpContext context)
        {
            string WBID = context.Request.QueryString["WBID"].ToString();
            string WBName = common.GetWBInfoByID(Convert.ToInt32(WBID))["strName"].ToString();
            string dtStart = context.Request.QueryString["dtStart"].ToString();
            string dtEnd = context.Request.QueryString["dtEnd"].ToString();



            StringBuilder strSqlSettle = new StringBuilder();

            StringBuilder strSqlInner = new StringBuilder();

            strSqlInner.Append(" SELECT * FROM dbo.StorageSell WHERE (");
            strSqlInner.Append(string.Format("    ISReturn IN (SELECT ID FROM dbo.StorageSell WHERE WBID={0})", WBID));
            strSqlInner.Append(string.Format(" OR dbo.StorageSell.WBID={0})", WBID));
            DateTime dateEnd = Convert.ToDateTime(dtEnd);
            strSqlInner.Append(string.Format(" AND dt_Sell> '{0}' ", dtStart));
            strSqlInner.Append(string.Format(" AND dt_Sell < '{0}' ", dateEnd.AddDays(1).ToString()));

            strSqlSettle.Append("  SELECT A.ID,B.strName AS WBName,C.AccountNumber,C.strName AS DepName,'存转销结算' AS BusinessName,CONVERT(NVARCHAR(100),A.dt_Sell,23) AS dt_Sell,DATEDIFF(DAY,A.dt_Sell,GETDATE()) AS dt_SellDay,");
            strSqlSettle.Append(" A.VarietyName,A.UnitName,A.VarietyCount,A.Price_JieSuan, A.StorageMoney,A.VarietyInterest,A.VarietyMoney,A.Money_Earn,A.ISReturn");

            strSqlSettle.Append("   FROM (" + strSqlInner.ToString() + ") A");
            strSqlSettle.Append("  INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlSettle.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber=C.AccountNumber");
       
            DataTable dtInfo = SQLHelper.ExecuteDataTable(strSqlSettle.ToString());
            if (dtInfo == null || dtInfo.Rows.Count == 0)
            {
                context.Response.Write("Error");
                return;
            }
            try
            {

                for (int i = 0; i < dtInfo.Rows.Count; i++)
                {

                    string SA_AN = "";//暂时不用
                    string StorageSellID = dtInfo.Rows[i]["ID"].ToString();

                    object objExit = SQLHelper.ExecuteScalar(" SELECT COUNT(ID) FROM dbo.SA_Sell WHERE StorageSellID=" + StorageSellID);
                    if (Convert.ToInt32(objExit) > 0)
                    {
                        continue;//如果已经存在了结算记录，则不在添加
                    }
                  
                    
                    string strGUID = Fun.getGUID();//获取新的GUID
                    //临时暂用的编号
                    string serialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0000001";
                    string ISReturn = dtInfo.Rows[i]["ISReturn"].ToString();
                    string Dep_AN = dtInfo.Rows[i]["AccountNumber"].ToString();
                    string Dep_Name = dtInfo.Rows[i]["DepName"].ToString();
                    string StorageDate = dtInfo.Rows[i]["dt_Sell"].ToString();
                    string StorageDay = dtInfo.Rows[i]["dt_SellDay"].ToString();
                    string BusinessName = "存转销结算";
                    if (ISReturn != "0") {
                        BusinessName = "退还存转销结算";
                    }
                    string GoodName = dtInfo.Rows[i]["VarietyName"].ToString();
                    string UnitName = dtInfo.Rows[i]["UnitName"].ToString();
                    string SpecName = "";
                    string numWeight = dtInfo.Rows[i]["VarietyCount"].ToString();
                    string numPrice = dtInfo.Rows[i]["Price_JieSuan"].ToString();
                    if (numPrice.Trim() == "")
                    {
                        numPrice = "0";
                    }
                    double Money_Interest =Convert.ToDouble( dtInfo.Rows[i]["VarietyInterest"]);

                    double Money_Storage =Convert.ToDouble( dtInfo.Rows[i]["StorageMoney"]);
                    double Money_Total = Convert.ToDouble(dtInfo.Rows[i]["Money_Earn"]);
                    double Money_Reality = Convert.ToDouble(dtInfo.Rows[i]["Money_Earn"]);
                    if (ISReturn != "0")
                    {
                        Money_Storage = -Money_Storage;
                        Money_Total = -Money_Total;
                        Money_Reality = -Money_Reality;
                    }
                    double Money_Surplus = 0;
                    string dt_Trade = DateTime.Now.ToString();
                    string Accountant_Name = "";




                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into [SA_Sell] (");
                    strSql.Append("SA_AN,StorageSellID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Storage,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,dt_Trade)");
                    strSql.Append(" values (");
                    strSql.Append("@SA_AN,@StorageSellID,@strGUID,@serialNumber,@WBName,@Dep_AN,@Dep_Name,@StorageDate,@StorageDay,@BusinessName,@GoodName,@UnitName,@SpecName,@numWeight,@numPrice,@Money_Interest,@Money_Storage,@Money_Total,@Money_Reality,@Money_Surplus,@Accountant_Name,@dt_Trade)");
                    strSql.Append(";select @@IDENTITY");
                    SqlParameter[] parameters = {
					new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageSellID", SqlDbType.Int,4),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WBName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDay", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
					new SqlParameter("@numWeight", SqlDbType.Decimal,9),
					new SqlParameter("@numPrice", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Interest", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Storage", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Surplus", SqlDbType.Decimal,9),
					new SqlParameter("@Accountant_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
                    parameters[0].Value = SA_AN;
                    parameters[1].Value = StorageSellID;
                    parameters[2].Value = strGUID;
                    parameters[3].Value = serialNumber;
                    parameters[4].Value = WBName;
                    parameters[5].Value = Dep_AN;
                    parameters[6].Value = Dep_Name;
                    parameters[7].Value = StorageDate;
                    parameters[8].Value = StorageDay;
                    parameters[9].Value = BusinessName;
                    parameters[10].Value = GoodName;
                    parameters[11].Value = UnitName;
                    parameters[12].Value = SpecName;
                    parameters[13].Value = numWeight;
                    parameters[14].Value = numPrice;
                    parameters[15].Value = Money_Interest;
                    parameters[16].Value = Money_Storage;
                    parameters[17].Value = Money_Total;
                    parameters[18].Value = Money_Reality;
                    parameters[19].Value = Money_Surplus;
                    parameters[20].Value = Accountant_Name;
                    parameters[21].Value = dt_Trade;
                    SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters);

                }

                context.Response.Write("OK");
            }
            catch
            {
                context.Response.Write("Error");
            }


        }



        /// <summary>
        /// 获取单笔代理费的信息
        /// </summary>
        /// <param name="context"></param>
        void GetShoppingSingle(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();//查询参数
            strSql.Append("  SELECT A.ID,B.strName AS WBName,C.AccountNumber,C.strName AS DepName,'换购结算' AS BusinessName,CONVERT(NVARCHAR(100),A.dt_Sell,23) AS dt_Sell,DATEDIFF(DAY,A.dt_Sell,GETDATE()) AS dt_SellDay,");
            strSql.Append(" A.VarietyName,A.UnitName,A.VarietyCount,A.Price_JieSuan, A.StorageMoney,A.VarietyInterest,A.VarietyMoney,A.ISReturn");
            strSql.Append("  FROM dbo.StorageShopping A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber=C.AccountNumber");
            strSql.Append("  where A.ID=" + ID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 当前记录是否已经发生代理费结算
        /// </summary>
        /// <param name="context"></param>
        void ISHaveShopping(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();

            strSql.Append(" SELECT COUNT(ID) FROM dbo.SA_Shopping WHERE  StorageShoppingID=" + ID);

            context.Response.Write(SQLHelper.ExecuteScalar(strSql.ToString()).ToString());
        }

        /// <summary>
        /// 添加单笔的结算业务
        /// </summary>
        /// <param name="context"></param>
        void AddSA_Shopping(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string Accountant_Name = context.Request.QueryString["Accountant"].ToString();
            StringBuilder strSqlInfo = new StringBuilder();//查询参数
            strSqlInfo.Append("  SELECT A.ID,B.strName AS WBName,C.AccountNumber,C.strName AS DepName,'存转销结算' AS BusinessName,CONVERT(NVARCHAR(100),A.dt_Sell,23) AS dt_Sell,DATEDIFF(DAY,A.dt_Sell,GETDATE()) AS dt_SellDay,");
            strSqlInfo.Append(" A.VarietyName,A.UnitName,A.VarietyCount,A.Price_JieSuan, A.StorageMoney,A.VarietyInterest,A.VarietyMoney,A.ISReturn");
            strSqlInfo.Append("  FROM dbo.StorageShopping A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlInfo.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber=C.AccountNumber");
            strSqlInfo.Append("  where A.ID=" + ID);
            DataTable dtInfo = SQLHelper.ExecuteDataTable(strSqlInfo.ToString());

            string SA_AN = "";//暂时不用
            string StorageShoppingID = ID;
            string strGUID = Fun.getGUID();//获取新的GUID
            //临时暂用的编号
            string serialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0000001";
            string ISReturn = dtInfo.Rows[0]["ISReturn"].ToString();
            string WBName = dtInfo.Rows[0]["WBName"].ToString();
            string Dep_AN = dtInfo.Rows[0]["AccountNumber"].ToString();
            string Dep_Name = dtInfo.Rows[0]["DepName"].ToString();
            string StorageDate = dtInfo.Rows[0]["dt_Sell"].ToString();
            string StorageDay = dtInfo.Rows[0]["dt_SellDay"].ToString();
            string BusinessName = "换购结算";
            if (ISReturn != "0") {
                BusinessName = "退还换购结算";
            }
            string GoodName = dtInfo.Rows[0]["VarietyName"].ToString();
            string UnitName = dtInfo.Rows[0]["UnitName"].ToString();
            string SpecName = "";
            string numWeight = dtInfo.Rows[0]["VarietyCount"].ToString();
            string numPrice = dtInfo.Rows[0]["Price_JieSuan"].ToString();
            if (numPrice.Trim() == "")
            {
                numPrice = "0";
            }
            double Money_Interest = Convert.ToDouble( dtInfo.Rows[0]["VarietyInterest"]);
            double Money_Storage =Convert.ToDouble( dtInfo.Rows[0]["StorageMoney"]);

            double Money_Total =Convert.ToDouble( dtInfo.Rows[0]["VarietyMoney"]);
            double Money_Reality =Convert.ToDouble( dtInfo.Rows[0]["VarietyMoney"]);
            if (ISReturn != "0")
            {
                Money_Storage = -Money_Storage;
                Money_Total = -Money_Total;
                Money_Reality = -Money_Reality;
            }
            double Money_Surplus = 0;
            string dt_Trade = DateTime.Now.ToString();




            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SA_Shopping] (");
            strSql.Append("SA_AN,StorageShoppingID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Storage,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,dt_Trade)");
            strSql.Append(" values (");
            strSql.Append("@SA_AN,@StorageShoppingID,@strGUID,@serialNumber,@WBName,@Dep_AN,@Dep_Name,@StorageDate,@StorageDay,@BusinessName,@GoodName,@UnitName,@SpecName,@numWeight,@numPrice,@Money_Interest,@Money_Storage,@Money_Total,@Money_Reality,@Money_Surplus,@Accountant_Name,@dt_Trade)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageShoppingID", SqlDbType.Int,4),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WBName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDay", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
					new SqlParameter("@numWeight", SqlDbType.Decimal,9),
					new SqlParameter("@numPrice", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Interest", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Storage", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Surplus", SqlDbType.Decimal,9),
					new SqlParameter("@Accountant_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parameters[0].Value = SA_AN;
            parameters[1].Value = StorageShoppingID;
            parameters[2].Value = strGUID;
            parameters[3].Value = serialNumber;
            parameters[4].Value = WBName;
            parameters[5].Value = Dep_AN;
            parameters[6].Value = Dep_Name;
            parameters[7].Value = StorageDate;
            parameters[8].Value = StorageDay;
            parameters[9].Value = BusinessName;
            parameters[10].Value = GoodName;
            parameters[11].Value = UnitName;
            parameters[12].Value = SpecName;
            parameters[13].Value = numWeight;
            parameters[14].Value = numPrice;
            parameters[15].Value = Money_Interest;
            parameters[16].Value = Money_Storage;
            parameters[17].Value = Money_Total;
            parameters[18].Value = Money_Reality;
            parameters[19].Value = Money_Surplus;
            parameters[20].Value = Accountant_Name;
            parameters[21].Value = dt_Trade;



            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 添加单笔的结算业务
        /// </summary>
        /// <param name="context"></param>
        void AddSA_ShoppingAll(HttpContext context)
        {
            string WBID = context.Request.QueryString["WBID"].ToString();
            string WBName = common.GetWBInfoByID(Convert.ToInt32(WBID))["strName"].ToString();
            string dtStart = context.Request.QueryString["dtStart"].ToString();
            string dtEnd = context.Request.QueryString["dtEnd"].ToString();



            StringBuilder strSqlSettle = new StringBuilder();
            StringBuilder strSqlInner = new StringBuilder();

            strSqlInner.Append(" SELECT * FROM dbo.StorageShopping WHERE (");
            strSqlInner.Append(string.Format("    ISReturn IN (SELECT ID FROM dbo.StorageShopping WHERE WBID={0})", WBID));
            strSqlInner.Append(string.Format(" OR dbo.StorageShopping.WBID={0})", WBID));
            DateTime dateEnd = Convert.ToDateTime(dtEnd);
            strSqlInner.Append(string.Format(" AND dt_Sell> '{0}' ", dtStart));
            strSqlInner.Append(string.Format(" AND dt_Sell < '{0}' ", dateEnd.AddDays(1).ToString()));

            strSqlSettle.Append("  SELECT A.ID,B.strName AS WBName,C.AccountNumber,C.strName AS DepName,'换购结算' AS BusinessName,CONVERT(NVARCHAR(100),A.dt_Sell,23) AS dt_Sell,DATEDIFF(DAY,A.dt_Sell,GETDATE()) AS dt_SellDay,");
            strSqlSettle.Append(" A.VarietyName,A.UnitName,A.VarietyCount,A.Price_JieSuan, A.StorageMoney,A.VarietyInterest,A.VarietyMoney,A.ISReturn");

            strSqlSettle.Append("   FROM (" + strSqlInner.ToString() + ") A");
            strSqlSettle.Append("  INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlSettle.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber=C.AccountNumber");

            DataTable dtInfo = SQLHelper.ExecuteDataTable(strSqlSettle.ToString());
            if (dtInfo == null || dtInfo.Rows.Count == 0)
            {
                context.Response.Write("Error");
                return;
            }
            try
            {

                for (int i = 0; i < dtInfo.Rows.Count; i++)
                {

                    string SA_AN = "";//暂时不用
                    string StorageShoppingID = dtInfo.Rows[i]["ID"].ToString();

                    object objExit = SQLHelper.ExecuteScalar(" SELECT COUNT(ID) FROM dbo.SA_Shopping WHERE StorageShoppingID=" + StorageShoppingID);
                    if (Convert.ToInt32(objExit) > 0)
                    {
                        continue;//如果已经存在了结算记录，则不在添加
                    }


                    string strGUID = Fun.getGUID();//获取新的GUID
                    //临时暂用的编号
                    string serialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "0000001";

                    string ISReturn = dtInfo.Rows[i]["ISReturn"].ToString();
                    string Dep_AN = dtInfo.Rows[i]["AccountNumber"].ToString();
                    string Dep_Name = dtInfo.Rows[i]["DepName"].ToString();
                    string StorageDate = dtInfo.Rows[i]["dt_Sell"].ToString();
                    string StorageDay = dtInfo.Rows[i]["dt_SellDay"].ToString();
                    string BusinessName = "存转销结算";
                    string GoodName = dtInfo.Rows[i]["VarietyName"].ToString();
                    string UnitName = dtInfo.Rows[i]["UnitName"].ToString();
                    string SpecName = "";
                    string numWeight = dtInfo.Rows[i]["VarietyCount"].ToString();
                    string numPrice = dtInfo.Rows[i]["Price_JieSuan"].ToString();
                    if (numPrice.Trim() == "")
                    {
                        numPrice = "0";
                    }
                    double Money_Interest = Convert.ToDouble(dtInfo.Rows[i]["VarietyInterest"]);
                    double Money_Storage = Convert.ToDouble(dtInfo.Rows[i]["StorageMoney"]);

                    double Money_Total = Convert.ToDouble(dtInfo.Rows[i]["VarietyMoney"]);
                    double Money_Reality = Convert.ToDouble(dtInfo.Rows[i]["VarietyMoney"]);
                    if (ISReturn != "0")
                    {
                        Money_Storage = -Money_Storage;
                        Money_Total = -Money_Total;
                        Money_Reality = -Money_Reality;
                    }
                    double Money_Surplus = 0;
                    string dt_Trade = DateTime.Now.ToString();
                    string Accountant_Name = "";




                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into [SA_Shopping] (");
                    strSql.Append("SA_AN,StorageShoppingID,strGUID,serialNumber,WBName,Dep_AN,Dep_Name,StorageDate,StorageDay,BusinessName,GoodName,UnitName,SpecName,numWeight,numPrice,Money_Interest,Money_Storage,Money_Total,Money_Reality,Money_Surplus,Accountant_Name,dt_Trade)");
                    strSql.Append(" values (");
                    strSql.Append("@SA_AN,@StorageShoppingID,@strGUID,@serialNumber,@WBName,@Dep_AN,@Dep_Name,@StorageDate,@StorageDay,@BusinessName,@GoodName,@UnitName,@SpecName,@numWeight,@numPrice,@Money_Interest,@Money_Storage,@Money_Total,@Money_Reality,@Money_Surplus,@Accountant_Name,@dt_Trade)");
                    strSql.Append(";select @@IDENTITY");
                    SqlParameter[] parameters = {
					new SqlParameter("@SA_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageShoppingID", SqlDbType.Int,4),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@serialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WBName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_AN", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDay", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
					new SqlParameter("@numWeight", SqlDbType.Decimal,9),
					new SqlParameter("@numPrice", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Interest", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Storage", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Surplus", SqlDbType.Decimal,9),
					new SqlParameter("@Accountant_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
                    parameters[0].Value = SA_AN;
                    parameters[1].Value = StorageShoppingID;
                    parameters[2].Value = strGUID;
                    parameters[3].Value = serialNumber;
                    parameters[4].Value = WBName;
                    parameters[5].Value = Dep_AN;
                    parameters[6].Value = Dep_Name;
                    parameters[7].Value = StorageDate;
                    parameters[8].Value = StorageDay;
                    parameters[9].Value = BusinessName;
                    parameters[10].Value = GoodName;
                    parameters[11].Value = UnitName;
                    parameters[12].Value = SpecName;
                    parameters[13].Value = numWeight;
                    parameters[14].Value = numPrice;
                    parameters[15].Value = Money_Interest;
                    parameters[16].Value = Money_Storage;
                    parameters[17].Value = Money_Total;
                    parameters[18].Value = Money_Reality;
                    parameters[19].Value = Money_Surplus;
                    parameters[20].Value = Accountant_Name;
                    parameters[21].Value = dt_Trade;
                    SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters);

                }

                context.Response.Write("OK");
            }
            catch
            {
                context.Response.Write("Error");
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}