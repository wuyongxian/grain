using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;

namespace Web.User.Query
{
    /// <summary>
    /// depositor 的摘要说明
    /// </summary>
    public class depositor : IHttpHandler, IRequiresSessionState
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
                    case "GetWebSiteByUserID": GetWebSiteByUserID(context); break;
                    case "Get_Address_Xian": Get_Address_Xian(context); break;
                    case "Get_Address_Xiang": Get_Address_Xiang(context); break;
                    case "Get_Address_Cun": Get_Address_Cun(context); break;

                    case "GetNewAccountNumber": GetNewAccountNumber(context); break;

                    case "GetByAccountNumver_Depositor": GetByAccountNumver_Depositor(context); break;
                    case "GetByName_Depositor": GetByName_Depositor(context); break;
                    case "Add_Depositor": Add_Depositor(context); break;
                    case "GetDepositor": GetDepositor(context); break;
                    case "Update_Depositor": Update_Depositor(context); break;
                    case "GetDep_Integral": GetDep_Integral(context); break;
                    case "GetDep_IntegralDetail": GetDep_IntegralDetail(context); break;
                    case "GetDep_IntegralSingle": GetDep_IntegralSingle(context); break;
                    case "Update_Address_Cun": Update_Address_Cun(context); break;
                    case "DeleteByID_Address_Cun": DeleteByID_Address_Cun(context); break;
                    case "GetRegularStorage": GetRegularStorage(context); break;
                    case "GetCurrentStorage": GetCurrentStorage(context); break;
                    case "GetExpireStorageCount": GetExpireStorageCount(context); break;
                        
                }
            }

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
            string XianID = context.Request.QueryString["XianID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName,WBID FROM dbo.BD_Address_Xiang ");
            strSql.Append(" where XianID=" + XianID);
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
            string XiangID = context.Request.QueryString["XiangID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName,WBID FROM dbo.BD_Address_Cun ");
            strSql.Append(" where XiangID=" + XiangID);
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

        void GetNewAccountNumber(HttpContext context)
        {
            string WB_ID = context.Session["WB_ID"].ToString();

            string AccountNumver = common.GetNewAccountNumber(WB_ID);

            context.Response.Write(AccountNumver);
        }


        void GetByAccountNumver_Depositor(HttpContext context)
        {
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select ID,WBID,AccountNumber,strPassword,CunID as  BD_Address_CunID,strAddress,strName,IDCard,PhoneNO,ISSendMessage,BankCardNO,numState,dt_Add,dt_Update");
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

        void GetByName_Depositor(HttpContext context)
        {
            string strName = context.Request.Form["strName"].ToString().Trim();
            string PhoneNO = context.Request.Form["PhoneNO"].ToString().Trim();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select ID,WBID,AccountNumber,strPassword,CunID as  BD_Address_CunID,strAddress,strName,IDCard,PhoneNO,ISSendMessage,BankCardNO,numState,dt_Add,dt_Update");
            strSql.Append(" FROM dbo.Depositor where 1=1");
            if (strName != "") {
                strSql.Append(string.Format(" and strName='{0}'",strName));
            }
            if (PhoneNO != "")
            {
                strSql.Append(string.Format(" and PhoneNO='{0}'", PhoneNO));
            }
          
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

        void Add_Depositor(HttpContext context)
        {
            string DepRecommend = "";
            if (context.Request.QueryString["DepRecommend"] != null) {
                DepRecommend = context.Request.QueryString["DepRecommend"].ToString();
            }
          
            string WBID = context.Session["WB_ID"].ToString();
           // string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string AccountNumber = common.GetNewAccountNumber(WBID);
            string strName = context.Request.Form["strName"].ToString();
            string strPassword = context.Request.Form["strPassword"].ToString();
            strPassword = Fun.GetMD5_32(strPassword);

            string XianID = context.Request.Form["XianID"].ToString();
            string XiangID = context.Request.Form["XiangID"].ToString();
            string CunID = context.Request.Form["CunID"].ToString();
            string IDCard = context.Request.Form["IDCard"].ToString();
            string PhoneNO = context.Request.Form["PhoneNO"].ToString();
            bool ISSendMessage = false;
            if (context.Request.Form["ISSendMessage"] == null) {
                ISSendMessage = true;
            }
            string strAddress = context.Request.Form["strAddress"].ToString();

            //检验账号唯一性
            string strSameAccount = "  SELECT  COUNT(ID)  FROM dbo.Depositor WHERE AccountNumber='" + AccountNumber + "'";
            if (SQLHelper.ExecuteScalar(strSameAccount).ToString() != "0")
            {
                var res = new { state = false, msg = "系统中已经有相同的储户账号!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }

            //检验身份证号唯一性(除去已经销户的储户)
            string strSameIDCard = "  SELECT  COUNT(ID)  FROM dbo.Depositor WHERE IDCard='" + IDCard + "' and ISClosing=0";
            if (SQLHelper.ExecuteScalar(strSameIDCard).ToString() != "0")
            {
                var res = new { state = false, msg = "当前的身份证号已存在于系统中，不可以重复使用!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }


            //string strCount = "   SELECT COUNT(ID) FROM dbo.Depositor WHERE IDCard='" + IDCard + "' AND WBID=" + WBID;
            //if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            //{
            //    context.Response.Write("1");
            //    return;
            //}
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Depositor] (");
            strSql.Append("WBID,AccountNumber,strPassword,strAddress,XianID,XiangID,CunID,strName,IDCard,PhoneNO,ISSendMessage,BankCardNO,numState,dt_Add,dt_Update)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@AccountNumber,@strPassword,@strAddress,@XianID,@XiangID,@CunID,@strName,@IDCard,@PhoneNO,@ISSendMessage,@BankCardNO,@numState,@dt_Add,@dt_Update)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@strPassword", SqlDbType.NVarChar,50),
					new SqlParameter("@strAddress", SqlDbType.NVarChar,100),
					new SqlParameter("@XianID", SqlDbType.Int,4),
					new SqlParameter("@XiangID", SqlDbType.Int,4),
					new SqlParameter("@CunID", SqlDbType.Int,4),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@IDCard", SqlDbType.NVarChar,50),
					new SqlParameter("@PhoneNO", SqlDbType.NVarChar,50),
					new SqlParameter("@ISSendMessage", SqlDbType.Bit,1),
					new SqlParameter("@BankCardNO", SqlDbType.NVarChar,50),
					new SqlParameter("@numState", SqlDbType.Int,4),
					new SqlParameter("@dt_Add", SqlDbType.DateTime),
					new SqlParameter("@dt_Update", SqlDbType.DateTime)};
            parameters[0].Value = WBID;
            parameters[1].Value = AccountNumber;
            parameters[2].Value = strPassword;
            parameters[3].Value = strAddress;
            parameters[4].Value = XianID;
            parameters[5].Value = XiangID;
            parameters[6].Value = CunID;
            parameters[7].Value = strName;
            parameters[8].Value = IDCard;
            parameters[9].Value = PhoneNO;
            parameters[10].Value = ISSendMessage;
            parameters[11].Value = 0;
            parameters[12].Value = 1;
            parameters[13].Value = DateTime.Now;
            parameters[14].Value = DateTime.Now;

            StringBuilder sqlIntegral = new StringBuilder();
            if (DepRecommend != "") {
                object objintegral_Total = SQLHelper.ExecuteScalar(" SELECT TOP 1 integral_Total FROM dbo.Dep_Integral WHERE AccountNumber='"+DepRecommend+"' ORDER BY ID DESC");
                double integral_Change = Convert.ToDouble(common.GetWBAuthority()["Integral"]);
                double integral_Total = integral_Change;
                if (objintegral_Total != null && objintegral_Total.ToString() != "") {
                    integral_Total = Convert.ToDouble(objintegral_Total) + integral_Change;
                }
            //添加积分记录
                sqlIntegral.Append(" INSERT INTO dbo.Dep_Integral");
                sqlIntegral.Append("  ( numType ,GEIntegralID, AccountNumber ,AccountNumber_New , numLevel ,integral_Change ,integral_Total , dt_Add)");
                sqlIntegral.Append(string.Format(" VALUES  ( 1,null, N'{0}' , N'{1}' , {2} , {3} ,   {4} ,  GETDATE() )",DepRecommend,AccountNumber,1,integral_Change,integral_Total));
            }

            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString(), parameters);
                    if (sqlIntegral.ToString() != "")
                    {
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sqlIntegral.ToString());
                    }
                    tran.Commit();

                    var res = new { state = true, AccountNumber = AccountNumber, msg = "新建账号成功!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
                catch
                {
                    tran.Rollback();
                    var res = new { state = false, msg = "新建账号失败!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                }
            }

        }

        //根据账号获取单个储户的信息
        void GetDepositor(HttpContext context)
        {

            string WBID = context.Session["WB_ID"].ToString();
            object objISHQ = SQLHelper.ExecuteScalar(" SELECT ISHQ FROM dbo.WB WHERE ID="+WBID);
           
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  select A.ID,B.strName AS WBID,A. AccountNumber,strPassword,E.ID AS XianID,D.ID AS XiangID,C.ID AS CunID,A.strAddress,A.strName,IDCard,PhoneNO,ISSendMessage,BankCardNO,numState,A.dt_Add,A.dt_Update");
            strSql.Append("  FROM dbo.Depositor A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Cun C ON A.CunID =C.ID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Xiang D ON C.XiangID=D.ID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Xian E ON D.XianID=E.ID");
            strSql.Append("  where 1=1");
            strSql.Append(" and A.ISClosing=0");//排除销户储户
            strSql.Append(" and A.numState=1");//排除挂失储户
            if (!(Convert.ToBoolean(objISHQ)))//如果不是总部，则只能修改自己的社员
            {
                strSql.Append("  and B.ID="+WBID);
            }

            strSql.Append(" and A.AccountNumber=@AccountNumber ");
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

        void Update_Depositor(HttpContext context)
        {

            string WBID = context.Session["WB_ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string strPassword = context.Request.Form["strPassword"].ToString();

            string XianID = context.Request.Form["XianID"].ToString();
            string XiangID = context.Request.Form["XiangID"].ToString();
            string BD_Address_CunID = context.Request.Form["CunID"].ToString();
            string strAddress = context.Request.Form["strAddress"].ToString();
            string IDCard = context.Request.Form["IDCard"].ToString();
            string PhoneNO = context.Request.Form["PhoneNO"].ToString();
            int ISSendMessage = 0;
            if (context.Request.Form["ISSendMessage"] == null)
            {
                ISSendMessage = 1;
            }
         


            StringBuilder strSql = new StringBuilder();
            strSql.Append(" UPDATE dbo.Depositor  SET");
            strSql.Append(" strName='"+strName+"'");
            if (strPassword.Trim() != "")
            {
                strPassword = Fun.GetMD5_32(strPassword);
                strSql.Append(" ,strPassword='" + strPassword + "'");
            }
            strSql.Append(" ,XianID=" + XianID);
            strSql.Append(" ,XiangID=" + XiangID);
            strSql.Append(" ,CunID=" + BD_Address_CunID);
            strSql.Append(" ,strAddress='" + strAddress+"'");
            strSql.Append(" ,IDCard='" + IDCard + "'");
            strSql.Append(" ,PhoneNO='" + PhoneNO + "'");
            strSql.Append(" ,ISSendMessage="+ ISSendMessage);
           
            strSql.Append(" WHERE AccountNumber='" + AccountNumber + "'");


            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetDep_Integral(HttpContext context)
        {
           
            string WBID = context.Request.Form["WBID"].ToString().Trim();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString().Trim();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  select s.AccountNumber,B.strName,s.integral_Total");
            strSql.Append("  FROM (");
            strSql.Append("   SELECT *,ROW_NUMBER() OVER (PARTITION BY AccountNumber ORDER BY ID DESC) as groupIndex");
            strSql.Append("   FROM dbo.Dep_Integral)s");
            strSql.Append("  LEFT OUTER JOIN dbo.Depositor B ON s.AccountNumber=B.AccountNumber");
            strSql.Append("  WHERE s.groupIndex=1 ");
            if (WBID != "0"&&WBID!="") {
                strSql.Append("   AND B.WBID=" + WBID);
            }
            strSql.Append(" and B.ISClosing=0");//排除销户储户
            strSql.Append(" and B.numState=1");//排除挂失储户
            if (AccountNumber!="")
            {
                strSql.Append(string.Format(" AND s.AccountNumber='{0}'",AccountNumber));
            }

           
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
        /// 储户积分变化详细
        /// </summary>
        /// <param name="context"></param>
        void GetDep_IntegralDetail(HttpContext context)
        {


            string AccountNumber = context.Request.Form["AccountNumber"].ToString().Trim();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT case (A.numType) when 1 then '储户开户推荐' when 2 then '积分兑换' when 3 then '首次存粮' end as  strnumType");
            strSql.Append(" ,A.numType,A.AccountNumber,B.strName,B.PhoneNO,AccountNumber_New,GEIntegralID,numLevel,integral_Change,integral_Total,CONVERT(varchar(100), A.dt_Add, 23) AS dt_Add");
            strSql.Append("  FROM dbo.Dep_Integral A LEFT OUTER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSql.Append(string.Format(" WHERE A.AccountNumber='{0}'",AccountNumber));
         

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
        /// 储户单项积分详细
        /// </summary>
        /// <param name="context"></param>
        void GetDep_IntegralSingle(HttpContext context)
        {


            string numType = context.Request.Form["numType"].ToString().Trim();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString().Trim();
            string AccountNumber_New = context.Request.Form["AccountNumber_New"].ToString().Trim();
            string GEIntegralID = context.Request.Form["GEIntegralID"].ToString().Trim();
            StringBuilder strReturn = new StringBuilder();
            if (numType == "1")//推荐储户开户 
            {
                string sql = string.Format(" select strName,AccountNumber, CONVERT(NVARCHAR(100),dt_Add,23) AS dt_Add from Depositor where AccountNumber ='{0}'", AccountNumber_New);
                DataTable dt = SQLHelper.ExecuteDataTable(sql);
                if (dt != null && dt.Rows.Count != 0) {
                    strReturn.Append("<span>开户人:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["strName"].ToString() + "</span>");
                    strReturn.Append("<span>账号:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["AccountNumber"].ToString() + "</span>");
                    strReturn.Append("<span>时间:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["dt_Add"].ToString() + "</span>");
                    strReturn.Append("<span>被推荐人:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + AccountNumber + "</span>");
                    }
            }
            else if (numType == "2")//积分兑换商品
            {
                string sql = string.Format("  SELECT GoodName,GoodCount,integral_change,CONVERT(NVARCHAR(100),dt_Integral,23) AS dt_Integral FROM dbo.GoodExchangeIntegral WHERE ID={0}", GEIntegralID);
                DataTable dt = SQLHelper.ExecuteDataTable(sql);
                if (dt != null && dt.Rows.Count != 0)
                {
                    strReturn.Append("<span>商品名:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["GoodName"].ToString() + "</span>");
                    strReturn.Append("<span>数量:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["GoodCount"].ToString() + "</span>");
                    strReturn.Append("<span>消费积分:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["integral_change"].ToString() + "</span>");
                    strReturn.Append("<span>时间:</span><span style='font-weight:bold;color:blue;'>" + dt.Rows[0]["dt_Integral"].ToString() + "</span>");
                }
            
            }

            else if (numType == "3")//首次存粮
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("   SELECT A.AccountNumber,StorageNumberRaw,CONVERT(NVARCHAR(100),StorageDate,23) AS  StorageDate,B.strName AS VarietyName ");
                sql.Append(" FROM dbo.Dep_StorageInfo A LEFT OUTER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID ");
                sql.Append(string.Format(" WHERE A.ID={0}",GEIntegralID));

                DataTable dt = SQLHelper.ExecuteDataTable(sql.ToString());
                if (dt != null && dt.Rows.Count != 0)
                {
                    strReturn.Append("<span>存粮储户:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["AccountNumber"].ToString() + "</span>");
                    strReturn.Append("<span>存粮品类:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["VarietyName"].ToString() + "</span>");
                    strReturn.Append("<span>数量:</span><span style='font-weight:bold;color:blue;margin-right:10px;'>" + dt.Rows[0]["StorageNumberRaw"].ToString() + "</span>");
                    strReturn.Append("<span>时间:</span><span style='font-weight:bold;color:blue;'>" + dt.Rows[0]["StorageDate"].ToString() + "</span>");
                }

            }

            context.Response.Write(strReturn.ToString());

          
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

        /// <summary>
        /// 查询当前网点今日到期的定期存储个数
        /// </summary>
        /// <param name="context"></param>
        void GetExpireStorageCount(HttpContext context) {
            string WBID = context.Session["WB_ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT Count(A.ID)");
            strSql.Append("   FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" INNER JOIN dbo.Depositor C ON A.AccountNumber=C.AccountNumber");
            strSql.Append(string.Format("   WHERE B.ISRegular=1 AND A.WBID={0}", WBID));
            strSql.Append(" and C.ISClosing=0");//排除销户储户
            strSql.Append(" and C.numState=1");//排除挂失储户
            strSql.Append(" and A.StorageNumber>0");//排除挂失储户
            strSql.Append("   AND B.numStorageDate-DATEDIFF(DAY,A.InterestDate,GETDATE()) =0");//只查询今天到期的存粮
            object objcount = SQLHelper.ExecuteScalar(strSql.ToString());
            if (Convert.ToInt32(objcount) > 0)
            {
                context.Response.Write("Y");
            }
            else {
                context.Response.Write("N");
            }
        }

        /// <summary>
        /// 当点网点的定期存粮信息
        /// </summary>
        /// <param name="context"></param>
        void GetRegularStorage(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string optionType = context.Request.Form["optionType"].ToString();
            string optionTime = context.Request.Form["optionTime"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.ID,A.AccountNumber,C.strName AS Dep_Name,C.PhoneNO, D.strName AS VarietyName,B.strName AS TimeName,A.StorageNumberRaw,A.StorageNumber,");
            strSql.Append("   CONVERT(varchar(100), A.StorageDate, 23) AS StorageDate,CONVERT(varchar(100), A.InterestDate, 23) AS InterestDate,");
            strSql.Append("  B.numStorageDate,DATEDIFF(DAY,A.InterestDate,GETDATE()) AS daycount,B.numStorageDate-DATEDIFF(DAY,A.InterestDate,GETDATE()) AS daylast");
            strSql.Append("   FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" INNER JOIN dbo.Depositor C ON A.AccountNumber=C.AccountNumber");
            strSql.Append(" LEFT OUTER JOIN dbo.StorageVariety D ON A.VarietyID=D.ID");
            strSql.Append(string.Format("   WHERE B.ISRegular=1 AND A.WBID={0}",WBID));
            if (AccountNumber.Trim() != "") {
                strSql.Append(string.Format("   AND A.AccountNumber ='{0}'", AccountNumber));
            }
            strSql.Append(" and A.StorageNumber>0");//排除已完结的存粮
            strSql.Append(" and C.ISClosing=0");//排除销户储户
            strSql.Append(" and C.numState=1");//排除挂失储户
            if (optionType == "1")//已到期存储
            {
                 strSql.Append("  AND B.numStorageDate-DATEDIFF(DAY,A.InterestDate,GETDATE())<0");
            }
            if (optionType == "2")//近期到期存储(查询一周之内的存储)
            {
                strSql.Append("   AND B.numStorageDate-DATEDIFF(DAY,A.InterestDate,GETDATE())>-1");
                strSql.Append("   AND B.numStorageDate-DATEDIFF(DAY,A.InterestDate,GETDATE())<" + optionTime);
            }
            strSql.Append(" ORDER BY daylast ");//按照到期时间排序
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            if (dt==null||dt.Rows.Count==0)
            {
                context.Response.Write("error");
            }
            else
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
        }



        /// <summary>
        /// 当点网点的活期存粮信息
        /// </summary>
        /// <param name="context"></param>
        void GetCurrentStorage(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.AccountNumber,A.strName,A.PhoneNO,A.strAddress,");
            strSql.Append(" T.strName+V.strName AS VarietyName,CONVERT(VARCHAR(100),B.StorageDate,23) AS StorageDate,B.StorageNumber,B.Price_ShiChang,B.CurrentRate,DATEDIFF(DAY,B.StorageDate,GETDATE()) AS daycount");
            strSql.Append(" FROM dbo.Depositor A INNER JOIN dbo.Dep_StorageInfo B ON A.AccountNumber=B.AccountNumber");
            strSql.Append(" LEFT OUTER JOIN dbo.StorageTime T ON B.TimeID=T.ID");
            strSql.Append(" LEFT OUTER JOIN dbo.StorageVariety V ON B.VarietyID=V.ID");
            strSql.Append(" WHERE T.ISRegular=0 AND A.numState=1 AND A.ISClosing=0 AND B.StorageNumber>0");
            strSql.Append(string.Format("  AND A.WBID={0}", WBID));
            if (AccountNumber.Trim() != "")
            {
                strSql.Append(string.Format("   AND A.AccountNumber ='{0}'", AccountNumber));
            }
            if (Qdtstart.Trim() != "")
            {
                strSql.Append(string.Format("   AND B.StorageDate >'{0}'", Qdtstart));
            }
            if (Qdtend.Trim() != "")
            {
                Qdtend = Convert.ToDateTime(Qdtend).AddDays(1).ToString();
                strSql.Append(string.Format("   AND B.StorageDate <'{0}'", Qdtend));
            }
            strSql.Append(" ORDER BY strAddress,A.AccountNumber,StorageDate");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            if (dt == null || dt.Rows.Count == 0)
            {
                context.Response.Write("error");
            }
            else
            {
                context.Response.Write(JsonHelper.ToJson(dt));
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