using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using DBComparator.Server;
using DBComparator.Models;

namespace DBComparator.Controllers
{
    public class DBComparatorController : ApiController
    {
        /// <summary>
        /// GET
        /// /api/DBComparator
        /// </summary>
        /// <param name="server1"></param>
        /// <param name="dbname1"></param>
        /// <param name="server2"></param>
        /// <param name="dbname2"></param>
        /// <returns></returns>
        [HttpGet]
        public TableDiff compareLocal(string server1, string dbname1, string server2, string dbname2)
        {

            // Init the basic info about response body
            TableDiff rtn = new TableDiff();
            rtn.code = ResponseCode.SUCCESS;
            rtn.msg = "Succecssful";

            if (server1 == null || dbname1 == null || server2 == null || dbname2 == null || server1.Trim() == "" || server2.Trim() == "" || dbname1.Trim() == "" || dbname2 == "")
            {
                rtn.code = ResponseCode.INPUT_ERROR;
                rtn.msg = "Null inputs are not be allowed!";
                return rtn;
            }

            // Connect to database
            SqlConnection conn1 = SqlServer.createConnectionLocal(server1, dbname1);
            SqlConnection conn2 = SqlServer.createConnectionLocal(server2, dbname2);
            if (conn1 == null || conn2 == null)
            {
                rtn.code = ResponseCode.DB_NOT_FUND;
                rtn.msg = "Database not found";
                // close the database connection
                if (conn1 != null) conn1.Close();
                if (conn2 != null) conn2.Close();

                return rtn;
            }

            // Compare the tables between two databases
            TableComparator tbCompare = new TableComparator();
            rtn.tables = tbCompare.compareTables(conn1, conn2);

            // Compare the stored procedures between two databases
            StoredProcedureComparator pc = new StoredProcedureComparator();
            rtn.storedProcedures = pc.compareStoredProcedure(conn1, conn2);

            // Compare the functions between two databases
            FunctionComparator fc = new FunctionComparator();
            rtn.functions = fc.compareFunctions(conn1, conn2);

            // close the database connection
            conn1.Close();
            conn2.Close();

            return rtn;
        }



       
    }
}
