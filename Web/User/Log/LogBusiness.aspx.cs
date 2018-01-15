using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.User.Log
{
    public partial class LogBusiness : System.Web.UI.Page
    {
        public double numTotol = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }
        private void GetDepositor(string dtDate)
        {
            
            StorageList.Style.Add("display", "none");
            exchangeList.Style.Add("display", "none");
            SellList.Style.Add("display", "none");
            StorageLog.Style.Add("display", "none");
            exchangeLog.Style.Add("display", "none");
            SellLog.Style.Add("display", "none");
           //获取存粮业务日志
                StringBuilder strSqlStorage = new StringBuilder();
                strSqlStorage.Append("  SELECT A.AccountNumber,B.strRealName as UserID, C.strName AS VarietyID,StorageNumber,CONVERT(varchar(100), A.StorageDate, 23) AS StorageDate,");
                strSqlStorage.Append("  A.Price_ShiChang,D.strName AS TimeID,B.strRealName AS UserID");
                strSqlStorage.Append("  FROM  dbo.Dep_StorageInfo A INNER JOIN dbo.Users B ON A.UserID=B.ID");
                strSqlStorage.Append("  INNER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
                strSqlStorage.Append("  INNER JOIN dbo.StorageTime D ON A.TimeID=D.ID");
                strSqlStorage.Append("  WHERE CONVERT(varchar(100), StorageDate, 23)=CONVERT(varchar(100), '"+dtDate.ToString()+"', 23)");//2015-01-01格式日期
                strSqlStorage.Append("  ");
                DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
                if (dtStorage != null && dtStorage.Rows.Count != 0)
                {
                    StorageList.Style.Add("display", "block");
                    Repeater1.DataSource = dtStorage;
                    Repeater1.DataBind();
                }
                else {
                    StorageLog.Style.Add("display", "block");
                }

                //获取兑换信息
                StringBuilder strSqlEx = new StringBuilder();

                strSqlEx.Append("  SELECT  A.ID, B.strRealName AS UserID, Dep_AccountNumber AS AccountNumber , BusinessName,GoodName,GoodPrice,GoodCount,UnitName,VarietyCount,Money_DuiHuan");
                strSqlEx.Append("  FROM dbo.GoodExchange A INNER JOIN dbo.Users B ON A.UserID=B.ID");
                strSqlEx.Append("  where ISReturn=0");//查询没有退还记录的商品 
                strSqlEx.Append("  and  CONVERT(varchar(100), dt_Exchange, 23)=CONVERT(varchar(100), '" + dtDate.ToString() + "', 23)");
                DataTable dtEx = SQLHelper.ExecuteDataTable(strSqlEx.ToString());
                if (dtEx != null && dtEx.Rows.Count != 0)
                {

                    exchangeList.Style.Add("display", "block");
                    R_exchange.DataSource = dtEx;
                    R_exchange.DataBind();
                }
                else {
                    exchangeLog.Style.Add("display", "block");
                }


                //获取存转销信息
                StringBuilder strSqlSell = new StringBuilder();

                strSqlSell.Append("  SELECT A.ID, B.strRealName AS UserID,A.Dep_AccountNumber AS AccountNumber, BusinessName,CONVERT(varchar(100), dt_Sell, 111) AS dt_Sell,VarietyName,VarietyCount,StorageDate,VarietyInterest,VarietyMoney");
                strSqlSell.Append("  FROM dbo.StorageSell A INNER JOIN dbo.Users B ON A.UserID=B.ID");
                strSqlSell.Append("  where ISReturn=0");//查询没有退还记录的商品 
                strSqlSell.Append("  and  CONVERT(varchar(100), dt_Sell, 23)=CONVERT(varchar(100), '" + dtDate.ToString() + "', 23)");
                DataTable dtSell = SQLHelper.ExecuteDataTable(strSqlSell.ToString());
                if (dtSell != null && dtSell.Rows.Count != 0)
                {

                    SellList.Style.Add("display", "block");

                    R_Sell.DataSource = dtSell;
                    R_Sell.DataBind();
                }
                else {
                    SellLog.Style.Add("display", "block");
                }


           

        }


        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (txtDate.Value.Trim() != "")
            {
                
                GetDepositor(txtDate.Value);
            }
           
        }

      

    }
}