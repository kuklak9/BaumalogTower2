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
    public partial class StationTransferControl : BTPControlLibrary.MainPanel
    {
        public StationTransferControl()
        {
            InitializeComponent();
        }

        #region InternalInit / InputInterface
        protected override void InternalInit()
        {
            base.InternalInit();

            bi_1.DefaultColor = Colors.LimeGreen;
            bi_2.DefaultColor = Colors.Red;
            bi_3.DefaultColor = Colors.Blue;
            bi_4.DefaultColor = Colors.Yellow;
        }
        public override void InputInterface(string name, object value)
        {
            base.InputInterface(name, value);

            if (name.EqualsIgnoreCase(ADSSPrefix.TowerPrefix_ForControlTower + "HMI_Signals"))
            {
                short[] v = (short[])value;
                //bi_1.Value = (v[0] >> 12 & 1) == 1;
                //bi_2.Value = (v[0] >> 11 & 1) == 1;
                //bi_3.Value = v[2] == (_cfg.TowerNrControl - 1);
                //bi_4.Value = (v[0] >> 4 & 1) == 1;
                //testowo zeby ogladac kolorki
                bi_1.Value = (v[12] >> 0 & 1) == 1;
                bi_2.Value = (v[12] >> 1 & 1) == 1;
                bi_3.Value = (v[12] >> 2 & 1) == 1;
                bi_4.Value = (v[12] >> 3 & 1) == 1;
            }
        }
        #endregion
    }

}
