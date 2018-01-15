using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Web.Admin.Commune
{
    public partial class CommuneStatistics : System.Web.UI.Page
    {
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               // DataBind();
            }
        }
      
        private void DataBind()
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.strName AS  WBID,  COUNT(A.ID)AS CommuneCount ,SUM(FieldCount) AS FieldCount");
            strSql.Append(" FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID");

            strSql.Append("  where 1=1 and B.ISSimulate=0");

            if (Request.Form["QWBID"] != null && Request.Form["QWBID"].ToString() != "")
            {
                strSql.Append("   AND B.ID = " + Request.Form["QWBID"].ToString());
            }
            
    
            if (Qdtstart.Value.Trim() != "")
            {
                strSql.Append("   AND A.dt_Commune> '" + Qdtstart.Value.Trim() + "'");
            }
            if (Qdtend.Value.Trim() != "")
            {
                strSql.Append("   AND A.dt_Commune < '" + Qdtend.Value.Trim() + "'");
            }

            strSql.Append("   GROUP BY B.strName");
            strSql.Append("  ");


            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();



            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("  SELECT COUNT(A.ID)AS CommuneCount ,SUM(A.FieldCount) AS FieldCount");
            strSql2.Append(" FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID");

            strSql2.Append("  where 1=1 and B.ISSimulate=0");
            if (Request.Form["QWBID"] != null && Request.Form["QWBID"].ToString() != "")
            {
                strSql2.Append("   AND WBID = " + Request.Form["QWBID"].ToString());
            }
            if (Qdtstart.Value.Trim() != "")
            {
                strSql2.Append("   AND dt_Commune> '" + Qdtstart.Value.Trim() + "'");
            }
            if (Qdtend.Value.Trim() != "")
            {
                strSql2.Append("   AND dt_Commune < '" + Qdtend.Value.Trim() + "'");
            }


            DataTable dt2 = SQLHelper.ExecuteDataTable(strSql2.ToString());
            Repeater2.DataSource = dt2;
            Repeater2.DataBind();
            //合计数据


        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

            DataBind();

        }

      
    }
}