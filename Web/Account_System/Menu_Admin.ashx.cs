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
    /// Menu_Admin1 的摘要说明
    /// </summary>
    public class Menu_Admin1 : IHttpHandler
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
                    case "ClearDB": ClearDB(context); break;
                    case "SaveCompanyInfo": SaveCompanyInfo(context); break;
                    case "GetCompanyInfo": GetCompanyInfo(context); break; 
                        
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
                string strSql = " select count(ID) from Menu_Admin where PID=@PID";
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

            string strSql = " SELECT ID,PID,strValue,strUrl,numSort,ISEnable,ISSysW FROM Menu_Admin where PID=@PID  order by numSort";
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
            foreach(var key in context.Request.Form.Keys)
            {
                 string strValue=context.Request.Form[key.ToString()].ToString();
                 if (key.ToString().IndexOf("selectEnable_") == 0)//是否禁用
                 {
                     menuID = key.ToString().Substring(13);
                     if (strValue == "启用")
                     {
                         strSql.Append(" UPDATE Menu_Admin SET ISEnable=1 WHERE ID=" + menuID + "; ");
                     }
                     else {
                         strSql.Append(" UPDATE Menu_Admin SET ISEnable=0 WHERE ID=" + menuID + "; ");
                     }
                 }
                 if (key.ToString().IndexOf("selectAuthority_") == 0)//是否禁用
                 {
                     menuID = key.ToString().Substring(16);
                     if (strValue == "是")
                     {
                         strSql.Append(" UPDATE Menu_Admin SET ISSysW=1 WHERE ID=" + menuID + "; ");
                     }
                     else
                     {
                         strSql.Append(" UPDATE Menu_Admin SET ISSysW=0 WHERE ID=" + menuID + "; ");
                     }
                 }
                 if (key.ToString().IndexOf("txtSort_") == 0)//排序
                 {
                     menuID = key.ToString().Substring(8);
                     strSql.Append(" UPDATE Menu_Admin SET numSort="+strValue+" WHERE ID=" + menuID + "; ");
                 }
                 if (key.ToString().IndexOf("txtUrl_") == 0)//网址
                 {
                     menuID = key.ToString().Substring(7);
                     strSql.Append(" UPDATE Menu_Admin SET strUrl='" + strValue + "' WHERE ID=" + menuID + "; ");
                 }
                 if (key.ToString().IndexOf("txtValue_") == 0)//名称
                 {
                     menuID = key.ToString().Substring(9);
                     strSql.Append(" UPDATE Menu_Admin SET strValue='" + strValue + "' WHERE ID=" + menuID + "; ");
                 }
            }
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else {
                context.Response.Write("Error");
            }
        }


        void AddMenu(HttpContext context)
        {
            string PID=context.Request.QueryString["PID"].ToString();
            var stream = context.Request.InputStream;
            string json = new StreamReader(stream).ReadToEnd();
            json = Fun.CutString_Head(json);
            string[] JsonArray = json.Split(',');
            string strValue="";
            string strUrl="";
            int numSort=100;
            
            for (int i = 0; i < JsonArray.Length; i++) {
                string[] JsonArray_Child = JsonArray[i].Split(':');
                string key =Fun.CutString_Head( JsonArray_Child[0]);
                string value = Fun.CutString_Head(JsonArray_Child[1]);
                switch (key)
                {
                    case "strValue": strValue = value; break;
                    case "strUrl": strUrl = value; break;
                    case "numSort": numSort =Convert.ToInt32( value); break;
                }
            }

            string strSql = " INSERT INTO dbo.Menu_Admin"
      +"  ( PID ,  strValue , strUrl , strDescript , numSort , ISEnable,ISSysW )"
      +" VALUES  ( "+PID+" , N'"+strValue+"' , N'"+strUrl+"' ,N'' ,"+numSort+" ,1,1  )";
            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
                context.Response.Write("OK");
            }
            else {
                context.Response.Write("Error");
            }
        }

        void DeleteMenu(HttpContext context)
        {
            string mID = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" DELETE FROM dbo.Menu_Admin WHERE PID="+mID);
            strSql.Append(" DELETE FROM dbo.Menu_Admin WHERE ID=" + mID);
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }

        }

        void SaveCompanyInfo(HttpContext context)
        {
            string strName = context.Request.QueryString["strName"].ToString();
            string pushmsgApiurl = context.Request.QueryString["pushmsgApiurl"].ToString();
            string webSiteCode = context.Request.QueryString["webSiteCode"].ToString();
          
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("UPDATE dbo.BD_Company SET strName='{0}',pushmsgApiurl='{1}',webSiteCode='{2}'",strName,pushmsgApiurl,webSiteCode));
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }

        }

        void GetCompanyInfo(HttpContext context)
        {
            string sql = " select * from BD_Company";
            DataTable dt = SQLHelper.ExecuteDataTable(sql);
            if (dt==null||dt.Rows.Count==0)
            {
                context.Response.Write("Error");
            }
            else
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }

        }


        void ClearDB(HttpContext context)
        {
            string strPassword = context.Request.QueryString["strPassword"].ToString();
            if (common.GetCompanyInfo()["strPassword"].ToString() != strPassword)
            {
                context.Response.Write("ErrorPassword");
                return;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  DELETE  FROM   dbo.PrintSetting WHERE WBID not in(0,2) ");
            strSql.Append("  DELETE  FROM  dbo.PrintSetting_Dep  WHERE WBID not in(0,2)");
            strSql.Append("  DELETE  FROM   dbo.WB WHERE ID!=2 ");
            strSql.Append("  DELETE  FROM   dbo.Users WHERE WB_ID!=2");
            strSql.Append("  DELETE  FROM   dbo.UserMenu");

            strSql.Append("  DELETE  FROM   dbo.WBSupplier");
            strSql.Append("  DELETE  FROM   dbo.WBWareHouse");
            strSql.Append("  DELETE  FROM   WBGoodCategory");

            strSql.Append("  DELETE  FROM dbo.BD_Address_Cun");
            strSql.Append("  DELETE  FROM dbo.BD_Address_Xian");
            strSql.Append("  DELETE  FROM dbo.BD_Address_Xiang");
            strSql.Append("  DELETE  FROM dbo.BD_Address_Zu");
            strSql.Append("  DELETE  FROM  dbo.BD_MeasuringUnit");
            strSql.Append("  DELETE  FROM dbo.BD_PackingSpec");

            strSql.Append("  DELETE  FROM dbo.C_ChangeCard");
            strSql.Append("  DELETE  FROM dbo.C_OperateLog");
            strSql.Append("  DELETE  FROM dbo.C_PreDefine");
            strSql.Append("  DELETE  FROM dbo.C_PreDefineConsume");
            strSql.Append("  DELETE  FROM   dbo.C_Supply");
            strSql.Append("  DELETE  FROM dbo.Commune");

            strSql.Append("  DELETE  FROM dbo.Dep_ChangeCard");
            strSql.Append("  DELETE  FROM dbo.Dep_OperateLog");
            strSql.Append("  DELETE  FROM dbo.Dep_StorageInfo");
            strSql.Append("  DELETE  FROM  dbo.Dep_StorageLog");
            strSql.Append("  DELETE  FROM dbo.Depositor");

            strSql.Append("  DELETE  FROM  dbo.Good ");
            strSql.Append("  DELETE  FROM  dbo.GoodExchange");
            strSql.Append("  DELETE  FROM  dbo.GoodExchangeProp");
            strSql.Append("  DELETE  FROM   dbo.GoodStock");
            strSql.Append("  DELETE  FROM   dbo.GoodStorage");
            strSql.Append("  DELETE  FROM  dbo.GoodSupplyApply");
            strSql.Append("  DELETE  FROM  dbo.GoodSupply");
            strSql.Append("  DELETE  FROM  dbo.GoodSupplyStock");
            strSql.Append("  DELETE  FROM   dbo.GoodSupplyStorage");

            strSql.Append("  DELETE  FROM  dbo.Info");
            strSql.Append("  DELETE  FROM  dbo.InfoNotice");
            strSql.Append("  DELETE  FROM   dbo.InfoReply");
            strSql.Append("  DELETE  FROM   dbo.InfoType");

            strSql.Append("  DELETE  FROM  dbo.StorageFee");
            strSql.Append("  DELETE  FROM   dbo.StorageInterest");
            strSql.Append("  DELETE  FROM    dbo.StorageRate");
            strSql.Append("  DELETE  FROM   dbo.StorageSell");
            strSql.Append("  DELETE  FROM  dbo.StorageSellApply");
            strSql.Append("  DELETE  FROM  dbo.StorageTime");
            strSql.Append("  DELETE  FROM   dbo.StorageType");
            strSql.Append("  DELETE  FROM   dbo.StorageVariety");
            strSql.Append("  DELETE FROM dbo.StorageShopping");
            strSql.Append("  DELETE  FROM   dbo.StorageVarietyLevel_L");

            strSql.Append("  DELETE FROM     dbo.SA_Account");
            strSql.Append("  DELETE  FROM    dbo.SABD_Accountant");
            strSql.Append("  DELETE  FROM    dbo.SA_AgentFee");
            strSql.Append("  DELETE  FROM    dbo.SA_CheckOut");
            strSql.Append("  DELETE  FROM    dbo.SA_CheckOutLog");
            strSql.Append("  DELETE  FROM    dbo.SA_Exchange");
            strSql.Append("  DELETE  FROM    dbo.SA_Sell");
            strSql.Append("  DELETE  FROM    dbo.SA_VarietyStorage");
            strSql.Append("  DELETE  FROM    dbo.SABD_Accountant");
            strSql.Append("  DELETE  FROM    dbo.SABD_Quality");
            strSql.Append("  DELETE  FROM    dbo.SABD_StockType");
            strSql.Append("  DELETE  FROM     dbo.SABD_Weigh");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  ");

            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString());//添加兑换交易记录
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}