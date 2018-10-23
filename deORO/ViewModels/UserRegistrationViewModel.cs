using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    class UserRegistrationViewModel : BaseViewModel
    {
        private BaseViewModel currentViewModel;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public BaseViewModel CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }
            set
            {
                if (currentViewModel == value)
                    return;

                currentViewModel = value;
                RaisePropertyChanged(() => CurrentViewModel);
            }
        }

        readonly static NewUserViewModel newUser = new NewUserViewModel();
        readonly static FingerPrintViewModel fingerPrint = new FingerPrintViewModel();

        public ICommand RegisterNewUserCommand { get { return new DelegateCommand(ExecuteRegisterNewUserCommand); } }

        public override void Init()
        {
            CurrentViewModel = UserRegistrationViewModel.newUser;

            aggregator.GetEvent<EventAggregation.UserRegistrationAddEvent>().Subscribe(UserAdd);
            aggregator.GetEvent<EventAggregation.FingerPrintSaveSuccessfulEvent>().Subscribe(FingerPrint);
            aggregator.GetEvent<EventAggregation.FingerPrintCancelEvent>().Subscribe(FingerPrint);
              

            base.Init();
        }

        private void ExecuteRegisterNewUserCommand()
        {
            CurrentViewModel = UserRegistrationViewModel.newUser;
        }

        private void FingerPrint(object parameter = null)
        {
            aggregator.GetEvent<EventAggregation.UserRegistrationCompleteEvent>().Publish(Global.User.UserName);
        }

        private void UserAdd(deOROMembershipUser user)
        {
            if (user != null)
            {
                Global.User = user;

                if (Helpers.Global.NewUserCredit != 0m)
                {
                    deOROMembershipProvider userProvider = new deOROMembershipProvider();
                    userProvider.UpdateUserBalance(Global.User.UserName, Helpers.Global.NewUserCredit, "New User Credit");
                    Global.User = userProvider.GetUser(Global.User.UserName) as deOROMembershipUser;

                    AccountBalanceHistoryRepository accountHistoryRepo = new AccountBalanceHistoryRepository();
                    accountHistoryRepo.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance, Helpers.Global.NewUserCredit, "New User Credit");
                }

                try
                {
                    Global.Email.SendUserCreated();
                }
                catch { }

                if (Global.EnableFingerprintAuthentication)
                {
                    CurrentViewModel = UserRegistrationViewModel.fingerPrint;
                    aggregator.GetEvent<EventAggregation.FingerPrintNewUserEvent>().Publish(Global.User.UserName);
                }
                else
                {
                    aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
                    aggregator.GetEvent<EventAggregation.UserRegistrationCompleteEvent>().Publish(Global.User.UserName);
                }
             
            }
        }

        public override void Dispose()
        {
            aggregator.GetEvent<EventAggregation.UserRegistrationAddEvent>().Unsubscribe(UserAdd);
            aggregator.GetEvent<EventAggregation.FingerPrintSaveSuccessfulEvent>().Unsubscribe(FingerPrint);
            aggregator.GetEvent<EventAggregation.FingerPrintCancelEvent>().Unsubscribe(FingerPrint);
        }
    }
}
