using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class CashAccountabilityDTO
    {
        public string source { get; set; }
        public decimal? coins { get; set; }
        public int? ones { get; set; }
        public int? twos { get; set; }
        public int? fives { get; set; }
        public int? tens { get; set; }
        public int? twenties { get; set; }
        public int? fifties { get; set; }
        public int? hundreds { get; set; }

    }
}
