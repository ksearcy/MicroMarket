using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Communication
{
    class MBDFactory
    {
        public static ICommunicationType GetMDBType()
        {
            switch ((Helpers.Enum.MDBVendor)System.Enum.Parse(typeof(Helpers.Enum.MDBVendor), Helpers.Global.MDBVendor))
            {
                case Helpers.Enum.MDBVendor.E2C:
                    {
                        return deORO.MDB.E2CMDB.GetMDB();
                    }
                case Helpers.Enum.MDBVendor.Coinco:
                    {
                        return deORO.MDB.CoincoMDB.GetMDB();
                    }
                case Helpers.Enum.MDBVendor.ThinkChip:
                    {
                        return deORO.MDB.ThinkChipMDB.GetMDB();
                    }
                case Helpers.Enum.MDBVendor.None:
                    {
                        return deORO.MDB.None.GetMDB();
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
