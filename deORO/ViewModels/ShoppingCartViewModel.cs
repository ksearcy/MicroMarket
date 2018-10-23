using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Text;
using deORODataAccessApp.Models;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using deORO.USBRelay;
using deORO.EventAggregation;
using System.Threading.Tasks;
using deOROSyncData;

namespace deORO.ViewModels
{
    class ShoppingCartViewModel : BaseViewModel
    {

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private DiscountRepository discountRepo = new DiscountRepository();
        private ComboDiscountRepository comboDiscountRepo = new ComboDiscountRepository();
        private SubsidyRepository subsidyRepo = new SubsidyRepository();

        public ICommand DeleteItemCommand { get { return new DelegateCommandWithParam(ExecuteDeleteItemCommand); } }
        public ICommand AddItemCommand { get { return new DelegateCommandWithParam(ExecuteAddItemCommand); } }
        public ICommand PayCommand { get { return new DelegateCommand(ExecutePayCommand, CanExecutePayCommand); } }
        public ICommand NoBarcodeCommand { get { return new DelegateCommand(ExecuteNobarcodeCommand); } }
        //public ICommand DamageBarCodeCommand { get { return new DelegateCommand(() => { DamageBarCodeVisible = false; }); } }
        public ICommand DamageBarCodeCommand { get { return new DelegateCommand(ExecuteDamageBarCodeCommand); } }


        private ObservableCollection<ShoppingCartItem> items = new ObservableCollection<ShoppingCartItem>();
        //private IDialogService dialogService = new MessageBoxViewService();
        //private IDialogService popupService = new PopupViewService(350, 350);
        private KMTronic kmtronic = new KMTronic();

        readonly static PaymentOptionsViewModel paymentOptions = new PaymentOptionsViewModel();
        readonly static CreditCardPaymentViewModel creditCardPayment = new CreditCardPaymentViewModel();
        readonly static CashPaymentViewModel cashPayment = new CashPaymentViewModel();
        readonly static MyAccountPaymentViewModel myAccountPayment = new MyAccountPaymentViewModel();
        readonly static MyPayrollPaymentViewModel myPayrollPayment = new MyPayrollPaymentViewModel();
        readonly static DispenseChangeViewModel dispenseChange = new DispenseChangeViewModel();
        readonly static ReturnChangeOptionsViewModel returnChange = new ReturnChangeOptionsViewModel();

        private BaseViewModel currentViewModel;
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

        public ObservableCollection<ShoppingCartItem> Items
        {
            get
            {
                return items;
            }
        }

