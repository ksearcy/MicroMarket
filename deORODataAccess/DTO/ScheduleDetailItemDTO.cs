using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class ScheduleDetailItemDTO
    {
        public int id { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string upc { get; set; }
        public string barcode { get; set; }
        public int quantity_at_schedule { get; set; }
        public int quantity_to_refill { get; set; }
        public string tote { get; set; }
        public string status { get; set; }
        public int? over_under { get; set; }
    }
}
