using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace Web.BasicData.StoragePara
{
    public partial class StorageVarietyLevel : System.Web.UI.Page
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
            strSql.Append("   SELECT A.ID, A.VarietyID,A.VarietyLevelID, B.strName AS VarietyName, C.strName AS VarietyLevelName ");
            strSql.Append("   FROM dbo.StorageVarietyLevel_L A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSql.Append("   INNER JOIN dbo.StorageVarietyLevel_B C ON A.VarietyLevelID=C.ID");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            BindData();
        }

        public string GetAddItem()
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "A");
            if (Authority)
            {
                return " <a href='#' onclick='ShowFrm(0)'>添加存储产品等级</a>";
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

        public string GetDeleteItem(object objID, object VarietyID,object VarietyLevelID)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return "<input type='button'  value='删除'  style='width:80px; height:25px;' onclick='FunDelete(" + objID.ToString() + "," + VarietyID.ToString() + "," + VarietyLevelID.ToString() + ")' />";
            }
            else
            {
                return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='删除'  onclick='FunDelete(" + objID.ToString() + "," + VarietyID.ToString() + "," + VarietyLevelID.ToString() + ")' />";
            }
        }


    }
}