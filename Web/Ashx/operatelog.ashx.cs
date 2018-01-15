using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Web.Ashx
{
    /// <summary>
    /// operatelog 的摘要说明
    /// </summary>
    public class operatelog : IHttpHandler,IRequiresSessionState
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
                    case "GetUserByWBID": GetUserByWBID(context); break;
                    case "Get_WBUserLog": Get_WBUserLog(context); break;
                }
            }
        }

        void GetUserByWBID(HttpContext context)
        {
            //string WBID = context.Session["WB_ID"].ToString();
            string WBID = context.Request.Form["wbid"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("SELECT * FROM dbo.Users WHERE WB_ID={0}", WBID));

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


        void Get_WBUserLog(HttpContext context)
        {
            string userid = context.Request.Form["userid"].ToString();
           // string WBID = context.Session["WB_ID"].ToString();
            string WBID = context.Request.Form["wbid"].ToString();
            DateTime dtDate =Convert.ToDateTime( context.Request.Form["dtDate"].ToString());
            DateTime dtDateEnd = dtDate.AddDays(1);
            //获取存粮业务日志
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("  SELECT A.AccountNumber, C.strName AS VarietyID,StorageNumber,CONVERT(varchar(100), A.StorageDate, 23) AS StorageDate,");
            strSqlStorage.Append("  A.Price_ShiChang,D.strName AS TimeID,B.strRealName AS UserID");
            strSqlStorage.Append("  FROM  dbo.Dep_StorageInfo A INNER JOIN dbo.Users B ON A.UserID=B.ID");
            strSqlStorage.Append("  INNER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSqlStorage.Append("  INNER JOIN dbo.StorageTime D ON A.TimeID=D.ID");
            strSqlStorage.Append("  WHERE StorageDate>='" + dtDate + "' and StorageDate<'"+dtDateEnd+"'");//2015-01-01格式日期
            strSqlStorage.Append(" and A.WBID=" + WBID);
            if (userid !="0")
            {
                strSqlStorage.Append(" and UserID="+userid);
            }
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            

            //获取兑换信息
            StringBuilder strSqlEx = new StringBuilder();

            strSqlEx.Append("  SELECT  A.ID, B.strRealName AS UserID, Dep_AccountNumber AS AccountNumber , BusinessName,GoodName,GoodPrice,GoodCount,UnitName,VarietyCount,Money_DuiHuan,CASE (ISReturn) WHEN 0 THEN '兑换' ELSE '退还兑换' END AS type");
            strSqlEx.Append("  FROM dbo.GoodExchange A INNER JOIN dbo.Users B ON A.UserID=B.ID");
           // strSqlEx.Append("  where ISReturn=0");//查询没有退还记录的商品 
            strSqlEx.Append("  WHERE dt_Exchange>='" + dtDate + "' and dt_Exchange<'" + dtDateEnd + "'");//2015-01-01格式日期
            strSqlEx.Append(" and A.WBID=" + WBID);
            if (userid != "0")
            {
                strSqlEx.Append(" and UserID=" + userid);
            }
            DataTable dtEx = SQLHelper.ExecuteDataTable(strSqlEx.ToString());
           

            //获取存转销信息
            StringBuilder strSqlSell = new StringBuilder();

            strSqlSell.Append("  SELECT A.ID, B.strRealName AS UserID,A.Dep_AccountNumber AS AccountNumber, BusinessName,CONVERT(varchar(100), dt_Sell, 111) AS dt_Sell,VarietyName,VarietyCount,StorageDate,VarietyInterest,VarietyMoney,CASE (ISReturn) WHEN 0 THEN '存转销' ELSE '退还存转销' END AS type");
            strSqlSell.Append("  FROM dbo.StorageSell A INNER JOIN dbo.Users B ON A.UserID=B.ID");
            //strSqlSell.Append("  where ISReturn=0");//查询没有退还记录的商品 
            strSqlSell.Append("  WHERE dt_Sell>='" + dtDate + "' and dt_Sell<'" + dtDateEnd + "'");//2015-01-01格式日期
            strSqlSell.Append(" and A.WBID=" + WBID);
            if (userid != "0")
            {
                strSqlSell.Append(" and UserID=" + userid);
            }
            DataTable dtSell = SQLHelper.ExecuteDataTable(strSqlSell.ToString());


            //获取产品换购信息
            StringBuilder strSqlShopping = new StringBuilder();

            strSqlShopping.Append("  SELECT A.ID, B.strRealName AS UserID,A.Dep_AccountNumber AS AccountNumber, BusinessName,CONVERT(varchar(100), dt_Sell, 111) AS dt_Sell,VarietyName,VarietyCount,StorageDate,VarietyInterest,VarietyMoney,CASE (ISReturn) WHEN 0 THEN '换购' ELSE '退还换购' END AS type");
            strSqlShopping.Append("  FROM dbo.StorageShopping A INNER JOIN dbo.Users B ON A.UserID=B.ID");
            //strSqlShopping.Append("  where ISReturn=0");//查询没有退还记录的商品 

            strSqlShopping.Append("  WHERE dt_Sell>='" + dtDate + "' and dt_Sell<'" + dtDateEnd + "'");//2015-01-01格式日期
            strSqlShopping.Append(" and A.WBID=" + WBID);
            if (userid != "0")
            {
                strSqlShopping.Append(" and UserID=" + userid);
            }
            DataTable dtShopping = SQLHelper.ExecuteDataTable(strSqlShopping.ToString());

            //获取商品销售信息
            StringBuilder strSqlGoodSell = new StringBuilder();

            strSqlGoodSell.Append("  SELECT  A.ID, B.strRealName AS UserID, Dep_AccountNumber AS AccountNumber , BusinessName,GoodName,GoodPrice,GoodCount,UnitName,GoodValue,CASE (ISReturn) WHEN 0 THEN '商品销售' ELSE '退还销售' END AS type");
            strSqlGoodSell.Append("  FROM dbo.GoodSell A INNER JOIN dbo.Users B ON A.UserID=B.ID");
            // strSqlEx.Append("  where ISReturn=0");//查询没有退还记录的商品 
            strSqlGoodSell.Append("  WHERE dt_Sell>='" + dtDate + "' and dt_Sell<'" + dtDateEnd + "'");//2015-01-01格式日期
            strSqlGoodSell.Append(" and A.WBID=" + WBID);
            if (userid != "0")
            {
                strSqlGoodSell.Append(" and UserID=" + userid);
            }
            DataTable dtGoodSell = SQLHelper.ExecuteDataTable(strSqlGoodSell.ToString());


            //获取商品销售信息
            StringBuilder strSqlIntegral = new StringBuilder();

            strSqlIntegral.Append("  SELECT  A.ID, B.strRealName AS UserID, Dep_AccountNumber AS AccountNumber , BusinessName,GoodName,GoodPrice,GoodCount,UnitName,GoodValue,integral_Change");
            strSqlIntegral.Append("  FROM dbo.GoodExchangeIntegral A INNER JOIN dbo.Users B ON A.UserID=B.ID");
            // strSqlEx.Append("  where ISReturn=0");//查询没有退还记录的商品 
            strSqlIntegral.Append("  WHERE dt_Integral>='" + dtDate + "' and dt_Integral<'" + dtDateEnd + "'");//2015-01-01格式日期
            strSqlIntegral.Append(" and A.WBID=" + WBID);
            if (userid != "0")
            {
                strSqlIntegral.Append(" and UserID=" + userid);
            }
            DataTable dtIntegral = SQLHelper.ExecuteDataTable(strSqlIntegral.ToString());

            var res = new { storage = JsonHelper.ToJson(dtStorage), exchange = JsonHelper.ToJson(dtEx), sell = JsonHelper.ToJson(dtSell), shopping = JsonHelper.ToJson(dtShopping), goodsell = JsonHelper.ToJson(dtGoodSell), Integral = JsonHelper.ToJson(dtIntegral) };

            context.Response.Write(JsonHelper.ToJson(res));
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