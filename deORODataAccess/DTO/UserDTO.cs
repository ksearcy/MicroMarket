using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccess.DTO
{
    public class UserDTO
    {
        public int id { get; set; }
        public int customerid { get; set; }
        public string customername { get; set; }
        public int locationid { get; set; }
        public string locationname { get; set; }
        public string email { get; set; }
        public DateTime? dob { get; set; }
        public string gender { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public DateTime? lastlogindate { get; set; }
        public DateTime? last_updated_on { get; set; }
        public string password { get; set; }
        public string pkid { get; set; }
        public int sync_vector { get; set; }
        public string barcode { get; set; }
        public decimal account_balance { get; set; }
        public decimal lastaccountbalancechangedamount { get; set; }
        public DateTime? lastaccountbalancechangeddate { get; set; }
        public string lastaccountbalancechangeddescription { get; set; }
        public decimal payroll_balance { get; set; }
        public decimal lastpayrollbalancechangedamount { get; set; }        
        public DateTime? lastpayrollbalancechangeddate { get; set; }         
        public string lastpayrollbalancechangeddescription { get; set; }
        public string salt { get; set; }
        public byte is_staff { get; set; }
        public byte is_active { get; set; }
        public byte is_superuser { get; set; }
        public byte is_approved { get; set; }
        public byte is_lockedout { get; set; }
        public string enrolled_fmd1 { get; set; }
        public int? finger_id1 { get; set; }
        public string enrolled_fmd2 { get; set; }
        public int? finger_id2 { get; set; }
        public string enrolled_fmd3 { get; set; }
        public int? finger_id3 { get; set; }
        public string enrolled_fmd4 { get; set; }
        public int? finger_id4 { get; set; }
      
    }
}
