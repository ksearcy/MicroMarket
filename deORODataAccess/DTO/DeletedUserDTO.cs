using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class DeletedUserDTO
    {
        public int id { get; set; }
        public int customerid { get; set; }
        public string customername { get; set; }
        public int locationid { get; set; }
        public string locationname { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string phone { get; set; }
        public decimal amount_to_refund { get; set; }
        public byte refund_processed { get; set; }
        public byte refund_cleared { get; set; }
    }
}
