using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Web
{
    //取数据表信息
    public class commondb
    {
        public static DataRow getStorageTimeByID(string id)
        {
            string sql = "SELECT * FROM dbo.StorageTime WHERE ID=@ID";
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = id;

            DataTable dt = SQLHelper.ExecuteDataTable(sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }
        }

        public static DataRow getStorageRateByID(string id) {
            string sql = "SELECT * FROM dbo.StorageRate WHERE ID=@ID";
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = id;

            DataTable dt= SQLHelper.ExecuteDataTable(sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else {
                return dt.Rows[0];
            }
        }

        public static DataRow getDep_StorageInfoByID(string id)
        {
            string sql = "SELECT * FROM dbo.Dep_StorageInfo WHERE ID=@ID";
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = id;

            DataTable dt = SQLHelper.ExecuteDataTable(sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }
        }

        public static DataRow getGoodExchangeGroupByID(string id)
        {
            string sql = "SELECT * FROM dbo.GoodExchangeGroup WHERE ID=@ID";
            SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.Int,4)};
            parameters[0].Value = id;

            DataTable dt = SQLHelper.ExecuteDataTable(sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0];
            }
        }
    }
}