using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Web.Settle
{
    public partial class CheckOutHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               // DataBind("");
            }
        }
        private void DataBind(string WBName)
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT  B.strName AS WBName,A.ID,A.SA_AN,V.strName AS  Variety_Name,VarietyLevel_Name,Weight_Mao,Weight_Pi,Weight_Jing,Weight_Reality,CONVERT(VARCHAR(100),dt_Trade,23) AS dt_Trade");
            strSql.Append("  FROM dbo.SA_CheckOut A INNER JOIN WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety V ON A.Variety_Name=V.ID");
          
            if (WBName.ToString().Trim() != "")
            {
                strSql.Append("   WHERE B.strName = '" + WBName + "'");
            }

            strSql.Append("  ORDER BY A.dt_Trade DESC");


            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0) {
                Fun.Alert("没有查询到出库记录!");
                Repeater1.Visible = false;
                Repeater2.Visible = false;
                return;
            }
            Repeater1.DataSource = dt;
            Repeater1.DataBind();
            Repeater1.Visible = true;
            Repeater2.Visible = true;

            StringBuilder strSqlTotal = new StringBuilder();
            strSqlTotal.Append("   SELECT  B.strName AS WBName,V.strName AS  Variety_Name,");
            strSqlTotal.Append("    SUM( Weight_Mao) AS Weight_Mao, SUM( Weight_Pi) AS Weight_Pi, SUM( Weight_Jing) AS Weight_Jing, SUM( Weight_Reality) AS Weight_Reality");
            strSqlTotal.Append("  FROM dbo.SA_CheckOut A INNER JOIN WB B ON A.WBID=B.ID");
            strSqlTotal.Append("   INNER JOIN dbo.StorageVariety V ON A.Variety_Name=V.ID");

            if (WBName.ToString().Trim() != "")
            {
                strSqlTotal.Append("   WHERE B.strName = '" + WBName + "'");
            }
            strSqlTotal.Append("   GROUP BY B.strName,V.strName");


            DataTable dtTotal = SQLHelper.ExecuteDataTable(strSqlTotal.ToString());
            Repeater2.DataSource = dtTotal;
            Repeater2.DataBind();

        }

    

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT COUNT(A.ID)");
            strSql.Append(" FROM dbo.SA_Account A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSql.Append(" WHERE B.strName ='" + txtWBID.Value.Trim() + "'");
            object objWB = SQLHelper.ExecuteScalar(strSql.ToString());
            if (Convert.ToInt32(objWB) <= 0)
            {
                Repeater1.Visible = false;
                Repeater2.Visible = false;
                Fun.Alert("您查询的网点不存在或该网点尚未出库开户");
                return;
            }
            DataBind(txtWBID.Value);
        }

        protected void btnOutPut_Click(object sender, EventArgs e)
        {
            outputExcel();
        }

        private void outputExcel()
        {
            string WBName = txtWBID.Value.ToString();
            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT  B.strName AS WBName,A.SA_AN,V.strName AS  Variety_Name,Weight_Mao,Weight_Pi,Weight_Jing,Weight_Reality,CONVERT(VARCHAR(100),dt_Trade,23) AS dt_Trade");
            strSql.Append("  FROM dbo.SA_CheckOut A INNER JOIN WB B ON A.WBID=B.ID");
            strSql.Append("  INNER JOIN dbo.StorageVariety V ON A.Variety_Name=V.ID");

            if (WBName.ToString().Trim() != "")
            {
                strSql.Append("   WHERE B.strName = '" + WBName + "'");
            }

            strSql.Append("  ORDER BY A.dt_Trade DESC");


            StringBuilder strSqlTotal = new StringBuilder();
            strSqlTotal.Append("   SELECT  B.strName AS WBName,V.strName AS  Variety_Name,");
            strSqlTotal.Append("    SUM( Weight_Mao) AS Weight_Mao, SUM( Weight_Pi) AS Weight_Pi, SUM( Weight_Jing) AS Weight_Jing, SUM( Weight_Reality) AS Weight_Reality");
            strSqlTotal.Append("  FROM dbo.SA_CheckOut A INNER JOIN WB B ON A.WBID=B.ID");
            strSqlTotal.Append("   INNER JOIN dbo.StorageVariety V ON A.Variety_Name=V.ID");

            if (WBName.ToString().Trim() != "")
            {
                strSqlTotal.Append("   WHERE B.strName = '" + WBName + "'");
            }
            strSqlTotal.Append("   GROUP BY B.strName,V.strName");


            DataTable dtTotal = SQLHelper.ExecuteDataTable(strSqlTotal.ToString());
            

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                Fun.Alert("请先检索要查询的信息后在导出！");
                return;
            }

            string fileName = "网点出库记录" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            HttpResponse resp;
            resp = Page.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            string colHeaders = "", ls_item = "", ls_Bottom = "";

            DataRow[] myRow = dt.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
            int i = 0;
            int cl = dt.Columns.Count;

            colHeaders += "网点名称\t网点账号\t产品名称\t毛重\t皮重\t净重\t出库实重\t出库时间\n";
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
            resp.Write("总计\n");

            myRow = dtTotal.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
           i = 0;
           cl = dtTotal.Columns.Count;
            foreach (DataRow row in myRow)
            {
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据    
                for (i = 0; i < cl; i++)
                {
                    if (i == 1)
                    { //网点账号一列不写入数据
                        ls_item +=  "\t";
                    }
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
    }
}