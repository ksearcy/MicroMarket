using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.ViewModels;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.EventAggregation;
using deORODataAccessApp.Models;

namespace deORO.Helpers
{
    public class Global
    {
        private static deOROMembershipUser user = null;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private static Dictionary<string, List<KeyValue>> applicationSettings;
        private static string invalidUserBarcode;
        private static decimal creditToAccount;
        private static decimal amountDue;
        private static decimal amountPaid;
        private static decimal amountInCredit;

        private static int locationId;
        private static string billAcceptorCOMPort;
        private static string pupMode;
        private static int customerId;
        private static int imageCycleInterval;
        private static string fromMailAddress;
        private static string imagesPath;
        private static string runMode;
        private static string smptServer;
        private static int smtpPort;
        private static int smtpAuthenticationMode;
        private static string smtpUserName;
        private static string smtpPassword;
        private static bool enableSSL;
        private static string adminUserId;
        private static string adminPassword;
        private static string slideShowImagesPath;
        private static string toAddress;
        private static string ccAddress;
        private static string creditcardProcessorUserName;
        private static string creditcardProcessorPassword;
        private static string creditcardProcessorUrl;
        private static string cardReaderSerialNumber;
        private static int cardReaderType;
        private static string cardType;
        private static string cardReaderDeviceName;
        private static string barcodeReaderDeviceName;
        private static string disableCancelTransactionAfter;
        private static bool enableVirtualKeyboard;
        private static string deOROServiceUrl;
        private static string deOROServiceAccessUserName;
        private static string deOROServiceAccessPassword;
        private static string creditCardProcessor;
        private static string heartlandSecretApiKey;
        private static bool zipCodeRequired;
        private static string kmtronic;
        private static int kmtronicTimeout;
        private static int idleTimeout;
        private static string locationName;
        private static string customerName;
        private static string autologoffCountdownTimer;
        private static int syncInterval;
        private static bool barcodeLogin;
        private static bool enableCameraFeed;
        private static int cameraCycleInterval;
        private static int enable20WhenAmountIsGreaterOrEqualTo;
        private static bool enableBill;
        private static bool enableCoin;
        private static int autoCloseMessage;
        private static bool enableFingerprintAuthentication;
        private static bool enableDiscounts;
        private static bool defaultToNumericKeyPad;
        private static bool emailRequiredToRegister;
        private static string encoding;
        private static string refillRewardTier1;
        private static string refillRewardTier2;
        private static string refillRewardTier3;
        private static string refillRewardTier4;
        private static bool enableRefillRewards;
        private static string cardReaderMake;
        private static string cardReaderCOMPort;
        private static bool enableDispenseChange;
        private static decimal newUserCredit;
        private static bool simulateFingerprintLogin;
        private static bool demoMode;
        private static string barcodeReaderDeviceMake;
        private static string cardProcessorCompanyName;
        private static bool firstLastNameRequiredToRegister;
        private static string mdbVendor;
        private static string copyFrom;
        private static string marshallCOMPort;
        private static int cardProcessorTimeout;
        private static string hardwareId;
        private static bool billDispenser;
        private static bool coinHopper;
        private static string recycleNote;
        private static string ip;
        private static string currency;
        private static decimal disableCoinDispenseWhenChangeIsGreaterThan;
        private static int fingerprintThresholdScore;
        private static string noteReaderNoteSetPayment;
        private static string noteReaderNoteSetRefil;
        private static string orientation;
        private static bool collapseRightPane;
        private static string companyEmail;
        private static decimal creditRound;
        private static decimal creditAddRefill;
        private static bool dobAndGenderRequired;
        private static string ftpHostName;
        private static int ftpPort;
        private static string ftpUser;
        private static string ftpPassword;
        private static string ftpCustomer;
        private static string ftpLocation;
        private static string loginPrefix;
        private static string paymentOptions;
        private static List<PaymentItem> paymentItems = new List<PaymentItem>();
        private static bool printerConnected;
        private static string portOptions;
        private static string portSettings;
        private static List<ShoppingCartItem> shoppingCartItemsForPrint = new List<ShoppingCartItem>();
        private static string paymentMethodForPrint;
        private static string shoppingCartIdForPrint;
        private static string dialogTypeForPrint;
        private static decimal previousAccountBalanceForPrint;
        private static decimal refillAmountForPrint;
        private static bool autoCredit;
        private static int iDTECHReaderBusyCount = 0;
        private static bool correctChange;
        private static string conversionPrefix;
        private static decimal conversionReward;
        private static string dynamicPanelDialogTitle;
        private static string dynamicPanelDialogMessage;
        private static bool disablePopups;
        private static string dynamicPanelDialogType = "Successful";
        private static bool dispenseInIdleTimeout;
        private static bool damagedBarcodeSectionLoaded = false;
        private static bool addItemsVisible = false;
        private static bool searchVisible = false;
        private static bool refillButtonVisible = false;
        private static string mainScreen;
        private static string secondScreen;
        private static bool enableCardAuthentication;
        private static DateTime coincoBillLastEnableDateTime;
        private static DateTime coincoCoinLastEnableDateTime;
        private static int coincoBillLastEnableResult;


