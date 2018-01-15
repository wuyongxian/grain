
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
namespace Web.BasicData.StoragePara
{
    public partial class SetStorageFee : System.Web.UI.Page
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
            strSql.Append("  select A.ID,B.strName AS TypeID,C.strName AS TimeID, D.strName AS VarietyID,numStorageFee,numUpper,numLower");
            strSql.Append("  FROM dbo.StorageFee A INNER JOIN dbo.StorageType B ON A.TypeID=B.ID");
            strSql.Append("  INNER JOIN  dbo.StorageTime C ON  A.TimeID=C.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety D ON A.VarietyID=D.ID");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            BindData();
        }

    }
}