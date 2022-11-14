using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class TableColumnCfg
    {
        public TableColumnCfg()
        {
        }

        public TableColumnCfg(String dbname, String name, String typename)
        {
            DBname = dbname;
            Name = name;
            TypeName = typename;
        }

        public String DBname = "";
        public String Name = "";
        public String TypeName = "";

    }
}
