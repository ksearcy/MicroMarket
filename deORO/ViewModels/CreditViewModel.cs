using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    class CreditViewModel : BaseViewModel
    {
        CreditActivityRepository repo = new CreditActivityRepository();
        List<CreditActivity> items = new List<CreditActivity>();

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand ApplyCommand { get { return new DelegateCommand(ExecuteApplyCommand, CanExecuteApplyCommand); } }

        private bool CanExecuteApplyCommand()
        {
            if (Items != null)
            {
                return Convert.ToBoolean(Items.Count(x => x.Selected == true));
            }
            else
            {
                return false;
            }
        }

        private void ExecuteApplyCommand()
        {
            AccountBalanceHistoryRepository repo1 = new AccountBalanceHistoryRepository();
            CreditActivityRepository repo2 = new CreditActivityRepository();
            deOROMembershipProvider userProvider = new deOROMembershipProvider();
            decimal TotalCreditAmount = 0;

            foreach (var item in Items)
            {
                if (Global.AutoCredit == true)
                {
                    item.Selected = true;
                }

                if (item.Selected)
                {
                    TotalCreditAmount += item.Amount;
                    userProvider.UpdateUserBalance(Global.User.UserName, item.Amount, "Reward Claimed");
                    Global.User = userProvider.GetUser(Global.User.UserName) as deOROMembershipUser;

                    repo1.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance + item.Amount, item.Amount,"Credit Redemption");

                    credit_activity activity = repo2.GetCreditActivity(item.Id);
                    if (activity != null)
                    {
                        activity.credit_claimed = 1;
                        activity.credit_claimed_date = DateTime.Now;
                        repo2.Edit(activity);
                    }
                }
            }

            repo2.Save();           
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

            DialogViewService.ShowAutoCloseDialog("Credits", "Your account has been credited with " + TotalCreditAmount.ToString("c"));
        

        }

        public ICommand CancelCommand { get { return new DelegateCommand(() => { aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null); }); } }


        public List<CreditActivity> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(() => Items); }
        }

        public override void Init()
        {
            Items = repo.GetAll(((Helpers.Global.User) as deOROMembershipUser).ProviderUserKey.ToString());

            if (Items.Count == 0)
            {
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
            }
            else {
                if (Global.AutoCredit == true)
                {
                    ExecuteApplyCommand();
                }
            }

            base.Init();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
