using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Models
{
    public class ResponseCode
    {
        public static readonly int SUCCESS = 1000;
        public static readonly int INPUT_ERROR = 1001;
        public static readonly int DB_NOT_FUND = 1002;
    }
}