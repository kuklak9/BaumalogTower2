using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BTPUtilities;

namespace BTPLogs
{
    public class Logs
    {
        public Statistics Stats = null;

        //protected BTPTranslation.Translation _translation = null;

        public void ChangeLanguage(String language)
        {
            //_translation = BTPTranslation.Translation.GetTranslation(this, "lang\\" + language + "\\");

            //if (_translation == null)
            //    return;

            //try
            //{
            //    if (_translation.TranslateTextDictionary.ContainsKey("Periods"))
            //    {
            //        Periods.Clear();
            //        for (int i = 0; i < 5; i++)
            //            Periods.Add(GetTranslatedText("Periods", i), i);
            //    }
            //}
            //catch { }

        }

        //protected String GetTranslatedText(String key, int value)
        //{
        //   // return _translation == null ? null : _translation.GetText(key, value);
        //}

        private BTPDataBase.DB_WMS_3 _dbWMS;
        private BTPConfig.Configuration _cfg;
        private LogQueue.LogQueue _logQueue;

        public const int LT_ALARM = 1;
        public const int LT_OPERATIONS = 2;
        public const int LT_LOGIN = 3;
        public const int LT_TRANSPORT = 4;

        public String Username = "-";

        public Dictionary<String, int> Periods = new Dictionary<string, int>();

        public Logs(BTPDataBase.DB_WMS_3 db, BTPConfig.Configuration cfg)
        {
            _dbWMS = db;
            _cfg = cfg;
            Stats = new Statistics(_dbWMS, _cfg);

            Periods.Add("1h", 1);
            Periods.Add("1d", 2);
            Periods.Add("1w", 3);
            Periods.Add("1M", 4);
            Periods.Add("Full", 0);

            _logQueue = new BTPLogs.LogQueue.LogQueue(_dbWMS);

            try
            {
                AddLog_Operators("Application ON");
            }
            catch { }


        }

