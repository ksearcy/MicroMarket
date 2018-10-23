using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.Models
{
    public class FastTouch
    {
        public int id { get; set; }
        public int itemid { get; set; }
        public string barcode { get; set; }
        public int order { get; set; }
        public string image { get; set; }
        public string category { get; set; }
    }
}
