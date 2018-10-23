using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.MDB;
using deORODataAccessApp.Models;

namespace deORO.EventAggregation
{
    public class EventLogEventArgs : EventArgs
    {

        private string @event;

        public string Event
        {
            get { return @event; }
            set { @event = value; }
        }

        private decimal source;

        public decimal Source
        {
            get { return source; }
            set { source = value; }
        }

        private string userPkid;

        public string UserPkid
        {
            get { return userPkid; }
            set { userPkid = value; }
        }

    }
    public class TransactionErrorEventArgs : EventArgs
    {
        private deORO.Helpers.Enum.DeviceType deviceType;

        public deORO.Helpers.Enum.DeviceType DeviceType
        {
            get { return deviceType; }
            set { deviceType = value; }
        }

        private string @event;

        public string Event
        {
            get { return @event; }
            set { @event = value; }
        }

        private decimal amount;

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        private string shoppingCartPkid;

        public string ShoppingCartPkid
        {
            get { return shoppingCartPkid; }
            set { shoppingCartPkid = value; }
        }

        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

    }

    public class DispenseEventArgs : EventArgs
    {
        List<decimal> coins;

        public List<decimal> Coins
        {
            get { return coins; }
            set { coins = value; }
        }
        decimal coinsValue;

        public decimal CoinsValue
        {
            get { return this.coinsValue; }
            set { this.coinsValue = value; }
        }

        List<decimal> notes;

        public List<decimal> Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        decimal notesValue;

        public decimal NotesValue
        {
            get { return notesValue; }
            set { notesValue = value; }
        }

    }

    public class DeviceFailEventArgs : EventArgs
    {
        private deORO.Helpers.Enum.DeviceType deviceType;

        public deORO.Helpers.Enum.DeviceType DeviceType
        {
            get { return deviceType; }
            set { deviceType = value; }
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    public class CoinAndBillStatusEventArgs : EventArgs
    {
        private StackerInfo stacker;

        public StackerInfo Stacker
        {
            get { return stacker; }
            set { stacker = value; }
        }


        private List<TubeInfo> tubes;

        public List<TubeInfo> Tubes
        {
            get { return tubes; }
            set { tubes = value; }
        }

    }

        
    public class CashEventArgs : EventArgs
    {
        private decimal amount;

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        private string routing;

        public string Routing
        {
            get { return routing; }
            set { routing = value; }
        }
    }

    public class PaymentCompleteEventArgs : EventArgs
    {
        private string source;
        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        private List<PaymentItem> paymentItems;
        public List<PaymentItem> PaymentItems
        {
            get { return paymentItems; }
            set { paymentItems = value; }
        }

        private TransactionError error;
        public TransactionError Error
        {
            get { return error; }
            set { error = value; }
        }

        decimal change;

        public decimal Change
        {
            get { return change; }
            set { change = value; }
        }

    }
}
