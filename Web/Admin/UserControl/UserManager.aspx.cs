using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace Web.Admin.UserControl
{
    public partial class UserManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            common.IsLogin();
            if (!IsPostBack)
            {
                GetDataCount();
                DataBind(AspNetPager1.CurrentPageIndex - 1, AspNetPager1.PageSize);
            }
        }

        private void GetDataCount()
        {


            //获取存粮信息
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT count(A.ID)");
            strSql.Append(" FROM dbo.Users A INNER JOIN dbo.UserGroup B ON A.UserGroup_ID=B.ID");
            strSql.Append(" INNER JOIN dbo.WB ON A.WB_ID=WB.ID");
            strSql.Append(" WHERE B.strName!='系统管理员' ");
           if (txtType.Value.Trim() != "")
            {
                strSql.Append("  AND A.strLoginName='" + txtType.Value.Trim() + "'");
            }

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
            AspNetPager1.RecordCount = Convert.ToInt32(obj);
        }

        public void DataBind(int pageindex, int pagesize)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT TOP "+pagesize+" A.ID,WB.ID as wbid, WB.strName AS WBName, A.SerialNumber,A.strRealName,A.strLoginName, B.strName AS UserGroup_ID,");
            strSql.Append(" CASE A.ISEnable WHEN 1 THEN '启用' ELSE '禁用' END  AS ISEnable ");
            strSql.Append(" FROM dbo.Users A INNER JOIN dbo.UserGroup B ON A.UserGroup_ID=B.ID");
            strSql.Append(" INNER JOIN dbo.WB ON A.WB_ID=WB.ID");
            strSql.Append(" WHERE B.strName!='系统管理员' ");
            strSql.Append("   and A.ID not in (select top " + pagesize * pageindex + " ID from Users)");
            if (txtType.Value.Trim() != "")
            {
                strSql.Append("  AND A.strLoginName='" + txtType.Value.Trim() + "'");
            }
            strSql.Append(" ORDER BY A.dt_Add");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());
            this.Repeater1.DataSource = dt.DefaultView;
            this.Repeater1.DataBind();
        }

        public string SetISAllowBackUp(object flag)
        {

            if (Convert.ToBoolean(flag) == true)
            {
                return "<span style='color:red;'>是</span>";
            }
            else
            {
                return "<span style='color:#555;'>否</span>";
            }
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

        public string GetAddItem()
        {
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "A");
            if (Authority && Session["UserGroup_Name"].ToString()=="单位管理员")
            {
                return " <a href='#' onclick='ShowFrm(0)'>添加营业员</a>";
            }
            else
            {
                return "";
            }
        }

        public string GetDelItem(object ID, object WBID, object UserGroup_ID)
        {

            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority && Session["UserGroup_Name"].ToString() == "单位管理员")
            {
                return "<input type='button' style='width:80px; height:25px;'  value='删除'  onclick='FunDel(" + ID.ToString() + ",\"" + UserGroup_ID.ToString() + "\")' />";
                //if (UserGroup_ID.ToString() == "单位管理员")
                //{
                //    return "<input type='button' style='width:80px; height:25px;' disabled='disabled'  value='删除'   />";
                //}
                //else
                //{
                //    return "<input type='button' style='width:80px; height:25px;'  value='删除'  onclick='FunDel(" + ID.ToString() + "," + UserGroup_ID.ToString() + ")' />";
                //}
            }
            else
            {
                return "<input type='button' style='width:80px; height:25px;' disabled='disabled'  value='删除'   />";
            }
        }



        public string GetUpdateItem(object ID,object WBID,object UserGroup_ID)
        {
            
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                
                    if (UserGroup_ID.ToString() == "单位管理员")
                    {
                        if (Session["UserGroup_ID"].ToString() == "2")
                        {
                            return "<input type='button'  value='查看/修改' style='width:80px; height:25px;'  onclick='ShowFrmEdit(" + ID.ToString() + "," + WBID + ",\"" + UserGroup_ID + "\")' />";
                        }
                        else {
                            return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='查看/修改'   />";
                        }
                    }
                    else if (UserGroup_ID.ToString() == "网点管理员")
                    {
                        if (Session["UserGroup_Name"].ToString() == "单位管理员")
                        {
                            return "<input type='button'  value='查看/修改' style='width:80px; height:25px;'  onclick='ShowFrmEdit(" + ID.ToString() + "," + WBID + ",\"" + UserGroup_ID + "\")' />";
                        }
                        else
                        {
                            return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='查看/修改'   />";
                        }
                    }
                    else {
                        return "<input type='button'  value='查看/修改' style='width:80px; height:25px;'  onclick='ShowFrmEdit(" + ID.ToString() + "," + WBID + ",\"" + UserGroup_ID + "\")' />";
                    }
            }
            else
            {
                return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='查看/修改'   />";
            }
        }

        public string GetAnthorityItem(object ID,object UserGroup_ID)
        {
           
            bool Authority = common.GetAuthority(Session["UserGroup_Name"], Session["ID"], Request.QueryString["MenuID"], "E");
            if (Authority)
            {
                if (UserGroup_ID.ToString() == "单位管理员")
                {
                    return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='权限设置' />";
                }
                else if (UserGroup_ID.ToString() == "网点管理员")
                {
                    if (Session["UserGroup_Name"].ToString() == "单位管理员")
                    {
                        return "<input type='button'  value='权限设置' style='width:80px; height:25px;'  onclick='FunMenu(" + ID.ToString() + ",\"" + UserGroup_ID + "\")' />";
                    }
                    else {
                        return "<input type='button'  value='权限设置' style='width:80px; height:25px;'  onclick='FunMenu(" + ID.ToString() + ",\"" + UserGroup_ID + "\")' />";
                    }
                }
                else
                {
                    return "<input type='button'  value='权限设置' style='width:80px; height:25px;'  onclick='FunMenu(" + ID.ToString() + ",\"" + UserGroup_ID + "\")' />";
                }
            }
            else
            {
                return "<input type='button' disabled='disabled' style='width:80px; height:25px;'  value='权限设置' />";
            }
        }

    }
}