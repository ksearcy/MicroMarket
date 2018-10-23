using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.CardProcessor
{
    public class CreditCardData
    {
        public byte[] Track2EncryptedData {get;set;}
        public byte[] AdditionalSecurityInformation { get; set; }
        public int EncryptedDataLength { get; set; }
        public object payload { get; set; }
        public string CardType { get; set; }
    }
}
