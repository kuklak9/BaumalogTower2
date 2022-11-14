using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPLogs.LogQueue
{
    class LogDescritpion
    {
        public LogDescritpion Copy()
        {
            return new LogDescritpion(Type, Description, Username, Device);
        }

        public LogDescritpion(int type, string desc, string username, string device)
        {
            Type = type;
            Description = desc;
            Username = username;
            Device = device;
        }
        public int Type = 0;
        public String Description = "";
        public String Username = "";
        public String Device = "";
    }
}
