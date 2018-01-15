using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.Caching;
using System.Web;
using System.Web.SessionState;

namespace Web
{
    public class common
    {
        public static double numTotol = 0;//保存储户的本金和利息总和

        /// <summary>
        /// 判断Session超时
        /// </summary>
        public static void IsLogin()
        {
            object um = HttpContext.Current.Session["ID"];// 查询ID的Session值是否存在
            object cook = HttpContext.Current.Request.Cookies["LoginInfo"];
            bool ok = false;
            try
            {
                if (um != null)
                {
                    ok = true;
                }
                else//Cookie重新写入Session  
                {
                    if (HttpContext.Current.Request.Cookies["LoginInfo"] != null)
                    {

                        int User_ID = int.Parse(HttpContext.Current.Request.Cookies["LoginInfo"]["ID"].ToString());
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append(" SELECT ID, UserGroup_ID,WB_ID,strRealName,strLoginName");
                        strSql.Append(" FROM dbo.Users");
                        strSql.Append(" WHERE ID=@ID");
                        SqlParameter parameter = new SqlParameter("@ID", System.Data.SqlDbType.Int, 4);
                        parameter.Value = User_ID.ToString();
                        System.Data.DataTable dtAccount = SQLHelper.ExecuteDataTable(strSql.ToString(), parameter);
                        if (dtAccount != null && dtAccount.Rows.Count != 0)
                        {
                            HttpContext.Current.Session.Clear();
                            HttpContext.Current.Session["WB_ID"] = dtAccount.Rows[0]["WB_ID"].ToString();//该用户所在的网点ID
                            HttpContext.Current.Session["UserGroup_ID"] = dtAccount.Rows[0]["UserGroup_ID"].ToString();//该用户所在的用户组ID
                            string UserGroupName = SQLHelper.ExecuteScalar("  SELECT strName FROM dbo.UserGroup WHERE ID=" + dtAccount.Rows[0]["UserGroup_ID"].ToString()).ToString();
                            HttpContext.Current.Session["UserGroup_Name"] = UserGroupName;
                            HttpContext.Current.Session["ID"] = dtAccount.Rows[0]["ID"].ToString();//用户ID
                            HttpContext.Current.Session["strLoginName"] = dtAccount.Rows[0]["strLoginName"].ToString();//用户登录名
                            HttpContext.Current.Session["strRealName"] = dtAccount.Rows[0]["strRealName"].ToString();

                            DataTable dtwb = SQLHelper.ExecuteDataTable(" select * from WB where ID=" + dtAccount.Rows[0]["WB_ID"].ToString());

                            HttpContext.Current.Session["ISHQ"] = dtwb.Rows[0]["ISHQ"].ToString();
                            HttpContext.Current.Session["ISSimulate"] = dtwb.Rows[0]["ISSimulate"].ToString();
                        }

                        ok = true;
                    }
                    else
                    {
                        ok = false;
                    }
                }
            }
            catch (SystemException ex)
            {
                // HyDebug.WriteToDoc("110:" + ex.Message);//HyDebug可以用直接写入文本的方式在网站发布之后查看我们的调试信息
                ok = false;
            }
            if (ok == false)
            {

                HttpContext.Current.Response.Write("<script>top.window.location.href='/index.html';</script>");
                HttpContext.Current.Response.End();
            }

        }

        /// <summary>
        /// 查询用户的菜单权限
        /// </summary>
        /// <param name="UserGroupName">角色组名称</param>
        /// <param name="UserID">用户编号</param>
        /// <param name="MenuID">菜单编号</param>
        /// <param name="MenuType">菜单类型</param>
        /// <returns></returns>
        public static bool GetAuthority(object UserGroupName, object UserID, object MenuID, object MenuType)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT TOP 1 menuList FROM dbo.UserMenu WHERE 1=1");
            if (UserGroupName.ToString() == "单位管理员")
            {
                return true;
            }
            if (UserGroupName.ToString() == "营业员")
            {
                strSql.Append(" and numtype=2 ");
            }
            else
            {
                strSql.Append(" and numtype=1 ");
            }
            strSql.Append(" and UserID=" + UserID.ToString());
            object objmenu = SQLHelper.ExecuteScalar(strSql.ToString());
            if (objmenu == null || objmenu.ToString() == "")
            {
                return false;
            }
            Dictionary<string, string> dicmenu = new Dictionary<string, string>();//key：菜单ID，value：菜单值
            string[] menuArray = objmenu.ToString().Split('|');
            for (int i = 0; i < menuArray.Length; i++)
            {
                string[] menuArrayChild = menuArray[i].Split(':');
                dicmenu.Add(menuArrayChild[0], menuArrayChild[1]);
            }
            if (MenuID == null) { return false; }
            if (dicmenu.ContainsKey(MenuID.ToString()))
            {
                string strValue = dicmenu[MenuID.ToString()];
                if (strValue.Contains(MenuType.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }



        }

        //计算产品利息和拼接返回信息
        public static Dictionary<string, string> FunJiSuan(HttpContext context, string Dep_SID, double StorageNumber)
        {
            // string strReturn = "";//返回的json信息
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string strMsg = "";//显示信息
            string model = "";
            if (context.Request.QueryString["model"] != null)
            {
                model = context.Request.QueryString["model"].ToString();
            }
            //计算利息
            double Interest = 0;
            if (model.ToLower() == "shopping")
            {
                Interest = common.GetLiXi(Dep_SID, StorageNumber);//利息
            }
            else
            {
                Interest = common.GetLiXi_Sell(Dep_SID, StorageNumber);//利息
            }
            double MoneyFee = 0; //保管费
            double MoneyVariety = 0;//产品总金额


            bool ISRegular = true;//是否是定期产品
            string TimeName = "";//存期类型
            string numStorageDate = "";//约定存储天数

            DateTime StorageDate = DateTime.Now;//存入日期
            TimeSpan ts = DateTime.Now.Subtract(StorageDate);
            int PricePolicy = 0;//约定存储时间
            double Price_ShiChang = 0;//市场价
            double StorageFee = 0;//保管费率

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select A.StorageDate,B.PricePolicy,B.numStorageDate, B.ISRegular,B.strName as TimeName, A.StorageFee,A.Price_ShiChang");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);
                TimeName = dt.Rows[0]["TimeName"].ToString();
                numStorageDate = dt.Rows[0]["numStorageDate"].ToString();

                StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                ts = DateTime.Now.Subtract(StorageDate);
                PricePolicy = Convert.ToInt32(dt.Rows[0]["PricePolicy"]);//约定存储时间
                Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//市场价
                StorageFee = Convert.ToDouble(dt.Rows[0]["StorageFee"]);//保管费率

                MoneyVariety = StorageNumber * Price_ShiChang;//产品总金额
                if (context.Request.QueryString["model"] == null)//如果是存转销则计算保管费，如果是产品换购则不计算保管费
                {
                    //计算保管费
                    if (ts.TotalDays < Convert.ToDouble(PricePolicy))//不到价格政策的日期
                    {
                        MoneyFee = MoneyVariety * Convert.ToDouble(StorageFee) * 0.01;
                    }
                }
            }
            double numMoney = MoneyVariety + Interest - MoneyFee;//计算得出的存转销总金额


            if (Convert.ToBoolean(ISRegular) == true)
            {
                strMsg += "储户类型：<span style='color:blue'>定期,</span>";
            }
            else
            {
                strMsg += "储户类型：<span style='color:blue'>活期,</span>";
            }
            strMsg += "存期：<span style='color:blue'>" + TimeName + ",</span>";
            if (Convert.ToBoolean(ISRegular))
            {
                strMsg += "约定存储时间：<span style='color:blue'>" + numStorageDate + "天,</span>";
                strMsg += "实际存储时间：<span style='color:blue'>" + Convert.ToInt32(Math.Round(ts.TotalDays, 0)).ToString() + "天,</span>";
            }
            strMsg += "产品金额=<span style='color:blue'>" + Math.Round(StorageNumber, 2) + "x" + Math.Round(Price_ShiChang, 2).ToString() + "=" + Math.Round(MoneyVariety, 2) + "元,</span>";
            strMsg += "存储利息：<span style='color:blue'>" + Math.Round(Interest, 2) + "元,</span>";
            if (context.Request.QueryString["model"] == null)
            {
                strMsg += "保管费：<span style='color:blue'>" + Math.Round(MoneyFee, 2) + "元,</span>";
            }
            strMsg += "折合现金：<span style='color:blue'>" + Math.Round(numMoney, 2) + "元</span>";

            dic.Add("Count", StorageNumber.ToString());
            dic.Add("Money", MoneyVariety.ToString());
            dic.Add("LiXi", Interest.ToString());
            dic.Add("BGF", MoneyFee.ToString());
            dic.Add("Msg", strMsg);


            return dic;
        }

