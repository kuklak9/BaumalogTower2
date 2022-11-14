using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPTwinCatADS
{
    public class CommandDictionary
    {
        public const short CMD_GET_TRAY = 1;
        public const short CMD_MOVE_TO_POSITION_POS = 12;
        public const short CMD_MOVE_TO_POSITION_MM = 10;
        public const short CMD_HOME_X = 18;
        public const short CMD_HOME_T = 25;
        public const short CMD_HOME_S = 34;
        public const short CMD_STATION_MOVE = 36;
        public const short CMD_SET_TRAY_ON_LOCATION = 37;
        public const short CMD_MOVE = 3;
        public const short CMD_SWITCH = 4;
        public const short CMD_ADD_TRAY = 500;
        public const short CMD_ADD_TRAY_FULL = 510;
        public const short CMD_DEL_TRAY = 501;
        public const short CMD_EDIT_TRAY = 502;
        public const short CMD_SV_DOOR_MODE = 39; //(* przełącz  bramę w trybie auto/manal*)
        public const short CMD_SV_DOOR = 16; //(* drzwi *)
        public const short C_HMI_CMD_BORROW_TRAY = 38; //(* Wypożyczenie półki*)
        public const short C_HMI_CMD_RETURN_TRAY = 39; //(* Zwrócenie półki*)
        public class Loader3015
        {
            public const short CMD_LoadUnload = 1;
        }
    }
}
