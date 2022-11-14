using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwinCAT.Ads;
using System.IO;
using BTPTwinCatADS.STR;
using System.Windows.Controls;

namespace BTPTwinCatADS
{
    public class PLC
    {
        private int _lastCommandIndex = -1;

        private BTPConfig.Configuration _cfg = null;
        private BTPLogs.Logs _log = null;

        private TcAdsClient _adsClient = null;

        private String _addr;
        private int _port;
        private int _towerNr = 1;
        private int _windowNr = 1;

        public int WindowNr
        {
            get
            {
                return _windowNr;
            }
        }

        private Dictionary<int, STR.PLCValue> _hConnect = new Dictionary<int, STR.PLCValue>();
        private BinaryReader _binRead;
        private AdsStream _dataStream;
        private BTPLogs.EventErrorLogger _eventErrorLogger;

        private bool _readTowerConfig = false;


        private TextBox _tbPom = new TextBox();

        public PLC(BTPConfig.Configuration cfg, int towerNr, int windowNr, BTPLogs.Logs log, BTPLogs.EventErrorLogger eventErrorLogger)
        {
            _cfg = cfg;
            _addr = _cfg.GetParamStr("PLC_AmsNetId");//_cfg.PLCAddr;
            _port = _cfg.GetParamInt("PLC_Port");//_cfg.PLCPort;
            _towerNr = towerNr;
            ADSSPrefix.CfgTowerNumber = towerNr;
            _windowNr = windowNr;
            _eventErrorLogger = eventErrorLogger;

            _log = log;

            PlcValueObservable.InitializeStaticInstance(this, eventErrorLogger, _cfg);
        }

        public String Init()
        {
            if (!_cfg.GetParamBool("PLC_Enable"))//(!_cfg.PLCEnable)
                return "WYŁĄCZONO";
            String rv = Connect();

            if (rv == "OK")
            {
                if (GetLifeWord())
                {
                    GetConfigurationFromPLC();

                    CreateNotificationEventHandlers_AUTO();
                    return rv;
                }
                else
                    return "BRAK POŁĄCZENIA";
            }
            else
                return rv;
        }

        private void GetConfigurationFromPLC()
        {
            try
            {
                GetTransferCfgForTowers();

                if (!_cfg.GetParamBool("TOWER_GlobalTransport"))
                    return;

                GetGlobalCfgDictionary();
                GetGlobalCfgTransferWindowNr();
            }
            catch (Exception ex)
            {
                _eventErrorLogger.AddLog(BTPLogs.ErrorType.PlcOnNotification, new string[]
                {
                    "PLC",
                    ex.Source,
                    ex.Data.GetType().ToString(),
                    ex.Message
                });
            }
        }

        private void GetTransferCfgForTowers()
        {
            for (int i = 1; i <= _cfg.GetParamInt("TOWER_ControlTowersCount"); i++)
            {
                short transferLocalWindowNr = READ_Short(ADSSPrefix.TowerPrefix_ForTower(i) + "Tower.TransferWindowNr");

                if (transferLocalWindowNr == 0)
                    _transferInfo[i] = new STR.TowerTransferInfo();
                else
                {
                    short transferColumnNr = READ_Short(ADSSPrefix.TowerPrefix_ForTower(i) + "Tower.Window[" + transferLocalWindowNr + "].Column");
                    short transferPositionNr = READ_Short(ADSSPrefix.TowerPrefix_ForTower(i) + "HMI_TransferPosition");

                    _transferInfo[i] = new STR.TowerTransferInfo()
                    {
                        WindowNr = transferLocalWindowNr,
                        ColumnNr = transferColumnNr,
                        PositionNr = transferPositionNr
                    };
                }
            }
        }

        private void GetGlobalCfgDictionary()
        {
            _towersGlobalWindowsConfig.Clear();
            for (int i = 1; i <= _cfg.GetParamInt("TOWER_ControlTowersCount"); i++)
            {
                _towersGlobalWindowsConfig.Add(i, READ_ShortTable(ADSSPrefix.MainPrefix + "Tower" + i + "_ExternalSignals.TransferInfo.GlobalWindowNr", 6));
            }
        }

        private void GetGlobalCfgTransferWindowNr()
        {
            _transferInfo.GlobalWindowNr = READ_Short(ADSSPrefix.MainPrefix + "TransferWindowNr");
        }

        private Dictionary<int, short[]> _towersGlobalWindowsConfig { get; set; } = new Dictionary<int, short[]>();
        private STR.TransferInfo _transferInfo = new STR.TransferInfo();

        public STR.TransferInfo TransferInfo
        {
            get => _transferInfo;
        }

        public int TransferGlobalWindowNr => _transferInfo.GlobalWindowNr;

        public int GetLocalWindowNr(int towerNr, int globalWindowNr)
        {
            var towerList = _towersGlobalWindowsConfig[towerNr];

            for (int i = 0; i < towerList.Length; i++)
            {
                if (towerList[i] == globalWindowNr)
                    return i;
            }

            return -1;
        }

        public short GetGlobalWindowNr(int towerNr, short localWindowNr)
        {
            if (_towersGlobalWindowsConfig.ContainsKey(towerNr))
                return _towersGlobalWindowsConfig[towerNr][localWindowNr];

            return localWindowNr;
        }


        public int GetGlobalWindowConfig()
        {
            int rv = 0;
            if (_towersGlobalWindowsConfig != null
                && _towersGlobalWindowsConfig.ContainsKey(_cfg.TowerNrCfg)
                && _cfg.WindowNr >= 1
                && _cfg.WindowNr <= 5)
            {
                rv = _towersGlobalWindowsConfig[_cfg.TowerNrCfg][_cfg.WindowNr];
            }

            if (rv == 0)
                rv = _cfg.WindowNr;

            return rv;
        }


        public int GetGlobalWindowConfig(int windowNr)
        {
            if (_towersGlobalWindowsConfig.ContainsKey(_cfg.TowerNrCfg))
                return _towersGlobalWindowsConfig[_cfg.TowerNrCfg][windowNr];

            return _cfg.WindowNr;
        }


        public String Close()
        {
            //
            try
            {
                //_adsClient.Dispose();
            }
            catch { }

            //CloseNotificationEventHandlers();

            return "OK";
        }

        private string Connect()
        {

            string ret = "OK";
            try
            {
                AmsNetId amsid = new AmsNetId(_addr);
                _adsClient = new TcAdsClient();
                try
                {
                    _adsClient.Connect(amsid, _port);

                    //Dispose przy braku połączenia z PLC zamula APP, a brak Dispose() psuje mapowania
                    if (_adsClient.IsConnected && !_adsClient.Disposed)
                    {
                        _adsClient.Dispose();
                        _adsClient.Connect(amsid, _port);
                    }

                    if (_adsClient.IsConnected)
                    {
                        ret = "OK";
                    }
                    else
                        ret = "Not connected";

                }
                catch (Exception err)
                {
                    ret = (err.Message);
                }
            }
            catch (Exception err)
            {
                ret = err.Message;
            }
            return ret;

        }



