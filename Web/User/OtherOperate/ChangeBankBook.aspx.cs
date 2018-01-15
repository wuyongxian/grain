using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.User.OtherOperate
{
    public partial class ChangeBankBook : System.Web.UI.Page
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
            divfrm.Style.Add("display", "none");
            //查询账户是否已经申请挂失

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
                divfrm.Style.Add("display", "block");
                D_strName.InnerText = dt.Rows[0]["strName"].ToString();
                D_strAddress.InnerText = dt.Rows[0]["strAddress"].ToString();
                D_PhoneNo.InnerText = dt.Rows[0]["PhoneNo"].ToString();
                txtC_AccountNumber.Value = dt.Rows[0]["AccountNumber"].ToString();
                D_AccountNumber.InnerText = dt.Rows[0]["AccountNumber"].ToString();
                D_numState.InnerText = "正常";
                if (dt.Rows[0]["numState"].ToString() == "0") {
                    D_numState.InnerText = "禁用";
                }
                D_IDCard.InnerText = dt.Rows[0]["IDCard"].ToString();
              

                //获取存粮信息
                DataTable dtStorage = common.getDepStorageInfo(AccountNumber);
                numTotol = common.GetLiXiTotal(dtStorage);
                if (dtStorage != null && dtStorage.Rows.Count != 0)
                {
                    StorageList.Style.Add("display", "block");
                    Repeater1.DataSource = dtStorage;
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