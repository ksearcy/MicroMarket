using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class LocationCreditActivityDTO
    {
        public int id { get; set; }
        public int customerid { get; set; }
        public string customername { get; set; }
        public int locationid { get; set; }
        public string locationame { get; set; }
        public string username { get; set; }
        public string barcode { get; set; }
        public decimal amount { get; set; }
        public DateTime? expiry_date { get; set; }
        public byte credit_claimed { get; set; }
        public DateTime? credit_claimed_date { get; set; }
    }
}
