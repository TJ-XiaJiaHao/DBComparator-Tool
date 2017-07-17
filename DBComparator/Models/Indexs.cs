using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class Indexs
    {
        public Indexs()
            : this("", "")
        {
        }

        public Indexs(string dbname, string tbname)
        {
            this.dbname = dbname;
            this.tbname = tbname;
            indexes = new List<string>();
        }

        public string dbname { get; set; }
        public string tbname { get; set; }
        public List<string> indexes { get; set; }
        public bool compare(Indexs index)
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