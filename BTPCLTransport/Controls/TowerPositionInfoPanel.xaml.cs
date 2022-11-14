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
using BTPUtilities;
using BTPTwinCatADS;

namespace BTPCLTransport.Controls
{
    public partial class TowerPositionInfoPanel : BTPControlLibrary.MainPanel
    {
        public TowerPositionInfoPanel()
        {
            InitializeComponent();
        }

        #region InternalInit / InputInterface
        protected override void InternalInit()
        {
            if (_cfg.GetParamInt("TOWER_ControlTowersCount") > 1) //TowerTables
            {
                changeTowerUniformGrid.Visibility = Visibility.Visible;
                for (int i = 1; i <= _cfg.GetParamInt("TOWER_ControlTowersCount"); i++)
                {
                    Button bt = new Button();
                    bt.Name = "bt_" + i;
                    RegisterName(bt.Name, bt);
                    bt.Content = ToRoman(i);
                    bt.Click += TowerChange_Click;
                    changeTowerUniformGrid.Children.Add(bt);
                }
            }
            else
                changeTowerUniformGrid.Visibility = Visibility.Hidden;

            _cfg.TowerNrChanged += TowerNumberChanged;
            TowerNumberChanged(this, new BTPUtilities.IntEventArgs(_cfg.TowerNrControl));

        }
        public override void InputInterface(string name, object value)
        {
            base.InputInterface(name, value);

            if (name.EqualsIgnoreCase(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_Signals"))
            {
                short[] pp = (short[])value;
                verticalValueTextBox.Text = pp[4].ToString();
                horizontalValueTextBox.Text = pp[3].ToString();
            }
            else if (name.EqualsIgnoreCase(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_ClosestPosition"))
            {
                closestPositionValueTextBox.Text = ((short)value).ToString();
            }
        }
        #endregion

        #region Event method handling
        private void TowerChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button bt)
                {
                    int towerNr = Convert.ToInt32(bt.Name.Substring(bt.Name.Length - 1));
                    _cfg.TowerNrControl = towerNr;
                }
            }
            catch { }
        }
        private void TowerNumberChanged(object? sender, IntEventArgs e)
        {
            RefreshButtonsColors(e.number);
        }
        private void RefreshButtonsColors(int towerNr)
        {
            foreach (var button in FindVisualChilds<Button>(this))
            {
                button.Background = button.Name == "bt_" + towerNr ? Brushes.GreenYellow : Brushes.Gainsboro;
            }
        }
        #endregion

        #region Additional Method
        private string ToRoman(int number)
        {
            if (number == 10) return "X";
            else if (number == 9) return "IX";
            else if (number == 8) return "VII";
            else if (number == 7) return "VII";
            else if (number == 6) return "VI";
            else if (number == 5) return "V";
            else if (number == 4) return "IV";
            else if (number == 3) return "III";
            else if (number == 2) return "II";
            else if (number == 1) return "I";
            else return String.Empty;
        }
        #endregion

    }
}
