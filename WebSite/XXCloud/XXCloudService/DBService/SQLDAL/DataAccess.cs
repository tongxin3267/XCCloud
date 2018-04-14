using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.IO;
using System.Text;

namespace XCCloudService.DBService.SQLDAL
{
    public class DBErrorLog
    {
        public static void SaveLog(Exception exp)
        {
            string strErrMsg = exp.Message + "\n";
            if (exp.Source != null)
            {
                strErrMsg = strErrMsg + "Source:" + exp.Source.ToString() + "\n";
            }
            if (exp.StackTrace != null)
            {
                strErrMsg = strErrMsg + "StackTrace:" + exp.StackTrace;
            }
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
            SaveLogFile(s + strErrMsg + "\n");
        }

        public static void SaveLog(string strMsg)
        {
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
            SaveLogFile(s + strMsg + "\n");
        }

        private static void SaveLogFile(string strErrMsg)
        {
            string logRootDirectory = HttpContext.Current.Server.MapPath(".");
            if (!Directory.Exists(logRootDirectory + "\\log\\"))
            {
                Directory.CreateDirectory(logRootDirectory + "\\log\\");
            }

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
            FileInfo inf = new FileInfo(logRootDirectory + "\\log\\" + fileName);
            StreamWriter wri = new StreamWriter(logRootDirectory + "\\log\\" + fileName, true, Encoding.UTF8, 1024);
            wri.WriteLine(strErrMsg);
            wri.Close();
        }
    }

    public enum DataAccessDB
    { 
        XCGameDB = 0,
        XCCloudDB = 1
    }

    public class DataAccess
    {
        private string sqlConnString = string.Empty;

        public DataAccess(DataAccessDB dataAccessDB)
        {
            if (dataAccessDB == DataAccessDB.XCGameDB)
            {
                sqlConnString = System.Configuration.ConfigurationManager.ConnectionStrings["XCGameDB"].ToString();
            }
            else if (dataAccessDB == DataAccessDB.XCCloudDB)
            {
                sqlConnString = System.Configuration.ConfigurationManager.ConnectionStrings["XCCloudDB"].ToString();
            }
        }

      //string connString = GetConnectionString();
      //   string sqlConnString;
        
        ///// <summary>
        ///// 获取数据库连接字符串
        ///// </summary>
        ///// <returns></returns>
        //public static string GetConnectionString()
        //{
        //    string connString = "";
        //    try
        //    {
        //        if (connString.Equals(""))
        //        {
        //            sqlConnString = System.Configuration.ConfigurationManager.ConnectionStrings["XCCloudDB"].ToString();
        //        }
        //    }
        //    catch
        //    {
        //        DBErrorLog.SaveLog("数据库连接错误：\r\n" + connString + "\r\n");
        //        throw;
        //    }
        //    return sqlConnString;
        //}
        /// <summary>
        /// 数据库查询
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string strSQL)
        {
            SqlConnection conn = new SqlConnection(sqlConnString);

            try
            {
                conn.Open();
            }
            catch
            {
                DBErrorLog.SaveLog("数据库打开错误：\r\n" + sqlConnString + "\r\n");
                throw;
            }
            SqlTransaction trans = conn.BeginTransaction();
            if (strSQL.Equals("")) return null;
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand(strSQL, conn, trans);
                da.SelectCommand.CommandTimeout = 120;
                da.Fill(ds);
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                DBErrorLog.SaveLog("数据库执行错误：\r\n" + strSQL + "\r\n");
                throw;
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }
        /// <summary>
        /// 数据库查询
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns></returns>
        public DataTable ExecuteQueryReturnTable(string strSQL)
        {
            SqlConnection conn = new SqlConnection(sqlConnString);
            try
            {
                conn.Open();
            }
            catch
            {
                DBErrorLog.SaveLog("数据库打开错误：\r\n" + sqlConnString + "\r\n");
                throw;
            }
            SqlTransaction trans = conn.BeginTransaction();
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand(strSQL, conn, trans);
                da.SelectCommand.CommandTimeout = 120;
                da.Fill(dt);
                trans.Commit();
            }
            catch
            {
                DBErrorLog.SaveLog("数据库执行错误：\r\n" + strSQL + "\r\n");
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }
        /// <summary>
        /// 单条数据操作数据库(增、删、改)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="ds">Dataset</param>
        /// <returns></returns>
        public int ExecuteDataSet(string strSQL, DataSet ds)
        {
            int i = 0;
            SqlConnection conn = new SqlConnection(sqlConnString);
            try
            {
                conn.Open();
            }
            catch
            {
                throw;
            }
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand(strSQL, conn, trans);
                da.InsertCommand = new SqlCommand("", conn, trans);
                da.UpdateCommand = new SqlCommand("", conn, trans);
                da.DeleteCommand = new SqlCommand("", conn, trans);
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.InsertCommand = cb.GetInsertCommand();
                da.UpdateCommand = cb.GetUpdateCommand();
                da.DeleteCommand = cb.GetDeleteCommand();

                da.Update(ds);
                trans.Commit();
                i = 1;
            }
            catch
            {
                DBErrorLog.SaveLog("数据库执行错误：\r\n" + strSQL + "\r\n");
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return i;
        }

