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
    /// user 的摘要说明
    /// </summary>
    public class user : IHttpHandler, IRequiresSessionState
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
                    case "GetWB": GetWB(context); break;
                    case "GetUserGroup": GetUserGroup(context); break;
                    
                        
                    //case "AddUser": AddUser(context); break;
                    //case "UpdateUser": UpdateUser(context); break;
                   // case "UpdateUserPart": UpdateUserPart(context); break;
                  //  case "GetUserBySessionID": GetUserBySessionID(context); break;
                    //case "GetUserByID": GetUserByID(context); break;
                   
                    case "DeleteUserByID": DeleteUserByID(context); break;
                    case "UpdateUserOperate": UpdateUserOperate(context); break;//更新用户操作记录

                        
                }
            }

        }

        void GetWB(HttpContext context)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(" SELECT ID,strName FROM WB");
            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void GetUserGroup(HttpContext context)
        {
            string WBID = context.Request.QueryString["WBID"].ToString();
            string strSql;
            object obj = SQLHelper.ExecuteScalar(" SELECT ISHQ  FROM dbo.WB  WHERE ID="+WBID);
            if (Convert.ToBoolean(obj) == true || obj.ToString() == "1")
            {
                strSql = " SELECT ID,strName FROM dbo.UserGroup WHERE strName!='系统管理员'";
            }
            else {
                strSql = " SELECT ID,strName FROM dbo.UserGroup WHERE strName='营业员'";
             
            }
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

       

        //void AddUser(HttpContext context)
        //{
        //    string wbid = context.Request.QueryString["ID"].ToString();

        //    string SerialNumber = "000000";

        //    SerialNumber = SQLHelper.ExecuteScalar("SELECT TOP 1 SerialNumber FROM dbo.Users ORDER BY dt_Add DESC").ToString();
        //    int numSerial = Convert.ToInt32(SerialNumber) + 1;
        //    if (numSerial > 0 && numSerial < 10)
        //    {
        //        SerialNumber = "0000" + numSerial.ToString(); ;
        //    }
        //    else if (numSerial > 10 && numSerial < 100)
        //    {
        //        SerialNumber = "000" + numSerial.ToString(); ;
        //    }
        //    else if (numSerial >= 100 && numSerial < 1000)
        //    {
        //        SerialNumber = "00" + numSerial.ToString(); ;
        //    }
        //    else if (numSerial >= 1000 && numSerial < 10000)
        //    {
        //        SerialNumber = "0" + numSerial.ToString(); ;
        //    }
        //    else { SerialNumber = numSerial.ToString(); }

        //    string UserGroup_ID = context.Request.Form["UserGroup_ID"].ToString();
        //    string WB_ID = context.Request.Form["WB_ID"].ToString();
        //    string strRealName = context.Request.Form["strRealName"].ToString();
        //    string strLoginName = context.Request.Form["strLoginName"].ToString();


        //    if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Users WHERE strLoginName='" + strLoginName + "'").ToString() != "0")
        //    {
        //        context.Response.Write("1");
        //        return;
        //    }
        //    string strPassword = context.Request.Form["strPassword"].ToString();
        //    strPassword = Fun.GetMD5_32(strPassword);// 获取md5加密后的用户密码
        //    string strPhone = context.Request.Form["strPhone"].ToString();
        //    string strAddress = context.Request.Form["strAddress"].ToString();
        //    string numLimitAmount = context.Request.Form["numLimitAmount"].ToString();
        //    bool ISEnable = true;
        //    if (context.Request.Form["ISEnable"].ToString() == "0") {
        //        ISEnable = false;
        //    }
             
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("insert into [Users] (");
        //    strSql.Append("SerialNumber,UserGroup_ID,WB_ID,strRealName,strLoginName,strPassword,strPhone,strAddress,numLimitAmount,ISEnable,dt_Add,dt_Update)");
        //    strSql.Append(" values (");
        //    strSql.Append("@SerialNumber,@UserGroup_ID,@WB_ID,@strRealName,@strLoginName,@strPassword,@strPhone,@strAddress,@numLimitAmount,@ISEnable,@dt_Add,@dt_Update)");
        //    strSql.Append(";select @@IDENTITY");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
        //            new SqlParameter("@UserGroup_ID", SqlDbType.Int,4),
        //            new SqlParameter("@WB_ID", SqlDbType.Int,4),
        //            new SqlParameter("@strRealName", SqlDbType.NVarChar,50),
        //            new SqlParameter("@strLoginName", SqlDbType.NVarChar,50),
        //            new SqlParameter("@strPassword", SqlDbType.NVarChar,50),
        //            new SqlParameter("@strPhone", SqlDbType.NVarChar,50),
        //            new SqlParameter("@strAddress", SqlDbType.NVarChar,50),
        //            new SqlParameter("@numLimitAmount", SqlDbType.Int,4),
        //            new SqlParameter("@ISEnable", SqlDbType.Bit,1),
        //            new SqlParameter("@dt_Add", SqlDbType.DateTime),
        //            new SqlParameter("@dt_Update", SqlDbType.DateTime)};
        //    parameters[0].Value = SerialNumber;
        //    parameters[1].Value = UserGroup_ID;
        //    parameters[2].Value = WB_ID;
        //    parameters[3].Value = strRealName;
        //    parameters[4].Value = strLoginName;
        //    parameters[5].Value = strPassword;
        //    parameters[6].Value = strPhone;
        //    parameters[7].Value = strAddress;
        //    parameters[8].Value = numLimitAmount;
        //    parameters[9].Value = ISEnable;
        //    parameters[10].Value = DateTime.Now;
        //    parameters[11].Value = DateTime.Now;


        //    if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
        //    {
        //        context.Response.Write("OK");
        //    }
        //    else
        //    {
        //        context.Response.Write("Error");
        //    }
        //}


        //void UpdateUser(HttpContext context)
        //{
        //    string wbid = context.Request.QueryString["ID"].ToString();

        //     string UserGroup_ID = context.Request.Form["UserGroup_ID"].ToString();
        //    string WB_ID = context.Request.Form["WB_ID"].ToString();
        //    string strRealName = context.Request.Form["strRealName"].ToString();
        //    string strLoginName = context.Request.Form["strLoginName"].ToString();
        //    string strPassword = context.Request.Form["strPassword"].ToString();
        
        //    string strPhone = context.Request.Form["strPhone"].ToString();
        //    string strAddress = context.Request.Form["strAddress"].ToString();
        //    string numLimitAmount = context.Request.Form["numLimitAmount"].ToString();
        //    bool ISEnable = true;
        //    if (context.Request.Form["ISEnable"].ToString() == "0")
        //    {
        //        ISEnable = false;
        //    }
        //    StringBuilder strSql=new StringBuilder();
        //    strSql.Append("update [Users] set ");
        //    strSql.Append("UserGroup_ID=@UserGroup_ID,");
        //    strSql.Append("WB_ID=@WB_ID,");
        //    strSql.Append("strRealName=@strRealName,");
        //    strSql.Append("strLoginName=@strLoginName,");
        //    strSql.Append("strPhone=@strPhone,");
        //    strSql.Append("strAddress=@strAddress,");
        //    strSql.Append("numLimitAmount=@numLimitAmount,");
        //    strSql.Append("ISEnable=@ISEnable,");
        //    strSql.Append("dt_Update=@dt_Update");
        //    strSql.Append(" where ID=@ID ");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@UserGroup_ID", SqlDbType.Int,4),
        //            new SqlParameter("@WB_ID", SqlDbType.Int,4),
        //            new SqlParameter("@strRealName", SqlDbType.NVarChar,50),
        //            new SqlParameter("@strLoginName", SqlDbType.NVarChar,50),
        //            new SqlParameter("@strPhone", SqlDbType.NVarChar,50),
        //            new SqlParameter("@strAddress", SqlDbType.NVarChar,50),
        //            new SqlParameter("@numLimitAmount", SqlDbType.Int,4),
        //            new SqlParameter("@ISEnable", SqlDbType.Bit,1),
        //            new SqlParameter("@dt_Update", SqlDbType.DateTime),
        //            new SqlParameter("@ID", SqlDbType.BigInt,8)};
        //    parameters[0].Value = UserGroup_ID;
        //    parameters[1].Value = WB_ID;
        //    parameters[2].Value = strRealName;
        //    parameters[3].Value = strLoginName;
        //    parameters[4].Value = strPhone;
        //    parameters[5].Value = strAddress;
        //    parameters[6].Value = numLimitAmount;
        //    parameters[7].Value = ISEnable;
        //    parameters[8].Value = DateTime.Now;
        //    parameters[9].Value = wbid;

        //    if (strPassword.Trim() != "")//是否修改密码
        //    {
        //        strPassword = Fun.GetMD5_32(strPassword);// 获取md5加密后的用户密码
        //        string strSqlUpdate = " update [Users] set strPassword="+strPassword+" where ID="+wbid;
        //        SQLHelper.ExecuteNonQuery(strSqlUpdate);
        //    }


        //        if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
        //        {
        //            context.Response.Write("OK");
        //        }
        //        else
        //        {
        //            context.Response.Write("Error");
        //        }
        //}


       

        //void GetUserByID(HttpContext context)
        //{
        //    string wbid = context.Request.QueryString["ID"].ToString();

        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("select ID,SerialNumber,UserGroup_ID,WB_ID,strRealName,strLoginName,strPassword,strPhone,strAddress,numLimitAmount,ISEnable,dt_Add,dt_Update ");
        //    strSql.Append(" FROM [Users] ");
        //    strSql.Append(" where ID=@ID ");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@ID", SqlDbType.BigInt)};
        //    parameters[0].Value = wbid;
        //    DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(),parameters);
        //    if (dt != null && dt.Rows.Count != 0)
        //    {
        //        context.Response.Write(JsonHelper.ToJson(dt));
        //    }
        //    else
        //    {
        //        context.Response.Write("Error");
        //    }
        //}


   

        void DeleteUserByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string strSql = " delete FROM dbo.Users WHERE ID=" + wbid;


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
        /// 更新用户操作记录
        /// </summary>
        /// <param name="context"></param>
        void UpdateUserOperate(HttpContext context)
        {
            try
            {
                string UserID = context.Session["ID"].ToString();
                //获取该用户的最近一次登录信息记录
                StringBuilder strSqlOperate = new StringBuilder();
                strSqlOperate.Append("  SELECT TOP 1 ID,dt_LoginIn");
                strSqlOperate.Append("  FROM dbo.UserOperate ");
                strSqlOperate.Append("  WHERE UserID=" + UserID);
                strSqlOperate.Append("  ORDER BY dt_LoginIn DESC");
                strSqlOperate.Append("  ");

                DataTable dtOperate = SQLHelper.ExecuteDataTable(strSqlOperate.ToString());

                DateTime dt_LoginIn = Convert.ToDateTime(dtOperate.Rows[0]["dt_LoginIn"]);
                string ID = dtOperate.Rows[0]["ID"].ToString();

                DateTime dt_LoginOut = DateTime.Now;
                TimeSpan ts = dt_LoginOut.Subtract(dt_LoginIn);
                string TimeLength = "";
                TimeLength += ts.Hours.ToString() + "时" + ts.Minutes + "分" + ts.Seconds + "秒";

                StringBuilder strSql = new StringBuilder();
                strSql.Append(" update [UserOperate] set ");
                strSql.Append(" dt_LoginOut='" + dt_LoginOut + "',");
                strSql.Append(" TimeLength='" + TimeLength + "'");
                strSql.Append("  where ID=" + ID);
                if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
                {
                    context.Response.Write("OK");
                }
                else
                {
                    context.Response.Write("Error");
                }
            }
            catch {
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