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
            this.coexit = coexit;
            this.different = new List<Function>();
        }

        public string name { get; set; }
        public bool coexit { get; set; }
        public List<Function> different { get; set; }
    }
}