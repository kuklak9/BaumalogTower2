using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS.STR
{
    public class TransferInfo
    {
        public TransferInfo()
        {
            _towersLocalTransferInfo = new Dictionary<int, TowerTransferInfo>();
        }

        public int GlobalWindowNr { get; set; } = 0;

        public Dictionary<int, TowerTransferInfo> _towersLocalTransferInfo;

        public TowerTransferInfo this[int index]
        {
            get
            {
                if (_towersLocalTransferInfo.ContainsKey(index))
                    return _towersLocalTransferInfo[index];
                else
                    return new TowerTransferInfo();
            }
            set
            {
                if (!_towersLocalTransferInfo.ContainsKey(index))
                    _towersLocalTransferInfo.Add(index, value);
                else
                    _towersLocalTransferInfo[index] = value;
            }
        }
    }
}
