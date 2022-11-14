using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class BoolTableEventArgs : EventArgs
    {
        public bool[] Table = null;

        public BoolTableEventArgs(bool[] t)
        {
            Table = t;
        }

    }
}
