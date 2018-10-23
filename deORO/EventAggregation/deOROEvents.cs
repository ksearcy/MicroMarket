using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Composite.Presentation.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using System.Collections.ObjectModel;
using Microsoft.PointOfService;
using deORODataAccessApp.Models;
using deORO.CardProcessor;
using deORO.MDB;


namespace deORO.EventAggregation
{

    public class HidCardLoginEvent : CompositePresentationEvent<string>
    {

    }

    public class HidCardLoginSuccessfulEvent : CompositePresentationEvent<string>
    {

    }

    public class HidCardSaveSuccessfulEvent : CompositePresentationEvent<object>
    {

    }

    public class HidCardNewUserEvent : CompositePresentationEvent<object>
    {

    }

    public class HidCardSaveFailEvent : CompositePresentationEvent<object>
    {

    }

    public class HidCardCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class ResetBillRecyclerEnableEvent : CompositePresentationEvent<object>
    {

    }

    public class ResetCoinHopperEnableEvent : CompositePresentationEvent<object>
    {

    }

    public class ResetMDBBillEnableEvent : CompositePresentationEvent<object>
    {

    }

    public class ResetMDBCoinEnableEvent : CompositePresentationEvent<object>
    {

    }
    
    public class EventLogEvent : CompositePresentationEvent<object>
    {

    }

    public class DiscountMarqueeSelectComplete : CompositePresentationEvent<object>
    {

    }

    public class DiscountSelectCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class RightToolBarEnableEvent : CompositePresentationEvent<object>
    {

    }

    public class ShowHomeScreenEvent : CompositePresentationEvent<object>
    {

    }

    public class LogTransactionErrorEvent : CompositePresentationEvent<TransactionErrorEventArgs>
    {

    }

    public class BillAndCoinStatusCompletedEvent : CompositePresentationEvent<CoinAndBillStatusEventArgs>
    {

    }


    public class UserControlLoaded : CompositePresentationEvent<string>
    {

    }

    public class Relay1CloseEvent : CompositePresentationEvent<object>
    {

    }

    public class Relay2CloseEvent : CompositePresentationEvent<object>
    {

    }

    public class AutoLogoffEvent : CompositePresentationEvent<object>
    {

    }

    public class AmoutDueChangeEvent : CompositePresentationEvent<decimal>
    {

    }

    public class UserRegistrationAddEvent : CompositePresentationEvent<deOROMembershipUser>
    {

    }

    public class UserRegistrationEvent : CompositePresentationEvent<object>
    {

    }

    public class UserRegistrationCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class DeleteMeCompletedEvent : CompositePresentationEvent<object>
    {

    }

    public class DeleteMeFailedEvent : CompositePresentationEvent<object>
    {

    }

    public class UserRegistrationCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class FingerPrintSaveSuccessfulEvent : CompositePresentationEvent<object>
    {

    }

    public class FingerPrintNewUserEvent : CompositePresentationEvent<object>
    {

    }

    public class FingerPrintSaveFailEvent : CompositePresentationEvent<object>
    {

    }

    public class FingerPrintCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class AutofillBarcodeEvent : CompositePresentationEvent<object>
    {

    }

    public class PayEvent : CompositePresentationEvent<object>
    {

    }

    public class LoginEvent : CompositePresentationEvent<object>
    {

    }

    public class LoginSuccessfulEvent : CompositePresentationEvent<deOROMembershipUser>
    {

    }

    public class LoginFailEvent : CompositePresentationEvent<deORO.Helpers.Enum.AuthenticationMode>
    {

    }

    public class LoginCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class FingerPrintLoginEvent : CompositePresentationEvent<string>
    {

    }

    public class FingerPrintLoginSuccessfulEvent : CompositePresentationEvent<string>
    {

    }

    
    public class EmailUpdatedEvent : CompositePresentationEvent<deOROMembershipUser>
    {

    }

    public class PasswordUpdatedEvent : CompositePresentationEvent<deOROMembershipUser>
    {

    }

    public class CoinErrorEvent : CompositePresentationEvent<object>
    {

    }

    public class CoinAcceptedEvent : CompositePresentationEvent<CashEventArgs>
    {

    }

    public class CoinShortfallEvent : CompositePresentationEvent<decimal>
    {

    }

    public class CoinDispenseCompleteEvent : CompositePresentationEvent<DispenseEventArgs>
    {

    }

    public class CoinDispenseFailedEvent : CompositePresentationEvent<object>
    {

    }

    public class NoteDispenseFailedEvent : CompositePresentationEvent<object>
    {

    }

    public class CoinJamEvent : CompositePresentationEvent<object>
    {

    }

    public class CoinUnknownEvent : CompositePresentationEvent<object>
    {

    }

    public class CashInTubesCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class CashInTubesExtendedCompleteEvent : CompositePresentationEvent<List<Tube>>
    {

    }

    public class CashInBillsCompleteEvent : CompositePresentationEvent<object>
    {

    }


    public class BillAcceptedEvent : CompositePresentationEvent<CashEventArgs>
    {

    }

    public class BillJamEvent : CompositePresentationEvent<object>
    {

    }

    public class NoteReaderInhibitedEvent : CompositePresentationEvent<object>
    {

    }

    public class BillCheatingEvent : CompositePresentationEvent<object>
    {

    }

    public class BillUnknownEvent : CompositePresentationEvent<object>
    {

    }

    public class E2CGeneralEvent : CompositePresentationEvent<object>
    {

    }

    public class PaymentMethodCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class ShowLoginEvent : CompositePresentationEvent<object>
    {

    }

    public class ShowMyAccountEvent : CompositePresentationEvent<object>
    {

    }

