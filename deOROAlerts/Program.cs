using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deOROAlerts
{
    class Program
    {
        static void Main(string[] args)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            var alerts = new Alerts();

            try
            {
                alerts.ProcessSubscriptions();
                alerts.SetNextRunDates();
            }
            catch (Exception ex)
            {
                logger.Log(NLog.LogLevel.Error, ex.ToString());
            }
        }
    }
}
