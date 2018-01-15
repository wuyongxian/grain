using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace Web.BasicData.Charges
{
    public partial class SetExchangePara : System.Web.UI.Page
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

            strSql.Append("  select A.ID,B.strName AS TypeID,C.strName AS TimeID,D.strName AS VarietyID,E.strName AS GoodID,ChuFenLv,FuPi,JiaGongFei");
            strSql.Append("  FROM dbo.GoodExchangeProp A INNER JOIN dbo.StorageType B ON A.TypeID=B.ID");
            strSql.Append("  INNER JOIN dbo.StorageTime C ON A.TimeID=C.ID ");
            strSql.Append("  INNER JOIN dbo.StorageVariety D ON A.VarietyID=D.ID");
            strSql.Append("  INNER JOIN dbo.Good E ON A.GoodID=E.ID");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }
    }
}