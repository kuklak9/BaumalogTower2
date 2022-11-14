using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPConfig.DBParameters
{
    public static class LoaderUnloader
    {
        public static bool Enable { get; set; }
        public static int Interval { get; internal set; }
        public static bool PLCDBWrite_Disable { get; set; }
        public static bool DBGetTrayForMaterial_Disable { get; set; }
        public static bool DBMaterialOnLoader_Disable { get; set; }
        public static bool UnloaderOverheight_Disable { get; set; }
        public static bool TrayOnLoaderEmpty_Disable { get; set; }
    }
}
