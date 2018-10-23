using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using deORO.CardProcessor;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using deOROSyncData;

namespace deORO.ViewModels
{
    class SyncDataViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private ISyncData sync = null;
        private DispatcherTimer timer = new DispatcherTimer();
        private CashStatus.CashStatus cashStatus = null;
        private LocationServiceRepository repo = new LocationServiceRepository();
        private BackgroundWorker worker = new BackgroundWorker();

        public override void Init()
        {
            sync = SyncDataFactory.GetSyncData();
            sync.Init();
            cashStatus = new CashStatus.CashStatus();

            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();

            base.Init();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            cashStatus.GetCashStatus("SyncData");
            sync.DownloadData();
            sync.UploadData();
            //repo.ResetService();
            Dispose();

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
            
        }


        void timer_Tick(object sender, EventArgs e)
        {
            timer.IsEnabled = false;
            worker.RunWorkerAsync();
        }

        public override void Dispose()
        {
            if (timer != null)
            {
                timer.Tick -= timer_Tick;
                timer.Stop();
                timer.IsEnabled = false;
                timer = null;
            }
        }

    }
}
