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
    public class XCCloudBLL
    {
        public static DataSet ExecuteQuerySentence(string strSQL, SqlParameter[] paramArr)
        {
            return new DataAccess(DataAccessDB.XCCloudDB).ExecuteQuerySentence(strSQL, paramArr);
        }

        public static string ExecuteScalar(string strSQL)
        {
            return new DataAccess(DataAccessDB.XCCloudDB).ExecuteScalar(strSQL);
        }

        public static void ExecuteStoredProcedureSentence(string storedProcedureName, SqlParameter[] paramArr)
        {
            new DataAccess(DataAccessDB.XCCloudDB).ExecuteStoredProcedureSentence(storedProcedureName, paramArr);
        }

        public static System.Data.DataSet GetStoredProcedureSentence(string storedProcedureName, SqlParameter[] paramArr)
        {
            return new DataAccess(DataAccessDB.XCCloudDB).GetStoredProcedureSentence(storedProcedureName, paramArr);
        }

        public static DataTable ExecuterSqlToTable(string strSQL)
        {
            return new DataAccess(DataAccessDB.XCCloudDB).ExecuteQueryReturnTable(strSQL);
        }

        public static int ExecuteSql(string strSQL)
        {
            return new DataAccess(DataAccessDB.XCCloudDB).Execute(strSQL);
        }
    }
}
