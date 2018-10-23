using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using deORO.Helpers;
using System.Windows.Controls;
using System.Timers;
using Microsoft.Practices.Composite.Events;
using System.Collections.Generic;
using System.Linq;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using System.Windows.Threading;
using deORO.EventAggregation;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using deORODataAccessApp.Models;
using deORO.CardProcessor;
using System.Reflection;
using System.Windows.Forms;
using deORO.BarcodeScanner;
using deORO.CardReader;
using deORO.Communication;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;



namespace deORO.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        readonly static MyAccountViewModel myAccount = new MyAccountViewModel();
        readonly static BarcodeViewModel barcodeChange = new BarcodeViewModel();
        readonly static ShoppingCartViewModel shoppingCart = new ShoppingCartViewModel();
        readonly static BillingViewModel refillAccount = new BillingViewModel();
        readonly static UserRegistrationViewModel userRegistration = new UserRegistrationViewModel();
        readonly static LoginViewModel login = new LoginViewModel();
        readonly static MissingBarcodeViewModel missingBarcode = new MissingBarcodeViewModel();
        readonly static MissingBarcodeItemsViewModel missingBarcodeItems = new MissingBarcodeItemsViewModel();
        readonly static MissingBarcodeItemViewModel missingBarcodeItem = new MissingBarcodeItemViewModel();
        readonly static BusyViewModel busy = new BusyViewModel();
        readonly static DispenseChangeViewModel dispenseChange = new DispenseChangeViewModel();
        readonly static CameraFeedViewModel cameraFeed = new CameraFeedViewModel();
        readonly static HelpViewModel help = new HelpViewModel();
        readonly static HomeViewModel home = new HomeViewModel();
        readonly static DiscountsViewModel discounts = new DiscountsViewModel();
        readonly static NavigationViewModel navigation = new NavigationViewModel();
        readonly static FastTouchViewModel fastTouch = new FastTouchViewModel();
        readonly static UnlockViewModel unlock = new UnlockViewModel();
        public ICommand ChangeLocaleCommand { get { return new DelegateCommandWithParam(ExecuteChangeLocaleCommand); } }
        private DispatcherTimer autoLogoffTimer = new DispatcherTimer();
        private DispatcherTimer updatePasswordTimer = new DispatcherTimer();
        private CashCounterRepository cashCounterRepo = new CashCounterRepository();
        private LoginActivity loginActivityRepo = new LoginActivity();
        private IBarcodeScanner barcodeScanner = BarcodeScanner.BarcodeScannerFactory.GetBarcodeScanner();
        private deOROMembershipProvider membership = new deOROMembershipProvider();

        public bool IsHiddenAddItemsVisible
        {
            get
            {
                if (Global.AddItemsVisible)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsHiddenSearchVisible
        {
            get
            {
                if (Global.SearchVisible)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private string version;

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
        private void ExecuteDamageBarCodeCommand()
        {
            DamagedBarcodeViewModel viewModel = new DamagedBarcodeViewModel();
            DialogViewService.ShowAutoCloseDialog(viewModel, 800, 600);
        }

        public string Version
        {
            get { return version; }
            set { version = value; RaisePropertyChanged(() => Version); }
        }

        private string footer;
        public string Footer
        {
            get { return footer; }
            set
            {
                footer = value;
                RaisePropertyChanged(() => Footer);
            }
        }

        private bool logoutVisible;
        public bool LogoutVisible
        {
            get { return logoutVisible; }
            set
            {
                logoutVisible = value;
                RaisePropertyChanged(() => LogoutVisible);
            }
        }

        private bool refillButtonVisible;
        public bool RefillButtonVisible
        {
            get { return refillButtonVisible; }
            set
            {
                refillButtonVisible = value;
                RaisePropertyChanged(() => RefillButtonVisible);
            }
        }

        private bool userRegistrationVisible;
        public bool UserRegistrationVisible
        {
            get { return userRegistrationVisible; }
            set
            {
                userRegistrationVisible = value;
                RaisePropertyChanged(() => UserRegistrationVisible);
            }
        }

        private bool rightToolBarEnable = true;
        public bool RightToolBarEnable
        {
            get { return rightToolBarEnable; }
            set { rightToolBarEnable = value; RaisePropertyChanged(() => RightToolBarEnable); }
        }

        private bool shoppingCartEnable = true;
        public bool ShoppingCartEnable
        {
            get { return shoppingCartEnable; }
            set { shoppingCartEnable = value; RaisePropertyChanged(() => ShoppingCartEnable); }
        }

        private bool myAccountEnable = true;
        public bool MyAccountEnable
        {
            get { return myAccountEnable; }
            set { myAccountEnable = value; RaisePropertyChanged(() => MyAccountEnable); }
        }


        private bool unlockEnable = true;
        public bool UnlockEnable
        {
            get { return unlockEnable; }
            set { unlockEnable = value; RaisePropertyChanged(() => UnlockEnable); }
        }

        private bool unlockVisible = false;
        public bool UnlockVisible
        {
            get { return unlockVisible; }
            set
            {
                unlockVisible = value;
                RaisePropertyChanged(() => UnlockVisible);
            }
        }

        private bool logoutEnable = true;
        public bool LogoutEnable
        {
            get { return logoutEnable; }
            set { logoutEnable = value; RaisePropertyChanged(() => logoutEnable); }
        }

        private bool userRegistrationEnable = true;
        public bool UserRegistrationEnable
        {
            get { return userRegistrationEnable; }
            set { UserRegistrationEnable = value; RaisePropertyChanged(() => UserRegistrationEnable); }
        }

        private bool myAccountVisible = false;
        public bool MyAccountVisible
        {
            get { return myAccountVisible; }
            set
            {
                myAccountVisible = value;
                RaisePropertyChanged(() => MyAccountVisible);
            }
        }

        private string myAccountText;
        public string MyAccountText
        {
            get { return myAccountText; }
            set
            {
                myAccountText = value;
                RaisePropertyChanged(() => MyAccountText);
            }
        }

        private bool loginVisible = true;
        public bool LoginVisible
        {
            get { return loginVisible; }
            set
            {
                loginVisible = value;
                RaisePropertyChanged(() => LoginVisible);
            }
        }

        private bool cameraFeedVisible;
        public bool CameraFeedVisible
        {
            get { return cameraFeedVisible; }
            set
            {
                cameraFeedVisible = value;
                RaisePropertyChanged(() => CameraFeedVisible);
            }
        }

        private BaseViewModel cameraViewModel;
        public BaseViewModel CameraViewModel
        {
            get
            {
                return cameraViewModel;
            }
            set
            {
                if (cameraViewModel == value)
                    return;

                cameraViewModel = value;
                RaisePropertyChanged(() => CameraViewModel);

            }
        }

        private BaseViewModel bottomPaneViewModel;

        public BaseViewModel BottomPaneViewModel
        {
            get { return bottomPaneViewModel; }
            set { bottomPaneViewModel = value; RaisePropertyChanged(() => BottomPaneViewModel); }
        }

        private BaseViewModel rightPaneViewModel;

        public BaseViewModel RightPaneViewModel
        {
            get { return rightPaneViewModel; }
            set { rightPaneViewModel = value; RaisePropertyChanged(() => RightPaneViewModel); }
        }

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
                Global.CurrentViewModel = currentViewModel.GetType().Name;
                RaisePropertyChanged(() => CurrentViewModel);
            }
        }

        private string column1Width = "0.75*";

        public string Column1Width
        {
            get { return column1Width; }
            set { column1Width = value; RaisePropertyChanged(() => Column1Width); }
        }
        private string column2Width = "0.20*";

        public string Column2Width
        {
            get { return column2Width; }
            set { column2Width = value; RaisePropertyChanged(() => Column2Width); }
        }

        private int rowHelpHeight = 0;

        public int RowHelpHeight
        {
            get { return rowHelpHeight; }
            set { rowHelpHeight = value; RaisePropertyChanged(() => RowHelpHeight); }
        }



        private int cameraWidthHelp = 0;

        public int CameraWidthHelp
        {
            get { return cameraWidthHelp; }
            set { cameraWidthHelp = value; RaisePropertyChanged(() => CameraWidthHelp); }
        }

        private static double Top { get; set; }
        private static double Left { get; set; }
        private static double Width { get; set; }
        private static double Height { get; set; }

        public static bool IsMaximized { get; private set; }
        public static bool BlockStateChange { get; set; }

        private static void SetWindowTop(Window window)
        {
            BlockStateChange = true;
            window.Top = Top;
        }

        private static void SetWindowLeft(Window window)
        {
            BlockStateChange = true;
            window.Left = Left;
        }

        void SetTargetDisplay()
        {



        }


        public MainWindowViewModel()
        {



            //UserRegistrationVisible = true;
            LogoutVisible = false;
            RefillButtonVisible = false;

            aggregator.GetEvent<EventAggregation.PayEvent>().Subscribe(Pay);
            aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Subscribe(LoggedIn);
            aggregator.GetEvent<EventAggregation.PaymentMethodCancelEvent>().Subscribe(ShowShoppingCart);
            aggregator.GetEvent<EventAggregation.MyAccountPaymentCompleteEvent>().Subscribe(ShowMyAccountPaymentComplete);
            aggregator.GetEvent<EventAggregation.MyPayrollPaymentCompleteEvent>().Subscribe(ShowMyPayrollPaymentComplete);
            aggregator.GetEvent<EventAggregation.ShowHomeScreenEvent>().Subscribe(ShowHome);
            aggregator.GetEvent<EventAggregation.UserRegistrationCancelEvent>().Subscribe(ShowShoppingCart);
            aggregator.GetEvent<EventAggregation.LoginCancelEvent>().Subscribe(ShowShoppingCart);
            aggregator.GetEvent<EventAggregation.UserRegistrationAddEvent>().Subscribe(UserAdd);
            aggregator.GetEvent<EventAggregation.UserRegistrationCompleteEvent>().Subscribe(UserRegistrationComplete);
            aggregator.GetEvent<EventAggregation.LoginFailEvent>().Subscribe(LoginFailed);
            aggregator.GetEvent<EventAggregation.DeleteMeCompletedEvent>().Subscribe(Logout);
            aggregator.GetEvent<EventAggregation.ConfigurationSettingsSaveSuccessfulEvent>().Subscribe(ConfigurationSettingsSaved);
            aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Subscribe(DeviceInitFailed);
            aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Subscribe(DeviceOpenFailed);
            aggregator.GetEvent<EventAggregation.MissingBarcodeEvent>().Subscribe(ShowMissingBarcodeItems);
            aggregator.GetEvent<EventAggregation.MissingBarcodeCategorySelectCancelEvent>().Subscribe(ShowShoppingCart);
            //aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCancelEvent>().Subscribe(ShowShoppingCart);
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCompleteEvent>().Subscribe(ShowShoppingCartAndAddItems);
            aggregator.GetEvent<EventAggregation.LoginEvent>().Subscribe(ShowLogin);
            aggregator.GetEvent<EventAggregation.AutoLogoffEvent>().Subscribe(ExecuteLogoutCommand);
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Subscribe(ProcessLogTransactionError);

            aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Subscribe(ProcessCoinAccepted);
            aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Subscribe(ProcessBillAccepted);

            aggregator.GetEvent<EventAggregation.BarcodeScannerDataGlobalEvent>().Subscribe(ProcessBarcode);
            aggregator.GetEvent<EventAggregation.DiscountSelectCompleteEvent>().Subscribe(ProcessDiscountSelectComplete);
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemSelectCompleteEvent>().Subscribe(ShowMissingItem);
            aggregator.GetEvent<EventAggregation.UserRegistrationEvent>().Subscribe(ShowUserRegistration);

            aggregator.GetEvent<EventAggregation.MissingBarcodeItemSelectCancelEvent>().Subscribe(ShowShoppingCart);
            aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Subscribe(ProcessToolBarEnabledEvent);
            aggregator.GetEvent<EventAggregation.DiscountMarqueeSelectComplete>().Subscribe((x) => CurrentViewModel = MainWindowViewModel.discounts);

            aggregator.GetEvent<EventAggregation.DiscountSelectCancelEvent>().Subscribe(ShowShoppingCart);

            aggregator.GetEvent<EventAggregation.ShowLoginEvent>().Subscribe(ShowLogin);
            aggregator.GetEvent<EventAggregation.ShowMyAccountEvent>().Subscribe(ShowMyAccount);
            aggregator.GetEvent<EventAggregation.ShowRefillEvent>().Subscribe(ShowRefill);
            aggregator.GetEvent<EventAggregation.ShowShoppingCartEvent>().Subscribe(ShowShoppingCart);
            aggregator.GetEvent<EventAggregation.ShowRegisterUserEvent>().Subscribe(ShowUserRegistration);
            aggregator.GetEvent<EventAggregation.UserLogoutEvent>().Subscribe(Logout);
            aggregator.GetEvent<EventAggregation.ShowHelpEvent>().Subscribe(ShowHelp);
            aggregator.GetEvent<EventAggregation.ShowRefillEvent>().Subscribe(ShowRefill);

            InitAutoLogoffTimer();
            InitUpdatePasswordTimer();

            ShowHome();
            if (Global.EnableCameraFeed || Global.EnableDiscounts)
            {
                CameraViewModel = cameraFeed;

                if (RowHelpHeight == 0)
                {
                    RowHelpHeight = 180;
                    CameraWidthHelp = 350;
                }
                else
                    RowHelpHeight = 0;
            }
            //if (Global.EnableCameraFeed || Global.EnableDiscounts)
            //{
            //    CameraViewModel = cameraFeed;

            //    if (RowHelpHeight == 0)
            //        RowHelpHeight = 150;
            //    else
            //        RowHelpHeight = 0;
            //}

            if (Global.Orientation.ToLower().Equals("landscape"))
            {

                BottomPaneViewModel = navigation;
                RightPaneViewModel = fastTouch;

            }
            else if (Global.Orientation.ToLower().Equals("portrait"))
            {
                BottomPaneViewModel = fastTouch;
                RightPaneViewModel = navigation;
            }
            else if (Global.Orientation.ToLower().Equals("hide"))
            {
                BottomPaneViewModel = navigation;
                RightPaneViewModel = fastTouch;
            }

            if (Global.CollapseRightPane)
            {
                RightPaneViewModel = null;
                Column1Width = "100*";
                Column2Width = "0*";
            }

            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void ProcessToolBarEnabledEvent(object obj)
        {
            RightToolBarEnable = Convert.ToBoolean(obj);
        }

        private void ExecuteChangeLocaleCommand(object obj)
        {
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.CreateSpecificCulture(obj.ToString());

        }

        private void ShowUserRegistration(object obj)
        {
            if (CurrentViewModel != MainWindowViewModel.userRegistration)
                CurrentViewModel = MainWindowViewModel.userRegistration;

            Footer = "User Registration View";
        }


        private void ShowMissingItem(object obj)
        {
            missingBarcodeItem.Init(Convert.ToInt32(obj));
            CurrentViewModel = MainWindowViewModel.missingBarcodeItem;
        }

        private void ProcessDiscountSelectComplete(object obj)
        {
            missingBarcodeItems.ItemSelector = "Discount";
            missingBarcodeItems.Init(Convert.ToInt32(obj));
            CurrentViewModel = MainWindowViewModel.missingBarcodeItems;
        }


        private void FingerPrintLoginSuccessful(string parameter)
        {
            membership.LogSuccessfulLogin(parameter.ToString(), deORODataAccessApp.Helpers.Enum.AuthenticationMode.FingerPrint);
            aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish(membership.GetUser(parameter.ToString()) as deOROMembershipUser);
        }

        public override void Init()
        {
            //scanner = BarcodeScanner.Honeywell.Instance;
            barcodeScanner.Open();
            //cardReader.Open();
            CameraFeedVisible = Global.EnableCameraFeed;

            try
            {
                if (Global.DemoMode)
                {
                    DialogViewService.ShowDialog("Demo Mode", "Application is running in Demo Mode");
                }
            }
            catch { }

            base.Init();
        }

        private void ProcessCoinAccepted(CashEventArgs obj)
        {
            if (obj.Routing == "CashBox")
            {
                cash_counter counter = new cash_counter();
                counter.amount = obj.Amount;
                counter.cash_type = "Coin";
                counter.pkid = Guid.NewGuid().ToString();
                counter.created_date_time = DateTime.Now;
                cashCounterRepo.AddCashCounter(counter);
            }
           
        }

        private void ProcessBillAccepted(CashEventArgs obj)
        {
            cash_counter counter = new cash_counter();
            counter.amount = obj.Amount;
            counter.cash_type = "Bill";
            counter.pkid = Guid.NewGuid().ToString();
            counter.created_date_time = DateTime.Now;
            cashCounterRepo.AddCashCounter(counter);
        }

        private void ProcessLogTransactionError(TransactionErrorEventArgs obj)
        {
            TransactionErrorRepository repo = new TransactionErrorRepository();
            transaction_error error = new transaction_error();

            error.amount = obj.Amount;
            error.created_date_time = DateTime.Now;
            error.pkid = Guid.NewGuid().ToString();
            error.source = obj.DeviceType.ToString();
            error.description = obj.Event;
            error.sourceid = obj.ShoppingCartPkid;
            error.code = obj.Code;
            repo.AddItem(error);

            Global.Email.SendTransactionError(error);
            DialogViewService.ShowAutoCloseDialog(obj.DeviceType.ToString(), obj.Event);
        }

        private void ExecuteHelpCommand()
        {
            ShowHelp();
        }

        private void InitAutoLogoffTimer()
        {
            autoLogoffTimer.Interval = new TimeSpan(0, 0, 0, 5);
            autoLogoffTimer.Tick += autoLogoffTimer_Tick;
            autoLogoffTimer.Start();
        }

        private void InitUpdatePasswordTimer()
        {
            updatePasswordTimer.Interval = new TimeSpan(23, 59, 59);
            updatePasswordTimer.Tick += updatePasswordTimer_Tick;
            updatePasswordTimer.Start();
        }

        void updatePasswordTimer_Tick(object sender, EventArgs e)
        {
            updatePasswordTimer.Stop();
            UpdatePassword();
            updatePasswordTimer.Start();
        }

        int iBuffer = 0;

        private void autoLogoffTimer_Tick(object sender, EventArgs e)
        {
            autoLogoffTimer.Stop();

            if (GetLastInputTime() > Helpers.Global.IdleTimeout)
            {
                if (Global.User != null)
                {
                    //SendKeysCode.Send("{ESC}");
                    aggregator.GetEvent<EventAggregation.CloseKeyboardOnTimeOut>().Publish(null);
                    
                    AutoLogoffViewModel vm = new AutoLogoffViewModel();
                    DialogViewService.ShowDialog(vm, 350, 350);

                    iBuffer = Helpers.Global.IdleTimeout + 1;
                }
                else if (shoppingCart.Items.Count > 0 || Global.AmountPaid != 0)
                {
                    //SendKeysCode.Send("{ESC}");
                    aggregator.GetEvent<EventAggregation.CloseKeyboardOnTimeOut>().Publish(null);

                    AutoLogoffViewModel vm = new AutoLogoffViewModel();
                    DialogViewService.ShowDialog(vm, 350, 350);
                }
                else
                {
                    iBuffer = 0;
                }
            }

            autoLogoffTimer.Start();
        }

        private void ShowShoppingCartAndAddItems(object obj)
        {
            shoppingCart.AddItems(obj as string[]);
            ShowShoppingCart();
        }

        private void ShowMissingBarcodeItems(object obj)
        {
            CurrentViewModel = MainWindowViewModel.missingBarcode;
        }

        private void DeviceInitFailed(EventAggregation.DeviceFailEventArgs obj)
        {
            DeviceErrorRepository repo = new DeviceErrorRepository();
            device_error error = new device_error();
            error.created_date_time = DateTime.Now;
            error.description = string.Format("Failed to Initialize {0}. \r\n {1}", obj.DeviceType.ToString(), obj.Message);
            error.pkid = Guid.NewGuid().ToString();
            error.source = obj.DeviceType.ToString();
            repo.AddDeviceError(error);

            DialogViewService.ShowAutoCloseDialog("Device Init", string.Format("Failed to Initialize {0}. \r\n {1}",
                                   obj.DeviceType.ToString(), obj.Message));
        }

        private void DeviceOpenFailed(EventAggregation.DeviceFailEventArgs obj)
        {
            DeviceErrorRepository repo = new DeviceErrorRepository();
            device_error error = new device_error();
            error.created_date_time = DateTime.Now;
            error.description = string.Format("Failed to Open {0}. \r\n {1}", obj.DeviceType.ToString(), obj.Message);
            error.pkid = Guid.NewGuid().ToString();
            error.source = obj.DeviceType.ToString();
            repo.AddDeviceError(error);

            DialogViewService.ShowAutoCloseDialog("Device Open", string.Format("Failed to Open {0}. \r\n {1}",
                                   obj.DeviceType.ToString(), obj.Message));
        }

        private void ConfigurationSettingsSaved(object parameter)
        {
            DialogViewService.ShowAutoCloseDialog("Configuration Settings", "Configuration Settings Saved");
        }


        private void LoginFailed(deORO.Helpers.Enum.AuthenticationMode mode)
        {
            if (mode == Helpers.Enum.AuthenticationMode.FingerPrint)
            {
                DialogViewService.ShowAutoCloseDialog("Login Failed", "Invalid Fingerprint");
                ShowLogin();
            }
            else if (mode == Helpers.Enum.AuthenticationMode.UserCredentials)
            {
                DialogViewService.ShowAutoCloseDialog("Login Failed", "Invalid UserName or Password");
                ShowLogin();
            }
            else if (mode == Helpers.Enum.AuthenticationMode.Barcode)
            {
                DialogViewService.ShowAutoCloseDialog("Login Failed", "Invalid Barcode");
            }
        }

        void scanner_DeviceScanner_DataEvent(object sender, ObjectArrEventArgs e)
        {
            //scanner.Dispose();
            //scanner.Init();
            ShowShoppingCart();

            shoppingCart.AddItem(e.data[2].ToString());
        }

        private void ProcessBarcode(object parameter)
        {
            //if(Global.RunMode.ToLower().Equals("debug"))
            //    MessageBox.Show(parameter.ToString());

            ClickRightMouseButton();
            //string barcode_login = parameter.ToString().Replace(",", "");
            if (parameter.ToString().ToLower().StartsWith(Global.LoginPrefix.ToLower()) || parameter.ToString().ToLower().StartsWith(Global.ConversionPrefix.ToLower()))
            {
                ShowLogin();

                if (membership.ValidateUser(parameter.ToString()))
                {
                    var user = membership.GetUserByBarcode(parameter.ToString());

                    membership.LogSuccessfulLogin(user.UserName, deORODataAccessApp.Helpers.Enum.AuthenticationMode.Barcode);
                    aggregator.GetEvent<EventAggregation.LoginSuccessfulEvent>().Publish(user as deOROMembershipUser);

                    if (parameter.ToString().ToLower().StartsWith(Global.ConversionPrefix.ToLower()) && Global.ConversionReward > 0)
                    {
                        ShowMyAccountBarcode();
                        DialogViewService.ShowAutoCloseDialog("Reward Yourself", "Register a new card and recieve a " + Global.ConversionReward.ToString("c") + " credit");
                    }
                }


                else
                {
                    //aggregator.GetEvent<EventAggregation.LoginFailEvent>().Publish(deORO.Helpers.Enum.AuthenticationMode.Barcode);
                    DialogViewService.ShowAutoCloseDialog("Register", "Fill in the required fields to register");
                    CurrentViewModel = MainWindowViewModel.userRegistration;
                    Global.InvalidUserBarcode = parameter.ToString();
                }
            }

            else
            {
                ShowShoppingCart();
                shoppingCart.AddItem(parameter);
            }
        }

        private void Pay(object parameter)
        {
            ObjectArrEventArgs args = parameter as ObjectArrEventArgs;
            Global.AmountDue = shoppingCart.TotalPrice;
        }

        private void ShowMyAccountPaymentComplete(deOROMembershipUser parameter)
        {
            Global.User = parameter;
        }

        private void ShowMyPayrollPaymentComplete(deOROMembershipUser parameter)
        {
            Global.User = parameter;
        }

        private void ShowHome(object parameter = null)
        {
            if (Global.User != null)
            {
                Logout();
            }
            else
            {
                CurrentViewModel = MainWindowViewModel.home;
                Footer = "Home View";
            }
        }


        private void MyAccountPaymentCancel(object parameter = null)
        {
            ShowShoppingCart();
        }

        private void MyPayrollPaymentCancel(object parameter = null)
        {
            ShowShoppingCart();
        }

        private void CashPaymentCancel(object parameter = null)
        {
            ShowShoppingCart();
        }

        private void ExecuteMyAccountCommand()
        {
            ShowMyAccount();
        }

        private void ExecuteRefillButtonCommand()
        {
            ShowRefill();
        }

        
        private void ExecuteLoginCommand()
        {
            ShowLogin();
        }

        private void ShowLogin(object parameter = null)
        {
            if (CurrentViewModel != MainWindowViewModel.login)
                CurrentViewModel = MainWindowViewModel.login;

            Footer = "Login View";

        }

        private void ShowMyAccount(object parameter = null)
        {
            if (Global.Orientation == "hide")
            {
                myAccount.TabSelectedIndex = 8;
            }
            else
            {
                myAccount.TabSelectedIndex = 3;
            }
            if (CurrentViewModel != MainWindowViewModel.myAccount)
                CurrentViewModel = MainWindowViewModel.myAccount;

            LogoutVisible = true;
            if (Global.RefillButtonVisible) { RefillButtonVisible = true; }
            UserRegistrationVisible = false;

            Footer = "My Account View";
        }

        private void ShowRefill(object parameter = null)
        {
            if (CurrentViewModel != MainWindowViewModel.refillAccount)
                CurrentViewModel = MainWindowViewModel.refillAccount;

            Footer = "Refill View";
        }

        //private void ShowRefill(object parameter = null)
        //{
        //    if (Global.Orientation == "hide")
        //    {
        //        myAccount.TabSelectedIndex = 8;
        //    }
        //    else
        //    {
        //        myAccount.TabSelectedIndex = 3;
        //    }
        //    if (CurrentViewModel != MainWindowViewModel.myAccount)
        //        CurrentViewModel = MainWindowViewModel.myAccount;

        //    Footer = "My Account View";
        //}


        private void ShowMyAccountBarcode(object parameter = null)
        {
            if (CurrentViewModel != MainWindowViewModel.myAccount)
                if (Global.Orientation == "hide")
                {
                    myAccount.TabSelectedIndex = 8;
                }
                else
                {
                    myAccount.TabSelectedIndex = 3;
                }
            CurrentViewModel = myAccount;
            Footer = "My Account View";
        }

        private void UserRegistrationComplete(object parameter = null)
        {
            if (Global.PaymentArgs != null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Global.PaymentArgs.PaymentItems.Add(new PaymentItem
                    {
                        DateTime = DateTime.Now,
                        Payment = -Global.CreditToAccount,
                        Routing = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                        Source = "Purchase Complete"
                    });

                    Global.PaymentArgs.Change = 0;
                    Global.AmountInCredit = 0;
                    Global.AmountPaid = 0;
                });

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(Global.PaymentArgs);
            }

            deOROMembershipProvider userProvider = new deOROMembershipProvider();
            Global.User = userProvider.GetUser(parameter.ToString()) as deOROMembershipUser;

            LoggedIn(Global.User);
            ShowMyAccount();

        }

        private void UserAdd(object parameter = null)
        {
            Global.CreditToAccount = 0;
            Global.AmountDue = 0;
            //shoppingCart.Items.Clear();

            LogoutVisible = true;
            if (Global.RefillButtonVisible) { RefillButtonVisible = true; }
            UserRegistrationVisible = false;
        }

        private void LoggedIn(deOROMembershipUser user)
        {

            Global.User = user;
            LoginVisible = false;
            MyAccountVisible = true;
            

            MyAccountText = user.UserName;

            try
            {
                login_activity loginActivity = new login_activity();
                loginActivity.created_date_time = DateTime.Now;
                loginActivity.userpkid = user.ProviderUserKey.ToString();
                loginActivity.method = user.LoginMethod;
                loginActivity.pkid = Guid.NewGuid().ToString();

                loginActivityRepo.AddLoginActivity(loginActivity);
            }
            catch { }

            CreditViewModel creditView = new CreditViewModel();
            DialogViewService.ShowDialog(creditView, 600, 600);


            if (Global.PaymentArgs != null)
            {
                deOROMembershipProvider userProvider = new deOROMembershipProvider();
                userProvider.UpdateUserBalance(Global.User.UserName, -Global.CreditToAccount, "CreditToMyAccount");
                Global.User = userProvider.GetUser(Global.User.UserName) as deOROMembershipUser;

                AccountBalanceHistoryRepository accountHistoryRepo = new AccountBalanceHistoryRepository();
                accountHistoryRepo.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance, -Global.CreditToAccount, "CreditToMyAccount - Transaction Complete");

                App.Current.Dispatcher.Invoke(() =>
                {
                    Global.PaymentArgs.PaymentItems.Add(new PaymentItem
                    {
                        DateTime = DateTime.Now,
                        Payment = -Global.CreditToAccount,
                        Routing = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                        Source = "Purchase Complete"
                    });

                    Global.PaymentArgs.Change = 0;
                    Global.AmountInCredit = 0;
                    Global.AmountPaid = 0;
                });

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(Global.PaymentArgs);
                return;
            }

            if (Global.Orientation == "hide")
            { ShowMyAccount(); }
            else { ShowShoppingCart(); }
            LogoutVisible = true;
            if (Global.RefillButtonVisible) { RefillButtonVisible = true; }
            UserRegistrationVisible = false;
        }


        private void ExecuteShoppingCartCommand()
        {
            ShowShoppingCart();
        }

        private void ShowHelp(object parameter = null)
        {
            DialogViewService.ShowDialog(help, 700, 700);
        }

        private void ShowShoppingCart(object parameter = null)
        {
            if (CurrentViewModel != MainWindowViewModel.shoppingCart)
                CurrentViewModel = MainWindowViewModel.shoppingCart;

            Footer = "Shopping Cart View";
        }

        private void ExecuteRegisterUserCommand()
        {
            ShowUserRegistration(null);
        }

        private void Logout(object parameter = null)
        {
            if (Global.User != null && Global.AmountPaid != 0)
            {
                //deOROMembershipProvider userProvider = new deOROMembershipProvider();
                //userProvider.UpdateUserBalance(Global.User.UserName, Global.AmountPaid, "Abandonded Cash");
                //Global.User = userProvider.GetUser(Global.User.UserName) as deOROMembershipUser;

                //if (Global.EnableRefillRewards)
                //    Global.AmountPaid += Helpers.RewardHelper.GetRewardAmount(Global.AmountPaid);

                //AccountBalanceHistoryRepository accountHistoryRepo = new AccountBalanceHistoryRepository();
                //accountHistoryRepo.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance, Global.AmountPaid, "Abandonded Cash");
                ProcessRefill(Global.PaymentItems);

            }
            else if (Global.AmountInCredit != 0)
            {
                try
                {
                    if (Global.DispenseInIdleTimeout != true || Global.EnableDispenseChange != true || Global.EnableCoin != true)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Global.PaymentArgs.PaymentItems.Add(new PaymentItem
                            {
                                DateTime = DateTime.Now,
                                Payment = -Global.CreditToAccount,
                                Routing = Helpers.Enum.PaymentMethod.ChangeAbandoned.ToString(),
                                Source = "Purchase Complete"
                            });

                            Global.PaymentArgs.Change = 0;
                            Global.CreditToAccount = 0;
                            Global.AmountInCredit = 0;
                            Global.AmountPaid = 0;
                        });
                    }
                    else
                    {

                        var commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
                        commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
                        commType.EnableCoins();
                        decimal AmountInCreditPositive = System.Math.Abs(Global.AmountInCredit);
                        commType.DispenseChange(AmountInCreditPositive);
                        commType.CloseDevices();

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Global.PaymentArgs.PaymentItems.Add(new PaymentItem
                            {
                                DateTime = DateTime.Now,
                                Payment = -Global.CreditToAccount,
                                Routing = "Dispense",
                                Source = "Purchase Complete"
                            });

                            Global.PaymentArgs.Change = 0;
                            Global.CreditToAccount = 0;
                            Global.AmountInCredit = 0;
                            Global.AmountPaid = 0;
                        });


                    }

                    aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(Global.PaymentArgs);

                }
                catch { }
            }
            else if (Global.AmountPaid != 0)
            {
                try
                {
                    var commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
                    commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
                    commType.EnableCoins();
                    commType.DispenseChange(Global.AmountPaid);
                    commType.CloseDevices();

                    Global.AmountInCredit = 0;
                    Global.AmountPaid = 0;
                }
                catch { }
            }

            Global.Dispose();

            LoginVisible = true;
            MyAccountVisible = false;
            MyAccountText = "";
            LogoutVisible = false;
            RefillButtonVisible = false;
            UserRegistrationVisible = true;

            App.Current.Dispatcher.Invoke(() =>
                {
                    shoppingCart.Items.Clear();
                });

            Global.ShoppingCartItemsCount = 0;

            aggregator.GetEvent<EventAggregation.LogoutCompleteEvent>().Publish(null);
            myAccount.Dispose();

            ShowHome();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void ExecuteLogoutCommand(object parameter = null)
        {
            ShoppingCartRepository shoppingRepo = new ShoppingCartRepository();
            shoppingRepo.SaveShoppingCart(shoppingCart.Items.ToList(), null, null, Global.User == null ? "" : Global.User.ProviderUserKey.ToString(), "Abandonment");
            Logout();
        }

        private void ExecuteRefillButtonCommand(object parameter = null)
        {
            ShowRefill();
        }

        private void ProcessRefill(List<PaymentItem> payments)
        {
            ItemRepository itemRepo = new ItemRepository();
            var item = itemRepo.GetItem("ACCOUNT_REFILL_BARCODE");

            if (item != null)
            {
                decimal refillAmount = payments.Sum(x => x.Payment);
                decimal reward = Helpers.RewardHelper.GetRewardAmount(refillAmount);

                if (Global.EnableRefillRewards)
                {
                    if (reward > 0)
                    {
                        payments.Add(new PaymentItem()
                        {
                            DateTime = DateTime.Now,
                            Payment = reward,
                            Source = Helpers.Enum.PaymentMethod.Reward.ToString()
                        });
                    }
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

                string cartpkid = shoppingRepo.SaveShoppingCart(cartItems, payments, null, Global.User.ProviderUserKey.ToString());

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

                userProvider.UpdateUserBalance(Global.User.UserName, refillAmount + reward, "Refill");

                Global.User = userProvider.GetUser(Global.User.UserName) as deOROMembershipUser;
                accountHistoryRepo.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance, refillAmount + reward, "Abandoned Cash", cartpkid);

            }
            else
            {
                DialogViewService.ShowAutoCloseDialog("Account Refill", "Account refill failed. ACCOUNT_REFILL_BARCODE is missing");
            }

        }

        public static void UpdatePassword()
        {
            ICreditCardProcessor processor = CreditCardProcessorFactory.CreditCardProcessorFactory.GetCreditCardProcessor();

            if (processor != null)
            {
                SynclogRepository syncLogRepo = new SynclogRepository();
                synclog synclog = new synclog();

                synclog.description = string.Format("Update Password");
                synclog.createddatetime = DateTime.Now;

                processor.userName = Global.CreditcardProcessorUserName;
                processor.password = Global.CreditcardProcessorPassword;
                processor.ChangePassword(Global.CardReaderSerialNumber, 1);

                if (Global.CreditcardProcessorPassword != processor.password && processor.password != "")
                {
                    KeyValue keyValue = new KeyValue { Key = "Password", Value = processor.password };
                    Helpers.ConfigFile.SaveSettings("cardprocessor", keyValue);

                    synclog.type = "Success";
                    synclog.message = processor.message;
                    Global.Init();
                }
                else
                {
                    synclog.type = "Failed";
                    synclog.message = processor.message;
                }

                syncLogRepo.AddSynclog(synclog);
                syncLogRepo.Save();
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }


        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExitWindowsEx(uint uFlags, uint dwReason);


        public int GetLastInputTime()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                int lastInputTick = (int)lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : 0);
        }


        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi;
        }
        [StructLayout(LayoutKind.Explicit)]
        struct MouseKeybdhardwareInputUnion
        {
            [FieldOffset(0)]
            public MouseInputData mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        struct MouseInputData
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [Flags]
        enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }
        enum SendInputEventType : int
        {
            InputMouse,
            InputKeyboard,
            InputHardware
        }

        public static void ClickLeftMouseButton()
        {
            INPUT mouseDownInput = new INPUT();
            mouseDownInput.type = SendInputEventType.InputMouse;
            mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
            SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

            INPUT mouseUpInput = new INPUT();
            mouseUpInput.type = SendInputEventType.InputMouse;
            mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
            SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
        }

        public static void ClickRightMouseButton()
        {
            INPUT mouseDownInput = new INPUT();
            mouseDownInput.type = SendInputEventType.InputMouse;
            
            mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
            SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

            INPUT mouseUpInput = new INPUT();
            mouseUpInput.type = SendInputEventType.InputMouse;
            mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
            SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
        }


        internal static void ClickLeftMouseButton(object obj)
        {
            throw new NotImplementedException();
        }
    }
}