using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class FunctionDiff
    {
        public FunctionDiff()
            : this("", false)
        {
        }

        public FunctionDiff(string name, bool coexit)
        {
            this.name = name;
            this.coexist = coexit;
            this.different = new List<Function>();
        }

        public string name { get; set; }
        public bool coexist { get; set; }
        public List<Function> different { get; set; }
    }
}