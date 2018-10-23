using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class NavigationViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public ICommand LoginCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.ShowLoginEvent>().Publish(null); }); } }
        public ICommand MyAccountCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.ShowMyAccountEvent>().Publish(null); }); } }
        public ICommand ShoppingCartCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.ShowShoppingCartEvent>().Publish(null); }); } }
        public ICommand RegisterUserCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.ShowRegisterUserEvent>().Publish(null); }); } }
        public ICommand LogoutCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.UserLogoutEvent>().Publish(null); }); } }
        public ICommand HelpCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.ShowHelpEvent>().Publish(null); }); } }
        public ICommand RefillButtonCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.ShowRefillEvent>().Publish(null); }); } }

        private int rows;

        public int Rows
        {
            get { return rows; }
            set { rows = value; RaisePropertyChanged(() => Rows); }
        }

        private bool rightToolBarEnable = true;
        public bool RightToolBarEnable
        {
            get { return rightToolBarEnable; }
            set { rightToolBarEnable = value; RaisePropertyChanged(() => RightToolBarEnable); }
        }


        private bool shoppingCartVisible = true;
        public bool ShoppingCartVisible
        {
            get { return shoppingCartVisible; }
            set
            {
                shoppingCartVisible = value;
                RaisePropertyChanged(() => ShoppingCartVisible);
            }
        }

        private bool logoutVisible = false;
        public bool LogoutVisible
        {
            get { return logoutVisible; }
            set
            {
                logoutVisible = value;
                RaisePropertyChanged(() => LogoutVisible);
            }
        }

        private bool refillButtonVisible = false;
        public bool RefillButtonVisible
        {
            get { return refillButtonVisible; }
            set
            {
                refillButtonVisible = value;
                RaisePropertyChanged(() => RefillButtonVisible);
            }
        }

        private bool loginVisible = true;

        public bool LoginVisible
        {
            get { return loginVisible; }
            set { loginVisible = value; RaisePropertyChanged(() => LoginVisible); }
        }

        private bool userRegistrationVisible = true;
        public bool UserRegistrationVisible
        {
            get { return userRegistrationVisible; }
            set
            {
                userRegistrationVisible = value;
                RaisePropertyChanged(() => UserRegistrationVisible);
            }
        }


        private bool myAccountVisible = false;
        public bool MyAccountVisible
        {
            get { return myAccountVisible; }
            set
            {
                myAccountVisible = value;
                RaisePropertyChanged(() => MyAccountVisible);
            }
        }


        private string myAccountText;
        public string MyAccountText
        {
            get { return myAccountText; }
            set
            {
                myAccountText = value;
                RaisePropertyChanged(() => MyAccountText);
            }
        }

        public override void Init()
        {
            if (Helpers.Global.Orientation.ToLower().Equals("landscape") || Helpers.Global.Orientation.ToLower().Equals("hide"))
            {
                if (Helpers.Global.Orientation.ToLower().Equals("hide"))
                {
                    ShoppingCartVisible = false;
                }
                Rows = 1;
            }
            else
            {
                Rows = 4;
            }

            aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Subscribe(LoggedIn);
            aggregator.GetEvent<EventAggregation.LogoutCompleteEvent>().Subscribe(Loggedout);
            aggregator.GetEvent<EventAggregation.UserRegistrationCompleteEvent>().Subscribe(UserRegistrationComplete);
            aggregator.GetEvent<EventAggregation.FingerPrintNewUserEvent>().Subscribe(UserRegistrationFingerPrint);
            aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Subscribe(ProcessToolBarEnabledEvent);


            base.Init();
        }

        private void UserRegistrationFingerPrint(object parameter = null)
        {
            MyAccountVisible = true;
            LogoutVisible = true;
            RefillButtonVisible = true;
            LoginVisible = false;
            UserRegistrationVisible = false;
        }

        private void UserRegistrationComplete(object parameter = null)
        {
            MyAccountVisible = true;
            LogoutVisible = true;
            RefillButtonVisible = true;
            LoginVisible = false;
            UserRegistrationVisible = false;
        }


        private void LoggedIn(deORODataAccessApp.DataAccess.deOROMembershipUser obj)
        {
            MyAccountVisible = true;
            LogoutVisible = true;
            RefillButtonVisible = true;
            LoginVisible = false;
            UserRegistrationVisible = false;

            MyAccountText = obj.UserName;
        }

        private void Loggedout(object obj)
        {
            MyAccountVisible = false;
            LogoutVisible = false;
            RefillButtonVisible = false;
            LoginVisible = true;
            UserRegistrationVisible = true;
            MyAccountText = "";
        }



        private void ProcessToolBarEnabledEvent(object obj)
        {
            RightToolBarEnable = Convert.ToBoolean(obj);
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
