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
        /// <summary>
        /// Compare all the table information between two databases
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <returns></returns>
        public List<Table> compareTables(SqlConnection conn1, SqlConnection conn2)
        {
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
            foreach (string item in tables1.Intersect(tables2))
            {
                //if (rtnTables.Count() > 23) break;  ///////////////////////// page??????
                Table table = new Table(item, true, "");
                table.columns = compareColumns(conn1, conn2, item);
                table.indexes = compareIndexs(conn1, conn2, item);
                table.keys = compareKeys(conn1, conn2, item);
                if (table.columns.Count() != 0 && table.indexes.Count() != 0 && table.keys.Count() != 0) rtnTables.Add(table);
            }
            return rtnTables;
        }


        /// <summary>
        /// Compare all the indexs information between two databases of one same table
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<Indexs> compareIndexs(SqlConnection conn1, SqlConnection conn2, string tableName)
        {
            List<Indexs> rtn = new List<Indexs>();
            Indexs indexs1 = new Indexs(conn1.Database, tableName);
            Indexs indexs2 = new Indexs(conn2.Database, tableName);

            string command = "SELECT  indexname = a.name , tablename = c. name , indexcolumns = d .name , a .indid " +
                             "FROM    sysindexes a JOIN sysindexkeys b ON a .id = b .id  AND a .indid = b.indid " +
                             "JOIN sysobjects c ON b .id = c .id " +
                             "JOIN syscolumns d ON b .id = d .id  AND b .colid = d .colid " +
                             "WHERE   a .indid NOT IN ( 0 , 255 )  " +
                             "AND c .name = '" + tableName + "' " +
                             "ORDER BY c. name , a.name , d.name";
            SqlDataReader dr1 = SqlServer.ExcuteSqlCommandReader(command, conn1);
            SqlDataReader dr2 = SqlServer.ExcuteSqlCommandReader(command, conn2);

            while (dr1.Read())
            {
                // index name is the first element
                indexs1.indexes.Add(dr1[0].ToString());
            }
            while (dr2.Read())
            {
                // index name is the first element
                indexs2.indexes.Add(dr2[0].ToString());
            }

            if (!indexs1.compare(indexs2))
            {
                rtn.Add(indexs1);
                rtn.Add(indexs2);
            }
            return rtn;
        }

        /// <summary>
        /// Compare all the Keys information between two databases of one same table
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<Key> compareKeys(SqlConnection conn1, SqlConnection conn2, string tableName)
        {
            List<Key> rtn = new List<Key>();
            Key key1 = new Key(conn1.Database, tableName);
            Key key2 = new Key(conn2.Database, tableName);

            // Excute command to get the primaryKey and foreignKey information of a table
            string command = "SELECT t.TABLE_NAME, t.CONSTRAINT_TYPE, c.COLUMN_NAME " +
                                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS t,INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS c " +
                                "WHERE t.CONSTRAINT_NAME = c.CONSTRAINT_NAME AND OBJECT_ID(t.TABLE_NAME) = OBJECT_ID('" + tableName + "')";
            SqlDataReader dr1 = SqlServer.ExcuteSqlCommandReader(command, conn1);
            SqlDataReader dr2 = SqlServer.ExcuteSqlCommandReader(command, conn2);

            while (dr1.Read())
            {
                // dr[1] means the type of the key
                string type = dr1[1].ToString();
                switch (type)
                {
                    case "FOREIGN KEY":
                        key1.foreignKeys.Add(dr1[2].ToString());
                        break;
                    case "PRIMARY KEY":
                        key1.primaryKeys.Add(dr1[2].ToString());
                        break;
                }
            }
            while (dr2.Read())
            {
                string type = dr2[1].ToString();
                switch (type)
                {
                    case "FOREIGN KEY":
                        key2.foreignKeys.Add(dr2[2].ToString());
                        break;
                    case "PRIMARY KEY":
                        key2.primaryKeys.Add(dr2[2].ToString());
                        break;
                }
            }

            if (!key1.compare(key2))
            {
                rtn.Add(key1);
                rtn.Add(key2);
            }
            return rtn;
        }

        /// <summary>
        /// Compare all the columns information between two databases of one same table 
        /// </summary>
        /// <param name="conn1"></param>
        /// <param name="conn2"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<ColumnDiff> compareColumns(SqlConnection conn1, SqlConnection conn2, string tableName)
        {

            List<ColumnDiff> columns = new List<ColumnDiff>();

            // Excute sql command to get all the columns information of a table
            string command = "SELECT COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,CHARACTER_OCTET_LENGTH,NUMERIC_PRECISION,NUMERIC_PRECISION_RADIX,NUMERIC_SCALE FROM INFORMATION_SCHEMA.columns WHERE TABLE_NAME='" + tableName + "' ";
            SqlDataReader dr1 = SqlServer.ExcuteSqlCommandReader(command, conn1);
            SqlDataReader dr2 = SqlServer.ExcuteSqlCommandReader(command, conn2);

            List<Column> cols1 = readColumns(conn1.Database, dr1);
            List<Column> cols2 = readColumns(conn2.Database, dr2);

            // get and delete not coexit columns
            foreach (ColumnDiff item in getAndDeleteNoCoexitColumns(cols1, cols2, conn1.Database, conn2.Database))
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
            return rtn;
        }

        /// <summary>
        /// Read the column information of a table
        /// It is just used in function compareColumns
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private List<Column> readColumns(string dbname, SqlDataReader dr)
        {
            List<Column> cols = new List<Column>();

            while (dr.Read())
            {
                Column col = new Column(dbname, dr["COLUMN_NAME"].ToString(), true);
                col.propeties = new List<ColumnPropetie>();
                col.propeties.Add(new ColumnPropetie() { name = "COLUMN_DEFAULT", value = dr["COLUMN_DEFAULT"].ToString() });
                col.propeties.Add(new ColumnPropetie() { name = "IS_NULLABLE", value = dr["IS_NULLABLE"].ToString() });
                col.propeties.Add(new ColumnPropetie() { name = "DATA_TYPE", value = dr["DATA_TYPE"].ToString() });
                col.propeties.Add(new ColumnPropetie() { name = "CHARACTER_MAXIMUM_LENGTH", value = dr["CHARACTER_MAXIMUM_LENGTH"].ToString() });
                col.propeties.Add(new ColumnPropetie() { name = "CHARACTER_OCTET_LENGTH", value = dr["CHARACTER_OCTET_LENGTH"].ToString() });
                col.propeties.Add(new ColumnPropetie() { name = "NUMERIC_PRECISION", value = dr["NUMERIC_PRECISION"].ToString() });
                col.propeties.Add(new ColumnPropetie() { name = "NUMERIC_PRECISION_RADIX", value = dr["NUMERIC_PRECISION_RADIX"].ToString() });
                col.propeties.Add(new ColumnPropetie() { name = "NUMERIC_SCALE", value = dr["NUMERIC_SCALE"].ToString() });
                cols.Add(col);
            }

            return cols;

        }


    }
}