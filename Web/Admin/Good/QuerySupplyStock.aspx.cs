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
    public partial class QuerySupplyStock : System.Web.UI.Page
    {
        public double KuCunJinET = 0;
        public double FanLiJineET = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
            //    GetRecord();
            //    BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
            }
        }

        void GetRecord()
        {
            string strCount = " SELECT COUNT(ID)  FROM dbo.GoodSupply WHERE 1=1 ";
            if (Fun.SafeData(txtstrName.Value).Trim() != "")
            {
                strCount += "     and strName like '%" + Fun.SafeData(txtstrName.Value).Trim() + "%'";
            }
            AspNetPager1.RecordCount = Convert.ToInt32(SQLHelper.ExecuteScalar(strCount));
        }

        void BindData(int pageStart, int pageSize)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("  select A.ID,A.strName,A.BarCode,B.strName AS SpecID,A.Price as numPrice, (STR( A.Price,10,2) +'元/'+C.strName) AS Price ,(STR( A.Price_WB,10,2) +'元/'+C.strName) AS Price_WB ,A.Price_WBBack as numPrice_WBBack,(STR( A.Price_WBBack,10,2) +'元/'+C.strName) AS Price_WBBack ,(STR( A.Price_Commune,10,2) +'元/'+C.strName) AS Price_Commune ,");
            strSql.Append(" (STR( A.numDiscount,10,2) +'%') AS  numDiscount, CASE (ISPreDefine) WHEN 1 THEN '是' ELSE '否' END AS ISPreDefine");
            strSql.Append("  FROM dbo.GoodSupply A INNER JOIN  dbo.BD_PackingSpec B ON  A.SpecID=B.ID");
            strSql.Append("  INNER JOIN dbo.BD_MeasuringUnit C ON A.UnitID=C.ID");

            strSql.Append("     where 1=1");
            strSql.Append(" and A.ID not in(select top " + pageStart * pageSize + " ID from GoodSupply)");
            if (Fun.SafeData(txtstrName.Value).Trim() != "")
            {
                strSql.Append("     and A.strName like '%" + Fun.SafeData(txtstrName.Value).Trim() + "%'");
            }
            strSql.Append("     ORDER BY A.ISDefault DESC,A.numSort ASC");
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

            string WBID = Request.Form["QWBID"].ToString();
            string strSql = " SELECT TOP 1 numStore FROM dbo.GoodSupplyStorage WHERE WBID=" + WBID + " and GoodSupplyID=" + obj.ToString();
            object objValue = SQLHelper.ExecuteScalar(strSql);
            if (objValue == null || objValue.ToString() == "")
            {

                return "<span style='color:red'>不存在</span>";
            }
            else
            {
                return objValue.ToString();
            }
        }


        public string KunCunJinE(object obj,object objPrice)
        {
            //获取总部仓库的库存

            string WBID = Request.Form["QWBID"].ToString();
            string strSql = " SELECT TOP 1 numStore FROM dbo.GoodSupplyStorage WHERE WBID=" + WBID + " and GoodSupplyID=" + obj.ToString();
            object objValue = SQLHelper.ExecuteScalar(strSql);
            if (objValue == null || objValue.ToString() == "")
            {

                return "<span style='color:red'>0</span>";
            }
            else
            {
                double JinE = Convert.ToDouble(objValue) * Convert.ToDouble(objPrice);
                KuCunJinET += JinE;
                return Math.Round(JinE,2).ToString();
            }
        }

        public string FanLiJinE(object obj, object objPrice_Back)
        {
            //获取总部仓库的库存

            string WBID = Request.Form["QWBID"].ToString();
            string strSql = " SELECT TOP 1 numStore FROM dbo.GoodSupplyStorage WHERE WBID=" + WBID + " and GoodSupplyID=" + obj.ToString();
            object objValue = SQLHelper.ExecuteScalar(strSql);
            if (objValue == null || objValue.ToString() == "")
            {

                return "<span style='color:red'>0</span>";
            }
            else
            {
                double JinE = Convert.ToDouble(objValue) * Convert.ToDouble(objPrice_Back);
                FanLiJineET += JinE;
                return Math.Round(JinE, 2).ToString();
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (Request.Form["QWBID"] == null || Request.Form["QWBID"].ToString() == "")
            {
                Fun.Alert("请选择网点！");
                return;
            }
            GetRecord();
            BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
        }

        protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
        {
            AspNetPager1.CurrentPageIndex = e.NewPageIndex;
            BindData(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
        }


        public string GetUpdateItem(object GoodSupplyID)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");


            string WBID = Request.Form["QWBID"].ToString();
            string strSql = " SELECT TOP 1 numStore FROM dbo.GoodSupplyStorage WHERE WBID=" + WBID + " and GoodSupplyID=" + GoodSupplyID.ToString();
            object objValue = SQLHelper.ExecuteScalar(strSql);
            if (objValue == null || objValue.ToString() == "")
            {

                return "<input type='button' disabled='disabled'  value='修改库存'  />";
            }
            else
            {
                if (Authority)
                {
                    return "<input type='button'  value='修改库存' style='width:80px;height:25px;'  onclick='ShowUpdateQuantity(" + WBID.ToString() + "," + GoodSupplyID.ToString() + "," + objValue.ToString() + ")' />";
                }
                else
                {
                    return "<input type='button' disabled='disabled' style='width:80px;height:25px;' value='修改库存'  />";
                }
            }

        }

    }
}