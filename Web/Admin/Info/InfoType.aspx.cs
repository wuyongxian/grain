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
    public partial class InfoType : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                BindData();
            }
        }

        public void BindData()
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("     select ID,strType From InfoType");
            strSql.Append("     ORDER BY ISDefault DESC,numSort ASC");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        public string GetAddItem()
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "A");
            if (Authority)
            {
                return " <a href='#' onclick='ShowFrm_Add(0)'>添加信息类型</a>";
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
                string strType = SQLHelper.ExecuteScalar("  select strType from InfoType where ID="+ID.ToString()).ToString();
                return "<input type='button'  value='查看/修改' style='width:80px; height:25px;'  onclick='ShowFrm_Update(" + ID.ToString() + ",\"" + strType.ToString() + "\")' />";
            }
            else
            {
                return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='查看/修改'  ";
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
                return "<input type='button' disabled='disabled'  value='删除' style='width:80px; height:25px;'  onclick='FunDelete(" + ID.ToString() + ")' />";
            }
        }
    }
}