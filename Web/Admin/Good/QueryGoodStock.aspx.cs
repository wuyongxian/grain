using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;


namespace Web.Admin.Good
{
    public partial class QueryGoodStock : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                //GetRecord();
                //BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
            }
        }

        void GetRecord()
        {
            string strCount = " SELECT COUNT(ID)  FROM dbo.Good WHERE 1=1 ";
            if (Fun.SafeData(txtstrName.Value).Trim() != "")
            {
                strCount += "     and strName like '%" + Fun.SafeData(txtstrName.Value).Trim() + "%'";
            }
            AspNetPager1.RecordCount = Convert.ToInt32(SQLHelper.ExecuteScalar(strCount));
        }

        void BindData(int pageStart, int pageSize)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("    SELECT top " + pageSize + " A.ID,A.strName,B.strName AS PackingSpecID,C.strName AS MeasuringUnit,");
            strSql.Append("     (STR( A.Price_Stock,10,2) +'元/'+C.strName) AS Price_Stock ,");
            strSql.Append("     (STR( A.Price_XiaoShou,10,2) +'元/'+C.strName) AS Price_XiaoShou ,");
            strSql.Append("    (STR( A.Price_DuiHuan,10,2) +'元/'+C.strName) AS Price_DuiHuan ,");
            strSql.Append("    (STR( A.Price_VIP,10,2) +'元/'+C.strName) AS Price_VIP ,");
            strSql.Append("     (STR( A.Price_PiFa,10,2) +'元/'+C.strName) AS Price_PiFa ,");
            strSql.Append("     (STR( A.Price_TeJia,10,2) +'元/'+C.strName) AS Price_TeJia ");
            strSql.Append("     ,S.ID AS GoodStorageID, S.numStore,W.strName AS WBWareHouseName ");
            strSql.Append("     FROM dbo.Good A LEFT OUTER JOIN dbo.BD_PackingSpec B ON A.PackingSpecID=B.ID");
            strSql.Append("     LEFT OUTER JOIN dbo.BD_MeasuringUnit C ON A.MeasuringUnit=C.ID");
            strSql.Append("     INNER JOIN dbo.GoodStorage S ON A.ID=S.GoodID");
            strSql.Append("    LEFT OUTER JOIN dbo.WBWareHouse W ON S.WBWareHouseID=W.ID");
            strSql.Append("     where 1=1");
            strSql.Append("     and S.WBID=" + Request.Form["QWBID"].ToString());
            strSql.Append(" and A.ID not in(");
            strSql.Append("  select top  " + pageStart * pageSize + " G.ID from Good G INNER JOIN dbo.GoodStorage GS ON G.ID=GS.GoodID ");
            strSql.Append("  where  GS.WBID=" + Request.Form["QWBID"].ToString() + "  order by G.ID");
            strSql.Append(" )");
            if (Fun.SafeData(txtstrName.Value).Trim() != "")
            {
                strSql.Append("     and A.strName like '%" + Fun.SafeData(txtstrName.Value).Trim() + "%'");
            }
            strSql.Append("  order by  A.ID");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }



        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (Request.Form["QWBID"] == null || Request.Form["QWBID"].ToString()=="")
            {
                Fun.Alert("请选择网点！");
                return;
            }
         
            GetRecord();
            BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
          
        }

        protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
        {
            AspNetPager1.CurrentPageIndex = e.NewPageIndex;
            BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);

        }


    }
}