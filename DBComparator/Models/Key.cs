using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class Key
    {
        public Key()
            : this("", "")
        {
        }

        public Key(string dbname, string tbname)
        {
            this.dbname = dbname;
            this.tbname = tbname;
            this.primaryKeys = new List<string>();
            this.foreignKeys = new List<string>();
        }
        public string dbname { get; set; }
        public string tbname { get; set; }
        public List<string> primaryKeys { get; set; }
        public List<string> foreignKeys { get; set; }
        public bool compare(Key key)
        {
            this.primaryKeys.Sort();
            this.foreignKeys.Sort();
            key.primaryKeys.Sort();
            key.foreignKeys.Sort();

            if (this.primaryKeys.Count() != key.primaryKeys.Count() ||
                this.foreignKeys.Count() != key.foreignKeys.Count()) return false;

            for (int i = 0; i < this.primaryKeys.Count(); i++)
            {
                if (this.primaryKeys[i] != key.primaryKeys[i]) return false;
            }
            for (int i = 0; i < this.foreignKeys.Count(); i++)
            {
                if (this.foreignKeys[i] != key.foreignKeys[i]) return false;
            }
            return true;
        }
    }
}