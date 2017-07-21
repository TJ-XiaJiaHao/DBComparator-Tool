using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class DBIndexes
    {
        public DBIndexes()
            : this("", "")
        {}
        public DBIndexes(string indexName, string tbName)
        {
            this.indexName = indexName;
            this.tbName = tbName;
        }
        public string indexName { get; set; }
        public string tbName { get; set; }
    }
}