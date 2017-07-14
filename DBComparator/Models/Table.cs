using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    /// <summary>
    /// Descripe the basic information of a table
    /// Used in the TableDiff Class which is a return body 
    /// </summary>
    public class Table
    {
        public Table()
            : this("", false, "")
        {
        }

        public Table(string name, bool coexit, string dbname)
        {
            this.name = name;
            this.coexit = coexit;
            this.dbnameIfNotCoexit = dbname;
            columns = new List<ColumnDiff>();
            keys = new List<Key>();
            indexs = new List<Indexs>();
        }

        public string name { get; set; }
        public bool coexit { get; set; }
        public string dbnameIfNotCoexit { get; set; }
        public List<ColumnDiff> columns { get; set; }
        public List<Key> keys { get; set; }
        public List<Indexs> indexs { get; set; }
    }
}