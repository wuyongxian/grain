using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace Web.BasicData.Data
{
    public partial class Address_Zu : System.Web.UI.Page
    {
        bool ISHQ = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                ISHQ = common.ISHQWB(Session["WB_ID"]);
                GetRecord();
                BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
            }
        }

        void GetRecord()
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("  SELECT Count(A.ID)");
            strSql.Append("  FROM dbo.BD_Address_Zu A INNER JOIN dbo.BD_Address_Cun B ON A.CunID=B.ID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Xiang C ON B.XiangID=C.ID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Xian D ON C.XianID=D.ID");
            strSql.Append("     where 1=1");
           
            if (Fun.SafeData(txtXian.Value).Trim() != "")
            {
                strSql.Append("     and D.strName like '%" + Fun.SafeData(txtXian.Value).Trim() + "%'");
            }
            if (Fun.SafeData(txtXiang.Value).Trim() != "")
            {
                strSql.Append("     and C.strName like '%" + Fun.SafeData(txtXiang.Value).Trim() + "%'");
            }
            if (Fun.SafeData(txtCun.Value).Trim() != "")
            {
                strSql.Append("     and B.strName like '%" + Fun.SafeData(txtCun.Value).Trim() + "%'");
            }
            AspNetPager1.RecordCount = Convert.ToInt32(SQLHelper.ExecuteScalar(strSql.ToString()));
        }

        public void BindData(int pageStart, int pageSize)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("  SELECT top "+pageSize+" A.ID, D.strName AS XianID,C.strName AS XiangID,B.strName AS CunID,A.strName");
            strSql.Append("  FROM dbo.BD_Address_Zu A INNER JOIN dbo.BD_Address_Cun B ON A.CunID=B.ID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Xiang C ON B.XiangID=C.ID");
            strSql.Append("  INNER JOIN dbo.BD_Address_Xian D ON C.XianID=D.ID");
            strSql.Append("     where 1=1");
            strSql.Append("  and A.ID not in(select top " + pageStart * pageSize + " ID from BD_Address_Zu)");

            if (Fun.SafeData( txtXian.Value).Trim() != "")
            {
                strSql.Append("     and D.strName like '%" + Fun.SafeData(txtXian.Value).Trim() + "%'");
            }
            if (Fun.SafeData(txtXiang.Value).Trim() != "")
            {
                strSql.Append("     and C.strName like '%" + Fun.SafeData(txtXiang.Value).Trim() + "%'");
            }
            if (Fun.SafeData(txtCun.Value).Trim() != "")
            {
                strSql.Append("     and B.strName like '%" + Fun.SafeData(txtCun.Value).Trim() + "%'");
            }

            strSql.Append("  ORDER BY D.ID,C.ID,B.ID,A.ID");
            strSql.Append("  ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
        {
            ISHQ = common.ISHQWB(Session["WB_ID"]);
            AspNetPager1.CurrentPageIndex = e.NewPageIndex;
            BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            GetRecord();
            BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
        }

        public string GetAddItem()
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "A");
            if (Authority)
            {
                return " <a href='#' onclick='ShowFrm(0)'>添加组名</a>";
            }
            else
            {
                return "";
            }
        }

      
        public string GetUpdateItem(object ID)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return "<input type='button'  value='查看/修改' style='width:80px; height:25px;'  onclick='ShowFrm(" + ID.ToString() + ")' />";
            }
            else
            {
                return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='查看/修改'  onclick='ShowFrm(" + ID.ToString() + ")' />";
            }
        }

        public string GetDeleteItem(object ID)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return "<input type='button'  value='删除' style='width:80px; height:25px;'  onclick='FunDelete(" + ID.ToString() + ")' />";
            }
            else
            {
                return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='删除'  onclick='FunDelete(" + ID.ToString() + ")' />";
            }
        }

    }
}