        public static DateTime CoincoCoinLastEnableDateTime
        {
            get { return Global.coincoCoinLastEnableDateTime; }
            set { Global.coincoCoinLastEnableDateTime = value; }
        }

        public static DateTime CoincoBillLastEnableDateTime
        {
            get { return Global.coincoBillLastEnableDateTime; }
            set { Global.coincoBillLastEnableDateTime = value; }
        }

        public static int CoincoBillLastEnableResult
        {
            get { return Global.coincoBillLastEnableResult; }
            set { Global.coincoBillLastEnableResult = value; }
        }


        public static bool EnableCardAuthentication
        {
            get
            {
                Global.enableCardAuthentication = Convert.ToBoolean(GetSettingValue("application", "EnableCardAuthentication"));
                return Global.enableCardAuthentication;
            }
            set { Global.enableCardAuthentication = value; }
        }

        public static string SecondScreen
        {
            get
            {
                Global.secondScreen = Convert.ToString(GetSettingValue("application", "SecondScreen"));
                return Global.secondScreen;
            }
            set { Global.secondScreen = value; }
        }
        
        public static string MainScreen
        {
            get
            {
                Global.mainScreen = Convert.ToString(GetSettingValue("application", "MainScreen"));
                return Global.mainScreen;
            }
            set { Global.mainScreen = value; }
        }

        public static bool RefillButtonVisible
        {
            get
            {
                Global.refillButtonVisible = Convert.ToBoolean(GetSettingValue("application", "RefillButtonVisible"));
                return Global.refillButtonVisible;
            }
            set { Global.refillButtonVisible = value; }
        }

        public static bool SearchVisible
        {
            get
            {
                Global.searchVisible = Convert.ToBoolean(GetSettingValue("application", "SearchVisible"));
                return Global.searchVisible;
            }
            set { Global.searchVisible = value; }
        }

        public static bool AddItemsVisible
        {
            get {
                Global.addItemsVisible = Convert.ToBoolean(GetSettingValue("application", "AddItemsVisible"));
                return Global.addItemsVisible;
            }
            set { Global.addItemsVisible = value; }
        }



        public static bool DamagedBarcodeSectionLoaded
        {
            get { return Global.damagedBarcodeSectionLoaded; }
            set { Global.damagedBarcodeSectionLoaded = value; }
        }


        public static decimal AmountInCredit
        {
            get { return Global.amountInCredit; }
            set { Global.amountInCredit = value; }
        }

        public static int IDTECHReaderBusyCount
        {
            get { return Global.iDTECHReaderBusyCount; }
            set { Global.iDTECHReaderBusyCount = value; }
        }

        public static string DynamicPanelDialogType
        {
            get { return Global.dynamicPanelDialogType; }
            set { Global.dynamicPanelDialogType = value; }
        }


        public static string DynamicPanelDialogTitle
        {
            get { return Global.dynamicPanelDialogTitle; }
            set { Global.dynamicPanelDialogTitle = value; }
        }

        public static string DynamicPanelDialogMessage
        {
            get { return Global.dynamicPanelDialogMessage; }
            set { Global.dynamicPanelDialogMessage = value; }
        }


        public static decimal ConversionReward
        {
            get
            {
                Global.conversionReward = Convert.ToDecimal(GetSettingValue("refillrewards", "ConversionReward"));
                return Global.conversionReward;
            }
            set { Global.conversionReward = value; }
        }

        public static string ConversionPrefix
        {
            get
            {
                Global.conversionPrefix = GetSettingValue("barcodereader", "ConversionPrefix");
                return Global.conversionPrefix;
            }
            set { Global.conversionPrefix = value; }
        }

        public static bool PrinterConnected
        {
            get
            {
                Global.printerConnected = Convert.ToBoolean(GetSettingValue("printer", "PrinterConnected"));
                return Global.printerConnected;
            }
            set { Global.printerConnected = value; }
        }

        public static string PortOptions
        {
            get
            {
                Global.ftpUser = GetSettingValue("printer", "PortOptions");
                return Global.portOptions;
            }
            set { Global.portOptions = value; }
        }

        public static string PortSettings
        {
            get
            {
                Global.ftpUser = GetSettingValue("printer", "PrinterSettings");
                return Global.portSettings;
            }
            set { Global.portSettings = value; }
        }

        public static string PaymentMethodForPrint
        {
            get { return Global.paymentMethodForPrint; }
            set { Global.paymentMethodForPrint = value; }
        }

        public static string ShoppingCartIdForPrint
        {
            get { return Global.shoppingCartIdForPrint; }
            set { Global.shoppingCartIdForPrint = value; }
        }

        public static string DialogTypeForPrint
        {
            get { return Global.dialogTypeForPrint; }
            set { Global.dialogTypeForPrint = value; }
        }

        public static decimal PreviousAccountBalanceForPrint
        {
            get { return Global.previousAccountBalanceForPrint; }
            set { Global.previousAccountBalanceForPrint = value; }
        }

        public static decimal RefillAmountForPrint
        {
            get { return Global.refillAmountForPrint; }
            set { Global.refillAmountForPrint = value; }
        }
        