        private string CloseNotificationEventHandlers()
        {
            string ret = "OK";
            try
            {
                foreach (int key in _hConnect.Keys)
                    _adsClient.DeleteDeviceNotification(key);
            }
            catch (Exception err)
            {
                ret = (err.Message);
            }

            return ret;
        }

        private bool CreateNotificationEventHandler(String name, int addr, int size, int time, bool cyclic, Type type, int arrayItemsCount)
        {
            try
            {
                int pom = _adsClient.AddDeviceNotification(
                    name,
                    _dataStream,
                    addr,
                    size,
                    (!cyclic ? AdsTransMode.OnChange : AdsTransMode.Cyclic),
                    time,
                    0,
                    _tbPom);

                STR.PLCValue plcv = new STR.PLCValue();
                plcv.Name = name;
                plcv.ValueType = type;
                plcv.ValueTypeArrayItemCount = arrayItemsCount;
                _hConnect.Add(pom, plcv);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool GetLifeWord()
        {
            bool rv = false;
            try
            {

                int iValue = 0;
                int hArr = _adsClient.CreateVariableHandle(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_LifeWord");
                AdsStream dataStream = new AdsStream(2);
                AdsBinaryReader binReader = new AdsBinaryReader(dataStream);
                _adsClient.Read(hArr, dataStream);
                iValue = binReader.ReadInt16();
                dataStream.Position = 0;

                //   if (iValue != 0)
                rv = true;

            }
            catch
            {
            }

            return rv;

        }

        public event EventHandler<BTPUtilities.StringEventArgs> CmdError;

        private void FireCmdError(BTPUtilities.StringEventArgs str)
        {
            if (CmdError != null)
            {
                CmdError(this, str);
            }
        }


        public event EventHandler<STR.PLCValueEventArgs> ValueChanged;

        private void FireValueChanged(STR.PLCValueEventArgs ea)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, ea);
            }

        }




        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {

            DateTime time = DateTime.FromFileTime(e.TimeStamp);
            e.DataStream.Position = e.Offset;
            STR.PLCValue plcV = new BTPTwinCatADS.STR.PLCValue();

            try
            {
                if (_hConnect.ContainsKey(e.NotificationHandle))
                {
                    plcV = _hConnect[e.NotificationHandle];
                    int count = plcV.ValueTypeArrayItemCount;

                    if (count == 0)
                        count = 1;

                    if (plcV.ValueType == typeof(bool))
                    {
                        byte[] data = _binRead.ReadBytes(count);

                        bool[] values = new bool[count];

                        for (int i = 0; i < count; i++)
                            values[i] = data[i] == 1;

                        if (plcV.ValueTypeArrayItemCount == 0)
                            plcV.Value = values[0];
                        else
                            plcV.Value = values;
                    }
                    else if (plcV.ValueType == typeof(byte) || plcV.ValueType == typeof(sbyte))
                    {
                        int bytesLenght = 1;

                        if (plcV.ValueType == typeof(byte))
                        {
                            byte[] values = PLCUtilities_Generic.ReadPLCValue(new byte(), _binRead, count, bytesLenght);

                            if (plcV.ValueTypeArrayItemCount == 0)
                                plcV.Value = values[0];
                            else
                                plcV.Value = values;
                        }
                        else
                        {
                            sbyte[] values = PLCUtilities_Generic.ReadPLCValue(new sbyte(), _binRead, count, bytesLenght);

                            if (plcV.ValueTypeArrayItemCount == 0)
                                plcV.Value = values[0];
                            else
                                plcV.Value = values;
                        }
                    }
                    else if (plcV.ValueType == typeof(short) || plcV.ValueType == typeof(ushort))
                    {
                        int bytesLenght = 2;

                        if (plcV.ValueType == typeof(short))
                        {
                            short[] values = PLCUtilities_Generic.ReadPLCValue(new short(), _binRead, count, bytesLenght);

                            if (plcV.ValueTypeArrayItemCount == 0)
                                plcV.Value = values[0];
                            else
                                plcV.Value = values;
                        }
                        else
                        {
                            ushort[] values = PLCUtilities_Generic.ReadPLCValue(new ushort(), _binRead, count, bytesLenght);

                            if (plcV.ValueTypeArrayItemCount == 0)
                                plcV.Value = values[0];
                            else
                                plcV.Value = values;
                        }
                    }
                    else if (plcV.ValueType == typeof(int) || plcV.ValueType == typeof(uint))
                    {
                        int bytesLenght = 4;

                        if (plcV.ValueType == typeof(int))
                        {
                            int[] values = PLCUtilities_Generic.ReadPLCValue(new int(), _binRead, count, bytesLenght);

                            if (plcV.ValueTypeArrayItemCount == 0)
                                plcV.Value = values[0];
                            else
                                plcV.Value = values;
                        }
                        else
                        {
                            uint[] values = PLCUtilities_Generic.ReadPLCValue(new uint(), _binRead, count, bytesLenght);

                            if (plcV.ValueTypeArrayItemCount == 0)
                                plcV.Value = values[0];
                            else
                                plcV.Value = values;
                        }
                    }
                    else if (plcV.ValueType == typeof(AdsBigType)) //Jakaś struktura albo coś innego
                    {
                        //Zwracam tablicę obiektów na podstawie struktury stworzonej przy mapowaniu zmiennych 

                        //Pobieram jaka jest struktura zmiennej
                        var variableStructure = _structureDataForMappedBigTypes[plcV.Name];

                        //Pobieram wszystkie bajty danych
                        int size = variableStructure.Size.Sum();

                        if (plcV.ValueTypeArrayItemCount != 0)
                            size = size * count;

                        //Odczytuje wszystkie bajty
                        byte[] data = _binRead.ReadBytes(size);

                        int dataOffset = 0;

                        List<object> temp = new List<object>();
                        for (int i = 0; i < count; i++)//po każdym z listy obiektów
                        {
                            temp.Add(GetDataFromBigVariable(
                                variableStructure.Values,
                                variableStructure.Size,
                                variableStructure.Count,
                                ref dataOffset,
                                ref data));
                        }

                        if (plcV.ValueTypeArrayItemCount > 0)
                            plcV.Value = temp.ToArray();
                        else
                            plcV.Value = temp.First();
                    }

                    FireValueChanged(new BTPTwinCatADS.STR.PLCValueEventArgs(plcV));
                }
            }
            catch (Exception ex)
            {
                _eventErrorLogger.AddLog(BTPLogs.ErrorType.PlcOnNotification, new string[]
                    {
                        "PLC",
                        plcV.Name,
                        plcV.Value.ToString(),
                        ex.Message
                    });
            }
        }



        #region WRITE


