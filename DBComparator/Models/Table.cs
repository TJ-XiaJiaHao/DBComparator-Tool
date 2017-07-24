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

        public Table(string name, bool coexist, string dbname)
        {
            this.name = name;
            this.coexist = coexist;
            this.dbnameIfNotCoexit = dbname;
            columns = new List<ColumnDiff>();
            keys = new List<Key>();
            indexes = new List<Indexes>();
        }

        public string name { get; set; }
        public bool coexist { get; set; }
        public string dbnameIfNotCoexit { get; set; }
        public List<ColumnDiff> columns { get; set; }
        public List<Key> keys { get; set; }
        public List<Indexes> indexes { get; set; }
    }
}