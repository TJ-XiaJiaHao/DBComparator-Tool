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
                Log.log("databaseLocal connection error - server: " + server + " ; database: " + database);

            }
            finally
            {
            }
            return conn;
        }

        public static SqlConnection createConnectionRemote(string server, string database, string username, string password)
        {
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
                Log.log("databaseRemote connection error - server: " + server + " ; database: " + database + " ; username: " + username + " ; password:" + password);

            }
            finally
            {
            }
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
                Console.WriteLine(e);
            }
            return rtn;
        }
    }
}