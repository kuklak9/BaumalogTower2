using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class IntEventArgs : EventArgs
    {
        public IntEventArgs()
        {
            number = -1;
        }

        public IntEventArgs(int nr)
        {
            number = nr;
        }
        public int number;
    }
}
