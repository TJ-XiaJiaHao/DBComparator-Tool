using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace DBComparator.Models
{
    public class StoredProcedure
    {
        public StoredProcedure()
            : this("", "", "", false)
        {
        }

        public StoredProcedure(string dbname, string name, string statements, bool exist)
        {
            this.dbname = dbname;
            this.name = name;
            this.statements = statements;
            this.exist = exist;
            this.regex = RemoveSqlComment(this.statements);
        }

        public bool compare(StoredProcedure storedProducer)
        {
            this.regex = RemoveSqlComment(this.statements);
            storedProducer.regex = RemoveSqlComment(storedProducer.statements);
            if (this.name != storedProducer.name || this.regex != storedProducer.regex) return false;
            return true;
        }

        private string RemoveSqlComment(string sql)
        {
            return Regex.Replace
            (
              sql,
              "(?ms)\\[[^\\]]*\\]|'[^']*'|--.*?$|/\\*(?>/\\*(?<C>)|\\*/(?<-C>)|(?!/\\*|\\*/).)*(?(C)(?!))\\*/",
              delegate(Match m)
              {
                  switch (m.Value[0])
                  {
                      case '-': return "";
                      case '/': return " ";
                      default: return m.Value;
                  }
              }
            ).Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("\t", "").ToUpper();
        }

        public string name { get; set; }
        public string dbname { get; set; }
        public bool exist { get; set; }
        public string statements { get; set; }
        public string regex { get; set; }
    }
}