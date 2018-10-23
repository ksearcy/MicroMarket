using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class ShoppingCartItemDTO
    {
        public string itemname { get; set; }
        public string barcode { get; set; }
        public decimal price { get; set; }
        public decimal tax { get; set; }
        public decimal price_tax_included { get; set; }
        
    }
}
