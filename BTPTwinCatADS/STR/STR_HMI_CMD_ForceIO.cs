using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS.STR
{
    class STR_HMI_CMD_ForceIO
    {
        public STR_HMI_CMD_ForceIO(short id, short isinput, short value)
        {
            ID = id;
            IsInput = isinput;
            Value = value;
        }

        public short ID;

        /// <summary>
        /// >0 jest wejściem
        /// </summary>
        public short IsInput;

        /// <summary>
        /// 1 ON, 0 Force OFF, -1 OFF
        /// </summary>
        public short Value; 

    }
}
