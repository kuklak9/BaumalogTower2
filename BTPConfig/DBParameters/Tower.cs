using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPConfig.DBParameters
{
    public static class Tower
    {
        public static bool AllowBorrowTrays { get; internal set; }
        public static bool AllowEditHeight { get; internal set; }
        public static bool AllowEditTrays { get; internal set; }
        public static int Columns { get; internal set; }
        public static bool HasWeight { get; internal set; }
        public static bool HideItemTypes { get; internal set; }
        public static bool MeasureHeight { get; internal set; }
        public static int MinHeight { get; internal set; }
        public static bool OnlyOrders { get; internal set; }
        public static bool OSKKeyboard { get; internal set; }
        public static bool ReadTrayConfig { get; internal set; }
        public static bool ResetOnTheRight { get; internal set; }
        public static bool ServiceIntervalDisplay { get; internal set; }
        public static int Station { get; internal set; }
        public static int StationLocation { get; internal set; }
        public static bool StationManual { get; internal set; }
        public static int Table { get; internal set; }
        public static bool TableOnTheLeft { get; internal set; }
        public static int TablePosition { get; internal set; }
        public static int TrayWeight { get; internal set; }
        public static int Windows { get; internal set; }
        public static int TowerNumber { get; internal set; }
        public static bool HasTransfer { get; internal set; }
        public static bool GlobalTransport { get; internal set; }
        public static int WindowNr { get; internal set; }
        public static bool ShowStationAlarmPopup { get; internal set; }
        public static bool TrayStates { get; internal set; }
        public static bool HasGate { get; internal set; }
        public static int ControlTowersCount { get; internal set; }
        public static bool LoaderUnloader { get; internal set; }
    }
}
