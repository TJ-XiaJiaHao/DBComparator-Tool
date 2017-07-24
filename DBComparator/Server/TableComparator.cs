using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using DBComparator.Server;
using DBComparator.Models;

namespace DBComparator.Server
{
    public class TableComparator
    {
        private log4net.ILog logger = Log.getLogger(typeof(TableComparator));

        /// <summary>
        /// Compare all the table information between two databases
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <returns></returns>
        public List<Table> compareTables(SqlConnection conn1, SqlConnection conn2)
        {

            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ compareTables ] - start time : " + start.ToString());

            List<Table> rtnTables = new List<Table>();

            // Excute the sql command to get all the table names in a database
            string command = "SELECT SysObjects.name AS Tablename FROM sysobjects WHERE xtype = 'U'";
            SqlDataReader dr1 = SqlServer.ExcuteSqlCommandReader(command, conn1);
            SqlDataReader dr2 = SqlServer.ExcuteSqlCommandReader(command, conn2);

            List<string> tables1 = new List<string>();
            List<string> tables2 = new List<string>();

            while (dr1.Read())
            {
                tables1.Add(dr1[0].ToString());
            }
            while (dr2.Read())
            {
                tables2.Add(dr2[0].ToString());
            }

            // Get the unique table only in database1 or database2 
            foreach (string item in tables1.Except(tables2))
            {
                Table table = new Table(item, false, conn1.Database);
                rtnTables.Add(table);
            }
            foreach (string item in tables2.Except(tables1))
            {
                Table table = new Table(item, false, conn2.Database);
                rtnTables.Add(table);
            }


            // Get the coeixt tables in database1 and database2 
            List<Table> candidateTables = new List<Table>();
            foreach (string item in tables1.Intersect(tables2))
            {
                Table table = new Table(item, true, ""); 
                candidateTables.Add(table);
            }


            initDiffColumns(conn1, conn2, candidateTables);
            initIndexes(conn1, conn2, candidateTables);
            initKeys(conn1, conn2, candidateTables);
            //table.indexes = compareIndexs(conn1, conn2, item);
            //table.keys = compareKeys(conn1, conn2, item);
            foreach (Table table in candidateTables)
            {
                if (table.columns.Count() != 0 || table.indexes.Count() != 0 || table.keys.Count() != 0) rtnTables.Add(table);
            }

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ compareTables ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return rtnTables;
        }


        public void initIndexes(SqlConnection conn1, SqlConnection conn2, List<Table> tables)
        {

            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ initIndexes ] - start time : " + start.ToString());

            if (tables.Count() == 0) return;
            List<DBIndexes> indexes1 = new List<DBIndexes>();
            List<DBIndexes> indexes2 = new List<DBIndexes>();

