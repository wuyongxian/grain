using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Web.User.Storage
{
    public partial class EditError : System.Web.UI.Page
    {
        bool ISHQ = false;//是否是总部的管理员打开次界面
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Account"] != null)
            {
                GetDepositor(Request.QueryString["Account"].ToString());
            }
        }
        private void GetDepositor(string AccountNumber)
        {


            depositorInfo.Style.Add("display", "none");
          
            common.IsLogin();
            string WBID = Session["WB_ID"].ToString();//当前网点ID
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   select ID,WBID,AccountNumber,strPassword, CunID as BD_Address_CunID,strAddress,strName,PhoneNO,ISSendMessage,BankCardNO,dt_Update,");
            strSql.Append("   CASE( numState) WHEN 1 THEN '正常' ELSE '挂失' END AS numState,dt_Add,");
            strSql.Append("   CASE (IDCard) WHEN '' THEN '未填写' ELSE '******' END as IDCard");
            strSql.Append(" FROM dbo.Depositor ");
            strSql.Append(" where 1=1 ");
            if(!ISHQ)
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
                if (!common.CheckPassword(AccountNumber, QPassword.Value.Trim()))
                {
                    Fun.Alert("储户密码错误，请重新输入!");
                    return;
                }
                depositorInfo.Style.Add("display", "block");
                D_strName.InnerText = dt.Rows[0]["strName"].ToString();
                D_strAddress.InnerText = dt.Rows[0]["strAddress"].ToString();
                D_PhoneNo.InnerText = dt.Rows[0]["PhoneNo"].ToString();
                D_AccountNumber.InnerText = dt.Rows[0]["AccountNumber"].ToString();
                D_numState.InnerText = dt.Rows[0]["numState"].ToString();
                D_IDCard.InnerText = dt.Rows[0]["IDCard"].ToString();
              
                //获取存粮信息
                StringBuilder strSqlStorage = new StringBuilder();
                strSqlStorage.Append("SELECT A.ID,A.StorageNumber,A.StorageDate, A.AccountNumber,A.WeighNo,B.strName AS VarietyID,A.Price_ShiChang,A.Price_DaoQi,C.strName AS TimeID,A.StorageFee");
                strSqlStorage.Append("  FROM dbo.Dep_StorageInfo A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
                strSqlStorage.Append("  INNER JOIN dbo.StorageTime C ON A.TimeID=C.ID");
                strSqlStorage.Append("  WHERE  AccountNumber='" + AccountNumber + "'");
                if (!ISHQ)
                {
                    strSqlStorage.Append("  and  DATEDIFF(DAY,A.StorageDate,GETDATE())<1 " );
              
                }
                DataTable dtStorage = SQLHelper.ExecuteDataTable(strSqlStorage.ToString());
                if (dtStorage != null && dtStorage.Rows.Count != 0)
                {
                    Repeater1.DataSource = dtStorage;
                    Repeater1.DataBind();
                }
                else
                {
                    StorageInfo.InnerText = "该储户今天没有存储信息，不需要修改";
                    Repeater1.DataSource = null;
                    Repeater1.DataBind();
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
            ISHQ = common.ISHQWB(Session["WB_ID"]);//是否是总部的网点
            GetDepositor(QAccountNumber.Value.Trim());
         }

        public string GetDay(object date)
        {
            DateTime t1 = Convert.ToDateTime(date);
            TimeSpan ts = DateTime.Now.Subtract(t1);
            int numday = Convert.ToInt32(Math.Floor((decimal)ts.TotalDays));
            return numday.ToString();

        }

        public string GetLiXi()
        {

            return "￥0.00";

        }
    }
}