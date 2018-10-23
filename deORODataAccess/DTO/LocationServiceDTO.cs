using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace deORODataAccess.DTO
{
    public class LocationServiceDTO
    {
        public int id { get; set; }
        public string pkid { get; set; }
        public int? customerid { get; set; }
        public int? locationid { get; set; }
        public string userpkid { get; set; }
        public string comments { get; set; }
        public DateTime? created_date_time { get; set; }
    }
}