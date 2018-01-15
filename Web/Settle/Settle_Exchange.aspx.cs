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
    public partial class Settle_Exchange : System.Web.UI.Page
    {
        public double VarietyCount = 0;
        public double VarietyInterest = 0;
        public double Money_DuiHuan = 0;
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

            strSqlInner.Append(" SELECT * FROM dbo.GoodExchange WHERE (");
            strSqlInner.Append(string.Format("    ISReturn IN (SELECT ID FROM dbo.GoodExchange WHERE WBID={0})",WBID));
            strSqlInner.Append(string.Format(" OR dbo.GoodExchange.WBID={0})",WBID));
            DateTime dateEnd = Convert.ToDateTime(dtEnd);
            strSqlInner.Append(string.Format(" AND dt_Exchange> '{0}' ",dtStart));
            strSqlInner.Append(string.Format(" AND dt_Exchange < '{0}' ", dateEnd.AddDays(1).ToString()));
          
            strSql.Append("  SELECT S.ID as SAID, A.ID, B.strName AS WBName,C.AccountNumber,C.strName AS DepName,CONVERT(NVARCHAR(100),A.dt_Exchange,23)  AS dt_Exchange,");
            strSql.Append(" A.BusinessName,A.GoodName,A.SpecName,A.UnitName,A.GoodCount,A.GoodPrice,A.VarietyCount,A.VarietyInterest,A.Money_DuiHuan,A.ISReturn");
            strSql.Append("   FROM (" + strSqlInner.ToString() + ") A");
            strSql.Append("  INNER JOIN dbo.WB B ON A.WBID=B.ID ");
            strSql.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber =C.AccountNumber");
            strSql.Append("  LEFT OUTER JOIN dbo.SA_Exchange S ON A.ID=S.GoodExchangeID");
            strSql.Append("   ORDER BY A.dt_Exchange DESC ");
          
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();

            VarietyCount = 0; VarietyInterest = 0; Money_DuiHuan = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["SAID"] == null || dt.Rows[i]["SAID"].ToString() == "")
                {
                    int ISReturn = Convert.ToInt32(dt.Rows[i]["ISReturn"]);
                    if (ISReturn == 0)
                    {
                        VarietyCount += Convert.ToDouble(dt.Rows[i]["VarietyCount"]);
                        VarietyInterest += Convert.ToDouble(dt.Rows[i]["VarietyInterest"]);
                        Money_DuiHuan += Convert.ToDouble(dt.Rows[i]["Money_DuiHuan"]);
                    }
                    else
                    {
                        VarietyCount -= Convert.ToDouble(dt.Rows[i]["VarietyCount"]);
                        VarietyInterest -= Convert.ToDouble(dt.Rows[i]["VarietyInterest"]);
                        Money_DuiHuan -= Convert.ToDouble(dt.Rows[i]["Money_DuiHuan"]);
                    }
                }
                else
                {
                    if (dt.Rows[i]["ISReturn"].ToString() == "0")
                    {
                        JieSuan += Convert.ToDouble(dt.Rows[i]["Money_DuiHuan"]);
                    }
                    else
                    {
                        JieSuan -= Convert.ToDouble(dt.Rows[i]["Money_DuiHuan"]);
                    }
                }
            }
            VarietyCount = Math.Round(VarietyCount, 2);
            VarietyInterest = Math.Round(VarietyInterest, 2);
            Money_DuiHuan = Math.Round(Money_DuiHuan, 2);


        }

        ////private void BindData2(string WBID, string dtStart, string dtEnd)
        ////{
        ////    string HQ_WBID = Session["WB_ID"].ToString();

        ////    //获取存粮信息
        ////    StringBuilder strSql = new StringBuilder();
        ////    StringBuilder strSqlPara = new StringBuilder();//查询参数
        ////    strSql.Append("  SELECT S.ID as SAID, A.ID, B.strName AS WBName,C.AccountNumber,C.strName AS DepName,CONVERT(NVARCHAR(100),A.dt_Exchange,23)  AS dt_Exchange,");
        ////    strSql.Append(" A.BusinessName,A.GoodName,A.SpecName,A.UnitName,A.GoodCount,A.GoodPrice,A.VarietyCount,A.VarietyInterest,A.Money_DuiHuan,A.ISReturn");
        ////    strSqlPara.Append("  FROM dbo.GoodExchange A INNER JOIN dbo.WB B ON A.WBID=B.ID");
        ////    strSqlPara.Append("  INNER JOIN dbo.Depositor C ON A.Dep_AccountNumber =C.AccountNumber");
        ////    strSqlPara.Append("  LEFT OUTER JOIN dbo.SA_Exchange S ON A.ID=S.GoodExchangeID");
        ////    strSqlPara.Append(" where 1=1  ");
        ////    if (WBID != "")
        ////    {
        ////       // strSqlPara.Append("   AND A.WBID = " + WBID + "");
        ////        strSqlPara.Append("   AND A.WBID in( " + WBID +","+HQ_WBID+ ")");
        ////    }
        ////    if (dtStart != "")
        ////    {
        ////        strSqlPara.Append("   AND A.dt_Exchange> '" + dtStart + "'");
        ////    }
        ////    if (dtEnd != "")
        ////    {
        ////        DateTime dateEnd = Convert.ToDateTime(dtEnd);

        ////        strSqlPara.Append("   AND A.dt_Exchange < '" + dateEnd.AddDays(1).ToString() + "'");
        ////    }
        ////    strSql.Append(strSqlPara.ToString());
        ////    strSql.Append(" order by A.dt_Exchange desc  ");
        ////    DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
        ////    Repeater1.DataSource = dt;
        ////    Repeater1.DataBind();

        ////    VarietyCount = 0;  VarietyInterest = 0; Money_DuiHuan = 0; 
        ////    for (int i = 0; i < dt.Rows.Count; i++)
        ////    {
        ////        if (dt.Rows[i]["SAID"] == null || dt.Rows[i]["SAID"].ToString() == "")
        ////        {
        ////            int ISReturn = Convert.ToInt32(dt.Rows[i]["ISReturn"]);
        ////            if (ISReturn == 0)
        ////            {
        ////                VarietyCount += Convert.ToDouble(dt.Rows[i]["VarietyCount"]);
        ////                VarietyInterest += Convert.ToDouble(dt.Rows[i]["VarietyInterest"]);
        ////                Money_DuiHuan += Convert.ToDouble(dt.Rows[i]["Money_DuiHuan"]);
        ////            }
        ////            else
        ////            {
        ////                VarietyCount -= Convert.ToDouble(dt.Rows[i]["VarietyCount"]);
        ////                VarietyInterest -= Convert.ToDouble(dt.Rows[i]["VarietyInterest"]);
        ////                Money_DuiHuan -= Convert.ToDouble(dt.Rows[i]["Money_DuiHuan"]);
        ////            }
        ////        }
        ////        else
        ////        {
        ////            if (dt.Rows[i]["ISReturn"].ToString() == "0")
        ////            {
        ////                JieSuan += Convert.ToDouble(dt.Rows[i]["Money_DuiHuan"]);
        ////            }
        ////            else
        ////            {
        ////                JieSuan -= Convert.ToDouble(dt.Rows[i]["Money_DuiHuan"]);
        ////            }
        ////        }
        ////    }
        ////    VarietyCount = Math.Round(VarietyCount, 2);
        ////    VarietyInterest = Math.Round(VarietyInterest, 2);
        ////    Money_DuiHuan = Math.Round(Money_DuiHuan, 2);


        ////}

        public string GetMoneyExchange(object ISReturn, object Money_DuiHuan) 
        {
            if (ISReturn.ToString() == "0")
            {
                return "<span style='color:Green'>" + Money_DuiHuan + "</span>";
            }
            else {
                return "<span style='color:Red'>-" + Money_DuiHuan + "</span>";
            }
        }

        public string GetOperateInfo(object ID)
        {
            string strReturn = "";
            //查询当前的这笔记录是否已经有结算的记录
            object objID = SQLHelper.ExecuteScalar("SELECT ID FROM dbo.SA_Exchange WHERE GoodExchangeID=" + ID);
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