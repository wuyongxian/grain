using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Web.User.Commune
{
    public partial class BusinessStatistics : System.Web.UI.Page
    {
        public double Money_Youhui = 0;
        public double Money_Yufukuan = 0;
        public double Money_Shifu = 0;
        public double Moeny_Fanli = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // DataBind();
            }
        }

        private void DataBind()
        {

            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.strName AS WBID,C.strName AS GoodSupplyID, C.Price,A.numDisCount , A.UnitName,");
            strSql.Append("  SUM(GoodSupplyCount)AS GoodSupplyCount,SUM(Money_Total) AS Money_Total,SUM(Money_YouHui) AS Money_YouHui,SUM(Money_PreDefine) AS Money_PreDefine,SUM(Money_Reality) AS Money_Reality,SUM(Money_Back) AS Money_Back");
            strSql.Append("  FROM dbo.C_Supply A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ID="+WBID);
           
            if (Qdtstart.Value.Trim() != "")
            {
                strSql.Append("   AND A.dt_Trade> '" + Qdtstart.Value.Trim() + "'");
            }
            if (Qdtend.Value.Trim() != "")
            {
                strSql.Append("   AND A.dt_Trade < '" + Qdtend.Value.Trim() + "'");
            }

            strSql.Append("   GROUP BY B.strName ,C.strName,C.Price,A.numDisCount,A.UnitName");
            strSql.Append("  ");


            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();


        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

            DataBind();

        }


        DataTable GetoutputTable()
        {
            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.strName AS WBID,C.strName AS GoodSupplyID, C.Price,STR(A.numDisCount,10,2)+'%'  as numDisCount, A.UnitName,");
            strSql.Append("  SUM(GoodSupplyCount)AS GoodSupplyCount,SUM(Money_Total) AS Money_Total,SUM(Money_YouHui) AS Money_YouHui,SUM(Money_PreDefine) AS Money_PreDefine,SUM(Money_Reality) AS Money_Reality");
            strSql.Append("  FROM dbo.C_Supply A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ID=" + WBID);
          
            if (Qdtstart.Value.Trim() != "")
            {
                strSql.Append("   AND A.dt_Trade> '" + Qdtstart.Value.Trim() + "'");
            }
            if (Qdtend.Value.Trim() != "")
            {
                strSql.Append("   AND A.dt_Trade < '" + Qdtend.Value.Trim() + "'");
            }

            strSql.Append("   GROUP BY B.strName ,C.strName,C.Price,A.numDisCount,A.UnitName");
            strSql.Append("  ");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return SQLHelper.ExecuteDataTable(strSql.ToString());
            }
        }

        private void outputExcel()
        {
            DataTable dt = GetoutputTable();
            if (dt == null || dt.Rows.Count == 0)
            {
                Fun.Alert("请先检索要查询的信息后在导出！");
                return;
            }

            string fileName = "业务统计" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".xls";

            HttpResponse resp;
            resp = Page.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            string colHeaders = "", ls_item = "", ls_Bottom = "";

            DataRow[] myRow = dt.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
            int i = 0;
            int cl = dt.Columns.Count;

            colHeaders += "网点\t商品种类\t商品单价\t折率\t计量单位\t购买数量\t商品总价\t优惠金额\t预付款金额\t实付金额\n";
            resp.Write(colHeaders);
            //向HTTP输出流中写入取得的数据信息

            //逐行处理数据  
            foreach (DataRow row in myRow)
            {
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据    
                for (i = 0; i < cl; i++)
                {
                    if (i == (cl - 1))//最后一列，加\n
                    {
                        ls_item += row[i].ToString() + "\n";
                    }
                    else
                    {
                        ls_item += row[i].ToString() + "\t";
                    }
                }
                resp.Write(ls_item);
                ls_item = "";

            }


            resp.End();
        }

        protected void btnOutPut_Click(object sender, EventArgs e)
        {
            outputExcel();
        }


    }
}