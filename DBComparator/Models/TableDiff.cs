using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    /// <summary>
    /// Return body
    /// Just like the http response body
    /// Contains some information such as code,msg and so on 
    /// </summary>
    public class TableDiff
    {
        public TableDiff()
        {
            code = 200;
            msg = "";
            tables = new List<Table>();
        }
        public int code { get; set; }
        public string msg { get; set; }
        public List<Table> tables { get; set; }
        public List<StoredProcedureDiff> storedProcedures { get; set; }
        public List<FunctionDiff> functions { get; set; }
    }
}