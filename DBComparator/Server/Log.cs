using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace DBComparator.Server
{
    public class Log
    {
        public static log4net.ILog getLogger(Type t){
            return log4net.LogManager.GetLogger(t);
        }
    }
}