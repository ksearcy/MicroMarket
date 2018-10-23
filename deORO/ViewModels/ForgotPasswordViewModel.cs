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

namespace deORO.ViewModels
{
    class ForgotPasswordViewModel : BaseViewModel
    {

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand SendPasswordCommand { get { return new DelegateCommand(ExecuteSendPasswordCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        private deOROMembershipProvider membership = new deOROMembershipProvider();

        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; RaisePropertyChanged(() => Email); }
        }


        public override void Init()
        {
            Email = "";
            base.Init();
        }

        private void ExecuteSendPasswordCommand()
        {
            var user = membership.GetUserByEmail(Email) as deOROMembershipUser;

            if (user != null)
            {
                Global.Email.SendPassword(user.UserName, Email, membership.ResetPassword(user.UserName, ""));
            }

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

    }
}
