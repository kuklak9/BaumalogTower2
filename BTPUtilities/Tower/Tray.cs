using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class Tray : IComparable<Tray>
    {
        public Tray()
        {

        }

        public static bool operator ==(Tray a, Tray b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return (a.Height == b.Height && a.ID == b.ID && a.Position == b.Position && a.Weight == b.Weight && a.Column == b.Column && a.Tower == b.Tower);
        }

        public static bool operator !=(Tray a, Tray b)
        {
            return !(a == b);
        }

        int IComparable<Tray>.CompareTo(Tray other)
        {
            if (other.ID > this.ID)
                return -1;
            else if (other.ID == this.ID)
                return 0;
            else
                return 1;
        }

        public Tray Copy()
        {
            return new Tray(ID, Tower,  Column, Position, Height, Weight);
        }

        public Tray(int id, int column, int position, int height, int weight)
        {
            ID = id;
            Column = column;
            Position = position;
            Height = height;
            Weight = weight;
            Tower = 1;
        }

        //i, z, y, x, 1, 3000

        public Tray(int id, int tower, int column, int position, int height, int weight)
        {
            ID = id;
            Column = column;
            Position = position;
            Height = height;
            Weight = weight;
            Tower = tower;
        }

        public int ID = 0;
        public int Column = 0;
        public int Position = 0;
        public int Height = 0;
        public int Weight = 0;
        public bool LockedOnPosition = false;
        public int Tower = 0;

    }
}
