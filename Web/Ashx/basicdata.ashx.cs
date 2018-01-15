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
    /// basicdata 的摘要说明
    /// </summary>
    public class basicdata : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //common.IsLogin();
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "GetStorageVariety": GetStorageVariety(context); break;
                    case "GetDepNameByID": GetDepNameByID(context); break;
                    case "GetStorageTimeNameByID": GetStorageTimeNameByID(context); break;
                    case "GetStorageVarietyNameByID": GetStorageVarietyNameByID(context); break;
                    case "GetNameByID": GetNameByID(context); break;//查找表的strName

                }
            }

        }

        void GetStorageVariety(HttpContext context)
        {
          
            DataTable dt = SQLHelper.ExecuteDataTable(" SELECT * FROM dbo.StorageVariety ORDER BY numSort ");

            if (dt != null && dt.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("");
            }
        }

        #region 储户信息
        void GetDepNameByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            object obj = SQLHelper.ExecuteScalar(" select strName from Depositor where ID="+ID);

            if (obj != null && obj.ToString() != "")
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("");
            }
        }
        #endregion

        #region 存储参数
        void GetStorageTimeNameByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            object obj = SQLHelper.ExecuteScalar(" select strName from StorageTime where ID=" + ID);

            if (obj != null && obj.ToString() != "")
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("");
            }
        }

        void GetStorageVarietyNameByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            object obj = SQLHelper.ExecuteScalar(" select strName from StorageVariety where ID=" + ID);

            if (obj != null && obj.ToString() != "")
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("");
            }
        }

        /// <summary>
        /// 通用方法（通过表的ID找到表的strName）
        /// </summary>
        /// <param name="context"></param>
        void GetNameByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string tableName = context.Request.QueryString["tableName"].ToString();
            object obj = SQLHelper.ExecuteScalar(" select strName from "+tableName+" where ID=" + ID);

            if (obj != null && obj.ToString() != "")
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("");
            }
        }
        #endregion


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}