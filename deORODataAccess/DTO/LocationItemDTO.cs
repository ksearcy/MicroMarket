using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class LocationItemDTO
    {
        public int id { get; set; }
        public string customername { get; set; }
        public string locationame { get; set; }
        public int locationid { get; set; }
        public int itemid { get; set; }
        public string itemname { get; set; }
        public string barcode { get; set; }
        public string upc { get; set; }
        public string discountname { get; set; }
        public byte is_taxable { get; set; }
        public decimal price { get; set; }
        public decimal tax { get; set; }
        public decimal price_tax_included { get; set; }
        public decimal tax_percent { get; set; }
        public decimal crv { get; set; }
        public int par { get; set; }
        public int quantity { get; set; }
        public int depletion_level { get; set; }
        public string image { get; set; }
        public string html { get; set; }
        public string salesdata { get; set; }
        public int combodiscount { get; set; }
        public string chartdata { get; set; }
        public string subsidyname { get; set; }
        public int? adjusted_par { get; set; }
        public decimal unitcost { get; set; }
    }
}
