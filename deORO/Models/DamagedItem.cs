using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Helpers;

namespace deORO.Models
{
    public class DamagedItem : NotificationObject
    {
        private bool selected;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; RaisePropertyChanged(() => Selected); }
        }
        private string category;

        public string Category
        {
            get { return category; }
            set { category = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string barcode;

        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        private decimal price;

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}
