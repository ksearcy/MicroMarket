using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.Helpers;

namespace deORODataAccessApp.Models
{
    public class CreditActivity : NotificationObject
    {
        private bool selected;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; RaisePropertyChanged(() => Selected); }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private decimal amount;

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        private DateTime expiry;

        public DateTime Expiry
        {
            get { return expiry; }
            set { expiry = value; }
        }
    }
}
