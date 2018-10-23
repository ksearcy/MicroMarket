using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Helpers;

namespace deORO.CardReader
{
    public class CardReaderFactory
    {
        public static ICardReader GetCreditCardReader()
        {
            switch ((Helpers.Enum.CreditReaderMake)System.Enum.Parse(typeof(Helpers.Enum.CreditReaderMake), Global.CardReaderMake))
            {
                //case Helpers.Enum.CreditReaderMake.IDTECH:
                //    {
                //        return IDTECH.Instance;
                //    }
                case Helpers.Enum.CreditReaderMake.OTI:
                    {
                        return OTI.Instance;
                    }
                case Helpers.Enum.CreditReaderMake.MagTek:
                    {
                        return MagTek.Instance;
                        //return POSCardReader.Instance;
                    }
                case Helpers.Enum.CreditReaderMake.POS:
                    {
                        return POSCardReader.Instance;
                    }
                case Helpers.Enum.CreditReaderMake.NayaxE2C:
                    {
                        return NayaxE2C.Instance;
                    }
                case Helpers.Enum.CreditReaderMake.NayaxMarshall:
                    {
                        return NayaxMarshall.Instance;
                    }
                case Helpers.Enum.CreditReaderMake.CardKnox:
                    {
                        return CardKnox.Instance;
                    }
                case Helpers.Enum.CreditReaderMake.IDTECH:
                    {
                        return IDTECH.Instance;
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