        public static List<PaymentItem> PaymentItems
        {
            get { return Global.paymentItems; }
            set { Global.paymentItems = value; }
        }

        public static List<ShoppingCartItem> ShoppingCartItemsForPrint
        {
            get { return Global.shoppingCartItemsForPrint; }
            set { Global.shoppingCartItemsForPrint = value; }
        }

        public static decimal AmountPaid
        {
            get { return Global.amountPaid; }
            set { Global.amountPaid = value; }
        }

        public static string InvalidUserBarcode
        {
            get { return Global.invalidUserBarcode; }
            set { Global.invalidUserBarcode = value; }
        }

        public static string PaymentOptions
        {
            get
            {
                Global.paymentOptions = GetSettingValue("application", "PaymentOptions");
                return Global.paymentOptions;
            }
            set { Global.paymentOptions = value; }
        }

        public static string LoginPrefix
        {
            get
            {
                Global.loginPrefix = GetSettingValue("barcodereader", "LoginPrefix");
                return Global.loginPrefix;
            }
            set { Global.loginPrefix = value; }
        }

        public static string FtpCustomer
        {
            get
            {
                Global.ftpCustomer = GetSettingValue("ftp", "FtpCustomer");
                return Global.ftpCustomer;
            }
            set { Global.ftpCustomer = value; }
        }

        public static string FtpLocation
        {
            get
            {
                Global.ftpLocation = GetSettingValue("ftp", "FtpLocation");
                return Global.ftpLocation;
            }
            set { Global.ftpLocation = value; }
        }

        public static string FtpPassword
        {
            get
            {
                Global.ftpPassword = GetSettingValue("ftp", "FtpPassword");
                return Global.ftpPassword;
            }
            set { Global.ftpPassword = value; }
        }

        public static string FtpUser
        {
            get
            {
                Global.ftpUser = GetSettingValue("ftp", "FtpUser");
                return Global.ftpUser;
            }
            set { Global.ftpUser = value; }
        }


        public static int FtpPort
        {
            get
            {
                Global.ftpPort = Convert.ToInt32(GetSettingValue("ftp", "FtpPort"));
                return Global.ftpPort;
            }
            set { Global.ftpPort = value; }
        }

     

        public static string FtpHostName
        {
            get
            {
                Global.ftpHostName = GetSettingValue("ftp", "FtpHostName");
                return Global.ftpHostName;
            }
            set { Global.ftpHostName = value; }
        }


        public static bool DOBAndGenderRequired
        {
            get
            {
                Global.dobAndGenderRequired = Convert.ToBoolean(GetSettingValue("application", "DOBandGenderRequired"));
                return Global.dobAndGenderRequired;
            }
            set { Global.dobAndGenderRequired = value; }
        }


        public static decimal CreditRound
        {
            get
            {
                Global.creditRound = Convert.ToDecimal(GetSettingValue("cardprocessor", "CreditRound"), System.Globalization.CultureInfo.InvariantCulture);
                return Global.creditRound;
            }
            set { Global.creditRound = value; }
        }

        public static decimal CreditAddRefill
        {
            get
            {
                Global.creditAddRefill = Convert.ToDecimal(GetSettingValue("cardprocessor", "CreditAddRefill"), System.Globalization.CultureInfo.InvariantCulture);
                return Global.creditAddRefill;
            }
            set { Global.creditAddRefill = value; }
        }

        public static string CompanyEmail
        {
            get
            {
                Global.companyEmail = GetSettingValue("application", "CompanyEmail");
                return Global.companyEmail;
            }
            set { Global.companyEmail = value; }
        }

        public static bool CollapseRightPane
        {
            get
            {
                Global.collapseRightPane = Convert.ToBoolean(GetSettingValue("application", "CollapseRightPane"));
                return Global.collapseRightPane;
            }
            set { Global.collapseRightPane = value; }
        }

        public static string Orientation
        {
            get
            {
                Global.orientation = GetSettingValue("application", "Orientation");
                return Global.orientation;
            }
            set { Global.orientation = value; }
        }

        public static string NoteReaderNoteSetRefil
        {
            get
            {
                Global.noteReaderNoteSetRefil = GetSettingValue("application", "NoteReaderNoteSetRefil");
                return Global.noteReaderNoteSetRefil;
            }
            set { Global.noteReaderNoteSetPayment = value; }
        }

        public static string NoteReaderNoteSetPayment
        {
            get
            {
                Global.noteReaderNoteSetPayment = GetSettingValue("application", "NoteReaderNoteSetPayment");
                return Global.noteReaderNoteSetPayment;
            }
            set { Global.noteReaderNoteSetPayment = value; }
        }


        public static int FingerprintThresholdScore
        {
            get
            {
                Global.fingerprintThresholdScore = Convert.ToInt32(GetSettingValue("application", "FingerprintThresholdScore"));
                return Global.fingerprintThresholdScore;
            }
            set { Global.fingerprintThresholdScore = value; }
        }


