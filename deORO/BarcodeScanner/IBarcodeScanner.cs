using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.BarcodeScanner
{
    interface IBarcodeScanner : IDisposable
    {
        void Open(string subscriber = "");
        void Close();
        void ErrorEvent(object payload);
        void DataEvent(object payload);
        void StatusUpdateEvent(object payload);
    }
}
