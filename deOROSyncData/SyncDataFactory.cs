using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deOROSyncData
{
    public class SyncDataFactory
    {
        public static ISyncData GetSyncData()
        {
            switch (ApplicationConfig.VmsProvider.ToLower())
            {
                case "vendwatch":
                    {
                        return new SyncVendWatch();
                    }
                case "deoro" :
                    {
                        return new SyncData();
                    }
                case "vendor4me":
                    {
                        return new SyncVendor4Me();
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
