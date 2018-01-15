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
    public partial class DepositorInfo : System.Web.UI.Page
    {
        public double numTotol = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Account"] != null)
                {
                    GetDepositor(Request.QueryString["Account"].ToString());
                }
            }
        }
        private void GetDepositor(string AccountNumber)
        {
            depositorInfo.Style.Add("display", "none");
            StorageList.Style.Add("display", "none");
            exchangeList.Style.Add("display", "none");
            SellList.Style.Add("display", "none");
            ShoppingList.Style.Add("display", "none");

            //查询账户 
            DataTable dt = common.getDepInfo(AccountNumber, Convert.ToBoolean(Session["ISHQ"]), Session["WB_ID"].ToString());
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
                DataTable dtStorage = common.getDepStorageInfo(AccountNumber);

                //价值总额
                numTotol = common.GetLiXiTotal(dtStorage);

                if (dtStorage != null && dtStorage.Rows.Count != 0)
                {
                    StorageList.Style.Add("display", "block");
                    Repeater1.DataSource = dtStorage;
                    Repeater1.DataBind();
                }

                //获取兑换信息
                StringBuilder strSqlEx = new StringBuilder();

                strSqlEx.Append("  SELECT ID, BusinessName,GoodName,GoodPrice,GoodCount,UnitName,VarietyCount,Money_DuiHuan");
                strSqlEx.Append("  ,CASE ISReturn WHEN 0 THEN '兑换' ELSE '退还兑换' END AS ISReturn");
                strSqlEx.Append("  FROM dbo.GoodExchange");
                strSqlEx.Append("  where 1=1");
                strSqlEx.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
                strSqlEx.Append("  order by dt_Exchange desc");
                DataTable dtEx = SQLHelper.ExecuteDataTable(strSqlEx.ToString());
                if (dtEx != null && dtEx.Rows.Count != 0)
                {

                    exchangeList.Style.Add("display", "block");
                    R_exchange.DataSource = dtEx;
                    R_exchange.DataBind();
                }
               

                //获取存转销信息
                StringBuilder strSqlSell = new StringBuilder();

                strSqlSell.Append("  SELECT ID, BusinessName,CONVERT(varchar(100), dt_Sell, 111) AS dt_Sell,VarietyName,VarietyCount,StorageDate,VarietyInterest,VarietyMoney");
                strSqlSell.Append("  ,CASE ISReturn WHEN 0 THEN '存转销' ELSE '退还存转销' END AS ISReturn");
                strSqlSell.Append("  FROM dbo.StorageSell ");
                strSqlSell.Append("  where 1=1");//查询没有退还记录的商品 
                strSqlSell.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
                strSqlSell.Append("  order by dt_Sell desc");
                DataTable dtSell = SQLHelper.ExecuteDataTable(strSqlSell.ToString());
                if (dtSell != null && dtSell.Rows.Count != 0)
                {

                    SellList.Style.Add("display", "block");
                 
                    R_Sell.DataSource = dtSell;
                    R_Sell.DataBind();
                }

                //获取换购信息
                StringBuilder strSqlShopping = new StringBuilder();

                strSqlShopping.Append("  SELECT ID, BusinessName,CONVERT(varchar(100), dt_Sell, 111) AS dt_Sell,VarietyName,VarietyCount,StorageDate,VarietyInterest,VarietyMoney");
                strSqlShopping.Append("  ,CASE ISReturn WHEN 0 THEN '换购' ELSE '退还换购' END AS ISReturn");
                strSqlShopping.Append("  FROM dbo.StorageShopping ");
                strSqlShopping.Append("  where 1=1");//查询没有退还记录的商品 
                strSqlShopping.Append("  AND Dep_AccountNumber='" + AccountNumber + "'");
                strSqlShopping.Append("  order by dt_Sell desc");
                DataTable dtShopping = SQLHelper.ExecuteDataTable(strSqlShopping.ToString());
                if (dtShopping != null && dtShopping.Rows.Count != 0)
                {

                    ShoppingList.Style.Add("display", "block");

                    R_Shopping.DataSource = dtShopping;
                    R_Shopping.DataBind();
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
                GetDepositor(QAccountNumber.Value.Trim());
            }
        }

        public string GetDay(object date)
        {
            DateTime t1 = Convert.ToDateTime(date);
            TimeSpan ts = DateTime.Now.Subtract(t1);
            int numday = Convert.ToInt32(Math.Floor((decimal)ts.TotalDays));
            return numday.ToString();

        }

     
    }
}