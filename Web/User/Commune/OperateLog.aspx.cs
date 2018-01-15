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
    public partial class OperateLog : System.Web.UI.Page
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
             depositorInfo.Style.Add("display", "none");
          
            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();//当前网点ID
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  select ID,WBID,AccountNumber,strPassword,ZuID,strAddress,strName,IDCard,PhoneNO,FieldCopies,FieldCount,ApplicationForm,CommunePic,dt_Commune,numState");
            strSql.Append("  FROM dbo.Commune");
            strSql.Append("  ");
            strSql.Append("  ");
            strSql.Append(" where 1=1 and numState=1 and WBID= " + WBID);

            strSql.Append(" and AccountNumber=@AccountNumber ");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50)};
            parameters[0].Value = AccountNumber;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dt != null && dt.Rows.Count != 0)
            {
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

            }

            StringBuilder strSqlCommune = new StringBuilder();
            strSqlCommune.Append("  SELECT A.ID,A.BusinessName, B.strName AS WBID,C.strRealName AS UserID,A.C_AccountNumber,A.BusinessNO,A.VarietyID,A.UnitID,");
            strSqlCommune.Append("  A.Price,A.Price_C,A.CountTrade, A.Money_Trade,A.Money_Reality,A.numDisCount,CONVERT(NVARCHAR(100),A.dt_Trade,23) AS  dt_Trade");
            strSqlCommune.Append("  FROM dbo.C_OperateLog A INNER JOIN WB B ON A.WBID=B.ID");
            strSqlCommune.Append("  INNER JOIN dbo.Users C ON A.UserID=C.ID");
            strSqlCommune.Append("  ");
            strSqlCommune.Append("  where 1=1");
            strSqlCommune.Append("   AND A.C_AccountNumber = '" + AccountNumber + "'");
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
        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (QAccountNumber.Value.Trim() != "")
            {
                DataBind(QAccountNumber.Value.Trim());
            }

        }
    }
}