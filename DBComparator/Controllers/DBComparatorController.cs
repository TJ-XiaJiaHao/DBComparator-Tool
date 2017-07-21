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
        private log4net.ILog logger = Log.getLogger(typeof(DBComparatorController));

        /// <summary>
        /// GET
        /// /api/DBComparator
        /// </summary>
        /// <param name="server1"></param>
        /// <param name="dbname1"></param>
        /// <param name="username1"></param>
        /// <param name="password1"></param>
        /// <param name="server2"></param>
        /// <param name="dbname2"></param>
        /// <returns></returns>
        [HttpGet]
        public TableDiff compareRemoteAndLocal(string server1, string dbname1, string username1, string password1, string server2, string dbname2)
        {
            // Logger
            logger.Info("[ compareRemoteAndLocal ] - database1 : " + server1 + " , " + dbname1 + " , " + username1 + " , " + password1 + " ; database2 : " + server2 + " , " + dbname2 +  ";");
            DateTime start = DateTime.Now;
            logger.Info("[ compareRemoteAndLocal ] - start time : " + start.ToString());

            // Init the basic info about response body
            TableDiff rtn = new TableDiff();
            rtn.code = ResponseCode.SUCCESS;
            rtn.msg = "Succecssful";

            if (server1 == null || dbname1 == null || username1 == null || password1 == null || server2 == null || dbname2 == null || 
                server1.Trim() == "" || dbname1.Trim() == "" || username1.Trim() == "" || password1.Trim() == "" || server2.Trim() == "" || dbname2.Trim() == "" )
            {
                rtn.code = ResponseCode.INPUT_ERROR;
                rtn.msg = "Null inputs are not be allowed!";
                return rtn;
            }

            // Connect to database
            SqlConnection conn1 = SqlServer.createConnectionRemote(server1, dbname1, username1, password1);
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

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ compareRemoteAndLocal ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");
            return rtn;
        }

        /// <summary>
        /// GET
        /// /api/DBComparator
        /// eg: http://localhost:16936/api/Dbcomparator?server1=SHALL778&&dbname1=Student1&username1=ttt&password1=ggg&Server2=SHALL778&dbname2=Student2&username2=ttt&password2=ggg
        /// </summary>
        /// <param name="server1"></param>
        /// <param name="dbname1"></param>
        /// <param name="username1"></param>
        /// <param name="password1"></param>
        /// <param name="server2"></param>
        /// <param name="dbname2"></param>
        /// <param name="username2"></param>
        /// <param name="password2"></param>
        /// <returns></returns>
        [HttpGet]
        public TableDiff compareRemote(string server1, string dbname1, string username1, string password1, string server2, string dbname2,string username2, string password2)
        {
            // Logger
            logger.Info("[ compareRemote ] - database1 : " + server1 + " , " + dbname1 + " , " + username1 + " , " + password1 + " ; database2 : " + server2 + " , " + dbname2 + " , " + username2 + " , " + password2 + ";");
            DateTime start = DateTime.Now;
            logger.Info("[ compareRemote ] - start time : " + start.ToString());


            // Init the basic info about response body
            TableDiff rtn = new TableDiff();
            rtn.code = ResponseCode.SUCCESS;
            rtn.msg = "Succecssful";

            if (server1 == null || dbname1 == null || username1 == null || password1 == null || 
                server2 == null || dbname2 == null || username2 == null || password2 == null || 
                server1.Trim() == "" || dbname1.Trim() == "" || username1.Trim() == "" || password1.Trim() == "" || 
                server2.Trim() == "" || dbname2.Trim() == "" || username2.Trim() == "" || password2.Trim() == "")
            {
                rtn.code = ResponseCode.INPUT_ERROR;
                rtn.msg = "Null inputs are not be allowed!";
                return rtn;
            }

            // Connect to database
            SqlConnection conn1 = SqlServer.createConnectionRemote(server1, dbname1,username1,password1);
            SqlConnection conn2 = SqlServer.createConnectionRemote(server2, dbname2,username2,password2);
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

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ compareRemote ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");
            return rtn;
        }

        /// <summary>
        /// GET
        /// /api/DBComparator
        /// eg: http://localhost:16936/api/Dbcomparator?server1=SHALL778&&dbname1=Student1&Server2=SHALL778&dbname2=Student2
        /// </summary>
        /// <param name="server1"></param>
        /// <param name="dbname1"></param>
        /// <param name="server2"></param>
        /// <param name="dbname2"></param>
        /// <returns></returns>
        [HttpGet]
        public TableDiff compareLocal(string server1, string dbname1, string server2, string dbname2)
        {
            // Logger
            logger.Info("[ compareLocal ] - database1 : " + server1 + " , " + dbname1 + " ; database2 : " + server2 + " , " + dbname2);
            DateTime start = DateTime.Now;
            logger.Info("[ compareLocal ] - start time : " + start.ToString());

            // Init the basic info about response body
            TableDiff rtn = new TableDiff();
            rtn.code = ResponseCode.SUCCESS;
            rtn.msg = "Succecssful";

            if (server1 == null || dbname1 == null || server2 == null || dbname2 == null || server1.Trim() == "" || server2.Trim() == "" || dbname1.Trim() == "" || dbname2.Trim() == "")
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

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ compareLocal ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");
            return rtn;
        }



       
    }
}
