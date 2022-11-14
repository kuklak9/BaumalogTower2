using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class TrayConfigEventArgs : EventArgs
    {
        public Dictionary<int,Tray> Trays = null;
        public Dictionary<int, TrayPosition> TrayPositions = null;

        public TrayConfigEventArgs(Dictionary<int, Tray> tray, Dictionary<int, TrayPosition> trayposition)
        {
            Trays = new Dictionary<int, Tray>(tray);
            TrayPositions = new Dictionary<int, TrayPosition>(trayposition);
        }

    }
}
