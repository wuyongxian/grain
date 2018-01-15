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
    public partial class GoodSupply : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["AccountNumber"] != null)
                {
                    QAccountNumber.Value = Request.QueryString["AccountNumber"].ToString();
                    GetDepositor(Request.QueryString["AccountNumber"].ToString());
                }
            }
        }

        /// <summary>
        /// 由账号查询社员信息
        /// </summary>
        /// <param name="AccountNumber"></param>
        private void GetDepositor(string AccountNumber)
        {
            depositorInfo.Style.Add("display", "none");
            Supply.Style.Add("display", "none");
            divPreDefine.Style.Add("display", "none");
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
                txtC_Name.Value = dt.Rows[0]["strName"].ToString();
                D_strAddress.InnerText = dt.Rows[0]["strAddress"].ToString();
                D_PhoneNo.InnerText = dt.Rows[0]["PhoneNo"].ToString();
                D_AccountNumber.InnerText = dt.Rows[0]["AccountNumber"].ToString();
                D_FieldCopies.InnerText = dt.Rows[0]["FieldCopies"].ToString();
                D_IDCard.InnerText = dt.Rows[0]["IDCard"].ToString();

                //历史交易信息
                StringBuilder strSqlSupply = new StringBuilder();
                strSqlSupply.Append(" select ID,GoodSupplyName,SpecName,UnitName,GoodSupplyCount,GoodSupplyPrice,Price_Commune,");
                strSqlSupply.Append("  Money_Total,Money_YouHui,Money_PreDefine,Money_Reality, CONVERT(NVARCHAR(100),dt_Trade,23) AS  dt_Trade ");
                strSqlSupply.Append("  FROM dbo.C_Supply ");
               
                strSqlSupply.Append("  ");
                strSqlSupply.Append("  WHERE C_AccountNumber='" + AccountNumber + "'");
                strSqlSupply.Append("  ORDER BY dt_Trade DESC");
                DataTable dtSupply = SQLHelper.ExecuteDataTable(strSqlSupply.ToString());
                if (dtSupply != null && dtSupply.Rows.Count != 0)
                {
                    Supply.Style.Add("display", "block");
                    Repeater1.DataSource = dtSupply;
                    Repeater1.DataBind();
                }

                //预存款信息
                StringBuilder strSqlStorage = new StringBuilder();
                strSqlStorage.Append("  SELECT top 1 A.ID,B.strRealName AS UserID,A.C_AccountNumber,A.Money_PreDefine,C.strName AS GoodSupplyID,CONVERT(varchar(100), A.dt_Trade, 23) AS dt_Trade,DATEDIFF(DAY,A.dt_Trade,GETDATE()) AS numday");
                strSqlStorage.Append("  FROM dbo.C_PreDefine A INNER JOIN dbo.Users B ON A.UserID=B.ID");
                strSqlStorage.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
                strSqlStorage.Append("  WHERE ISUsed=0 and  C_AccountNumber='" + AccountNumber + "'");
                DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
                if (dtStorage != null && dtStorage.Rows.Count != 0)
                {
                    divPreDefine.Style.Add("display", "block");
                    RPreDefine.DataSource = dtStorage;
                    RPreDefine.DataBind();
                }
               

            }
            else
            {
                Fun.Alert("您查询的社员不存在");
                return;
            }



        }

        private void GetDepositor()
        {
            depositorInfo.Style.Add("display", "none");
            Supply.Style.Add("display", "none");
            divPreDefine.Style.Add("display", "none");
            divfrm.Style.Add("display", "none");
            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();//当前网点ID
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  select ID,WBID,AccountNumber,strPassword,ZuID,strAddress,strName,IDCard,PhoneNO,FieldCopies,FieldCount,ApplicationForm,CommunePic,dt_Commune,numState");
            strSql.Append("  FROM dbo.Commune");

            strSql.Append(" where 1=1 and numState=1 and WBID="+WBID);

            if (Convert.ToBoolean(common.GetWBAuthority()["Enable_Distance"]) == false)//不允许异地存取的情况
            {
                strSql.Append(" and WBID= " + WBID);
            }

            if (QAccountNumber.Value.Trim() != "")
            {
                strSql.Append(" and AccountNumber like  '%"+(QAccountNumber.Value.Trim() +"%'"));
            }
            if (QstrName.Value.Trim() != "")
            {
                strSql.Append(" and strName like '%" + (QstrName.Value.Trim() + "%'"));
            }
            if (QIDCard.Value.Trim() != "")
            {
                strSql.Append(" and IDCard like '%" + (QIDCard.Value.Trim() + "%'"));
            }
            if (QPhoneNO.Value.Trim() != "")
            {
                strSql.Append(" and PhoneNO like '%" + (QPhoneNO.Value.Trim() + "%'"));
            }

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            if (dt != null && dt.Rows.Count != 0)
            {
                if (dt.Rows.Count == 1)
                {
                   // txtC_AccountNumber.Value = dt.Rows[0]["AccountNumber"].ToString();
                   // Response.Write(txtC_AccountNumber.Value + dt.Rows[0]["AccountNumber"].ToString());

                    //txtC_AccountNumber.Attributes["Value"] = dt.Rows[0]["AccountNumber"].ToString();
                    QAccountNumber.Value = dt.Rows[0]["AccountNumber"].ToString();
                    divCommuneList.Style.Add("display", "none");
                    if (dt.Rows[0]["numState"].ToString() == "0")//该社员被禁用
                    {
                        Fun.Alert("该社员已被禁用，无法进行操作!");
                        return;
                    }
                    txtC_AccountNumber.Value = QAccountNumber.Value.Trim();
                    depositorInfo.Style.Add("display", "block");
                    divfrm.Style.Add("display", "block");
                    D_strName.InnerText = dt.Rows[0]["strName"].ToString();
                    txtC_Name.Value = dt.Rows[0]["strName"].ToString();
                    D_strAddress.InnerText = dt.Rows[0]["strAddress"].ToString();
                    D_PhoneNo.InnerText = dt.Rows[0]["PhoneNo"].ToString();
                    D_AccountNumber.InnerText = dt.Rows[0]["AccountNumber"].ToString();
                    D_FieldCopies.InnerText = dt.Rows[0]["FieldCopies"].ToString();
                    D_IDCard.InnerText = dt.Rows[0]["IDCard"].ToString();

                    //历史交易信息
                    StringBuilder strSqlSupply = new StringBuilder();
                    strSqlSupply.Append(" select ID,GoodSupplyName,SpecName,UnitName,GoodSupplyCount,GoodSupplyPrice,Price_Commune,");
                    strSqlSupply.Append("  Money_Total,Money_YouHui,Money_PreDefine,Money_Reality, CONVERT(NVARCHAR(100),dt_Trade,23) AS  dt_Trade ");
                    strSqlSupply.Append("  FROM dbo.C_Supply ");

                    strSqlSupply.Append("  ");
                    strSqlSupply.Append("  WHERE C_AccountNumber='" + QAccountNumber.Value.Trim() + "'");
                    strSqlSupply.Append("  ORDER BY dt_Trade DESC");
                    DataTable dtSupply = SQLHelper.ExecuteDataTable(strSqlSupply.ToString());
                    if (dtSupply != null && dtSupply.Rows.Count != 0)
                    {
                        Supply.Style.Add("display", "block");
                        Repeater1.DataSource = dtSupply;
                        Repeater1.DataBind();
                    }

                    //预存款信息
                    StringBuilder strSqlStorage = new StringBuilder();
                    strSqlStorage.Append("  SELECT top 1 A.ID,B.strRealName AS UserID,A.C_AccountNumber,A.Money_PreDefine,C.strName AS GoodSupplyID,CONVERT(varchar(100), A.dt_Trade, 23) AS dt_Trade,DATEDIFF(DAY,A.dt_Trade,GETDATE()) AS numday");
                    strSqlStorage.Append("  FROM dbo.C_PreDefine A INNER JOIN dbo.Users B ON A.UserID=B.ID");
                    strSqlStorage.Append("  INNER JOIN dbo.GoodSupply C ON A.GoodSupplyID=C.ID");
                    strSqlStorage.Append("  WHERE ISUsed=0 and  C_AccountNumber='" + QAccountNumber.Value.Trim() + "'");
                    DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
                    if (dtStorage != null && dtStorage.Rows.Count != 0)
                    {
                        divPreDefine.Style.Add("display", "block");
                        RPreDefine.DataSource = dtStorage;
                        RPreDefine.DataBind();
                    }


                }
                else {
                    divCommuneList.Style.Add("display", "block");
                    rptCommuneList.DataSource = dt;
                    rptCommuneList.DataBind();
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
            Cache.Remove("Supply");//重新检索的时候，将之前的缓存清空
            GetDepositor();
        }

    }
}