using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class DBProcedures
    {
        public DBProcedures()
            : this("", "")
        {
        }
        public DBProcedures(string name, string statement)
        {
            this.name = name;
            this.statement = statement;
        }
        public string name { get; set; }
        public string statement { get; set; }
    }
}