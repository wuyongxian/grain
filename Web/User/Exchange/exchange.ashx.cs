using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;
using System.Web.Mvc;
namespace Web.User.Exchange
{
    /// <summary>
    /// exchange 的摘要说明
    /// </summary>
    public class exchange : IHttpHandler, IRequiresSessionState
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
                    case "getDep_IntegralLast": getDep_IntegralLast(context); break;
                    case "getDepositorInfo": getDepositorInfo(context); break;
                    case "getDepositor_Simulate": getDepositor_Simulate(context); break;
                    case "getDepositorStorageInfo": getDepositorStorageInfo(context); break;
                    case "GetExchangePolicy": GetExchangePolicy(context); break;
                    case "GetExchangeVarietyCount": GetExchangeVarietyCount(context); break;
                    case "ISExchangeLimit": ISExchangeLimit(context); break;
                    case "UpdateExChangeList": UpdateExChangeList(context); break;
                    case "DeleteExChangeList": DeleteExChangeList(context); break;
                    case "Add_GoodExchange": Add_GoodExchange(context); break;
                    case "Add_GoodExchangeGroup": Add_GoodExchangeGroup(context); break;
                    case "Query_GoodExchangeGroup": Query_GoodExchangeGroup(context); break;
                    case "Add_GoodExchangeGroupDetail": Add_GoodExchangeGroupDetail(context); break;
                    case "Add_GoodExchangeGroupDetail_All": Add_GoodExchangeGroupDetail_All(context); break;
                    case "Get_GoodExchangeGroupDetail": Get_GoodExchangeGroupDetail(context); break;
                    case "Add_GoodSell": Add_GoodSell(context); break;
                    case "Add_GoodExchangeIntegral": Add_GoodExchangeIntegral(context); break;
                    case "PrintDep_OperateLogList": PrintDep_OperateLogList(context); break;
                    case "PrintGoodExchangeList": PrintGoodExchangeList(context); break;
                    case "PrintGoodExchangeGroupList": PrintGoodExchangeGroupList(context); break;
                    case "PrintGoodSellList": PrintGoodSellList(context); break;//打印储户购买商品信息
                    case "PrintGoodExchangeIntegral": PrintGoodExchangeIntegral(context); break;//打印积分兑换商品信息
                    case "Add_Dep_Storage": Add_Dep_Storage(context); break;
                    case "Update_Dep_Storage": Update_Dep_Storage(context); break;
                    case "GetByID_Dep_Storage": GetByID_Dep_Storage(context); break;
                    case "GetStorageInfoByID": GetStorageInfoByID(context); break;
                    case "GetInterestState": GetInterestState(context); break;
                    case "GetDepInfoByStorageID": GetDepInfoByStorageID(context); break;
                    case "GetStorageRateByID": GetStorageRateByID(context); break;
                    case "GetSRTByID": GetSRTByID(context); break;
                    case "StoreToSell": StoreToSell(context); break;//存转销
                    case "GetSellApplyByAN": GetSellApplyByAN(context); break;//存转销
                    case "StorageShopping": StorageShopping(context); break;//存转销
                        
                }
            }

        }


        void getPage(HttpContext context)
        {
            var res = new { ID = 1, Name = "name1" };
            string strName = res.Name;
            string str1 = context.Request.Form["stu"].ToString();
            
           DataTable dt= JsonHelper.JsonToDataTable(str1);
            context.Response.Write("Error");
        }


        object getValue() {
            var res = new { ID = 1, Name = "name1" };
            return res;
        }

        /// <summary>
        /// 获取储户最新的积分信息
        /// </summary>
        /// <param name="context"></param>
        void getDep_IntegralLast(HttpContext context)
        {
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();

            string sql = string.Format(" SELECT TOP 1  * FROM dbo.Dep_Integral WHERE AccountNumber='{0}' ORDER BY dt_Add DESC",AccountNumber);

            DataTable dt = SQLHelper.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count != 0)
            {

                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
               
                context.Response.Write("Error");
                return;
            }

        }

        /// <summary>
        /// 获取储户信息
        /// </summary>
        /// <param name="context"></param>
        void getDepositorInfo(HttpContext context)
        {
            var state = false;
            var msg = "";

            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string Password = context.Request.Form["Password"].ToString();
          
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  select A.ID,WBID,A.AccountNumber,strPassword, CunID as BD_Address_CunID,A.strAddress,A.strName,PhoneNO,ISSendMessage,BankCardNO,A.dt_Update,  ");
            strSql.Append("     numState,A.dt_Add,   CASE (IDCard) WHEN '' THEN '未填写' ELSE '******' END as IDCard,");
            strSql.Append("   B.ISHQ,B.ISSimulate ");
            strSql.Append("  FROM dbo.Depositor A INNER JOIN dbo.WB B ON A.WBID=B.ID  where 1=1 ");
            strSql.Append("  and A.ISClosing=0 ");//排除已经被销户的储户
            if (Convert.ToBoolean(common.GetWBAuthority()["Enable_Distance"]) == false)//不允许异地存取的情况
            {
                strSql.Append(" and WBID= " + WBID);
            }
            else
            {
                //当前登录的网店是模拟网点
                if (Convert.ToBoolean(context.Session["ISSimulate"]) == true)
                {
                    strSql.Append(" and WBID= " + WBID);
                }
            }

            strSql.Append(string.Format(" and A.AccountNumber='{0}'", AccountNumber));

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                if (Convert.ToBoolean(common.GetWBAuthority()["Enable_Distance"]) == true)//不允许异地存取的情况
                {
                    //储户是模拟网点账号且异地操作
                    if (Convert.ToInt32(dt.Rows[0]["ISSimulate"]) == 1 && WBID != dt.Rows[0]["WBID"].ToString())
                    {
                        var res = new { state = false, msg = "当前储户属于模拟网点的账号，不可以异地操作！" };
                        context.Response.Write(JsonHelper.ToJson(res));
                        return;
                    }

                }

                string numState = dt.Rows[0]["numState"].ToString();

                if (numState == "0")
                {
                    state = false;
                    msg = "您查询的账户已经申请挂失!";
                    var res = new { state = state, msg = msg };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;

                }
                //储户存粮记录
                var dep = JsonHelper.ToJson(dt);          
                state = true;
                msg = "查询库存记录成功!";
                var resValue = new { state = state, msg = msg, dep = dep };
                context.Response.Write(JsonHelper.ToJson(resValue));
            }
            else
            {
                state = false;
                msg = "您查询的储户不存在!";
                var res = new { state = state, msg = msg };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

        }

        /// <summary>
        /// 获取网点模拟储户(不存在的时候就添加一个)
        /// </summary>
        /// <param name="context"></param>
        void getDepositor_Simulate(HttpContext context)
        {

            string WBID = context.Session["WB_ID"].ToString();//当前网点ID

            string WB_SerialNumber = common.GetWBInfoByID(Convert.ToInt32(WBID))["SerialNumber"].ToString();
            string AccountNumber = WB_SerialNumber + "0000000";

            DataTable dt_Depositor = SQLHelper.ExecuteDataTable(string.Format(" SELECT * FROM dbo.Depositor WHERE AccountNumber='{0}'",AccountNumber));
            if (dt_Depositor == null || dt_Depositor.Rows.Count == 0)
            {
                StringBuilder sql_insert = new StringBuilder();
                sql_insert.Append(" INSERT INTO dbo.Depositor");
                sql_insert.Append(" ( WBID,AccountNumber,strPassword,strAddress,XianID,XiangID,CunID ,strName , IDCard ,PhoneNO , ISSendMessage , BankCardNO ,numState ,dt_Add , dt_Update , ISClosing )");
                sql_insert.Append(string.Format(" VALUES  ( {0} , N'{1}' , N'******' , N'******' ,  0 , 0 , 0 , N'模拟储户' ,N'***' , N'***' , 0 ,  N'***' ,  0 , GETDATE() , GETDATE(), 1)",WBID,AccountNumber));
                SQLHelper.ExecuteNonQuery(sql_insert.ToString());

                dt_Depositor = SQLHelper.ExecuteDataTable(string.Format(" SELECT * FROM dbo.Depositor WHERE AccountNumber='{0}'", AccountNumber));
            }

            context.Response.Write(JsonHelper.ToJson(dt_Depositor));


        }

        /// <summary>
        /// 获取储户信息及储户存储信息
        /// </summary>
        /// <param name="context"></param>
        void getDepositorStorageInfo(HttpContext context)
        {
            var state = false;
            var msg = "";
            
            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string Password = context.Request.Form["Password"].ToString();
            string ISVirtual = "";//"":不限制，“1”：预存粮,“0”：实际存粮（预存转实存页面使用参数）
            if (context.Request.Form["ISVirtual"] != null)
            {//当前请求是经审核过的存转销
                ISVirtual = context.Request.Form["ISVirtual"].ToString();
            }
            string ApplyID = "";//存转销审核页面使用参数
            if (context.Request.Form["ApplyID"] != null) {//当前请求是经审核过的存转销
                ApplyID = context.Request.Form["ApplyID"].ToString();
            }

            bool ISError = false;//修改错误存粮页面使用参数
            if (context.Request.Form["ISError"] != null)
            {//当前请求是经审核过的存转销
                ISError = true;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  select A.ID,WBID,AccountNumber,strPassword, CunID as BD_Address_CunID,A.strAddress,A.strName,PhoneNO,ISSendMessage,BankCardNO,A.dt_Update,   ");
            strSql.Append("     numState,A.dt_Add,   CASE (IDCard) WHEN '' THEN '未填写' ELSE '******' END as IDCard,");
            strSql.Append("   B.ISHQ,B.ISSimulate ");
            strSql.Append("  FROM dbo.Depositor A INNER JOIN dbo.WB B ON A.WBID=B.ID   where 1=1");
            strSql.Append("  and A.ISClosing=0 ");//排除已经被销户的储户
            if (Convert.ToBoolean(common.GetWBAuthority()["Enable_Distance"]) == false)//不允许异地存取的情况
            {
                strSql.Append(" and WBID= " + WBID);
            }
            else {
                //当前登录的网店是模拟网点
                if (Convert.ToBoolean(context.Session["ISSimulate"]) == true) 
                {
                    strSql.Append(" and WBID= " + WBID);
                }
            }

            strSql.Append(string.Format( " and AccountNumber='{0}'",AccountNumber));
          
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                if (Convert.ToBoolean(common.GetWBAuthority()["Enable_Distance"]) == true)//不允许异地存取的情况
                {
                    //储户是模拟网点账号且异地操作
                    if (Convert.ToInt32(dt.Rows[0]["ISSimulate"]) == 1 && WBID != dt.Rows[0]["WBID"].ToString())
                    {
                        var res = new { state = false, msg = "当前储户属于模拟网点的账号，不可以异地操作！" };
                        context.Response.Write(JsonHelper.ToJson(res));
                        return;
                    }              
                   
                }

                if (ApplyID == "")
                {
                    if (!common.CheckPassword(AccountNumber, Password))
                    {
                        state = false;
                        msg = "储户密码错误，请重新输入!";
                        var res = new { state = state, msg = msg };
                        context.Response.Write(JsonHelper.ToJson(res));
                        return;
                    }
                }


                string numState = dt.Rows[0]["numState"].ToString();

                if (numState == "0")
                {
                    state = false;
                    msg = "您查询的账户已经申请挂失!";
                    var res = new { state = state, msg = msg };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;

                }


                //获取存粮信息
                StringBuilder strSqlStorage = new StringBuilder();
                strSqlStorage.Append(" SELECT  A.ID, A.TypeID,A.StorageRateID,A.StorageNumber,convert(varchar(10),A.StorageDate,120) AS StorageDate,A.ISVirtual,");
                strSqlStorage.Append(" DATEDIFF( Day, A.StorageDate,GETDATE())AS daycount,");
                strSqlStorage.Append("  A.AccountNumber,B.ID AS VarietyID,A.VarietyLevelID, B.strName AS VarietyName,A.Price_ShiChang,A.Price_DaoQi,A.CurrentRate,A.WeighNo,");
                strSqlStorage.Append(" C.ID AS TimeID, C.strName AS TimeName,C.InterestType,A.StorageFee,D.strName AS UnitName");
                strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
                strSqlStorage.Append("  INNER JOIN dbo.StorageTime C ON A.TimeID=C.ID");
                strSqlStorage.Append("  LEFT JOIN dbo.BD_MeasuringUnit D ON B.MeasuringUnitID=D.ID");
             
                strSqlStorage.Append("  WHERE AccountNumber='" + AccountNumber + "'");
                strSqlStorage.Append("  and A.StorageNumber>0");

                if (ISVirtual != "") {
                    strSqlStorage.Append("  and A.ISVirtual="+ISVirtual);
                }

                if (ISError) {
                   bool ISHQ = common.ISHQWB(context.Session["WB_ID"]);//是否是总部的网点
                   if (!ISHQ) //非总部网点修改错误存粮
                   {
                       strSqlStorage.Append("  and  DATEDIFF(DAY,A.StorageDate,GETDATE())<1 ");//存储天数在一天之内的存粮
                   }
                }

                DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
                if (dtStorage == null || dtStorage.Rows.Count == 0)
                {
                    state = false;
                    msg = "当前的储户不存在存粮记录!";
                    var dep_r = JsonHelper.ToJson(dt);
                    var res = new { state = state, msg = msg, dep = dep_r };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }
                else {
                    dtStorage.Columns.Add("SellApplyCount", typeof(double));//添加申请存转销重量字段
                    for (int i = 0; i < dtStorage.Rows.Count; i++)
                    {
                        string dsiID = dtStorage.Rows[i]["ID"].ToString();
                        string sqlApply = string.Format(" SELECT SUM( VarietyCount) FROM dbo.StorageSellApply  WHERE Dep_SID={0}  AND ApplyState IN (0,1)", dsiID);
                        object objSellApplyCount = SQLHelper.ExecuteScalar(sqlApply);
                        if (objSellApplyCount != null && objSellApplyCount.ToString() != "")
                        {
                            dtStorage.Rows[i]["SellApplyCount"] = objSellApplyCount.ToString();
                        }
                        else
                        {
                            dtStorage.Rows[i]["SellApplyCount"] = 0;
                        }
                    }
                }
                DataColumn dcstrlixi=new DataColumn("strlixi",typeof(string));
                DataColumn dcnumlixi = new DataColumn("numlixi", typeof(string));
                dtStorage.Columns.Add(dcstrlixi);
                dtStorage.Columns.Add(dcnumlixi);
                for (int i = 0; i < dtStorage.Rows.Count; i++) {
                    Dictionary<string, string> dicLixi = common.GetLiXi_html(dtStorage.Rows[i]["ID"]);
                    string strlixi = dicLixi["strLixi"];
                    string numlixi = dicLixi["numLixi"];
                    dtStorage.Rows[i]["strlixi"] = strlixi;
                    dtStorage.Rows[i]["numlixi"] = numlixi;
                }
                //储户存粮记录
                var dep = JsonHelper.ToJson(dt);
                var storage = JsonHelper.ToJson(dtStorage);
                state = true;
                msg = "查询库存记录成功!";
                var resValue = new { state = state, msg = msg,dep=dep,storage=storage };
                context.Response.Write(JsonHelper.ToJson(resValue));
               
            }
            else
            {
                state = false;
                msg = "您查询的储户不存在!";
                var res = new { state = state, msg = msg };
                context.Response.Write(JsonHelper.ToJson(res));
                return; 
            }

        }

        /// <summary>
        /// 获取存储政策
        /// </summary>
        void GetExchangePolicy(HttpContext context)
        {
            string strPolicy = "";//兑换政策
            string Dep_SID = context.Request.Form["Dep_SID"].ToString();
            double numMoney = Convert.ToDouble(context.Request.Form["numMoney"]);
            double Exchange_trading = Convert.ToDouble(context.Request.Form["Exchange_trading"]);
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.ISRegular,B.InterestType,A.StorageNumber,A.Price_ShiChang,A.StorageDate,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID.ToString());

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                bool ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);//是否是定期类型
                int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
                if (ISRegular && InterestType == 3)
                { //按照定期取利息，并且按到期价取利息，确定是定期类型

                   // strPolicy = common.GetExPolicy_DingQi(Dep_SID, numMoney, Exchange_trading);
                    Dictionary<string, string> dic = common.GetEP_DingQi(Dep_SID, numMoney, Exchange_trading);
                    string daoqi = dic["daoqi"];
                    string youhui = dic["youhui"];
                    string msg = dic["msg"];
                    string Price_ShiChang = dic["Price_ShiChang"];

                    string Price_DaoQi = "";//到期价格
                    string YouHui_Count = "";//已优惠兑换数量
                    string SurPlue_Count = "";//剩余可优惠兑换数量
                    if (daoqi == "true")
                    {
                        Price_DaoQi = dic["Price_DaoQi"];
                    }
                    else {
                        if (youhui == "true") {
                            Price_DaoQi = dic["Price_DaoQi"];
                            YouHui_Count = dic["YouHui_Count"];
                            SurPlue_Count = dic["SurPlue_Count"];
                        }
                    }
                    var res = new { state = "success", ISRegular = true, strPolicy = msg, daoqi = daoqi, youhui = youhui, Price_ShiChang = Price_ShiChang, Price_DaoQi = Price_DaoQi, YouHui_Count = YouHui_Count, SurPlue_Count = SurPlue_Count };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                else
                {
                    //活期存储方式
                    strPolicy = common.GetExPolicy(Dep_SID, numMoney, Exchange_trading);
                    var res = new { state = "success", ISRegular = false, strPolicy = strPolicy };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
               
            }
            else
            {
                var res = new { state = "false", ISRegular = false, strPolicy = "" };
                context.Response.Write(JsonHelper.ToJson(res));
            }

        }

        //每月商品兑换数量是否被限额
        void ISExchangeLimit(HttpContext context) {
            double GoodCount =Convert.ToDouble( context.Request.Form["GoodCount"]);
            string GoodID = context.Request.Form["GoodID"].ToString();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();

            bool ISLimit = false;//是否被限额
            bool ISExchangeLimit = Convert.ToBoolean(common.GetWBAuthority()["ISExchangeLimit"]);
            double GoodExchangeLimit = 0;
            double DepExchangeCount = 0;
            if (!ISExchangeLimit) //不对兑换数量做限制
            {
                ISLimit = false;
            }
            else
            {
                //当前商品的兑换额度
               
                GoodExchangeLimit = Convert.ToDouble(SQLHelper.ExecuteScalar(" SELECT numExchangeLimit  FROM dbo.Good WHERE ID=" + GoodID));
                if (GoodExchangeLimit <= 0)
                {
                    ISLimit = false;
                }
                else
                {
                    //当前储户在这个月已经兑换该商品的数量

                    DateTime dtBegin = DateTime.Now.AddDays(1 - DateTime.Now.Day);//本月初日期
                    DateTime dtEnd = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1);//下月初日期
                    string sqlDepExchangeCount = " SELECT SUM(GoodCount) FROM dbo.GoodExchange WHERE Dep_AccountNumber='" + AccountNumber + "' AND GoodID=" + GoodID + " AND dt_Exchange BETWEEN '" + dtBegin.ToString("yyyy-MM-dd") + "' AND '" + dtEnd.ToString("yyyy-MM-dd") + "'";

                    object objExchangeCount = SQLHelper.ExecuteScalar(sqlDepExchangeCount);
                    if (objExchangeCount == null || objExchangeCount.ToString() == "")
                    {

                        DepExchangeCount = 0;
                    }
                    else
                    {
                        DepExchangeCount = Convert.ToDouble(objExchangeCount);
                    }
                    if (GoodCount + DepExchangeCount > GoodExchangeLimit)
                    {
                        ISLimit = true;
                    }
                    else
                    {
                        ISLimit = false;
                    }
                }
            }

            var res = new { ISLimit = ISLimit, GoodExchangeLimit = GoodExchangeLimit, DepExchangeCount = DepExchangeCount };
            context.Response.Write(JsonHelper.ToJson(res));
            
        }

        /// <summary>
        /// 获取兑换时候需要的产品数量
        /// </summary>
        /// <param name="context"></param>
        void GetExchangeVarietyCount(HttpContext context)
        {
            double numMoney = Convert.ToDouble(context.Request.Form["numMoney"]);
            double Exchange_trading = Convert.ToDouble(context.Request.Form["Exchange_trading"]);//已经准备兑换的数量
            string Dep_SID = context.Request.Form["Dep_SID"].ToString();
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.ISRegular,B.InterestType,A.StorageNumber,A.Price_ShiChang,A.StorageDate,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID.ToString());

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                bool ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);//是否是定期类型
                int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式

                double VarietyCount = 0;//需要折合的产品数量
                double VarietyLiXi = 0;//活期兑换方式获得的利息
                double Money_YouHui = 0;//定期兑换优惠的金额
                double YouHui_Count = 0;//定期兑换优惠处理的存粮数量
                if (ISRegular && InterestType == 3)
                { //按照定期取利息，并且按到期价取利息，确定是定期类型
                   // VarietyCount = common.GetExVarietyCount_DingQi(Dep_SID, numMoney, Exchange_trading);
                    Dictionary<string, double> dicEV = common.GetExchangeVC_DingQi(Dep_SID, numMoney, Exchange_trading);
                    VarietyCount = dicEV["VarietyCount"];
                    Money_YouHui = dicEV["Ex_YouHui"];
                    YouHui_Count = dicEV["Ex_YouHui_Count"];
                }
                else
                {
                    Dictionary<string, double> dicEV = common.GetExchangeVC(Dep_SID, numMoney);
                    VarietyCount = dicEV["VarietyCount"];
                    if (InterestType == 1)
                    {
                        VarietyLiXi = dicEV["VarietyLiXi"];//付息类型计利息
                    }
                    else {
                        Money_YouHui = dicEV["VarietyLiXi"];//分红和入股计优惠金额
                    }
                   
                }



                //bool ISLimit = false;//是否被限额
                //bool ISExchangeLimit = Convert.ToBoolean(common.GetWBAuthority()["ISExchangeLimit"]);
                //double GoodExchangeLimit = 0;
                //double DepExchangeCount = 0;
                //if (!ISExchangeLimit) //不对兑换数量做限制
                //{
                //    ISLimit = false;
                //}
                //else
                //{
                //    //当前商品的兑换额度
                //    string GoodID = context.Request.Form["GoodID"].ToString();
                //    GoodExchangeLimit = Convert.ToDouble(SQLHelper.ExecuteScalar(" SELECT numExchangeLimit  FROM dbo.Good WHERE ID=" + GoodID));
                //    //当前储户在这个月已经兑换该商品的数量
                //    string AccountNumber = context.Request.Form["AccountNumber"].ToString();
                //    DateTime dtBegin = DateTime.Now.AddDays(1 - DateTime.Now.Day);//本月初日期
                //    DateTime dtEnd = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1);//下月初日期
                //    string sqlDepExchangeCount = " SELECT SUM(GoodCount) FROM dbo.GoodExchange WHERE Dep_AccountNumber='" + AccountNumber + "' AND GoodID=" + GoodID + " AND dt_Exchange BETWEEN '" + dtBegin.ToString("yyyy-MM-dd") + "' AND '" + dtEnd.ToString("yyyy-MM-dd") + "'";

                //    object objExchangeCount = SQLHelper.ExecuteScalar(sqlDepExchangeCount);
                //    if (objExchangeCount == null || objExchangeCount.ToString() == "")
                //    {

                //        DepExchangeCount = 0;
                //    }
                //    else
                //    {
                //        DepExchangeCount = Convert.ToDouble(objExchangeCount);
                //    }
                //    double ExchangeCount = Convert.ToDouble(context.Request.QueryString["ExchangeCount"]);
                //    if (ExchangeCount + DepExchangeCount > GoodExchangeLimit)
                //    {
                //        ISLimit = true;
                //    }
                //    else
                //    {
                //        ISLimit = false;
                //    }
                //}
                //var res = new { state = "success", VarietyCount = VarietyCount, VarietyLiXi = VarietyLiXi, Money_YouHui = Money_YouHui, YouHui_Count = YouHui_Count, ISLimit = ISLimit, GoodExchangeLimit = GoodExchangeLimit, DepExchangeCount = DepExchangeCount };
                var res = new { state = "success", VarietyCount = VarietyCount, VarietyLiXi = VarietyLiXi, Money_YouHui = Money_YouHui, YouHui_Count = YouHui_Count };

                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = "error" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }


        /// <summary>
        /// 更新兑换商品的数据
        /// </summary>
        /// <param name="context"></param>
        void UpdateExChangeList(HttpContext context)
        {
            string GoodID = context.Request.QueryString["GoodID"].ToString();

            string VarietyUnitName = context.Request.Form["txtVarietyUnitName"].ToString();
            string txtVarietyName = context.Request.Form["txtVarietyName"].ToString();//用于兑换的产品名称
            double GoodCount = Math.Round(Convert.ToDouble(context.Request.Form["txtGoodCount"]), 2);
            double VarietyCount = Math.Round(Convert.ToDouble(context.Request.Form["txtVarietyCount"]), 2);
            double JinE = Math.Round(Convert.ToDouble(context.Request.Form["txtGoodJinE"]), 2);
            double JieCun_Now = Math.Round(Convert.ToDouble(context.Request.Form["txtJieCun_Now"]), 2);

            double Total_VarietyCount = 0;//折合产品总数
            double Total_LiXi = 0;//利息总和
            double Total_JinE = 0;//金额总和
            DataTable dtExChange;
            if (context.Cache["ExChange"] != null)
            {
                dtExChange = (DataTable)context.Cache["ExChange"];
            }
            else
            {
                dtExChange = new DataTable();
                dtExChange.Columns.Add("numIndex", typeof(int));
                dtExChange.Columns.Add("BusinessName", typeof(string));
                dtExChange.Columns.Add("GoodID", typeof(string));
                dtExChange.Columns.Add("GoodName", typeof(string));
                dtExChange.Columns.Add("SpecName", typeof(string));
                dtExChange.Columns.Add("UnitName", typeof(string));
                dtExChange.Columns.Add("GoodPrice", typeof(string));
                dtExChange.Columns.Add("GoodCount", typeof(string));
                dtExChange.Columns.Add("VarietyLiXi", typeof(string));
                dtExChange.Columns.Add("VarietyCount", typeof(string));
                dtExChange.Columns.Add("JinE", typeof(string));
                dtExChange.Columns.Add("JieCun", typeof(string));
                dtExChange.Columns.Add("Money_YouHui", typeof(string));
                dtExChange.Columns.Add("YouHui_Count", typeof(string));

                context.Cache.Insert("ExChange", dtExChange, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            int newindex = 0;//新的目录索引
            if (dtExChange.Rows.Count > 0)
            {
                for (int i = 0; i < dtExChange.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtExChange.Rows[i]["numIndex"]) > newindex)
                    {
                        newindex = Convert.ToInt32(dtExChange.Rows[i]["numIndex"]);
                    }
                }
            }
            newindex += 1;
            StringBuilder strSqlGoodInfo = new StringBuilder();
            strSqlGoodInfo.Append(" SELECT   A.strName, B.strName AS UnitName,C.strName AS SpecName,A.Price_DuiHuan ");
            strSqlGoodInfo.Append("  FROM dbo.Good A INNER JOIN dbo.BD_MeasuringUnit B ON A.MeasuringUnit=B.ID");
            strSqlGoodInfo.Append("  INNER JOIN dbo.BD_PackingSpec C ON A.PackingSpecID =C.ID");
            strSqlGoodInfo.Append("  WHERE A.ID=" + GoodID);
            DataTable dtGoodInfo = SQLHelper.ExecuteDataTable(strSqlGoodInfo.ToString());
            if (dtGoodInfo == null || dtGoodInfo.Rows.Count == 0)
            {
                context.Response.Write("Error");
                return;
            }
            string GoodName = dtGoodInfo.Rows[0]["strName"].ToString();
            string UnitName = dtGoodInfo.Rows[0]["UnitName"].ToString();
            string SpecName = dtGoodInfo.Rows[0]["SpecName"].ToString();
            string Price_DuiHuan = dtGoodInfo.Rows[0]["Price_DuiHuan"].ToString();
            double Ex_LiXi = 0;
            if (context.Session["Ex_LiXi"] != null)
            {
                Ex_LiXi = Math.Round(Convert.ToDouble(context.Session["Ex_LiXi"]), 2);
            }
            double Money_YouHui = Math.Round(Convert.ToDouble(context.Session["Ex_YouHui"]), 2);//优惠金额
            double YouHui_Count = Math.Round(Convert.ToDouble(context.Session["Ex_YouHui_Count"]), 2);//优惠数量
            context.Session["Ex_YouHui"] = null;
            context.Session["Ex_YouHui_Count"] = null;

            dtExChange.Rows.Add(newindex, "兑换", GoodID, GoodName, SpecName, UnitName, Price_DuiHuan, GoodCount, Ex_LiXi, VarietyCount, JinE, JieCun_Now, Money_YouHui, YouHui_Count);

            StringBuilder exchangemsg = new StringBuilder();//要返回的信息

            exchangemsg.Append("  <table class='tabData' style='margin:10px 0px;'>");

            exchangemsg.Append("  <tr><td colspan='10\' style='font-weight:bolder; height:25px; color:Green; font-size:16px;'>兑换产品信息</td></tr>");
            exchangemsg.Append("  <tr class='tr_head'>");
            exchangemsg.Append("  <th style='width: 80px; height:30px; text-align: center;'> 业务名称 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'> 品名 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'> 规格 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'> 单价 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'>  数量 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'> 计量单位 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'>  折合原粮 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'> 利息 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'>  收费 </th>");
            exchangemsg.Append("   <th style='width: 80px; text-align: center;'>  删除 </th>");
            exchangemsg.Append(" </tr>");
            for (int i = 0; i < dtExChange.Rows.Count; i++)
            {
                DataRow row = dtExChange.Rows[i];
                Total_JinE += Convert.ToDouble(row["JinE"]);
                Total_LiXi += Convert.ToDouble(row["VarietyLiXi"]);
                Total_VarietyCount += Convert.ToDouble(row["VarietyCount"]);
                exchangemsg.Append("<tr>");
                exchangemsg.Append("  <td style='height:30px;'><span style='font-weight:bolder; color:Blue; padding:5px 0px;'>兑换</span></td>");

                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["GoodName"] + "</span></td>");
                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["SpecName"] + "</span></td>");
                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["GoodPrice"] + "</span></td>");

                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["GoodCount"] + "</span></td>");
                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["UnitName"] + "</span></td>");
                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["VarietyCount"] + "</span></td>");
                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["VarietyLiXi"] + "</span></td>");
                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["JinE"] + "</span></td>");
                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'><input type='button' value='删除' style='width:60px;' onclick='FunDelList(" + row["numIndex"] + ");'></span></td>");

                exchangemsg.Append("    </tr>");

            }
            exchangemsg.Append("  <tr>");
            exchangemsg.Append("   <td colspan='9' style='text-align:left'>");
            exchangemsg.Append("   消费金额:<span  style='color:Red'>" + Math.Round(Total_JinE, 2) + "</span>元,");
            exchangemsg.Append("   折合<span style='color:Blue'>" + txtVarietyName + "</span><span style='color:Red' >" + Math.Round(Total_VarietyCount, 2) + "</span><span>" + VarietyUnitName + ",</span>");
            exchangemsg.Append("   利息<span style='color:Red' >" + Math.Round(Total_LiXi, 2) + "元,</span>");
            exchangemsg.Append("  结存：<span style='color:Red'>" + JieCun_Now + "</span><span>" + VarietyUnitName + "</span>&nbsp;");
            exchangemsg.Append(" ");
            exchangemsg.Append(" ");

            exchangemsg.Append("  </td>");
            exchangemsg.Append("  </tr>");
            exchangemsg.Append("  </table>");
            exchangemsg.Append(" ");

            var returnValue = "{\"Total_VarietyCount\":\"" + Math.Round(Total_VarietyCount, 2) + "\",\"exchangemsg\":\"" + exchangemsg + "\"}";


            context.Response.Write(returnValue);

        }

        /// <summary>
        /// 更新兑换商品的数据
        /// </summary>
        /// <param name="context"></param>
        void DeleteExChangeList(HttpContext context)
        {
            string numIndex = context.Request.QueryString["numIndex"].ToString();

            string txtVarietyName = context.Request.Form["txtVarietyName"].ToString();//用于兑换的产品名称
            double GoodCount = Math.Round(Convert.ToDouble(context.Request.Form["txtGoodCount"]), 2);
            double VarietyCount = Math.Round(Convert.ToDouble(context.Request.Form["txtVarietyCount"]), 2);
            double JinE = Math.Round(Convert.ToDouble(context.Request.Form["txtGoodJinE"]), 2);
            double JieCun_Now = Math.Round(Convert.ToDouble(context.Request.Form["txtJieCun_Now"]), 2);

            double Total_VarietyCount = 0;//折合产品总数
            double Total_LiXi = 0;//利息总和
            double Total_JinE = 0;//金额总和
            DataTable dtExChange = null;
            if (context.Cache["ExChange"] != null)
            {
                dtExChange = (DataTable)context.Cache["ExChange"];
            }
            else
            {

            }
            int rowindex = 0;//要删除的行索引
            for (int i = 0; i < dtExChange.Rows.Count; i++)
            {
                if (dtExChange.Rows[i]["numIndex"].ToString() == numIndex.ToString())
                {
                    break;//找到目标行，跳出循环
                }
                else
                {
                    rowindex += 1;
                }
            }

            string UnitName = dtExChange.Rows[0]["UnitName"].ToString();
            double selectVarietyCount = Convert.ToDouble(dtExChange.Rows[rowindex]["VarietyCount"]);//被选择行的折合数量
            JieCun_Now = JieCun_Now + selectVarietyCount;//重心定义结存数量
            dtExChange.Rows.RemoveAt(rowindex);//移除指定的行

            StringBuilder strReturn = new StringBuilder();//要返回的信息

            strReturn.Append("  <table class='tabData' style='margin:10px 0px;'>");

            strReturn.Append("  <tr><td colspan='10\' style='font-weight:bolder; height:25px; color:Green; font-size:16px;'>兑换产品信息</td></tr>");
            strReturn.Append("  <tr class='tr_head'>");
            strReturn.Append("  <th style='width: 80px; height:30px; text-align: center;'> 业务名称 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 品名 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 规格 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 单价 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  数量 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 计量单位 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  折合原粮 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'> 利息 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  收费 </th>");
            strReturn.Append("   <th style='width: 80px; text-align: center;'>  删除 </th>");
            strReturn.Append(" </tr>");
            for (int i = 0; i < dtExChange.Rows.Count; i++)
            {
                DataRow row = dtExChange.Rows[i];
                Total_JinE += Convert.ToDouble(row["JinE"]);
                Total_LiXi += Convert.ToDouble(row["VarietyLiXi"]);
                Total_VarietyCount += Convert.ToDouble(row["VarietyCount"]);
                strReturn.Append("<tr>");
                strReturn.Append("  <td style='height:30px;'><span style='font-weight:bolder; color:Blue; padding:5px 0px;'>兑换</span></td>");

                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["GoodName"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["SpecName"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["GoodPrice"] + "</span></td>");

                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["GoodCount"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["UnitName"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["VarietyCount"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["VarietyLiXi"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'>" + row["JinE"] + "</span></td>");
                strReturn.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'><input type='button' value='删除' style='width:60px;' onclick='FunDelList(" + row["numIndex"] + ");'></span></td>");

                strReturn.Append("    </tr>");

            }
            strReturn.Append("  <tr>");
            strReturn.Append("   <td colspan='9' style='text-align:left'>");
            strReturn.Append("   消费金额:<span  style='color:Red'>" + Math.Round(Total_JinE, 2) + "</span>元,");
            strReturn.Append("   折合<span style='color:Blue'>" + txtVarietyName + "</span><span style='color:Red' >" + Math.Round(Total_VarietyCount, 2) + "</span><span>" + UnitName + ",</span>");
            strReturn.Append("   利息<span style='color:Red' >" + Math.Round(Total_LiXi, 2) + "元,</span>");
            strReturn.Append("  结存：<span style='color:Red'>" + JieCun_Now + "</span><span>" + UnitName + "</span>&nbsp;");
            strReturn.Append(" ");
            strReturn.Append(" ");

            strReturn.Append("  </td>");
            strReturn.Append("  </tr>");
            strReturn.Append("  </table>");
            strReturn.Append(" ");


            string strReturnMsg = "{\"JieCun_Now\":\"" + JieCun_Now + "\",\"Msg\":\"" + strReturn.ToString() + "\"}";

            context.Response.Write(strReturnMsg);

        }


        /// <summary>
        /// 添加商品兑换
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodExchange(HttpContext context)
        {
            string dsiID = context.Request.Form["dsiID"].ToString();
            string exlist = context.Request.Form["exlist"].ToString();//兑换列表
            DataTable dtexlist = JsonHelper.JsonToDataTable(exlist);
            if (dtexlist == null || dtexlist.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "请选择您要兑换的商品!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            //获取储户信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("  SELECT top 1  A.ID,A.AccountNumber,B.strName AS Dep_Name, VarietyID,TypeID,TimeID,StorageNumber, StorageNumberRaw,Price_ShiChang,Price_DaoQi");
            strSqlStorage.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlStorage.Append("  WHERE A.ID=" + dsiID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            if (dtStorage == null || dtStorage.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "获取储户的存储信息错误!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            string AccountNumber = dtStorage.Rows[0]["AccountNumber"].ToString();//储户账号
            string Dep_Name = dtStorage.Rows[0]["Dep_Name"].ToString();//储户名
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号
            double StorageNumber = Convert.ToDouble(dtStorage.Rows[0]["StorageNumber"]);//该产品上一次的剩余结存

            //add 20171122 结存不足时，禁止兑换
            if (StorageNumber <= 0)
            {
                var res = new { state = "error", msg = "该笔存储的结存已不足，无法继续操作!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            //end add
            double StorageNumberRaw = Convert.ToDouble(dtStorage.Rows[0]["StorageNumberRaw"]);//该产品原始结存
            string VarietyID = dtStorage.Rows[0]["VarietyID"].ToString();

            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "兑换";//GoodExchange存储的兑换业务名称
            string BusinessName_Log = "2";//Dep_OperateLog存储的兑换业务名称

            double JieCun_Last = StorageNumber;//上一次结存
            double JieCun_Now = 0;//现在剩余结存
            double JieCun_Raw = StorageNumberRaw;
            double JieCun_Total = StorageNumberRaw - StorageNumber;//该存储信息已经发生的结存

             double limitExChangeProp=1;
             double VarietyCount_t = 0;//在兑换中发生的折合产品数总计
             double VarietyCount_exchangemonth = common.GetMonthJieCun_Total(dsiID);//当月已经发生结存
            DataRow rowStorageTime=commondb.getStorageTimeByID(dtStorage.Rows[0]["TimeID"].ToString());
            if (rowStorageTime != null)
            {
                limitExChangeProp = Convert.ToDouble(rowStorageTime["limitExChangeProp"]) / 100;
            }


            #region 循环处理所有的兑换记录
            string BusinessNOList = "";//所有的兑换列表的集合
            List<string> BNOList = new List<string>();//兑换列表集合

            StringBuilder sqlEx = new StringBuilder();//所有的兑换sql
            StringBuilder sqlO_Log = new StringBuilder();//所有的日志sql
            StringBuilder sqlGoodStorage = new StringBuilder();//每次仓库商品数量发生的变化

            double Count_Balance = common.GetDep_StorageNumber(AccountNumber, VarietyID);//发生兑换之前储户总结存
            for (int i = 0; i < dtexlist.Rows.Count; i++)
            {
                if (i != 0)
                {
                    BusinessNO = Fun.ConvertIntToString(Convert.ToInt32(BusinessNO) + 1, 4);//新的对话编号
                }
                BNOList.Add(BusinessNO);
                string strGUID = Fun.getGUID();//防伪码
                string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + AccountNumber + BusinessNO;

                DataRow row = dtexlist.Rows[i];
                //string VarietyID = row["VarietyID"].ToString();//产品ID
                string WBWareHouseID = row["WBWareHouseID"].ToString();//商品仓库ID
                string GoodID = row["GoodID"].ToString();//兑换商品ID
                string GoodName = row["GoodName"].ToString();//商品名称
                string SpecName = row["SpecName"].ToString();//商品规格
                string UnitName = row["UnitName"].ToString();//商品计量单位

                double GoodCount = Convert.ToDouble(row["GoodCount"]);//兑换商品数量
                string GoodPrice = row["GoodPrice"].ToString();//兑换商品单价
                double VarietyCount = Convert.ToDouble(row["VarietyCount"]);//折合产品数量
                double VarietyInterest = Convert.ToDouble(row["VarietyLiXi"]);//折合产品利息
                double Money_DuiHuan = Convert.ToDouble(row["JinE"]);
                double Money_YouHui = Convert.ToDouble(row["Money_YouHui"]);

                JieCun_Now = JieCun_Last - VarietyCount;
                JieCun_Total = JieCun_Total + VarietyCount;

                VarietyCount_t += VarietyCount;
               

                //add 20170802
                if (JieCun_Now < 0)//结存不足时，禁止兑换
                {
                    var res = new { state = "error", msg = "储户存粮结存不足，无法完成兑换!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }
                //end add
                #region 兑换记录

                sqlEx.Append("  insert into [GoodExchange] (");
                sqlEx.Append("SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn,WBWareHouseID)");
                sqlEx.Append(" values (");

                sqlEx.Append(string.Format("'{0}','{1}','{2}',{3},'{4}','{5}',{6},{7},'{8}',{9},'{10}','{11}','{12}',{13},{14},{15},{16},{17},{18},'{19}',{20},{21},{22},{23},{24},{25})", SerialNumber, strGUID, BusinessNO, dsiID, AccountNumber, Dep_Name, WBID, UserID, BusinessName, GoodID, GoodName, SpecName, UnitName, GoodCount, GoodPrice, VarietyCount, VarietyInterest, Money_DuiHuan, Money_YouHui, DateTime.Now, JieCun_Last, JieCun_Now, JieCun_Raw, JieCun_Total, 0,WBWareHouseID));

                #endregion
                JieCun_Last = JieCun_Now;//更新上一次结存

                #region 日志记录
                double Money_Trade = Convert.ToDouble(GoodPrice) * Convert.ToDouble(GoodCount);
               
                Count_Balance = Count_Balance - VarietyCount;
                sqlO_Log.Append("  insert into [Dep_OperateLog] (");
                sqlO_Log.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
                sqlO_Log.Append(" values (");

                sqlO_Log.Append(string.Format("{0},{1},'{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},{11},'{12}','{13}','{14}',{15})", WBID, UserID, AccountNumber, BusinessNO, BusinessName_Log, VarietyID, UnitName, GoodPrice, GoodCount, VarietyCount, Money_Trade, Count_Balance, DateTime.Now.ToString(), GoodName, UnitName,dsiID));

                #endregion

                //商品库存数变化
                sqlGoodStorage.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore-{0} WHERE GoodID={1} AND WBID={2} and WBWareHouseID={3}", GoodCount,GoodID, WBID,WBWareHouseID));
            }
            #endregion

            //add 20180111
            if (limitExChangeProp < 1)//该存期限制每月兑换额度
            {

                if (VarietyCount_t + VarietyCount_exchangemonth > limitExChangeProp * StorageNumberRaw) //兑换折合存粮超出每月兑换额度
                {
                    var res = new { state = "error", msg = "该产品每月最多兑换额度:" +Math.Round( (limitExChangeProp * 100),2) + "%，共" + Math.Round( limitExChangeProp * StorageNumberRaw ,2)+ "公斤,当月兑换已折合存粮:" +Math.Round(  VarietyCount_exchangemonth,2) + "公斤，本次折合存粮:"+Math.Round( VarietyCount_t,2)+"公斤，无法继续完成兑换!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }

            }
            //end add

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    //兑换记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlEx.ToString());
                    //日志记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlO_Log.ToString());
                    //商品库存数变化
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlGoodStorage.ToString());
                    //储户的产品结存
                    string SqlDep_JieCun = string.Format(" UPDATE dbo.Dep_StorageInfo  SET StorageNumber={0} WHERE ID={1}", JieCun_Now, dsiID);
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, SqlDep_JieCun.ToString());

                    tran.Commit();

                    var res = new { state = "success", msg = "兑换成功!", BNOList = Fun.ListToString(BNOList, '|') };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "success", msg = "执行sql错误!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion
        }


        /// <summary>
        /// 添加批量商品兑换
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodExchangeGroup(HttpContext context)
        {
            string dsiID = context.Request.Form["dsiID"].ToString();
            string exlist = context.Request.Form["exlist"].ToString();//兑换列表
            DataRow rowWBAuthority = common.GetWBAuthority();
            double exchangeGroupProp = Convert.ToDouble(rowWBAuthority["exchangeGroupProp"]);
            int exchangeGroupPeriod = Convert.ToInt32(rowWBAuthority["exchangeGroupPeriod"]);

            string orderdate = "";
             string orderdateDone = "";
            int orderstate=1;
            DateTime dt_order = DateTime.Now;
            orderdateDone = Fun.getDate_YM(dt_order);
            for (int i = 0; i < exchangeGroupPeriod; i++) {
                orderdate += Fun.getDate_YM(dt_order)+"|";
                dt_order = dt_order.AddMonths(1);
            }
            orderdate = orderdate.Substring(0, orderdate.Length - 1);


            DataTable dtexlist = JsonHelper.JsonToDataTable(exlist);
            if (dtexlist == null || dtexlist.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "请选择您要兑换的商品!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            //获取储户信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("  SELECT top 1  A.ID,A.AccountNumber,B.strName AS Dep_Name, VarietyID,TypeID,TimeID,StorageNumber, StorageNumberRaw,Price_ShiChang,Price_DaoQi");
            strSqlStorage.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlStorage.Append("  WHERE A.ID=" + dsiID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            if (dtStorage == null || dtStorage.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "获取储户的存储信息错误!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            string AccountNumber = dtStorage.Rows[0]["AccountNumber"].ToString();//储户账号
            string Dep_Name = dtStorage.Rows[0]["Dep_Name"].ToString();//储户名
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号
            double StorageNumber = Convert.ToDouble(dtStorage.Rows[0]["StorageNumber"]);//该产品上一次的剩余结存

            //add 20171122 结存不足时，禁止兑换
            if (StorageNumber <= 0)
            {
                var res = new { state = "error", msg = "该笔存储的结存已不足，无法继续操作!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            //end add
            double StorageNumberRaw = Convert.ToDouble(dtStorage.Rows[0]["StorageNumberRaw"]);//该产品原始结存
            string VarietyID = dtStorage.Rows[0]["VarietyID"].ToString();

            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "兑换";//GoodExchange存储的兑换业务名称
            string BusinessName_Log = "17";//Dep_OperateLog存储的兑换业务名称

            double JieCun_Last = StorageNumber;//上一次结存
            double JieCun_Now = 0;//现在剩余结存
            double JieCun_Raw = StorageNumberRaw;
            double JieCun_Total = StorageNumberRaw - StorageNumber;//该存储信息已经发生的结存

            double limitExChangeProp = 1;
            double VarietyCount_t = 0;//在兑换中发生的折合产品数总计
            double VarietyCount_exchangemonth = common.GetMonthJieCun_Total(dsiID);//当月已经发生结存
            DataRow rowStorageTime = commondb.getStorageTimeByID(dtStorage.Rows[0]["TimeID"].ToString());
            if (rowStorageTime != null)
            {
                limitExChangeProp = Convert.ToDouble(rowStorageTime["limitExChangeProp"]) / 100;
            }


            #region 循环处理所有的兑换记录
            string BusinessNOList = "";//所有的兑换列表的集合
            List<string> BNOList = new List<string>();//兑换列表集合

            StringBuilder sqlEx = new StringBuilder();//所有的兑换sql
            StringBuilder sqlExDetail = new StringBuilder();//所有的详细兑付记录sql
            StringBuilder sqlO_Log = new StringBuilder();//所有的日志sql
            StringBuilder sqlGoodStorage = new StringBuilder();//每次仓库商品数量发生的变化

            double Count_Balance = common.GetDep_StorageNumber(AccountNumber, VarietyID);//发生兑换之前储户总结存
            for (int i = 0; i < dtexlist.Rows.Count; i++)
            {
                if (i != 0)
                {
                    BusinessNO = Fun.ConvertIntToString(Convert.ToInt32(BusinessNO) + 1, 4);//新的对话编号
                }
                BNOList.Add(BusinessNO);
                string strGUID = Fun.getGUID();//防伪码
                string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + AccountNumber + BusinessNO;

                DataRow row = dtexlist.Rows[i];
                //string VarietyID = row["VarietyID"].ToString();//产品ID
                string WBWareHouseID = row["WBWareHouseID"].ToString();//商品仓库ID
                string GoodID = row["GoodID"].ToString();//兑换商品ID
                string GoodName = row["GoodName"].ToString();//商品名称
                string SpecName = row["SpecName"].ToString();//商品规格
                string UnitName = row["UnitName"].ToString();//商品计量单位

                double GoodCount = Convert.ToDouble(row["GoodCount"]);//兑换商品数量
                string GoodPrice = row["GoodPrice"].ToString();//兑换商品单价
                double GoodCount_ex = Convert.ToDouble(row["GoodCount_ex"]);//兑换商品数量
                string GoodPrice_ex = row["GoodPrice_ex"].ToString();//兑换商品单价

                double VarietyCount = Convert.ToDouble(row["VarietyCount"]);//折合产品数量
                double VarietyInterest = Convert.ToDouble(row["VarietyLiXi"]);//折合产品利息
                double Money_DuiHuan = Convert.ToDouble(row["JinE"]);
                double Money_YouHui = Convert.ToDouble(row["Money_YouHui"]);

                JieCun_Now = JieCun_Last - VarietyCount;
                JieCun_Total = JieCun_Total + VarietyCount;

                VarietyCount_t += VarietyCount;


                //add 20170802
                if (JieCun_Now < 0)//结存不足时，禁止兑换
                {
                    var res = new { state = "error", msg = "储户存粮结存不足，无法完成兑换!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }
                //end add
                #region 兑换记录

                sqlEx.Append("  insert into [GoodExchangeGroup] (");
                sqlEx.Append("SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn,WBWareHouseID,exchangeGroupProp,exchangeGroupPeriod,orderdate,orderdateDone,orderstate)");
                sqlEx.Append(" values (");

                sqlEx.Append(string.Format("'{0}','{1}','{2}',{3},'{4}','{5}',{6},{7},'{8}',{9},'{10}','{11}','{12}',{13},{14},{15},{16},{17},{18},'{19}',{20},{21},{22},{23},{24},{25},{26},{27},'{28}','{29}','{30}')", SerialNumber, strGUID, BusinessNO, dsiID, AccountNumber, Dep_Name, WBID, UserID, BusinessName, GoodID, GoodName, SpecName, UnitName, GoodCount, GoodPrice_ex, VarietyCount, VarietyInterest, Money_DuiHuan, Money_YouHui, DateTime.Now, JieCun_Last, JieCun_Now, JieCun_Raw, JieCun_Total, 0, WBWareHouseID, exchangeGroupProp,exchangeGroupPeriod, orderdate, orderdateDone, orderstate));

                #endregion

                #region 兑付条目记录

                sqlExDetail.Append("  insert into [GoodExchangeGroupDetail] (");
                sqlExDetail.Append("EGID,GoodID,GoodCount,GoodPrice,exchangeGroupProp,dt_Trade,userID,orderdate,orderstate)");
                sqlExDetail.Append(" values (");

                sqlExDetail.Append(string.Format("'{0}',{1},{2},{3},{4},'{5}',{6},'{7}',{8})", strGUID, GoodID, GoodCount_ex, GoodPrice_ex, exchangeGroupProp, DateTime.Now.ToString(), UserID,orderdateDone, 1));

                #endregion


                JieCun_Last = JieCun_Now;//更新上一次结存

                #region 日志记录
                double Money_Trade = Convert.ToDouble(GoodPrice) * Convert.ToDouble(GoodCount);

                Count_Balance = Count_Balance - VarietyCount;
                sqlO_Log.Append("  insert into [Dep_OperateLog] (");
                sqlO_Log.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
                sqlO_Log.Append(" values (");

                sqlO_Log.Append(string.Format("{0},{1},'{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},{11},'{12}','{13}','{14}',{15})", WBID, UserID, AccountNumber, BusinessNO, BusinessName_Log, VarietyID, UnitName, GoodPrice, GoodCount, VarietyCount, Money_Trade, Count_Balance, DateTime.Now.ToString(), GoodName, UnitName, dsiID));

                #endregion

                //商品库存数变化
                sqlGoodStorage.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore-{0} WHERE GoodID={1} AND WBID={2} and WBWareHouseID={3}", GoodCount, GoodID, WBID, WBWareHouseID));
            }
            #endregion

            //add 20180111
            if (limitExChangeProp < 1)//该存期限制每月兑换额度
            {

                if (VarietyCount_t + VarietyCount_exchangemonth > limitExChangeProp * StorageNumberRaw) //兑换折合存粮超出每月兑换额度
                {
                    var res = new { state = "error", msg = "该产品每月最多兑换额度:" + Math.Round((limitExChangeProp * 100), 2) + "%，共" + Math.Round(limitExChangeProp * StorageNumberRaw, 2) + "公斤,当月兑换已折合存粮:" + Math.Round(VarietyCount_exchangemonth, 2) + "公斤，本次折合存粮:" + Math.Round(VarietyCount_t, 2) + "公斤，无法继续完成兑换!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }

            }
            //end add

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    //兑换记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlEx.ToString());
                    //兑付条目
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlExDetail.ToString());
                    //日志记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlO_Log.ToString());
                    //商品库存数变化
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlGoodStorage.ToString());
                    //储户的产品结存
                    string SqlDep_JieCun = string.Format(" UPDATE dbo.Dep_StorageInfo  SET StorageNumber={0} WHERE ID={1}", JieCun_Now, dsiID);
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, SqlDep_JieCun.ToString());

                    tran.Commit();

                    var res = new { state = "success", msg = "批量兑换成功!", BNOList = Fun.ListToString(BNOList, '|') };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "success", msg = "执行sql错误!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion
        }


        /// <summary>
        /// 添加批量商品兑换
        /// </summary>
        /// <param name="context"></param>
        void Query_GoodExchangeGroup(HttpContext context)
        {

            string strDate = Fun.getDate_YM(DateTime.Now);
            string orderstate = context.Request.Form["orderstate"].ToString();//1:进行中，2：已结束
            string AccountNumber = "";
            if (context.Request.Form["AccountNumber"] != null)
            {
                AccountNumber = context.Request.Form["AccountNumber"].ToString();
            }
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT A.ID,A.strGUID, B.AccountNumber,B.strName AS DepName,A.GoodName,A.UnitName,A.GoodCount,A.GoodPrice,A.GoodPrice*exchangeGroupProp/100 AS GoodPriceGroup,A.exchangeGroupPeriod,");
             strSql.Append("    D.strName AS VarietyName,  A.VarietyCount, A.VarietyInterest,A.Money_YouHui,A.Money_DuiHuan,");
             strSql.Append(string.Format("     CONVERT(VARCHAR(100),dt_Exchange,23) AS dt_Exchange, CHARINDEX('{0}',A.orderdateDone) AS isExchange", strDate));
             strSql.Append("    FROM dbo.GoodExchangeGroup A INNER JOIN dbo.Depositor B ON A.Dep_AccountNumber=B.AccountNumber");
             strSql.Append("     INNER JOIN dbo.Dep_StorageInfo C ON A.Dep_SID=C.ID");
             strSql.Append("     LEFT OUTER JOIN dbo.StorageVariety D ON C.VarietyID=D.ID");
             strSql.Append("   WHERE 1=1");

            if(orderstate!=""){
             strSql.Append(string.Format( "   and A.orderstate={0}",orderstate));
            }
            if(AccountNumber!=""){
             strSql.Append(string.Format( "   and B.AccountNumber='{0}'",AccountNumber));
            }

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                var res = new { state = "false", msg = "无分时批量兑换信息!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = "true", data = JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
                  
        }


        /// <summary>
        /// 添加批量商品兑换
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodExchangeGroupDetail(HttpContext context)
        {

            //string strDate = Fun.getDate_YM(DateTime.Now);
            string orderdateadd = context.Request.Form["orderdateadd"].ToString();
            string ID = context.Request.Form["ID"].ToString();//1:进行中，2：已结束
            DataRow rowGoodExchangeGroupByID = commondb.getGoodExchangeGroupByID(ID);

            string WBID = rowGoodExchangeGroupByID["WBID"].ToString();
            string WBWareHouseID = rowGoodExchangeGroupByID["WBWareHouseID"].ToString();
            string strGUID = rowGoodExchangeGroupByID["strGUID"].ToString();
            string GoodID = rowGoodExchangeGroupByID["GoodID"].ToString();
            double GoodCount = Convert.ToDouble(rowGoodExchangeGroupByID["GoodCount"]) /Convert.ToDouble(rowGoodExchangeGroupByID["exchangeGroupPeriod"]);//每次兑付的数量
            string GoodPrice = rowGoodExchangeGroupByID["GoodPrice"].ToString();//原兑换价格
            string exchangeGroupProp = rowGoodExchangeGroupByID["exchangeGroupProp"].ToString();
            string dt_Trade = DateTime.Now.ToString();
            string UserID = context.Session["ID"].ToString();
           

            string orderdate = rowGoodExchangeGroupByID["orderdate"].ToString();
            string orderdateDone = rowGoodExchangeGroupByID["orderdateDone"].ToString();
            if (orderdateDone != "")
            {
                orderdateDone += "|" + orderdateadd;
            }
            else {
                orderdateDone = orderdateadd;
            }
            int orderstate = 1;
            if (Fun.compareArr(orderdate.Split('|'), orderdateDone.Split('|'))) {
                orderstate = 2;//全部兑付完成
            }

            StringBuilder sqlExDetail = new StringBuilder();
            sqlExDetail.Append("  insert into [GoodExchangeGroupDetail] (");
            sqlExDetail.Append("EGID,GoodID,GoodCount,GoodPrice,exchangeGroupProp,dt_Trade,userID,orderdate,orderstate)");
            sqlExDetail.Append(" values (");

            sqlExDetail.Append(string.Format("'{0}',{1},{2},{3},{4},'{5}',{6},'{7}',{8})", strGUID, GoodID, GoodCount, GoodPrice, exchangeGroupProp, DateTime.Now.ToString(), UserID, orderdateadd,1));

            StringBuilder sqlGroup = new StringBuilder();
            sqlGroup.Append("  UPDATE dbo.GoodExchangeGroup");
            sqlGroup.Append(string.Format(" SET orderdateDone='{0}',",orderdateDone));
            sqlGroup.Append(string.Format(" orderstate={0}", orderstate));
            sqlGroup.Append(string.Format(" WHERE ID={0}", ID));

            StringBuilder sqlGoodStorage = new StringBuilder();
            sqlGoodStorage.Append("  SELECT A.numStore,B.strName AS WareHouseName,C.strName AS GoodName");
            sqlGoodStorage.Append(" FROM dbo.GoodStorage A INNER JOIN dbo.WBWareHouse B ON A.WBWareHouseID=B.ID");
            sqlGoodStorage.Append(" INNER JOIN dbo.Good C ON A.GoodID=C.ID");
            sqlGoodStorage.Append(string.Format("  WHERE A.WBID={0} AND WBWareHouseID={1} AND GoodID={2} ",WBID,WBWareHouseID,GoodID));
            DataTable dtGoodStorage = SQLHelper.ExecuteDataTable(sqlGoodStorage.ToString());
            if (dtGoodStorage == null || dtGoodStorage.Rows.Count == 0)
            {
                var res = new { state = "false", msg = "系统中没有当前商品的库存信息，无法完成兑付!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            else {
                double numStore = Convert.ToDouble(dtGoodStorage.Rows[0]["numStore"]);
                if (numStore < GoodCount) {
                    var res = new { state = "false", msg = "仓库[" + dtGoodStorage.Rows[0]["WareHouseName"].ToString() + "]中，[" + dtGoodStorage.Rows[0]["GoodName"].ToString() + "]库存量剩余" + GoodCount + "，本次需兑付"+numStore+",无法完成兑付!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }
            }

            StringBuilder updateGoodStorage = new StringBuilder();
            updateGoodStorage.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore-{0} WHERE GoodID={1} AND WBID={2} and WBWareHouseID={3}", GoodCount, GoodID, WBID, WBWareHouseID));

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    //兑付条目
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlExDetail.ToString());
                    //更新兑换记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlGroup.ToString());
                    //更新商品库存
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, updateGoodStorage.ToString());
                    tran.Commit();
                    var res = new { state = "true", msg = "兑付成功!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "false", msg = "兑付失败!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion

        }


        /// <summary>
        /// 兑付本月所有的商品
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodExchangeGroupDetail_All(HttpContext context)
        {

            string strDate = Fun.getDate_YM(DateTime.Now);
            string Qorderstate = context.Request.Form["orderstate"].ToString();//1:进行中，2：已结束
            string AccountNumber = "";
            if (context.Request.Form["AccountNumber"] != null)
            {
                AccountNumber = context.Request.Form["AccountNumber"].ToString();
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT *  FROM dbo.GoodExchangeGroup");
            strSql.Append("   WHERE 1=1");

            if (Qorderstate != "")
            {
                strSql.Append(string.Format("   and orderstate={0}", Qorderstate));
            }
            if (AccountNumber != "")
            {
                strSql.Append(string.Format("   and Dep_AccountNumber='{0}'", AccountNumber));
            }

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                var res = new { state = "false", msg = "无分时批量兑换信息!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }


            DataTable dtWBStorage = new DataTable();//当前网点的仓库库存
            dtWBStorage.Columns.Add("WBID");
            dtWBStorage.Columns.Add("WBWareHouseID");
            dtWBStorage.Columns.Add("GoodID");
            dtWBStorage.Columns.Add("numStore");

            StringBuilder sqlExDetail = new StringBuilder();
            StringBuilder sqlGroup = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++) {

                //string strDate = Fun.getDate_YM(DateTime.Now);
                string orderdateadd = strDate;
                DataRow rowGroup = dt.Rows[i];
                string orderdate = rowGroup["orderdate"].ToString();
                string orderdateDone = rowGroup["orderdateDone"].ToString();
                if (orderdateDone.Contains(orderdateadd)) {
                    continue;//当前兑换已经在本月兑付过，不需要重复兑付
                }
                string ID = rowGroup["ID"].ToString();
                string WBID = rowGroup["WBID"].ToString();
                string WBWareHouseID = rowGroup["WBWareHouseID"].ToString();
                string strGUID = rowGroup["strGUID"].ToString();
                string GoodID = rowGroup["GoodID"].ToString();
                double GoodCount = Convert.ToDouble(rowGroup["GoodCount"]) / Convert.ToDouble(rowGroup["exchangeGroupPeriod"]);//每次兑付的数量
                string GoodPrice = rowGroup["GoodPrice"].ToString();//原兑换价格
                string exchangeGroupProp = rowGroup["exchangeGroupProp"].ToString();
                string dt_Trade = DateTime.Now.ToString();
                string UserID = context.Session["ID"].ToString();


               
                if (orderdateDone != "")
                {
                    orderdateDone += "|" + orderdateadd;
                }
                else
                {
                    orderdateDone = orderdateadd;
                }
                int orderstate = 1;
                if (Fun.compareArr(orderdate.Split('|'), orderdateDone.Split('|')))
                {
                    orderstate = 2;//全部兑付完成
                }

                
                sqlExDetail.Append("  insert into [GoodExchangeGroupDetail] (");
                sqlExDetail.Append("EGID,GoodID,GoodCount,GoodPrice,exchangeGroupProp,dt_Trade,userID,orderdate,orderstate)");
                sqlExDetail.Append(" values (");

                sqlExDetail.Append(string.Format("'{0}',{1},{2},{3},{4},'{5}',{6},'{7}',{8})", strGUID, GoodID, GoodCount, GoodPrice, exchangeGroupProp, DateTime.Now.ToString(), UserID, orderdateadd, 1));

               
                sqlGroup.Append("  UPDATE dbo.GoodExchangeGroup");
                sqlGroup.Append(string.Format(" SET orderdateDone='{0}',", orderdateDone));
                sqlGroup.Append(string.Format(" orderstate={0}", orderstate));
                sqlGroup.Append(string.Format(" WHERE ID={0}", ID));

                //库存变化量
                AddWBStoreRow(ref dtWBStorage, WBID, GoodID, WBWareHouseID, GoodCount);

               
            }

            //库存 更新记录
            StringBuilder updateGoodStorage = new StringBuilder();
            for (int i = 0; i < dtWBStorage.Rows.Count; i++) {
                string WBID = dtWBStorage.Rows[i]["WBID"].ToString();
                string WBWareHouseID = dtWBStorage.Rows[i]["WBWareHouseID"].ToString();
                string GoodID = dtWBStorage.Rows[i]["GoodID"].ToString();
                double GoodCount = Convert.ToDouble( dtWBStorage.Rows[i]["numStore"]);

                StringBuilder sqlGoodStorage = new StringBuilder();
                sqlGoodStorage.Append("  SELECT A.numStore,B.strName AS WareHouseName,C.strName AS GoodName");
                sqlGoodStorage.Append(" FROM dbo.GoodStorage A INNER JOIN dbo.WBWareHouse B ON A.WBWareHouseID=B.ID");
                sqlGoodStorage.Append(" INNER JOIN dbo.Good C ON A.GoodID=C.ID");
                sqlGoodStorage.Append(string.Format("  WHERE A.WBID={0} AND WBWareHouseID={1} AND GoodID={2} ", WBID, WBWareHouseID, GoodID));
                DataTable dtGoodStorage = SQLHelper.ExecuteDataTable(sqlGoodStorage.ToString());
                if (dtGoodStorage == null || dtGoodStorage.Rows.Count == 0)
                {
                    var res = new { state = "false", msg = "系统中没有当前商品的库存信息，无法完成兑付!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }
                else
                {
                    double numStore = Convert.ToDouble(dtGoodStorage.Rows[0]["numStore"]);
                    if (numStore < GoodCount)
                    {
                        var res = new { state = "false", msg = "仓库[" + dtGoodStorage.Rows[0]["WareHouseName"].ToString() + "]中，[" + dtGoodStorage.Rows[0]["GoodName"].ToString() + "]库存量剩余" + numStore + "，本次需兑付" + GoodCount + ",无法完成兑付!" };
                        context.Response.Write(JsonHelper.ToJson(res));
                        return;
                    }
                }

                
                updateGoodStorage.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore-{0} WHERE GoodID={1} AND WBID={2} and WBWareHouseID={3}", GoodCount, GoodID, WBID, WBWareHouseID));
            }


                #region 数据处理
                using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
                {
                    try
                    {
                        //兑付条目
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlExDetail.ToString());
                        //更新兑换记录
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlGroup.ToString());

                        //更新商品库存
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, updateGoodStorage.ToString());

                        tran.Commit();
                        var res = new { state = "true", msg = "兑付成功!" };
                        context.Response.Write(JsonHelper.ToJson(res));
                    }
                    catch
                    {
                        tran.Rollback();
                        var res = new { state = "false", msg = "兑付失败!" };
                        context.Response.Write(JsonHelper.ToJson(res));
                    }
                }
            #endregion

        }

        void AddWBStoreRow(ref DataTable dt, string WBID, string GoodID, string WBWareHouseID, double numStore) {
            if (dt == null || dt.Rows.Count == 0) {
                dt.Rows.Add(WBID, WBWareHouseID, GoodID, numStore);
                return;
            }
            bool exitrow = false;
            for (int i = 0; i < dt.Rows.Count; i++) {
                if (dt.Rows[i]["WBID"].ToString() == WBID && dt.Rows[i]["GoodID"].ToString() == GoodID && dt.Rows[i]["WBWareHouseID"].ToString() == WBWareHouseID)
                {
                    exitrow = true;
                    dt.Rows[i]["numStore"] = Convert.ToDouble(dt.Rows[i]["numStore"]) + numStore;
                }
            }
            if (!exitrow) {
                dt.Rows.Add(WBID, WBWareHouseID, GoodID, numStore);
            }
            return;
        }

        /// <summary>
        /// 获取批量兑换详细信息
        /// </summary>
        /// <param name="context"></param>
        void Get_GoodExchangeGroupDetail(HttpContext context)
        {
            string EGID = context.Request.Form["EGID"].ToString();
            DataRow rowGoodExchangeGroupByID = SQLHelper.ExecuteDataTable(" select * from  GoodExchangeGroup where strGUID='"+EGID+"'").Rows[0];

            string orderdate = rowGoodExchangeGroupByID["orderdate"].ToString();
            string orderdateDone = rowGoodExchangeGroupByID["orderdateDone"].ToString();
            string orderstate = rowGoodExchangeGroupByID["orderstate"].ToString();
           

            StringBuilder sqlDetail = new StringBuilder();
            sqlDetail.Append(" SELECT B.strName AS GoodName,A.GoodCount,A.GoodPrice,A.GoodPrice*exchangeGroupProp/100 AS GoodPriceGroup,");
            sqlDetail.Append(" CONVERT(VARCHAR(100),dt_Trade,23) AS dt_Trade,C.strRealName AS UserName,A.orderdate,A.orderstate");
            sqlDetail.Append(" FROM dbo.GoodExchangeGroupDetail A INNER JOIN dbo.Good B ON A.GoodID=B.ID");
            sqlDetail.Append(" LEFT OUTER JOIN dbo.Users C ON A.userID=C.ID");
            sqlDetail.Append(" WHERE A.EGID='"+EGID+"'");
            DataTable dtDetail = SQLHelper.ExecuteDataTable(sqlDetail.ToString());

            if (dtDetail == null)//还没有任何兑付
            {
                dtDetail = new DataTable();
                dtDetail.Columns.Add("GoodName");
                dtDetail.Columns.Add("GoodCount");
                dtDetail.Columns.Add("GoodPrice");
                dtDetail.Columns.Add("GoodPriceGroup");
                dtDetail.Columns.Add("dt_Trade");
                dtDetail.Columns.Add("UserName");
                dtDetail.Columns.Add("orderdate");
                dtDetail.Columns.Add("orderstate");
            }

            if (orderstate != "2")//未完成的兑换订单
            {
                string[] orderdateArr = orderdate.Split('|');
                string[] orderdateDoneArr = orderdateDone.Split('|');
                for (int i = 0; i < orderdateArr.Length; i++)
                {
                    if (!orderdateDoneArr.Contains(orderdateArr[i]))
                    {
                        dtDetail.Rows.Add("", 0, 0, 0, "", "", orderdateArr[i], "0");
                    }
                }
            }
            if (dtDetail == null || dtDetail.Rows.Count == 0)
            {
                var res = new { state = "true", msg = "没有查询到数据!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else {
                var res = new { state = "true", data = JsonHelper.ToJson(dtDetail),exgroup=JsonHelper.ToJson(rowGoodExchangeGroupByID.Table) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
         

        }


        /// <summary>
        /// 添加商品销售
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodSell(HttpContext context)
        {
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string Dep_Name = context.Request.Form["strName"].ToString();
            string exlist = context.Request.Form["exlist"].ToString();//兑换列表
            DataTable dtexlist = JsonHelper.JsonToDataTable(exlist);
            if (dtexlist == null || dtexlist.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "请选择您要购买的商品!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号
          
            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "商品销售";//GoodExchange存储的兑换业务名称
            string BusinessName_Log = "13";//Dep_OperateLog存储的兑换业务名称


            #region 循环处理所有记录
            List<string> BNOList = new List<string>();//兑换列表集合

            StringBuilder sqlEx = new StringBuilder();//所有的兑换sql
            StringBuilder sqlO_Log = new StringBuilder();//所有的日志sql
            StringBuilder sqlGoodStorage = new StringBuilder();//每次仓库商品数量发生的变化

         
            for (int i = 0; i < dtexlist.Rows.Count; i++)
            {
                if (i != 0)
                {
                    BusinessNO = Fun.ConvertIntToString(Convert.ToInt32(BusinessNO) + 1, 4);//新的对话编号
                }
                BNOList.Add(BusinessNO);
                string strGUID = Fun.getGUID();//防伪码
                string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + AccountNumber + BusinessNO;

                DataRow row = dtexlist.Rows[i];
                //string VarietyID = row["VarietyID"].ToString();//产品ID
                string WBWareHouseID = row["WBWareHouseID"].ToString();//商品仓库ID
                string GoodID = row["GoodID"].ToString();//兑换商品ID
                string GoodName = row["GoodName"].ToString();//商品名称
                string SpecName = row["SpecName"].ToString();//商品规格
                string UnitName = row["UnitName"].ToString();//商品计量单位

                double GoodCount = Convert.ToDouble(row["GoodCount"]);//兑换商品数量
                string GoodPrice = row["GoodPrice"].ToString();//兑换商品单价
                double GoodValue = Convert.ToDouble(row["GoodValue"]);

                #region 兑换记录

                sqlEx.Append("  insert into [GoodSell] (");
                sqlEx.Append("SerialNumber,strGUID,BusinessNO,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,GoodValue,WBWareHouseID,ISReturn,dt_Sell)");
                sqlEx.Append(" values (");

                sqlEx.Append(string.Format("'{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}',{8},'{9}','{10}','{11}',{12},{13},{14},{15},{16},'{17}')", SerialNumber, strGUID, BusinessNO, AccountNumber, Dep_Name, WBID, UserID, BusinessName, GoodID, GoodName, SpecName, UnitName, GoodCount, GoodPrice, GoodValue, WBWareHouseID, 0, DateTime.Now.ToString()));

                #endregion

                #region 日志记录
              
                sqlO_Log.Append("  insert into [Dep_OperateLog] (");
                sqlO_Log.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
                sqlO_Log.Append(" values (");

                sqlO_Log.Append(string.Format("{0},{1},'{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},{11},'{12}','{13}','{14}',{15})", WBID, UserID, AccountNumber, BusinessNO, BusinessName_Log, GoodID, UnitName, GoodPrice, GoodCount, 0, GoodValue, 0, DateTime.Now.ToString(), GoodName, UnitName, 0));

                #endregion

                //商品库存数变化
                sqlGoodStorage.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore-{0} WHERE GoodID={1} AND WBID={2} and WBWareHouseID={3}", GoodCount, GoodID, WBID, WBWareHouseID));
            }
            #endregion

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    //兑换记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlEx.ToString());
                    //日志记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlO_Log.ToString());
                    //商品库存数变化
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlGoodStorage.ToString());
                   
                    tran.Commit();

                    var res = new { state = "success", msg = "商品销售成功!", BNOList = Fun.ListToString(BNOList, '|') };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "success", msg = "执行sql错误!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion
        }


        /// <summary>
        /// 添加积分兑换
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodExchangeIntegral(HttpContext context)
        {
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string Dep_Name = context.Request.Form["strName"].ToString();
            string exlist = context.Request.Form["exlist"].ToString();//兑换列表
            DataTable dtexlist = JsonHelper.JsonToDataTable(exlist);
            if (dtexlist == null || dtexlist.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "请选择您要购买的商品!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号

            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "积分兑换";//GoodExchange存储的兑换业务名称
            string BusinessName_Log = "15";//Dep_OperateLog存储的兑换业务名称


            #region 循环处理所有记录
          
            StringBuilder sqlEx = new StringBuilder();//所有的兑换sql
            StringBuilder sqlO_Log = new StringBuilder();//所有的日志sql
            StringBuilder sqlGoodStorage = new StringBuilder();//每次仓库商品数量发生的变化


            string strGUID = Fun.getGUID();//防伪码
            string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + AccountNumber + BusinessNO;

            DataRow row = dtexlist.Rows[0];
            //string VarietyID = row["VarietyID"].ToString();//产品ID
            string WBWareHouseID = row["WBWareHouseID"].ToString();//商品仓库ID
            string GoodID = row["GoodID"].ToString();//兑换商品ID
            string GoodName = row["GoodName"].ToString();//商品名称
            string SpecName = row["SpecName"].ToString();//商品规格
            string UnitName = row["UnitName"].ToString();//商品计量单位

            double GoodCount = Convert.ToDouble(row["GoodCount"]);//兑换商品数量
            string GoodPrice = row["GoodPrice"].ToString();//兑换商品单价
            double GoodValue = Convert.ToDouble(row["GoodValue"]);
            double integral_Change = Convert.ToDouble(row["integral_Change"]);
            double integral_Total = Convert.ToDouble(row["integral_Total"]);
            integral_Total = integral_Total - integral_Change;
            #region 兑换记录

            sqlEx.Append("  insert into [GoodExchangeIntegral] (");
            sqlEx.Append("SerialNumber,strGUID,BusinessNO,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,GoodValue,WBWareHouseID,integral_Change,dt_Integral)");
            sqlEx.Append(" values (");

            sqlEx.Append(string.Format("'{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}',{8},'{9}','{10}','{11}',{12},{13},{14},{15},{16},'{17}')", SerialNumber, strGUID, BusinessNO, AccountNumber, Dep_Name, WBID, UserID, BusinessName, GoodID, GoodName, SpecName, UnitName, GoodCount, GoodPrice, GoodValue, WBWareHouseID, integral_Change, DateTime.Now.ToString()));
            sqlEx.Append(" select @@identity");

            #endregion

            #region 日志记录

            sqlO_Log.Append("  insert into [Dep_OperateLog] (");
            sqlO_Log.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
            sqlO_Log.Append(" values (");

            sqlO_Log.Append(string.Format("{0},{1},'{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},{11},'{12}','{13}','{14}',{15})", WBID, UserID, AccountNumber, BusinessNO, BusinessName_Log, GoodID, UnitName, GoodPrice, GoodCount, 0, GoodValue, 0, DateTime.Now.ToString(), GoodName, UnitName, 0));

            #endregion

            //商品库存数变化
            sqlGoodStorage.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore-{0} WHERE GoodID={1} AND WBID={2} and WBWareHouseID={3}", GoodCount, GoodID, WBID, WBWareHouseID));

            //用户积分数变化
          
            #endregion

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    //兑换记录
                  object objIntegralID=  SQLHelper.ExecuteScalar(tran, CommandType.Text, sqlEx.ToString());
                    //日志记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlO_Log.ToString());
                    //商品库存数变化
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlGoodStorage.ToString());
                    //储户积分变化
                    StringBuilder sqlIntegral = new StringBuilder();
                    sqlIntegral.Append(" INSERT INTO dbo.Dep_Integral");
                    sqlIntegral.Append("  ( numType ,GEIntegralID, AccountNumber ,AccountNumber_New, numLevel ,integral_Change ,integral_Total , dt_Add)");
                    sqlIntegral.Append(string.Format(" VALUES  ( 2,{0}, N'{1}' , N'{2}' , {3} , {4} ,   {5} ,  GETDATE() )", objIntegralID.ToString(), AccountNumber, "", 1, integral_Change, integral_Total));
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlIntegral.ToString());
                    tran.Commit();

                    var res = new { state = "success", msg = "积分兑换成功!", BusinessNO = BusinessNO };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "success", msg = "执行sql错误!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion
        }


        /// <summary>
        /// 储户的存折内容打印(打印多条存折的记录)
        /// </summary>
        /// <param name="context"></param>
        void PrintDep_OperateLogList(HttpContext context)
        {
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string BNOList = context.Request.Form["BNOList"].ToString();
           
            string BNListSurPlue = "";//剩余的需要打印的编号集合
            string[] BNArray = BNOList.Split('|');//需要打印的编号集合

            string BusinessNO = BNArray[0];//首个编号
          

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
                    strSqlLog.Append(" select A.ID, (CASE  A.BusinessName WHEN '1' THEN '存入' WHEN '2' THEN '兑换' WHEN '3' THEN '存转销'  WHEN '4' THEN '提取' WHEN '5' THEN '修改' WHEN '6' THEN '退还兑换' WHEN '7' THEN '退还存转销' WHEN '8' THEN '退还存粮' WHEN '9' THEN '产品换购' WHEN '10' THEN '退还换购' WHEN '11' THEN '结息' WHEN '12' THEN '换存折' END  ) AS BusinessName ");
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
                    double goodCount = Convert.ToDouble(Money_Trade) / Convert.ToDouble(Price);
                    strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>" + Math.Round(goodCount, 2).ToString() + "</td>");
                    strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>" + Count_Trade + "</td>");
                    strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>" + Count_Balance + "kg</td>");
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
        /// 打印商品兑换凭证(多个凭证)
        /// </summary>
        /// <param name="context"></param>
        void PrintGoodExchangeList(HttpContext context)
        {
            
            string model = context.Request.Form["model"].ToString();//需要打印的序列组合
            string BNOList = context.Request.Form["BNOList"].ToString();//需要打印的序列组合

            string AccountNumber = context.Request.Form["AccountNumber"].ToString();

            string[] BNArray = BNOList.Split('|');//需要打印的编号集合

            string BusinessNO = BNArray[0];//首个编号

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
            else
            {
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
        /// 打印商品兑换凭证(多个凭证)
        /// </summary>
        /// <param name="context"></param>
        void PrintGoodExchangeGroupList(HttpContext context)
        {

            string model = context.Request.Form["model"].ToString();//需要打印的序列组合
            string BNOList = context.Request.Form["BNOList"].ToString();//需要打印的序列组合

            string AccountNumber = context.Request.Form["AccountNumber"].ToString();

            string[] BNArray = BNOList.Split('|');//需要打印的编号集合

            string BusinessNO = BNArray[0];//首个编号

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
            strSqlLog.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,CONVERT(NVARCHAR(100),dt_Exchange,23) AS  dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn,exchangeGroupProp,orderdate,orderdateDone, orderstate ");
            strSqlLog.Append("  FROM dbo.GoodExchangeGroup");
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
                strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户分时批量兑换凭证</span>" + printValue + "</td> </tr>");
            }
            else
            {
                strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户(退还)分时批量兑换凭证</span>" + printValue + "</td> </tr>");

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
            double T_Group_YouHui=0;
            for (int i = 0; i < BNArray.Length; i++)
            {
                BusinessNO = BNArray[i];
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,VarietyCount,VarietyInterest,Money_DuiHuan,Money_YouHui,CONVERT(NVARCHAR(100),dt_Exchange,23) AS  dt_Exchange,JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total,ISReturn  ");
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
            strReturn.Append("   <td > <span>利息合计：</span></td>  <td>  <span>￥" + Math.Round(T_VarietyInterest, 2) + Math.Round(T_Money_YouHui, 2)+"元</span>  </td>");
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
        /// 打印商品销售凭证(多个凭证)
        /// </summary>
        /// <param name="context"></param>
        void PrintGoodSellList(HttpContext context)
        {

            string model = context.Request.Form["model"].ToString();//需要打印的序列组合
            string BNOList = context.Request.Form["BNOList"].ToString();//需要打印的序列组合

            string[] BNArray = BNOList.Split('|');//需要打印的编号集合

      
            string BusinessNOList = "";
            for (int i = 0; i < BNArray.Length; i++) {
                BusinessNOList += "'" + BNArray[i] + "',";
            }
            BusinessNOList = BusinessNOList.Substring(0, BusinessNOList.Length - 1);


            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select ID,SerialNumber,strGUID,BusinessNO,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,GoodValue,CONVERT(NVARCHAR(100),dt_Sell,23) AS dt_Sell,ISReturn  ");
            strSqlLog.Append("  FROM dbo.GoodSell");
            strSqlLog.Append("  ");
            strSqlLog.Append(" where BusinessNO in(" + BusinessNOList + ") and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }

            //共有参数
          
            string strGUID = dtLog.Rows[0]["strGUID"].ToString();//
            string SerialNumber = dtLog.Rows[0]["SerialNumber"].ToString();//
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();//
            string Dep_AccountNumber = dtLog.Rows[0]["Dep_AccountNumber"].ToString();//
            string dt_Sell = dtLog.Rows[0]["dt_Sell"].ToString();

            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string UserID = dtLog.Rows[0]["UserID"].ToString();
            string WBName = SQLHelper.ExecuteScalar(" SELECT top 1 strName FROM dbo.WB WHERE ID=" + WBID).ToString();
            string UserName = SQLHelper.ExecuteScalar(" SELECT top 1 strLoginName   FROM dbo.Users WHERE ID=" + UserID).ToString();

           
            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            if (model == "")
            {
                strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户商品购买凭证</span></td> </tr>");
            }
            else
            {
                strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户(退还)商品购买凭证</span></td> </tr>");

            }
            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + SerialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");

            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 150px;'>  <span >姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 150px;'>  <span >账号：" + AccountNumber + "</span> </td>");
            strReturn.Append("    <td >  <span >营业网点：" + WBName + "</span> </td>");
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
            strReturn.Append("    <td style='width: 150px;'> <span>商品名</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>单位</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>数量</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>单价</span></td>");
            strReturn.Append("    <td style='width: 100px;'> <span>金额</span></td>");
            strReturn.Append("  </tr>");

            double T_Jine = 0;//商品数量
            double T_GoodCount = 0;
            for (int i = 0; i < dtLog.Rows.Count; i++)
            {
                DataRow row = dtLog.Rows[i];
                string BusinessName = row["BusinessName"].ToString();//业务名
                string GoodName = row["GoodName"].ToString();//品名
                string SpecName = row["SpecName"].ToString();
                string UnitName = row["UnitName"].ToString();

                double GoodCount = Convert.ToDouble(row["GoodCount"]);//商品数量
                double GoodPrice = Convert.ToDouble(row["GoodPrice"]);//商品价格
                double GoodValue = Convert.ToDouble(row["GoodValue"]);//商品价格
                T_GoodCount += GoodCount;
                T_Jine += GoodValue;

                #region 返回信息

                strReturn.Append("   <tr style='height: 20px;'>");
                strReturn.Append("    <td > <span>" + BusinessName + "</span></td>");
                strReturn.Append("    <td> <span>" + GoodName + "</span></td>");
                strReturn.Append("    <td> <span>" + UnitName + "</span></td>");
                strReturn.Append("    <td> <span>" + GoodCount + "</span></td>");
                strReturn.Append("    <td> <span>" + GoodPrice + "</span></td>");
                strReturn.Append("    <td> <span>" + GoodValue + "</span></td>");
                strReturn.Append("  </tr>");


                #endregion
            }
            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td style='width: 200px;'> <span>共购买商品：</span><span>" + T_GoodCount + "件</span>  </td>");
            strReturn.Append("   <td > <span>总价值:" + T_Jine + "元</span></td>");
            strReturn.Append("  </tr>");

          
           

            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>营业员：</span> <span>" + UserName + "</span>  </td>");
            strReturn.Append("  <td>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");


            context.Response.Write(strReturn.ToString());
        }


        /// <summary>
        /// 打印积分兑换商品
        /// </summary>
        /// <param name="context"></param>
        void PrintGoodExchangeIntegral(HttpContext context)
        {

           // string model = context.Request.Form["model"].ToString();//需要打印的序列组合
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();//需要打印的序列组合
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select   TOP 1 SerialNumber,strGUID,BusinessNO,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,GoodID,GoodName,SpecName,UnitName,GoodCount,GoodPrice,GoodValue,A.integral_Change,  ");
            strSqlLog.Append(" CONVERT(NVARCHAR(100),dt_Integral,23) AS dt_Integral,B.integral_Total ");
            strSqlLog.Append("  FROM dbo.GoodExchangeIntegral A INNER JOIN dbo.Dep_Integral B ON A.Dep_AccountNumber=B.AccountNumber");
            
            strSqlLog.Append(" where BusinessNO ='" + BusinessNO + "' and  A.Dep_AccountNumber='" + AccountNumber + "'");
            strSqlLog.Append("  ORDER BY B.dt_Add DESC");
            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }

            //共有参数

            string strGUID = dtLog.Rows[0]["strGUID"].ToString();//
            string SerialNumber = dtLog.Rows[0]["SerialNumber"].ToString();//
            string Dep_Name = dtLog.Rows[0]["Dep_Name"].ToString();//
            string Dep_AccountNumber = dtLog.Rows[0]["Dep_AccountNumber"].ToString();//
            string dt_Integral = dtLog.Rows[0]["dt_Integral"].ToString();
            string integral_Change = dtLog.Rows[0]["integral_Change"].ToString();
            string integral_Total = dtLog.Rows[0]["integral_Total"].ToString();

            string WBID = dtLog.Rows[0]["WBID"].ToString();
            string UserID = dtLog.Rows[0]["UserID"].ToString();
            string WBName = SQLHelper.ExecuteScalar(" SELECT top 1 strName FROM dbo.WB WHERE ID=" + WBID).ToString();
            string UserName = SQLHelper.ExecuteScalar(" SELECT top 1 strLoginName   FROM dbo.Users WHERE ID=" + UserID).ToString();


            StringBuilder strReturn = new StringBuilder();
            //标题
            string CompanyName = common.GetCompanyInfo()["strName"].ToString();
            strReturn.Append("  <table style='width: 640px; padding: 10px 0px;'>");
            strReturn.Append("   <tr><td align='center' style='font-size: 18px; font-weight: bolder; text-align: center;'><span>" + CompanyName + "  储户积分兑换凭证</span></td> </tr>");

            strReturn.Append("   <tr><td align='center' style='font-size: 12px;  text-align: center;'> <span>防伪码：" + strGUID + "</span>  &nbsp;&nbsp;<span>编号：" + SerialNumber + "</span> </td> </tr>");
            strReturn.Append("  </table>");

            //首行内容
            strReturn.Append("  <table style='font-size: 14px; padding-bottom:5px;'><tr>");
            strReturn.Append("    <td style='width: 150px;'>  <span >姓名：" + Dep_Name + "</span> </td>");
            strReturn.Append("    <td style='width: 150px;'>  <span >账号：" + AccountNumber + "</span> </td>");
            strReturn.Append("    <td >  <span >营业网点：" + WBName + "</span> </td>");
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
            strReturn.Append("    <td style='width: 150px;'> <span>商品名</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>单位</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>数量</span></td>");
            strReturn.Append("    <td style='width: 80px;'> <span>消费积分</span></td>");
            strReturn.Append("    <td style='width: 100px;'> <span>剩余积分</span></td>");
            strReturn.Append("  </tr>");

            DataRow row = dtLog.Rows[0];
            string BusinessName = row["BusinessName"].ToString();//业务名
            string GoodName = row["GoodName"].ToString();//品名
            string SpecName = row["SpecName"].ToString();
            string UnitName = row["UnitName"].ToString();

            double GoodCount = Convert.ToDouble(row["GoodCount"]);//商品数量
            double GoodPrice = Convert.ToDouble(row["GoodPrice"]);//商品价格
            double GoodValue = Convert.ToDouble(row["GoodValue"]);//商品价格
           

            strReturn.Append("   <tr style='height: 20px;'>");
            strReturn.Append("    <td > <span>" + BusinessName + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodName + "</span></td>");
            strReturn.Append("    <td> <span>" + UnitName + "</span></td>");
            strReturn.Append("    <td> <span>" + GoodCount + "</span></td>");
            strReturn.Append("    <td> <span>" + integral_Change + "</span></td>");
            strReturn.Append("    <td> <span>" + integral_Total + "</span></td>");
            strReturn.Append("  </tr>");
            //第三行内容
            strReturn.Append("   <table style='font-size: 14px; padding:5px 0px;'>");
            strReturn.Append("    <tr style='height: 25px;'>");
            strReturn.Append("   <td > <span>营业员：</span> <span>" + UserName + "</span>  </td>");
            strReturn.Append("  <td>   <span>储户签名：</span> </td><td> <div style='width:100px;height:25px; border-bottom:1px solid #333;'></div></td>");
            strReturn.Append("   </tr>   </table>");


            context.Response.Write(strReturn.ToString());
        }

        /// <summary>
        /// 添加储户存储信息
        /// </summary>
        /// <param name="context"></param>
        void Add_Dep_Storage(HttpContext context)
        {

            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            string StorageRateID = context.Request.Form["StorageRateID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string TypeID = context.Request.Form["TypeID"].ToString();

            string TimeID = context.Request.Form["TimeID"].ToString();
            string StorageFee = context.Request.Form["StorageFee"].ToString();
            string WeighNo = context.Request.Form["WeighNO"].ToString();
            string StorageNumber = context.Request.Form["StorageNumber"].ToString();

            string CurrentRate = "";
            string Price_ShiChang = "";
            string Price_DaoQi = "";
            string Price_HeTong = "";

            //由价格与利率表获取当前的价格信息
            StringBuilder strSqlPrice = new StringBuilder();
            strSqlPrice.Append(" select ID,TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,");
            strSqlPrice.Append(" CurrentRate,EarningRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou ");
            strSqlPrice.Append(" FROM  dbo.StorageRate");
            strSqlPrice.Append("  WHERE VarietyID= " + VarietyID + " and TypeID=" + TypeID + " and TimeID=" + TimeID);
            DataTable dtPrice = SQLHelper.ExecuteDataTable(strSqlPrice.ToString());
            if (dtPrice != null && dtPrice.Rows.Count != 0)
            {
                CurrentRate = dtPrice.Rows[0]["CurrentRate"].ToString();
                Price_ShiChang = dtPrice.Rows[0]["Price_ShiChang"].ToString();
                Price_DaoQi = dtPrice.Rows[0]["Price_DaoQi"].ToString();
                Price_HeTong = dtPrice.Rows[0]["Price_HeTong"].ToString();
            }
            else
            {
                context.Response.Write("Price");
                return;
            }



            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Dep_StorageInfo] (");
            strSql.Append("AccountNumber,StorageRateID,VarietyID,TypeID,TimeID,StorageDate,WeighNo,StorageNumber,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong)");
            strSql.Append(" values (");
            strSql.Append("@AccountNumber,@StorageRateID,@VarietyID,@TypeID,@TimeID,@StorageDate,@WeighNo,@StorageNumber,@StorageFee,@CurrentRate,@Price_ShiChang,@Price_DaoQi,@Price_HeTong)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageRateID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@WeighNo", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageNumber", SqlDbType.Int,4),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9)};
            parameters[0].Value = AccountNumber;
            parameters[1].Value = StorageRateID;
            parameters[2].Value = VarietyID;
            parameters[3].Value = TypeID;
            parameters[4].Value = TimeID;
            parameters[5].Value = DateTime.Now;
            parameters[6].Value = WeighNo;
            parameters[7].Value = StorageNumber;
            parameters[8].Value = StorageFee;
            parameters[9].Value = CurrentRate;
            parameters[10].Value = Price_ShiChang;
            parameters[11].Value = Price_DaoQi;
            parameters[12].Value = Price_HeTong;
            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }
        //暂未修改
        void Update_Dep_Storage(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string WeighNo = context.Request.Form["WeighNO"].ToString();
            string StorageNumber = context.Request.Form["StorageNumber"].ToString();

            string strSql = "UPDATE dbo.Dep_StorageInfo SET WeighNo='" + WeighNo + "',StorageNumber=" + StorageNumber + " WHERE ID=" + ID;

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
        /// 添加储户存储信息
        /// </summary>
        /// <param name="context"></param>
        void GetByID_Dep_Storage(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            //获取存粮信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("SELECT A.ID,A.StorageNumber,A.StorageDate,A.WeighNo, A.AccountNumber,B.strName AS VarietyID,A.Price_ShiChang,A.Price_DaoQi,C.strName AS TimeID,A.StorageFee");
            strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSqlStorage.Append("  INNER JOIN dbo.StorageTime C ON A.TimeID=C.ID");
            strSqlStorage.Append("  WHERE A.ID=" + ID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());

            if (dtStorage != null && dtStorage.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dtStorage));
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        /// <summary>
        /// 根据Dep_Storage表的ID查询单条存储信息
        /// </summary>
        /// <param name="context"></param>
        void GetStorageInfoByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            //获取存粮信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("SELECT A.VarietyID as txtVarietyID,A.TypeID as txtTypeID, A.TimeID as txtTimeID, B.strName AS VarietyID,D.strName as UnitID, A.StorageNumber,A.StorageNumberRaw,convert(varchar(10),StorageDate,120) AS StorageDate,");
            strSqlStorage.Append("  A.Price_ShiChang,C.strName AS TimeID,DATEDIFF(DAY,A.StorageDate,GETDATE())AS numDate");
            strSqlStorage.Append("  FROM  dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSqlStorage.Append("  INNER JOIN dbo.StorageTime C ON A.TimeID=C.ID");
            strSqlStorage.Append("  INNER JOIN dbo.BD_MeasuringUnit D ON B.MeasuringUnitID=D.ID");
            strSqlStorage.Append("  WHERE A.ID=" + ID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());

            if (dtStorage != null && dtStorage.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dtStorage));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        ///// <summary>
        /////  获取当前存储的可结息状态
        ///// </summary>
        ///// <param name="context"></param>
        //void GetInterestState(HttpContext context)
        //{

             
        //    string ID = context.Request.QueryString["ID"].ToString();
        //    int ISJiexi = 0;//是否允许仅结息操作
        //    double Interest = 0;
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append(" select B.InterestType,InterestDate,StorageDate,B.numStorageDate,WeighNo,StorageNumber,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong ");
        //    strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
        //    strSql.Append(" WHERE A.ID=" + ID);
        //    DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
        //    if (dt != null && dt.Rows.Count != 0)
        //    {
        //        int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
        //        int StorageNumber = Convert.ToInt32(dt.Rows[0]["StorageNumber"]);//存储数量
        //        DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
        //        DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);
        //        TimeSpan tsStorage = DateTime.Now.Subtract(StorageDate);
        //        TimeSpan tsInterest = DateTime.Now.Subtract(InterestDate);
        //        int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
        //        double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//活期利率
        //        double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//市场价
        //        double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价
        //        switch (InterestType)
        //        {
        //            case 1://按月结息方式
        //                Interest = StorageNumber * CurrentRate * tsInterest.TotalDays / (double)30;
        //                ISJiexi = 1;
        //                break;
        //            case 2://按市场价结息方式
        //                if (tsStorage.TotalDays < numStorageDate)
        //                {
        //                    Interest = -1;
        //                }
        //                else
        //                {
        //                    //计算当前的市场价格
        //                    StringBuilder strSqlShiChang = new StringBuilder();
        //                    strSqlShiChang.Append(" SELECT B.Price_ShiChang,B.EarningRate");
        //                    strSqlShiChang.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageRate B ON A.StorageRateID=B.ID");
        //                    strSqlShiChang.Append(" WHERE A.ID=" + ID);
        //                    DataTable dtShiChang = SQLHelper.ExecuteDataTable(strSqlShiChang.ToString());
        //                    if (dtShiChang != null && dtShiChang.Rows.Count != 0)
        //                    {
        //                        double Price_JieCun = Convert.ToDouble(dtShiChang.Rows[0]["Price_ShiChang"]);
        //                        int EarningRate = Convert.ToInt32(dtShiChang.Rows[0]["EarningRate"]);
        //                        if (Price_JieCun >= Price_ShiChang)//到期的结存价格比现在的市场价高
        //                        {
        //                            Interest = (Price_JieCun - Price_ShiChang) * StorageNumber * EarningRate / (double)100;
                                  
        //                        }
        //                        else
        //                        {
        //                            Interest = -(Price_ShiChang - Price_JieCun) * StorageNumber;
        //                          }
        //                    }
        //                }
        //                break;
        //            case 3://定期结息方式
        //                if (tsStorage.TotalDays < numStorageDate)
        //                {
        //                    Interest = -1;
        //                }
        //                else
        //                {
        //                    Interest = (Price_DaoQi - Price_ShiChang) * StorageNumber;
                           
        //                }
        //                break;
        //        }
               
        //    }
        //    Interest = Math.Round(Interest, 2);
        //    string strReturn="[{\"Interest\":\""+Interest+"\",\"ISJiexi\":\""+ISJiexi+"\"}]";
        //    context.Response.Write(strReturn);
        //}


        /// <summary>
        ///  获取当前存储的可结息状态
        /// </summary>
        /// <param name="context"></param>
        void GetInterestState(HttpContext context)
        {


            string ID = context.Request.QueryString["ID"].ToString();
            int ISJiexi = 0;//是否允许仅结息操作
            double numLixi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.InterestType,InterestDate,StorageDate,B.numStorageDate,WeighNo,StorageNumber,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong ");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + ID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
                int StorageNumber = Convert.ToInt32(dt.Rows[0]["StorageNumber"]);//存储数量
                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);
                TimeSpan tsStorage = DateTime.Now.Subtract(StorageDate);
                TimeSpan tsInterest = DateTime.Now.Subtract(InterestDate);
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
              
                switch (InterestType)
                {
                    case 1://按月结息方式
                        numLixi = common.GetLiXi_FuXi(ID, StorageNumber);
                        ISJiexi = 1;
                        break;
                    case 2://按市场价结息方式
                        if (tsInterest.TotalDays < numStorageDate)
                        {
                            ISJiexi = 0;
                        }
                        else
                        {
                            numLixi = common.GetLiXi_FenHong(ID, StorageNumber);
                            ISJiexi = 1;
                        }
                        break;
                    case 3://定期结息方式
                        if (tsInterest.TotalDays < numStorageDate)
                        {
                            ISJiexi = 0;
                        }
                        else
                        {
                            numLixi = common.GetLiXi_DingQi(ID, StorageNumber);
                            ISJiexi = 1;
                        }
                        break;
                    case 4://定期结息方式
                        if (tsInterest.TotalDays < numStorageDate)
                        {
                            ISJiexi = 0;
                        }
                        else
                        {
                            numLixi = common.GetLiXi_RuGu(ID, StorageNumber);
                            ISJiexi = 1;
                        }
                        break;
                }

            }
            numLixi = Math.Round(numLixi, 2);
            string strReturn = "[{\"numLixi\":\"" + numLixi + "\",\"ISJiexi\":\"" + ISJiexi + "\"}]";
            context.Response.Write(strReturn);
        }

        /// <summary>
        /// 根据存期类型和存储产品类型来确定一条价格比例
        /// </summary>
        /// <param name="context"></param>
        void GetStorageRateByID(HttpContext context)
        {
            string TimeID = context.Request.QueryString["TimeID"].ToString();
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select A.ID,A.TypeID,A.VarietyID,A.VarietyLevelID,A.TimeID,A.StorageFee,A.BankRate,A.CurrentRate,A.EarningRate,");
            strSql.Append("A.Price_ShiChang,A.Price_DaoQi,A.Price_HeTong,A.Price_XiaoShou,B.ISRegular,B.InterestType,B.numStorageDate,B.PricePolicy");
            strSql.Append(" FROM dbo.StorageRate A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID ");
        
            strSql.Append(" where A.TimeID=" + TimeID + " and  A.VarietyID=" + VarietyID);

            //获取存粮信息

            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dtStorage != null && dtStorage.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dtStorage));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetSRTByID(HttpContext context)
        {
            string StorageRateID = context.Request.QueryString["StorageRateID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select A.ID,A.TypeID,A.VarietyID,A.VarietyLevelID,A.TimeID,A.StorageFee,A.BankRate,A.CurrentRate,A.EarningRate,");
            strSql.Append("A.Price_ShiChang,A.Price_DaoQi,A.Price_HeTong,A.Price_XiaoShou,B.ISRegular,B.InterestType,B.numStorageDate,B.PricePolicy");
            strSql.Append(" FROM dbo.StorageRate A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID ");

            strSql.Append(" where A.ID=" + StorageRateID);

            //获取存粮信息

            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dtStorage != null && dtStorage.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dtStorage));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void StoreToSell2(HttpContext context) {
            context.Response.Write("Error");
        }

        /// <summary>
        /// 存转销
        /// </summary>
        /// <param name="context"></param>
        void StoreToSell(HttpContext context)
        {
            //参数
            string ApplyID = context.Request.Form["ApplyID"];//存转销申请编号
            string dsiID = context.Request.Form["dsiID"].ToString();//兑换产品ID
            double VarietyCount =Convert.ToDouble( context.Request.Form["VarietyCount"].ToString());
            double VarietyMoney_input = Convert.ToDouble(context.Request.Form["VarietyMoney"].ToString());
            string calcType = context.Request.Form["calcType"].ToString();//计算类型，1：计算，2：反算

            //获取储户信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("  SELECT top 1  A.ID,A.AccountNumber,B.strName AS Dep_Name, VarietyID,TypeID,TimeID,StorageNumber, StorageNumberRaw,Price_ShiChang,Price_DaoQi");
            strSqlStorage.Append("   ,A.CurrentRate,A.StorageFee,A.StorageDate,DATEDIFF( Day, A.StorageDate,GETDATE())AS daycount,C.strName AS VarietyName,D.strName AS UnitName");
            strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlStorage.Append("  LEFT JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSqlStorage.Append("   LEFT JOIN dbo.BD_MeasuringUnit D ON C.MeasuringUnitID=D.ID");
            strSqlStorage.Append("  WHERE A.ID=" + dsiID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            if (dtStorage == null || dtStorage.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "获取储户的存储信息错误!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            string AccountNumber = dtStorage.Rows[0]["AccountNumber"].ToString();//储户账号
            string Dep_Name = dtStorage.Rows[0]["Dep_Name"].ToString();//储户名
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号
            string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + AccountNumber + BusinessNO;
            string strGUID = Fun.getGUID();//防伪码
            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "存转销";//GoodExchange存储的兑换业务名称
            string BusinessName_Log = "3";//Dep_OperateLog存储的兑换业务名称

            string UnitName = dtStorage.Rows[0]["UnitName"].ToString();//单位名称
            string VarietyID = dtStorage.Rows[0]["VarietyID"].ToString();//产品ID
            string VarietyName = dtStorage.Rows[0]["VarietyName"].ToString();//产品名称


            Dictionary<string, string> dicSell = common.FunJiSuan(context, dsiID, VarietyCount);

            double VarietyMoney = Convert.ToDouble(dicSell["Money"]);//存转销金额
          
            //获取当前营业员的存转销限额
            double limitAmount = Convert.ToDouble(context.Request.Form["limitMount"]);
            if (VarietyMoney > limitAmount) {
                var res = new { state = "error", msg = "存转销金额已大于当前营业员的操作限额，无法完成操作!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            double VarietyInterest = Convert.ToDouble(dicSell["LiXi"]);//存转销产生利息
            string StorageDate = dtStorage.Rows[0]["daycount"].ToString();//实际存储天数
            string CurrentRate = dtStorage.Rows[0]["CurrentRate"].ToString();//活期利率
            double EarningRate = VarietyInterest / VarietyMoney;//盈利率
            string StorageFee = dtStorage.Rows[0]["StorageFee"].ToString();//保管费率
            double StorageMoney = Convert.ToDouble(dicSell["BGF"]);//保管费
            string Price_JieSuan = dtStorage.Rows[0]["Price_ShiChang"].ToString();
            //double Money_Earn = VarietyInterest - StorageMoney;//总收益
            double Money_Earn = 0;
            if (calcType == "2")
            {
                Money_Earn = VarietyMoney_input;//如果是用反算，则结算金额采用输入值。
            }
            else
            {
                Money_Earn = VarietyMoney + VarietyInterest - StorageMoney;//总结算金额
            }
            
            string dt_Sell = DateTime.Now.ToString();

            double StorageNumber = Convert.ToDouble(dtStorage.Rows[0]["StorageNumber"]);//该产品上一次的剩余结存
            if (StorageNumber <= 0)
            {
                var res = new { state = "error", msg = "该笔存储剩余结存不足，无法完成操作!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            double StorageNumberRaw = Convert.ToDouble(dtStorage.Rows[0]["StorageNumberRaw"]);//该产品原始结
            double JieCun_Last = StorageNumber;//上次结存
            double JieCun_Now = StorageNumber - VarietyCount;//现在结存
            if (JieCun_Now <0)
            {
                var res = new { state = "error", msg = "该笔存储剩余结存不足，无法完成操作!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            double Count_Balance = common.GetDep_StorageNumber(AccountNumber, VarietyID);//储户总结存
            Count_Balance = Count_Balance - VarietyCount;
          
            #region 写入存转销记录
            StringBuilder strSqlInsert = new StringBuilder();
            strSqlInsert.Append("insert into [StorageSell] (");
            strSqlInsert.Append("SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,UnitName,VarietyID,VarietyName,VarietyCount,VarietyMoney,VarietyInterest,StorageDate,CurrentRate,EarningRate,StorageFee,StorageMoney,Price_JieSuan,Money_Earn,dt_Sell,JieCun_Last,JieCun_Now,ISReturn)");
            strSqlInsert.Append(" values (");
            strSqlInsert.Append("@SerialNumber,@strGUID,@BusinessNO,@Dep_SID,@Dep_AccountNumber,@Dep_Name,@WBID,@UserID,@BusinessName,@UnitName,@VarietyID,@VarietyName,@VarietyCount,@VarietyMoney,@VarietyInterest,@StorageDate,@CurrentRate,@EarningRate,@StorageFee,@StorageMoney,@Price_JieSuan,@Money_Earn,@dt_Sell,@JieCun_Last,@JieCun_Now,@ISReturn)");
            strSqlInsert.Append(";select @@IDENTITY");
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
            parameters[3].Value = dsiID;
            parameters[4].Value = AccountNumber;
            parameters[5].Value = Dep_Name;
            parameters[6].Value = WBID;
            parameters[7].Value = UserID;
            parameters[8].Value = BusinessName;
            parameters[9].Value = UnitName;
            parameters[10].Value = VarietyID;
            parameters[11].Value = VarietyName;
            parameters[12].Value = VarietyCount;
            parameters[13].Value = Math.Round(VarietyMoney, 2);
            parameters[14].Value = Math.Round(VarietyInterest, 2);
            parameters[15].Value = StorageDate;
            parameters[16].Value = CurrentRate;
            parameters[17].Value = Math.Round(EarningRate, 4);//盈利率(暂用)
            parameters[18].Value = StorageFee;
            parameters[19].Value = Math.Round(StorageMoney, 2);
            parameters[20].Value = Price_JieSuan;
            parameters[21].Value = Money_Earn;
            parameters[22].Value = dt_Sell;
            parameters[23].Value = JieCun_Last;
            parameters[24].Value = JieCun_Now;
            parameters[25].Value = 0;//是否退还
            #endregion

            #region 存转销日志记录

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
            parametersOperateLog[2].Value = AccountNumber;
            parametersOperateLog[3].Value = BusinessNO;
            parametersOperateLog[4].Value = BusinessName_Log;//1:存入 2：兑换  3:存转销 4: 提取
            parametersOperateLog[5].Value = VarietyID;
            parametersOperateLog[6].Value = UnitName;
            parametersOperateLog[7].Value = Price_JieSuan;
            parametersOperateLog[8].Value = VarietyCount;
            parametersOperateLog[9].Value = VarietyCount;
            parametersOperateLog[10].Value = Money_Earn;
            parametersOperateLog[11].Value = Count_Balance;
            parametersOperateLog[12].Value = dt_Sell;
            parametersOperateLog[13].Value = VarietyName;
            parametersOperateLog[14].Value = UnitName;
            parametersOperateLog[15].Value = dsiID;
            #endregion

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    //添加存转销交易记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlInsert.ToString(), parameters);//
                    //修改储户的商品结存
                    string strSqlDep_JieCun = " UPDATE dbo.Dep_StorageInfo  SET StorageNumber=" + JieCun_Now + " WHERE ID=" + dsiID;
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep_JieCun.ToString());//储户结存修改
                    //日志记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加存转销日志记录

                    //取消存转销申请记录
                    if (ApplyID != "")
                    {
                        //string strSqldelete = " DELETE FROM dbo.StorageSellApply WHERE ID=" + ApplyID.ToString();
                        string strSqldelete =string.Format( " update StorageSellApply  set ApplyState={0}  WHERE ID={1}",3,  ApplyID.ToString());//改变存转销申请的状态 
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqldelete.ToString());
                    }

                    tran.Commit();

                    var res = new { state = "success", msg = "存转销成功!", BusinessNO = BusinessNO };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "error", msg = "数据库操作失败!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion


        }


        void GetSellApplyByAN(HttpContext context)
        {
          
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT ID,  Dep_AccountNumber,Dep_Name,CASE (ApplyType) WHEN 1 THEN '存转销' ELSE '提取原粮' END AS ApplyType,VarietyName, VarietyCount,Dep_SID,");
            strSql.Append("  CONVERT(varchar(100), ApplyDate, 111) AS ApplyDate ,  CASE (ApplyState) WHEN 0 THEN '待审核' WHEN 1 THEN '审核通过' WHEN 2 THEN '审核不通过' ELSE '已结算' END AS strApplyState,ApplyState");
            strSql.Append("  FROM dbo.StorageSellApply WHERE 1=1");

           
            if (AccountNumber != "")
            {
                strSql.Append("   AND Dep_AccountNumber='" + AccountNumber + "'");
            }
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                var res=new {state="error",msg="没有查询到该储户的存转销申请记录!"};
                 context.Response.Write(JsonHelper.ToJson(res));
            }
            else {
                var res = new { state = "success", msg = "查询成功!",data=JsonHelper.ToJson(dt) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }


        /// <summary>
        /// 产品换购
        /// </summary>
        /// <param name="context"></param>
        void StorageShopping(HttpContext context)
        {
            #region 存转销交易记录

            string dsiID = context.Request.Form["dsiID"].ToString();//存储产品编号

            //获取储户信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("  SELECT top 1  A.ID,A.AccountNumber,B.strName AS Dep_Name, VarietyID,TypeID,TimeID,StorageNumber, StorageNumberRaw,Price_ShiChang,Price_DaoQi");
            strSqlStorage.Append("   ,A.CurrentRate,A.StorageFee,A.StorageDate,DATEDIFF( Day, A.StorageDate,GETDATE())AS daycount,C.strName AS VarietyName,D.strName AS UnitName");
            strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlStorage.Append("  LEFT JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSqlStorage.Append("   LEFT JOIN dbo.BD_MeasuringUnit D ON C.MeasuringUnitID=D.ID");
            strSqlStorage.Append("  WHERE A.ID=" + dsiID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            if (dtStorage == null || dtStorage.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "获取储户的存储信息错误!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            string AccountNumber = dtStorage.Rows[0]["AccountNumber"].ToString();//储户账号
            string Dep_Name = dtStorage.Rows[0]["Dep_Name"].ToString();//储户名
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号
            string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + AccountNumber + BusinessNO;
            string strGUID = Fun.getGUID();//防伪码
            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "产品换购";//GoodExchange存储的兑换业务名称
            string BusinessName_Log = "9";//Dep_OperateLog存储的兑换业务名称

            string UnitName = dtStorage.Rows[0]["UnitName"].ToString();//单位名称
            string VarietyID = dtStorage.Rows[0]["VarietyID"].ToString();//产品ID
            string VarietyName = dtStorage.Rows[0]["VarietyName"].ToString();//产品名称

            string DepStorageDate = dtStorage.Rows[0]["daycount"].ToString();//实际存储天数
            string Price_ShiChang = dtStorage.Rows[0]["Price_ShiChang"].ToString();//商品存入价格
            string CurrentRate = dtStorage.Rows[0]["CurrentRate"].ToString();//活期利率
            string StorageFee = dtStorage.Rows[0]["StorageFee"].ToString();//保管费率
          
            double VarietyMoney = Convert.ToDouble(context.Request.Form["VarietyMoney"]);//商品价值金额
            double VarietyCount = Convert.ToDouble(context.Request.Form["VarietyCount"]);//折合产品的数量
            double Interest = Convert.ToDouble(context.Request.Form["txtLiXi"]);//利息
            double MoneyFee = Convert.ToDouble(context.Request.Form["txtBGF"]);//保管费
        
            double Money_Earn = VarietyMoney + Interest - MoneyFee;
            double EarningRate = Interest / VarietyMoney;//盈利率
         
           //获取此产品的上一次的结存信息
            double StorageNumber = Convert.ToDouble(dtStorage.Rows[0]["StorageNumber"]);//该产品上一次的剩余结存
            double StorageNumberRaw = Convert.ToDouble(dtStorage.Rows[0]["StorageNumberRaw"]);//该产品原始结
            double JieCun_Last = StorageNumber;//上次结存
            double JieCun_Now = StorageNumber - VarietyCount;//现在结存
            double Count_Balance = common.GetDep_StorageNumber(AccountNumber, VarietyID);//储户总结存
            Count_Balance = Count_Balance - VarietyCount;
            //写入存转销记录
            StringBuilder strSqlInsert = new StringBuilder();
            strSqlInsert.Append("insert into [StorageShopping] (");
            strSqlInsert.Append("SerialNumber,strGUID,BusinessNO,Dep_SID,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,UnitName,VarietyID,VarietyName,VarietyCount,VarietyMoney,VarietyInterest,StorageDate,CurrentRate,EarningRate,StorageFee,StorageMoney,Price_JieSuan,Money_Earn,dt_Sell,JieCun_Last,JieCun_Now,ISReturn)");
            strSqlInsert.Append(" values (");
            strSqlInsert.Append("@SerialNumber,@strGUID,@BusinessNO,@Dep_SID,@Dep_AccountNumber,@Dep_Name,@WBID,@UserID,@BusinessName,@UnitName,@VarietyID,@VarietyName,@VarietyCount,@VarietyMoney,@VarietyInterest,@StorageDate,@CurrentRate,@EarningRate,@StorageFee,@StorageMoney,@Price_JieSuan,@Money_Earn,@dt_Sell,@JieCun_Last,@JieCun_Now,@ISReturn)");
            strSqlInsert.Append(";select @@IDENTITY");
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
            parameters[3].Value = dsiID;
            parameters[4].Value = AccountNumber;
            parameters[5].Value = Dep_Name;
            parameters[6].Value = WBID;
            parameters[7].Value = UserID;
            parameters[8].Value = BusinessName;
            parameters[9].Value = UnitName;
            parameters[10].Value = VarietyID;
            parameters[11].Value = VarietyName;
            parameters[12].Value = VarietyCount;//折合产品数量
            parameters[13].Value = Math.Round(VarietyMoney, 2);
            parameters[14].Value = Math.Round(Interest, 2);
            parameters[15].Value = DepStorageDate;
            parameters[16].Value = CurrentRate;
            parameters[17].Value = Math.Round(EarningRate, 4);//盈利率(暂用)
            parameters[18].Value = StorageFee;
            parameters[19].Value = Math.Round(MoneyFee, 2);
            parameters[20].Value = Price_ShiChang;
            parameters[21].Value = Money_Earn;
            parameters[22].Value = DateTime.Now;
            parameters[23].Value = JieCun_Last;
            parameters[24].Value = Count_Balance;
            parameters[25].Value = 0;//是否退还
            #endregion



            #region 换购日志记录
           
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
            parametersOperateLog[2].Value = AccountNumber;
            parametersOperateLog[3].Value = BusinessNO;
            parametersOperateLog[4].Value = BusinessName_Log;//1:存入 2：兑换  3:存转销 4: 提取
            parametersOperateLog[5].Value = VarietyID;
            parametersOperateLog[6].Value = UnitName;
            parametersOperateLog[7].Value = Price_ShiChang;//产品换购的时候按照市场价换购
            parametersOperateLog[8].Value = VarietyMoney;//商品换购写入商品总价值金额
            parametersOperateLog[9].Value = VarietyCount;
            parametersOperateLog[10].Value = VarietyMoney;
            parametersOperateLog[11].Value = Count_Balance;
            parametersOperateLog[12].Value = DateTime.Now;
            parametersOperateLog[13].Value = VarietyName;
            parametersOperateLog[14].Value = UnitName;
            parametersOperateLog[15].Value = dsiID;
            #endregion

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlInsert.ToString(), parameters);//添加存转销交易记录
                    //修改储户的商品结存
                    string strSqlDep_JieCun = " UPDATE dbo.Dep_StorageInfo  SET StorageNumber=" + JieCun_Now + " WHERE ID=" + dsiID;
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep_JieCun.ToString());//储户结存修改
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加存转销日志记录



                    tran.Commit();

                    var res = new { state = "error", msg = "产品换购成功!", BusinessNO = BusinessNO };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "error", msg = "数据库操作失败!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
            #endregion


        }



        /// <summary>
        /// 根据当前存储信息查找存储产品的储户信息
        /// </summary>
        /// <param name="context"></param>
        void GetDepInfoByStorageID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            //获取存粮信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("select  A.AccountNumber,A.strAddress,A.strName,A.PhoneNO,A.ISSendMessage,A.BankCardNO,");
            strSqlStorage.Append("  CASE( A.IDCard) WHEN '' THEN '未填写' ELSE '******' END AS IDCard,");
            strSqlStorage.Append(" CASE (A.numState) WHEN '1' THEN '正常' ELSE '挂失' END AS numState");
            strSqlStorage.Append("  FROM dbo.Depositor A INNER JOIN dbo.Dep_StorageInfo B ON B.AccountNumber=A.AccountNumber");
            strSqlStorage.Append("  WHERE B.ID=" + ID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());

            if (dtStorage != null && dtStorage.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dtStorage));
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
