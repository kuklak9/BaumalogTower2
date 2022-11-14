using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS.STR
{
    class STR_HMI_CMD
    {
        public STR_HMI_CMD(int index, short id_cmd, short param1, short param2, short param3, short param4, short param5)
        {
            Index = index;
            ID_cmd = id_cmd;
            Param = new short[5];
            Param[0] = param1;
            Param[1] = param2;
            Param[2] = param3;
            Param[3] = param4;
            Param[4] = param5;

        }
        public int Index;
        public short ID_cmd;
        public short[] Param;
    }
}
