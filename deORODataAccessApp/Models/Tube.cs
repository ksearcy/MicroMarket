using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.Models
{
    public class Tube
    {
        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        private int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        private bool full;

        public bool Full
        {
            get { return full; }
            set { full = value; }
        }
    }
}
