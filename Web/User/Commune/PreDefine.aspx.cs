using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.User.Commune
{
    public partial class PreDefine : System.Web.UI.Page
    {
       
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
            divfrm.Style.Add("display", "none");
            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();//当前网点ID
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  select ID,WBID,AccountNumber,strPassword,ZuID,strAddress,strName,IDCard,PhoneNO,FieldCopies,FieldCount,ApplicationForm,CommunePic,dt_Commune,numState");
            strSql.Append("  FROM dbo.Commune");
            strSql.Append(" where 1=1 and numState=1");

            if (Convert.ToBoolean(common.GetWBAuthority()["Enable_Distance"]) == false)//不允许异地存取的情况
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
                //if (!common.CheckPassword_Commune(AccountNumber, QPassword.Value.Trim()))
                //{
                //    Fun.Alert("储户密码错误，请重新输入!");
                //    return;
                //}

                if (dt.Rows[0]["numState"].ToString() == "0")//该社员被禁用
                {
                    Fun.Alert("该社员已被禁用，无法进行操作!");
                    return;
                }
                txtC_AccountNumber.Value = AccountNumber;
                depositorInfo.Style.Add("display", "block");
                divfrm.Style.Add("display", "block");
                D_strName.InnerText = dt.Rows[0]["strName"].ToString();
                D_strAddress.InnerText = dt.Rows[0]["strAddress"].ToString();
                D_PhoneNo.InnerText = dt.Rows[0]["PhoneNo"].ToString();
                D_AccountNumber.InnerText = dt.Rows[0]["AccountNumber"].ToString();
                D_FieldCopies.InnerText = dt.Rows[0]["FieldCopies"].ToString();
                D_IDCard.InnerText = dt.Rows[0]["IDCard"].ToString();
                
                //获取预存款
                StringBuilder strSqlStorage = new StringBuilder();
                strSqlStorage.Append("  SELECT top 1 A.ID,B.strRealName AS UserID,A.C_AccountNumber,A.Money_PreDefine, C.strName AS GoodSupplyID,CONVERT(varchar(100), A.dt_Trade, 23) AS dt_Trade,DATEDIFF(DAY,A.dt_Trade,GETDATE()) AS numday");
                strSqlStorage.Append("  FROM dbo.C_PreDefine A INNER JOIN dbo.Users B ON A.UserID=B.ID");
                strSqlStorage.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
                strSqlStorage.Append("  WHERE ISUsed=0 and C_AccountNumber='" + AccountNumber + "'");
                DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
                if (dtStorage != null && dtStorage.Rows.Count != 0)
                {
                    StorageList.Style.Add("display", "block");
                    Repeater1.DataSource = dtStorage;
                    Repeater1.DataBind();
                }
            }
            else
            {
                Fun.Alert("您查询的社员不存在");
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

    

      
    }
}