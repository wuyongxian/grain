using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;


namespace Web.Admin.WebSiteSetting
{
    public partial class WBControl : System.Web.UI.Page
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
            strSql.Append("  SELECT WB.ID,WB.SerialNumber,strName,strAddress, dbo.WBType.strType AS  WBType_ID,numSettle,numAgent,ISAllowBackUp,ISHQ,ISSimulate ");
            strSql.Append("  FROM dbo.WB INNER JOIN dbo.WBType ON dbo.WB.WBType_ID=dbo.WBType.ID");
            if (txtType.Value.Trim() != "")
            {
                strSql.Append("  WHERE strName='" + txtType.Value.Trim() + "'");
            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        public string SetISAllowBackUp(object flag)
        {

            if (Convert.ToBoolean(flag) == true)
            {
                return "<span style='color:red;'>是</span>";
            }
            else
            {
                return "<span style='color:#555;'>否</span>";
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            BindData();
        }

        public string SetISHQ(object obj,object ISSimulate)
        {
            if (Convert.ToBoolean( obj) == true)
            {
                return "<span style='color:red'>(总部)</span>";
            }
            if (ISSimulate != null)
            {
                if (Convert.ToBoolean(ISSimulate) == true)
                {
                    return "<span style='color:#666;'>(模拟)</span>";
                }
            }
            return "";
        }

        public string GetAddItem()
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "A");
            if (Authority)
            {
                return " <a href='#' onclick='ShowFrm(0)'>添加新网点</a>";
            }
            else
            {
                return "";
            }
        }

        public string GetDelItem(object ID, object ISHQ)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                if (Convert.ToBoolean( ISHQ)==true)
                {
                    return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='删除' />";
                }
                else {
                    return "<input type='button'  value='删除' style='width:80px; height:25px;'  onclick='WBDelete(" + ID.ToString() + ")' />";
                }
            
            }
            else
            {
                return "<input type='button' disabled='disabled'  value='删除' />";
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

    }
}