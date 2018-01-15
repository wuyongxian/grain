using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;

namespace Web.Admin.Good
{
    /// <summary>
    /// good 的摘要说明
    /// </summary>
    public class good : IHttpHandler, IRequiresSessionState
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
                    case "Get_WBGoodCategory": Get_WBGoodCategory(context); break;
                    case "Get_WBGoodCategoryALL": Get_WBGoodCategoryALL(context); break;
                    case "Get_WBGoodCategoryByCode": Get_WBGoodCategoryByCode(context); break;
                    case "Get_WBSupplier": Get_WBSupplier(context); break;
                    case "Get_WBWareHouse": Get_WBWareHouse(context); break;
                    case "Get_BD_PackingSpec": Get_BD_PackingSpec(context); break;
                    case "Get_BD_MeasuringUnit": Get_BD_MeasuringUnit(context); break;

                    case "GetByID_Supplier": GetByID_Supplier(context); break;
                    case "Add_Supplier": Add_Supplier(context); break;
                    case "Update_Supplier": Update_Supplier(context); break;
                    case "DeleteByID_Supplier": DeleteByID_Supplier(context); break;

                    case "Get_Good": Get_Good(context); break;
                    case "Get_IntegralGood": Get_IntegralGood(context); break;
                    case "GetWBGoodByWHID": GetWBGoodByWHID(context); break;
                    case "GetGoodByWBID": GetGoodByWBID(context); break;
                    case "GetByID_Good": GetByID_Good(context); break;
                    case "Add_Good": Add_Good(context); break;
                    case "Update_Good": Update_Good(context); break;
                    case "DeleteByID_Good": DeleteByID_Good(context); break;



                    case "GetGoodStorageByGoodID": GetGoodStorageByGoodID(context); break;
                    case "GetWBGoodStorage": GetWBGoodStorage(context); break;
                    case "GetByID_GoodStorage": GetByID_GoodStorage(context); break;
                    case "Add_GoodStorage": Add_GoodStorage(context); break;
                    case "Update_GoodStorage": Update_GoodStorage(context); break;//更改仓库信息
                    case "UpdateGoodQuantity": UpdateGoodQuantity(context); break;//修改网点的库存数量
                    case "Update_numStore": Update_numStore(context); break;//更改库存信息
                        


                    case "GetByID_GoodSupply": GetByID_GoodSupply(context); break;
                    case "Add_GoodSupply": Add_GoodSupply(context); break;
                    case "Update_GoodSupply": Update_GoodSupply(context); break;
                    case "DeleteByID_GoodSupply": DeleteByID_GoodSupply(context); break;



                    case "Get_GoodSupply": Get_GoodSupply(context); break;
                    case "GetGoodSupplyByWBID": GetGoodSupplyByWBID(context); break;

                    case "GetGoodSupplyStorageByID": GetGoodSupplyStorageByID(context); break;//社员商品进货信息
                    case "Add_GoodSupplyStorage": Add_GoodSupplyStorage(context); break;
                    case "Update_GoodSupplyStorage": Update_GoodSupplyStorage(context); break;//更改仓库信息
                    case "UpdateGoodSupplyQuantity": UpdateGoodSupplyQuantity(context); break;

                    case "ShowExchangeUnitByGoodID": ShowExchangeUnitByGoodID(context); break;//商品兑换，显示兑换产品信息
                        

                    case "Add_GoodStock": Add_GoodStock(context); break;//添加总部进货信息

                    case "Add_GoodAllocateWB": Add_GoodAllocateWB(context); break;//网点之间调货
                    case "Add_GoodStockWB": Add_GoodStockWB(context); break;//添加网点进货信息

                    case "Update_GoodStockWB": Update_GoodStockWB(context); break;//修改库存数量和进货数量
                    case "Update_GoodStockWarn": Update_GoodStockWarn(context); break;//修改网点进货提醒状态
                    case "Get_GoodStockWarn": Get_GoodStockWarn(context); break;//获取网i单进货提醒
                  
                    case "GetHQStorage_Dep": GetHQStorage_Dep(context); break;//获取总部库存

