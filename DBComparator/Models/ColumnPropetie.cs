using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace DBComparator.Models
{
    public class ColumnPropetie
    {
        public ColumnPropetie()
        {
            name = "";
            value = "";
        }
        public string name { get; set; }
        public string value { get; set; }
        public bool Compare(ColumnPropetie propetie)
        {
            if (this.name != propetie.name) return false;
            if (this.value != propetie.value) return false;
            return true;
        }
    }
}