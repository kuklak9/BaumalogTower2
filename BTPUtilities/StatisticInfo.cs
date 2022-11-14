using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class StatisticInfo
    {
        public int Id=0;
        public String Name ="";
        public String Description="";
        public bool IsTime=false;

        public StatisticInfo()
        {
        }

        public StatisticInfo Copy()
        {
            return new StatisticInfo(Id, Name, Description, IsTime);

        }

        public StatisticInfo(int id, string name, string desc, bool istime)
        {
            Id = id;
            Name = name;
            Description = desc;
            IsTime = istime;
        }
    }
}
