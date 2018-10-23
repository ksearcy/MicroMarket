using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.Helpers;

namespace deORODataAccessApp.Models
{
    public class Item : NotificationObject
    {
        public int id { get; set; }
        public string name { get; set; }
        public string barcode { get; set ; }
        private Nullable<int> quantity;

        public Nullable<int> Quantity
        {
            get { return quantity; }
            set { quantity = value; RaisePropertyChanged(() => Quantity); }
        }
        private Nullable<int> stale;

        public Nullable<int> Stale
        {
            get { return stale; }
            set { stale = value; RaisePropertyChanged(() => Stale); }
        }
        private Nullable<int> @short;

        public Nullable<int> Short
        {
            get { return @short; }
            set { @short = value; RaisePropertyChanged(() => Short); }
        }
    }
}
