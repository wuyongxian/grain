using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.SessionState;

namespace Web.BasicData.HQStorage
{
    /// <summary>
    /// storage 的摘要说明
    /// </summary>
    public class storage : IHttpHandler, IRequiresSessionState
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
                    case "GetHQWareHouseList": GetHQWareHouseList(context); break;
                    case "GetWBWareHouseList": GetWBWareHouseList(context); break;
                    case "GetByID_storage": GetByID_storage(context); break;
                    case "Add_storage": Add_storage(context); break;
                    case "Update_storage": Update_storage(context); break;
                    case "DeleteByID_storage": DeleteByID_storage(context); break;
                }
            }

        }

        /// <summary>
        /// 总部仓库
        /// </summary>
        /// <param name="context"></param>
        void GetHQWareHouseList(HttpContext context)
        {

            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID, WB_ID,strName,Address,Capacity,ISDefault,numSort FROM dbo.WBWareHouse");
            strSql.Append("   WHERE WB_ID=(SELECT ID FROM dbo.WB WHERE ISHQ=1)");
            strSql.Append("   order by ISDefault desc,numSort desc");
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
        /// 网点仓库
        /// </summary>
        /// <param name="context"></param>
        void GetWBWareHouseList(HttpContext context)
        {
           
            string WBID = context.Session["WB_ID"].ToString();

            if (context.Request.Form["WBID"] != null) {
                WBID = context.Request.Form["WBID"].ToString();
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID, WB_ID,strName,Address,Capacity,ISDefault,numSort FROM dbo.WBWareHouse");
            strSql.Append("   where WB_ID="+WBID);
            strSql.Append("   order by ISDefault desc,numSort desc");
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

        void GetByID_storage(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT WB_ID,strName,Address,Capacity,ISDefault,numSort");
            strSql.Append(" FROM dbo.WBWareHouse WHERE ID="+ID);
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

        void Add_storage(HttpContext context)
        {
         
            string WB_ID = context.Session["WB_ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string Address = context.Request.Form["Address"].ToString();
            string Capacity = context.Request.Form["Capacity"].ToString();
            bool ISDefault = false;
            if (context.Request.Form["ISDefault"] != null) {
                ISDefault = true;
            }
           

            string strCount = "    SELECT COUNT(ID)  FROM dbo.WBWareHouse WHERE WB_ID="+WB_ID+"  AND strName='" + strName + "'";
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            if (ISDefault)
            {
                //修改其他的仓库默认
                SQLHelper.ExecuteNonQuery(" update WBWareHouse set ISDefault=0 where WB_ID=" + WB_ID);
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [WBWareHouse] (");
            strSql.Append("WB_ID,strName,Address,Capacity,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@WB_ID,@strName,@Address,@Capacity,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WB_ID", SqlDbType.NVarChar,10),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@Address", SqlDbType.NVarChar,100),
					new SqlParameter("@Capacity", SqlDbType.BigInt,8),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = WB_ID;
            parameters[1].Value = strName;
            parameters[2].Value = Address;
            parameters[3].Value = Capacity;
            parameters[4].Value = ISDefault;
            parameters[5].Value = 1;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void Update_storage(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string WB_ID = context.Session["WB_ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            string Address = context.Request.Form["Address"].ToString();
            string Capacity = context.Request.Form["Capacity"].ToString();

            string strCount = "    SELECT COUNT(ID)  FROM dbo.WBWareHouse WHERE WB_ID=" + WB_ID + "  AND strName='" + strName + "' and ID !="+ID;
            if (SQLHelper.ExecuteScalar(strCount).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            bool ISDefault = false;
            if (context.Request.Form["Capacity"] != null)
            {
                ISDefault = true;
            }
            if (ISDefault)
            {
                //修改其他的仓库默认
                SQLHelper.ExecuteNonQuery(" update WBWareHouse set ISDefault=0 where WB_ID=" + WB_ID);
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [WBWareHouse] set ");
            strSql.Append("strName=@strName,");
            strSql.Append("Address=@Address,");
            strSql.Append("Capacity=@Capacity,");
            strSql.Append("ISDefault=@ISDefault");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@Address", SqlDbType.NVarChar,100),
					new SqlParameter("@Capacity", SqlDbType.BigInt,8),
                    new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = Address;
            parameters[2].Value = Capacity;
            parameters[3].Value = ISDefault;
            parameters[4].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(),parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteByID_storage(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodStorage WHERE WBWareHouseID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.GoodSupplyStorage WHERE WBWareHouseID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [WBWareHouse] ");
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