        public static decimal DisableCoinDispenseWhenChangeIsGreaterThan
        {
            get
            {
                Global.disableCoinDispenseWhenChangeIsGreaterThan = Convert.ToDecimal(GetSettingValue("application", "DisableCoinDispenseWhenChangeIsGreaterThan"), System.Globalization.CultureInfo.InvariantCulture);
                return Global.disableCoinDispenseWhenChangeIsGreaterThan;
            }
            set
            {
                Global.disableCoinDispenseWhenChangeIsGreaterThan = value;
            }
        }

        public static string Currency
        {
            get
            {
                Global.currency = GetSettingValue("application", "Currency");
                return Global.currency;
            }
            set { Global.currency = value; }
        }


        public static string IP
        {
            get
            {
                Global.ip = GetSettingValue("cardprocessor", "IP");
                return Global.ip;
            }
            set { Global.ip = value; }
        }
        private static string ipPort;

        public static string IPPort
        {
            get
            {
                Global.ipPort = GetSettingValue("cardprocessor", "IPPort");
                return Global.ipPort;
            }
            set { Global.ipPort = value; }
        }

        private static string cardKnoxKey;

        public static string CardKnoxKey
        {
            get
            {
                Global.cardKnoxKey = GetSettingValue("cardprocessor", "CardKnoxKey");
                return Global.cardKnoxKey;
            }
            set { Global.cardKnoxKey = value; }
        }
        private static string apiVersion;

        public static string ApiVersion
        {
            get
            {
                Global.apiVersion = GetSettingValue("cardprocessor", "ApiVersion");
                return Global.apiVersion;
            }
            set { Global.apiVersion = value; }
        }

        public static string RecycleNote
        {
            get
            {
                Global.recycleNote = GetSettingValue("dispense", "RecycleNote");
                return Global.recycleNote;
            }
            set { Global.recycleNote = value; }
        }

        public static bool BillDispenser
        {
            get
            {
                Global.billDispenser = Convert.ToBoolean(GetSettingValue("dispense", "BillDispenser"));
                return Global.billDispenser;
            }
            set { Global.billDispenser = value; }
        }

        public static bool CoinHopper
        {
            get
            {
                Global.coinHopper = Convert.ToBoolean(GetSettingValue("dispense", "CoinHopper"));
                return Global.coinHopper;
            }
            set { Global.coinHopper = value; }
        }


        public static string HardwareId
        {
            get
            {
                Global.hardwareId = GetSettingValue("marshall", "HardwareId");
                return Global.hardwareId;
            }
            set { Global.hardwareId = value; }
        }

        public static int CardProcessorTimeout
        {
            get
            {
                Global.cardProcessorTimeout = Convert.ToInt32(GetSettingValue("cardprocessor", "CardProcessorTimeout"));
                return Global.cardProcessorTimeout;
            }
            set { Global.cardProcessorTimeout = value; }
        }

        public static string MarshallCOMPort
        {
            get
            {
                Global.marshallCOMPort = GetSettingValue("marshall", "COMPort");
                return Global.marshallCOMPort;
            }
            set { Global.marshallCOMPort = value; }
        }

        public static string CopyFrom
        {
            get
            {
                Global.copyFrom = GetSettingValue("application", "CopyFrom");
                return Global.copyFrom;
            }
            set { Global.copyFrom = value; }
        }

        public static string MDBVendor
        {
            get
            {
                Global.mdbVendor = GetSettingValue("application", "MDBVendor");
                return Global.mdbVendor;
            }
            set { Global.mdbVendor = value; }
        }

        public static bool FirstLastNameRequiredToRegister
        {
            get
            {
                Global.firstLastNameRequiredToRegister = Convert.ToBoolean(GetSettingValue("application", "FirstLastNameRequiredToRegister"));
                return Global.firstLastNameRequiredToRegister;
            }
            set { Global.firstLastNameRequiredToRegister = value; }
        }

        public static string CardProcessorCompanyName
        {
            get
            {
                Global.cardProcessorCompanyName = GetSettingValue("cardprocessor", "CardProcessorCompanyName");
                return Global.cardProcessorCompanyName;
            }
            set { Global.cardProcessorCompanyName = value; }
        }

        public static string BarcodeReaderDeviceMake
        {
            get
            {
                Global.barcodeReaderDeviceMake = GetSettingValue("barcodereader", "BarcodeReaderDeviceMake");
                return Global.barcodeReaderDeviceMake;
            }
            set { Global.barcodeReaderDeviceMake = value; }
        }

        public static bool DemoMode
        {
            get
            {
                Global.demoMode = Convert.ToBoolean(GetSettingValue("application", "DemoMode"));
                return Global.demoMode;
            }
            set { Global.demoMode = value; }
        }

        public static bool SimulateFingerprintLogin
        {
            get
            {
                Global.simulateFingerprintLogin = Convert.ToBoolean(GetSettingValue("application", "SimulateFingerprintLogin"));
                return Global.simulateFingerprintLogin;
            }
            set { Global.simulateFingerprintLogin = value; }
        }

        public static decimal NewUserCredit
        {
            get
            {
                Global.newUserCredit = Convert.ToDecimal(GetSettingValue("application", "NewUserCredit"), System.Globalization.CultureInfo.InvariantCulture);
                return Global.newUserCredit;
            }
            set { Global.newUserCredit = value; }
        }

