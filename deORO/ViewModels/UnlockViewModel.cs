using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using deOROSyncData;
using deORO.EventAggregation;
using deORO.Templates;
using Microsoft.Practices.Composite.Events;
using deORO.Helpers;
using deORO.Printer;
using deORO.Templates;
using deORO.ViewModels;

namespace deORO.ViewModels
{
    public class UnlockViewModel : BaseViewModel
    {

        
        public ICommand KMtronicRelay1OnCommand { get { return new DelegateCommand(ExecuteKMtronicRelay1OnCommand, () => { return KMtronic1OnEnabled; }); } }
        public ICommand KMtronicRelay1OffCommand { get { return new DelegateCommandWithParam(ExecuteKMtronicRelay1OffCommand, () => { return KMtronic1OffEnabled; }); } }
        
        public ICommand KMtronicRelay2OnCommand { get { return new DelegateCommand(ExecuteKMtronicRelay2OnCommand, () => { return KMtronic2OnEnabled; }); } }
        public ICommand KMtronicRelay2OffCommand { get { return new DelegateCommandWithParam(ExecuteKMtronicRelay2OffCommand, () => { return KMtronic2OffEnabled; }); } }
       
        private string userpkid;

        private USBRelay.KMTronic km = new USBRelay.KMTronic();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        LocationServiceRepository repo = new LocationServiceRepository();

        private bool kmtronic1OffEnabled = false;
        public bool KMtronic1OffEnabled
        {
            get { return kmtronic1OffEnabled; }
            set { kmtronic1OffEnabled = value; RaisePropertyChanged(() => KMtronic1OffEnabled); }
        }

        private bool kmtronic1OnEnabled;
        public bool KMtronic1OnEnabled
        {
            get { return kmtronic1OnEnabled; }
            set { kmtronic1OnEnabled = value; RaisePropertyChanged(() => KMtronic1OnEnabled); }

        }

        private bool kmtronic2OffEnabled = false;
        public bool KMtronic2OffEnabled
        {
            get { return kmtronic2OffEnabled; }
            set { kmtronic2OffEnabled = value; RaisePropertyChanged(() => KMtronic2OffEnabled); }
        }

        private bool kmtronic2OnEnabled;
        public bool KMtronic2OnEnabled
        {
            get { return kmtronic2OnEnabled; }
            set { kmtronic2OnEnabled = value; RaisePropertyChanged(() => KMtronic2OnEnabled); }

        }

       

        private void ExecuteKMtronicRelay1OnCommand()
        {
            EventLogRepository repo = new EventLogRepository();
            aggregator.GetEvent<EventAggregation.Relay1CloseEvent>().Subscribe(ExecuteKMtronicRelay1OffCommand);
            try
            {
                KMtronic1OnEnabled = false;
                KMtronic1OffEnabled = true;

                km.OpenRelay1();
                event_log eventlog = new event_log();
                eventlog.description = "DoorOpen";
                eventlog.source = "Relay1";
                eventlog.code = "open";
                eventlog.created_date_time = DateTime.Now;
                eventlog.pkid = Guid.NewGuid().ToString();
                eventlog.userpkid = Global.User.ProviderUserKey.ToString();
                repo.AddEventLog(eventlog);
            }
            catch { }

            CommandManager.InvalidateRequerySuggested();
        }
        private void ExecuteKMtronicRelay1OffCommand(object param = null)
        {
            EventLogRepository repo = new EventLogRepository();
            aggregator.GetEvent<EventAggregation.Relay1CloseEvent>().Unsubscribe(ExecuteKMtronicRelay1OffCommand);
            try
            {
                KMtronic1OnEnabled = true;
                KMtronic1OffEnabled = false;

                event_log eventlog = new event_log();
                eventlog.description = "DoorClosed";
                eventlog.source = "Relay1";
                eventlog.code = "closed";
                eventlog.created_date_time = DateTime.Now;
                eventlog.pkid = Guid.NewGuid().ToString();
                eventlog.userpkid = Global.User.ProviderUserKey.ToString();
                repo.AddEventLog(eventlog);
            }
            catch { }

            CommandManager.InvalidateRequerySuggested();
        }

        private void ExecuteKMtronicRelay2OnCommand()
        {
            EventLogRepository repo = new EventLogRepository();
            aggregator.GetEvent<EventAggregation.Relay2CloseEvent>().Subscribe(ExecuteKMtronicRelay2OffCommand);
            try
            {

                KMtronic2OnEnabled = false;
                KMtronic2OffEnabled = true;

                km.OpenRelay2();
                event_log eventlog = new event_log();
                eventlog.description = "TestMotorOn";
                eventlog.source = "Relay2";
                eventlog.code = "on";
                eventlog.created_date_time = DateTime.Now;
                eventlog.pkid = Guid.NewGuid().ToString();
                eventlog.userpkid = Global.User.ProviderUserKey.ToString();
                repo.AddEventLog(eventlog);
            }
            catch { }

            CommandManager.InvalidateRequerySuggested();
        }
        private void ExecuteKMtronicRelay2OffCommand(object param = null)
        {
            EventLogRepository repo = new EventLogRepository();
            aggregator.GetEvent<EventAggregation.Relay2CloseEvent>().Unsubscribe(ExecuteKMtronicRelay2OffCommand);
            try
            {

                KMtronic2OnEnabled = true;
                KMtronic2OffEnabled = false;

                event_log eventlog = new event_log();
                eventlog.description = "TestMotorOff";
                eventlog.source = "Relay2";
                eventlog.code = "off";
                eventlog.created_date_time = DateTime.Now;
                eventlog.pkid = Guid.NewGuid().ToString();
                eventlog.userpkid = Global.User.ProviderUserKey.ToString();
                repo.AddEventLog(eventlog);
            }
            catch { }

            CommandManager.InvalidateRequerySuggested();
        }

        public override void Dispose()
        {
            if (km != null)
            {
                km.CloseRelay1();
                km.CloseRelay2();
                km.Dispose();
            }
        }
    }
}
