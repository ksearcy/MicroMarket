using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class LocationCreditDTO
    {
        public int id { get; set; }
        public int customerid { get; set; }
        public string customername { get; set; }
        public int locationid { get; set; }
        public string locationame { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public decimal amount { get; set; }
        public int? expiry { get; set; }
        public int? interval { get; set; }
        public DateTime? effective_date { get; set; }
        public DateTime? end_date { get; set; }
        public byte is_active { get; set; }
        public DateTime? created_date_time { get; set; }
        public string created_by { get; set; }
    }
}
