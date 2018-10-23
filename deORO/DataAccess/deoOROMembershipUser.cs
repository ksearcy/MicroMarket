using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace deORO.DataAccess
{
    public class deoOROMembershipUser : MembershipUser
    {
        private int userId;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private decimal accountBalance;

        public decimal AccountBalance
        {
            get { return accountBalance; }
            set { accountBalance = value; }
        }

        private bool isAdmin;

        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }

        private string barcode;

        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public deoOROMembershipUser(
           int userId,
           string providername,
           string username,
           object providerUserKey,
           string email,
           bool isApproved,
           bool isLockedOut,
           DateTime creationDate,
           DateTime lastLoginDate,
           DateTime lastPasswordChangedDate,
           DateTime lastLockedOutDate,
           decimal accountBalance,
           bool isAdmin,
           string barcode,
           string password) :
            base(providername,
                                       username,
                                       providerUserKey,
                                       email,
                                       string.Empty,
                                       string.Empty,
                                       isApproved,
                                       isLockedOut,
                                       creationDate,
                                       lastLoginDate,
                                       lastLoginDate,
                                       lastPasswordChangedDate,
                                       lastLockedOutDate)
        {
            this.userId = userId;
            this.accountBalance = accountBalance;
            this.isAdmin = isAdmin;
            this.barcode = barcode;
            this.password = password;
        }
    }
}
