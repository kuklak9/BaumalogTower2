using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS
{
    public class PLCData
    {
        public bool[] Alarms = new bool[200];

        public int Weight = 0;

        public double Current_Z = 0.0;
        public double Current_X = 0.0;
        public double Current_T = 0.0;
       

        public int TowerState = 0;
        public int FakirState = 0;
        public short[] TrayConfig = new short[2000];
        public short[] TrayOnExtractorInfo = new short[3];

        public int MeasureHeight = 0;
        public int ClosestPosition = 0;
        public int ClosestPositionDistance = 0;

        public int EZ_Pos = 0;
        public int EX_Pos = 0;
        public int ET_Pos = 0;

        public int ZMoveDir = 0;
        public int XMoveDir = 0;
        public int TMoveDir = 0;

        public int TrayInExtractor = -1;
        public int TrayInWindow = -1;
        public int TrayInTable = -1;

        public bool AnyJogIsActive = false;

        public int TargetTrayInTransport = 0;
        public int ActualTrayInTransport = 0;

        public int LastAlarmNr = -1;

        public bool MoveToPosition = false;
        public bool MoveToPosition_mm = false;

        public bool JogXEn = false;
        public bool JogTEn = false;
        public bool JogZEn = false;

        /// <summary>
        /// 0 - stop
        /// 1 - prawo
        /// 2 - lewo
        /// </summary>
        public int JogX_Move = 0; 

        /// <summary>
        /// 0 - stop
        /// 1 - prawo
        /// 2 - lewo
        /// </summary>
        public int JogT_Move = 0;

        /// <summary>
        /// 0 - stop
        /// 1 - prawo
        /// 2 - lewo
        /// </summary>
        public int JogZ_Move = 0;

        public int HomeX = 0;
        public int HomeT = 0;

        public STR.STR_HMI_CMD_Status ActualCommandStatus = null;

        
        /// <summary>
        /// BI0
        /// </summary>
        public bool BS_SafeXPos = false;

        /// <summary>
        /// BI1
        /// </summary>
        public bool BS_ChainX_F = false;

        /// <summary>
        /// BI2
        /// </summary>
        public bool BS_ChainX_B = false;

        /// <summary>
        /// BI3
        /// </summary>
        public bool BS_TrayInExtractor_F = false;

        /// <summary>
        /// BI4
        /// </summary>
        public bool BS_TrayInExtractor_B = false;

        /// <summary>
        /// BI5
        /// </summary>
        public bool BS_TrayInWindow_F = false;

        /// <summary>
        /// BI6
        /// </summary>
        public bool BS_TrayInWindow_B = false;

        /// <summary>
        /// BI7
        /// </summary>
        public bool BS_TrayInTable = false;

        /// <summary>
        /// BI8
        /// </summary>
        public bool BS_ChainT = false;

        public int TrayInMove = 0;
        public int TrayInMoveState = 0;
        public int TrayInMoveTable = 0;
        public int TrayInMoveTableState = 0;
     
        //di
        public STR.STR_IO[] DI = new BTPTwinCatADS.STR.STR_IO[40];

        //do
        public STR.STR_IO[] DO = new BTPTwinCatADS.STR.STR_IO[16];

        //life word
        public int LifeWord = 0;
             



      
    }
}
