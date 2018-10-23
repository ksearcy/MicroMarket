using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Communication
{
    public class CommunicationTypeFactory
    {
        public static ICommunicationType GetCommunicationType()
        {
            switch ((Helpers.Enum.CommunicationType)System.Enum.Parse(typeof(Helpers.Enum.CommunicationType), Helpers.Global.CommunicationType))
            {
                case Helpers.Enum.CommunicationType.MDB:
                    {
                        return deORO.Communication.MBDFactory.GetMDBType();
                    }
                case Helpers.Enum.CommunicationType.MEI:
                    {
                        return deORO.MEI.MEI.GetMEI();
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
