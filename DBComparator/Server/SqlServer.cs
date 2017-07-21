using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace DBComparator.Server
{
    public class SqlServer
    {

        /// <summary>
        /// Create a connection to sql server with window identify
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static SqlConnection createConnectionLocal(string server, string database)
        {
            // Logger
            log4net.ILog logger = Log.getLogger(typeof(SqlServer));
            logger.Info("[ createConnectionLocal ] - database : " + server + " , " + database);
            DateTime start = DateTime.Now;
            logger.Info("[ createConnectionLocal ] - start time : " + start.ToString());

            if (server == "" || database == "") return null;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "server=" + server + ";database=" + database + ";Trusted_Connection=SSPI";
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                conn = null;
                logger.Error("[ createConnectionLocal ] - " + e);
            }
            finally
            {
            }

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ createConnectionLocal ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");
            return conn;
        }

        public static SqlConnection createConnectionRemote(string server, string database, string username, string password)
        {
            // Logger
            log4net.ILog logger = Log.getLogger(typeof(SqlServer));
            logger.Info("[ createConnectionRemote ] - database : " + server + " , " + database + " , " + username + " , " + password);
            DateTime start = DateTime.Now;
            logger.Info("[ createConnectionRemote ] - start time : " + start.ToString());

            if (server == "" || database == "" || username == "" || password == "") return null;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "server=" + server + ";database=" + database + ";User Id=" + username + ";Password=" + password + ";";
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                conn = null;
                logger.Error("[ createConnectionRemote ] - " + e);
            }
            finally
            {
            }

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ createConnectionRemote ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");
            return conn;
        }

        /// <summary>
        /// Excute sql command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static SqlDataReader ExcuteSqlCommandReader(string command, SqlConnection connection)
        {
            // Logger
            log4net.ILog logger = Log.getLogger(typeof(SqlServer));
            //logger.Info("[ ExcuteSqlCommandReader ] - command : " + command);
            //DateTime start = DateTime.Now;
            //logger.Info("[ ExcuteSqlCommandReader ] - start time : " + start.ToString());

            connection.Close();
            connection.Open();
            SqlCommand cmd = new SqlCommand(command, connection);
            SqlDataReader rtn = null;
            try
            {
                rtn = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                rtn = null;
                logger.Error("[ ExcuteSqlCommandReader ] - " + e);
            }

            // Logger
            //DateTime end = DateTime.Now;
            //logger.Info("[ ExcuteSqlCommandReader ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return rtn;
        }
    }
}