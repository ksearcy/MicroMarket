using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.CardReader
{
    public interface ICardReader : IDisposable
    {
        void Open();
        void Close();
        void Reset();
        void SetParams(decimal payload);
        void DataEvent(object payload);
        void ErrorEvent(object payload);
        void Authorize(object payload);
        
   }
}
