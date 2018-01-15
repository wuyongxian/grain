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
    /// wbinfo 的摘要说明
    /// </summary>
    public class wbinfo : IHttpHandler, IRequiresSessionState
    {


        public void ProcessRequest(HttpContext context)
        {
            common.IsLogin();
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {

                    case "GetCompanyInfo": GetCompanyInfo(context); break;
                    case "GetWBPushMsgState": GetWBPushMsgState(context); break;
                    case "UpdateWBPushMsgState": UpdateWBPushMsgState(context); break;
                    case "GetWBNameForCommuneQuery": GetWBNameForCommuneQuery(context); break;
                    case "GetMenuAuthority": GetMenuAuthority(context); break;
                    case "GetWBAuthority": GetWBAuthority(context); break;//获取网点权限信息
                    case "GetCurrentWBInfo": GetCurrentWBInfo(context); break;//当前账号登录的网点信息
                    case "GetWBByName": GetWBByName(context); break;//根据网店姓名查询网店
                    case "GetWBAll": GetWBAll(context); break;
                    case "GetWBLogin": GetWBLogin(context); break;//当前登陆的网店信息（总部除外）
                        
                    case "GetUserLimit": GetUserLimit(context); break;//获取营业员支取限额
                    case "GetUserLimit_sell": GetUserLimit_sell(context); break;//获取营业员支取限额
                    case "GetUserLimit_shopping": GetUserLimit_shopping(context); break;//获取营业员支取限额

                    case "GetPrintSetting": GetPrintSetting(context); break;
                    case "UpdatePrintSetting": UpdatePrintSetting(context); break;//社员存折坐标更新

                    case "GetPrintSetting_Dep": GetPrintSetting_Dep(context); break;
                    case "UpdatePrintSetting_Dep": UpdatePrintSetting_Dep(context); break;//储户存折坐标更新


                    case "GetPrintSettingModel": GetPrintSettingModel(context); break;
                    case "UpdatePrintSettingModel": UpdatePrintSettingModel(context); break;//社员存折坐标更新

                    case "GetPrintSetting_DepModel": GetPrintSetting_DepModel(context); break;
                    case "UpdatePrintSetting_DepModel": UpdatePrintSetting_DepModel(context); break;//储户存折坐标更新

                    case "SetWBPrintSetting": SetWBPrintSetting(context); break;
                    case "SetWBPrintSetting_Dep": SetWBPrintSetting_Dep(context); break;


                    case "ShowPrintText": ShowPrintText(context); break;
                    case "ShowCommuneInfo": ShowCommuneInfo(context); break;


                    case "GetUserBySessionID": GetUserBySessionID(context); break;//获取当前的社员信息

                    case "UpdateUserPart": UpdateUserPart(context); break;//更新当前的社员信息
                
                    case "GetUserPWD": GetUserPWD(context); break;//js获取经过加密后的字串
                 
                }
            }

        }

        void GetCompanyInfo(HttpContext context)
        {

            DataTable dt = SQLHelper.ExecuteDataTable(" select * from BD_Company");
            if (dt != null &&dt.Rows.Count!=0)
            {
               
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 获取当前网点最近的一个未读消息
        /// </summary>
        /// <param name="context"></param>
        void GetWBPushMsgState(HttpContext context) {
            string WBID = context.Session["WB_ID"].ToString();
            string idlist = context.Request.QueryString["idlist"].ToString();
            string sql = "SELECT  msgID,wbid,numstate FROM WBPushMsgState WHERE msgID in(" + idlist + ")";

            DataTable dtmsg = SQLHelper.ExecuteDataTable(sql);

            string[] idlistarray = idlist.Split(',');
            if (dtmsg == null || dtmsg.Rows.Count == 0)
            {
                //未找到未读消息记录
                string unReadID = idlistarray[0].Substring(1);
                unReadID = unReadID.Substring(0, unReadID.Length - 1);
                context.Response.Write(unReadID);
                return;
            }
            else {
                for (int i = 0; i < idlistarray.Length; i++) { 
                    string unReadID = idlistarray[i].Substring(1);
                    unReadID = unReadID.Substring(0, unReadID.Length - 1);
                    bool UnWriteMsg = true;//此条消息是否被写进本地数据库记录
                    for (int j = 0; j < dtmsg.Rows.Count; j++) {
                        string msgID = dtmsg.Rows[j]["msgID"].ToString();
                        if (msgID == unReadID) {
                            UnWriteMsg = false;
                        }
                    }
                    if (UnWriteMsg)
                    {
                        context.Response.Write(unReadID);
                        return;
                    }
                }
               
            }

            if (dtmsg != null && dtmsg.Rows.Count != 0)
            {
                for (int k = 0; k < dtmsg.Rows.Count; k++)
                {
                    string wbidlist = dtmsg.Rows[k]["wbid"].ToString();
                    string[] wbidarray = wbidlist.Split('|');
                    bool exitCurrentwbid = false;//当前网点没有阅读此条消息的记录
                    for (int i = 0; i < wbidarray.Length; i++)
                    {
                        if (WBID == wbidarray[i])
                        {
                            exitCurrentwbid = true;
                        }
                    }
                    if (!exitCurrentwbid)
                    { //当前的这条消息还没有被网点阅读
                        context.Response.Write(dtmsg.Rows[k]["msgID"].ToString());
                        return;
                    }
                }

            }
            context.Response.Write("");
         
        }

        void UpdateWBPushMsgState(HttpContext context) {
            string WBID = context.Session["WB_ID"].ToString();

            //查询所有的网点信息（不包含模拟网点）
            int WBcount = Convert.ToInt32( SQLHelper.ExecuteScalar("  SELECT  count( ID) FROM dbo.WB WHERE ISSimulate=0"));//当前所有的网点个数

            string msgid = context.Request.QueryString["msgid"].ToString();
            string sql = "SELECT top 1 msgID,wbid,numstate FROM WBPushMsgState WHERE msgID='" + msgid+"'";
            DataTable dtmsg = SQLHelper.ExecuteDataTable(sql);
            int numstate = 0;//此条消息未被全部网点阅读阅读
            if (dtmsg == null || dtmsg.Rows.Count == 0) { //这是第一个阅读此消息的网点
                //向数据库插入阅读记录
                if (WBcount == 1) { numstate = 1; } else { numstate = 0; }
                string sqlinsert = string.Format("INSERT INTO WBPushMsgState(msgID,wbid,numstate,dtAdd) VALUES('{0}','{1}',{2},'{3}')", msgid, WBID,numstate,DateTime.Now.ToString());
                SQLHelper.ExecuteNonQuery(sqlinsert);
                context.Response.Write(numstate.ToString());
            }
            else//此前已经站点阅读过此条消息
            {
                string wbidlist = dtmsg.Rows[0]["wbid"].ToString();
                string[] wbidarray = wbidlist.Split('|');
                bool exitCurrentwbid = false;//当前网点没有阅读此条消息的记录
                for (int i = 0; i < wbidarray.Length; i++) {
                    if (WBID == wbidarray[i]) {
                        exitCurrentwbid = true;
                    }
                }
                if (!exitCurrentwbid) {
                    wbidlist += "|" + WBID;
                    if (wbidarray.Length + 1 >= WBcount) { numstate = 1; } else { numstate = 0; }
                    string sqlupdate = string.Format("   UPDATE WBPushMsgState SET wbid='{0}',numstate={1},dtUpdateLast='{2}' WHERE msgID='{3}'", wbidlist, numstate,DateTime.Now.ToString(), msgid);
                    SQLHelper.ExecuteNonQuery(sqlupdate);//插入更新记录
                }
               
                context.Response.Write(numstate.ToString());


            }

        }

        //根据Sesseion获取ID
        void GetUserBySessionID(HttpContext context)
        {
            string ID = context.Session["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SerialNumber,UserGroup_ID,WB_ID,strRealName,strLoginName,strPassword,strPhone,strAddress,numLimitAmount,numLimitAmount_sell,numLimitAmount_shopping,ISEnable,dt_Add,dt_Update ");
            strSql.Append(" FROM [Users] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.BigInt)};
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



        //修改登陆用户的部分信息
        void UpdateUserPart(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();


            string strRealName = context.Request.Form["strRealName"].ToString();
            string strLoginName = context.Request.Form["strLoginName"].ToString();
            string strPassword = context.Request.Form["strPassword"].ToString();

            string strPhone = context.Request.Form["strPhone"].ToString();
            string strAddress = context.Request.Form["strAddress"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [Users] set ");
            strSql.Append("strRealName=@strRealName,");
            strSql.Append("strLoginName=@strLoginName,");
            strSql.Append("strPhone=@strPhone,");
            strSql.Append("strAddress=@strAddress,");
            strSql.Append("dt_Update=@dt_Update");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@strRealName", SqlDbType.NVarChar,50),
					new SqlParameter("@strLoginName", SqlDbType.NVarChar,50),
					new SqlParameter("@strPhone", SqlDbType.NVarChar,50),
					new SqlParameter("@strAddress", SqlDbType.NVarChar,50),
					new SqlParameter("@dt_Update", SqlDbType.DateTime),
					new SqlParameter("@ID", SqlDbType.BigInt,8)};
            parameters[0].Value = strRealName;
            parameters[1].Value = strLoginName;
            parameters[2].Value = strPhone;
            parameters[3].Value = strAddress;
            parameters[4].Value = DateTime.Now;
            parameters[5].Value = wbid;

            if (strPassword.Trim() != "")//是否修改密码
            {
                strPassword = Fun.GetMD5_32(strPassword);// 获取md5加密后的用户密码
                string strSqlUpdate = " update [Users] set strPassword='" + strPassword + "' where ID=" + wbid;
                SQLHelper.ExecuteNonQuery(strSqlUpdate);
            }


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetUserPWD(HttpContext context)
        {
            string strPWD = context.Request.QueryString["PWD"].ToString();
            context.Response.Write(Fun.GetMD5_32(strPWD));
           
        }


        /// <summary>
        /// 查询社员时，时总部可查询全部，网点查询自身
        /// </summary>
        /// <param name="context"></param>
        void GetWBNameForCommuneQuery(HttpContext context)
        {
            string strReturn = "";
            string WBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT ID,strName ,ISHQ,ISSimulate  FROM dbo.WB WHERE ID="+WBID;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null && dt.Rows.Count != 0)
            {
                bool ISHQ = Convert.ToBoolean(dt.Rows[0]["ISHQ"]);
                if (!ISHQ)
                {
                    strReturn = dt.Rows[0]["strName"].ToString();
                }
            }
            else {
                strReturn = "Error";
            }

            context.Response.Write(strReturn);

        }

        /// <summary>
        /// 获取登录用户的菜单权限
        /// </summary>
        /// <param name="context"></param>
        void GetMenuAuthority(HttpContext context)
        {
            string MenuID = context.Request.QueryString["MenuID"].ToString();
            bool Authority_R = common.GetAuthority(context.Session["UserGroup_Name"], context.Session["ID"], MenuID, "R");
            bool Authority_A = common.GetAuthority(context.Session["UserGroup_Name"], context.Session["ID"], MenuID, "A");
            bool Authority_E = common.GetAuthority(context.Session["UserGroup_Name"], context.Session["ID"], MenuID, "E");
            var res = new { R = Authority_R, A = Authority_A, E = Authority_E };

            context.Response.Write(JsonHelper.ToJson(res));
        }


        void GetWBAuthority(HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT TOP 1 * FROM dbo.WBAuthority");
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
        /// 根据网店名称查询网店
        /// </summary>
        /// <param name="context"></param>
        void GetCurrentWBInfo(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            StringBuilder strSql = new StringBuilder();

            strSql.Append(" SELECT *  FROM WB   WHERE  ID="+WBID);//排除总部和模拟网点
           
            strSql.Append("  ");



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

        ///// <summary>
        ///// 根据网店名称查询网店
        ///// </summary>
        ///// <param name="context"></param>
        //void GetWBByName(HttpContext context)
        //{
        //    string strName = context.Request.QueryString["strName"].ToString();
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("   SELECT ID,strName ");
        //    strSql.Append("   FROM dbo.WB");
        //    strSql.Append("   WHERE  ISHQ=0 and ISSimulate=0");//排除总部和模拟网点
        //    if (strName.Trim() != "")
        //    {
        //        strSql.Append("  AND strName LIKE '%"+strName+"%' ");
        //    }
        //    strSql.Append("  ");



        //    DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
        //    if (dt != null && dt.Rows.Count != 0)
        //    {
        //        context.Response.Write(JsonHelper.ToJson(dt));
        //    }
        //    else
        //    {
        //        context.Response.Write("Error");
        //    }
        //}


        /// <summary>
        /// 根据网店名称查询网店
        /// </summary>
        /// <param name="context"></param>
        void GetWBByName(HttpContext context)
        {
            string strName = context.Request.QueryString["strName"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,strName ");
            strSql.Append("   FROM dbo.WB");
            strSql.Append("   WHERE  ISHQ=0 and ISSimulate=0");//排除总部和模拟网点
            if (strName.Trim() != "")
            {
                strSql.Append("  AND strName LIKE '%" + strName + "%' ");
            }
            strSql.Append("  ");
            DataTable dt= SQLHelper.ExecuteDataTable(strSql.ToString());

            DataTable dt2 = null;
            if (strName.Trim() != "")
            {
                StringBuilder strSql2 = new StringBuilder();
                strSql2.Append("   SELECT ID,strName ");
                strSql2.Append("   FROM dbo.WB");
                strSql2.Append("   WHERE  ISHQ=0 and ISSimulate=0");//排除总部和模拟网点
                strSql2.Append("  AND strName not LIKE '%" + strName + "%' ");
                dt2 = SQLHelper.ExecuteDataTable(strSql2.ToString());
            }
            if (dt2 != null && dt2.Rows.Count != 0)
            {
                dt.Merge(dt2);
            }
            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetWBAll(HttpContext context)
        {

            string sql = " select * from WB";
           
            DataTable dt = SQLHelper.ExecuteDataTable(sql);

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
        /// 根据当前登录的网点信息
        /// </summary>
        /// <param name="context"></param>
        void GetWBLogin(HttpContext context)
        {
            string strName = context.Request.QueryString["strName"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,strName ");
            strSql.Append("   FROM dbo.WB");
            strSql.Append("   WHERE  ISHQ=0 and ID=" +context.Session["WB_ID"]);//排除总部和模拟网点
            if (strName.Trim() != "")
            {
                strSql.Append("  AND strName LIKE '%" + strName + "%' ");
            }
            strSql.Append("  ");



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

        //获取营业员的支取限额
        void GetUserLimit(HttpContext context)
        {
          

            string ID = context.Session["ID"].ToString();
            string strSql = " SELECT numLimitAmount FROM dbo.Users  WHERE ID="+ID;
            object obj = SQLHelper.ExecuteScalar(strSql);
            if (obj!=null)
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetUserLimit_sell(HttpContext context)
        {


            string ID = context.Session["ID"].ToString();
            string strSql = " SELECT numLimitAmount_sell FROM dbo.Users  WHERE ID=" + ID;
            object obj = SQLHelper.ExecuteScalar(strSql);
            if (obj != null)
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetUserLimit_shopping(HttpContext context)
        {


            string ID = context.Session["ID"].ToString();
            string strSql = " SELECT numLimitAmount_shopping FROM dbo.Users  WHERE ID=" + ID;
            object obj = SQLHelper.ExecuteScalar(strSql);
            if (obj != null)
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        #region 获取和设置每个网店的打印坐标
        void GetPrintSetting(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X " ;
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += " where 1=1 and WBID="+WBID;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null&&dt.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdatePrintSetting(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
       
            string Width = context.Request.Form["Width"].ToString();
            string Height = context.Request.Form["Height"].ToString();
            string DriftRateX = context.Request.Form["DriftRateX"].ToString();
            string DriftRateY = context.Request.Form["DriftRateY"].ToString();
            string FontSize = context.Request.Form["FontSize"].ToString();

            string HomeR1C1X = context.Request.Form["HomeR1C1X"].ToString();
            string HomeR1C1Y = context.Request.Form["HomeR1C1Y"].ToString();
            string HomeR1C2X = context.Request.Form["HomeR1C2X"].ToString();
            string HomeR1C2Y = context.Request.Form["HomeR1C2Y"].ToString();

            string HomeR2C1X = context.Request.Form["HomeR2C1X"].ToString();
            string HomeR2C1Y = context.Request.Form["HomeR2C1Y"].ToString();
            string HomeR2C2X = context.Request.Form["HomeR2C2X"].ToString();
            string HomeR2C2Y = context.Request.Form["HomeR2C2Y"].ToString();

            string HomeR3C1X = context.Request.Form["HomeR3C1X"].ToString();
            string HomeR3C1Y = context.Request.Form["HomeR3C1Y"].ToString();
            string HomeR3C2X = context.Request.Form["HomeR3C2X"].ToString();
            string HomeR3C2Y = context.Request.Form["HomeR3C2Y"].ToString();

            string HomeR4C1X = context.Request.Form["HomeR4C1X"].ToString();
            string HomeR4C1Y = context.Request.Form["HomeR4C1Y"].ToString();
            string HomeR4C2X = context.Request.Form["HomeR4C2X"].ToString();
            string HomeR4C2Y = context.Request.Form["HomeR4C2Y"].ToString();

            string HomeR5C1X = context.Request.Form["HomeR5C1X"].ToString();
            string HomeR5C1Y = context.Request.Form["HomeR5C1Y"].ToString();
            string HomeR5C2X = context.Request.Form["HomeR5C2X"].ToString();
            string HomeR5C2Y = context.Request.Form["HomeR5C2Y"].ToString();

            string RecordR1Y = context.Request.Form["RecordR1Y"].ToString();
            string RecordR2Y = context.Request.Form["RecordR2Y"].ToString();
            string RecordR3Y = context.Request.Form["RecordR3Y"].ToString();
            string RecordR4Y = context.Request.Form["RecordR4Y"].ToString();
            string RecordR5Y = context.Request.Form["RecordR5Y"].ToString();
            string RecordR6Y = context.Request.Form["RecordR6Y"].ToString();
            string RecordR7Y = context.Request.Form["RecordR7Y"].ToString();
            string RecordR8Y = context.Request.Form["RecordR8Y"].ToString();
            string RecordR9Y = context.Request.Form["RecordR9Y"].ToString();
            string RecordR10Y = context.Request.Form["RecordR10Y"].ToString();
            string RecordR11Y = context.Request.Form["RecordR11Y"].ToString();
            string RecordR12Y = context.Request.Form["RecordR12Y"].ToString();
            string RecordR13Y = context.Request.Form["RecordR13Y"].ToString();
            string RecordR14Y = context.Request.Form["RecordR14Y"].ToString();
            string RecordR15Y = context.Request.Form["RecordR15Y"].ToString();
            string RecordR16Y = context.Request.Form["RecordR16Y"].ToString();
            string RecordR17Y = context.Request.Form["RecordR17Y"].ToString();
            string RecordR18Y = context.Request.Form["RecordR18Y"].ToString();
            string RecordR19Y = context.Request.Form["RecordR19Y"].ToString();
            string RecordR20Y = context.Request.Form["RecordR20Y"].ToString();

            string RecordC1X = context.Request.Form["RecordC1X"].ToString();
            string RecordC2X = context.Request.Form["RecordC2X"].ToString();
            string RecordC3X = context.Request.Form["RecordC3X"].ToString();
            string RecordC4X = context.Request.Form["RecordC4X"].ToString();
            string RecordC5X = context.Request.Form["RecordC5X"].ToString();
            string RecordC6X = context.Request.Form["RecordC6X"].ToString();
            string RecordC7X = context.Request.Form["RecordC7X"].ToString();
            string RecordC8X = context.Request.Form["RecordC8X"].ToString();
            string RecordC9X = context.Request.Form["RecordC9X"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [PrintSetting] set ");
            strSql.Append("Width=@Width,");
            strSql.Append("Height=@Height,");
            strSql.Append("DriftRateX=@DriftRateX,");
            strSql.Append("DriftRateY=@DriftRateY,");
            strSql.Append("FontSize=@FontSize,");
            strSql.Append("HomeR1C1X=@HomeR1C1X,");
            strSql.Append("HomeR1C1Y=@HomeR1C1Y,");
            strSql.Append("HomeR1C2X=@HomeR1C2X,");
            strSql.Append("HomeR1C2Y=@HomeR1C2Y,");
            strSql.Append("HomeR2C1X=@HomeR2C1X,");
            strSql.Append("HomeR2C1Y=@HomeR2C1Y,");
            strSql.Append("HomeR2C2X=@HomeR2C2X,");
            strSql.Append("HomeR2C2Y=@HomeR2C2Y,");
            strSql.Append("HomeR3C1X=@HomeR3C1X,");
            strSql.Append("HomeR3C1Y=@HomeR3C1Y,");
            strSql.Append("HomeR3C2X=@HomeR3C2X,");
            strSql.Append("HomeR3C2Y=@HomeR3C2Y,");
            strSql.Append("HomeR4C1X=@HomeR4C1X,");
            strSql.Append("HomeR4C1Y=@HomeR4C1Y,");
            strSql.Append("HomeR4C2X=@HomeR4C2X,");
            strSql.Append("HomeR4C2Y=@HomeR4C2Y,");
            strSql.Append("HomeR5C1X=@HomeR5C1X,");
            strSql.Append("HomeR5C1Y=@HomeR5C1Y,");
            strSql.Append("HomeR5C2X=@HomeR5C2X,");
            strSql.Append("HomeR5C2Y=@HomeR5C2Y,");
            strSql.Append("RecordR1Y=@RecordR1Y,");
            strSql.Append("RecordR2Y=@RecordR2Y,");
            strSql.Append("RecordR3Y=@RecordR3Y,");
            strSql.Append("RecordR4Y=@RecordR4Y,");
            strSql.Append("RecordR5Y=@RecordR5Y,");
            strSql.Append("RecordR6Y=@RecordR6Y,");
            strSql.Append("RecordR7Y=@RecordR7Y,");
            strSql.Append("RecordR8Y=@RecordR8Y,");
            strSql.Append("RecordR9Y=@RecordR9Y,");
            strSql.Append("RecordR10Y=@RecordR10Y,");
            strSql.Append("RecordR11Y=@RecordR11Y,");
            strSql.Append("RecordR12Y=@RecordR12Y,");
            strSql.Append("RecordR13Y=@RecordR13Y,");
            strSql.Append("RecordR14Y=@RecordR14Y,");
            strSql.Append("RecordR15Y=@RecordR15Y,");
            strSql.Append("RecordR16Y=@RecordR16Y,");
            strSql.Append("RecordR17Y=@RecordR17Y,");
            strSql.Append("RecordR18Y=@RecordR18Y,");
            strSql.Append("RecordR19Y=@RecordR19Y,");
            strSql.Append("RecordR20Y=@RecordR20Y,");
            strSql.Append("RecordC1X=@RecordC1X,");
            strSql.Append("RecordC2X=@RecordC2X,");
            strSql.Append("RecordC3X=@RecordC3X,");
            strSql.Append("RecordC4X=@RecordC4X,");
            strSql.Append("RecordC5X=@RecordC5X,");
            strSql.Append("RecordC6X=@RecordC6X,");
            strSql.Append("RecordC7X=@RecordC7X,");
            strSql.Append("RecordC8X=@RecordC8X,");
            strSql.Append("RecordC9X=@RecordC9X");
            strSql.Append("  where WBID="+WBID);
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4),
					new SqlParameter("@Width", SqlDbType.Int,4),
					new SqlParameter("@Height", SqlDbType.Int,4),
					new SqlParameter("@DriftRateX", SqlDbType.Int,4),
					new SqlParameter("@DriftRateY", SqlDbType.Int,4),
					new SqlParameter("@FontSize", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C2Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR1Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR2Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR3Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR4Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR5Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR6Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR7Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR8Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR9Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR10Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR11Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR12Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR13Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR14Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR15Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR16Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR17Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR18Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR19Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR20Y", SqlDbType.Int,4),
					new SqlParameter("@RecordC1X", SqlDbType.Int,4),
					new SqlParameter("@RecordC2X", SqlDbType.Int,4),
					new SqlParameter("@RecordC3X", SqlDbType.Int,4),
					new SqlParameter("@RecordC4X", SqlDbType.Int,4),
					new SqlParameter("@RecordC5X", SqlDbType.Int,4),
					new SqlParameter("@RecordC6X", SqlDbType.Int,4),
					new SqlParameter("@RecordC7X", SqlDbType.Int,4),
					new SqlParameter("@RecordC8X", SqlDbType.Int,4),
					new SqlParameter("@RecordC9X", SqlDbType.Int,4)};
            parameters[0].Value = 0;
            parameters[1].Value = Width;
            parameters[2].Value = Height;
            parameters[3].Value = DriftRateX;
            parameters[4].Value = DriftRateY;
            parameters[5].Value = FontSize;
            parameters[6].Value = HomeR1C1X;
            parameters[7].Value = HomeR1C1Y;
            parameters[8].Value = HomeR1C2X;
            parameters[9].Value = HomeR1C2Y;
            parameters[10].Value = HomeR2C1X;
            parameters[11].Value = HomeR2C1Y;
            parameters[12].Value = HomeR2C2X;
            parameters[13].Value = HomeR2C2Y;
            parameters[14].Value = HomeR3C1X;
            parameters[15].Value = HomeR3C1Y;
            parameters[16].Value = HomeR3C2X;
            parameters[17].Value = HomeR3C2Y;
            parameters[18].Value = HomeR4C1X;
            parameters[19].Value = HomeR4C1Y;
            parameters[20].Value = HomeR4C2X;
            parameters[21].Value = HomeR4C2Y;
            parameters[22].Value = HomeR5C1X;
            parameters[23].Value = HomeR5C1Y;
            parameters[24].Value = HomeR5C2X;
            parameters[25].Value = HomeR5C2Y;
            parameters[26].Value = RecordR1Y;
            parameters[27].Value = RecordR2Y;
            parameters[28].Value = RecordR3Y;
            parameters[29].Value = RecordR4Y;
            parameters[30].Value = RecordR5Y;
            parameters[31].Value = RecordR6Y;
            parameters[32].Value = RecordR7Y;
            parameters[33].Value = RecordR8Y;
            parameters[34].Value = RecordR9Y;
            parameters[35].Value = RecordR10Y;
            parameters[36].Value = RecordR11Y;
            parameters[37].Value = RecordR12Y;
            parameters[38].Value = RecordR13Y;
            parameters[39].Value = RecordR14Y;
            parameters[40].Value = RecordR15Y;
            parameters[41].Value = RecordR16Y;
            parameters[42].Value = RecordR17Y;
            parameters[43].Value = RecordR18Y;
            parameters[44].Value = RecordR19Y;
            parameters[45].Value = RecordR20Y;
            parameters[46].Value = RecordC1X;
            parameters[47].Value = RecordC2X;
            parameters[48].Value = RecordC3X;
            parameters[49].Value = RecordC4X;
            parameters[50].Value = RecordC5X;
            parameters[51].Value = RecordC6X;
            parameters[52].Value = RecordC7X;
            parameters[53].Value = RecordC8X;
            parameters[54].Value = RecordC9X;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }

        }


        void GetPrintSetting_Dep(HttpContext context)
        {
            
            string WBID = context.Session["WB_ID"].ToString();
           // string WBID = "15";
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += " where 1=1 and WBID="+WBID;
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

        void UpdatePrintSetting_Dep(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
      
            string Width = context.Request.Form["Width"].ToString();
            string Height = context.Request.Form["Height"].ToString();
            string DriftRateX = context.Request.Form["DriftRateX"].ToString();
            string DriftRateY = context.Request.Form["DriftRateY"].ToString();
            string FontSize = context.Request.Form["FontSize"].ToString();

            string HomeR1C1X = context.Request.Form["HomeR1C1X"].ToString();
            string HomeR1C1Y = context.Request.Form["HomeR1C1Y"].ToString();
            string HomeR1C2X = context.Request.Form["HomeR1C2X"].ToString();
            string HomeR1C2Y = context.Request.Form["HomeR1C2Y"].ToString();

            string HomeR2C1X = context.Request.Form["HomeR2C1X"].ToString();
            string HomeR2C1Y = context.Request.Form["HomeR2C1Y"].ToString();
            string HomeR2C2X = context.Request.Form["HomeR2C2X"].ToString();
            string HomeR2C2Y = context.Request.Form["HomeR2C2Y"].ToString();

            string HomeR3C1X = context.Request.Form["HomeR3C1X"].ToString();
            string HomeR3C1Y = context.Request.Form["HomeR3C1Y"].ToString();
            string HomeR3C2X = context.Request.Form["HomeR3C2X"].ToString();
            string HomeR3C2Y = context.Request.Form["HomeR3C2Y"].ToString();

            string HomeR4C1X = context.Request.Form["HomeR4C1X"].ToString();
            string HomeR4C1Y = context.Request.Form["HomeR4C1Y"].ToString();
            string HomeR4C2X = context.Request.Form["HomeR4C2X"].ToString();
            string HomeR4C2Y = context.Request.Form["HomeR4C2Y"].ToString();

            string HomeR5C1X = context.Request.Form["HomeR5C1X"].ToString();
            string HomeR5C1Y = context.Request.Form["HomeR5C1Y"].ToString();
            string HomeR5C2X = context.Request.Form["HomeR5C2X"].ToString();
            string HomeR5C2Y = context.Request.Form["HomeR5C2Y"].ToString();

            string RecordR1Y = context.Request.Form["RecordR1Y"].ToString();
            string RecordR2Y = context.Request.Form["RecordR2Y"].ToString();
            string RecordR3Y = context.Request.Form["RecordR3Y"].ToString();
            string RecordR4Y = context.Request.Form["RecordR4Y"].ToString();
            string RecordR5Y = context.Request.Form["RecordR5Y"].ToString();
            string RecordR6Y = context.Request.Form["RecordR6Y"].ToString();
            string RecordR7Y = context.Request.Form["RecordR7Y"].ToString();
            string RecordR8Y = context.Request.Form["RecordR8Y"].ToString();
            string RecordR9Y = context.Request.Form["RecordR9Y"].ToString();
            string RecordR10Y = context.Request.Form["RecordR10Y"].ToString();
            string RecordR11Y = context.Request.Form["RecordR11Y"].ToString();
            string RecordR12Y = context.Request.Form["RecordR12Y"].ToString();
            string RecordR13Y = context.Request.Form["RecordR13Y"].ToString();
            string RecordR14Y = context.Request.Form["RecordR14Y"].ToString();
            string RecordR15Y = context.Request.Form["RecordR15Y"].ToString();
            string RecordR16Y = context.Request.Form["RecordR16Y"].ToString();
            string RecordR17Y = context.Request.Form["RecordR17Y"].ToString();
            string RecordR18Y = context.Request.Form["RecordR18Y"].ToString();
            string RecordR19Y = context.Request.Form["RecordR19Y"].ToString();
            string RecordR20Y = context.Request.Form["RecordR20Y"].ToString();

            string RecordC1X = context.Request.Form["RecordC1X"].ToString();
            string RecordC2X = context.Request.Form["RecordC2X"].ToString();
            string RecordC3X = context.Request.Form["RecordC3X"].ToString();
            string RecordC4X = context.Request.Form["RecordC4X"].ToString();
            string RecordC5X = context.Request.Form["RecordC5X"].ToString();
            string RecordC6X = context.Request.Form["RecordC6X"].ToString();
            string RecordC7X = context.Request.Form["RecordC7X"].ToString();
            string RecordC8X = context.Request.Form["RecordC8X"].ToString();
            string RecordC9X = context.Request.Form["RecordC9X"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [PrintSetting_Dep] set ");
          
            strSql.Append("Width=@Width,");
            strSql.Append("Height=@Height,");
            strSql.Append("DriftRateX=@DriftRateX,");
            strSql.Append("DriftRateY=@DriftRateY,");
            strSql.Append("FontSize=@FontSize,");
            strSql.Append("HomeR1C1X=@HomeR1C1X,");
            strSql.Append("HomeR1C1Y=@HomeR1C1Y,");
            strSql.Append("HomeR1C2X=@HomeR1C2X,");
            strSql.Append("HomeR1C2Y=@HomeR1C2Y,");
            strSql.Append("HomeR2C1X=@HomeR2C1X,");
            strSql.Append("HomeR2C1Y=@HomeR2C1Y,");
            strSql.Append("HomeR2C2X=@HomeR2C2X,");
            strSql.Append("HomeR2C2Y=@HomeR2C2Y,");
            strSql.Append("HomeR3C1X=@HomeR3C1X,");
            strSql.Append("HomeR3C1Y=@HomeR3C1Y,");
            strSql.Append("HomeR3C2X=@HomeR3C2X,");
            strSql.Append("HomeR3C2Y=@HomeR3C2Y,");
            strSql.Append("HomeR4C1X=@HomeR4C1X,");
            strSql.Append("HomeR4C1Y=@HomeR4C1Y,");
            strSql.Append("HomeR4C2X=@HomeR4C2X,");
            strSql.Append("HomeR4C2Y=@HomeR4C2Y,");
            strSql.Append("HomeR5C1X=@HomeR5C1X,");
            strSql.Append("HomeR5C1Y=@HomeR5C1Y,");
            strSql.Append("HomeR5C2X=@HomeR5C2X,");
            strSql.Append("HomeR5C2Y=@HomeR5C2Y,");
            strSql.Append("RecordR1Y=@RecordR1Y,");
            strSql.Append("RecordR2Y=@RecordR2Y,");
            strSql.Append("RecordR3Y=@RecordR3Y,");
            strSql.Append("RecordR4Y=@RecordR4Y,");
            strSql.Append("RecordR5Y=@RecordR5Y,");
            strSql.Append("RecordR6Y=@RecordR6Y,");
            strSql.Append("RecordR7Y=@RecordR7Y,");
            strSql.Append("RecordR8Y=@RecordR8Y,");
            strSql.Append("RecordR9Y=@RecordR9Y,");
            strSql.Append("RecordR10Y=@RecordR10Y,");
            strSql.Append("RecordR11Y=@RecordR11Y,");
            strSql.Append("RecordR12Y=@RecordR12Y,");
            strSql.Append("RecordR13Y=@RecordR13Y,");
            strSql.Append("RecordR14Y=@RecordR14Y,");
            strSql.Append("RecordR15Y=@RecordR15Y,");
            strSql.Append("RecordR16Y=@RecordR16Y,");
            strSql.Append("RecordR17Y=@RecordR17Y,");
            strSql.Append("RecordR18Y=@RecordR18Y,");
            strSql.Append("RecordR19Y=@RecordR19Y,");
            strSql.Append("RecordR20Y=@RecordR20Y,");
            strSql.Append("RecordC1X=@RecordC1X,");
            strSql.Append("RecordC2X=@RecordC2X,");
            strSql.Append("RecordC3X=@RecordC3X,");
            strSql.Append("RecordC4X=@RecordC4X,");
            strSql.Append("RecordC5X=@RecordC5X,");
            strSql.Append("RecordC6X=@RecordC6X,");
            strSql.Append("RecordC7X=@RecordC7X,");
            strSql.Append("RecordC8X=@RecordC8X,");
            strSql.Append("RecordC9X=@RecordC9X");
            strSql.Append(" where WBID="+WBID);

            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4),
					new SqlParameter("@Width", SqlDbType.Int,4),
					new SqlParameter("@Height", SqlDbType.Int,4),
					new SqlParameter("@DriftRateX", SqlDbType.Int,4),
					new SqlParameter("@DriftRateY", SqlDbType.Int,4),
					new SqlParameter("@FontSize", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C2Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR1Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR2Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR3Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR4Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR5Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR6Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR7Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR8Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR9Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR10Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR11Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR12Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR13Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR14Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR15Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR16Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR17Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR18Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR19Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR20Y", SqlDbType.Int,4),
					new SqlParameter("@RecordC1X", SqlDbType.Int,4),
					new SqlParameter("@RecordC2X", SqlDbType.Int,4),
					new SqlParameter("@RecordC3X", SqlDbType.Int,4),
					new SqlParameter("@RecordC4X", SqlDbType.Int,4),
					new SqlParameter("@RecordC5X", SqlDbType.Int,4),
					new SqlParameter("@RecordC6X", SqlDbType.Int,4),
					new SqlParameter("@RecordC7X", SqlDbType.Int,4),
					new SqlParameter("@RecordC8X", SqlDbType.Int,4),
					new SqlParameter("@RecordC9X", SqlDbType.Int,4)};
            parameters[0].Value = 0;
            parameters[1].Value = Width;
            parameters[2].Value = Height;
            parameters[3].Value = DriftRateX;
            parameters[4].Value = DriftRateY;
            parameters[5].Value = FontSize;
            parameters[6].Value = HomeR1C1X;
            parameters[7].Value = HomeR1C1Y;
            parameters[8].Value = HomeR1C2X;
            parameters[9].Value = HomeR1C2Y;
            parameters[10].Value = HomeR2C1X;
            parameters[11].Value = HomeR2C1Y;
            parameters[12].Value = HomeR2C2X;
            parameters[13].Value = HomeR2C2Y;
            parameters[14].Value = HomeR3C1X;
            parameters[15].Value = HomeR3C1Y;
            parameters[16].Value = HomeR3C2X;
            parameters[17].Value = HomeR3C2Y;
            parameters[18].Value = HomeR4C1X;
            parameters[19].Value = HomeR4C1Y;
            parameters[20].Value = HomeR4C2X;
            parameters[21].Value = HomeR4C2Y;
            parameters[22].Value = HomeR5C1X;
            parameters[23].Value = HomeR5C1Y;
            parameters[24].Value = HomeR5C2X;
            parameters[25].Value = HomeR5C2Y;
            parameters[26].Value = RecordR1Y;
            parameters[27].Value = RecordR2Y;
            parameters[28].Value = RecordR3Y;
            parameters[29].Value = RecordR4Y;
            parameters[30].Value = RecordR5Y;
            parameters[31].Value = RecordR6Y;
            parameters[32].Value = RecordR7Y;
            parameters[33].Value = RecordR8Y;
            parameters[34].Value = RecordR9Y;
            parameters[35].Value = RecordR10Y;
            parameters[36].Value = RecordR11Y;
            parameters[37].Value = RecordR12Y;
            parameters[38].Value = RecordR13Y;
            parameters[39].Value = RecordR14Y;
            parameters[40].Value = RecordR15Y;
            parameters[41].Value = RecordR16Y;
            parameters[42].Value = RecordR17Y;
            parameters[43].Value = RecordR18Y;
            parameters[44].Value = RecordR19Y;
            parameters[45].Value = RecordR20Y;
            parameters[46].Value = RecordC1X;
            parameters[47].Value = RecordC2X;
            parameters[48].Value = RecordC3X;
            parameters[49].Value = RecordC4X;
            parameters[50].Value = RecordC5X;
            parameters[51].Value = RecordC6X;
            parameters[52].Value = RecordC7X;
            parameters[53].Value = RecordC8X;
            parameters[54].Value = RecordC9X;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }

        }
        #endregion 



        #region 获取和设置打印坐标模板
        void GetPrintSettingModel(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting] ";
            strSql += " where 1=1 and WBID=0" ;
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

        void UpdatePrintSettingModel(HttpContext context)
        {
         
            string Width = context.Request.Form["Width"].ToString();
            string Height = context.Request.Form["Height"].ToString();
            string DriftRateX = context.Request.Form["DriftRateX"].ToString();
            string DriftRateY = context.Request.Form["DriftRateY"].ToString();
            string FontSize = context.Request.Form["FontSize"].ToString();

            string HomeR1C1X = context.Request.Form["HomeR1C1X"].ToString();
            string HomeR1C1Y = context.Request.Form["HomeR1C1Y"].ToString();
            string HomeR1C2X = context.Request.Form["HomeR1C2X"].ToString();
            string HomeR1C2Y = context.Request.Form["HomeR1C2Y"].ToString();

            string HomeR2C1X = context.Request.Form["HomeR2C1X"].ToString();
            string HomeR2C1Y = context.Request.Form["HomeR2C1Y"].ToString();
            string HomeR2C2X = context.Request.Form["HomeR2C2X"].ToString();
            string HomeR2C2Y = context.Request.Form["HomeR2C2Y"].ToString();

            string HomeR3C1X = context.Request.Form["HomeR3C1X"].ToString();
            string HomeR3C1Y = context.Request.Form["HomeR3C1Y"].ToString();
            string HomeR3C2X = context.Request.Form["HomeR3C2X"].ToString();
            string HomeR3C2Y = context.Request.Form["HomeR3C2Y"].ToString();

            string HomeR4C1X = context.Request.Form["HomeR4C1X"].ToString();
            string HomeR4C1Y = context.Request.Form["HomeR4C1Y"].ToString();
            string HomeR4C2X = context.Request.Form["HomeR4C2X"].ToString();
            string HomeR4C2Y = context.Request.Form["HomeR4C2Y"].ToString();

            string HomeR5C1X = context.Request.Form["HomeR5C1X"].ToString();
            string HomeR5C1Y = context.Request.Form["HomeR5C1Y"].ToString();
            string HomeR5C2X = context.Request.Form["HomeR5C2X"].ToString();
            string HomeR5C2Y = context.Request.Form["HomeR5C2Y"].ToString();

            string RecordR1Y = context.Request.Form["RecordR1Y"].ToString();
            string RecordR2Y = context.Request.Form["RecordR2Y"].ToString();
            string RecordR3Y = context.Request.Form["RecordR3Y"].ToString();
            string RecordR4Y = context.Request.Form["RecordR4Y"].ToString();
            string RecordR5Y = context.Request.Form["RecordR5Y"].ToString();
            string RecordR6Y = context.Request.Form["RecordR6Y"].ToString();
            string RecordR7Y = context.Request.Form["RecordR7Y"].ToString();
            string RecordR8Y = context.Request.Form["RecordR8Y"].ToString();
            string RecordR9Y = context.Request.Form["RecordR9Y"].ToString();
            string RecordR10Y = context.Request.Form["RecordR10Y"].ToString();
            string RecordR11Y = context.Request.Form["RecordR11Y"].ToString();
            string RecordR12Y = context.Request.Form["RecordR12Y"].ToString();
            string RecordR13Y = context.Request.Form["RecordR13Y"].ToString();
            string RecordR14Y = context.Request.Form["RecordR14Y"].ToString();
            string RecordR15Y = context.Request.Form["RecordR15Y"].ToString();
            string RecordR16Y = context.Request.Form["RecordR16Y"].ToString();
            string RecordR17Y = context.Request.Form["RecordR17Y"].ToString();
            string RecordR18Y = context.Request.Form["RecordR18Y"].ToString();
            string RecordR19Y = context.Request.Form["RecordR19Y"].ToString();
            string RecordR20Y = context.Request.Form["RecordR20Y"].ToString();

            string RecordC1X = context.Request.Form["RecordC1X"].ToString();
            string RecordC2X = context.Request.Form["RecordC2X"].ToString();
            string RecordC3X = context.Request.Form["RecordC3X"].ToString();
            string RecordC4X = context.Request.Form["RecordC4X"].ToString();
            string RecordC5X = context.Request.Form["RecordC5X"].ToString();
            string RecordC6X = context.Request.Form["RecordC6X"].ToString();
            string RecordC7X = context.Request.Form["RecordC7X"].ToString();
            string RecordC8X = context.Request.Form["RecordC8X"].ToString();
            string RecordC9X = context.Request.Form["RecordC9X"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [PrintSetting] set ");
          
            strSql.Append("Width=@Width,");
            strSql.Append("Height=@Height,");
            strSql.Append("DriftRateX=@DriftRateX,");
            strSql.Append("DriftRateY=@DriftRateY,");
            strSql.Append("FontSize=@FontSize,");
            strSql.Append("HomeR1C1X=@HomeR1C1X,");
            strSql.Append("HomeR1C1Y=@HomeR1C1Y,");
            strSql.Append("HomeR1C2X=@HomeR1C2X,");
            strSql.Append("HomeR1C2Y=@HomeR1C2Y,");
            strSql.Append("HomeR2C1X=@HomeR2C1X,");
            strSql.Append("HomeR2C1Y=@HomeR2C1Y,");
            strSql.Append("HomeR2C2X=@HomeR2C2X,");
            strSql.Append("HomeR2C2Y=@HomeR2C2Y,");
            strSql.Append("HomeR3C1X=@HomeR3C1X,");
            strSql.Append("HomeR3C1Y=@HomeR3C1Y,");
            strSql.Append("HomeR3C2X=@HomeR3C2X,");
            strSql.Append("HomeR3C2Y=@HomeR3C2Y,");
            strSql.Append("HomeR4C1X=@HomeR4C1X,");
            strSql.Append("HomeR4C1Y=@HomeR4C1Y,");
            strSql.Append("HomeR4C2X=@HomeR4C2X,");
            strSql.Append("HomeR4C2Y=@HomeR4C2Y,");
            strSql.Append("HomeR5C1X=@HomeR5C1X,");
            strSql.Append("HomeR5C1Y=@HomeR5C1Y,");
            strSql.Append("HomeR5C2X=@HomeR5C2X,");
            strSql.Append("HomeR5C2Y=@HomeR5C2Y,");
            strSql.Append("RecordR1Y=@RecordR1Y,");
            strSql.Append("RecordR2Y=@RecordR2Y,");
            strSql.Append("RecordR3Y=@RecordR3Y,");
            strSql.Append("RecordR4Y=@RecordR4Y,");
            strSql.Append("RecordR5Y=@RecordR5Y,");
            strSql.Append("RecordR6Y=@RecordR6Y,");
            strSql.Append("RecordR7Y=@RecordR7Y,");
            strSql.Append("RecordR8Y=@RecordR8Y,");
            strSql.Append("RecordR9Y=@RecordR9Y,");
            strSql.Append("RecordR10Y=@RecordR10Y,");
            strSql.Append("RecordR11Y=@RecordR11Y,");
            strSql.Append("RecordR12Y=@RecordR12Y,");
            strSql.Append("RecordR13Y=@RecordR13Y,");
            strSql.Append("RecordR14Y=@RecordR14Y,");
            strSql.Append("RecordR15Y=@RecordR15Y,");
            strSql.Append("RecordR16Y=@RecordR16Y,");
            strSql.Append("RecordR17Y=@RecordR17Y,");
            strSql.Append("RecordR18Y=@RecordR18Y,");
            strSql.Append("RecordR19Y=@RecordR19Y,");
            strSql.Append("RecordR20Y=@RecordR20Y,");
            strSql.Append("RecordC1X=@RecordC1X,");
            strSql.Append("RecordC2X=@RecordC2X,");
            strSql.Append("RecordC3X=@RecordC3X,");
            strSql.Append("RecordC4X=@RecordC4X,");
            strSql.Append("RecordC5X=@RecordC5X,");
            strSql.Append("RecordC6X=@RecordC6X,");
            strSql.Append("RecordC7X=@RecordC7X,");
            strSql.Append("RecordC8X=@RecordC8X,");
            strSql.Append("RecordC9X=@RecordC9X");
            strSql.Append("  where WBID=0");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4),
					new SqlParameter("@Width", SqlDbType.Int,4),
					new SqlParameter("@Height", SqlDbType.Int,4),
					new SqlParameter("@DriftRateX", SqlDbType.Int,4),
					new SqlParameter("@DriftRateY", SqlDbType.Int,4),
					new SqlParameter("@FontSize", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C2Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR1Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR2Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR3Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR4Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR5Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR6Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR7Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR8Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR9Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR10Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR11Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR12Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR13Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR14Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR15Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR16Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR17Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR18Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR19Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR20Y", SqlDbType.Int,4),
					new SqlParameter("@RecordC1X", SqlDbType.Int,4),
					new SqlParameter("@RecordC2X", SqlDbType.Int,4),
					new SqlParameter("@RecordC3X", SqlDbType.Int,4),
					new SqlParameter("@RecordC4X", SqlDbType.Int,4),
					new SqlParameter("@RecordC5X", SqlDbType.Int,4),
					new SqlParameter("@RecordC6X", SqlDbType.Int,4),
					new SqlParameter("@RecordC7X", SqlDbType.Int,4),
					new SqlParameter("@RecordC8X", SqlDbType.Int,4),
					new SqlParameter("@RecordC9X", SqlDbType.Int,4)};
            parameters[0].Value = 0;
            parameters[1].Value = Width;
            parameters[2].Value = Height;
            parameters[3].Value = DriftRateX;
            parameters[4].Value = DriftRateY;
            parameters[5].Value = FontSize;
            parameters[6].Value = HomeR1C1X;
            parameters[7].Value = HomeR1C1Y;
            parameters[8].Value = HomeR1C2X;
            parameters[9].Value = HomeR1C2Y;
            parameters[10].Value = HomeR2C1X;
            parameters[11].Value = HomeR2C1Y;
            parameters[12].Value = HomeR2C2X;
            parameters[13].Value = HomeR2C2Y;
            parameters[14].Value = HomeR3C1X;
            parameters[15].Value = HomeR3C1Y;
            parameters[16].Value = HomeR3C2X;
            parameters[17].Value = HomeR3C2Y;
            parameters[18].Value = HomeR4C1X;
            parameters[19].Value = HomeR4C1Y;
            parameters[20].Value = HomeR4C2X;
            parameters[21].Value = HomeR4C2Y;
            parameters[22].Value = HomeR5C1X;
            parameters[23].Value = HomeR5C1Y;
            parameters[24].Value = HomeR5C2X;
            parameters[25].Value = HomeR5C2Y;
            parameters[26].Value = RecordR1Y;
            parameters[27].Value = RecordR2Y;
            parameters[28].Value = RecordR3Y;
            parameters[29].Value = RecordR4Y;
            parameters[30].Value = RecordR5Y;
            parameters[31].Value = RecordR6Y;
            parameters[32].Value = RecordR7Y;
            parameters[33].Value = RecordR8Y;
            parameters[34].Value = RecordR9Y;
            parameters[35].Value = RecordR10Y;
            parameters[36].Value = RecordR11Y;
            parameters[37].Value = RecordR12Y;
            parameters[38].Value = RecordR13Y;
            parameters[39].Value = RecordR14Y;
            parameters[40].Value = RecordR15Y;
            parameters[41].Value = RecordR16Y;
            parameters[42].Value = RecordR17Y;
            parameters[43].Value = RecordR18Y;
            parameters[44].Value = RecordR19Y;
            parameters[45].Value = RecordR20Y;
            parameters[46].Value = RecordC1X;
            parameters[47].Value = RecordC2X;
            parameters[48].Value = RecordC3X;
            parameters[49].Value = RecordC4X;
            parameters[50].Value = RecordC5X;
            parameters[51].Value = RecordC6X;
            parameters[52].Value = RecordC7X;
            parameters[53].Value = RecordC8X;
            parameters[54].Value = RecordC9X;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }

        }


        void GetPrintSetting_DepModel(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting_Dep] ";
            strSql += " where 1=1 and WBID=0" ;
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

        void UpdatePrintSetting_DepModel(HttpContext context)
        {
          
            string Width = context.Request.Form["Width"].ToString();
            string Height = context.Request.Form["Height"].ToString();
            string DriftRateX = context.Request.Form["DriftRateX"].ToString();
            string DriftRateY = context.Request.Form["DriftRateY"].ToString();
            string FontSize = context.Request.Form["FontSize"].ToString();

            string HomeR1C1X = context.Request.Form["HomeR1C1X"].ToString();
            string HomeR1C1Y = context.Request.Form["HomeR1C1Y"].ToString();
            string HomeR1C2X = context.Request.Form["HomeR1C2X"].ToString();
            string HomeR1C2Y = context.Request.Form["HomeR1C2Y"].ToString();

            string HomeR2C1X = context.Request.Form["HomeR2C1X"].ToString();
            string HomeR2C1Y = context.Request.Form["HomeR2C1Y"].ToString();
            string HomeR2C2X = context.Request.Form["HomeR2C2X"].ToString();
            string HomeR2C2Y = context.Request.Form["HomeR2C2Y"].ToString();

            string HomeR3C1X = context.Request.Form["HomeR3C1X"].ToString();
            string HomeR3C1Y = context.Request.Form["HomeR3C1Y"].ToString();
            string HomeR3C2X = context.Request.Form["HomeR3C2X"].ToString();
            string HomeR3C2Y = context.Request.Form["HomeR3C2Y"].ToString();

            string HomeR4C1X = context.Request.Form["HomeR4C1X"].ToString();
            string HomeR4C1Y = context.Request.Form["HomeR4C1Y"].ToString();
            string HomeR4C2X = context.Request.Form["HomeR4C2X"].ToString();
            string HomeR4C2Y = context.Request.Form["HomeR4C2Y"].ToString();

            string HomeR5C1X = context.Request.Form["HomeR5C1X"].ToString();
            string HomeR5C1Y = context.Request.Form["HomeR5C1Y"].ToString();
            string HomeR5C2X = context.Request.Form["HomeR5C2X"].ToString();
            string HomeR5C2Y = context.Request.Form["HomeR5C2Y"].ToString();

            string RecordR1Y = context.Request.Form["RecordR1Y"].ToString();
            string RecordR2Y = context.Request.Form["RecordR2Y"].ToString();
            string RecordR3Y = context.Request.Form["RecordR3Y"].ToString();
            string RecordR4Y = context.Request.Form["RecordR4Y"].ToString();
            string RecordR5Y = context.Request.Form["RecordR5Y"].ToString();
            string RecordR6Y = context.Request.Form["RecordR6Y"].ToString();
            string RecordR7Y = context.Request.Form["RecordR7Y"].ToString();
            string RecordR8Y = context.Request.Form["RecordR8Y"].ToString();
            string RecordR9Y = context.Request.Form["RecordR9Y"].ToString();
            string RecordR10Y = context.Request.Form["RecordR10Y"].ToString();
            string RecordR11Y = context.Request.Form["RecordR11Y"].ToString();
            string RecordR12Y = context.Request.Form["RecordR12Y"].ToString();
            string RecordR13Y = context.Request.Form["RecordR13Y"].ToString();
            string RecordR14Y = context.Request.Form["RecordR14Y"].ToString();
            string RecordR15Y = context.Request.Form["RecordR15Y"].ToString();
            string RecordR16Y = context.Request.Form["RecordR16Y"].ToString();
            string RecordR17Y = context.Request.Form["RecordR17Y"].ToString();
            string RecordR18Y = context.Request.Form["RecordR18Y"].ToString();
            string RecordR19Y = context.Request.Form["RecordR19Y"].ToString();
            string RecordR20Y = context.Request.Form["RecordR20Y"].ToString();

            string RecordC1X = context.Request.Form["RecordC1X"].ToString();
            string RecordC2X = context.Request.Form["RecordC2X"].ToString();
            string RecordC3X = context.Request.Form["RecordC3X"].ToString();
            string RecordC4X = context.Request.Form["RecordC4X"].ToString();
            string RecordC5X = context.Request.Form["RecordC5X"].ToString();
            string RecordC6X = context.Request.Form["RecordC6X"].ToString();
            string RecordC7X = context.Request.Form["RecordC7X"].ToString();
            string RecordC8X = context.Request.Form["RecordC8X"].ToString();
            string RecordC9X = context.Request.Form["RecordC9X"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [PrintSetting_Dep] set ");
          
            strSql.Append("Width=@Width,");
            strSql.Append("Height=@Height,");
            strSql.Append("DriftRateX=@DriftRateX,");
            strSql.Append("DriftRateY=@DriftRateY,");
            strSql.Append("FontSize=@FontSize,");
            strSql.Append("HomeR1C1X=@HomeR1C1X,");
            strSql.Append("HomeR1C1Y=@HomeR1C1Y,");
            strSql.Append("HomeR1C2X=@HomeR1C2X,");
            strSql.Append("HomeR1C2Y=@HomeR1C2Y,");
            strSql.Append("HomeR2C1X=@HomeR2C1X,");
            strSql.Append("HomeR2C1Y=@HomeR2C1Y,");
            strSql.Append("HomeR2C2X=@HomeR2C2X,");
            strSql.Append("HomeR2C2Y=@HomeR2C2Y,");
            strSql.Append("HomeR3C1X=@HomeR3C1X,");
            strSql.Append("HomeR3C1Y=@HomeR3C1Y,");
            strSql.Append("HomeR3C2X=@HomeR3C2X,");
            strSql.Append("HomeR3C2Y=@HomeR3C2Y,");
            strSql.Append("HomeR4C1X=@HomeR4C1X,");
            strSql.Append("HomeR4C1Y=@HomeR4C1Y,");
            strSql.Append("HomeR4C2X=@HomeR4C2X,");
            strSql.Append("HomeR4C2Y=@HomeR4C2Y,");
            strSql.Append("HomeR5C1X=@HomeR5C1X,");
            strSql.Append("HomeR5C1Y=@HomeR5C1Y,");
            strSql.Append("HomeR5C2X=@HomeR5C2X,");
            strSql.Append("HomeR5C2Y=@HomeR5C2Y,");
            strSql.Append("RecordR1Y=@RecordR1Y,");
            strSql.Append("RecordR2Y=@RecordR2Y,");
            strSql.Append("RecordR3Y=@RecordR3Y,");
            strSql.Append("RecordR4Y=@RecordR4Y,");
            strSql.Append("RecordR5Y=@RecordR5Y,");
            strSql.Append("RecordR6Y=@RecordR6Y,");
            strSql.Append("RecordR7Y=@RecordR7Y,");
            strSql.Append("RecordR8Y=@RecordR8Y,");
            strSql.Append("RecordR9Y=@RecordR9Y,");
            strSql.Append("RecordR10Y=@RecordR10Y,");
            strSql.Append("RecordR11Y=@RecordR11Y,");
            strSql.Append("RecordR12Y=@RecordR12Y,");
            strSql.Append("RecordR13Y=@RecordR13Y,");
            strSql.Append("RecordR14Y=@RecordR14Y,");
            strSql.Append("RecordR15Y=@RecordR15Y,");
            strSql.Append("RecordR16Y=@RecordR16Y,");
            strSql.Append("RecordR17Y=@RecordR17Y,");
            strSql.Append("RecordR18Y=@RecordR18Y,");
            strSql.Append("RecordR19Y=@RecordR19Y,");
            strSql.Append("RecordR20Y=@RecordR20Y,");
            strSql.Append("RecordC1X=@RecordC1X,");
            strSql.Append("RecordC2X=@RecordC2X,");
            strSql.Append("RecordC3X=@RecordC3X,");
            strSql.Append("RecordC4X=@RecordC4X,");
            strSql.Append("RecordC5X=@RecordC5X,");
            strSql.Append("RecordC6X=@RecordC6X,");
            strSql.Append("RecordC7X=@RecordC7X,");
            strSql.Append("RecordC8X=@RecordC8X,");
            strSql.Append("RecordC9X=@RecordC9X");
            string WBID = context.Session["WB_ID"].ToString();
            strSql.Append(" where WBID in (0,"+WBID+")" );

            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4),
					new SqlParameter("@Width", SqlDbType.Int,4),
					new SqlParameter("@Height", SqlDbType.Int,4),
					new SqlParameter("@DriftRateX", SqlDbType.Int,4),
					new SqlParameter("@DriftRateY", SqlDbType.Int,4),
					new SqlParameter("@FontSize", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR1C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR2C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR3C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR4C2Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C1X", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C1Y", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C2X", SqlDbType.Int,4),
					new SqlParameter("@HomeR5C2Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR1Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR2Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR3Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR4Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR5Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR6Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR7Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR8Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR9Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR10Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR11Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR12Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR13Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR14Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR15Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR16Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR17Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR18Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR19Y", SqlDbType.Int,4),
					new SqlParameter("@RecordR20Y", SqlDbType.Int,4),
					new SqlParameter("@RecordC1X", SqlDbType.Int,4),
					new SqlParameter("@RecordC2X", SqlDbType.Int,4),
					new SqlParameter("@RecordC3X", SqlDbType.Int,4),
					new SqlParameter("@RecordC4X", SqlDbType.Int,4),
					new SqlParameter("@RecordC5X", SqlDbType.Int,4),
					new SqlParameter("@RecordC6X", SqlDbType.Int,4),
					new SqlParameter("@RecordC7X", SqlDbType.Int,4),
					new SqlParameter("@RecordC8X", SqlDbType.Int,4),
					new SqlParameter("@RecordC9X", SqlDbType.Int,4)};
            parameters[0].Value = 0;
            parameters[1].Value = Width;
            parameters[2].Value = Height;
            parameters[3].Value = DriftRateX;
            parameters[4].Value = DriftRateY;
            parameters[5].Value = FontSize;
            parameters[6].Value = HomeR1C1X;
            parameters[7].Value = HomeR1C1Y;
            parameters[8].Value = HomeR1C2X;
            parameters[9].Value = HomeR1C2Y;
            parameters[10].Value = HomeR2C1X;
            parameters[11].Value = HomeR2C1Y;
            parameters[12].Value = HomeR2C2X;
            parameters[13].Value = HomeR2C2Y;
            parameters[14].Value = HomeR3C1X;
            parameters[15].Value = HomeR3C1Y;
            parameters[16].Value = HomeR3C2X;
            parameters[17].Value = HomeR3C2Y;
            parameters[18].Value = HomeR4C1X;
            parameters[19].Value = HomeR4C1Y;
            parameters[20].Value = HomeR4C2X;
            parameters[21].Value = HomeR4C2Y;
            parameters[22].Value = HomeR5C1X;
            parameters[23].Value = HomeR5C1Y;
            parameters[24].Value = HomeR5C2X;
            parameters[25].Value = HomeR5C2Y;
            parameters[26].Value = RecordR1Y;
            parameters[27].Value = RecordR2Y;
            parameters[28].Value = RecordR3Y;
            parameters[29].Value = RecordR4Y;
            parameters[30].Value = RecordR5Y;
            parameters[31].Value = RecordR6Y;
            parameters[32].Value = RecordR7Y;
            parameters[33].Value = RecordR8Y;
            parameters[34].Value = RecordR9Y;
            parameters[35].Value = RecordR10Y;
            parameters[36].Value = RecordR11Y;
            parameters[37].Value = RecordR12Y;
            parameters[38].Value = RecordR13Y;
            parameters[39].Value = RecordR14Y;
            parameters[40].Value = RecordR15Y;
            parameters[41].Value = RecordR16Y;
            parameters[42].Value = RecordR17Y;
            parameters[43].Value = RecordR18Y;
            parameters[44].Value = RecordR19Y;
            parameters[45].Value = RecordR20Y;
            parameters[46].Value = RecordC1X;
            parameters[47].Value = RecordC2X;
            parameters[48].Value = RecordC3X;
            parameters[49].Value = RecordC4X;
            parameters[50].Value = RecordC5X;
            parameters[51].Value = RecordC6X;
            parameters[52].Value = RecordC7X;
            parameters[53].Value = RecordC8X;
            parameters[54].Value = RecordC9X;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }

        }
        #endregion 

        #region 重设网店打印坐标
        /// <summary>
        /// 将网店的坐标重置为模板坐标
        /// </summary>
        /// <param name="context"></param>
        void SetWBPrintSetting(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string strSqlDelete = " DELETE FROM dbo.PrintSetting WHERE WBID="+WBID;


            //为网店初始化社员打印坐标
            StringBuilder strSqlCommune = new StringBuilder();
            strSqlCommune.Append("insert into [PrintSetting] (");
            strSqlCommune.Append("Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X)");
            strSqlCommune.Append(" select Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ");
            strSqlCommune.Append("  FROM dbo.PrintSetting WHERE WBID=0");

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDelete.ToString());
                   SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlCommune.ToString());
                   string strSqlCommuneUpdate = " UPDATE dbo.PrintSetting SET WBID=" + WBID.ToString() + " WHERE ID=(SELECT MAX(ID) FROM dbo.PrintSetting)";
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlCommuneUpdate.ToString());

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

        /// <summary>
        /// 将网店的坐标重置为模板坐标
        /// </summary>
        /// <param name="context"></param>
        void SetWBPrintSetting_Dep(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string strSqlDelete = " DELETE FROM dbo.PrintSetting_Dep WHERE WBID=" + WBID;


            //为网店初始化储户打印坐标
            StringBuilder strSqlDep = new StringBuilder();
            strSqlDep.Append("insert into [PrintSetting_Dep] (");
            strSqlDep.Append("Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X)");
            strSqlDep.Append(" select Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ");
            strSqlDep.Append("  FROM dbo.PrintSetting_Dep WHERE WBID=0");

            //添加事务处理
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {

                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDelete.ToString());
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDep.ToString());
                    string strSqlDepUpdate = " UPDATE dbo.PrintSetting_Dep SET WBID=" + WBID.ToString() + " WHERE ID=(SELECT MAX(ID) FROM dbo.PrintSetting_Dep)";
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSqlDepUpdate.ToString());


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
        #endregion 


        void ShowPrintText(HttpContext context)
        {
            string WBID = context.Session["WBID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting] ";
            strSql += " where 1=1 and WBID="+WBID;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null && dt.Rows.Count != 0)
            {

                string BusinessNo = context.Request.QueryString["BusinessNO"].ToString();
                int numBusinessNo = Convert.ToInt32(BusinessNo);
                int numIndex = 0;//确定当前打印的行的索引
                List<int> listUp = new List<int>();
                List<int> listDown = new List<int>();
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (i % 2 == 0)
                        {
                            listDown.Add(i * 10 + j + 1);
                        }
                        else {
                            listUp.Add(i * 10 + j + 1);
                        }
                    }
                }
                if (listUp.Contains(numBusinessNo))
                {
                    if (numBusinessNo % 10 == 0)
                    {
                        numIndex = 10;
                    }
                    else
                    {
                        numIndex = Convert.ToInt32(BusinessNo.Substring(BusinessNo.Length - 1));
                    }
                }
                else
                {
                    if (numBusinessNo % 10 == 0)
                    {
                        numIndex = 20;
                    }
                    else
                    {
                        numIndex = 10+Convert.ToInt32(BusinessNo.Substring(BusinessNo.Length - 1));
                    }
                }

                string strName="RecordR" + numIndex.ToString() + "Y";
                int RecordRY = Convert.ToInt32(dt.Rows[0][strName]);//当前行的Y坐标位置


                int FontSize = Convert.ToInt32(dt.Rows[0]["FontSize"]);
                int RecordC1X = Convert.ToInt32(dt.Rows[0]["RecordC1X"]);
                int RecordC2X = Convert.ToInt32(dt.Rows[0]["RecordC2X"]);
                int RecordC3X = Convert.ToInt32(dt.Rows[0]["RecordC3X"]);
                int RecordC4X = Convert.ToInt32(dt.Rows[0]["RecordC4X"]);
                int RecordC5X = Convert.ToInt32(dt.Rows[0]["RecordC5X"]);
                int RecordC6X = Convert.ToInt32(dt.Rows[0]["RecordC6X"]);
                int RecordC7X = Convert.ToInt32(dt.Rows[0]["RecordC7X"]);
                int RecordC8X = Convert.ToInt32(dt.Rows[0]["RecordC8X"]);
                int RecordC9X = Convert.ToInt32(dt.Rows[0]["RecordC9X"]);

                StringBuilder strReturn = new StringBuilder();
                strReturn.Append("  <table style='width:100%; height:" + RecordRY + "px'><tr><td></td> </tr></table>");
                strReturn.Append("   <table style='margin-left:"+RecordC1X+"px; font-size:"+FontSize+"px;'><tr>");
                strReturn.Append("   <td style='width:"+(RecordC2X-RecordC1X).ToString()+"px;'>20150115</td>");
                strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>种子</td>");
                strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>100.00</td>");
                strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>95.50</td>");
                strReturn.Append("   <td style='width:" + (RecordC6X - RecordC5X).ToString() + "px;'>200</td>");  
                strReturn.Append("   <td style='width:" + (RecordC7X - RecordC6X).ToString() + "px;'>20000</td>");
                strReturn.Append("   <td style='width:" + (RecordC8X - RecordC7X).ToString() + "px;'>12.00</td>");
                strReturn.Append("   <td style='width:" + (RecordC9X - RecordC8X).ToString() + "px;'>18000.50</td>");
                strReturn.Append("   <td >大新</td>");
              
                strReturn.Append("   </tr></table>");
                if (numIndex == 1)//当前第一个打印，改写底部数据
                {
                    int RecordR1Y = Convert.ToInt32(dt.Rows[0]["RecordR1Y"]);
                    int RecordR2Y = Convert.ToInt32(dt.Rows[0]["RecordR2Y"]);
                  int  RecordR20Y = Convert.ToInt32(dt.Rows[0]["RecordR20Y"]);//当前行的Y坐标位置
                  //strReturn.Append("  <table style='width:100%; height:" + (RecordR20Y - RecordR2Y + RecordR1Y).ToString() + "px'><tr><td></td> </tr></table>");
                  // 
                  strReturn.Append("  <table style='width:100%; height:" + RecordR20Y + "px'><tr><td></td> </tr></table>");
                  strReturn.Append("   <table style='margin-left:" + RecordC1X + "px; font-size:14px; font-weight:bolder'><tr>");
                  strReturn.Append("   <td style='width:" + (RecordC2X - RecordC1X).ToString() + "px;'></td>");
                  strReturn.Append("   <td style='width:" + (RecordC3X - RecordC2X).ToString() + "px;'>品种</td>");
                  strReturn.Append("   <td style='width:" + (RecordC4X - RecordC3X).ToString() + "px;'>市场价</td>");
                  strReturn.Append("   <td style='width:" + (RecordC5X - RecordC4X).ToString() + "px;'>社员价</td>");
                  strReturn.Append("   </tr></table>");
                }


                context.Response.Write(strReturn.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void ShowCommuneInfo(HttpContext context)
        {
            string WBID = context.Session["WB_ID"].ToString();
            string strSql = " SELECT TOP 1 ID,Width,Height,DriftRateX,DriftRateY,FontSize,HomeR1C1X,HomeR1C1Y,HomeR1C2X,HomeR1C2Y,HomeR2C1X,HomeR2C1Y,HomeR2C2X,HomeR2C2Y,HomeR3C1X,HomeR3C1Y,HomeR3C2X,HomeR3C2Y,HomeR4C1X,HomeR4C1Y,HomeR4C2X,HomeR4C2Y,HomeR5C1X,HomeR5C1Y,HomeR5C2X,HomeR5C2Y,RecordR1Y,RecordR2Y,RecordR3Y,RecordR4Y,RecordR5Y,RecordR6Y,RecordR7Y,RecordR8Y,RecordR9Y,RecordR10Y,RecordR11Y,RecordR12Y,RecordR13Y,RecordR14Y,RecordR15Y,RecordR16Y,RecordR17Y,RecordR18Y,RecordR19Y,RecordR20Y,RecordC1X,RecordC2X,RecordC3X,RecordC4X,RecordC5X,RecordC6X,RecordC7X,RecordC8X,RecordC9X ";
            strSql += "   FROM [PrintSetting] ";
            strSql += " where 1=1 and WBID="+WBID;
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
                strReturn.Append("   <td style='font-size:20px; font-weight:bolder; width:" + (Convert.ToInt32(HomeR1C2X) - Convert.ToInt32(HomeR1C1X)).ToString() + "px;'>0010000001</td>");
                strReturn.Append("   <td style='font-size:20px; font-weight:bolder;'>刘玉溪</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32( HomeR3C1Y)-Convert.ToInt32(HomeR2C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR2C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR2C2X) - Convert.ToInt32(HomeR2C1X)).ToString() + "px;'>闵行 东川路</td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;'>485478545125698547</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR4C1Y) - Convert.ToInt32(HomeR3C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR3C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR3C2X) - Convert.ToInt32(HomeR3C1X)).ToString() + "px;'>第一网店</td>");
                strReturn.Append("  <td style='font-size:14px; font-weight:bold;'>15687458954</td>");
                strReturn.Append("   </tr></table>");

                strReturn.Append("  <table style='height:" + (Convert.ToInt32(HomeR5C1Y) - Convert.ToInt32(HomeR4C1Y)).ToString() + "px'><tr>");
                strReturn.Append("   <td style='width:" + HomeR4C1X + "px;'></td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;width:" + (Convert.ToInt32(HomeR4C2X) - Convert.ToInt32(HomeR4C1X)).ToString() + "px;'>2015-05-06</td>");
                strReturn.Append("   <td style='font-size:14px; font-weight:bold;' >100</td>");

                strReturn.Append("   </tr></table>");
               

                context.Response.Write(strReturn.ToString());
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