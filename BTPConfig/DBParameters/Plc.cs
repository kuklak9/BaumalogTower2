using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPConfig.DBParameters
{
    public static class Plc
    {
        public static string AmsNetId { get; internal set; }
        public static int Enable { get; internal set; }
        public static int Port { get; internal set; }
    }
}
