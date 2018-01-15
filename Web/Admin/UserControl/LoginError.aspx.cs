using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;


namespace Web.Admin.UserControl
{
    public partial class LoginError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetDataCount();
                DataBind(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
            }
        }

        private void GetDataCount()
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT COUNT(A.ID)");
            strSql.Append(" FROM dbo.UserOperate A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append(" INNER JOIN dbo.Users C ON A.UserID=C.ID");
            strSql.Append(" WHERE 1=1 AND ErrorTime!=0 ");
            
            if (WBName.Value.Trim() != "")
            {
                strSql.Append(" and B.strName like '%"+WBName.Value.Trim()+"%' ");
            }
            if (UserLoginName.Value.Trim() != "") {
                strSql.Append(" and C.strLoginName like '%" + WBName.Value.Trim() + "%' ");
            }

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
            AspNetPager1.RecordCount = Convert.ToInt32(obj);
        }

        public void DataBind(int pageindex, int pagesize)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSqlPara = new StringBuilder();
            strSql.Append(" SELECT TOP " + pagesize + " A.ID,A.WBID,A.UserID, B.strName AS WBName,C.strLoginName AS LoginName,C.strRealName AS RealName,A.dt_LoginIn,A.ErrorTime");

            strSqlPara.Append(" FROM dbo.UserOperate A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlPara.Append(" INNER JOIN dbo.Users C ON A.UserID=C.ID");
           
            strSqlPara.Append(" WHERE 1=1 AND ErrorTime!=0  ");

            if (WBName.Value.Trim() != "")
            {
                strSqlPara.Append(" and B.strName like '%" + WBName.Value.Trim() + "%' ");
            }
            if (UserLoginName.Value.Trim() != "")
            {
                strSqlPara.Append(" and C.strLoginName like '%" + UserLoginName.Value.Trim() + "%' ");
            }
            strSql.Append(strSqlPara.ToString());

            strSql.Append(" and A.ID NOT IN( select top " + pageindex * pagesize + " A.ID");
            strSql.Append(strSqlPara.ToString());
            strSql.Append(" ORDER BY A.dt_LoginIn desc)");

            strSql.Append(" ORDER BY A.dt_LoginIn desc");

           

            
          
          

            strSql.Append(" ");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            this.Repeater1.DataSource = dt.DefaultView;
            this.Repeater1.DataBind();
        }

        public string GetState(object objErrorTime, object dt_LoginIn)
        {
            TimeSpan ts = DateTime.Now.Subtract(Convert.ToDateTime(dt_LoginIn));
            if (Convert.ToInt32(objErrorTime) >= 3)
            {
                if (ts.TotalHours < 24)
                {
                    return "<span style='color:red;'>禁用</span>";
                }
                else
                {
                    return "<span style='color:#555;'>正常</span>";

                }
            }
            else {
                return "<span style='color:#555;'>正常</span>";
            }

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