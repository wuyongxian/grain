using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace Web.Admin.UserControl
{
    /// <summary>
    /// user 的摘要说明
    /// </summary>
    public class user : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "GetWB": GetWB(context); break;
                    case "GetUserGroup": GetUserGroup(context); break;
                    case "AddUser": AddUser(context); break;
                    case "UpdateUser": UpdateUser(context); break;
                    case "GetUserByID": GetUserByID(context); break;
                    case "DeleteUserByID": DeleteUserByID(context); break;

                    case "ClearError": ClearError(context); break;


                    case "getMenuAdmin": getMenuAdmin(context); break;
                    case "updateMenuAdmin": updateMenuAdmin(context); break;

                    case "getMenuUser": getMenuUser(context); break;
                    case "updateMenuUser": updateMenuUser(context); break;

                    case "getMenuUserAll": getMenuUserAll(context); break;
                    case "getMenuAdminAll": getMenuAdminAll(context); break;
                        
                }
            }

        }

        void updateMenuAdmin(HttpContext context)
        {
            string UserID = context.Request.QueryString["UserID"].ToString();
            int numtype =1;//管理员菜单
            Dictionary<int,string> dicMenuList=new Dictionary<int,string> ();
            foreach (var key in context.Request.Form.Keys)
            {
                if (key.ToString().IndexOf("chk") == -1)
                {
                    continue;
                }
                string strKey = key.ToString();
                string strValue = context.Request.Form[key.ToString()].ToString();
                int MenuID =Convert.ToInt32( key.ToString().Substring(key.ToString().IndexOf('_') + 1));
                string menuType = key.ToString().Substring(3, 1);//菜单类型（R\A\E）

                if (dicMenuList.Keys.Contains(MenuID))
                {
                    string menuValue = dicMenuList[MenuID];
                    dicMenuList[MenuID] = menuValue + menuType;
                }
                else {
                    dicMenuList.Add(MenuID, menuType);
                }
            }

            StringBuilder strMenuList = new StringBuilder();

            foreach (int i in dicMenuList.Keys)
            {
                if (strMenuList.ToString() == "")
                {
                    strMenuList.Append(i + ":" + dicMenuList[i]);
                }
                else {
                    strMenuList.Append("|"+i + ":" + dicMenuList[i]);
                }
            }

            int userMenuCount = Convert.ToInt32(SQLHelper.ExecuteScalar(" SELECT COUNT(ID) FROM UserMenu WHERE UserID="+UserID+" AND numtype="+numtype));

            StringBuilder strSql=new StringBuilder ();
            if (userMenuCount == 0)//该用户尚没有菜单
            {
                strSql.Append(" INSERT INTO UserMenu");
                strSql.Append(" (UserID,menuList,numtype,strRemark)");
                strSql.Append(" VALUES");
                strSql.Append(" (" + UserID + ",'" + strMenuList.ToString() + "'," + numtype + ",'')");
                strSql.Append(" ");

            }
            else {
                strSql.Append(" UPDATE UserMenu");
                strSql.Append(" SET menuList='"+strMenuList+"'");
                strSql.Append(" WHERE UserID="+UserID+" AND numtype="+numtype);
                strSql.Append(" ");
            }
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else { context.Response.Write("Error"); }
            
            
        }

        void getMenuAdmin(HttpContext context)
        {


            string PID = context.Request.QueryString["PID"].ToString();
            string UserID = context.Request.QueryString["UserID"].ToString();
          
            DataTable dt = GetMenuAdminTable(Convert.ToInt32(PID), UserID);
            dt.Columns.Add("HasChild", typeof(int));//添加标识列，标识该菜单是否有子菜单
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strSql = " select count(ID) from Menu_Admin where PID=@PID";
                SqlParameter para = new SqlParameter("@PID", dt.Rows[i]["ID"].ToString());
                string strCount = SQLHelper.ExecuteScalar(strSql, para).ToString();
                if (strCount != "0")
                {
                    dt.Rows[i]["HasChild"] = 1;
                }
                else
                {
                    dt.Rows[i]["HasChild"] = 0;
                }
            }

            context.Response.Write(JsonHelper.ToJson(dt));
        }

        DataTable GetMenuAdminTable(int PID, string UserID)
        {
            // string strSql = " SELECT ID,PID,strName,strUrl,Seq FROM Menu where PID="+PID;
            string strSql = " SELECT ID,PID,strValue,strUrl,numSort,ISEnable,ISSysW FROM Menu_Admin where PID=@PID and ISEnable=1  order by numSort";
            SqlParameter para = new SqlParameter("@PID", PID);

            DataTable dt = SQLHelper.ExecuteDataTable(strSql, para);
          //求管理员的菜单
            object objmenuList = SQLHelper.ExecuteScalar(" SELECT menuList FROM UserMenu WHERE UserID="+UserID+" AND numtype=1");
            Dictionary<int, string> dicMenuList = new Dictionary<int, string>();
            if (objmenuList != null && objmenuList.ToString() != "")
            {
                string[] ArraymenuList = objmenuList.ToString().Split('|');
                for (int i = 0; i < ArraymenuList.Length; i++)
                { 
                    string strmenu=ArraymenuList[i];
                    int menuKey=Convert.ToInt32(strmenu.Substring(0,strmenu.IndexOf(':')));
                    string menuValue=strmenu.Substring(strmenu.IndexOf(':')+1);
                    dicMenuList.Add(menuKey, menuValue);
                }
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            dt.Columns.Add("R", typeof(int));//添加标识列，当前用户是否有菜单的读权限
            dt.Columns.Add("A", typeof(int));//添加标识列，当前用户是否有菜单的添加权限
            dt.Columns.Add("E", typeof(int));//添加标识列，当前用户是否有菜单的编辑权限
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (objmenuList == null || objmenuList.ToString() == "")//该用户尚未写入菜单记录
                {
                    dt.Rows[i]["R"] = 0;
                    dt.Rows[i]["A"] = 0;
                    dt.Rows[i]["E"] = 0;
                }
                int ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                if (dicMenuList.Keys.Contains(ID))
                {
                    if (dicMenuList[ID].Contains("R"))
                    {
                        dt.Rows[i]["R"] = 1;
                    }
                    else
                    {
                        dt.Rows[i]["R"] = 0;
                    }
                    if (dicMenuList[ID].Contains("A"))
                    {
                        dt.Rows[i]["A"] = 1;
                    }
                    else
                    {
                        dt.Rows[i]["A"] = 0;
                    }
                    if (dicMenuList[ID].Contains("E"))
                    {
                        dt.Rows[i]["E"] = 1;
                    }
                    else
                    {
                        dt.Rows[i]["E"] = 0;
                    }
                }
                else {
                    dt.Rows[i]["R"] = 0;
                    dt.Rows[i]["E"] = 0;
                }
            }

            return dt;
        }


        void updateMenuUser(HttpContext context)
        {
            string UserID = context.Request.QueryString["UserID"].ToString();
            int numtype = 2;//管理员菜单
            Dictionary<int, string> dicMenuList = new Dictionary<int, string>();
            foreach (var key in context.Request.Form.Keys)
            {
                if (key.ToString().IndexOf("chk") == -1)
                {
                    continue;
                }
                string strKey = key.ToString();
                string strValue = context.Request.Form[key.ToString()].ToString();
                int MenuID = Convert.ToInt32(key.ToString().Substring(key.ToString().IndexOf('_') + 1));
                string menuType = key.ToString().Substring(3, 1);//菜单类型（R或者E）

                if (dicMenuList.Keys.Contains(MenuID))
                {
                    string menuValue = dicMenuList[MenuID];
                    dicMenuList[MenuID] = menuValue + menuType;
                }
                else
                {
                    dicMenuList.Add(MenuID, menuType);
                }
            }

            StringBuilder strMenuList = new StringBuilder();

            foreach (int i in dicMenuList.Keys)
            {
                if (strMenuList.ToString() == "")
                {
                    strMenuList.Append(i + ":" + dicMenuList[i]);
                }
                else
                {
                    strMenuList.Append("|" + i + ":" + dicMenuList[i]);
                }
            }

            int userMenuCount = Convert.ToInt32(SQLHelper.ExecuteScalar(" SELECT COUNT(ID) FROM UserMenu WHERE UserID=" + UserID + " AND numtype=" + numtype));

            StringBuilder strSql = new StringBuilder();
            if (userMenuCount == 0)//该用户尚没有菜单
            {
                strSql.Append(" INSERT INTO UserMenu");
                strSql.Append(" (UserID,menuList,numtype,strRemark)");
                strSql.Append(" VALUES");
                strSql.Append(" (" + UserID + ",'" + strMenuList.ToString() + "'," + numtype + ",'')");
                strSql.Append(" ");

            }
            else
            {
                strSql.Append(" UPDATE UserMenu");
                strSql.Append(" SET menuList='" + strMenuList + "'");
                strSql.Append(" WHERE UserID=" + UserID + " AND numtype=" + numtype);
                strSql.Append(" ");
            }
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else { context.Response.Write("Error"); }


        }

        void getMenuUser(HttpContext context)
        {


            string PID = context.Request.QueryString["PID"].ToString();
            string UserID = context.Request.QueryString["UserID"].ToString();
          
            DataTable dt = GetMenuUserTable(Convert.ToInt32(PID), UserID);
            dt.Columns.Add("HasChild", typeof(int));//添加标识列，标识该菜单是否有子菜单
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strSql = " select count(ID) from Menu_User where PID=@PID";
                SqlParameter para = new SqlParameter("@PID", dt.Rows[i]["ID"].ToString());
                string strCount = SQLHelper.ExecuteScalar(strSql, para).ToString();
                if (strCount != "0")
                {
                    dt.Rows[i]["HasChild"] = 1;
                }
                else
                {
                    dt.Rows[i]["HasChild"] = 0;
                }
            }

            context.Response.Write(JsonHelper.ToJson(dt));
        }

        DataTable GetMenuUserTable(int PID, string UserID)
        {
            int numtype = 2;
            // string strSql = " SELECT ID,PID,strName,strUrl,Seq FROM Menu where PID="+PID;
            string strSql = " SELECT ID,PID,strValue,strUrl,numSort,ISEnable FROM Menu_User where PID=@PID and ISEnable=1  order by numSort";
            SqlParameter para = new SqlParameter("@PID", PID);

            DataTable dt = SQLHelper.ExecuteDataTable(strSql, para);
            //求管理员的菜单
            object objmenuList = SQLHelper.ExecuteScalar(" SELECT menuList FROM UserMenu WHERE UserID=" + UserID + " AND numtype="+numtype);
            Dictionary<int, string> dicMenuList = new Dictionary<int, string>();
            if (objmenuList != null && objmenuList.ToString() != "")
            {
                string[] ArraymenuList = objmenuList.ToString().Split('|');
                for (int i = 0; i < ArraymenuList.Length; i++)
                {
                    string strmenu = ArraymenuList[i];
                    int menuKey = Convert.ToInt32(strmenu.Substring(0, strmenu.IndexOf(':')));
                    string menuValue = strmenu.Substring(strmenu.IndexOf(':') + 1);
                    dicMenuList.Add(menuKey, menuValue);
                }
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            dt.Columns.Add("R", typeof(int));//添加标识列，当前用户是否有菜单的读权限
            dt.Columns.Add("A", typeof(int));//添加标识列，当前用户是否有菜单的读权限
            dt.Columns.Add("E", typeof(int));//添加标识列，当前用户是否有菜单的编辑权限
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (objmenuList == null || objmenuList.ToString() == "")//该用户尚未写入菜单记录
                {
                    dt.Rows[i]["R"] = 0;
                    dt.Rows[i]["A"] = 0;
                    dt.Rows[i]["E"] = 0;
                }
                int ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                if (dicMenuList.Keys.Contains(ID))
                {
                    if (dicMenuList[ID].Contains("R"))
                    {
                        dt.Rows[i]["R"] = 1;
                    }
                    else
                    {
                        dt.Rows[i]["R"] = 0;
                    }
                    if (dicMenuList[ID].Contains("A"))
                    {
                        dt.Rows[i]["A"] = 1;
                    }
                    else
                    {
                        dt.Rows[i]["A"] = 0;
                    }
                    if (dicMenuList[ID].Contains("E"))
                    {
                        dt.Rows[i]["E"] = 1;
                    }
                    else
                    {
                        dt.Rows[i]["E"] = 0;
                    }
                }
                else
                {
                    dt.Rows[i]["R"] = 0;
                    dt.Rows[i]["E"] = 0;
                }
            }

            return dt;
        }

        void getMenuUserAll(HttpContext context)
        {


            string PID = context.Request.QueryString["PID"].ToString();
            string UserID = context.Request.QueryString["UserID"].ToString();

            DataTable dt = GetMenuUserTable(Convert.ToInt32(PID), UserID);
            dt.Columns.Add("HasChild", typeof(int));//添加标识列，标识该菜单是否有子菜单
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strSql = " select count(ID) from Menu_User where PID=@PID";
                SqlParameter para = new SqlParameter("@PID", dt.Rows[i]["ID"].ToString());
                string strCount = SQLHelper.ExecuteScalar(strSql, para).ToString();
                if (strCount != "0")
                {
                    dt.Rows[i]["HasChild"] = 1;
                }
                else
                {
                    dt.Rows[i]["HasChild"] = 0;
                }
            }

            StringBuilder strContent = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string m_ID = dt.Rows[i]["ID"].ToString();
                string m_PID = dt.Rows[i]["PID"].ToString();
                string m_strValue = dt.Rows[i]["strValue"].ToString();
                string R = dt.Rows[i]["R"].ToString();
                string A = dt.Rows[i]["A"].ToString();
                string E = dt.Rows[i]["E"].ToString();
                strContent.Append("<table class='tabEdit tabP' id='tabMenu" + m_ID + "' ><tr>");


                if (dt.Rows[i]["HasChild"].ToString() != "0")
                {
                    strContent.Append("<td style='width:40px;'><img id='img_" + m_ID + "' alt='Open' src='../../images/menuOpen.png' style='width:20px;height:20px;' onclick='OpenChildMenu(" + m_ID + ",\"img_" + m_ID + "\",\"tabMenu" + m_ID + "\",0)' /></td>");

                }
                else
                {
                    strContent.Append("<td style='width:40px;'></td>");

                }
                strContent.Append("<td><span >" + m_strValue + "</span></td>");

                if (R == "1")
                {
                    //strContent.Append("<td style='width:80px;'>查看：<input  type='checkbox' checked='checked' name='chkR_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='R-" + m_ID + "' name='chkR_" + m_ID + "'/><label for='R-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>查看</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>查看：<input  type='checkbox'  name='chkR_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox'  id='R-" + m_ID + "' name='chkR_" + m_ID + "'/><label for='R-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>查看</span></td>");
                }
                if (A == "1")
                {
                   // strContent.Append("<td style='width:80px;'>新增：<input  type='checkbox' checked='checked' name='chkA_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='A-" + m_ID + "' name='chkA_" + m_ID + "'/><label for='A-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>新增</span></td>");
                }
                else
                {
                   // strContent.Append("<td style='width:80px;'>新增：<input  type='checkbox' name='chkA_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox'  id='A-" + m_ID + "' name='chkA_" + m_ID + "'/><label for='A-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>新增</span></td>");

                }
                if (E == "1")
                {
                   // strContent.Append("<td style='width:80px;'>编辑：<input  type='checkbox' checked='checked' name='chkE_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='E-" + m_ID + "' name='chkE_" + m_ID + "'/><label for='E-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>编辑</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>编辑：<input  type='checkbox' name='chkE_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox'  id='E-" + m_ID + "' name='chkE_" + m_ID + "'/><label for='E-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>编辑</span></td>");
                }
              
                strContent.Append("</tr></table>");
                if (dt.Rows[i]["HasChild"].ToString() != "0")
                {
                 
                  strContent.Append(  getMenuUserChild(m_ID,"0",UserID));
                }
               
            }

            // context.Response.Write(JsonHelper.ToJson(dt));
            context.Response.Write(strContent.ToString());
        }

        string getMenuUserChild(string PID,string numLevel,string UserID)
        {
            
            DataTable dt = GetMenuUserTable(Convert.ToInt32(PID), UserID);
            dt.Columns.Add("HasChild", typeof(int));//添加标识列，标识该菜单是否有子菜单
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strSql = " select count(ID) from Menu_User where PID=@PID";
                SqlParameter para = new SqlParameter("@PID", dt.Rows[i]["ID"].ToString());
                string strCount = SQLHelper.ExecuteScalar(strSql, para).ToString();
                if (strCount != "0")
                {
                    dt.Rows[i]["HasChild"] = 1;
                }
                else
                {
                    dt.Rows[i]["HasChild"] = 0;
                }
            }

            StringBuilder strContent = new StringBuilder();
            strContent.Append("<div id='div_" + PID + "' style='display:none;'>");
          
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string m_ID = dt.Rows[i]["ID"].ToString();
                string m_PID = dt.Rows[i]["PID"].ToString();
                string m_strValue = dt.Rows[i]["strValue"].ToString();
                string R = dt.Rows[i]["R"].ToString();
                string A= dt.Rows[i]["A"].ToString();
                string E = dt.Rows[i]["E"].ToString();
                if (numLevel == "0")
                {
                    strContent.Append("<table class='tabEdit tabC'  id='tabMenu" + m_ID + "' ><tr>");
                }
                else
                {
                    strContent.Append("<table class='tabEdit tabC'   id='tabMenu" + m_ID + "' ><tr>");
                }


                if (dt.Rows[i]["HasChild"].ToString() != "0")
                {
                    strContent.Append("<td style='width:40px;'><img id='img_" + m_ID + "' alt='Open' src='../../images/menuOpen.png' style='width:20px;height:20px;' onclick='OpenChildMenu(" + m_ID + ",\"img_" + m_ID + "\",\"tabMenu" + m_ID + "\",0)' /></td>");

                }
                else
                {
                    strContent.Append("<td style='width:40px;'></td>");

                }
                strContent.Append("<td><span>" + m_strValue + "</span></td>");

                if (R == "1")
                {
                   // strContent.Append("<td style='width:80px;'>查看：<input  type='checkbox' checked='checked' name='chkR_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='R-" + m_ID + "' name='chkR_" + m_ID + "'/><label for='R-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>查看</span></td>");
                }
                else
                {
                   // strContent.Append("<td style='width:80px;'>查看：<input  type='checkbox'  name='chkR_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox'  id='R-" + m_ID + "' name='chkR_" + m_ID + "'/><label for='R-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>查看</span></td>");
                }

                if (A == "1")
                {
                    //strContent.Append("<td style='width:80px;'>新增：<input  type='checkbox' checked='checked' name='chkA_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='A-" + m_ID + "' name='chkA_" + m_ID + "'/><label for='A-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>新增</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>新增：<input  type='checkbox' name='chkA_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox'  id='A-" + m_ID + "' name='chkA_" + m_ID + "'/><label for='A-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>新增</span></td>");

                }
                if (E == "1")
                {
                    //strContent.Append("<td style='width:80px;'>编辑：<input  type='checkbox' checked='checked' name='chkE_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='E-" + m_ID + "' name='chkE_" + m_ID + "'/><label for='E-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>编辑</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>编辑：<input  type='checkbox' name='chkE_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' id='E-" + m_ID + "' name='chkE_" + m_ID + "'/><label for='E-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>编辑</span></td>");
                }
                strContent.Append("</tr></table>");
                if (dt.Rows[i]["HasChild"].ToString() != "0")
                {
                 
                  strContent.Append(  getMenuUserChild(m_ID,"1",UserID));
                }
               
            }
            strContent.Append("</div>");
            return strContent.ToString();
        }

        void getMenuAdminAll(HttpContext context)
        {
            string PID = context.Request.QueryString["PID"].ToString();
            string UserID = context.Request.QueryString["UserID"].ToString();

            DataTable dt = GetMenuAdminTable(Convert.ToInt32(PID), UserID);
            dt.Columns.Add("HasChild", typeof(int));//添加标识列，标识该菜单是否有子菜单
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strSql = " select count(ID) from Menu_Admin where PID=@PID";
                SqlParameter para = new SqlParameter("@PID", dt.Rows[i]["ID"].ToString());
                string strCount = SQLHelper.ExecuteScalar(strSql, para).ToString();
                if (strCount != "0")
                {
                    dt.Rows[i]["HasChild"] = 1;
                }
                else
                {
                    dt.Rows[i]["HasChild"] = 0;
                }
            }

           
            StringBuilder strContent = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string m_ID = dt.Rows[i]["ID"].ToString();
                string m_PID = dt.Rows[i]["PID"].ToString();
                string m_strValue = dt.Rows[i]["strValue"].ToString();
                string R = dt.Rows[i]["R"].ToString();
                string A = dt.Rows[i]["A"].ToString();
                string E = dt.Rows[i]["E"].ToString();
                strContent.Append("<table class='tabEdit tabP' id='tabMenu" + m_ID + "' ><tr>");


                if (dt.Rows[i]["HasChild"].ToString() != "0")
                {
                    strContent.Append("<td style='width:40px;'><img id='img_" + m_ID + "' alt='Open' src='../../images/menuOpen.png' style='width:20px;height:20px;' onclick='OpenChildMenu(" + m_ID + ",\"img_" + m_ID + "\",\"tabMenu" + m_ID + "\",0)' /></td>");

                }
                else
                {
                    strContent.Append("<td style='width:40px;'></td>");

                }
                strContent.Append("<td><span>" + m_strValue + "</span></td>");

                if (R == "1")
                {
                    //strContent.Append("<td style='width:80px;'>查看：<input  type='checkbox' checked='checked' name='chkR_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='R-" + m_ID + "' name='chkR_" + m_ID + "'/><label for='R-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>查看</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>查看：<input  type='checkbox'  name='chkR_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' id='R-" + m_ID + "' name='chkR_" + m_ID + "'/><label for='R-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>查看</span></td>");
                }

                if (A == "1")
                {
                    //strContent.Append("<td style='width:80px;'>新增：<input  type='checkbox' checked='checked' name='chkA_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='A-" + m_ID + "' name='chkA_" + m_ID + "'/><label for='A-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>新增</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>新增：<input  type='checkbox'  name='chkA_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox'  id='A-" + m_ID + "' name='chkA_" + m_ID + "'/><label for='A-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>新增</span></td>");
                }

                if (E == "1")
                {
                    //strContent.Append("<td style='width:80px;'>编辑：<input  type='checkbox' checked='checked' name='chkE_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='E-" + m_ID + "' name='chkE_" + m_ID + "'/><label for='E-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>编辑</span></td>");
                }
                else
                {
                   // strContent.Append("<td style='width:80px;'>编辑：<input  type='checkbox' name='chkE_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox'  id='E-" + m_ID + "' name='chkE_" + m_ID + "'/><label for='E-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>编辑</span></td>");

                }
                strContent.Append("</tr></table>");
                if (dt.Rows[i]["HasChild"].ToString() != "0")
                {

                    strContent.Append(getMenuAdminChild(m_ID, "0", UserID));
                }

            }

            // context.Response.Write(JsonHelper.ToJson(dt));
            context.Response.Write(strContent.ToString());
        }

        string getMenuAdminChild(string PID, string numLevel, string UserID)
        {

            DataTable dt = GetMenuAdminTable(Convert.ToInt32(PID), UserID);
            dt.Columns.Add("HasChild", typeof(int));//添加标识列，标识该菜单是否有子菜单
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strSql = " select count(ID) from Menu_Admin where PID=@PID";
                SqlParameter para = new SqlParameter("@PID", dt.Rows[i]["ID"].ToString());
                string strCount = SQLHelper.ExecuteScalar(strSql, para).ToString();
                if (strCount != "0")
                {
                    dt.Rows[i]["HasChild"] = 1;
                }
                else
                {
                    dt.Rows[i]["HasChild"] = 0;
                }
            }


            StringBuilder strContent = new StringBuilder();
            strContent.Append("<div id='div_" + PID + "' style='display:none;'>");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string m_ID = dt.Rows[i]["ID"].ToString();
                string m_PID = dt.Rows[i]["PID"].ToString();
                string m_strValue = dt.Rows[i]["strValue"].ToString();
                string R = dt.Rows[i]["R"].ToString();
                string A = dt.Rows[i]["A"].ToString();
                string E = dt.Rows[i]["E"].ToString();
                if (numLevel == "0")
                {
                    strContent.Append("<table class='tabEdit tabC'  id='tabMenu" + m_ID + "'><tr>");

                }
                else
                {
                    strContent.Append("<table class='tabEdit tabC'  id='tabMenu" + m_ID + "' ><tr>");
                }


                if (dt.Rows[i]["HasChild"].ToString() != "0")
                {
                    strContent.Append("<td style='width:40px;'><img id='img_" + m_ID + "' alt='Open' src='../../images/menuOpen.png' style='width:20px;height:20px;' onclick='OpenChildMenu(" + m_ID + ",\"img_" + m_ID + "\",\"tabMenu" + m_ID + "\",0)' /></td>");

                }
                else
                {
                    strContent.Append("<td style='width:40px;'></td>");

                }
                strContent.Append("<td><span>" + m_strValue + "</span></td>");

                if (R == "1")
                {
                   // strContent.Append("<td style='width:80px;'>查看：<input  type='checkbox' checked='checked' name='chkR_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='R-" + m_ID + "' name='chkR_" + m_ID + "'/><label for='R-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>查看</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>查看：<input  type='checkbox'  name='chkR_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' id='R-" + m_ID + "' name='chkR_" + m_ID + "'/><label for='R-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>查看</span></td>");
                }
                if (A == "1")
                {
                    //strContent.Append("<td style='width:80px;'>新增：<input  type='checkbox' checked='checked' name='chkA_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='A-" + m_ID + "' name='chkA_" + m_ID + "'/><label for='A-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>新增</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>新增：<input  type='checkbox'  name='chkA_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' id='A-" + m_ID + "' name='chkA_" + m_ID + "'/><label for='A-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>新增</span></td>");
                }
                if (E == "1")
                {
                   // strContent.Append("<td style='width:80px;'>编辑：<input  type='checkbox' checked='checked' name='chkE_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox' checked='checked' id='E-" + m_ID + "' name='chkE_" + m_ID + "'/><label for='E-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>编辑</span></td>");
                }
                else
                {
                    //strContent.Append("<td style='width:80px;'>编辑：<input  type='checkbox' name='chkE_" + m_ID + "'/></td>");
                    strContent.Append("<td style='width:80px;'><input  type='checkbox' class='custom-checkbox'  id='E-" + m_ID + "' name='chkE_" + m_ID + "'/><label for='E-" + m_ID + "'></label><span style='margin:0px 0px 0px 5px'>编辑</span></td>");

                }
                strContent.Append("</tr></table>");
                if (dt.Rows[i]["HasChild"].ToString() != "0")
                {

                    strContent.Append(getMenuAdminChild(m_ID, "1", UserID));
                }

            }
            strContent.Append("</div>");
            return strContent.ToString();
        }



        //解除营业员24小时不能登录的限制
        void ClearError(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            int i = SQLHelper.ExecuteNonQuery(" UPDATE dbo.UserOperate SET ErrorTime=0 WHERE ID="+ID);
            if (i!= 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetWB(HttpContext context)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(" SELECT ID,strName FROM WB");
            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetUserGroup(HttpContext context)
        {
            string WBID = context.Request.QueryString["WBID"].ToString();
            string strSql;
            object obj = SQLHelper.ExecuteScalar(" SELECT ISHQ  FROM dbo.WB  WHERE ID=" + WBID);
            if (Convert.ToBoolean(obj) == true || obj.ToString() == "1")
            {
                strSql = " SELECT ID,strName FROM dbo.UserGroup WHERE strName!='系统管理员' and strName!='营业员'";
            }
            else
            {
                strSql = " SELECT ID,strName FROM dbo.UserGroup WHERE strName='营业员'";

            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);
            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void AddUser(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string SerialNumber = "000000";

            SerialNumber = SQLHelper.ExecuteScalar("SELECT TOP 1 SerialNumber FROM dbo.Users ORDER BY dt_Add DESC").ToString();
            int numSerial = Convert.ToInt32(SerialNumber) + 1;
            if (numSerial > 0 && numSerial < 10)
            {
                SerialNumber = "0000" + numSerial.ToString(); ;
            }
            else if (numSerial > 10 && numSerial < 100)
            {
                SerialNumber = "000" + numSerial.ToString(); ;
            }
            else if (numSerial >= 100 && numSerial < 1000)
            {
                SerialNumber = "00" + numSerial.ToString(); ;
            }
            else if (numSerial >= 1000 && numSerial < 10000)
            {
                SerialNumber = "0" + numSerial.ToString(); ;
            }
            else { SerialNumber = numSerial.ToString(); }

            string UserGroup_ID = context.Request.Form["UserGroup_ID"].ToString();
            string WB_ID = context.Request.Form["WB_ID"].ToString();
            string strRealName = context.Request.Form["strRealName"].ToString();
            string strLoginName = context.Request.Form["strLoginName"].ToString();


            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Users WHERE strLoginName='" + strLoginName + "'").ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }
            string strPassword = context.Request.Form["strPassword"].ToString();
            strPassword = Fun.GetMD5_32(strPassword);// 获取md5加密后的用户密码
            string strPhone = context.Request.Form["strPhone"].ToString();
            string strAddress = context.Request.Form["strAddress"].ToString();
            string numLimitAmount = context.Request.Form["numLimitAmount"].ToString();
            string numLimitAmount_sell = context.Request.Form["numLimitAmount_sell"].ToString();
            string numLimitAmount_shopping = context.Request.Form["numLimitAmount_shopping"].ToString();
            string numPrint = context.Request.Form["numPrint"].ToString();
            bool ISEnable = true;
            if (context.Request.Form["ISEnable"].ToString() == "0") {
                ISEnable = false;
            }
             
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Users] (");
            strSql.Append("SerialNumber,UserGroup_ID,WB_ID,strRealName,strLoginName,strPassword,strPhone,strAddress,numLimitAmount,numLimitAmount_sell,numLimitAmount_shopping,numPrint,ISEnable,dt_Add,dt_Update)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@UserGroup_ID,@WB_ID,@strRealName,@strLoginName,@strPassword,@strPhone,@strAddress,@numLimitAmount,@numLimitAmount_sell,@numLimitAmount_shopping,@numPrint,@ISEnable,@dt_Add,@dt_Update)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@UserGroup_ID", SqlDbType.Int,4),
					new SqlParameter("@WB_ID", SqlDbType.Int,4),
					new SqlParameter("@strRealName", SqlDbType.NVarChar,50),
					new SqlParameter("@strLoginName", SqlDbType.NVarChar,50),
					new SqlParameter("@strPassword", SqlDbType.NVarChar,50),
					new SqlParameter("@strPhone", SqlDbType.NVarChar,50),
					new SqlParameter("@strAddress", SqlDbType.NVarChar,50),
					new SqlParameter("@numLimitAmount", SqlDbType.Int,4),
                    new SqlParameter("@numLimitAmount_sell", SqlDbType.Int,4),
                    new SqlParameter("@numLimitAmount_shopping", SqlDbType.Int,4),
                     new SqlParameter("@numPrint", SqlDbType.Int,4),
					new SqlParameter("@ISEnable", SqlDbType.Bit,1),
					new SqlParameter("@dt_Add", SqlDbType.DateTime),
					new SqlParameter("@dt_Update", SqlDbType.DateTime)};
            parameters[0].Value = SerialNumber;
            parameters[1].Value = UserGroup_ID;
            parameters[2].Value = WB_ID;
            parameters[3].Value = strRealName;
            parameters[4].Value = strLoginName;
            parameters[5].Value = strPassword;
            parameters[6].Value = strPhone;
            parameters[7].Value = strAddress;
            parameters[8].Value = numLimitAmount;
            parameters[9].Value = numLimitAmount_sell;
            parameters[10].Value = numLimitAmount_shopping;
            parameters[11].Value = numPrint;
            parameters[12].Value = ISEnable;
            parameters[13].Value = DateTime.Now;
            parameters[14].Value = DateTime.Now;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

     

        void UpdateUser(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string UserGroup_ID = context.Request.QueryString["UserGroup_ID"].ToString();
            string strRealName = context.Request.Form["strRealName"].ToString();
            string strLoginName = context.Request.Form["strLoginName"].ToString();
            string sql = string.Format(" SELECT COUNT(ID) FROM dbo.Users WHERE strLoginName='{0}' and UserGroup_ID={1} and ID !={2}",strLoginName,UserGroup_ID,ID);
            if (SQLHelper.ExecuteScalar(sql).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }

            string strPassword = context.Request.Form["strPassword"].ToString();
           string  strPasswordMD5 = Fun.GetMD5_32(strPassword);// 获取md5加密后的用户密码
            string strPhone = context.Request.Form["strPhone"].ToString();
            string strAddress = context.Request.Form["strAddress"].ToString();
            string numLimitAmount = context.Request.Form["numLimitAmount"].ToString();
            string numLimitAmount_sell = context.Request.Form["numLimitAmount_sell"].ToString();
            string numLimitAmount_shopping = context.Request.Form["numLimitAmount_shopping"].ToString();
            string numPrint = context.Request.Form["numPrint"].ToString();
            string ISEnable = context.Request.Form["ISEnable"].ToString();
           
            StringBuilder strSql=new StringBuilder();
			strSql.Append("update [Users] set ");
            //if (UserGroup_ID != "单位管理员")
            //{
            //    strSql.Append("UserGroup_ID=" + UserGroup_ID + ",");
            //}
			strSql.Append("strRealName='"+strRealName+"',");
			strSql.Append("strLoginName='"+strLoginName+"',");
			strSql.Append("strPhone='"+strPhone+"',");
			strSql.Append("strAddress='"+strAddress+"',");
			strSql.Append("numLimitAmount="+numLimitAmount+",");
            strSql.Append("numLimitAmount_sell=" + numLimitAmount_sell + ",");
            strSql.Append("numLimitAmount_shopping=" + numLimitAmount_shopping + ",");
            strSql.Append("numPrint=" + numPrint + ",");
			strSql.Append("ISEnable="+ISEnable+",");
			strSql.Append("dt_Update='"+DateTime.Now.ToString()+"'");
            if (strPassword.Trim() != "")
            {
                strSql.Append(",strPassword='" + strPasswordMD5 + "'");
            }
			strSql.Append(" where ID="+ID);
			

                if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
                {
                    context.Response.Write("OK");
                }
                else
                {
                    context.Response.Write("Error");
                }
        }

        void GetUserByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,SerialNumber,UserGroup_ID,WB_ID,strRealName,strLoginName,strPassword,strPhone,strAddress,numLimitAmount,numLimitAmount_sell,numLimitAmount_shopping,numPrint,numPrint_exchange,numPrint_sell,numPrint_shopping,ISEnable,dt_Add,dt_Update ");
            strSql.Append(" FROM [Users] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.BigInt)};
            parameters[0].Value = wbid;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(),parameters);
            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void DeleteUserByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            string UserGroup_ID = context.Request.QueryString["UserGroup_ID"].ToString();
            
            //是否被使用
            int numDep = Convert.ToInt32(SQLHelper.ExecuteScalar(" SELECT COUNT(ID) FROM Dep_StorageInfo WHERE UserID="+ID));
            if (numDep > 0) {
                var res = new { state = "error", msg = "该营业员已有操作记录，不可以删除!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return ;
            }
            int numCom = Convert.ToInt32(SQLHelper.ExecuteScalar(" SELECT COUNT(ID) FROM C_OperateLog WHERE UserID=" + ID));
            if (numCom > 0)
            {
                var res = new { state = "error", msg = "该营业员已有操作记录，不可以删除!" };
                context.Response.Write(JsonHelper.ToJson(res));
                return;
            }



            string strSql = "";

            if (UserGroup_ID == "单位管理员")
            {
                int usercount = Convert.ToInt32(SQLHelper.ExecuteScalar(" SELECT COUNT(ID) FROM dbo.Users WHERE UserGroup_ID=2"));
                if (usercount <= 1)
                {
                    var res = new { state = "error", msg = "系统中至少要有一个单位管理员!" };
                    context.Response.Write(JsonHelper.ToJson(res));
                    return;
                }
                else {
                    strSql = " delete FROM dbo.Users WHERE ID=" + ID;
                }
            }
            else {
                strSql = " delete FROM dbo.Users WHERE ID=" + ID;
            }


            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
               var res = new { state = "success", msg = "操作成功!" };
                context.Response.Write(JsonHelper.ToJson(res));
               
            }
            else
            {
               var res = new { state = "error", msg = "数据操作失败!" };
                context.Response.Write(JsonHelper.ToJson(res));
               
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