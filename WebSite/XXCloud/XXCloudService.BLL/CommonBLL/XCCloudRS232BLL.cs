using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.DAL.CommonDAL;

namespace XCCloudService.BLL.CommonBLL
{
    public class XCCloudRS232BLL
    {
        public static DataSet ExecuteQuerySentence(string strSQL, SqlParameter[] paramArr)
        {
            return new DataAccess(DataAccessDB.XCCloudRS232DB).ExecuteQuerySentence(strSQL, paramArr);
        }

        public static int ExecuteSql(string strSQL)
        {
            return new DataAccess(DataAccessDB.XCCloudRS232DB).Execute(strSQL);
        }

        public static int ExecuteSql(string strSQL, SqlParameter[] paramArr)
        {
            return new DataAccess(DataAccessDB.XCCloudRS232DB).Execute(strSQL, paramArr);
        }

        /// <summary>
        /// 查询sql语句，返回DataTable
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public static DataTable ExecuterSqlToTable(string strSQL)
        {
            return new DataAccess(DataAccessDB.XCCloudRS232DB).ExecuteQueryReturnTable(strSQL);
        }

        /// <summary>
        /// 查询sql语句，返回DataSet
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public static DataSet ExecuterSqlToDataSet(string strSQL)
        {
            return new DataAccess(DataAccessDB.XCCloudRS232DB).ExecuteQuery(strSQL);
        }
    }
}
