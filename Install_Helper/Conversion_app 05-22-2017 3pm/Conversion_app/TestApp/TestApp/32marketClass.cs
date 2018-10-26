using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestApp
{
    public class User2
    {
        public int idUser { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string creationDate { get; set; }
        public DateTime? lastActivity { get; set; }
        public int idWarehouse { get; set; }
        public int permissionLevel { get; set; }
        public bool? isEmployee { get; set; }
        public int idParentUser { get; set; }
        public List<object> subUsers { get; set; }
    }

    public class User
    {
        public User2 user { get; set; }
        public List<Credentials> credentials { get; set; }
        public List<object> cards { get; set; }
        public List<Tags> tags { get; set; }
        public List<object> userNames { get; set; }
        public List<object> passwords { get; set; }
        public List<UserSettings> userSettings { get; set; }
        public List<object> hashedLogins { get; set; }
        public List<object> actionPermissions { get; set; }
    }

    public class Credentials
    { 
        public int idUserCredentials { get; set; }
        public string accountCode { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public bool? isEnabled { get; set; }
        public int? idUser { get; set; }
        public bool isEmailVerified { get; set; }
        public string lastPasswordChangeDate { get; set; }
    }

    public class UserSettings
    { 
        public int idUser { get; set; }
        public int userSettingType { get; set; }
        public string value { get; set; }
        public bool? isEnabled { get; set; }
        public int idUserSetting { get; set; }
    }

    public class Tags
    { 
        public int idUserDogTag { get; set; }
        public string upc { get; set; }
        public int idUser { get; set; }
        public bool isEnabled { get; set; }            
    }

    public class FingerPrint
    {
        private string _data;

        public int idUserFingerprint { get; set; }
        public string data 
        {
            get
            { 
                byte[] d = Convert.FromBase64String(_data);
                return Encoding.Unicode.GetString(d);                
            }
            set
            {
                _data = value;
            }
        }
        public int idUser { get; set; }
    }

    public class Value
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public double? marketBalance { get; set; }
        public double? payrollBalance { get; set; }
        public int contractType { get; set; }
        public User user { get; set; }
        public List<FingerPrint> fingerPrints { get; set; }
        public bool isSynced { get; set; }
        public DateTime? lastDeposit { get; set; }
    }

    public class Row
    {
        public string id { get; set; }
        public int key { get; set; }
        public Value value { get; set; }
    }

    public class RootObject
    {
        public int total_rows { get; set; }
        public int offset { get; set; }
        public List<Row> rows { get; set; }
    }
}
