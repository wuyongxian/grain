using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Web.Admin.Commune
{
    public partial class CommuneQuery : System.Web.UI.Page
    {
        public double IDCount = 0;
        public double FieldCount = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                Get_Address_Xian();
            }
        }
        //private void DataBind(int pageindex, int pagesize)
        //{

         
        //    //获取存粮信息
        //    StringBuilder strSql = new StringBuilder();//sql语句
        //    StringBuilder strSqlPara = new StringBuilder();//查询参数
        //    strSql.Append("   SELECT top "+pagesize+" B.strName AS WBID,A.ID,A.AccountNumber,A.strAddress,A.strName,A.IDCard,A.PhoneNO,A.FieldCopies,A.FieldCount,");
        //    strSql.Append("  CONVERT(varchar(100), A.dt_Commune, 23) AS dt_Commune,SUBSTRING(A.ApplicationForm,2,200) AS ApplicationForm ,SUBSTRING(A.CommunePic,2,200) AS CommunePic ,CASE (A.numState) WHEN 1 THEN '正常' ELSE '禁用' END  AS numState");
           
        //    strSql.Append("    FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID");
        //    strSql.Append("   INNER JOIN dbo.BD_Address_Zu C ON A.ZuID=C.ID");
        //    strSql.Append("   INNER JOIN dbo.BD_Address_Cun D ON A.CunID=D.ID");
        //    strSql.Append("    INNER JOIN dbo.BD_Address_Xiang E ON A.XiangID=E.ID");
        //    strSql.Append("    INNER JOIN dbo.BD_Address_Xian F ON A.XianID=F.ID");
        //    strSql.Append("   where 1=1  and A.numState=1 and B.ISSimulate=0");//取消模拟账户的查询
        //    strSql.Append("   and A.ID not in (select top " + pagesize * pageindex + " ID from Commune ORDER BY ID ASC)");
        //    if (QWBID.Value.Trim() != "")
        //    {
        //        strSql.Append("   AND B.strName LIKE '%" + QWBID.Value.Trim() + "%'");
        //    }
        //    if (QAccountNumber.Value.Trim() != "")
        //    {
        //        strSql.Append("   AND A.AccountNumber LIKE '%" + QAccountNumber.Value.Trim() + "%'");
        //    }
        //    if (QstrName.Value.Trim() != "")
        //    {
        //        strSql.Append("   AND A.strName LIKE '%" + QstrName.Value.Trim() + "%'");
        //    }
        //    if (QIDCard.Value.Trim() != "")
        //    {
        //        strSql.Append("   AND A.IDCard LIKE '%" + QIDCard.Value.Trim() + "%'");
        //    }
        //    if (QPhoneNO.Value.Trim() != "")
        //    {
        //        strSql.Append("   AND A.PhoneNO LIKE '%" + QPhoneNO.Value.Trim() + "%'");
        //    }
        //    if (QFieldCopies.Value.Trim() != "")
        //    {
        //        strSql.Append("   AND A.FieldCopies LIKE '%" + QFieldCopies.Value.Trim() + "%'");
        //    }
        //    if (Qdt_Commune1.Value.Trim() != "")
        //    {
        //        strSql.Append("    AND A.dt_Commune>'" + Qdt_Commune1.Value.Trim() + "'");
        //    }
        //    if (Qdt_Commune2.Value.Trim() != "")
        //    {
        //        strSql.Append("    AND A.dt_Commune<'" + Qdt_Commune2.Value.Trim() + "'");
        //    } 
        //    if (XianID.SelectedValue.Trim() != "0" && XianID.SelectedValue.Trim() != "")
        //    {
        //        strSql.Append("    AND F.ID=" + XianID.SelectedValue.Trim());
        //    }
        //    if (XiangID.SelectedValue.Trim() != "0" && XiangID.SelectedValue.Trim() != "")
        //    {
        //        strSql.Append("    AND E.ID=" + XiangID.SelectedValue.Trim());
        //    }
        //    if (CunID.SelectedValue.Trim() != "0" && CunID.SelectedValue.Trim() != "")
        //    {
        //        strSql.Append("    AND D.ID=" + CunID.SelectedValue.Trim());
        //    }
        //    if (ZuID.SelectedValue.Trim() != "0" && ZuID.SelectedValue.Trim() != "")
        //    {
        //        strSql.Append("    AND C.ID=" + ZuID.SelectedValue.Trim());
        //    }

        //    strSql.Append("  ORDER BY  A.ID ASC");
        //    DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
        //    Repeater1.DataSource = dt;
        //    Repeater1.DataBind();





        //    GetCount(); //获取人数和田亩数
        //}

        private void DataBind(int pageindex, int pagesize)
        {
            string WBID = Session["WB_ID"].ToString();
            object objISSimulate = SQLHelper.ExecuteScalar("  SELECT ISSimulate FROM dbo.WB WHERE ID="+WBID);

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();//sql语句
            StringBuilder strSqlPara = new StringBuilder();//查询参数
            strSql.Append("   SELECT top " + pagesize + " B.strName AS WBID,A.ID,A.AccountNumber,A.strAddress,A.strName,A.IDCard,A.PhoneNO,A.FieldCopies,A.FieldCount,");
            strSql.Append("  CONVERT(varchar(100), A.dt_Commune, 23) AS dt_Commune,SUBSTRING(A.ApplicationForm,2,200) AS ApplicationForm ,SUBSTRING(A.CommunePic,2,200) AS CommunePic ,CASE (A.numState) WHEN 1 THEN '正常' ELSE '禁用' END  AS numState");

          

            strSqlPara.Append("    FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlPara.Append("   INNER JOIN dbo.BD_Address_Zu C ON A.ZuID=C.ID");
            strSqlPara.Append("   INNER JOIN dbo.BD_Address_Cun D ON A.CunID=D.ID");
            strSqlPara.Append("    INNER JOIN dbo.BD_Address_Xiang E ON A.XiangID=E.ID");
            strSqlPara.Append("    INNER JOIN dbo.BD_Address_Xian F ON A.XianID=F.ID");
            strSqlPara.Append("   where 1=1  and A.numState=1 ");//取消模拟账户的查询
            if (!Convert.ToBoolean(objISSimulate)) {
                strSqlPara.Append("    and B.ISSimulate=0");
            }
          //  strSql.Append("   and A.ID not in (select top " + pagesize * pageindex + " ID from Commune ORDER BY ID ASC)");
            if (QWBID.Value.Trim() != "")
            {
                strSqlPara.Append("   AND B.strName LIKE '%" + QWBID.Value.Trim() + "%'");
            }
            if (QAccountNumber.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.AccountNumber LIKE '%" + QAccountNumber.Value.Trim() + "%'");
            }
            if (QstrName.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.strName LIKE '%" + QstrName.Value.Trim() + "%'");
            }
            if (QIDCard.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.IDCard LIKE '%" + QIDCard.Value.Trim() + "%'");
            }
            if (QPhoneNO.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.PhoneNO LIKE '%" + QPhoneNO.Value.Trim() + "%'");
            }
            if (QFieldCopies.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.FieldCopies LIKE '%" + QFieldCopies.Value.Trim() + "%'");
            }
            if (Qdt_Commune1.Value.Trim() != "")
            {
                strSqlPara.Append("    AND A.dt_Commune>'" + Qdt_Commune1.Value.Trim() + "'");
            }
            if (Qdt_Commune2.Value.Trim() != "")
            {
                strSqlPara.Append("    AND A.dt_Commune<'" + Qdt_Commune2.Value.Trim() + "'");
            }
            if (XianID.SelectedValue.Trim() != "0" && XianID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND F.ID=" + XianID.SelectedValue.Trim());
            }
            if (XiangID.SelectedValue.Trim() != "0" && XiangID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND E.ID=" + XiangID.SelectedValue.Trim());
            }
            if (CunID.SelectedValue.Trim() != "0" && CunID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND D.ID=" + CunID.SelectedValue.Trim());
            }
            if (ZuID.SelectedValue.Trim() != "0" && ZuID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND C.ID=" + ZuID.SelectedValue.Trim());
            }

            strSql.Append(strSqlPara.ToString());
            strSql.Append("   and A.ID not in (");
            strSql.Append("   select top "+pagesize*pageindex+" A.ID ");
            strSql.Append(strSqlPara.ToString());
            strSql.Append("   ORDER BY B.ID ASC, ID ASC)  ");
            strSql.Append("  ORDER BY B.ID ASC, A.ID ASC");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            Repeater1.DataSource = dt;
            Repeater1.DataBind();





            GetCount(); //获取人数和田亩数
        }

       


        private void GetDataCount()
        {

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();//sql语句
            StringBuilder strSqlPara = new StringBuilder();//查询参数
            strSql.Append("  SELECT COUNT(A.ID)");
           

            strSqlPara.Append("    FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlPara.Append("   INNER JOIN dbo.BD_Address_Zu C ON A.ZuID=C.ID");
            strSqlPara.Append("   INNER JOIN dbo.BD_Address_Cun D ON A.CunID=D.ID");
            strSqlPara.Append("    INNER JOIN dbo.BD_Address_Xiang E ON A.XiangID=E.ID");
            strSqlPara.Append("    INNER JOIN dbo.BD_Address_Xian F ON A.XianID=F.ID");
            strSqlPara.Append("   where 1=1  and A.numState=1 and B.ISSimulate=0");//取消模拟账户的查询
            //  strSql.Append("   and A.ID not in (select top " + pagesize * pageindex + " ID from Commune ORDER BY ID ASC)");
            if (QWBID.Value.Trim() != "")
            {
                strSqlPara.Append("   AND B.strName LIKE '%" + QWBID.Value.Trim() + "%'");
            }
            if (QAccountNumber.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.AccountNumber LIKE '%" + QAccountNumber.Value.Trim() + "%'");
            }
            if (QstrName.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.strName LIKE '%" + QstrName.Value.Trim() + "%'");
            }
            if (QIDCard.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.IDCard LIKE '%" + QIDCard.Value.Trim() + "%'");
            }
            if (QPhoneNO.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.PhoneNO LIKE '%" + QPhoneNO.Value.Trim() + "%'");
            }
            if (QFieldCopies.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.FieldCopies LIKE '%" + QFieldCopies.Value.Trim() + "%'");
            }
            if (Qdt_Commune1.Value.Trim() != "")
            {
                strSqlPara.Append("    AND A.dt_Commune>'" + Qdt_Commune1.Value.Trim() + "'");
            }
            if (Qdt_Commune2.Value.Trim() != "")
            {
                strSqlPara.Append("    AND A.dt_Commune<'" + Qdt_Commune2.Value.Trim() + "'");
            }
            if (XianID.SelectedValue.Trim() != "0" && XianID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND F.ID=" + XianID.SelectedValue.Trim());
            }
            if (XiangID.SelectedValue.Trim() != "0" && XiangID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND E.ID=" + XiangID.SelectedValue.Trim());
            }
            if (CunID.SelectedValue.Trim() != "0" && CunID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND D.ID=" + CunID.SelectedValue.Trim());
            }
            if (ZuID.SelectedValue.Trim() != "0" && ZuID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND C.ID=" + ZuID.SelectedValue.Trim());
            }

            strSql.Append(strSqlPara.ToString());
            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
            AspNetPager1.RecordCount = Convert.ToInt32(obj);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            GetDataCount();
            DataBind(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);

        }

        protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
        {
            AspNetPager1.CurrentPageIndex = e.NewPageIndex;
            DataBind(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
        }

        void Get_Address_Xian()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName FROM dbo.BD_Address_Xian ");
            strSql.Append(" ORDER BY ISDefault DESC,numSort ASC ");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            XianID.Items.Clear();
            if (dt != null && dt.Rows.Count != 0)
            {
                XianID.Items.Add(new ListItem("--请选择--", "0"));
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    XianID.Items.Add(new ListItem(dt.Rows[i]["strName"].ToString(), dt.Rows[i]["ID"].ToString()));
                }
            }
            else
            {
                XianID.Items.Add(new ListItem("--请选择--", "0"));
            }
        }

        void Get_Address_Xiang()
        {
            string strXianID = XianID.SelectedValue;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName,WBID FROM dbo.BD_Address_Xiang ");
            strSql.Append(" where XianID=" + strXianID);
            strSql.Append(" ORDER BY ISDefault DESC,numSort ASC ");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            XiangID.Items.Clear();
            if (dt != null && dt.Rows.Count != 0)
            {
                XiangID.Items.Add(new ListItem("--请选择--", "0"));
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    XiangID.Items.Add(new ListItem(dt.Rows[i]["strName"].ToString(), dt.Rows[i]["ID"].ToString()));
                }
            }
            else
            {
                XiangID.Items.Add(new ListItem("--请选择--", "0"));
            }
        }

        void Get_Address_Cun()
        {
            string strXiangID = XiangID.SelectedValue;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName,WBID FROM dbo.BD_Address_Cun ");
            strSql.Append(" where XiangID=" + strXiangID);
            strSql.Append(" ORDER BY ISDefault DESC,numSort ASC ");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            CunID.Items.Clear();
            if (dt != null && dt.Rows.Count != 0)
            {
                CunID.Items.Add(new ListItem("--请选择--", "0"));
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    CunID.Items.Add(new ListItem(dt.Rows[i]["strName"].ToString(), dt.Rows[i]["ID"].ToString()));
                }
            }
            else
            {
                CunID.Items.Add(new ListItem("--请选择--", "0"));
            }
        }

        void Get_Address_Zu()
        {
            string strCunID = CunID.SelectedValue;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,strName FROM dbo.BD_Address_Zu ");
            strSql.Append(" where CunID=" + strCunID);
            strSql.Append(" ORDER BY ISDefault DESC,numSort ASC ");

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            ZuID.Items.Clear();
            if (dt != null && dt.Rows.Count != 0)
            {
                ZuID.Items.Add(new ListItem("--请选择--", "0"));
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    ZuID.Items.Add(new ListItem(dt.Rows[i]["strName"].ToString(), dt.Rows[i]["ID"].ToString()));
                }
            }
            else
            {
                ZuID.Items.Add(new ListItem("--请选择--", "0"));
            }
        }

        protected void XianID_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Address_Xiang();
        }

        protected void XiangID_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Address_Cun();
        }

        protected void CunID_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Address_Zu();
        }


        /// <summary>
        ///  //获取人数和田亩数
        /// </summary>
        private void GetCount()
        {
            StringBuilder strSqlCount = new StringBuilder();
            strSqlCount.Append("   SELECT Count(A.ID) as IDCount,SUM( A.FieldCount) as FieldCount");
            strSqlCount.Append("    FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlCount.Append("   INNER JOIN dbo.BD_Address_Zu C ON A.ZuID=C.ID");
            strSqlCount.Append("   INNER JOIN dbo.BD_Address_Cun D ON A.CunID=D.ID");
            strSqlCount.Append("    INNER JOIN dbo.BD_Address_Xiang E ON A.XiangID=E.ID");
            strSqlCount.Append("    INNER JOIN dbo.BD_Address_Xian F ON A.XianID=F.ID");
            strSqlCount.Append("   where 1=1  and A.numState=1 and B.ISSimulate=0");

            if (QWBID.Value.Trim() != "")
            {
                strSqlCount.Append("   AND B.strName LIKE '%" + QWBID.Value.Trim() + "%'");
            }
            if (QAccountNumber.Value.Trim() != "")
            {
                strSqlCount.Append("   AND A.AccountNumber LIKE '%" + QAccountNumber.Value.Trim() + "%'");
            }
            if (QstrName.Value.Trim() != "")
            {
                strSqlCount.Append("   AND A.strName LIKE '%" + QstrName.Value.Trim() + "%'");
            }
            if (QIDCard.Value.Trim() != "")
            {
                strSqlCount.Append("   AND A.IDCard LIKE '%" + QIDCard.Value.Trim() + "%'");
            }
            if (QPhoneNO.Value.Trim() != "")
            {
                strSqlCount.Append("   AND A.PhoneNO LIKE '%" + QPhoneNO.Value.Trim() + "%'");
            }
            if (QFieldCopies.Value.Trim() != "")
            {
                strSqlCount.Append("   AND A.FieldCopies LIKE '%" + QFieldCopies.Value.Trim() + "%'");
            }
            if (Qdt_Commune1.Value.Trim() != "")
            {
                strSqlCount.Append("    AND A.dt_Commune>'" + Qdt_Commune1.Value.Trim() + "'");
            }
            if (Qdt_Commune2.Value.Trim() != "")
            {
                strSqlCount.Append("    AND A.dt_Commune<'" + Qdt_Commune2.Value.Trim() + "'");
            }
            if (XianID.SelectedValue.Trim() != "0" && XianID.SelectedValue.Trim() != "")
            {
                strSqlCount.Append("    AND F.ID=" + XianID.SelectedValue.Trim());
            }
            if (XiangID.SelectedValue.Trim() != "0" && XiangID.SelectedValue.Trim() != "")
            {
                strSqlCount.Append("    AND E.ID=" + XiangID.SelectedValue.Trim());
            }
            if (CunID.SelectedValue.Trim() != "0" && CunID.SelectedValue.Trim() != "")
            {
                strSqlCount.Append("    AND D.ID=" + CunID.SelectedValue.Trim());
            }
            if (ZuID.SelectedValue.Trim() != "0" && ZuID.SelectedValue.Trim() != "")
            {
                strSqlCount.Append("    AND C.ID=" + ZuID.SelectedValue.Trim());
            }

            DataTable dtCount = SQLHelper.ExecuteDataTable(strSqlCount.ToString());
            if (dtCount != null && dtCount.Rows.Count != 0)
            {
                
                IDCount = Convert.ToDouble(dtCount.Rows[0]["IDCount"]);
                if (IDCount == 0)
                {
                    FieldCount = 0;
                }
                else
                {
                    FieldCount = Convert.ToDouble(dtCount.Rows[0]["FieldCount"]);
                }
            }
            else
            {
                IDCount = 0;
                FieldCount = 0;
            }
        }
        DataTable GetoutputTable()
        {

            //获取存粮信息
            StringBuilder strSql = new StringBuilder();//sql语句
            StringBuilder strSqlPara = new StringBuilder();//查询参数
            strSql.Append("   SELECT  B.strName AS WBID,A.AccountNumber,A.strAddress,A.strName,('@'+A.IDCard) as IDCard,A.PhoneNO,A.FieldCopies,A.FieldCount,");
            strSql.Append("  CONVERT(varchar(100), A.dt_Commune, 23) AS dt_Commune");



            strSqlPara.Append("    FROM dbo.Commune A INNER JOIN dbo.WB B ON A.WBID=B.ID");
            strSqlPara.Append("   INNER JOIN dbo.BD_Address_Zu C ON A.ZuID=C.ID");
            strSqlPara.Append("   INNER JOIN dbo.BD_Address_Cun D ON A.CunID=D.ID");
            strSqlPara.Append("    INNER JOIN dbo.BD_Address_Xiang E ON A.XiangID=E.ID");
            strSqlPara.Append("    INNER JOIN dbo.BD_Address_Xian F ON A.XianID=F.ID");
            strSqlPara.Append("   where 1=1  and A.numState=1 and B.ISSimulate=0");//取消模拟账户的查询
            //  strSql.Append("   and A.ID not in (select top " + pagesize * pageindex + " ID from Commune ORDER BY ID ASC)");
            if (QWBID.Value.Trim() != "")
            {
                strSqlPara.Append("   AND B.strName LIKE '%" + QWBID.Value.Trim() + "%'");
            }
            if (QAccountNumber.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.AccountNumber LIKE '%" + QAccountNumber.Value.Trim() + "%'");
            }
            if (QstrName.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.strName LIKE '%" + QstrName.Value.Trim() + "%'");
            }
            if (QIDCard.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.IDCard LIKE '%" + QIDCard.Value.Trim() + "%'");
            }
            if (QPhoneNO.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.PhoneNO LIKE '%" + QPhoneNO.Value.Trim() + "%'");
            }
            if (QFieldCopies.Value.Trim() != "")
            {
                strSqlPara.Append("   AND A.FieldCopies LIKE '%" + QFieldCopies.Value.Trim() + "%'");
            }
            if (Qdt_Commune1.Value.Trim() != "")
            {
                strSqlPara.Append("    AND A.dt_Commune>'" + Qdt_Commune1.Value.Trim() + "'");
            }
            if (Qdt_Commune2.Value.Trim() != "")
            {
                strSqlPara.Append("    AND A.dt_Commune<'" + Qdt_Commune2.Value.Trim() + "'");
            }
            if (XianID.SelectedValue.Trim() != "0" && XianID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND F.ID=" + XianID.SelectedValue.Trim());
            }
            if (XiangID.SelectedValue.Trim() != "0" && XiangID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND E.ID=" + XiangID.SelectedValue.Trim());
            }
            if (CunID.SelectedValue.Trim() != "0" && CunID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND D.ID=" + CunID.SelectedValue.Trim());
            }
            if (ZuID.SelectedValue.Trim() != "0" && ZuID.SelectedValue.Trim() != "")
            {
                strSqlPara.Append("    AND C.ID=" + ZuID.SelectedValue.Trim());
            }

            strSql.Append(strSqlPara.ToString());
           
            strSql.Append("  ORDER BY B.ID ASC, A.ID ASC");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            return dt;
        }

        private void outputExcel()
        {


            DataTable dt = GetoutputTable();
            if (dt == null || dt.Rows.Count == 0)
            {
                Fun.Alert("请先检索要查询的信息后在导出！");
                return;
            }

            string fileName = "社员信息" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".xls";

            HttpResponse resp;
            resp = Page.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            string colHeaders = "", ls_item = "",ls_Bottom="";

            DataRow[] myRow = dt.Select();//可以类似dt.Select("id>10")之形式达到数据筛选目的
            int i = 0;
            int cl = dt.Columns.Count;


            //取得数据表各列标题，各标题之间以\t分割，最后一个列标题后加回车符
            //for (i = 0; i < cl; i++)
            //{
            //    if (i == (cl - 1))//最后一列，加\n
            //    {
            //        colHeaders += dt.Columns[i].Caption.ToString() + "\n";
            //    }
            //    else
            //    {
            //        colHeaders += dt.Columns[i].Caption.ToString() + "\t";
            //    }

            //}
            colHeaders += "网点\t社员账号\t地址\t姓名\t身份证号\t手机号\t田亩册\t田亩数\t入社时间\n";
            resp.Write(colHeaders);
            //向HTTP输出流中写入取得的数据信息

            //逐行处理数据  
            foreach (DataRow row in myRow)
            {
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据    
                for (i = 0; i < cl; i++)
                {
                    if (i == (cl - 1))//最后一列，加\n
                    {
                        ls_item += row[i].ToString() + "\n";
                    }
                    else
                    {
                        ls_item += row[i].ToString() + "\t";
                    }
                }
                resp.Write(ls_item);
                ls_item = "";

            }
            GetCount();//再次获取统计数目
            ls_Bottom += "统计\t社员数:" + IDCount + "\t\t\t\t\t田亩数:\t" + FieldCount + "\t\n";
            resp.Write(ls_Bottom);
            resp.End();
        }

        protected void btnOutPut_Click(object sender, EventArgs e)
        {
            outputExcel();
        }


        public string GetUpdateItem(object AccountNumber)
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                return " <a href='/Admin/Commune/CommuneEdit.aspx?AccountNumber=" + AccountNumber.ToString() + "' >修改</a> ";
            }
            else
            {
                return "";
            }
        }


    }
}