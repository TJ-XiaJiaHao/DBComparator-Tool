using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DBComparator.Models;

namespace DBComparator.Server
{
    public class FunctionComparator
    {
        private log4net.ILog logger = Log.getLogger(typeof(FunctionComparator));

        public List<FunctionDiff> compareFunctions(SqlConnection conn1, SqlConnection conn2)
        {
            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ compareFunctions ] - start time : " + start.ToString());

            List<Function> functions1 = getFunctions(conn1);
            List<Function> functions2 = getFunctions(conn2);
            List<FunctionDiff> funDiff = getDiffFunctions(functions1, functions2, conn1.Database, conn2.Database);

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ compareFunctions ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return funDiff;
        }

        private List<FunctionDiff> getDiffFunctions(List<Function> functions1, List<Function> functions2, string dbname1, string dbname2)
        {
            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ getDiffFunctions ] - start time : " + start.ToString());

            List<FunctionDiff> rtn = new List<FunctionDiff>();

            functions1.Sort((a, b) => a.name.CompareTo(b.name));
            functions2.Sort((a, b) => a.name.CompareTo(b.name));

            int pos1 = 0, pos2 = 0;
            while (pos1 < functions1.Count() && pos2 < functions2.Count())
            {
                Function function1 = functions1[pos1];
                Function function2 = functions2[pos2];

                if (function1.name == function2.name)
                {
                    if (!function1.compare(function2))
                    {
                        FunctionDiff funDiff = new FunctionDiff(function1.name, true);
                        funDiff.different.Add(function1);
                        funDiff.different.Add(function2);
                        rtn.Add(funDiff);
                    }
                    pos1++;
                    pos2++;
                }
                else if (function1.name.CompareTo(function2.name) < 0)
                {
                    FunctionDiff funDiff = new FunctionDiff(function1.name, false);
                    funDiff.different.Add(function1);
                    funDiff.different.Add(new Function(dbname2, function1.name, "", false));
                    rtn.Add(funDiff);
                    pos1++;

                }
                else if (function1.name.CompareTo(function2.name) > 0)
                {
                    FunctionDiff funDiff = new FunctionDiff(function2.name, false);
                    funDiff.different.Add(new Function(dbname1, function2.name, "", false));
                    funDiff.different.Add(function2);
                    rtn.Add(funDiff);
                    pos2++;
                }
            }
            while (pos1 < functions1.Count())
            {
                Function function1 = functions1[pos1];
                FunctionDiff funDiff = new FunctionDiff(function1.name, false);
                funDiff.different.Add(function1);
                funDiff.different.Add(new Function(dbname2, function1.name, "", false));
                rtn.Add(funDiff);
                pos1++;
            }
            while (pos2 < functions2.Count())
            {
                Function function2 = functions2[pos2];
                FunctionDiff funDiff = new FunctionDiff(function2.name, false);
                funDiff.different.Add(new Function(dbname1, function2.name, "", false));
                funDiff.different.Add(function2);
                rtn.Add(funDiff);
                pos2++;
            }

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ getDiffFunctions ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return rtn;
        }

        private List<Function> getFunctions(SqlConnection connection)
        {
            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ getFunctions ] - start time : " + start.ToString());

            List<Function> rtn = new List<Function>();

            // get function names
            List<DBFunctions> dbfunctions = new List<DBFunctions>();
            string command = "select name,text from sys.objects  a join sys.syscomments b on a.object_id = b.id where type='FN'";
            SqlDataReader dr = SqlServer.ExcuteSqlCommandReader(command, connection);
            while (dr.Read())
            {
                List<DBFunctions> befireFunction = dbfunctions.Where(a => a.name == dr[0].ToString()).ToList();
                if (befireFunction.Count() > 0)
                {
                    befireFunction[0].statement += dr[1].ToString();
                }
                else dbfunctions.Add(new DBFunctions() { name = dr[0].ToString(), statement = dr[1].ToString() });
            }
            foreach (DBFunctions dbfunction in dbfunctions)
            {
                rtn.Add(new Function(connection.Database, dbfunction.name, dbfunction.statement, true));
            }


            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ getFunctions ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return rtn;
        }
    }
}