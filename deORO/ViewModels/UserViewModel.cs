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
    public class UserViewModel : BaseViewModel
    {
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        public ICommand SaveCommand { get { return new DelegateCommand(ExecuteSaveCommand); } }

        private user user;

        public user User
        {
            get { return user; }
            set
            {
                user = value;
                RaisePropertyChanged(() => User);
            }
        }
        private UserRepository repo = new UserRepository();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public UserViewModel()
        {
            user = new user();
        }

        public UserViewModel(int userId)
        {
            user = new user();
            user = repo.GetUser(userId);
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        private void ExecuteSaveCommand()
        {
            if (user.id == 0)
            {
                if (repo.AddUser(user))
                {
                    aggregator.GetEvent<EventAggregation.UserAddCompleteEvent>().Publish(null);
                }
                else
                {
                    aggregator.GetEvent<EventAggregation.UserAddFailEvent>().Publish(null);
                }
            }
            else
            {
                if (repo.UpdateUser(user))
                {
                    aggregator.GetEvent<EventAggregation.UserUpdateCompleteEvent>().Publish(null);
                }
                else
                {
                    aggregator.GetEvent<EventAggregation.UserUpdateFailEvent>().Publish(null);
                }
            }

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }
    }
}
