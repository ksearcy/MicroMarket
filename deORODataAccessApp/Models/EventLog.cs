using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.Models
{
    public class EventLog
    {
        private string source;

        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        private string @event;

        public string Event
        {
            get { return @event; }
            set { @event = value; }
        }
        private decimal amount;

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        private DateTime dateTime;

        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

    }
}