        public DataTable GetLog(String period, bool errors, bool operations, bool login, bool transport)
        {
            List<int> types = new List<int>();
            if (errors)
                types.Add(LT_ALARM);

            if (operations)
                types.Add(LT_OPERATIONS);

            if (login)
                types.Add(LT_LOGIN);

            if (transport)
                types.Add(LT_TRANSPORT);

            try
            {
                DateTime date = new DateTime(1753, 1, 1, 00, 00, 00);
                switch (Convert.ToInt32(Periods[period]))
                {
                    case 1: // godzina
                        date = DateTime.Now.AddHours(-1);
                        break;
                    case 2: // dizen
                        date = DateTime.Now.AddDays(-1);
                        break;
                    case 3: // tydzien
                        date = DateTime.Now.AddDays(-7);
                        break;
                    case 4: // miesiac
                        date = DateTime.Now.AddMonths(-1);
                        break;

                }

                String q = "";
                if (types != null && types.Count > 0)
                {
                    foreach (var v in types)
                        q += v.ToString() + ",";
                }

                return _dbWMS.Logs.GetLogs(date, q);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public DataTable GetLog(String period, List<int> types)
        {
            try
            {
                DateTime date = new DateTime(1753, 1, 1, 00, 00, 00);
                switch (Convert.ToInt32(Periods[period]))
                {
                    case 1: // godzina
                        date = DateTime.Now.AddHours(-1);
                        break;
                    case 2: // dizen
                        date = DateTime.Now.AddDays(-1);
                        break;
                    case 3: // tydzien
                        date = DateTime.Now.AddDays(-7);
                        break;
                    case 4: // miesiac
                        date = DateTime.Now.AddMonths(-1);
                        break;
                }

                String q = "";
                if (types != null && types.Count > 0)
                {
                    foreach (var v in types)
                        q += v.ToString() + ",";
                }

                return _dbWMS.Logs.GetLogs(date, q);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        #region New logs
        private void AddLog_N(LT type, string device, string desc)
        {
            _logQueue.AddLog((int)type, desc, Username, device);
        }

        private void AddLog_N(LT type, string device, string desc, string userName)
        {
            _logQueue.AddLog((int)type, desc, userName, device);
        }

        /// <summary>
        /// Log LT.ALARM -> [Maszyna]: device | [Opis]: "Error: " + nr
        /// </summary>
        /// <param name="device"></param>
        /// <param name="nr"></param>
        public void AddLog_Alarm(string device, int nr)
        {
            AddLog_N(LT.ALARM, device.GetPrefixForLog(), "Error: " + nr.ToString());
        }

        /// <summary>
        /// Log LT.TRANSPORT -> [Maszyna]: "-" | [Opis]: desc
        /// </summary>
        /// <param name="desc"></param>
        public void AddLog_Transport(string desc)
        {
            AddLog_N(LT.TRANSPORT, "-", desc);
        }

        /// <summary>
        /// Log LT.TRANSPORT -> [Maszyna]: device | [Opis]: desc
        /// </summary>
        /// <param name="device"></param>
        /// <param name="desc"></param>
        public void AddLog_Transport(string device, string desc)
        {
            AddLog_N(LT.TRANSPORT, device.GetPrefixForLog(), desc);
        }


        /// <summary>
        /// Log LT.TRANSPORT -> [Maszyna]: "-" | [Opis]: "Transport: T:[" + tray + "] -> W:[" + window + "]"
        /// </summary>
        /// <param name="tray"></param>
        /// <param name="window"></param>
        public void AddLog_Transport(int tray, int window)
        {
            AddLog_N(LT.TRANSPORT, "-", "Transport: T:[" + tray + "] -> W:[" + window + "]");
        }

        /// <summary>
        /// Log LT.TRANSPORT -> [Maszyna]: "-" | [Opis]: prefix + ": T:[" + tray + "] -> W:[" + window + "]" + "]")
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="tray"></param>
        /// <param name="window"></param>
        public void AddLog_Transport(string prefix, int tray, int window)
        {
            AddLog_N(LT.TRANSPORT, "-", prefix + ": T:[" + tray + "] -> W:[" + window + "]" + "]");
        }

        /// <summary>
        /// Log LT.TRANSPORT -> [Maszyna]: "-" | [Opis]: prefix + " ID " + ID + ": T:[" + tray + "] -> W:[" + window + "]"
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="tray"></param>
        /// <param name="window"></param>
        /// <param name="ID"></param>
        public void AddLog_Transport(string prefix, int tray, int window, int ID)
        {
            AddLog_N(LT.TRANSPORT, "-", prefix + " ID " + ID + ": T:[" + tray + "] -> W:[" + window + "]");
        }

        /// <summary>
        /// Log LT.TRANSPORT -> [Maszyna]: "-" | [Opis]: prefix + " ID " + ID + ": T:[" + tray + "] -> W:[" + window + "]"
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="tray"></param>
        /// <param name="window"></param>
        /// <param name="ID"></param>
        public void AddLog_Transport(string prefix, int tray, int window, long ID)
        {
            AddLog_N(LT.TRANSPORT, "-", prefix + " ID " + ID + ": T:[" + tray + "] -> W:[" + window + "]");
        }

        /// <summary>
        /// Log LT.PLC_VALUES -> [Maszyna]: device | [Opis]: desc
        /// </summary>
        /// <param name="device"></param>
        /// <param name="desc"></param>
        public void AddLog_PlcVal(string device, string desc)
        {
            AddLog_N(LT.PLC_VALUES, device.GetPrefixForLog(), desc);
        }

        /// <summary>
        ///  Log LT.ARTICLES -> [Maszyna]: "-" | [Opis]: desc
        /// </summary>
        /// <param name="desc"></param>
        public void AddLog_Articles(string desc)
        {
            AddLog_N(LT.ARTICLES, "-", desc);
        }

        /// <summary>
        /// Log LT.OPERATORS -> [Maszyna]: "-" | [Opis]: desc
        /// </summary>
        /// <param name="desc"></param>
        public void AddLog_Operators(string desc)
        {
            AddLog_N(LT.OPERATORS, "-", desc);
        }

        /// <summary>
        /// Log LT.BUTTONS -> [Maszyna]: device | [Opis]: desc
        /// </summary>
        /// <param name="device"></param>
        /// <param name="desc"></param>
        public void AddLog_Button(string device, string desc)
        {
            AddLog_N(LT.BUTTONS, device.GetPrefixForLog(), desc);
        }

        /// <summary>
        /// Log LT.OTHER -> [Maszyna]: "-" | [Opis]: desc
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="userName"></param>
        public void AddLog_Other(string desc, string userName = "")
        {
            if (userName == "")
                userName = Username;

            AddLog_N(LT.OTHER, "-", desc, userName);
        }

        /// <summary>
        /// Log LT.RAW_CMD -> [Maszyna]: device | [Opis]: desc
        /// </summary>
        /// <param name="device"></param>
        /// <param name="desc"></param>
        public void AddLog_CMDRaw(string device, string desc)
        {
            AddLog_N(LT.RAW_CMD, device.GetPrefixForLog(), desc);
        }
        #endregion
    }
    public enum LT
    {
        /// <summary>
        /// ALARMY
        /// </summary>
        ALARM = 1,
        /// <summary>
        /// Komendy transportu i operacje
        /// </summary>
        TRANSPORT = 2,
        /// <summary>
        /// Operacje na operatorach i logowanie
        /// </summary>
        OPERATORS = 3,
        /// <summary>
        /// Auto mapowane zmienne z PLC
        /// </summary>
        PLC_VALUES = 4,
        /// <summary>
        /// Naciśnięcie przyciku
        /// </summary>
        BUTTONS = 5,
        /// <summary>
        /// Operacje na artykułach
        /// </summary>
        ARTICLES = 6,
        /// <summary>
        /// Inne, np. Przegląd itp.
        /// </summary>
        OTHER = 90,
        /// <summary>
        /// Komendy wysyłane do PLC -> RAW
        /// </summary>
        RAW_CMD = 99
    }
}
