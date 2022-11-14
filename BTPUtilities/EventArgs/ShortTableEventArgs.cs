using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class ShortTableEventArgs : EventArgs
    {
        public short[] Table = null;

        public ShortTableEventArgs(short[] t)
        {
            Table = t;
        }
    }
}
