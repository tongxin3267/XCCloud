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
    public class XCGameBLL
    {
        public static DataSet ExecuteQuerySentence(string strSQL, string dbName,SqlParameter[] paramArr)
        {
            return new DataAccess(DataAccessDB.XCGameDB).ExecuteQuerySentence(strSQL,dbName, paramArr);
        }

        public static void ExecuteCommandSentence(string strSQL, string dbName, SqlParameter[] paramArr)
        {
            new DataAccess(DataAccessDB.XCGameDB).ExecuteCommandSentence(strSQL, dbName, paramArr);
        }

        public static void ExecuteStoredProcedureSentence(string strSQL, string dbName, SqlParameter[] paramArr)
        {
            new DataAccess(DataAccessDB.XCGameDB).ExecuteStoredProcedureSentence(strSQL, dbName, paramArr);
        }
    }
}
