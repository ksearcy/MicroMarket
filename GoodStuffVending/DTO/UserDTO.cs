using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodStuffVending.DTO
{
    public class UserDTO
    {
        public string userName { get; set; }
        public string password { get; set; }
        public double? accountBalance { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public Guid role { get; set; }
    }
}
