using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;

namespace Web.User.Storage
{
    /// <summary>
    /// storage 的摘要说明
    /// </summary>
    public class storage : IHttpHandler,IRequiresSessionState
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

                    case "GetStorageVarietyUnitByID": GetStorageVarietyUnitByID(context); break;//获取存储产品的计量单位
                    case "GetWeighNO": GetWeighNO(context); break;
                    case "GetVarietyFromStorageRate": GetVarietyFromStorageRate(context); break;
                    case "GetVarietyLevelFromStorageRate": GetVarietyLevelFromStorageRate(context); break;
                    case "GetUnitByVarietyID": GetUnitByVarietyID(context); break;

                    case "GetTypeFromVarietyRate": GetTypeFromVarietyRate(context); break;
                    case "GetUserTypeByVarietyID": GetUserTypeByVarietyID(context); break;
                    case "GetTimeByVUID": GetTimeByVUID(context); break;
                    case "GetSotorageByVUTID": GetSotorageByVUTID(context); break;
                    case "ShowOptionInfo": ShowOptionInfo(context); break;

                    case "Add_Dep_Storage": Add_Dep_Storage(context); break;
                    case "Update_Dep_Storage": Update_Dep_Storage(context); break;
                    case "StorageTypeChange": StorageTypeChange(context); break;//存粮转存
                    case "Delete_Dep_Storage": Delete_Dep_Storage(context); break;
                        
                    case "GetByID_Dep_Storage": GetByID_Dep_Storage(context); break;
                    case "StorageVirtual": StorageVirtual(context); break;//预存转实存

                    case "GetDepositorPrint": GetDepositorPrint(context); break;

                    case "GetStorageInfoByID": GetStorageInfoByID(context); break;//获取单条存储信息

                    case "GetNewBusinessNO": GetNewBusinessNO(context); break;

