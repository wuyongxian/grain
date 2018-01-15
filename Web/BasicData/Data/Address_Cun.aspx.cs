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
    public partial class Address_Cun : System.Web.UI.Page
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
 
            strSql.Append("  SELECT C.strName AS XianID,B.strName AS XiangID,A.ID,A.strName,A.ISDefault,A.numSort");
            strSql.Append("  FROM dbo.BD_Address_Cun A INNER JOIN dbo.BD_Address_Xiang B ON A.XiangID=B.ID");
             strSql.Append("  INNER JOIN dbo.BD_Address_Xian C ON B.XianID=C.ID");
             strSql.Append("  ORDER BY C.ID,B.ID,A.ID");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }


        public string GetAddItem()
        {

            if (Request.QueryString["Type"] != null)
            {
                string type = Request.QueryString["Type"].ToString();
                if (type == "1")
                {
                    return " <a href='#' onclick='ShowFrm(0)'>添加村名</a>";
                }
            }
            else
            {
                bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "A");
                if (Authority)
                {
                    return " <a href='#' onclick='ShowFrm(0)'>添加村名</a>";
                }
                else
                {
                    return "";
                }
            }
            return "";
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