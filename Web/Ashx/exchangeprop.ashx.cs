using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Web.SessionState;

namespace Web.BasicData.Charges
{
    /// <summary>
    /// exchangeprop 的摘要说明
    /// </summary>
    public class exchangeprop : IHttpHandler, IRequiresSessionState
    {


        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "GetExchangePropByID": GetExchangePropByID(context); break;

                    case "GetExchangeVarietyCount": GetExchangeVarietyCount(context); break;//获取兑换的时候需要折合的原粮的数量
                    case "UpdateExChangeList": UpdateExChangeList(context); break;//更新用户选择的兑换列表
                    case "DeleteExChangeList": DeleteExChangeList(context); break;//取消一项兑换记录
                        
                    case "Add_GoodExchange": Add_GoodExchange(context); break;
                    case "Return_GoodExchange": Return_GoodExchange(context); break;
                    case "Return_StorageSell": Return_StorageSell(context); break;
                    case "Return_StorageShopping": Return_StorageShopping(context); break;


                    case "SellJiSuan": SellJiSuan(context); break;
                    case "SellFanSuan": SellFanSuan(context); break;
                    case "StoreToSell": StoreToSell(context); break;//添加存转销记录
                    case "StorageShopping": StorageShopping(context); break;//添加产品换购记录
                        
                    case "Add_SellApply": Add_SellApply(context); break;//申请存转销

