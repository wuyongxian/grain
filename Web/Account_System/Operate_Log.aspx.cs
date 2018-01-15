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
    public partial class Operate_Log : System.Web.UI.Page
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

        void verifyerror()
        {
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
            if (Session["isCheck"] == null || Convert.ToBoolean(Session["isCheck"]) == false)
            {
                return;
            }

            DataTable dtLog = SQLHelper.ExecuteDataTable("  SELECT * FROM dbo.Dep_OperateLog");
            if (dtLog == null || dtLog.Rows.Count == 0) {
                Fun.Alert("没有操作记录!");
                return;
            }
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < dtLog.Rows.Count; i++) {
                string VarietyName = dtLog.Rows[i]["VarietyID"].ToString();
                string UnitName = dtLog.Rows[i]["UnitID"].ToString();
                string ID = dtLog.Rows[i]["ID"].ToString();
                sql.Append(string.Format("    UPDATE dbo.Dep_OperateLog SET VarietyID=(SELECT ID FROM dbo.StorageVariety WHERE strName='{0}') WHERE dbo.Dep_OperateLog.ID={1}",VarietyName,ID));
                //sql.Append(string.Format("    UPDATE dbo.Dep_OperateLog SET UnitID=(SELECT ID FROM dbo.BD_MeasuringUnit WHERE strName='{0}') WHERE dbo.Dep_OperateLog.ID={1}", UnitName, ID));
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

        protected void btnInitDep_SID_Click(object sender, EventArgs e) {
            string sqlLog = "  SELECT ID,BusinessNO, BusinessName,Dep_AccountNumber FROM dbo.Dep_OperateLog ";
            DataTable dt = SQLHelper.ExecuteDataTable(sqlLog);
            if (dt == null || dt.Rows.Count == 0) {
                return;
            }
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++) {
                string ID = dt.Rows[i]["ID"].ToString();
                string BusinessNO = dt.Rows[i]["BusinessNO"].ToString();
                string BusinessName = dt.Rows[i]["BusinessName"].ToString();
                string Dep_AccountNumber = dt.Rows[i]["Dep_AccountNumber"].ToString();
                string Dep_SID = "0";
                 object obj ;
                switch (BusinessName) { 
                    case "1":
                    case "5":
                    case "8"://存粮,修改错误存粮,退还存粮
                       obj= SQLHelper.ExecuteScalar(string.Format("  SELECT TOP 1 ID FROM dbo.Dep_StorageInfo WHERE BusinessNO='{0}' AND AccountNumber='{1}'", BusinessNO, Dep_AccountNumber));
                        if (obj != null && obj.ToString() != "") {
                            sql.Append(string.Format( "  UPDATE dbo.Dep_OperateLog SET Dep_SID={0} WHERE    BusinessNO='{1}' AND Dep_AccountNumber='{2}'", obj.ToString(), BusinessNO, Dep_AccountNumber));
                        }
                        break;
                    case "2" : case "6"://兑换、退还兑换
                        obj = SQLHelper.ExecuteScalar(string.Format("  SELECT  TOP 1  Dep_SID FROM dbo.GoodExchange WHERE BusinessNO='{0}' AND Dep_AccountNumber='{1}'", BusinessNO, Dep_AccountNumber));
                        if (obj != null && obj.ToString() != "")
                        {
                            sql.Append(string.Format("  UPDATE dbo.Dep_OperateLog SET Dep_SID={0} WHERE    BusinessNO='{1}' AND Dep_AccountNumber='{2}'", obj.ToString(), BusinessNO, Dep_AccountNumber));
                        }
                        break;
                    case "3":case "7"://存转销、退还存转销
                        obj = SQLHelper.ExecuteScalar(string.Format("  SELECT  TOP 1  Dep_SID FROM dbo.StorageSell WHERE BusinessNO='{0}' AND Dep_AccountNumber='{1}'", BusinessNO, Dep_AccountNumber));
                        if (obj != null && obj.ToString() != "")
                        {
                            sql.Append(string.Format("  UPDATE dbo.Dep_OperateLog SET Dep_SID={0} WHERE    BusinessNO='{1}' AND Dep_AccountNumber='{2}'", obj.ToString(), BusinessNO, Dep_AccountNumber));
                        }
                        break;
                    case "9":
                    case "10"://换购、退还换购
                        obj = SQLHelper.ExecuteScalar(string.Format("  SELECT  TOP 1  Dep_SID FROM dbo.StorageShopping WHERE BusinessNO='{0}' AND Dep_AccountNumber='{1}'", BusinessNO, Dep_AccountNumber));
                        if (obj != null && obj.ToString() != "")
                        {
                            sql.Append(string.Format("  UPDATE dbo.Dep_OperateLog SET Dep_SID={0} WHERE    BusinessNO='{1}' AND Dep_AccountNumber='{2}'", obj.ToString(), BusinessNO, Dep_AccountNumber));
                        }
                        break;
                    case "11"://结息
                        obj = SQLHelper.ExecuteScalar(string.Format("  SELECT  TOP 1  Dep_SID FROM dbo.StorageInterest WHERE BusinessNO='{0}' AND Dep_AccountNumber='{1}'", BusinessNO, Dep_AccountNumber));
                        if (obj != null && obj.ToString() != "")
                        {
                            sql.Append(string.Format("  UPDATE dbo.Dep_OperateLog SET Dep_SID={0} WHERE    BusinessNO='{1}' AND Dep_AccountNumber='{2}'", obj.ToString(), BusinessNO, Dep_AccountNumber));
                        }
                        break;


                }


            }
            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, sql.ToString());//将储户存粮置0
                  
                    tran.Commit();
                    Fun.Alert("初始化成功！");
                }
                catch
                {
                    tran.Rollback();
                    Fun.Alert("初始化失败！");
                }
            }

        }
    }
}