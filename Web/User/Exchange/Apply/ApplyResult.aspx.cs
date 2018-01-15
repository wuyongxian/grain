using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Web.User.Exchange.Apply
{
    public partial class ApplyResult : System.Web.UI.Page
    {
        public double numTotol = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Account"] != null)
                {
                    GetSellApply(Request.QueryString["Account"].ToString());
                }
            }
        }
        private void GetSellApply(string AccountNumber)
        {
            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();//当前网点ID

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT ID,  Dep_AccountNumber,Dep_Name,CASE (ApplyType) WHEN 1 THEN '存转销' ELSE '提取原粮' END AS ApplyType,VarietyName, VarietyCount,Dep_SID,");
            strSql.Append("  CONVERT(varchar(100), ApplyDate, 111) AS ApplyDate ,  CASE (ApplyState) WHEN 0 THEN '待审核' WHEN 1 THEN '审核通过' ELSE '审核不通过' END AS strApplyState,ApplyState");
            strSql.Append("  FROM dbo.StorageSellApply WHERE 1=1");

            strSql.Append("   and WBID="+WBID);
            if (AccountNumber != "")
            {
                strSql.Append("   AND Dep_AccountNumber='"+AccountNumber+"'");
            }
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
            
                GetSellApply(QAccountNumber.Value.Trim());
           
        }


        public string GetLink(object ApplyID, object Dep_SID, object obj)
        {
            string strReturn = "";
            if (obj.ToString() == "1")
            { //审核通过
                //strReturn = " <a   href='/User/Exchange/StoreToSell.aspx?Dep_SID=" + Dep_SID.ToString() + "&ApplyID="+ApplyID+"'>存转销</a>";
                strReturn = " <a   href='/User/Exchange/storagesell.html?ApplyID=" + ApplyID + "'>存转销</a>";
            }
            else {
                strReturn = " <a  style='display:none' href='/User/Exchange/StoreToSell.aspx?Dep_SID="+Dep_SID.ToString()+"'>存转销</a>";
            }
            return strReturn;
        }
      

    }
}