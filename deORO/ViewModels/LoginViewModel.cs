using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;
using WPFLocalizeExtension.Extensions;
using System.Reflection;
using deORO.BarcodeScanner;
//using HidGlobal.OK.Readers;
//using HidGlobal.OK.Readers.AViatoR.Components;
//using HidGlobal.OK.Readers.Components;
//using HidGlobal.OK.SampleCodes.Utilities;
using deORO.Card;

namespace deORO.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        UserRepository UserRepository = new UserRepository();

        User user = new User();

        private deOROMembershipProvider membership = new deOROMembershipProvider();
        private static ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel();

        private FingerprintReader.DigitalPersona reader = null;
        private ContactclessReader.HIDCard hidreader = null;
        private IBarcodeScanner barcodeScanner = BarcodeScanner.BarcodeScannerFactory.GetBarcodeScanner();

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(() => UserName); }
        }

        public string UserName
        {
            get { return user.UserName; }
            set
            {
                user.UserName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public string Password
        {
            get { return user.Password; }
            set
            {
                user.Password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        private bool enableCancel = true;

        public bool EnableCancel
        {
            get { return enableCancel; }
            set { enableCancel = value; RaisePropertyChanged(() => EnableCancel); }
        }

        private bool barcodeVisible;

        public bool BarcodeVisible
        {
            get { return barcodeVisible; }
            set { barcodeVisible = value; RaisePropertyChanged(() => BarcodeVisible); }
        }

        private bool barcodeChecked;

        public bool BarcodeChecked
        {
            get
            {
                if (barcodeChecked)
                {
                    Title = LocalizationProvider.GetLocalizedValue<string>("Login.Title2");
                }
                return barcodeChecked;
            }
            set
            {
                barcodeChecked = value;
                RaisePropertyChanged(() => BarcodeChecked);
            }
        }

        private bool userNamePasswordChecked;

        public bool UserNamePasswordChecked
        {
            get
            {
                if (userNamePasswordChecked)
                {
                    Title = LocalizationProvider.GetLocalizedValue<string>("Login.Title1");
                }
                return userNamePasswordChecked;
            }
            set
            {
                userNamePasswordChecked = value;
                RaisePropertyChanged(() => UserNamePasswordChecked);
            }
        }


        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public ICommand LoginCommand { get { return new DelegateCommand(ExecuteLoginCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        public ICommand UserNamePasswordCommand { get { return new DelegateCommand(ExecuteUserNamePasswordCommand); } }
        public ICommand BarcodeCommand { get { return new DelegateCommand(ExecuteBarcodeCommand); } }
        public ICommand ForgotPasswordCommand { get { return new DelegateCommand(ExecuteForgotPasswordCommand); } }

        private void ExecuteForgotPasswordCommand()
        {
            //IDialogService dialog = new PopupViewService(600, 350);
            DialogViewService.ShowDialog(forgotPasswordViewModel, 600, 350);
        }

        private void ExecuteBarcodeCommand()
        {
            barcodeScanner.Open("Local");
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Unsubscribe(ProcessBarcode);
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Subscribe(ProcessBarcode);
            BarcodeChecked = true;
        }

        private void ExecuteUserNamePasswordCommand()
        {
            barcodeScanner.Open();

            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Unsubscribe(ProcessBarcode);
            UserNamePasswordChecked = true;
        }

        private void HIDLoginSuccessful(object obj)
        {
            Message = "";

            if (membership.ValidateHIDUser(obj.ToString()))
            {
                var user = membership.GetUserByHIDSerialNumber(obj.ToString());

                try
                {
                    membership.LogSuccessfulLogin(user.UserName, deORODataAccessApp.Helpers.Enum.AuthenticationMode.HIDCard);
                    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish((user as deOROMembershipUser));
                }
                catch
                {
                    membership.LogSuccessfulLogin(user.UserName, deORODataAccessApp.Helpers.Enum.AuthenticationMode.HIDCard);
                    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish((user as deOROMembershipUser));
                }



            }
            else
            {
               
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new EventAggregation.TransactionErrorEventArgs
                {
                    DeviceType = Helpers.Enum.DeviceType.Login,
                    Event = "Login Failed. Invalid Card",
                    ShoppingCartPkid = Helpers.Enum.DeviceType.HIDReader.ToString(),
                    Code = "HID"
                });
            }



            //try
            //{
            //    Message = "";
            //    membership.LogSuccessfulLogin(parameter.ToString(), deORODataAccessApp.Helpers.Enum.AuthenticationMode.HIDCard);
            //    deOROMembershipUser deOROUser = membership.GetUser(parameter.ToString()) as deOROMembershipUser;
            //    deOROUser.LoginMethod = "HIDCard";
            //    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish(deOROUser);
            //}
            //catch
            //{

            //    Message = "";
            //    membership.LogSuccessfulLogin(parameter.ToString(), deORODataAccessApp.Helpers.Enum.AuthenticationMode.HIDCard);
            //    deOROMembershipUser deOROUser = membership.GetUser(parameter.ToString()) as deOROMembershipUser;
            //    deOROUser.LoginMethod = "HIDCard";
            //    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish(deOROUser);

            //}

        }

        public override void Init()
        {

            if (Global.EnableFingerprintAuthentication)
            {
                reader = new FingerprintReader.DigitalPersona();
                reader.Init();
                aggregator.GetEvent<EventAggregation.FingerPrintLoginSuccessfulEvent>().Subscribe(FingerPrintLoginSuccessful);
            }

            if (Global.EnableCardAuthentication)
            {               
                hidreader = new ContactclessReader.HIDCard();           
                hidreader.Init();
                aggregator.GetEvent<EventAggregation.HidCardLoginSuccessfulEvent>().Subscribe(HIDLoginSuccessful);
            }

            if (Global.RunMode.ToLower() == "debug")
            {
                UserName = "deORO";
                Password = "999999";
            }
                        
            BarcodeVisible = Global.BarcodeLogin;
            UserNamePasswordChecked = true;

            if (WPFLocalizeExtension.Engine.LocalizeDictionary.Instance != null)
            {
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.PropertyChanged += Instance_PropertyChanged;
            }

            base.Init();
        }

        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("BarcodeChecked");
            RaisePropertyChanged("UserNamePasswordChecked");
        }

        private void ProcessBarcode(object obj)
        {
            Message = "";

            if (membership.ValidateUser(obj.ToString()))
            {
                var user = membership.GetUserByBarcode(obj.ToString());
                
                try
                {                    
                    membership.LogSuccessfulLogin(user.UserName, deORODataAccessApp.Helpers.Enum.AuthenticationMode.Barcode);
                    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish((user as deOROMembershipUser));
                }
                catch
                {
                    membership.LogSuccessfulLogin(user.UserName, deORODataAccessApp.Helpers.Enum.AuthenticationMode.Barcode);
                    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish((user as deOROMembershipUser));
                }

            }
            else
            {
                //aggregator.GetEvent<EventAggregation.LoginFailEvent>().Publish(deORO.Helpers.Enum.AuthenticationMode.Barcode);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new EventAggregation.TransactionErrorEventArgs
                {
                    DeviceType = Helpers.Enum.DeviceType.Login,
                    Event = "Login Failed. Invalid Barcode",
                    ShoppingCartPkid = Helpers.Enum.DeviceType.BarcodeScanner.ToString(),
                    Code = "ZLN"
                });
            }
        }

        private void ExecuteLoginCommand()
        {
            Message = "";

            if (membership.ValidateUser(user.UserName, user.Password))
            {
                try
                {

                    user User = UserRepository.GetUserByUsername(user.UserName);
                    membership.LogSuccessfulLogin(user.UserName, deORODataAccessApp.Helpers.Enum.AuthenticationMode.UserCredentials);
                    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish(membership.GetUser(user.UserName) as deOROMembershipUser);

                }
                catch
                {
                    membership.LogSuccessfulLogin(user.UserName, deORODataAccessApp.Helpers.Enum.AuthenticationMode.UserCredentials);
                    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish(membership.GetUser(user.UserName) as deOROMembershipUser);
                }

            }
            else
            {
                membership.LogFailedLogin(user.UserName, deORODataAccessApp.Helpers.Enum.AuthenticationMode.UserCredentials);
                //aggregator.GetEvent<EventAggregation.LoginFailEvent>().Publish(deORO.Helpers.Enum.AuthenticationMode.UserCredentials);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(
                 new EventAggregation.TransactionErrorEventArgs
                 {

                     DeviceType = Helpers.Enum.DeviceType.Login,
                     Event = "Login Failed. Invalid UserName or Password",
                     ShoppingCartPkid = "User Credentials",
                     Code = "ZLN"
                 });
            }
        }

        private void FingerPrintLoginSuccessful(object parameter)
        {

            try
            {
                Message = "";
                membership.LogSuccessfulLogin(parameter.ToString(), deORODataAccessApp.Helpers.Enum.AuthenticationMode.FingerPrint);
                deOROMembershipUser deOROUser = membership.GetUser(parameter.ToString()) as deOROMembershipUser;
                deOROUser.LoginMethod = "FingerPrint";
                aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish(deOROUser);
            }
            catch
            {

                Message = "";
                membership.LogSuccessfulLogin(parameter.ToString(), deORODataAccessApp.Helpers.Enum.AuthenticationMode.FingerPrint);
                deOROMembershipUser deOROUser = membership.GetUser(parameter.ToString()) as deOROMembershipUser;
                deOROUser.LoginMethod = "FingerPrint";
                aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish(deOROUser);

            }

        }


        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.LoginCancelEvent>().Publish(null);
        }

        public override void Dispose()
        {
            barcodeScanner.Close();
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Unsubscribe(ProcessBarcode);

            if (Global.EnableFingerprintAuthentication)
            {
                aggregator.GetEvent<EventAggregation.FingerPrintLoginSuccessfulEvent>().Unsubscribe(FingerPrintLoginSuccessful);

                if (reader != null)
                {
                    reader.Dispose();
                }
            }

            if (Global.EnableCardAuthentication)
            {
                aggregator.GetEvent<EventAggregation.HidCardLoginSuccessfulEvent>().Unsubscribe(HIDLoginSuccessful);

                if (hidreader != null)
                {
                    hidreader.Dispose();
                }
            }

            try
            {
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.PropertyChanged -= Instance_PropertyChanged;
            }
            catch { }

            UserName = "";
            Password = "";
        }
    }
}
