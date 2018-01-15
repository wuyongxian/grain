using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Web.Admin.Returned
{
    public partial class REGoods : System.Web.UI.Page
    {
 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Web.common.numTotol = 0;
                if (Request.QueryString["Account"] != null)
                {
                    GetDepositor(Request.QueryString["Account"].ToString());
                }
            }
        }
        private void GetDepositor(string AccountNumber)
        {
            string strSqlSimulate = "SELECT B.ISSimulate FROM dbo.Depositor A INNER JOIN dbo.WB B ON A.WBID=B.ID WHERE A.AccountNumber='"+AccountNumber+"' ";
            object objSimulate = SQLHelper.ExecuteScalar(strSqlSimulate);
            if (Convert.ToBoolean(objSimulate) )
            {
                Fun.Alert("您当前查询的账户为模拟账户，不可以继续操作！");
                return;
            }

            depositorInfo.Style.Add("display", "none");
            StorageList.Style.Add("display", "none");
         
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select ID,WBID,AccountNumber,strPassword,CunID as  BD_Address_CunID,strAddress,strName,PhoneNO,ISSendMessage,BankCardNO,dt_Update,");
            strSql.Append("   CASE( numState) WHEN 1 THEN '正常' ELSE '挂失' END AS numState,dt_Add,");
            strSql.Append("   CASE (IDCard) WHEN '' THEN '未填写' ELSE '******' END as IDCard");
            strSql.Append(" FROM dbo.Depositor ");
            strSql.Append(" where 1=1" );

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
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "js", "ShowDepositorInfo()", true);

                //获取存粮信息
                StringBuilder strSqlStorage = new StringBuilder();
                strSqlStorage.Append("SELECT A.ID,A.StorageNumber,convert(varchar(10),A.StorageDate,120) AS StorageDate, A.AccountNumber,B.strName AS VarietyID,A.Price_ShiChang,A.Price_DaoQi,A.CurrentRate,C.strName AS TimeID,A.StorageFee");
                strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
                strSqlStorage.Append("  INNER JOIN dbo.StorageTime C ON A.TimeID=C.ID");
                strSqlStorage.Append("  WHERE AccountNumber='" + AccountNumber + "'");
                strSqlStorage.Append("  and A.StorageNumber>0");
                DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
                if (dtStorage != null && dtStorage.Rows.Count != 0)
                {
                    StorageList.Style.Add("display", "block");
                    Repeater1.DataSource = dtStorage;
                    Repeater1.DataBind();
                }


                //获取储户的兑换信息
                StringBuilder strSqlEx = new StringBuilder();

                strSqlEx.Append("  SELECT ID, BusinessName,GoodName,GoodPrice,GoodCount,UnitName,VarietyCount,Money_DuiHuan");
                strSqlEx.Append("  FROM dbo.GoodExchange");
                strSqlEx.Append("  WHERE 1=1");
                strSqlEx.Append("  and ISReturn=0");//查询没有退还记录的商品 
                strSqlEx.Append("  AND Dep_AccountNumber='" + QAccountNumber.Value.Trim() + "'");
                strSqlEx.Append(" AND ID NOT IN (SELECT GoodExchangeID FROM dbo.SA_Exchange WHERE Dep_AN='" + QAccountNumber.Value.Trim() + "')");//排除已经被结算的兑换
                DataTable dtEx = SQLHelper.ExecuteDataTable(strSqlEx.ToString());
                if (dtEx != null && dtEx.Rows.Count != 0)
                {
                    divYes.Style.Add("display", "block");
                    divNO.Style.Add("display", "none");
                    divNO.Visible = false;
                    Repeater2.DataSource = dtEx;
                    Repeater2.DataBind();
                }
                else
                {
                    divYes.Style.Add("display", "none");
                    divNO.Style.Add("display", "block");
                }
            }
            else
            {
                string StrScript;
                StrScript = ("<script language=javascript>");
                StrScript += ("alert('您查询的储户不存在!');");
                StrScript += ("</script>");
                System.Web.HttpContext.Current.Response.Write(StrScript);
                return;
            }


        }


        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (QAccountNumber.Value.Trim() != "")
            {
                common.numTotol = 0;//清零折合现金
                GetDepositor(QAccountNumber.Value.Trim());
            }
        }

    }
}