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
    /// storage 的摘要说明
    /// </summary>
    public class storage : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "GetGoodStoreByGoodID": GetGoodStoreByGoodID(context); break;
                    case "GetExchangeGoodStore": GetExchangeGoodStore(context); break;
                    case "PrintDep_StorageInfo": PrintDep_StorageInfo(context); break;//储户的存折内容打印
                    case "PrintDep_OperateLog": PrintDep_OperateLog(context); break;//储户的存折内容打印
                    case "PrintDep_OperateLogList": PrintDep_OperateLogList(context); break;//储户的存折内容打印

                    case "PrintStorageSell": PrintStorageSell(context); break;//打印存转销凭据
                    case "PrintStorageShopping": PrintStorageShopping(context); break;//打印产品换购凭据
                        
                    case "PrintGoodExchangeList": PrintGoodExchangeList(context); break;//打印兑换凭据
                    case "PrintGoodExchange": PrintGoodExchange(context); break;//打印兑换凭据
                    case "PrintGoodExchangeGroup": PrintGoodExchangeGroup(context); break;//打印分时批量兑换凭据

                    case "updatePrintTime": updatePrintTime(context); break;//更新小票打印次数
                    case "updatePrintTime_List": updatePrintTime_List(context); break;//更新小票打印次数
                    case "getPrintTime": getPrintTime(context); break;//获取小票打印次数

                        
                }
            }

        }


        /// <summary>
        /// 储户的存折内容打印(打印单条存折的记录)
        /// </summary>
        /// <param name="context"></param>
        void PrintDep_OperateLog(HttpContext context)
        {
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append(" select A.ID,  A.BusinessName");
            strSqlLog.Append("  , B.strName AS  WBID, VarietyID,UnitID,VarietyName,UnitName,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,CONVERT(NVARCHAR(100),dt_Trade,23) AS  dt_Trade");
            strSqlLog.Append("  FROM dbo.Dep_OperateLog A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string numBusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string BusinessName = "";
            switch (numBusinessName) {
                case "1": BusinessName = "存入"; break;
                case "2": BusinessName = "兑换"; break;
                case "3": BusinessName = "存转销"; break;
                case "4": BusinessName = "提取原粮"; break;
                case "5": BusinessName = "修改"; break;
                case "6": BusinessName = "退换兑换"; break;
                case "7": BusinessName = "退换存转销"; break;
                case "8": BusinessName = "退换存粮"; break;
                case "9": BusinessName = "产品换购"; break;
                case "10": BusinessName = "退还换购"; break;
                case "11": BusinessName = "结息"; break;
                case "12": BusinessName = "换存折"; break;
                case "13": BusinessName = "商品销售"; break;
                case "14": BusinessName = "商品销售退还"; break;
                case "15": BusinessName = "积分兑换"; break;
                case "16": BusinessName = "存粮转存"; break;

            }
            string WBID = dtLog.Rows[0]["WBID"].ToString();
            //string VarietyID = dtLog.Rows[0]["VarietyID"].ToString();            
            //string UnitID = dtLog.Rows[0]["UnitID"].ToString();
            string VarietyID = dtLog.Rows[0]["VarietyName"].ToString();
            string UnitID = dtLog.Rows[0]["UnitName"].ToString();
            string Price = dtLog.Rows[0]["Price"].ToString();
            string GoodCount = dtLog.Rows[0]["GoodCount"].ToString();
            string Count_Trade = dtLog.Rows[0]["Count_Trade"].ToString();
            string Money_Trade = dtLog.Rows[0]["Money_Trade"].ToString();
            string Count_Balance = dtLog.Rows[0]["Count_Balance"].ToString();
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();
            string[] dt_TradeArray = dt_Trade.Split('-');
            dt_Trade = dt_TradeArray[0] + dt_TradeArray[1] + dt_TradeArray[2];

            string strWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT  ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += "  where 1=1 and WBID=" + strWBID;
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
                strReturn.Append("   <td style='width:" + (RecordC2X - RecordC1X).ToString() + "px;'>" + dt_Trade + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>" + BusinessName + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>" + VarietyID + " </td>");
                //strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>" + Price + " </td>");
                //strReturn.Append("   <td style='width:" + (RecordC6X - RecordC5X).ToString() + "px;'>" + UnitID + " </td>");

                strReturn.Append("   <td style='width:" + (RecordC6X - RecordC4X).ToString() + "px;'>" + Price + "元/" + UnitID + " </td>");

                strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>" + GoodCount + "</td>");
                strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>" + Count_Trade + "</td>");
                strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>" + Count_Balance + "kg</td>");
                strReturn.Append("   <td >" + WBID + " </td>");

                strReturn.Append("   </tr></table>");



                context.Response.Write(strReturn.ToString());
            }
        }


        /// <summary>
        /// 打印储户存粮凭证
        /// </summary>
        /// <param name="context"></param>
        void PrintDep_StorageInfo(HttpContext context)
        {
            string OperateType = context.Request.Form["OperateType"].ToString();//存粮、结息续存 
            string BusinessNO = context.Request.Form["BusinessNO"].ToString(); 
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();

            StringBuilder sql = new StringBuilder();
            sql.Append("   SELECT W.strName AS WBName,U.strRealName AS UserName,D.strName AS DepName,CONVERT(varchar(100), A.dt_Trade, 23)AS dt_Trade,");
            sql.Append("   A.VarietyName,A.UnitName,A.Price,A.Count_Trade,A.Count_Balance,A.BusinessName");
            sql.Append("   FROM dbo.Dep_OperateLog A LEFT OUTER JOIN dbo.WB W ON A.WBID=W.ID ");
            sql.Append("   LEFT OUTER JOIN dbo.Users U ON A.UserID=U.ID");
            sql.Append("   LEFT OUTER JOIN dbo.Depositor D ON A.Dep_AccountNumber=D.AccountNumber");
            sql.Append(string.Format("   WHERE Dep_AccountNumber='{0}' AND BusinessNO='{1}'", AccountNumber, BusinessNO));
          //  sql.Append(" where BusinessNO='" + BusinessNO + "' and  A.AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(sql.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }

            //共有参数
            string DepName = dtLog.Rows[0]["DepName"].ToString();
            string VarietyName = dtLog.Rows[0]["VarietyName"].ToString();
            string UnitName = dtLog.Rows[0]["UnitName"].ToString();
            string StorageDate = dtLog.Rows[0]["dt_Trade"].ToString();
            string Price_ShiChang = dtLog.Rows[0]["Price"].ToString();
            string StorageNumberRaw = dtLog.Rows[0]["Count_Trade"].ToString();
            string Count_Balance = dtLog.Rows[0]["Count_Balance"].ToString();
           string WBName = dtLog.Rows[0]["WBName"].ToString();
            string UserName = dtLog.Rows[0]["UserName"].ToString();
            double numMoney = Convert.ToDouble(Price_ShiChang) * Convert.ToDouble(StorageNumberRaw);
            numMoney = Math.Round(numMoney, 2);
           
            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");

            strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户" + OperateType + "凭证</span></td> </tr>");
            strReturn.Append("  </table>");

            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 200px;'>  <span >姓名：" + DepName + "</span> </td>");
            strReturn.Append("    <td style='width: 200px;'>  <span >账号：" + AccountNumber + "</span> </td>");
            strReturn.Append("    <td style='width: 240px;'>  <span >网点：" + WBName + "</span> </td>");
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
            strReturn.Append("    <td style='width: 80px;'> <span>存粮种类</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>单位</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>存入价</span></td>");      
            strReturn.Append("    <td style='width: 80px;'> <span>本次存粮</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>价值</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>共存粮</span></td>");
            
            
            strReturn.Append("    <td style='width: 80px;'> <span>时间</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>"+OperateType+"</span></td>");
            strReturn.Append("    <td> <span>" + VarietyName+ "</span></td>");
            strReturn.Append("    <td> <span>" + UnitName + "</span></td>");
            strReturn.Append("    <td> <span>" + Price_ShiChang + "</span></td>");
            strReturn.Append("    <td> <span>" + StorageNumberRaw + "</span></td>");

            strReturn.Append("    <td> <span>￥" + numMoney + "</span></td>");
            strReturn.Append("    <td> <span>" + Count_Balance + "</span></td>");
            strReturn.Append("    <td> <span>" + StorageDate + "</span></td>");
            strReturn.Append("  </tr>");
            strReturn.Append("   </table>");
            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width: 200px;'> <span>营业员：</span> <span>" + UserName + "</span>  </td>");
            strReturn.Append("  <td>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");


            context.Response.Write(strReturn.ToString());
        }

        /// <summary>
        /// 储户的存折内容打印(打印多条存折的记录)
        /// </summary>
        /// <param name="context"></param>
        void PrintDep_OperateLogList(HttpContext context)
        {
            string BNList = "";
            if (context.Request.QueryString["Surplus"] != null)
            {
                BNList = context.Session["BNListSurPlue"].ToString();
            }
            else
            {
                BNList = context.Session["BNList"].ToString();
            }
            string BNListSurPlue = "";//剩余的需要打印的编号集合
            string[] BNArray = BNList.Split('|');//需要打印的编号集合

            string BusinessNO = BNArray[0];//首个编号
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            string strReturnMsg = "";


            string strWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT  ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += "  where 1=1 and WBID=" + strWBID;
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
                for (int i = 0; i < BNArray.Length; i++)
                {

                    BusinessNO = BNArray[i];
                    StringBuilder strSqlLog = new StringBuilder();
                    strSqlLog.Append(" select A.ID, (CASE  A.BusinessName WHEN '1' THEN '存入' WHEN '2' THEN '兑换' WHEN '3' THEN '存转销'  WHEN '4' THEN '提取' WHEN '5' THEN '修改' WHEN '6' THEN '退还兑换' WHEN '7' THEN '退还存转销' WHEN '8' THEN '退还存粮' WHEN '9' THEN '产品换购' WHEN '10' THEN '退还换购' WHEN '11' THEN '结息' WHEN '12' THEN '换存折' END ) AS BusinessName ");
                    strSqlLog.Append("  , B.strName AS  WBID, VarietyID,UnitID,VarietyName,UnitName,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,CONVERT(NVARCHAR(100),dt_Trade,23) AS  dt_Trade");
                    strSqlLog.Append("  FROM dbo.Dep_OperateLog A INNER JOIN dbo.WB B ON A.WBID=B.ID");
                    strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

                    DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
                    if (dtLog == null || dtLog.Rows.Count == 0)
                    {
                        context.Response.Write("");
                        return;
                    }
                    string WBID = dtLog.Rows[0]["WBID"].ToString();
                    //string VarietyID = dtLog.Rows[0]["VarietyID"].ToString();                  
                    //string UnitID = dtLog.Rows[0]["UnitID"].ToString();
                    string VarietyID = dtLog.Rows[0]["VarietyName"].ToString();
                    string UnitID = dtLog.Rows[0]["UnitName"].ToString();
                    string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
                    string Price = dtLog.Rows[0]["Price"].ToString();
                    string Count_Trade = dtLog.Rows[0]["Count_Trade"].ToString();
                    string Money_Trade = dtLog.Rows[0]["Money_Trade"].ToString();
                    string Count_Balance = dtLog.Rows[0]["Count_Balance"].ToString();
                    string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();
                    string[] dt_TradeArray = dt_Trade.Split('-');
                    dt_Trade = dt_TradeArray[0] + dt_TradeArray[1] + dt_TradeArray[2];



                    if (i == 0)//首行设置于存折最上方的间距
                    {
                        strReturn.Append("  <table style='width:100%; height:" + RecordRY + "px'><tr><td></td> </tr></table>");
                    }
                    strReturn.Append("   <table style='margin-left:" + RecordC1X + "px; font-size:" + FontSize + "px;'><tr>");
                    strReturn.Append("   <td style='width:" + (RecordC2X - RecordC1X).ToString() + "px;height:25px;'>" + dt_Trade + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>" + BusinessName + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>" + VarietyID + " </td>");
                    //strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>" + Price + " </td>");
                    //strReturn.Append("   <td style='width:" + (RecordC6X - RecordC5X).ToString() + "px;'>" + UnitID + " </td>");

                    strReturn.Append("   <td style='width:" + (RecordC6X - RecordC4X).ToString() + "px;'>" + Price + "元/" + UnitID + " </td>");
                    double goodCount =Convert.ToDouble( Money_Trade) / Convert.ToDouble( Price);
                    strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>" + Math.Round(goodCount,2).ToString() + "</td>");
                    strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>" + Count_Trade + "</td>");
                    strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>" + Count_Balance + "</td>");
                    strReturn.Append("   <td >" + WBID + " </td>");

                    strReturn.Append("   </tr></table>");
                    numIndex += 1;//增加连续打印序列
                    if (numIndex > 20)//如果是连续打印，并且超出了本页的范围
                    {
                        if (i < BNArray.Length - 1)//此条不是最后一个被打印的数据
                        {
                            for (int j = i+1; j < BNArray.Length; j++)
                            {
                                if (j == i+1)
                                {
                                    BNListSurPlue = BNArray[j];
                                }
                                else
                                {
                                    BNListSurPlue = BNListSurPlue + "|" + BNArray[j];
                                }
                            }
                            context.Session["BNListSurPlue"] = BNListSurPlue;//缓存剩余打印项
                            strReturnMsg = "{\"SurPlus\":\"" + BNListSurPlue + "\",\"Msg\":\"" + strReturn.ToString() + "\"}";

                            context.Response.Write(strReturnMsg);
                            return;
                        }
                    }

                  
                }



                strReturnMsg = "{\"SurPlus\":\"" + BNListSurPlue + "\",\"Msg\":\"" + strReturn.ToString() + "\"}";

                context.Response.Write(strReturnMsg);
            }
        }


        /// <summary>
        /// 打印存转销凭证
        /// </summary>
        /// <param name="context"></param>
        void PrintStorageSell(HttpContext context)
        {
            string model = "";
            if (context.Request.QueryString["model"] != null)
            {
                model = context.Request.QueryString["model"].ToString();
            }

            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            int printTime = common.getPrintTime(AccountNumber, BusinessNO);
            int printTimeUser = Convert.ToInt32(common.GetUserInfoByID(Convert.ToInt32(context.Session["ID"]))["numPrint"]);
            if (printTime > printTimeUser)
            {
                var res = new { state = "false", msg = "该小票最多打印" + printTimeUser + "次，您已经打印" + (printTime - 1).ToString() + "次,不能继续打印!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string printValue = "<span style='font-size: 12px; font-weight: bolder;'>(首次打印)</span>";
            if (printTime > 1)
            {
                printValue = "<span style='font-size: 12px;'>(第" + printTime + "次打印)</span>";
            }

            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,UnitName,VarietyID,VarietyName,VarietyCount,VarietyMoney,VarietyInterest,StorageDate,CurrentRate,EarningRate,StorageFee,StorageMoney,Price_JieSuan,Money_Earn,dt_Sell,JieCun_Last,JieCun_Now,ISReturn ");
            strSqlLog.Append("  FROM dbo.StorageSell");
            strSqlLog.Append("  ");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                var res = new { state = "false", msg = "未查询到需要打印的信息！" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
         
            string SerialNumber = dtLog.Rows[0]["SerialNumber"].ToString();
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();
            string Dep_AccountNumber = dtLog.Rows[0]["Dep_AccountNumber"].ToString();
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();
            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string UserID = dtLog.Rows[0]["UserID"].ToString();
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string UnitName = dtLog.Rows[0]["UnitName"].ToString();
            string VarietyID = dtLog.Rows[0]["VarietyID"].ToString();
            string VarietyName = dtLog.Rows[0]["VarietyName"].ToString();
            string VarietyCount = dtLog.Rows[0]["VarietyCount"].ToString();
            string VarietyMoney = dtLog.Rows[0]["VarietyMoney"].ToString();
            string VarietyInterest = dtLog.Rows[0]["VarietyInterest"].ToString();
            string StorageDate = dtLog.Rows[0]["StorageDate"].ToString();
            string CurrentRate = dtLog.Rows[0]["CurrentRate"].ToString();
            string EarningRate = dtLog.Rows[0]["EarningRate"].ToString();
            EarningRate =(Math.Round( Convert.ToDecimal( EarningRate) * 100,2)).ToString()+"%";
            string StorageFee = dtLog.Rows[0]["StorageFee"].ToString();
            StorageFee = (Math.Round(Convert.ToDecimal(StorageFee), 2)).ToString() + "%";
            string StorageMoney = dtLog.Rows[0]["StorageMoney"].ToString();
            string Price_JieSuan = dtLog.Rows[0]["Price_JieSuan"].ToString();
            string Money_Earn = dtLog.Rows[0]["Money_Earn"].ToString();
            string dt_Sell = dtLog.Rows[0]["dt_Sell"].ToString();
            string JieCun_Last = dtLog.Rows[0]["JieCun_Last"].ToString();
            string JieCun_Now = dtLog.Rows[0]["JieCun_Now"].ToString();

              #region 返回信息
            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            if (model == "")
            {
                strReturn.Append("   <tr><td align='center' style='text-align: center;'><span style='font-size: 18px; font-weight: bolder;'>" + CompanyName + "  储户存转销凭证</span>"+printValue+"</td> </tr>");
            }
            else {
               // strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户退还存转销凭证</span></td> </tr>");
                strReturn.Append("   <tr><td align='center' style='text-align: center;'><span style='font-size: 18px; font-weight: bolder;'>" + CompanyName + "  储户退还存转销凭证</span>" + printValue + "</td> </tr>");
           
            }
            strReturn.Append("   <tr><td align='center' style='font-size: 12px; text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + SerialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");

            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 160px;'>  <span >姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >账号：" + AccountNumber + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >存储产品：" + VarietyName + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >上期结存：" + JieCun_Last + "</span> </td>");
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
            strReturn.Append("    <td style='width: 70px;'> <span>保值</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>数量</span></td>");
            strReturn.Append("    <td align='center' style='width: 110px;'> <span>" + VarietyCount + "</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>结算价</span></td>");
            strReturn.Append("    <td align='center' style='width: 110px;'> <span>" + Price_JieSuan + "</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>结算金额</span></td>");
            strReturn.Append("    <td align='center' style='width: 120px;'> <span>￥" + VarietyMoney + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>利息</span></td>");
            strReturn.Append("    <td > <span>实存天数</span></td>");
            strReturn.Append("    <td align='center' > <span>" + StorageDate + "</span></td>");
            strReturn.Append("    <td > <span>利率</span></td>");
            strReturn.Append("    <td align='center' > <span>" + EarningRate +"</span></td>");
            strReturn.Append("    <td > <span>利息金额</span></td>");
            strReturn.Append("    <td align='center'> <span>￥" + VarietyInterest + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>保管费</span></td>");
            strReturn.Append("    <td > <span>实存天数</span></td>");
            strReturn.Append("    <td align='center' > <span>" + StorageDate + "</span></td>");
            strReturn.Append("    <td > <span>费率</span></td>");
            strReturn.Append("    <td align='center' > <span>" + StorageFee + "</span></td>");
            strReturn.Append("    <td > <span>保管费金额</span></td>");
            strReturn.Append("    <td align='center'> <span>￥" + StorageMoney + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>总收益</span></td>");
            strReturn.Append("     <td colspan='5' > <span></span></td>");
            strReturn.Append("    <td align='center' > ￥<span>" + Money_Earn + "</span></td>");
            strReturn.Append("  </tr>");
            string strDaXie = Fun.ChangeToRMB(Money_Earn);
            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>大写收益</span></td>");
            strReturn.Append("     <td colspan='6' align='left'> <span>" + strDaXie + "</span></td>");
            strReturn.Append("  </tr>");
            strReturn.Append("   </table>");

            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width: 80px;'> <span>折合"+VarietyName+"：</span></td>  <td style='width: 170px;'>  <span>" + VarietyCount + "</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>计量单位：</span></td>  <td style='width: 110px;'>  <span>元、" + UnitName + "</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>"+VarietyName+"结存：</span></td>  <td style='width: 140px;'>  <span>" + JieCun_Now +UnitName+ "</span>  </td>");
            strReturn.Append("  </tr>");

            string WBName = SQLHelper.ExecuteScalar(" SELECT top 1 strName FROM dbo.WB WHERE ID=" + WBID).ToString();
            string UserName = SQLHelper.ExecuteScalar(" SELECT top 1 strLoginName   FROM dbo.Users WHERE ID=" + UserID).ToString();
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>结算日期：</span></td>  <td>  <span>" + dt_Sell + "</span>  </td>");
            strReturn.Append("   <td > <span>营业网点：</span></td>  <td >  <span>" + WBName + "</span>  </td>");
            strReturn.Append("   <td > <span>营业员：</span></td>  <td >  <span>" + UserName + "</span>  </td>");
            strReturn.Append("  </tr>");

            strReturn.Append("    <tr style='height: 25px;'>");

            strReturn.Append("  </tr>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("  <td>   <span>身份证号：</span> </td><td> <div style='width:180px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   <td>   </td><td>   </td>");
            strReturn.Append("  <td>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");
            #endregion

            var returnValue = new { state = "true", msg = strReturn.ToString()};
            context.Response.Write(JsonHelper.ToJson(returnValue));
            
        }

        /// <summary>
        /// 打印产品换购凭证
        /// </summary>
        /// <param name="context"></param>
        void PrintStorageShopping(HttpContext context)
        {
            string model = "";
            if (context.Request.QueryString["model"] != null)
            {
                model = context.Request.QueryString["model"].ToString();
            }

            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            int printTime = common.getPrintTime(AccountNumber, BusinessNO);
            int printTimeUser = Convert.ToInt32(common.GetUserInfoByID(Convert.ToInt32(context.Session["ID"]))["numPrint"]);
            if (printTime > printTimeUser)
            {
                var res = new { state = "false", msg = "该小票最多打印" + printTimeUser + "次，您已经打印" + (printTime - 1).ToString() + "次,不能继续打印!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string printValue = "<span style='font-size: 12px; font-weight: bolder;'>(首次打印)</span>";
            if (printTime > 1)
            {
                printValue = "<span style='font-size: 12px;'>(第" + printTime + "次打印)</span>";
            }


            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,UnitName,VarietyID,VarietyName,VarietyCount,VarietyMoney,VarietyInterest,StorageDate,CurrentRate,EarningRate,StorageFee,StorageMoney,Price_JieSuan,Money_Earn,dt_Sell,JieCun_Last,JieCun_Now,ISReturn ");
            strSqlLog.Append("  FROM dbo.StorageShopping");
            strSqlLog.Append("  ");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                var res = new { state = "false", msg = "未查询到需要打印的信息！" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;

            }

            string SerialNumber = dtLog.Rows[0]["SerialNumber"].ToString();
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();
            string Dep_AccountNumber = dtLog.Rows[0]["Dep_AccountNumber"].ToString();
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();
            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string UserID = dtLog.Rows[0]["UserID"].ToString();
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string UnitName = dtLog.Rows[0]["UnitName"].ToString();
            string VarietyID = dtLog.Rows[0]["VarietyID"].ToString();
            string VarietyName = dtLog.Rows[0]["VarietyName"].ToString();
            string VarietyCount = dtLog.Rows[0]["VarietyCount"].ToString();
            string VarietyMoney = dtLog.Rows[0]["VarietyMoney"].ToString();
            string VarietyInterest = dtLog.Rows[0]["VarietyInterest"].ToString();
            string StorageDate = dtLog.Rows[0]["StorageDate"].ToString();
            string CurrentRate = dtLog.Rows[0]["CurrentRate"].ToString();
            string EarningRate = dtLog.Rows[0]["EarningRate"].ToString();
            EarningRate = (Math.Round(Convert.ToDecimal(EarningRate) * 100, 2)).ToString() + "%";
            string StorageFee = dtLog.Rows[0]["StorageFee"].ToString();
            StorageFee = (Math.Round(Convert.ToDecimal(StorageFee), 2)).ToString() + "%";
            string StorageMoney = dtLog.Rows[0]["StorageMoney"].ToString();
            string Price_JieSuan = dtLog.Rows[0]["Price_JieSuan"].ToString();
            string Money_Earn = dtLog.Rows[0]["Money_Earn"].ToString();
            string dt_Sell = dtLog.Rows[0]["dt_Sell"].ToString();
            string JieCun_Last = dtLog.Rows[0]["JieCun_Last"].ToString();
            string JieCun_Now = dtLog.Rows[0]["JieCun_Now"].ToString();

            #region 返回信息
            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            if (model == "")
            {
                strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户产品换购凭证</span>"+printValue+"</td> </tr>");
            }
            else {
                strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户(退还)产品换购凭证</span>"+printValue+"</td> </tr>");
            }
            strReturn.Append("   <tr><td align='center' style='font-size: 12px; text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + SerialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");

            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 160px;'>  <span >姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >账号：" + AccountNumber + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >存储产品：" + VarietyName + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >上期结存：" + JieCun_Last + "</span> </td>");
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
            strReturn.Append("    <td style='width: 70px;'> <span>保值</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>数量</span></td>");
            strReturn.Append("    <td align='center' style='width: 110px;'> <span>" + VarietyCount + "</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>结算价</span></td>");
            strReturn.Append("    <td align='center' style='width: 110px;'> <span>" + Price_JieSuan + "</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>消费金额</span></td>");
            strReturn.Append("    <td align='center' style='width: 120px;'> <span>￥" + VarietyMoney + "</span></td>");
            strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>利息</span></td>");
            strReturn.Append("    <td > <span>实存天数</span></td>");
            strReturn.Append("    <td align='center' > <span>" + StorageDate + "</span></td>");
            strReturn.Append("    <td > <span>利率</span></td>");
            strReturn.Append("    <td align='center' > <span>" + EarningRate  + "</span></td>");
            strReturn.Append("    <td > <span>利息金额</span></td>");
            strReturn.Append("    <td align='center'> <span>￥" + VarietyInterest + "</span></td>");
            strReturn.Append("  </tr>");

            //strReturn.Append("   <tr style='height: 20px;'>");
            //strReturn.Append("    <td > <span>保管费</span></td>");
            //strReturn.Append("    <td > <span>实存天数</span></td>");
            //strReturn.Append("    <td align='center' > <span>" + StorageDate + "</span></td>");
            //strReturn.Append("    <td > <span>费率</span></td>");
            //strReturn.Append("    <td align='center' > <span>" + StorageFee + "</span></td>");
            //strReturn.Append("    <td > <span>保管费金额</span></td>");
            //strReturn.Append("    <td align='center'> <span>￥" + StorageMoney + "</span></td>");
            //strReturn.Append("  </tr>");

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>总收益</span></td>");
            strReturn.Append("     <td colspan='5' > <span></span></td>");
            strReturn.Append("    <td align='center' > ￥<span>" + Money_Earn + "</span></td>");
            strReturn.Append("  </tr>");
            //string strDaXie = Fun.ChangeToRMB(Money_Earn);
            //strReturn.Append("   <tr style='height: 20px;'>");
            //strReturn.Append("    <td > <span>大写收益</span></td>");
            //strReturn.Append("     <td colspan='6' align='left'> <span>" + strDaXie + "</span></td>");
            //strReturn.Append("  </tr>");
            //strReturn.Append("   </table>");

            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width: 80px;'> <span>折合"+VarietyName+"：</span></td>  <td style='width: 170px;'>  <span>" + VarietyCount + "</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>计量单位：</span></td>  <td style='width: 110px;'>  <span>元、" + UnitName + "</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>"+VarietyName+"结存：</span></td>  <td style='width: 140px;'>  <span>" + JieCun_Now + UnitName + "</span>  </td>");
            strReturn.Append("  </tr>");

            string WBName = SQLHelper.ExecuteScalar(" SELECT top 1 strName FROM dbo.WB WHERE ID=" + WBID).ToString();
            string UserName = SQLHelper.ExecuteScalar(" SELECT top 1 strLoginName   FROM dbo.Users WHERE ID=" + UserID).ToString();
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>结算日期：</span></td>  <td>  <span>" + dt_Sell + "</span>  </td>");
            strReturn.Append("   <td > <span>营业网点：</span></td>  <td >  <span>" + WBName + "</span>  </td>");
            strReturn.Append("   <td > <span>营业员：</span></td>  <td >  <span>" + UserName + "</span>  </td>");
            strReturn.Append("  </tr>");

            strReturn.Append("    <tr style='height: 25px;'>");

            strReturn.Append("  </tr>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("  <td>   <span>身份证号：</span> </td><td> <div style='width:180px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   <td>   </td><td>   </td>");
            strReturn.Append("  <td>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");
            #endregion

            var returnValue = new { state = "true", msg = strReturn.ToString() };
            context.Response.Write(JsonHelper.ToJson(returnValue));
        }

        /// <summary>
        /// 打印商品兑换凭证(多个凭证)
        /// </summary>
        /// <param name="context"></param>
        void PrintGoodExchangeList(HttpContext context)
        {
            string model = "";
            if (context.Request.QueryString["model"] != null)
            {
                model = context.Request.QueryString["model"].ToString();
            }
            string BNList = context.Session["BNList"].ToString();//需要打印的序列组合

            string[] BNArray = BNList.Split('|');//需要打印的编号集合

            string BusinessNO = BNArray[0];//首个编号
          
          
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            int printTime = common.getPrintTime(AccountNumber, BusinessNO);
            int printTimeUser = Convert.ToInt32(common.GetUserInfoByID(Convert.ToInt32(context.Session["ID"]))["numPrint"]);
            if (printTime > printTimeUser)
            {
                var res = new { state = "false", msg = "该小票最多打印" + printTimeUser + "次，您已经打印" + (printTime - 1).ToString() + "次,不能继续打印!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string printValue = "<span style='font-size: 12px; font-weight: bolder;'>(首次打印)</span>";
            if (printTime > 1)
            {
                printValue = "<span style='font-size: 12px;'>(第" + printTime + "次打印)</span>";
            }
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,CONVERT(NVARCHAR(100),dt_Exchange,23) AS  dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn  ");
            strSqlLog.Append("  FROM dbo.GoodExchange");
            strSqlLog.Append("  ");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                var res = new { state = "false", msg = "未查询到需要打印的信息！" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;

            }

            //共有参数
            string JieCun_Last = dtLog.Rows[0]["JieCun_Last"].ToString();//上期结存
            double JieCun_Now = 0;//现在结存
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();//
            string SerialNumber = dtLog.Rows[0]["SerialNumber"].ToString();//
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();//
            string Dep_AccountNumber = dtLog.Rows[0]["Dep_AccountNumber"].ToString();//
            string dt_Exchange = dtLog.Rows[0]["dt_Exchange"].ToString();

            string Dep_SID = dtLog.Rows[0]["Dep_SID"].ToString();//姓名
            //获取存储产品信息
            StringBuilder strSqlDep_SID = new StringBuilder();
            strSqlDep_SID.Append("  SELECT TOP 1 B.strName,A.Price_ShiChang,C.strName AS UnitID");
            strSqlDep_SID.Append("  FROM  dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSqlDep_SID.Append("  INNER JOIN dbo.BD_MeasuringUnit C ON B.MeasuringUnitID=C.ID");
            strSqlDep_SID.Append("  WHERE A.ID=" + Dep_SID);
            strSqlDep_SID.Append("  ");
            DataTable dtDep_SID = SQLHelper.ExecuteDataTable(strSqlDep_SID.ToString());


            string VName = dtDep_SID.Rows[0]["strName"].ToString();
            string VPrice = dtDep_SID.Rows[0]["Price_ShiChang"].ToString();
            string VUnit = dtDep_SID.Rows[0]["UnitID"].ToString();

            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            if (model == "")
            {
                strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户兑换凭证</span>"+printValue+"</td> </tr>");
            }
            else {
                strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户(退还)兑换凭证</span>"+printValue+"</td> </tr>");
           
            } 
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + SerialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");

            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 140px;'>  <span >姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 140px;'>  <span >账号：" + AccountNumber + "</span> </td>");
            strReturn.Append("    <td style='width: 200px;'>  <span >存储产品：" + VName + " " + VPrice + "元/" + VUnit + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >上期结存：" + JieCun_Last + "</span> </td>");
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
            strReturn.Append("    <td style='width: 60px;'> <span>业务名称</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>品名</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>规格型号</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>单位</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>数量</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>单价</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>利息</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>折合原粮</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>金额</span></td>");
            strReturn.Append("  </tr>");

            double T_GoodCount = 0;//商品数量
          
            double T_VarietyInterest = 0;//利息
            double T_VarietyCount = 0;//折合产品数量
            double T_Money_DuiHuan = 0;//金额
            double T_Money_YouHui = 0;//优惠
            for (int i = 0; i < BNArray.Length; i++)
            {
                BusinessNO = BNArray[i];
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,CONVERT(NVARCHAR(100),dt_Exchange,23) AS  dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn  ");
                strSql.Append("  FROM dbo.GoodExchange");
                strSql.Append("  ");
                strSql.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

                DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
                if (dt == null || dt.Rows.Count == 0)
                {
                    context.Response.Write("");
                    return;
                }
                JieCun_Now = Convert.ToDouble(dt.Rows[0]["JieCun_Now"]);//剩余结存
                string BusinessName = dt.Rows[0]["BusinessName"].ToString();//业务名
                string GoodName = dt.Rows[0]["GoodName"].ToString();//品名
                string SpecName = dt.Rows[0]["SpecName"].ToString();
                string UnitName = dt.Rows[0]["UnitName"].ToString();

                double GoodCount = Convert.ToDouble(dt.Rows[0]["GoodCount"]);//商品数量
                double GoodPrice = Convert.ToDouble(dt.Rows[0]["GoodPrice"]);//商品价格
                double VarietyInterest = Convert.ToDouble(dt.Rows[0]["VarietyInterest"]);//利息
                double VarietyCount = Convert.ToDouble(dt.Rows[0]["VarietyCount"]);//折合产品数量
                double Money_DuiHuan = Convert.ToDouble(dt.Rows[0]["Money_DuiHuan"]);//金额
                double Money_YouHui = Convert.ToDouble(dt.Rows[0]["Money_YouHui"]);//优惠
                T_GoodCount += GoodCount;
                T_VarietyInterest += VarietyInterest;
                T_VarietyCount += VarietyCount;
                T_Money_DuiHuan += Money_DuiHuan;
                T_Money_YouHui += Money_YouHui;

            



            #region 返回信息
           


            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>兑换</span></td>");
            strReturn.Append("    <td> <span>"+GoodName+"</span></td>");
            strReturn.Append("    <td> <span>" + SpecName + "</span></td>");
            strReturn.Append("    <td> <span>" + UnitName + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodCount + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodPrice + "</span></td>");
            strReturn.Append("    <td> <span>" + VarietyInterest + "</span></td>");
            strReturn.Append("    <td> <span>" + VarietyCount + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_DuiHuan + "</span></td>");
            strReturn.Append("  </tr>");

          
            #endregion
                }
            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width: 80px;'> <span>消费金额：</span></td>  <td style='width: 170px;'>  <span>￥" + T_Money_DuiHuan + "元</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>折合" + VName + "：</span></td>  <td style='width: 110px;'>  <span>" + T_VarietyCount + VUnit + "</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>" + VName + "结存：</span></td>  <td style='width: 140px;'>  <span>" + JieCun_Now + VUnit + "</span>  </td>");
            strReturn.Append("  </tr>");

            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string UserID = dtLog.Rows[0]["UserID"].ToString();
            string WBName = SQLHelper.ExecuteScalar(" SELECT top 1 strName FROM dbo.WB WHERE ID=" + WBID).ToString();
            string UserName = SQLHelper.ExecuteScalar(" SELECT top 1 strLoginName   FROM dbo.Users WHERE ID=" + UserID).ToString();
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>利息合计：</span></td>  <td>  <span>￥" +Math.Round( T_VarietyInterest,2) + "元</span>  </td>");
            strReturn.Append("   <td > <span>优惠合计：</span></td>  <td >  <span>￥" + Math.Round(T_Money_YouHui,2) + "元</span>  </td>");
            strReturn.Append("   <td > <span>兑换日期：</span></td>  <td >  <span>" + dt_Exchange + "</span>  </td>");
            strReturn.Append("  </tr>");

            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>营业网点：</span></td>  <td>  <span>" + WBName
             + "</span>  </td>");
            strReturn.Append("   <td > <span>营业员：</span></td>  <td >  <span>" + UserName + "</span>  </td>");
            strReturn.Append("  <td>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");


            var returnValue = new { state = "true", msg = strReturn.ToString() };
            context.Response.Write(JsonHelper.ToJson(returnValue));
        }

        /// <summary>
        /// 更新一个小票被打印的次数
        /// </summary>
        /// <param name="context"></param>
        void updatePrintTime(HttpContext context)
        {
      
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select * from Dep_PrintLog");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());

            StringBuilder sql = new StringBuilder();
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                sql.Append(" insert into Dep_PrintLog (Dep_AccountNumber,BusinessNO,numTime)");
                sql.Append(string.Format(" values('{0}','{1}',1)", AccountNumber,BusinessNO));
            }
            else {
                int numTime = Convert.ToInt32(dtLog.Rows[0]["numTime"]) + 1;
                sql.Append(" update Dep_PrintLog set");
                sql.Append(string.Format(" numTime={0}", numTime));
                sql.Append(string.Format(" where Dep_AccountNumber='{0}' and BusinessNO='{1}'", AccountNumber, BusinessNO));
            }

            if (SQLHelper.ExecuteNonQuery(sql.ToString()) > 0)
            {
                context.Response.Write("Y");
            }
            else {
                context.Response.Write("N");
            }
           
        }

        /// <summary>
        /// 更新连续小票被打印的次数
        /// </summary>
        /// <param name="context"></param>
        void updatePrintTime_List(HttpContext context)
        {

            string BNOList = context.Request.QueryString["BNOList"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            string[] BNArray = BNOList.Split('|');//需要打印的编号集合
            StringBuilder sql = new StringBuilder();

            for (int i = 0; i < BNArray.Length; i++) {
                string BusinessNO = BNArray[i];
                StringBuilder strSqlLog = new StringBuilder();
                strSqlLog.Append("  select * from Dep_PrintLog");
                strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

                DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());


                if (dtLog == null || dtLog.Rows.Count == 0)
                {
                    sql.Append(" insert into Dep_PrintLog (Dep_AccountNumber,BusinessNO,numTime)");
                    sql.Append(string.Format(" values('{0}','{1}',1)", AccountNumber, BusinessNO));
                }
                else
                {
                    int numTime = Convert.ToInt32(dtLog.Rows[0]["numTime"]) + 1;
                    sql.Append(" update Dep_PrintLog set");
                    sql.Append(string.Format(" numTime={0}", numTime));
                    sql.Append(string.Format(" where Dep_AccountNumber='{0}' and BusinessNO='{1}'", AccountNumber, BusinessNO));
                }

            }

           
            if (SQLHelper.ExecuteNonQuery(sql.ToString()) > 0)
            {
                context.Response.Write("Y");
            }
            else
            {
                context.Response.Write("N");
            }

        }

        /// <summary>
        /// 获取一个小票被打印的次数
        /// </summary>
        /// <param name="context"></param>
        void getPrintTime(HttpContext context)
        {

            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select * from Dep_PrintLog");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());

            StringBuilder sql = new StringBuilder();
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write(1);
            }
            else
            {
                int numTime = Convert.ToInt32(dtLog.Rows[0]["numTime"])+1;
                context.Response.Write(numTime);
            }


        }

        /// <summary>
        /// 打印商品兑换凭证(单个凭证)
        /// </summary>
        /// <param name="context"></param>
        void PrintGoodExchange(HttpContext context)
        {

            string model = "";
            if (context.Request.QueryString["model"] != null)
            {
                model = context.Request.QueryString["model"].ToString(); 
            }
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            int printTime = common.getPrintTime(AccountNumber, BusinessNO);
            int printTimeUser = Convert.ToInt32(common.GetUserInfoByID(Convert.ToInt32(context.Session["ID"]))["numPrint"]);
            if (printTime > printTimeUser) {
                var res = new { state = "false", msg = "该小票最多打印"+printTimeUser+"次，您已经打印"+(printTime-1).ToString()+"次,不能继续打印!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string printValue = "<span style='font-size: 12px; font-weight: bolder;'>(首次打印)</span>";
            if (printTime > 1)
            {
                printValue = "<span style='font-size: 12px;'>(第" + printTime + "次打印)</span>";
            }

            


            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,CONVERT(NVARCHAR(100),dt_Exchange,23) AS  dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn  ");
            strSqlLog.Append("  FROM dbo.GoodExchange");
            strSqlLog.Append("  ");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                var res = new { state = "false", msg = "没有查找到需要打印的信息" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            //共有参数
            string JieCun_Last = dtLog.Rows[0]["JieCun_Last"].ToString();//上期结存
            double JieCun_Now = 0;//现在结存
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();//
            string SerialNumber = dtLog.Rows[0]["SerialNumber"].ToString();//
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();//
            string Dep_AccountNumber = dtLog.Rows[0]["Dep_AccountNumber"].ToString();//
            string dt_Exchange = dtLog.Rows[0]["dt_Exchange"].ToString();

            string Dep_SID = dtLog.Rows[0]["Dep_SID"].ToString();//姓名
            //获取存储产品信息
            StringBuilder strSqlDep_SID = new StringBuilder();
            strSqlDep_SID.Append("  SELECT TOP 1 B.strName,A.Price_ShiChang,C.strName AS UnitID");
            strSqlDep_SID.Append("  FROM  dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSqlDep_SID.Append("  INNER JOIN dbo.BD_MeasuringUnit C ON B.MeasuringUnitID=C.ID");
            strSqlDep_SID.Append("  WHERE A.ID=" + Dep_SID);
            strSqlDep_SID.Append("  ");
            DataTable dtDep_SID = SQLHelper.ExecuteDataTable(strSqlDep_SID.ToString());


            string VName = dtDep_SID.Rows[0]["strName"].ToString();
            string VPrice = dtDep_SID.Rows[0]["Price_ShiChang"].ToString();
            string VUnit = dtDep_SID.Rows[0]["UnitID"].ToString();

           

            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            if (model == "")
            {
                strReturn.Append("   <tr><td align='center' style=' text-align: center;'><span style='font-size: 18px; font-weight: bolder;'>" + CompanyName + "  储户兑换凭证</span>" + printValue + "</td> </tr>");
            }
            else {
               // strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户(退还)兑换凭证</span></td> </tr>");

                strReturn.Append("   <tr><td align='center' style=' text-align: center;'><span style='font-size: 18px; font-weight: bolder;'>" + CompanyName + "  储户(退还)兑换凭证</span>" + printValue + "</td> </tr>");
            }
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + SerialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");


            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 140px;'>  <span >姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 140px;'>  <span >账号：" + AccountNumber + "</span> </td>");
            strReturn.Append("    <td style='width: 200px;'>  <span >存储产品：" + VName + " " + VPrice + "元/" + VUnit + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >上期结存：" + JieCun_Last + "</span> </td>");
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
            strReturn.Append("    <td style='width: 60px;'> <span>业务名称</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>品名</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>规格型号</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>单位</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>数量</span></td>");
            strReturn.Append("    <td style='width: 70px;'> <span>单价</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>利息</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>折合原粮</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>金额</span></td>");
            strReturn.Append("  </tr>");
            double T_GoodCount = 0;//商品数量

            double T_VarietyInterest = 0;//利息
            double T_VarietyCount = 0;//折合产品数量
            double T_Money_DuiHuan = 0;//金额
            double T_Money_YouHui = 0;//优惠
            
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn  ");
                strSql.Append("  FROM dbo.GoodExchange");
                strSql.Append("  ");
                strSql.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

                DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
                if (dt == null || dt.Rows.Count == 0)
                {
                    context.Response.Write("");
                    return;
                }
                JieCun_Now = Convert.ToDouble(dt.Rows[0]["JieCun_Now"]);//剩余结存
                string BusinessName = dt.Rows[0]["BusinessName"].ToString();//业务名
                string GoodName = dt.Rows[0]["GoodName"].ToString();//品名
                string SpecName = dt.Rows[0]["SpecName"].ToString();
                string UnitName = dt.Rows[0]["UnitName"].ToString();

                double GoodCount = Convert.ToDouble(dt.Rows[0]["GoodCount"]);//商品数量
                double GoodPrice = Convert.ToDouble(dt.Rows[0]["GoodPrice"]);//商品价格
                double VarietyInterest = Convert.ToDouble(dt.Rows[0]["VarietyInterest"]);//利息
                double VarietyCount = Convert.ToDouble(dt.Rows[0]["VarietyCount"]);//折合产品数量
                double Money_DuiHuan = Convert.ToDouble(dt.Rows[0]["Money_DuiHuan"]);//金额
                double Money_YouHui = Convert.ToDouble(dt.Rows[0]["Money_YouHui"]);//优惠
                T_GoodCount += GoodCount;
                T_VarietyInterest += VarietyInterest;
                T_VarietyCount += VarietyCount;
                T_Money_DuiHuan += Money_DuiHuan;
                T_Money_YouHui += Money_YouHui;





                #region 返回信息



                strReturn.Append("   <tr style='height: 20px;'>");
                strReturn.Append("    <td > <span>兑换</span></td>");
                strReturn.Append("    <td> <span>" + GoodName + "</span></td>");
                strReturn.Append("    <td> <span>" + SpecName + "</span></td>");
                strReturn.Append("    <td> <span>" + UnitName + "</span></td>");
                strReturn.Append("    <td> <span>" + GoodCount + "</span></td>");
                strReturn.Append("    <td> <span>" + GoodPrice + "</span></td>");
                strReturn.Append("    <td> <span>" + VarietyInterest + "</span></td>");
                strReturn.Append("    <td> <span>" + VarietyCount + "</span></td>");
                strReturn.Append("    <td> <span>￥" + Money_DuiHuan + "</span></td>");
                strReturn.Append("  </tr>");


                #endregion
           
            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width: 80px;'> <span>消费金额：</span></td>  <td style='width: 170px;'>  <span>￥" + T_Money_DuiHuan + "元</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>折合" + VName + "：</span></td>  <td style='width: 110px;'>  <span>" + T_VarietyCount + VUnit + "</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>" + VName + "结存：</span></td>  <td style='width: 140px;'>  <span>" + JieCun_Now + VUnit + "</span>  </td>");
            strReturn.Append("  </tr>");

            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string UserID = dtLog.Rows[0]["UserID"].ToString();
            string WBName = SQLHelper.ExecuteScalar(" SELECT top 1 strName FROM dbo.WB WHERE ID=" + WBID).ToString();
            string UserName = SQLHelper.ExecuteScalar(" SELECT top 1 strLoginName   FROM dbo.Users WHERE ID=" + UserID).ToString();
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>利息合计：</span></td>  <td>  <span>￥" + Math.Round(T_VarietyInterest, 2) + "元</span>  </td>");
            strReturn.Append("   <td > <span>优惠合计：</span></td>  <td >  <span>￥" + Math.Round(T_Money_YouHui, 2) + "元</span>  </td>");
            strReturn.Append("   <td > <span>兑换日期：</span></td>  <td >  <span>" + dt_Exchange + "</span>  </td>");
            strReturn.Append("  </tr>");

            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>营业网点：</span></td>  <td>  <span>" + WBName
             + "</span>  </td>");
            strReturn.Append("   <td > <span>营业员：</span></td>  <td >  <span>" + UserName + "</span>  </td>");
            strReturn.Append("  <td>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");

            var returnValue = new { state = "true", msg = strReturn.ToString() };
            context.Response.Write(JsonHelper.ToJson(returnValue));
           
        }


        /// <summary>
        /// 打印商品兑换凭证(单个凭证)
        /// </summary>
        /// <param name="context"></param>
        void PrintGoodExchangeGroup(HttpContext context)
        {

            string model = "";
            if (context.Request.QueryString["model"] != null)
            {
                model = context.Request.Form["model"].ToString();
            }
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();

            int printTime = common.getPrintTime(AccountNumber, BusinessNO);
            int printTimeUser = Convert.ToInt32(common.GetUserInfoByID(Convert.ToInt32(context.Session["ID"]))["numPrint"]);
            if (printTime > printTimeUser)
            {
                var res = new { state = "false", msg = "该小票最多打印" + printTimeUser + "次，您已经打印" + (printTime - 1).ToString() + "次,不能继续打印!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string printValue = "<span style='font-size: 12px; font-weight: bolder;'>(首次打印)</span>";
            if (printTime > 1)
            {
                printValue = "<span style='font-size: 12px;'>(第" + printTime + "次打印)</span>";
            }




            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,CONVERT(NVARCHAR(100),dt_Exchange,23) AS  dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn ,exchangeGroupProp,orderdate,orderdateDone, orderstate ");
            strSqlLog.Append("  FROM dbo.GoodExchangeGroup");
            strSqlLog.Append("  ");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                var res = new { state = "false", msg = "没有查找到需要打印的信息" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            //共有参数
            string JieCun_Last = dtLog.Rows[0]["JieCun_Last"].ToString();//上期结存
            double JieCun_Now = 0;//现在结存
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();//
            string SerialNumber = dtLog.Rows[0]["SerialNumber"].ToString();//
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();//
            string Dep_AccountNumber = dtLog.Rows[0]["Dep_AccountNumber"].ToString();//
            string dt_Exchange = dtLog.Rows[0]["dt_Exchange"].ToString();
            double exchangeGroupProp = Convert.ToDouble(dtLog.Rows[0]["exchangeGroupProp"]) / 100;

            string Dep_SID = dtLog.Rows[0]["Dep_SID"].ToString();//姓名
            //获取存储产品信息
            StringBuilder strSqlDep_SID = new StringBuilder();
            strSqlDep_SID.Append("  SELECT TOP 1 B.strName,A.Price_ShiChang,C.strName AS UnitID");
            strSqlDep_SID.Append("  FROM  dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSqlDep_SID.Append("  INNER JOIN dbo.BD_MeasuringUnit C ON B.MeasuringUnitID=C.ID");
            strSqlDep_SID.Append("  WHERE A.ID=" + Dep_SID);
            strSqlDep_SID.Append("  ");
            DataTable dtDep_SID = SQLHelper.ExecuteDataTable(strSqlDep_SID.ToString());


            string VName = dtDep_SID.Rows[0]["strName"].ToString();
            string VPrice = dtDep_SID.Rows[0]["Price_ShiChang"].ToString();
            string VUnit = dtDep_SID.Rows[0]["UnitID"].ToString();



            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            if (model == "")
            {
                strReturn.Append("   <tr><td align='center' style=' text-align: center;'><span style='font-size: 18px; font-weight: bolder;'>" + CompanyName + "  储户分时批量兑换凭证</span>" + printValue + "</td> </tr>");
            }
            else
            {
                // strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户(退还)兑换凭证</span></td> </tr>");

                strReturn.Append("   <tr><td align='center' style=' text-align: center;'><span style='font-size: 18px; font-weight: bolder;'>" + CompanyName + "  储户(退还)分时批量兑换凭证</span>" + printValue + "</td> </tr>");
            }
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + SerialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");


            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 140px;'>  <span >姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 140px;'>  <span >账号：" + AccountNumber + "</span> </td>");
            strReturn.Append("    <td style='width: 200px;'>  <span >存储产品：" + VName + " " + VPrice + "元/" + VUnit + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >上期结存：" + JieCun_Last + "</span> </td>");
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
            strReturn.Append("    <td style='width: 80px;'> <span>品名</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>单位</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>数量</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>单价</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>批量价</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>利息</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>优惠金额</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>折合原粮</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>金额</span></td>");
            strReturn.Append("  </tr>");
            double T_GoodCount = 0;//商品数量

            double T_VarietyInterest = 0;//利息
            double T_VarietyCount = 0;//折合产品数量
            double T_Money_DuiHuan = 0;//金额
            double T_Money_YouHui = 0;//优惠
            double T_Group_YouHui = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn  ");
            strSql.Append("  FROM dbo.GoodExchangeGroup");
            strSql.Append("  ");
            strSql.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            JieCun_Now = Convert.ToDouble(dt.Rows[0]["JieCun_Now"]);//剩余结存
            string BusinessName = dt.Rows[0]["BusinessName"].ToString();//业务名
            string GoodName = dt.Rows[0]["GoodName"].ToString();//品名
            string SpecName = dt.Rows[0]["SpecName"].ToString();
            string UnitName = dt.Rows[0]["UnitName"].ToString();

            double GoodCount = Convert.ToDouble(dt.Rows[0]["GoodCount"]);//商品数量
            double GoodPrice_ex = Convert.ToDouble(dt.Rows[0]["GoodPrice"]);//商品价格
            double GoodPrice = GoodPrice_ex * exchangeGroupProp;
            double Group_YouHui = Math.Round(GoodCount * (GoodPrice_ex - GoodPrice), 2);

            double VarietyInterest = Convert.ToDouble(dt.Rows[0]["VarietyInterest"]);//利息
            double VarietyCount = Convert.ToDouble(dt.Rows[0]["VarietyCount"]);//折合产品数量
            double Money_DuiHuan = Convert.ToDouble(dt.Rows[0]["Money_DuiHuan"]);//金额
            double Money_YouHui = Convert.ToDouble(dt.Rows[0]["Money_YouHui"]);//优惠
            T_GoodCount += GoodCount;
            T_VarietyInterest += VarietyInterest;
            T_VarietyCount += VarietyCount;
            T_Money_DuiHuan += Money_DuiHuan;
            T_Money_YouHui += Money_YouHui;
            T_Group_YouHui += Group_YouHui;




            #region 返回信息


            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td> <span>" + GoodName + "</span></td>");
            strReturn.Append("    <td> <span>" + UnitName + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodCount + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodPrice_ex + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodPrice + "</span></td>");
            strReturn.Append("    <td> <span>" + VarietyInterest + "</span></td>");
            strReturn.Append("    <td> <span>" + Group_YouHui + "</span></td>");
            strReturn.Append("    <td> <span>" + VarietyCount + "</span></td>");
            strReturn.Append("    <td> <span>￥" + Money_DuiHuan + "</span></td>");
            strReturn.Append("  </tr>");


            #endregion

            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width: 80px;'> <span>消费金额：</span></td>  <td style='width: 170px;'>  <span>￥" + T_Money_DuiHuan + "元</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>折合" + VName + "：</span></td>  <td style='width: 110px;'>  <span>" + T_VarietyCount + VUnit + "</span>  </td>");
            strReturn.Append("   <td style='width: 80px;'> <span>" + VName + "结存：</span></td>  <td style='width: 140px;'>  <span>" + JieCun_Now + VUnit + "</span>  </td>");
            strReturn.Append("  </tr>");

            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string UserID = dtLog.Rows[0]["UserID"].ToString();
            string WBName = SQLHelper.ExecuteScalar(" SELECT top 1 strName FROM dbo.WB WHERE ID=" + WBID).ToString();
            string UserName = SQLHelper.ExecuteScalar(" SELECT top 1 strLoginName   FROM dbo.Users WHERE ID=" + UserID).ToString();
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>利息合计：</span></td>  <td>  <span>￥" + Math.Round(T_VarietyInterest, 2) + Math.Round(T_Money_YouHui, 2) + "元</span>  </td>");
            strReturn.Append("   <td > <span>优惠合计：</span></td>  <td >  <span>￥" + Math.Round(T_Group_YouHui, 2) + "元</span>  </td>");
            strReturn.Append("   <td > <span>兑换日期：</span></td>  <td >  <span>" + dt_Exchange + "</span>  </td>");
            strReturn.Append("  </tr>");

            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>营业网点：</span></td>  <td>  <span>" + WBName
             + "</span>  </td>");
            strReturn.Append("   <td > <span>营业员：</span></td>  <td >  <span>" + UserName + "</span>  </td>");
            strReturn.Append("  <td>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");

            var returnValue = new { state = "true", msg = strReturn.ToString() };
            context.Response.Write(JsonHelper.ToJson(returnValue));

        }

        /// <summary>
        /// 根据商品ID 和网点编号 获取该网点的库存
        /// </summary>
        /// <param name="context"></param>
        void GetGoodStoreByGoodID(HttpContext context)
        {
            string GoodID = context.Request.Form["GoodID"].ToString();
            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();
            
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT SUM(A.numStore) AS numStore, B.strName AS GoodName,B.Price_Stock,B.Price_XiaoShou,B.Price_DuiHuan, C.strName AS UnitName,D.strName as SpecName ");
            strSql.Append("  FROM  dbo.GoodStorage A  INNER JOIN dbo.Good B ON A.GoodID=B.ID  ");
            strSql.Append("     LEFT JOIN dbo.BD_MeasuringUnit C ON B.MeasuringUnit=C.ID   ");
            strSql.Append("     LEFT JOIN dbo.BD_PackingSpec D ON B.PackingSpecID=D.ID   ");
            strSql.Append("    WHERE A.GoodID=" + GoodID + " AND A.WBID=" + WBID + " and A.WBWareHouseID="+WBWareHouseID);
            strSql.Append("   GROUP BY B.strName,B.Price_Stock,B.Price_XiaoShou,B.Price_DuiHuan,C.strName,D.strName");
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
        /// 获取当前网点可以用于兑换的商品
        /// </summary>
        /// <param name="context"></param>
        void GetExchangeGoodStore(HttpContext context)
        {
            
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.ID AS GoodID,B.strName AS GoodName,A.WBWareHouseID,B.Price_Stock,B.Price_XiaoShou,B.Price_DuiHuan,C.strName AS UnitName,D.strName as SpecName,SUM(A.numStore) AS numStore");
            strSql.Append("   FROM dbo.GoodStorage A INNER JOIN dbo.Good B ON A.GoodID=B.ID");
            strSql.Append("  LEFT JOIN dbo.BD_MeasuringUnit C ON B.MeasuringUnit=C.ID");
            strSql.Append(" LEFT JOIN dbo.BD_PackingSpec D ON B.PackingSpecID=D.ID");
            strSql.Append(string.Format( " WHERE numStore>0 AND A.WBID={0}",WBID));
            strSql.Append(" GROUP BY B.ID,B.strName,A.WBWareHouseID,B.Price_Stock,B.Price_XiaoShou,B.Price_DuiHuan,C.strName ,D.strName");
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


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}