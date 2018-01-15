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
    public partial class GoodSupplyReturn : System.Web.UI.Page
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
                if (!common.CheckPassword_Commune(AccountNumber, QPassword.Value.Trim()))
                {
                    Fun.Alert("储户密码错误，请重新输入!");
                    return;
                }

                if (dt.Rows[0]["numState"].ToString() == "0")//该社员被禁用
                {
                    Fun.Alert("该社员已被禁用，无法进行操作!");
                    return;
                }
               
                depositorInfo.Style.Add("display", "block");
               
                D_strName.InnerText = dt.Rows[0]["strName"].ToString();
              
                D_strAddress.InnerText = dt.Rows[0]["strAddress"].ToString();
                D_PhoneNo.InnerText = dt.Rows[0]["PhoneNo"].ToString();
                D_AccountNumber.InnerText = dt.Rows[0]["AccountNumber"].ToString();
                D_FieldCopies.InnerText = dt.Rows[0]["FieldCopies"].ToString();
                D_IDCard.InnerText = dt.Rows[0]["IDCard"].ToString();

                //获取社员当前的购买信息
                StringBuilder strReturn = new StringBuilder();
                strReturn.Append(" SELECT ID, GoodSupplyName,UnitName,GoodSupplyPrice,GoodSupplyCount,Money_YouHui,(Money_Total-Money_YouHui) AS Money_Return");
                strReturn.Append(" FROM dbo.C_Supply ");
                strReturn.Append(" WHERE C_AccountNumber='"+AccountNumber+"' AND DATEDIFF(DAY,dt_Trade,GETDATE())<1");
                strReturn.Append(" ");
                strReturn.Append(" ");

                DataTable dtReturn = SQLHelper.ExecuteDataTable(strReturn.ToString());
                Repeater2.DataSource = dtReturn;
                Repeater2.DataBind();

            }
            else
            {
                Fun.Alert("您查询的社员不存在");
                return;
            }



        }


        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Cache.Remove("Supply");//重新检索的时候，将之前的缓存清空
            if (QAccountNumber.Value.Trim() != "")
            {
                GetDepositor(QAccountNumber.Value.Trim());
            }
        }

    }
}