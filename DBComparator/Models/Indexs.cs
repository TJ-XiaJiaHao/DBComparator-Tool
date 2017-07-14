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
            indexs = new List<string>();
        }

        public string dbname { get; set; }
        public string tbname { get; set; }
        public List<string> indexs { get; set; }
        public bool compare(Indexs index)
        {
            this.indexs.Sort();
            index.indexs.Sort();

            if (this.indexs.Count() != index.indexs.Count()) return false;

            for (int i = 0; i < this.indexs.Count(); i++)
            {
                if (this.indexs[i] != index.indexs[i]) return false;
            }

            return true;
        }
    }
}