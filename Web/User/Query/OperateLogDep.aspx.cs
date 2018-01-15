using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.User.Query
{
    public partial class OperateLogDep : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["AccountNumber"] != null)
                {
                    DataBind(Request.QueryString["AccountNumber"].ToString());
                }
            }
        }
        private void DataBind(string AccountNumber)
        {
          
            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();//当前网点ID
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select ID,WBID,AccountNumber,strPassword, CunID as  BD_Address_CunID,strAddress,strName,PhoneNO,ISSendMessage,BankCardNO,dt_Update,");
            strSql.Append("   CASE( numState) WHEN 1 THEN '正常' ELSE '挂失' END AS numState,dt_Add,");
            strSql.Append("   CASE (IDCard) WHEN '' THEN '未填写' ELSE '******' END as IDCard");
            strSql.Append(" FROM dbo.Depositor  where 1=1 ");
            strSql.Append(" and ISClosing=0");//排除销户储户
            strSql.Append(" and numState=1");//排除挂失储户
            if (Convert.ToBoolean( common.GetWBAuthority()["Enable_Distance"]) == false)//不允许异地存取的要进行判断
            {
                strSql.Append(" and WBID= " + WBID);
            }

            strSql.Append(" and AccountNumber=@AccountNumber ");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50)};
            parameters[0].Value = AccountNumber;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dt != null && dt.Rows.Count != 0)
            {
                string numState = dt.Rows[0]["numState"].ToString();

                if (numState == "0")
                {
                    string StrScript;
                    StrScript = ("<script language=javascript>");
                    StrScript += ("alert('您查询的账户已经申请挂失!');");
                    StrScript += ("</script>");
                    System.Web.HttpContext.Current.Response.Write(StrScript);
                    return;
                }
                depositorInfo.Style.Add("display", "block");
                D_strName.InnerText = dt.Rows[0]["strName"].ToString();
                D_strAddress.InnerText = dt.Rows[0]["strAddress"].ToString();
                D_PhoneNo.InnerText = dt.Rows[0]["PhoneNo"].ToString();
                D_AccountNumber.InnerText = dt.Rows[0]["AccountNumber"].ToString();
                D_numState.InnerText = dt.Rows[0]["numState"].ToString();
                D_IDCard.InnerText = dt.Rows[0]["IDCard"].ToString();


                StringBuilder strSqlCommune = new StringBuilder();
                strSqlCommune.Append("  SELECT A.ID,B.strName AS WBID,C.strRealName AS UserID,A.Price,A.Dep_AccountNumber,A.BusinessNO, A.VarietyName,A.UnitName,A.GoodCount,A.Count_Trade,A.Money_Trade,A.Count_Balance,CONVERT(NVARCHAR(100),A.dt_Trade,23) AS dt_Trade,");
                strSqlCommune.Append("  CASE A.BusinessName WHEN '1' THEN '存入' WHEN '2' THEN '兑换' WHEN '3' THEN '存转销'  WHEN '5' THEN '修改错误存粮' WHEN '6' THEN '退还兑换' WHEN '7' THEN '退还存转销' WHEN '8' THEN '退还存粮' WHEN '9' THEN '产品换购' WHEN '10' THEN '退还产品换购' WHEN '11' THEN '结息' WHEN '12' THEN '换存折' WHEN '13' THEN '商品销售' WHEN '14' THEN '退还商品销售' WHEN '15' THEN '积分兑换商品' WHEN '16' THEN '存粮转存'  WHEN '17' THEN '批量兑换' END AS BusinessName");
                strSqlCommune.Append("  FROM dbo.Dep_OperateLog A INNER JOIN dbo.WB B ON A.WBID=B.ID");
                strSqlCommune.Append("  INNER JOIN dbo.Users C ON A.UserID=C.ID");
                strSqlCommune.Append("  where 1=1");
                strSqlCommune.Append("   AND A.Dep_AccountNumber = '" + AccountNumber + "'");
                if (Qdtstart.Value.Trim() != "")
                {
                    strSqlCommune.Append("   AND A.dt_Trade> '" + Qdtstart.Value.Trim() + "'");
                }
                if (Qdtend.Value.Trim() != "")
                {
                    strSqlCommune.Append("   AND A.dt_Trade < '" + Qdtend.Value.Trim() + "'");
                }

                strSqlCommune.Append("   order by A.dt_Trade desc");
                strSql.Append("  ");


                DataTable dtCommune = SQLHelper.ExecuteDataTable(strSqlCommune.ToString());
                Repeater1.DataSource = dtCommune;
                Repeater1.DataBind();
            }
            else {
                string StrScript;
                StrScript = ("<script language=javascript>");
                StrScript += ("alert('没有查询到信息，请检查输入账号和选择日期是否正确!');");
                StrScript += ("</script>");
                System.Web.HttpContext.Current.Response.Write(StrScript);
                return;
            }
         

        }
        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (QAccountNumber.Value.Trim() != "")
            {
                DataBind(QAccountNumber.Value.Trim());
            }

        }
    }
}