using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Composite.Events;

namespace deORO.EventAggregation
{
    public static class deOROEventAggregator
    {
        private static IEventAggregator aggregator = null;

        public static IEventAggregator GetEventAggregator()
        {
            if (aggregator == null)
            {
                aggregator = new EventAggregator();
                return aggregator;
            }
            else
            {
                return aggregator;
            }
        }
    }
}
