using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class TrayPositionEventArgs : EventArgs
    {
        public Dictionary<int,TrayPosition> Table = null;

        public TrayPositionEventArgs(Dictionary<int, TrayPosition> t)
        {
            Table = new Dictionary<int, TrayPosition>(t);
       
        }

    }
}
