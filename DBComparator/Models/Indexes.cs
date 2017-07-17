using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class Indexes
    {
        public string dbname { get; set; }
        public string tbname { get; set; }
        public List<string> indexes { get; set; }

        public Indexes()
            : this("", "")
        {}

        public Indexes(string dbname, string tbname)
        {
            this.dbname = dbname;
            this.tbname = tbname;
            indexes = new List<string>();
        }

        public bool compare(Indexes index)
        {
            this.indexes.Sort();
            index.indexes.Sort();

            if (this.indexes.Count() != index.indexes.Count()) return false;

            for (int i = 0; i < this.indexes.Count(); i++)
            {
                if (this.indexes[i] != index.indexes[i]) return false;
            }

            return true;
        }
    }
}