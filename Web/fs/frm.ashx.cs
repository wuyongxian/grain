using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.SessionState;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
namespace Web.fs
{
    /// <summary>
    /// frm 的摘要说明
    /// </summary>
    public class frm : IHttpHandler, IRequiresSessionState
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
                    case "BackUpDB": BackUpDB(context); break;//备份数据库
                    case "BackUpDBToClient": BackUpDBToClient(context); break;//备份数据库到客户端
                        
                    case "getUserInfo": getUserInfo(context); break;
                    case "GetMenu": GetMenu(context); break;
                    case "GetNotice": GetNotice(context); break;
                    case "getMenuHomePage": getMenuHomePage(context); break;
                  
                }
            }

        }

        /// <summary>
        /// 由当前登陆用户的ID获取其所在的网点信息
        /// </summary>
        /// <param name="context"></param>
        void BackUpDB(HttpContext context)
        {
            string filePath = "d:\\BackUpDB\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day;
            if (!Directory.Exists("filePath"))
            {
                Directory.CreateDirectory(filePath);
            }

            //获取当前数据库的数据库名称
            string dbname = "GrainBankDB";
            string connectionString = ConfigurationManager.ConnectionStrings["sqlconn"].ToString();
            int index1 = connectionString.IndexOf("Initial Catalog=");
            if (index1 != -1)
            {
                string connectionString_part = connectionString.Substring(index1 + 16);
                dbname = connectionString_part.Substring(0, connectionString_part.IndexOf(';'));
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("  BACKUP DATABASE "+dbname);
            strSql.Append("  TO disk = '" + filePath + "\\"+dbname+".bak'");

            try
            {
                if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
                {
                    //if (File.Exists(filePath + "\\GrainBankDB.bak"))
                    //{
                    //    File.SetAttributes(filePath + "\\GrainBankDB.bak", FileAttributes.ReadOnly);//设置文件为只读文件
                    //}
                    context.Response.Write("OK");
                }
                else
                {
                    context.Response.Write("Error");
                }
            }
            catch {
                context.Response.Write("Error");
            }
        }


        ///// <summary>
        ///// 由当前登陆用户的ID获取其所在的网点信息
        ///// </summary>
        ///// <param name="context"></param>
        //void BackUpDB(HttpContext context)
        //{
        //    string strfilePath = "BackUpDB\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month+"\\"+DateTime.Now.Day;
        //    string filePath = context.Server.MapPath("~/"+strfilePath);
        //    string fullfilePath = context.Server.MapPath("~/" + strfilePath + "\\GrainBankDB.bak");
        //    if (!Directory.Exists(filePath))
        //    {
        //        Directory.CreateDirectory(filePath);
        //    }

        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("  BACKUP DATABASE GrainBankDB");
        //    strSql.Append("  TO disk = '" + fullfilePath + "'");
          
        //    if (SQLHelper.ExecuteNonQuery(strSql.ToString())!= 0)
        //    {
        //        //if (File.Exists(fullfilePath))
        //        //{
        //        //    File.SetAttributes(fullfilePath, FileAttributes.ReadOnly);//设置文件为只读文件
        //        //}
        //        context.Response.Write(fullfilePath);
        //    }
        //    else
        //    {
        //        context.Response.Write("Error");
        //    }
        //}

        void BackUpDBToClient(HttpContext context)
        {
            string strfilePath = "BackUpDB\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day;
            string filePath = context.Server.MapPath("~/" + strfilePath);
            string fullfilePath = context.Server.MapPath("~/" + strfilePath + "\\GrainBankDB.bak");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
           

            StringBuilder strSql = new StringBuilder();
            strSql.Append("  BACKUP DATABASE GrainBankDB");
            strSql.Append("  TO disk = '" + fullfilePath + "'");

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                //if (File.Exists(fullfilePath))
                //{
                //    File.SetAttributes(fullfilePath, FileAttributes.ReadOnly);//设置文件为只读文件
                //}

                //将文件下载到客户端
               //context.Response.ContentType = "application/x-zip-compressed";
               //context.Response.AddHeader("Content-Disposition", "attachment;filename=GrainBankDB.bak");
               //string filename = fullfilePath;
               // context.Response.TransmitFile(filename);

               
                FileInfo fileInfo = new FileInfo(fullfilePath);
                context.Response.Clear();
                context.Response.ClearContent();
                context.Response.ClearHeaders();
                context.Response.AddHeader("Content-Disposition", "attachment;filename=GrainBankDB.bak");
                context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                context.Response.WriteFile(fileInfo.FullName);
                context.Response.Flush();
                context.Response.End();
            }
            else
            {
               
            }
        }

        void getUserInfo(HttpContext context)
        {

            string ID = context.Session["ID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT A.ID, A.strLoginName,A.strRealName,B.strName AS WBID");
            strSql.Append("  FROM dbo.Users A INNER JOIN dbo.WB B ON A.WB_ID=B.ID");
            strSql.Append("  WHERE A.ID="+ID);

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (SQLHelper.ExecuteNonQuery(strSql.ToString())!= 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetMenu(HttpContext context)
        {
           // bool ISHQ = Convert.ToBoolean(context.Request.QueryString["ISHQ"]);
            bool ISHQ = Convert.ToBoolean(context.Session["ISHQ"]);
            if (ISHQ)
            {
                context.Response.Write(GetMenu_Admin(context));
              
            }
            else
            {
                context.Response.Write(GetMenu_User(context));
            }
        }

        void GetNotice(HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT top 1 strContent FROM dbo.InfoNotice");

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
            if (obj == null)
            {
                context.Response.Write("");
            }
            else
            {
                context.Response.Write(obj.ToString()); 
            }
        }

        string GetMenu_Admin(HttpContext context)
        {

            StringBuilder str = new StringBuilder();
            DataTable dt = GetMenuTable_Admin(0,context);//获取根节点
            if (dt == null || dt.Rows.Count == 0)
            {
                return "";
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[i]["ISShow"]) == 0)
                {
                    continue;//如果当前用户没有使用菜单的权限，则隐藏这一条菜单
                }
                str.Append(" <p class='menu_head'> " + dt.Rows[i]["strValue"].ToString() + "</p>");

                DataTable dt_child = GetMenuTable_Admin(Convert.ToInt32(dt.Rows[i]["ID"]),context);//获取子节点
                if (dt_child != null && dt_child.Rows.Count != 0)
                {
                    str.Append("  <div class='menu_body'>");
                    for (int j = 0; j < dt_child.Rows.Count; j++)
                    {
                        if (Convert.ToInt32(dt_child.Rows[j]["ISShow"]) == 0)
                        {
                            continue;//如果当前用户没有使用菜单的权限，则隐藏这一条菜单
                        }
                        DataTable dt_child_2 = GetMenuTable_Admin(Convert.ToInt32(dt_child.Rows[j]["ID"]),context);//后续子节点
                        if (dt_child_2 != null && dt_child_2.Rows.Count != 0)
                        {

                            str.Append(" <p class='menu_head_child'> " + dt_child.Rows[j]["strValue"].ToString() + "</p>");
                            str.Append("  <div class='menu_body_child'>");
                            for (int k = 0; k < dt_child_2.Rows.Count; k++)
                            {
                                if (Convert.ToInt32(dt_child_2.Rows[k]["ISShow"]) == 0)
                                {
                                    continue;//如果当前用户没有使用菜单的权限，则隐藏这一条菜单
                                }
                                string strValue = dt_child_2.Rows[k]["strValue"].ToString();
                                string strUrl = dt_child_2.Rows[k]["strUrl"].ToString();
                                string MenuID = dt_child_2.Rows[k]["ID"].ToString();
                                strUrl += "?MenuID=" + MenuID;
                                str.Append("<div class='menu_body_child_link'><a href='" + strUrl + "' target='main'>" + strValue + "</a></div>");
                            }
                            str.Append(" </div>");
                        }
                        else
                        {
                            string strValue = dt_child.Rows[j]["strValue"].ToString();
                            string strUrl = dt_child.Rows[j]["strUrl"].ToString();
                            string MenuID = dt_child.Rows[j]["ID"].ToString();
                            strUrl += "?MenuID=" + MenuID;
                            str.Append(" <div class='menu_body_link'> <a href='" + strUrl + "'  target='main'>" + strValue + "</a></div>");
                            // str.Append(" <a  href='<%=ResolveClientUrl("+strUrl+") %>'>" + strValue + "</a>");

                        }
                    }
                    str.Append(" </div>");
                }

            }
            return str.ToString();
        }

        string GetMenu_User(HttpContext context)
        {
            StringBuilder str = new StringBuilder();
            DataTable dt = GetMenuTable_User(0, context);//获取根节点
            if (dt == null || dt.Rows.Count == 0)
            {
                return "";
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[i]["ISShow"]) == 0)
                {
                    continue;//如果当前用户没有使用菜单的权限，则隐藏这一条菜单
                }
                str.Append(" <p class='menu_head'> " + dt.Rows[i]["strValue"].ToString() + "</p>");

                DataTable dt_child = GetMenuTable_User(Convert.ToInt32(dt.Rows[i]["ID"]), context);//获取子节点
                if (dt_child != null && dt_child.Rows.Count != 0)
                {
                    str.Append("  <div class='menu_body'>");
                    for (int j = 0; j < dt_child.Rows.Count; j++)
                    {
                        if (Convert.ToInt32(dt_child.Rows[j]["ISShow"]) == 0)
                        {
                            continue;//如果当前用户没有使用菜单的权限，则隐藏这一条菜单
                        }
                        DataTable dt_child_2 = GetMenuTable_User(Convert.ToInt32(dt_child.Rows[j]["ID"]), context);//后续子节点
                        if (dt_child_2 != null && dt_child_2.Rows.Count != 0)
                        {
                            str.Append(" <p class='menu_head_child'> " + dt_child.Rows[j]["strValue"].ToString() + "</p>");
                            str.Append("  <div class='menu_body_child'>");
                            for (int k = 0; k < dt_child_2.Rows.Count; k++)
                            {
                                if (Convert.ToInt32(dt_child_2.Rows[k]["ISShow"]) == 0)
                                {
                                    continue;//如果当前用户没有使用菜单的权限，则隐藏这一条菜单
                                }
                                string strValue = dt_child_2.Rows[k]["strValue"].ToString();
                                string strUrl = dt_child_2.Rows[k]["strUrl"].ToString();
                                string MenuID = dt_child_2.Rows[k]["ID"].ToString();
                                strUrl += "?MenuID=" + MenuID;
                                str.Append("<div class='menu_body_child_link'><a href='" + strUrl + "' target='main'>" + strValue + "</a></div>");
                            }
                            str.Append(" </div>");
                        }
                        else
                        {
                            string strValue = dt_child.Rows[j]["strValue"].ToString();
                            string strUrl = dt_child.Rows[j]["strUrl"].ToString();
                            string MenuID = dt_child.Rows[j]["ID"].ToString();
                            strUrl += "?MenuID=" + MenuID;
                            str.Append(" <div class='menu_body_link'> <a href='" + strUrl + "'  target='main'>" + strValue + "</a></div>");


                        }
                    }
                    str.Append(" </div>");
                }

            }
            return str.ToString();
        }

        DataTable GetMenuTable_Admin(int PID, HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT ID,PID,strValue,strUrl,numSort,ISEnable FROM Menu_Admin where PID=@PID");
            strSql.Append(" and ISEnable=1");
            //if (Session["UserGroup_ID"] != null && Session["UserGroup_ID"].ToString() == "3")
            //{
            //    strSql.Append(" and ISSysW=1");
            //}
            strSql.Append(" order by numSort");
            SqlParameter para = new SqlParameter("@PID", PID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(), para);

            if (dt == null || dt.Rows.Count == 0)//Menu_User表没有菜单
            {
                return null;
            }
            dt.Columns.Add("ISShow", typeof(int));//是否要对用户显示此菜单(1:显示，2：不显示)
            //查询当前用户的营业员的菜单
            string UserID = context.Session["ID"].ToString();
            string UserGroupName = SQLHelper.ExecuteScalar(" SELECT strName FROM dbo.UserGroup WHERE ID=( SELECT UserGroup_ID FROM dbo.Users WHERE ID=" + UserID + ")").ToString();
            object objmenuList = SQLHelper.ExecuteScalar(" SELECT menuList FROM UserMenu WHERE UserID=" + UserID + " AND numtype=1");
            if (UserGroupName != "单位管理员")
            {
                if (objmenuList == null || objmenuList.ToString() == "")//userMenu表没有该用户的菜单记录
                {
                    return null;
                }
            }
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

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (UserGroupName == "单位管理员")
                {
                    dt.Rows[i]["ISShow"] = 1;
                }
                else
                {
                    int ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                    if (dicMenuList.Keys.Contains(ID))
                    {
                        if (dicMenuList[ID].Contains("R"))
                        {
                            dt.Rows[i]["ISShow"] = 1;
                        }
                        else
                        {
                            dt.Rows[i]["ISShow"] = 0;
                        }

                    }
                    else
                    {
                        dt.Rows[i]["ISShow"] = 0;
                    }
                }
            }




            return dt;
        }


        DataTable GetMenuTable_User(int PID, HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("   SELECT ID,PID,strValue,strUrl,numSort,ISEnable");
            strSql.Append("  FROM Menu_User");
            strSql.Append("  where PID=@PID and ISEnable=1");
            strSql.Append("  order by numSort");

            SqlParameter para = new SqlParameter("@PID", PID);
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString(), para);
            if (dt == null || dt.Rows.Count == 0)//Menu_User表没有菜单
            {
                return null;
            }
            dt.Columns.Add("ISShow", typeof(int));//是否要对用户显示此菜单(1:显示，2：不显示)
            //查询当前用户的营业员的菜单
            string UserID = context.Session["ID"].ToString();
            object objmenuList = SQLHelper.ExecuteScalar(" SELECT menuList FROM UserMenu WHERE UserID=" + UserID + " AND numtype=2");
            if (objmenuList == null || objmenuList.ToString() == "")//userMenu表没有该用户的菜单记录
            {
                return null;
            }
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

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                int ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                if (dicMenuList.Keys.Contains(ID))
                {
                    if (dicMenuList[ID].Contains("R"))
                    {
                        dt.Rows[i]["ISShow"] = 1;
                    }
                    else
                    {
                        dt.Rows[i]["ISShow"] = 0;
                    }

                }
                else
                {
                    dt.Rows[i]["ISShow"] = 0;
                }
            }

            return dt;
        }


        void getMenuHomePage(HttpContext context)
        {

            string ID = context.Session["ID"].ToString();
            string menulist = SQLHelper.ExecuteScalar(" SELECT menuList FROM dbo.UserMenu WHERE UserID="+ID).ToString();
            DataTable dtmenu_user = SQLHelper.ExecuteDataTable(" SELECT * FROM dbo.Menu_User WHERE strValue IN ('新建账户','存粮','兑换商品','存转销') AND PID!=0");

            if(menulist==""||dtmenu_user==null||dtmenu_user.Rows.Count==0){
            var res=new {state="error",msg="查询用户菜单权限失败!"};
                   context.Response.Write(JsonHelper.ToJson(res));
                   return;
            }
            string[] menu_array = menulist.Split('|');
            for (int i = 0; i < menu_array.Length; i++) {
                menu_array[i] = menu_array[i].Substring(0,menu_array[i].IndexOf(':'));
            }
            bool is_kaihu = false;
            bool is_cunliang = false;
            bool is_duihuan = false;
            bool is_czx = false;
            int id_kaihu = 0;
            int id_cunliang = 0;
            int id_duihuan = 0;
            int id_czx = 0;
            for (int i = 0; i < dtmenu_user.Rows.Count; i++) {
                if (dtmenu_user.Rows[i]["strValue"].ToString() == "新建账户") {
                    id_kaihu = Convert.ToInt32(dtmenu_user.Rows[i]["ID"]);
                }
                if (dtmenu_user.Rows[i]["strValue"].ToString() == "存粮")
                {
                    id_cunliang = Convert.ToInt32(dtmenu_user.Rows[i]["ID"]);
                }
                if (dtmenu_user.Rows[i]["strValue"].ToString() == "兑换商品")
                {
                    id_duihuan = Convert.ToInt32(dtmenu_user.Rows[i]["ID"]);
                }
                if (dtmenu_user.Rows[i]["strValue"].ToString() == "存转销")
                {
                    id_czx = Convert.ToInt32(dtmenu_user.Rows[i]["ID"]);
                }
            }
            if (id_kaihu != 0)
            {
                if (menu_array.Contains(id_kaihu.ToString()))
                {
                    is_kaihu = true;
                }
            }
            if (id_cunliang != 0)
            {
                if (menu_array.Contains(id_cunliang.ToString()))
                {
                    is_cunliang = true;
                }
            }
            if (id_duihuan != 0)
            {
                if (menu_array.Contains(id_duihuan.ToString()))
                {
                    is_duihuan = true;
                }
            }
            if (id_czx != 0)
            {
                if (menu_array.Contains(id_czx.ToString()))
                {
                    is_czx = true;
                }
            }
            var res2 = new { state = "success", msg = "查询用户菜单权限成功!",is_kaihu=is_kaihu,is_cunliang=is_cunliang,is_duihuan=is_duihuan,is_czx=is_czx };
            context.Response.Write(JsonHelper.ToJson(res2));
            
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