        public static bool EnableDispenseChange
        {
            get
            {
                Global.enableDispenseChange = Convert.ToBoolean(GetSettingValue("application", "EnableDispenseChange"));
                return Global.enableDispenseChange;
            }
            set { Global.enableDispenseChange = value; }
        }

        public static string CardReaderCOMPort
        {
            get
            {
                Global.cardReaderCOMPort = GetSettingValue("cardreader", "CardReaderCOMPort");
                return Global.cardReaderCOMPort;
            }
            set { Global.cardReaderCOMPort = value; }
        }

        public static string CardReaderMake
        {
            get
            {
                Global.cardReaderMake = GetSettingValue("cardreader", "CardReaderMake");
                return Global.cardReaderMake;
            }
            set { Global.cardReaderMake = value; }
        }


        public static bool EnableRefillRewards
        {
            get
            {
                Global.enableRefillRewards = Convert.ToBoolean(GetSettingValue("refillrewards", "EnableRefillRewards"));
                return Global.enableRefillRewards;
            }
            set { Global.enableRefillRewards = value; }
        }


        public static string RefillRewardTier1
        {
            get
            {
                Global.refillRewardTier1 = GetSettingValue("refillrewards", "1-4.99");
                return Global.refillRewardTier1;
            }
            set { Global.refillRewardTier1 = value; }
        }



        public static string RefillRewardTier2
        {
            get
            {
                Global.refillRewardTier2 = GetSettingValue("refillrewards", "5-9.99");
                return Global.refillRewardTier2;
            }
            set { Global.refillRewardTier2 = value; }
        }



        public static string RefillRewardTier3
        {
            get
            {
                Global.refillRewardTier3 = GetSettingValue("refillrewards", "10-19.99");
                return Global.refillRewardTier3;
            }
            set { Global.refillRewardTier3 = value; }
        }



        public static string RefillRewardTier4
        {
            get
            {
                Global.refillRewardTier4 = GetSettingValue("refillrewards", "20+");
                return Global.refillRewardTier4;
            }
            set { Global.refillRewardTier4 = value; }
        }

        public static string Encoding
        {
            get
            {
                Global.encoding = GetSettingValue("cardreader", "Encoding");
                return Global.encoding;
            }
            set { Global.encoding = value; }
        }

        public static bool EmailRequiredToRegister
        {
            get
            {
                Global.emailRequiredToRegister = Convert.ToBoolean(GetSettingValue("application", "EmailRequiredToRegister"));
                return Global.emailRequiredToRegister;
            }
            set { Global.emailRequiredToRegister = value; }
        }

        public static bool DefaultToNumericKeyPad
        {
            get
            {
                Global.defaultToNumericKeyPad = Convert.ToBoolean(GetSettingValue("application", "DefaultToNumericKeyPad"));
                return Global.defaultToNumericKeyPad;
            }
            set { Global.defaultToNumericKeyPad = value; }
        }

        public static bool EnableDiscounts
        {
            get
            {
                Global.enableDiscounts = Convert.ToBoolean(GetSettingValue("application", "EnableDiscounts"));
                return Global.enableDiscounts;
            }
            set { Global.enableDiscounts = value; }
        }

        public static bool EnableFingerprintAuthentication
        {
            get
            {
                Global.enableFingerprintAuthentication = Convert.ToBoolean(GetSettingValue("application", "EnableFingerprintAuthentication"));
                return Global.enableFingerprintAuthentication;
            }
            set { Global.enableFingerprintAuthentication = value; }
        }

        public static int AutoCloseMessage
        {
            get
            {
                Global.autoCloseMessage = Convert.ToInt32(GetSettingValue("application", "AutoCloseMessage"));
                return Global.autoCloseMessage;
            }
            set { Global.autoCloseMessage = value; }
        }

        public static bool EnableCoin
        {
            get
            {
                Global.enableCoin = Convert.ToBoolean(GetSettingValue("application", "EnableCoin"));
                return Global.enableCoin;
            }
            set { Global.enableCoin = value; }
        }

        public static bool EnableBill
        {
            get
            {
                Global.enableBill = Convert.ToBoolean(GetSettingValue("application", "EnableBill"));
                return Global.enableBill;
            }
            set { Global.enableBill = value; }
        }

        private static string communicationType;

        public static string CommunicationType
        {
            get
            {
                Global.communicationType = GetSettingValue("application", "CommunicationType");
                return Global.communicationType;
            }
            set { Global.communicationType = value; }
        }

        public static int Enable20WhenAmountIsGreaterOrEqualTo
        {
            get
            {
                Global.enable20WhenAmountIsGreaterOrEqualTo = Convert.ToInt32(GetSettingValue("application", "Enable20WhenAmountIsGreaterOrEqualTo"));
                return Global.enable20WhenAmountIsGreaterOrEqualTo;
            }
            set { Global.enable20WhenAmountIsGreaterOrEqualTo = value; }
        }

        public static int CameraCycleInterval
        {
            get
            {
                Global.cameraCycleInterval = Convert.ToInt32(GetSettingValue("application", "CameraCycleInterval"));
                return Global.cameraCycleInterval;
            }
            set { Global.cameraCycleInterval = value; }
        }


