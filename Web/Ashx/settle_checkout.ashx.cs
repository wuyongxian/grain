using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Web.Ashx
{
    /// <summary>
    /// settle_checkout 的摘要说明
    /// </summary>
    public class settle_checkout : IHttpHandler,IRequiresSessionState
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
                    case "getGrainOutPut": getGrainOutPut(context); break;//原粮出库操作
                    case "getGrainOutPut_Single": getGrainOutPut_Single(context); break;
                    case "getCheckOutHistory": getCheckOutHistory(context); break;//原粮出库历史
                }
            }
        }

        //获取原粮出库数据
        void getGrainOutPut(HttpContext context)
        {
            string wbid = context.Request.Form["wbid"].ToString();
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT A.ID,B.ID as WBID, B.strName AS WBName,C.ID as VarietyID, C.strName AS VarietyName,D.ID as VarietyLevelID, D.strName AS VarietyLevelName,A.numStorage");
            strSql.Append("  FROM dbo.SA_VarietyStorage A INNER JOIN WB B ON A.WBID=B.ID");
            strSql.Append("  LEFT OUTER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSql.Append("  LEFT OUTER JOIN dbo.StorageVarietyLevel_B  D ON A.VarietyLevelID=D.ID");
            strSql.Append("  ");

            if (wbid.ToString().Trim() != ""&&wbid.ToString().Trim() !="0")
            {
                strSql.Append("   WHERE B.ID = '" + wbid + "'");
            }

            strSql.Append("   order by B.ID");

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


        //获取单条原粮出库数据
        void getGrainOutPut_Single(HttpContext context)
        {
            string ID = context.Request.Form["ID"].ToString();
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT top 1 A.ID,B.ID as WBID, B.strName AS WBName,C.ID as VarietyID, C.strName AS VarietyName,D.ID as VarietyLevelID, D.strName AS VarietyLevelName,A.numStorage");
            strSql.Append("  FROM dbo.SA_VarietyStorage A INNER JOIN WB B ON A.WBID=B.ID");
            strSql.Append("  LEFT OUTER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSql.Append("  LEFT OUTER JOIN dbo.StorageVarietyLevel_B  D ON A.VarietyLevelID=D.ID");
            strSql.Append("  ");

            if (ID.ToString().Trim() != "" && ID.ToString().Trim() != "0")
            {
                strSql.Append("   WHERE A.ID = '" + ID + "'");
            }

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


        //获取原粮出库历史
        void getCheckOutHistory(HttpContext context)
        {
            string wbid = context.Request.Form["wbid"].ToString();
            string Qdtstart = context.Request.Form["Qdtstart"].ToString();
            string Qdtend = context.Request.Form["Qdtend"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT  B.strName AS WBName,A.ID,A.SA_AN,V.strName AS  Variety_Name,VarietyLevel_Name,Weight_Mao,Weight_Pi,Weight_Jing,Weight_Reality,CONVERT(VARCHAR(100),dt_Trade,23) AS dt_Trade");
            strSql.Append("  FROM dbo.SA_CheckOut A INNER JOIN WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety V ON A.Variety_Name=V.ID");

            if (wbid.ToString().Trim() != "")
            {
                strSql.Append("   WHERE B.ID = '" + wbid + "'");
            }
            if (Qdtstart.ToString().Trim() != "")
            {
                strSql.Append("   and A.dt_Trade > '" + Qdtstart.ToString().Trim() + "'");
            }
            if (Qdtend.ToString().Trim() != "")
            {
                Qdtend = Convert.ToDateTime(Qdtend.ToString().Trim()).AddDays(1).ToString();
                strSql.Append("   and A.dt_Trade < '" + Qdtend + "'");
            }

            strSql.Append("  ORDER BY A.dt_Trade DESC");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());



            StringBuilder strSqlTotal = new StringBuilder();
            strSqlTotal.Append("   SELECT  B.strName AS WBName,V.strName AS  Variety_Name,");
            strSqlTotal.Append("    SUM( Weight_Mao) AS Weight_Mao, SUM( Weight_Pi) AS Weight_Pi, SUM( Weight_Jing) AS Weight_Jing, SUM( Weight_Reality) AS Weight_Reality");
            strSqlTotal.Append("  FROM dbo.SA_CheckOut A INNER JOIN WB B ON A.WBID=B.ID");
            strSqlTotal.Append("   INNER JOIN dbo.StorageVariety V ON A.Variety_Name=V.ID");

            if (wbid.ToString().Trim() != "")
            {
                strSqlTotal.Append("   WHERE B.ID = '" + wbid + "'");
            }
            if (Qdtstart.ToString().Trim() != "")
            {
                strSqlTotal.Append("   and A.dt_Trade > '" + Qdtstart.ToString().Trim() + "'");
            }
            if (Qdtend.ToString().Trim() != "")
            {
                Qdtend = Convert.ToDateTime(Qdtend.ToString().Trim()).AddDays(1).ToString();
                strSqlTotal.Append("   and A.dt_Trade < '" + Qdtend + "'");
            }
            strSqlTotal.Append("   GROUP BY B.strName,V.strName");


            DataTable dtTotal = SQLHelper.ExecuteDataTable(strSqlTotal.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                var res = new { state = true, msg = "查询成功!", data = JsonHelper.ToJson(dt), total = JsonHelper.ToJson(dtTotal) };
                context.Response.Write(JsonHelper.ToJson(res));
            }
            else {
                var res = new { state = false, msg = "查询失败!"};
                context.Response.Write(JsonHelper.ToJson(res));
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