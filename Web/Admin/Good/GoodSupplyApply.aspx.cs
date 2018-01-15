using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.Admin.Good
{
    public partial class GoodSupplyApply : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                // DataBind();
                Qdtend.Value = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                Qdtstart.Value = DateTime.Now.AddMonths(-1).AddDays(1).ToString("yyyy-MM-dd");
            }
        }
        private void DataBind()
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select A.ID,B.ID as WBID, B.strName AS  WBName,C.ID as GoodSupplyID, C.strName AS  GoodSupplyName,A.Price,A.Price_WB, A.Price_WBBack,  Price_Money,  Quantity");
            strSql.Append("   FROM dbo.GoodSupplyApply   A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql.Append("  WHERE A.ISHQ=0");//排除总部
            strSql.Append("  and B.ISSimulate=0");//排除模拟网点
            strSql.Append("  and A.numState=0");//只查询未经过批准的申请
            if (Request.Form["QWBID"] != null && Request.Form["QWBID"].ToString() != "")
            {
                strSql.Append("   AND B.ID = " + Request.Form["QWBID"].ToString());
            }
            if (Qdtstart.Value.Trim() != "")
            {
                strSql.Append("   AND A.dt_Trade> '" + Qdtstart.Value.Trim() + "'");
            }
            if (Qdtend.Value.Trim() != "")
            {
                strSql.Append("   AND A.dt_Trade < '" + Qdtend.Value.Trim() + "'");
            }

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

            DataBind();

        }

        public string GetApplyItem(object ID)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return "  <input type='button' value='批准' onclick='FunUpdate("+ID.ToString()+",1)' style='width:60px;height:25px;' />&nbsp; <input type='button' value='撤销' onclick='FunUpdate("+ID.ToString()+",-1)' style='width:60px' />";
            }
            else
            {
                return "  <input type='button' value='批准' disabled='disabled' style='width:60px;height:25px;' />&nbsp; <input type='button' value='撤销'disabled='disabled' style='width:60px' />";
            }
        }
    }
}