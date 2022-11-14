using System;

namespace BTPTwinCatADS.STR
{
    public class PLCValue
    {
        public PLCValue()
        {
            Changed = false;
            ValueType = typeof(int);
            ValueTypeArrayItemCount = 0;
            Value = 0;
            Name = "";
        }

        public bool Changed = false;
        public Object Value;
        public Type ValueType;
        public String Name;
        public int ValueTypeArrayItemCount = 0;
    }
}
