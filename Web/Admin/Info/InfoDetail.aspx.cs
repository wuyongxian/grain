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
    public partial class InfoDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    BindData();
                }
            }
        }

        public void BindData()
        {


            //查询全部信息列表
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT B.strRealName AS RUserID,A.dt_Reply AS RTime,A.strContent AS RContent");
            strSql.Append("  FROM dbo.InfoReply A INNER JOIN dbo.Users B ON A.UserID=B.ID");
            strSql.Append("   WHERE InfoID=" + Request.QueryString["ID"].ToString());
            strSql.Append("  order by A.dt_Reply desc");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());


            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        public string GetEditContent()
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return "<a onclick='FunUpdate()' href='#'>[修改]</a>&nbsp; <a onclick='FunDelete()' href='#'>[删除]</a>";
            }
            else
            {
                return "";
            }
        }

    }
}