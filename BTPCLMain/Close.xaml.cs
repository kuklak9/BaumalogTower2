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
using System.Windows.Media.Animation;
using BTPTwinCatADS;
using BTPUtilities;

namespace BTPCLMain
{
    /// <summary>
    /// Interaction logic for Close.xaml
    /// </summary>
    public partial class Close : BTPControlLibrary.MainPanel
    {
        public Close()
        {

        }

        protected override void InternalInit()
        {
            base.InternalInit();
            DataContext = this;
            InitializeComponent();
        }

        public override void InputInterface(String name, Object value)
        {
            if (name.EqualsIgnoreCase(ADSSPrefix.TowerPrefix_ForCfgTower + "HMI_Signals"))
            {
                bool shutdown = (((short[])value)[6] >> 0 & 1) == 1;
                if (shutdown)
                    ShutDown();
            }
        }

        #region Events

        public delegate void showLoginControl();
        public event showLoginControl? ShowLoginControlEvent;

        private void ShotDownApp_Click(object sender, RoutedEventArgs e)
        {
            if (closeAppPassword.Password == _cfg.GetParamStr("CLOSE_Password"))
                Application.Current.Shutdown();
            else
                WrongClosePassword();
        }

        private void ShutDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _plc.WRITE_Bit(ADSSPrefix.MainPrefix + "HMI_Shutdown");
            }
            catch { }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            ShowLoginControlEvent?.Invoke();
        }

        private void TV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("C:\\Baumalog\\Serwis\\TeamViewer\\TeamViewerQS.exe");
            }
            catch
            {
                MessageBox.Show("Aplikacja TeamViewer nieznaleziona.");
            }
        }
        #endregion

        #region Additional Methods
        private void ShutDown()
        {
            //var psi = new System.Diagnostics.ProcessStartInfo("shutdown", "/s /t 0");
            //psi.CreateNoWindow = true;
            //psi.UseShellExecute = false;
            //System.Diagnostics.Process.Start(psi);
        }

        public void WrongClosePassword()
        {
            ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromSeconds(0.1))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(5) };
            closeAppPassword.Background = new SolidColorBrush(Colors.White);
            closeAppPassword.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
            closeAppPassword.Password = "";
        }
        #endregion

    }
}
