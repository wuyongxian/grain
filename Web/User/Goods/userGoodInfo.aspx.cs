using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace Web.User.Goods
{
    public partial class userGoodInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                GetRecord();
                BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
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
            strSql.Append("     (STR( A.Price_TeJia,10,2) +'元/'+C.strName) AS Price_TeJia ,");
            strSql.Append("     (STR( A.numExchangeLimit,10,2) +C.strName) AS numExchangeLimit ");
            strSql.Append("     FROM dbo.Good A  left outer JOIN dbo.BD_PackingSpec B ON A.PackingSpecID=B.ID");
            strSql.Append("      left outer JOIN dbo.BD_MeasuringUnit C ON A.MeasuringUnit=C.ID");
            strSql.Append("     where 1=1");
            strSql.Append(" and A.ID not in(select top " + pageStart * pageSize + " ID from Good order by ISDefault desc,numSort asc,ID asc)");
            if (Fun.SafeData(txtstrName.Value).Trim() != "")
            {
                strSql.Append("     and A.strName like '%" + Fun.SafeData(txtstrName.Value).Trim() + "%'");
            }
            strSql.Append("     ORDER BY A.ISDefault DESC,A.numSort ASC,ID asc");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        public string GetGoodStorage(object obj)
        {
            //获取网点仓库的库存
            int WBID = Convert.ToInt32(Session["WB_ID"]);
            string strSql = " SELECT SUM(numStore) FROM dbo.GoodStorage WHERE WBID=" + WBID + " and GoodID=" + obj.ToString();
            object objValue = SQLHelper.ExecuteScalar(strSql);
            if (objValue == null || objValue.ToString() == "")
            {

                return "<span style='color:red'>不存在</span>";
            }
            else
            {
                //return "<a href='#' style='color:blue' onclick='ShowStorageDetail(" + obj.ToString() + ",this)'>" + objValue .ToString()+ "</a>";
                return "<a href='#' style='color:blue' class='a-sd' GoodID='" + obj.ToString() + "'>" + objValue.ToString() + "</a>";
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
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