using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
namespace Web.Account_System
{
    public partial class StorageVariety : System.Web.UI.Page
    {
        public string strContent;
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
         
            //检验账户正确性22
            StringBuilder strAccount = new StringBuilder();
            strAccount.Append(" SELECT ID, WB_ID,UserGroup_ID,strLoginName,strRealName,strPassword,ISEnable from Users");
            strAccount.Append(" WHERE  UserGroup_ID=@UserGroup_ID AND LOWER(strLoginName)=@strLoginName ");
            SqlParameter[] para = new SqlParameter[]{
            new SqlParameter("@UserGroup_ID",1),
            new SqlParameter("@strLoginName",txtuid.Text.Trim().ToLower())
            };
            DataTable dtAccount = SQLHelper.ExecuteDataTable(strAccount.ToString(), para);
            if (dtAccount != null && dtAccount.Rows.Count != 0)
            {
                string strPassword = dtAccount.Rows[0]["strPassword"].ToString();
                bool ISEnable = Convert.ToBoolean(dtAccount.Rows[0]["ISEnable"]);
                if (Fun.GetMD5_32(txtpwd.Text.Trim()) == strPassword)
                {
                    if (ISEnable)
                    {
                        divSql.Style.Add("display", "block");
                        Session["isCheck"] = true;
                    }
                    else
                    {
                        verifyerror();
                    }
                }
                else
                {
                    verifyerror();
                }
            }
            else
            {
                verifyerror();
            }
            
        }

        void verifyerror() {
            Session["isCheck"] = false;
            divSql.Style.Add("display", "none");
            string StrScript;
            StrScript = ("<script language=javascript>");
            StrScript += ("alert('验证失败!');");
            StrScript += ("</script>");
            System.Web.HttpContext.Current.Response.Write(StrScript);
            return;

        }

        protected void btnSql_Click(object sender, EventArgs e)
        {
            StringBuilder sqlStorageInfo = new StringBuilder();
            sqlStorageInfo.Append(" SELECT WBID AS 网点,VarietyID AS 粮食品种, SUM(StorageNumberRaw) AS 存储量");
            sqlStorageInfo.Append("  FROM dbo.Dep_StorageInfo");
            sqlStorageInfo.Append(" GROUP BY VarietyID,WBID");
            DataTable dtDep_StorageInfo = SQLHelper.ExecuteDataTable(sqlStorageInfo.ToString());
            string str1 = GetSqlContent(dtDep_StorageInfo);


            StringBuilder sqlCheckOut = new StringBuilder();
            sqlCheckOut.Append(" SELECT WBID AS 网点,VarietyID AS 粮食品种, SUM(Count_Trade) AS 出库量");
            sqlCheckOut.Append(" FROM dbo.SA_CheckOutLog");
            sqlCheckOut.Append(" GROUP BY VarietyID,WBID");
            DataTable dtCheckOut = SQLHelper.ExecuteDataTable(sqlCheckOut.ToString());
            string str2 = GetSqlContent(dtCheckOut);
            strContent = str1 + str2;

        }

