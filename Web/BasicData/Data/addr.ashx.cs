using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;

namespace Web.BasicData.Data
{
    /// <summary>
    /// addr 的摘要说明
    /// </summary>
    public class addr : IHttpHandler, IRequiresSessionState
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
                    case "Get_Address_Zu": Get_Address_Zu(context); break;

                    case "GetByID_Address_Xian": GetByID_Address_Xian(context); break;
                    case "Add_Address_Xian": Add_Address_Xian(context); break;
                    case "Update_Address_Xian": Update_Address_Xian(context); break;
                    case "DeleteByID_Address_Xian": DeleteByID_Address_Xian(context); break;

                    case "GetByID_Address_Xiang": GetByID_Address_Xiang(context); break;
                    case "Add_Address_Xiang": Add_Address_Xiang(context); break;
                    case "Update_Address_Xiang": Update_Address_Xiang(context); break;
                    case "DeleteByID_Address_Xiang": DeleteByID_Address_Xiang(context); break;

                    case "GetByID_Address_Cun": GetByID_Address_Cun(context); break;
                    case "Add_Address_Cun": Add_Address_Cun(context); break;
                    case "Update_Address_Cun": Update_Address_Cun(context); break;
                    case "DeleteByID_Address_Cun": DeleteByID_Address_Cun(context); break;

                    case "GetByID_Address_Zu": GetByID_Address_Zu(context); break;
                    case "Add_Address_Zu": Add_Address_Zu(context); break;
                    case "Update_Address_Zu": Update_Address_Zu(context); break;
                    case "DeleteByID_Address_Zu": DeleteByID_Address_Zu(context); break;

                    case "GetByID_Unit": GetByID_Unit(context); break;
                    case "Add_Unit": Add_Unit(context); break;
                    case "Update_Unit": Update_Unit(context); break;
                    case "DeleteByID_Unit": DeleteByID_Unit(context); break;

                    case "GetByID_Spec": GetByID_Spec(context); break;
                    case "Add_Spec": Add_Spec(context); break;
                    case "Update_Spec": Update_Spec(context); break;
                    case "DeleteByID_Spec": DeleteByID_Spec(context); break;

                    case "GetByID_WBGoodCategory": GetByID_WBGoodCategory(context); break;
                    case "Add_WBGoodCategory": Add_WBGoodCategory(context); break;
                    case "Update_WBGoodCategory": Update_WBGoodCategory(context); break;
                    case "DeleteByID_WBGoodCategory": DeleteByID_WBGoodCategory(context); break;
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
            strSql.Append(" SELECT ID,strName FROM dbo.BD_Address_Cun ");
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