    public class ShowRefillEvent : CompositePresentationEvent<object>
    {

    }

    public class ShowShoppingCartEvent : CompositePresentationEvent<object>
    {

    }

    public class ShowRegisterUserEvent : CompositePresentationEvent<object>
    {

    }

    public class UserLogoutEvent : CompositePresentationEvent<object>
    {

    }

    public class ShowHelpEvent : CompositePresentationEvent<object>
    {

    }

    public class CashPaymentEvent : CompositePresentationEvent<object>
    {

    }

    public class CashPaymentCancelEvent : CompositePresentationEvent<decimal>
    {

    }

    public class CashRefilCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class CashRefillCompleteEvent : CompositePresentationEvent<PaymentCompleteEventArgs>
    {

    }


    public class CreditCardPaymentEvent : CompositePresentationEvent<object>
    {

    }

    public class CreditCardPaymentCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class ShowPaymentOptionsEvent : CompositePresentationEvent<object>
    {

    }


    public class TransactionCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class CreditCardTransactionCompleteEvent : CompositePresentationEvent<decimal>
    {

    }

    public class CreditCardTransactionFailedEvent : CompositePresentationEvent<object>
    {

    }

    public class CoincoCashDevicesDisabled : CompositePresentationEvent<object>
    {

    }
    

    public class CreditCardRefilCancelEvent : CompositePresentationEvent<object>
    {

    }


    public class MyAccountPaymentEvent : CompositePresentationEvent<object>
    {

    }

    public class MyAccountPaymentCompleteEvent : CompositePresentationEvent<deOROMembershipUser>
    {

    }

    public class MyAccountPaymentCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class MyPayrollPaymentEvent : CompositePresentationEvent<object>
    {

    }

    public class MyPayrollPaymentCompleteEvent : CompositePresentationEvent<deOROMembershipUser>
    {

    }

    public class MyPayrollPaymentCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class PaymentCompleteEvent : CompositePresentationEvent<PaymentCompleteEventArgs>
    {
        public int SubscriptionCount
        {
            get
            {
                return Subscriptions.Count;
            }
        }
    }

    public class DynamicPanelOKButtonClick : CompositePresentationEvent<object>
    {

    }

    public class DynamicPanelPrintButtonClick : CompositePresentationEvent<object>
    {

    }

    public class ShowDynamicPanelDialog : CompositePresentationEvent<object>
    {

    }

    public class FastTouchItemSelectedEvent : CompositePresentationEvent<object>
    {

    }

    public class BarcodeScannerDataGlobalEvent : CompositePresentationEvent<object>
    {

    }

    public class BarcodeScannerDataLocalEvent : CompositePresentationEvent<object>
    {

    }


    public class LogoutCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class ConfigurationSettingsSaveSuccessfulEvent : CompositePresentationEvent<object>
    {

    }

    public class CreditCardReaderDataEvent : CompositePresentationEvent<CreditCardData>
    {

    }

    public class CreditCardWaitingForVendApprovalEvent : CompositePresentationEvent<CreditCardData>
    {

    }

    public class CreditCardReaderErrorEvent : CompositePresentationEvent<object>
    {

    }

    public class CreditCardReaderInitFailEvent : CompositePresentationEvent<object>
    {

    }

    public class DeviceOpenFailEvent : CompositePresentationEvent<DeviceFailEventArgs>
    {

    }

    public class CreditCardSetParamsFailEvent : CompositePresentationEvent<object>
    {

    }

    public class DeviceInitFailEvent : CompositePresentationEvent<DeviceFailEventArgs>
    {

    }

    public class CreditCardTransactionEvent : CompositePresentationEvent<string>
    {

    }

    public class DiscountAddCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class DiscountAddFailEvent : CompositePresentationEvent<object>
    {

    }

    public class DiscountUpdateCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class DiscountUpdateFailEvent : CompositePresentationEvent<object>
    {

    }

    public class ItemAddCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class ItemAddFailEvent : CompositePresentationEvent<object>
    {

    }

    public class ItemUpdateCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class ItemUpdateFailEvent : CompositePresentationEvent<object>
    {

    }

    public class UserAddCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class UserAddFailEvent : CompositePresentationEvent<object>
    {

    }

    public class UserUpdateCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class UserUpdateFailEvent : CompositePresentationEvent<object>
    {

    }

    public class PopupCloseEvent : CompositePresentationEvent<object>
    {

    }

    public class PopupTimeoutEvent : CompositePresentationEvent<object>
    {

    }

    public class FastTouchCloseEvent : CompositePresentationEvent<object>
    {

    }

    public class PopupCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class SyncDataCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class MissingBarcodeEvent : CompositePresentationEvent<object>
    {

    }

    public class DiscountSelectCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class MissingBarcodeCategorySelectCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class MissingBarcodeCategorySelectCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class MissingBarcodeItemSelectCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class MissingBarcodeItemAddToCartCancelEvent : CompositePresentationEvent<object>
    {

    }

    public class MissingBarcodeItemAddToCartCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class MissingBarcodeItemSelectCompleteEvent : CompositePresentationEvent<object>
    {

    }

    public class ShoppingCartItemsChangedEvent : CompositePresentationEvent<object>
    {

    }

    public class ReturnChangeOptionsEvent : CompositePresentationEvent<object>
    {

    }

    public class TubeInfoCompleteEvent : CompositePresentationEvent<List<TubeInfo>>
    {

    }


    public class OpenDamagedBarcodeItemsPanel : CompositePresentationEvent<List<TubeInfo>>
    {

    }

    public class CloseKeyboardOnTimeOut : CompositePresentationEvent<object>
    {

    }

}
