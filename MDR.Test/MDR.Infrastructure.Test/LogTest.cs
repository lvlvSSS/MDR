using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDR.Infrastructure.Log;
using MDR.Infrastructure.Log.Implementation;

namespace MDR.Infrastructure.Test
{
    public class LogTest
    {
        [Fact]
        public void TestNlog()
        {
            var logging = new NLog4Logging();
            logging.Error("this is error", new Exception("ex"));
            logging.Trace("this is trace");
        }
    }
}