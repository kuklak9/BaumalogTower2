using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class TrayToMove
    {
        public static bool operator ==(TrayToMove a, TrayToMove b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.NewPosition == b.NewPosition && a.NewPositionColumn == b.NewPositionColumn && a.Tray == b.Tray && a.NewPositionTower == b.NewPositionTower;
        }

        public static bool operator !=(TrayToMove a, TrayToMove b)
        {
            return !(a == b);
        }

        public Tray Tray = null;
        public int NewPosition = 0;
        public int NewPositionColumn = 0;
        public int NewPositionTower = 0;

    }
}
