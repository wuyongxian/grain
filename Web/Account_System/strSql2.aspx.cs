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
    public partial class strSql2 : System.Web.UI.Page
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
            if (Session["isCheck"] == null || Convert.ToBoolean(Session["isCheck"]) == false)
            {
                return;
            }
            string strSql = txtSql.Text.Trim();
            string strType=strSql.Substring(0,strSql.IndexOf(" "));
            switch (strType.ToLower()) {
                case "insert":
                case "update":
                case "delete": strContent = SQLHelper.ExecuteNonQuery(strSql).ToString(); break;
                case "select":
                    DataTable dt = SQLHelper.ExecuteDataTable(strSql);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        strContent = "没有查找到任何信息!";
                    }
                    else {
                      strContent=  GetSqlContent(dt);
                    }
                    break;
            }
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
            for(int i=0;i<dt.Rows.Count;i++)
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


    }
}