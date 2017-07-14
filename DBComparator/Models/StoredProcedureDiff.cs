using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class StoredProcedureDiff
    {
        public StoredProcedureDiff()
            : this("", false)
        {
        }

        public StoredProcedureDiff(string name, bool coexit)
        {
            this.name = name;
            this.coexit = coexit;
            this.different = new List<StoredProcedure>();
        }

        public string name { get; set; }
        public bool coexit { get; set; }
        public List<StoredProcedure> different { get; set; }
    }
}