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
using BTPUtilities;
using BTPTwinCatADS;

namespace BTPCLMenu
{
    public partial class BottomMain : BTPControlLibrary.MainPanel
    {
        public BottomMain()
        {


        }

        public delegate void showHideMenu(bool showLeftMenu);
        public event showHideMenu ShowMenuEvent;

        protected override void InternalInit()
        {
            base.InternalInit();
            DataContext = this;
            InitializeComponent();
        }

        public override void InputInterface(String name, Object value)
        {
            if (name == "PLC")
            {
                bool v = (bool)value;
                PLCState(v);
            }
            else if (name.EqualsIgnoreCase(ADSSPrefix.MainPrefix + "HMI_TrayInWindow"))
            {
                int[] t = ((int[])value);
                stateTextBox.Text = t[1].ToString();
            }
        }

        private void StateUpdate(string val)
        {

        }

        private void PLCState(bool val)
        {
            plcTextBox.Visibility = val == true ? Visibility.Hidden : Visibility.Visible;
            if (!val)
                BackgroundColorAnimation();

        }
        public void BackgroundColorAnimation()
        {
            ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromSeconds(1))) { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            plcTextBox.Background = new SolidColorBrush(Colors.White);
            plcTextBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
        }
        public void ChangeArrow(bool isVisible)
        {
            showHideLeftMenuButton.Content = isVisible == true ? "<" : ">";
        }
        private void showHide_Click(object sender, RoutedEventArgs e)
        {
            if (showHideLeftMenuButton.Content == ">")
                ShowMenuEvent?.Invoke(true);
            else
                ShowMenuEvent?.Invoke(false);
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            stateTextBox.Text = "111";
        }
    }
}
