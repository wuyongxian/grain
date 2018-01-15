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
    public partial class SettleList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind("");
            }
        }
        private void DataBind(string WBName)
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT ID,strName,ISHQ,ISSimulate FROM dbo.WB");
            strSql.Append("  WHERE ISSimulate=0 AND ISHQ=0");
            if (WBName.ToString().Trim() != "")
            {
                strSql.Append("   and strName LIKE '%" + WBName + "%'");
            }



            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();

        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            DataBind(txtWBID.Value);
        }
    }
}