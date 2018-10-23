using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using deORO.Helpers;

namespace deORO.Models
{
    public class User : NotificationObject, IDataErrorInfo
    {
        private bool emailRequired = false;

        public User()
        {

        }

        public User(bool emailRequired)
        {
            this.emailRequired = emailRequired;
        }

        string userName;

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }
        string email;

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged(() => Email);
            }
        }
        string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged(() => Password);
            }
        }
        string confirmPassword;

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                confirmPassword = value;
                RaisePropertyChanged(() => ConfirmPassword);
            }
        }

        string barcode;

        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; RaisePropertyChanged(() => Barcode); }
        }

        string IDataErrorInfo.Error { get { return null; } }

        public string this[string columnName]
        {
            get
            {
                String errorMessage = String.Empty;
                switch (columnName)
                {
                    case "UserName":
                        if (String.IsNullOrEmpty(UserName))
                        {
                            errorMessage = "User Name is required";
                        }
                        break;
                    case "Email":
                        {
                            errorMessage = ValidateEmail();
                        }
                        break;
                    case "Password":
                        {
                            errorMessage = ValidatePassword();
                        }
                        break;
                    case "ConfirmPassword":
                        {
                            errorMessage = ValidateConfirmPassword();
                        }
                        break;

                }
                return errorMessage;
            }
        }

        string ValidatePassword()
        {
            if (IsStringMissing(Password))
            {
                return "Password is requried";
            }

            return null;
        }

        string ValidateConfirmPassword()
        {
            if (IsStringMissing(ConfirmPassword))
            {
                return "Password is requried";
            }
            else if (ConfirmPassword != Password)
            {
                return "Both Passwords should match";
            }

            return null;
        }

        string ValidateEmail()
        {
            if (emailRequired)
            {
                if (IsStringMissing(this.Email))
                {
                    return "Email is required";
                }
                else if (!IsValidEmailAddress(this.Email))
                {
                    return "Invalid Email";
                }
            }

            return null;
        }

        static bool IsStringMissing(string value)
        {
            return String.IsNullOrEmpty(value) ||  value.Trim() == String.Empty;
        }

        static bool IsValidEmailAddress(string email)
        {
            if (IsStringMissing(email))
                return false;

            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

    }
}