                    case "ApprovalApply": ApprovalApply(context); break;//存转销审核
                    case "GetApplyCount": GetApplyCount(context); break;//获取申请存转销的数量
                    case "GetApplyInfo": GetApplyInfo(context); break;//获取申请存转销的信息
                    case "GetSellApply_Advice_Request": GetSellApply_Advice_Request(context); break;
                    case "GetSellApply_Advice_Response": GetSellApply_Advice_Response(context); break;
                    case "UpdateAdviceState_Request": UpdateAdviceState_Request(context); break;
                    case "UpdateAdviceState_Response": UpdateAdviceState_Response(context); break;
                    case "JieXi": JieXi(context); break;//结息操作
                }
            }

        }

        /// <summary>
        /// 根据期限类型、产品类型、商品类型 查询兑换比例
        /// </summary>
        /// <param name="context"></param>
        void GetExchangePropByID(HttpContext context)
        {
            string TimeID = context.Request.QueryString["TimeID"].ToString();
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            string GoodID = context.Request.QueryString["GoodID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT ChuFenLv,FuPi,JiaGongFei ");
            strSql.Append("  FROM dbo.GoodExchangeProp ");
            strSql.Append(" WHERE TimeID="+TimeID+" and VarietyID="+VarietyID+" and GoodID="+GoodID);
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
        /// 获取兑换时候需要的产品数量
        /// </summary>
        /// <param name="context"></param>
        void GetExchangeVarietyCount(HttpContext context)
        {
            double numMoney=Convert.ToDouble( context.Request.QueryString["numMoney"]);
            double Exchange_trading = Convert.ToDouble(context.Request.QueryString["Exchange_trading"]);//已经准备兑换的数量
            
            string Dep_SID = context.Request.QueryString["Dep_SID"].ToString();
            double VarietyCount = 0;//需要折合的产品数量
            string strPolicy = "";//兑换政策
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
                    VarietyCount = common.GetExVarietyCount_DingQi(Dep_SID, numMoney,Exchange_trading);
                    strPolicy = common.GetExPolicy_DingQi(Dep_SID, numMoney,Exchange_trading);
                }
                else
                {
                    VarietyCount = common.GetExVarietyCount(Dep_SID, numMoney);
                    strPolicy = common.GetExPolicy(Dep_SID, numMoney, Exchange_trading);
                }
                
                

                bool ISLimit = false;//是否被限额
                bool ISExchangeLimit = Convert.ToBoolean(common.GetWBAuthority()["ISExchangeLimit"]);
                double GoodExchangeLimit = 0;
                double DepExchangeCount = 0;
                if (!ISExchangeLimit) //不对兑换数量做限制
                {
                    ISLimit = false;
                }
                else {
                    //当前商品的兑换额度
                    string GoodID = context.Request.QueryString["GoodID"].ToString();
                     GoodExchangeLimit = Convert.ToDouble(SQLHelper.ExecuteScalar(" SELECT numExchangeLimit  FROM dbo.Good WHERE ID=" + GoodID));
                    //当前储户在这个月已经兑换该商品的数量
                    string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
                    DateTime dtBegin = DateTime.Now.AddDays(1 - DateTime.Now.Day);
                    DateTime dtEnd = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1);
                    string sqlDepExchangeCount = " SELECT SUM(GoodCount) FROM dbo.GoodExchange WHERE Dep_AccountNumber='"+AccountNumber+"' AND GoodID="+GoodID+" AND dt_Exchange BETWEEN '"+dtBegin.ToString("yyyy-MM-dd")+"' AND '"+dtEnd.ToString("yyyy-MM-dd")+"'";
                    
                    object objExchangeCount = SQLHelper.ExecuteScalar(sqlDepExchangeCount);
                    if (objExchangeCount == null||objExchangeCount.ToString()=="")
                    {
                        
                        DepExchangeCount = 0;
                    }
                    else
                    {
                        DepExchangeCount = Convert.ToDouble(objExchangeCount);
                    }
                    double ExchangeCount = Convert.ToDouble(context.Request.QueryString["ExchangeCount"]);
                    if (ExchangeCount + DepExchangeCount > GoodExchangeLimit)
                    {
                        ISLimit = true;
                    }
                    else {
                        ISLimit = false;
                    }
                }

                string strReturnMsg = "{\"VarietyCount\":\"" + VarietyCount + "\",\"strPolicy\":\"" + strPolicy + "\",\"ISLimit\":\"" + ISLimit + "\",\"GoodExchangeLimit\":\"" + GoodExchangeLimit + "\",\"DepExchangeCount\":\"" + DepExchangeCount + "\"}";

                context.Response.Write(strReturnMsg);
            }
            else {
                context.Response.Write("Error");
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
            double Money_YouHui=Math.Round( Convert.ToDouble(  context.Session["Ex_YouHui"] ),2);//优惠金额
            double YouHui_Count=Math.Round( Convert.ToDouble(  context.Session["Ex_YouHui_Count"] ),2);//优惠数量
            context.Session["Ex_YouHui"] = null;
            context.Session["Ex_YouHui_Count"] = null;

            dtExChange.Rows.Add(newindex, "兑换",GoodID, GoodName, SpecName, UnitName, Price_DuiHuan, GoodCount, Ex_LiXi, VarietyCount, JinE,JieCun_Now,Money_YouHui,YouHui_Count);

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
                exchangemsg.Append(" <td><span  style='font-weight:bolder; color:Blue; padding:5px 0px;'><input type='button' value='删除' style='width:60px;' onclick='FunDelList("+row["numIndex"]+");'></span></td>");

                exchangemsg.Append("    </tr>");

            }
            exchangemsg.Append("  <tr>");
            exchangemsg.Append("   <td colspan='9' style='text-align:left'>");
            exchangemsg.Append("   消费金额:<span  style='color:Red'>" + Math.Round(Total_JinE,2) + "</span>元,");
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
            DataTable dtExChange=null;
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
                else {
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
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();//交易编号
            string Dep_SID = context.Request.Form["txtDep_SID"].ToString();//兑换产品ID

            string Dep_AccountNumber = "";//储户账号
            string Dep_Name = "";//储户名
            double JieCun_Raw = Convert.ToDouble(context.Request.Form["txtJieCun_Raw"]);//储户的原始结存
            string VarietyID = context.Request.Form["txtVarietyID"].ToString();
            //获取储户信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("  SELECT A.ID,B.AccountNumber AS Dep_AccountNumber ,B.strName AS Dep_Name,StorageNumberRaw");
            strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlStorage.Append("  WHERE A.ID=" + Dep_SID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            if (dtStorage != null && dtStorage.Rows.Count != 0)
            {
                Dep_AccountNumber = dtStorage.Rows[0]["Dep_AccountNumber"].ToString();
                Dep_Name = dtStorage.Rows[0]["Dep_Name"].ToString();

            }

            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "兑换";//兑换业务


            DataTable dtExChange = null;
            if (context.Cache["ExChange"] != null)
            {
                dtExChange = (DataTable)context.Cache["ExChange"];
            }

            double JieCun_Last = JieCun_Raw;//上一期的结存
            double JieCun_Total = 0;//总共发生的结存
          
            #region 上一次的结存总量
            //获取上次的结存数量
            StringBuilder strSqlEx = new StringBuilder();
            strSqlEx.Append("  SELECT TOP 1 JieCun_Last,JieCun_Now,JieCun_Raw,JieCun_Total");
            strSqlEx.Append("  FROM dbo.GoodExchange");
            strSqlEx.Append("  WHERE Dep_SID=" + Dep_SID);
            strSqlEx.Append("  ORDER BY dt_Exchange DESC");
            strSqlEx.Append("  ");
            DataTable dtEx = SQLHelper.ExecuteDataTable(strSqlEx.ToString());
            if (dtEx != null && dtEx.Rows.Count != 0)
            {
                JieCun_Total = Convert.ToDouble(dtEx.Rows[0]["JieCun_Total"]);
            }
            #endregion

            #region 循环处理所有的兑换记录
            string BusinessNOList = "";//所有的兑换列表的集合
            for (int i = 0; i < dtExChange.Rows.Count; i++)
            {
                if (i != 0)
                {
                    BusinessNO = Fun.ConvertIntToString( Convert.ToInt32(BusinessNO) + 1,4);
                }
                if (BusinessNOList == "")
                {
                    BusinessNOList = BusinessNO;
                }
                else {
                    BusinessNOList =BusinessNOList+"|"+ BusinessNO;
                }
                if (BusinessNO == "")
                {
                    context.Response.Write("Error");
                    return;
                }
                string strGUID = Fun.getGUID();//防伪码
                string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + Dep_AccountNumber + BusinessNO;


                DataRow row = dtExChange.Rows[i];
                string GoodID = row["GoodID"].ToString();//兑换商品ID
                string GoodName = row["GoodName"].ToString();//商品名称
                string SpecName = row["SpecName"].ToString();//商品规格
                string UnitName = row["UnitName"].ToString();//商品计量单位

                string GoodCount = row["GoodCount"].ToString();//兑换商品数量
                string GoodPrice = row["GoodPrice"].ToString();//兑换商品单价
                string VarietyCount = row["VarietyCount"].ToString();//折合产品数量
                string VarietyInterest = row["VarietyLiXi"].ToString();//折合产品利息
                double Money_DuiHuan = Convert.ToDouble(row["JinE"]);
                double Money_YouHui = Convert.ToDouble(row["Money_YouHui"]);
                double JieCun_Now = Convert.ToDouble(row["JieCun"]);
                JieCun_Total = JieCun_Total + JieCun_Last - JieCun_Now;
              
                #region 兑换记录
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
					new SqlParameter("@ISReturn", SqlDbType.Bit,1)};
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
                parameters[24].Value = 0;
                #endregion 


                #region 日志记录
                StringBuilder strSqlOperateLog = new StringBuilder();
                strSqlOperateLog.Append("insert into [Dep_OperateLog] (");
                strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName)");
                strSqlOperateLog.Append(" values (");
                strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade,@VarietyName,@UnitName)");
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
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50)};
                parametersOperateLog[0].Value = WBID;
                parametersOperateLog[1].Value = UserID;
                parametersOperateLog[2].Value = Dep_AccountNumber;
                parametersOperateLog[3].Value = BusinessNO;
                parametersOperateLog[4].Value = "2";//1:存入 2：兑换  3:存转销 4: 提取
                parametersOperateLog[5].Value = GoodID;
                parametersOperateLog[6].Value = UnitName;
                parametersOperateLog[7].Value = GoodPrice;
                parametersOperateLog[8].Value = GoodCount;
                parametersOperateLog[9].Value = VarietyCount;
                double Money_Trade = Convert.ToDouble(GoodPrice) * Convert.ToDouble(GoodCount);
                parametersOperateLog[10].Value = Math.Round(Money_Trade, 2);
                object objBalance = SQLHelper.ExecuteScalar("  SELECT SUM( StorageNumber)  AS StorageNumber  FROM dbo.Dep_StorageInfo WHERE AccountNumber='" + Dep_AccountNumber + "' AND VarietyID=" + VarietyID);
                string Count_Balance = "0";
                if (objBalance != null && objBalance.ToString() != "")
                {
                    Count_Balance = Math.Round(Convert.ToDouble(objBalance)- Convert.ToDouble(VarietyCount), 2).ToString();
                }

                parametersOperateLog[11].Value = Count_Balance;
                parametersOperateLog[12].Value = DateTime.Now;
                parametersOperateLog[13].Value = GoodName;
                parametersOperateLog[14].Value = UnitName;
                #endregion

                #region 数据处理
                using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
                {
                    try
                    {
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);//添加兑换交易记录

                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加兑换日志记录

                        //修改储户的商品结存
                        string strSqlDep_JieCun = " UPDATE dbo.Dep_StorageInfo  SET StorageNumber=StorageNumber-" + VarietyCount + " WHERE ID=" + Dep_SID;
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep_JieCun.ToString());
                        //修改仓库库存信息
                        string strGoodStorage = "  UPDATE dbo.GoodStorage SET numStore=numStore-" + GoodCount + " WHERE GoodID=" + GoodID + " AND WBID=" + WBID;
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
            #endregion 
            context.Session["BNList"] = BusinessNOList;//保存此次所有兑换的参数
            context.Cache.Remove("ExChange");//结账之后将兑换条目的缓存清除
            context.Response.Write("OK");

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
                    string WBID = context.Session["WB_ID"].ToString();//网点编号
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
                    double numJieCun_Total = Convert.ToDouble(dt.Rows[0]["JieCun_Total"]) + Convert.ToDouble(VarietyCount);
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
                    strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName)");
                    strSqlOperateLog.Append(" values (");
                    strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade,@VarietyName,@UnitName)");
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
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50)};
                    parametersOperateLog[0].Value = WBID;
                    parametersOperateLog[1].Value = UserID;
                    parametersOperateLog[2].Value = Dep_AccountNumber;
                    parametersOperateLog[3].Value = BusinessNO;
                    parametersOperateLog[4].Value = "6";//1:存入 2：兑换  3:存转销 4: 提取
                    parametersOperateLog[5].Value = VarietyID;
                    parametersOperateLog[6].Value = UnitName;
                    parametersOperateLog[7].Value = GoodPrice ;
                    parametersOperateLog[8].Value ="-"+ GoodCount;
                    parametersOperateLog[9].Value ="-"+ VarietyCount;
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
                            string strGoodStorage = string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore+{0} WHERE GoodID={1} and WBWareHouseID={2} AND WBID={3}", GoodCount, GoodID, WBWareHouseID, WBID);
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

                context.Session["BNList"] = BusinessNOList;//保存此次所有兑换的参数
                // context.Cache.Remove("ExChange");//结账之后将兑换条目的缓存清除
                context.Response.Write("OK");
            }
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
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();//业务编号
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
            string JieCun_Last = SQLHelper.ExecuteScalar(" SELECT TOP 1 JieCun_Now FROM dbo.StorageSell WHERE Dep_SID="+Dep_SID+" ORDER BY dt_Sell DESC").ToString();//查询上一期最近的结存情况
             double numJieCun_Now = Convert.ToDouble(JieCun_Last)+Convert.ToDouble(VarietyCount);
            string JieCun_Now=numJieCun_Now.ToString();
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
            strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName)");
            strSqlOperateLog.Append(" values (");
            strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade,@VarietyName,@UnitName)");
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
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50)};
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

                    context.Response.Write("OK");
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("Error");
                }
            }
            #endregion 



         
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
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();//业务编号
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
            string JieCun_Last = SQLHelper.ExecuteScalar(" SELECT TOP 1 JieCun_Now FROM dbo.StorageShopping WHERE Dep_SID="+Dep_SID+" ORDER BY dt_Sell DESC").ToString();//查询上一期最近的结存情况
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
            strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName)");
            strSqlOperateLog.Append(" values (");
            strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade,@VarietyName,@UnitName)");
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
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50)};
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

                    context.Response.Write("OK");
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("Error");
                }
            }
            #endregion




        }

        #region 存转销 计算和反算
        /// <summary>
        /// 根据期限类型、产品类型、商品类型 查询兑换比例
        /// </summary>
        /// <param name="context"></param>
        void SellJiSuan(HttpContext context)
        {
            string Dep_SID = context.Request.QueryString["txtDep_SID"].ToString();//存储产品单号（由传递参数获取）
            double StorageNumber = Convert.ToDouble(context.Request.QueryString["VarietyCount"]);//存储数量(此处表示存转销的数量)
            string returnValue = FunJiSuan(context, Dep_SID, StorageNumber);
            context.Response.Write(returnValue);
        }



        /// <summary>
        /// 反算 计算100公斤产品的总金额M1，然后反推X公斤的总金额M2， 则X=（100*M2）/M1
        /// </summary>
        /// <param name="context"></param>
        void SellFanSuan(HttpContext context)
        {
            string Dep_SID = context.Request.QueryString["txtDep_SID"].ToString();//存储产品单号（由传递参数获取）
            double StorageNumber = GetStorageSell(context);//存储数量(此处表示存转销的数量)
            string returnValue= FunJiSuan(context, Dep_SID, StorageNumber);
            context.Response.Write(returnValue);
        }

        //计算产品利息和拼接返回信息
        string FunJiSuan(HttpContext context, string Dep_SID, double StorageNumber)
        {
            string strReturn = "";//返回的json信息
            string strMsg = "";//显示信息
            string model = "";
            if (context.Request.QueryString["model"] != null)
            {
                model = context.Request.QueryString["model"].ToString();
            }
            //计算利息
            double Interest=0;
            //if (model.ToLower() == "shopping")
            //{
            //    Interest = common.GetLiXi(Dep_SID, StorageNumber);//利息
            //}
            //else
            //{
            //     Interest = common.GetLiXi_Sell(Dep_SID, StorageNumber);//利息
            //}
            Interest = common.GetLiXi_Sell(Dep_SID, StorageNumber);//利息
            double MoneyFee = 0; //保管费
            double MoneyVariety = 0;//产品总金额


            bool ISRegular = true;//是否是定期产品
            string TimeName = "";//存期类型
            string numStorageDate = "";//约定存储天数

            DateTime StorageDate = DateTime.Now;//存入日期
            TimeSpan ts = DateTime.Now.Subtract(StorageDate);
            int PricePolicy = 0;//约定存储时间
            double Price_ShiChang = 0;//市场价
            double StorageFee = 0;//保管费率

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select A.StorageDate,B.PricePolicy,B.numStorageDate, B.ISRegular,B.strName as TimeName, A.StorageFee,A.Price_ShiChang");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);
                TimeName = dt.Rows[0]["TimeName"].ToString();
                numStorageDate = dt.Rows[0]["numStorageDate"].ToString();

                StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                ts = DateTime.Now.Subtract(StorageDate);
                PricePolicy = Convert.ToInt32(dt.Rows[0]["PricePolicy"]);//约定存储时间
                Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//市场价
                StorageFee = Convert.ToDouble(dt.Rows[0]["StorageFee"]);//保管费率

                MoneyVariety = StorageNumber * Price_ShiChang;//产品总金额
                if (context.Request.QueryString["model"] == null)//如果是存转销则计算保管费，如果是产品换购则不计算保管费
                {
                    //计算保管费
                    if (ts.TotalDays < Convert.ToDouble(PricePolicy))//不到价格政策的日期
                    {
                        MoneyFee = MoneyVariety * Convert.ToDouble(StorageFee) * 0.01;
                    }
                }
            }
            double numMoney = MoneyVariety + Interest - MoneyFee;//计算得出的存转销总金额
          

            if (Convert.ToBoolean(ISRegular) == true)
            {
                strMsg += "储户类型：<span style='color:blue'>定期,</span>";
            }
            else
            {
                strMsg += "储户类型：<span style='color:blue'>活期,</span>";
            }
            strMsg += "存期：<span style='color:blue'>" + TimeName + ",</span>";
            if (Convert.ToBoolean(ISRegular))
            {
                strMsg += "约定存储时间：<span style='color:blue'>" + numStorageDate + "天,</span>";
                strMsg += "实际存储时间：<span style='color:blue'>" + Convert.ToInt32(Math.Round(ts.TotalDays, 0)).ToString() + "天,</span>";
            }
            strMsg += "产品金额=<span style='color:blue'>" + Math.Round( StorageNumber,2) + "x" + Math.Round(Price_ShiChang, 2).ToString() + "=" + Math.Round(MoneyVariety, 2) + "元,</span>";
            strMsg += "存储利息：<span style='color:blue'>" + Math.Round(Interest, 2) + "元,</span>";
              if (context.Request.QueryString["model"] == null)
              {
            strMsg += "保管费：<span style='color:blue'>" + Math.Round(MoneyFee, 2) + "元,</span>";
        }
            strMsg += "折合现金：<span style='color:blue'>" + Math.Round(numMoney, 2) + "元</span>";


            strReturn += "{\"Count\":\"" + Math.Round(StorageNumber, 2) + "\",\"Money\":\"" + Math.Round(numMoney, 2) + "\",\"LiXi\":\"" + Math.Round(Interest, 2) + "\",\"BGF\":\"" + Math.Round(MoneyFee, 2) + "\",\"Msg\":\"" + strMsg + "\"}";

            return strReturn;
        }

       /// <summary>
        /// 反算 计算100公斤产品的总金额M1，然后反推X公斤的总金额M2， 则X=（100*M2）/M1
       /// </summary>
       /// <param name="context"></param>
       /// <returns></returns>
        double GetStorageSell(HttpContext context) 
        {

            //计算利息
            string Dep_SID = context.Request.QueryString["txtDep_SID"].ToString();//存储产品单号（由传递参数获取）
            double VarietyMoney = Convert.ToDouble(context.Request.QueryString["VarietyMoney"]);//需要存转销的金额（由传递参数获取）
            double StorageSell = 0;//要计算的需要的产品数量
            double StorageNumber = 100;//假设的产品基数

            double Interest = common.GetLiXi_Sell(Dep_SID, StorageNumber);//利息
            double MoneyFee = 0; //保管费
            double MoneyVariety = 0;//产品总金额


            bool ISRegular = true;//是否是定期产品
            string TimeName = "";//存期类型
            string numStorageDate = "";//约定存储天数

            DateTime StorageDate = DateTime.Now;//存入日期
            TimeSpan ts = DateTime.Now.Subtract(StorageDate);
            int PricePolicy = 0;//约定存储时间
            double Price_ShiChang = 0;//市场价
            double StorageFee = 0;//保管费率

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select A.StorageDate,B.PricePolicy,B.numStorageDate, B.ISRegular,B.strName as TimeName, A.StorageFee,A.Price_ShiChang");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);
                TimeName = dt.Rows[0]["TimeName"].ToString();
                numStorageDate = dt.Rows[0]["numStorageDate"].ToString();

                StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                ts = DateTime.Now.Subtract(StorageDate);
                PricePolicy = Convert.ToInt32(dt.Rows[0]["PricePolicy"]);//约定存储时间
                Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//市场价
                StorageFee = Convert.ToDouble(dt.Rows[0]["StorageFee"]);//保管费率

                MoneyVariety = StorageNumber * Price_ShiChang;//产品总金额
                if (context.Request.QueryString["model"] == null)//如果是存转销则计算保管费，如果是产品换购则不计算保管费
                {
                    //计算保管费
                    if (ts.TotalDays < Convert.ToDouble(PricePolicy))//不到价格政策的日期
                    {
                        MoneyFee = MoneyVariety * Convert.ToDouble(StorageFee) * 0.01;
                    }
                }

            }
            double numMoney = MoneyVariety + Interest - MoneyFee;//计算得出的存转销总金额
            StorageSell = (StorageNumber * VarietyMoney) / numMoney;//计算需要的产品数量
            return StorageSell;
        }
        #endregion 


        /// <summary>
        /// 存转销
        /// </summary>
        /// <param name="context"></param>
        void StoreToSell(HttpContext context)
        {
            #region 存转销交易记录
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();//业务编号
            object ApplyID = context.Request.QueryString["ApplyID"];//申请编号
            string Dep_SID = context.Request.Form["txtDep_SID"].ToString();//存储产品编号

           

            double StorageNumber = Convert.ToDouble( context.Request.Form["VarietyCount"].ToString());//存转销数量
            string StorageFee = context.Request.Form["StorageFee"].ToString();//保管费率
            double MoneyFee = Convert.ToDouble( context.Request.Form["txtBGF"]);//保管费
            string CurrentRate = context.Request.Form["CurrentRate"].ToString();//活期利率
            double Interest = Convert.ToDouble( context.Request.Form["txtLiXi"]);//利息

            string DepStorageDate = context.Request.Form["DepStorageDate"].ToString();//实际存储天数
            string Price_ShiChang = context.Request.Form["txtPrice_ShiChang"].ToString();//商品存入价格
            //double VarietyMoney = Convert.ToDouble(context.Request.Form["VarietyMoney"]);//商品价值金额
            //double Money_Earn = VarietyMoney + Interest - MoneyFee;
         
            double Money_Earn = Convert.ToDouble(context.Request.Form["VarietyMoney"]);
            double VarietyMoney = Money_Earn - Interest + MoneyFee; //商品价值金额
            double EarningRate = Interest / VarietyMoney;//盈利率
            string Dep_AccountNumber = "";//储户账号
            string Dep_Name = "";//储户名
            //获取储户信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("  SELECT A.ID,B.AccountNumber AS Dep_AccountNumber ,B.strName AS Dep_Name,StorageNumberRaw");
            strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlStorage.Append("  WHERE A.ID=" + Dep_SID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            if (dtStorage != null && dtStorage.Rows.Count != 0)
            {
                Dep_AccountNumber = dtStorage.Rows[0]["Dep_AccountNumber"].ToString();
                Dep_Name = dtStorage.Rows[0]["Dep_Name"].ToString();

            }
            string strGUID = Fun.getGUID();//防伪码
            string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + Dep_AccountNumber + BusinessNO;

            //获取当次的存储产品信息
            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "存转销";//兑换业务

            //获取此产品的上一次的结存信息
            double JieCun_Last = Convert.ToDouble(context.Request.Form["txtJieCun"]);//上次结存
            double JieCun_Now = JieCun_Last - StorageNumber;//现在结存


            string VarietyID = context.Request.Form["txtVarietyID"].ToString();
            string TimeID = context.Request.Form["txtTimeID"].ToString();
            string TimeName = SQLHelper.ExecuteScalar(" SELECT TOP 1 strName FROM dbo.StorageTime WHERE ID=" + TimeID).ToString();
            
            string UnitName = "";
            string VarietyName = "";
            //查询商品信息
            StringBuilder strSqlVariety = new StringBuilder();
            strSqlVariety.Append("  SELECT A.ID,A.strName,B.strName AS Unit");
            strSqlVariety.Append("  FROM dbo.StorageVariety A INNER JOIN dbo.BD_MeasuringUnit B ON A.MeasuringUnitID=B.ID");
            strSqlVariety.Append("  WHERE A.ID=" + VarietyID);
            strSqlVariety.Append("  ");
            DataTable dtVariety = SQLHelper.ExecuteDataTable(strSqlVariety.ToString());
            if (dtVariety != null && dtVariety.Rows.Count != 0)
            {
                UnitName = dtVariety.Rows[0]["Unit"].ToString();
                VarietyName = dtVariety.Rows[0]["strName"].ToString();
            }

            //写入存转销记录
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
            parameters[3].Value = Dep_SID;
            parameters[4].Value = Dep_AccountNumber;
            parameters[5].Value = Dep_Name;
            parameters[6].Value = WBID;
            parameters[7].Value = UserID;
            parameters[8].Value = BusinessName;
            parameters[9].Value = UnitName;
            parameters[10].Value = VarietyID;
            parameters[11].Value = VarietyName;
            parameters[12].Value = StorageNumber;
            parameters[13].Value = Math.Round(VarietyMoney, 2);
            parameters[14].Value = Math.Round(Interest, 2);
            parameters[15].Value = DepStorageDate;
            parameters[16].Value = CurrentRate;
            parameters[17].Value = Math.Round( EarningRate,4);//盈利率(暂用)
            parameters[18].Value = StorageFee;
            parameters[19].Value = Math.Round( MoneyFee,2);
            parameters[20].Value =Price_ShiChang;
            parameters[21].Value = Money_Earn;
            parameters[22].Value = DateTime.Now;
            parameters[23].Value = JieCun_Last;
            parameters[24].Value = JieCun_Now;
            parameters[25].Value = 0;//是否退还
#endregion 
           
           

            #region 存转销日志记录
            //添加交易记录

            string Price = context.Request.Form["txtVarietyPrice"].ToString();//价格
            string Count_Trade = StorageNumber.ToString();//存储数量
            string Money_Trade = Math.Round(Money_Earn, 2).ToString();//存入的时候没有交易量
            string Count_Balance = JieCun_Now.ToString();//当前产品结存
            //查找当前用户的当前产品的总的结存
            object objBalance = SQLHelper.ExecuteScalar("  SELECT SUM( StorageNumber)  AS StorageNumber  FROM dbo.Dep_StorageInfo WHERE AccountNumber='" + Dep_AccountNumber + "' AND VarietyID=" + VarietyID);
            if (objBalance != null && objBalance.ToString() != "")
            {
               // Count_Balance = objBalance.ToString();
                Count_Balance = (Convert.ToDouble(objBalance) - StorageNumber).ToString();
            }

            string UnitID = "公斤";
            object objUnitID = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.BD_MeasuringUnit WHERE ID=( SELECT MeasuringUnitID FROM dbo.StorageVariety WHERE ID=" + VarietyID + ")");//获取计价单位
            if (objUnitID != null && objUnitID.ToString() != "")
            {
                UnitID = objUnitID.ToString();
            }

            StringBuilder strSqlOperateLog = new StringBuilder();
            strSqlOperateLog.Append("insert into [Dep_OperateLog] (");
            strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade)");
            strSqlOperateLog.Append(" values (");
            strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade)");
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
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parametersOperateLog[0].Value = WBID;
            parametersOperateLog[1].Value = UserID;
            parametersOperateLog[2].Value = Dep_AccountNumber;
            parametersOperateLog[3].Value = BusinessNO;
            parametersOperateLog[4].Value = "3";//1:存入 2：兑换  3:存转销 4: 提取
            parametersOperateLog[5].Value =TimeName+ VarietyName;
            parametersOperateLog[6].Value = UnitID;
            parametersOperateLog[7].Value = Price;
            parametersOperateLog[8].Value = Count_Trade;
            parametersOperateLog[9].Value = Count_Trade;
            parametersOperateLog[10].Value = Money_Trade;
            parametersOperateLog[11].Value = Count_Balance;
            parametersOperateLog[12].Value = DateTime.Now;
            #endregion

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlInsert.ToString(), parameters);//添加存转销交易记录
                    //修改储户的商品结存
                    string strSqlDep_JieCun = " UPDATE dbo.Dep_StorageInfo  SET StorageNumber=StorageNumber-" + StorageNumber + " WHERE ID=" + Dep_SID;
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep_JieCun.ToString());//储户结存修改
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加存转销日志记录

                    //取消存转销申请记录
                    if (ApplyID != null)
                    {
                        string strSqldelete = " DELETE FROM dbo.StorageSellApply WHERE ID=" + ApplyID.ToString();
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqldelete.ToString());
                    }
                    
                    tran.Commit();

                    context.Response.Write("OK");
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("Error");
                }
            }
            #endregion 


        }

        /// <summary>
        /// 产品换购
        /// </summary>
        /// <param name="context"></param>
        void StorageShopping(HttpContext context)
        {
            #region 存转销交易记录
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();//业务编号
          
            string Dep_SID = context.Request.Form["txtDep_SID"].ToString();//存储产品编号



            double StorageNumber = Convert.ToDouble(context.Request.Form["VarietyCount"].ToString());//存转销数量
            string StorageFee = context.Request.Form["StorageFee"].ToString();//保管费率
            double MoneyFee = Convert.ToDouble(context.Request.Form["txtBGF"]);//保管费
            string CurrentRate = context.Request.Form["CurrentRate"].ToString();//活期利率
            double Interest = Convert.ToDouble(context.Request.Form["txtLiXi"]);//利息
            double VarietyMoney = Convert.ToDouble(context.Request.Form["VarietyMoney"]);//商品价值金额
            string DepStorageDate = context.Request.Form["DepStorageDate"].ToString();//实际存储天数
            string Price_ShiChang = context.Request.Form["txtPrice_ShiChang"].ToString();//商品存入价格
            double Money_Earn = VarietyMoney + Interest - MoneyFee;
            double EarningRate = Interest / VarietyMoney;//盈利率
            string Dep_AccountNumber = "";//储户账号
            string Dep_Name = "";//储户名
            //获取储户信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("  SELECT A.ID,B.AccountNumber AS Dep_AccountNumber ,B.strName AS Dep_Name,StorageNumberRaw");
            strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlStorage.Append("  WHERE A.ID=" + Dep_SID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            if (dtStorage != null && dtStorage.Rows.Count != 0)
            {
                Dep_AccountNumber = dtStorage.Rows[0]["Dep_AccountNumber"].ToString();
                Dep_Name = dtStorage.Rows[0]["Dep_Name"].ToString();

            }
            string strGUID = Fun.getGUID();//防伪码
            string SerialNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + Dep_AccountNumber + BusinessNO;

            //获取当次的存储产品信息
            string WBID = context.Session["WB_ID"].ToString();//当前网点ID
            string UserID = context.Session["ID"].ToString();//当前营业员ID
            string BusinessName = "产品换购";//兑换业务

            //获取此产品的上一次的结存信息
            double JieCun_Last = Convert.ToDouble(context.Request.Form["txtJieCun"]);//上次结存
            double JieCun_Now = JieCun_Last - StorageNumber;//现在结存


            string VarietyID = context.Request.Form["txtVarietyID"].ToString();
            string TimeID = context.Request.Form["txtTimeID"].ToString();
            string TimeName = SQLHelper.ExecuteScalar(" SELECT TOP 1 strName FROM dbo.StorageTime WHERE ID=" + TimeID).ToString();

            string UnitName = "";
            string VarietyName = "";
            //查询商品信息
            StringBuilder strSqlVariety = new StringBuilder();
            strSqlVariety.Append("  SELECT A.ID,A.strName,B.strName AS Unit");
            strSqlVariety.Append("  FROM dbo.StorageVariety A INNER JOIN dbo.BD_MeasuringUnit B ON A.MeasuringUnitID=B.ID");
            strSqlVariety.Append("  WHERE A.ID=" + VarietyID);
            strSqlVariety.Append("  ");
            DataTable dtVariety = SQLHelper.ExecuteDataTable(strSqlVariety.ToString());
            if (dtVariety != null && dtVariety.Rows.Count != 0)
            {
                UnitName = dtVariety.Rows[0]["Unit"].ToString();
                VarietyName = dtVariety.Rows[0]["strName"].ToString();
            }

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
            parameters[3].Value = Dep_SID;
            parameters[4].Value = Dep_AccountNumber;
            parameters[5].Value = Dep_Name;
            parameters[6].Value = WBID;
            parameters[7].Value = UserID;
            parameters[8].Value = BusinessName;
            parameters[9].Value = UnitName;
            parameters[10].Value = VarietyID;
            parameters[11].Value = VarietyName;
            parameters[12].Value = StorageNumber;
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
            parameters[24].Value = JieCun_Now;
            parameters[25].Value = 0;//是否退还
            #endregion



            #region 存转销日志记录
            //添加交易记录

            string Price = context.Request.Form["txtVarietyPrice"].ToString();//价格
            string Count_Trade = StorageNumber.ToString();//存储数量
            string Money_Trade = Math.Round(Money_Earn, 2).ToString();//存入的时候没有交易量
            string Count_Balance = JieCun_Now.ToString();//当前产品结存
            //查找当前用户的当前产品的总的结存
            object objBalance = SQLHelper.ExecuteScalar("  SELECT SUM( StorageNumber)  AS StorageNumber  FROM dbo.Dep_StorageInfo WHERE AccountNumber='" + Dep_AccountNumber + "' AND VarietyID=" + VarietyID);
            if (objBalance != null && objBalance.ToString() != "")
            {
                // Count_Balance = objBalance.ToString();
                Count_Balance = (Convert.ToDouble(objBalance) - StorageNumber).ToString();
            }

            string UnitID = "公斤";
            object objUnitID = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.BD_MeasuringUnit WHERE ID=( SELECT MeasuringUnitID FROM dbo.StorageVariety WHERE ID=" + VarietyID + ")");//获取计价单位
            if (objUnitID != null && objUnitID.ToString() != "")
            {
                UnitID = objUnitID.ToString();
            }

            StringBuilder strSqlOperateLog = new StringBuilder();
            strSqlOperateLog.Append("insert into [Dep_OperateLog] (");
            strSqlOperateLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade)");
            strSqlOperateLog.Append(" values (");
            strSqlOperateLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade)");
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
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parametersOperateLog[0].Value = WBID;
            parametersOperateLog[1].Value = UserID;
            parametersOperateLog[2].Value = Dep_AccountNumber;
            parametersOperateLog[3].Value = BusinessNO;
            parametersOperateLog[4].Value = "9";//1:存入 2：兑换  3:存转销 4: 提取
            parametersOperateLog[5].Value = TimeName + VarietyName;
            parametersOperateLog[6].Value = UnitID;
            parametersOperateLog[7].Value = Price;
            parametersOperateLog[8].Value = VarietyMoney;//商品换购写入商品总价值金额
            parametersOperateLog[9].Value = Count_Trade;
            parametersOperateLog[10].Value = Money_Trade;
            parametersOperateLog[11].Value = Count_Balance;
            parametersOperateLog[12].Value = DateTime.Now;
            #endregion

            #region 数据处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlInsert.ToString(), parameters);//添加存转销交易记录
                    //修改储户的商品结存
                    string strSqlDep_JieCun = " UPDATE dbo.Dep_StorageInfo  SET StorageNumber=StorageNumber-" + StorageNumber + " WHERE ID=" + Dep_SID;
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep_JieCun.ToString());//储户结存修改
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加存转销日志记录

                   

                    tran.Commit();

                    context.Response.Write("OK");
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("Error");
                }
            }
            #endregion


        }


        /// <summary>
        /// 申请存转销
        /// </summary>
        /// <param name="context"></param>
        void Add_SellApply(HttpContext context)
        {
            
            string Dep_SID = context.Request.Form["txtDep_SID"].ToString();//存储产品编号
            string VarietyCount = context.Request.Form["VarietyCount"].ToString();//申请数量

          //  var returnValue = FunJiSuan(context, Dep_SID, Convert.ToDouble( VarietyCount));
          //  DataTable dtSell = JsonHelper.JsonToDataTable(returnValue);
           // string ApplyMoney = dtSell.Rows[0]["Money"].ToString();
            string ApplyMoney = context.Request.Form["VarietyMoney"].ToString();//申请金额

            StringBuilder strSqlVariety = new StringBuilder();
            strSqlVariety.Append(" SELECT B.AccountNumber AS Dep_AccountNumber,B.strName AS Dep_Name,A.VarietyID,C.strName AS VarietyName,A.Price_ShiChang AS ApplyPrice,E.strName as UnitName");
            strSqlVariety.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlVariety.Append(" INNER JOIN  dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSqlVariety.Append(" INNER JOIN dbo.BD_MeasuringUnit E ON C.MeasuringUnitID=E.ID");
            strSqlVariety.Append(" WHERE A.ID="+Dep_SID);

            DataTable dtVariety = SQLHelper.ExecuteDataTable(strSqlVariety.ToString());
            if (dtVariety != null && dtVariety.Rows.Count != 0)
            {
                string Dep_AccountNumber = dtVariety.Rows[0]["Dep_AccountNumber"].ToString();
                string Dep_Name = dtVariety.Rows[0]["Dep_Name"].ToString();
                string VarietyID = dtVariety.Rows[0]["VarietyID"].ToString();
                string VarietyName = dtVariety.Rows[0]["VarietyName"].ToString();
                string ApplyPrice = dtVariety.Rows[0]["ApplyPrice"].ToString();
                string UnitName = dtVariety.Rows[0]["UnitName"].ToString();


                string WBID = context.Session["WB_ID"].ToString();
                string UserID = context.Session["ID"].ToString();


                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into [StorageSellApply] (");
                strSql.Append("WBID,UserID,Dep_AccountNumber,Dep_Name,ApplyType,VarietyID,UnitName,VarietyName,VarietyCount,ApplyPrice,ApplyMoney,ApplyDate,ApplyState,ApproverID,ApproveDate,Dep_SID)");
                strSql.Append(" values (");
                strSql.Append("@WBID,@UserID,@Dep_AccountNumber,@Dep_Name,@ApplyType,@VarietyID,@UnitName,@VarietyName,@VarietyCount,@ApplyPrice,@ApplyMoney,@ApplyDate,@ApplyState,@ApproverID,@ApproveDate,@Dep_SID)");
                strSql.Append(";select @@IDENTITY");
                SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@ApplyType", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyName", SqlDbType.NVarChar,50),
					new SqlParameter("@VarietyCount", SqlDbType.Decimal,9),
					new SqlParameter("@ApplyPrice", SqlDbType.Decimal,9),
					new SqlParameter("@ApplyMoney", SqlDbType.Decimal,9),
					new SqlParameter("@ApplyDate", SqlDbType.DateTime),
					new SqlParameter("@ApplyState", SqlDbType.Int,4),
					new SqlParameter("@ApproverID", SqlDbType.Int,4),
					new SqlParameter("@ApproveDate", SqlDbType.DateTime),
                                            new SqlParameter("@Dep_SID", SqlDbType.Int,4)};
                parameters[0].Value = WBID;
                parameters[1].Value = UserID;
                parameters[2].Value = Dep_AccountNumber;
                parameters[3].Value = Dep_Name;
                parameters[4].Value = 1;//1;存转销  2：提取原粮
                parameters[5].Value = VarietyID;
                parameters[6].Value = UnitName;
                parameters[7].Value = VarietyName;
                parameters[8].Value = VarietyCount;
                parameters[9].Value = ApplyPrice;
                parameters[10].Value = ApplyMoney;
                parameters[11].Value = DateTime.Now;//申请日期
                parameters[12].Value = 0;//申请状态
                parameters[13].Value = 0;//审核人
                parameters[14].Value = DateTime.Now;//批准日期
                parameters[15].Value = Dep_SID;



              

                //添加事务处理
                using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
                {
                    try
                    {
                      
                      object objApplyID=  SQLHelper.ExecuteScalar(tran, CommandType.Text, strSql.ToString(), parameters);

                      StringBuilder strSql_Advice = new StringBuilder();
                      strSql_Advice.Append("insert into [StorageSellApply_Advice] (");
                      strSql_Advice.Append("StorageSellApplyID,WBID,UserID,Dep_AccountNumber,ApplyType,ApplyState,ISRequestRead,ISResponseRead)");
                      strSql_Advice.Append(" values (");
                      strSql_Advice.Append("@StorageSellApplyID,@WBID,@UserID,@Dep_AccountNumber,@ApplyType,@ApplyState,@ISRequestRead,@ISResponseRead)");
                      strSql_Advice.Append(";select @@IDENTITY");
                      SqlParameter[] parameters_Advice = {
                    new SqlParameter("@StorageSellApplyID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@ApplyType", SqlDbType.Int,4),
					new SqlParameter("@ApplyState", SqlDbType.Int,4),
					new SqlParameter("@ISRequestRead", SqlDbType.Int,4),
					new SqlParameter("@ISResponseRead", SqlDbType.Int,4)};
                      parameters_Advice[0].Value = objApplyID;
                      parameters_Advice[1].Value = WBID;
                      parameters_Advice[2].Value = UserID;
                      parameters_Advice[3].Value = Dep_AccountNumber;
                      parameters_Advice[4].Value = 1;//1;存转销  2：提取原粮
                      parameters_Advice[5].Value = 0;
                      parameters_Advice[6].Value = 0;
                      parameters_Advice[7].Value = 0;

                      SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql_Advice.ToString(), parameters_Advice);
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
            else {
                context.Response.Write("Error");
            }

            
        }


        /// <summary>
        /// 审批申请
        /// </summary>
        /// <param name="context"></param>
        void ApprovalApply(HttpContext context)
        {
            string Apply = context.Request.QueryString["Apply"].ToString();
            int ApplyState = 1;//通过申请
            if (Apply == "0")
            {
                ApplyState = 2;//不通过申请 
            }
            string ID = context.Request.QueryString["ID"].ToString();
            string strSql = " UPDATE dbo.StorageSellApply SET ApplyState=" + ApplyState + " WHERE ID=" + ID;

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
        /// 获取存转销审批数量
        /// </summary>
        /// <param name="context"></param>
        void GetApplyCount(HttpContext context)
        {
            string ID = context.Request.QueryString["ApplyID"].ToString();

            string strSql = "   SELECT TOP 1 VarietyCount FROM dbo.StorageSellApply WHERE ID=" + ID;
            object obj = SQLHelper.ExecuteScalar(strSql);
            if (obj!=null)
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 获取存转销审批数量
        /// </summary>
        /// <param name="context"></param>
        void GetApplyInfo(HttpContext context)
        {
            string ID = context.Request.QueryString["ApplyID"].ToString();

            string strSql = "   SELECT TOP 1 * FROM dbo.StorageSellApply WHERE ID=" + ID;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null&&dt.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetSellApply_Advice_Request(HttpContext context)
        {

            string strSql = "   SELECT * FROM StorageSellApply_Advice WHERE ApplyType=1 AND ApplyState=0 AND ISRequestRead=0";
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetSellApply_Advice_Response(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string strSql = string.Format(" SELECT A.* FROM StorageSellApply_Advice A INNER JOIN dbo.StorageSellApply S ON A.StorageSellApplyID=S.ID  WHERE A.ApplyType=1 AND S.ApplyState IN (1,2) AND ISResponseRead=0 AND A.WBID={0}",WBID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        void UpdateAdviceState_Response(HttpContext context)
        {
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string strSql = string.Format("UPDATE StorageSellApply_Advice SET ISResponseRead=1 WHERE Dep_AccountNumber='{0}'",AccountNumber);
            int num = SQLHelper.ExecuteNonQuery(strSql);
            if (num > 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateAdviceState_Request(HttpContext context)
        {
            string strSql = "UPDATE StorageSellApply_Advice SET ISRequestRead=1 ";
            int num = SQLHelper.ExecuteNonQuery(strSql);
            if (num>0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void JieXi(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = context.Session["ID"].ToString();
            string BusinessNO = context.Request.Form["BusinessNO"].ToString();//业务编号
            string AccountNumber = context.Request.Form["QAccountNumber"].ToString();//储户账号
            string Dep_Name = context.Request.Form["Dep_Name"].ToString();//储户账号
            
            string Dep_SID = context.Request.Form["txtDep_SID"].ToString();//储户存储业务
            string JiexiType = context.Request.Form["InterestType"].ToString();//转存类型
            string numInterest = context.Request.Form["numInterest"].ToString();//转存类型
            string VarietyID = context.Request.Form["txtVarietyID"].ToString();//存储产品
            string TypeID = context.Request.Form["txtTypeID"].ToString();//存储类型
            string TimeID = context.Request.Form["txtTypeID"].ToString();//存期

        


            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.InterestType,InterestDate,StorageDate,B.numStorageDate,WeighNo,StorageNumber,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong ");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                return;//返回
            }
            int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
            int StorageNumber = Convert.ToInt32(dt.Rows[0]["StorageNumber"]);//存储数量
            DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
            DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);
            TimeSpan tsStorage = DateTime.Now.Subtract(StorageDate);
            TimeSpan tsInterest = DateTime.Now.Subtract(InterestDate);
            int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
            double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//活期利率
            double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//市场价
            double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价


             double Price_ShiChangNow=Price_ShiChang;//当前市场的价格
             double Price_DaoQiNow = Price_DaoQi;//当前的到期价格
             //计算当前的市场价格
             StringBuilder strSqlShiChang = new StringBuilder();
             strSqlShiChang.Append(" SELECT B.Price_ShiChang,B.Price_DaoQi,B.EarningRate");
             strSqlShiChang.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageRate B ON A.StorageRateID=B.ID");
             strSqlShiChang.Append(" WHERE A.ID=" + Dep_SID);
             DataTable dtShiChang = SQLHelper.ExecuteDataTable(strSqlShiChang.ToString());
             if (dtShiChang != null && dtShiChang.Rows.Count != 0)
             {
                 Price_DaoQiNow = Convert.ToDouble(dtShiChang.Rows[0]["Price_DaoQi"]);
                 Price_ShiChangNow = Convert.ToDouble(dtShiChang.Rows[0]["Price_ShiChang"]);
                 int EarningRate = Convert.ToInt32(dtShiChang.Rows[0]["EarningRate"]);
             }

            DateTime newStorageDate =StorageDate;//存储时间
            DateTime newInterestDate = InterestDate;//续存时间
            double newPrice_ShiChang = Price_ShiChang;//市场价
            double newPrice_DaoQi = Price_DaoQi;//到期价
         
            switch (JiexiType)
            {
                case "1": //仅结息方式
                    switch (InterestType)
                    {
                        case 1://按月结息方式
                              newInterestDate = DateTime.Now;//仅改变结息时间
                            break;
                        case 2://按市场价结息方式
                            break;
                        case 3: //按到期价格结息方式 
                            break;
                    }
                    break;
                case "2": //结息后续存方式
                    switch (InterestType)
                    {
                        case 1://按月结息方式
                            newStorageDate = DateTime.Now;
                            newInterestDate = DateTime.Now;
                          
                            break;
                        case 2://按市场价结息方式(随行就市)
                             newStorageDate = DateTime.Now;
                            newInterestDate = DateTime.Now;
                         
                            break;
                        case 3: //按到期价格结息方式 
                             newStorageDate = DateTime.Now;
                            newInterestDate = DateTime.Now;
                            break;
                    }
                    break;
                case "3"://不结息直接续存方式
                    switch (InterestType)
                    {
                        case 1://按月结息方式
                             newStorageDate = DateTime.Now;
                            newInterestDate = DateTime.Now;
                              newPrice_ShiChang = Price_ShiChang + CurrentRate * tsInterest.TotalDays / (double)30;//新的存入价格
                            break;
                        case 2://按市场价结息方式
                             newStorageDate = DateTime.Now;
                            newInterestDate = DateTime.Now;
                            newPrice_ShiChang = Price_ShiChangNow;
                            break;
                        case 3: //按到期价格结息方式 
                             newStorageDate = DateTime.Now;
                            newInterestDate = DateTime.Now;
                            newPrice_DaoQi = Price_DaoQiNow;
                            break;
                    }
                    break;
            }

            StringBuilder strSqlUpdate = new StringBuilder();
            strSqlUpdate.Append("   UPDATE dbo.Dep_StorageInfo ");
            strSqlUpdate.Append("  SET StorageDate='"+newStorageDate+"',");
            strSqlUpdate.Append("   InterestDate='"+newInterestDate+"',");
            strSqlUpdate.Append("   Price_ShiChang='"+newPrice_ShiChang+"',");
            strSqlUpdate.Append("   Price_DaoQi='"+newPrice_DaoQi+"'");
            strSqlUpdate.Append("  WHERE ID="+Dep_SID);
            strSqlUpdate.Append("  ");

          
            //添加储户结息记录
            StringBuilder strSqlInterest = new StringBuilder();
            strSqlInterest.Append("insert into [StorageInterest] (");
            strSqlInterest.Append("BusinessNO,Dep_AccountNumber,Dep_Name,WBID,UserID,BusinessName,Dep_SID,VarietyID,JieCun,InterestType,TypeID,TimeID,dt_Interest,numInterest)");
            strSqlInterest.Append(" values (");
            strSqlInterest.Append("@BusinessNO,@Dep_AccountNumber,@Dep_Name,@WBID,@UserID,@BusinessName,@Dep_SID,@VarietyID,@JieCun,@InterestType,@TypeID,@TimeID,@dt_Interest,@numInterest)");
            strSqlInterest.Append(";select @@IDENTITY");
            SqlParameter[] parametersInterest = {
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_Name", SqlDbType.NVarChar,50),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@BusinessName", SqlDbType.NVarChar,50),
					new SqlParameter("@Dep_SID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@JieCun", SqlDbType.Decimal,9),
					new SqlParameter("@InterestType", SqlDbType.Int,4),
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@dt_Interest", SqlDbType.DateTime),
					new SqlParameter("@numInterest", SqlDbType.Decimal,9)};
            parametersInterest[0].Value = BusinessNO;
            parametersInterest[1].Value = AccountNumber;
            parametersInterest[2].Value = Dep_Name;
            parametersInterest[3].Value = WBID;
            parametersInterest[4].Value = UserID;
            parametersInterest[5].Value = "4";
            parametersInterest[6].Value = Dep_SID;
            parametersInterest[7].Value = VarietyID;
            parametersInterest[8].Value = 0;
            parametersInterest[9].Value = JiexiType;
            parametersInterest[10].Value = TypeID;
            parametersInterest[11].Value = TimeID;
            parametersInterest[12].Value = DateTime.Now;
            parametersInterest[13].Value = numInterest;




            //添加储户操作记录

            //添加交易记录
            string Count_Balance = "0";//当前产品结存
            //查找当前用户的当前产品的总的结存
            object objBalance = SQLHelper.ExecuteScalar("  SELECT SUM( StorageNumber)  AS StorageNumber  FROM dbo.Dep_StorageInfo WHERE AccountNumber='" + AccountNumber + "' AND VarietyID=" + VarietyID);
            if (objBalance != null && objBalance.ToString() != "")
            {
                Count_Balance = objBalance.ToString();
            }

 


            string UnitID = "公斤";
            object objUnitID = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.BD_MeasuringUnit WHERE ID=( SELECT MeasuringUnitID FROM dbo.StorageVariety WHERE ID=" + VarietyID + ")");//获取计价单位
            string VarietyName = SQLHelper.ExecuteScalar("  SELECT strName FROM dbo.StorageVariety WHERE ID=" + VarietyID).ToString();
            if (objUnitID != null && objUnitID.ToString() != "")
            {
                UnitID = objUnitID.ToString();
            }


            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("insert into [Dep_OperateLog] (");
            strSqlLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade)");
            strSqlLog.Append(" values (");
            strSqlLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade)");
            strSqlLog.Append(";select @@IDENTITY");
            SqlParameter[] parametersLog = {
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
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parametersLog[0].Value = WBID;
            parametersLog[1].Value = UserID;
            parametersLog[2].Value = AccountNumber;
            parametersLog[3].Value = BusinessNO;
            parametersLog[4].Value = "11";//11:结息
            parametersLog[5].Value = VarietyName;
            parametersLog[6].Value = UnitID;
            parametersLog[7].Value = newPrice_ShiChang;
            parametersLog[8].Value = StorageNumber;
            parametersLog[9].Value = StorageNumber;
            double Money_Trade = Math.Round(StorageNumber * Price_ShiChang, 2);
            parametersLog[10].Value = Money_Trade;
            parametersLog[11].Value = Count_Balance;
            parametersLog[12].Value = DateTime.Now;
            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlUpdate.ToString());
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlInterest.ToString(), parametersInterest);
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlLog.ToString(), parametersLog);//添加结息日志记录
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
        

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}