            string command = "SELECT  indexname = a.name , tablename = c. name , indexcolumns = d .name , a .indid " +
                             "FROM    sysindexes a JOIN sysindexkeys b ON a .id = b .id  AND a .indid = b.indid " +
                             "JOIN sysobjects c ON b .id = c .id " +
                             "JOIN syscolumns d ON b .id = d .id  AND b .colid = d .colid " +
                             "WHERE   a .indid NOT IN ( 0 , 255 )  " +
                             "AND (c.name = '" + tables[0].name + "' ";
            for(int i = 1; i < tables.Count(); i++){
                command += " or c.name = '" + tables[i].name + "' ";
            }
            command += ") ORDER BY c. name , a.name , d.name";
            SqlDataReader dr1 = SqlServer.ExcuteSqlCommandReader(command, conn1);
            SqlDataReader dr2 = SqlServer.ExcuteSqlCommandReader(command, conn2);
            while (dr1.Read())
            {
                if (indexes1.Where(a => a.indexName == dr1[0].ToString() && a.tbName == dr1[1].ToString()).Count() == 0)
                {
                    indexes1.Add(new DBIndexes(dr1[0].ToString(), dr1[1].ToString()));
                }
            }
            while (dr2.Read())
            {
                if (indexes2.Where(a => a.indexName == dr2[0].ToString() && a.tbName == dr2[1].ToString()).Count() == 0)
                {
                    indexes2.Add(new DBIndexes(dr2[0].ToString(), dr2[1].ToString()));
                }
            }
            foreach (Table table in tables)
            {
                List<DBIndexes> index1 = indexes1.Where(a => a.tbName == table.name).ToList();
                List<DBIndexes> index2 = indexes2.Where(a => a.tbName == table.name).ToList();
                table.indexes = compareIndexs(conn1.Database, conn2.Database, table.name, index1, index2);
            }


            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ initIndexes ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");
        }

        /// <summary>
        /// Compare all the indexs information between two databases of one same table
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<Indexes> compareIndexs(string dbname1,string dbname2,string tbname,List<DBIndexes> dbindexes1, List<DBIndexes> dbindexes2)
        {

            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ compareIndexs ] - start time : " + start.ToString());

            List<Indexes> rtn = new List<Indexes>();
            Indexes indexs1 = new Indexes(dbname1, tbname);
            Indexes indexs2 = new Indexes(dbname2, tbname);

            foreach (DBIndexes dbindex in dbindexes1)
            {
                indexs1.indexes.Add(dbindex.indexName);
            }
           foreach(DBIndexes dbindex in dbindexes2)
            {
                // index name is the first element
                indexs2.indexes.Add(dbindex.indexName);
            }

            if (!indexs1.compare(indexs2))
            {
                rtn.Add(indexs1);
                rtn.Add(indexs2);
            }


            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ compareIndexs ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return rtn;
        }


        public void initKeys(SqlConnection conn1, SqlConnection conn2, List<Table> tables)
        {

            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ initKeys ] - start time : " + start.ToString());

            if (tables.Count() == 0) return;
            List<DBKeys> keys1 = new List<DBKeys>();
            List<DBKeys> keys2 = new List<DBKeys>();
            string command = "SELECT t.TABLE_NAME, t.CONSTRAINT_TYPE, c.COLUMN_NAME " +
                                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS t,INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS c " +
                                "WHERE t.CONSTRAINT_NAME = c.CONSTRAINT_NAME AND (OBJECT_ID(t.TABLE_NAME) = OBJECT_ID('" + tables[0].name + "') ";
            for (int i = 1; i < tables.Count(); i++)
            {
                command += " or OBJECT_ID(t.TABLE_NAME) = OBJECT_ID('" + tables[i].name + "') ";
            }
            command += ")";
            SqlDataReader dr1 = SqlServer.ExcuteSqlCommandReader(command, conn1);
            SqlDataReader dr2 = SqlServer.ExcuteSqlCommandReader(command, conn2);
            while (dr1.Read())
            {
                keys1.Add(new DBKeys() { tbname = dr1[0].ToString(), constType = dr1[1].ToString(), colName = dr1[2].ToString() });
            }
            while (dr2.Read())
            {
                keys2.Add(new DBKeys() { tbname = dr2[0].ToString(), constType = dr2[1].ToString(), colName = dr2[2].ToString() });
            }

            foreach (Table table in tables)
            {
                List<DBKeys> key1 = keys1.Where(a => a.tbname == table.name).ToList();
                List<DBKeys> key2 = keys2.Where(a => a.tbname == table.name).ToList();
                table.keys = compareKeys(conn1.Database, conn2.Database, table.name, key1, key2);
            }


            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ initKeys ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");
        }


        /// <summary>
        /// Compare all the Keys information between two databases of one same table
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<Key> compareKeys(string dbname1,string dbname2,string tbname,List<DBKeys> dbkeys1,List<DBKeys> dbkeys2) 
        {

            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ compareKeys ] - start time : " + start.ToString());

            List<Key> rtn = new List<Key>();
            Key key1 = new Key(dbname1, tbname);
            Key key2 = new Key(dbname2, tbname);
            foreach(DBKeys key in dbkeys1)
            {
                // dr[1] means the type of the key
                string type = key.constType;
                switch (type)
                {
                    case "FOREIGN KEY":
                        key1.foreignKeys.Add(key.colName);
                        break;
                    case "PRIMARY KEY":
                        key1.primaryKeys.Add(key.colName);
                        break;
                }
            }
            foreach (DBKeys key in dbkeys2)
            {
                string type = key.constType;
                switch (type)
                {
                    case "FOREIGN KEY":
                        key2.foreignKeys.Add(key.colName);
                        break;
                    case "PRIMARY KEY":
                        key2.primaryKeys.Add(key.colName);
                        break;
                }
            }

            if (!key1.compare(key2))
            {
                rtn.Add(key1);
                rtn.Add(key2);
            }

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ compareKeys ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return rtn;
        }



        /// <summary>
        /// Init the different columns information in the tables list
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <param name="tables"></param>
        private void initDiffColumns(SqlConnection conn1, SqlConnection conn2, List<Table> tables)
        {

            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ initDiffColumns ] - start time : " + start.ToString());

            if (tables.Count() == 0) return;
            List<DBColumns> dbColumns1 = new List<DBColumns>();
            List<DBColumns> dbColumns2 = new List<DBColumns>();

            // Excute sql command to get all the columns information of all tables
            string command = "SELECT TABLE_NAME,COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,CHARACTER_OCTET_LENGTH,NUMERIC_PRECISION,NUMERIC_PRECISION_RADIX,NUMERIC_SCALE FROM INFORMATION_SCHEMA.columns WHERE TABLE_NAME='" + tables[0].name + "' ";
            for (int i = 1; i < tables.Count(); i++)
            {
                command += " OR TABLE_NAME='" + tables[i].name + "' ";
            }

            SqlDataReader dr1 = SqlServer.ExcuteSqlCommandReader(command, conn1);
            SqlDataReader dr2 = SqlServer.ExcuteSqlCommandReader(command, conn2);

            while (dr1.Read())
            {
                dbColumns1.Add(new DBColumns()
                {
                    tbName = dr1[0].ToString(),
                    colName = dr1[1].ToString(),
                    colDefault = dr1[2].ToString(),
                    isNullable = dr1[3].ToString(),
                    dataType = dr1[4].ToString(),
                    charMaxLen = dr1[5].ToString(),
                    charOctetLen = dr1[6].ToString(),
                    numericPrecision = dr1[7].ToString(),
                    numericPrecisionRadix = dr1[8].ToString(),
                    numericScale = dr1[9].ToString()
                });
            } 
            while (dr2.Read())
            {
                dbColumns2.Add(new DBColumns()
                {
                    tbName = dr2[0].ToString(),
                    colName = dr2[1].ToString(),
                    colDefault = dr2[2].ToString(),
                    isNullable = dr2[3].ToString(),
                    dataType = dr2[4].ToString(),
                    charMaxLen = dr2[5].ToString(),
                    charOctetLen = dr2[6].ToString(),
                    numericPrecision = dr2[7].ToString(),
                    numericPrecisionRadix = dr2[8].ToString(),
                    numericScale = dr2[9].ToString()
                });
            }

            foreach (Table table in tables)
            {
                string tbname = table.name;
                List<DBColumns> columns1 = dbColumns1.Where(a => a.tbName == tbname).ToList();
                List<DBColumns> columns2 = dbColumns2.Where(a => a.tbName == tbname).ToList();
                table.columns = compareColumns(conn1.Database, conn2.Database, columns1, columns2);
            }

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ initDiffColumns ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return;
        }

        /// <summary>
        /// Compare all the columns information between two databases of one same table 
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<ColumnDiff> compareColumns(string dbname1,string dbname2,List<DBColumns> columns1,List<DBColumns> columns2)
        {
            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ compareColumns ] - start time : " + start.ToString());


            List<ColumnDiff> columns = new List<ColumnDiff>();

            List<Column> cols1 = readColumns(dbname1, columns1);
            List<Column> cols2 = readColumns(dbname2, columns2);

            // get and delete not coexit columns
            foreach (ColumnDiff item in getAndDeleteNoCoexitColumns(cols1, cols2, dbname1, dbname2))
            {
                columns.Add(item);
            }

            // compare the coeixt columns
            cols1.Sort((a, b) => a.colname.CompareTo(b.colname));
            cols2.Sort((a, b) => a.colname.CompareTo(b.colname));

            for (int i = 0; i < cols1.Count(); i++)
            {
                if (!cols1[i].Compare(cols2[i]))
                {
                    ColumnDiff colDiff = new ColumnDiff(cols1[i].colname);
                    colDiff.different.Add(cols1[i]);
                    colDiff.different.Add(cols2[i]);
                    columns.Add(colDiff);
                }
            }

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ compareColumns ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return columns;
        }

        /// <summary>
        /// Get the Column that only in database1 or only in database2
        /// The unique columns will be deleted from the list
        /// Time complex: 2n
        /// </summary>
        /// <param name="cols1"></param>
        /// <param name="cols2"></param>
        /// <param name="dbname1"></param>
        /// <param name="dbname2"></param>
        /// <returns></returns>
        private List<ColumnDiff> getAndDeleteNoCoexitColumns(List<Column> cols1, List<Column> cols2, string dbname1, string dbname2)
        {

            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ getAndDeleteNoCoexitColumns ] - start time : " + start.ToString());

            List<ColumnDiff> rtn = new List<ColumnDiff>();
            List<Column> toBeDelete1 = new List<Column>();
            List<Column> toBeDelete2 = new List<Column>();


            // Sort the columns information by column name
            cols1.Sort((a, b) => a.colname.CompareTo(b.colname));
            cols2.Sort((a, b) => a.colname.CompareTo(b.colname));

            // Get all the unique columns 
            int pos1 = 0, pos2 = 0;
            while (pos1 < cols1.Count() && pos2 < cols2.Count())
            {
                if (cols1[pos1].colname == cols2[pos2].colname)
                {
                    pos1++;
                    pos2++;
                    continue;
                }
                else if (cols1[pos1].colname.CompareTo(cols2[pos2].colname) < 0)
                {
                    toBeDelete1.Add(cols1[pos1]);
                    pos1++;
                }
                else
                {
                    toBeDelete2.Add(cols2[pos2]);
                    pos2++;
                }
            }
            while (pos1 < cols1.Count())
            {
                toBeDelete1.Add(cols1[pos1]);
                pos1++;
            }
            while (pos2 < cols2.Count())
            {
                toBeDelete2.Add(cols2[pos2]);
                pos2++;
            }

            foreach (Column item in toBeDelete1)
            {
                ColumnDiff colDiff = new ColumnDiff(item.colname);
                item.exist = true;
                Column noCol = new Column(dbname2, item.colname, false);

                colDiff.different.Add(item);
                colDiff.different.Add(noCol);
                cols1.Remove(item);
                rtn.Add(colDiff);
            }

            foreach (Column item in toBeDelete2)
            {
                ColumnDiff colDiff = new ColumnDiff(item.colname);
                item.exist = true;
                Column noCol = new Column(dbname1, item.colname, false);

                colDiff.different.Add(item);
                colDiff.different.Add(noCol);
                cols2.Remove(item);
                rtn.Add(colDiff);
            }

            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ getAndDeleteNoCoexitColumns ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");

            return rtn;
        }

        /// <summary>
        /// Read the column information of a table
        /// It is just used in function compareColumns
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private List<Column> readColumns(string dbname, List<DBColumns> dbColumns)
        {

            // Logger
            DateTime start = DateTime.Now;
            logger.Info("[ readColumns ] - start time : " + start.ToString());

            List<Column> cols = new List<Column>();

            foreach(DBColumns dbcolumn in dbColumns)
            {
                Column col = new Column(dbname, dbcolumn.colName, true);
                col.propeties = new List<ColumnPropetie>();
                col.propeties.Add(new ColumnPropetie() { name = "COLUMN_DEFAULT", value = dbcolumn.colDefault });
                col.propeties.Add(new ColumnPropetie() { name = "IS_NULLABLE", value = dbcolumn.isNullable });
                col.propeties.Add(new ColumnPropetie() { name = "DATA_TYPE", value = dbcolumn.dataType });
                col.propeties.Add(new ColumnPropetie() { name = "CHARACTER_MAXIMUM_LENGTH", value = dbcolumn.charMaxLen });
                col.propeties.Add(new ColumnPropetie() { name = "CHARACTER_OCTET_LENGTH", value = dbcolumn.charOctetLen });
                col.propeties.Add(new ColumnPropetie() { name = "NUMERIC_PRECISION", value = dbcolumn.numericPrecision });
                col.propeties.Add(new ColumnPropetie() { name = "NUMERIC_PRECISION_RADIX", value = dbcolumn.numericPrecisionRadix });
                col.propeties.Add(new ColumnPropetie() { name = "NUMERIC_SCALE", value = dbcolumn.numericScale });
                cols.Add(col);
            }


            // Logger
            DateTime end = DateTime.Now;
            logger.Info("[ readColumns ] - end time : " + end.ToString() + " ; spend time : " + (end - start).ToString() + "\n");
            return cols;

        }


    }
}