        #region 价格政策

        /// <summary>
        /// 获取固定数量的产品的利息和金额总和
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="StorageNumber"></param>
        /// <returns></returns>
        public static string GetExPolicy(object obj, double StorageNumber, double Exchange_trading)
        {
            string strReturn = "";

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.ISRegular,B.InterestType,A.StorageNumber,A.Price_ShiChang,A.StorageDate,B.numStorageDate,B.strName as StorageName");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + obj.ToString());

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                bool ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);//是否是定期类型
                int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价

                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                string StorageName = dt.Rows[0]["StorageName"].ToString();
                switch (InterestType)
                {
                    case 1://按月结息方式
                        strReturn = GetPolicy_FuXi(obj.ToString(), StorageNumber, StorageName);
                        break;
                    case 2://按市场价结息方式
                        strReturn = GetPolicy_FenHong(obj.ToString(), StorageNumber, StorageName);
                        break;
                    case 3:
                        strReturn = GetExPolicy_DingQi(obj.ToString(), StorageNumber, Exchange_trading);
                        break;
                    case 4:
                        strReturn = GetPolicy_RuGu(obj.ToString(), StorageNumber, StorageName);
                        break;
                }

            }

            return strReturn;
        }

        /// <summary>
        /// 获取定期类型兑换时的兑换政策
        /// </summary>
        /// <param name="Dep_SID"></param>
        /// <param name="numMoney"></param>
        /// <returns></returns>
        public static string GetExPolicy_DingQi(string Dep_SID, double numMoney, double Exchange_trading)
        {
            string strReturn = "当前产品存期类型：定期,";//返回信息
            double VarietyCount = 0;//需要折合的原粮的数量
            double Money_YouHui = 0;//定期优惠的金额
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价

                VarietyCount = numMoney / Price_ShiChang;//暂时设定折合原粮数量按照存入价格计算

                if (ts_Storage.TotalDays < numStorageDate)//定期存期未到期的情况
                {
                    strReturn += "未到期,";
                    //获取每个月的优惠兑换比例
                    StringBuilder strSqlProp = new StringBuilder();
                    strSqlProp.Append("  SELECT TOP 1 A.StorageNumberRaw,B.numExChangeProp");
                    strSqlProp.Append("   FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
                    strSqlProp.Append("    WHERE A.ID=" + Dep_SID);
                    DataTable dtProp = SQLHelper.ExecuteDataTable(strSqlProp.ToString());
                    if (dtProp == null || dtProp.Rows.Count == 0)//没有查询到优惠兑换比例
                    {
                        strReturn += "存入价：" + Price_ShiChang + ",无优惠兑换比例!";
                        return strReturn;
                    }
                    double JieCun_Raw = Convert.ToDouble(dtProp.Rows[0]["StorageNumberRaw"]);
                    //double JieCun_Total = Convert.ToDouble(dt.Rows[0]["JieCun_Total"]);//已经发生的结存
                    double numExChangeProp = Convert.ToDouble(dtProp.Rows[0]["numExChangeProp"]);

                    double JieCun_Total = GetMonthJieCun_Total(Dep_SID);//已经发生的结存
                    JieCun_Total = JieCun_Total + Exchange_trading;//实际的结存数量为已经发生的结存数量和已经准备结存的数量
                    //查询本月已经发生的结存
                    if (numExChangeProp <= 0)
                    {
                        strReturn += "存入价：" + Price_ShiChang + ",无优惠兑换比例!";
                        return strReturn;
                    }
                    double Exchange_Count = JieCun_Raw * numExChangeProp * 0.01;//最多允许兑换的数量
                    double SurPlue_Count = Exchange_Count - JieCun_Total;//剩余的可以优惠兑换的数量

                    if (JieCun_Total > Exchange_Count)
                    {
                        JieCun_Total = Exchange_Count;
                        SurPlue_Count = 0;
                    }

                    strReturn += "存入价：" + Price_ShiChang + ",到期价：" + Price_DaoQi + ",本月已优惠兑换数量：" + Math.Round(JieCun_Total, 2) + ",剩余可优惠兑换数量：" + Math.Round(SurPlue_Count, 2);
                    return strReturn;
                }
                else
                {
                    strReturn += "已到期,到期价：" + Price_DaoQi ;

                }
            }
            return strReturn;
        }

        /// <summary>
        /// 计算利息 活期付息类型
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static string GetPolicy_FuXi(string Dep_SID, double StorageNumber, string StorageName)
        {
            string strReturn = "当前产品存期类型：" + StorageName;//返回信息

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {


                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//约定月利率
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价


                strReturn += ",存入价：" + Price_ShiChang + "，月利率：" + CurrentRate;
            }
            return strReturn;
        }


        /// <summary>
        /// 计算利息 活期 分红类型
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static string GetPolicy_FenHong(string Dep_SID, double StorageNumber, string StorageName)
        {
            string strReturn = "当前产品存期类型：" + StorageName;//返回信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {


                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//约定月利率
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价


                if (ts_Storage.TotalDays < numStorageDate)//分红类型未到约定的存期
                {
                    strReturn += "未到期，存入价：" + Price_ShiChang;
                }
                else
                {
                    //计算当前的市场价格
                    StringBuilder strSqlShiChang = new StringBuilder();
                    strSqlShiChang.Append(" SELECT B.Price_ShiChang,B.EarningRate");
                    strSqlShiChang.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageRate B ON A.StorageRateID=B.ID");
                    strSqlShiChang.Append(" WHERE A.ID=" + Dep_SID);
                    DataTable dtShiChang = SQLHelper.ExecuteDataTable(strSqlShiChang.ToString());
                    if (dtShiChang != null && dtShiChang.Rows.Count != 0)
                    {

                        strReturn += "已到期，存入价：" + Price_ShiChang + ",当前市场价：" + dtShiChang.Rows[0]["Price_ShiChang"].ToString();
                    }
                }




            }
            return strReturn;
        }

        /// <summary>
        /// 计算利息 定期类型(入股)
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static string GetPolicy_RuGu(string Dep_SID, double StorageNumber, string StorageName)
        {
            string strReturn = "当前产品存期类型：" + StorageName;//返回信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,A.Price_HeTong,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {


                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//约定月利率
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价
                double Price_HeTong = Convert.ToDouble(dt.Rows[0]["Price_HeTong"]);//到期价
                if (ts_Storage.TotalDays < numStorageDate)//入股类型未到约定的存期
                {
                    strReturn += "，未到期，存入价：" + Price_ShiChang;
                }
                else {
                    strReturn += "，已到期，合同价：" + Price_HeTong;
                }
               
            }
            return strReturn;
        }
        #endregion

        #region 页面显示的利息计算

        public static Dictionary<string, string> GetLiXi_html(object obj)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string strReturn = "";
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.ISRegular,B.InterestType,A.StorageNumber,A.Price_ShiChang,A.StorageDate,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + obj.ToString());

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                bool ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);//是否是定期类型
                int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
                double StorageNumber = Convert.ToDouble(dt.Rows[0]["StorageNumber"]);//存储数量
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价

                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间

                switch (InterestType)
                {
                    case 1://按月结息方式
                        numLiXi = GetLiXi_FuXi(obj.ToString(), StorageNumber);
                        strReturn = "<span style=\"color:Blue;\">￥" + Math.Round(numLiXi, 2).ToString() + "</span>";
                        break;
                    case 2://按市场价结息方式
                        numLiXi = GetLiXi_FenHong_html(obj.ToString(), StorageNumber);
                        if (ts_Storage.TotalDays >= numStorageDate)
                        {
                            strReturn = "<span style=\"color:Blue;\">￥" + Math.Round(numLiXi, 2).ToString() + "</span>";
                        }
                        else
                        {
                            strReturn = "<span style=\"font-size:12px;color:Red\">￥" + Math.Round(numLiXi, 2).ToString() + "(未到期)</span>";
                        }
                        break;
                    case 3:
                        numLiXi = GetLiXi_DingQi_html(obj.ToString(), StorageNumber);
                        if (ts_Storage.TotalDays >= numStorageDate)
                        {
                            strReturn = "<span style=\"color:Blue;\">￥" + Math.Round(numLiXi, 2).ToString() + "</span>";
                        }
                        else
                        {
                            strReturn = "<span style=\"font-size:12px;color:Red\">￥" + Math.Round(numLiXi, 2).ToString() + "(未到期)</span>";
                        }
                        break;
                    case 4://入股
                        numLiXi = GetLiXi_RuGu_html(obj.ToString(), StorageNumber);
                        if (ts_Storage.TotalDays >= numStorageDate)
                        {
                            strReturn = "<span style=\"color:Blue;\">￥" + Math.Round(numLiXi, 2).ToString() + "</span>";
                        }
                        else
                        {
                            strReturn = "<span style=\"font-size:12px;color:Red\">￥" + Math.Round(numLiXi, 2).ToString() + "(未到期)</span>";
                        }
                        break;
                }

            }
            dic.Add("strLixi", strReturn);
            dic.Add("numLixi", numLiXi.ToString());
            return dic;
        }


        /// <summary>
        /// 计算利息 活期付息类型
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi_FuXi(string Dep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {


                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//约定月利率
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价
                //获取利息计算方式
                int InterestDay = Convert.ToInt32(ts_Interest.TotalDays);//可以计算利息的总天数
                int InterestType = Convert.ToInt32(GetWBAuthority()["InterestType"]);//1：按天结息  2：按月结息
                if (InterestType == 2)
                {
                    InterestDay = InterestDay - InterestDay % 30;
                }

                numLiXi = StorageNumber * CurrentRate * InterestDay / (double)30;
            }
            return numLiXi;
        }


        /// <summary>
        /// 计算利息 活期 分红类型
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi_FenHong(string Dep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {


                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//约定月利率
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价


                if (ts_Storage.TotalDays < numStorageDate)//分红类型未到约定的存期
                {
                    numLiXi = 0;
                }
                else
                {
                    //计算当前的市场价格
                    StringBuilder strSqlShiChang = new StringBuilder();
                    strSqlShiChang.Append(" SELECT B.Price_ShiChang,B.EarningRate,B.LossRate");
                    strSqlShiChang.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageRate B ON A.StorageRateID=B.ID");
                    strSqlShiChang.Append(" WHERE A.ID=" + Dep_SID);
                    DataTable dtShiChang = SQLHelper.ExecuteDataTable(strSqlShiChang.ToString());
                    if (dtShiChang != null && dtShiChang.Rows.Count != 0)
                    {
                        double Price_JieCun = Convert.ToDouble(dtShiChang.Rows[0]["Price_ShiChang"]);
                        double EarningRate = Convert.ToDouble(dtShiChang.Rows[0]["EarningRate"]);
                        double LossRate = Convert.ToDouble(dtShiChang.Rows[0]["LossRate"]);
                        if (Price_JieCun >= Price_ShiChang)//到期的结存价格比现在的市场价高(存储产品盈利的时候 按照分红比例分红)
                        {
                            numLiXi = (Price_JieCun - Price_ShiChang) * StorageNumber * EarningRate / (double)100;
                        }
                        else//存储产品价格降低的时候由储户承担全部损耗
                        {
                            numLiXi = (Price_JieCun - Price_ShiChang) * StorageNumber * LossRate / (double)100;
                        }
                    }
                }

            }
            return numLiXi;
        }

        /// <summary>
        /// 计算利息 活期 分红类型
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi_FenHong_html(string Dep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//约定月利率
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价

                //计算当前的市场价格
                StringBuilder strSqlShiChang = new StringBuilder();
                strSqlShiChang.Append(" SELECT B.Price_ShiChang,B.EarningRate,B.LossRate");
                strSqlShiChang.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageRate B ON A.StorageRateID=B.ID");
                strSqlShiChang.Append(" WHERE A.ID=" + Dep_SID);
                DataTable dtShiChang = SQLHelper.ExecuteDataTable(strSqlShiChang.ToString());
                if (dtShiChang != null && dtShiChang.Rows.Count != 0)
                {
                    double Price_JieCun = Convert.ToDouble(dtShiChang.Rows[0]["Price_ShiChang"]);
                    double EarningRate = Convert.ToDouble(dtShiChang.Rows[0]["EarningRate"]);
                    double LossRate = Convert.ToDouble(dtShiChang.Rows[0]["LossRate"]);
                    if (Price_JieCun >= Price_ShiChang)//到期的结存价格比现在的市场价高(存储产品盈利的时候 按照分红比例分红)
                    {
                        numLiXi = (Price_JieCun - Price_ShiChang) * StorageNumber * EarningRate / (double)100;
                    }
                    else//存储产品价格降低的时候由储户承担全部损耗
                    {
                        numLiXi = (Price_JieCun - Price_ShiChang) * StorageNumber * LossRate / (double)100;
                    }
                }
            }
            return numLiXi;
        }

        /// <summary>
        /// 计算利息 定期类型(1年 2年)
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi_DingQi(string Dep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {


                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价
                if (ts_Storage.TotalDays < numStorageDate)//定期存期未到期的情况
                {
                    //add20170419 查看是否可以按照活期计算利息
                    int ISCurrentCal = Convert.ToInt32(GetWBAuthority()["ISCurrentCal"]);
                    if (ISCurrentCal == 1)
                    {
                        //活期利息的计算方式
                        double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//存入时约定的活期利率
                        TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                        int InterestDay = Convert.ToInt32(ts_Interest.TotalDays);//可以计算利息的总天数
                        int InterestType = Convert.ToInt32(GetWBAuthority()["InterestType"]);//1：按天结息  2：按月结息
                        if (InterestType == 2)
                        {
                            InterestDay = InterestDay - InterestDay % 30;
                        }

                        numLiXi = StorageNumber * CurrentRate * InterestDay / (double)30;
                    }
                    else
                    {
                        //end add
                        numLiXi = 0;
                    }
                }
                else
                {
                    numLiXi = (Price_DaoQi - Price_ShiChang) * StorageNumber;
                }

            }
            return numLiXi;
        }

        /// <summary>
        /// 计算利息 定期类型(1年 2年)
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi_DingQi_html(string Dep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价

                int ISCurrentCal = Convert.ToInt32(GetWBAuthority()["ISCurrentCal"]);
                if (ISCurrentCal == 1)
                {
                    //活期利息的计算方式
                    double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//存入时约定的活期利率
                    TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                    int InterestDay = Convert.ToInt32(ts_Interest.TotalDays);//可以计算利息的总天数
                    int InterestType = Convert.ToInt32(GetWBAuthority()["InterestType"]);//1：按天结息  2：按月结息
                    if (InterestType == 2)
                    {
                        InterestDay = InterestDay - InterestDay % 30;
                    }

                    numLiXi = StorageNumber * CurrentRate * InterestDay / (double)30;
                }
                else
                {
                    numLiXi = (Price_DaoQi - Price_ShiChang) * StorageNumber;
                }
            }
            return numLiXi;
        }

        /// <summary>
        /// 计算利息 定期类型(入股)
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi_RuGu(string Dep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,A.Price_HeTong,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {


                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//约定月利率
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_HeTong = Convert.ToDouble(dt.Rows[0]["Price_HeTong"]);//到期价

                ////获取利息计算方式
                //int InterestDay = Convert.ToInt32(ts_Interest.TotalDays);//可以计算利息的总天数
                //int InterestType = Convert.ToInt32(GetWBAuthority()["InterestType"]);//1：按天结息  2：按月结息
                //if (InterestType == 2)
                //{
                //    InterestDay = InterestDay - InterestDay % 30;
                //}

                //numLiXi = StorageNumber * CurrentRate * InterestDay / (double)30;

                if (ts_Storage.TotalDays < numStorageDate)//定期存期未到期的情况
                {
                    numLiXi = 0;
                }
                else
                {
                    numLiXi = (Price_HeTong - Price_ShiChang) * StorageNumber;
                }

            }
            return numLiXi;
        }

        /// <summary>
        /// 计算利息 定期类型(入股)
        /// </summary>
        /// <param name="Dep_SID">储户存储单编号</param>
        /// <param name="StorageNumber">用于计算利息的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi_RuGu_html(string Dep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,A.Price_HeTong,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {


                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                TimeSpan ts_Interest = DateTime.Now.Subtract(InterestDate);
                double CurrentRate = Convert.ToDouble(dt.Rows[0]["CurrentRate"]);//约定月利率
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_HeTong = Convert.ToDouble(dt.Rows[0]["Price_HeTong"]);//到期价
                //if (ts_Storage.TotalDays < numStorageDate)//定期存期未到期的情况
                //{
                //    numLiXi = 0;
                //}
                //else
                //{
                //    numLiXi = (Price_HeTong - Price_ShiChang) * StorageNumber;
                //}
                numLiXi = (Price_HeTong - Price_ShiChang) * StorageNumber;

            }
            return numLiXi;
        }

        #endregion

        #region 存转销售利息计算和反算
        /// <summary>
        /// 获取村转销产生的利息
        /// </summary>
        /// <param name="objDep_SID">储户的存贮编号</param>
        /// <param name="StorageNumber">储户的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi_Sell(object objDep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.ISRegular,B.InterestType,A.StorageNumber,A.Price_ShiChang,A.StorageDate,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + objDep_SID.ToString());

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            //Add 0825 计算到存转销模模式的计算利息时间
            DateTime dtStorageTime = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);
            // int numLink = Convert.ToInt32(GetCompanyInfo()["strLink"]);
            int numLink = Convert.ToInt32(GetWBAuthority()["numMinDay"]);
            //如何没到设置的存储期限，则在存储转销时不计算利息
            if (DateTime.Now.Subtract(dtStorageTime).TotalDays < numLink)
            {
                return 0;
            }
            //end Add
            if (dt != null && dt.Rows.Count != 0)
            {
                bool ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);//是否是定期类型
                int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
                double Price_ShiChang = Convert.ToInt32(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价

                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间

                switch (InterestType)
                {
                    case 1://按月结息方式
                        numLiXi = GetLiXi_FuXi(objDep_SID.ToString(), StorageNumber);
                        break;
                    case 2://按市场价结息方式
                        numLiXi = GetLiXi_FenHong(objDep_SID.ToString(), StorageNumber);
                        break;
                    case 3:
                        numLiXi = GetLiXi_DingQi(objDep_SID.ToString(), StorageNumber);
                        break;
                    case 4:
                        numLiXi = GetLiXi_RuGu(objDep_SID.ToString(), StorageNumber);
                        break;
                }

            }

            return numLiXi;
        }
        #endregion


        #region 兑换利息计算和反算

        /// <summary>
        /// 获取利息
        /// </summary>
        /// <param name="objDep_SID">储户的存贮编号</param>
        /// <param name="StorageNumber">储户的存储数量</param>
        /// <returns></returns>
        public static double GetLiXi(object objDep_SID, double StorageNumber)
        {
            double numLiXi = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.ISRegular,B.InterestType,A.StorageNumber,A.Price_ShiChang,A.StorageDate,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + objDep_SID.ToString());

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                bool ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);//是否是定期类型
                int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
                double Price_ShiChang = Convert.ToInt32(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价

                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间

                switch (InterestType)
                {
                    case 1://按月结息方式
                        numLiXi = GetLiXi_FuXi(objDep_SID.ToString(), StorageNumber);
                        break;
                    case 2://按市场价结息方式
                        numLiXi = GetLiXi_FenHong(objDep_SID.ToString(), StorageNumber);
                        break;
                    case 3:
                        numLiXi = GetLiXi_DingQi(objDep_SID.ToString(), StorageNumber);
                        break;
                    case 4:
                        numLiXi = GetLiXi_RuGu(objDep_SID.ToString(), StorageNumber);
                        break;
                }

            }

            return numLiXi;
        }

        /// <summary>
        /// 获取兑换时需要折合的原粮数量
        /// </summary>
        /// <param name="Dep_SID"></param>
        /// <param name="numMoney"></param>
        /// <returns></returns>
        public static double GetExVarietyCount(string Dep_SID, double numMoney)
        {
            double VarietyMark = 100;//标定的产品数量
            double MoneyMark = GetMoneyMark(Dep_SID, VarietyMark);
            double VarietyCount = (VarietyMark * numMoney) / MoneyMark;
            double Money_LiXi = GetLiXi(Dep_SID, VarietyCount);
            HttpContext.Current.Session["Ex_LiXi"] = Money_LiXi;//缓存兑换时产生的利息金额
            return VarietyCount;
        }

        /// <summary>
        /// 获取定期类型兑换时需要折合的原粮数量
        /// </summary>
        /// <param name="Dep_SID"></param>
        /// <param name="numMoney"></param>
        /// <returns></returns>
        public static double GetExVarietyCount_DingQi(string Dep_SID, double numMoney, double Exchange_trading)
        {
            double VarietyCount = 0;//需要折合的原粮的数量
            double Money_YouHui = 0;//定期优惠的金额
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价

                VarietyCount = numMoney / Price_ShiChang;//暂时设定折合原粮数量按照存入价格计算

                if (ts_Storage.TotalDays < numStorageDate)//定期存期未到期的情况
                {
                    //获取每个月的优惠兑换比例
                    StringBuilder strSqlProp = new StringBuilder();
                    strSqlProp.Append("  SELECT TOP 1 A.StorageNumberRaw,B.numExChangeProp");
                    strSqlProp.Append("   FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
                    strSqlProp.Append("    WHERE A.ID=" + Dep_SID);
                    DataTable dtProp = SQLHelper.ExecuteDataTable(strSqlProp.ToString());
                    if (dtProp == null || dtProp.Rows.Count == 0)//没有查询到优惠兑换比例
                    {
                        return VarietyCount;
                    }
                    double JieCun_Raw = Convert.ToDouble(dtProp.Rows[0]["StorageNumberRaw"]);
                    //double JieCun_Total = Convert.ToDouble(dt.Rows[0]["JieCun_Total"]);//已经发生的结存
                    double numExChangeProp = Convert.ToDouble(dtProp.Rows[0]["numExChangeProp"]);
                    double JieCun_Total = GetMonthJieCun_Total(Dep_SID);//已经发生的结存
                    JieCun_Total = JieCun_Total + Exchange_trading;
                    //查询本月已经发生的结存
                    if (numExChangeProp <= 0)
                    {
                        return VarietyCount;//兑换利率为0；
                    }
                    double Exchange_Count = JieCun_Raw * numExChangeProp * 0.01;//最多允许兑换的数量
                    double SurPlue_Count = Exchange_Count - JieCun_Total;//剩余的可以优惠兑换的数量
                    if (SurPlue_Count <= 0)//优惠兑换比例已经用完
                    {
                        return VarietyCount;
                    }
                    if (SurPlue_Count * Price_DaoQi >= numMoney)//可优惠兑换数量大于兑换数量
                    {
                        VarietyCount = numMoney / Price_DaoQi;
                        Money_YouHui = (Price_DaoQi - Price_ShiChang) * VarietyCount;
                        HttpContext.Current.Session["Ex_YouHui"] = Money_YouHui;//缓存优惠金额
                        HttpContext.Current.Session["Ex_YouHui_Count"] = VarietyCount;//缓存此次定期优惠的数额
                    }
                    else
                    {
                        HttpContext.Current.Session["Ex_YouHui_Count"] = SurPlue_Count;//缓存此次定期优惠的数额
                        Money_YouHui = (Price_DaoQi - Price_ShiChang) * SurPlue_Count;
                        HttpContext.Current.Session["Ex_YouHui"] = Money_YouHui;//缓存优惠金额
                        VarietyCount = SurPlue_Count + (numMoney - SurPlue_Count * Price_DaoQi) / Price_ShiChang;
                    }

                }
                else
                {
                    VarietyCount = numMoney / Price_DaoQi;
                }
            }
            return VarietyCount;
        }


        /// <summary>
        /// 获取固定数量的产品的利息和金额总和
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="StorageNumber"></param>
        /// <returns></returns>
        public static double GetMoneyMark(object obj, double StorageNumber)
        {
            double numLiXi = 0;
            double MoneyMark = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select B.ISRegular,B.InterestType,A.StorageNumber,A.Price_ShiChang,A.StorageDate,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + obj.ToString());

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                bool ISRegular = Convert.ToBoolean(dt.Rows[0]["ISRegular"]);//是否是定期类型
                int InterestType = Convert.ToInt32(dt.Rows[0]["InterestType"]);//利息计算方式
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价

                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                MoneyMark = StorageNumber * Price_ShiChang;
                switch (InterestType)
                {
                    case 1://按月结息方式
                        numLiXi = GetLiXi_FuXi(obj.ToString(), StorageNumber);
                        break;
                    case 2://按市场价结息方式
                        numLiXi = GetLiXi_FenHong(obj.ToString(), StorageNumber);
                        break;
                    case 3:
                        numLiXi = GetLiXi_DingQi(obj.ToString(), StorageNumber);
                        break;
                    case 4:
                        numLiXi = GetLiXi_RuGu(obj.ToString(), StorageNumber);
                        break;
                }

            }

            return numLiXi + MoneyMark;
        }


        /// <summary>
        /// 获取每个月的结存总额
        /// </summary>
        /// <param name="Dep_SID"></param>
        /// <returns></returns>
        public  static double GetMonthJieCun_Total(string Dep_SID)
        {
            //string strYear=DateTime.Now.Year.ToString();
            //string strMonth = DateTime.Now.Month.ToString();
            //string strDateStart = strYear + "-" + strMonth + "-1";//每月的开始查询日期
            //string strDateEnd = strYear + "-" + strMonth + "-31";//每月的结束查询日期
            //if (strMonth == "4" || strMonth == "6" || strMonth == "9" || strMonth == "11") {
            //     strDateEnd = strYear + "-" + strMonth + "-30";
            //}
            //if (strMonth == "2") {
            //     strDateEnd = strYear + "-" + strMonth + "-28";
            //}
            string strDateStart = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToShortDateString();
            string strDateEnd = DateTime.Now.AddMonths(1).AddDays(1 - DateTime.Now.Day).ToShortDateString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT  SUM( CASE ISReturn WHEN  0 THEN  VarietyCount ELSE 0-VarietyCount END) AS VarietyCount   FROM dbo.GoodExchange ");
            strSql.Append("  WHERE Dep_SID='" + Dep_SID + "'");
            strSql.Append("   AND dt_Exchange>='" + strDateStart + "' AND dt_Exchange<='" + strDateEnd + "'");
            strSql.Append("  ");
            object objSum = SQLHelper.ExecuteScalar(strSql.ToString());
            if (objSum == null || objSum.ToString() == "")
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(objSum);
            }
        }

        #endregion


        #region 兑换利息计算和反算  方式二

        /// <summary>
        /// 获取兑换时需要折合的原粮数量
        /// </summary>
        /// <param name="Dep_SID"></param>
        /// <param name="numMoney"></param>
        /// <returns></returns>
        public static Dictionary<string, double> GetExchangeVC(string Dep_SID, double numMoney)
        {
            double VarietyMark = 100;//标定的产品数量
            double MoneyMark = GetMoneyMark(Dep_SID, VarietyMark);
            double VarietyCount = (VarietyMark * numMoney) / MoneyMark;
            double VarietyLiXi = GetLiXi(Dep_SID, VarietyCount);

            Dictionary<string, double> dic = new Dictionary<string, double>();
            dic.Add("VarietyCount", VarietyCount);
            dic.Add("VarietyLiXi", VarietyLiXi);
            return dic;
        }

        /// <summary>
        /// 获取定期类型兑换时需要折合的原粮数量
        /// </summary>
        /// <param name="Dep_SID"></param>
        /// <param name="numMoney"></param>
        /// <returns></returns>
        public static Dictionary<string, double> GetExchangeVC_DingQi(string Dep_SID, double numMoney, double Exchange_trading)
        {
            double VarietyCount = 0;//需要折合的原粮的数量
            double Money_YouHui = 0;//定期优惠的金额
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Dictionary<string, double> dic = new Dictionary<string, double>();
            if (dt != null && dt.Rows.Count != 0)
            {
                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价

                VarietyCount = numMoney / Price_ShiChang;//暂时设定折合原粮数量按照存入价格计算

                if (ts_Storage.TotalDays < numStorageDate)//定期存期未到期的情况
                {
                    //获取每个月的优惠兑换比例
                    StringBuilder strSqlProp = new StringBuilder();
                    strSqlProp.Append("  SELECT TOP 1 A.StorageNumberRaw,B.numExChangeProp");
                    strSqlProp.Append("   FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
                    strSqlProp.Append("    WHERE A.ID=" + Dep_SID);
                    DataTable dtProp = SQLHelper.ExecuteDataTable(strSqlProp.ToString());
                    if (dtProp == null || dtProp.Rows.Count == 0)//没有查询到优惠兑换比例
                    {
                        dic.Clear();
                        dic.Add("VarietyCount", VarietyCount);
                        dic.Add("Ex_YouHui", 0);
                        dic.Add("Ex_YouHui_Count", 0);
                        return dic;
                    }
                    double JieCun_Raw = Convert.ToDouble(dtProp.Rows[0]["StorageNumberRaw"]);
                    //double JieCun_Total = Convert.ToDouble(dt.Rows[0]["JieCun_Total"]);//已经发生的结存
                    double numExChangeProp = Convert.ToDouble(dtProp.Rows[0]["numExChangeProp"]);
                    double JieCun_Total = GetMonthJieCun_Total(Dep_SID);//已经发生的结存
                    JieCun_Total = JieCun_Total + Exchange_trading;
                    //查询本月已经发生的结存
                    if (numExChangeProp <= 0)
                    {
                        dic.Clear();
                        dic.Add("VarietyCount", VarietyCount);
                        dic.Add("Ex_YouHui", 0);
                        dic.Add("Ex_YouHui_Count", 0);
                        return dic;
                    }
                    double Exchange_Count = JieCun_Raw * numExChangeProp * 0.01;//最多允许兑换的数量
                    double SurPlue_Count = Exchange_Count - JieCun_Total;//剩余的可以优惠兑换的数量
                    if (SurPlue_Count <= 0)//优惠兑换比例已经用完
                    {
                        dic.Clear();
                        dic.Add("VarietyCount", VarietyCount);
                        dic.Add("Ex_YouHui", 0);
                        dic.Add("Ex_YouHui_Count", 0);
                        return dic;
                    }
                    if (SurPlue_Count * Price_DaoQi >= numMoney)//可优惠兑换数量大于兑换数量
                    {
                        VarietyCount = numMoney / Price_DaoQi;
                        Money_YouHui = (Price_DaoQi - Price_ShiChang) * VarietyCount;
                        dic.Clear();
                        dic.Add("VarietyCount", VarietyCount);
                        dic.Add("Ex_YouHui", Money_YouHui);
                        dic.Add("Ex_YouHui_Count", VarietyCount);
                        //HttpContext.Current.Session["Ex_YouHui"] = Money_YouHui;//缓存优惠金额
                        //HttpContext.Current.Session["Ex_YouHui_Count"] = VarietyCount;//缓存此次定期优惠的数额
                    }
                    else
                    {
                        Money_YouHui = (Price_DaoQi - Price_ShiChang) * SurPlue_Count;
                        VarietyCount = SurPlue_Count + (numMoney - SurPlue_Count * Price_DaoQi) / Price_ShiChang;
                        dic.Clear();
                        dic.Add("VarietyCount", VarietyCount);
                        dic.Add("Ex_YouHui", Money_YouHui);
                        dic.Add("Ex_YouHui_Count", VarietyCount);
                        //HttpContext.Current.Session["Ex_YouHui"] = Money_YouHui;//缓存优惠金额
                        //HttpContext.Current.Session["Ex_YouHui_Count"] = SurPlue_Count;//缓存此次定期优惠的数额
                    }


                }
                else
                {
                    VarietyCount = numMoney / Price_DaoQi;
                    dic.Clear();
                    dic.Add("VarietyCount", VarietyCount);
                    dic.Add("Ex_YouHui", 0);
                    dic.Add("Ex_YouHui_Count", 0);
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取定期类型兑换时的兑换政策
        /// </summary>
        /// <param name="Dep_SID"></param>
        /// <param name="numMoney"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEP_DingQi(string Dep_SID, double numMoney, double Exchange_trading)
        {
            string strReturn = "当前产品存期类型：定期,";//返回信息
            Dictionary<string, string> dic = new Dictionary<string, string>();
            double VarietyCount = 0;//需要折合的原粮的数量
            double Money_YouHui = 0;//定期优惠的金额
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT A.StorageDate,A.InterestDate,A.CurrentRate, A.Price_DaoQi, A.Price_ShiChang,B.numStorageDate");
            strSql.Append(" FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
            strSql.Append(" WHERE A.ID=" + Dep_SID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                DateTime StorageDate = Convert.ToDateTime(dt.Rows[0]["StorageDate"]);//存入日期
                DateTime InterestDate = Convert.ToDateTime(dt.Rows[0]["InterestDate"]);//上一次的取利息日期
                int numStorageDate = Convert.ToInt32(dt.Rows[0]["numStorageDate"]);//约定存储时间
                TimeSpan ts_Storage = DateTime.Now.Subtract(StorageDate);
                double Price_ShiChang = Convert.ToDouble(dt.Rows[0]["Price_ShiChang"]);//存入时的市场价
                double Price_DaoQi = Convert.ToDouble(dt.Rows[0]["Price_DaoQi"]);//到期价

                VarietyCount = numMoney / Price_ShiChang;//暂时设定折合原粮数量按照存入价格计算

                if (ts_Storage.TotalDays < numStorageDate)//定期存期未到期的情况
                {
                    strReturn += "未到期,";
                    //获取每个月的优惠兑换比例
                    StringBuilder strSqlProp = new StringBuilder();
                    strSqlProp.Append("  SELECT TOP 1 A.StorageNumberRaw,B.numExChangeProp");
                    strSqlProp.Append("   FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageTime B ON A.TimeID=B.ID");
                    strSqlProp.Append("    WHERE A.ID=" + Dep_SID);
                    DataTable dtProp = SQLHelper.ExecuteDataTable(strSqlProp.ToString());
                    if (dtProp == null || dtProp.Rows.Count == 0)//没有查询到优惠兑换比例
                    {

                        strReturn += "存入价：" + Price_ShiChang + ",无优惠兑换比例!";

                        dic.Clear();
                        dic.Add("daoqi", "false");
                        dic.Add("youhui", "false");
                        dic.Add("msg", strReturn);
                        dic.Add("Price_ShiChang", Price_ShiChang.ToString());
                        return dic;
                    }
                    double JieCun_Raw = Convert.ToDouble(dtProp.Rows[0]["StorageNumberRaw"]);
                    //double JieCun_Total = Convert.ToDouble(dt.Rows[0]["JieCun_Total"]);//已经发生的结存
                    double numExChangeProp = Convert.ToDouble(dtProp.Rows[0]["numExChangeProp"]);

                    double JieCun_Total = GetMonthJieCun_Total(Dep_SID);//已经发生的结存
                    JieCun_Total = JieCun_Total + Exchange_trading;//实际的结存数量为已经发生的结存数量和已经准备结存的数量
                    //查询本月已经发生的结存
                    if (numExChangeProp <= 0)
                    {
                        strReturn += "存入价：" + Price_ShiChang + ",无优惠兑换比例!";

                        dic.Clear();
                        dic.Add("daoqi", "false");
                        dic.Add("youhui", "false");
                        dic.Add("msg", strReturn);
                        dic.Add("Price_ShiChang", Price_ShiChang.ToString());
                        return dic;
                    }
                    double Exchange_Count = JieCun_Raw * numExChangeProp * 0.01;//最多允许兑换的数量
                    double SurPlue_Count = Exchange_Count - JieCun_Total;//剩余的可以优惠兑换的数量

                    if (JieCun_Total > Exchange_Count)
                    {
                        JieCun_Total = Exchange_Count;
                        SurPlue_Count = 0;
                    }

                    strReturn += "存入价：" + Price_ShiChang + ",到期价：" + Price_DaoQi + ",本月已优惠兑换数量：" + Math.Round(JieCun_Total, 2) + ",剩余可优惠兑换数量：" + Math.Round(SurPlue_Count, 2);
                    dic.Clear();
                    dic.Add("daoqi", "false");
                    dic.Add("youhui", "true");
                    dic.Add("msg", strReturn);
                    dic.Add("Price_ShiChang", Price_ShiChang.ToString());
                    dic.Add("Price_DaoQi", Price_DaoQi.ToString());
                    dic.Add("YouHui_Count", Math.Round(JieCun_Total, 2).ToString());
                    dic.Add("SurPlue_Count", Math.Round(SurPlue_Count, 2).ToString());
                    return dic;
                }
                else
                {
                    strReturn += "已到期,到期价：" + Price_DaoQi;
                    dic.Clear();
                    dic.Add("daoqi", "true");
                    dic.Add("youhui", "true");
                    dic.Add("msg", strReturn);
                    dic.Add("Price_ShiChang", Price_ShiChang.ToString());
                    dic.Add("Price_DaoQi", Price_DaoQi.ToString());

                }
            }
            return dic;
        }



        #endregion

        #region  登录及站点信息
        /// <summary>
        /// 获取公司信息（strLink:存储期限；strRemark:价格政策）
        /// </summary>
        /// <returns></returns>
        public static DataRow GetCompanyInfo()
        {
            string strSql = "SELECT TOP 1 strName,strAddress,strLink,strPhone,strRemark,strPassword,webSiteCode  FROM dbo.BD_Company";
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }

        }

        public static bool ISHQWB(object wbid)
        {
            object obj = SQLHelper.ExecuteScalar(" SELECT ISHQ FROM dbo.WB  WHERE ID=" + wbid);
            return Convert.ToBoolean(obj);
        }

        /// <summary>
        /// 网点信息查询
        /// </summary>
        /// <param name="wbname"></param>
        /// <returns></returns>
        public static DataRow GetWBInfoByName(string wbname)
        {
            string sql = " SELECT * FROM dbo.WB WHERE strName='" + wbname + "'";
            DataTable dt = SQLHelper.ExecuteDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }

        }

        public static DataRow GetWBInfoByID(int ID)
        {
            string sql = " SELECT * FROM dbo.WB WHERE ID=" + ID;
            DataTable dt = SQLHelper.ExecuteDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }

        }

        //获取Uses信息
        public static DataRow GetUserInfoByID(int ID)
        {
            string sql = " SELECT * FROM dbo.Users WHERE ID=" + ID;
            DataTable dt = SQLHelper.ExecuteDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }

        }

        /// <summary>
        /// 返回当前的网点权限设置
        /// </summary>
        /// <returns></returns>
        public static DataRow GetWBAuthority()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT TOP 1 *  FROM dbo.WBAuthority");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            return dt.Rows[0];
        }

        /// <summary>
        /// 检测储户的密码是否正确
        /// </summary>
        /// <param name="AccountNumber">储户账号</param>
        /// <param name="Password">未经加密的密码</param>
        /// <returns></returns>
        public static bool CheckPassword(string AccountNumber, string Password)
        {
            bool flag = true;
            if (Convert.ToInt32(common.GetWBAuthority()["VerifyType"]) == 1)//需要输入密码验证 
            {
                //查询当前的密码与储户的密码是否符合
                string strPassword = SQLHelper.ExecuteScalar(" SELECT strPassword FROM dbo.Depositor WHERE AccountNumber='" + AccountNumber + "'").ToString();
                if (Fun.GetMD5_32(Password) == strPassword)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = true;
            }
            return flag;

        }

        /// <summary>
        /// 检测储户的密码是否正确
        /// </summary>
        /// <param name="AccountNumber">储户账号</param>
        /// <param name="Password">未经加密的密码</param>
        /// <returns></returns>
        public static bool CheckPassword_Commune(string AccountNumber, string Password)
        {
            bool flag = true;
            if (Convert.ToInt32(common.GetWBAuthority()["VerifyType"]) == 1)//需要输入密码验证 
            {
                //查询当前的密码与储户的密码是否符合
                string strPassword = SQLHelper.ExecuteScalar(" SELECT strPassword FROM dbo.Commune WHERE AccountNumber='" + AccountNumber + "'").ToString();
                if (Fun.GetMD5_32(Password) == strPassword)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = true;
            }
            return flag;

        }
        #endregion

        #region 储户账号、流水号创建、存粮记录及查询
        /// <summary>
        /// 添加储户的时候，获取新的储户编号
        /// </summary>
        /// <param name="WB_ID"></param>
        /// <returns></returns>
        public static string GetNewAccountNumber(string WB_ID)
        {
            string strSqlNum = "SELECT SerialNumber FROM dbo.WB WHERE ID=" + WB_ID;
            string SerialNumber = SQLHelper.ExecuteScalar(strSqlNum).ToString();
            string AccountNumber = SerialNumber + "0000001";

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT TOP 1 AccountNumber  FROM  dbo.Depositor ");
            strSql.Append("  WHERE WBID=" + WB_ID);
            //strSql.Append("  ORDER BY dt_Add DESC ");
            strSql.Append("  ORDER BY AccountNumber DESC ");

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());

            if (obj != null)
            {
                int numIndex = Convert.ToInt32(obj.ToString().Substring(3));
                AccountNumber = SerialNumber + Fun.ConvertIntToString(numIndex + 1, 7);
            }

            string strSameAccount = "  SELECT  COUNT(ID)  FROM dbo.Depositor WHERE AccountNumber='" + AccountNumber + "'";
            string ANCount = SQLHelper.ExecuteScalar(strSameAccount).ToString();
            
            while (ANCount != "0") {
                int numIndex = Convert.ToInt32(AccountNumber.ToString().Substring(3));
                AccountNumber = SerialNumber + Fun.ConvertIntToString(numIndex + 1, 7);
                 strSameAccount = "  SELECT  COUNT(ID)  FROM dbo.Depositor WHERE AccountNumber='" + AccountNumber + "'";
                 ANCount = SQLHelper.ExecuteScalar(strSameAccount).ToString();
            }
            return AccountNumber;
        }

        /// <summary>
        /// 返回储户新的流水号
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <returns></returns>
        public static string GetNewBusinessNO_Dep(string AccountNumber)
        {
            string BusinessNO = "0001";//使用4位编号
            //查询当前社员的业务编号
            StringBuilder strSqlNO = new StringBuilder();
            strSqlNO.Append("  SELECT TOP 1 BusinessNO ");
            strSqlNO.Append("  FROM dbo.Dep_OperateLog  ");
            strSqlNO.Append("  WHERE Dep_AccountNumber='" + AccountNumber.Trim() + "'");
            strSqlNO.Append("  ORDER BY CONVERT(INT,BusinessNO) DESC");
            object obj = SQLHelper.ExecuteScalar(strSqlNO.ToString());
            if (obj != null && obj.ToString() != "")
            {
                int BNO = Convert.ToInt32(obj) + 1;
                BusinessNO = Fun.ConvertIntToString(BNO, 4);
            }

            return BusinessNO;
        }

        /// <summary>
        /// 查询储户某一种粮食的存储数量
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="VarietyID"></param>
        /// <returns></returns>
        public static double GetDep_StorageNumber(string AccountNumber, string VarietyID)
        {
            string sql = string.Format("SELECT SUM( StorageNumber)  AS StorageNumber FROM dbo.Dep_StorageInfo WHERE AccountNumber='{0}' AND VarietyID={1}", AccountNumber, VarietyID);
            object obj = SQLHelper.ExecuteScalar(sql);
            if (obj == null || obj.ToString() == "")
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(obj);
            }
        }

        public static DataTable getDepInfo(string AccountNumber,bool ISHQ,string WBID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select ID,WBID,AccountNumber,strPassword,CunID as BD_Address_CunID,strAddress,strName,PhoneNO,ISSendMessage,BankCardNO,dt_Update,");
            strSql.Append("   numState,dt_Add,");
            strSql.Append("   CASE (IDCard) WHEN '' THEN '未填写' ELSE '******' END as IDCard");
            strSql.Append(" FROM dbo.Depositor ");
            strSql.Append(" where  1=1");
            strSql.Append(" and ISClosing=0");
            strSql.Append(string.Format(" and AccountNumber='{0}' ", AccountNumber));
            if (!ISHQ)
            { //非总部网点查看
                strSql.Append(string.Format(" and WBID={0} ",WBID));
            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            return dt;
        }

        public static DataTable getDepStorageInfo(string AccountNumber)
        {
            //获取存粮信息
            StringBuilder strSqlStorage = new StringBuilder();
            strSqlStorage.Append("SELECT A.ID,A.StorageNumber,convert(varchar(10),A.StorageDate,120) AS StorageDate, A.AccountNumber,B.strName AS VarietyID,A.Price_ShiChang,A.Price_DaoQi,A.CurrentRate,C.strName AS TimeID,A.StorageFee");
            strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSqlStorage.Append("  INNER JOIN dbo.StorageTime C ON A.TimeID=C.ID");
            strSqlStorage.Append("  WHERE AccountNumber='" + AccountNumber + "'");
            strSqlStorage.Append("  and A.StorageNumber>0");
            DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());

            DataColumn dcstrlixi = new DataColumn("strlixi", typeof(string));
            DataColumn dcnumlixi = new DataColumn("numlixi", typeof(string));
            dtStorage.Columns.Add(dcstrlixi);
            dtStorage.Columns.Add(dcnumlixi);
            for (int i = 0; i < dtStorage.Rows.Count; i++)
            {
                Dictionary<string, string> dicLixi = common.GetLiXi_html(dtStorage.Rows[i]["ID"]);
                string strlixi = dicLixi["strLixi"];
                string numlixi = dicLixi["numLixi"];
                dtStorage.Rows[i]["strlixi"] = strlixi;
                dtStorage.Rows[i]["numlixi"] = numlixi;
            }
            return dtStorage;
        }

        public static double GetLiXiTotal(DataTable dt) {
            double numTotol = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                numTotol += Convert.ToDouble(dt.Rows[i]["StorageNumber"]) * Convert.ToDouble(dt.Rows[i]["Price_ShiChang"]) + Convert.ToDouble(dt.Rows[i]["numlixi"]);
            }
            return Math.Round(numTotol, 2);
        }

        /// <summary>
        /// 查找上一次粮食存储记录
        /// </summary>
        /// <param name="wbid"></param>
        /// <param name="varietyid"></param>
        /// <returns></returns>
        public static DataRow GetLastVSLog(string wbid, string varietyid, string varietylevelid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("  SELECT TOP 1  * FROM dbo.SA_VarietyStorageLog");
            sql.Append(string.Format("  WHERE WBID={0} AND VarietyID={1} AND VarietyLevelID={2}", wbid, varietyid,varietylevelid));
            sql.Append("  ORDER BY ID DESC");
            DataTable dt = SQLHelper.ExecuteDataTable(sql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }
        }

        /// <summary>
        /// 根据CheckOutID查找粮食存储记录
        /// </summary>
        /// <param name="CheckOutID"></param>
        /// <returns></returns>
        public static DataRow getVSLogByCheckOutID(int CheckOutID)
        {
            string sql = " SELECT * FROM dbo.SA_VarietyStorageLog WHERE CheckOutID=" + CheckOutID;
            DataTable dt = SQLHelper.ExecuteDataTable(sql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }
        }


        public static int getPrintTime(string AccountNumber,string BusinessNO) {
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("  select * from Dep_PrintLog");
            strSqlLog.Append(" where BusinessNO='" + BusinessNO + "' and  Dep_AccountNumber='" + AccountNumber + "'");

            DataTable dtLog = SQLHelper.ExecuteDataTable(strSqlLog.ToString());

            StringBuilder sql = new StringBuilder();
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                return 1;
            }
            else
            {
                int numTime = Convert.ToInt32(dtLog.Rows[0]["numTime"]) + 1;
                return numTime;
            }


        }
        #endregion

        #region 数据唯一性检测
        public static bool UniqueCheck_Add(string tabname,string colname,string colvalue) {
           string sql = string.Format(" SELECT COUNT(ID) FROM {0} WHERE {1}='{2}'",tabname,colname,colvalue);
           return Convert.ToInt32(SQLHelper.ExecuteScalar(sql)) == 0;
        }
        public static bool UniqueCheck_Update(string tabname, string colname, string colvalue,string id)
        {
            string sql = string.Format(" SELECT COUNT(ID) FROM {0} WHERE {1}='{2}' and ID!={3}", tabname, colname, colvalue,id);
            return Convert.ToInt32(SQLHelper.ExecuteScalar(sql)) == 0;
        }
        #endregion 
    }
}