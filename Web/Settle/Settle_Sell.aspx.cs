using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.Settle
{
    public partial class Settle_Sell : System.Web.UI.Page
    {
        public double VarietyCount = 0;
        public double StorageMoney = 0;
        public double VarietyInterest = 0;
        public double VarietyMoney = 0;
        public double Money_Earn = 0;
        public double JieSuan = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Request.QueryString["WBID"] != null)
                {
                    txtWBID.Value = SQLHelper.ExecuteScalar(" SELECT  TOP 1 strName FROM dbo.WB WHERE ID=" + Request.QueryString["WBID"].ToString()).ToString();
                }
                if (Request.QueryString["WBName"] != null)
                {
                    string WBName = Request.QueryString["WBName"].ToString();
                    string dtStart = Request.QueryString["dtStart"].ToString();
                    string dtEnd = Request.QueryString["dtEnd"].ToString();

                }
                else
                {
                    Qdtend.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    Qdtstart.Value = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                }

            }
        }

        private void BindData(string WBID, string dtStart, string dtEnd)
        {
            
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSqlInner = new StringBuilder();

            strSqlInner.Append(" SELECT * FROM dbo.StorageSell WHERE (");
            strSqlInner.Append(string.Format("    ISReturn IN (SELECT ID FROM dbo.StorageSell WHERE WBID={0})", WBID));
            strSqlInner.Append(string.Format(" OR dbo.StorageSell.WBID={0})", WBID));
            DateTime dateEnd = Convert.ToDateTime(dtEnd);
            strSqlInner.Append(string.Format(" AND dt_Sell> '{0}' ", dtStart));
            strSqlInner.Append(string.Format(" AND dt_Sell < '{0}' ", dateEnd.AddDays(1).ToString()));


            strSql.Append("  SELECT S.ID AS SAID, A.ID,B.strName AS WBName,C.AccountNumber,C.strName AS DepName,'存转销结算' AS BusinessName,CONVERT(NVARCHAR(100),A.dt_Sell,23) AS dt_Sell,DATEDIFF(DAY,A.dt_Sell,GETDATE()) AS dt_SellDay,");
            strSql.Append(" A.VarietyName,A.UnitName,A.VarietyCount,A.Price_JieSuan, A.StorageMoney,A.VarietyInterest,A.VarietyMoney,A.Money_Earn,A.ISReturn");
            strSql.Append("   FROM (" + strSqlInner.ToString() + ") A");

            strSql.Append("  INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber=C.AccountNumber");
            strSql.Append("   LEFT OUTER JOIN dbo.SA_Sell S ON A.ID=S.StorageSellID");
            strSql.Append("  ORDER BY A.dt_Sell DESC ");
          
          
           
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();

            VarietyCount = 0; VarietyInterest = 0; StorageMoney = 0; VarietyMoney = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["SAID"] == null || dt.Rows[i]["SAID"].ToString() == "")
                {
                    if (dt.Rows[i]["ISReturn"].ToString() == "0")
                    {
                        VarietyCount += Convert.ToDouble(dt.Rows[i]["VarietyCount"]);
                        VarietyInterest += Convert.ToDouble(dt.Rows[i]["VarietyInterest"]);
                        StorageMoney += Convert.ToDouble(dt.Rows[i]["StorageMoney"]);
                        VarietyMoney += Convert.ToDouble(dt.Rows[i]["VarietyMoney"]);
                        Money_Earn += Convert.ToDouble(dt.Rows[i]["Money_Earn"]);
                    }
                    else
                    {
                        VarietyCount -= Convert.ToDouble(dt.Rows[i]["VarietyCount"]);
                        VarietyInterest -= Convert.ToDouble(dt.Rows[i]["VarietyInterest"]);
                        StorageMoney -= Convert.ToDouble(dt.Rows[i]["StorageMoney"]);
                        VarietyMoney -= Convert.ToDouble(dt.Rows[i]["VarietyMoney"]);
                        Money_Earn -= Convert.ToDouble(dt.Rows[i]["Money_Earn"]);
                    }

                }
                else
                {
                    if (dt.Rows[i]["ISReturn"].ToString() == "0")
                    {
                        JieSuan += Convert.ToDouble(dt.Rows[i]["Money_Earn"]);
                    }
                    else
                    {
                        JieSuan -= Convert.ToDouble(dt.Rows[i]["Money_Earn"]);
                    }
                }

            }
            VarietyCount = Math.Round(VarietyCount, 2);
            VarietyInterest = Math.Round(VarietyInterest, 2);
            StorageMoney = Math.Round(StorageMoney, 2);
            VarietyMoney = Math.Round(VarietyMoney, 2);

            Money_Earn = Math.Round(Money_Earn, 2);
            JieSuan = Math.Round(JieSuan, 2);

        }

        //private void BindData(string WBID, string dtStart, string dtEnd)
        //{
        //    string HQ_WBID = Session["WB_ID"].ToString();

        //    //获取存粮信息
        //    StringBuilder strSql = new StringBuilder();
        //    StringBuilder strSqlPara = new StringBuilder();//查询参数
        //    strSql.Append("  SELECT S.ID AS SAID, A.ID,B.strName AS WBName,C.AccountNumber,C.strName AS DepName,'存转销结算' AS BusinessName,CONVERT(NVARCHAR(100),A.dt_Sell,23) AS dt_Sell,DATEDIFF(DAY,A.dt_Sell,GETDATE()) AS dt_SellDay,");
        //    strSql.Append(" A.VarietyName,A.UnitName,A.VarietyCount,A.Price_JieSuan, A.StorageMoney,A.VarietyInterest,A.VarietyMoney,A.Money_Earn,A.ISReturn");
        //    strSqlPara.Append("  FROM dbo.StorageSell A INNER JOIN dbo.WB B ON A.WBID=B.ID");
        //    strSqlPara.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber=C.AccountNumber");
        //    strSqlPara.Append("   LEFT OUTER JOIN dbo.SA_Sell S ON A.ID=S.StorageSellID");
        //    strSqlPara.Append(" where 1=1 ");
        //    if (WBID != "")
        //    {
        //        //strSqlPara.Append("   AND A.WBID = '" + WBID + "'");
        //        strSqlPara.Append("   AND A.WBID in( " + WBID + "," + HQ_WBID + ")");
        //    }
        //    if (dtStart != "")
        //    {
        //        strSqlPara.Append("   AND A.dt_Sell> '" + dtStart + "'");
        //    }
        //    if (dtEnd != "")
        //    {
        //        DateTime dateEnd = Convert.ToDateTime(dtEnd);
        //        strSqlPara.Append("   AND A.dt_Sell < '" + dateEnd.AddDays(1).ToString() + "'");
        //    }
        //    strSql.Append(strSqlPara.ToString());
        //    strSql.Append(" order by A.dt_Sell desc  ");
        //    DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
        //    Repeater1.DataSource = dt;
        //    Repeater1.DataBind();

        //    VarietyCount = 0; VarietyInterest = 0; StorageMoney = 0; VarietyMoney = 0;
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        if (dt.Rows[i]["SAID"] == null || dt.Rows[i]["SAID"].ToString() == "") {
        //            if (dt.Rows[i]["ISReturn"].ToString() == "0")
        //            {
        //                VarietyCount += Convert.ToDouble(dt.Rows[i]["VarietyCount"]);
        //                VarietyInterest += Convert.ToDouble(dt.Rows[i]["VarietyInterest"]);
        //                StorageMoney += Convert.ToDouble(dt.Rows[i]["StorageMoney"]);
        //                VarietyMoney += Convert.ToDouble(dt.Rows[i]["VarietyMoney"]);
        //                Money_Earn += Convert.ToDouble(dt.Rows[i]["Money_Earn"]);
        //            }
        //            else
        //            {
        //                VarietyCount -= Convert.ToDouble(dt.Rows[i]["VarietyCount"]);
        //                VarietyInterest -= Convert.ToDouble(dt.Rows[i]["VarietyInterest"]);
        //                StorageMoney -= Convert.ToDouble(dt.Rows[i]["StorageMoney"]);
        //                VarietyMoney -= Convert.ToDouble(dt.Rows[i]["VarietyMoney"]);
        //                Money_Earn -= Convert.ToDouble(dt.Rows[i]["Money_Earn"]);
        //            }
               
        //        }
        //        else
        //        {
        //            if (dt.Rows[i]["ISReturn"].ToString() == "0")
        //            {
        //                JieSuan += Convert.ToDouble(dt.Rows[i]["Money_Earn"]);
        //            }
        //            else
        //            {
        //                JieSuan -= Convert.ToDouble(dt.Rows[i]["Money_Earn"]);
        //            }
        //        }
               
        //    }
        //    VarietyCount = Math.Round(VarietyCount, 2);
        //    VarietyInterest = Math.Round(VarietyInterest, 2);
        //    StorageMoney = Math.Round(StorageMoney, 2);
        //    VarietyMoney = Math.Round(VarietyMoney, 2);

        //    Money_Earn = Math.Round(Money_Earn, 2);
        //    JieSuan = Math.Round(JieSuan, 2);
         
        //}

        public string GetVarietyMoney(object Money_Earn, object ISReturn)
        {
            if (Convert.ToInt32(ISReturn) == 0)//第一次存转销的记录
            {
                return "<span style='color:Green'>" + Money_Earn + "</span>";
            }
            else {
                return "<span style='color:Red'>-" + Money_Earn + "</span>";
            }
        }

        public string GetOperateInfo(object ID)
        {
            string strReturn = "";
            //查询当前的这笔记录是否已经有结算的记录
            object objID = SQLHelper.ExecuteScalar("SELECT ID FROM dbo.SA_Sell WHERE StorageSellID=" + ID);
            if (objID == null || objID.ToString() == "")
            {
                strReturn = "  <a style='color:Red; font-weight:bold;' href='#' onclick='GetSettleSingle(" + ID + ")'>未结算</a>";

            }
            else
            {
                strReturn = "<span style='color:Green'>已结算</span>&nbsp;<a style='color:Blue; font-weight:bold;' href='#' onclick='PrintPage(" + ID + ")'>结算单据</a>";
            }
            return strReturn;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            string WBID = Request.Form["QWBID"].ToString();
            if (WBID == "" || WBID == "0")
            {
                Fun.Alert("请选择查询网点!");
                return;
            }
            if (Qdtstart.Value.Trim() == "")
            {
                Fun.Alert("请输入开始查询日期");
                return;
            }
            if (Qdtend.Value.Trim() == "")
            {
                Fun.Alert("请输入结束查询日期");
                return;
            }
            BindData(WBID, Qdtstart.Value.Trim(), Qdtend.Value.Trim());

        }
    }
}