        public bool WRITE_Correction(int value, int type, int p1, int p2)
        {
            try
            {

                String name = "";
                switch (type)
                {
                    case 0:
                        name = ADSSPrefix.TowerPrefix_ForControlTower + "MEMO_Tower_Corrections.ZeroPointCorrection";
                        break;
                    case 1:
                        name = ADSSPrefix.TowerPrefix_ForControlTower + "MEMO_Tower_Corrections.WindowEncCorrection[" + p1 + "]";//_adss.WindowEncCorrection(p1);
                        break;
                    case 2:
                        name = ADSSPrefix.TowerPrefix_ForControlTower + "MEMO_Tower_Corrections.PostionEncCorrection[" + p1 + "," + p2 + "]";//_adss.PositionEncCorrection(p1, p2);
                        break;
                    default:
                        return false;
                }

                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(name), Convert.ToInt16(value));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_GetTray(int trayID)
        {
            try
            {
                return WRITE_Command(CommandDictionary.CMD_GET_TRAY, Convert.ToInt16(trayID), Convert.ToInt16(_windowNr), 0, 0, Convert.ToInt16(_windowNr));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_MoveToPosition(int pos, bool mm)
        {
            try
            {
                short cmd = Convert.ToInt16(mm ? CommandDictionary.CMD_MOVE_TO_POSITION_MM : CommandDictionary.CMD_MOVE_TO_POSITION_POS);
                return WRITE_Command(cmd, Convert.ToInt16(pos), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr));
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_MoveToPosition_with_speed(int pos, bool mm, int speed, int column)
        {
            try
            {
                short cmd = Convert.ToInt16(mm ? CommandDictionary.CMD_MOVE_TO_POSITION_MM : CommandDictionary.CMD_MOVE_TO_POSITION_POS);
                if (speed > 100)
                    speed = 100;
                if (speed < 10)
                    speed = 10;
                return WRITE_Command(cmd, Convert.ToInt16(pos), Convert.ToInt16(speed), Convert.ToInt16(column), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr));
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_MoveToPosition_with_speed(int pos, bool mm, int speed)
        {
            try
            {
                short cmd = Convert.ToInt16(mm ? CommandDictionary.CMD_MOVE_TO_POSITION_MM : CommandDictionary.CMD_MOVE_TO_POSITION_POS);
                if (speed > 100)
                    speed = 100;
                if (speed < 10)
                    speed = 10;
                return WRITE_Command(cmd, Convert.ToInt16(pos), Convert.ToInt16(speed), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr));
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_SetJogEn(char axis)
        {
            try
            {
                bool rv = true;
                int nr = 0;
                switch (axis)
                {
                    case 'x':
                        nr = 22;
                        break;
                    case 'z':
                        nr = 23;
                        break;
                    case 't':
                        nr = 24;
                        break;
                    case 's':
                        nr = 33;
                        break;
                    default:
                        return false;
                }

                short[] param = new short[5];
                short cmd = Convert.ToInt16(nr);

                for (int i = 0; i < 5; i++)
                {
                    param[i] = Convert.ToInt16(_windowNr);
                }

                return WRITE_Command(cmd, param[0], param[1], param[2], param[3], param[4]);
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_Stop()
        {
            if (!_cfg.GetParamBool("PLC_Enable"))
            {
                //STR.PLCValue plcValue = new STR.PLCValue();
                //plcValue.Name = "Tryb_Offline_PLC_Stop_Transport";

                //ValueChanged(null, new BTPTwinCatADS.STR.PLCValueEventArgs(plcValue));

                PlcValueObservable.Instance.ForceFireValueChanged("Tryb_Offline_PLC_Stop_Transport", null);
                return true;
            }

            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_CMD_Stop"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_ShortArray(String str, short[] table)
        {
            try
            {
                int hArr;

                hArr = _adsClient.CreateVariableHandle(str);
                _adsClient.WriteAny(hArr, table);
                _adsClient.DeleteVariableHandle(hArr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_Int(String str, int value)
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(str), value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_Short(String str, short value)
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(str), value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_Bit(String str, bool v)
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(str), v ? "True" : "False");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_Bit(String str)
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(str), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_Ce()
        {
            if (!_cfg.GetParamBool("PLC_Enable"))
            {
                ////Zmiana polki w
                //STR.PLCValue plcValue = new STR.PLCValue();
                //plcValue.Name = "Tryb_Offline_PLC_CE_Transport";

                //ValueChanged(null, new BTPTwinCatADS.STR.PLCValueEventArgs(plcValue));
                PlcValueObservable.Instance.ForceFireValueChanged("Tryb_Offline_PLC_CE_Transport", null);

                return true;
            }
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_CMD_Ce"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_Re()
        {
            if (!_cfg.GetParamBool("PLC_Enable"))
            {
                ////Zmiana polki w
                //STR.PLCValue plcValue = new STR.PLCValue();
                //plcValue.Name = "Tryb_Offline_PLC_RE_Transport";

                //ValueChanged(null, new BTPTwinCatADS.STR.PLCValueEventArgs(plcValue));

                PlcValueObservable.Instance.ForceFireValueChanged("Tryb_Offline_PLC_RE_Transport", null);


                return true;
            }

            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_CMD_Re"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_ResetCommunication()
        {
            try
            {


                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_CMD_ReseCommunication"), "True");

                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool WRITE_Reset()
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_CMD_ResetAlarms"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }





        public bool WRITE_ResetSafety()
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_CMD_ResetSafety"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_ResetForce()
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_CMD_ResetForce"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_ResetJog()
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_ResetJog"), "True");

                if(_cfg.GetParamBool("HasTransfer"))
                    _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TransferPrefix + "HMI_CMD_StopJog"), "True");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_ResetTrayInTransport()
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_ResetTrayInTransport"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_StopJog()
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_StopJog"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_SetJogMove(char axis, bool direction)
        {
            String symbol_name = "";

            switch (axis)
            {
                case 'x':
                    symbol_name = direction ? ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_JogX_Left" : ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_JogX_Right";
                    break;
                case 'z':
                    symbol_name = direction ? ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_JogZ_Up" : ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_JogZ_Down";
                    break;
                case 't':
                    symbol_name = direction ? ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_JogT_Left" : ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_JogT_Right";
                    break;
                case 's':
                    symbol_name = direction ? ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_JogS_Right" : ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_JogS_Left";
                    break;

                default:
                    return false;
            }

            try
            {

                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(symbol_name), true);
                return true;
            }
            catch
            {
                return false;
            }
        }



        public bool WRITE_Home(char axis, bool direction)
        {
            short function = 0;
            switch (axis)
            {
                case 'x':
                    function = CommandDictionary.CMD_HOME_X;
                    break;
                case 't':
                    function = CommandDictionary.CMD_HOME_T;
                    break;
                case 's':
                    function = CommandDictionary.CMD_HOME_S;
                    break;


                default:
                    return false;
            }

            try
            {
                WRITE_Command(function, (short)(direction ? -1 : 0), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_Material(string variable, STR_Material material)
        {
            try
            {
                AdsStream dataStream = new AdsStream(226);
                BinaryWriter binWrite = new BinaryWriter(dataStream);
                int hcomplexStruct = _adsClient.CreateVariableHandle(variable);

                dataStream.Position = 0;
                char[] chAttribute1 = new char[51];
                char[] chAttribute2 = new char[51];
                char[] chIndex = new char[51];
                char[] chJob = new char[51];

                if (material.Attribute1 == null)
                    material.Attribute1 = "";


                if (material.Attribute2 == null)
                    material.Attribute2 = "";

                if (material.Index == null)
                    material.Index = "";

                if (material.Job == null)
                    material.Job = "";

                for (int i = 0; i < 51; i++)
                {
                    if (i < material.Attribute1.Length)
                        chAttribute1[i] = material.Attribute1[i];
                    else
                        chAttribute1[i] = '\0';

                    if (i < material.Attribute2.Length)
                        chAttribute2[i] = material.Attribute2[i];
                    else
                        chAttribute2[i] = '\0';

                    if (i < material.Index.Length)
                        chIndex[i] = material.Index[i];
                    else
                        chIndex[i] = '\0';

                    if (i < material.Job.Length)
                        chJob[i] = material.Job[i];
                    else
                        chJob[i] = '\0';
                }

                binWrite.Write(chAttribute1);
                binWrite.Write(chAttribute2);
                binWrite.Write(chIndex);
                binWrite.Write(chJob);
                binWrite.Write(material.X);
                binWrite.Write(material.Y);
                binWrite.Write(material.Z);
                binWrite.Write(material.JobNr);
                binWrite.Write(material.IndexNr);
                binWrite.Write(material.UnloadingLoacation);
                binWrite.Write(material.HasPaper);
                binWrite.Write(material.Qty);

                _adsClient.Write(hcomplexStruct, dataStream);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_SetTrayInPlace(int towerNr, int windowNr, int tray, int place, int placeNr)
        {
            return WRITE_Command(ADSSPrefix.TowerPrefix_ForTower(towerNr) + "HMI_Command_" + WindowNr
                , CommandDictionary.CMD_SET_TRAY_ON_LOCATION
                , (short)tray
                , (short)place
                , (short)placeNr
                , 0
                , (short)WindowNr);
        }



        public bool WRITE_SetTrayInPlace(int tray, int place, int placeNr)
        {
            //if(_cfg.GetParamBool("TOWER_GlobalTransport")  && false)
            //{
            //    if(place == 0)
            //    {
            //        placeNr = GetGlobalWindowNr(placeNr, 0); // podmieniam okno na globalne dla windy
            //    }

            //    return WRITE_Short(".G_TrayInWindow[" + placeNr + "]", (short)tray);
            //}


            return WRITE_Command(CommandDictionary.CMD_SET_TRAY_ON_LOCATION, (short)tray, (short)place, (short)placeNr, 0, (short)WindowNr);

            //String symbol = "";
            //switch (place)
            //{
            //    case 0:
            //        symbol = _adss.SET_TRAY_IN_EXTRACTOR;
            //        break;
            //    case 1:
            //        symbol = _adss.SET_TRAY_IN_WINDOW_1;
            //        break;
            //    case 2:
            //        symbol = _adss.SET_TRAY_IN_WINDOW_2;
            //        break;
            //    case 3:
            //        symbol = _adss.SET_TRAY_IN_TABLE;
            //        break;
            //    default:
            //        return false;
            //}

            //try
            //{
            //    _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(symbol), Convert.ToInt16(tray));
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        public STR_Material READ_Material(string variable)
        {
            try
            {
                AdsStream ads = new AdsStream(228);

                int hcomplexStruct = _adsClient.CreateVariableHandle(variable);
                var v = _adsClient.Read(hcomplexStruct, ads);
                BinaryReader br = new BinaryReader(ads);
                BTPTwinCatADS.STR.STR_Material data = new BTPTwinCatADS.STR.STR_Material();
                data.Attribute1 = new string(br.ReadChars(51));
                data.Attribute2 = new string(br.ReadChars(51));
                data.Index = new string(br.ReadChars(51));
                data.Job = new string(br.ReadChars(51));

                data.X = br.ReadInt32();
                data.Y = br.ReadInt32();
                data.Z = br.ReadInt32();
                data.JobNr = br.ReadInt16();
                data.IndexNr = br.ReadInt16();
                data.UnloadingLoacation = br.ReadInt16();
                data.HasPaper = br.ReadInt16();
                data.Qty = br.ReadInt16();

                data.Attribute1 = data.Attribute1.Substring(0, data.Attribute1.IndexOf('\0'));
                data.Attribute2 = data.Attribute2.Substring(0, data.Attribute2.IndexOf('\0'));
                data.Index = data.Index.Substring(0, data.Index.IndexOf('\0'));
                data.Job = data.Job.Substring(0, data.Job.IndexOf('\0'));

                return data;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 0 = brak force
        /// 1 = force on
        /// -1 = force off
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <param name="value"></param>
        public bool WRITE_ForceIO(int id, bool input, int value)
        {
            AdsStream dataStream = new AdsStream(6);
            BinaryWriter binWrite = new BinaryWriter(dataStream);
            int hcomplexStruct = _adsClient.CreateVariableHandle(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_SetForce");

            dataStream.Position = 0;

            STR.STR_HMI_CMD_ForceIO fio = new BTPTwinCatADS.STR.STR_HMI_CMD_ForceIO(Convert.ToInt16(id), (short)(input ? 1 : 0), Convert.ToInt16(value));

            if (!ForceIOToBinWrite(fio, binWrite))
                return false;
            try
            {
                _adsClient.Write(hcomplexStruct, dataStream);

                return true;
            }
            catch (Exception err)
            {

                return false;

            }

        }


        public bool WRITE_SwitchTrayGlobal(int trayA, int trayB, int window, bool forceTransport)
        {
            try
            {
                return WRITE_Command(ADSSPrefix.MainPrefix + "HMI_Commands[" + _cfg.WindowNr + "]",
                    CommandDictionary.CMD_SWITCH,
                    (short)trayA,
                    (short)trayB,
                    _towersGlobalWindowsConfig[_cfg.TowerNrCfg][window],
                    forceTransport ? (short)1 : (short)0, 0);
            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_SwitchTray(int trayIDA, int positionA, int columnA, int trayIDB, int positionB, int columnB, int window)
        {
            try
            {
                return WRITE_Command(CommandDictionary.CMD_SWITCH,
                    Convert.ToInt16(trayIDA),
                    Convert.ToInt16(positionA + columnA * 1000),
                    Convert.ToInt16(trayIDB),
                    Convert.ToInt16(positionB + columnB * 1000),
                    Convert.ToInt16(window));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_MoveTrayGlobal(int trayID, int position, int column, int tower)
        {
            try
            {
                return WRITE_Command(ADSSPrefix.MainPrefix + "HMI_Commands[" + _cfg.WindowNr + "]",
                    CommandDictionary.CMD_MOVE,
                    Convert.ToInt16(trayID),
                    Convert.ToInt16(position),
                    Convert.ToInt16(column),
                    Convert.ToInt16(tower),
                    Convert.ToInt16(_windowNr));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_MoveTray(int trayID, int position, int column)
        {
            try
            {
                return WRITE_Command(CommandDictionary.CMD_MOVE, Convert.ToInt16(trayID), Convert.ToInt16(position), Convert.ToInt16(column), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_DeleteTrayGlobal(int trayID)
        {

            try
            {
                return WRITE_Command(ADSSPrefix.MainPrefix + "HMI_Commands[" + _cfg.WindowNr + "]",
                    CommandDictionary.CMD_DEL_TRAY,
                    Convert.ToInt16(trayID),
                    Convert.ToInt16(_windowNr),
                    Convert.ToInt16(_windowNr),
                    Convert.ToInt16(_windowNr),
                    Convert.ToInt16(_windowNr));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_DeleteTray(int trayID)
        {
            if (!_cfg.GetParamBool("PLC_Enable")) // PLC Offline
            {
                ////Zmiana polki w
                //STR.PLCValue plcValue = new STR.PLCValue();
                //plcValue.Name = "Tryb_Offline_PLC_Delete_Tray";
                //plcValue.Value = trayID;
                //ValueChanged(null, new BTPTwinCatADS.STR.PLCValueEventArgs(plcValue));

                PlcValueObservable.Instance.ForceFireValueChanged("Tryb_Offline_PLC_Delete_Tray", trayID);

                return true;
            }


            try
            {
                return WRITE_Command(CommandDictionary.CMD_DEL_TRAY, Convert.ToInt16(trayID), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_EditTray(int trayID, int position, int column, int height, int locked)
        {
            if (!_cfg.GetParamBool("PLC_Enable")) // PLC Offline
            {
                //STR.PLCValue plcValue = new STR.PLCValue();
                //plcValue.Name = "Tryb_Offline_PLC_Edit_Tray";
                //plcValue.Value = new int[] { trayID, position, column, height, 1 };

                //ValueChanged(null, new BTPTwinCatADS.STR.PLCValueEventArgs(plcValue));

                PlcValueObservable.Instance.ForceFireValueChanged("Tryb_Offline_PLC_Edit_Tray", new int[] { trayID, position, column, height, 1 });

                return true;
            }


            try
            {
                return WRITE_Command(CommandDictionary.CMD_EDIT_TRAY, Convert.ToInt16(trayID), Convert.ToInt16(position), Convert.ToInt16(column), Convert.ToInt16(height), Convert.ToInt16(locked));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_EditTrayGlobal(int trayID, int position, int column, int tower, int height)
        {

            try
            {
                return WRITE_Command(ADSSPrefix.MainPrefix + "HMI_Commands[" + _cfg.WindowNr + "]",
                    CommandDictionary.CMD_EDIT_TRAY,
                    Convert.ToInt16(trayID),
                    Convert.ToInt16(position),
                    Convert.ToInt16(column),
                    Convert.ToInt16(tower),
                    Convert.ToInt16(height));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_AddTrayGlobal(int trayID, int position, int column, int tower, int window)
        {

            try
            {
                return WRITE_Command(ADSSPrefix.MainPrefix + "HMI_Commands[" + _cfg.WindowNr + "]",
                    CommandDictionary.CMD_ADD_TRAY_FULL,
                    Convert.ToInt16(trayID),
                    Convert.ToInt16(position),
                    Convert.ToInt16(column),
                    Convert.ToInt16(tower),
                    _towersGlobalWindowsConfig[_cfg.TowerNrCfg][window]
                    );
            }
            catch
            {
                return false;
            }

        }


        public bool WRITE_AddTray(int trayID, int position, int column, int height, int locked)
        {
            if (!_cfg.GetParamBool("PLC_Enable")) // PLC Offline
            {
                //STR.PLCValue plcValue = new STR.PLCValue();
                //plcValue.Name = "Tryb_Offline_PLC_Add_Tray";
                //plcValue.Value = new int[] { trayID, position, column, height, 1 };
                //ValueChanged(null, new BTPTwinCatADS.STR.PLCValueEventArgs(plcValue));

                PlcValueObservable.Instance.ForceFireValueChanged("Tryb_Offline_PLC_Add_Tray", new int[] { trayID, position, column, height, 1 });

                return true;
            }

            try
            {
                return WRITE_Command(CommandDictionary.CMD_ADD_TRAY_FULL, Convert.ToInt16(trayID), Convert.ToInt16(position), Convert.ToInt16(column), Convert.ToInt16(height), Convert.ToInt16(locked));
            }
            catch
            {
                return false;
            }

        }

        public bool WRITE_ImportTrays(short[] trays)
        {
            int hArr = _adsClient.CreateVariableHandle(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_TrayImport");
            try
            {
                _adsClient.WriteAny(hArr, trays);
                return true;
            }
            catch (Exception err)
            {
                //OnError(err.Message);
                return false;
            }
        }

        public bool WRITE_CM_Mode()
        {
            try
            {
                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_CM_Mode"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public bool WRITE_AddTray(int trayID)
        //{
        //    try
        //    {
        //        return WRITE_Command(CommandDictionary.CMD_ADD_TRAY, Convert.ToInt16(trayID), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr), Convert.ToInt16(_windowNr));
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}

        public bool WRITE_Command(String variable, short cmd, short par1, short par2, short par3, short par4, short par5)
        {
            // log
            if (_log != null)
            {
                String log = //"[" + variable + "] " + "CMD: " + cmd.ToString()
                    "CMD: " + cmd.ToString()
                    + " | " + par1.ToString()
                    + " | " + par2.ToString()
                    + " | " + par3.ToString()
                    + " | " + par4.ToString()
                    + " | " + par5.ToString();
                //_log.AddLog(log, 999);

                _log.AddLog_CMDRaw(variable.GetValuePrefix(), log);

                if (cmd == 1 && par1 != 0)
                    _log.Stats.TrayUsed(par1);
            }

            //Obsługa trybu OFFLINE PLC
            if (!_cfg.GetParamBool("PLC_Enable") && cmd == 1)
            {
                PlcValueObservable.Instance.ForceFireValueChanged("Tryb_Offline_PLC_Start_Transport", par1);
                return true;
            }


            AdsStream dataStream = new AdsStream(16);
            BinaryWriter binWrite = new BinaryWriter(dataStream);
            int hcomplexStruct = _adsClient.CreateVariableHandle(variable);


            dataStream.Position = 0;
            try
            {
                //Fill stream according to the order with data from the text boxes
                DateTime Data = DateTime.Now;
                int IndexCmd = Data.Hour * 60 * 6000 + Data.Minute * 6000 + Data.Second * 100 + Data.Millisecond / 10;

                STR.STR_HMI_CMD command = new STR.STR_HMI_CMD(
                    IndexCmd,
                    cmd,
                    par1,
                    par2,
                    par3,
                    par4,
                    par5);

                if (!CmdToBinWrite(command, binWrite))
                    return false;

                //Write complete stream in the PLC
                _adsClient.Write(hcomplexStruct, dataStream);
                _lastCommandIndex = IndexCmd;
                return true;
            }
            catch (Exception err)
            {
                return false;

            }
            //return ret;
        }

        public bool WRITE_Command(short cmd, short par1, short par2, short par3, short par4, short par5)
        {
            if (_cfg.GetParamBool("TOWER_GlobalTransport") && cmd == BTPTwinCatADS.CommandDictionary.CMD_GET_TRAY)
                return WRITE_GlobalTransportTray(par2, par1);
            else //UWAGA MAIN pisze po oknie nr. 5! (wyłączone zaczytywanie lokalne z regałów)
                return WRITE_Command(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_Command_" + _cfg.WindowNr, cmd, par1, par2, par3, par4, par5);

        }

        /// <summary>
        /// Zapisuje komende transportu na PLC
        /// </summary>
        /// <param name="window"></param>
        /// <param name="tray"></param>
        /// <returns></returns>
        public bool WRITE_GlobalTransportTray(int window, int tray)
        {
            try
            {
                int index = _towersGlobalWindowsConfig[_cfg.TowerNrCfg][window];
                String varName = ADSSPrefix.MainPrefix + "HMI_TrayToWindowCMD[" + index + "]";

                String log = "[" + varName + "] := " + tray.ToString();
                _log.AddLog_CMDRaw("-", log);

                return WRITE_Short(varName, (short)tray);

            }
            catch
            {
                return false;
            }
        }

        public bool WRITE_WeightZero()
        {
            try
            {
                if (!_cfg.GetParamBool("PLC_Enable"))
                {
                    Random randomGenerator = new Random();
                    int weight = randomGenerator.Next() % 1000 + 100;

                    short[] signalValue = new short[20];
                    signalValue[0] = Convert.ToInt16(weight);
                    signalValue[12] = 256;// zeby okno safety sie nie pokazywalo

                    PlcValueObservable.Instance.ForceFireValueChanged(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_Signals",
                        signalValue);
                    PlcValueObservable.Instance.ForceFireValueChanged(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_Weight",
                        signalValue[0]);

                    return true;
                }



                _adsClient.WriteSymbol(_adsClient.ReadSymbolInfo(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_CMD_WeightZero"), "True");
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion


        private bool CmdToBinWrite(STR.STR_HMI_CMD cmd, BinaryWriter bw)
        {
            try
            {
                bw.Write(cmd.Index);
                bw.Write(cmd.ID_cmd);
                for (int i = 0; i < cmd.Param.Length; i++)
                    bw.Write(cmd.Param[i]);

                return true;
            }
            catch
            {
                return false;
            }

        }


        private bool ForceIOToBinWrite(STR.STR_HMI_CMD_ForceIO fio, BinaryWriter bw)
        {
            try
            {
                bw.Write(fio.ID);
                bw.Write(fio.IsInput);
                bw.Write(fio.Value);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public short[] READ_ShortTable(string name, int count)
        {
            //byte[] data = _binRead.ReadBytes(count * 2);
            //int j = 0;
            //short[] val = new short[count];
            //for (int i = 0; i < count * 2; i++)
            //{
            //    val[j] = BitConverter.ToInt16(new byte[] { data[i], data[i + 1] }, 0);
            //    j++;
            //    i++;
            //}
            //plcV.Value = val;
            try
            {
                AdsStream dataStream = new AdsStream(count * 2);
                AdsBinaryReader binReader = new AdsBinaryReader(dataStream);
                _adsClient.Read(_adsClient.CreateVariableHandle(name), dataStream);
                byte[] data = binReader.ReadBytes(count * 2);
                int j = 0;
                short[] val = new short[count];
                for (int i = 0; i < count * 2; i++)
                {
                    val[j] = BitConverter.ToInt16(new byte[] { data[i], data[i + 1] }, 0);
                    j++;
                    i++;
                }
                return val;
            }
            catch
            {
                return null;
            }
        }

        public int READ_Int(String name)
        {
            try
            {
                AdsStream dataStream = new AdsStream(4);
                AdsBinaryReader binReader = new AdsBinaryReader(dataStream);
                _adsClient.Read(_adsClient.CreateVariableHandle(name), dataStream);
                return binReader.ReadInt32();

            }
            catch
            {
                return 0;
            }
        }

        public int[,] READ_IntTable(string name, int x, int y)
        {
            try
            {
                int count = x * y;
                AdsStream dataStream = new AdsStream(count * 4);
                AdsBinaryReader binReader = new AdsBinaryReader(dataStream);
                _adsClient.Read(_adsClient.CreateVariableHandle(name), dataStream);
                int[,] val = new int[x, y];

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        val[i, j] = binReader.ReadInt32();
                    }
                }


                return val;
            }
            catch
            {
                return null;
            }

        }

        public int[] READ_IntTable(string name, int count)
        {

            try
            {
                AdsStream dataStream = new AdsStream(count * 4);
                AdsBinaryReader binReader = new AdsBinaryReader(dataStream);
                _adsClient.Read(_adsClient.CreateVariableHandle(name), dataStream);
                int[] val = new int[count];
                for (int i = 0; i < count; i++)
                {
                    val[i] = binReader.ReadInt32();
                }


                return val;
            }
            catch
            {
                return null;
            }
        }

        public short READ_Short(string name)
        {
            try
            {
                AdsStream dataStream = new AdsStream(2);
                AdsBinaryReader binReader = new AdsBinaryReader(dataStream);
                _adsClient.Read(_adsClient.CreateVariableHandle(name), dataStream);
                return binReader.ReadInt16();

            }
            catch
            {
                return 0;
            }
        }

        public object[] READ_Correction()
        {
            //ZeroPointCorrection : INT; (* korekta punktu 0 enkodera pionowego *)
            //WindowEncCorrection : ARRAY [1..5] OF INT; (* korekta polozenia okna *)
            //PostionEncCorrection : ARRAY [1..2,1..200] OF INT; (* korekta połozenia prowadnic *)
            try
            {
                short[] t = READ_ShortTable(ADSSPrefix.TowerPrefix_ForControlTower + "MEMO_Tower_Corrections", 406);
                short zpc = t[0];
                short[] wc = new short[5];
                for (int i = 1; i <= 5; i++)
                {
                    wc[i - 1] = t[i];
                }

                short[] cc1 = new short[200];
                short[] cc2 = new short[200];
                for (int i = 0; i < 200; i++)
                {
                    cc1[i] = t[i + 6];
                    cc2[i] = t[i + 206];
                }

                return new object[] { zpc, wc, cc1, cc2 };
            }
            catch
            {
                return null;
            }


        }

        /// <summary>
        /// Czyta korekte
        /// </summary>
        /// <param name="type">typ 0-zp, 1-wp, 2-p</param>
        /// <param name="p1">nr kolumny/okna</param>
        /// <param name="p2">nr pozycji</param>
        /// <returns></returns>
        public int READ_Correction(int type, int p1, int p2)
        {
            try
            {
                int hVar = -1;
                switch (type)
                {
                    case 0:
                        hVar = _adsClient.CreateVariableHandle(ADSSPrefix.TowerPrefix_ForControlTower + "MEMO_Tower_Corrections.ZeroPointCorrection");
                        break;
                    case 1:
                        hVar = _adsClient.CreateVariableHandle(ADSSPrefix.TowerPrefix_ForControlTower + "MEMO_Tower_Corrections.WindowEncCorrection[" + p1 + "]");
                        break;
                    case 2:
                        if (p1 >= 1 && p1 <= 2 && p2 >= 1 && p2 <= 100)
                            hVar = _adsClient.CreateVariableHandle(ADSSPrefix.TowerPrefix_ForControlTower + "MEMO_Tower_Corrections.PostionEncCorrection[" + p1 + ", " + p2 + "]");
                        else
                            return -666;
                        break;
                    default:
                        return -666;
                }

                if (hVar != -1)
                {
                    AdsStream dataStream = new AdsStream(2);
                    AdsBinaryReader binReader = new AdsBinaryReader(dataStream);
                    _adsClient.Read(hVar, dataStream);
                    int rv = binReader.ReadInt16();
                    dataStream.Position = 0;
                    return rv;
                }
                else
                    return -666;
            }
            catch
            {
                return -666;
            }
        }


        public uint[] READ_Statistics()
        {
            uint[] rv = new uint[15];
            try
            {

                // AdsStream which gets the data
                AdsStream dataStream = new AdsStream(15 * 4);
                BinaryReader binRead = new BinaryReader(dataStream);

                //read comlpete Array 
                int hVar = _adsClient.CreateVariableHandle(ADSSPrefix.TowerPrefix_ForCfgTower + "MEMO_Tower_Statistics");
                _adsClient.Read(hVar, dataStream);

                dataStream.Position = 0;
                for (int i = 0; i < 15; i++)
                {
                    rv[i] = binRead.ReadUInt32();
                }

            }
            catch
            {

            }
            return rv;
        }

        public string MEMO_ConfigTable
        {
            get
            {
                return ADSSPrefix.TowerPrefix_ForControlTower + "MEMO_ConfigTable";
            }
        }


        #region Read symbol data from PLC
        private TcAdsSymbolInfo[] GetAllHMI_VariablesNames()
        {
            try
            {
                TcAdsSymbolInfoLoader tcAdsSymbolInfoLoader;

                tcAdsSymbolInfoLoader = _adsClient.CreateSymbolInfoLoader();

                var q = tcAdsSymbolInfoLoader.GetSymbols(true);

                List<TcAdsSymbolInfo> vs = new List<TcAdsSymbolInfo>();

                GetAllVariablesNames_DrillBigType(ref vs, q);

                return vs.ToArray();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Wczytanie zmiennych z ADST_BIGTYPE
        /// </summary>
        /// <param name="vs"></param>
        /// <param name="col"></param>
        private void GetAllVariablesNames_DrillBigType(ref List<TcAdsSymbolInfo> vs, TcAdsSymbolInfoCollection col)
        {
            //PLC zwraca FB/STR itp jako ADST_BIGTYPE
            var q2 = col.Cast<TcAdsSymbolInfo>()
                .Where(r => r.Datatype == AdsDatatypeId.ADST_BIGTYPE
                    && !r.Type.Contains("TON")
                    && !r.Type.Contains("TOF"));

            vs.AddRange(col.Cast<TcAdsSymbolInfo>()
                .Where(r => r.Name.Contains("HMI"))
                .Select(r => r));

            if (q2.Any())
            {
                foreach (var value in q2)
                {
                    GetAllVariablesNames_DrillBigType(ref vs, value.SubSymbols);
                }
            }
        }
        #endregion

        #region Create notification on DB values
        /// <summary>
        /// Nazwy zmiennych nie zmapowanych z PLC, a będących w DB
        /// </summary>
        public string[] PlcDbMappingDiff { get; set; }

        /// <summary>
        /// Automatyczne tworzenie połączenia z PLC na podstawie danych w DB
        /// </summary>
        /// <returns></returns>
        public string CreateNotificationEventHandlers_AUTO()
        {
            try
            {
                //Odczytanie wszystkich zmiennych HMi z PLC
                TcAdsSymbolInfoLoader tcAdsSymbolInfoLoader;
                tcAdsSymbolInfoLoader = _adsClient.CreateSymbolInfoLoader();
                var plcVariables = GetAllHMI_VariablesNames();

                var q = plcVariables.Select(r => r.Name);
                //Odczytanie zmiennych z DB
                var cfgVariablesNames = _cfg.GetPlcValues().Select(r => r.Nazwa).ToArray();

                //Odczytanie listy zmiennych do połączenia z APP
                var valuesToMapp = plcVariables.Cast<TcAdsSymbolInfo>()
                    .Where(r => cfgVariablesNames.Contains(r.Name, StringComparer.CurrentCultureIgnoreCase));

                //Sprawdzenie czy wszystkie zmienne z DB zostały podpięte
                var valuesNames = valuesToMapp.Select(r => r.Name).ToArray();
                var diff = cfgVariablesNames.AsEnumerable()
                    .Except(valuesToMapp.AsEnumerable().Select(r => r.Name),
                        StringComparer.CurrentCultureIgnoreCase)
                    .ToArray();

                PlcDbMappingDiff = diff;

                //Dodanie powiadomień od zmiany
                _dataStream = new AdsStream();
                _binRead = new BinaryReader(_dataStream, System.Text.Encoding.ASCII);
                _hConnect.Clear();
                _structureDataForMappedBigTypes.Clear();
                int addr = 0;

                foreach (var symbol in valuesToMapp)
                {
                    //Obsługa zmiennych typu BIGTYPE
                    if (symbol.Datatype == AdsDatatypeId.ADST_BIGTYPE)
                    {
                        if (symbol.IsArray) //Zmienna jest tablicą AdsBigType
                        {
                            //Nazwa struktury z PLC
                            string typeName = symbol.SubSymbols.Cast<TcAdsSymbolInfo>()
                                .Select(r => r)
                                .First().Type;

                            //Wszystkie zmienne ze struktury
                            var subSymbolsData = symbol.SubSymbols.Cast<TcAdsSymbolInfo>()
                                .Select(r => r)
                                .First().SubSymbols.Cast<TcAdsSymbolInfo>().ToArray();

                            //Czy już zmienną tego typu dodaną
                            var valueInDictionary = _structureDataForMappedBigTypes.Values.AsEnumerable()
                                .Where(r => r.VariableTypeName == typeName);

                            if (valueInDictionary.Any()) //Mam doddaną
                                _structureDataForMappedBigTypes.Add(symbol.Name, valueInDictionary.First());
                            else //Tworzę nową
                                _structureDataForMappedBigTypes.Add(symbol.Name, new AdsBigType(subSymbolsData, typeName));
                        }
                        else //Zmienna jest tylko AdsBigType
                        {
                            //Nazwa struktury z PLC
                            string typeName = symbol.Type;

                            //Wszystkie zmienne ze struktury
                            var subSymbolsData = symbol.SubSymbols.Cast<TcAdsSymbolInfo>().ToArray();

                            //Czy już zmienną tego typu dodaną
                            var valueInDictionary = _structureDataForMappedBigTypes.Values.AsEnumerable()
                                .Where(r => r.VariableTypeName == typeName);

                            if (valueInDictionary.Any()) //Mam doddaną
                                _structureDataForMappedBigTypes.Add(symbol.Name, valueInDictionary.First());
                            else //Tworzę nową
                                _structureDataForMappedBigTypes.Add(symbol.Name, new AdsBigType(subSymbolsData, typeName));
                        }
                    }

                    bool isArray = symbol.Datatype == AdsDatatypeId.ADST_BIGTYPE && symbol.IsArray
                        || symbol.Datatype != AdsDatatypeId.ADST_BIGTYPE && symbol.SubSymbolCount > 0;

                    //Dodanie powiadomień
                    CreateNotificationEventHandler(
                        symbol.Name,
                        addr,
                        symbol.Size,
                        symbol.Name.Equals(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_LifeWord", StringComparison.CurrentCultureIgnoreCase) ? 500 : 200,
                        symbol.Name.Equals(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_LifeWord", StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        GetTybeByAdsDatatypeId(symbol.Datatype),
                        isArray ? symbol.SubSymbolCount : 0);// jak tablica to ilość zmiennych, w innym wypadku 0 -> dla AdsBigType

                    addr += symbol.Size;
                }

                _adsClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);

                //List<string> vs = new List<string>();

                //foreach(var val in plcVariables)
                //{

                //}

                //var q4 = vs.Distinct();

                return "OK";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Zwraca typ c# na podstawie typu z PLC
        /// </summary>
        /// <param name="adsDatatypeId"></param>
        /// <returns></returns>
        private Type GetTybeByAdsDatatypeId(AdsDatatypeId adsDatatypeId)
        {
            switch (adsDatatypeId)
            {
                case AdsDatatypeId.ADST_BIT:
                    return typeof(Boolean);
                case AdsDatatypeId.ADST_INT16:
                    return typeof(short);
                case AdsDatatypeId.ADST_INT32:
                    return typeof(int);
                case AdsDatatypeId.ADST_INT64:
                    return typeof(long);
                case AdsDatatypeId.ADST_INT8:
                    return typeof(sbyte);
                case AdsDatatypeId.ADST_REAL32:
                    return typeof(float);
                case AdsDatatypeId.ADST_REAL64:
                    return typeof(double);
                case AdsDatatypeId.ADST_UINT16:
                    return typeof(ushort);
                case AdsDatatypeId.ADST_UINT32:
                    return typeof(uint);
                case AdsDatatypeId.ADST_UINT64:
                    return typeof(ulong);
                case AdsDatatypeId.ADST_UINT8:
                    return typeof(byte);
                case AdsDatatypeId.ADST_STRING:
                    return typeof(string);
                case AdsDatatypeId.ADST_BIGTYPE:
                    return typeof(AdsBigType);
                default:
                    return typeof(Nullable);
            }
        }
        #endregion

        #region AdsBigType
        /// <summary>
        /// Wszystkie zmienne typu BigType zmapowane z APP
        /// </summary>
        Dictionary<string, AdsBigType> _structureDataForMappedBigTypes = new Dictionary<string, AdsBigType>();

        /// <summary>
        /// Zwraca listę obiektów dla zmiennej typu AdsBigType
        /// </summary>
        /// <param name="values"></param>
        /// <param name="sizes"></param>
        /// <param name="counts"></param>
        /// <param name="dataOffset"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private object GetDataFromBigVariable(List<object> values, List<int> sizes, List<int> counts, ref int dataOffset, ref Byte[] data)
        {
            List<object> rv = new List<object>();
            for (int i = 0; i < values.Count; i++)
            {
                var bigType = values[i] as AdsBigType;

                if (bigType != null) //Struktura w strukturze
                {
                    //Zwróci tablice ze wszystkich obiektów w tablicy
                    rv.Add(GetDataFromBigVariable(
                        bigType.Values,
                        bigType.Size,
                        bigType.Count,
                        ref dataOffset,
                        ref data));
                }
                else
                {
                    //Kroje bajty
                    byte[] slicedData = data.Slice(dataOffset, sizes[i]);
                    //Jaka długość 1 wartości
                    int bytesLenght = GetByteLenghtByType(values[i].GetType());
                    //Jak string to biorę całość
                    if (bytesLenght == -1)
                        bytesLenght = sizes[i];
                    //Odczytanie zmiennej jako dynamic -> żeby zwróciło dokładny typ a nie object
                    Type t = values[i].GetType().IsArray ? values[i].GetType().GetElementType() : values[i].GetType();
                    dynamic temp_dyn;

                    if (t == typeof(string)) //bo string nie ma pustego konstruktora new()
                        temp_dyn = "";
                    else
                        temp_dyn = t.Assembly.CreateInstance(t.FullName);
                    
                    var val = PLCUtilities_Generic.ReadPlcValue_ByBytesArr(
                        temp_dyn,
                        slicedData,
                        counts[i],
                        bytesLenght);

                    rv.Add(values[i].GetType().IsArray ? val : val[0]);
                    //Przesunięcie offsetu
                    dataOffset += sizes[i];
                }
            }
            return rv.ToArray();
        }

        /// <summary>
        /// Zwraca długość bajtów dla typu zmiennej c#
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private int GetByteLenghtByType(Type t)
        {
            if (t.IsArray)
            {
                t = t.GetElementType();
            }


            if (t == typeof(byte) || t == typeof(sbyte) || t == typeof(bool))
            {
                return 1;
            }
            else if (t == typeof(short) || t == typeof(ushort))
            {
                return 2;
            }
            else if (t == typeof(int) || t == typeof(uint) || t == typeof(float))
            {
                return 4;
            }
            else if (t == typeof(string))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Przechowuje strukturę zmiuennych dla ADST_BIGTYPE -> struktur, FB, ITP.
        /// </summary>
        class AdsBigType
        {
            public AdsBigType(TcAdsSymbolInfo[] symbols, string typeName)
            {
                foreach (var val in symbols)
                    AddValesAndSizeByAdsTypeId(val);

                VariableTypeName = typeName;
            }
            /// <summary>
            /// Przechowuje domyślne wartości dla danego typu
            /// </summary>
            public List<object> Values = new List<object>();
            /// <summary>
            /// Przechowuje długość bitów dla danego typu
            /// </summary>
            public List<int> Size = new List<int>();
            /// <summary>
            /// Przechowuje ilość zmiennych z tablicy
            /// </summary>
            public List<int> Count = new List<int>();
            /// <summary>
            /// Nazwa zmiennej na PLC
            /// </summary>
            public String VariableTypeName = "";


            private void AddValesAndSizeByAdsTypeId(TcAdsSymbolInfo symbolInfo)
            {
                switch (symbolInfo.Datatype)
                {
                    case AdsDatatypeId.ADST_BIT:
                        if (!symbolInfo.IsArray)
                            Values.Add(new bool());
                        else
                            Values.Add(new bool[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_INT16:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new short());
                        else
                            Values.Add(new short[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_INT32:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new int());
                        else
                            Values.Add(new int[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_INT64:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new long());
                        else
                            Values.Add(new long[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_INT8:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new sbyte());
                        else
                            Values.Add(new sbyte[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_REAL32:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new float());
                        else
                            Values.Add(new float[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_REAL64:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new double());
                        else
                            Values.Add(new double[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_UINT16:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new ushort());
                        else
                            Values.Add(new ushort[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_UINT32:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new uint());
                        else
                            Values.Add(new uint[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_UINT64:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new ulong());
                        else
                            Values.Add(new ulong[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_UINT8:
                        if (symbolInfo.SubSymbolCount == 0)
                            Values.Add(new byte());
                        else
                            Values.Add(new byte[symbolInfo.SubSymbolCount]);
                        break;
                    case AdsDatatypeId.ADST_STRING:
                        string temp = "";
                        Values.Add(temp);
                        break;
                    case AdsDatatypeId.ADST_BIGTYPE:
                        Values.Add(new AdsBigType(symbolInfo.SubSymbols.Cast<TcAdsSymbolInfo>().ToArray(), symbolInfo.Name));
                        break;
                    default:
                        throw new Exception("Variable wrong type");
                }
                Size.Add(symbolInfo.Size);
                Count.Add(symbolInfo.SubSymbolCount);
            }
        }
        #endregion
    }
}
