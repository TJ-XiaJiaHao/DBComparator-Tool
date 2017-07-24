using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace DBComparator.Models
{
    public class Function
    {
        public Function()
            : this("", "", "", false)
        {

        }
        public Function(string dbname, string name, string statements, bool exist)
        {
            this.dbname = dbname;
            this.name = name;
            this.statements = statements;
            this.exist = exist;
            this.regex = RemoveSqlComment(this.statements);
        }

        public bool compare(Function function)
        {
            this.regex = RemoveSqlComment(this.statements);
            function.regex = RemoveSqlComment(function.statements);
            if (this.name != function.name || this.regex != function.regex) return false;
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

        public string dbname { get; set; }
        public string name { get; set; }
        public bool exist { get; set; }
        public string statements { get; set; }
        public string regex { get; set; }
    }
}