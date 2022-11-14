using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS
{
    public class ADSSPrefix
    {
        static int _towerNumber = 1;
        static int _windowNumber = 1;
        public static int CfgTowerNumber { internal set; get; }
        public static string TowerPrefix_ForCfgTower { get { return "Tower_" + CfgTowerNumber + "."; } }
        public static string StationTowerPrefix_ForCfgTower { get { return "Tower_" + CfgTowerNumber + "." + "Station[" + _windowNumber + "]."; } }
        public static int TowerNumber { set { _towerNumber = value; } }
        public static int WindowNumber { set { _windowNumber = value; } }
        public static string TowerPrefix_ForControlTower { get { return "Tower_" + _towerNumber + "."; } }
        public static string StationTowerPrefix_ForControlTower { get { return "Tower_" + _towerNumber + "." + "Station[" + _windowNumber + "]."; } }
        public static string TransferPrefix { get { return "Station_Transfer."; } }
        public static string MainPrefix { get { return "MAIN."; } }
        public static string StationTowerPrefix_ForTowerAndWindow(int towerNumber, int windowNumber) { return "Tower_" + towerNumber + "." + "Station[" + windowNumber + "]."; }
        public static string TowerPrefix_ForTower(int towerNumber) { return "Tower_" + towerNumber + "."; }
        public static string Loader3015Prefix => "LoaderUnloader.";
    }
}
