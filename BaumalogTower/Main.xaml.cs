using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BTPTwinCatADS;
using System.Windows.Media.Animation;
using BTPUtilities;
using System.Collections;
using System.Windows.Threading;
using System.Reflection;

namespace BaumalogTower
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window, IObserver<PlcValue>
    {
        private BTPLogin.LoggedUser _loggedUser = null;
        private BTPLogs.EventErrorLogger _eventErrorLogger = new BTPLogs.EventErrorLogger();
        private BTPTranslation.Translation _translation = null;
        private BTPConfig.Configuration _cfg = null;
        private BTPDataBase.DB_WMS_3 _dbWMS = null;
        private BTPTwinCatADS.PLC _plc = null;
        private BTPLogs.Logs _log = null;
        private BTPCLInput.Keyboard _kb = null;
        private BTPCLInput.NumericKeyboard _nkb = null;

        private List<BTPControlLibrary.MainPanel> _mpList = new List<BTPControlLibrary.MainPanel>();
        private List<BTPControlLibrary.MainWindow> _mpListWindow = new List<BTPControlLibrary.MainWindow>();

        public BTPCLPopup.MessagePopup msg_OK= new BTPCLPopup.MessagePopup(false);
        public BTPCLPopup.MessagePopup msg_YesNo= new BTPCLPopup.MessagePopup(true);

        public Main()
        {
            InitializeComponent();
            InitEvents();
            InitTimer();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            AutoInit();
        }

        #region Events
        private void InitEvents()
        {
            leftMain1.MenuChangedEvent += LeftMain1_MenuChangedEvent;
            leftMain1.MenuVisibleEvent += LeftMain1_MenuVisibleEvent;
            bottomMain1.ShowMenuEvent += BottomMain1_ShowMenuEvent;
            login1.DoLoginEvent += Login1_DoLoginEvent;
            close1.ShowLoginControlEvent +=Close1_ShowLoginControlEvent;
        }
        #endregion

        #region Timer
        private int _plcLifeWord_memo = -1;
        private int _plcLifeWord = -1;
        private int second_timer = 0;
        private int msx100_timer = 0;
        private int _plcData_TowerState = 0;

        private void InitTimer()
        {
            DispatcherTimer tm = new();
            tm.Tick +=Tm_Tick;
            tm.Interval = new TimeSpan(0, 0, 0, 0, 100);
            tm.Start();
        }
        private void Tm_Tick(object? sender, EventArgs e)
        {
            this.TimerTick();
        }
        public void TimerTick()
        {
            //if (!_cfg.GetParamBool("PLC_Enable")) //PLCEnable
            //{
            //    if (!trayConfigUpdated)
            //    {
            //        trayConfigUpdated = true;
            //        LoadTrayCfgFromDB();
            //    }
            //    return;
            //}
            msx100_timer++;

            if (msx100_timer >= 10)
            {
                //RefreshTrayConfigInDB();

                second_timer++;
                msx100_timer = 0;

                if (second_timer % 5 == 0)
                {
                    if (_plcLifeWord_memo == _plcLifeWord)
                    {
                        _plc.Close();
                        _plc.Init();
                        bottomMain1.InputInterface("PLC", false);
                    }
                    else
                    {
                        bottomMain1.InputInterface("PLC", true);
                    }
                    _plcLifeWord_memo = _plcLifeWord;

                }

                if (second_timer >= 60)
                    second_timer = 0;


                if (_loggedUser.LoggedIn && _plcData_TowerState != 1)
                {
                    _loggedUser.LoginTimer++;
                    if (_loggedUser.LoginTimer > _cfg.GetParamInt("CLOSE_LoginTimeout") && _cfg.GetParamInt("CLOSE_LoginTimeout") > 0) //LoginTimeout LoginTimeout
                    {
                        LogoutClicked();
                    }
                }
                else
                {
                    _loggedUser.LoginTimer = 0;
                }

            }
        }
        #endregion

        #region Init
        private void Init()
        {
            CreateConfig();
            CreateDBWMS();
            _translation = new BTPTranslation.Translation(_cfg);
            CreateLog();
            _plcLogValues = _cfg.GetPlcLogValues();
            CreatePLC();
            CreateLoggedUser();


        }
        private void AutoInit()
        {
            BTPControlLibrary.MainPanel.InitFirst(_kb, _nkb, _dbWMS, _cfg, _log, _loggedUser, _plc, _translation, _eventErrorLogger);
            BTPControlLibrary.MainWindow.InitFirst(_kb, _nkb, _dbWMS, _cfg, _log, _loggedUser, _plc, _translation, _eventErrorLogger);

            foreach (BTPControlLibrary.MainPanel mp in FindVisualChilds<BTPControlLibrary.MainPanel>(this))
            {
                _mpList.Add(mp);
                mp.Init();
            }

            foreach (BTPControlLibrary.MainPanel mp in FindVisualChilds<BTPControlLibrary.MainPanel>(this))
            {
                _mpList.Add(mp);
                mp.Init();
            }
        }
        IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
            }
        }
        #endregion

        #region Creating Instances
        private void CreateLoggedUser()
        {
            _loggedUser = new BTPLogin.LoggedUser(_dbWMS);
        }

        private void CreateConfig()
        {
            _cfg = new BTPConfig.Configuration();
        }

        private void CreateDBWMS()
        {
            _dbWMS = new BTPDataBase.DB_WMS_3(_cfg.DBWMS_Local, _cfg.DBWMS_Remote, _cfg.DBWMS_Remote_new);
        }

        private void CreateLog()
        {
            _log = new BTPLogs.Logs(_dbWMS, _cfg);
        }

        private void CreatePLC()
        {
            _plc = new BTPTwinCatADS.PLC(_cfg, _cfg.TowerNrControl, _cfg.WindowNr, _log, _eventErrorLogger);
            PlcValueObservable.Instance.Subscribe(this);
            _plc.Init();
        }

        #endregion

        #region Login 
        private void Login1_DoLoginEvent(string username, string password)
        {
            if (_loggedUser.DoLogin(username, password))
            {
                SetVisibility(1);
                leftMain1.HideMenu();
                _log.Username = _loggedUser.UserName;
                leftMain1.UserName = _loggedUser.UserName;
                login1.ClearTB();
            }
            else
                login1.WrongUsernameOrPassword();
        }
        private void LogoutClicked()
        {
            if (_loggedUser.LoggedIn)
            {
                _loggedUser.DoLogOut();
                SetAfterLogOut();
                //_plc.WRITE_Bit(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_LoggedIn", false);
            }
        }
        private void SetAfterLogOut()
        {
            leftMain1.UserName = "";
            leftMain1.ButtonDefaultColors();
            login1.Visibility = Visibility.Visible;
            leftMain1.ShowMenu();
        }
        private void AnyButtonClicked(object sender, EventArgs e)
        {
            ResetLoginTimer();
        }
        public void ResetLoginTimer()
        {
            try
            {
                _loggedUser.LoginTimer = 0;
            }
            catch { }
        }
        #endregion

        #region LeftMenu
        private void BottomMain1_ShowMenuEvent(bool showLeftMenu)
        {
            if (showLeftMenu)
                leftMain1.ShowMenu();
            else
                leftMain1.HideMenu();
        }
        private void LeftMain1_MenuVisibleEvent(bool isLeftMenuVisible)
        {
            bottomMain1.ChangeArrow(isLeftMenuVisible);
        }
        private void LeftMain1_MenuChangedEvent(int tab)
        {
            try
            {
                SetVisibility(tab);
            }
            catch { }
        }
        private void Close1_ShowLoginControlEvent()
        {
            SetVisibility(6);
        }
        #endregion

        #region Menu
        private void SetVisibility(int selected)
        {
            DoubleAnimation opacityAnimationON = new()
            {
                From = 0,
                To = 1,
                Duration = (TimeSpan.FromSeconds(0.5))
            };
            DoubleAnimation opacityAnimationOFF = new()
            {
                From = 1,
                To = 0,
                Duration = (TimeSpan.FromSeconds(0.5))
            };
            switch (selected)
            {
                case 1:
                    login1.Visibility = Visibility.Hidden;
                    close1.Visibility = Visibility.Hidden;
                    transport1.Visibility = Visibility.Visible;
                    articles1.Visibility = Visibility.Hidden;
                    administrator1.Visibility = Visibility.Hidden;
                    service1.Visibility = Visibility.Hidden;
                    leftMain1.LogoutButtonVisible(true);
                    break;
                case 2:
                    login1.Visibility = Visibility.Hidden;
                    close1.Visibility = Visibility.Hidden;
                    transport1.Visibility = Visibility.Hidden;
                    articles1.Visibility = Visibility.Visible;
                    administrator1.Visibility = Visibility.Hidden;
                    service1.Visibility = Visibility.Hidden;
                    leftMain1.LogoutButtonVisible(true);
                    break;
                case 3:
                    login1.Visibility = Visibility.Hidden;
                    close1.Visibility = Visibility.Hidden;
                    transport1.Visibility = Visibility.Hidden;
                    articles1.Visibility = Visibility.Hidden;
                    administrator1.Visibility =Visibility.Visible;
                    service1.Visibility = Visibility.Hidden;
                    leftMain1.LogoutButtonVisible(true);
                    break;
                case 4:
                    login1.Visibility = Visibility.Hidden;
                    close1.Visibility = Visibility.Hidden;
                    transport1.Visibility = Visibility.Hidden;
                    articles1.Visibility = Visibility.Hidden;
                    administrator1.Visibility = Visibility.Hidden;
                    service1.Visibility = Visibility.Visible;
                    leftMain1.LogoutButtonVisible(true);
                    break;
                case 5:
                    login1.Visibility = Visibility.Hidden;
                    close1.Visibility = Visibility.Hidden;
                    transport1.Visibility = Visibility.Hidden;
                    articles1.Visibility = Visibility.Hidden;
                    administrator1.Visibility = Visibility.Hidden;
                    service1.Visibility = Visibility.Hidden;
                    leftMain1.LogoutButtonVisible(true);
                    break;
                case 6:
                    LogoutClicked();
                    login1.Visibility = Visibility.Visible;
                    close1.Visibility = Visibility.Hidden;
                    transport1.Visibility = Visibility.Hidden;
                    articles1.Visibility = Visibility.Hidden;
                    administrator1.Visibility = Visibility.Hidden;
                    service1.Visibility = Visibility.Hidden;
                    leftMain1.LogoutButtonVisible(false);
                    break;
                case 7:
                    login1.Visibility = Visibility.Hidden;
                    close1.Visibility = Visibility.Visible;
                    transport1.Visibility = Visibility.Hidden;
                    articles1.Visibility = Visibility.Hidden;
                    administrator1.Visibility = Visibility.Hidden;
                    service1.Visibility = Visibility.Hidden;
                    leftMain1.LogoutButtonVisible(false);
                    break;
            }
        }
        #endregion

        #region PLC
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        private BTPConfig.ConfTable.PLC_ZmienneRow[] _plcLogValues;

        public void OnNext(PlcValue value)
        {
            try
            {
                if (value.Name.EqualsIgnoreCaseAndPrefixIndex(ADSSPrefix.TowerPrefix_ForControlTower, "HMI_LifeWord"))
                {
                    _plcLifeWord = (short)value.Value;
                }
                if (_plcLogValues.AsEnumerable()
                   .Where(r => r.Nazwa.EqualsIgnoreCase(value.Name))
                   .Any())
                {
                    AddPlcValueChangeLog(value);
                }

                foreach (var mp in _mpList)
                    mp.InputInterface(value.Name, value.Value);
                foreach (var mp in _mpListWindow)
                    mp.InputInterface(value.Name, value.Value);
            }
            catch { }

        }
        private void AddPlcValueChangeLog(PlcValue value)
        {
            string text = "[ ";

            if (value.Value.GetType().IsArray)
            {
                var q = ((IEnumerable)value.Value).Cast<object>()
                        .Select(r => r);

                foreach (var obj in q)
                    text += obj + " | ";

                text += " ]";
            }
            else
                text += value.Value + " ]";

            string prefix = value.Name.GetValuePrefix();
            string valName = value.Name.Substring(prefix.Length + 1, value.Name.Length - prefix.Length - 1);

            _log.AddLog_PlcVal(prefix, "PLC Val " + valName + " : " + text);
        }
        #endregion 

    }
}

