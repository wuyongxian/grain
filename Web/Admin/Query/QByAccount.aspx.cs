using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Web.Admin.Query
{
    public partial class QByAccount : System.Web.UI.Page
    {
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        private void DataBind(int pageindex, int pagesize)
        {

            if (QAccountNumber.Value.Trim() == "")
            {
                string StrScript;
                StrScript = ("<script language=javascript>");
                StrScript += ("alert('请输入查询条件!');");
                StrScript += ("</script>");
                System.Web.HttpContext.Current.Response.Write(StrScript);
                return;
            }

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT top " + pagesize + " A.ID,H.strName as WBID, B.AccountNumber,B.strName,C.strName AS CunID,CONVERT(varchar(100), A.StorageDate, 111) AS StorageDate,D.strName AS TypeID, E.strName AS TimeID,F.strName AS VarietyID,A.StorageNumber");
            strSql.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSql.Append("  INNER JOIN dbo.WB H ON H.ID =B.WBID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Cun C ON B.CunID =C.ID");
            strSql.Append(" INNER JOIN dbo.StorageType D ON A.TypeID=D.ID ");
            strSql.Append("  INNER JOIN dbo.StorageTime E ON A.TimeID=E.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety F ON A.VarietyID=F.ID");
            strSql.Append("  WHERE 1=1 ");

            strSql.Append("  AND A.ID NOT IN (SELECT TOP " + pageindex * pagesize + " ID FROM dbo.Dep_StorageInfo ORDER BY A.StorageDate DESC,B.AccountNumber ASC )");

            //if (QCunName.Value.Trim() != "")
            //{
            //    strSql.Append("   AND C.strName LIKE '%" + QCunName.Value.Trim() + "%'");
            //}
            if (QAccountNumber.Value.Trim() != "")
            {
                strSql.Append("   AND B.AccountNumber = '" + QAccountNumber.Value.Trim() + "'");
            }
            //if (QName.Value.Trim() != "")
            //{
            //    strSql.Append("   AND B.strName LIKE '%" + QName.Value.Trim() + "%'");
            //}
            //if (QIDCard.Value.Trim() != "")
            //{
            //    strSql.Append("   AND B.IDCard LIKE '%" + QIDCard.Value.Trim() + "%'");
            //}
            // strSql.Append("  ORDER BY A.StorageDate DESC,B.AccountNumber ASC");
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();

        }


        private void GetDataCount()
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT COUNT(A.ID)");
            strSql.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.Depositor B ON A.AccountNumber=B.AccountNumber");
            strSql.Append("  INNER JOIN dbo.BD_Address_Cun C ON B.CunID =C.ID");
            strSql.Append("  WHERE 1=1 ");

            //if (QCunName.Value.Trim() != "")
            //{
            //    strSql.Append("   AND C.strName LIKE '%" + QCunName.Value.Trim() + "%'");
            //}
            if (QAccountNumber.Value.Trim() != "")
            {
                strSql.Append("   AND B.AccountNumber = '" + QAccountNumber.Value.Trim() + "'");
            }
            //if (QName.Value.Trim() != "")
            //{
            //    strSql.Append("   AND B.strName LIKE '%" + QName.Value.Trim() + "%'");
            //}
            //if (QIDCard.Value.Trim() != "")
            //{
            //    strSql.Append("   AND B.IDCard LIKE '%" + QIDCard.Value.Trim() + "%'");
            //}

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
            AspNetPager1.RecordCount = Convert.ToInt32(obj);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            GetDataCount();
            DataBind(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);

        }

        protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
        {
            AspNetPager1.CurrentPageIndex = e.NewPageIndex;
            DataBind(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
        }

    }
}