using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace deOROMonitor.Assistant.Utils
{
    public abstract class Task
    {
        public Thread CurrentThread
        {
            get;
            set;
        }

        protected Task()
        {
        }

        public abstract bool IsRunning();

        public abstract void Start();

        public abstract void Stop();
    }
}
