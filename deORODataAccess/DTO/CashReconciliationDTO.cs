using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
   public  class CashReconciliationDTO
    {
        public int id;
        public int customerid;
        public int locationid;
        public string cashcollectionpkid;
        public decimal total_coin;
        public int C1_total;
        public int C2_total;
        public int C5_total;
        public int C20_total;
        public int C50_total;
        public int C100_total;
    }
}
