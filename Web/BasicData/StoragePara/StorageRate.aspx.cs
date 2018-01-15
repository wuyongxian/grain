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
    public partial class StorageRate : System.Web.UI.Page
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
            strSql.Append("     select A.ID,B.strName AS TypeID,C.strName AS VarietyID,D.strName AS VarietyLevelID,E.strName AS TimeID,");
            strSql.Append("     StorageFee,BankRate,CurrentRate,EarningRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou ");
            strSql.Append("     FROM dbo.StorageRate A INNER JOIN dbo.StorageType B ON A.TypeID=B.ID");
            strSql.Append("     INNER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSql.Append("     INNER JOIN dbo.StorageVarietyLevel_B D ON A.VarietyLevelID=D.ID");
            strSql.Append("     INNER JOIN dbo.StorageTime E ON A.TimeID=E.ID");
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

        public string getEarningRate(object rate)
        {
            return rate.ToString() + "%";
        }

        public string GetAddItem()
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "A");
            if (Authority)
            {
                return " <a href='#' onclick='ShowFrm(0)'>添加价格利率</a>";
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