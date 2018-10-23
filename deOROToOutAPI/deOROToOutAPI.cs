using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace deOROToOutAPI
{
    public partial class deOROToOutAPI : ServiceBase
    {
        System.Timers.Timer timer;
        private Object timerLock = new Object();


        public deOROToOutAPI()
        {
            InitializeComponent();
            this.ServiceName = "deORO To Out API";
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            //Setup logging
            this.AutoLog = false;

            ((ISupportInitialize)this.EventLog).BeginInit();
            if (!EventLog.SourceExists(this.ServiceName))
            {
                EventLog.CreateEventSource(this.ServiceName, "Application");
            }
            ((ISupportInitialize)this.EventLog).EndInit();

            this.EventLog.Source = this.ServiceName;
            this.EventLog.Log = "Application";

            Common.eventLog = this.EventLog;
            DatabasesCommunicator.Initialize();
            APICommunicator.Initialize();

            this.timer = new System.Timers.Timer(Properties.Settings.Default.RUN_SERVICE_EVERY_X_MINUTES * 60 * 1000);  // minutes expressed as milliseconds
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerElapsed);
            this.timer.AutoReset = true;
            this.timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Run Download database, customer, location
            // plus get form linode items, and planogram with prices
            // plus All Cart uploads to db
            //throw new NotImplementedException();
            lock (timerLock)
            {
                APICommunicator.RunAll();
            }
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            this.EventLog.WriteEntry("Started", EventLogEntryType.Information);
            lock (timerLock)
            {
                APICommunicator.RunAll();
            }
        }


        protected override void OnStop()
        {
            this.timer.Stop();
            this.timer.Dispose();
            this.EventLog.WriteEntry("Stopped", EventLogEntryType.Information);
            base.OnStop();
        }

        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            OnStop();
        }
    }
}
