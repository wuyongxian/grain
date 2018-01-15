using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;
namespace Web.Ashx
{
    /// <summary>
    /// commune 的摘要说明
    /// </summary>
    public class commune : IHttpHandler, IRequiresSessionState
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

                    case "GetGoodSupply": GetGoodSupply(context); break;
                    case "GetGoodSupplyInfo": GetGoodSupplyInfo(context); break;
                    case "GetMoney_PreDefine": GetMoney_PreDefine(context); break;
                    case "AddGoodSupply": AddGoodSupply(context); break;//添加供销商品信息
                    case "AddC_Supply": AddC_Supply(context); break;//添加供销商品交易信息
                    case "UpdateSupplyList": UpdateSupplyList(context); break;//添加供销商品列表
                    case "DeleteSupplyList": DeleteSupplyList(context); break;//取消供销商品列表

                    case "GetC_SupplyPrint": GetC_SupplyPrint(context); break;
                    case "PrintC_OperateLog": PrintC_OperateLog(context); break;
                    case "PrintC_OperateLogList": PrintC_OperateLogList(context); break;//打印购买记录 多条
                    case "PrintSupplyList": PrintSupplyList(context); break;//打印社员购买凭据
                    
                    case "GetNewBusinessNO": GetNewBusinessNO(context); break;
                        

                    case "GetWebSiteByUserID": GetWebSiteByUserID(context); break;

                    case "Get_Address_Xian": Get_Address_Xian(context); break;
                    case "Get_Address_Xiang": Get_Address_Xiang(context); break;
                    case "Get_Address_Cun": Get_Address_Cun(context); break;
                    case "Get_Address_Zu": Get_Address_Zu(context); break;

                    case "GetNewAccountNumber": GetNewAccountNumber(context); break;//获取新的社员编号



                    case "GetByAccountNumver_Depositor": GetByAccountNumver_Depositor(context); break;
                    case "GetCommunePrint": GetCommunePrint(context); break;
                    case "Add_Commune": Add_Commune(context); break;
                    case "Update_Commune": Update_Commune(context); break;
                    case "Update_CommuneState": Update_CommuneState(context); break;
                    case "CloseCommune": CloseCommune(context); break;
                        

                    case "GetCommuneByAccountNumber": GetCommuneByAccountNumber(context); break;
                    case "Update_Address_Cun": Update_Address_Cun(context); break;
                    case "DeleteByID_Address_Cun": DeleteByID_Address_Cun(context); break;

