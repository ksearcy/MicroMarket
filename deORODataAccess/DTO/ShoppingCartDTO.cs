using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class ShoppingCartDTO
    {
        public string pkid { get; set; }
        public string locationname { get; set; }
        public string customername { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int itemcount { get; set; }
        public decimal price_tax_included { get; set; }
        public DateTime? created_date_time { get; set; }
    }
}