        /// <summary>
        /// 执行一条非查询语句,返回受影响的行数
        /// </summary>
        public int Execute(string sql)
        {
            int i = 0;
            SqlConnection conn = new SqlConnection(sqlConnString);
            try
            {
                conn.Open();
            }
            catch
            {
                DBErrorLog.SaveLog("数据库打开错误：\r\n" + sqlConnString + "\r\n");
                throw;
            }
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = 120;
                i = cmd.ExecuteNonQuery();
            }
            catch
            {
                DBErrorLog.SaveLog("数据库执行错误：\r\n" + sql + "\r\n");
                throw;
            }
            finally
            {
                conn.Close();
            }
            return i;
        }

        /// <summary>
        /// 带条件数据库查询
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <param name="model">条件参数</param>
        /// <returns></returns>
        public DataSet ExecuteQuerySentence(string strSQL, SqlParameter[] paramArr)
        {
            SqlConnection conn = new SqlConnection(sqlConnString);
            try
            {
                conn.Open();
            }
            catch
            {
                throw;
            }
            SqlTransaction trans = conn.BeginTransaction();

            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand(strSQL, conn, trans);
                da.SelectCommand.CommandTimeout = 120;
                if (paramArr != null && paramArr.Length > 0)
                {
                    foreach (SqlParameter param in paramArr)
                    {
                        da.SelectCommand.Parameters.Add(param);
                    }
                }
                da.Fill(ds);
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }
        /// <summary>
        /// 带条件修改数据库
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="model">条件参数</param>
        /// <returns></returns>
        public int ExecuteModifyWhere(string strSQL, SqlParameter[] paramArr)
        {
            int s = 0;
            SqlConnection conn = new SqlConnection(sqlConnString);
            try
            {
                conn.Open();
            }
            catch
            {
                throw;
            }
            SqlTransaction trans = conn.BeginTransaction();
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, conn, trans);
                if (paramArr != null && paramArr.Length > 0)
                {
                    foreach (SqlParameter param in paramArr)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                cmd.ExecuteNonQuery();
                trans.Commit();
                s = 1;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return s;
        }
        /// <summary>
        /// 多事物操作数据库(增、删、改)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="ds">Dataset</param>
        /// <returns></returns>
        public int ExecuteDataSetTransaction(List<TransactionBody> Arr)
        {
            int i = 0;
            SqlConnection conn = new SqlConnection(sqlConnString);
            try
            {
                conn.Open();
            }
            catch
            {
                throw;
            }
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                for (int j = 0; j < Arr.Count; j++)
                {
                    SqlDataAdapter da = new SqlDataAdapter(Arr[j].SSQLText, conn);
                    da.SelectCommand = new SqlCommand(Arr[j].SSQLText, conn, trans);
                    da.InsertCommand = new SqlCommand("", conn, trans);
                    da.UpdateCommand = new SqlCommand("", conn, trans);
                    da.DeleteCommand = new SqlCommand("", conn, trans);
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    da.InsertCommand = cb.GetInsertCommand();
                    da.UpdateCommand = cb.GetUpdateCommand();
                    da.DeleteCommand = cb.GetDeleteCommand();

                    da.Update(Arr[j].DsTransaction);
                }
                trans.Commit();
                i = 1;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return i;
        }
        /// <summary>
        /// 多事务处理
        /// </summary>
        /// <returns></returns>
        public bool multiTransation(List<TransactionBody> Arr)
        {
            SqlConnection conn = new SqlConnection(sqlConnString);
            string sql = "";
            try
            {
                conn.Open();
            }
            catch
            {
                throw;
            }
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                for (int i = 0; i < Arr.Count; i++)
                {
                    sql += "执行语句：" + ((TransactionBody)Arr[i]).SSQLText;
                    SqlCommand cmd = new SqlCommand(((TransactionBody)Arr[i]).SSQLText, conn, trans);
                    cmd.CommandTimeout = 120;
                    if (((TransactionBody)Arr[i]).Parameters != null && ((TransactionBody)Arr[i]).Parameters.Length > 0)
                    {
                        foreach (SqlParameter p in ((TransactionBody)Arr[i]).Parameters)
                        {
                            cmd.Parameters.Add(p);
                            sql += "|参数：" + p.Value;
                        }
                    }
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch
            {
                DBErrorLog.SaveLog("数据库执行错误：\r\n" + sql + "\r\n");
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return true;
        }

        public int InsertDataByObject(object obj, string tabName)
        {
            string fields = "";
            string values = "";

            Type t = obj.GetType();
            foreach (PropertyInfo p in t.GetProperties())
            {
                fields += p.Name + ",";
                if (p.Name.ToLower() == "realtime" || p.Name.ToLower() == "printtime")
                    values += "" + p.GetValue(obj, null).ToString() + ",";
                else
                    values += "'" + p.GetValue(obj, null).ToString() + "',";
            }
            fields = fields.Substring(0, fields.Length - 1);
            values = values.Substring(0, values.Length - 1);
            string sql = string.Format("Insert into {0} ({1}) values ({2})", tabName, fields, values);
            if (Execute(sql) > 0)
            {
                DataTable dt = ExecuteQueryReturnTable("select max(id) from " + tabName);
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0].ToString());
                }
            }
            return -1;
        }

        public bool UpdateDataByObject(object obj, string tabName, string key)
        {
            string fields = "";
            string value = "";

            Type t = obj.GetType();
            foreach (PropertyInfo p in t.GetProperties())
            {
                value = "'" + p.GetValue(obj, null).ToString() + "',";
                fields += p.Name + "='" + value + "',";
            }
            fields = fields.Substring(0, fields.Length - 1);
            string sql = string.Format("update {0} set {1} where id='{2}'", tabName, fields, key);
            return (Execute(sql) > 0);
        }

        public bool DeleteDataByObject(string tabName, string key)
        {
            string sql = string.Format("delete from {0} where id='{1}'", tabName, key);
            return (Execute(sql) > 0);
        }

        //public List<object> GetDataList(object obj, string tabName, string where)
        //{
        //    List<object> list = new List<object>();
        //    string sql = string.Format("select * from {0} where {1}", tabName, where);
        //    DataTable dt = ExecuteQueryReturnTable(sql);
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        Type t = obj.GetType();
        //        foreach (PropertyInfo p in t.GetProperties())
        //        {
        //            p.SetValue(obj, row[p.Name].ToString(), null);
        //        }

        //    }
        //}
    }
}