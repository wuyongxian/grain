using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web.test
{
    public partial class log4netTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
           Common.MyLog4NetInfo.LogInfo("错误日志test");
           Common.MyLog4NetInfo.LogInfo("错误日志test");
           Common.MyLog4NetInfo.DebugInfo("错误日志test");
        }
    }
}