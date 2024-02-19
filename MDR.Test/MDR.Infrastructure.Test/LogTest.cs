using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDR.Infrastructure.Log;
using MDR.Infrastructure.Log.Implementation;
using NLog;
using NLog.Config;

namespace MDR.Infrastructure.Test
{
    public class LogTest
    {
        [Fact]
        public void TestNlog()
        {
            var logging = new NLog4Logging("./NLog.config");
            logging.Trace("this is trace");
            logging.Trace(123);
            logging.Device("this is device", null);
            logging.Device("this is device", "WWKS");
            logging.Task("this is task", TaskLogStatus.CREATE);
            logging.Task("this is task", TaskLogStatus.FLOW);

            /* for (int i = 0; i < 100; i++)
            {
                logging.Error("this is error", new Exception("ex"));
            } */
            /* logging.Error("this is error...", new Exception("exsss"));
            logging.Trace("PleaseLogThis sdfsdfsdff");
            logging.Trace("NotPleaseLogThis sdfsdfsdff");
 */
            /*             var systemLogger = LogManager.GetLogger("System.NLog.Test");
                        for (int i = 0; i < 10000; i++)
                        {
                            systemLogger.Info("this is system ...");
                        } */


            /*  for (int i = 0; i < 100; i++)
             {
                 logging.Error("this is error!", new Exception("exc"));
             } */
            LogManager.Flush();
        }
    }
}