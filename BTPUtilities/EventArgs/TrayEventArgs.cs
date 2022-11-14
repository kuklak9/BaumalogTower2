using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class TrayEventArgs : EventArgs
    {
        public Dictionary<int,Tray> Table = null;

        public TrayEventArgs(Dictionary<int,Tray> t)
        {
            Table = new Dictionary<int, Tray>(t);
       
        }

    }
}
