using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTPUtilities;

namespace BTPTwinCatADS
{
    public sealed class PlcValue
    {
        public PlcValue(string name, object value)
        {
            Name = name;
            _value = value;
        }

        public string Name { get; }

        internal object _value;
        public object Value => _value;

        public override bool Equals(object obj)
        {
            if (obj is string comapredName)
                return CompareNames(comapredName);

            if (obj is PlcValue compared)
                return Name.EqualsIgnoreCase(compared.Name);

            return false;
        }

        public bool CompareNames(string comapredName)
        {
            return this.Name.EqualsIgnoreCase(comapredName);
        }
    }

    public sealed class PlcValueObservable : IObservable<PlcValue>
    {
        #region Singleton
        private static PlcValueObservable _singletonInstance;
        public static void InitializeStaticInstance(PLC plc, BTPLogs.EventErrorLogger errorLogger, BTPConfig.Configuration cfg)
        {
            _singletonInstance = new PlcValueObservable(plc, errorLogger, cfg);
        }
        public static PlcValueObservable Instance
        {
            get
            {
                if (_singletonInstance != null)
                    return _singletonInstance;
                else
                    throw new TypeInitializationException("InitializeStaticInstance function was not invoked. Istance was not initialized."
                        , new NullReferenceException());
            }
        }
        #endregion

        private readonly List<IObserver<PlcValue>> _observers;
        private readonly PLC _plc;
        private readonly BTPLogs.EventErrorLogger _errorLogger;
        private readonly BTPConfig.Configuration _cfg;
        private readonly List<PlcValue> _valuesMemo;
        private readonly Dictionary<int, Dictionary<int, BTPUtilities.TrayPosition>> _towersPositionConfig;
        private readonly Dictionary<int, Dictionary<int, BTPUtilities.Tray>> _towersTrayConfig;

        private PlcValueObservable(PLC plc, BTPLogs.EventErrorLogger errorLogger, BTPConfig.Configuration cfg)
        {
            _plc = plc;
            _errorLogger = errorLogger;
            _cfg = cfg;

            _plc.ValueChanged += Plc_ValueChanged;
            _cfg.TowerNrChanged += _cfg_TowerNrChanged;

            _observers = new List<IObserver<PlcValue>>();
            _valuesMemo = new List<PlcValue>();
            _towersTrayConfig = new Dictionary<int, Dictionary<int, Tray>>();
            _towersPositionConfig = new Dictionary<int, Dictionary<int, TrayPosition>>();
        }

        private void _cfg_TowerNrChanged(object sender, IntEventArgs e)
        {
            ADSSPrefix.TowerNumber = e.number;

            //UWAGA: Wysyłam jeszcze raz wszystkie zmienne poza konfiguracją półek
            var q_valuesToInvoke = _valuesMemo.AsEnumerable()
                .Where(r => r.Name.StartsWith(ADSSPrefix.TowerPrefix_ForTower(e.number), StringComparison.CurrentCultureIgnoreCase)
                    && !r.Name.ToUpper().Contains("HMI_TowerConfig".ToUpper()));

            foreach (var value in q_valuesToInvoke)
                FireValueChangedToObservers(value);
        }

        private void Plc_ValueChanged(object sender, STR.PLCValueEventArgs e)
        {
            try
            {
                ChangePlcValueIfNeeded(ref e);

                string name = e.Value.Name;
                object value = e.Value.Value;

                var q_memoValue = _valuesMemo.AsEnumerable()
                        .Where(r => r.CompareNames(name));

                if (q_memoValue.Any())
                    q_memoValue.First()._value = value;
                else
                    _valuesMemo.Add(new PlcValue(name, value));

                FireValueChangedToObservers(q_memoValue.First());
            }
            catch (Exception ex)
            {
                _errorLogger.AddLog(BTPLogs.ErrorType.InputInterface, new string[]
                {
                    this.GetType().FullName.Replace("+", "."),
                    e.Value.Name,
                    e.Value.Value.ToString(),
                    ex.Message
                });
            }
        }

