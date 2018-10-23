using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Communication;
using deORO.MDB;

namespace deORO.CardReader
{
    public class NayaxE2C : ICardReader
    {
        ICommunicationType commType;
        private static NayaxE2C instance;

        public static NayaxE2C Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NayaxE2C();
                }
                return instance;
            }
        }

        private NayaxE2C()
        {
            commType = CommunicationTypeFactory.GetCommunicationType();
        }

        public void Open()
        {

        }

        public void Close()
        {
            commType.DisableCreditCard();
        }

        public void Reset()
        {

        }

        public void SetParams(decimal payload)
        {
            commType.EnableCreditCard();
        }

        public void DataEvent(object payload)
        {

        }

        public void ErrorEvent(object payload)
        {

        }

        public void Dispose()
        {
            commType.DisableCreditCard();
        }

        public void Authorize(object payload)
        {
            //throw new NotImplementedException();
        }



    }
}