        private decimal totalPrice = 0;
        public decimal TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                RaisePropertyChanged(() => TotalPrice);
            }
        }

        private decimal totalDiscount = 0;
        public decimal TotalDiscount
        {
            get { return totalDiscount; }
            set
            {
                totalDiscount = value;
                RaisePropertyChanged(() => TotalDiscount);
            }
        }

        private decimal totalSubsidy = 0;

        public decimal TotalSubsidy
        {
            get { return totalSubsidy; }
            set { totalSubsidy = value; RaisePropertyChanged(() => TotalSubsidy); }
        }

        private bool damageBarCodeVisible = true;
        public bool DamageBarCodeVisible
        {
            get { return damageBarCodeVisible; }
            set
            {
                damageBarCodeVisible = value;
                RaisePropertyChanged(() => DamageBarCodeVisible);
            }
        }

        private string barcode;
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; RaisePropertyChanged(() => Barcode); }
        }

        private bool DispenseChangeFail = false;
        private bool PendingChangeDispense = false;


        public override void Init()
        {

            kmtronic.Init();
            DamageBarCodeVisible = true;


            if (Global.PaymentArgs != null)
            {
                CurrentViewModel = ShoppingCartViewModel.returnChange;
            }
            else
            {
                CurrentViewModel = ShoppingCartViewModel.paymentOptions;
            }

            //aggregator.GetEvent<EventAggregation.BarcodeScannerDataEvent>().Subscribe(AddItem);
            aggregator.GetEvent<EventAggregation.LogoutCompleteEvent>().Subscribe(ClearItems);

            //Temp Code
            try
            {
                aggregator.GetEvent<EventAggregation.CreditCardPaymentEvent>().Unsubscribe(x => { CurrentViewModel = ShoppingCartViewModel.creditCardPayment; });
            }
            catch { }
            aggregator.GetEvent<EventAggregation.CreditCardPaymentEvent>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.creditCardPayment; });

            aggregator.GetEvent<EventAggregation.CashPaymentEvent>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.cashPayment; });
            aggregator.GetEvent<EventAggregation.MyAccountPaymentEvent>().Subscribe(ProcessMyAccountPaymentEvent);
            aggregator.GetEvent<EventAggregation.MyPayrollPaymentEvent>().Subscribe(ProcessMyPayrollPaymentEvent);
            aggregator.GetEvent<EventAggregation.CreditCardPaymentCancelEvent>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.paymentOptions; });
            aggregator.GetEvent<EventAggregation.CashPaymentCancelEvent>().Subscribe(ProcessCashPaymentCancelEvent);
            aggregator.GetEvent<EventAggregation.MyAccountPaymentCancelEvent>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.paymentOptions; });
            aggregator.GetEvent<EventAggregation.MyPayrollPaymentCancelEvent>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.paymentOptions; });

            aggregator.GetEvent<EventAggregation.ReturnChangeOptionsEvent>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.returnChange; });
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Subscribe(ProcessCoinJamEvent);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Subscribe(ProcessCoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Subscribe(ProcessCoinDispenseComplete);

            aggregator.GetEvent<EventAggregation.TransactionCancelEvent>().Subscribe(ProcessTransactionCancelEvent);

            //aggregator.GetEvent<EventAggregation.CreditCardPaymentCancelEvent>().Subscribe(x => { CurrentViewModel = CurrentViewModel = ShoppingCartViewModel.paymentOptions; });
            aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.paymentOptions; });

            aggregator.GetEvent<EventAggregation.CoincoCashDevicesDisabled>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.paymentOptions; });

            aggregator.GetEvent<EventAggregation.ShowPaymentOptionsEvent>().Subscribe(x => { CurrentViewModel = ShoppingCartViewModel.paymentOptions; });


            //if (Items.Count == 0)
            if (aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().SubscriptionCount == 0)
            {
                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Subscribe(PaymentComplete);
            }

            //items.CollectionChanged += items_CollectionChanged;
            base.Init();

            if (Global.RunMode.ToLower() == "debug")
                Barcode = "850006000012";
        }

        void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TotalDiscount = items.Sum(x => x.DiscountPrice);
            TotalPrice = items.Sum(x => x.PriceTaxIncluded);
            TotalSubsidy = items.Sum(x => x.SubsidyPrice);

            Global.ShoppingCartItemsCount = items.Count;
            Global.AmountDue = TotalPrice;
            aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Publish(Global.AmountDue);
        }

        private void CollectionChanged()
        {
            TotalDiscount = items.Sum(x => x.DiscountPrice);
            TotalPrice = items.Sum(x => x.PriceTaxIncluded);
            TotalSubsidy = items.Sum(x => x.SubsidyPrice);

            Global.ShoppingCartItemsCount = items.Count;
            Global.AmountDue = TotalPrice;
            aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Publish(Global.AmountDue);
        }

        private void ExecuteNobarcodeCommand()
        {
            aggregator.GetEvent<EventAggregation.MissingBarcodeEvent>().Publish(null);
        }


        private void ExecuteDamageBarCodeCommand()
        {
            DamagedBarcodeViewModel viewModel = new DamagedBarcodeViewModel();
            DialogViewService.ShowAutoCloseDialog(viewModel, 800, 600);
        }

        void ProcessMyAccountPaymentEvent(object param)
        {
            if (Global.User != null)
                CurrentViewModel = ShoppingCartViewModel.myAccountPayment;
            else
                aggregator.GetEvent<EventAggregation.LoginEvent>().Publish(null);
        }

        void ProcessMyPayrollPaymentEvent(object param)
        {
            if (Global.User != null)
                CurrentViewModel = ShoppingCartViewModel.myPayrollPayment;
            else
                aggregator.GetEvent<EventAggregation.LoginEvent>().Publish(null);
        }

        void ProcessCashPaymentCancelEvent(decimal param)
        {
            if (param != 0.0m)
            {

                if (Global.EnableCoin && Global.EnableDispenseChange)
                {
                    DialogViewService.Show(dispenseChange);
                    dispenseChange.Dispense(param, "Transaction Cancelled", "");
                    kmtronic.OpenRelay2();
                    Global.AmountInCredit = 0;
                    Global.AmountPaid = 0;

                    //CurrentViewModel = ShoppingCartViewModel.paymentOptions;
                }
                else
                {
                    Global.CreditToAccount = -param;
                    aggregator.GetEvent<EventAggregation.ReturnChangeOptionsEvent>().Publish(null);
                }
            }
            else
            {
                CurrentViewModel = ShoppingCartViewModel.paymentOptions;
            }
        }

        public void AddItems(string[] barcodes)
        {
            foreach (string barcode in barcodes)
            {
                AddItem(barcode);
            }
        }

        public void AddItem(object barcode)
        {

            ItemRepository itemRepo = new ItemRepository();
            item item = itemRepo.GetItem(barcode.ToString());

            if (item == null && Global.DemoMode)
            {
                item = itemRepo.GetAll().Where(x => x.price != 0).FirstOrDefault();
            }

            if (item != null)
            {
                ShoppingCartItem cartItem = new ShoppingCartItem();
                cartItem.Guid = Guid.NewGuid().ToString();
                cartItem.Id = item.id;
                cartItem.BarCode = item.barcode;
                cartItem.Name = item.name;
                cartItem.Categoryid = item.categoryid.HasValue ? item.categoryid.Value : 0;

                cartItem.Price = (decimal?)item.price ?? 0;
                cartItem.Crv = (decimal?)item.crv ?? 0;
                cartItem.Tax = (cartItem.Price + cartItem.Crv) * ((decimal?)item.tax_percent * 0.01m ?? 0);

                cartItem.OriginalPrice = cartItem.Price;
                cartItem.OriginalTax = cartItem.Tax;
                cartItem.PriceTaxIncluded = cartItem.Price + cartItem.Crv + cartItem.Tax;
                cartItem.DiscountId = item.discountid ?? 0;
                cartItem.TaxPercent = item.tax_percent ?? 0;

                App.Current.Dispatcher.Invoke(() =>
                {
                    cartItem.Combodiscountid = comboDiscountRepo.GetDiscountId(item.id);
                    cartItem.SubsidyId = subsidyRepo.GetSubsidyId(item.id);
                    Items.Add(cartItem);
                });

                try
                {
                    ApplySubsidy(cartItem);
                }
                catch { }

                try
                {
                    ApplyDiscount(cartItem);
                }
                catch { }

                cartItem.Tax = Math.Round(cartItem.Tax, 2);
                cartItem.PriceTaxIncluded = Math.Round(cartItem.PriceTaxIncluded, 2);

                CollectionChanged();
            }
            else
            {
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = string.Format("{0} barcode does not exist. Please retry.", barcode),
                    DeviceType = Helpers.Enum.DeviceType.BarcodeScanner,
                    ShoppingCartPkid = barcode.ToString(),
                    Code = "ZMB"
                });
            }

        }

        public void RemoveItem(string guid)
        {
            try
            {
                Items.Remove(items.FirstOrDefault(x => x.Guid.Equals(guid)));
                ResetDiscounts();
                CollectionChanged();
            }
            catch { }
        }


        public void ResetDiscounts()
        {
            Items.ToList().ForEach(x =>
            {
                x.CombodiscountApplied = false;
                //x.Combodiscountid = 0;
            });

            ItemRepository itemRepo = new ItemRepository();

            foreach (var i in Items)
            {
                item item = itemRepo.GetItem(i.BarCode);

                i.Price = (decimal?)item.price ?? 0;
                i.Crv = (decimal?)item.crv ?? 0;
                i.Tax = (i.Price + i.Crv) * ((decimal?)item.tax_percent * 0.01m ?? 0);

                i.OriginalPrice = i.Price;
                i.OriginalTax = i.Tax;
                i.PriceTaxIncluded = i.Price + i.Crv + i.Tax;

                i.Combodiscountid = comboDiscountRepo.GetDiscountId(item.id);
                i.DiscountId = item.discountid ?? 0;
                i.TaxPercent = item.tax_percent ?? 0;

                i.DiscountDescription = "";
                i.DiscountPrice = 0;
                i.DiscountTax = 0;
                i.DiscountPercentage = 0;

                ApplyDiscount(i);

                i.Tax = Math.Round(i.Tax, 2);
                i.PriceTaxIncluded = Math.Round(i.PriceTaxIncluded, 2);

            }
        }

        public void ApplySubsidy(ShoppingCartItem cartItem)
        {

            Subsidy subsidy = subsidyRepo.GetSubsidy(cartItem.SubsidyId, (decimal?)cartItem.Price ?? 0);

            if (subsidy != null)
            {
                cartItem.SubsidyDescription = subsidy.Description;
                cartItem.SubsidyPrice = subsidy.Amount;
                cartItem.SubsidyTax = subsidy.Amount * ((cartItem.TaxPercent != 0 ? cartItem.TaxPercent : 0) * 0.01m);
                cartItem.SubsidyPercentage = subsidy.Percent;

                cartItem.Price = cartItem.Price - cartItem.SubsidyPrice;
                cartItem.Tax = (cartItem.Price + cartItem.Crv) * ((cartItem.TaxPercent != 0 ? cartItem.TaxPercent : 0) * 0.01m);

                cartItem.PriceTaxIncluded = cartItem.Price + cartItem.Crv + cartItem.Tax;
            }

        }

        public void ApplyDiscount(ShoppingCartItem cartItem)
        {
            if (cartItem.Combodiscountid != 0 && items.Count > 1)
            {
                bool IsComboDiscountApplicable = comboDiscountRepo.IsComboDiscountApplicable(cartItem.Combodiscountid, Items.ToList());

                if (IsComboDiscountApplicable)
                {
                    if (Items.Where(x => x.CombodiscountApplied == true).Count() == 0)
                    {
                        //Discount discount = comboDiscountRepo.GetDiscount(DateTime.Now, cartItem.Combodiscountid, (decimal?)cartItem.Price ?? 0);

                        //if (discount != null)
                        //{
                        var i = Items.Where(x => x.Combodiscountid == cartItem.Combodiscountid).Distinct();

                        foreach (var i1 in i)
                        {
                            if (!i1.CombodiscountApplied && (Items.Where(x => x.Id == i1.Id && x.CombodiscountApplied == true).Count() == 0))
                            {
                                Discount discount = comboDiscountRepo.GetDiscount(DateTime.Now, cartItem.Combodiscountid, (decimal?)i1.Price ?? 0);

                                if (discount != null)
                                {
                                    i1.DiscountDescription = discount.Description;
                                    i1.DiscountPrice = discount.Amount;
                                    i1.DiscountTax = discount.Amount * ((i1.TaxPercent != 0 ? i1.TaxPercent : 0) * 0.01m);
                                    i1.DiscountPercentage = discount.Percent;

                                    i1.Price = i1.Price - i1.DiscountPrice;
                                    i1.Tax = (i1.Price + i1.Crv) * ((i1.TaxPercent != 0 ? i1.TaxPercent : 0) * 0.01m);

                                    i1.PriceTaxIncluded = i1.Price + i1.Crv + i1.Tax;
                                    i1.CombodiscountApplied = true;
                                }
                            }
                        }
                        //}
                    }
                }
            }
            else if (cartItem.DiscountId != 0)
            {
                Discount discount = discountRepo.GetDiscount(DateTime.Now, cartItem.DiscountId, (decimal?)cartItem.Price ?? 0);

                if (discount != null)
                {

                    cartItem.DiscountDescription = discount.Description;
                    cartItem.DiscountPrice = discount.Amount;
                    cartItem.DiscountTax = discount.Amount * ((cartItem.TaxPercent != 0 ? cartItem.TaxPercent : 0) * 0.01m);
                    cartItem.DiscountPercentage = discount.Percent;

                    cartItem.Price = cartItem.Price - cartItem.DiscountPrice;
                    cartItem.Tax = (cartItem.Price + cartItem.Crv) * ((cartItem.TaxPercent != 0 ? cartItem.TaxPercent : 0) * 0.01m);

                    cartItem.PriceTaxIncluded = cartItem.Price + cartItem.Crv + cartItem.Tax;
                }
            }
        }



        public void ClearItems(object parameter = null)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                items.Clear();
                cashPayment.Items.Clear();
            });

            CollectionChanged();

            TotalDiscount = 0;
            TotalPrice = 0;
            TotalSubsidy = 0;
        }

        private void ExecuteAddItemCommand(object parameter)
        {
            if (!parameter.ToString().Equals(""))
                AddItem(parameter.ToString());
        }

        private void ExecuteDeleteItemCommand(object parameter)
        {
            RemoveItem(parameter.ToString());
        }

        private void ExecutePayCommand()
        {
            aggregator.GetEvent<EventAggregation.PayEvent>().Publish(null);
        }

        private bool CanExecutePayCommand()
        {
            return Convert.ToBoolean(items.Count);
        }


        public override void Dispose()
        {
            //aggregator.GetEvent<EventAggregation.BarcodeScannerDataEvent>().Unsubscribe(AddItem);
            aggregator.GetEvent<EventAggregation.LogoutCompleteEvent>().Unsubscribe(ClearItems);

            aggregator.GetEvent<EventAggregation.CreditCardPaymentEvent>().Unsubscribe(x =>
            {
                CurrentViewModel = ShoppingCartViewModel.creditCardPayment;
            });
            aggregator.GetEvent<EventAggregation.CashPaymentEvent>().Unsubscribe(x =>
            {
                CurrentViewModel = ShoppingCartViewModel.cashPayment;
            });
            aggregator.GetEvent<EventAggregation.MyAccountPaymentEvent>().Unsubscribe(ProcessMyAccountPaymentEvent);
            aggregator.GetEvent<EventAggregation.MyPayrollPaymentEvent>().Unsubscribe(ProcessMyPayrollPaymentEvent);
            aggregator.GetEvent<EventAggregation.CreditCardPaymentCancelEvent>().Unsubscribe(x =>
            {
                CurrentViewModel = CurrentViewModel = ShoppingCartViewModel.paymentOptions;
            });
            aggregator.GetEvent<EventAggregation.CashPaymentCancelEvent>().Unsubscribe(x =>
            {
                CurrentViewModel = CurrentViewModel = ShoppingCartViewModel.paymentOptions;
            });
            aggregator.GetEvent<EventAggregation.MyAccountPaymentCancelEvent>().Unsubscribe(x =>
            {
                CurrentViewModel = CurrentViewModel = ShoppingCartViewModel.paymentOptions;
            });
            aggregator.GetEvent<EventAggregation.MyPayrollPaymentCancelEvent>().Unsubscribe(x =>
            {
                CurrentViewModel = CurrentViewModel = ShoppingCartViewModel.paymentOptions;
            });

            if (Items.Count == 0 && PendingChangeDispense == false)
                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Unsubscribe(PaymentComplete);


            //items.CollectionChanged -= items_CollectionChanged;

            if (kmtronic != null)
                kmtronic.Dispose();

            aggregator.GetEvent<EventAggregation.CashPaymentCancelEvent>().Unsubscribe(ProcessCashPaymentCancelEvent);
            aggregator.GetEvent<EventAggregation.TransactionCancelEvent>().Unsubscribe(ProcessTransactionCancelEvent);

        }

        private void ProcessTransactionCancelEvent(object param)
        {
            if (Global.CreditToAccount != 0)
            {
                Global.AmountDue = 0;
                Global.PaymentArgs = null;
                ClearItems();

                DialogViewService.ShowAutoCloseDialog("Transaction Cancelled", string.Format("{0} is credited to your account.", (-Global.CreditToAccount).ToString("c2")));

                Global.CreditToAccount = 0;
                aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Publish(null);
            }
        }

        private void PaymentComplete(PaymentCompleteEventArgs args)
        {
            Global.AmountDue = 0;
            Global.AmountPaid = 0;

            if (Items.Count != 0 && PendingChangeDispense == false)
            {
                ShoppingCartRepository shoppingRepo = new ShoppingCartRepository();
                string cartpkid = shoppingRepo.SaveShoppingCart(Items.ToList(),
                                                                args.PaymentItems,
                                                                args.Error, Global.User == null ? "" : Global.User.ProviderUserKey.ToString());
                if (cartpkid != "")
                {

                    try
                    {
                        ISyncData syncData = SyncDataFactory.GetSyncData();
                        syncData.Init();
                        Task.Factory.StartNew(() =>
                        {
                            syncData.UploadShoppingCartDetails(cartpkid);
                        });
                    }
                    catch { }

                    if (args.Source == "MyAccountPay")
                    {
                        AccountBalanceHistoryRepository accountHistoryRepo = new AccountBalanceHistoryRepository();
                        accountHistoryRepo.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance,
                                               -args.PaymentItems.Sum(x => x.Payment),
                                               "Purchase", cartpkid);
                    }

                    Global.Email.SendPaymentComplete(args.Source, Items.ToList());


                    Global.ShoppingCartItemsForPrint = Items.ToList();
                    Global.PaymentMethodForPrint = args.Source;
                    Global.ShoppingCartIdForPrint = cartpkid;
                    Global.DialogTypeForPrint = "Purchase Complete";

                    ClearItems();

                    if (args.Change > 0)
                    {
                        if (Global.EnableCoin)
                        {
                            PendingChangeDispense = true;
                            DialogViewService.Show(ShoppingCartViewModel.dispenseChange);
                            dispenseChange.Dispense(args.Change, "Purchase Complete", cartpkid);
                            kmtronic.OpenRelay2();

                            if (DispenseChangeFail == false)
                            {
                               
                                //aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Unsubscribe(PaymentComplete);
                                //aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Publish(null);
                                //Global.PaymentArgs = null;
                            }
                            else
                            {

                                DispenseChangeFail = false;

                            }

                        }
                        else
                        {
                            

                            DialogViewService.ShowDialog("Purchase Complete", "Purchase Complete");

                            aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Unsubscribe(PaymentComplete);
                            aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Publish(null);
                            Global.PaymentArgs = null;

                        }
                    }
                    else
                    {
                       

                        DialogViewService.ShowDialog("Purchase Complete", "Purchase Complete");

                        aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Unsubscribe(PaymentComplete);
                        aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Publish(null);
                        Global.PaymentArgs = null;
                    }
                }
                else
                {
                  

                    throw new Exception("SaveShoppingCart() failed.");

                    aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Unsubscribe(PaymentComplete);
                    aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Publish(null);
                    Global.PaymentArgs = null;

                }



            }
            else 
            {

                if (args.Change > 0)
                {
                    aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Publish(null);
                }
                else {
                    PendingChangeDispense = false;

                    DialogViewService.ShowDialog("Purchase Complete", "Purchase Complete");

                    aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Unsubscribe(PaymentComplete);
                    aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Publish(null);
                    Global.PaymentArgs = null;
                
                }


             
            
            }
            


            //aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Unsubscribe(PaymentComplete);
            //aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Publish(null);
            //Global.PaymentArgs = null;
        }

        private void ProcessCoinJamEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Unsubscribe(ProcessCoinDispenseComplete);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Unsubscribe(ProcessCoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Unsubscribe(ProcessCoinJamEvent);
            DispenseChangeFail = true;
        }


        private void ProcessCoinDispenseFailed(object obj)
        {
            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Unsubscribe(ProcessCoinDispenseComplete);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Unsubscribe(ProcessCoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Unsubscribe(ProcessCoinJamEvent);
            DispenseChangeFail = true;
        }

        private void ProcessCoinDispenseComplete(object obj)
        {
            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Unsubscribe(ProcessCoinDispenseComplete);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Unsubscribe(ProcessCoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Unsubscribe(ProcessCoinJamEvent);
            PendingChangeDispense = false;
            aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Unsubscribe(PaymentComplete);
            aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Publish(null);
            Global.PaymentArgs = null;

        }

    }
}
