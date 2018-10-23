using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        UserRepository repo = new UserRepository();
        private List<user> users;

        public ICommand DeleteUserCommand { get { return new DelegateCommandWithParam(ExecuteDeleteUserCommand); } }
        public ICommand EditUserCommand { get { return new DelegateCommandWithParam(ExecuteEditUserCommand); } }
        public ICommand AddUserCommand { get { return new DelegateCommand(ExecuteAddUserCommand); } }

        //IDialogService popup = new PopupViewService(800,400);
        //IDialogService message = new MessageBoxViewService();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public List<user> Users
        {
            get
            {
                users = repo.GetList();
                return users;
            }
            set
            {
                users = value;
            }
        }

        public UsersViewModel()
        {
            aggregator.GetEvent<EventAggregation.UserAddCompleteEvent>().Subscribe(RefreshUsers);
            aggregator.GetEvent<EventAggregation.UserUpdateCompleteEvent>().Subscribe(RefreshUsers);
            aggregator.GetEvent<EventAggregation.UserAddFailEvent>().Subscribe(delegate(object x) { DialogViewService.ShowAutoCloseDialog("User Add", "Unable to add user record."); });
            aggregator.GetEvent<EventAggregation.UserUpdateFailEvent>().Subscribe(delegate(object x) { DialogViewService.ShowAutoCloseDialog("User Edit", "Unable to edit user record."); });
        }

        private void ExecuteDeleteUserCommand(object obj)
        {
            if (!repo.DeleteUser(Convert.ToInt32(obj)))
            {
                DialogViewService.ShowAutoCloseDialog("Delete User", "Unable to delete user record.");
            }
            else
            {
                RefreshUsers();
            }
        }

        public void RefreshUsers(object obj = null)
        {
            users = repo.GetList();
            RaisePropertyChanged(() => Users);
        }

        private void ExecuteEditUserCommand(object obj)
        {
            UserViewModel vm = new UserViewModel(Convert.ToInt32(obj));
            DialogViewService.Show(vm);
        }

        private void ExecuteAddUserCommand()
        {
            UserViewModel vm = new UserViewModel();
            DialogViewService.Show(vm);
        }

        public override void Dispose()
        {
            aggregator.GetEvent<EventAggregation.UserAddCompleteEvent>().Unsubscribe(RefreshUsers);
            aggregator.GetEvent<EventAggregation.UserUpdateCompleteEvent>().Unsubscribe(RefreshUsers);
        }
    }
}
