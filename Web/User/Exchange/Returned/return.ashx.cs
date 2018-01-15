using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Web.User.Exchange.Returned
{
    /// <summary>
    /// _return 的摘要说明
    /// </summary>
    public class _return : IHttpHandler,IRequiresSessionState
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
                    case "getPage": getPage(context); break;
                    case "get_rExchange": get_rExchange(context); break;
                    case "get_rGoodSell": get_rGoodSell(context); break;
                    case "Return_GoodExchange": Return_GoodExchange(context); break;
                    case "Return_GoodSell": Return_GoodSell(context); break;
                    case "get_rSell": get_rSell(context); break;
                    case "Return_StorageSell": Return_StorageSell(context); break;
                    case "get_rShopping": get_rShopping(context); break;
                    case "Return_StorageShopping": Return_StorageShopping(context); break;
                }
            }

        }


        void getPage(HttpContext context)
        {
            var res = new { ID = 1, Name = "name1" };
            string strName = res.Name;
            string str1 = context.Request.Form["stu"].ToString();

            DataTable dt = JsonHelper.JsonToDataTable(str1);
            context.Response.Write("Error");
        }

        //返回销售商品列表
        void get_rGoodSell(HttpContext context)
        {

            bool ISHQ = Convert.ToBoolean(context.Session["ISHQ"]);
            string date_begin = "";//开始查询日期
            string date_end = "";//结束查询日期
            if (ISHQ)
            {
                date_begin = context.Request.Form["date_begin"].ToString();
                date_end = Convert.ToDateTime(context.Request.Form["date_end"]).AddDays(1).ToString();
            }
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            //查询已经有退换的产品列表 
            string ReturnList = GetReturnGoodSellList(ISHQ, AccountNumber, date_begin, date_end);


            //获取储户的今日兑换信息
            StringBuilder strSql = new StringBuilder();

            strSql.Append("  SELECT ID, BusinessName,GoodName,GoodPrice,GoodCount,UnitName,GoodValue,CONVERT(varchar(100), dt_Sell, 23) as dt_Sell");
            strSql.Append("  FROM dbo.GoodSell where 1=1");
            if (!ISHQ)//非总部管理员只能查看当天的兑换信息
            {
                strSql.Append("  and DATEDIFF(DAY,dt_Sell,GETDATE())<1");
            }
            else
            {
                strSql.Append(string.Format(" and dt_Sell>'{0}' AND dt_Sell<'{1}'", date_begin, date_end));
            }

            strSql.Append("  and ISReturn=0");//查询没有退还记录的商品 
            strSql.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
          
            if (ReturnList != "")
            {
                strSql.Append("  and ID not in (" + ReturnList + ")");//排除已经做过退换的记录
            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt == null || dt.Rows.Count == 0)
            {
                var msg = "储户" + AccountNumber + "今天没有商品购买记录!";
                if (ISHQ)
                {
                    msg = "当前期限内没有查询到储户" + AccountNumber + "的商品购买记录!";
                }
                var res = new { state = "error", msg = msg };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {

                var res = new { state = "success", msg = "查询数据成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }
        private string GetReturnGoodSellList(bool ISHQ, string AccountNumber, string date_begin, string date_end)
        {
            StringBuilder strSqlISReturn = new StringBuilder();
            strSqlISReturn.Append("  SELECT ID, ISReturn");
            strSqlISReturn.Append("  FROM dbo.GoodSell where 1=1");
            if (!ISHQ)
            {
                strSqlISReturn.Append("  and DATEDIFF(DAY,dt_Sell,GETDATE())<1");
            }
            else
            {
                strSqlISReturn.Append(string.Format(" and dt_Sell>'{0}' AND dt_Sell<'{1}'", date_begin, date_end));
            }

            strSqlISReturn.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
            strSqlISReturn.Append("  and ISReturn!=0");
            DataTable dtExISReturn = SQLHelper.ExecuteDataTable(strSqlISReturn.ToString());
            string strReturnList = "";
            if (dtExISReturn != null && dtExISReturn.Rows.Count != 0)
            {
                for (int i = 0; i < dtExISReturn.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        strReturnList = dtExISReturn.Rows[i]["ISReturn"].ToString();
                    }
                    else
                    {
                        strReturnList = strReturnList + "," + dtExISReturn.Rows[i]["ISReturn"].ToString();
                    }
                }
            }
            return strReturnList;
        }


        /// <summary>
        /// 退还销售商品
        /// </summary>
        /// <param name="context"></param>
        void Return_GoodSell(HttpContext context)
        {
            //添加兑换记录信息
            // string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();//交易编号
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//获取该用户新的业务编号
            string[] IDList = context.Request.QueryString["IDList"].ToString().Split(',');


            string BusinessNOList = "";//所有的兑换列表的集合

            StringBuilder sqlEx = new StringBuilder();
            StringBuilder sqlO_Log = new StringBuilder();
            StringBuilder strGoodStorage = new StringBuilder();
            for (int i = 0; i < IDList.Length; i++)
            {
                string ID = IDList[i];
                if (i != 0)
                {
                    BusinessNO = Fun.ConvertIntToString(Convert.ToInt32(BusinessNO) + 1, 4);
                }
                if (BusinessNOList == "")
                {
                    BusinessNOList = BusinessNO;
                }
                else
                {
                    BusinessNOList = BusinessNOList + "|" + BusinessNO;
                }
                string strGUID = Fun.getGUID();//防伪码

                string sqlGoodSell = string.Format(" select * from GoodSell where ID={0}",ID);
                DataTable dt = SQLHelper.ExecuteDataTable(sqlGoodSell);
                if (dt != null && dt.Rows.Count != 0)
                {
                    string ISReturn = dt.Rows[0]["ID"].ToString();//需要退换的商品编号
                    string Dep_AccountNumber = dt.Rows[0]["Dep_AccountNumber"].ToString();

                    string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                    DateTime.Now.Day.ToString() + Dep_AccountNumber + BusinessNO;


                    string Dep_Name = dt.Rows[0]["Dep_Name"].ToString();//储户姓名
                    string WBID = context.Session["WB_ID"].ToString();//操作网点编号
                    string WBID_Dep = dt.Rows[0]["WBID"].ToString();//储户网点编号
                    string UserID = context.Session["ID"].ToString();//营业员编号
                    string BusinessName = "退换商品销售";
                    string BusinessName_Log = "14";
                    string WBWareHouseID = dt.Rows[0]["WBWareHouseID"].ToString();
                    string GoodID = dt.Rows[0]["GoodID"].ToString();
                    string GoodName = dt.Rows[0]["GoodName"].ToString();
                    string SpecName = dt.Rows[0]["SpecName"].ToString();
                    string UnitName = dt.Rows[0]["UnitName"].ToString();
                    string GoodCount = dt.Rows[0]["GoodCount"].ToString();
                    string GoodPrice = dt.Rows[0]["GoodPrice"].ToString();
                    string GoodValue = dt.Rows[0]["GoodValue"].ToString();
                   
                    string dt_Sell = DateTime.Now.ToString();
                    
                    #region 退还兑换记录
                  
                    sqlEx.Append("  insert into [GoodSell] (");
                    sqlEx.Append("SerialNumber,strGUID,BusinessNO,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,GoodValue,WBWareHouseID,ISReturn,dt_Sell)");
                    sqlEx.Append(" values (");

                    sqlEx.Append(string.Format("'{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}',{8},'{9}','{10}','{11}',{12},{13},{14},{15},{16},'{17}')", SerialNumber, strGUID, BusinessNO, AccountNumber, Dep_Name, WBID, UserID, BusinessName, GoodID, GoodName, SpecName, UnitName, GoodCount, GoodPrice, GoodValue, WBWareHouseID, ISReturn, DateTime.Now.ToString()));
                    #endregion


                    #region 日志记录                  
                    sqlO_Log.Append("  insert into [Dep_OperateLog] (");
                    sqlO_Log.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
                    sqlO_Log.Append(" values (");

                    sqlO_Log.Append(string.Format("{0},{1},'{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},{11},'{12}','{13}','{14}',{15})", WBID, UserID, AccountNumber, BusinessNO, BusinessName_Log, GoodID, UnitName, GoodPrice, "-" + GoodCount, 0, "-" + GoodValue, 0, DateTime.Now.ToString(), GoodName, UnitName, 0));
                    #endregion


                    strGoodStorage.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore+{0} WHERE GoodID={1} and WBWareHouseID={2} AND WBID={3}", GoodCount, GoodID, WBWareHouseID, WBID_Dep));
                  

                }

            }

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlEx.ToString());//添加兑换交易记录

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlO_Log.ToString());//添加兑换日志记录

                    //修改仓库库存信息
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strGoodStorage.ToString());
                    tran.Commit();

                    var res = new { state = "success", msg = "退还商品成功!", BNList = BusinessNOList };

                    context.Response.Write(JsonHelper.ToJson(res));

                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "error", msg = "退还商品失败!", BNList = BusinessNOList };

                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion
         
        }

        //返回兑换商品列表
        void get_rExchange(HttpContext context)
        {

            bool ISHQ = Convert.ToBoolean(context.Session["ISHQ"]);
            string date_begin = "";//开始查询日期
            string date_end = "";//结束查询日期
            if (ISHQ) {
                date_begin = context.Request.Form["date_begin"].ToString();
                date_end =Convert.ToDateTime( context.Request.Form["date_end"]).AddDays(1).ToString();
            }
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            //查询已经有退换的产品列表 
            string ReturnList = GetReturnEXList(ISHQ,AccountNumber,date_begin,date_end);


            //获取储户的今日兑换信息
            StringBuilder strSql = new StringBuilder();

            strSql.Append("  SELECT ID, BusinessName,GoodName,GoodPrice,GoodCount,UnitName,VarietyCount,Money_DuiHuan,CONVERT(varchar(100), dt_Exchange, 23) as dt_Exchange");
            strSql.Append("  FROM dbo.GoodExchange where 1=1");
            if (!ISHQ)//非总部管理员只能查看当天的兑换信息
            {
                strSql.Append("  and DATEDIFF(DAY,dt_Exchange,GETDATE())<1");
            }
            else
            {
                strSql.Append(string.Format(" and dt_Exchange>'{0}' AND dt_Exchange<'{1}'", date_begin, date_end));
            }

            strSql.Append("  and ISReturn=0");//查询没有退还记录的商品 
            strSql.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
            strSql.Append(" AND ID NOT IN (SELECT GoodExchangeID FROM dbo.SA_Exchange WHERE Dep_AN='" + AccountNumber + "')");//排除已经被结算的兑换
            if (ReturnList != "")
            {
                strSql.Append("  and ID not in (" + ReturnList + ")");//排除已经做过退换的记录
            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt == null || dt.Rows.Count == 0)
            {
                var msg = "储户" + AccountNumber + "今天没有兑换记录!";
                if (ISHQ) {
                    msg = "当前期限内没有查询到储户"+AccountNumber+"的兑换记录!";
                }
                var res = new { state = "error", msg = msg};
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
               
                var res = new { state = "success", msg ="查询数据成功!" ,data=JsonHelper.ToJson(dt)};
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }
        //已经
        private string GetReturnEXList(bool ISHQ,string AccountNumber,string date_begin,string date_end)
        {
            StringBuilder strSqlISReturn = new StringBuilder();
            strSqlISReturn.Append("  SELECT ID, ISReturn");
            strSqlISReturn.Append("  FROM dbo.GoodExchange where 1=1");
            if (!ISHQ)
            {
                strSqlISReturn.Append("  and DATEDIFF(DAY,dt_Exchange,GETDATE())<1");
            }
            else {
                strSqlISReturn.Append( string.Format(" and dt_Exchange>'{0}' AND dt_Exchange<'{1}'",date_begin,date_end));
            }

            strSqlISReturn.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
            strSqlISReturn.Append("  and ISReturn!=0");
            DataTable dtExISReturn = SQLHelper.ExecuteDataTable(strSqlISReturn.ToString());
            string strReturnList = "";
            if (dtExISReturn != null && dtExISReturn.Rows.Count != 0)
            {
                for (int i = 0; i < dtExISReturn.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        strReturnList = dtExISReturn.Rows[i]["ISReturn"].ToString();
                    }
                    else
                    {
                        strReturnList = strReturnList + "," + dtExISReturn.Rows[i]["ISReturn"].ToString();
                    }
                }
            }
            return strReturnList;
        }


        /// <summary>
        /// 退还兑换商品
        /// </summary>
        /// <param name="context"></param>
        void Return_GoodExchange(HttpContext context)
        {
            //添加兑换记录信息
            // string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();//交易编号
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//获取该用户新的业务编号
            string[] IDList = context.Request.QueryString["IDList"].ToString().Split(',');


            string BusinessNOList = "";//所有的兑换列表的集合
            for (int i = 0; i < IDList.Length; i++)
            {
                string ID = IDList[i];
                if (i != 0)
                {
                    BusinessNO = Fun.ConvertIntToString(Convert.ToInt32(BusinessNO) + 1, 4);
                }
                if (BusinessNOList == "")
                {
                    BusinessNOList = BusinessNO;
                }
                else
                {
                    BusinessNOList = BusinessNOList + "|" + BusinessNO;
                }
                string strGUID = Fun.getGUID();//防伪码

                StringBuilder strSqlGoodExchange = new StringBuilder();
                strSqlGoodExchange.Append("SELECT B.VarietyID, A.ID,SerialNumber,Dep_SID,Dep_AccountNumber,Dep_Name,A.WBID,A.UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn,WBWareHouseID ");
                strSqlGoodExchange.Append("  FROM dbo.GoodExchange A LEFT OUTER JOIN dbo.Dep_StorageInfo B ON A.Dep_SID=B.ID ");
                strSqlGoodExchange.Append(" where A.ID=@ID ");
                SqlParameter[] parametersGoodExchange = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
                parametersGoodExchange[0].Value = ID;

                DataTable dt = SQLHelper.ExecuteDataTable(strSqlGoodExchange.ToString(), parametersGoodExchange);
                if (dt != null && dt.Rows.Count != 0)
                {
                    string ISReturn = dt.Rows[0]["ID"].ToString();//需要退换的商品编号
                    string Dep_SID = dt.Rows[0]["Dep_SID"].ToString();
                    string Dep_AccountNumber = dt.Rows[0]["Dep_AccountNumber"].ToString();

                    string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                    DateTime.Now.Day.ToString() + Dep_AccountNumber + BusinessNO;


                    string Dep_Name = dt.Rows[0]["Dep_Name"].ToString();//储户姓名
                    string WBID = context.Session["WB_ID"].ToString();//操作网点编号
                    string WBID_Dep = dt.Rows[0]["WBID"].ToString();//储户网点编号
                    string UserID = context.Session["ID"].ToString();//营业员编号
                    string BusinessName = "退换兑换";
                    string VarietyID = dt.Rows[0]["VarietyID"].ToString();
                    string WBWareHouseID = dt.Rows[0]["WBWareHouseID"].ToString();
                    string GoodID = dt.Rows[0]["GoodID"].ToString();
                    string GoodName = dt.Rows[0]["GoodName"].ToString();
                    string SpecName = dt.Rows[0]["SpecName"].ToString();
                    string UnitName = dt.Rows[0]["UnitName"].ToString();
                    string GoodCount = dt.Rows[0]["GoodCount"].ToString();
                    string GoodPrice = dt.Rows[0]["GoodPrice"].ToString();
                    string VarietyCount = dt.Rows[0]["VarietyCount"].ToString();
                    string VarietyInterest = dt.Rows[0]["VarietyInterest"].ToString();
                    string Money_DuiHuan = dt.Rows[0]["Money_DuiHuan"].ToString();
                    string Money_YouHui = dt.Rows[0]["Money_YouHui"].ToString();
                    string dt_Exchange = DateTime.Now.ToString();
                    string JieCun_Last = "";//最近一期的结存数量

                    //获取上次的结存数量
                    StringBuilder strSqlEx = new StringBuilder();
                    strSqlEx.Append("  SELECT TOP 1 JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total");
                    strSqlEx.Append("  FROM dbo.GoodExchange");
                    strSqlEx.Append("  WHERE Dep_SID=" + Dep_SID);
                    strSqlEx.Append("  ORDER BY ID DESC");
                    strSqlEx.Append("  ");
                    DataTable dtEx = SQLHelper.ExecuteDataTable(strSqlEx.ToString());
                    if (dtEx != null && dtEx.Rows.Count != 0)
                    {
                        JieCun_Last = dtEx.Rows[0]["JieCun_Now"].ToString();
                    }

                    string JieCun_Now = "";
                    double numJieCun_Now = Convert.ToDouble(JieCun_Last) + Convert.ToDouble(VarietyCount);
                    JieCun_Now = numJieCun_Now.ToString();//新的结存数量

                    string JieCun_Raw = dt.Rows[0]["JieCun_Raw"].ToString();
                    double numJieCun_Total = Convert.ToDouble(dt.Rows[0]["JieCun_Total"]) - Convert.ToDouble(VarietyCount);
                    string JieCun_Total = numJieCun_Total.ToString();

                    #region 退还兑换记录
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into [GoodExchange] (");
                    strSql.Append("SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn)");
                    strSql.Append(" values (");
                    strSql.Append("@SerialNumber,@strGUID,@BusinessNO,@Dep_SID,@Dep_AccountNumber,@Dep_Name,@WBID,@UserID,@BusinessName,@GoodID,@GoodName,@SpecName,@UnitName,@GoodCount,@GoodPrice,@VarietyCount,@VarietyInterest,@Money_DuiHuan,@Money_YouHui,@dt_Exchange,@JieCun_Last,@JieCun_Now,@JieCun_Raw,@JieCun_Total,@ISReturn)");
                    strSql.Append(";select @@IDENTITY");
                    SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_SID", SqlDbType.Int,4),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodID", SqlDbType.Int,4),
					new SqlParameter("@GoodName", SqlDbType.NVarChar,50),
					new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodCount", SqlDbType.Decimal,9),
					new SqlParameter("@GoodPrice", SqlDbType.Decimal,9),
					new SqlParameter("@VarietyCount", SqlDbType.Decimal,9),
					new SqlParameter("@VarietyInterest", SqlDbType.Decimal,9),
					new SqlParameter("@Money_DuiHuan", SqlDbType.Decimal,9),
					new SqlParameter("@Money_YouHui", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Exchange", SqlDbType.DateTime),
					new SqlParameter("@JieCun_Last", SqlDbType.Decimal,9),
					new SqlParameter("@JieCun_Now", SqlDbType.Decimal,9),
					new SqlParameter("@JieCun_Raw", SqlDbType.Decimal,9),
					new SqlParameter("@JieCun_Total", SqlDbType.Decimal,9),
					new SqlParameter("@ISReturn", SqlDbType.Int,4)};
                    parameters[0].Value = SerialNumber;
                    parameters[1].Value = strGUID;
                    parameters[2].Value = BusinessNO;
                    parameters[3].Value = Dep_SID;
                    parameters[4].Value = Dep_AccountNumber;
                    parameters[5].Value = Dep_Name;
                    parameters[6].Value = WBID;
                    parameters[7].Value = UserID;
                    parameters[8].Value = BusinessName;
                    parameters[9].Value = GoodID;
                    parameters[10].Value = GoodName;
                    parameters[11].Value = SpecName;
                    parameters[12].Value = UnitName;
                    parameters[13].Value = GoodCount;
                    parameters[14].Value = GoodPrice;
                    parameters[15].Value = VarietyCount;
                    parameters[16].Value = VarietyInterest;
                    parameters[17].Value = Money_DuiHuan;
                    parameters[18].Value = Money_YouHui;
                    parameters[19].Value = DateTime.Now;
                    parameters[20].Value = JieCun_Last;
                    parameters[21].Value = JieCun_Now;
                    parameters[22].Value = JieCun_Raw;
                    parameters[23].Value = JieCun_Total;
                    parameters[24].Value = ISReturn;//退换兑换的编号
                    #endregion


                    #region 日志记录
                    StringBuilder strSqlOperateLog = new StringBuilder();
                    strSqlOperateLog.Append("insert into [Dep_OperateLog] (");
                    strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
                    strSqlOperateLog.Append(" values (");
                    strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade,@VarietyName,@UnitName,@Dep_SID)");
                    strSqlOperateLog.Append(";select @@IDENTITY");
                    SqlParameter[] parametersOperateLog = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyID", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitID", SqlDbType.NVarChar,50),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
                    new SqlParameter("@GoodCount", SqlDbType.Decimal,9),
					new SqlParameter("@Count_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Count_Balance", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime),
					new SqlParameter("@VarietyName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_SID", SqlDbType.Int,4)};
                    parametersOperateLog[0].Value = WBID;
                    parametersOperateLog[1].Value = UserID;
                    parametersOperateLog[2].Value = Dep_AccountNumber;
                    parametersOperateLog[3].Value = BusinessNO;
                    parametersOperateLog[4].Value = "6";//1:存入 2：兑换  3:存转销 4: 提取
                    parametersOperateLog[5].Value = VarietyID;
                    parametersOperateLog[6].Value = UnitName;
                    parametersOperateLog[7].Value = GoodPrice;
                    parametersOperateLog[8].Value = "-" + GoodCount;
                    parametersOperateLog[9].Value = "-" + VarietyCount;
                    double Money_Trade = Convert.ToDouble(GoodPrice) * Convert.ToDouble(GoodCount);
                    parametersOperateLog[10].Value = Math.Round(Money_Trade, 2);

                    object objVarietyID = SQLHelper.ExecuteScalar("SELECT VarietyID  FROM  dbo.Dep_StorageInfo WHERE ID=" + Dep_SID.ToString());
                    if (objVarietyID != null && objVarietyID.ToString() != "")
                    {

                        object objBalance = SQLHelper.ExecuteScalar("  SELECT SUM( StorageNumber)  AS StorageNumber  FROM dbo.Dep_StorageInfo WHERE AccountNumber='" + Dep_AccountNumber + "' AND VarietyID=" + objVarietyID.ToString());
                        string Count_Balance = "0";
                        if (objBalance != null && objBalance.ToString() != "")
                        {
                            Count_Balance = Math.Round(Convert.ToDouble(objBalance) + Convert.ToDouble(VarietyCount), 2).ToString();
                        }

                        parametersOperateLog[11].Value = Count_Balance;
                    }
                    else
                    {
                        parametersOperateLog[10].Value = JieCun_Now;
                    }
                    parametersOperateLog[12].Value = DateTime.Now;
                    parametersOperateLog[13].Value = GoodName;
                    parametersOperateLog[14].Value = UnitName;
                    parametersOperateLog[15].Value = Dep_SID;
                    #endregion

                    #region 数据处理
                    using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
                    {
                        try
                        {
                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);//添加兑换交易记录

                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加兑换日志记录

                            //修改储户的商品结存
                            string strSqlDep_JieCun = " UPDATE dbo.Dep_StorageInfo  SET StorageNumber=StorageNumber+" + VarietyCount + " WHERE ID=" + Dep_SID;
                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep_JieCun.ToString());
                            ////修改仓库库存信息
                            string strGoodStorage = string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore+{0} WHERE GoodID={1} and WBWareHouseID={2} AND WBID={3}", GoodCount, GoodID, WBWareHouseID, WBID_Dep);
                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strGoodStorage.ToString());
                            tran.Commit();


                        }
                        catch
                        {
                            tran.Rollback();
                            context.Response.Write("Error");
                        }
                    }
                    #endregion

                }

            }
            var res = new { state = "success", msg = "退还兑换成功!", BNList = BusinessNOList };

            context.Response.Write(JsonHelper.ToJson(res));
        }

        void get_rSell(HttpContext context) {
            bool ISHQ = Convert.ToBoolean(context.Session["ISHQ"]);
            string date_begin = "";//开始查询日期
            string date_end = "";//结束查询日期
            if (ISHQ)
            {
                date_begin = context.Request.Form["date_begin"].ToString();
                date_end = Convert.ToDateTime(context.Request.Form["date_end"]).AddDays(1).ToString();
            }
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            //查询已经有退换的产品列表 
            string ReturnList = GetReturnSellList(ISHQ, AccountNumber, date_begin, date_end);
            //获取储户的今日存转销售信息
            StringBuilder strSql = new StringBuilder();

            strSql.Append("  SELECT ID, BusinessName,CONVERT(varchar(100), dt_Sell, 111) AS dt_Sell, StorageDate,VarietyName,VarietyCount, Price_JieSuan,VarietyMoney,StorageDate,VarietyInterest, StorageMoney,Money_Earn");
            strSql.Append("  FROM dbo.StorageSell where 1=1");
            if (!ISHQ)
            {
                strSql.Append("  and DATEDIFF(DAY,dt_Sell,GETDATE())<1");
            }
            else
            {
                strSql.Append(string.Format(" and dt_Sell>'{0}' AND dt_Sell<'{1}'", date_begin, date_end));
            }
            strSql.Append("  and ISReturn=0");//查询没有退还记录的商品 
            strSql.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
            strSql.Append("  and  ID NOT IN (SELECT StorageSellID  FROM dbo.SA_Sell WHERE Dep_AN='" + AccountNumber + "')");//排除已经被结算的存转销

            if (ReturnList != "")
            {
                strSql.Append("  and ID not in (" + ReturnList + ")");//排除已经做过退换的记录
            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                var msg = "储户" + AccountNumber + "今天没有存转销记录!";
                if (ISHQ)
                {
                    msg = "当前期限内没有查询到储户" + AccountNumber + "的存转销记录!";
                }
                var res = new { state = "error", msg = msg };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {

                var res = new { state = "success", msg = "查询数据成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        /// <summary>
        /// 已经退还的存转销列表
        /// </summary>
        /// <param name="ISHQ"></param>
        /// <param name="AccountNumber"></param>
        /// <param name="date_begin"></param>
        /// <param name="date_end"></param>
        /// <returns></returns>
        private string GetReturnSellList(bool ISHQ, string AccountNumber, string date_begin, string date_end)
        {
            StringBuilder strSqlISReturn = new StringBuilder();
            strSqlISReturn.Append("  SELECT ID, ISReturn");
            strSqlISReturn.Append("  FROM dbo.StorageSell where 1=1");
            if (!ISHQ)
            {
                strSqlISReturn.Append("  and DATEDIFF(DAY,dt_Sell,GETDATE())<1");
            }
            else {
                strSqlISReturn.Append(string.Format(" and dt_Sell>'{0}' AND dt_Sell<'{1}'", date_begin, date_end));
            }

            strSqlISReturn.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
            strSqlISReturn.Append("  and ISReturn!=0");
            DataTable dtExISReturn = SQLHelper.ExecuteDataTable(strSqlISReturn.ToString());
            string strReturnList = "";
            if (dtExISReturn != null && dtExISReturn.Rows.Count != 0)
            {
                for (int i = 0; i < dtExISReturn.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        strReturnList = dtExISReturn.Rows[i]["ISReturn"].ToString();
                    }
                    else
                    {
                        strReturnList = strReturnList + "," + dtExISReturn.Rows[i]["ISReturn"].ToString();
                    }
                }
            }
            return strReturnList;
        }

        /// <summary>
        /// 退还存转销
        /// </summary>
        /// <param name="context"></param>
        void Return_StorageSell(HttpContext context)
        {
            //添加兑换记录信息
            string IDList = context.Request.QueryString["IDList"].ToString();

            StringBuilder strSqlSelect = new StringBuilder();
            strSqlSelect.Append("select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,UnitName,VarietyID,VarietyName,VarietyCount,VarietyMoney,VarietyInterest,StorageDate,CurrentRate,EarningRate,StorageFee,StorageMoney,Price_JieSuan,Money_Earn,dt_Sell,JieCun_Last,JieCun_Now,ISReturn ");
            strSqlSelect.Append(" FROM [StorageSell] ");
            strSqlSelect.Append(" where ID=@ID ");
            SqlParameter[] parametersSelect = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parametersSelect[0].Value = IDList;

            DataTable dt = SQLHelper.ExecuteDataTable(strSqlSelect.ToString(), parametersSelect);
            if (dt == null || dt.Rows.Count == 0)
            {
                context.Response.Write("Error");
                return;
            }
            string Dep_AccountNumber = dt.Rows[0]["Dep_AccountNumber"].ToString();
            //string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();//业务编号
            string BusinessNO = common.GetNewBusinessNO_Dep(Dep_AccountNumber);
            string strGUID = Fun.getGUID();//防伪码
            string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + Dep_AccountNumber + BusinessNO;


            string Dep_SID = dt.Rows[0]["Dep_SID"].ToString();
            string Dep_Name = dt.Rows[0]["Dep_Name"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = dt.Rows[0]["UserID"].ToString();
            string BusinessName = "退还存转销";
            string UnitName = dt.Rows[0]["UnitName"].ToString();
            string VarietyID = dt.Rows[0]["VarietyID"].ToString();
            string VarietyName = dt.Rows[0]["VarietyName"].ToString();
            string VarietyCount = dt.Rows[0]["VarietyCount"].ToString();
            string VarietyMoney = dt.Rows[0]["VarietyMoney"].ToString();
            string VarietyInterest = dt.Rows[0]["VarietyInterest"].ToString();
            string StorageDate = dt.Rows[0]["StorageDate"].ToString();
            string CurrentRate = dt.Rows[0]["CurrentRate"].ToString();
            string EarningRate = dt.Rows[0]["EarningRate"].ToString();
            string StorageFee = dt.Rows[0]["StorageFee"].ToString();
            string StorageMoney = dt.Rows[0]["StorageMoney"].ToString();
            string Price_JieSuan = dt.Rows[0]["Price_JieSuan"].ToString();
            string Money_Earn = dt.Rows[0]["Money_Earn"].ToString();
            string dt_Sell = DateTime.Now.ToString();
            // string JieCun_Last = dt.Rows[0]["JieCun_Now"].ToString();
            string JieCun_Last = SQLHelper.ExecuteScalar(" SELECT TOP 1 JieCun_Now FROM dbo.StorageSell WHERE Dep_SID=" + Dep_SID + " ORDER BY dt_Sell DESC").ToString();//查询上一期最近的结存情况
            double numJieCun_Now = Convert.ToDouble(JieCun_Last) + Convert.ToDouble(VarietyCount);
            string JieCun_Now = numJieCun_Now.ToString();
            string ISReturn = dt.Rows[0]["ID"].ToString();

            //写入存转销记录
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [StorageSell] (");
            strSql.Append("SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,UnitName,VarietyID,VarietyName,VarietyCount,VarietyMoney,VarietyInterest,StorageDate,CurrentRate,EarningRate,StorageFee,StorageMoney,Price_JieSuan,Money_Earn,dt_Sell,JieCun_Last,JieCun_Now,ISReturn)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@strGUID,@BusinessNO,@Dep_SID,@Dep_AccountNumber,@Dep_Name,@WBID,@UserID,@BusinessName,@UnitName,@VarietyID,@VarietyName,@VarietyCount,@VarietyMoney,@VarietyInterest,@StorageDate,@CurrentRate,@EarningRate,@StorageFee,@StorageMoney,@Price_JieSuan,@Money_Earn,@dt_Sell,@JieCun_Last,@JieCun_Now,@ISReturn)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_SID", SqlDbType.Int,4),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@VarietyName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyCount", SqlDbType.Decimal,9),
					new SqlParameter("@VarietyMoney", SqlDbType.Decimal,9),
					new SqlParameter("@VarietyInterest", SqlDbType.Decimal,9),
					new SqlParameter("@StorageDate", SqlDbType.Int,4),
					new SqlParameter("@CurrentRate", SqlDbType.NChar,10),
					new SqlParameter("@EarningRate", SqlDbType.Decimal,9),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@StorageMoney", SqlDbType.Decimal,9),
					new SqlParameter("@Price_JieSuan", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Earn", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Sell", SqlDbType.DateTime),
					new SqlParameter("@JieCun_Last", SqlDbType.Decimal,9),
					new SqlParameter("@JieCun_Now", SqlDbType.Decimal,9),
					new SqlParameter("@ISReturn", SqlDbType.Int,4)};
            parameters[0].Value = SerialNumber;
            parameters[1].Value = strGUID;
            parameters[2].Value = BusinessNO;
            parameters[3].Value = Dep_SID;
            parameters[4].Value = Dep_AccountNumber;
            parameters[5].Value = Dep_Name;
            parameters[6].Value = WBID;
            parameters[7].Value = UserID;
            parameters[8].Value = BusinessName;
            parameters[9].Value = UnitName;
            parameters[10].Value = VarietyID;
            parameters[11].Value = VarietyName;
            parameters[12].Value = VarietyCount;
            parameters[13].Value = VarietyMoney;
            parameters[14].Value = VarietyInterest;
            parameters[15].Value = StorageDate;
            parameters[16].Value = CurrentRate;
            parameters[17].Value = EarningRate;
            parameters[18].Value = StorageFee;
            parameters[19].Value = StorageMoney;
            parameters[20].Value = Price_JieSuan;
            parameters[21].Value = Money_Earn;
            parameters[22].Value = dt_Sell;
            parameters[23].Value = JieCun_Last;
            parameters[24].Value = JieCun_Now;
            parameters[25].Value = ISReturn;



            #region 存转销日志记录
            //添加交易记录

            string Price = Price_JieSuan;//价格
            string Count_Trade = VarietyCount;//存储数量
            string Money_Trade = VarietyMoney;
            string Count_Balance = JieCun_Now.ToString();//当前产品结存
            //查找当前用户的当前产品的总的结存
            object objBalance = SQLHelper.ExecuteScalar("  SELECT SUM( StorageNumber)  AS StorageNumber  FROM dbo.Dep_StorageInfo WHERE AccountNumber='" + Dep_AccountNumber + "' AND VarietyID=" + VarietyID);
            if (objBalance != null && objBalance.ToString() != "")
            {
                // Count_Balance = objBalance.ToString();
                Count_Balance = (Convert.ToDouble(objBalance) + Convert.ToDouble(VarietyCount)).ToString();//现在总共剩余的结存
            }

            string UnitID = UnitName;

            StringBuilder strSqlOperateLog = new StringBuilder();
            strSqlOperateLog.Append("insert into [Dep_OperateLog] (");
            strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
            strSqlOperateLog.Append(" values (");
            strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade,@VarietyName,@UnitName,@Dep_SID)");
            strSqlOperateLog.Append(";select @@IDENTITY");
            SqlParameter[] parametersOperateLog = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyID", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitID", SqlDbType.NVarChar,50),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
                    new SqlParameter("@GoodCount", SqlDbType.Decimal,9),
					new SqlParameter("@Count_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Count_Balance", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime),
					new SqlParameter("@VarietyName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_SID", SqlDbType.Int,4)};
            parametersOperateLog[0].Value = WBID;
            parametersOperateLog[1].Value = UserID;
            parametersOperateLog[2].Value = Dep_AccountNumber;
            parametersOperateLog[3].Value = BusinessNO;
            parametersOperateLog[4].Value = "7";//1:存入 2：兑换  3:存转销 4: 提取
            parametersOperateLog[5].Value = VarietyID;
            parametersOperateLog[6].Value = UnitID;
            parametersOperateLog[7].Value = Price;
            parametersOperateLog[8].Value = "-" + Count_Trade;
            parametersOperateLog[9].Value = "-" + Count_Trade;
            parametersOperateLog[10].Value = Money_Trade;
            parametersOperateLog[11].Value = Count_Balance;
            parametersOperateLog[12].Value = DateTime.Now;
            parametersOperateLog[13].Value = VarietyName;
            parametersOperateLog[14].Value = UnitID;
            parametersOperateLog[15].Value = Dep_SID;
            #endregion

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);//添加存转销交易记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加存转销日志记录
                    //修改储户的商品结存
                    string strSqlDep_JieCun = " UPDATE dbo.Dep_StorageInfo  SET StorageNumber=StorageNumber+" + VarietyCount + " WHERE ID=" + Dep_SID;
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep_JieCun.ToString());//储户结存修改

                    tran.Commit();

                    var res = new { state = "success", msg = "退还存转销成功!", BusinessNO = BusinessNO };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "error", msg = "退还存转销失败!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion

        }


        void get_rShopping(HttpContext context)
        {
            bool ISHQ = Convert.ToBoolean(context.Session["ISHQ"]);
            string date_begin = "";//开始查询日期
            string date_end = "";//结束查询日期
            if (ISHQ)
            {
                date_begin = context.Request.Form["date_begin"].ToString();
                date_end = Convert.ToDateTime(context.Request.Form["date_end"]).AddDays(1).ToString();
            }
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string SellList = GetReturnShoppingList(ISHQ,AccountNumber,date_begin,date_end);
           
            StringBuilder strSql = new StringBuilder();

            strSql.Append("  SELECT ID, BusinessName,CONVERT(varchar(100), dt_Sell, 111) AS dt_Sell,VarietyName,VarietyCount,StorageDate,VarietyInterest,VarietyMoney,Price_JieSuan,Money_Earn");
            strSql.Append("  FROM dbo.StorageShopping where 1=1");
            if (!ISHQ)
            {
                strSql.Append("  and DATEDIFF(DAY,dt_Sell,GETDATE())<1");
            }
            else
            {
                strSql.Append(string.Format(" and dt_Sell>'{0}' AND dt_Sell<'{1}'", date_begin, date_end));
            }
            strSql.Append("  and ISReturn=0");//查询没有退还记录的商品 
            strSql.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
            strSql.Append("  and  ID NOT IN (SELECT StorageShoppingID  FROM dbo.SA_Shopping WHERE Dep_AN='" + AccountNumber + "')");//排除已经被结算的存转销

            if (SellList != "")
            {
                strSql.Append("  and ID not in (" + SellList + ")");//排除已经做过退换的记录
            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
          
            if (dt == null || dt.Rows.Count == 0)
            {
                var msg = "储户" + AccountNumber + "今天没有产品换购记录!";
                if (ISHQ)
                {
                    msg = "当前期限内没有查询到储户" + AccountNumber + "的产品换购记录!";
                }
                var res = new { state = "error", msg = msg };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {

                var res = new { state = "success", msg = "查询数据成功!", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        /// <summary>
        /// 已经退还的存转销列表
        /// </summary>
        /// <param name="ISHQ"></param>
        /// <param name="AccountNumber"></param>
        /// <param name="date_begin"></param>
        /// <param name="date_end"></param>
        /// <returns></returns>
        private string GetReturnShoppingList(bool ISHQ, string AccountNumber, string date_begin, string date_end)
        {
            StringBuilder strSqlISReturn = new StringBuilder();
            strSqlISReturn.Append("  SELECT ID, ISReturn");
            strSqlISReturn.Append("  FROM dbo.StorageShopping where 1=1");
            if (!ISHQ)
            {
                strSqlISReturn.Append("  and DATEDIFF(DAY,dt_Sell,GETDATE())<1");
            }
            else
            {
                strSqlISReturn.Append(string.Format(" and dt_Sell>'{0}' AND dt_Sell<'{1}'", date_begin, date_end));
            }

            strSqlISReturn.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
            strSqlISReturn.Append("  and ISReturn!=0");
            DataTable dtExISReturn = SQLHelper.ExecuteDataTable(strSqlISReturn.ToString());
            string strReturnList = "";
            if (dtExISReturn != null && dtExISReturn.Rows.Count != 0)
            {
                for (int i = 0; i < dtExISReturn.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        strReturnList = dtExISReturn.Rows[i]["ISReturn"].ToString();
                    }
                    else
                    {
                        strReturnList = strReturnList + "," + dtExISReturn.Rows[i]["ISReturn"].ToString();
                    }
                }
            }
            return strReturnList;
        }


        /// <summary>
        /// 退还存转销
        /// </summary>
        /// <param name="context"></param>
        void Return_StorageShopping(HttpContext context)
        {
            //添加兑换记录信息
            string IDList = context.Request.QueryString["IDList"].ToString();
            StringBuilder strSqlSelect = new StringBuilder();
            strSqlSelect.Append("select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,UnitName,VarietyID,VarietyName,VarietyCount,VarietyMoney,VarietyInterest,StorageDate,CurrentRate,EarningRate,StorageFee,StorageMoney,Price_JieSuan,Money_Earn,dt_Sell,JieCun_Last,JieCun_Now,ISReturn ");
            strSqlSelect.Append(" FROM [StorageShopping] ");
            strSqlSelect.Append(" where ID=@ID ");
            SqlParameter[] parametersSelect = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parametersSelect[0].Value = IDList;

            DataTable dt = SQLHelper.ExecuteDataTable(strSqlSelect.ToString(), parametersSelect);
            if (dt == null || dt.Rows.Count == 0)
            {
                context.Response.Write("Error");
                return;
            }
            string Dep_AccountNumber = dt.Rows[0]["Dep_AccountNumber"].ToString();
            //string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();//业务编号
            string BusinessNO = common.GetNewBusinessNO_Dep(Dep_AccountNumber);
            string strGUID = Fun.getGUID();//防伪码
            string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + Dep_AccountNumber + BusinessNO;


            string Dep_SID = dt.Rows[0]["Dep_SID"].ToString();
            string Dep_Name = dt.Rows[0]["Dep_Name"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = dt.Rows[0]["UserID"].ToString();
            string BusinessName = "退还换购";
            string UnitName = dt.Rows[0]["UnitName"].ToString();
            string VarietyID = dt.Rows[0]["VarietyID"].ToString();
            string VarietyName = dt.Rows[0]["VarietyName"].ToString();
            string VarietyCount = dt.Rows[0]["VarietyCount"].ToString();
            string VarietyMoney = dt.Rows[0]["VarietyMoney"].ToString();
            string VarietyInterest = dt.Rows[0]["VarietyInterest"].ToString();
            string StorageDate = dt.Rows[0]["StorageDate"].ToString();
            string CurrentRate = dt.Rows[0]["CurrentRate"].ToString();
            string EarningRate = dt.Rows[0]["EarningRate"].ToString();
            string StorageFee = dt.Rows[0]["StorageFee"].ToString();
            string StorageMoney = dt.Rows[0]["StorageMoney"].ToString();
            string Price_JieSuan = dt.Rows[0]["Price_JieSuan"].ToString();
            string Money_Earn = dt.Rows[0]["Money_Earn"].ToString();
            string dt_Sell = DateTime.Now.ToString();
            // string JieCun_Last = dt.Rows[0]["JieCun_Now"].ToString();
            string JieCun_Last = SQLHelper.ExecuteScalar(" SELECT TOP 1 JieCun_Now FROM dbo.StorageShopping WHERE Dep_SID=" + Dep_SID + " ORDER BY dt_Sell DESC").ToString();//查询上一期最近的结存情况
            double numJieCun_Now = Convert.ToDouble(JieCun_Last) + Convert.ToDouble(VarietyCount);
            string JieCun_Now = numJieCun_Now.ToString();
            string ISReturn = dt.Rows[0]["ID"].ToString();

            //写入存转销记录
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [StorageShopping] (");
            strSql.Append("SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,UnitName,VarietyID,VarietyName,VarietyCount,VarietyMoney,VarietyInterest,StorageDate,CurrentRate,EarningRate,StorageFee,StorageMoney,Price_JieSuan,Money_Earn,dt_Sell,JieCun_Last,JieCun_Now,ISReturn)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@strGUID,@BusinessNO,@Dep_SID,@Dep_AccountNumber,@Dep_Name,@WBID,@UserID,@BusinessName,@UnitName,@VarietyID,@VarietyName,@VarietyCount,@VarietyMoney,@VarietyInterest,@StorageDate,@CurrentRate,@EarningRate,@StorageFee,@StorageMoney,@Price_JieSuan,@Money_Earn,@dt_Sell,@JieCun_Last,@JieCun_Now,@ISReturn)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_SID", SqlDbType.Int,4),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@VarietyName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyCount", SqlDbType.Decimal,9),
					new SqlParameter("@VarietyMoney", SqlDbType.Decimal,9),
					new SqlParameter("@VarietyInterest", SqlDbType.Decimal,9),
					new SqlParameter("@StorageDate", SqlDbType.Int,4),
					new SqlParameter("@CurrentRate", SqlDbType.NChar,10),
					new SqlParameter("@EarningRate", SqlDbType.Decimal,9),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@StorageMoney", SqlDbType.Decimal,9),
					new SqlParameter("@Price_JieSuan", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Earn", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Sell", SqlDbType.DateTime),
					new SqlParameter("@JieCun_Last", SqlDbType.Decimal,9),
					new SqlParameter("@JieCun_Now", SqlDbType.Decimal,9),
					new SqlParameter("@ISReturn", SqlDbType.Int,4)};
            parameters[0].Value = SerialNumber;
            parameters[1].Value = strGUID;
            parameters[2].Value = BusinessNO;
            parameters[3].Value = Dep_SID;
            parameters[4].Value = Dep_AccountNumber;
            parameters[5].Value = Dep_Name;
            parameters[6].Value = WBID;
            parameters[7].Value = UserID;
            parameters[8].Value = BusinessName;
            parameters[9].Value = UnitName;
            parameters[10].Value = VarietyID;
            parameters[11].Value = VarietyName;
            parameters[12].Value = VarietyCount;
            parameters[13].Value = VarietyMoney;
            parameters[14].Value = VarietyInterest;
            parameters[15].Value = StorageDate;
            parameters[16].Value = CurrentRate;
            parameters[17].Value = EarningRate;
            parameters[18].Value = StorageFee;
            parameters[19].Value = StorageMoney;
            parameters[20].Value = Price_JieSuan;
            parameters[21].Value = Money_Earn;
            parameters[22].Value = dt_Sell;
            parameters[23].Value = JieCun_Last;
            parameters[24].Value = JieCun_Now;
            parameters[25].Value = ISReturn;

            #region 存转销日志记录
            //添加交易记录
            string Price = Price_JieSuan;//价格
            string Count_Trade = VarietyCount;//存储数量
            string Money_Trade = VarietyMoney;
            string Count_Balance = JieCun_Now.ToString();//当前产品结存
            //查找当前用户的当前产品的总的结存
            object objBalance = SQLHelper.ExecuteScalar("  SELECT SUM( StorageNumber)  AS StorageNumber  FROM dbo.Dep_StorageInfo WHERE AccountNumber='" + Dep_AccountNumber + "' AND VarietyID=" + VarietyID);
            if (objBalance != null && objBalance.ToString() != "")
            {
                // Count_Balance = objBalance.ToString();
                Count_Balance = (Convert.ToDouble(objBalance) + Convert.ToDouble(VarietyCount)).ToString();//现在总共剩余的结存
            }

            string UnitID = UnitName;

            StringBuilder strSqlOperateLog = new StringBuilder();
            strSqlOperateLog.Append("insert into [Dep_OperateLog] (");
            strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
            strSqlOperateLog.Append(" values (");
            strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade,@VarietyName,@UnitName,@Dep_SID)");
            strSqlOperateLog.Append(";select @@IDENTITY");
            SqlParameter[] parametersOperateLog = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyID", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitID", SqlDbType.NVarChar,50),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
                    new SqlParameter("@GoodCount", SqlDbType.Decimal,9),
					new SqlParameter("@Count_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Count_Balance", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime),
					new SqlParameter("@VarietyName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_SID", SqlDbType.Int,4)};
            parametersOperateLog[0].Value = WBID;
            parametersOperateLog[1].Value = UserID;
            parametersOperateLog[2].Value = Dep_AccountNumber;
            parametersOperateLog[3].Value = BusinessNO;
            parametersOperateLog[4].Value = "10";//1:存入 2：兑换  3:存转销 4: 提取
            parametersOperateLog[5].Value = VarietyID;
            parametersOperateLog[6].Value = UnitID;
            parametersOperateLog[7].Value = Price;
            parametersOperateLog[8].Value = "-" + VarietyMoney;//退还换购写入退还金额
            parametersOperateLog[9].Value = "-" + Count_Trade;
            parametersOperateLog[10].Value = Money_Trade;
            parametersOperateLog[11].Value = Count_Balance;
            parametersOperateLog[12].Value = DateTime.Now;
            parametersOperateLog[13].Value = VarietyName;
            parametersOperateLog[14].Value = UnitID;
            parametersOperateLog[15].Value = Dep_SID;
            #endregion

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);//添加存转销交易记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加存转销日志记录
                    //修改储户的商品结存
                    string strSqlDep_JieCun = " UPDATE dbo.Dep_StorageInfo  SET StorageNumber=StorageNumber+" + VarietyCount + " WHERE ID=" + Dep_SID;
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep_JieCun.ToString());//储户结存修改
                    tran.Commit();
                    var res = new { state = "success", msg = "退还产品换购成功!", BusinessNO = BusinessNO };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "error", msg = "退还产品换购失败!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion




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