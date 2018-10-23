using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class AutoLogoffViewModel : BaseViewModel
    {
        DispatcherTimer timer = new DispatcherTimer();
        int i;
        //string idleTime = Helpers.Global.IdleTimeout.ToString();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }

        private string messageText = "";

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public override void Init()
        {
            i = Convert.ToInt32(Global.AutologoffCountdownTimer);

            messageText = LocalizationProvider.GetLocalizedValue<string>("AutoLogoff.Message");
            Message = string.Format(messageText, i.ToString().PadLeft(2, '0'));

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();
            base.Init();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            i--;
            
            Message = string.Format(messageText, i.ToString().PadLeft(2, '0'));

            if (i == 0)
            {
                aggregator.GetEvent<EventAggregation.AutoLogoffEvent>().Publish(null);
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
            }
        }

        public void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        public override void Dispose()
        {
            timer.Stop();
            timer.Tick -= timer_Tick;
        }
    }
}
