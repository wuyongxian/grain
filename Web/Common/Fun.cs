using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Web
{
    /// <summary>
    /// 常用函数
    /// </summary>
    public class Fun
    {
         #region Alert
        /// <summary>
        /// 直接返回
        /// </summary>
        public static void Back()
        {
            HttpContext.Current.Response.Write("<script language='JavaScript'>");
            HttpContext.Current.Response.Write(" history.go(-1);");
            HttpContext.Current.Response.Write("</script>");

        }

        /// <summary>
        /// 提示后返回
        /// </summary>
        /// <param name="str"></param>
        public static void Back(string str)
        {
            HttpContext.Current.Response.Write("<script language='JavaScript'>");
            if (str != null)
            {
                HttpContext.Current.Response.Write(" alert('" + str + "');");
            }
            HttpContext.Current.Response.Write(" history.go(-1);");
            HttpContext.Current.Response.Write("</script>");

        }

        /// <summary>
        /// 提示
        /// </summary>
        /// <param name="str"></param>
        public static void Alert(string str)
        {
            HttpContext.Current.Response.Write("<script language='JavaScript'>");
            HttpContext.Current.Response.Write("alert('" + str + "');");
            HttpContext.Current.Response.Write("</script/>");
            
        }

        /// <summary>
        /// 提示后跳转到url页面
        /// </summary>
        /// <param name="str"></param>
        /// <param name="url"></param>
        public static void Alert(string str, string url)
        {
            HttpContext.Current.Response.Write("<script language='JavaScript'>");
            if (str != null)
            {
                HttpContext.Current.Response.Write(" alert('" + str + "');");
            }
            if (url != null)
            {
                HttpContext.Current.Response.Write(" location.href='" + url + "';");
            }
            HttpContext.Current.Response.Write("</script>");
        }

        /// <summary>
        /// 跳转到其他页面
        /// </summary>
        /// <param name="str"></param>
        /// <param name="url"></param>
        /// <param name="target"></param>
        public static void Alert(string str, string url, string target)
        {
            HttpContext.Current.Response.Write("<script language='JavaScript'>");
            if (str != null)
            {
                HttpContext.Current.Response.Write(" alert('" + str + "');");
            }
            if (url != null)
            {
                HttpContext.Current.Response.Write(" location.href='" + url + "';");
            }
            HttpContext.Current.Response.Write("</script>");
        }

        /// <summary>
        /// 提示后关闭
        /// </summary>
        /// <param name="str"></param>
        public static void AlertClose(string str)
        {
            HttpContext.Current.Response.Write("<script language='JavaScript'>");
            HttpContext.Current.Response.Write("alert('" + str + "');window.opener=null;window.close();");
            HttpContext.Current.Response.Write("</script/>");
        }
        #endregion 

         #region  数据过滤
        /// <summary>
        /// 过滤危险字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SafeData(string str)//　数据过滤
        {
            string tempStr = str;
            if (tempStr != null)
            {
                tempStr = tempStr.Replace("<", "");
                tempStr = tempStr.Replace(">", "");
                tempStr = tempStr.Replace("'", "‘");
                tempStr = tempStr.Replace("\"", "");
            }
            return tempStr;
        }
        /// <summary>
        /// 过滤危险字符，HTML
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SafeDataHTML(string str)//　数据过滤
        {
            string tempStr = str;
            if (tempStr != null)
            {
                tempStr = tempStr.Replace("'", "‘");
            }
            return tempStr;
        }

        /// <summary>
        /// 字符串截取
        /// </summary>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string CutString(object input, int len)
        {
            if (input == null)
                return "";

            string inputString = input.ToString();
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }
            //如果截过则加上半个省略号   
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "...";

            return tempString;
        }

        /// <summary>
        /// 除去字符串的开头和结尾的字符 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string CutString_Head(object obj)
        {
            obj = obj.ToString().Substring(1);
            obj = obj.ToString().Substring(0, obj.ToString().Length - 1);
            return obj.ToString();
        }
        #endregion 

         #region 数据类型验证
        /// <summary>
        /// 验证传递过来的参数是否是数字
        /// </summary>
        /// <param name="objName"></param>
        /// <returns>true为数字</returns>
        public static bool RequestIsNumber(string objName)
        {
            object obj = System.Web.HttpContext.Current.Request.QueryString[objName];
            if (obj != null && IsNumberR(obj.ToString()))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 检测obj是否为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(object obj)
        {
            if (obj == null)
                return true;
            else
                return false;
        }

        public static bool IsNumber(string lstr)
        {
            bool isDecimal = false;
            for (int i = 0; i < lstr.Length; i++)
            {
                char ochar = lstr[i];
                if (i == 0 && ochar == '-')
                    continue;
                if (ochar == '.' && !isDecimal)
                {
                    isDecimal = true;
                    continue;
                }
                if (ochar < '0' || ochar > '9')
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsInt(string str)
        {
            try
            {
                Convert.ToInt32(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNumberR(string lstr)
        {
            bool IsNum = Regex.IsMatch(lstr, @"^\d+$");
            return IsNum;
        }

        public static bool IsDateTime(string str)
        {
            try
            {
                Convert.ToDateTime(str);
                return true;
            }
            catch
            {
                return false;
            }
        }



        #endregion
 
         #region 获取页面传递参数
        /// <summary>
        /// 获取get方式提交参数和url提交参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Query(string key)
        {
            if (HttpContext.Current.Request.QueryString[key] == null)
                return "";
            else
                return HttpContext.Current.Request.QueryString[key].ToString();
        }

        /// <summary>
        /// 获取post方式提交的参数 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Form(string key)
        {
            if (HttpContext.Current.Request.Form[key] == null)
            {
                return "";
            }
            else {
                return HttpContext.Current.Request.Form[key].ToString();
            }
        }
        #endregion

         #region MD5
        /// <summary>
        ///32位 md5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5_32(string str)
        {
            byte[] arrHashInput;
            byte[] arrHashOutput;
            //MD5CryptoServiceProvider objMD5  = new MD5CryptoServiceProvider();
            MD5 objMD5 = new MD5CryptoServiceProvider();
            arrHashInput = C2B(str);
            arrHashOutput = objMD5.ComputeHash(arrHashInput);
            return BitConverter.ToString(arrHashOutput);
        }
        protected static byte[] C2B(string str)
        {
            char[] arrChar;
            arrChar = str.ToCharArray();
            byte[] arrByte = new byte[arrChar.Length];
            for (int i = 0; i < arrChar.Length; i++)
            {
                arrByte[i] = Convert.ToByte(arrChar[i]);
            }
            return arrByte;
        }

        /// <summary>
        /// 16位 MD5加密
        /// </summary>
        /// <param name="sInputString"></param>
        /// <returns></returns>
        public static string GetMD5_16(string sInputString)
        {
            byte[] data = Encoding.UTF8.GetBytes(sInputString);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            string sKey = GenerateKey();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return BitConverter.ToString(result);
        }

        // 创建Key
        public static string GenerateKey()
        {
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }
        // 加密字符串
        public static string EncryptString(string sInputString, string sKey)
        {
            byte[] data = Encoding.UTF8.GetBytes(sInputString);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return BitConverter.ToString(result);
        }
        // 解密字符串
        public static string DecryptString(string sInputString, string sKey)
        {
            string[] sInput = sInputString.Split("-".ToCharArray());
            byte[] data = new byte[sInput.Length];
            for (int i = 0; i < sInput.Length; i++)
            {
                data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
            }
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateDecryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(result);
        }
        #endregion


         #region 控件编辑操作
        /// <summary>
        /// 为DropDownList添加项
        /// </summary>
        /// <param name="ddl">当前编辑的DropDownList控件</param>
        /// <param name="dt">绑定的数据源</param>
        /// <param name="strText">ListItem的Text</param>
        /// <param name="strValue">ListItem的Value</param>
        public static void SelectOptionAdd(DropDownList ddl, DataTable dt, string strText, string strValue) {
            if (dt != null && dt.Rows.Count != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ddl.Items.Add(new ListItem(dt.Rows[i][strText].ToString(), dt.Rows[i][strValue].ToString()));
                }
                ddl.SelectedIndex = 0;
            }
        }

        public static void SetOption(DropDownList ddl, int s, int e, string tip)
        {
            if (s > e)
            {
                for (int i = e; i <= s; i++)
                {
                    ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
            }
            else
            {
                for (int i = e; i >= s; i--)
                {
                    ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
            }
            if (tip != null)
            {
                ddl.Items.Insert(0, new ListItem(tip, ""));
            }
        }
        #endregion

        /// <summary>
        /// 将数据类型转换为string类型
        /// </summary>
        /// <param name="num">需要转换的数字</param>
        /// <param name="strLength">要转换成的字符串长度</param>
        /// <returns></returns>
        public static string ConvertIntToString(int num, int strLength)
        {
            string strNum=num.ToString();
            string strReturn = "";
            for (int i = 0; i < strLength - strNum.Length; i++)
            {
                strReturn += "0";
            }

            return strReturn+strNum;
        }

        public static string ListToString(List<string> list, char c) {
            if (list.Count <= 0) {
                return "";
            }
            var returnValue = "";
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    returnValue = list[i];
                }
                else {
                    returnValue += c + list[i];
                }
            }
            return returnValue;
        }

        /// <summary>
        /// 获取文件的类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetExName(string fileName)
        {
            int dotPos = fileName.LastIndexOf(".");
            return fileName.Substring(dotPos);
        }

        /// <summary>
        /// 时间转换成SN
        /// </summary>
        /// <returns></returns>
        public static string GetSN()
        {
            string _sn = (DateTime.Now).ToString();
            _sn = _sn.Replace("-", "");
            _sn = _sn.Replace(":", "");
            _sn = _sn.Replace(" ", "");
            _sn = _sn.Replace("/", "");
            return _sn;

        }
        /// <summary>  
        /// 比较两个List string是否相等  
        /// </summary>  
        /// <param name="list1"></param>  
        /// <param name="list2"></param>  
        /// <returns></returns>  
        private static bool SameListString(List<string> list1, List<string> list2)
        {
            if (null == list1 && null == list2)
                return true;
            if (null == list1 || null == list2)
                return false;
            if (list1.Count != list2.Count)
                return false;
            list1.Sort();
            list2.Sort();
            int nCount = list1.Count;
            for (int n = 0; n < nCount; n++)
            {
                if (0 != string.Compare(list1[n], list2[n], false))
                {
                    return false;
                }
            }
            return true;
        }


        public static bool compareArr(object[] arr1, object[] arr2)
        {
            
            return Enumerable.SequenceEqual(arr1, arr2);

        }

        /// <summary>
        /// 获取日期中的年月
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string getDate_YM(DateTime dt) {
            string year = dt.Year.ToString();
            string month = dt.Month.ToString();
            if (dt.Month < 10) {
                month = "0" + dt.Month.ToString();
            }
            return year + month;
        }

        /// <summary>
        /// 获取日期中的年月日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string getDate_YMD(DateTime dt)
        {
            string year = dt.Year.ToString();
            string month = dt.Month.ToString();
            if (dt.Month < 10)
            {
                month = "0" + dt.Month.ToString();
            }
            string day = dt.Day.ToString();
            if (dt.Day < 10)
            {
                day = "0" + dt.Day.ToString();
            }
            return year + month+day;
        }

        public static string getGUID()
        {
            System.Guid guid = new Guid();
            guid = Guid.NewGuid();
            string str = guid.ToString();
            return str;
        }

        /// <summary>
        /// 转换人民币大小金额
        /// </summary>
        /// <param name="num">金额</param>
        /// <returns>返回大写形式</returns>
        public static string ChangeToRMB(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字
            string str3 = "";    //从原num值中取出的值
            string str4 = "";    //数字的字符串形式
            string str5 = "";  //人民币大写金额形式
            int i;    //循环变量
            int j;    //num的值乘以100的字符串长度
            string ch1 = "";    //数字的汉语读法
            string ch2 = "";    //数字位的汉字读法
            int nzero = 0;  //用来计算连续的零值是几个
            int temp;            //从原num值中取出的值

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式
            j = str4.Length;      //找出最高位
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分

            //循环取出每一位需要转换的值
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值
                temp = Convert.ToInt32(str3);      //转换为数字
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整”
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }

        /**/
        /// <summary>
        /// 一个重载，将字符串先转换成数字在调用CmycurD(decimal num)
        /// </summary>
        /// <param name="num">用户输入的金额，字符串形式未转成decimal</param>
        /// <returns></returns>
        public static string ChangeToRMB(string numstr)
        {
            try
            {
                decimal num = Convert.ToDecimal(numstr);
                return ChangeToRMB(num);
            }
            catch
            {
                return "非数字形式！";
            }
        }

    }


    public class PageValidate
    {
        public PageValidate()
        {
        }

        private static Regex RegNumber = new Regex("^[0-9]+$");
        private static Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");
        private static Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$");
        private static Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //等价于^[+-]?\d+[.]?\d+$
        private static Regex RegEmail = new Regex(@"^[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?$");
        private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
        private static Regex RegCell = new Regex(@"^[\d]{11}$");
        #region 国内座机验证
        /// <summary>
        /// 座机验证
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsPhone(string inputData)
        {
            Match m = new Regex(@"^\d{3}-\d{8}|\d{4}-\{7,8}$").Match(inputData);
            return m.Success;
        }
        #endregion

        #region 手机号验证
        /// <summary>
        /// 手机号验证
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsCell(string inputData)
        {
            Match m = RegCell.Match(inputData);
            return m.Success;
        }
        #endregion
        //---- 2013.05.17

        #region 数字字符串检查

        /// <summary>
        /// 检查Request查询字符串的键值，是否是数字，最大长度限制
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="inputKey">Request的键值</param>
        /// <param name="maxLen">最大长度</param>
        /// <returns>返回Request查询字符串</returns>
        public static string FetchInputDigit(HttpRequest req, string inputKey, int maxLen)
        {
            string retVal = string.Empty;
            if (inputKey != null && inputKey != string.Empty)
            {
                retVal = req.QueryString[inputKey];
                if (null == retVal)
                    retVal = req.Form[inputKey];
                if (null != retVal)
                {
                    retVal = SqlText(retVal, maxLen);
                    if (!IsNumber(retVal))
                        retVal = string.Empty;
                }
            }
            if (retVal == null)
                retVal = string.Empty;
            return retVal;
        }
        /// <summary>
        /// 是否数字字符串
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumber(string inputData)
        {
            Match m = RegNumber.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否数字字符串 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumberSign(string inputData)
        {
            Match m = RegNumberSign.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否是浮点数
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(string inputData)
        {
            Match m = RegDecimal.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimalSign(string inputData)
        {
            Match m = RegDecimalSign.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 中文检测

        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsHasCHZN(string inputData)
        {
            Match m = RegCHZN.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 邮件地址
        /// <summary>
        /// 是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsEmail(string inputData)
        {
            Match m = RegEmail.Match(inputData);
            return m.Success;
        }

        #endregion

        #region 时间
        /// <summary>
        /// 是否是时间格式
        /// </summary>
        /// <param name="StrDate">时间字符串</param>
        public static bool IsDateTime(string StrDate)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(StrDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region 其他
        /// <summary>
        /// 检查字符串最大长度，返回指定长度的串
        /// </summary>
        /// <param name="sqlInput">输入字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>			
        public static string SqlText(string sqlInput, int maxLength)
        {
            if (sqlInput != null && sqlInput != string.Empty)
            {
                sqlInput = sqlInput.Trim();
                if (sqlInput.Length > maxLength)//按最大长度截取字符串
                    sqlInput = sqlInput.Substring(0, maxLength);
            }
            return sqlInput;
        }


        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static string HtmlEncode(string inputData)
        {
            return HttpUtility.HtmlEncode(inputData);
        }
        /// <summary>
        /// 设置Label显示Encode的字符串
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="txtInput"></param>
        public static void SetLabel(Label lbl, string txtInput)
        {
            lbl.Text = HtmlEncode(txtInput);
        }
        public static void SetLabel(Label lbl, object inputObj)
        {
            SetLabel(lbl, inputObj.ToString());
        }

        #endregion
    }
}
