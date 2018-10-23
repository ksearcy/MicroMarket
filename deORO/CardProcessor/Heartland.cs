using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Composite.Events;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;

namespace deORO.CardProcessor
{
    public class Heartland : ICreditCardProcessor
    {
        private SecureSubmit.Services.HpsServicesConfig config = new SecureSubmit.Services.HpsServicesConfig();
        private SecureSubmit.Services.Credit.HpsCreditService creditService = null;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public Heartland()
        {
            config.SecretApiKey = Helpers.Global.HeartlandSecretApiKey;
            creditService = new SecureSubmit.Services.Credit.HpsCreditService(config);
        }

        //public async Task ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails, string zipCode)
        //{
        //    var creditCard = new SecureSubmit.Entities.HpsCreditCard();
        //    creditCard.Number = "4012002000060016";
        //    creditCard.ExpMonth = 12;
        //    creditCard.ExpYear = 2015;
        //    creditCard.Cvv = 123;

        //    //var creditCard = new SecureSubmit.Entities.HpsCreditCard();
        //    //creditCard.Number = msr.AccountNumber;
        //    //try { creditCard.ExpMonth = Convert.ToInt32(msr.ExpirationDate.Substring(2, 2)); }
        //    //catch { }
        //    //try { creditCard.ExpYear = Convert.ToInt32(msr.ExpirationDate.Substring(0, 2)); }
        //    //catch { }

        //    var cardHolder = new SecureSubmit.Entities.HpsCardHolder();
        //    cardHolder.Address = new SecureSubmit.Entities.HpsAddress() { Address = "6860 Dallas Pkwy", Zip = "750241234" };
        //    //cardHolder.Address = new SecureSubmit.Entities.HpsAddress() { Zip = zipCode };


        //    try
        //    {
        //        HpsCharge charge;
        //        await Task.Run(() => { charge = creditService.Charge(amount, "usd", creditCard, cardHolder); });
        //        aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Approved");
        //    }
        //    //catch (HpsInvalidRequestException)
        //    //{
        //    //    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
        //    //}
        //    //catch (HpsAuthenticationException)
        //    //{
        //    //    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
        //    //}
        //    //catch (HpsCreditException)
        //    //{
        //    //    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
        //    //}
        //    //catch (HpsGatewayException)
        //    //{
        //    //    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
        //    //}
        //    catch (Exception)
        //    {
        //        aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
        //    }
        //}

        public string userName
        {
            get
            {
                return "";
            }
            set
            {
                userName = value;
            }
        }

        public string password
        {
            get
            {
                return "";
            }
            set
            {
                userName = value;
            }
        }

        public string message
        {
            get
            {
                return "";
            }
            set
            {
                message = value;
            }
        }

        public void ChangePassword(string serialNumber, int updateStatus)
        {
            //throw new NotImplementedException();
        }

        public void ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails = "", string zipCode = "")
        {
            
        }
    }
}
