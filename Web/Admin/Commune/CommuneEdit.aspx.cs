using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;

namespace Web.Admin.Commune
{
    public partial class CommuneEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               // txtAccountNumber.Value = GetNewAccountNumber();
            }
        }

        //上传社员申请表 
        protected void btnUploadApplication_Click(object sender, EventArgs e)
        {
            if (FileApplication.HasFile)//存在上传文件
            {
                if (FileApplication.PostedFile.ContentLength >10* 1024 * 1024)//判断图片的大小
                {
                    string StrScript;
                    StrScript = ("<script language=javascript>");
                    StrScript += ("alert('申请表图片最大为10M!');");
                    StrScript += ("</script>");
                    System.Web.HttpContext.Current.Response.Write(StrScript);
                    return;
                }

                string fileName = FileApplication.PostedFile.FileName;

                string fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);//文件类型
                //判断图片的类型
                if (fileType.ToLower() != "png" && fileType.ToLower() != "jpg" && fileType.ToLower() != "jpeg" && fileType.ToLower() != "bmp")
                {
                    string StrScript;
                    StrScript = ("<script language=javascript>");
                    StrScript += ("alert('上传图片的类型只可以为\\.jpg\\.jpeg\\.png\\.bmp!');");
                    StrScript += ("</script>");
                    System.Web.HttpContext.Current.Response.Write(StrScript);
                    return;
                }

                //查看并创建存放图片的文件夹
                string WBID = Session["WB_ID"].ToString();
                string serverPath = "~/Files/ApplicationImg/" + WBID + "/";
                string serverDirectory = Server.MapPath(serverPath);
                if (!System.IO.Directory.Exists(serverDirectory))
                {
                    //创建目录
                    System.IO.Directory.CreateDirectory(serverDirectory);
                }
                string newfilename = txtAccountNumber.Value + "." + fileType;//重新定义文件名
                FileApplication.SaveAs(Server.MapPath(serverPath + newfilename));
                txtApplicationFileName.Value = serverPath + newfilename;
               // spanApplication.InnerHtml = "已上传申请表：" + fileName;
                imgApplication.Src = serverPath + newfilename;
            }
        }

        protected void btnUploadCommunePic_Click(object sender, EventArgs e)
        {
            if (FileCommunePic.HasFile)//存在上传文件
            {
                if (FileCommunePic.PostedFile.ContentLength > 5*1024 * 1024)//判断图片的大小
                {
                    string StrScript;
                    StrScript = ("<script language=javascript>");
                    StrScript += ("alert('社员头像图片最大为5M!');");
                    StrScript += ("</script>");
                    System.Web.HttpContext.Current.Response.Write(StrScript);
                    return;
                }

                string fileName = FileCommunePic.PostedFile.FileName;

                string fileType = fileName.Substring(fileName.LastIndexOf(".") + 1);//文件类型
                //判断图片的类型
                if (fileType.ToLower() != "png" && fileType.ToLower() != "jpg" && fileType.ToLower() != "jpeg" && fileType.ToLower() != "bmp")
                {
                    string StrScript;
                    StrScript = ("<script language=javascript>");
                    StrScript += ("alert('上传图片的类型只可以为\\.jpg\\.jpeg\\.png\\.bmp!');");
                    StrScript += ("</script>");
                    System.Web.HttpContext.Current.Response.Write(StrScript);
                    return;
                }

                //查看并创建存放图片的文件夹
                string WBID = Session["WB_ID"].ToString();
                string serverPath = "~/Files/CommuneImg/" + WBID + "/";
                string serverDirectory = Server.MapPath(serverPath);
                if (!System.IO.Directory.Exists(serverDirectory))
                {
                    //创建目录
                    System.IO.Directory.CreateDirectory(serverDirectory);
                }
                string newfilename = txtAccountNumber.Value + "." + fileType;//重新定义文件名
                FileCommunePic.SaveAs(Server.MapPath(serverPath + newfilename));
                txtCommuneFileName.Value = serverPath + newfilename;
                //spanCommunePic.InnerHtml = "已上传用户头像:" + fileName;
                imgCommune.Src = serverPath + newfilename;
            }
        }



      
    }
}