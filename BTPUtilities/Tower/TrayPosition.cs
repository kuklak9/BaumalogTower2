using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class TrayPosition : IComparable<TrayPosition>
    {
        public TrayPosition()
        {

        }

        int IComparable<TrayPosition>.CompareTo(TrayPosition other)
        {

            if (other.Tower > this.Tower)
                return -1;
            else if (other.Tower < this.Tower)
                return 1;
            else
            {
                if (other.Column > this.Column)
                    return -1;
                else if (other.Column < this.Column)
                    return 1;
                else
                {
                    if (other.Position > this.Position)
                        return -1;
                    else if (other.Position == this.Position)
                        return 0;
                    else
                        return 1;
                }
            }
        }

        public TrayPosition Copy()
        {
            return new TrayPosition(Tower, Position, Column, PositionType, Tray);
        }

        //x, y, z, 1, id

        public TrayPosition(int tower, int position, int column, int positiontype, int tray)
        {
            Position = position;
            Column = column;
            PositionType = positiontype;
            Tray = tray;
            Tower = tower;
        }

        public TrayPosition(int position, int column, int positiontype, int tray)
        {
            Position = position;
            Column = column;
            PositionType = positiontype;
            Tray = tray;
            Tower = 1;
        }

        public int Tray = 0;
        public int Position = 0;
        public int Column = 0;
        public int Tower = 0;
        public int PositionType = PT_NONE;

        public const int PT_OK = 1;
        public const int PT_LOCKED = 0;
        public const int PT_ART_ONLY = 2;
        public const int PT_NONE = -1;

    }
}
