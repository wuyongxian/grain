using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.Admin.Approval
{
    public partial class AToSell : System.Web.UI.Page
    {
      
        public string Approval = "";
        public string dt_Apply = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetSellApply();
            }
            Approval = Session["strRealName"].ToString();
            dt_Apply = DateTime.Now.ToString();
            
        }
        private void GetSellApply()
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT  A.ID, B.strName AS WBID,  A.Dep_AccountNumber,A.Dep_Name,CASE (ApplyType) WHEN 1 THEN '存转销' ELSE '提取原粮' END AS ApplyType,VarietyName, VarietyCount,ApplyPrice,ApplyMoney, UnitName,");
            strSql.Append("   CONVERT(varchar(100), ApplyDate, 111) AS ApplyDate ,  CASE (ApplyState) WHEN 0 THEN '待审核' WHEN 1 THEN '审核通过' ELSE '审核不通过' END AS strApplyState,ApplyState");
            strSql.Append("  FROM dbo.StorageSellApply A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append(" where 1=1");
            strSql.Append(" and  ApplyState=0 and B.ISSimulate=0");//排除模拟账户的申请
            
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                Repeater1.DataSource = dt;
                Repeater1.DataBind();
            }

        }


        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
           GetSellApply();
        }


       
    }
}