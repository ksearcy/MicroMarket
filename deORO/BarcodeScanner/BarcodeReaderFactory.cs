using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.BarcodeScanner
{
    class BarcodeScannerFactory
    {
        public static IBarcodeScanner GetBarcodeScanner()
        {
            switch ((Helpers.Enum.BarcodeReaderMake)System.Enum.Parse(typeof(Helpers.Enum.BarcodeReaderMake), Helpers.Global.BarcodeReaderDeviceMake))
            {
                case Helpers.Enum.BarcodeReaderMake.Honeywell:
                case Helpers.Enum.BarcodeReaderMake.Code:
                    {
                        return POSBarcodeScanner.Instance;
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