        private void FireValueChangedToObservers(PlcValue plcValue)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(plcValue);
                }
                catch (Exception ex)
                {
                    _errorLogger.AddLog(BTPLogs.ErrorType.InputInterface, new string[]
                    {
                            observer.GetType().FullName.Replace("+", "."),
                            plcValue.Name,
                            plcValue.Value.ToString(),
                            ex.Message
                    });
                }
            }
        }

        private void ChangePlcValueIfNeeded(ref STR.PLCValueEventArgs e)
        {
            if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_JogState"))
            {
                e.Value.Value = PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_TowerConfig"))
            {
                var towerNr = int.Parse(e.Value.Name.Substring(e.Value.Name.IndexOf('.') - 1, 1));

                RefreshTrayConfig(towerNr, (short[])e.Value.Value);

                e.Value.Value = new object[]
                {
                    _towersPositionConfig.SelectMany(r => r.Value)
                        .ToDictionary(k => k.Key, v => v.Value),
                    _towersTrayConfig.SelectMany(r => r.Value)
                        .ToDictionary(k => k.Key, v => v.Value)
                };
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_TrayState"))
            {
                short[] data = (short[])e.Value.Value;

                int[] p = new int[10];

                for (int i = 0; i < 10; i++)
                    p[i] = data[i];

                e.Value.Value = p;
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_Alarms"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_AlarmsS"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_BinarySignals"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_Di"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetIOTableFromUintArr((uint[])e.Value.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_Do"))
            {
               e.Value.Value = BTPTwinCatADS.PLCUtilities.GetIOTableFromUintArr((uint[])e.Value.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_HomingState"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_MoveToPositionState"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_State"))
            {
                e.Value.Value = Convert.ToInt32((short)e.Value.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCase(ADSSPrefix.TransferPrefix + "HMI_Alarms"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCase(ADSSPrefix.MainPrefix + "HMI_TrayInWindow"))
            {
                short[] data = (short[])e.Value.Value;

                int[] p = new int[10];

                for (int i = 0; i < 10; i++)
                    p[i] = data[i];

                e.Value.Value = p;
            }
            else if (e.Value.Name.EqualsIgnoreCase(ADSSPrefix.Loader3015Prefix + "HMI_Alarm_DWordArray"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
            else if (e.Value.Name.EqualsIgnoreCase(ADSSPrefix.MainPrefix + "HMI_Alarm"))
            {
                e.Value.Value = BTPTwinCatADS.PLCUtilities.GetBoolTableFromPLCValue(e.Value);
            }
        }

        private void RefreshTrayConfig(int towerNr, short[] data)
        {

            if (!_towersPositionConfig.ContainsKey(towerNr))
                _towersPositionConfig.Add(towerNr, new Dictionary<int, TrayPosition>());
            else
                _towersPositionConfig[towerNr] = new Dictionary<int, TrayPosition>();


            if (!_towersTrayConfig.ContainsKey(towerNr))
                _towersTrayConfig.Add(towerNr, new Dictionary<int, Tray>());
            else
                _towersTrayConfig[towerNr] = new Dictionary<int, Tray>();


            for (int i = 1; i <= 2000; i++)
            {
                int j = i / 1000 + 1; //nr kolumny 
                int k = ((i % 1000) - 1) / 5 + 1;
                int ipom = i - 1;
                int positionNr = data[ipom];
                i++;
                ipom = i - 1;
                int positionType = data[ipom];
                i++;
                ipom = i - 1;
                int columnNr = data[ipom];
                i++;
                ipom = i - 1;
                int trayId = data[ipom];
                i++;
                ipom = i - 1;
                int height = data[ipom];

                if (positionNr > 0)
                    _towersPositionConfig[towerNr].Add(positionNr + columnNr * 1000 + towerNr * 10000,
                        new BTPUtilities.TrayPosition(towerNr,
                            positionNr,
                            columnNr,
                            positionType,
                            trayId));

                if (positionNr > 0 && trayId > 0 && !_towersTrayConfig[towerNr].ContainsKey(trayId))
                    _towersTrayConfig[towerNr].Add(trayId,
                        new BTPUtilities.Tray(trayId,
                            towerNr,
                            columnNr,
                            positionNr,
                            height,
                            _cfg.GetParamInt("TOWER_TrayWeight"))); //TrayWeight
            }
        }

        internal void ForceFireValueChanged(string name, object value)
        {
            FireValueChangedToObservers(new PlcValue(name, value));
        }

        #region ObserverInterface 
        public IDisposable Subscribe(IObserver<PlcValue> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<PlcValue>> _observers;
            private IObserver<PlcValue> _observer;

            public Unsubscriber(List<IObserver<PlcValue>> observers, IObserver<PlcValue> observer)
            {
                _observer = observer;
                _observers = observers;
            }

            public void Dispose()
            {
                if (_observer != null
                    && _observers != null
                    && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }
        #endregion
    }
}
