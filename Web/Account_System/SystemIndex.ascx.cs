using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web.Account
{
    public partial class SystemIndex1 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //检测session
           
                //if (Session["UserGroup_ID"] == null || Session["UserGroup_ID"].ToString() != "1")
                //{
                //    Response.Redirect(@"~/default.aspx");
                //}
            }
        }
    }
}