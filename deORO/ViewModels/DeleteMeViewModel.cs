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
    public class DeleteMeViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();


        private string accountBalance;
        public string AccountBalance
        {
            get { return accountBalance; }
            set
            {
                accountBalance = value;
                RaisePropertyChanged(() => AccountBalance);
            }
        }

        private string email;
        public string Email
        {
            get
            {
                email = Global.User.Email;
                return email;
            }
            set { email = value; }
        }
        private string firstName;

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; RaisePropertyChanged(() => FirstName); }
        }
        private string lastName;

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; RaisePropertyChanged(() => LastName); }
        }
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; RaisePropertyChanged(() => Address); }
        }
        private string city;

        public string City
        {
            get { return city; }
            set { city = value; RaisePropertyChanged(() => City); }
        }
        private string state;

        public string State
        {
            get { return state; }
            set { state = value; RaisePropertyChanged(() => State); }
        }
        private string zip;

        public string Zip
        {
            get { return zip; }
            set { zip = value; RaisePropertyChanged(() => Zip); }
        }
        private string phone;

        public string Phone
        {
            get { return phone; }
            set { phone = value; RaisePropertyChanged(() => Phone); }
        }


        public ICommand DeleteCommand { get { return new DelegateCommand(ExecuteDeleteCommand); } }

        private IDialogService dialogService;

        public IDialogService DialogService
        {
            get { return dialogService; }
            set { dialogService = value; }
        }

        public override void Init()
        {
            //DialogService = new MessageBoxViewService();
            AccountBalance = Global.User.AccountBalance.ToString("C2");

            base.Init();
        }


        private void ExecuteDeleteCommand()
        {
            DialogViewService.Show("Delete Confirmation", "Are you sure you want to delete?",
                (DialogResult result) =>
                {
                    if (result.Result.Value)
                    {
                        DeleteMeRepository rep = new DeleteMeRepository();
                        decimal accountBalance = Global.User.AccountBalance;

                        if (!rep.Add(FirstName, LastName, Email, Address, City, State, Zip, Phone,
                                    Global.User.AccountBalance, Global.User.ProviderUserKey.ToString(), Global.User.UserId))
                        {
                            aggregator.GetEvent<EventAggregation.DeleteMeFailedEvent>().Publish(null);
                            //throw new Exception("Unable to Perform Delete");
                        }
                        else
                        {
                            Global.Email.SendUserDeleted(accountBalance, firstName, lastName, address, city, state, zip, phone);
                            DialogViewService.ShowAutoCloseDialog("Delete Confirmation", "Your account has been deleted successfully.");
                            aggregator.GetEvent<EventAggregation.DeleteMeCompletedEvent>().Publish(null);
                        }
                    }

                });
        }
    }
}
