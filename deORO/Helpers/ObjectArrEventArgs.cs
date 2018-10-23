using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deORO.Helpers
{
    public class ObjectArrEventArgs : EventArgs
    {
        public object[] data;

        public ObjectArrEventArgs(object[] eventData) : base()
        {
            this.data = eventData;
        }
    }
}
