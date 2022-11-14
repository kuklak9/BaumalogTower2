using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class DoubleTableEventArgs : EventArgs
    {
        public double[] Table = null;

        public DoubleTableEventArgs(double[] t)
        {
            Table = t;
        }
    }
}
