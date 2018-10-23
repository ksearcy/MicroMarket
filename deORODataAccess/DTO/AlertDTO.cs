using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class AlertDTO
    {
        public int id { get; set; }
        public int metricid { get; set; }
        public string name { get; set; }
        public string frequeny { get; set; }
        public string period { get; set; }
        public DateTime ?last_run_date { get; set; }
        public DateTime ?next_run_date { get; set; }
        public string status { get; set; }
        public string query { get; set; }
        public string date_range { get; set; }
        public string report_type { get; set; }
    }
}
