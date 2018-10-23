using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.Models
{
    public class TransactionHistory
    {
        public string username { get; set; }
        public string type { get; set; }
        public decimal? amount { get; set; }
        public DateTime createddatetime { get; set; }
    }
}
