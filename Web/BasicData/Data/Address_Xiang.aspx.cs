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
    public partial class Address_Xiang : System.Web.UI.Page
    {
        bool ISHQ = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                ISHQ = common.ISHQWB(Session["WB_ID"]);
                BindData();
            }
        }

        public void BindData()
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("  SELECT  A.ID, B.strName AS XianID,A.strName");
            strSql.Append("  FROM dbo.BD_Address_Xiang A INNER JOIN  dbo.BD_Address_Xian B ON A.XianID=B.ID");
            strSql.Append("  ORDER BY A.numSort");
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
                return " <a href='#' onclick='ShowFrm(0)'>添加乡名</a>";
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
                return "<input type='button'  value='删除'  style='width:80px; height:25px;' onclick='FunDelete(" + ID.ToString() + ")' />";
            }
            else
            {
                return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='删除'  onclick='FunDelete(" + ID.ToString() + ")' />";
            }
        }

    }
}