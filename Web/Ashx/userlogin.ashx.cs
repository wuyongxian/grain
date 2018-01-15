using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Web.Ashx
{
    /// <summary>
    /// userlogin 的摘要说明
    /// </summary>
    public class userlogin : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "GetCompanyInfo": GetCompanyInfo(context); break;
                    case "GetLoginUserGroup": GetLoginUserGroup(context); break;
                    case "UserLogin": UserLogin(context); break;

                    case "UpdateUserOperate": UpdateUserOperate(context); break;
                }
            }
        }

        void GetCompanyInfo(HttpContext context)
        {

            DataTable dt = SQLHelper.ExecuteDataTable(" select * from BD_Company");
            if (dt != null && dt.Rows.Count != 0)
            {

                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetLoginUserGroup(HttpContext context)
        {
            string strSql = "SELECT ID,strName FROM UserGroup WHERE ISEnable=1 AND strName !='系统管理员' order by numSort";
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

        void UserLogin(HttpContext context)
        {
            var userType = context.Request.QueryString["user"];//index 普通用户账号;adminindex:单位管理员;sysadminindex:系统管理员
            var LoginName = context.Request.Form["LoginName"];
            var Password = context.Request.Form["Password"];
            if (LoginName.Trim() == "")
            {
                context.Response.Write("请输入登录名!");
                return;
            }

            if (Password.Trim() == "")
            {
                context.Response.Write("请输入密码!");
                return;
            }
            //检验账户正确性
            StringBuilder strAccount = new StringBuilder();
            strAccount.Append(" SELECT ID, WB_ID,UserGroup_ID,strLoginName,strRealName,strPassword,ISEnable from Users");
            strAccount.Append(" WHERE   LOWER(strLoginName)=@strLoginName ");
            SqlParameter[] para = new SqlParameter[]{
            new SqlParameter("@strLoginName",LoginName.Trim().ToLower())
            };
            DataTable dtAccount = SQLHelper.ExecuteDataTable(strAccount.ToString(), para);

            if (dtAccount != null && dtAccount.Rows.Count != 0)
            {
                int UserGroup_ID = Convert.ToInt32(dtAccount.Rows[0]["UserGroup_ID"]);
                string UserGroupName = SQLHelper.ExecuteScalar("  SELECT strName FROM dbo.UserGroup WHERE ID=" + UserGroup_ID).ToString();
                string UserGroupVerify = "OK";
                switch (userType)
                {
                    case "index": if (UserGroup_ID == 1 || UserGroup_ID == 2)
                        //case "index": if (UserGroup_ID == 1)
                        {
                            UserGroupVerify = "不存在的登陆账号!";
                        }
                        break;
                    case "adminindex": if (UserGroup_ID != 2&&UserGroup_ID!=3)
                        {
                            UserGroupVerify = "不存在的登陆账号!";
                        }
                        break;
                    case "sysadminindex": if (UserGroup_ID != 1)
                        {
                            UserGroupVerify = "不存在的登陆账号!";
                        }
                        break;
                }

                if (UserGroupVerify != "OK")
                {
                    context.Response.Write(UserGroupVerify);
                    return;
                }
                ////ip地址验证 
                //if (Convert.ToBoolean(common.GetWBAuthority()["ISWBControl"]) == true)//检验是否开启网点个数验证
                //{
                //    if (UserGroupName == "营业员" || UserGroupName == "网点管理员")
                //    {
                //        //验证当前用户所在网点的ip地址是否可用
                //        object objIPaddress = SQLHelper.ExecuteScalar(" SELECT top 1 IPaddress FROM dbo.WB WHERE ID=" + dtAccount.Rows[0]["WB_ID"].ToString());
                //        if (objIPaddress == null || objIPaddress.ToString().Trim() == "")
                //        {
                //            UserGroupVerify = "!";
                //            context.Response.Write("系统没有为当前账户的网点配置可用的IP地址，请与系统管理员联系取得可用的IP地址!");
                //            return;
                //        }
                //        string ip = context.Request.QueryString["ip"].ToString();
                //        if (objIPaddress.ToString().Trim() != ip)
                //        {
                //            UserGroupVerify = "";
                //            context.Response.Write("当前计算机使用的IP地址无效，请与系统管理员联系取得可用的IP地址!");
                //            return;
                //        }
                //    }
                //}


                bool ErrorLoginCheck = false;//是否启用了秘密错误检验 
                if (UserGroup_ID == 3 || UserGroup_ID == 4)
                {
                    if (Convert.ToBoolean(common.GetWBAuthority()["ErrorLogin_User"]) == true)
                    {
                        ErrorLoginCheck = true;
                    }
                }
                else
                {
                    if (Convert.ToBoolean(common.GetWBAuthority()["ErrorLogin_Admin"]) == true)
                    {
                        ErrorLoginCheck = true;
                    }
                }

                string UserID = dtAccount.Rows[0]["ID"].ToString();

                object objErrorTime = null;
                if (ErrorLoginCheck)
                {
                    objErrorTime = SQLHelper.ExecuteScalar(" SELECT TOP 1 ErrorTime FROM dbo.UserOperate WHERE UserID=" + UserID + " ORDER BY dt_LoginIn desc");
                    if (objErrorTime != null && objErrorTime.ToString() != "")
                    {
                        if (Convert.ToInt32(objErrorTime) >= 3)
                        {
                            DateTime dt_LoginIn = Convert.ToDateTime(SQLHelper.ExecuteScalar(" SELECT TOP 1 dt_LoginIn FROM dbo.UserOperate WHERE UserID=" + UserID + " ORDER BY dt_LoginIn desc"));
                            TimeSpan tsLogin = DateTime.Now.Subtract(dt_LoginIn);
                            if (tsLogin.TotalHours < 24)
                            {
                                Fun.Alert("您的密码已经连续三次输入错误，请于24小时后重试，或请求管理员解除限制！");
                                return;
                            }
                        }
                    }
                }
                string strPassword = dtAccount.Rows[0]["strPassword"].ToString();
                bool ISEnable = Convert.ToBoolean(dtAccount.Rows[0]["ISEnable"]);
                if (Fun.GetMD5_32(Password.Trim()) == strPassword)
                {
                    if (ISEnable)
                    {
                        if (HttpContext.Current.Session != null)
                        {
                            HttpContext.Current.Session.Clear();
                        }
                        DataTable dtwb = SQLHelper.ExecuteDataTable(" select * from WB where ID=" + dtAccount.Rows[0]["WB_ID"].ToString());
                        context.Session["WB_ID"] = dtAccount.Rows[0]["WB_ID"].ToString();//该用户所在的网点ID
                        context.Session["ISHQ"] = dtwb.Rows[0]["ISHQ"].ToString();//是否是总部网点
                        context.Session["ISSimulate"] = dtwb.Rows[0]["ISSimulate"].ToString();//是否是模拟网点
                        context.Session["UserGroup_ID"] = UserGroup_ID;
                        context.Session["UserGroup_Name"] = UserGroupName;
                        context.Session["ID"] = dtAccount.Rows[0]["ID"].ToString();//用户ID


                        context.Session["strLoginName"] = dtAccount.Rows[0]["strLoginName"].ToString();//用户登录名
                        context.Session["strRealName"] = dtAccount.Rows[0]["strRealName"].ToString();
                        //设置保存session的Cookies值
                        context.Request.Cookies.Clear();
                        context.Response.Cookies.Clear();

                        HttpCookie cookie = new HttpCookie("LoginInfo");
                        DateTime dtNow = DateTime.Now;
                        TimeSpan ts = new TimeSpan(1, 0, 0, 0);//设置cookie的保存时间为一天
                        cookie.Expires = dtNow.Add(ts);
                        cookie.Values.Add("ID", dtAccount.Rows[0]["ID"].ToString());
                        HttpContext.Current.Response.Cookies.Add(cookie);

                        //添加营业员访问记录
                        AddUserOperate(false, dtAccount,context);


                        string userinfo = JsonHelper.ToJson(dtAccount);
                        //userinfo = userinfo.Substring(1);
                        //userinfo = userinfo.Substring(0, userinfo.Length - 1);


                        string wbinfo = JsonHelper.ToJson(dtwb);
                        //wbinfo = wbinfo.Substring(1);
                        //wbinfo = wbinfo.Substring(0, userinfo.Length - 1);
                        var returnValue = "{\"wbinfo\":" + wbinfo + ",\"userinfo\":" + userinfo + "}";
                        //context.Response.Write("Success");
                        context.Response.Write(returnValue);
                        return;
                    }
                    else
                    {
                        context.Response.Write("当前账户已被禁用，请与管理员联系!");
                        return;
                    }
                }
                else
                {
                    AddUserOperate(true, dtAccount,context);

                    if (ErrorLoginCheck)
                    {
                        int numErrorTime = 0;
                        if (objErrorTime != null && objErrorTime.ToString() != "")
                        {
                            numErrorTime = Convert.ToInt32(objErrorTime) + 1;
                        }
                        context.Response.Write("这是您第" + numErrorTime + "次输入密码错误，连续输入错误3次以上您的账号将被禁用24小时！");
                        return;
                    }
                    else
                    {
                        context.Response.Write("您输入的密码不正确");
                        return;
                    }

                }
            }
            else
            {
                context.Response.Write("不存在的登陆账号!");
                return;
            }

        }


        private static void AddUserOperate(bool ISError, DataTable dt,HttpContext context)
        {
            string IpAddress = "192.168.1.1";
            string hostname = Dns.GetHostName();//得到主机名
            System.Net.IPAddress[] addressList = Dns.GetHostAddresses(hostname);

            foreach (IPAddress ip in addressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IpAddress = ip.ToString();//暂时获取内容的IPV4地址
                }
            }

            //string IpAddress = context.Request.ServerVariables.Get("Remote_Addr").ToString(); 
            //string hostname = context.Request.ServerVariables.Get("Remote_Host").ToString();


            object WBID = dt.Rows[0]["WB_ID"];
            object UserID = dt.Rows[0]["ID"];
            int ErrorTime = 0;
            if (ISError)
            {//这一次是错误的登陆 
                object objError = SQLHelper.ExecuteScalar(" SELECT TOP 1 ErrorTime FROM dbo.UserOperate where UserID=" + UserID + " ORDER BY dt_LoginIn desc");
                if (objError == null || objError.ToString() == "")
                {
                    ErrorTime = 1;
                }
                else
                {
                    ErrorTime = Convert.ToInt32(objError) + 1;
                }
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [UserOperate] (");
            strSql.Append("WBID,UserID,dt_LoginIn,dt_LoginOut,IpAddress,TimeLength,ErrorTime)");
            strSql.Append(" values (");
            strSql.Append("@WBID,@UserID,@dt_LoginIn,@dt_LoginOut,@IpAddress,@TimeLength,@ErrorTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WBID", SqlDbType.Int,4),
					new SqlParameter("@UserID", SqlDbType.Int,4),
					new SqlParameter("@dt_LoginIn", SqlDbType.DateTime),
					new SqlParameter("@dt_LoginOut", SqlDbType.DateTime),
					new SqlParameter("@IpAddress", SqlDbType.NVarChar,50),
					new SqlParameter("@TimeLength", SqlDbType.NVarChar,50),
                    new SqlParameter("@ErrorTime", SqlDbType.Int,4)};
            parameters[0].Value = WBID;
            parameters[1].Value = UserID;
            parameters[2].Value = DateTime.Now;
            parameters[3].Value = DateTime.Now;
            parameters[4].Value = IpAddress;
            parameters[5].Value = "0时0分0秒";
            parameters[6].Value = ErrorTime;
            SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters);

        }

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
                strSqlOperate.Append("  ORDER BY ID DESC");
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
                SQLHelper.ExecuteNonQuery(strSql.ToString());

                context.Session.Clear();//清除session
                HttpContext.Current.Response.Cookies.Clear();//清除cookie
            }
            catch
            {

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