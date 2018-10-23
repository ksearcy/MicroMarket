using System;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class BaseViewModel : NotificationObject, IDisposable
    {
        private readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public virtual void Init()
        {
            //aggregator.GetEvent<EventAggregation.UserControlLoaded>().Publish(this.GetType().Name);
            //Global.CurrentViewModel = this.GetType().Name;
        }

        public virtual void Dispose()
        {
          
        }
    }
}