        void Get_Address_Zu(HttpContext context)
        {
            string CunID = context.Request.QueryString["CunID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName FROM dbo.BD_Address_Zu ");
            strSql.Append(" where CunID=" + CunID);
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


        void GetByID_Address_Xiang(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,XianID,WBID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [BD_Address_Xiang] ");
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

        void Add_Address_Xiang(HttpContext context)
        {
            string ID = context.Session["ID"].ToString();//操作人编号
            string WBID = SQLHelper.ExecuteScalar("SELECT WB_ID  FROM dbo.Users  WHERE ID=" + ID).ToString();

            string XianID = context.Request.Form["XianID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string strCount = "   SELECT COUNT(ID) FROM dbo.BD_Address_Xiang WHERE XianID=" + XianID + " AND strName='" + strName + "'";
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [BD_Address_Xiang] (");
            strSql.Append("XianID,WBID,strName,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@XianID,@WBID,@strName,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@XianID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = XianID;
            parameters[1].Value = WBID;
            parameters[2].Value = strName;
            parameters[3].Value = false;
            parameters[4].Value = 1;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Address_Xiang(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string UserID = context.Session["ID"].ToString();//操作人编号
            string WBID = SQLHelper.ExecuteScalar("SELECT WB_ID  FROM dbo.Users  WHERE ID=" + UserID).ToString();

            string XianID = context.Request.Form["XianID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string strCount = "   SELECT COUNT(ID) FROM dbo.BD_Address_Xiang WHERE XianID=" + XianID + " AND strName='" + strName + "' and ID !="+ID;
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [BD_Address_Xiang] set ");
            strSql.Append("XianID=@XianID,");
            strSql.Append("WBID=@WBID,");
            strSql.Append("strName=@strName,");
            strSql.Append("ISDefault=@ISDefault,");
            strSql.Append("numSort=@numSort");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@XianID", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = XianID;
            parameters[1].Value = WBID;
            parameters[2].Value = strName;
            parameters[3].Value = 0;
            parameters[4].Value = 1;
            parameters[5].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_Address_Xiang(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Depositor WHERE XiangID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Commune WHERE XiangID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.SA_Account WHERE XiangID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [BD_Address_Xiang] ");
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


        void GetByID_Address_Xian(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [BD_Address_Xian] ");
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

        void Add_Address_Xian(HttpContext context)
        {
            
            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Add("BD_Address_Xian", "strName", strName)) {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [BD_Address_Xian] (");
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

        void Update_Address_Xian(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
        
            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("BD_Address_Xian", "strName", strName,ID))
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [BD_Address_Xian] set ");
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

        void DeleteByID_Address_Xian(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Depositor WHERE XianID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Commune WHERE XianID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.SA_Account WHERE XianID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [BD_Address_Xian] ");
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


        void GetByID_Address_Cun(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT C.ID AS XianID,B.ID AS XiangID,A.ID,A.strName");
            strSql.Append(" FROM dbo.BD_Address_Cun A INNER JOIN dbo.BD_Address_Xiang B ON A.XiangID=B.ID");
            strSql.Append("   INNER JOIN dbo.BD_Address_Xian C ON B.XianID=C.ID");
            strSql.Append(" where A.ID=@ID ");
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

        void Add_Address_Cun(HttpContext context)
        {
            string ID = context.Session["ID"].ToString();//操作人编号

            string XiangID = context.Request.QueryString["XiangID"].ToString();
            string strName = context.Request.QueryString["strName"].ToString();
            string WBID = context.Session["WB_ID"].ToString();


            string strCount = "   SELECT COUNT(ID) FROM dbo.BD_Address_Cun WHERE XiangID="+XiangID+" AND strName='"+strName+"'";
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [BD_Address_Cun] (");
            strSql.Append("XiangID,strName,ISDefault,numSort,WBID)");
            strSql.Append(" values (");
            strSql.Append("@XiangID,@strName,@ISDefault,@numSort,@WBID)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@XiangID", SqlDbType.Int,4),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4),
					new SqlParameter("@WBID", SqlDbType.Int,4)};
            parameters[0].Value = XiangID;
            parameters[1].Value = strName;
            parameters[2].Value = 0;
            parameters[3].Value = 1;
            parameters[4].Value = WBID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
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
            string XiangID = context.Request.QueryString["XiangID"].ToString();
            string strCount = "   SELECT COUNT(ID) FROM dbo.BD_Address_Cun WHERE XiangID=" + XiangID + " AND strName='" + strName + "' and ID !="+ID;
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [BD_Address_Cun] set strName='"+strName+"' where ID= "+ID);
          

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

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Depositor WHERE CunID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Commune WHERE CunID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.SA_Account WHERE CunID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

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

        
        void GetByID_Address_Zu(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT D.ID AS XianID,C.ID AS XiangID,B.ID AS CunID,A.strName");

            strSql.Append(" FROM dbo.BD_Address_Zu A INNER JOIN dbo.BD_Address_Cun B ON A.CunID=B.ID");
            strSql.Append("   INNER JOIN dbo.BD_Address_Xiang C ON B.XiangID=C.ID");
            strSql.Append("   INNER JOIN dbo.BD_Address_Xian D ON C.XianID=D.ID");
            strSql.Append(" where A.ID=@ID ");
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

        void Add_Address_Zu(HttpContext context)
        {
    
            string CunID = context.Request.Form["CunID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
        

            string strCount = "   SELECT COUNT(ID) FROM dbo.BD_Address_Zu WHERE CunID=" + CunID + " AND strName='" + strName + "'";
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [BD_Address_Zu] (");
            strSql.Append("CunID,strName,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@CunID,@strName,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@CunID", SqlDbType.Int,4),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = CunID;
            parameters[1].Value = strName;
            parameters[2].Value = 0;
            parameters[3].Value = 1;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_Address_Zu(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string CunID = context.Request.QueryString["CunID"].ToString();
            string strCount = "   SELECT COUNT(ID) FROM dbo.BD_Address_Zu WHERE CunID=" + CunID + " AND strName='" + strName + "' and ID !="+ID;
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [BD_Address_Zu] set strName='" + strName + "' where ID= " + ID);


            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_Address_Zu(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
        
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Commune WHERE ZuID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [BD_Address_Zu] ");
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


        void GetByID_Unit(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [BD_MeasuringUnit] ");
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

        void Add_Unit(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Add("BD_MeasuringUnit", "strName", strName))
            {
                context.Response.Write("Exit");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [BD_MeasuringUnit] (");
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

        void Update_Unit(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("BD_MeasuringUnit", "strName", strName, ID))
            {
                context.Response.Write("Exit");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [BD_MeasuringUnit] set ");
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

        void DeleteByID_Unit(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageVariety WHERE MeasuringUnitID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Good WHERE MeasuringUnit=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodSupply WHERE UnitID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [BD_MeasuringUnit] ");
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


        void GetByID_Spec(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,ISDefault,numSort ");
            strSql.Append(" FROM [BD_PackingSpec] ");
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

        void Add_Spec(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Add("BD_PackingSpec", "strName", strName))
            {
                context.Response.Write("Exit");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [BD_PackingSpec] (");
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

        void Update_Spec(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("BD_PackingSpec", "strName", strName, ID))
            {
                context.Response.Write("Exit");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [BD_PackingSpec] set ");
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

        void DeleteByID_Spec(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用

            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Good WHERE PackingSpecID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodSupply WHERE SpecID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [BD_PackingSpec] ");
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


        void GetByID_WBGoodCategory(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,strName,strCode,ISCustom,ISDefault,numSort ");
            strSql.Append(" FROM [WBGoodCategory] ");
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

        void Add_WBGoodCategory(HttpContext context)
        {

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Add("WBGoodCategory", "strName", strName)) {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [WBGoodCategory] (");
            strSql.Append("strName,strCode,ISCustom,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@strName,@strCode,@ISDefault,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
                    new SqlParameter("@strCode", SqlDbType.NVarChar,50),
                    new SqlParameter("@ISCustom", SqlDbType.Bit,1),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = "";
            parameters[2].Value = 0;
            parameters[3].Value = false;
            parameters[4].Value = 1;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_WBGoodCategory(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            if (!common.UniqueCheck_Update("WBGoodCategory", "strName", strName,ID))
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [WBGoodCategory] set ");
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

        void DeleteByID_WBGoodCategory(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();
            //查询该商品分类是否已经被使用
         
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Good WHERE CategoryID="+wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return ;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodSupply WHERE CategoryID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT ISCustom FROM dbo.WBGoodCategory WHERE ID=" + wbid).ToString() == "1")
            {
                context.Response.Write("ISCustom");
                return;
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [WBGoodCategory] ");
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