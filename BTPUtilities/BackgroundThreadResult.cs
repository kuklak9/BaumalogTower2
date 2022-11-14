using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class BackgroundThreadResult
    {
        public BackgroundThreadResult(string cmd, Object data)
        {
            Command = cmd;
            Data = data;
        }
        public String Command = "";
        public Object Data = null;

    }
}