                    case "JieXi": JieXi(context); break;
                }
            }

        }


        /// <summary>
        /// 获取单条存储信息
        /// </summary>
        /// <param name="context"></param>
        void GetStorageInfoByID(HttpContext context)
        {
            string ID=context.Request.Form["ID"].ToString();
            DataTable dt = SQLHelper.ExecuteDataTable(" SELECT * FROM dbo.Dep_StorageInfo WHERE ID="+ID);
            context.Response.Write(JsonHelper.ToJson(dt));
        }

        /// <summary>
        /// 获取新的业务编号
        /// </summary>
        /// <param name="context"></param>
        void GetNewBusinessNO(HttpContext context)
        {

            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            string BusinessNO= common.GetNewBusinessNO_Dep(AccountNumber);     
            context.Response.Write(BusinessNO);
        }


        void JieXi(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = context.Session["ID"].ToString();
            string dsiID = context.Request.Form["dsiID"].ToString();
            string JiexiType = context.Request.Form["JiexiType"].ToString();//转存类型(1:仅结息，2：结息后转存)
            string numInterest = context.Request.Form["numInterest"].ToString();//利息         
            string TypeID = context.Request.Form["txtTypeID"].ToString();//存储类型
            string TimeID = context.Request.Form["txtTimeID"].ToString();//存期

            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("    SELECT top 1  A.ID,A.AccountNumber,A.TypeID,A.TimeID,B.strName AS Dep_Name, VarietyID,VarietyLevelID,StorageNumber, StorageNumberRaw,Price_ShiChang,Price_DaoQi");
            strSqlStorage.Append("    ,A.InterestDate,A.CurrentRate, A.StorageDate,DATEDIFF( Day, A.StorageDate,GETDATE())AS daycount,C.strName AS VarietyName,D.strName AS UnitName");
            strSqlStorage.Append("    ,E.InterestType,E.numStorageDate");
            strSqlStorage.Append("   FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSqlStorage.Append("   INNER JOIN dbo.StorageTime E ON A.TimeID=E.ID");
            strSqlStorage.Append("    LEFT JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSqlStorage.Append("     LEFT JOIN dbo.BD_MeasuringUnit D ON C.MeasuringUnitID=D.ID");
            strSqlStorage.Append("  WHERE A.ID=" + dsiID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
            if (dtStorage == null || dtStorage.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "获取储户的存储信息错误!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string AccountNumber = dtStorage.Rows[0]["AccountNumber"].ToString();//储户账号
            string Dep_Name = dtStorage.Rows[0]["Dep_Name"].ToString();//储户账号
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号
            string VarietyID = dtStorage.Rows[0]["VarietyID"].ToString();//存储产品
            string VarietyLevelID = dtStorage.Rows[0]["VarietyLevelID"].ToString();//存储产品
            string VarietyName = dtStorage.Rows[0]["VarietyName"].ToString();
              string UnitName= dtStorage.Rows[0]["UnitName"].ToString();

            int InterestType = Convert.ToInt32(dtStorage.Rows[0]["InterestType"]);//利息计算方式
            double StorageNumber = Convert.ToDouble(dtStorage.Rows[0]["StorageNumber"]);//存储数量
            DateTime StorageDate = Convert.ToDateTime(dtStorage.Rows[0]["StorageDate"]);//存入日期
            DateTime InterestDate = Convert.ToDateTime(dtStorage.Rows[0]["InterestDate"]);
            TimeSpan tsStorage = DateTime.Now.Subtract(StorageDate);
            TimeSpan tsInterest = DateTime.Now.Subtract(InterestDate);
            int numStorageDate = Convert.ToInt32(dtStorage.Rows[0]["numStorageDate"]);//约定存储时间
            double CurrentRate = Convert.ToDouble(dtStorage.Rows[0]["CurrentRate"]);//活期利率
            double Price_ShiChang = Convert.ToDouble(dtStorage.Rows[0]["Price_ShiChang"]);//市场价
            double Price_DaoQi = Convert.ToDouble(dtStorage.Rows[0]["Price_DaoQi"]);//到期价
            double Price_ShiChangNow = Price_ShiChang;//当前市场的价格

           

            StringBuilder strSql_newStorage = new StringBuilder();

            StringBuilder strSqlPrice = new StringBuilder();
            strSqlPrice.Append(" select ID,TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,");
            strSqlPrice.Append(" CurrentRate,EarningRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou ");
            strSqlPrice.Append(" FROM  dbo.StorageRate");
            strSqlPrice.Append("  WHERE VarietyID= " + VarietyID + " and TypeID=" + TypeID + " and TimeID=" + TimeID);
            DataTable dtPrice = SQLHelper.ExecuteDataTable(strSqlPrice.ToString());
            if (dtPrice == null || dtPrice.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "获取新的存储价格与利率失败!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }
            string InterestDate_new = DateTime.Now.ToString();
            string StorageRateID_new = dtPrice.Rows[0]["ID"].ToString();
            string CurrentRate_new = dtPrice.Rows[0]["CurrentRate"].ToString();
            string Price_ShiChang_new = dtPrice.Rows[0]["Price_ShiChang"].ToString();
            Price_ShiChangNow = Convert.ToDouble(Price_ShiChang_new);
            string Price_DaoQi_new = dtPrice.Rows[0]["Price_DaoQi"].ToString();
            string Price_HeTong_new = dtPrice.Rows[0]["Price_HeTong"].ToString();
            string StorageFee_new = dtPrice.Rows[0]["StorageFee"].ToString();
            string StorageRateID = dtPrice.Rows[0]["ID"].ToString();

            //修改结息前的存储记录
            StringBuilder strSqlUpdate = new StringBuilder();

            if (JiexiType == "1") {//不结息转存，待添加
            }
            else if (JiexiType == "2") {
                strSqlUpdate.Append("   UPDATE dbo.Dep_StorageInfo SET");
                strSqlUpdate.Append("   InterestDate='" + InterestDate_new + "',");
                strSqlUpdate.Append("   StorageNumber=0");
                strSqlUpdate.Append("  WHERE ID=" + dsiID);
            }

            //添加新的存储记录
            strSql_newStorage.Append("insert into [Dep_StorageInfo] (");
            strSql_newStorage.Append("BusinessNO,AccountNumber,StorageRateID,VarietyID,VarietyLevelID,TypeID,TimeID,InterestDate,StorageDate,WeighNo,StorageNumber,StorageNumberRaw,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong,UserID,WBID)");
            strSql_newStorage.Append(" values (");
            strSql_newStorage.Append("@BusinessNO,@AccountNumber,@StorageRateID,@VarietyID,@VarietyLevelID,@TypeID,@TimeID,@InterestDate,@StorageDate,@WeighNo,@StorageNumber,@StorageNumberRaw,@StorageFee,@CurrentRate,@Price_ShiChang,@Price_DaoQi,@Price_HeTong,@UserID,@WBID)");
            strSql_newStorage.Append(";select @@IDENTITY");
            SqlParameter[] parameters_newStorage = {
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageRateID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
                    	new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@InterestDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@WeighNo", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageNumber", SqlDbType.Decimal,9),
					new SqlParameter("@StorageNumberRaw", SqlDbType.Decimal,9),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
					new SqlParameter("@UserID", SqlDbType.Int,4),
                    new SqlParameter("@WBID", SqlDbType.Int,4)};
            parameters_newStorage[0].Value = BusinessNO;
            parameters_newStorage[1].Value = AccountNumber;
            parameters_newStorage[2].Value = StorageRateID;
            parameters_newStorage[3].Value = VarietyID;
            parameters_newStorage[4].Value = VarietyLevelID;
            parameters_newStorage[5].Value = TypeID;
            parameters_newStorage[6].Value = TimeID;
            parameters_newStorage[7].Value = DateTime.Now;
            parameters_newStorage[8].Value = DateTime.Now;
            parameters_newStorage[9].Value = "";//结息的过磅号为‘’
            parameters_newStorage[10].Value = StorageNumber;
            parameters_newStorage[11].Value = StorageNumber;
            parameters_newStorage[12].Value = StorageFee_new;
            parameters_newStorage[13].Value = CurrentRate_new;
           // parameters_newStorage[13].Value = Price_ShiChang_new;
            parameters_newStorage[14].Value = Price_ShiChang;
            parameters_newStorage[15].Value = Price_DaoQi_new;
            parameters_newStorage[16].Value = Price_HeTong_new;
            parameters_newStorage[17].Value = UserID;
            parameters_newStorage[18].Value = WBID;



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
            parametersInterest[5].Value = "结息";
            parametersInterest[6].Value = dsiID;
            parametersInterest[7].Value = VarietyID;
            parametersInterest[8].Value = 0;
            parametersInterest[9].Value = JiexiType;
            parametersInterest[10].Value = TypeID;
            parametersInterest[11].Value = TimeID;
            parametersInterest[12].Value = DateTime.Now;
            parametersInterest[13].Value = numInterest;




            //添加储户操作记录

            //添加交易记录
   
            double Count_Balance = common.GetDep_StorageNumber(AccountNumber, VarietyID);//储户总结存
         
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("insert into [Dep_OperateLog] (");
            strSqlLog.Append("WBID,UserID,Dep_AccountNumber,BusinessNO,BusinessName,VarietyID,UnitID,Price,GoodCount,Count_Trade,Money_Trade,Count_Balance,dt_Trade,VarietyName,UnitName,Dep_SID)");
            strSqlLog.Append(" values (");
            strSqlLog.Append("@WBID,@UserID,@Dep_AccountNumber,@BusinessNO,@BusinessName,@VarietyID,@UnitID,@Price,@GoodCount,@Count_Trade,@Money_Trade,@Count_Balance,@dt_Trade,@VarietyName,@UnitName,@Dep_SID)");
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
					new SqlParameter("@dt_Trade", SqlDbType.DateTime),
					new SqlParameter("@VarietyName", SqlDbType.NVarChar,50),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
                    new SqlParameter("@Dep_SID", SqlDbType.Int,4)};
            parametersLog[0].Value = WBID;
            parametersLog[1].Value = UserID;
            parametersLog[2].Value = AccountNumber;
            parametersLog[3].Value = BusinessNO;
            parametersLog[4].Value = "11";//11:结息
            parametersLog[5].Value = VarietyID;
            parametersLog[6].Value = UnitName;
            parametersLog[7].Value = Price_ShiChangNow;
            parametersLog[8].Value = StorageNumber;
            parametersLog[9].Value = StorageNumber;
            double Money_Trade = Math.Round(StorageNumber * Price_ShiChangNow, 2);
            parametersLog[10].Value = Money_Trade;
            parametersLog[11].Value = Count_Balance;
            parametersLog[12].Value = DateTime.Now;
            parametersLog[13].Value = VarietyName;
            parametersLog[14].Value = UnitName;
            parametersLog[15].Value = dsiID;
            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    if (strSql_newStorage.ToString().Trim() != "") {
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql_newStorage.ToString(), parameters_newStorage);
                    }

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlUpdate.ToString());
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlInterest.ToString(), parametersInterest);
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlLog.ToString(), parametersLog);//添加结息日志记录
                    tran.Commit();
                    var res = new { state = "success", msg = "储户结息成功!", BusinessNO = BusinessNO };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "error", msg = "数据库执行错误!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }
            }
        }
        /// <summary>
        /// 获取储户的打印信息
        /// </summary>
        /// <param name="context"></param>
        void GetDepositorPrint(HttpContext context)
        {
            //获取需要打印的信息
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            StringBuilder strSqlCommume = new StringBuilder();
            strSqlCommume.Append("SELECT A.AccountNumber,A.strName,A.strAddress,A.IDCard,B.strName AS WBID,'密码' AS OperateWay, PhoneNO, CONVERT(NVARCHAR(100),A.dt_Add,23) AS dt_Add");
            strSqlCommume.Append(" FROM dbo.Depositor A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlCommume.Append(" WHERE A.AccountNumber='"+AccountNumber+"'");
           

            DataTable dtCommune = SQLHelper.ExecuteDataTable(strSqlCommume.ToString());
            if (dtCommune == null || dtCommune.Rows.Count == 0)
            {
                context.Response.Write("");
                return;
            }
            DataRow rowWBWBAuthority = common.GetWBAuthority();
            string strName = dtCommune.Rows[0]["strName"].ToString();
            string strAddress = dtCommune.Rows[0]["strAddress"].ToString();
            string IDCard = dtCommune.Rows[0]["IDCard"].ToString();
            string WBID = dtCommune.Rows[0]["WBID"].ToString();
            string PhoneNO = dtCommune.Rows[0]["PhoneNO"].ToString();
            string dt_Commune = dtCommune.Rows[0]["dt_Add"].ToString();
            string OperateWay = dtCommune.Rows[0]["OperateWay"].ToString();

            if (Convert.ToBoolean(rowWBWBAuthority["ISPrintIDCard"]) == false)
            {
                IDCard = "******";
            }

            if (Convert.ToBoolean(rowWBWBAuthority["ISPrintPhoneNo"]) == false)
            {
                PhoneNO = "******";
            }

            string strWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += "  where 1=1 and WBID="+strWBID;

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
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;'>"+IDCard+"</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR4C1Y) - Convert.ToInt32(HomeR3C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR3C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR3C2X) - Convert.ToInt32(HomeR3C1X)).ToString() + "px;'>" + WBID + "</td>");
                strReturn.Append("  <td style='font-size:14px; font-weight:bold;'>"+OperateWay+"</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR5C1Y) - Convert.ToInt32(HomeR4C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR4C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR4C2X) - Convert.ToInt32(HomeR4C1X)).ToString() + "px;'>" + PhoneNO + "</td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;' >" + dt_Commune + "</td>");

                strReturn.Append("   </tr></table>");

                //是否打印价格政策
                if (Convert.ToBoolean(rowWBWBAuthority["Price_PrintOnCunZhe"]) == true)
                {


                    //strReturn.Append("  <div style='font-size:12px; font-weight:bold;text-align:center; height:" + (Convert.ToInt32(HomeR5C1Y) - Convert.ToInt32(HomeR4C1Y)).ToString() + "px'>");
                    strReturn.Append("  <div style='font-size:12px; font-weight:bold;padding-left:50px; height:" + (Convert.ToInt32(HomeR5C1Y) - Convert.ToInt32(HomeR4C1Y)).ToString() + "px'>");
                    strReturn.Append("<p>" + rowWBWBAuthority["strPricePolicy"] + "</p>");

                    strReturn.Append("  </div>");
                }


                context.Response.Write(strReturn.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        

        /// <summary>
        /// 获取存储产品的计量单位
        /// </summary>
        /// <param name="context"></param>
        void GetStorageVarietyUnitByID(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT TOP 1 B.strName");
            strSql.Append("  FROM dbo.StorageVariety A INNER JOIN dbo.BD_MeasuringUnit B ON A.MeasuringUnitID=B.ID");
            strSql.Append("  WHERE A.ID=" + VarietyID);

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


        /// <summary>
        /// 获取磅单编号(许修改)
        /// </summary>
        /// <param name="context"></param>
        void GetWeighNO(HttpContext context)
        {
            string strYear = Fun.ConvertIntToString(DateTime.Now.Year, 4);
            string strMonth = Fun.ConvertIntToString(DateTime.Now.Month, 2);
            string strDay = Fun.ConvertIntToString(DateTime.Now.Day, 2);
            string strWeighNO = strYear + strMonth + strDay + "0000600001";
            context.Response.Write(strWeighNO);
        }

        /// <summary>
        /// 获取存储利率表中的存储产品信息
        /// </summary>
        /// <param name="context"></param>
        void GetVarietyFromStorageRate(HttpContext context)
        {
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT DISTINCT B.ID,B.strName");
            strSql.Append(" FROM dbo.StorageRate A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            //add 20170801
            string WBID = context.Session["WB_ID"].ToString();
            strSql.Append("  INNER JOIN StorageRate_WB SW ON A.ID=SW.StorageRateID");
            strSql.Append( string.Format("  WHERE SW.WBID={0}",WBID));
            //end add
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null&&dt.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        /// <summary>
        /// 获取存储利率表中的存储产品等级信息
        /// </summary>
        /// <param name="context"></param>
        void GetVarietyLevelFromStorageRate(HttpContext context)
        {
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT DISTINCT B.ID,B.strName");
            strSql.Append(" FROM dbo.StorageRate A INNER JOIN dbo.StorageVarietyLevel_B B ON A.VarietyLevelID=B.ID");
            //add 20170801
            string WBID = context.Session["WB_ID"].ToString();
            strSql.Append("  INNER JOIN StorageRate_WB SW ON A.ID=SW.StorageRateID");
            strSql.Append(string.Format("  WHERE SW.WBID={0}", WBID));
            strSql.Append(string.Format("  and A.VarietyID={0}", VarietyID));
            //end add
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
        /// 根据存储产品ID获取存储产品的单位
        /// </summary>
        /// <param name="context"></param>
        void GetUnitByVarietyID(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString(); 
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.strName");
            strSql.Append(" FROM dbo.StorageVariety A INNER JOIN dbo.BD_MeasuringUnit B ON A.MeasuringUnitID=B.ID");
            strSql.Append("  WHERE A.ID= " + VarietyID);
            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
            if (obj!=null)
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetTypeFromVarietyRate(HttpContext context)
        {
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT  DISTINCT B.ID,B.strName");
            strSql.Append(" FROM  dbo.StorageRate A INNER JOIN dbo.StorageType B ON A.TypeID=B.ID");
         
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
        /// 根据存储产品ID获取与之关联的储户类型(storageRate表)
        /// </summary>
        /// <param name="context"></param>
        void GetUserTypeByVarietyID(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            string VarietyLevelID = context.Request.QueryString["VarietyLevelID"].ToString(); 
            //StringBuilder strSql = new StringBuilder();
            //strSql.Append(" SELECT  DISTINCT B.ID,B.strName");
            //strSql.Append(" FROM  dbo.StorageRate A INNER JOIN dbo.StorageType B ON A.TypeID=B.ID");
            //strSql.Append("  WHERE VarietyID= " + VarietyID);

            //edit 20170801
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT  DISTINCT B.ID,B.strName");
            strSql.Append(" FROM  dbo.StorageRate A INNER JOIN dbo.StorageType B ON A.TypeID=B.ID");
            strSql.Append(" INNER JOIN StorageRate_WB SW ON A.ID=SW.StorageRateID");
            strSql.Append("  WHERE VarietyID= " + VarietyID);
            strSql.Append(" and VarietyLevelID=" + VarietyLevelID);
            strSql.Append(" and SW.WBID="+WBID);
            //edit end
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null&&dt.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 根据存储产品ID和储户类型获取与之关联的存期类型(storageRate表)
        /// </summary>
        /// <param name="context"></param>
        void GetTimeByVUID(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            string VarietyLevelID = context.Request.QueryString["VarietyLevelID"].ToString();
            string TypeID = context.Request.QueryString["TypeID"].ToString(); 
            //StringBuilder strSql = new StringBuilder();
            //strSql.Append(" SELECT  DISTINCT B.ID,B.strName,B.PricePolicy");
            //strSql.Append(" FROM  dbo.StorageRate A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            //strSql.Append("  WHERE A.VarietyID= " + VarietyID + " and A.TypeID=" + TypeID);

            //edit 20170801
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT  DISTINCT B.ID,B.strName,B.PricePolicy");
            strSql.Append(" FROM  dbo.StorageRate A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" INNER JOIN StorageRate_WB SW ON A.ID=SW.StorageRateID");
            strSql.Append("  WHERE A.VarietyID= " + VarietyID + " and A.TypeID=" + TypeID);
            strSql.Append("  and A.VarietyLevelID= " + VarietyLevelID);
            strSql.Append(" and SW.WBID=" + WBID);
            //edit end
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
        /// 根据存储产品ID\、户类型获、存期类型取与之关联的存储利率信息(storageRate表)
        /// </summary>
        /// <param name="context"></param>
        void GetSotorageByVUTID(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            string VarietyLevelID = context.Request.QueryString["VarietyLevelID"].ToString();
            string TypeID = context.Request.QueryString["TypeID"].ToString();
            string TimeID = context.Request.QueryString["TimeID"].ToString(); 
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select ID,TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,");
            strSql.Append(" CurrentRate,EarningRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou ");
            strSql.Append(" FROM  dbo.StorageRate");
            strSql.Append("  WHERE VarietyID= " + VarietyID + " and VarietyLevelID=" + VarietyLevelID + " and TypeID=" + TypeID + " and TimeID=" + TimeID);
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
        /// 根据存储产品ID\、户类型获、存期类型取与之关联的存储利率信息(storageRate表)
        /// </summary>
        /// <param name="context"></param>
        void ShowOptionInfo(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            string VarietyLevelID = context.Request.QueryString["VarietyLevelID"].ToString();
            string TypeID = context.Request.QueryString["TypeID"].ToString();
            string TimeID = context.Request.QueryString["TimeID"].ToString();

            string Price_ShiChang_Cunru = "";//存粮时为“”，结息时存在
            if (context.Request.QueryString["Price_ShiChang"] != null)
            {
                Price_ShiChang_Cunru = context.Request.QueryString["Price_ShiChang"].ToString();
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select ID,TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,");
            strSql.Append(" CurrentRate,EarningRate,LossRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou ");
            strSql.Append(" FROM  dbo.StorageRate");
            strSql.Append("  WHERE VarietyID= " + VarietyID + " and VarietyLevelID=" + VarietyLevelID + " and TypeID=" + TypeID + " and TimeID=" + TimeID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                string Price_ShiChang = dt.Rows[0]["Price_ShiChang"].ToString();
                string Price_DaoQi = dt.Rows[0]["Price_DaoQi"].ToString();
                string Price_HeTong = dt.Rows[0]["Price_HeTong"].ToString();
                string CurrentRate = dt.Rows[0]["CurrentRate"].ToString();
                string EarningRate = dt.Rows[0]["EarningRate"].ToString();
                string LossRate = dt.Rows[0]["LossRate"].ToString();
                //获取存储产品的计量单位
                string strUnit = "";
                StringBuilder strSqlUnit = new StringBuilder();
                strSqlUnit.Append("  SELECT TOP 1 B.strName");
                strSqlUnit.Append("  FROM dbo.StorageVariety A INNER JOIN dbo.BD_MeasuringUnit B ON A.MeasuringUnitID=B.ID");
                strSqlUnit.Append("  WHERE A.ID=" + VarietyID);

                object obj = SQLHelper.ExecuteScalar(strSqlUnit.ToString());
                if (obj != null)
                {
                    strUnit = obj.ToString();
                }
                //查询当前选项的结息方式
                StringBuilder strSqlTime = new StringBuilder();
                strSqlTime.Append(" SELECT strName,PricePolicy, InterestType,numStorageDate");
                strSqlTime.Append(" FROM  dbo.StorageTime");
                strSqlTime.Append(" WHERE ID=" + TimeID);
                DataTable dtTime = SQLHelper.ExecuteDataTable(strSqlTime.ToString());
                string strReturn = "";
                if (dtTime != null && dtTime.Rows.Count != 0)
                {
                    string strName = dtTime.Rows[0]["strName"].ToString();
                    string PricePolicy = dtTime.Rows[0]["PricePolicy"].ToString();
                    string InterestType = dtTime.Rows[0]["InterestType"].ToString();
                 
                    string numStorageDate = dtTime.Rows[0]["numStorageDate"].ToString();
                    if (Price_ShiChang_Cunru == "")
                    {
                        strReturn += "当前选项: 存期：<b>" + strName + "</b>  存入价：<b>" + Price_ShiChang + "</b>元/" + strUnit;
                    }
                    else {
                        strReturn += "当前选项: 存期：<b>" + strName + "</b>  存入价：<b>" + Price_ShiChang_Cunru + "</b>元/" + strUnit;
                    }

                    switch (InterestType)
                    {
                        case "1": strReturn += ",活期利率：<b>" + CurrentRate + "</b>元/" + strUnit + "/月";
                            break;
                        case "2": strReturn += ",约定存储期限：<b>" + numStorageDate + "</b>天,到期受益比例：<b>" + EarningRate + "%</b>,到期亏损承担比例：<b>" + LossRate + "%</b>";
                            break;
                        case "3": strReturn += ",约定存储期限：<b>" + numStorageDate + "</b>天,到期价：<b>" + Price_DaoQi + "</b>元/" + strUnit;
                            break;
                        case "4": strReturn += ",约定存储期限：<b>" + numStorageDate + "</b>天,合同价：<b>" + Price_HeTong + "</b>元/" + strUnit;
                            break;
                    }
                    strReturn += "  请注意存储产品与存期，并核对账号与数量。";
                }

                context.Response.Write(strReturn.ToString());
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
        void Add_Dep_Storage(HttpContext context)
        {
           // string BusinessNO = context.Request.Form["BusinessNO"].ToString();//交易号

            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString().Trim();
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号
            string StorageRateID = context.Request.Form["StorageRateID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string VarietyLevelID = context.Request.Form["VarietyLevelID"].ToString();
            string TypeID = context.Request.Form["TypeID"].ToString();

            string TimeID = context.Request.Form["TimeID"].ToString();
            string StorageFee = context.Request.Form["StorageFee"].ToString();
            string WeighNo = context.Request.Form["WeighNO"].ToString();
            string StorageNumber = context.Request.Form["StorageNumber"].ToString();
            string strISVirtual = "0";
            if (context.Request.Form["ISVirtual"] != null)
            {
                strISVirtual = context.Request.Form["ISVirtual"].ToString();
            }
            bool ISVirtual = false;
            if (strISVirtual == "1")
            {
                ISVirtual = true;
            }

            string CurrentRate = "";
            string Price_ShiChang = "";
            string Price_DaoQi = "";
            string Price_HeTong = "";

            //由价格与利率表获取当前的价格信息
            StringBuilder strSqlPrice = new StringBuilder();
            strSqlPrice.Append(" select ID,TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,");
            strSqlPrice.Append(" CurrentRate,EarningRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou ");
            strSqlPrice.Append(" FROM  dbo.StorageRate");
            strSqlPrice.Append("  WHERE VarietyID= " + VarietyID + " and VarietyLevelID=" + VarietyLevelID + " and TypeID=" + TypeID + " and TimeID=" + TimeID);
            DataTable dtPrice = SQLHelper.ExecuteDataTable(strSqlPrice.ToString());
            if (dtPrice != null && dtPrice.Rows.Count != 0)
            {
                CurrentRate = dtPrice.Rows[0]["CurrentRate"].ToString();
                Price_ShiChang = dtPrice.Rows[0]["Price_ShiChang"].ToString();
                Price_DaoQi = dtPrice.Rows[0]["Price_DaoQi"].ToString();
                Price_HeTong = dtPrice.Rows[0]["Price_HeTong"].ToString();
            }
            else {
                context.Response.Write("Price");
                return;
            }

            string UserID = context.Session["ID"].ToString();
            string WBID = context.Session["WB_ID"].ToString();


            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Dep_StorageInfo] (");
            strSql.Append("BusinessNO,AccountNumber,StorageRateID,VarietyID,VarietyLevelID,TypeID,TimeID,InterestDate,StorageDate,WeighNo,StorageNumber,StorageNumberRaw,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong,UserID,WBID,ISVirtual)");
            strSql.Append(" values (");
            strSql.Append("@BusinessNO,@AccountNumber,@StorageRateID,@VarietyID,@VarietyLevelID,@TypeID,@TimeID,@InterestDate,@StorageDate,@WeighNo,@StorageNumber,@StorageNumberRaw,@StorageFee,@CurrentRate,@Price_ShiChang,@Price_DaoQi,@Price_HeTong,@UserID,@WBID,@ISVirtual)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@BusinessNO", SqlDbType.NVarChar,50),
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageRateID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
                    new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@InterestDate", SqlDbType.DateTime),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@WeighNo", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageNumber", SqlDbType.Decimal,9),
					new SqlParameter("@StorageNumberRaw", SqlDbType.Decimal,9),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
					new SqlParameter("@UserID", SqlDbType.Int,4),
                    new SqlParameter("@WBID", SqlDbType.Int,4),
                    new SqlParameter("@ISVirtual", SqlDbType.Bit,2)};
            parameters[0].Value = BusinessNO;
            parameters[1].Value = AccountNumber;
            parameters[2].Value = StorageRateID;
            parameters[3].Value = VarietyID;
            parameters[4].Value = VarietyLevelID;
            parameters[5].Value = TypeID;
            parameters[6].Value = TimeID;
            parameters[7].Value = DateTime.Now;
            parameters[8].Value = DateTime.Now;
            parameters[9].Value = WeighNo;
            parameters[10].Value = StorageNumber;
            parameters[11].Value = StorageNumber;
            parameters[12].Value = StorageFee;
            parameters[13].Value = CurrentRate;
            parameters[14].Value = Price_ShiChang;
            parameters[15].Value = Price_DaoQi;
            parameters[16].Value = Price_HeTong;
            parameters[17].Value = UserID;
            parameters[18].Value = WBID;
            parameters[19].Value = ISVirtual;
          

         

            //添加交易记录     
            string Price = Price_ShiChang;//价格
            string Count_Trade = StorageNumber;//存储数量
            string Money_Trade = "0";//存入的时候没有交易量
            double Count_Balance = common.GetDep_StorageNumber(AccountNumber, VarietyID);//储户总结存
            Count_Balance = Count_Balance + Convert.ToDouble(StorageNumber);

            string VarietyName = SQLHelper.ExecuteScalar("  SELECT strName FROM dbo.StorageVariety WHERE ID=" + VarietyID).ToString();

            string TimeName = SQLHelper.ExecuteScalar(" SELECT TOP 1 strName FROM dbo.StorageTime WHERE ID=" + TimeID).ToString();
            string UnitID = "公斤";
            object objUnitID = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.BD_MeasuringUnit WHERE ID=( SELECT MeasuringUnitID FROM dbo.StorageVariety WHERE ID=" + VarietyID + ")");//获取计价单位
            if (objUnitID != null && objUnitID.ToString() != "")
            {
                UnitID = objUnitID.ToString();
            }



            int storageType = 1;
            double numStorageIn = 0;
            double numStorageOut = 0;
            double numStorage = 0;
            double numStorageChange = 0;
            double numStorageLoss = 0;
            //查询上一次的存粮操作记录
            DataRow rowVSLog_last = common.GetLastVSLog(WBID, VarietyID,VarietyLevelID);
            if (rowVSLog_last != null)
            {
                numStorageIn = Convert.ToDouble(rowVSLog_last["numStorageIn"]);
                numStorageOut = Convert.ToDouble(rowVSLog_last["numStorageOut"]);
                numStorage = Convert.ToDouble(rowVSLog_last["numStorage"]);
                //double numStorageChange_last = Convert.ToDouble(rowVSLog_last["numStorageChange"]);
                //double numStorageLoss_last = Convert.ToDouble(rowVSLog_last["numStorageLoss"]);
            }
            numStorageChange = Convert.ToDouble(Count_Trade);//存粮数据变化量，存入数取正数
            numStorageIn = numStorageIn + numStorageChange;//改变存入数量
           // numStorageOut = numStorageOut + numStorageChange;
            numStorage = numStorage + numStorageChange;

            //储户存粮次数
            object objStorageCount = SQLHelper.ExecuteScalar(" SELECT COUNT(ID) from dbo.Dep_StorageInfo where AccountNumber='" + AccountNumber + "'");

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    string sqlDep_SIDExit = string.Format(" SELECT ID FROM Dep_StorageInfo  WHERE AccountNumber='{0}' AND StorageRateID={1} AND VarietyID={2}  AND VarietyLevelID={3} AND TypeID={4} AND TimeID={5} and CurrentRate={6} and Price_ShiChang={7} and Price_DaoQi={8} and Price_HeTong={9} and ISVirtual={10} AND DATEDIFF(DAY,GETDATE(),StorageDate)>-1", AccountNumber, StorageRateID, VarietyID, VarietyLevelID, TypeID, TimeID, CurrentRate, Price_ShiChang, Price_DaoQi, Price_HeTong, strISVirtual);
                   
                    object objDep_SIDExit = SQLHelper.ExecuteScalar(sqlDep_SIDExit);
                    object objDep_SID = null;
                    if (objDep_SIDExit != null && objDep_SIDExit.ToString() != "")
                    {
                        objDep_SID = objDep_SIDExit;
                        string sqlDep_SIDUpdate = string.Format("  UPDATE dbo.Dep_StorageInfo SET StorageNumber=StorageNumber+{0},StorageNumberRaw=StorageNumberRaw+{1}  WHERE ID={2}", StorageNumber, StorageNumber, objDep_SIDExit);
                        SQLHelper.ExecuteNonQuery(sqlDep_SIDUpdate);

                        //string sql_BusinessNO=string.Format(" select BusinessNO from Dep_StorageInfo where ID={0}",objDep_SIDExit);
                        //BusinessNO = SQLHelper.ExecuteScalar(sql_BusinessNO).ToString();//使用之前存粮的序列号 
                    }
                    else {
                         objDep_SID = SQLHelper.ExecuteScalar(tran, CommandType.Text, strSql.ToString(), parameters);
                    }
                 

                    #region 添加存粮记录、操作记录
                    StringBuilder strSqlLog = new StringBuilder();
                    strSqlLog.Append("insert into [Dep_StorageLog] (");
                    strSqlLog.Append("AccountNumber,StorageRateID,VarietyID,TypeID,TimeID,StorageDate,WeighNo,StorageNumber,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong,StorageNumberRaw,UserID,WBID,StorageInfoID)");
                    strSqlLog.Append(" values (");
                    strSqlLog.Append("@AccountNumber,@StorageRateID,@VarietyID,@TypeID,@TimeID,@StorageDate,@WeighNo,@StorageNumber,@StorageFee,@CurrentRate,@Price_ShiChang,@Price_DaoQi,@Price_HeTong,@StorageNumberRaw,@UserID,@WBID,@StorageInfoID)");
                    strSqlLog.Append(";select @@IDENTITY");
                    SqlParameter[] parametersLog = {
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageRateID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@StorageDate", SqlDbType.DateTime),
					new SqlParameter("@WeighNo", SqlDbType.NVarChar,50),
					new SqlParameter("@StorageNumber", SqlDbType.Decimal,9),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
                    new SqlParameter("@StorageNumberRaw", SqlDbType.Decimal,9),
                    new SqlParameter("@UserID", SqlDbType.Int,4),
                    new SqlParameter("@WBID", SqlDbType.Int,4),
                    new SqlParameter("@StorageInfoID", SqlDbType.Int,4)};
                    parametersLog[0].Value = AccountNumber;
                    parametersLog[1].Value = StorageRateID;
                    parametersLog[2].Value = VarietyID;
                    parametersLog[3].Value = TypeID;
                    parametersLog[4].Value = TimeID;
                    parametersLog[5].Value = DateTime.Now;
                    parametersLog[6].Value = WeighNo;
                    parametersLog[7].Value = StorageNumber;
                    parametersLog[8].Value = StorageFee;
                    parametersLog[9].Value = CurrentRate;
                    parametersLog[10].Value = Price_ShiChang;
                    parametersLog[11].Value = Price_DaoQi;
                    parametersLog[12].Value = Price_HeTong;
                    parametersLog[13].Value = StorageNumber;
                    parametersLog[14].Value = UserID;
                    parametersLog[15].Value = WBID;
                    parametersLog[16].Value = objDep_SID;

                    object objID = SQLHelper.ExecuteScalar(tran, CommandType.Text, strSqlLog.ToString(), parametersLog);


                    
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
                       	new SqlParameter("@Dep_SID", SqlDbType.Int,4)                                   };
                    parametersOperateLog[0].Value = WBID;
                    parametersOperateLog[1].Value = UserID;
                    parametersOperateLog[2].Value = AccountNumber;
                    parametersOperateLog[3].Value = BusinessNO;
                    parametersOperateLog[4].Value = "1";//1:存入 2：兑换  3:存转销 4: 提取
                    parametersOperateLog[5].Value = VarietyID;
                    parametersOperateLog[6].Value = UnitID;
                    parametersOperateLog[7].Value = Price;
                    parametersOperateLog[8].Value = Count_Trade;
                    parametersOperateLog[9].Value = Count_Trade;
                    parametersOperateLog[10].Value = Money_Trade;
                    parametersOperateLog[11].Value = Count_Balance;
                    parametersOperateLog[12].Value = DateTime.Now;
                    parametersOperateLog[13].Value = TimeName + VarietyName;
                    parametersOperateLog[14].Value = UnitID;
                    parametersOperateLog[15].Value = objDep_SID;

                    //插入数据
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);
                    #endregion 


                    #region 积分添加
                    //储户存粮次数
                    StringBuilder sql_Integral_Dep = new StringBuilder();
                    StringBuilder sql_Integral_Recommend = new StringBuilder();
                    
                    int DepStorageCount = 0;
                    if (objStorageCount != null && objStorageCount.ToString() != "") {
                        DepStorageCount = Convert.ToInt32(objStorageCount);
                    }
                    if (DepStorageCount <= 0)//首次存粮
                    {
                        //添加积分记录
                        DataRow drWBAuthority = common.GetWBAuthority();
                        bool ISIntegral = Convert.ToBoolean(drWBAuthority["ISIntegral"]);


                        if (ISIntegral)
                        {//允许添加积分
                            double Integral_StorageDep = Convert.ToDouble(drWBAuthority["Integral_StorageDep"]);
                            double Integral_StorageRecommend = Convert.ToDouble(drWBAuthority["Integral_StorageRecommend"]);
                            if (Integral_StorageDep > 0)//存粮储户获取积分不为0
                            {
                                double numIntegral_StorageDep = Convert.ToDouble(StorageNumber) * Convert.ToDouble(Price_ShiChang) * 0.001 * Integral_StorageDep;
                                numIntegral_StorageDep = Math.Round(numIntegral_StorageDep, 2);

                                //当前储户最近一次的积分记录
                                object objintegral_TotalLast = SQLHelper.ExecuteScalar("  SELECT TOP 1 integral_Total FROM dbo.Dep_Integral WHERE AccountNumber='" + AccountNumber + "' ORDER BY dt_Add DESC");
                                double integral_Total = numIntegral_StorageDep;
                                if (objintegral_TotalLast != null && objintegral_TotalLast.ToString() != "")
                                {
                                    integral_Total = Convert.ToDouble(objintegral_TotalLast) + numIntegral_StorageDep;
                                }


                                sql_Integral_Dep.Append("  INSERT INTO dbo.Dep_Integral");
                                sql_Integral_Dep.Append("  ( numType,  AccountNumber,AccountNumber_New, numLevel,integral_Change,integral_Total,dt_Add,GEIntegralID )");
                                sql_Integral_Dep.Append(string.Format(" VALUES  ( 3 ,  N'{0}' , N'{1}' ,1 , {2} , {3} ,GETDATE(),  {4} )", AccountNumber,AccountNumber, numIntegral_StorageDep, integral_Total, objDep_SID));
                            }

                            if (Integral_StorageRecommend > 0)//推荐人获取积分不为0
                            {
                                double numIntegral_StorageRecommend = Convert.ToDouble(StorageNumber) * Convert.ToDouble(Price_ShiChang) * 0.001 * Integral_StorageRecommend;
                                numIntegral_StorageRecommend = Math.Round(numIntegral_StorageRecommend, 2);

                                //当前储户的推荐人
                                object objReCommand = SQLHelper.ExecuteScalar("  SELECT  TOP 1  AccountNumber FROM dbo.Dep_Integral WHERE AccountNumber_New='" + AccountNumber + "'");
                                string AccountNumber_ReCommand = "";//推荐人
                                if (objReCommand != null && objReCommand.ToString() != "")
                                {
                                    AccountNumber_ReCommand = objReCommand.ToString();
                                }

                                if (AccountNumber_ReCommand != "")
                                {

                                    //当前储户最近一次的积分记录
                                    object objintegral_TotalLast = SQLHelper.ExecuteScalar("  SELECT TOP 1 integral_Total FROM dbo.Dep_Integral WHERE AccountNumber='" + AccountNumber_ReCommand + "' ORDER BY dt_Add DESC");
                                    double integral_Total = numIntegral_StorageRecommend;
                                    if (objintegral_TotalLast != null && objintegral_TotalLast.ToString() != "")
                                    {
                                        integral_Total = Convert.ToDouble(objintegral_TotalLast) + numIntegral_StorageRecommend;
                                    }


                                    sql_Integral_Recommend.Append("  INSERT INTO dbo.Dep_Integral");
                                    sql_Integral_Recommend.Append("  ( numType,  AccountNumber,AccountNumber_New, numLevel,integral_Change,integral_Total,dt_Add,GEIntegralID )");
                                    sql_Integral_Recommend.Append(string.Format(" VALUES  ( 3 ,  N'{0}' , N'{1}' ,1 , {2} , {3} ,GETDATE(),  {4} )", AccountNumber_ReCommand,AccountNumber, numIntegral_StorageRecommend, integral_Total, objDep_SID));
                                }


                            }
                        }


                    }

                    //键入储户、推荐人积分记录
                    if (sql_Integral_Dep.ToString() != "")
                    {
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql_Integral_Dep.ToString());
                    }

                    if (sql_Integral_Recommend.ToString() != "")
                    {
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql_Integral_Recommend.ToString());
                    }
                    #endregion

                    

                    //if (objDep_SIDExit != null && objDep_SIDExit.ToString() != "")//当天存在相同的存粮
                    //{
                    //    StringBuilder sql_OperateLog_update = new StringBuilder();
                    //    sql_OperateLog_update.Append(" UPDATE  Dep_OperateLog");
                    //    sql_OperateLog_update.Append(string.Format("  SET GoodCount+={0}, Count_Trade+={1},Money_Trade+={2},Count_Balance+={3}", Count_Trade, Count_Trade, Money_Trade, Count_Trade));
                    //    sql_OperateLog_update.Append(string.Format(" WHERE Dep_AccountNumber='{0}' AND BusinessNO='{1}'", AccountNumber, BusinessNO));
                    //    //更新数据
                    //    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql_OperateLog_update.ToString());

                    //}
                    //else {
                    //    //插入数据
                    //    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);

                    //}

                    if (!ISVirtual)//实际存粮
                    {
                        #region  网点库存和库存日志
                        //添加网点的产品库存记录
                        string VarietyStorageExit = " SELECT COUNT(ID)   FROM dbo.SA_VarietyStorage WHERE WBID=" + WBID + " AND VarietyID=" + VarietyID + " and VarietyLevelID=" + VarietyLevelID;
                        if (Convert.ToInt32(SQLHelper.ExecuteScalar(VarietyStorageExit)) <= 0)
                        {
                            StringBuilder VarietyStorageInsert = new StringBuilder();
                            VarietyStorageInsert.Append("insert into [SA_VarietyStorage] (");
                            VarietyStorageInsert.Append("VarietyID,VarietyLevelID,WBID,numStorage,WareHouseID,ISHQ,ISSimulate)");
                            VarietyStorageInsert.Append(" values (");
                            VarietyStorageInsert.Append("@VarietyID,@VarietyLevelID,@WBID,@numStorage,@WareHouseID,@ISHQ,@ISSimulate)");
                            VarietyStorageInsert.Append(";select @@IDENTITY");
                            SqlParameter[] parametersVarietyStorageInsert = {
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
                    new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@numStorage", SqlDbType.Decimal,9),
					new SqlParameter("@WareHouseID", SqlDbType.Int,4),
					new SqlParameter("@ISHQ", SqlDbType.Bit,1),
					new SqlParameter("@ISSimulate", SqlDbType.Bit,1)};
                            parametersVarietyStorageInsert[0].Value = VarietyID;
                            parametersVarietyStorageInsert[1].Value = VarietyLevelID;
                            parametersVarietyStorageInsert[2].Value = WBID;
                            parametersVarietyStorageInsert[3].Value = StorageNumber;
                            parametersVarietyStorageInsert[4].Value = 1;//暂时不适用
                            parametersVarietyStorageInsert[5].Value = 0;
                            parametersVarietyStorageInsert[6].Value = 0;

                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, VarietyStorageInsert.ToString(), parametersVarietyStorageInsert);//首次添加网点产品的库存记录
                        }
                        else
                        {
                            string VarietyStorageUpdate = " UPDATE dbo.SA_VarietyStorage SET numStorage=" + numStorage + " WHERE WBID=" + WBID + " AND VarietyID=" + VarietyID + " and VarietyLevelID=" + VarietyLevelID; ;
                            SQLHelper.ExecuteNonQuery(tran, CommandType.Text, VarietyStorageUpdate.ToString());

                        }

                        //添加存粮操作日志
                        StringBuilder sqlVS = new StringBuilder();
                        sqlVS.Append(" ");
                        sqlVS.Append(" INSERT INTO dbo.SA_VarietyStorageLog");
                        sqlVS.Append("   ( storageType , OperateLogID ,  CheckOutID ,   AccountNumber ,  VarietyID ,VarietyLevelID ,WBID , numStorage, numStorageChange, numStorageIn ,numStorageOut ,numStorageLoss , WareHouseID ,ISHQ , ISSimulate,dtLog)");
                        sqlVS.Append(" VALUES  ( " + storageType + " ," + objID.ToString() + " ,   0 ,  N'" + AccountNumber + "' ,  " + VarietyID + " ,  " + VarietyLevelID + " ,  " + WBID + " , ");
                        sqlVS.Append("    " + numStorage + " ," + numStorageChange + " , " + numStorageIn + " , " + numStorageOut + " , " + numStorageLoss + " ,");
                        sqlVS.Append("    0 , 0 ,   0  ,'" + DateTime.Now.ToString() + "' )");
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlVS.ToString());
                        #endregion
                    }
                    tran.Commit();

                    var res = new { state = "true", BusinessNO = BusinessNO,msg="存粮成功！" };

                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "false", BusinessNO = BusinessNO, msg = "存粮失败！" };

                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
        }
       
        void Update_Dep_Storage(HttpContext context)
        {
            //string BusinessNO = context.Request.Form["BusinessNO"].ToString();//交易号
          
            string ID = context.Request.Form["ID"].ToString();
              StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("select ID,BusinessNO,AccountNumber,StorageRateID,VarietyID,VarietyLevelID,TypeID,TimeID,InterestDate,StorageDate,WeighNo,StorageNumber,StorageNumberRaw,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong,UserID,WBID ");
            strSqlStorage.Append(" FROM [Dep_StorageInfo] ");
            strSqlStorage.Append(" where ID=@ID ");
            SqlParameter[] parametersStorage = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parametersStorage[0].Value = ID;
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString(), parametersStorage);

             string AccountNumber = dtStorage.Rows[0]["AccountNumber"].ToString();
             string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);

            string editType = context.Request.Form["editType"].ToString();
            string WeighNo = context.Request.Form["WeighNO"].ToString();
            string StorageNumber = context.Request.Form["StorageNumber"].ToString();
            double numStorage = -Convert.ToDouble(StorageNumber);
            string strSqlEdit = "UPDATE dbo.Dep_StorageInfo SET WeighNo='" + WeighNo + "',StorageNumber=StorageNumber-" + StorageNumber + ",StorageNumberRaw= StorageNumberRaw-" + StorageNumber + " WHERE ID=" + ID;//修改存粮信息表
            //string strSqlEditLog = "UPDATE dbo.Dep_StorageLog SET WeighNo='" + WeighNo + "',StorageNumber=StorageNumber-" + StorageNumber + ",StorageNumberRaw= StorageNumberRaw-" + StorageNumber + " WHERE StorageInfoID=" + ID;//修改存粮记录表
            if (editType == "增加")
            {
                numStorage = Convert.ToDouble(StorageNumber);
                strSqlEdit = "UPDATE dbo.Dep_StorageInfo SET WeighNo='" + WeighNo + "',StorageNumber=StorageNumber+" + StorageNumber + ",StorageNumberRaw= StorageNumberRaw+" + StorageNumber + " WHERE ID=" + ID;
            }

          
            double StorageNumber_Start = Convert.ToDouble(dtStorage.Rows[0]["StorageNumber"]);
            double num_balance = StorageNumber_Start + numStorage;
            string VarietyID = dtStorage.Rows[0]["VarietyID"].ToString();
            string VarietyLevelID = dtStorage.Rows[0]["VarietyLevelID"].ToString();
           

            //添加交易记录

            string WBID = dtStorage.Rows[0]["WBID"].ToString();
            string UserID = context.Session["ID"].ToString();
            string Price = dtStorage.Rows[0]["Price_ShiChang"].ToString();//价格
            string Count_Trade = numStorage.ToString();//存储数量
            string Money_Trade = "0";//存入的时候没有交易量
            double Count_Balance = common.GetDep_StorageNumber(AccountNumber, VarietyID);//储户总结存
            Count_Balance = Count_Balance + numStorage;

            string VarietyName = SQLHelper.ExecuteScalar("  SELECT strName FROM dbo.StorageVariety WHERE ID="+VarietyID).ToString();
            string UnitID = "公斤";
            object objUnitID = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.BD_MeasuringUnit WHERE ID=( SELECT MeasuringUnitID FROM dbo.StorageVariety WHERE ID="+VarietyID+")");//获取计价单位
            if (objUnitID != null && objUnitID.ToString() != "")
            {
                UnitID = objUnitID.ToString();
            }

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
                    new SqlParameter("@Dep_SID", SqlDbType.Int,4)                        };
            parametersOperateLog[0].Value = WBID;
            parametersOperateLog[1].Value = UserID;
            parametersOperateLog[2].Value = AccountNumber;
            parametersOperateLog[3].Value = BusinessNO;
            parametersOperateLog[4].Value = "5";//1:存入 2：兑换  3:存转销 4: 提取:5修改错误存量
            parametersOperateLog[5].Value = VarietyID;
            parametersOperateLog[6].Value = UnitID;
            parametersOperateLog[7].Value = Price;
            parametersOperateLog[8].Value = Count_Trade;
            parametersOperateLog[9].Value = Count_Trade;
            parametersOperateLog[10].Value = Money_Trade;
            parametersOperateLog[11].Value = Count_Balance;
            parametersOperateLog[12].Value = DateTime.Now;
            parametersOperateLog[13].Value = VarietyName;
            parametersOperateLog[14].Value = UnitID;
            parametersOperateLog[15].Value = ID;


            int storageType = 2;
            double numStorageIn = 0;
            double numStorageOut = 0;
            double numStorage_Log = 0;
            double numStorageChange = 0;
            double numStorageLoss = 0;
            //查询上一次的存粮操作记录
            DataRow rowVSLog_last = common.GetLastVSLog(WBID, VarietyID,VarietyLevelID);
            if (rowVSLog_last != null)
            {
                numStorageIn = Convert.ToDouble(rowVSLog_last["numStorageIn"]);
                numStorageOut = Convert.ToDouble(rowVSLog_last["numStorageOut"]);
                numStorage_Log = Convert.ToDouble(rowVSLog_last["numStorage"]);
                //double numStorageChange_last = Convert.ToDouble(rowVSLog_last["numStorageChange"]);
                //double numStorageLoss_last = Convert.ToDouble(rowVSLog_last["numStorageLoss"]);
            }
            numStorageChange = numStorage;//存粮数据变化量
            numStorageIn = numStorageIn + numStorageChange;
            //numStorageOut = numStorageOut + numStorageChange;
            numStorage_Log = numStorage_Log + numStorageChange;

            string VarietyStorageUpdate = " UPDATE dbo.SA_VarietyStorage SET numStorage=" + numStorage_Log + " WHERE WBID=" + WBID + " AND VarietyID=" + VarietyID+" and VarietyLevelID="+VarietyLevelID;

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlEdit.ToString());//更新储户存储信息

                    //SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlEditLog.ToString());//更新储户存储信息记录
                    
                   object objID= SQLHelper.ExecuteScalar(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);

                    //修改网点的产品库存记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, VarietyStorageUpdate.ToString());

                    //添加存粮操作日志
                    StringBuilder sqlVS = new StringBuilder();
                    sqlVS.Append(" ");
                    sqlVS.Append(" INSERT INTO dbo.SA_VarietyStorageLog");
                    sqlVS.Append("   ( storageType , OperateLogID ,  CheckOutID ,   AccountNumber ,  VarietyID ,VarietyLevelID ,WBID , numStorage, numStorageChange, numStorageIn ,numStorageOut ,numStorageLoss , WareHouseID ,ISHQ , ISSimulate,dtLog)");
                    sqlVS.Append(" VALUES  ( " + storageType + " ," + objID.ToString() + " ,   0,  N'" + AccountNumber + "' ,  " + VarietyID + " ,  " + VarietyLevelID + " ,  " + WBID + " , ");
                    sqlVS.Append("    " + numStorage_Log + " ," + numStorageChange + " , " + numStorageIn + " , " + numStorageOut + " , " + numStorageLoss + " ,");
                    sqlVS.Append("    0 , 0 ,   0 ,'" + DateTime.Now.ToString() + "'  )");
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlVS.ToString());


                    tran.Commit();
                    var res = new { state = "true", msg = "更新数据成功!", BusinessNO = BusinessNO };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "false", msg = "更新数据失败!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
           
        }

        /// <summary>
        /// 添加储户存储信息
        /// </summary>
        /// <param name="context"></param>
        void StorageTypeChange(HttpContext context)
        {
            // string BusinessNO = context.Request.Form["BusinessNO"].ToString();//交易号
            string StorageRateID = context.Request.Form["StorageRateID"].ToString();
            string ID = context.Request.Form["ID"].ToString();

            DataRow rowDep_StorageInfo = commondb.getDep_StorageInfoByID(ID);
            string StorageNumber = rowDep_StorageInfo["StorageNumber"].ToString();
            string AccountNumber = rowDep_StorageInfo["AccountNumber"].ToString().Trim();
            string BusinessNO = common.GetNewBusinessNO_Dep(AccountNumber);//交易流水号

            DataRow rowStorageRate = commondb.getStorageRateByID(StorageRateID);
            string CurrentRate = rowStorageRate["CurrentRate"].ToString();
            string Price_ShiChang = rowStorageRate["Price_ShiChang"].ToString();
            string Price_DaoQi = rowStorageRate["Price_DaoQi"].ToString();
            string Price_HeTong = rowStorageRate["Price_HeTong"].ToString();
            string VarietyID = rowStorageRate["VarietyID"].ToString();
            string VarietyLevelID = rowStorageRate["VarietyLevelID"].ToString();
            string TypeID = rowStorageRate["TypeID"].ToString();
            string TimeID = rowStorageRate["TimeID"].ToString();
            string StorageFee = rowStorageRate["StorageFee"].ToString();
        

            //string UserID = context.Session["ID"].ToString();
            //string WBID = context.Session["WB_ID"].ToString();
            string UserID = rowDep_StorageInfo["UserID"].ToString();
            string WBID = rowDep_StorageInfo["WBID"].ToString();

            //转存数据更新
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" update [Dep_StorageInfo] ");
            strSql.Append("  SET StorageRateID=@StorageRateID, ");
            strSql.Append("   TypeID=@TypeID, ");
            strSql.Append("   TimeID=@TimeID, ");
            strSql.Append("   StorageFee=@StorageFee, ");
            strSql.Append("   CurrentRate=@CurrentRate, ");
            strSql.Append("   Price_ShiChang=@Price_ShiChang, ");
            strSql.Append("   Price_DaoQi=@Price_DaoQi, ");
            strSql.Append("   Price_HeTong=@Price_HeTong");
            strSql.Append("  WHERE ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@StorageRateID", SqlDbType.Int,4),
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
					new SqlParameter("@ID", SqlDbType.Int,4),};
            parameters[0].Value = StorageRateID;
            parameters[1].Value = TypeID;
            parameters[2].Value = TimeID;
            parameters[3].Value = StorageFee;
            parameters[4].Value = CurrentRate;
            parameters[5].Value = Price_ShiChang;
            parameters[6].Value = Price_DaoQi;
            parameters[7].Value = Price_HeTong;
            parameters[8].Value = ID;


            //转存操作记录
            string VarietyName = SQLHelper.ExecuteScalar("  SELECT strName FROM dbo.StorageVariety WHERE ID=" + VarietyID).ToString();

            string TimeName = SQLHelper.ExecuteScalar(" SELECT TOP 1 strName FROM dbo.StorageTime WHERE ID=" + TimeID).ToString();
            string UnitID = "公斤";
            object objUnitID = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.BD_MeasuringUnit WHERE ID=( SELECT MeasuringUnitID FROM dbo.StorageVariety WHERE ID=" + VarietyID + ")");//获取计价单位
            if (objUnitID != null && objUnitID.ToString() != "")
            {
                UnitID = objUnitID.ToString();
            }

            double Count_Balance = common.GetDep_StorageNumber(AccountNumber, VarietyID);//储户总结存
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
                       	new SqlParameter("@Dep_SID", SqlDbType.Int,4)                                   };
            parametersOperateLog[0].Value = WBID;
            parametersOperateLog[1].Value = UserID;
            parametersOperateLog[2].Value = AccountNumber;
            parametersOperateLog[3].Value = BusinessNO;
            parametersOperateLog[4].Value = "16";
            parametersOperateLog[5].Value = VarietyID;
            parametersOperateLog[6].Value = UnitID;
            parametersOperateLog[7].Value = Price_ShiChang;
            parametersOperateLog[8].Value = 0;
            parametersOperateLog[9].Value = 0;
            parametersOperateLog[10].Value = 0;
            parametersOperateLog[11].Value = Count_Balance;
            parametersOperateLog[12].Value = DateTime.Now;
            parametersOperateLog[13].Value = TimeName + VarietyName;
            parametersOperateLog[14].Value = UnitID;
            parametersOperateLog[15].Value = ID;

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    //更新存粮类型
                    SQLHelper.ExecuteScalar(tran, CommandType.Text, strSql.ToString(), parameters);

                    //存粮转存记录
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);

                    tran.Commit();

                    var res = new { state = "true", BusinessNO = BusinessNO, msg = "转存成功！" };

                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = "false", BusinessNO = BusinessNO, msg = "转存失败！" };

                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }
        }

        /// <summary>
        /// 删除存粮记录
        /// </summary>
        /// <param name="context"></param>
        void Delete_Dep_Storage(HttpContext context)
        {
            string BusinessNO = context.Request.QueryString["BusinessNO"].ToString();//交易号
            string ID = context.Request.QueryString["ID"].ToString();
            

            //查询被退还的存储信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("select ID,BusinessNO,AccountNumber,StorageRateID,VarietyID,VarietyLevelID,TypeID,TimeID,InterestDate,StorageDate,WeighNo,StorageNumber,StorageNumberRaw,StorageFee,CurrentRate,Price_ShiChang,Price_DaoQi,Price_HeTong,UserID,WBID ");
            strSqlStorage.Append(" FROM [Dep_StorageInfo] ");
            strSqlStorage.Append(" where ID= "+ID);
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());

            if (dtStorage == null || dtStorage.Rows.Count == 0)//此条存粮信息已经被删除
            {
                context.Response.Write("0");
                return;
            }
            //添加退还日志记录
          
            double StorageNumber= Convert.ToDouble(dtStorage.Rows[0]["StorageNumber"]);
            string VarietyID = dtStorage.Rows[0]["VarietyID"].ToString();
            string VarietyLevelID = dtStorage.Rows[0]["VarietyLevelID"].ToString();
            string AccountNumber = dtStorage.Rows[0]["AccountNumber"].ToString();

            //添加交易记录
            //string WBID = context.Session["WB_ID"].ToString();
            //string UserID = context.Session["ID"].ToString();
            string WBID = dtStorage.Rows[0]["WBID"].ToString();
            string UserID = dtStorage.Rows[0]["UserID"].ToString();
            string Price = dtStorage.Rows[0]["Price_ShiChang"].ToString();//价格

            string VarietyName = SQLHelper.ExecuteScalar("  SELECT strName FROM dbo.StorageVariety WHERE ID=" + VarietyID).ToString();
            string UnitID = "公斤";
            object objUnitID = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.BD_MeasuringUnit WHERE ID=( SELECT MeasuringUnitID FROM dbo.StorageVariety WHERE ID=" + VarietyID + ")");//获取计价单位
            if (objUnitID != null && objUnitID.ToString() != "")
            {
                UnitID = objUnitID.ToString();
            }

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
            parametersOperateLog[4].Value = "8";//1:存入 2：兑换  3:存转销 4: 提取:5修改错误存量
            parametersOperateLog[5].Value = VarietyID;
            parametersOperateLog[6].Value = UnitID;
            parametersOperateLog[7].Value = Price;
            parametersOperateLog[8].Value = StorageNumber;
            parametersOperateLog[9].Value = -StorageNumber;
            parametersOperateLog[10].Value = StorageNumber;
            parametersOperateLog[11].Value = 0;
            parametersOperateLog[12].Value = DateTime.Now;
            parametersOperateLog[13].Value = VarietyName;
            parametersOperateLog[14].Value = UnitID;
            parametersOperateLog[15].Value = ID;

            //string strSqlDelete = " DELETE FROM dbo.Dep_StorageInfo WHERE ID="+ID;
            string strSqlUpdate_Dep = string.Format("   UPDATE dbo.Dep_StorageInfo SET StorageNumber=0 ,StorageNumberRaw=StorageNumberRaw-{0} WHERE ID={1}",StorageNumber, ID);


            int storageType = 3;
            double numStorageIn = 0;
            double numStorageOut = 0;
            double numStorage = 0;
            double numStorageChange = 0;
            double numStorageLoss = 0;
            //查询上一次的存粮操作记录
            DataRow rowVSLog_last = common.GetLastVSLog(WBID, VarietyID,VarietyLevelID);
            if (rowVSLog_last != null)
            {
                numStorageIn = Convert.ToDouble(rowVSLog_last["numStorageIn"]);
                numStorageOut = Convert.ToDouble(rowVSLog_last["numStorageOut"]);
                numStorage = Convert.ToDouble(rowVSLog_last["numStorage"]);
                //double numStorageChange_last = Convert.ToDouble(rowVSLog_last["numStorageChange"]);
                //double numStorageLoss_last = Convert.ToDouble(rowVSLog_last["numStorageLoss"]);
            }
            numStorageChange = -Convert.ToDouble(StorageNumber);//退还存粮，取负数
            numStorageIn = numStorageIn + numStorageChange;
           // numStorageOut = numStorageOut + numStorageChange;
            numStorage = numStorage + numStorageChange;

            //修改该产品在网点中的库存
            string strSqlUpdate = " UPDATE dbo.SA_VarietyStorage SET numStorage=" + numStorage + " WHERE WBID=" + WBID + " AND VarietyID=" + VarietyID+" and VarietyLevelID="+VarietyLevelID;

         
            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    object objID=  SQLHelper.ExecuteScalar(tran, CommandType.Text, strSqlOperateLog.ToString(), parametersOperateLog);//添加退还日志记录

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlUpdate_Dep.ToString());//将储户存粮置0
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlUpdate.ToString());//修改网点库存


                    //添加存粮操作日志
                    StringBuilder sqlVS = new StringBuilder();
                    sqlVS.Append(" ");
                    sqlVS.Append(" INSERT INTO dbo.SA_VarietyStorageLog");
                    sqlVS.Append("   ( storageType , OperateLogID ,  CheckOutID ,   AccountNumber ,  VarietyID ,VarietyLevelID ,WBID , numStorage, numStorageChange, numStorageIn ,numStorageOut ,numStorageLoss , WareHouseID ,ISHQ , ISSimulate,dtLog)");
                    sqlVS.Append(" VALUES  ( " + storageType + " ," + objID.ToString() + " ,   0 ,  N'" + AccountNumber + "' ,  " + VarietyID + " ,  " + VarietyLevelID + " ,  " + WBID + " , ");
                    sqlVS.Append("    " + numStorage + " ," + numStorageChange + " , " + numStorageIn + " , " + numStorageOut + " , " + numStorageLoss + " ,");
                    sqlVS.Append("    0 , 0 ,   0 ,'" + DateTime.Now.ToString() + "'  )");
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlVS.ToString());
                   
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

            if (dtStorage!=null&&dtStorage.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dtStorage));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        /// <summary>
        /// 预存转实存
        /// </summary>
        /// <param name="context"></param>
        void StorageVirtual(HttpContext context)
        {
            string ID = context.Request.Form["ID"].ToString();
            string WeighNO = context.Request.Form["WeighNO"].ToString();
            string sqlupdate = string.Format("  UPDATE dbo.Dep_StorageInfo SET WeighNo='{0}' ,  ISVirtual=0 WHERE ID={1} ",WeighNO,ID);

            DataTable dt = SQLHelper.ExecuteDataTable(" select * from Dep_StorageInfo where ID=" + ID);
            DataRow row = dt.Rows[0];
            string WBID = row["WBID"].ToString();
            string VarietyID = row["VarietyID"].ToString();
            string VarietyLevelID = row["VarietyLevelID"].ToString();
            string StorageNumber = row["StorageNumber"].ToString();
            string AccountNumber = row["AccountNumber"].ToString();


            int storageType = 1;
            double numStorageIn = 0;
            double numStorageOut = 0;
            double numStorage = 0;
            double numStorageChange = 0;
            double numStorageLoss = 0;
            //查询上一次的存粮操作记录
            DataRow rowVSLog_last = common.GetLastVSLog(WBID, VarietyID, VarietyLevelID);
            if (rowVSLog_last != null)
            {
                numStorageIn = Convert.ToDouble(rowVSLog_last["numStorageIn"]);
                numStorageOut = Convert.ToDouble(rowVSLog_last["numStorageOut"]);
                numStorage = Convert.ToDouble(rowVSLog_last["numStorage"]);
                //double numStorageChange_last = Convert.ToDouble(rowVSLog_last["numStorageChange"]);
                //double numStorageLoss_last = Convert.ToDouble(rowVSLog_last["numStorageLoss"]);
            }
            numStorageChange = Convert.ToDouble(StorageNumber);//存粮数据变化量，存入数取正数
            numStorageIn = numStorageIn + numStorageChange;//改变存入数量
            // numStorageOut = numStorageOut + numStorageChange;
            numStorage = numStorage + numStorageChange;

            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlupdate);//更新存粮状态

                    #region  网点库存和库存日志
                    //添加网点的产品库存记录
                    string VarietyStorageExit = " SELECT COUNT(ID)   FROM dbo.SA_VarietyStorage WHERE WBID=" + WBID + " AND VarietyID=" + VarietyID + " and VarietyLevelID=" + VarietyLevelID;
                    if (Convert.ToInt32(SQLHelper.ExecuteScalar(VarietyStorageExit)) <= 0)
                    {
                        StringBuilder VarietyStorageInsert = new StringBuilder();
                        VarietyStorageInsert.Append("insert into [SA_VarietyStorage] (");
                        VarietyStorageInsert.Append("VarietyID,VarietyLevelID,WBID,numStorage,WareHouseID,ISHQ,ISSimulate)");
                        VarietyStorageInsert.Append(" values (");
                        VarietyStorageInsert.Append("@VarietyID,@VarietyLevelID,@WBID,@numStorage,@WareHouseID,@ISHQ,@ISSimulate)");
                        VarietyStorageInsert.Append(";select @@IDENTITY");
                        SqlParameter[] parametersVarietyStorageInsert = {
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
                    new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@numStorage", SqlDbType.Decimal,9),
					new SqlParameter("@WareHouseID", SqlDbType.Int,4),
					new SqlParameter("@ISHQ", SqlDbType.Bit,1),
					new SqlParameter("@ISSimulate", SqlDbType.Bit,1)};
                        parametersVarietyStorageInsert[0].Value = VarietyID;
                        parametersVarietyStorageInsert[1].Value = VarietyLevelID;
                        parametersVarietyStorageInsert[2].Value = WBID;
                        parametersVarietyStorageInsert[3].Value = StorageNumber;
                        parametersVarietyStorageInsert[4].Value = 1;//暂时不适用
                        parametersVarietyStorageInsert[5].Value = 0;
                        parametersVarietyStorageInsert[6].Value = 0;

                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, VarietyStorageInsert.ToString(), parametersVarietyStorageInsert);//首次添加网点产品的库存记录
                    }
                    else
                    {
                        string VarietyStorageUpdate = " UPDATE dbo.SA_VarietyStorage SET numStorage=" + StorageNumber + " WHERE WBID=" + WBID + " AND VarietyID=" + VarietyID + " and VarietyLevelID=" + VarietyLevelID; ;
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, VarietyStorageUpdate.ToString());

                    }

                    //添加存粮操作日志
                    StringBuilder sqlVS = new StringBuilder();
                    sqlVS.Append(" ");
                    sqlVS.Append(" INSERT INTO dbo.SA_VarietyStorageLog");
                    sqlVS.Append("   ( storageType , OperateLogID ,  CheckOutID ,   AccountNumber ,  VarietyID ,VarietyLevelID ,WBID , numStorage, numStorageChange, numStorageIn ,numStorageOut ,numStorageLoss , WareHouseID ,ISHQ , ISSimulate,dtLog)");
                    sqlVS.Append(" VALUES  ( " + storageType + " ," + ID + " ,   0 ,  N'" + AccountNumber + "' ,  " + VarietyID + " ,  " + VarietyLevelID + " ,  " + WBID + " , ");
                    sqlVS.Append("    " + numStorage + " ," + numStorageChange + " , " + numStorageIn + " , " + numStorageOut + " , " + numStorageLoss + " ,");
                    sqlVS.Append("    0 , 0 ,   0  ,'" + DateTime.Now.ToString() + "' )");
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlVS.ToString());
                    #endregion
                    tran.Commit();
                    var res = new { state = "true",  msg = "保存成功！" };

                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch {
                    tran.Rollback();
                    var res = new { state = "false", msg = "保存失败！" };

                    context.Response.Write(JsonHelper.ToJson(res));
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