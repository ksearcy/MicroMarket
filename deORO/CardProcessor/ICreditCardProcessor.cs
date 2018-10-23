using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.CardProcessor
{
    public interface ICreditCardProcessor
    {
        string userName { get; set; }
        string password { get; set; }
        string message { get; set; }
                
        //Task ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails = "", string zipCode = "");
        void ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails = "", string zipCode = "");
        void ChangePassword(string serialNumber, int updateStatus);
    
    }
}
