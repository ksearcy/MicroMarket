using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace deORODataAccess.DTO
{
    public class LocationDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public byte is_active { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email_address { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int customerid { get; set; }
        public string customername { get; set; }
        public string driverid { get; set; }
        public int service_interval { get; set; }
        public DateTime? last_service_date_time { get; set; }
        public string camera_feed_path { get; set; }
        public byte users_shared { get; set; }
    }
}