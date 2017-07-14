using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBComparator.Server
{
    public class Log
    {
        public static void log(string msg)
        {
            Console.WriteLine("[ " + DateTime.Now.ToString() + " ] - " + msg);
        }
    }
}