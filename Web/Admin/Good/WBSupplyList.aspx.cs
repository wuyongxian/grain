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
    public partial class WBSupplyList : System.Web.UI.Page
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
            strSql.Append("   select A.ID,B.ID as WBID, B.strName AS  WBName,C.ID as GoodSupplyID, C.strName AS  GoodSupplyName,A.Price,A.Price_WB, A.Price_WBBack,  Price_Money,  Quantity,CONVERT(NVARCHAR(100),dt_Trade,23) AS dt_Trade");
            strSql.Append("   FROM dbo.GoodSupplyStock   A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql.Append("  WHERE A.ISHQ=0");//排除总部
            strSql.Append("  and B.ISSimulate=0");//排除模拟网点
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

            strSql.Append("  order by dt_Trade desc");//排除模拟网点



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
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select A.ID,B.strName AS  WBID,C.strName AS  GoodSupplyID,A.Price,A.Price_WB, A.Price_WBBack,  Price_Money,  Quantity");
            strSql.Append("   FROM dbo.GoodSupplyStock   A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
            strSql.Append("  WHERE A.ISHQ=0");//排除总部
            strSql.Append("  and B.ISSimulate=0");//排除模拟网点
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
            strSql.Append("  order by dt_Trade desc");//排除模拟网点


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

            string fileName = "社员商品进货记录" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".xls";

            HttpResponse resp;
            resp = Page.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            string colHeaders = "", ls_item = "", ls_Bottom = "";

            DataRow[] myRow = dt.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
            int i = 0;
            int cl = dt.Columns.Count;

            colHeaders += "网点\t存期\t存储产品\t存入量\t存入价\t折合现金\n";
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


        public string GetUpdateItem(object ID,object GoodSupplyID,object WBID, object Quantity)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return " <input type='button' value='修改进货数量' onclick='ShowUpdateQuanlity("+ID+","+GoodSupplyID+","+WBID+","+Quantity+");' style='width:120px;height:25px;' />";
            }
            else
            {
                return "<input type='button' disabled='disabled'  value='修改进货数量' style='width:120px;height:25px;' />";
            }
        }

    }
}