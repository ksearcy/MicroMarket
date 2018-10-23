using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.CardReader;
using deORO.Communication;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.CardProcessor
{
    public class Nayax : ICreditCardProcessor
    {
        ICommunicationType commType;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public Nayax()
        {
            commType = CommunicationTypeFactory.GetCommunicationType();
        }

        public string userName
        {
            get
            {
                return "";
            }
            set
            {

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

            }
        }


        public void ChangePassword(string serialNumber, int updateStatus)
        {
            //throw new NotImplementedException();
        }


        public void ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails = "", string zipCode = "")
        {
            System.Threading.Thread.Sleep(1000);
            if ((Helpers.Enum.CreditReaderMake)System.Enum.Parse(typeof(Helpers.Enum.CreditReaderMake), Global.CardReaderMake) == Helpers.Enum.CreditReaderMake.NayaxE2C)
            {
                commType.AuthorizeAmount(amount);
            }
            else
            {
                ICardReader reader = CardReader.CardReaderFactory.GetCreditCardReader();
                reader.Authorize(amount * 100);
            }
        }
    }
}
