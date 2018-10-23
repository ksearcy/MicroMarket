using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class CategoryDTO
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int pick_order { get; set; }
        public int parentid { get; set; }
        public string parentname { get; set; }
        public int depletion_level { get; set; }
        public string image { get; set; }
    }
}