                    case "ChangeCard_Commune": ChangeCard_Commune(context); break;//社员换存折 
                    case "ChangeCard_Dep": ChangeCard_Dep(context); break;//储户换存折 

                }
            }

        }

        /// <summary>
        /// 社员换存折 
        /// </summary>
        /// <param name="context"></param>
        void ChangeCard_Commune(HttpContext context)
        {
            string AccountNumber_Old = context.Request.Form["txtC_AccountNumber"].ToString();
            string AccountNumber_New = context.Request.Form["AccountNumber"].ToString();
            string Toll = context.Request.Form["Toll"].ToString();
           
            //添加换存折信息记录
            StringBuilder strSqlChange = new StringBuilder();
            strSqlChange.Append("insert into [C_ChangeCard] (");
            strSqlChange.Append("AccountNumber_Old,AccountNumber_New,Toll,dt_change)");
            strSqlChange.Append(" values (");
            strSqlChange.Append("@AccountNumber_Old,@AccountNumber_New,@Toll,@dt_change)");
            strSqlChange.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber_Old", SqlDbType.NVarChar,50),
					new SqlParameter("@AccountNumber_New", SqlDbType.NVarChar,50),
					new SqlParameter("@Toll", SqlDbType.Int,4),
					new SqlParameter("@dt_change", SqlDbType.DateTime)};
            parameters[0].Value = AccountNumber_Old;
            parameters[1].Value = AccountNumber_New;
            parameters[2].Value = Toll;
            parameters[3].Value = DateTime.Now;

            string strSqlCommune = " UPDATE dbo.Commune SET AccountNumber='"+AccountNumber_New+"' WHERE AccountNumber='"+AccountNumber_Old+"'";
            string strSqlPreDefine = " UPDATE dbo.C_PreDefine SET C_AccountNumber='" + AccountNumber_New + "' WHERE C_AccountNumber='" + AccountNumber_Old + "'";
            string strSqlPreDefineConsume = " UPDATE dbo.C_PreDefineConsume SET C_AccountNumber='" + AccountNumber_New + "' WHERE C_AccountNumber='" + AccountNumber_Old + "'";
            string strSqlSupply = " UPDATE dbo.C_Supply SET C_AccountNumber='" + AccountNumber_New + "' WHERE C_AccountNumber='" + AccountNumber_Old + "'";


            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlChange.ToString(),parameters);//添加换存折记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlCommune.ToString());
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlPreDefine.ToString());
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlPreDefineConsume.ToString());
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlSupply.ToString());
                    tran.Commit();

                    context.Response.Write("OK");
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("Error");
                }
            }

        }


        /// <summary>
        /// 储户换存折 
        /// </summary>
        /// <param name="context"></param>
        void ChangeCard_Dep(HttpContext context)
        {
            string AccountNumber_Old = context.Request.Form["txtC_AccountNumber"].ToString();
           // string AccountNumber_New = context.Request.Form["AccountNumber"].ToString();//储户新账号
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = context.Session["ID"].ToString();
            string AccountNumber_New = common.GetNewAccountNumber(WBID);
            string Toll = context.Request.Form["Toll"].ToString();

            //添加换存折信息记录
            StringBuilder strSqlChange = new StringBuilder();
            strSqlChange.Append("insert into [Dep_ChangeCard] (");
            strSqlChange.Append("AccountNumber_Old,AccountNumber_New,Toll,dt_change)");
            strSqlChange.Append(" values (");
            strSqlChange.Append("@AccountNumber_Old,@AccountNumber_New,@Toll,@dt_change)");
            strSqlChange.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber_Old", SqlDbType.NVarChar,50),
					new SqlParameter("@AccountNumber_New", SqlDbType.NVarChar,50),
					new SqlParameter("@Toll", SqlDbType.Int,4),
					new SqlParameter("@dt_change", SqlDbType.DateTime)};
            parameters[0].Value = AccountNumber_Old;
            parameters[1].Value = AccountNumber_New;
            parameters[2].Value = Toll;
            parameters[3].Value = DateTime.Now;
            StringBuilder sqlupdate = new StringBuilder();
            sqlupdate.Append(string.Format(" UPDATE dbo.Depositor SET AccountNumber='{0}', dt_Update='" + DateTime.Now.ToString() + "' WHERE AccountNumber='{1}'",AccountNumber_New,AccountNumber_Old));
            sqlupdate.Append(string.Format(" UPDATE dbo.Dep_StorageInfo SET AccountNumber='{0}' WHERE AccountNumber='{1}'", AccountNumber_New, AccountNumber_Old));
            sqlupdate.Append(string.Format(" UPDATE dbo.GoodExchange SET Dep_AccountNumber='{0}' WHERE Dep_AccountNumber='{1}'", AccountNumber_New, AccountNumber_Old));
            sqlupdate.Append(string.Format(" UPDATE dbo.StorageSell SET Dep_AccountNumber='{0}' WHERE Dep_AccountNumber='{1}'", AccountNumber_New, AccountNumber_Old));
            sqlupdate.Append(string.Format(" UPDATE dbo.StorageShopping SET Dep_AccountNumber='{0}' WHERE Dep_AccountNumber='{1}'", AccountNumber_New, AccountNumber_Old));
            //sqlupdate.Append(string.Format(" UPDATE dbo.Dep_OperateLog SET Dep_AccountNumber='{0}' WHERE Dep_AccountNumber='{1}'", AccountNumber_New, AccountNumber_Old));

            DataTable dtStorage = GetDep_StorageInfo(AccountNumber_Old);
            StringBuilder sqlO_Log = new StringBuilder();
            List<string> BNOList = new List<string>();//此储户的存储记录集合         
            if (dtStorage != null && dtStorage.Rows.Count != 0) {
                //string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber_New);//交易流水号
                string BusinessNO = GetDepBNO_ChangeCard(AccountNumber_Old);
                string BusinessName_Log = "12";//储户换存折
                for (int i = 0; i < dtStorage.Rows.Count; i++) {
                    string StorageNumber = dtStorage.Rows[i]["StorageNumber"].ToString();
                    if (Convert.ToDouble(StorageNumber) <= 0) {
                        continue;//如果此条记录的存量数为0，则不写入存粮记录
                    }
                    if (i != 0)
                    {
                        BusinessNO = Fun.ConvertIntToString(Convert.ToInt32(BusinessNO) + 1, 4);//新的对话编号
                    }
                    BNOList.Add(BusinessNO);

                    string Dep_SID = dtStorage.Rows[i]["Dep_SID"].ToString();
                    string VarietyID = dtStorage.Rows[i]["VarietyID"].ToString();
                    string VarietyName = dtStorage.Rows[i]["VarietyName"].ToString();
                    string UnitID = dtStorage.Rows[i]["UnitID"].ToString();
                    string UnitName = dtStorage.Rows[i]["UnitName"].ToString();
                    string Price_ShiChang = dtStorage.Rows[i]["Price_ShiChang"].ToString();
                   
                    string Money_Trade = "0";//交易额

                    sqlO_Log.Append("  insert into [Dep_OperateLog] (");
                    sqlO_Log.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
                    sqlO_Log.Append(" values (");

                    sqlO_Log.Append(string.Format("{0},{1},'{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},{11},'{12}','{13}','{14}',{15})", WBID, UserID, AccountNumber_New, BusinessNO, BusinessName_Log, VarietyID, UnitID, Price_ShiChang, StorageNumber, StorageNumber, Money_Trade, StorageNumber, DateTime.Now.ToString(), VarietyName, UnitName,Dep_SID));
                }
                   
            }

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlChange.ToString(), parameters);//添加换存折记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlupdate.ToString());
                    if (sqlO_Log.ToString() != "") {//储户有存粮记录的
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlO_Log.ToString());
                    }
                    tran.Commit();
                    var res = new { state = "success", msg = "更换存折成功!", BNOList = Fun.ListToString(BNOList, '|'), AccountNumber_New = AccountNumber_New };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                     var res=new {state="error",msg="更换存折失败!"};
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }

        }

 
       /// <summary>
       /// 返回新的储户业务编号，至确保此储户的第一笔业务打印在首页上
       /// </summary>
       /// <param name="AccountNumber"></param>
       /// <returns></returns>
        private  string GetDepBNO_ChangeCard(string AccountNumber)
        {
            int BusinessNO = 1;//使用4位编号
            //查询当前社员的业务编号
            StringBuilder strSqlNO = new StringBuilder();
            strSqlNO.Append("  SELECT TOP 1 BusinessNO ");
            strSqlNO.Append("  FROM dbo.Dep_OperateLog  ");
            strSqlNO.Append("  WHERE Dep_AccountNumber='" + AccountNumber + "'");
            strSqlNO.Append("  ORDER BY dt_Trade DESC,BusinessNO DESC");
            object obj = SQLHelper.ExecuteScalar(strSqlNO.ToString());
            if (obj != null && obj.ToString() != "")
            {
                BusinessNO = Convert.ToInt32(obj);
            }
            while (BusinessNO % 20 != 0) {
                BusinessNO += 1;
            }
            return Fun.ConvertIntToString(BusinessNO + 1, 4);//取回新的业务编号
        }


        private DataTable GetDep_StorageInfo(string AccountNumber)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT A.ID as Dep_SID, VarietyID,  T.strName+ V.strName AS VarietyName,U.ID AS UnitID,U.strName AS UnitName,Price_ShiChang, SUM(StorageNumber)AS StorageNumber,SUM(StorageNumberRaw) AS StorageNumberRaw");
            sql.Append(" FROM dbo.Dep_StorageInfo A LEFT OUTER JOIN dbo.StorageVariety V ON A.VarietyID=V.ID ");
            sql.Append(" LEFT OUTER JOIN dbo.StorageTime T  ON A.TimeID=T.ID");
            sql.Append(" LEFT OUTER JOIN dbo.BD_MeasuringUnit U ON V.MeasuringUnitID=U.ID");
            sql.Append( string.Format(" WHERE AccountNumber='{0}'",AccountNumber));
            sql.Append(" GROUP BY A.ID, VarietyID,V.strName,T.strName, U.ID,U.strName,Price_ShiChang");
            
            return SQLHelper.ExecuteDataTable(sql.ToString());
        }


        /// <summary>
        /// 获取社员记录的打印信息
        /// </summary>
        /// <param name="context"></param>
        void GetC_SupplyPrint(HttpContext context)
        {
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("SELECT A.ID,A.BusinessName,B.strName AS WBID,A.VarietyID,A.Price,A.UnitID,A.CountTrade,A.Money_Trade,A.Money_YouHui,A.numDisCount,A.Money_Reality,CONVERT(NVARCHAR(100),A.dt_Trade,23) AS dt_Trade");

            strSqlLog.Append(" FROM dbo.C_OperateLog A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlLog.Append(" where A.BusinessNO='" + BusinessNO + "' and  A.C_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string VarietyID = dtLog.Rows[0]["VarietyID"].ToString();
            string UnitID = dtLog.Rows[0]["UnitID"].ToString();
            string Price = dtLog.Rows[0]["Price"].ToString();
            string PriceUnit = Price + "/" + UnitID;
            if (Price == "0.00")
            {
                PriceUnit = "";
            }
           // string Price_C = dtLog.Rows[0]["Price_C"].ToString();
            string CountTrade = dtLog.Rows[0]["CountTrade"].ToString();
            if (CountTrade == "0.00")
            {
                CountTrade = "";
            }
            string Money_Trade = dtLog.Rows[0]["Money_Trade"].ToString();
            string numDisCount = dtLog.Rows[0]["numDisCount"].ToString();
            string strDisCount = numDisCount+"%";
            if (numDisCount == "0.00")
            {
                strDisCount = "";
            }
            string Money_Reality = dtLog.Rows[0]["Money_Reality"].ToString();

            string Money_YouHui = dtLog.Rows[0]["Money_YouHui"].ToString();
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();
            string[] dt_TradeArray = dt_Trade.Split('-');
            dt_Trade = dt_TradeArray[0] + dt_TradeArray[1] + dt_TradeArray[2];
            string numWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting] ";
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
                strReturn.Append("   <td style='width:" + (RecordC2X - RecordC1X).ToString() + "px;'>" + dt_Trade + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>" + BusinessName + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>" + VarietyID + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>" + PriceUnit + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC6X - RecordC5X).ToString() + "px;'>" + CountTrade + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>" + Money_Trade + "</td>");
                strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>" + Money_YouHui + "</td>");
                strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>" + Money_Reality + "</td>");
                strReturn.Append("   <td >" + WBID + " </td>");

                strReturn.Append("   </tr></table>");

                context.Response.Write(strReturn.ToString());
            }
        }

        /// <summary>
        /// 获取消费信息与预付款消费记录 
        /// </summary>
        /// <param name="context"></param>
        void PrintC_OperateLog(HttpContext context)
        {

            string SupplyID = context.Request.QueryString["SupplyID"].ToString();
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("SELECT A.ID,A.BusinessName,B.strName AS WBID,A.VarietyID,A.Price,A.UnitID,A.CountTrade,A.Money_Trade,A.Money_YouHui,A.numDisCount,A.Money_Reality,CONVERT(NVARCHAR(100),A.dt_Trade,23) AS dt_Trade");

            strSqlLog.Append(" FROM dbo.C_OperateLog A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlLog.Append(" where A.BusinessNO='" + BusinessNO + "' and  A.C_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
            string VarietyID = dtLog.Rows[0]["VarietyID"].ToString();
            string UnitID = dtLog.Rows[0]["UnitID"].ToString();
            string Price = dtLog.Rows[0]["Price"].ToString();
            string PriceUnit = Price + "/" + UnitID;
            if (Price == "0.00")
            {
                PriceUnit = "";
            }
            // string Price_C = dtLog.Rows[0]["Price_C"].ToString();
            string CountTrade = dtLog.Rows[0]["CountTrade"].ToString();
            if (CountTrade == "0.00")
            {
                CountTrade = "";
            }
            string Money_Trade = dtLog.Rows[0]["Money_Trade"].ToString();


            string numDisCount = dtLog.Rows[0]["numDisCount"].ToString();
            string strDisCount = numDisCount+"%";
            if (numDisCount == "0.00")
            {
                strDisCount = "";
            }
            string Money_Reality = dtLog.Rows[0]["Money_Reality"].ToString();


            string Money_YouHui = dtLog.Rows[0]["Money_YouHui"].ToString();
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();
            string[] dt_TradeArray = dt_Trade.Split('-');
            dt_Trade = dt_TradeArray[0] + dt_TradeArray[1] + dt_TradeArray[2];

            string numWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting] ";
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
                strReturn.Append("   <td style='width:" + (RecordC2X - RecordC1X).ToString() + "px;'>" + dt_Trade + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>" + BusinessName + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>" + VarietyID + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>" + PriceUnit + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC6X - RecordC5X).ToString() + "px;'>" + CountTrade + " </td>");
                strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>" + Money_Trade + "</td>");
                strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>" + Money_YouHui + "</td>");
                strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>" + Money_Reality + "</td>");
                strReturn.Append("   <td >" + WBID + " </td>");
                strReturn.Append("   </tr></table>");

                //查询是否存在与消费记录相关的预付款记录

                StringBuilder strSqlLogConsume = new StringBuilder();
                strSqlLogConsume.Append("SELECT A.BusinessNO, A.ID,A.BusinessName,B.strName AS WBID,A.VarietyID,A.Price,A.UnitID,A.CountTrade,A.Money_Trade,A.numDisCount,A.Money_Reality,CONVERT(NVARCHAR(100),A.dt_Trade,23) AS dt_Trade");

                strSqlLogConsume.Append(" FROM dbo.C_OperateLog A INNER JOIN dbo.WB B ON A.WBID=B.ID");
                strSqlLogConsume.Append(" WHERE BusinessNO=(SELECT BusinessNO FROM dbo.C_PreDefineConsume WHERE C_SupplyID=" + SupplyID + ")");
                strSqlLogConsume.Append(" and  A.C_AccountNumber='" + AccountNumber + "'");
                DataTable dtLogConsume = SQLHelper.ExecuteDataTable(strSqlLogConsume.ToString());

                if (dtLogConsume == null || dtLogConsume.Rows.Count == 0)
                {
                    context.Response.Write(strReturn.ToString());//仅返回消费记录
                    return;
                }
                string BusinessNO2=dtLogConsume.Rows[0]["BusinessNO"].ToString();
                string WBID2 = dtLogConsume.Rows[0]["WBID"].ToString();
                string BusinessName2 = dtLogConsume.Rows[0]["BusinessName"].ToString();
                string VarietyID2 = dtLogConsume.Rows[0]["VarietyID"].ToString();
                string UnitID2 = dtLogConsume.Rows[0]["UnitID"].ToString();
                string Price2 = dtLogConsume.Rows[0]["Price"].ToString();
                string PriceUnit2 = Price2 + "/" + UnitID2;
                if (Price2 == "0.00")
                {
                    PriceUnit2 = "";
                }
                // string Price_C = dtLog.Rows[0]["Price_C"].ToString();
                string CountTrade2 = dtLogConsume.Rows[0]["CountTrade"].ToString();
                if (CountTrade2 == "0.00")
                {
                    CountTrade2 = "";
                }
                string Money_Trade2 = dtLogConsume.Rows[0]["Money_Trade"].ToString();
                string numDisCount2 = dtLogConsume.Rows[0]["numDisCount"].ToString();
                string strDisCount2 = numDisCount2+"%";
                if (numDisCount2 == "0.00")
                {
                    strDisCount2 = "";
                }
                string Money_Reality2 = dtLogConsume.Rows[0]["Money_Reality"].ToString();
                string dt_Trade2 = dtLogConsume.Rows[0]["dt_Trade"].ToString();
                string[] dt_TradeArray2 = dt_Trade2.Split('-');
                dt_Trade2 = dt_TradeArray2[0] + dt_TradeArray2[1] + dt_TradeArray2[2];


                if (numIndex != 20)//当前没有打印到最后一条记录
                {
                    strReturn.Append("   <table style='margin-left:" + RecordC1X + "px; font-size:" + FontSize + "px;'><tr>");
                    strReturn.Append("   <td style='width:" + (RecordC2X - RecordC1X).ToString() + "px;'>" + dt_Trade2 + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>" + BusinessName2 + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>" + VarietyID2 + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>" + PriceUnit2 + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC6X - RecordC5X).ToString() + "px;'>" + CountTrade2 + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>" + Money_Trade2 + "</td>");
                    strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>" + strDisCount2 + "</td>");
                    strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>" + Money_Reality2 + "</td>");
                    strReturn.Append("   <td >" + WBID2 + " </td>");
                    strReturn.Append("   </tr></table>");
                    context.Response.Write(strReturn.ToString());
                }
                else
                {
                    context.Response.Write(BusinessNO2+"&"+ strReturn.ToString());
                }

            }
        }



        /// <summary>
        /// 获取消费信息与预付款消费记录 
        /// </summary>
        /// <param name="context"></param>
        void PrintC_OperateLogList(HttpContext context)
        {
            string BNList = "";
            if (context.Request.QueryString["Surplus"] != null)
            {
                BNList = context.Session["SupplyListSurPlue"].ToString();
            }
            else
            {
                BNList = context.Session["SupplyList"].ToString();
            }
            string BNListSurPlue = "";//剩余的需要打印的编号集合
            string[] BNArray = BNList.Split('|');//需要打印的编号集合

            string BusinessNO = BNArray[0];//首个编号
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            string strReturnMsg = "";


            string numWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting] ";
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
                for (int i = 0; i < BNArray.Length; i++)
                {

                    BusinessNO = BNArray[i];
                    StringBuilder strSqlLog = new StringBuilder();
                    strSqlLog.Append("SELECT A.ID,A.BusinessName,B.strName AS WBID,A.VarietyID,A.Price,A.UnitID,A.CountTrade,A.Money_Trade,A.Money_YouHui,A.numDisCount,A.Money_Reality,CONVERT(NVARCHAR(100),A.dt_Trade,23) AS dt_Trade");

                    strSqlLog.Append(" FROM dbo.C_OperateLog A INNER JOIN dbo.WB B ON A.WBID=B.ID");
                    strSqlLog.Append(" where A.BusinessNO='" + BusinessNO + "' and  A.C_AccountNumber='" + AccountNumber + "'");

                    DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
                    if (dtLog == null || dtLog.Rows.Count == 0)
                    {
                        context.Response.Write("");
                        return;
                    }
                    string WBID = dtLog.Rows[0]["WBID"].ToString();
                    string BusinessName = dtLog.Rows[0]["BusinessName"].ToString();
                    string VarietyID = dtLog.Rows[0]["VarietyID"].ToString();
                    string UnitID = dtLog.Rows[0]["UnitID"].ToString();
                    string Price = dtLog.Rows[0]["Price"].ToString();
                    string PriceUnit = Price + "/" + UnitID;
                    if (Price == "0.00")
                    {
                        PriceUnit = "";
                    }
                    // string Price_C = dtLog.Rows[0]["Price_C"].ToString();
                    string CountTrade = dtLog.Rows[0]["CountTrade"].ToString();
                    if (CountTrade == "0.00")
                    {
                        CountTrade = "";
                    }
                    string Money_Trade = dtLog.Rows[0]["Money_Trade"].ToString();


                    string numDisCount = dtLog.Rows[0]["numDisCount"].ToString();
                    string strDisCount = numDisCount + "%";
                    if (numDisCount == "0.00")
                    {
                        strDisCount = "";
                    }
                    string Money_Reality = dtLog.Rows[0]["Money_Reality"].ToString();


                    string Money_YouHui = dtLog.Rows[0]["Money_YouHui"].ToString();
                    string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();
                    string[] dt_TradeArray = dt_Trade.Split('-');
                    dt_Trade = dt_TradeArray[0] + dt_TradeArray[1] + dt_TradeArray[2];



                    if (i == 0)//首行设置于存折最上方的间距
                    {
                        strReturn.Append("  <table style='width:100%; height:" + RecordRY + "px'><tr><td></td> </tr></table>");
                    }
                    strReturn.Append("   <table style='margin-left:" + RecordC1X + "px; font-size:" + FontSize + "px;'><tr>");
                    strReturn.Append("   <td style='width:" + (RecordC2X - RecordC1X).ToString() + "px;'>" + dt_Trade + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>" + BusinessName + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>" + VarietyID + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>" + PriceUnit + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC6X - RecordC5X).ToString() + "px;'>" + CountTrade + " </td>");
                    strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>" + Money_Trade + "</td>");
                    strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>" + Money_YouHui + "</td>");
                    strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>" + Money_Reality + "</td>");
                    strReturn.Append("   <td >" + WBID + " </td>");
                    strReturn.Append("   </tr></table>");

                    numIndex += 1;//增加连续打印序列
                    if (numIndex > 20)//如果是连续打印，并且超出了本页的范围
                    {
                        if (i < BNArray.Length - 1)//此条不是最后一个被打印的数据
                        {
                            for (int j = i + 1; j < BNArray.Length; j++)
                            {
                                if (j == i + 1)
                                {
                                    BNListSurPlue = BNArray[j];
                                }
                                else
                                {
                                    BNListSurPlue = BNListSurPlue + "|" + BNArray[j];
                                }
                            }
                            context.Session["SupplyListSurPlue"] = BNListSurPlue;//缓存剩余打印项
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
        /// 打印社员购买凭据
        /// </summary>
        /// <param name="context"></param>
        void PrintSupplyList(HttpContext context)
        {

            string BNList = context.Session["SupplyList"].ToString();//需要打印的序列组合

            string[] BNArray = BNList.Split('|');//需要打印的编号集合

            string BusinessNO = BNArray[0];//首个编号


            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select ID,SerialNumber,strGUID,BusinessNO,C_AccountNumber,C_Name,WBID,UserID,BusinessName,GoodSupplyID,GoodSupplyName,SpecName,UnitName,GoodSupplyCount,GoodSupplyPrice,Price_Commune,numDisCount,Money_Total,Money_YouHui,Money_PreDefine,Money_Reality,Money_Back,CONVERT(NVARCHAR(100),dt_Trade ,23) AS dt_Trade ");
            strSqlLog.Append("  FROM dbo.C_Supply");
            strSqlLog.Append("  ");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  C_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }

            //共有参数
     
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();//
            string SerialNumber = dtLog.Rows[0]["SerialNumber"].ToString();//
            string C_Name = dtLog.Rows[0]["C_Name"].ToString();//
            string C_AccountNumber = dtLog.Rows[0]["C_AccountNumber"].ToString();//
            string dt_Trade = dtLog.Rows[0]["dt_Trade"].ToString();

            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + " 社员购物凭证</span></td> </tr>");
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + SerialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");

            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 160px;'>  <span >姓名：" + C_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >账号：" + C_AccountNumber + "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span >购买日期：" +dt_Trade+ "</span> </td>");
            strReturn.Append("    <td style='width: 160px;'>  <span ></span> </td>");
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
            strReturn.Append("    <td style='width: 60px;'> <span>规格型号</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>单位</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>数量</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>单价</span></td>");
            strReturn.Append("    <td style='width: 60px;'> <span>优惠价</span></td>");
            strReturn.Append("    <td style='width: 65px;'> <span>总金额</span></td>");
            strReturn.Append("    <td style='width: 65px;'> <span>优惠金额</span></td>");
            strReturn.Append("    <td style='width: 65px;'> <span>预付款</span></td>");
            strReturn.Append("    <td style='width: 65px;'> <span>实付金额</span></td>");
            strReturn.Append("  </tr>");

            double T_SMoney = 0;//商品总金额
            double T_SMoney_Commune = 0;//优惠总金额
           
            double T_SMoney_PreDefine = 0;//预付款总金额
            double T_SMoney_Reality = 0;//实付款总金额
            for (int i = 0; i < BNArray.Length; i++)
            {
                BusinessNO = BNArray[i];
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  select ID,SerialNumber,strGUID,BusinessNO,C_AccountNumber,C_Name,WBID,UserID,BusinessName,GoodSupplyID,GoodSupplyName,SpecName,UnitName,GoodSupplyCount,GoodSupplyPrice,Price_Commune,numDisCount,Money_Total,Money_YouHui,Money_PreDefine,Money_Reality,Money_Back,CONVERT(NVARCHAR(100),dt_Trade ,23) AS dt_Trade ");
                strSql.Append("  FROM dbo.C_Supply");
                strSql.Append("  ");
                strSql.Append(" where BusinessNO='" + BusinessNO + "' and  C_AccountNumber='" + AccountNumber + "'");

                DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
                if (dt == null || dt.Rows.Count == 0)
                {
                    continue;//遇到序号中断的情况 可能是中间用到了预付款
                }
                string GoodSupplyName = dt.Rows[0]["GoodSupplyName"].ToString();//业务名
                string SpecName = dt.Rows[0]["SpecName"].ToString();//品名
                string UnitName = dt.Rows[0]["UnitName"].ToString();

                double GoodSupplyCount = Convert.ToDouble(dt.Rows[0]["GoodSupplyCount"]);//商品数量
                double GoodSupplyPrice = Convert.ToDouble(dt.Rows[0]["GoodSupplyPrice"]);//商品价格
                double Price_Commune = Convert.ToDouble(dt.Rows[0]["Price_Commune"]);//社员优惠价
                double Money_Total = Convert.ToDouble(dt.Rows[0]["Money_Total"]);//总金额
                double Money_YouHui = Convert.ToDouble(dt.Rows[0]["Money_YouHui"]);
                double Money_PreDefine = Convert.ToDouble(dt.Rows[0]["Money_PreDefine"]);
                double Money_Reality = Convert.ToDouble(dt.Rows[0]["Money_Reality"]);
                T_SMoney += Money_Total;
                T_SMoney_Commune += Money_YouHui;
                T_SMoney_PreDefine += Money_PreDefine;
                T_SMoney_Reality += Money_Reality;


                #region 返回信息



                strReturn.Append("   <tr style='height: 20px;'>");
                strReturn.Append("    <td> <span>" + GoodSupplyName + "</span></td>");
                strReturn.Append("    <td> <span>" + SpecName + "</span></td>");
                strReturn.Append("    <td> <span>" + UnitName + "</span></td>");
                strReturn.Append("    <td> <span>" + GoodSupplyCount + "</span></td>");
                strReturn.Append("    <td> <span>￥" + GoodSupplyPrice + "</span></td>");
                strReturn.Append("    <td> <span>￥" + Price_Commune + "</span></td>");
                strReturn.Append("    <td> <span>￥" + Money_Total + "</span></td>");
                strReturn.Append("    <td> <span>￥" + Money_YouHui + "</span></td>");
                strReturn.Append("    <td> <span>￥" + Money_PreDefine + "</span></td>");
                strReturn.Append("    <td> <span>￥" + Money_Reality + "</span></td>");
                strReturn.Append("  </tr>");


                #endregion
            }
            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width: 160px;'> <span>消费合计："+T_SMoney+"元</span></td> ");
            strReturn.Append("   <td style='width: 160px;'> <span>优惠合计：" + T_SMoney_Commune + "元</span></td> ");
            strReturn.Append("   <td style='width: 160px;'> <span>预付款合计：" + T_SMoney_PreDefine + "元</span></td> ");
            strReturn.Append("   <td style='width: 160px;'> <span>实付合计：" + T_SMoney_Reality + "元</span></td> ");
            strReturn.Append("  </tr>");

            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string UserID = dtLog.Rows[0]["UserID"].ToString();
            string WBName = SQLHelper.ExecuteScalar(" SELECT top 1 strName FROM dbo.WB WHERE ID=" + WBID).ToString();
            string UserName = SQLHelper.ExecuteScalar(" SELECT top 1 strLoginName   FROM dbo.Users WHERE ID=" + UserID).ToString();

            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>营业网点：" + WBName
             + "</span></td> ");
            strReturn.Append("   <td > <span>营业员：" + UserName + "</span></td> ");
            strReturn.Append("  <td align='right'>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");


            context.Response.Write(strReturn.ToString());
        }

        void AddGoodSupply(HttpContext context)
        {
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = context.Session["ID"].ToString();
            string C_AccountNumber = context.Request.Form["txtC_AccountNumber"].ToString();
         
            string Money_Reality = context.Request.Form["Money_Reality"].ToString();
            string Money_Rate = context.Request.Form["Money_Rate"].ToString();
            string Money_PreDefine = context.Request.Form["Money_PreDefine"].ToString();
            double numMoney_YouHui = Convert.ToDouble(Money_PreDefine) - Convert.ToDouble(Money_Reality);
            string Money_YouHui = Math.Round(numMoney_YouHui, 2).ToString();

            //查询当前社员的当前品种是否已经存有预付款，如果存在，则将两次的预付款金额合并
            double numMoney = 0;
            StringBuilder strSqlMoney = new StringBuilder();
            strSqlMoney.Append("  SELECT SUM( Money_PreDefine)");
            strSqlMoney.Append("  FROM dbo.C_PreDefine");
            strSqlMoney.Append("  WHERE ISUsed=0 AND C_AccountNumber='"+C_AccountNumber+"'");
            object objMoney = SQLHelper.ExecuteScalar(strSqlMoney.ToString());
            bool ISExitRecord = false;//是否已经存在该社员的预存款记录
            if (objMoney != null && objMoney.ToString() != "")
            {
                ISExitRecord = true;
                numMoney = Convert.ToDouble(objMoney);
            }
            Money_PreDefine = Math.Round(Convert.ToDouble(Money_PreDefine) + numMoney, 2).ToString();//当前社员的的总共预付款金额
            


            StringBuilder strSql = new StringBuilder();

            if (!ISExitRecord)//储户第一次交预存款
            {
                strSql.Append("insert into [C_PreDefine] (");
                strSql.Append("WBID,UserID,BusinessNO,C_AccountNumber,GoodSupplyID,Money_Reality,Money_Rate,Money_PreDefine,dt_Trade,Money_min,numDay_min,ISUsed)");
                strSql.Append(" values (");
                strSql.Append("@WBID,@UserID,@BusinessNO,@C_AccountNumber,@GoodSupplyID,@Money_Reality,@Money_Rate,@Money_PreDefine,@dt_Trade,@Money_min,@numDay_min,@ISUsed)");
                strSql.Append(";select @@IDENTITY");
                SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@C_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@GoodSupplyID", SqlDbType.Int,4),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Rate", SqlDbType.Decimal,9),
					new SqlParameter("@Money_PreDefine", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime),
					new SqlParameter("@Money_min", SqlDbType.Decimal,9),
					new SqlParameter("@numDay_min", SqlDbType.Int,4),
					new SqlParameter("@ISUsed", SqlDbType.Bit,1)};
                parameters[0].Value = WBID;
                parameters[1].Value = UserID;
                parameters[2].Value = BusinessNO;
                parameters[3].Value = C_AccountNumber;
                parameters[4].Value = 1;//修改后不使用社员商品ID
                parameters[5].Value = Money_Reality;
                parameters[6].Value = Money_Rate;
                parameters[7].Value = Money_PreDefine;
                parameters[8].Value = DateTime.Now;
                parameters[9].Value = 0;
                parameters[10].Value = 0;
                parameters[11].Value = 0;

                SQLHelper.ExecuteNonQuery( strSql.ToString(), parameters);
            }
            else {
                strSql.Append("   UPDATE C_PreDefine SET  Money_PreDefine="+Money_PreDefine);
                strSql.Append("   WHERE C_AccountNumber='"+C_AccountNumber+"'");//更新预存款数额
                SQLHelper.ExecuteNonQuery(strSql.ToString());
            }



            //添加消费记录

            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("insert into [C_OperateLog] (");
            strSqlLog.Append("WBID,UserID,C_AccountNumber,BusinessNO,BusinessName,VarietyID,Price,UnitID,Price_C,CountTrade,Money_Trade,Money_YouHui,numDisCount,Money_Reality,dt_Trade)");
            strSqlLog.Append(" values (");
            strSqlLog.Append("@WBID,@UserID,@C_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@Price,@UnitID,@Price_C,@CountTrade,@Money_Trade,@Money_YouHui,@numDisCount,@Money_Reality,@dt_Trade)");
            strSqlLog.Append(";select @@IDENTITY");
            SqlParameter[] parametersLog = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@C_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyID", SqlDbType.NVarChar,50),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
					new SqlParameter("@UnitID", SqlDbType.NVarChar,50),
					new SqlParameter("@Price_C", SqlDbType.Decimal,9),
					new SqlParameter("@CountTrade", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Trade", SqlDbType.Decimal,9),
					new SqlParameter("@Money_YouHui", SqlDbType.NChar,10),
					new SqlParameter("@numDisCount", SqlDbType.Decimal,9),
					new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parametersLog[0].Value = WBID;
            parametersLog[1].Value = UserID;
            parametersLog[2].Value = C_AccountNumber;
            parametersLog[3].Value = BusinessNO;
            parametersLog[4].Value = "预付";//类型分为 预付/消费
            parametersLog[5].Value = "预付款";//使用商品的名称
            parametersLog[6].Value = 0;
            parametersLog[7].Value = "";
            parametersLog[8].Value = 0;
            parametersLog[9].Value = 0;
            parametersLog[10].Value = Money_PreDefine; 
            parametersLog[11].Value = Money_YouHui;
            parametersLog[12].Value = Money_Rate; 
            parametersLog[13].Value = Money_Reality;
            parametersLog[14].Value = DateTime.Now;


            //向数据表写入预付款使用记录
            StringBuilder strSqlConsume = new StringBuilder();
            strSqlConsume.Append("insert into [C_PreDefineConsume] (");
            strSqlConsume.Append("WBID,UserID,BusinessNO,C_AccountNumber,C_PreDefineID,C_SupplyID,Money_PreDefine,dt_Trade)");
            strSqlConsume.Append(" values (");
            strSqlConsume.Append("@WBID,@UserID,@BusinessNO,@C_AccountNumber,@C_PreDefineID,@C_SupplyID,@Money_PreDefine,@dt_Trade)");
            strSqlConsume.Append(";select @@IDENTITY");
            SqlParameter[] parametersConsume = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@C_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@C_PreDefineID", SqlDbType.Int,4),
					new SqlParameter("@C_SupplyID", SqlDbType.Int,4),
					new SqlParameter("@Money_PreDefine", SqlDbType.Decimal,9),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parametersConsume[0].Value = WBID;
            parametersConsume[1].Value = UserID;
            parametersConsume[2].Value = BusinessNO;
            parametersConsume[3].Value = C_AccountNumber;
            parametersConsume[4].Value = 1;//1:存款 2：消费
            parametersConsume[5].Value = 0;//不适用
            parametersConsume[6].Value = Money_PreDefine;
            parametersConsume[7].Value = DateTime.Now;

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                { 
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlLog.ToString(), parametersLog);
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlConsume.ToString(),parametersConsume);
                    tran.Commit();

                    context.Response.Write("OK");
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("Error");
                }
            }
          
        }


        void GetGoodSupply(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName FROM dbo.GoodSupply");
          
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

        //获取供销商品的详细信息
        void GetGoodSupplyInfo(HttpContext context)
        {

            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT A.ID,A.strName,A.BarCode,B.strName AS UnitID,C.strName AS SpecID,A.Price,A.numDiscount,A.Price_Commune,A.Price_WB,A.Price_WBBack,A.Price_Commune, A.ISPreDefine");
            strSql.Append("  FROM dbo.GoodSupply A INNER JOIN dbo.BD_MeasuringUnit B ON A.UnitID=B.ID");
            strSql.Append("  INNER JOIN dbo.BD_PackingSpec C ON A.SpecID =C.ID");
            strSql.Append(" WHERE A.ID="+ID);
            strSql.Append(" ");
            strSql.Append(" ");

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

        //获取社员的预付款信息
        void GetMoney_PreDefine(HttpContext context)
        {

            string C_AccountNumber = context.Request.QueryString["C_AccountNumber"].ToString();
           // string GoodSupplyID = context.Request.QueryString["GoodSupplyID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT SUM(Money_PreDefine) AS Money_PreDefine  ");
            strSql.Append("   FROM dbo.C_PreDefine");
            strSql.Append("  WHERE ISUsed=0 and  C_AccountNumber='"+C_AccountNumber+"'");

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
       
            if (obj != null)
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
            

        }

        void GetNewBusinessNO(HttpContext context)
        {

            string C_AccountNumber = context.Request.QueryString["txtC_AccountNumber"].ToString();

            string BusinessNO = "001";
            //查询当前社员的业务编号
            StringBuilder strSqlNO = new StringBuilder();
            strSqlNO.Append("  SELECT TOP 1 BusinessNO ");
            strSqlNO.Append("  FROM dbo.C_OperateLog  ");
            strSqlNO.Append("  WHERE C_AccountNumber='"+C_AccountNumber+"'");
            strSqlNO.Append("  ORDER BY dt_Trade DESC,ID DESC");
            object obj = SQLHelper.ExecuteScalar(strSqlNO.ToString());
            if (obj != null&&obj.ToString()!="") { 
                int BNO=Convert.ToInt32(obj)+1;
                BusinessNO = Fun.ConvertIntToString(BNO, 4);
            }

            context.Response.Write(BusinessNO);
        }


        //添加供销产品购买列表
        void UpdateSupplyList(HttpContext context)
        {


            DataTable dtSupply;
            if (context.Cache["Supply"] != null)
            {
                dtSupply = (DataTable)context.Cache["Supply"];
            }
            else
            {
                dtSupply = new DataTable();
                dtSupply.Columns.Add("numIndex", typeof(int));
                dtSupply.Columns.Add("SID", typeof(string));//商品编号
                dtSupply.Columns.Add("SName", typeof(string));//商品名
                dtSupply.Columns.Add("SpecName", typeof(string));//规格
                dtSupply.Columns.Add("UnitName", typeof(string));//单位
                dtSupply.Columns.Add("SCount", typeof(string));//商品数量
                dtSupply.Columns.Add("SPrice", typeof(string));//商品单价
                dtSupply.Columns.Add("SPrice_Commune", typeof(string));//社员优惠价
                dtSupply.Columns.Add("SPrice_WBBack", typeof(string));//网点返利价
                dtSupply.Columns.Add("numDisCount", typeof(string));//折率
                dtSupply.Columns.Add("SMoney", typeof(string));//总金额
                dtSupply.Columns.Add("SMoney_Commune", typeof(string));//优惠金额
                dtSupply.Columns.Add("SMoney_WBBack", typeof(string));//返利金额
                dtSupply.Columns.Add("SMoney_PreDefine", typeof(string));//预付款金额
                dtSupply.Columns.Add("SMoney_Reality", typeof(string));//社员实付金额

                context.Cache.Insert("Supply", dtSupply, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);//将购买商品暂时放入缓存
            }
            int newindex = 0;//新的目录索引
            if (dtSupply.Rows.Count > 0)
            {
                for (int i = 0; i < dtSupply.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtSupply.Rows[i]["numIndex"]) > newindex)
                    {
                        newindex = Convert.ToInt32(dtSupply.Rows[i]["numIndex"]);
                    }
                }
            }
            newindex += 1;

            string GoodSupplyID = context.Request.Form["GoodSupplyID"].ToString();
            string GoodSupplyName = context.Request.Form["S_GoodSupplyID"].ToString();
            string SpecName = context.Request.Form["S_SpecName"].ToString();
            string UnitName = context.Request.Form["S_UnitName"].ToString();
            double GoodSupplyCount = Convert.ToDouble( context.Request.Form["GoodSupplyCount"]);//商品数量
            double S_Price = Convert.ToDouble(context.Request.Form["S_Price"]);//价格
            double Price_Commune = Convert.ToDouble(context.Request.Form["S_Price_Commune"]);//社员优惠价
            double S_Price_WBBack = Convert.ToDouble(context.Request.Form["S_Price_WBBack"]);//网点返利

            double numDisCount =Convert.ToDouble( context.Request.Form["S_numDisCount"]);

            double SMoney = GoodSupplyCount*S_Price;
            double SMoney_Commune = GoodSupplyCount *(S_Price- Price_Commune);
            double SMoney_WBBack = GoodSupplyCount * S_Price_WBBack;
            double SMoney_PreDefine = Convert.ToDouble(context.Request.Form["S_Money_PreDefine"]);//预付款

            double SMoney_Reality = SMoney - SMoney_Commune-SMoney_PreDefine;//实付款

            if (SMoney_PreDefine >= SMoney - SMoney_Commune)
            {
                SMoney_PreDefine = SMoney - SMoney_Commune;//使用的预付款金额
                SMoney_Reality = 0;
            }
            dtSupply.Rows.Add(newindex, GoodSupplyID,GoodSupplyName,SpecName,UnitName,GoodSupplyCount,S_Price,Price_Commune,S_Price_WBBack,numDisCount, SMoney,SMoney_Commune,SMoney_WBBack, SMoney_PreDefine,SMoney_Reality);

            double T_SMoney = 0;//商品总金额
            double T_SMoney_Commune = 0;//优惠总金额
            double T_SMoney_WBBack = 0;//网点返利总金额
            double T_SMoney_PreDefine = 0;//预付款总金额
            double T_SMoney_Reality = 0;//实付款总金额

            //添加返回信息
            StringBuilder strReturn = new StringBuilder();//要返回的信息

            strReturn.Append("  <table class='tabData' style='margin:10px 0px;'>");

            strReturn.Append("  <tr><td colspan='11\' style='font-weight:bolder; height:25px; color:Green; font-size:16px;'>购买商品信息</td></tr>");
            strReturn.Append("  <tr class='tr_head'>");
            strReturn.Append("  <th style='width: 120px; height:30px; text-align: center;'> 商品名 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 规格型号 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 单位 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 数量 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  单价 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 优惠价 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  总金额 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 优惠金额 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  预付款 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  实付金额 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  删除 </th>");
            strReturn.Append(" </tr>");
            for (int i = 0; i < dtSupply.Rows.Count; i++)
            {
                DataRow row = dtSupply.Rows[i];
                T_SMoney += Convert.ToDouble(row["SMoney"]);
                T_SMoney_Commune += Convert.ToDouble(row["SMoney_Commune"]);
                T_SMoney_WBBack += Convert.ToDouble(row["SMoney_WBBack"]);
                T_SMoney_PreDefine += Convert.ToDouble(row["SMoney_PreDefine"]);
                T_SMoney_Reality += Convert.ToDouble(row["SMoney_Reality"]);
                strReturn.Append("<tr>");
                strReturn.Append("  <td style='height:25px;'><span style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["SName"].ToString() + "</span></td>");

                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["SpecName"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["UnitName"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + Math.Round( Convert.ToDouble( row["SCount"]),2) + "</span></td>");

                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SPrice"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SPrice_Commune"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SMoney"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SMoney_Commune"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SMoney_PreDefine"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SMoney_Reality"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'><input type='button' value='删除' style='width:60px;' onclick='FunDelList(" + row["numIndex"] + "," + row["SMoney_PreDefine"] + ");'></span></td>");

                strReturn.Append("    </tr>");

            }
            strReturn.Append("  <tr>");
            strReturn.Append("   <td colspan='11' style='text-align:left'>");
            strReturn.Append("   消费总计:<span  style='color:Red'>￥" + Math.Round(T_SMoney, 2) + "元</span>,");
            strReturn.Append("   优惠总计<span style='color:Red'>￥" + Math.Round(T_SMoney_Commune, 2) + "元</span>");
            strReturn.Append("   预付款总计<span style='color:Red' >￥" + Math.Round(T_SMoney_PreDefine, 2) + "元,</span>");
            strReturn.Append("  实付总计：<span style='color:Red'>￥" + Math.Round(T_SMoney_Reality, 2) + "元</span>&nbsp;");
           

            strReturn.Append("  </td>");
            strReturn.Append("  </tr>");
            strReturn.Append("  </table>");
            strReturn.Append(" ");




            context.Response.Write(strReturn.ToString());
        }

        void DeleteSupplyList(HttpContext context)
        {

            string numIndex = context.Request.QueryString["numIndex"].ToString();
            DataTable dtSupply = null;
            if (context.Cache["Supply"] != null)
            {
                dtSupply = (DataTable)context.Cache["Supply"];
            }
            else
            {

            }
            int rowindex = 0;//要删除的行索引
            for (int i = 0; i < dtSupply.Rows.Count; i++)
            {
                if (dtSupply.Rows[i]["numIndex"].ToString() == numIndex.ToString())
                {
                    break;//找到目标行，跳出循环
                }
                else
                {
                    rowindex += 1;
                }
            }

            dtSupply.Rows.RemoveAt(rowindex);//移除指定的行


            double T_SMoney = 0;//商品总金额
            double T_SMoney_Commune = 0;//优惠总金额
            double T_SMoney_WBBack = 0;//网点返利总金额
            double T_SMoney_PreDefine = 0;//预付款总金额
            double T_SMoney_Reality = 0;//实付款总金额

            //添加返回信息
            StringBuilder strReturn = new StringBuilder();//要返回的信息

            strReturn.Append("  <table class='tabData' style='margin:10px 0px;'>");

            strReturn.Append("  <tr><td colspan='11\' style='font-weight:bolder; height:25px; color:Green; font-size:16px;'>购买商品信息</td></tr>");
            strReturn.Append("  <tr class='tr_head'>");
            strReturn.Append("  <th style='width: 120px; height:30px; text-align: center;'> 商品名 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 规格型号 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 单位 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 数量 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  单价 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 优惠价 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  总金额 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 优惠金额 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  预付款 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  实付金额 </th>");
            strReturn.Append(" </tr>");
            for (int i = 0; i < dtSupply.Rows.Count; i++)
            {
                DataRow row = dtSupply.Rows[i];
                T_SMoney += Convert.ToDouble(row["SMoney"]);
                T_SMoney_Commune += Convert.ToDouble(row["SMoney_Commune"]);
                T_SMoney_WBBack += Convert.ToDouble(row["SMoney_WBBack"]);
                T_SMoney_PreDefine += Convert.ToDouble(row["SMoney_PreDefine"]);
                T_SMoney_Reality += Convert.ToDouble(row["SMoney_Reality"]);
                strReturn.Append("<tr>");
                strReturn.Append("  <td style='height:25px;'><span style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["SName"].ToString() + "</span></td>");

                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["SpecName"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["UnitName"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + Math.Round(Convert.ToDouble(row["SCount"]), 2) + "</span></td>");

                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SPrice"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SPrice_Commune"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SMoney"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SMoney_Commune"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SMoney_PreDefine"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>￥" + Math.Round(Convert.ToDouble(row["SMoney_Reality"]), 2) + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'><input type='button' value='删除' style='width:60px;' onclick='FunDelList(" + row["numIndex"] + "," + row["SMoney_PreDefine"] + ");'></span></td>");

                strReturn.Append("    </tr>");

            }
            strReturn.Append("  <tr>");
            strReturn.Append("   <td colspan='11' style='text-align:left'>");
            strReturn.Append("   消费总计:<span  style='color:Red'>￥" + Math.Round(T_SMoney, 2) + "元</span>,");
            strReturn.Append("   优惠总计<span style='color:Red'>￥" + Math.Round(T_SMoney_Commune, 2) + "元</span>");
            strReturn.Append("   预付款总计<span style='color:Red' >￥" + Math.Round(T_SMoney_PreDefine, 2) + "元,</span>");
            strReturn.Append("  实付总计：<span style='color:Red'>￥" + Math.Round(T_SMoney_Reality, 2) + "元</span>&nbsp;");


            strReturn.Append("  </td>");
            strReturn.Append("  </tr>");
            strReturn.Append("  </table>");
            strReturn.Append(" ");




            context.Response.Write(strReturn.ToString());
        }






        //添加供销产品交易信息
        void AddC_Supply(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = context.Session["ID"].ToString();
            //string C_AccountNumber = context.Request.Form["txtC_AccountNumber"].ToString();
            string C_AccountNumber = context.Request.Form["QAccountNumber"].ToString();
            string C_Name = context.Request.Form["txtC_Name"].ToString();
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();
            string BusinessName = "消费";

            DataTable dtSupply = null;
            if (context.Cache["Supply"] != null)
            {
                dtSupply = (DataTable)context.Cache["Supply"];
            }
            if (dtSupply == null && dtSupply.Rows.Count == 0)
            {

            }

            string BusinessNOList = "";//所有的兑换列表的集合


            for (int i = 0; i < dtSupply.Rows.Count; i++)
            {
                if (i != 0)
                {
                    BusinessNO = Fun.ConvertIntToString(Convert.ToInt32(BusinessNO) + 1, 4);//生成新的业务编号
                }
                if (BusinessNOList == "")
                {
                    BusinessNOList = BusinessNO;
                }
                else
                {
                    BusinessNOList = BusinessNOList + "|" + BusinessNO;
                }

                string strGUID = Fun.getGUID();
                string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + C_AccountNumber + BusinessNO;
                DataRow row = dtSupply.Rows[i];

                string GoodSupplyID = row["SID"].ToString();
                string GoodSupplyName = row["SName"].ToString();
                string SpecName = row["SpecName"].ToString();
                string UnitName = row["UnitName"].ToString();

                double GoodSupplyCount = Math.Round(Convert.ToDouble(row["SCount"]), 2);
                double GoodSupplyPrice = Math.Round(Convert.ToDouble(row["SPrice"]), 2);
                double Price_Commune = Math.Round(Convert.ToDouble(row["SPrice_Commune"]), 2);
                double numDisCount = Math.Round(Convert.ToDouble(row["numDisCount"]), 2);
                double Money_Total = Math.Round(Convert.ToDouble(row["SMoney"]), 2);
                double Money_YouHui = Math.Round(Convert.ToDouble(row["SMoney_Commune"]), 2);
                double Money_PreDefine = Math.Round(Convert.ToDouble(row["SMoney_PreDefine"]), 2);
                double Money_Reality = Math.Round(Convert.ToDouble(row["SMoney_Reality"]), 2);
                double Money_Back = Math.Round(Convert.ToDouble(row["SMoney_WBBack"]), 2);
                StringBuilder strSql = new StringBuilder();


                #region   商品销售记录
                strSql.Append("insert into [C_Supply] (");
                strSql.Append("SerialNumber,strGUID,BusinessNO,C_AccountNumber,C_Name,WBID,UserID,BusinessName,GoodSupplyID,GoodSupplyName,SpecName,UnitName,GoodSupplyCount,GoodSupplyPrice,Price_Commune,numDisCount,Money_Total,Money_YouHui,Money_PreDefine,Money_Reality,Money_Back,dt_Trade)");
                strSql.Append(" values (");
                strSql.Append("@SerialNumber,@strGUID,@BusinessNO,@C_AccountNumber,@C_Name,@WBID,@UserID,@BusinessName,@GoodSupplyID,@GoodSupplyName,@SpecName,@UnitName,@GoodSupplyCount,@GoodSupplyPrice,@Price_Commune,@numDisCount,@Money_Total,@Money_YouHui,@Money_PreDefine,@Money_Reality,@Money_Back,@dt_Trade)");
                strSql.Append(";select @@IDENTITY");
                SqlParameter[] parameters = {
                    new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
                    new SqlParameter("@strGUID", SqlDbType.NVarChar,50),
                    new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
                    new SqlParameter("@C_AccountNumber", SqlDbType.NVarChar,50),
                    new SqlParameter("@C_Name", SqlDbType.NVarChar,50),
                    new SqlParameter("@WBID", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
                    new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
                    new SqlParameter("@GoodSupplyID", SqlDbType.Int,4),
                    new SqlParameter("@GoodSupplyName", SqlDbType.NVarChar,50),
                    new SqlParameter("@SpecName", SqlDbType.NVarChar,50),
                    new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
                    new SqlParameter("@GoodSupplyCount", SqlDbType.Decimal,9),
                    new SqlParameter("@GoodSupplyPrice", SqlDbType.Decimal,9),
                    new SqlParameter("@Price_Commune", SqlDbType.Decimal,9),
                    new SqlParameter("@numDisCount", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_Total", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_YouHui", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_PreDefine", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_Back", SqlDbType.Decimal,9),
                    new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
                parameters[0].Value = SerialNumber;
                parameters[1].Value = strGUID;
                parameters[2].Value = BusinessNO;
                parameters[3].Value = C_AccountNumber;
                parameters[4].Value = C_Name;
                parameters[5].Value = WBID;
                parameters[6].Value = UserID;
                parameters[7].Value = BusinessName;
                parameters[8].Value = GoodSupplyID;
                parameters[9].Value = GoodSupplyName;
                parameters[10].Value = SpecName;
                parameters[11].Value = UnitName;
                parameters[12].Value = GoodSupplyCount;
                parameters[13].Value = GoodSupplyPrice;
                parameters[14].Value = Price_Commune;
                parameters[15].Value = numDisCount;
                parameters[16].Value = Money_Total;
                parameters[17].Value = Money_YouHui;
                parameters[18].Value = Money_PreDefine;
                parameters[19].Value = Money_Reality;
                parameters[20].Value = Money_Back;
                parameters[21].Value = DateTime.Now;
                #endregion

                //减去网店相应商品的库存
                string strSqlStorage = " UPDATE dbo.GoodSupplyStorage SET numStore=numStore-" + GoodSupplyCount + " WHERE GoodSupplyID=" + GoodSupplyID + " AND WBID=" + WBID;


                #region  添加购买日志
                StringBuilder strSqlLog = new StringBuilder();
                strSqlLog.Append("insert into [C_OperateLog] (");
                strSqlLog.Append("WBID,UserID,C_AccountNumber,BusinessNO,BusinessName,VarietyID,Price,UnitID,Price_C,CountTrade,Money_Trade,Money_YouHui,numDisCount,Money_Reality,dt_Trade)");
                strSqlLog.Append(" values (");
                strSqlLog.Append("@WBID,@UserID,@C_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@Price,@UnitID,@Price_C,@CountTrade,@Money_Trade,@Money_YouHui,@numDisCount,@Money_Reality,@dt_Trade)");
                strSqlLog.Append(";select @@IDENTITY");
                SqlParameter[] parametersLog = {
                    new SqlParameter("@WBID", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
                    new SqlParameter("@C_AccountNumber", SqlDbType.NVarChar,50),
                    new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
                    new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
                    new SqlParameter("@VarietyID", SqlDbType.NVarChar,50),
                    new SqlParameter("@Price", SqlDbType.Decimal,9),
                    new SqlParameter("@UnitID", SqlDbType.NVarChar,50),
                    new SqlParameter("@Price_C", SqlDbType.Decimal,9),
                    new SqlParameter("@CountTrade", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_Trade", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_YouHui", SqlDbType.NChar,10),
                    new SqlParameter("@numDisCount", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
                    new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
                parametersLog[0].Value = WBID;
                parametersLog[1].Value = UserID;
                parametersLog[2].Value = C_AccountNumber;
                parametersLog[3].Value = BusinessNO;
                parametersLog[4].Value = BusinessName;
                parametersLog[5].Value = GoodSupplyName;
                parametersLog[6].Value = GoodSupplyPrice;
                parametersLog[7].Value = UnitName;
                parametersLog[8].Value = GoodSupplyPrice;
                parametersLog[9].Value = GoodSupplyCount;
                parametersLog[10].Value = Money_Total;
                parametersLog[11].Value = Money_YouHui;
                parametersLog[12].Value = numDisCount;
                parametersLog[13].Value = Money_Reality;
                parametersLog[14].Value = DateTime.Now;
                #endregion


                #region  添加事务处理
                using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
                {
                    try
                    {


                        object C_SupplyID = SQLHelper.ExecuteScalar(tran, CommandType.Text, strSql.ToString(), parameters);//添加社员购买信息
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlLog.ToString(), parametersLog);//添加购买日志
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlStorage);//减去商品库存

                        //当存在预付款使用的时候
                        if (Money_PreDefine > 0)
                        {
                            BusinessNO = Fun.ConvertIntToString(Convert.ToInt16(BusinessNO) + 1, 4);
                            BusinessNOList = BusinessNOList + "|" + BusinessNO;
                            #region  添加预付款日志

                            StringBuilder strSqlLogConmune = new StringBuilder();
                            strSqlLogConmune.Append("insert into [C_OperateLog] (");
                            strSqlLogConmune.Append("WBID,UserID,C_AccountNumber,BusinessNO,BusinessName,VarietyID,Price,UnitID,Price_C,CountTrade,Money_Trade,Money_YouHui,numDisCount,Money_Reality,dt_Trade)");
                            strSqlLogConmune.Append(" values (");
                            strSqlLogConmune.Append("@WBID,@UserID,@C_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@Price,@UnitID,@Price_C,@CountTrade,@Money_Trade,@Money_YouHui,@numDisCount,@Money_Reality,@dt_Trade)");
                            strSqlLogConmune.Append(";select @@IDENTITY");
                            SqlParameter[] parametersLogConMune = {
                    new SqlParameter("@WBID", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
                    new SqlParameter("@C_AccountNumber", SqlDbType.NVarChar,50),
                    new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
                    new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
                    new SqlParameter("@VarietyID", SqlDbType.NVarChar,50),
                    new SqlParameter("@Price", SqlDbType.Decimal,9),
                    new SqlParameter("@UnitID", SqlDbType.NVarChar,50),
                    new SqlParameter("@Price_C", SqlDbType.Decimal,9),
                    new SqlParameter("@CountTrade", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_Trade", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_YouHui", SqlDbType.NChar,10),
                    new SqlParameter("@numDisCount", SqlDbType.Decimal,9),
                    new SqlParameter("@Money_Reality", SqlDbType.Decimal,9),
                    new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
                            parametersLogConMune[0].Value = WBID;
                            parametersLogConMune[1].Value = UserID;
                            parametersLogConMune[2].Value = C_AccountNumber;
                            parametersLogConMune[3].Value = BusinessNO;
                            parametersLogConMune[4].Value = "消费";
                            parametersLogConMune[5].Value = "预付款";
                            parametersLogConMune[6].Value = 0;
                            parametersLogConMune[7].Value = "";
                            parametersLogConMune[8].Value = 0;
                            parametersLogConMune[9].Value = 0;
                            parametersLogConMune[10].Value = 0;
                            parametersLogConMune[11].Value = 0;
                            parametersLogConMune[12].Value = 0;
                            parametersLogConMune[13].Value = Money_PreDefine;//预付款金额
                            parametersLogConMune[14].Value = DateTime.Now;
                            #endregion


                            #region 预付款使用记录
                            //添加预存款消费记录
                            //向数据表写入预付款使用记录
                            StringBuilder strSqlConsume = new StringBuilder();
                            strSqlConsume.Append("insert into [C_PreDefineConsume] (");
                            strSqlConsume.Append("WBID,UserID,BusinessNO,C_AccountNumber,C_PreDefineID,C_SupplyID,Money_PreDefine,dt_Trade)");
                            strSqlConsume.Append(" values (");
                            strSqlConsume.Append("@WBID,@UserID,@BusinessNO,@C_AccountNumber,@C_PreDefineID,@C_SupplyID,@Money_PreDefine,@dt_Trade)");
                            strSqlConsume.Append(";select @@IDENTITY");
                            SqlParameter[] parametersConsume = {
                    new SqlParameter("@WBID", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
                    new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
                    new SqlParameter("@C_AccountNumber", SqlDbType.NVarChar,50),
                    new SqlParameter("@C_PreDefineID", SqlDbType.Int,4),
                    new SqlParameter("@C_SupplyID", SqlDbType.Int,4),
                    new SqlParameter("@Money_PreDefine", SqlDbType.Decimal,9),
                    new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
                            parametersConsume[0].Value = WBID;
                            parametersConsume[1].Value = UserID;
                            parametersConsume[2].Value = BusinessNO;
                            parametersConsume[3].Value = C_AccountNumber;
                            parametersConsume[4].Value = 2;
                            parametersConsume[5].Value = 0;
                            parametersConsume[6].Value = Money_PreDefine;
                            parametersConsume[7].Value = DateTime.Now;
                            #endregion

                            //更改用户的预付款剩余金额
                            string strSqlPreDefineUpdate = "  UPDATE dbo.C_PreDefine SET Money_PreDefine=Money_PreDefine-" + Money_PreDefine + " WHERE C_AccountNumber='" + C_AccountNumber + "'";


                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlLogConmune.ToString(), parametersLogConMune);//减去商品库存
                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlConsume.ToString(), parametersConsume);//减去商品库存
                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlPreDefineUpdate);//减去商品库存
                        }

                        tran.Commit();

                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
                #endregion


            }

            context.Session["SupplyList"] = BusinessNOList;//保存此次所有兑换的参数
            context.Cache.Remove("Supply");//结账之后将兑换条目的缓存清除

            context.Response.Write("OK");


        }


     
        /// <summary>
        /// 由当前登陆用户的ID获取其所在的网点信息
        /// </summary>
        /// <param name="context"></param>
        void GetWebSiteByUserID(HttpContext context)
        {
            string ID = context.Session["ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.ID,A.SerialNumber,A.strName,A.strAddress,A.WBType_ID,numSettle,numAgent,ISAllowBackUp,ISHQ ");
            strSql.Append(" FROM dbo.WB A INNER JOIN dbo.Users B ON A.ID=B.WB_ID");
            strSql.Append(" WHERE B.ID= " + ID);
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

        void Get_Address_Xian(HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName FROM dbo.BD_Address_Xian ");
            strSql.Append(" ORDER BY ISDefault DESC,numSort ASC ");

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

        void Get_Address_Xiang(HttpContext context)
        {
         
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName,WBID FROM dbo.BD_Address_Xiang ");
            if (context.Request.QueryString["XianID"] != null)
            {
                string XianID = context.Request.QueryString["XianID"].ToString();
                strSql.Append(" where XianID=" + XianID);
            }
           
            strSql.Append(" ORDER BY ISDefault DESC,numSort ASC ");

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

        void Get_Address_Cun(HttpContext context)
        {
            
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName,WBID FROM dbo.BD_Address_Cun ");
            if (context.Request.QueryString["XiangID"] != null)
            {
                string XiangID = context.Request.QueryString["XiangID"].ToString();
                strSql.Append(" where XiangID=" + XiangID);
            }
          
            strSql.Append(" ORDER BY ISDefault DESC,numSort ASC ");

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

        void Get_Address_Zu(HttpContext context)
        {
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName FROM dbo.BD_Address_Zu ");
            if (context.Request.QueryString["CunID"] != null)
            {
                string CunID = context.Request.QueryString["CunID"].ToString();
                strSql.Append(" where CunID=" + CunID);
            }
          
            strSql.Append(" ORDER BY ISDefault DESC,numSort ASC ");

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

        //获取新的社员编号
        void GetNewAccountNumber(HttpContext context)
        {
            string WB_ID = context.Session["WB_ID"].ToString();

            string strSqlNum = "SELECT SerialNumber FROM dbo.WB WHERE ID=" + WB_ID;//获取网点编号
            string SerialNumber = SQLHelper.ExecuteScalar(strSqlNum).ToString();
            string AccountNumver ="C"+ SerialNumber + "0000001";

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT TOP 1 AccountNumber  FROM  dbo.Commune ");
            strSql.Append("  WHERE WBID=" + WB_ID);
            strSql.Append("  ORDER BY AccountNumber DESC ");

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());

            if (obj != null)
            {
                int numIndex = Convert.ToInt32(obj.ToString().Substring(4));
                AccountNumver ="C"+ SerialNumber + Fun.ConvertIntToString(numIndex + 1, 7);
                context.Response.Write(AccountNumver);
            }
            else
            {
                context.Response.Write(AccountNumver);
            }
        }


        void GetByAccountNumver_Depositor(HttpContext context)
        {
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select ID,WBID,AccountNumber,strPassword,CunID as BD_Address_CunID,strAddress,strName,IDCard,PhoneNO,ISSendMessage,BankCardNO,numState,dt_Add,dt_Update");
            strSql.Append(" FROM dbo.Depositor ");

            strSql.Append(" where AccountNumber=@AccountNumber ");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50)};
            parameters[0].Value = AccountNumber;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);

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
        /// 获取社员的打印信息
        /// </summary>
        /// <param name="context"></param>
        void GetCommunePrint(HttpContext context)
        {
            //获取需要打印的信息
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            StringBuilder strSqlCommume = new StringBuilder();
            strSqlCommume.Append("select A.ID,B.strName AS  WBID,A. AccountNumber,A.strAddress,A.strName,A.IDCard,A.PhoneNO,CONVERT(NVARCHAR(100),dt_Commune,23)AS  dt_Commune,A.FieldCount");
            strSqlCommume.Append(" FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID ");
            strSqlCommume.Append(" where A.AccountNumber=@AccountNumber ");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50)};
            parameters[0].Value = AccountNumber;

            DataTable dtCommune = SQLHelper.ExecuteDataTable(strSqlCommume.ToString(), parameters);
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
            string dt_Commune = dtCommune.Rows[0]["dt_Commune"].ToString();
            string FieldCount = dtCommune.Rows[0]["FieldCount"].ToString();


            string strWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT  ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting] ";
            strSql += "  where 1=1  and WBID=" + strWBID;
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
                strReturn.Append("   <td style='font-size:20px; font-weight:bolder; width:" + (Convert.ToInt32(HomeR1C2X) - Convert.ToInt32(HomeR1C1X)).ToString() + "px;'>"+AccountNumber+"</td>");
                strReturn.Append("   <td style='font-size:20px; font-weight:bolder;'>"+strName+"</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR3C1Y) - Convert.ToInt32(HomeR2C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR2C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR2C2X) - Convert.ToInt32(HomeR2C1X)).ToString() + "px;'>"+strAddress+"</td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;'>"+IDCard+"</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR4C1Y) - Convert.ToInt32(HomeR3C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR3C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR3C2X) - Convert.ToInt32(HomeR3C1X)).ToString() + "px;'>"+WBID+"</td>");
                strReturn.Append("  <td style='font-size:14px; font-weight:bold;'>"+PhoneNO+"</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR5C1Y) - Convert.ToInt32(HomeR4C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR4C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR4C2X) - Convert.ToInt32(HomeR4C1X)).ToString() + "px;'>"+dt_Commune+"</td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;' >"+FieldCount+"</td>");

                strReturn.Append("   </tr></table>");


                context.Response.Write(strReturn.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Add_Commune(HttpContext context)
        {

            string WBID = context.Session["WB_ID"].ToString();
            string strName = context.Request.Form["txtstrName"].ToString();
            string AccountNumber = context.Request.Form["txtAccountNumber"].ToString();//社员账号
          //  string strPassword = context.Request.Form["txtstrPassword"].ToString();
           // strPassword = Fun.GetMD5_32(strPassword);
            string XianID = context.Request.Form["XianID"].ToString();
            string XiangID = context.Request.Form["XiangID"].ToString();
            string CunID = context.Request.Form["CunID"].ToString();
            string ZuID = context.Request.Form["ZuID"].ToString();
            string strAddress = context.Request.Form["strAddress"].ToString();
            string IDCard = context.Request.Form["txtIDCard"].ToString();
            string PhoneNO = context.Request.Form["txtPhoneNO"].ToString();

            string FieldCopies = context.Request.Form["txtFieldCopies"].ToString();
            string FieldCount = context.Request.Form["txtFiledCount"].ToString();
            string ApplicationForm = context.Request.Form["txtApplicationFileName"].ToString();
            string CommunePic = context.Request.Form["txtCommuneFileName"].ToString();

            string strSameAccount = "  SELECT  COUNT(ID)  FROM dbo.Commune WHERE AccountNumber='" + AccountNumber + "'";
            if (SQLHelper.ExecuteScalar(strSameAccount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }

            string strCount = "  SELECT  COUNT(ID)  FROM dbo.Commune WHERE FieldCopies='" + FieldCopies + "' or IDCard='" + IDCard + "'";
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Commune] (");
            strSql.Append("WBID,AccountNumber,strPassword,XianID,XiangID,CunID,ZuID,strAddress,strName,IDCard,PhoneNO,FieldCopies,FieldCount,ApplicationForm,CommunePic,dt_Commune,numState)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@AccountNumber,@strPassword,@XianID,@XiangID,@CunID,@ZuID,@strAddress,@strName,@IDCard,@PhoneNO,@FieldCopies,@FieldCount,@ApplicationForm,@CommunePic,@dt_Commune,@numState)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@strPassword", SqlDbType.NVarChar,50),
					new SqlParameter("@XianID", SqlDbType.Int,4),
					new SqlParameter("@XiangID", SqlDbType.Int,4),
					new SqlParameter("@CunID", SqlDbType.Int,4),
					new SqlParameter("@ZuID", SqlDbType.Int,4),
					new SqlParameter("@strAddress", SqlDbType.NVarChar,100),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@IDCard", SqlDbType.NVarChar,50),
					new SqlParameter("@PhoneNO", SqlDbType.NVarChar,50),
					new SqlParameter("@FieldCopies", SqlDbType.NVarChar,50),
					new SqlParameter("@FieldCount", SqlDbType.Decimal,9),
					new SqlParameter("@ApplicationForm", SqlDbType.NVarChar,200),
					new SqlParameter("@CommunePic", SqlDbType.NVarChar,200),
					new SqlParameter("@dt_Commune", SqlDbType.DateTime),
					new SqlParameter("@numState", SqlDbType.Int,4)};
            parameters[0].Value = WBID;
            parameters[1].Value = AccountNumber;
            parameters[2].Value = "";
            parameters[3].Value = XianID;
            parameters[4].Value = XiangID;
            parameters[5].Value = CunID;
            parameters[6].Value = ZuID;
            parameters[7].Value = strAddress;
            parameters[8].Value = strName;
            parameters[9].Value = IDCard;
            parameters[10].Value = PhoneNO;
            parameters[11].Value = FieldCopies;
            parameters[12].Value = FieldCount;
            parameters[13].Value = ApplicationForm;
            parameters[14].Value = CommunePic;
            parameters[15].Value = DateTime.Now;
            parameters[16].Value = 1;
           

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void Update_Commune(HttpContext context)
        {

            string WBID = context.Session["WB_ID"].ToString();
            string strName = context.Request.Form["txtstrName"].ToString();
            string AccountNumber = context.Request.Form["txtAccountNumber"].ToString();
           
            string IDCard = context.Request.Form["txtIDCard"].ToString();
            string PhoneNO = context.Request.Form["txtPhoneNO"].ToString();

            string FieldCopies = context.Request.Form["txtFieldCopies"].ToString();
            string FieldCount = context.Request.Form["txtFiledCount"].ToString();
            string ApplicationForm = context.Request.Form["txtApplicationFileName"].ToString();
            string CommunePic = context.Request.Form["txtCommuneFileName"].ToString();
            string XianID = context.Request.Form["XianID"].ToString();
            string XiangID = context.Request.Form["XiangID"].ToString();
            string CunID = context.Request.Form["CunID"].ToString();
            string ZuID = context.Request.Form["ZuID"].ToString();
            string strXian = SQLHelper.ExecuteScalar("  SELECT strName  FROM dbo.BD_Address_Xian WHERE ID="+XianID).ToString();
            string strXiang = SQLHelper.ExecuteScalar("  SELECT strName  FROM dbo.BD_Address_Xiang WHERE ID=" + XiangID).ToString();
            string strCun = SQLHelper.ExecuteScalar("  SELECT strName  FROM dbo.BD_Address_Cun WHERE ID=" + CunID).ToString();
            string strZu = SQLHelper.ExecuteScalar("  SELECT strName  FROM dbo.BD_Address_Zu WHERE ID=" + ZuID).ToString();
            string strAddress = strXian + " " + strXiang + " " + strCun + " " + strZu;

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" update  [Commune]  set");
            strSql.Append("  strName='" + strName + "'");
            strSql.Append("  ,IDCard='" + IDCard + "'");
            strSql.Append("  ,XianID=" + XianID);
            strSql.Append("  ,XiangID=" + XiangID);
            strSql.Append("  ,CunID=" + CunID);
            strSql.Append("  ,ZuID=" + ZuID);
            strSql.Append("  ,strAddress='" + strAddress + "'");
            strSql.Append("  ,PhoneNO='" + PhoneNO + "'");
            strSql.Append("  ,FieldCopies='" + FieldCopies + "'");
            strSql.Append("  ,FieldCount=" + FieldCount);
            if (ApplicationForm.Trim() != "")
            {
                strSql.Append("  ,ApplicationForm='" + ApplicationForm + "'");
            }
            if (CommunePic.Trim() != "")
            {
                strSql.Append("  ,CommunePic='" + CommunePic + "'");
            }
          
            strSql.Append("  where AccountNumber='"+AccountNumber+"'");

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        /// <summary>
        /// 更新用户状态（挂失与解除挂失）
        /// </summary>
        /// <param name="context"></param>
        void Update_CommuneState(HttpContext context)
        {
            string AccountNumber = context.Request.QueryString["AN"].ToString();
            string numState = context.Request.Form["GuaShi"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" UPDATE dbo.Commune SET numState=" + numState);
            strSql.Append("  WHERE AccountNumber='" + AccountNumber + "'");


            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        /// <summary>
        /// 更新用户状态（挂失与解除挂失）
        /// </summary>
        /// <param name="context"></param>
        void CloseCommune(HttpContext context)
        {
            string AccountNumber = context.Request.QueryString["AN"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" delete dbo.Commune ");
            strSql.Append("  WHERE AccountNumber='" + AccountNumber + "'");


            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetCommuneByAccountNumber(HttpContext context)
        {

            string AccountNumber = context.Request.QueryString["txtAccountNumber"].ToString();
           

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT A.ID,B.strName AS WBID,F.ID AS XianID,E.ID AS XiangID,D.ID AS CunID,C.ID AS ZuID,");
            strSql.Append(" A.AccountNumber,A.strPassword,A.strName,A.IDCard,A.PhoneNO,A.FieldCopies,A.FieldCount,A.ApplicationForm,A.CommunePic,A.dt_Commune");
            strSql.Append("  ,F.strName +' '+E.strName +' '+D.strName+' '+C.strName AS strAddress, CONVERT(varchar(100), dt_Commune, 23) AS strTitme");
            strSql.Append(" FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append(" INNER JOIN dbo.BD_Address_Zu C ON A.ZuID=C.ID");
            strSql.Append(" INNER JOIN dbo.BD_Address_Cun D ON A.CunID=D.ID");
            strSql.Append(" INNER JOIN dbo.BD_Address_Xiang E ON A.XiangID=E.ID");
            strSql.Append(" INNER JOIN dbo.BD_Address_Xian F ON A.XianID=F.ID");
            strSql.Append(" WHERE A.AccountNumber='"+AccountNumber+"'");
            strSql.Append(" ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt!=null&&dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Address_Cun(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [BD_Address_Cun] set strName='" + strName + "' where ID= " + ID);


            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_Address_Cun(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [BD_Address_Cun] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = wbid;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
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