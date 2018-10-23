using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;
using deORO.BarcodeScanner;


namespace deORO.ViewModels
{
    class NewUserViewModel : BaseViewModel
    {
        private User user = null;



        private decimal accountBalance;

        public decimal AccountBalance
        {
            get { return accountBalance; }
            set { accountBalance = value; }
        }

        public User User
        {
            get { return user; }
            set { user = value; }
        }


        private bool barcodeVisible = false;

        public bool BarcodeVisible
        {
            get { return barcodeVisible; }
            set { barcodeVisible = value; RaisePropertyChanged(() => BarcodeVisible); }
        }

        private bool dobAndGenderRequiredVisible = false;

        public bool DOBAndGenderRequiredVisible
        {
            get { return dobAndGenderRequiredVisible; }
            set { dobAndGenderRequiredVisible = value; RaisePropertyChanged(() => DOBAndGenderRequiredVisible); }
        }


        private deOROMembershipProvider membership = new deOROMembershipProvider();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand RegisterNewUserCommand { get { return new DelegateCommand(ExecuteRegisterNewUserCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        public ICommand BarcodeCommand { get { return new DelegateCommand(ExecuteBarcodeCommand); } }
        private IBarcodeScanner barcodeScanner = BarcodeScanner.BarcodeScannerFactory.GetBarcodeScanner();


        public override void Init()
        {
            DOBAndGenderRequiredVisible = Global.DOBAndGenderRequired;
            BarcodeVisible = Global.BarcodeLogin;

            base.Init();
        }

        private void ExecuteBarcodeCommand()
        {
            barcodeScanner.Open("Local");
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Subscribe(ProcessBarcode);
        }

        private void ProcessBarcode(object obj)
        {
            User.Barcode = obj.ToString();
        }

        private void ExecuteRegisterNewUserCommand()
        {
            if (user.Barcode != null && user.Barcode.Trim() == "")
                user.Barcode = null;

            string UserDateString = "";
            DateTime UserDateTime ;

            try
            {
               UserDateString = @user.DOB;

                UserDateTime = Convert.ToDateTime(UserDateString,
                    System.Globalization.CultureInfo.GetCultureInfo("en-US").DateTimeFormat);
            }
            catch 
            {
                UserDateTime = DateTime.Today;
            }
       
            

            System.Web.Security.MembershipCreateStatus createStatus;
            deOROMembershipUser newUser = (deOROMembershipUser)membership.CreateUser(user.FirstName, user.LastName, user.UserName,
                                                                                     user.Password, user.Email, UserDateTime,
                                                                                     user.Gender, "", "", true,
                                                                                     Guid.NewGuid().ToString(), -Global.CreditToAccount, 0, user.Barcode,
                                                                                     out createStatus);

            if (createStatus == System.Web.Security.MembershipCreateStatus.DuplicateEmail)
            {
                DialogViewService.ShowAutoCloseDialog("New User", "Email already exists. Please re-enter Email");
            }
            else if (createStatus == System.Web.Security.MembershipCreateStatus.DuplicateUserName)
            {
                DialogViewService.ShowAutoCloseDialog("New User", "UserName already exists. Please re-enter UserName");
            }
            else if (createStatus == System.Web.Security.MembershipCreateStatus.UserRejected)
            {
                DialogViewService.ShowAutoCloseDialog("New User", "Barcode already exists. Please re-enter Barcode");
            }
            else
            {
                aggregator.GetEvent<EventAggregation.UserRegistrationAddEvent>().Publish(newUser);
            }
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.UserRegistrationCancelEvent>().Publish(null);
        }

        public override void Dispose()
        {
            if (barcodeScanner != null)
            {
                barcodeScanner.Close();
            }
            Global.InvalidUserBarcode = "";
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Unsubscribe(ProcessBarcode);
        }
    }
}
