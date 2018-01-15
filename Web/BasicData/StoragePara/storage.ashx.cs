using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Web.BasicData.StoragePara
{
    /// <summary>
    /// storage 的摘要说明
    /// </summary>
    public class storage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
        
            if (context.Request.QueryString["type"] != null)
            {
                string strType = context.Request.QueryString["type"].ToString();
                switch (strType)
                {
                    case "GetMeasuringUnit": GetMeasuringUnit(context); break;

                    case "GetStorageVarietyUnitByID": GetStorageVarietyUnitByID(context); break;
                    case "GetStorageVariety": GetStorageVariety(context); break;
                    case "GetStorageLevel_B": GetStorageLevel_B(context); break;
                    case "GetStorageUser": GetStorageUser(context); break;
                    case "GetStorageTime": GetStorageTime(context); break;

                    case "GetStorageType": GetStorageType(context); break;//获取储户类型
                    case "GetStorageUserByID": GetStorageUserByID(context); break;
                    case "AddStorageUser": AddStorageUser(context); break;
                    case "UpdateStorageUser": UpdateStorageUser(context); break;
                    case "DeleteStorageUserByID": DeleteStorageUserByID(context); break;

                    case "GetStorageVarietyLevel": GetStorageVarietyLevel(context); break;
                    case "GetStorageVarietyByID": GetStorageVarietyByID(context); break;
                    case "AddStorageVariety": AddStorageVariety(context); break;
                    case "UpdateStorageVariety": UpdateStorageVariety(context); break;
                    case "DeleteStorageVarietyByID": DeleteStorageVarietyByID(context); break;

                    case "GetStorageTimeByTypeID": GetStorageTimeByTypeID(context); break;
                    case "GetStorageTimeByID": GetStorageTimeByID(context); break;
                    case "AddStorageTime": AddStorageTime(context); break;
                    case "UpdateStorageTime": UpdateStorageTime(context); break;
                    case "DeleteStorageTimeByID": DeleteStorageTimeByID(context); break;
                    case "getPricePolicy": getPricePolicy(context); break;
                        
                        

                    case "GetStorageLevelByVarietyID": GetStorageLevelByVarietyID(context); break;
                    case "GetStorageLevelByID": GetStorageLevelByID(context); break;
                    case "AddStorageLevel": AddStorageLevel(context); break;
                    case "UpdateStorageLevel": UpdateStorageLevel(context); break;
                    case "DeleteStorageLevelByID": DeleteStorageLevelByID(context); break;

                    case "GetStorageRateAll": GetStorageRateAll(context); break;
                    case "GetStorageRateLogAll": GetStorageRateLogAll(context); break;
                    case "GetStorageRateWBByID": GetStorageRateWBByID(context); break;
                    case "UpdateStorageRateWB": UpdateStorageRateWB(context); break;
                    case "GetStorageRateByID": GetStorageRateByID(context); break;
                    case "AddStorageRate": AddStorageRate(context); break;
                    case "UpdateStorageRate": UpdateStorageRate(context); break;
                    case "DeleteStorageRateByID": DeleteStorageRateByID(context); break;

                    case "GetStorageFeeByID": GetStorageFeeByID(context); break;
                    case "AddStorageFee": AddStorageFee(context); break;
                    case "UpdateStorageFee": UpdateStorageFee(context); break;
                    case "DeleteStorageFeeByID": DeleteStorageFeeByID(context); break;

                    case "UpdateExchangeProp": UpdateExchangeProp(context); break;//设置商品的兑换比例
                    case "DeleteExchangeProp": DeleteExchangeProp(context); break;//删除商品的兑换比例
                }
            }

        }

        void GetMeasuringUnit(HttpContext context)
        {
            
            string strSql = " SELECT ID,strName FROM dbo.BD_MeasuringUnit";
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





        void GetStorageUser(HttpContext context)
        {
            
            string strSql = " SELECT ID,strName FROM dbo.StorageType  ";
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



        void GetStorageType(HttpContext context)
        {
           
            string strSql = " SELECT ID,strName FROM dbo.StorageType " ;
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


        void GetStorageUserByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string strSql = " SELECT ID,strName FROM dbo.StorageType  WHERE ID=" + wbid;
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);

            if (dt!=null&&dt.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void AddStorageUser(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();


            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageType WHERE strName='" + strName + "'").ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [StorageType] (");
            strSql.Append("strName,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@strName,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
				
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
                    new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = strName;
            parameters[1].Value = 0;
            parameters[2].Value = 1;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateStorageUser(HttpContext context)
        {
             string wbid = context.Request.QueryString["ID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [StorageType] set strName='" + strName + "' where ID=" + wbid);
          

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteStorageUserByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageRate WHERE TypeID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Dep_StorageInfo WHERE TypeID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }


            string strSql = " delete FROM dbo.StorageType WHERE ID=" + wbid;


            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetStorageVarietyByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string strSql = " SELECT ID,strName,MeasuringUnitID,AgencyFee,ISDefault,numSort FROM dbo.StorageVariety  WHERE ID=" + wbid;
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

        void GetStorageVarietyLevel(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT top 1 C.strName");
            strSql.Append(" FROM dbo.StorageVarietyLevel_L A INNER JOIN dbo.StorageVariety B ON A.VarietyID=B.ID");
            strSql.Append(" INNER JOIN dbo.StorageVarietyLevel_B C ON A.VarietyLevelID=C.ID");
            strSql.Append(" WHERE B.ID=" + VarietyID);

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());
            if (obj == null)
            {
                obj = "";
            }

            context.Response.Write(obj);
            
        }

        /// <summary>
        /// 获取存储产品的计量单位
        /// </summary>
        /// <param name="context"></param>
        void GetStorageVarietyUnitByID(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("  SELECT TOP 1 B.strName");
            strSql.Append("  FROM dbo.StorageVariety A INNER JOIN dbo.BD_MeasuringUnit B ON A.MeasuringUnitID=B.ID");
            strSql.Append("  WHERE A.ID=" + VarietyID);

            object obj = SQLHelper.ExecuteScalar(strSql.ToString());

            if (obj != null )
            {
                context.Response.Write(obj.ToString());
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetStorageVariety(HttpContext context)
        {
            string strSql = " SELECT ID,strName,MeasuringUnitID,AgencyFee,ISDefault,numSort FROM dbo.StorageVariety " ;
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

        void GetStorageLevelByVarietyID(HttpContext context)
        {
            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT B.ID,B.strName ");
            strSql.Append("  FROM dbo.StorageVarietyLevel_L A INNER JOIN  dbo.StorageVarietyLevel_B B ON A.VarietyLevelID=B.ID");
            strSql.Append("  WHERE A.VarietyID=" + VarietyID);

            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetStorageLevel_B(HttpContext context)
        {
            string strSql = " SELECT ID,strName FROM dbo.StorageVarietyLevel_B";
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

        void AddStorageVariety(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            string MeasuringUnitID = context.Request.Form["MeasuringUnitID"].ToString();
            string AgencyFee = context.Request.Form["AgencyFee"].ToString();

            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageVariety WHERE strName='" + strName + "'").ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [StorageVariety] (");
            strSql.Append(" strName , MeasuringUnitID , AgencyFee , ISDefault , numSort)");
            strSql.Append(" values (");
            strSql.Append("'"+strName+"',"+MeasuringUnitID+","+AgencyFee+",0,1)");
            strSql.Append(";select @@IDENTITY");
           
            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateStorageVariety(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string strName = context.Request.Form["strName"].ToString();
            string MeasuringUnitID = context.Request.Form["MeasuringUnitID"].ToString();
            string AgencyFee = context.Request.Form["AgencyFee"].ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [StorageVariety]");
            strSql.Append(" set strName='" + strName );
            strSql.Append("', MeasuringUnitID=" + MeasuringUnitID);
            strSql.Append(", AgencyFee=" + AgencyFee);
            strSql.Append(" where ID=" + wbid);


            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteStorageVarietyByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();


            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageVarietyLevel_L WHERE VarietyID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageRate WHERE VarietyID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Dep_StorageInfo WHERE VarietyID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            string strSql = " delete FROM dbo.StorageVariety WHERE ID=" + wbid;


            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void GetStorageTime(HttpContext context)
        
        {
            string strSql = " select ID,TypeID,strName,ISRegular,numStorageDate,InterestType,CalculateInterest,numExChangeProp,PricePolicy,ISDefault,numSort from StorageTime";
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

        void GetStorageTimeByTypeID(HttpContext context)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT ID,strName,InterestType,PricePolicy,ISRegular,numStorageDate");
            strSql.Append("  FROM dbo.StorageTime");
            
            string TypeID = "";
            if (context.Request.QueryString["TypeID"] != null)
            {
                 TypeID = context.Request.QueryString["TypeID"].ToString();
                 strSql.Append("  WHERE TypeID=" + TypeID);
            }

           
            


            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetStorageTimeByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string strSql = " select ID,TypeID,strName,ISRegular,numStorageDate,InterestType,CalculateInterest,numExChangeProp,limitExChangeProp,PricePolicy,ISDefault,numSort  FROM dbo.StorageTime WHERE ID=" + wbid;
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

        void AddStorageTime(HttpContext context)
        {

            string TypeID = context.Request.Form["TypeID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            bool ISRegular = false;
            if (context.Request.Form["ISRegular"].ToString() == "2")
            {
                ISRegular = true;
            }

            string numStorageDate = context.Request.Form["numStorageDate"].ToString();
            string InterestType = context.Request.Form["InterestType"].ToString();
            bool CalculateInterest = false;
            if (context.Request.Form["CalculateInterest"] != null)
            {
                CalculateInterest = true;
            }
            string numExChangeProp = context.Request.Form["numExChangeProp"].ToString();
            string limitExChangeProp = context.Request.Form["limitExChangeProp"].ToString();
            string PricePolicy = context.Request.Form["PricePolicy"].ToString();


            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageTime WHERE TypeID=" + TypeID + " and strName='" + strName + "'").ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [StorageTime] (");
            strSql.Append("TypeID,strName,ISRegular,numStorageDate,InterestType,CalculateInterest,numExChangeProp,limitExChangeProp,PricePolicy,ISDefault,numSort)");
            strSql.Append(" values (");
            strSql.Append("@TypeID,@strName,@ISRegular,@numStorageDate,@InterestType,@CalculateInterest,@numExChangeProp,@limitExChangeProp,@PricePolicy,@ISDefault,@numSort)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISRegular", SqlDbType.Bit,1),
					new SqlParameter("@numStorageDate", SqlDbType.Int,4),
					new SqlParameter("@InterestType", SqlDbType.Int,4),
					new SqlParameter("@CalculateInterest", SqlDbType.Bit,1),
					new SqlParameter("@numExChangeProp", SqlDbType.Decimal,9),
                    new SqlParameter("@limitExChangeProp", SqlDbType.Decimal,9),
                    new SqlParameter("@PricePolicy", SqlDbType.Int,4),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4)};
            parameters[0].Value = TypeID;
            parameters[1].Value = strName;
            parameters[2].Value = ISRegular;
            parameters[3].Value = numStorageDate;
            parameters[4].Value = InterestType;
            parameters[5].Value = CalculateInterest;
            parameters[6].Value = numExChangeProp;
            parameters[7].Value = limitExChangeProp;
            parameters[8].Value = PricePolicy;
            parameters[9].Value = 0;
            parameters[10].Value = 1;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateStorageTime(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            string TypeID = context.Request.Form["txtTypeID"].ToString();
            string strName = context.Request.Form["strName"].ToString();
            bool ISRegular = false;
            if (context.Request.Form["ISRegular"].ToString() == "2")
            {
                ISRegular = true;
            }
            string numStorageDate = context.Request.Form["numStorageDate"].ToString();
            string InterestType = context.Request.Form["InterestType"].ToString();
            bool CalculateInterest = false;
            if (context.Request.Form["CalculateInterest"] != null)
            {
                CalculateInterest = true;
            }
            string numExChangeProp = context.Request.Form["numExChangeProp"].ToString();
            string limitExChangeProp = context.Request.Form["limitExChangeProp"].ToString();
            string PricePolicy = context.Request.Form["PricePolicy"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [StorageTime] set ");
            strSql.Append("TypeID=@TypeID,");
            strSql.Append("strName=@strName,");
            strSql.Append("ISRegular=@ISRegular,");
            strSql.Append("numStorageDate=@numStorageDate,");
            strSql.Append("InterestType=@InterestType,");
            strSql.Append("CalculateInterest=@CalculateInterest,");
            strSql.Append("numExChangeProp=@numExChangeProp,");
            strSql.Append("limitExChangeProp=@limitExChangeProp,");
            strSql.Append("PricePolicy=@PricePolicy,");
            strSql.Append("ISDefault=@ISDefault,");
            strSql.Append("numSort=@numSort");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@strName", SqlDbType.NVarChar,50),
					new SqlParameter("@ISRegular", SqlDbType.Bit,1),
					new SqlParameter("@numStorageDate", SqlDbType.Int,4),
					new SqlParameter("@InterestType", SqlDbType.Int,4),
					new SqlParameter("@CalculateInterest", SqlDbType.Bit,1),
					new SqlParameter("@numExChangeProp", SqlDbType.Decimal,9),
                    	new SqlParameter("@limitExChangeProp", SqlDbType.Decimal,9),
                    	new SqlParameter("@PricePolicy", SqlDbType.Int,9),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = TypeID;
            parameters[1].Value = strName;
            parameters[2].Value = ISRegular;
            parameters[3].Value = numStorageDate;
            parameters[4].Value = InterestType;
            parameters[5].Value = CalculateInterest;
            parameters[6].Value = numExChangeProp;
            parameters[7].Value = limitExChangeProp;
            parameters[8].Value = PricePolicy;
            parameters[9].Value = 0;
            parameters[10].Value = 1;
            parameters[11].Value = ID;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(),parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteStorageTimeByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageRate WHERE TimeID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Dep_StorageInfo WHERE TimeID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            string strSql = " delete FROM dbo.StorageTime WHERE ID=" + wbid;


            if (SQLHelper.ExecuteNonQuery(strSql) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

      

         /// <summary>
        /// 保存产品价格政策 
        /// </summary>
        /// <param name="context"></param>
        void getPricePolicy(HttpContext context)
        {
            string strSql = " SELECT TOP 1 strName,strAddress,strLink,strPhone,strRemark  FROM dbo.BD_Company";
            DataTable dt = SQLHelper.ExecuteDataTable(strSql);

            if (dt != null & dt.Rows.Count!=0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        
        void GetStorageLevelByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,VarietyID,VarietyLevelID,ISDefault,numSort,YingDu,ShuiFen,ShuiFen_CK,ShuiFen_DZ,Rongzhong,Rongzhong_DK,Rongzhong_CZ,ZaZhi,ZaZhi_CK,ZaZhi_DZ,ChuCao,ChuCao_CZ,ChuCao_DK,MeiBian,MeiBian_CK,MeiBian_DZ ");
            strSql.Append(" FROM [StorageVarietyLevel_L] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = wbid;
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

        void AddStorageLevel(HttpContext context)
        {


            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string VarietyLevelID = context.Request.Form["VarietyLevelID"].ToString();
            string YingDu = context.Request.Form["YingDu"].ToString();

            string ShuiFen = context.Request.Form["ShuiFen"].ToString();
            string ShuiFen_CK = context.Request.Form["ShuiFen_CK"].ToString();
            string ShuiFen_DZ = context.Request.Form["ShuiFen_DZ"].ToString();

            string Rongzhong = context.Request.Form["Rongzhong"].ToString();
            string Rongzhong_DK = context.Request.Form["Rongzhong_DK"].ToString();
            string Rongzhong_CZ = context.Request.Form["Rongzhong_CZ"].ToString();

            string ZaZhi = context.Request.Form["ZaZhi"].ToString();
            string ZaZhi_CK = context.Request.Form["ZaZhi_CK"].ToString();
            string ZaZhi_DZ = context.Request.Form["ZaZhi_DZ"].ToString();

            string ChuCao = context.Request.Form["ChuCao"].ToString();
            string ChuCao_CZ = context.Request.Form["ChuCao_CZ"].ToString();
            string ChuCao_DK = context.Request.Form["ChuCao_DK"].ToString();

            string MeiBian = context.Request.Form["MeiBian"].ToString();
            string MeiBian_CK = context.Request.Form["MeiBian_CK"].ToString();
            string MeiBian_DZ = context.Request.Form["MeiBian_DZ"].ToString();

            string strSqlExit = "select count(ID) from StorageVarietyLevel_L where VarietyID=" + VarietyID + " and VarietyLevelID=" + VarietyLevelID;
            if (SQLHelper.ExecuteScalar(strSqlExit).ToString() != "0") {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [StorageVarietyLevel_L] (");
            strSql.Append("VarietyID,VarietyLevelID,ISDefault,numSort,YingDu,ShuiFen,ShuiFen_CK,ShuiFen_DZ,Rongzhong,Rongzhong_DK,Rongzhong_CZ,ZaZhi,ZaZhi_CK,ZaZhi_DZ,ChuCao,ChuCao_CZ,ChuCao_DK,MeiBian,MeiBian_CK,MeiBian_DZ)");
            strSql.Append(" values (");
            strSql.Append("@VarietyID,@VarietyLevelID,@ISDefault,@numSort,@YingDu,@ShuiFen,@ShuiFen_CK,@ShuiFen_DZ,@Rongzhong,@Rongzhong_DK,@Rongzhong_CZ,@ZaZhi,@ZaZhi_CK,@ZaZhi_DZ,@ChuCao,@ChuCao_CZ,@ChuCao_DK,@MeiBian,@MeiBian_CK,@MeiBian_DZ)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4),
					new SqlParameter("@YingDu", SqlDbType.Decimal,9),
					new SqlParameter("@ShuiFen", SqlDbType.Decimal,9),
					new SqlParameter("@ShuiFen_CK", SqlDbType.Decimal,9),
					new SqlParameter("@ShuiFen_DZ", SqlDbType.Decimal,9),
					new SqlParameter("@Rongzhong", SqlDbType.Decimal,9),
					new SqlParameter("@Rongzhong_DK", SqlDbType.Decimal,9),
					new SqlParameter("@Rongzhong_CZ", SqlDbType.Decimal,9),
					new SqlParameter("@ZaZhi", SqlDbType.Decimal,9),
					new SqlParameter("@ZaZhi_CK", SqlDbType.Decimal,9),
					new SqlParameter("@ZaZhi_DZ", SqlDbType.Decimal,9),
					new SqlParameter("@ChuCao", SqlDbType.Decimal,9),
					new SqlParameter("@ChuCao_CZ", SqlDbType.Decimal,9),
					new SqlParameter("@ChuCao_DK", SqlDbType.Decimal,9),
					new SqlParameter("@MeiBian", SqlDbType.Decimal,9),
					new SqlParameter("@MeiBian_CK", SqlDbType.Decimal,9),
					new SqlParameter("@MeiBian_DZ", SqlDbType.Decimal,9)};
            parameters[0].Value = VarietyID;
            parameters[1].Value = VarietyLevelID;
            parameters[2].Value = false;
            parameters[3].Value = 1;
            parameters[4].Value = YingDu;
            parameters[5].Value = ShuiFen;
            parameters[6].Value = ShuiFen_CK;
            parameters[7].Value = ShuiFen_DZ;
            parameters[8].Value = Rongzhong;
            parameters[9].Value = Rongzhong_DK;
            parameters[10].Value = Rongzhong_CZ;
            parameters[11].Value = ZaZhi;
            parameters[12].Value = ZaZhi_CK;
            parameters[13].Value = ZaZhi_DZ;
            parameters[14].Value = ChuCao;
            parameters[15].Value = ChuCao_CZ;
            parameters[16].Value = ChuCao_DK;
            parameters[17].Value = MeiBian;
            parameters[18].Value = MeiBian_CK;
            parameters[19].Value = MeiBian_DZ;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateStorageLevel(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string VarietyLevelID = context.Request.Form["VarietyLevelID"].ToString();
            string YingDu = context.Request.Form["YingDu"].ToString();

            string ShuiFen = context.Request.Form["ShuiFen"].ToString();
            string ShuiFen_CK = context.Request.Form["ShuiFen_CK"].ToString();
            string ShuiFen_DZ = context.Request.Form["ShuiFen_DZ"].ToString();

            string Rongzhong = context.Request.Form["Rongzhong"].ToString();
            string Rongzhong_DK = context.Request.Form["Rongzhong_DK"].ToString();
            string Rongzhong_CZ = context.Request.Form["Rongzhong_CZ"].ToString();

            string ZaZhi = context.Request.Form["ZaZhi"].ToString();
            string ZaZhi_CK = context.Request.Form["ZaZhi_CK"].ToString();
            string ZaZhi_DZ = context.Request.Form["ZaZhi_DZ"].ToString();

            string ChuCao = context.Request.Form["ChuCao"].ToString();
            string ChuCao_CZ = context.Request.Form["ChuCao_CZ"].ToString();
            string ChuCao_DK = context.Request.Form["ChuCao_DK"].ToString();

            string MeiBian = context.Request.Form["MeiBian"].ToString();
            string MeiBian_CK = context.Request.Form["MeiBian_CK"].ToString();
            string MeiBian_DZ = context.Request.Form["MeiBian_DZ"].ToString();


            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [StorageVarietyLevel_L] set ");
            strSql.Append("VarietyID=@VarietyID,");
            strSql.Append("VarietyLevelID=@VarietyLevelID,");
            strSql.Append("ISDefault=@ISDefault,");
            strSql.Append("numSort=@numSort,");
            strSql.Append("YingDu=@YingDu,");
            strSql.Append("ShuiFen=@ShuiFen,");
            strSql.Append("ShuiFen_CK=@ShuiFen_CK,");
            strSql.Append("ShuiFen_DZ=@ShuiFen_DZ,");
            strSql.Append("Rongzhong=@Rongzhong,");
            strSql.Append("Rongzhong_DK=@Rongzhong_DK,");
            strSql.Append("Rongzhong_CZ=@Rongzhong_CZ,");
            strSql.Append("ZaZhi=@ZaZhi,");
            strSql.Append("ZaZhi_CK=@ZaZhi_CK,");
            strSql.Append("ZaZhi_DZ=@ZaZhi_DZ,");
            strSql.Append("ChuCao=@ChuCao,");
            strSql.Append("ChuCao_CZ=@ChuCao_CZ,");
            strSql.Append("ChuCao_DK=@ChuCao_DK,");
            strSql.Append("MeiBian=@MeiBian,");
            strSql.Append("MeiBian_CK=@MeiBian_CK,");
            strSql.Append("MeiBian_DZ=@MeiBian_DZ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@ISDefault", SqlDbType.Bit,1),
					new SqlParameter("@numSort", SqlDbType.Int,4),
					new SqlParameter("@YingDu", SqlDbType.Decimal,9),
					new SqlParameter("@ShuiFen", SqlDbType.Decimal,9),
					new SqlParameter("@ShuiFen_CK", SqlDbType.Decimal,9),
					new SqlParameter("@ShuiFen_DZ", SqlDbType.Decimal,9),
					new SqlParameter("@Rongzhong", SqlDbType.Decimal,9),
					new SqlParameter("@Rongzhong_DK", SqlDbType.Decimal,9),
					new SqlParameter("@Rongzhong_CZ", SqlDbType.Decimal,9),
					new SqlParameter("@ZaZhi", SqlDbType.Decimal,9),
					new SqlParameter("@ZaZhi_CK", SqlDbType.Decimal,9),
					new SqlParameter("@ZaZhi_DZ", SqlDbType.Decimal,9),
					new SqlParameter("@ChuCao", SqlDbType.Decimal,9),
					new SqlParameter("@ChuCao_CZ", SqlDbType.Decimal,9),
					new SqlParameter("@ChuCao_DK", SqlDbType.Decimal,9),
					new SqlParameter("@MeiBian", SqlDbType.Decimal,9),
					new SqlParameter("@MeiBian_CK", SqlDbType.Decimal,9),
					new SqlParameter("@MeiBian_DZ", SqlDbType.Decimal,9),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = VarietyID;
            parameters[1].Value = VarietyLevelID;
            parameters[2].Value = 0;
            parameters[3].Value = 1;
            parameters[4].Value = YingDu;
            parameters[5].Value = ShuiFen;
            parameters[6].Value = ShuiFen_CK;
            parameters[7].Value = ShuiFen_DZ;
            parameters[8].Value = Rongzhong;
            parameters[9].Value = Rongzhong_DK;
            parameters[10].Value = Rongzhong_CZ;
            parameters[11].Value = ZaZhi;
            parameters[12].Value = ZaZhi_CK;
            parameters[13].Value = ZaZhi_DZ;
            parameters[14].Value = ChuCao;
            parameters[15].Value = ChuCao_CZ;
            parameters[16].Value = ChuCao_DK;
            parameters[17].Value = MeiBian;
            parameters[18].Value = MeiBian_CK;
            parameters[19].Value = MeiBian_DZ;
            parameters[20].Value = wbid;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(),parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteStorageLevelByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            string VarietyID = context.Request.QueryString["VarietyID"].ToString();
            string VarietyLevelID = context.Request.QueryString["VarietyLevelID"].ToString();
            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.StorageRate  WHERE VarietyID="+VarietyID+" AND VarietyLevelID=" + VarietyLevelID).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [StorageVarietyLevel_L] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = wbid;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(),parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetStorageRateWBByID(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT SW.WBID");
            sql.Append(" FROM dbo.StorageRate S INNER JOIN StorageRate_WB SW ON S.ID=SW.StorageRateID");
            sql.Append(" WHERE S.ID="+ID);
            DataTable dt = SQLHelper.ExecuteDataTable(sql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateStorageRateWB(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();

            List<string> wblist = new List<string>();
            foreach (var key in context.Request.Form.Keys)
            {
                if (key.ToString().IndexOf("chkWB") == -1)
                {
                    continue;
                }
                string strKey = key.ToString();
                string strValue = context.Request.Form[key.ToString()].ToString();
                string WBID = key.ToString().Substring(key.ToString().IndexOf('_') + 1);

                if (!wblist.Contains(WBID))
                {
                    wblist.Add(WBID);
                }
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format(" delete from StorageRate_WB where StorageRateID={0}", ID));
            for (int i = 0; i < wblist.Count; i++) {
                strSql.Append(string.Format(" insert into StorageRate_WB (StorageRateID,WBID) VALUES({0},{1})",ID,wblist[i]));
            }

            using (SqlTransaction tran = SQLHelper.BeginTransaction(SQLHelper.connectionString))
            {
                try
                {
                    SQLHelper.ExecuteNonQuery(tran, CommandType.Text, strSql.ToString());               
                    tran.Commit();
                    context.Response.Write("OK");
                }
                catch
                {
                    tran.Rollback();
                    context.Response.Write("Error");
                }
            }
        }


        void GetStorageRateAll(HttpContext context)
        {
            string QVarietyID = "";
            if (context.Request.Form["QVarietyID"] != null && context.Request.Form["QVarietyID"].ToString() != "0" && context.Request.Form["QVarietyID"].ToString() != "null")
            {
                QVarietyID = context.Request.Form["QVarietyID"].ToString();
            }
            string QTimeID = "";
            if (context.Request.Form["QTimeID"] != null && context.Request.Form["QTimeID"].ToString() != "0" && context.Request.Form["QTimeID"].ToString() != "null")
            {
                QTimeID = context.Request.Form["QTimeID"].ToString();
            }
            string QWBID = "";
            if (context.Request.Form["QWBID"] != null && context.Request.Form["QWBID"].ToString() != "0" && context.Request.Form["QWBID"].ToString() != "null")
            {
                QWBID = context.Request.Form["QWBID"].ToString();
            }
             

            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select A.ID,B.strName AS TypeID,C.strName AS VarietyID,D.strName AS VarietyLevelID,E.strName AS TimeID,");
            strSql.Append("     StorageFee,BankRate,CurrentRate,EarningRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou ");
            strSql.Append("     FROM dbo.StorageRate A INNER JOIN dbo.StorageType B ON A.TypeID=B.ID");
            strSql.Append("     INNER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSql.Append("     INNER JOIN dbo.StorageVarietyLevel_B D ON A.VarietyLevelID=D.ID");
            strSql.Append("     INNER JOIN dbo.StorageTime E ON A.TimeID=E.ID");
            if (QVarietyID != "") {
                strSql.Append(string.Format(" and C.ID={0}", QVarietyID));
            }
            if (QTimeID != "")
            {
                strSql.Append(string.Format(" and E.ID={0}", QTimeID));
            }
            if (QWBID != "") {
                strSql.Append("  INNER JOIN StorageRate_WB SW ON A.ID=SW.StorageRateID");
                strSql.Append(string.Format(" and SW.WBID={0}", QWBID));
            }
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }


        void GetStorageRateLogAll(HttpContext context)
        {
            string QVarietyID = "";
            if (context.Request.Form["QVarietyID"] != null && context.Request.Form["QVarietyID"].ToString() != "0" && context.Request.Form["QVarietyID"].ToString() != "null")
            {
                QVarietyID = context.Request.Form["QVarietyID"].ToString();
            }
            string QTimeID = "";
            if (context.Request.Form["QTimeID"] != null && context.Request.Form["QTimeID"].ToString() != "0" && context.Request.Form["QTimeID"].ToString() != "null")
            {
                QTimeID = context.Request.Form["QTimeID"].ToString();
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("     select A.ID,B.strName AS TypeID,C.strName AS VarietyID,D.strName AS VarietyLevelID,E.strName AS TimeID,");
            strSql.Append("     StorageFee,BankRate,CurrentRate,EarningRate,LossRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou ");
            strSql.Append("   ,CASE(A.strType) WHEN 'A' THEN '新增' WHEN 'U' THEN '更新' ELSE '删除' END AS strType,  CONVERT(varchar(100), A.dtLog, 23) as dtLog ");
            strSql.Append("     FROM dbo.StorageRateLog A INNER JOIN dbo.StorageType B ON A.TypeID=B.ID");
            strSql.Append("     INNER JOIN dbo.StorageVariety C ON A.VarietyID=C.ID");
            strSql.Append("     INNER JOIN dbo.StorageVarietyLevel_B D ON A.VarietyLevelID=D.ID");
            strSql.Append("     INNER JOIN dbo.StorageTime E ON A.TimeID=E.ID");
            if (QVarietyID != "")
            {
                strSql.Append(string.Format(" and C.ID={0}", QVarietyID));
            }
            if (QTimeID != "")
            {
                strSql.Append(string.Format(" and E.ID={0}", QTimeID));
            }

            strSql.Append("  order by TypeID,TimeID,VarietyID,VarietyLevelID,dtLog");
            DataTable dt = SQLHelper.ExecuteDataTable(strSql.ToString());

            if (dt != null && dt.Rows.Count != 0)
            {
                context.Response.Write(JsonHelper.ToJson(dt));
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void GetStorageRateByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,CurrentRate,EarningRate,LossRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou,synPrice_ShiChang,synPrice_XiaoShou");
            strSql.Append(" FROM [StorageRate] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = wbid;
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

        void AddStorageRate(HttpContext context)
        {


            string TypeID = context.Request.Form["TypeID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string VarietyLevelID = context.Request.Form["VarietyLevelID"].ToString();
            string TimeID = context.Request.Form["TimeID"].ToString();

            string StorageFee = context.Request.Form["StorageFee"].ToString();
            string CurrentRate = context.Request.Form["CurrentRate"].ToString();
            string BankRate = context.Request.Form["BankRate"].ToString();
            string Price_ShiChang = context.Request.Form["Price_ShiChang"].ToString();
            string Price_DaoQi = context.Request.Form["Price_DaoQi"].ToString();
            string Price_HeTong = context.Request.Form["Price_HeTong"].ToString();
            string Price_XiaoShou = context.Request.Form["Price_XiaoShou"].ToString();
            string EarningRate = context.Request.Form["EarningRate"].ToString();
            string LossRate = context.Request.Form["LossRate"].ToString();
            //bool synPrice_ShiChang = false;
            //if (context.Request.Form["synPrice_ShiChang"] != null)
            //{
            //    synPrice_ShiChang = true;
            //}
            //bool synPrice_XiaoShou = false;
            //if (context.Request.Form["synPrice_XiaoShou"] != null)
            //{
            //    synPrice_XiaoShou = true;
            //}
            bool chkPrice_ShiChang = false;
            if (context.Request.Form["chkPrice_ShiChang"] != null)
            {
                chkPrice_ShiChang = true;
            }
            bool chkPrice_XiaoShou = false;
            if (context.Request.Form["chkPrice_XiaoShou"] != null)
            {
                chkPrice_XiaoShou = true;
            }


            string strSqlExit = "select count(ID) from StorageRate where TypeID=" + TypeID + " and VarietyID=" + VarietyID + " and VarietyLevelID=" + VarietyLevelID + " and TimeID=" + TimeID;
            if (SQLHelper.ExecuteScalar(strSqlExit).ToString() != "0")
            {
                context.Response.Write("1");
                return;
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [StorageRate] (");
            strSql.Append("TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,CurrentRate,EarningRate,LossRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou,synPrice_ShiChang,synPrice_XiaoShou)");
            strSql.Append(" values (");
            strSql.Append("@TypeID,@VarietyID,@VarietyLevelID,@TimeID,@StorageFee,@BankRate,@CurrentRate,@EarningRate,@LossRate,@Price_ShiChang,@Price_DaoQi,@Price_HeTong,@Price_XiaoShou,@synPrice_ShiChang,@synPrice_XiaoShou)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@BankRate", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@EarningRate", SqlDbType.Decimal,9),
                    new SqlParameter("@LossRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
					new SqlParameter("@Price_XiaoShou", SqlDbType.Decimal,9),
					new SqlParameter("@synPrice_ShiChang", SqlDbType.Bit,1),
					new SqlParameter("@synPrice_XiaoShou", SqlDbType.Bit,1)};
            parameters[0].Value = TypeID;
            parameters[1].Value = VarietyID;
            parameters[2].Value = VarietyLevelID;
            parameters[3].Value = TimeID;
            parameters[4].Value = StorageFee;
            parameters[5].Value = BankRate;
            parameters[6].Value = CurrentRate;
            parameters[7].Value = EarningRate;
            parameters[8].Value = LossRate;
            parameters[9].Value = Price_ShiChang;
            parameters[10].Value = Price_DaoQi;
            parameters[11].Value = Price_HeTong;
            parameters[12].Value = Price_XiaoShou;
            parameters[13].Value = 0;
            parameters[14].Value = 0;


            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("insert into [StorageRateLog] (");
            strSqlLog.Append("TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,CurrentRate,EarningRate,LossRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou,strType,dtLog)");
            strSqlLog.Append(" values (");
            strSqlLog.Append("@TypeID,@VarietyID,@VarietyLevelID,@TimeID,@StorageFee,@BankRate,@CurrentRate,@EarningRate,@LossRate,@Price_ShiChang,@Price_DaoQi,@Price_HeTong,@Price_XiaoShou,@strType,@dtLog)");
            strSqlLog.Append(";select @@IDENTITY");
            SqlParameter[] parametersLog = {
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@BankRate", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@EarningRate", SqlDbType.Decimal,9),
                    new SqlParameter("@LossRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
					new SqlParameter("@Price_XiaoShou", SqlDbType.Decimal,9),
					new SqlParameter("@strType", SqlDbType.NVarChar,20),
					new SqlParameter("@dtLog", SqlDbType.DateTime,8)};
            parametersLog[0].Value = TypeID;
            parametersLog[1].Value = VarietyID;
            parametersLog[2].Value = VarietyLevelID;
            parametersLog[3].Value = TimeID;
            parametersLog[4].Value = StorageFee;
            parametersLog[5].Value = BankRate;
            parametersLog[6].Value = CurrentRate;
            parametersLog[7].Value = EarningRate;
            parametersLog[8].Value = LossRate;
            parametersLog[9].Value = Price_ShiChang;
            parametersLog[10].Value = Price_DaoQi;
            parametersLog[11].Value = Price_HeTong;
            parametersLog[12].Value = Price_XiaoShou;
            parametersLog[13].Value = "A";//新增记录
            parametersLog[14].Value = DateTime.Now.ToString();



            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                SQLHelper.ExecuteNonQuery(strSqlLog.ToString(), parametersLog);//插入更新记录
                if (chkPrice_ShiChang) { 
                //更新同储户类型同产品的市场价

                    StringBuilder strSqlShiChang = new StringBuilder();
                    strSqlShiChang.Append("  UPDATE dbo.StorageRate SET ");
                    strSqlShiChang.Append("  Price_ShiChang="+Price_ShiChang);
                    strSqlShiChang.Append("  WHERE VarietyLevelID=" + VarietyLevelID + " AND VarietyID=" + VarietyID);
                    SQLHelper.ExecuteNonQuery(strSqlShiChang.ToString());
                }
                if (chkPrice_XiaoShou)
                {
                    //更新同储户类型同产品的销售价

                    StringBuilder strSqlShiChang = new StringBuilder();
                    strSqlShiChang.Append("  UPDATE dbo.StorageRate SET ");
                    strSqlShiChang.Append("  Price_XiaoShou=" + Price_XiaoShou);
                    strSqlShiChang.Append("  WHERE VarietyLevelID=" + VarietyLevelID + " AND VarietyID=" + VarietyID);
                    SQLHelper.ExecuteNonQuery(strSqlShiChang.ToString());
                }
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateStorageRate(HttpContext context)
        {
            string ID = context.Request.QueryString["ID"].ToString();
            //string TypeID = context.Request.Form["TypeID"].ToString();
            //string VarietyID = context.Request.Form["VarietyID"].ToString();
            //string VarietyLevelID = context.Request.Form["VarietyLevelID"].ToString();
            //string TimeID = context.Request.Form["TimeID"].ToString();

            DataTable dtStorageRate = SQLHelper.ExecuteDataTable(" SELECT TOP 1 * FROM  dbo.StorageRate WHERE ID=" + ID);
            string TypeID = dtStorageRate.Rows[0]["TypeID"].ToString();
            string VarietyID = dtStorageRate.Rows[0]["VarietyID"].ToString();
            string VarietyLevelID = dtStorageRate.Rows[0]["VarietyLevelID"].ToString();
            string TimeID = dtStorageRate.Rows[0]["TimeID"].ToString();

            string StorageFee = context.Request.Form["StorageFee"].ToString();
            string CurrentRate = context.Request.Form["CurrentRate"].ToString();
            string BankRate = context.Request.Form["BankRate"].ToString();
            string Price_ShiChang = context.Request.Form["Price_ShiChang"].ToString();
            string Price_DaoQi = context.Request.Form["Price_DaoQi"].ToString();
            string Price_HeTong = context.Request.Form["Price_HeTong"].ToString();
            string Price_XiaoShou = context.Request.Form["Price_XiaoShou"].ToString();
            string EarningRate = context.Request.Form["EarningRate"].ToString();
            string LossRate = context.Request.Form["LossRate"].ToString();
            //bool synPrice_ShiChang = false;
            //if (context.Request.Form["synPrice_ShiChang"] != null)
            //{
            //    synPrice_ShiChang = true;
            //}
            //bool synPrice_XiaoShou = false;
            //if (context.Request.Form["synPrice_XiaoShou"] != null)
            //{
            //    synPrice_XiaoShou = true;
            //}
            bool chkPrice_ShiChang = false;
            if (context.Request.Form["chkPrice_ShiChang"] != null)
            {
                chkPrice_ShiChang = true;
            }
            bool chkPrice_XiaoShou = false;
            if (context.Request.Form["chkPrice_XiaoShou"] != null)
            {
                chkPrice_XiaoShou = true;
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [StorageRate] set ");
            strSql.Append("StorageFee=@StorageFee,");
            strSql.Append("BankRate=@BankRate,");
            strSql.Append("CurrentRate=@CurrentRate,");
            strSql.Append("EarningRate=@EarningRate,");
            strSql.Append("LossRate=@LossRate,");
            strSql.Append("Price_ShiChang=@Price_ShiChang,");
            strSql.Append("Price_DaoQi=@Price_DaoQi,");
            strSql.Append("Price_HeTong=@Price_HeTong,");
            strSql.Append("Price_XiaoShou=@Price_XiaoShou");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@BankRate", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@EarningRate", SqlDbType.Decimal,9),
                    new SqlParameter("@LossRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
					new SqlParameter("@Price_XiaoShou", SqlDbType.Decimal,9),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = StorageFee;
            parameters[1].Value = BankRate;
            parameters[2].Value = CurrentRate;
            parameters[3].Value = EarningRate;
            parameters[4].Value = LossRate;
            parameters[5].Value = Price_ShiChang;
            parameters[6].Value = Price_DaoQi;
            parameters[7].Value = Price_HeTong;
            parameters[8].Value = Price_XiaoShou;
            parameters[9].Value = ID;


            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("insert into [StorageRateLog] (");
            strSqlLog.Append("TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,CurrentRate,EarningRate,LossRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou,strType,dtLog)");
            strSqlLog.Append(" values (");
            strSqlLog.Append("@TypeID,@VarietyID,@VarietyLevelID,@TimeID,@StorageFee,@BankRate,@CurrentRate,@EarningRate,@LossRate,@Price_ShiChang,@Price_DaoQi,@Price_HeTong,@Price_XiaoShou,@strType,@dtLog)");
            strSqlLog.Append(";select @@IDENTITY");
            SqlParameter[] parametersLog = {
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@BankRate", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@EarningRate", SqlDbType.Decimal,9),
                    new SqlParameter("@LossRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
					new SqlParameter("@Price_XiaoShou", SqlDbType.Decimal,9),
					new SqlParameter("@strType", SqlDbType.NVarChar,20),
					new SqlParameter("@dtLog", SqlDbType.DateTime,8)};
            parametersLog[0].Value = TypeID;
            parametersLog[1].Value = VarietyID;
            parametersLog[2].Value = VarietyLevelID;
            parametersLog[3].Value = TimeID;
            parametersLog[4].Value = StorageFee;
            parametersLog[5].Value = BankRate;
            parametersLog[6].Value = CurrentRate;
            parametersLog[7].Value = EarningRate;
            parametersLog[8].Value = LossRate;
            parametersLog[9].Value = Price_ShiChang;
            parametersLog[10].Value = Price_DaoQi;
            parametersLog[11].Value = Price_HeTong;
            parametersLog[12].Value = Price_XiaoShou;
            parametersLog[13].Value = "U";//更新记录
            parametersLog[14].Value = DateTime.Now.ToString();

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                SQLHelper.ExecuteNonQuery(strSqlLog.ToString(), parametersLog);
                if (chkPrice_ShiChang)
                {
                    //更新同储户类型同产品的市场价

                    StringBuilder strSqlShiChang = new StringBuilder();
                    strSqlShiChang.Append("  UPDATE dbo.StorageRate SET ");
                    strSqlShiChang.Append("  Price_ShiChang=" + Price_ShiChang);
                    strSqlShiChang.Append("  WHERE VarietyLevelID=" + VarietyLevelID + " AND VarietyID=" + VarietyID);
                    SQLHelper.ExecuteNonQuery(strSqlShiChang.ToString());
                }
                if (chkPrice_XiaoShou)
                {
                    //更新同储户类型同产品的销售价

                    StringBuilder strSqlShiChang = new StringBuilder();
                    strSqlShiChang.Append("  UPDATE dbo.StorageRate SET ");
                    strSqlShiChang.Append("  Price_XiaoShou=" + Price_XiaoShou);
                    strSqlShiChang.Append("  WHERE VarietyLevelID=" + VarietyLevelID + " AND VarietyID=" + VarietyID);
                    SQLHelper.ExecuteNonQuery(strSqlShiChang.ToString());
                }
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteStorageRateByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            //查询该条目是否已经被使用
            if (SQLHelper.ExecuteScalar("SELECT COUNT(ID) FROM dbo.Dep_StorageInfo WHERE StorageRateID=" + wbid).ToString() != "0")
            {
                context.Response.Write("Exit");
                return;
            }


            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [StorageRate] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = wbid;

            DataRow drStorageRate = SQLHelper.ExecuteDataTable(" SELECT TOP 1 * FROM  dbo.StorageRate WHERE ID=" + wbid).Rows[0];
            StringBuilder strSqlLog = new StringBuilder();
            strSqlLog.Append("insert into [StorageRateLog] (");
            strSqlLog.Append("TypeID,VarietyID,VarietyLevelID,TimeID,StorageFee,BankRate,CurrentRate,EarningRate,LossRate,Price_ShiChang,Price_DaoQi,Price_HeTong,Price_XiaoShou,strType,dtLog)");
            strSqlLog.Append(" values (");
            strSqlLog.Append("@TypeID,@VarietyID,@VarietyLevelID,@TimeID,@StorageFee,@BankRate,@CurrentRate,@EarningRate,@LossRate,@Price_ShiChang,@Price_DaoQi,@Price_HeTong,@Price_XiaoShou,@strType,@dtLog)");
            strSqlLog.Append(";select @@IDENTITY");
            SqlParameter[] parametersLog = {
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@VarietyLevelID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@StorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@BankRate", SqlDbType.Decimal,9),
					new SqlParameter("@CurrentRate", SqlDbType.Decimal,9),
					new SqlParameter("@EarningRate", SqlDbType.Decimal,9),
                    new SqlParameter("@LossRate", SqlDbType.Decimal,9),
					new SqlParameter("@Price_ShiChang", SqlDbType.Decimal,9),
					new SqlParameter("@Price_DaoQi", SqlDbType.Decimal,9),
					new SqlParameter("@Price_HeTong", SqlDbType.Decimal,9),
					new SqlParameter("@Price_XiaoShou", SqlDbType.Decimal,9),
					new SqlParameter("@strType", SqlDbType.NVarChar,20),
					new SqlParameter("@dtLog", SqlDbType.DateTime,8)};
            parametersLog[0].Value = drStorageRate["TypeID"];
            parametersLog[1].Value =drStorageRate["VarietyID"] ;
            parametersLog[2].Value =drStorageRate["VarietyLevelID"] ;
            parametersLog[3].Value =drStorageRate["TimeID"] ;
            parametersLog[4].Value =drStorageRate["StorageFee"] ;
            parametersLog[5].Value =drStorageRate["BankRate"] ;
            parametersLog[6].Value =drStorageRate["CurrentRate"] ;
            parametersLog[7].Value =drStorageRate["EarningRate"] ;
            parametersLog[8].Value =drStorageRate["LossRate"] ;
            parametersLog[9].Value =drStorageRate["Price_ShiChang"] ;
            parametersLog[10].Value =drStorageRate["Price_DaoQi"] ;
            parametersLog[11].Value =drStorageRate["Price_HeTong"] ;
            parametersLog[12].Value = drStorageRate["Price_XiaoShou"];
            parametersLog[13].Value = "D";//删除记录
            parametersLog[14].Value = DateTime.Now.ToString();


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                SQLHelper.ExecuteNonQuery(strSqlLog.ToString(), parametersLog);
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }



        void GetStorageFeeByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,TypeID,TimeID,VarietyID,numStorageFee,numUpper,numLower ");
            strSql.Append(" FROM [StorageFee] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = wbid;
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

        void AddStorageFee(HttpContext context)
        {


            string TypeID = context.Request.Form["TypeID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string TimeID = context.Request.Form["TimeID"].ToString();

            string numStorageFee = context.Request.Form["numStorageFee"].ToString();
            string numUpper = context.Request.Form["numUpper"].ToString();
            string numLower = context.Request.Form["numLower"].ToString();
           

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [StorageFee] (");
            strSql.Append("TypeID,TimeID,VarietyID,numStorageFee,numUpper,numLower)");
            strSql.Append(" values (");
            strSql.Append("@TypeID,@TimeID,@VarietyID,@numStorageFee,@numUpper,@numLower)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@numStorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@numUpper", SqlDbType.Int,4),
					new SqlParameter("@numLower", SqlDbType.Int,4)};
            parameters[0].Value = TypeID;
            parameters[1].Value = TimeID;
            parameters[2].Value = VarietyID;
            parameters[3].Value = numStorageFee;
            parameters[4].Value = numUpper;
            parameters[5].Value = numLower;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void UpdateStorageFee(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();
            string TypeID = context.Request.Form["TypeID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string TimeID = context.Request.Form["TimeID"].ToString();

            string numStorageFee = context.Request.Form["numStorageFee"].ToString();
            string numUpper = context.Request.Form["numUpper"].ToString();
            string numLower = context.Request.Form["numLower"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [StorageFee] set ");
            strSql.Append("TypeID=@TypeID,");
            strSql.Append("TimeID=@TimeID,");
            strSql.Append("VarietyID=@VarietyID,");
            strSql.Append("numStorageFee=@numStorageFee,");
            strSql.Append("numUpper=@numUpper,");
            strSql.Append("numLower=@numLower");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@TypeID", SqlDbType.Int,4),
					new SqlParameter("@TimeID", SqlDbType.Int,4),
					new SqlParameter("@VarietyID", SqlDbType.Int,4),
					new SqlParameter("@numStorageFee", SqlDbType.Decimal,9),
					new SqlParameter("@numUpper", SqlDbType.Int,4),
					new SqlParameter("@numLower", SqlDbType.Int,4),
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = TypeID;
            parameters[1].Value = TimeID;
            parameters[2].Value = VarietyID;
            parameters[3].Value = numStorageFee;
            parameters[4].Value = numUpper;
            parameters[5].Value = numLower;
            parameters[6].Value = wbid;

            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        void DeleteStorageFeeByID(HttpContext context)
        {
            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [StorageFee] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = wbid;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
            {
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        //设置按比例兑换参数
        void UpdateExchangeProp(HttpContext context)
        {


            string TypeID = context.Request.Form["TypeID"].ToString();
            string VarietyID = context.Request.Form["VarietyID"].ToString();
            string TimeID = context.Request.Form["TimeID"].ToString();
            string GoodID = context.Request.Form["GoodID"].ToString();

            string ChuFenLv = context.Request.Form["ChuFenLv"].ToString();
            string FuPi = context.Request.Form["FuPi"].ToString();
            string JiaGongFei = context.Request.Form["JiaGongFei"].ToString();
            bool synGood = false;
            if (context.Request.Form["synGood"] != null)
            {
                synGood = true;
            }
            //查询是否设置了同种商品的兑换比例
            StringBuilder strSqlCount = new StringBuilder();
            strSqlCount.Append("  SELECT COUNT(ID)");
            strSqlCount.Append("  FROM dbo.GoodExchangeProp");
            strSqlCount.Append("  WHERE TypeID="+TypeID+" AND TimeID="+TimeID+" AND VarietyID="+VarietyID+" AND GoodID="+GoodID);
            object obj = SQLHelper.ExecuteScalar(strSqlCount.ToString());

            StringBuilder strSql = new StringBuilder();

            if (obj.ToString() == "0")
            {
        
                strSql.Append("insert into [GoodExchangeProp] (");
                strSql.Append("TypeID,TimeID,VarietyID,GoodID,ChuFenLv,FuPi,JiaGongFei)");
                strSql.Append(" values (");
                strSql.Append( TypeID+","+TimeID+","+VarietyID+","+GoodID+","+ChuFenLv+","+FuPi+","+JiaGongFei+")");
             
            }
            else
            {
                strSql.Append("update [GoodExchangeProp] set ");
                strSql.Append("ChuFenLv="+ChuFenLv+",");
                strSql.Append("FuPi="+FuPi+",");
                strSql.Append("JiaGongFei="+JiaGongFei);
                strSql.Append(" where TypeID="+TypeID+" and TimeID="+TimeID+" and VarietyID="+VarietyID+" and GoodID="+GoodID);
               
            }
           

            if (SQLHelper.ExecuteNonQuery(strSql.ToString()) != 0)
            {
                if (synGood)//选择了同步设置商品 
                {
                    StringBuilder strSqlsyn = new StringBuilder();
                    strSqlsyn.Append("update [GoodExchangeProp] set ");
                    strSqlsyn.Append("ChuFenLv=" + ChuFenLv + ",");
                    strSqlsyn.Append("FuPi=" + FuPi + ",");
                    strSqlsyn.Append("JiaGongFei=" + JiaGongFei);
                    strSqlsyn.Append(" where TypeID=" + TypeID + " and VarietyID=" + VarietyID + " and GoodID=" + GoodID);
                    if (SQLHelper.ExecuteNonQuery(strSqlsyn.ToString()) != 0)
                    {
                        context.Response.Write("OK");
                    }
                    else
                    {
                        context.Response.Write("Error");
                    }
                }
               
            }
            else
            {
                context.Response.Write("Error");
            }
        }

        //删除按比例兑换参数
        void DeleteExchangeProp(HttpContext context)
        {

            string wbid = context.Request.QueryString["ID"].ToString();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [GoodExchangeProp] ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = wbid;


            if (SQLHelper.ExecuteNonQuery(strSql.ToString(), parameters) != 0)
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