        string GetSqlContent(DataTable dt)
        {
            int tblWidth = dt.Columns.Count * 100;
            StringBuilder strReturn = new StringBuilder();

            //表格内容
            strReturn.Append("    <table class='tabPrint' style='width: " + tblWidth + "px;padding: 5px 0px; font-size: 14px;'>");
            //添加表格样式
            strReturn.Append("    <style>");
            strReturn.Append("    table.tabPrint{ border-collapse: collapse; border: 1px solid #666;  font-size: 14px;}");
            strReturn.Append("     table.tabPrint thead td, table.set_border th{ font-weight: bold; background-color: White;}");
            strReturn.Append("    table.tabPrint tr:nth-child(even){ background-color: #eee;}");
            strReturn.Append("     table.tabPrint td, table.border th{  border: 1px solid #666;}");
            strReturn.Append("   </style>");
            strReturn.Append("   <tr style='height: 25px;'>");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                strReturn.Append("    <td style='width: 100px;'> <span>" + dt.Columns[i].ColumnName + "</span></td>");
            }
            strReturn.Append("  </tr>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strReturn.Append("   <tr style='height: 20px;'>");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    strReturn.Append("   <td style='width: 100px;'> <span>" + dt.Rows[i][j] + "</span></td>");
                }
                strReturn.Append("  </tr>");
            }
            strReturn.Append("  </table>");
            return strReturn.ToString();
        }

        protected void btnInitStorage_Click(object sender, EventArgs e)
        {
            if (Session["isCheck"] == null || Convert.ToBoolean( Session["isCheck"])==false)
            {
                return;
            }
            StringBuilder sqlStorageInfo = new StringBuilder();
            sqlStorageInfo.Append(" SELECT WBID AS 网点,VarietyID AS 粮食品种, SUM(StorageNumberRaw) AS 存储量");
            sqlStorageInfo.Append("  FROM dbo.Dep_StorageInfo");
            sqlStorageInfo.Append(" GROUP BY VarietyID,WBID");
            DataTable dtDep_StorageInfo = SQLHelper.ExecuteDataTable(sqlStorageInfo.ToString());
          


            StringBuilder sqlCheckOut = new StringBuilder();
            sqlCheckOut.Append(" SELECT WBID AS 网点,VarietyID AS 粮食品种, SUM(Count_Trade) AS 出库量");
            sqlCheckOut.Append(" FROM dbo.SA_CheckOutLog");
            sqlCheckOut.Append(" GROUP BY VarietyID,WBID");
            DataTable dtCheckOut = SQLHelper.ExecuteDataTable(sqlCheckOut.ToString());
            StringBuilder sql = new StringBuilder();
            sql.Append(" DELETE FROM dbo.SA_VarietyStorageLog");
            sql.Append(" ");
            for (int i = 0; i < dtDep_StorageInfo.Rows.Count; i++) {
                int WBID = Convert.ToInt32(dtDep_StorageInfo.Rows[i]["网点"]);
                int VarietyID = Convert.ToInt32(dtDep_StorageInfo.Rows[i]["粮食品种"]);
                double numStorageIn = Convert.ToDouble(dtDep_StorageInfo.Rows[i]["存储量"]);
                double numStorageOut = 0;
                double numStorage = 0;
                double numStorageChange = 0;
                double numStorageLoss = 0;
                DataView dv = dtCheckOut.DefaultView;
                dv.RowFilter = "网点=" + WBID + " and 粮食品种="+VarietyID;
                if (dv.Count > 0) {
                    DataRow row = dv.ToTable().Rows[0];
                    numStorageOut = -Convert.ToDouble(row["出库量"]);//出库量取负数
                }
                numStorage = numStorageIn + numStorageOut;
                sql.Append(" INSERT INTO dbo.SA_VarietyStorageLog");
                sql.Append("   ( storageType , OperateLogID ,  CheckOutID ,   AccountNumber ,  VarietyID ,WBID , numStorage, numStorageChange, numStorageIn ,numStorageOut ,numStorageLoss , WareHouseID ,ISHQ , ISSimulate,dtLog)");
                sql.Append(" VALUES  ( 0 ,0 ,   0 ,  N'' ,  "+VarietyID+" ,  "+WBID+" , ");
                sql.Append("    "+numStorage+" ,"+numStorageChange+" , "+numStorageIn+" , "+numStorageOut+" , "+numStorageLoss+" ,");
                sql.Append("    0 , 0 ,   0 ,'"+DateTime.Now.ToString()+"'  )");
            }
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql.ToString());
                    tran.Commit();
                    Fun.Alert("初始化成功!");
                }
                catch
                {
                    tran.Rollback();
                    Fun.Alert("初始化失败!");
                }
            }

        }



    }
}