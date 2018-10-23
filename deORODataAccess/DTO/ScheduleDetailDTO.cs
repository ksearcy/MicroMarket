using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class ScheduleDetailDTO
    {
        public int id { get; set; }
        public int scheduleid { get; set; }
        public string driverid { get; set; }
        public string username { get; set; }
        public int customerid { get; set; }
        public string customername { get; set; }
        public int locationid { get; set; }
        public string locationname { get; set; }
        public string status { get; set; }
        public int count { get; set; }
        public bool selected { get; set; }
        public string excluded_categories { get; set; }
        public string excluded_planograms { get; set; }
    }
}
