using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Web.SessionState;


namespace Web.Ashx
{
    /// <summary>
    /// settlebasic 的摘要说明
    /// </summary>
    public class settlebasic : IHttpHandler, IRequiresSessionState
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

                    case "ISExitSA_Account": ISExitSA_Account(context); break;
                    case "PrintSAAccount": PrintSAAccount(context); break;//网点账户的存折打印

                    case "Get_WBSA": Get_WBSA(context); break;
                    case "Get_Address_Xian": Get_Address_Xian(context); break;
                    case "Get_Address_Xiang": Get_Address_Xiang(context); break;
                    case "Get_Address_Cun": Get_Address_Cun(context); break;
                    case "GetNewAccountNumber": GetNewAccountNumber(context); break;
                    case "Add_SA_Account": Add_SA_Account(context); break;
                    case "Update_SA_Account": Update_SA_Account(context); break;
                    case "DeleteByID_SA_Account": DeleteByID_SA_Account(context); break;
                    case "Get_SA_Account": Get_SA_Account(context); break;
                    case "Get_SA_AccountByID": Get_SA_AccountByID(context); break;
                    case "Get_SA_AccountAll": Get_SA_AccountAll(context); break;

                    case "GetAccountant": GetAccountant(context); break;
                    case "GetByID_Accountant": GetByID_Accountant(context); break;
                    case "Add_Accountant": Add_Accountant(context); break;
                    case "Update_Accountant": Update_Accountant(context); break;
                    case "DeleteByID_Accountant": DeleteByID_Accountant(context); break;

                    case "GetStockType": GetStockType(context); break;
                    case "GetByID_StockType": GetByID_StockType(context); break;
                    case "Add_StockType": Add_StockType(context); break;
                    case "Update_StockType": Update_StockType(context); break;
                    case "DeleteByID_StockType": DeleteByID_StockType(context); break;

                    case "GetWeigh": GetWeigh(context); break;
                    case "GetByID_Weigh": GetByID_Weigh(context); break;
                    case "Add_Weigh": Add_Weigh(context); break;
                    case "Update_Weigh": Update_Weigh(context); break;
                    case "DeleteByID_Weigh": DeleteByID_Weigh(context); break;

                    case "GetQuality": GetQuality(context); break;
                    case "GetByID_Quality": GetByID_Quality(context); break;
                    case "Add_Quality": Add_Quality(context); break;
                    case "Update_Quality": Update_Quality(context); break;
                    case "DeleteByID_Quality": DeleteByID_Quality(context); break;

