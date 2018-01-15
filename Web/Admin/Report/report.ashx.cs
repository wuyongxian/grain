using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;

namespace Web.Admin.Report
{
    /// <summary>
    /// report 的摘要说明
    /// </summary>
    public class report : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            common.IsLogin();
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "Get_StorageLog": Get_StorageLog(context); break;
                    case "Get_Storage": Get_Storage(context); break;
                    case "Get_Storage_Detail": Get_Storage_Detail(context); break;
                    case "Get_Exchange": Get_Exchange(context); break;
                    case "Get_Exchange_Detail": Get_Exchange_Detail(context); break;
                    case "Get_GoodExchangeIntegral": Get_GoodExchangeIntegral(context); break;
                    case "Get_GoodExchangeIntegral_Detail": Get_GoodExchangeIntegral_Detail(context); break;
                    case "Get_GoodSell": Get_GoodSell(context); break;
                    case "Get_GoodSell_Detail": Get_GoodSell_Detail(context); break;
                    case "Get_Sell": Get_Sell(context); break;
                    case "Get_Sell_Detail": Get_Sell_Detail(context); break;
                    case "Get_Sell_Chart": Get_Sell_Chart(context); break;
                    case "Get_ExchangeGood": Get_ExchangeGood(context); break;
                    case "Get_Exchange_Trend": Get_Exchange_Trend(context); break;
                    case "Get_Exchange_Catagory": Get_Exchange_Catagory(context); break;
                    case "Get_Shopping": Get_Shopping(context); break;
                    case "Get_Shopping_Detail": Get_Shopping_Detail(context); break;
                }
            }

        }

        /// <summary>
        /// 获取全部商品分类
        /// </summary>
        /// <param name="context"></param>
        void Get_StorageLog(HttpContext context)
        {
            string QWBID = context.Request.Form["QWBID"].ToString();
            string QVarietyID = context.Request.Form["QVarietyID"].ToString();
            string storageType = context.Request.Form["storageType"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT A.ID, B.strName AS WBName, CASE (A.storageType) WHEN 1 THEN '存入' WHEN 2 THEN '修改存粮' WHEN 3 THEN '退还存粮' WHEN 4 THEN '出库' WHEN 5 THEN '修改出库' END AS storageType,");
            strSql.Append("  AccountNumber,C.strName AS VarietyName,convert(char(10),A.dtLog,120) as dtLog, ");
            strSql.Append("  cast( A.numStorageChange AS DECIMAL(18,2)) AS numStorageChange,");
            strSql.Append("  cast( A.numStorageIn AS DECIMAL(18,2)) AS numStorageIn,");
            strSql.Append("  cast( A.numStorageOut AS DECIMAL(18,2)) AS numStorageOut,");
            strSql.Append("  cast( A.numStorage AS DECIMAL(18,2)) AS numStorage,");
            strSql.Append("  cast( A.numStorageLoss AS DECIMAL(18,2)) AS numStorageLoss");
            strSql.Append("  ");
            strSql.Append("   FROM dbo.SA_VarietyStorageLog A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSql.Append("  where 1=1 ");

            if (QWBID != "0")
            {
                strSql.Append("   AND B.ID = " + QWBID);
            }
            if (QVarietyID != "0")
            {
                strSql.Append("   AND C.ID = " + QVarietyID);
            }
            if (storageType != "0")
            {
                if (storageType == "1")
                {
                    strSql.Append(" and A.storageType in (1,2,3)");
                }
                else if (storageType == "2")
                {
                    strSql.Append(" and A.storageType in (4,5)");
                }
            }

            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dtLog> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    Qdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dtLog < '" + Qdtend + "'");
                }
              
            }
            strSql.Append("  ORDER BY A.ID DESC");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        /// <summary>
        /// 获取全部商品分类
        /// </summary>
        /// <param name="context"></param>
        void Get_Storage(HttpContext context)
        {

            string report_type = context.Request.Form["report_type"].ToString();
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string QVarietyID = context.Request.Form["QVarietyID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            if (report_type == "1")
            {
                strSql.Append("  SELECT C.ID as WBID, C.strName AS WBName,W.ID as TradeWBID,W.strName as TradeWBName, D.ID as TimeID, D.strName AS  TimeName ,E.ID as VarietyID, E.strName AS  VarietyName,SUM(StorageNumberRaw) AS StorageNumber ,A.Price_ShiChang,SUM(StorageNumberRaw)*Price_ShiChang AS TotalPrice");
            }
            else
            {
                strSql.Append("  SELECT C.ID as WBID, C.strName AS WBName,W.ID as TradeWBID,W.strName as TradeWBName, D.ID as TimeID, D.strName AS  TimeName ,E.ID as VarietyID, E.strName AS  VarietyName,SUM(StorageNumber) AS StorageNumber ,A.Price_ShiChang,SUM(StorageNumber)*Price_ShiChang AS TotalPrice");
            }
            strSql.Append("  FROM Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSql.Append("   INNER JOIN dbo.WB C ON B.WBID=C.ID");
            strSql.Append("   INNER JOIN dbo.WB W ON A.WBID=W.ID");
            strSql.Append("  INNER JOIN dbo.StorageTime D ON A.TimeID=D.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql.Append("  where 1=1 and C.ISSimulate=0");

            //if (trade_type == "1")
            //{
            //    strSql.Append("   AND A.WBID=B.WBID");
            //}
            //else if (trade_type == "2")
            //{
            //    strSql.Append("   AND A.WBID!=B.WBID");
            //}

            //if (QWBID != "0")
            //{
            //    strSql.Append("   AND C.ID = " + QWBID);
            //}
            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (W.ID ={0} or C.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND W.ID ={0}   AND C.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (W.ID!=C.ID AND W.ID ={0}   or C.ID={0} AND W.ID!=C.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND W.ID ={0}   AND C.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND W.ID !={0}   AND C.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND W.ID=C.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND W.ID!=C.ID");
                }
            }

            if (QVarietyID != "0") {
                strSql.Append("   AND E.ID = " + QVarietyID);
            }

            if (Qdtstart != "")
            {
                strSql.Append("   AND A.StorageDate> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                  string  strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                  strSql.Append("   AND A.StorageDate < '" + strQdtend + "'");
                }
               
            }
            strSql.Append("   GROUP BY C.ID,W.ID,W.strName, C.strName ,D.ID,D.strName,E.ID,E.strName,A.Price_ShiChang");
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            StringBuilder strSql_T = new StringBuilder();
            if (report_type== "1")
            {
                strSql_T.Append("  SELECT E.strName AS  VarietyName,SUM(StorageNumberRaw) AS StorageNumber ,A.Price_ShiChang,SUM(StorageNumberRaw)*Price_ShiChang AS TotalPrice");
            }
            else
            {
                strSql_T.Append("  SELECT E.strName AS  VarietyName,SUM(StorageNumber) AS StorageNumber ,A.Price_ShiChang,SUM(StorageNumber)*Price_ShiChang AS TotalPrice");
            }
            strSql_T.Append("  FROM Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSql_T.Append("   INNER JOIN dbo.WB C ON B.WBID=C.ID");
            strSql_T.Append("   INNER JOIN dbo.WB W ON A.WBID=W.ID");
            strSql_T.Append("  INNER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql_T.Append("  where 1=1 and C.ISSimulate=0");

            //if (trade_type == "1")
            //{
            //    strSql_T.Append("   AND A.WBID=B.WBID");
            //}
            //else if (trade_type == "2")
            //{
            //    strSql_T.Append("   AND A.WBID!=B.WBID");
            //}
            //if (QWBID != "0")
            //{
            //    strSql_T.Append("   AND C.ID = " + QWBID);
            //}

            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                // strSql_t.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql_T.Append(string.Format(" AND (W.ID ={0} or C.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql_T.Append(string.Format(" AND W.ID ={0}   AND C.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql_T.Append(string.Format("  AND (W.ID!=C.ID AND W.ID ={0}   or C.ID={0} AND W.ID!=C.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql_T.Append(string.Format(" AND W.ID ={0}   AND C.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql_T.Append(string.Format(" AND W.ID !={0}   AND C.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql_T.Append(" AND W.ID=C.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql_T.Append(" AND W.ID!=C.ID");
                }
            }

            if (QVarietyID != "0")
            {
                strSql_T.Append("   AND E.ID = " + QVarietyID);
            }
          
            if (Qdtstart != "")
            {
                strSql_T.Append("   AND A.StorageDate> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql_T.Append("   AND A.StorageDate < '" + strQdtend + "'");
                }
               
               
            }

            strSql_T.Append("   GROUP BY E.strName,A.Price_ShiChang");

            DataTable dt2 = SQLHelper.ExecuteDataTable(strSql_T.ToString());

            if (dt != null && dt.Rows.Count != 0 && dt2 != null && dt2.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt),data_t=JsonHelper.ToJson(dt2) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Storage_Detail(HttpContext context) {
            string report_type = context.Request.Form["report_type"].ToString();
            string TradeWBID = context.Request.Form["TradeWBID"].ToString();
            string WBID = context.Request.Form["WBID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string TimeID = context.Request.Form["TimeID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();
            string Price_ShiChang = context.Request.Form["Price_ShiChang"].ToString();

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT C.strName AS WBName,W.strName as TradeWBName, D.strName AS  TimeName ,E.strName AS  VarietyName,B.AccountNumber,B.strName,CONVERT(varchar(100), StorageDate, 23) AS StorageDate,");

            strSql.Append("  A.Price_ShiChang,A.StorageNumberRaw, A.StorageNumber,");
            strSql.Append("  A.StorageNumberRaw*Price_ShiChang AS TotalPriceRaw,A.StorageNumber*Price_ShiChang AS TotalPrice");
            strSql.Append("   FROM Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSql.Append("   INNER JOIN dbo.WB C ON B.WBID=C.ID");
            strSql.Append("   INNER JOIN dbo.WB W ON A.WBID=W.ID");
            strSql.Append("   INNER JOIN dbo.StorageTime D ON A.TimeID=D.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql.Append("    where 1=1 and C.ISSimulate=0");

            if (TradeWBID != "")
            {
                strSql.Append("   AND W.ID = " + TradeWBID);
            }
            if (WBID != "")
            {
                strSql.Append("   AND C.ID = " + WBID);
            }
            if (TimeID != "")
            {
                strSql.Append("   AND D.ID = " + TimeID);
            }
            if (VarietyID != "")
            {
                strSql.Append("   AND E.ID = " + VarietyID);
            }
            if (Price_ShiChang != "")
            {
                strSql.Append("   AND A.Price_ShiChang = " + Price_ShiChang);
            }

            if (Qdtstart != "")
            {
                strSql.Append("   AND A.StorageDate> '" + Qdtstart + "'");

            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    Qdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.StorageDate < '" + Qdtend + "'");
                }
              
            }
            strSql.Append("   ORDER BY A.StorageDate,A.AccountNumber");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0 )
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt)};
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_GoodSell(HttpContext context)
        {
            string report_type = context.Request.Form["report_type"].ToString();
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT W.ID as WBID, W.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName,E.ID as GoodID, E.strName AS  GoodName,A.GoodPrice, ");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  GoodCount ELSE 0-GoodCount END) AS GoodCount,");
            strSql.Append("     SUM( CASE ISReturn WHEN  0 THEN  GoodValue ELSE 0-GoodValue END) AS GoodValue,");
            strSql.Append("      E.Price_StockHQ, SUM( CASE ISReturn WHEN  0 THEN  E.Price_StockHQ*GoodCount ELSE 0- E.Price_StockHQ*GoodCount END) AS costHQ,");
            strSql.Append("      E.Price_Stock, SUM( CASE ISReturn WHEN  0 THEN  E.Price_Stock*GoodCount ELSE 0- E.Price_Stock*GoodCount END) AS costWB");
          
            strSql.Append("  FROM dbo.GoodSell A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("   LEFT OUTER JOIN dbo.Good E ON A.GoodID=E.ID");
            strSql.Append("   INNER JOIN dbo.Depositor F ON A.Dep_AccountNumber=F.AccountNumber");
            strSql.Append("   INNER JOIN dbo.WB W ON F.WBID=W.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }



            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (W.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND W.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (W.ID!=B.ID AND W.ID ={0}   or B.ID={0} AND W.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND W.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND W.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND W.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND W.ID!=B.ID");
                }
            }
            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Sell> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Sell < '" + strQdtend + "'");
                }

            }
            strSql.Append("     GROUP BY W.ID, W.strName,B.ID, B.strName,E.ID, E.strName,A.GoodPrice ,E.Price_Stock,E.Price_StockHQ ");
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_GoodSell_Detail(HttpContext context)
        {

            string report_type = context.Request.Form["report_type"].ToString();
            string TradeWBID = context.Request.Form["TradeWBID"].ToString();
            string WBID = context.Request.Form["WBID"].ToString();
            string GoodID = context.Request.Form["GoodID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT F.ID as Dep_ID,F.AccountNumber,F.strName as Dep_Name, W.ID as WBID, W.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName, E.strName AS  GoodName, GoodCount,A.GoodPrice,A.GoodValue,CONVERT(varchar(100), dt_Sell, 23) AS dt_Sell");
            strSql.Append("   ,CASE(ISReturn) WHEN 0 THEN '商品销售' ELSE '退还销售' END AS ISReturn");
            strSql.Append("  FROM dbo.Goodsell A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.Good E ON A.GoodID=E.ID");
            strSql.Append("   INNER JOIN dbo.Depositor F ON A.Dep_AccountNumber=F.AccountNumber");
            strSql.Append("   INNER JOIN dbo.WB W ON F.WBID=W.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }
            if (TradeWBID != "")
            {
                strSql.Append("   AND B.ID = " + TradeWBID);
            }
            if (WBID != "")
            {
                strSql.Append("   AND W.ID = " + WBID);
            }
            if (GoodID != "")
            {
                strSql.Append("   AND E.ID = " + GoodID);
            }
           
            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Sell> '" + Qdtstart + "'");

            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    Qdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Sell < '" + Qdtend + "'");
                }

            }
            strSql.Append("  order by dt_Sell");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_GoodExchangeIntegral(HttpContext context)
        {
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT W.ID as WBID, W.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName,E.ID as GoodID, E.strName AS  GoodName,A.GoodPrice, ");
            strSql.Append("    SUM(GoodCount) AS GoodCount,SUM(integral_Change) AS integral_Change");
           

            strSql.Append("  FROM dbo.GoodExchangeIntegral A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("   LEFT OUTER JOIN dbo.Good E ON A.GoodID=E.ID");
            strSql.Append("   INNER JOIN dbo.Depositor F ON A.Dep_AccountNumber=F.AccountNumber");
            strSql.Append("   INNER JOIN dbo.WB W ON F.WBID=W.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");    


            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (W.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND W.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (W.ID!=B.ID AND W.ID ={0}   or B.ID={0} AND W.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND W.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND W.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND W.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND W.ID!=B.ID");
                }
            }
            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Integral> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Integral < '" + strQdtend + "'");
                }

            }
            strSql.Append("   GROUP BY W.ID, W.strName,B.ID, B.strName,E.ID, E.strName,A.GoodPrice");
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_GoodExchangeIntegral_Detail(HttpContext context)
        {

            string TradeWBID = context.Request.Form["TradeWBID"].ToString();
            string WBID = context.Request.Form["WBID"].ToString();
            string GoodID = context.Request.Form["GoodID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT F.ID as Dep_ID,F.AccountNumber,F.strName as Dep_Name, W.ID as WBID, W.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName, E.strName AS  GoodName, GoodCount,A.GoodPrice,A.GoodValue,A.integral_Change,CONVERT(varchar(100), dt_Integral, 23) AS dt_Integral");
            strSql.Append("  FROM dbo.GoodExchangeIntegral A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.Good E ON A.GoodID=E.ID");
            strSql.Append("   INNER JOIN dbo.Depositor F ON A.Dep_AccountNumber=F.AccountNumber");
            strSql.Append("   INNER JOIN dbo.WB W ON F.WBID=W.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
        
            if (TradeWBID != "")
            {
                strSql.Append("   AND B.ID = " + TradeWBID);
            }
            if (WBID != "")
            {
                strSql.Append("   AND W.ID = " + WBID);
            }
            if (GoodID != "")
            {
                strSql.Append("   AND E.ID = " + GoodID);
            }

            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Integral> '" + Qdtstart + "'");

            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    Qdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Integral < '" + Qdtend + "'");
                }

            }
            strSql.Append("  order by dt_Integral");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Exchange(HttpContext context) {

            string QVarietyID = context.Request.Form["QVarietyID"].ToString();
            string report_type = context.Request.Form["report_type"].ToString();
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT W.ID as WBID, W.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName,D.ID as VarietyID, D.strName AS  VarietyName,E.ID as GoodID, E.strName AS  GoodName, ");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  GoodCount ELSE 0-GoodCount END) AS GoodCount,");
            strSql.Append("     SUM( CASE ISReturn WHEN  0 THEN  VarietyCount ELSE 0-VarietyCount END) AS VarietyCount,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  Money_DuiHuan ELSE 0-Money_DuiHuan END) AS Money_DuiHuan,");
            strSql.Append("     SUM( CASE ISReturn WHEN  0 THEN  VarietyInterest ELSE 0-VarietyInterest END) AS VarietyInterest");
            strSql.Append("  FROM dbo.GoodExchange A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql.Append("    LEFT OUTER JOIN dbo.StorageVariety D ON C.VarietyID=D.ID");
            strSql.Append("   LEFT OUTER JOIN dbo.Good E ON A.GoodID=E.ID");
            strSql.Append("   INNER JOIN dbo.Depositor F ON A.Dep_AccountNumber=F.AccountNumber");
            strSql.Append("   INNER JOIN dbo.WB W ON F.WBID=W.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }

            if (QVarietyID != "0")
            {
                strSql.Append("   AND D.ID = " + QVarietyID);
            }

            

            if (QWBID != "0"&&QWBID!="")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (W.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND W.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (W.ID!=B.ID AND W.ID ={0}   or B.ID={0} AND W.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND W.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND W.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {
                    
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND W.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND W.ID!=B.ID");
                }
            }
            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Exchange> '" + Qdtstart+ "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend)) {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Exchange < '" + strQdtend + "'");
                }
                
            }
            strSql.Append("   GROUP BY W.ID, W.strName,B.ID, B.strName,E.ID, E.strName,D.ID,D.strName");
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            StringBuilder strSql_t = new StringBuilder();
            strSql_t.Append("  SELECT  D.strName AS  VarietyName,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyCount ELSE 0-VarietyCount END) AS VarietyCount,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  Money_DuiHuan ELSE 0-Money_DuiHuan END) AS Money_DuiHuan,");
            strSql_t.Append("   SUM( CASE ISReturn WHEN  0 THEN  VarietyInterest ELSE 0-VarietyInterest END) AS VarietyInterest");
            strSql_t.Append("  FROM dbo.GoodExchange A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql_t.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql_t.Append("   LEFT OUTER JOIN dbo.StorageVariety D ON C.VarietyID=D.ID");
            strSql_t.Append("   INNER JOIN dbo.Depositor F ON A.Dep_AccountNumber=F.AccountNumber");
            strSql_t.Append("   INNER JOIN dbo.WB W ON F.WBID=W.ID");
            strSql_t.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql_t.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql_t.Append("  and A.ISReturn!=0");
            }


            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
               // strSql_t.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql_t.Append(string.Format(" AND (W.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql_t.Append(string.Format(" AND W.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql_t.Append(string.Format("  AND (W.ID!=B.ID AND W.ID ={0}   or B.ID={0} AND W.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql_t.Append(string.Format(" AND W.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql_t.Append(string.Format(" AND W.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql_t.Append(" AND W.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql_t.Append(" AND W.ID!=B.ID");
                }
            }

            if (Qdtstart!= "")
            {
                strSql_t.Append("   AND A.dt_Exchange> '" + Qdtstart + "'");
            }
            if (Qdtend!= "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql_t.Append("   AND A.dt_Exchange < '" + strQdtend + "'");
                }
            }
            strSql_t.Append("   GROUP BY  D.strName");
            DataTable dt2 = SQLHelper.ExecuteDataTable(strSql_t.ToString());

            if (dt != null && dt.Rows.Count != 0 && dt2 != null && dt2.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt), data_t = JsonHelper.ToJson(dt2) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Exchange_Detail(HttpContext context){

            string report_type = context.Request.Form["report_type"].ToString();
            string TradeWBID = context.Request.Form["TradeWBID"].ToString();
            string WBID = context.Request.Form["WBID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string GoodID = context.Request.Form["GoodID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT F.ID as Dep_ID,F.AccountNumber,F.strName as Dep_Name, W.ID as WBID, W.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName, D.strName AS  VarietyName, E.strName AS  GoodName, GoodCount,VarietyCount,CONVERT(varchar(100), dt_Exchange, 23) AS dt_Exchange,");
            strSql.Append("   Money_DuiHuan,VarietyInterest");
            strSql.Append("   ,CASE(ISReturn) WHEN 0 THEN '兑换' ELSE '退还兑换' END AS ISReturn");
            strSql.Append("  FROM dbo.GoodExchange A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql.Append("   INNER JOIN dbo.StorageVariety D ON C.VarietyID=D.ID");
            strSql.Append("  INNER JOIN dbo.Good E ON A.GoodID=E.ID");
            strSql.Append("   INNER JOIN dbo.Depositor F ON A.Dep_AccountNumber=F.AccountNumber");
            strSql.Append("   INNER JOIN dbo.WB W ON F.WBID=W.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }
            if (TradeWBID != "")
            {
                strSql.Append("   AND B.ID = " + TradeWBID);
            }
            if (WBID != "")
            {
                strSql.Append("   AND W.ID = " + WBID);
            }
            if (GoodID != "")
            {
                strSql.Append("   AND E.ID = " + GoodID);
            }
            if (VarietyID != "")
            {
                strSql.Append("   AND D.ID = " + VarietyID);
            }
            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Exchange> '" + Qdtstart + "'");

            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    Qdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Exchange < '" + Qdtend + "'");
                }
                
            }
            strSql.Append("  order by dt_Exchange");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Sell(HttpContext context)
        {
            string report_type = context.Request.Form["report_type"].ToString();
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string QVarietyID = context.Request.Form["QVarietyID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT F.ID as WBID,F.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName,D.ID as TimeID, D.strName AS TimeName,E.ID as VarietyID, E.strName AS VarietyName,C.Price_ShiChang,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyCount ELSE 0-VarietyCount END) AS VarietyCount,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyInterest ELSE 0-VarietyInterest END) AS VarietyInterest,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  StorageMoney ELSE 0-StorageMoney END) AS StorageMoney,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyMoney ELSE 0-VarietyMoney END) AS VarietyMoney");
            strSql.Append("   FROM dbo.StorageSell A INNER JOIN dbo.WB B ON A.WBID =B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql.Append("  LEFT OUTER JOIN dbo.StorageTime D ON C.TimeID=D.ID");
            strSql.Append("  LEFT OUTER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
             else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }

            if (QVarietyID != "0")
            {
                strSql.Append("   AND E.ID = " + QVarietyID);
            }

            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (F.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (F.ID!=B.ID AND F.ID ={0}   or B.ID={0} AND F.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND F.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND F.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND F.ID!=B.ID");
                }
            }

            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Sell> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Sell < '" + strQdtend + "'");
                }
               
            }
            strSql.Append("    GROUP BY F.ID,F.strName,B.ID, B.strName,D.ID,D.strName,E.ID, E.strName,C.Price_ShiChang");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            StringBuilder strSql_t = new StringBuilder();
            strSql_t.Append("   SELECT E.strName AS VarietyName,C.Price_ShiChang,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyCount ELSE 0-VarietyCount END) AS VarietyCount,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyInterest ELSE 0-VarietyInterest END) AS VarietyInterest,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  StorageMoney ELSE 0-StorageMoney END) AS StorageMoney,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyMoney ELSE 0-VarietyMoney END) AS VarietyMoney");

            strSql_t.Append("   FROM dbo.StorageSell A INNER JOIN dbo.WB B ON A.WBID =B.ID");
            strSql_t.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql_t.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql_t.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql_t.Append("  INNER JOIN dbo.StorageTime D ON C.TimeID=D.ID");
            strSql_t.Append("  INNER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql_t.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql_t.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql_t.Append("  and A.ISReturn!=0");
            }  
            if (QVarietyID != "0")
            {
                strSql_t.Append("   AND E.ID = " + QVarietyID);
            }

            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql_t.Append(string.Format(" AND (F.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql_t.Append(string.Format(" AND F.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql_t.Append(string.Format("  AND (F.ID!=B.ID AND F.ID ={0}   or B.ID={0} AND F.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql_t.Append(string.Format(" AND F.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql_t.Append(string.Format(" AND F.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql_t.Append(" AND F.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql_t.Append(" AND F.ID!=B.ID");
                }
            }


            if (Qdtstart != "")
            {
                strSql_t.Append("   AND A.dt_Sell> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql_t.Append("   AND A.dt_Sell < '" + strQdtend + "'");
                }
              
            }
            strSql_t.Append("   GROUP BY E.strName,C.Price_ShiChang");
            DataTable dt2 = SQLHelper.ExecuteDataTable(strSql_t.ToString());

            if (dt != null && dt.Rows.Count != 0 && dt2 != null && dt2.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt), data_t = JsonHelper.ToJson(dt2) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Sell_Detail(HttpContext context)
        {
            string report_type = context.Request.Form["report_type"].ToString();
            string TradeWBID = context.Request.Form["TradeWBID"].ToString();
            string WBID = context.Request.Form["WBID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string TimeID = context.Request.Form["TimeID"].ToString();
            string Price_ShiChang = context.Request.Form["Price_ShiChang"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT H.ID as Dep_ID,H.AccountNumber,H.strName as Dep_Name, F.ID as WBID,F.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName,D.ID as TimeID, D.strName AS TimeName,E.ID as VarietyID, E.strName AS VarietyName,C.Price_ShiChang,CONVERT(varchar(100), dt_Sell, 23) AS dt_Sell,");
            strSql.Append("  VarietyCount,VarietyInterest ,StorageMoney, VarietyMoney");
            strSql.Append("   ,CASE(ISReturn) WHEN 0 THEN '存转销' ELSE '退还存转销' END AS ISReturn");
            strSql.Append("   FROM dbo.StorageSell A INNER JOIN dbo.WB B ON A.WBID =B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql.Append("  INNER JOIN dbo.StorageTime D ON C.TimeID=D.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }
            if (TradeWBID != "")
            {
                strSql.Append("   AND B.ID = " + TradeWBID);
            }
            if (WBID != "")
            {
                strSql.Append("   AND F.ID = " + WBID);
            }
            if (TimeID != "")
            {
                strSql.Append("   AND D.ID = " + TimeID);
            }
            if (VarietyID != "")
            {
                strSql.Append("   AND E.ID = " + VarietyID);
            }
            if (Price_ShiChang != "")
            {
                strSql.Append("   AND C.Price_ShiChang = " + Price_ShiChang);
            }

            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Sell> '" + Qdtstart + "'");

            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    Qdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Sell < '" + Qdtend + "'");
                }
             
            }
            strSql.Append("  order by dt_Sell");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Sell_Chart(HttpContext context)
        {
            string report_type = context.Request.Form["report_type"].ToString();
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string dttype = context.Request.Form["dttype"].ToString();
            string dtyear = context.Request.Form["dtyear"].ToString();
            string dtmonth = context.Request.Form["dtmonth"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT   DATEPART("+dttype+",A.dt_Sell) AS chartdate, ");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyInterest ELSE 0-VarietyInterest END) AS VarietyInterest,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  StorageMoney ELSE 0-StorageMoney END) AS StorageMoney,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyMoney ELSE 0-VarietyMoney END) AS VarietyMoney");
            strSql.Append("   FROM dbo.StorageSell A INNER JOIN dbo.WB B ON A.WBID =B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }

            

            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (F.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (F.ID!=B.ID AND F.ID ={0}   or B.ID={0} AND F.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND F.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND F.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND F.ID!=B.ID");
                }
            }
            if (dttype == "year") { }
            else if (dttype == "week" || dttype == "month")
            {
                DateTime yearFirstDay = new DateTime(Convert.ToInt32(dtyear), 1, 1);
                DateTime yearLastDay = yearFirstDay.AddYears(1);
                strSql.Append(string.Format( "   AND A.dt_Sell>'{0}' and A.dt_Sell<'{1}'" ,yearFirstDay,yearLastDay));
            }
            else {
                DateTime monthFirstDay = new DateTime(Convert.ToInt32(dtyear), Convert.ToInt32(dtmonth), 1);
                DateTime monthLastDay = monthFirstDay.AddMonths(1);
                strSql.Append(string.Format("   AND A.dt_Sell>'{0}' and A.dt_Sell<'{1}'", monthFirstDay, monthLastDay));
            }
            strSql.Append("  GROUP BY DATEPART(" + dttype + ",A.dt_Sell)");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            if (dt != null && dt.Rows.Count != 0 )
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt)};
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        /// <summary>
        /// 获取在兑换中存在的商品种类
        /// </summary>
        /// <param name="context"></param>
        void Get_ExchangeGood(HttpContext context)
        {
            string QWBID = context.Request.Form["QWBID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("    SELECT DISTINCT A.ID,A.strName ");
            strSql.Append("    FROM dbo.Good A INNER JOIN dbo.GoodExchange B ON A.ID=B.GoodID");
          
            if(QWBID!=""&&QWBID!="0")
            {
                strSql.Append(string.Format("  WHERE B.WBID={0}",QWBID));
            }
           

           
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "当前网点不存在商品兑换数据，无法完成统计!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Exchange_Trend(HttpContext context)
        {
            string report_type = context.Request.Form["report_type"].ToString();
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string dttype = context.Request.Form["dttype"].ToString();
            string dtyear = context.Request.Form["dtyear"].ToString();
            string dtmonth = context.Request.Form["dtmonth"].ToString();
            string GoodIDList = context.Request.Form["GoodIDList"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT   DATEPART(" + dttype + ",A.dt_Exchange) AS chartdate,A.GoodID, ");
            strSql.Append("     SUM( CASE ISReturn WHEN  0 THEN  GoodCount ELSE 0-GoodCount END) AS GoodCount");
            strSql.Append("   FROM dbo.GoodExchange A INNER JOIN dbo.WB B ON A.WBID =B.ID  ");
            strSql.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            strSql.Append(string.Format("   AND GoodID IN ({0})",GoodIDList));
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }



            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (F.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (F.ID!=B.ID AND F.ID ={0}   or B.ID={0} AND F.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND F.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND F.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND F.ID!=B.ID");
                }
            }
            if (dttype == "year") { }
            else if (dttype == "week" || dttype == "month")
            {
                DateTime yearFirstDay = new DateTime(Convert.ToInt32(dtyear), 1, 1);
                DateTime yearLastDay = yearFirstDay.AddYears(1);
                strSql.Append(string.Format("   AND A.dt_Exchange>'{0}' and A.dt_Exchange<'{1}'", yearFirstDay, yearLastDay));
            }
            else
            {
                DateTime monthFirstDay = new DateTime(Convert.ToInt32(dtyear), Convert.ToInt32(dtmonth), 1);
                DateTime monthLastDay = monthFirstDay.AddMonths(1);
                strSql.Append(string.Format("   AND A.dt_Exchange>'{0}' and A.dt_Exchange<'{1}'", monthFirstDay, monthLastDay));
            }
            strSql.Append("  GROUP BY DATEPART(" + dttype + ",A.dt_Exchange),GoodID");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Exchange_Catagory(HttpContext context)
        {
            string report_type = context.Request.Form["report_type"].ToString();
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string dttype = context.Request.Form["dttype"].ToString();
            string dtyear = context.Request.Form["dtyear"].ToString();
            string dtmonth = context.Request.Form["dtmonth"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT   DATEPART(" + dttype + ",A.dt_Sell) AS chartdate, ");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyInterest ELSE 0-VarietyInterest END) AS VarietyInterest,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  StorageMoney ELSE 0-StorageMoney END) AS StorageMoney,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyMoney ELSE 0-VarietyMoney END) AS VarietyMoney");
            strSql.Append("   FROM dbo.StorageSell A INNER JOIN dbo.WB B ON A.WBID =B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }



            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (F.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (F.ID!=B.ID AND F.ID ={0}   or B.ID={0} AND F.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND F.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND F.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND F.ID!=B.ID");
                }
            }
            if (dttype == "year") { }
            else if (dttype == "week" || dttype == "month")
            {
                DateTime yearFirstDay = new DateTime(Convert.ToInt32(dtyear), 1, 1);
                DateTime yearLastDay = yearFirstDay.AddYears(1);
                strSql.Append(string.Format("   AND A.dt_Sell>'{0}' and A.dt_Sell<'{1}'", yearFirstDay, yearLastDay));
            }
            else
            {
                DateTime monthFirstDay = new DateTime(Convert.ToInt32(dtyear), Convert.ToInt32(dtmonth), 1);
                DateTime monthLastDay = monthFirstDay.AddMonths(1);
                strSql.Append(string.Format("   AND A.dt_Sell>'{0}' and A.dt_Sell<'{1}'", monthFirstDay, monthLastDay));
            }
            strSql.Append("  GROUP BY DATEPART(" + dttype + ",A.dt_Sell)");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Shopping(HttpContext context)
        {
            string report_type = context.Request.Form["report_type"].ToString();
            string trade_type = context.Request.Form["trade_type"].ToString();
            string QWBID = context.Request.Form["QWBID"].ToString();
            string QVarietyID = context.Request.Form["QVarietyID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT F.ID as WBID,F.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName,D.ID as TimeID, D.strName AS TimeName,E.ID as VarietyID, E.strName AS VarietyName,C.Price_ShiChang,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyCount ELSE 0-VarietyCount END) AS VarietyCount,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyInterest ELSE 0-VarietyInterest END) AS VarietyInterest,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  StorageMoney ELSE 0-StorageMoney END) AS StorageMoney,");
            strSql.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyMoney ELSE 0-VarietyMoney END) AS VarietyMoney");
            strSql.Append("   FROM dbo.StorageShopping A INNER JOIN dbo.WB B ON A.WBID =B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql.Append("  INNER JOIN dbo.StorageTime D ON C.TimeID=D.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }
            if (QVarietyID != "0")
            {
                strSql.Append("   AND E.ID = " + QVarietyID);
            }

            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql.Append(string.Format(" AND (F.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(string.Format("  AND (F.ID!=B.ID AND F.ID ={0}   or B.ID={0} AND F.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql.Append(string.Format(" AND F.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql.Append(string.Format(" AND F.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql.Append(" AND F.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql.Append(" AND F.ID!=B.ID");
                }
            }

            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Sell> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Sell < '" + strQdtend + "'");
                }
               
            }
            strSql.Append("    GROUP BY F.ID,F.strName,B.ID, B.strName,D.ID,D.strName,E.ID, E.strName,C.Price_ShiChang");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            StringBuilder strSql_t = new StringBuilder();
            strSql_t.Append("   SELECT E.strName AS VarietyName,C.Price_ShiChang,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyCount ELSE 0-VarietyCount END) AS VarietyCount,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyInterest ELSE 0-VarietyInterest END) AS VarietyInterest,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  StorageMoney ELSE 0-StorageMoney END) AS StorageMoney,");
            strSql_t.Append("    SUM( CASE ISReturn WHEN  0 THEN  VarietyMoney ELSE 0-VarietyMoney END) AS VarietyMoney");

            strSql_t.Append("   FROM dbo.StorageShopping A INNER JOIN dbo.WB B ON A.WBID =B.ID");
            strSql_t.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql_t.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql_t.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql_t.Append("  INNER JOIN dbo.StorageTime D ON C.TimeID=D.ID");
            strSql_t.Append("  INNER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql_t.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql_t.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql_t.Append("  and A.ISReturn!=0");
            }
            if (QVarietyID != "0")
            {
                strSql_t.Append("   AND E.ID = " + QVarietyID);
            }
            if (QWBID != "0" && QWBID != "")//查询指定的粮食银行
            {
                //strSql.Append("   AND W.ID =" + QWBID);
                if (trade_type == "0")//查询所有
                {
                    strSql_t.Append(string.Format(" AND (F.ID ={0} or B.ID={0})", QWBID));
                }
                else if (trade_type == "1")//本地操作
                {
                    strSql_t.Append(string.Format(" AND F.ID ={0}   AND B.ID={0}", QWBID));
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql_t.Append(string.Format("  AND (F.ID!=B.ID AND F.ID ={0}   or B.ID={0} AND F.ID!=B.ID)", QWBID));
                }
                else if (trade_type == "3")//本行储户在异地操作
                {
                    strSql_t.Append(string.Format(" AND F.ID ={0}   AND B.ID!={0}", QWBID));
                }
                else if (trade_type == "4")//异地储户在本行操作
                {
                    strSql_t.Append(string.Format(" AND F.ID !={0}   AND B.ID={0}", QWBID));
                }
            }
            else
            {//查询所有粮食银行 
                if (trade_type == "0")//查询所有
                {

                }
                else if (trade_type == "1")//本地操作
                {
                    strSql_t.Append(" AND F.ID=B.ID");
                }
                else if (trade_type == "2")//异地操作
                {
                    strSql_t.Append(" AND F.ID!=B.ID");
                }
            }


            if (Qdtstart != "")
            {
                strSql_t.Append("   AND A.dt_Sell> '" + Qdtstart + "'");
            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    string strQdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql_t.Append("   AND A.dt_Sell < '" + strQdtend + "'");
                }
               
              
            }
            strSql_t.Append("   GROUP BY E.strName,C.Price_ShiChang");
            DataTable dt2 = SQLHelper.ExecuteDataTable(strSql_t.ToString());

            if (dt != null && dt.Rows.Count != 0 && dt2 != null && dt2.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt), data_t = JsonHelper.ToJson(dt2) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        void Get_Shopping_Detail(HttpContext context)
        {
            string report_type = context.Request.Form["report_type"].ToString();
            string TradeWBID = context.Request.Form["TradeWBID"].ToString();
            string WBID = context.Request.Form["WBID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string TimeID = context.Request.Form["TimeID"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT H.ID as Dep_ID,H.AccountNumber,H.strName as Dep_Name, F.ID as WBID,F.strName as WBName, B.ID as TradeWBID, B.strName AS TradeWBName,D.ID as TimeID, D.strName AS TimeName,E.ID as VarietyID, E.strName AS VarietyName,C.Price_ShiChang,CONVERT(varchar(100), dt_Sell, 23) AS dt_Sell,");
            strSql.Append("  VarietyCount,VarietyInterest ,StorageMoney, VarietyMoney");
            strSql.Append("   ,CASE(ISReturn) WHEN 0 THEN '换购' ELSE '退还换购' END AS ISReturn");
            strSql.Append("   FROM dbo.StorageShopping A INNER JOIN dbo.WB B ON A.WBID =B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor H ON H.AccountNumber=A.Dep_AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB F ON F.ID=H.WBID");
            strSql.Append("  INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
            strSql.Append("  INNER JOIN dbo.StorageTime D ON C.TimeID=D.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety E ON A.VarietyID=E.ID");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
            if (report_type == "P")
            {
                strSql.Append("  and A.ISReturn=0");
            }
            else if (report_type == "N")
            {
                strSql.Append("  and A.ISReturn!=0");
            }
            if (TradeWBID != "")
            {
                strSql.Append("   AND B.ID = " + TradeWBID);
            }
            if (WBID != "")
            {
                strSql.Append("   AND F.ID = " + WBID);
            }
            if (TimeID != "")
            {
                strSql.Append("   AND D.ID = " + TimeID);
            }
            if (VarietyID != "")
            {
                strSql.Append("   AND E.ID = " + VarietyID);
            }
            if (Qdtstart != "")
            {
                strSql.Append("   AND A.dt_Sell> '" + Qdtstart + "'");

            }
            if (Qdtend != "")
            {
                if (Fun.IsDateTime(Qdtend))
                {
                    Qdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToShortDateString();
                    strSql.Append("   AND A.dt_Sell < '" + Qdtend + "'");
                }
               
               
            }
            strSql.Append("  order by dt_Sell");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = false, msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
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