        private static string currentViewModel;

        public static string CurrentViewModel
        {
            get { return currentViewModel; }
            set { currentViewModel = value; }
        }


        private static decimal round;

        public static decimal Round
        {
            get
            {
                Global.round = Convert.ToDecimal(GetSettingValue("application", "Round"), System.Globalization.CultureInfo.InvariantCulture);
                return Global.round;
            }
            set { Global.round = value; }
        }

        private static string videosPath;

        public static string VideosPath
        {
            get
            {
                Global.videosPath = GetSettingValue("application", "VideosPath");
                return Global.videosPath;
            }
            set { Global.videosPath = value; }
        }


        public static bool EnableCameraFeed
        {
            get
            {
                Global.enableCameraFeed = Convert.ToBoolean(GetSettingValue("application", "EnableCameraFeed"));
                return Global.enableCameraFeed;
            }
            set { Global.enableCameraFeed = value; }
        }

        private static string camera1;
        public static string Camera1
        {
            get
            {
                Global.camera1 = GetSettingValue("camerafeed", "Camera1");
                return Global.camera1;
            }
            set { Global.camera1 = value; }
        }

        private static string camera2;
        public static string Camera2
        {
            get
            {
                Global.camera2 = GetSettingValue("camerafeed", "Camera2");
                return Global.camera2;
            }
            set { Global.camera2 = value; }
        }

        private static string camera3;
        public static string Camera3
        {
            get
            {
                Global.camera3 = GetSettingValue("camerafeed", "Camera3");
                return Global.camera3;
            }
            set { Global.camera3 = value; }
        }

        private static string camera4;
        public static string Camera4
        {
            get
            {
                Global.camera4 = GetSettingValue("camerafeed", "Camera4");
                return Global.camera4;
            }
            set { Global.camera4 = value; }
        }

        public static bool BarcodeLogin
        {
            get
            {
                Global.barcodeLogin = Convert.ToBoolean(GetSettingValue("application", "BarcodeLogin"));
                return Global.barcodeLogin;
            }
            set { Global.barcodeLogin = value; }
        }

        public static int SyncInterval
        {
            get
            {

                Global.syncInterval = Convert.ToInt32(GetSettingValue("application", "SyncInterval"));
                return Global.syncInterval;
            }
            set { Global.syncInterval = value; }
        }

        public static string AutologoffCountdownTimer
        {
            get
            {

                Global.autologoffCountdownTimer = GetSettingValue("application", "AutologoffCountdownTimer");
                return Global.autologoffCountdownTimer;
            }
            set { Global.autologoffCountdownTimer = value; }
        }

        public static string CustomerName
        {
            get
            {

                Global.customerName = GetSettingValue("application", "CustomerName");
                return Global.customerName;
            }
            set { Global.customerName = value; }
        }

        public static string LocationName
        {
            get
            {
                Global.locationName = GetSettingValue("application", "LocationName");
                return Global.locationName;
            }
            set { Global.locationName = value; }
        }



        public static string RunMode
        {
            get
            {
                Global.runMode = GetSettingValue("application", "RunMode");
                return Global.runMode;
            }
            set { Global.runMode = value; }
        }

        public static int IdleTimeout
        {
            get
            {
                Global.idleTimeout = Convert.ToInt32(GetSettingValue("application", "IdleTimeout"));
                return Global.idleTimeout;
            }
            set { Global.idleTimeout = value; }
        }


        public static int KMtronicTimeout
        {
            get
            {
                Global.kmtronicTimeout = Convert.ToInt32(GetSettingValue("usbrelay", "KMtronicTimeout"));
                return Global.kmtronicTimeout;
            }
            set { Global.kmtronicTimeout = value; }
        }

        public static string KMtronic
        {
            get
            {
                Global.kmtronic = GetSettingValue("usbrelay", "KMtronic");
                return Global.kmtronic;
            }
            set { Global.kmtronic = value; }
        }

        public static bool ZipCodeRequired
        {
            get
            {
                Global.zipCodeRequired = Convert.ToBoolean(GetSettingValue("cardprocessor", "ZipCodeRequired"));
                return Global.zipCodeRequired;
            }
            set { Global.zipCodeRequired = value; }
        }

        public static string HeartlandSecretApiKey
        {
            get
            {
                Global.heartlandSecretApiKey = GetSettingValue("cardprocessor", "HeartlandSecretApiKey");
                return Global.heartlandSecretApiKey;
            }
            set { Global.heartlandSecretApiKey = value; }
        }


        public static string CreditCardProcessor
        {
            get
            {
                Global.creditCardProcessor = GetSettingValue("application", "CreditCardProcessor");
                return Global.creditCardProcessor;
            }
            set { Global.creditCardProcessor = value; }
        }


        public static string DeOROServiceAccessUserName
        {
            get
            {
                Global.deOROServiceAccessUserName = GetSettingValue("application", "deOROServiceAccessUserName");
                return Global.deOROServiceAccessUserName;
            }
            set { Global.deOROServiceAccessUserName = value; }
        }


