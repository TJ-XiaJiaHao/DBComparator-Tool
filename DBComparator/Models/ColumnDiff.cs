using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class ColumnDiff
    {
        public ColumnDiff()
            : this("")
        {
        }
        public ColumnDiff(string name)
        {
            this.name = name;
            different = new List<Column>();
        }
        public string name { get; set; }
        public List<Column> different;
    }
}