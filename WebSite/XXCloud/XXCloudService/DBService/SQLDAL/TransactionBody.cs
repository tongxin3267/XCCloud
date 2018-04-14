using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace XCCloudService.DBService.SQLDAL
{
    public class TransactionBody
    {
        public TransactionBody()
        {

        }
        public TransactionBody(string sql)
        {
            this.sSQLText = sql;
        }
        private string sSQLText;

        public string SSQLText
        {
            get { return sSQLText; }
            set { sSQLText = value; }
        }

        private SqlParameter[] parameters = null;
        public SqlParameter[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        private DataTable dsTransaction;
        public DataTable DsTransaction
        {
            get { return dsTransaction; }
            set { dsTransaction = value; }
        }
    }
}