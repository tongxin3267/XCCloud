using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

namespace CloudRS232Server
{
    public class DataAccess
    {
        string GetConnectString()
        {
            //XmlDocument xmldoc = XMLCrypto.FileDecrypt("mysql.xml", 7);
            //string connString = string.Format("Data Source = {0};Initial Catalog = {1};User Id = {2};Password = {3};Connection Timeout=5;",
            //        xmldoc.DocumentElement["server"].InnerText,
            //        xmldoc.DocumentElement["database"].InnerText,
            //        xmldoc.DocumentElement["uid"].InnerText,
            //        xmldoc.DocumentElement["pwd"].InnerText);
            return "Data Source = 192.168.1.119;Initial Catalog = XCCloudRS232;User Id = sa;Password = xinchen;Connection Timeout=5;";
            //return "Data Source = .;Initial Catalog = XCCloudRS232;User Id = sa;Password = hxyw801102;Connection Timeout=5;";
        }

        /// <summary>
        /// 数据库查询
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns></returns>
        public DataTable ExecuteQueryReturnTable(string strSQL)
        {
            strSQL = strSQL.Replace("`", "");
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectString()))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(strSQL, conn))
                    {
                        da.Fill(dt);
                    }
                    conn.Close();
                    conn.Dispose();
                    //GC.Collect();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteDBLog(strSQL, ex);
            }
            return dt;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>影响条数</returns>
        public int Execute(string strSQL)
        {
            strSQL = strSQL.Replace("`", "");
            int count = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(strSQL, conn))
                    {
                        count = cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    conn.Dispose();
                    //GC.Collect();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteDBLog(strSQL, ex);
            }
            return count;
        }
    }
}
