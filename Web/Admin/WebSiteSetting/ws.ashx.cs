using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Web.Admin.WebSiteSetting
{
    /// <summary>
    /// ws 的摘要说明
    /// </summary>
    public class ws : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
           
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "getWBAuthority": getWBAuthority(context); break;
                    case "WBAuthority_Price": WBAuthority_Price(context); break;
                    case "WBAuthority_IS": WBAuthority_IS(context); break;
                    case "UpateWBAuthority": UpateWBAuthority(context); break;


                    case "getWB": getWB(context); break;
                    case "SetSimulate": SetSimulate(context); break;

                    case "getWBType": getWBType(context); break;
                    case "getWBTypeByID": getWBTypeByID(context); break;
                    case "UpdateWBType": UpdateWBType(context); break;
                    case "AddWBType": AddWBType(context); break;
                    case "DeleteWBTypeByID": DeleteWBTypeByID(context); break;
                    case "UpdateWB": UpdateWB(context); break;
                    case "GetWBByID": GetWBByID(context); break;
                    case "DeleteWBByID": DeleteWBByID(context); break;
                    case "SetHQ": SetHQ(context); break;

                        
                }
            }

        }

        void getWBAuthority(HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT TOP 1 * FROM dbo.WBAuthority");
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

        void WBAuthority_Price(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            int Price_Merge = 0;
            if (context.Request.Form["Price_Merge"] != null)
            {
                Price_Merge = 1;
            }
            int Price_Update = 0;
            if (context.Request.Form["Price_Update"] != null)
            {
                Price_Update = 1;
            }
            int Price_PrintOnPingZheng = 0;
            if (context.Request.Form["Price_PrintOnPingZheng"] != null)
            {
                Price_PrintOnPingZheng = 1;
            }
            int Price_PrintOnCunZhe = 0;
            if (context.Request.Form["Price_PrintOnCunZhe"] != null)
            {
                Price_PrintOnCunZhe = 1;
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("  UPDATE dbo.WBAuthority SET ");
            strSql.Append(" Price_Merge="+Price_Merge+",Price_Update="+Price_Update+",Price_PrintOnPingZheng="+Price_PrintOnPingZheng+",Price_PrintOnCunZhe="+Price_PrintOnCunZhe);
               strSql.Append(" WHERE ID="+ID);
           
            if (SQLHelper.ExecuteNonQuery(strSql.ToString())!= 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void WBAuthority_IS(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
           

            int IS_Extract = 0;
            if (context.Request.Form["IS_Extract"] != null)
            {
                IS_Extract = 1;
            }
            int IS_StorageToSell = 0;
            if (context.Request.Form["IS_StorageToSell"] != null)
            {
                IS_StorageToSell = 1;
            }
            int IS_QuZheng = 0;
            if (context.Request.Form["IS_QuZheng"] != null)
            {
                IS_QuZheng = 1;
            }
            int IS_ZhuanCun = 0;
            if (context.Request.Form["IS_ZhuanCun"] != null)
            {
                IS_ZhuanCun = 1;
            }
            int IS_TeJia = 0;
            if (context.Request.Form["IS_TeJia"] != null)
            {
                IS_TeJia = 1;
            }
            StringBuilder strSql = new StringBuilder();
            int IS_YouHui = 0;
            if (context.Request.Form["IS_YouHui"] != null)
            {
                IS_YouHui = 1;
                string YouHuiLimit = context.Request.Form["YouHuiLimit"].ToString();
                string YouHuiProp = context.Request.Form["YouHuiProp"].ToString();
                strSql.Append("  UPDATE dbo.WBAuthority SET ");
                strSql.Append(" IS_Extract=" + IS_Extract + ",IS_StorageToSell=" + IS_StorageToSell + ",IS_QuZheng=" + IS_QuZheng + ",IS_ZhuanCun=" + IS_ZhuanCun + ",IS_TeJia=" + IS_TeJia + ",IS_YouHui=" + IS_YouHui + ",YouHuiLimit=" + YouHuiLimit + ",YouHuiProp=" + YouHuiProp + "");
                strSql.Append(" WHERE ID=" + ID);
            }
            else {
                strSql.Append("  UPDATE dbo.WBAuthority SET ");
                strSql.Append(" IS_Extract=" + IS_Extract + ",IS_StorageToSell=" + IS_StorageToSell + ",IS_QuZheng=" + IS_QuZheng + ",IS_ZhuanCun=" + IS_ZhuanCun + ",IS_TeJia=" + IS_TeJia + ",IS_YouHui=" + IS_YouHui );
                strSql.Append(" WHERE ID=" + ID);
            }
           



          
          

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpateWBAuthority(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
           

            int VerifyType = 0;
            if (context.Request.Form["VerifyType"] != null)
            {
                VerifyType = 1;
            }
            int Enable_Distance = 0;
            if (context.Request.Form["Enable_Distance"] != null)
            {
                Enable_Distance = 1;
            }
            int VerifyDepInfo = 0;
            if (context.Request.Form["VerifyDepInfo"] != null)
            {
                VerifyDepInfo = 1;
            }

            int ISIntegral = 0;
            if (context.Request.Form["ISIntegral"] != null)
            {
                ISIntegral = 1;
            }
            double Integral = Convert.ToDouble(context.Request.Form["Integral"]);
            double Integral_StorageDep = Convert.ToDouble(context.Request.Form["Integral_StorageDep"]);
            double Integral_StorageRecommend = Convert.ToDouble(context.Request.Form["Integral_StorageRecommend"]);

            int ErrorLogin_Admin = 0;
            if (context.Request.Form["ErrorLogin_Admin"] != null)
            {
                ErrorLogin_Admin = 1;
            }
            int ErrorLogin_User = 0;
            if (context.Request.Form["ErrorLogin_User"] != null)
            {
                ErrorLogin_User = 1;
            }

            int ISPrintIDCard = 0;
            if (context.Request.Form["ISPrintIDCard"] != null)
            {
                ISPrintIDCard = 1;
            }
            int ISPrintPhoneNo = 0;
            if (context.Request.Form["ISPrintPhoneNo"] != null)
            {
                ISPrintPhoneNo = 1;
            }
            int Price_PrintOnCunZhe = 0;
            if (context.Request.Form["Price_PrintOnCunZhe"] != null)
            {
                Price_PrintOnCunZhe = 1;
            }
            int InterestType = Convert.ToInt32(context.Request.Form["InterestType"]);
            int ISCodekeyboard = 0;
            if (context.Request.Form["ISCodekeyboard"] != null)
            {
                ISCodekeyboard = 1;
            }
            int ISCurrentCal = 0;
            if (context.Request.Form["ISCurrentCal"] != null)
            {
                ISCurrentCal = 1;
            }
            int ISExchangeLimit = 0;
            if (context.Request.Form["ISExchangeLimit"] != null)
            {
                ISExchangeLimit = 1;
            }
            int numMinDay = 0;
            numMinDay = Convert.ToInt32(context.Request.Form["numMinDay"]);

            double exchangeGroupProp = 0;
            exchangeGroupProp = Convert.ToDouble(context.Request.Form["exchangeGroupProp"]);

            int exchangeGroupPeriod = 0;
            exchangeGroupPeriod = Convert.ToInt32(context.Request.Form["exchangeGroupPeriod"]);

            string strPricePolicy = "";
            strPricePolicy = context.Request.Form["strPricePolicy"].ToString();
           
            

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  UPDATE dbo.WBAuthority SET ");
            strSql.Append(" VerifyType=" + VerifyType );
            strSql.Append("  ,Enable_Distance=" + Enable_Distance);
            strSql.Append("  ,VerifyDepInfo=" + VerifyDepInfo);
            strSql.Append("  ,ISIntegral=" + ISIntegral);
            strSql.Append("  ,Integral=" + Integral);
            strSql.Append("  ,Integral_StorageDep=" + Integral_StorageDep);
            strSql.Append("  ,Integral_StorageRecommend=" + Integral_StorageRecommend);
            strSql.Append("  ,ErrorLogin_Admin=" + ErrorLogin_Admin);
            strSql.Append("  ,ErrorLogin_User=" + ErrorLogin_User);
            strSql.Append("  ,ISPrintIDCard=" + ISPrintIDCard);
            strSql.Append("  ,ISPrintPhoneNo=" + ISPrintPhoneNo);
            strSql.Append("  ,Price_PrintOnCunZhe=" + Price_PrintOnCunZhe);
            strSql.Append("  ,InterestType=" + InterestType);
            strSql.Append("  ,ISCodekeyboard=" + ISCodekeyboard);
            strSql.Append("  ,ISCurrentCal=" + ISCurrentCal);
            strSql.Append("  ,ISExchangeLimit=" + ISExchangeLimit);
            strSql.Append("  ,numMinDay=" + numMinDay);
            strSql.Append("  ,exchangeGroupProp=" + exchangeGroupProp);
            strSql.Append("  ,exchangeGroupPeriod=" + exchangeGroupPeriod);
            strSql.Append("  ,strPricePolicy='" + strPricePolicy + "'");
           
            strSql.Append(" WHERE ID=" + ID);

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void getWB(HttpContext context)
        {
            string strSql = "  SELECT ID,strName,ISHQ,ISSimulate FROM  WB ";
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

        void SetSimulate(HttpContext context)
        {
            string ISSimulate = context.Request.QueryString["ISSimulate"].ToString();
            string ID = context.Request.QueryString["ID"].ToString();
            //查询当前的网点是不是总部
            string strSqlQ = " SELECT top 1 ISHQ FROM dbo.WB WHERE ID="+ID;
            if (Convert.ToBoolean(SQLHelper.ExecuteScalar(strSqlQ)) == true)
            {
                context.Response.Write("HQ");
                return;
            }
            string strSql = "   UPDATE dbo.WB SET ISSimulate="+ISSimulate+" WHERE ID=" + ID;
          
            if (SQLHelper.ExecuteNonQuery(strSql)!= 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }




        void getWBTypeByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();
            string strSql = "  SELECT ID,SerialNumber,strType,strDescript FROM  WBType WHERE ID=" + wbid;
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

        void getWBType(HttpContext context)
        {
            
            string strSql = "  SELECT ID,SerialNumber,strType,strDescript FROM  WBType;";
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

        void AddWBType(HttpContext context)
        {
            string strType = context.Request.Form["strType"].ToString();
            string strDescript = context.Request.Form["strDescript"].ToString();
            if (!common.UniqueCheck_Add("WBType", "strType", strType))
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("  INSERT INTO dbo.WBType");
            sql.Append("  ( SerialNumber ,strType , strDescript , numSort)");
            sql.Append(string.Format("  VALUES  ( 0 ,  N'{0}' , N'{1}' , 0  )",strType,strDescript));
            if (SQLHelper.ExecuteNonQuery(sql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateWBType(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();
            string strType = context.Request.Form["strType"].ToString();
            if (!common.UniqueCheck_Update("WBType", "strType", strType, wbid))
            {
                context.Response.Write("1");
                return;
            }
            string strDescript = context.Request.Form["strDescript"].ToString();
            string strSql = "      UPDATE dbo.WBType SET strType='"+strType+"',strDescript='"+strDescript+"' WHERE ID=" + wbid;
           ;
            if (SQLHelper.ExecuteNonQuery(strSql)!=0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteWBTypeByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            //查询是否有用户使用
            int numUser = Convert.ToInt32(SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.WB WHERE WBType_ID=" + ID));
            if (numUser > 0)
            {
                //var res = new { state = "error", msg = "该类型已有网点记录，不可以删除!" };
                //context.Response.Write(JsonHelper.ToJson(res));
                context.Response.Write("Exit");
                return;
            }

            string strSql = " delete FROM dbo.WBType WHERE ID=" + ID;


            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
                //var res = new { state = "success", msg = "操作成功!" };
                //context.Response.Write(JsonHelper.ToJson(res));
                context.Response.Write("OK");
            }
            else
            {
                //var res = new { state = "error", msg = "数据操作失败!" };
                //context.Response.Write(JsonHelper.ToJson(res));
                context.Response.Write("Error");
            }
        }

        void UpdateWB(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string SerialNumber = context.Request.QueryString["sNumber"].ToString();
            if (wbid == "0") //新增网点时候，设置新的网点编号
            {
                SerialNumber = SQLHelper.ExecuteScalar("SELECT TOP 1 SerialNumber   FROM dbo.WB ORDER BY dt_Add DESC").ToString();
                int numSerial = Convert.ToInt32(SerialNumber) + 1;
                if (numSerial > 0 && numSerial < 10)
                {
                    SerialNumber = "00" + numSerial.ToString(); 
                }
                else if (numSerial >= 10 && numSerial < 100)
                {
                    SerialNumber = "0" + numSerial.ToString(); 
                }
                else { SerialNumber = numSerial.ToString(); }
            }

            string strName = context.Request.Form["strType"].ToString();

            if (wbid == "0")//新增网点检查是否有相同的网点名称呢
            {
                if (!common.UniqueCheck_Add("WB", "strName", strName))
                {
                    context.Response.Write("1");
                    return;
                }
            }
            else {
                if (!common.UniqueCheck_Update("WB", "strName", strName, wbid))
                {
                    context.Response.Write("1");
                    return;
                }
            }
            string strAddress = context.Request.Form["strAddress"].ToString();
            string numAgent = context.Request.Form["numAgent"].ToString();
            string numSettle = context.Request.Form["numSettle"].ToString();
            string numDay = context.Request.Form["numDay"].ToString();

            int draw_exchange = 0;
            if (context.Request.Form["draw_exchange"] != null)
            {
                draw_exchange = 1;
            }
            int draw_sell = 0;
            if (context.Request.Form["draw_sell"] != null)
            {
                draw_sell = 1;
            }
            int draw_shopping = 0;
            if (context.Request.Form["draw_shopping"] != null)
            {
                draw_shopping = 1;
            }
           
            int ISAllowBackUp = 0;
           
            int ISHQ = 0;

            string WBType_ID = context.Request.Form["WBType_ID"].ToString();


            if (wbid == "0")
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into [WB] (");
                strSql.Append("SerialNumber,strName,strAddress,WBType_ID,numSettle,numAgent,numDay,ISAllowBackUp,ISHQ,ISSimulate,dt_Add,dt_Update,draw_exchange,draw_sell,draw_shopping)");
                strSql.Append(" values (");
                strSql.Append("@SerialNumber,@strName,@strAddress,@WBType_ID,@numSettle,@numAgent,@numDay,@ISAllowBackUp,@ISHQ,@ISSimulate,@dt_Add,@dt_Update,@draw_exchange,@draw_sell,@draw_shopping)");
                strSql.Append(";select @@IDENTITY");
                SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@strAddress", SqlDbType.NVarChar,50),
					new SqlParameter("@WBType_ID", SqlDbType.Int,4),
					new SqlParameter("@numSettle", SqlDbType.Decimal,9),
					new SqlParameter("@numAgent", SqlDbType.Decimal,9),
					new SqlParameter("@numDay", SqlDbType.Int,4),
					new SqlParameter("@ISAllowBackUp", SqlDbType.Bit,1),
					new SqlParameter("@ISHQ", SqlDbType.Bit,1),
					new SqlParameter("@ISSimulate", SqlDbType.Bit,1),
					new SqlParameter("@dt_Add", SqlDbType.DateTime),
					new SqlParameter("@dt_Update", SqlDbType.NChar,10),
					new SqlParameter("@draw_exchange", SqlDbType.Bit,1),
					new SqlParameter("@draw_sell", SqlDbType.Bit,1),
					new SqlParameter("@draw_shopping", SqlDbType.Bit,1)};
                parameters[0].Value = SerialNumber;
                parameters[1].Value = strName;
                parameters[2].Value = strAddress;
                parameters[3].Value = WBType_ID;
                parameters[4].Value = numSettle;
                parameters[5].Value = numAgent;
                parameters[6].Value = numDay;
                parameters[7].Value = ISAllowBackUp;
                parameters[8].Value = 0;
                parameters[9].Value = 0;
                parameters[10].Value = DateTime.Now;
                parameters[11].Value = DateTime.Now;
                parameters[12].Value = draw_exchange;
                parameters[13].Value = draw_sell;
                parameters[14].Value = draw_shopping;

                //为网店初始化社员打印坐标
                StringBuilder strSqlCommune = new StringBuilder();
                strSqlCommune.Append("insert into [PrintSetting] (");
                strSqlCommune.Append("Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X)");
                strSqlCommune.Append(" select Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ");
                strSqlCommune.Append("  FROM dbo.PrintSetting WHERE WBID=0");


                //为网店初始化储户打印坐标
                StringBuilder strSqlDep = new StringBuilder();
                strSqlDep.Append("insert into [PrintSetting_Dep] (");
                strSqlDep.Append("Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X)");
                strSqlDep.Append(" select Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ");
                strSqlDep.Append("  FROM dbo.PrintSetting_Dep WHERE WBID=0");

                //添加事务处理
                using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
                {
                    try
                    {
                        object objWBID = SQLHelper.ExecuteScalar(tran, CommandType.Text, strSql.ToString(), parameters);
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlCommune.ToString());
                        string strSqlCommuneUpdate = " UPDATE dbo.PrintSetting SET WBID="+objWBID.ToString()+" WHERE ID=(SELECT MAX(ID) FROM dbo.PrintSetting)";
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlCommuneUpdate.ToString());


                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep.ToString());
                        string strSqlDepUpdate = " UPDATE dbo.PrintSetting_Dep SET WBID=" + objWBID.ToString() + " WHERE ID=(SELECT MAX(ID) FROM dbo.PrintSetting_Dep)";
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDepUpdate.ToString());


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
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update [WB] set ");
                strSql.Append("SerialNumber=@SerialNumber,");
                strSql.Append("strName=@strName,");
                strSql.Append("strAddress=@strAddress,");
                strSql.Append("WBType_ID=@WBType_ID,");
                strSql.Append("numSettle=@numSettle,");
                strSql.Append("numAgent=@numAgent,");
                strSql.Append("numDay=@numDay,");
                strSql.Append("ISAllowBackUp=@ISAllowBackUp,");
                strSql.Append("dt_Update=@dt_Update,");
                strSql.Append("draw_exchange=@draw_exchange,");
                strSql.Append("draw_sell=@draw_sell,");
                strSql.Append("draw_shopping=@draw_shopping");
                strSql.Append(" where ID=@ID ");
                SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@strAddress", SqlDbType.NVarChar,50),
					new SqlParameter("@WBType_ID", SqlDbType.Int,4),
					new SqlParameter("@numSettle", SqlDbType.Decimal,9),
					new SqlParameter("@numAgent", SqlDbType.Decimal,9),
                    new SqlParameter("@numDay", SqlDbType.Int,4),
					new SqlParameter("@ISAllowBackUp", SqlDbType.Bit,1),
					new SqlParameter("@dt_Update", SqlDbType.NChar,10),
					new SqlParameter("@ID", SqlDbType.Int,4),
					new SqlParameter("@draw_exchange", SqlDbType.Bit,1),
					new SqlParameter("@draw_sell", SqlDbType.Bit,1),
					new SqlParameter("@draw_shopping", SqlDbType.Bit,1)};
                parameters[0].Value = SerialNumber;
                parameters[1].Value = strName;
                parameters[2].Value = strAddress;
                parameters[3].Value = WBType_ID;
                parameters[4].Value = numSettle;
                parameters[5].Value = numAgent;
                parameters[6].Value = numDay;
                parameters[7].Value = ISAllowBackUp;
                parameters[8].Value = DateTime.Now;
                parameters[9].Value = wbid;
                parameters[10].Value = draw_exchange;
                parameters[11].Value = draw_sell;
                parameters[12].Value = draw_shopping;


                if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
                {
                    context.Response.Write("OK");
                }
                else
                {
                    context.Response.Write("Error");
                }

            }


            
        }

        void GetWBByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string strSql = " SELECT ID,SerialNumber,strName,strAddress,WBType_ID,numSettle,numAgent,numDay,ISAllowBackUp,ISHQ,draw_exchange,draw_sell,draw_shopping FROM dbo.WB WHERE ID=" + wbid;

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


        void DeleteWBByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            //查询是否有用户使用
            int numUser = Convert.ToInt32(SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Users WHERE WB_ID=" + ID));
            if (numUser > 0)
            {
                var res = new { state = "error", msg = "该网点已有营业员记录，不可以删除!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            string strSql = " delete FROM dbo.WB WHERE ID="+ID;


            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
                var res = new { state = "success", msg = "操作成功!" };
                context.Response.Write(JsonHelper.ToJson(res));

            }
            else
            {
                var res = new { state = "error", msg = "数据操作失败!" };
                context.Response.Write(JsonHelper.ToJson(res));

            }
        }

        void SetHQ(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //如果改网点为模拟网点则不允许设置
            string strSqlQ = " SELECT top 1 ISSimulate FROM dbo.WB WHERE ID=" + wbid;
            if (Convert.ToBoolean(SQLHelper.ExecuteScalar(strSqlQ)) == true)
            {
                context.Response.Write("S");
                return;
            }
            string strSqlHQ = "  SELECT TOP 1 ID FROM dbo.WB WHERE ISHQ=1";
            object obj = SQLHelper.ExecuteScalar(strSqlHQ);
            if (obj != null && obj != null)
            { //当前存在总部
                string strSql = " UPDATE dbo.WB SET ISHQ=0 WHERE ID!=" + wbid;
                string strSql2 = "  UPDATE dbo.WB SET ISHQ=1 WHERE ID=" + wbid;
                string strSql3 = "   UPDATE dbo.Users SET WB_ID="+wbid+" WHERE UserGroup_ID IN (2,3) AND WB_ID="+obj.ToString();

                //添加事务处理
                using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
                {
                    try
                    {
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql);
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql2);
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql3);
                        tran.Commit();
                        //重新设置Session
                        context.Session["WB_ID"] = wbid;
                        context.Response.Write("OK");
                    }
                    catch
                    {
                        tran.Rollback();
                        context.Response.Write("Error");
                    }
                }
                
               
            }
            else { //当前不存在总部
                string strSql = " UPDATE dbo.WB SET ISHQ=0 WHERE ID!=" + wbid;
              
                if (SQLHelper.ExecuteNonQuery(strSql) != 0)
                {
                    context.Response.Write("OK");
                }
                else
                {
                    context.Response.Write("Error");
                }
            }

            //string strSql = " UPDATE dbo.WB SET ISHQ=0 WHERE ID!=" + wbid;
            //strSql += "  UPDATE dbo.WB SET ISHQ=1 WHERE ID="+wbid;

           
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