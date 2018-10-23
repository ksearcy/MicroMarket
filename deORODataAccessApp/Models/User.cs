using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using deORODataAccessApp.Helpers;

namespace deORODataAccessApp.Models
{
    public class User : NotificationObject, IDataErrorInfo
    {
        private bool emailRequired = false;
        private bool firstLastNameRequired = false;
        private bool dobAndGenderRequired = false;

        public User()
        {

        }

        public User(bool emailRequired, bool firstLastNameRequired, bool dobAndGenderRequired)
        {
            this.emailRequired = emailRequired;
            this.firstLastNameRequired = firstLastNameRequired;
            this.dobAndGenderRequired = dobAndGenderRequired;
        }


        string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; RaisePropertyChanged(() => FirstName); }
        }

        string lastName;
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; RaisePropertyChanged(() => LastName); }
        }

        string dob;
        public string DOB
        {
            get { return dob; }
            set { dob = value; RaisePropertyChanged(() => DOB); }
        }

        string gender;
        public string Gender
        {
            get { return gender; }
            set { gender = value; RaisePropertyChanged(() => Gender); }
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
                RaisePropertyChanged(() => ConfirmPassword);
            }
        }

        string confirmPassword;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                confirmPassword = value;
                RaisePropertyChanged(() => Password);
                RaisePropertyChanged(() => ConfirmPassword);
            }
        }

        string barcode;
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; ; RaisePropertyChanged(() => Barcode); }
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
                        else
                        {
                            errorMessage = ValidateUserName();
                        }
                        break;
                    case "FirstName":
                        if (String.IsNullOrEmpty(FirstName))
                        {
                            if (firstLastNameRequired)
                                errorMessage = "First Name is required";
                        }
                        break;
                    case "LastName":
                        if (String.IsNullOrEmpty(LastName))
                        {
                            if (firstLastNameRequired)
                                errorMessage = "Last Name is required";
                        }
                        break;
                    case "DOB":
                        if (String.IsNullOrEmpty(dob))
                        {
                            if (dobAndGenderRequired)
                                errorMessage = "Date of Birth is required";
                        }
                        break;
                    case "Gender":
                        if (String.IsNullOrEmpty(gender))
                        {
                            if (dobAndGenderRequired)
                                errorMessage = "Gender is required";
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
            else if (Password != ConfirmPassword)
            {
                return "Both Passwords should match";
            }

            return null;
        }

        string ValidateUserName()
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (!regexItem.IsMatch(UserName))
            {
                return "Invalid UserName";
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
            return String.IsNullOrEmpty(value) || value.Trim() == String.Empty;
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
