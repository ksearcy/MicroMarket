using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class LocationDashboardDTO
    {
        public List<LocationDashboardItemDTO> last30Top5 { get; set; }
        public List<LocationDashboardItemDTO>last7Top5 { get; set; }
        public List<LocationDashboardItemDTO> dateRangeTop5 { get; set; }

        public List<LocationDashboardItemDTO> last30Bottom5 { get; set; }
        public List<LocationDashboardItemDTO> last7Bottom5 { get; set; }
        public List<LocationDashboardItemDTO> dateRangeBottom5 { get; set; }

        public List<LocationDashboardErrorDTO> deviceErrors { get; set; }
        public List<LocationDashboardErrorDTO> transactionErrors { get; set; }

    }

    public class LocationDashboardItemDTO
    {
        public string itemname { get; set; }
        public int count { get; set; }
        public decimal amount { get; set; }
        public int totalSaleCount { get; set; }
        public decimal totalSaleAmount { get; set; }
    }

    public class LocationDashboardErrorDTO
    {
        public string source { get; set; }
        public int count { get; set; }
        public string description { get; set; }
    }
}
