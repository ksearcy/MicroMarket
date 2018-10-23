using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace deOROToOutAPI
{
    /// <summary>
    /// The main entry point for the application.
    /// Ivana: changed it so it opts out of full service mode when running in interactive mode (i.e. as a console), while falling back to normal service behaviour when run by the Service Controller
    /// </summary>
    static class Program
    {
        static void Main(string[] args)
        {
            deOROToOutAPI service = new deOROToOutAPI();
            if (Environment.UserInteractive)
            {
                service.RunAsConsole(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { service };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
