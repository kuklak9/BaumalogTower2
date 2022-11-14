using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS.STR
{
    public class STR_Material
    {
        public String Attribute1;//: STRING(50); (*cecha 1 *)
        public String Attribute2;//: STRING(50); (*cecha 2 *)
        public String Index;//: STRING(50); (*indeks *)
        public String Job;//: STRING(50);
        public int X;//: DINT; (*wymiar x 0,1mm *)
        public int Y;//: DINT; (*wymiar y 0,1mm *)
        public int Z;//: DINT; (*wymiar z 0,1mm *)
        public short JobNr;//: INT;
        public short IndexNr;//: INT;
        public short UnloadingLoacation;//: INT;
        public short HasPaper;//: INT;
        public short Qty;//: INT;

        public STR_Material()
        {

        }

        public bool IsIndexEmpty()
        {
            return Index == "";
        }
        public bool IsIndexCut(string cutPrefix)
        {
            return Index.Contains(cutPrefix);
        }

        public bool MaterialNeedCutUpdate(object[] programObjData, string cutPrefix)
        {
            return !IsIndexCut(cutPrefix) && CompareMaterialWithProgramObjData(programObjData);
        }

        public bool CompareMaterialWithProgramObjData(object[] programObjData)
        {
            return !IsIndexEmpty() && (Job == programObjData[5].ToString() || Job == "") // patrze na program tylko kiedy jest uzupełniony
                && X >= Convert.ToInt32((int)programObjData[1])
                && Y >= Convert.ToInt32((int)programObjData[2])
                && Z == Convert.ToInt32((int)programObjData[3])
                && Attribute1 == programObjData[0].ToString();
        }
    }
}
