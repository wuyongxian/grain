using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;
using Newtonsoft.Json;


namespace Web.Admin.Info
{
    /// <summary>
    /// info 的摘要说明
    /// </summary>
    public class info : IHttpHandler, IRequiresSessionState
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
                  
                    case "HasReply": HasReply(context); break;
                    case "Get_InfoType": Get_InfoType(context); break;
                    case "Add_InfoType": Add_InfoType(context); break;
                    case "Update_InfoType": Update_InfoType(context); break;
                    case "DeleteByID_InfoType": DeleteByID_InfoType(context); break;

                    case "UpdateGetByID_Info": UpdateGetByID_Info(context); break;
                    case "GetByID_Info": GetByID_Info(context); break;
                    case "Add_Info": Add_Info(context); break;
                    case "Update_Info": Update_Info(context); break;
                    case "Update_InfoBroswerTime": Update_InfoBroswerTime(context); break;
                    case "DeleteByID_Info": DeleteByID_Info(context); break;
                    case "Add_Reply": Add_Reply(context); break;                 
                    case "Get_InfoNotice": Get_InfoNotice(context); break;
                    case "Update_InfoNotice": Update_InfoNotice(context); break;

                    case "getWBContact": getWBContact(context); break;

                    case "Add_TSReq": Add_TSReq(context); break;
                    case "Add_TSRes": Add_TSRes(context); break;
                    case "Update_TSReq_State": Update_TSReq_State(context); break;
                    case "Update_TSReqList_State": Update_TSReqList_State(context); break;
                    case "Update_TSResList_State": Update_TSResList_State(context); break;
                    case "Update_TSRes_State": Update_TSRes_State(context); break;
                    case "Get_TSReq": Get_TSReq(context); break;
                    case "Get_TSReqList": Get_TSReqList(context); break;
                    case "Get_TSRes": Get_TSRes(context); break;
                    case "Get_TSResList": Get_TSResList(context); break;
                }
            }

        }

      
        void HasReply(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            object obj = SQLHelper.ExecuteScalar("   SELECT COUNT(ID)  FROM dbo.InfoReply WHERE InfoID=" + ID);


            if (obj.ToString()=="0")
            {
                context.Response.Write("0");
            }
            else
            {
                context.Response.Write("1");
            }
        }

        void Get_InfoType(HttpContext context)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,strType as strName FROM dbo.InfoType order by ISDefault desc,numSort asc");
           
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

        void Add_InfoType(HttpContext context)
        {

            string strType = context.Request.Form["strType"].ToString();


            if (!common.UniqueCheck_Add("InfoType", "strType", strType))
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" INSERT INTO dbo.InfoType( strType, ISDefault, numSort )");
            strSql.Append(" VALUES  ( N'"+strType+"',  0,  1 )");

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_InfoType(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string strType = context.Request.Form["strType"].ToString();
            if (!common.UniqueCheck_Update("InfoType", "strType", strType, ID))
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" UPDATE dbo.InfoType SET strType='"+strType+"' WHERE ID="+ID);
           

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_InfoType(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Info WHERE InfoTypeID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [InfoType] ");
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
        /// 更新浏览次数后在获取
        /// </summary>
        /// <param name="context"></param>
        void UpdateGetByID_Info(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();


            StringBuilder strSqlCount = new StringBuilder();
            strSqlCount.Append(" UPDATE dbo.Info  ");
            strSqlCount.Append(" SET BrowseTime=(SELECT BrowseTime+1  FROM dbo.Info WHERE ID=" + ID + ")");
            strSqlCount.Append(" WHERE dbo.Info.ID=" + ID);

            SQLHelper.ExecuteNonQuery(strSqlCount.ToString());

            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select A.ID,InfoTypeID,B.strRealName AS  UserID,strTitle,strContent,ISStick,ISKeepSecret,BrowseTime,A.dt_Add,A.dt_Update ");
            strSql.Append("   FROM dbo.Info A INNER JOIN dbo.Users B ON A.UserID=B.ID ");
            strSql.Append("  WHERE A.ID=" + ID);
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

        void GetByID_Info(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select A.ID,InfoTypeID,B.strRealName AS  UserID,strTitle,strContent,ISStick,ISKeepSecret,BrowseTime,A.dt_Add,A.dt_Update ");
            strSql.Append("   FROM dbo.Info A INNER JOIN dbo.Users B ON A.UserID=B.ID ");
            strSql.Append("  WHERE A.ID=" + ID);
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

        void Add_Info(HttpContext context)
        {

            string InfoTypeID = context.Request.Form["InfoTypeID"].ToString();
            string UserID = context.Session["ID"].ToString();
            string strTitle = context.Request.Form["strTitle"].ToString();
            string strContent = context.Request.Form["strContent"].ToString();
            bool ISStick = false;
            if (context.Request.Form["ISStick"] != null)
            {
                ISStick = true;
            }
            bool ISKeepSecret = false;
            if (context.Request.Form["ISKeepSecret"] != null)
            {
                ISKeepSecret = true;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Info] (");
            strSql.Append("InfoTypeID,UserID,strTitle,strContent,ISStick,ISKeepSecret,BrowseTime,dt_Add,dt_Update)");
            strSql.Append(" values (");
            strSql.Append("@InfoTypeID,@UserID,@strTitle,@strContent,@ISStick,@ISKeepSecret,@BrowseTime,@dt_Add,@dt_Update)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@InfoTypeID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@strTitle", SqlDbType.NVarChar,100),
					new SqlParameter("@strContent", SqlDbType.NVarChar,4000),
					new SqlParameter("@ISStick", SqlDbType.Bit,1),
					new SqlParameter("@ISKeepSecret", SqlDbType.Bit,1),
					new SqlParameter("@BrowseTime", SqlDbType.Int,4),
					new SqlParameter("@dt_Add", SqlDbType.DateTime),
					new SqlParameter("@dt_Update", SqlDbType.DateTime)};
            parameters[0].Value = InfoTypeID;
            parameters[1].Value = UserID;
            parameters[2].Value = strTitle;
            parameters[3].Value = strContent;
            parameters[4].Value = ISStick;
            parameters[5].Value = ISKeepSecret;
            parameters[6].Value = 0;
            parameters[7].Value = DateTime.Now;
            parameters[8].Value = DateTime.Now;
            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Info(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string InfoTypeID = context.Request.Form["InfoTypeID"].ToString();
            string strTitle = context.Request.Form["strTitle"].ToString();
            string strContent = context.Request.Form["strContent"].ToString();
            bool ISStick = false;
            if (context.Request.Form["strContent"] != null)
            {
                ISStick = true;
            }
            bool ISKeepSecret = false;
            if (context.Request.Form["ISKeepSecret"] != null)
            {
                ISKeepSecret = true;
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [Info] set ");
            strSql.Append("InfoTypeID=@InfoTypeID,");
            strSql.Append("strTitle=@strTitle,");
            strSql.Append("strContent=@strContent,");
            strSql.Append("ISStick=@ISStick,");
            strSql.Append("ISKeepSecret=@ISKeepSecret,");
            strSql.Append("dt_Update=@dt_Update");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@InfoTypeID", SqlDbType.Int,4),
					new SqlParameter("@strTitle", SqlDbType.NVarChar,100),
					new SqlParameter("@strContent", SqlDbType.NVarChar,4000),
					new SqlParameter("@ISStick", SqlDbType.Bit,1),
					new SqlParameter("@ISKeepSecret", SqlDbType.Bit,1),
					new SqlParameter("@dt_Update", SqlDbType.DateTime),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = InfoTypeID;
            parameters[1].Value = strTitle;
            parameters[2].Value = strContent;
            parameters[3].Value = ISStick;
            parameters[4].Value = ISKeepSecret;
            parameters[5].Value = DateTime.Now;
            parameters[6].Value = ID;
            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }
        void Update_InfoBroswerTime(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" UPDATE dbo.Info  ");
            strSql.Append(" SET BrowseTime=(SELECT BrowseTime+1  FROM dbo.Info WHERE ID="+ID+")");
            strSql.Append(" WHERE dbo.Info.ID="+ID);
           
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void DeleteByID_Info(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [Info] ");
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
        /// 为一条信息添加评论
        /// </summary>
        /// <param name="context"></param>
        void Add_Reply(HttpContext context)
        {

            string InfoID = context.Request.QueryString["ID"].ToString();

            string UserID = context.Session["ID"].ToString();
            string strContent = context.Request.Form["strContent"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" INSERT  INTO dbo.InfoReply( InfoID ,UserID , strContent , dt_Reply )");
            strSql.Append(" VALUES  ( "+InfoID+" ,  "+UserID+" , N'"+strContent+"' ,  N'"+DateTime.Now+"'  )");
           
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void Get_InfoNotice(HttpContext context)
        {
           
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT top 1 strContent FROM dbo.InfoNotice");

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
            if (obj != null)
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("");
            }
        }
        void Update_InfoNotice(HttpContext context)
        {
            string strContent=context.Request.Form["strContent"].ToString();
            string strNoticeCount = SQLHelper.ExecuteScalar(" select count(ID) from InfoNotice").ToString();
            string strSql = "";
            if (strNoticeCount == "0")
            {
                strSql = " INSERT INTO dbo.InfoNotice ( strContent )VALUES  ( N'" + strContent + "' )";
            }
            else {
                strSql = " UPDATE dbo.InfoNotice SET strContent='"+strContent+"'";
            }
            
            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 获取网点的默认联系人
        /// </summary>
        /// <param name="context"></param>
        void getWBContact(HttpContext context)
        {
            string webSiteCode = common.GetCompanyInfo()["webSiteCode"].ToString();
            string ComName = common.GetCompanyInfo()["strName"].ToString();
            string userID=context.Session["ID"].ToString();
            string sql = string.Format("SELECT B.strName AS WBName,B.strAddress,A.strRealName,A.strPhone FROM dbo.Users A INNER JOIN dbo.WB B ON A.WB_ID=B.ID WHERE A.ID={0}", userID);
            DataTable dt = SQLHelper.ExecuteDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                var res = new { state = "error", msg = "查询登陆用户的信息失败!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else {
                ComName=ComName+"•"+dt.Rows[0]["WBName"].ToString();
                string ComAddresss=dt.Rows[0]["strAddress"].ToString();
                 string Contact=dt.Rows[0]["strRealName"].ToString();
                 string ContactPhoneNo=dt.Rows[0]["strPhone"].ToString();
                 var res = new { state = "success", msg = "查询登陆用户的信息成功!", webSiteCode = webSiteCode, ComName = ComName, ComAddresss = ComAddresss, Contact = Contact, ContactPhoneNo = ContactPhoneNo };
                context.Response.Write(JsonHelper.ToJson(res));
            }
        }

        /// <summary>
        /// 添加发布请求
        /// </summary>
        /// <param name="context"></param>
        void Add_TSReq(HttpContext context) {
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = context.Session["ID"].ToString();
            string webSiteCode = context.Request.Form["webSiteCode"].ToString();
            string reqSNNO = Fun.getGUID();//序列号
            string ReqNO = webSiteCode + Fun.GetSN();//序列号
            string tranType = context.Request.Form["tranType"].ToString();
            string tranTitle = context.Request.Form["tranTitle"].ToString();
            string CommodityName = context.Request.Form["CommodityName"].ToString();
            string CommodityLevel = context.Request.Form["CommodityLevel"].ToString();
            string Quantity = context.Request.Form["Quantity"].ToString();
            string UnitName = context.Request.Form["UnitName"].ToString();
            string QualityReq = context.Request.Form["QualityReq"].ToString();
            string FreightType = context.Request.Form["FreightType"].ToString();
            string PaymentType = context.Request.Form["PaymentType"].ToString();
            string AcceptStandard = context.Request.Form["AcceptStandard"].ToString();
            string Guarantee = context.Request.Form["Guarantee"].ToString();
            string Price_range = context.Request.Form["Price_range"].ToString();
            if (Price_range == "1")
            {
                string Price_min = context.Request.Form["Price_min"].ToString();
                string Price_max = context.Request.Form["Price_max"].ToString();
                Price_range = Price_min + "元/" + UnitName + "~" + Price_max + "元/" + UnitName;
            }
            string Date_begin = context.Request.Form["Date_begin"].ToString();
            string Date_end = context.Request.Form["Date_end"].ToString();
            string ComName = context.Request.Form["ComName"].ToString();
            string ComAddresss = context.Request.Form["ComAddresss"].ToString();
            string Contact = context.Request.Form["Contact"].ToString();
            string ContactPhoneNo = context.Request.Form["ContactPhoneNo"].ToString();
            string strRemark = context.Request.Form["strRemark"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into TransactionReq(");
            strSql.Append("WBID,UserID,webSiteCode,tranType,tranTitle,reqSNNO,ReqNO,CommodityName,CommodityLevel,Quantity,UnitName,QualityReq,AcceptStandard,strRemark,FreightType,PaymentType,Guarantee,Price_range,Date_begin,Date_end,ComName,ComAddresss,Contact,ContactPhoneNo,numState,dt_Add)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@UserID,@webSiteCode,@tranType,@tranTitle,@reqSNNO,@ReqNO,@CommodityName,@CommodityLevel,@Quantity,@UnitName,@QualityReq,@AcceptStandard,@strRemark,@FreightType,@PaymentType,@Guarantee,@Price_range,@Date_begin,@Date_end,@ComName,@ComAddresss,@Contact,@ContactPhoneNo,@numState,@dt_Add)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@webSiteCode", SqlDbType.NVarChar,50),
					new SqlParameter("@tranType", SqlDbType.Int,4),
					new SqlParameter("@tranTitle", SqlDbType.NVarChar,200),
                    new SqlParameter("@reqSNNO", SqlDbType.NVarChar,50),
					new SqlParameter("@ReqNO", SqlDbType.NVarChar,50),
					new SqlParameter("@CommodityName", SqlDbType.NVarChar,50),
					new SqlParameter("@CommodityLevel", SqlDbType.NVarChar,50),
					new SqlParameter("@Quantity", SqlDbType.Decimal,9),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@QualityReq", SqlDbType.NVarChar,1000),
					new SqlParameter("@AcceptStandard", SqlDbType.NVarChar,1000),
					new SqlParameter("@strRemark", SqlDbType.NVarChar,1000),
					new SqlParameter("@FreightType", SqlDbType.TinyInt,1),
					new SqlParameter("@PaymentType", SqlDbType.TinyInt,1),
					new SqlParameter("@Guarantee", SqlDbType.TinyInt,1),
					new SqlParameter("@Price_range", SqlDbType.NVarChar,100),
					new SqlParameter("@Date_begin", SqlDbType.DateTime),
					new SqlParameter("@Date_end", SqlDbType.DateTime),
					new SqlParameter("@ComName", SqlDbType.NVarChar,50),
					new SqlParameter("@ComAddresss", SqlDbType.NVarChar,50),
					new SqlParameter("@Contact", SqlDbType.NVarChar,50),
					new SqlParameter("@ContactPhoneNo", SqlDbType.NVarChar,50),
					new SqlParameter("@numState", SqlDbType.TinyInt,1),
					new SqlParameter("@dt_Add", SqlDbType.DateTime)};
            parameters[0].Value = WBID;
            parameters[1].Value = UserID;
            parameters[2].Value = webSiteCode;
            parameters[3].Value = tranType;
            parameters[4].Value = tranTitle;
            parameters[5].Value = reqSNNO;
            parameters[6].Value = ReqNO;
            parameters[7].Value = CommodityName;
            parameters[8].Value = CommodityLevel;
            parameters[9].Value = Quantity;
            parameters[10].Value = UnitName;
            parameters[11].Value = QualityReq;
            parameters[12].Value = AcceptStandard;
            parameters[13].Value = strRemark;
            parameters[14].Value = FreightType;
            parameters[15].Value = PaymentType;
            parameters[16].Value = Guarantee;
            parameters[17].Value = Price_range;
            parameters[18].Value = Date_begin;
            parameters[19].Value = Date_end;
            parameters[20].Value = ComName;
            parameters[21].Value = ComAddresss;
            parameters[22].Value = Contact;
            parameters[23].Value = ContactPhoneNo;
            parameters[24].Value = 1;
            parameters[25].Value = DateTime.Now;

            object obj = SQLHelper.ExecuteScalar(strSql.ToString(),parameters);
            if (obj == null)
            {
                var res = new { state = "error", msg = "添加发布请求失败!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = "success", msg = "添加发布请求成功!", ID = obj.ToString(), ReqNO = ReqNO, reqSNNO = reqSNNO };
                context.Response.Write(JsonHelper.ToJson(res));
            }

        }

        void Update_TSReq_State(HttpContext context)
        {
            string state = context.Request.Form["state"].ToString();
             string ReqNO = context.Request.Form["ReqNO"].ToString();
            string sql = string.Format(" update TransactionReq set numState={0} where ReqNO='{1}'",state,ReqNO);
            context.Response.Write(SQLHelper.ExecuteNonQuery(sql));
        }

        void Update_TSReqList_State(HttpContext context)
        {
            //reqstate req_model = new reqstate();
            string ReqNOList = context.Request.Form["ReqNOList"].ToString();
            DataTable dt = JsonHelper.JsonToDataTable(ReqNOList);
            StringBuilder sql = new StringBuilder();
            if (dt != null && dt.Rows.Count != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.Append(string.Format(" update TransactionReq set numState={0} where ReqNO = '{1}'", dt.Rows[i]["state"].ToString(), dt.Rows[i]["ReqNO"].ToString()));
                }
                using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
                {
                    try
                    {
                        SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql.ToString());
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

        //void Update_TSReqList_State(HttpContext context)
        //{
        //    string state = context.Request.Form["state"].ToString();
        //    string ReqNOList = context.Request.Form["ReqNOList"].ToString();
        //    string sql = string.Format(" update TransactionReq set numState={0} where ReqNO in ({1})", state, ReqNOList);
        //    context.Response.Write(SQLHelper.ExecuteNonQuery(sql));
        //}

        void Update_TSResList_State(HttpContext context)
        {

            string state = context.Request.Form["state"].ToString();
            string residlist = context.Request.Form["residlist"].ToString();
            string sql = string.Format(" update TransactionRes set numState={0} where resSNNO in ({1})", state, residlist);
            context.Response.Write(SQLHelper.ExecuteNonQuery(sql));
        }


        void Update_TSRes_State(HttpContext context)
        {
            string state = context.Request.Form["state"].ToString();
            string resSNNO = context.Request.Form["resSNNO"].ToString();
            string sql = string.Format(" update TransactionRes set numState={0} where resSNNO='{1}'", state, resSNNO);
            context.Response.Write(SQLHelper.ExecuteNonQuery(sql));
        }



        /// <summary>
        /// 获取发布请求
        /// </summary>
        /// <param name="context"></param>
        void Get_TSReq(HttpContext context) {
            string ReqNO = context.Request.Form["ReqNO"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 ID,WBID,UserID,webSiteCode,tranType,tranTitle,reqSNNO,ReqNO,CommodityName,CommodityLevel,Quantity,UnitName,QualityReq,AcceptStandard,strRemark,FreightType,PaymentType,Guarantee,Price_range,Date_begin,Date_end,ComName,ComAddresss,Contact,ContactPhoneNo,numState,dt_Add from TransactionReq ");
            strSql.Append(string.Format("where ReqNO='{0}'",ReqNO));

           
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("error");
            }
        }

        /// <summary>
        /// 获取发布请求
        /// </summary>
        /// <param name="context"></param>
        void Get_TSReqList(HttpContext context)
        {
            string key = context.Request.Form["key"].ToString().Trim();
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT WBID,UserID,webSiteCode,reqSNNO,ReqNO, CASE ( tranType) WHEN 1 THEN '买入' ELSE '卖出' END AS tranType,tranTitle,");
            strSql.Append(" CommodityName+CommodityLevel AS CommodityName,STR(Quantity)+UnitName AS Quantity,Price_range,");
            strSql.Append(" CONVERT(varchar(100), Date_begin, 23)+'~'+CONVERT(varchar(100), Date_end, 23) AS strdate,");
            strSql.Append(" CASE(numState) WHEN 1 THEN '已保存' WHEN 2 THEN '审核中' WHEN 3 THEN '已发布' ELSE '交易成功' END AS numState");
            strSql.Append(" FROM dbo.TransactionReq");
            strSql.Append(string.Format( " where WBID={0}",WBID));
            if (key != "") {
                strSql.Append(string.Format("  and tranTitle LIKE '%{0}%' OR CommodityName LIKE '%{0}%' ", key));
            }

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("error");
            }
        }

        //添加回复请求
        void Add_TSRes(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string UserID = context.Session["ID"].ToString();
            string webSiteCode = context.Request.Form["webSiteCode"].ToString();
            string resSNNO = Fun.getGUID();
            string ReqNO = context.Request.Form["ReqNO"].ToString();//序列号
            string tranType = context.Request.Form["tranType"].ToString();
            string tranTitle = context.Request.Form["tranTitle"].ToString();
            string CommodityName = context.Request.Form["CommodityName"].ToString();
            string Quantity_trade = context.Request.Form["Quantity_trade"].ToString();
            string UnitName = context.Request.Form["UnitName"].ToString();
            string Price_range = context.Request.Form["Price_range"].ToString();
            //if (Price_range == "1")
            //{
            //    string Price_min = context.Request.Form["Price_min"].ToString();
            //    string Price_max = context.Request.Form["Price_max"].ToString();
            //    Price_range = Price_min + "元/" + UnitName + "~" + Price_max + "元/" + UnitName;
            //}

            string ComName = context.Request.Form["ComName"].ToString();
            string ComAddresss = context.Request.Form["ComAddresss"].ToString();
            string Contact = context.Request.Form["Contact"].ToString();
            string ContactPhoneNo = context.Request.Form["ContactPhoneNo"].ToString();
            string strRemark = context.Request.Form["strRemark"].ToString();
            string numState = "1";
            string dt_Add = DateTime.Now.ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into TransactionRes(");
            strSql.Append("WBID,UserID,webSiteCode,tranTitle,tranType,resSNNO,ReqNO,CommodityName,Quantity_trade,UnitName,Price_range,strRemark,ComName,ComAddresss,Contact,ContactPhoneNo,numState,dt_Add)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@UserID,@webSiteCode,@tranTitle,@tranType,@resSNNO,@ReqNO,@CommodityName,@Quantity_trade,@UnitName,@Price_range,@strRemark,@ComName,@ComAddresss,@Contact,@ContactPhoneNo,@numState,@dt_Add)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@webSiteCode", SqlDbType.NVarChar,50),
					new SqlParameter("@tranTitle", SqlDbType.NVarChar,200),
					new SqlParameter("@tranType", SqlDbType.Int,4),
					new SqlParameter("@resSNNO", SqlDbType.NVarChar,50),
					new SqlParameter("@ReqNO", SqlDbType.NVarChar,50),
					new SqlParameter("@CommodityName", SqlDbType.NVarChar,50),
					new SqlParameter("@Quantity_trade", SqlDbType.Decimal,9),
					new SqlParameter("@UnitName", SqlDbType.NVarChar,50),
					new SqlParameter("@Price_range", SqlDbType.NVarChar,100),
					new SqlParameter("@strRemark", SqlDbType.NVarChar,1000),
					new SqlParameter("@ComName", SqlDbType.NVarChar,50),
					new SqlParameter("@ComAddresss", SqlDbType.NVarChar,50),
					new SqlParameter("@Contact", SqlDbType.NVarChar,50),
					new SqlParameter("@ContactPhoneNo", SqlDbType.NVarChar,50),
					new SqlParameter("@numState", SqlDbType.TinyInt,1),
					new SqlParameter("@dt_Add", SqlDbType.DateTime)};
            parameters[0].Value = WBID;
            parameters[1].Value = UserID;
            parameters[2].Value = webSiteCode;
            parameters[3].Value = tranTitle;
            parameters[4].Value = tranType;
            parameters[5].Value = resSNNO;
            parameters[6].Value = ReqNO;
            parameters[7].Value = CommodityName;
            parameters[8].Value = Quantity_trade;
            parameters[9].Value = UnitName;
            parameters[10].Value = Price_range;
            parameters[11].Value = strRemark;
            parameters[12].Value = ComName;
            parameters[13].Value = ComAddresss;
            parameters[14].Value = Contact;
            parameters[15].Value = ContactPhoneNo;
            parameters[16].Value = numState;
            parameters[17].Value = dt_Add;

            object obj = SQLHelper.ExecuteScalar(strSql.ToString(), parameters);
            if (obj == null)
            {
                var res = new { state = "error", msg = "添加回复请求失败!" };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else
            {
                var res = new { state = "success", msg = "添加回复请求成功!", ID = obj.ToString(), ReqNO = ReqNO, resSNNO = resSNNO };
                context.Response.Write(JsonHelper.ToJson(res));
            }

        }


        /// <summary>
        /// 获取发布回复
        /// </summary>
        /// <param name="context"></param>
        void Get_TSRes(HttpContext context)
        {
            string resSNNO = context.Request.Form["resSNNO"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 ID,WBID,UserID,webSiteCode,tranTitle,tranType,resSNNO,ReqNO,CommodityName,Quantity_trade,UnitName,Price_range,strRemark,ComName,ComAddresss,Contact,ContactPhoneNo,numState,dt_Add from TransactionRes ");
            strSql.Append(" where resSNNO=@resSNNO");
            SqlParameter[] parameters = {
					new SqlParameter("@resSNNO", SqlDbType.NVarChar,50)
			};
            parameters[0].Value = resSNNO;


            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("error");
            }
        }


        /// <summary>
        /// 获取发布回复
        /// </summary>
        /// <param name="context"></param>
        void Get_TSResList(HttpContext context)
        {
            string key = context.Request.Form["key"].ToString().Trim();
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT WBID,UserID,webSiteCode,resSNNO,ReqNO, CASE ( tranType) WHEN 1 THEN '买入' ELSE '卖出' END AS tranType,tranTitle,");
            strSql.Append(" CommodityName,STR(Quantity_trade)+UnitName AS Quantity,Price_range,");

            strSql.Append(" CASE(numState) WHEN 1 THEN '已保存' WHEN 2 THEN '审核中' WHEN 3 THEN '已发布' ELSE '交易成功' END AS numState");
            strSql.Append(" FROM dbo.TransactionRes");
            strSql.Append(string.Format(" where WBID={0}", WBID));
            if (key != "")
            {
                strSql.Append(string.Format("  and tranTitle LIKE '%{0}%' OR CommodityName LIKE '%{0}%' ", key));
            }

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt.Rows.Count > 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("error");
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