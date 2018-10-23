using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.Models
{
    public class PaymentItem
    {
        private string source;

        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        private DateTime dateTime;

        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        private decimal payment;

        public decimal Payment
        {
            get { return payment; }
            set { payment = value; }
        }

        private string routing;

        public string Routing
        {
            get { return routing; }
            set { routing = value; }
        }
        
    }
}