        public static string DeOROServiceAccessPassword
        {
            get
            {
                Global.deOROServiceAccessPassword = GetSettingValue("application", "deOROServiceAccessPassword");
                return Global.deOROServiceAccessPassword;
            }
            set { Global.deOROServiceAccessPassword = value; }
        }

        public static string DeOROServiceUrl
        {
            get
            {
                Global.deOROServiceUrl = GetSettingValue("application", "deOROServiceUrl");
                return Global.deOROServiceUrl;
            }
            set
            {
                Global.deOROServiceUrl = value;
            }
        }

        public static bool EnableVirtualKeyboard
        {
            get
            {
                Global.enableVirtualKeyboard = Convert.ToBoolean(GetSettingValue("application", "EnableVirtualKeyboard"));
                return Global.enableVirtualKeyboard;
            }
            set
            {
                Global.enableVirtualKeyboard = value;
            }
        }

        public static string DisableCancelTransactionAfter
        {
            get
            {
                Global.DisableCancelTransactionAfter = GetSettingValue("application", "DisableCancelTransactionAfter");
                return Global.disableCancelTransactionAfter;
            }
            set { Global.disableCancelTransactionAfter = value; }
        }

        private static Email email;

        public static Email Email
        {
            get { return Global.email = Email.Instance; }
        }


        public static string ImagesPath
        {
            get
            {
                Global.imagesPath = GetSettingValue("application", "ImagesPath");
                return Global.imagesPath;
            }
            set { Global.imagesPath = value; }
        }

        public static string BarcodeReaderDeviceName
        {
            get
            {
                Global.barcodeReaderDeviceName = GetSettingValue("barcodereader", "DeviceName");
                return Global.barcodeReaderDeviceName;
            }
            set { Global.barcodeReaderDeviceName = value; }
        }

        public static string CardReaderDeviceName
        {
            get
            {
                Global.cardReaderDeviceName = GetSettingValue("cardreader", "DeviceName");
                return Global.cardReaderDeviceName;
            }
            set { Global.cardReaderDeviceName = value; }
        }



        public static string CreditcardProcessorUserName
        {
            get
            {
                Global.creditcardProcessorUserName = GetSettingValue("cardprocessor", "UserName");
                return Global.creditcardProcessorUserName;
            }
            set { Global.creditcardProcessorUserName = value; }
        }


        public static string CreditcardProcessorPassword
        {
            get
            {
                Global.creditcardProcessorPassword = GetSettingValue("cardprocessor", "Password");
                return Global.creditcardProcessorPassword;
            }
            set { Global.creditcardProcessorPassword = value; }
        }


        public static string CreditcardProcessorUrl
        {
            get
            {
                Global.creditcardProcessorUrl = GetSettingValue("cardprocessor", "Url");
                return Global.creditcardProcessorUrl;
            }
            set { Global.creditcardProcessorUrl = value; }
        }


        public static string CardReaderSerialNumber
        {
            get
            {
                Global.cardReaderSerialNumber = GetSettingValue("cardreader", "SerialNumber");
                return Global.cardReaderSerialNumber;
            }
            set { Global.cardReaderSerialNumber = value; }
        }

        public static int CardReaderType
        {
            get
            {
                Global.cardReaderType = Convert.ToInt32(GetSettingValue("cardreader", "CardReaderType"));
                return Global.cardReaderType;
            }
            set { Global.cardReaderType = value; }
        }

        public static string CardType
        {
            get
            {
                Global.cardType = GetSettingValue("cardreader", "CardType");
                return Global.cardType;
            }
            set { Global.cardType = value; }
        }

        public static string CcAddress
        {
            get
            {
                Global.ccAddress = GetSettingValue("mail", "CCAddress");
                return Global.ccAddress;
            }
            set { Global.ccAddress = value; }
        }

        public static string ToAddress
        {
            get
            {
                Global.toAddress = GetSettingValue("mail", "ToAddress");
                return Global.toAddress;
            }
            set { Global.toAddress = value; }
        }

        public static string SlideShowImagesPath
        {
            get
            {
                Global.slideShowImagesPath = GetSettingValue("application", "SlideShowImagesPath");
                return Global.slideShowImagesPath;

            }
            set { Global.slideShowImagesPath = value; }
        }

        public static Dictionary<string, List<KeyValue>> ApplicationSettings
        {
            get { return Global.applicationSettings; }
            set { Global.applicationSettings = value; }
        }

        public static string AdminPassword
        {
            get
            {
                Global.adminPassword = GetSettingValue("application", "AdminPassword");
                return Global.adminPassword;
            }
            set { Global.adminPassword = value; }
        }

        public static string AdminUserId
        {
            get
            {
                Global.adminUserId = GetSettingValue("application", "AdminUserId");
                return Global.adminUserId;
            }
            set { Global.adminUserId = value; }
        }

        public static string SmptServer
        {
            get
            {
                Global.smptServer = GetSettingValue("mail", "SmtpServer");
                return Global.smptServer;
            }
            set { Global.smptServer = value; }
        }

        public static int SmtpPort
        {
            get
            {
                Global.smtpPort = Convert.ToInt32(GetSettingValue("mail", "SmtpPort"));
                return Global.smtpPort;
            }
            set { Global.smtpPort = value; }
        }


