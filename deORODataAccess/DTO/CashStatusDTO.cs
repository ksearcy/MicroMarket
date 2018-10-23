using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class CashStatusDTO
    {
        public string Description { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public int Count { get; set; }
        public string IsFull { get; set; }
    }
}
