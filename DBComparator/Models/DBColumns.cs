using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{

    /// <summary>
    /// Get all the columns from database, this will reduce the count to connect to the database
    /// </summary>
    public class DBColumns
    {
        public DBColumns()
            : this("", "", "", "", "", "", "", "", "", "")
        {}

        public DBColumns(string tbName, string colName, string colDefault, string isNullable,
            string dataType, string charMaxLen, string charOctetLen, string numericPrecision,
            string numericPrecisionRadix, string numericScale)
        {
            this.tbName = tbName;
            this.colName = colName;
            this.colDefault = colDefault;
            this.isNullable = isNullable;
            this.dataType = dataType;
            this.charMaxLen = charMaxLen;
            this.charOctetLen = charOctetLen;
            this.numericPrecision = numericPrecision;
            this.numericPrecisionRadix = numericPrecisionRadix;
            this.numericScale = numericScale;
        }
        public string tbName { get; set; }
        public string colName { get; set; }
        public string colDefault { get; set; }
        public string isNullable { get; set; }
        public string dataType { get; set; }
        public string charMaxLen { get; set; }
        public string charOctetLen { get; set; }
        public string numericPrecision { get; set; }
        public string numericPrecisionRadix { get; set; }
        public string numericScale { get; set; }
    }
}