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
using deORODataAccessApp.Models;

namespace deORO.ViewModels
{
    public class PasswordViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand ChangePasswordCommand { get { return new DelegateCommand(ExecuteChangePasswordCommand); } }

        private deOROMembershipProvider membership = new deOROMembershipProvider();
        //private IDialogService dialog = new MessageBoxViewService();

        private User user = null;

        public User User
        {
            get { return user; }
            set
            {
                user = value;
                RaisePropertyChanged(() => User);
            }
        }

        private void ExecuteChangePasswordCommand()
        {
            if (membership.ChangePassword(Global.User.UserName, "", user.Password))
            {
                User.Password = "";
                User.ConfirmPassword = "";

                Global.User = membership.GetUser(Global.User.UserName) as deOROMembershipUser;
                Global.Email.SendPasswordChanged();
                DialogViewService.ShowAutoCloseDialog("Change Password", "Password was changed successfully.");
            }
            else
            {
                DialogViewService.ShowAutoCloseDialog("Change Password", "Unable to update password. Please retry.");
            }

        }
    }
}