                    case "GetWBWH": GetWBWH(context); break;   

                }
            }

        }

        //查询是否存在此账号信息
        void ISExitSA_Account(HttpContext context)
        {
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT numState  FROM dbo.SA_Account  WHERE AccountNumber='"+AccountNumber+"'");

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());

            if (obj==null||obj.ToString()=="")
            {
                context.Response.Write("-1");
            }
            else
            {
                if (Convert.ToBoolean(obj) == true)
                {
                    context.Response.Write("1");
                }
                else {
                    context.Response.Write("0");
                }
            }
        }


        /// <summary>
        /// 获取储户的打印信息
        /// </summary>
        /// <param name="context"></param>
        void PrintSAAccount(HttpContext context)
        {
            //获取需要打印的信息
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();

            StringBuilder strSqlCommume = new StringBuilder();
            strSqlCommume.Append("SELECT A.AccountNumber,A.strName,A.strAddress,A.IDCard,B.strName AS WBID,'密码' AS OperateWay, PhoneNO, CONVERT(NVARCHAR(100),A.dt_Add,23) AS dt_Add");
            strSqlCommume.Append(" FROM dbo.SA_Account A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlCommume.Append(" WHERE A.AccountNumber='" + AccountNumber + "'");


            DataTable dtCommune = SQLHelper.ExecuteDataTable(strSqlCommume.ToString());
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
            string dt_Commune = dtCommune.Rows[0]["dt_Add"].ToString();
            string OperateWay = dtCommune.Rows[0]["OperateWay"].ToString();


            string strWBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += "  where 1=1 and WBID=" + strWBID;

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
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;'>" + IDCard + "</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR4C1Y) - Convert.ToInt32(HomeR3C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR3C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR3C2X) - Convert.ToInt32(HomeR3C1X)).ToString() + "px;'>" + WBID + "</td>");
                strReturn.Append("  <td style='font-size:14px; font-weight:bold;'>" + OperateWay + "</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR5C1Y) - Convert.ToInt32(HomeR4C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR4C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR4C2X) - Convert.ToInt32(HomeR4C1X)).ToString() + "px;'>" + PhoneNO + "</td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;' >" + dt_Commune + "</td>");

                strReturn.Append("   </tr></table>");


                context.Response.Write(strReturn.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 获取还没有原粮出库开户的网点
        /// </summary>
        /// <param name="context"></param>
        void Get_WBSA(HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT ID,strName  FROM dbo.WB WHERE ISHQ=0 AND ISSimulate=0 AND ID NOT IN (SELECT DISTINCT WBID FROM dbo.SA_Account)");

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
            string SerialNumber = "999";
            string AccountNumver = SerialNumber + "0000001";

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT TOP 1 AccountNumber  FROM dbo.SA_Account ORDER BY ID DESC");


            object obj = SQLHelper.ExecuteScalar(strSql.ToString());

            if (obj != null)
            {
                int numIndex = Convert.ToInt32(obj.ToString().Substring(3));
                AccountNumver = SerialNumber + Fun.ConvertIntToString(numIndex + 1, 7);
                context.Response.Write(AccountNumver);
            }
            else
            {
                context.Response.Write(AccountNumver);
            }
        }


        void Add_SA_Account(HttpContext context)
        {

            string WBID = context.Request.Form["WBID"].ToString();//从提交表单获取 
            string strName = context.Request.Form["strName"].ToString();
            string AccountNumber = context.Request.Form["AccountNumber"].ToString();


            string XianID = context.Request.Form["XianID"].ToString();
            string XiangID = context.Request.Form["XiangID"].ToString();
            string CunID = context.Request.Form["CunID"].ToString();
            string IDCard = context.Request.Form["IDCard"].ToString();
            string PhoneNO = context.Request.Form["PhoneNO"].ToString();

            string strAddress = context.Request.Form["strAddress"].ToString();

            string strSameAccount = "  SELECT  COUNT(ID)  FROM dbo.SA_Account WHERE AccountNumber='" + AccountNumber + "' or WBID=" + WBID;
            if (SQLHelper.ExecuteScalar(strSameAccount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SA_Account] (");
            strSql.Append("AccountNumber,WBID,strPassword,XianID,XiangID,CunID,strAddress,strName,IDCard,PhoneNO,ISSendMessage,numState,dt_Add)");
            strSql.Append(" values (");
            strSql.Append("@AccountNumber,@WBID,@strPassword,@XianID,@XiangID,@CunID,@strAddress,@strName,@IDCard,@PhoneNO,@ISSendMessage,@numState,@dt_Add)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@strPassword", SqlDbType.NVarChar,50),
					new SqlParameter("@XianID", SqlDbType.Int,4),
					new SqlParameter("@XiangID", SqlDbType.Int,4),
					new SqlParameter("@CunID", SqlDbType.Int,4),
					new SqlParameter("@strAddress", SqlDbType.NChar,10),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@IDCard", SqlDbType.NVarChar,50),
					new SqlParameter("@PhoneNO", SqlDbType.NVarChar,50),
					new SqlParameter("@ISSendMessage", SqlDbType.Bit,1),
					new SqlParameter("@numState", SqlDbType.Int,4),
					new SqlParameter("@dt_Add", SqlDbType.DateTime)};
            parameters[0].Value = AccountNumber;
            parameters[1].Value = WBID;
            parameters[2].Value = "";
            parameters[3].Value = XianID;
            parameters[4].Value = XiangID;
            parameters[5].Value = CunID;
            parameters[6].Value = strAddress;
            parameters[7].Value = strName;
            parameters[8].Value = IDCard;
            parameters[9].Value = PhoneNO;
            parameters[10].Value = 1;
            parameters[11].Value = 1;
            parameters[12].Value = DateTime.Now;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_SA_Account(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string XianID = context.Request.Form["XianID"].ToString();
            string XiangID = context.Request.Form["XiangID"].ToString();
            string CunID = context.Request.Form["CunID"].ToString();
            string IDCard = context.Request.Form["IDCard"].ToString();
            string PhoneNO = context.Request.Form["PhoneNO"].ToString();
            string strAddress = context.Request.Form["strAddress"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [SA_Account] set ");
            strSql.Append(string.Format("  XianID={0},",XianID));
            strSql.Append(string.Format("  XiangID={0},", XiangID));
            strSql.Append(string.Format("  CunID={0},", CunID));
            strSql.Append(string.Format("  strAddress='{0}',", strAddress));
            strSql.Append(string.Format("  strName='{0}',", strName));
            strSql.Append(string.Format("  IDCard='{0}',", IDCard));
            strSql.Append(string.Format("  PhoneNO='{0}'", PhoneNO));
            strSql.Append(string.Format("  where  ID={0}", ID));
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_SA_Account(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();
            string SA_AN = context.Request.QueryString["SA_AN"].ToString();
            if (SQLHelper.ExecuteScalar("SELECT COUNT (ID) FROM dbo.SA_CheckOut WHERE SA_AN='"+SA_AN+"'").ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [SA_Account] ");
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


        void Get_SA_Account(HttpContext context)
        {
            string AccountNumber = context.Request.QueryString["AccountNumber"].ToString();
            string WBID = context.Request.QueryString["WBID"].ToString();
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select ID,AccountNumber,WBID,strPassword,XianID,XiangID,CunID,strAddress,strName,IDCard,PhoneNO,ISSendMessage,numState,dt_Add ");
            strSql.Append(" FROM dbo.SA_Account");
            strSql.Append(" WHERE AccountNumber='"+AccountNumber+"' AND WBID="+WBID);

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt!=null&&dt.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Get_SA_AccountByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
          
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.ID,A.AccountNumber,B.strName AS WBName,A.XianID,A.XiangID,A.CunID,A.strAddress,A.strName,A.IDCard,A.PhoneNO,A.numState,A.dt_Add");
            strSql.Append("  FROM dbo.SA_Account A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append(" WHERE A.ID=" + ID);

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

        void Get_SA_AccountAll(HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.ID,A.AccountNumber,B.strName AS WBName,A.XianID,A.XiangID,A.CunID,A.strAddress,A.strName,A.IDCard,A.PhoneNO,A.numState,A.dt_Add");
            strSql.Append("  FROM dbo.SA_Account A INNER JOIN dbo.WB B ON A.WBID=B.ID");
          
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

        void GetStockType(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [SABD_StockType] ");

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

        void GetByID_StockType(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [SABD_StockType] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = ID;
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

        void Add_StockType(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Add("SABD_StockType", "strName", strName))
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SABD_StockType] (");
            strSql.Append("strName,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@strName,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = false;
            parameters[2].Value = 1;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_StockType(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("SABD_StockType", "strName", strName, ID))
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [SABD_StockType] set ");
            strSql.Append("strName=@strName");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_StockType(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [SABD_StockType] ");
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

        void GetWeigh(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [SABD_Weigh] ");

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

        void GetByID_Weigh(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [SABD_Weigh] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = ID;
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

        void Add_Weigh(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Add("SABD_Weigh", "strName", strName))
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SABD_Weigh] (");
            strSql.Append("strName,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@strName,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = false;
            parameters[2].Value = 1;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Weigh(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("SABD_Weigh", "strName", strName, ID))
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [SABD_Weigh] set ");
            strSql.Append("strName=@strName");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_Weigh(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [SABD_Weigh] ");
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

        void GetQuality(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [SABD_Quality] ");

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

        void GetByID_Quality(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [SABD_Quality] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = ID;
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

        void Add_Quality(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Add("SABD_Quality", "strName", strName))
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SABD_Quality] (");
            strSql.Append("strName,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@strName,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = false;
            parameters[2].Value = 1;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Quality(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("SABD_Quality", "strName", strName, ID))
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [SABD_Quality] set ");
            strSql.Append("strName=@strName");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_Quality(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [SABD_Quality] ");
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

        void GetAccountant(HttpContext context)
        {
          
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [SABD_Accountant] ");
          
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

        void GetByID_Accountant(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [SABD_Accountant] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = ID;
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

        void Add_Accountant(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Add("SABD_Accountant", "strName", strName))
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [SABD_Accountant] (");
            strSql.Append("strName,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@strName,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = false;
            parameters[2].Value = 1;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Accountant(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("SABD_Accountant", "strName", strName,ID))
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [SABD_Accountant] set ");
            strSql.Append("strName=@strName");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_Accountant(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [SABD_Accountant] ");
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
        /// 获取网店仓库信息
        /// </summary>
        /// <param name="context"></param>
        void GetWBWH(HttpContext context)
        {
            string WBID = context.Request.QueryString["WBID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName FROM  dbo.WBWareHouse WHERE WB_ID="+WBID);
        

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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}