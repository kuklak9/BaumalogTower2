using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class StringEventArgs : EventArgs
    {
        public StringEventArgs()
        {
        }

        public StringEventArgs(String[] str)
        {
            Strings = str;
        }

        public String[] Strings = null;
    }
}
