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
    public class XCGameManaLogBLL
    {
        public static DataSet ExecuteQuerySentence(string strSQL, SqlParameter[] paramArr)
        {
            return new DataAccess(DataAccessDB.XCGameManaLogDB).ExecuteQuerySentence(strSQL, paramArr);
        }

        public static void ExecuteStoredProcedureSentence(string storedProcedureName, SqlParameter[] paramArr)
        {
            new DataAccess(DataAccessDB.XCGameManaLogDB).ExecuteStoredProcedureSentence(storedProcedureName, paramArr);
        }
    }
}
