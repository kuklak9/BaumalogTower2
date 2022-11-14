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
using BTPControlLibrary;
using BTPTwinCatADS;
using BTPUtilities;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace BTPCLMenu
{
    public partial class LeftMain : BTPControlLibrary.MainPanel
    {
        public LeftMain() { }

        #region InternalInit / InputInterface
        protected override void InternalInit()
        {
            base.InternalInit();
            DataContext = this;
            InitializeComponent();
            isLeftMenuVisible = true;
            LogoutButtonVisible(false);
        }
        public override void InputInterface(String name, Object value)
        {
            if (name.EqualsIgnoreCase(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_TrayState"))
            {
                if (_cfg.GetParamBool("TOWER_GlobalTransport"))
                    return;

                int[] t = ((int[])value);
            }
            else if (name.EqualsIgnoreCase(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_Signals"))
            {
                short lockedValue = ((short[])value)[1];

                int towerNr = lockedValue / 100;
                int windowNr = lockedValue - 100 * towerNr;

                if (towerNr != 0 && windowNr != 0)
                    OtherServiceIsActive(towerNr, windowNr);
            }
            else if (name.EqualsIgnoreCase(ADSSPrefix.MainPrefix + "HMI_TrayInWindow"))
            {
                if (!_cfg.GetParamBool("TOWER_GlobalTransport"))
                    return;

                int[] t = ((int[])value);
                trayLabel.Content = t[0];
            }
        }
        #endregion

        #region Fields
        public String UserName
        {
            get
            {
                return UserName;
            }
            set
            {
                operatorLabel.Content = value;
            }
        }
        public bool isLeftMenuVisible { get; set; }

        // public Dictionary<int, string> AccessLeftMenu = new Dictionary<int, string>() { { 1, "isTransport" }, { 2, "isWMS" }, { 3, "isAdministrator" }, { 4, "isService" } };
        public Dictionary<int, string> AccessLeftMenu = new Dictionary<int, string>() { { 1, "isArticle" }, { 2, "isArticle" }, { 3, "isArticle" }, { 4, "isArticle" } };
        #endregion

        #region Events
        public delegate void menuChanged(int tab);
        public event menuChanged? MenuChangedEvent;
        public delegate void menuVisible(bool isLeftMenuVisible);
        public event menuVisible? MenuVisibleEvent;
        #endregion

        #region Handling Left Menu
        private void MenuChange(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            int index = Convert.ToInt32(bt.Name.Substring(2, 1));
            if (HasAccessToLeftMenu(index))
                MenuChangeCorrect(bt, index);

        }

        private void MenuChangeCorrect(Button bt, int index)
        {
            ButtonDefaultColors();
            if (index <= 5)
                bt.Background = (SolidColorBrush)Application.Current.Resources["leftMenuButtonBackgroundSelected"];

            MenuChangedEvent?.Invoke(index);
            if (index < 5 && index > 0)
            {
                HideMenu();
            }
            else
                ShowMenu();
        }

        private bool HasAccessToLeftMenu(int index)
        {
            try
            {
                if (_loggedUser.LoggedIn)
                {
                    foreach (KeyValuePair<int, string> kvp in AccessLeftMenu)
                    {
                        if (kvp.Key == index)
                            if ((bool)_loggedUser.OperatorRights[kvp.Value].GetValue(0))
                                return true;
                    }
                    if (index == 6 || index == 7)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (index == 6 || index == 7)
                        return true;
                    else
                    {
                        BTPCLPopup.MessagePopup msg_OK = new BTPCLPopup.MessagePopup(false);
                        msg_OK.Caption = "Unrecognized  user";
                        msg_OK.Description = "Please Log in";
                        msg_OK.Owner = Window.GetWindow(this);
                        msg_OK.ShowDialog();
                        return false;
                    }
                }
            }
            catch { return false; }

        }

        private void AddWindowToActWindow(Window name)
        {
            name.Owner = Window.GetWindow(this);
            name.ShowDialog();
        }
        private void TrayReturn_Click(object sender, RoutedEventArgs e)
        {
            //odwozenie polki
        }
        #endregion

        #region Animation
        public void HideMenu()
        {
            if (this.ActualWidth == 150)
            {
                DoubleAnimation da = new()
                {
                    From = 150,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };
                da.Completed += Da_Completed;
                this.BeginAnimation(WidthProperty, da);
            }
        }
        public void ShowMenu()
        {
            if (this.ActualWidth == 0)
            {
                DoubleAnimation da = new()
                {
                    From = 0,
                    To = 150,
                    Duration = TimeSpan.FromSeconds(0.5)
                };
                da.Completed += Da_Completed;
                this.BeginAnimation(WidthProperty, da);

            }
        }
        private void Da_Completed(object? sender, EventArgs e)
        {
            isLeftMenuVisible = this.ActualWidth < 10 ? false : true;
            MenuVisibleEvent?.Invoke(isLeftMenuVisible);
        }
        #endregion

        #region Additional Methods
        public void LogoutButtonVisible(bool b)
        {
            bt6Button.Visibility = b ? Visibility.Visible : Visibility.Hidden;
            bt7Button.Visibility = !b ? Visibility.Visible : Visibility.Hidden;
        }
        public void ButtonDefaultColors()
        {
            foreach (Button bt in FindVisualChilds<Button>(this))
            {
                bt.Background = (SolidColorBrush)Application.Current.Resources["leftMenuButtonBackgroundDefault"];
            }
        }
        #endregion

        #region Service
        private void OtherServiceIsActive(int tower, int window)
        {
            int act_tower = _cfg.GetParamInt("TOWER_TowerNumber");
            int act_window = _cfg.GetParamInt("TOWER_WindowNr");

            if (tower != act_tower  && window != act_window ||
                tower == act_tower  && window != act_window ||
                tower != act_tower  && window == act_window)
            {
                serviceLockedTextBox.Text = "T. " + tower + " P. "  + window;
                bt4Button.IsEnabled = false;
                bt4Button.Background = Brushes.OrangeRed;
            }
            else
            {
                serviceLockedTextBox.Text = "";
                bt4Button.IsEnabled = true;
                bt4Button.Background = Brushes.Gainsboro;
            }
        }
        #endregion


    }
}
