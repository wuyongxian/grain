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
    public partial class StorageRate : System.Web.UI.Page
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

            DataTable dtLog = SQLHelper.ExecuteDataTable("  SELECT ID FROM dbo.StorageRate");

            DataTable dtWB = SQLHelper.ExecuteDataTable(" select ID from WB");
            if (dtLog == null || dtLog.Rows.Count == 0)
            {
                Fun.Alert("没有操作记录!");
                return;
            }
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < dtLog.Rows.Count; i++)
            {
                string rateID = dtLog.Rows[i]["ID"].ToString();
                for (int j = 0; j < dtWB.Rows.Count;j++ )
                {
                    string wbid = dtWB.Rows[j]["ID"].ToString();
                    sql.Append(string.Format("   INSERT INTO StorageRate_WB(StorageRateID,WBID) VALUES({0},{1})", rateID, wbid));
                }
                   
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