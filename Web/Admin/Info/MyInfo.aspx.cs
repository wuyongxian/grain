using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;


namespace Web.Admin.Info
{
    public partial class MyInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                int UserID = Convert.ToInt32(Session["ID"]);
                StringBuilder strSqlCount = new StringBuilder();
                strSqlCount.Append("  SELECT Count(ID)");
                strSqlCount.Append("  FROM dbo.Info");
                strSqlCount.Append("   WHERE UserID=" + UserID);
                AspNetPager1.PageSize = 10;
                AspNetPager1.RecordCount = Convert.ToInt32(SQLHelper.ExecuteScalar(strSqlCount.ToString()));

                BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
            }
        }

        public void BindData(int pageStart, int pageSize)
        {
            int UserID = Convert.ToInt32(Session["ID"]);
            //查询全部信息列表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT top " + pageSize + " A.ID,C.strType AS InfoTypeID,B.strRealName AS UserID,");
            strSql.Append("  A.strTitle,A.strContent,A.BrowseTime,A.dt_Add,A.ISStick");
            strSql.Append("  FROM dbo.Info A INNER JOIN dbo.Users B ON A.UserID=B.ID");
            strSql.Append("  INNER JOIN dbo.InfoType C ON A.InfoTypeID=C.ID");
            strSql.Append("   WHERE B.ID=" + UserID);
            strSql.Append(" and A.ID not in(select top " + pageStart * pageSize + " ID from Info)");
            strSql.Append("  ORDER BY A.ISStick DESC,A.dt_Add DESC");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }


        public string ISStick(object obj)
        {
            if (Convert.ToBoolean(obj) == true)
            {
                return "<span style='color:green'>[置顶]</span>";
            }
            else
            {
                return "";
            }
        }

        protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
        {
            AspNetPager1.CurrentPageIndex = e.NewPageIndex;
            BindData(AspNetPager1.CurrentPageIndex, AspNetPager1.PageSize);
        }

      

       
    }
}