using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class UniversalEventArgs : EventArgs
    {
        public object Data = null;
        public Type TypeOfData = null;
        public UniversalEventArgs(object data, Type typeOfData)
        {
            Data = data;
            TypeOfData = typeOfData;
        }
    }
}
