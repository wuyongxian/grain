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
    public partial class StorageVarietyLevel : System.Web.UI.Page
    {
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

        protected void btnInitStorage_Click(object sender, EventArgs e)
        {
            if (Session["isCheck"] == null || Convert.ToBoolean(Session["isCheck"]) == false)
            {
                return;
            }

            DataTable dtLog = SQLHelper.ExecuteDataTable("  SELECT * FROM dbo.Dep_StorageInfo");

            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                Fun.Alert("没有操作记录!");
                return;
            }
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < dtLog.Rows.Count; i++)
            {
                string ID = dtLog.Rows[i]["ID"].ToString();
                string StorageRateID = dtLog.Rows[i]["StorageRateID"].ToString();
                sql.Append(string.Format(" UPDATE dbo.Dep_StorageInfo SET VarietyLevelID=(SELECT VarietyLevelID FROM dbo.StorageRate WHERE ID={0}) WHERE dbo.Dep_StorageInfo.ID={1}",StorageRateID,ID));
               

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

        protected void btnStorageLevel_Click(object sender, EventArgs e)
        {
            if (Session["isCheck"] == null || Convert.ToBoolean(Session["isCheck"]) == false)
            {
                return;
            }

            StringBuilder sqlLevel=new StringBuilder ();
            sqlLevel.Append("  SELECT L.VarietyID,L.VarietyLevelID,B.strName AS VarietyLevelName");
            sqlLevel.Append("  FROM dbo.StorageVarietyLevel_L L INNER JOIN dbo.StorageVarietyLevel_B B ON L.VarietyLevelID=B.ID");
            sqlLevel.Append(" ORDER BY B.ID");
            DataTable dtLevel = SQLHelper.ExecuteDataTable(sqlLevel.ToString());
            Dictionary<string, string> dicLevel = new Dictionary<string, string>();
            for (int i = 0; i < dtLevel.Rows.Count; i++) 
            {
                if (!dicLevel.Keys.Contains(dtLevel.Rows[i]["VarietyID"].ToString()))
                {
                    dicLevel.Add(dtLevel.Rows[i]["VarietyID"].ToString(), dtLevel.Rows[i]["VarietyLevelID"].ToString());
                }
            }


            StringBuilder sql = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in dicLevel)
            {
                string key = kv.Key;
                string value = kv.Value;
                sql.Append(string.Format( "   UPDATE dbo.SA_VarietyStorage SET VarietyLevelID={0} WHERE VarietyID={1}",value,key));
                sql.Append(string.Format("   UPDATE dbo.SA_VarietyStorageLog SET VarietyLevelID={0} WHERE VarietyID={1}", value, key));
                sql.Append(string.Format("   UPDATE dbo.SA_CheckOut SET VarietyLevel_Name={0} WHERE Variety_Name={1}", value, key));
                Console.WriteLine(kv.Key + kv.Value);

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