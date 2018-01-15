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
    public partial class CheckOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind("");
            }
        }
        private void DataBind(string WBName)
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT A.ID,B.ID as WBID, B.strName AS WBName,C.ID as VarietyID, C.strName AS VarietyName,D.ID as VarietyLevelID, D.strName AS VarietyLevelName,A.numStorage");
            strSql.Append("  FROM dbo.SA_VarietyStorage A INNER JOIN WB B ON A.WBID=B.ID");
            strSql.Append("  LEFT OUTER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSql.Append("  LEFT OUTER JOIN dbo.StorageVarietyLevel_B  D ON A.VarietyLevelID=D.ID");
            strSql.Append("  ");
       
            if (WBName.ToString().Trim() != "")
            {
                strSql.Append("   WHERE B.strName = '" + WBName + "'");
            }

            strSql.Append("   order by B.strName");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();

        }

        public string GetVarietyLevel(object objVarietyName)
        {
            if (objVarietyName == null || objVarietyName.ToString() == "") {
                return "";
            }
            
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT TOP 1 C.strName");
            strSql.Append("  FROM dbo.StorageVariety A INNER JOIN dbo.StorageVarietyLevel_L B ON B.VarietyID=A.ID");
            strSql.Append("  INNER JOIN dbo.StorageVarietyLevel_B C ON B.VarietyLevelID=C.ID ");
            strSql.Append("  WHERE A.strName='"+objVarietyName+"'");
            return SQLHelper.ExecuteScalar(strSql.ToString()).ToString() ;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            object objWB = SQLHelper.ExecuteScalar(" SELECT  COUNT(ID) FROM dbo.WB WHERE strName ='" + txtWBID.Value.Trim() + "'");
            if (Convert.ToInt32(objWB) <= 0)
            {
                Fun.Alert("您查询的网点不存在");
                return;
            }
            DataBind(txtWBID.Value);
        }



        public string OutPut(object objID, object objWBID, object objVarietyID, object objVarietyLevelID)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return " <a href='/Settle/CheckOutDetail.aspx?ID=" + objID + "&WBID=" + objWBID + "&VarietyID=" + objVarietyID + "&VarietyLevelID=" + objVarietyLevelID + "'>出库</a>";
            }
            else
            {
                return " <a href='#' disabled='disabled'>出库</a> ";
            }
        }

        protected void btnOutPut_Click(object sender, EventArgs e)
        {
            outputExcel();
        }

        private void outputExcel()
        {


            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT B.strName AS WBName, C.strName AS VarietyName,D.strName AS VarietyLevelName,A.numStorage");
            strSql.Append("  FROM dbo.SA_VarietyStorage A INNER JOIN WB B ON A.WBID=B.ID");
            strSql.Append("    LEFT OUTER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSql.Append("   LEFT OUTER JOIN dbo.StorageVarietyLevel_B  D ON A.VarietyLevelID=D.ID");

            if (txtWBID.Value.Trim() != "")
            {
                strSql.Append("   WHERE B.strName = '" + txtWBID.Value + "'");
            }

            strSql.Append("   order by B.strName");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt == null || dt.Rows.Count == 0)
            {
                Fun.Alert("请先检索要查询的信息后在导出！");
                return;
            }

            string fileName = "网点库存" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            HttpResponse resp;
            resp = Page.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            string colHeaders = "", ls_item = "", ls_Bottom = "";

            DataRow[] myRow = dt.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
            int i = 0;
            int cl = dt.Columns.Count;

            colHeaders += "网点\t品种\t等级\t库存\n";
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
    }
}