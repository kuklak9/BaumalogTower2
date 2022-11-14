using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class IntTableEventArgs : EventArgs
    {
        public int[] Table = null;

        public IntTableEventArgs(int[] t)
        {
            Table = t;
        }
    }
}
