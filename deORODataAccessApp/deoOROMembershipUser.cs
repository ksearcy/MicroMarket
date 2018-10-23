using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace deORODataAccessApp.DataAccess
{
    public class deOROMembershipUser : MembershipUser
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

        private decimal payrollBalance;

        public decimal PayrollBalance
        {
            get { return payrollBalance; }
            set { payrollBalance = value; }
        }

        private bool isAdmin;

        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }

        private bool isStaff;

        public bool IsStaff
        {
            get { return isStaff; }
            set { isStaff = value; }
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

        private string loginMethod;

        public string LoginMethod
        {
            get { return loginMethod; }
            set { loginMethod = value; }
        }



        public deOROMembershipUser(
           int userId,
           string providername,
           string username,
           object providerUserKey,
           string email,
           DateTime dob,
           string Gender,
           bool isApproved,
           bool isLockedOut,
           DateTime creationDate,
           DateTime lastLoginDate,
           DateTime lastPasswordChangedDate,
           DateTime lastLockedOutDate,
           decimal accountBalance,
           decimal payrollBalance,
           bool isAdmin,
           string barcode,
           string password,
           string salt,
           bool isStaff
            ) :
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
            this.payrollBalance = payrollBalance;
            this.isAdmin = isAdmin;
            this.barcode = barcode;
            this.password = password;
            this.isStaff = isStaff;
        }
    }
}