        public static int SmtpAuthenticationMode
        {
            get
            {
                Global.smtpAuthenticationMode = Convert.ToInt32(GetSettingValue("mail", "SmtpAuthenticationMode"));
                return Global.smtpAuthenticationMode;
            }
            set { Global.smtpAuthenticationMode = value; }
        }


        public static string SmtpUserName
        {
            get
            {
                Global.smtpUserName = GetSettingValue("mail", "SmtpUserName");
                return Global.smtpUserName;
            }
            set { Global.smtpUserName = value; }
        }


        public static string SmtpPassword
        {
            get
            {
                Global.smtpPassword = GetSettingValue("mail", "SmtpPassword");
                return Global.smtpPassword;
            }
            set { Global.smtpPassword = value; }
        }


        public static bool EnableSSL
        {
            get
            {
                Global.enableSSL = Convert.ToBoolean(GetSettingValue("mail", "EnableSSL"));
                return Global.enableSSL;
            }
            set { Global.enableSSL = value; }
        }


        public static string FromMailAddress
        {
            get
            {
                Global.fromMailAddress = GetSettingValue("mail", "FromMailAddress");
                return Global.fromMailAddress;
            }
            set { Global.fromMailAddress = value; }
        }


        public static int ImageCycleInterval
        {
            get
            {
                Global.imageCycleInterval = Convert.ToInt32(GetSettingValue("application", "ImageCycleInterval")) * 1000;
                return Global.imageCycleInterval;
            }
            set { Global.imageCycleInterval = value; }
        }

        public static string PupMode
        {
            get
            {
                Global.pupMode = GetSettingValue("bill", "PUPMode");
                return Global.pupMode;
            }
            set { Global.pupMode = value; }
        }

        public static string BillAcceptorCOMPort
        {
            get
            {
                Global.billAcceptorCOMPort = GetSettingValue("bill", "COMPort");
                return Global.billAcceptorCOMPort;
            }
            set { Global.billAcceptorCOMPort = value; }
        }

        public static int LocationId
        {
            get
            {
                Global.locationId = Convert.ToInt32(GetSettingValue("application", "LocationId"));
                return Global.locationId;
            }
            set { Global.locationId = value; }
        }


        public static int CustomerId
        {
            get
            {
                Global.customerId = Convert.ToInt32(GetSettingValue("application", "CustomerId"));
                return Global.customerId;
            }
            set { Global.customerId = value; }
        }

        public static decimal AmountDue
        {
            get { return Global.amountDue; }
            set
            {
                Global.amountDue = value;
            }
        }

        public static decimal CreditToAccount
        {
            get { return Global.creditToAccount; }
            set { Global.creditToAccount = value; }
        }

        private static int shoppingCartItemsCount;

        public static int ShoppingCartItemsCount
        {
            get { return Global.shoppingCartItemsCount; }
            set { Global.shoppingCartItemsCount = value; }
        }

        public static deOROMembershipUser User
        {
            get { return Global.user; }
            set { Global.user = value; }
        }

        private static PaymentCompleteEventArgs paymentArgs;

        public static PaymentCompleteEventArgs PaymentArgs
        {
            get { return Global.paymentArgs; }
            set { Global.paymentArgs = value; }
        }



        public static bool AutoCredit
        {
            get
            {
                Global.autoCredit = Convert.ToBoolean(GetSettingValue("application", "AutoCredit"));
                return Global.autoCredit;
            }
            set { Global.autoCredit = value; }
        }

        public static bool CorrectChange
        {
            get
            {
                Global.correctChange = Convert.ToBoolean(GetSettingValue("application", "CorrectChange"));
                return Global.correctChange;
            }
            set { Global.correctChange = value; }
        }

        public static bool DispenseInIdleTimeout
        {
            get
            {
                Global.dispenseInIdleTimeout = Convert.ToBoolean(GetSettingValue("application", "DispenseInIdleTimeout"));
                return Global.dispenseInIdleTimeout;
            }
            set { Global.dispenseInIdleTimeout = value; }
        }



        public static bool DisablePopups
        {
            get
            {
                Global.disablePopups = Convert.ToBoolean(GetSettingValue("application", "DisablePopups"));
                return Global.disablePopups;
            }
            set { Global.disablePopups = value; }
        }

        public static void Dispose()
        {
            user = null;
            amountDue = 0;
            creditToAccount = 0;
            amountPaid = 0;
            paymentArgs = null;
            paymentItems = null;

        }

        public static void Init()
        {
            if (ApplicationSettings != null)
                ApplicationSettings.Clear();

            ApplicationSettings = Helpers.ConfigFile.LoadConfigSections();
        }

        public static string GetSettingValue(string sectionName, string key)
        {
            foreach (var app in ApplicationSettings)
            {
                if (app.Key.Equals(sectionName))
                {
                    try
                    {
                        return (app.Value.Single(x => x.Key.Equals(key)) as KeyValue).Value;
                    }
                    catch
                    {
                        return "";
                    }
                }
            }

            return "";
        }
    }
}

