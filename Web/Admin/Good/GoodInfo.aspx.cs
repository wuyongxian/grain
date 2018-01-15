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
    public partial class GoodInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                GetRecord();
                BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
            }
        }

         void GetRecord()
        {
            string strCount = " SELECT COUNT(ID)  FROM dbo.Good WHERE 1=1 ";
            if (Fun.SafeData(txtstrName.Value).Trim() != "")
            {
                strCount += "     and strName like '%" + Fun.SafeData(txtstrName.Value).Trim() + "%'";
            }
            AspNetPager1.RecordCount = Convert.ToInt32(SQLHelper.ExecuteScalar(strCount));
        }

         void BindData(int pageStart,int pageSize)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("    SELECT top "+pageSize+" A.ID,A.strName,B.strName AS PackingSpecID,C.strName AS MeasuringUnit,");
            strSql.Append("      A.Price_StockHQ, A.Price_Stock,A.Price_XiaoShou , A.Price_DuiHuan,A.Price_VIP,A.Price_PiFa,A.Price_TeJia,A.numExchangeLimit,");
            strSql.Append("     CASE (A.ISIntegral) WHEN 0 THEN '非积分商品' ELSE STR(Integralvalue,18,2)  END AS Integral   ");
            strSql.Append("    FROM dbo.Good A  left outer JOIN dbo.BD_PackingSpec B ON A.PackingSpecID=B.ID");
            strSql.Append("     left outer JOIN dbo.BD_MeasuringUnit C ON A.MeasuringUnit=C.ID  ");
          
            strSql.Append("     where 1=1");
            strSql.Append(" and A.ID not in(select top " + pageStart * pageSize + " ID from Good order by ISDefault desc,numSort asc,ID asc)");
            //strSql.Append(" and D.ISCustom!=1");
            if (Fun.SafeData(txtstrName.Value).Trim() != "")
            {
                strSql.Append("     and A.strName like '%" + Fun.SafeData(txtstrName.Value).Trim() + "%'");
            }
            strSql.Append("     ORDER BY A.ISDefault DESC,A.numSort ASC,ID asc");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                this.Repeater1.DataSource = dt.DefaultView;
                this.Repeater1.DataBind();
            }
        }

        public string GetGoodStorage(object obj)
        { 
        //获取总部仓库的库存
            int WBID = Convert.ToInt32(Session["WB_ID"]);
            string strSql = " SELECT SUM(numStore) FROM dbo.GoodStorage WHERE WBID=" + WBID + " and GoodID=" + obj.ToString();
            object objValue = SQLHelper.ExecuteScalar(strSql);
            if (objValue == null || objValue.ToString() == "")
            {

                return "<span style='color:red'>不存在</span>";
            }
            else {
                //return "<a href='#' style='color:blue' onclick='ShowStorageDetail(" + obj.ToString() + ",this)'>" + objValue .ToString()+ "</a>";
                return "<a href='#' style='color:blue' class='a-sd' GoodID='" + obj.ToString() + "'>" + objValue.ToString() + "</a>";
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            GetRecord();
            BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
        }

        protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
        {
            AspNetPager1.CurrentPageIndex = e.NewPageIndex;
            BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
        }


        public string GetAddItem()
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "A");
            if (Authority)
            {
                return " <a href='#' onclick='ShowFrm(0)'>添加网点商品</a>";
            }
            else
            {
                return "";
            }
        }


        public string GetUpdateColumn(object objID) 
        {
            string strReturn = "";
            string userID = Session["ID"].ToString();
            string UserType = SQLHelper.ExecuteScalar(" SELECT A.strName FROM dbo.UserGroup A INNER JOIN dbo.Users B ON A.ID=B.UserGroup_ID WHERE B.ID="+userID).ToString();

            if (UserType == "单位管理员")
            {
                strReturn = "<input type='button'  style='width:80px;height:25px;' value='查看/修改' onclick='ShowFrm_Update("+objID+")'  />";
            }
            else
            {
                strReturn = "<input type='button' disabled='disabled' style='width:80px;height:25px;' value='查看/修改' onclick='ShowFrm_Update(" + objID + ")'  />";

            }
            return strReturn;
        }

        public string GetDeleteColumn(object objID)
        {
            string strReturn = "";
            string userID = Session["ID"].ToString();
            string UserType = SQLHelper.ExecuteScalar(" SELECT A.strName FROM dbo.UserGroup A INNER JOIN dbo.Users B ON A.ID=B.UserGroup_ID WHERE B.ID=" + userID).ToString();

            if (UserType == "单位管理员")
            {
                strReturn = "<input type='button'  style='width:60px;height:25px;' value='删除' onclick='FunDelete(" + objID + ")'  />";
            }
            else
            {
                strReturn = "<input type='button' disabled='disabled' style='width:60px;height:25px;' value='删除' onclick='FunDelete(" + objID + ")'  />";

            }
            return strReturn;
        }

        public string GetUpdateItem(object ID)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return "<input type='button'  value='查看/修改'  onclick='ShowFrm(" + ID.ToString() + ")' />";
            }
            else
            {
                return "<input type='button' disabled='disabled'  value='查看/修改'  onclick='ShowFrm(" + ID.ToString() + ")' />";
            }
        }

        public string GetDeleteItem(object ID)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return "<input type='button'  value='删除'  onclick='FunDelete(" + ID.ToString() + ")' />";
            }
            else
            {
                return "<input type='button' disabled='disabled'  value='删除'  onclick='FunDelete(" + ID.ToString() + ")' />";
            }
        }

    }
}