using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.Admin.Commune
{
    public partial class BusinessStatistics : System.Web.UI.Page
    
    {
        public double Money_Youhui=0;
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

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.strName AS WBID,C.strName AS GoodSupplyID, C.Price, A.UnitName,");
            strSql.Append("  SUM(GoodSupplyCount)AS GoodSupplyCount,SUM(Money_Total) AS Money_Total,SUM(Money_YouHui) AS Money_YouHui,SUM(Money_PreDefine) AS Money_PreDefine,SUM(Money_Reality) AS Money_Reality,SUM(Money_Back) AS Money_Back");
            strSql.Append("  FROM dbo.C_Supply A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");

            if (Request.Form["QWBID"] != null&&Request.Form["QWBID"].ToString()!="")
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

            strSql.Append("   GROUP BY B.strName ,C.strName,C.Price,A.UnitName");
            strSql.Append("  ");


            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();



            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("  SELECT C.strName AS GoodSupplyID,  ");
            strSql2.Append("  SUM(GoodSupplyCount)AS GoodSupplyCount,SUM(Money_Total) AS Money_Total,SUM(Money_YouHui) AS Money_YouHui,SUM(Money_PreDefine) AS Money_PreDefine,SUM(Money_Reality) AS Money_Reality,SUM(Money_Back) AS Money_Back");
            strSql2.Append("  FROM dbo.C_Supply A  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql2.Append(" INNER JOIN dbo.WB D ON A.WBID=D.ID");
            strSql2.Append("  where 1=1 and D.ISSimulate=0");
            if (Request.Form["QWBID"] != null && Request.Form["QWBID"].ToString() != "")
            {
                strSql2.Append("   AND A.WBID = " + Request.Form["QWBID"].ToString());
            }
            if (Qdtstart.Value.Trim() != "")
            {
                strSql2.Append("   AND dt_Trade> '" + Qdtstart.Value.Trim() + "'");
            }
            if (Qdtend.Value.Trim() != "")
            {
                strSql2.Append("   AND dt_Trade < '" + Qdtend.Value.Trim() + "'");
            }
            strSql2.Append("    GROUP BY C.strName");

            DataTable dt2 = SQLHelper.ExecuteDataTable(strSql2.ToString());
            Repeater2.DataSource = dt2;
            Repeater2.DataBind();
            //合计数据
         Money_Youhui=0;
         Money_Yufukuan = 0;
       Money_Shifu = 0;
        Moeny_Fanli = 0;
            if(dt2!=null&&dt2.Rows.Count!=0)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    Money_Youhui += Convert.ToDouble(dt2.Rows[i]["Money_YouHui"]);
                    Money_Yufukuan += Convert.ToDouble(dt2.Rows[i]["Money_PreDefine"]);
                    Money_Shifu += Convert.ToDouble(dt2.Rows[i]["Money_Reality"]);
                    Moeny_Fanli += Convert.ToDouble(dt2.Rows[i]["Money_Back"]);
                }
            }
            Money_Youhui = Math.Round(Money_Youhui, 2);
            Money_Yufukuan = Math.Round(Money_Yufukuan, 2);
            Money_Shifu = Math.Round(Money_Shifu, 2);
            Moeny_Fanli = Math.Round(Moeny_Fanli, 2);

        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

            DataBind();

        }

        DataTable GetoutputTable()
        {
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.strName AS WBID,C.strName AS GoodSupplyID, C.Price, A.UnitName,");
            strSql.Append("  SUM(GoodSupplyCount)AS GoodSupplyCount,SUM(Money_Total) AS Money_Total,SUM(Money_YouHui) AS Money_YouHui,SUM(Money_PreDefine) AS Money_PreDefine,SUM(Money_Reality) AS Money_Reality,SUM(Money_Back) AS Money_Back");
            strSql.Append("  FROM dbo.C_Supply A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append("  where 1=1 and B.ISSimulate=0");
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

            strSql.Append("   GROUP BY B.strName ,C.strName,C.Price,A.UnitName");
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

            colHeaders += "网点\t商品种类\t商品单价\t计量单位\t购买数量\t商品总价\t优惠金额\t预付款金额\t实付金额\t返利金额\n";
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
   

            //添加第二张表格的内容
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("  SELECT '', C.strName AS GoodSupplyID, '','', ");
            strSql2.Append("  SUM(GoodSupplyCount)AS GoodSupplyCount,SUM(Money_Total) AS Money_Total,SUM(Money_YouHui) AS Money_YouHui,SUM(Money_PreDefine) AS Money_PreDefine,SUM(Money_Reality) AS Money_Reality,SUM(Money_Back) AS Money_Back");
            strSql2.Append("  FROM dbo.C_Supply A  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql2.Append(" INNER JOIN dbo.WB D ON A.WBID=D.ID");
            strSql2.Append("  where 1=1 and D.ISSimulate=0");
            if (Request.Form["QWBID"] != null && Request.Form["QWBID"].ToString() != "")
            {
                strSql2.Append("   AND A.WBID = " + Request.Form["QWBID"].ToString());
            }
            if (Qdtstart.Value.Trim() != "")
            {
                strSql2.Append("   AND dt_Trade> '" + Qdtstart.Value.Trim() + "'");
            }
            if (Qdtend.Value.Trim() != "")
            {
                strSql2.Append("   AND dt_Trade < '" + Qdtend.Value.Trim() + "'");
            }
            strSql2.Append("    GROUP BY C.strName");

            DataTable dt2 = SQLHelper.ExecuteDataTable(strSql2.ToString());
            DataRow[] myRow2 = dt2.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的

            int cl2 = dt2.Columns.Count;

            colHeaders = "总计\n";
            resp.Write(colHeaders);
            //向HTTP输出流中写入取得的数据信息
            ls_item = "";
            //逐行处理数据  
            foreach (DataRow row in myRow2)
            {
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据    
                for (int j = 0; j < cl2; j++)
                {
                    if (j == (cl2 - 1))//最后一列，加\n
                    {
                        ls_item += row[j].ToString() + "\n";
                    }
                    else
                    {
                        ls_item += row[j].ToString() + "\t";
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