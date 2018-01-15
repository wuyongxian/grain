using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;

namespace Web.Account_System
{
    /// <summary>
    /// Menu_User1 的摘要说明
    /// </summary>
    public class Menu_User1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "GetMenu": getMenu(context); break;
                    case "frmsubmit": frmsubmit(context); break;
                    case "AddMenu": AddMenu(context); break;
                    case "DeleteMenu": DeleteMenu(context); break;
                }
            }

        }

        void getMenu(HttpContext context)
        {


            string PID = context.Request.QueryString["PID"].ToString();
            DataTable dt = GetMenu(Convert.ToInt32(PID));
            dt.Columns.Add("HasChild", typeof(int));//添加标识列，标识该菜单是否有子菜单
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strSql = " select count(ID) from Menu_User where PID=@PID";
                SqlParameter para = new SqlParameter("@PID", dt.Rows[i]["ID"].ToString());
                string strCount = SQLHelper.ExecuteScalar(strSql, para).ToString();
                if (strCount != "0")
                {
                    dt.Rows[i]["HasChild"] = 1;
                }
                else
                {
                    dt.Rows[i]["HasChild"] = 0;
                }
            }

            context.Response.Write(JsonHelper.ToJson(dt));
        }

        DataTable GetMenu(int PID)
        {
            // string strSql = " SELECT ID,PID,strName,strUrl,Seq FROM Menu where PID="+PID;

            string strSql = " SELECT ID,PID,strValue,strUrl,numSort,ISEnable FROM Menu_User where PID=@PID  order by numSort";
            SqlParameter para = new SqlParameter("@PID", PID);

            return SQLHelper.ExecuteDataTable(strSql, para);
        }


        void frmsubmit(HttpContext context)
        {
            if (context.Request.Form.Count <= 0)
            {
                context.Response.Write("OK");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            string menuID = "0";//菜单编号 
            foreach (var key in context.Request.Form.Keys)
            {
                string strValue = context.Request.Form[key.ToString()].ToString();
                if (key.ToString().IndexOf("select_") == 0)//是否禁用
                {
                    menuID = key.ToString().Substring(7);
                    if (strValue == "启用")
                    {
                        strSql.Append(" UPDATE Menu_User SET ISEnable=1 WHERE ID=" + menuID + "; ");
                    }
                    else
                    {
                        strSql.Append(" UPDATE Menu_User SET ISEnable=0 WHERE ID=" + menuID + "; ");
                    }
                }
                if (key.ToString().IndexOf("txtSort_") == 0)//排序
                {
                    menuID = key.ToString().Substring(8);
                    strSql.Append(" UPDATE Menu_User SET numSort=" + strValue + " WHERE ID=" + menuID + "; ");
                }
                if (key.ToString().IndexOf("txtUrl_") == 0)//网址
                {
                    menuID = key.ToString().Substring(7);
                    strSql.Append(" UPDATE Menu_User SET strUrl='" + strValue + "' WHERE ID=" + menuID + "; ");
                }
                if (key.ToString().IndexOf("txtValue_") == 0)//名称
                {
                    menuID = key.ToString().Substring(9);
                    strSql.Append(" UPDATE Menu_User SET strValue='" + strValue + "' WHERE ID=" + menuID + "; ");
                }
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


        void AddMenu(HttpContext context)
        {
            string PID = context.Request.QueryString["PID"].ToString();
            var stream = context.Request.InputStream;
            string json = new StreamReader(stream).ReadToEnd();
            json = Fun.CutString_Head(json);
            string[] JsonArray = json.Split(',');
            string strValue = "";
            string strUrl = "";
            int numSort = 100;

            for (int i = 0; i < JsonArray.Length; i++)
            {
                string[] JsonArray_Child = JsonArray[i].Split(':');
                string key = Fun.CutString_Head(JsonArray_Child[0]);
                string value = Fun.CutString_Head(JsonArray_Child[1]);
                switch (key)
                {
                    case "strValue": strValue = value; break;
                    case "strUrl": strUrl = value; break;
                    case "numSort": numSort = Convert.ToInt32(value); break;
                }
            }

            string strSql = " INSERT INTO dbo.Menu_User"
      + "  ( PID ,  strValue , strUrl , strDescript , numSort , ISEnable )"
      + " VALUES  ( " + PID + " , N'" + strValue + "' , N'" + strUrl + "' ,N'' ," + numSort + " ,1 )";
            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteMenu(HttpContext context)
        {
            string mID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" DELETE FROM dbo.Menu_User WHERE PID=" + mID);
            strSql.Append(" DELETE FROM dbo.Menu_User WHERE ID=" + mID);
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