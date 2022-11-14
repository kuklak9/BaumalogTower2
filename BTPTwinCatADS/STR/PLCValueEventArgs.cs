using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS.STR
{
    public class PLCValueEventArgs : EventArgs
    {
        public PLCValueEventArgs(PLCValue val)
        {
            Value = val;
        }

        public PLCValue Value;
    }
}
