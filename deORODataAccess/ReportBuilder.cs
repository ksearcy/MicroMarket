using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess
{
    public class ReportBuilder
    {
        deORO_MasterEntities entites = new deORO_MasterEntities();

        public dynamic GetSalesReport(string customerid, string[] locations, string reportType, string sortBy, string fromDate, string toDate)
        {
            string sql = "";
            return entites.Database.DynamicSqlQuery(sql);
        }
    }
}
