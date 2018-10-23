using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class ItemDTO
    {
        public int id { get; set; }
        public int manufacturerid { get; set; }
        public string manufacturername { get; set; }
        public int categoryid { get; set; }
        public string categoryname { get; set; }
        public int subcategoryid { get; set; }
        public string subcategoryname { get; set; }
        public string upc { get; set; }
        public string name { get; set; }
        public string barcode { get; set; }
        public string description { get; set; }
        public int count { get; set; }
        public decimal unitcost { get; set; }
        public string avgshelflife { get; set; }
        public int pickorder { get; set; }
        public byte is_taxable { get; set; }
        public decimal price { get; set; }
        public decimal tax { get; set; }
        public decimal price_tax_included { get; set; }
        public decimal tax_percent { get; set; }
        public byte has_barcode { get; set; }
        public string image { get; set; }
        public byte is_active { get; set; }
        public decimal crv { get; set; }
        
    }
}
