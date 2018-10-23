using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace deOROImageMaster.Models
{
    public class ItemDTO
    {
        public string id { get; set; }
        public string upc { get; set; }
        public string description {get;set;}
        public string manufacturer { get; set; }
        public string brand { get; set; }
    }
}