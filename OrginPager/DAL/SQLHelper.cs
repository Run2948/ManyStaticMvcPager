using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace OrginPager.DAL
{
    public abstract class SqlHelper
    {
        #region ### Sql数据库连接属性
        // 数据库连接字符串
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ToString();
        // 数据库连接
        private static SqlConnection Sqlconnection;       
        public static SqlConnection SqlConnection
        {
            get
            {
                string SqlconnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnStr"].ToString();
                if (Sqlconnection == null)
                {
                    Sqlconnection = new SqlConnection(SqlconnectionString);
                    Sqlconnection.Open();
                }
                else if (Sqlconnection.State == System.Data.ConnectionState.Closed)
                {
                    Sqlconnection.Open();
                }
                else if (Sqlconnection.State == System.Data.ConnectionState.Broken)
                {
                    Sqlconnection.Close();
                    Sqlconnection.Open();
                }
                return Sqlconnection;
            }
        }
        #endregion

        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = null;
                if (SqlConnection.State == ConnectionState.Closed)
                    return null;
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
        }

        public static object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalar(string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                cmd.Connection = conn;
                cmd.CommandText = cmdText;

                if (trans != null)
                    cmd.Transaction = trans;

                cmd.CommandType = cmdType;

                if (cmdParms != null)
                {
                    foreach (SqlParameter parm in cmdParms)
                        cmd.Parameters.Add(parm);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// SqlGetDataTable [ 执行查询-返DataTable]
        /// </summary>
        /// <param name="proc">存储过程名称</param>
        /// <param name="type">存储过程类型</param>
        /// <param name="paramValue">参数值-[字符串数组]{详细参数请看P_AspNetPage存储过程里面的注释}</param>
        /// <param name="OutTotalCount">out总记录数</param>
        /// <returns>DataTable</returns>
        public static DataTable SqlGetDataTable(string proc, CommandType type, string[] paramValue, out int OutTotalCount)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(proc, conn))
                {
                    //分页开始 
                    SqlParameter[] myParms = new SqlParameter[11];

                    myParms[0] = new SqlParameter("@TableName", SqlDbType.VarChar, 50);
                    myParms[0].Value = paramValue[0];

                    myParms[1] = new SqlParameter("@FieldList", SqlDbType.VarChar, 50);
                    myParms[1].Value = paramValue[1];

                    myParms[2] = new SqlParameter("@PrimaryKey", SqlDbType.VarChar, 50);
                    myParms[2].Value = paramValue[2];

                    myParms[3] = new SqlParameter("@Where", SqlDbType.VarChar, 500);
                    myParms[3].Value = paramValue[3];

                    myParms[4] = new SqlParameter("@Order", SqlDbType.VarChar, 50);
                    myParms[4].Value = paramValue[4];

                    myParms[5] = new SqlParameter("@SortType", SqlDbType.Int, 4);
                    myParms[5].Value = paramValue[5];

                    myParms[6] = new SqlParameter("@RecorderCount", SqlDbType.Int, 4);
                    myParms[6].Value = paramValue[6];

                    myParms[7] = new SqlParameter("@PageSize", SqlDbType.Int, 4);
                    myParms[7].Value = paramValue[7];

                    myParms[8] = new SqlParameter("@PageIndex", SqlDbType.Int, 4);
                    myParms[8].Value = paramValue[8];

                    myParms[9] = new SqlParameter("@TotalCount", SqlDbType.Int, 4);
                    myParms[9].Direction = ParameterDirection.Output;

                    myParms[10] = new SqlParameter("@TotalPageCount", SqlDbType.Int, 4);
                    myParms[10].Direction = ParameterDirection.Output;
                    foreach (SqlParameter parameter in myParms)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.CommandType = type;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                        //out 总记录数
                        OutTotalCount = Convert.ToInt32(myParms[9].Value);
                        return ds.Tables[0];
                    }
                }
            }
        }

        public static DataTable SqlGetDataTable(string proc, string[] paramValue, out int OutTotalCount)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(proc, conn))
                {
                    //分页开始 
                    SqlParameter[] myParms = new SqlParameter[11];

                    myParms[0] = new SqlParameter("@TableName", SqlDbType.VarChar, 50);
                    myParms[0].Value = paramValue[0];

                    myParms[1] = new SqlParameter("@FieldList", SqlDbType.VarChar, 50);
                    myParms[1].Value = paramValue[1];

                    myParms[2] = new SqlParameter("@PrimaryKey", SqlDbType.VarChar, 50);
                    myParms[2].Value = paramValue[2];

                    myParms[3] = new SqlParameter("@Where", SqlDbType.VarChar, 500);
                    myParms[3].Value = paramValue[3];

                    myParms[4] = new SqlParameter("@Order", SqlDbType.VarChar, 50);
                    myParms[4].Value = paramValue[4];

                    myParms[5] = new SqlParameter("@SortType", SqlDbType.Int, 4);
                    myParms[5].Value = paramValue[5];

                    myParms[6] = new SqlParameter("@RecorderCount", SqlDbType.Int, 4);
                    myParms[6].Value = paramValue[6];

                    myParms[7] = new SqlParameter("@PageSize", SqlDbType.Int, 4);
                    myParms[7].Value = paramValue[7];

                    myParms[8] = new SqlParameter("@PageIndex", SqlDbType.Int, 4);
                    myParms[8].Value = paramValue[8];

                    myParms[9] = new SqlParameter("@TotalCount", SqlDbType.Int, 4);
                    myParms[9].Direction = ParameterDirection.Output;

                    myParms[10] = new SqlParameter("@TotalPageCount", SqlDbType.Int, 4);
                    myParms[10].Direction = ParameterDirection.Output;
                    foreach (SqlParameter parameter in myParms)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                        //out 总记录数
                        OutTotalCount = Convert.ToInt32(myParms[9].Value);
                        return ds.Tables[0];
                    }
                }
            }
        }
    }
}