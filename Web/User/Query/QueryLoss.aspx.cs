using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Web.User.Query
{
    public partial class QueryLoss : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }
        private void DataBind()
        {
            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();//当前网点ID

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT  A.ID,AccountNumber,A.strName,strAddress,IDCard,PhoneNO,B.strName AS CunID,C.strName AS XiangID,CASE (A.numState) WHEN 1 THEN '正常' ELSE '挂失' END AS numState");
            strSql.Append("  FROM dbo.Depositor A INNER JOIN dbo.BD_Address_Cun B ON A.CunID=B.ID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Xiang C ON B.XiangID=C.ID");
            strSql.Append("  WHERE A.numState=0");
            strSql.Append("   and A.WBID=" + WBID);
           
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                Repeater1.DataSource = dt;
                Repeater1.DataBind();
            }

        }


    }
}