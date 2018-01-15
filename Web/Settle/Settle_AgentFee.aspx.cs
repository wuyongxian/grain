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
    public partial class Settle_AgentFee : System.Web.UI.Page
    {
        public double numCunRu=0;
        public double numZhiQu=0;
        public double numShiCun=0;
        public double numMoenyFee = 0;
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

            //当前网点最少要存储满多少天可以给网点代理费
            DataTable dtWB = SQLHelper.ExecuteDataTable(" SELECT  top 1  numDay,draw_exchange,draw_sell,draw_shopping FROM dbo.WB WHERE ID =" + WBID);
            if (dtWB == null || dtWB.Rows.Count == 0) {
                Fun.Alert("查询该网点信息失败！");
                return;
            }
            int numDay = Convert.ToInt32(dtWB.Rows[0]["numDay"]);
            int numMinute = numDay * 24 * 60;//提前支取计算的分钟数
            int draw_exchange = Convert.ToInt32(dtWB.Rows[0]["draw_exchange"]);
            int draw_sell = Convert.ToInt32(dtWB.Rows[0]["draw_sell"]);
            int draw_shopping = Convert.ToInt32(dtWB.Rows[0]["draw_shopping"]);
            string buslist="";
            if (draw_exchange == 1) {
                buslist += "," + "'2','6'";
            }
            if (draw_sell == 1)
            {
                buslist += "," + "'3','7'";
            }
            if (draw_shopping == 1)
            {
                buslist += "," + "'9','10'";
            }
            if (buslist != "")
            {
                buslist = buslist.Substring(1);
            }
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSqlPara = new StringBuilder();//查询参数
            strSql.Append("   SELECT F.ID AS AgentFeeID, A.ID,A.VarietyID ,V.strName AS VarietyName, C.strName AS WBName,B.AccountNumber,B.strName,'存粮代理' AS BusinessName,CONVERT(NVARCHAR(100),A.StorageDate,23 ) AS  StorageDate,");
            strSql.Append("  A.StorageDate as StorageTime, ");
            strSql.Append("   DATEDIFF(DAY,A.StorageDate,GETDATE()) AS StorageDay,  A.StorageNumberRaw AS CunRu, 0 as zhiqu,A.StorageNumberRaw AS shicun, C.numAgent,A.StorageNumberRaw*C.numAgent AS MoenyFee ");
            strSql.Append("    FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber  ");
            strSql.Append("    LEFT OUTER JOIN dbo.StorageVariety V ON A.VarietyID=V.ID");
            strSql.Append("    LEFT OUTER JOIN dbo.SA_AgentFee F ON A.ID=F.Dep_StorageInfoID");
            strSql.Append("   INNER JOIN dbo.WB C ON B.WBID=C.ID ");

            strSqlPara.Append("  WHERE DATEDIFF(minute,A.StorageDate,GETDATE())>" + numMinute);
            if (WBID != "" && WBID!="0")
            {
                strSqlPara.Append("   AND A.WBID = " + WBID);
            }
            if (dtStart != "")
            {
                strSqlPara.Append("   AND A.StorageDate> '" + dtStart + "'");
            }
            if (dtEnd != "")
            {
                if (Fun.IsDateTime(dtEnd)) {
                    dtEnd = Convert.ToDateTime(dtEnd).AddDays(1).ToString();
                }
                strSqlPara.Append("   AND A.StorageDate < '" + dtEnd + "'");
            }
            strSql.Append(strSqlPara.ToString());
            strSql.Append("   order by A.StorageDate desc");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            numCunRu = 0; numZhiQu = 0; numShiCun = 0; numMoenyFee = 0;
            if (dt != null && dt.Rows.Count != 0)
            {               
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (buslist != "")
                    {
                        double zhiqu = 0;
                        double shicun = 0;
                        double cunru = Convert.ToDouble(dt.Rows[i]["CunRu"]);
                        double numAgent = Convert.ToDouble(dt.Rows[i]["numAgent"]);
                        double MoenyFee = Convert.ToDouble(dt.Rows[i]["MoenyFee"]);
                        string ID = dt.Rows[i]["ID"].ToString();
                        //string AccountNumber = dt.Rows[i]["AccountNumber"].ToString();
                        //string VarietyID = dt.Rows[i]["VarietyID"].ToString();
                        //string StorageDate = dt.Rows[i]["StorageDate"].ToString();
                        string StorageTime = dt.Rows[i]["StorageTime"].ToString();//储户的准确存储时间
                                           
                        //string sql = string.Format("   SELECT SUM(Count_Trade) FROM dbo.Dep_OperateLog  WHERE BusinessName IN ({0}) AND Dep_AccountNumber='{1}' AND VarietyID='{2}'  AND  DATEDIFF(minute,'{3}',dt_Trade)<={4}", buslist, AccountNumber, VarietyID, StorageTime, numMinute);
                        string sql = string.Format("   SELECT SUM(Count_Trade) FROM dbo.Dep_OperateLog  WHERE BusinessName IN ({0}) AND Dep_SID={1}  AND  DATEDIFF(minute,'{2}',dt_Trade)<={3}", buslist, ID, StorageTime, numMinute);
                        object obj_zhiqu = SQLHelper.ExecuteScalar(sql);
                        if (obj_zhiqu != null && obj_zhiqu.ToString() != "")
                        {
                            zhiqu = Convert.ToDouble(obj_zhiqu);
                        }
                        shicun = cunru - zhiqu;
                        MoenyFee = shicun * numAgent;
                        dt.Rows[i]["zhiqu"] = Math.Round(zhiqu, 2);
                        dt.Rows[i]["shicun"] = Math.Round(shicun, 2);
                        dt.Rows[i]["MoenyFee"] = Math.Round(MoenyFee, 2);
                    }
                    object AgentFeeID = dt.Rows[i]["AgentFeeID"];
                    if (AgentFeeID == null || AgentFeeID.ToString() == "")
                    {
                        numCunRu += Convert.ToDouble(dt.Rows[i]["CunRu"]);
                        numZhiQu += Convert.ToDouble(dt.Rows[i]["zhiqu"]);
                        numShiCun += Convert.ToDouble(dt.Rows[i]["shicun"]);
                        numMoenyFee += Convert.ToDouble(dt.Rows[i]["MoenyFee"]);
                    }
                    else
                    {
                        JieSuan += Convert.ToDouble(dt.Rows[i]["MoenyFee"]);
                    }
                   
                }

                numCunRu = Math.Round(numCunRu, 2);
                numZhiQu = Math.Round(numZhiQu, 2);
                numShiCun = Math.Round(numShiCun, 2);
                numMoenyFee = Math.Round(numMoenyFee, 2);
                JieSuan = Math.Round(JieSuan, 2);
            }

            Repeater1.DataSource = dt;
            Repeater1.DataBind();
        }

    

        public string GetOperateInfo(object Dep_SID, object objAfentFeeID)
        {
            string strReturn = "";
            
            if (objAfentFeeID == null || objAfentFeeID.ToString() == "")
            {
                strReturn = "  <a style='color:Red; font-weight:bold;' href='#' onclick='GetAgentFeeSingle("+Dep_SID+")'>未结算</a>";
               
            }
            else {
                strReturn = "<span style='color:Green'>已结算</span>&nbsp;<a style='color:Blue; font-weight:bold;' href='#' onclick='PrintPage(" + Dep_SID + ")'>结算单据</a>";
            }
            return strReturn;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
          
             string WBID  = Request.Form["QWBID"].ToString();
            if (WBID == "" || WBID=="0")
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