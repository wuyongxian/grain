using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Web.SessionState;


namespace Web.Ashx
{
    /// <summary>
    /// depositor 的摘要说明
    /// </summary>
    public class depositor : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            common.IsLogin();
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "GetDepositorByAccountNumber": GetDepositorByAccountNumber(context); break;
                    case "UpdateDepositorState": UpdateDepositorState(context); break;
                    case "CloseDepositor": CloseDepositor(context); break;
                  
                }
            }

        }

     
        /// <summary>
        /// 根据储户编号查询储户
        /// </summary>
        /// <param name="context"></param>
        void GetDepositorByAccountNumber(HttpContext context)
        {
            string AccountNumber = context.Request.QueryString["AN"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select ID,WBID,AccountNumber,strPassword,strAddress,CunID as  BD_Address_CunID,strName,IDCard,PhoneNO,ISSendMessage,BankCardNO,numState,dt_Add,dt_Update");
            strSql.Append(" FROM [Depositor] ");
            strSql.Append(" where AccountNumber=@AccountNumber ");
            SqlParameter[] parameters = {
					new SqlParameter("@AccountNumber", SqlDbType.NVarChar,50)};
            parameters[0].Value = AccountNumber;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(), parameters);
            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        /// <summary>
        /// 更新用户状态（挂失与解除挂失）
        /// </summary>
        /// <param name="context"></param>
        void UpdateDepositorState(HttpContext context)
        {
           
            string AccountNumber = context.Request.QueryString["AN"].ToString();
            string numState = context.Request.Form["GuaShi"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" UPDATE dbo.Depositor SET numState="+numState);
            strSql.Append("  WHERE AccountNumber='"+AccountNumber+"'");
           
          
            if (SQLHelper.ExecuteNonQuery(strSql.ToString())!=0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        /// <summary>
        /// 更新用户状态(销户)
        /// </summary>
        /// <param name="context"></param>
        void CloseDepositor(HttpContext context)
        {
            string AccountNumber = context.Request.QueryString["AN"].ToString();
          
            //StringBuilder strSql = new StringBuilder();
            //strSql.Append(" delete dbo.Depositor " );
            //strSql.Append("  WHERE AccountNumber='" + AccountNumber + "'");
            string strSql = string.Format("  UPDATE dbo.Depositor SET ISClosing=1 WHERE AccountNumber='{0}'",AccountNumber);

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }




        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}