                    case "Add_GoodSupplyStockApply": Add_GoodSupplyStockApply(context); break;//社员商品进货申请
                    case "Add_GoodSupplyStock": Add_GoodSupplyStock(context); break;//添加社员商品进货信息
                    case "Add_GoodSupplyStockWB": Add_GoodSupplyStockWB(context); break;
                    case "Update_GoodSupplyStockWB": Update_GoodSupplyStockWB(context); break;//更新网点的库存信息
                    case "GetHQStorage": GetHQStorage(context); break;//获取总部库存
                    case "GetGoodSupplyStorage": GetGoodSupplyStorage(context); break;//获取网店库存
                        
                        
                }
            }

        }

        /// <summary>
        /// 获取全部商品分类
        /// </summary>
        /// <param name="context"></param>
        void Get_WBGoodCategoryALL(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,strName");
            strSql.Append("   FROM dbo.WBGoodCategory");
            strSql.Append("   ORDER BY ISDefault DESC,numSort ASC");
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

        void Get_WBGoodCategory(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,strName");
            strSql.Append("   FROM dbo.WBGoodCategory");
            strSql.Append("   where ISCustom!=1");
            strSql.Append("   ORDER BY ISDefault DESC,numSort ASC");
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

        void Get_WBGoodCategoryByCode(HttpContext context)
        {
            string code = context.Request.QueryString["code"].ToString();
            if (code.IndexOf("NY") == 0)
            {
                code = "NY";
            }
            else if (code.IndexOf("HF") == 0)
            {
                code = "HF";
            }
            else if (code.IndexOf("ZHZ") == 0)
            {
                code = "ZHZ";
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,strName");
            strSql.Append("   FROM dbo.WBGoodCategory");
            strSql.Append("   where strCode='" + code + "'");
            strSql.Append("   ORDER BY ISDefault DESC,numSort ASC");
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
        /// 获取供应商分类
        /// </summary>
        /// <param name="context"></param>
        void Get_WBSupplier(HttpContext context)
        {
            string WB_ID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT ID,strName");
            strSql.Append("   FROM dbo.WBSupplier");
            strSql.Append("   where WB_ID="+WB_ID);
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
        /// 获取仓库
        /// </summary>
        /// <param name="context"></param>
        void Get_WBWareHouse(HttpContext context)
        {
            string WB_ID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT ID,strName");
            strSql.Append("   FROM dbo.WBWareHouse");
            strSql.Append("   where WB_ID="+WB_ID);
            strSql.Append("   ORDER BY ISDefault DESC,numSort ASC");
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
        /// 获取包装规格
        /// </summary>
        /// <param name="context"></param>
        void Get_BD_PackingSpec(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,strName");
            strSql.Append("   FROM dbo.BD_PackingSpec");
            strSql.Append("   ORDER BY ISDefault DESC,numSort ASC");
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
        /// 获取产品尺寸
        /// </summary>
        /// <param name="context"></param>
        void Get_BD_MeasuringUnit(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,strName");
            strSql.Append("   FROM dbo.BD_MeasuringUnit");
            strSql.Append("   ORDER BY ISDefault DESC,numSort ASC");
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



        void GetByID_Supplier(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select ID,WB_ID,strName,LinkMan,LinkManDuty,Address,PostCode,PhoneNO,Mobile,strRemark ");
            strSql.Append("   FROM dbo.WBSupplier WHERE ID=" + ID);
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

        void Add_Supplier(HttpContext context)
        {
           
            string WB_ID = context.Session["WB_ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string LinkMan = context.Request.Form["LinkMan"].ToString();
            string LinkManDuty = context.Request.Form["LinkManDuty"].ToString();
            string Address = context.Request.Form["Address"].ToString();
            string PostCode = context.Request.Form["PostCode"].ToString();
            string PhoneNO = context.Request.Form["PhoneNO"].ToString();
            string Mobile = context.Request.Form["Mobile"].ToString();
            string strRemark = context.Request.Form["strRemark"].ToString();


            string strCount = "    SELECT COUNT(ID)  FROM dbo.WBSupplier WHERE WB_ID=" + WB_ID + "  AND strName='" + strName + "'";
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [WBSupplier] (");
            strSql.Append("WB_ID,strName,LinkMan,LinkManDuty,Address,PostCode,PhoneNO,Mobile,strRemark)");
            strSql.Append(" values (");
            strSql.Append("@WB_ID,@strName,@LinkMan,@LinkManDuty,@Address,@PostCode,@PhoneNO,@Mobile,@strRemark)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WB_ID", SqlDbType.NVarChar,10),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@LinkMan", SqlDbType.NVarChar,50),
					new SqlParameter("@LinkManDuty", SqlDbType.NVarChar,50),
					new SqlParameter("@Address", SqlDbType.NVarChar,100),
					new SqlParameter("@PostCode", SqlDbType.NVarChar,50),
					new SqlParameter("@PhoneNO", SqlDbType.NVarChar,50),
					new SqlParameter("@Mobile", SqlDbType.NVarChar,50),
					new SqlParameter("@strRemark", SqlDbType.NVarChar,500)};
            parameters[0].Value = WB_ID;
            parameters[1].Value = strName;
            parameters[2].Value = LinkMan;
            parameters[3].Value = LinkManDuty;
            parameters[4].Value = Address;
            parameters[5].Value = PostCode;
            parameters[6].Value = PhoneNO;
            parameters[7].Value = Mobile;
            parameters[8].Value = strRemark;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Supplier(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string WB_ID = context.Session["WB_ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string LinkMan = context.Request.Form["LinkMan"].ToString();
            string LinkManDuty = context.Request.Form["LinkManDuty"].ToString();
            string Address = context.Request.Form["Address"].ToString();
            string PostCode = context.Request.Form["PostCode"].ToString();
            string PhoneNO = context.Request.Form["PhoneNO"].ToString();
            string Mobile = context.Request.Form["Mobile"].ToString();
            string strRemark = context.Request.Form["strRemark"].ToString();
            string strCount = "    SELECT COUNT(ID)  FROM dbo.WBSupplier WHERE WB_ID=" + WB_ID + "  AND strName='" + strName + "' and ID !="+ID;
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [WBSupplier] set ");
            strSql.Append("WB_ID=@WB_ID,");
            strSql.Append("strName=@strName,");
            strSql.Append("LinkMan=@LinkMan,");
            strSql.Append("LinkManDuty=@LinkManDuty,");
            strSql.Append("Address=@Address,");
            strSql.Append("PostCode=@PostCode,");
            strSql.Append("PhoneNO=@PhoneNO,");
            strSql.Append("Mobile=@Mobile,");
            strSql.Append("strRemark=@strRemark");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@WB_ID", SqlDbType.NVarChar,10),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@LinkMan", SqlDbType.NVarChar,50),
					new SqlParameter("@LinkManDuty", SqlDbType.NVarChar,50),
					new SqlParameter("@Address", SqlDbType.NVarChar,100),
					new SqlParameter("@PostCode", SqlDbType.NVarChar,50),
					new SqlParameter("@PhoneNO", SqlDbType.NVarChar,50),
					new SqlParameter("@Mobile", SqlDbType.NVarChar,50),
					new SqlParameter("@strRemark", SqlDbType.NVarChar,500),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = WB_ID;
            parameters[1].Value = strName;
            parameters[2].Value = LinkMan;
            parameters[3].Value = LinkManDuty;
            parameters[4].Value = Address;
            parameters[5].Value = PostCode;
            parameters[6].Value = PhoneNO;
            parameters[7].Value = Mobile;
            parameters[8].Value = strRemark;
            parameters[9].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_Supplier(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();


            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodStock WHERE WBSupplierID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            //if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodSupplyStorage WHERE WBSupplierID=" + wbid).ToString() != "0")
            //{
            //    context.Response.Write("Exit");
            //    return;
            //}


            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [WBSupplier] ");
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


        void Get_Good(HttpContext context)
        {
            
            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select ID,strName");       
            strSql.Append("   FROM dbo.Good ");
            strSql.Append("   where 1=1");
            string strName = context.Request.QueryString["strName"].ToString();
            if (strName.Trim()!="")
            {
                strSql.Append("   and  strName like '%" + strName + "%'");
            }
            strSql.Append("   order by ISDefault desc,numSort asc ");
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
        /// 获取积分兑换商品
        /// </summary>
        /// <param name="context"></param>
        void Get_IntegralGood(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select ID,strName,ISIntegral,Integralvalue");
            strSql.Append("   FROM dbo.Good ");
            strSql.Append("   where ISIntegral=1");
            string strName = context.Request.QueryString["strName"].ToString();
            if (strName.Trim() != "")
            {
                strSql.Append("   and  strName like '%" + strName + "%'");
            }
            strSql.Append("   order by ISDefault desc,numSort asc ");
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

        //显示商品兑换信息
        void ShowExchangeUnitByGoodID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.strName AS MeasuringUnit,C.strName AS PackingSpecID,A.Price_DuiHuan,A.Price_XiaoShou,A.Price_Stock AS Price_ShiChang,A.Price_TeJia");
            strSql.Append("  FROM dbo.Good A INNER JOIN dbo.BD_MeasuringUnit B ON A.MeasuringUnit=B.ID");
            strSql.Append("  INNER JOIN dbo.BD_PackingSpec C ON A.PackingSpecID =C.ID");
            strSql.Append("  WHERE A.ID="+ID);
           
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
        /// 查询当前网店的仓库商品库存
        /// </summary>
        /// <param name="context"></param>
        void GetWBGoodByWHID(HttpContext context)
        {
      
            string WBID = context.Request.Form["WBID"].ToString();
            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select A.ID,A.strName,D.strName AS WBWareHouseID,  E.strName AS PackingSpecID,F.strName AS MeasuringUnit,A.Price_Stock,A.Price_StockHQ,B.numStore");
            strSql.Append("   FROM dbo.Good A INNER JOIN dbo.GoodStorage B ON A.ID=B.GoodID");
            strSql.Append("   LEFT OUTER JOIN dbo.WBWareHouse D ON B.WBWareHouseID=D.ID");
            strSql.Append("    LEFT OUTER JOIN dbo.BD_PackingSpec E ON A.PackingSpecID=E.ID");
            strSql.Append("   LEFT OUTER JOIN dbo.BD_MeasuringUnit F ON A.MeasuringUnit=F.ID ");
            strSql.Append("   WHERE B.WBWareHouseID=" + WBWareHouseID + " AND B.WBID= " + WBID);
            strSql.Append("   ");
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
        /// 查询当前网店的商品信息
        /// </summary>
        /// <param name="context"></param>
        void GetGoodByWBID(HttpContext context)
        {
            string GoodID = context.Request.QueryString["GoodID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select A.ID,A.strName,D.strName AS WBWareHouseID,  E.strName AS PackingSpecID,F.strName AS MeasuringUnit,A.Price_Stock,A.Price_StockHQ");
            strSql.Append("   FROM dbo.Good A INNER JOIN dbo.GoodStorage B ON A.ID=B.GoodID");
            strSql.Append("   LEFT OUTER JOIN dbo.WBWareHouse D ON B.WBWareHouseID=D.ID");
            strSql.Append("    LEFT OUTER JOIN dbo.BD_PackingSpec E ON A.PackingSpecID=E.ID");
            strSql.Append("   LEFT OUTER JOIN dbo.BD_MeasuringUnit F ON A.MeasuringUnit=F.ID ");
            strSql.Append("   WHERE A.ID=" + GoodID + " AND B.WBID= "+WBID);
            strSql.Append("   ");
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

        void GetByID_Good(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select ID,strName,BarCode,CategoryID,PackingSpecID,MeasuringUnit,");
            strSql.Append("   Price_Stock,Price_StockHQ,Price_XiaoShou,Price_DuiHuan,Price_VIP,Price_PiFa,PiFaCount_Start,Price_TeJia,numExchangeLimit,ISIntegral,IntegralValue,ISDefault,numSort");
            strSql.Append("   FROM dbo.Good WHERE ID=" + ID);
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

        void Add_Good(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            string BarCode = context.Request.Form["BarCode"].ToString();
            string CategoryID = context.Request.Form["CategoryID"].ToString();
            string PackingSpecID = context.Request.Form["PackingSpecID"].ToString();
            string MeasuringUnit = context.Request.Form["MeasuringUnit"].ToString();
    
            string Price_Stock = context.Request.Form["Price_Stock"].ToString();
            string Price_StockHQ = context.Request.Form["Price_StockHQ"].ToString();
            int ISIntegral = 0;
            if (context.Request.Form["ISIntegral"] != null) {
                ISIntegral = 1;
            }
            string IntegralValue = context.Request.Form["IntegralValue"].ToString();
            string Price_XiaoShou = context.Request.Form["Price_XiaoShou"].ToString();
            string Price_DuiHuan = context.Request.Form["Price_DuiHuan"].ToString();
            string Price_VIP = context.Request.Form["Price_VIP"].ToString();
            string Price_PiFa = context.Request.Form["Price_PiFa"].ToString();
            string Price_TeJia = context.Request.Form["Price_TeJia"].ToString();
            string numExchangeLimit = context.Request.Form["numExchangeLimit"].ToString();
            string PiFaCount_Start = context.Request.Form["PiFaCount_Start"].ToString();

            if (!common.UniqueCheck_Add("Good", "strName", strName)) {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Good] (");
            strSql.Append("strName,BarCode,CategoryID,PackingSpecID,MeasuringUnit,Price_Stock,Price_XiaoShou,Price_DuiHuan,Price_VIP,Price_PiFa,PiFaCount_Start,Price_TeJia,numExchangeLimit,ISDefault,numSort,Price_StockHQ,ISIntegral,IntegralValue)");
            strSql.Append(" values (");
            strSql.Append("@strName,@BarCode,@CategoryID,@PackingSpecID,@MeasuringUnit,@Price_Stock,@Price_XiaoShou,@Price_DuiHuan,@Price_VIP,@Price_PiFa,@PiFaCount_Start,@Price_TeJia,@numExchangeLimit,@ISDefault,@numSort,@Price_StockHQ,@ISIntegral,@IntegralValue)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@BarCode", SqlDbType.NVarChar,200),
					new SqlParameter("@CategoryID", SqlDbType.Int,4),
					new SqlParameter("@PackingSpecID", SqlDbType.Int,4),
					new SqlParameter("@MeasuringUnit", SqlDbType.Int,4),
					new SqlParameter("@Price_Stock", SqlDbType.Decimal,9),
					new SqlParameter("@Price_XiaoShou", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DuiHuan", SqlDbType.Decimal,9),
					new SqlParameter("@Price_VIP", SqlDbType.Decimal,9),
					new SqlParameter("@Price_PiFa", SqlDbType.Decimal,9),
					new SqlParameter("@PiFaCount_Start", SqlDbType.Int,4),
					new SqlParameter("@Price_TeJia", SqlDbType.Decimal,9),
                    new SqlParameter("@numExchangeLimit", SqlDbType.Decimal,9),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4),
                    new SqlParameter("@Price_StockHQ", SqlDbType.Decimal,9),
					new SqlParameter("@ISIntegral", SqlDbType.Bit,1),
                    new SqlParameter("@IntegralValue", SqlDbType.Decimal,9)};
            parameters[0].Value = strName;
            parameters[1].Value = BarCode;
            parameters[2].Value = CategoryID;
            parameters[3].Value = PackingSpecID;
            parameters[4].Value = MeasuringUnit;
            parameters[5].Value = Price_Stock;
            parameters[6].Value = Price_XiaoShou;
            parameters[7].Value = Price_DuiHuan;
            parameters[8].Value = Price_VIP;
            parameters[9].Value = Price_PiFa;
            parameters[10].Value = PiFaCount_Start;
            parameters[11].Value = Price_TeJia;
            parameters[12].Value = numExchangeLimit;
            parameters[13].Value = 0;
            parameters[14].Value = 1;
            parameters[15].Value = Price_StockHQ;
            parameters[16].Value = ISIntegral;
            parameters[17].Value = IntegralValue;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Good(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("Good", "strName", strName,ID))
            {
                context.Response.Write("1");
                return;
            }
            string BarCode = context.Request.Form["BarCode"].ToString();
            string CategoryID = context.Request.Form["CategoryID"].ToString();
            string PackingSpecID = context.Request.Form["PackingSpecID"].ToString();
            string MeasuringUnit = context.Request.Form["MeasuringUnit"].ToString();

            string Price_Stock = context.Request.Form["Price_Stock"].ToString();
            string Price_StockHQ = context.Request.Form["Price_StockHQ"].ToString();
            int ISIntegral = 0;
            if (context.Request.Form["ISIntegral"] != null)
            {
                ISIntegral = 1;
            }
            string IntegralValue = context.Request.Form["IntegralValue"].ToString();
            string Price_XiaoShou = context.Request.Form["Price_XiaoShou"].ToString();
            string Price_DuiHuan = context.Request.Form["Price_DuiHuan"].ToString();
            string Price_VIP = context.Request.Form["Price_VIP"].ToString();
            string Price_PiFa = context.Request.Form["Price_PiFa"].ToString();
            string Price_TeJia = context.Request.Form["Price_TeJia"].ToString();
            string numExchangeLimit = context.Request.Form["numExchangeLimit"].ToString();
            string PiFaCount_Start = context.Request.Form["PiFaCount_Start"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [Good] set ");
            strSql.Append("strName=@strName,");
            strSql.Append("BarCode=@BarCode,");
            strSql.Append("CategoryID=@CategoryID,");
            strSql.Append("PackingSpecID=@PackingSpecID,");
            strSql.Append("MeasuringUnit=@MeasuringUnit,");
            strSql.Append("Price_Stock=@Price_Stock,");
            strSql.Append("Price_StockHQ=@Price_StockHQ,");
            strSql.Append("ISIntegral=@ISIntegral,");
            strSql.Append("IntegralValue=@IntegralValue,");
            strSql.Append("Price_XiaoShou=@Price_XiaoShou,");
            strSql.Append("Price_DuiHuan=@Price_DuiHuan,");
            strSql.Append("Price_VIP=@Price_VIP,");
            strSql.Append("Price_PiFa=@Price_PiFa,");
            strSql.Append("PiFaCount_Start=@PiFaCount_Start,");
            strSql.Append("Price_TeJia=@Price_TeJia,");
            strSql.Append("numExchangeLimit=@numExchangeLimit,");
            strSql.Append("ISDefault=@ISDefault,");
            strSql.Append("numSort=@numSort");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
                    new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@BarCode", SqlDbType.NVarChar,200),
					new SqlParameter("@CategoryID", SqlDbType.Int,4),
					new SqlParameter("@PackingSpecID", SqlDbType.Int,4),
					new SqlParameter("@MeasuringUnit", SqlDbType.Int,4),
					new SqlParameter("@Price_Stock", SqlDbType.Decimal,9),
                    new SqlParameter("@Price_StockHQ", SqlDbType.Decimal,9),
                    new SqlParameter("@ISIntegral", SqlDbType.Bit,1),
                    new SqlParameter("@IntegralValue", SqlDbType.Decimal,9),
					new SqlParameter("@Price_XiaoShou", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DuiHuan", SqlDbType.Decimal,9),
					new SqlParameter("@Price_VIP", SqlDbType.Decimal,9),
					new SqlParameter("@Price_PiFa", SqlDbType.Decimal,9),
					new SqlParameter("@PiFaCount_Start", SqlDbType.Int,4),
					new SqlParameter("@Price_TeJia", SqlDbType.Decimal,9),
                    	new SqlParameter("@numExchangeLimit", SqlDbType.Decimal,9),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = BarCode;
            parameters[2].Value = CategoryID;
            parameters[3].Value = PackingSpecID;
            parameters[4].Value = MeasuringUnit;
            parameters[5].Value = Price_Stock;
            parameters[6].Value = Price_StockHQ;
            parameters[7].Value = ISIntegral;
            parameters[8].Value = IntegralValue;
            parameters[9].Value = Price_XiaoShou;
            parameters[10].Value = Price_DuiHuan;
            parameters[11].Value = Price_VIP;
            parameters[12].Value = Price_PiFa;
            parameters[13].Value = PiFaCount_Start;
            parameters[14].Value = Price_TeJia;
            parameters[15].Value = numExchangeLimit;
            parameters[16].Value = 0;
            parameters[17].Value = 1;
            parameters[18].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void UpdateGoodQuantity(HttpContext context)
        {
            string GoodStorageID = context.Request.Form["GoodStorageID"].ToString();
            string Quantity = context.Request.Form["Quantity"].ToString();
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append("UPDATE dbo.GoodStorage SET numStore="+Quantity);
            strSql.Append(" WHERE ID=" + GoodStorageID);
          


            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateGoodSupplyQuantity(HttpContext context)
        {
            string WBID = context.Request.QueryString["WBID"].ToString();
            string GoodSupplyID = context.Request.QueryString["GoodSupplyID"].ToString();
            string Quantity = context.Request.QueryString["Quantity"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("UPDATE dbo.GoodSupplyStorage SET numStore=" + Quantity);
            strSql.Append(" WHERE GoodSupplyID=" + GoodSupplyID + " AND WBID=" + WBID);



            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        void DeleteByID_Good(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();


            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodExchange WHERE GoodID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodExchangeProp WHERE GoodID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodStock WHERE GoodID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodStorage WHERE GoodID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [Good] ");
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




        void Get_GoodSupply(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select ID,strName");
            strSql.Append("   FROM dbo.GoodSupply ");
            strSql.Append("   where 1=1");
            if (context.Request.QueryString["strName"] != null)
            {
                strSql.Append("   and  strName like '%" + context.Request.QueryString["strName"].ToString() + "%'");
            }
            strSql.Append("   order by ISDefault desc,numSort asc ");
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

        void GetByID_GoodSupply(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select ID,strName,BarCode,CategoryID,SpecID,UnitID,Price,Price_WB,Price_WBBack,Price_Commune,numDiscount,ISPreDefine,ISDefault,numSort ");

            strSql.Append("   FROM dbo.GoodSupply WHERE ID=" + ID);
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

        void Add_GoodSupply(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            string BarCode = context.Request.Form["BarCode"].ToString();
            string CategoryID = context.Request.Form["CategoryID"].ToString();
            string SpecID = context.Request.Form["PackingSpecID"].ToString();
            string UnitID = context.Request.Form["MeasuringUnit"].ToString();
            string Price = context.Request.Form["Price"].ToString();
            string Price_WB = context.Request.Form["Price_WB"].ToString();
            string Price_WBBack = context.Request.Form["Price_WBBack"].ToString();
            string Price_Commune = context.Request.Form["Price_Commune"].ToString();
            string numDiscount = context.Request.Form["numDiscount"].ToString();
            bool ISPreDefine = false;
            if (context.Request.Form["ISPreDefine"] != null)
            {
                ISPreDefine = true;
            }


            string strCount = "    SELECT COUNT(ID)  FROM dbo.GoodSupply WHERE strName='" + strName + "'";
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodSupply] (");
            strSql.Append("strName,BarCode,CategoryID,SpecID,UnitID,Price,Price_WB,Price_WBBack,Price_Commune,numDiscount,ISPreDefine,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@strName,@BarCode,@CategoryID,@SpecID,@UnitID,@Price,@Price_WB,@Price_WBBack,@Price_Commune,@numDiscount,@ISPreDefine,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@BarCode", SqlDbType.NVarChar,50),
					new SqlParameter("@CategoryID", SqlDbType.Int,4),
					new SqlParameter("@SpecID", SqlDbType.Int,4),
					new SqlParameter("@UnitID", SqlDbType.Int,4),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WB", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WBBack", SqlDbType.Decimal,9),
					new SqlParameter("@Price_Commune", SqlDbType.Decimal,9),
					new SqlParameter("@numDiscount", SqlDbType.Decimal,9),
					new SqlParameter("@ISPreDefine", SqlDbType.Bit,1),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = BarCode;
            parameters[2].Value = CategoryID;
            parameters[3].Value = SpecID;
            parameters[4].Value = UnitID;
            parameters[5].Value = Price;
            parameters[6].Value = Price_WB;
            parameters[7].Value = Price_WBBack;
            parameters[8].Value = Price_Commune;
            parameters[9].Value = numDiscount;
            parameters[10].Value = ISPreDefine;
            parameters[11].Value = 0;
            parameters[12].Value = 1;
          


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_GoodSupply(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            string BarCode = context.Request.Form["BarCode"].ToString();
            string CategoryID = context.Request.Form["CategoryID"].ToString();
            string SpecID = context.Request.Form["PackingSpecID"].ToString();
            string UnitID = context.Request.Form["MeasuringUnit"].ToString();
            string Price = context.Request.Form["Price"].ToString();
            string Price_WB = context.Request.Form["Price_WB"].ToString();
            string Price_WBBack = context.Request.Form["Price_WBBack"].ToString();
            string Price_Commune = context.Request.Form["Price_Commune"].ToString();
            string numDiscount = context.Request.Form["numDiscount"].ToString();
            bool ISPreDefine = false;
            if (context.Request.Form["ISPreDefine"] != null)
            {
                ISPreDefine = true;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [GoodSupply] set ");
            strSql.Append("strName=@strName,");
            strSql.Append("BarCode=@BarCode,");
            strSql.Append("CategoryID=@CategoryID,");
            strSql.Append("SpecID=@SpecID,");
            strSql.Append("UnitID=@UnitID,");
            strSql.Append("Price=@Price,");
            strSql.Append("Price_WB=@Price_WB,");
            strSql.Append("Price_WBBack=@Price_WBBack,");
            strSql.Append("Price_Commune=@Price_Commune,");
            strSql.Append("numDiscount=@numDiscount,");
            strSql.Append("ISPreDefine=@ISPreDefine,");
            strSql.Append("ISDefault=@ISDefault,");
            strSql.Append("numSort=@numSort");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@BarCode", SqlDbType.NVarChar,50),
					new SqlParameter("@CategoryID", SqlDbType.Int,4),
					new SqlParameter("@SpecID", SqlDbType.Int,4),
					new SqlParameter("@UnitID", SqlDbType.Int,4),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WB", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WBBack", SqlDbType.Decimal,9),
					new SqlParameter("@Price_Commune", SqlDbType.Decimal,9),
					new SqlParameter("@numDiscount", SqlDbType.Decimal,9),
					new SqlParameter("@ISPreDefine", SqlDbType.Bit,1),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = BarCode;
            parameters[2].Value = CategoryID;
            parameters[3].Value = SpecID;
            parameters[4].Value = UnitID;
            parameters[5].Value = Price;
            parameters[6].Value = Price_WB;
            parameters[7].Value = Price_WBBack;
            parameters[8].Value = Price_Commune;
            parameters[9].Value = numDiscount;
            parameters[10].Value = ISPreDefine;
            parameters[11].Value = 0;
            parameters[12].Value = 1;
            parameters[13].Value = ID;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_GoodSupply(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodSupplyApply WHERE GoodSupplyID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodSupplyStock WHERE GoodSupplyID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodSupplyStorage WHERE GoodSupplyID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.C_Supply WHERE GoodSupplyID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" delete from [GoodSupply] ");
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


        /// <summary>
        /// 获取网店商品的存储信息
        /// </summary>
        /// <param name="context"></param>
        void GetGoodStorageByGoodID(HttpContext context)
        {
            string GoodID = context.Request.QueryString["GoodID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("    SELECT A.ID,B.strName AS WBWareHouseName, WBWareHouseID,numStore,maxStore ");
            strSql.Append("    FROM dbo.GoodStorage A INNER JOIN dbo.WBWareHouse B ON A.WBWareHouseID=B.ID");
            strSql.Append("   WHERE A.WBID="+WBID+" AND GoodID="+GoodID);
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
        /// 获取网店商品的存储信息
        /// </summary>
        /// <param name="context"></param>
        void GetWBGoodStorage(HttpContext context)
        {
          
            string WBID = context.Session["WB_ID"].ToString();
            if (context.Request.Form["ISHQ"]!=null)//查询总部库存
            {
                WBID = SQLHelper.ExecuteScalar(" select ID from WB where ISHQ=1").ToString();
            }
            string WHID = context.Request.Form["WHID"].ToString();//仓库
            string GoodID = context.Request.Form["GoodID"].ToString();//商品
            StringBuilder strSql = new StringBuilder();
            strSql.Append("    SELECT A.ID,B.strName AS WBWareHouseName, WBWareHouseID,numStore,maxStore ");
            strSql.Append("    FROM dbo.GoodStorage A INNER JOIN dbo.WBWareHouse B ON A.WBWareHouseID=B.ID");
            strSql.Append(string.Format("   WHERE A.WBID={0} AND WBWareHouseID={1}  AND GoodID={2}", WBID, WHID, GoodID));
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
        /// 查询当前网店的社员 商品信息
        /// </summary>
        /// <param name="context"></param>
        void GetGoodSupplyByWBID(HttpContext context)
        {
            string GoodID = context.Request.QueryString["GoodID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select A.ID,A.strName,C.strName AS WBSupplierID,D.strName AS WBWareHouseID,  E.strName AS PackingSpecID,F.strName AS MeasuringUnit,A.Price,A.Price_WB,A.Price_WBBack,A.Price_Commune ");
            strSql.Append("   FROM dbo.GoodSupply A INNER JOIN dbo.GoodSupplyStorage B ON A.ID=B.GoodSupplyID");
            strSql.Append("    INNER JOIN dbo.WBSupplier C ON B.WBSupplierID=C.ID");
            strSql.Append("   INNER JOIN dbo.WBWareHouse D ON B.WBWareHouseID=D.ID");
            strSql.Append("    INNER JOIN dbo.BD_PackingSpec E ON A.SpecID=E.ID");
            strSql.Append("   INNER JOIN dbo.BD_MeasuringUnit F ON A.UnitID=F.ID ");
            strSql.Append("   WHERE A.ID=" + GoodID + " AND B.WBID= " + WBID);
            strSql.Append("   ");
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


        void GetByID_GoodStorage(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("    SELECT ID,WBID,GoodID, WBWareHouseID,numStore,maxStore FROM dbo.GoodStorage");
            strSql.Append("   WHERE  ID="+ID);
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

        void Add_GoodStorage(HttpContext context)
        {

            string GoodID = context.Request.QueryString["GoodID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();

            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();
            string maxStore = context.Request.Form["maxStore"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodStorage] (");
            strSql.Append("GoodID,WBID,WBWareHouseID,numStore,maxStore)");
            strSql.Append(" values (");
            strSql.Append("@GoodID,@WBID,@WBWareHouseID,@numStore,@maxStore)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@GoodID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@WBWareHouseID", SqlDbType.Int,4),
					new SqlParameter("@numStore", SqlDbType.BigInt,8),
					new SqlParameter("@maxStore", SqlDbType.BigInt,8)};
            parameters[0].Value = GoodID;
            parameters[1].Value = WBID;
            parameters[2].Value = WBWareHouseID;
            parameters[3].Value = 0;
            parameters[4].Value = maxStore;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_GoodStorage(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();
            string maxStore = context.Request.Form["maxStore"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" UPDATE dbo.GoodStorage  SET WBWareHouseID="+WBWareHouseID+",maxStore="+maxStore);
          
            strSql.Append(" where ID="+ID);
           
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        //更改库存信息
        void Update_numStore(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();


            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();
            string maxStore = context.Request.Form["maxStore"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" UPDATE dbo.GoodStorage  SET WBWareHouseID=" + WBWareHouseID + ",maxStore=" + maxStore);

            strSql.Append(" where ID=" + ID);

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        //添加进货信息
        void Add_GoodStock(HttpContext context)
        {

            string GoodID = context.Request.Form["GoodID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();

            string WBSupplierID = "0";
            if (context.Request.Form["WBSupplierID"] != null) { 
            WBSupplierID = context.Request.Form["WBSupplierID"].ToString();
            }
            string Price_Stock = context.Request.Form["Price_Stock"].ToString();
            string Quantity = context.Request.Form["Quantity"].ToString();
            bool ISCash = false;
            if (context.Request.Form["ISCash"] != null)
            {
                ISCash = true;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodStock] (");
            strSql.Append("WBID,GoodID,Price_Stock,Num_Stock,Quantity,ISCash,dt_Trade,WBWareHouseID,WBSupplierID)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@GoodID,@Price_Stock,@Num_Stock,@Quantity,@ISCash,@dt_Trade,@WBWareHouseID,@WBSupplierID)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@GoodID", SqlDbType.Int,4),
					new SqlParameter("@Price_Stock", SqlDbType.Decimal,9),
					new SqlParameter("@Num_Stock", SqlDbType.BigInt,8),
					new SqlParameter("@Quantity", SqlDbType.BigInt,8),
					new SqlParameter("@ISCash", SqlDbType.Bit,1),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime),
                   new SqlParameter("@WBWareHouseID", SqlDbType.Int,4),
                   new SqlParameter("@WBSupplierID", SqlDbType.Int,4)};
            parameters[0].Value = WBID;
            parameters[1].Value = GoodID;
            parameters[2].Value = Price_Stock;
            parameters[3].Value = 0;
            parameters[4].Value = Quantity;
            parameters[5].Value = ISCash;
            parameters[6].Value = DateTime.Now;
            parameters[7].Value = WBWareHouseID;
            parameters[8].Value = WBSupplierID;
          
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(),parameters);

                    StringBuilder sql_GoodStorage = new StringBuilder();
                    object obj_count = SQLHelper.ExecuteScalar(string.Format("  select count(ID) from GoodStorage  WHERE GoodID={0} and WBWareHouseID={1} and WBID={2}", GoodID, WBWareHouseID, WBID));
                    if (Convert.ToInt32(obj_count) > 0) {
                        sql_GoodStorage .Append( string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore+" + Quantity + " WHERE GoodID={0} and WBWareHouseID={1} and WBID={2}", GoodID, WBWareHouseID, WBID));
                          SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql_GoodStorage.ToString());

                    }else{

                        sql_GoodStorage.Append("insert into [GoodStorage] (");
                        sql_GoodStorage.Append("GoodID,WBID,WBWareHouseID,numStore,maxStore)");
                        sql_GoodStorage.Append(" values (");
                        sql_GoodStorage.Append("@GoodID,@WBID,@WBWareHouseID,@numStore,@maxStore)");
                        sql_GoodStorage.Append(";select @@IDENTITY");
                        SqlParameter[] para_GoodStorage = {
					new SqlParameter("@GoodID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@WBWareHouseID", SqlDbType.Int,4),
					new SqlParameter("@numStore", SqlDbType.BigInt,8),
					new SqlParameter("@maxStore", SqlDbType.BigInt,8)};
                        para_GoodStorage[0].Value = GoodID;
                        para_GoodStorage[1].Value = WBID;
                        para_GoodStorage[2].Value = WBWareHouseID;
                        para_GoodStorage[3].Value = Quantity;
                        para_GoodStorage[4].Value = 0;
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql_GoodStorage.ToString(), para_GoodStorage);
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
         
        }


        /// <summary>
        /// 分行之间调货
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodAllocateWB(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();//当前网点
            string UserID = context.Session["ID"].ToString();//当前操作人
            string AllocateWBID = context.Request.Form["QWBID"].ToString();//接收调货网点
            string GoodID = context.Request.Form["GoodID"].ToString();
            string WBWareHouseID = context.Request.Form["WareHouseID"].ToString();
            string AllocateWBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();
            string Num_Stock = context.Request.Form["Quantity"].ToString();
            string Price_Stock = context.Request.Form["Price_Stock"].ToString();
            string Quantity = context.Request.Form["Quantity"].ToString();
            bool ISCash =true;
            string strRecipient = context.Request.Form["strRecipient"].ToString();
            if (strRecipient.Length > 50)
            {
                strRecipient = strRecipient.Substring(0, 50);
            }

          
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodAllocate] (");
            strSql.Append("WBID , AllocateWBID , UserID , GoodID , WBWareHouseID ,AllocateWBWareHouseID ,Num_Stock ,Price_Stock ,Quantity ,ISCash , strRecipient ,dt_Trade)");
            strSql.Append(" values (");
            strSql.Append("@WBID , @AllocateWBID , @UserID , @GoodID , @WBWareHouseID ,@AllocateWBWareHouseID ,@Num_Stock ,@Price_Stock ,@Quantity ,@ISCash , @strRecipient ,@dt_Trade)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@AllocateWBID", SqlDbType.Int,4),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@GoodID", SqlDbType.Int,4),
                     new SqlParameter("@WBWareHouseID", SqlDbType.Int,4),
					new SqlParameter("@AllocateWBWareHouseID", SqlDbType.Int,4),
					new SqlParameter("@Num_Stock", SqlDbType.Decimal,9),
					new SqlParameter("@Price_Stock", SqlDbType.Decimal,8),
					new SqlParameter("@Quantity",SqlDbType.Decimal,8),
					new SqlParameter("@ISCash", SqlDbType.Bit,1),
                    new SqlParameter("@strRecipient", SqlDbType.NVarChar,100),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parameters[0].Value = WBID;
            parameters[1].Value = AllocateWBID;
            parameters[2].Value = UserID;
            parameters[3].Value = GoodID;
            parameters[4].Value = WBWareHouseID;
            parameters[5].Value = AllocateWBWareHouseID;
            parameters[6].Value = Num_Stock;
            parameters[7].Value = Price_Stock;
            parameters[8].Value = Quantity;
            parameters[9].Value = ISCash;
            parameters[10].Value = strRecipient;
            parameters[11].Value = DateTime.Now.ToString();


            //减去本网点库存
            string sqlWB = string.Format("UPDATE dbo.GoodStorage SET numStore=numStore-{0} WHERE GoodID={1} and WBWareHouseID={2}  and WBID={3}", Quantity, GoodID, WBWareHouseID, WBID);


            object obj_count = SQLHelper.ExecuteScalar(string.Format("  select count(ID) from GoodStorage  WHERE GoodID={0} and WBWareHouseID={1} and WBID={2}", GoodID, AllocateWBWareHouseID, AllocateWBID));
            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    //添加商品调货记录
                     SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);
                     //调货方库存更新
                     SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlWB.ToString());

                    //接收方库存更新
                     //网点和总行库存记录修改
                     StringBuilder sqlWBAllocate = new StringBuilder();
                   
                     if (Convert.ToInt32(obj_count) > 0)
                     {
                         sqlWBAllocate.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore+" + Quantity + " WHERE GoodID={0} and WBWareHouseID={1} and WBID={2}", GoodID, AllocateWBWareHouseID, AllocateWBID));

                         SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlWBAllocate.ToString()); 
                     }
                     else
                     {

                         sqlWBAllocate.Append("insert into [GoodStorage] (");
                         sqlWBAllocate.Append("GoodID,WBID,WBWareHouseID,numStore,maxStore)");
                         sqlWBAllocate.Append(" values (");
                         sqlWBAllocate.Append("@GoodID,@WBID,@WBWareHouseID,@numStore,@maxStore)");
                         sqlWBAllocate.Append(";select @@IDENTITY");
                         SqlParameter[] para_Allocate = {
					new SqlParameter("@GoodID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@WBWareHouseID", SqlDbType.Int,4),
					new SqlParameter("@numStore", SqlDbType.BigInt,8),
					new SqlParameter("@maxStore", SqlDbType.BigInt,8)};
                         para_Allocate[0].Value = GoodID;
                         para_Allocate[1].Value = AllocateWBID;
                         para_Allocate[2].Value = AllocateWBWareHouseID;
                         para_Allocate[3].Value = Quantity;
                         para_Allocate[4].Value = 1000000;//默认容量

                         SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlWBAllocate.ToString(), para_Allocate); 

                     }
                                   
                    tran.Commit();
                    var res = new { state = "true", msg = "调货成功!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "true", msg = "调货失败!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }

        }

        /// <summary>
        /// 分行进货
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodStockWB(HttpContext context)
        {

            string WBID = context.Session["WB_ID"].ToString();
             string UserID = context.Session["ID"].ToString();
            string HQWBID = SQLHelper.ExecuteScalar(" SELECT top 1 ID FROM dbo.WB WHERE ISHQ=1").ToString();//总部网店
            object objISSimulate = SQLHelper.ExecuteScalar("  SELECT TOP 1 ISSimulate FROM dbo.WB WHERE ID=" + WBID);
            bool ISSimulate = Convert.ToBoolean(objISSimulate);
            string WBSupplierID = "0";
            if (context.Request.Form["WBSupplierID"] != null)
            {
                WBSupplierID = context.Request.Form["WBSupplierID"].ToString();
            }


            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();//网点仓库号
            string HQWareHouseID = context.Request.Form["HQWareHouseID"].ToString();//总部仓库号
            string GoodID = context.Request.Form["GoodIDSelect"].ToString();
           
            string Price_Stock = context.Request.Form["Price_Stock"].ToString();
            string Quantity = context.Request.Form["Quantity"].ToString();
            bool ISCash = false;
            if (context.Request.Form["ISCash"] != null)
            {
                ISCash = true;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodStock] (");
            strSql.Append("WBID,GoodID,Price_Stock,Num_Stock,Quantity,ISCash,dt_Trade,WBWareHouseID,WBSupplierID)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@GoodID,@Price_Stock,@Num_Stock,@Quantity,@ISCash,@dt_Trade,@WBWareHouseID,@WBSupplierID)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@GoodID", SqlDbType.Int,4),
					new SqlParameter("@Price_Stock", SqlDbType.Decimal,9),
					new SqlParameter("@Num_Stock", SqlDbType.BigInt,8),
					new SqlParameter("@Quantity", SqlDbType.BigInt,8),
					new SqlParameter("@ISCash", SqlDbType.Bit,1),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime),
					new SqlParameter("@WBWareHouseID", SqlDbType.Int,4),
					new SqlParameter("@WBSupplierID", SqlDbType.Int,4)};
            parameters[0].Value = WBID;
            parameters[1].Value = GoodID;
            parameters[2].Value = Price_Stock;
            parameters[3].Value = 0;
            parameters[4].Value = Quantity;
            parameters[5].Value = ISCash;
            parameters[6].Value = DateTime.Now;
            parameters[7].Value = WBWareHouseID;
            parameters[8].Value = WBSupplierID;


             //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    object objGoodStockID= SQLHelper.ExecuteScalar(tran, CommandType.Text, strSql.ToString(), parameters);//添加商品进货记录

                    //网点进货提醒
                    double Amount=Math.Round(Convert.ToDouble( Quantity)*Convert.ToDouble( Price_Stock),2);
                    StringBuilder sqlwarn = new StringBuilder();
                    sqlwarn.Append(" INSERT INTO dbo.GoodStockWarn");
                    sqlwarn.Append("  ( GoodStockID , WBID ,UserID ,GoodID , Num_Stock , Price_Stock ,Amount , OTime ,ISRead)");
                    sqlwarn.Append(string.Format(" VALUES ( {0} , {1} , {2} , {3} , {4} , {5} , {6} ,'{7}' ,{8}  )", objGoodStockID, WBID, UserID, GoodID, Quantity, Price_Stock,Amount,DateTime.Now.ToString(),0));

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlwarn.ToString());//添加商品进货记录提醒

                    //网点和总行库存记录修改
                    StringBuilder sql_GoodStorage = new StringBuilder();
                    object obj_count = SQLHelper.ExecuteScalar(string.Format("  select count(ID) from GoodStorage  WHERE GoodID={0} and WBWareHouseID={1} and WBID={2}", GoodID, WBWareHouseID, WBID));
                    if (Convert.ToInt32(obj_count) > 0)
                    {
                        sql_GoodStorage.Append(string.Format("  UPDATE dbo.GoodStorage SET numStore=numStore+" + Quantity + " WHERE GoodID={0} and WBWareHouseID={1} and WBID={2}", GoodID, WBWareHouseID, WBID));
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql_GoodStorage.ToString());

                    }
                    else
                    {

                        sql_GoodStorage.Append("insert into [GoodStorage] (");
                        sql_GoodStorage.Append("GoodID,WBID,WBWareHouseID,numStore,maxStore)");
                        sql_GoodStorage.Append(" values (");
                        sql_GoodStorage.Append("@GoodID,@WBID,@WBWareHouseID,@numStore,@maxStore)");
                        sql_GoodStorage.Append(";select @@IDENTITY");
                        SqlParameter[] para_GoodStorage = {
					new SqlParameter("@GoodID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@WBWareHouseID", SqlDbType.Int,4),
					new SqlParameter("@numStore", SqlDbType.BigInt,8),
					new SqlParameter("@maxStore", SqlDbType.BigInt,8)};
                        para_GoodStorage[0].Value = GoodID;
                        para_GoodStorage[1].Value = WBID;
                        para_GoodStorage[2].Value = WBWareHouseID;
                        para_GoodStorage[3].Value = Quantity;
                        para_GoodStorage[4].Value = 0;
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql_GoodStorage.ToString(), para_GoodStorage);
                    }
                   
                    if (!ISSimulate)//如果不是模拟账户则减去总部库存
                    {
                        string strSqlHQDel = "";
                        if (Convert.ToDouble( Quantity) >= 0)
                        {
                            //减去总部库存
                            strSqlHQDel = string.Format("UPDATE dbo.GoodStorage SET numStore=numStore-{0} WHERE GoodID={1} and WBWareHouseID={2}  and WBID={3}", Quantity, GoodID, HQWareHouseID, HQWBID);
                        }
                        else {
                            Quantity = Math.Abs(Convert.ToDouble(Quantity)).ToString();
                            //增加总部库存
                            strSqlHQDel = string.Format("UPDATE dbo.GoodStorage SET numStore=numStore+{0} WHERE GoodID={1} and WBWareHouseID={2}  and WBID={3}", Quantity, GoodID, HQWareHouseID, HQWBID);
                        }
                        
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlHQDel.ToString());
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

        }

        void Update_GoodStockWB(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string WBID = context.Request.QueryString["WBID"].ToString();
            string GoodID = context.Request.QueryString["GoodID"].ToString();
            int QuanlityOriginal = Convert.ToInt32(context.Request.QueryString["QuanlityOriginal"]);
            int Quanlity = Convert.ToInt32(context.Request.QueryString["Quanlity"]);

            int QuanlityAdd = Quanlity - QuanlityOriginal;//商品修改增量
            string UpdateGoodStock = " UPDATE dbo.GoodStock SET Quantity=" + Quanlity + " WHERE ID=" + ID;

            string UpdateGoodStorage = " UPDATE  dbo.GoodStorage SET numStore=numStore+"+QuanlityAdd+" WHERE WBID="+WBID+" AND GoodID="+GoodID;

            //减去总部库存
            string HQWBID = SQLHelper.ExecuteScalar(" SELECT top 1 ID FROM dbo.WB WHERE ISHQ=1").ToString();//总部网店
             string strSqlDel = "";
             if (QuanlityAdd > 0)
             {
                 strSqlDel = "UPDATE dbo.GoodStorage SET numStore=numStore-" + QuanlityAdd + " WHERE GoodID=" + GoodID + " and WBID=" + HQWBID;
             }
             else {
                 strSqlDel = "UPDATE dbo.GoodStorage SET numStore=numStore+" +   Math.Abs(QuanlityAdd) + " WHERE GoodID=" + GoodID + " and WBID=" + HQWBID;
             }

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, UpdateGoodStock.ToString());//添加换存折记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, UpdateGoodStorage.ToString());
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDel.ToString());
                 
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
        /// 更新网点进货记录提醒
        /// </summary>
        /// <param name="context"></param>
        void Update_GoodStockWarn(HttpContext context)
        {

            string idlist = context.Request.QueryString["idlist"].ToString();

            string sql = string.Format(" UPDATE dbo.GoodStockWarn SET ISRead={0} WHERE ID IN ({1})",1,idlist);
            SQLHelper.ExecuteNonQuery(sql);
            context.Response.Write("Y");

        }


        /// <summary>
        /// 更新网点进货记录提醒
        /// </summary>
        /// <param name="context"></param>
        void Get_GoodStockWarn(HttpContext context)
        {

            string ISRead = context.Request.QueryString["ISRead"].ToString();
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT A.ID,B.strName AS WBName,C.strRealName AS UserName,D.strName AS GoodName,");
            sql.Append(" A.Num_Stock,A.Price_Stock,A.Amount,CONVERT(varchar(100), A.OTime, 23) AS OTime,A.ISRead");
            sql.Append(" FROM dbo.GoodStockWarn A LEFT OUTER JOIN dbo.WB B ON A.WBID=B.ID ");
            sql.Append(" LEFT OUTER JOIN Users C ON A.UserID=C.ID");
            sql.Append(" LEFT OUTER JOIN dbo.Good D ON A.GoodID=D.ID");
            if (ISRead == "0"||ISRead=="1") {
                sql.Append(" WHERE ISRead="+ISRead);
            }
            DataTable dt = SQLHelper.ExecuteDataTable(sql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                context.Response.Write("Error");
            }
            else {
                context.Response.Write(JsonHelper.ToJson(dt));
              
            }
           
        }

        /// <summary>
        /// 查找某种商品的库存
        /// </summary>
        /// <param name="context"></param>
        void GetHQStorage_Dep(HttpContext context)
        {
            string GoodID = context.Request.QueryString["GoodID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("    SELECT  sum(A.numStore) as numStore");
            strSql.Append("  FROM dbo.GoodStorage A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("   WHERE B.ISHQ=1 AND  GoodID="+GoodID);
            strSql.Append("  ");
            
            object objCount = SQLHelper.ExecuteScalar(strSql.ToString());
            if (objCount == null || objCount.ToString() == "")
            {
                context.Response.Write("0");
            }
            else
            {

                context.Response.Write(objCount.ToString());
            }
        }




        //添加总部进货信息
        void Add_GoodSupplyStock(HttpContext context)
        {

            string GoodSupplyID = context.Request.Form["GoodIDSelect"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            object objISHQ = SQLHelper.ExecuteScalar(" SELECT ISHQ  FROM dbo.WB WHERE ID=" + WBID);
            bool ISHQ = Convert.ToBoolean(objISHQ);
            string Price = context.Request.Form["Price"].ToString();
            string Price_WB = context.Request.Form["Price_WB"].ToString();
            string Price_WBBack = context.Request.Form["Price_WBBack"].ToString();
            string Quantity = context.Request.Form["Quantity"].ToString();

            double Price_Money = Math.Round(Convert.ToDouble(Price_WB) * Convert.ToDouble(Quantity), 2);
            bool ISCash = false;
            if (context.Request.Form["ISCash"] != null)
            {
                ISCash = true;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodSupplyStock] (");
            strSql.Append("WBID,ISHQ,GoodSupplyID,Price,Price_WB,Price_WBBack,Price_Money,Num_Stock,Quantity,ISCash,dt_Trade)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@ISHQ,@GoodSupplyID,@Price,@Price_WB,@Price_WBBack,@Price_Money,@Num_Stock,@Quantity,@ISCash,@dt_Trade)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@ISHQ", SqlDbType.Int,4),
					new SqlParameter("@GoodSupplyID", SqlDbType.Int,4),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WB", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WBBack", SqlDbType.Decimal,9),
					new SqlParameter("@Price_Money", SqlDbType.Decimal,9),
					new SqlParameter("@Num_Stock", SqlDbType.Decimal,9),
					new SqlParameter("@Quantity", SqlDbType.Decimal,9),
					new SqlParameter("@ISCash", SqlDbType.Bit,1),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parameters[0].Value = WBID;
            parameters[1].Value = ISHQ;
            parameters[2].Value = GoodSupplyID;
            parameters[3].Value = Price;
            parameters[4].Value = Price_WB;
            parameters[5].Value = Price_WBBack;
            parameters[6].Value = Price_Money;
            parameters[7].Value = 0;
            parameters[8].Value = Quantity;
            parameters[9].Value = ISCash;
            parameters[10].Value = DateTime.Now;


            int ID = SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters);

            if (ID != 0)
            {
                //刷新库存信息
                SQLHelper.ExecuteNonQuery("  UPDATE dbo.GoodSupplyStorage SET numStore=numStore+" + Quantity + " WHERE GoodSupplyID=" + GoodSupplyID + " and WBID=" + WBID);
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

       
        /// <summary>
        /// 查找网店某种商品的库存
        /// </summary>
        /// <param name="context"></param>
        void GetGoodSupplyStorage(HttpContext context)
        {
            string GoodSupplyID = context.Request.QueryString["GoodSupplyID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            string strSql = "    SELECT numStore FROM dbo.GoodSupplyStorage WHERE GoodSupplyID=" + GoodSupplyID + " AND WBID="+WBID;
            object objCount = SQLHelper.ExecuteScalar(strSql);
            if (objCount == null || objCount.ToString() == "")
            {
                context.Response.Write("0");
            }
            else
            {

                context.Response.Write(objCount.ToString());
            }
        }

        /// <summary>
        /// 查找某种社员商品的库存
        /// </summary>
        /// <param name="context"></param>
        void GetHQStorage(HttpContext context)
        {
            string GoodSupplyID = context.Request.QueryString["GoodSupplyID"].ToString();
            string strSql = "    SELECT numStore FROM dbo.GoodSupplyStorage WHERE GoodSupplyID="+GoodSupplyID+" AND ISHQ=1";
            object objCount = SQLHelper.ExecuteScalar(strSql);
            if (objCount == null || objCount.ToString() == "")
            {
                context.Response.Write("0");
            }
            else
            {

                context.Response.Write(objCount.ToString());
            }
        }


        /// <summary>
        /// 网点进货申请
        /// </summary>
        /// <param name="context"></param>
        void Add_GoodSupplyStockApply(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string HQWBID = SQLHelper.ExecuteScalar(" SELECT top 1 ID FROM dbo.WB WHERE ISHQ=1").ToString();//总部网店
            object objISSimulate = SQLHelper.ExecuteScalar("  SELECT TOP 1 ISSimulate FROM dbo.WB WHERE ID=" + WBID);
            bool ISSimulate = Convert.ToBoolean(objISSimulate);
            string GoodSupplyID = context.Request.Form["GoodIDSelect"].ToString();

            object objISHQ = SQLHelper.ExecuteScalar(" SELECT ISHQ  FROM dbo.WB WHERE ID=" + WBID);
            bool ISHQ = Convert.ToBoolean(objISHQ);
            string Price = context.Request.Form["Price"].ToString();
            string Price_WB = context.Request.Form["Price_WB"].ToString();
            string Price_WBBack = context.Request.Form["Price_WBBack"].ToString();
            string Quantity = context.Request.Form["Quantity"].ToString();

            double Price_Money = Math.Round(Convert.ToDouble(Price_WB) * Convert.ToDouble(Quantity), 2);
            bool ISCash = false;
            if (context.Request.Form["ISCash"] != null)
            {
                ISCash = true;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodSupplyApply] (");
            strSql.Append("WBID,ISHQ,GoodSupplyID,Price,Price_WB,Price_WBBack,Price_Money,Num_Stock,Quantity,ISCash,dt_Trade,numState)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@ISHQ,@GoodSupplyID,@Price,@Price_WB,@Price_WBBack,@Price_Money,@Num_Stock,@Quantity,@ISCash,@dt_Trade,@numState)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@ISHQ", SqlDbType.Int,4),
					new SqlParameter("@GoodSupplyID", SqlDbType.Int,4),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WB", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WBBack", SqlDbType.Decimal,9),
					new SqlParameter("@Price_Money", SqlDbType.Decimal,9),
					new SqlParameter("@Num_Stock", SqlDbType.Decimal,9),
					new SqlParameter("@Quantity", SqlDbType.Decimal,9),
					new SqlParameter("@ISCash", SqlDbType.Bit,1),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime),
                                 new SqlParameter("@numState", SqlDbType.Int,4)       };
            parameters[0].Value = WBID;
            parameters[1].Value = ISHQ;
            parameters[2].Value = GoodSupplyID;
            parameters[3].Value = Price;
            parameters[4].Value = Price_WB;
            parameters[5].Value = Price_WBBack;
            parameters[6].Value = Price_Money;
            parameters[7].Value = 0;
            parameters[8].Value = Quantity;
            parameters[9].Value = ISCash;
            parameters[10].Value = DateTime.Now;
            parameters[11].Value = 0;
         

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);
                   
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

        //添加网店进货信息
        //void Add_GoodSupplyStockWB(HttpContext context)
        //{
        //    string WBID = context.Session["WB_ID"].ToString();
        //    string HQWBID = SQLHelper.ExecuteScalar(" SELECT top 1 ID FROM dbo.WB WHERE ISHQ=1").ToString();//总部网店
        //    object objISSimulate = SQLHelper.ExecuteScalar("  SELECT TOP 1 ISSimulate FROM dbo.WB WHERE ID="+WBID);
        //    bool ISSimulate = Convert.ToBoolean(objISSimulate);
        //    string GoodSupplyID = context.Request.Form["GoodIDSelect"].ToString();
         
        //    object objISHQ = SQLHelper.ExecuteScalar(" SELECT ISHQ  FROM dbo.WB WHERE ID=" + WBID);
        //    bool ISHQ = Convert.ToBoolean(objISHQ);
        //    string Price = context.Request.Form["Price"].ToString();
        //    string Price_WB = context.Request.Form["Price_WB"].ToString();
        //    string Price_WBBack = context.Request.Form["Price_WBBack"].ToString();
        //    string Quantity = context.Request.Form["Quantity"].ToString();

        //    double Price_Money = Math.Round(Convert.ToDouble(Price_WB) * Convert.ToDouble(Quantity), 2);
        //    bool ISCash = false;
        //    if (context.Request.Form["ISCash"] != null)
        //    {
        //        ISCash = true;
        //    }
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("insert into [GoodSupplyStock] (");
        //    strSql.Append("WBID,ISHQ,GoodSupplyID,Price,Price_WB,Price_WBBack,Price_Money,Num_Stock,Quantity,ISCash,dt_Trade)");
        //    strSql.Append(" values (");
        //    strSql.Append("@WBID,@ISHQ,@GoodSupplyID,@Price,@Price_WB,@Price_WBBack,@Price_Money,@Num_Stock,@Quantity,@ISCash,@dt_Trade)");
        //    strSql.Append(";select @@IDENTITY");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@WBID", SqlDbType.Int,4),
        //            new SqlParameter("@ISHQ", SqlDbType.Int,4),
        //            new SqlParameter("@GoodSupplyID", SqlDbType.Int,4),
        //            new SqlParameter("@Price", SqlDbType.Decimal,9),
        //            new SqlParameter("@Price_WB", SqlDbType.Decimal,9),
        //            new SqlParameter("@Price_WBBack", SqlDbType.Decimal,9),
        //            new SqlParameter("@Price_Money", SqlDbType.Decimal,9),
        //            new SqlParameter("@Num_Stock", SqlDbType.Decimal,9),
        //            new SqlParameter("@Quantity", SqlDbType.Decimal,9),
        //            new SqlParameter("@ISCash", SqlDbType.Bit,1),
        //            new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
        //    parameters[0].Value = WBID;
        //    parameters[1].Value = ISHQ;
        //    parameters[2].Value = GoodSupplyID;
        //    parameters[3].Value = Price;
        //    parameters[4].Value = Price_WB;
        //    parameters[5].Value = Price_WBBack;
        //    parameters[6].Value = Price_Money;
        //    parameters[7].Value = 0;
        //    parameters[8].Value = Quantity;
        //    parameters[9].Value = ISCash;
        //    parameters[10].Value = DateTime.Now;

        //    //添加网店库存
        //    string strSqlAdd = "   UPDATE dbo.GoodSupplyStorage SET numStore=numStore+" + Quantity + " WHERE GoodSupplyID=" + GoodSupplyID + " and WBID=" + WBID;

        //    //减去总部库存
        //    string strSqlDel = "UPDATE dbo.GoodSupplyStorage SET numStore=numStore-" + Quantity + " WHERE GoodSupplyID=" + GoodSupplyID + " and WBID=" + HQWBID;


        //    //添加事务处理
        //    using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
        //    {
        //        try
        //        {

        //            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);//添加换存折记录
        //            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlAdd.ToString());
        //            if (!ISSimulate)//如果不是模拟账户则减去总部库存
        //            {
        //                SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDel.ToString());
        //            }
        //            tran.Commit();

        //            context.Response.Write("OK");
        //        }
        //        catch
        //        {
        //            tran.Rollback();
        //            context.Response.Write("Error");
        //        }
        //    }

           
        //}

        void Add_GoodSupplyStockWB(HttpContext context)
        {
            int ID=Convert.ToInt32( context.Request.QueryString["ID"]);
            int numState=Convert.ToInt32( context.Request.QueryString["numState"]);
            string strState = SQLHelper.ExecuteScalar("   SELECT numState FROM GoodSupplyApply WHERE ID="+ID).ToString();
            if (strState != "0") {
                context.Response.Write("H");//标志此一条记录已经经过申请
                return;
            }
            if(numState==-1)
            {
                 SQLHelper.ExecuteNonQuery("   UPDATE GoodSupplyApply SET numState=-1 WHERE ID="+ID);
                context.Response.Write("-1");
                return;
            }



            StringBuilder strSqlApply=new StringBuilder();
			strSqlApply.Append("select ID,WBID,ISHQ,GoodSupplyID,Price,Price_WB,Price_WBBack,Price_Money,Num_Stock,Quantity,ISCash,dt_Trade,numState ");
			strSqlApply.Append(" FROM [GoodSupplyApply] ");
			strSqlApply.Append(" where ID="+ID);
            DataTable dtApply=SQLHelper.ExecuteDataTable(strSqlApply.ToString());
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodSupplyStock] (");
            strSql.Append("WBID,ISHQ,GoodSupplyID,Price,Price_WB,Price_WBBack,Price_Money,Num_Stock,Quantity,ISCash,dt_Trade)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@ISHQ,@GoodSupplyID,@Price,@Price_WB,@Price_WBBack,@Price_Money,@Num_Stock,@Quantity,@ISCash,@dt_Trade)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@ISHQ", SqlDbType.Int,4),
					new SqlParameter("@GoodSupplyID", SqlDbType.Int,4),
					new SqlParameter("@Price", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WB", SqlDbType.Decimal,9),
					new SqlParameter("@Price_WBBack", SqlDbType.Decimal,9),
					new SqlParameter("@Price_Money", SqlDbType.Decimal,9),
					new SqlParameter("@Num_Stock", SqlDbType.Decimal,9),
					new SqlParameter("@Quantity", SqlDbType.Decimal,9),
					new SqlParameter("@ISCash", SqlDbType.Bit,1),
					new SqlParameter("@dt_Trade", SqlDbType.DateTime)};
            parameters[0].Value = dtApply.Rows[0]["WBID"];
            parameters[1].Value = dtApply.Rows[0]["ISHQ"];
            parameters[2].Value = dtApply.Rows[0]["GoodSupplyID"];
            parameters[3].Value = dtApply.Rows[0]["Price"];
            parameters[4].Value = dtApply.Rows[0]["Price_WB"];
            parameters[5].Value = dtApply.Rows[0]["Price_WBBack"];
            parameters[6].Value = dtApply.Rows[0]["Price_Money"];
            parameters[7].Value = 0;
            parameters[8].Value = dtApply.Rows[0]["Quantity"];
            parameters[9].Value = dtApply.Rows[0]["ISCash"];
            parameters[10].Value = DateTime.Now;

            double Quantity = Convert.ToDouble(dtApply.Rows[0]["Quantity"]);
            string WBID = dtApply.Rows[0]["WBID"].ToString();
            string HQWBID = SQLHelper.ExecuteScalar(" SELECT top 1 ID FROM dbo.WB WHERE ISHQ=1").ToString();//总部网店
            string GoodSupplyID = dtApply.Rows[0]["GoodSupplyID"].ToString();
            object objISSimulate = SQLHelper.ExecuteScalar("  SELECT TOP 1 ISSimulate FROM dbo.WB WHERE ID=" + WBID);
            bool ISSimulate = Convert.ToBoolean(objISSimulate);
            //添加网店库存
            string strSqlAdd = "   UPDATE dbo.GoodSupplyStorage SET numStore=numStore+" + Quantity + " WHERE GoodSupplyID=" + GoodSupplyID + " and WBID=" + WBID;

            //减去总部库存
            string strSqlDel = "UPDATE dbo.GoodSupplyStorage SET numStore=numStore-" + Quantity + " WHERE GoodSupplyID=" + GoodSupplyID + " and WBID=" + HQWBID;


            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran,CommandType.Text, "   UPDATE GoodSupplyApply SET numState=1 WHERE ID=" + ID);//更新申请状态

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);//添加换存折记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlAdd.ToString());
                    if (!ISSimulate)//如果不是模拟账户则减去总部库存
                    {
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDel.ToString());
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


        }

        void Update_GoodSupplyStockWB(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string WBID = context.Request.QueryString["WBID"].ToString();
            string GoodSupplyID = context.Request.QueryString["GoodSupplyID"].ToString();
            int QuanlityOriginal = Convert.ToInt32(context.Request.QueryString["QuanlityOriginal"]);
            int Quanlity = Convert.ToInt32(context.Request.QueryString["Quanlity"]);

            int QuanlityAdd = Quanlity - QuanlityOriginal;//商品修改增量
            double Price =Convert.ToDouble( SQLHelper.ExecuteScalar("    SELECT Price_WB FROM dbo.GoodSupplyStock WHERE ID="+ID));
            double Price_Money = Price * Quanlity;
            string UpdateGoodStock = " UPDATE dbo.GoodSupplyStock SET Price_Money="+Price_Money+",  Quantity=" + Quanlity + " WHERE ID=" + ID;

            string UpdateGoodStorage = " UPDATE  dbo.GoodSupplyStorage SET numStore=numStore+" + QuanlityAdd + " WHERE WBID=" + WBID + " AND GoodSupplyID=" + GoodSupplyID;

            //减去总部库存
            string HQWBID = SQLHelper.ExecuteScalar(" SELECT top 1 ID FROM dbo.WB WHERE ISHQ=1").ToString();//总部网店
            string strSqlDel = "";
            if (QuanlityAdd > 0)
            {
               strSqlDel= "UPDATE dbo.GoodSupplyStorage SET numStore=numStore-" + QuanlityAdd + " WHERE GoodSupplyID=" + GoodSupplyID + " and WBID=" + HQWBID;
            }
            else {
                strSqlDel = "UPDATE dbo.GoodSupplyStorage SET numStore=numStore+" +Math.Abs( QuanlityAdd )+ " WHERE GoodSupplyID=" + GoodSupplyID + " and WBID=" + HQWBID;
            }

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, UpdateGoodStock.ToString());//添加换存折记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, UpdateGoodStorage.ToString());
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDel.ToString());

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
        /// 获取社员商品的存储信息
        /// </summary>
        /// <param name="context"></param>
        void GetGoodSupplyStorageByID(HttpContext context)
        {
            //string GoodID = context.Request.QueryString["GoodID"].ToString();
            //string WBID = context.Session["WB_ID"].ToString();
            //StringBuilder strSql = new StringBuilder();
            //strSql.Append("    SELECT ID,WBSupplierID,WBWareHouseID,numStore,maxStore FROM dbo.GoodSupplyStorage");
            //strSql.Append("   WHERE WBID=" + WBID + " AND GoodSupplyID=" + GoodID);
            //DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            //if (dt != null && dt.Rows.Count != 0)
            //{
            //    context.Response.Write(JsonHelper.ToJson(dt));
            //}
            //else
            //{
            //    context.Response.Write("Error");
            //}

            string GoodID = context.Request.QueryString["GoodID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("    SELECT ID,WBSupplierID,WBWareHouseID,numStore,maxStore FROM dbo.GoodSupplyStorage");
            strSql.Append("   WHERE WBID=" + WBID + " AND GoodSupplyID=" + GoodID);
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

        void Add_GoodSupplyStorage(HttpContext context)
        {

            string GoodSupplyID = context.Request.QueryString["GoodSupplyID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();
            object objISHQ = SQLHelper.ExecuteScalar(" SELECT ISHQ  FROM dbo.WB WHERE ID="+WBID);
            bool ISHQ = Convert.ToBoolean(objISHQ);
           
            string WBSupplierID = context.Request.Form["WBSupplierID"].ToString();
            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();
            string maxStore = context.Request.Form["maxStore"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [GoodSupplyStorage] (");
            strSql.Append("GoodSupplyID,WBID,ISHQ,WBSupplierID,WBWareHouseID,numStore,maxStore)");
            strSql.Append(" values (");
            strSql.Append("@GoodSupplyID,@WBID,@ISHQ,@WBSupplierID,@WBWareHouseID,@numStore,@maxStore)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@GoodSupplyID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@ISHQ", SqlDbType.Bit,1),
                    new SqlParameter("@WBSupplierID", SqlDbType.Int,4),
					new SqlParameter("@WBWareHouseID", SqlDbType.Int,4),
					new SqlParameter("@numStore", SqlDbType.Decimal,9),
					new SqlParameter("@maxStore", SqlDbType.Decimal,9)};
            parameters[0].Value = GoodSupplyID;
            parameters[1].Value = WBID;
            parameters[2].Value = ISHQ;
            parameters[3].Value = WBSupplierID;
            parameters[4].Value = WBWareHouseID;
            parameters[5].Value = 0;
            parameters[6].Value = maxStore;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_GoodSupplyStorage(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();


            string WBSupplierID = context.Request.Form["WBSupplierID"].ToString();
            string WBWareHouseID = context.Request.Form["WBWareHouseID"].ToString();
            string maxStore = context.Request.Form["maxStore"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" UPDATE dbo.GoodSupplyStorage  SET WBSupplierID=" + WBSupplierID + ",WBWareHouseID=" + WBWareHouseID + ",maxStore=" + maxStore);

            strSql.Append(" where ID=" + ID);

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
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