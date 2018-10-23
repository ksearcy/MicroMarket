using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Input;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;

namespace deORO.ViewModels
{
    public class EmailViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand ChangeEmailCommand { get { return new DelegateCommand(ExecuteChangeEmailCommand); } }

        private deOROMembershipProvider membership = new deOROMembershipProvider();
        //private IDialogService dialog = new MessageBoxViewService();

        private string email;

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged(() => Email);
            }
        }

        private string newEmail;

        public string NewEmail
        {
            get { return newEmail; }
            set
            {
                newEmail = value;
                RaisePropertyChanged(() => NewEmail);
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

        public EmailViewModel()
        {
            if (Helpers.Global.User != null)
                Email = Helpers.Global.User.Email;
        }

        private void ExecuteChangeEmailCommand()
        {
            MembershipCreateStatus status;
            membership.UpdateEmail(Global.User.UserName, NewEmail, out status);

            if (status == MembershipCreateStatus.DuplicateEmail)
            {
                DialogViewService.ShowAutoCloseDialog("Change Email", "Email address already exists. Please retry.");
            }
            else if (status == MembershipCreateStatus.ProviderError)
            {
                DialogViewService.ShowAutoCloseDialog("Change Email", "Unable to update your email address. Please retry.");
            }
            else if (status == MembershipCreateStatus.Success)
            {
                Global.User = membership.GetUser(Global.User.UserName) as deOROMembershipUser;
                Email = Global.User.Email;
                NewEmail = "";
                Global.Email.SendEmailChanged();
                DialogViewService.ShowAutoCloseDialog("Change Email", "Email address was changed successfully.");
            }

        }
    }
}
