using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace Web.Admin.Good
{
    public partial class Supplier : System.Web.UI.Page
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

            string WBID = Session["WB_ID"].ToString();
            strSql.Append("     select ID,WB_ID,strName,LinkMan,LinkManDuty,Address,PostCode,PhoneNO,Mobile,strRemark ");
            strSql.Append("     FROM dbo.WBSupplier");
            strSql.Append("     where WB_ID=" + WBID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
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