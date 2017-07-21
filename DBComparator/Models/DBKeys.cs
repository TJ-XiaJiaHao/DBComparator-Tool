using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class DBKeys
    {
        public DBKeys()
            : this("", "", "")
        {}
        public DBKeys(string tbname, string constType, string colName)
        {
            this.tbname = tbname;
            this.constType = constType;
            this.colName = colName;
        }
        public string tbname { get; set; }
        public string constType { get; set; }
        public string colName { get; set; }
    }
}