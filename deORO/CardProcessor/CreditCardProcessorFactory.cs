using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.CardProcessor;
using deORO.Helpers;

namespace deORO.CreditCardProcessorFactory
{
    public class CreditCardProcessorFactory
    {
        public static ICreditCardProcessor GetCreditCardProcessor()
        {
            try
            {
                if (Helpers.Global.DemoMode)
                {
                    return new Demo();
                }
            }
            catch { }

            switch ((Helpers.Enum.CreditCardProcessor)System.Enum.Parse(typeof(Helpers.Enum.CreditCardProcessor), Global.CardProcessorCompanyName))
            {
                case Helpers.Enum.CreditCardProcessor.Heartland:
                    {
                        return new Heartland();
                    }
                case Helpers.Enum.CreditCardProcessor.USAT:
                    {
                        return new USAT();
                    }
                case Helpers.Enum.CreditCardProcessor.Nayax:
                    {
                        return new Nayax();
                    }
                case Helpers.Enum.CreditCardProcessor.CardKnox:
                    {
                        return new CardKnox();
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
