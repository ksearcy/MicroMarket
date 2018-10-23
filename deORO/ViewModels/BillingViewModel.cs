using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.EventAggregation;
using deORO.Helpers;
using deORODataAccessApp.Models;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class BillingViewModel : BaseViewModel
    {
        UserRepository UserRepository = new UserRepository();

        private BaseViewModel currentViewModel;

        readonly static CashRefillViewModel cash = new CashRefillViewModel();
        readonly static CreditCardRefilViewModel creditCard = new CreditCardRefilViewModel();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public ICommand CashCommand { get { return new DelegateCommand(ExecuteCashCommand); } }
        public ICommand CreditCardCommand { get { return new DelegateCommand(ExecuteCreditCommand, () => { return CanExecuteCreditCard; }); } }


        private decimal accountBalance;
        private bool creditCardChecked;
        private decimal creditAddRefill;
        //private IDialogService dialog = new MessageBoxViewService();

        private ItemRepository itemRepo = new ItemRepository();
        private item item = null;
        private bool canExecuteCreditCard = true;

        public bool CanExecuteCreditCard
        {
            get { return canExecuteCreditCard; }
            set { canExecuteCreditCard = value; RaisePropertyChanged(() => CanExecuteCreditCard); }
        }

        public bool CreditCardChecked
        {
            get { return creditCardChecked; }
            set
            {
                creditCardChecked = value;
                RaisePropertyChanged(() => CreditCardChecked);
            }
        }
        private bool cashChecked;

        public bool CashChecked
        {
            get { return cashChecked; }
            set
            {
                cashChecked = value;
                RaisePropertyChanged(() => CashChecked);
            }
        }

        private bool cashPaymentVisible = false;

        public bool CashPaymentVisible
        {
            get { return cashPaymentVisible; }
            set { cashPaymentVisible = value; RaisePropertyChanged(() => CashPaymentVisible); }
        }

        private bool creditCardPaymentVisible = false;

        public bool CreditCardPaymentVisible
        {
            get { return creditCardPaymentVisible; }
            set { creditCardPaymentVisible = value; RaisePropertyChanged(() => CreditCardPaymentVisible); }
        }        

        public decimal AccountBalance
        {
            get
            {
                return accountBalance;
            }
            set
            {
                accountBalance = value;
                RaisePropertyChanged(() => AccountBalance);
            }
        }

        public override void Init()
        {
            if (Global.User != null)

                try
                {
                    decimal ActualAccountBalance = Convert.ToDecimal(UserRepository.GetUserByUsername(Global.User.UserName).account_balance, System.Globalization.CultureInfo.InvariantCulture);
                    Global.User.AccountBalance = ActualAccountBalance;
                }
                catch { }

            AccountBalance = Global.User.AccountBalance;

            aggregator.GetEvent<EventAggregation.CashRefillCompleteEvent>().Subscribe(CashRefillComplete);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionCompleteEvent>().Subscribe(CreditCardRefillComplete);

            aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Subscribe(ResetView);
            aggregator.GetEvent<EventAggregation.CreditCardRefilCancelEvent>().Subscribe(ResetView);
            aggregator.GetEvent<EventAggregation.CashRefilCancelEvent>().Subscribe(ResetView);
            aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Subscribe(RightToolBarEnable);

            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.BillRefill.ToString()) || 
                Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.CoinRefill.ToString()))
                CashPaymentVisible = true;

            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.CreditCardRefill.ToString()))
                CreditCardPaymentVisible = true;

            base.Init();
        }

        private void RightToolBarEnable(object obj)
        {
            CanExecuteCreditCard = Convert.ToBoolean(obj);
        }

        private void CreditCardRefillComplete(decimal parameter)
        {
            List<PaymentItem> items = new List<PaymentItem>();
            items.Add(new PaymentItem
            {
                DateTime = DateTime.Now,
                Payment = parameter,
                Source = Helpers.Enum.PaymentMethod.CreditCardRefill.ToString()
            });
            ProcessRefill(new PaymentCompleteEventArgs()
            {
                Source = Helpers.Enum.PaymentMethod.CreditCardRefill.ToString(),
                PaymentItems = items
            });
        }

        private void CashRefillComplete(PaymentCompleteEventArgs args)
        {
            ProcessRefill(args);
        }

        private void ResetView(object parameter = null)
        {
            CurrentViewModel = null;
            CashChecked = false;
            CreditCardChecked = false;
            aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Publish(true);
        }

        private void ProcessRefill(PaymentCompleteEventArgs args)
        {

            if (item == null)
                item = itemRepo.GetItem("ACCOUNT_REFILL_BARCODE");

            if (item != null)
            {
                decimal refillAmount = args.PaymentItems.Sum(x => x.Payment);
                decimal reward = Helpers.RewardHelper.GetRewardAmount(refillAmount);

                if (Global.EnableRefillRewards)
                {
                    args.PaymentItems.Add(new PaymentItem()
                    {
                        DateTime = DateTime.Now,
                        Payment = reward,
                        Source = Helpers.Enum.PaymentMethod.Reward.ToString()
                    });

                }
                if (Global.CreditAddRefill > 0)
                {
                    args.PaymentItems.Add(new PaymentItem()
                    {
                        DateTime = DateTime.Now,
                        Payment = Global.CreditAddRefill,
                        Source = Helpers.Enum.PaymentMethod.CreditAddRefill.ToString()
                    });
                }

                ShoppingCartRepository shoppingRepo = new ShoppingCartRepository();
                AccountBalanceHistoryRepository accountHistoryRepo = new AccountBalanceHistoryRepository();
                deOROMembershipProvider userProvider = new deOROMembershipProvider();

                ShoppingCartItem cartItem = new ShoppingCartItem();
                cartItem.Guid = Guid.NewGuid().ToString();
                cartItem.Id = item.id;
                cartItem.BarCode = item.barcode;
                cartItem.Name = item.name;
                cartItem.Price = refillAmount + reward;
                cartItem.Tax = 0;
                cartItem.PriceTaxIncluded = refillAmount + reward;

                List<ShoppingCartItem> cartItems = new List<ShoppingCartItem>();
                cartItems.Add(cartItem);

                string cartpkid = shoppingRepo.SaveShoppingCart(cartItems, args.PaymentItems, args.Error, Global.User.ProviderUserKey.ToString());

                try
                {
                    deOROSyncData.SyncData syncData = new deOROSyncData.SyncData();
                    syncData.Init();
                    Task.Factory.StartNew(() =>
                    {
                        syncData.UploadShoppingCartDetails(cartpkid);
                    });
                }
                catch { }

                Global.PreviousAccountBalanceForPrint = Global.User.AccountBalance;
                Global.RefillAmountForPrint = refillAmount + reward;
                //userProvider.AddToUserBalance(Global.User.UserName, args.PaymentItems.Sum(x => x.Payment),"Refill");
                userProvider.UpdateUserBalance(Global.User.UserName, refillAmount + reward, "Refill");

                Global.User = userProvider.GetUser(Global.User.UserName) as deOROMembershipUser;
                accountHistoryRepo.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance, refillAmount + reward, args.Source, cartpkid);
                AccountBalance = Global.User.AccountBalance;
                Global.Email.SendAccountRefilled(refillAmount, reward, args.Source);
                Global.DialogTypeForPrint = "Account Refill";
                DialogViewService.ShowDialog("Account Refill", "Account refill was successful.");
            }
            else
            {
                DialogViewService.ShowDialog("Account Refill", "Account refill failed. ACCOUNT_REFILL_BARCODE is missing");
            }

            ResetView();
        }

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
                //if (value == null)
                //{
                //    currentViewModel = BillingViewModel.cash;
                //}
                //else
                //{
                //    currentViewModel = value;
                //}
                RaisePropertyChanged(() => CurrentViewModel);
            }
        }


        private void ExecuteCashCommand()
        {
            if (item == null)
                item = itemRepo.GetItem("ACCOUNT_REFILL_BARCODE");

            if (item != null)
            {
                CashChecked = true;
                CreditCardChecked = false;
                CurrentViewModel = cash;
            }
            else
            {
                DialogViewService.ShowAutoCloseDialog("Account Refill", "ACCOUNT_REFILL_BARCODE is missing");
            }
        }


        private void ExecuteCreditCommand()
        {
            if (item == null)
                item = itemRepo.GetItem("ACCOUNT_REFILL_BARCODE");

            if (item != null)
            {
                CashChecked = false;
                CreditCardChecked = true;
                CurrentViewModel = creditCard;
            }
            else
            {
                DialogViewService.ShowAutoCloseDialog("Account Refill", "ACCOUNT_REFILL_BARCODE is missing");
            }
        }

        public override void Dispose()
        {
            aggregator.GetEvent<EventAggregation.CashRefillCompleteEvent>().Unsubscribe(CashRefillComplete);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionCompleteEvent>().Unsubscribe(CreditCardRefillComplete);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Unsubscribe(ResetView);
            aggregator.GetEvent<EventAggregation.CreditCardRefilCancelEvent>().Unsubscribe(ResetView);
            aggregator.GetEvent<EventAggregation.CashRefilCancelEvent>().Unsubscribe(ResetView);
            aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Unsubscribe(RightToolBarEnable);

        }
    }
}
