using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace Web.User.Log
{
    public partial class LogOnline : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        public void BindData()
        {
            StringBuilder strSql = new StringBuilder();
            string WBID = Session["WB_ID"].ToString();
            strSql.Append("   select top 100 A.ID,B.strRealName AS UserID,dt_LoginIn,dt_LoginOut,IpAddress,TimeLength ");
            strSql.Append("  FROM dbo.UserOperate  A INNER JOIN dbo.Users B ON A.UserID=B.ID");
            strSql.Append("  WHERE WBID="+WBID);
            strSql.Append(" ORDER BY dt_LoginIn DESC");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }
    }
}